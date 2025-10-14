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
	[Table("RolEcosistemaUsuario")]
	public partial class RolEcosistemaUsuario
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public RolEcosistemaUsuario()
		{

		}

		public Guid RolID { get; set; }

		public Guid UsuarioID { get; set; }

		public virtual RolEcosistema RolEcosistema { get; set; }

		public virtual Usuario Usuario { get; set; }
	}
}