using System.Collections.Generic;

namespace Es.Riam.Interfaces
{
    /// <summary>
    /// Interfaz para elemento filtrado
    /// </summary>
    public interface IElementoFiltrado : IElementoGnoss
    {
        /// <summary>
        /// Obtiene o establece el elemento filtrado
        /// </summary>
        IElementoGnoss ElementoFiltrado
        {
            get;
            set;
        }

        /// <summary>
        /// Obtiene o establece los elementos hijos filtrados
        /// </summary>
        Dictionary<IElementoGnoss, IElementoFiltrado> HijosFiltrados
        {
            get;
            set;
        }

        /// <summary>
        /// Obtiene o establece el elemento padre filtrado
        /// </summary>
        IElementoFiltrado PadreFiltrado
        {
            get;
            set;
        }
    }
}
