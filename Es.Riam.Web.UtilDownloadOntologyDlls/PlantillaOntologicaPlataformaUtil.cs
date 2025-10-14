using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.UtilDownloadOntologyDlls
{
	public class PlantillaOntologicaPlataformaUtil : APlantillaOntologica
	{
		private AdministrarPlantillasOntologicasViewModel mDescargaClasesModel;
		private GestorDocumental mGestorDocumentalOntologiasPlataforma;
		private EntityContext mEntityContext;
		private LoggingService mLoggingService;
		private ConfigService mConfigService;
		private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
		private DataWrapperProyecto mProyectoConfigSemDataWrapperProyecto;
		/// <summary>
		/// Lista de documentos con imagenes
		/// </summary>
		private Dictionary<Guid, bool> mListaDocumentosConRecursos;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public PlantillaOntologicaPlataformaUtil(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, Guid proyectoID, string proyectoNombreCorto, ILogger<PlantillaOntologicaPlataformaUtil> logger, ILoggerFactory loggerFactory) :base(configService, loggingService, proyectoID, Guid.Empty, proyectoNombreCorto)
		{
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mConfigService = configService;
			mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;

        }
		public override GestorDocumental GestorDocumental
		{
			get
			{
				if (mGestorDocumentalOntologiasPlataforma == null)
				{
					mGestorDocumentalOntologiasPlataforma = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);


					DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
					docCN.ObtenerOntologiasPlataforma(mGestorDocumentalOntologiasPlataforma.DataWrapperDocumentacion);
					docCN.Dispose();

					mGestorDocumentalOntologiasPlataforma.CargarDocumentos();
				}
				return mGestorDocumentalOntologiasPlataforma;
			}
		}

		public override AdministrarPlantillasOntologicasViewModel DescargaClasesModel
		{
			get
			{
				if (mDescargaClasesModel == null)
				{
					mDescargaClasesModel = new AdministrarPlantillasOntologicasViewModel();
					mDescargaClasesModel.Templates = new List<PlantillaOntologicaViewModel>();
					mDescargaClasesModel.SecondaryTemplates = new List<PlantillaOntologicaViewModel>();

					foreach (Documento doc in GestorDocumental.ListaDocumentos.Values)
					{
						if (doc.TipoDocumentacion == TiposDocumentacion.Ontologia)
						{
							mDescargaClasesModel.Templates.Add(ObtenerTemplate(doc));
						}
						else if (doc.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria)
						{
							mDescargaClasesModel.SecondaryTemplates.Add(ObtenerTemplate(doc));
						}
					}

					mDescargaClasesModel.SelectedOntology = new PlantillaOntologicaViewModel();
				}
				return mDescargaClasesModel;
			}
		}

		public override Dictionary<Guid, bool> ListaDocumentosConRecursos
		{
			get
			{
				if (mListaDocumentosConRecursos == null)
				{
					List<Guid> listaDocs = new List<Guid>(GestorDocumental.ListaDocumentos.Keys);

					DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
					mListaDocumentosConRecursos = docCN.ComprobarSiOntologiaTieneRecursos(listaDocs);
					docCN.Dispose();
				}
				return mListaDocumentosConRecursos;
			}
		}

		/// <summary>
		/// DataWrapper del proyecto actual
		/// </summary>
		public override DataWrapperProyecto ProyectoConfigSemDataWrapperProyecto
		{
			get
			{
				if (mProyectoConfigSemDataWrapperProyecto == null)
				{
					ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
					mProyectoConfigSemDataWrapperProyecto = proyCN.ObtenerConfiguracionSemanticaExtraDeProyectos();
					proyCN.Dispose();
				}
				return mProyectoConfigSemDataWrapperProyecto;
			}

		}

		public override PlantillaOntologicaViewModel ObtenerTemplate(Documento pDocumento)
		{
			PlantillaOntologicaViewModel template = new PlantillaOntologicaViewModel();
			template.OntologyID = pDocumento.Clave;
			template.IsSecondaryOntology = pDocumento.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria;

			template.Name = pDocumento.Titulo;
			template.OntologyName = pDocumento.Enlace.Replace(".owl", "");
			template.Description = pDocumento.Descripcion;
			if (!string.IsNullOrEmpty(pDocumento.FilaDocumento.NombreCategoriaDoc) && pDocumento.FilaDocumento.NombreCategoriaDoc.Contains(".jpg"))
			{
				template.Image = pDocumento.FilaDocumento.NombreCategoriaDoc.Split(',')[1].Replace(".jpg", $"_{pDocumento.FilaDocumento.NombreCategoriaDoc.Split(',')[0]}.jpg");
			}
			template.Protected = pDocumento.FilaDocumento.Protegido;
			if (ListaDocumentosConRecursos.ContainsKey(pDocumento.Clave))
			{
				template.HasResources = true;
			}


			bool cargasMultiplesDisponibles = false;
			Dictionary<string, string> propiedades = UtilCadenas.ObtenerPropiedadesDeTexto(pDocumento.NombreEntidadVinculada);
			if (propiedades.ContainsKey(PropiedadesOntologia.cargasmultiples.ToString()) && propiedades[PropiedadesOntologia.cargasmultiples.ToString()] == "true")
			{
				cargasMultiplesDisponibles = true;
			}
			template.AllowMasiveUpload = cargasMultiplesDisponibles;

			template.HasXmlFile = true;

			if (pDocumento.TipoDocumentacion == TiposDocumentacion.OntologiaSecundaria)
			{
				List<ProyectoConfigExtraSem> filas = ProyectoConfigSemDataWrapperProyecto.ListaProyectoConfigExtraSem.Where(proy => proy.UrlOntologia.Equals(pDocumento.Enlace) && proy.Tipo.Equals((short)TipoConfigExtraSemantica.EntidadSecundaria)).ToList();
				if (filas == null || filas.Count == 0)
				{
					template.HasXmlFile = false;
				}
			}

			return template;
		}
	}
}
