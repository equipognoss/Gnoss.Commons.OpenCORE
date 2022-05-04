using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPasoRegistro")]
    public partial class ProyectoPasoRegistro
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        [Required]
        public string PasoRegistro { get; set; }

        [Required]
        public bool Obligatorio { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
