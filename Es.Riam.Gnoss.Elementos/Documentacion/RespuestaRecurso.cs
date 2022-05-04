using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Editor de un recurso
    /// </summary>
    public class RespuestaRecurso : ElementoGnoss
    {

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pFilaRespuesta">Fila de respuesta</param>
        /// <param name="pGestorDocumental">Gestor documental</param>
        public RespuestaRecurso(AD.EntityModel.Models.Documentacion.DocumentoRespuesta pFilaRespuesta, GestorDocumental pGestorDocumental, LoggingService loggingService)
            : base(pFilaRespuesta, pGestorDocumental, loggingService)
        {

        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el Identificador de la identidad de la respuesta
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaRespuesta.RespuestaID;
            }
        }

        /// <summary>
        /// Obtiene el documento editado
        /// </summary>
        public Documento DocumentoEditado
        {
            get
            {
                if (GestionDocumental.ListaDocumentos.ContainsKey(FilaRespuesta.DocumentoID))
                {
                    return GestionDocumental.ListaDocumentos[FilaRespuesta.DocumentoID];
                }
                return null;
            }
        }
      

        /// <summary>
        /// Obtiene la fila de la respuesta
        /// </summary>
        public AD.EntityModel.Models.Documentacion.DocumentoRespuesta FilaRespuesta
        {
            get
            {
                return (AD.EntityModel.Models.Documentacion.DocumentoRespuesta)FilaElementoEntity;
            }
        }

        /// <summary>
        /// Obtiene el gestor de documentación
        /// </summary>
        public GestorDocumental GestionDocumental
        {
            get
            {
                return (GestorDocumental)GestorGnoss;
            }
        }

        #endregion

    }
}
