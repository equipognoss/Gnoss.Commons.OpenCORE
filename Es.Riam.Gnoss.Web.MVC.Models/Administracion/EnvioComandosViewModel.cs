using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class EnvioComandosViewModels
    {
        public string comando { get; set; }
        public string nombreAplicacion { get; set; }
        public Guid EntornoID { get; set; }
    }
}