using System;
using System.ComponentModel.DataAnnotations;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class Variables
    {
        public string NombreVariable { get; set; }
        public string ValorDesarrollo {get;set; }
        public string ValorPreproduccion {get;set; }
        public string ValorProduccion {get;set; }
    }
}
