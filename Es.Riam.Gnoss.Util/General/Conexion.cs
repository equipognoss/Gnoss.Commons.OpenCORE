using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;

namespace Es.Riam.Gnoss.Util.General
{
    /// <summary>
    /// Métodos de utilidad para realizar la conexión con la base de datos
    /// </summary>
    public class Conexion
    {
        private readonly IHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UtilPeticion _utilPeticion;
        private readonly LoggingService _loggingService;
        private readonly Usuario _usuario;

        public Conexion(IHttpContextAccessor httpContextAccessor, IHostEnvironment env, UtilPeticion utilPeticion, LoggingService loggingService, Usuario usuario)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
            _utilPeticion = utilPeticion;
            _loggingService = loggingService;
            _usuario = usuario;
        }
        #region Miembros estáticos

        /// <summary>
        /// Almacena la lista de claves leídas desde los ficheros de configuración. 
        /// La estructura es nombreFichero -> Diccionario(claveParametro, valor)
        /// </summary>
        private static volatile ConcurrentDictionary<string, ConcurrentDictionary<string, string>> mListaClaves = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

        private static volatile ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>> mListaClavesCompuestas = new ConcurrentDictionary<string, ConcurrentDictionary<string, List<string>>>();

        private static volatile ConcurrentDictionary<string, string> mContenidoFicheros = new ConcurrentDictionary<string, string>();
        private static volatile ConcurrentDictionary<string, DateTime> mUltimaLecturaFicheros = new ConcurrentDictionary<string, DateTime>();

        private static volatile ConcurrentDictionary<string, Dictionary<string, string>> mListaIdiomas = new ConcurrentDictionary<string, Dictionary<string, string>>();

        private static volatile ConcurrentDictionary<string, Dictionary<string, List<string>>> mListaServicios = new ConcurrentDictionary<string, Dictionary<string, List<string>>>();

        private static volatile ConcurrentDictionary<string, string> mMimeType = new ConcurrentDictionary<string, string>();

        private static char[] mSeparador = new char[] { ',' };
        /// <summary>
        /// Indica si estamos usando un servicio Windows o no.
        /// </summary>
        private static bool mServicioWindows = false;

        /// <summary>
        /// Indica si estamos usando un servicio Windows o no.
        /// </summary>
        private static bool mServicioWeb = false;

        /// <summary>
        /// Ficheros de configurarción externos cargados con el elemento raiz del archivo de configuración.
        /// </summary>
        private static Dictionary<string, XmlDocument> mFicherosConfigExternos = null;

        private static string mRutaFicheroConfigBD = "";

        #endregion

        #region Métodos públicos

        /// <summary>
        /// Carga los mimeType cargador por el fichero mimeType.txt
        /// </summary>
        public static void CargarMimeType()
        {
            string ficheroVersion = "Config/mimeType.txt";

            System.IO.StreamReader sr = new System.IO.StreamReader(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + ficheroVersion);
            while (!sr.EndOfStream)
            {
                string linea = sr.ReadLine();
                if (linea.Contains("|"))
                {
                    string[] delimiter = { "|" };
                    string[] separacion = linea.Split(delimiter, StringSplitOptions.None);
                    if (separacion[0].Contains(".") && !mMimeType.ContainsKey(separacion[0]) && !string.IsNullOrEmpty(separacion[1]))
                    {
                        mMimeType.TryAdd(separacion[0], separacion[1]);
                    }
                }
            }
            sr.Close();
        }

        public static ConcurrentDictionary<string, string> ObtenerMimeType()
        {
            return mMimeType;
        }

        /// <summary>
        /// Obtiene la lista de idiomas de la forma (es, Español)
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerListaIdiomas()
        {
            string ficheroConexion = (string)_utilPeticion.ObtenerObjetoDePeticion("FicheroConexion");

            return mListaIdiomas[ficheroConexion];
        }

        public static string ObtenerMetaDescription()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obtiene la lista de idiomas de la forma (es, Español)
        /// </summary>
        /// <returns></returns>
        public void AgregarListaIdiomas(Dictionary<string, string> pListaIdiomas)
        {
            string ficheroConexion = (string)_utilPeticion.ObtenerObjetoDePeticion("FicheroConexion");

            if (!mListaIdiomas.ContainsKey(ficheroConexion))
            {
                mListaIdiomas.TryAdd(ficheroConexion, pListaIdiomas);
            }
            else
            {
                mListaIdiomas[ficheroConexion] = pListaIdiomas;
            }
        }

