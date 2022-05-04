using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// View Model de la pagina de indice
    /// </summary>
    [Serializable]
    public class IndexViewModel
    {
        /// <summary>
        /// Lista de categorias
        /// </summary>
        public List<CategoryModel> Categories { get; set; }
        /// <summary>
        /// Url para construir los enlaces de las categorias
        /// </summary>
        public string UrlBaseCategories { get; set; }
    }
}
