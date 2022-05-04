namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{

    public class RedItemId
    {
        private bool mIsPermaLink = false;
        private string mId = string.Empty;


        public bool IsPermaLink
        {
            get { return mIsPermaLink; }
            set { mIsPermaLink = value; }
        }


        public string Id
        {
            get { return mId; }
            set { mId = value; }
        }
    }
}
