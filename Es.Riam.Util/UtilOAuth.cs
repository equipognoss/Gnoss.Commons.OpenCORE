using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Web;

namespace Es.Riam.Util
{
    /// <summary>
    /// Utilidades para trabajar con OAuth.
    /// </summary>
    public class UtilOAuth
    {
        /// <summary>
        /// Caracteres no reservados de oauth.
        /// </summary>
        protected static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Obtiene una URL con la petición OAuth trasformada a GET.
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <returns>URL GET para petición OAuth</returns>
        public static string ObtenerUrlGetDePeticionOAuth(HttpRequest pRequest, string urlOriginal = null, bool pLimpiarParametrosAdicionales = true)
        {
            string token = null;
            string consumerKey = null;
            string nonce = null;
            string method = null;
            string timespan = null;
            string signature = null;

            if (!string.IsNullOrEmpty(pRequest.Query["oauth_token"]))
            {
                token = UrlEncode(UrlDecode(pRequest.Query["oauth_token"]));
                consumerKey = UrlEncode(UrlDecode(pRequest.Query["oauth_consumer_key"]));
                nonce = pRequest.Query["oauth_nonce"];
                method = pRequest.Query["oauth_signature_method"];
                timespan = pRequest.Query["oauth_timestamp"];
                signature = UrlEncode(UrlDecode(pRequest.Query["oauth_signature"]));
            }
            else
            {
                //Lo obtengo del query string
                string http_authorization = "Authorization"; //Cambiarlo por la cabecera

                if (pRequest.Headers.ContainsKey(http_authorization))
                {
                    string parametros = pRequest.Headers[http_authorization];
                    char[] separadores = { ',' };
                    string[] listaParams = parametros.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string param in listaParams)
                    {
                        char[] separadoIgual = { '=' };
                        string[] claveValor = param.Split(separadoIgual, StringSplitOptions.RemoveEmptyEntries);
                        if (claveValor.Length > 1)
                        {
                            string valor = claveValor[1].Replace("\"", "").Trim();
                            switch (claveValor[0].Trim().ToLower())
                            {
                                case "oauth_token":
                                    token = UrlEncode(UrlDecode(valor));
                                    break;
                                case "oauth_consumer_key":
                                    consumerKey = UrlEncode(UrlDecode(valor));
                                    break;
                                case "oauth_nonce":
                                    nonce = valor;
                                    break;
                                case "oauth_signature_method":
                                    method = valor;
                                    break;
                                case "oauth_timestamp":
                                    timespan = valor;
                                    break;
                                case "oauth_signature":
                                    signature = UrlEncode(UrlDecode(valor));
                                    break;
                            }
                        }
                    }
                }
            }


            token = "oauth_token=" + token;
            consumerKey = "&oauth_consumer_key=" + consumerKey;
            nonce = "&oauth_nonce=" + nonce;
            method = "&oauth_signature_method=" + method;
            timespan = "&oauth_timestamp=" + timespan;
            signature = "&oauth_signature=" + signature;

            string url = UtilWeb.RequestUrl(pRequest);

            if (!string.IsNullOrEmpty(urlOriginal))
            {
                url = urlOriginal + pRequest.Path;
                if (url.Contains("?"))
                {
                    url = url.Substring(0, url.IndexOf("?"));
                }

                url += pRequest.QueryString.ToString();
                
            }

            if (pLimpiarParametrosAdicionales)
            {
                //url = LimpiarParametrosExpurios(UtilWeb.RequestUrl(pRequest));
                url = LimpiarParametrosExpurios(url);
                if (!url.Contains("?"))
                {
                    url += "?";
                }
                else
                {
                    url += "&";
                }

                url += token + consumerKey + nonce + method + timespan + signature;
            }

            return url;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static string UrlDecode(string value)
        {
            //Si te hace un decode del signo +, te lo convierte en un espacio. Lo convertimos a %2b para que no haga la conversión. 
            return HttpUtility.UrlDecode(value.Replace("+", "%2b"));
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public static string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Elimina los parámetros que no sirven para la petición OAuth de la URL.
        /// </summary>
        /// <param name="pUrl">URL</param>
        /// <returns>URL sin los parámetros que no sirven para la petición OAuth</returns>
        public static string LimpiarParametrosExpurios(string pUrl)
        {
            string url = pUrl;

            if (url.Contains("?"))
            {
                url = url.Substring(0, url.IndexOf("?"));
            }

            return url;
        }
    }
}
