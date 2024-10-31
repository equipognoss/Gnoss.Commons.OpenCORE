using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases.Enumeraciones;
using Es.Riam.Semantica.OWL;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace OntologiaAClase
{
    class EntidadAClaseJava
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
        public StringBuilder Clase { get; }

        private Constantes c = new Constantes();
        private ConstantesJava constJava = new ConstantesJava();
        private XmlDocument doc;
        private List<string> listaObjetosExternos;
        private string nombreOnto;
        private byte[] contentXML;
        private Ontologia ontologia;
        private string nombrePropTitulo;

        private string nombrePropTituloEntero;
        private string nombrePropDescripcionEntera;
        private string nombrePropDescripcion;
        private bool esPrimaria;
        private bool? mEsMultiIdiomaConfig;
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
        public readonly Guid proyID;
        public readonly string nombreCortoProy;
        public Dictionary<string, string> listasInicializar = new Dictionary<string, string>();
        private List<FacetaObjetoConocimientoProyecto> listaFacetaObjetoConocimientoProyecto;
        private LoggingService mLoggingService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public EntidadAClaseJava(LoggingService loggingService)
        {
            mLoggingService = loggingService;
            Clase = new StringBuilder();
        }

        /// <summary>
        /// Constructor de la clase que necesita uan ontologia, su nombre, y el xml , además del string builder
        /// </summary>
        /// <param name="pOntologia"></param>
        /// <param name="pNombreOnto"></param>
        /// <param name="pContentXML"></param>
        public EntidadAClaseJava(Ontologia pOntologia, string pNombreOnto, byte[] pContentXML, bool pEsPrimaria, List<string> pListaIdiomas, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, List<ObjetoPropiedad> pListaObjetosPropiedad, EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = pLoggingService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            FacetaCN facetaCN = new FacetaCN(pEntityContext, pLoggingService, pConfigService, mServicesUtilVirtuosoAndReplication);
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            Clase = new StringBuilder();
            this.ontologia = pOntologia;
            this.nombreOnto = pNombreOnto;
            this.contentXML = pContentXML;
            this.listaObjetosPropiedad = pListaObjetosPropiedad;
            this.esPrimaria = pEsPrimaria;
            this.dicPref = this.ontologia.NamespacesDefinidos;
            this.listaIdiomas = pListaIdiomas;
            this.nombresOntologia = pNombresOntologia;
            this.doc = new XmlDocument();
            if (pContentXML != null)
            {
                MemoryStream ms = new MemoryStream(pContentXML);
                doc.Load(ms);
            }
            listaObjetosExternos = new List<string>();
            
            listaFacetaObjetoConocimientoProyecto = facetaCN.ObtenerFacetasObjetoConocimientoProyectoDeOntologia(pNombreOnto.Replace("Ontology", ""), proyID);
        }

        /// <summary>
        /// Creamos la clase a partir de la entidad 
        /// </summary>
        /// <param name="pEntidad"></param>
        /// <returns></returns>
        public string GenerarClaseJava(ElementoOntologia pEntidad, string pNombreOnto, string pRdfType, List<string> pListaPropiedadesSearch, List<string> pListaPadrePropiedadesAnidadas)
        {
            GenerarUsings(pEntidad, pNombreOnto);
            EscribirHerencias(pEntidad);
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{{");
            if (pEntidad.Superclases == null || pEntidad.Superclases.Count == 0 || pEntidad.Superclases[0].Equals("http://www.w3.org/2002/07/owl#Thing"))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}SimpleDateFormat sdf1 = new SimpleDateFormat(\"yyyyMMddHHmmss\");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}String date1=\"00010101000000\";");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}Date fechaMin1 = null;");
            }
            Clase.AppendLine();
            Clase.AppendLine();
            CargarRangoPropiedades(pEntidad);
            CreacionDePropiedades(pEntidad);
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombrePropJava(pEntidad.TipoEntidad)}() {{ ");
            ConstructorList(listasInicializar);
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
            ObtenerPropiedades(pEntidad);
            Clase.AppendLine();
            ObtenerEntidades();
            if (esPrimaria)
            {
                ObtenerTituloDescripcion();
            }
            Clase.AppendLine();
            CrearToRecurso(esPrimaria, pEntidad);
            Clase.AppendLine();
            CrearToOntologyGraphTriples(pEntidad);
            Clase.AppendLine();
            CrearToSearchGraphTriples(esPrimaria, pEntidad, pRdfType, pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas, listaFacetaObjetoConocimientoProyecto);
            Clase.AppendLine();
            PintarObtenerValorPropiedadMetodo();
            Clase.AppendLine();
            PintarObtenerStringDePropiedad();
            Clase.AppendLine();
            PintarObtenerPropiedadReflection();
            Clase.AppendLine();
            CrearToAcidData(esPrimaria, pEntidad);
            Clase.AppendLine();
            CrearAgregarTag(pEntidad, listaFacetaObjetoConocimientoProyecto);
            Clase.AppendLine();
            CrearGetURI(pEntidad);
            Clase.AppendLine();
            CrearTextoSinSaltoDeLinea();
            Clase.AppendLine();
            AgregarTituloRecurso(pEntidad);
            Clase.AppendLine();
            if (esPrimaria)
            {
                AgregarDescripcionRecurso(pEntidad);
            }
            Clase.AppendLine();
            AnadirImagenes(pEntidad);
            Clase.AppendLine();
            AnadirArchivos(pEntidad);
            ToCalendar();
            EscribirMetodoFecha();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}}}");
            return Clase.ToString();
        }

        private void CargarRangoPropiedades(ElementoOntologia pEntidad)
        {
            foreach (Propiedad pPropiedad in pEntidad.Propiedades)
            {
                if (string.IsNullOrEmpty(pPropiedad.Rango))
                {
                    pPropiedad.Rango = ObtenerRangoDePropiedad(pPropiedad);
                }
            }
        }

        public string CrearEnum(List<string> pListaIdiomas)
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)} enum LanguageEnum");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{{");
            foreach (string idiom in pListaIdiomas)
            {
                if (pListaIdiomas.Count == 1)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{idiom}");
                }
                else
                {
                    if (pListaIdiomas.LastIndexOf(idiom) + 1 == pListaIdiomas.Count)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{idiom}");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{idiom},");
                    }
                }
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}}}");
            Clase.AppendLine();
            return Clase.ToString();
        }

        public void ConstructorList(Dictionary<string, string> pListaIniciar)
        {
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} super(); ");
            Clase.AppendLine();
            foreach (KeyValuePair<string, string> s in pListaIniciar)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} {s.Key}= new {s.Value}();");
            }
            Clase.AppendLine();
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
                else
                {
                    return null;
                }
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
                XmlNodeList listaEspef = null;
                if (doc.DocumentElement != null)
                {
                    listaEspef = doc.DocumentElement.SelectNodes("EspefEntidad");
                    if (listaEspef != null)
                    {
                        if (listaEspef.Count > 1)
                        {
                            foreach (XmlNode espef in listaEspef)
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
                            foreach (XmlNode espef in listaEspef)
                            {
                                if (espef.Attributes.Count < 1)
                                {
                                    return espef;
                                }
                            }
                        }
                    }
                }
                if (listaEspef != null)
                {
                    return listaEspef.Item(0);
                }
                else 
                {
                    return null; 
                }
            }
        }

        /// <summary>
        /// Diferenciamos clases con herencia
        /// </summary>
        /// <param name="pEntidad"></param>
        public void EscribirHerencias(ElementoOntologia pEntidad)
        {
            if (ontologia.EntidadesAuxiliares.Contains(pEntidad) || pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
            {
                EstablecerClaseBaseSiTieneHerencia(pEntidad);
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{c.publicClass} {ObtenerEntidad(pEntidad)} extends GnossOCBase");
            }
        }

        /// <summary>
        /// Obtenemos la entidad
        /// </summary>
        /// <param name="pEntidad"></param>
        /// <returns></returns>
        public string ObtenerEntidad(ElementoOntologia pEntidad)
        {
            return UtilCadenasOntology.ObtenerNombrePropJava(pEntidad.TipoEntidad);
        }

        /// <summary>
        /// Si tiene herencia.
        /// </summary>
        /// <param name="pEntidad"></param>
        public void EstablecerClaseBaseSiTieneHerencia(ElementoOntologia pEntidad)
        {
            if (pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{c.publicClass} {ObtenerEntidad(pEntidad)} extends {UtilCadenasOntology.ObtenerNombrePropJava(pEntidad.Superclases[0])}");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{c.publicClass} {ObtenerEntidad(pEntidad)} extends GnossOCBase");
            }
        }

        /// <summary>
        /// Añadimos al principio los usings que necesitamos 
        /// </summary>
        private void GenerarUsings(ElementoOntologia pEntidad, string pNombreOnto)
        {
            Clase.AppendLine($"package {UtilCadenas.PrimerCaracterAMinuscula(pNombreOnto)};");
            Clase.AppendLine("import java.io.IOException;");
            Clase.AppendLine("import java.util.ArrayList;");
            Clase.AppendLine("import java.util.Calendar;");
            Clase.AppendLine("import java.util.HashMap;");
            Clase.AppendLine("import java.util.UUID;");
            Clase.AppendLine("import java.io.File;");
            Clase.AppendLine("import java.io.FileWriter;");
            Clase.AppendLine("import java.util.regex.Matcher;");
            Clase.AppendLine("import java.util.regex.Pattern;");
            Clase.AppendLine("import java.lang.reflect.Method;");
            Clase.AppendLine("import java.lang.reflect.InvocationTargetException;");
            Clase.AppendLine("import java.nio.CharBuffer;");
            Clase.AppendLine("import java.util.Date;");
            Clase.AppendLine("import java.util.Calendar;");
            Clase.AppendLine("import java.text.SimpleDateFormat;");
            Clase.AppendLine("import java.util.GregorianCalendar;");
            Clase.AppendLine("import java.text.ParseException;");
            Clase.AppendLine("import GnossBase.GnossOCBase;");
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

            List<string> listaImportsExternos = ObtenerImportsExternos(pEntidad);
            foreach (string import in listaImportsExternos)
            {
                int indiceInicio = import.IndexOf(" ") + 2;
                string valorImport = UtilCadenas.PrimerCaracterAMinuscula(import.Substring(indiceInicio, import.Length - indiceInicio));
                Clase.AppendLine($"import {valorImport}");
            }

            Clase.AppendLine();
        }

        public List<string> ObtenerImportsExternos(ElementoOntologia pEntidad)
        {
            List<string> listaImports = new List<string>();
            foreach (Propiedad prop in pEntidad.Propiedades)
            {
                List<string> importsExternosPropiedad = ObtenerImportsExternosPropiedad(prop);
                if (importsExternosPropiedad.Count > 0)
                {
                    foreach (string import in importsExternosPropiedad)
                    {
                        if (!string.IsNullOrEmpty(import) && !listaImports.Contains(import) && !nombreOnto.Equals(import))
                        {
                            listaImports.Add(import);
                        }
                    }
                }
            }
            return listaImports;
        }

        /// <summary>
        /// obtener los using de las clases externas para poder utilizarlas
        /// </summary>
        /// <param name="pPropiedad"></param>
        /// <returns></returns>
        public List<string> ObtenerImportsExternosPropiedad(Propiedad pPropiedad)
        {
            List<string> imports = new List<string>();
            string graf = "";

            if (EspefEntidad != null)
            {
                XmlNode nodoGrafo = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/Grafo");

                if (nodoGrafo != null)
                {
                    string nombreGrafoCorrecto = nombresOntologia.FirstOrDefault(n => n.Equals(nodoGrafo.InnerText.Replace(".owl", ""), StringComparison.InvariantCultureIgnoreCase));
                    if (!string.IsNullOrEmpty(nombreGrafoCorrecto))
                    {
                        graf = UtilCadenas.PrimerCaracterAMayuscula(nombreGrafoCorrecto).Split('.')[0] + "Ontology";
                        imports.Add($"import  {graf}.{ObtenerRangoDePropiedad(pPropiedad)};");
                    }
                }
            }

            return imports;
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
                            clase = UtilCadenasOntology.ObtenerNombrePropJava(node.InnerText);
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
                return "Object";
            }
        }

        public string CompletarTipoSeleccion(Propiedad pPropiedad)
        {
            string clase = "Object";

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
        /// Metodo interno de cada clase para obtener las propiedades
        /// </summary>
        /// <param name="ontologia"></param>
        /// <param name="pEntidad"></param>
        public void ObtenerPropiedades(ElementoOntologia pEntidad)
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} public void getProperties()");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}super.getProperties();");
            ObtenerPropiedadesPadre(pEntidad);
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
        }

        /// <summary>
        /// Creamos el metodo obtenerPropiedades para cuando tenga multiidioma
        /// </summary>
        /// <param name="pEntidad"></param>
        public void ObtenerPropiedadesPadre(ElementoOntologia pEntidad)
        {
            foreach (KeyValuePair<string, PropiedadTipo> propiedad in propListiedadesTipo)
            {
                string prefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Key));
                string nombrePropiedad = UtilCadenasOntology.ObtenerNombrePropJava(propiedad.Key);
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
                        if (listaObjetosExternos.Contains(propiedad.Key))
                        {
                            id = "Id";
                        }
                        break;
                    case PropiedadTipo.StringMultiple:
                        tipoPropiedad = "ListStringOntologyProperty";
                        if (listaObjetosExternos.Contains(propiedad.Key))
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
                        aux = "";
                        break;
                    case PropiedadTipo.DateNull:
                        tipoPropiedad = "DateOntologyProperty";
                        aux = "";
                        nulable = true;
                        break;
                    case PropiedadTipo.IntMultiple:
                        thi = "";
                        tipoPropiedad = "ListStringOntologyProperty";
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<String> {prefijoPropiedad}_{nombrePropiedad}String = new ArrayList<String>();");
                        Clase.AppendLine($"String[] miarray = new String [{prefijoPropiedad}_{nombrePropiedad}.lenght()];");
                        Clase.AppendLine($"miarray=this.{prefijoPropiedad}_{nombrePropiedad}.split(\"\");");
                        Clase.AppendLine($"for(String s : miArray){{");
                        Clase.AppendLine($" {UtilCadenasOntology.Tabs(3)}{prefijoPropiedad}_{nombrePropiedad}String.add(s);");
                        Clase.AppendLine($"}}");
                        aux = "String";
                        break;
                }
                if (propiedad.Value == PropiedadTipo.DateMultiple)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for (Calendar fecha : {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}){{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + n)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", fecha));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");

                }
                else
                {
                    if (EsPropiedadMultiIdioma(propiedad.Key))
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for (LanguageEnum LanguageEnum : LanguageEnum.values())");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}[LanguageEnum], LanguageEnum.values().toString()));");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    }
                    else if (!EsImagen(pEntidad, propiedad.Key))
                    {
                        if (tipoPropiedad.Equals("DateOntologyProperty"))
                        {

                            if (nulable)
                            {
                                n = 1;
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if ({thi}{id}{prefijoPropiedad}_{nombrePropiedad}!=null){{");
                            }

                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3 + n)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                            if (nulable) { Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}"); }
                        }
                        else
                        {
                            if (nulable)
                            {
                                n = 1;
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if ({thi}{id}{prefijoPropiedad}_{nombrePropiedad}!=null){{");
                            }
                            if (tipoPropiedad.Equals("StringOntologyProperty"))
                            {
                                if (propiedad.Value.Equals(PropiedadTipo.IntSimple))
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", String.valueOf({thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux})));");
                                }
                                else
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                                }
                            }
                            else if (tipoPropiedad.Equals("ListStringOntologyProperty"))
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                            }
                            else
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3 + n)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                            }
                            if (nulable) { Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}"); }
                        }
                    }
                    else if (!EsArchivo(pEntidad, propiedad.Key))
                    {
                        if (tipoPropiedad.Equals("DateOntologyProperty"))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                        }
                        else if (tipoPropiedad.Equals("StringOntologyProperty"))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", String.valueOf({thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux})));");
                        }
                        else if (tipoPropiedad.Equals("ListStringOntologyProperty"))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}propList.add(new { tipoPropiedad }(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}));");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtenemos un string de la forma prefijo:propiedad
        /// </summary>
        /// <param name="pDiccionarioPrefijos"></param>
        /// <param name="pRango"></param>
        /// <returns></returns>
        public string ObtenerPrefijoYPropiedad(Dictionary<string, string> pDiccionarioPrefijos, string pRango)
        {
            return UtilCadenasOntology.ObtenerPrefijo(pDiccionarioPrefijos, pRango) + ":" + UtilCadenasOntology.ObtenerNombrePropSinNamespaceJava(pRango);
        }

        /// <summary>
        /// Nos indica si la propiedad es multi idioma
        /// </summary>
        /// <param name="pPropiedad"></param>
        /// <returns></returns>
        public bool EsPropiedadMultiIdioma(string pPropiedad)
        {
            return propListiedadesMultidioma.ContainsKey(pPropiedad) && propListiedadesMultidioma[pPropiedad];
        }

        /// <summary>
        /// Nos indica si la propiedad es una imagen
        /// </summary>
        /// <param name="pEntidad"></param>
        /// <param name="pPropiedad"></param>
        /// <returns></returns>
        public bool EsImagen(ElementoOntologia pEntidad, string pPropiedad)
        {
            List<Propiedad> propListImagen = ObtenerPropiedadesImagen(pEntidad);
            return propListImagen.Any(prop2 => prop2.Nombre.Equals(pPropiedad));
        }

        /// <summary>
        /// Nos indica si la propiedad es un Archivo
        /// </summary>
        /// <param name="pEntidad"></param>
        /// <param name="pPropiedad"></param>
        /// <returns></returns>
        public bool EsArchivo(ElementoOntologia pEntidad, string pPropiedad)
        {
            List<Propiedad> propListArchivo = ObtenerPropiedadesArchivo(pEntidad);
            return propListArchivo.Any(prop2 => prop2.Nombre.Equals(pPropiedad));
        }

        /// <summary>
        /// Obitene las propiedades de una entidad
        /// </summary>
        /// <param name="pEntidad">Entidad de la que se quieren obtener las propiedades</param>
        /// <returns></returns>
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

        /// <summary>
        /// Obtiene las propiedades de tipo imagen de la entidad
        /// </summary>
        /// <param name="pEntidad">Entidad de la que se quiere obtener las imágenes</param>
        /// <returns></returns>
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

        /// <summary>
        /// Creamos el metodo ObtenerEntidades dentro de la clase . Si es hijo sobreescribirá el metodo, obteniendo los elementos del pPadre
        /// </summary>        
        public void ObtenerEntidades()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void getEntities() ");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            ObtenerEntidadesPadre();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}} ");
        }

        /// <summary>
        /// Obtenermos las entidades del Padre para usar en el ToRecurso
        /// </summary>
        public void ObtenerEntidadesPadre()
        {
            foreach (Propiedad prop in listentidadesAux)
            {
                string rango = UtilCadenasOntology.ObtenerNombrePropJava(prop.Rango);
                if (!rango.Equals("object"))
                {
                    string obtenerPrefijo = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre));
                    string nombreProp = UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre);
                    if (prop.ValorUnico)
                    {
                        if (prop.CardinalidadMaxima == 1)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({obtenerPrefijo}_{nombreProp} != null)");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{obtenerPrefijo}_{nombreProp}.getProperties();");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{obtenerPrefijo}_{nombreProp}.getEntities();");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}OntologyEntity entity{obtenerPrefijo}_{nombreProp};");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}try{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}entity{obtenerPrefijo}_{nombreProp} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", {obtenerPrefijo}_{nombreProp}.propList, {obtenerPrefijo}_{nombreProp}.entList);");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}entList.add(entity{obtenerPrefijo}_{nombreProp});");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}} catch (GnossAPIArgumentException ex) {{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}ex.printStackTrace();");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{obtenerPrefijo}_{nombreProp}.getProperties();");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{obtenerPrefijo}_{nombreProp}.getEntities();");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}OntologyEntity entity{obtenerPrefijo}_{nombreProp};");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}try{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}entity{obtenerPrefijo}_{nombreProp} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", {obtenerPrefijo}_{nombreProp}.propList, {obtenerPrefijo}_{nombreProp}.entList);");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}entList.add(entity{obtenerPrefijo}_{nombreProp});");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}} catch (GnossAPIArgumentException ex) {{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ex.printStackTrace();");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                        }
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({obtenerPrefijo}_{nombreProp}!=null){{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for({rango} prop : {obtenerPrefijo}_{nombreProp}){{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.getProperties();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.getEntities();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}OntologyEntity entity{rango};");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}try{{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}entity{rango} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", prop.propList, prop.entList);");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}entList.add(entity{rango});");
                        if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(prop.Rango)))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}prop.setEntity(entity{rango});");
                        }
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}} catch (GnossAPIArgumentException ex) {{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}ex.printStackTrace();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    }
                }
            }
        }

        /// <summary>
        /// Obtenemos el titulo y la descripción de la clase, los cuales añadiremos en el toRecurso
        /// </summary>
        public void ObtenerTituloDescripcion()
        {
            XmlNode nodoTitulo = ConfiguracionGeneral.SelectSingleNode("TituloDoc");
            if (nodoTitulo != null)
            {
                nombrePropTituloEntero = nodoTitulo.InnerText;
                nombrePropTitulo = $"this.{ UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, nodoTitulo.InnerText))}_{ UtilCadenasOntology.ObtenerNombrePropJava(nodoTitulo.InnerText)}";
            }
            if (esPrimaria)
            {
                XmlNode nodoDescripcion = ConfiguracionGeneral.SelectSingleNode("DescripcionDoc");
                if (nodoDescripcion != null)
                {
                    nombrePropDescripcionEntera = nodoDescripcion.InnerText;
                    nombrePropDescripcion = $"this.{ UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, nodoDescripcion.InnerText))}_{ UtilCadenasOntology.ObtenerNombrePropJava(nodoDescripcion.InnerText)}";
                }
            }
        }

        private string ObtenerPrefijoPropiedad(Propiedad pPropiedad)
        {
            string pref = UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre).Trim();
            if (!string.IsNullOrEmpty(pref))
            {
                return UtilCadenas.PrimerCaracterAMayuscula(pref);
            }
            return pref;
        }


        /// <summary>
        /// Si la propiedad es unica para objetos o tipos de la entidad
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>
        /// <param name="pRango">Rango de la propiedad</param>
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        public void PropiedadObjetoEntidadValorUnico(Propiedad pPropiedad, string pRango, string pNombreClase, ElementoOntologia pPadre)
        {
            string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
            bool usarNew = false;
            if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
            {
                usarNew = true;
            }
            pRango = pRango.Replace("object", "Object").Replace("string", "String");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} {pRango} {nombrePropiedad};\n");            
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.getter(pRango, nombrePropiedad)}\n");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.setter(pRango, nombrePropiedad)}\n");
        }

        /// <summary>
        /// Si la propiedad es multiple para objetos o tipos de la entidad
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>
        /// <param name="pRango">Rango de la propiedad</param>
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        public void PropiedadObjetoEntidadValorMultiple(Propiedad pPropiedad, string pRango, string pNombreClase, ElementoOntologia pPadre)
        {
            string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
            bool usarNew = false;
            if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
            {
                usarNew = true;
            }
            if (pRango.Contains("Dictionary<"))
            {
                pRango.Replace("Dictionary", "HashMap");
                pRango.Replace("string", "String");
                pRango.Replace("object", "Object");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} {pRango} {nombrePropiedad};\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.getter(pRango, nombrePropiedad)}\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.setter(pRango, nombrePropiedad)}\n");
            }
            else
            {
                if (pRango == "string")
                {
                    pRango = "String";
                }
                string nuevoRango = $"ArrayList<{pRango}>";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} ArrayList<{pRango}> {nombrePropiedad} = new ArrayList<{pRango}>();\n");
                listasInicializar.Add(nombrePropiedad, nuevoRango);
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.getter(nuevoRango, nombrePropiedad)}\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.setter(nuevoRango, nombrePropiedad)}\n");
            }
        }

        /// <summary>
        /// Creacion de propiedades con los tipos u objetos de la entidad
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>
        /// <param name="pRango">Rango de la propiedad</param>
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pEsHeredada">Nos indica si la propiedad es heredada o no</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        public void CreacionPropiedadObjetosEntidad(Propiedad pPropiedad, string pRango, string pNombreClase, ElementoOntologia pPadre, bool pEsHeredada)
        {
            if (!pRango.Equals("object"))
            {
                listentidades.Add(pPropiedad);
                if (ontologia.Entidades.Exists(x => x.TipoEntidad.Contains(pPropiedad.Rango)))
                {
                    listentidadesAux.Add(pPropiedad);
                }
            }
            if (pPropiedad.ValorUnico && !pEsHeredada)
            {
                pRango.Replace("object", "Object");
                PropiedadObjetoEntidadValorUnico(pPropiedad, pRango, pNombreClase, pPadre);
            }
            else if (!pEsHeredada)
            {
                pRango.Replace("object", "Object");
                PropiedadObjetoEntidadValorMultiple(pPropiedad, pRango, pNombreClase, pPadre);
            }
        }

        /// <summary>
        /// Pinta el método "ObtenerValorPropiedadMetodo" en la clase
        /// </summary>
        protected void PintarObtenerValorPropiedadMetodo()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}private ArrayList<Object> ObtenerValorPropiedadMetodo(Object pPropiedad, String pNombreGet) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}Method[] metodos = pPropiedad.getClass().getMethods();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for(Method metodo : metodos) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(metodo.getName().equals(pNombreGet)) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}try{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}Object resultado = metodo.invoke(pPropiedad);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}if(resultado instanceof ArrayList<?>) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}return (ArrayList<Object>)resultado;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}ArrayList <Object> listaObjeto = new ArrayList<Object>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}listaObjeto.add(resultado);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}return listaObjeto;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}} catch (IllegalAccessException | IllegalArgumentException | InvocationTargetException e) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}e.printStackTrace();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return null;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        /// <summary>
        /// Pitna el método "ObtenerStringDePropiedad" en la clase
        /// </summary>
        protected void PintarObtenerStringDePropiedad()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}protected ArrayList<String> ObtenerStringDePropiedad(Object propiedad) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<String> lista = new ArrayList<String>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (propiedad instanceof ArrayList<?>) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for (String item : (ArrayList<String>) propiedad) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}lista.add(item);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else if (propiedad instanceof HashMap<?, ?>) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for (Object key : ((HashMap<?, ?>) propiedad).keySet()) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}if (((HashMap<?, ?>) propiedad).get(key) instanceof ArrayList<?>) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}ArrayList<String> listaValores = (ArrayList<String>) ((HashMap<LanguageEnum, ArrayList<String>>) propiedad).get(key); ");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}for (String valor : listaValores) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}lista.add(valor);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}else{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}lista.add((String) ((HashMap<LanguageEnum, String>) propiedad).get(key));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else if (propiedad instanceof String) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}lista.add((String) propiedad);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return lista;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        /// <summary>
        /// Si la propiedad es única para Objetos externos
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>        
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        public void PropiedadObjetoExternoValorUnico(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre)
        {
            if (!string.IsNullOrEmpty(pPropiedad.Rango))
            {
                string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
                bool useNew = false;
                if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
                {
                    useNew = true;
                }
                pPropiedad.Rango = pPropiedad.Rango.Replace("string", "String");
                pPropiedad.Rango = pPropiedad.Rango.Replace("object", "Object");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(useNew ? "new" : "")} {pPropiedad.Rango} {nombrePropiedad};\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.getter(pPropiedad.Rango, nombrePropiedad)}\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.setter(pPropiedad.Rango, nombrePropiedad)}\n");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} String Id{nombrePropiedad};" + "\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public String getId{nombrePropiedad}(){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return Id{nombrePropiedad};");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void setId{nombrePropiedad}(String value){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}Id{nombrePropiedad} = value;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        /// <summary>
        /// Si la propiedad es multiple para Objetos externos
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>        
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        public void PropiedadObjetoExternoValorMultiple(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre)
        {
            if (!string.IsNullOrEmpty(pPropiedad.Rango))
            {
                string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{pNombreClase}";
                bool usarNew = false;
                if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
                {
                    usarNew = true;
                }
                if (pPropiedad.Rango == "string")
                {
                    pPropiedad.Rango = "String";
                }
                if (pPropiedad.Rango == "object")
                {
                    pPropiedad.Rango = "Object";
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} {(usarNew ? "new" : "")} ArrayList<{pPropiedad.Rango}> {nombrePropiedad} = new ArrayList<{pPropiedad.Rango}>();\n");
                string nuevoRango = $"ArrayList <{pPropiedad.Rango}>";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.getter(nuevoRango, nombrePropiedad)}\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.setter(nuevoRango, nombrePropiedad)}\n");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{c.publicSolo} ArrayList<String> Ids{nombrePropiedad} = new ArrayList<String>();" + "\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.getter("ArrayList<String>", "Ids" + nombrePropiedad)}\n");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constJava.setter("ArrayList<String>", "Ids" + nombrePropiedad)}\n");
            }
        }

        /// <summary>
        /// Creacion de propiedades para tipos de Objetos externos
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>        
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        /// <param name="pEsHeredada">Nos indica si la propiedad es heredada</param>
        public void CreacionPropiedadObjetosExternos(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre, bool pEsHeredada)
        {
            if (pPropiedad.ValorUnico)
            {
                propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.StringSimple);
                if (!pEsHeredada)
                {
                    PropiedadObjetoExternoValorUnico(pPropiedad, pNombreClase, pPadre);
                }
            }
            else
            {
                if (!propListiedadesTipo.ContainsKey(pPropiedad.Nombre))
                {
                    propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.StringMultiple);
                }
                if (!pEsHeredada)
                {
                    PropiedadObjetoExternoValorMultiple(pPropiedad, pNombreClase, pPadre);
                }
            }
        }

        /// <summary>
        /// Creacion de las propiedades para objeto 
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>        
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        /// <param name="pEsHeredada">Nos indica si la propiedad es heredada</param>
        public void PropiedadesObjetoEntidad(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre, bool pEsHeredada)
        {
            string rango = "";
            if (pNombreClase.Equals("object"))
            {
                rango = "Object";
            }
            else
            {
                rango = UtilCadenasOntology.ObtenerNombrePropJava(pPropiedad.Rango);
            }

            if (ontologia.Entidades.Exists(x => x.TipoEntidad.Equals(pPropiedad.Rango)))
            {
                CreacionPropiedadObjetosEntidad(pPropiedad, rango, UtilCadenasOntology.ObtenerNombrePropJava(pPropiedad.Nombre), pPadre, pEsHeredada);
            }
            else
            {
                CreacionPropiedadObjetosExternos(pPropiedad, UtilCadenasOntology.ObtenerNombrePropJava(pPropiedad.Nombre), pPadre, pEsHeredada);
            }
        }

        /// <summary>
        /// Creacion de propiedades para datos
        /// </summary>
        /// <param name="pPropiedad">Propiedad del objeto</param>        
        /// <param name="pNombreClase">Nombre de la clase a la que pertenece la propiedad</param>
        /// <param name="pPadre">Ontología a la que pertenece la propiedad</param>
        /// <param name="pEsHeredada">Nos indica si la propiedad es heredada</param>
        /// <param name="pEntidad">Entidad a la que pertenece la propiedad</param>
        public void PropiedadesData(Propiedad pPropiedad, string pNombreClase, ElementoOntologia pPadre, ElementoOntologia pEntidad, bool pEsHeredada)
        {
            if (!string.IsNullOrEmpty(pPropiedad.Rango))
            {
                string rango = UtilCadenasOntology.ObtenerNombrePropJava(pPropiedad.Rango);
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
                                rango = "Calendar";
                                propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.DateSimple);
                            }
                            else
                            {
                                rango = "Calendar";
                                propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.DateNull);
                            }
                        }
                        else
                        {
                            rango = "Calendar";
                            propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.DateMultiple);
                        }
                        cadena = false;
                        break;
                    case "boolean":
                        rango = "boolean";
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
                                rango = "float";
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
                                rango = "int";
                            }
                        }
                        else
                        {
                            rango = "int";
                        };
                        entero = true;
                        break;
                    default:
                        rango = "String";
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

                    if (EsPropiedadMultiIdioma(pPropiedad.Nombre) && !esfecha)
                    {
                        rango = "HashMap<LanguageEnum, String>";
                    }

                    if (!pEsHeredada)
                    {
                        PropiedadObjetoEntidadValorUnico(pPropiedad, rango, pNombreClase, pPadre);
                    }
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

                    if (EsPropiedadMultiIdioma(pPropiedad.Nombre))
                    {
                        rango = "HashMap<LanguageEnum, ArrayList<String>>";
                    }

                    if (!pEsHeredada)
                    {
                        PropiedadObjetoEntidadValorMultiple(pPropiedad, rango, pNombreClase, pPadre);
                    }
                }
            }
        }

        /// <summary>
        /// Comprobamos si la propiedad pasada por parametro es multiidioma
        /// </summary>
        /// <param name="pPropiedad">Propiedad a comprobar</param>
        /// <returns></returns>
        public bool GetMultiIdiomaPropiedad(ElementoOntologia pEntidad, Propiedad pPropiedad)
        {
            ObjetoPropiedad objeto = listaObjetosPropiedad.FirstOrDefault(x => x.NombrePropiedad.Equals(pPropiedad.Nombre) && x.NombreEntidad.Equals(pEntidad.TipoEntidad));
            if (objeto != null)
            {
                return objeto.Multiidioma;
            }
            return false;
        }

        /// <summary>
        /// Creamos las propiedades de cada clase, si en la ontología la propiedad está dentro del Dominio
        /// </summary>
        /// <param name="pEntidad">Entidad de la cual se van a crear las entidades</param>
        public void CreacionDePropiedades(ElementoOntologia pEntidad)
        {
            if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public OntologyEntity Entity;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public OntologyEntity getEntity(){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return this.Entity; }}");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void setEntity(OntologyEntity ent){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.Entity=ent; }}");
                Clase.AppendLine();
            }

            ElementoOntologia padre = null;
            if (pEntidad.Superclases.Count > 0 && pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
            {
                padre = ontologia.Entidades.FirstOrDefault(ent => ent.TipoEntidad.Equals(pEntidad.Superclases[0]));
            }

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                bool esHeredada = pEntidad.Superclases != null && pEntidad.Superclases.Count > 0 && propiedad.Dominio.Contains(pEntidad.Superclases[0]);
                if (propiedad.Dominio.Contains(pEntidad.TipoEntidad) || esHeredada)
                {
                    string nombreProp = UtilCadenasOntology.ObtenerNombrePropJava(propiedad.Nombre);
                    Dictionary<string, string> dicLan = ObtenerLabels(pEntidad, propiedad);

                    if (propiedad.Tipo.ToString().Equals("ObjectProperty"))
                    {
                        if (ObtenerImportsExternosPropiedad(propiedad).Count == 0 && !ontologia.Entidades.Exists(x => x.TipoEntidad.Equals(propiedad.Rango)))
                        {
                            nombreProp = "Object";
                        }
                        else
                        {
                            listaObjetosExternos.Add(propiedad.Nombre);
                        }

                        PropiedadesObjetoEntidad(propiedad, nombreProp, padre, esHeredada);
                    }
                    else if (propiedad.Tipo.ToString().Equals("DatatypeProperty"))
                    {
                        PropiedadesData(propiedad, nombreProp, padre, pEntidad, esHeredada);
                    }

                    if (!esHeredada)
                    {
                        Clase.AppendLine();
                    }
                }

                if (EsMultiIdiomaConfig && propiedad.Rango.ToLower().Equals("string"))
                {
                    if (!propListiedadesMultidioma.ContainsKey(propiedad.Nombre))
                    {
                        propListiedadesMultidioma.Add(propiedad.Nombre, true);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene las etiquetas para la propiedad indicada
        /// </summary>
        /// <param name="pEntidad">Entidad a la que pertenece la propiedad</param>
        /// <param name="pPropiedad">Propiedad a obtener las etiquetas</param>
        /// <returns></returns>
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
        /// Crear el interior del toRecurso
        /// </summary>
        /// <param name="pEntidad">Entidad a crear el contenido del método "toRecurso"</param>
        /// <param name="pTipoOntologyResource">Tipo de la ontología. Puede ser para recursos semánticos</param>
        public void CrearToRecursoPadre(ElementoOntologia pEntidad, string pTipoOntologyResource)
        {
            if (pTipoOntologyResource.Equals("ComplexOntologyResource"))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}Ontology ontology=null;");
                if (listentidades.Any())
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}getEntities();");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}getProperties();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(idrecurso == null && idarticulo == null)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ontology = new Ontology(resourceAPI.getGraphsUrl().toString(), resourceAPI.getOntologyUrl().toString(), \"{pEntidad.TipoEntidad}\", \"{pEntidad.TipoEntidad}\", prefList, propList, entList);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ontology = new Ontology(resourceAPI.getGraphsUrl().toString(), resourceAPI.getOntologyUrl().toString(), \"{pEntidad.TipoEntidad}\", \"{pEntidad.TipoEntidad}\", prefList, propList, entList,idrecurso,idarticulo);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setGnossId(this.getGNOSSID());");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setOntology(ontology);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setTextCategories(listaDeCategorias);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddResourceTitle(resource);");

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddResourceTitle(resource);");
                if (!string.IsNullOrEmpty(this.nombrePropDescripcionEntera))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddResourceDescription(resource);");
                }

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddImages(resource);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddFiles(resource);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(super.tagList != null && super.tagList.size() > 0) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}String[] etiquetas = new String[super.tagList.size()];");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}etiquetas =  super.tagList.toArray(etiquetas);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}resource.setTags(etiquetas);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<SecondaryEntity> listSecondaryEntity = null;");
                if (listentidades.Any())
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}getEntities();");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}getProperties();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SecondaryOntology ontology = new SecondaryOntology(resourceAPI.getGraphsUrl(), resourceAPI.getOntologyUrl(), \"{pEntidad.TipoEntidad}\", \"{pEntidad.TipoEntidad}\", prefList, propList,identificador,listSecondaryEntity, {(listentidades.Any() ? "entList" : "null")});");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setSecondaryOntology(ontology);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setId(identificador);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.setGNOSSID(resourceAPI.GraphsUrl.concat(\"items\").concat(\"/\").concat(resource.getId()));");
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return resource;");
        }

        /// <summary>
        /// Créa el método "toRecurso" de la ontología indicada
        /// </summary>
        /// <param name="esPrimaria">Nos indica si la entidad es primaria o no</param>
        /// <param name="pEntidad">Entidad a crear el método toRecurso</param>
        public void CrearToRecurso(bool esPrimaria, ElementoOntologia pEntidad)
        {
            if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
            {
                string tipoOntologyResource = "ComplexOntologyResource";
                string modificador = "public";

                if (!esPrimaria)
                {
                    tipoOntologyResource = "SecondaryResource";
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI,String identificador) throws GnossAPIArgumentException, IOException, GnossAPIException");
                }
                else
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI, ArrayList<String> listaDeCategorias)throws GnossAPIArgumentException, IOException, GnossAPIException");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return ToGnossApiResource(resourceAPI, listaDeCategorias, null, null);");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                    Clase.AppendLine();
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI, ArrayList<String> listaDeCategorias, UUID idrecurso, UUID idarticulo) throws GnossAPIArgumentException, IOException, GnossAPIException");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{tipoOntologyResource} resource = new {tipoOntologyResource}();");
                CrearToRecursoPadre(pEntidad, tipoOntologyResource);
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }


        /// <summary>
        /// Genera el sujeto de los triples para la propiedad HasEntidad
        /// </summary>
        /// <returns>Devuelve el sujeto para el triple de la propiedad HasEntidad</returns>
        private string GenerarSujetoHasEntidad()
        {
            return "\".concat(resourceAPI.getGraphsUrl().toString().concat(getResourceID().toString()))";
        }

        /// <summary>
        /// Genera el sujeto de los triples para el grafo de búsqueda
        /// </summary>
        /// <returns>Devuelve el sujeto para el triple de la entidad indicada para el grafo de búsqueda</returns>
        private string GenerarSujetoBusqueda()
        {
            return "http://gnoss/\".concat(getResourceID().toString().toUpperCase())";
        }

        /// <summary>
        /// Genera el sujeto de los triples para el grafo de ontología
        /// </summary>
        /// <param name="pEntidad">La entidad de la cual vamos a generar el sujeto</param>
        /// <param name="pItem">Indica el nivel de anidamiento de las entidades auxiliares, si no se indica será una entidad principal</param>
        /// <returns>Devuelve el sujeto para el triple de la entidad indicada para el grafo de ontología</returns>
        private string GenerarSujetoOntologia(ElementoOntologia pEntidad, string pItem = "")
        {
            string articleID = "ArticleID";

            if (!string.IsNullOrEmpty(pItem))
            {
                articleID = $"{pItem}.{articleID}";
            }
            string entidadRelativo = pEntidad.TipoEntidadRelativo + "_";
            string p1 = "\".concat(resourceAPI.getGraphsUrl().toString()).concat(\"items/";
            string p2 = "\").concat(getResourceID().toString()).concat(\"_\").concat(getArticleID().toString())";

            return p1 + entidadRelativo + p2;
        }

        /// <summary>
        /// Nos genera el sujeto para una entidad indicada. En función de lo que se indique generará para el grafo de búsqueda o de ontología
        /// </summary>
        /// <param name="pEntidad">Entidad del sujeto del cual se va a generar el triple</param>        
        /// <param name="pItem">Indica el nivel de anidamiento de las entidades auxiliares, si no se indica será una entidad principal</param>
        /// <param name="pTipoSujeto">Nos indica si el sujeto que se va a generar es el sujeto para el grafo de búsqueda, ontología o es para el triple HasEntidad</param>
        /// <returns>Devuelve el sujeto del triple de la entidad indicada, para el grafo de búsqueda o de ontología según se indique</returns>
        private string GenerarSujeto(ElementoOntologia pEntidad, TiposSujeto pTipoSujeto, string pItem = "")
        {
            switch (pTipoSujeto)
            {
                case TiposSujeto.Busqueda:
                    return GenerarSujetoBusqueda();
                case TiposSujeto.Ontologia:
                    return GenerarSujetoOntologia(pEntidad, pItem);
                case TiposSujeto.HasEntidad:
                    return GenerarSujetoHasEntidad();
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Transforma el sujeto a un id corto.
        /// </summary>
        /// <param name="pNombreVariable">Nombre de la variable</param>
        /// <param name="pEsObject">Si la variable que se va a modificar es un objeto o no</param>
        /// <param name="pEsOntologia">Nos indica si la variable se va a generar para el grafo de búsqueda u ontología</param>
        /// <returns></returns>
        private string ModificarAIDCorto(string pNombreVariable, bool pEsObject, bool pEsOntologia)
        {
            if (pEsObject)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}String regex = (\"\\\\/items\\\\/.+_[0-9A-Fa-f]{{8}}[-]?(?:[0-9A-Fa-f]{{4}}[-]?){{3}}[0-9A-Fa-f]{{12}}_[0-9A-Fa-f]{{8}}[-]?(?:[0-9A-Fa-f]{{4}}[-]?){{3}}[0-9A-Fa-f]{{12}}\");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}String itemRegex = {pNombreVariable}.toString();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Pattern pat = Pattern.compile(regex);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Matcher mat = pat.matcher(itemRegex);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}if (mat.find())");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}itemRegex = (\"http://gnoss/\"+resourceAPI.GetShortGuid(itemRegex).toString().toUpperCase());");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");

                if (!pEsOntologia)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}else{{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}itemRegex = itemRegex.toLowerCase();");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                }
                return "itemRegex";
            }
            return pNombreVariable;
        }

        /// <summary>
        /// Se encarga de pintar en la clase las propiedades principales de la entidad.
        /// </summary>
        /// <param name="pElem">Ontología de la cual se van a pintar las propiedades</param>
        /// <param name="pEsOntologia">Nos indica si las propiedades a pintar son para el grafo de ontología o para el grafo de búsqueda</param>
        /// <param name="pNombrePadres">Nombre de la jerarquía de entidades y propiedades hasta llegar al nivel actual</param>
        /// <param name="pListaFacetaObjetoConocimientoProyecto">Lista con todas las facetas del proyecto del cual se estan generando las clases</param>
        /// <param name="pListaPadrePropiedadesAnidadas">Lista con las propiedades anidadas para generar el search</param>
        /// <param name="pListaPropiedadesSearch">Lista de propiedades simples para genear el search</param>
        /// <param name="pRutaPadreSearch">Conjunto de los niveles de la propiedad search pintados hasta ahora</param>
        /// <param name="pSujetoEntidadSuperior">Sujeto de la entidad de la que hereda esta propiedad</param>
        private void PintarPropiedades(ElementoOntologia pElem, bool pEsOntologia, string pSujetoEntidadSuperior, string pRutaPadreSearch = null, string pNombrePadres = "this", List<string> pListaPropiedadesSearch = null, List<string> pListaPadrePropiedadesAnidadas = null, List<FacetaObjetoConocimientoProyecto> pListaFacetaObjetoConocimientoProyecto = null)
        {
            TiposSujeto tipoSujeto = TiposSujeto.Busqueda;

            if (pEsOntologia)
            {
                tipoSujeto = TiposSujeto.Ontologia;
            }

            foreach (Propiedad prop in pElem.Propiedades)
            {
                bool pintarSearch = false;
                bool esPropiedadSearchAnidada = false;
                List<string> propiedadSearchAnidada = new List<string>();

                if (!pElem.Ontologia.EntidadesAuxiliares.Any(x => x.TipoEntidad.Equals(prop.Rango)))
                {
                    bool esPropiedadTextoInvariable = false;
                    if (pListaFacetaObjetoConocimientoProyecto != null)
                    {
                        esPropiedadTextoInvariable = pListaFacetaObjetoConocimientoProyecto.Any(item => (item.Faceta.Equals(prop.NombreConNamespace) || item.Faceta.Equals($"{pRutaPadreSearch}@@@{prop.NombreConNamespace}")) && item.TipoPropiedad.Value.Equals(5));
                    }

                    ConfiguracionObjetoJava configuracionObjeto = new ConfiguracionObjetoJava(dicPref, prop, pElem, pEsOntologia, esPropiedadTextoInvariable, mLoggingService);

                    string identificadorValor = $"{configuracionObjeto.Id}{configuracionObjeto.PrefijoPropiedad}_{configuracionObjeto.NombrePropiedad}";
                    string propiedadParaSearch = $"{configuracionObjeto.PrefijoPropiedad}:{configuracionObjeto.NombrePropiedad}".ToLower();

                    if (!string.IsNullOrEmpty(pRutaPadreSearch))
                    {
                        propiedadParaSearch = $"{pRutaPadreSearch}@@@{propiedadParaSearch}";
                    }

                    if (pListaPropiedadesSearch != null && pListaPropiedadesSearch.Any(item => item.ToLower().Equals(propiedadParaSearch)) && !pEsOntologia)
                    {
                        pintarSearch = true;
                    }

                    if (pListaPadrePropiedadesAnidadas != null && pListaPadrePropiedadesAnidadas.Where(item => item.ToLower().StartsWith(propiedadParaSearch)).FirstOrDefault() != null && !pEsOntologia /* && propiedadParaSearch.Contains("@@@")*/)
                    {
                        esPropiedadSearchAnidada = true;
                        propiedadSearchAnidada = pListaPropiedadesSearch.Where(item => item.ToLower().StartsWith(propiedadParaSearch)).ToList();
                    }

                    if (configuracionObjeto.Rango.ToLower().Equals("datetime"))
                    {
                        if (prop.CardinalidadMaxima > 1)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null)");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null && !{pNombrePadres}.{identificadorValor}.equals(toCalendar(fechaMin1)))");
                        }
                    }
                    else
                    {
                        if (configuracionObjeto.Rango == "string" || configuracionObjeto.Rango == "String" && pintarSearch)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null && !{pNombrePadres}.{identificadorValor}.isEmpty())");
                        }
                        else if (!configuracionObjeto.EsPrimitivo)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if({pNombrePadres}.{identificadorValor} != null)");
                        }
                    }

                    if (!configuracionObjeto.EsPrimitivo)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
                    }

                    if (prop.CardinalidadMinima < 1)
                    {
                        if (!prop.ValorUnico)
                        {
                            if (EsPropiedadMultiIdioma(prop.Nombre) || EsPropiedadExternaMultiIdioma(pElem, prop))
                            {
                                GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch);
                            }
                            else
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}for(Object item2 : {pNombrePadres}.{identificadorValor})");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");

                                string valorTriple = "item2";
                                if (!pEsOntologia)
                                {
                                    valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia);
                                }
                                if (configuracionObjeto.Rango.ToLower() == "date" || configuracionObjeto.Rango.ToLower() == "datetime")
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}item2 = new SimpleDateFormat(\"yyyyMMddHHmmss\").format(((Calendar){valorTriple}).getTime());");
                                }
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}if(String.valueOf({valorTriple}){configuracionObjeto.Aux} != null)");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{{");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat(String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\"{configuracionObjeto.SimboloFin}\").concat(\" . \")));");

                                if (pintarSearch)
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}search += String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\" \");");
                                }

                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");

                                if (esPropiedadSearchAnidada)
                                {
                                    if (prop.ValorUnico)
                                    {
                                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Object itemProp0 = null;");
                                    }
                                    foreach (string propiedad in propiedadSearchAnidada)
                                    {
                                        GenerarSearch(propiedad, prop);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (EsPropiedadMultiIdioma(prop.Nombre) || EsPropiedadExternaMultiIdioma(pElem, prop))
                            {
                                GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch);
                            }
                            else
                            {
                                string valorTriple = $"{pNombrePadres}.{identificadorValor}";
                                if (!pEsOntologia)
                                {
                                    valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia);
                                }

                                if (configuracionObjeto.Rango.ToLower() == "date" || configuracionObjeto.Rango.ToLower() == "datetime")
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}String format = new SimpleDateFormat(\"yyyyMMddHHmmss\").format(((Calendar){valorTriple}).getTime()).toString();");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat(format).concat(\"{configuracionObjeto.SimboloFin} \").concat(\".\"));");
                                }
                                else
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat(String.valueOf({valorTriple}){configuracionObjeto.Aux}).concat(\"{configuracionObjeto.SimboloFin}\").concat(\" . \"));");
                                }

                                if (pintarSearch)
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\" \");");
                                }

                                if (esPropiedadSearchAnidada)
                                {
                                    if (prop.ValorUnico)
                                    {
                                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Object itemProp0 = null;");
                                    }

                                    foreach (string propiedad in propiedadSearchAnidada)
                                    {
                                        GenerarSearch(propiedad, prop);
                                    }
                                }
                            }
                        }
                    }
                    else if (!prop.ValorUnico)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}for(Object item2 : {pNombrePadres}.{identificadorValor})");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");

                        if (EsPropiedadMultiIdioma(prop.Nombre) || EsPropiedadExternaMultiIdioma(pElem, prop))
                        {
                            GenerarPropiedadMultiIdioma(prop, pSujetoEntidadSuperior, configuracionObjeto, pNombrePadres, identificadorValor, pintarSearch);
                        }
                        else
                        {
                            string valorTriple = $"item2";
                            if (configuracionObjeto.Rango.ToLower() == "date" || configuracionObjeto.Rango.ToLower() == "datetime")
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}item2 = new SimpleDateFormat(\"yyyyMMddHHmmss\").format(((Calendar){valorTriple}).getTime());");
                            }

                            valorTriple += ".toString()";
                            if (!pEsOntologia)
                            {
                                valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia);
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}if(String.valueOf({valorTriple}){configuracionObjeto.Aux} != null)");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{{");

                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat(String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\"{configuracionObjeto.SimboloFin}\" ).concat(\" . \")));");

                            if (pintarSearch)
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\" \");");
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
                        }
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");

                        if (esPropiedadSearchAnidada)
                        {
                            if (prop.ValorUnico)
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Object itemProp0 = null;");
                            }
                            foreach (string propiedad in propiedadSearchAnidada)
                            {
                                GenerarSearch(propiedad, prop);
                            }
                        }
                    }
                    else
                    {
                        if (EsPropiedadMultiIdioma(prop.Nombre) || EsPropiedadExternaMultiIdioma(pElem, prop))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}for (LanguageEnum LanguageEnum : LanguajeEnum.values())");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat({pNombrePadres}.{identificadorValor}{configuracionObjeto.Aux}).concat(\"{configuracionObjeto.SimboloFin} @[LanguageEnum]\").concat(\" . \"));");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                        }
                        else
                        {
                            string valorTriple = $"{pNombrePadres}.{identificadorValor}";
                            if (!pEsOntologia)
                            {
                                valorTriple = ModificarAIDCorto(valorTriple, configuracionObjeto.EsObject, pEsOntologia);
                            }

                            if (configuracionObjeto.Rango.ToLower() == "date" || configuracionObjeto.Rango.ToLower() == "datetime")
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}String format = new SimpleDateFormat(\"yyyyMMddHHmmss\").format(((Calendar){valorTriple}).getTime()).toString();");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat(format).concat(\"{configuracionObjeto.SimboloFin} \").concat(\".\"));");
                            }
                            else
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}list.add((\"<{pSujetoEntidadSuperior}).concat(\"> <{prop.NombreFormatoUri}> {configuracionObjeto.SimboloInicio}\").concat(String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\"{configuracionObjeto.SimboloFin} \").concat(\".\")));");
                            }

                            if (pintarSearch)
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += String.valueOf({valorTriple}){configuracionObjeto.Aux}.concat(\" \");");
                            }

                            if (esPropiedadSearchAnidada)
                            {
                                if (prop.ValorUnico)
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}Object itemProp0 = null;");
                                }
                                foreach (string propiedad in propiedadSearchAnidada)
                                {
                                    GenerarSearch(propiedad, prop);
                                }
                            }
                        }
                    }
                    if (!configuracionObjeto.EsPrimitivo)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                    }
                }
            }
        }

        /// <summary>
        /// Geneara el contenido para pintar las propiedades multiidioma
        /// </summary>
        /// <param name="pProp">Propiedad a pintar</param>
        /// <param name="pSujetoEntidadSuperior">Sujeto de la entidad de la que hereda la propiedad</param>
        /// <param name="pConfiguracionObjeto">Configuración de la propiedad a pintar</param>
        /// <param name="pNombrePadre">Nombre de la entidad de la que hereda la propiedad a pintar</param>
        /// <param name="pIdentificadorValor">Nombre de la variable de la propiedad a pintar</param>
        /// <param name="pPintarSearch">Indica si el valor de esta propiedad se debe añadir o no al search</param>
        private void GenerarPropiedadMultiIdioma(Propiedad pProp, string pSujetoEntidadSuperior, ConfiguracionObjetoJava pConfiguracionObjeto, string pNombrePadre, string pIdentificadorValor, bool pPintarSearch)
        {
            if (!pProp.ValorUnico)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}for (LanguageEnum idioma : {pNombrePadre}.{pIdentificadorValor}.keySet())");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}ArrayList<String> listaValores = {pNombrePadre}.{pIdentificadorValor}.get(idioma);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}for (String valor : listaValores)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(9)}list.add(\"<{pSujetoEntidadSuperior}> <{pProp.NombreFormatoUri}> {pConfiguracionObjeto.SimboloInicio}valor{pConfiguracionObjeto.Aux}{pConfiguracionObjeto.SimboloFin}@[idioma] . \");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}}}");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}LanguageEnum idioma = {pNombrePadre}.{pIdentificadorValor}.getKey();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}list.Add($\"<{pSujetoEntidadSuperior}> <{pProp.NombreFormatoUri}> {pConfiguracionObjeto.SimboloInicio}{pNombrePadre}.{pIdentificadorValor}.get(idioma){pConfiguracionObjeto.Aux}{pConfiguracionObjeto.SimboloFin}@[idioma] . \");");
            }
        }

        /// <summary>
        /// Se encarga de pintar en la clase de la ontología indicada sus entidades auxiliares.
        /// </summary>
        /// <param name="pElem">Ontología de la cual se van a pintar las entidades auxiliares</param>
        /// <param name="pPropiedadPadre">Propiedad propiedad padre</param>
        /// <param name="pElemPadre">En caso de ser entidades auxiliares anidadas, ontología de donde proviene las entidades que se van a pintar</param>
        /// <param name="pEsOntologia">Nos indica si las entidades auxiliares se van a pintar para el grafo de búsqueda o de ontología. Si no es uno, es otro</param>
        /// <param name="pNombrePadres">Nombre de la jerarquía de entidades y propiedades hasta llegar al nivel actual</param>
        /// <param name="pNumIteraciones">Numero de veces que se ha utilizado el método recursivamente</param>
        /// <param name="pSujetoEntidadSuperior">Sujeto de la entidad de la que hereda la propiedad</param>
        /// <param name="pListaPropiedadesSearch">Lista de propiedades simples para genear el search</param>
        /// <param name="pListaPadrePropiedadesAnidadas">Lista de propiedades anidadas para generar el search</param>
        private void PintarEntidadesAuxiliares(ElementoOntologia pElem, Propiedad pPropiedadPadre, ElementoOntologia pElemPadre, bool pEsOntologia, string pSujetoEntidadSuperior, string pNombrePadres = "this", List<string> pListaPropiedadesSearch = null, List<string> pListaPadrePropiedadesAnidadas = null, List<FacetaObjetoConocimientoProyecto> pListaFacetaObjetoConocimientoProyecto = null, int pNumIteraciones = 0)
        {
            string prefijoPadre = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedadPadre.Nombre));
            string nombrePropPadre = UtilCadenasOntology.ObtenerNombrePropJava(pPropiedadPadre.Nombre);
            string nombreCompletoPadre = $"{pNombrePadres}.{prefijoPadre}_{nombrePropPadre}";
            TiposSujeto tipoSujeto = TiposSujeto.Busqueda;
            if (pEsOntologia)
            {
                tipoSujeto = TiposSujeto.Ontologia;
            }

            string item = nombreCompletoPadre;
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({nombreCompletoPadre} != null)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");

            if (!pPropiedadPadre.ValorUnico)
            {
                item = $"item{pNumIteraciones}";

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for({pElem.TipoEntidadRelativo} {item} : {nombreCompletoPadre})");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            }

            string sujetoEntidadAuxiliar;

            if (pEsOntologia)
            {
                sujetoEntidadAuxiliar = $"resourceAPI.getGraphsUrl().toString(){AgregarConcat($"\"items/{pElem.TipoEntidadRelativo}\"")}{AgregarConcat("\"_\"")}{AgregarConcat("getResourceID().toString()")}{AgregarConcat("\"_\"")}{AgregarConcat($"{item}.getArticleID().toString()")}";

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}list.add(\"<{GenerarSujeto(pElem, tipoSujeto, item)}{AgregarConcat($"\"> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <{pElem.TipoEntidad}> . \"")});");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}list.add(\"<{GenerarSujeto(pElem, tipoSujeto, item)}{AgregarConcat($"\"> <http://www.w3.org/2000/01/rdf-schema#label> \\\"{pElem.TipoEntidad}\\\" . \"")});");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}list.add(\"<{GenerarSujeto(pElem, TiposSujeto.HasEntidad)}{AgregarConcat($"\"> <http://gnoss/hasEntidad> <\"")}{AgregarConcat("resourceAPI.getGraphsUrl().toString()")}{AgregarConcat($"\"items/{pElem.TipoEntidadRelativo}_\"")}{AgregarConcat("getResourceID().toString()")}{AgregarConcat("\"_\"")}{AgregarConcat($"{item}.getArticleID().toString()")}{AgregarConcat("\">.\"")}); ");
            }
            else
            {
                sujetoEntidadAuxiliar = $"resourceAPI.getGraphsUrl().toString(){AgregarConcat($"\"items/{pElem.TipoEntidadRelativo.ToLower()}\"")}{AgregarConcat("\"_\"")}{AgregarConcat("getResourceID().toString()")}{AgregarConcat("\"_\"")}{AgregarConcat($"{item}.getArticleID().toString()")}";
            }

            if (pSujetoEntidadSuperior.StartsWith("resourceAPI"))
            {
                pSujetoEntidadSuperior = $"\"{AgregarConcat(pSujetoEntidadSuperior)}";
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}list.add(\"<{pSujetoEntidadSuperior}{AgregarConcat($"\"> <{pPropiedadPadre.NombreFormatoUri}> <\"")}{AgregarConcat($"{sujetoEntidadAuxiliar}")}{AgregarConcat("\"> . \"")});");

            foreach (ElementoOntologia elem in pElem.Ontologia.EntidadesAuxiliares)
            {
                foreach (Propiedad propiedadPadre in pElem.Propiedades)
                {
                    if (elem.TipoEntidad.Equals(propiedadPadre.Rango) && !pElem.TipoEntidad.Equals(propiedadPadre.Rango))
                    {
                        PintarEntidadesAuxiliares(elem, propiedadPadre, pElem, pEsOntologia, sujetoEntidadAuxiliar, item, pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas, pListaFacetaObjetoConocimientoProyecto, ++pNumIteraciones);
                    }
                }
            }

            if (sujetoEntidadAuxiliar.StartsWith("resourceAPI"))
            {
                sujetoEntidadAuxiliar = $"\"{AgregarConcat(sujetoEntidadAuxiliar)}";
            }

            PintarPropiedades(pElem, pEsOntologia, sujetoEntidadAuxiliar, pPropiedadPadre.NombreConNamespace, item, pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas, pListaFacetaObjetoConocimientoProyecto);

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");

            if (!pPropiedadPadre.ValorUnico)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            }
        }

        public void CrearToOntologyGraphTriples(ElementoOntologia pEntidad)
        {
            if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public  ArrayList<String> ToOntologyGnossTriples(ResourceApi resourceAPI)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<String> list = new ArrayList<String>();");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} try{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)} fechaMin1=sdf1.parse(date1);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)} }} catch (ParseException e) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)} e.printStackTrace();}}");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Ontologia)}.concat(\"> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> <{pEntidad.TipoEntidad}> . \"));");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Ontologia)}.concat(\"> <http://www.w3.org/2000/01/rdf-schema#label> \\\"{pEntidad.TipoEntidad}\\\" . \"));");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.HasEntidad)}.concat(\"> <http://gnoss/hasEntidad> <\").concat(resourceAPI.getGraphsUrl().toString())+\"items/{pEntidad.TipoEntidadGeneracionClases}_\".concat(getResourceID().toString()).concat(\"_\").concat(getArticleID().toString()).concat(\"> . \"));");

                //Recorro las entidades auxiliares
                foreach (ElementoOntologia elem in ontologia.EntidadesAuxiliares)
                {
                    foreach (Propiedad propiedadPadre in listentidadesAux)
                    {
                        if (elem.TipoEntidad.Equals(propiedadPadre.Rango))
                        {
                            PintarEntidadesAuxiliares(elem, propiedadPadre, pEntidad, true, GenerarSujeto(pEntidad, TiposSujeto.Ontologia));
                        }
                    }
                }

                PintarPropiedades(pEntidad, true, GenerarSujeto(pEntidad, TiposSujeto.Ontologia));
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return list;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        private void GenerarSearch(string pPropiedad, Propiedad pProp, string pNombreVariableEntidadActual = "this")
        {
            string[] listaPropiedadesAnidadas = pPropiedad.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

            int longitudLista = listaPropiedadesAnidadas.Length;
            string nombreVariableActual = $"{pNombreVariableEntidadActual}.{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[0]).Replace(":", "_")}";

            if (pProp.ValorUnico)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}itemProp0 = {nombreVariableActual};");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}for(Object itemProp0 : {nombreVariableActual})");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
            }
            if (listaPropiedadesAnidadas.Length == 2)
            {
                string nombrePropiedadHija = $"{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[1]).Replace(":", "_")}";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}ArrayList<Object> lista{nombrePropiedadHija} = ObtenerPropiedadReflection(itemProp{0}, \"get{nombrePropiedadHija}\");");
            }

            for (int i = 1; i + 1 < listaPropiedadesAnidadas.Length; i++)
            {
                string nombrePropiedadHija = $"{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[i]).Replace(":", "_")}";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}ArrayList<Object> lista{nombrePropiedadHija} = ObtenerPropiedadReflection(itemProp{i - 1}, \"get{nombrePropiedadHija}\");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}for(Object itemProp{i} : lista{nombrePropiedadHija})");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{{");
                nombrePropiedadHija = $"{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[i + 1]).Replace(":", "_")}";
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6 + i)}ArrayList<Object> lista{nombrePropiedadHija} = ObtenerPropiedadReflection(itemProp{i}, \"get{nombrePropiedadHija}\");");
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + longitudLista)}for(Object itemProp{longitudLista - 1} : lista{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[longitudLista - 1]).Replace(":", "_")})");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + longitudLista)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5 + longitudLista)}listaSearch.addAll(ObtenerStringDePropiedad(itemProp{longitudLista - 1}));");

            if (!pProp.ValorUnico)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4 + longitudLista)}}}");
            }

            for (int i = longitudLista - 1; i > 0; i--)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5 + i)}}}");
            }
        }

        /// <summary>
        /// Pinta el método que crea el fichero que va a utilizarse para cargar los datos de la base de datos con la carga masiva
        /// </summary>
        /// <param name="pEsPrimaria">Nos indica si la ontología es o no primaria</param>
        /// <param name="pEntidad">Entidad para la cual se va a crear el método</param>
        public void CrearToAcidData(bool pEsPrimaria, ElementoOntologia pEntidad)
        {
            if (pEsPrimaria)
            {
                if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public  HashMap<UUID, String> ToAcidData(ResourceApi resourceAPI)");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                    Clase.AppendLine();

                    //Insert en la tabla Documento y Guid que identifica
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}//Insert en la tabla Documento");

                    PintarValorTituloDescripcion(pEntidad, true);
                    if (!string.IsNullOrEmpty(nombrePropDescripcion))
                    {
                        PintarValorTituloDescripcion(pEntidad, false);

                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String tablaDoc = \"'\" + titulo + \"', '\" + descripcion + \"', '\" + resourceAPI.getGraphsUrl() + \"', '\";");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String tablaDoc = \"'\" + titulo + \"', '', '\" + resourceAPI.getGraphsUrl() + \"', '\";");
                    }

                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for(String tag : tagList) {{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}tablaDoc += tag + \", \";");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}tablaDoc = tablaDoc.substring(0, tablaDoc.lastIndexOf(','));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}if(tagList != null && tagList.size() > 0) {{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}tablaDoc += \"'\";");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}HashMap<UUID, String> valor = new HashMap<UUID, String>();");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}valor.put(getResourceID(), tablaDoc);");
                    Clase.AppendLine();
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return valor;");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                }
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public  HashMap<UUID, String> ToAcidData(ResourceApi resourceAPI)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}HashMap<UUID, String> valor = new HashMap<UUID, String>();");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return valor;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        /// <summary>
        /// Pinta el método que se encarga de generar la lista de tags para insertar en virtuoso
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se va a crear el método</param>
        /// <param name="pListaFacetaObjetoConocimientoProyecto">Lista de Facetas configuradas para el proyecto del cual se generarán las clases</param>
        private void CrearAgregarTag(ElementoOntologia pEntidad, List<FacetaObjetoConocimientoProyecto> pListaFacetaObjetoConocimientoProyecto)
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}private void AgregarTags(ArrayList<String> pListaTriples)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for(String tag : tagList)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            if (pListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals("sioc_t:Tag") && item.TipoPropiedad.Value.Equals(5)))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pListaTriples.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}{AgregarConcat("\"> <http://rdfs.org/sioc/types#Tag> \\\"\"")}{AgregarConcat("tag")}{AgregarConcat("\"\\\" . \"")});");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pListaTriples.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}{AgregarConcat("\"> <http://rdfs.org/sioc/types#Tag> \\\"\"")}{AgregarConcat("tag.toLowerCase()")}{AgregarConcat("\"\\\" . \"")});");
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        /// <summary>
        /// Se encarga de pintar la parte que genera el título y la descripción para la base de datos con la carga masiva
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se va a generar el título y la descripción</param>
        /// <param name="pEsTitulo">Nos indica si el valor que va a pintar es el título. Si no es el título es la descripción</param>
        private void PintarValorTituloDescripcion(ElementoOntologia pEntidad, bool pEsTitulo)
        {
            string nombreVariable = "descripcion";
            string nombreProp = nombrePropDescripcion;
            int tabulador = 3;
            if (pEsTitulo)
            {
                nombreProp = nombrePropTitulo;
                nombreVariable = "titulo";
                tabulador = 2;
            }

            if (pEntidad.Propiedades.Any(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero)))
            {
                Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero));

                if (propiedad.CardinalidadMinima == 0)
                {
                    throw new Exception($"La propiedad {propiedad.Nombre} de la entidad {pEntidad.TipoEntidad} debe de tener valor obligatorio.");
                }

                if (EsPropiedadMultiIdioma(nombrePropTituloEntero) || EsPropiedadExternaMultiIdioma(pEntidad, nombrePropTituloEntero))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String {nombreVariable} = \"\"");

                    if (!pEsTitulo)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador)}if({nombreProp} != null) {{");
                    }
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 1)}for(LanguageEnum idioma : {nombreProp}.keySet())");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 1)}{{");
                    if (propiedad.ValorUnico)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 2)}{nombreVariable} += ${nombreProp}.get(idioma) + \"@\" + idioma;");
                    }
                    else
                    {

                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 2)}for(String tituloMultiidioma : {nombreProp}.values())");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 2)}{{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 2)}{nombreVariable} += tituloMultiidioma + \"@\" + idioma + \"|| \"");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 2)}}}");
                    }
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador + 1)}}}");
                    if (!pEsTitulo)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(tabulador)}}}");
                    }
                }
                else
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String {nombreVariable} = \"\";");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({nombreProp} != null)");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                    if (propiedad.ValorUnico)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{nombreVariable} = {nombreProp}.replace(\"\\r\\n\", \"\").replace(\"\\n\", \"\").replace(\"\\r\", \"\").replace(\"'\", \"#COMILLA#\").replace(\"|\", \"#PIPE#\");");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{nombreVariable} = string.join(\", \",{nombreProp}).replace(\"\\r\\n\", \"\").replace(\"\\n\", \"\").replace(\"\\r\", \"\").replace(\"'\", \"#COMILLA#\").replace(\"|\", \"#PIPE#\");");
                    }
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                }
            }
        }

        /// <summary>
        /// Comprueba si la propiedad externa indicada esta configurada como multi idioma
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan generando las clases</param>
        /// <param name="pProp">Propiedad la cual se va comprobar si es o no multi idioma</param>
        /// <returns></returns>
        public bool EsPropiedadExternaMultiIdioma(ElementoOntologia pEntidad, Propiedad pProp)
        {
            return EsPropiedadExternaMultiIdioma(pEntidad, pProp.Nombre);
        }

        /// <summary>
        /// Comprueba si la propiedad externa indicada esta configurada como multi idioma
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan generando las clases</param>
        /// <param name="pNombre">Nombre de la propiedad la cual se va comprobar si es o no multi idioma</param>
        /// <returns></returns>
        public bool EsPropiedadExternaMultiIdioma(ElementoOntologia pEntidad, string pNombre)
        {
            ObjetoPropiedad propiedad = listaObjetosPropiedad.FirstOrDefault(item => item.NombrePropiedad.Equals(pNombre) && item.NombreEntidad.Equals(pEntidad.TipoEntidad));
            return propiedad.Multiidioma;
        }

        /// <summary>
        /// Pinta una concatenación en la clase con el contenido indicado
        /// </summary>
        /// <param name="pContenidoConcat">Contenido que se va a concatenar</param>
        /// <returns></returns>
        private string AgregarConcat(string pContenidoConcat)
        {
            return $".concat({pContenidoConcat})";
        }

        /// <summary>
        /// Pinta el método que devuelve la URI del recurso indicado
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan generando las calses</param>
        public void CrearGetURI(ElementoOntologia pEntidad)
        {
            if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public  String GetURI(ResourceApi resourceAPI)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return (resourceAPI.getGraphsUrl().toString()).concat(\"items/{nombreOnto}\").concat(\"_\").concat(getResourceID().toString()).concat(getArticleID().toString());");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        /// <summary>
        /// Pinta el método que se utiliza para dada una propiedad y un nombre del método obtiene la lista de propiedades de la clase.
        /// </summary>
        private void PintarObtenerPropiedadReflection()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}private ArrayList<Object> ObtenerPropiedadReflection(Object propiedad, String nombreGet) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<Object> listaPropiedades = new ArrayList<Object>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(propiedad instanceof ArrayList<?>) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for(Object objetoPropiedad : (ArrayList<Object>)propiedad) {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}listaPropiedades.addAll(ObtenerValorPropiedadMetodo(objetoPropiedad, nombreGet));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else {{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}listaPropiedades.addAll(ObtenerValorPropiedadMetodo(propiedad, nombreGet));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return listaPropiedades;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        /// <summary>
        /// Pinta el método que que añade un titulo al recurso pasado por parametro
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        public void AgregarTituloRecurso(ElementoOntologia pEntidad)
        {
            if ((!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count) && !pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddResourceTitle(ComplexOntologyResource resource)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                if (esPrimaria)
                {
                    if (EsPropiedadMultiIdioma(this.nombrePropTituloEntero))
                    {
                        Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero));

                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<Multilanguage> multiTitleList = new ArrayList<Multilanguage>();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for (LanguageEnum languageEnum : {nombrePropTitulo}.keySet())");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        if (propiedad.ValorUnico)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}multiTitleList.add(new Multilanguage({nombrePropTitulo}.get(languageEnum), languageEnum.toString()));");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}multiTitleList.add(new Multilanguage(String.join(\", \", {nombrePropTitulo}.get(languageEnum)), languageEnum.ToString()));");
                        }
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setMultilanguageTitle(multiTitleList);");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setTitle({nombrePropTitulo});");
                    }
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        /// <summary>
        /// Pinta el método que que añade una descripción al recurso pasado por parametro
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        public void AgregarDescripcionRecurso(ElementoOntologia pEntidad)
        {
            if ((!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count) && !pEntidad.Superclases.Any(s => !s.Contains("http://www.w3.org/2002/07/owl#Thing")))
            {
                if (!string.IsNullOrEmpty(this.nombrePropDescripcionEntera))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddResourceDescription(ComplexOntologyResource resource)");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");

                    if (EsPropiedadMultiIdioma(this.nombrePropDescripcionEntera))
                    {
                        Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero));

                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<Multilanguage> listMultilanguageDescription = new ArrayList<Multilanguage>();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for (LanguageEnum idioma : {nombrePropDescripcion}.keySet())");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        if (propiedad.ValorUnico)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}listMultilanguageDescription.add(new Multilanguage({nombrePropDescripcion}.get(idioma), idioma.toString()));");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}listMultilanguageDescription.add(new Multilanguage(String.Join(\", \", {nombrePropDescripcion}.get(idioma), idioma.toString()));");
                        }

                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setMultilanguageDescription(listMultilanguageDescription);");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.setDescription({nombrePropDescripcion});");
                    }
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                }
            }
        }

        /// <summary>
        /// Pinta la de la clase que se utiliza para añadir una imagen a un recurso
        /// </summary>
        /// <param name="pPropiedad">Propiedad de la ontología para la imagen</param>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        /// <param name="pNombre">Nombre de la variable que contendrá la imagen</param>
        public void AnadirImagenesSimples(Propiedad pPropiedad, ElementoOntologia pEntidad, string pNombre)
        {
            string nombreProp = UtilCadenasOntology.ObtenerNombrePropJava(pPropiedad.Nombre);
            string prefijoYPropiedad = ObtenerPrefijoYPropiedad(dicPref, pPropiedad.Nombre);
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<ImageAction> actionList{nombreProp} = new ArrayList<ImageAction>();");
            foreach (List<string> listaAcc in ImageAction(pPropiedad).Values)
            {
                for (int i = 0; i < listaAcc.Count; i = i + 3)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}actionList{nombreProp}.add(new ImageAction({listaAcc[i + 2]},{listaAcc[i + 1]}, {listaAcc[i]}, 100));");
                }
                if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}pResource.AttachImage({pNombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", true, this.GetExtension({pNombre}), this.Entity, null, null);");
                }
                else
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}pResource.AttachImage({pNombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", true, this.GetExtension({pNombre}), null, null, null);");
                }
            }
        }

        /// <summary>
        /// Pinta la parte de la clase que genera la acción que se quiere realizar con la imagen
        /// </summary>
        /// <param name="propiedad">Propiedad de la ontología para la imagen</param>
        /// <returns></returns>
        public Dictionary<string, List<string>> ImageAction(Propiedad propiedad)
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
        /// Pinta la parte de la clase que se utiliza para añadir la imagen a un recurso
        /// </summary>
        public void AnadirImagenesEntidades()
        {
            foreach (Propiedad prop in listentidades)
            {
                if (!prop.Rango.Equals("object"))
                {
                    if (prop.ValorUnico)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre)}.AddImages(pResource);");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({ UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre)}!=null){{");
                        Clase.AppendLine($"{ UtilCadenasOntology.Tabs(4)}for ({UtilCadenasOntology.ObtenerNombrePropJava(prop.Rango)} prop : { UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre)})");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}prop.AddImages(pResource);");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    }
                }
            }
        }

        /// <summary>
        /// Pinta la parte de la clase que se utiliza para añadir imagenes múltiples a un recurso
        /// </summary>
        /// <param name="pPropiedad">Propiedad de la ontología que tiene las imágenes</param>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        /// <param name="pNombre">Nombre de la variable que contiene el valor de la imagen</param>
        public void AnadirImagenesMultiples(Propiedad pPropiedad, ElementoOntologia pEntidad, string pNombre)
        {
            string nombreProp = UtilCadenasOntology.ObtenerNombrePropJava(pPropiedad.Nombre);
            string prefijoYPropiedad = ObtenerPrefijoYPropiedad(dicPref, pPropiedad.Nombre);
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ArrayList<ImageAction> actionList{nombreProp} = new ArrayList<ImageAction>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}boolean principal=true;");
            foreach (List<string> listaAcc in ImageAction(pPropiedad).Values)
            {
                for (int i = 0; i < listaAcc.Count; i = i + 3)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}actionList{nombreProp}.add(new ImageAction({listaAcc[i + 2]},{listaAcc[i + 1]}, {listaAcc[i]}, 100));");
                }
                if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachImage({pNombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", principal, this.GetExtension({pNombre}),this.Entidad, null, null);");
                }
                else
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachImage({pNombre}, actionList{nombreProp},\"{prefijoYPropiedad}\", principal, this.GetExtension({pNombre}), null, null, null);");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}principal = false;");
            }
        }

        /// <summary>
        /// Se encarga de pintar el método que añadirá imagenes a un recurso
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        public void AnadirImagenes(ElementoOntologia pEntidad)
        {
            List<Propiedad> listapropimage = ObtenerPropiedadesImagen(pEntidad);
            if (!listentidades.Any() && listapropimage.Any())
            {
                string tipo;
                if (esPrimaria)
                {
                    tipo = "ComplexOntologyResource";

                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)} void AddImages({tipo} pResource)");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}super.AddImages(pResource);");

                    if (listapropimage != null && listapropimage.Count > 0)
                    {
                        foreach (Propiedad propiedad in listapropimage)
                        {
                            if (propiedad.ValorUnico)
                            {
                                string nombre = $"this.{ UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(propiedad.Nombre)}";
                                AnadirImagenesSimples(propiedad, pEntidad, nombre);
                            }
                            else
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(propiedad.Nombre)}!=null){{");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for(String prop : this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(propiedad.Nombre)})");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
                                AnadirImagenesMultiples(propiedad, pEntidad, "prop");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                            }
                        }
                    }
                    AnadirImagenesEntidades();
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                }
            }
        }

        /// <summary>
        /// Se encarga de pintar la parte de la clase que añadirá archivos a un recurso
        /// </summary>
        /// <param name="pObjeto">Propiedad que tiene el archivo a adjuntar</param>
        /// <param name="pNombre">Nombre de la variable que tiene el archivo que se va a adjuntar</param>
        public void AnadirArchivosSimples(ObjetoPropiedad pObjeto, string pNombre)
        {
            string idioma = "";
            if (pObjeto.Multiidioma)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for (LanguageEnum lan : LanguajeEnum.values()){{");
                idioma = "[lan]";
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}File file{pNombre.Substring(pNombre.LastIndexOf(".") + 1)} = new File(\"{pNombre}{idioma}\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}long length{pNombre.Substring(pNombre.LastIndexOf(".") + 1)} = file{pNombre.Substring(pNombre.LastIndexOf(".") + 1)}.length();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}FileReader reader = new FileReader({pNombre.Substring(pNombre.LastIndexOf(".") + 1)})");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}char[] data{pNombre.Substring(pNombre.LastIndexOf(".") + 1)} = new char[(int) length{pNombre.Substring(pNombre.LastIndexOf(".") + 1)}())]");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}reader.read(data{pNombre.Substring(pNombre.LastIndexOf(".") + 1)}, 0, (int) length{pNombre.Substring(pNombre.LastIndexOf(".") + 1)}());");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String name{pNombre.Substring(pNombre.LastIndexOf(".") + 1)} = {pNombre.Substring(pNombre.LastIndexOf(".") + 1)}{idioma};");

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}pResource.AttachFile(data{pNombre.Substring(pNombre.LastIndexOf(".") + 1)}, \"{pObjeto.NombrePropiedad}\", name{pNombre.Substring(pNombre.LastIndexOf(".") + 1)});");
            if (pObjeto.Multiidioma)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            }
        }

        /// <summary>
        /// Pinta el método que se utiliza para añadir archivos a las entidades
        /// </summary>        
        public void AnadirArchivosEntidades()
        {
            foreach (Propiedad prop in listentidades)
            {
                if (!prop.Rango.Equals("object"))
                {
                    if (prop.ValorUnico)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre)}.AddFiles(pResource);");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre)}!=null){{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for({UtilCadenasOntology.ObtenerNombrePropJava(prop.Rango)} prop : {UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombrePropJava(prop.Nombre)})");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}prop.AddFiles(pResource);");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    }
                }
            }
        }

        /// <summary>
        /// Pinta el método que se utilizará para añadir imágenes al recurso pasado por parámetro
        /// </summary>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        public void AnadirArchivos(ElementoOntologia pEntidad)
        {
            List<Propiedad> propTipoArchivo = ObtenerPropiedadesArchivo(pEntidad);
            if (propTipoArchivo.Any())
            {
                string tipo;
                if (esPrimaria)
                {
                    tipo = "ComplexOntologyResource";
                }
                else
                {
                    tipo = "SecondaryResource";
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public void AddFiles({tipo} pResource) throws IOException");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}super.AddFiles(pResource);");

                foreach (Propiedad propiedad in propTipoArchivo)
                {
                    ObjetoPropiedad objeto = listaObjetosPropiedad.FirstOrDefault(x => x.NombrePropiedad.Equals(propiedad.Nombre) && x.NombreEntidad.Equals(propiedad.ElementoOntologia.TipoEntidad));
                    string prefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, objeto.NombrePropiedad));
                    string nombrePropiedad = UtilCadenasOntology.ObtenerNombrePropJava(objeto.NombrePropiedad);
                    if (!objeto.Multivaluada)
                    {
                        string nombre = $"{ prefijoPropiedad}_{nombrePropiedad}";
                        AnadirArchivosSimples(objeto, nombre);
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(this.{prefijoPropiedad}_{nombrePropiedad}!=null){{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for(String prop : this.{prefijoPropiedad}_{nombrePropiedad})");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
                        AnadirArchivosSimples(objeto, "prop");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    }
                }
                AnadirArchivosEntidades();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
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
        /// Pinta el método que se encarga de transformar una variable Date a Calendar
        /// </summary>
        public void ToCalendar()
        {
            Clase.AppendLine();
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public static Calendar toCalendar(Date pDate){{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}Calendar cal = Calendar.getInstance();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}cal.setTime(pDate);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return cal;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            Clase.AppendLine();
        }

        /// <summary>
        /// Pinta el método que se utilizará para generar el fichero con los triples utilizados para cargar el grafo de búsqueda con la carga masiva.
        /// </summary>
        /// <param name="pEsPrimaria">Si la ontología que se esta pintando es o no principal</param>
        /// <param name="pEntidad">Entidad para la cual se estan pintando las clases</param>
        /// <param name="pRdfType">Nombre de la ontología que se esta pintando</param>
        /// <param name="pListaPropiedadesSearch">Lista de propiedes que se han configurado para añadir al triple Search</param>
        /// <param name="pListaPadrePropiedadesAnidadas">Lista de propiedades anidadas que se han configurado para añadir al triple Search</param>
        /// <param name="pListaFacetaObjetoConocimientoProyecto">Lista de facetas del proyecto para el cual se estan pintando las clases</param>
        public void CrearToSearchGraphTriples(bool pEsPrimaria, ElementoOntologia pEntidad, string pRdfType, List<string> pListaPropiedadesSearch, List<string> pListaPadrePropiedadesAnidadas, List<FacetaObjetoConocimientoProyecto> pListaFacetaObjetoConocimientoProyecto)
        {
            if (!ontologia.EntidadesAuxiliares.Any(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public ArrayList<String> ToSearchGraphTriples(ResourceApi resourceAPI)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<String> list = new ArrayList<String>();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}ArrayList<String> listaSearch = new ArrayList<String>();");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(!listaMetakeyword.containsKey(\"{pEntidad.TipoEntidad}\")) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}try {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}HashMap<UUID, ArrayList<MetaKeyword>> hashMapMetakeyword = resourceAPI.GetMetakeywords(resourceID, \"{pRdfType}.owl\");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}ArrayList<MetaKeyword> listaMetakeywordClase = new ArrayList<MetaKeyword>();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}for(ArrayList<MetaKeyword> listMetaKeyword : hashMapMetakeyword.values()) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}for(MetaKeyword metaKeyword : listMetaKeyword){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}if((metaKeyword.getEntidadID() == null || metaKeyword.getEntidadID().equals(\"\")) || metaKeyword.getEntidadID().equals(\"{pEntidad.TipoEntidad}\")) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}listaSearch.add(metaKeyword.getContent());");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(8)}listaMetakeywordClase.add(metaKeyword);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}listaMetakeyword.put(\"{pEntidad.TipoEntidad}\", listaMetakeywordClase);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}} catch (Exception e) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}e.printStackTrace();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ArrayList<MetaKeyword> listaMetakeywordClase = listaMetakeyword.get(\"{pEntidad.TipoEntidad}\");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for(MetaKeyword metaKeyword : listaMetakeywordClase){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}listaSearch.add(metaKeyword.getContent());");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)} try{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)} fechaMin1=sdf1.parse(date1);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)} }} catch (ParseException e) {{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)} e.printStackTrace();}}");
                Clase.AppendLine();

                if (pEsPrimaria)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AgregarTags(list);");
                    if (pEntidad.Padre != null)
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \\\"{pEntidad.Padre.Nombre.ToLower()}\\\" . \"));");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \\\"{pRdfType}\\\" . \"));");
                    }

                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/type> \\\"{pEntidad.TipoEntidad}\\\" . \"));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasfechapublicacion> \" +fechaActual()+\" . \"));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hastipodoc> \\\"5\\\" . \"));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasfechamodificacion> \" +fechaActual()+\" . \"));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasnumeroVisitas>  0 . \"));");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasprivacidadCom> \\\"publico\\\" . \"));");

                    if (EsPropiedadMultiIdioma(nombrePropTituloEntero) || EsPropiedadExternaMultiIdioma(pEntidad, nombrePropTituloEntero))
                    {
                        Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero));
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}for(LanguageEnum idioma : {nombrePropTitulo}.keySet())");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        if (propiedad.ValorUnico)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://xmlns.com/foaf/0.1/firstName> \\\"\").concat(GenerarTextoSinSaltoDeLinea({nombrePropTitulo}.get(idioma))).concat(\"\\\" . \"));");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasnombrecompleto> \\\"\").concat(GenerarTextoSinSaltoDeLinea({nombrePropTitulo}.get(idioma))).concat(\"\\\" . \"));");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://xmlns.com/foaf/0.1/firstName> \\\"\").concat(GenerarTextoSinSaltoDeLinea(String.join(\" ,\", {nombrePropTitulo}.get(idioma)))).concat(\"\\\" . \"));");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasnombrecompleto> \\\"\").concat(GenerarTextoSinSaltoDeLinea(String.join(\" ,\", {nombrePropTitulo}.get(idioma)))).concat(\"\\\" . \"));");
                        }
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://xmlns.com/foaf/0.1/firstName> \\\"\").concat(GenerarTextoSinSaltoDeLinea({nombrePropTitulo})).concat(\"\\\" . \"));");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/hasnombrecompleto> \\\"\").concat(GenerarTextoSinSaltoDeLinea({nombrePropTitulo})).concat(\"\\\" . \"));");
                    }
                }

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String search = \"\";");

                //Recorro las entidades auxiliares
                foreach (ElementoOntologia elem in ontologia.EntidadesAuxiliares)
                {
                    foreach (Propiedad propiedadPadre in listentidadesAux)
                    {
                        if (elem.TipoEntidad.Equals(propiedadPadre.Rango))
                        {
                            PintarEntidadesAuxiliares(elem, propiedadPadre, pEntidad, false, GenerarSujeto(pEntidad, TiposSujeto.Busqueda), "this", pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas, pListaFacetaObjetoConocimientoProyecto);
                        }
                    }
                }

                PintarPropiedades(pEntidad, false, GenerarSujeto(pEntidad, TiposSujeto.Busqueda), null, "this", pListaPropiedadesSearch, pListaPadrePropiedadesAnidadas, pListaFacetaObjetoConocimientoProyecto);

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (listaSearch != null && listaSearch.size() > 0){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}for(String valorSearch : listaSearch){{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}search += valorSearch.replace(\"\\r\\n\", \" \").replace(\"\\n\", \" \").replace(\"\\r\", \" \").replace(\"\\\\\", \"\\\\\\\\\").replace(\"\\\"\", \"\\\\\\\"\").concat(\" \");");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(search != null && !search.isEmpty())");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}list.add(\"<{GenerarSujeto(pEntidad, TiposSujeto.Busqueda)}.concat(\"> <http://gnoss/search> \\\"\").concat(search.toLowerCase()).concat(\"\\\" . \"));");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return list;");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        /// <summary>
        /// Se encarga de pintar un método que se utiliza para limpiar los textos de los triples de saltos de linea
        /// </summary>
        private void CrearTextoSinSaltoDeLinea()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}private String GenerarTextoSinSaltoDeLinea(String pTexto)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return pTexto.replace(\"\\r\\n\", \" \").replace(\"\\n\", \" \").replace(\"\\r\", \" \").replace(\"\\\"\", \"\\\\\\\"\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }

        /// <summary>
        /// Pinta el método que se encarga de devolver un string con la fecha actual
        /// </summary>
        public void EscribirMetodoFecha()
        {
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public String fechaActual(){{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}String timeStamp = new SimpleDateFormat(\"yyyyMMddHHmmss\").format(Calendar.getInstance().getTime());");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return timeStamp;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
        }
    }
}
