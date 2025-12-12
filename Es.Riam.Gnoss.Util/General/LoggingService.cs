using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Es.Riam.Gnoss.Util.General
{
    public class LoggingService
    {
        /// <summary>
        /// Tamaño máximo en bytes del fichero de log de errores
        /// </summary>
        private static long TAMAÑO_MAXIMO_LOG = 1073741824;

        private static long TAMAÑO_MAXIMO_LOG_DIARIO = 104857600;
        private Microsoft.Extensions.Logging.ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        private static string ULTIMO = "_last";

        //public static string RUTA_FICHERO_ERROR = "";

        //public static string RUTA_FICHERO_ERROR_REDIS = "";

        //public static string RUTA_FICHERO_CONSULTA_COSTOSA = "";

        public static UtilTelemetry.UbicacionLogsYTrazas UBICACIONLOGS = UtilTelemetry.UbicacionLogsYTrazas.Archivo;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UtilPeticion _utilPeticion;
        private readonly UtilTelemetry _utilTelemetry;
        private readonly Usuario _usuario;

        public LoggingService(UtilPeticion utilPeticion, IHttpContextAccessor httpContextAccessor, UtilTelemetry utilTelemetry, Usuario usuario, ILogger<LoggingService> logger, ILoggerFactory loggerFactory)
        {
            _httpContextAccessor = httpContextAccessor;
            _utilPeticion = utilPeticion;
            _utilTelemetry = utilTelemetry;
            _usuario = usuario;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
        public LoggingService(UtilPeticion utilPeticion, UtilTelemetry utilTelemetry, Usuario usuario, ILogger<LoggingService> logger, ILoggerFactory loggerFactory)
        {
            _utilPeticion = utilPeticion;
            _utilTelemetry = utilTelemetry;
            _usuario = usuario;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Miembros estáticos

        /// <summary>
        /// Almacena el estado de la traza (habilitada / deshabilitada)
        /// </summary>
        private static bool mTrazaHabilitada = false;

        /// <summary>
        /// Almacena el tiempo minimo de la peticion para que se guarde la traza
        /// </summary>
        protected static int mTiempoMinPeticion = 0;

        protected static DateTime mHoraComprobacionCache = DateTime.MinValue;

        protected static int mTiempoDuracionComprobacion = 60; //minimo 1 minuto

        public static UtilTelemetry.UbicacionLogsYTrazas UBICACIONTRAZA = UtilTelemetry.UbicacionLogsYTrazas.Archivo;


        // Creación del objeto Log
        public static Logger Log { get; set; }

        private static string _PRODUCTO = "";

        private static string _VERSION = "";


        #endregion

        #region Metodos

        /// <summary>
        /// Escribe unan entrada en el log con el nivel TRACE
        /// </summary>
        /// <param name="pMensaje">Mensaje a insertar en el log</param>
        /// <param name="pLogger">Implementacion del sistema de registros</param>
        public void GuardarLogTrace(string pMensaje, Microsoft.Extensions.Logging.ILogger pLogger)
        {
            try
            {
                string rutaFichero = ObtenerRutaFichero(null, LogLevel.Trace);

                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Escribo el error
                pLogger.LogTrace(lineaLog);

                //Añado el error al fichero
                EscribirLogEnFichero(rutaFichero, lineaLog);
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
        public void GuardarLogDebug(string pMensaje, Microsoft.Extensions.Logging.ILogger pLogger)
        {
            try
            {
                string rutaFichero = ObtenerRutaFichero(null, LogLevel.Debug);

                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Escribo el error
                pLogger.LogDebug(lineaLog);

                //Añado el error al fichero
                EscribirLogEnFichero(rutaFichero, lineaLog);
            }
            catch
            {

            }
        }

        //Guardar el log de ver donde pasa;
        /// <param name="pExcepcion">Excepción producida</param>
        [Obsolete("Este metodo dejara de estar disponible en futuras versiones, use el metodo con ILogger")]
        public void GuardarLog(string pError, string pRutaFicheroError = null, bool pYaEnviado = false)
        {
            try
            {
                pRutaFicheroError = ObtenerRutaFichero(pRutaFicheroError, LogLevel.Information);

                string mensajeLog = PrepararMensajeLog(pError, string.Empty);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Añado el error al fichero
                EscribirLogEnFichero(pRutaFicheroError, lineaLog);

                if (!pYaEnviado && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo) && UtilTelemetry.EstaConfiguradaTelemetria)
                {
                    try
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(new Exception(), pError);
                    }
                    catch
                    { }
                }

                if (!pYaEnviado)
                {
                    //Envia el error al servidor Logstash
                    EnviarLogLogstash(null, pError);
                }
            }
            catch
            {

            }
        }

        //Guardar el log de ver donde pasa;
        /// <param name="pExcepcion">Excepción producida</param>
        public void GuardarLog(string pError, Microsoft.Extensions.Logging.ILogger pLogger, string pRutaFicheroError = null, bool pYaEnviado = false)
        {
            try
            {
                pRutaFicheroError = ObtenerRutaFichero(pRutaFicheroError, LogLevel.Information);

                string mensajeLog = PrepararMensajeLog(pError, string.Empty);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Escribo el error
                pLogger.LogInformation(lineaLog);

                //Añado el error al fichero
                EscribirLogEnFichero(pRutaFicheroError, lineaLog);

                if (!pYaEnviado && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo) && UtilTelemetry.EstaConfiguradaTelemetria)
                {
                    try
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(new Exception(), pError);
                    }
                    catch
                    { }
                }

                if (!pYaEnviado)
                {
                    //Envia el error al servidor Logstash
                    EnviarLogLogstash(null, pError);
                }
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
        public void GuardarLogWarning(string pMensaje, Microsoft.Extensions.Logging.ILogger pLogger)
        {
            try
            {
                string rutaFichero = ObtenerRutaFichero(null, LogLevel.Warning);

                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Escribo el error
                pLogger.LogWarning(lineaLog);

                //Añado el error al fichero
                EscribirLogEnFichero(rutaFichero, lineaLog);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        [Obsolete("Este metodo dejara de estar disponible en futuras versiones, use el metodo con ILogger")]
        public void GuardarLogError(Exception pExcepcion)
        {
            GuardarLogError(pExcepcion, mlogger);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        public void GuardarLogError(Exception pExcepcion, Microsoft.Extensions.Logging.ILogger pLogger)
        {
            GuardarLogError(pExcepcion, null, pLogger);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        /// <param name="pMensajeExtra">Mensaje extra a guardar</param>
        [Obsolete("Este metodo dejara de estar disponible en futuras versiones, use el metodo con ILogger")]
        public void GuardarLogError(Exception pExcepcion, string pMensajeExtra, bool pErrorCritico = false, string pTipoError = "-")
        {
            if (!UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.ApplicationInsights))
            {
                try
                {
                    if (!(pExcepcion is ThreadAbortException))
                    {
                        GuardarLogError(DevolverCadenaError(pExcepcion, "") + $" \"{pMensajeExtra}\"", null, true, pTipoError);
                        if (pExcepcion.InnerException != null)
                        {
                            GuardarLogError(pExcepcion.InnerException, pMensajeExtra, pErrorCritico, "INNER EXCEPTION");
                        }
                        //JUAN
                        //else if (pExcepcion is AggregateException)
                        //{
                        //    AggregateException aggregateException = (AggregateException)pExcepcion;
                        //    GuardarLogError(aggregateException, pMensajeExtra, pErrorCritico, "Aggregate Exception");
                        //}
                    }
                }
                catch
                { }
            }

            if (UtilTelemetry.EstaConfiguradaTelemetria && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo))
            {
                try
                {
                    if (!(pExcepcion is ThreadAbortException))
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(pExcepcion, pMensajeExtra, pErrorCritico);
                    }
                }
                catch
                { }
            }

            //Envia el error al servidor Logstash
            EnviarLogLogstash(pExcepcion, pMensajeExtra);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        /// <param name="pMensajeExtra">Mensaje extra a guardar</param>
        public void GuardarLogError(Exception pExcepcion, string pMensajeExtra, Microsoft.Extensions.Logging.ILogger pLogger, bool pErrorCritico = false, string pTipoError = "-")
        {
            if (!UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.ApplicationInsights))
            {
                try
                {
                    if (!(pExcepcion is ThreadAbortException))
                    {
                        GuardarLogError(DevolverCadenaError(pExcepcion, "") + $" \"{pMensajeExtra}\"", pLogger, null, true, pTipoError);
                        if (pExcepcion.InnerException != null)
                        {
                            GuardarLogError(pExcepcion.InnerException, pMensajeExtra, pLogger, pErrorCritico, "INNER EXCEPTION");
                        }
                        //JUAN
                        //else if (pExcepcion is AggregateException)
                        //{
                        //    AggregateException aggregateException = (AggregateException)pExcepcion;
                        //    GuardarLogError(aggregateException, pMensajeExtra, pErrorCritico, "Aggregate Exception");
                        //}
                    }
                }
                catch
                { }
            }

            if (UtilTelemetry.EstaConfiguradaTelemetria && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo))
            {
                try
                {
                    if (!(pExcepcion is ThreadAbortException))
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(pExcepcion, pMensajeExtra, pErrorCritico);
                    }
                }
                catch
                { }
            }

            //Envia el error al servidor Logstash
            EnviarLogLogstash(pExcepcion, pMensajeExtra);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        [Obsolete("Este metodo dejara de estar disponible en futuras versiones, use el metodo con ILogger")]
        public void GuardarLogError(string pError, string pRutaFicheroError = null, bool pYaEnviado = false, string pTipoError = "-")
        {
            try
            {
                pRutaFicheroError = ObtenerRutaFichero(pRutaFicheroError, LogLevel.Error);

                string mensajeLog = PrepararMensajeLog(pError, pTipoError);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Añado el error al fichero
                EscribirLogEnFichero(pRutaFicheroError, lineaLog);

                if (!pYaEnviado && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo) && UtilTelemetry.EstaConfiguradaTelemetria)
                {
                    try
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(new Exception(), pError);
                    }
                    catch
                    { }
                }

                if (!pYaEnviado)
                {
                    //Envia el error al servidor Logstash
                    EnviarLogLogstash(null, pError);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        public void GuardarLogError(string pError, Microsoft.Extensions.Logging.ILogger pLogger, string pRutaFicheroError = null, bool pYaEnviado = false, string pTipoError = "-")
        {
            try
            {
                pRutaFicheroError = ObtenerRutaFichero(pRutaFicheroError, LogLevel.Error);

                string mensajeLog = PrepararMensajeLog(pError, pTipoError);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";
                //Escribo el error
                pLogger.LogError(lineaLog);

                //Añado el error al fichero
                EscribirLogEnFichero(pRutaFicheroError, lineaLog);

                if (!pYaEnviado && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo) && UtilTelemetry.EstaConfiguradaTelemetria)
                {
                    try
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(new Exception(), pError);
                    }
                    catch
                    { }
                }

                if (!pYaEnviado)
                {
                    //Envia el error al servidor Logstash
                    EnviarLogLogstash(null, pError);
                }
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
        public void GuardarLogCritical(string pMensaje, Microsoft.Extensions.Logging.ILogger pLogger)
        {
            try
            {
                string rutaFichero = ObtenerRutaFichero(null, LogLevel.Critical);

                string mensajeLog = PrepararMensajeLog(pMensaje, string.Empty);
                string lineaLog = $"[{DateTime.Now}] {mensajeLog}";

                //Escribo el error
                pLogger.LogCritical(mensajeLog);

                //Añado el error al fichero
                EscribirLogEnFichero(rutaFichero, lineaLog);
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


        public void GuardarLogErrorAJAX(string pError)
        {
            GuardarLogError(pError, mlogger ,Path.Combine(RUTA_DIRECTORIO_ERROR, $"errorAJAX_{DateTime.Now.ToString("yyyy-MM-dd")}.log"),true);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        /// <param name="pExcepcion">Excepción producida</param>
        /// <param name="pMensajeExtra">Mensaje extra a guardar</param>
        public void GuardarLogErrorRedis(Exception pExcepcion, string pMensajeExtra, bool pErrorCritico = false)
        {


            if (!UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.ApplicationInsights))
            {
                try
                {
                    GuardarLogError(DevolverCadenaError(pExcepcion, "") + pMensajeExtra,mlogger, Path.Combine(RUTA_DIRECTORIO_ERROR, $"error_redis_{DateTime.Now.ToString("yyyy-MM-dd")}.log"), true);
                }
                catch
                { }
            }

            if (UtilTelemetry.EstaConfiguradaTelemetria && !UBICACIONLOGS.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo))
            {
                try
                {
                    if (!(pExcepcion is ThreadAbortException))
                    {
                        _utilTelemetry.EnviarTelemetriaExcepcion(pExcepcion, pMensajeExtra, pErrorCritico);
                    }
                }
                catch
                { }
            }

            //Envia el error al servidor Logstash
            EnviarLogLogstash(pExcepcion, pMensajeExtra);
        }

        /// <summary>
        /// Guarda el log de error
        /// </summary>
        public void GuardarLogConsultaCostosa(string pError)
        {
            GuardarLogError(pError,mlogger ,Path.Combine(RUTA_DIRECTORIO_ERROR, $"consulta_costosa_{DateTime.Now.ToString("yyyy-MM-dd")}.log"));
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

        private string ObtenerRutaFichero(string pRutaFicheroError, LogLevel pTipoLog)
        {
            string ficheroPorDefecto = (pTipoLog == LogLevel.Error || pTipoLog == LogLevel.Critical) ? "error" : "log";
            if (string.IsNullOrEmpty(pRutaFicheroError))
            {
                pRutaFicheroError = Path.Combine(RUTA_DIRECTORIO_ERROR, $"{ficheroPorDefecto}_{DateTime.Now.ToString("yyyy-MM-dd")}.log");
            }
            bool ficheroCienMegas = false;
            string rutaFicheroErrorLast = Path.Combine(RUTA_DIRECTORIO_ERROR, $"{ficheroPorDefecto}_{DateTime.Now.ToString("yyyy-MM-dd")}{ULTIMO}.log");

            FileInfo fichero = new FileInfo(pRutaFicheroError);
            if (fichero.Name.Equals(pRutaFicheroError))
            {
                pRutaFicheroError = Path.Combine(RUTA_DIRECTORIO_ERROR, pRutaFicheroError.TrimStart('/').TrimStart('\\'));
                fichero = new FileInfo(pRutaFicheroError);
            }

            FileInfo ficheroGiga = new FileInfo(rutaFicheroErrorLast);
            if (ficheroGiga.Name.Equals(rutaFicheroErrorLast))
            {
                pRutaFicheroError = Path.Combine(RUTA_DIRECTORIO_ERROR, rutaFicheroErrorLast.TrimStart('/').TrimStart('\\'));
                ficheroGiga = new FileInfo(rutaFicheroErrorLast);
            }
            //Comprobar si existe el fichero de 1 Gb para solo almacenar de 100 Mb
            if (File.Exists(rutaFicheroErrorLast))
            {
                if (ficheroGiga.Length > TAMAÑO_MAXIMO_LOG)
                {
                    ficheroCienMegas = true;
                }
            }

            if (!Directory.Exists(fichero.DirectoryName))
            {
                Directory.CreateDirectory(fichero.DirectoryName);
            }

            //Si el fichero supera el tamaño máximo lo dejo como last y cambio el log
            if (File.Exists(pRutaFicheroError))
            {
                if (ficheroCienMegas)
                {
                    if (fichero.Length > TAMAÑO_MAXIMO_LOG_DIARIO)
                    {
                        fichero.Delete();
                    }
                }
                else
                {
                    if (fichero.Length > TAMAÑO_MAXIMO_LOG)
                    {
                        fichero.CopyTo(rutaFicheroErrorLast);
                    }
                }

            }
            return pRutaFicheroError;
        }

        private string PrepararMensajeLog(string pMensaje, string pTipoError)
        {

            StringBuilder sb = new StringBuilder();
            sb.Append($"{pMensaje?.TrimEnd()} \"{pTipoError?.TrimEnd()}\"");

            try
            {
                if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null)
                {
                    //sw.WriteLine(HttpContext.Current.Request.Url.ToString());
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

        private void EscribirLogEnFichero(string pRutaFicheroError, string lineaLog)
        {
            using (StreamWriter sw = new StreamWriter(pRutaFicheroError, true, Encoding.Default))
            {
                sw.WriteLine(lineaLog);
            }
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

                if (pExcepcion is AggregateException)
                {
                    if (((AggregateException)pExcepcion).InnerExceptions.Count > 0)
                    {
                        datosError.Message += ". Aggregated exceptions: ";
                        int numero = 1;
                        foreach (Exception exception in ((AggregateException)pExcepcion).InnerExceptions)
                        {
                            datosError.Message += $". Exception {numero++}: {exception.Message}";
                        }
                    }
                }
            }

            return datosError;
        }

        private static string DevolverCadenaErrorExcepcion(Exception pExcepcion, string pTipoExcepcion = null)
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
                if (pExcepcion.InnerException != null)
                {
                    //resultado += $" \"{DevolverCadenaErrorExcepcion(pExcepcion.InnerException)}\"";
                }
                if (pExcepcion is AggregateException)
                {
                    //AggregateException aggregateException = (AggregateException)pExcepcion;
                    //resultado += $" \"";
                    //if (aggregateException.InnerExceptions.Count == 0)
                    //{
                    //    resultado += $"-";
                    //}
                    //foreach (Exception ex in aggregateException.InnerExceptions)
                    //{
                    //    resultado += $" {DevolverCadenaErrorExcepcion(ex)}";
                    //}
                    //resultado += $"\"";
                }
            }

            return resultado;
        }

        /// <summary>
        /// Acción de enviar los logs al servidor.
        /// <param name="pLog">Mensaje a enviar.</param>
        /// </summary>
        public void EnviarLogLogstash(Exception pExcepcion, string pMensajeExtra)
        {
            if (Log != null)
            {
                DatosError data = GenerarDatosError(pExcepcion, pMensajeExtra);
                DatosRequest request = null;
                DatosUsuario user = null;
                DatosProducto datosProducto = new DatosProducto();

                try
                {
                    datosProducto.Version = Version;
                    datosProducto.Producto = Producto;
                    datosProducto.Plataforma = PLATAFORMA;

                    if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null)
                    {
                        request = new DatosRequest();
                        // Rellenar objeto con los datos (Request)
                        HttpRequest currentRequest = _httpContextAccessor.HttpContext.Request;

                        var cookie = currentRequest.Cookies["ASP.NET_SessionId"];
                        if (cookie != null)
                        {
                            request.CookieSessionId = cookie;
                        }

                        string headers = "";
                        string[] arrayHeaders = currentRequest.Headers.Keys.ToArray();
                        foreach (string clave in arrayHeaders)
                        {
                            headers += $"{clave} : {string.Join(" ", currentRequest.Headers[clave])}\n";
                        }
                        request.Headers = headers.TrimEnd();
                        request.HttpMethod = currentRequest.Method;
                        request.RawURL = $"{currentRequest.Scheme}://{currentRequest.Host}{currentRequest.Path}";
                        request.URL = currentRequest.Path;
                        request.UserAgent = currentRequest.Headers["User-Agent"];
                        request.Domain = currentRequest.Host.Value;
                        if (currentRequest.Body != null && currentRequest.Body.Length > 0)
                        {
                            currentRequest.Body.Position = 0;
                            StreamReader streamInputStream = new StreamReader(currentRequest.Body);
                            request.Body = streamInputStream.ReadToEnd();
                        }

                        string ip = currentRequest.Headers["X-FORWARDED-FOR"];
                        if (string.IsNullOrEmpty(ip))
                        {
                            ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                            request.Ip = ip;
                        }
                    }

                    if (_usuario.UsuarioActual != null)
                    {
                        user = new DatosUsuario();
                        // Rellenar objeto con datos (User)               
                        user.UsuarioId = _usuario.UsuarioActual.UsuarioID.ToString();
                        user.IdentidadId = _usuario.UsuarioActual.IdentidadID.ToString();
                        user.ProyectoId = _usuario.UsuarioActual.ProyectoID.ToString();
                    }
                }
                catch { }

                try
                {
                    if (user == null && request == null)
                    {
                        if (pExcepcion != null)
                        {
                            Log.Error(pExcepcion, "{@datosProducto},{@data}", datosProducto, data);
                        }
                        else
                        {
                            Log.Error("{@datosProducto},{@data}", datosProducto, data);
                        }
                    }
                    else if (user != null && request == null)
                    {
                        if (pExcepcion != null)
                        {
                            Log.Error(pExcepcion, "{@datosProducto},{@data},{@user}", datosProducto, data, user);
                        }
                        else
                        {
                            Log.Error("{@datosProducto},{@data},{@user}", datosProducto, data, user);
                        }
                    }
                    else if (user == null && request != null)
                    {
                        if (pExcepcion != null)
                        {
                            Log.Error(pExcepcion, "{@datosProducto},{@data},{@request}", datosProducto, data, request);
                        }
                        else
                        {
                            Log.Error("{@datosProducto},{@data},{@request}", datosProducto, data, request);
                        }
                    }
                    else if (data != null && user != null && request != null)
                    {
                        if (pExcepcion != null)
                        {
                            Log.Error(pExcepcion, "{@datosProducto},{@data},{@user},{@request}", datosProducto, data, user, request);
                        }
                        else
                        {
                            Log.Error("{@datosProducto},{@data},{@user},{@request}", datosProducto, data, user, request);
                        }
                    }
                }
                catch { }
            }
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
        public void AgregarEntradaApplicationInsights(string pMensaje)
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

        private Traza CrearTraza(string pIP, int pPuerto)
        {
            return CrearTraza(pIP, pPuerto, _httpContextAccessor != null && _httpContextAccessor.HttpContext.Request != null);
        }

        public Traza CrearTraza(string pIP, int pPuerto, bool pEsPeticionWeb)
        {
            Traza traza;
            if (pEsPeticionWeb)
            {
                traza = new TrazaWeb(pIP, pPuerto, _httpContextAccessor, _utilTelemetry);
            }
            else
            {
                traza = new Traza(pIP, pPuerto, _utilTelemetry);
            }

            return traza;
        }

        public static void InicializarLogstash(string pEndpoint)
        {
            // QueueLimitBytes: Limite en bytes que va a guardar los log en memoria
            // Si es nulo no está limitado
            Log = new LoggerConfiguration()
                .WriteTo.Http(requestUri: pEndpoint, queueLimitBytes: null)
                .CreateLogger();
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
        /// Obtiene el Producto.
        /// </summary>
        public static string Producto
        {
            get
            {
                if (string.IsNullOrEmpty(_PRODUCTO))
                {
                    _PRODUCTO = ObtenerProducto();
                }

                return _PRODUCTO;
            }
            set
            {
                if (string.IsNullOrEmpty(_PRODUCTO))
                {
                    _PRODUCTO = value;
                }
            }
        }

        /// <summary>
        /// Obtiene la Versión.
        /// </summary>
        public static string Version
        {
            get
            {
                if (string.IsNullOrEmpty(_VERSION))
                {
                    _VERSION = ObtenerVersion();
                }

                return _VERSION;
            }
            set
            {
                if (string.IsNullOrEmpty(_VERSION))
                {
                    _VERSION = value;
                }
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

        public static DateTime HoraComprobacionCache
        {
            get
            {
                return mHoraComprobacionCache;
            }
            set
            {
                mHoraComprobacionCache = value;
            }
        }

        public static int TiempoDuracionComprobacion
        {
            get
            {
                return mTiempoDuracionComprobacion;
            }
            set
            {
                mTiempoDuracionComprobacion = value;
            }
        }

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
                    mTrazaActual = CrearTraza(IP, Puerto);
                }
                return mTrazaActual;
            }
        }

        public static string IP { get; set; }
        public static int Puerto { get; set; }
        public string PLATAFORMA
        {
            get
            {
                return _utilPeticion.ObtenerObjetoDePeticion("PLATAFORMA") as string;
            }
            set
            {
                _utilPeticion.AgregarObjetoAPeticionActual("PLATAFORMA", value);
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

        // Creación del objeto con los datos de los posibles excepciones
        public static DatosTraza data;

        /// <summary>
        /// Almacena la hora de la última traza guardada
        /// </summary>
        private DateTime mHoraUltimaTraza;

        /// <summary>
        /// Almacena la hora de la última traza guardada
        /// </summary>
        private DateTime mHoraInicio;

        private Guid mPeticionID = Guid.NewGuid();

        private int mOrdenTraza = 0;

        /// <summary>
        /// Almacena toda la traza generada hasta ahora
        /// </summary>
        private StringBuilder mTraza;

        /// <summary>
        /// Almacena toda la traza generada hasta ahora
        /// </summary>
        private List<string> mMensajesTrazaFichero;

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

        private string IP;

        private int Puerto;

        /// <summary>
        /// Tamaño máximo en bytes del fichero de log de errores
        /// </summary>
        private static long TAMAÑO_MAXIMO_LOG = 102400000;

        #endregion

        #region Constructores
        private UtilTelemetry _utilTelemetry;
        /// <summary>
        /// Constructor de la Traza
        /// </summary>
        public Traza(string pIP, int pPuerto, UtilTelemetry utilTelemetry)
        {
            _utilTelemetry = utilTelemetry;
            mMensajesTrazaFichero = new List<string>();
            mEstaTrazaHabilitada = LoggingService.TrazaHabilitada;
            if (mEstaTrazaHabilitada)
            {
                mHoraUltimaTraza = DateTime.Now;
                mHoraInicio = mHoraUltimaTraza;

                mPeticionID = Guid.NewGuid();
                mOrdenTraza = 0;

                IP = pIP;
                Puerto = pPuerto;
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

                // Mandar por UDP el mensaje de la traza.
                //if (!string.IsNullOrEmpty(IP) && Puerto != 0)
                //{
                //    // Enviamos por UDP la traza
                //    EnvioTrazaServicioExternoUDP(pEntrada, mensajeTraza);
                //}

                //if (UtilTelemetry.EstaConfiguradaTelemetria && !LoggingService.UBICACIONTRAZA.Equals(UtilTelemetry.UbicacionLogsYTrazas.Archivo))
                //{
                //    _utilTelemetry.EnviarTelemetriaTraza(pEntrada, pNombreDependencia, pReloj, pExito);
                //}

                data = new DatosTraza();
                data.Tiempo = horaActual.Subtract(mHoraUltimaTraza).TotalMilliseconds.ToString();
                data.Mensaje = LimpiarDatos(pEntrada);
                data.Version = LoggingService.Version;
                data.Producto = LoggingService.Producto;

                //Envia el error al servidor Logstash
                //EnviarLogLogstash("{@data}", data);
            }

            mHoraUltimaTraza = horaActual;
        }

        /// <summary>
        /// Acción de enviar los logs al servidor.
        /// <param name="pLog">Mensaje a enviar.</param>
        /// </summary>
        public static void EnviarLogLogstash(string pLog, DatosTraza pDatos)
        {
            if (LoggingService.Log != null)
            {
                LoggingService.Log.Information(pLog, pDatos);
            }
        }

        private void EnvioTrazaServicioExternoUDP(string pEntrada, string pMensajeTraza)
        {
            StringBuilder traza = new StringBuilder();

            string identificadorTraza = ObtenerCadenaInicioTrazaUDP(pEntrada);

            traza.Append(identificadorTraza);
            traza.Append(mMensajeInicioTraza);
            traza.Append(pMensajeTraza);
            traza.Append(mMensajeFinalTraza);

            byte[] sdata = Encoding.ASCII.GetBytes(traza.ToString());

            UdpClient udpc = new UdpClient(IP, Puerto);
            if (sdata.Length > 65500)
            {
                // Trocear el envío y mandarlo en diferentes paquetes UDP
                List<string> trozos = new List<string>();
                string trazaTemporal = traza.ToString();
                while (trazaTemporal.Length > 0)
                {
                    if (trazaTemporal.Length > 2500)
                    {
                        string fragmento = trazaTemporal.ToString().Substring(0, 2500);
                        trazaTemporal = trazaTemporal.Substring(2500);
                        trozos.Add(fragmento);
                    }
                    else
                    {
                        trozos.Add(trazaTemporal);
                        trazaTemporal = string.Empty;
                    }
                }

                // Agregamos una cadena que identifique el envío de la traza y el fragmento que es:
                // FRAGMENTOTRAZAID##ORDENFRAGMENTO##NUMFRAGMENTOSTOTALES##...TRAZA...

                Guid trazaID = Guid.NewGuid();
                for (int i = 0; i < trozos.Count; i++)
                {
                    string fragmento = trazaID + "##" + i + "##" + trozos.Count + "##" + trozos[i];
                    byte[] bytes = Encoding.ASCII.GetBytes(fragmento);
                    udpc.Send(bytes, bytes.Length);
                }

            }
            else
            {
                udpc.Send(sdata, sdata.Length);
            }

            udpc.Close();
        }

        private string ObtenerCadenaInicioTrazaUDP(string pEntrada)
        {
            // ##PETICIONID##TRAZAID##ORDENTRAZA##

            string delimitador = "##";
            if (pEntrada.StartsWith("**"))
            {
                delimitador = "**";
            }

            string inicioTraza = string.Empty;
            inicioTraza += delimitador + mPeticionID + delimitador + Guid.NewGuid() + delimitador + mOrdenTraza++ + delimitador;

            return inicioTraza;
        }

        private void ObtenerMemoriaProceso(out long privateMemorySize64, out long peakPagedMemorySize)
        {
            privateMemorySize64 = -1;
            peakPagedMemorySize = -1;

            try
            {
                Process proc = Process.GetCurrentProcess();
                privateMemorySize64 = proc.PrivateMemorySize64;
                peakPagedMemorySize = proc.PeakPagedMemorySize64;
            }
            catch (Exception) { }
        }

        public string LimpiarDatos(string pDatos)
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
                    if (File.Exists(pRutaFicheroTraza))
                    {
                        if (fichero.Length > TAMAÑO_MAXIMO_LOG)
                        {
                            LoggingService.TrazaHabilitada = false; // true
                            mEstaTrazaHabilitada = LoggingService.TrazaHabilitada;
                        }
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

        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Crea la traza y añade los datos de la petición web
        /// </summary>
        /// <param name="pRutaTraza">Ruta donde se almacena la traza</param>
        public TrazaWeb(string pIP, int pPuerto, IHttpContextAccessor pHttpContextAccessor, UtilTelemetry pUtilTelemetry)
            : base(pIP, pPuerto, pUtilTelemetry)
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
