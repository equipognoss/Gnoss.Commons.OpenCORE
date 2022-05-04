using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Modelo de actividad reciente (Temporal)
    /// </summary>
    [Serializable]
    public partial class RecentActivity
    {
        /// <summary>
        /// Lista de HTML de los elementos de la actividad reciente (Temporal)
        /// </summary>
        public List<RecentActivityItem> RecentActivityItems { get; set; }
        //public List<string> RecentActivityItems { get; set; }
        /// <summary>
        /// Número de pagina
        /// </summary>
        public int NumPage { get; set; }
        /// <summary>
        /// Numero de elementos por pagina
        /// </summary>
        public int NumItemsPage { get; set; }
        /// <summary>
        /// Indica el tipo de actividad reciente (Home comunidad, perfil.....)
        /// </summary>
        public int TypeActivity { get; set; }
        /// <summary>
        /// Indica la url para cargar mas actividad reciente
        /// </summary>
        public string UrlLoadMoreActivity { get; set; }
        /// <summary>
        /// Indica la clave del componente al que pertenece la actividad reciente(Guid.Empty si no pertenece a ningun componente)
        /// </summary>
        public Guid ComponentKey { get; set; }
        /// <summary>
        /// Indica la clave del perfil si estamos viendo la actividad reciente de un perfil
        /// </summary>
        public Guid? ProfileKey { get; set; }
    }

    [Serializable]
    public partial class RecentActivityItem
    {
        public string Key { get; set; }
        public bool Readed { get; set; }
        public string UrlCommunity { get; set; }
        public string NameCommunity { get; set; }
    }

    [Serializable]
    public partial class RecentActivityResourceItem : RecentActivityItem
    {
        public ResourceModel Resource { get; set; }
        public List<ResourceEventModel> Events { get; set; }
    }

    [Serializable]
    public partial class RecentActivityMemberItem : RecentActivityItem
    {
        public ProfileModel Profile { get; set; }
    }

    [Serializable]
    public partial class RecentActivitySomeMemberItem : RecentActivityItem
    {
        public int NumProfile { get; set; }
    }

}
