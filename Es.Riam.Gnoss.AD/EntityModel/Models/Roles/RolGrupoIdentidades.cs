using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Roles
{
	[Serializable]
	[Table("RolGrupoIdentidades")]
	public partial class RolGrupoIdentidades
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public RolGrupoIdentidades()
		{

		}

		public Guid RolID { get; set; }

		public Guid GrupoID { get; set; }

		public virtual Rol Rol { get; set; }

		public virtual GrupoIdentidades GrupoIdentidades { get; set; }
	}
}