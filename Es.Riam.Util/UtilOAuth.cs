using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        /// <param name="pUrlApi">Url publica API</param>
        /// <returns>URL GET para petición OAuth</returns>
        public static string ObtenerUrlGetDePeticionOAuth(HttpRequest pRequest, string pUrlApi)
        {
            // 1. Extraer parámetros OAuth de la cabecera Authorization
            var oauthParams = ParseAuthorizationHeader(pRequest);
            if (oauthParams is null)
                return null;

            // 2. Construir la URL base (sin query string)
            string scheme = pRequest.Headers.TryGetValue("X-Forwarded-Proto", out var forwardedProto)
               ? forwardedProto.ToString().Split(',')[0].Trim()  // puede venir como "https, http" en cadena
               : pRequest.Scheme;

            string host = pRequest.Headers.TryGetValue("X-Forwarded-Host", out var forwardedHost)
                ? forwardedHost.ToString().Split(',')[0].Trim()
                : pRequest.Host.ToString();

            Uri uriPublicaApi = new Uri(pUrlApi);
            string prefix = string.Empty;
            if (pRequest.Headers.TryGetValue("X-Forwarded-Prefix", out var forwardedPrefix))
            {
                prefix = forwardedPrefix.ToString();
            }
            else if (host == uriPublicaApi.Host) { 
                prefix = uriPublicaApi.AbsolutePath.TrimEnd('/');
            }

            string baseUrl = $"{scheme}://{host}{prefix}{pRequest.Path}";

            if (baseUrl.Contains("?"))
                baseUrl = baseUrl[..baseUrl.IndexOf('?')];

            // 3. Empezar con los parámetros de query string originales (si los hay)
            var sb = new StringBuilder(baseUrl);
            var separator = "?";

            var originalQuery = pRequest.QueryString.ToString().TrimStart('?');
            if (!string.IsNullOrEmpty(originalQuery))
            {
                sb.Append('?').Append(originalQuery);
                separator = "&";
            }

            // 4. Añadir los parámetros OAuth encodeados
            sb.Append(separator);
            sb.Append("oauth_token=").Append(UrlEncode(oauthParams["oauth_token"]));
            sb.Append("&oauth_consumer_key=").Append(UrlEncode(oauthParams["oauth_consumer_key"]));
            sb.Append("&oauth_nonce=").Append(UrlEncode(oauthParams["oauth_nonce"]));
            sb.Append("&oauth_signature_method=").Append(UrlEncode(oauthParams["oauth_signature_method"]));
            sb.Append("&oauth_timestamp=").Append(UrlEncode(oauthParams["oauth_timestamp"]));
            sb.Append("&oauth_signature=").Append(UrlEncode(oauthParams["oauth_signature"]));
            sb.Append("&oauth_version=").Append(UrlEncode(oauthParams["oauth_version"]));

            return sb.ToString();
        }

        private static Dictionary<string, string> ParseAuthorizationHeader(HttpRequest pRequest)
        {
            if (!pRequest.Headers.TryGetValue("Authorization", out var headerValue))
                return null;

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var part in headerValue.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var idx = part.IndexOf('=');
                if (idx < 0) continue;

                var key = part[..idx].Trim();
                var value = part[(idx + 1)..].Trim().Trim('"');

                if (key.Equals("realm", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("OAuth", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Decodificar el valor que viene de la cabecera
                result[key] = UrlDecode(value);
            }

            return result.Count > 0 ? result : null;
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
