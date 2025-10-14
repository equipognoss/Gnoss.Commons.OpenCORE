using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{
    public class ControladorCMS
    {
        private readonly LoggingService mLoggingService;
        private readonly VirtuosoAD mVirtuosoAD;
        private readonly EntityContext mEntityContext;
        private readonly ConfigService mConfigService;
        private readonly RedisCacheWrapper mRedisCacheWrapper;
        private readonly EntityContextBASE mEntityContextBASE;
        private readonly IAvailableServices mAvailableServices;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        public ControladorCMS(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IAvailableServices availableServices, ILogger<ControladorCMS> logger, ILoggerFactory loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
            mAvailableServices = availableServices;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public void ActualizarModeloBaseSimple(Guid pPaginaCMSID, Guid pProyectoID, PrioridadBase pPrioridadBase, bool pEliminado)
        {
            if (mAvailableServices.CheckIfServiceIsAvailable(mAvailableServices.GetBackServiceCode(BackgroundService.SearchGraphGenerator), ServiceType.Background))
            {
				// Creamos una fila 
				BasePaginaCMSDS paginaCMSDS = ObtenerFilaPaginaCMS_Base(pPaginaCMSID, pProyectoID, pPrioridadBase, pEliminado);

				BaseComunidadCN brPaginaCMCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, null, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
				brPaginaCMCN.InsertarFilasEnRabbit("ColaTagsPaginaCMS", paginaCMSDS);
			}           
        }

        private BasePaginaCMSDS ObtenerFilaPaginaCMS_Base(Guid pPaginaCMSID, Guid pProyectoID, PrioridadBase pPrioridadBase, bool pEliminado)
        {
            int id = -1;

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            id = proyCL.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);
            proyCL.Dispose();

            BasePaginaCMSDS paginaCMSDS = new BasePaginaCMSDS();

            BasePaginaCMSDS.ColaTagsPaginaCMSRow filaColaPaginaCMS = paginaCMSDS.ColaTagsPaginaCMS.NewColaTagsPaginaCMSRow();

            filaColaPaginaCMS.Estado = (short)EstadosColaTags.EnEspera;
            filaColaPaginaCMS.FechaPuestaEnCola = DateTime.Now;
            filaColaPaginaCMS.TablaBaseProyectoID = id;
            filaColaPaginaCMS.Tags = Constantes.ID_TAG_PAGINA_CMS + pPaginaCMSID.ToString() + Constantes.ID_TAG_PAGINA_CMS;

            if (pEliminado)
            {
                filaColaPaginaCMS.Tipo = 1;
            }
            else
            {
                filaColaPaginaCMS.Tipo = 0;
            }
            filaColaPaginaCMS.Prioridad = (short)pPrioridadBase;
            filaColaPaginaCMS.Prioridad = (short)pPrioridadBase;

            paginaCMSDS.ColaTagsPaginaCMS.AddColaTagsPaginaCMSRow(filaColaPaginaCMS);

            return paginaCMSDS;
        }
    }
}
