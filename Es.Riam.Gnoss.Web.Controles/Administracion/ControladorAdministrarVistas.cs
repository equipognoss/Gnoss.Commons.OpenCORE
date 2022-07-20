using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorAdministrarVistas : ControladorBase
    {
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;

        public ControladorAdministrarVistas(Proyecto pProyectoSeleccionado, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;

            ProyectoSeleccionado = pProyectoSeleccionado;
        }

        public void InvalidarVistas(bool pVistasEcosistema)
        {
            LimpiarCacheVistasRedis(pVistasEcosistema, null, null);
            string urlsResultados = mConfigService.ObtenerUrlServicioResultados();
            string[] urlsServicios = urlsResultados.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string url in urlsServicios)
            {
                CargadorResultados cargadorResultados = new CargadorResultados();
                cargadorResultados.Url = url;
                cargadorResultados.InvalidarVistas(UsuarioActual.IdentidadID);
            }

            CargadorFacetas cargadorFacetas = new CargadorFacetas();
            cargadorFacetas.Url = mConfigService.ObtenerUrlServicioFacetas();
            cargadorFacetas.InvalidarVistas(UsuarioActual.IdentidadID);        
        }

        public string CrearXMLVistasCMS()
        {
            Dictionary<Guid, Tuple<string, string>> listaVistasCMS = ObtenerDiccionarioCMSBD();
            return JsonConvert.SerializeObject(listaVistasCMS, Newtonsoft.Json.Formatting.Indented);
        }

        public void LimpiarCacheVistasRedis(bool pEsAdministracionEcosistema, string pFicheroConfiguracion, string pUrlIntragnoss, string pVistaActualizada = null)
        {
            VistaVirtualCL vistaVirtualCL = null;
            ProyectoCL proyCL = null;

            if (string.IsNullOrEmpty(pFicheroConfiguracion))
            {
                vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                vistaVirtualCL = new VistaVirtualCL(pFicheroConfiguracion + "@@@acid", pFicheroConfiguracion + "@@@acid", mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                proyCL = new ProyectoCL(pFicheroConfiguracion + "@@@acid", pFicheroConfiguracion + "@@@acid", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

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
            Dictionary<string, string> diccionarioRefrescoCache = mGnossCache.ObtenerObjetoDeCache(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + pPersonalizacionID) as Dictionary<string, string>;

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

        public Dictionary<Guid, Tuple<string, string>> ObtenerDiccionarioCMSBD()
        {
            Dictionary<Guid, Tuple<string, string>> listaVistasCMS = new Dictionary<Guid, Tuple<string, string>>();
            using (VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                DataWrapperVistaVirtual vistaVirtualDW = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave);
                    foreach (AD.EntityModel.Models.VistaVirtualDS.VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDW.ListaVistaVirtualCMS)
                {
                    string datosextra = string.Empty;
                    if (filaVistaVirtualCMS.DatosExtra != null)
                    {
                        datosextra = filaVistaVirtualCMS.DatosExtra;
                    }
                    listaVistasCMS.Add(filaVistaVirtualCMS.PersonalizacionComponenteID, new Tuple<string, string>(filaVistaVirtualCMS.Nombre, datosextra));
                }
            }

            return listaVistasCMS;
        }
    }
}
