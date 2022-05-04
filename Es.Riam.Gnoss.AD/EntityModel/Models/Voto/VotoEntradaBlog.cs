using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Es.Riam.Gnoss.AD.EntityModel.Models.Voto
{
    [Serializable]
    [Table("VotoEntradaBlog")]
    public partial class VotoEntradaBlog
    {
        [Column(Order = 0)]
        public Guid BlogID { get; set; }

        [Column(Order = 1)]
        public Guid EntradaBlogID { get; set; }

        [Column(Order = 2)]
        public Guid VotoID { get; set; }

        public virtual Voto Voto { get; set; }
    }
}
