using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("EstadoGrupo")]
	public partial class EstadoGrupo
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public EstadoGrupo() { }


		public Guid EstadoID { get; set; }

		public Guid GrupoID { get; set; }

		public bool Editor { get; set; }


		public virtual Estado Estado { get; set; }

		public virtual GrupoIdentidades GrupoIdentidades { get; set; }
	}
}
