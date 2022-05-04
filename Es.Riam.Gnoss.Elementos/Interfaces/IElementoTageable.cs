using System.Collections.Generic;

namespace Es.Riam.Gnoss.Elementos.Interfaces
{
    /// <summary>
    /// Interfaz para elementos tageables
    /// </summary>
    public interface IElementoTageable
    {
        /// <summary>
        /// Obtiene la lista de tags
        /// </summary>
        List<string> ListaTags
        {
            get;
        }
    }
}
