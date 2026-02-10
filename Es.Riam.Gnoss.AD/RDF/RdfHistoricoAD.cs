using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Es.Riam.Gnoss.AD.RDF
{

    /// <summary>
    /// Clase de acceso a datos para manejar RDFs.
    /// </summary>
    public class RdfHistoricoAD : BaseAD
    {
        #region Miembros

        /// <summary>
        /// Caracteres de documento.
        /// </summary>
        private string mCaracteresDoc = "";
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public RdfHistoricoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mloggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        public RdfHistoricoAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mloggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }
        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        public RdfHistoricoAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mloggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }
        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCaracteresDoc">Ultimos caracteres que se añaden a la tabla RdfHistorico</param>
        public RdfHistoricoAD(string pFicheroConfiguracionBD, string pCaracteresDoc, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mloggerFactory = loggerFactory;
            mCaracteresDoc = pCaracteresDoc;
            this.CargarConsultasYDataAdapters(IBD);
        }

        public RdfHistoricoAD(string pFicheroConfiguracionBD, string pCaracteresDoc, LoggingService loggingService, EntityContext entityContext, EntityContextBASE entityContextBASE, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<RdfHistoricoAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mloggerFactory = loggerFactory;
            mCaracteresDoc = pCaracteresDoc;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas
        private string sqlSelectRdfHistorico;
        #endregion

        #region DataAdapter
        #region RdfHistorico
        private string sqlRdfHistoricoInsert;
        private string sqlRdfHistoricoDelete;
        private string sqlRdfHistoricoModify;
        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Actualiza en base de datos los cambios del dataset de RDFs
        /// </summary>
        /// <param name="pDataSet">Dataset sin tipar</param>
        public void ActualizarBD(RdfHistoricoDS pDataSet)
        {
            VerificarExisteTabla("RdfHistorico_" + mCaracteresDoc, true);
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);

            pDataSet.AcceptChanges();
        }

        /// <summary>
        /// Elimina datos de RDFs
        /// </summary>
        /// <param name="pDataSet">Dataset sin tipar</param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted

                    #region Eliminar tabla RdfHistorico

                    if (mCaracteresDoc != "")
                    {
                        bool esOracle = (ConexionMaster is OracleConnection);
                        bool esPostgre = EsPostgres();

                        DbCommand DeleteRdfHistoricoCommand = ObtenerComando(sqlRdfHistoricoDelete);
                        AgregarParametro(DeleteRdfHistoricoCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);

                        ActualizarBaseDeDatos(deletedDataSet, "RdfHistorico", null, null, DeleteRdfHistoricoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional, esOracle, esPostgre);
                    }

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
        /// Guarda en base de datos las modificaciones de RDFs
        /// </summary>
        /// <param name="pDataSet">Dataset sin tipar</param>
        public void GuardarActualizaciones(RdfHistoricoDS pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added);

                bool esOracle = (ConexionMaster is OracleConnection);
                bool esPostgre = EsPostgres();

                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla RdfHistorico

                    if (mCaracteresDoc != "")
                    {
                        DbCommand InsertRdfHistoricoCommand = ObtenerComando(sqlRdfHistoricoInsert);
                        AgregarParametro(InsertRdfHistoricoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                        AgregarParametro(InsertRdfHistoricoCommand, IBD.ToParam("RdfSem"), DbType.String, "RdfSem", DataRowVersion.Current);
                        AgregarParametro(InsertRdfHistoricoCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), "IdentidadID", DataRowVersion.Current);
                        AgregarParametro(InsertRdfHistoricoCommand, IBD.ToParam("FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Current);
                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "RdfHistorico", InsertRdfHistoricoCommand, null, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional, esOracle, esPostgre, true, mEntityContextBASE);
                    }

                    #endregion

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }

                RdfHistoricoDS modificadosDS = (RdfHistoricoDS)pDataSet.GetChanges(DataRowState.Modified);
                if (modificadosDS != null && modificadosDS.RdfHistorico.Count > 0)
                {
                    string nombreBaseDeDatos = null;
                    if (ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
                    {
                        nombreBaseDeDatos = ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
                    }

                    foreach (RdfHistoricoDS.RdfHistoricoRow filaDoc in modificadosDS.RdfHistorico)
                    {
                        string update = "";
                        if (!string.IsNullOrEmpty(nombreBaseDeDatos))
                        {
                            update = $"UPDATE \"{nombreBaseDeDatos}\".\"RdfHistorico_{mCaracteresDoc}\" SET \"RdfSem\" = {IBD.ToParam("RdfSem")}, \"IdentidadID\" = {IBD.ToParam("IdentidadID")}, \"FechaModificacion\" = {IBD.ToParam("FechaModificacion")} WHERE (\"DocumentoID\" = {IBD.FormatearGuid(filaDoc.DocumentoID)})";
                        }
                        else
                        {
                            update = $"UPDATE \"RdfHistorico_{mCaracteresDoc}\" SET \"RdfSem\" = {IBD.ToParam("RdfSem")}, \"IdentidadID\" = {IBD.ToParam("IdentidadID")}, \"FechaModificacion\" = {IBD.ToParam("FechaModificacion")} WHERE (\"DocumentoID\" = {IBD.FormatearGuid(filaDoc.DocumentoID)})";
                        }

                        DbCommand dbCommand = ObtenerComando(update);
                        AgregarParametro(dbCommand, IBD.ToParam("RdfSem"), DbType.String, filaDoc.RdfSem);
                        AgregarParametro(dbCommand, IBD.ToParam("IdentidadID"), IBD.TipoGuidToObject(DbType.Guid), filaDoc.IdentidadID);
                        AgregarParametro(dbCommand, IBD.ToParam("FechaModificacion"), DbType.DateTime, filaDoc.FechaModificacion);
                        ActualizarBaseDeDatos(dbCommand, true, esOracle, esPostgre, mEntityContextBASE);
                    }

                    modificadosDS.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Devuelve el RDF del documento solicitado.
        /// </summary>
        /// <param name="pDocumentoID">Identificador de documento</param>
        /// <returns>Dataset de RDFs</returns>
        public RdfHistoricoDS ObtenerRdfPorDocumentoID(Guid pDocumentoID)
        {
            RdfHistoricoDS rdfHistoricoDS = new RdfHistoricoDS();

            try
            {
                string nombreBaseDeDatos = null;
                if (ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
                {
                    nombreBaseDeDatos = ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
                }

                bool esOracle = (ConexionMaster is OracleConnection);
                string comando;
                if (nombreBaseDeDatos == null)
                {
                    comando = "SELECT " + IBD.CargarGuid("\"DocumentoID\"") + ", \"RdfSem\", " + IBD.CargarGuid("\"IdentidadID\"") + ", \"FechaModificacion\" FROM \"RdfHistorico_" + mCaracteresDoc + "\" WHERE \"DocumentoID\"=" + IBD.FormatearGuid(pDocumentoID);
                }
                else
                {
                    comando = "SELECT " + IBD.CargarGuid("\"DocumentoID\"") + ", \"RdfSem\", " + IBD.CargarGuid("\"IdentidadID\"") + ", \"FechaModificacion\" FROM \"" + nombreBaseDeDatos + "\".\"RdfHistorico_" + mCaracteresDoc + "\" WHERE \"DocumentoID\"=" + IBD.FormatearGuid(pDocumentoID);
                }


                DbCommand dbCommand = ObtenerComando(comando);

                CargarDataSet(dbCommand, rdfHistoricoDS, "RdfHistorico", null, esOracle, EsPostgres(), mEntityContextBASE);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"No se ha encontrado la tabla RdfHistorico_{mCaracteresDoc}");
            }

            return rdfHistoricoDS;
        }

        /// <summary>
        /// Borra los documentos de la lista de los proyectos donde se encuentran compartidos
        /// </summary>
        /// <param name="pNumTabla">Tres primeros dígitos identificativos de la tabla a la que pertenecen los documentos</param>
        /// <param name="pDocumentosID">Lista de documentos a borrar</param>
        public void EliminarDocumentosDeRDF(string pNumTabla, List<Guid> pDocumentosID)
        {
            if (pDocumentosID.Count > 0)
            {

                string sql = "Delete from RdfHistorico_" + pNumTabla + " where DocumentoID in(";
                int contador = 0;
                string documentoIN = "IN (";

                foreach (Guid docID in pDocumentosID)
                {
                    if (contador < 100)
                    {
                        documentoIN += IBD.GuidValor(docID) + ", ";
                        contador++;
                    }
                    else
                    {
                        documentoIN += documentoIN.Substring(0, documentoIN.LastIndexOf(",")) + ")";
                        BorrarDocumentos(pNumTabla, documentoIN);

                        //hay que seguir añadiendo del 100 en adelante
                        documentoIN = " IN (";
                        contador = 0;
                        documentoIN += IBD.GuidValor(docID) + ", ";
                        contador++;
                    }
                }

                if (contador > 0)
                {
                    documentoIN = documentoIN.Substring(0, documentoIN.LastIndexOf(",")) + ")";
                    BorrarDocumentos(pNumTabla, documentoIN);
                }
            }
        }

        /// <summary>
        /// Borra los documentos de la lista de los proyectos donde se encuentran compartidos
        /// </summary>
        /// <param name="pNumTabla">Tres primeros dígitos identificativos de la tabla a la que pertenecen los documentos</param>
        /// <param name="pDocumentosID">Lista de documentos a borrar</param>
        public void EliminarDocumentosDeRDF(List<Guid> pDocumentosID)
        {
            if (pDocumentosID.Count > 0)
            {
                bool esOracle = (ConexionMaster is OracleConnection);

                StringBuilder sql = new StringBuilder();
                int contador = 0;

                var deletes = pDocumentosID.GroupBy(id => id.ToString().Substring(0, 3));

                foreach (var item in deletes)
                {
                    sql.Append($"Delete from RdfHistorico_{item.Key} where DocumentoID ");
                    if (item.Count() > 1)
                    {
                        sql.Append("IN (");
                        string coma = "";
                        foreach (Guid docID in item)
                        {
                            sql.Append($"{coma} {IBD.GuidValor(docID)}");
                            coma = ", ";
                        }
                        sql.Append(") ");
                    }
                    else
                    {
                        sql.Append($"= {IBD.GuidValor(item.First())}");
                    }
                    sql.AppendLine();
                    if (contador < 10)
                    {
                        contador++;
                    }
                    else
                    {
                        ActualizarBaseDeDatos(ObtenerComando(sql.ToString()), true, esOracle);

                        //hay que seguir añadiendo del 10 en adelante
                        contador = 0;
                        sql.Clear();
                    }
                }

                if (contador > 0)
                {
                    ActualizarBaseDeDatos(ObtenerComando(sql.ToString()), true, esOracle);
                }
            }
        }

        /// <summary>
        /// Borra todos los documentos de las tablas RdfHistorico_XX
        /// </summary>
        public void VaciarTablasRdf()
        {
            string delete;
            string numTabla;
            List<char> caracteres = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
            DbCommand comSql;

            for (int i = 0; i < caracteres.Count; i++)
            {
                for (int j = 0; j < caracteres.Count; j++)
                {
                    numTabla = caracteres[i].ToString() + caracteres[j].ToString();

                    try
                    {
                        delete = $"truncate table \"RdfHistorico{numTabla}\"";
                        comSql = ObtenerComando(delete);
                        comSql.CommandTimeout = 300;
                        ActualizarBaseDeDatos(comSql, false, true, true, mEntityContextBASE);
                    }
                    catch
                    {
                        //La tabla RdfHistorico indicada no existe
                    }

                }
            }
        }

        private void BorrarDocumentos(string pNumTabla, string pDocumentoIN)
        {
            bool esOracle = (ConexionMaster is OracleConnection);

            string sqlDeleteDocumento = "delete from RdfHistorico_" + pNumTabla + " where DocumentoID " + pDocumentoIN;

            DbCommand comsqlDeleteDocumento = ObtenerComando(sqlDeleteDocumento);
            ActualizarBaseDeDatos(comsqlDeleteDocumento, true, esOracle);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDF(Guid pDocumentoID)
        {
            bool esOracle = (ConexionMaster is OracleConnection);

            string nombreBaseDeDatos = null;
            if (BaseAD.ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
            {
                nombreBaseDeDatos = BaseAD.ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
            }

            string numTabla = pDocumentoID.ToString().Substring(0, 2);
            string sql = "";
            if (string.IsNullOrEmpty(nombreBaseDeDatos))
            {
                sql = $"Delete from \"RdfHistorico_{numTabla}\" where \"DocumentoID\" = {IBD.FormatearGuid(pDocumentoID)}";
            }
            else
            {
                sql = $"Delete from \"{nombreBaseDeDatos}\".\"RdfHistorico_{numTabla}\" where \"DocumentoID\" = {IBD.FormatearGuid(pDocumentoID)}";
            }
            DbCommand comandoDelete = ObtenerComando(sql);
            ActualizarBaseDeDatos(comandoDelete, true, true, true, mEntityContextBASE);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDFSinTransaccion(Guid pDocumentoID)
        {
            string numTabla = pDocumentoID.ToString().Substring(0, 2);
            string sql = "Delete from RdfHistorico_" + numTabla + " where DocumentoID = " + IBD.GuidValor(pDocumentoID);
            DbCommand comandoDelete = ObtenerComando(sql);
            ActualizarBaseDeDatos(comandoDelete, false);
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
            this.sqlSelectRdfHistorico = "SELECT " + IBD.CargarGuid("RdfHistorico.DocumentoID") + ", RdfHistorico.RdfSem, RdfHistorico.IdentidadID, RdfHistorico.FechaModificacion FROM RdfHistorico";

            string nombreBaseDeDatos = null;
            if (ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
            {
                nombreBaseDeDatos = ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
            }

            if (nombreBaseDeDatos == null)
            {
                this.sqlRdfHistoricoInsert = IBD.ReplaceParam("INSERT INTO \"RdfHistorico_" + mCaracteresDoc + "\" (\"DocumentoID\", \"RdfSem\", \"IdentidadID\", \"FechaModificacion\") VALUES (" + IBD.GuidParamColumnaTabla("DocumentoID") + ", @RdfSem, " + IBD.GuidParamColumnaTabla("IdentidadID") + ", @FechaModificacion)");

                this.sqlRdfHistoricoDelete = IBD.ReplaceParam("DELETE FROM \"RdfHistorico_" + mCaracteresDoc + "\" WHERE (\"DocumentoID\" = " + IBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");
                this.sqlRdfHistoricoModify = IBD.ReplaceParam("UPDATE \"RdfHistorico_" + mCaracteresDoc + "\" SET \"RdfSem\" = @RdfSem WHERE (\"DocumentoID\" = '" + IBD.GuidParamColumnaTabla("ODocumentoID") + "')");
            }
            else
            {
                this.sqlRdfHistoricoInsert = IBD.ReplaceParam("INSERT INTO \"" + nombreBaseDeDatos + "\".\"RdfHistorico_" + mCaracteresDoc + "\" (\"DocumentoID\", \"RdfSem\", \"IdentidadID\", \"FechaModificacion\") VALUES (" + IBD.GuidParamColumnaTabla("DocumentoID") + ", @RdfSem, \"+ IBD.GuidParamColumnaTabla(\"IdentidadID\") + \", @FechaModificacion)");
                this.sqlRdfHistoricoDelete = IBD.ReplaceParam("DELETE FROM \"" + nombreBaseDeDatos + "\".\"RdfHistorico_" + mCaracteresDoc + "\" WHERE (\"DocumentoID\" = " + IBD.GuidParamColumnaTabla("Original_DocumentoID") + ")");
                this.sqlRdfHistoricoModify = IBD.ReplaceParam("UPDATE  \"" + nombreBaseDeDatos + "\".\"RdfHistorico_" + mCaracteresDoc + "\" SET \"RdfSem\" = @RdfSem WHERE (\"DocumentoID\" = '" + IBD.GuidParamColumnaTabla("ODocumentoID") + "')");
            }
        }

        #endregion

        #region Verificación de existencia y creación de tablas

        /// <summary>
        /// Comprueba si existe la tabla de RdfHistorico. Si no existe y se indica, la crea.
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla</param>
        /// <param name="pCrearTablaSiNoExiste">TRUE si se debe crear la tabla en caso de que no exista</param>
        private void VerificarExisteTabla(string pNombreTabla, bool pCrearTablaSiNoExiste)
        {
            bool existeTabla = VerificarExisteTabla(pNombreTabla);

            if (!existeTabla && pCrearTablaSiNoExiste)
            {
                if (ConexionMaster is OracleConnection)
                {
                    CrearTablaOracle(pNombreTabla);
                }
                else if (ConexionMaster is NpgsqlConnection)
                {
                    CrearTablaPostgre(pNombreTabla);
                }
                else
                {
                    CrearTablaSQL(pNombreTabla);
                }
            }
        }

        /// <summary>
        /// Comprueba si existen las tablas sobre las que está configurado este AD. Si no existen las crea. 
        /// </summary>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public bool VerificarExisteTabla(string pNombreTabla)
        {
            string existeTabla = "SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = " + IBD.ToParam("nombreTabla");
            if (ConexionMaster is OracleConnection)
            {
                existeTabla = "SELECT 1 FROM all_tables WHERE TABLE_NAME = " + IBD.ToParam("nombreTabla") + " AND OWNER = " + IBD.ToParam("owner");
            }

            DbCommand cmdExisteTabla = ObtenerComando(existeTabla);
            AgregarParametro(cmdExisteTabla, IBD.ToParam("nombreTabla"), DbType.String, pNombreTabla);

            if (ConexionMaster is OracleConnection)
            {
                OracleConnectionStringBuilder stringBuilder = new OracleConnectionStringBuilder(ConexionMaster.ConnectionString);
                string userID = stringBuilder.UserID;
                AgregarParametro(cmdExisteTabla, IBD.ToParam("owner"), DbType.String, userID);
            }

            object resultado = EjecutarEscalar(cmdExisteTabla, true, true, mEntityContextBASE);
            int resultadoOracle = 0;
            try
            {
                resultadoOracle = Convert.ToInt32(resultado);
            }
            catch (Exception ex)
            {
                resultadoOracle = 0;
            }

            return ((resultado != null) && (resultado is int) && (((int)resultado).Equals(1))) || resultadoOracle == 1;
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de conuslta
        /// </summary>
        public void CrearTablaSQL(string pNombreTabla)
        {
            if (pNombreTabla.Contains("RdfHistorico_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE {pNombreTabla} ([DocumentoID] [uniqueidentifier] NOT NULL, [RdfSem] [nvarchar](max) NULL, [IdentidadID] [uniqueidentifier] NOT NULL, [FechaModificacion] [datetime] NULL, CONSTRAINT [PK_{pNombreTabla}] PRIMARY KEY ([DocumentoID])) ON [PRIMARY]");

                ActualizarBaseDeDatos(cmdCrearTabla, true, false, false, mEntityContextBASE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pNombreTabla"></param>
        public void CrearTablaOracle(string pNombreTabla)
        {
            if (pNombreTabla.Contains("RdfHistorico_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\" (\"DocumentoID\" RAW(16) NOT NULL, \"RdfSem\" NCLOB NULL, \"IdentidadID\" RAW(16) NOT NULL, \"FechaModificacion\" TIMESTAMP, PRIMARY KEY(\"DocumentoID\"))");

                ActualizarBaseDeDatos(cmdCrearTabla, true, true, false, mEntityContextBASE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pNombreTabla"></param>
        public void CrearTablaPostgre(string pNombreTabla)
        {
            if (pNombreTabla.Contains("RdfHistorico_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\" (\"DocumentoID\" UUID NOT NULL, \"RdfSem\" VARCHAR, \"IdentidadID\" UUID NOT NULL, \"FechaModificacion\" TIMESTAMP,  PRIMARY KEY (\"DocumentoID\"))");

                ActualizarBaseDeDatos(cmdCrearTabla, true, false, true, mEntityContextBASE);
                TerminarTransaccion(true);
            }
        }

        #endregion

        #endregion
    }
}
