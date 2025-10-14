using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Roles
{
	[Serializable]
	[Table("RolOntologiaPermiso")]
	public class RolOntologiaPermiso
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public RolOntologiaPermiso()
		{
		}

		public Guid DocumentoID { get; set; }
		public Guid RolID { get; set; }
		public ulong Permisos { get; set; }

		public virtual Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.Documento Documento { get; set; }
		public virtual Rol Rol { get; set; }
	}
}
