using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Es.Riam.Gnoss.Util.General;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.RedesSociales.GoogleDrive
{
    //TODO Migrar a .net Core
    /*
    /// <summary>
    /// Clase de autenticación para GoogleDrive
    /// </summary>
    public class OAuthGoogleDrive : OAuthBase
    {
        #region Miembros

        /// <summary>
        /// 
        /// </summary>
        private DriveService mDriveService;

        /// <summary>
        /// 
        /// </summary>
        private OAuth2Authenticator<WebServerClient> mAuthenticator;

        /// <summary>
        /// 
        /// </summary>
        private IAuthorizationState mState;

        /// <summary>
        /// 
        /// </summary>
        private WebServerClient mClient;

        /// <summary>
        /// Clave del consumidor en GoogleDrive
        /// </summary>
        private string mConsumerKey = "";

        /// <summary>
        /// Clave secreta del consumidor en GoogleDrive
        /// </summary>
        private string mConsumerSecret = "";

        private IHttpContextAccessor mHttpContextAccessor;

        #endregion

        #region constructores

        public OAuthGoogleDrive(IHttpContextAccessor httpContextAccessor)
            : base()
        {
            mHttpContextAccessor = httpContextAccessor;
            CargarConfiguracionGoogleDrive(true);
        }

        public OAuthGoogleDrive(string pConsumerKey, string pConsumerSecret, IHttpContextAccessor httpContextAccessor)
            : base()
        {
            mHttpContextAccessor = httpContextAccessor;
            CargarConfiguracionGoogleDrive(false);
            ConsumerKey = pConsumerKey;
            ConsumerSecret = pConsumerSecret;
        }

        #endregion constructores

        #region propiedades

        /// <summary>
        /// Obtiene o establece la clave del consumidor
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                return mConsumerKey;
            }
            set
            {
                mConsumerKey = value;
            }
        }

        /// <summary>
        /// Obtiene o establece la clave secreta del consumidor
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                return mConsumerSecret;
            }
            set
            {
                mConsumerSecret = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el servicio Drive
        /// </summary>
        public DriveService DriveService
        {
            get { return this.mDriveService; }
            set { this.mDriveService = value; }
        }

        /// <summary>
        /// Obtiene o establece el Authenticator del cliente del servidor
        /// </summary>
        public OAuth2Authenticator<WebServerClient> Authenticator
        {
            get { return this.mAuthenticator; }
            set { this.mAuthenticator = value; }
        }

        /// <summary>
        /// Obtiene o establece el estado de la autorización
        /// </summary>
        public IAuthorizationState State
        {
            get { return this.mState; }
            set { this.mState = value; }
        }

        /// <summary>
        /// Obtiene o establece el cliente del servidor
        /// </summary>
        public WebServerClient Client
        {
            get { return this.mClient; }
            set { this.mClient = value; }
        }

        #endregion propiedades

        #region Privados

        /// <summary>
        /// Carga la configuración para la integración con GoogleDrive
        /// </summary>
        public void CargarConfiguracionGoogleDrive(bool pCargarTokens)
        {
            if (Conexion.UsarVariablesEntorno && pCargarTokens)
            {
                ConsumerKey = Environment.GetEnvironmentVariable("GoogleDriveConsumerKey");
                ConsumerSecret = Environment.GetEnvironmentVariable("GoogleDriveConsumerSecret");
            }
            else
            {
                LeerConfiguracionesXML(pCargarTokens);
            }

        }

        private void LeerConfiguracionesXML(bool pCargarTokens)
        {
            XmlDocument documentoXml = new XmlDocument();
            if (System.IO.File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config"))
            {
                documentoXml.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config");

                if (pCargarTokens)
                {
                    XmlNodeList listaNodosConsummer = documentoXml.GetElementsByTagName("GoogleDriveConsumerKey");
                    if (listaNodosConsummer != null && listaNodosConsummer.Count > 0)
                    {
                        ConsumerKey = listaNodosConsummer.Item(0).InnerText;
                    }

                    listaNodosConsummer = documentoXml.GetElementsByTagName("GoogleDriveConsumerSecret");
                    if (listaNodosConsummer != null && listaNodosConsummer.Count > 0)
                    {
                        ConsumerSecret = listaNodosConsummer.Item(0).InnerText;
                    }
                }
            }
        }

        #endregion Privados

        private static readonly string[] SCOPES = new[] { DriveService.Scopes.Drive.GetStringValue() };

        /// <summary>
        /// Establece el Authenticator del cliente
        /// </summary>
        public void CreateAuthenticator()
        {
            CreateAuthenticator(GetAuthorization);
        }

        /// <summary>
        /// Establece el Authenticator del cliente con un método de autorización
        /// </summary>
        /// <param name="pMetodoAutorizacion">Método de autorización</param>
        public void CreateAuthenticator(Func<WebServerClient, IAuthorizationState> pMetodoAutorizacion)
        {
            var provider = new WebServerClient(GoogleAuthenticationServer.Description, ConsumerKey, ConsumerSecret);
            Authenticator = new OAuth2Authenticator<WebServerClient>(provider, pMetodoAutorizacion);
            if (this.DriveService == null)
            {
                this.DriveService = new DriveService(new BaseClientService.Initializer()
                {
                    Authenticator = this.Authenticator
                });
            }
        }

        /// <summary>
        /// Obtiene el refresco de la autorización para el cliente
        /// </summary>
        /// <param name="client">Cliente web</param>
        /// <returns>Devuelve estado de autorización válido si todo es correcto, null en caso contrario</returns>
        public IAuthorizationState GetRefreshAuthorization(WebServerClient client)
        {
            IAuthorizationState state = new AuthorizationState(SCOPES);
            if (Conexion.UsarVariablesEntorno)
            {
                state.AccessToken = Environment.GetEnvironmentVariable("GoogleDriveAccessToken");
                state.AccessTokenExpirationUtc = new DateTime(long.Parse(Environment.GetEnvironmentVariable("GoogleDriveAccessTokenExpires")), DateTimeKind.Utc);
                state.RefreshToken = Environment.GetEnvironmentVariable("GoogleDriveRefreshToken");
                state.AccessTokenIssueDateUtc = DateTime.UtcNow;
                state.Callback = new Uri(Environment.GetEnvironmentVariable("GoogleDriveCallback"));
                if (state.AccessTokenExpirationUtc.Value.CompareTo(DateTime.UtcNow) <= 0)
                {
                    try
                    {
                        RefreshToken(client, state);
                        Environment.SetEnvironmentVariable("GoogleDriveAccessToken",state.AccessToken);
                        Environment.SetEnvironmentVariable("GoogleDriveAccessTokenExpires", state.AccessTokenExpirationUtc.Value.ToFileTimeUtc().ToString());
                    }
                    catch
                    {

                    }
                }
                State = state;
                return State;
            }
            else
            {
                return RefreshToken(client, state);
            }

        }

        private IAuthorizationState RefreshToken(WebServerClient client, IAuthorizationState pState)
        {
            XmlDocument documentoXml = new XmlDocument();
            if (System.IO.File.Exists(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config"))
            {
                documentoXml.Load(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config");

                //Esto para refrescar un token que ya tenemos
                pState.AccessToken = documentoXml.GetElementsByTagName("GoogleDriveAccessToken").Item(0).InnerText;
                long fecha = long.Parse(documentoXml.GetElementsByTagName("GoogleDriveAccessTokenExpires").Item(0).InnerText);
                pState.AccessTokenExpirationUtc = new DateTime(fecha);
                pState.RefreshToken = documentoXml.GetElementsByTagName("GoogleDriveRefreshToken").Item(0).InnerText;
                pState.AccessTokenIssueDateUtc = DateTime.Now;
                string callback = documentoXml.GetElementsByTagName("GoogleDriveCallback").Item(0).InnerText;

                pState.Callback = new Uri(callback);

                if (pState.AccessTokenExpirationUtc.Value.CompareTo(DateTime.Now.ToUniversalTime()) <= 0)
                {
                    //control de la concurrencia
                    try
                    {
                        RefreshToken(client, pState);
                        documentoXml.GetElementsByTagName("GoogleDriveAccessToken").Item(0).InnerText = pState.AccessToken.ToString();
                        documentoXml.GetElementsByTagName("GoogleDriveAccessTokenExpires").Item(0).InnerText = pState.AccessTokenExpirationUtc.Value.Ticks.ToString();
                        documentoXml.Save(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config/GnossRedesSociales.config");
                    }
                    catch { }
                }
                State = pState;
                return pState;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene la autorización para el cliente
        /// </summary>
        /// <param name="client">Cliente web</param>
        /// <returns>Devuelve estado de autorización válido si todo es correcto, null en caso contrario</returns>
        public IAuthorizationState GetAuthorization(WebServerClient client)
        {
            IAuthorizationState state = this.State;
            if (state != null)
            {
                if (state.AccessTokenExpirationUtc.Value.CompareTo(DateTime.Now.ToUniversalTime()) > 0)
                    return state;
                else
                    state = null;
            }
            state = client.ProcessUserAuthorization(new HttpRequestInfo(mHttpContextAccessor.HttpContext.Request));
            if (state != null && (!string.IsNullOrEmpty(state.AccessToken) || !string.IsNullOrEmpty(state.RefreshToken)))
            {
                if (state.RefreshToken == null)
                {
                    state.RefreshToken = "";
                }
                this.State = state;
                return state;
            }
            //Esto sirve para obtener los tokens
            if (mHttpContextAccessor.HttpContext != null)
            {
                string url = "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=" + ConsumerKey + "&redirect_uri=http%3A%2F%2Fautocompletar4.gnoss.net%2Fsubirdocwebapp.aspx&state=&access_type=offline&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive";
                mHttpContextAccessor.HttpContext.Response.Redirect(url);
            }
            return null;
        }

        /// <summary>
        /// Establece el estado de autorización del cliente
        /// </summary>
        /// <returns>Estado de autorización del cliente</returns>
        public IAuthorizationState ProcesarPeticionUsuario()
        {
            this.State = this.Client.ProcessUserAuthorization(new HttpRequestInfo(mHttpContextAccessor.HttpContext.Request));
            return this.State;
        }

        /// <summary>
        /// Realiza la petición de los Tokens de Acceso
        /// </summary>
        /// <param name="pUrlCallback">Url de retorno de la redirección</param>
        public void RedireccionarAGoogle(string pUrlCallback)
        {
            this.Client = new WebServerClient(GoogleAuthenticationServer.Description, ConsumerKey, ConsumerSecret);
            IAuthorizationState state = this.State;
            if (state != null)
            {
                if (state.AccessTokenExpirationUtc.Value.CompareTo(DateTime.Now.ToUniversalTime()) > 0)
                    return;
                else
                    state = null;
            }
            state = ProcesarPeticionUsuario();
            if (state != null && (!string.IsNullOrEmpty(state.AccessToken) || !string.IsNullOrEmpty(state.RefreshToken)))
            {
                if (state.RefreshToken == null)
                {
                    state.RefreshToken = "";
                }
                this.State = state;
                return;
            }
            //Esto sirve para obtener los tokens
            if (mHttpContextAccessor.HttpContext != null)
            {
                string url = "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=" + ConsumerKey + "&redirect_uri=" + HttpUtility.UrlEncode(pUrlCallback) + "&state=&access_type=offline&scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fdrive";
                mHttpContextAccessor.HttpContext.Response.Redirect(url);
            }
        }

        public bool ConvertFile(string pExtension)
        {
            return (pExtension.Equals("txt") || pExtension.Equals("doc") || pExtension.Equals("docx") || pExtension.Equals("rtf") || pExtension.Equals("xls") || pExtension.Equals("xlsx"));
        }

        /// <summary>
        /// Sube el documento a la cuenta de GoogleDrive
        /// </summary>
        /// <param name="pFileName">Nombre del documento, con extensión</param>
        /// <param name="pExt">Extensión del documento</param>
        /// <param name="pBuffer">Buffer con el contenido del documento</param>
        /// <returns>Cadena con el identificador qeu GoogleDrive le da al documento</returns>
        public string SubirDocumento(string pFileName, string pExt, byte[] pBuffer)
        {
            CreateAuthenticator(GetRefreshAuthorization);
            string strType = pExt.Substring(1, pExt.Length - 1);
            string type = GetContentType(strType);
            Google.Apis.Drive.v2.Data.File newFile = new Google.Apis.Drive.v2.Data.File { Title = pFileName, MimeType = type };
            MemoryStream stream = new MemoryStream(pBuffer);

            FilesResource.InsertMediaUpload request = this.DriveService.Files.Insert(newFile, stream, newFile.MimeType);
            request.Convert = ConvertFile(strType);
            request.Upload();
            Google.Apis.Drive.v2.Data.File file = request.ResponseBody;

            //HttpContext.Current.Response.Write(file.AlternateLink);
            return file.Id;
        }

        /// <summary>
        /// Edita el documento subido en la cuenta de GoogleDrive
        /// </summary>
        /// <param name="pFileId">Identificador del documento en GoogleDrive</param>
        /// <param name="pNewDescription">La nueva descripción del documento</param>
        /// <param name="pExt">Extensión del documento nuevo</param>
        /// <param name="pNewFilename">Nombre del documento nuevo</param>
        /// <param name="pBuffer">Contenido del documento nuevo</param>
        /// <param name="pNewRevision">Indica si se desea guardar versión anterior del documento</param>
        public void EditarDocumento(string pFileId, string pNewDescription, string pExt, string pNewFilename, byte[] pBuffer, bool pNewRevision)
        {
            CreateAuthenticator(GetRefreshAuthorization);
            string strType = pExt.Substring(1, pExt.Length - 1);
            string newMimeType = GetContentType(strType);

            // First retrieve the file from the API.
            Google.Apis.Drive.v2.Data.File file = this.DriveService.Files.Get(pFileId).Fetch();

            // File's new metadata.
            file.Title = pNewFilename + pExt;
            file.Description = pNewDescription;
            file.MimeType = newMimeType;

            // File's new content.
            //byte[] byteArray = System.IO.File.ReadAllBytes(pNewFilename);
            System.IO.MemoryStream stream = new System.IO.MemoryStream(pBuffer);

            // Send the request to the API.
            FilesResource.UpdateMediaUpload request = this.DriveService.Files.Update(file, pFileId, stream, newMimeType);
            request.NewRevision = pNewRevision;
            request.Upload();

            Google.Apis.Drive.v2.Data.File updatedFile = request.ResponseBody;
            //return updatedFile.Id;            
        }

        /// <summary>
        /// Eliminar el documento subido en la cuenta de GoogleDrive
        /// </summary>
        /// <param name="pFileID"></param>
        public void EliminarDocumento(string pFileID)
        {
            CreateAuthenticator(GetRefreshAuthorization);
            this.DriveService.Files.Delete(pFileID).Fetch();

            //FilesResource.DeleteRequest request = DriveService.Files.Delete(pFileID);
            //return request.Fetch();
        }

        /// <summary>
        /// Obtiene el MIME Type a partir de la extensión de una extensión
        /// </summary>
        /// <param name="fileExtension">Extensión del documento</param>
        /// <returns>String con el MIME Type</returns>
        private string GetContentType(string fileExtension)
        {
            if (string.IsNullOrEmpty(fileExtension))
                return string.Empty;

            string contentType = string.Empty;
            switch (fileExtension)
            {
                case "htm":
                case "html":
                    contentType = "text/HTML";
                    break;

                case "txt":
                    contentType = "text/plain";
                    break;

                case "doc":
                case "docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;

                case "rtf":
                    contentType = "application/rtf";
                    break;

                case "xls":
                case "xlsx":
                    //contentType = "Application/x-msexcel";
                    //contentType = "application/vnd.ms-excel";
                    //contentType = "application/x-vnd.oasis.opendocument.spreadsheet"; //error 500 InternalServerError
                    contentType = "application/octet-stream";
                    break;

                case "jpg":
                case "jpeg":
                    contentType = "image/jpeg";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                case "bmp":
                    contentType = "image/bmp";
                    break;
                case "gif":
                    contentType = "image/GIF";
                    break;

                case "pdf":
                    contentType = "application/pdf";
                    break;
            }

            return contentType;
        }

        /// <summary>
        /// Crea un directorio con el API de GoogleDrive
        /// </summary>
        public void CrearDirectorio()
        {
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = "document title";
            body.Description = "document description";
            body.MimeType = "application/vnd.google-apps.folder";

            // service is an authorized Drive API service instance
            DriveService.Files.Insert(body);
        }


        /// <summary>Descarga un archivo y devuelve una cadena con su contenido</summary>
        /// <param name="pGoogleID"></param>
        /// <returns></returns>
        public byte[] DescargarDocumento(string pGoogleID, string pExt)
        {
            if (!String.IsNullOrEmpty(pGoogleID))
            {
                string type = GetContentType(pExt);
                CreateAuthenticator(GetRefreshAuthorization);
                var getRequest = DriveService.Files.Get(pGoogleID);
                Google.Apis.Drive.v2.Data.File archivoGoogle = getRequest.Fetch();

                HttpWebRequest request = null;
                //es una extensión contenida en google drive
                if (archivoGoogle.ExportLinks != null && archivoGoogle.ExportLinks.Count > 0)
                {
                    if (archivoGoogle.ExportLinks.ContainsKey(type))
                    {
                        request = (HttpWebRequest)WebRequest.Create(new Uri(archivoGoogle.ExportLinks[type]));
                    }
                    else
                    {
                        request = (HttpWebRequest)WebRequest.Create(new Uri(archivoGoogle.ExportLinks.First().Value));
                    }
                }
                else
                {
                    request = (HttpWebRequest)WebRequest.Create(new Uri(archivoGoogle.DownloadUrl));
                }
                Authenticator.ApplyAuthenticationToRequest(request);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    BinaryReader br = new BinaryReader(response.GetResponseStream());
                    const int bufferSize = 4096;
                    using (var ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[bufferSize];
                        int count;
                        while ((count = br.Read(buffer, 0, buffer.Length)) != 0)
                            ms.Write(buffer, 0, count);
                        return ms.ToArray();
                    }
                }
                else
                {
                    throw new Exception("Error al leer el documento desde Google Drive");
                }
            }
            else
            {
                //El archivo no tiene contenido almacenado en Drive
                return null;
            }
        }
    }
    */
}
