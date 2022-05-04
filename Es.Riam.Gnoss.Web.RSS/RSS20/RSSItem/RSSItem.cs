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
        /// Constructor del Ítem
        /// </summary>
        public RSSItem()
        {
            mCategory = new List<RSSCategory>();
        }

        /// <summary>
        /// Obtiene o establece el título del Ítem
        /// </summary>
        public string Title
        {
            set { mTitle = value; }
            get { return mTitle; }
        }

        /// <summary>
        /// Obtiene o establece la URL del Ítem
        /// </summary>
        public Uri Link
        {
            set { mLink = value; }
            get { return mLink; }
        }

        /// <summary>
        /// Obtiene o establece la descripción del Ítem
        /// </summary>
        public string Description
        {
            set { mDescription = value; }
            get { return mDescription; }
        }

        /// <summary>
        /// Obtiene o establece la dirección del autor del ítem (ej:aa@aa.com (Nombre Apellido))
        /// </summary>
        public string Author
        {
            set { mAuthor = value; }
            get { return mAuthor; }
        }

        /// <summary>
        /// Obtiene o establece la lista de categorías a las que pertenece el ítem
        /// </summary>
        public List<RSSCategory> Category
        {
            set { mCategory = value; }
            get { return mCategory; }
        }

        /// <summary>
        /// Obtiene o establece la URL de una página con comentarios del ítem
        /// </summary>
        public Uri Comments
        {
            set { mComments = value; }
            get { return mComments; }
        }

        /// <summary>
        /// Obtiene o establece el objeto media asociado al ítem
        /// </summary>
        public RSSItemEnclosure Enclosure
        {
            set { mEnclosure = value; }
            get { return mEnclosure; }
        }

        /// <summary>
        /// Obtiene o establece el Guid (identificador) del ítem
        /// </summary>
        public RSSItemGuid Guid
        {
            set { mGuid = value; }
            get { return mGuid; }
        }

        /// <summary>
        /// Obtiene o establece cuando se publicó el ítem
        /// </summary>
        public DateTime PubDate
        {
            set { mPubDate = value; }
            get { return mPubDate; }
        }

        /// <summary>
        /// Obtiene o establece la fuente original RSS de la que proviene el Ítem
        /// </summary>
        public RSSItemSource Source
        {
            set { mSource = value; }
            get { return mSource; }
        }
    }
}
