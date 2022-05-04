using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("ProyectoRolUsuario")]
    public partial class ProyectoRolUsuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProyectoRolUsuario()
        {
            ProyRolUsuClausulaReg = new HashSet<ProyRolUsuClausulaReg>();
        }

        [Column(Order = 0)]
        public Guid OrganizacionGnossID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(16)]
        public string RolPermitido { get; set; }

        [StringLength(16)]
        public string RolDenegado { get; set; }

        public bool EstaBloqueado { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyRolUsuClausulaReg> ProyRolUsuClausulaReg { get; set; }
    }
}
