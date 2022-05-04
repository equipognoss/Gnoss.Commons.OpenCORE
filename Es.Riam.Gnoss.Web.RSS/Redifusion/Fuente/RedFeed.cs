using Es.Riam.Util;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedFuente : IDisposable
    {
        private string mTitulo = string.Empty;
        private Uri mLink = null;
        private string mDescripcion = string.Empty;
        private string mCopyright = string.Empty;
        private List<RedPersona> mAutores = new List<RedPersona>();
        private List<RedPersona> mPublicadores = new List<RedPersona>();
        private List<RedPersona> mContribuyentes = new List<RedPersona>();
        private string mIdentificador = string.Empty;
        private string mGenerador = string.Empty;
        private List<RedCategoria> mCategorias = new List<RedCategoria>();
        private string mDocumentacion = string.Empty;
        private DateTime? mFechaActualizacion = null;
        private DateTime? mFechaPublicaion = null;
        private RedImage mImagen = null;
        private RedTextInput mTextInput = null;
        private List<RedItem> mItems = new List<RedItem>();

        public string Titulo
        {
            set { mTitulo = UtilCadenas.HtmlDecode(value); }
            get { return mTitulo; }
        }
        public Uri Link
        {
            set { mLink = value; }
            get { return mLink; }
        }
        public string Descripcion
        {
            set { mDescripcion = UtilCadenas.HtmlDecode(value); }
            get { return mDescripcion; }
        }
        public string Copyright
        {
            set { mCopyright = UtilCadenas.HtmlDecode(value); ; }
            get { return mCopyright; }
        }
        public List<RedPersona> Autores
        {
            set { mAutores = value; }
            get { return mAutores; }
        }
        public List<RedPersona> Publicadores
        {
            set { mPublicadores = value; }
            get { return mPublicadores; }
        }
        public List<RedPersona> Contribuyentes
        {
            set { mContribuyentes = value; }
            get { return mContribuyentes; }
        }
        public string Identificador
        {
            set { mIdentificador = value; }
            get { return mIdentificador; }
        }

        public string Generador
        {
            set { mGenerador = value; }
            get { return mGenerador; }
        }
        public List<RedCategoria> Categorias
        {
            set { mCategorias = value; }
            get { return mCategorias; }
        }
        public string Documentacion
        {
            set { mDocumentacion = UtilCadenas.HtmlDecode(value); ; }
            get { return mDocumentacion; }
        }
        public DateTime? FechaActualizacion
        {
            set { mFechaActualizacion = value; }
            get { return mFechaActualizacion; }
        }
        public DateTime? FechaPublicaion
        {
            set { mFechaPublicaion = value; }
            get { return mFechaPublicaion; }
        }

        public RedImage Imagen
        {
            set { mImagen = value; }
            get { return mImagen; }
        }
        public RedTextInput TextInput
        {
            set { mTextInput = value; }
            get { return mTextInput; }
        }

        public List<RedItem> Items
        {
            get { return mItems; }
        }

        public void AgnadirItem(RedItem item)
        {
            item.mFuente = this;
            mItems.Add(item);
        }


        #region dispose


        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~RedFuente()
        {
            //Libero los recursos
            Dispose(false);
        }


        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    mTitulo = null;
                    mLink = null;
                    mDescripcion = null;
                    mCopyright = null;
                    mAutores.Clear();
                    mAutores = null;
                    mPublicadores.Clear();
                    mPublicadores = null;
                    mContribuyentes.Clear();
                    mContribuyentes = null;
                    mIdentificador = null;
                    mGenerador = null;
                    mCategorias.Clear();
                    mCategorias = null;
                    mDocumentacion = null;
                    mFechaActualizacion = null;
                    mFechaPublicaion = null;
                    mImagen = null;
                    mTextInput = null;
                    mItems.Clear();
                    mItems = null;
                }
                catch (Exception ex)
                {
                    //mLoggingService.GuardarLogError(ex);
                }
            }
        }
        #endregion




    }
}
