using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Petici�n de una invitaci�n a una organizaci�n
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
        /// <param name="pFilaPeticionInvitacionOrganizacion">Fila de petici�n de invitaci�n a una organizaci�n</param>
        /// <param name="pFilaPeticion">Fila de petici�n</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionInvOrganizacion(PeticionOrgInvitaPers pFilaPeticionInvitacionOrganizacion, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones)
            : base(pFilaPeticion, pGestorPeticiones)
        {
            this.mFilaInvOrg = pFilaPeticionInvitacionOrganizacion;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de petici�n de invitaci�n a organizaci�n
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
