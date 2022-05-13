using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public enum TipoPestanyaMenu
    {
        Home = 0,
        Indice = 1,
        Recursos = 2,
        Preguntas = 3,
        Debates = 4,
        Encuestas = 5,
        PersonasYOrganizaciones = 6,
        AcercaDe = 7,
        CMS = 8,
        BusquedaSemantica = 9,
        EnlaceInterno = 10,
        EnlaceExterno = 11,
        BusquedaAvanzada = 12,
        Comunidades = 13,
        Contribuciones = 14,
        Borradores = 15

    }

    /// <summary>
    /// Modelo de pestañas de una comunidad
    /// </summary>
    [Serializable]
    public partial class TabModel
    {
        public TabModel()
        {
            ListaFacetas = new List<FacetasTabModel>();
        }
        /// <summary>
        /// Identificador de la pestaña
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Nombre de la pestaña
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool EsNombrePorDefecto { get; set; }
        /// <summary>
        /// Url de la pestaña
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Indica si la url de la pestaña es la de por defecto (ej: pestaña de tipo recursos => /recursos )
        /// </summary>
        public bool EsUrlPorDefecto { get; set; }
        /// <summary>
        /// Nombre corto de la pestaña
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// Clase del body en esta pestaña
        /// </summary>
        public string ClassCSSBody { get; set; }
        /// <summary>
        /// Tipo de la pestaña
        /// </summary>
        public TipoPestanyaMenu Type { get; set; }
        /// <summary>
        /// Indica si la pestaña esta activa o no
        /// </summary>
        public bool Active { get; set; }
        /// <summary>
        /// Indica si la pestaña esta eliminada o no
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// Indica si la pestaña esta eliminada o no
        /// </summary>
        public bool Modified { get; set; }
        /// <summary>
        /// Indica si la pestaña es nueva, añadida en esta peticion
        /// </summary>
        public bool Nueva { get; set; }
        /// <summary>
        /// Indica si la pestaña esta visible o no
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Indica si la pestaña esta visible o no para los usuarios que no tienen acceso
        /// </summary>
        public bool VisibleSinAcceso { get; set; }
        /// <summary>
        /// Indica si la pestaña se abre en la ventana actual o en una nueva
        /// </summary>
        public bool OpenInNewWindow { get; set; }
        /// <summary>
        /// Indica la privacidad de la pestaña
        /// </summary>
        public short Privacidad { get; set; }
        /// <summary>
        /// Lista de perfiles que pueden ver esta pestaña, en caso de que sea privada
        /// </summary>
        public Dictionary<Guid, string> PrivacidadPerfiles { get; set; }
        /// <summary>
        /// Lista de grupos que pueden ver esta pestaña, en caso de que sea privada
        /// </summary>
        public Dictionary<Guid, string> PrivacidadGrupos { get; set; }
        /// <summary>
        /// Html que se muestra en la pestaña si el usuario que intenta verla no tiene acceso
        /// </summary>
        public string HtmlAlternativoPrivacidad { get; set; }
        /// <summary>
        /// Identificador de la pestaña padre
        /// </summary>
        public Guid ParentTabKey { get; set; }
        /// <summary>
        /// Orden de la pestaña con respecto a las de su mismo nivel
        /// </summary>
        public short Order { get; set; }
        /// <summary>
        /// Si la pestaña es de tipo home del CMS, indica el tipo de home que es
        /// </summary>
        /// 
        public HomeCMTabSModel HomeCMS { get; set; }
        /// <summary>
        /// Meta descripcion de la pestaña
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Modelo que indica el tipo de home que es
        /// </summary>
        public partial class HomeCMTabSModel
        {
            /// <summary>
            /// Indica si la home es para todos los usuarios
            /// </summary>
            public bool HomeTodosUsuarios { get; set; }
            /// <summary>
            /// Indica si la home es para los miembros
            /// </summary>
            public bool HomeMiembros { get; set; }
            /// <summary>
            /// Indica si la home es para los no miebros
            /// </summary>
            public bool HomeNoMiembros { get; set; }
        }
        /// <summary>
        /// Opciones de busqueda de la pestaña, si la pestaña es de busqueda
        /// </summary>
        public SearchTabModel OpcionesBusqueda { get; set; }

        /// <summary>
        /// Modelo de administracion de busqueda de una pestaña
        /// </summary>
        [Serializable]
        public partial class SearchTabModel
        {
            /// <summary>
            /// 
            /// </summary>
            public bool ValoresPorDefecto { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string CampoFiltro { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<FiltroOrden> FiltrosOrden { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string GruposConfiguracion { get; set; }
            /// <summary>
            /// 
            /// </summary>
            [Serializable]
            public partial class FiltroOrden
            {
                /// <summary>
                /// 
                /// </summary>
                public string Nombre { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string Filtro { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public short Orden { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public bool Deleted { get; set; }
            }

            /// <summary>
            /// 
            /// </summary>
            public ViewsSearchTabModel OpcionesVistas { get; set; }

            /// <summary>
            /// Modelo de administracion de busqueda de una pestaña
            /// </summary>
            [Serializable]
            public partial class ViewsSearchTabModel
            {
                /// <summary>
                /// 
                /// </summary>
                public short VistaPorDefecto { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public bool VistaListado { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public bool VistaMosaico { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public bool VistaMapa { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public bool VistaGrafico { get; set; }

                public string PosicionCentralMapa { get; set; }
                //public string PropiedadLatitud { get; set; }
                //public string PropiedadLongitud { get; set; }

                //public List<GraficosSearchTabModel> Graficos { get; set; }

                //[Serializable]
                //public partial class GraficosSearchTabModel
                //{
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public Guid Key { get; set; }
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public string Nombre { get; set; }
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public string Ontologia { get; set; }
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public string SelectConsultaVirtuoso { get; set; }
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public string FiltrosConsultaVirtuoso { get; set; }
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public string JavascriptGraficoBase { get; set; }
                //    /// <summary>
                //    /// 
                //    /// </summary>
                //    public string JavascriptGraficoBusqueda { get; set; }
                //}
            }

            /// <summary>
            /// 
            /// </summary>
            public short NumeroResultados { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool MostrarFacetas { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool AgruparFacetasPorTipo { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool MostrarEnBusquedaCabecera { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool MostrarCajaBusqueda { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public Guid ProyectoOrigenBusqueda { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool OcultarResultadosSinFiltros { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public string TextoBusquedaSinResultados { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool IgnorarPrivacidadEnBusqueda { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public bool OmitirCargaInicialFacetasResultados { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public string RelacionMandatory { get; set; }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> ListaIdiomasDisponibles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<ExportacionSearchTabModel> ListaExportaciones { get; set; }

        [Serializable]
        public partial class ExportacionSearchTabModel
        {
            /// <summary>
            /// 
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public short Orden { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool Deleted { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Nombre { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Dictionary<Guid, string> GruposPermiso { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<PropiedadesExportacionSearchTabModel> ListaPropiedades { get; set; }

            [Serializable]
            public partial class PropiedadesExportacionSearchTabModel
            {
                /// <summary>
                /// 
                /// </summary>
                public short Orden { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public bool Deleted { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string Nombre { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string Ontologia { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string Propiedad { get; set; }
                /// <summary>
                /// 
                /// </summary>
                public string DatoExtraPropiedad { get; set; }
            }
        }
        ///<summary>
        /// 
        /// </summary>
        public List<FacetasTabModel> ListaFacetas { get; set; }

        [Serializable]
        public partial class FacetasTabModel
        {
            ///<summary>
            ///
            /// </summary>
            public string Faceta { get; set; }
            ///<summary>
            /// 
            /// </summary>
            public string ObjetoConocimiento { get; set; }
            ///<summary>
            /// 
            /// </summary>
            public bool Deleted { get; set; }
            ///<summary>
            /// 
            /// </summary>
            public Guid ClavePestanya { get; set; }
        }
    }
}
