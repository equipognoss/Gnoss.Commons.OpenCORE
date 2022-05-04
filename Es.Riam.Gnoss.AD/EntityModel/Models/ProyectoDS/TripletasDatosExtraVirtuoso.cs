using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public class TripletasDatosExtraVirtuoso
    {   
        public Guid IdentidadID { get; set; }
        public string PredicadorRDF { get; set; }
        public string Opcion { get; set; }
    }
}
