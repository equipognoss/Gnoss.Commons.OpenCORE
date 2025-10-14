using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("Usuario")]
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            AdministradorOrganizacion = new HashSet<AdministradorOrganizacion>();
            ProyectoRolUsuario = new HashSet<ProyectoRolUsuario>();
            ProyectoUsuarioIdentidad = new HashSet<ProyectoUsuarioIdentidad>();
            TesauroUsuario = new HashSet<TesauroUsuario>();
            GrupoUsuarioUsuario = new HashSet<GrupoUsuarioUsuario>();
            OrganizacionRolUsuario = new  HashSet<OrganizacionRolUsuario>();
            UsuarioVinculadoLoginRedesSociales = new HashSet<UsuarioVinculadoLoginRedesSociales>();
            Persona = new HashSet<Persona>();
            BaseRecursosUsuario = new HashSet<BaseRecursosUsuario>();
        }

        
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(12)]
        public string Login { get; set; }

        [StringLength(50)]
        public string Password { get; set; }

        public bool? EstaBloqueado { get; set; }

        public bool TwoFactorAuthentication { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCorto { get; set; }

        public short? Version { get; set; }

        public DateTime? FechaCambioPassword { get; set; }

        public short Validado { get; set; }

        public virtual AdministradorGeneral AdministradorGeneral { get; set; }

        public virtual ICollection<AdministradorOrganizacion> AdministradorOrganizacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrupoUsuarioUsuario> GrupoUsuarioUsuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoUsuarioIdentidad> ProyectoUsuarioIdentidad { get; set; }

        public virtual InicioSesion InicioSesion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProyectoRolUsuario> ProyectoRolUsuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizacionRolUsuario> OrganizacionRolUsuario { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsuarioVinculadoLoginRedesSociales> UsuarioVinculadoLoginRedesSociales { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Persona> Persona { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TesauroUsuario> TesauroUsuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaseRecursosUsuario> BaseRecursosUsuario { get; set; }

        public virtual GeneralRolUsuario GeneralRolUsuario { get; set; }
        public virtual UsuarioContadores UsuarioContadores { get; set; }
        public virtual UsuarioRedirect UsuarioRedirect { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolEcosistemaUsuario> RolEcosistemaUsuario { get; set; }
	}
}
