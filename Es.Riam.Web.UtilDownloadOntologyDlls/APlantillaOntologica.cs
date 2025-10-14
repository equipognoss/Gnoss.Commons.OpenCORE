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
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
namespace Es.Riam.Gnoss.Web.UtilDownloadOntologyDlls
{
	public abstract class APlantillaOntologica
	{
		private ConfigService mConfigService;
		private LoggingService mLoggingService;
		private Guid mOntologiaID;
		private Guid mProyectoID;
		private string mProyectoNombreCorto;
		public APlantillaOntologica(ConfigService configService, LoggingService loggingService, Guid proyectoID, Guid ontologiaID, string proyectoNombreCorto)
		{
			mConfigService = configService;
			mLoggingService = loggingService;
			mOntologiaID = ontologiaID;
			mProyectoID = proyectoID;
			mProyectoNombreCorto = proyectoNombreCorto;
		}
		public Guid OntologiaID { get { return mOntologiaID; } }
		public Guid ProyectoID { get { return mProyectoID; } }
		public string ProyectoNombreCorto { get { return mProyectoNombreCorto; } }
		/// <summary>
		/// 
		/// </summary>
		public abstract GestorDocumental GestorDocumental{ get; }
		public abstract AdministrarPlantillasOntologicasViewModel DescargaClasesModel { get; }
		public abstract PlantillaOntologicaViewModel ObtenerTemplate(Documento pDocumento);
		public abstract Dictionary<Guid, bool> ListaDocumentosConRecursos { get; }
		public abstract DataWrapperProyecto ProyectoConfigSemDataWrapperProyecto {  get; }
		public byte[] ObtenerOntologia(Guid pOntologiaID, string pVersion = null)
		{
			CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
			byte[] bytesArchivo = null;
			if (string.IsNullOrEmpty(pVersion))
			{
				bytesArchivo = fileService.ObtenerOntologiaBytes(pOntologiaID);
			}
			else
			{
				bytesArchivo = fileService.DescargarVersionBytes(pOntologiaID, pVersion);
			}
			return bytesArchivo;
		}

		public byte[] ObtenerXmlOntologia(Guid pOntologiaID)
		{
			CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
			byte[] bytesArchivo = fileService.ObtenerXmlOntologiaBytes(pOntologiaID);
			return bytesArchivo;
		}

	}
}
