namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe el identificador de un Item
    public class RSSItemGuid
    {
        private bool mIsPermaLink;
        private string mGuid;

        /// <summary>
        /// Constructor a partir del Identificador del �tem equivalente a RSSItemGuid(pGuid, false)
        /// </summary>
        /// <param name="pGuid">Identificador �nico del �tem</param>
        public RSSItemGuid(string pGuid)
        {
            mGuid = pGuid;
        }

        /// <summary>
        /// Constructor a partir del Identificador del �tem que especifica si el Guid es una URL 
        /// </summary>
        /// <param name="pGuid">Identificador �nico del �tem</param>
        /// <param name="pIsPermaLink">Indica si el Guid es una URL permanente</param>
        public RSSItemGuid(string pGuid, bool pIsPermaLink)
        {
            mIsPermaLink = pIsPermaLink;
            mGuid = pGuid;
        }

        public RSSItemGuid()
        {
        }

        /// <summary>
        /// Obtiene si el Guid utilizado para identificar el �tem es una URL permanente
        /// </summary>
        public bool IsPermaLink
        {
            get { return mIsPermaLink; }
            set { mIsPermaLink = value; }
        }

        /// <summary>
        /// Obtiene el identificador del �tem que ser� utilizado por los agregadores para diferenciar los �tems.
        /// </summary>
        public string Guid
        {
            get { return mGuid; }
            set { mGuid = value; }
        }
    }
}
