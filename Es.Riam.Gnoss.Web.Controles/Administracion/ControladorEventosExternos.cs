using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Parametro.Model;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorEventosExternos: ControladorBase
    {
        #region Miembros

        private Proyecto ProyectoSeleccionado = null;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        public ControladorEventosExternos(Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, EntityContextBASE pEntityContextBASE, ILogger<ControladorEventosExternos> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mEntityContextBASE = pEntityContextBASE;

            ProyectoSeleccionado = pProyecto;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos públicos

        public EventosConfiguradosModel ActualizarEventosExternos(string pEvento, bool pEcosistema)
        {
            EventosConfiguradosModel config = ObtenerConfiguracionEventos(pEcosistema);
            switch (pEvento)
            {
                case "comentarios":
                    config.EventsConfiguration.CommentsActive = !config.EventsConfiguration.CommentsActive;
                    break;
                case "recursos":
                    config.EventsConfiguration.ResourcesActive = !config.EventsConfiguration.ResourcesActive;
                    break;
                case "usuario":
                    config.EventsConfiguration.UsersActive = !config.EventsConfiguration.UsersActive;
                    break;
                case "traducciones":
                    config.EventsConfiguration.TranslationsActive = !config.EventsConfiguration.TranslationsActive;
                    break;
                case "paginasCms":
                    config.EventsConfiguration.PagesCmsActive = !config.EventsConfiguration.PagesCmsActive;
                    break;
                case "componentesCms":
                    config.EventsConfiguration.ComponentsCmsActive = !config.EventsConfiguration.ComponentsCmsActive;
                    break;
            }

            GuardarEventosExternos(config, pEcosistema);

            InvalidarCacheParametroProyecto();
            return config;

        }

        public EventosConfiguradosModel ActualizarConfiguracionEventosExternos()
        {
            EventosConfiguradosModel config = ObtenerConfiguracionEventos(false);

            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);

            if (config == null)
            {
                // Añadimos el parametro en la comunidad que no existe
                EventosConfiguradosModel newConfig = new EventosConfiguradosModel()
                {
                    EventsActive = true,
                    EventsConfiguration = new EventsConfiguration()
                    {
                        CommentsActive = true,
                        ResourcesActive = true,
                        UsersActive = true,
                        TranslationsActive = true,
                        PagesCmsActive = true,
                        ComponentsCmsActive = true
                    }
                };
                config = newConfig;
            }
            else
            {
                config.EventsActive = !config.EventsActive;
            }

            string valor = JsonSerializer.Serialize(config);

            parametroCN.ActualizarParametroEnProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, "EventosConfigurados", valor);

            // Invalidamos la cache
            InvalidarCacheParametroProyecto();

            return config;
        }

        public EventosConfiguradosModel ObtenerConfiguracionEventos(bool pEsEcosistema)
        {
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);

            if (pEsEcosistema) return parametroCN.ObtenerEventosConfigurados(ProyectoAD.MetaProyecto, true);

            return parametroCN.ObtenerEventosConfigurados(ProyectoSeleccionado.Clave, false);

        }

        public void GuardarEventosExternos(EventosConfiguradosModel pEvento, bool pEsEcosistema)
        {
            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);

            string valor = JsonSerializer.Serialize(pEvento);

            if (pEsEcosistema)
            {
                parametroCN.ActualizarParametroEnProyecto(ProyectoAD.MetaProyecto, ProyectoAD.MetaProyecto, "EventosConfiguradosEcosistema", valor);
            }
            else
            {
                parametroCN.ActualizarParametroEnProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID, "EventosConfigurados", valor);
            }

        }

        #endregion

        #region Metodos privados
        private void InvalidarCacheParametroProyecto()
        {
            ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);

            proyectoCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);

            proyectoCL.Dispose();
        }

        #endregion
    }
}
