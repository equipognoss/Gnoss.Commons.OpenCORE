using System;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Web.Controles;
using System.Collections.Generic;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Microsoft.AspNetCore.Http;
using Es.Riam.AbstractsOpen;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.Elementos.Amigos;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ControladorAdministrarVistas : ControladorBase
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;
        public ControladorAdministrarVistas(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorAdministrarVistas> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mLoggingService = loggingService;
            mVirtuosoAD = virtuosoAD;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }


        /// <summary>
        /// Limpia cachés en redis y añade la clave para que todos los servicios y procesos se actualicen
        /// </summary>
        /// <param name="pEsAdministracionEcosistema">Verdad si estamos limpiando la caché del ecosistema</param>
        public void LimpiarCacheVistasRedis(bool pEsAdministracionEcosistema, string pVistaActualizada = null)
        {
            LimpiarCacheVistasRedis(pEsAdministracionEcosistema, null, null, pVistaActualizada);
        }

        /// <summary>
        /// Limpia cachés en redis y añade la clave para que todos los servicios y procesos se actualicen
        /// </summary>
        /// <param name="pEsAdministracionEcosistema">Verdad si estamos limpiando la caché del ecosistema</param>
        /// <param name="pFicheroConfiguracion">Fichero de configuración de la base de datos que queremos limpiar (Usar sólo si estamos haciendo un paso a producción de vistas)</param>
        /// <param name="pUrlIntragnoss">Url Intragnoss (Usar sólo si estamos haciendo un paso a producción de vistas)</param>
        public void LimpiarCacheVistasRedis(bool pEsAdministracionEcosistema, string pFicheroConfiguracion, string pUrlIntragnoss, string pVistaActualizada = null)
        {
            VistaVirtualCL vistaVirtualCL = null;
            ProyectoCL proyCL = null;

            if (string.IsNullOrEmpty(pFicheroConfiguracion))
            {
                vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<VistaVirtualCL>(), mloggerFactory);
                proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<ProyectoCL>(), mloggerFactory);
            }
            else
            {
                vistaVirtualCL = new VistaVirtualCL(pFicheroConfiguracion + "@@@acid", pFicheroConfiguracion + "@@@acid", mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VistaVirtualCL>(), mLoggerFactory);
                proyCL = new ProyectoCL(pFicheroConfiguracion + "@@@acid", pFicheroConfiguracion + "@@@acid", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

                if (!string.IsNullOrEmpty(pUrlIntragnoss))
                {
                    vistaVirtualCL.EstablecerDominioCache(pUrlIntragnoss);
                    proyCL.EstablecerDominioCache(pUrlIntragnoss);
                }
            }

            if (pEsAdministracionEcosistema)
            {
                vistaVirtualCL.InvalidarVistasVirtualesEcosistema();
                proyCL.InvalidarTodasComunidadesMVC();

                ActualizarCache(PersonalizacionEcosistemaID, pVistaActualizada);
            }
            else
            {
                //invalidar vistas virtuales y comunidadMVC de todos los proyectos con esta personalizacion

                vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);
                proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
                proyCL.InvalidarComunidadMVC(ProyectoAD.MetaProyecto);

                ActualizarCache(ProyectoSeleccionado.PersonalizacionID, pVistaActualizada);
            }
            vistaVirtualCL.Dispose();
            proyCL.Dispose();
        }

        private void ActualizarCache(Guid pPersonalizacionID, string pVistaActualizada)
        {
            Dictionary<string, string> diccionarioRefrescoCache = mGnossCache.ObtenerObjetoDeCache(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + pPersonalizacionID, typeof(Dictionary<string, string>)) as Dictionary<string, string>;

            if (diccionarioRefrescoCache == null)
            {
                diccionarioRefrescoCache = new Dictionary<string, string>();
                diccionarioRefrescoCache.Add("ClaveActualizacion", Guid.NewGuid().ToString());
                diccionarioRefrescoCache.Add("PersonalizacionID", Guid.NewGuid().ToString());
            }
            else
            {
                diccionarioRefrescoCache["ClaveActualizacion"] = Guid.NewGuid().ToString();

                if (string.IsNullOrEmpty(pVistaActualizada))
                {
                    diccionarioRefrescoCache["PersonalizacionID"] = Guid.NewGuid().ToString();
                }
                else
                {
                    diccionarioRefrescoCache[pVistaActualizada] = DateTime.Now.ToString();
                }
            }

            if (!pPersonalizacionID.Equals(Guid.Empty))
            {
                mGnossCache.AgregarObjetoCache(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + pPersonalizacionID, diccionarioRefrescoCache, BaseCL.DURACION_CACHE_UN_DIA);
            }
        }
    }
}
