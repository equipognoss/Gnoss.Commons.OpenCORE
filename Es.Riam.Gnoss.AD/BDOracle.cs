using Es.Riam.Gnoss.Util.Configuracion;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Oracle;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;

namespace Es.Riam.Gnoss.AD
{
    /// <summary>
    /// Clase para base de datos Oracle
    /// </summary>
    public class BDOracle : IBaseDatos
    {
        private ConfigService mConfigService;

        public BDOracle(ConfigService configService)
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
        /// Devuelve un string con la cadena de conexión
        /// </summary>
        /// <returns>Cadena de conexión</returns>
        public string ObtenerCadenaConexion()
        {
            return mConfigService.ObtenerSqlConnectionString();
        }

        /// <summary>
        /// Obtiene el símbolo para concatenar en Oracle
        /// </summary>
        /// <returns>Cadena de texto con el símbolo para concatenar en Oracle</returns>
        public string SimboloConcatenar()
        {
            return "||";
        }

        /// <summary>
        /// Devuelve la fecha y hora actual del servidor.
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <returns>DateTime</returns>
        public DateTime FechaHoraServidor(Database pDatabase)
        {
            DbCommand commando = pDatabase.GetSqlStringCommand("SELECT SYSDATE FROM dual");
            DateTime fecha = (DateTime)pDatabase.ExecuteScalar(commando);

            return fecha;
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
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSet(Database pDatabase, DbCommand pComando, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla, bool pContar, string pConsultaLigeraCount)
        {
            string sql = pComando.CommandText;
            if (pConsultaLigeraCount == "")
            {
                pConsultaLigeraCount = pComando.CommandText;
            }

            //calculo el total de filas con las que trabaja la consulta
            pComando.CommandText = "Select count(*) from (" + pConsultaLigeraCount + ")";
            int numero = 0;
            if (pContar)
            {
                numero = System.Convert.ToInt32(pDatabase.ExecuteScalar(pComando));
            }

            if (pInicio == -1 && pLimite == -1)
            {
                //Cargo el DataSet , me traigo TODAS las filas posibles
                pComando.CommandText = "select *  from ( select  a.*, ROWNUM rnum from (" + sql + " ) a where ROWNUM <=  " + numero.ToString() + " ) where rnum  >= 1";
            }
            else
            {
                //Cargo el DataSet paginandolo, solo me traigo las filas entre Inicio y Límite
                pComando.CommandText = "select *  from ( select  a.*, ROWNUM rnum from (" + sql + " ) a where ROWNUM <=  " + pLimite.ToString() + " ) where rnum  >= " + pInicio.ToString();
            }
            pDatabase.LoadDataSet(pComando, pDataSet, pNombreTabla);

            return numero;
        }

        /// <summary>
        /// Obtiene de un Reader abierto y una posicion a leer el valor de un guid (o lo formatea para que devuelva un guid)
        /// </summary>
        /// <param name="pReader">Objeto IDataReader</param>
        /// <param name="pPosicionALeerDelReader">indice del campo que se va a buscar</param>
        /// <returns>Guid</returns>
        public Guid ReaderGetGuid(IDataReader pReader, int pPosicionALeerDelReader)
        {
            return new Guid(pReader.GetValue(pPosicionALeerDelReader).ToString());
        }

        /// <summary>
        /// Devuelve una cadena con la sentencia para desactivar una FK habilitada
        /// </summary>
        /// <param name="pTable">Nombre de la tabla en la BD</param>
        /// <param name="pNombreFK">Nombre de la FK en la BD</param>
        /// <returns>Sentencia SQL para realizar el cambio</returns>
        public string DesactivarFK(string pTable, string pNombreFK)
        {
            return "ALTER TABLE " + pTable + " DISABLE CONSTRAINT " + pNombreFK;
        }

        /// <summary>
        /// Devuelve una cadena con la sentencia para activar una FK desabilitada
        /// </summary>
        /// <param name="pTable">Nombre de la tabla en la BD</param>
        /// <param name="pNombreFK">Nombre de la FK en la BD</param>
        /// <returns>Sentencia SQL para realizar el cambio</returns>
        public string ActivarFK(string pTable, string pNombreFK)
        {
            return "ALTER TABLE " + pTable + " ENABLE CONSTRAINT " + pNombreFK;
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

            //Calculo el total de filas con las que trabaja la consulta
            string count = "Select count(*) from (" + sql + ")";

            //Cargo el DataSet paginandolo, solo me traigo las filas entre Inicio y Límite
            string datos = "select *  from ( select  a.*, ROWNUM rnum from (" + sql + " ) a where ROWNUM <=  " + pLimite.ToString() + " ) where rnum  >= " + pInicio.ToString();

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
            return ObtenerConsultasPaginar(pComando, pOrderBy, pInicio, pLimite);
        }

        /// <summary>
        /// Pagina un dataset pasado por parámetro
        /// </summary>
        /// <param name="pDatabase">Base de datos que se va a consultar</param>
        /// <param name="pComando">Comando con la consulta para paginar</param>
        /// <param name="pConsultaJerarquica">Consulta jerárquica</param>
        /// <param name="pOrderBy">Criterio de ordenación de los resultados</param>
        /// <param name="pDataSet">Dataset</param>
        /// <param name="pInicio">Primera fila que se desea obtener</param>
        /// <param name="pLimite">Última fila que se desea obtener</param>
        /// <param name="pNombreTabla">Nombre de la tabla del dataset que se quiere paginar</param>
        /// <returns>Número de registros del resultado</returns>
        public int PaginarDataSetConConsultaJerarquica(Database pDatabase, DbCommand pComando, IConsultaJerarquica pConsultaJerarquica, string pOrderBy, DataSet pDataSet, int pInicio, int pLimite, string pNombreTabla)
        {
            return PaginarDataSet(pDatabase, pComando, pOrderBy, pDataSet, pInicio, pLimite, pNombreTabla, true, "");
        }

        /// <summary>
        /// Crea una base de datos de Enterprise Library
        /// </summary>
        /// <returns>Base de datos de Enterprise Library</returns>
        public Database CrearBDEnterprise()
        {
            //return new OracleDatabase(Configuracion.ObtenerCadenaConexion(null));
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
        /// Crea una base de datos de Enterprise Library a partir de un fichero de configuración 
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Indica si se puede usar la variable estatica mIBDEstatica o no</param>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        /// <returns>Base de datos</returns>
        public Database CrearBDEnterprise(string pFicheroConfiguracionBD, List<string> pCadenasInvalidas)
        {
            string conexion = mConfigService.ObtenerSqlConnectionString();

            if (string.IsNullOrEmpty(conexion))
            {
                return CrearBDEnterprise();
            }
            else
            {
                //Oracle.ManagedDataAccess.Client
                Database oracleDB = new OracleDatabase(conexion);
                if (string.IsNullOrEmpty(pFicheroConfiguracionBD) || !pFicheroConfiguracionBD.Equals("CargarConfiguracion"))
                {
                    AgregarConexionAListaConexionesConCredenciales(conexion, oracleDB.ConnectionStringWithoutCredentials + pFicheroConfiguracionBD);
                }
                return oracleDB;
            }
        }

        /// <summary>
        /// Crea comando de base de datos tipado
        /// </summary>
        /// <param name="pComandoSql">Texto para la consulta</param>
        /// <returns>Comando de base de datos</returns>
        public DbCommand CrearDbCommand(string pComandoSql)
        {
            return new OracleCommand(pComandoSql, new OracleConnection(mConfigService.ObtenerSqlConnectionString()));
        }

        /// <summary>
        /// Captura la fecha y hora del servidor Oracle
        /// </summary>
        /// <returns>Cadena de texto con la fecha</returns>
        public string CapturarFecha()
        {
            return " SYSDATE ";
        }

        /// <summary>
        /// Captura la fecha (Dia,Mes,Año) del servidor Oracle
        /// </summary>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,Año) del sistema</returns>
        public string CapturarFechaSinHora()
        {
            return "to_date(to_char(sysdate, 'DD/MM/YY'))";
        }

        /// <summary>
        /// Captura la fecha (Dia,Mes,Año) de un campo de tipo Fecha pasado por parámetro
        /// </summary>
        /// <param name="pColumnaFecha">Nombre de la columna que guarda la fecha</param>
        /// <returns>Cadena de texto con la fecha (Dia,Mes,Año)</returns>
        public string CapturarFechaSinHora(string pColumnaFecha)
        {
            return "to_date(to_char(" + pColumnaFecha + ", 'DD/MM/YY'))";
        }

        /// <summary>
        /// Conversión y tratamiento del tipo Guid. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        public DbType TipoGuidToObject(DbType pTipo)
        {
            if (pTipo == DbType.Guid)
            {
                return DbType.Object;
            }
            return pTipo;
        }

        /// <summary>
        /// Conversión y tratamiento del tipo Binary. Para Uso de DataAdapter
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos modificado si fuese necesario</returns>
        public DbType TipoBinaryToObject(DbType pTipo)
        {
            if (pTipo == DbType.Binary)
            {
                return DbType.Object;
            }
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
                return "(dbms_lob.compare(" + pNombreCampo + ",TO_BLOB(:O_" + pNombreCampo + "),4292967295 ,1 ,1) = 0 OR :O_" + pNombreCampo + " IS NULL AND " + pNombreCampo + " IS NULL)";
            }
            else
            {
                return "(dbms_lob.compare(" + pNombreCampo + ",TO_BLOB(:O_" + pNombreCampo + "),4292967295 ,1 ,1) = 0 )";
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
                return "(dbms_lob.compare(" + pNombreCampo + ",TO_NCLOB(:O_" + pNombreCampo + "),4292967295 ,1 ,1) = 0 OR :O_" + pNombreCampo + " IS NULL AND " + pNombreCampo + " IS NULL)";
            }
            else
            {
                return "(dbms_lob.compare(" + pNombreCampo + ",TO_NCLOB(:O_" + pNombreCampo + "),4292967295 ,1 ,1) = 0 )";
            }
        }

        /// <summary>
        /// Conversión y tratamiento del tipo Guid. Para Uso de consultas 
        /// </summary>
        /// <param name="pTipo">Tipo de datos</param>
        /// <returns>Tipo de datos Object si estamos con Guids en Oracle</returns>
        public DbType TipoGuidToString(DbType pTipo)
        {
            if (pTipo == DbType.Guid)
            {
                return DbType.String;
            }
            return pTipo;
        }

        /// <summary>
        /// Método para poder seleccionar un campo de tipo Guid en una consulta SELECT
        /// </summary>
        /// <param name="pCampo">Nombre del campo de tipo Guid</param>
        /// <param name="pConAs">True si deseamos renombrar el campo con la clausula "as"</param>
        /// <returns>Cadena de texto formateada</returns>
        public string CargarGuid(string pCampo, bool pConAs)
        {
            int pos = pCampo.IndexOf('.');

            if (pos == -1)
            {
                if (pConAs)
                {
                    return "rawtohex(" + pCampo + ") as " + pCampo;
                }
                else
                {
                    return "rawtohex(" + pCampo + ") ";
                }
            }
            else
            {
                string nombreCampo = pCampo.Substring(pos + 1);

                if (pConAs)
                {
                    return "rawtohex(" + pCampo + ") as " + nombreCampo;
                }
                else
                {
                    return "rawtohex(" + pCampo + ") ";
                }
            }
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
        /// Añade el símbolo especial de parámetro de Oracle a la cadena dada
        /// </summary>
        /// <param name="pString">Cadena de texto que será el parámetro</param>
        /// <returns>Cadena de texto con formato de parámetro en Oracle</returns>
        public string ToParam(string pString)
        {
            if (pString.Trim().Length == 0)
            {
                return String.Empty;
            }
            else
            {
                return ":" + pString;
            }
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
                return " ( SELECT SCORE(1) as Rank , " + pClaveprincipal + " as Key FROM " + nombreTabla + " WHERE CONTAINS(" + nombreColumna + "," + ToParam(pParametro) + ",1) > 0 ) ";
            }
            else
            {
                return " ( SELECT SCORE(1) as Rank , " + pClaveprincipal + " as Key FROM " + nombreTabla + " WHERE CONTAINS(" + nombreColumna + ",'" + pParametro + "',1) > 0 ) ";
            }
        }

        /// <summary>
        /// Formateado de la consulta que será utilizada para las búsquedas en catálogos de texto
        /// </summary>
        /// <param name="pString">Consulta que será utilizada para las búsquedas en catálogos de texto</param>
        /// <returns>Cadena de texto con la consulta formateada</returns>
        public string CriterioBusqueda(string pString)
        {
            //Quito espacios al principio y final de la frase                         
            while (pString.StartsWith(" ") || pString.EndsWith(" "))
            {
                if (pString.StartsWith(" "))
                {
                    pString.Remove(0, 1);
                }
                //Quito espacios al principio y final de la frase             
                if (pString.EndsWith(" "))
                {
                    pString.Remove(pString.Length - 1, 1);
                }
            }
            //Quito las palabras que no deseo incluir en la busqueda
            string minusculas = pString.ToLower();

            string[] palabras = { "y", "o", "u", "e", "ni", "mi", "mis", "tu", "tus", "su", "sus", "nuestro", "nuestros", "vuestro", "vuestros", "que", "cuanto", "como", "cual", "donde", "cuando", "quien", "qué", "cuánto", "cómo", "cuál", "dónde", "cuándo", "quién", "a", "ante", "bajo", "con", "contra", "de", "del", "desde", "en", "entre", "hacia", "hasta", "para", "por", "segun", "sin", "so", "sobre", "durante", "mediante", "al", "excepto", "salvo" };

            foreach (string palabra in palabras)
            {
                if (minusculas.Contains(palabra))
                {
                    minusculas.Replace(palabra, "");
                }
            }
            //Concateno con ACCUM para que se tenga en cuenta el peso en la busqueda de cada palabra
            return minusculas.Replace(" ", " ACCUM ");
        }

        /// <summary>
        /// Controla en las consultas Insert,Delete y Update de los DataAdapter el uso de parámetros que son Guid 
        /// </summary>
        /// <param name="pValor">Nombre del parámetro que hay que controlar en las sentencias Insert,Delete y Update del dataAdapter</param>
        /// <returns>Cadena de texto con el parámetro en el formato correcto para Oracle</returns>
        public string GuidParamColumnaTabla(string pValor)
        {
            //return "cast(replace(:" + pValor + ",'-') as RAW(16))";
            return "hextoraw(replace(:" + pValor + ",'-'))";
            //return "utl_raw.cast_to_raw(:" + pValor + ")";
            //return "CONVERTIR_GUID(:" + pValor + ")";
        }

        /// <summary>
        /// Reemplaza los carácteres reservados para la definición de parámetros que no son de tipo Guid, 
        /// aplicándoles el formato correcto para Oracle
        /// </summary>
        /// <param name="pValor">Cadena de texto con toda la consulta</param>
        /// <returns>Cadena de texto con la consulta formateada</returns>
        public string ReplaceParam(string pValor)
        {
            return pValor.Replace("@", ":");
        }

        /// <summary>
        /// Convierte los caracteres reservados para la definición de parámetros de tipo Guid 
        /// </summary>
        /// <param name="pValor">Cadena de texto con la consulta para formatear</param>
        /// <returns>Cadena de texto con la consulta sustituida</returns>
        public string GuidParamValor(String pValor)
        {
            return "HEXTORAW(:" + pValor + ")";
        }

        /// <summary>
        /// Devuelve un string para usar en la consulta con el formato correcto del valor del Guid para Oracle
        /// </summary>
        /// <param name="pValor">Guid</param>
        /// <returns>Cadena de texto formateada</returns>
        public string GuidValor(Guid pValor)
        {
            return "HEXTORAW('" + ValorDeGuid(pValor) + "')";
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
                    valor += "HEXTORAW('" + ValorDeGuid(idValor) + "'),";
                }
                valor = valor.Substring(0, valor.Length - 1);
            }
            return valor;
        }

