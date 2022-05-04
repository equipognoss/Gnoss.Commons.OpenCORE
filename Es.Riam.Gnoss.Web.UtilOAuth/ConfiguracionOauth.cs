using System.IO;
using System.Xml;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Configuración básica de una aplicación oauth
    /// </summary>
    public class ConfiguracionAplicacionOauth
    {
        #region Miembros

        private string mRequestTokenUrl;
        private string mAuthorizeUrl;
        private string mAccessTokenUrl;
        private string mCallbackUrl;
        private string mConsumerKey;
        private string mConsumerSecret;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ConfiguracionAplicacionOauth() { }

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pRequestTokenUrl">Url a la que se solicita el request token</param>
        /// <param name="pAuthorizeUrl">Url en la que el usaurio autoriza una aplicación</param>
        /// <param name="pAccessTokenUrl">Url en la que se solicita el access token</param>
        /// <param name="pCallbackUrl">Url de callback de la aplicación</param>
        /// <param name="pConsumerKey">Consumer Key</param>
        /// <param name="pConsumerSecret">Consumer secret</param>
        public ConfiguracionAplicacionOauth(string pRequestTokenUrl, string pAuthorizeUrl, string pAccessTokenUrl, string pCallbackUrl, string pConsumerKey, string pConsumerSecret)
        {
            mRequestTokenUrl = pRequestTokenUrl;
            mAuthorizeUrl = pAuthorizeUrl;
            mAccessTokenUrl = pAccessTokenUrl;
            mCallbackUrl = pCallbackUrl;
            mConsumerKey = pConsumerKey;
            mConsumerSecret = pConsumerSecret;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la Url a la que se solicita el request token
        /// </summary>
        public string RequestTokenUrl
        {
            get
            {
                return mRequestTokenUrl;
            }
        }

        /// <summary>
        /// Obtiene la Url en la que el usaurio autoriza una aplicación
        /// </summary>
        public string AuthorizeUrl
        {
            get
            {
                return mAuthorizeUrl;
            }
        }

        /// <summary>
        /// Obtiene la Url en la que se solicita el access token
        /// </summary>
        public string AccessTokenUrl
        {
            get
            {
                return mAccessTokenUrl;
            }
        }

        /// <summary>
        /// Obtiene la Url de callback de la aplicación
        /// </summary>
        public string CallbackUrl
        {
            get
            {
                return mCallbackUrl;
            }
        }

        /// <summary>
        /// Obtiene el consumer key
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                return mConsumerKey;
            }
        }

        /// <summary>
        /// Obtiene el Consumer Secret
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                return mConsumerSecret;
            }
        }

        #endregion
    }

    /// <summary>
    /// Lee la configuración de las aplicaciones Oauth
    /// </summary>
    public class LectorConfiguracionOauth
    {
        /// <summary>
        /// Carga la configuración para la integración con Ideas4All
        /// </summary>
        public static ConfiguracionAplicacionOauth CargarConfiguracionIdeas4All()
        {
            XmlDocument documentoXml = new XmlDocument();

            if (File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/OauthRedesSociales.config"))
            {
                documentoXml.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/OauthRedesSociales.config");

                string requestTokenUrl = documentoXml.GetElementsByTagName("Ideas4AllRequestToken").Item(0).InnerText;
                string authorizeUrl = documentoXml.GetElementsByTagName("Ideas4AllAuth").Item(0).InnerText;
                string accesTokenUrl = documentoXml.GetElementsByTagName("Ideas4AllAccesToken").Item(0).InnerText;
                string callBackUrl = documentoXml.GetElementsByTagName("Ideas4AllCallbackURL").Item(0).InnerText;

                string consumerKey = documentoXml.GetElementsByTagName("Ideas4AllConsumerKey").Item(0).InnerText;
                string consumerSecret = documentoXml.GetElementsByTagName("Ideas4AllConsumerSecret").Item(0).InnerText;

                return new ConfiguracionAplicacionOauth(requestTokenUrl, authorizeUrl, accesTokenUrl, callBackUrl, consumerKey, consumerSecret);
            }

            return new ConfiguracionAplicacionOauth();
        }
    }
}
