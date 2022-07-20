namespace Es.Riam.Gnoss.Web.MVC.Models
{
    /// <summary>
    /// Enumeración para distinguir tipos de roles en la organización
    /// </summary>
    public enum TipoAdministradoresOrganizacion
    {
        /// <summary>
        /// Administrador
        /// </summary>
        Administrador = 0,
        /// <summary>
        /// Editor
        /// </summary>
        Editor = 1,
        /// <summary>
        /// Puede realizar comentarios en blog y en recursos en nombre se la organización
        /// </summary>
        Comentarista = 2
    }
}
