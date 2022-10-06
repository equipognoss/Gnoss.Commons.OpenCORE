
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Newtonsoft.Json;
using OpenLink.Data.Virtuoso;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Es.Riam.AbstractsOpen
{
    public abstract class IServicesUtilVirtuosoAndReplication
    {
        protected ConfigService mConfigService;
        protected LoggingService mLoggingService;
        public const string COLA_REPLICACION_MASTER = "ColaReplicacionMaster";
        public const string COLA_REPLICACION_MASTER_HOME = "ColaReplicacionMasterHome";
        private string mFicheroConfiguracionMaster;
        public string FicheroConfiguracion = "";
        public bool UsarClienteTradicional { get; set; }
        /// <summary>
        /// Tabla de replicación donde se insertan las actualizaciones de virtuoso por defecto.
        /// </summary>
        private string mTablaReplicacion = COLA_REPLICACION_MASTER;
        public VirtuosoConnection Connection { get; set; }
        public VirtuosoConnection ConexionMaster { get; set; }
        //DatosPeticion
        private bool noConfirmarTransacciones;
        private Dictionary<string, DbTransaction> transaccionesPendientes;
        public Dictionary<string, DbTransaction> TransaccionesPendientes { get { return transaccionesPendientes; } }
        public string CadenaConexion { get; set; }
        public DateTime? InicioPeticionVirtuoso { get; set; }
        public string ConexionAfinidadVirtuoso { get; set; }
        public DateTime FechaFinAfinidad { get; set; }
        public bool NoConfirmarTransacciones
        {
            get
            {
                return noConfirmarTransacciones;
            }
            set
            {
                noConfirmarTransacciones = value;

                if (value && transaccionesPendientes == null)
                {
                    transaccionesPendientes = new Dictionary<string, DbTransaction>();
                }
            }
        }


        public IServicesUtilVirtuosoAndReplication(ConfigService configService, LoggingService loggingService)
        {
            mConfigService = configService;
            mLoggingService = loggingService;
            transaccionesPendientes = new Dictionary<string, DbTransaction>();
        }
        protected Dictionary<string, List<KeyValuePair<string, short>>> mListaConsultasEjecutarEnReplicas { get; set; }
        protected long XCorrelationID { get; set; }
        protected Dictionary<string, List<KeyValuePair<string, short>>> ListaConsultasEjecutarEnReplicas
        {
            get
            {
                Dictionary<string, List<KeyValuePair<string, short>>> listaConsultas = mListaConsultasEjecutarEnReplicas;
                if (listaConsultas == null)
                {
                    listaConsultas = new Dictionary<string, List<KeyValuePair<string, short>>>();
                    mListaConsultasEjecutarEnReplicas = listaConsultas;
                }

                return listaConsultas;
            }
        }
        public List<string> ListaGrafos
        {
            get;
            set;

        }

        public DbTransaction TransaccionVirtuoso
        {
            get; set;
        }
        /// <summary>
        /// Obtiene o establece la tabla de replicación
        /// </summary>
        public string TablaReplicacion
        {
            get
            {
                return mTablaReplicacion;
            }
            set
            {
                mTablaReplicacion = value;
            }
        }


        // Abstract 
        public abstract void InsertarEnReplicacionBidireccional(string pQuery, string pGrafo, short pPrioridad, string pNombreConexionAfinidad, string pCadenaConexion, VirtuosoConnection pConexion);

        public string FicheroConfiguracionMaster
        {
            get
            {
                if (string.IsNullOrEmpty(FicheroConfiguracion))
                {
                    mFicheroConfiguracionMaster = "virtuoso_Master";
                }
                else if (!FicheroConfiguracion.EndsWith("_Master") && !FicheroConfiguracion.EndsWith("CargarConfiguracion"))
                {
                    mFicheroConfiguracionMaster = FicheroConfiguracion + "_Master";
                }
                else
                {
                    mFicheroConfiguracionMaster = FicheroConfiguracion;
                }
                return mFicheroConfiguracionMaster;
            }
        }
        public string ConexionAfinidad { get; set; }

        public Dictionary<string, List<KeyValuePair<string, short>>> GetListaConsultasEjecutarEnReplicas()
        {
            return ListaConsultasEjecutarEnReplicas;
        }

        public void ListaConsultasEjecutarEnReplicasAdd(string pKey, List<KeyValuePair<string, short>> pValue)
        {
            ListaConsultasEjecutarEnReplicas.Add(pKey, pValue);
        }
        public void ListaConsultasEjecutarEnReplicasAddValueToKey(string pKey, KeyValuePair<string, short> pValue)
        {
            ListaConsultasEjecutarEnReplicas[pKey].Add(pValue);
        }
        public void ListaConsultasEjecutarEnReplicasClear()
        {
            ListaConsultasEjecutarEnReplicas.Clear();
        }

        public bool EsConexionHAPROXY(string pCadenaConexion)
        {
            return (mConfigService.CheckBidirectionalReplicationIsActive() && mConfigService.ObtenerVirtuosoConnectionString().Equals(pCadenaConexion));
        }

        public int ActualizarVirtuoso(string pQuery, string pGrafo, bool pReplicar, short pPrioridad, VirtuosoConnection pConexion, bool pLanzarExcepcionSiFallaReplicacion = true, string pCadenaConexion = null, int pNumReintentos = 0)
        {
            int resultado = 0;
            string cadenaConexion = pCadenaConexion;
            if (string.IsNullOrEmpty(cadenaConexion))
            {
                if (FicheroConfiguracionMaster.ToLower().Contains("home"))
                {
                    cadenaConexion = mConfigService.ObtenerVirtuosoEscrituraHome();
                }
                else
                {
                    var virtuoso = mConfigService.ObtenerVirtuosoEscritura();
                    cadenaConexion = virtuoso.Value;
                    ConexionAfinidad = virtuoso.Key;
                }
            }

            string instruccion = pQuery.Substring(0, pQuery.IndexOf('{') + 1);
            int numeroLineas = pQuery.Count(salto => salto.Equals('\n'));

            if (instruccion.Contains(" INSERT INTO ") && numeroLineas > 600)
            {
                //Le pongo el resultado como el número de partes que tiene la inserción en negativo, para poder identificarlo
                //EscribirLogActualizarVirtuoso(pQuery, "queryLarga", (numeroLineas / 500) + 1);
                //Es un insert de más de 600 líneas, la parto para que no falle
                EjecutarInsertPorTrozos(pQuery, pGrafo, pReplicar, pPrioridad, pConexion);
            }
            else
            {
                string conexionAfinidadVirtuoso = "conexionAfinidadVirtuoso" + (FicheroConfiguracionMaster.ToLower().Contains("home") ? "Home" : "");

                KeyValuePair<string, string> ip_puerto = ObtenerIpVirtuosoDeCadenaConexion(cadenaConexion);
                string ipVirtuoso = ip_puerto.Key;
                string puertoVirtuoso = ip_puerto.Value;
                string url = "http://" + ipVirtuoso + ":" + puertoVirtuoso + "/sparql";

                mLoggingService.AgregarEntrada("Escribo en virtuoso " + ipVirtuoso + ". " + pQuery);

                try
                {
                    if (UsarClienteTradicional)
                    {
                        pQuery = UtilCadenas.PasarAUtf8(pQuery);
                        resultado = ActualizarVirtuosoClienteTradicional(pQuery, pGrafo, pReplicar, pPrioridad, pConexion, pCadenaConexion);
                    }
                    else
                    {
                        //Quito el inicio de SPARQL
                        if (pQuery.Trim().ToUpper().StartsWith("SPARQL"))
                        {
                            pQuery = pQuery.Trim().Substring(6);
                        }
                        NameValueCollection parametros = new NameValueCollection();
                        parametros.Add("query", pQuery);

                        resultado = ActualizarVirtuoso_WebClient(url, pQuery, parametros);
                    }
                }
                catch (ExcepcionDeBaseDeDatos)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    mLoggingService.AgregarEntrada("Reintentar Actualizar virtuoso. Descripcion error: " + ex.Message);
                    mLoggingService.GuardarLogError(ex);

                    Thread.Sleep(1000);

                    if (ControlarErrorVirtuosoConection(cadenaConexion, conexionAfinidadVirtuoso))
                    {
                        resultado = ActualizarVirtuoso(pQuery, pGrafo, pReplicar, pPrioridad, pConexion, pLanzarExcepcionSiFallaReplicacion, pCadenaConexion);
                        return resultado;
                    }
                    else
                    {
                        mLoggingService.AgregarEntrada("VIRTUOSO ERROR : Se ha superado el tiempo establecido para realizar la actualización en virtuoso.");
                        throw;
                    }
                }

                mLoggingService.AgregarEntrada("Actualizado con éxito: " + pQuery);

                if (pReplicar)
                {

                    string nombreConexionAfinidad = ConexionAfinidad;

                    try
                    {
                        if ((string.IsNullOrEmpty(FicheroConfiguracion) || !FicheroConfiguracion.ToLower().Contains("home")) && !string.IsNullOrEmpty(nombreConexionAfinidad))
                        {
                            //Para la replicaciónBidireccional
                            InsertarEnReplicacionBidireccional(pQuery, pGrafo, pPrioridad, nombreConexionAfinidad, pCadenaConexion, pConexion);
                        }

                        //Para la Replicación Normal.
                        if (mConfigService.ObtenerReplicacionActivada() && (TablaReplicacion == null || TablaReplicacion.Equals(COLA_REPLICACION_MASTER)))
                        {
                            if (!UsarClienteTradicional)
                            {
                                InsertarEnReplicacion(pQuery, TablaReplicacion);
                            }
                            else if (TransaccionVirtuoso == null)
                            {
                                InsertarEnReplicacion(pQuery, TablaReplicacion, false);
                            }
                            else
                            {
                                if (!GetListaConsultasEjecutarEnReplicas().ContainsKey(""))
                                {
                                    ListaConsultasEjecutarEnReplicasAdd("", new List<KeyValuePair<string, short>>());
                                }
                                ListaConsultasEjecutarEnReplicasAddValueToKey("", new KeyValuePair<string, short>(pQuery, pPrioridad));
                            }
                        }

                        //Para la Replicacion de la home
                        if (mConfigService.ObtenerReplicacionActivadaHOME() && TablaReplicacion != null && TablaReplicacion.Equals(COLA_REPLICACION_MASTER_HOME))
                        {
                            if (!UsarClienteTradicional)
                            {
                                InsertarEnReplicacion(pQuery, TablaReplicacion);
                            }
                            else if (TransaccionVirtuoso == null)
                            {
                                InsertarEnReplicacion(pQuery, TablaReplicacion, false);
                            }
                            else
                            {
                                if (!GetListaConsultasEjecutarEnReplicas().ContainsKey(""))
                                {
                                    ListaConsultasEjecutarEnReplicasAdd("", new List<KeyValuePair<string, short>>());
                                }
                                ListaConsultasEjecutarEnReplicasAddValueToKey("", new KeyValuePair<string, short>(pQuery, pPrioridad));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (pLanzarExcepcionSiFallaReplicacion)
                        {
                            throw;
                        }
                        else
                        {
                            mLoggingService.GuardarLogError(ex);
                        }
                    }

                    mLoggingService.AgregarEntrada("Replicado con éxito");
                }

                mLoggingService.AgregarEntrada("Escrito en virtuoso.");
            }

            return resultado;
        }
        private string ObtenerConexionVirtuosoProximoIntento(string conexionActual)
        {
            string nombreConexionReplica = "";
            if (string.IsNullOrEmpty(conexionActual))
            {
                var virtuoso = mConfigService.ObtenerVirtuosoEscritura();
                nombreConexionReplica = virtuoso.Value;
                ConexionAfinidad = virtuoso.Key;
            }
            else
            {
                nombreConexionReplica = mConfigService.ObtenerNombreConexionReplica(conexionActual.Replace("_Master", ""));
            }

            return nombreConexionReplica;
        }
        private bool ComprobarConexionVirtuosoSinTareasPendientes(string nombreConexionReplica)
        {
            int numReplicacionesPendientes = ContarReplicacionesPendientesEnReplica(nombreConexionReplica); ;
            if (numReplicacionesPendientes > 500)
            {
                return false;
            }
            else if (numReplicacionesPendientes > 0)
            {
                Thread.Sleep(1000);
            }
            return true;
        }
        public int ContarReplicacionesPendientesEnReplica(string pNombreConexion)
        {
            string NombreTablaReplica = $"TablaReplicacion_{pNombreConexion.Replace("_Master", "")}";

            int numReplicacionesPendientes = -1;

            using (RabbitMQClient rMQ = new RabbitMQClient("colaReplicacion", NombreTablaReplica, mLoggingService, mConfigService))
            {
                numReplicacionesPendientes = rMQ.ContarElementosEnCola();
            }

            return numReplicacionesPendientes;
        }

        public bool ControlarErrorVirtuosoConection(string cadenaConexion, string conexionAfinidadVirtuoso)
        {
            DateTime inicioPeticionVirtuoso = DateTime.Now;
            if (InicioPeticionVirtuoso.HasValue)
            {
                inicioPeticionVirtuoso = InicioPeticionVirtuoso.Value;
            }
            else
            {
                InicioPeticionVirtuoso = inicioPeticionVirtuoso;
            }

            string nombreConexionAfinidad = "";

            if (!EsConexionHAPROXY(cadenaConexion))
            {
                nombreConexionAfinidad = ConexionAfinidadVirtuoso;
            }

            bool conexionLibreEncontrada = false;
            while (!conexionLibreEncontrada && (inicioPeticionVirtuoso.AddSeconds(30) - DateTime.Now).TotalSeconds > 0)
            {
                string nombreConexionReplica = ObtenerConexionVirtuosoProximoIntento(nombreConexionAfinidad);

                if (string.IsNullOrEmpty(nombreConexionReplica))
                {
                    //Solo hay un servidor, no hay replicacion bidireccional con HA-PROXY
                    conexionLibreEncontrada = true;
                }
                else
                {
                    conexionLibreEncontrada = ComprobarConexionVirtuosoSinTareasPendientes(nombreConexionReplica);
                    if (!conexionLibreEncontrada)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
            return conexionLibreEncontrada;
        }
        protected int ActualizarVirtuoso_WebClient(string pUrl, string pQuery, NameValueCollection pParametros, string pConexionAfinidad)
        {
            mLoggingService.AgregarEntrada("EscrituraWebClient: Inicio");
            RiamWebClient webClient = new RiamWebClient(TimeOutVirtuoso);
            webClient.Encoding = Encoding.UTF8;

            //no se necesita la cabecera
            //webClient.Headers.Add(HttpRequestHeader.ContentType, "application/sparql-query"); //"application/x-www-form-urlencoded"

            DateTime horaInicio = DateTime.Now;
            int milisegundos = 0;
            string error = null;
            int resultado = 0;

            try
            {
                byte[] responseArray = webClient.UploadValues(pUrl, "POST", pParametros);
                milisegundos = (int)DateTime.Now.Subtract(horaInicio).TotalMilliseconds;
                string respuesta = Encoding.UTF8.GetString(responseArray);

                resultado = ObtenerResultadoRespuesta(respuesta);

                //Al actualizar datos en virtuoso, guardamos los datos del servidor en el que hemos guardado para acceder a él.
                string[] cabeceraServidor = webClient.ResponseHeaders.GetValues("X-App-Server");
                if (cabeceraServidor == null || cabeceraServidor.Length == 0)
                {
                    cabeceraServidor = webClient.ResponseHeaders.GetValues("Server");
                }

                if (cabeceraServidor != null && cabeceraServidor.Length > 0)
                {
                    ConexionAfinidad = cabeceraServidor.FirstOrDefault();
                }

                mLoggingService.AgregarEntrada("EscrituraWebClient: Respuesta obtenida de virtuoso");
            }
            catch (System.Net.WebException webException)
            {
                //int numIntentosFallidos = 0;
                //if (UtilPeticion.ObtenerObjetoDePeticion("intentosFallidosVirtuoso") != null)
                //{
                //    numIntentosFallidos = (int)UtilPeticion.ObtenerObjetoDePeticion("intentosFallidosVirtuoso");
                //}
                //UtilPeticion.AgregarObjetoAPeticionActual("intentosFallidosVirtuoso", numIntentosFallidos + 1);

                milisegundos = (int)DateTime.Now.Subtract(horaInicio).TotalMilliseconds;
                string respuesta = "";

                try
                {
                    //Intento recuperar información del error
                    StreamReader dataStream = new StreamReader(webException.Response.GetResponseStream());
                    respuesta = dataStream.ReadToEnd();
                    webException.Response.Close();
                }
                catch { }

                respuesta += "\n\nQuery: " + pQuery;
                respuesta += "\n\nUrl: " + pUrl;
                error = respuesta;
                mLoggingService.GuardarLogError(webException, respuesta);


                if (webException.Message.Contains("(503)") || webException.Message.Contains("(404)") || (webException.Response != null && ((!((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.BadRequest) && !((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.InternalServerError) && webException.Status.Equals(WebExceptionStatus.ProtocolError)) || ((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.NotFound))))
                {
                    // Es un error de checkpoint
                    throw new ExcepcionCheckpointVirtuoso();
                }
                else if (webException.Status.Equals(WebExceptionStatus.ConnectFailure))
                {
                    // Es un error de que virtuoso se ha caído
                    throw new ExcepcionConectionFailVirtuoso();
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pQuery, webException);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                mLoggingService.GuardarLogError(ex, "\n\nQuery: " + pQuery + "\n\nUrl: " + pUrl);
                throw new ExcepcionDeBaseDeDatos(pQuery, ex);
            }
            finally
            {
                webClient.Dispose();

                if (milisegundos > 700)
                {
                    //Error.GuardarLogConsultaCostosa(string.Format("Consulta: {0} \r\nTiempo transcurrido:\r\n{1} \r\nUrl:\r\n{2} \r\nError:\r\n{3}", pQuery, milisegundos, pUrl, error));
                }
            }

            mLoggingService.AgregarEntrada("EscrituraWebClient: Fin");
            return resultado;
        }

        protected int ObtenerResultadoRespuesta(string pRespuesta)
        {
            //las cadenas de respuesta esperadas pueden ser del tipo:
            //Insert into <http://grafoprueba>, 1 (or less) triples -- done
            //Delete from <http://grafoprueba>, 1 (or less) triples -- done
            //Modify <http://grafoprueba>, delete 0 (or less) and insert 0 (or less) triples -- done

            int resultado = 0;

            if (!string.IsNullOrEmpty(pRespuesta) && pRespuesta.Contains(','))
            {
                try
                {
                    XmlDocument docXml = new XmlDocument();
                    docXml.LoadXml(pRespuesta);

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(docXml.NameTable);
                    nsmgr.AddNamespace("sp", "http://www.w3.org/2005/sparql-results#");
                    XmlNode nodoLiteral = docXml.SelectSingleNode("//sp:results//sp:result//sp:binding//sp:literal", nsmgr);

                    if (nodoLiteral != null)
                    {
                        string aux = nodoLiteral.InnerText;
                        bool esModify = aux.ToLower().StartsWith("modify");

                        if (!string.IsNullOrEmpty(aux) && pRespuesta.Contains(','))
                        {
                            int indiceComa = aux.IndexOf(',');
                            aux = aux.Substring(indiceComa + 1);

                            if (!string.IsNullOrEmpty(aux) && pRespuesta.Contains('('))
                            {
                                int indiceParentesis = aux.IndexOf('(');
                                if (!esModify)
                                {
                                    aux = aux.Substring(0, indiceParentesis - 1).Trim();
                                }
                                else
                                {
                                    int indiceDelete = aux.ToLower().IndexOf("delete") + 6;
                                    aux = aux.Substring(indiceDelete, indiceParentesis - indiceDelete).Trim();
                                }
                                int numero = 0;

                                if (int.TryParse(aux, out numero))
                                {
                                    resultado = numero;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pGrafo">Grafo que se va a actualiar</param>
        /// <param name="pReplicar">Verdad si esta consulta debe replicarse (por defecto TRUE)</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public void EjecutarInsertPorTrozos(string pQuery, string pGrafo, bool pReplicar, short pPrioridad, VirtuosoConnection pConexion)
        {
            bool transaccionIniciada = IniciarTransaccion();
            try
            {
                int numeroLineasPorTrozo = 500;
                int indiceInicioTriples = pQuery.IndexOf('{') + 1;
                int finTriples = pQuery.LastIndexOf('}');
                string instruccion = pQuery.Substring(0, indiceInicioTriples);
                string triples = pQuery.Substring(indiceInicioTriples, finTriples - indiceInicioTriples);

                char[] separador = { '\n' };
                string[] lineas = triples.Split(separador, StringSplitOptions.RemoveEmptyEntries);
                int numeroPartes = (lineas.Length / numeroLineasPorTrozo) + 1;

                int lineaAnterior = 0;

                for (int i = 0; i < numeroPartes; i++)
                {
                    int numeroLineas = numeroLineasPorTrozo;

                    if (i.Equals(numeroPartes - 1))
                    {
                        //Es el último trozo, cojo sólo los que quedan
                        numeroLineas = lineas.Length % numeroLineasPorTrozo;
                    }

                    string[] lineasInstruccion = new string[numeroLineas];
                    Array.Copy(lineas, lineaAnterior, lineasInstruccion, 0, numeroLineas);
                    lineaAnterior += numeroLineas;

                    string miniQuery = string.Join("\n", lineasInstruccion);

                    miniQuery = instruccion + miniQuery + " } ";

                    ActualizarVirtuoso(miniQuery, pGrafo, pReplicar, pPrioridad, pConexion);
                }

                if (transaccionIniciada)
                {
                    // La transacción no se ha iniciado aquí, la confirmarán donde se haya iniciado
                    TerminarTransaccion(true);
                }
            }
            catch
            {
                if (transaccionIniciada)
                {
                    // La transacción no se ha iniciado aquí, harán rollback donde se haya iniciado
                    TerminarTransaccion(false);
                }
                throw;
            }
        }
        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito"></param>
        public void TerminarTransaccion(bool pExito)
        {
            if (!NoConfirmarTransacciones && UsarClienteTradicional)
            {
                string nombreTransaccion = $"VirtuosoTransaccion";
                if (TransaccionVirtuoso != null)
                {
                    DbTransaction transaccion = TransaccionVirtuoso;
                    if (pExito)
                    {
                        transaccion.Commit();

                        InsertarInstruccionesEnReplica();
                    }
                    else if (transaccion.Connection != null)
                    {
                        try
                        {
                            transaccion.Rollback();
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex);
                        }

                    }
                    transaccion = null;
                }
                TransaccionVirtuoso.Dispose();
                TransaccionVirtuoso = null;
            }
        }
        internal void InsertarInstruccionesEnReplica()
        {
            if (ListaConsultasEjecutarEnReplicas != null && ListaConsultasEjecutarEnReplicas.Count > 0)
            {
                Guid transactionID = Guid.NewGuid();
                foreach (string tablaReplicacion in GetListaConsultasEjecutarEnReplicas().Keys)
                {
                    List<string> listaConsultas = new List<string>();
                    foreach (KeyValuePair<string, short> consulta in ListaConsultasEjecutarEnReplicas[tablaReplicacion])
                    {
                        listaConsultas.Add(consulta.Key);
                    }

                    InsertarEnReplicacion(listaConsultas, tablaReplicacion, false);
                }

                ListaConsultasEjecutarEnReplicas.Clear();
            }
        }

        private static int? mTimeOutVirtuosoConfiguracion = null;
        private int? mTimeOutVirtuoso = null;

        protected int TimeOutVirtuoso
        {
            get
            {
                if (!mTimeOutVirtuoso.HasValue)
                {
                    int timeOut = 100;
                    if (!mTimeOutVirtuosoConfiguracion.HasValue)
                    {
                        int.TryParse(mConfigService.ObtenerTimeoutVirtuoso(), out timeOut);
                        mTimeOutVirtuosoConfiguracion = timeOut;
                    }

                    mTimeOutVirtuoso = mTimeOutVirtuosoConfiguracion;
                }
                return mTimeOutVirtuoso.Value;
            }
            set
            {
                mTimeOutVirtuoso = value;
            }
        }
        /// <summary>
        /// Crea una conexión a la BD de forma rápida.
        /// </summary>
        /// <param name="pConexion">Conexión</param>
        /// <param name="pTimeout">Tiempo de espera</param>
        public void QuickOpen(DbConnection pConexion)
        {
            Type tipo = pConexion.GetType();
            string connectionString = pConexion.ConnectionString;
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                pConexion.Open();
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, null, true);
                throw;
            }

            mLoggingService.AgregarEntradaDependencia($"Conexión establecida a {connectionString} ", false, $"Conexion {tipo.Name}", sw, true);
        }

        /// <summary>
        /// Crea una conexión a la BD de forma rápida.
        /// </summary>
        /// <param name="pConexion">Conexión</param>
        /// <param name="pTimeout">Tiempo de espera</param>
        public short QuickOpen(DbConnection pConexion, int pTimeout)
        {
            short connectSuccess = 0;
            Type tipo = pConexion.GetType();
            string connectionString = pConexion.ConnectionString;
            //Cambiamos el puerto por el 1111
            KeyValuePair<string, string> ip_puerto = ObtenerIpVirtuosoDeCadenaConexion(connectionString);
            string ipVirtuoso = ip_puerto.Key;
            string puertoVirtuoso = ip_puerto.Value;
            connectionString = connectionString.Replace(ipVirtuoso + ":" + puertoVirtuoso, ipVirtuoso + $":{mConfigService.ObtenerPuertoVirtuosoAux()}");

            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            CancellationTokenSource tokenCancelacion = new CancellationTokenSource();
            Task tarea = Task.Factory.StartNew(() =>
            {
                try
                {
                    pConexion.Open();
                    //Si se ha cancelado la tarea, lanzo la excepción para cancelarla
                    //tokenCancelacion.Token.ThrowIfCancellationRequested();
                    if (!tokenCancelacion.Token.IsCancellationRequested)
                    {
                        connectSuccess = 1;
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("A connection attempt failed because the connected party did not properly respond after a period of time"))
                    {
                        //Error de conexión relacionado con el checkpoint de virtuoso
                        connectSuccess = 2;
                    }
                    mLoggingService.GuardarLogError(ex);
                }
                finally
                {
                    if (pConexion != null && connectSuccess != 1)
                    {
                        try
                        {
                            if (pConexion.State.Equals(ConnectionState.Open))
                            {
                                pConexion.Close();
                            }
                            pConexion.Dispose();
                        }
                        catch { }
                    }
                }
            }, tokenCancelacion.Token);

            mLoggingService.AgregarEntrada("Join de " + pTimeout);
            tarea.Wait(pTimeout);

            if (connectSuccess == 0)
            {
                //La tarea no ha acabado, la cancelo
                tokenCancelacion.Cancel();
                mLoggingService.AgregarEntradaDependencia($"tarea de conexión cancelada a {connectionString}", false, $"Conexion {tipo.Name}", sw, false);
            }

            mLoggingService.AgregarEntradaDependencia($"¿la conexión ha sido exitosa a {connectionString}? " + connectSuccess, false, $"Conexion {tipo.Name}", sw, true);
            return connectSuccess;
        }

        /// <summary>
        /// Comprueba si la conexión es null o la cadena de conexión está vacía
        /// </summary>
        /// <param name="pConexion">Conexión a virtuoso</param>
        /// <returns></returns>
        public bool ComprobarConexionValida(VirtuosoConnection pConexion)
        {
            if (pConexion != null)
            {
                try
                {
                    string cadenaConexion = pConexion.ConnectionString;
                    if (string.IsNullOrEmpty(cadenaConexion))
                    {
                        //la cadena de conexión es vacía
                        return false;
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                    //La cadena de conexión está mal formada
                    return false;
                }
                //Si llega hasta aquí, la conexión es buena
                return true;
            }
            //La cadena de conexión es null
            return false;
        }

        public string obtenerNombreConexionReplicaHAProxy(string pNombreConexion)
        {
            string haProxi = mConfigService.ObtenerVirtuosoConnectionString();
            if (FicheroConfiguracionMaster.ToLower().Contains("home"))
            {
                haProxi = mConfigService.ObtenerVirtuosoConnectionStringHome();
            }
            string conexionHAProxy = "";
            if (!string.IsNullOrEmpty(haProxi))
            {
                KeyValuePair<string, string> ip_puerto = ObtenerIpVirtuosoDeCadenaConexion(haProxi);
                string ipVirtuoso = ip_puerto.Key;
                string puertoVirtuoso = ip_puerto.Value;
                string url = "http://" + ipVirtuoso + ":" + puertoVirtuoso + "/sparql";

                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                webClient.DownloadString(url);
                //Al actualizar datos en virtuoso, guardamos los datos del servidor en el que hemos guardado para acceder a él.
                string[] cabeceraServidor = webClient.ResponseHeaders.GetValues("X-App-Server");
                if (cabeceraServidor == null || cabeceraServidor.Length == 0)
                {
                    cabeceraServidor = webClient.ResponseHeaders.GetValues("Server");
                }
                webClient.Dispose();
                if (cabeceraServidor != null && cabeceraServidor.Length > 0)
                {
                    conexionHAProxy = $"{cabeceraServidor.FirstOrDefault()}";
                    return conexionHAProxy;
                }

            }

            if (string.IsNullOrEmpty(conexionHAProxy))
            {
                var conexionReplica = mConfigService.ObtenerVirtuosoEscritura();
                ConexionAfinidad = conexionReplica.Key;
                return conexionReplica.Value;
            }

            return null;
        }
        /// <summary>
        /// Verdad si existe el fichero bd.config con el elemento acidMaster, falso en caso contrario
        /// </summary>
        private static bool ExisteFicheroMaster
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// Obtiene la conexión a virtuoso
        /// </summary>
        public VirtuosoConnection ParentConnectionMaster
        {
            get
            {
                int numIntentos = 0;
                int numMaxIntentos = 9;
                while ((ConexionMaster == null || !ConexionMaster.State.Equals(ConnectionState.Open)) && (numIntentos < numMaxIntentos))
                {
                    if (numIntentos == 3 || numIntentos == 6) //Espero 1 segundo antes de intentarlo otras 3 veces.
                    {
                        Thread.Sleep(1000);
                    }
                    numIntentos++;
                    if (!ComprobarConexionValida(ConexionMaster))
                    {
                        if (ExisteFicheroMaster)
                        {                       
                            ConexionMaster = Connection;
                            List<string> listaGrafos = ListaGrafos;
                            if (listaGrafos == null)
                            {
                                listaGrafos = new List<string>();
                            }
                            if (!listaGrafos.Contains("mConexionMaster" + FicheroConfiguracionMaster))
                            {
                                listaGrafos.Add("mConexionMaster" + FicheroConfiguracionMaster);
                            }

                            if (!ComprobarConexionValida(ConexionMaster))
                            {
                                string cadenaConexion = null;
                                try
                                {
                                    if (FicheroConfiguracionMaster.ToLower().Contains("home"))
                                    {
                                        cadenaConexion = mConfigService.ObtenerVirtuosoEscrituraHome();
                                    }
                                    else
                                    {
                                        cadenaConexion = mConfigService.ObtenerVirtuosoEscritura().Value;
                                    }

                                    if (mConfigService.CheckBidirectionalReplicationIsActive())
                                    {
                                        string conexionAfinidadVirtuoso = "conexionAfinidadVirtuoso";
                                        if (FicheroConfiguracionMaster != null && FicheroConfiguracionMaster.ToLower().Contains("home"))
                                        {
                                            conexionAfinidadVirtuoso += "Home";
                                        }

                                        string nombreConexionReplica = obtenerNombreConexionReplicaHAProxy(FicheroConfiguracionMaster);
                                    }
                                    ConexionMaster = new VirtuosoConnection(cadenaConexion);
                                }
                                catch (Exception e)
                                {
                                    //mExisteFicheroMaster = false;
                                    ConexionMaster = ParentConnection;
                                }
                            }
                        }
                        else
                        {
                            ConexionMaster = ParentConnection;
                        }
                    }

                    if (!ConexionMaster.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            mLoggingService.AgregarEntrada("Abro conexión master a virtuoso " + numIntentos);
                            QuickOpen(ConexionMaster);
                            mLoggingService.AgregarEntrada("conexión master abierta a virtuoso " + ConexionMaster.ConnectionString);
                        }
                        catch
                        {
                            ConexionMaster = null;
                            if (numIntentos == numMaxIntentos)
                            {
                                throw;
                            }
                        }
                    }
                }
                return ConexionMaster;
            }
        }


        /// <summary>
        /// Obtiene la conexión a virtuoso
        /// </summary>
        public VirtuosoConnection ParentConnection
        {
            get
            {
                int numIntentos = 0;
                int numMaxIntentos = 9;
                int conectado = 0;

                while ((Connection == null || !Connection.State.Equals(ConnectionState.Open)) && (numIntentos < numMaxIntentos))
                {
                    if (numIntentos == 3 || numIntentos == 6 || conectado.Equals(2)) //Espero 1 segundo antes de intentarlo otras 3 veces.
                    {
                        if (!conectado.Equals(2))
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    numIntentos++;

                    if (!ComprobarConexionValida(Connection))
                    {

                        if (!ComprobarConexionValida(Connection))
                        {
                            Connection = new VirtuosoConnection(CadenaConexion);
                        }
                    }

                    if (!Connection.State.Equals(ConnectionState.Open))
                    {
                        try
                        {
                            mLoggingService.AgregarEntrada("Abro conexión a virtuoso " + numIntentos);
                            //mConexion.Open();
                            conectado = QuickOpen(Connection, 200);
                            mLoggingService.AgregarEntrada("conexión abierta a virtuoso " + Connection.ConnectionString);
                        }
                        catch (Exception ex)
                        {
                            mLoggingService.GuardarLogError(ex);

                            if (numIntentos == numMaxIntentos)
                            {
                                throw;
                            }
                        }
                        finally
                        {
                            if (!conectado.Equals(1))
                            {
                                try
                                {
                                    Connection.Dispose();
                                    Connection = null;
                                }
                                catch (Exception e)
                                {
                                    mLoggingService.GuardarLogError(e);
                                }
                            }
                        }
                    }
                }
                return Connection;
            }
        }

        public int GetTimeOutVirtuoso()
        {
            return TimeOutVirtuoso;
        }

        public void SetTimeOutVirtuoso(int value)
        {
            TimeOutVirtuoso = value;
        }
        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarEnReplicacion(string pQuery, string pTablaReplicacion, bool pUsarHttpPost = true)
        {
            List<string> listaQuerys = new List<string>();
            listaQuerys.Add(pQuery);

            InsertarEnReplicacion(listaQuerys, pTablaReplicacion, pUsarHttpPost);
        }
        /// <summary>
        /// Se inserta la query en la cola de replicación pasada como parámetro
        /// </summary>
        /// <param name="pQuery">Query que se va a ejecutar</param>
        /// <param name="pPrioridad">Prioridad que tiene la query</param>
        /// <param name="pTablaReplicacion">Tabla donde se va a replicar la query.</param>
        /// <param name="pInfoExtra">Información extra para el procesado por el base.</param>
        public void InsertarEnReplicacion(List<string> pListaQuerys, string pTablaReplicacion, bool pUsarHttpPost = true)
        {
            string rabbitBD = RabbitMQClient.BD_REPLICACION;

            string exchange = "";

            if (string.IsNullOrEmpty(pTablaReplicacion))
            {
                pTablaReplicacion = COLA_REPLICACION_MASTER;
            }
            if (string.Equals(pTablaReplicacion, COLA_REPLICACION_MASTER) || string.Equals(pTablaReplicacion, COLA_REPLICACION_MASTER_HOME))
            {
                exchange = pTablaReplicacion;
            }

            try
            {

                if (!string.IsNullOrEmpty(mConfigService.ObtenerRabbitMQClient(rabbitBD)))
                {
                    KeyValuePair<List<string>, bool> datosReplicacion = new KeyValuePair<List<string>, bool>(pListaQuerys, pUsarHttpPost);

                    using (RabbitMQClient rMQ = new RabbitMQClient(rabbitBD, pTablaReplicacion, mLoggingService, mConfigService, exchange))
                    {
                        rMQ.AgregarElementoACola(JsonConvert.SerializeObject(datosReplicacion));
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Error al intentar replicar");
            }
        }
        /// <summary>
        /// Inicia una transacción, si no estaba ya iniciada
        /// </summary>
        /// <returns>True si ha inicado la transacción, false si ya estaba iniciada</returns>
        public bool IniciarTransaccion(bool entity = false)
        {
            if (UsarClienteTradicional)
            {
                string nombreTransaccion = $"VirtuosoTransaccion";
                if (TransaccionVirtuoso != null)
                {
                    //transaccion = (DbTransaction)UtilPeticion.ObtenerObjetoDePeticion(nombreTransaccion);
                    return false;
                }
                else
                {
                    DbTransaction transaccion = this.ParentConnectionMaster.BeginTransaction();

                    TransaccionVirtuoso = transaccion;

                    if (NoConfirmarTransacciones)
                    {
                        TransaccionesPendientes.Add(nombreTransaccion, transaccion);
                    }
                    return true;
                }
            }
            return false;
        }
        public KeyValuePair<string, string> ObtenerIpVirtuosoDeCadenaConexion(string pCadenaConexion)
        {
            string ip = null;
            string puerto = mConfigService.ObtenerPuertoVirtuoso();//"8890";

            pCadenaConexion = pCadenaConexion.ToLower();
            char[] separadores = { ';' };
            string[] parametros = pCadenaConexion.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

            var host = parametros.Where(item => item.Trim().StartsWith("host"));

            if (host.Any())
            {
                ip = host.First().Substring(host.First().IndexOf('=') + 1).Trim();
                if (ip.Contains(':'))
                {
                    string puertoAux = ip.Substring(ip.IndexOf(':') + 1);
                    if (!puertoAux.Equals(mConfigService.ObtenerPuertoVirtuosoAux()))//"1111"
                    {
                        puerto = puertoAux;
                    }
                    ip = ip.Substring(0, ip.IndexOf(':'));
                }
            }

            return new KeyValuePair<string, string>(ip, puerto);
        }
        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pGrafo">Grafo que se va a actualiar</param>
        /// <param name="pReplicar">Verdad si esta consulta debe replicarse (por defecto TRUE)</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public int ActualizarVirtuosoClienteTradicional(string pQuery, string pGrafo, bool pReplicar, short pPrioridad, VirtuosoConnection pConexion, string pCadenaConexion = null)
        {
            int resultado = 0;

            VirtuosoCommand myCommand = null;
            VirtuosoConnection conexion = null;

            try
            {
                //if (EsConexionHAPROXY())
                //{

                //    string nombreConexionAfinidad = "conexionAfinidadVirtuoso" + (FicheroConfiguracionMaster.ToLower().Contains("home") ? "Home" : "");
                //    UtilPeticion.AgregarObjetoAPeticionActual(nombreConexionAfinidad, $"{cabeceraServidor.FirstOrDefault()}_Master");
                //    string nombreFechaFinAfinidad = "fechaFinAfinidadVirtuoso" + (FicheroConfiguracionMaster.ToLower().Contains("home") ? "Home" : "");
                //    UtilPeticion.AgregarObjetoAPeticionActual(nombreFechaFinAfinidad, DateTime.Now.AddMinutes(1));
                //}

                mLoggingService.AgregarEntrada("Actualizo virtuoso");
                conexion = pConexion;//ObtenerConexionParaGrafo(pGrafo, true, pCadenaConexion);

                myCommand = new VirtuosoCommand(pQuery, conexion);
                myCommand.CommandTimeout = TimeOutVirtuoso;

                resultado = myCommand.ExecuteNonQuery();

                //EscribirLogActualizarVirtuoso(pQuery, conexion.ConnectionString, resultado);

                mLoggingService.AgregarEntrada("Actualizado con éxito: " + pQuery);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);

                if (TransaccionVirtuoso == null)
                {
                    // Si hay una transacción activa no cerramos las conexiones, para que el que la haya lanzado haga un rollback
                    CerrarConexion();
                }
                if (ex.Message.Contains("syntax error"))
                {
                    throw new ExcepcionDeBaseDeDatos(pQuery, ex);
                }
                throw new ExcepcionConectionFailVirtuoso();
            }
            finally
            {
                try
                {
                    if (myCommand != null)
                    {
                        myCommand.Dispose();
                    }

                    myCommand = null;
                    conexion = null;
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                }
            }

            return resultado;
        }
        public int ActualizarVirtuoso_WebClient(string pUrl, string pQuery, NameValueCollection pParametros)
        {
            RiamWebClient webClient = new RiamWebClient(TimeOutVirtuoso);
            webClient.Encoding = Encoding.UTF8;
            //no se necesita la cabecera
            //webClient.Headers.Add(HttpRequestHeader.ContentType, "application/sparql-query"); //"application/x-www-form-urlencoded"

            DateTime horaInicio = DateTime.Now;
            int milisegundos = 0;
            string error = null;
            int resultado = 0;

            try
            {
                byte[] responseArray = webClient.UploadValues(pUrl, "POST", pParametros);
                milisegundos = (int)DateTime.Now.Subtract(horaInicio).TotalMilliseconds;
                string respuesta = System.Text.Encoding.UTF8.GetString(responseArray);

                resultado = ObtenerResultadoRespuesta(respuesta);
            }
            catch (System.Net.WebException webException)
            {
                milisegundos = (int)DateTime.Now.Subtract(horaInicio).TotalMilliseconds;
                string respuesta = "";

                try
                {
                    //Intento recuperar información del error
                    StreamReader dataStream = new StreamReader(webException.Response.GetResponseStream());
                    respuesta = dataStream.ReadToEnd();
                    webException.Response.Close();
                }
                catch { }

                respuesta += "\n\nQuery: " + pQuery;
                respuesta += "\n\nUrl: " + pUrl;
                error = respuesta;
                mLoggingService.GuardarLogError(webException, respuesta);

                if (webException.Status.Equals(WebExceptionStatus.ConnectFailure) || webException.Message.Contains("(503)"))
                {
                    throw new ExcepcionCheckpointVirtuoso();
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pQuery + "\n\n" + error, webException);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                mLoggingService.GuardarLogError(ex, "\n\nQuery: " + pQuery + "\n\nUrl: " + pUrl);
                throw new ExcepcionDeBaseDeDatos(pQuery, ex);
            }
            finally
            {
                webClient.Dispose();

                if (milisegundos > 700)
                {
                    mLoggingService.GuardarLogConsultaCostosa(string.Format("Consulta: {0} \r\nTiempo transcurrido:\r\n{1} \r\nUrl:\r\n{2} \r\nError:\r\n{3}", pQuery, milisegundos, pUrl, error));
                }
            }

            return resultado;
        }

        /// <summary>
        /// Cierra una conexión existente a virtuoso que ha sido previamente abierta
        /// </summary>
        /// <param name="pClaveConexion">Clave con la que se ha almacenado la conexión para esta petición</param>
        public void CerrarConexion()
        {
            try
            {
                if (Connection != null && Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                    Connection.Dispose();
                    Connection = null;
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
            }
        }
    }
}
