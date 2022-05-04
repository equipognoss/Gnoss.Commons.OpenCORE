using Es.Riam.Gnoss.Elementos.Interfaces;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Elementos.ListaResultados
{
    /// <summary>
    /// Clase que representa una lista de resultados obtenida a trav�s de una b�squeda.
    /// </summary>
    public class ListaResultados : ElementoGnoss, IElementoTageable, IDisposable
    {
        #region Miembros

        #region Est�ticos

        /// <summary>
        /// Campo auxiliar para almacenar el ID de la lista de resultados.
        /// </summary>
        public const string CAMPO_LISTARESULTADOSID = "ListaResultadosID";

        /// <summary>
        /// Campo auxiliar para almacenar el ID de la lista de resultados.
        /// </summary>
        public const string CAMPO_NUMERORESULTADOS = "NumberOfResults";

        /// <summary>
        /// Campo auxiliar para almacenar el ID de la lista de resultados.
        /// </summary>
        public const string CAMPO_NUMEROPAGINA = "Page";

        /// <summary>
        /// Campo auxiliar para almacenar el ID de la lista de resultados.
        /// </summary>
        public const string CAMPO_NUMEROELEMTPORPAG = "ItemsPerPage";

        #endregion

        /// <summary>
        /// Lista de tags de la b�squeda.
        /// </summary>
        protected List<string> mListaTags;

        /// <summary>
        /// Resultados.
        /// </summary>
        private List<IElementoGnoss> mResultados;

         /// <summary>
        /// Filtros agregados a la b�squeda realizada.
        /// </summary>
        private List<Filtro> mFiltros;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para la lista.
        /// </summary>
        /// <param name="pListaResultadosRow">Fila ficticia de la lista</param>
        /// <param name="pGestor">Gestor</param>
        public ListaResultados(DataRow pListaResultadosRow, GestionGnoss pGestor, LoggingService loggingService)
            : base(pListaResultadosRow, pGestor, loggingService)
        {
        }

        /// <summary>
        /// Constructor para la lista.
        /// </summary>
        /// <param name="pGestor">Gestor</param>
        /// <param name="pResultados">Lista con los resultados</param>
        /// `<param name="pTotalResultados">N�mero total de elementos que satisfacen la b�squeda</param>
        /// <param name="pPaginaActual">P�gina actual en la navegaci�n sobre el total de resultados</param>
        public ListaResultados(GestionGnoss pGestor, List<ElementoGnoss> pResultados, int pTotalResultados, int pPaginaActual, LoggingService loggingService)
            : base(null, pGestor, loggingService)
        {
            //Agrego la fila auxiliar que ser� manejada por el exportadorGnoss:
            FilaElemento = pGestor.DataSet.Tables[GestorListaResultados.TABLA_GENERICA].NewRow();

            FilaElemento[CAMPO_LISTARESULTADOSID] = Guid.NewGuid();

            NumeroResultados = pTotalResultados;
            NumeroPaginaActual = pPaginaActual;
            NumeroResultadosPorPagina = pResultados.Count;

            Resultados = new List<IElementoGnoss>();
            foreach (ElementoGnoss elemento in pResultados)
            {
                Resultados.Add(elemento);
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve los resultados.
        /// </summary>
        public List<IElementoGnoss> Resultados
        {
            get
            {
                return mResultados;
            }
            set
            {
                mResultados = value;
            }
        }

        /// <summary>
        /// Identificador de la lista de resultados.
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return new Guid((string)FilaElemento[CAMPO_LISTARESULTADOSID]);
            }
        }

        /// <summary>
        /// N�mero total de elementos que satisfacen la b�squeda.
        /// </summary>
        public int NumeroResultados
        {
            get
            {
                return int.Parse((string)FilaElemento[CAMPO_NUMERORESULTADOS]);
            }
            set
            {
                FilaElemento[CAMPO_NUMERORESULTADOS] = value;
            }
        }

        /// <summary>
        /// P�gina actual en la navegaci�n sobre el total de resultados.
        /// </summary>
        public int NumeroPaginaActual
        {
            get
            {
                return int.Parse((string)FilaElemento[CAMPO_NUMEROPAGINA]);
            }
            set
            {
                FilaElemento[CAMPO_NUMEROPAGINA] = value;
            }
        }

        /// <summary>
        /// Elementos que se mostrar�n por p�gina.
        /// </summary>
        public int NumeroResultadosPorPagina
        {
            get
            {
                return int.Parse((string)FilaElemento[CAMPO_NUMEROELEMTPORPAG]);
            }
            set
            {
                FilaElemento[CAMPO_NUMEROELEMTPORPAG] = value;
            }
        }

        #endregion

        #region Miembros de IElementoTageable

        /// <summary>
        /// Lista de tags agregados a la b�squeda de la lista de resultados.
        /// </summary>
        public List<string> ListaTags
        {
            get
            {
                if (mListaTags == null)
                {
                    mListaTags = new List<string>();
                }

                return mListaTags;
            }
            set
            {
                mListaTags = value;
            }
        }

        /// <summary>
        /// Filtros agregados a la b�squeda realizada.
        /// </summary>
        public List<Filtro> Filtros
        {
            get
            {
                if (mFiltros == null)
                {
                    mFiltros = new List<Filtro>();
                }
                return mFiltros;
            }
            set
            {
                mFiltros = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ListaResultados()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                try
                {
                    if (disposing)
                    {

                    }

                    //Libero todos los recursos nativos sin administrar que he a�adido a esta clase
                }
                finally
                {
                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
