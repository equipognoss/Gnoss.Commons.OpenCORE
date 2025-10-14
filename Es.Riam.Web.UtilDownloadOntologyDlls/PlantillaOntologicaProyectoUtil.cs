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
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Util;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Es.Riam.Gnoss.Elementos.Amigos;

namespace Es.Riam.Gnoss.Web.UtilDownloadOntologyDlls
{
	public class PlantillaOntologicaProyectoUtil : APlantillaOntologica
	{
		private AdministrarPlantillasOntologicasViewModel mDescargaClasesModel;
		private GestorDocumental mGestorDocumental;
		private EntityContext mEntityContext;
		private LoggingService mLoggingService;
		private ConfigService mConfigService;
		private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
		private DataWrapperProyecto mProyectoConfigSemDataWrapperProyecto;
		/// <summary>
		/// Lista de documentos con imagenes
		/// </summary>
		private Dictionary<Guid, bool> mListaDocumentosConRecursos;
		private Guid mProyectoID;
		private Guid mUsuarioID;
		private Guid mOrganizacionID;
		private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public PlantillaOntologicaProyectoUtil(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication,RedisCacheWrapper redisCacheWrapper, Guid proyectoID, Guid organizacionID, Guid usuarioID, Guid ontologiaID, string nombreCortoProyecto, ILogger<PlantillaOntologicaProyectoUtil> logger, ILoggerFactory loggerFactory) : base(configService, loggingService, proyectoID, ontologiaID, nombreCortoProyecto)
		{
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mConfigService = configService;
			mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
			mProyectoID = proyectoID;
			mRedisCacheWrapper = redisCacheWrapper;
			mUsuarioID = usuarioID;
			mOrganizacionID = organizacionID;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }
		public override GestorDocumental GestorDocumental
		{
			get
			{
				if (mGestorDocumental == null)
				{
					mGestorDocumental = new GestorDocumental(new DataWrapperDocumentacion(), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestorDocumental>(), mLoggerFactory);
					DocumentacionCL docCL = new DocumentacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
					docCL.ObtenerBaseRecursosProyecto(mGestorDocumental.DataWrapperDocumentacion, mProyectoID, mOrganizacionID, mUsuarioID);

					DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
					docCN.ObtenerOntologiasProyecto(mProyectoID, mGestorDocumental.DataWrapperDocumentacion, true, true, false, true);
					docCN.Dispose();

					mGestorDocumental.CargarDocumentos();
				}
				return mGestorDocumental;
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

					Guid ontologiaSeleccionada = OntologiaID;

					if (!ontologiaSeleccionada.Equals(Guid.Empty) && GestorDocumental.ListaDocumentos.ContainsKey(ontologiaSeleccionada))
					{
						mDescargaClasesModel.SelectedOntology = ObtenerTemplate(GestorDocumental.ListaDocumentos[ontologiaSeleccionada]);
					}

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

		public override DataWrapperProyecto ProyectoConfigSemDataWrapperProyecto
		{
			get
			{
				if (mProyectoConfigSemDataWrapperProyecto == null)
				{
					ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
					mProyectoConfigSemDataWrapperProyecto = proyCN.ObtenerConfiguracionSemanticaExtraDeProyecto(mProyectoID);
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
