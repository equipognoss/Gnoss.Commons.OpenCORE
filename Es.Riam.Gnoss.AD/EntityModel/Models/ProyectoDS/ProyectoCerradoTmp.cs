using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoCerradoTmp")]
    public partial class ProyectoCerradoTmp
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Required]
        [StringLength(2000)]
        public string Motivo { get; set; }

        public DateTime FechaCierre { get; set; }

        public DateTime FechaReapertura { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
