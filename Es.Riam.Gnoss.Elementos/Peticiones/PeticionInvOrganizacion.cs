using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Petición de una invitación a una organización
    /// </summary>
    public class PeticionInvOrganizacion : Peticion
    {

        #region Miembros

        private PeticionOrgInvitaPers mFilaInvOrg;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaPeticionInvitacionOrganizacion">Fila de petición de invitación a una organización</param>
        /// <param name="pFilaPeticion">Fila de petición</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionInvOrganizacion(PeticionOrgInvitaPers pFilaPeticionInvitacionOrganizacion, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones)
            : base(pFilaPeticion, pGestorPeticiones)
        {
            this.mFilaInvOrg = pFilaPeticionInvitacionOrganizacion;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de petición de invitación a organización
        /// </summary>
        public PeticionOrgInvitaPers FilaInvitacionOrganizacion
        {
            get
            {
                return mFilaInvOrg;
            }
        }

        #endregion
    }

}
