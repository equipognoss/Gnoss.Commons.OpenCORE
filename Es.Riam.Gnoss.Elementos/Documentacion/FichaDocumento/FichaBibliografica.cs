using Es.Riam.Gnoss.Elementos.Documentacion.FichaDocumento;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Documentacion
{
    /// <summary>
    /// Ficha bibliográfica de un documento
    /// </summary>
    public class FichaBibliografica : ElementoGnoss
    {
        #region Miembros

        ///// <summary>
        ///// Fila de la ficha
        ///// </summary>
        //protected DocumentacionDS.FichaBibliograficaRow mFilaFichaBibliografica;

        ///// <summary>
        ///// Documento
        ///// </summary>
        //protected Documento mDocumento;

        ///// <summary>
        ///// Gestor documental de la ficha
        ///// </summary>
        //protected GestorDocumental mGestorDocumental;

        /// <summary>
        /// Contiene una lista con los atributos de la bibliografia.
        /// </summary>
        private SortedList<int, AtributoBibliografico> mListaAtributos;

        /// <summary>
        /// Identificador de ficha bibliografica.
        /// </summary>
        private Guid mFichaBibliograficaID;

        #endregion

        #region Constructores

        ///// <summary>
        ///// Constructor para la ficha bibliografica
        ///// </summary>
        ///// <param name="pFilaFichaBibliografica">Fila de la ficha</param>
        //public FichaBibliografica(DocumentacionDS.FichaBibliograficaRow pFilaFichaBibliografica, Documento pDocumento, GestionGnoss pGestor)
        //    : base(pFilaFichaBibliografica, pGestor)
        //{
        //    mFilaFichaBibliografica = pFilaFichaBibliografica;
        //    mDocumento = pDocumento;

        //    if (pGestor is GestorDocumental)
        //    {
        //        GestorDocumental = (GestorDocumental)pGestor;
        //    }
        //}

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public FichaBibliografica(LoggingService loggingService)
            : base(loggingService)
        {
        }

        /// <summary>
        /// Constructor de ficha bibliográfica
        /// </summary>
        /// <param name="pFichaBibliograficaID">Identificador de ficha bibliográfica</param>
        public FichaBibliografica(Guid pFichaBibliograficaID, LoggingService loggingService)
            : base(loggingService)
        {
            mFichaBibliograficaID = pFichaBibliograficaID;
        }

        #endregion

        #region Propiedades

        ///// <summary>
        ///// Fila de la ficha
        ///// </summary>
        //public DocumentacionDS.FichaBibliograficaRow FilaFichaBibliografica
        //{
        //    get
        //    {
        //        return mFilaFichaBibliografica;
        //    }
        //}

        ///// <summary>
        ///// Fila de la ficha del tipo especifico
        ///// </summary>
        //public virtual DataRow FilaFichaBibPropia
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// Devuelve o establece el gestor documental del documento
        ///// </summary>
        //public GestorDocumental GestorDocumental
        //{
        //    get
        //    {
        //        return mGestorDocumental;
        //    }
        //    set
        //    {
        //        mGestorDocumental = value;
        //    }
        //}

        /// <summary>
        /// Obtiene una lista con los atributos de la bibliografia
        /// </summary>
        public SortedList<int, AtributoBibliografico> Atributos
        {
            get
            {
                if (mListaAtributos == null)
                    mListaAtributos = new SortedList<int, AtributoBibliografico>();
                return mListaAtributos;
            }
        }

        /// <summary>
        /// Obtiene una lista con los atributos de la bibliografia, con clave atributoID
        /// </summary>
        public Dictionary<Guid, AtributoBibliografico> AtributosPorAtributoID
        {
            get
            {
                Dictionary<Guid, AtributoBibliografico> lista = new Dictionary<Guid, AtributoBibliografico>();
                foreach (AtributoBibliografico atributo in Atributos.Values)
                {
                    lista.Add(atributo.AtributoID, atributo);
                }
                return lista;
            }
        }

        /// <summary>
        /// Obtiene el identificador de ficha bibliografica
        /// </summary>
        public Guid FichaBibliograficaID
        {
            get
            {
                return mFichaBibliograficaID;
            }
        }

        #endregion
    }
}
