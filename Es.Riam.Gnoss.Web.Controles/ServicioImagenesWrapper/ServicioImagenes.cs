using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.FileManager;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;

namespace Es.Riam.Gnoss.Web.Controles.ServicioImagenesWrapper
{

    public class ServicioImagenes
    {
        private LoggingService mLoggingService;
        private CallTokenService mCallTokenService;
        private TokenBearer mToken;

        public ServicioImagenes(LoggingService loggingService, ConfigService configService)
        {
            mLoggingService=loggingService;
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
                mLoggingService.GuardarLogError(ex);
                return false;
            }
        }

        public bool AgregarFichero(byte[] pFichero, string pNombre, string pExtension, string pRuta)
        {
            GnossFile ficheroEnviar = new GnossFile();
            ficheroEnviar.path = pRuta;
            ficheroEnviar.name = pNombre;
            ficheroEnviar.extension = pExtension;
            ficheroEnviar.file = pFichero;

            try
            {
                PeticionWebRequest("POST", "add-document-to-directory", ficheroEnviar, "DocumentosLink");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
                return false;
            }
        }

        public List<FileInfoModel> ObtenerDatosFicherosDeCarpeta(string pDirectorio)
        {
            string respuesta = PeticionWebRequest("GET", $"get-files-data-from-directory?relative_path={HttpUtility.UrlEncode(pDirectorio)}");
            return JsonConvert.DeserializeObject<List<FileInfoModel>>(respuesta);
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
                return false;
            }
        }

        public bool BorrarImagenesDeRecurso(string pDirectorio)
        {
            try
            {
                PeticionWebRequest("POST", $"remove-image-from-resource?relative_path={HttpUtility.UrlEncode(pDirectorio)}");
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                return false;
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
                mLoggingService.GuardarLogError(ex);
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
                mLoggingService.GuardarLogError(ex);
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
            return JsonConvert.DeserializeObject<string[]>(respuesta);
        }

        public double ObtenerEspacioImagenDocumentoPersonal(string pNombre, string pExtension, Guid pPersonaID)
        {
            string respuesta = PeticionWebRequest("GET", $"get-space-for-personal-document-image?name={HttpUtility.UrlEncode(pNombre)}&extension={pExtension}&person_id={pPersonaID}");
            return JsonConvert.DeserializeObject<double>(respuesta);
        }

        public double ObtenerEspacioImagenDocumentoOrganizacion(string pNombre, string pExtension, Guid pOrganizacionID)
        {
            string respuesta = PeticionWebRequest("GET", $"get-space-for-organization-document-image?name={HttpUtility.UrlEncode(pNombre)}&extension={pExtension}&organization_id={pOrganizacionID}");
            return JsonConvert.DeserializeObject<double>(respuesta);
        }


        private string PeticionWebRequest(string pMethod, string pAccion, object pObjeto = null, string controlador = "image-service")
        {
            if (!Url.EndsWith("/"))
            {
                Url += "/";
            }
            string urlPeticion = $"{Url}{controlador}/{pAccion}";
            HttpWebRequest webRequest = WebRequest.Create(urlPeticion) as HttpWebRequest;
            webRequest.Method = pMethod;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 3600000;

            if (mToken != null)
            {
                webRequest.Headers.Add("Authorization", $"{mToken.token_type} {mToken.access_token}");
            }
            if (pObjeto != null)
            {
                webRequest.ContentType = "application/json";
                string json = JsonConvert.SerializeObject(pObjeto);

                StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(json);
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            else if (!pMethod.Equals("GET"))
            {
                webRequest.ContentLength = 0;
            }

            try
            {

                WebResponse response = webRequest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string respuesta = sr.ReadToEnd();
                sr.Close();

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
