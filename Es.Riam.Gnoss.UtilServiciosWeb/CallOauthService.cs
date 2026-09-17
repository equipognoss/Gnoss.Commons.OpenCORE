using Es.Riam.Gnoss.Util.Configuracion;
using System.Text.Json;
using System;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallOauthService
    {
        private ConfigService mConfigService;
        private string mUrlOauthService;

        public CallOauthService(ConfigService configService)
        {
            mConfigService = configService;
            mUrlOauthService = mConfigService.ObtenerUrlServicio("urlOauth");
            if (mUrlOauthService.StartsWith("https://"))
            {
                mUrlOauthService = mUrlOauthService.Replace("https://", "http://");
            }
        }

        public Guid ObtenerUsuarioAPartirDeUrl(string pUrl, string pMetodoHttp)
        {
            string result = CallWebMethods.CallGetApi(mUrlOauthService, $"ServicioOauth/ObtenerUsuarioAPartirDeUrl?pUrl={pUrl}&pMetodoHttp={pMetodoHttp}");
            Guid id = JsonSerializer.Deserialize<Guid>(result);
            return id;
        }
    }
}