        /// <summary>
        /// Formatea el valor del Guid para que sea compatible con Oracle, quita los caracteres del Guid  '{' ,'}','-'   
        /// </summary>
        /// <param name="pGuid">Guid</param>
        /// <returns>Guid formateado para Oracle</returns>
        public object ValorDeGuid(Guid pGuid)
        {
            return pGuid.ToString().Replace("-", "").Replace("{", "").Replace("}", "").ToUpper();
        }

        /// <summary>
        /// Formatea un campo de texto en una cadena de caracteres separado por guiones para simular un campo Guid
        /// </summary>
        /// <param name="pCampo">Campo Guid traido como cadena de texto</param>
        /// <returns>Cadena de caracteres separada por guiones simulando un Guid</returns>
        public string FormatearGuid(Guid pCampo, bool pAniadirGuiones = false)
        {
            string[] partesGuid = pCampo.ToString().Split('-');
            string guion = "";
            if (pAniadirGuiones)
            {
                guion = "-";
            }

            string nuevoGuid = $"{TransformarParteGuidAGuidOracle(partesGuid[0], 4)}{guion}{TransformarParteGuidAGuidOracle(partesGuid[1], 2)}{guion}{TransformarParteGuidAGuidOracle(partesGuid[2], 2)}{guion}{partesGuid[3]}{guion}{partesGuid[4]}";

            if (!pAniadirGuiones)
            {
                nuevoGuid = $"hextoraw('{nuevoGuid}')";
            }

            return nuevoGuid;
        }

