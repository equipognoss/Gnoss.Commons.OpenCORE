using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using OpenLink.Data.Virtuoso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.OpenReplication
{
    public class ServicesVirtuosoAndBidirectionalReplicationOpen : IServicesUtilVirtuosoAndReplication
    {

        public ServicesVirtuosoAndBidirectionalReplicationOpen(ConfigService configService, LoggingService loggingService) : base(configService, loggingService)
        {
        }

        public override void InsertarEnReplicacionBidireccional(string pQuery, string pGrafo, short pPrioridad, string pNombreConexionAfinidad, string pCadenaConexion, VirtuosoConnection pConexion)
        {
            throw new NotImplementedException();
        }
    }
}
