using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.CL.RelatedVirtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using OpenLink.Data.Virtuoso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.OpenReplication
{
    public class ServicesVirtuosoAndBidirectionalReplicationOpen : IServicesUtilVirtuosoAndReplication
    {
        RelatedVirtuosoCL mRelatedVirtuosoCL;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ServicesVirtuosoAndBidirectionalReplicationOpen(ConfigService configService, LoggingService loggingService, RelatedVirtuosoCL relatedVirtuosoCL, ILogger<ServicesVirtuosoAndBidirectionalReplicationOpen> logger, ILoggerFactory loggerFactory) : base(configService, loggingService, logger, loggerFactory)
        {
            mRelatedVirtuosoCL = relatedVirtuosoCL;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
        public override string ConexionAfinidadVirtuoso
        {
            get
            {
                return "";
            }
        }

        public override string ConexionAfinidad
        {
            get
            {
                return "";
            }
        }

        public override bool ControlarErrorVirtuosoConection()
        {
            DateTime horaActual = DateTime.Now;
            bool estaOperativo = ServidorOperativo();
            while (!estaOperativo && DateTime.Now < horaActual.AddSeconds(60))
            {
                Thread.Sleep(1000);
                estaOperativo = ServidorOperativo();
            }
            mLoggingService.GuardarLogError("Terminado de checkear el virtuoso", mlogger);
            return estaOperativo;
        }

        public override void InsertarEnReplicacionBidireccional(string pQuery, string pGrafo, short pPrioridad, string pNombreConexionAfinidad, VirtuosoConnectionData pVirtuosoConnectionData, VirtuosoConnection pConexion)
        {
            
        }
    }
}