        /// <summary>
        /// Revierte el conjunto de caracteres para formatearlo y adaptarlo a oracle
        /// </summary>
        /// <param name="pParteGuid"></param> Parte del guid a revertir
        /// <param name="pNumeroParesCaracteres"></param> Número de pares de caracteres
        /// <returns></returns>
        public string TransformarParteGuidAGuidOracle(string pParteGuid, int pNumeroParesCaracteres)
        {
            string nuevoGuid = "";
            for (int i = 0; i < pNumeroParesCaracteres; i++)
            {
                string parte = pParteGuid.Substring(i * 2, 2);
                nuevoGuid = parte + nuevoGuid;
            }

            return nuevoGuid;
        }

        /// <summary>
        /// Crea una consulta jerárquica
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en la consulta global</param>
        /// <returns>Consulta jerárquica</returns>
        public IConsultaJerarquica CrearConsultaJerarquica(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom)
        {
            return new ConsultaJerarquicaOracle(pSelectConsultaGlobal, pFromConsultaGlobal, pRestoConsultaGlobal, pColumnaClavePadre, pColumnaClavePadre, pTablaRelacion, pFromConsultaJerarquica, pColumnasTablaResultado, pNombreTablaResultado, pCondicionesJerarquia, pIncluirTablaResultadoEnFrom);
        }

