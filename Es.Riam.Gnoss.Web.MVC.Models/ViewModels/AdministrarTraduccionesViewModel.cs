using Es.Riam.Gnoss.Web.MVC.Models.AdministrarTraducciones;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View model de la pagina "AdministrarTraducciones"
    /// </summary>
    [Serializable]
    public class AdministrarTraduccionesViewModel
    {
        public AdministrarTraduccionesViewModel()
        {
        }
        public List<TextoTraducidoModel> Textos { get; set; }

        public string URLActionCrearTexto { get; set; }

        public string URLActionEditarTexto { get; set; }

        public string URLActionCrearEntradas { get; set; }

        public string URLActionEliminarEntradas { get; set; }

        public bool traduccionesActivas { get; set; }

    }
}
