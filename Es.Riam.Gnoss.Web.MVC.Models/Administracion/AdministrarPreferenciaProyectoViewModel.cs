using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// Modelo de la página de administración de la página de administración de preferencia proyecto
    /// </summary>
    [Serializable]
    public class AdministrarPreferenciaProyectoViewModel
    {
        /// <summary>
        /// EL modelo del árbol del tesauro
        /// </summary>
        public ThesaurusEditorModel Thesaurus { get; set; }
        /// <summary>
        /// Si la página es multilenguaje
        /// </summary>
        public bool MultiLanguaje { get; set; }
        /// <summary>
        /// el idioma por defecto de la página
        /// </summary>
        public string IdiomaDefecto { get; set; }
        /// <summary>
        /// el idioma que tiene actualmente el tesauro
        /// </summary>
        public string IdiomaTesauro { get; set; }
        /// <summary>
        /// La acción que se le da al tesauro
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// Los pasos realizados en el tesauro
        /// </summary>
        public string PasosRealizados { get; set; }
        /// <summary>
        /// La categoria a la que pertenece el tesauro
        /// </summary>
        public KeyValuePair<Guid, Dictionary<string, string>> Categoria { get; set; }
    }
}