        /// <summary>
        /// Crea una consulta jerárquica de grafo
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en la consulta global</param>
        /// <param name="pNombreTablaElementos">Nombre de la tabla de los elementos originales sin jerarquía</param>
        /// <param name="pClaveTablaElementos">Clave de la tabla de los elementos originales sin jerarquía</param>
        /// <returns>Consulta jerárquica de grafo</returns>
        public IConsultaJerarquica CrearConsultaJerarquicaDeGrafo(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom, string pNombreTablaElementos, string pClaveTablaElementos)
        {
            return CrearConsultaJerarquica(pSelectConsultaGlobal, pFromConsultaGlobal, pRestoConsultaGlobal, pColumnaClavePadre, pColumnaClavePadre, pTablaRelacion, pFromConsultaJerarquica, pColumnasTablaResultado, pNombreTablaResultado, pCondicionesJerarquia, pIncluirTablaResultadoEnFrom);
        }

        /// <summary>
        /// Crea una consulta jerárquica múltiple
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de inculir la tabla de resultados en la consulta global</param>
        /// <returns>Consulta jerárquica múltiple</returns>
        public IConsultaJerarquicaMultiple CrearConsultaJerarquicaMultiple(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, bool pIncluirTablaResultadoEnFrom)
        {
            return new ConsultaJerarquicaMultipleOracle(pSelectConsultaGlobal, pFromConsultaGlobal, pRestoConsultaGlobal, pIncluirTablaResultadoEnFrom);
        }

