using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.Facetado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.InterfacesOpen
{
    public interface IServicesBidirectionalReplication
    {
        public void InsertarEnReplicacionBidireccional(FacetadoAD facetadoAD, ReplicacionAD replicacionAD, string pQuery, string pGrafo, short pPrioridad);
    }
}
