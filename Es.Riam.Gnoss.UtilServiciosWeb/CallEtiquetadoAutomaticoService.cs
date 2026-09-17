using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Util;

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
        }

        public string SeleccionarEtiquetasDesdeServicio(string titulo, string descripcion, string proyectoID)
        {
            return UtilWeb.WebRequestStringData(UtilWeb.Metodo.POST, $"{mServicioEtiquetadoUrl}/SeleccionarEtiquetasDesdeServicio", $"titulo={titulo}&descripcion={descripcion}&ProyectoID={proyectoID}");
        }
    }
}
