using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
	public class TraducirRecursosRequest
	{
		public string IdiomaOrigen { get; set; }
		public TipoRecursoTraducir TipoRecurso { get; set; }
		public List<string> IdiomasTraducir { get; set; }
	}

	public class TipoRecursoTraducir
	{
		public int TipoRecurso { get; set; }
		public Guid OntologiaID { get; set; }
	}
}
