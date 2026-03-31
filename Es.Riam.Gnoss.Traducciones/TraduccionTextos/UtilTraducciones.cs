using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
	public static class UtilTraducciones
	{
		public static TranslationConfig CrearTranslationConfig(ConfigService pConfigService)
		{
			TranslationConfig config = new TranslationConfig();
			config.EndPoint = pConfigService.ObtenerHostSCIA();
			config.ApiKey = pConfigService.ObtenerTokenUrlServicioTraducciones();

			return config;
		}

        public static TranslationConfig CrearTranslationConfig(string pEndpoint, string pToken)
        {
            TranslationConfig config = new TranslationConfig();
            config.EndPoint = pEndpoint;
            config.ApiKey = pToken;

            return config;
        }

        public static string ComprobarIdiomasDisponibles(List<string> pIdiomasTraducir, List<string> pIdiomasDisponibles, LoggingService pLoggingService, ILogger pLogger)
		{
			List<string> idiomasNoContenidos = pIdiomasTraducir.Where(item => !pIdiomasDisponibles.Contains(item)).ToList();

			if (idiomasNoContenidos.Count > 0)
			{
				string idiomasNoDisponibles = string.Join(", ", idiomasNoContenidos);

				pLoggingService.GuardarLogError($"Idiomas no disponibles: {idiomasNoDisponibles}", pLogger);
				return idiomasNoDisponibles;
			}

			return "";
		}

        public static TranslationResponse TraducirTexto(string pTexto, string pEndpoint, string pToken, string pPrompt, string pModelo, string pIdiomaOrigen, string pIdiomaDestino)
        {
            TranslationConfig config = CrearTranslationConfig(pEndpoint, pToken);
            ITranslationStrategy strategy = new TranslationStrategyFactory().CreateTranslationStrategy(config, TranslationProvider.Scia);
            TranslationService service = new TranslationService(strategy);
            TranslationRequest request = new TranslationRequest
            {
                Text = pTexto,
                AdditionalInstructions = pPrompt,
                Model = pModelo,
                SourceLanguage = pIdiomaOrigen,
                TargetLanguage = pIdiomaDestino
            };

            return service.ExecuteTranslation(request);
        }
    }
}
