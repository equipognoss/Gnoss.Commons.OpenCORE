using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("GrupoIdentidades")]
    public partial class GrupoIdentidades
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GrupoIdentidades()
        {
            GrupoIdentidadesOrganizacion = new HashSet<GrupoIdentidadesOrganizacion>();
            GrupoIdentidadesParticipacion = new HashSet<GrupoIdentidadesParticipacion>();
        }

        [Key]
        public Guid GrupoID { get; set; }

        [Required]
        [StringLength(300)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(300)]
        public string NombreCorto { get; set; }

        [Required (AllowEmptyStrings = true)]
        public string Descripcion { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaBaja { get; set; }

        public string Tags { get; set; }

        public bool Publico { get; set; }

        public bool PermitirEnviarMensajes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoIdentidadesOrganizacion> GrupoIdentidadesOrganizacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoIdentidadesParticipacion> GrupoIdentidadesParticipacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoIdentidadesProyecto> GrupoIdentidadesProyecto { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolGrupoIdentidades> RolGrupoIdentidades { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<EstadoGrupo> EstadoGrupo { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<TransicionGrupo> TransicionGrupo { get; set; }		
	}
}
