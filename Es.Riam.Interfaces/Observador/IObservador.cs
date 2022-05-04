
namespace Es.Riam.Interfaces.Observador
{
    /// <summary>
	/// Interfaz para que los controles y formularios se sincronicen con un origen de datos
	/// </summary>
    public interface IObservador
    {
        /// <summary>
        /// Método para que el ISujeto informe de los cambios de datos y se sincronice el IObservador
        /// </summary>
        /// <param name="pMensaje">Mensaje codificado para condicionar el comportamiento del observador</param>
        void Avisar(MensajeObservador pMensaje);
        //void sendNotify(AccionesObservador pAccion);

        /// <summary>
        /// Determina si el observador está observando
        /// </summary>
        bool EstaObservando
        {
            get;
            set;
        }

        /// <summary>
        /// Determina si el observador ha sido eliminado
        /// </summary>
        bool EstaDisposed
        {
            get;
        }
    }
}
