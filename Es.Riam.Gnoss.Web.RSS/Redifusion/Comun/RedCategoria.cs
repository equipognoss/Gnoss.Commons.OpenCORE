using System;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedCategoria
    {
        private string mNombre=string.Empty;
        private Uri mDominio=null;

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

        public Uri Dominio
        {
            set
            {
                mDominio = value;
            }
            get
            {
                return mDominio;
            }
        }

    }
}
