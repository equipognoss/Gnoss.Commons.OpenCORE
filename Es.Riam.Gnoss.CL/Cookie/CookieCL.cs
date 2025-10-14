using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cookies;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.Cookie;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.CL.Cookie
{
    public class CookieCL : BaseCL, IDisposable
    {

        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private CookieCN mCookieCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { "Cookie" };

        private const double DURACION_CACHE_COOKIES = 86400; // 60 * 60 * 24 = 1 día


        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para CookieCL
        /// </summary>
        public CookieCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CookieCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para IdentidadCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CookieCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<CookieCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos generales

        public List<CategoriaProyectoCookieViewModel> ObtenerCategoriasProyectoCookie(Guid pProyectoID)
        {
            List<CategoriaProyectoCookieViewModel> listaCategoriaProyectoCookieViewModel = (List<CategoriaProyectoCookieViewModel>)ObtenerObjetoDeCache($"ListaCategoriaProyectoCookie_{pProyectoID}", true, typeof(List<CategoriaProyectoCookieViewModel>));            

            if (listaCategoriaProyectoCookieViewModel == null || listaCategoriaProyectoCookieViewModel.Count == 0)
            {
                listaCategoriaProyectoCookieViewModel = new List<CategoriaProyectoCookieViewModel>();
                List<CategoriaProyectoCookie> listaCategoriaProyectoCookie = CookieCN.ObtenerCategoriasProyectoCookie(pProyectoID);

                foreach (CategoriaProyectoCookie categoriaProyectoCookie in listaCategoriaProyectoCookie)
                {
                    listaCategoriaProyectoCookieViewModel.Add(new CategoriaProyectoCookieViewModel() { CategoriaID = categoriaProyectoCookie.CategoriaID, Descripcion = 
                    categoriaProyectoCookie.Descripcion, EsCategoriaTecnica = categoriaProyectoCookie.EsCategoriaTecnica, Nombre = categoriaProyectoCookie.Nombre, NombreCorto = 
                    categoriaProyectoCookie.NombreCorto, OrganizacionID = categoriaProyectoCookie.OrganizacionID, ProyectoID = categoriaProyectoCookie.ProyectoID });
                }

                AgregarObjetoCache($"ListaCategoriaProyectoCookie_{pProyectoID}", listaCategoriaProyectoCookieViewModel);
            }

            return listaCategoriaProyectoCookieViewModel;
        }

        public void InvalidarCategoriaProyectoCookie(Guid pProyectoID)
        {
            InvalidarCache($"ListaCategoriaProyectoCookie_{pProyectoID}", true);
        }


        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected CookieCN CookieCN
        {
            get
            {
                if (mCookieCN == null)
                {
                    if (string.IsNullOrEmpty(mFicheroConfiguracionBD))
                    {
                        mCookieCN = new CookieCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CookieCN>(), mLoggerFactory);
                    }
                    else
                    {
                        mCookieCN = new CookieCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<CookieCN>(), mLoggerFactory);
                    }
                }

                return mCookieCN;
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
        ~CookieCL()
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
