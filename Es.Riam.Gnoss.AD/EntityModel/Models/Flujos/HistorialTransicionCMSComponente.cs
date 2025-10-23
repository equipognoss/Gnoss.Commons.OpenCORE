using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("HistorialTransicionCMSComponente")]
	public partial class HistorialTransicionCMSComponente
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public HistorialTransicionCMSComponente() { }


		public Guid HistorialTransicionID { get; set; }

		public Guid ComponenteID { get; set; }

		public Guid TransicionID { get; set; }

		public Guid IdentidadID { get; set; }

		public DateTime Fecha { get; set; }

		public string Comentario { get; set; }


		public virtual CMSComponente CMSComponente { get; set; }

		public virtual IdentidadDS.Identidad Identidad { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual Transicion Transicion { get; set; }
	}
}
