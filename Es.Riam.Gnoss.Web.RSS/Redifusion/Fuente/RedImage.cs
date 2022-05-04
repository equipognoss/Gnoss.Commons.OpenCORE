using System;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedImage
    {
        private string mTitulo=string.Empty;
        private Uri mLink=null;
        private Uri mUrl=null;
        private int mAncho=-1;
        private int mAlto=-1;
        private string mDescripcion=string.Empty;

        public string Titulo
        {
            set
            {
                mTitulo = value;
            }
            get
            {
                return mTitulo;
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

        public Uri Url
        {
            set
            {
                mUrl = value;
            }
            get
            {
                return mUrl;
            }
        }

        public int Ancho
        {
            set
            {
                mAncho = value;
            }
            get
            {
                return mAncho;
            }
        }

        public int Alto
        {
            set
            {
                mAlto = value;
            }
            get
            {
                return mAlto;
            }
        }

        public string Descripcion
        {
            set
            {
                mDescripcion = value;
            }
            get
            {
                return mDescripcion;
            }
        }
    }
}
