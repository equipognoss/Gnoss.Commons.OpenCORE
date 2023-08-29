using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Es.Riam.Util
{
    /// <summary>
    /// Clase con las utilidades para peticiones web
    /// </summary>
    public class UtilWeb
    {
        #region Enumeraciones

        /// <summary>
        /// Método HTTP a través del cual se envía la solicitud
        /// </summary>
        public enum Metodo
        {
            /// <summary>
            /// GET
            /// </summary>
            GET,
            /// <summary>
            /// POST
            /// </summary>
            POST,
            /// <summary>
            /// PUT
            /// </summary>
            PUT,
            /// <summary>
            /// DELETE
            /// </summary>
            DELETE
        }

        #endregion
        private IHttpContextAccessor _httpContextAccessor;
        public UtilWeb(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        #region Miembros estáticos

        /// <summary>
        /// Caracteres sustitutos del ampersand
        /// </summary>
        public static string SUSTITUTO_ANPERSAN = "_and_";

        #endregion

        #region Métodos

        /// <summary>
        /// Envía una petición web
        /// </summary>
        /// <param name="pMetodo">Método Http (GET, POST, PUT, etc)</param>
        /// <param name="pUrl">Url completa del recurso web</param>
        /// <param name="pPostData">Datos para enviar en la petición (en formato querystring)</param>
        /// <returns>Respuesta del servidor</returns>
        public string WebRequest(Metodo pMetodo, string pUrl, string pPostData, IHttpContextAccessor pRequest = null)
        {
            return WebRequest(pMetodo, pUrl, pPostData, "application/x-www-form-urlencoded", pRequest);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        /// <param name="pMetodo">Método Http (GET, POST, PUT, etc)</param>
        /// <param name="pUrl">Url completa del recurso web</param>
        /// <param name="pPostData">Datos para enviar en la petición (en formato querystring)</param>
        /// <returns>Respuesta del servidor</returns>
        public static string WebRequest(Metodo pMetodo, string pUrl, string pPostData, HttpRequest pRequest = null)
        {
            return WebRequest(pMetodo, pUrl, pPostData, "application/x-www-form-urlencoded", true, null, pRequest);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        /// <param name="pMetodo">Método Http (GET, POST, PUT, etc)</param>
        /// <param name="pUrl">Url completa del recurso web</param>
        /// <param name="pPostData">Datos para enviar en la petición (en formato querystring)</param>
        /// <returns>Respuesta del servidor</returns>
        public string WebRequest(Metodo pMetodo, string pUrl, string pPostData, string pContentType, IHttpContextAccessor pRequest = null)
        {
            return WebRequest(pMetodo, pUrl, pPostData, pContentType, true, pRequest);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        /// <param name="pMetodo">Método Http (GET, POST, PUT, etc)</param>
        /// <param name="pUrl">Url completa del recurso web</param>
        /// <param name="pPostData">Datos para enviar en la petición (en formato querystring)</param>
        /// <returns>Respuesta del servidor</returns>
        public string WebRequest(Metodo pMetodo, string pUrl, string pPostData, string pContentType, bool pSeguirRedireccion, IHttpContextAccessor pRequest = null)
        {
            return WebRequest(pMetodo, pUrl, pPostData, pContentType, pSeguirRedireccion, null, pRequest);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        /// <param name="pMetodo">Método Http (GET, POST, PUT, etc)</param>
        /// <param name="pUrl">Url completa del recurso web</param>
        /// <param name="pPostData">Datos para enviar en la petición (en formato querystring)</param>
        /// <returns>Respuesta del servidor</returns>
        public string WebRequest(Metodo pMetodo, string pUrl, string pPostData, string pContentType, bool pSeguirRedireccion, Dictionary<string, string> pCabeceras, IHttpContextAccessor pRequest = null)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            Uri UriActual = null;

            webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
            webRequest.Method = pMetodo.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            if (pRequest != null)
            {
                webRequest.UserAgent = pRequest.HttpContext.Request.Headers["UserAgent"];
                UriActual = new Uri(UriHelper.GetEncodedUrl(pRequest.HttpContext.Request));
                if (pRequest.HttpContext.Request.Headers != null)
                {
                    string accept = pRequest.HttpContext.Request.Headers["Accept"];
                    webRequest.Accept = accept;
                }
            }
            else if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request != null)
            {
                webRequest.UserAgent = _httpContextAccessor.HttpContext.Request.Headers["UserAgent"];
                UriActual = new Uri(UriHelper.GetEncodedUrl(_httpContextAccessor.HttpContext.Request));
            }

            if (_httpContextAccessor.HttpContext != null)
            {
                if (_httpContextAccessor.HttpContext.Request != null && _httpContextAccessor.HttpContext.Request.Headers != null)
                {
                    string accept = _httpContextAccessor.HttpContext.Request.Headers["Accept"];
                    
                    if (accept.Contains("application/json"))
                    {
                        webRequest.UserAgent += " GnossInternalRequest";
                    }
                }
            }
            else
            {
                webRequest.UserAgent += " GnossInternalRequest";
            }

            if (UriActual != null)
            {
                string urlReferer = UriActual.ToString();

                //if (urlReferer.Contains("?"))
                //{
                //    urlReferer = urlReferer.Remove(urlReferer.IndexOf("?"));
                //}
                webRequest.Referer = urlReferer;
            }
            //webRequest.Timeout = 20000;

            if (pCabeceras != null && pCabeceras.Count > 0)
            {
                foreach (string cabecera in pCabeceras.Keys)
                {
                    if (cabecera.ToLower().Equals("accept"))
                    {
                        webRequest.Accept = pCabeceras[cabecera];
                    }
                    else
                    {
                        webRequest.Headers.Add(cabecera, pCabeceras[cabecera]);
                    }
                }
            }

            if (!pSeguirRedireccion)
            {
                webRequest.AllowAutoRedirect = false;
            }

            if (pMetodo == Metodo.POST || pMetodo == Metodo.PUT)
            {
                webRequest.ContentType = pContentType;

                //Enviamos los datos
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(pPostData.Replace("+", "%2b"));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }

        public static string WebRequest(Metodo pMetodo, string pUrl, string pPostData, string pContentType, bool pSeguirRedireccion, Dictionary<string, string> pCabeceras, HttpRequest pRequest = null)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            Uri UriActual = null;

            webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
            webRequest.Method = pMetodo.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            if (pRequest != null)
            {
                webRequest.UserAgent = pRequest.HttpContext.Request.Headers["UserAgent"];
                UriActual = new Uri(UriHelper.GetEncodedUrl(pRequest.HttpContext.Request));
                if (pRequest.HttpContext.Request.Headers != null)
                {
                    string accept = pRequest.HttpContext.Request.Headers["Accept"];
                    webRequest.Accept = accept;
                }
            }
            

            if (pRequest != null && pRequest.HttpContext != null)
            {
                if (pRequest.HttpContext.Request != null && pRequest.HttpContext.Request.Headers != null)
                {
                    string accept = pRequest.HttpContext.Request.Headers["Accept"];

                    if (accept.Contains("application/json"))
                    {
                        webRequest.UserAgent += " GnossInternalRequest";
                    }
                }
            }
            else
            {
                webRequest.UserAgent += " GnossInternalRequest";
            }

            if (UriActual != null)
            {
                string urlReferer = UriActual.ToString();

                //if (urlReferer.Contains("?"))
                //{
                //    urlReferer = urlReferer.Remove(urlReferer.IndexOf("?"));
                //}
                webRequest.Referer = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(urlReferer));
            }
            //webRequest.Timeout = 20000;

            if (pCabeceras != null && pCabeceras.Count > 0)
            {
                foreach (string cabecera in pCabeceras.Keys)
                {
                    if (cabecera.ToLower().Equals("accept"))
                    {
                        webRequest.Accept = pCabeceras[cabecera];
                    }
                    else
                    {
                        webRequest.Headers.Add(cabecera, pCabeceras[cabecera]);
                    }
                }
            }

            if (pPostData.Contains("pIdentidadID"))
            {
                string identidadID = ObtenerIdentidadID(pPostData);
                webRequest.Headers.Add("Authorization", $"bearer {identidadID}");
            }

            if (!pSeguirRedireccion)
            {
                webRequest.AllowAutoRedirect = false;
            }

            if (pMetodo == Metodo.POST || pMetodo == Metodo.PUT)
            {
                webRequest.ContentType = pContentType;

                //Enviamos los datos
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(pPostData.Replace("+", "%2b"));
                }
                catch
                {
                    throw;
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            responseData = WebResponseGet(webRequest);

            webRequest = null;

            return responseData;
        }

        private static string ObtenerIdentidadID(string pPostData)
        {
            string identidadID = pPostData.Split("pIdentidadID=")[1].Split("&")[0];

            return identidadID;
        }


        /// <summary>
        /// Procesa la respuesta del servidor a una petición
        /// </summary>
        /// <param name="pWebRequest">Petición Http</param>
        /// <returns>Datos de la respuesta del servidor</returns>
        public static string WebResponseGet(HttpWebRequest pWebRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(pWebRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }
            }
            return responseData;
        }

        /// <summary>
        /// Hace una petición POST a una URL con una serie de parámetros
        /// </summary>
        /// <param name="pUrl">URL para la petición</param>
        /// <param name="pParametros">Clave y valor de cada parámetro de la petición. El valor NO debe estar codificado, se hará en el método.</param>
        /// <returns></returns>
        public static string HacerPeticionPost(string pUrl, Dictionary<string, string> pParametros, Dictionary<string, string> pCabeceras = null)
        {
            WebRequest wr = (HttpWebRequest)System.Net.WebRequest.Create(pUrl);
            wr.Method = "POST";
            wr.ContentType = "application/x-www-form-urlencoded";
            wr.Timeout = 300000;

            if (pCabeceras != null)
            {
                foreach (string cabecera in pCabeceras.Keys)
                {
                    wr.Headers.Add(cabecera, pCabeceras[cabecera]);
                }
            }

            //Codificación del mensaje
            string requestParameters = "";

            if (pParametros != null && pParametros.Count > 0)
            {
                foreach (string key in pParametros.Keys)
                {
                    requestParameters = string.Concat(requestParameters, key, "=", HttpUtility.UrlEncode(pParametros[key]), "&");
                }

                requestParameters = requestParameters.Substring(0, requestParameters.Length - 1);
            }

            byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
            wr.ContentLength = byteData.Length;
            Stream newStream = wr.GetRequestStream();
           
            //Envio de parametros                    
            newStream.Write(byteData, 0, byteData.Length);

            // Obtiene la respuesta
            WebResponse response = wr.GetResponse();

            // Stream con el contenido recibido del servidor
            newStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(newStream);

            // Leemos el contenido
            string responseFromServer = reader.ReadToEnd();

            // Cerramos los streams
            reader.Close();
            newStream.Close();

            return responseFromServer;
        }

        /// <summary>
        /// Hace una petición POST a una URL con una serie de parámetros
        /// </summary>
        /// <param name="pUrl">URL para la petición</param>
        /// <param name="pParametros">Clave y valor de cada parámetro de la petición. El valor NO debe estar codificado, se hará en el método.</param>
        /// <returns></returns>
        public static WebResponse HacerPeticionPostDevolviendoWebResponse(string pUrl, Dictionary<string, string> pParametros)
        {
            return HacerPeticionDevolviendoWebResponse("POST", pUrl, pParametros);
        }

        /// <summary>
        /// Hace una petición GET a una URL con una serie de parámetros
        /// </summary>
        /// <param name="pUrl">URL para la petición</param>
        /// <param name="pParametros">Clave y valor de cada parámetro de la petición. El valor NO debe estar codificado, se hará en el método.</param>
        /// <returns></returns>
        public static WebResponse HacerPeticionGetDevolviendoWebResponse(string pUrl)
        {
            return HacerPeticionDevolviendoWebResponse("GET", pUrl, null);
        }

        /// <summary>
        /// Hace una petición a una URL con una serie de parámetros
        /// </summary>
        /// <param name="pUrl">URL para la petición</param>
        /// <param name="pParametros">Clave y valor de cada parámetro de la petición. El valor NO debe estar codificado, se hará en el método.</param>
        /// <returns></returns>
        private static WebResponse HacerPeticionDevolviendoWebResponse(string pMethod, string pUrl, Dictionary<string, string> pParametros)
        {
            WebRequest wr = (HttpWebRequest)System.Net.WebRequest.Create(pUrl);
            wr.Timeout = 1200000;//20 minutos
            wr.Method = pMethod;
            wr.ContentType = "application/x-www-form-urlencoded";

            //Codificación del mensaje
            string requestParameters = "";

            if (pParametros != null && pParametros.Count > 0)
            {
                foreach (string key in pParametros.Keys)
                {
                    requestParameters = string.Concat(requestParameters, key, "=", HttpUtility.UrlEncode(pParametros[key]), "&");
                }

                requestParameters = requestParameters.Substring(0, requestParameters.Length - 1);

                byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                wr.ContentLength = byteData.Length;
                Stream newStream = wr.GetRequestStream();
                
                //Envio de parametros                    
                newStream.Write(byteData, 0, byteData.Length);
            }

            return wr.GetResponse();
        }

        /// <summary>
        /// Comprueba si existe una URL. Devuelve True si existe o False en caso contrario.
        /// </summary>
        /// <param name="pUrl">Url que se desa comprobar</param>
        /// <returns>Devuelve True si existe o False en caso contrario.</returns>
        public static bool ExisteUrl(string pUrl)
        {
            try
            {
                //comprobar que es una url valida
                string regexpresion = "^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\\:\\/\\/)?([\\w\\.\\-]+(\\:[\\w\\.\\&%\\$\\-]+)*@)?((([^\\s\\(\\)\\<\\>\\\\\\\"\\.\\[\\]\\,@;:]+)(\\.[^\\s\\(\\)\\<\\>\\\\\\\"\\.\\[\\]\\,@;:]+)*(\\.[a-zA-Z]{2,4}))|((([01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}([01]?\\d{1,2}|2[0-4]\\d|25[0-5])))(\\b\\:(6553[0-5]|655[0-2]\\d|65[0-4]\\d{2}|6[0-4]\\d{3}|[1-5]\\d{4}|[1-9]\\d{0,3}|0)\\b)?((\\/[^\\/][\\w\\.\\,\\?\\'\\\\\\/\\+&%\\$#\\=~_\\-@:]*)*[^\\.\\,\\?\\\"\\'\\(\\)\\[\\]!;<>{}\\s\\x7F-\\xFF])?)$";

                Regex reg = new Regex(regexpresion);
                if (reg.IsMatch(pUrl.Trim()))
                {
                    new WebClient().DownloadData(pUrl);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve la url semántica actual. Antiguo Request.Url.
        /// </summary>
        /// <returns>Url semántica actual</returns>
        public string RequestUrl()
        {
            return RequestUrl(_httpContextAccessor.HttpContext.Request);
        }

        /// <summary>
        /// Devuelve la url semántica actual. Antiguo Request.Url.
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <returns>Url semántica actual</returns>
        public static string RequestUrl(HttpRequest pRequest)
        {        
            string url = pRequest.Scheme + "://" + new Uri(UriHelper.GetEncodedUrl(pRequest.HttpContext.Request)).Authority + pRequest.Path;
            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf("?"));
            }

            url += pRequest.QueryString.ToString();

            return url;
        }

        /// <summary>
        /// Devuelve la url semántica actual sin query. Antiguo Request.Url sin query.
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <returns>Url semántica actual</returns>
        public static string RequestUrlSinQuery(HttpRequest pRequest)
        {
            string url = pRequest.Scheme + "://" + new Uri(UriHelper.GetEncodedUrl(pRequest.HttpContext.Request)).Authority + pRequest.PathBase;
            return url;
        }

        /// <summary>
        /// Devuelve la url semántica actual. Antiguo Request.AbsoluteUri.
        /// </summary>
        /// <returns>Url semántica actual</returns>
        public string AbsoluteUri()
        {
            return RequestUrl();
        }

        /// <summary>
        /// Devuelve la url semántica actual. Antiguo Request.AbsoluteUri.
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <returns>Url semántica actual</returns>
        public string AbsoluteUri(HttpRequest pRequest)
        {
            return RequestUrl(pRequest);
        }

        /// <summary>
        /// Devuelve la url semántica actual. Antiguo Request.Segments.
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <returns>Url semántica actual</returns>
        public static string[] Segments(HttpRequest pRequest)
        {
            string pathBase = pRequest.PathBase;
            string[] segmentos = pathBase.Split(new char[] { '/' });

            for (int i = 0; i < segmentos.Length; i++)
            {
                segmentos[i] += "/";
            }

            return segmentos;
        }


        /// <summary>
        /// Devuelve la url semántica actual. Antiguo Request.AbsolutePath.
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <returns>Url semántica actual</returns>
        public static string AbsolutePath(HttpRequest pRequest)
        {
            return pRequest.PathBase;
        }


        /// <summary>
        /// Make a POST request to an url with an oauth sign and an object in the body of the request as json
        /// </summary>
        /// <param name="url">Url to make the request</param>
        /// <param name="model">Object to send in the body request as json</param>
        /// <param name="acceptHeader">(Optional) Accept header</param>
        /// <returns>Response of the server</returns>
        public static string WebRequestPostWithJsonObject(string url, object model, string acceptHeader = "", Dictionary<HttpRequestHeader, string> cabecerasAdicionales = null)
        {
            string json = JsonConvert.SerializeObject(model);
            return WebRequest("POST", url, json, "application/json", acceptHeader, cabecerasAdicionales);
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
        public static string WebRequest(string httpMethod, string url, string postData = "", string contentType = "", string acceptHeader = "", Dictionary<HttpRequestHeader, string> cabecerasAdicionales = null)
        {
            HttpContent contentData = new StringContent(postData, System.Text.Encoding.UTF8, "application/json");
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                response = client.PostAsync($"{url}", contentData).Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
                return result;
            }
            catch (HttpRequestException ex)
            {
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
            }
        }

        /*
        public static string WebRequest(string httpMethod, string url, string postData = "", string contentType = "", string acceptHeader = "", Dictionary<HttpRequestHeader, string> cabecerasAdicionales = null)
        {
            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = httpMethod;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 200000;

            string signUrl = webRequest.RequestUri.ToString();

            if (!string.IsNullOrEmpty(webRequest.RequestUri.Query))
            {
                signUrl = webRequest.RequestUri.ToString().Replace(webRequest.RequestUri.Query, "");
            }

            if (!string.IsNullOrEmpty(contentType))
            {
                webRequest.ContentType = contentType;
            }
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                webRequest.Accept = acceptHeader;
            }

            if (cabecerasAdicionales != null)
            {
                foreach (HttpRequestHeader header in cabecerasAdicionales.Keys)
                {
                    webRequest.Headers.Add(header, cabecerasAdicionales[header]);
                }
            }


            if (httpMethod == "POST" || httpMethod == "PUT" || httpMethod == "DELETE")
            {
                requestWriter = new StreamWriter(webRequest.GetRequestStream());
                try
                {
                    requestWriter.Write(postData);
                }
                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }
            try
            {
                responseData = WebResponseGet(webRequest);
            }
            catch (WebException ex)
            {
                string message = null;
                StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                message = sr.ReadToEnd();
                throw new Exception(message, ex);
            }

            webRequest = null;

            return responseData;
        }*/



        #endregion
    }
}
