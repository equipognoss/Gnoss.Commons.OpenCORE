using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View Model de la pagina Home
    /// </summary>
    [Serializable]
    public class HomeConectedViewModel
    {
        /// <summary>
        /// Actividad reciente en la comunidad
        /// </summary>
        public RecentActivity RecentActivity { get; set; }
    }
}
