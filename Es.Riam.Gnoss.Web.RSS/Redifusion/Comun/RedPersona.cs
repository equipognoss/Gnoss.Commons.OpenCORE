using System;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedPersona
    {
        private string mNombre=string.Empty;
        private Uri mLink=null;
        private string mMail=string.Empty;

        public string Nombre
        {
            set
            {
                mNombre = value;
            }
            get
            {
                return mNombre;
            }
        }

        public Uri Link
        {
            set
            {
                mLink = value;
            }
            get
            {
                return mLink;
            }
        }

        public string Mail
        {
            set
            {
                mMail=value;
            }
            get
            {
                return mMail;
            }
        }
    }
}
