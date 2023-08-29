using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Npgsql;
using System.Net;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.AbstractsOpen;

namespace Es.Riam.Gnoss.AD
{
    /// <summary>
    /// Data adapter para base de datos
    /// </summary>
    public abstract class BaseAD : IDisposable
    {
        #region Miembros
        //public static bool Virtuoso_HA_Proxy = false;

        public static Dictionary<string, List<string>> mListaPools = new Dictionary<string, List<string>>();

        /// <summary>
        /// Base de datos de Enterprise
        /// </summary>
        private Microsoft.Practices.EnterpriseLibrary.Data.Database mBaseDatos;

        /// <summary>
        /// Base de datos de Enterprise Master (accede a la base de datos de escritura)
        /// </summary>
        private Microsoft.Practices.EnterpriseLibrary.Data.Database mBaseDatosMaster;

        /// <summary>
        /// Interfaz de base de datos (variable miembro, si se usan hilos no se usa la variable estática)
        /// </summary>
        private IBaseDatos mIBD;

        /// <summary>
        /// Contador para no repetir el número de contador
        /// </summary>
        private int mNumparametroNoRepetido = 0;

        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        /// <summary>
        /// Verdad si se deben consultar la base de datos Master (por defecto false, se consulta las bases de datos replicadas)
        /// </summary>
        private bool mUsarMasterParaLectura = false;

        /// <summary>
        /// Almacena la lista de parámetros para cada comando
        /// </summary>
        private Dictionary<DbCommand, List<ParametroComando>> mListaParametros = new Dictionary<DbCommand, List<ParametroComando>>();

        private static string[] mArrayErroresPermitidos = { "quedó en interbloqueo", "was deadlocked on lock", "Error relacionado con la red o específico", "network-related or instance-specific error occurred", "requires an open and available Connection", "requiere una conexión abierta y disponible" };

        private static List<string> mListaErroresPermitidos;

        protected LoggingService mLoggingService;
        protected EntityContext mEntityContext;
        protected readonly ConfigService mConfigService;
        protected EntityContextBASE mEntityContextBASE;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public BaseAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            Cargar();
        }

