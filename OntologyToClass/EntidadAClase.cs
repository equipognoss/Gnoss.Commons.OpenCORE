using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.InterfacesOpen;
using Es.Riam.Gnoss.Recursos;

namespace OntologiaAClase
{
	public class EntidadAClase
	{
		public enum PropiedadTipo
		{
			StringSimple,
			StringMultiple,
			Bool,
			DateSimple,
			DateMultiple,
			DateNull,
			IntSimple,
			IntMultiple,
			Image
		}

		private const string PERSONA_GNOSS = "PersonaGnoss";

		private Constantes c = new Constantes();
		public StringBuilder Clase { get; }
		private XmlDocument doc;
		private List<string> listaObjetosExternos;
		private string nombreOnto;
		private Ontologia ontologia;
		private string nombrePropTitulo;
		private byte[] contentXML;

		private string nombrePropTituloEntero;
		private string nombrePropDescripcionEntera;
		private string nombrePropDescripcion;
		private bool esPrincipal;
		private bool? mEsMultiIdiomaConfig;
		private int mNumItem;
		Dictionary<string, string> dicPref;
		public Dictionary<string, PropiedadTipo> propListiedadesTipo = new Dictionary<string, PropiedadTipo>();
		public Dictionary<string, bool> propListiedadesMultidioma = new Dictionary<string, bool>();
		public List<Propiedad> listentidades = new List<Propiedad>();
		public List<Propiedad> listentidadesAux = new List<Propiedad>();
		public List<string> entListPadres = new List<string>();
		public Dictionary<string, List<Propiedad>> propListPadre = new Dictionary<string, List<Propiedad>>();
		public List<string> listaIdiomas;
		public List<string> nombresOntologia;
		public List<ObjetoPropiedad> listaObjetosPropiedad = new List<ObjetoPropiedad>();
		public Dictionary<Propiedad, bool> listaPropMultiidiomaFalse = new Dictionary<Propiedad, bool>();
		public readonly Guid proyID;
		public readonly string nombreCortoProy;
		private EntityContext mEntityContext;
		private LoggingService mLoggingService;
		private ConfigService mConfigService;
		private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
		private IMassiveOntologyToClass mMassiveOntologyClass;
		private string mRdfType;

		/// <summary>
		/// Constructor de la clase , que la inicializa con un stringBuilder
		/// </summary>
		public EntidadAClase(EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, IServicesUtilVirtuosoAndReplication pServicesUtilVirtuosoAndReplication, IMassiveOntologyToClass pMassiveOntologyToClass)
		{
			Clase = new StringBuilder();
			mEntityContext = pEntityContext;
			mLoggingService = pLoggingService;
			mConfigService = pConfigService;
			mServicesUtilVirtuosoAndReplication = pServicesUtilVirtuosoAndReplication;
			mMassiveOntologyClass = pMassiveOntologyToClass;
		}

		/// <summary>
		/// Constructor de la clase que necesita uan ontologia, su nombre, y el xml , además del string builder
		/// </summary>
		/// <param name="pOntologia"></param>
		/// <param name="pNombreOnto"></param>
		/// <param name="pContentXML"></param>
		public EntidadAClase(Ontologia pOntologia, string pNombreOnto, byte[] pContentXML, bool pEsPrincipal, List<string> pListaIdiomas, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, List<ObjetoPropiedad> pListaObjetosPropiedad, EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, IServicesUtilVirtuosoAndReplication pServicesUtilVirtuosoAndReplication, IMassiveOntologyToClass pMassiveOntologyToClass, string pRdfType)
		{
			nombreCortoProy = pNombreCortoProy;
			proyID = pProyID;
			Clase = new StringBuilder();
			ontologia = pOntologia;
			nombreOnto = pNombreOnto;
			contentXML = pContentXML;
			listaObjetosPropiedad = pListaObjetosPropiedad;
			esPrincipal = pEsPrincipal;
			dicPref = pOntologia.NamespacesDefinidos;
			listaIdiomas = pListaIdiomas;
			nombresOntologia = pNombresOntologia;
			mEntityContext = pEntityContext;
			mLoggingService = pLoggingService;
			mConfigService = pConfigService;
			doc = new XmlDocument();
			if (pContentXML != null)
			{
				MemoryStream ms = new MemoryStream(pContentXML);
				doc.Load(ms);
			}
			mNumItem = 0;
			listaObjetosExternos = new List<string>();
			mServicesUtilVirtuosoAndReplication = pServicesUtilVirtuosoAndReplication;
			mMassiveOntologyClass = pMassiveOntologyToClass;
			mRdfType = pRdfType;
		}

		/// <summary>
		/// Creamos la clase a partir de la entidad 
		/// </summary>
		/// <param name="pEntidad"></param>
		/// <returns></returns>
		public string GenerarClase(ElementoOntologia pEntidad, List<string> pListaPropiedesSearch, List<string> pListaPadrePropiedadesAnidadas)
		{
			ObtenerUsings(pEntidad);
			Clase.AppendLine($"{c.namespac} {nombreOnto}");
			Clase.AppendLine("{");
			EscribirHerencias(pEntidad);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{{");
			ConstructorSencillo(pEntidad);
			Clase.AppendLine();
			ConstructorComplejo(pEntidad);
			Clase.AppendLine();
			CreacionDePropiedades(pEntidad);
			Clase.AppendLine();
			ObtenerPropiedades(pEntidad);
			ObtenerEntidades(pEntidad);
			if (esPrincipal)
			{
				ObtenerTituloDescripcion();
			}
			CrearToRecurso(esPrincipal, pEntidad);
			Clase.AppendLine();
			//---------------
			mMassiveOntologyClass.RdfType = mRdfType;
			mMassiveOntologyClass.CrearToOntologyGraphTriples(esPrincipal, pEntidad, Clase, listentidadesAux, ontologia, dicPref, propListiedadesMultidioma, listaObjetosPropiedad);
			Clase.AppendLine();
			mMassiveOntologyClass.CrearToSearchGraphTriples(esPrincipal, pEntidad, pListaPropiedesSearch, pListaPadrePropiedadesAnidadas, Clase, ontologia, nombrePropDescripcion, nombrePropTitulo, nombrePropTituloEntero, propListiedadesMultidioma, listaObjetosPropiedad, listentidadesAux, dicPref, listaPropMultiidiomaFalse);
			Clase.AppendLine();
			mMassiveOntologyClass.CrearToAcidData(esPrincipal, pEntidad, ontologia, Clase, nombrePropDescripcion, nombrePropTitulo, nombrePropTituloEntero, propListiedadesMultidioma, listaObjetosPropiedad, listaPropMultiidiomaFalse);
			Clase.AppendLine();
			//-------------- 
			CrearGetURI(pEntidad);
			Clase.AppendLine();
			Clase.AppendLine();
			AgregarTituloRecurso(pEntidad);
			Clase.AppendLine();
			if (esPrincipal)
			{
				AgregarDescripcionRecurso(pEntidad);
			}
			Clase.AppendLine();
			Clase.AppendLine();
			Clase.AppendLine();
			AnadirImagenes(pEntidad);
			Clase.AppendLine();
			AnadirArchivos(pEntidad);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}}}");
			Clase.AppendLine("}");
			return Clase.ToString();
		}

		public XmlNode EspefPropiedad
		{
			get
			{
				XmlNodeList listaEspefPropiedad = null;
				if (doc.DocumentElement != null)
				{
					listaEspefPropiedad = doc.DocumentElement.SelectNodes("EspefPropiedad");
					if (listaEspefPropiedad != null)
					{
						if (listaEspefPropiedad.Count > 1)
						{
							foreach (XmlNode espef in listaEspefPropiedad)
							{

								if (espef.Attributes.Count > 0)
								{
									if (espef.Attributes[0].InnerText.Equals(proyID))
									{
										return espef;
									}
									else if (espef.Attributes[0].InnerText.Equals(nombreCortoProy))
									{
										return espef;
									}
								}
							}
							foreach (XmlNode espef in listaEspefPropiedad)
							{
								if (espef.Attributes.Count < 1)
								{
									return espef;
								}
							}
						}
					}
				}
				if (listaEspefPropiedad != null)
				{
					return listaEspefPropiedad.Item(0);
				}
				else { return null; }
			}
		}

		public XmlNode ConfiguracionGeneral
		{
			get
			{
				XmlNodeList listaconfig = null;
				if (doc.DocumentElement != null)
				{
					listaconfig = doc.DocumentElement.SelectNodes("ConfiguracionGeneral");

					if (listaconfig != null)
					{
						if (listaconfig.Count > 1)
						{
							foreach (XmlNode config in listaconfig)
							{
								if (config.Attributes.Count > 0)
								{
									if (config.Attributes[0].InnerText.Equals(proyID))
									{
										return config;
									}
									else if (config.Attributes[0].InnerText.Equals(nombreCortoProy))
									{
										return config;
									}
								}
							}
							foreach (XmlNode config in listaconfig)
							{
								if (config.Attributes.Count < 1)
								{
									return config;
								}
							}
						}
					}
				}
				if (listaconfig != null)
				{
					return listaconfig.Item(0);
				}
				else { return null; }
			}
		}

