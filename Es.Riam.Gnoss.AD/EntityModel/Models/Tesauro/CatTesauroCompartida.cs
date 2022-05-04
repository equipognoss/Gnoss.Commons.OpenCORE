using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("CatTesauroCompartida")]
    public partial class CatTesauroCompartida
    {
        [Column(Order = 0)]
        public Guid TesauroOrigenID { get; set; }

        [Column(Order = 1)]
        public Guid CategoriaOrigenID { get; set; }

        [Column(Order = 2)]
        public Guid TesauroDestinoID { get; set; }

        public Guid? CategoriaSupDestinoID { get; set; }

        public short Orden { get; set; }

        public virtual CategoriaTesauro CategoriaTesauro { get; set; }

        public virtual CategoriaTesauro CategoriaTesauro1 { get; set; }
    }
}
