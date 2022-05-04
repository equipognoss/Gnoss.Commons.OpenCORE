using Es.Riam.Gnoss.Elementos.Interfaces;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Elementos.ListaResultados
{
    /// <summary>
    /// Clase que representa una lista de resultados obtenida a través de una búsqueda.
    /// </summary>
    public class ListaResultados : ElementoGnoss, IElementoTageable, IDisposable
    {
        #region Miembros

        #region Estáticos

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
        /// Lista de tags de la búsqueda.
        /// </summary>
        protected List<string> mListaTags;

        /// <summary>
        /// Resultados.
        /// </summary>
        private List<IElementoGnoss> mResultados;

         /// <summary>
        /// Filtros agregados a la búsqueda realizada.
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
        /// `<param name="pTotalResultados">Número total de elementos que satisfacen la búsqueda</param>
        /// <param name="pPaginaActual">Página actual en la navegación sobre el total de resultados</param>
        public ListaResultados(GestionGnoss pGestor, List<ElementoGnoss> pResultados, int pTotalResultados, int pPaginaActual, LoggingService loggingService)
            : base(null, pGestor, loggingService)
        {
            //Agrego la fila auxiliar que será manejada por el exportadorGnoss:
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
        /// Número total de elementos que satisfacen la búsqueda.
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
        /// Página actual en la navegación sobre el total de resultados.
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
        /// Elementos que se mostrarán por página.
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
        /// Lista de tags agregados a la búsqueda de la lista de resultados.
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
        /// Filtros agregados a la búsqueda realizada.
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
        /// Determina si está disposed
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
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
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

                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
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
