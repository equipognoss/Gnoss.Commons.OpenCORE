using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View model de las pagina de busqueda
    /// </summary>
    [Serializable]
    public class ManageSuscriptionsViewModel
    {
        /// <summary>
        /// Lista de comunidades a las que estas suscrito
        /// </summary>
        public Dictionary<Guid, CommunityModel> Communities { get; set; }
        /// <summary>
        /// Lista de personas a las que estas suscrito
        /// </summary>
        public Dictionary<Guid, ProfileModel> Profiles { get; set; }
        /// <summary>
        /// Lista de organizaciones a los que estas suscrito
        /// </summary>
        public Dictionary<Guid, ProfileModel> Organizations { get; set; }
    }
}
