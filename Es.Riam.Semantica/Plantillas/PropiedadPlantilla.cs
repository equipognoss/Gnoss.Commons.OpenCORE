using System;
using System.Collections.Generic;
using System.Text;
using Es.Riam.Semantica.OWL;

namespace Es.Riam.Semantica.Plantillas
{
    /// <summary>
    /// Clase que representa un propiedad de una ontología especializada para las platillas.
    /// </summary>

    [Serializable]
    public class PropiedadPlantilla : Propiedad
    {
        #region Miembros

        /// <summary>
        /// Elementos relacionados con la propiedad.
        /// </summary>
        private List<ElementoOntologia> mElementosRelacionados;

        /// <summary>
        /// Propiedades relacionadas con la propiedad.
        /// </summary>
        private List<Propiedad> mPropiedadesRelacionadas;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor para la propiedad.
        /// </summary>
        /// <param name="pNombre">Nombre de la propiedad</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public PropiedadPlantilla(string pNombre, Ontologia pOntologia) : base(pNombre, TipoPropiedad.DatatypeProperty, pOntologia)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Elementos relacionados con la propiedad.
        /// </summary>
        public List<ElementoOntologia> ElementosRelacionados
        {
            get
            {
                return mElementosRelacionados;
            }
            set
            {
                mElementosRelacionados = value;
            }
        }

        /// <summary>
        /// Propiedades relacionadas con la propiedad.
        /// </summary>
        public List<Propiedad> PropiedadesRelacionadas
        {
            get
            {
                return mPropiedadesRelacionadas;
            }
            set
            {
                mPropiedadesRelacionadas = value;
            }
        }

        #endregion
    }
}
