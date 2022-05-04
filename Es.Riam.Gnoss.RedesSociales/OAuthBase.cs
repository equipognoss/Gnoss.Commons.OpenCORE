using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.RedesSociales
{
    /// <summary>
    /// Clase de autenticaci�n base
    /// </summary>
    public class OAuthBase
    {
        #region Enumeraciones

        /// <summary>
        /// Tipos de algoritmo de encriptaci�n que soporta el protocolo OAuth
        /// </summary>
        public enum SignatureTypes
        {
            /// <summary>
            /// HMAC-SHA1
            /// </summary>
            HMACSHA1,
            /// <summary>
            /// Texto plano
            /// </summary>
            PLAINTEXT,
            /// <summary>
            /// RSA-SHA1
            /// </summary>
            RSASHA1
        }

        #endregion

        #region Constantes

        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";

        //
        // Lista de nombres de par�metros de OAuth
        //        
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";

        #endregion

        #region Miembros

        /// <summary>
        /// Generador de n�meros aleatorios
        /// </summary>
        protected Random mRandom = new Random();

        /// <summary>
        /// Caracteres no reservados
        /// </summary>
        protected string mUnreservedChars ="a����bc�de����fghi����jklmn�o����pqrstu����vwxyzA����BC�DE����FGHI����JKLMN�O����PQRSTU����VWXYZ0123456789-_.�~��";

        #endregion

        #region M�todos

        #region Privados

        /// <summary>
        /// Genera el valor hash de los datos pasados por par�metro
        /// </summary>
        /// <param name="pHashAlgorithm">Algoritmo usado para generar el hash. Si necesita inicializaci�n (como HMAC y sus derivados) 
        /// debe ser inicializado antes de pasarlo como par�metro a esta funci�n</param>
        /// <param name="pData">Los datos de los que generar el hash</param>
        /// <returns>Un string en base 64 con el valor hash</returns>
        private string ComputeHash(HashAlgorithm pHashAlgorithm, string pData)
        {
            if (pHashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(pData))
            {
                throw new ArgumentNullException("data");
            }
            byte[] dataBuffer = System.Text.Encoding.ASCII.GetBytes(pData);
            byte[] hashBytes = pHashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Obtiene la lista de par�metros(nombre, valor) que no son par�metros OAuth (los que no comienzan por "oauth_")
        /// </summary>
        /// <param name="pParametros">La parte de la Url correspondiente a los par�metros</param>
        /// <returns>Lista de objetos QueryParameter, cada uno contiene nombre y valor del par�metro</returns>
        private List<QueryParameter> GetQueryParameters(string pParametros)
        {
            if (pParametros.StartsWith("?"))
            {
                pParametros = pParametros.Remove(0, 1);
            }
            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(pParametros))
            {
                string[] p = pParametros.Split('&');
                
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region Protegidos

        /// <summary>
        /// Normaliza los par�metros de la petici�n
        /// </summary>
        /// <param name="pParametros">Lista ordenada de par�metros</param>
        /// <returns>String que representa los par�metros normalizados</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> pParametros)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            
            for (int i = 0; i < pParametros.Count; i++)
            {
                p = pParametros[i];
                sb.AppendFormat("{0}={1}", p.Nombre, p.Valor);

                if (i < pParametros.Count - 1)
                {
                    sb.Append("&");
                }
            }
            return sb.ToString();
        }

        #endregion

        #region P�blicos

        /// <summary>
        /// Implementaci�n de UrlEncode que aplica el '%' en may�sculas, que es como lo utiliza OAuth
        /// </summary>
        /// <param name="pValue">El valor para codificar</param>
        /// <returns>String con la Url codificada</returns>
        public string UrlEncode(string pValue)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in pValue)
            {
                if (mUnreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Genera la firma base que se utilizar� para generar la firma
        /// </summary>
        /// <param name="pUrl">La url completa que se va a firmar incluyendo los par�metros que no sean de OAuth</param>
        /// <param name="pConsumerKey">Clave del consumidor</param>        
        /// <param name="pToken">El token, si est� disponible. Si no est� disponible se debe pasar NULL o string vac�o</param>
        /// <param name="pTokenSecret">El token secreto, si est� disponible. Si no est� disponible se debe pasar NULL o string vac�o</param>
        /// <param name="pHttpMethod">El m�todo HTTP usado (POST,GET,PUT, etc)</param>
        /// <param name="pSignatureType">El tipo de encriptaci�n que se usar�. Valores por defecto en <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see></param>
        /// <returns>String con la firma base</returns>
        public string GenerateSignatureBase(Uri pUrl, string pConsumerKey, string pToken, string pTokenSecret, string pHttpMethod, string pTimeStamp, string pNonce, string pSignatureType, out string pNormalizedUrl, out string pNormalizedRequestParameters)
        {
            if (pToken == null)
            {
                pToken = string.Empty;
            }

            if (pTokenSecret == null)
            {
                pTokenSecret = string.Empty;
            }

            if (string.IsNullOrEmpty(pConsumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(pHttpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(pSignatureType))
            {
                throw new ArgumentNullException("signatureType");
            }
            pNormalizedUrl = null;
            pNormalizedRequestParameters = null;

            List<QueryParameter> parameters = GetQueryParameters(pUrl.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, pNonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, pTimeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, pSignatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, pConsumerKey));

            if (!string.IsNullOrEmpty(pToken))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, pToken));
            }
            parameters.Sort(new QueryParameterComparer());

            pNormalizedUrl = string.Format("{0}://{1}", pUrl.Scheme, pUrl.Host);
            
            if (!((pUrl.Scheme == "http" && pUrl.Port == 80) || (pUrl.Scheme == "https" && pUrl.Port == 443)))
            {
                pNormalizedUrl += ":" + pUrl.Port;
            }
            pNormalizedUrl += pUrl.AbsolutePath;
            pNormalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", pHttpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(pNormalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(pNormalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Genera la firma a partir de la firma base y el algoritmo de hasheado pasados por par�metro
        /// </summary>
        /// <param name="pSignatureBase">Firma base</param>
        /// <param name="pHash">Algoritmo que se usar� para generar el hash de la firma. Si requiere inicializaci�n debe hacerse antes de llamar a este m�todo</param>
        /// <returns>Un string en base 64 con el valor hash de la firma</returns>
        public string GenerateSignatureUsingHash(string pSignatureBase, HashAlgorithm pHash)
        {
            return ComputeHash(pHash, pSignatureBase);
        }

        /// <summary>
        /// Genera una firma usando el algoritmo HMAC-SHA1
        /// </summary>		
        /// <param name="pUrl">La url completa que se va a firmar incluyendo los par�metros que no sean de OAuth</param>
        /// <param name="pConsumerKey">Clave del consumidor</param>
        /// <param name="pConsumerSecret">Clave secreta del consumidor</param>
        /// <param name="pToken">El token, si est� disponible. Si no est� disponible se debe pasar NULL o string vac�o</param>
        /// <param name="pTokenSecret">El token secreto, si est� disponible. Si no est� disponible se debe pasar NULL o string vac�o</param>
        /// <param name="pHttpMethod">El m�todo HTTP usado (POST,GET,PUT, etc)</param>
        /// <returns>Un string en base 64 con el valor hash</returns>
        public string GenerateSignature(Uri pUrl, string pConsumerKey, string pConsumerSecret, string pToken, string pTokenSecret, string pHttpMethod, string pTimeStamp, string pNonce, out string pNormalizedUrl, out string pNormalizedRequestParameters)
        {
            return GenerateSignature(pUrl, pConsumerKey, pConsumerSecret, pToken, pTokenSecret, pHttpMethod, pTimeStamp, pNonce, SignatureTypes.HMACSHA1, out pNormalizedUrl, out pNormalizedRequestParameters);
        }

        /// <summary>
        /// Genera una firma usando el tipo de encriptaci�n pasado por par�metro 
        /// </summary>		
        /// <param name="pUrl">La url completa que se va a firmar incluyendo los par�metros que no sean de OAuth</param>
        /// <param name="pConsumerKey">Clave del consumidor</param>
        /// <param name="pConsumerSecret">Clave secreta del consumidor</param>
        /// <param name="pToken">El token, si est� disponible. Si no est� disponible se debe pasar NULL o string vac�o</param>
        /// <param name="pTokenSecret">El token secreto, si est� disponible. Si no est� disponible se debe pasar NULL o string vac�o</param>
        /// <param name="pHttpMethod">El m�todo HTTP usado (POST,GET,PUT, etc)</param>
        /// <param name="pSignatureType">El tipo de encriptaci�n que se usar�</param>
        /// <returns>Un string en base 64 con el valor hash</returns>
        public string GenerateSignature(Uri pUrl, string pConsumerKey, string pConsumerSecret, string pToken, string pTokenSecret, string pHttpMethod, string pTimeStamp, string pNonce, SignatureTypes pSignatureType, out string pNormalizedUrl, out string pNormalizedRequestParameters)
        {
            pNormalizedUrl = null;
            pNormalizedRequestParameters = null;

            switch (pSignatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", pConsumerSecret, pTokenSecret));
                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(pUrl, pConsumerKey, pToken, pTokenSecret, pHttpMethod, pTimeStamp, pNonce, HMACSHA1SignatureType, out pNormalizedUrl, out pNormalizedRequestParameters);

                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.ASCII.GetBytes(string.Format("{0}&{1}", UrlEncode(pConsumerSecret), string.IsNullOrEmpty(pTokenSecret) ? "" : UrlEncode(pTokenSecret)));

                    return GenerateSignatureUsingHash(signatureBase, hmacsha1);
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Genera la marca de tiempo(timestamp) para la firma
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            //Por definici�n del protocolo OAuth debe ser la diferencia entre la fecha actual y el 1/1/1970 expresada en segundos
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Genera un Nonce a partir de una marca de tiempo (TimeStamp) pasada por par�metro
        /// </summary>
        /// <param name="pTimeStamp">TimeStamp</param>
        /// <returns>Nonce generado</returns>
        //TODO Alvaro Javier mirar
        //public virtual string GenerateNonce(string pTimeStamp)
        //{
        //    GuidNonceProvider gnp = new GuidNonceProvider();
        //    return gnp.GenerateNonce(int.Parse(pTimeStamp));
        //}

        #endregion

        #endregion

        /// <summary>
        /// Calse para los par�metros
        /// </summary>
        protected class QueryParameter
        {
            #region Miembros

            /// <summary>
            /// Nombre del par�metro
            /// </summary>
            private string mNombre = null;

            /// <summary>
            /// Valor del par�metro
            /// </summary>
            private string mValor = null;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor de
            /// </summary>
            /// <param name="pNombre">Nombre del par�metro</param>
            /// <param name="pValor">Valor del par�metro</param>
            public QueryParameter(string pNombre, string pValor)
            {
                this.mNombre = pNombre;
                this.mValor = pValor;
            }

            #endregion

            #region Propiedades

            /// <summary>
            /// Obtiene el nombre del par�metro
            /// </summary>
            public string Nombre
            {
                get { return mNombre; }
            }

            /// <summary>
            /// Obtiene el valor del par�metro
            /// </summary>
            public string Valor
            {
                get { return mValor; }
            }

            #endregion
        }

        /// <summary>
        /// Clase para comparar par�metros
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            /// <summary>
            /// Compara dos par�metros
            /// </summary>
            /// <param name="x">Par�metro X</param>
            /// <param name="y">Par�metro Y</param>
            /// <returns>Entero resultado de la comparaci�n</returns>
            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Nombre == y.Nombre)
                {
                    return string.Compare(x.Valor, y.Valor);
                }
                else
                {
                    return string.Compare(x.Nombre, y.Nombre);
                }
            }
        }
    }
}
