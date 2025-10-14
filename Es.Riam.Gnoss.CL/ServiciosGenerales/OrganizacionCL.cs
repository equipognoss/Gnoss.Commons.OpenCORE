using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.CL.ServiciosGenerales
{

    /// <summary>
    /// Capa de caché de organización
    /// </summary>
    public class OrganizacionCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private OrganizacionCN mOrganizacionCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.ORGANIZACION };

       
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        public OrganizacionCL( EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<OrganizacionCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Métodos

        /// <summary>
        /// Verdad si una organización es una clase
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClase(Guid pOrganizacionID)
        {
            string rawKey = string.Concat(NombresCL.ORGANIZACION, "_EsClase_", pOrganizacionID);

            // Compruebo si está en la caché
            bool? esClase = ObtenerObjetoDeCache(rawKey, typeof(bool)) as bool?;
            if (!esClase.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                esClase = OrganizacionCN.ComprobarOrganizacionEsClase(pOrganizacionID);
                AgregarObjetoCache(rawKey, esClase);
            }

            return esClase.Value;
        }

        /// <summary>
        /// Verdad si una organización es una clase de primaria
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <returns></returns>
        public bool ComprobarOrganizacionEsClasePrimaria(Guid pOrganizacionID)
        {
            string rawKey = string.Concat(NombresCL.ORGANIZACION, "_EsClasePrimaria_", pOrganizacionID);

            // Compruebo si está en la caché
            bool? esClase = ObtenerObjetoDeCache(rawKey, typeof(bool)) as bool?;
            if (!esClase.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                esClase = OrganizacionCN.ComprobarOrganizacionEsClasePrimaria(pOrganizacionID);
                AgregarObjetoCache(rawKey, esClase);
            }

            return esClase.Value;
        }

        /// <summary>
        /// Obtiene el nombre de la organizacion
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la identidad de la organización</param>
        /// <returns></returns>
        public DataWrapperOrganizacion ObtenerNombreOrganizacionPorIdentidad(Guid pIdentidadID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("NombreOrgPorIdentidad_", pIdentidadID);

            // Compruebo si está en la caché
            DataWrapperOrganizacion orgDW = (DataWrapperOrganizacion)ObtenerObjetoDeCache(rawKey, typeof(DataWrapperOrganizacion));
            if (orgDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                orgDW = OrganizacionCN.ObtenerNombreOrganizacionPorIdentidad(pIdentidadID);
                AgregarObjetoCache(rawKey, orgDW);
            }
            mEntityContext.UsarEntityCache = false;
            return orgDW;
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada organización para crear la tabla BASE
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <returns></returns>
        public int ObtenerTablaBaseOrganzacionIDOrganizacionPorID(Guid? pOrganizacionID)
        {
            if (pOrganizacionID.HasValue)
            {
                string rawKey = string.Concat(NombresCL.TABLABASEORGANIZACIONID, "_", pOrganizacionID);

                // Compruebo si está en la caché
                object id = ObtenerObjetoDeCache(rawKey, typeof(int));
                if (id == null)
                {
                    // Si no está, lo cargo y lo almaceno en la caché
                    id = OrganizacionCN.ObtenerTablaBaseOrganizacionIDOrganizacionPorID(pOrganizacionID.Value);
                    AgregarObjetoCache(rawKey, id);
                }

                return (int)id;
            }
            else
            {
                return -1;
            }
        }

        public void AgregarClasesEcosistema(DataWrapperOrganizacion mOrganizacionDW)
        {
            AgregarObjetoCache("OrganizacionClase_ClasesEcosistema", mOrganizacionDW);
        }

        public void InvalidarClasesEcosistema()
        {
            InvalidarCache("OrganizacionClase_ClasesEcosistema");
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected OrganizacionCN OrganizacionCN
        {
            get
            {
                if (mOrganizacionCN == null)
                {
                    mOrganizacionCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OrganizacionCN>(), mLoggerFactory);
                }

                return mOrganizacionCN;
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
        ~OrganizacionCL()
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