		public XmlNode EspefEntidad
		{
			get
			{
				XmlNodeList listaespef = null;
				if (doc.DocumentElement != null)
				{
					listaespef = doc.DocumentElement.SelectNodes("EspefEntidad");
					if (listaespef != null)
					{
						if (listaespef.Count > 1)
						{
							foreach (XmlNode espef in listaespef)
							{
								if (espef.Attributes.Count > 0)
								{
									if (espef.Attributes[0].InnerText.Equals(proyID))
									{
										return espef;
									}
									else if (espef.Attributes[0].InnerText.Equals(nombreCortoProy))
									{
										return espef;
									}
								}
							}
							foreach (XmlNode espef in listaespef)
							{
								if (espef.Attributes.Count < 1)
								{
									return espef;
								}
							}
						}
					}
				}
				if (listaespef != null)
				{
					return listaespef.Item(0);
				}
				else { return null; }
			}
		}

		/// <summary>
		/// Diferenciamos clases con herencia
		/// </summary>
		/// <param name="pEntidad"></param>
		public void EscribirHerencias(ElementoOntologia pEntidad)
		{
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}[ExcludeFromCodeCoverage]");
			if (ontologia.EntidadesAuxiliares.Contains(pEntidad) || pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
			{
				EstablecerClaseBaseSiTieneHerencia(pEntidad);
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{c.publicClass} {ObtenerEntidad(pEntidad)} : GnossOCBase");
			}
		}

