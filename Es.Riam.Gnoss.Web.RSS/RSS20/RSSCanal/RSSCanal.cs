using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    public class RSSCanal
    {        
        private string mTitle;
        private Uri mLink;
        private string mDescription;
        private string mLanguage;
        private string mCopyright;
        private string mManagingEditor;
        private string mWebMaster;
        private DateTime mPubDate;
        private DateTime mLastBuildDate;
        private List<RSSCategory> mCategory;
        private string mGenerator;
        private Uri mDocs;
        private RSSCanalCloud mCloud;
        private int mTtl;
        private RSSCanalImage mImage;
        private string mRating;
        private RSSCanalTextInput mTextInput;
        private List<int> mSkipHours;
        private List<int> mSkipDays;
        private List<RSSItem> mItems;


        /// <summary>
        /// Constructor Con los parámetros oblgatorios de un Canal
        /// </summary>
        /// <param name="pTitle"></param>
        /// <param name="pLink"></param>
        /// <param name="pDescription"></param>
        public RSSCanal(string pTitle, Uri pLink,string pDescription )
        {
            mTitle = pTitle;
            mLink = pLink;
            mDescription = pDescription;
            mCategory = new List<RSSCategory>();
            mTtl = -1;
            mSkipHours = new List<int>();
            mSkipDays = new List<int>();
            mItems = new List<RSSItem>();
        }

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public RSSCanal()
        {
            mCategory = new List<RSSCategory>();
            mTtl = -1;
            mSkipHours = new List<int>();
            mSkipDays = new List<int>();
            mItems = new List<RSSItem>();
        }

        /// <summary>
        /// Obtiene o establece el título del canal.
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Obtiene o establece la URL del sitio WEB que se corresponde con el canal.
        /// </summary>
        public Uri Link
        {
            get { return mLink; }
            set { mLink = value; }
        }

        /// <summary>
        /// Obtiene o establece la descripción del canal.
        /// </summary>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Obtiene o establece el lenguaje del canal, deben usarse los valores establecidos por el W3C (ej:en-us,es-es)
        /// </summary>
        public string Language
        {
            set { mLanguage = value; }
            get { return mLanguage; }
        }

        /// <summary>
        /// Obtiene o establece el Copyright del canal
        /// </summary>
        public string Copyright
        {
            set { mCopyright = value; }
            get { return mCopyright; }
        }

        /// <summary>
        /// Obtiene o establece la dirección del responsable editorial del contenido del canal (ej:aa@aa.com (Nombre Apellido))
        /// </summary>
        public string ManagingEditor
        {
            set { mManagingEditor = value; }
            get { return mManagingEditor; }
        }

        /// <summary>
        /// Obtiene o establece la dirección del responsable de los problemas técnicos relativos al canal (ej:aa@aa.com (Nombre Apellido))
        /// </summary>
        public string WebMaster
        {
            set { mWebMaster = value; }
            get { return mWebMaster; }
        }

        /// <summary>
        /// Obtiene o establece la fecha de publicacíon del canal
        /// </summary>
        public DateTime PubDate
        {
            set { mPubDate = value; }
            get { return mPubDate; }
        }

        /// <summary>
        /// Obtiene o establece la fecha de los últimos cambios del canal
        /// </summary>
        public DateTime LastBuildDate
        {
            set { mLastBuildDate = value; }
            get { return mLastBuildDate; }
        }


        /// <summary>
        /// Obtiene o establece una lista de las categorías a las que pertenece el canal
        /// </summary>
        public List<RSSCategory> Category
        {
            set { mCategory = value; }
            get { return mCategory; }
        }

        /// <summary>
        /// Obtiene o establece el programa utilizado para generar el canal
        /// </summary>
        public string Generator
        {
            set { mGenerator = value; }
            get { return mGenerator; }
        }

        /// <summary>
        /// Obtiene o establece la URL que informa del formato usado en el RSS
        /// </summary>
        public Uri Docs
        {
            set { mDocs = value; }
            get { return mDocs; }
        }

        /// <summary>
        /// Obtiene o establece TODO
        /// </summary>
        public RSSCanalCloud Cloud
        {
            set { mCloud = value; }
            get { return mCloud; }
        }

        /// <summary>
        /// Obtiene o establece el tiempo en minutos que el canal puede ser cacheado entes de actualizarse de la fuente
        /// </summary>
        public int Ttl
        {
            set { mTtl = value; }
            get { return mTtl; }
        }

        /// <summary>
        /// Obtiene o establece la imagen del canal
        /// </summary>
        public RSSCanalImage Image
        {
            set { mImage = value; }
            get { return mImage; }
        }

        /// <summary>
        /// Obtiene o establece The PICS rating for the channel(TODO)
        /// </summary>
        public string Rating
        {
            set { mRating = value; }
            get { return mRating; }
        }

        /// <summary>
        /// Obtiene o establece un campo TextInput que se muestra con el canal
        /// </summary>
        public RSSCanalTextInput TextInput
        {
            set { mTextInput = value; }
            get { return mTextInput; }
        }

        /// <summary>
        /// Obtiene o establece una lista de enteros entre 0 y 23 que representa una hora en GMT en la que los agregadores pueden dejar de consultar el canal
        /// </summary>
        public List<int> SkipHours
        {
            set { mSkipHours = value; }
            get { return mSkipHours; }
        }

        /// <summary>
        /// Obtiene o establece una lista de enteros entre 1 y 7 que representan los días de la semana en los que los agregadores pueden dejar de consultar el canal
        /// </summary>
        public List<int> SkipDays
        {
            set { mSkipDays = value; }
            get { return mSkipDays; }
        }

        /// <summary>
        /// Obtiene o establece la lista de items del canal
        /// </summary>
        public List<RSSItem> Items
        {
            set { mItems = value; }
            get { return mItems; }
        }
    }
}
