using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("Perfil")]
    public partial class Perfil
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Perfil()
        {
            Identidad = new HashSet<Identidad>();
            Profesor = new HashSet<Profesor>();
            PerfilPersonaOrg = new HashSet<PerfilPersonaOrg>();
        }

        public Guid PerfilID { get; set; }

        [Required]
        [StringLength(300)]
        public string NombrePerfil { get; set; }

        [StringLength(255)]
        public string NombreOrganizacion { get; set; }

        public bool Eliminado { get; set; }

        [StringLength(50)]
        public string NombreCortoOrg { get; set; }

        [StringLength(50)]
        public string NombreCortoUsu { get; set; }

        public Guid? OrganizacionID { get; set; }

        public Guid? PersonaID { get; set; }

        public bool TieneTwitter { get; set; }

        [StringLength(15)]
        public string UsuarioTwitter { get; set; }

        [StringLength(1000)]
        public string TokenTwitter { get; set; }

        [StringLength(1000)]
        public string TokenSecretoTwitter { get; set; }

        public Guid? CurriculumPerfilID { get; set; }

        public int CaducidadResSusc { get; set; }

        public Guid? CurriculumID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Identidad> Identidad { get; set; }
        public virtual PerfilPersona PerfilPersona { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Profesor> Profesor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PerfilPersonaOrg> PerfilPersonaOrg { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraEcosistemaOpcionPerfil> DatoExtraEcosistemaOpcionPerfil { get; set; }

        public virtual Persona Persona { get; set; }
    }
}
