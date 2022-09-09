using Es.Riam.Util;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Util.Configuracion
{
    [Serializable]
    public class ConfigService
    {
        private IConfigurationRoot Configuration { get; set; }
        private IDictionary EnvironmentVariables { get; set; }
        private string sqlConnectionString { get; set; }
        private string virtuosoConnectionString;
        private string virtuosoConnectionStringHome;
        private string tipoBD;
        private string timeoutVirtuoso;
        private string azure;
        private string baseConnectionString;
        private string hAPoxyVirtuosoConnectionString;
        private string usarHilosInteractuarRedis;
        private int numeroPersonasEnvioNewsletter;
        private string usarCache;
        private string numeroPersonasEnvioNewsletterString;
        private string azureServiceBusReintentos;
        private string azureServiceBusEspera;
        private string urlBase;
        private int mTiempocapturaurl;
        private bool? jsyCssUnificado;
        private bool? peticionhttps;
        private bool? sitioComunidadesPorDefecto;
        private bool? captchaActive;
        private Guid? proyectoConexion;
        private Guid? organizacionConexion;
        private Guid? proyectoGnoss;
        private Guid? organizacionGnoss;
        private string urlLogin;
        private string urlFacetas;
        private string urlFacetasExterno;
        private string urlResultados;
        private string urlResultadosExterno;
        private string ubicacionIndiceLucene;
        private List<string> listaIdiomas;
        private Dictionary<string, string> listaIdiomasDictionary;
        private string version;
        private Guid? proyectoPrincipal;
        private string ipServiciosSockets;
        private int puertoServiciosSockets;
        private string dominio;
        private string claveApiGoogle;
        private string urlApiDesplieguesEntornoSiguiente;
        private string urlApiDesplieguesEntornoParametro;
        private string urlApiDesplieguesEntornoAnterior;
        private string urlApiDesplieguesEntorno;
        private string urlApiIntegracionContinua;
        private string urlIntraGnoss;
        private bool viewsAdministracion;
        private string urlApiLucene;
        private string urlEtiquetadoInteligente;
        private string ignorarVistasPersonalizadas;
        private string urlContent;
        private string logStahsConnection;
        private string implementationKeyAutocompletar;
        private string implementationKeyFacetas;
        private string implementationKeyResultados;
        private string implementationKeyWeb;
        private string implementationKeyAutocompletarEtiquetas;
        private string ubicacionLogsResultados;
        private string ubicacionTrazasAutocompletar;
        private string ubicacionLogsAutocompletar;
        private string ubicacionLogsAutocompletarEtiquetas;
        private string ubicacionTrazasResultados;
        private string ubicacionLogsFacetas;
        private string ubicacionTrazasAutocompletarEtiquetas;
        private string ubicacionTrazasFacetas;
        private string ubicacionTrazasWeb;
        private string ubicacionTrazasLogin;
        private string ubicacionLogsLogin;
        private string ipServicioTrazasUDP;
        private string implementationKeyLogin;
        private string ubicacionTrazasApiV3;
        private string ubicacionLogsApiV3;
        private string implementationKeyApiV3;
        private string proyectoID;
        private string oauth;
        private string puertoServicioTrazasUDP;
        private int captchaNumIntentos;
        private int horasBorrado;
        private int intervalo;
        private string colalive;
        private string colabase;
        private string tiempos;
        private string trazas;
        private string guardarDatosPeticion;
        private string rutaBaseTriples;
        private string urlTriples;
        private string emailErrores;
        private int hilosAplicacion;
        private string tokenJenkins;
        private string connectionJenkins;
        private int horaEnvioErrores;
        private bool? replicacionActivada;
        private bool? replicacionActivadaHome;
        private bool? procesarStringGrafo;
        private bool? show500Error;
        private bool? escribirFicheroExternoTriples;
        private bool? cacheV4;
        private bool? configuradoEtiquetadoInteligente;
        private string urlDespligues;
        private string robots;
        private string urlDocuments;
        private string urlServicioEtiquetas;
        private string urlIntern;
        private string paginaPresentacion;
        private string keySession;
        private string urlServicioBrightcove;
        private string Authority;
        private string urlServicioTOP;
        private int mNumVisitasHilo = 100;
        private int mNumHilosAbiertos = 5;
        private int mMinutosAntesProcesar = 5;
        private int mHorasProcesarVisitasVirtuoso = 6;
        private int mVisitasVotosComentarios = 5;
        private int mNumMaxPeticionesWebSimultaneas = 5;
        private int mMinutosAgruparRegistrosUsuariosEnProyecto = 60;
        private string idiomaDefecto;
        private int mPuertoUDP;
        private int mIntervaloVVC;
        string ruta;
        private string vapidPublicKey;
        private string vapidPrivateKey;
        private string vapidSubject;
        private string puertoVirtuoso;
        private string puertoVirtuosoAux;
        private bool? trazasHabilitadas;
        private string azureStorageConnectionString;
        private string logstashEndpoint;
        private string implementationKey;
        private string logLocation;
        private string rutaOntologias;
        private string rutaMapping;
        private string cadenaConexion;
        private string cadenaConexionVirtuoso;
        private string cadenaConexionAzureStorage;
        private string errorRoute;
        private string rutaOnto;
        private string ipServidorFTP;
        private int minPort = 0;
        private int maxPort = 0;

        public string GetCadenaConexion()
        {
            if (string.IsNullOrEmpty(cadenaConexion))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("CadenaConexion"))
                {
                    cadenaConexion = environmentVariables["CadenaConexion"] as string;
                }
                else
                {
                    cadenaConexion = Configuration.GetConnectionString("CadenaConexion");
                }
            }
            return cadenaConexion;
        }


        public string ErrorRoute
        {
            get
            {
                return errorRoute;
            }
            set
            {
                if (string.IsNullOrEmpty(errorRoute))
                {
                    errorRoute = value;
                }
            }
        }

        public string RutaOnto
        {
            get
            {
                return rutaOnto;
            }
            set
            {
                if (string.IsNullOrEmpty(rutaOnto))
                {
                    rutaOnto = value;
                }
            }
        }

        public string GetRutaOntologias()
        {
            if (string.IsNullOrEmpty(rutaOntologias))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("rutaOntologias"))
                {
                    rutaOntologias = environmentVariables["rutaOntologias"] as string;
                }
                else
                {
                    rutaOntologias = Configuration["rutaOntologias"];
                }

            }
            return rutaOntologias;
        }

        public string RutaMapping
        {
            get
            {
                if (string.IsNullOrEmpty(rutaMapping))
                {
                    IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                    if (environmentVariables.Contains("rutaMapping"))
                    {
                        rutaMapping = environmentVariables["rutaMapping"] as string;
                    }
                    else
                    {
                        rutaMapping = Configuration["rutaMapping"];
                    }
                }
                return rutaMapping;
            }
            set
            {
                rutaMapping = value;
            }
        }

        public string GetApplicationImplementationKey()
        {
            if (string.IsNullOrEmpty(implementationKey))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("applicationInsights:implementationKey"))
                {
                    implementationKey = environmentVariables["applicationInsights:implementationKey"] as string;
                }
                else
                {
                    implementationKey = Configuration["applicationInsights:implementationKey"];
                }
            }
            return implementationKey;
        }

        public string GetApplicationLogLocation()
        {
            if (string.IsNullOrEmpty(logLocation))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("applicationInsights__logLocation"))
                {
                    logLocation = environmentVariables["applicationInsights__logLocation"] as string;
                }
                else
                {
                    logLocation = Configuration["applicationInsights:logLocation"];
                }
            }
            return logLocation;
        }

        public string GetLogstashEndpoint()
        {
            if (string.IsNullOrEmpty(logstashEndpoint))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("logstashEndpoint"))
                {
                    logstashEndpoint = environmentVariables["logstashEndpoint"] as string;
                }
                else
                {
                    logstashEndpoint = Configuration["logstash:logstashEndpoint"];
                }
            }
            return logstashEndpoint;
        }


        public string GetCadenaConexionVirtuoso()
        {
            if (string.IsNullOrEmpty(cadenaConexionVirtuoso))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("cadenaConexionVirtuoso"))
                {
                    cadenaConexionVirtuoso = environmentVariables["cadenaConexionVirtuoso"] as string;
                }
                else
                {
                    cadenaConexionVirtuoso = Configuration.GetConnectionString("cadenaConexionVirtuoso");
                }
            }
            return cadenaConexionVirtuoso;
        }

        public string GetCadenaConexionAzureStorage()
        {
            if (string.IsNullOrEmpty(cadenaConexionAzureStorage))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("AzureStorageConnectionString"))
                {
                    cadenaConexionAzureStorage = environmentVariables["AzureStorageConnectionString"] as string;
                }
                else
                {
                    cadenaConexionAzureStorage = Configuration.GetConnectionString("AzureStorageConnectionString");
                }
            }
            return cadenaConexionAzureStorage;
        }
        public ConfigService()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json");
            EnvironmentVariables = Environment.GetEnvironmentVariables();
            Configuration = builder.Build();
        }

        public string GetAzureStorageConnectionString()
        {
            if (string.IsNullOrEmpty(azureStorageConnectionString))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("AzureStorageConnectionString"))
                {
                    azureStorageConnectionString = environmentVariables["AzureStorageConnectionString"] as string;
                }
                else
                {
                    azureStorageConnectionString = Configuration["AzureStorageConnectionString"];
                }
            }
            return azureStorageConnectionString;
        }

        
        public string GetImplementationKey()
        {
            if (string.IsNullOrEmpty(implementationKey))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("ImplementationKey"))
                {
                    implementationKey = environmentVariables["ImplementationKey"] as string;
                }
                else
                {
                    implementationKey = Configuration["ImplementationKey"];
                }
            }
            return implementationKey;
        }

        public string GetLogLocation()
        {
            if (string.IsNullOrEmpty(logLocation))
            {
                IDictionary environmentVariables = Environment.GetEnvironmentVariables();
                if (environmentVariables.Contains("LogLocation"))
                {
                    logLocation = environmentVariables["LogLocation"] as string;
                }
                else
                {
                    logLocation = Configuration["LogLocation"];
                }
            }
            return logLocation;
        }

        public string ObtenerAzureServiceBusReintentos()
        {
            if (string.IsNullOrEmpty(azureServiceBusReintentos))
            {
                if (EnvironmentVariables.Contains("azureServiceBusReintentos"))
                {
                    azureServiceBusReintentos = EnvironmentVariables["azureServiceBusReintentos"] as string;
                }
                else
                {
                    azureServiceBusReintentos = Configuration["azureServiceBusReintentos"];
                }
            }
            return azureServiceBusReintentos;
        }

        public bool ObtenerShow500Error()
        {

            if (show500Error == null)
            {
                if (EnvironmentVariables.Contains("show500Error"))
                {
                    string variable = EnvironmentVariables["show500Error"] as string;
                    if (variable.ToLower() == "true")
                    {
                        show500Error = true;
                    }
                }
                else
                {
                    show500Error = Configuration.GetValue<bool?>("show500Error");
                }
            }
            if (!show500Error.HasValue)
            {
                return false;
            }
            return show500Error.Value;

        }

        public Guid? ObtenerOrganizacionConexion()
        {
            if (organizacionConexion == null)
            {
                if (EnvironmentVariables.Contains("OrganizacionConexion"))
                {
                    string variable = EnvironmentVariables["OrganizacionConexion"] as string;
                    Guid devolver = Guid.Empty;
                    if (Guid.TryParse(variable, out devolver))
                    {
                        organizacionConexion = devolver;
                    }
                }
                else
                {
                    organizacionConexion = Configuration.GetValue<Guid?>("OrganizacionConexion");
                }
            }
            return organizacionConexion;
        }
        public string ObtenerIdiomaDefecto()
        {
            if (string.IsNullOrEmpty(idiomaDefecto))
            {
                if (EnvironmentVariables.Contains("IdiomaDefecto"))
                {
                    idiomaDefecto = EnvironmentVariables["IdiomaDefecto"] as string;
                }
                else
                {
                    idiomaDefecto = Configuration["IdiomaDefecto"];
                }
            }
            return idiomaDefecto;
        }

        public string ObtenerUrlIntraGnoss()
        {
            if (string.IsNullOrEmpty(urlIntraGnoss))
            {
                if (EnvironmentVariables.Contains("UrlIntraGnoss"))
                {
                    urlIntraGnoss = EnvironmentVariables["UrlIntraGnoss"] as string;
                }
                else
                {
                    urlIntraGnoss = Configuration["UrlIntraGnoss"];
                }
            }
            return urlIntraGnoss;
        }

        public bool EstaDesplegadoEnDocker()
        {
            if (viewsAdministracion == false)
            {
                if (EnvironmentVariables.Contains("DesplegadoDocker"))
                {
                    viewsAdministracion = bool.Parse(EnvironmentVariables["DesplegadoDocker"] as string);
                }
                else if(!string.IsNullOrEmpty(Configuration["DesplegadoDocker"]))
                {
                    viewsAdministracion = bool.Parse(Configuration["DesplegadoDocker"]);
                }
            }

            return viewsAdministracion;
        }

        public string ObtenerAzureServiceBusEspera()
        {
            if (string.IsNullOrEmpty(azureServiceBusEspera))
            {
                if (EnvironmentVariables.Contains("azureServiceBusEspera"))
                {
                    azureServiceBusEspera = EnvironmentVariables["azureServiceBusEspera"] as string;
                }
                else
                {
                    azureServiceBusEspera = Configuration["azureServiceBusEspera"];
                }
            }
            return azureServiceBusEspera;
        }

        public int ObtenerNumeroPersonasEnvioNewsletter()
        {
            if (numeroPersonasEnvioNewsletter == 0)
            {
                if (EnvironmentVariables.Contains("numeroPersonasEnvioNewsletter"))
                {
                    numeroPersonasEnvioNewsletterString = EnvironmentVariables["numeroPersonasEnvioNewsletter"] as string;
                }
                else
                {
                    numeroPersonasEnvioNewsletterString = Configuration["numeroPersonasEnvioNewsletter"];
                }
            }
            int numeroPersonasEnvioNewsletterOut;
            Int32.TryParse(numeroPersonasEnvioNewsletterString, out numeroPersonasEnvioNewsletterOut);
            numeroPersonasEnvioNewsletter = numeroPersonasEnvioNewsletterOut;
            return numeroPersonasEnvioNewsletter;
        }

        public string ObtenerUsarCache()
        {
            if (string.IsNullOrEmpty(usarCache))
            {
                if (EnvironmentVariables.Contains("usarCache"))
                {
                    usarCache = EnvironmentVariables["usarCache"] as string;
                }
                else
                {
                    usarCache = Configuration["usarCache"];
                }
            }
            return usarCache;
        }

        public int ObtenerCaptchaNumIntentos()
        {
            string captchaNumIntentosString = "";
            if (captchaNumIntentos == 0)
            {
                if (EnvironmentVariables.Contains("captcha__numIntentos"))
                {
                    captchaNumIntentosString = EnvironmentVariables["captcha__numIntentos"] as string;
                }
                else
                {
                    captchaNumIntentosString = Configuration.GetSection("captcha")["numIntentos"];
                }
            }
            int captchaNumIntentosOut;
            Int32.TryParse(captchaNumIntentosString, out captchaNumIntentosOut);
            captchaNumIntentos = captchaNumIntentosOut;
            return captchaNumIntentos;
        }



        public int ObtenerHorasBorrado()
        {
            string horasBorradoString = "";
            if (horasBorrado == 0)
            {
                if (EnvironmentVariables.Contains("horasBorrado"))
                {
                    horasBorradoString = EnvironmentVariables["horasBorrado"] as string;
                }
                else
                {
                    horasBorradoString = Configuration["horasBorrado"];
                }
            }
            int horasBorradoOut;
            Int32.TryParse(horasBorradoString, out horasBorradoOut);
            horasBorrado = horasBorradoOut;
            return horasBorrado;
        }

        public int ObtenerIntervalo()
        {
            string intervaloString = "";
            if (intervalo == 0)
            {
                if (EnvironmentVariables.Contains("intervalo"))
                {
                    intervaloString = EnvironmentVariables["intervalo"] as string;
                }
                else
                {
                    intervaloString = Configuration["intervalo"];
                }
            }
            int intervaloOut;
            Int32.TryParse(intervaloString, out intervaloOut);
            intervalo = intervaloOut;
            return intervalo;
        }
        public string ObtenerUsarHilosInteractuarRedis()
        {
            if (string.IsNullOrEmpty(usarHilosInteractuarRedis))
            {
                if (EnvironmentVariables.Contains("usarHilosInteractuarRedis"))
                {
                    usarHilosInteractuarRedis = EnvironmentVariables["usarHilosInteractuarRedis"] as string;
                }
                else
                {
                    usarHilosInteractuarRedis = Configuration["usarHilosInteractuarRedis"];
                }
            }
            return usarHilosInteractuarRedis;
        }

        public string ObtenerRabbitMQClient(string tipoCola)
        {
            string cadena = "";
            if (EnvironmentVariables.Contains($"RabbitMQ__{tipoCola}"))
            {
                cadena = EnvironmentVariables[$"RabbitMQ__{tipoCola}"] as string;
            }
            else
            {
                cadena = Configuration.GetSection("RabbitMQ")[tipoCola];
            }

            return cadena;
        }

        public string ObtenerUrlPaginaPresentacion()
        {
            if (string.IsNullOrEmpty(paginaPresentacion))
            {
                if (EnvironmentVariables.Contains("PaginaPresentacion"))
                {
                    paginaPresentacion = EnvironmentVariables["PaginaPresentacion"] as string;
                }
                else
                {
                    paginaPresentacion = Configuration["PaginaPresentacion"];
                }
            }
            return paginaPresentacion;
        }

        public string ObtenerColaBase()
        {
            if (string.IsNullOrEmpty(colabase))
            {
                if (EnvironmentVariables.Contains("colabase"))
                {
                    colabase = EnvironmentVariables["colabase"] as string;
                }
                else
                {
                    colabase = Configuration["colabase"];
                }
            }
            return colabase;
        }

        public string ObtenerImplementationKeyWeb()
        {
            if (string.IsNullOrEmpty(implementationKeyWeb))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyWeb"))
                {
                    implementationKeyWeb = EnvironmentVariables["ImplementationKeyWeb"] as string;
                }
                else
                {
                    implementationKeyWeb = Configuration["ImplementationKeyWeb"];
                }
            }
            return implementationKeyWeb;
        }
        public string ObtenerUbicacionTrazasAutocompletarEtiquetas()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasAutocompletarEtiquetas))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasAutocompletarEtiquetas"))
                {
                    ubicacionTrazasAutocompletarEtiquetas = EnvironmentVariables["UbicacionTrazasAutocompletarEtiquetas"] as string;
                }
                else
                {
                    ubicacionTrazasAutocompletarEtiquetas = Configuration["UbicacionTrazasAutocompletarEtiquetas"];
                }
            }
            return ubicacionTrazasAutocompletarEtiquetas;
        }
        public string ObtenerUbicacionTrazasWeb()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasWeb))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasWeb"))
                {
                    ubicacionTrazasWeb = EnvironmentVariables["UbicacionTrazasWeb"] as string;
                }
                else
                {
                    ubicacionTrazasWeb = Configuration["UbicacionTrazasWeb"];
                }
            }
            return ubicacionTrazasWeb;
        }
        public string ObtenerImplementationKeyAutocompletarEtiquetas()
        {
            if (string.IsNullOrEmpty(implementationKeyAutocompletarEtiquetas))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyAutocompletarEtiquetas"))
                {
                    implementationKeyAutocompletarEtiquetas = EnvironmentVariables["ImplementationKeyAutocompletarEtiquetas"] as string;
                }
                else
                {
                    implementationKeyAutocompletarEtiquetas = Configuration["ImplementationKeyAutocompletarEtiquetas"];
                }
            }
            return implementationKeyAutocompletarEtiquetas;
        }

        public string ObtenerTrazaHabilitada()
        {
            if (string.IsNullOrEmpty(trazas))
            {
                if (EnvironmentVariables.Contains("trazas"))
                {
                    trazas = EnvironmentVariables["trazas"] as string;
                }
                else
                {
                    trazas = Configuration["trazas"];
                }
            }
            return trazas;
        }

        public string ObtenerGuardarDatosPeticion()
        {
            if (string.IsNullOrEmpty(guardarDatosPeticion))
            {
                if (EnvironmentVariables.Contains("guardarDatosPeticion"))
                {
                    guardarDatosPeticion = EnvironmentVariables["guardarDatosPeticion"] as string;
                }
                else
                {
                    guardarDatosPeticion = Configuration["guardarDatosPeticion"];
                }
            }
            return guardarDatosPeticion;
        }

        public string ObtenerTiempos()
        {
            if (string.IsNullOrEmpty(tiempos))
            {
                if (EnvironmentVariables.Contains("tiempos"))
                {
                    tiempos = EnvironmentVariables["tiempos"] as string;
                }
                else
                {
                    tiempos = Configuration["tiempos"];
                }
            }
            return tiempos;
        }

        public string ObtenerLogStashConnection()
        {
            if (string.IsNullOrEmpty(logStahsConnection))
            {
                if (EnvironmentVariables.Contains("logStahsConnection"))
                {
                    logStahsConnection = EnvironmentVariables["logStahsConnection"] as string;
                }
                else
                {
                    logStahsConnection = Configuration.GetConnectionString("HA_logStahsConnectionPROXY");
                }
            }
            return logStahsConnection;
        }
        public string ObtenerVirutosoLectura()
        {
            string cadena = "";
            var rand = new Random();
            if (EnvironmentVariables.Contains($"Virtuoso__NumLectura"))
            {
                int? childs = EnvironmentVariables[$"Virtuoso__NumLectura"] as int?;
                var numRand = rand.Next(0, childs.Value - 1);
                cadena = EnvironmentVariables[$"Virtuoso__Lectura{numRand}"] as string;
            }
            else
            {
                var lectura = Configuration.GetSection("ConnectionStrings").GetSection("Virtuoso").GetSection("Lectura").GetChildren();
                int childs = lectura.Count();

                var numRand = rand.Next(0, childs - 1);
                cadena = lectura.ToList().ElementAt(numRand).Value;
            }

            return cadena;
        }

        public KeyValuePair<string, string> ObtenerVirtuosoEscritura()
        {
            string cadena = "";
            string nombreConexionVirtuoso = "";
            var rand = new Random();
            var virtuososEnvironment = EnvironmentVariables.Keys.Cast<string>().Where(item => item.StartsWith("Virtuoso__Escritura"));
            var virtuososSettings = Configuration.GetSection("ConnectionStrings").GetSection("Virtuoso").GetSection("Escritura").GetChildren();
            if (virtuososEnvironment.Any())
            {
                var numRand = rand.Next(0, virtuososEnvironment.Count() - 1);
                string key = virtuososEnvironment.ElementAt(numRand);
                cadena = EnvironmentVariables[key] as string;
                nombreConexionVirtuoso = key.Replace("Virtuoso__Escritura__", "");
            }
            else if (virtuososSettings.Any())
            {
                var numRand = rand.Next(0, virtuososSettings.ToList().Count - 1);
                cadena = virtuososSettings.ElementAt(numRand).Value;
                nombreConexionVirtuoso = virtuososSettings.ElementAt(numRand).Key;
            }
            else
            {
                cadena = ObtenerVirtuosoConnectionString();
            }

            return new KeyValuePair<string, string>(nombreConexionVirtuoso, cadena);
        }

        public bool CheckBidirectionalReplicationIsActive()
        {
            if (EnvironmentVariables.Keys.Cast<string>().Any(item => item.StartsWith("BidirectionalReplication")))
            {
                return true;
            }
            else
            {
                return Configuration.GetSection("ConnectionStrings").GetSection("BidirectionalReplication").Exists();
            }
        }

        public string ObtenerNombreConexionReplica(string pNombreConexion)
        {
            string replica = "";
            if (EnvironmentVariables.Contains($"BidirectionalReplication__{pNombreConexion}"))
            {
                replica = EnvironmentVariables[$"BidirectionalReplication__{pNombreConexion}"] as string;
            }
            else
            {
                var bidirectionalReplicationSection = Configuration.GetSection("ConnectionStrings").GetSection("BidirectionalReplication");
                if (bidirectionalReplicationSection != null)
                {
                    replica = bidirectionalReplicationSection[pNombreConexion];
                }
            }

            if (string.IsNullOrEmpty(replica) && CheckBidirectionalReplicationIsActive())
            {
                throw new Exception($"La conexión {pNombreConexion} no está configurada en la sección BidirectionalReplication.");
            }

            return replica;
        }

        public List<string> ObtenerListaIdiomas()
        {
            string idiomas = "";
            if (listaIdiomas == null)
            {

                listaIdiomas = ObtenerListaIdiomasDictionary().Keys.ToList();
            }

            return listaIdiomas;
        }

        public Dictionary<string, string> ObtenerListaIdiomasDictionary()
        {
            string idiomas = "";
            List<string> codes = new List<string>();
            if (listaIdiomasDictionary == null)
            {
                listaIdiomasDictionary = new Dictionary<string, string>();
                if (EnvironmentVariables.Contains($"idiomas"))
                {
                    idiomas = EnvironmentVariables[$"idiomas"] as string;
                }
                else
                {
                    idiomas = Configuration["idiomas"];
                }
                codes = idiomas.Split(',').ToList();
                foreach (string idioma in codes)
                {
                    listaIdiomasDictionary.Add(idioma.Split('|')[0], idioma.Split('|')[1]);
                }
            }

            return listaIdiomasDictionary;
        }


        public string ObtenerVirutosoEscritura(string name)
        {
            string cadena = "";
            if (EnvironmentVariables.Contains($"Virtuoso__Escritura__{name}"))
            {
                cadena = EnvironmentVariables[$"Virtuoso__Escritura__{name}"] as string;
            }
            else
            {
                cadena = Configuration.GetSection("ConnectionStrings").GetSection("Virtuoso").GetSection("Escritura")[name];
            }

            return cadena;
        }

        public string ObtenerVirtuosoEscrituraHome()
        {
            string cadena = "";
            if (EnvironmentVariables.Contains($"VirtuosoHome__VirtuosoEscrituraHome"))
            {
                cadena = EnvironmentVariables[$"VirtuosoHome__VirtuosoEscrituraHome"] as string;
            }
            else
            {
                cadena = Configuration.GetSection("ConnectionStrings").GetSection("Virtuosohome")["VirtuosoEscrituraHome"];
            }

            return cadena;
        }

        public string ObtenerVersion()
        {
            if (string.IsNullOrEmpty(version))
            {
                if (EnvironmentVariables.Contains("version"))
                {
                    version = EnvironmentVariables["version"] as string;
                }
                else
                {
                    version = Configuration["version"];
                }
            }
            return version;
        }

        //public string ObtenerHAProxyVirtuosoConnectionString()
        //{
        //    if (string.IsNullOrEmpty(hAPoxyVirtuosoConnectionString))
        //    {
        //        if (EnvironmentVariables.Contains("HA_PROXY"))
        //        {
        //            hAPoxyVirtuosoConnectionString = EnvironmentVariables["HA_PROXY"] as string;
        //        }
        //        else
        //        {
        //            hAPoxyVirtuosoConnectionString = Configuration.GetConnectionString("HA_PROXY");
        //        }
        //    }
        //    return hAPoxyVirtuosoConnectionString;
        //}

        public string ObtenerUrlBase()
        {
            if (string.IsNullOrEmpty(urlBase))
            {
                if (EnvironmentVariables.Contains("Servicios__urlBase"))
                {
                    urlBase = EnvironmentVariables["Servicios__urlBase"] as string;
                }
                else
                {
                    urlBase = Configuration.GetSection("Servicios")["urlBase"];
                }
                if (!string.IsNullOrEmpty(urlBase) && urlBase.EndsWith("/"))
                {
                    urlBase = urlBase.TrimEnd('/');
                }
            }
            return urlBase;
        }

        public string ObtenerUrlApiLucene()
        {
            if (string.IsNullOrEmpty(urlApiLucene))
            {
                if (EnvironmentVariables.Contains("Servicios__urlLucene"))
                {
                    urlApiLucene = EnvironmentVariables["Servicios__urlLucene"] as string;
                }
                else
                {
                    urlApiLucene = Configuration.GetSection("Servicios")["urlLucene"];
                }
                if (!string.IsNullOrEmpty(urlApiLucene) && !urlApiLucene.EndsWith("/"))
                {
                    urlApiLucene = $"{urlApiLucene}/";
                }
            }
            return urlApiLucene;
        }


        public string ObtenerCadenaConexion(string pNombreCadena)
        {
            string cadena = "";
            if (EnvironmentVariables.Contains(pNombreCadena))
            {
                cadena = EnvironmentVariables[pNombreCadena] as string;
            }
            else
            {
                cadena = Configuration.GetConnectionString(pNombreCadena);
            }

            return cadena;
        }

        public string ObtenerTimeoutVirtuoso()
        {
            if (string.IsNullOrEmpty(timeoutVirtuoso))
            {
                if (EnvironmentVariables.Contains("TimeoutVirtuoso"))
                {
                    timeoutVirtuoso = EnvironmentVariables["TimeoutVirtuoso"] as string;
                }
                else
                {
                    timeoutVirtuoso = Configuration["TimeoutVirtuoso"];
                }
            }
            return timeoutVirtuoso;
        }

        public bool CacheV4()
        {
            if (EnvironmentVariables.Contains("CacheV4"))
            {
                string variable = EnvironmentVariables["CacheV4"] as string;
                if (variable.ToLower() == "false")
                {
                    cacheV4 = false;
                }
                else if (variable.ToLower() == "true")
                {
                    cacheV4 = true;
                }
            }
            else
            {
                cacheV4 = Configuration.GetValue<bool?>("CacheV4");
            }
            if (cacheV4 == null)
            {
                return false;
            }
            return cacheV4.Value;
        }

        public string ObtenerSqlConnectionString()
        {
            if (string.IsNullOrEmpty(sqlConnectionString))
            {
                if (EnvironmentVariables.Contains("acid"))
                {
                    sqlConnectionString = EnvironmentVariables["acid"] as string;
                }
                else
                {
                    sqlConnectionString = Configuration.GetConnectionString("acid");
                }
            }
            return sqlConnectionString;
        }
        public string ObtenerOauthConnectionString()
        {
            if (string.IsNullOrEmpty(oauth))
            {
                if (EnvironmentVariables.Contains("oauth"))
                {
                    oauth = EnvironmentVariables["oauth"] as string;
                }
                else
                {
                    oauth = Configuration.GetConnectionString("oauth");
                }

                if (string.IsNullOrEmpty(oauth))
                {
                    oauth = ObtenerSqlConnectionString();
                }
            }
            
            return oauth;
        }
        public string ObtenerBaseConnectionString()
        {
            if (string.IsNullOrEmpty(baseConnectionString))
            {
                if (EnvironmentVariables.Contains("base"))
                {
                    baseConnectionString = EnvironmentVariables["base"] as string;
                }
                else
                {
                    baseConnectionString = Configuration.GetConnectionString("base");
                }
            }
            return baseConnectionString;
        }
        public string ObtenerVirtuosoConnectionString()
        {

            if (string.IsNullOrEmpty(virtuosoConnectionString))
            {
                if (EnvironmentVariables.Contains("virtuosoConnectionString"))
                {
                    virtuosoConnectionString = EnvironmentVariables["virtuosoConnectionString"] as string;
                }
                else
                {
                    virtuosoConnectionString = Configuration.GetConnectionString("virtuosoConnectionString");
                }
            }
            return virtuosoConnectionString;
        }

        public string ObtenerVirtuosoConnectionStringHome()
        {
            if (string.IsNullOrEmpty(virtuosoConnectionStringHome))
            {
                if (EnvironmentVariables.Contains($"virtuosoConnectionString_home"))
                {
                    virtuosoConnectionStringHome = EnvironmentVariables[$"virtuosoConnectionString_home"] as string;
                }
                else
                {
                    virtuosoConnectionStringHome = Configuration.GetConnectionString($"virtuosoConnectionString_home");
                }

                if (string.IsNullOrEmpty(virtuosoConnectionStringHome))
                {
                    virtuosoConnectionStringHome = ObtenerVirtuosoConnectionString();
                }
            }
            return virtuosoConnectionStringHome;
        }

        public string ObtenerTipoBD()
        {
            if (string.IsNullOrEmpty(tipoBD))
            {
                if (EnvironmentVariables.Contains("connectionType"))
                {
                    tipoBD = EnvironmentVariables["connectionType"] as string;
                }
                else
                {
                    tipoBD = Configuration.GetConnectionString("connectionType");
                }
            }
            return tipoBD;
        }

        public string ObtenerAzureStorageConnectionString()
        {
            if (string.IsNullOrEmpty(azure))
            {
                if (EnvironmentVariables.Contains("AzureStorageConnectionString"))
                {
                    azure = EnvironmentVariables["AzureStorageConnectionString"] as string;
                }
                else
                {
                    azure = Configuration.GetConnectionString("AzureStorageConnectionString");
                }
            }
            return azure;
        }

        public string ObtenerConexionRedisIPMaster(string mPoolName)
        {
            string cadena = "";
            if (EnvironmentVariables.Contains($"redis__{mPoolName}__ip__master"))
            {
                cadena = EnvironmentVariables[$"redis__{mPoolName}__ip__master"] as string;
            }
            else
            {
                cadena = Configuration.GetSection("ConnectionStrings").GetSection("redis").GetSection(mPoolName)["ip-master"];
            }

            return cadena;
        }

        public string ObtenerConexionRedisIPRead(string mPoolName)
        {
            string cadena = "";
            if (EnvironmentVariables.Contains($"redis__{mPoolName}__ip__read"))
            {
                cadena = EnvironmentVariables[$"redis__{mPoolName}__ip__read"] as string;
            }
            else
            {
                cadena = Configuration.GetSection("ConnectionStrings").GetSection("redis").GetSection(mPoolName)["ip-read"];
            }

            return cadena;
        }

        public int ObtenerConexionRedisBD(string mPoolName)
        {
            string bd = "";
            if (EnvironmentVariables.Contains($"redis__{mPoolName}__bd"))
            {
                bd = EnvironmentVariables[$"redis__{mPoolName}__bd"] as string;
            }
            else
            {
                bd = Configuration.GetSection("ConnectionStrings").GetSection("redis").GetSection(mPoolName)["bd"];
            }

            int bdInt;
            Int32.TryParse(bd, out bdInt);
            return bdInt;
        }

        public string Scheme
        {
            get
            {
                if (PeticionHttps())
                {
                    return "https://";
                }
                else
                {
                    return "http://";
                }
            }
        }

        public bool PeticionHttps()
        {
            if (EnvironmentVariables.Contains("https"))
            {
                string variable = EnvironmentVariables["https"] as string;
                if (variable.ToLower() == "false")
                {
                    peticionhttps = false;
                }
            }
            else
            {
                peticionhttps = Configuration.GetValue<bool?>("https");
            }
            if (peticionhttps == null)
            {
                return true;
            }
            return peticionhttps.Value;
        }

        public int ObtenerConexionRedisTimeout(string mPoolName)
        {
            string timeout = "";
            if (EnvironmentVariables.Contains($"redis__{mPoolName}__timeout"))
            {
                timeout = EnvironmentVariables[$"redis__{mPoolName}__timeout"] as string;
            }
            else
            {
                timeout = Configuration.GetSection("ConnectionStrings").GetSection("redis").GetSection(mPoolName)["timeout"];
            }

            int timeoutInt;
            Int32.TryParse(timeout, out timeoutInt);
            return timeoutInt;
        }

        public bool ExistRabbitConnection(string pTipo)
        {
            return !string.IsNullOrEmpty(ObtenerRabbitMQClient(pTipo));
        }

        public bool JSYCSSunificado()
        {
            if (jsyCssUnificado == null)
            {
                if (EnvironmentVariables.Contains("JSYCSSunificado"))
                {
                    string variable = EnvironmentVariables["JSYCSSunificado"] as string;
                    if (variable.ToLower() == "true")
                    {
                        jsyCssUnificado = true;
                    }
                }
                else
                {
                    jsyCssUnificado = Configuration.GetValue<bool?>("JSYCSSunificado");
                }
            }
            if (jsyCssUnificado == null)
            {
                return false;
            }
            return jsyCssUnificado.Value;
        }

        public bool SitioComunidadesPorDefecto()
        {
            if (sitioComunidadesPorDefecto == null)
            {
                if (EnvironmentVariables.Contains("SitioComunidadesPorDefecto"))
                {
                    string variable = EnvironmentVariables["SitioComunidadesPorDefecto"] as string;
                    if (variable.ToLower() == "true")
                    {
                        sitioComunidadesPorDefecto = true;
                    }
                }
                else
                {
                    sitioComunidadesPorDefecto = Configuration.GetValue<bool?>("SitioComunidadesPorDefecto");
                }
            }
            if (sitioComunidadesPorDefecto == null)
            {
                return false;
            }
            return sitioComunidadesPorDefecto.Value;
        }
        public bool CaptchaActive()
        {
            if (captchaActive == null)
            {
                if (EnvironmentVariables.Contains("captcha__active"))
                {
                    string variable = EnvironmentVariables["captcha__active"] as string;
                    if (variable.ToLower() == "true")
                    {
                        captchaActive = true;
                    }
                }
                else
                {
                    captchaActive = Configuration.GetSection("captcha").GetValue<bool?>("active");
                }
            }
            if (captchaActive == null)
            {
                return false;
            }
            return captchaActive.Value;
        }

        public Guid? ObtenerProyectoConexion()
        {
            if (proyectoConexion == null)
            {
                if (EnvironmentVariables.Contains("ProyectoConexion"))
                {
                    string variable = EnvironmentVariables["ProyectoConexion"] as string;
                    Guid devolver = Guid.Empty;
                    if (Guid.TryParse(variable, out devolver))
                    {
                        proyectoConexion = devolver;
                    }
                }
                else
                {
                    proyectoConexion = Configuration.GetValue<Guid?>("ProyectoConexion");
                }
            }
            if (proyectoConexion == null)
            {
                return null;
            }
            return proyectoConexion.Value;
        }

        public Guid? ObtenerProyectoGnoss()
        {
            if (proyectoGnoss == null)
            {
                if (EnvironmentVariables.Contains("ProyectoGnoss"))
                {
                    string variable = EnvironmentVariables["ProyectoGnoss"] as string;
                    Guid devolver = Guid.Empty;
                    if (Guid.TryParse(variable, out devolver))
                    {
                        proyectoGnoss = devolver;
                    }
                }
                else
                {
                    proyectoGnoss = Configuration.GetValue<Guid?>("ProyectoGnoss");
                }
            }
            return proyectoGnoss;
        }

        public Guid? ObtenerOrganizacionGnoss()
        {
            if (organizacionGnoss == null)
            {
                if (EnvironmentVariables.Contains("OrganizacionGnoss"))
                {
                    string variable = EnvironmentVariables["OrganizacionGnoss"] as string;
                    Guid devolver = Guid.Empty;
                    if (Guid.TryParse(variable, out devolver))
                    {
                        organizacionGnoss = devolver;
                    }
                }
                else
                {
                    organizacionGnoss = Configuration.GetValue<Guid?>("OrganizacionGnoss");
                }
            }
            if(organizacionGnoss == null)
            {
                organizacionGnoss = new Guid("11111111-1111-1111-1111-111111111111");
            }
            return organizacionGnoss;
        }

        public string ObtenerUrlServicioLogin()
        {
            if (string.IsNullOrEmpty(urlLogin))
            {
                if (EnvironmentVariables.Contains("Servicios__urlLogin"))
                {
                    urlLogin = EnvironmentVariables["Servicios__urlLogin"] as string;
                }
                else
                {
                    urlLogin = Configuration.GetSection("Servicios")["urlLogin"];
                }
                if (!string.IsNullOrEmpty(urlLogin) && urlLogin.EndsWith("/"))
                {
                    urlLogin = urlLogin.TrimEnd('/');
                }
            }
            return urlLogin;
        }

        public string ObtenerUrlServicio(string servicio)
        {
            string urlServicio = "";

            if (EnvironmentVariables.Contains($"Servicios__{servicio}"))
            {
                urlServicio = EnvironmentVariables[$"Servicios__{servicio}"] as string;
            }
            else
            {
                urlServicio = Configuration.GetSection("Servicios")[servicio];
            }


            return urlServicio;
        }

        public string ObtenerUrlServicioResultados()
        {
            if (string.IsNullOrEmpty(urlResultados))
            {
                if (EnvironmentVariables.Contains("Servicios__urlResultados"))
                {
                    urlResultados = EnvironmentVariables["Servicios__urlResultados"] as string;
                }
                else
                {
                    urlResultados = Configuration.GetSection("Servicios")["urlResultados"];
                }
                if (!string.IsNullOrEmpty(urlResultados) &&  urlResultados.EndsWith("/"))
                {
                    urlResultados = urlResultados.TrimEnd('/');
                }
            }
            return urlResultados;
        }

        public string ObtenerUrlServicioResultadosExterno()
        {
            if (string.IsNullOrEmpty(urlResultadosExterno))
            {
                if (EnvironmentVariables.Contains("Servicios__urlResultados__externo"))
                {
                    urlResultadosExterno = EnvironmentVariables["Servicios__urlResultados__externo"] as string;
                }
                else
                {
                    urlResultadosExterno = Configuration.GetSection("Servicios")["urlResultadosExterno"];
                }
            }
            if (string.IsNullOrEmpty(urlResultadosExterno))
            {
                urlResultadosExterno = ObtenerUrlServicioResultados();
            }
            return urlResultadosExterno;
        }

        public string ObtenerUrlServicioDespliegues()
        {
            if (string.IsNullOrEmpty(urlDespligues))
            {
                if (EnvironmentVariables.Contains("Servicios__urlServicioDespliegues"))
                {
                    urlDespligues = EnvironmentVariables["Servicios__urlServicioDespliegues"] as string;
                }
                else
                {
                    urlDespligues = Configuration.GetSection("Servicios")["urlServicioDespliegues"];
                }
            }
            return urlDespligues;
        }

        public string ObtenerUrlServicioFacetas()
        {
            if (string.IsNullOrEmpty(urlFacetas))
            {
                if (EnvironmentVariables.Contains("Servicios__urlFacetas"))
                {
                    urlFacetas = EnvironmentVariables["Servicios__urlFacetas"] as string;
                }
                else
                {
                    urlFacetas = Configuration.GetSection("Servicios")["urlFacetas"];
                }
                if (!string.IsNullOrEmpty(urlFacetas) && urlFacetas.EndsWith("/"))
                {
                    urlFacetas = urlFacetas.TrimEnd('/');
                }
            }
            return urlFacetas;
        }

        public string ObtenerUrlServicioFacetasExterno()
        {
            if (string.IsNullOrEmpty(urlFacetasExterno))
            {
                if (EnvironmentVariables.Contains("Servicios__urlFacetas__externo"))
                {
                    urlFacetasExterno = EnvironmentVariables["Servicios__urlFacetas__externo"] as string;
                }
                else
                {
                    urlFacetasExterno = Configuration.GetSection("Servicios")["urlFacetasExterno"];
                }
            }
            if (string.IsNullOrEmpty(urlFacetasExterno))
            {
                urlFacetasExterno = ObtenerUrlServicioFacetas();
            }
            if (!string.IsNullOrEmpty(urlFacetasExterno) && urlFacetasExterno.EndsWith("/"))
            {
                urlFacetasExterno = urlFacetasExterno.TrimEnd('/');
            }
            return urlFacetasExterno;
        }

        public Guid? ObtenerProyectoPrincipal()
        {
            if (proyectoPrincipal == null)
            {
                if (EnvironmentVariables.Contains("ProyectoPrincipal"))
                {
                    string variable = EnvironmentVariables["ProyectoPrincipal"] as string;
                    Guid devolver = Guid.Empty;
                    if (Guid.TryParse(variable, out devolver))
                    {
                        proyectoPrincipal = devolver;
                    }
                }
                else
                {
                    proyectoPrincipal = Configuration.GetValue<Guid?>("ProyectoPrincipal");
                }
            }

            return proyectoPrincipal;
        }

        public string ObtenerIpServicioSocketsOffline()
        {
            if (string.IsNullOrEmpty(ipServiciosSockets))
            {
                if (EnvironmentVariables.Contains("IpServicioSocketsOffline"))
                {
                    ipServiciosSockets = EnvironmentVariables["IpServicioSocketsOffline"] as string;
                }
                else
                {
                    ipServiciosSockets = Configuration["IpServicioSocketsOffline"];
                }
            }
            return ipServiciosSockets;
        }

        public int ObtenerPuertoServicioSocketsOffline()
        {
            if (puertoServiciosSockets.Equals(0))
            {
                string puerto;
                if (EnvironmentVariables.Contains("PuertoServicioSocketsOffline"))
                {
                    puerto = EnvironmentVariables["PuertoServicioSocketsOffline"] as string;
                }
                else
                {
                    puerto = Configuration["PuertoServicioSocketsOffline"];
                }

                int.TryParse(puerto, out puertoServiciosSockets);
                if (puertoServiciosSockets.Equals(0))
                {
                    puertoServiciosSockets = 1745;
                }
            }

            return puertoServiciosSockets;
        }

        public string ObtenerDominio()
        {
            if (string.IsNullOrEmpty(dominio))
            {
                if (EnvironmentVariables.Contains("dominio"))
                {
                    dominio = EnvironmentVariables["dominio"] as string;
                }
                else
                {
                    dominio = Configuration["dominio"];
                }
            }
            return dominio;
        }

        public string ObtenerClaveApiGoogle()
        {
            if (string.IsNullOrEmpty(claveApiGoogle))
            {
                if (EnvironmentVariables.Contains("claveApiGoogle"))
                {
                    claveApiGoogle = EnvironmentVariables["claveApiGoogle"] as string;
                }
                else
                {
                    claveApiGoogle = Configuration["claveApiGoogle"];
                }
            }
            return claveApiGoogle;
        }

        public string ObtenerUrlApiDesplieguesEntornoSiguiente()
        {
            if (string.IsNullOrEmpty(urlApiDesplieguesEntornoSiguiente))
            {
                if (EnvironmentVariables.Contains("Servicios__urlApiDesplieguesEntornoSiguiente"))
                {
                    urlApiDesplieguesEntornoSiguiente = EnvironmentVariables["Servicios__urlApiDesplieguesEntornoSiguiente"] as string;
                }
                else
                {
                    urlApiDesplieguesEntornoSiguiente = Configuration.GetSection("Servicios")["urlApiDesplieguesEntornoSiguiente"];
                }
            }
            return urlApiDesplieguesEntornoSiguiente;
        }

        public string ObtenerUrlApiDesplieguesEntornoParametro()
        {
            if (string.IsNullOrEmpty(urlApiDesplieguesEntornoParametro))
            {
                if (EnvironmentVariables.Contains("Servicios__urlApiDesplieguesEntornoParametro"))
                {
                    urlApiDesplieguesEntornoParametro = EnvironmentVariables["Servicios__urlApiDesplieguesEntornoParametro"] as string;
                }
                else
                {
                    urlApiDesplieguesEntornoParametro = Configuration.GetSection("Servicios")["urlApiDesplieguesEntornoParametro"];
                }
            }
            return urlApiDesplieguesEntornoParametro;
        }

        public string ObtenerParametro(string pParametro)
        {
            string parametro = "";
            if (EnvironmentVariables.Contains(pParametro))
            {
                parametro = EnvironmentVariables[pParametro] as string;
            }
            else
            {
                parametro = Configuration[pParametro];
            }
            return parametro;
        }

        public string ObtenerUrlApiDesplieguesEntornoAnterior()
        {
            if (string.IsNullOrEmpty(urlApiDesplieguesEntornoAnterior))
            {
                if (EnvironmentVariables.Contains("Servicios__urlApiDesplieguesEntornoAnterior"))
                {
                    urlApiDesplieguesEntornoAnterior = EnvironmentVariables["Servicios__urlApiDesplieguesEntornoAnterior"] as string;
                }
                else
                {
                    urlApiDesplieguesEntornoAnterior = Configuration.GetSection("Servicios")["urlApiDesplieguesEntornoAnterior"];
                }
            }
            return urlApiDesplieguesEntornoAnterior;
        }
        public string ObtenerUrlApiDesplieguesEntorno()
        {
            if (string.IsNullOrEmpty(urlApiDesplieguesEntorno))
            {
                if (EnvironmentVariables.Contains("Servicios__urlApiDesplieguesEntorno"))
                {
                    urlApiDesplieguesEntorno = EnvironmentVariables["Servicios__urlApiDesplieguesEntorno"] as string;
                }
                else
                {
                    urlApiDesplieguesEntorno = Configuration.GetSection("Servicios")["urlApiDesplieguesEntorno"];
                }
            }
            return urlApiDesplieguesEntorno;
        }

        public string ObtenerUrlApiIntegracionContinua()
        {
            if (string.IsNullOrEmpty(urlApiIntegracionContinua))
            {
                if (EnvironmentVariables.Contains("Servicios__urlApiIntegracionContinua"))
                {
                    urlApiIntegracionContinua = EnvironmentVariables["Servicios__urlApiIntegracionContinua"] as string;
                }
                else
                {
                    urlApiIntegracionContinua = Configuration.GetSection("Servicios")["urlApiIntegracionContinua"];
                }
            }
            return urlApiIntegracionContinua;
        }

        public string ObtenerIgnorarVistasPersonalizadas()
        {
            if (string.IsNullOrEmpty(ignorarVistasPersonalizadas))
            {
                if (EnvironmentVariables.Contains("IgnorarVistasPersonalizadas"))
                {
                    ignorarVistasPersonalizadas = EnvironmentVariables["IgnorarVistasPersonalizadas"] as string;
                }
                else
                {
                    ignorarVistasPersonalizadas = Configuration["IgnorarVistasPersonalizadas"];
                }
            }
            return ignorarVistasPersonalizadas;
        }

        public string ObtenerUrlContent()
        {
            if (string.IsNullOrEmpty(urlContent))
            {
                if (EnvironmentVariables.Contains("Servicios__urlContent"))
                {
                    urlContent = EnvironmentVariables["Servicios__urlContent"] as string;
                }
                else
                {
                    urlContent = Configuration.GetSection("Servicios")["urlContent"];
                }
                if (!string.IsNullOrEmpty(urlContent) && urlContent.EndsWith("/"))
                {
                    urlContent = urlContent.TrimEnd('/');
                }
            }
            return urlContent;
        }
        public string ObtenerImplementationKeyAutocompletar()
        {
            if (string.IsNullOrEmpty(implementationKeyAutocompletar))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyAutocompletar"))
                {
                    implementationKeyAutocompletar = EnvironmentVariables["ImplementationKeyAutocompletar"] as string;
                }
                else
                {
                    implementationKeyAutocompletar = Configuration["ImplementationKeyAutocompletar"];
                }
            }
            return implementationKeyAutocompletar;
        }
        public string ObtenerImplementationKeyFacetas()
        {
            if (string.IsNullOrEmpty(implementationKeyFacetas))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyFacetas"))
                {
                    implementationKeyFacetas = EnvironmentVariables["ImplementationKeyFacetas"] as string;
                }
                else
                {
                    implementationKeyFacetas = Configuration["ImplementationKeyFacetas"];
                }
            }
            return implementationKeyFacetas;
        }

        public string ObtenerKeySesion()
        {
            if (string.IsNullOrEmpty(keySession))
            {
                if (EnvironmentVariables.Contains("KeySession"))
                {
                    keySession = EnvironmentVariables["KeySession"] as string;
                }
                else
                {
                    keySession = Configuration["KeySession"];
                }
            }
            return keySession;
        }

        public string ObtenerImplementationKeyResultados()
        {
            if (string.IsNullOrEmpty(implementationKeyResultados))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyResultados"))
                {
                    implementationKeyResultados = EnvironmentVariables["ImplementationKeyResultados"] as string;
                }
                else
                {
                    implementationKeyResultados = Configuration["ImplementationKeyResultados"];
                }
            }
            return implementationKeyResultados;
        }
        public string ObtenerUbicacionLogsAutocompletar()
        {
            if (string.IsNullOrEmpty(ubicacionLogsAutocompletar))
            {
                if (EnvironmentVariables.Contains("UbicacionLogsAutocompletar"))
                {
                    ubicacionLogsAutocompletar = EnvironmentVariables["UbicacionLogsAutocompletar"] as string;
                }
                else
                {
                    ubicacionLogsAutocompletar = Configuration["UbicacionLogsAutocompletar"];
                }
            }
            return ubicacionLogsAutocompletar;
        }

        public string ObtenerUbicacionLogsFacetas()
        {
            if (string.IsNullOrEmpty(ubicacionLogsFacetas))
            {
                if (EnvironmentVariables.Contains("UbicacionLogsFacetas"))
                {
                    ubicacionLogsFacetas = EnvironmentVariables["UbicacionLogsFacetas"] as string;
                }
                else
                {
                    ubicacionLogsFacetas = Configuration["UbicacionLogsFacetas"];
                }
            }
            return ubicacionLogsFacetas;
        }
        public string ObtenerUbicacionLogsResultados()
        {
            if (string.IsNullOrEmpty(ubicacionLogsResultados))
            {
                if (EnvironmentVariables.Contains("UbicacionLogsResultados"))
                {
                    ubicacionLogsResultados = EnvironmentVariables["UbicacionLogsResultados"] as string;
                }
                else
                {
                    ubicacionLogsResultados = Configuration["UbicacionLogsResultados"];
                }
            }
            return ubicacionLogsResultados;
        }
        public string ObtenerUbicacionLogsAutocompletarEtiquetas()
        {
            if (string.IsNullOrEmpty(ubicacionLogsAutocompletarEtiquetas))
            {
                if (EnvironmentVariables.Contains("UbicacionLogsAutocompletarEtiquetas"))
                {
                    ubicacionLogsAutocompletarEtiquetas = EnvironmentVariables["UbicacionLogsAutocompletarEtiquetas"] as string;
                }
                else
                {
                    ubicacionLogsAutocompletarEtiquetas = Configuration["UbicacionLogsAutocompletarEtiquetas"];
                }
            }
            return ubicacionLogsAutocompletarEtiquetas;
        }
        public string ObtenerUbicacionTrazasFacetas()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasFacetas))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasFacetas"))
                {
                    ubicacionTrazasFacetas = EnvironmentVariables["UbicacionTrazasFacetas"] as string;
                }
                else
                {
                    ubicacionTrazasFacetas = Configuration["UbicacionTrazasFacetas"];
                }
            }
            return ubicacionTrazasFacetas;
        }
        public string ObtenerUbicacionTrazasResultados()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasResultados))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasResultados"))
                {
                    ubicacionTrazasResultados = EnvironmentVariables["UbicacionTrazasResultados"] as string;
                }
                else
                {
                    ubicacionTrazasResultados = Configuration["UbicacionTrazasResultados"];
                }
            }
            return ubicacionTrazasResultados;
        }

        public string ObtenerUbicacionTrazasAutocompletar()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasAutocompletar))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasAutocompletar"))
                {
                    ubicacionTrazasAutocompletar = EnvironmentVariables["UbicacionTrazasAutocompletar"] as string;
                }
                else
                {
                    ubicacionTrazasAutocompletar = Configuration["UbicacionTrazasAutocompletar"];
                }
            }
            return ubicacionTrazasAutocompletar;
        }

        public string ObtenerUbicacionLogsLogin()
        {
            if (string.IsNullOrEmpty(ubicacionLogsLogin))
            {
                if (EnvironmentVariables.Contains("UbicacionLogsLogin"))
                {
                    ubicacionLogsLogin = EnvironmentVariables["UbicacionLogsLogin"] as string;
                }
                else
                {
                    ubicacionLogsLogin = Configuration["UbicacionLogsLogin"];
                }
            }
            return ubicacionLogsLogin;
        }


        public string ObtenerUbicacionTrazasLogin()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasLogin))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasLogin"))
                {
                    ubicacionTrazasLogin = EnvironmentVariables["UbicacionTrazasLogin"] as string;
                }
                else
                {
                    ubicacionTrazasLogin = Configuration["UbicacionTrazasLogin"];
                }
            }
            return ubicacionTrazasLogin;
        }

        public string ObtenerImplementationKeyLogin()
        {
            if (string.IsNullOrEmpty(implementationKeyLogin))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyLogin"))
                {
                    implementationKeyLogin = EnvironmentVariables["ImplementationKeyLogin"] as string;
                }
                else
                {
                    implementationKeyLogin = Configuration["ImplementationKeyLogin"];
                }
            }
            return implementationKeyLogin;
        }

        public string ObtenerUbicacionLogsApiV3()
        {
            if (string.IsNullOrEmpty(ubicacionLogsApiV3))
            {
                if (EnvironmentVariables.Contains("UbicacionLogsApiV3"))
                {
                    ubicacionLogsApiV3 = EnvironmentVariables["UbicacionLogsApiV3"] as string;
                }
                else
                {
                    ubicacionLogsApiV3 = Configuration["UbicacionLogsApiV3"];
                }
            }
            return ubicacionLogsApiV3;
        }


        public string ObtenerUbicacionTrazasApiV3()
        {
            if (string.IsNullOrEmpty(ubicacionTrazasApiV3))
            {
                if (EnvironmentVariables.Contains("UbicacionTrazasApiV3"))
                {
                    ubicacionTrazasApiV3 = EnvironmentVariables["UbicacionTrazasApiV3"] as string;
                }
                else
                {
                    ubicacionTrazasApiV3 = Configuration["UbicacionTrazasApiV3"];
                }
            }
            return ubicacionTrazasApiV3;
        }

        public string ObtenerImplementationKeyApiV3()
        {
            if (string.IsNullOrEmpty(implementationKeyApiV3))
            {
                if (EnvironmentVariables.Contains("ImplementationKeyApiV3"))
                {
                    implementationKeyApiV3 = EnvironmentVariables["ImplementationKeyApiV3"] as string;
                }
                else
                {
                    implementationKeyApiV3 = Configuration["ImplementationKeyApiV3"];
                }
            }
            return implementationKeyApiV3;
        }

        public string ObtenerColaLive()
        {
            if (string.IsNullOrEmpty(colalive))
            {
                if (EnvironmentVariables.Contains("colalive"))
                {
                    colalive = EnvironmentVariables["colalive"] as string;
                }
                else
                {
                    colalive = Configuration["colalive"];
                }
            }
            return colalive;
        }

        public bool ExistColaReplicacionMaster()
        {
            bool existEnvironmentVariables = false;
            bool exist = false;
            foreach (var key in EnvironmentVariables.Keys)
            {
                if (((string)key).Contains("ColaReplicacionMaster"))
                {
                    existEnvironmentVariables = true;
                    exist = true;
                }
            }
            if (!existEnvironmentVariables)
            {
                exist = Configuration.GetSection("ColaReplicacionMaster").Exists();
            }

            return exist;
        }

        public bool ExistColaReplicacionMasterHome()
        {
            bool existEnvironmentVariables = false;
            bool exist = false;
            foreach (var key in EnvironmentVariables.Keys)
            {
                if (((string)key).Contains("ColaReplicacionMasterHome"))
                {
                    existEnvironmentVariables = true;
                    exist = true;
                }
            }
            if (!existEnvironmentVariables)
            {
                exist = Configuration.GetSection("ColaReplicacionMasterHome").Exists();
            }

            return exist;
        }

        public Dictionary<string, string> ObtenerColasReplicacionMaster()
        {
            bool existEnvironmentVariables = false;
            Dictionary<string, string> colasReplicacionMaster = new Dictionary<string, string>();
            foreach (var key in EnvironmentVariables.Keys)
            {
                if (((string)key).Contains("ColaReplicacionMaster_"))
                {
                    existEnvironmentVariables = true;
                    string keyValue = (string)key;
                    colasReplicacionMaster.Add(keyValue.Replace("ColaReplicacionMaster_", ""), EnvironmentVariables[keyValue] as string);
                }
            }
            if (!existEnvironmentVariables)
            {
                bool exist = Configuration.GetSection("ColaReplicacionMaster").Exists();
                if (exist)
                {
                    var colas = Configuration.GetSection("ColaReplicacionMaster").GetChildren();
                    foreach (var cola in colas)
                    {
                        colasReplicacionMaster.Add(cola.Key, cola.Value);
                    }
                }
            }

            return colasReplicacionMaster;
        }

        public Dictionary<string, string> ObtenerColasReplicacionMasterHome()
        {
            bool existEnvironmentVariables = false;
            Dictionary<string, string> colasReplicacionMasterHome = new Dictionary<string, string>();
            foreach (var key in EnvironmentVariables.Keys)
            {
                if (((string)key).Contains("ColaReplicacionMasterHome__"))
                {
                    existEnvironmentVariables = true;
                    string keyValue = (string)key;
                    colasReplicacionMasterHome.Add(keyValue.Replace("ColaReplicacionMasterHome__", ""), EnvironmentVariables[keyValue] as string);
                }
            }
            if (!existEnvironmentVariables)
            {
                bool exist = Configuration.GetSection("ColaReplicacionMasterHome").Exists();
                if (exist)
                {
                    var colas = Configuration.GetSection("ColaReplicacionMasterHome").GetChildren();
                    foreach (var cola in colas)
                    {
                        colasReplicacionMasterHome.Add(cola.Key, cola.Value);
                    }
                }
            }

            return colasReplicacionMasterHome;
        }

        public string ObtenerRutaBaseTriples()
        {
            if (string.IsNullOrEmpty(rutaBaseTriples))
            {
                if (EnvironmentVariables.Contains("rutaBaseTriples"))
                {
                    rutaBaseTriples = EnvironmentVariables["rutaBaseTriples"] as string;
                }
                else
                {
                    rutaBaseTriples = Configuration["rutaBaseTriples"];
                }
            }
            return rutaBaseTriples;
        }

        public string ObtenerUrlTriples()
        {
            if (string.IsNullOrEmpty(urlTriples))
            {
                if (EnvironmentVariables.Contains("urlTriples"))
                {
                    urlTriples = EnvironmentVariables["urlTriples"] as string;
                }
                else
                {
                    urlTriples = Configuration["urlTriples"];
                }
            }
            return urlTriples;
        }

        public string ObtenerJenkinsToken()
        {
            if (string.IsNullOrEmpty(tokenJenkins))
            {
                if (EnvironmentVariables.Contains("JenkinsToken"))
                {
                    tokenJenkins = EnvironmentVariables["JenkinsToken"] as string;
                }
                else
                {
                    tokenJenkins = Configuration["JenkinsToken"];
                }
            }
            return tokenJenkins;
        }

        public string ObtenerJenkinsPipeline()
        {
            if (string.IsNullOrEmpty(connectionJenkins))
            {
                if (EnvironmentVariables.Contains("JenkinsConnection"))
                {
                    connectionJenkins = EnvironmentVariables["JenkinsConnection"] as string;
                }
                else
                {
                    connectionJenkins = Configuration["JenkinsConnection"];
                }
            }
            return connectionJenkins;
        }

        public string ObtenerEmailErrores()
        {
            if (string.IsNullOrEmpty(emailErrores))
            {
                if (EnvironmentVariables.Contains("emailErrores"))
                {
                    emailErrores = EnvironmentVariables["emailErrores"] as string;
                }
                else
                {
                    emailErrores = Configuration["emailErrores"];
                }
            }
            return emailErrores;
        }


        public int ObtenerHilosAplicacion()
        {
            string hilos;
            if (EnvironmentVariables.Contains("hilosAplicacion"))
            {
                hilos = EnvironmentVariables["hilosAplicacion"] as string;
            }
            else
            {
                hilos = Configuration["hilosAplicacion"];
            }
            if (!string.IsNullOrEmpty(hilos))
            {
                int numHilosOut;
                Int32.TryParse(hilos, out numHilosOut);
                if (numHilosOut != 0)
                {
                    hilosAplicacion = numHilosOut;
                }
            }
            else
            {
                hilosAplicacion = 0;
            }
            
            return hilosAplicacion;
        }

        public bool ObtenerProcesarStringGrafo()
        {
            if (procesarStringGrafo == null)
            {
                if (EnvironmentVariables.Contains("procesarStringGrafo"))
                {
                    string variable = EnvironmentVariables["procesarStringGrafo"] as string;
                    if (variable.ToLower() == "true")
                    {
                        procesarStringGrafo = true;
                        UtilCadenas.LowerStringGraph = true;
                    }
                }
                else
                {
                    procesarStringGrafo = Configuration.GetValue<bool?>("procesarStringGrafo");
                    if (procesarStringGrafo.HasValue)
                    {
                        UtilCadenas.LowerStringGraph = procesarStringGrafo.Value;
                    }
                }
            }
            if (procesarStringGrafo == null)
            {
                return false;
            }
            return procesarStringGrafo.Value;
        }

        public bool ObtenerReplicacionActivada()
        {
            if (replicacionActivada == null)
            {
                if (EnvironmentVariables.Contains("replicacionActivada"))
                {
                    string variable = EnvironmentVariables["replicacionActivada"] as string;
                    if (variable.ToLower() == "true")
                    {
                        replicacionActivada = true;
                    }
                }
                else
                {
                    replicacionActivada = Configuration.GetValue<bool?>("replicacionActivada");
                }
            }
            if (replicacionActivada == null)
            {
                return true;
            }
            return replicacionActivada.Value;
        }

        public bool ObtenerReplicacionActivadaHOME()
        {
            if (replicacionActivadaHome == null)
            {
                if (EnvironmentVariables.Contains(""))
                {
                    string variable = EnvironmentVariables["replicacionActivadaHome"] as string;
                    if (variable.ToLower() == "true")
                    {
                        replicacionActivadaHome = true;
                    }
                }
                else
                {
                    replicacionActivadaHome = Configuration.GetValue<bool?>("replicacionActivadaHome");
                }
            }
            if (replicacionActivadaHome == null)
            {
                replicacionActivadaHome = ObtenerReplicacionActivada();
            }
            return replicacionActivadaHome.Value;
        }

        public bool ObtenerEscribirFicheroExternoTriples()
        {
            if (escribirFicheroExternoTriples == null)
            {
                if (EnvironmentVariables.Contains("escribirFicheroExternoTriples"))
                {
                    string variable = EnvironmentVariables["escribirFicheroExternoTriples"] as string;
                    if (variable.ToLower() == "true")
                    {
                        escribirFicheroExternoTriples = true;
                    }
                }
                else
                {
                    escribirFicheroExternoTriples = Configuration.GetValue<bool?>("escribirFicheroExternoTriples");
                }
            }
            if (escribirFicheroExternoTriples == null)
            {
                return false;
            }
            return escribirFicheroExternoTriples.Value;
        }

        public int ObtenerHoraEnvioErrores()
        {
            if (horaEnvioErrores == 0)
            {
                string horaEnvioErroresString;
                if (EnvironmentVariables.Contains("horaEnvioErrores"))
                {
                    horaEnvioErroresString = EnvironmentVariables["horaEnvioErrores"] as string;
                }
                else
                {
                    horaEnvioErroresString = Configuration["horaEnvioErrores"];
                }
                int horaEnvioErroresOut;
                Int32.TryParse(horaEnvioErroresString, out horaEnvioErroresOut);
                horaEnvioErrores = horaEnvioErroresOut;

            }
            return horaEnvioErrores;
        }

        public string ObtenerRobots()
        {
            if (string.IsNullOrEmpty(robots))
            {
                if (EnvironmentVariables.Contains("robots"))
                {
                    robots = EnvironmentVariables["robots"] as string;
                }
                else
                {
                    robots = Configuration["robots"];
                }
            }
            return robots;
        }

        public string ObtenerUrlServicioDocumental()
        {
            if (string.IsNullOrEmpty(urlDocuments))
            {
                if (EnvironmentVariables.Contains("Servicios__urlDocuments"))
                {
                    urlDocuments = EnvironmentVariables["Servicios__urlDocuments"] as string;
                }
                else
                {
                    urlDocuments = Configuration.GetSection("Servicios")["urlDocuments"];
                }
                if (!string.IsNullOrEmpty(urlDocuments) && urlDocuments.EndsWith("/"))
                {
                    urlDocuments = urlDocuments.TrimEnd('/');
                }
            }
            return urlDocuments;

        }

        public string ObtenerUrlServicioEtiquetas()
        {
            if (string.IsNullOrEmpty(urlServicioEtiquetas))
            {
                if (EnvironmentVariables.Contains("Servicios__urlServicioEtiquetas"))
                {
                    urlServicioEtiquetas = EnvironmentVariables["Servicios__urlServicioEtiquetas"] as string;
                }
                else
                {
                    urlServicioEtiquetas = Configuration.GetSection("Servicios")["urlServicioEtiquetas"];
                }
            }
            return urlServicioEtiquetas;
        }

        public string ObtenerUrlServicioInterno()
        {
            if (string.IsNullOrEmpty(urlIntern))
            {
                if (EnvironmentVariables.Contains("Servicios__urlInterno"))
                {
                    urlIntern = EnvironmentVariables["Servicios__urlInterno"] as string;
                }
                else
                {
                    urlIntern = Configuration.GetSection("Servicios")["urlInterno"];
                }
                if (!string.IsNullOrEmpty(urlIntern) && urlIntern.EndsWith("/"))
                {
                    urlIntern = urlIntern.TrimEnd('/');
                }
            }
            return urlIntern;
        }

        public string ObtenerUrlServicioBrightcove()
        {
            if (string.IsNullOrEmpty(urlServicioBrightcove))
            {
                if (EnvironmentVariables.Contains("Servicios__urlServicioBrightcove"))
                {
                    urlServicioBrightcove = EnvironmentVariables["Servicios__urlServicioBrightcove"] as string;
                }
                else
                {
                    urlServicioBrightcove = Configuration.GetSection("Servicios")["urlServicioBrightcove"];
                }
            }
            return urlServicioBrightcove;
        }

        /// <summary>
        /// obtiene el endpoint para la llamada de obtención del token
        /// </summary> 
        public string GetAuthority()
        {
            if (string.IsNullOrEmpty(Authority))
            {
                string authority = "";
                if (EnvironmentVariables.Contains("Authority"))
                {
                    authority = EnvironmentVariables["Authority"] as string;
                }
                else
                {
                    authority = Configuration["Authority"];
                }

                Authority = authority;
            }
            return Authority;
        }
        public string ObtenerUrlServicioTOP()
        {
            if (string.IsNullOrEmpty(urlServicioTOP))
            {
                if (EnvironmentVariables.Contains("Servicios__urlServicioTOP"))
                {
                    urlServicioTOP = EnvironmentVariables["Servicios__urlServicioTOP"] as string;
                }
                else
                {
                    urlServicioTOP = Configuration.GetSection("Servicios")["urlServicioTOP"];
                }
            }
            return urlServicioTOP;
        }

        public string ObtnerIpServicioTrazasUDP()
        {
            if (string.IsNullOrEmpty(ipServicioTrazasUDP))
            {
                if (EnvironmentVariables.Contains("ipServicioTrazasUDP"))
                {
                    ipServicioTrazasUDP = EnvironmentVariables["ipServicioTrazasUDP"] as string;
                }
                else
                {
                    ipServicioTrazasUDP = Configuration["ipServicioTrazasUDP"];
                }
            }
            return ipServicioTrazasUDP;
        }

        public ReadOnlySpan<char> ObtenerPuertoServicioTrazasUDP()
        {
            if (string.IsNullOrEmpty(puertoServicioTrazasUDP))
            {
                if (EnvironmentVariables.Contains("puertoServicioTrazasUDP"))
                {
                    puertoServicioTrazasUDP = EnvironmentVariables["puertoServicioTrazasUDP"] as string;
                }
                else
                {
                    puertoServicioTrazasUDP = Configuration["puertoServicioTrazasUDP"];
                }
            }
            return puertoServicioTrazasUDP;
        }

        public string ObtenerProyectoID()
        {
            if (string.IsNullOrEmpty(proyectoID))
            {
                if (EnvironmentVariables.Contains("proyectoID"))
                {
                    proyectoID = EnvironmentVariables["proyectoID"] as string;
                }
                else
                {
                    proyectoID = Configuration["proyectoID"];
                }
            }
            return proyectoID;
        }

        public string ObtenerDirectorioEstilos()
        {
            if (string.IsNullOrEmpty(proyectoID))
            {
                if (EnvironmentVariables.Contains("rootStylesDirectory"))
                {
                    proyectoID = EnvironmentVariables["rootStylesDirectory"] as string;
                }
                else
                {
                    proyectoID = Configuration["rootStylesDirectory"];
                }
            }
            return proyectoID;
        }
        public string ObtenerDirectorioServiciosWeb()
        {
            if (string.IsNullOrEmpty(proyectoID))
            {
                if (EnvironmentVariables.Contains("rootWebServiceDirectory"))
                {
                    proyectoID = EnvironmentVariables["rootWebServiceDirectory"] as string;
                }
                else
                {
                    proyectoID = Configuration["rootWebServiceDirectory"];
                }
            }
            return proyectoID;
        }

        public int ObtenerNumVisitasHilo()
        {
            if (mNumVisitasHilo == 0 || mNumVisitasHilo == 100)
            {
                string numVisitasHilo;
                if (EnvironmentVariables.Contains("num-visitas-hilo"))
                {
                    numVisitasHilo = EnvironmentVariables["num-visitas-hilo"] as string;
                }
                else
                {
                    numVisitasHilo = Configuration["num-visitas-hilo"];
                }
                int numVisitasHiloOut;
                Int32.TryParse(numVisitasHilo, out numVisitasHiloOut);
                if (numVisitasHiloOut != 0)
                {
                    mNumVisitasHilo = numVisitasHiloOut;
                }

            }
            return mNumVisitasHilo;
        }

        public int ObtenerNumHilosAbiertos()
        {
            if (mNumHilosAbiertos == 0 || mNumHilosAbiertos == 5)
            {
                string numHilosAbiertos;
                if (EnvironmentVariables.Contains("num-hilos-abiertos"))
                {
                    numHilosAbiertos = EnvironmentVariables["num-hilos-abiertos"] as string;
                }
                else
                {
                    numHilosAbiertos = Configuration["num-hilos-abiertos"];
                }
                int numHilosAbiertosOut;
                Int32.TryParse(numHilosAbiertos, out numHilosAbiertosOut);
                if (numHilosAbiertosOut != 0)
                {
                    mNumHilosAbiertos = numHilosAbiertosOut;
                }

            }
            return mNumHilosAbiertos;
        }

        public int ObtenerMinutosAntesProcesar()
        {
            if (mMinutosAntesProcesar == 0 || mMinutosAntesProcesar == 5)
            {
                string minutosAntesProcesar;
                if (EnvironmentVariables.Contains("min-pre-procesado"))
                {
                    minutosAntesProcesar = EnvironmentVariables["min-pre-procesado"] as string;
                }
                else
                {
                    minutosAntesProcesar = Configuration["min-pre-procesado"];
                }
                int minutosAntesProcesarsOut;
                Int32.TryParse(minutosAntesProcesar, out minutosAntesProcesarsOut);
                if (minutosAntesProcesarsOut != 0)
                {
                    mMinutosAntesProcesar = minutosAntesProcesarsOut;
                }

            }
            return mMinutosAntesProcesar;
        }

        public int ObtenerHorasProcesarVisitasVirtuoso()
        {
            if (mHorasProcesarVisitasVirtuoso == 0 || mHorasProcesarVisitasVirtuoso == 6)
            {
                string horasProcesarVisitasVirtuoso;
                if (EnvironmentVariables.Contains("horas-procesar-visitas-virtuoso"))
                {
                    horasProcesarVisitasVirtuoso = EnvironmentVariables["horas-procesar-visitas-virtuoso"] as string;
                }
                else
                {
                    horasProcesarVisitasVirtuoso = Configuration["horas-procesar-visitas-virtuoso"];
                }

                int horasProcesarVisitasVirtuosoOut;
                Int32.TryParse(horasProcesarVisitasVirtuoso, out horasProcesarVisitasVirtuosoOut);
                if (horasProcesarVisitasVirtuosoOut != 0)
                {
                    mHorasProcesarVisitasVirtuoso = horasProcesarVisitasVirtuosoOut;
                }

            }
            return mHorasProcesarVisitasVirtuoso;
        }

        public int ObtenerPuertoUDP()
        {
            if (mPuertoUDP == 0)
            {
                string puertoUDP;
                if (EnvironmentVariables.Contains("puertoUDP"))
                {
                    puertoUDP = EnvironmentVariables["puertoUDP"] as string;
                }
                else
                {
                    puertoUDP = Configuration["puertoUDP"];
                }

                int puertoUDPOut;
                Int32.TryParse(puertoUDP, out puertoUDPOut);
                if (puertoUDPOut != 0)
                {
                    mPuertoUDP = puertoUDPOut;
                }

            }
            return mPuertoUDP;
        }

        public int ObtenerTiempocapturaurl()
        {
            if (mTiempocapturaurl == 0)
            {
                string tiempocapturaurl;
                if (EnvironmentVariables.Contains("tiempocapturaurl"))
                {
                    tiempocapturaurl = EnvironmentVariables["tiempocapturaurl"] as string;
                }
                else
                {
                    tiempocapturaurl = Configuration["tiempocapturaurl"];
                }

                int tiempocapturaurlOut;
                Int32.TryParse(tiempocapturaurl, out tiempocapturaurlOut);
                if (tiempocapturaurlOut != 0)
                {
                    mTiempocapturaurl = tiempocapturaurlOut;
                }

            }
            return mTiempocapturaurl;
        }
        public int ObtenerVisitasVotosComentarios()
        {
            if (mVisitasVotosComentarios == 0 || mVisitasVotosComentarios == 5)
            {
                string visitasVotosComentarios;
                if (EnvironmentVariables.Contains("actualizarVisitasVotosComentario"))
                {
                    visitasVotosComentarios = EnvironmentVariables["actualizarVisitasVotosComentario"] as string;
                }
                else
                {
                    visitasVotosComentarios = Configuration["actualizarVisitasVotosComentario"];
                }

                int visitasVotosComentariosOut;
                Int32.TryParse(visitasVotosComentarios, out visitasVotosComentariosOut);
                if (visitasVotosComentariosOut != 0)
                {
                    mVisitasVotosComentarios = visitasVotosComentariosOut;
                }

            }
            return mVisitasVotosComentarios;
        }

        public int ObtenerIntervaloVVC()
        {
            if (mIntervaloVVC == 0)
            {
                string intervaloVVC;
                if (EnvironmentVariables.Contains("intervaloVVC"))
                {
                    intervaloVVC = EnvironmentVariables["intervaloVVC"] as string;
                }
                else
                {
                    intervaloVVC = Configuration["intervaloVVC"];
                }

                int intervaloVVCOut;
                Int32.TryParse(intervaloVVC, out intervaloVVCOut);
                if (intervaloVVCOut != 0)
                {
                    mIntervaloVVC = intervaloVVCOut;
                }

            }
            return mIntervaloVVC;
        }
        public int ObtenerNumMaxPeticionesWebSimultaneas()
        {
            if (mNumMaxPeticionesWebSimultaneas == 0 || mNumMaxPeticionesWebSimultaneas == 5)
            {
                string numMaxPeticionesWebSimultaneas;
                if (EnvironmentVariables.Contains("numMaxPeticionesWebSimultaneas"))
                {
                    numMaxPeticionesWebSimultaneas = EnvironmentVariables["numMaxPeticionesWebSimultaneas"] as string;
                }
                else
                {
                    numMaxPeticionesWebSimultaneas = Configuration["numMaxPeticionesWebSimultaneas"];
                }

                int numMaxPeticionesWebSimultaneasOut;
                Int32.TryParse(numMaxPeticionesWebSimultaneas, out numMaxPeticionesWebSimultaneasOut);
                if (numMaxPeticionesWebSimultaneasOut != 0)
                {
                    mNumMaxPeticionesWebSimultaneas = numMaxPeticionesWebSimultaneasOut;
                }

            }
            return mNumMaxPeticionesWebSimultaneas;
        }

        public int ObtenerMinutosAgruparRegistrosUsuariosEnProyecto()
        {
            if (mMinutosAgruparRegistrosUsuariosEnProyecto == 0 || mMinutosAgruparRegistrosUsuariosEnProyecto == 60)
            {
                string minutosAgruparRegistrosUsuariosEnProyecto;
                if (EnvironmentVariables.Contains("MinutosAgruparRegistrosUsuariosEnProyecto"))
                {
                    minutosAgruparRegistrosUsuariosEnProyecto = EnvironmentVariables["MinutosAgruparRegistrosUsuariosEnProyecto"] as string;
                }
                else
                {
                    minutosAgruparRegistrosUsuariosEnProyecto = Configuration["MinutosAgruparRegistrosUsuariosEnProyecto"];
                }

                int minutosAgruparRegistrosUsuariosEnProyectoOut;
                Int32.TryParse(minutosAgruparRegistrosUsuariosEnProyecto, out minutosAgruparRegistrosUsuariosEnProyectoOut);
                if (minutosAgruparRegistrosUsuariosEnProyectoOut != 0)
                {
                    mMinutosAgruparRegistrosUsuariosEnProyecto = minutosAgruparRegistrosUsuariosEnProyectoOut;
                }

            }
            return mMinutosAgruparRegistrosUsuariosEnProyecto;
        }

        public string ObtenerVirtuosoEndpointEN()
        {
            string endpoint = "";
            var rand = new Random();
            if (EnvironmentVariables.Contains($"Virtuoso__virtuosoEndpointEN"))
            {
                endpoint = EnvironmentVariables[$"Virtuoso__virtuosoEndpointEN"] as string;
            }
            else
            {
                endpoint = Configuration.GetSection("Virtuoso")["virtuosoEndpointEN"];

            }

            return endpoint;
        }
        public string ObtenerVirtuosoEndpointES()
        {
            string endpoint = "";
            var rand = new Random();
            if (EnvironmentVariables.Contains($"Virtuoso__virtuosoEndpointES"))
            {
                endpoint = EnvironmentVariables[$"Virtuoso__virtuosoEndpointES"] as string;
            }
            else
            {
                endpoint = Configuration.GetSection("Virtuoso")["virtuosoEndpointES"];

            }

            return endpoint;
        }

        public string ObtenerRuta()
        {
            if (string.IsNullOrEmpty(ruta))
            {
                if (EnvironmentVariables.Contains("ruta"))
                {
                    ruta = EnvironmentVariables["ruta"] as string;
                }
                else
                {
                    ruta = Configuration["ruta"];
                }
            }
            return ruta;
        }

        public string ObtenerUbicacionIndiceLucene()
        {
            if (string.IsNullOrEmpty(ubicacionIndiceLucene))
            {
                if (EnvironmentVariables.Contains("UbicacionIndiceLucene"))
                {
                    ubicacionIndiceLucene = EnvironmentVariables["UbicacionIndiceLucene"] as string;
                }
                else
                {
                    ubicacionIndiceLucene = Configuration["UbicacionIndiceLucene"];
                }
            }
            return ubicacionIndiceLucene;
        }

        public string ObtenerVapidPublicKey()
        {
            if (string.IsNullOrEmpty(vapidPublicKey))
            {
                if (EnvironmentVariables.Contains("VAPID__publicKey"))
                {
                    vapidPublicKey = EnvironmentVariables["VAPID__publicKey"] as string;
                }
                else
                {
                    vapidPublicKey = Configuration.GetSection("VAPID")["publicKey"];
                }
            }
            return vapidPublicKey;
        }

        public string ObtenerVapidPrivateKey()
        {
            if (string.IsNullOrEmpty(vapidPrivateKey))
            {
                if (EnvironmentVariables.Contains("VAPID__privateKey"))
                {
                    vapidPrivateKey = EnvironmentVariables["VAPID__privateKey"] as string;
                }
                else
                {
                    vapidPrivateKey = Configuration.GetSection("VAPID")["privateKey"];
                }
            }
            return vapidPrivateKey;
        }

        public string ObtenerVapidSubject()
        {
            if (string.IsNullOrEmpty(vapidSubject))
            {
                if (EnvironmentVariables.Contains("VAPID__subject"))
                {
                    vapidSubject = EnvironmentVariables["VAPID__subject"] as string;
                }
                else
                {
                    vapidSubject = Configuration.GetSection("VAPID")["subject"];
                }
            }
            return vapidSubject;
        }

        public string ObtenerPuertoVirtuoso()
        {
            if (string.IsNullOrEmpty(puertoVirtuoso))
            {
                if (EnvironmentVariables.Contains("puertoVirtuoso"))
                {
                    puertoVirtuoso = EnvironmentVariables["puertoVirtuoso"] as string;
                }
                else
                {
                    puertoVirtuoso = Configuration.GetConnectionString("puertoVirtuoso");
                }
                if (string.IsNullOrEmpty(puertoVirtuoso))
                {
                    puertoVirtuoso = "8890";
                }
            }
            return puertoVirtuoso;
        }

        public string ObtenerPuertoVirtuosoAux()
        {
            if (string.IsNullOrEmpty(puertoVirtuosoAux))
            {
                if (EnvironmentVariables.Contains("puertoVirtuosoAux"))
                {
                    puertoVirtuosoAux = EnvironmentVariables["puertoVirtuosoAux"] as string;
                }
                else
                {
                    puertoVirtuosoAux = Configuration.GetConnectionString("puertoVirtuosoAux");
                }
                if (string.IsNullOrEmpty(puertoVirtuosoAux))
                {
                    puertoVirtuosoAux = "1111";
                }
            }
            return puertoVirtuosoAux;
        }

        public bool TrazaHabilitada()
        {
            if (!trazasHabilitadas.HasValue)
            {
                if (EnvironmentVariables.Contains("trazasHabilitadas"))
                {
                    string variable = EnvironmentVariables["trazasHabilitadas"] as string;
                    if (variable.ToLower() == "false")
                    {
                        trazasHabilitadas = false;
                    }
                    else if (variable.ToLower() == "true")
                    {
                        trazasHabilitadas = true;
                    }
                }
                else
                {
                    trazasHabilitadas = Configuration.GetValue<bool?>("trazasHabilitadas");
                }
                if (trazasHabilitadas == null)
                {
                    trazasHabilitadas = false;
                }
            }
            return trazasHabilitadas.Value;
        }

        public bool ConfiguradoEtiquetadoInteligente()
        {
            if (configuradoEtiquetadoInteligente == null)
            {
                if (EnvironmentVariables.Contains("configuradoEtiquetadoInteligente"))
                {
                    string variable = EnvironmentVariables["configuradoEtiquetadoInteligente"] as string;
                    if (variable.ToLower() == "true")
                    {
                        configuradoEtiquetadoInteligente = true;
                    }
                }
                else
                {
                    configuradoEtiquetadoInteligente = Configuration.GetValue<bool?>("configuradoEtiquetadoInteligente");
                }
            }
            if (configuradoEtiquetadoInteligente == null)
            {
                return false;
            }
            return configuradoEtiquetadoInteligente.Value;
        }

        public string ObtenerUrlEtiquetadoInteligente()
        {

            if (string.IsNullOrEmpty(urlEtiquetadoInteligente))
            {
                if (EnvironmentVariables.Contains("Servicios__urlEtiquetadoInteligente"))
                {
                    urlEtiquetadoInteligente = EnvironmentVariables["Servicios__urlEtiquetadoInteligente"] as string;
                }
                else
                {
                    urlEtiquetadoInteligente = Configuration.GetSection("Servicios")["urlEtiquetadoInteligente"];
                }
                if (!string.IsNullOrEmpty(urlEtiquetadoInteligente) && !urlEtiquetadoInteligente.EndsWith("/"))
                {
                    urlEtiquetadoInteligente = $"{urlEtiquetadoInteligente}/";
                }
            }
            return urlEtiquetadoInteligente;
        }

        public string ObtenerIpServidorFTP()
        {
            if (string.IsNullOrEmpty(ipServidorFTP))
            {
                if (EnvironmentVariables.Contains("serverIP"))
                {
                    ipServidorFTP = EnvironmentVariables["serverIP"] as string;
                }
                else
                {
                    ipServidorFTP = Configuration["serverIP"];
                }
            }
            return ipServidorFTP;
        }

        public int ObtenerMinPortFTP()
        {
            if (minPort == 0)
            {
                string port;
                if (EnvironmentVariables.Contains("minPort"))
                {
                    port = EnvironmentVariables["minPort"] as string;

                }
                else
                {
                    port = Configuration["minPort"];
                }
                minPort = int.Parse(port);
            }
            return minPort;
        }

        public int ObtenerMaxPortFTP()
        {
            if (maxPort == 0)
            {
                string port;
                if (EnvironmentVariables.Contains("maxPort"))
                {
                    port = EnvironmentVariables["maxPort"] as string;

                }
                else
                {
                    port = Configuration["maxPort"];
                }
                maxPort = int.Parse(port);
            }
            return maxPort;
        }

    }
}
