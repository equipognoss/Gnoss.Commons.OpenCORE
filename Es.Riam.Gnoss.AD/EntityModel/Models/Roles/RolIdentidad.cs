using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Roles
{
	[Serializable]
	[Table("RolIdentidad")]
	public partial class RolIdentidad
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public RolIdentidad()
		{
			
		}

		public Guid RolID { get; set; }
		
		public Guid IdentidadID { get; set; }

		public virtual Rol Rol { get; set; }

		public virtual IdentidadDS.Identidad Identidad { get; set; }
	}
}