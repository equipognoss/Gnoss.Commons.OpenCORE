using Es.Riam.Gnoss.AD.EntityModel;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.ParametroAplicacion
{
    [Serializable]
    public class GestorParametroAplicacion
    {
        private List<AD.EntityModel.ParametroAplicacion> parametrosAplicacion; 
        private List<ConfiguracionServiciosDominio> configuracionServiciosDominio;
        private List<ConfiguracionServicios> configuracionServicios;
        private List<ConfiguracionServiciosProyecto> configuracionServiciosProyecto;
        private List<ConfiguracionBBDD> configuracionBBDD;
        private List<AccionesExternas> accionesExternas;
        private List<ConfigApplicationInsightsDominio> configApplicationInsightsDominio;
        private List<TextosPersonalizadosPlataforma> textosPersonalizadosPlataforma;
        private List<ProyectoSinRegistroObligatorio> proyectoSinRegistroObligatorio;
        private List<ProyectoRegistroObligatorio> proyectoRegistroObligatorio;
        private static GestorParametroAplicacion instance=null;

        public GestorParametroAplicacion()
        {
            configuracionServicios = new List<ConfiguracionServicios>();
            configuracionServiciosDominio = new List<ConfiguracionServiciosDominio>();
            configuracionServiciosProyecto = new List<ConfiguracionServiciosProyecto>();
            configuracionBBDD = new List<ConfiguracionBBDD>();
            parametrosAplicacion = new List<AD.EntityModel.ParametroAplicacion>();
            accionesExternas = new List<AccionesExternas>();
            configApplicationInsightsDominio = new List<ConfigApplicationInsightsDominio>();
            textosPersonalizadosPlataforma = new List<TextosPersonalizadosPlataforma>();
            proyectoSinRegistroObligatorio = new List<ProyectoSinRegistroObligatorio>();
            proyectoRegistroObligatorio = new List<ProyectoRegistroObligatorio>();
           
        }

        public static GestorParametroAplicacion Instance
        {
            get
            {
                if (instance==null)
                {
                    instance = new GestorParametroAplicacion();
                }
                return instance;
            }
        }

        public List<AD.EntityModel.ParametroAplicacion> ParametroAplicacion
        {
            get; set;
        }

        public List<ConfiguracionServiciosDominio> ListaConfiguracionServiciosDominio
        {
            get
            {
                return configuracionServiciosDominio;
            }
            set
            {
                configuracionServiciosDominio = value;
            }
        }

        public List<ConfiguracionServicios> ListaConfiguracionServicios
        {
            get
            {
                return configuracionServicios;
            }
            set
            {
                configuracionServicios = value;
            }
        }

        public List<ConfiguracionServiciosProyecto> ListaConfiguracionServiciosProyecto
        {
            get
            {
                return configuracionServiciosProyecto;
            }
            set
            {
                configuracionServiciosProyecto = value;
            }
        }

        public List<ConfiguracionBBDD> ListaConfiguracionBBDD
        {
            get
            {
                return configuracionBBDD;
            }
            set
            {
                configuracionBBDD = value;
            }
        }

        public List<AccionesExternas> ListaAccionesExternas
        {
            get; set;
            
        }

        public List<ConfigApplicationInsightsDominio> ListaConfigApplicationInsightsDominio
        {
            get;set;

        }

        public List<TextosPersonalizadosPlataforma> ListaTextosPersonalizadosPlataforma
        {
            get;set;

        }

        public List<ProyectoSinRegistroObligatorio> ListaProyectoSinRegistroObligatorio
        {
            get ;set;

        }

        public List<ProyectoRegistroObligatorio> ListaProyectoRegistroObligatorio
        {
            get;set;

        }
    }
}
