using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cache;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.UtilServiciosWeb;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
	public class ControladorCaches
	{
		private DataWrapperDocumentacion mDataWrapperDocumentacion = null;
		private DataWrapperProyecto mDataWrapperProyecto = null;
		private Dictionary<string, string> mParametroProyecto;

		private Elementos.ServiciosGenerales.Proyecto ProyectoSeleccionado = null;

		private GestorParametroGeneral mParametrosGeneralesDS;
		private ParametroGeneral mFilaParametrosGenerales = null;

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

        public ControladorCaches(Elementos.ServiciosGenerales.Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, GnossCache gnossCache, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorCaches> logger,ILoggerFactory loggerFactory)
		{
			mVirtuosoAD = virtuosoAD;
			mLoggingService = loggingService;
			mEntityContext = entityContext;
			mConfigService = configService;
			mHttpContextAccessor = httpContextAccessor;
			mRedisCacheWrapper = redisCacheWrapper;
			mEntityContextBASE = entityContextBASE;
			mGnossCache = gnossCache;
			mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
			ProyectoSeleccionado = pProyecto;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

		public void GuardarConfiguracionCachesDeBusqueda(bool pCachesDeBusquedasActiva, bool pCachesAnonimas, long pDuracionConsulta, long pTiempoExpiracion, long pTiempoExpiracionCachesDeUsuario, long pTiempoRecalcularCaches)
		{
			try
			{
				ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
				ConfiguracionCachesCostosas configuracionCaches = proyectoCN.ObtenerConfiguracionCachesCostosasDeProyecto(ProyectoSeleccionado.Clave);

				if (configuracionCaches == null)
				{
					CrearConfiguracionCachesDeBusqueda(pCachesDeBusquedasActiva, pCachesAnonimas, pDuracionConsulta, pTiempoExpiracion, pTiempoExpiracionCachesDeUsuario, pTiempoRecalcularCaches);
				}
				else
				{
					ModificarConfiguracionCachesDeBusqueda(pCachesDeBusquedasActiva, pCachesAnonimas, pDuracionConsulta, pTiempoExpiracion, pTiempoExpiracionCachesDeUsuario, pTiempoRecalcularCaches);
				}
			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex, $"Error al guardar las configuraciones de las cachés de búsqueda",mlogger);
			}
		}

		public void CrearConfiguracionCachesDeBusqueda(bool pCachesDeBusquedasActiva, bool pCachesAnonimas, long pDuracionConsulta, long pTiempoExpiracion, long pTiempoExpiracionCachesDeUsuario, long pTiempoRecalcularCaches)
		{
			ConfiguracionCachesCostosas configuracionCaches = new ConfiguracionCachesCostosas();
			configuracionCaches.ProyectoID = ProyectoSeleccionado.Clave;
			configuracionCaches.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
			configuracionCaches.CachesDeBusquedasActivas = pCachesDeBusquedasActiva;
			configuracionCaches.CachesAnonimas = pCachesAnonimas;
			configuracionCaches.DuracionConsulta = pDuracionConsulta;
			configuracionCaches.TiempoRecalcularCaches = pTiempoRecalcularCaches;
			configuracionCaches.TiempoExpiracion = pTiempoExpiracion;
			configuracionCaches.TiempoExpiracionCachesDeUsuario = pTiempoExpiracionCachesDeUsuario;

			mEntityContext.ConfiguracionCachesCostosas.Add(configuracionCaches);
			mEntityContext.SaveChanges();
		}

		public void ModificarConfiguracionCachesDeBusqueda(bool pCachesDeBusquedasActiva, bool pCachesAnonimas, long pDuracionConsulta, long pTiempoExpiracion, long pTiempoExpiracionCachesDeUsuario, long pTiempoRecalcularCaches)
		{
			ConfiguracionCachesCostosas filaConfiguracionCaches = mEntityContext.ConfiguracionCachesCostosas.Where(x => x.ProyectoID.Equals(ProyectoSeleccionado.Clave)).FirstOrDefault();
			if (filaConfiguracionCaches != null)
			{
				filaConfiguracionCaches.ProyectoID = ProyectoSeleccionado.Clave;
				filaConfiguracionCaches.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
				filaConfiguracionCaches.CachesDeBusquedasActivas = pCachesDeBusquedasActiva;
				filaConfiguracionCaches.CachesAnonimas = pCachesAnonimas;
				filaConfiguracionCaches.DuracionConsulta = pDuracionConsulta;
				filaConfiguracionCaches.TiempoRecalcularCaches = pTiempoRecalcularCaches;
				filaConfiguracionCaches.TiempoExpiracion = pTiempoExpiracion;
				filaConfiguracionCaches.TiempoExpiracionCachesDeUsuario = pTiempoExpiracionCachesDeUsuario;

				mEntityContext.SaveChanges();
			}		
		}

		public ConfiguracionCachesCostosas ObtenerConfiguracionCachesDeProyecto(Guid pProyectoID)
		{
			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
			ConfiguracionCachesCostosas configuracionCaches = proyectoCN.ObtenerConfiguracionCachesCostosasDeProyecto(pProyectoID);
			proyectoCN.Dispose();

			return configuracionCaches;

		}
	}
}
