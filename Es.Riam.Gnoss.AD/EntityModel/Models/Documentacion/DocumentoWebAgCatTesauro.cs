using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoWebAgCatTesauro")]
    public partial class DocumentoWebAgCatTesauro
    {
        public DateTime? Fecha { get; set; }

        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid CategoriaTesauroID { get; set; }

        [Column(Order = 2)]
        public Guid BaseRecursosID { get; set; }

        [Column(Order = 3)]
        public Guid DocumentoID { get; set; }
        
        public virtual CategoriaTesauro CategoriaTesauro { get; set; }
    }
}