        /// <summary>
        /// Obtiene la cláusula FROM de una consulta sin tablas
        /// </summary>
        /// <returns>La cláusula FROM sin ninguna tabla</returns>
        public string ObtenerFromSinTablas()
        {
            return "FROM dual";
        }

        /// <summary>
        /// Escribe SQLServer2005--> [key] , Oracle --> Key
        /// </summary>
        /// <returns>[Key] o Key</returns>
        public string Key()
        {
            return "Key";
        }

        /// <summary>
        /// Escribe SQLServer2005--> ISNULL() , Oracle --> NVL()
        /// </summary>
        /// <param name="pCampo">Campo que devuelve</param>
        /// <param name="pValorPorDefecto">Valor que devuelve si es null</param>
        /// <returns></returns>
        public string ISNULL(string pCampo, string pValorPorDefecto)
        {
            return "NVL(" + pCampo + ", " + pValorPorDefecto + ")";
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de conexiones con credenciales para cada conexion SIN credenciales
        /// </summary>
        public ConcurrentDictionary<string, string> ListaConexionesConCredenciales { get; } = new ConcurrentDictionary<string, string>();

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
                return "NULL";
            }
        }

        #endregion
    }

    /// <summary>
    /// Consulta jerárquica para Oracle
    /// </summary>
    public class ConsultaJerarquicaOracle : IConsultaJerarquica
    {
        #region Miembros

        /// <summary>
        /// Consulta jerárquica
        /// </summary>
        private string mConsultaJerarquica;

        #endregion

        #region Constructor

        /// <summary>
        /// Crea una consulta jerárquica para Oracle a partir de los datos pasados por parámetro
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se debe de incluir la tabla de resultados en el FROM de la consulta global</param>
        /// <returns>Consulta jerárquica para Oracle</returns>
        public ConsultaJerarquicaOracle(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pIncluirTablaResultadoEnFrom)
        {
            //Inicializo variables
            string fromConsultaGlobal = string.Empty;
            string caracterComa = string.Empty;

            //Creo la consulta jerárquica
            string consultaJerarquica = CrearConsultaJerarquica(pColumnaClavePadre, pColumnaClaveHija, pTablaRelacion, pFromConsultaJerarquica, pColumnasTablaResultado, pNombreTablaResultado, pCondicionesJerarquia);

            //Compruebo si se debe incluir la tabla de resultado en el from o ya está incluida
            if ((pFromConsultaGlobal == null) || (!string.IsNullOrEmpty(pFromConsultaGlobal)))
            {
                pFromConsultaGlobal = " FROM ";
            }
            else
            {
                caracterComa = ", ";
            }
            //Agrego al from global la consulta jerárquica
            pFromConsultaGlobal += caracterComa + consultaJerarquica;

            string consultaGlobal = pSelectConsultaGlobal + " " + pFromConsultaGlobal + " " + pRestoConsultaGlobal;
            mConsultaJerarquica = consultaGlobal;
        }

        #endregion

        #region Miembros de IConsultaJerarquica

        /// <summary>
        /// Obtiene la consulta jerárquica
        /// </summary>
        public string ConsultaJerarquica
        {
            get
            {
                return this.mConsultaJerarquica;
            }
        }

        #endregion

        #region Métodos generales

        /// <summary>
        /// Crea una consulta jerárquica a partir de los datos pasados por parámetro
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta
        /// jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado 
        /// (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <returns>Cadena de texto con la consulta jerárquica creada</returns>
        public static string CrearConsultaJerarquica(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia)
        {
            //Inicializo variables
            string clavePadre = string.Empty;
            string claveHijo = string.Empty;
            string condicionPadreHijo = string.Empty;
            string listaColumnasResultado = string.Empty;
            string fromConsultaGlobal = string.Empty;
            string caracterComa = string.Empty;

            if (pColumnasTablaResultado != null)
            {
                listaColumnasResultado = pColumnasTablaResultado;
            }
            listaColumnasResultado += caracterComa + pColumnaClavePadre + ", " + pColumnaClaveHija;
            clavePadre += caracterComa + pTablaRelacion + "." + pColumnaClavePadre;
            claveHijo += caracterComa + pTablaRelacion + "." + pColumnaClaveHija;
            condicionPadreHijo += pTablaRelacion + "." + pColumnaClavePadre + " = " + pTablaRelacion + "." + pColumnaClaveHija;

            string fromJerarquico = "";

            if (pFromConsultaJerarquica == null)
            {
                fromJerarquico = " FROM " + pTablaRelacion;
            }
            else
            {
                fromJerarquico = pFromConsultaJerarquica + ", " + pTablaRelacion;
            }
            //Construyo la consulta jerarquica
            string consultaJerarquica = "Select " + listaColumnasResultado + fromJerarquico + " START WITH " + pCondicionesJerarquia + " CONNECT BY PRIOR " + condicionPadreHijo;

            //Agrego al from global la consulta jerárquica
            return "( " + consultaJerarquica + ") " + pNombreTablaResultado;
        }

        #endregion
    }

    /// <summary>
    /// Consulta jerárquica múltiple para Oracle
    /// </summary>
    public class ConsultaJerarquicaMultipleOracle : IConsultaJerarquicaMultiple
    {
        #region Miembros

        /// <summary>
        /// Caracter "," para separar cada una de las consultas jerárquicas
        /// </summary>
        private string mCaracterComa = "";

        /// <summary>
        /// Número de consultas jerárquicas creadas
        /// </summary>
        private int mNumeroConsultas = 0;

        /// <summary>
        /// Parte SELECT de la consulta global
        /// </summary>
        private string mSelectConsultaGlobal;

        /// <summary>
        /// PArte FROM de la consulta global
        /// </summary>
        private string mFromConsultaGlobal;

        /// <summary>
        /// Resto de la consulta global
        /// </summary>
        private string mRestoConsultaGlobal;

        /// <summary>
        /// Indica si se debe de incluir la tabla de resultados en el FROM
        /// </summary>
        private bool mIncluirTablaResultadoEnFrom;

        #endregion

        #region Constructor

        /// <summary>
        /// Crea una consulta jerárquica múltiple para Oracle a partir de los datos pasados por parámetro
        /// </summary>
        /// <param name="pSelectConsultaGlobal">SELECT de la consulta que va a usar la consulta global</param>
        /// <param name="pFromConsultaGlobal">FROM de la consulta que va a usar la consulta global</param>
        /// <param name="pRestoConsultaGlobal">Resto de la consulta que va a usar la consulta global</param>
        /// <param name="pIncluirTablaResultadoEnFrom">TRUE si se incluye la tabla de resultados en el FROM de la consulta global</param>
        /// <returns>Consulta jerárquica múltiple para Oracle</returns>
        public ConsultaJerarquicaMultipleOracle(string pSelectConsultaGlobal, string pFromConsultaGlobal, string pRestoConsultaGlobal, bool pIncluirTablaResultadoEnFrom)
        {
            //Inicializo variables
            mSelectConsultaGlobal = pSelectConsultaGlobal;
            mFromConsultaGlobal = pFromConsultaGlobal;
            mRestoConsultaGlobal = pRestoConsultaGlobal;
            mIncluirTablaResultadoEnFrom = pIncluirTablaResultadoEnFrom;

            if (!pFromConsultaGlobal.Trim().Equals(string.Empty))
            {
                mCaracterComa = ", ";
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece el carácter "," para separar cada una de las tablas virtuales
        /// </summary>
        private string CaracterComa
        {
            get
            {
                return mCaracterComa;
            }
            set
            {
                mCaracterComa = "";
            }
        }

        #endregion

        #region Miembros de IConsultaJerarquica

        /// <summary>
        /// Obtiene la consulta jerárquica múltiple
        /// </summary>
        public string ConsultaJerarquicaMultiple
        {
            get
            {
                return mSelectConsultaGlobal + " " + mFromConsultaGlobal + " " + mRestoConsultaGlobal;
            }
        }

        /// <summary>
        /// Agrega una consulta jerárquica
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en en el FROM de la consulta
        /// jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado 
        /// (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        public void AgregarConsultaJerarquica(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia)
        {
            string consultaJerarquica = ConsultaJerarquicaOracle.CrearConsultaJerarquica(pColumnaClavePadre, pColumnaClaveHija, pTablaRelacion, pFromConsultaJerarquica, pColumnasTablaResultado, pNombreTablaResultado, pCondicionesJerarquia);

            //Agrego al from global la consulta jerárquica
            mFromConsultaGlobal += CaracterComa + consultaJerarquica;

            mNumeroConsultas++;
            mCaracterComa = ", ";
        }

        /// <summary>
        /// Agrega una consulta jerárquica de grafo
        /// </summary>
        /// <param name="pColumnaClavePadre">Columna que contiene la clave del padre (EJ: PadreID)</param>
        /// <param name="pColumnaClaveHija">Columna que contiene la clave de los hijos (EJ: ElementoID)</param>
        /// <param name="pTablaRelacion">Tabla que relaciona a padres e hijos</param>
        /// <param name="pFromConsultaJerarquica">Cadena de texto con las tablas que se desean incluir en el FROM de la consulta 
        /// jerárquica (NULL si no se necesitan más tablas que la tabla de relación)</param>
        /// <param name="pColumnasTablaResultado">Columnas que se desean incluir en la tabla de resultado 
        /// (NULL si sólo se necesitan las columnas pColumnaClavePadre y pColumnaClaveHija)</param>
        /// <param name="pNombreTablaResultado">Nombre de la tabla virtual resultante</param>
        /// <param name="pCondicionesJerarquia">Condición de inicio de la recursividad (Ej: PadreID IS NULL)</param>
        /// <param name="pEsGrafo">TRUE si la consulta jerárquica es un grafo (no un árbol)</param>
        /// <param name="pNombreTablaElementos">Nombre de la tabla de los elementos originales sin jerarquía</param>
        /// <param name="pClaveTablaElementos">Clave de la tabla de los elementos originales sin jerarquía</param>
        public void AgregarConsultaJerarquicaDeGrafo(string pColumnaClavePadre, string pColumnaClaveHija, string pTablaRelacion, string pFromConsultaJerarquica, string pColumnasTablaResultado, string pNombreTablaResultado, string pCondicionesJerarquia, bool pEsGrafo, string pNombreTablaElementos, string pClaveTablaElementos)
        {
            AgregarConsultaJerarquica(pColumnaClavePadre, pColumnaClaveHija, pTablaRelacion, pFromConsultaJerarquica, pColumnasTablaResultado, pNombreTablaResultado, pCondicionesJerarquia);
        }

        #endregion
    }
}
