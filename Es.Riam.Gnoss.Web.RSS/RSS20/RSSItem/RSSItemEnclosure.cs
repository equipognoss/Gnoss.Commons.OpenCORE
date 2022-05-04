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
        /// <param name="pUrl">Direcci�n URL del objeto</param>
        /// <param name="pLength">Tama�o en bytes del objeto</param>
        /// <param name="pType">Tipo del objeto en el est�ndar MIME</param>
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
        /// Obtiene la localizaci�n URL del objeto
        /// </summary>
        public Uri Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        /// <summary>
        /// Obtiene el tama�o en bytes del objeto
        /// </summary>
        public int Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        /// <summary>
        /// Obtiene el tipo del objeto en el est�ndar MIME(ej:audio/mpeg)
        /// </summary>
        public string Type
        {
            get { return mType; }
            set { mType = value; }
        }

    }
}
