using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class UtilServicios
    {
        #region Miembros estáticos

        /// <summary>
        /// Dominio de la aplicación
        /// </summary>
        private static string mDominoAplicacion = null;

        /// <summary>
        /// Url Intragnoss
        /// </summary>
        private static string mUrlIntragnoss = null;

        /// <summary>
        /// Fila de parámetros de aplicación
        /// </summary>
        //private static ParametroAplicacionDS mParametrosAplicacionDS = null;
        private static List<AD.EntityModel.ParametroAplicacion> mParametrosAplicacionDS = null;


        private static List<string> DOMINIOS_PERMITIDOS_CORS = new List<string>();
		public static string DOMINIO_DEFECTO = "";
		public static string IDIOMA_PRINCIPAL_DOMINIO = "es";
		#endregion

		private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private static ILoggerFactory mLoggerFactory;
        public UtilServicios(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<UtilServicios> logger,ILoggerFactory loggerFactory)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Métodos generales

        /// <summary>
        /// Envía al cliente un mensaje de error (401 Unauthorized)
        /// </summary>
        public static void AccesoProhibido()
        {
            AccesoProhibido("");
        }

        /// <summary>
        /// Envía al cliente un mensaje de error (401 Unauthorized)
        /// </summary>
        public static void AccesoProhibido(string pMensaje)
        {
            //RedirigirConStatusCode(401, "Unauthorized" + pMensaje);
        }

        /// <summary>
        /// Envía al cliente un mensaje de error particular
        /// </summary>
        /// <param name="pStatusCode">Status code del error (404, 500...)</param>
        /// <param name="pMensaje">Mensaje del error (Unauthorized, Internal Server Error...)</param>
        public static void RedirigirConStatusCode(int pStatusCode, string pMensaje)
        {
            //try
            //{
            //    if (HttpContext.Current != null)
            //    {
            //        HttpContext.Current.Response.StatusCode = pStatusCode;
            //        HttpContext.Current.Response.Status = pStatusCode + " " + pMensaje;
            //        HttpContext.Current.Response.End();
            //    }
            //}
            //catch { }
        }

        /// <summary>
        /// Guarda el log de error y devuleve un error 500
        /// </summary>
        /// <param name="pMensaje">Mensaje del error</param>
        /// <param name="pNombreLog">Nombre del log (solo el nombre: error, errorBots...)</param>
        public void GuardarLog(string pMensaje, string pNombreLog)
        {
            GuardarLog(pMensaje, pNombreLog, true);
        }


        /// <summary>
        /// Guarda el log de error y devuleve un error 500
        /// </summary>
        /// <param name="pMensaje">Mensaje del error</param>
        /// <param name="pNombreLog">Nombre del log (solo el nombre: error, errorBots...)</param>
        public void GuardarLog(string pMensaje, string pNombreLog, bool pLanzarError)
        {
            try
            {
               
                string rutaConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", $"{pNombreLog}_{DateTime.Now.ToString("yyyy-MM-dd")}.log");
                mLoggingService.GuardarLogError(pMensaje, mlogger, rutaConfig);
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, Environment.NewLine + Environment.NewLine + "Error Original: " + Environment.NewLine + pMensaje,mlogger);
            }
            if (pLanzarError)
            {
                RedirigirConStatusCode(500, "Internal Server Error");
            }
        }

        /// <summary>
        /// Envía un mensaje a la cuenta de errores y guarda un log del error
        /// </summary>
        /// <param name="pMensaje">Mensaje del error</param>
        /// <param name="pNombreLog">Nombre del log (solo el nombre: error, errorBots...)</param>
        public void EnviarErrorYGuardarLog(string pMensaje, string pNombreLog, bool pEsBot)
        {
            try
            {
                GuardarLog(pMensaje, pNombreLog);
                //string esBot = "";
                //if (pEsBot)
                //{
                //    esBot += " bot";
                //}

                //GestionNotificaciones.EnviarCorreoError(pMensaje, Version + " " + NombreAplicacion + NombreEquipo + esBot, ParametrosAplicacionDS);
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }
            RedirigirConStatusCode(500, "Internal Server Error");
        }

        /// <summary>
        /// Obtiene la url de Intragnoss
        /// </summary>
        public string UrlIntragnoss
        {
            get
            {
                if (string.IsNullOrEmpty(mUrlIntragnoss))
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL("acid", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    //ParametroAplicacionDS parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacion();
                    // mUrlIntragnoss = (string)parametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'UrlIntragnoss'")[0]["Valor"];
                    List<ParametroAplicacion> parametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    mUrlIntragnoss = parametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.UrlIntragnoss)).ToList().First().Valor;
                }
                return mUrlIntragnoss;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros de aplicación
        /// </summary>
        //private static ParametroAplicacionDS ParametrosAplicacionDS
        private List<ParametroAplicacion> ParametrosAplicacionDS
        {
            get
            {
                if (mParametrosAplicacionDS == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    // mParametrosAplicacionDS = ((ParametroAplicacionDS)paramCL.ObtenerParametrosAplicacion());
                    mParametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                }
                return mParametrosAplicacionDS;
            }
        }

        /// <summary>
        /// Obtiene la versión de la aplicación
        /// </summary>
        public string Version
        {
            get
            {
                return mConfigService.ObtenerVersion(); 
            }
        }

        public bool ComprobacionCambiosCachesLocales(Guid pProyectoID)
        {
            string claveRefrescoCache = GnossCacheCL.CLAVE_REFRESCO_CACHE_LOCAL + pProyectoID;
			
            Guid? idRefrescoCacheRedis = mGnossCache.ObtenerObjetoDeCache(claveRefrescoCache, typeof(Guid?)) as Guid?;
			Guid? idRefrescoCacheLocal = mRedisCacheWrapper.Cache.Get(claveRefrescoCache) as Guid?;

            if (idRefrescoCacheRedis.HasValue && (!idRefrescoCacheLocal.HasValue || !idRefrescoCacheRedis.Value.Equals(idRefrescoCacheLocal.Value)))
            {
                mRedisCacheWrapper.Cache.Set(claveRefrescoCache, idRefrescoCacheRedis.Value);

                if (idRefrescoCacheLocal.HasValue)
                {
                    BaseCL.ActualizarCacheDependencyCacheLocal(pProyectoID, idRefrescoCacheRedis.Value.ToString());
                    return true;
                }
            }
            return false;
        }

        public bool ComprobacionCambiosRutasPestanyas(Guid pProyectoID)
        {
            bool actualizarRutasPestanyas = false;

            string claveRefrescoRutas = GnossCacheCL.CLAVE_REFRESCO_RUTAS_PESTANYAS + pProyectoID;

            Guid? idRefrescoRutasRedis = mGnossCache.ObtenerObjetoDeCache(claveRefrescoRutas, typeof(Guid?)) as Guid?;
            Guid? idRefrescoRutasLocal = mRedisCacheWrapper.Cache.Get(claveRefrescoRutas) as Guid?;

            if (!idRefrescoRutasRedis.HasValue)
            {
                idRefrescoRutasRedis = Guid.NewGuid();
                mGnossCache.AgregarObjetoCache(claveRefrescoRutas, idRefrescoRutasRedis);
            }

            if (!idRefrescoRutasLocal.HasValue || !idRefrescoRutasRedis.Value.Equals(idRefrescoRutasLocal.Value))
            {
                mRedisCacheWrapper.Cache.Set(claveRefrescoRutas, idRefrescoRutasRedis.Value);
                actualizarRutasPestanyas = idRefrescoRutasLocal.HasValue;
            }

            return actualizarRutasPestanyas;
        }

        public bool ComprobacionCambiosRedirecciones()
        {
            // Se estaban produciendo errores al obtener de caché la clave CLAVE_RECALCULO_RUTAS, y estaba continuamente registrando rutas
            //return false;
            bool recalcularRutas = false;

            Guid? idRecalculoRutasRedis = mGnossCache.ObtenerObjetoDeCache(GnossCacheCL.CLAVE_RECALCULO_RUTAS, typeof(Guid?)) as Guid?;
            Guid? idRecalculoRutasLocal = mRedisCacheWrapper.Cache.Get(GnossCacheCL.CLAVE_RECALCULO_RUTAS) as Guid?;

            if (!idRecalculoRutasRedis.HasValue)
            {
                idRecalculoRutasRedis = Guid.NewGuid();
                mGnossCache.AgregarObjetoCache(GnossCacheCL.CLAVE_RECALCULO_RUTAS, idRecalculoRutasRedis);
            }

            if (!idRecalculoRutasLocal.HasValue || !idRecalculoRutasRedis.Value.Equals(idRecalculoRutasLocal.Value))
            {
                mRedisCacheWrapper.Cache.Set(GnossCacheCL.CLAVE_RECALCULO_RUTAS, idRecalculoRutasRedis.Value, DateTime.Now.AddYears(1));

                recalcularRutas = idRecalculoRutasLocal.HasValue;
            }

            return recalcularRutas;
        }

        private bool ComprobacionInvalidarVistasLocalesPorPersonalizacion(Guid pPersonalizacionID)
        {
            if (!pPersonalizacionID.Equals(Guid.Empty))
            {
                string claveCachePersonalizacionRedis = GnossCacheCL.CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS + pPersonalizacionID;

                Dictionary<string, string> diccionarioRefrescoCache = mGnossCache.ObtenerObjetoDeCache(claveCachePersonalizacionRedis, typeof(Dictionary<string, string>)) as Dictionary<string, string>;

                if (diccionarioRefrescoCache != null)
                {
                    Dictionary<string, string> diccionarioRefrescoCacheLocal = mRedisCacheWrapper.Cache.Get(claveCachePersonalizacionRedis) as Dictionary<string, string>;

                    if (diccionarioRefrescoCacheLocal == null || !diccionarioRefrescoCache["ClaveActualizacion"].Equals(diccionarioRefrescoCacheLocal["ClaveActualizacion"]))
                    {
                        mRedisCacheWrapper.Cache.Set(claveCachePersonalizacionRedis, diccionarioRefrescoCache);

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Comprueba si tiene que invalidar las vistas locales
        /// </summary>
        public bool ComprobacionInvalidarVistasLocales(Guid pPersonalizacionID, Guid pPersonalizacionEcosistemaID)
        {            
            try
            {
                bool borrarVistasProyecto = ComprobacionInvalidarVistasLocalesPorPersonalizacion(pPersonalizacionID);

                bool borrarVistasEcosistema = ComprobacionInvalidarVistasLocalesPorPersonalizacion(pPersonalizacionEcosistemaID);

                if (borrarVistasProyecto || borrarVistasEcosistema)
                {
                    return true;
                }
            }
            catch
            {
                //Si hay un error al comprobar si debemos o no invalidar vistas locales no invalidamos.
            }

            return false;
        }

        private bool ComprobacionInvalidarTraduccionesLocalesPorPersonalizacion(Guid pPersonalizacionID)
        {
            if (!pPersonalizacionID.Equals(Guid.Empty))
            {
                string claveCachePersonalizacionRedis = BaseCL.CLAVE_REFRESCO_CACHE_TRADUCCIONES + pPersonalizacionID;
				
                Guid? idActualRedisPersonalizacion = mGnossCache.ObtenerObjetoDeCache(claveCachePersonalizacionRedis, typeof(Guid)) as Guid?;
				Guid? idActualLocalPersonalizacion = mRedisCacheWrapper.Cache.Get(claveCachePersonalizacionRedis) as Guid?;

                if (idActualRedisPersonalizacion.HasValue && (!idActualLocalPersonalizacion.HasValue || !idActualRedisPersonalizacion.Equals(idActualLocalPersonalizacion)))
                {
                    mRedisCacheWrapper.Cache.Set(claveCachePersonalizacionRedis, idActualRedisPersonalizacion);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Comprueba si tiene que invalidar las traduciones locales
        /// </summary>
        public bool ComprobacionInvalidarTraduccionesLocales(Guid pPersonalizacionID, Guid pPersonalizacionEcosistemaID)
        {
            try
            {
                bool invalidarPersonalizacion = ComprobacionInvalidarTraduccionesLocalesPorPersonalizacion(pPersonalizacionID);
                bool invalidarPersonalizacionEcosistema = ComprobacionInvalidarTraduccionesLocalesPorPersonalizacion(pPersonalizacionEcosistemaID);

                return invalidarPersonalizacion || invalidarPersonalizacionEcosistema;
            }
            catch 
            {
                //Si no se puede comprobar si debemos recalcular las traducciones no las recalculamos
            }

            return false;
        }

        public static void CargarDominiosPermitidosCORS(EntityContext entity)
        {
            List<string> dominios = entity.Proyecto.Select(proyecto => proyecto.URLPropia).Distinct().ToList();
            List<string> dominiosSinIdioma = new List<string>();

            foreach(string dominio in dominios)
            {
                string dominioAux = dominio;
                if (dominio.Contains("@"))
                {
                    dominioAux = dominio.Substring(0, dominio.IndexOf("@"));
                }
                DOMINIOS_PERMITIDOS_CORS.Add(dominioAux);
            }
                        
            string dominiosPermitidosCORS = entity.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DominiosPermitidosCORS)).Select(parametro => parametro.Valor).FirstOrDefault();
            if (!string.IsNullOrEmpty(dominiosPermitidosCORS))
            {
                List<string> listaOtrosDominiosPermitdosCORS = dominiosPermitidosCORS.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
                DOMINIOS_PERMITIDOS_CORS.AddRange(listaOtrosDominiosPermitdosCORS);
            }
            string dominioPaginasAdministracion = entity.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.DominioPaginasAdministracion)).Select(parametro => parametro.Valor).FirstOrDefault();
            if (!string.IsNullOrEmpty(dominioPaginasAdministracion))
            {
                DOMINIOS_PERMITIDOS_CORS.Add(dominioPaginasAdministracion);
            }
            DOMINIOS_PERMITIDOS_CORS.Add("http://depuracion.net");
            DOMINIOS_PERMITIDOS_CORS.Add("http://depuracion.net:5003");
        }

        public static bool ComprobarDominioPermitidoCORS(string dominio)
        {
            return DOMINIOS_PERMITIDOS_CORS.Contains(dominio);
        }

		public static void CargarIdiomasPlataforma(Es.Riam.Gnoss.AD.EntityModel.EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, RedisCacheWrapper redisCacheWrapper,ILoggerFactory loggerFactory)
		{
			Dictionary<string, string> listaIdiomas = new Dictionary<string, string>();
            CargarDominio(configService);
			ParametroAplicacionCL paramCL = new ParametroAplicacionCL(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<ParametroAplicacionCL>(), loggerFactory);
			listaIdiomas = paramCL.ObtenerListaIdiomasDictionary();
			if (listaIdiomas.Count == 1 || string.IsNullOrEmpty(DOMINIO_DEFECTO))
			{
				IDIOMA_PRINCIPAL_DOMINIO = listaIdiomas.First().Key;
			}
			else if (listaIdiomas.Count > 1)
			{
				ProyectoCN proyCN = new ProyectoCN(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, loggerFactory.CreateLogger<ProyectoCN>(), loggerFactory);
				IDIOMA_PRINCIPAL_DOMINIO = proyCN.ObtenerIdiomaPrincipalDominio(DOMINIO_DEFECTO);//select proyectoid, URLPropia from Proyecto where URLPropia like '%pruebasiphoneen.gnoss.net@%'

				if (!listaIdiomas.ContainsKey(IDIOMA_PRINCIPAL_DOMINIO))
				{
					IDIOMA_PRINCIPAL_DOMINIO = listaIdiomas.First().Key;
				}
			}
		}

		private static void CargarDominio(ConfigService configService)
		{
			if (string.IsNullOrEmpty(DOMINIO_DEFECTO))
			{
				string dominioConfig = configService.ObtenerDominio();
				if (!string.IsNullOrEmpty(dominioConfig))
				{
					DOMINIO_DEFECTO = dominioConfig;
				}

				if (DOMINIO_DEFECTO.Contains("depuracion.net"))
				{
					DOMINIO_DEFECTO = "";
				}
			}
		}

		#endregion
	}
}
