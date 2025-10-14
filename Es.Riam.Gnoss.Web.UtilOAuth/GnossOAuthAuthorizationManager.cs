using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Gestor de autorizaciones en Gnoss mediante OAuth
    /// </summary>
    public class GnossOAuthAuthorizationManager
    {

        #region Miembros

        public static string CadenaFicheroConfig;
        private LoggingService mLoggingService;
        private GnossCache mGnossCache;
        private IHostingEnvironment mEnv;
        private EntityContextOauth mEntityContextOauth;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private static object bloqueo = new object();
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public GnossOAuthAuthorizationManager(GnossCache gnossCache, IHostingEnvironment env, EntityContextOauth entityContextOauth, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GnossOAuthAuthorizationManager> logger, ILoggerFactory loggerFactory)
        {
            mGnossCache = gnossCache;
            mEnv = env;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mEntityContextOauth = entityContextOauth;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos generales


        /// <summary>
        /// Obtiene el usuario que realiza una petición de acceso a datos privados del usuario
        /// </summary>
        /// <param name="pRequest">Request de la petición</param>
        /// <param name="pParametrosOauth">Compone los parametros de la petición oauth en un string</param>
        /// <returns></returns>
        public string ObtenerUsuarioDePeticion(HttpRequest pRequest, out string pParametrosOauth, out string pConsumerKey, out string pConsumerSecret, out string pToken, out string pFirmaGnoss, out string pFirmaExterna, bool pDesdeJs)
        {
            string token = "";
            string consumerKey = "";
            string nonce = "";
            string method = "";
            string timespan = "";
            string signature = "";

            if (pRequest.Form.ContainsKey("oauth_token"))
            {
                token = pRequest.Form["oauth_token"];
                consumerKey = pRequest.Form["oauth_consumer_key"];
                nonce = pRequest.Form["oauth_nonce"];
                method = pRequest.Form["oauth_signature_method"];
                timespan = pRequest.Form["oauth_timestamp"];
                signature = pRequest.Form["oauth_signature"];
            }
            else if (pRequest.Method.Equals("GET"))
            {
                if (!string.IsNullOrEmpty(pRequest.Headers["oauth_token"]))
                {
                    token = pRequest.Headers["oauth_token"];
                    consumerKey = pRequest.Headers["oauth_consumer_key"];
                    nonce = pRequest.Headers["oauth_nonce"];
                    method = pRequest.Headers["oauth_signature_method"];
                    timespan = pRequest.Headers["oauth_timestamp"];
                    signature = pRequest.Headers["oauth_signature"];
                }
                else if (!string.IsNullOrEmpty(pRequest.Query["oauth_token"]))
                {
                    token = pRequest.Query["oauth_token"];
                    consumerKey = pRequest.Query["oauth_consumer_key"];
                    nonce = pRequest.Query["oauth_nonce"];
                    method = pRequest.Query["oauth_signature_method"];
                    timespan = pRequest.Query["oauth_timestamp"];
                    signature = pRequest.Query["oauth_signature"];
                }
            }
            else
            {
                //Lo obtengo del query string
                string http_authorization = "HTTP_AUTHORIZATION:OAuth ";
                string saltoLinea = "\r\n";

                string parametros = pRequest.Query["ALL_HTTP"];
                if (parametros.IndexOf(http_authorization) != -1)
                {
                    parametros = parametros.Substring(parametros.IndexOf(http_authorization) + http_authorization.Length);
                    parametros = parametros.Substring(0, parametros.IndexOf(saltoLinea));

                    char[] separadores = { ',' };
                    string[] listaParams = parametros.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string param in listaParams)
                    {
                        char[] separadoIgual = { '=' };
                        string[] claveValor = param.Split(separadoIgual, StringSplitOptions.RemoveEmptyEntries);
                        if (claveValor.Length > 1)
                        {
                            string valor = HttpUtility.UrlDecode(claveValor[1].Replace("\"", "").Trim());
                            switch (claveValor[0].Trim().ToLower())
                            {
                                case "oauth_token":
                                    token = valor;
                                    break;
                                case "oauth_consumer_key":
                                    consumerKey = valor;
                                    break;
                                case "oauth_nonce":
                                    nonce = valor;
                                    break;
                                case "oauth_signature_method":
                                    method = valor;
                                    break;
                                case "oauth_timestamp":
                                    timespan = valor;
                                    break;
                                case "oauth_signature":
                                    signature = valor;
                                    break;
                            }
                        }
                    }
                }
            }
            string parametroToken = HttpUtility.UrlDecode(token);
            string parametroConsumerKey = HttpUtility.UrlDecode(consumerKey);
            string parametroSignature = HttpUtility.UrlDecode(signature);

            if ((!token.Contains("+")) && (!signature.Contains("+")))
            {
                token = parametroToken;
                consumerKey = parametroConsumerKey;
                signature = parametroSignature;
            }

            pParametrosOauth = "?oauth_token=" + parametroToken + "&oauth_consumer_key=" + parametroConsumerKey + "&oauth_nonce=" + nonce + "&oauth_signature_method=" + method + "&oauth_timestamp=" + timespan + "&oauth_signature=" + parametroSignature;

            pConsumerKey = consumerKey;
            pToken = token;
            pFirmaGnoss = "";
            pFirmaExterna = signature;

            QueryOauth queryOauth = new QueryOauth(pRequest.Path.ToString(), token, consumerKey, nonce, method, timespan, signature, mEntityContextOauth, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<QueryOauth>(), mLoggerFactory);
            pConsumerSecret = queryOauth.ConsumerSecret;
            return ObtenerUsuarioDeParametrosPeticionOAuth(queryOauth, pRequest.Method).Login;
        }

        /// <summary>
        /// Obtiene el usuario que realiza una petición de acceso a datos privados del usuario.
        /// </summary>
        /// <param name="pUrl">URL de la petición</param>
        /// <returns>ID del usuario</returns>
        public Guid ObtenerUsuarioIDDePeticionGet(string pUrl, string pMetodoHttp)
        {
            QueryOauth queryOauth = OAuthBase.ObtenerParametrosUrl(pUrl, mEntityContextOauth, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<QueryOauth>(), mLoggerFactory);
            try
            {
                return ObtenerUsuarioDeParametrosPeticionOAuth(queryOauth, pMetodoHttp).UsuarioID;
            }
            catch(Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Obtiene la fila de usuario la que pertenece el token de los parámetros de una petición OAuth.
        /// </summary>
        /// <param name="pParametrosQuery">Parámetros de la petición OAuth</param>
        /// <param name="pHttpMethod">Método de la petición</param>
        /// <returns>fila de usuario la que pertenece el token de los parámetros de una petición OAuth</returns>
        private Es.Riam.Gnoss.OAuthAD.OAuth.Usuario ObtenerUsuarioDeParametrosPeticionOAuth(QueryOauth pParametrosQuery, string pHttpMethod)
        {
            Es.Riam.Gnoss.OAuthAD.OAuth.Usuario user = null;
            OAuthBase oauthbase = new OAuthBase();

            string urlNormal = "";
            string queryNormal = "";

            bool bienFirmado = false;
            string firma = "";
            StringBuilder mensajeExtraSB = new StringBuilder();
            if (pParametrosQuery.Method.Equals("HMAC-SHA1"))
            {
                mensajeExtraSB.AppendLine("Chequeo firma");
                firma = oauthbase.GenerateSignature(new Uri(pParametrosQuery.Url), pParametrosQuery.ConsumerKey, pParametrosQuery.ConsumerSecret, pParametrosQuery.Token, pParametrosQuery.TokenSecret, pHttpMethod, pParametrosQuery.Timespan, pParametrosQuery.Nonce, out urlNormal, out queryNormal);
                if (firma.Equals(pParametrosQuery.Signature))
                {
                    mensajeExtraSB.AppendLine("Firma correcta");
                    DateTime fecha = new DateTime(1970, 1, 1, 0, 0, 0, 0).Subtract(new TimeSpan(0, 0, int.Parse(pParametrosQuery.Timespan)).Negate());

                    //Comprobar que la fecha es inferior al tiempo establecido para la petición (10 minutos)
                    if (fecha.AddMinutes(10) > DateTime.UtcNow)
                    {
                        mensajeExtraSB.AppendLine("Fecha correcta");
                        //Comprobar que el nonce no existe en Redis (Si no esta, guardarlo con una duracion de 10 minutos)
                        string claveCacheNonceStore = "NonceStore_" + pParametrosQuery.Nonce;
                        Guid aleatorio = Guid.NewGuid();
                        DateTime tiempo = DateTime.Now;
                        if (!mGnossCache.ExisteClaveEnCache(claveCacheNonceStore))
                        {
                            mensajeExtraSB.AppendLine("Nonce correcto");
                            tiempo = DateTime.Now;
                            mGnossCache.AgregarObjetoCache(claveCacheNonceStore, fecha, 10 * 60);
                            bienFirmado = true;
                        }
                    }
                }
            }
            else if (pParametrosQuery.Method.Equals("PLAINTEXT"))
            {
                char[] seps = { '&' };
                string[] claves = pParametrosQuery.Signature.Split(seps, StringSplitOptions.RemoveEmptyEntries);

                if ((claves[0].Equals(pParametrosQuery.ConsumerSecret)) && (claves[1].Equals(pParametrosQuery.TokenSecret)))
                {
                    bienFirmado = true;
                }
            }
            else
            {
                throw new Exception($"Método de cifrado no valido. Use HMAC-SHA1 o PLAINTEXT. Method param: <{pParametrosQuery.Method}>");
            }

            if (bienFirmado)
            {
                user = pParametrosQuery.TokenGnoss.FilaToken.Usuario;
            }
            else
            {
                throw new Exception($"La firma es incorrecta. Parametros usados para generar la firma: \npParametrosQuery.Url: {pParametrosQuery.Url}\npParametrosQuery.ConsumerKey: {pParametrosQuery.ConsumerKey}\npParametrosQuery.ConsumerSecret: {pParametrosQuery.ConsumerSecret}\npParametrosQuery.Token: {pParametrosQuery.Token}\npParametrosQuery.TokenSecret: {pParametrosQuery.TokenSecret}\npHttpMethod: {pHttpMethod}\n, pParametrosQuery.Timespan: {pParametrosQuery.Timespan}\npParametrosQuery.Nonce: {pParametrosQuery.Nonce}\npParametrosQuery.Signature: {pParametrosQuery.Signature}\npParametrosQuery.Signature. \nFirma obtenida: {firma}\nMensaje extra: {mensajeExtraSB.ToString()}");
            }
            return user;
        }

        #endregion
    }
}