using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;

namespace Es.Riam.Gnoss.Util.General
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración para distinguir tipos Bases de datos
    /// </summary>
    public enum TipoBaseDatos
    {
        /// <summary>
        /// BD de SQL Server
        /// </summary>
        SqlServer = 0,
        /// <summary>
        /// BD de Oracle
        /// </summary>
        Oracle = 1,
        /// <summary>
        /// BD de Postgres
        /// </summary>
        Postgres = 2
    }

    #endregion

    /// <summary>
    /// Emcriptado y guardado de la cadena de conexión
    /// </summary>
    public class Configuracion
    {
        #region Miembros

        /// <summary>
        /// Reuta de los nodos de parámetros en el XML
        /// </summary>
        private const string RUTA_PARAMETRO_XML = @"/ConfigParams/parameter";

        /// <summary>
        /// Noimbre del parámetro de conexión a base de datos
        /// </summary>
        private const string PARAMETRO_CONEXION_BD = "conexionBD";

        /// <summary>
        /// Tipo de base de datos
        /// </summary>
        private const string TIPO_BD = "tipoBD";

        #region Estáticos

        #region Privados

        /// <summary>
        /// Tipo de la base de datos
        /// </summary>
        private static TipoBaseDatos mTipoBD = TipoBaseDatos.SqlServer;

        /// <summary>
        /// Determina si se ha cargado el tipo de BD
        /// </summary>
        private static bool mTipoBDCargado = false;

        /// <summary>
        /// Determina si se debe obtener la cadena de conexión siempre desde el fichero de conexión (NUNCA desde el de configuración, esto es para la WEB)
        /// </summary>
        private static bool mObtenerDesdeFicheroConexion = false;

        #endregion

        #region Públicos

        /// <summary>
        /// Ruta del fichero de configuración
        /// </summary>
        public static string FICHERO_CONFIG = Environment.CurrentDirectory + @"\GnossConfig.xml";

        /// <summary>
        /// Estructura del fichero de configuración
        /// </summary>
        public static string ESTRUCTURA_CONFIG = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + "<ConfigParams>" + Environment.NewLine + "\t<parameter>" + Environment.NewLine + "\t\t<name>" + PARAMETRO_CONEXION_BD + "</name>" + Environment.NewLine + "\t\t<value></value>" + Environment.NewLine + "\t\t<description>Cadena de conexión a la base de datos</description>" + Environment.NewLine + "\t</parameter>" + Environment.NewLine + "\t<parameter>" + Environment.NewLine + "\t\t<name>" + TIPO_BD + "</name>" + Environment.NewLine + "\t\t<value></value>" + Environment.NewLine + "\t\t<description>Tipo de datos</description>" + Environment.NewLine + "\t</parameter>" + Environment.NewLine + "</ConfigParams>";

        #endregion

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Comprueba si la conexión a la Base de Datos está guardada
        /// </summary>
        public bool CadenaConexionGuardada
        {
            get
            {
                return (ObtenerCadenaConexion(null).Trim() != string.Empty);
            }
        }

        /// <summary>
        /// Obtiene o establece si se debe obtener la cadena de conexión siempre desde el fichero de conexión (NUNCA desde el de configuración, esto es para la WEB)
        /// </summary>
        public static bool ObtenerDesdeFicheroConexion
        {
            get
            {
                return mObtenerDesdeFicheroConexion;
            }
            set
            {
                mObtenerDesdeFicheroConexion = value;
            }
        }

        #endregion

        private Conexion _conexion;
       
        public Configuracion(Conexion conexion)
        {
            _conexion = conexion;
        }
        #region Métodos

        #region Métodos de parámetros

        /// <summary>
        /// Obtiene el valor de un parámetro
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parámetro del que se quiere obtener el valor</param>
        /// <returns>Parámetro</returns>
        private static string ObtenerParametro(string pNombreParametro)
        {
            return ObtenerParametro(pNombreParametro, FICHERO_CONFIG);
        }

        /// <summary>
        /// Obtiene el valor de un parámetro
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parámetro del que se quiere obtener el valor</param>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración</param>
        /// <returns>Parámetro</returns>
        private static string ObtenerParametro(string pNombreParametro, string pFicheroConfiguracionBD)
        {
            return UtilXML.LeerValorXml(pFicheroConfiguracionBD, RUTA_PARAMETRO_XML + "[name = '" + pNombreParametro + "']", "value");
        }

        /// <summary>
        /// Guarda un parámetro
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parámetro que se quiere guardar</param>
        /// <param name="pValor">Valor del parámetro</param>
        private void GuardarParametro(string pNombreParametro, string pValor)
        {
            UtilXML.GuardarValorXml(FICHERO_CONFIG, RUTA_PARAMETRO_XML + "[name = '" + pNombreParametro + "']", "value", pValor);
        }

        /// <summary>
        /// Guarda un parámetro
        /// </summary>
        /// <param name="pNombreParametro">Nombre del parámetro que se quiere guardar</param>
        /// <param name="pValor">Valor del parámetro</param>
        /// <param name="pFicheroConfiguracion">Ruta del fichero de configuración</param>
        private void GuardarParametro(string pNombreParametro, string pValor, string pFicheroConfiguracion)
        {
            UtilXML.GuardarValorXml(pFicheroConfiguracion, RUTA_PARAMETRO_XML + "[name = '" + pNombreParametro + "']", "value", pValor);
        }

        #endregion

        #region Métodos de cadena de conexión

        /// <summary>
        /// Obtiene la cadena de conexión a la base de datos
        /// </summary>
        /// <param name="pTipoBD">Tipo de BD para la configuración de la BD</param></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        /// <returns></returns>
        public string ObtenerCadenaConexion(string pTipoBD, List<string> pCadenasInvalidas)
        {
            string cadenaConexion = "";

            //Si se está ejecutando desde la web
            if (/*ObtenerDesdeFicheroConexion &&*/ pTipoBD.EndsWith(".xml") && !File.Exists(pTipoBD))
            {
                //pFicheroConfiguracionBD = "config/gnoss.config";
                pTipoBD = "acid";
            }
            cadenaConexion = _conexion.ObtenerCadenaConexion(_conexion.ObtenerUrlFicheroConfigXML(), pTipoBD, pCadenasInvalidas);
            

            return cadenaConexion;
        }

        /// <summary>
        /// Obtiene la cadena de conexión a la base de datos
        /// </summary>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        /// <returns></returns>
        public string ObtenerCadenaConexion(List<string> pCadenasInvalidas)
        {
            return ObtenerCadenaConexion(FICHERO_CONFIG, pCadenasInvalidas);
        }


        #endregion

        #endregion
    }
}
