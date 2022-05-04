using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Es.Riam.Gnoss.Web.Controles.ServiciosGenerales
{
    public class ControladorCMS
    {
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private EntityContextBASE mEntityContextBASE;

        public ControladorCMS(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
        }

        public void ActualizarModeloBaseSimple(Guid pPaginaCMSID, Guid pProyectoID, PrioridadBase pPrioridadBase, bool pEliminado)
        {
            // Creamos una fila 
            BasePaginaCMSDS paginaCMSDS = ObtenerFilaPaginaCMS_Base(pPaginaCMSID, pProyectoID, pPrioridadBase, pEliminado);

            BaseComunidadCN brPaginaCMCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, mEntityContextBASE, mConfigService, null);
            brPaginaCMCN.InsertarFilasEnRabbit("ColaTagsPaginaCMS", paginaCMSDS);
        }

        private BasePaginaCMSDS ObtenerFilaPaginaCMS_Base(Guid pPaginaCMSID, Guid pProyectoID, PrioridadBase pPrioridadBase, bool pEliminado)
        {
            int id = -1;

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
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
