using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
	public class TareasSegundoPlanoViewModel
	{
		public Guid TareaID { get; set; }
		public Guid ProyectoID { get; set; }
		public Guid OrganizacionID { get; set; }
		public string Tipo { get; set; }
		public string Nombre { get; set; }
		public string Estado { get; set; }
		public DateTime FechaInicio { get; set; }
		public int EventosTotales { get; set; }
		public int Contador { get; set; }
	}
}
