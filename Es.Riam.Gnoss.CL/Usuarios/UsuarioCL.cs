using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.CL.Usuarios
{
    public class UsuarioCL : BaseCL, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private UsuarioCN mUsuarioCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Usuario" };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private const string SlidingRateLimiter = @$"
            local current_time = redis.call('TIME')
            local trim_time = tonumber(current_time[1]) - @window
            redis.call('ZREMRANGEBYSCORE', @key, 0, trim_time)
            local request_count = redis.call('ZCARD',@key)

            if request_count < tonumber(@max_requests) then
                redis.call('ZADD', @key, current_time[1], current_time[1] .. current_time[2])
                redis.call('EXPIRE', @key, @window)
                return 0
            end
            return 1
            ";
        private static LuaScript SlidingRateLimiterScript => LuaScript.Prepare(SlidingRateLimiter);
        private const string CLAVE_USUARIO_BLOQUEADO = "LoginUsuarioEstaBloqueado_";
        private const string CLAVE_INTENTOS_LOGIN_USUARIO = "IntentosDeLoginUsuario_";
        private string mRedisIP;
        private int mRedisDB;
        #endregion

        public UsuarioCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<UsuarioCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisIP = mConfigService.ObtenerConexionRedisIPMaster("liveUsuarios");
            mRedisDB = mConfigService.ObtenerConexionRedisBD("liveUsuarios");
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #region Métodos

        /// <summary>
        /// Elimina la caché de usuarios de una organizacion con sus identidades
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        public void EliminarCacheUsuariosCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            string rawKey = string.Concat("UsuariosDeOrganizacionCargaLigeraParaFiltros_", pOrganizacionID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Agrega la caché de usuarios de una organizacion con sus identidades
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        public DataWrapperUsuario ObtenerCacheUsuariosCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("UsuariosDeOrganizacionCargaLigeraParaFiltros_", pOrganizacionID);

            // Compruebo si está en la caché
            DataWrapperUsuario dataWrapperUsuario = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperUsuario)) as DataWrapperUsuario;
            if (dataWrapperUsuario == null)
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                // Si no está, lo cargo y lo almaceno en la caché
                dataWrapperUsuario = usuarioCN.CargarUsuariosDeOrganizacionCargaLigeraParaFiltros(pOrganizacionID);
                if (dataWrapperUsuario != null)
                {
                    dataWrapperUsuario.CargaRelacionesPerezosasCache();
                }
                usuarioCN.Dispose();
                AgregarObjetoCache(rawKey, dataWrapperUsuario, 600); //La clave dura 10 minutos
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperUsuario;
        }

        /// <summary>
        /// Datos de login de red social
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        public void GuardarDatosRedSocial(string pID, Dictionary<string, object> pDatos)
        {
            string rawKey = string.Concat("idRedSocial_", pID);
            AgregarObjetoCache(rawKey, pDatos, 7200); //La clave caduca a las 2 horas
        }

        /// <summary>
        /// Obtener datos de login de red social
        /// </summary>
        public Dictionary<string, object> ObtenerDatosRedSocial(string pID)
        {
            Dictionary<string, object> datos = null;

            string rawKey = string.Concat("idRedSocial_", pID);

            // Compruebo si está en la caché
            datos = ObtenerObjetoDeCache(rawKey, typeof(Dictionary<string, object>)) as Dictionary<string, object>;
            return datos;
        }

        /// <summary>
        /// Obtener Redireccion Usuario
        /// </summary>
        /// <param name="pIdioma">Idioma</param>
        public string ObtenerRedireccionUsuario(Guid pUsuarioID)
        {
            string rawKey = string.Concat("UsuarioRedireccion_", pUsuarioID);
            string redireccion = ObtenerObjetoDeCache(rawKey, typeof(string)) as string;
            if (redireccion == null)
            {
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                redireccion = UsuarioCN.ObtenerUrlRedirect(pUsuarioID);
                usuarioCN.Dispose();

                if (string.IsNullOrEmpty(redireccion))
                {
                    AgregarObjetoCache(rawKey, string.Empty, 0);
                }
            }
            return redireccion;
        }

        /// <summary>
        /// Comprueba si un usuario puede loguearse o esta bloqueado por haberlo intentado demasiadas veces sin exito
        /// </summary>
        /// <param name="pUsuarioID"></param>
        /// <returns></returns>
        public EstadoLoginUsuario ComprobarSiUsuarioPuedeHacerLogin(Guid pUsuarioID)
        {         
            ConnectionMultiplexer conexion = ConnectionMultiplexer.Connect($"{mRedisIP},defaultDatabase={mRedisDB}");
            IDatabase db = conexion.GetDatabase();
            int ventanaDeTiempoPeticiones = mConfigService.ObtenerVentanaTiempoLogin();
            int maximoPeticiones = mConfigService.ObtenerNumMaxPeticionesLogin();
            string estaBloqueado = db.StringGet($"{CLAVE_USUARIO_BLOQUEADO}{pUsuarioID}");
            if (!string.IsNullOrEmpty(estaBloqueado))
            {
                conexion.Close();
                return EstadoLoginUsuario.Bloqueado;
            }

            string claveCache = $"{CLAVE_INTENTOS_LOGIN_USUARIO}{pUsuarioID}";
            bool limited = ((int)db.ScriptEvaluate(SlidingRateLimiterScript, new { key = claveCache, window = ventanaDeTiempoPeticiones, max_requests = maximoPeticiones })) == 1;
            if (limited)
            {
                db.StringSet($"{CLAVE_USUARIO_BLOQUEADO}{pUsuarioID}", "Bloqueado", TimeSpan.FromMinutes(30));
                conexion.Close();
                return EstadoLoginUsuario.Bloqueado;
            }

            return EstadoLoginUsuario.PuedeIntentarHacerLogin;
        }

        /// <summary>
        /// Elimina de cache las claves que bloquean el login del usuario
        /// </summary>
        /// <param name="pUsuarioID"></param>
        public void DesbloquearUsuario(Guid pUsuarioID)
        {
            ConnectionMultiplexer conexion = ConnectionMultiplexer.Connect($"{mRedisIP},defaultDatabase={mRedisDB}");
            IDatabase db = conexion.GetDatabase();
            db.KeyDelete($"{CLAVE_USUARIO_BLOQUEADO}{pUsuarioID}");
            db.KeyDelete($"{CLAVE_INTENTOS_LOGIN_USUARIO}{pUsuarioID}");
            conexion.Close();
        }

        /// <summary>
        /// Elimina de cache el contador de intentos de login de un usuario
        /// </summary>
        /// <param name="pUsuarioID"></param>
        public void ReiniciarNumeroIntentosDeLoginUsuario(Guid pUsuarioID)
        {
            ConnectionMultiplexer conexion = ConnectionMultiplexer.Connect($"{mRedisIP},defaultDatabase={mRedisDB}");
            IDatabase db = conexion.GetDatabase();
            db.KeyDelete($"{CLAVE_INTENTOS_LOGIN_USUARIO}{pUsuarioID}");
            conexion.Close();
        }
        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected UsuarioCN UsuarioCN
        {
            get
            {
                if (mUsuarioCN == null)
                {
                    mUsuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                }

                return mUsuarioCN;
            }
        }

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~UsuarioCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }
        }

        #endregion
    }
}
