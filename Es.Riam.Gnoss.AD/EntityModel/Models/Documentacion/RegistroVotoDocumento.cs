using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public class RegistroVotoDocumento
    {
        public Guid DocumentoID { get; set; }
        public double Voto { get; set; }
    }
}
