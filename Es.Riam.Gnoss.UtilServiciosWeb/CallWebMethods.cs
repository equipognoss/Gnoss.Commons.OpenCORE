using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallWebMethods
    {
        /// <summary>
        /// Hace una petición get
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        public static string CallGetApi(string urlBase, string urlMethod)
        {
            string result = "";
            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(urlBase) && !urlBase.EndsWith("/") && !string.IsNullOrEmpty(urlMethod) && !urlMethod.StartsWith("/"))
            {
                urlBase = $"{urlBase}/";
            }
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
                response = client.GetAsync($"{urlBase}{urlMethod}").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                StringBuilder except = new StringBuilder();
                except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    except.AppendLine(response.Content.ReadAsStringAsync().Result);
                    throw new HttpRequestException(except.ToString());
                }
                else
                {
                    except.AppendLine(response.ReasonPhrase);
                    throw new HttpRequestException(except.ToString());
                }
            }
            catch (Exception ex)
            {
                StringBuilder except = new StringBuilder();
                except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                except.AppendLine(ex.Message);
                throw new Exception(except.ToString());
            }
            return result;
        }

        /// <summary>
        /// Hace una petición post al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        public static string CallPostApi(string urlBase, string urlMethod, object item, bool isFile = false, string fileName = "file")
        {
            HttpContent contentData = null;
            if (!string.IsNullOrEmpty(urlBase) && !urlBase.EndsWith("/") && !string.IsNullOrEmpty(urlMethod) && !urlMethod.StartsWith("/"))
            {
                urlBase = $"{urlBase}/";
            }
            if (!isFile)
            {
                if (item != null)
                {
                    string stringData = JsonConvert.SerializeObject(item);
                    string contentType = "application/json";
                    contentData = new StringContent(stringData, System.Text.Encoding.UTF8, contentType);

                }
            }
            else
            {
                byte[] data;
                if (!(item is byte[])) 
                {
                    using (var br = new BinaryReader(((IFormFile)item).OpenReadStream()))
                    {
                        data = br.ReadBytes((int)((IFormFile)item).OpenReadStream().Length);
                    }

                }
                else
                {
                    data = (byte[])item;
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                contentData = new MultipartFormDataContent();
                ((MultipartFormDataContent)contentData).Add(bytes, fileName, fileName);
            }
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
                response = client.PostAsync($"{urlBase}{urlMethod}", contentData).Result;
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



        /// <summary>
        /// Hace una petición post al api
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="item">objeto a pasar</param>
        /// <param name="isFile">si el objeto pasado es un fichero</param>
        /// <param name="fileName">nombre del parametro del fichero, en el caso de que el objeto pasado sea un fichero</param>
        /// <param name="pToken">Token para validar la peticion</param>
        public static string CallPostApiToken(string urlBase, string urlMethod, object item, bool isFile = false, string fileName = "file", TokenBearer pToken = null)
        {
            HttpContent contentData = null;
            if (!string.IsNullOrEmpty(urlBase) && !urlBase.EndsWith("/") && !string.IsNullOrEmpty(urlMethod) && !urlMethod.StartsWith("/"))
            {
                urlBase = $"{urlBase}/";
            }
            if (!isFile)
            {
                if (item != null)
                {
                    string stringData = JsonConvert.SerializeObject(item);
                    string contentType = "application/json";
                    contentData = new StringContent(stringData, System.Text.Encoding.UTF8, contentType);

                }
            }
            else
            {
                byte[] data;
                if (!(item is byte[]))
                {
                    using (var br = new BinaryReader(((IFormFile)item).OpenReadStream()))
                    {
                        data = br.ReadBytes((int)((IFormFile)item).OpenReadStream().Length);
                    }

                }
                else
                {
                    data = (byte[])item;
                }
                ByteArrayContent bytes = new ByteArrayContent(data);
                contentData = new MultipartFormDataContent();
                ((MultipartFormDataContent)contentData).Add(bytes, fileName, fileName);
            }
            string result = "";
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
                if (pToken != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{pToken.token_type} {pToken.access_token}");
                }
                response = client.PostAsync($"{urlBase}{urlMethod}", contentData).Result;
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

        /// <summary>
        /// Hace una petición get
        /// </summary>
        /// <param name="urlBase">Url donde se encuentra el api</param>
        /// <param name="urlMethod">Url del método</param>
        /// <param name="pToken">Token para validar la peticion</param>
        public static string CallGetApiToken(string urlBase, string urlMethod, TokenBearer pToken = null)
        {
            string result = "";
            HttpResponseMessage response = null;
            if (!string.IsNullOrEmpty(urlBase) && !urlBase.EndsWith("/") && !string.IsNullOrEmpty(urlMethod) && !urlMethod.StartsWith("/"))
            {
                urlBase = $"{urlBase}/";
            }
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
                if (pToken != null)
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"{pToken.token_type} {pToken.access_token}");
                }
                response = client.GetAsync($"{urlBase}{urlMethod}").Result;
                response.EnsureSuccessStatusCode();
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (HttpRequestException)
            {
                StringBuilder except = new StringBuilder();
                except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    except.AppendLine(response.Content.ReadAsStringAsync().Result);
                    throw new HttpRequestException(except.ToString());
                }
                else
                {
                    except.AppendLine(response.ReasonPhrase);
                    throw new HttpRequestException(except.ToString());
                }
            }
            catch (Exception ex)
            {
                StringBuilder except = new StringBuilder();
                except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                except.AppendLine(ex.Message);
                throw new Exception(except.ToString());
            }
            return result;
        }

    }
}
