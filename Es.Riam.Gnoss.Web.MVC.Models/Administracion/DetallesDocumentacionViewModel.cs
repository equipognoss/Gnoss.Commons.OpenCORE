using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class DetallesDocumentacionViewModel
    {
        // Idioma por defecto (puede ayudarte en la vista para seleccionar la pestaña activa)
        public string IdiomaPorDefecto { get; set; } = "es";

        // Propiedades por idioma
        public Dictionary<string, string> TituloIdioma { get; set; }
        public Dictionary<string, string> AutoresIdioma { get; set; }
        public Dictionary<string, string> OntologiasImportadasIdioma { get; set; }
        public Dictionary<string, string> EnlaceOWLIdioma { get; set; }
        public Dictionary<string, string> DescripcionIdioma { get; set; }


        // Propiedades no traducibles
        public string Privado { get; set; } = "off";
        public IFormFile Imagen { get; set; }
        public string UrlImagen { get; set; }
        public string Licencia { get; set; }
        public string UrlLicencia { get; set; }

    }
}

