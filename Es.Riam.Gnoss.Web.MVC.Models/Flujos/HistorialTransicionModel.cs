using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Flujos
{
	public class HistorialTransicionModel
	{
		public HistorialTransicionModel() { }

		public Guid HistorialID { get; set; }
		public Guid DocumentoID { get; set; }
		public string NombreTransicion { get; set; }
		public string Revisor { get; set; }
		public DateTime Fecha { get; set; }
		public string Comentario { get; set; }
	}
}
