using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Voto;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Voto
{
    public class JoinVotoVotoComentario
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoComentario VotoComentario { get; set; }
    }

    public class JoinVotoVotoComentarioDocumentoComentario
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoComentario VotoComentario { get; set; }
        public DocumentoComentario DocumentoComentario { get; set; }
    }

    public class JoinVotoVotoDocumento
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoDocumento VotoDocumento { get; set; }
    }

    public class JoinVotoVotoComentarioComentarioBlog
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoComentario VotoComentario { get; set; }
        public ComentarioBlog ComentarioBlog { get; set; }
    }

    public class JoinVotoVotoEntradaBlog
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoEntradaBlog VotoEntradaBlog { get; set; }
    }

    public class JoinVotoVotoMensajeForo
    {
        public EntityModel.Models.Voto.Voto Voto { get; set; }
        public VotoMensajeForo VotoMensajeForo { get; set; }
    }

    //INNER JOIN VotoMensajeForo ON VotoMensajeForo.VotoID = Voto.VotoID

    public static class Joins
    {
        public static IQueryable<JoinVotoVotoMensajeForo> JoinVotoMensajeForo(this IQueryable<EntityModel.Models.Voto.Voto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VotoMensajeForo, voto => voto.VotoID, votoMensajeForo => votoMensajeForo.VotoID, (voto, votoMensajeForo) => new JoinVotoVotoMensajeForo
            {
                VotoMensajeForo = votoMensajeForo,
                Voto = voto
            });
        }

        public static IQueryable<JoinVotoVotoEntradaBlog> JoinVotoEntradaBlog(this IQueryable<EntityModel.Models.Voto.Voto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VotoEntradaBlog, voto => voto.VotoID, votoEntradaBlog => votoEntradaBlog.VotoID, (voto, votoEntradaBlog) => new JoinVotoVotoEntradaBlog
            {
                VotoEntradaBlog = votoEntradaBlog,
                Voto = voto
            });
        }

        public static IQueryable<JoinVotoVotoComentarioComentarioBlog> JoinComentarioBlog(this IQueryable<JoinVotoVotoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.ComentarioBlog, item => item.VotoComentario.ComentarioID, comentarioBlog => comentarioBlog.ComentarioID, (item, comentarioBlog) => new JoinVotoVotoComentarioComentarioBlog
            {
                ComentarioBlog = comentarioBlog,
                Voto = item.Voto,
                VotoComentario = item.VotoComentario
            });
        }

        public static IQueryable<JoinVotoVotoDocumento> JoinVotoDocumento(this IQueryable<EntityModel.Models.Voto.Voto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VotoDocumento, voto => voto.VotoID, votoDocumento => votoDocumento.VotoID, (voto, votoDocumento) => new JoinVotoVotoDocumento
            {
                VotoDocumento = votoDocumento,
                Voto = voto
            });
        }

        public static IQueryable<JoinVotoVotoComentarioDocumentoComentario> JoinDocumentoComentario(this IQueryable<JoinVotoVotoComentario> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.DocumentoComentario, item => item.VotoComentario.ComentarioID, documentoComentario => documentoComentario.ComentarioID, (item, documentoComentario) => new JoinVotoVotoComentarioDocumentoComentario
            {
                DocumentoComentario = documentoComentario,
                Voto = item.Voto,
                VotoComentario = item.VotoComentario
            });
        }

        public static IQueryable<JoinVotoVotoComentario> JoinVotoComentario(this IQueryable<EntityModel.Models.Voto.Voto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.VotoComentario, voto => voto.VotoID, votoComentario => votoComentario.VotoID, (voto, votoComentario) => new JoinVotoVotoComentario
            {
                VotoComentario = votoComentario,
                Voto = voto
            });
        }
    }

    /// <summary>
    /// DataAdapter de Voto
    /// </summary>
    public class VotoAD : BaseAD
    {
        private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public VotoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VotoAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public VotoAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<VotoAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectVoto;
        private string sqlSelectVotoPorIDVoto;
        private string sqlSelectVotosEntradaBlogPorID;
        private string sqlSelectVotosComentariosEntradaBlogPorID;
        private string sqlSelectVotosTemaForoPorID;
        private string sqlSelectVotosDocumentoPorID;
        private string sqlSelectVotosComentariosDocumentoPorID;
        private string sqlSelectVotosComentariosFactorPorID;

        #endregion

        #region DataAdapter

        #region Voto

        private string sqlVotoInsert;
        private string sqlVotoDelete;
        private string sqlVotoModify;

        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Actualiza la base de datos con las modificaciones del dataset pasado por parámetro
        /// </summary>
        /// <param name="pDataSet">Dataset de voto</param>
        public void ActualizarBD(DataSet pDataSet)
        {
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }

        /// <summary>
        /// Elimina datos de votos
        /// </summary>
        /// <param name="pDataSet">Dataset de voto</param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted
                    #region Eliminar tabla Voto
                    //DbCommand DeleteVotoCommand = ObtenerComando(sqlVotoDelete);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Original);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Original);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_IdentidadVotadaID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadVotadaID", DataRowVersion.Original);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_Voto"), DbType.Double, "Voto", DataRowVersion.Original);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    //AgregarParametro(DeleteVotoCommand, IBD.ToParam("O_FechaVotacion"), DbType.DateTime, "FechaVotacion", DataRowVersion.Original);
                    //ActualizarBaseDeDatos(deletedDataSet, "Voto", null, null, DeleteVotoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    deletedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Guarda las modificaciones de votos realizadas
        /// </summary>
        /// <param name="pDataSet">Dataset de voto</param>
        public void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified
                    #region Actualizar tabla Voto
                    DbCommand InsertVotoCommand = ObtenerComando(sqlVotoInsert);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Current);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Current);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("IdentidadVotadaID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadVotadaID", DataRowVersion.Current);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("Voto"), DbType.Double, "Voto", DataRowVersion.Current);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertVotoCommand, IBD.ToParam("FechaVotacion"), DbType.DateTime, "FechaVotacion", DataRowVersion.Current);

                    DbCommand ModifyVotoCommand = ObtenerComando(sqlVotoModify);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_IdentidadVotadaID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadVotadaID", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_Voto"), DbType.Double, "Voto", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("O_FechaVotacion"), DbType.DateTime, "FechaVotacion", DataRowVersion.Original);

                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("VotoID"), IBD.TipoGuidToObject(DbType.Guid), "VotoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("IdentidadVotadaID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadVotadaID", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("Voto"), DbType.Double, "Voto", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyVotoCommand, IBD.ToParam("FechaVotacion"), DbType.DateTime, "FechaVotacion", DataRowVersion.Current);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "Voto", InsertVotoCommand, ModifyVotoCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Obtiene los votos de una entrada de blog pasada por parámetro
        /// </summary>
        /// <param name="pBlogID">Identificador del blog</param>
        /// <param name="pEntradaBlogID">Identificador de la entrada de blog</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosEntradaBlogPorID(Guid pBlogID, Guid pEntradaBlogID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.JoinVotoEntradaBlog().Where(item => item.VotoEntradaBlog.BlogID.Equals(pBlogID) && item.VotoEntradaBlog.EntradaBlogID.Equals(pEntradaBlogID)).Select(item => item.Voto).ToList();

            return votoDW;
        }

        /// <summary>
        /// Obtiene los votos de los comentarios de una entrada de blog pasada por parámetro
        /// </summary>
        /// <param name="pBlogID">Identificador del blog</param>
        /// <param name="pEntradaBlogID">Identificador de la entrada de blog</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosComentariosEntradaBlogPorID(Guid pBlogID, Guid pEntradaBlogID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.JoinVotoComentario().JoinComentarioBlog().Where(item => item.ComentarioBlog.BlogID.Equals(pBlogID) && item.ComentarioBlog.EntradaBlogID.Equals(pEntradaBlogID)).Select(item => item.Voto).ToList();

            return votoDW;
        }

        /// <summary>
        /// Obtiene los votos de un tema de foro pasado por parámetro
        /// </summary>
        /// <param name="pForoID">Identificador de foro</param>
        /// <param name="pCategoriaForoID">Identificador de la categoría de foro</param>
        /// <param name="pTemaID">Identificador del tema de foro</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosTemaForoPorID(Guid pForoID, Guid pCategoriaForoID, Guid pTemaID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.JoinVotoMensajeForo().Where(item => item.VotoMensajeForo.ForoID.Equals(pForoID) && item.VotoMensajeForo.CategoriaForoID.Equals(pCategoriaForoID) && item.VotoMensajeForo.TemaID.Equals(pTemaID)).Select(item => item.Voto).ToList();

            return votoDW;
        }


        /// <summary>
        /// Obtiene los votos de un documento por IDVoto
        /// </summary>
        /// <param name="pVotoID">Identificador de voto</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosDocumentoPorIDVoto(Guid pVotoID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.Where(item => item.VotoID.Equals(pVotoID)).ToList();

            return votoDW;
        }

        /// <summary>
        /// Obtiene los votos de un documento
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosDocumentoPorID(Guid pDocumentoID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.JoinVotoDocumento().Where(item => item.VotoDocumento.DocumentoID.Equals(pDocumentoID)).Select(item => item.Voto).ToList();

            return votoDW;
        }

        /// <summary>
        /// Obtiene los votos de un documento.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento.</param>
        /// <param name="pProyectoID">proyecto </param>
        public string ObtenerNumeroVotos(Guid pDocumentoID, Guid pProyectoID)
        {
            int numeroVotos = 0;

            DataWrapperVoto votoDW = new DataWrapperVoto();

            votoDW.ListaVotos = mEntityContext.Voto.JoinVotoDocumento().Where(item => item.VotoDocumento.DocumentoID.Equals(pDocumentoID) && item.VotoDocumento.ProyectoID.HasValue && item.VotoDocumento.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.Voto).ToList();

            numeroVotos = votoDW.ListaVotos.Count;

            return numeroVotos.ToString();
        }

        /// <summary>
        /// Obtiene los votos de varios documentos.
        /// </summary>
        /// <param name="pListaDocumentosID">Identificadores de los documento</param>
        /// <returns>DataSet de voto</returns>
        public DataWrapperVoto ObtenerVotosDocumentosPorID(List<Guid> pListaDocumentosID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.Where(item => pListaDocumentosID.Contains(item.ElementoID)).ToList();

            return votoDW;
        }

        /// <summary>
        /// Obtiene los votos de los comentarios un documento pasado por parámetro
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de voto</returns>
        public DataWrapperVoto ObtenerVotosComentariosDocumentoPorID(Guid pDocumentoID)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            //Voto
            votoDW.ListaVotos = mEntityContext.Voto.JoinVotoComentario().JoinDocumentoComentario().Where(item => item.DocumentoComentario.DocumentoID.Equals(pDocumentoID)).Select(item => item.Voto).ToList();

            return votoDW;
        }

        /// <summary>
        /// Devuelve los votos de los comentarios pasados como parámetro
        /// </summary>
        /// <param name="pListaComentariosID">Lista con los identificadores de los comentarios</param>
        /// <param name="pComentarioDW">Dataset de comentario</param>
        /// <returns>Dataset con los votos de los comentarios</returns>
        public DataWrapperVoto ObtenerVotosComentariosPorComentarioID(List<Guid> pListaComentariosID, DataWrapperComentario pComentarioDW)
        {
            DataWrapperVoto votoDW = new DataWrapperVoto();

            if (pListaComentariosID.Count > 0)
            {
                //voto
                votoDW.ListaVotos = mEntityContext.Voto.JoinVotoComentario().Where(item => pListaComentariosID.Contains(item.VotoComentario.ComentarioID)).Select(item => item.Voto).ToList();

                //votoComentario
                pComentarioDW.ListaVotoComentario = mEntityContext.VotoComentario.Where(item => pListaComentariosID.Contains(item.ComentarioID)).ToList();
            }
            return votoDW;
        }

        public Guid? ObtenerVotosPorUsuario(Guid pDocumentoID, Guid pUsuarioID, Guid pProyectoID)
        {
            //Voto
            return mEntityContext.Voto.JoinVotoDocumento().Where(item => item.Voto.IdentidadID.Equals(pUsuarioID) && item.VotoDocumento.DocumentoID.Equals(pDocumentoID) && item.VotoDocumento.ProyectoID.Value.Equals(pProyectoID)).Select(item => item.Voto.VotoID).FirstOrDefault();
        }


        public void ActualizarVoto(Guid pVotoID, Guid pIdentidadID, float pValorVoto)
        {
            EntityModel.Models.Voto.Voto voto = mEntityContext.Voto.Where(item => item.VotoID.Equals(pVotoID) && item.IdentidadID.Equals(pIdentidadID)).FirstOrDefault();
            voto.Voto1 = pValorVoto;

            ActualizarBaseDeDatosEntityContext();
        }

        public int InsertarVoto(Guid pIdentidadID, Guid pDocumentoID, float pVoto, Guid pIdentidadVotadaID, Guid pVotoID)
        {
            EntityModel.Models.Voto.Voto voto = new EntityModel.Models.Voto.Voto();
            voto.VotoID = pVotoID;
            voto.IdentidadID = pIdentidadID;
            voto.ElementoID = pDocumentoID;
            voto.IdentidadVotadaID = pIdentidadVotadaID;
            voto.Voto1 = pVoto;
            voto.Tipo = 5;
            voto.FechaVotacion = DateTime.Now;

            try
            {
                mEntityContext.Voto.Add(voto);
                ActualizarBaseDeDatosEntityContext();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        public int InsertarVotoDocumento(Guid pDocumentoID, Guid pProyectoID, Guid pVotoID)
        {
            VotoDocumento votoDocumento = new VotoDocumento();
            votoDocumento.DocumentoID = pDocumentoID;
            votoDocumento.ProyectoID = pProyectoID;
            votoDocumento.VotoID = pVotoID;

            try
            {
                mEntityContext.VotoDocumento.Add(votoDocumento);
                ActualizarBaseDeDatosEntityContext();
                return 0;
            }
            catch
            {
                return -1;
            }
        }

        #endregion

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            this.sqlSelectVoto = "SELECT " + IBD.CargarGuid("Voto.VotoID") + ", " + IBD.CargarGuid("Voto.IdentidadID") + ", " + IBD.CargarGuid("Voto.ElementoID") + ", " + IBD.CargarGuid("Voto.IdentidadVotadaID") + ", Voto.Voto, Voto.Tipo, Voto.FechaVotacion FROM Voto";

            this.sqlSelectVotosEntradaBlogPorID = sqlSelectVoto + " INNER JOIN VotoEntradaBlog ON VotoEntradaBlog.VotoID = Voto.VotoID WHERE BlogID =" + IBD.GuidParamValor("blogID") + " AND EntradaBlogID = " + IBD.GuidParamValor("entradaBlogID");

            this.sqlSelectVotosComentariosEntradaBlogPorID = sqlSelectVoto + " INNER JOIN VotoComentario ON VotoComentario.VotoID = Voto.VotoID INNER JOIN ComentarioBlog ON VotoComentario.ComentarioID = ComentarioBlog.ComentarioID WHERE ComentarioBlog.BlogID =" + IBD.GuidParamValor("blogID") + " AND ComentarioBlog.EntradaBlogID = " + IBD.GuidParamValor("entradaBlogID");

            this.sqlSelectVotosTemaForoPorID = sqlSelectVoto + " INNER JOIN VotoMensajeForo ON VotoMensajeForo.VotoID = Voto.VotoID WHERE VotoMensajeForo.ForoID = " + IBD.GuidParamValor("foroID") + " AND VotoMensajeForo.CategoriaForoID = " + IBD.GuidParamValor("categoriaForoID") + " AND VotoMensajeForo.TemaID = " + IBD.GuidParamValor("temaID");

            this.sqlSelectVotosDocumentoPorID = sqlSelectVoto + " INNER JOIN VotoDocumento ON VotoDocumento.VotoID = Voto.VotoID WHERE DocumentoID =" + IBD.GuidParamValor("DocumentoID");

            this.sqlSelectVotoPorIDVoto = sqlSelectVoto + " WHERE Voto.VotoID =" + IBD.GuidParamValor("VotoID");

            this.sqlSelectVotosComentariosDocumentoPorID = sqlSelectVoto + " INNER JOIN VotoComentario ON VotoComentario.VotoID = Voto.VotoID INNER JOIN DocumentoComentario ON VotoComentario.ComentarioID = DocumentoComentario.ComentarioID WHERE DocumentoComentario.DocumentoID =" + IBD.GuidParamValor("DocumentoID");

            this.sqlSelectVotosComentariosFactorPorID = sqlSelectVoto + " INNER JOIN VotoComentario ON VotoComentario.VotoID = Voto.VotoID INNER JOIN ComentarioDafoFactor ON VotoComentario.ComentarioID = ComentarioDafoFactor.ComentarioID WHERE ComentarioDafoFactor.FactorID =" + IBD.GuidParamValor("FactorID");

            #endregion

            #region DataAdapter

            #region Voto

            this.sqlVotoInsert = IBD.ReplaceParam("INSERT INTO Voto (VotoID, IdentidadID, ElementoID, IdentidadVotadaID, Voto, Tipo, FechaVotacion) VALUES (" + IBD.GuidParamColumnaTabla("VotoID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ", " + IBD.GuidParamColumnaTabla("ElementoID") + ", " + IBD.GuidParamColumnaTabla("IdentidadVotadaID") + ", @Voto, @Tipo, @FechaVotacion)");

            this.sqlVotoDelete = IBD.ReplaceParam("DELETE FROM Voto WHERE (VotoID = " + IBD.GuidParamColumnaTabla("O_VotoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (ElementoID = " + IBD.GuidParamColumnaTabla("O_ElementoID") + ") AND (IdentidadVotadaID = " + IBD.GuidParamColumnaTabla("O_IdentidadVotadaID") + ") AND (Voto = @O_Voto) AND (Tipo = @O_Tipo) AND (FechaVotacion = @O_FechaVotacion)");

            this.sqlVotoModify = IBD.ReplaceParam("UPDATE Voto SET VotoID = " + IBD.GuidParamColumnaTabla("VotoID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + ", ElementoID = " + IBD.GuidParamColumnaTabla("ElementoID") + ", IdentidadVotadaID = " + IBD.GuidParamColumnaTabla("IdentidadVotadaID") + ", Voto = @Voto, Tipo = @Tipo, FechaVotacion = @FechaVotacion WHERE (VotoID = " + IBD.GuidParamColumnaTabla("O_VotoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("O_IdentidadID") + ") AND (ElementoID = " + IBD.GuidParamColumnaTabla("O_ElementoID") + ") AND (IdentidadVotadaID = " + IBD.GuidParamColumnaTabla("O_IdentidadVotadaID") + ") AND (Voto = @O_Voto) AND (Tipo = @O_Tipo) AND (FechaVotacion = @O_FechaVotacion)");

            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
