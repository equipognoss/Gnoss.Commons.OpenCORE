using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoLoginConfiguracion")]
    public partial class ProyectoLoginConfiguracion
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Required]
        public string Mensaje { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
