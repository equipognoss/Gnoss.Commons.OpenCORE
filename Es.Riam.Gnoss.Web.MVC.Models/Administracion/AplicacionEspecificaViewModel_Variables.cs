using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecificaViewModel_Variables
    {
        public List<Variables> variables { get; set; }
        public string NombreCortoPersonalizacion { get; set; }
        public Guid EntornoID { get; set; }
        public Guid AplicacionID { get; set; }
        public string UrlBaseServicios { get; set; }
    }
}
