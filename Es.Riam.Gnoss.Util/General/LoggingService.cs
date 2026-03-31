using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Es.Riam.Gnoss.Util.General
{
    public class LoggingService
    {
        private readonly ILogger _logger;

        private readonly ILoggerFactory _loggerFactory;

        public static UtilTelemetry.UbicacionLogsYTrazas UBICACIONLOGS = UtilTelemetry.UbicacionLogsYTrazas.Archivo;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UtilPeticion _utilPeticion;
        private readonly Usuario _usuario;


        public LoggingService(UtilPeticion utilPeticion, IHttpContextAccessor httpContextAccessor, Usuario usuario, ILoggerFactory loggerFactory, ILogger<LoggingService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _utilPeticion = utilPeticion;
            _usuario = usuario;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }
        public LoggingService(UtilPeticion utilPeticion, Usuario usuario, ILoggerFactory loggerFactory, ILogger<LoggingService> logger)
        {
            _utilPeticion = utilPeticion;
            _usuario = usuario;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        #region Miembros estáticos

        /// <summary>
        /// Almacena el estado de la traza (habilitada / deshabilitada)
        /// </summary>
        private static bool mTrazaHabilitada = false;

        /// <summary>
        /// Almacena el tiempo minimo de la peticion para que se guarde la traza
        /// </summary>
        private static int mTiempoMinPeticion = 0;
        public static UtilTelemetry.UbicacionLogsYTrazas UBICACIONTRAZA = UtilTelemetry.UbicacionLogsYTrazas.Archivo;


        #endregion

        #region Metodos

        /// <summary>
        /// Escribe unan entrada en el log con el nivel TRACE
        /// </summary>
        /// <param name="pMensaje">Mensaje a insertar en el log</param>
        /// <param name="pLogger">Implementacion del sistema de registros</param>
        public void GuardarLogTrace(string pMensaje, ILogger pLogger)
        {
            try
            {
                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);

                //Escribo el error
                pLogger.LogTrace(mensajeLog);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Escribe unan entrada en el log con el nivel DEBUG
        /// </summary>
        /// <param name="pMensaje">Mensaje a insertar en el log</param>
        /// <param name="pLogger">Implementacion del sistema de registros</param>
        public void GuardarLogDebug(string pMensaje, ILogger pLogger)
        {
            try
            {
                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);

                //Escribo el error
                pLogger.LogDebug(mensajeLog);
            }
            catch
            {

            }
        }

        ///Guardar el log de ver donde pasa;
        /// <param name="pExcepcion">Excepción producida</param>
        public void GuardarLog(string pError, ILogger pLogger)
        {
            try
            {

                string mensajeLog = PrepararMensajeLog(pError, string.Empty);

                //Escribo el error
                pLogger.LogInformation(mensajeLog);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Escribe unan entrada en el log con el nivel WARNING
        /// </summary>
        /// <param name="pMensaje">Mensaje a insertar en el log</param>
        /// <param name="pLogger">Implementacion del sistema de registros</param>
        public void GuardarLogWarning(string pMensaje, ILogger pLogger)
        {
            try
            {
                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);

                //Escribo el error
                pLogger.LogWarning(mensajeLog);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        public void GuardarLogError(Exception pExcepcion, ILogger pLogger)
        {
            GuardarLogError(pExcepcion, null, pLogger);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        /// <param name="pMensajeExtra">Mensaje extra a guardar</param>
        public void GuardarLogError(Exception pExcepcion, string pMensajeExtra, ILogger pLogger, bool pErrorCritico = false, string pTipoError = "-")
        {
            try
            {
                if (!(pExcepcion is ThreadAbortException))
                {
                    GuardarLogError(DevolverCadenaError(pExcepcion, "") + $" \"{pMensajeExtra}\"", pLogger, pTipoError);
                    if (pExcepcion.InnerException != null)
                    {
                        GuardarLogError(pExcepcion.InnerException, pMensajeExtra, pLogger, pErrorCritico, "INNER EXCEPTION");
                    }
                }
            }
            catch
            { }
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        public void GuardarLogError(string pError, ILogger pLogger, string pTipoError = "-")
        {
            try
            {
                string mensajeLog = PrepararMensajeLog(pError, pTipoError);

                //Escribo el error
                pLogger.LogError(mensajeLog);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Escribe unan entrada en el log con el nivel CRITICAL
        /// </summary>
        /// <param name="pMensaje">Mensaje a insertar en el log</param>
        /// <param name="pLogger">Implementacion del sistema de registros</param>
        public void GuardarLogCritical(string pMensaje, ILogger pLogger)
        {
            try
            {
                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);

                //Escribo el error
                pLogger.LogCritical(mensajeLog);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        public string GuardarLogErrorView(Exception pExcepcion)
        {
            GuardarLogError(pExcepcion, null);
            return string.Empty;
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        /// <param name="pMensajeExtra">Mensaje extra a guardar</param>
        public void GuardarLogErrorRedis(Exception pExcepcion, string pMensajeExtra, bool pErrorCritico = false)
        {
            GuardarLogError(DevolverCadenaError(pExcepcion, "") + pMensajeExtra, _logger);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        public void GuardarLogConsultaCostosa(string pError)
        {
            GuardarLogTrace(pError, _logger);
        }

        /// <summary>
        /// Devuelve el mensaje de error personalizado
        /// </summary>
        /// <param name="e">Excepción</param>
        /// <param name="pVersion">Versión de Gnoss</param>
        /// <returns></returns>
        public string DevolverCadenaError(Exception pExcepcion, string pVersion)
        {
            string identidadUsuario = "";
            if (_usuario != null && _usuario.UsuarioActual != null)
            {
                identidadUsuario = "\"Versión Gnoss:   " + pVersion + Environment.NewLine + Environment.NewLine + "UsuarioID: " + _usuario.UsuarioActual.UsuarioID.ToString() + " IdentidadID: " + _usuario.UsuarioActual.IdentidadID.ToString() + " ProyectoID: " + _usuario.UsuarioActual.ProyectoID.ToString() + "\" ";
            }
            else
            {
                identidadUsuario = "\"-\" ";
            }

            return identidadUsuario + DevolverCadenaErrorExcepcion(pExcepcion);
        }

        private string PrepararMensajeLog(string pMensaje, string pTipoError)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append($"{pMensaje?.TrimEnd()} \"{pTipoError?.TrimEnd()}\"");

            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null)
                {
                    sb.Append($" \"{_httpContextAccessor.HttpContext.Request.Path.ToString()}\"");

                    sb.Append($" ''");
                    if (_httpContextAccessor.HttpContext.Request.Headers.Keys.Count == 0)
                    {
                        sb.Append($"-");
                    }
                    foreach (string key in _httpContextAccessor.HttpContext.Request.Headers.Keys)
                    {
                        sb.Append($" {key}: {_httpContextAccessor.HttpContext.Request.Headers[key]}");
                    }
                    sb.Append($" ''");
                }
                else
                {
                    sb.Append($" \"-\"");
                    sb.Append($" '-'");
                }
            }
            catch { }
            string linea = sb.ToString().Replace('\r', ' ').Replace('\n', ' ');
            return linea;
        }

        private static DatosError GenerarDatosError(Exception pExcepcion, string pMensajeExtra = null)
        {
            DatosError datosError = new DatosError();
            datosError.Message = pMensajeExtra;
            if (pExcepcion != null)
            {
                if (pExcepcion.TargetSite != null)
                {
                    // Rellenar objeto con datos (Data)
                    datosError.NameSpace = pExcepcion.TargetSite.DeclaringType.Namespace;
                    datosError.Name = pExcepcion.TargetSite.Name;
                }

                datosError.Source = pExcepcion.Source;
                datosError.Type = pExcepcion.GetType().ToString();


                if (!string.IsNullOrEmpty(pMensajeExtra))
                {
                    datosError.Message += $". Error: {pExcepcion.Message}";
                }
                else
                {
                    datosError.Message = pExcepcion.Message;
                }

                datosError.StackTrace = pExcepcion.StackTrace;

                if (pExcepcion.InnerException != null)
                {
                    DatosError inner = new DatosError();
                    datosError.InnerException = GenerarDatosError(pExcepcion.InnerException);
                }

                if (pExcepcion is AggregateException && ((AggregateException)pExcepcion).InnerExceptions.Count > 0)
                {
                    datosError.Message += ". Aggregated exceptions: ";
                    int numero = 1;
                    foreach (Exception exception in ((AggregateException)pExcepcion).InnerExceptions)
                    {
                        datosError.Message += $". Exception {numero++}: {exception.Message}";
                    }
                }
            }

            return datosError;
        }

        private static string DevolverCadenaErrorExcepcion(Exception pExcepcion)
        {
            string target = "";
            string resultado = "";

            if (pExcepcion != null)
            {
                if (pExcepcion.TargetSite != null)
                {
                    target = $"{pExcepcion.TargetSite.DeclaringType.Namespace} {pExcepcion.TargetSite.Name}";
                }
                else
                {
                    target = "- -";
                }

                resultado = $"\"{pExcepcion.Source}\" {target} {pExcepcion.GetType()} \"{pExcepcion.Message}\" \"{pExcepcion.StackTrace}\"";
            }

            return resultado;
        }

        /// <summary>
        /// Reinicia el reloj usado para la telemetría de application insights
        /// </summary>
        public static Stopwatch IniciarRelojTelemetria()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        /// <summary>
        /// Agrega un mensaje a la traza
        /// </summary>
        /// <param name="pMensaje">Mensaje a incluir en la traza</param>
        public void AgregarEntradaTrazaEntity(string pMensaje)
        {
            if (!string.IsNullOrWhiteSpace(pMensaje))
            {
                AgregarEntrada($"SQL: {pMensaje}", false);
            }
        }

        /// <summary>
        /// Agrega un mensaje a la traza
        /// </summary>
        /// <param name="pMensaje">Mensaje a incluir en la traza</param>
        public void AgregarEntrada(string pMensaje)
        {
            AgregarEntrada(pMensaje, false);
        }

        /// <summary>
        /// Agrega un mensaje a la traza
        /// </summary>
        /// <param name="pMensaje">Mensaje a incluir en la traza</param>
        public void AgregarEntrada(string pMensaje, bool pObtenerTrazaYMemoria)
        {
            if (TrazaHabilitada) // if (true)
            {
                try
                {
                    if (!string.IsNullOrEmpty(pMensaje) && TrazaActual != null)
                    {
                        TrazaActual.EscribirEntrada(pMensaje, pObtenerTrazaYMemoria);
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Agrega un mensaje a la traza
        /// </summary>
        /// <param name="pMensaje">Mensaje a incluir en la traza</param>
        public void AgregarEntradaDependencia(string pMensaje, bool pObtenerTrazaYMemoria, string pNombreDependencia, Stopwatch pReloj, bool pExito)
        {
            if (TrazaHabilitada)
            {
                try
                {
                    if (!string.IsNullOrEmpty(pMensaje) && TrazaActual != null)
                    {
                        TrazaActual.EscribirEntrada(pMensaje, pObtenerTrazaYMemoria, pNombreDependencia, pReloj, pExito);
                    }
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Guarda la traza en un archivo de texto
        /// </summary>
        /// <param name="pRutaFicheroTraza">Ruta del archivo de la traza</param>
        public void GuardarTraza(string pRutaFicheroTraza)
        {
            try
            {
                TrazaActual.GuardarTraza(pRutaFicheroTraza);
                _utilPeticion.EliminarObjetoDePeticion("traza");
            }
            catch (Exception) { }

        }

        /// <summary>
        /// Guarda la traza en un archivo de texto
        /// </summary>
        /// <param name="pRutaFicheroTraza">Ruta del archivo de la traza</param>
        public void GuardarTraza()
        {
            try
            {
                TrazaActual.GuardarTraza(ObtenerRutaTraza());
                _utilPeticion.EliminarObjetoDePeticion("traza");
            }
            catch (Exception) { }

        }

        private Traza CrearTraza()
        {
            return CrearTraza(_httpContextAccessor != null && _httpContextAccessor.HttpContext.Request != null);
        }

        public Traza CrearTraza(bool pEsPeticionWeb)
        {
            Traza traza;
            if (pEsPeticionWeb)
            {
                traza = new TrazaWeb(_httpContextAccessor);
            }
            else
            {
                traza = new Traza();
            }

            return traza;
        }

        public static string ObtenerVersion()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "versioninfo.txt");
            string version = null;
            if (File.Exists(ruta))
            {
                using (StreamReader sr = new StreamReader(ruta))
                {
                    version = sr.ReadLine();

                    if (!string.IsNullOrEmpty(version) && version.Contains(":"))
                    {
                        version = version.Substring(version.IndexOf(':') + 1).Trim();
                    }
                }
            }
            return version;
        }

        public static string ObtenerProducto()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "versioninfo.txt");
            string producto = null;
            if (File.Exists(ruta))
            {
                using (StreamReader sr = new StreamReader(ruta))
                {
                    sr.ReadLine(); // primera linea
                    producto = sr.ReadLine(); // segunda linea
                    if (!string.IsNullOrEmpty(producto) && producto.Contains(':'))
                    {
                        producto = producto.Substring(producto.IndexOf(':') + 1).Trim();
                    }
                }
            }
            return producto;
        }

        protected string ObtenerRutaTraza()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "trazas");

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            ruta = Path.Combine(ruta, $"traza_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");

            return ruta;
        }

        public static void ConfigurarLogging(IServiceCollection pServices, IConfiguration pConfiguration)
        {
            bool logToFile = new ConfigService().EscribirLogEnFichero();

            var serilogConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(pConfiguration)
                .Enrich.FromLogContext()  // ← habilita los scopes
                .WriteTo.Console(
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Properties} {Message:lj}{NewLine}{Exception}"
                );

            if (logToFile)
            {
                serilogConfig.WriteTo.File(
                    path: "logs/log_.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Properties} {Message:lj}{NewLine}{Exception}"
                );
            }

            Log.Logger = serilogConfig.CreateLogger();

            // Recarga en caliente cuando cambia el ConfigMap
            ChangeToken.OnChange(
                () => pConfiguration.GetReloadToken(),
                () => Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(pConfiguration)
                    .CreateLogger()
            );

            pServices.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(Log.Logger, dispose: true);
            });
        }

        public ILogger<T> CrearLogger<T>()
        {
            return _loggerFactory.CreateLogger<T>();
        }
        #endregion

        #region Propiedades

        /// <summary>
        /// Habilita o deshabilita la traza
        /// </summary>
        public static bool TrazaHabilitada
        {
            get
            {
                return mTrazaHabilitada;
            }
            set
            {
                mTrazaHabilitada = value;
            }
        }


        /// <summary>
        /// Almacena el tiempo minimo de la peticion para que se guarde la traza
        /// </summary>
        public static int TiempoMinPeticion
        {
            get
            {
                return mTiempoMinPeticion;
            }
            set
            {
                mTiempoMinPeticion = value;
            }
        }

        public static DateTime HoraComprobacionCache { get; set; } = DateTime.MinValue;

        public static int TiempoDuracionComprobacion { get; set; } = 60;

        private Traza mTrazaActual;

        /// <summary>
        /// Obtiene la traza actual
        /// </summary>
        public Traza TrazaActual
        {
            get
            {
                if (mTrazaActual == null)
                {
                    mTrazaActual = CrearTraza();
                }
                return mTrazaActual;
            }
        }

        public static string RUTA_DIRECTORIO_ERROR
        {
            get; set;
        }
        #endregion

    }

    /// <summary>
    /// Traza para guardar mensajes de logging
    /// </summary>
    public class Traza
    {
        #region Miembros

        /// <summary>
        /// Almacena la hora de la última traza guardada
        /// </summary>
        private DateTime mHoraUltimaTraza;

        /// <summary>
        /// Almacena la hora de la última traza guardada
        /// </summary>
        private readonly DateTime mHoraInicio;

        /// <summary>
        /// Almacena toda la traza generada hasta ahora
        /// </summary>
        private StringBuilder mTraza;

        /// <summary>
        /// Almacena toda la traza generada hasta ahora
        /// </summary>
        private readonly List<string> mMensajesTrazaFichero;

        /// <summary>
        /// Mensaje que se agrega al final de la traza
        /// </summary>
        protected string mMensajeFinalTraza = "";

        /// <summary>
        /// Mensaje que se agrega al final de la traza
        /// </summary>
        protected string mMensajeInicioTraza = "";

        /// <summary>
        /// Almacena si la traza estaba habilitada cuando comenzó a regisrarse
        /// </summary>
        protected bool mEstaTrazaHabilitada;

        /// <summary>
        /// Tamaño máximo en bytes del fichero de log de errores
        /// </summary>
        private static long TAMAÑO_MAXIMO_LOG = 102400000;

        #endregion

        #region Constructores
        /// <summary>
        /// Constructor de la Traza
        /// </summary>
        public Traza()
        {
            mMensajesTrazaFichero = new List<string>();
            mEstaTrazaHabilitada = LoggingService.TrazaHabilitada;
            if (mEstaTrazaHabilitada)
            {
                mHoraUltimaTraza = DateTime.Now;
                mHoraInicio = mHoraUltimaTraza;
            }
        }

        #endregion

        #region Métodos

        public void EscribirEntrada(string pEntrada, bool pObtenerTrazaYMemoria, string pNombreDependencia = null, Stopwatch pReloj = null, bool pExito = false)
        {
            DateTime horaActual = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(pEntrada))
            {
                string mensajeTraza = string.Concat((int)horaActual.Subtract(mHoraUltimaTraza).TotalMilliseconds, "|", LimpiarDatos(pEntrada), "|");

                mMensajesTrazaFichero.Add(mensajeTraza);
            }

            mHoraUltimaTraza = horaActual;
        }

        public static string LimpiarDatos(string pDatos)
        {
            if (string.IsNullOrEmpty(pDatos))
            {
                return "";
            }
            return pDatos.Replace("|", "(*)").Replace("\r\n", "(**)").Replace("\n", "(**)").Replace("\t", "(***)");
        }

        public static string ObtenerStackTraceDeCodigo()
        {
            StackTrace st = new StackTrace(true);
            string stackIndent = "";
            char[] delimiter = { ' ' };
            //for (int i = 3; i < st.FrameCount; i++)
            for (int i = st.FrameCount - 1; i > 2; i--)
            {
                // Note that at this level, there are four
                // stack frames, one for each method invocation.

                StackFrame sf = st.GetFrame(i);
                stackIndent += "/" + sf.GetMethod().Name;
            }

            if (stackIndent.StartsWith("/"))
            {
                stackIndent = stackIndent.Substring(1);
            }

            return stackIndent;
        }

        /// <summary>
        /// Escribe el mensaje de fin de traza y devuelve la traza completa
        /// </summary>
        private void FinalizarTraza()
        {
            if (mEstaTrazaHabilitada)
            {
                mMensajeInicioTraza = $"{DateTime.Now.ToString("HH:mm:ss.fff")}|{(int)DateTime.Now.Subtract(mHoraInicio).TotalMilliseconds}|";
            }
        }

        /// <summary>
        /// Guarda la traza en un archivo de texto
        /// </summary>
        /// <param name="pRutaFicheroTraza">Ruta del archivo de la traza</param>
        public void GuardarTraza(string pRutaFicheroTraza)
        {
            TimeSpan tiempoPeticion = mHoraUltimaTraza - mHoraInicio;
            if (tiempoPeticion.TotalSeconds > LoggingService.TiempoMinPeticion)
            {
                try
                {
                    FinalizarTraza();

                    foreach (string mensaje in mMensajesTrazaFichero)
                    {
                        if (mTraza == null)
                        {
                            mTraza = new StringBuilder();
                        }

                        mTraza.Append(mMensajeInicioTraza);
                        mTraza.Append(mensaje);
                        mTraza.AppendLine(mMensajeFinalTraza);
                    }

                    FileInfo fichero = new FileInfo(pRutaFicheroTraza);

                    //Si el fichero supera el tamaño máximo lo elimino
                    if (File.Exists(pRutaFicheroTraza) && fichero.Length > TAMAÑO_MAXIMO_LOG)
                    {
                        LoggingService.TrazaHabilitada = false; // true
                        mEstaTrazaHabilitada = LoggingService.TrazaHabilitada;
                    }

                    if (!fichero.Directory.Exists)
                    {
                        fichero.Directory.Create();
                    }

                    //Añado el error al fichero
                    using (StreamWriter sw = new StreamWriter(pRutaFicheroTraza, true, System.Text.Encoding.Default))
                    {
                        sw.Write(mTraza);
                    }
                }
                catch (Exception) { }
            }
        }
        #endregion
    }

    /// <summary>
    /// Traza para guardar mensajes de logging de una petición web
    /// </summary>
    public class TrazaWeb : Traza
    {
        #region Constructores

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Crea la traza y añade los datos de la petición web
        /// </summary>
        /// <param name="pRutaTraza">Ruta donde se almacena la traza</param>
        public TrazaWeb(IHttpContextAccessor pHttpContextAccessor)
            : base()
        {
            if (mEstaTrazaHabilitada)
            {
                _httpContextAccessor = pHttpContextAccessor;
                if (pHttpContextAccessor.HttpContext != null && pHttpContextAccessor.HttpContext.Request != null)
                {
                    try
                    {
                        HttpRequest request = pHttpContextAccessor.HttpContext.Request;
                        mMensajeFinalTraza += $"|{LimpiarDatos(request.Method)}|{LimpiarDatos(request.PathBase)}|{LimpiarDatos(UriHelper.GetEncodedUrl(request))}|{LimpiarDatos(request.Headers["UserAgent"])}|{ObtenerParametrosPost()}";

                        string ip = request.Headers["X-FORWARDED-FOR"];
                        if (string.IsNullOrEmpty(ip))
                        {
                            ip = pHttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                        }

                        mMensajeFinalTraza += $"|{LimpiarDatos(ip)}|";

                        string cabeceras = "";
                        string separador = "";
                        foreach (string headerKey in request.Headers.Keys)
                        {
                            string valor = request.Headers[headerKey];

                            cabeceras += $"{separador}{headerKey}: {request.Headers[headerKey]}";
                            separador = "$";
                        }

                        mMensajeFinalTraza += $"|{LimpiarDatos(cabeceras)}|";
                    }
                    catch (Exception) { }
                }
            }
        }

        /// <summary>
        /// Obtiene todos los parámetros enviados en el cuerpo de la petición separados por & (param1=valor1&param2=valor2&...)
        /// </summary>
        /// <returns></returns>
        private string ObtenerParametrosPost()
        {
            StringBuilder parametros = new StringBuilder();
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null && _httpContextAccessor.HttpContext.Request.Method.Equals("POST"))
            {
                string separador = "";
                string[] keys = _httpContextAccessor.HttpContext.Request.Form.Keys.ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    parametros.Append($"{separador}{keys[i]}={_httpContextAccessor.HttpContext.Request.Form[keys[i]]}");
                    separador = "&";
                }
            }

            return parametros.ToString();
        }

        #endregion
    }
}
