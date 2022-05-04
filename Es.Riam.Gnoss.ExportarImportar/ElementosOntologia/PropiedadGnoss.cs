using System.Collections.Generic;
using Es.Riam.Semantica.OWL;

namespace Es.Riam.Gnoss.ExportarImportar.ElementosOntologia
{
    /// <summary>
    /// Propiedades de las entidades Gnoss.
    /// </summary>
    public class PropiedadGnoss : Propiedad
    {
        #region Constructores
        
        /// <summary>
        /// Crea una propiedad a partir de sus atributos.
        /// </summary>
        /// <param name="pNombre">nombre</param>
        /// <param name="pTipo">tipo de propiedad</param>
        /// <param name="pDominio">dominio</param>
        /// <param name="pRango">rango</param>
        /// <param name="pFunctionalProperty">verdad si la propiedad es funcional.</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public PropiedadGnoss(string pNombre, TipoPropiedad pTipo, List<string> pDominio, string pRango, bool pFunctionalProperty, Ontologia pOntologia)
            : base(pNombre, pTipo, pDominio, pRango, pFunctionalProperty, pOntologia)
        {
        }

        /// <summary>
        /// Crea una propiedad a partir de sus atributos.
        /// </summary>
        /// <param name="pNombre">nombre</param>
        /// <param name="pTipo">tipo de propiedad</param>
        /// <param name="pOntologia">Ontología a la que pertenece el elemento</param>
        public PropiedadGnoss(string pNombre, TipoPropiedad pTipo, Ontologia pOntologia)
            : base(pNombre, pTipo, pOntologia)
        {
        }

        /// <summary>
        /// Crea una propiedad a partir de otra.
        /// </summary>
        /// <param name="pPropiedad">propiedad que se tomará como referencia.</param>
        public PropiedadGnoss(Propiedad pPropiedad) : base(pPropiedad)
        {
            this.mNombreReal = pPropiedad.NombreReal;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Nombre de la propiedad.
        /// </summary>
        public override string Nombre
        {
            get
            {
                return base.Nombre;
            }
        }

        /// <summary>
        /// Obtiene el nombre de la propiedad + el namespace.
        /// </summary>
        public override string NombreConNamespace
        {
            get
            {
                if (!Nombre.Contains("#") && !Nombre.Contains("/"))
                {
                    return "gnoss:" + Nombre;
                }
                else //Ya tiene namespace o tiene URL completa.
                {
                    return base.NombreConNamespace;
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre real de la propiedad, con acentos y espacios.
        /// </summary>
        public override string NombreReal
        {
            set
            {
                mNombreReal = value;
            }
            get
            {
                if (mElementoOntologia != null)
                {
                    UtilImportarExportar.ObtenerNombreRealPropiedad(mElementoOntologia, null, this);
                }
                return mNombreReal;
            }
        }

        #endregion
    }
}
