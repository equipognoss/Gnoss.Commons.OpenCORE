using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Voto;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Voto
{
    //TODO: EDU COMPLETAR LOS COMENTARIOS QUE FALTAN

    /// <summary>
    /// Lógica de VotoCN
    /// </summary>
    public class VotoCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor para VotoCN
        /// </summary>
        public VotoCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VotoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            VotoAD = new VotoAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        public VotoCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VotoCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            VotoAD = new VotoAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<VotoAD>(), mLoggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos

        ///// <summary>
        ///// Actualiza BD
        ///// </summary>
        ///// <param name="pVotoDS">Dataset de votos</param>
        //public void ActualizarBD(VotoDS pVotoDS)
        //{
        //    try
        //    {
        //        if (Transaccion != null)
        //        {
        //            this.VotoAD.ActualizarBD(pVotoDS);
        //        }
        //        else
        //        {
        //            IniciarTransaccion();
        //            {
        //                this.VotoAD.ActualizarBD(pVotoDS);
        //                if (pVotoDS != null)
        //                {
        //                    pVotoDS.AcceptChanges();
        //                }
        //                TerminarTransaccion(true);
        //            }
        //        }
        //        

        //    }
        //    catch (DBConcurrencyException ex)
        //    {
        //        TerminarTransaccion(false);
        //        // Error de concurrencia
        //        Error.GuardarLogError(ex);
        //        throw new ErrorConcurrencia(ex.Row);
        //    }
        //    catch (DataException ex)
        //    {
        //        TerminarTransaccion(false);
        //        //Error interno de la aplicación
        //        Error.GuardarLogError(ex);
        //        throw new ErrorInterno();
        //    }
        //    catch
        //    {
        //        TerminarTransaccion(false);
        //        throw;
        //    }
        //}

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pVotoDS">Dataset de votos</param>
        public void ActualizarEntity()
        {
            base.Actualizar();
        }

        /// <summary>
        /// Obtiene los votos de un documento por IDVoto
        /// </summary>
        /// <param name="pVotoID">Identificador de voto</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosDocumentoPorIDVoto(Guid pVotoID)
        {
            return VotoAD.ObtenerVotosDocumentoPorIDVoto(pVotoID);

        }

        /// <summary>
        /// Obtiene los votos de una entrada
        /// </summary>
        /// <param name="pBlogID">Identificador del blog</param>
        /// <param name="pEntradaBlogID">Identificador de la entrada de blog</param>
        /// <returns>Dataset de votos</returns>
        public DataWrapperVoto ObtenerVotosEntradaBlogPorID(Guid pBlogID, Guid pEntradaBlogID)
        {
            return VotoAD.ObtenerVotosEntradaBlogPorID(pBlogID, pEntradaBlogID);
        }

        /// <summary>
        /// Obtiene los votos de una entrada
        /// </summary>
        /// <param name="pForoID">Identificador del foro</param>
        /// <param name="pCategoriaForoID">Identificador de la categoria de foro</param>
        /// <param name="pTemaID">Identificador del tema de foro</param>
        /// <returns>Dataset de votos</returns>
        public DataWrapperVoto ObtenerVotosTemaForoPorID(Guid pForoID, Guid pCategoriaForoID, Guid pTemaID)
        {
            return VotoAD.ObtenerVotosTemaForoPorID(pForoID, pCategoriaForoID, pTemaID);
        }

        /// <summary>
        /// Obtiene los votos de los comentarios una entrada
        /// </summary>
        /// <param name="pBlogID">Identificador del blog</param>
        /// <param name="pEntradaBlogID">Identificador de la entrada de blog</param>
        /// <returns>Dataset de votos</returns>
        public DataWrapperVoto ObtenerVotosComentariosEntradaBlogPorID(Guid pBlogID, Guid pEntradaBlogID)
        {
            return VotoAD.ObtenerVotosComentariosEntradaBlogPorID(pBlogID, pEntradaBlogID);
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>DataSet de voto</returns>
        public DataWrapperVoto ObtenerVotosDocumentoPorID(Guid pDocumentoID)
        {
            return VotoAD.ObtenerVotosDocumentoPorID(pDocumentoID);
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pProyectoID">proyecto </param>
        public string ObtenerNumeroVotos(Guid pDocumentoID, Guid pProyectoID)
        {
            return VotoAD.ObtenerNumeroVotos(pDocumentoID, pProyectoID);
        }


        /// <summary>
        /// Obtiene los votos de varios documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documento</param>
        /// <returns>DataSet de voto</returns>
        public DataWrapperVoto ObtenerVotosDocumentosPorID(List<Guid> pListaDocumentosID)
        {
            return VotoAD.ObtenerVotosDocumentosPorID(pListaDocumentosID);
        }

        /// <summary>
        /// Obtiene los votos de los comentarios un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns>DataSet de voto</returns>
        public DataWrapperVoto ObtenerVotosComentariosDocumentoPorID(Guid pDocumentoID)
        {
            return VotoAD.ObtenerVotosComentariosDocumentoPorID(pDocumentoID);
        }

        /// <summary>
        /// Devuelve los votos de los comentarios pasados como parametro.
        /// </summary>
        /// <param name="mListaComentariosID">Lista con los identificadores de los comentarios</param>
        /// <param name="pComentarioDW">DataSet de comentario</param>
        /// <returns>DataSet con los votos de los comentarios pasados como parametro</returns>
        public DataWrapperVoto ObtenerVotosComentariosPorComentarioID(List<Guid> mListaComentariosID, DataWrapperComentario pComentarioDW)
        {
            return VotoAD.ObtenerVotosComentariosPorComentarioID(mListaComentariosID, pComentarioDW);
        }

        public Guid? ObtenerVotosPorUsuario(Guid pDocumentoID, Guid pIdentidadID, Guid pProyectoID)
        {
            return VotoAD.ObtenerVotosPorUsuario(pDocumentoID, pIdentidadID, pProyectoID);
        }

        public void ActualizarVoto(Guid pVotoID, Guid pIdentidadID, float pValorVoto)
        {
            VotoAD.ActualizarVoto(pVotoID, pIdentidadID, pValorVoto);
        }


        public void insertarVotoDocumento(Guid pDocumentoID, Guid pProyectoID, Guid pIdentidadID, float pVoto, Guid pIdentidadVotadaID)
        {
            Guid guidVoto = Guid.NewGuid();

            VotoAD.InsertarVoto(pIdentidadID, pDocumentoID, pVoto, pIdentidadVotadaID, guidVoto);
            VotoAD.InsertarVotoDocumento(pDocumentoID, pProyectoID, guidVoto);
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
        ~VotoCN()
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
                    if (VotoAD != null)
                    {
                        VotoAD.Dispose();
                    }
                }
                VotoAD = null;
            }
        }


        #endregion

        #region Propiedades

        private VotoAD VotoAD
        {
            get
            {
                return (VotoAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
