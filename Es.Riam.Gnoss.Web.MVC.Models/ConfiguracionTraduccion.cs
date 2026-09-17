using Es.Riam.Gnoss.Web.MVC.Models.ConsultasSparql;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    public class ConfiguracionTraduccion
    {
        /// <summary>
        /// Diccionario con los idiomas en los que está guardado los triples del recurso
        /// </summary>
        public Dictionary<string, string> IdiomasOrigen { get; set; }

        /// <summary>
        /// Diccionario con los idiomas disponibles en la plataforma y el traductor configurado
        /// </summary>
        public Dictionary<string, string> IdiomasDestino { get; set; }

        /// <summary>
        /// Url para enviar la peticion de traducir un texto
        /// </summary>
        public string UrlTraduccion { get; set; }

        /// <summary>
        /// Idioma actual del usuario
        /// </summary>
        public string IdiomaActual { get; set; }

        public PropiedadATraducir Propiedad { get; set; }

    }

    public class PropiedadATraducir
    {
        /// <summary>
        /// Entidad a la que pertenece la propiedad
        /// </summary>
        public string Entidad { get; set; }

        /// <summary>
        /// Label de la propiedad a traducir
        /// </summary>
        public string NombrePropiedad { get; set; }

        /// <summary>
        /// Propiedad a traducir
        /// </summary>
        public string Propiedad { get; set; }

        /// <summary>
        /// Indica si la propiedad puede tener varios valores
        /// </summary>
        public bool EsMultiValor { get; set; }

        /// <summary>
        /// Valor actual de la propiedad
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Traducciones de la propiedad
        /// </summary>
        public List<TraduccionIdioma> Traducciones { get; set; } = new List<TraduccionIdioma>();
    }
}
