using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Petición de una invitación a una comunidad
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
        /// <param name="pFilaPeticionInvitacionComunidad">Fila de petición de invitación a una comunidad</param>
        /// <param name="pFilaPeticion">Fila de petición</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionInvComunidad(PeticionInvitacionComunidad pFilaPeticionInvitacionComunidad, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones)
            : base(pFilaPeticion, pGestorPeticiones)
        {
            this.mFilaInvComunidad = pFilaPeticionInvitacionComunidad;
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

        #endregion
    }
}
