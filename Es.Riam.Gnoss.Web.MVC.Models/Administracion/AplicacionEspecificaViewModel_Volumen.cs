using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecificaViewModel_Volumen
    {
        public List<Volumen> Volumen { get; set; }

        // Nombre corto de la personalización
        public string NombreCortoPersonalizacion { get; set; }
        public string UrlRepositorio { get; set; }

        public string UrlBaseServicios { get; set; }
    }
}
