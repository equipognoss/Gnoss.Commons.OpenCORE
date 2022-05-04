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
    [Table("CatTesauroAgCatTesauro")]
    public partial class CatTesauroAgCatTesauro
    {
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid CategoriaSuperiorID { get; set; }

        [Column(Order = 2)]
        public Guid CategoriaInferiorID { get; set; }

        public short Orden { get; set; }

        public virtual CategoriaTesauro CategoriaTesauro { get; set; }
        public virtual CategoriaTesauro CategoriaTesuaro1 { get; set; }
    }
}
