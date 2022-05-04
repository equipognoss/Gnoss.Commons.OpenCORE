using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Petici�n de una invitaci�n a una comunidad
    /// </summary>
    public class PeticionInvComunidad : Peticion
    {

        #region Miembros

        private PeticionInvitacionComunidad mFilaInvComunidad;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaPeticionInvitacionComunidad">Fila de petici�n de invitaci�n a una comunidad</param>
        /// <param name="pFilaPeticion">Fila de petici�n</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionInvComunidad(PeticionInvitacionComunidad pFilaPeticionInvitacionComunidad, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones, LoggingService loggingService)
            : base(pFilaPeticion, pGestorPeticiones, loggingService)
        {
            this.mFilaInvComunidad = pFilaPeticionInvitacionComunidad;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de petici�n de invitaci�n a comunidad
        /// </summary>
        public PeticionInvitacionComunidad FilaInvitacionComunidad
        {
            get
            {
                return mFilaInvComunidad;
            }
        }

        #endregion
    }
}
