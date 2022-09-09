using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// Modelo de la vista de edición de administrar cookies
    /// </summary>
    [Serializable]
    public class CookieEditModel
    {
        /// <summary>
        /// Id de la cookie
        /// </summary>
        public string CookieID { get; set; }

        /// <summary>
        /// Nombre de la cookie
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Categoría de la cookie
        /// </summary>
        public string Categoria { get; set; }

        /// <summary>
        /// Descripción de la cookie
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Tipo de la cookie 0 -> Persistent, 1 -> Session, 2 -> Third party
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Si se ha eliminado la cookie
        /// </summary>
        public string Deleted { get; set; }

        /// <summary>
        /// Si se ha modificado la cookie
        /// </summary>
        public string EsModificada { get; set; }
    }
}
