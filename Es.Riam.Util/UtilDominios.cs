using System;
using System.Collections.Generic;

namespace Es.Riam.Util
{
    /// <summary>
    /// Utilidades de dominios
    /// </summary>
    public static class UtilDominios
    {
        /// <summary>
        /// Obtiene el dominio de una URL (de la manera .gnoss.com)
        /// </summary>
        /// <param name="pUrl">Url de la que se quiere saber su dominio</param>
        /// <param name="pConHTTP">Verdad si se debe añadir http:// al principio del dominio</param>
        /// <returns></returns>
        public static string ObtenerDominioUrl(Uri pUrl, bool pConHTTP)
        {
            string host = pUrl.Host;
            host = UtilDominios.DomainToPunyCode(host);

            if (pConHTTP)
            {
                host = pUrl.Scheme + "://" + host;

                if (host.EndsWith("/"))
                {
                    host = host.Remove(host.Length - 1);
                }
            }
            else
            {
                if (host.StartsWith("www."))
                {
                    host = host.Remove(0, 3);
                }
                else if (!host.StartsWith("."))
                {
                    host = "." + host;
                }
            }

            if(pConHTTP && pUrl.Port != 80 && pUrl.Port != 443)
            {
                host += $":{pUrl.Port}";    
            }

            return host;
        }


        /// <summary>
        /// Obtiene el dominio de una URL (de la manera .gnoss.com)
        /// </summary>
        /// <param name="pUrl">Url de la que se quiere saber su dominio</param>
        /// <param name="pConHTTP">Verdad si se debe añadir http:// al principio del dominio</param>
        /// <returns></returns>
        public static string ObtenerDominioUrl(string pUrl, bool pConHTTP)
        {
            return ObtenerDominioUrl(new Uri(pUrl), pConHTTP);
        }

        ///// <summary>
        ///// Convierte el dominio a su correspondiente punycode (para dominios con ñ o acentos)
        ///// </summary>
        ///// <param name="pDominio">Dominio que se quiere transformar</param>
        ///// <returns></returns>
        public static string DomainToPunyCode(string pDominio)
        {
            //TODO
            //    if (!string.IsNullOrEmpty(pDominio))
            //    {
            //        try
            //        {
            //            string inicioDominio = "";
            //            string finDominio = "";

            //            if (pDominio.Contains("http://"))
            //            {
            //                inicioDominio = "http://";
            //                pDominio = pDominio.Replace("http://", "");
            //            }
            //            else if (pDominio.Contains("https://"))
            //            {
            //                inicioDominio = "https://";
            //                pDominio = pDominio.Replace("https://", "");
            //            }

            //            if (pDominio.Contains(":"))
            //            {
            //                finDominio = pDominio.Substring(pDominio.IndexOf(':'));
            //                pDominio = pDominio.Remove(pDominio.IndexOf(':'));
            //            }

            //            if (pDominio.Contains("/"))
            //            {
            //                finDominio = pDominio.Substring(pDominio.IndexOf('/'));
            //                pDominio = pDominio.Remove(pDominio.IndexOf('/'));
            //            }

            //            string dominio = IDNLib.Encode(pDominio);
            //            dominio = inicioDominio + dominio + finDominio;

            //            return dominio;
            //        }
            //        catch (Exception)
            //        {

            //        }
            //    }

            return pDominio;
        }

    /// <summary>
    /// Genera un dictionary de clave-valor con los parámetros de la url (?pagina=1&tag=web)
    /// </summary>
    /// <param name="pQueryString">QueryString de una peticion web</param>
    /// <returns>Un diccionario de la manera: pagina->1, tag->web</returns>
    public static Dictionary<string, string> QueryStringToDictionary(string pQueryString)
        {
            Dictionary<string, string> hashQuery = new Dictionary<string, string>();
            char[] separadorParametros = { '&' };
            string[] parametros = pQueryString.Split(separadorParametros, StringSplitOptions.RemoveEmptyEntries);

            foreach (string parametro in parametros)
            {
                char[] separadorValor = { '=' };
                string[] claveValor = parametro.Split(separadorValor, StringSplitOptions.RemoveEmptyEntries);

                string valor = "";
                if (claveValor.Length > 1)
                {
                    valor = claveValor[1];
                }

                if (claveValor.Length > 2)
                {
                    //Añado el resto del parámetro que se ha separado
                    for (int i = 2; i < claveValor.Length; i++)
                    {
                        valor += "=" + claveValor[i];
                    }
                }

                hashQuery.Add(claveValor[0], valor);
            }
            return hashQuery;
        }

        /// <summary>
        /// Redirecciona el response a sí misma
        /// </summary>
        /// <param name="pCambiarHttps">Verdad si se debe cambiar http por https</param>
        public static string CambiarHttpAHttpsEnUrl(string pUrl)
        {
            return "https" + pUrl.Remove(0, 4);
        }

        /// <summary>
        /// Redirecciona el response a sí misma
        /// </summary>
        /// <param name="pCambiarHttps">Verdad si se debe cambiar http por https</param>
        public static string CambiarHttpsAHttpEnUrl(string pUrl)
        {
            return "http" + pUrl.Remove(0, 5);
        }
    }
}
