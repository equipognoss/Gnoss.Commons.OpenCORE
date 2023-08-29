

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class EditOntologyViewModel
    {
        #region Propiedades
        public string Name { get; set; }
        public string NameOWL { get; set; }
        public string Description { get; set; }
        public bool Principal { get; set; }
        public bool EditXML { get; set; }
        public bool GenericXML { get; set; }
        public bool NoUseXML { get; set; }
        public bool EditCSS { get; set; }
        public bool GenericCSS { get; set; }
        public bool EditIMG { get; set; }
        public bool GenericIMG { get; set; }
        public bool EditJS { get; set; }
        public bool GenericJS { get; set; }
        public bool Protected { get; set; }
        public string OntologyProperties { get; set; }
        public bool Deleted { get; set; }
        public Guid OntologyID { get; set; }
        #endregion


        #region Archivos obtenidos web
        [JsonIgnore]
        public IFormFile JS { get; set; }
        [JsonIgnore]
        public IFormFile IMG { get; set; }
        [JsonIgnore]
        public IFormFile CSS { get; set; }
        [JsonIgnore]
        public IFormFile XML { get; set; }
        [JsonIgnore]
        public IFormFile OWL { get; set; }
        #endregion


        #region Archivos integracion continua
        public string FicheroJS { get; set; }
        public string FicheroOWL { get; set; }
        public string FicheroXML { get; set; }
        public string FicheroIMG { get; set; }
        public string FicheroCSS { get; set; }
        #endregion
    }
}
