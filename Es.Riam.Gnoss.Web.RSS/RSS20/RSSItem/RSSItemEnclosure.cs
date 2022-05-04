using System;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe un objeto media asociado al Item
    public class RSSItemEnclosure
    {
        private Uri mUrl;
        private int mLength;
        private string mType;

        /// <summary>
        /// Constructor de un objeto media, contiene los campos obligatorios
        /// </summary>
        /// <param name="pUrl">Dirección URL del objeto</param>
        /// <param name="pLength">Tamaño en bytes del objeto</param>
        /// <param name="pType">Tipo del objeto en el estándar MIME</param>
        public RSSItemEnclosure(Uri pUrl,int pLength,string pType)
        {
            mUrl = pUrl;
            mLength = pLength;
            mType = pType;
        }
        public RSSItemEnclosure()
        {
        }


        /// <summary>
        /// Obtiene la localización URL del objeto
        /// </summary>
        public Uri Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        /// <summary>
        /// Obtiene el tamaño en bytes del objeto
        /// </summary>
        public int Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        /// <summary>
        /// Obtiene el tipo del objeto en el estándar MIME(ej:audio/mpeg)
        /// </summary>
        public string Type
        {
            get { return mType; }
            set { mType = value; }
        }

    }
}
