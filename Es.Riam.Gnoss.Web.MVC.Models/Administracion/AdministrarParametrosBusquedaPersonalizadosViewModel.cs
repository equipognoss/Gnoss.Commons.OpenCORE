using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    /// <summary>
    /// Modelo de administrar las consultas de SPARQL de la tabla ProyectoSearchPersonalizado
    /// </summary>
    public class AdministrarParametrosBusquedaPersonalizadosViewModel
    {
        /// <summary>
        /// Lista de los parámetros de búsqueda personalizados
        /// </summary>
        public List<ParametroBusquedaPersonalizadoModel> ListaParametros { get; set; }
    }
}
