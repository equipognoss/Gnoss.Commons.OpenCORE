using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    public class TranslatePropertyModel
    {
        /// <summary>
        /// Diccionario con los idiomas en los que está guardado los triples del recurso
        /// </summary>
        public Dictionary<string, string> OriginLanguges { get; set; }

        /// <summary>
        /// Diccionario con los idiomas disponibles en la plataforma y el traductor configurado
        /// </summary>
        public Dictionary<string, string> TargetLanguages { get; set; }

        /// <summary>
        /// Url para enviar la peticion de traducir un texto
        /// </summary>
        public string UrlTranslate { get; set; }

        /// <summary>
        /// Idioma actual del usuario
        /// </summary>
        public string CurrentLanguage { get; set; }

        /// <summary>
        /// Label de la propiedad a traducir
        /// </summary>
        public string PropertyName { get; set; }
    }
}
