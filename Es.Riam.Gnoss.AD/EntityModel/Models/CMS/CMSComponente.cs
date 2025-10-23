using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{
    [Serializable]
    [Table("CMSComponente")]
    public partial class CMSComponente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CMSComponente()
        {
            CMSBloqueComponente = new HashSet<CMSBloqueComponente>();
            CMSPropiedadComponente = new HashSet<CMSPropiedadComponente>();
            CMSComponenteRolGrupoIdentidades = new HashSet<CMSComponenteRolGrupoIdentidades>();
            CMSComponenteRolIdentidad = new HashSet<CMSComponenteRolIdentidad>();
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

        public Guid? EstadoID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSBloqueComponente> CMSBloqueComponente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSPropiedadComponente> CMSPropiedadComponente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSComponenteRolGrupoIdentidades> CMSComponenteRolGrupoIdentidades { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSComponenteRolIdentidad> CMSComponenteRolIdentidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CMSComponenteVersion> CMSComponenteVersion { get; set; }

		public virtual Estado Estado { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<HistorialTransicionCMSComponente> HistorialTransicionCMSComponente { get; set; }
	}
}
