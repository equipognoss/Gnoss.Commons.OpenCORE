using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("EstadoIdentidad")]
	public partial class EstadoIdentidad
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public EstadoIdentidad() { }


		public Guid EstadoID { get; set; }

		public Guid IdentidadID { get; set; }

		public bool Editor { get; set; }


		public virtual Estado Estado { get; set; }

		public virtual IdentidadDS.Identidad Identidad { get; set; }
	}
}
