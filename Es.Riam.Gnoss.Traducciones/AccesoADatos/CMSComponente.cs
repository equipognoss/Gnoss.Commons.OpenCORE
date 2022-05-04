namespace Traduccion.AccesoADatos
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CMSComponente")]
    public partial class CMSComponente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CMSComponente()
        {
            CMSPropiedadComponente = new HashSet<CMSPropiedadComponente>();
        }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Key]
        public Guid ComponenteID { get; set; }

        [Required]
        public string Nombre { get; set; }

        public short TipoComponente { get; set; }

        public short TipoCaducidadComponente { get; set; }

        public DateTime? FechaUltimaActualizacion { get; set; }

        public string Estilos { get; set; }

        public bool Activo { get; set; }

        public string IdiomasDisponibles { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCortoComponente { get; set; }

        public bool AccesoPublico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSPropiedadComponente> CMSPropiedadComponente { get; set; }
    }
}
