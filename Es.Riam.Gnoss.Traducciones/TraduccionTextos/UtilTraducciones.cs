using Es.Riam.Gnoss.AD.EntityModel.Models.Traductor;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.Administracion;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
	public static class UtilTraducciones
	{
        public const string PROCESO_TRADUCCION = "Traducir";        

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

        public static Dictionary<string, string> ObtenerIdiomasBaseParaTraducir(Guid pProyectoID, ParametroAplicacionCN pParametroAplicacionCN, ProyectoCN pProyectoCN, UtilIdiomas pUtilIdiomas, LoggingService pLoggingService, ILogger pLogger)
        {
            List<string> idiomasComunidad = pParametroAplicacionCN.ObtenerListaIdiomasDictionary().Keys.ToList();

            List<string> idiomasPlataforma = ControladorOpcionesAvanzadas.ObtenerIdiomasPlataforma();

            List<string> idiomasDisponiblesComunidad = idiomasComunidad.Where(x => idiomasPlataforma.Contains(x)).ToList();

            List<string> idiomasTraductor = ObtenerIdiomasDisponiblesTraductor(pProyectoCN.ObtenerTraductorDeProyecto(pProyectoID), pUtilIdiomas, pLoggingService, pLogger);

            return idiomasDisponiblesComunidad.Where(x => idiomasTraductor.Contains(x)).ToDictionary(kvp => kvp, kvp => pUtilIdiomas.GetText("COMMON", $"IDIOMA{kvp.ToUpper()}"));
        }

        public static List<string> ObtenerIdiomasDisponiblesTraductor(TraductorProyecto pTraductor, UtilIdiomas pUtilIdiomas, LoggingService pLoggingService, ILogger pLogger)
        {
            if (pTraductor != null)
            {
				TranslationConfig config = UtilTraducciones.CrearTranslationConfig(pTraductor.Endpoint, pTraductor.Token);
				ITranslationStrategy strategy = new TranslationStrategyFactory().CreateTranslationStrategy(config, TranslationProvider.Scia);
				TranslationService service = new TranslationService(strategy);

				LanguagesResponse response = service.GetAvailableLanguages();

				if (!response.Success)
				{
					pLoggingService.GuardarLogError(response.ErrorMessage, pLogger);
					throw new ArgumentException($"{pUtilIdiomas.GetText("DEVTOOLS", "ERROROBTENERIDIOMAS")}: {response.ErrorMessage}");
				}

				return response.AvailableLanguajes;
			}

            return new List<string>();
        }
    }
}
