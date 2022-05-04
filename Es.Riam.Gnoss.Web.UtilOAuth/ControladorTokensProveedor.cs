using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Controlador de tokens para el consumidor
    /// </summary>
    public class ControladorTokensProveedor : ControladorTokens
    {

        #region Miembros

        private string mConsumerKey;
        private string mConsumerSecret;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControladorTokensProveedor(string pConsumerKey, string pConsumerSecret, EntityContextOauth entityContextOauth, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContextOauth, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mConsumerKey = pConsumerKey;
            mConsumerSecret = pConsumerSecret;
        }

        #endregion

        #region Miembros de IConsumerTokenManager

        /// <summary>
        /// Obtiene la clave pública del consumer
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                return mConsumerKey;
                //return "sYFT1TPaSjmwSmuBwdmg";
            }
        }

        /// <summary>
        /// Obtiene la clave secreta del cosumer
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                return mConsumerSecret;
                //return "PDQFNo5ECmsYWNFTcfdHuYDyahIV7SVPgDlriMQb";
            }
        }

        #endregion
    }
}