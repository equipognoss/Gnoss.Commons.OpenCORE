using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [NotMapped]
    public class DocumentoWebAgCatTesauroConVinculoTesauroID : DocumentoWebAgCatTesauro
    {
        public Guid VinculoTesauroID { get; set; }
        public int TipoTesauro { get; set; }
    }
}
