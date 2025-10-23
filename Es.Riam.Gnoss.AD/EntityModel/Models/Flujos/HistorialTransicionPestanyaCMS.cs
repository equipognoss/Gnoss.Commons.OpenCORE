using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("HistorialTransicionPestanyaCMS")]
	public partial class HistorialTransicionPestanyaCMS
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public HistorialTransicionPestanyaCMS() { }


		public Guid HistorialTransicionID { get; set; }

		public Guid PestanyaID { get; set; }

		public short Ubicacion { get; set; }

		public Guid TransicionID { get; set; }

		public Guid IdentidadID { get; set; }

		public DateTime Fecha { get; set; }

		public string Comentario { get; set; }


		public ProyectoPestanyaCMS ProyectoPestanyaCMS { get; set; }

		public virtual IdentidadDS.Identidad Identidad { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual Transicion Transicion { get; set; }
	}
}
