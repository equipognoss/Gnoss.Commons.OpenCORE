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
	[Table("TransicionGrupo")]
	public partial class TransicionGrupo
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public TransicionGrupo() { }


		public Guid TransicionID { get; set; }

		public Guid GrupoID { get; set; }


		public virtual Transicion Transicion { get; set; }

		public virtual GrupoIdentidades GrupoIdentidades { get; set; }
	}
}
