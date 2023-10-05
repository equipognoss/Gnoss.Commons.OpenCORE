using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.Trazas;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Es.Riam.Gnoss.Servicios
{
    /// <summary>
    /// Controlador para un servicio GNOSS.
    /// </summary>
    public abstract class ControladorServicioGnoss : ICloneable
    {
        #region Miembros

        /// <summary>
        /// Intervalo de tiempo que espera el proceso entre ejecuciones
        /// </summary>
        public static int INTERVALO_SEGUNDOS = 60;

        /// <summary>
        /// IP del servidor desde el que se envia el estado del servicio 
        /// </summary>
        public static int HORA_ARRANQUE;

        ///// <summary>
        ///// Puerto que atiende a las peticiones UDP de visitas de la web.
        ///// </summary>
        //public static int PUERTO_UDP_VISITAS;

        ///// <summary>
        ///// Puerto que atiende a las peticiones UDP del servicio LevantaServicios
        ///// </summary>
        //public static int PUERTO_UDP_LEVANTA;

        ///// <summary>
        ///// IP del servidor desde el que se envia el estado del servicio 
        ///// </summary>
        //public static string IP_MANTENIMIENTO;

        /// <summary>
        /// Fichero con las ip de los servidores que están caídos.
        /// </summary>
        public static string mFicheroControlServidores;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos
        /// </summary>
        protected string mFicheroConfiguracionBDOriginal;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos
        /// </summary>
        protected string mFicheroConfiguracionBD;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos
        /// </summary>
        protected string mFicheroConfiguracionHomeBD;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos LIVE
        /// </summary>
        protected string mFicheroConfiguracionBDLive;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos BASE
        /// </summary>
        protected string mFicheroConfiguracionBDBase;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos
        /// </summary>
        protected string mFicheroConfiguracionBDRecursos;

        /// <summary>
        /// Ruta al archivo de configuración de la base de datos TAGS.
        /// </summary>
        protected string mFicheroConfiguracionBDTags;

        /// <summary>
        /// Ruta que se usara para los ficheros de log
        /// </summary>
        protected string mFicheroLog;

        /// <summary>
        /// Ruta que se usara para los ficheros de log
        /// </summary>
        protected string mDirectorioLog;

        protected string mPlataforma;

        /// <summary>
        /// Ruta que se usará para esribir los ficheros con las triples
        /// </summary>
        public string mFicheroEscritura;

        /// <summary>
        /// Variable con el dominio sobre el que se ejecuta el hilo
        /// </summary>
        protected string mDominio;

        /// <summary>
        /// Variable con los idiomas del ecosistema
        /// </summary>
        protected Dictionary<string, string> mListaIdiomasEcosistema = new Dictionary<string, string>();

        /// <summary>
        /// Variable con la urlIntragnoss
        /// </summary>
        protected string mUrlIntragnoss;

        /// <summary>
        /// Token de cancelación para abortar de manera segura los hilos creados.
        /// </summary>
        private CancellationToken mTokenCancelacion;

        private GestorParametroAplicacion mGestorAplicacion;

        protected string mVersion = "1.0";

        public string SOCKETSOFFLINE_ACTIVADOR_UDP = "ActivarHiloUDP";
        public string SOCKETSOFFLINE_CANCELADOR_HILO_UDP = "CancelarHiloUDP";

        /// <summary>
        /// Base Static
        /// </summary>
        private Dictionary<Guid, string> mDicUrlStatics = null;

        /// <summary>
        /// Diccionario con las UrlContent de cada proyecto
        /// </summary>
        private Dictionary<Guid, string> mDicUrlContent = null;

        public int ThreadID { get; private set; }

        protected RabbitMQClient RabbitMqClientLectura;
        protected bool mReiniciarLecturaRabbit = false;
        protected ConfigService mConfigService;
        protected EntityContext mEntityContext;
        protected LoggingService mLoggingService;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private IHostingEnvironment mEnv;
        private static object BLOQUEO_COMPROBACION_TRAZA = new object();
        private static DateTime HORA_COMPROBACION_TRAZA;

        protected IServiceScopeFactory ScopedFactory { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public ControladorServicioGnoss(IServiceScopeFactory scopedFactory, ConfigService configService)
        {
            ScopedFactory = scopedFactory;
            mConfigService = configService;
            //string directorioLog = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + mPlataforma;
            string directorioLog = ObtenerRutaLog();

            mFicheroConfiguracionBD = "acid_Master";
            mFicheroConfiguracionHomeBD = "acidHome_Master";
            mFicheroConfiguracionBDLive = "live_Master";
            mFicheroConfiguracionBDBase = "base_Master";
            mFicheroConfiguracionBDTags = "tags_Master";
            mFicheroConfiguracionBDRecursos = "recursos_Master";

            if (!Directory.Exists(directorioLog))
            {
                if (File.Exists(directorioLog))
                {
                    File.Delete(directorioLog);
                }
                Directory.CreateDirectory(directorioLog);
            }
            //mFicheroLog = directorioLog + Path.DirectorySeparatorChar + nombreLog;
            mDirectorioLog = directorioLog;
            mFicheroEscritura = directorioLog + Path.DirectorySeparatorChar;
        }
        protected string ObtenerRutaLog()
        {
            //Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + mPlataforma
            string ruta = "";
            if (!string.IsNullOrEmpty(mPlataforma))
            {
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", mPlataforma);
            }
            else
            {
                ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            }

            return ruta;
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta al archivo de configuración de la base de datos</param>
        public ControladorServicioGnoss(string pFicheroConfiguracionBD, IServiceScopeFactory scopedFactory)
        {
            ScopedFactory = scopedFactory;

            mFicheroConfiguracionBDOriginal = pFicheroConfiguracionBD;
            mFicheroConfiguracionBD = "acid_Master";
            mFicheroConfiguracionHomeBD = "acidHome_Master";
            mFicheroConfiguracionBDLive = "live_Master";
            mFicheroConfiguracionBDBase = "base_Master";
            mFicheroConfiguracionBDTags = "tags_Master";
            mFicheroConfiguracionBDRecursos = "recursos_Master";

            mPlataforma = Path.GetFileNameWithoutExtension(pFicheroConfiguracionBD);
            //string directorioLog = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + mPlataforma;
            string directorioLog = ObtenerRutaLog();

            if (!Directory.Exists(directorioLog))
            {
                if (File.Exists(directorioLog))
                {
                    File.Delete(directorioLog);
                }
                Directory.CreateDirectory(directorioLog);
            }
            //mFicheroLog = directorioLog + Path.DirectorySeparatorChar + nombreLog;
            mDirectorioLog = directorioLog;
            mFicheroEscritura = directorioLog + Path.DirectorySeparatorChar;
        }

        #endregion

        #region Métodos

        public void EmpezarMantenimiento()
        {
            LoggingService.RUTA_DIRECTORIO_ERROR = mDirectorioLog;
            using (var scope = ScopedFactory.CreateScope())
            {
                EntityContext entityContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
                EntityContextBASE entityContextBASE = scope.ServiceProvider.GetRequiredService<EntityContextBASE>();
                /*UtilidadesVirtuoso utilidadesVirtuoso = scope.ServiceProvider.GetRequiredService<UtilidadesVirtuoso>();*/
                LoggingService loggingService = scope.ServiceProvider.GetRequiredService<LoggingService>();
                RedisCacheWrapper redisCacheWrapper = scope.ServiceProvider.GetRequiredService<RedisCacheWrapper>();
                GnossCache gnossCache = scope.ServiceProvider.GetRequiredService<GnossCache>();
                VirtuosoAD virtuosoAD = scope.ServiceProvider.GetRequiredService<VirtuosoAD>();
                IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication = scope.ServiceProvider.GetRequiredService<IServicesUtilVirtuosoAndReplication>();
                LoggingService.TrazaHabilitada = mConfigService.TrazaHabilitada();
                loggingService.GuardarLog($"Trazas habilitadas: {LoggingService.TrazaHabilitada}");
                try
                {
                    RegistrarInicio(loggingService);


                    loggingService.PLATAFORMA = mPlataforma;

                    ThreadID = Thread.CurrentThread.ManagedThreadId;
                    // Por si existen cosas de otro hilo, las elimino
                    //UtilPeticion.EliminarObjetosDeHilo(ThreadID);

                    GestorParametroAplicacion tempGestorParametros = new GestorParametroAplicacion();


                    ConfiguracionBBDD configLogStash = entityContext.ConfiguracionBBDD.FirstOrDefault(configuracionBBDD => configuracionBBDD.TipoConexion.Equals((short)TipoConexion.Logstash));
                    if (configLogStash != null)
                    {
                        LoggingService.InicializarLogstash(configLogStash.Conexion);
                    }

                    tempGestorParametros.ListaConfiguracionBBDD = entityContext.ConfiguracionBBDD.ToList();
                    tempGestorParametros.ListaConfiguracionServicios = entityContext.ConfiguracionServicios.ToList();
                    tempGestorParametros.ListaConfiguracionServiciosProyecto = entityContext.ConfiguracionServiciosProyecto.ToList();
                    tempGestorParametros.ListaConfiguracionServiciosDominio = entityContext.ConfiguracionServiciosDominio.ToList();
                    GestorParametroAplicacionDS = tempGestorParametros;
                    //CargarServicios(tempGestorParametros);


                    if (GestorParametroAplicacionDS.ParametroAplicacion == null || GestorParametroAplicacionDS.ParametroAplicacion.Count == 0)
                    {
                        GestorParametroAplicacionDS.ParametroAplicacion = entityContext.ParametroAplicacion.ToList();
                        GestorParametroAplicacionDS.ListaProyectoRegistroObligatorio = entityContext.ProyectoRegistroObligatorio.ToList();
                        GestorParametroAplicacionDS.ListaProyectoSinRegistroObligatorio = entityContext.ProyectoSinRegistroObligatorio.ToList();
                        GestorParametroAplicacionDS.ListaAccionesExternas = entityContext.AccionesExternas.ToList();
                        GestorParametroAplicacionDS.ListaConfigApplicationInsightsDominio = entityContext.ConfigApplicationInsightsDominio.ToList();
                        GestorParametroAplicacionDS.ListaTextosPersonalizadosPlataforma = entityContext.TextosPersonalizadosPlataforma.ToList();
                    }

                    EstablecerDominioCache();

                    string serviceName = this.ToString().Replace("Es.Riam.Gnoss.Win.", "");
                    serviceName = serviceName.Substring(0, serviceName.IndexOf('.'));

                    string bdName = BaseCL.DominioEstatico;

                    RabbitMQClient.ClientName = $"{serviceName}_{bdName}";

                    RealizarMantenimiento(entityContext, entityContextBASE, null, loggingService, redisCacheWrapper, gnossCache, virtuosoAD, servicesUtilVirtuosoAndReplication);
                }
                catch (Exception ex)
                {
                    loggingService.GuardarLogError(ex, $"Conexion: {mFicheroConfiguracionBDOriginal}");
                    throw;
                }
            }
        }


        protected void ComprobarTraza(string NombreTraza, EntityContext mEntityContext, LoggingService mLoggingService, RedisCacheWrapper mRedisCacheWrapper, ConfigService mConfigService, IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication)
        {
            if (DateTime.Now > HORA_COMPROBACION_TRAZA)
            {
                lock (BLOQUEO_COMPROBACION_TRAZA)
                {
                    if (DateTime.Now > HORA_COMPROBACION_TRAZA)
                    {
                        HORA_COMPROBACION_TRAZA = DateTime.Now.AddSeconds(15);
                        TrazasCL trazasCL = new TrazasCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                        string tiempoTrazaResultados = trazasCL.ObtenerTrazaEnCache(NombreTraza);

                        if (!string.IsNullOrEmpty(tiempoTrazaResultados))
                        {
                            int valor = 0;
                            int.TryParse(tiempoTrazaResultados, out valor);
                            LoggingService.TrazaHabilitada = true;
                            LoggingService.TiempoMinPeticion = valor; //Para sacar los segundos
                        }
                        else
                        {
                            LoggingService.TrazaHabilitada = false;
                            LoggingService.TiempoMinPeticion = 0;
                        }
                    }
                }
            }
        }

        protected void GuardarTraza(LoggingService pLoggingService)
        {
            pLoggingService.GuardarTraza(ObtenerRutaTraza());
        }

        private string ObtenerRutaTraza()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "trazas");

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            ruta = Path.Combine(ruta, $"traza_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

            return ruta;
        }

        /// <summary>
        /// Establece el dominio de la cache.
        /// </summary>
        private void EstablecerDominioCache()
        {
            GestorParametroAplicacion gestorParametros = GestorParametroAplicacionDS;

            string dominio = gestorParametros.ParametroAplicacion.First(parametro => parametro.Parametro.Equals("UrlIntragnoss")).Valor;

            dominio = dominio.Replace("http://", "").Replace("https://", "").Replace("www.", "");

            if (dominio[dominio.Length - 1] == '/')
            {
                dominio = dominio.Substring(0, dominio.Length - 1);
            }

            BaseCL.DominioEstatico = dominio;
        }

        /// <summary>
        /// Realiza las tareas del servicio.
        /// </summary>
        public virtual void RealizarMantenimiento(EntityContext entityContext, EntityContextBASE entityContextBASE, UtilidadesVirtuoso utilidadesVirtuoso, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            throw new Exception("Implementar");
        }

        /// <summary>
        /// Escribe una entrada en el fichero de log, indicando que el servicio ha sido iniciado
        /// </summary>
        public virtual void RegistrarInicio(LoggingService loggingService)
        {
            GuardarLog($"{LogStatus.Inicio.ToString().ToUpper()} => {this.ToString().Replace("Es.Riam.Gnoss.Win.", "")}", loggingService);
        }

        /// <summary>
        /// Escribe una entrada en el fichero de log, indicando que el servicio ha sido detenido
        /// </summary>
        public void RegistrarParada(LoggingService loggingService)
        {
            GuardarLog(LogStatus.Parada.ToString().ToUpper(), loggingService);
        }

        /// <summary>
        /// Envía un correo con un mensaje de error y guarda una línea en el fichero log correspondiente.
        /// </summary>
        /// <param name="pMensajeError">Mensaje de error</param>
        /// <param name="pVersion">Versión</param>
        /// <returns></returns>
        public void EnviarCorreoErrorYGuardarLog(Exception pExcepcion, string pVersion, EntityContext entityContext, LoggingService loggingService)
        {
            EnviarCorreoErrorYGuardarLog(loggingService.DevolverCadenaError(pExcepcion, pVersion), pVersion, null, entityContext, loggingService);
        }

        /// <summary>
        /// Envía un correo con un mensaje de error y guarda una línea en el fichero log correspondiente.
        /// </summary>
        /// <param name="pMensajeError">Mensaje de error</param>
        /// <param name="pVersion">Versión</param>
        /// <returns></returns>
        public void EnviarCorreoErrorYGuardarLog(string pMensajeError, string pVersion, EntityContext entityContext, LoggingService loggingService)
        {
            EnviarCorreoErrorYGuardarLog(pMensajeError, pVersion, null, entityContext, loggingService);
        }

        /// <summary>
        /// Envía un mensaje a la cuenta de errores y guarda un log del error.
        /// </summary>
        /// <param name="pMensajeError">Mensaje de error</param>
        /// <param name="pTitulo">Versión</param>
        /// <param name="pClaveCorreoDestinatario"></param>
        protected void EnviarCorreoErrorYGuardarLog(string pMensajeError, string pVersion, string pClaveCorreoDestinatario, EntityContext entityContext, LoggingService loggingService)
        {
            EnviarCorreo(pMensajeError, pVersion, pClaveCorreoDestinatario, entityContext);
            GuardarLog(pMensajeError, loggingService);
        }

        /// <summary>
        /// Envía un mensaje a la cuenta de errores y guarda un log del error.
        /// </summary>
        /// <param name="pMensajeError">Mensaje de error</param>
        /// <param name="pTitulo">Versión</param>
        /// <param name="pClaveCorreoDestinatario"></param>
        protected void EnviarCorreo(string pMensajeError, string pVersion, string pClaveCorreoDestinatario, EntityContext entityContext)
        {
            List<ParametroAplicacion> filasConfiguracion = entityContext.ParametroAplicacion.ToList();
           
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            foreach (ParametroAplicacion fila in filasConfiguracion)
            {
                parametros.Add(fila.Parametro, fila.Valor);
            }

            if (string.IsNullOrEmpty(pClaveCorreoDestinatario) || !parametros.ContainsKey(pClaveCorreoDestinatario))
            {
                pClaveCorreoDestinatario = "CorreoErrores";
            }

            if (parametros.Count > 0)
            {
                try
                {
                    //UtilCorreo.EnviarCorreoError(pMensajeError, pVersion, (string)parametros["CorreoErrores"], (string)parametros[pClaveCorreoDestinatario], (string)parametros["ServidorSmtp"], int.Parse(parametros["PuertoSmtp"]), (string)parametros["UsuarioSmtp"], (string)parametros["PasswordSmtp"]);
                }
                catch (Exception)
                {
                    //UtilCorreo.EnviarCorreoError(pMensajeError, pVersion, null, null, null, -1, null, null);
                }
            }
            else
            {
                //UtilCorreo.EnviarCorreoError(pMensajeError, pVersion, null, null, null, -1, null, null);
            }
        }

        /// <summary>
        /// Escribe fisicamente las entradas en el log
        /// </summary>
        /// <param name="pInfoEntry"></param>
        /// <param name="pParametroExtra"></param>
        public void GuardarLog(Exception pExcepcion, LoggingService loggingService)
        {
            GuardarLog(pExcepcion, "", loggingService);
        }

        /// <summary>
        /// Escribe fisicamente las entradas en el log
        /// </summary>
        /// <param name="pInfoEntry"></param>
        /// <param name="pParametroExtra"></param>
        public void GuardarLog(Exception pExcepcion, string pParametroExtra, LoggingService loggingService)
        {
            GuardarLog(loggingService.DevolverCadenaError(pExcepcion, "1.0.0.0"), pParametroExtra, loggingService, pExcepcion);
        }

        /// <summary>
        /// Escribe fisicamente las entradas en el log
        /// </summary>
        /// <param name="infoEntry"></param>
        public void GuardarLog(string pInfoEntry, LoggingService loggingService)
        {
            GuardarLog(pInfoEntry, "", loggingService);
        }

        /// <summary>
        /// Escribe fisicamente las entradas en el log indicado
        /// </summary>
        /// <param name="pInfoEntry"></param>
        /// <param name="pParametroExtra"></param>
        public void GuardarLog(string pInfoEntry, string pParametroExtra, LoggingService loggingService, Exception pExcepcion = null)
        {
            if (!string.IsNullOrEmpty(pInfoEntry))
            {
                try
                {
                    string nombreFichero = Path.Combine(LoggingService.RUTA_DIRECTORIO_ERROR, $"log{ (pParametroExtra.Length > 0 ? "_" + pParametroExtra : "") }_{ DateTime.Now.ToString("yyyy-MM-dd") }.log");

                    FileInfo fichero = new FileInfo(nombreFichero);

                    FileStream logFile = null;

                    if (File.Exists(nombreFichero))
                    {
                        if (fichero.Length > 1000000000)
                        {
                            fichero.Delete();
                        }
                        logFile = new FileStream(nombreFichero, FileMode.Append, FileAccess.Write);
                    }
                    else
                    {
                        logFile = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);
                    }

                    TextWriter logWriter = new StreamWriter(logFile, Encoding.UTF8);

                    // Log entry
                    CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.ToString());
                    string logEntry = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss", culture) + " " + pInfoEntry;

                    logWriter.WriteLine(logEntry);
                    logWriter.Close();
                    logFile.Close();

                    loggingService.EnviarLogLogstash(pExcepcion, pInfoEntry);
                }
                catch (Exception ex)
                {
                    loggingService.GuardarLogError(ex);
                }
            }
        }

        /// <summary>
        /// Escribe los triples generados en un fichero ANSII
        /// </summary>
        /// <param name="pInfoEntry"></param>
        /// <param name="pParametroExtra"></param>
        public void EscribirFichero(string pTextoEscribir, string pGrafo, string pExtension, LoggingService loggingService)
        {
            if (pTextoEscribir != string.Empty)
            {
                StreamWriter sw = null;
                try
                {
                    string grafoSinCaracteresRaros = pGrafo.Replace("/", "_").Replace(":", "_");

                    if (!Directory.Exists(mFicheroEscritura + "Tripletas"))
                    {
                        Directory.CreateDirectory(mFicheroEscritura + "Tripletas");
                    }

                    // File access and writing
                    string rutaFichero = mFicheroEscritura + "Tripletas" + Path.DirectorySeparatorChar;
                    string nombreFichero = grafoSinCaracteresRaros + pExtension;

                    nombreFichero = ComprobarEstadoFichero(pGrafo, rutaFichero, nombreFichero, 50000000);

                    //Escribir fichero en UTF-8
                    sw = new StreamWriter(nombreFichero, true, Encoding.Default);
                    sw.WriteLine(pTextoEscribir);
                }
                catch (Exception ex)
                {
                    loggingService.GuardarLogError(ex);
                }
                finally
                {
                    if (sw != null)
                    {
                        sw.Flush();
                        sw.Dispose();
                        sw.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Pasa una cadena de texto a UTF8.
        /// </summary>
        /// <param name="cadena">Cadena</param>
        /// <returns>cadena de texto en UTF8</returns>
        public static string PasarAUtf8(string cadena)
        {
            Encoding EncodingANSI = Encoding.GetEncoding("iso8859-1");
            return EncodingANSI.GetString(Encoding.UTF8.GetBytes(cadena));
        }

        /// <summary>
        /// Comprueba si existe el fichero y si el nombre no sobrepasa el tamaño pasado como parámetro, si lo pasa lo renombra y crea otro nuevo.
        /// </summary>
        /// <param name="nombreFichero">Nombre del fichero</param>
        /// <param name="pTamanoFichero">Tamaño que no debe sobrepasar</param>
        /// <returns>Nombre final del fichero ANSII</returns>
        private string ComprobarEstadoFichero(string pGrafo, string pRutaFichero, string pNombreFichero, int pTamanoFichero)
        {
            FileInfo f = new FileInfo(pNombreFichero);

            string ficheroEscritura = pRutaFichero + pNombreFichero;
            string ficheroGrafo = pRutaFichero + pNombreFichero + ".graph";
            if (f.Exists && f.Length > pTamanoFichero)
            {
                int i = 0;
                string extension = f.Extension;
                string fichero = f.Name.Substring(0, f.Name.LastIndexOf("."));
                string nombreFicheroOriginal = f.Name.Substring(0, f.Name.LastIndexOf("."));

                FileInfo fi = new FileInfo(fichero + extension);
                while (File.Exists(fichero + extension) && fi.Length > pTamanoFichero)
                {
                    fichero = nombreFicheroOriginal + "_" + i++;
                    fi = new FileInfo(fichero + extension);
                }

                ficheroEscritura = pRutaFichero + fichero + extension;
            }

            if (!File.Exists(ficheroEscritura))
            {
                DateTime fechaActual = DateTime.Now;
                string fechaString = fechaActual.ToString("yyyy") + "-" + fechaActual.ToString("MM") + "-" + fechaActual.ToString("dd") + " " + fechaActual.ToString("hh") + ":" + fechaActual.ToString("mm") + ":" + fechaActual.ToString("ss") + "." + fechaActual.Millisecond;

                StreamWriter crearFicheroAnsii = new StreamWriter(ficheroEscritura, true, Encoding.Default);
                crearFicheroAnsii.WriteLine("# Dump of graph <" + pGrafo + ">, as of " + fechaString);
                crearFicheroAnsii.WriteLine("@base <> .");
                crearFicheroAnsii.Flush();
                crearFicheroAnsii.Dispose();
                crearFicheroAnsii.Close();
            }

            if (!File.Exists(ficheroGrafo))
            {
                StreamWriter crearFicheroGraph = new StreamWriter(ficheroGrafo, true, Encoding.Default);
                crearFicheroGraph.Write(pGrafo);
                crearFicheroGraph.Flush();
                crearFicheroGraph.Dispose();
                crearFicheroGraph.Close();
            }

            return ficheroEscritura;
        }

        /// <summary>
        /// Método que cancela un hilo de manera segura.
        /// </summary>
        protected void ComprobarCancelacionHilo()
        {
            if (mTokenCancelacion != null)
            {
                mTokenCancelacion.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Envía un mensaje a la cuenta de errores y guarda un log del error
        /// </summary>
        /// <param name="pExcepcion">Excepción que ha causado el error</param>
        /// <param name="pTitulo">Titulo del mensaje</param>
        protected void EnviarErrorYGuardarLog(Exception pExcepcion, string pTitulo, LoggingService loggingService)
        {
            try
            {
                string error = loggingService.DevolverCadenaError(pExcepcion, VersionEnsamblado());
                GuardarLog(error, "", loggingService, pExcepcion);
            }
            catch (Exception ex)
            {
                loggingService.GuardarLogError(ex);
            }
        }

        /// <summary>
        /// Obtiene la URL del los elementos estaticos de la página
        /// </summary>
        public string BaseURLStatic(Guid pProyectoID, string pDominio, ConfigService configService)
        {
            if (mDicUrlStatics == null)
            {
                mDicUrlStatics = new Dictionary<Guid, string>();
            }

            if (!mDicUrlStatics.ContainsKey(pProyectoID))
            {
                mDicUrlStatics.Add(pProyectoID, configService.ObtenerUrlServicio("urlStatic"));
            }

            return mDicUrlStatics[pProyectoID];
        }

        /// <summary>
        /// URL de los elementos de contenido de la página
        /// </summary>
        public string UrlContent(Guid pProyectoID, string pDominio, ConfigService configService)
        {
            if (mDicUrlContent == null)
            {
                mDicUrlContent = new Dictionary<Guid, string>();
            }

            if (!mDicUrlContent.ContainsKey(pProyectoID))
            {
                mDicUrlContent.Add(pProyectoID, configService.ObtenerUrlServicio("urlContent"));
            }

            return mDicUrlContent[pProyectoID];
        }

        #endregion

        #region Propiedades

        public GestorParametroAplicacion GestorParametroAplicacionDS
        {
            get
            {
                if (mGestorAplicacion == null)
                {
                    //mGestorAplicacion = new GestorParametroAplicacion();
                    //mGestorAplicacion.ListaProyectoRegistroObligatorio = mEntityContext.ProyectoRegistroObligatorio.ToList();
                    //mGestorAplicacion.ListaProyectoSinRegistroObligatorio = mEntityContext.ProyectoSinRegistroObligatorio.ToList();
                    //mGestorAplicacion.ListaAccionesExternas = mEntityContext.AccionesExternas.ToList();
                    //mGestorAplicacion.ListaConfigApplicationInsightsDominio = mEntityContext.ConfigApplicationInsightsDominio.ToList();
                    //mGestorAplicacion.ListaTextosPersonalizadosPlataforma = mEntityContext.TextosPersonalizadosPlataforma.ToList();
                }
                return mGestorAplicacion;
            }
            set
            {
                mGestorAplicacion = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public CancellationToken TokenCancelacion
        {
            get
            {
                return mTokenCancelacion;
            }
            set
            {
                mTokenCancelacion = value;
            }
        }

        /// <summary>
        /// Obtiene o establece si el hilo esta realizando operaciones. 
        /// </summary>
        public bool EstaHiloActivo
        {
            get;
            set;
        }

        /// <summary>
        /// Cliente UDP asociado al controlador.
        /// </summary>
        public UdpClient ClienteUDP { get; set; }

        public virtual string VersionEnsamblado()
        {
            return mVersion;
        }

        public string FicheroConfiguracionBDOriginal
        {
            get
            {
                return mFicheroConfiguracionBDOriginal;
            }
        }

        #endregion

        #region Miembros de ICloneable

        public object Clone()
        {
            return ClonarControlador();

            /*
             * Ejemplo para implementar este método en los controladores. 
             * IMPORTANTE: Cojer la variable mFicheroConfiguracionBDOriginal, 
             * la variable mFicheroConfiguracionBD ¡¡¡¡NO SIRVE!!!!
             * 
              protected override ControladorServicioGnoss ClonarControlador()
              {
                  return new Controlador(mFicheroConfiguracionBDOriginal, ...);
              }
             * 
             * */
        }

        protected abstract ControladorServicioGnoss ClonarControlador();

        /// <summary>
        /// Se ha parado el proceso, hay que cancelar todas las tareas de escucha que estén en marcha
        /// </summary>
        public virtual void CancelarTarea()
        {
            if (RabbitMqClientLectura != null)
            {
                mReiniciarLecturaRabbit = false;
                RabbitMqClientLectura.CerrarConexionLectura();
            }
        }

        protected void OnShutDown()
        {
            mReiniciarLecturaRabbit = true;
        }

        #endregion
    }

    #region Enumeraciones

    /// <summary>
    /// Enumeración para representar el estado del servicio en el log
    /// </summary>
    public enum LogStatus
    {
        Inicio, Parada, Correcto, Creadas, Error, NoCreadas, NoGenerado, Enviando, NoEnviado, LimiteSobrepasado
    }

    #endregion
}
