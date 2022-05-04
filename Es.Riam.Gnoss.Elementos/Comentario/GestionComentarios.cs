using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Gnoss.Elementos.Comentario
{
    /// <summary>
    /// Gestor de comentarios
    /// </summary>
    [Serializable]
    public class GestionComentarios : GestionGnoss, ISerializable, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Gestor de votos de comentario
        /// </summary>
        private GestorVotosComentario mGestorVotos;

        /// <summary>
        /// Lista de comentarios
        /// </summary>
        protected Dictionary<Guid, Comentario> mListaComentarios;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pComentarioDW">Dataset de comentarios</param>
        public GestionComentarios(DataWrapperComentario pComentarioDW,  LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pComentarioDW, loggingService)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionComentarios(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //mLoggingService = loggingService;
            //mEntityContext = entityContext;
            //mConfigService = configService;

            mGestorVotos = (GestorVotosComentario)info.GetValue("GestorVotos", typeof(GestorVotosComentario));
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Recarga la lista de comentarios del gestor
        /// </summary>
        public virtual void RecargarComentarios()
        {
            mListaComentarios = new Dictionary<Guid, Comentario>();

            foreach (AD.EntityModel.Models.Comentario.Comentario filaComentario in ComentarioDW.ListaComentario)
            {
                if (!ListaComentarios.ContainsKey(filaComentario.ComentarioID))
                {
                    Comentario comentario = new Comentario(filaComentario, this,  mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mListaComentarios.Add(comentario.Clave, comentario);
                }
            }
        }

        /// <summary>
        /// Agrega un comentario nuevo
        /// </summary>
        /// <param name="pTexto">Texto del nuevo comentario</param>
        /// <param name="pIdentidadID">Identificador de la identidad que publica el comentario</param>
        /// <returns>Comentario nuevo</returns>
        public Comentario AgregarComentario(string pTexto, Guid pIdentidadID)
        {
            AD.EntityModel.Models.Comentario.Comentario filaComentario = new AD.EntityModel.Models.Comentario.Comentario();
            filaComentario.ComentarioID = Guid.NewGuid();
            filaComentario.IdentidadID = pIdentidadID;
            filaComentario.Descripcion = pTexto;
            filaComentario.Fecha = DateTime.Now;
            filaComentario.Eliminado = false;

            Comentario nuevoComentario = new Comentario(filaComentario, this,  mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            ComentarioDW.ListaComentario.Add(nuevoComentario.FilaComentario);
            mEntityContext.Comentario.Add(filaComentario);
            if (!ListaComentarios.ContainsKey(nuevoComentario.Clave))
            {
                ListaComentarios.Add(nuevoComentario.Clave, nuevoComentario);
            }
            return nuevoComentario;
        }

        /// <summary>
        /// Elimina un comentario
        /// </summary>
        /// <param name="pComentario">Comentario para eliminar</param>
        public void EliminarComentario(Comentario pComentario)
        {
            pComentario.FilaComentario.Eliminado = true;
        }

        #region Estáticos

        /// <summary>
        /// Compara los dos comentarios dados por su fecha de publicación
        /// </summary>
        /// <param name="x">Comentario x</param>
        /// <param name="y">Comentario y</param>
        /// <returns>Entero indicando cual es mayor por fecha</returns>
        public static int CompararComentariosPorFecha(Comentario x, Comentario y)
        {
            return y.Fecha.CompareTo(x.Fecha);
        }

        /// <summary>
        /// Compara los dos comentarios dados por su fecha de publicación de manera descendente
        /// </summary>
        /// <param name="x">Comentario x</param>
        /// <param name="y">Comentario y</param>
        /// <returns>Entero indicando cual es mayor por fecha de manera descendente</returns>
        public static int CompararComentariosPorFechaDesc(Comentario x, Comentario y)
        {
            return x.Fecha.CompareTo(y.Fecha);
        }

        /// <summary>
        /// Compara los dos comentarios dados por el nombre de su autor
        /// </summary>
        /// <param name="x">Comentario x</param>
        /// <param name="y">Comentario y</param>
        /// <returns>Entero indicando cual es mayor por autor</returns>
        public static int CompararComentariosPorColumnaNombreAutor(Comentario x, Comentario y)
        {
            return x.NombreAutor.CompareTo(y.NombreAutor);
        }

        /// <summary>
        /// Compara los dos comentarios dados por el nombre de su autor de manera descendente
        /// </summary>
        /// <param name="x">Comentario x</param>
        /// <param name="y">Comentario y</param>
        /// <returns>Entero indicando cual es mayor por autor de manera descendente</returns>
        public static int CompararComentariosPorColumnaNombreAutorDesc(Comentario x, Comentario y)
        {
            return y.NombreAutor.CompareTo(x.NombreAutor);
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el dataset de comentarios
        /// </summary>
        public DataWrapperComentario ComentarioDW
        {
            get
            {
                return (DataWrapperComentario)DataWrapper;
            }
        }

        /// <summary>
        /// Obtiene la lista de comentarios del gestor
        /// </summary>
        public Dictionary<Guid, Comentario> ListaComentarios
        {
            get
            {
                if (mListaComentarios == null)
                {
                    RecargarComentarios();
                }
                return mListaComentarios;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de votos
        /// </summary>
        public GestorVotosComentario GestorVotos
        {
            get
            {
                return mGestorVotos;
            }
            set
            {
                mGestorVotos = value;
            }
        }

        /// <summary>
        /// Devuelve el número de comentarios Raiz del gestor.
        /// </summary>
        public int NumeroDeComentariosRaiz
        {
            get
            {
                int count = 0;

                foreach (Comentario comentario in ListaComentarios.Values)
                {
                    if (!comentario.FilaComentario.ComentarioSuperiorID.HasValue)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("GestorVotos", GestorVotos);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GestionComentarios()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                disposed = true;
                
                try
                {
                    if(disposing)
                    {
                        if (mListaComentarios != null)
                        {
                            foreach (Comentario comentario in mListaComentarios.Values)
                            {
                                comentario.Dispose();
                            }
                            mListaComentarios.Clear();
                        }
                    }
                }
                finally
                {
                    mGestorVotos = null;
                    mListaComentarios = null;

                    // Llamo al dispose de la clase base
                    base.Dispose(disposing);
                }
            }
        }

        #endregion
    }
}