        public BaseAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mEntityContextBASE = entityContextBASE;
            Cargar();
        }

        /// <summary>
        /// Constructor a partir de la ruta del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>

        public BaseAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            if (mServicesUtilVirtuosoAndReplication != null)
            {
                mServicesUtilVirtuosoAndReplication.FicheroConfiguracion = pFicheroConfiguracionBD;
            }
            Cargar(pFicheroConfiguracionBD);
        }

        public BaseAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication.FicheroConfiguracion = pFicheroConfiguracionBD;
            mEntityContextBASE = entityContextBASE;
            Cargar(pFicheroConfiguracionBD);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene un interfaz de base de datos a partir de la ruta del fichero de configuración de conexión a base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuracion de la conexión a base de datos</param>
        /// <returns>Interfaz de base de datos</returns>
        [Obsolete("El metodo ObtenerBaseDatos desaparecera en futras versiones. Usar la variable IBD en su lugar")]
        protected IBaseDatos ObtenerBaseDatos(string pFicheroConfiguracionBD)
        {
            if (mIBD == null)
            {
                mIBD = new BDSqlServer(mConfigService);
            }
            return mIBD;
        }

        /// <summary>
        /// Carga el AD
        /// </summary>
        protected void Cargar()
        {
            mBaseDatos = IBD.CrearBDEnterprise();
        }

        /// <summary>
        /// Carga el AD
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la conexión a base de datos</param>
        protected void Cargar(string pFicheroConfiguracionBD)
        {
            mBaseDatos = IBD.CrearBDEnterprise(pFicheroConfiguracionBD);
        }

        /// <summary>
        /// Obtiene el nombre de la base de datos
        /// </summary>
        /// <returns>Nombre de la base de datos</returns>
        public string ObtenerNombreBaseDatos()
        {
            string nombreBD = mBaseDatos.ConnectionStringWithoutCredentials.Substring(BaseDatos.ConnectionStringWithoutCredentials.IndexOf("database="));
            nombreBD = nombreBD.Substring(0, nombreBD.IndexOf(";"));

            return nombreBD;
        }

        public bool EsOracle()
        {
            string tipoBD = mConfigService.ObtenerTipoBD();

            if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("1"))
            {
                return true;
            }
            return false;
        }

        public bool EsPostgres()
        {
            string tipoBD = mConfigService.ObtenerTipoBD();

            if (!string.IsNullOrEmpty(tipoBD) && tipoBD.Equals("2"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Devuelve el nombre de un parámetro asegurando que no sea repetido
        /// </summary>
        /// <returns>Nombre de parámetro</returns>
        public string NombreParamNoRepetido()
        {
            mNumparametroNoRepetido++;
            return "param" + mNumparametroNoRepetido.ToString();
        }

        /// <summary>
        /// Genera un script con las inserciones y actualizaciones necesarias para actualizar la base de datos
        /// </summary>
        /// <param name="pDataSet">DataSet con los cambios</param>
        /// <returns>Devuelve una cadena con los errores ocurridos</returns>
        public virtual string GuardarActualizacionInsercionMedianteScript(DataSet pDataSet, bool pLanzarExcepciones)
        {
            List<DbCommand> comandosPendientes = new List<DbCommand>();

            pDataSet = pDataSet.GetChanges();
            int numParamInsert = 0;
            int numParamUpdate = 0;
            if (pDataSet != null)
            {
                List<DataTable> listaTablas = ObtenerOrdenTablas(pDataSet);
                foreach (DataTable tabla in listaTablas)
                {
                    string nombreTabla = tabla.TableName;
                    if (tabla.Rows.Count > 0)
                    {
                        foreach (DataRow fila in tabla.Rows)
                        {
                            try
                            {
                                if (nombreTabla.StartsWith("CorreoInterno"))
                                {
                                    if ((Guid)fila["Destinatario"] == Guid.Empty)
                                    {
                                        //Enviado
                                        Guid emisor = (Guid)fila["Autor"];
                                        nombreTabla = "CorreoInterno_" + emisor.ToString().Substring(0, 2);
                                    }
                                    else
                                    {
                                        //Recibido
                                        Guid receptor = (Guid)fila["Destinatario"];
                                        nombreTabla = "CorreoInterno_" + receptor.ToString().Substring(0, 2);
                                    }
                                }

                                DbCommand comandoInsert = ObtenerComando("SELECT *");
                                DbCommand comandoUpdate = ObtenerComando("SELECT *");
                                comandoInsert.CommandText = "";
                                comandoUpdate.CommandText = "";

                                if (fila.RowState.Equals(DataRowState.Added))
                                {
                                    //genero insert
                                    comandoInsert.CommandText += System.Environment.NewLine + "INSERT INTO " + nombreTabla + " ";
                                    string columnas = "(";
                                    string coma = "";
                                    string valores = " VALUES (";
                                    foreach (DataColumn columna in tabla.Columns)
                                    {
                                        if (!fila.IsNull(columna) && !columna.AutoIncrement)
                                        {
                                            columnas += coma + columna.ColumnName;
                                            if ((columna.DataType.Equals(typeof(string))) || (columna.DataType.Equals(typeof(Guid))) || (columna.DataType.Equals(typeof(DateTime))) || (columna.DataType.Equals(typeof(bool))) || columna.DataType.Equals(typeof(Byte[])))
                                            {
                                                //con parámetro
                                                string param = IBD.ToParam("columna" + numParamInsert++);
                                                valores += coma + param;
                                                AgregarParametro(comandoInsert, param, TypeToDbType(columna.DataType), fila[columna]);
                                            }
                                            else
                                            {
                                                //no hace falta parámetro
                                                valores += coma + fila[columna];
                                            }
                                            coma = ", ";
                                        }
                                    }

                                    comandoInsert.CommandText += columnas + ")" + System.Environment.NewLine + valores + ")";

                                    //Hay problemas por el orden de las
                                    try
                                    {
                                        //Si no puede ejecutar la consulta la guardo en una lista de consultas pendientes.
                                        ActualizarBaseDeDatos(comandoInsert);
                                    }
                                    catch (Exception)
                                    {
                                        comandosPendientes.Add(comandoInsert);
                                        if (pLanzarExcepciones)
                                        {
                                            throw;
                                        }
                                    }
                                    finally
                                    {
                                        List<DbCommand> copiaComandosPendientes = new List<DbCommand>();
                                        foreach (DbCommand comandoPendiente in comandosPendientes)
                                        {
                                            copiaComandosPendientes.Add(comandoPendiente);
                                        }
                                        foreach (DbCommand comandoPendiente in copiaComandosPendientes)
                                        {
                                            if (comandoPendiente != comandoInsert)
                                            {
                                                try
                                                {
                                                    ActualizarBaseDeDatos(comandoPendiente);
                                                    comandosPendientes.Remove(comandoPendiente);
                                                }
                                                catch (Exception e)
                                                {
                                                    mLoggingService.GuardarLogError(e, null, true);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (fila.RowState.Equals(DataRowState.Modified))
                                {
                                    //genero update
                                    comandoUpdate.CommandText += System.Environment.NewLine + "UPDATE " + nombreTabla + System.Environment.NewLine;
                                    string where = " WHERE ";
                                    string coma = "";
                                    string and = "";
                                    string valores = " SET ";
                                    foreach (DataColumn columna in tabla.Columns)
                                    {
                                        where += and + columna.ColumnName;
                                        if (!columna.AutoIncrement)
                                        {
                                            valores += coma + columna.ColumnName + " = ";
                                            coma = ", ";
                                        }
                                        if ((columna.DataType.Equals(typeof(string))) || (columna.DataType.Equals(typeof(Guid))) || (columna.DataType.Equals(typeof(DateTime))) || (columna.DataType.Equals(typeof(bool))))
                                        {
                                            //con parámetro
                                            string param = IBD.ToParam("columna" + numParamUpdate++);

                                            if (!columna.AutoIncrement)
                                            {
                                                valores += param;
                                                AgregarParametro(comandoUpdate, param, TypeToDbType(columna.DataType), fila[columna]);
                                            }

                                            if (fila.IsNull(columna, DataRowVersion.Original))
                                            {
                                                where += " IS NULL ";
                                            }
                                            else
                                            {
                                                where += " = " + param + "O";
                                                AgregarParametro(comandoUpdate, param + "O", TypeToDbType(columna.DataType), fila[columna, DataRowVersion.Original]);
                                            }

                                        }
                                        else
                                        {
                                            //no hace falta parámetro
                                            if (!columna.AutoIncrement)
                                            {
                                                valores += fila[columna];
                                            }
                                            if (fila.IsNull(columna, DataRowVersion.Original))
                                            {
                                                where += " IS NULL ";
                                            }
                                            else
                                            {
                                                where += " = " + fila[columna, DataRowVersion.Original];
                                            }
                                        }
                                        and = " AND ";
                                    }

                                    comandoUpdate.CommandText += valores + System.Environment.NewLine + where;

                                    ActualizarBaseDeDatos(comandoUpdate);
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            return "";
        }


        private DbType TypeToDbType(Type pTipo)
        {
            if (pTipo.Equals(typeof(string)))
            {
                return DbType.String;
            }
            else if (pTipo.Equals(typeof(Guid)))
            {
                return IBD.TipoGuidToString(DbType.Guid);
            }
            else if (pTipo.Equals(typeof(DateTime)))
            {
                return IBD.TipoGuidToString(DbType.DateTime);
            }

            return DbType.Int32;
        }

        /// <summary>
        /// Obtiene una lista con las tablas del dataSet en orden
        /// </summary>
        /// <param name="pDataSet">DataSet del que se quieren ordenar las tablas</param>
        /// <returns>Lista con las tablas del dataSet en orden</returns>
        public List<DataTable> ObtenerOrdenTablas(DataSet pDataSet)
        {
            return new List<DataTable>();
        }

        /// <summary>
        /// Crea una conexión a la BD de forma rápida.
        /// </summary>
        /// <param name="pConexion">Conexión</param>
        /// <param name="pTimeout">Tiempo de espera</param>
        public void QuickOpen(DbConnection pConexion)
        {
            mServicesUtilVirtuosoAndReplication.QuickOpen(pConexion);
        }

        #region Métodos de interacción con la base de datos 

        /// <summary>
        /// Obtiene un comando de base de datos a partir de una sentencia SQL
        /// </summary>
        /// <param name="pSentenciaSQL">Sentencia SQL</param>
        /// <returns></returns>
        protected DbCommand ObtenerComando(string pSentenciaSQL, EntityContextBASE pEntityContextBASE = null)
        {
            var type = ConexionMaster;
            if (pEntityContextBASE != null)
            {
                type = pEntityContextBASE.Database.GetDbConnection();
            }
            
            if (type is SqlConnection)
            {
                return new SqlCommand(pSentenciaSQL, (SqlConnection)type);
            }
            else if (type is OracleConnection)
            {
                return new OracleCommand(pSentenciaSQL, (OracleConnection)type);
            }
            else if (type is NpgsqlConnection)
            {
                return new NpgsqlCommand(pSentenciaSQL, (NpgsqlConnection)type);
            }
            return null;

        }

        /// <summary>
        /// Agrega el parámetro a la lista de parámetros
        /// </summary>
        /// <param name="pParametro">Parámetro a agregar</param>
        private void AgregarParametro(ParametroComando pParametro)
        {
            if (!mListaParametros.ContainsKey(pParametro.Comando))
            {
                mListaParametros.Add(pParametro.Comando, new List<ParametroComando>());
            }

            if (!mListaParametros[pParametro.Comando].Contains(pParametro))
            {
                mListaParametros[pParametro.Comando].Add(pParametro);
            }
        }

        /// <summary>
        /// Agrega un parámetro a un comando
        /// </summary>
        /// <param name="pComando">Comando al que pertenece el parámetro</param>
        /// <param name="pNombre">Nombre del parámetro</param>
        /// <param name="pTipo">Tipo del parámetro</param>
        /// <param name="pCampoDataSet">Campo del DataSet al que se refiere el parámetro</param>
        /// <param name="pVersion">Versión del campo del DataSet al que hace referencia el parámetro</param>
        protected void AgregarParametro(DbCommand pComando, string pNombre, DbType pTipo, string pCampoDataSet, DataRowVersion pVersion)
        {
            AgregarParametro(new ParametroComandoDataSet(pComando, pNombre, pTipo, pCampoDataSet, pVersion));
        }

        /// <summary>
        /// Constructor del parámetro
        /// </summary>
        /// <param name="pComando">Comando al que pertenece el parámetro</param>
        /// <param name="pNombre">Nombre del parámetro</param>
        /// <param name="pTipo">Tipo del parámetro</param>
        /// <param name="pValor">Valor del parámetro</param>
        public void AgregarParametro(DbCommand pComando, string pNombre, DbType pTipo, object pValor)
        {
            AgregarParametro(new ParametroComandoValor(pComando, pNombre, pTipo, pValor));
        }

        /// <summary>
        /// Agrega los parámetros que se han creado para este comando a la base de datos especificada
        /// </summary>
        /// <param name="pDataBase">Base de datos</param>
        /// <param name="pComando">Comando</param>
        private void AgregarParametrosABaseDeDatos(Microsoft.Practices.EnterpriseLibrary.Data.Database pDataBase, DbCommand pComando)
        {
            StringBuilder mensajeTraza = new StringBuilder();
            try
            {
                mensajeTraza.AppendLine("inicio traza " + (pDataBase == null));
                if (pComando != null)
                {
                    mensajeTraza.AppendLine("pcomando no es null");
                    if (mListaParametros.ContainsKey(pComando))
                    {
                        mensajeTraza.AppendLine("mlistaparametros contiene a comando");
                        foreach (ParametroComando parametro in mListaParametros[pComando])
                        {
                            mensajeTraza.AppendLine("para cada parametro " + (parametro == null));
                            if (parametro is ParametroComandoDataSet)
                            {
                                mensajeTraza.AppendLine("parametro is parametrocomandodataset");
                                ParametroComandoDataSet paramDS = (ParametroComandoDataSet)parametro;
                                mensajeTraza.AppendLine("casting " + (pDataBase == null));
                                AgregarParametroACommand(pComando, paramDS, pDataBase);

                                mensajeTraza.AppendLine("addinparameter");
                            }
                            else if (parametro is ParametroComandoValor)
                            {
                                mensajeTraza.AppendLine("parametro is parametrocomandovalor");
                                ParametroComandoValor paramValor = (ParametroComandoValor)parametro;
                                mensajeTraza.AppendLine("casting " + (pDataBase == null));
                                AgregarParametroACommand(pComando, paramValor, pDataBase);

                            }
                            mensajeTraza.AppendLine("fin foreach");
                        }
                        mensajeTraza.AppendLine("mlistaparametros de comando clear");
                        mListaParametros[pComando].Clear();
                        mensajeTraza.AppendLine("mListaParametros limpios");
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                mLoggingService.GuardarLogError(ex, mensajeTraza.ToString());
                throw;
            }
        }

        private void AgregarParametroACommand(DbCommand pComando, ParametroComandoDataSet paramDS, Microsoft.Practices.EnterpriseLibrary.Data.Database pDataBase)
        {
            if (ConexionMaster is OracleConnection)
            {
                var parameter = pComando.CreateParameter() as OracleParameter;
                AgregarTipoParametroAParametro(parameter, paramDS.Tipo);
                parameter.ParameterName = paramDS.Nombre;
                parameter.SourceColumn = paramDS.CampoDataSet;
                parameter.SourceVersion = paramDS.Version;
                pComando.Parameters.Add(parameter);
            }
            else if (ConexionMaster is NpgsqlConnection)
            {
                var parameter = pComando.CreateParameter() as NpgsqlParameter;
                AgregarTipoParametroAParametro(parameter, paramDS.Tipo);
                parameter.ParameterName = paramDS.Nombre;
                parameter.SourceColumn = paramDS.CampoDataSet;
                parameter.SourceVersion = paramDS.Version;
                pComando.Parameters.Add(parameter);
            }
            else
            {
                var parameter = pComando.CreateParameter() as SqlParameter;
                AgregarTipoParametroAParametro(parameter, paramDS.Tipo);
                parameter.ParameterName = paramDS.Nombre;
                parameter.SourceColumn = paramDS.CampoDataSet;
                parameter.SourceVersion = paramDS.Version;
                pComando.Parameters.Add(parameter);
               // pDataBase.AddInParameter(pComando, paramDS.Nombre, paramDS.Tipo, paramDS.CampoDataSet, paramDS.Version);
            }
        }

        private void AgregarParametroACommand(DbCommand pComando, ParametroComandoValor pParametroComandoValor, Microsoft.Practices.EnterpriseLibrary.Data.Database pDataBase)
        {
            if (ConexionMaster is OracleConnection)
            {
                var parameter = pComando.CreateParameter() as OracleParameter;
                AgregarTipoParametroAParametro(parameter, pParametroComandoValor.Tipo);
                parameter.ParameterName = pParametroComandoValor.Nombre;
                parameter.Value = pParametroComandoValor.Valor;

                if (parameter.OracleDbType.Equals(OracleDbType.Raw))
                {
                    parameter.Value = IBD.FormatearGuid((Guid)pParametroComandoValor.Valor);
                }

                pComando.Parameters.Add(parameter);
            }
            else if (ConexionMaster is NpgsqlConnection)
            {
                var parameter = pComando.CreateParameter() as NpgsqlParameter;
                AgregarTipoParametroAParametro(parameter, pParametroComandoValor.Tipo);
                parameter.ParameterName = pParametroComandoValor.Nombre;
                parameter.Value = pParametroComandoValor.Valor;

                if (parameter.NpgsqlDbType.Equals(NpgsqlTypes.NpgsqlDbType.Uuid))
                {
                    parameter.Value = IBD.FormatearGuid((Guid)pParametroComandoValor.Valor);
                }

                pComando.Parameters.Add(parameter);
            }
            else
            {
                var parameter = pComando.CreateParameter() as SqlParameter;
                AgregarTipoParametroAParametro(parameter, pParametroComandoValor.Tipo);
                parameter.ParameterName = pParametroComandoValor.Nombre;
                parameter.Value = pParametroComandoValor.Valor;
                pComando.Parameters.Add(parameter);
                //pDataBase.AddInParameter(pComando, pParametroComandoValor.Nombre, pParametroComandoValor.Tipo, pParametroComandoValor.Valor);
            }
        }

        private void AgregarTipoParametroAParametro(OracleParameter pParameter, DbType pTipo)
        {
            if (pTipo.Equals(DbType.Guid) || pTipo.Equals(DbType.Object))
            {
                pParameter.OracleDbType = OracleDbType.Raw;
            }
            else
            {
                pParameter.DbType = pTipo;
            }
        }
        private void AgregarTipoParametroAParametro(NpgsqlParameter pParameter, DbType pTipo)
        {
            if (pTipo.Equals(DbType.Guid) || pTipo.Equals(DbType.Object))
            {
                pParameter.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Uuid;
            }
            else
            {
                pParameter.DbType = pTipo;
            }
        }

        private void AgregarTipoParametroAParametro(SqlParameter pParameter, DbType pTipo)
        {

            pParameter.DbType = pTipo;
            
        }

        //static Dictionary<string, int> listaConsultas = new Dictionary<string, int>();

        /// <summary>
        /// Obtiene la conexión a una base de datos
        /// </summary>
        /// <param name="pBaseDatos">Base de datos</param>
        /// <returns></returns>
        private DbConnection ObtenerConexionBaseDatos(Microsoft.Practices.EnterpriseLibrary.Data.Database pBaseDatos)
        {
            
            DbConnection conexion = mEntityContext.Database.GetDbConnection();
            if(conexion.State != ConnectionState.Open)
            {
                conexion.Open();
            }
            return conexion;
        }

        public static ConcurrentDictionary<string, string> ListaDefaultSchemaPorConexion { get; set; } = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Cierra todas las conexiones abiertas a SQLServer
        /// </summary>
        public void CerrarConexion()
        {
            mEntityContext.EliminarInstance();
        }

        /// <summary>
        /// Comprueba si una consulta que ha dado error debe reintentarse
        /// </summary>
        /// <param name="pMensajeError">Mensaje de la excepción de base de datos producida</param>
        /// <returns></returns>
        private bool ComprobarMensajeErrorParaReintento(Exception pError)
        {
            if ((pError.Message.Contains("Timeout expired") || pError.Message.ToLower().Contains("tiempo de espera agotado")) && (pError.StackTrace.Contains("System.Data.SqlClient.SqlConnection.Open()")))
            {
                return true;
            }

            foreach (string mensaje in ListaErroresPermitidos)
            {
                if (pError.Message.Contains(mensaje))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Carga una tabla del DataSet con un comando concreto
        /// </summary>
        /// <param name="pDataSet">DataSet a cargar</param>
        /// <param name="pTabla">Tabla del dataSet</param>
        /// <param name="pComando">Comando que carga la tabla</param>
        /// <returns></returns>
        protected void CargarDataSet(DbCommand pComando, DataSet pDataSet, string pTabla, string pCadenaConexion = null, bool pEjecutarSiEsOracle = false, bool pEjecutarSiEsPostgres = false, EntityContextBASE pEntityContextBASE = null, bool pUsarBase = false)
        {
            if (ConexionMaster is OracleConnection && !pEjecutarSiEsOracle)
            {
                return;
            }
            if (ConexionMaster is NpgsqlConnection && !pEjecutarSiEsPostgres)
            {
                return;
            }
            //DateTime antes = DateTime.Now;
            AgregarParametrosABaseDeDatos(BaseDatos, pComando);
            DbDataAdapter dataAdapter = null;

            try
            {
                AgregarEntradaTraza("CargarDataSet");

                if (pEjecutarSiEsOracle)
                {
                    dataAdapter = new OracleDataAdapter();
                }
                else if (pEjecutarSiEsPostgres)
                {
                    dataAdapter = new NpgsqlDataAdapter();
                }
                else
                {
                    dataAdapter = new SqlDataAdapter();
                }
                dataAdapter.SelectCommand = pComando;
                pComando.Transaction = Transaccion;

                if (!string.IsNullOrEmpty(pCadenaConexion))
                {
                    AgregarEntradaTraza("CargarDataSet : INICIO Crear conexión");

                    SqlDatabase database = new SqlDatabase(pCadenaConexion);
                    pComando.Connection = database.CreateConnection();

                    AgregarEntradaTraza("CargarDataSet : FIN Crear conexión");
                }
                else
                {
                    AgregarEntradaTraza("CargarDataSet : INICIO Obtener conexión");
                    if(pEntityContextBASE != null)
                    {
                        pComando.Connection = pEntityContextBASE.Database.GetDbConnection();
                        if (pComando.Connection.State != ConnectionState.Open)
                        {
                            pComando.Connection.Open();
                        }
                    }
                    else
                    {
                        string baseConnection = mConfigService.ObtenerBaseConnectionString();

                        if (pUsarBase)
                        {
                            var type = ConexionMaster;
                            if (pEntityContextBASE != null)
                            {
                                type = pEntityContextBASE.Database.GetDbConnection();
                            }

                            DbConnection db = null;
                            if (type is SqlConnection)
                            {
                                db = new SqlConnection(baseConnection);
                            }
                            else if (type is OracleConnection)
                            {
                                db = new OracleConnection(baseConnection);
                            }
                            else if (type is NpgsqlConnection)
                            {
                                db = new NpgsqlConnection(baseConnection);
                            }

                            pComando.Connection = db;
                        }
                        else
                        {
                            pComando.Connection = Conexion;
                        }
                        
                    }
                    

                    AgregarEntradaTraza("CargarDataSet : FIN Obtener conexión");
                }

                pComando.CommandTimeout = 600 * 1000;

                dataAdapter.Fill(pDataSet, pTabla);

                TransformaGuidColumnasOracle(pDataSet, pTabla);
            }
            catch (SqlException ex)
            {
                if (ComprobarMensajeErrorParaReintento(ex))
                {
                    AgregarEntradaTraza(ex.Message);

                    try
                    {
                        pComando.Transaction = Transaccion;
                        pComando.Connection = Conexion;
                        dataAdapter.Fill(pDataSet, pTabla);
                    }
                    catch (Exception ex2)
                    {
                        throw new ExcepcionDeBaseDeDatos(pComando, ex2);
                    }
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pComando, ex);
                }
            }
            catch (Exception)
            {
                throw;
            }

            AgregarEntradaTraza("Acabo CargarDataSet: " + pComando.CommandText);
        }

        private void TransformaGuidColumnasOracle(DataSet pDataSet, string pTabla)
        {
            if (ConexionMaster is OracleConnection)
            {
                List<DataColumn> listaColumnas = new List<DataColumn>();
                foreach (DataColumn columna in pDataSet.Tables[pTabla].Columns)
                {
                    if (columna.DataType.Equals(typeof(Guid)))
                    {
                        listaColumnas.Add(columna);
                    }
                }

                if (listaColumnas.Count > 0)
                {
                    foreach (DataRow fila in pDataSet.Tables[pTabla].Rows)
                    {
                        foreach (DataColumn columna in listaColumnas)
                        {
                            fila[columna] = new Guid(IBD.FormatearGuid((Guid)fila[columna], true));
                        }
                        fila.AcceptChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Ejecuta el comando en la base de datos y devuelve un reader para recorrer los resultados
        /// </summary>
        /// <param name="pComando">Comando a ejecutar</param>
        /// <returns></returns>
        protected IDataReader EjecutarReader(DbCommand pComando)
        {
            if (ConexionMaster is OracleConnection)
            {
                return new EmptyDataReader();
            }

            AgregarParametrosABaseDeDatos(BaseDatos, pComando);
            IDataReader resultado = null;
            try
            {
                AgregarEntradaTraza("EjecutarReader");
                pComando.Transaction = Transaccion;
                pComando.Connection = Conexion;

                AgregarEntradaTraza("EjecutarReader Inicio");

                resultado = pComando.ExecuteReader();

                AgregarEntradaTraza("EjecutarReader FIN");

            }
            catch (SqlException ex)
            {
                if (ComprobarMensajeErrorParaReintento(ex))
                {
                    AgregarEntradaTraza("Error: " + ex.Message);

                    try
                    {
                        pComando.Transaction = Transaccion;
                        pComando.Connection = Conexion;
                        resultado = pComando.ExecuteReader();
                    }
                    catch (Exception ex2)
                    {
                        throw new ExcepcionDeBaseDeDatos(pComando, ex2);
                    }
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pComando, ex);
                }
            }
            catch (Exception)
            {
                throw;
            }

            AgregarEntradaTraza("Acabo EjecutarReader: " + pComando.CommandText);
            return resultado;
        }

        /// <summary>
        /// Ejecuta el comando en la base de datos y devuelve el primer campo de la primera fila obtenida en los resultados
        /// </summary>
        /// <param name="pComando">Comando a ejecutar</param>
        /// <returns></returns>
        protected object EjecutarEscalar(DbCommand pComando, bool pEsOracle = false, bool pEsPostgres = false, EntityContextBASE pEntityContextBASE = null)
        {
            if (ConexionMaster is OracleConnection && !pEsOracle)
            {
                return null;
            }
            else if (ConexionMaster is NpgsqlConnection && !pEsPostgres)
            {
                return null;
            }
            AgregarParametrosABaseDeDatos(BaseDatos, pComando);
            object resultado = null;

            try
            {
                AgregarEntradaTraza("EjecutarEscalar");
                if(pEntityContextBASE != null)
                {
                    pComando.Connection = pEntityContextBASE.Database.GetDbConnection();
                    if (pComando.Connection.State != ConnectionState.Open)
                    {
                        //mLoggingService.GuardarLog($"cadena de conexion: {pComando.Connection.ConnectionString}");
                       
                        pComando.Connection.Open();
                    }
                }
                else
                {
                    pComando.Connection = Conexion;
                }
                pComando.Transaction = Transaccion;
                pComando.CommandTimeout = 600 * 1000;
                resultado = pComando.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                mLoggingService.GuardarLog($"cadena de conexion: {pComando.Connection.ConnectionString}");
                if (ComprobarMensajeErrorParaReintento(ex))
                {
                    AgregarEntradaTraza("Error: " + ex.Message);

                    try
                    {
                        pComando.Transaction = Transaccion;
                        pComando.Connection = Conexion;
                        resultado = pComando.ExecuteScalar();
                    }
                    catch (Exception ex2)
                    {
                        throw new ExcepcionDeBaseDeDatos(pComando, ex2);
                    }
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pComando, ex);
                }
            }
            catch (Exception)
            {
                throw;
            }
            AgregarEntradaTraza("Acabo EjecutarEscalar: " + pComando.CommandText);
            return resultado;
        }

        /// <summary>
        /// Ejecuta el comando insert/update/delete en la base de datos
        /// </summary>
        /// <param name="pComando">Comando insert/update/delete a ejecutar</param>
        /// <param name="pIniciarTransaccion">Falso si no se desea iniciar una Transacción</param>
        /// <returns></returns>
        protected int ActualizarBaseDeDatos(DbCommand pComando, bool pIniciarTransaccion = true, bool pEjecutarSiEsOracle = false, bool pEjecutarSiEsPostgres = false, EntityContextBASE pEntityContextBASE = null)
        {
            if (ConexionMaster is OracleConnection && !pEjecutarSiEsOracle)
            {
                return 0;
            }
            if (ConexionMaster is NpgsqlConnection && !pEjecutarSiEsPostgres)
            {
                return 0;
            }
            AgregarParametrosABaseDeDatos(BaseDatosMaster, pComando);
            int resultado = 0;

            try
            {
                bool transaccionIniciada = pIniciarTransaccion;

                if (pIniciarTransaccion && pEntityContextBASE == null)
                {
                    transaccionIniciada = IniciarTransaccion(false);
                }
                
                //if (Transaccion != null)
                {
                    AgregarEntradaTraza("NonQuery");
                    if(pEntityContextBASE != null)
                    {
                        pComando.Connection = pEntityContextBASE.Database.GetDbConnection();
                        if (pComando.Connection.State != ConnectionState.Open)
                        {
                            pComando.Connection.Open();
                        }
                    }
                    else
                    {
                        pComando.Connection = ConexionMaster;
                    }
                    pComando.CommandTimeout = 600;
                    if (Transaccion != null && pComando.Connection.Equals(Transaccion.Connection))
                    {
                        pComando.Transaction = Transaccion;
                    }
                    
                    resultado = pComando.ExecuteNonQuery();
                }
                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }
            }
            catch (Exception ex)
            {
                TerminarTransaccion(false);
                throw new ExcepcionDeBaseDeDatos(pComando, ex);
            }

            AgregarEntradaTraza("Acabo NonQuery: " + pComando.CommandText);
            return resultado;
        }

        protected void ActualizarBaseDeDatosEntityContext()
        {
            try
            {
                bool transaccionIniciada = IniciarTransaccionEntityContext();

                mEntityContext.SaveChanges();

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }

            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                TerminarTransaccion(false);
                throw;
            }
        }

        /// <summary>
        /// Actualiza los cambios que contenga el DataSet en la base de datos
        /// </summary>
        /// <param name="pDataSet">DataSet que contiene los cambios</param>
        /// <param name="pTabla">Tabla del dataSet a actualizar</param>
        /// <param name="pComandoInsert">Comando INSERT que actualiza la tabla en la base de datos</param>
        /// <param name="pComandoUpdate">Comando UPDATE que acutaliza la tabla en la base de datos</param>
        /// <param name="pComandoDelete">Comando DELETE que actualiza la tabla en la base de datos</param>
        /// <param name="pComportamiento">Comportamiento de la transacción</param>
        /// <returns></returns>
        protected int ActualizarBaseDeDatos(DataSet pDataSet, string pTabla, DbCommand pComandoInsert, DbCommand pComandoUpdate, DbCommand pComandoDelete, UpdateBehavior pComportamiento, bool pEsOracle = false, bool pEsPostgres = false, bool pUsarBase = false, EntityContextBASE pEntityContextBASE = null)
        {
            if (ConexionMaster is OracleConnection && !pEsOracle)
            {
                return 0;
            }
            if (ConexionMaster is NpgsqlConnection && !pEsPostgres)
            {
                return 0;
            }
            int resultado = 0;
            AgregarParametrosABaseDeDatos(BaseDatosMaster, pComandoInsert);
            AgregarParametrosABaseDeDatos(BaseDatosMaster, pComandoUpdate);
            AgregarParametrosABaseDeDatos(BaseDatosMaster, pComandoDelete);
            DbDataAdapter dataAdapter = null;

            AgregarEntradaTraza("Actualizo");

            if (pEsOracle)
            {
                dataAdapter = new OracleDataAdapter();
            }
            else if(pEsPostgres)
            {
                dataAdapter = new NpgsqlDataAdapter();
            }
            else
            {
                
                dataAdapter = new SqlDataAdapter();
            }
            dataAdapter.InsertCommand = pComandoInsert;
            dataAdapter.UpdateCommand = pComandoUpdate;
            dataAdapter.DeleteCommand = pComandoDelete;

            try
            {
                string baseConnection = mConfigService.ObtenerBaseConnectionString();
                
                var type = ConexionMaster;
                if (pEntityContextBASE != null)
                {
                    type = pEntityContextBASE.Database.GetDbConnection();
                }

                DbConnection db = null;
                if (type is SqlConnection)
                {
                    db = new SqlConnection(baseConnection);
                }
                else if (type is OracleConnection)
                {
                    db = new OracleConnection(baseConnection);
                }
                else if (type is NpgsqlConnection)
                {
                    db = new NpgsqlConnection(baseConnection);
                }

                bool transaccionIniciada = IniciarTransaccion();
             
                if (pUsarBase)
                {
                    
                    if (pComandoInsert != null)
                    {
                        pComandoInsert.Connection = db;
                        dataAdapter.InsertCommand.Transaction = TransaccionBASE;
                    }
                    if (pComandoUpdate != null)
                    {
                        pComandoUpdate.Connection = db;
                        dataAdapter.UpdateCommand.Transaction = TransaccionBASE;
                    }
                    if (pComandoDelete != null)
                    {
                        pComandoDelete.Connection = db;
                        dataAdapter.DeleteCommand.Transaction = TransaccionBASE;
                    }
                }
                else
                {
                    if (pComandoInsert != null)
                    {
                        pComandoInsert.Connection = ConexionMaster;
                        dataAdapter.InsertCommand.Transaction = Transaccion;
                    }
                    if (pComandoUpdate != null)
                    {
                        pComandoUpdate.Connection = ConexionMaster;
                        dataAdapter.UpdateCommand.Transaction = Transaccion;
                    }
                    if (pComandoDelete != null)
                    {
                        pComandoDelete.Connection = ConexionMaster;
                        dataAdapter.DeleteCommand.Transaction = Transaccion;
                    }
                }

                dataAdapter.Update(pDataSet, pTabla);

                if (transaccionIniciada)
                {
                    TerminarTransaccion(true);
                }
            }
            catch (Exception)
            {
                TerminarTransaccion(false);
                throw;
            }

            AgregarEntradaTraza("Actualizada " + pTabla);
            return resultado;
        }

        /// <summary>
        /// Devuelve la fecha y hora actual del servidor SQLServer
        /// </summary>
        /// <returns>DateTime</returns>
        public DateTime FechaHoraServidor()
        {
            return IBD.FechaHoraServidor(BaseDatos);
        }

        /// <summary>
        /// Pagina un dataset pasado por parámetro
        /// </summary>
        /// <param name="pComando">Comando de base de datos con la consulta y parámetros normales</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset para paginar</param>
        /// <param name="pInicio">Primera fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pLimite">Última fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pNombreTabla">Nombre de la tabla que queremos paginar del dataset</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSet(DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla)
        {
            return PaginarDataSet(pComando, pOrderBy, pDataSet, pInicio, pLimite, pNombreTabla, true);
        }


        /// <summary>
        /// Pagina un dataset
        /// </summary>
        /// <param name="pComando">Comando de base de datos con la consulta y parámetros normales</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset para paginar</param>
        /// <param name="pInicio">Primera fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pLimite">Última fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pNombreTabla">Nombre de la tabla que queremos paginar del dataset</param>
        /// <param name="pContar">Indica si queremos contar o no el número de registros del resultado</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSet(DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla, bool pContar)
        {
            return PaginarDataSet(pComando, pOrderBy, pDataSet, pInicio, pLimite, pNombreTabla, pContar, "");
        }

        /// <summary>
        /// Pagina un dataset
        /// </summary>
        /// <param name="pComando">Comando de base de datos con la consulta y parámetros normales</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset para paginar</param>
        /// <param name="pInicio">Primera fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pLimite">Última fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pNombreTabla">Nombre de la tabla que queremos paginar del dataset</param>
        /// <param name="pContar">Indica si queremos contar o no el número de registros del resultado</param>
        /// <param name="pConsultaLigeraCount">Consulta ligera para hacer el count de la paginacion</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSet(DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla, bool pContar, string pConsultaLigeraCount)
        {
            AgregarParametrosABaseDeDatos(BaseDatos, pComando);
            pComando.Transaction = Transaccion;
            pComando.Connection = Conexion;
            int numero = 0;
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                numero = IBD.PaginarDataSet(BaseDatos, pComando, pOrderBy, pDataSet, pInicio, pLimite, pNombreTabla, pContar, pConsultaLigeraCount);
                mLoggingService.AgregarEntradaDependencia("Paginacion del DataSet: " + pComando.CommandText, false, "Paginar DataSet", sw, true);
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia("Paginacion del DataSet: " + pComando.CommandText, false, "Paginar DataSet", sw, false);
                throw new ExcepcionDeBaseDeDatos(pComando, ex);
            }

            return numero;
        }

        /// <summary>
        /// Pagina un dataset pasado por parámetro
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jerárquica</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <param name="pNombreTabla">Nombre de la tabla del dataset que se quiere paginar</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSetConConsultaJerarquica(DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla)
        {
            AgregarParametrosABaseDeDatos(BaseDatos, pComando);
            pComando.Transaction = Transaccion;
            pComando.Connection = Conexion;
            int numero = 0;
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                numero = IBD.PaginarDataSetConConsultaJerarquica(BaseDatos, pComando, pConsultaJerarquica, pOrderBy, pDataSet, pInicio, pLimite, pNombreTabla);
                mLoggingService.AgregarEntradaDependencia("Paginacion del DataSet: " + pComando.CommandText, false, "Paginar DataSet", sw, true);
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia("Paginacion del DataSet: " + pComando.CommandText, false, "Paginar DataSet", sw, false);
                throw new ExcepcionDeBaseDeDatos(pComando, ex);
            }

            return numero;
        }

        /// <summary>
        /// Pagina un dataset
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pParteWith">Parte WITH de la consulta jerárquica manual</param>
        /// <param name="pParteSelect">Parte SELECT de la consulta jerárquica manual</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <param name="pNombreTabla">Nombre de la tabla del dataset que se quiere paginar</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDatasetConsultaJerarquicaManual(DbCommand pComando, string pParteWith, string pParteSelect, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla)
        {
            AgregarParametrosABaseDeDatos(BaseDatos, pComando);
            pComando.Transaction = Transaccion;
            pComando.Connection = Conexion;
            int numero = 0;
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                numero = ((BDSqlServer)IBD).PaginarDatasetConsultaJerarquicaManual(BaseDatos, pComando, pParteWith, pParteSelect, pOrderBy, pDataSet, pInicio, pLimite, pNombreTabla);
                mLoggingService.AgregarEntradaDependencia("Paginacion del DataSet: " + pComando.CommandText, false, "Paginar DataSet", sw, true);
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia("Paginacion del DataSet: " + pComando.CommandText, false, "Paginar DataSet", sw, false);
                throw new ExcepcionDeBaseDeDatos(pComando, ex);
            }

            return numero;
        }

        protected void AgregarEntradaTraza(string pMensaje)
        {
            mLoggingService.AgregarEntrada(pMensaje);
        }


		/// <summary>
		/// Inicia una transacción, si no estaba ya iniciada
		/// </summary>
		/// <param name="pIniciarTransaccionEntity">Comando con la consulta para paginar</param>
		/// <returns>True si ha inicado la transacción, false si ya estaba iniciada</returns>
		public virtual bool IniciarTransaccion(bool pIniciarTransaccionEntity = true)
        {
			if (ConexionMaster is OracleConnection)
            {
                string nombreTransaccion = $"Transaccion_{((OracleConnection)ConexionMaster).ToString().ToLower()}";
                if (TransaccionesPendientes.ContainsKey(nombreTransaccion) || Transaccion != null)
                {
                    
                    //transaccion = (DbTransaction)UtilPeticion.ObtenerObjetoDePeticion(nombreTransaccion);
                    return false;
                }
                else
                {
                    DbTransaction transaccion = ConexionMaster.BeginTransaction();
                    
                    if (mEntityContext.Database.GetDbConnection().Equals(ConexionMaster))
                    {
                        mEntityContext.Database.UseTransaction(transaccion);
                    }

                    if (NoConfirmarTransacciones)
                    {
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                    }
                    //if (pIniciarTransaccionEntity)
                    //{
                    //    IniciarTransaccionEntityContext();
                    //}

                    return true;
                }
            }
            else if (ConexionMaster is NpgsqlConnection)
            {
                string nombreTransaccion = $"Transaccion_{((NpgsqlConnection)ConexionMaster).ToString().ToLower()}";
                if (TransaccionesPendientes.ContainsKey(nombreTransaccion) || Transaccion != null)
                {
                    //transaccion = (DbTransaction)UtilPeticion.ObtenerObjetoDePeticion(nombreTransaccion);
                    return false;
                }
                else
                {
					DbTransaction transaccion = ConexionMaster.BeginTransaction();

					if (mEntityContext.Database.GetDbConnection().Equals(ConexionMaster))
					{
						mEntityContext.Database.UseTransaction(transaccion);
					}
					if (NoConfirmarTransacciones)
                    {   
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);                        
                    }
                    //if (pIniciarTransaccionEntity)
                    //{
                    //    IniciarTransaccionEntityContext();
                    //}

                    return true;
                }
            }
            else
            {
                string nombreTransaccion = $"Transaccion_{((SqlConnection)ConexionMaster).ClientConnectionId.ToString().ToLower()}";
                if (TransaccionesPendientes.ContainsKey(nombreTransaccion) || Transaccion != null)
                {
                    return false;
                }
                else
                {
					DbTransaction transaccion = ConexionMaster.BeginTransaction();

					if (mEntityContext.Database.GetDbConnection().Equals(ConexionMaster))
					{
						mEntityContext.Database.UseTransaction(transaccion);
					}

					if (NoConfirmarTransacciones)
                    {
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                    }
                    //if (pIniciarTransaccionEntity)
                    //{
                    //    IniciarTransaccionEntityContext();
                    //}

                    return true;
                }
            }
        }

        /// <summary>
        /// Inicia una transacción, si no estaba ya iniciada
        /// </summary>
        /// <param name="pIniciarTransaccionEntity">Comando con la consulta para paginar</param>
        /// <returns>True si ha inicado la transacción, false si ya estaba iniciada</returns>
        public virtual bool IniciarTransaccionBASE(bool pIniciarTransaccionEntity = true)
        {
            if (ConexionMaster is OracleConnection)
            {
                string nombreTransaccion = $"Transaccion_{((OracleConnection)ConexionMaster).ToString().ToLower()}";
                if (TransaccionesPendientes.ContainsKey(nombreTransaccion) || Transaccion != null)
                {

                    //transaccion = (DbTransaction)UtilPeticion.ObtenerObjetoDePeticion(nombreTransaccion);
                    return false;
                }
                else
                {
                    DbTransaction transaccion = ConexionMaster.BeginTransaction();
                    if (mEntityContextBASE.Database.GetDbConnection().Equals(ConexionMaster))
                    {
                        mEntityContextBASE.Database.UseTransaction(transaccion);
                    }

                    if (NoConfirmarTransacciones)
                    {
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                    }
                    //if (pIniciarTransaccionEntity)
                    //{
                    //    IniciarTransaccionEntityContext();
                    //}

                    return true;
                }
            }
            else if (ConexionMaster is NpgsqlConnection)
            {
                string nombreTransaccion = $"Transaccion_{((NpgsqlConnection)ConexionMaster).ToString().ToLower()}";
                if (TransaccionesPendientes.ContainsKey(nombreTransaccion) || Transaccion != null)
                {
                    //transaccion = (DbTransaction)UtilPeticion.ObtenerObjetoDePeticion(nombreTransaccion);
                    return false;
                }
                else
                {
                    DbTransaction transaccion = ConexionMaster.BeginTransaction();
                    if (mEntityContextBASE.Database.GetDbConnection().Equals(ConexionMaster))
                    {
                        mEntityContextBASE.Database.UseTransaction(transaccion);
                    }
                    if (NoConfirmarTransacciones)
                    {
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                    }
                    //if (pIniciarTransaccionEntity)
                    //{
                    //    IniciarTransaccionEntityContext();
                    //}

                    return true;
                }
            }
            else
            {
                string nombreTransaccion = $"Transaccion_{((SqlConnection)ConexionMaster).ClientConnectionId.ToString().ToLower()}";
                if (TransaccionesPendientes.ContainsKey(nombreTransaccion) || Transaccion != null)
                {
                    return false;
                }
                else
                {
                    DbTransaction transaccion = ConexionMaster.BeginTransaction();

                    if (mEntityContextBASE.Database.GetDbConnection().Equals(ConexionMaster))
                    {
                        mEntityContextBASE.Database.UseTransaction(transaccion);
                    }

                    if (NoConfirmarTransacciones)
                    {
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                    }
                    //if (pIniciarTransaccionEntity)
                    //{
                    //    IniciarTransaccionEntityContext();
                    //}

                    return true;
                }
            }
        }


        public bool IniciarTransaccionEntityContext()
        {
            bool transaccionIniciada = false;
            if (Transaccion == null)
            {
                transaccionIniciada = IniciarTransaccion(false);
            }
            if (mServicesUtilVirtuosoAndReplication == null || string.IsNullOrEmpty(mServicesUtilVirtuosoAndReplication.FicheroConfiguracion) || mServicesUtilVirtuosoAndReplication.FicheroConfiguracion.Contains("acid"))
            {
                if (mEntityContext.Database.CurrentTransaction == null || !mEntityContext.Database.CurrentTransaction.GetDbTransaction().Equals(Transaccion))
                {
                    if (mEntityContext.ContextoInicializado && mEntityContext.ChangeTracker.HasChanges() && !mEntityContext.Database.GetDbConnection().Equals(ConexionMaster))
                    {

                    }
                    else
                    {
                        mEntityContext.Database.UseTransaction(Transaccion);
                    }

                }
            }

            return transaccionIniciada;
        }

        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito">Verdad si se deba hacer commit. Falso si se debe deshacer la transacción</param>
        public virtual void TerminarTransaccion(bool pExito)
        {
            if (!NoConfirmarTransacciones)
            {
                if (Transaccion != null)
                {
                    if (pExito)
                    {
                        mEntityContext.Database.CommitTransaction();
                    }
                    else if (Transaccion.Connection != null)
                    {
                        try
                        {
                            mEntityContext.Database.RollbackTransaction();
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito">Verdad si se deba hacer commit. Falso si se debe deshacer la transacción</param>
        public virtual void TerminarTransaccionBASE(bool pExito)
        {
            if (!NoConfirmarTransacciones)
            {
                if (TransaccionBASE != null)
                {
                    if (pExito)
                    {
                        mEntityContextBASE.Database.CommitTransaction();
                    }
                    else if (TransaccionBASE.Connection != null)
                    {
                        try
                        {
                            mEntityContextBASE.Database.RollbackTransaction();
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex);
                        }
                    }
                }
            }
        }


        #endregion

        #endregion

        #region Propiedades

        protected string FicheroConfiguracion
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.FicheroConfiguracion;
            }
        }

        [Obsolete("Esta propiedad no se usa para nada, desaparecera en futras versiones.")]
        protected bool UsarVariableEstatica
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene la interfaz de base de datos
        /// </summary>
        protected IBaseDatos IBD
        {
            get
            {
                if (mIBD == null)
                {
                    string tipoBD = mConfigService.ObtenerTipoBD();

                    if (tipoBD.Equals("1"))
                    {
                        mIBD = new BDOracle(mConfigService);
                    }
                    else if (tipoBD.Equals("2"))
                    {
                        mIBD = new BDPostgres(mConfigService);
                    }
                    else
                    {
                        mIBD = new BDSqlServer(mConfigService);
                    }
                }

                return mIBD;
            }
        }

        /// <summary>
        /// Obtiene la base de datos que se conecta al servidor maestro de BD. 
        /// </summary>
        private Microsoft.Practices.EnterpriseLibrary.Data.Database BaseDatosMaster
        {
            get
            {
                if (mBaseDatosMaster == null)
                {
                    if (string.IsNullOrEmpty(mServicesUtilVirtuosoAndReplication.FicheroConfiguracion))
                    {
                        mBaseDatosMaster = IBD.CrearBDEnterprise("acid_Master");
                    }
                    else if (!mServicesUtilVirtuosoAndReplication.FicheroConfiguracion.EndsWith("_Master") && !mServicesUtilVirtuosoAndReplication.FicheroConfiguracion.EndsWith("CargarConfiguracion"))
                    {
                        mBaseDatosMaster = IBD.CrearBDEnterprise(mServicesUtilVirtuosoAndReplication.FicheroConfiguracion + "_Master");
                    }
                    else
                    {
                        return mBaseDatos;
                    }
                }
                return mBaseDatosMaster;
            }
        }

        /// <summary>
        /// Obtiene la base de datos que se conecta al SGBD concreto
        /// </summary>
        private Microsoft.Practices.EnterpriseLibrary.Data.Database BaseDatos
        {
            get
            {
                    if (mBaseDatos == null)
                    {
                        // A saber por qué la variable mBaseDatos es null, no debería serlo nunca salvo que se haya hecho dispose del AD. La cargo de nuevo. 
                        if (string.IsNullOrEmpty(mServicesUtilVirtuosoAndReplication.FicheroConfiguracion))
                        {
                            Cargar();
                        }
                        else
                        {
                            Cargar(mServicesUtilVirtuosoAndReplication.FicheroConfiguracion);
                        }
                    }
                    return mBaseDatos;
                
            }
        }

        /// <summary>
        /// Guarda la conexión a la base de datos
        /// </summary>
        private DbConnection mConexion = null;

        /// <summary>
        /// Obtiene la conexión a la base de datos
        /// </summary>
        protected DbConnection Conexion
        {
            get
            {
                return ConexionMaster;
            }
        }

        /// <summary>
        /// Guarda la conexión a la base de datos Master
        /// </summary>
        private DbConnection mConexionMaster = null;

        /// <summary>
        /// Obtiene la conexión a la base de datos Master
        /// </summary>
        protected virtual DbConnection ConexionMaster
        {
            get
            {
                var conexion = mEntityContext.Database.GetDbConnection();
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                }
                return conexion;
            }
        }



        /// <summary>
        /// Obtiene la cadena de conexión de la base de datos
        /// </summary>
        public string CadenaConexion
        {
            get
            {
                return BaseDatos.CreateConnection().ConnectionString;
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión de la base de datos sin credenciales de acceso
        /// </summary>
        public string CadenaConexionSinCredenciales
        {
            get
            {
                return BaseDatos.ConnectionStringWithoutCredentials;
            }
        }

        /// <summary>
        /// Transacción actual
        /// </summary>
        public virtual DbTransaction Transaccion
        {
            get
            {
                try
                {
                    if(mEntityContext.Database.CurrentTransaction != null)
                    {
                        return mEntityContext.Database.CurrentTransaction.GetDbTransaction();
                    }
                    return null;
                    
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }
                return null;
            }
        }

        public virtual DbTransaction TransaccionBASE
        {
            get
            {
                try
                {
                    if (mEntityContextBASE != null && mEntityContextBASE.Database.CurrentTransaction != null)
                    {
                        return mEntityContextBASE.Database.CurrentTransaction.GetDbTransaction();
                    }
                    return null;

                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }
                return null;
            }
        }

        /// <summary>
        /// Verdad si existe el fichero bd.config con el elemento acidMaster, falso en caso contrario
        /// </summary>
        private static bool ExisteFicheroMaster
        {
            get
            {
                //if (!mExisteFicheroMaster.HasValue)
                //{
                //    string ficheroMaster = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "configBD/bd.config";
                //    StreamReader fich = new StreamReader(ficheroMaster);
                //    string contenidoFich = fich.ReadToEnd();
                //    fich.Close();
                //    fich.Dispose();
                //    fich = null;

                //    mExisteFicheroMaster = contenidoFich.ToLower().Contains("<acidmaster>");
                //}
                //return mExisteFicheroMaster.Value;

                return true;
            }
        }

        private static List<string> ListaErroresPermitidos
        {
            get
            {
                if (mListaErroresPermitidos == null)
                {
                    mListaErroresPermitidos = new List<string>(mArrayErroresPermitidos);
                }
                return mListaErroresPermitidos;
            }
        }

        /// <summary>
        /// Obtiene la lista de transacciones pendientes de confirmar o deshacer. Null si NoConfirmarTransacciones es false
        /// </summary>
        public Dictionary<string, DbTransaction> TransaccionesPendientes
        {
            get
            {
                return mEntityContext.TransaccionesPendientes;
            }
        }

        /// <summary>
        /// Obtiene o establece si se deben confirmar las transacciones que se inicien. 
        /// IMPORTANTE: El llamador que cambie esta propiedad a TRUE debe responsabilizarse de confirmar o deshacer las transacciones pendientes, almacenadas en la propiedad TransaccionesPendientes
        /// </summary>
        public bool NoConfirmarTransacciones
        {
            get
            {
                return mEntityContext.NoConfirmarTransacciones;
            }
            set
            {
                mEntityContext.NoConfirmarTransacciones = value;
            }
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
        ~BaseAD()
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
        public void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;
                try
                {
                    this.mBaseDatos = null;
                    this.mBaseDatosMaster = null;
                }
                finally
                {
                }
            }
        }

        #endregion

        #region Clases internas

        /// <summary>
        /// Parametro de base de datos para un comando
        /// </summary>
        private class ParametroComando
        {

            #region Miembros

            private DbCommand mComando;
            private string mNombre;
            private DbType mTipo;

            #endregion

            #region Constructores

            /// <summary>
            /// Constructor del parámetro
            /// </summary>
            /// <param name="pComando">Comando al que pertenece el parámetro</param>
            /// <param name="pNombre">Nombre del parámetro</param>
            /// <param name="pTipo">Tipo del parámetro</param>
            public ParametroComando(DbCommand pComando, string pNombre, DbType pTipo)
            {
                mComando = pComando;
                mNombre = pNombre;
                mTipo = pTipo;
            }

            #endregion

            #region Propiedades

            /// <summary>
            /// Obtiene el comando al que pertenece el parámetro
            /// </summary>
            public DbCommand Comando
            {
                get
                {
                    return mComando;
                }
            }

            /// <summary>
            /// Obtiene el nombre del parámetro
            /// </summary>
            public string Nombre
            {
                get
                {
                    return mNombre;
                }
            }

            /// <summary>
            /// Obtiene el tipo del parámetro
            /// </summary>
            public DbType Tipo
            {
                get
                {
                    return mTipo;
                }
            }

            #endregion
        }

        /// <summary>
        /// Parametro de base de datos para un comando
        /// </summary>
        private class ParametroComandoDataSet : ParametroComando
        {

            #region Miembros

            private string mCampoDataSet;
            private DataRowVersion mVersion;

            #endregion

            #region Constructores

            /// <summary>
            /// Constructor del parámetro
            /// </summary>
            /// <param name="pComando">Comando al que pertenece el parámetro</param>
            /// <param name="pNombre">Nombre del parámetro</param>
            /// <param name="pTipo">Tipo del parámetro</param>
            /// <param name="pCampoDataSet">Campo del DataSet al que se refiere el parámetro</param>
            /// <param name="pVersion">Versión del campo del DataSet al que hace referencia el parámetro</param>
            public ParametroComandoDataSet(DbCommand pComando, string pNombre, DbType pTipo, string pCampoDataSet, DataRowVersion pVersion)
                : base(pComando, pNombre, pTipo)
            {
                mCampoDataSet = pCampoDataSet;
                mVersion = pVersion;
            }

            #endregion

            #region Propiedades

            /// <summary>
            /// Obtiene el campo del DataSet al que se refiere el parámetro
            /// </summary>
            public string CampoDataSet
            {
                get
                {
                    return mCampoDataSet;
                }
            }

            /// <summary>
            /// Obtiene la versión del campo del DataSet al que hace referencia el parámetro
            /// </summary>
            public DataRowVersion Version
            {
                get
                {
                    return mVersion;
                }
            }

            #endregion
        }

        /// <summary>
        /// Parametro de base de datos para un comando
        /// </summary>
        private class ParametroComandoValor : ParametroComando
        {

            #region Miembros

            private object mValor;

            #endregion

            #region Constructores

            /// <summary>
            /// Constructor del parámetro
            /// </summary>
            /// <param name="pComando">Comando al que pertenece el parámetro</param>
            /// <param name="pNombre">Nombre del parámetro</param>
            /// <param name="pTipo">Tipo del parámetro</param>
            /// <param name="pValor">Valor del parámetro</param>
            public ParametroComandoValor(DbCommand pComando, string pNombre, DbType pTipo, object pValor)
                : base(pComando, pNombre, pTipo)
            {
                mValor = pValor;
            }

            #endregion

            #region Propiedades

            /// <summary>
            /// Obtiene el campo del DataSet al que se refiere el parámetro
            /// </summary>
            public object Valor
            {
                get
                {
                    return mValor;
                }
            }

            #endregion
        }

        #endregion

        private static volatile bool recalculandoConexiones = false;

        private static volatile ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>> ListaConexiones = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>>();

        protected static volatile ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ListaServidoresReplicacion = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();


        public static void LeerConfiguracionConexion(string pFicheroConexion, List<ConfiguracionBBDD> pFilasConexion)
        {
            if (!ListaConexiones.ContainsKey(pFicheroConexion))
            {
                ListaConexiones.TryAdd(pFicheroConexion, new ConcurrentDictionary<string, List<string>>());
            }
            if (!ListaServidoresReplicacion.ContainsKey(pFicheroConexion))
            {
                ListaServidoresReplicacion.TryAdd(pFicheroConexion, new ConcurrentDictionary<string, string>());
            }

            ConcurrentDictionary<string, List<string>> listaAuxConexiones = new ConcurrentDictionary<string, List<string>>();

            foreach (ConfiguracionBBDD fila in pFilasConexion)
            {
                if (fila.TipoConexion.Equals((short)TipoConexion.Virtuoso) && !string.IsNullOrEmpty(fila.DatosExtra) && fila.EsMaster)
                {
                    ListaServidoresReplicacion[pFicheroConexion].TryAdd(fila.NombreConexion, fila.DatosExtra);
                }

                if (fila.LecturaPermitida)
                {
                    string nombreConexion = fila.NombreConexion;
                    if (fila.TipoConexion.Equals((short)TipoConexion.Virtuoso_HA_PROXY)) { nombreConexion = "HA_PROXY" + nombreConexion; }

                    string cadenaConexion = fila.Conexion;
                    if (fila.ConectionTimeout != null)
                    {
                        cadenaConexion += "$$$TimeOut=" + fila.ConectionTimeout;
                    }

                    if (!listaAuxConexiones.ContainsKey(nombreConexion))
                    {
                        listaAuxConexiones.TryAdd(nombreConexion, new List<string>());
                    }

                    listaAuxConexiones[nombreConexion].Add(cadenaConexion);
                }
                if (fila.EsMaster)
                {
                    string nombreConexion = fila.NombreConexion + "_Master";
                    if (fila.TipoConexion.Equals((short)TipoConexion.Virtuoso_HA_PROXY)) { nombreConexion = "HA_PROXY" + nombreConexion; }

                    string cadenaConexion = fila.Conexion;
                    if (fila.ConectionTimeout != null)
                    {
                        cadenaConexion += "$$$TimeOut=" + fila.ConectionTimeout;
                    }

                    if (!listaAuxConexiones.ContainsKey(nombreConexion))
                    {
                        listaAuxConexiones.TryAdd(nombreConexion, new List<string>());
                    }

                    listaAuxConexiones[nombreConexion].Add(cadenaConexion);
                }
            }

            foreach (string nombreConexion in listaAuxConexiones.Keys)
            {
                if (!ListaConexiones[pFicheroConexion].ContainsKey(nombreConexion))
                {
                    ListaConexiones[pFicheroConexion].TryAdd(nombreConexion, new List<string>());
                }

                ListaConexiones[pFicheroConexion][nombreConexion] = listaAuxConexiones[nombreConexion];
            }
        }

        // <summary>
        // Obtiene la cadena de conexion de Virtuoso
        // </summary>
        // <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        // <returns>Cadena de conexión de Virtuoso</returns>
        //public string ObtenerCadenaConexionVirtuoso(List<string> pCadenasInvalidas)
        //{
        //    return ObtenerCadenaConexionVirtuoso("virtuoso", pCadenasInvalidas);
        //}

        /// <summary>
        /// Obtiene la cadena de conexion de Virtuoso
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pBD">Apelativo de la BD a la que se va a conectar</param>
        /// <returns>Cadena de conexión de Virtuoso</returns>
        public string ObtenerCadenaConexion(string pNombreConexion)
        {
            //Obtener de configService
            string nombreConexion = pNombreConexion;

            if (string.IsNullOrEmpty(nombreConexion))
            {
                nombreConexion = "acid";
            }

            if (nombreConexion.Contains("@@@"))
            {
                // Esto se usa si se está consultando una base de datos diferente a la configurada. Ej: Pasos a producción
                string[] conexion = nombreConexion.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                nombreConexion = conexion[1];
            }

            if (nombreConexion.Equals("CargarConfiguracion") || nombreConexion.Equals("acid"))
            {
                return mConfigService.ObtenerSqlConnectionString();
            }
            else
            {
                //Cambio hecho por Fernando para controlar que no vaya a cargar la configuración de BBDD cada vez que hace una consulta para obtener las vistas.
                    if (nombreConexion.Contains("vistas"))
                    {
                        nombreConexion = nombreConexion.Replace("vistas", "acid");
                    }
                    if (nombreConexion.Contains("colasReplicacion"))
                    {
                        nombreConexion = nombreConexion.Replace("colasReplicacion", "base");
                    }

                return mConfigService.ObtenerCadenaConexion(nombreConexion);
            }
        }        

    }
}
