using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Organizador.Correo.Model;
using Es.Riam.Gnoss.Util.General;
using System;
using IDE = Es.Riam.Gnoss.Elementos.Identidad.Identidad;

namespace Es.Riam.Gnoss.Elementos.Organizador.Correo
{
    /// <summary>
    /// Clase para correo
    /// </summary>
    public class Correo : ElementoGnoss
    {
        #region Miembros

        /// <summary>
        /// Fila del correo
        /// </summary>
        protected CorreoDS.CorreoInternoRow mFilaCorreo;

        /// <summary>
        /// Gestor de correo
        /// </summary>
        protected GestionCorreo mGestionCorreo;

        /// <summary>
        /// Indica si el correo está marcado
        /// </summary>
        protected bool mEstaMarcado;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public Correo(LoggingService loggingService)
            : base(loggingService)
        {
        }

        /// <summary>
        /// Constructor a partir de una fila y un gestor de correo
        /// </summary>
        /// <param name="pFilaCorreo">Fila de correo</param>
        /// <param name="pGestionCorreo">Gestor de correo</param>
        public Correo(CorreoDS.CorreoInternoRow pFilaCorreo, GestionCorreo pGestionCorreo, LoggingService loggingService)
            : base(loggingService)
        {
            mFilaCorreo = pFilaCorreo;
            mGestionCorreo = pGestionCorreo;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la fila del correo
        /// </summary>
        public CorreoDS.CorreoInternoRow FilaCorreo
        {
            get
            {
                return mFilaCorreo;
            }
            set
            {
                mFilaCorreo = value;
            }
        }

        /// <summary>
        /// Obtiene el identificador del correo
        /// </summary>
        public override Guid Clave
        {
            get
            {
                return FilaCorreo.CorreoID;
            }
        }

        /// <summary>
        /// Indica si el mensaje es enviado
        /// </summary>
        public bool Enviado
        {
            get
            {
                return FilaCorreo.Destinatario == Guid.Empty;
            }
        }

        /// <summary>
        /// Indica si el mensaje es redcibido
        /// </summary>
        public bool Recibido
        {
            get
            {
                return FilaCorreo.Destinatario != Guid.Empty;
            }
        }

        /// <summary>
        /// Devuelve la identidad del autor del correo
        /// </summary>
        public IDE Autor
        {
            get
            {
                //Recupero el objeto identidad de la lista del gestor
                if (mGestionCorreo.GestorIdentidades.ListaIdentidades.ContainsKey(FilaCorreo.Autor))
                {
                    return mGestionCorreo.GestorIdentidades.ListaIdentidades[FilaCorreo.Autor];
                }
                return null;
            }
        }

        /// <summary>
        /// Devuelve la identidad del destinatario del correo
        /// </summary>
        public IDE Destinatario
        {
            get
            {
                //Recupero el objeto identidad de la lista del gestor
                if (mGestionCorreo.GestorIdentidades.ListaIdentidades.ContainsKey(FilaCorreo.Destinatario))
                {
                    return mGestionCorreo.GestorIdentidades.ListaIdentidades[FilaCorreo.Destinatario];
                }
                return null;
            }
        }

        /// <summary>
        /// Obtiene el nombre del autor del correo
        /// </summary>
        public string NombreAutor
        {
            get
            {
                if (Autor != null)
                {
                    if (Autor.Tipo != TiposIdentidad.Organizacion)
                    {
                        return Autor.Nombre();
                    }
                    else
                    {
                        return Autor.NombreOrganizacion;
                    }
                }
                else
                {
                    return "myGnoss";
                }
            }
        }

        /// <summary>
        /// Obtiene el nombre del destinatario del correo
        /// </summary>
        public string NombreDestinatario
        {
            get
            {
                if (Destinatario != null)
                {
                    if (Destinatario.Tipo != TiposIdentidad.Organizacion)
                    {
                        return Destinatario.Nombre();
                    }
                    else
                    {
                        return Destinatario.NombreOrganizacion;
                    }
                }
                else
                {
                    return "myGnoss";
                }
            }
        }

        /// <summary>
        /// Obtiene el asunto del correo
        /// </summary>
        public string Asunto
        {
            get
            {
                return FilaCorreo.Asunto;
            }
        }

        /// <summary>
        /// Devuelve el cuerpo del correo
        /// </summary>
        public string Cuerpo
        {
            get
            {
                return FilaCorreo.Cuerpo;
            }
        }

        /// <summary>
        /// Devuelve la fecha en la que ha sido enviado el correo
        /// </summary>
        public override DateTime Fecha
        {
            get
            {
                return FilaCorreo.Fecha;
            }
        }

        /// <summary>
        /// Devuelve el gestor de correo
        /// </summary>
        public GestionCorreo GestionCorreo
        {
            get
            {
                return mGestionCorreo;
            }
        }

        /// <summary>
        /// Obtiene o establece si el correo está marcado
        /// </summary>
        public bool EstaMarcado
        {
            get
            {
                return mEstaMarcado;
            }
            set
            {
                mEstaMarcado = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el correo ha sido leído
        /// </summary>
        public bool Leido
        {
            get
            {
                return FilaCorreo.Leido;
            }
            set
            {
                FilaCorreo.Leido = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el correo ha sido eliminado
        /// </summary>
        public bool Eliminado
        {
            get
            {
                return FilaCorreo.Eliminado;
            }
            set
            {
                FilaCorreo.Eliminado = value;
            }
        }

        #endregion
    }
}
