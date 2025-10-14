using Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarVistasDiagramasModel
    {
        /// <summary>
        /// Lista de clases en el modelo OWL
        /// </summary>
        public List<ClaseOwl> Clases { get; set; } = new List<ClaseOwl>();

        /// <summary>
        /// Lista de propiedades en el modelo OWL
        /// </summary>
        public List<PropiedadOwl> Propiedades { get; set; } = new List<PropiedadOwl>();

        /// <summary>
        /// Lista de relaciones entre los elementos en el modelo OWL
        /// </summary>
        public List<RelacionOwl> Relaciones { get; set; } = new List<RelacionOwl>();
    }

    public class ClaseOwl
    {
        /// <summary>
        /// Nombre de la clase
        /// </summary>
        public string NombreClase { get; set; }

        /// <summary>
        /// Etiqueta de la clase, puede ser opcional
        /// </summary>
        public string Label { get; set; } = null;

        /// <summary>
        /// Nombre de la ontología a la que pertenece
        /// </summary>
        public string Ontologia { get; set; }
    }

    public class PropiedadOwl
    {
        /// <summary>
        /// Nombre de la propiedad
        /// </summary>
        public string NombrePropiedad { get; set; }

        /// <summary>
        /// Etiqueta de la propiedad, puede ser opcional
        /// </summary>
        public string Label { get; set; } = null;

        /// <summary>
        /// Comentario de la propiedad, puede ser opcional
        /// </summary>
        public string Comentario { get; set; } = null;

        /// <summary>
        /// Dominio de la propiedad (la clase a la que pertenece)
        /// </summary>
        public string Dominio { get; set; }

        /// <summary>
        /// Rango de la propiedad (el tipo de la propiedad)
        /// </summary>
        public string Rango { get; set; }

        /// <summary>
        /// Cardinalidad de la propiedad
        /// </summary>
        public string Cardinalidad { get; set; }
    }

    public class RelacionOwl
    {
        /// <summary>
        /// Origen de la relación
        /// </summary>
        public string Origen { get; set; }

        /// <summary>
        /// Destino de la relación
        /// </summary>
        public string Destino { get; set; }

        /// <summary>
        /// Predicado de la relación (nombre de la propiedad que conecta las entidades)
        /// </summary>
        public string Predicado { get; set; }

        /// <summary>
        /// Tipo de la relación
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TipoRelacionOwl Tipo { get; set; }
    }

    public enum TipoRelacionOwl { [EnumMember(Value = "Herencia")] Herencia, [EnumMember(Value = "Asociacion")] Asociacion }
}
