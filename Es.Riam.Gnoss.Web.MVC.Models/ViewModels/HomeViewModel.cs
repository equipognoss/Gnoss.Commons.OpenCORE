using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View model de la home de la comunidad
    /// </summary>
    [Serializable]
    public class HomeViewModel
    {
        //public int NumberOfResources { get; set; }

        //public int NumberOfPersonOrganizations { get; set; }

        /// <summary>
        /// Lista de los perfiles de los usuarios mas activos
        /// </summary>
        public List<ProfileModel> MostActiveUsers { get; set; }
        /// <summary>
        /// Lista de los ultimos usuarios registrados en la comunidad
        /// </summary>
        public List<ProfileModel> LastUsers { get; set; }
        /// <summary>
        /// Actividad reciente en la comunidad
        /// </summary>
        public RecentActivity RecentActivity { get; set; }
        /// <summary>
        /// Lista de gadgets configurados en la comunidad
        /// </summary>
        public List<GadgetModel> Gadgets { get; set; }
    }

    /// <summary>
    /// View model de la home catalogo de la comunidad
    /// </summary>
    [Serializable]
    public class HomeCatalogoViewModel
    {
        public List<SectionHome> Sections { get; set; }

        /// <summary>
        /// Modelo de una seccion
        /// </summary>
        [Serializable]
        public class SectionHome
        {
            /// <summary>
            /// Tipos de vista
            /// </summary>
            public enum ResourceViewType
            {
                /// <summary>
                /// Listado
                /// </summary>
                List = 0,

                /// <summary>
                /// Mosaicp
                /// </summary>
                Grid = 1,
            }

            /// <summary>
            /// Titulo de la seccion
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// Tipo de vista
            /// </summary>
            public ResourceViewType ViewType { get; set; }
            /// <summary>
            /// Lista de recursos de la seccion
            /// </summary>
            public List<ResourceModel> Resources { get; set; }
        }
    }
}
