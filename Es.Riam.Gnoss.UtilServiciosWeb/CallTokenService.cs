using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
using System;
using System.Net.Http;
using System.Text.Json;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallTokenService
    {
        private static readonly HttpClient mClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        });

        private ConfigService mConfigService;

        public CallTokenService(ConfigService configService)
        {
            mConfigService = configService;
        }

        /// <summary>
        /// Obtiene un token de seguridad de acceso para el Api de uris
        /// </summary>
        /// <returns>Token bearer</returns>
        public TokenBearer CallTokenApi()
        {
            if(string.IsNullOrEmpty(mConfigService.ObtenerClientIDIdentity()) || string.IsNullOrEmpty(mConfigService.ObtenerClientSecretIDIdentity()) || string.IsNullOrEmpty(mConfigService.ObtenerScopeIdentity()))
            {
                throw new Exception("Es necesario configurar los parametro para el Identity Provider");
            }
            string stringData = $"grant_type=client_credentials&scope={mConfigService.ObtenerScopeIdentity()}&client_id={mConfigService.ObtenerClientIDIdentity()}&client_secret={mConfigService.ObtenerClientSecretIDIdentity()}";
            return CallTokenIdentity(stringData);
        }


        /// <summary>
        /// Llama al api de gestión de tokens
        /// </summary>
        /// <param name="stringData">cadena con la información de configuración de los tokens de un api;"grant_type={grantType}&scope={scope del api}&client_id={ClienteId del Api}&client_secret={contraseña del api}"</param>
        /// <returns>token bearer</returns>
        private TokenBearer CallTokenIdentity(string stringData)
        {
            var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
            HttpResponseMessage response = null;
            try
            {
                string authority = mConfigService.GetAuthority() + "/connect/token";
                response = mClient.PostAsync($"{authority}", contentData).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                TokenBearer token = JsonSerializer.Deserialize<TokenBearer>(result);
                return token;
            }
            catch (HttpRequestException)
            {
                if (response != null && !string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                {
                    throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                }
                else if (response != null)
                {
                    throw new HttpRequestException(response.ReasonPhrase);
                }
                else
                {
                    throw new HttpRequestException();
                }
            }
        }

    }
}
