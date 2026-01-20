using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
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
			config.EndPoint = pConfigService.ObtenerUrlServicioTraducciones();
			config.ApiKey = pConfigService.ObtenerTokenUrlServicioTraducciones();

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
	}
}
