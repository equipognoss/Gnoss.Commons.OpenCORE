namespace OAuthServiceProvider.Code
{
    using DotNetOpenAuth.Messaging.Bindings;
    using Es.Riam.AbstractsOpen;
    using Es.Riam.Gnoss.AD.EntityModel;
    using Es.Riam.Gnoss.LogicaOAuth.OAuth;
    using Es.Riam.Gnoss.OAuthAD;
    using Es.Riam.Gnoss.Util.Configuracion;
    using Es.Riam.Gnoss.Util.General;
    using System;

    /// <summary>
    /// A database-persisted nonce store.
    /// </summary>
    public class DatabaseNonceStore : INonceStore
    {

        private EntityContextOauth mEntityContextOauth;
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #region Constructores

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseNonceStore"/> class.
        /// </summary>
        public DatabaseNonceStore(EntityContextOauth entityContextOauth, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mEntityContextOauth = entityContextOauth;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        public bool StoreNonce(string context, string nonce, DateTime timestampUtc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INonceStore Members


        #endregion
    }
}