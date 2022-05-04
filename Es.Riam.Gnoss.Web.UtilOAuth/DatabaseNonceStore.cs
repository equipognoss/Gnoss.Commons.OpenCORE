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

        #endregion

        #region INonceStore Members

        /// <summary>
        /// Stores a given nonce and timestamp.
        /// </summary>
        /// <param name="pContext">The context, or namespace, within which the
        /// <paramref name="nonce"/> must be unique.
        /// The context SHOULD be treated as case-sensitive.
        /// The value will never be <c>null</c> but may be the empty string.</param>
        /// <param name="pNonce">A series of random characters.</param>
        /// <param name="pTimestampUtc">The UTC timestamp that together with the nonce string make it unique
        /// within the given <paramref name="context"/>.
        /// The timestamp may also be used by the data store to clear out old nonces.</param>
        /// <returns>
        /// True if the context+nonce+timestamp (combination) was not previously in the database.
        /// False if the nonce was stored previously with the same timestamp and context.
        /// </returns>
        /// <remarks>
        /// The nonce must be stored for no less than the maximum time window a message may
        /// be processed within before being discarded as an expired message.
        /// This maximum message age can be looked up via the
        /// <see cref="DotNetOpenAuth.Configuration.MessagingElement.MaximumMessageLifetime"/>
        /// property, accessible via the <see cref="DotNetOpenAuth.Configuration.DotNetOpenAuthSection.Configuration"/>
        /// property.
        /// </remarks>
        public bool StoreNonce(string pContext, string pNonce, DateTime pTimestampUtc)
        {
           return StoreNonce(pContext, pNonce, pTimestampUtc, false);
        }

        /// <summary>
        /// Stores a given nonce and timestamp.
        /// </summary>
        /// <param name="pContext">The context, or namespace, within which the
        /// <paramref name="nonce"/> must be unique.
        /// The context SHOULD be treated as case-sensitive.
        /// The value will never be <c>null</c> but may be the empty string.</param>
        /// <param name="pNonce">A series of random characters.</param>
        /// <param name="pTimestampUtc">The UTC timestamp that together with the nonce string make it unique
        /// within the given <paramref name="context"/>.
        /// The timestamp may also be used by the data store to clear out old nonces.</param>
        /// <param name="pSoloComprobar">Verdad si solo se debe comprobar que no exista, sin crearlo en la base de datos</param>
        /// <returns>
        /// True if the context+nonce+timestamp (combination) was not previously in the database.
        /// False if the nonce was stored previously with the same timestamp and context.
        /// </returns>
        /// <remarks>
        /// The nonce must be stored for no less than the maximum time window a message may
        /// be processed within before being discarded as an expired message.
        /// This maximum message age can be looked up via the
        /// <see cref="DotNetOpenAuth.Configuration.MessagingElement.MaximumMessageLifetime"/>
        /// property, accessible via the <see cref="DotNetOpenAuth.Configuration.DotNetOpenAuthSection.Configuration"/>
        /// property.
        /// </remarks>
        public bool StoreNonce(string pContext, string pNonce, DateTime pTimestampUtc, bool pSoloComprobar)
        {
            OAuthCN oauthcn = new OAuthCN("oauth", mEntityContext, mLoggingService, mConfigService, mEntityContextOauth, mServicesUtilVirtuosoAndReplication);
            bool existe = oauthcn.ComprobarExisteNonce(pContext, pNonce, pTimestampUtc);

            if ((!existe) && (!pSoloComprobar))
            {
                mEntityContextOauth.Nonce.Add(new Es.Riam.Gnoss.OAuthAD.OAuth.Nonce() { Context = pContext, Code = pNonce, Timestamp = pTimestampUtc } );

                oauthcn.ActualizarBD();
            }

            oauthcn.Dispose();

            return !existe;
        }

        #endregion
    }
}