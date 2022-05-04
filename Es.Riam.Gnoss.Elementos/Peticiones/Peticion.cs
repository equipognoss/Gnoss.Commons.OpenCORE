using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Elementos.Peticiones
{

    /// <summary>
    /// Representa una petición
    /// </summary>
    public class Peticion : ElementoGnoss
    {

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaPeticion">Fila de la petición</param>
        /// <param name="pGestorPeticiones">Gestor de peticiones</param>
        public Peticion(AD.EntityModel.Models.Peticion.Peticion pFilaPeticion, GestionPeticiones pGestorPeticiones, LoggingService loggingService)
            : base(pFilaPeticion, pGestorPeticiones, loggingService)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la fila de esta petición
        /// </summary>
        public AD.EntityModel.Models.Peticion.Peticion FilaPeticion
        {
            get
            {
                return (AD.EntityModel.Models.Peticion.Peticion)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene el identificador de la peticíon
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaPeticion.PeticionID;
            }
        }

        /// <summary>
        /// Obtiene el gestor de peticiones
        /// </summary>
        public GestionPeticiones GestorPeticiones
        {
            get
            {
                return (GestionPeticiones)GestorGnoss;
            }
        }

        #endregion

    }
}
