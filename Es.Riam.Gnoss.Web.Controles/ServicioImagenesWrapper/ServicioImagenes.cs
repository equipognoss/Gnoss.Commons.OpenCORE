using Es.Riam.Gnoss.FileManager;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Net.Http;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper
{

    public class ServicioImagenes
    {
        private static readonly HttpClient mClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        });

        private LoggingService mLoggingService;
        private CallTokenService mCallTokenService;
        private TokenBearer mToken;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ServicioImagenes(LoggingService loggingService, ConfigService configService, ILogger<ServicioImagenes> logger, ILoggerFactory loggerFactory)
        {
            mLoggingService=loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mCallTokenService = new CallTokenService(configService);
            mToken = mCallTokenService.CallTokenApi();
        }

        public string Url { get; set; }

        public bool AgregarImagen(byte[] pFichero, string pNombre, string pExtension)
        {
            GnossImage imagenGnoss = new GnossImage();
            imagenGnoss.name = pNombre;
            imagenGnoss.extension = pExtension;
            imagenGnoss.file = pFichero;

            try
            {
                PeticionWebRequest("POST", "add-image", imagenGnoss);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool AgregarFichero(byte[] pFichero, string pNombre, string pExtension, string pRuta, bool pErrorSiYaExiste = false)
        {
            GnossFile ficheroEnviar = new GnossFile();
            ficheroEnviar.path = pRuta;
            ficheroEnviar.name = pNombre;
            ficheroEnviar.extension = pExtension;
            ficheroEnviar.file = pFichero;
            ficheroEnviar.error_if_exists = pErrorSiYaExiste;

            try
            {
                PeticionWebRequest("POST", "add-document-to-directory", ficheroEnviar, "DocumentosLink");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public string ObtenerNombreDisponible(string pRuta)
        {
            return PeticionWebRequest("GET", $"get-available-name?relative_path={HttpUtility.UrlEncode(pRuta)}");
        }

        public bool AgregarImagenDocumentoPersonal(byte[] pFichero, string pNombre, string pExtension, Guid pPersonaID)
        {
            GnossPersonImage imagenGnoss = new GnossPersonImage();
            imagenGnoss.name = pNombre;
            imagenGnoss.extension = pExtension;
            imagenGnoss.file = pFichero;
            imagenGnoss.person_id = pPersonaID;

            try
            {
                PeticionWebRequest("POST", "add-image-to-personal-document", imagenGnoss);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool AgregarImagenEnMiniaturaADocumento(byte[] pFichero, string pNombre, string pExtension)
        {
            GnossPersonImage imagenGnoss = new GnossPersonImage();
            imagenGnoss.name = pNombre;
            imagenGnoss.extension = pExtension;
            imagenGnoss.file = pFichero;

            try
            {
                PeticionWebRequest("POST", "AgregarImagenEnMiniaturaADocumento", imagenGnoss);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool AgregarImagenDocumentoOrganizacion(byte[] pFichero, string pNombre, string pExtension, Guid pOrganizacionID)
        {
            OrganizationPersonImage imagenGnoss = new OrganizationPersonImage();
            imagenGnoss.name = pNombre;
            imagenGnoss.extension = pExtension;
            imagenGnoss.file = pFichero;
            imagenGnoss.organization_id = pOrganizacionID;

            try
            {
                PeticionWebRequest("POST", "add-image-to-organization-document", imagenGnoss);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool AgregarImagenADirectorio(byte[] pFichero, string pDirectorio, string pNombre, string pExtension)
        {
            GnossImage imagenGnoss = new GnossImage();
            imagenGnoss.name = pNombre;
            imagenGnoss.extension = pExtension;
            imagenGnoss.file = pFichero;
            imagenGnoss.relative_path = pDirectorio;

            try
            {
                PeticionWebRequest("POST", "add-image-to-directory", imagenGnoss);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }
        public bool AgregarImagenADirectorioOntologia(byte[] pFichero, string pDirectorio, string pNombre, string pExtension)
        {
            GnossImage imagenGnoss = new GnossImage();
            imagenGnoss.name = pNombre;
            imagenGnoss.extension = pExtension;
            imagenGnoss.file = pFichero;
            imagenGnoss.relative_path = pDirectorio;

            try
            {
                PeticionWebRequest("POST", "add-image-to-ontology-directory", imagenGnoss);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public List<FileInfoModel> ObtenerDatosFicherosDeCarpeta(string pDirectorio)
        {
            string respuesta = PeticionWebRequest("GET", $"get-files-data-from-directory?relative_path={HttpUtility.UrlEncode(pDirectorio)}");
            if (string.IsNullOrWhiteSpace(respuesta))
            {
                return new List<FileInfoModel>();
            }
            return JsonSerializer.Deserialize<List<FileInfoModel>>(respuesta);
        }

        public bool MoverImagenesRecursoAlmacenamientoTemporal(string pDirectorio, Guid pDocumentoID, string pNombre)
        {
            try
            {
                PeticionWebRequest("POST", $"move-images-deleted-resource?relative_path={HttpUtility.UrlEncode(pDirectorio)}&pDocumentoID={pDocumentoID}&pNombre={pNombre}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool MoverImagenesDoclinksYVideosDeRecursoModificado(Guid pDocumentoID)
        {
            try
            {
                PeticionWebRequest("POST", $"move-images-docs-videos-modified-resource?pDocumentoID={pDocumentoID}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool MoverImagenRecursoAlmacenamientoTemporal(string pRuta, Guid pDocumentoID)
        {
            try
            {
                PeticionWebRequest("POST", $"move-image-to-temp?pRuta={HttpUtility.UrlEncode(pRuta)}&pDocumentoID={pDocumentoID}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool MoverImagenRecursoModificado(string pImagen, Guid pDocumentoID)
        {
            try
            {
                PeticionWebRequest("POST", $"move-image-modified-resource?pImagen={HttpUtility.UrlEncode(pImagen)}&pDocumentoID={pDocumentoID}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool BorrarFichero(byte[] pFichero, string pNombre, string pExtension, string pRuta)
        {
            GnossFile ficheroEnviar = new GnossFile();
            ficheroEnviar.path = pRuta;
            ficheroEnviar.name = pNombre;
            ficheroEnviar.extension = pExtension;
            ficheroEnviar.file = pFichero;
            try
            {
                PeticionWebRequest("DELETE", $"remove-document-to-directory", ficheroEnviar, "DocumentosLink");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool BorrarImagen(string pNombre)
        {
            try
            {
                PeticionWebRequest("POST", $"remove-image?name={HttpUtility.UrlEncode(pNombre)}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool BorrarImagenDocumentoPersonal(string pNombre, Guid pPersonaID)
        {
            try
            {
                PeticionWebRequest("POST", $"remove-image-from-personal-document?name={HttpUtility.UrlEncode(pNombre)}&person_id={pPersonaID}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool BorrarImagenDeDirectorio(string pRutaImagen)
        {
            try
            {
                PeticionWebRequest("POST", $"remove-image-from-directory?relative_image_path={HttpUtility.UrlEncode(pRutaImagen)}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool BorrarImagenesDeRecurso(string pDirectorio)
        {
            try
            {
                PeticionWebRequest("POST", $"remove-images-from-resource?relative_path={HttpUtility.UrlEncode(pDirectorio)}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool BorrarImagenesCategoria(Guid pCategoriaID, Guid pProyectoID)
        {
            try
            {
                PeticionWebRequest("POST", $"remove-category-images?pCategoriaID={pCategoriaID}&pProyectoID={pProyectoID}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public string ObtenerExtensionImagen(string pRutaImagen, string pNombreImagenSinExtension)
        {
            try
            {
                string extension = PeticionWebRequest("POST", $"get-image-extension?pRutaImagen={HttpUtility.UrlEncode(pRutaImagen)}&pNombreImagen={pNombreImagenSinExtension}");
                return extension;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return "";
            }
        }

        public bool CopiarCortarImagen(bool pCopiar, Guid pPersonaIDOrigen, Guid pOrganizacionIDOrigen, Guid pDocumentoIDOrigen, Guid pPersonaIDDestino, Guid pOrganizacionIDDestino, Guid pDocumentoIDDestino, string pExtension)
        {
            return CopiarCortarImagenEspecificandoRuta(pCopiar, pPersonaIDOrigen, pOrganizacionIDOrigen, pDocumentoIDOrigen, pPersonaIDDestino, pOrganizacionIDDestino, pDocumentoIDDestino, pExtension, "");
        }

        public bool CopiarCortarImagenEspecificandoRuta(bool pCopiar, Guid pPersonaIDOrigen, Guid pOrganizacionIDOrigen, Guid pDocumentoIDOrigen, Guid pPersonaIDDestino, Guid pOrganizacionIDDestino, Guid pDocumentoIDDestino, string pExtension, string pRuta)
        {
            CopyPasteImageModel copiarModel = new CopyPasteImageModel();
            copiarModel.copy = pCopiar;
            copiarModel.person_id_destination = pPersonaIDDestino;
            copiarModel.person_id_origin = pPersonaIDOrigen;
            copiarModel.organization_id_destination = pOrganizacionIDDestino;
            copiarModel.organization_id_origin = pOrganizacionIDOrigen;
            copiarModel.document_id_origin = pDocumentoIDOrigen;
            copiarModel.extension = pExtension;
            copiarModel.relative_path = pRuta;
            copiarModel.document_id_destination = pDocumentoIDDestino;

            try
            {
                PeticionWebRequest("POST", "move-or-copy-image", copiarModel);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool CopiarImagenesSemanticas(Guid pDocumentoIDOrigen, Guid pDocumentoIDDestino)
        {
            try
            {
                PeticionWebRequest("POST", $"copy-semantic-images?origin_document_id={pDocumentoIDOrigen}&destination_document_id={pDocumentoIDDestino}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        /// <summary>
        /// Llama al serviicio interno para copiar la imagen indicada de un documento a otro. Si la imagen no tiene
        /// extensi�n se copia la imagen y todos sus recortes.
        /// </summary>
        /// <param name="pDocumentoIDOrigen">Identificador del documento original que contiene la imagen</param>
        /// <param name="pDocumentoIDDestino">Identificador del documento al que se quiere copiar la imagen</param>
        /// <param name="pNombreImagen">Nombre de la imagen a copiar. Si no se define la extensi�n se copiar� la imagen y sus recortes</param>
        /// <returns></returns>
        public bool CopiarImagenSemantica(Guid pDocumentoIDOrigen, Guid pDocumentoIDDestino, string pNombreImagen)
        {
            try
            {
                PeticionWebRequest("POST", $"copy-semantic-image?origin_document_id={pDocumentoIDOrigen}&destination_document_id={pDocumentoIDDestino}&name_image={pNombreImagen}");
                return true;
            }
            catch(Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public byte[] ObtenerImagenDeDirectorioOntologia(string pDirectorio, string pNombre, string pExtension)
        {
            string respuesta = PeticionWebRequest("GET", $"get-image-from-ontology-directory?relative_path={HttpUtility.UrlEncode(pDirectorio)}&name={HttpUtility.UrlEncode(pNombre)}&extension={pExtension}");
            return FromBase64(respuesta);
        }
        public byte[] ObtenerImagenDocumento(string pNombre, string pExtension, Guid pPersonaID, Guid pOrganizacionID)
        {
            string respuesta = PeticionWebRequest("GET", $"get-image-document?pNombre={pNombre}&pExtension={pExtension}&extension={pExtension}&pPersonaID={pPersonaID}&pOrganizacionID={pOrganizacionID}");
            return FromBase64(respuesta);
        }

        public byte[] ObtenerImagen(string pNombre, string pExtension)
        {
            string respuesta = PeticionWebRequest("GET", $"get-image?name={HttpUtility.UrlEncode(pNombre)}&extension={pExtension}");
            return FromBase64(respuesta);
        }

        public string[] ObtenerIDsImagenesPorNombreImagen(string pDirectorio, string pNombreImagen)
        {
            string respuesta = PeticionWebRequest("GET", $"get-image-ids-from-image-name?relative_path={HttpUtility.UrlEncode(pDirectorio)}&image_name={HttpUtility.UrlEncode(pNombreImagen)}");
            return JsonSerializer.Deserialize<string[]>(respuesta);
        }

        public double ObtenerEspacioImagenDocumentoPersonal(string pNombre, string pExtension, Guid pPersonaID)
        {
            string respuesta = PeticionWebRequest("GET", $"get-space-for-personal-document-image?name={HttpUtility.UrlEncode(pNombre)}&extension={pExtension}&person_id={pPersonaID}");
            return JsonSerializer.Deserialize<double>(respuesta);
        }

        public double ObtenerEspacioImagenDocumentoOrganizacion(string pNombre, string pExtension, Guid pOrganizacionID)
        {
            string respuesta = PeticionWebRequest("GET", $"get-space-for-organization-document-image?name={HttpUtility.UrlEncode(pNombre)}&extension={pExtension}&organization_id={pOrganizacionID}");
            return JsonSerializer.Deserialize<double>(respuesta);
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
        public byte[] WebRequestToken(string httpMethod, string url, byte[] byteData = null)
        {
            MultipartFormDataContent contentData = contentData = new MultipartFormDataContent();
            contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
            byte[] result = null;
            HttpResponseMessage response = null;
            try
            {
                if (httpMethod == "POST")
                {
                    if (byteData == null) { byteData = new byte[0]; }

                    ByteArrayContent bytes = new ByteArrayContent(byteData);
                    contentData.Add(bytes, "file", "file");
                }
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = contentData };
                if (mToken != null)
                {
                    request.Headers.Add("Authorization", $"{mToken.token_type} {mToken.access_token}");
                }
                response = mClient.SendAsync(request).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsByteArrayAsync().Result;
                return result;
            }
            catch (HttpRequestException)
            {
                if (response != null && !string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (response != null)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
        }

        private string PeticionWebRequest(string pMethod, string pAccion, object pObjeto = null, string controlador = "image-service")
        {
            if (!Url.EndsWith("/"))
            {
                Url += "/";
            }
            string urlPeticion = $"{Url}{controlador}/{pAccion}";
            if (pMethod.Equals("GET"))
            {
                HttpResponseMessage httpResponseMessage = UtilWeb.HacerPeticionDevolviendoHttpResponseMessage(pMethod, urlPeticion, new Dictionary<string, string>(), mToken.access_token);
                Stream stream = httpResponseMessage.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                try
                {
                    mLoggingService.AgregarEntrada($"INICIO peticion servicio interno a la accion {pAccion}");
                    StreamReader sr = new StreamReader(stream);
                    string respuesta = sr.ReadToEnd();
                    sr.Close();
                    mLoggingService.AgregarEntrada($"FIN peticion servicio interno a la accion {pAccion}");
                    return respuesta;
                }
                catch (WebException ex)
                {
                    if (ex.Response != null)
                    {
                        //Leer respuesta
                        StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                        string respuesta = sr.ReadToEnd();
                        sr.Close();

                        string cabeceras = "";
                        try
                        {
                            foreach (string key in ex.Response.Headers.Keys)
                            {
                                cabeceras += $"{Environment.NewLine}{key}: {ex.Response.Headers[key]}";
                            }
                        }
                        catch { }

                        throw new Exception($"Error al enviar la peticion a {urlPeticion}:{System.Environment.NewLine}{cabeceras} {System.Environment.NewLine}{respuesta}", ex);
                    }
                    throw;
                }
            }
            else
            {
                mLoggingService.AgregarEntrada($"INICIO peticion servicio interno a la accion {pAccion}");
                string respuesta = UtilWeb.WebRequestPostWithJsonObject(urlPeticion, pObjeto, mToken.access_token);
                mLoggingService.AgregarEntrada($"FIN peticion servicio interno a la accion {pAccion}");
                return respuesta;
            }

        }

        private byte[] FromBase64(string pBase64String)
        {
            if (!string.IsNullOrEmpty(pBase64String))
            {
                return Convert.FromBase64String(pBase64String);
            }
            return null;
        }

    }
}
