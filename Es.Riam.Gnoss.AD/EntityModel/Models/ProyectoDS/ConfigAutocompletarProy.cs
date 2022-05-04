using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Table("ConfigAutocompletarProy")]
    [Serializable]
    public partial class ConfigAutocompletarProy
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(100)]
        public string Clave { get; set; }

        [Required]
        public string Valor { get; set; }

        public Guid? PestanyaID { get; set; }
        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
