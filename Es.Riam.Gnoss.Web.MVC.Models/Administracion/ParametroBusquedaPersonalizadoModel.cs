namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class ParametroBusquedaPersonalizadoModel
    {
        public ParametroBusquedaPersonalizadoModel()
        {
            NombreParametro = string.Empty;
            WhereParametro = string.Empty;
            OrderByParametro = string.Empty;
            WhereFacetaParametro = string.Empty;
        }
        public ParametroBusquedaPersonalizadoModel(string nombreParametro, string whereParametro, string orderByParametro, string whereFacetaParametro)
        {
            NombreParametro = nombreParametro;
            WhereParametro = whereParametro;
            OrderByParametro = orderByParametro;
            WhereFacetaParametro = whereFacetaParametro;
        }
        /// <summary>
        /// Nombre del parámetro de búsqueda
        /// </summary>
        public string NombreParametro { get; set; }
        /// <summary>
        /// Where del parametro de búsqueda
        /// </summary>
        public string WhereParametro { get; set; }
        /// <summary>
        /// OrderBy del parámetro de búsqueda
        /// </summary>
        public string OrderByParametro { get; set; }
        /// <summary>
        /// Where de la faceta
        /// </summary>
        public string WhereFacetaParametro { get; set; }
        public bool Nueva { get; set; }
        public bool Deleted { get; set; }
    }
}
