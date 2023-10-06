using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.CL.RelatedVirtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
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
        public ServicesVirtuosoAndBidirectionalReplicationOpen(ConfigService configService, LoggingService loggingService, RelatedVirtuosoCL relatedVirtuosoCL) : base(configService, loggingService)
        {
            mRelatedVirtuosoCL = relatedVirtuosoCL;
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

        public override bool ControlarErrorVirtuosoConection(string cadenaConexion, string conexionAfinidadVirtuoso)
        {
            DateTime horaActual = DateTime.Now;
            bool estaOperativo = ServidorOperativo();
            while (!estaOperativo && DateTime.Now < horaActual.AddSeconds(60))
            {
                Thread.Sleep(1000);
                estaOperativo = ServidorOperativo();
            }
            mLoggingService.GuardarLogError("Terminado de checkear el virtuoso");
            return estaOperativo;
        }

        public override void InsertarEnReplicacionBidireccional(string pQuery, string pGrafo, short pPrioridad, string pNombreConexionAfinidad, string pCadenaConexion, VirtuosoConnection pConexion)
        {
            
        }
    }
}
