using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("CatTesauroPermiteTipoRec")]
    public partial class CatTesauroPermiteTipoRec
    {
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid CategoriaTesauroID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoRecurso { get; set; }

        public string OntologiasID { get; set; }
    }
}
