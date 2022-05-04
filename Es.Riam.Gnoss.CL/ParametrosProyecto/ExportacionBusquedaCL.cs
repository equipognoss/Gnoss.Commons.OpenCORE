using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Logica.ExportacionBusqueda;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.CL.ParametrosProyecto
{
    public class ExportacionBusquedaCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.EXPORTACIONBUSQUEDA };

        private ConfigService _configService;
        private EntityContext _entityContext;
        private LoggingService _loggingService;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor para ParametroGeneralCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ExportacionBusquedaCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Constructor para ParametroGeneralCL
        /// </summary>
        public ExportacionBusquedaCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Recupera las exportaciones de búsqueda de un proyecto
        /// </summary>
        /// <returns>Países y provincias</returns>
        public DataWrapperExportacionBusqueda ObtenerExportacionesProyecto(Guid pProyectoID)
        {
            string rawKey = pProyectoID.ToString();

            // Compruebo si está en la caché
            DataWrapperExportacionBusqueda exportacionDW = ObtenerObjetoDeCacheLocal(ObtenerClaveCache(rawKey)) as DataWrapperExportacionBusqueda;
            if (exportacionDW == null)
            {
                exportacionDW = ObtenerObjetoDeCache(rawKey) as DataWrapperExportacionBusqueda;
                AgregarObjetoCacheLocal(pProyectoID, ObtenerClaveCache(rawKey), exportacionDW);
            }

            if (exportacionDW == null)
            {
                if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
                {
                    // Si no está, lo cargo y lo almaceno en la caché
                    ExportacionBusquedaCN exportacionBusquedaCN = new ExportacionBusquedaCN(mFicheroConfiguracionBD, _entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                    if (TieneComunidadPadreConfigurada(pProyectoID))
                    {
                        exportacionDW = exportacionBusquedaCN.ObtenerExportacionesProyecto(ProyectoIDPadreEcosistema.Value);
                    }
                    else
                    {
                        exportacionDW = exportacionBusquedaCN.ObtenerExportacionesProyecto(pProyectoID);
                    }
                    exportacionBusquedaCN.Dispose();
                }
                else
                {
                    // Si no está, lo cargo y lo almaceno en la caché
                    ExportacionBusquedaCN exportacionBusquedaCN = new ExportacionBusquedaCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                    if (TieneComunidadPadreConfigurada(pProyectoID))
                    {
                        exportacionDW = exportacionBusquedaCN.ObtenerExportacionesProyecto(ProyectoIDPadreEcosistema.Value);
                    }
                    else
                    {
                        exportacionDW = exportacionBusquedaCN.ObtenerExportacionesProyecto(pProyectoID);
                    }
                    exportacionBusquedaCN.Dispose();
                }


                AgregarObjetoCache(rawKey, exportacionDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, exportacionDW);
            }

            return exportacionDW;
        }


        /// <summary>
        /// Invalida todas las exportaciones de búsqueda de un proyecto
        /// </summary>
        public void InvalidarCacheExportacionesProyecto(Guid pProyectoID)
        {
            InvalidarCache(ObtenerClaveCache(pProyectoID.ToString()));

            VersionarCacheLocal(pProyectoID);
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

        #endregion
    }
}
