﻿using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Es.Riam.Web.Util
{
    /// <summary>
    /// Clase para hacer llamadas http.
    /// </summary>
    public class UtilHttpWeb
    {
        /// <summary>
        /// Hace una petición HTTP POST a una url.
        /// </summary>
        /// <param name="pUrl">Url petición</param>
        /// <param name="pParametros">Parámetros POST</param>
        /// <returns>Respuesta de la petición</returns>
        public static string HacerPeticionPost(string pUrl, Dictionary<string, string> pParametros)
        {
            string parametros = "";

            foreach (string param in pParametros.Keys)
            {
                parametros += param + "=" + HttpUtility.UrlEncode(pParametros[param]) + "&";
            }

            parametros = parametros.Substring(0, parametros.Length - 1);

            return HacerPeticion(pUrl, "application/x-www-form-urlencoded", parametros, null, "POST");
        }

        /// <summary>
        /// Hace una petición HTTP POST a una url.
        /// </summary>
        /// <param name="pUrl">Url petición</param>
        /// <param name="pContentType">Content type de la llamada</param>
        /// <param name="pCuerpo">Cuerpo de la llamada</param>
        /// <returns>Respuesta de la petición</returns>
        public static string HacerPeticion(string pUrl, string pContentType, string pCuerpo, string pAuthorization, string pMetodo)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pUrl);

            request.UserAgent = UtilWeb.GenerarUserAgent();
            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            //request.MaximumResponseHeadersLength = 4;
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;

            request.Method = pMetodo;

            if (!string.IsNullOrEmpty(pAuthorization))
            {
                request.PreAuthenticate = true;
                request.Headers["Authorization"] = pAuthorization;
            }

            request.UserAgent = UtilWeb.GenerarUserAgent();
            request.KeepAlive = true;
            request.ContentType = pContentType;
            request.Timeout = 600000; // 10 minutos de timeout

            if (!string.IsNullOrEmpty(pCuerpo))
            {
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] paramByte = encoding.GetBytes(pCuerpo);
                request.ContentLength = paramByte.Length;

                Stream streamRequest = request.GetRequestStream();
                streamRequest.Write(paramByte, 0, paramByte.Length);
                streamRequest.Flush();
                streamRequest.Close();
                streamRequest.Dispose();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string respuesta = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            readStream.Dispose();

            return respuesta;
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
        public static string HacerPeticionPostHttpClient(string url, string postData, string contentType, string acceptHeader = "", Dictionary<HttpRequestHeader, string> cabecerasAdicionales = null)
        {
            HttpContent contentData = new StringContent(postData, System.Text.Encoding.UTF8, contentType);
            contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
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

        public static string EnviarFicheroEnPeticionPOST(string pUrl, byte[] pBytes)
        {
            string respuesta = "";
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pUrl);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Timeout = 600 * 1000; //10 minutos
            request.UserAgent = UtilWeb.GenerarUserAgent();

            BinaryWriter binaryWriter = new BinaryWriter(request.GetRequestStream());

            binaryWriter.Write(pBytes);

            WebResponse wresp = null;
            try
            {
                wresp = request.GetResponse();
                StreamReader reader = new StreamReader(wresp.GetResponseStream());
                if (reader != null)
                {
                    respuesta = reader.ReadToEnd();

                    reader.Close();
                }
            }
            finally
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
                request = null;
            }

            return respuesta;
        }

        ///// <summary>
        ///// Hace una petición HTTP a una url.
        ///// </summary>
        ///// <param name="pUrl">Url petición</param>
        ///// <param name="pParametros">Parámetros POST</param>
        ///// <returns>Respuesta de la petición</returns>
        //public static string HacerPeticion(string pUrl, string pMetodo, string pAuthorization)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(pUrl);

        //    // Set some reasonable limits on resources used by this request
        //    request.MaximumAutomaticRedirections = 4;
        //    // Set credentials to use for this request.
        //    request.Credentials = CredentialCache.DefaultCredentials;

        //    request.Method = pMetodo;
        //    request.PreAuthenticate = true;

        //    if (!string.IsNullOrEmpty(pAuthorization))
        //    {
        //        request.Headers["Authorization"] = pAuthorization;
        //    }

        //    request.UserAgent = "gnoss";
        //    request.KeepAlive = true;

        //    request.ContentType = "application/x-www-form-urlencoded";

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        //    // Get the stream associated with the response.
        //    Stream receiveStream = response.GetResponseStream();

        //    // Pipes the stream to a higher level stream reader with the required encoding format. 
        //    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

        //    string respuesta = readStream.ReadToEnd();
        //    response.Close();
        //    readStream.Close();
        //    readStream.Dispose();

        //    return respuesta;
        //}
    }
}