using System;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe el canal RSS del que proviene el item
    public class RSSItemSource
    {
        private Uri mUrl;
        private string mSource;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pUrl">URL del feed RSS original en el que el item fue publicado</param>
        /// <param name="pSource">Nombre del canal RSS del que proviene el ítem</param>
        public RSSItemSource(Uri pUrl,string pSource)
        {
            mUrl = pUrl;
            mSource = pSource;
        }

        public RSSItemSource()
        {
        }


        /// <summary>
        /// Obtiene la URL del feed RSS original en el que el item fue publicado
        /// </summary>
        public Uri Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        /// <summary>
        /// Obtiene el nombre del canal RSS del que proviene el ítem
        /// </summary>
        public string Source
        {
            get { return mSource; }
            set { mSource = value; }
        }
    }
}
