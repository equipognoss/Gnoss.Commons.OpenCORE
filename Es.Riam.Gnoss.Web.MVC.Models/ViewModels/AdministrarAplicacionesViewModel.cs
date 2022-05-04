using Es.Riam.Gnoss.Web.MVC.Models.OAuth;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View model de la pagina "AdministrarAplicaciones"
    /// </summary>
    [Serializable]
    public class AdministrarAplicacionesViewModel
    {

        public List<ConsumerModel> Aplicaciones { get; set; }
        public string URL { get; set; }
    }
}
