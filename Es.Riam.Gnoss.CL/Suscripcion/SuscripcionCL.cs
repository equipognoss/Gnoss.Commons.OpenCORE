using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL.Suscripcion
{
    public class SuscripcionCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private SuscripcionCN mSuscripcionCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Suscripcion" };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para IdentidadCL
        /// </summary>
        public SuscripcionCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<SuscripcionCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para IdentidadCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public SuscripcionCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<SuscripcionCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos generales

        public List<Guid> ObtenerListaIdentidadesSuscritasPerfil(Guid pPerfilID)
        {
            List<Guid> listaIdentidadesSuscritasPefil = null;
            try
            {
                listaIdentidadesSuscritasPefil = (List<Guid>)ObtenerObjetoDeCache("ListaIdentidadesSuscritasPerfil_" + pPerfilID, true,typeof(List<Guid>));
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
            if (listaIdentidadesSuscritasPefil == null)
            {
                listaIdentidadesSuscritasPefil = SuscripcionCN.ObtenerListaIdentidadesSuscritasPerfil(pPerfilID);

                AgregarObjetoCache("ListaIdentidadesSuscritasPerfil_" + pPerfilID, listaIdentidadesSuscritasPefil);
            }
            return listaIdentidadesSuscritasPefil;
        }

        public void InvalidarListaIdentidadesSuscritasPerfil(Guid pPerfilID)
        {
            InvalidarCache("ListaIdentidadesSuscritasPerfil_" + pPerfilID, true);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected SuscripcionCN SuscripcionCN
        {
            get
            {
                if (mSuscripcionCN == null)
                {
                    if (string.IsNullOrEmpty(mFicheroConfiguracionBD))
                    {
                        mSuscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                    }
                    else
                    {
                        mSuscripcionCN = new SuscripcionCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                    }
                }

                return mSuscripcionCN;
            }
        }

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~SuscripcionCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }
        }

        #endregion

    }
}
