using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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

        #endregion

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public UtilServicios(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
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
                mLoggingService.GuardarLogError(pMensaje, rutaConfig);
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, Environment.NewLine + Environment.NewLine + "Error Original: " + Environment.NewLine + pMensaje);
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
                mLoggingService.GuardarLogError(e);
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
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL("acid", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
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
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
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

        public void ComprobacionCambiosCachesLocales(Guid pProyectoID)
        {
            string claveRefrescoCache = GnossCacheCL.CLAVE_REFRESCO_CACHE_LOCAL + pProyectoID;
            Guid? idRefrescoCacheRedis = mGnossCache.ObtenerObjetoDeCache(claveRefrescoCache) as Guid?;
            Guid? idRefrescoCacheLocal = mRedisCacheWrapper.Cache.Get(claveRefrescoCache) as Guid?;

            if (idRefrescoCacheRedis.HasValue && (!idRefrescoCacheLocal.HasValue || !idRefrescoCacheRedis.Value.Equals(idRefrescoCacheLocal.Value)))
            {
                mRedisCacheWrapper.Cache.Set(claveRefrescoCache, idRefrescoCacheRedis.Value);

                if (idRefrescoCacheLocal.HasValue)
                {
                    BaseCL.ActualizarCacheDependencyCacheLocal(pProyectoID, idRefrescoCacheRedis.Value.ToString());
                }
            }
        }

        public bool ComprobacionCambiosRutasPestanyas(Guid pProyectoID)
        {
            bool actualizarRutasPestanyas = false;

            string claveRefrescoRutas = GnossCacheCL.CLAVE_REFRESCO_RUTAS_PESTANYAS + pProyectoID;

            Guid? idRefrescoRutasRedis = mGnossCache.ObtenerObjetoDeCache(claveRefrescoRutas) as Guid?;
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
            
            Guid? idRecalculoRutasRedis = mGnossCache.ObtenerObjetoDeCache(GnossCacheCL.CLAVE_RECALCULO_RUTAS) as Guid?;
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

                Dictionary<string, string> diccionarioRefrescoCache = mGnossCache.ObtenerObjetoDeCache(claveCachePersonalizacionRedis) as Dictionary<string, string>;

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
            int proceso = System.Diagnostics.Process.GetCurrentProcess().Id;

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

            }

            return false;
        }

        private bool ComprobacionInvalidarTraduccionesLocalesPorPersonalizacion(Guid pPersonalizacionID)
        {
            if (!pPersonalizacionID.Equals(Guid.Empty))
            {
                string claveCachePersonalizacionRedis = GnossCacheCL.CLAVE_REFRESCO_CACHE_TRADUCCIONES + pPersonalizacionID;

                Guid? idActualRedisPersonalizacion = mGnossCache.ObtenerObjetoDeCache(claveCachePersonalizacionRedis) as Guid?;
                Guid? idActualLocalPersonalizacion = mRedisCacheWrapper.Cache.Get(claveCachePersonalizacionRedis) as Guid?;

                if (idActualRedisPersonalizacion.HasValue && (!idActualLocalPersonalizacion.HasValue || !idActualRedisPersonalizacion.Equals(idActualLocalPersonalizacion)))
                {
                    mRedisCacheWrapper.Cache.Set(claveCachePersonalizacionRedis, idActualRedisPersonalizacion);
                    return idActualLocalPersonalizacion.HasValue;
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
            catch { }

            return false;
        }

        #endregion
    }
}
