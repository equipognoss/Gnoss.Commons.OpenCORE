using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Flujos
{
	public class EstadoModel
	{
		public EstadoModel() { }

		public Guid EstadoID { get; set; }
		public Guid FlujoID { get; set; }
		public string Nombre { get; set; }
		public bool Publico { get; set; }
		public bool IdentidadActualEditora { get; set; }
		public List<Guid> IdentidadesEditoras { get; set; }
		public List<Guid> GruposEditores { get; set; }
		public List<Guid> IdentidadesLectoras { get; set; }
		public List<Guid> GruposLectores { get; set; }		
		public List<TransicionModel> Transiciones { get; set; }
		public string Color { get; set; }
		public bool EsFinal { get; set; }
		public bool PermiteMejora { get; set; }		
	}
}