		/// <summary>
		/// Obtenemos la entidad
		/// </summary>
		/// <param name="pEntidad"></param>
		/// <returns></returns>
		public string ObtenerEntidad(ElementoOntologia pEntidad)
		{
			return UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad);
		}

		/// <summary>
		/// Si tiene herencia.
		/// </summary>
		/// <param name="pEntidad"></param>
		public void EstablecerClaseBaseSiTieneHerencia(ElementoOntologia pEntidad)
		{
			if (pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{c.publicClass} {ObtenerEntidad(pEntidad)} : {UtilCadenasOntology.ObtenerNombreProp(pEntidad.Superclases[0])}");
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{c.publicClass} {ObtenerEntidad(pEntidad)} : GnossOCBase");
			}
		}

		/// <summary>
		/// Creamos las propiedades de cada clase, si en la ontología la propiedad está dentro del Dominio
		/// </summary>
		/// <param name="pEntidad"></param>
		public void CreacionDePropiedades(ElementoOntologia pEntidad)
		{
			bool tienePadre = pEntidad.Superclases.Count > 0 && pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing"));
			string modificadorSobrescritura = "virtual";

			if (tienePadre)
			{
				modificadorSobrescritura = "override";
			}

			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificadorSobrescritura} string RdfType {{ get {{ return \"{pEntidad.TipoEntidad}\"; }} }}");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificadorSobrescritura} string RdfsLabel {{ get {{ return \"{pEntidad.TipoEntidad}\"; }} }}");

			if (!esPrincipal)
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public string Identificador {{ get; set; }}");
			}

			if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public OntologyEntity Entity {{ get; set; }}");

				Clase.AppendLine();
			}

			ElementoOntologia padre = null;
			if (tienePadre)
			{
				padre = ontologia.Entidades.FirstOrDefault(ent => ent.TipoEntidad.Equals(pEntidad.Superclases[0]));
			}

			foreach (Propiedad propiedad in pEntidad.Propiedades)
			{
                if (EsMultiIdiomaConfig && propiedad.Rango.ToLower().Equals("http://www.w3.org/2001/XMLSchema#string"))
                {
                    if (!propListiedadesMultidioma.ContainsKey(propiedad.Nombre))
                    {
                        propListiedadesMultidioma.Add(propiedad.Nombre, true);
                    }
                }

                if (propiedad.Dominio.Contains(pEntidad.TipoEntidad))
				{
					string nombreProp = UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre);
					bool propMultiidioma = PropiedadConAtributoMultiidioma(propiedad, pEntidad);					
                    
                    Dictionary<string, string> dicLan = ObtenerLabels(pEntidad, propiedad);
					foreach (string pLan in dicLan.Keys)
					{
						if (listaIdiomas.Contains(pLan))
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}[LABEL(LanguageEnum.{pLan},\"{dicLan[pLan]}\")]");
						}
						if (!propMultiidioma)
						{
							break;
						}
					}
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}[RDFProperty(\"{propiedad.Nombre}\")]");
					if (propiedad.Tipo.ToString().Equals("ObjectProperty"))
					{
						if (ObtenerUsingExternos(propiedad).Count == 0 && !ontologia.Entidades.Exists(x => x.TipoEntidad.Equals(propiedad.Rango)) && !EsPersonaGnoss(propiedad))
						{
							throw new Exception($"la propiedad {propiedad.Nombre} de la entidad {propiedad.ElementoOntologia.TipoEntidad} no esta configurado correctamente. Revisa que el grafo del selector de entidad y la definición de la propiedad sea correcta.");
						}
						else
						{
							listaObjetosExternos.Add(nombreProp);
						}

						PropiedadesObjetoEntidad(propiedad, padre);
					}
					else if (propiedad.Tipo.ToString().Equals("DatatypeProperty"))
					{
						PropiedadesData(propiedad, nombreProp, padre, pEntidad);
					}
					Clase.AppendLine();
				}
			}
		}

		public bool PropiedadConAtributoMultiidioma(Propiedad pPropiedad, ElementoOntologia pEntidad)
		{
			bool esMultiidioma = true;

            if (EspefEntidad != null)
            {
                XmlNode nodoProp = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pEntidad.TipoEntidad}\"]");
                if (nodoProp != null)
                {
                    XmlNode atributoMultiidioma = nodoProp.SelectSingleNode($"MultiIdioma");

                    if (atributoMultiidioma != null)
                    {
						if (string.IsNullOrEmpty(atributoMultiidioma.InnerText))
						{
							return true;
						}
						bool.TryParse(atributoMultiidioma.InnerText, out esMultiidioma);
                    }
                }
            }

			if (!esMultiidioma)
			{
                listaPropMultiidiomaFalse.Add(pPropiedad, esMultiidioma);
            }
            
            return esMultiidioma;
        }

		public Dictionary<string, string> ObtenerLabels(ElementoOntologia pEntidad, Propiedad pPropiedad)
		{
			Dictionary<string, string> dicIdiomaLabel = new Dictionary<string, string>();
			if (EspefEntidad != null)
			{
				XmlNode propiedad = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pEntidad.TipoEntidad}\"]");

				if (propiedad != null)
				{
					XmlNodeList listaLabelsLectura = propiedad.SelectNodes("AtrNombreLectura");

					if (listaLabelsLectura != null)
					{					
						foreach (XmlNode label in listaLabelsLectura)
						{
							if (label.Attributes.Count > 0 && !dicIdiomaLabel.ContainsKey(label.Attributes[0].InnerText))
							{
								dicIdiomaLabel.Add(label.Attributes[0].InnerText, label.InnerText);
							}
							else
							{
								dicIdiomaLabel.Add("es", label.InnerText);
							}
						}
					}
					XmlNodeList listaLabels = propiedad.SelectNodes("AtrNombre");
					if (listaLabels != null)
					{
						foreach (XmlElement label in listaLabels)
						{
							if (label.Attributes.Count > 0 && !dicIdiomaLabel.ContainsKey(label.Attributes[0].InnerText))
							{
								dicIdiomaLabel.Add(label.Attributes[0].InnerText, label.InnerText);
							}
						}
					}
				}
			}
			return dicIdiomaLabel;
		}

		/// <summary>
		/// Añadimos al principio los usings que necesitamos 
		/// </summary>
		private void ObtenerUsings(ElementoOntologia pEntidad)
		{
			Clase.AppendLine("using System;");
			Clase.AppendLine("using System.IO;");
			Clase.AppendLine("using System.Collections.Generic;");
			Clase.AppendLine("using System.Linq;");
			Clase.AppendLine("using System.Text;");
			Clase.AppendLine("using System.Threading.Tasks;");
			Clase.AppendLine("using System.ComponentModel.DataAnnotations;");
			Clase.AppendLine("using Gnoss.ApiWrapper;");
			Clase.AppendLine("using Gnoss.ApiWrapper.Model;");
			Clase.AppendLine("using Gnoss.ApiWrapper.Helpers;");
			Clase.AppendLine("using GnossBase;");
			Clase.AppendLine("using Es.Riam.Gnoss.Web.MVC.Models;");
			Clase.AppendLine("using System.Text.RegularExpressions;");
			Clase.AppendLine("using System.Globalization;");
			Clase.AppendLine("using System.Collections;");
			Clase.AppendLine("using Gnoss.ApiWrapper.Exceptions;");
			Clase.AppendLine("using System.Diagnostics.CodeAnalysis;");

			List<string> listaUsingExt = listaUsingsExternos(pEntidad);
			foreach (string usin in listaUsingExt)
			{
				Clase.AppendLine(usin);
			}

			Clase.AppendLine();
		}

		/// <summary>
		/// Si la propiedad es única para Objetos externos
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pNombreClase"></param>
		public void PropiedadObjetoExternoValorUnico(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre)
		{
			if (!string.IsNullOrEmpty(pPropiedad.Rango))
			{
				string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
				bool useNew = false;
				if (pPropiedad.FunctionalProperty)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.required}");
				}
				if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
				{
					useNew = true;
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(useNew ? "new" : "")} {pPropiedad.Rango} {nombrePropiedad}  {c.getSet} ");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} string Id{nombrePropiedad}  {c.getSet} ");
			}
		}

		/// <summary>
		/// Si la propiedad es multiple para Objetos externos
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pNombreClase"></param>
		public void PropiedadObjetoExternoValorMultiple(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre)
		{

			if (!string.IsNullOrEmpty(pPropiedad.Rango))
			{
				string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
				bool usarNew = false;
				if (!pPropiedad.CardinalidadMaxima.Equals(-1))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.maxLeng}({pPropiedad.CardinalidadMaxima})]");
				}
				if (!pPropiedad.CardinalidadMinima.Equals(0))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.minLeng}({pPropiedad.CardinalidadMinima})]");
				}
				if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
				{
					usarNew = true;
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} List<{pPropiedad.Rango}> {nombrePropiedad} {c.getSet}");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} List<string> Ids{nombrePropiedad} {c.getSet}");
			}
		}

		/// <summary>
		/// Creacion de propiedades para tipos de Objetos externos
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pNombreClase"></param>
		public void CreacionPropiedadObjetosExternos(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre)
		{
			if (pPropiedad.ValorUnico)
			{
				propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.StringSimple);
				PropiedadObjetoExternoValorUnico(pPropiedad, pNombreClase, pPadre);
			}
			else
			{
				if (!propListiedadesTipo.ContainsKey(pPropiedad.Nombre))
				{
					propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.StringMultiple);
				}
				PropiedadObjetoExternoValorMultiple(pPropiedad, pNombreClase, pPadre);
			}
		}

		/// <summary>
		/// Si la propiedad es unica para objetos o tipos de la entidad
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pRango"></param>
		/// <param name="pNombreClase"></param>
		public void PropiedadObjetoEntidadValorUnico(Propiedad pPropiedad, string pRango, string pNombreClase, ElementoOntologia pPadre)
		{
			string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
			bool usarNew = false;
			if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
			{
				usarNew = true;
			}
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} {pRango} {nombrePropiedad} {c.getSet}");
		}

		/// <summary>
		/// Si la propiedad es multiple para objetos o tipos de la entidad
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pRango"></param>
		/// <param name="pNombreClase"></param>
		public void PropiedadObjetoEntidadValorMultiple(Propiedad pPropiedad, string pRango, string pNombreClase, ElementoOntologia pPadre)
		{
			string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
			bool usarNew = false;
			if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
			{
				usarNew = true;
			}
			if (!pPropiedad.CardinalidadMaxima.Equals(-1))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.maxLeng}({pPropiedad.CardinalidadMaxima})]");
			}
			if (!pPropiedad.CardinalidadMinima.Equals(0))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.minLeng}({pPropiedad.CardinalidadMinima})]");
			}
			if (pRango.Contains("Dictionary<"))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} {pRango} {nombrePropiedad} {c.getSet}");
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} List<{pRango}> {nombrePropiedad} {c.getSet}");
			}
		}

		/// <summary>
		/// Creacion de propiedades con los tipos u objetos de la entidad
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pRango"></param>
		/// <param name="pNombreClase"></param>
		public void CreacionPropiedadObjetosEntidad(Propiedad pPropiedad, string pRango, string pNombreClase, ElementoOntologia pPadre)
		{
			if (!pRango.Equals("Object"))
			{
				listentidades.Add(pPropiedad);
				if (ontologia.Entidades.Exists(x => x.TipoEntidad.Contains(pPropiedad.Rango)))
				{
					listentidadesAux.Add(pPropiedad);
				}
			}
			if (pPropiedad.ValorUnico)
			{
				PropiedadObjetoEntidadValorUnico(pPropiedad, pRango, pNombreClase, pPadre);
			}
			else
			{
				PropiedadObjetoEntidadValorMultiple(pPropiedad, pRango, pNombreClase, pPadre);
			}
		}

		/// <summary>
		/// Creacion de las propiedades para objeto 
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pNombrePropiedad"></param>
		public void PropiedadesObjetoEntidad(Propiedad pPropiedad, ElementoOntologia pPadre)
		{
			string rango = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango);

			if (ontologia.Entidades.Exists(x => x.TipoEntidad.Equals(pPropiedad.Rango)))
			{
				CreacionPropiedadObjetosEntidad(pPropiedad, rango, UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre), pPadre);
			}
			else
			{
				CreacionPropiedadObjetosExternos(pPropiedad, UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre), pPadre);
			}
		}

		/// <summary>
		/// Creacion de propiedades para datos
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <param name="pNombreClase"></param>
		public void PropiedadesData(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre, ElementoOntologia pEntidad)
		{
			if (!string.IsNullOrEmpty(pPropiedad.Rango))
			{
				string rango = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango);
				bool cadena = true;
				bool entero = false;
				bool esfecha = false;
				switch (rango.ToLower())
				{
					case "date":
					case "datetime":
					case "time":
						if (pPropiedad.ValorUnico)
						{
							if (pPropiedad.FunctionalProperty)
							{
								rango = "DateTime";
								propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.DateSimple);
							}
							else
							{
								rango = "DateTime?";
								propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.DateNull);
							}
						}
						else
						{
							rango = "DateTime";
							propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.DateMultiple);
						}
						cadena = false;
						break;
					case "boolean":
						rango = "bool";
						cadena = false;
						propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.Bool);
						break;
					case "float":
						if (pPropiedad.ValorUnico)
						{
							if (pPropiedad.FunctionalProperty)
							{
								rango = "float";
							}
							else
							{
								rango = "float?";
							}
						}
						else
						{
							rango = "float";
						}

						entero = true;
						break;
					case "int":
						if (pPropiedad.ValorUnico)
						{
							if (pPropiedad.FunctionalProperty)
							{
								rango = "int";
							}
							else
							{
								rango = "int?";
							}
						}
						else
						{
							rango = "int";
						};
						entero = true;
						break;
					default:
						rango = "string";
						break;
				}

				if (pPropiedad.ValorUnico)
				{
					if (cadena && !entero)
					{
						propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.StringSimple);
						if (!propListiedadesMultidioma.ContainsKey(pPropiedad.Nombre))
						{
							propListiedadesMultidioma.Add(pPropiedad.Nombre, GetMultiIdiomaPropiedad(pEntidad, pPropiedad));
						}
					}
					else if (entero && cadena)
					{
						propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.IntSimple);
					}
					if (mMassiveOntologyClass.EsPropiedadMultiIdioma(pPropiedad.Nombre, propListiedadesMultidioma) && !esfecha)
					{
						rango = "Dictionary<LanguageEnum,string>";
					}
					PropiedadObjetoEntidadValorUnico(pPropiedad, rango, pNombreClase, pPadre);
				}
				else
				{
					if (cadena && !entero)
					{
						propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.StringMultiple);
						if (!propListiedadesMultidioma.ContainsKey(pPropiedad.Nombre))
						{
							propListiedadesMultidioma.Add(pPropiedad.Nombre, GetMultiIdiomaPropiedad(pEntidad, pPropiedad));
						}
					}
					else if (entero && cadena)
					{
						propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.IntMultiple);
					}
					if (mMassiveOntologyClass.EsPropiedadMultiIdioma(pPropiedad.Nombre, propListiedadesMultidioma))
					{
						rango = "Dictionary<LanguageEnum,List<string>>";
					}
					PropiedadObjetoEntidadValorMultiple(pPropiedad, rango, pNombreClase, pPadre);
				}
			}
		}

		/// <summary>
		/// Obtenemos la clase externa por medio del xml
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <returns></returns>
		/// 
		public string ObtenerRangoDePropiedad(Propiedad pPropiedad)
		{
			string clase = string.Empty;

			if (EspefEntidad != null)
			{
				XmlNode nodo = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/Grafo")?[0];
				if (nodo != null)
				{
					string nombre = nodo.InnerText.Substring(0, nodo.InnerText.LastIndexOf("."));
					if (nombresOntologia.Any(n => n.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)))
					{
						XmlNode node = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/PropsEntLectura/Propiedad/@EntidadID")?[0];
						if (node != null)
						{
							clase = UtilCadenasOntology.ObtenerNombreProp(node.InnerText);
						}
						else
						{
							clase = CompletarTipoSeleccion(pPropiedad);
						}
					}
					else
					{
						clase = CompletarTipoSeleccion(pPropiedad);
					}
				}
				else
				{
					clase = CompletarTipoSeleccion(pPropiedad);

				}
				return clase;
			}
			else
			{
				return "object";
			}
		}

		public string CompletarTipoSeleccion(Propiedad pPropiedad)
		{
			string clase = "object";

			XmlNodeList tipoSeleccion = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/TipoSeleccion");
			if (tipoSeleccion != null)
			{
				if (tipoSeleccion.Count > 0 && tipoSeleccion[0].InnerText.Equals("Tesauro"))
				{
					clase = "Concept";
				}
			}
			return clase;
		}

		/// <summary>
		/// Creamos una lista con los diferentes usings
		/// </summary>
		/// <param name="pEntidad"></param>
		/// <returns></returns>
		public List<string> listaUsingsExternos(ElementoOntologia pEntidad)
		{
			List<string> listaUsing = new List<string>();
			foreach (Propiedad prop in pEntidad.Propiedades)
			{
				List<string> usingExternoProp = ObtenerUsingExternos(prop);
				if (usingExternoProp.Count > 0)
				{
					foreach (string usext in usingExternoProp)
					{
						if (!string.IsNullOrEmpty(usext) && !listaUsing.Contains(usext) && !nombreOnto.Equals(usext))
						{
							listaUsing.Add(usext);
						}
					}
				}
			}
			return listaUsing;
		}

		/// <summary>
		/// Obtenemos el titulo y la descripción de la clase , los cuales añadiremos en el toRecurso
		/// </summary>
		public void ObtenerTituloDescripcion()
		{
			XmlNode nodoTitulo = ConfiguracionGeneral.SelectSingleNode("TituloDoc");
			if (nodoTitulo != null)
			{
				nombrePropTituloEntero = nodoTitulo.InnerText;
				nombrePropTitulo = $"this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, nodoTitulo.InnerText, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(nodoTitulo.InnerText)}";
			}
			if (esPrincipal)
			{
				XmlNode nodoDescripcion = ConfiguracionGeneral.SelectSingleNode("DescripcionDoc");
				if (nodoDescripcion != null)
				{
					nombrePropDescripcionEntera = nodoDescripcion.InnerText;
					nombrePropDescripcion = $"this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, nodoDescripcion.InnerText, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(nodoDescripcion.InnerText)}";
				}
			}
		}

		/// <summary>
		/// obtener los using de las clases externas para poder utilizarlas
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <returns></returns>
		public List<string> ObtenerUsingExternos(Propiedad pPropiedad)
		{
			List<string> usings = new List<string>();
			string graf = "";

			if (EspefEntidad != null)
			{
				XmlNode nodeGrafo = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/Grafo");

				if (nodeGrafo != null)
				{
					string nombreGrafoCorrecto = nombresOntologia.FirstOrDefault(n => n.Equals(nodeGrafo.InnerText.Replace(".owl", ""), StringComparison.InvariantCultureIgnoreCase));
					if (!string.IsNullOrEmpty(nombreGrafoCorrecto))
					{
						graf = UtilCadenas.PrimerCaracterAMayuscula(nombreGrafoCorrecto).Split('.')[0] + "Ontology";
						usings.Add($"using {ObtenerRangoDePropiedad(pPropiedad)} = {graf}.{ObtenerRangoDePropiedad(pPropiedad)};");

					}
				}
			}
			return usings;
		}

		public bool EsPersonaGnoss(Propiedad pPropiedad)
		{
			if(EspefPropiedad != null)
			{
				XmlNode nodeGrafo = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/TipoSeleccion");
				if(nodeGrafo != null)
				{
					if (nodeGrafo.InnerText.Equals(PERSONA_GNOSS))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Creacion del toRecurso
		/// </summary>
		/// <param name="pEsPrimaria"></param>
		/// <param name="pEntidad"></param>
		public void CrearToRecurso(bool pEsPrimaria, ElementoOntologia pEntidad)
		{
			if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
			{
				string tipoOntologyResource = "ComplexOntologyResource";
				string modificador = "virtual";
				if (pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
				{
					modificador = "override";
				}
				if (!pEsPrimaria)
				{
					tipoOntologyResource = "SecondaryResource";
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI,string identificador)");
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return ToGnossApiResource(resourceAPI, listaDeCategorias, Guid.Empty, Guid.Empty);");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
					Clase.AppendLine("");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI, List<Guid> listaDeCategorias)");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return ToGnossApiResource(resourceAPI, null, Guid.Empty, Guid.Empty, listaDeCategorias);");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                    Clase.AppendLine("");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI, List<string> listaDeCategorias, Guid idrecurso, Guid idarticulo, List<Guid> listaIdDeCategorias = null)");
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{tipoOntologyResource} resource = new {tipoOntologyResource}();");
				CrearToRecursoPadre(pEsPrimaria, pEntidad, tipoOntologyResource);
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
			}
		}

		/// <summary>
		/// Crear el interior del toRecurso
		/// </summary>
		/// <param name="pEsPrimaria"></param>
		/// <param name="pEntidad"></param>
		public void CrearToRecursoPadre(bool pEsPrimaria, ElementoOntologia pEntidad, string pTipoOntologyResource)
		{
			if (pTipoOntologyResource.Equals("ComplexOntologyResource"))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}Ontology ontology = null;");
				if (listentidades.Any() || pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetEntities();");
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetProperties();");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(idrecurso.Equals(Guid.Empty) && idarticulo.Equals(Guid.Empty))");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList);");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ontology = new Ontology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, RdfType, RdfsLabel, prefList, propList, entList,idrecurso,idarticulo);");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Id = GNOSSID;");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Ontology = ontology;");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.TextCategories = listaDeCategorias;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.CategoriesIds = listaIdDeCategorias;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddResourceTitle(resource);");
				if (!string.IsNullOrEmpty(nombrePropDescripcionEntera))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddResourceDescription(resource);");
				}
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}List<SecondaryEntity> listSecondaryEntity = null;");
				if (listentidades.Any() || pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetEntities();");
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetProperties();");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SecondaryOntology ontology = new SecondaryOntology(resourceAPI.GraphsUrl, resourceAPI.OntologyUrl, \"{pEntidad.TipoEntidad}\", \"{pEntidad.TipoEntidad}\", prefList, propList,identificador,listSecondaryEntity, {(listentidades.Any() ? "entList" : "null")});");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.SecondaryOntology = ontology;");
			}
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddImages(resource);");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddFiles(resource);");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return resource;");
		}

		public void CrearGetURI(ElementoOntologia pEntidad)
		{
			if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
			{
				//string modificador = "virtual";
				string modificador = "override";

				if (pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
				{
					modificador = "override";
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificador} string GetURI(ResourceApi resourceAPI)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return $\"{{resourceAPI.GraphsUrl}}items/{nombreOnto}_{{ResourceID}}_{{ArticleID}}\";");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
			}
		}

		private void GenerarSearchPropiedadAnidada(string pPropiedadSearchAnidada, Propiedad pPropiedad, string pNombreVariableEntidadActual, int pContadorLlamadas, ElementoOntologia pEntidad)
		{
			string[] listaPropiedadesAnidadas = pPropiedadSearchAnidada.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

			if (pEntidad.Propiedades.Any(item => item.NombreConNamespace.Equals(listaPropiedadesAnidadas[pContadorLlamadas])))
			{
				ElementoOntologia claseRepresentaPropiedad = pEntidad.EntidadesRelacionadas.Where(item => item.TipoEntidad.Equals(UtilCadenas.PrimerCaracterAMayuscula(pPropiedad.TipoEntidadRepresenta))).FirstOrDefault();
				Propiedad propiedadHija = claseRepresentaPropiedad.Propiedades.Where(item => item.NombreConNamespace.Equals(listaPropiedadesAnidadas[pContadorLlamadas])).FirstOrDefault();

				string nombreVariableActual = $"{pNombreVariableEntidadActual}.{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[pContadorLlamadas]).Replace(":", "_")}";
				if (propiedadHija.ValorUnico)
				{
					if (mMassiveOntologyClass.EsPropiedadMultiIdioma(propiedadHija.Nombre, propListiedadesMultidioma) || mMassiveOntologyClass.EsPropiedadExternaMultiIdioma(claseRepresentaPropiedad, propiedadHija, listaObjetosPropiedad))
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}foreach (LanguageEnum idioma in {nombreVariableActual}.Keys)");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}search += \"{{idioma}} \"");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
					}
					else
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({nombreVariableActual} != null)");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
						if (pContadorLlamadas < listaPropiedadesAnidadas.Count() - 1)
						{
							GenerarSearchPropiedadAnidada(pPropiedadSearchAnidada, propiedadHija, nombreVariableActual, pContadorLlamadas++, claseRepresentaPropiedad);
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += \"{{{nombreVariableActual}}} \"");
						}
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
					}
				}
				else
				{
					if (mMassiveOntologyClass.EsPropiedadMultiIdioma(propiedadHija.Nombre, propListiedadesMultidioma) || mMassiveOntologyClass.EsPropiedadExternaMultiIdioma(claseRepresentaPropiedad, propiedadHija, listaObjetosPropiedad))
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}foreach (LanguageEnum idioma in {nombreVariableActual}.Keys)");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}foreach (List<string> valor in {nombreVariableActual}[idioma])");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{{");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}search += \"{{valor}} \"");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
					}
					else
					{
						if (pContadorLlamadas < listaPropiedadesAnidadas.Count() - 1)
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach({propiedadHija.TipoEntidadRepresenta} item_{pContadorLlamadas + 2} in {nombreVariableActual})");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
							GenerarSearchPropiedadAnidada(pPropiedadSearchAnidada, propiedadHija, nombreVariableActual, pContadorLlamadas++, claseRepresentaPropiedad);
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach({propiedadHija.Rango} item_{pContadorLlamadas + 2} in {nombreVariableActual})");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += \"{{item_{pContadorLlamadas + 2}}} \"");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
						}
					}
				}
			}
		}

		public void AgregarTituloRecurso(ElementoOntologia pEntidad)
		{
			if ((!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count) && !pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal void AddResourceTitle(ComplexOntologyResource resource)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
				if (esPrincipal)
				{
					if (mMassiveOntologyClass.EsPropiedadMultiIdioma(this.nombrePropTituloEntero, propListiedadesMultidioma))
					{
						Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero));

						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}List<Multilanguage> multiTitleList = new List<Multilanguage>();");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach (LanguageEnum idioma in {nombrePropTitulo}.Keys)");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
						if (propiedad.ValorUnico)
						{

							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}multiTitleList.Add(new Multilanguage({nombrePropTitulo}[idioma], idioma.ToString()));");
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}multiTitleList.Add(new Multilanguage(string.Join(\", \", {nombrePropTitulo}[idioma]), idioma.ToString()));");
						}
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.MultilanguageTitle = multiTitleList;");

					}
					else
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Title = {nombrePropTitulo};");
					}
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
			}
		}

		public void AgregarDescripcionRecurso(ElementoOntologia pEntidad)
		{
			if ((!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count) && !pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
			{
				if (!string.IsNullOrEmpty(this.nombrePropDescripcionEntera))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal void AddResourceDescription(ComplexOntologyResource resource)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");

					if (mMassiveOntologyClass.EsPropiedadMultiIdioma(this.nombrePropDescripcionEntera, propListiedadesMultidioma))
					{
						Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropDescripcionEntera));

						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}List<Multilanguage> listMultilanguageDescription = new List<Multilanguage>();");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach (LanguageEnum idioma in {nombrePropDescripcion}.Keys)");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
						if (propiedad.ValorUnico)
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}listMultilanguageDescription.Add(new Multilanguage({nombrePropDescripcion}[idioma], idioma.ToString()));");
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}listMultilanguageDescription.Add(new Multilanguage(string.Join(\", \", {nombrePropDescripcion}[idioma]), idioma.ToString()));");
						}
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.MultilanguageDescription = listMultilanguageDescription;");
					}
					else
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Description = {nombrePropDescripcion};");
					}
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
				}
			}
		}

		/// <summary>
		/// Creamos el metodo obtenerPropiedades para cuando tenga multiidioma
		/// </summary>
		/// <param name="pEntidad"></param>
		public void ObtenerPropiedadesPadre(ElementoOntologia pEntidad)
		{
			List<string> propImagenes = ObtenerNombrePropiedades(ObtenerPropiedadesImagen(pEntidad));
			List<string> propArchivos = ObtenerNombrePropiedades(ObtenerPropiedadesArchivo(pEntidad));

			foreach (KeyValuePair<string, PropiedadTipo> propiedad in propListiedadesTipo)
			{
				if (!propImagenes.Contains(propiedad.Key) && !propArchivos.Contains(propiedad.Key))
				{
					string prefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Key, mLoggingService));
					string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(propiedad.Key);
					string tipoPropiedad = "";
					string aux = "";
					string id = "";
					string thi = "this.";
					bool nulable = false;
					int n = 0;
					switch (propiedad.Value)
					{
						case PropiedadTipo.StringSimple:
							tipoPropiedad = "StringOntologyProperty";
							if (listaObjetosExternos.Contains(UtilCadenasOntology.ObtenerNombreProp(propiedad.Key)))
							{
								id = "Id";
							}
							break;
						case PropiedadTipo.StringMultiple:
							tipoPropiedad = "ListStringOntologyProperty";
							if (listaObjetosExternos.Contains(UtilCadenasOntology.ObtenerNombreProp(propiedad.Key)))
							{
								id = "Ids";
							}
							break;
						case PropiedadTipo.Bool:
							tipoPropiedad = "BoolOntologyProperty";
							break;
						case PropiedadTipo.DateMultiple:
						case PropiedadTipo.DateSimple:
							tipoPropiedad = "DateOntologyProperty";
							break;
						case PropiedadTipo.IntSimple:
							tipoPropiedad = "StringOntologyProperty";
							aux = ".ToString()";
							break;
						case PropiedadTipo.DateNull:
							tipoPropiedad = "DateOntologyProperty";
							aux = ".Value";
							nulable = true;
							break;
						case PropiedadTipo.IntMultiple:
							thi = "";
							tipoPropiedad = "ListStringOntologyProperty";
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}List<string> {prefijoPropiedad}_{nombrePropiedad}String = new List<string>();");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (this.{prefijoPropiedad}_{nombrePropiedad} != null)");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{prefijoPropiedad}_{nombrePropiedad}String.AddRange(Array.ConvertAll(this.{prefijoPropiedad}_{nombrePropiedad}.ToArray() , element => element.ToString()){aux});");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
							aux = "String";
							break;
					}
					if (propiedad.Value == PropiedadTipo.DateMultiple)
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach (DateTime fecha in {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}){{");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + n)}propList.Add(new {tipoPropiedad}(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", fecha));");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");

					}
					else
					{
						if (mMassiveOntologyClass.EsPropiedadMultiIdioma(propiedad.Key, propListiedadesMultidioma))
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux} != null)");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (LanguageEnum idioma in {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}.Keys)");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}propList.Add(new {tipoPropiedad}(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}[idioma], idioma.ToString()));");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
							Propiedad propiedadCompleta = pEntidad.Propiedades.Where(item => item.Nombre.Equals(propiedad.Key)).FirstOrDefault();
							if (propiedadCompleta != null)
							{
								if (propiedadCompleta.CardinalidadMinima > 0 || propiedadCompleta.FunctionalProperty)
								{
									Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else");
									Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
									Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}throw new GnossAPIException($\"La propiedad {ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)} debe tener al menos un valor en el recurso: {{resourceID}}\");");
									Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
								}
							}
						}
						else if (!EsImagen(pEntidad, propiedad.Key))
						{
							if (nulable)
							{
								n = 1;
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if ({thi}{id}{prefijoPropiedad}_{nombrePropiedad}.HasValue){{");
							}
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3 + n)}propList.Add(new {tipoPropiedad}(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
							if (nulable) { Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}"); }
						}
						else if (!EsArchivo(pEntidad, propiedad.Key))
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.Add(new {tipoPropiedad}(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
						}
					}
				}
			}
		}

		/// <summary>
		/// Metodo interno de cada clase para obtener las propiedades
		/// </summary>
		/// <param name="ontologia"></param>
		/// <param name="pEntidad"></param>
		public void ObtenerPropiedades(ElementoOntologia pEntidad)
		{
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal override void GetProperties()");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}base.GetProperties();");
			ObtenerPropiedadesPadre(pEntidad);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
			Clase.AppendLine();
		}

		/// <summary>
		/// Creamos el metodo ObtenerEntidades dentro de la clase . Si es hijo sobreescribirá el metodo, obteniendo los elementos del pPadre
		/// </summary>
		/// <param name="ontologia"></param>
		/// <param name="pEntidad"></param>
		public void ObtenerEntidades(ElementoOntologia pEntidad)
		{
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal override void GetEntities()");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}base.GetEntities();");
			ObtenerEntidadesPadre(pEntidad);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}} ");
		}

		/// <summary>
		/// Obtenermos las entidades del Padre para usar en el ToRecurso
		/// </summary>
		/// <param name="ontologia"></param>
		/// <param name="pEntidad"></param>
		public void ObtenerEntidadesPadre(ElementoOntologia pEntidad)
		{
			foreach (Propiedad prop in listentidadesAux)
			{
				string rango = UtilCadenasOntology.ObtenerNombreProp(prop.Rango);
				if (!rango.Equals("object"))
				{
					string obtenerPrefijo = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService));
					string nombreProp = UtilCadenasOntology.ObtenerNombreProp(prop.Nombre);
					if (prop.ValorUnico)
					{
						if (prop.CardinalidadMaxima == 1)
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({obtenerPrefijo}_{nombreProp}!=null){{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{obtenerPrefijo}_{nombreProp}.GetProperties();");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{obtenerPrefijo}_{nombreProp}.GetEntities();");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}OntologyEntity entity{obtenerPrefijo}_{nombreProp} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", {obtenerPrefijo}_{nombreProp}.propList, {obtenerPrefijo}_{nombreProp}.entList);");
                            if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(prop.Rango)))
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{obtenerPrefijo}_{nombreProp}.Entity = entity{obtenerPrefijo}_{nombreProp};");
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}entList.Add(entity{obtenerPrefijo}_{nombreProp});");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{obtenerPrefijo}_{nombreProp}.GetProperties();");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{obtenerPrefijo}_{nombreProp}.GetEntities();");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}OntologyEntity entity{obtenerPrefijo}_{nombreProp} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", {obtenerPrefijo}_{nombreProp}.propList, {obtenerPrefijo}_{nombreProp}.entList);");
                            if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(prop.Rango)))
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{obtenerPrefijo}_{nombreProp}.Entity = entity{obtenerPrefijo}_{nombreProp};");
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}entList.Add(entity{obtenerPrefijo}_{nombreProp});");
						}
					}
					else
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({obtenerPrefijo}_{nombreProp}!=null){{");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach({rango} prop in {obtenerPrefijo}_{nombreProp}){{");
						//Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}List<OntologyProperty> propListiedades{rango} = prop.ObtenerPropiedades();");
						// Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}List<OntologyEntity> entListidades{rango} = new List<OntologyEntity>()");
						//TODO
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.GetProperties();");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.GetEntities();");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}OntologyEntity entity{rango} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", prop.propList, prop.entList);");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}entList.Add(entity{rango});");
						if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(prop.Rango)))
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}prop.Entity = entity{rango};");
						}
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
					}
				}
			}
		}

		/// <summary>
		/// Comprobamos si la propiedad pasada por parametro es multiidioma
		/// </summary>
		/// <param name="pPropiedad"></param>
		/// <returns></returns>
		public bool GetMultiIdiomaPropiedad(ElementoOntologia pEntidad, Propiedad pPropiedad)
		{
			ObjetoPropiedad objeto = listaObjetosPropiedad.FirstOrDefault(x => x.NombrePropiedad.Equals(pPropiedad.Nombre) && x.NombreEntidad.Equals(pEntidad.TipoEntidad) && x.NombreOntologia.ToLower().Equals(mRdfType.ToLower()));
			if (objeto != null)
			{
				return objeto.Multiidioma;
			}
			return false;
		}

		/// <summary>
		/// Comprobamos si es multiidioma el xml
		/// </summary>
		/// <returns></returns>
		public bool EsMultiIdiomaConfig
		{
			get
			{
				if (ConfiguracionGeneral != null)
				{
					if (!mEsMultiIdiomaConfig.HasValue)
					{
						foreach (XmlNode multi in ConfiguracionGeneral.ChildNodes)
						{
							if (multi.Name.Equals("MultiIdioma"))
							{
								mEsMultiIdiomaConfig = true;
								return mEsMultiIdiomaConfig.Value;
							}
						}
						mEsMultiIdiomaConfig = false;
					}
					return mEsMultiIdiomaConfig.Value;
				}
				return false;

			}
		}

		/// <summary>
		/// Obtenemos un string de la forma prefijo:propiedad
		/// </summary>
		/// <param name="pDicPref"></param>
		/// <param name="pRang"></param>
		/// <returns></returns>
		public string ObtenerPrefijoYPropiedad(Dictionary<string, string> pDicPref, string pRang)
		{
			return UtilCadenasOntology.ObtenerPrefijo(pDicPref, pRang, mLoggingService) + ":" + UtilCadenasOntology.ObtenerNombrePropSinNamespace(pRang);
		}

		public List<Propiedad> ObtenerPropiedadesImagen(ElementoOntologia pEntidad)
		{
			List<Propiedad> propListiedadesImagenes = new List<Propiedad>();
			if (EspefEntidad != null)
			{
				XmlNodeList imagenes = EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"Imagen\"]");
				if (imagenes != null)
				{
					foreach (XmlNode imagen in imagenes)
					{
						XmlNode attrID = imagen.SelectSingleNode("@ID");

						Propiedad propiedadImagen = pEntidad.Propiedades.FirstOrDefault(propiedad => propiedad.Nombre.Equals(attrID.Value));
						if (propiedadImagen != null)
						{
							propListiedadesImagenes.Add(propiedadImagen);
						}
					}
				}
			}
			return propListiedadesImagenes;
		}

		public List<string> ObtenerNombrePropiedades(List<Propiedad> pListaPropiedades)
		{
			List<string> listaNombres = new List<string>();

			foreach (Propiedad propiedad in pListaPropiedades)
			{
				listaNombres.Add(propiedad.Nombre);
			}

			return listaNombres;
		}

		public List<Propiedad> ObtenerPropiedadesArchivo(ElementoOntologia pEntidad)
		{
			List<Propiedad> propListEntidadesArchivos = new List<Propiedad>();
			if (EspefPropiedad != null)
			{
				if (EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"Archivo\"]/@ID") != null || EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"ArchivoLink\"]/@ID") != null)
				{
					XmlNodeList archivos = EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"Archivo\"]/@ID");
					if (archivos == null)
					{
						archivos = EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"Archivo\"]/@ID");
					}

					if (archivos != null)
					{
						foreach (XmlAttribute archivo in archivos)
						{
							Propiedad propiedadArchivo = pEntidad.Propiedades.FirstOrDefault(propiedad => propiedad.Nombre.Equals(archivo.Value));

							if (propiedadArchivo != null)
							{
								propListEntidadesArchivos.Add(propiedadArchivo);
							}
						}
					}
				}
			}
			return propListEntidadesArchivos;
		}

		public void AnadirImagenes(ElementoOntologia pEntidad)
		{
			List<Propiedad> listapropimage = ObtenerPropiedadesImagen(pEntidad);
			bool entidadesAuxConPropImagen = EntidadesAuxiliaresConPropImagen(pEntidad);

			if (listapropimage.Any() || entidadesAuxConPropImagen)
			{
				string tipo;
				if (esPrincipal)
				{
					tipo = "ComplexOntologyResource";

					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal override void AddImages({tipo} pResource)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}base.AddImages(pResource);");

					if (listapropimage != null && listapropimage.Count > 0)
					{
						foreach (Propiedad propiedad in listapropimage)
						{
							if (propiedad.ValorUnico)
							{
								string nombre = $"this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre)}";
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (!string.IsNullOrEmpty({nombre}))");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                                AnadirImagenesSimples(propiedad, pEntidad, nombre);
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                            }
							else
							{
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre)} != null)");
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach(string prop in this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre)})");
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
								AnadirImagenesMultiples(propiedad, pEntidad, "prop");
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
								Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
							}
						}
					}
					AnadirImagenesEntidades(pEntidad);
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
				}
			}
		}

		public bool EntidadesAuxiliaresConPropImagen(ElementoOntologia pEntidad)
		{
			foreach (ElementoOntologia entidadAux in pEntidad.Ontologia.EntidadesAuxiliares)
			{
				List<Propiedad> propImagen = ObtenerPropiedadesImagen(entidadAux);
				if (propImagen.Any())
				{
					return true;
				}
			}

			return false;
		}

		public void AnadirImagenesSimples(Propiedad pPropiedad, ElementoOntologia pEntidad, string pNombre)
		{
			string nombreProp = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre);
			string prefijoYPropiedad = ObtenerPrefijoYPropiedad(dicPref, pPropiedad.Nombre);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}List<ImageAction> actionList{nombreProp} = new List<ImageAction>();");
			foreach (List<string> listaAcc in ImageAction(pPropiedad, pEntidad).Values)
			{
				for (int i = 0; i < listaAcc.Count; i = i + 3)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}actionList{nombreProp}.Add(new ImageAction({listaAcc[i + 2]},{listaAcc[i + 1]}, {listaAcc[i]}, 100));");
				}
			}
			if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachImage({pNombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", true, this.GetExtension({pNombre}), this.Entity);");
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachImage({pNombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", true, this.GetExtension({pNombre}));");
			}
		}

		public void AnadirImagenesMultiples(Propiedad propiedad, ElementoOntologia pEntidad, string nombre)
		{
			string nombreProp = UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre);
			string prefijoYPropiedad = ObtenerPrefijoYPropiedad(dicPref, propiedad.Nombre);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}List<ImageAction> actionList{nombreProp} = new List<ImageAction>();");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}bool principal=true;");
			foreach (List<string> listaAcc in ImageAction(propiedad, pEntidad).Values)
			{
				for (int i = 0; i < listaAcc.Count; i = i + 3)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}actionList{nombreProp}.Add(new ImageAction({listaAcc[i + 2]},{listaAcc[i + 1]}, {listaAcc[i]}, 100));");
				}
			}
			if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachImage({nombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", principal, this.GetExtension({nombre}),this.Entidad);");
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachImage({nombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", principal, this.GetExtension({nombre}));");
			}
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}principal = false;");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pEntidad"></param>
		public void AnadirArchivos(ElementoOntologia pEntidad)
		{
			List<Propiedad> propTipoArchivo = ObtenerPropiedadesArchivo(pEntidad);
			bool propArchivoEnAuxiliares = EntidadesAuxiliaresConPropArchivo(pEntidad);

			if (propTipoArchivo.Any() || propArchivoEnAuxiliares)
			{
				string tipo;
				if (esPrincipal)
				{
					tipo = "ComplexOntologyResource";
				}
				else
				{
					tipo = "SecondaryResource";
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal override void AddFiles({tipo} pResource)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}base.AddFiles(pResource);");

				foreach (Propiedad propiedad in propTipoArchivo)
				{
					ObjetoPropiedad objeto = listaObjetosPropiedad.FirstOrDefault(x => x.NombrePropiedad.Equals(propiedad.Nombre) && x.NombreEntidad.Equals(propiedad.ElementoOntologia.TipoEntidad));
					string prefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, objeto.NombrePropiedad, mLoggingService));
					string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(objeto.NombrePropiedad);
					if (!objeto.Multivaluada)
					{
						string nombre = $"{prefijoPropiedad}_{nombrePropiedad}";
						AnadirArchivosSimples(objeto, pEntidad, nombre);
					}
					else
					{
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(this.{prefijoPropiedad}_{nombrePropiedad} != null)");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach(string prop in this.{prefijoPropiedad}_{nombrePropiedad})");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
						AnadirArchivosSimples(objeto, pEntidad, "prop");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
						Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
					}
				}
				AnadirArchivosEntidades(pEntidad);
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
			}
		}

		public bool EntidadesAuxiliaresConPropArchivo(ElementoOntologia pEntidad)
		{
			foreach (ElementoOntologia entidadAux in pEntidad.Ontologia.EntidadesAuxiliares)
			{
				List<Propiedad> propArchivo = ObtenerPropiedadesArchivo(entidadAux);
				if (propArchivo.Any())
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propiedad"></param>
		/// <param name="pEntidad"></param>
		/// <param name="nombre"></param>

		public void AnadirArchivosSimples(ObjetoPropiedad objeto, ElementoOntologia pEntidad, string nombre)
		{
			string idioma = "";
			if (objeto.Multiidioma)
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach (LanguageEnum lan in Enum.GetValues(typeof(LanguageEnum))){{");
				idioma = "[lan]";
			}
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}FileStream file{nombre.Substring(nombre.LastIndexOf(".") + 1)} = System.IO.File.Create({nombre}{idioma});");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}long length{nombre.Substring(nombre.LastIndexOf(".") + 1)} = file{nombre.Substring(nombre.LastIndexOf(".") + 1)}.Length;");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}byte[] data{nombre.Substring(nombre.LastIndexOf(".") + 1)} = new byte[length{nombre.Substring(nombre.LastIndexOf(".") + 1)}];");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}file{nombre.Substring(nombre.LastIndexOf(".") + 1)}.Read(data{nombre.Substring(nombre.LastIndexOf(".") + 1)}, 0, Convert.ToInt32(length{nombre.Substring(nombre.LastIndexOf(".") + 1)}));");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}string name{nombre.Substring(nombre.LastIndexOf(".") + 1)} = {nombre.Substring(nombre.LastIndexOf(".") + 1)}{idioma};");

			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}pResource.AttachFile(data{nombre.Substring(nombre.LastIndexOf(".") + 1)}, \"{objeto.NombrePropiedad}\", name{nombre.Substring(nombre.LastIndexOf(".") + 1)});");
			if (objeto.Multiidioma)
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pEntidad"></param>
		public void AnadirImagenesEntidades(ElementoOntologia pEntidad)
		{
			List<string> propiedadesYaAnadidas = new List<string>();
			foreach (ElementoOntologia entidadAuxiliar in pEntidad.Ontologia.EntidadesAuxiliares)
			{
				foreach (Propiedad prop in listentidades)
				{
					if (!prop.Rango.Equals("object") && !propiedadesYaAnadidas.Contains(prop.Nombre))
					{
						if (prop.ValorUnico)
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}.AddImages(pResource);");
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)} != null)");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach ({UtilCadenasOntology.ObtenerNombreProp(prop.Rango)} prop in {UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)})");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.AddImages(pResource);");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
						}
					}

					propiedadesYaAnadidas.Add(prop.Nombre);
				}
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pEntidad"></param>
		public void AnadirArchivosEntidades(ElementoOntologia pEntidad)
		{
			List<string> propYaAnadidas = new List<string>();
			foreach (ElementoOntologia entidadAuxiliar in pEntidad.Ontologia.EntidadesAuxiliares)
			{
				foreach (Propiedad prop in listentidades)
				{
					if (!prop.Rango.Equals("object") && !propYaAnadidas.Contains(prop.Nombre))
					{
						if (prop.ValorUnico)
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}.AddFiles(pResource);");
						}
						else
						{
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)} != null)");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach({UtilCadenasOntology.ObtenerNombreProp(prop.Rango)} prop in {UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)})");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.AddFiles(pResource);");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
							Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
						}
					}

					propYaAnadidas.Add(prop.Nombre);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propiedad"></param>
		/// <returns></returns>
		public Dictionary<string, List<string>> ImageAction(Propiedad propiedad, ElementoOntologia pEntidad)
		{
			Dictionary<string, List<string>> accionesImagen = new Dictionary<string, List<string>>();
			List<string> actionList = new List<string>();

			XmlNodeList accionesXML = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{propiedad.Nombre}\" and @EntidadID=\"{propiedad.ElementoOntologia}\"]/ImgMiniVP");

			if (accionesXML.Count > 0)
			{
				XmlNode acc = accionesXML.Item(0);
				if (acc != null)
				{
					XmlNodeList size = acc.SelectNodes("Size");
					XmlNodeList alto;
					XmlNodeList ancho;
					foreach (XmlElement siz in size)
					{
						switch (siz.GetAttribute("Tipo"))
						{
							case "RecorteCuadrado":
								actionList.Add("ImageTransformationType.Crop");
								alto = siz.GetElementsByTagName("Alto");
								foreach (XmlElement alt in alto)
								{
									actionList.Add(alt.InnerText);
								}
								actionList.Add("0");
								break;
							default:
								actionList.Add("ImageTransformationType.ResizeToWidth");
								alto = siz.GetElementsByTagName("Alto");
								foreach (XmlElement alt in alto)
								{
									actionList.Add(alt.InnerText);
								}
								ancho = siz.GetElementsByTagName("Ancho");
								foreach (XmlElement anc in ancho)
								{
									actionList.Add(anc.InnerText);
								}
								break;
						}
					}
					accionesImagen.Add(propiedad.Nombre, actionList);
				}
			}
			return accionesImagen;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pEntidad"></param>
		/// <param name="propiedad"></param>
		/// <returns></returns>
		public bool EsImagen(ElementoOntologia pEntidad, string propiedad)
		{
			List<Propiedad> propListImagen = ObtenerPropiedadesImagen(pEntidad);
			if (propListImagen.Any(prop2 => prop2.Nombre.Equals(propiedad)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pEntidad"></param>
		/// <param name="propiedad"></param>
		/// <returns></returns>
		public bool EsArchivo(ElementoOntologia pEntidad, string propiedad)
		{
			List<Propiedad> propListArchivo = ObtenerPropiedadesArchivo(pEntidad);
			if (propListArchivo.Any(prop2 => prop2.Nombre.Equals(propiedad)))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Genera un constructor vacío para las entidades principales, para las secundarias genera el constructor solicitandole la URI.
		/// </summary>
		/// <param name="pEntidad"></param>
		public void ConstructorSencillo(ElementoOntologia pEntidad)
		{
			if (esPrincipal)
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}() : base() {{ }} ");
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}(string pIdentificador) : base()");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}Identificador = pIdentificador;");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
			}
		}

		/// <summary>
		/// Genera un constructor que utilizarán entre sí las clases generadas para crear las variables referentes a las propiedades de objetos semánticos
		/// </summary>
		/// <param name="pEntidad"></param>
		public void ConstructorComplejo(ElementoOntologia pEntidad)
		{
			if (esPrincipal && (!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count))
			{
				if (pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base(pSemCmsModel,idiomaUsuario)");
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()");
				}
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;");
				RellenarConstructorInverso(pEntidad);
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
				Clase.AppendLine();
			}

			if (pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base(pSemCmsModel,idiomaUsuario)");
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()");
			}
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}mGNOSSID = pSemCmsModel.Entity.Uri;");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}mURL = pSemCmsModel.Properties.FirstOrDefault(p => p.PropertyValues.Any(prop => prop.DownloadUrl != null))?.FirstPropertyValue.DownloadUrl;");
			RellenarConstructorInverso(pEntidad);
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
		}

		public void RellenarConstructorInverso(ElementoOntologia pEntidad)
		{
			foreach (Propiedad pPropiedad in pEntidad.Propiedades)
			{
				if (string.IsNullOrEmpty(pPropiedad.Rango))
				{
					pPropiedad.Rango = ObtenerRangoDePropiedad(pPropiedad);
				}
				if (!pPropiedad.Rango.Equals("object"))
				{
					ElementoOntologia padre = null;
					if (pEntidad.Superclases.Count > 0 && pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
					{
						padre = ontologia.Entidades.FirstOrDefault(ent => ent.TipoEntidad.Equals(pEntidad.Superclases[0]));
					}
					if (padre == null || !padre.Propiedades.Any(x => x.Nombre.Equals(pPropiedad.Nombre)))
					{
						RellenarConstructorInversoContenido(pPropiedad);
					}
				}
			}
		}

		public void RellenarConstructorInversoContenido(Propiedad pPropiedad)
		{
			if (pPropiedad.Tipo.ToString().Equals("ObjectProperty"))
			{
				ConstructorInversoPropiedadObjeto(pPropiedad);
			}
			else if (pPropiedad.Tipo.ToString().Equals("DatatypeProperty"))
			{
				ConstructorInversoPropiedadData(pPropiedad);
			}
		}

		public void ConstructorInversoPropiedadObjeto(Propiedad pPropiedad)
		{
			if (pPropiedad.ValorUnico)
			{
				ConstructorInversoPropiedadObjetoValorUnico(pPropiedad);
			}
			else
			{
				ConstructorInversoPropiedadObjetoValorMultiple(pPropiedad);
			}
		}

		public void ConstructorInversoPropiedadObjetoValorMultiple(Propiedad pPropiedad)
		{
			string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad);
			string nombreProp = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre);
			string nombreRango = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango);
			if (string.IsNullOrEmpty(nombreRango))
			{
				nombreRango = "object";
			}
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{prefijoPropiedad}_{nombreProp} = new List<{nombreRango}>();");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombreProp} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(prop{prefijoPropiedad}_{nombreProp} != null && prop{prefijoPropiedad}_{nombreProp}.PropertyValues.Count > 0)");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombreProp}.PropertyValues)");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}if(propValue.RelatedEntity!=null){{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{nombreRango} {UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre, mLoggingService)}_{nombreProp} = new {nombreRango}(propValue.RelatedEntity,idiomaUsuario);");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{prefijoPropiedad}_{nombreProp}.Add({UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre, mLoggingService)}_{nombreProp});");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
		}

		public void ConstructorInversoPropiedadObjetoValorUnico(Propiedad pPropiedad)
		{
			string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad).Trim();
			string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre).Trim();
			string nombreRango = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Trim();
			if (string.IsNullOrEmpty(nombreRango))
			{
				nombreRango = "object";
			};
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0 && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues[0].RelatedEntity != null)");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{prefijoPropiedad}_{nombrePropiedad} = new {nombreRango}(prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues[0].RelatedEntity,idiomaUsuario);");

			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
		}

		public void ConstructorInversoPropiedadData(Propiedad pPropiedad)
		{
			if (UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Equals("date") || UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Equals("dateTime"))
			{
				ConstructorInversoPropiedadDate(pPropiedad);
			}
			else if (UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Equals("int"))
			{
				ConstructorInversoPropiedadInt(pPropiedad, UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango));
			}
			else if (UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Equals("float"))
			{
				ConstructorInversoPropiedadFloat(pPropiedad, UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango));
			}
			else if (UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Equals("boolean"))
			{
				ConstructorInversoBool(pPropiedad, "bool");
			}
			else
			{
				ConstructorInversoPropiedadDataRelleno(pPropiedad);
			}
		}

		public void ConstructorInversoBool(Propiedad pPropiedad, string rango)
		{
			Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)}= GetBooleanPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\"));");
		}

		public void ConstructorInversoPropiedadDate(Propiedad pPropiedad)
		{
			string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad);
			string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre);
			if (pPropiedad.ValorUnico)
			{
				if (pPropiedad.FunctionalProperty)
				{

					Clase.AppendLine($"var item{mNumItem} = GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\"));");
					Clase.AppendLine($"if(item{mNumItem}.HasValue){{");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)} = item{mNumItem}.Value;");
					Clase.AppendLine("}");
					mNumItem++;
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)} = GetDateValuePropertySemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\"));");
				}
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = new List<DateTime>();");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");

				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}DateTime fecha = new DateTime();");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}DateTime.TryParse(propValue.Value,out fecha);");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(fecha);");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");

				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
			}
		}

		public bool GuardarFechaComoEntero(Propiedad pPropiedad)
		{
			XmlNode elementos = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]");
			if (elementos != null)
			{
				if (elementos.SelectNodes("GuardarFechaComoEntero").Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		public void ConstructorInversoPropiedadInt(Propiedad pPropiedad, string rango)
		{
			string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad);
			string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre);

			if (pPropiedad.ValorUnico)
			{
				if (pPropiedad.FunctionalProperty)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\")).Value;");
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = GetNumberIntPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\"));");
				}
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = new List<{rango}>();");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(GetNumberIntPropertyMultipleValueSemCms(propValue).Value);");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
			}
		}

		public void ConstructorInversoPropiedadFloat(Propiedad pPropiedad, string rango)
		{
			string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad);
			string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre);

			if (pPropiedad.ValorUnico)
			{
				if (pPropiedad.FunctionalProperty)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = GetNumberFloatPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\")).Value;");
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = GetNumberFloatPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\"));");
				}
			}
			else
			{
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = new List<{rango}>();");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues)");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(GetNumberFloatPropertyValueSemCms(propValue.Value).Value);");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
				Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
			}
		}

		private string ObtenerPrefijoPropiedad(Propiedad pPropiedad)
		{
			string pref = UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre, mLoggingService).Trim();
			if (!string.IsNullOrEmpty(pref))
			{
				return UtilCadenas.PrimerCaracterAMayuscula(pref);
			}
			return pref;
		}

		public void ConstructorInversoPropiedadDataRelleno(Propiedad pPropiedad)
		{
			string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad);
			string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre);

			if (GetMultiIdiomaPropiedad(pPropiedad.ElementoOntologia, pPropiedad) && !UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango).Equals("date"))
			{

				if (pPropiedad.ValorUnico)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = new Dictionary<LanguageEnum,string>();");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(idiomaUsuario , GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\")));");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}");
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = new Dictionary<LanguageEnum,List<{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango)}>>();");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
					string tipoPropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango);
					if (tipoPropiedad.ToLower().Equals("geometry"))
					{
						tipoPropiedad = "string";
					}
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}List<string> aux = new List<{tipoPropiedad}>();");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}aux.Add(propValue.Value);");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(idiomaUsuario,aux);");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
				}
			}
			else
			{
				if (pPropiedad.ValorUnico)
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = GetPropertyValueSemCms(pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\"));");
				}
				else
				{
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
					string tipoPropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango);
					if (tipoPropiedad.ToLower().Equals("geometry"))
					{
						tipoPropiedad = "string";
					}
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{prefijoPropiedad}_{nombrePropiedad} = new List<{tipoPropiedad}>();");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues)");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(propValue.Value);");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
					Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
				}
			}
		}
	}
}

