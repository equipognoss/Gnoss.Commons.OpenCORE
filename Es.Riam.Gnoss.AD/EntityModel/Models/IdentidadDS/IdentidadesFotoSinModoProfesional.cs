using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    public class IdentidadesFotoSinModoProfesional
    { 
        public Guid IdentidadID { get; set; }
        
        public string Url { get; set; }
    }
}
