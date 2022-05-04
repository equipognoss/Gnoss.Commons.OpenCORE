using System;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe la imagen que representa el canal
    public class RSSCanalImage
    {
        private Uri mUrl;
        private string mTitle;
        private Uri mLink;
        private string mDescription;
        private int mWidth;
        private int mHeight;

        /// <summary>
        /// Constructor de la imagen del canal, contiene los campos obligatorios
        /// </summary>
        /// <param name="pUrl">URL de la imagen GIF,JPEG o PNG</param>
        /// <param name="pTitle">Descripción de la imagen  Es usada por el atributo ALT de <img> cuando el canal se representa en HTML,en la práctica debe ser el mismo string que el atributo Title del canal</param>
        /// <param name="pLink">URL a la página, en la práctica debe ser la misma url que el atributo Link del canal</param>
        public RSSCanalImage(Uri pUrl,string pTitle,Uri pLink)
        {
            mUrl = pUrl;
            mTitle = pTitle;
            mLink = pLink;
            mWidth=-1;
            mHeight = -1;
        }

        public RSSCanalImage()
        {
        }

        /// <summary>
        /// Obtiene om establece la URL de la imagen GIF,JPEG o PNG
        /// </summary>
        public Uri Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        /// <summary>
        /// Obtiene o establece la descripción de la imagen. Es usada por el atributo ALT de <img> cuando el canal se representa en HTML,en la práctica debe ser el mismo string que el atributo Title del canal
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Obtiene o establece un link a la página, en la práctica debe ser la misma url que el atributo Link del canal
        /// </summary>
        public Uri Link
        {
            get { return mLink; }
            set { mLink = value; }
        }

        /// <summary>
        /// Obtiene o establece o establece TODO
        /// </summary>
        public string Description 
        {
            set { mDescription = value; }
            get { return mDescription; }
        }

        /// <summary>
        /// Obtiene o establece el ancho en pixels de la imagen. El máximo es 144, por defecto es 88
        /// </summary>
        public int Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Obtiene o establece el alto en pixels de la imagen. El máximo es 400, por defecto es 31
        /// </summary>
        public int Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

    }
}
