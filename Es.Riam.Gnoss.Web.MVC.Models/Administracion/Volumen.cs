using System;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class Volumen
    {
        public Guid VolumenID { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
    }
}
