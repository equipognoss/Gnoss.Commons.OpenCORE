using Es.Riam.Gnoss.Elementos;
using Es.Riam.Util;
using System.Collections.Generic;
using System.Drawing;

namespace Es.Riam.Gnoss.Recursos
{
    /// <summary>
    /// Útiles de Gnoss
    /// </summary>
    public class UtilGnoss
    {
        #region Miembros

        /// <summary>
        /// Lista de imágenes con su clave
        /// </summary>
        static SortedList<string, Bitmap> mListaImagenes = new SortedList<string,Bitmap>();

        #endregion

        #region Métodos

        /// <summary>
        /// Devuelve el texto del ToolTipText
        /// </summary>
        /// <param name="pElemento">Elemento del que se muestra el tooltip</param>
        /// <returns></returns>
        public static string TextoToolTipText(ElementoGnoss pElemento)
        {
            return pElemento.Nombre;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Lista de imágenes con su clave
        /// </summary>
        public static SortedList<string, Bitmap> ListaImagenesGnoss
        {
            get
            {
                return mListaImagenes;
            }
        }

        /// <summary>
        /// Obtiene el contenido de la ontología de GNOSS
        /// </summary>
        public static List<byte[]> OntologiaGnoss
        {
            get
            {
                List<byte[]> listaArchivosOnto = new List<byte[]>();
                listaArchivosOnto.Add(Properties.Resources.ontologia);
                listaArchivosOnto.Add(Properties.Resources.ns);
                listaArchivosOnto.Add(Properties.Resources.types);
                listaArchivosOnto.Add(Properties.Resources.DC);
                listaArchivosOnto.Add(Properties.Resources.skos);
                listaArchivosOnto.Add(Properties.Resources.foaf_20100101);
                listaArchivosOnto.Add(Properties.Resources.rdf);
                //return Properties.Resources.ontologia;
                return listaArchivosOnto;
            }
        }

        #endregion
    }
}
