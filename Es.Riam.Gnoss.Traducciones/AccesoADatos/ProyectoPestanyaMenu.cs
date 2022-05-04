namespace Traduccion.AccesoADatos
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ProyectoPestanyaMenu")]
    public partial class ProyectoPestanyaMenu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoPestanyaMenu()
        {
            ProyectoPestanyaMenu1 = new HashSet<ProyectoPestanyaMenu>();
        }

        [Key]
        public Guid PestanyaID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid? PestanyaPadreID { get; set; }

        public short TipoPestanya { get; set; }

        public string Nombre { get; set; }

        public string Ruta { get; set; }

        public short Orden { get; set; }

        public bool NuevaPestanya { get; set; }

        public bool Visible { get; set; }

        public short Privacidad { get; set; }

        public string HtmlAlternativo { get; set; }

        public string IdiomasDisponibles { get; set; }

        public string Titulo { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCortoPestanya { get; set; }

        public bool VisibleSinAcceso { get; set; }

        public string CSSBodyClass { get; set; }

        public bool Activa { get; set; }

        [StringLength(500)]
        public string MetaDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoPestanyaMenu> ProyectoPestanyaMenu1 { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu2 { get; set; }
    }
}
