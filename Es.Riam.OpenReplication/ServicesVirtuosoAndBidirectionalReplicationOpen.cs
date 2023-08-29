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
                throw new NotImplementedException();
            }
        }

        public override string ConexionAfinidad
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool ControlarErrorVirtuosoConection(string cadenaConexion, string conexionAfinidadVirtuoso)
        {
            throw new NotImplementedException();
        }

        public override void InsertarEnReplicacionBidireccional(string pQuery, string pGrafo, short pPrioridad, string pNombreConexionAfinidad, string pCadenaConexion, VirtuosoConnection pConexion)
        {
            throw new NotImplementedException();
        }
    }
}
