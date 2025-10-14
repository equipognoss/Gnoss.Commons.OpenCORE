using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Azure;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Es.Riam.Semantica.OWL;
using Microsoft.AspNetCore.Mvc;
using OntologiaAClase;
using Es.Riam.Gnoss.UtilServiciosWeb;
using System.IO.Compression;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.InterfacesOpen;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Es.Riam.Gnoss.Elementos.Amigos;

namespace Es.Riam.Gnoss.Web.UtilDownloadOntologyDlls
{
	public class PlantillaOntologicaUtil
	{
		EntityContext mEntityContext;
		LoggingService mLoggingService;
		RedisCacheWrapper mRedisCacheWrapper;
		ConfigService mConfigService;
		IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
		private APlantillaOntologica mPlantillaOntologica;
		private Dictionary<Guid, bool> mListaDocumentosConRecursos;
		private IMassiveOntologyToClass mMassiveOntologyToClass;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public PlantillaOntologicaUtil(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IMassiveOntologyToClass massiveOntologyToClass, APlantillaOntologica plantillaOntologica, ILogger<PlantillaOntologicaUtil> logger, ILoggerFactory loggerFactory)
		{
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mRedisCacheWrapper = redisCacheWrapper;
			mConfigService = configService;
			mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
			mPlantillaOntologica = plantillaOntologica;
			mMassiveOntologyToClass = massiveOntologyToClass;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

		public AdministrarPlantillasOntologicasViewModel DescargaClasesModel
		{
			get { return mPlantillaOntologica.DescargaClasesModel; }
		}


		//Descargar todas las clases del ecosistema
		public byte[] DownloadClasses(string pVersionWeb ="", string pNombreRepositorio = "", string pTokenRepositorio = "")
		{
			try
			{
				Dictionary<string, string> dicPref = DicPref();
				Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = DiccionarioOntologias();
				ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
				List<string> listaIdiomas = paramCL.ObtenerListaIdiomas();

				string directorio = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
				Directory.CreateDirectory(directorio);
				List<string> nombresOntologias = mPlantillaOntologica.DescargaClasesModel.Templates.Select(p => p.OntologyName).Union(mPlantillaOntologica.DescargaClasesModel.SecondaryTemplates.Select(p => p.OntologyName)).ToList();
				GenerarClaseYVista claseYvista = new GenerarClaseYVista(directorio, mPlantillaOntologica.ProyectoNombreCorto, mPlantillaOntologica.ProyectoID, nombresOntologias, dicPref, diccionarioOntologias, false, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass, mLoggerFactory.CreateLogger<GenerarClaseYVista>(), mLoggerFactory);
				claseYvista.CrearObjetos(diccionarioOntologias);
				claseYvista.CrearRelaciones();
				foreach (var ontologiaPrincipal in mPlantillaOntologica.DescargaClasesModel.Templates)
				{
					try
					{
						byte[] bytesOntologia = mPlantillaOntologica.ObtenerOntologia(ontologiaPrincipal.OntologyID);
						byte[] bytesXmlOntologia = mPlantillaOntologica.ObtenerXmlOntologia(ontologiaPrincipal.OntologyID);

						Ontologia ontologia = new Ontologia(bytesOntologia);
						OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaPrincipal.OntologyName, ontologia, bytesXmlOntologia, true, listaIdiomas, directorio);
						claseYvista.CrearClases(contenedorOntologia);
						claseYvista.CrearVistas(contenedorOntologia, mPlantillaOntologica.ProyectoNombreCorto);
					}
					catch (Exception ex)
					{
						//throw new Exception($"En la ontologia {UtilCadenas.ObtenerTextoDeIdioma(ontologiaPrincipal.Name, IdiomaUsuario, IdiomaPorDefecto)}, {ex.Message}", ex);
					}
				}
				foreach (var ontologiaSecundaria in mPlantillaOntologica.DescargaClasesModel.SecondaryTemplates)
				{
					byte[] bytesOntologia = mPlantillaOntologica.ObtenerOntologia(ontologiaSecundaria.OntologyID);
					byte[] bytesXmlOntologia = null;
					if (ontologiaSecundaria.HasXmlFile)
					{
						bytesXmlOntologia = mPlantillaOntologica.ObtenerXmlOntologia(ontologiaSecundaria.OntologyID);
					}

					Ontologia ontologia = new Ontologia(bytesOntologia);
					OntologiaGenerar contenedorOntologia = new OntologiaGenerar(ontologiaSecundaria.OntologyName, ontologia, bytesXmlOntologia, false, listaIdiomas, directorio);

					claseYvista.CrearClases(contenedorOntologia);
				}
				claseYvista.GenerarPaqueteCSPROJ(mPlantillaOntologica.ProyectoNombreCorto);
				if (!string.IsNullOrEmpty(pVersionWeb))
				{
					//Crear Dockerfile y archivo json 
					claseYvista.CrearDockerFile(pVersionWeb, mPlantillaOntologica.ProyectoNombreCorto);
					claseYvista.CrearArchivoJson(mPlantillaOntologica.ProyectoNombreCorto);
				}
				//Generar clases
				//Comprimirlo
				DirectoryInfo directoryPrincipal = new DirectoryInfo(directorio);
				DirectoryInfo[] directories = directoryPrincipal.GetDirectories();
				string nombrefichero = string.Empty;
				foreach (DirectoryInfo dir in directories)
				{
					if (dir.Name.Contains("ClasesYVistas_"))
					{
						nombrefichero = dir.Name;
					}
				}

				string pZipPath = Path.Combine(directorio, "comprimido.zip");
				string folderPath = Path.Combine(directorio, nombrefichero);

				ZipFile.CreateFromDirectory(folderPath, pZipPath);
				byte[] bytes = System.IO.File.ReadAllBytes(pZipPath);
				Thread.Sleep(1000);

				try
				{
					Directory.Delete(directorio, true);
				}
				catch (Exception)
				{
					Thread.Sleep(1000);
					try
					{
						Directory.Delete(directorio, true);
					}
					catch
					{
						mLoggingService.GuardarLogError("Fallo al intentar borrra el fichero temporal de la carpeta: " + directorio, mlogger);
					}
				}
				//Enviar clases


				return bytes;


			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex.Message, mlogger);
				throw;
			}
		}
		public Dictionary<string, KeyValuePair<Ontologia, byte[]>> DiccionarioOntologias()
		{
			Dictionary<string, string> dicPref = new Dictionary<string, string>();
			Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();

			foreach (var ontologiaPrimaria in mPlantillaOntologica.DescargaClasesModel.Templates)
			{
				if (!ontologiaPrimaria.OntologyName.Equals("dbpedia"))
				{
					byte[] bytesOntologias = mPlantillaOntologica.ObtenerOntologia(ontologiaPrimaria.OntologyID);
					byte[] bytesXmlOntologia = mPlantillaOntologica.ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);
					Ontologia ontologia = new Ontologia(bytesOntologias);
					ontologia.LeerOntologia();

					foreach (string key in ontologia.NamespacesDefinidos.Keys)
					{
						if (!dicPref.ContainsKey(key))
						{
							dicPref.Add(key, ontologia.NamespacesDefinidos[key]);

						}
					}
					diccionarioOntologias.Add(ontologiaPrimaria.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
				}
			}
			foreach (var secondaryOntology in mPlantillaOntologica.DescargaClasesModel.SecondaryTemplates)
			{
				byte[] bytesOntologias = mPlantillaOntologica.ObtenerOntologia(secondaryOntology.OntologyID);
				byte[] bytesXmlOntologia = mPlantillaOntologica.ObtenerXmlOntologia(secondaryOntology.OntologyID);
				Ontologia ontologia = new Ontologia(bytesOntologias);
				ontologia.LeerOntologia();
				foreach (string key in ontologia.NamespacesDefinidos.Keys)
				{
					if (!dicPref.ContainsKey(key))
					{
						dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
					}
				}
				diccionarioOntologias.Add(secondaryOntology.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
			}
			return diccionarioOntologias;
		}
		public Dictionary<string, string> DicPref()
		{
			Dictionary<string, string> dicPref = new Dictionary<string, string>();
			Dictionary<string, KeyValuePair<Ontologia, byte[]>> diccionarioOntologias = new Dictionary<string, KeyValuePair<Ontologia, byte[]>>();

			foreach (var ontologiaPrimaria in mPlantillaOntologica.DescargaClasesModel.Templates)
			{
				if (!ontologiaPrimaria.OntologyName.Equals("dbpedia"))
				{
					byte[] bytesOntologias = mPlantillaOntologica.ObtenerOntologia(ontologiaPrimaria.OntologyID);
					byte[] bytesXmlOntologia = mPlantillaOntologica.ObtenerXmlOntologia(ontologiaPrimaria.OntologyID);
					Ontologia ontologia = new Ontologia(bytesOntologias);
					ontologia.LeerOntologia();

					foreach (string key in ontologia.NamespacesDefinidos.Keys)
					{
						if (!dicPref.ContainsKey(key))
						{
							dicPref.Add(key, ontologia.NamespacesDefinidos[key]);

						}
					}
					diccionarioOntologias.Add(ontologiaPrimaria.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
				}
			}
			foreach (var secondaryOntology in mPlantillaOntologica.DescargaClasesModel.SecondaryTemplates)
			{
				byte[] bytesOntologias = mPlantillaOntologica.ObtenerOntologia(secondaryOntology.OntologyID);
				byte[] bytesXmlOntologia = mPlantillaOntologica.ObtenerXmlOntologia(secondaryOntology.OntologyID);
				Ontologia ontologia = new Ontologia(bytesOntologias);
				ontologia.LeerOntologia();
				foreach (string key in ontologia.NamespacesDefinidos.Keys)
				{
					if (!dicPref.ContainsKey(key))
					{
						dicPref.Add(key, ontologia.NamespacesDefinidos[key]);
					}
				}
				diccionarioOntologias.Add(secondaryOntology.OntologyName, new KeyValuePair<Ontologia, byte[]>(ontologia, bytesXmlOntologia));
			}
			return dicPref;
		}
	}
}
