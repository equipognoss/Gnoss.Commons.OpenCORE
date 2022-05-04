using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class AdministrarVistas : ControladorBase
    {
        public Proyecto ProyectoSeleccionado { get; set; }

        public AdministrarVistas(Proyecto pProyectoSeleccionado, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {

            ProyectoSeleccionado = pProyectoSeleccionado;
        }

        public void InvalidarVistas(bool pVistasEcosistema)
        {
            using (VistaVirtualCL vistaVirtualCL = new VistaVirtualCL(mEntityContext, mLoggingService, mGnossCache, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication))
            using (ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication))
            using (GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication))
            {
                Guid idCache = Guid.NewGuid();
                if (pVistasEcosistema)
                {
                    vistaVirtualCL.InvalidarVistasVirtualesEcosistema();
                    proyCL.InvalidarTodasComunidadesMVC();
                    if (!PersonalizacionEcosistemaID.Equals(Guid.Empty))
                    {
                        gnossCacheCL.AgregarObjetoCache(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + PersonalizacionEcosistemaID, idCache, 0);
                    }
                }
                else
                {
                    vistaVirtualCL.InvalidarVistasVirtuales(ProyectoSeleccionado.Clave);
                    proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
                    proyCL.InvalidarComunidadMVC(ProyectoAD.MetaProyecto);
                    if (!ProyectoSeleccionado.PersonalizacionID.Equals(Guid.Empty))
                    {
                        gnossCacheCL.AgregarObjetoCache(GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + ProyectoSeleccionado.PersonalizacionID, idCache, 0);
                    }
                }
            }
        }
        public string CrearXMLVistasCMS()
        {
            Dictionary<Guid, Tuple<string, string>> listaVistasCMS = ObtenerDiccionarioCMSBD();
            return JsonConvert.SerializeObject(listaVistasCMS, Newtonsoft.Json.Formatting.Indented);
        }

        public Dictionary<Guid, Tuple<string, string>> ObtenerDiccionarioCMSBD()
        {
            Dictionary<Guid, Tuple<string, string>> listaVistasCMS = new Dictionary<Guid, Tuple<string, string>>();
            using (VistaVirtualCN vistaVirtualCN = new VistaVirtualCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication))
            using (DataWrapperVistaVirtual vistaVirtualDS = vistaVirtualCN.ObtenerVistasVirtualPorProyectoID(ProyectoSeleccionado.Clave))
            {

                foreach (VistaVirtualCMS filaVistaVirtualCMS in vistaVirtualDS.ListaVistaVirtualCMS)
                {
                    string datosextra = string.Empty;
                    if (! string.IsNullOrEmpty(filaVistaVirtualCMS.DatosExtra))
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
