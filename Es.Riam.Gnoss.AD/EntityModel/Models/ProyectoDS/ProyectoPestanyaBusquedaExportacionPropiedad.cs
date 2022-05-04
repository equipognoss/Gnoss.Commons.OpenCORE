using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoPestanyaBusquedaExportacionPropiedad")]
    public partial class ProyectoPestanyaBusquedaExportacionPropiedad
    {
        [Column(Order = 0)]
        public Guid ExportacionID { get; set; }

        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        public Guid? OntologiaID { get; set; }

        public string Ontologia { get; set; }

        [Required]
        public string Propiedad { get; set; }

        [Required]
        public string NombrePropiedad { get; set; }

        public string DatosExtraPropiedad { get; set; }

        public virtual ProyectoPestanyaBusquedaExportacion ProyectoPestanyaBusquedaExportacion { get; set; }
    }
}
