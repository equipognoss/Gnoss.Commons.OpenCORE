using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Semantica.OWL;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.ExportarImportar.ElementosOntologia
{
    /// <summary>
    /// Representa una ontología GNOSS leída desde un archivo en formato OWL.
    /// </summary>
    public class OntologiaGnoss : Ontologia
    {
        private EntityContext mEntityContext;

        #region Constructor

        /// <summary>
        /// Crea una ontología a partir de una lista de entidades y los bytes del fichero de la ontología.
        /// </summary>
        /// <param name="pContenidoOntologia">Bytes del fichero de ontología.</param>
        public OntologiaGnoss(byte[] pContenidoOntologia, EntityContext entityContext)
            : base(pContenidoOntologia)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Crea una ontología a partir de una lista de bytes de los ficheros de las ontologías.
        /// </summary>
        /// <param name="pContenidoOntologias">Lista con los Bytes de los ficheros de ontologías.</param>
        public OntologiaGnoss(List<byte[]> pContenidoOntologias, EntityContext entityContext)
            : base(pContenidoOntologias)
        {
            mEntityContext = entityContext;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve la entidad del tipo pTipoEntidad.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de la entidad buscada.</param>
        /// <returns>La entidad del tipo buscado.</returns>
        public override Es.Riam.Semantica.OWL.ElementoOntologia GetEntidadTipo(string pTipoEntidad)
        {
            return base.GetEntidadTipo(pTipoEntidad.Replace("gnoss:", ""));
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Gestor OWL
        /// </summary>
        public override GestionOWL GestorOWL
        {
            get
            {
                if (this.mGestionOwl == null)
                {
                    mGestionOwl = new GestionOWLGnoss(mEntityContext);
                }
                return mGestionOwl;
            }
        }

        #endregion
    }
}
