using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServicioCargaResultadosMVC.Controllers
{
    public class TrazaController : Controller
    {

        private ControladorBase mControladorBase;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public TrazaController(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<TrazaController> logger, ILoggerFactory loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mControladorBase = new ControladorBase(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ControladorBase>(), mLoggerFactory);
        }

        // GET: Traza      
        public ActionResult Index(string txtNombre, string txtPassword)
        {
            if (Request.Method.Equals("GET"))
            {
                EstablecerTextos();

                return View("Index");
            }
            else
            {
                if ((txtNombre.ToLower().Equals("admintrazas")) && (txtPassword.ToLower().Equals("riam123")))
                {
                    HabilitarTraza();
                }
                else
                {
                    ViewBag.Error = "Usuario o contraseña incorrectas, intentalo de nuevo";
                }

                EstablecerTextos();

                return View("Index");
            }
        }

        private void HabilitarTraza()
        {
            LoggingService.TrazaHabilitada = !LoggingService.TrazaHabilitada;

            GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            if (LoggingService.TrazaHabilitada)
            {
                gnossCacheCL.AgregarACache("traza" + mControladorBase.DominoAplicacion, true, 60 * 60 * 72);//72horas
            }
            else
            {
                gnossCacheCL.InvalidarDeCache("traza" + mControladorBase.DominoAplicacion);
            }
        }

        private void EstablecerTextos()
        {
            string estadoTraza = "deshabilitada";
            string botonTraza = "Habilitar traza";

            if (LoggingService.TrazaHabilitada)
            {
                estadoTraza = "HABILITADA";
                botonTraza = "Deshabilitar traza";
            }

            ViewBag.Estado = "La traza está " + estadoTraza;
            ViewBag.Accion= botonTraza;
        }
    }
}