using System;
using System.Text.Json;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallParametroConfiguracionService
    {
        public string Url { get; set; }

        public CallParametroConfiguracionService()
        {

        }

        public Guid ProyectoIDPorNombreCorto(string pNombreCorto)
        {
            string result = CallWebMethods.CallGetApi(Url, $"ProyectoIDPorNombreCorto?pNombreCorto={pNombreCorto}");
            Guid guidResult = JsonSerializer.Deserialize<Guid>(result);

            return guidResult;
        }

        public string UrlPropiaProyecto(Guid pProyectoID)
        {
            string result = CallWebMethods.CallGetApi(Url, $"UrlPropiaProyecto?pProyectoID={pProyectoID}");
            string resultParser = JsonSerializer.Deserialize<string>(result);

            return resultParser;
        }

        public string UrlServicioResultados()
        {
            string result = CallWebMethods.CallGetApi(Url, $"UrlServicioResultados");
            string resultParser = JsonSerializer.Deserialize<string>(result);

            return resultParser;
        }
    }
}
