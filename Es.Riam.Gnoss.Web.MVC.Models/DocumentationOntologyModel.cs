using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    public class DocumentationOntologyModel
    {
        public string Html {  get; set; }
        public string UrlImagen { get; set; }
        public bool Modificado { get; set; }
        public string Idioma { get; set; }
        public DetallesDocumentacionViewModel Detalles { get; set; }
    }
}
