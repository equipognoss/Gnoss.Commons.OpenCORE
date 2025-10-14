using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{
    /// <summary>
    /// Query Oauth.
    /// </summary>
    public class QueryOauth
    {
        #region Miembros

        /// <summary>
        /// URL de la Query.
        /// </summary>
        private string mUrl;

        /// <summary>
        /// Token de la Query.
        /// </summary>
        private string mToken;

        /// <summary>
        /// URL de la Query.
        /// </summary>
        private string mConsumerKey;

        /// <summary>
        /// Nonce de la Query.
        /// </summary>
        private string mNonce;

        /// <summary>
        /// Method de la Query.
        /// </summary>
        private string mMethod;

        /// <summary>
        /// Timespan de la Query.
        /// </summary>
        private string mTimespan;

        /// <summary>
        /// Signature de la Query.
        /// </summary>
        private string mSignature;

        /// <summary>
        /// Manejador de tokens.
        /// </summary>
        private TokenGnoss mTokenGnoss;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructor

        public QueryOauth(string pUrl, string pToken, string pConsumerKey, string pNonce, string pMethod, string pTimespan, string pSignature, EntityContextOauth entityContextOauth, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<QueryOauth> logger, ILoggerFactory loggerFactory)
        {
            mUrl = pUrl;
            mToken = pToken;
            mConsumerKey = pConsumerKey;
            mNonce = pNonce;
            mMethod = pMethod;
            mTimespan = pTimespan;
            mSignature = pSignature;
            mlogger = logger;
            mLoggerFactory = loggerFactory;

            ControladorTokens tokenControler = new ControladorTokens(entityContextOauth, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorTokens>(), mLoggerFactory);
            //OAuthBase oauthbase = new OAuthBase();
            mTokenGnoss = tokenControler.ObtenerToken(mToken);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// URL de la Query.
        /// </summary>
        public string Url
        {
            get
            {
                return mUrl;
            }
            set
            {
                mUrl = value;
            }
        }

        /// <summary>
        /// Token de la Query.
        /// </summary>
        public string Token
        {
            get
            {
                return mToken;
            }
            set
            {
                mToken = value;
            }
        }

        /// <summary>
        /// ConsumerKey de la Query.
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                return mConsumerKey;
            }
            set
            {
                mConsumerKey = value;
            }
        }

        /// <summary>
        /// Nonce de la Query.
        /// </summary>
        public string Nonce
        {
            get
            {
                return mNonce;
            }
            set
            {
                mNonce = value;
            }
        }

        /// <summary>
        /// Method de la Query.
        /// </summary>
        public string Method
        {
            get
            {
                return mMethod;
            }
            set
            {
                mMethod = value;
            }
        }

        /// <summary>
        /// Timespan de la Query.
        /// </summary>
        public string Timespan
        {
            get
            {
                return mTimespan;
            }
            set
            {
                mTimespan = value;
            }
        }

        /// <summary>
        /// Signature de la Query.
        /// </summary>
        public string Signature
        {
            get
            {
                return mSignature;
            }
            set
            {
                mSignature = value;
            }
        }

        /// <summary>
        /// Manejador de tokens.
        /// </summary>
        public TokenGnoss TokenGnoss
        {
            get
            {
                return mTokenGnoss;
            }
        }

        /// <summary>
        /// ConsumerSecret de la Query.
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                return mTokenGnoss.FilaToken.OAuthConsumer.ConsumerSecret;
                //return mTokenGnoss.FilaToken.OAuthConsumerRow.ConsumerSecret;
            }
        }

        /// <summary>
        /// TokenSecret de la Query.
        /// </summary>
        public string TokenSecret
        {
            get
            {
                return mTokenGnoss.FilaToken.TokenSecret;
            }
        }

        #endregion
    }
}
