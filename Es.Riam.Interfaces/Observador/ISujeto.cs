namespace Es.Riam.Interfaces.Observador
{
    /// <summary>
	/// Interfaz para que la cumplan los proveedores de datos
	/// </summary>
	public interface ISujeto
    {
        /// <summary>
        /// Método para que los IObservadores se registren y el ISujeto los tenga presentes
        /// </summary>
        /// <param name="pObs">Observador que se registra</param>
        void AgregarObservador(IObservador pObs);

        /// <summary>
        /// Método para que eliminar un observador
        /// </summary>
        /// <param name="pObs">Observador que se registra</param>
        void EliminarObservador(IObservador pObs);

        /// <summary>
        /// Notifica al observador
        /// </summary>
        /// <param name="pMensaje">Mensaje del observador</param>
        void Notificar(MensajeObservador pMensaje);

    }
}
