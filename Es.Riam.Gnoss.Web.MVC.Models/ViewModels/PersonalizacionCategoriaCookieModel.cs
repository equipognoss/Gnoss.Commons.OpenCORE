using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
    /// <summary>
    /// Modelo que contiene una categoría de cookie con su estado (0 -> Pendiente, 1 -> Aceptada, 2 -> Denegada)
    /// </summary>
    [Serializable]
    public class PersonalizacionCategoriaCookieModel
    {
        /// <summary>
        /// Categoría de la cookie
        /// </summary>
        public CategoriaProyectoCookieViewModel CategoriaCookie { get; set; }

        /// <summary>
        /// Estado de la categoría
        /// </summary>
        public short Estado { get; set; }
    }

    /// <summary>
    /// Modelo para las categorías de las cookies para la vista
    /// </summary>
    [Serializable]
    public class CategoriaProyectoCookieViewModel
    {
        public CategoriaProyectoCookieViewModel() { }

        public Guid CategoriaID { get; set; }

        public string Nombre { get; set; }

        
        public string NombreCorto { get; set; }

        public string Descripcion { get; set; }

        public bool EsCategoriaTecnica { get; set; }

        public Guid ProyectoID { get; set; }
        
        public Guid OrganizacionID { get; set; }
    }

}
