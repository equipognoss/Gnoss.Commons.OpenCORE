using System;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedTextInput
    {
        private string mTitulo=string.Empty;
        private string mDescripcion=string.Empty;
        private string mNombre=string.Empty;
        private Uri mLink=null;

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
    }
}