        /// <summary>
        /// Obtiene la lista de idiomas de la forma (es, Español)
        /// </summary>
        /// <returns></returns>
        public string ObtenerServicio(string pNombreServicio, Guid pProyectoID, string pDominio)
        {
            List<string> listaUrls = new List<string>();

            string ficheroConexion = (string)_utilPeticion.ObtenerObjetoDePeticion("FicheroConexion");

            string clave = "";
            if (!pProyectoID.Equals(Guid.Empty))
            {
                clave = pNombreServicio + "_" + pProyectoID.ToString();
                if (mListaServicios[ficheroConexion].ContainsKey(clave))
                {
                    listaUrls = mListaServicios[ficheroConexion][clave];
                }
            }

            if (!ServicioWindows && !ServicioWeb)
            {

                pDominio = _httpContextAccessor.HttpContext.Request.Host.Host;
            }

            if (!string.IsNullOrEmpty(pDominio))
            {
                clave = pNombreServicio + "_" + pDominio.Replace("http://", "").Replace("https://", "").Trim('/');
                if (listaUrls.Count == 0 && mListaServicios[ficheroConexion].ContainsKey(clave))
                {
                    listaUrls = mListaServicios[ficheroConexion][clave];
                }
            }
            clave = pNombreServicio;

            if (listaUrls.Count == 0 && mListaServicios[ficheroConexion].ContainsKey(clave))
            {
                listaUrls = mListaServicios[ficheroConexion][clave];
            }

            if (listaUrls.Count == 0)
            {
                return "";
            }

            string resultado = "";

            if (listaUrls.Count == 1)
            {
                resultado = listaUrls[0];
            }
            else
            {
                foreach (string url in listaUrls)
                {
                    resultado += url + ",";
                }
            }

            // TODO: Revisar, no todos los servicios de archivos tienen certificado
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Scheme == "https" && !pNombreServicio.Equals("urlApiIntegracionContinua"))
            {
                resultado = resultado.Replace("http://", "https://");
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene la lista de idiomas de la forma (es, Español)
        /// </summary>
        /// <returns></returns>
        public void AgregarListaServicios(Dictionary<string, List<string>> pListaServicios)
        {
            string ficheroConexion = (string)_utilPeticion.ObtenerObjetoDePeticion("FicheroConexion");

            if (!mListaServicios.ContainsKey(ficheroConexion))
            {
                mListaServicios.TryAdd(ficheroConexion, pListaServicios);
            }
            else
            {
                mListaServicios[ficheroConexion] = pListaServicios;
            }
        }

        /// <summary>
        /// Obtiene un valor aleatorio (por RoundRobin) de una lista de parámetros
        /// Ej: config/acid -> cadenaConexion, cadenaConexion....
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pNombreParametro">Nombre del parámetro que envuelve a cada parámetro individual (Ej: acid)</param>
        /// <param name="pNombreParametroInterno">Nombre de los parámetros internos (Ej: cadenaConexion)</param>
        /// <param name="pCadenasInvalidas">Lista de valores que no sirven y se desean obtener otros diferentes (si hay)</param>
        /// <returns></returns>
        public string ObtenerValorAleatorioDeParametroCompuesto(string pFicheroConfiguracionBD, string pNombreParametro, string pNombreParametroInterno, List<string> pValoresInvalidos)
        {
            string resultado = "";
            List<string> listaResultados = ObtenerParametroCompuesto(pFicheroConfiguracionBD, pNombreParametro, pNombreParametroInterno);
            try
            {
                if (listaResultados != null)
                {
                    //Cálculo por RoundRobin
                    string claveLista = pFicheroConfiguracionBD + "_" + pNombreParametro + "_" + pNombreParametroInterno;

                    RoundRobin roundRobin = RoundRobin.ObtenerRoundRobinDeClave(claveLista, listaResultados.Count, 60);

                    if (roundRobin.FechaCaducidad < DateTime.Now)
                    {
                        //Si la hora de caducidad ha pasado, lo vuelvo a leer del fichero de configuración
                        List<string> listaResultadosNuevo = ObtenerParametroCompuesto(pFicheroConfiguracionBD, pNombreParametro, pNombreParametroInterno, false);

                        //Si por alguna razón se ha leído mal y viene sin valores, sigo con el listado antiguo. 
                        if (listaResultadosNuevo.Count > 0)
                        {
                            listaResultados = listaResultadosNuevo;
                        }

                        //Ha podido cambiar el número de elementos de la lista, actualizo el RoundRobin
                        roundRobin.ActualizarRoundRobin(listaResultados.Count, 60);
                    }

                    //Devuelvo el siguiente elemento de la lista
                    resultado = roundRobin.ObtenerSiguienteElemento(listaResultados, pValoresInvalidos);
                }
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex);
            }

            if (string.IsNullOrEmpty(resultado) && listaResultados != null && listaResultados.Count > 0)
            {
                resultado = listaResultados[0];
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene un parámetro que se compone de una lista de parámetros
        /// Ej: urlsContent -> urlContent, urlContent....
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pNombreParametro">Nombre del parámetro que envuelve a cada parámetro individual (Ej: urlsContent)</param>
        /// <param name="pNombreParametroInterno">Nombre de los parámetros internos (Ej: urlContent)</param>
        /// <returns></returns>
        public List<string> ObtenerParametroCompuesto(string pFicheroConfiguracionBD, string pNombreParametro, string pNombreParametroInterno)
        {
            return ObtenerParametroCompuesto(pFicheroConfiguracionBD, pNombreParametro, pNombreParametroInterno, true);
        }

        /// <summary>
        /// Obtiene un parámetro que se compone de una lista de parámetros
        /// Ej: urlsContent -> urlContent, urlContent....
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pNombreParametro">Nombre del parámetro que envuelve a cada parámetro individual (Ej: urlsContent)</param>
        /// <param name="pNombreParametroInterno">Nombre de los parámetros internos (Ej: urlContent)</param>
        /// <param name="pObtenerDeListaEstatica">Verdad si se va a obtener el valor de la lista estática, falso si se quiere leer del fichero de configuración</param>
        /// <returns></returns>
        public List<string> ObtenerParametroCompuesto(string pFicheroConfiguracionBD, string pNombreParametro, string pNombreParametroInterno, bool pObtenerDeListaEstatica)
        {
            List<string> resultado = new List<string>();
            try
            {
                if (!mListaClavesCompuestas.ContainsKey(pFicheroConfiguracionBD))
                {
                    mListaClavesCompuestas.TryAdd(pFicheroConfiguracionBD, new ConcurrentDictionary<string, List<string>>());
                }

                if (pObtenerDeListaEstatica && mListaClavesCompuestas[pFicheroConfiguracionBD].ContainsKey(pNombreParametro + pNombreParametroInterno))
                {
                    return mListaClavesCompuestas[pFicheroConfiguracionBD][pNombreParametro + pNombreParametroInterno];
                }

                LeerParametrosConfiguracion(pFicheroConfiguracionBD, pNombreParametro, pNombreParametroInterno, resultado);

                if (!mListaClavesCompuestas[pFicheroConfiguracionBD].ContainsKey(pNombreParametro + pNombreParametroInterno))
                {
                    try
                    {
                        mListaClavesCompuestas[pFicheroConfiguracionBD].TryAdd(pNombreParametro + pNombreParametroInterno, resultado);
                    }
                    catch (Exception e)
                    {
                        _loggingService.GuardarLogError(e);
                    }
                }
                else
                {
                    List<string> listaOriginal = mListaClavesCompuestas[pFicheroConfiguracionBD][pNombreParametro + pNombreParametroInterno];
                    if (!listaOriginal.Equals(resultado) && resultado.Count > 0)
                    {
                        mListaClavesCompuestas[pFicheroConfiguracionBD][pNombreParametro + pNombreParametroInterno] = resultado;
                    }
                }
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, string.Format("Error al obtener parametro compuesto. pFicheroConfiguracionBD {0}, pNombreParametro {1}, pNombreParametroInterno {2}, pObtenerDeListaEstatica {3}", pFicheroConfiguracionBD, pNombreParametro, pNombreParametroInterno, pObtenerDeListaEstatica));
                throw;
            }
            return resultado;
        }



        private void LeerParametrosConfiguracion(string pFicheroConfiguracionBD, string pNombreParametro, string pNombreParametroInterno, List<string> resultado)
        {
            if (UsarVariablesEntorno)
            {
                string variable = Environment.GetEnvironmentVariable(UtilCadenas.ReemplazarCadenaCaracteresNoAlfanumericos($"{pNombreParametro}_{pNombreParametroInterno}", "_"));
                if (!string.IsNullOrEmpty(variable))
                {
                    resultado.AddRange(variable.Split(mSeparador, StringSplitOptions.RemoveEmptyEntries));
                }
            }
            else
            {
                LeerFicheroConfiguracionXML(pFicheroConfiguracionBD, pNombreParametro, pNombreParametroInterno, resultado);
            }
        }

        private void LeerFicheroConfiguracionXML(string pFicheroConfiguracionBD, string pNombreParametro, string pNombreParametroInterno, List<string> resultado)
        {
            XmlDocument docXml = null;
            if (!pFicheroConfiguracionBD.StartsWith("http"))
            {
                string fichero = null;


                //Si empiezan C:\\ es que en pFicheroConfiguracionBD viene la ruta entera
                if (!pFicheroConfiguracionBD.Substring(1, 2).Equals(":\\"))
                {
                    //Si empiezan C:\\ es que en pFicheroConfiguracionBD viene la ruta entera
                    if (!pFicheroConfiguracionBD.Substring(1, 2).Equals(":\\"))
                    {
                        fichero = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + pFicheroConfiguracionBD;
                    }
                    else
                    {
                        fichero = pFicheroConfiguracionBD;
                    }
                }
                else
                {
                    fichero = pFicheroConfiguracionBD;
                }


                //if (pFicheroConfiguracionBD.StartsWith(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Substring(0, 10)))
                //{
                //    fichero = pFicheroConfiguracionBD;
                //}
                if (!ServicioWindows && pFicheroConfiguracionBD.ToLower().Contains("bd.config") && (!string.IsNullOrEmpty(mRutaFicheroConfigBD) || (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)))
                {
                    fichero = RutaFicheroConfigBD;
                }

                try
                {
                    if (ServicioWindows && FicherosConfigExternos != null && FicherosConfigExternos.ContainsKey(pFicheroConfiguracionBD))
                    {
                        docXml = FicherosConfigExternos[pFicheroConfiguracionBD];
                    }
                    else
                    {
                        docXml = new XmlDocument();
                        docXml.Load(fichero);
                    }
                }
                catch (Exception e)
                {
                    _loggingService.GuardarLogError(e);
                }
            }
            else
            {
                byte[] byteArray = Encoding.ASCII.GetBytes(LeerFicheroConfigWeb(pFicheroConfiguracionBD, 0));
                MemoryStream stream = new MemoryStream(byteArray);
                docXml = new XmlDocument();
                docXml.Load(stream);
            }

            try
            {
                XmlElement parametros = (XmlElement)docXml.GetElementsByTagName(pNombreParametro)[0];

                if (parametros != null)
                {
                    foreach (XmlElement parametro in parametros.GetElementsByTagName(pNombreParametroInterno))
                    {
                        if (!resultado.Contains(parametro.InnerText) && !string.IsNullOrEmpty(parametro.InnerText))
                        {
                            resultado.Add(parametro.InnerText);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _loggingService.GuardarLogError(e);
            }
        }

        /// <summary>
        /// Devuelve un string con el código xml leído desde la url pasada como parámetro
        /// </summary>
        /// <param name="pUrl">URL de la que se obtiene el xml</param>
        /// <returns>Contenido del fichero XML</returns>
        private string LeerFicheroConfigWeb(string pUrl, int pNumeroReintentos)
        {
            if (!mContenidoFicheros.ContainsKey(pUrl) || !mUltimaLecturaFicheros.ContainsKey(pUrl) || DateTime.Now.Subtract(mUltimaLecturaFicheros[pUrl]).TotalSeconds > 60)
            {
                WebResponse myResponse = null;
                Stream receiveStream = null;
                StreamReader readStream = null;
                try
                {
                    HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(pUrl);
                    myRequest.Method = "GET";
                    myResponse = myRequest.GetResponse();
                    receiveStream = myResponse.GetResponseStream();
                    readStream = new StreamReader(receiveStream);
                    try
                    {
                        string contenido = readStream.ReadToEnd();
                        if (!mContenidoFicheros.ContainsKey(pUrl))
                        {
                            mContenidoFicheros.TryAdd(pUrl, contenido);
                            mUltimaLecturaFicheros.TryAdd(pUrl, DateTime.Now);
                        }
                        else if (!mContenidoFicheros[pUrl].Equals(contenido))
                        {
                            mContenidoFicheros[pUrl] = contenido;
                            mUltimaLecturaFicheros[pUrl] = DateTime.Now;
                        }
                    }
                    catch (Exception ex)
                    {
                        _loggingService.GuardarLogError(ex);
                        if (!mContenidoFicheros.ContainsKey(pUrl) && pNumeroReintentos < 10)
                        {
                            Thread.Sleep(500);
                            LeerFicheroConfigWeb(pUrl, pNumeroReintentos + 1);
                        }
                    }
                    receiveStream.Close();
                    readStream.Close();
                    myResponse.Close();
                }
                catch (Exception e)
                {
                    _loggingService.GuardarLogError(e);
                }
                finally
                {
                    if (myResponse != null) { myResponse.Close(); }
                    if (receiveStream != null) { receiveStream.Close(); }
                    if (readStream != null) { readStream.Close(); }
                }
            }

            return mContenidoFicheros[pUrl];
        }

        public string ObtenerParametro(string pFicheroConfiguracionBD, string pRutaParametro, bool pDevolverError)
        {
            if (UsarVariablesEntorno)
            {
                return Environment.GetEnvironmentVariable(UtilCadenas.ReemplazarCadenaCaracteresNoAlfanumericos(pRutaParametro, "_"));
            }
            else
            {
                return ObtenerParametroFicheroConfiguracion(pFicheroConfiguracionBD, pRutaParametro, pDevolverError);
            }
        }
        /// <summary>
        /// Obtien el valor de un determinado parámetro
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pRutaParametro">Ruta del parámetro a obtener dentro del árbol XML (Ej: config/urlServicioLogin)</param>
        /// <returns>url</returns>
        public string ObtenerParametroFicheroConfiguracion(string pFicheroConfiguracionBD, string pRutaParametro, bool pDevolverError)
        {
            try
            {
                if (!mListaClaves.ContainsKey(pFicheroConfiguracionBD))
                {
                    mListaClaves.TryAdd(pFicheroConfiguracionBD, new ConcurrentDictionary<string, string>());
                }

                if (mListaClaves[pFicheroConfiguracionBD].ContainsKey(pRutaParametro))
                {
                    return mListaClaves[pFicheroConfiguracionBD][pRutaParametro];
                }
            }
            catch (Exception) { }

            string resultado = "";
            try
            {
                XmlDocument docXml = null;

                if (ServicioWindows && FicherosConfigExternos != null && FicherosConfigExternos.ContainsKey(pFicheroConfiguracionBD))
                {
                    docXml = FicherosConfigExternos[pFicheroConfiguracionBD];
                }
                else
                {
                    if (!pFicheroConfiguracionBD.StartsWith("http"))
                    {
                        string fichero = null;



                        //Si empiezan C:\\ es que en pFicheroConfiguracionBD viene la ruta entera
                        if (!pFicheroConfiguracionBD.Substring(1, 2).Equals(":\\"))
                        {
                            //Si empiezan C:\\ es que en pFicheroConfiguracionBD viene la ruta entera
                            if (!pFicheroConfiguracionBD.Substring(1, 2).Equals(":\\"))
                            {
                                fichero = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + pFicheroConfiguracionBD;
                            }
                            else
                            {
                                fichero = pFicheroConfiguracionBD;
                            }
                        }
                        else
                        {
                            fichero = pFicheroConfiguracionBD;
                        }


                        //Si empiezan igual es que en pFicheroConfiguracionBD viene la ruta entera
                        if (pFicheroConfiguracionBD.StartsWith(AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Substring(0, 10)))
                        {
                            fichero = pFicheroConfiguracionBD;
                        }
                        if (!ServicioWindows && pFicheroConfiguracionBD.ToLower().Contains("bd.config") && (!string.IsNullOrEmpty(mRutaFicheroConfigBD) || (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null)))
                        {
                            fichero = RutaFicheroConfigBD;
                        }

                        fichero = fichero.Replace("\\bin\\Debug\\config\\", "\\config\\");

                        docXml = new XmlDocument();
                        docXml.Load(fichero);
                    }
                    else
                    {
                        byte[] byteArray = Encoding.ASCII.GetBytes(LeerFicheroConfigWeb(pFicheroConfiguracionBD, 0));
                        MemoryStream stream = new MemoryStream(byteArray);

                        docXml = new XmlDocument();
                        docXml.Load(stream);
                    }
                }
                XmlNode nodo = docXml.SelectSingleNode(pRutaParametro);

                if (nodo != null)
                {
                    resultado = nodo.InnerText;
                }
                else if (pDevolverError)
                {
                    throw new ErrorElementoNoExiste(new Exception("\nEl nodo especificado no existe: " + pFicheroConfiguracionBD + ":" + pRutaParametro));
                }
            }
            catch (Exception)
            {
                if (pDevolverError)
                {
                    throw;
                }
            }
            try
            {
                if (!mListaClaves[pFicheroConfiguracionBD].ContainsKey(pRutaParametro))
                {
                    mListaClaves[pFicheroConfiguracionBD].TryAdd(pRutaParametro, resultado);
                }
                else if (!mListaClaves[pFicheroConfiguracionBD][pRutaParametro].Equals(resultado))
                {
                    mListaClaves[pFicheroConfiguracionBD][pRutaParametro] = resultado;
                }
            }
            catch (Exception) { }
            return resultado;
        }

        /// <summary>
        /// Agrega un nuevo parámetro al archivo
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pRutaParametro">Ruta del parámetro a obtener dentro del árbol XML (Ej: config/urlServicioLogin)</param>
        /// <param name="pNombreParametro">Nombre del nuevo parámetro</param>
        /// <param name="pValor">Valor del parámetro</param>
        /// <returns>url</returns>
        public static void AgregarParametro(string pFicheroConfiguracionBD, string pRutaParametro, string pNombreParametro, string pValor)
        {
            string fichero = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + pFicheroConfiguracionBD;

            if (!File.Exists(fichero))
            {
                StreamWriter sw = new StreamWriter(File.Create(fichero));
                sw.Write("<" + pRutaParametro + "></" + pRutaParametro + ">");
                sw.Close();
                sw = null;
            }

            XmlDocument docXml = new XmlDocument();
            docXml.Load(fichero);

            XmlNode nodo = docXml.SelectSingleNode(pRutaParametro);

            if (nodo != null)
            {
                XmlNode nodoCreado = docXml.CreateNode(XmlNodeType.Element, pNombreParametro, "");
                nodoCreado.InnerText = pValor;
                nodo.AppendChild(nodoCreado);

                docXml.PreserveWhitespace = true;
                XmlTextWriter wrtr = new XmlTextWriter(fichero, Encoding.ASCII);
                docXml.WriteTo(wrtr);
                wrtr.Close();
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexion
        /// </summary>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        /// <returns>Cadena de conexión </returns>
        public string ObtenerCadenaConexion(List<string> pCadenasInvalidas)
        {
            return ObtenerCadenaConexion(ObtenerUrlFicheroConfigXML(), "acid", pCadenasInvalidas);
        }

        /// <summary>
        /// Obtiene la cadena de conexion
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la BD</param>
        /// <param name="pBD">Apelativo de la BD a la que se va a conectar</param>
        /// <param name="pCadenasInvalidas">Lista de cadenas de conexión que no han funcionado bien</param>
        /// <returns>Cadena de conexión </returns>
        public string ObtenerCadenaConexion(string pFicheroConfiguracionBD, string pBD, List<string> pCadenasInvalidas)
        {
            if (ServicioWindows && pBD.Contains("/"))
            {
                pFicheroConfiguracionBD = pBD.Substring(0, pBD.LastIndexOf("/"));
                pBD = pBD.Substring(pBD.LastIndexOf("/") + 1);
            }

            //return ObtenerParametro(pFicheroConfiguracionBD, "config/" + pBD + "/cadenaConexion", true);
            return ObtenerValorAleatorioDeParametroCompuesto(pFicheroConfiguracionBD, pBD, "cadenaConexion", pCadenasInvalidas);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametrosLoginFacebook()
        {
            return ObtenerParametrosLoginFacebook(ObtenerUrlFicheroConfigXML());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametrosLoginFacebook(string pFicheroConfiguracionBD)
        {
            Dictionary<string, string> listaParametrosFacebook = new Dictionary<string, string>();

            string id = ObtenerParametro(pFicheroConfiguracionBD, "config/ListaRedesSociales/facebook/id", false);
            string clientSecret = ObtenerParametro(pFicheroConfiguracionBD, "config/ListaRedesSociales/facebook/clientsecret", false);

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(clientSecret))
            {
                listaParametrosFacebook.Add("id", id);
                listaParametrosFacebook.Add("clientsecret", clientSecret);
            }

            return listaParametrosFacebook;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametrosLoginGoogle()
        {
            return ObtenerParametrosLoginGoogle(ObtenerUrlFicheroConfigXML());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametrosLoginGoogle(string pFicheroConfiguracionBD)
        {
            Dictionary<string, string> listaParametrosGoogle = new Dictionary<string, string>();

            string id = ObtenerParametro(pFicheroConfiguracionBD, "config/ListaRedesSociales/google/id", false);
            string clientSecret = ObtenerParametro(pFicheroConfiguracionBD, "config/ListaRedesSociales/google/clientsecret", false);

            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(clientSecret))
            {
                listaParametrosGoogle.Add("id", id);
                listaParametrosGoogle.Add("clientsecret", clientSecret);
            }

            return listaParametrosGoogle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametrosLoginTwitter()
        {
            return ObtenerParametrosLoginTwitter(ObtenerUrlFicheroConfigXML());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerParametrosLoginTwitter(string pFicheroConfiguracionBD)
        {
            Dictionary<string, string> listaParametrosTwitter = new Dictionary<string, string>();

            string consumerKey = ObtenerParametro(pFicheroConfiguracionBD, "config/ListaRedesSociales/twitter/consumerkey", false);
            string consumerSecret = ObtenerParametro(pFicheroConfiguracionBD, "config/ListaRedesSociales/twitter/consumersecret", false);

            if (!string.IsNullOrEmpty(consumerKey) && !string.IsNullOrEmpty(consumerSecret))
            {
                listaParametrosTwitter.Add("consumerkey", consumerKey);
                listaParametrosTwitter.Add("consumersecret", consumerSecret);
            }

            return listaParametrosTwitter;
        }

        /// <summary>
        /// Obtien la URL en la que se encuentra albergado el servicio de login
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioLogin()
        {
            return ObtenerServicio("urlServicioLogin", Guid.Empty, string.Empty);
        }

        /// <summary>
        /// Obtien la URL en la que se encuentra albergado el servicio de login
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioOauth()
        {
            string urlServicioOauth = ObtenerServicio("urlServicioOauth", Guid.Empty, string.Empty);
            if (urlServicioOauth.StartsWith("https://"))
            {
                urlServicioOauth = urlServicioOauth.Replace("https://", "http://");
            }

            return urlServicioOauth;
        }

        /// <summary>
        /// Url del servicio de archivos.
        /// </summary>
        public string ObtenerUrlServicioArchivos()
        {
            string urlServicioArchivos = ObtenerServicio("urlServicioArchivos", Guid.Empty, string.Empty).Replace("https://", "http://") + "/ServicioArchivos.asmx";
            if (urlServicioArchivos.StartsWith("https://"))
            {
                urlServicioArchivos = urlServicioArchivos.Replace("https://", "http://");
            }
            return urlServicioArchivos;
        }

        public string ObtenerUrlServicioFlash(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioFlash", pProyectoID, pDominio);
        }

        public string ObtenerUrlServicioFichas(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioFichas", pProyectoID, pDominio) + "/ServicioFichas.asmx";
        }

        public string ObtenerUrlServicioBrightcove(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioBrightcove", pProyectoID, pDominio);
        }

        public string ObtenerUrlServicioTOP(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioTOP", pProyectoID, pDominio);
        }

        public string ObtenerUrlServicioEtiquetadoAutomatico(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioEtiquetadoAutomatico", pProyectoID, pDominio);
        }

        /// <summary>
        /// Obtien la URL del servicio de autocompletar
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioAutocompletar(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioAutocompletar", pProyectoID, pDominio) + "/AutoCompletar.asmx";
        }

        /// <summary>
        /// Obtien la URL del servicio de autocompletar etiquetas.
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioAutocompletarEtiquetas(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioAutocompletarEtiquetas", pProyectoID, pDominio) + "/AutoCompletarEtiquetas.asmx";
        }

        /// <summary>
        /// Obtien la URL del servicio de facetas.
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioFacetas(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioFacetas", pProyectoID, pDominio) + "/CargadorFacetas";
        }

        /// <summary>
        /// Obtien la URL del servicio de facetas para la home.
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioFacetasHome(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioFacetasHome", pProyectoID, pDominio) + "/CargadorFacetas";
        }

        /// <summary>
        /// Obtien la URL del servicio de resultados
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioResultados(Guid pProyectoID, string pDominio)
        {
            string urlsServiciosAux = ObtenerUrlsServiciosResultados(pProyectoID, pDominio);

            string[] urlsServicios = urlsServiciosAux.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            int indice = 0;
            if (urlsServicios.Length > 1)
            {
                Random rnd = new Random();
                indice = rnd.Next(urlsServicios.Length);
            }

            return urlsServicios[indice];
        }

        /// <summary>
        /// Obtien lal URLs del servicio de resultados
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlsServiciosResultados(Guid pProyectoID, string pDominio)
        {
            string urlsAux = ObtenerServicio("urlServicioResultados", pProyectoID, pDominio);
            string urlFin = "";
            string coma = "";
            foreach (string url in urlsAux.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                urlFin += coma + url + "/CargadorResultados";
                coma = ",";
            }
            return urlFin;
        }

        /// <summary>
        /// Obtien la URL del servicio de resultados para la home.
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioResultadosHome(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioResultadosHome", pProyectoID, pDominio) + "/CargadorResultados";
        }

        /// <summary>
        /// Obtien la URL del servicio de resultados para la home.
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioContextosHome(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioResultadosHome", pProyectoID, pDominio) + "/CargadorContextoMensajes";
        }

        /// <summary>
        /// Obtien la URL del servicio de autocompletar de la home
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlServicioAutocompletarHome(Guid pProyectoID, string pDominio)
        {
            return ObtenerServicio("urlServicioAutocompletarHome", pProyectoID, pDominio) + "/AutoCompletar.asmx";
        }


        public string ObtenerUrlServicioDocumental()
        {
            string urlServicioWebDocumentacion = ObtenerServicio("urlServicioWebDocumentacion", Guid.Empty, null);

            if (urlServicioWebDocumentacion.StartsWith("https://"))
            {
                urlServicioWebDocumentacion = urlServicioWebDocumentacion.Replace("https://", "http://");
            }

            return urlServicioWebDocumentacion;
        }

        public string ObtenerUrlApiIntegracionContinua()
        {
            string urlApiIntegracionContinua = ObtenerServicio("urlApiIntegracionContinua", Guid.Empty, null);

            return urlApiIntegracionContinua;
        }

        public string ObtenerUrlApiDesplieguesEntornoParametro(string pEntornoIntegracionContinua, Guid pProyectoSeleccionado, string entorno)
        {
            string urlApiIntegracionContinua = ObtenerUrlApiIntegracionContinua();
            if (!string.IsNullOrEmpty(urlApiIntegracionContinua))
            {
                string peticion = urlApiIntegracionContinua + "/integracion/get-apiurl-parameter";
                string requestParameters = $"User={_usuario.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&Entorno={entorno}";

                byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                string urlEnvironment = UtilGeneral.WebRequest("POST", peticion, byteData);

                if (!string.IsNullOrEmpty(urlEnvironment))
                {
                    return urlEnvironment;
                }
            }

            return "";
        }

        public string ObtenerUrlApiDesplieguesEntorno(string pEntornoIntegracionContinua, Guid pProyectoSeleccionado)
        {
            if (!string.IsNullOrEmpty(pEntornoIntegracionContinua))
            {
                string urlApiIntegracionContinua = ObtenerUrlApiIntegracionContinua();
                if (!string.IsNullOrEmpty(urlApiIntegracionContinua))
                {
                    string peticion = urlApiIntegracionContinua + "/integracion/get-apiurl-environment";
                    string requestParameters = $"User={_usuario.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";

                    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                    string urlEnvironment = UtilGeneral.WebRequest("POST", peticion, byteData);

                    if (!string.IsNullOrEmpty(urlEnvironment))
                    {
                        return urlEnvironment;
                    }
                }
            }

            return "";
        }

        public string ObtenerUrlApiDesplieguesEntornoAnterior(string pEntornoIntegracionContinua, Guid pProyectoSeleccionado)
        {
            if (!string.IsNullOrEmpty(pEntornoIntegracionContinua))
            {
                string urlApiIntegracionContinua = ObtenerUrlApiIntegracionContinua();
                if (!string.IsNullOrEmpty(urlApiIntegracionContinua))
                {
                    string peticion = urlApiIntegracionContinua + "/integracion/get-apiurl-previous-environment";
                    string requestParameters = $"User={_usuario.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";

                    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                    string urlEnvironment = UtilGeneral.WebRequest("POST", peticion, byteData);

                    if (!string.IsNullOrEmpty(urlEnvironment))
                    {
                        return urlEnvironment;
                    }
                }
            }

            return null;
        }

        public string ObtenerUrlApiDesplieguesEntornoSiguiente(string pEntornoIntegracionContinua, Guid pProyectoSeleccionado)
        {
            if (!string.IsNullOrEmpty(pEntornoIntegracionContinua))
            {
                string urlApiIntegracionContinua = ObtenerUrlApiIntegracionContinua();
                if (!string.IsNullOrEmpty(urlApiIntegracionContinua))
                {
                    string peticion = urlApiIntegracionContinua + "/integracion/get-apiurl-next-environment";
                    string requestParameters = $"User={_usuario.UsuarioActual.UsuarioID}&Project={pProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";

                    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                    string urlEnvironment = UtilGeneral.WebRequest("POST", peticion, byteData);

                    if (!string.IsNullOrEmpty(urlEnvironment))
                    {
                        return urlEnvironment;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// Obtiene la URL base de la intranet
        /// </summary>
        /// <returns>url</returns>
        public string ObtenerUrlBase()
        {
            return ObtenerParametro("config/gnoss.config", "config/urlBase", true);
        }

        /// <summary>
        /// Obtiene la URL base de la intranet
        /// </summary>
        /// <returns>url</returns>
        public int ObtenerNumeroPersonasEnvioNewsletter()
        {
            string numPersonas = ObtenerParametro("config/gnoss.config", "config/NumeroPersonasEnvioNewsletter", false);

            int resultado = -1;
            if (!int.TryParse(numPersonas, out resultado))
            {
                resultado = -1;
            }
            return resultado;
        }

        /// <summary>
        /// Obtiene el proyecto al que se va a conectar la aplicación
        /// </summary>
        /// <returns>Identificador de proyecto (NULL si no hay ninguna configuración para algún proyecto en concreto)</returns>
        public Guid? ObtenerProyectoConexion()
        {
            string proyecto = ObtenerParametro("config/project.config", "config/ProyectoConexion/ProyectoID", false);

            Guid? proyectoID = null;
            if ((proyecto != null) && (!proyecto.Trim().Equals(string.Empty)))
            {
                proyectoID = new Guid(proyecto);
            }
            return proyectoID;
        }

        /// <summary>
        /// Obtiene el proyecto al que se va a conectar la aplicación
        /// </summary>
        /// <returns>Identificador de proyecto (NULL si no hay ninguna configuración para algún proyecto en concreto)</returns>
        public Guid? ObtenerProyectoPrincipal()
        {
            string proyecto = ObtenerParametro("config/project.config", "config/ProyectoConexion/ProyectoPrincipalID", false);

            Guid? proyectoID = null;
            if ((proyecto != null) && (!proyecto.Trim().Equals(string.Empty)))
            {
                proyectoID = new Guid(proyecto);
            }
            return proyectoID;
        }

        /// <summary>
        /// Obtiene la organización del proyecto al que se va a conectar la aplicación
        /// </summary>
        /// <returns>Identificador de organización (NULL si no hay ninguna configuración para algún proyecto en concreto)</returns>
        public Guid? ObtenerOrganizacionConexion()
        {
            string organizacion = ObtenerParametro("config/project.config", "config/ProyectoConexion/OrganizacionID", false);

            Guid? organizacionID = null;
            if ((organizacion != null) && (!organizacion.Trim().Equals(string.Empty)))
            {
                organizacionID = new Guid(organizacion);
            }
            return organizacionID;
        }

        /// <summary>
        /// Obtiene la organización del proyecto al que se va a conectar la aplicación
        /// </summary>
        /// <returns>Identificador de organización (NULL si no hay ninguna configuración para algún proyecto en concreto)</returns>
        public Guid? ObtenerOrganizacionGnoss()
        {
            string organizacion = ObtenerParametro("config/gnoss.config", "config/OrganizacionGnoss/OrganizacionID", false);

            Guid? organizacionID = null;
            if ((organizacion != null) && (!organizacion.Trim().Equals(string.Empty)))
            {
                organizacionID = new Guid(organizacion);
            }
            return organizacionID;
        }

        /// <summary>
        /// Obtiene el proyecto al que se va a conectar la aplicación
        /// </summary>
        /// <returns>Identificador de proyecto (NULL si no hay ninguna configuración para algún proyecto en concreto)</returns>
        public Guid? ObtenerProyectoGnoss()
        {
            string proyecto = ObtenerParametro("config/gnoss.config", "config/OrganizacionGnoss/ProyectoID", false);

            Guid? proyectoID = null;
            if ((proyecto != null) && (!proyecto.Trim().Equals(string.Empty)))
            {
                proyectoID = new Guid(proyecto);
            }
            return proyectoID;
        }

        /// <summary>
        /// Crea el fichero de configuración
        /// </summary>
        public static void CrearFicheroConfiguracion()
        {
            StreamWriter escritor = new StreamWriter(Configuracion.FICHERO_CONFIG, false, System.Text.Encoding.UTF8);
            escritor.Write(Configuracion.ESTRUCTURA_CONFIG);
            escritor.Close();
            escritor.Dispose();
        }

        /// <summary>
        /// Obtiene la key de la sesión para un dominio concreto
        /// </summary>
        /// <param name="pDominioAplicacion">Dominio de la aplicación actual</param>
        /// <returns></returns>
        public string ObtenerKeySesion(string pDominioAplicacion)
        {
            string key = ObtenerParametro("config/keySesion.config", "config/KeySesion", false);

            if (string.IsNullOrEmpty(key))
            {
                if (string.IsNullOrEmpty(pDominioAplicacion))
                {
                    pDominioAplicacion = "GNOSS Sistema Semántico";
                }

                pDominioAplicacion += "_*#%&_" + ObtenerProyectoConexion();

                key = HashHelper.GenerarMD5(pDominioAplicacion);
                AgregarParametro("config/keySesion.config", "config", "KeySesion", key);

            }
            return key;
        }

        public string ObtenerUrlFicheroConfigXML()
        {
            return ObtenerParametro("config/UrlConfigXML.config", "config/urlFicheroConfigXML", false);
        }


        #endregion

        #region Propiedades

        /// <summary>
        /// Indica si estamos usando un servicio Windows o no.
        /// </summary>
        public static bool ServicioWindows
        {
            get
            {
                return mServicioWindows;
            }
            set
            {
                mServicioWindows = value;
            }
        }

        /// <summary>
        /// Indica si estamos usando un servicio Web o no.
        /// </summary>
        public static bool ServicioWeb
        {
            get
            {
                return mServicioWeb;
            }
            set
            {
                mServicioWeb = value;
            }
        }
        public static bool UsarVariablesEntorno
        {
            get
            {
                return (Environment.GetEnvironmentVariable("useEnvironmentVariables") != null && Environment.GetEnvironmentVariable("useEnvironmentVariables").ToLower().Equals("true"));
            }
        }

        /// <summary>
        /// Obtiene el Application path de la petición actual
        /// </summary>
        private string ApplicationPath
        {
            get
            {
                string applicationpath = "";
                try
                {
                    applicationpath = _httpContextAccessor.HttpContext.Request.PathBase;
                    //applicationpath = HttpContext.Current.Request.ApplicationPath;
                }
                catch (Exception)
                {
                    //No capturo la excepción porque sólo salta cuando llega a este código desde el Application_Start de global.asax
                }
                return applicationpath;
            }
        }

        /// <summary>
        /// Ficheros de configurarción externos cargados con el elemento raiz del archivo de configuración.
        /// </summary>
        public static Dictionary<string, XmlDocument> FicherosConfigExternos
        {
            get
            {
                return mFicherosConfigExternos;
            }
            set
            {
                mFicherosConfigExternos = value;
            }
        }

        /// <summary>
        /// Obtiene la ruta del fichero de configuración de base de datos
        /// </summary>
        private string RutaFicheroConfigBD
        {
            get
            {
                if (string.IsNullOrEmpty(mRutaFicheroConfigBD))
                {
                    mRutaFicheroConfigBD = Path.Combine(_env.ContentRootPath, "/configBD/bd.config");
                    //mRutaFicheroConfigBD = HttpContext.Current.Server.MapPath(ApplicationPath + "/configBD/bd.config");
                }
                return mRutaFicheroConfigBD;
            }
        }

        #endregion
    }
}
