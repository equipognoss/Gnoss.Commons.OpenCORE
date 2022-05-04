using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;

namespace Es.Riam.Gnoss.CL.ServiciosGenerales
{
    public class PaisCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private PaisCN mPaisCN = null;

        static DataWrapperPais mPaisDW = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.PAISES };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #endregion

        public PaisCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #region Métodos

        /// <summary>
        /// Recupera todos los paises y todas las provincias
        /// </summary>
        /// <returns>Países y provincias</returns>
        public DataWrapperPais ObtenerPaisesProvincias()
        {
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            if (mPaisDW == null)
            {
                mPaisDW = ObtenerObjetoDeCache(null) as DataWrapperPais;
                if (mPaisDW == null)
                {
                    // Si no está, lo cargo y lo almaceno en la caché
                    mPaisDW = PaisCN.ObtenerPaisesProvincias();
                    AgregarObjetoCache(null, mPaisDW);
                }
            }
            mEntityContext.UsarEntityCache = false;
            return mPaisDW;
        }

        /// <summary>
        /// Actualiza los cambios realizados en la lista de paises
        /// </summary>
        /// <param name="pPaisDS">Lista de Paises</param>
        public void ActualizarPaises()
        {
            PaisAD paisAd = new PaisAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            paisAd.ActualizarPaises();
        }

        #endregion        

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected PaisCN PaisCN
        {
            get
            {
                if (mPaisCN == null)
                {
                    mPaisCN = new PaisCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                }

                return mPaisCN;
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
    }
}
