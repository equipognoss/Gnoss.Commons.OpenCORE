using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("HistorialTransicionDocumento")]
	public partial class HistorialTransicionDocumento
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public HistorialTransicionDocumento() { }


		public Guid HistorialTransicionID { get; set; }

		public Guid DocumentoID { get; set; }

		public Guid TransicionID { get; set; }

		public Guid IdentidadID { get; set; }

		public DateTime Fecha { get; set; }

		public string Comentario { get; set; }


		public virtual Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.Documento Documento { get; set; }

		public virtual IdentidadDS.Identidad Identidad { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual Transicion Transicion { get; set; }
	}
}
