using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoServicioExterno")]
    public partial class ProyectoServicioExterno
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(150)]
        public string NombreServicio { get; set; }

        [Required]
        public string UrlServicio { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
