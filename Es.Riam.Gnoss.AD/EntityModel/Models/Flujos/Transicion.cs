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
	[Table("Transicion")]
	public partial class Transicion
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Transicion() { }

		public Guid TransicionID { get; set; }

		public string Nombre { get; set; }

		public Guid EstadoOrigenID { get; set; }
		
		public Guid EstadoDestinoID { get; set; }


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<TransicionIdentidad> TransicionIdentidad { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<TransicionGrupo> TransicionGrupo { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual Estado EstadoOrigen{ get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual Estado EstadoDestino { get; set; }

		public virtual ICollection<HistorialTransicionDocumento> HistorialTransicionDocumento { get; set; }

		public virtual ICollection<HistorialTransicionCMSComponente> HistorialTransicionCMSComponente { get; set; }

		public virtual ICollection<HistorialTransicionPestanyaCMS> HistorialTransicionPestanyaCMS { get; set; }
	}
}
