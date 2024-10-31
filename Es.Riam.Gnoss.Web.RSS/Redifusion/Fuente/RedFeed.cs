using Es.Riam.Util;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedFuente : IDisposable
    {
        private string mTitulo = string.Empty;
        private string mDescripcion = string.Empty;
        private string mCopyright = string.Empty;
        private List<RedPersona> mAutores = new List<RedPersona>();
        private List<RedPersona> mPublicadores = new List<RedPersona>();
        private List<RedPersona> mContribuyentes = new List<RedPersona>();
        private List<RedCategoria> mCategorias = new List<RedCategoria>();
        private string mDocumentacion = string.Empty;
        private List<RedItem> mItems = new List<RedItem>();

        public string Titulo
        {
            set { mTitulo = UtilCadenas.HtmlDecode(value); }
            get { return mTitulo; }
        }

        public Uri Link { get; set; }

        public string Descripcion
        {
            set { mDescripcion = UtilCadenas.HtmlDecode(value); }
            get { return mDescripcion; }
        }

        public string Copyright
        {
            set { mCopyright = UtilCadenas.HtmlDecode(value); }
            get { return mCopyright; }
        }

        public List<RedPersona> Autores { get; set; }

        public List<RedPersona> Publicadores { get; set; }
        
        public List<RedPersona> Contribuyentes { get; set; }

        public string Identificador { get; set; }

        public string Generador { get; set; }

        public List<RedCategoria> Categorias { get; set; }

        public string Documentacion
        {
            set { mDocumentacion = UtilCadenas.HtmlDecode(value); }
            get { return mDocumentacion; }
        }

        public DateTime? FechaActualizacion { get; set; }

        public DateTime? FechaPublicaion { get; set; }

        public RedImage Imagen { get; set; }

        public RedTextInput TextInput{ get; set; }

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

            //Impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                try
                {
                    mTitulo = null;
                    Link = null;
                    mDescripcion = null;
                    mCopyright = null;
                    mAutores.Clear();
                    mAutores = null;
                    mPublicadores.Clear();
                    mPublicadores = null;
                    mContribuyentes.Clear();
                    mContribuyentes = null;
                    Identificador = null;
                    Generador = null;
                    mCategorias.Clear();
                    mCategorias = null;
                    mDocumentacion = null;
                    FechaActualizacion = null;
                    FechaPublicaion = null;
                    Imagen = null;
                    TextInput = null;
                    mItems.Clear();
                    mItems = null;
                }
                catch
                {
                    //Si falla al hacer un dispose no rompemos el proceso
                }
            }
        }
        #endregion




    }
}
