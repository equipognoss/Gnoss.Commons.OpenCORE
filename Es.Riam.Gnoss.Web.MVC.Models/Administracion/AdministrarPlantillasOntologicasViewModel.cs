using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
	/// <summary>
	/// 
	/// </summary>
	public class AdministrarPlantillasOntologicasViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		public List<PlantillaOntologicaViewModel> Templates { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public List<PlantillaOntologicaViewModel> SecondaryTemplates { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public PlantillaOntologicaViewModel SelectedOntology { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class PlantillaOntologicaViewModel
	{
		/// <summary>
		/// 
		/// </summary>
		public Guid OntologyID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsSecondaryOntology { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string OntologyName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Image { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool Protected { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool HasResources { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool AllowMasiveUpload { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool HasXmlFile { get; set; }

	}
}
