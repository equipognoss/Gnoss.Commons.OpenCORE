using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public List<CategoriaCookiesModel> ListaCategorias { get; set; }

    }

    public partial class CategoriaCookiesModel
    {
        public Guid CategoriaID { get; set; }

        public string Nombre { get; set; }

        public string NombreCorto { get; set; }

        public string Descripcion { get; set; }
    }

    public partial class CategoriaProyectoCookieModel
    {
        public Guid CategoriaID { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
        public string Descripcion { get; set; }
        public bool EsCategoriaTecnica { get; set; }
        public Guid ProyectoID { get; set; }
        public Guid OrganizacionID { get; set; }
        public bool Deleted { get; set; }
        public bool EsModificada { get; set; }
    }

    public partial class ProyectoCookieModel
    {
        public Guid CookieID { get; set; }
        public string Nombre { get; set; }
        public string NombreCorto { get; set; }
        public short Tipo { get; set; }
        public string Descripcion { get; set; }
        public bool EsEditable { get; set; }
        public Guid CategoriaID { get; set; }
        public Guid ProyectoID { get; set; }
        public Guid OrganizacionID { get; set; }
        public bool Deleted { get; set; }
        public bool EsModificada { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }

    public partial class CategoriasCookiesModel
    {
        public List<CategoriaProyectoCookieModel> Categorias { get; set; }
        public List<ProyectoCookieModel> Cookies { get; set; }
    }
}
