using System;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedAdjunto
    {
        private Uri mHref=null;        
        private string mType=string.Empty;
        private string mTitulo=string.Empty;
        private int mLength=-1;
        
        public Uri Href 
        {
            set { mHref = value; }
            get { return mHref; }
        }

        public string Type
        {
            set { mType = value; }
            get { return mType; }
        }

        public string Titulo
        {
            set { mTitulo = value; }
            get { return mTitulo; }
        }

        public int Length
        {
            set { mLength = value; }
            get { return mLength; }
        }
    }
}
