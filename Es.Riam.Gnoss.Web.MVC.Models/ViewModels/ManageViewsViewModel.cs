using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View Model de la página de Administrar vistas
    /// </summary>
    [Serializable]
    public class ManageViewsViewModel
    {
        /// <summary>
        /// En umeración con las acciones disponibles para cada vista
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// Subir la vista
            /// </summary>
            Upload = 0,
            /// <summary>
            /// Descargar la vista
            /// </summary>
            Download = 1,
            /// <summary>
            /// Elminar la vista
            /// </summary>
            Delete = 2,
            /// <summary>
            /// Descargar la vista original
            /// </summary>
            DownloadOriginal = 3,
            /// <summary>
            /// Guarda cambios adicionales
            /// </summary>
            Save = 4,
        }

        /// <summary>
        /// Enumeración con los datos extra disponibles para cargar
        /// </summary>
        public enum ExtraInformation
        {
            Resources = 0,
            ResourcesExtra = 1,
            Identities = 2,
            IdentitiesExtra = 3
        }

        public Guid Personalizacion { get; set; }

        /// <summary>
        /// Lista de vistas personalizables editadas
        /// </summary>
        public List<string> ListEditedViews { get; set; }
        /// <summary>
        /// Lista de vistas personalizables sin editar
        /// </summary>
        public List<string> ListOriginalViews { get; set; }

        /// <summary>
        /// Lista de formularios semanticos personalizables editados
        /// </summary>
        public List<string> ListEditedFormsViews { get; set; }
        /// <summary>
        /// Lista de formularios semanticos personalizables sin editar
        /// </summary>
        public List<string> ListOriginalFormsViews { get; set; }

        /// <summary>
        /// Lista de vistas del servicio de resultados editados
        /// </summary>
        public List<string> ListEditedResultsServiceViews { get; set; }
        /// <summary>
        /// Lista de vistas del servicio de resultados sin editar
        /// </summary>
        public List<string> ListOriginalResultsServiceViews { get; set; }

        /// <summary>
        /// Lista de vistas del servicio de facetas editados
        /// </summary>
        public List<string> ListEditedFacetedServiceViews { get; set; }
        /// <summary>
        /// Lista de vistas del servicio de facetas sin editar
        /// </summary>
        public List<string> ListOriginalFacetedServiceViews { get; set; }

        public List<string> ListDomainsShared { get; set; }

        public List<CMSComponentViewModel> ListCMSComponents { get; set; }
        public List<CMSResourceViewModel> ListCMSResources { get; set; }
        public List<CMSListResourceViewModel> ListCMSListResources { get; set; }
        public List<CMSGroupComponentViewModel> ListCMSGroupComponents { get; set; }
        public string PathNameResourceDefault { get; set; }
        public string PathNameListResourcesDefault { get; set; }
        public string PathNameGroupComponentsDefault { get; set; }


        public string UrlActionFacets { get; set; }
        public string UrlActionResults { get; set; }
        public string UrlActionWeb { get; set; }
        public string UrlActionCMS { get; set; }
        public string UrlActionCMSExtra { get; set; }
        public string UrlActionInvalidateViews { get; set; }
        public string UrlActionShareViews { get; set; }
        public string UrlActionStopSharing { get; set; }
		public string OKMessage { get; set; }


        [Serializable]
        public class CMSComponentViewModel
        {
            /// <summary>
            /// Path de la vista ('/Views/CMSPagina/HTML/_HTML.cshtml')
            /// </summary>
            public string PathName { get; set; }
            /// <summary>
            /// Nombre de la vista ('HTML')
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Diccionario clave:identificador de la vista, nombre: nombre de la personalización de la vista
            /// </summary>
            public Dictionary<Guid, string> CustomizationName { get; set; }
        }

        [Serializable]
        public class CMSResourceViewModel
        {
            /// <summary>
            /// Path de la vista
            /// </summary>
            public string PathName { get; set; }
            /// <summary>
            /// Nombre de la vista ('HTML')
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Índica si se trata de una vista genérica (personalizada o no)
            /// </summary>
            public bool Generic { get; set; }
            /// <summary>
            /// Identificador
            /// </summary>
            public Guid CustomizationID { get; set; }
            /// <summary>
            /// Datos extra para cargar
            /// </summary>
            public Dictionary<ExtraInformation, bool> ExtraInformation { get; set; }
        }

        [Serializable]
        public class CMSListResourceViewModel
        {
            /// <summary>
            /// Path de la vista
            /// </summary>
            public string PathName { get; set; }
            /// <summary>
            /// Nombre de la vista ('HTML')
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Índica si se trata de una vista genérica (personalizada o no)
            /// </summary>
            public bool Generic { get; set; }
            /// <summary>
            /// Identificador
            /// </summary>
            public Guid CustomizationID { get; set; }
        }

        [Serializable]
        public class CMSGroupComponentViewModel
        {
            /// <summary>
            /// Path de la vista
            /// </summary>
            public string PathName { get; set; }
            /// <summary>
            /// Nombre de la vista ('HTML')
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Índica si se trata de una vista genérica (personalizada o no)
            /// </summary>
            public bool Generic { get; set; }
            /// <summary>
            /// Identificador
            /// </summary>
            public Guid CustomizationID { get; set; }
        }
    }
}
