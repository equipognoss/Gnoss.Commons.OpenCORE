using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;

namespace Es.Riam.Gnoss.CL.ParametrosProyecto
{
    public class VistaVirtualCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.PROYECTO };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private GnossCache mGnossCache;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para VistaVirtualCL
        /// </summary>
        public VistaVirtualCL(EntityContext entityContext, LoggingService loggingService, GnossCache gnossCache, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication,ILogger<VistaVirtualCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mGnossCache = gnossCache;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para VistaVirtualCL con parámetros
        /// </summary>
        public VistaVirtualCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, GnossCache gnossCache, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VistaVirtualCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mGnossCache = gnossCache;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos


        /// <summary>
        /// Obtiene las tablas de VistaVirtual de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public DataWrapperVistaVirtual ObtenerVistasVirtualPorProyectoID(Guid pProyectoID, Guid pPersonalizacionEcosistemaID, bool pComunidadExcluidaPersonalizacionEcosistema)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("VistaVirtual", "_", pProyectoID.ToString());
            DataWrapperVistaVirtual vistaVirtualDW = (DataWrapperVistaVirtual)ObtenerObjetoDeCacheLocal(rawKey);
            if (vistaVirtualDW == null)
            {
                vistaVirtualDW = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperVistaVirtual)) as DataWrapperVistaVirtual;
                if (vistaVirtualDW != null)
                {
                    vistaVirtualDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, vistaVirtualDW);
            }

            if (vistaVirtualDW != null)
            {
                DataWrapperVistaVirtual vistaVirtualAuxDW = new DataWrapperVistaVirtual();
                try
                {
                    vistaVirtualAuxDW.Merge(vistaVirtualDW);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de caché, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    vistaVirtualDW = vistaVirtualAuxDW;
                }
                catch
                {
                    vistaVirtualDW = null;
                }
            }

            if (vistaVirtualDW == null)
            {
                VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(pProyectoID);
                if (vistaVirtualDW != null)
                {
                    vistaVirtualDW.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, vistaVirtualDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, vistaVirtualDW);
            }

            //Si tiene personalización el ecosistema, la cargamos.
            if (!pComunidadExcluidaPersonalizacionEcosistema && pPersonalizacionEcosistemaID != Guid.Empty)
            {
                string rawKey2 = string.Concat("VistaVirtualEcosistema");

                DataWrapperVistaVirtual vistaVirtualEcosistemaDW = (DataWrapperVistaVirtual)ObtenerObjetoDeCacheLocal(rawKey2);

                if (vistaVirtualEcosistemaDW == null)
                {
                    vistaVirtualEcosistemaDW = ObtenerObjetoDeCache(rawKey2, typeof(DataWrapperVistaVirtual)) as DataWrapperVistaVirtual;

                    AgregarObjetoCacheLocal(ProyectoAD.MetaProyecto, rawKey2, vistaVirtualEcosistemaDW);
                    //AgregarObjetoCacheLocal(pProyectoID, rawKey2, vistaVirtualEcosistemaDW);
                }
                if (vistaVirtualEcosistemaDW == null)
                {
                    VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCN>(), mLoggerFactory);
                    vistaVirtualEcosistemaDW = vistaVirtualCN.ObtenerVistasVirtualPorEcosistemaID(pPersonalizacionEcosistemaID);
                    if (vistaVirtualEcosistemaDW != null)
                    {
                        vistaVirtualEcosistemaDW.CargaRelacionesPerezosasCache();
                    }
                    AgregarObjetoCache(rawKey2, vistaVirtualEcosistemaDW);
                    AgregarObjetoCacheLocal(ProyectoAD.MetaProyecto, rawKey2, vistaVirtualEcosistemaDW);
                    //AgregarObjetoCacheLocal(pProyectoID, rawKey2, vistaVirtualEcosistemaDW);
                }

                if (vistaVirtualEcosistemaDW != null)
                {
                    vistaVirtualDW.Merge(vistaVirtualEcosistemaDW);
                }
            }
            mEntityContext.UsarEntityCache = false;
            return vistaVirtualDW;
        }

        /// <summary>
        /// Invalida la cache de las tablas de VistaVirtual de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public void InvalidarVistasVirtuales(Guid pProyectoID)
        {
            string rawKey = string.Concat("VistaVirtual", "_", pProyectoID.ToString());
            InvalidarCache(rawKey);

            mGnossCache.VersionarCacheLocal(pProyectoID);

            InvalidarVistasVirtualesEcosistema();
        }

        public void InvalidarVistasVirtualesEnCacheLocal(Guid pProyectoID)
        {
            string rawKey = string.Concat("VistaVirtual", "_", pProyectoID.ToString());
            InvalidarCacheLocal(rawKey);
        }

        /// <summary>
        /// Invalida la cache de las tablas de VistaVirtual de un ecosistema
        /// </summary>
        /// <returns>DataSet de VistaVirtual de un proyecto</returns>
        public void InvalidarVistasVirtualesEcosistema()
        {
            string rawKey = string.Concat("VistaVirtualEcosistema");
            InvalidarCache(rawKey);
        }

        public void InvalidarVistasVirtualesEcosistemaEnCacheLocal()
        {
            string rawKey = string.Concat("VistaVirtualEcosistema");
            InvalidarCacheLocal(rawKey);
        }

        #endregion

        #region Propiedades

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

        /// <summary>
        /// Dominio sobre el que se genera la cache
        /// </summary>
        public override string Dominio
        {
            get
            {
                return mDominio;
            }
            set
            {
                mDominio = value;
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
        ~VistaVirtualCL()
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
