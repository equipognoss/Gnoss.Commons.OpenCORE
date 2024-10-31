using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace Es.Riam.Util
{
    /// <summary>
    /// Útiles generales
    /// </summary>
    public class UtilGeneral
    {

        public UtilGeneral()
        {
        }

        #region Métodos estáticos

        public static T Copiar<T>(T objeto)
        {
            //Verificamos que sea serializable antes de hacer la copia
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("El tipo de dato debe ser serializable.", "objeto");
            }
            if (Object.ReferenceEquals(objeto, null))
            {
                return default(T);
            }
            //Creamos un stream en memoria
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, objeto);
                stream.Seek(0, SeekOrigin.Begin);
                //Deserializamos la porcón de memoria en el nuevo objeto
                return (T)formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Crea un nuevo objeto del mismo tipo que el del objeto pasado por parámetro
        /// </summary>
        /// <param name="pObjetoOriginal">Objeto del que se va a copiar el tipo</param>
        /// <returns>Objeto del mismo tipo</returns>
        public static object CrearObjetoDelMismoTipo(object pObjetoOriginal)
        {
            Type tipo = pObjetoOriginal.GetType();
            Assembly ensamblado = Assembly.GetAssembly(tipo);
            return ensamblado.CreateInstance(tipo.FullName);
        }

        /// <summary>
        /// Comprueba si dos arrays son iguales
        /// </summary>
        /// <param name="pArray1">Array 1</param>
        /// <param name="pArray2">Array 2</param>
        /// <returns>TRUE si son iguales</returns>
        public static bool ArraysIguales(Array pArray1, Array pArray2)
        {
            // Si alguno de los dos es nulo o tienen diferente longitud no son iguales
            if (pArray1 == null || pArray2 == null || !pArray1.Length.Equals(pArray2.Length))
                return false;

            // Comparar elemento con elemento
            for (int i = 0; i < pArray1.Length; i++)
            {
                if (!pArray1.GetValue(i).Equals(pArray2.GetValue(i)))
                    return false;
            }

            // Si se ha llegado hasta aquí, los arrays son iguales
            return true;
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
        public static string WebRequest(string httpMethod, string url, byte[] byteData)
        {
            HttpWebRequest webRequest = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = httpMethod;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.UserAgent = UtilWeb.GenerarUserAgent();

            if (httpMethod == "POST")
            {
                webRequest.ContentLength = 0;

                if (byteData != null)
                {
                    webRequest.ContentLength = byteData.Length;

                    Stream dataStream = webRequest.GetRequestStream();
                    dataStream.Write(byteData, 0, byteData.Length);
                    dataStream.Close();
                }
            }
            try
            {
                responseData = WebResponseGet(webRequest);
            }
            catch (WebException ex)
            {
                string message = url;
                try
                {
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                    message += "\r\nError: " + sr.ReadToEnd();
                }
                catch { }

                // Error reading the error response, throw the original exception
                throw new Exception(message, ex);
            }

            webRequest = null;

            return responseData;
        }

        public static string WebRequest(string httpMethod, string url, string token, byte[] byteData)
        {
            HttpWebRequest webRequest = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = httpMethod;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Headers.Add("Authorization", "Bearer " + token);
            webRequest.UserAgent = UtilWeb.GenerarUserAgent();
            if (httpMethod == "POST")
            {
                webRequest.ContentLength = 0;

                if (byteData != null)
                {
                    webRequest.ContentLength = byteData.Length;

                    Stream dataStream = webRequest.GetRequestStream();
                    dataStream.Write(byteData, 0, byteData.Length);
                    dataStream.Close();
                }
            }
            try
            {
                responseData = WebResponseGet(webRequest);
            }
            catch (WebException ex)
            {
                string message = url;
                try
                {
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                    message += "\r\nError: " + sr.ReadToEnd();
                }
                catch { }

                // Error reading the error response, throw the original exception
                //throw new Exception(message, ex);
                return "";
            }

            webRequest = null;

            return responseData;
        }

        public static string WebRequestPost(string pUrl, object pObjeto = null)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 3600000;
            webRequest.UserAgent = UtilWeb.GenerarUserAgent();

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
                    throw new Exception(respuesta, ex);
                }
                throw;
            }
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
        public byte[] WebRequestBytes(string httpMethod, string url, byte[] byteData)
        {
            HttpWebRequest webRequest = null;
            byte[] responsebytes = null;

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = httpMethod;
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 600000;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.UserAgent = UtilWeb.GenerarUserAgent();

            if (httpMethod == "POST")
            {
                webRequest.ContentLength = 0;

                if (byteData != null)
                {
                    webRequest.ContentLength = byteData.Length;

                    Stream dataStream = webRequest.GetRequestStream();
                    dataStream.Write(byteData, 0, byteData.Length);
                    dataStream.Close();
                }
            }
            try
            {
                var webResponse = webRequest.GetResponse();

                using (BinaryReader ns = new BinaryReader(webResponse.GetResponseStream()))
                {
                    responsebytes = ns.ReadBytes((int)webResponse.ContentLength);
                }
            }
            catch (WebException ex)
            {
                string message = null;
                try
                {
                    StreamReader sr = new StreamReader(ex.Response.GetResponseStream());
                    message = sr.ReadToEnd();
                }
                catch { }

                // Error reading the error response, throw the original exception
                throw;
            }

            webRequest = null;

            return responsebytes;
        }

        /// <summary>
        /// Make a http get request
        /// </summary>
        /// <param name="pWebRequest">HttpWebRequest object</param>
        /// <returns>Server response</returns>
        private static string WebResponseGet(HttpWebRequest pWebRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(pWebRequest.GetResponse().GetResponseStream(), Encoding.UTF8);
                responseData = responseReader.ReadToEnd();
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

        #endregion
    }

}
