using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaBusquedaExportacion")]
    public partial class ProyectoPestanyaBusquedaExportacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaBusquedaExportacion()
        {
            ProyectoPestanyaBusquedaExportacionPropiedad = new HashSet<ProyectoPestanyaBusquedaExportacionPropiedad>();
        }

        [Key]
        public Guid ExportacionID { get; set; }

        public Guid PestanyaID { get; set; }

        [Required]
        public string NombreExportacion { get; set; }

        public short Orden { get; set; }

        public string GruposExportadores { get; set; }

        [Required]
        [StringLength(10)]
        public string FormatosExportacion { get; set; }

        public virtual ProyectoPestanyaBusqueda ProyectoPestanyaBusqueda { get; set; }

        public virtual ProyectoPestanyaBusquedaExportacionExterna ProyectoPestanyaBusquedaExportacionExterna { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaBusquedaExportacionPropiedad> ProyectoPestanyaBusquedaExportacionPropiedad { get; set; }
    }
}
