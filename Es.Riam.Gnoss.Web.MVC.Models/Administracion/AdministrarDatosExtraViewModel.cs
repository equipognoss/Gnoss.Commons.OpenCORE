using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
	[Serializable]
	public class AdministrarDatosExtraViewModel
	{
		public AdministrarDatosExtraViewModel() 
		{ 
		}

		public List<DatoExtraModel>	ListaDatosExtraProyecto { get; set; }

		public List<DatoExtraVirtuosoModel> ListaDatosExtraVirtuoso { get; set; }
	}

	[Serializable]
	public class DatoExtraModel
	{
		public Guid DatoExtraID { get; set; }

		public string Nombre { get; set; }

		public TipoDatoExtra Tipo { get; set; }

		public List<DatoExtraOpcionModel> Opciones { get; set; }

		public string PredicadoRDF { get; set; }

		public bool Obligatorio { get; set; }

		public int Orden { get; set; }

		public Guid ProyectoID { get; set; }

		public Guid OrganizacionID { get; set; }

		public bool Deleted { get; set; }
	}

	[Serializable]
	public class DatoExtraOpcionModel
	{
		public Guid OrganizacionID { get; set; }

		public Guid ProyectoID { get; set; }

		public Guid DatoExtraID { get; set; }

		public Guid OpcionID { get; set; }

		public int Orden { get; set; }

		public string Nombre { get; set; }
	}

	[Serializable]
	public class DatoExtraVirtuosoModel
	{
		public Guid DatoExtraID { get; set; }

		public string Nombre { get; set; }

		public string NombreInput { get; set; }

		public bool Obligatorio { get; set; }

		public TipoDatoExtra Tipo { get; set; }

		public int Orden { get; set; }

		public string QueryVirtuoso { get; set; }

		public string PredicadoRDF { get; set; }
	}

	[Serializable]
	public class DatoExtraEditModel
	{
		public Guid DatoExtraID { get; set; }

		public string Nombre { get; set; }

		public bool Obligatorio { get; set; }

		public TipoDatoExtra Tipo { get; set; }

		public string PredicadoRDF { get; set; }

		public List<DatoExtraOpcionModel> Opciones { get; set; }

		public int Orden { get; set; }
	}

	public enum TipoDatoExtra
	{
		Opcion = 0,
		TextoLibre = 1
	}
}
