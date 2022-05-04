using Es.Riam.Gnoss.Util.Configuracion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Guid id = JsonConvert.DeserializeObject<Guid>(result);
            return id;
        }
    }
}
