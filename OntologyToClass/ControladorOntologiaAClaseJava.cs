using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OntologiaAClase
{
    public class ControladorOntologiaAClaseJava
    {
        byte[] contentXML;
        string nombreOnto;
        bool esPrimaria;
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
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private string nombreOntoOriginal;
        private Dictionary<string, string> listaPrefijosOntologias;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public ControladorOntologiaAClaseJava(OntologiaGenerar pOnto, string pCarpetaPadre, string pNombreCarpeta, string pDirectorio, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, List<ObjetoPropiedad> pListaObjetosPropiedad, Dictionary<string, string> pListaPrefijosOntologias, EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorOntologiaAClaseJava> logger, ILoggerFactory loggerFactory)
        {
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.Clase = new StringBuilder();
            this.directorio = pDirectorio;
            this.nombreOnto = pOnto.nombreOnto;
            this.contentXML = pOnto.contentXML;
            this.esPrimaria = pOnto.esPrincipal;
            this.ontologia = pOnto.ontologia;
            this.listaIdiomas = pOnto.listaIdiomas;
            this.nombreCarpeta = pNombreCarpeta;
            this.listaObjetosPropiedad = pListaObjetosPropiedad;
            this.carpetaPadre = pCarpetaPadre;
            this.nombresOntologia = pNombresOntologia;
            this.nombreOntoOriginal = pOnto.nombreOnto;
            this.ontologia.LeerOntologia();
            this.listaPropiedadesSearch = ObtenerPropiedadesSearch();
            this.listaPropiedadesPadreAnidadasSearch = ObtenerPropiedadesPadreAnidadasSearch();
            this.listaPrefijosOntologias = pListaPrefijosOntologias;
            mEntityContext = pEntityContext;
            mLoggingService = pLoggingService;
            mConfigService = pConfigService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Crea la clases a partir de las entidades de la ontología y las guarda en una carpeta llamada ClasesGeneradas
        /// </summary>
        /// 
        public void CrearClasesDeLaOntologia()
        {
            string[] nombont = this.nombreOnto.Split(' ');
            string nombreOnto = "";
            int i = 0;

            foreach (string ont in nombont)
            {
                if (i > 0) { nombreOnto += UtilCadenas.PrimerCaracterAMayuscula(ont); }
                else { nombreOnto += ont; }
                i++;
            }
            string rdfType = this.nombreOntoOriginal;
            nombreOnto += "Ontology";
            ObtenerEntidadesDeLaClase(ontologia);
            string ruta = Path.Combine(directorio, carpetaPadre, nombreCarpeta, $"{UtilCadenas.PrimerCaracterAMinuscula(nombreOnto)}");
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            CrearEntidadesPadre(nombreOnto, rdfType);
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
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            return proyectoCN.ObtenerPropiedadesSearch(proyID);
        }

        private List<string> ObtenerPropiedadesPadreAnidadasSearch()
        {
            List<string> listaPropiedadesPadreAnidadasSearch = new List<string>();
            foreach (string propiedad in listaPropiedadesSearch)
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
            EntidadAClaseJava entidadAClaseJava = new EntidadAClaseJava(ontologia, pNombreOnto, contentXML, esPrimaria, listaIdiomas, nombreCortoProy, proyID, nombresOntologia, listaObjetosPropiedad, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<EntidadAClaseJava>(), mLoggerFactory);
            GenerarEnumJava enumJava = new GenerarEnumJava(contentXML, listaIdiomas, nombreCortoProy, proyID, nombresOntologia, listaObjetosPropiedad);
            File.WriteAllText(Path.Combine(directorio, carpetaPadre, nombreCarpeta, pNombreOnto, UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad) + ".java"), entidadAClaseJava.GenerarClaseJava(pEntidad, pNombreOnto, pRdfType, listaPropiedadesSearch, listaPropiedadesPadreAnidadasSearch));
            File.WriteAllText(Path.Combine(directorio, carpetaPadre, nombreCarpeta, pNombreOnto, "LanguageEnum" + ".java"), enumJava.CrearEnum(listaIdiomas, pNombreOnto));
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
            string nombreFichero = Path.Combine(ruta, "GnossOCBase.java");
            if (!File.Exists(nombreFichero))
            {
                File.WriteAllText(Path.Combine(ruta, "GnossOCBase.java"), CrearClaseOCBase());
            }
        }

        /// <summary>
        /// Creación del código de la clase GnossOCBase.cs
        /// </summary>
        /// <returns></returns>
        /// 
        public string CrearEnum2(List<string> pListaIdiomas)
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} enum LanguageEnum");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            foreach (string idiom in pListaIdiomas)
            {
                if (pListaIdiomas.Count == 1)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom}");
                }
                else
                {
                    if (pListaIdiomas.LastIndexOf(idiom) + 1 == pListaIdiomas.Count)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom}");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{idiom},");
                    }
                }

            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            return Clase.ToString();
        }

        public string CrearClaseOCBase()
        {
            ImportsClaseOCBase();
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{Constante.publicClass} GnossOCBase implements IGnossOCBase");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public ArrayList<OntologyEntity> entList = new ArrayList<OntologyEntity>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public ArrayList<OntologyProperty> propList = new ArrayList<OntologyProperty>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public static final HashMap<String, ArrayList<MetaKeyword>> listaMetakeyword = new HashMap<String, ArrayList<MetaKeyword>>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public ArrayList<OntologyProperty> imagePropList = new ArrayList<OntologyProperty>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public ArrayList<String> prefList = new ArrayList<String>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public String mGNOSSID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public String mURL;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public UUID resourceID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public UUID articleID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} private static ArrayList<String> NoEnIdiomas = new ArrayList<String>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public ArrayList<String> tagList = new ArrayList<String>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public GnossOCBase()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} NoEnIdiomas.add(\"Não\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} NoEnIdiomas.add(\"Non\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} NoEnIdiomas.add(\"Ez\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} NoEnIdiomas.add(\"Nein\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} NoEnIdiomas.add(\"No\");");
            foreach (string nmspace in listaPrefijosOntologias.Keys)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}prefList.add(\"xmlns:{listaPrefijosOntologias[nmspace]}=\\\"{nmspace}\\\"\");");
            }
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.resourceID = UUID.randomUUID();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.articleID = UUID.randomUUID();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public String getGNOSSID()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return this.mGNOSSID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void setGNOSSID(String value)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.mGNOSSID = value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}int barraBaja = this.mGNOSSID.lastIndexOf('_');");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}int primeraBarraBaja = this.mGNOSSID.indexOf('_');");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}try{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}UUID nuevoResource = UUID.fromString(this.mGNOSSID.substring(primeraBarraBaja + 1, barraBaja));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}UUID nuevoArticle = UUID.fromString(this.mGNOSSID.substring(barraBaja + 1));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!this.getResourceID().equals(nuevoResource))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.resourceID = nuevoResource;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(!this.getArticleID().equals(nuevoArticle))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}this.articleID = nuevoArticle;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}catch(IllegalArgumentException|StringIndexOutOfBoundsException ex) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public String getURL()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return mURL;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public UUID getResourceID()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return resourceID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void setResourceID(UUID value)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.resourceID = value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String primeraParte = this.mGNOSSID.substring(0, this.mGNOSSID.indexOf('_') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String antiguoGuid = this.mGNOSSID.substring(this.mGNOSSID.indexOf('_') + 1, this.mGNOSSID.lastIndexOf('_'));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String ultimaParte = this.mGNOSSID.substring(this.mGNOSSID.lastIndexOf('_') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(!antiguoGuid.equals(this.resourceID.toString()))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}this.mGNOSSID = primeraParte +this.resourceID.toString()+ ultimaParte;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public UUID getArticleID()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return articleID;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void setArticleID(UUID value)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.articleID = value;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String primeraParte = this.mGNOSSID.substring(0, this.mGNOSSID.lastIndexOf('_') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String antiguoGuid = this.mGNOSSID.substring(this.mGNOSSID.lastIndexOf('_') + 1);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(!antiguoGuid.equals(this.articleID.toString()))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}this.mGNOSSID = primeraParte + this.articleID.toString();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine();
            MetodosDeLaClaseOCBase();
            MetodosAbstractos();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}}}");
            return Clase.ToString();
        }

        public void ImportsClaseOCBase()
        {
            Clase.AppendLine("package GnossBase;");
            Clase.AppendLine("import java.util.ArrayList;");
            Clase.AppendLine("import java.util.Calendar;");
            Clase.AppendLine("import java.util.HashMap;");
            Clase.AppendLine("import java.util.UUID;");
            Clase.AppendLine("import java.io.File;");
            Clase.AppendLine("import java.io.FileWriter;");
            Clase.AppendLine("import java.io.IOException;");
            Clase.AppendLine("import java.nio.CharBuffer;");
            Clase.AppendLine("import java.util.Date;");
            Clase.AppendLine("import java.text.SimpleDateFormat;");
            Clase.AppendLine();
            Clase.AppendLine("import org.gnoss.apiWrapper.Excepciones.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.Interfaces.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.Main.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.models.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.Helpers.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.ApiModel.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.Utilities.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.Utils.*;");
            Clase.AppendLine("import org.gnoss.apiWrapper.Web.*;");
            Clase.AppendLine();
            Clase.AppendLine("import com.microsoft.applicationinsights.web.dependencies.apachecommons.lang3.NotImplementedException;");


            Clase.AppendLine();
        }


        public void MetodosDeLaClaseOCBase()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void getProperties()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void getEntities()  ");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddImages(ComplexOntologyResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddImages(SecondaryResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddFiles(ComplexOntologyResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddFiles(SecondaryResource pResource)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public String getExtension(String file)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return file.substring(file.lastIndexOf('.'));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
        }

        public void MetodosAbstractos()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public ArrayList<String> ToOntologyGnossTriples(ResourceApi pResourceApi) {{ throw new NotImplementedException(mGNOSSID); }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public ArrayList<String> ToSearchGraphTriples(ResourceApi pResourceApi) {{ throw new NotImplementedException(mGNOSSID); }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public HashMap<UUID, String> ToAcidData(ResourceApi resourceAPI) {{ throw new NotImplementedException(mGNOSSID); }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public String GetURI(ResourceApi resourceAPI) {{ throw new NotImplementedException(mGNOSSID); }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public int GetID() {{ return 0; }}");
        }
    }
}