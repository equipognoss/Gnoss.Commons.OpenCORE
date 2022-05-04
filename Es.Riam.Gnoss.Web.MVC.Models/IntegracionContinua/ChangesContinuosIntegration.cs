using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua
{
    public class ChangesContinuosIntegration
    {
        public string NombreArchivo { get; set; }
        public Guid Entorno { get; set; }
        public string Tipo { get; set; }
        public string Path { get; set; }
    }
}
