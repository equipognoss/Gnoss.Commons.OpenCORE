using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Migrations.EntityContextMigrations;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Live;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Notificaciones;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.Controles
{
    public class ControladorNotificacionesPersonalizadas : ControladorBase
    {
        private EntityContextBASE mEntityContextBASE;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ControladorNotificacionesPersonalizadas(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorNotificacionesPersonalizadas> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContextBASE = entityContextBASE;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public List<NotificacionesModel> ObtenerNotificaciones(Guid pPerfil)
        {
            string TITULOCACHEDEFAULT = $"WebNotificationDefault_{pPerfil}_";
            string TITULOCACHEHTML = $"WebNotificationHtml_{pPerfil}_";
            List<string> clavesCache = new List<string>();
            List<NotificacionesModel> notificacionesModelList = new List<NotificacionesModel>();
            List<NotificacionesModel> notificacionesSinLeer = new List<NotificacionesModel>();

            GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);

            clavesCache = gnossCacheCL.ObtenerClavesCacheQueContengaCadena(TITULOCACHEDEFAULT);

            foreach (string clave in clavesCache)
            {
                NotificacionDefault not = (NotificacionDefault)gnossCacheCL.ObtenerObjetoDeCache(clave, typeof(NotificacionDefault));
                NotificacionesModel notificacionesModel = new NotificacionesModel(clave, not.contenidoNotificacion, not.idPerfil, not.fechaNotificacion, not.leida, not.urlNotificacion);
                notificacionesModelList.Add(notificacionesModel);
            }

            clavesCache = gnossCacheCL.ObtenerClavesCacheQueContengaCadena(TITULOCACHEHTML);

            foreach (string clave in clavesCache)
            {
                NotificationHtml not = (NotificationHtml)gnossCacheCL.ObtenerObjetoDeCache(clave, typeof(NotificationHtml));
                NotificacionesModel notificacionesModel = new NotificacionesModel(clave, not.html, not.perfilID, not.fechaNotificacion, not.leida);
                notificacionesModelList.Add(notificacionesModel);
            }

            notificacionesModelList.OrderByDescending(x => DateTime.Parse(x.fechaNotificacion));
            return notificacionesModelList;
        }
        public List<NotificacionesModel> ObtenerNotificacionesSinLeer(Guid pPerfil)
        {
            string TITULOCACHEDEFAULT = $"WebNotificationDefault_{pPerfil}_";
            string TITULOCACHEHTML = $"WebNotificationHtml_{pPerfil}_";
            List<string> clavesCache = new List<string>();
            List<NotificationHtml> notificacioneshtml = new List<NotificationHtml>();
            List<NotificacionesModel> notificacionesModelList = new List<NotificacionesModel>();
            List<NotificacionesModel> notificacionesSinLeer = new List<NotificacionesModel>();

            GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);

            clavesCache = gnossCacheCL.ObtenerClavesCacheQueContengaCadena(TITULOCACHEDEFAULT);

            foreach (string clave in clavesCache)
            {
                NotificacionDefault not = (NotificacionDefault)gnossCacheCL.ObtenerObjetoDeCache(clave, typeof(NotificacionDefault));
                NotificacionesModel notificacionesModel = new NotificacionesModel(clave,not.contenidoNotificacion,not.idPerfil,not.fechaNotificacion,not.leida,not.urlNotificacion);
                notificacionesModelList.Add(notificacionesModel);
            }

            clavesCache = gnossCacheCL.ObtenerClavesCacheQueContengaCadena(TITULOCACHEHTML);

            foreach (string clave in clavesCache)
            {
                NotificationHtml not = (NotificationHtml)gnossCacheCL.ObtenerObjetoDeCache(clave, typeof(NotificationHtml));
                NotificacionesModel notificacionesModel = new NotificacionesModel(clave, not.html,not.perfilID,not.fechaNotificacion,not.leida);
                notificacionesModelList.Add(notificacionesModel);
            }

            foreach (var item in notificacionesModelList)
            {
                
                if (!item.leida)
                {
                    notificacionesSinLeer.Add(item);
                }
            }
             
            return notificacionesSinLeer;
        }
        public NotificacionesModel ObtenerNotificacionPorClave(string claveCache)
        {
            GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            NotificacionesModel notificacionMostrar = (NotificacionesModel) gnossCacheCL.ObtenerDeCache(claveCache);
            return notificacionMostrar;
        }
        public void EliminarNotificacionesLeidas(string claveNotificacionLeida)
        {
            GnossCacheCL gnossCacheCL = new GnossCacheCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCacheCL>(), mLoggerFactory);
            gnossCacheCL.InvalidarCache(claveNotificacionLeida);

        }
    }
}
