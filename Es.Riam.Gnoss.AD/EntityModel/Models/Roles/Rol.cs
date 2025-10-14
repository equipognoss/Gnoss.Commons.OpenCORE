using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Roles
{
	[Serializable]
	[Table("Rol")]
	public partial class Rol
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Rol() 
		{
			
		}

		public Guid ProyectoID { get; set; }

		public Guid OrganizacionID { get; set; }
		
		public Guid RolID { get; set; }

		public short Tipo { get; set; }

		public string Nombre { get; set; }

		public string Descripcion { get; set; }

		public ulong PermisosAdministracion { get; set; }

		public ulong PermisosContenidos { get; set; }

		public ulong PermisosRecursos { get; set; }

		public DateTime FechaModificacion { get; set; }

		public bool EsRolUsuario { get; set; }

		public virtual Proyecto Proyecto { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolIdentidad> RolIdentidad { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolGrupoIdentidades> RolGrupoIdentidades { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolOntologiaPermiso> RolOntologiaPermiso { get; set; }
	}
}