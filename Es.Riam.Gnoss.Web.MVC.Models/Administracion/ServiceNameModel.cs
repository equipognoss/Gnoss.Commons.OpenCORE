using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    [Serializable]
    public class ServiceNameModel
    {
        public string NombreServicio { get; set; }
        public string UrlServicio { get; set; }
        public bool Nueva { get; set; }
        public bool Deleted { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
