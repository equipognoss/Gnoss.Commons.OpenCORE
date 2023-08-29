using System;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class VolumenAplicacion
    {
        public Guid AplicacionID { get; set; }
        public Guid VolumenID { get; set; }
        public string NombreAplicacion { get; set; }
        public string NombreVolumen { get; set; }
        public string Ruta { get; set; }
        public bool TipoVolumen { get; set; }
    }
}
