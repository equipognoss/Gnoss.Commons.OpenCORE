using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio
{
    [Serializable]
    public class GestorParametroGeneral
    {
        private List<ParametroGeneral> parametroGeneral;
        private List<ConfiguracionAmbitoBusquedaProyecto> configuracionAmbitoBusquedaProyecto;
        private List<ProyectoMetaRobots> proyectoMetaRobots;
        private List<TextosPersonalizadosProyecto> textosPersonalizadosProyecto;
        private List<ParametroProyecto> parametroProyecto;
        private List<TextosPersonalizadosPersonalizacion> textosPersonalizadosPersonalizacion;
        private List<ProyectoElementoHtml> proyectoElementoHtml;
        private List<ProyectoElementoHTMLRol> proyectoElementoHTMLRol;
        private List<ProyectoRDFType> proyectoRDFType;

        public GestorParametroGeneral()
        {
            this.parametroGeneral = new List<ParametroGeneral>();
            this.proyectoElementoHtml = new List<ProyectoElementoHtml>();
            this.configuracionAmbitoBusquedaProyecto = new List<ConfiguracionAmbitoBusquedaProyecto>();
            this.proyectoMetaRobots = new List<ProyectoMetaRobots>();
            this.textosPersonalizadosPersonalizacion = new List<TextosPersonalizadosPersonalizacion>();
            this.textosPersonalizadosProyecto = new List<TextosPersonalizadosProyecto>();
            this.parametroProyecto = new List<ParametroProyecto>();
            this.proyectoElementoHTMLRol = new List<ProyectoElementoHTMLRol>();
            this.proyectoRDFType = new List<ProyectoRDFType>();
        }

        public List<ParametroGeneral> ListaParametroGeneral
        {
            get
            {
                return this.parametroGeneral;
            }
            set
            {
                this.parametroGeneral = value;
            }
        }

        public List<ConfiguracionAmbitoBusquedaProyecto> ListaConfiguracionAmbitoBusquedaProyecto
        {
            get
            {
                return this.configuracionAmbitoBusquedaProyecto;
            }
            set
            {
                this.configuracionAmbitoBusquedaProyecto = value;
            }
        }

        public List<ProyectoMetaRobots> ListaProyectoMetaRobots
        {
            get
            {
                return this.proyectoMetaRobots;
            }
            set
            {
                this.proyectoMetaRobots = value;
            }
        }

        public List<TextosPersonalizadosProyecto> ListaTextosPersonalizadosProyecto
        {
            get
            {
                return this.textosPersonalizadosProyecto;
            }
            set
            {
                this.textosPersonalizadosProyecto = value;
            }
        }

        public List<ParametroProyecto> ListaParametroProyecto
        {
            get
            {
                return this.parametroProyecto;
            }
            set
            {
                this.parametroProyecto = value;
            }
        }

        public List<TextosPersonalizadosPersonalizacion> ListaTextosPersonalizadosPersonalizacion
        {
            get
            {
                return this.textosPersonalizadosPersonalizacion;
            }
            set
            {
                this.textosPersonalizadosPersonalizacion = value;
            }
        }

        public List<ProyectoElementoHtml> ListaProyectoElementoHtml
        {
            get
            {
                return this.proyectoElementoHtml;
            }
            set
            {
                this.proyectoElementoHtml = value;
            }
        }

        public List<ProyectoElementoHTMLRol> ListaProyectoElementoHTMLRol
        {
            get
            {
                return this.proyectoElementoHTMLRol;
            }
            set
            {
                this.proyectoElementoHTMLRol = value;
            }
        }

        public List<ProyectoRDFType> ListaProyectoRDFType
        {
            get
            {
                return this.proyectoRDFType;
            }
            set
            {
                this.proyectoRDFType = value;
            }
        }


    }
}
