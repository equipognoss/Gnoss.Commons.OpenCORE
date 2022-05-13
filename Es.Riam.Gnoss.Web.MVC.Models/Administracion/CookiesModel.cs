using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public partial class CookiesModel
    {
        /// <summary>
        /// Id de la cookie
        /// </summary>
        public Guid CookieID { get; set; }

        /// <summary>
        /// Nombre de la cookie
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Categoría de la cookie
        /// </summary>
        public CategoriaCookiesModel Categoria { get; set; }

        /// <summary>
        /// Descripción de la cookie
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Tipo de la cookie 0 -> Persistent, 1 -> Session, 2 -> Third party
        /// </summary>
        public int Tipo { get; set; }

        /// <summary>
        /// Si se ha eliminado la cookie
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Si se ha modificado la cookie
        /// </summary>
        public bool EsModificada { get; set; }

        public List<CategoriaCookiesModel> ListaCategorias { get; set; }

    }

    public partial class CategoriaCookiesModel
    {
        public Guid CategoriaID { get; set; }

        public string Nombre { get; set; }

        public string NombreCorto { get; set; }

        public string Descripcion { get; set; }
    }
}
