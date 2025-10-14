using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Roles
{
	[Serializable]
	[Table("RolEcosistema")]
	public partial class RolEcosistema
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public RolEcosistema()
		{

		}

		public Guid RolID { get; set; }

		public string Nombre { get; set; }

		public string Descripcion { get; set; }

		public ulong Permisos { get; set; }

		public DateTime FechaModificacion { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<RolEcosistemaUsuario> RolEcosistemaUsuario { get; set; }
	}
}