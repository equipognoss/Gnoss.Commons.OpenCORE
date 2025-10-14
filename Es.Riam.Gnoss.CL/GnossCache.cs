using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProtoBuf.WellKnownTypes;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL
{

    /// <summary>
    /// Clase gen�rica para acceder a memcached
    /// </summary>
    public class GnossCache
    {
        private EntityContext _entityContext;
        private LoggingService _loggingService;
        private RedisCacheWrapper _redisCacheWrapper;
        private ConfigService _configService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public GnossCache(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GnossCache> logger, ILoggerFactory loggerFactory)
        {
            _entityContext = entityContext;
            _loggingService = loggingService;
            _configService = configService;
            _redisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pDuracion">Duraci�n de la cach� (en segundos)</param>
        public void AgregarObjetoCache(string pClave, object pValor, double pDuracion = 0)
        {
            object objetoCache = pValor;
            //if (!(pValor is KeyValuePair<object, double>))
            //{
            //    objetoCache = new KeyValuePair<object, double>(pValor, pDuracion);
            //}

            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            gnossCache.AgregarObjetoCache(pClave, objetoCache, pDuracion);
            gnossCache.Dispose();
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pDuracion">Duraci�n de la cach�</param>
        public void AgregarObjetoCacheLocal(Guid pProyectoID, string pClave, object pValor, bool pObtenerParametroSiempre = false, DateTime? pExpirationDate = null)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            gnossCache.AgregarObjetoCacheLocal(pProyectoID, pClave, pValor, pObtenerParametroSiempre, pExpirationDate);
            gnossCache.Dispose();
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la cach�, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCache(string pClave)
        {
            return ObtenerDeCache(pClave, false);
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la cach�, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCache(string pClave, bool pGenerarClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);

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
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pTipo">Especifica el tipo con el que se deserializar�</param>
        /// <returns>Devuelve el objeto almacenado en la cach�, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerObjetoDeCache(string pClave, Type pTipo)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            object item = gnossCache.ObtenerObjetoDeCache(pClave,pTipo);
            gnossCache.Dispose();

            return item;
        }

        /// <summary>
        /// Comprueba si una clave existe en cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve si el objeto est� almacenado en la cach�, o false si no existe nada con esa clave o ha caducado</returns>
        public bool ExisteClaveEnCache(string pClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            bool existeEnCache = gnossCache.ExisteClaveEnCache(pClave);
            gnossCache.Dispose();

            return existeEnCache;
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la cach�, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCacheLocal(string pClave)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            object item = gnossCache.ObtenerObjetoDeCacheLocal(pClave);
            gnossCache.Dispose();

            return item;
        }

        /// <summary>
        /// Invalida un objeto de la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        public void InvalidarDeCache(string pClave, bool pGenerarClave = false)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            if (pGenerarClave)
            {
                pClave = gnossCache.ObtenerClaveCache(pClave);
            }

            gnossCache.InvalidarDeCache(pClave.ToLower());
            gnossCache.Dispose();
        }

        /// <summary>
        /// Aumenta la versi�n del diccionario de control de las cach�s locales.
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
        /// Aumenta la versi�n del diccionario de control de las cach�s locales.
        /// </summary>
        public void RecalcularRutasProyecto(Guid pProyectoID)
        {
            AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_RUTAS_PESTANYAS + pProyectoID, Guid.NewGuid());
        }

        /// <summary>
        /// Aumenta la versi�n del diccionario de control de las cach�s locales.
        /// </summary>
        public void RecalcularRedirecciones()
        {
            Guid claveRecalculoRutas = Guid.NewGuid();

            AgregarObjetoCache(GnossCacheCL.CLAVE_RECALCULO_RUTAS, claveRecalculoRutas);
            _redisCacheWrapper.Cache.Set(GnossCacheCL.CLAVE_RECALCULO_RUTAS, claveRecalculoRutas);
        }

        /// <summary>
        /// Le aumenta la caducidad a un objeto en cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pDuracion">Duraci�n de la cach� (en segundos)</param>
        public void AgregarCaducidadAObjetoCache(string pClave, double pDuracion)
        {
            GnossCacheCL gnossCache = new GnossCacheCL(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            pClave = gnossCache.ObtenerClaveCache(pClave);
            gnossCache.AgregarCaducidadAObjetoCache(pClave, pDuracion);
            gnossCache.Dispose();
        }
    }

    /// <summary>
    /// Cach� generica de GNOSS
    /// </summary>
    public partial class GnossCacheCL : BaseCL
    {

        public GnossCacheCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GnossCacheCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
        }

        public GnossCacheCL(string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GnossCacheCL> logger, ILoggerFactory loggerFactory)
            : base(pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
        }

        /// <summary>
        /// Constructor para VistaVirtualCL con par�metros
        /// </summary>
        public GnossCacheCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GnossCacheCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
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
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <param name="pValor">Valor</param>
        /// <param name="pDuracion">Duraci�n de la cach�</param>
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
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pClave">Clave</param>
        /// <returns>Devuelve el objeto almacenado en la cach�, o null si no existe nada con esa clave o ha caducado</returns>
        public object ObtenerDeCache(string pClave)
        {
            try
            {
                if (ClienteRedisLectura != null)
                {
                    byte[] bytes = ClienteRedisLectura.Get<byte[]>(pClave).Result;
                    object diccionario = ByteArrayToObject(bytes);
                 
                    if (diccionario != null)
                    {
                        KeyValuePair<object, double> objetoCache = (KeyValuePair<object, double>)diccionario;                 
                        return objetoCache.Key;
                    }
                }
            }
            catch (Exception) { }
            return null;
        }

        /// <summary>
        /// Invalida un objeto de la cach�
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