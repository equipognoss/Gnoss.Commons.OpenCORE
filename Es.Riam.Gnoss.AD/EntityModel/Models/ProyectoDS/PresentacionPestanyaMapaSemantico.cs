using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("PresentacionPestanyaMapaSemantico")]
    public partial class PresentacionPestanyaMapaSemantico
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid PestanyaID { get; set; }

        [Column(Order = 3)]
        public Guid OntologiaID { get; set; }

        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        [Required]
        [StringLength(1000)]
        public string Ontologia { get; set; }

        [Required]
        [StringLength(2000)]
        public string Propiedad { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
