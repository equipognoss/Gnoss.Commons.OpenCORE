using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public class DatoExtraFichasdentidad
    {  
        public Guid IdentidadID { get; set; }
        public string Titulo { get; set; }
        public string Opcion { get; set; }
        public Guid ProyectoID { get; set; }
        public Guid PerfilID { get; set; }
    }
}
