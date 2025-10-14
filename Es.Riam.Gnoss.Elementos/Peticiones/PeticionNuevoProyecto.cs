
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Petición de creación de una comunidad
    /// </summary>
    public class PeticionNuevoProyecto:Peticion
    {

        #region Miembros

        private AD.EntityModel.Models.Peticion.PeticionNuevoProyecto mFilaNuevoProyecto;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaPeticionNuevoProyecto">Fila de petición de nuevo proyecto</param>
        /// <param name="pFilaPeticion">Fila de petición</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public PeticionNuevoProyecto(AD.EntityModel.Models.Peticion.PeticionNuevoProyecto pFilaPeticionNuevoProyecto, AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones)
            : base(pFilaPeticion, pGestorPeticiones)
        {
            this.mFilaNuevoProyecto = pFilaPeticionNuevoProyecto;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de petición de nuevo proyecto
        /// </summary>
        public AD.EntityModel.Models.Peticion.PeticionNuevoProyecto FilaNuevoProyecto
        {
            get
            {
                return mFilaNuevoProyecto;
            }
        }

        #endregion

    }
}
