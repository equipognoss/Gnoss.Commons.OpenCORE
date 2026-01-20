using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpen.Model;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using VDS.RDF.Parsing.Tokens;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    internal class SciaTranslationStrategy : ITranslationStrategy
    {
        private string mApiKey;
        private string mRegion;
        private string mProvider;
        private string mEndPoint;
        private static readonly HttpClient mHttpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) };

        public SciaTranslationStrategy(TranslationConfig pTranslationConfig)
        {
            mApiKey = pTranslationConfig.ApiKey;
            mRegion = pTranslationConfig.Region;
            mProvider = "SCIA service";
            mEndPoint = pTranslationConfig.EndPoint;
            mHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", mApiKey);
        }

        public LanguagesResponse GetAvailableLanguages()
        {
            LanguagesResponse languagesResponse = new LanguagesResponse();
            try
            {
                languagesResponse.Provider = mProvider;
                string endpoint = $"{mEndPoint}/api/Gateway/Translate/languages";
                HttpResponseMessage response = mHttpClient.GetAsync(endpoint).Result;                
                languagesResponse.Status = (short)response.StatusCode;

                switch (response.StatusCode)
                {

                    case HttpStatusCode.OK:
                        Dictionary<string, string> languages = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content.ReadAsStringAsync().Result);
                        List<string> translateLanguages = languages.Keys.ToList();
                        languagesResponse.AvailableLanguajes = translateLanguages;
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
            TranslationResponse translationResponse = new TranslationResponse();
            translationResponse.Provider = mProvider;

            try
            {
                
                string endpoint = $"{mEndPoint}/api/Gateway/Translate/translate";
                string requestParameters = JsonConvert.SerializeObject(pTranslationRequest);
                byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
                HttpResponseMessage response = mHttpClient.PostAsJsonAsync(endpoint,pTranslationRequest).Result;

                translationResponse.Status = (short)response.StatusCode;

                switch (response.StatusCode)
                {
                   
                    case HttpStatusCode.OK:
                        SciaTranslateResponse sciaResponse = JsonConvert.DeserializeObject<SciaTranslateResponse>(response.Content.ReadAsStringAsync().Result);                      
                        translationResponse.TranslatedText = sciaResponse.TextTranslate;
                        break;
                    case HttpStatusCode.BadRequest:
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.NotFound:
                    case HttpStatusCode.MethodNotAllowed:
                        try
                        {
                            SciaTranslateErrorResponse sciaTranslateErrorResponse = JsonConvert.DeserializeObject<SciaTranslateErrorResponse>(response.Content.ReadAsStringAsync().Result);
                            translationResponse.ErrorMessage = sciaTranslateErrorResponse.Message;
                        }
                        catch (Exception ex) 
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
    }
}
