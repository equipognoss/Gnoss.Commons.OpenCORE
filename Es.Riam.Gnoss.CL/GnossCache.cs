using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Caching.Memory;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL
{

    /// <summary>
    /// Clase genérica para acceder a memcached
    /// </summary>
    public class GnossCache
    {
        private EntityContext _entityContext;
        private LoggingService _loggingService;
        private RedisCacheWrapper _redisCacheWrapper;
        private ConfigService _configService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public GnossCache(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            _entityContext = entityContext;
            _loggingService = loggingService;
            _configService = configService;
            _redisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pDuracion">Duración de la caché (en segundos)</param>
        public void AgregarObjetoCache(string pClave, object pValor, double pDuracion = 0)
        {
            object objetoCache = pValor;
            if (!(pValor is KeyValuePair<object, double>))
            {
                objetoCache = new KeyValuePair<object, double>(pValor, pDuracion);
            }

            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            gnossCache.AgregarObjetoCache(pClave, objetoCache, pDuracion);
            gnossCache.Dispose();
        }

        /// <summary>
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pDuracion">Duración de la caché</param>
        public void AgregarObjetoCacheLocal(Guid pProyectoID, string pClave, object pValor, bool pObtenerParametroSiempre = false, DateTime? pExpirationDate = null)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            gnossCache.AgregarObjetoCacheLocal(pProyectoID, pClave, pValor, pObtenerParametroSiempre, pExpirationDate);
            gnossCache.Dispose();
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la caché, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCache(string pClave)
        {
            return ObtenerDeCache(pClave, false);
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la caché, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCache(string pClave, bool pGenerarClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);

            if (pGenerarClave)
            {
                pClave = gnossCache.ObtenerClaveCache(pClave);
            }
            pClave = pClave.ToLower();

            object item = gnossCache.ObtenerDeCache(pClave);
            gnossCache.Dispose();

            return item;
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la caché, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerObjetoDeCache(string pClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            object item = gnossCache.ObtenerObjetoDeCache(pClave);
            gnossCache.Dispose();

            return item;
        }

        /// <summary>
        /// Comprueba si una clave existe en caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve si el objeto está almacenado en la caché, o false si no existe nada con esa clave o ha caducado</returns>
        public bool ExisteClaveEnCache(string pClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            bool existeEnCache = gnossCache.ExisteClaveEnCache(pClave);
            gnossCache.Dispose();

            return existeEnCache;
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la caché, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCacheLocal(string pClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            object item = gnossCache.ObtenerObjetoDeCacheLocal(pClave);
            gnossCache.Dispose();

            return item;
        }

        /// <summary>
        /// Invalida un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        public void InvalidarDeCache(string pClave, bool pGenerarClave = false)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            if (pGenerarClave)
            {
                pClave = gnossCache.ObtenerClaveCache(pClave);
            }

            gnossCache.InvalidarDeCache(pClave.ToLower());
            gnossCache.Dispose();
        }

        /// <summary>
        /// Aumenta la versión del diccionario de control de las cachés locales.
        /// </summary>
        public void VersionarCacheLocal(Guid pProyectoID)
        {
            AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_CACHE_LOCAL + pProyectoID, Guid.NewGuid());
        }

        public void VersionarCacheLocalComunidadesHijas(Guid pProyectoID)
        {
            

            AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_CACHE_LOCAL + pProyectoID, Guid.NewGuid());
        }

        /// <summary>
        /// Aumenta la versión del diccionario de control de las cachés locales.
        /// </summary>
        public void RecalcularRutasProyecto(Guid pProyectoID)
        {
            AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_RUTAS_PESTANYAS + pProyectoID, Guid.NewGuid());
        }

        /// <summary>
        /// Aumenta la versión del diccionario de control de las cachés locales.
        /// </summary>
        public void RecalcularRedirecciones()
        {
            Guid claveRecalculoRutas = Guid.NewGuid();

            AgregarObjetoCache(GnossCacheCL.CLAVE_RECALCULO_RUTAS, claveRecalculoRutas);
            _redisCacheWrapper.Cache.Set(GnossCacheCL.CLAVE_RECALCULO_RUTAS, claveRecalculoRutas);
        }

        /// <summary>
        /// Le aumenta la caducidad a un objeto en caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pDuracion">Duración de la caché (en segundos)</param>
        public void AgregarCaducidadAObjetoCache(string pClave, double pDuracion)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication);
            pClave = gnossCache.ObtenerClaveCache(pClave);
            gnossCache.AgregarCaducidadAObjetoCache(pClave, pDuracion);
            gnossCache.Dispose();
        }
    }

    /// <summary>
    /// Caché generica de GNOSS
    /// </summary>
    public partial class GnossCacheCL : BaseCL
    {

        public GnossCacheCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        public GnossCacheCL(string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        /// <summary>
        /// Constructor para VistaVirtualCL con parámetros
        /// </summary>
        public GnossCacheCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        public override string[] ClaveCache
        {
            get
            {
                string[] array = { "" };
                return array;
            }
        }

        /// <summary>
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pDuracion">Duración de la caché</param>
        public void AgregarACache(string pClave, object pValor, double pDuracion)
        {
            try
            {
                if (pValor != null)
                {
                    KeyValuePair<object, double> objetoCache = new KeyValuePair<object, double>(pValor, pDuracion);
                    var t = ClienteRedisEscritura.Set(pClave, ObjectToByteArray(objetoCache), (int)pDuracion);
                    t.AsTask().Wait();
                    //ClienteRedisEscritura.Set(pClave, ObjectToByteArray(objetoCache));
                    if (pDuracion > 0)
                    {
                        var t1 = ClienteRedisEscritura.Expire(pClave, (int)pDuracion);
                        t1.AsTask().Wait();
                        //ClienteRedisEscritura.Expire(pClave, (int)pDuracion);
                    }
                    //ClienteRedisEscritura.Dispose();
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la caché, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCache(string pClave)
        {
            try
            {
                if (ClienteRedisLectura != null)
                {
                    //var t = ClienteRedisLectura.Get<object>(pClave);
                    //t.AsTask().Wait();
                    byte[] bytes = ClienteRedisLectura.Get<byte[]>(pClave).Result;
                    object diccionario = ByteArrayToObject(bytes);
                    //object diccionario = ClienteRedisLectura.Get<object>(pClave).Result;
                    if (diccionario != null)
                    {
                        KeyValuePair<object, double> objetoCache = (KeyValuePair<object, double>)diccionario;
                        //ClienteRedisLectura.Dispose();
                        return objetoCache.Key;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// Invalida un objeto de la caché
        /// </summary>
        /// <param name="pClave">Clave</param>
        public void InvalidarDeCache(string pClave)
        {
            try
            {

                if (ClienteRedisEscritura.Exists(pClave).Result == 1)
                {
                    ClienteRedisEscritura.Del(pClave);
                }
                //ClienteRedisEscritura.Dispose();
            }
            catch (Exception) { }
        }
    }
}