using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace Es.Riam.Util
{
    /// <summary>
    /// Clase con utilidades para Oracle
    /// </summary>
    public class UtilOracle
    {
        #region Métodos públicos

        /// <summary>
        /// Genera la cadena de conexión a SQL Server
        /// </summary>
        /// <param name="pServidor">Servidor de base de datos</param>
        /// <param name="pSegWindows">Determina si se usa la seguridad de windows</param>
        /// <param name="pUsuario">Usuario del servidor</param>
        /// <param name="pPassword">Password del servidor</param>
        /// <param name="pBD">Base de datos</param>
        /// <param name="pPuerto">Puerto en el que escucha el servicio Oracle</param>
        /// <param name="pNombreServicio">Nombre del servicio Oracle</param>
        /// <returns>Cadena de conexión</returns>
        public static string CadenaConexion(string pServidor, bool pSegWindows, string pUsuario, string pPassword, string pBD, int pPuerto, string pNombreServicio)
        {
            string usuario = "", password = "";
            if (!pSegWindows)
            {
                usuario = pUsuario;
                password = pPassword;
                return "DATA SOURCE=//" + pServidor + ":" + pPuerto + "/" + pNombreServicio + ";User Id=" + usuario + ";Password=" + password + ";";
            }
            else
            {   //no sabemos ni como funciona la seguridad de windows sobre oracle
                return "Data Source=" + pServidor + ":" + pPuerto + ";Integrated Security=" + pSegWindows + ";";
            }
        }

        /// <summary>
        /// Obtiene las bases de datos almacenadas en un servidor SQL Server
        /// </summary>
        /// <param name="pCadena">Cadena de conexión. Debe estar enlazado a la base de datos "master"</param>
        /// <returns>Array con las bases de datos</returns>
        public static string[] ObtenerBasesDatos(string pCadena)
        {
            //// Las bases de datos propias de SQL Server
            //string[] basesSys = { "master", "model", "msdb", "tempdb", "distribution" };
            string[] bases = null;
            //DataTable dt = new DataTable();

            //// La orden T-SQL para recuperar las bases de master
            //string sel = "SELECT name FROM sysdatabases ORDER BY name";

            //SqlConnection con = new SqlConnection();
            //try
            //{
            //    SqlDataAdapter da = new SqlDataAdapter();
            //    con = new SqlConnection(pCadena);
            //    SqlCommand sqlc = new SqlCommand(sel, con);
            //    da.SelectCommand = sqlc;
            //    da.SelectCommand.CommandTimeout = 20;
            //    da.Fill(dt);
            //}
            //catch
            //{
            //    return new string[0];
            //}
            //finally
            //{
            //    if (con != null && con.State == ConnectionState.Open)
            //    {
            //        con.Close();
            //    }
            //}
            //bases = new string[dt.Rows.Count - 1];
            //int k = -1;
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    string s = dt.Rows[i]["name"].ToString();
            //    // Sólo asignar las bases que no son del sistema
            //    if (Array.IndexOf(basesSys, s) == -1)
            //    {
            //        k += 1;
            //        bases[k] = s;
            //    }
            //}
            //if (k == -1) return null;
            //{
            //    int i1_RPbases = bases.Length;
            //    string[] copiaDe_bases = new string[i1_RPbases];
            //    Array.Copy(bases, copiaDe_bases, i1_RPbases);
            //    bases = new string[(k + 1)];
            //    Array.Copy(copiaDe_bases, bases, (k + 1));
            //}
            return bases;
        }

        /// <summary>
        /// Prueba la conexión especificada en el formulario
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>Devuelve true si la conexión funciona</returns>
        public static bool ProbarConexion(string pCadena)
        {
            OracleConnection con = new OracleConnection();
            OracleDataAdapter da = new OracleDataAdapter();
            con = new OracleConnection(pCadena);

            //Compruebo que sea una base de datos GNOSS
            try
            {
                OracleCommand sqlc = new OracleCommand("SELECT VersionGNOSS FROM Version WHERE 1=2");
                sqlc.Connection = con;
                da.SelectCommand = sqlc;
                da.SelectCommand.CommandTimeout = 20;
                con.Open();
                sqlc.ExecuteNonQuery();
            }
            //Excepción para SQl
            catch (System.Data.OracleClient.OracleException x)
            {
                //COMPROBAR NUMERO PARA ORACLE
                //Si es 208 ea que no existen las tablas y no es una base de datos GNOSS
                if (x.Code == 942)
                    throw new Exception("No es una base de datos GNOSS");
                else
                    throw x;
            }
            finally
            {
                if (con != null && con.State == ConnectionState.Open)
                {
                    con.Close();
                    con.Dispose();
                    da.Dispose();
                }
            }
            return true;
        }

        /// <summary>
        /// Obtiene el servidor de bases de datos desde la cadena de conexión
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>Nombre del servidor Oracle</returns>
        public static string ObtenerServidor(string pCadena)
        {
            if (pCadena == "")
                return "";

            int indice = pCadena.IndexOf("DATA SOURCE=//");
            string cadenaConexion = "";
            if (indice != -1)
            {
                int comienzo = indice + "DATA SOURCE=//".Length;

                int longitud = pCadena.IndexOf(':', indice) - comienzo;

                cadenaConexion = pCadena.Substring(comienzo, longitud);
            }
            return cadenaConexion;
        }

        /// <summary>
        /// Obtiene el puerto del servidor desde la cadena de conexión
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>Numero del puerto de Oracle</returns>
        public static int ObtenerPuerto(string pCadena)
        {
            if (pCadena == "")
                return 1521;

            int indice = pCadena.IndexOf(":");
            string puerto = "";
            if (indice != -1)
            {
                int comienzo = indice + ":".Length;
                int longitud = pCadena.IndexOf('/', pCadena.IndexOf(":")) - comienzo;
                puerto = pCadena.Substring(comienzo, longitud);
            }
            return System.Convert.ToInt32(puerto);
        }

        /// <summary>
        /// Obtiene si la cadena de conexión usa la seguridad de windows
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>Devuelve true si usa la seguridad de Windows</returns>
        public static bool ObtenerSeguridadWindows(string pCadena)
        {
            if (pCadena == "")
                return false;
            //TODO: cuando sepamos como funciona la seguridad de Windows en Oracle
            //int indice = pCadena.IndexOf("Trusted_Connection=");
            //bool admite = false;
            //if (indice != -1)
            //{
            //    int comienzo = indice + "Trusted_Connection=".Length;
            //    int longitud = pCadena.IndexOf(';', pCadena.IndexOf("Trusted_Connection=")) - comienzo;
            //    string seg = pCadena.Substring(comienzo, longitud);
            //    admite = seg == "True";
            //}
            return false; ;
        }

        /// <summary>
        /// Obtiene el usuario del servidor de bases de datos desde la cadena de conexión
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>Nombre del usuario del servidor Oracle</returns>
        public static string ObtenerUsuario(string pCadena)
        {
            if (pCadena == "")
                return "";

            int comienzo = pCadena.IndexOf("User Id=") + "User Id=".Length;
            int longitud = pCadena.IndexOf(';', pCadena.IndexOf("User Id=")) - comienzo;
            if (comienzo >= 0 && longitud >= 1)
            {
                return pCadena.Substring(comienzo, longitud);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Obtiene el password del usuario del servidor de bases de datos desde la cadena de conexión
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>Pasword del usuario del servidor Oracle</returns>
        public static string ObtenerPassword(string pCadena)
        {
            if (pCadena == "")
                return "";

            int comienzo = pCadena.IndexOf("Password=") + "Password=".Length;
            int longitud = pCadena.IndexOf(';', pCadena.IndexOf("Password=")) - comienzo;
            if (comienzo >= 0 && longitud >= 1)
            {
                return pCadena.Substring(comienzo, longitud);
            }
            else
            {
                return "";
            }
        }

        ///// <summary>
        ///// Obtiene la bases de datos desde la cadena de conexión
        ///// </summary>
        ///// <param name="pCadena">Cadena de conexión</param>
        ///// <returns>Nombre de la base de datos</returns>
        //public static string ObtenerBD(string pCadena)
        //{
        //    if (pCadena == "")
        //    {
        //        return "";
        //    }
        //    int comienzo = pCadena.IndexOf("Initial Catalog=") + "Initial Catalog=".Length;
        //    int longitud = pCadena.IndexOf(';', pCadena.IndexOf("Initial Catalog=")) - comienzo;
        //    return pCadena.Substring(comienzo, longitud);
        //}

        /// <summary>
        /// Obtiene el nombre del servicio de oracle desde la cadena de conexión
        /// </summary>
        /// <param name="pCadena">Cadena de conexión</param>
        /// <returns>nombre del servicio de oracle</returns>
        public static string ObtenerServicio(string pCadena)
        {
            if (pCadena == "")
                return "";

            int indice = pCadena.IndexOf(":");
            string aux = pCadena.Substring(indice);
            int comienzo = aux.IndexOf("/") + "/".Length;
            aux = aux.Substring(comienzo);

            return aux.Substring(0, aux.IndexOf(";"));
        }

        #endregion
    }
    
}
