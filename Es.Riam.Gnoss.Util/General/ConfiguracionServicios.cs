using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Es.Riam.Gnoss.Util.General
{
    public class ConfiguracionServicios
    {
        #region Miembros

        private readonly string FICHERO_CONFIGURACION = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "config" + Path.DirectorySeparatorChar + "service.xml";

        protected XmlDocument mRaizXml;
        private Dictionary<String, String> mParametrosServicio;
        protected Dictionary<string, Dictionary<string, string>> mParametrosNombreConexion;
        protected Dictionary<string, Dictionary<string, string>> mParametrosArchivoConexion;

        protected List<string> mFicherosConfiguracionBD;
        private List<string> mNombresConexiones;
        protected Dictionary<string, List<KeyValuePair<string, int>>> mIPsRedis;

        protected Dictionary<string, int> mPuertoUDP;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ConfiguracionServicios()
        {
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista con las rutas de los ficheros de configuración de las conexiones a las BBDD
        /// </summary>
        public List<string> FicherosConfiguracionBD
        {
            get
            {
                if (mFicherosConfiguracionBD == null)
                {
                    this.CargarFicherosConfiguracionBD();
                }
                return mFicherosConfiguracionBD;
            }
        }

        /// <summary>
        /// Obtiene la lista con las rutas de los ficheros de configuración de las conexiones a las BBDD
        /// </summary>
        public Dictionary<string, int> PuertoUDP
        {
            get
            {
                if (mPuertoUDP == null)
                {
                    this.CargarPuertosFicherosConfiguracionBD();
                }
                return mPuertoUDP;
            }
        }

        /// <summary>
        /// Obtiene la lista con las rutas de los ficheros de configuración de las conexiones a las BBDD
        /// </summary>
        public List<string> NombresConexiones
        {
            get
            {
                if (mNombresConexiones == null)
                {
                    this.CargarNombresConexiones();
                }
                return mNombresConexiones;
            }
        }

        /// <summary>
        /// Devuelve los parametros de configuración de "services.xml" referidos a parámetros del servicio
        /// Si el fichero de configuración contiene: 
        ///     <!--services-parameters>
        ///         <logfile value="C:\gnosservice.log" />
        ///     <services-parameters-->        
        /// Para acceder al valor del fichero de log (logfile), se procedería como sigue:
        ///     objeto.ParametrosServicio["logfile"];
        /// </summary>
        public Dictionary<String, String> ParametrosServicio
        {
            get
            {
                if (this.mParametrosServicio == null)
                {
                    this.CargarParametrosServicio();
                }
                return this.mParametrosServicio;
            }
        }


        public Dictionary<string, Dictionary<string, string>> ParametrosNombreConexion
        {
            get
            {
                if (this.mParametrosNombreConexion == null)
                {
                    this.CargarParametrosConexion();
                }
                return this.mParametrosNombreConexion;
            }
        }

        public Dictionary<string, Dictionary<string, string>> ParametrosArchivoConexion
        {
            get
            {
                if (this.mParametrosArchivoConexion == null)
                {
                    this.CargarParametrosConexion();
                }
                return this.mParametrosArchivoConexion;
            }
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Carga el documento "services.xml" en memoria
        /// </summary>
        protected void CargarDocumento()
        {
            CargarDocumento(FICHERO_CONFIGURACION);
        }

        /// <summary>
        /// Carga el documento "services.xml" en memoria
        /// </summary>
        protected void CargarDocumento(string pFicheroConfiguracion)
        {
            this.mRaizXml = new XmlDocument();

            pFicheroConfiguracion = pFicheroConfiguracion.Replace("\\bin\\Debug\\config\\", "\\config\\");

            XmlReader xmlReader = new XmlTextReader(pFicheroConfiguracion);
            this.mRaizXml.Load(xmlReader);
            xmlReader.Close();
        }

        /// <summary>
        /// Carga los parámetros referentes al(los) servicio(s)
        /// </summary>
        private void CargarParametrosServicio()
        {
            if (this.mRaizXml == null)
            {
                this.CargarDocumento();
            }
            this.mParametrosServicio = new Dictionary<string, string>();
            XmlElement xmlParametros = this.mRaizXml["servicio-gnoss"]["parametros-servicio"];

            foreach (XmlNode paramNode in xmlParametros.ChildNodes)
            {
                this.mParametrosServicio.Add(paramNode.Name, paramNode.Attributes["valor"].Value);
            }
        }

        protected virtual void CargarParametrosConexion()
        {
            if (this.mRaizXml == null)
            {
                this.CargarDocumento();
            }

            this.mParametrosNombreConexion = new Dictionary<string, Dictionary<string, string>>();
            this.mParametrosArchivoConexion = new Dictionary<string, Dictionary<string, string>>();

            XmlElement xmlParametros = this.mRaizXml["servicio-gnoss"]["conexiones"];

            if (xmlParametros.ChildNodes != null)
            {
                foreach (XmlNode bdNode in xmlParametros.ChildNodes)
                {
                    if (!bdNode.NodeType.Equals(XmlNodeType.Comment))
                    {
                        string nombreConexion = bdNode["nombre"].Attributes["valor"].Value;
                        string archivoConexion = bdNode["archivoconexion"].Attributes["valor"].Value;
                        this.mParametrosNombreConexion.Add(nombreConexion, new Dictionary<string, string>());
                        this.mParametrosArchivoConexion.Add(archivoConexion, new Dictionary<string, string>());
                        foreach (XmlNode parametroConexion in bdNode.ChildNodes)
                        {
                            if (parametroConexion.Name != "nombre" && parametroConexion.Name != "archivoconexion" && parametroConexion.Attributes != null)
                            {
                                this.mParametrosNombreConexion[nombreConexion].Add(parametroConexion.Name, parametroConexion.Attributes["valor"].Value);
                                this.mParametrosArchivoConexion[archivoConexion].Add(parametroConexion.Name, parametroConexion.Attributes["valor"].Value);
                            }
                        }
                    }
                }
            }
        }

        private void CargarNombresConexiones()
        {
            if (this.mRaizXml == null)
            {
                this.CargarDocumento();
            }
            this.mNombresConexiones = new List<string>();
            XmlElement xmlConexiones = this.mRaizXml["servicio-gnoss"]["conexiones"];

            if (xmlConexiones.ChildNodes != null)
            {
                foreach (XmlNode bdNode in xmlConexiones.ChildNodes)
                {
                    mNombresConexiones.Add(bdNode["nombre"].Attributes["valor"].Value);
                }
            }
        }

        protected virtual void CargarFicherosConfiguracionBDDeFicheroConexion(string pNombreConexion, string pUrlConexion)
        {
            XmlDocument docConfig = ObtenerFicheroConfigWeb(pUrlConexion);
            mFicherosConfiguracionBD.Add(pNombreConexion);
            Conexion.FicherosConfigExternos.Add(pNombreConexion, docConfig);
        }

        protected virtual void CargarFicherosConfiguracionBD()
        {
            if (this.mRaizXml == null)
            {
                this.CargarDocumento();
            }
            this.mFicherosConfiguracionBD = new List<string>();
            XmlElement xmlConexiones = this.mRaizXml["servicio-gnoss"]["conexiones"];

            Conexion.ServicioWindows = true;

            if (xmlConexiones.ChildNodes != null)
            {
                foreach (XmlNode bdNode in xmlConexiones.ChildNodes)
                {
                    if (!bdNode.NodeType.Equals(XmlNodeType.Comment))
                    {
                        mFicherosConfiguracionBD.Add(bdNode["archivoconexion"].Attributes["valor"].Value);
                    }
                }
            }
        }

        private void CargarPuertosFicherosConfiguracionBD()
        {
            if (this.mRaizXml == null)
            {
                this.CargarDocumento();
            }
            this.mPuertoUDP = new Dictionary<string, int>();
            XmlElement xmlConexiones = this.mRaizXml["servicio-gnoss"]["conexiones"];

            if (xmlConexiones.ChildNodes != null)
            {
                foreach (XmlNode bdNode in xmlConexiones.ChildNodes)
                {
                    mPuertoUDP.Add(bdNode["archivoconexion"].Attributes["valor"].Value, int.Parse(bdNode["puertoUDP"].Attributes["valor"].Value));
                }
            }
        }

        /// <summary>
        /// Obtiene el fichero XML de configuración web.
        /// </summary>
        /// <param name="pUrlFichero">Url del fichero</param>
        /// <returns>Fichero XML de configuración web</returns>
        private XmlDocument ObtenerFicheroConfigWeb(string pUrlFichero)
        {
            XmlDocument docConfig = new XmlDocument();
            XmlReader xmlReader = new XmlTextReader(pUrlFichero);
            docConfig.Load(xmlReader);
            xmlReader.Close();
            xmlReader = null;

            return docConfig;
        }

        #endregion
    }
}
