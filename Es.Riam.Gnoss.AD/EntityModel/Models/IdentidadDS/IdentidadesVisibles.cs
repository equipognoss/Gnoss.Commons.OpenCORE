using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    public class IdentidadesVisibles
    {
        public Guid IdentidadID { get; set; }
        public bool EsBuscable { get; set; }
        public bool EsBuscableExternos { get; set; }
    }
}
