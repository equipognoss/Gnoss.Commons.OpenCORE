using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
	[Serializable]
	public class AdministrarFacetasViewModel
	{
		public Dictionary<string, string> ListaIdiomas { get; set; }

		public string IdiomaPorDefecto { get; set; }
		public List<FacetaModel> ListaFacetas { get; set; }
		public List<FacetaModel> ListaFacetasPropuestas { get; set; }
	}
}
