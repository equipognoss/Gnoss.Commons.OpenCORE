using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// View Model para el controlador AdministracionSearch
    /// </summary>
    [Serializable]
    public class AdministrarSearchViewModel
    {
        /// <summary>
        /// Propiedades para autocompletar la busqueda
        /// </summary>
        public string PropiedadesParaAutocompletar { get; set; }

        /// <summary>
        /// Propiedades para el texto libre
        /// </summary>
        public string PropiedadesParaTxtLibre { get; set; }

        /// <summary>
        /// Lista de las etiquetas que se van añadiendo
        /// </summary>
        public string TagsAutocompletar { get; set; }

        /// <summary>
        /// Lista de las etiquetas que se van añadiendo
        /// </summary>
        public string TagsTxtLibre { get; set; }
    }
}
