using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Comentario;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Comentario
{
    /// <summary>
    /// Lógica de comentario
    /// </summary>
    public class ComentarioCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ComentarioCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ComentarioCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.ComentarioAD = new ComentarioAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor para ComentarioCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ComentarioCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ComentarioCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.ComentarioAD = new ComentarioAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ComentarioAD>(), mLoggerFactory);
        }

        #endregion

        #region Métodos generales
        /// <summary>
        /// Obtiene los comentarios de una lista de comentarios pasados por parámetro
        /// </summary>
        /// <param name="pListaComentarioID">Lista de identificadores de comentario</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerComentariosPorID(List<Guid> pListaComentarioID)
        {
            return ComentarioAD.ObtenerComentariosPorID(pListaComentarioID);
        }

        public AD.EntityModel.Models.Comentario.Comentario ObtenerComentarioPorID(Guid pComentarioID)
        {
            return ComentarioAD.ObtenerComentarioPorID(pComentarioID);
        }

        /// <summary>
        /// Obtiene los comentarios de una lista de comentarios pasados por parámetro con el autor y el padre del elemento vinculado
        /// </summary>
        /// <param name="pListaComentarioID">Lista de identificadores de comentario</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerEntradasBlogPorIDConPadreElemVinYAutor(List<Guid> pListaComentarioID)
        {
            return ComentarioAD.ObtenerEntradasBlogPorIDConPadreElemVinYAutor(pListaComentarioID);
        }

        /// <summary>
        /// Obtiene todos los comentarios
        /// </summary>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerTodosComentarios()
        {
            return ComentarioAD.ObtenerTodosComentarios();
        }

        /// <summary>
        /// Obtiene los comentarios de una entrada de blog
        /// </summary>
        /// <param name="pBlogID">Identificador de blog</param>
        /// <param name="pEntradaBlogID">Identificador de entrada de blog</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerComentariosDeEntradaBlog(Guid pBlogID, Guid pEntradaBlogID)
        {
            return ComentarioAD.ObtenerComentariosDeEntradaBlog(pBlogID, pEntradaBlogID);
        }

        /// <summary>
        /// Obtiene los comentarios de un blog
        /// </summary>
        /// <param name="pBlogID">Identificador de blog</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerComentariosDeBlog(Guid pBlogID)
        {
            return ComentarioAD.ObtenerComentariosDeBlog(pBlogID);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <returns>DataSet de Comentario</returns>
        public DataWrapperComentario ObtenerComentariosDeDocumento(Guid pDocumentoID)
        {
            return ObtenerComentariosDeDocumento(pDocumentoID, Guid.Empty);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        /// <param name="pUsuarioID">UsuarioID</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <returns>DataSet de Comentario</returns>
        public int ObtenerComentariosDeUsuarioEnProyecto(Guid pIdentidadID, Guid pProyectoID, DateTime? pFechaInit, DateTime? pFechaFin)
        {
            return ComentarioAD.ObtenerComentariosdeUsuarioEnProyecto(pIdentidadID, pProyectoID, pFechaInit, pFechaFin);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <returns>DataSet de Comentario</returns>
        public DataWrapperComentario ObtenerComentariosDeDocumento(Guid pDocumentoID, Guid pSoloProyectoSinPrivadosID)
        {
            return ComentarioAD.ObtenerComentariosDeDocumento(pDocumentoID, pSoloProyectoSinPrivadosID);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        /// <param name="pComentarioDS">DataSet de Comentario</param>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Limite paginación</param>
        /// <param name="pMostrarComentariorSoloProyecto">Indica si se deben mostrar solo los comentarios este proyecto</param>
        /// <returns>Numero de resultados raíz totales</returns>
        public int ObtenerComentariosDeDocumento(DataWrapperComentario pComentarioDS, Guid pDocumentoID, Guid pSoloProyectoSinPrivadosID, int pInicio, int pLimite, bool pMostrarComentariorSoloProyecto)
        {
            List<Guid> listDocID = new List<Guid>();
            listDocID.Add(pDocumentoID);
            return ObtenerComentariosDeDocumentos(pComentarioDS, listDocID, pSoloProyectoSinPrivadosID, pInicio, pLimite, pMostrarComentariorSoloProyecto);
        }

        /// <summary>
        /// Obtiene los comentarios de unos documentos.
        /// </summary>
        /// <param name="pComentarioDS">DataSet de Comentario</param>
        ///<param name="pDocumentosID">Identificadores de documentos</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <param name="pInicio">Inicio paginación</param>
        /// <param name="pLimite">Limite paginación</param>
        /// <param name="pMostrarComentariorSoloProyecto">Indica si se deben mostrar solo los comentarios este proyecto</param>
        /// <returns>Numero de resultados raíz totales</returns>
        public int ObtenerComentariosDeDocumentos(DataWrapperComentario pComentarioDS, List<Guid> pDocumentosID, Guid pSoloProyectoSinPrivadosID, int pInicio, int pLimite, bool pMostrarComentariorSoloProyecto)
        {
            return ComentarioAD.ObtenerComentariosDeDocumentos(pComentarioDS, pDocumentosID, pSoloProyectoSinPrivadosID, pInicio, pLimite, pMostrarComentariorSoloProyecto);
        }

        /// <summary>
        /// Obtiene los comentarios de documentos hijos de los comentarios ya cargados.
        /// </summary>
        /// <param name="pComentarioDS">DataSet de comentario</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        public void ObtenerComentariosDocHijosDeComentariosCargados(DataWrapperComentario pComentarioDS, Guid pSoloProyectoSinPrivadosID)
        {
            ComentarioAD.ObtenerComentariosDocHijosDeComentariosCargados(pComentarioDS, pSoloProyectoSinPrivadosID);
        }

        public List<AD.EntityModel.Models.Comentario.Comentario> ObtenerTodosComentariosHijosDeComentarios(List<AD.EntityModel.Models.Comentario.Comentario> pComentarioID)
        {
            return ComentarioAD.ObtenerTodosComentariosHijosDeComentarios(pComentarioID);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <returns>DataSet de Comentario</returns>
        public string ObtenerUltimoComentarioDeDocumento(Guid pDocumentoID)
        {
            return ComentarioAD.ObtenerUltimoComentarioDeDocumento(pDocumentoID);
        }

        /// <summary>
        /// Obtiene los comentarios de un documento.
        /// </summary>
        ///<param name="pDocumentoID">Identificador de documento</param>
        /// <param name="pSoloProyectoSinPrivadosID">Contiene el proyecto del que se deben traer los coment a parte de los publicos
        /// y restringidos. Empty si se deben traer todos</param>
        /// <returns>DataSet de Comentario</returns>
        public DataWrapperComentario ObtenerUltimoComentarioDeDocumentoCompleto(Guid pDocumentoID)
        {
            return ComentarioAD.ObtenerUltimoComentarioDeDocumentoCompleto(pDocumentoID);
        }

        /// <summary>
        /// Indica si un documento tiene comentario en otra comunidad que no es la indicada.
        /// </summary>
        /// <param name="pDocumentoID">Documento</param>
        /// <param name="pProyectoID">Comunindad</param>
        /// <returns>TRUE si un documento tiene comentario en otra comunidad que no es la indicada, FALSE en caso contrario</returns>
        public bool DocumentoTieneComentarioOtraComunidad(Guid pDocumentoID, Guid pProyectoID)
        {
            return ComentarioAD.DocumentoTieneComentarioOtraComunidad(pDocumentoID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los comentarios hechos por una persona en todos los blogs 
        /// </summary>
        /// <param name="pAutorBlog">Identidad del autor</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerComentariosDeBlogDePersona(Guid pAutorBlog)
        {
            return ComentarioAD.ObtenerComentariosDeBlogDePersona(pAutorBlog);
        }

        /// <summary>
        /// Actualiza Comentario 
        /// </summary>
        public void ActualizarComentarioEntity()
        {
            ComentarioAD.ActualizarComentarioEntity();

        }

        /// <summary>
        /// Obtiene los comentarios de la persona en myGnoss
        /// </summary>
        /// <param name="pPersonaID">Persona para la que se obtienen los comentarios</param>
        /// <param name="pOrganizazionID">Organizacion del modo en el que se encuentra, null si el modo es personal</param>
        /// <param name="EsAdmin">verdad si el modo es de organizacion y es el administrador de la misma</param>
        /// <param name="pEsProfesor">Indica si la persona está conectada con identidad de profesor</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeMyGnoss(Guid pPersonaID, Guid? pOrganizazionID, bool EsAdmin, bool pEsProfesor)
        {
            return ComentarioAD.ObtenerComentariosDeMyGnoss(pPersonaID, pOrganizazionID, EsAdmin, pEsProfesor);
        }

        /// <summary>
        /// Compruebo si una identidad ha hecho algún comentario
        /// </summary>
        /// <param name="pIdentidadID">Identidad a comprobar</param>
        /// <returns></returns>
        public bool ComprobarIdentidadHaComentado(Guid pIdentidadID)
        {
            return ComentarioAD.ComprobarIdentidadHaComentado(pIdentidadID);
        }

        /// <summary>
        /// Obtiene los comentarios de las identidades 
        /// </summary>
        /// <param name="pListaIdentidadesID">Lista de identidades para la que se obtienen los comentarios</param>
        /// <returns>Dataset de comentarios</returns>
        public DataWrapperComentario ObtenerComentariosDeIdentidades(List<Guid> pListaIdentidadesID)
        {
            return ComentarioAD.ObtenerComentariosDeIdentidades(pListaIdentidadesID);
        }

        /// <summary>
        /// Obtiene los comentarios de las identidades
        /// </summary>
        /// <param name="pListaComentariosID">Identidades para la que se obtienen los comentarios</param>
        /// <returns></returns>
        public DataWrapperComentario ObtenerComentariosDeDocumentosPorComentariosID(List<Guid> pListaComentariosID)
        {
            return ComentarioAD.ObtenerComentariosDeDocumentosPorComentariosID(pListaComentariosID);
        }

        /// <summary>
        /// Obtiene el numero de comentarios de un documento por su clave
        /// </summary>
        /// <param name="pClave"></param>
        /// <returns></returns>
        public int ObtenerNumeroDeComentariosDeDocumento(Guid pClave)
        {
            return ComentarioAD.ObtenerNumeroDeComentariosDeDocumento(pClave);
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
        ~ComentarioCN()
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
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (ComentarioAD != null)
                    {
                        ComentarioAD.Dispose();
                    }
                }
                ComentarioAD = null;
            }
        }

        #endregion

        #region Propiedades

        private ComentarioAD ComentarioAD
        {
            get
            {
                return (ComentarioAD)AD;
            }
            set
            {
                AD = value;
            }
        }



        #endregion

    }
}
