using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Es.Riam.Semantica.OWL;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
namespace Es.Riam.Gnoss.Web.UtilDownloadOntologyDlls
{
	public interface IPlantillaOntologica
	{
		/// <summary>
		/// 
		/// </summary>
		GestorDocumental GestorDocumental{ get; }
		AdministrarPlantillasOntologicasViewModel DescargaClasesModel { get; }
		public Dictionary<string, string> DicPref();
		public Dictionary<string, KeyValuePair<Ontologia, byte[]>> DiccionarioOntologias();
		byte[] ObtenerXmlOntologia(Guid pOntologiaID);
		byte[] ObtenerOntologia(Guid pOntologiaID, string pVersion = null);
		PlantillaOntologicaViewModel ObtenerTemplate(Documento pDocumento);
	}
}
