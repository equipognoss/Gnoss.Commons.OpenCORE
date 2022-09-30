using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.RDF.Model;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Es.Riam.Gnoss.AD.RDF
{
    #region Enumeraciones

    /// <summary>
    /// Tipos de objetos que pueden asignarse a una propiedad de una sentencia
    /// </summary>
    public enum TipoObjetoPropiedad
    {
        /// <summary>
        /// Entidad
        /// </summary>
        Entidad = 0,
        /// <summary>
        /// Literal
        /// </summary>
        Literal = 1
    }

    #endregion

    /// <summary>
    /// Clase de acceso a datos para manejar RDFs.
    /// </summary>
    public class RdfAD : BaseAD
    {
        #region Miembros

        /// <summary>
        /// Caracteres de documento.
        /// </summary>
        private string mCaracteresDoc = "";

        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public RdfAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public RdfAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCaracteresDoc">Ultimos caracteres que se añaden a la tabla RdfDocumento</param>
        public RdfAD(string pFicheroConfiguracionBD, string pCaracteresDoc, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mCaracteresDoc = pCaracteresDoc;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas
        private string sqlSelectRdfDocumento;
        #endregion

        #region DataAdapter
        #region RdfDocumento
        private string sqlRdfDocumentoInsert;
        private string sqlRdfDocumentoDelete;
        private string sqlRdfDocumentoModify;
        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Actualiza en base de datos los cambios del dataset de RDFs
        /// </summary>
        /// <param name="pDataSet">Dataset sin tipar</param>
        public void ActualizarBD(RdfDS pDataSet)
        {
            VerificarExisteTabla("RdfDocumento_" + mCaracteresDoc, true);

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

                    #region Eliminar tabla RdfDocumento

                    if (mCaracteresDoc != "")
                    {
                        bool esOracle = (ConexionMaster is OracleConnection);

                        DbCommand DeleteRdfDocumentoCommand = ObtenerComando(sqlRdfDocumentoDelete);
                        AgregarParametro(DeleteRdfDocumentoCommand, IBD.ToParam("Original_DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Original);
                        AgregarParametro(DeleteRdfDocumentoCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);

                        ActualizarBaseDeDatos(deletedDataSet, "RdfDocumento", null, null, DeleteRdfDocumentoCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional, esOracle);
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
        public void GuardarActualizaciones(RdfDS pDataSet)
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
                    #region Actualizar tabla RdfDocumento

                    if (mCaracteresDoc != "")
                    {
                        DbCommand InsertRdfDocumentoCommand = ObtenerComando(sqlRdfDocumentoInsert);
                        AgregarParametro(InsertRdfDocumentoCommand, IBD.ToParam("DocumentoID"), IBD.TipoGuidToObject(DbType.Guid), "DocumentoID", DataRowVersion.Current);
                        AgregarParametro(InsertRdfDocumentoCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                        AgregarParametro(InsertRdfDocumentoCommand, IBD.ToParam("RdfSem"), DbType.String, "RdfSem", DataRowVersion.Current);
                        AgregarParametro(InsertRdfDocumentoCommand, IBD.ToParam("RdfDoc"), DbType.String, "RdfDoc", DataRowVersion.Current);
                        ActualizarBaseDeDatos(addedAndModifiedDataSet, "RdfDocumento", InsertRdfDocumentoCommand, null, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional, esOracle, esPostgre, true, mEntityContextBASE);
                    }

                    #endregion

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }

                //Santi: Aplicar a CorreoADTest_CORE
                RdfDS modificadosDS = (RdfDS)pDataSet.GetChanges(DataRowState.Modified);
                if (modificadosDS != null && modificadosDS.RdfDocumento.Count > 0)
                {
                    string nombreBaseDeDatos = null;
                    if (BaseAD.ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
                    {
                        nombreBaseDeDatos = BaseAD.ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
                    }

                    foreach (RdfDS.RdfDocumentoRow filaDoc in modificadosDS.RdfDocumento)
                    {
                        string update = "";
                        if (!string.IsNullOrEmpty(nombreBaseDeDatos))
                        {
                            update = $"UPDATE \"{nombreBaseDeDatos}\".\"RdfDocumento_{mCaracteresDoc}\" SET \"RdfSem\" = {IBD.ToParam("RdfSem")}, \"RdfDoc\" = {IBD.ToParam("RdfDoc")} WHERE (\"DocumentoID\" = {IBD.FormatearGuid(filaDoc.DocumentoID)} AND \"ProyectoID\" = {IBD.FormatearGuid(filaDoc.ProyectoID)})";
                        }
                        else
                        {
                            update = $"UPDATE \"RdfDocumento_{mCaracteresDoc}\" SET \"RdfSem\" = {IBD.ToParam("RdfSem")}, \"RdfDoc\" = {IBD.ToParam("RdfDoc")} WHERE (\"DocumentoID\" = {IBD.FormatearGuid(filaDoc.DocumentoID)} AND \"ProyectoID\" = {IBD.FormatearGuid(filaDoc.ProyectoID)})";
                        }

                        DbCommand dbCommand = ObtenerComando(update);
                        AgregarParametro(dbCommand, IBD.ToParam("RdfSem"), DbType.String, filaDoc.RdfSem);
                        AgregarParametro(dbCommand, IBD.ToParam("RdfDoc"), DbType.String, filaDoc.RdfDoc);
                        ActualizarBaseDeDatos(dbCommand, true, esOracle);
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
        /// <param name="pProyectoID">Proyecto del documento</param>
        /// <returns>Dataset de RDFs</returns>
        public RdfDS ObtenerRdfPorDocumentoID(Guid pDocumentoID, Guid pProyectoID)
        {
            RdfDS rdfDS = new RdfDS();

            try
            {
                string nombreBaseDeDatos = null;
                if (BaseAD.ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
                {
                    nombreBaseDeDatos = BaseAD.ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
                }

                bool esOracle = (ConexionMaster is OracleConnection);
                string comando;
                if (nombreBaseDeDatos == null)
                {
                    comando = "SELECT " + IBD.CargarGuid("\"DocumentoID\"") + ", " + IBD.CargarGuid("\"ProyectoID\"") + ", \"RdfSem\", \"RdfDoc\" FROM \"RdfDocumento_" + mCaracteresDoc + "\" WHERE \"DocumentoID\"=" + IBD.FormatearGuid(pDocumentoID) + " AND \"ProyectoID\"= " + IBD.FormatearGuid(pProyectoID);
                }
                else
                {
                    comando = "SELECT " + IBD.CargarGuid("\"DocumentoID\"") + ", " + IBD.CargarGuid("\"ProyectoID\"") + ", \"RdfSem\", \"RdfDoc\" FROM \"" + nombreBaseDeDatos + "\".\"RdfDocumento_" + mCaracteresDoc + "\" WHERE \"DocumentoID\"=" + IBD.FormatearGuid(pDocumentoID) + " AND \"ProyectoID\"= " + IBD.FormatearGuid(pProyectoID);
                }


                DbCommand dbCommand = ObtenerComando(comando);

                CargarDataSet(dbCommand, rdfDS, "RdfDocumento", null, esOracle, EsPostgres(), mEntityContextBASE, true);
            }
            catch
            {
                //Falta la tabla
            }

            return rdfDS;
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

                string sql = "Delete from RdfDocumento_" + pNumTabla + " where documentoID in(";
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
                    sql.Append($"Delete from RdfDocumento_{ item.Key} where documentoID ");
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
        /// Borra todos los documentos de las tablas RdfDocumento_XXX
        /// </summary>
        public void VaciarTablasRdf()
        {
            string delete = string.Empty;
            string numTabla = string.Empty;
            List<char> caracteres = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            DbCommand comSql;
            bool esOracle = (ConexionMaster is OracleConnection);
            for (int i = 0; i < caracteres.Count; i++)
            {
                for (int j = 0; j < caracteres.Count; j++)
                {
                    for (int k = 0; k < caracteres.Count; k++)
                    {
                        numTabla = caracteres[i].ToString() + caracteres[j].ToString() + caracteres[k].ToString();
                        if (!esOracle)
                        {
                            delete = $"IF OBJECT_ID('RdfDocumento_{numTabla}', 'U') IS NOT NULL truncate table RdfDocumento_{numTabla}";
                            comSql = ObtenerComando(delete);
                            comSql.CommandTimeout = 300;
                            ActualizarBaseDeDatos(comSql, true, esOracle);
                        }
                        else
                        {
                            try
                            {
                                delete = $"truncate table RdfDocumento_{numTabla}";
                                comSql = ObtenerComando(delete);
                                comSql.CommandTimeout = 300;
                                ActualizarBaseDeDatos(comSql, true, esOracle);
                            }
                            catch (ExcepcionDeBaseDeDatos)
                            {
                                mLoggingService.GuardarLogError($"La tabla RdfDocumento_{numTabla} no existe");
                            }
                        }
                    }
                }
            }
        }
        private void BorrarDocumentos(string pNumTabla, string pDocumentoIN)
        {
            bool esOracle = (ConexionMaster is OracleConnection);

            string sqlDeleteDocumento = "delete from RdfDocumento_" + pNumTabla + " where DocumentoID " + pDocumentoIN;

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

            string numTabla = pDocumentoID.ToString().Substring(0, 3);
            string sql = "";
            if (string.IsNullOrEmpty(nombreBaseDeDatos))
            {
                sql = $"Delete from \"RdfDocumento_{numTabla}\" where \"DocumentoID\" = {IBD.FormatearGuid(pDocumentoID)}";
            }
            else
            {
                sql = $"Delete from \"{nombreBaseDeDatos}\".\"RdfDocumento_{numTabla}\" where \"DocumentoID\" = {IBD.FormatearGuid(pDocumentoID)}";
            }
            DbCommand comandoDelete = ObtenerComando(sql);
            ActualizarBaseDeDatos(comandoDelete, true, esOracle);
        }

        /// <summary>
        /// Borra el documento de los proyectos donde se encuentra compartido
        /// </summary>
        /// <param name="pDocumentoID">Identificador del documento</param>
        public void EliminarDocumentoDeRDFSinTransaccion(Guid pDocumentoID)
        {
            string numTabla = pDocumentoID.ToString().Substring(0, 3);
            string sql = "Delete from RdfDocumento_" + numTabla + " where documentoID = " + IBD.GuidValor(pDocumentoID);
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
            this.sqlSelectRdfDocumento = "SELECT " + IBD.CargarGuid("RdfDocumento.DocumentoID") + ", " + IBD.CargarGuid("RdfDocumento.ProyectoID") + ", RdfDocumento.RdfSem, RdfDocumento.RdfDoc FROM RdfDocumento";

            string nombreBaseDeDatos = null;
            if (BaseAD.ListaDefaultSchemaPorConexion.ContainsKey(Conexion.ConnectionString))
            {
                nombreBaseDeDatos = BaseAD.ListaDefaultSchemaPorConexion[Conexion.ConnectionString];
            }

            if (nombreBaseDeDatos == null)
            {
                this.sqlRdfDocumentoInsert = IBD.ReplaceParam("INSERT INTO \"RdfDocumento_" + mCaracteresDoc + "\" (\"DocumentoID\", \"ProyectoID\", \"RdfSem\", \"RdfDoc\") VALUES (" + IBD.GuidParamColumnaTabla("DocumentoID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @RdfSem, @RdfDoc)");
                this.sqlRdfDocumentoDelete = IBD.ReplaceParam("DELETE FROM \"RdfDocumento_" + mCaracteresDoc + "\" WHERE (\"DocumentoID\" = " + IBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (\"ProyectoID\" = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")");
                this.sqlRdfDocumentoModify = IBD.ReplaceParam("UPDATE \"RdfDocumento_" + mCaracteresDoc + "\" SET \"RdfSem\" = @RdfSem, \"RdfDoc\" = @RdfDoc WHERE (\"DocumentoID\" = '" + IBD.GuidParamColumnaTabla("ODocumentoID") + "') AND (\"ProyectoID\" = '" + IBD.GuidParamColumnaTabla("OProyectoID") + "')");
            }
            else
            {
                this.sqlRdfDocumentoInsert = IBD.ReplaceParam("INSERT INTO \"" + nombreBaseDeDatos + "\".\"RdfDocumento_" + mCaracteresDoc + "\" (\"DocumentoID\", \"ProyectoID\", \"RdfSem\", \"RdfDoc\") VALUES (" + IBD.GuidParamColumnaTabla("DocumentoID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @RdfSem, @RdfDoc)");
                this.sqlRdfDocumentoDelete = IBD.ReplaceParam("DELETE FROM \"" + nombreBaseDeDatos + "\".\"RdfDocumento_" + mCaracteresDoc + "\" WHERE (\"DocumentoID\" = " + IBD.GuidParamColumnaTabla("Original_DocumentoID") + ") AND (\"ProyectoID\" = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ")");
                this.sqlRdfDocumentoModify = IBD.ReplaceParam("UPDATE  \"" + nombreBaseDeDatos + "\".\"RdfDocumento_" + mCaracteresDoc + "\" SET \"RdfSem\" = @RdfSem, \"RdfDoc\" = @RdfDoc WHERE (\"DocumentoID\" = '" + IBD.GuidParamColumnaTabla("ODocumentoID") + "') AND (\"ProyectoID\" = '" + IBD.GuidParamColumnaTabla("OProyectoID") + "')");
            }
        }

        #endregion

        #region Verificación de existencia y creación de tablas

        /// <summary>
        /// Comprueba si existe la tabla de RdfDocumento. Si no existe y se indica, la crea.
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla</param>
        /// <param name="pCrearTablaSiNoExiste">TRUE si se debe crear la tabla en caso de que no exista</param>
        /// <returns>TRUE si la tabla existe (o ha sido recién creada).</returns>
        private bool VerificarExisteTabla(string pNombreTabla, bool pCrearTablaSiNoExiste)
        {
            bool existeTabla = VerificarExisteTabla(pNombreTabla);

            if (!existeTabla && pCrearTablaSiNoExiste)
            {
                if (ConexionMaster is OracleConnection)
                {
                    CrearTablaOracle(pNombreTabla);
                }
                else if(ConexionMaster is NpgsqlConnection)
                {
                    CrearTablaPostgre(pNombreTabla);
                }
                else
                {
                    CrearTablaSQL(pNombreTabla);
                }
                existeTabla = true;
            }
            return existeTabla;
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
                existeTabla = "SELECT 1 FROM all_tables WHERE TABLE_NAME = " + IBD.ToParam("nombreTabla");
            }

            DbCommand cmdExisteTabla = ObtenerComando(existeTabla);
            AgregarParametro(cmdExisteTabla, IBD.ToParam("nombreTabla"), DbType.String, pNombreTabla);

            object resultado = EjecutarEscalar(cmdExisteTabla, true);
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
            if (pNombreTabla.Contains("RdfDocumento_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE {pNombreTabla} ([DocumentoID] [uniqueidentifier] NOT NULL, [ProyectoID] [uniqueidentifier] NOT NULL, [RdfSem] [nvarchar](max) NULL, [RdfDoc] [nvarchar](max) NULL, CONSTRAINT [PK_{pNombreTabla}] PRIMARY KEY CLUSTERED ([DocumentoID] ASC, [ProyectoID] ASC)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY]");

                ActualizarBaseDeDatos(cmdCrearTabla);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pNombreTabla"></param>
        public void CrearTablaOracle(string pNombreTabla)
        {
            if (pNombreTabla.Contains("RdfDocumento_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\" (\"DocumentoID\" RAW(16) NOT NULL, \"ProyectoID\" RAW(16) NOT NULL, \"RdfSem\" NCLOB NULL, \"RdfDoc\" NCLOB NULL, PRIMARY KEY(\"DocumentoID\", \"ProyectoID\"))");

                ActualizarBaseDeDatos(cmdCrearTabla, true, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pNombreTabla"></param>
        public void CrearTablaPostgre(string pNombreTabla)
        {
            if (pNombreTabla.Contains("RdfDocumento_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\" (\"DocumentoID\" UUID NOT NULL, \"ProyectoID\" UUID NOT NULL, \"RdfSem\" VARCHAR, \"RdfDoc\" VARCHAR, PRIMARY KEY (\"DocumentoID\", \"ProyectoID\"))");

                ActualizarBaseDeDatos(cmdCrearTabla, true, true, true);
                TerminarTransaccion(true);
            }
        }

        #endregion

        #endregion
    }
}
