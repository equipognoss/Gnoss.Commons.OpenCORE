using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion
{
    [Serializable]
    [Table("CategoriaTesVinSuscrip")]
    public partial class CategoriaTesVinSuscrip
    {
        [Column(Order = 0)]
        public Guid SuscripcionID { get; set; }

        [Column(Order = 1)]
        public Guid TesauroID { get; set; }

        [Column(Order = 2)]
        public Guid CategoriaTesauroID { get; set; }

        public virtual Suscripcion Suscripcion { get; set; }
    }
}
