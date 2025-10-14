using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.ViewModels
{
	public class PestanyaSubTiposPesoModel
	{
		public Guid PestanyaID { get; set; }
		public short TipoAutocompletar {  get; set; } 
		public List<SubtiposPeso> SubtiposPesos { get; set; }
	}

	public class SubtiposPeso
	{
		public string Ontologia { get; set; }
		public string Subtipo {  get; set; }
		public short Peso {  get; set; }
	}
}
