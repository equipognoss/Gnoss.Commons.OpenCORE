namespace Es.Riam.Interfaces.Observador
{
    /// <summary>
    /// Enumeración para representar las acciones que realiza un observador al ser notificado
    /// </summary>
    public enum AccionesObservador
    {
        /// <summary>
        /// Invalida cuando se ha modificado
        /// </summary>
        Invalidar,
        /// <summary>
        /// Invalida pero con modificación de tamaño
        /// </summary>
        Ajustar,
        /// <summary>
        /// Adicción de un elemento
        /// </summary>
        Aniadir,
        /// <summary>
        /// Eliminación de un elemento
        /// </summary>
        Eliminar,
        /// <summary>
        /// Recargar los hijos
        /// </summary>
        Recargar,
        /// <summary>
        /// Pegado de un elemento
        /// </summary>
        Pegar
    }
}
