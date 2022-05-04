using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaExportacionBusqueda")]
    public partial class ProyectoPestanyaExportacionBusqueda
    {
        [Key]
        public Guid ExportacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Required]
        [StringLength(400)]
        public string NombrePestanya { get; set; }

        [Required]
        [StringLength(400)]
        public string NombreExportacion { get; set; }

        public short Orden { get; set; }
    }
}
