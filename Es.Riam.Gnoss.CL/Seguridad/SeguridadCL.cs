using BeetleX.Redis;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace Es.Riam.Gnoss.CL.Seguridad
{
    public class SeguridadCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Seguridad" };
        private const string CLAVE_CACHE_BLOQUEO_IP = "Lokut_TooManyRequest_";
        private const string CLAVE_CACHE_PETICION = "Lokut_Request_";
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private ConfigService mConfigService;
        private string mRedisIP;
        private int mRedisDB;

        #endregion

        public SeguridadCL( EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<SeguridadCL> logger, ILoggerFactory loggerFactory)
            :base( entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mRedisIP = mConfigService.ObtenerConexionRedisIPMaster("liveUsuarios");
            mRedisDB = mConfigService.ObtenerConexionRedisBD("liveUsuarios");
        }

        #region Métodos

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP"></param>
        /// <returns></returns>
        public int ObtenerNumIntentosIP(string IP)
        {
            string rawKey = string.Concat("NumIntentos_", IP);

            // Compruebo si está en la caché
            int? numIntentos = ObtenerObjetoDeCache(rawKey, typeof(int)) as int?;
            if (!numIntentos.HasValue)
            {
                numIntentos = 0;
            }

            return numIntentos.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP"></param>
        public void AumentarNumIntentosIP(string IP)
        {
            string rawKey = string.Concat("NumIntentos_", IP);

            // Compruebo si está en la caché
            int? numIntentos = ObtenerObjetoDeCache(rawKey, typeof(int)) as int?;
            if (!numIntentos.HasValue)
            {
                numIntentos = 0;
            }

            double duracion = 60;
            if (numIntentos == 2) { duracion = 60 * 60; }
            AgregarObjetoCache(rawKey, numIntentos + 1, duracion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pToken"></param>
        /// <param name="pCaptcha"></param>
        /// <returns></returns>
        public bool ValidarTokenCaptcha(string pToken, string pCaptcha)
        {
            string rawKey = string.Concat("Token_Captcha_", pToken);

            // Compruebo si está en la caché
            string captcha = ObtenerObjetoDeCache(rawKey, typeof(string)) as string;
            if (captcha != null && pCaptcha == captcha)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pToken"></param>
        /// <param name="pCaptcha"></param>
        public void AgregarTokenCaptcha(string pToken, string pCaptcha)
        {
            string rawKey = string.Concat("Token_Captcha_", pToken);

            ///Añadimos una cache con la misma duracion que el token
            AgregarObjetoCache(rawKey, pCaptcha, 60 * 20);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pToken"></param>
        public void EliminarCaptchaToken(string pToken)
        {
            string rawKey = string.Concat("Token_Captcha_", pToken);

            InvalidarCache(rawKey);
        }
        public void ObtenerConexionAfinidad(string pTokenAfinidad)
        {
            //string conexionAfinidadVirtuoso = (string)ObtenerObjetoDeCache("TokenAfinidad_" + pTokenAfinidad);

            //if (string.IsNullOrEmpty(conexionAfinidadVirtuoso))
            //{
            //    conexionAfinidadVirtuoso = BaseAD.ObtenerCadenaConexionVirtuoso("virtuoso", new List<string>());

            //    AgregarObjetoCache("TokenAfinidad_" + pTokenAfinidadAfinidadVirtuoso, 60);
            //}

            //if (!string.IsNullOrEmpty(conexionAfinidadVirtuoso))
            //{
            //    if(BaseAD.EsConexionHAPROXY("virtuoso"AfinidadVirtuoso))
            //    {
            //        conexionAfinidadVirtuoso = BaseAD.obtenerNombreConexionReplicaAleatoria();
            //    }

            //    UtilPeticion.AgregarObjetoAPeticionActual("afinidadVirtuoso"AfinidadVirtuoso);
            //}
        }

        public bool ComprobarSiSeSuperaLimiteDePeticiones(int pMaxPeticiones, int pVentanaTiempo, int pTiempoBloqueado, string pIP, Guid pAsistenteID)
        {
            try
            {
                bool puedeHacerPeticion = false;
                ConnectionMultiplexer conexion = ConnectionMultiplexer.Connect($"{mRedisIP},defaultDatabase={mRedisDB}");
                IDatabase db = conexion.GetDatabase();
                string claveBloqueo = GenerarClaveParaLimitadorDePeticiones(CLAVE_CACHE_BLOQUEO_IP, pAsistenteID, pIP);
                
                if (ComprobarSiYaSuperoElLimiteDePeticiones(db, claveBloqueo))
                {
                    puedeHacerPeticion = false;
                }
                else
                {
                    string clavePeticion = GenerarClaveParaLimitadorDePeticiones(CLAVE_CACHE_PETICION, pAsistenteID, pIP);
                    bool estaLimitado = ((int)db.ScriptEvaluate(SlidingRateLimiterScript, new { key = clavePeticion, window = pVentanaTiempo, max_requests = pMaxPeticiones })) == 1;
                    if (estaLimitado)
                    {
                        db.StringSet($"{claveBloqueo}", "Bloqueado", TimeSpan.FromMinutes(pTiempoBloqueado));
                        puedeHacerPeticion = false;
                    }
                    else
                    {
                        puedeHacerPeticion = true;
                    }
                }

                conexion.Close();   

                return puedeHacerPeticion;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error al comprobar el limite de peticiones de la IP '{pIP}' y el asistente '{pAsistenteID}'. Los límites configurados son: Ventana de tiempo -> {pVentanaTiempo}, Número máximo de peticiones -> {pMaxPeticiones}, Tiempo de bloqueo -> {pTiempoBloqueado}", mlogger);
                return false;
            }   
        }

        public string GenerarClaveParaLimitadorDePeticiones(string pClavePrincipal, Guid pAsistenteID, string pIP)
        {
            string clave = $"{pClavePrincipal}{pIP}_{pAsistenteID.ToString().ToLower()}";
            return clave;
        }

        public bool ComprobarSiYaSuperoElLimiteDePeticiones(IDatabase pDB, string pClave)
        {
            string existeClave = pDB.StringGet(pClave);
            if (!string.IsNullOrEmpty(existeClave))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Propiedades

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
        ~SeguridadCL()
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
