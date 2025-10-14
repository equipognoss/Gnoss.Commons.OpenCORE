using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
	public class ProyectoPestanyasBusquedaOCsViewModel
	{
		public List<ProyectoPestanyaBusquedaOCModel> PesntanyasBusquedaOCsProyecto {  get; set; }
		public Dictionary<string, string> ListaIdiomas { get; set; }
		public string IdiomaPorDefecto { get; set; }
	}
}
