using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
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
	[Table("Flujo")]
	public partial class Flujo
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Flujo() { }


		public Guid FlujoID { get; set; }

		public string Nombre { get; set; }

		public string Descripcion { get; set; }

		public Guid OrganizacionID { get; set; }

		public Guid ProyectoID { get; set; }

		public bool Nota { get; set; }

		public bool Adjunto { get; set; }

		public bool Video { get; set; }

		public bool Link { get; set; }

		public bool Encuesta { get; set; }

		public bool Debate { get; set; }

		public bool PaginaCMS { get; set; }

		public bool ComponenteCMS { get; set; }

		public bool RecursoSemantico { get; set; }

		public DateTime Fecha { get; set; }

		public virtual Proyecto Proyecto { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<FlujoObjetoConocimientoProyecto> FlujoObjetoConocimientoProyecto { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Estado> Estado { get; set; }
	}
}
