using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Guid guidResult = JsonConvert.DeserializeObject<Guid>(result);

            return guidResult;
        }

        public string UrlPropiaProyecto(Guid pProyectoID)
        {
            string result = CallWebMethods.CallGetApi(Url, $"UrlPropiaProyecto?pProyectoID={pProyectoID}");
            string resultParser = JsonConvert.DeserializeObject<string>(result);

            return resultParser;
        }

        public string UrlServicioResultados()
        {
            string result = CallWebMethods.CallGetApi(Url, $"UrlServicioResultados");
            string resultParser = JsonConvert.DeserializeObject<string>(result);

            return resultParser;
        }
    }
}
