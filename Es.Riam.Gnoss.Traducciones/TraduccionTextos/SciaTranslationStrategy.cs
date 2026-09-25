using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    internal class SciaTranslationStrategy : ITranslationStrategy
    {
        private readonly string mApiKey;
        private readonly string mRegion;
        private readonly string mProvider;
        private readonly string mEndPoint;
        // Estatico y compartido para no agotar sockets. PooledConnectionLifetime recicla las conexiones
        // en vez de dejarlas fijadas de por vida (mismo patron que CallTokenService).
        private static readonly HttpClient mHttpClient = new HttpClient(new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(5)
        })
        {
            Timeout = TimeSpan.FromMinutes(10)
        };

        public SciaTranslationStrategy(TranslationConfig pTranslationConfig)
        {
            mApiKey = pTranslationConfig.ApiKey;
            mRegion = pTranslationConfig.Region;
            mProvider = "SCIA service";
            mEndPoint = pTranslationConfig.EndPoint;
        }

        public LanguagesResponse GetAvailableLanguages()
        {
            LanguagesResponse languagesResponse = new LanguagesResponse();
            try
            {
                languagesResponse.Provider = mProvider;
                string endpoint = $"{mEndPoint}/api/Gateway/Translate/languages";
                // El token va por peticion, no en DefaultRequestHeaders del cliente estatico compartido.
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", mApiKey);
                using HttpResponseMessage response = mHttpClient.SendAsync(request).Result;
                languagesResponse.Status = (short)response.StatusCode;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        Dictionary<string, string> languages = JsonSerializer.Deserialize<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
                        List<string> translateLanguages = languages.Keys.ToList();
                        languagesResponse.AvailableLanguajes = translateLanguages;
                        break;
                    case HttpStatusCode.TooManyRequests:
                        languagesResponse.Status = (short)HttpStatusCode.TooManyRequests;
                        languagesResponse.ErrorMessage = "Se ha excedido el límite de peticiones configurado en el ApiKey.";
                        languagesResponse.AvailableLanguajes = new List<string>();
                        break;
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.MethodNotAllowed:

                        break;
                    default:
                        languagesResponse.ErrorMessage = "";
                        break;
                }
            }
            catch (Exception)
            {
                languagesResponse.Status = (short)HttpStatusCode.InternalServerError;
                languagesResponse.ErrorMessage = "Error en la petición de idiomas";
                languagesResponse.AvailableLanguajes = new List<string>();
                throw;
            }

            return languagesResponse;
        }

        public TranslationResponse Translate(TranslationRequest pTranslationRequest)
        {
            return TranslateAsync(pTranslationRequest, CancellationToken.None).GetAwaiter().GetResult();
        }

        public async Task<TranslationResponse> TranslateAsync(TranslationRequest pTranslationRequest, CancellationToken pCancellationToken = default)
        {
            TranslationResponse translationResponse = new TranslationResponse();
            translationResponse.Provider = mProvider;

            try
            {
                string endpoint = $"{mEndPoint}/api/Gateway/Translate/translate";
                // El token va por peticion, no en DefaultRequestHeaders del cliente estatico compartido.
                using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = JsonContent.Create(pTranslationRequest)
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", mApiKey);
                using HttpResponseMessage response = await mHttpClient.SendAsync(request, pCancellationToken);

                translationResponse.Status = (short)response.StatusCode;

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        SciaTranslateResponse sciaResponse = JsonSerializer.Deserialize<SciaTranslateResponse>(await response.Content.ReadAsStringAsync(pCancellationToken));
                        translationResponse.TranslatedText = sciaResponse.TextTranslate;
                        break;
                    case HttpStatusCode.TooManyRequests:
                        // Se prioriza el mensaje de SCIA (puede detallar la cuota y cuándo se renueva) y,
                        // si no llega o no es legible, se informa igualmente del motivo real del rechazo.
                        translationResponse.ErrorMessage = await ObtenerMensajeErrorSciaAsync(response, pCancellationToken);
                        if (string.IsNullOrEmpty(translationResponse.ErrorMessage))
                        {
                            translationResponse.ErrorMessage = "Se ha excedido el límite de peticiones configurado en el ApiKey.";
                        }
                        break;
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.MethodNotAllowed:
                        translationResponse.ErrorMessage = await ObtenerMensajeErrorSciaAsync(response, pCancellationToken);
                        if (string.IsNullOrEmpty(translationResponse.ErrorMessage))
                        {
                            translationResponse.ErrorMessage = "Error inesperado";
                        }
                        break;
                    default:
                        translationResponse.ErrorMessage = "";
                        break;
                }
            }
            catch (Exception)
            {
                translationResponse.Status = 500;
                translationResponse.ErrorMessage = "Error inesperado";
                throw;
            }
            return translationResponse;
        }

        /// <summary>
        /// Devuelve el mensaje de error que envía SCIA en el cuerpo de la respuesta, o cadena vacía si
        /// no viene o no se puede deserializar.
        /// </summary>
        private static async Task<string> ObtenerMensajeErrorSciaAsync(HttpResponseMessage pResponse, CancellationToken pCancellationToken)
        {
            try
            {
                SciaTranslateErrorResponse sciaTranslateErrorResponse = JsonSerializer.Deserialize<SciaTranslateErrorResponse>(await pResponse.Content.ReadAsStringAsync(pCancellationToken));
                return sciaTranslateErrorResponse?.Message ?? "";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
