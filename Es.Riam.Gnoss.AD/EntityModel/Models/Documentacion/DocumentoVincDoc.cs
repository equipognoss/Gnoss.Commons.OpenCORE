using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoVincDoc")]
    public partial class DocumentoVincDoc
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid DocumentoVincID { get; set; }

        public Guid IdentidadID { get; set; }

        public DateTime Fecha { get; set; }

        public virtual Documento Documento { get; set; }

        public virtual Documento Documento1 { get; set; }
    }
}
