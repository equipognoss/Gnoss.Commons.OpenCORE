using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View Model de la pagina de mis comunidades
    /// </summary>
    [Serializable]
    public class MyCommunitiesViewModel
    {
        /// <summary>
        /// Lista de comunidades
        /// </summary>
        public List<MyCommunityModel> Communities { get; set; }

        /// <summary>
        /// Modelo de comunidad
        /// </summary>
        [Serializable]
        public partial class MyCommunityModel
        {
            /// <summary>
            /// Identificador de la comunidad
            /// </summary>
            public Guid Key { get; set; }
            /// <summary>
            /// Identificador de la comunidad padre (Guid.Empty si no tiene padre)
            /// </summary>
            public Guid ParentKey { get; set; }
            /// <summary>
            /// Nombre de la comunidad
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Logo de la comunidad
            /// </summary>
            public string Logo { get; set; }
            /// <summary>
            /// Url de la comunidad
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// Tipo de acceso del proyecto: privado, publico....
            /// </summary>
            public CommunityModel.TypeAccessProject AccessType { get; set; }
        }
    }
}
