namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    //Describe el identificador de un Item
    public class RSSItemGuid
    {
        private bool mIsPermaLink;
        private string mGuid;

        /// <summary>
        /// Constructor a partir del Identificador del ítem equivalente a RSSItemGuid(pGuid, false)
        /// </summary>
        /// <param name="pGuid">Identificador único del ítem</param>
        public RSSItemGuid(string pGuid)
        {
            mGuid = pGuid;
        }

        /// <summary>
        /// Constructor a partir del Identificador del ítem que especifica si el Guid es una URL 
        /// </summary>
        /// <param name="pGuid">Identificador único del ítem</param>
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
        /// Obtiene si el Guid utilizado para identificar el Ítem es una URL permanente
        /// </summary>
        public bool IsPermaLink
        {
            get { return mIsPermaLink; }
            set { mIsPermaLink = value; }
        }

        /// <summary>
        /// Obtiene el identificador del ítem que será utilizado por los agregadores para diferenciar los ítems.
        /// </summary>
        public string Guid
        {
            get { return mGuid; }
            set { mGuid = value; }
        }
    }
}
