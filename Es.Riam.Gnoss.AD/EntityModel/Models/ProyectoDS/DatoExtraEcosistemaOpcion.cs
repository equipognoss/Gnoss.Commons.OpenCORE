using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraEcosistemaOpcion")]
    public partial class DatoExtraEcosistemaOpcion
    {
        [Column(Order = 0)]
        public Guid DatoExtraID { get; set; }

        [Column(Order = 1)]
        public Guid OpcionID { get; set; }

        public int Orden { get; set; }

        [Required]
        [StringLength(500)]
        public string Opcion { get; set; }

        public virtual DatoExtraEcosistema DatoExtraEcosistema { get; set; }
    }
}
