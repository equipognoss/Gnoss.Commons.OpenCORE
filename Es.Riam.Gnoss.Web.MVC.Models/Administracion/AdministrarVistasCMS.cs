using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarVistasCMS
    {
        public string Ruta { get; set; }
        public string Nombre { get; set; }
        public string HTML { get; set; }
        public string DatosExtra { get; set; }
        public Guid PersonalizacionComponenteID { get; set; }
    }
}
