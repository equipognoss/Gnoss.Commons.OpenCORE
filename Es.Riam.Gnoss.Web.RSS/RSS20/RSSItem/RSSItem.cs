using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe un Item rss
    public class RSSItem
    {
        private string mTitle;
        private Uri mLink;
        private string mDescription;
        private string mAuthor;
        private List<RSSCategory> mCategory;
        private Uri mComments;
        private RSSItemEnclosure mEnclosure;
        private RSSItemGuid mGuid;
        private DateTime mPubDate;
        private RSSItemSource mSource;

        /// <summary>
        /// Constructor del �tem
        /// </summary>
        public RSSItem()
        {
            mCategory = new List<RSSCategory>();
        }

        /// <summary>
        /// Obtiene o establece el t�tulo del �tem
        /// </summary>
        public string Title
        {
            set { mTitle = value; }
            get { return mTitle; }
        }

        /// <summary>
        /// Obtiene o establece la URL del �tem
        /// </summary>
        public Uri Link
        {
            set { mLink = value; }
            get { return mLink; }
        }

        /// <summary>
        /// Obtiene o establece la descripci�n del �tem
        /// </summary>
        public string Description
        {
            set { mDescription = value; }
            get { return mDescription; }
        }

        /// <summary>
        /// Obtiene o establece la direcci�n del autor del �tem (ej:aa@aa.com (Nombre Apellido))
        /// </summary>
        public string Author
        {
            set { mAuthor = value; }
            get { return mAuthor; }
        }

        /// <summary>
        /// Obtiene o establece la lista de categor�as a las que pertenece el �tem
        /// </summary>
        public List<RSSCategory> Category
        {
            set { mCategory = value; }
            get { return mCategory; }
        }

        /// <summary>
        /// Obtiene o establece la URL de una p�gina con comentarios del �tem
        /// </summary>
        public Uri Comments
        {
            set { mComments = value; }
            get { return mComments; }
        }

        /// <summary>
        /// Obtiene o establece el objeto media asociado al �tem
        /// </summary>
        public RSSItemEnclosure Enclosure
        {
            set { mEnclosure = value; }
            get { return mEnclosure; }
        }

        /// <summary>
        /// Obtiene o establece el Guid (identificador) del �tem
        /// </summary>
        public RSSItemGuid Guid
        {
            set { mGuid = value; }
            get { return mGuid; }
        }

        /// <summary>
        /// Obtiene o establece cuando se public� el �tem
        /// </summary>
        public DateTime PubDate
        {
            set { mPubDate = value; }
            get { return mPubDate; }
        }

        /// <summary>
        /// Obtiene o establece la fuente original RSS de la que proviene el �tem
        /// </summary>
        public RSSItemSource Source
        {
            set { mSource = value; }
            get { return mSource; }
        }
    }
}
