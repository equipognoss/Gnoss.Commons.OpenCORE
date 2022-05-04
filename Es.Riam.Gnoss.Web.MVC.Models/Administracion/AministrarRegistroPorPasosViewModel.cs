using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// View model para el controlador de administrar registro por pasos
    /// </summary>
    public class AministrarRegistroPorPasosViewModel
    {
        /// <summary>
        /// Lista de los servicios que tengo en la página
        /// </summary>
        public List<PasoRegistroModel> ListaServicios { get; set; }
        public List<string> ListaPestanyasMenu { get; set; }
    }
}
