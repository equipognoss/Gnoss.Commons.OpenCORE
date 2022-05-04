using Es.Riam.Gnoss.Util.Configuracion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallEtiquetadoAutomaticoService
    {
        private ConfigService mConfigService;
        private string mServicioEtiquetadoUrl;

        public CallEtiquetadoAutomaticoService(ConfigService configService)
        {
            mConfigService = configService;
            mServicioEtiquetadoUrl = mConfigService.ObtenerUrlServicio("etiquetadoAutomatico");
            if (mServicioEtiquetadoUrl.StartsWith("https://"))
            {
                mServicioEtiquetadoUrl = mServicioEtiquetadoUrl.Replace("https://", "http://");
            }
        }

        public string SeleccionarEtiquetasDesdeServicio(string titulo, string descripcion, string proyectoID)
        {
            string result = CallWebMethods.CallGetApi(mServicioEtiquetadoUrl, $"SeleccionarEtiquetasDesdeServicio?titulo={titulo}&descripcion={descripcion}&ProyectoID={proyectoID}");
            string buffer = JsonConvert.DeserializeObject<string>(result);
            return buffer;
        }
    }
}
