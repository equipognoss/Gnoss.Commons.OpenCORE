using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Core;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Web;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{

    public class GestionDocumental
    {
        private string mUrl;
        private LoggingService mLoggingService;
        private TokenBearer mToken;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public GestionDocumental(LoggingService loggingService, ConfigService configService, ILogger<GestionDocumental> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            CallTokenService callTokenService = new CallTokenService(configService);
            mToken = callTokenService.CallTokenApi();
        }

        public string Url
        {
            get
            {
                return mUrl;
            }
            set
            {
                mUrl = value;
            }
        }

        public byte[] ObtenerDocumentoDeDirectorio(string pDirectorio, string pNombreArchivo, string pExtension)
        {
            if (pDirectorio[pDirectorio.Length - 1] != '/' || pDirectorio[pDirectorio.Length - 1] != '\\')
            {
                pDirectorio = pDirectorio.Substring(0, pDirectorio.Length - 1);
            }

            return BajarDocumento(pDirectorio, pNombreArchivo, pExtension);
        }

        private byte[] BajarDocumento(string pDirectorio, string pNombreArchivo, string pExtension)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion BajarDocumento");
            string peticion = Url + "/GetFile?Name=" + pNombreArchivo + "&Extension=" + pExtension + "&Path=" + pDirectorio;
            byte[] respuesta = WebRequestGetBytes(peticion, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion BajarDocumento");
            return respuesta;
        }

        public byte[] ObtenerDocumentoDeBaseRecursosUsuario(string pTipoEntidad, Guid pPersonaID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, Guid.Empty, Guid.Empty, pPersonaID, pExtension);

            return BajarDocumento(directorio, pDocumentoID.ToString(), pExtension);
        }

        public byte[] ObtenerDocumentoDeBaseRecursosOrganizacion(string pTipoEntidad, Guid pOrganizacionID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, Guid.Empty, Guid.Empty, pExtension);

            return BajarDocumento(directorio, pDocumentoID.ToString(), pExtension);
        }

        public byte[] ObtenerDocumento(string pTipoEntidad, Guid pOrganizacionID, Guid pProyectoID, Guid pDocumentoID, string pExtension)
        {
            mLoggingService.GuardarLog($"Parametros para obtener el directorio pTipoEntidad: {pTipoEntidad} -- pOrganizacionID: {pOrganizacionID} -- pProyectoID: {pProyectoID} -- pExtension: {pExtension}", mlogger);
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, pProyectoID, Guid.Empty, pExtension);
            mLoggingService.GuardarLog($"Al hacer la llamada a gesdoc el directorio es: {directorio}", mlogger);
            return BajarDocumento(directorio, pDocumentoID.ToString(), pExtension);
        }

        public byte[] ObtenerDocumentoWebTemporal(Guid pIdentidadID, string pNombreArchivo, string pExtension)
        {
            pNombreArchivo = FormatearNombreArchivoWebTemporal(pNombreArchivo);

            return BajarDocumento("WebTemporal", pIdentidadID.ToString() + "_" + pNombreArchivo, pExtension);
        }

        public byte[] ObtenerRecursoTemporal(Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(null, Guid.Empty, Guid.Empty, Guid.Empty, pExtension);

            return BajarDocumento(directorio, pDocumentoID.ToString(), pExtension);
        }

        public string AdjuntarDocumentoADirectorio(byte[] pFichero, string pDirectorio, string pNombreArchivo, string pExtension)
        {
            if (pDirectorio[pDirectorio.Length - 1] != '/' || pDirectorio[pDirectorio.Length - 1] != '\\')
            {
                pDirectorio = pDirectorio.Substring(0, pDirectorio.Length - 1);
            }

            return SubirDocumento(pFichero, pDirectorio, pNombreArchivo, pExtension);
        }

        private string SubirDocumento(byte[] pFichero, string pDirectorio, string pNombreArchivo, string pExtension)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion SubirDocumento");
            string peticion = Url + "/SetFile?Name=" + pNombreArchivo + "&Extension=" + HttpUtility.UrlEncode(pExtension) + "&Path=" + HttpUtility.UrlEncode(pDirectorio);
            string respuesta = WebRequest("POST", peticion, pFichero, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion SubirDocumento");

            if (respuesta == "")
            {
                return respuesta;
            }
            else
            {
                return JsonConvert.DeserializeObject<string>(respuesta);
            }
        }

        public string AdjuntarDocumento(byte[] pFichero, string pTipoEntidad, Guid pOrganizacionID, Guid pProyectoID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, pProyectoID, Guid.Empty, pExtension);

            return SubirDocumento(pFichero, directorio, pDocumentoID.ToString(), pExtension);
        }

        public string AdjuntarDocumentoABaseRecursosUsuario(byte[] pFichero, string pTipoEntidad, Guid pPersonaID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, Guid.Empty, Guid.Empty, pPersonaID, pExtension);

            return SubirDocumento(pFichero, directorio, pDocumentoID.ToString(), pExtension);
        }

        public string AdjuntarDocumentoABaseRecursosOrganizacion(byte[] pFichero, string pTipoEntidad, Guid pOrganizacionID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, Guid.Empty, Guid.Empty, pExtension);

            return SubirDocumento(pFichero, directorio, pDocumentoID.ToString(), pExtension);
        }

        public string AdjuntarDocumentoWebTemporal(byte[] pFichero, Guid pIdentidadID, string pNombreArchivo, string pExtension)
        {
            pNombreArchivo = FormatearNombreArchivoWebTemporal(pNombreArchivo);

            return AdjuntarDocumentoADirectorio(pFichero, "WebTemporal/", pIdentidadID.ToString() + "_" + pNombreArchivo, pExtension);
        }

        public double ObtenerTamañoArchivo(string pDirectorio, string pNombreArchivo, string pExtension)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerTamañoArchivo");
            string peticion = Url + "/GetSizeFile?Name=" + pNombreArchivo + "&Extension=" + pExtension + "&Path=" + pDirectorio;
            string respuesta = WebRequest("GET", peticion, null, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerTamañoArchivo");
            return JsonConvert.DeserializeObject<double>(respuesta);
        }

        public double ObtenerEspacioDocumentoDeBaseRecursosUsuario(string pTipoEntidad, Guid pPersonaID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, Guid.Empty, Guid.Empty, pPersonaID, pExtension);

            return ObtenerTamañoArchivo(directorio, pDocumentoID.ToString(), pExtension);
        }

        public double ObtenerEspacioDocumentoDeBaseRecursosOrganizacion(string pTipoEntidad, Guid pOrganizacionID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, Guid.Empty, Guid.Empty, pExtension);

            return ObtenerTamañoArchivo(directorio, pDocumentoID.ToString(), pExtension);
        }

        public bool BorrarDocumentoDeDirectorio(string pDirectorio, string pNombreArchivo, string pExtension)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion BorrarDocumentoDeDirectorio");
            string peticion = Url + "/DeleteFile?Name=" + pNombreArchivo + "&Extension=" + pExtension + "&Path=" + pDirectorio;
            string respuesta = WebRequest("POST", peticion, null, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion BorrarDocumentoDeDirectorio");
            return JsonConvert.DeserializeObject<bool>(respuesta);
        }

        public bool BorrarDocumento(string pTipoEntidad, Guid pOrganizacionID, Guid pProyectoID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, pProyectoID, Guid.Empty, pExtension);

            return BorrarDocumentoDeDirectorio(directorio, pDocumentoID.ToString(), pExtension);
        }

        public bool BorrarDocumentoDeBaseRecursosUsuario(string pTipoEntidad, Guid pPersonaID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, Guid.Empty, Guid.Empty, pPersonaID, pExtension);

            return BorrarDocumentoDeDirectorio(directorio, pDocumentoID.ToString(), pExtension);
        }

        public bool BorrarDocumentoDeBaseRecursosOrganizacion(string pTipoEntidad, Guid pOrganizacionID, Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(pTipoEntidad, pOrganizacionID, Guid.Empty, Guid.Empty, pExtension);

            return BorrarDocumentoDeDirectorio(directorio, pDocumentoID.ToString(), pExtension);
        }

        public bool BorrarRecursoTemporal(Guid pDocumentoID, string pExtension)
        {
            string directorio = ObtenerPathFile(null, Guid.Empty, Guid.Empty, Guid.Empty, pExtension);

            return BorrarDocumentoDeDirectorio(directorio, pDocumentoID.ToString(), pExtension);
        }

        public bool BorrarDocumentosDeDirectorio(string pDirectorio)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion BorrarDocumentosDeDirectorio");
            string peticion = Url + "/DeleteFilesDirectory?Path=" + pDirectorio;
            string respuesta = WebRequest("POST", peticion, null, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion BorrarDocumentosDeDirectorio");
            return JsonConvert.DeserializeObject<bool>(respuesta);
        }

        public bool BorrarArchivosDeOntologia(Guid pOntologiaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion BorrarArchivosDeOntologia");
            string peticion = Url + "/DeleteFilesOntology?Ontology=" + pOntologiaID;
            string respuesta = WebRequest("POST", peticion, null, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion BorrarArchivosDeOntologia");
            return JsonConvert.DeserializeObject<bool>(respuesta);
        }

        public bool CopiarCortarDocumento(bool pCopiar, string pDirectorioOrigen, string pDirectorioDestino, string pNombreArchivo, string pExtension, string pNombreArchivoDestino = null)
        {
            string functionName = pCopiar ? "CopyFile" : "MoveFile";

            mLoggingService.AgregarEntrada("INICIO Peticion CopiarCortarDocumento");
            string peticion = $"{Url}/{functionName}?Name={pNombreArchivo}&Extension={pExtension}&PathOrigin={pDirectorioOrigen}&PathDestination={pDirectorioDestino}&NameDestination={pNombreArchivoDestino}";
            string respuesta = WebRequest("POST", peticion, null, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion CopiarCortarDocumento");
            return JsonConvert.DeserializeObject<bool>(respuesta);
        }

        public bool CopiarCortarDocumento(bool pCopiar, string pTipoEntidadOrigen, Guid pOrganizacionIDOrigen, Guid pProyectoIDOrigen, Guid pPersonaIDOrigen, Guid pDocumentoIDOrigen, string pTipoEntidadDestino, Guid pOrganizacionIDDestino, Guid pProyectoIDDestino, Guid pPersonaIDDestino, Guid pDocumentoIDDestino, string pExtension)
        {
            string directorioOrigen = ObtenerPathFile(pTipoEntidadOrigen, pOrganizacionIDOrigen, pProyectoIDOrigen, pPersonaIDOrigen, pExtension);
            string directorioDestino = ObtenerPathFile(pTipoEntidadDestino, pOrganizacionIDDestino, pProyectoIDDestino, pPersonaIDDestino, pExtension);

            return CopiarCortarDocumento(pCopiar, directorioOrigen, directorioDestino, pDocumentoIDOrigen.ToString(), pExtension, pDocumentoIDDestino.ToString());
        }

        public bool CopiarDocumentosDeDirectorio(string pDirectorioOrigen, string pDirectorioDestino)
        {
            if (pDirectorioOrigen[pDirectorioOrigen.Length - 1] != '/' || pDirectorioOrigen[pDirectorioOrigen.Length - 1] != '\\')
            {
                pDirectorioOrigen = pDirectorioOrigen.Substring(0, pDirectorioOrigen.Length - 1);
            }
            if (pDirectorioDestino[pDirectorioDestino.Length - 1] != '/' || pDirectorioDestino[pDirectorioDestino.Length - 1] != '\\')
            {
                pDirectorioDestino = pDirectorioDestino.Substring(0, pDirectorioDestino.Length - 1);
            }
            mLoggingService.AgregarEntrada("INICIO Peticion CopiarDocumentosDeDirectorio");
            string peticion = $"{Url}/CopyDocsDirectory?PathOrigin={pDirectorioOrigen}&PathDestination={pDirectorioDestino}";
            string respuesta = WebRequest("POST", peticion, null, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion CopiarDocumentosDeDirectorio");
            return JsonConvert.DeserializeObject<bool>(respuesta);
        }

        public string[] ObtenerListadoDeDocumentosDeDirectorio(string pDirectorio)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion BajarDocumento");
            mLoggingService.AgregarEntrada("FIN Peticion BajarDocumento");
            string peticion = $"{Url}/GetFilesName?Path={pDirectorio}";
            string respuesta = WebRequest("GET", peticion, null, mToken);

            string[] listaDocumentos = JsonConvert.DeserializeObject<string[]>(respuesta);
            return listaDocumentos;
        }

        public string[] ObtenerListadoDeDirectoriosDeDirectorio(string pDirectorio)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion BajarDocumento");
            mLoggingService.AgregarEntrada("FIN Peticion BajarDocumento");
            string peticion = Url + "/GetDirectoriesName?Path=" + pDirectorio;
            string respuesta = WebRequest("GET", peticion, null, mToken);

            string[] listaDirectorios = JsonConvert.DeserializeObject<string[]>(respuesta);
            return listaDirectorios;
        }

        private static string ObtenerPathFile(string pTipoEntidad, Guid pOrganizacionID, Guid pProyectoID, Guid pPersonaID, string pExtension)
        {
            string proyecto = "";
            if (!pProyectoID.Equals(Guid.Empty))
            {
                proyecto = pProyectoID.ToString();
            }

            string organizacion = pOrganizacionID.ToString();

            string rutaFichero = null;
            if (pOrganizacionID != Guid.Empty)
            {
                rutaFichero = Path.Combine("Organizaciones", organizacion, proyecto, pTipoEntidad);
            }
            else if (pPersonaID != Guid.Empty)
            {
                rutaFichero = Path.Combine("Personas", pPersonaID.ToString(), pTipoEntidad);
            }

            if (pProyectoID.Equals(Guid.Empty) && pPersonaID.Equals(Guid.Empty) && pOrganizacionID.Equals(Guid.Empty))
            {
                rutaFichero = "Temporal";
            }

            return rutaFichero;
        }

        /// <summary>
        /// Request an url with an oauth sign
        /// </summary>
        /// <param name="httpMethod">Http method (GET, POST, PUT...)</param>
        /// <param name="url">Url to make the request</param>
        /// <param name="postData">(Optional) Post data to send in the body request</param>
        /// <param name="contentType">(Optional) Content type of the postData</param>
        /// <param name="acceptHeader">(Optional) Accept header</param>
        /// <returns>Response of the server</returns>
        private static string WebRequest(string httpMethod, string url, byte[] byteData, TokenBearer pToken = null)
        {
            string result = "";
            try
            {
                HttpResponseMessage response = null;
                HttpClient client = new HttpClient();
                if (pToken != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{pToken.token_type} {pToken.access_token}");
                }
                if (httpMethod == "POST")
                {
                    HttpContent contentData = null;
                    if (byteData != null)
                    {
                        ByteArrayContent bytes = new ByteArrayContent(byteData);
                        contentData = new MultipartFormDataContent();
                        ((MultipartFormDataContent)contentData).Add(bytes, "FileBytes", "FileBytes");
                        contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
                    }
                    response = client.PostAsync($"{url}", contentData).Result;
                    response.EnsureSuccessStatusCode();
                    result = response.Content.ReadAsStringAsync().Result;

                }
                else
                {
                    client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
                    response = client.GetAsync(url).Result;
                    response.EnsureSuccessStatusCode();
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (WebException ex)
            {
                string message;
                try
                {
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                    message = sr.ReadToEnd();
                }
                catch 
                {
                    // Si falla no rompemos la ejecución
                }

                // Error reading the error response, throw the original exception
                throw;
            }

            return result;
        }

        /// <summary>
        /// Request an url with an oauth sign
        /// </summary>
        /// <param name="httpMethod">Http method (GET, POST, PUT...)</param>
        /// <param name="url">Url to make the request</param>
        /// <param name="postData">(Optional) Post data to send in the body request</param>
        /// <param name="contentType">(Optional) Content type of the postData</param>
        /// <param name="acceptHeader">(Optional) Accept header</param>
        /// <returns>Response of the server</returns>
        private static byte[] WebRequestGetBytes(string url, TokenBearer pToken = null)
        {
            byte[] result = null;
            try
            {
                HttpResponseMessage response = null;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
                if (pToken != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{pToken.token_type} {pToken.access_token}");
                }
                response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsByteArrayAsync().Result;
            }
            catch (WebException ex)
            {
                string message;
                try
                {
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                    message = sr.ReadToEnd();
                }
                catch 
                {
                    // Si falla no rompemos la ejecución
                }

                // Error reading the error response, throw the original exception
                throw;
            }

            return result;
        }

        /// <summary>
        /// Se encarga de modificar el nombre para que cumpla con los requisitos del sistema y no falle al guardarse en el servidor
        /// </summary>
        /// <param name="pNombreArchivo">Nombre del archivo a guardar</param>
        /// <returns>Devuelve el nombre formateado</returns>
        private static string FormatearNombreArchivoWebTemporal(string pNombreArchivo)
        {
            if (pNombreArchivo.Length > 100)
            {
                pNombreArchivo = pNombreArchivo.Substring(0, 100);
            }

            return pNombreArchivo;
        }
    }
}
