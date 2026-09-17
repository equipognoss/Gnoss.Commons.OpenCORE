using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
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
            /// <summary>GET</summary>
            GET,
            /// <summary>POST</summary>
            POST,
            /// <summary>PUT</summary>
            PUT,
            /// <summary>DELETE</summary>
            DELETE
        }

        #endregion

        private IHttpContextAccessor _httpContextAccessor;

        private static readonly HttpClient mHttpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        private static readonly HttpClient mHttpClientNoRedirect = new HttpClient(new HttpClientHandler
        {
            AllowAutoRedirect = false
        })
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

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
        public static string WebRequestStringData(Metodo pMetodo, string pUrl, string pPostData, HttpRequest pRequest = null)
        {
            return WebRequestStringData(pMetodo, pUrl, pPostData, "application/x-www-form-urlencoded", true, null, pRequest);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        public string WebRequestStringData(Metodo pMetodo, string pUrl, string pPostData, string pContentType)
        {
            return WebRequestStringData(pMetodo, pUrl, pPostData, pContentType, true);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        public string WebRequestStringData(Metodo pMetodo, string pUrl, string pPostData, string pContentType, bool pSeguirRedireccion)
        {
            return WebRequestStringData(pMetodo, pUrl, pPostData, pContentType, pSeguirRedireccion, null);
        }

        /// <summary>
        /// Envía una petición web
        /// </summary>
        public string WebRequestStringData(Metodo pMetodo, string pUrl, string pPostData, string pContentType, bool pSeguirRedireccion, Dictionary<string, string> pCabeceras)
        {
            try
            {
                HttpClient client = pSeguirRedireccion ? mHttpClient : mHttpClientNoRedirect;
                HttpRequestMessage request = BuildRequest(pMetodo, pUrl, pPostData, pContentType, pCabeceras);

                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private HttpRequestMessage BuildRequest(Metodo pMetodo, string pUrl, string pPostData, string pContentType, Dictionary<string, string> pCabeceras)
        {
            var request = new HttpRequestMessage(new HttpMethod(pMetodo.ToString()), pUrl);

            SetUserAgentAndReferer(request);
            SetCabeceras(request, pCabeceras);

            if (pMetodo == Metodo.POST || pMetodo == Metodo.PUT)
            {
                request.Content = new StringContent(pPostData ?? string.Empty, Encoding.UTF8, pContentType);
            }

            return request;
        }

        private void SetUserAgentAndReferer(HttpRequestMessage request)
        {
            Uri uriActual = null;
            string userAgent;

            if (_httpContextAccessor?.HttpContext?.Request != null)
            {
                userAgent = _httpContextAccessor.HttpContext.Request.Headers["UserAgent"];
                uriActual = new Uri(UriHelper.GetEncodedUrl(_httpContextAccessor.HttpContext.Request));
            }
            else
            {
                userAgent = GenerarUserAgent();
            }

            if (_httpContextAccessor?.HttpContext?.Request?.Headers != null)
            {
                string accept = _httpContextAccessor.HttpContext.Request.Headers["Accept"];
                if (!string.IsNullOrEmpty(accept) && accept.Contains("application/json"))
                {
                    userAgent += " GnossInternalRequest";
                }
            }
            else
            {
                userAgent += " GnossInternalRequest";
            }

            if (!string.IsNullOrEmpty(userAgent))
            {
                request.Headers.TryAddWithoutValidation("User-Agent", userAgent);
            }

            if (uriActual != null)
            {
                request.Headers.TryAddWithoutValidation("Referer", uriActual.ToString());
            }
        }

        private void SetCabeceras(HttpRequestMessage request, Dictionary<string, string> pCabeceras)
        {
            if (pCabeceras?.Count > 0)
            {
                foreach (var cabecera in pCabeceras)
                {
                    if (cabecera.Key.ToLower().Equals("accept"))
                    {
                        request.Headers.Accept.ParseAdd(cabecera.Value);
                    }
                    else
                    {
                        request.Headers.TryAddWithoutValidation(cabecera.Key, cabecera.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Envía una petición web (estático)
        /// </summary>
        public static string WebRequestStringData(Metodo pMetodo, string pUrl, string pPostData, string pContentType, bool pSeguirRedireccion, Dictionary<string, string> pCabeceras, HttpRequest pRequest = null)
        {
            try
            {
                HttpClient client = pSeguirRedireccion ? mHttpClient : mHttpClientNoRedirect;
                var request = new HttpRequestMessage(new HttpMethod(pMetodo.ToString()), pUrl);

                string userAgent = GenerarUserAgent();
                Uri uriActual = null;

                if (pRequest?.HttpContext?.Request != null)
                {
                    uriActual = new Uri(UriHelper.GetEncodedUrl(pRequest.HttpContext.Request));
                    string accept = pRequest.HttpContext.Request.Headers["Accept"];
                    if (!string.IsNullOrEmpty(accept))
                    {
                        request.Headers.Accept.ParseAdd(accept);
                        if (accept.Contains("application/json"))
                        {
                            userAgent += " GnossInternalRequest";
                        }
                    }
                }
                else
                {
                    userAgent += " GnossInternalRequest";
                }

                request.Headers.TryAddWithoutValidation("User-Agent", userAgent);

                if (uriActual != null)
                {
                    string urlReferer = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(uriActual.ToString()));
                    request.Headers.TryAddWithoutValidation("Referer", urlReferer);
                }

                if (pCabeceras?.Count > 0)
                {
                    foreach (var cabecera in pCabeceras)
                    {
                        if (cabecera.Key.ToLower().Equals("accept"))
                        {
                            request.Headers.Accept.ParseAdd(cabecera.Value);
                        }
                        else
                        {
                            request.Headers.TryAddWithoutValidation(cabecera.Key, cabecera.Value);
                        }
                    }
                }

                if (pPostData?.Contains("pIdentidadID") == true)
                {
                    string identidadID = ObtenerIdentidadID(pPostData);
                    request.Headers.TryAddWithoutValidation("Authorization", $"bearer {identidadID}");
                }

                if (pMetodo == Metodo.POST || pMetodo == Metodo.PUT)
                {
                    request.Content = new StringContent(pPostData ?? string.Empty, Encoding.UTF8, pContentType);
                }

                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private static string ObtenerIdentidadID(string pPostData)
        {
            return pPostData.Split("pIdentidadID=")[1].Split("&")[0];
        }

        /// <summary>
        /// Hace una petición POST a una URL con una serie de parámetros
        /// </summary>
        public static string HacerPeticionPost(string pUrl, Dictionary<string, string> pParametros, Dictionary<string, string> pCabeceras = null)
        {
            string requestParameters = "";

            if (pParametros != null && pParametros.Count > 0)
            {
                foreach (string key in pParametros.Keys)
                {
                    requestParameters = string.Concat(requestParameters, key, "=", HttpUtility.UrlEncode(pParametros[key]), "&");
                }
                requestParameters = requestParameters.Substring(0, requestParameters.Length - 1);
            }

            return WebRequestStringData(Metodo.POST, pUrl, requestParameters, "application/x-www-form-urlencoded", true, pCabeceras);
        }

        /// <summary>
        /// Hace una petición POST devolviendo HttpResponseMessage
        /// </summary>
        public static HttpResponseMessage HacerPeticionPostDevolviendoHttpResponseMessage(string pUrl, Dictionary<string, string> pParametros)
        {
            return HacerPeticionDevolviendoHttpResponseMessage("POST", pUrl, pParametros);
        }

        /// <summary>
        /// Hace una petición POST devolviendo HttpResponseMessage
        /// </summary>
        public static HttpResponseMessage HacerPeticionPostDevolviendoHttpResponseMessage(string pUrl, byte[] pByteData)
        {
            return HacerPeticionDevolviendoHttpResponseMessage("POST", pUrl, pByteData);
        }

        /// <summary>
        /// Hace una petición GET devolviendo HttpResponseMessage
        /// </summary>
        public static HttpResponseMessage HacerPeticionGetDevolviendoHttpResponseMessage(string pUrl, string pToken = "")
        {
            return HacerPeticionDevolviendoHttpResponseMessage("GET", pUrl, new Dictionary<string, string>(), pToken);
        }

        /// <summary>
        /// Hace una petición GET devolviendo HttpResponseMessage
        /// </summary>
        public static HttpResponseMessage HacerPeticionGetDevolviendoHttpResponseMessage(Uri pUrl, string pToken = "")
        {
            return HacerPeticionDevolviendoHttpResponseMessage("GET", pUrl, new Dictionary<string, string>(), pToken);
        }

        public static HttpResponseMessage HacerPeticionDevolviendoHttpResponseMessage(string pMethod, string pUrl, Dictionary<string, string> pParametros, string pToken = "")
        {
            try
            {
                HttpResponseMessage response = HacerPeticionDevolviendoHttpResponseMessageSinValidarEstado(pMethod, pUrl, pParametros, pToken);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Hace una petición devolviendo el HttpResponseMessage sin comprobar el código de estado de la respuesta.
        /// A diferencia de HacerPeticionDevolviendoHttpResponseMessage, no lanza excepción cuando la respuesta no es
        /// correcta, por lo que el llamante puede leer el estado y el cuerpo, donde el servicio detalla el motivo del error.
        /// </summary>
        /// <param name="pMethod">Método HTTP de la petición</param>
        /// <param name="pUrl">Url a la que se hace la petición</param>
        /// <param name="pParametros">Parámetros que se envían como formulario</param>
        /// <param name="pToken">Token bearer, si la petición va autenticada</param>
        /// <returns>La respuesta del servicio, sea cual sea su código de estado</returns>
        public static HttpResponseMessage HacerPeticionDevolviendoHttpResponseMessageSinValidarEstado(string pMethod, string pUrl, Dictionary<string, string> pParametros, string pToken = "")
        {
            var request = new HttpRequestMessage(new HttpMethod(pMethod), pUrl);
            request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

            if (!string.IsNullOrEmpty(pToken))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
            }

            if (pParametros?.Count > 0)
            {
                request.Content = new FormUrlEncodedContent(pParametros);
            }
            else
            {
                request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            return mHttpClient.SendAsync(request).GetAwaiter().GetResult();
        }

        public static HttpResponseMessage HacerPeticionDevolviendoHttpResponseMessage(string pMethod, Uri pUrl, Dictionary<string, string> pParametros, string pToken = "")
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(pMethod), pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
                }

                if (pParametros?.Count > 0)
                {
                    request.Content = new FormUrlEncodedContent(pParametros);
                }
                else
                {
                    request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");
                }

                HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static HttpResponseMessage HacerPeticionDevolviendoHttpResponseMessage(string pMethod, string pUrl, byte[] pByteData, string pToken = "")
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(pMethod), pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
                }

                if (pMethod == "POST" && pByteData != null)
                {
                    request.Content = new ByteArrayContent(pByteData);
                    //request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
                }
                else
                {
                    request.Content = new StringContent(string.Empty);
                }

                HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Envía una petición web devolviendo bytes
        /// </summary>
        public static byte[] WebRequestBytes(string httpMethod, string url, byte[] byteData)
        {
            return WebRequestBytes(httpMethod, url, byteData, true);
        }

        public static byte[] WebRequestBytes(string httpMethod, string url, byte[] byteData, bool pSeguirRedireccion)
        {
            try
            {
                HttpClient client = pSeguirRedireccion ? mHttpClient : mHttpClientNoRedirect;
                var request = new HttpRequestMessage(new HttpMethod(httpMethod), url);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

                if (httpMethod == "POST")
                {
                    request.Content = byteData != null
                        ? new ByteArrayContent(byteData) { Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded") } }
                        : new StringContent(string.Empty);
                }

                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Envía una petición web devolviendo bytes con token
        /// </summary>
        public static byte[] WebRequestBytes(string httpMethod, string url, byte[] byteData, string pContentType, string pToken = "")
        {
            try
            {
                var request = new HttpRequestMessage(new HttpMethod(httpMethod), url);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
                }

                if (httpMethod == "POST")
                {
                    request.Content = byteData != null
                        ? new ByteArrayContent(byteData) { Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(pContentType) } }
                        : new StringContent(string.Empty);
                }

                HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Envía una petición web con token y body string
        /// </summary>
        public static string WebRequest(string pHttpMethod, string pUrl, byte[] pByteData, bool pRedirect = true, string pUserAgent = "")
        {
            return WebRequest(pHttpMethod, pUrl, null, pByteData, pRedirect: pRedirect, pUserAgent: pUserAgent);
        }

        public static string WebRequest(string pHttpMethod, string pUrl, string pToken, byte[] pByteData, string pContentType = "x-www-form-urlencoded", bool pRedirect = true, string pUserAgent = "")
        {
            try
            {
                HttpClient client = pRedirect ? mHttpClient : mHttpClientNoRedirect;

                var request = new HttpRequestMessage(new HttpMethod(pHttpMethod), pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", !string.IsNullOrEmpty(pUserAgent) ? pUserAgent : GenerarUserAgent());

                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
                }

                if (pHttpMethod == "POST")
                {
                    request.Content = pByteData != null
                        ? new ByteArrayContent(pByteData) { Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue($"application/{pContentType}") } }
                        : new StringContent(string.Empty);
                }
                else
                {
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue($"application/{pContentType}"));
                }

                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                string message = pUrl;
                if (ex.StatusCode != null)
                {
                    message += $"\r\nError: {ex.Message}";
                }
                return "";
            }
        }

        /// <summary>
        /// Se hace una peticion Post por defecto con un diccionario FormData
        /// </summary>
        public static HttpResponseMessage WebRequestFormDataConToken(string pUrl, Dictionary<string, string> pFormData, string pTipoToken = "Bearer", string pToken = "")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(pTipoToken, pToken);
                }

                request.Content = new FormUrlEncodedContent(pFormData);

                HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Se hace una peticion Post por defecto con un diccionario FormData
        /// </summary>
        public static HttpResponseMessage WebRequestFormDataConToken(string pUrl, byte[] pData, string pTipoToken = "Bearer", string pToken = "", bool pIsPut = false)
        {
            try
            {
                HttpMethod httpMethod = HttpMethod.Post;
                if (pIsPut)
                {
                    httpMethod = HttpMethod.Put;
                }
                var request = new HttpRequestMessage(httpMethod, pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());

                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(pTipoToken, pToken);
                }

                request.Content = pData != null
                        ? new ByteArrayContent(pData) 
                        : new StringContent(string.Empty);

                HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static HttpResponseMessage WebRequestPutStringContent(string pUrl, string pData, string pToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, pUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
            request.Content = new StringContent("write", Encoding.UTF8, "text/plain");

            HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
            return response;
        }


        /// <summary>
        /// POST con objeto JSON
        /// </summary>
        public static string WebRequestPostWithJsonObject(string pUrl, object pObjeto = null, string pToken = null)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());
                if (!string.IsNullOrEmpty(pToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", pToken);
                }
                request.Content = pObjeto != null
                    ? new StringContent(JsonConvert.SerializeObject(pObjeto), Encoding.UTF8, "application/json")
                    : new StringContent(string.Empty);

                HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public static void DescargarFichero(string pUrl, string pRuta, bool pSeguirRedireccion)
        {
            byte[] bytes = WebRequestBytes("GET", pUrl, null, pSeguirRedireccion);
            File.WriteAllBytes(pRuta, bytes);
        }

        private static readonly ConcurrentDictionary<string, HttpClient> mHttpClientsConCredenciales = new ConcurrentDictionary<string, HttpClient>();

        private static HttpClient ObtenerClienteConCredenciales(string pUrl, string pUsuario, string pPassword, int pTimeoutSegundos = 100)
        {
            if (pTimeoutSegundos == 0)
            {
                pTimeoutSegundos = 100;
            }
            string clave = $"{pUrl}_{pUsuario}";

            return mHttpClientsConCredenciales.GetOrAdd(clave, _ =>
            {
                var credentialCache = new CredentialCache();
                credentialCache.Add(
                    new Uri(pUrl),
                    "Digest",
                    new NetworkCredential(pUsuario, pPassword)
                );

                var handler = new HttpClientHandler
                {
                    Credentials = credentialCache
                };

                return new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(pTimeoutSegundos)
                };
            });
        }
        public static byte[] WebRequestCredenciales(string pMetodo, string pUrl, Dictionary<string, string> pFormData, string pUsuario, string pPassword, int pTimeoutSegundos)
        {
            try
            {
                HttpClient client = !string.IsNullOrEmpty(pUsuario)
                    ? ObtenerClienteConCredenciales(pUrl, pUsuario, pPassword, pTimeoutSegundos)
                    : mHttpClient;

                var request = new HttpRequestMessage(new HttpMethod(pMetodo), pUrl);
                request.Headers.TryAddWithoutValidation("User-Agent", GenerarUserAgent());
                //request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                if (pMetodo == "POST")
                {
                    request.Content = pFormData?.Count > 0
                        ? new FormUrlEncodedContent(pFormData)
                        : new StringContent(string.Empty);
                }

                HttpResponseMessage response = client.SendAsync(request).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        /// <summary>
        /// Comprueba si existe una URL.
        /// </summary>
        public static bool ExisteUrl(string pUrl)
        {
            try
            {
                string regexpresion = "^((([hH][tT][tT][pP][sS]?|[fF][tT][pP])\\:\\/\\/)?([\\w\\.\\-]+(\\:[\\w\\.\\&%\\$\\-]+)*@)?((([^\\s\\(\\)\\<\\>\\\\\\\"\\.\\[\\]\\,@;:]+)(\\.[^\\s\\(\\)\\<\\>\\\\\\\"\\.\\[\\]\\,@;:]+)*(\\.[a-zA-Z]{2,4}))|((([01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}([01]?\\d{1,2}|2[0-4]\\d|25[0-5])))(\\b\\:(6553[0-5]|655[0-2]\\d|65[0-4]\\d{2}|6[0-4]\\d{3}|[1-5]\\d{4}|[1-9]\\d{0,3}|0)\\b)?((\\/[^\\/][\\w\\.\\,\\?\\'\\\\\\/\\+&%\\$#\\=~_\\-@:]*)*[^\\.\\,\\?\\\"\\'\\(\\)\\[\\]!;<>{}\\s\\x7F-\\xFF])?)$";
                Regex reg = new Regex(regexpresion);
                if (reg.IsMatch(pUrl.Trim()))
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, pUrl);
                    HttpResponseMessage response = mHttpClient.SendAsync(request).GetAwaiter().GetResult();
                    return response.IsSuccessStatusCode;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Devuelve la url semántica actual.
        /// </summary>
        public string RequestUrl()
        {
            return RequestUrl(_httpContextAccessor.HttpContext.Request);
        }

        /// <summary>
        /// Devuelve la url semántica actual.
        /// </summary>
        public static string RequestUrl(HttpRequest pRequest)
        {
            string scheme = pRequest.Headers.TryGetValue("X-Forwarded-Proto", out var forwardedProto)
                ? forwardedProto.ToString().Split(',')[0].Trim()  // puede venir como "https, http" en cadena
                : pRequest.Scheme;

            string host = pRequest.Headers.TryGetValue("X-Forwarded-Host", out var forwardedHost)
                ? forwardedHost.ToString().Split(',')[0].Trim()
                : pRequest.Host.ToString();

            string prefix = pRequest.Headers.TryGetValue("X-Forwarded-Prefix", out var forwardedPrefix)
                ? forwardedPrefix.ToString()
                : string.Empty;

            string url = $"{scheme}://{host}{prefix}{pRequest.Path}";
            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf("?"));
            }
            url += pRequest.QueryString.ToString();
            return url;
        }

        /// <summary>
        /// Devuelve la url semántica actual sin query.
        /// </summary>
        public static string RequestUrlSinQuery(HttpRequest pRequest)
        {
            return pRequest.Scheme + "://" + new Uri(UriHelper.GetEncodedUrl(pRequest.HttpContext.Request)).Authority + pRequest.PathBase;
        }

        /// <summary>
        /// Devuelve la url semántica actual.
        /// </summary>
        public string AbsoluteUri()
        {
            return RequestUrl();
        }

        /// <summary>
        /// Devuelve la url semántica actual.
        /// </summary>
        public string AbsoluteUri(HttpRequest pRequest)
        {
            return RequestUrl(pRequest);
        }

        /// <summary>
        /// Devuelve los segmentos de la url.
        /// </summary>
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
        /// Devuelve el path absoluto de la url.
        /// </summary>
        public static string AbsolutePath(HttpRequest pRequest)
        {
            return pRequest.PathBase;
        }

        /// <summary>
        /// Genera el UserAgent
        /// </summary>
        public static string GenerarUserAgent()
        {
            string OSVersion = Environment.OSVersion.ToString();
            Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
            return $"Mozilla/5.0 ({OSVersion}) +https://www.gnoss.com Gnoss.WebRequestModule/{assemblyVersion}";
        }

        #endregion
    }
}
