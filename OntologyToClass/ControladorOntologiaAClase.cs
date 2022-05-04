using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.InterfacesOpen;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OntologiaAClase
{
    public class ControladorOntologiaAClase
    {
        byte[] contentXML;
        string nombreOnto;
        bool esPrimaria;
        string nombreOntoOriginal;
        private Ontologia ontologia;
        private Constantes Constante = new Constantes();
        private List<ElementoOntologia> listaElementosPadre = new List<ElementoOntologia>();
        public string nombreCarpeta;
        public StringBuilder Clase { get; }
        public List<string> listaIdiomas;
        public string carpetaPadre;
        public string directorio;
        public List<ObjetoPropiedad> listaObjetosPropiedad;
        public readonly Guid proyID;
        public readonly string nombreCortoProy;
        public List<string> nombresOntologia;
        private List<string> listaPropiedadesSearch;
        private List<string> listaPropiedadesPadreAnidadasSearch;
        private Dictionary<string, string> listaPrefijosOntologias;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private IMassiveOntologyToClass mMassiveOntologyToClass;

        public ControladorOntologiaAClase(OntologiaGenerar onto, string carpetaPadre, string nombreCarpeta, string directorio, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, List<ObjetoPropiedad> listaObjetosPropiedad, Dictionary<string, string> pDicPref, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IMassiveOntologyToClass massiveOntologyToClass)
        {
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            this.Clase = new StringBuilder();
            this.directorio = directorio;
            this.nombreOnto = UtilCadenas.PrimerCaracterAMayuscula(onto.nombreOnto);
            this.nombreOntoOriginal = onto.nombreOnto;
            this.contentXML = onto.contentXML;
            this.esPrimaria = onto.esprimaria;
            this.ontologia = onto.ontologia;
            this.listaIdiomas = onto.listaIdiomas;
            this.nombreCarpeta = nombreCarpeta;
            this.listaObjetosPropiedad = listaObjetosPropiedad;
            this.carpetaPadre = carpetaPadre;
            this.nombresOntologia = pNombresOntologia;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            this.ontologia.LeerOntologia();
            this.listaPropiedadesSearch = ObtenerPropiedadesSearch();
            this.listaPropiedadesPadreAnidadasSearch = ObtenerPropiedadesPadreAnidadasSearch();
            this.listaPrefijosOntologias = pDicPref;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mMassiveOntologyToClass = massiveOntologyToClass;
        }

        /// <summary>
        /// Crea la clases a partir de las entidades de la ontología y las guarda en una carpeta llamada ClasesGeneradas
        /// </summary>
        /// 


        public void CrearClasesDeLaOntologia()
        {
            string[] nombont = this.nombreOnto.Split(' ');
            string pNombreOnto = "";
            int i = 0;

            foreach (string ont in nombont)
            {
                if (i > 0) 
                { 
                    pNombreOnto += UtilCadenas.PrimerCaracterAMayuscula(ont); 
                }
                else 
                { 
                    pNombreOnto += ont; 
                }
                i++;
            }
            string rdfType = this.nombreOntoOriginal;
            pNombreOnto += "Ontology";
            ObtenerEntidadesDeLaClase(ontologia);
            string ruta = Path.Combine(directorio, carpetaPadre, nombreCarpeta, $"{pNombreOnto}");
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            CrearEntidadesPadre(pNombreOnto, rdfType);

            //CrearEntidadesHijosConHerencia(pNombreOnto);
            //CrearEntidadesHijosSinHerencia(pNombreOnto);
        }


        public void CrearEntidadesPadre(string pNombreOnto, string pRdfType)
        {
            foreach (ElementoOntologia pEntidad in listaElementosPadre)
            {
                CrearEntidades(pEntidad, pNombreOnto, pRdfType);
            }
        }

        private List<string> ObtenerPropiedadesSearch()
        {
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            return proyectoCN.ObtenerPropiedadesSearch(proyID);
        }

        private List<string> ObtenerPropiedadesPadreAnidadasSearch()
        {
            List<string> listaPropiedadesPadreAnidadasSearch = new List<string>();
            foreach(string propiedad in listaPropiedadesSearch)
            {
                if (propiedad.Contains("@@@"))
                {
                    listaPropiedadesPadreAnidadasSearch.Add(propiedad);
                }
            }

            return listaPropiedadesPadreAnidadasSearch;
        }

        public void CrearEntidades(ElementoOntologia pEntidad, string pNombreOnto, string pRdfType)
        {
            EntidadAClase entidadAClase = new EntidadAClase(ontologia, pNombreOnto, contentXML, esPrimaria, listaIdiomas, nombreCortoProy, proyID, nombresOntologia, listaObjetosPropiedad, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mMassiveOntologyToClass);
            File.WriteAllText(Path.Combine(directorio, carpetaPadre, nombreCarpeta, $"{pNombreOnto}", UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad) + ".cs"), entidadAClase.GenerarClase(pEntidad, pRdfType, listaPropiedadesSearch, listaPropiedadesPadreAnidadasSearch));
        }

        /// <summary>
        /// Rellenamos las diferentes listas según si son padres , hijos con herencia o hijos sin herencia
        /// </summary>
        /// <param name="pOntology"></pa
        /// ram>
        public void ObtenerEntidadesDeLaClase(Ontologia pOntology)
        {
            foreach (ElementoOntologia entidades in pOntology.Entidades)
            {
                //No quitar nunca esta linea, ya que si la quitas no se cargan los elementoOntologia en cada propiedad de la entidad
                bool esTesauroSemantico = entidades.EsEntidadPathTesSemantico;
                listaElementosPadre.Add(entidades);

            }
        }

        /// <summary>
        /// Generador de la clase GnossOCBase
        /// </summary>
        public void GenerarClaseOCBase()
        {
            string ruta = Path.Combine(directorio, carpetaPadre, nombreCarpeta, "GnossBase");
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            string nombreFichero = Path.Combine(ruta, "GnossOCBase.cs");
            if (!File.Exists(nombreFichero))
            {
                File.WriteAllText(Path.Combine(ruta, "GnossOCBase.cs"), CrearClaseOCBase(this.listaIdiomas));
            }
        }

        /// <summary>
        /// Creación del código de la clase GnossOCBase.cs
        /// </summary>
        /// <returns></returns>
        public string CrearClaseOCBase(List<string> listaIdiomas)
        {
            UsingsClaseOCBase();
            Clase.AppendLine("namespace GnossBase");
            Clase.AppendLine("{");
            EscribirAtributosRDFA();
            EscribirAtributosLabel(listaIdiomas);
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}[ExcludeFromCodeCoverage]");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{Constante.publicClass} GnossOCBase : IGnossOCBase");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public enum LanguageEnum");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            foreach (string idiom in listaIdiomas)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom},");
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal List<OntologyEntity> entList = new List<OntologyEntity>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal List<OntologyProperty> propList = new List<OntologyProperty>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal List<OntologyProperty> imagePropList = new List<OntologyProperty>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal List<string> prefList = new List<string>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal string mGNOSSID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal string mURL;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal Guid resourceID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal Guid articleID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}private static List<string> NoEnIdiomas = new List<string> {{ \"Não\",\"Non\", \"Ez\", \"Nein\", \"No\" }};");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public List<string> tagList = new List<string>();");

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public GnossOCBase()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            foreach (string nmspace in listaPrefijosOntologias.Keys)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}prefList.Add(\"xmlns:{listaPrefijosOntologias[nmspace]}=\\\"{nmspace}\\\"\");");
            }
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.resourceID = Guid.NewGuid();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.articleID = Guid.NewGuid();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public string GNOSSID");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}get{{return mGNOSSID;}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}set");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}this.mGNOSSID = value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}var GnossIDSplit = this.mGNOSSID.Split('_');");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}Guid nuevoResource = Guid.Parse(GnossIDSplit[GnossIDSplit.Count() - 2]);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}Guid nuevoArticle = Guid.Parse(GnossIDSplit.Last());");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!this.ResourceID.Equals(nuevoResource))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.resourceID = nuevoResource;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!this.ArticleID.Equals(nuevoArticle))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.articleID = nuevoArticle;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public string URL");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}get{{return mURL;}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public Guid ResourceID");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}get{{return resourceID;}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}set");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}this.resourceID = value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}string primeraParte = this.mGNOSSID.Substring(0, this.mGNOSSID.LastIndexOf('/') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}string antiguoGuid = this.mGNOSSID.Substring(this.mGNOSSID.LastIndexOf('/') + 1, this.mGNOSSID.LastIndexOf('_'));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}string ultimaParte = this.mGNOSSID.Substring(this.mGNOSSID.LastIndexOf('_') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!antiguoGuid.Equals(this.resourceID.ToString()))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.mGNOSSID = $\"{{primeraParte}}{{this.resourceID.ToString()}}{{ultimaParte}}\";");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public Guid ArticleID");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}get{{return articleID;}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}set");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}this.articleID = value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}string primeraParte = this.mGNOSSID.Substring(0, this.mGNOSSID.LastIndexOf('_') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}string antiguoGuid = this.mGNOSSID.Substring(this.mGNOSSID.LastIndexOf('_') +1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!antiguoGuid.Equals(this.articleID.ToString()))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.mGNOSSID = $\"{{primeraParte}}{{this.articleID.ToString()}}\";");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            ObtenerValorPropiedadSemCMS();
            Clase.AppendLine();
            ObtenerNumberIntPropertyValueSemCMS();
            Clase.AppendLine();
            ObtenerNumberIntMultiplePropertyValueSemCMS();
            Clase.AppendLine();
            ObtenerNumberFloatPropertyValueSemCMS();
            Clase.AppendLine();
            ObtenerDateTimePropertyValueSemCMS();
            Clase.AppendLine();
            ObtenerBoolPropertyValueSemCMS();
            Clase.AppendLine();
            MetodosDeLaClaseOCBase();
            GetPropertyURI();
            GetLabel();
            Clase.AppendLine();
            MetodosAbstractos();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}}}");
            Clase.AppendLine("}");
            return Clase.ToString();
        }

        public void EscribirAtributosLabel(List<string> pListaIdiomas)
        {
            Clase.AppendLine(
    $@"
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class LABELAttribute : Attribute
    {{
        private GnossOCBase.LanguageEnum IdiomaDefecto = GnossOCBase.LanguageEnum.{pListaIdiomas[0]};
        private GnossOCBase.LanguageEnum midioma;
        private string mlabel;
        public LABELAttribute(GnossOCBase.LanguageEnum idioma, string label)
        {{
            mlabel = label;
            midioma = idioma;
        }}
        public string LABEL(GnossOCBase.LanguageEnum pLang)
        {{
            if (midioma.Equals(pLang))
            {{
                return mlabel;
            }}
            return """";
        }}
    }}");
        }
        public void EscribirAtributosRDFA()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}public class RDFPropertyAttribute : Attribute");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{{");
            // The constructor is called when the attribute is set.
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public RDFPropertyAttribute(string pRDFA)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}mRDFA = pRDFA;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            // Keep a variable internally ...
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}protected string mRDFA;");
            // .. and show a copy to the outside world.
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public string RDFProperty");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}get {{ return mRDFA; }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}}}");
        }

        public void GetLabel()
        {
            Clase.AppendLine(
$@"
        public string GetLabel(string nombrePropiedad, LanguageEnum pLang)
        {{
            Type type = this.GetType();
            PropertyInfo mInfo = type.GetProperty(nombrePropiedad);

            if (mInfo != null)
            {{
                Attribute[] attr = Attribute.GetCustomAttributes(mInfo, typeof(LABELAttribute));
                {{
                    foreach (Attribute atributo in attr)
                    {{
                        if (atributo != null)
                        {{
                            if (!((LABELAttribute)atributo).LABEL(pLang).Equals(""""))
                            {{
                                return ((LABELAttribute)atributo).LABEL(pLang);
                            }}
                        }}
                    }}
                }}
            }}

            return """";
        }}
");
        }
        public void MetodosAbstractos()
        {
            Clase.AppendLine(
                $@"
        public virtual List<string> ToOntologyGnossTriples(ResourceApi pResourceApi){{throw new NotImplementedException();}}

        public virtual List<string> ToSearchGraphTriples(ResourceApi pResourceApi){{throw new NotImplementedException();}}

        public virtual KeyValuePair<Guid, string> ToAcidData(ResourceApi resourceAPI){{throw new NotImplementedException();}}

        public virtual string GetURI(ResourceApi resourceAPI){{throw new NotImplementedException();}}
");

            Clase.AppendLine("public int GetID() { return 0; }");

        }

        public void GetPropertyURI()
        {
            Clase.AppendLine(
$@"
        public string GetPropertyURI(string nombrePropiedad)
        {{
            Type type = this.GetType();
            PropertyInfo mInfo = type.GetProperty(nombrePropiedad);
            if (mInfo != null)
            {{
                Attribute attr = Attribute.GetCustomAttribute(mInfo, typeof(RDFPropertyAttribute));
                if (attr != null)
                {{
                    return ((RDFPropertyAttribute)attr).RDFProperty;
                }}
            }}
            return """";
        }}
    
   ");
        }
        public void MetodosDeLaClaseOCBase()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal virtual void GetProperties()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal virtual void GetEntities()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal virtual void AddImages(ComplexOntologyResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal virtual void AddImages(SecondaryResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal virtual void AddFiles(ComplexOntologyResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal virtual void AddFiles(SecondaryResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal string GetExtension(string file)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return file.Substring(file.LastIndexOf('.'));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
        }


        public void ObtenerValorPropiedadSemCMS()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static string GetPropertyValueSemCms(SemanticPropertyModel pProperty)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (pProperty != null && pProperty.PropertyValues.Count > 0)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}return pProperty.PropertyValues[0].Value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return \"\";");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        public void ObtenerNumberIntPropertyValueSemCMS()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static int? GetNumberIntPropertyValueSemCms(SemanticPropertyModel pProperty)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(pProperty != null && pProperty.PropertyValues.Count > 0 && !string.IsNullOrEmpty(pProperty.PropertyValues[0].Value))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}return int.Parse(pProperty.PropertyValues[0].Value);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return 0;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }
        public void ObtenerNumberIntMultiplePropertyValueSemCMS()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static int? GetNumberIntPropertyMultipleValueSemCms(SemanticPropertyModel.PropertyValue pProperty)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(pProperty != null && !string.IsNullOrEmpty(pProperty.Value))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}return int.Parse(pProperty.Value);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return 0;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }
        public void ObtenerNumberFloatPropertyValueSemCMS()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static float? GetNumberFloatPropertyValueSemCms(SemanticPropertyModel pProperty)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(pProperty != null && pProperty.PropertyValues.Count > 0 && !string.IsNullOrEmpty(pProperty.PropertyValues[0].Value))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}return float.Parse(pProperty.PropertyValues[0].Value.Replace('.',','));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return 0;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }
        public void ObtenerBoolPropertyValueSemCMS()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static bool GetBooleanPropertyValueSemCms(SemanticPropertyModel pProperty)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}bool resultado = false;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(pProperty != null && pProperty.PropertyValues.Count > 0 && !string.IsNullOrEmpty(pProperty.PropertyValues[0].Value))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if (!bool.TryParse(pProperty.PropertyValues[0].Value, out resultado))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}resultado = !NoEnIdiomas.Contains(pProperty.PropertyValues[0].Value);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return resultado;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        public void ObtenerDateTimePropertyValueSemCMS()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static DateTime? GetDateValuePropertySemCms(SemanticPropertyModel pProperty)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}string stringDate = GetPropertyValueSemCms(pProperty);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}if (!string.IsNullOrEmpty(stringDate))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}int year = 0;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}int month = 0;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}int day = 0;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}if (stringDate.Contains('/'))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}day = int.Parse(stringDate.Split('/')[0]);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}month = int.Parse(stringDate.Split('/')[1]);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}year = int.Parse(stringDate.Split('/')[2].Split(' ')[0]);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}else");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}year = int.Parse(stringDate.Substring(0, 4));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}month = int.Parse(stringDate.Substring(4, 2));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}day = int.Parse(stringDate.Substring(6, 2));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}if (stringDate.Length == 14)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}if (month == 0 || day == 0)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}return new DateTime(year);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}else");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}int hour = int.Parse(stringDate.Substring(8, 2));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}int minute = int.Parse(stringDate.Substring(10, 2));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}int second = int.Parse(stringDate.Substring(12, 2));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}return new DateTime(year, month, day, hour, minute, second);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}else");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}return new DateTime(year, month, day);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}return null;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }
        /// <summary>
        /// Obtenemos los prefijos a usar en la clase base
        /// </summary>
        public void UsingsClaseOCBase()
        {
            Clase.AppendLine("using System;");
            Clase.AppendLine("using System.Collections.Generic;");
            Clase.AppendLine("using System.Linq;");
            Clase.AppendLine("using System.Text;");
            Clase.AppendLine("using System.Threading.Tasks;");
            Clase.AppendLine("using Gnoss.ApiWrapper;");
            Clase.AppendLine("using Gnoss.ApiWrapper.Model;");
            Clase.AppendLine("using Es.Riam.Gnoss.Web.MVC.Models;");
            Clase.AppendLine("using Gnoss.ApiWrapper.Interfaces;");
            Clase.AppendLine("using System.Diagnostics.CodeAnalysis;");
            Clase.AppendLine("using System.Reflection;");

            Clase.AppendLine();
        }
    }
}