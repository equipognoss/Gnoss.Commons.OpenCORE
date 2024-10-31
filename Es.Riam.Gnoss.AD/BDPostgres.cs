using Es.Riam.Gnoss.Util.Configuracion;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD
{
    public class BDPostgres : IBaseDatos
    {
        private ConfigService mConfigService;

        public BDPostgres(ConfigService configService)
        {
            mConfigService = configService;
        }

        #region Métodos generales

        private void AgregarConexionAListaConexionesConCredenciales(string pCadenaConexionConCredenciales, string pCadenaConexionSinCredenciales)
        {
            if (!ListaConexionesConCredenciales.ContainsKey(pCadenaConexionSinCredenciales))
            {
                try
                {
                    ListaConexionesConCredenciales.TryAdd(pCadenaConexionSinCredenciales, pCadenaConexionConCredenciales);
                }
                catch (Exception)
                {
                    //Esta excepción no hace falta controlarla, ha fallado porque otro proceso a añadido a la vez el mismo valor
                }
            }
        }

        /// <summary>
        /// Crea una base de datos de Enterprise Library
        /// </summary>
        /// <returns>Base de datos de Enterprise Library</returns>
        public Database CrearBDEnterprise()
        {
            return CrearBDEnterprise("", null);
        }

        /// <summary>
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <returns>Base de datos de Enterprise Library</returns>
        public Database CrearBDEnterprise(string pFicheroConfiguracionBD)
        {
            return CrearBDEnterprise(pFicheroConfiguracionBD, null);
        }

        /// <summary>
        /// Captura sólo el año de un campo datetime de Oracle
        /// </summary>
        /// <param name="pCampo">Nombre del campo del que extraer la fecha</param>
        /// <returns>Cadena de texto con el año</returns>
        public string AnioCampo(string pCampo)
        {
            return "extract (year from " + pCampo + ")";
        }

        /// <summary>
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de la base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        /// <returns>Base de datos de Enterprise Library</returns>
        public Database CrearBDEnterprise(string pFicheroConfiguracionBD, List<string> pCadenasInvalidas)
        {
            string conexion = ObtenerCadenaConexion();

            string[] partesConexion = conexion.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);

            SqlDatabase database = new SqlDatabase(partesConexion[0]);

            if (string.IsNullOrEmpty(pFicheroConfiguracionBD) || !pFicheroConfiguracionBD.Equals("CargarConfiguracion"))
            {
                AgregarConexionAListaConexionesConCredenciales(conexion, database.ConnectionStringWithoutCredentials + pFicheroConfiguracionBD);
            }
            return database;
        }

        /// <summary>
        /// Crea comando de base de datos tipado
        /// </summary>
        /// <param name="pComandoSql">Texto para la consulta</param>
        /// <returns>Comando de base de datos</returns>
        public DbCommand CrearDbCommand(string pComandoSql)
        {
            return new NpgsqlCommand(pComandoSql, new NpgsqlConnection(ObtenerCadenaConexion()));
        }

        /// <summary>
        /// Devuelve la cadena de conexión
        /// </summary>
        /// <returns>Cadena de conexión</returns>
        public string ObtenerCadenaConexion()
        {
            return mConfigService.ObtenerSqlConnectionString();
        }

        /// <summary>
        /// Pagina un dataset
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <param name="pComando">Comando de base de datos con la consulta y parámetros normales</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset para paginar</param>
        /// <param name="pInicio">Primera fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pLimite">Última fila que se desea obtener.Si pInicio == -1 y pLimite == -1 se trae TODOS los resultados posibles </param>
        /// <param name="pNombreTabla">Nombre de la tabla que queremos paginar del dataset</param>
        /// <param name="pContar">Indica si queremos contar o no el número de registros del resultado</param>
        /// <param name="pConsultaLigeraCount">Consulta ligera para hacer el count de la paginacion</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSet(Database pDatabase, DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla, bool pContar, string pConsultaLigeraCount)
        {
            string sql = pComando.CommandText;
            string consultaSinOrden = sql;

            if (!string.IsNullOrEmpty(pOrderBy))
            {
                consultaSinOrden = consultaSinOrden.Replace(pOrderBy, "");
            }

            if (pConsultaLigeraCount == "")
            {
                pConsultaLigeraCount = consultaSinOrden;
            }

            KeyValuePair<string, string> consultaPaginada = ObtenerConsultasPaginar(pComando, pOrderBy, pInicio, pLimite);

            //Calculo el total de filas con las que trabaja la consulta            
            pComando.CommandText = "Select count(*) from (" + pConsultaLigeraCount + ") as Contar";
            int numero = 0;
            if (pContar)
            {
                numero = System.Convert.ToInt32(pComando.ExecuteScalar());
            }
            if (pInicio == -1 && pLimite == -1)
            {
                //Cargo el DataSet , me traigo TODAS las filas posibles
                pInicio = 1;
                pLimite = numero;
            }
            pComando.CommandText = consultaSinOrden;

            pComando.CommandText = ObtenerConsultasPaginar(pComando, pOrderBy, pInicio, pLimite).Value;

            DbDataAdapter dataAdapter = null;

            dataAdapter = pDatabase.GetDataAdapter();
            dataAdapter.SelectCommand = pComando;
            dataAdapter.Fill(pDataSet, pNombreTabla);

            return numero;
        }

        /// <summary>
        /// Devuelve un KeyValuePair(consulta count, consulta resultados) con las dos consultas necesarias para realizar la paginación 
        /// (la primera devuelve el número de resultados totales y la segunda los x resultados seleccionados)
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <returns>KeyValuePair(consulta count, consulta resultados)</returns>
        public KeyValuePair<string, string> ObtenerConsultasPaginar(DbCommand pComando, string pOrderBy, int pInicio, int pLimite)
        {
            string sql = pComando.CommandText;
            string consultaSinOrden = sql;

            if (!string.IsNullOrEmpty(pOrderBy))
            {
                consultaSinOrden = consultaSinOrden.Replace(pOrderBy, "");
            }

            //Calculo el total de filas con las que trabaja la consulta            
            string count = "Select count(*) from (" + consultaSinOrden + ") as Contar";
            string datos = "";
            if (pInicio == 1)
            {
                datos = "SELECT TOP " + pLimite + " * FROM (" + consultaSinOrden + ") auxPaginacion " + pOrderBy;
            }
            else
            {
                datos = "select MasterRowNums.* from  ( select *, ROW_NUMBER() over (" + pOrderBy + ") as RowNum from (" + consultaSinOrden + ")as Consulta )MasterRowNums where RowNum between " + pInicio + " and " + pLimite;
            }

            return new KeyValuePair<string, string>(count, datos);
        }

        /// <summary>
        /// Devuelve un KeyValuePair(consulta count, consulta resultados) con las dos consultas necesarias para realizar la paginación 
        /// de una consulta jerárquica (la primera devuelve el numero de resultados totales y la segunda los x resultados seleccionados)
        /// </summary>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jerárquica que vamos a paginar</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <returns>KeyValuePair(consulta count, consulta resultados)</returns>
        public KeyValuePair<string, string> ObtenerConsultasPaginarConsultaJerarquica(DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, int pInicio, int pLimite)
        {
            ConsultaJerarquicaSqlServer consultaJerarquica = (ConsultaJerarquicaSqlServer)pConsultaJerarquica;
            string sql = consultaJerarquica.ParteSelect;
            string consultaSinOrden = sql.Replace(pOrderBy, "");

            //Calculo el total de filas con las que trabaja la consulta            
            string count = consultaJerarquica.ParteWith + "Select count(*) from (" + consultaSinOrden + ") as Contar";

            //Cargo el DataSet paginandolo, solo me traigo las filas entre Inicio y Límite           
            string datos = consultaJerarquica.ParteWith + "select MasterRowNums.* from  ( select *, ROW_NUMBER() over (" + pOrderBy + ") as RowNum from (" + consultaSinOrden + ")as Consulta )MasterRowNums where RowNum between " + pInicio + " and " + pLimite;

            return new KeyValuePair<string, string>(count, datos);
        }

        /// <summary>
        /// Captura la fecha y hora del servidor SQLServer
        /// </summary>
        /// <returns>Cadena de texto con la fecha</returns>
        public string CapturarFecha()
        {
            return " GETDATE() ";
        }

        /// <summary>
        /// Captura la fecha (Dia,Mes,Año) del servidor SQLServer
        /// </summary>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,Año) del sistema</returns>
        public string CapturarFechaSinHora()
        {
            return "Convert(Char(10), getdate(),112)";
        }

        /// <summary>
        /// Captura la fecha (Dia,Mes,Año) de un campo de tipo Fecha pasado por parámetro
        /// </summary>
        /// <param name="pColumnaFecha">Nombre de la columna que guarda la fecha</param>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,Año)</returns>
        public string CapturarFechaSinHora(string pColumnaFecha)
        {
            return "Convert(Char(10), " + pColumnaFecha + ",112)";
        }

        /// <summary>
        /// Obtiene el símbolo para concatenar en SQLServer
        /// </summary>
        /// <returns>Cadena de texto con el símbolo para concatenar en SQLServer</returns>
        public string SimboloConcatenar()
        {
            return "+";
        }

        /// <summary>
        /// Devuelve la fecha y hora actual del servidor SQLServer
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <returns>DateTime</returns>
        public DateTime FechaHoraServidor(Database pDatabase)
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Captura sólo el año de un campo datetime de SQLServer
        /// </summary>
        /// <param name="pCampo">Nombre del campo del que extraer la fecha</param>
        /// <returns>Cadena de texto con el año</returns>
        public string AñoCampo(string pCampo)
        {
            return "YEAR(" + pCampo + ")";
        }

        /// <summary>
        /// Conversión y tratamiento del tipo Guid. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        public DbType TipoGuidToObject(DbType pTipo)
        {
            return pTipo;
        }

        /// <summary>
        /// Conversión y tratamiento del tipo Binary. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        public DbType TipoBinaryToObject(DbType pTipo)
        {
            return pTipo;
        }

        /// <summary>
        /// Devuelve una cadena con en formato correcto para que la BD pueda realizar una comparacion de valores para usar en las consultas delete y modify de los dataAdapter
        /// </summary>
        /// <param name="pNombreCampo">Nombre del campo </param>
        /// <param name="pAdmiteNulos">True si admite nulos dicho campo</param>
        /// <returns>Cadena con en formato de una comparacion de valores para usar en las consultas delete y modify de los dataAdapter </returns>
        public String ComparacionCamposImagenesConOriginal(String pNombreCampo, bool pAdmiteNulos)
        {
            if (pAdmiteNulos)
            {
                return "(" + pNombreCampo + " = @O_" + pNombreCampo + " OR @O_" + pNombreCampo + " IS NULL AND " + pNombreCampo + " IS NULL)";
            }
            else
            {
                return "(" + pNombreCampo + " = @O_" + pNombreCampo + " )";
            }
        }

        /// <summary>
        /// Devuelve una cadena con en formato correcto para que la BD pueda realizar una comparacion de valores para usar en las consultas delete y modify de los dataAdapter con campos de texto pesado
        /// </summary>
        /// <param name="pNombreCampo">Nombre del campo </param>
        /// <param name="pAdmiteNulos">True si admite nulos dicho campo</param>
        /// <returns>Cadena con en formato de una comparacion de valores para usar en las consultas delete y modify de los dataAdapter </returns>
        public String ComparacionCamposTextoPesadoConOriginal(String pNombreCampo, bool pAdmiteNulos)
        {
            if (pAdmiteNulos)
            {
                return "(" + pNombreCampo + " = @O_" + pNombreCampo + " OR @O_" + pNombreCampo + " IS NULL AND " + pNombreCampo + " IS NULL)";
            }
            else
            {
                return "(" + pNombreCampo + " = @O_" + pNombreCampo + " )";
            }
        }

        /// <summary>
        /// Método para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <param name="pConAs">True si deseamos renombrar el campo con la clausula "as"</param>
        /// <returns>Cadena de texto formateada</returns>
        public string CargarGuid(string pCampo, bool pConAs)
        {
            return pCampo;
        }

        /// <summary>
        /// Método para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        public string CargarGuid(string pCampo)
        {
            return CargarGuid(pCampo, true);
        }

        /// <summary>
        /// Añade el símbolo especial de parámetro de SQLServer a la cadena dada
        /// </summary>
        /// <param name="pString">Cadena de texto que será el parámetro</param>
        /// <returns>Cadena de texto con formato de parámetro en SQLServer</returns>
        public string ToParam(string pString)
        {
            if (pString.Trim().Length == 0)
            {
                return String.Empty;
            }
            else
            {
                return "@" + pString;
            }
        }

        /// <summary>
        /// Controla en las consultas Insert,Delete y Update de los DataAdapter el uso de parámetros que son Guid 
        /// </summary>
        /// <param name="pValor">Nombre del parámetro que hay que controlar en las sentencias Insert,Delete y Update del dataAdapter</param>
        /// <returns>Cadena de texto con el parámetro en el formato correcto para SQLServer</returns>
        public string GuidParamColumnaTabla(string pValor)
        {
            return "@" + pValor;
        }

        /// <summary>
        /// Reemplaza los carácteres reservados para la definición de parámetros que no son de tipo Guid, 
        /// aplicándoles el formato correcto para SQLServer
        /// </summary>
        /// <param name="pValor">Cadena de texto con toda la consulta</param>
        /// <returns>Cadena de texto con la consulta formateada</returns>
        public string ReplaceParam(string pValor)
        {
            return pValor;
        }

        /// <summary>
        /// Convierte los caracteres reservados para la definición de parámetros de tipo Guid 
        /// </summary>
        /// <param name="pValor">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con la consulta sustituida</returns>
        public string GuidParamValor(String pValor)
        {
            return "@" + pValor;
        }

        /// <summary>
        /// Devuelve un string para usar en la consulta con el formato correcto del valor del Guid para SQLServer
        /// </summary>
        /// <param name="pValor">Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        public string GuidValor(Guid pValor)
        {
            return "'" + pValor.ToString() + "'";
        }

        /// <summary>
        /// Devuelve un string para usar en la consulta con el formato correcto del valor de una lista de Guid 
        /// para el servidor de base de datos correspondiente
        /// </summary>
        /// <param name="pValor">Lista de Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        public string ListGuidValor(List<Guid> pListaValores)
        {
            string valor = "";
            if (pListaValores.Count > 0)
            {
                foreach (Guid idValor in pListaValores)
                {
                    valor += "'" + idValor.ToString() + "',";
                }
                valor = valor.Substring(0, valor.Length - 1);
            }
            return valor;
        }

        /// <summary>
        /// Obtiene de un Reader abierto y una posicion a leer el valor de un guid (o lo formatea para que devuelva un guid)
        /// </summary>
        /// <param name="pReader">Objeto IDataReader</param>
        /// <param name="pPosicionALeerDelReader">indice del campo que se va a buscar</param>
        /// <returns>Guid</returns>
        public Guid ReaderGetGuid(IDataReader pReader, int pPosicionALeerDelReader)
        {
            return pReader.GetGuid(pPosicionALeerDelReader);
        }

        /// <summary>
        /// Devuelve una cadena con la sentencia para desactivar una FK habilitada
        /// </summary>
        /// <param name="pTable">Nombre de la tabla en la BD</param>
        /// <param name="pNombreFK">Nombre de la FK en la BD</param>
        /// <returns>Sentencia SQL para realizar el cambio</returns>
        public string DesactivarFK(string pTable, string pNombreFK)
        {
            return "ALTER TABLE " + pTable + " NOCHECK CONSTRAINT " + pNombreFK;
        }

        /// <summary>
        /// Devuelve una cadena con la sentencia para activar una FK desabilitada
        /// </summary>
        /// <param name="pTable">Nombre de la tabla en la BD</param>
        /// <param name="pNombreFK">Nombre de la FK en la BD</param>
        /// <returns>Sentencia SQL para realizar el cambio</returns>
        public string ActivarFK(string pTable, string pNombreFK)
        {
            return "ALTER TABLE " + pTable + " CHECK CONSTRAINT " + pNombreFK;
        }

        /// <summary>
        /// Conversión y tratamiento del tipo Guid. Para Uso de consultas 
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos Object si estamos con Guids en SQLServer</returns>
        public DbType TipoGuidToString(DbType pTipo)
        {
            return pTipo;
        }

        /// <summary>
        /// Formatea el valor del Guid para que sea compatible con SQLServer, quita los caracteres del Guid  '{' ,'}','-'   
        /// </summary>
        /// <param name="pGuid">Guid</param>
        /// <returns>Guid formateado para SQLServer</returns>
        public object ValorDeGuid(Guid pGuid)
        {
            return pGuid;
        }

        /// <summary>
        /// Formatea un campo de texto en una cadena de caracteres separado por guiones para simular un campo Guid
        /// </summary>
        /// <param name="pCampo">Campo Guid traido como cadena de texto</param>
        /// <returns>Cadena de caracteres separada por guiones simulando un Guid</returns>
        public string FormatearGuid(Guid pCampo, bool pAniadirGuiones = false)
        {
            // David: Como en SQLServer vienen los guids separados correctamente, se devuelve el campo tal cual
            return $"'{pCampo.ToString()}'";
        }

        /// <summary>
        /// Búsqueda de texto avanzada, uso de catálogos en BD
        /// </summary>
        /// <param name="pNombreTablaColumna">Nombre de la tabla + '.' + columna de texto indexada ("Tabla.Columna")</param>
        /// <param name="pClaveprincipal">Nombre de la columna que es clave principal de la tabla</param>
        /// <param name="pParametro">Nombre del parámetro para después asignar el valor para el criterio de búsqueda</param>
        /// <param name="pConParametro">True si el valor del tag se pasa con un parametro, False si se pasa el valor directamente para la busqueda en el pParametro</param>
        /// <returns>Cadena de texto con la consulta de la búsqueda</returns>
        public string Catalogo(string pNombreTablaColumna, string pClaveprincipal, string pParametro, bool pConParametro)
        {
            int pos = pNombreTablaColumna.IndexOf('.');
            string nombreColumna = pNombreTablaColumna.Substring(pos + 1);
            string nombreTabla = pNombreTablaColumna.Substring(0, pos);

            if (pConParametro)
            {
                return "FREETEXTTABLE(" + nombreTabla + "," + nombreColumna + "," + ToParam(pParametro) + ")";
            }
            else
            {
                if (pParametro.Contains(".") && !pParametro.Contains(" "))
                {
                    return "FREETEXTTABLE(" + nombreTabla + "," + nombreColumna + ",'\"" + pParametro + "\"')";
                }
                else
                {
                    return "FREETEXTTABLE(" + nombreTabla + "," + nombreColumna + ",'" + pParametro + "')";
                }
            }
        }

        /// <summary>
        /// Formateado de la consulta que será utilizada para las búsquedas en catálogos de texto
        /// </summary>
        /// <param name="pString">Consulta que será utilizada para las búsquedas en catálogos de texto</param>
        /// <returns>Cadena de texto con la consulta formateada</returns>
        public string CriterioBusqueda(string pString)
        {
            string mayusculas = pString.ToUpper();
            return mayusculas.Replace("DEL", "");
        }


        /// <summary>
        /// Obtiene la cláusula FROM de una consulta sin tablas
        /// </summary>
        /// <returns>La cláusula FROM sin ninguna tabla</returns>
        public string ObtenerFromSinTablas()
        {
            return "";
        }

        /// <summary>
        /// Escribe SQLServer2005--> [key] , Oracle --> Key
        /// </summary>
        /// <returns>[Key] o Key</returns>
        public string Key()
        {
            return "[Key]";
        }

        /// <summary>
        /// Escribe SQLServer2005--> ISNULL() , Oracle --> NVL()
        /// </summary>
        /// <param name="pCampo">Campo que devuelve</param>
        /// <param name="pValorPorDefecto">Valor que devuelve si es null</param>
        /// <returns></returns>
        public string ISNULL(string pCampo, string pValorPorDefecto)
        {
            return "ISNULL(" + pCampo + ", " + pValorPorDefecto + ")";
        }

        public int PaginarDataSetConConsultaJerarquica(Database pDatabase, DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla)
        {
            throw new NotImplementedException();
        }

        public IConsultaJerarquica CrearConsultaJerarquica(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom)
        {
            throw new NotImplementedException();
        }

        public IConsultaJerarquica CrearConsultaJerarquicaDeGrafo(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom, string pNombreTablaElementos, string pClaveTablaElementos)
        {
            throw new NotImplementedException();
        }

        public IConsultaJerarquicaMultiple CrearConsultaJerarquicaMultiple(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, bool pIncluirTablaResultadoEnFrom)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Propiedades

        private static ConcurrentDictionary<string, string> mListaConexionesConCredenciales = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Obtiene la lista de conexiones con credenciales para cada conexion SIN credenciales
        /// </summary>
        public ConcurrentDictionary<string, string> ListaConexionesConCredenciales
        {
            get
            {
                return mListaConexionesConCredenciales;
            }
        }

        public string Concatenador
        {
            get
            {
                return "||";
            }
        }

        public string CadenaVacia
        {
            get
            {
                return "''";
            }
        }

        #endregion
    }
}

