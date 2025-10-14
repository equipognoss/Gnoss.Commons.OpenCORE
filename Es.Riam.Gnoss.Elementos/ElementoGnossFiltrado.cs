using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Elementos
{
    /// <summary>
    /// Elemento Gnoss Filtrado
    /// </summary>
    public class ElementoGnossFiltrado : ElementoGnoss, IElementoFiltrado, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Elemento representado
        /// </summary>
        private IElementoGnoss mElemento;

        /// <summary>
        /// Hijos filtrados
        /// </summary>
        private Dictionary<IElementoGnoss, IElementoFiltrado> mHijosFiltrados;

        /// <summary>
        /// Padre filtrado
        /// </summary>
        private IElementoFiltrado mPadreFiltrado;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pElemento">Elemento al que representa</param>
        public ElementoGnossFiltrado(IElementoGnoss pElemento)
            : base(pElemento)
        {
            mElemento = pElemento;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Crea la lista de elementos filtrados a partir de una lista de elementos
        /// </summary>
        /// <param name="pListaElementos">lista de elementos a incluir en la lista de elementos filtrados</param>
        /// <param name="pListaElementosFiltrados">lista de elementos filtrados a la que se añaden los elementos</param>
        /// <returns></returns>
        public IElementoFiltrado CreaListaFiltrados(System.Collections.IList pListaElementos, Dictionary<IElementoGnoss, ElementoGnossFiltrado> pListaElementosFiltrados)
        {
            IElementoFiltrado elementoRaiz = null;
            if (pListaElementosFiltrados == null)
            {
                pListaElementosFiltrados = new Dictionary<IElementoGnoss, ElementoGnossFiltrado>();
            }
            foreach (IElementoGnoss elemento in pListaElementos)//Montamos la lista de IElementoFiltrados
            {
                IElementoGnoss aux = elemento;

                while (aux != null) //Hasta que lleguemos al padre
                {
                    ElementoGnossFiltrado elem = null;
                    if (!pListaElementosFiltrados.ContainsKey(aux))
                    {
                        elem = new ElementoGnossFiltrado(aux);
                        pListaElementosFiltrados.Add(aux, elem);
                    }
                    else
                    {
                        elem = pListaElementosFiltrados[aux];
                    }
                    if (aux.Padre != null && pListaElementosFiltrados.ContainsKey(aux.Padre))
                    {
                        if (!pListaElementosFiltrados[aux.Padre].HijosFiltrados.ContainsKey(aux))
                        {
                            pListaElementosFiltrados[aux.Padre].HijosFiltrados.Add(aux, elem);
                            elem.PadreFiltrado = pListaElementosFiltrados[aux.Padre];
                        }
                    }
                    else if (aux.Padre != null)
                    {
                        ElementoGnossFiltrado elemPadre = new ElementoGnossFiltrado(aux.Padre);
                        pListaElementosFiltrados.Add(aux.Padre, elemPadre);
                        elem.PadreFiltrado = elemPadre;
                        elemPadre.HijosFiltrados.Add(aux, elem);
                    }

                    aux = aux.Padre;
                    if (aux != null && aux.Padre == null) //entonces es la raiz
                    {
                        elementoRaiz = pListaElementosFiltrados[aux];
                    }
                }
            }
            return elementoRaiz;
        }

        /// <summary>
        /// Determina si el elemento admite un hijo
        /// </summary>
        /// <param name="pHijoCandidato">Hijo candidato</param>
        /// <returns></returns>
        public override bool AdmiteHijo(IElementoGnoss pHijoCandidato)
        {
            if (pHijoCandidato is ElementoGnossFiltrado)
            {
                return ((ElementoGnoss)ElementoFiltrado).AdmiteHijo(((ElementoGnossFiltrado)pHijoCandidato).ElementoFiltrado);
            }
            return ((ElementoGnoss)ElementoFiltrado).AdmiteHijo(pHijoCandidato);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece la fila del elemento
        /// </summary>
        public override DataRow FilaElemento
        {
            get
            {
                return ((ElementoGnoss)ElementoFiltrado).FilaElemento;
            }
            set
            {
                ((ElementoGnoss)ElementoFiltrado).FilaElemento = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el nombre del elemento
        /// </summary>
        public override string Nombre
        {
            get
            {
                return ElementoFiltrado.Nombre;
            }
            set
            {
                ElementoFiltrado.Nombre = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el color del elemento
        /// </summary>
        public override System.Drawing.Color Color
        {
            get
            {
                return ElementoFiltrado.Color;
            }
            set
            {
                ElementoFiltrado.Color = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor GNOSS
        /// </summary>
        public override GestionGnoss GestorGnoss
        {
            get
            {
                if (ElementoFiltrado is ElementoGnoss)
                    return ((ElementoGnoss)ElementoFiltrado).GestorGnoss;
                return null;
            }
        }

        /// <summary>
        /// Obtiene el código del elemento filtrado
        /// </summary>
        public override string Codigo
        {
            get
            {
                return ElementoFiltrado.Codigo;
            }
        }

        /// <summary>
        /// Obtiene o establece si el elemento filtrado está contraído
        /// </summary>
        public override bool EstaContraido
        {
            get
            {
                return ElementoFiltrado.EstaContraido;
            }
            set
            {
                ElementoFiltrado.EstaContraido = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el índice del elemento filtrado
        /// </summary>
        public override short Indice
        {
            get
            {
                return ElementoFiltrado.Indice;
            }
            set
            {
                ElementoFiltrado.Indice = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el elemento filtrado está seleccionado
        /// </summary>
        public override bool EstaSeleccionado
        {
            get
            {
                return ElementoFiltrado.EstaSeleccionado;
            }
            set
            {
                ElementoFiltrado.EstaSeleccionado = value;
            }
        }

        /// <summary>
        /// Devuelve si el elemento se puede cortar o arrastrar
        /// </summary>
        public override bool SePuedeCortarArrastrar
        {
            get
            {
                return ((ElementoGnoss)ElementoFiltrado).SePuedeCortarArrastrar;
            }
        }

        #endregion

        #region Miembros de IElementoFiltrado

        /// <summary>
        /// Obtiene o establece el elemento filtrado
        /// </summary>
        public IElementoGnoss ElementoFiltrado
        {
            get
            {
                return mElemento;
            }
            set
            {
                mElemento = value;
            }
        }

        /// <summary>
        /// Obtiene la lista de hijos
        /// </summary>
        public override List<IElementoGnoss> Hijos
        {
            get
            {
                return ElementoFiltrado.Hijos;
            }
        }

        /// <summary>
        /// Obtiene o establece la lista de hijos filtrados
        /// </summary>
        public Dictionary<IElementoGnoss, IElementoFiltrado> HijosFiltrados
        {
            get
            {
                if (mHijosFiltrados == null)
                    mHijosFiltrados = new Dictionary<IElementoGnoss, IElementoFiltrado>();
                return mHijosFiltrados;
            }
            set
            {
                mHijosFiltrados = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el padre filtrado
        /// </summary>
        public IElementoFiltrado PadreFiltrado
        {
            get
            {
                return mPadreFiltrado;
            }
            set
            {
                mPadreFiltrado = value;
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
        ~ElementoGnossFiltrado()
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
                        if (this.mHijosFiltrados != null)
                            this.mHijosFiltrados.Clear();
                    }
                    //Libero todos los recursos nativos sin administrar que he añadido a esta clase
                    this.mElemento = null;
                    this.mHijosFiltrados = null;
                    this.mPadreFiltrado = null;
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
