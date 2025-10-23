using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Flujos
{
	public class TransicionModel
	{
		public TransicionModel() { }

		public Guid TransicionID { get; set; }
		public string Nombre { get; set; }
	}
}
