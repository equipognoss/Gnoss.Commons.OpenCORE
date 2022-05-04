using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Es.Riam.Util
{
    public class UtilTelemetry
    {
        UtilPeticion _utilPeticion;
        public UtilTelemetry(UtilPeticion utilPeticion)
        {
            _utilPeticion = utilPeticion;
        }

        #region Enumeraciones
        /// <summary>
        /// Ubicación de la información almacenada por logs y trazas
        /// </summary>
        public enum UbicacionLogsYTrazas
        {
            /// <summary>
            /// Indica que la información de logs y trazas se guardará en un archivo
            /// </summary>
            Archivo = 0,
            /// <summary>
            /// Indica que se enviará la información de logs y trazas al entorno de ApplicationInsights configurado
            /// </summary>
            ApplicationInsights = 1,
            /// <summary>
            /// Indica que se guardarán los logs y trazas en un archivo y enviará su información al entorno de ApplicationInsights configurado
            /// </summary>
            ArchivoYAppInsights = 2
        }
        #endregion

        #region Métodos Públicos
        public TelemetryClient Telemetry
        {
            get
            {
                TelemetryClient telemetryClient = null;
                if (_utilPeticion.ExisteObjetoDePeticion("TelemetryClient"))
                {
                    telemetryClient = (TelemetryClient)_utilPeticion.ObtenerObjetoDePeticion("TelemetryClient");
                }
                else if (!string.IsNullOrEmpty(Microsoft.ApplicationInsights.Extensibility.
        TelemetryConfiguration.Active.InstrumentationKey))
                {
                    telemetryClient = new TelemetryClient();
                    //TODO: la versión deberíamos obtenerla de otro ensamblado (algún AD pej.)
                    telemetryClient.Context.Component.Version = typeof(Es.Riam.Util.UtilTelemetry).Assembly.GetName().Version.ToString();
                    _utilPeticion.AgregarObjetoAPeticionActual("TelemetryClient", telemetryClient);
                }
                return telemetryClient;
            }
        }

        //enviar telemetria -> método que compruebe si está configurada la telemetría y que la envíe usando la propiedad Telemetry
        public void EnviarTelemetriaEvento(string pNombreEvento, Dictionary<string, string> pPropiedades, Dictionary<string, double> pMetricas)
        {
            if (EstaConfiguradaTelemetria)
            {
                if (EstaConfiguradaTelemetria)
                {
                    Telemetry.TrackEvent(pNombreEvento, pPropiedades, pMetricas);
                }
            }
        }

        public void EnviarTelemetriaTraza(string pMensajeTraza, string pNombreDependencia = null, Stopwatch pReloj = null, bool pExito = false)
        {
            if (EstaConfiguradaTelemetria)
            {
                DateTime horaActual = DateTime.Now;
                TimeSpan duracion = new TimeSpan();

                if (pReloj != null)
                {
                    duracion = pReloj.Elapsed;
                }

                if (string.IsNullOrEmpty(pNombreDependencia))
                {
                    Telemetry.TrackTrace(pMensajeTraza, SeverityLevel.Information);
                }
                else
                {
                    Telemetry.TrackDependency(pNombreDependencia, pMensajeTraza, horaActual, duracion, pExito);
                }
            }
        }

        public void EnviarTelemetriaExcepcion(Exception pExcepcion, string pMensajeExtra, bool pErrorCritico = false)
        {
            if (EstaConfiguradaTelemetria)
            {
                Telemetry.TrackException(pExcepcion);
                SeverityLevel nivelError = SeverityLevel.Information;
                if (pErrorCritico)
                {
                    nivelError = SeverityLevel.Critical;
                }
                Telemetry.TrackTrace(pMensajeExtra, nivelError);
            }
        }

        #endregion

        #region Propiedades
        /// <summary>
        /// Indica si está configurado el envío de métricas con Application Insights
        /// </summary>
        /// <returns></returns>
        public static bool EstaConfiguradaTelemetria
        {
            get
            {

                return !string.IsNullOrEmpty(UtilTelemetry.InstrumentationKey);
            }
        }

        public static bool ModoDepuracion
        {
            get
            {
                if (TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode.HasValue)
                {
                    return TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode.Value;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = value;
            }
        }

        public static bool EstaActiva
        {
            get
            {
                return TelemetryConfiguration.Active.DisableTelemetry;
            }
            set
            {
                TelemetryConfiguration.Active.DisableTelemetry = value;
            }
        }

        public static string InstrumentationKey
        {
            get
            {
                return Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey;
            }
        }

        #endregion
    }
}
