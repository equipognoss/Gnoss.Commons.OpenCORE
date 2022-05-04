using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaBusquedaExportacionExterna")]
    public partial class ProyectoPestanyaBusquedaExportacionExterna
    {
        [Key]
        public Guid ExportacionID { get; set; }

        public Guid PestanyaID { get; set; }

        [Required]
        public string UrlServicioExterno { get; set; }

        public virtual ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
    }
}
