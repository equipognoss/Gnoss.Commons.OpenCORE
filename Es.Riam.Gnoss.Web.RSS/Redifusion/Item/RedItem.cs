using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Es.Riam.Gnoss.Web.RSS.Redifusion
{
    public class RedItem
    {
        private string mTitulo = string.Empty;
        private Uri mLink = null;
        private string mDescripcion = string.Empty;
        private string mResumen = string.Empty;
        private List<RedCategoria> mCategorias = new List<RedCategoria>();
        private Uri mComentarios = null;
        private RedItemId mId = null;
        private DateTime? mFechaPublicacion = null;
        private List<RedAdjunto> mAdjuntos = new List<RedAdjunto>();
        private List<RedPersona> mAutores = new List<RedPersona>();
        private List<RedPersona> mPublicadores = new List<RedPersona>();
        private List<RedPersona> mContribuyentes = new List<RedPersona>();
        internal RedFuente mFuente = null;

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

        public List<RedAdjunto> Adjuntos
        {
            set { mAdjuntos = value; }
            get { return mAdjuntos; }
        }

        public string Descripcion
        {
            set { mDescripcion = UtilCadenas.HtmlDecode(value); }
            get { return mDescripcion; }
        }

        public string Resumen
        {
            set { mResumen = UtilCadenas.HtmlDecode(value); }
            get { return mResumen; }
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

        public List<RedCategoria> Categorias
        {
            set { mCategorias = value; }
            get { return mCategorias; }
        }

        public Uri Comentarios
        {
            set { mComentarios = value; }
            get { return mComentarios; }
        }

        public RedItemId Id
        {
            set { mId = value; }
            get { return mId; }
        }
        public DateTime? FechaPublicacion
        {
            set { mFechaPublicacion = value; }
            get { return mFechaPublicacion; }
        }

        public RedFuente Fuente
        {
            get { return mFuente; }
        }

        public string DescripcionDetallada
        {
            get
            {
                string descripcion = "";

                if (mDescripcion != "")
                {
                    descripcion = mDescripcion;
                }
                else
                {
                    descripcion = mResumen;
                }

                return descripcion + AgnadirDescripcion();
            }
        }



        public string DescripcionResumida
        {
            get
            {
                string descripcion = "";

                if (mResumen != "")
                {
                    descripcion = mResumen;
                }
                else
                {
                    Regex regex = new Regex(@"<(.|\n)*?>", RegexOptions.Compiled);
                    descripcion = regex.Replace(mDescripcion, string.Empty);
                }
                return descripcion + AgnadirDescripcion();
            }
        }

        private string AgnadirDescripcion()
        {
            String descripcion = "";

            //Publicado en .... por .... el ....
            descripcion += "<p>Publicado en ";
            if (mFuente.Link == null || mFuente.Link.ToString() == "")
            {
                descripcion += mFuente.Titulo;
            }
            else
            {
                descripcion += "<a href=\"" + mFuente.Link.ToString() + "\">" + mFuente.Titulo + "</a>";
            }
            if (mPublicadores.Count > 0)
            {
                descripcion += " por ";
                int i = 0;
                foreach (RedPersona persona in mPublicadores)
                {
                    if (i == 0)
                    {
                        descripcion += persona.Nombre;
                    }
                    else
                    {
                        descripcion += ", " + persona.Nombre;
                    }
                    i++;
                }
            }
            if (mFechaPublicacion != null)
            {
                descripcion += " el: " + mFechaPublicacion.Value.ToString();
            }

            descripcion += "</p>";

            //Puedes ver los comentarios originales aqui            
            if (mComentarios != null)
            {
                descripcion += "<p>Puedes ver los comentarios originales ";
                descripcion += "<a href=\"" + mComentarios.ToString() + "\">aqui</a>";
                descripcion += "</p>";
            }

            //Puedes ver los archivos adjuntos ....
            if (mAdjuntos.Count > 0)
            {
                descripcion += "<p>Puedes ver los archivos adjuntos: ";
                int j = 0;
                foreach (RedAdjunto adjunto in mAdjuntos)
                {
                    string nombre = adjunto.Titulo;
                    if (nombre == "")
                    {
                        nombre = adjunto.Href.AbsoluteUri.Substring(adjunto.Href.AbsoluteUri.LastIndexOf('/') + 1);
                    }
                    if (j > 0)
                    {
                        descripcion += ", ";
                    }
                    descripcion += "<a href=\"" + adjunto.Href.ToString() + "\">" + nombre + "</a>";
                    j++;
                }
                descripcion += "</p>";
            }
            return descripcion;
        }
    }
}
