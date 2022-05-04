using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{
    public class PeticionInvGrupoYComunidad : Peticion
    {
        #region Miembros

        private PeticionInvitacionComunidad mFilaInvComunidad;
        private PeticionInvitacionGrupo mFilaInvGrupo;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaPeticionInvitacionGrupo">Fila de peticion de invitacion a grupos</param>
        /// <param name="pFilaPeticionInvitacionComunidad">Fila de petición de invitación a una comunidad</param>
        /// <param name="pFilaPeticion">Fila de petición</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionInvGrupoYComunidad(PeticionInvitacionGrupo pFilaPeticionInvitacionGrupo, PeticionInvitacionComunidad pFilaPeticionInvitacionComunidad, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones, LoggingService loggingService)
            : base(pFilaPeticion, pGestorPeticiones, loggingService)
        {
            this.mFilaInvComunidad = pFilaPeticionInvitacionComunidad;
            this.mFilaInvGrupo = pFilaPeticionInvitacionGrupo;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de petición de invitación a comunidad
        /// </summary>
        public PeticionInvitacionComunidad FilaInvitacionComunidad
        {
            get
            {
                return mFilaInvComunidad;
            }
        }

        /// <summary>
        /// Obtiene la fila de petición de invitación a grupos
        /// </summary>
        public PeticionInvitacionGrupo FilaInvitacionGrupo
        {
            get
            {
                return mFilaInvGrupo;
            }
        }

        #endregion
    }
}
