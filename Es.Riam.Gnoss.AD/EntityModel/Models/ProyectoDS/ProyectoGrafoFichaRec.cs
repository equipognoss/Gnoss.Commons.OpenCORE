using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoGrafoFichaRec")]
    public partial class ProyectoGrafoFichaRec
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid GrafoID { get; set; }

        [Required]
        [StringLength(300)]
        public string PropEnlace { get; set; }

        public int? NodosLimiteNivel { get; set; }

        public string Extra { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
