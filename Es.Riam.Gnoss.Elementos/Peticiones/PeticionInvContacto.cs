using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Petición de una invitación a un contacto
    /// </summary>
    public class PeticionInvContacto : Peticion
    {

        #region Miembros

        private PeticionInvitaContacto mFilaInvContacto;
        private PeticionInvitacionComunidad mFilaInvComunidad;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaPeticionInvitacionContacto">Fila de petición de invitación a un contacto</param>
        /// <param name="pFilapeticionInvtiacionComunidad">Fila de petición de invitación a una comunidad</param>
        /// <param name="pFilaPeticion">Fila de petición</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionInvContacto(PeticionInvitaContacto pFilaPeticionInvitacionContacto, PeticionInvitacionComunidad pFilapeticionInvtiacionComunidad, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones, LoggingService loggingService)
            : base(pFilaPeticion, pGestorPeticiones, loggingService)
        {
            this.mFilaInvContacto = pFilaPeticionInvitacionContacto;
            this.mFilaInvComunidad = pFilapeticionInvtiacionComunidad;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de petición de invitación del contacto
        /// </summary>
        public PeticionInvitaContacto FilaInvitacionContacto
        {
            get
            {
                return mFilaInvContacto;
            }
        }

        /// <summary>
        /// Obtiene la fila de petición de invitación a la comunidad
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
