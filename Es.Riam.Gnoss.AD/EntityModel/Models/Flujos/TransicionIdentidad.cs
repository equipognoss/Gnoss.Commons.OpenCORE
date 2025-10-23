using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("TransicionIdentidad")]
	public partial class TransicionIdentidad
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public TransicionIdentidad() { }


		public Guid TransicionID { get; set; }

		public Guid IdentidadID { get; set; }


		public virtual Transicion Transicion { get; set; }

		public virtual IdentidadDS.Identidad Identidad { get; set; }
	}
}
