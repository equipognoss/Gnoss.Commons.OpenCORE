using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
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
	[Table("Estado")]
	public partial class Estado
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
		public Estado() { }


		public Guid EstadoID { get; set; }

		public Guid FlujoID { get; set; }

		public string Nombre { get; set; }

		public bool Publico { get; set; }

		public short Tipo { get; set; }

		public string Color {  get; set; }

		public bool PermiteMejora {  get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<CMSComponente> CMSComponente { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<ProyectoPestanyaCMS> ProyectoPestanyaCMS { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.Documento> Documento { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<EstadoIdentidad> EstadoIdentidad { get; set; }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public virtual ICollection<EstadoGrupo> EstadoGrupo { get; set; }

		public virtual Flujo Flujo { get; set; }

		public virtual ICollection<Transicion> TransicionesOrigen { get; set; }
		public virtual ICollection<Transicion> TransicionesDestino {  get; set; }
		public virtual ICollection<VersionDocumento> VersionesDocumentos { get; set; }
	}
}
