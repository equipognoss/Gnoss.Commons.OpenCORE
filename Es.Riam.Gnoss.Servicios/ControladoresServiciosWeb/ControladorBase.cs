using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb
{
    public abstract class ControladorBase
    {
        public string PeticionServicio(string pMetodo, Dictionary<string, string> pParametros, HttpRequest pRequest = null)
        {
            string url = $"{Url}/{pMetodo}";

            string parametros = "";
            if (pParametros != null)
            {
                foreach (string claveParametro in pParametros.Keys)
                {
                    if (!string.IsNullOrEmpty(parametros))
                    {
                        parametros += "&";
                    }
                    parametros += $"{claveParametro}={pParametros[claveParametro]}";
                }
            }
            string respuesta = UtilWeb.WebRequest(UtilWeb.Metodo.POST, url, parametros, pRequest);
            return respuesta;
        }

        public string Url { get; set; }
    }
}
