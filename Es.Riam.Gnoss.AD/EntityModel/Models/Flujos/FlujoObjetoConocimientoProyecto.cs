using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Flujos
{
	[Serializable]
	[Table("FlujoObjetoConocimientoProyecto")]
	public partial class FlujoObjetoConocimientoProyecto
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public FlujoObjetoConocimientoProyecto() { }


		public Guid FlujoID { get; set; }

		[StringLength(100)]
		public string Ontologia { get; set; }

		public Guid OrganizacionID { get; set; }

		public Guid ProyectoID { get; set; }


		public virtual Flujo Flujo { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual OntologiaProyecto OntologiaProyecto { get; set; }
	}
}
