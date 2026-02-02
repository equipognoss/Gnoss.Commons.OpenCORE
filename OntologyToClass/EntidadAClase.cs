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
using System.Xml.Linq;

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
        private const string URI_THING = "http://www.w3.org/2002/07/owl#Thing";

        private readonly Constantes constantes = new Constantes();
        public StringBuilder Clase { get; }
        private readonly XmlDocument doc;
        private readonly List<string> listaObjetosExternos;
        private readonly string grafoUrl;
        private readonly string nombreOnto;
        private readonly Ontologia ontologia;
        private string nombrePropTitulo;
        private string nombrePropTituloEntero;
        private string nombrePropDescripcionEntera;
        private string nombrePropDescripcion;
        private readonly bool esPrincipal;
        private bool? mEsMultiIdiomaConfig;
        private int mNumItem;
        private readonly Dictionary<string, string> dicPref;
        private readonly Dictionary<string, PropiedadTipo> propListiedadesTipo = new Dictionary<string, PropiedadTipo>();
        private readonly Dictionary<string, bool> propListiedadesMultidioma = new Dictionary<string, bool>();
        private readonly List<Propiedad> listaEntidades = new List<Propiedad>();
        private readonly List<Propiedad> listentidadesAux = new List<Propiedad>();
        private readonly List<string> listaIdiomas;
        private readonly List<string> nombresOntologia;
        private readonly List<ObjetoPropiedad> listaObjetosPropiedad = new List<ObjetoPropiedad>();
        private readonly Dictionary<Propiedad, bool> listaPropMultiidiomaFalse = new Dictionary<Propiedad, bool>();
        public readonly Guid proyID;
        public readonly string nombreCortoProy;
        private readonly IMassiveOntologyToClass mMassiveOntologyClass;
        private readonly string mRdfType;
        private readonly bool mGenerarClaseConPrefijo;

        /// <summary>
        /// Constructor de la clase , que la inicializa con un stringBuilder
        /// </summary>
        public EntidadAClase(EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, IServicesUtilVirtuosoAndReplication pServicesUtilVirtuosoAndReplication, IMassiveOntologyToClass pMassiveOntologyToClass)
        {
            Clase = new StringBuilder();
            mMassiveOntologyClass = pMassiveOntologyToClass;
            mGenerarClaseConPrefijo = pConfigService.ObtenerClasesGeneradasConPrefijo();
        }

        /// <summary>
        /// Constructor de la clase que necesita uan ontologia, su nombre, y el xml , además del string builder
        /// </summary>
        /// <param name="pOntologia"></param>
        /// <param name="pNombreOnto"></param>
        /// <param name="pContentXML"></param>
        public EntidadAClase(Ontologia pOntologia, string pNombreOnto, string pGrafoUrl, byte[] pContentXML, bool pEsPrincipal, List<string> pListaIdiomas, string pNombreCortoProy, Guid pProyID, List<string> pNombresOntologia, List<ObjetoPropiedad> pListaObjetosPropiedad, EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, IServicesUtilVirtuosoAndReplication pServicesUtilVirtuosoAndReplication, IMassiveOntologyToClass pMassiveOntologyToClass, string pRdfType, Dictionary<string, string> pDicPref)
        {
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            Clase = new StringBuilder();
            ontologia = pOntologia;
            nombreOnto = pNombreOnto;
            grafoUrl = pGrafoUrl;
            listaObjetosPropiedad = pListaObjetosPropiedad;
            esPrincipal = pEsPrincipal;
            dicPref = pDicPref;
            listaIdiomas = pListaIdiomas;
            nombresOntologia = pNombresOntologia;
            doc = new XmlDocument();
            if (pContentXML != null)
            {
                MemoryStream ms = new MemoryStream(pContentXML);
                doc.Load(ms);
            }
            mNumItem = 0;
            listaObjetosExternos = new List<string>();
            mMassiveOntologyClass = pMassiveOntologyToClass;
            mRdfType = pRdfType;
            mGenerarClaseConPrefijo = pConfigService.ObtenerClasesGeneradasConPrefijo();
        }

        /// <summary>
        /// Creamos la clase a partir de la entidad 
        /// </summary>
        /// <param name="pEntidad"></param>
        /// <returns></returns>
        public string GenerarClase(ElementoOntologia pEntidad, List<string> pListaPropiedesSearch, List<string> pListaPadrePropiedadesAnidadas)
        {
            ObtenerUsings(pEntidad);
            Clase.AppendLine($"{constantes.namespac} {nombreOnto}");
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
            mMassiveOntologyClass.CrearToAcidData(esPrincipal, pEntidad, ontologia, Clase, nombrePropDescripcion, nombrePropTitulo, nombrePropTituloEntero, nombrePropDescripcionEntera, propListiedadesMultidioma, listaObjetosPropiedad, listaPropMultiidiomaFalse);
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
                    if (listaEspefPropiedad != null && listaEspefPropiedad.Count > 1)
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

                    if (listaconfig != null && listaconfig.Count > 1)
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
                    if (listaespef != null && listaespef.Count > 1)
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
            if (ontologia.EntidadesAuxiliares.Contains(pEntidad) || !EntidadEsPadre(pEntidad))
            {
                EstablecerClaseBaseSiTieneHerencia(pEntidad);
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{constantes.publicClass} {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)} : GnossOCBase");
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
            if (EntidadEsPadre(pEntidad))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{constantes.publicClass} {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)} : GnossOCBase");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(1)}{constantes.publicClass} {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)} : {UtilCadenasOntology.ObtenerNombreClase(pEntidad.Superclases[0], dicPref, mGenerarClaseConPrefijo)}");
            }
        }

        /// <summary>
        /// Creamos las propiedades de cada clase, si en la ontología la propiedad está dentro del Dominio
        /// </summary>
        /// <param name="pEntidad"></param>
        public void CreacionDePropiedades(ElementoOntologia pEntidad)
        {
            bool entidadEsPadre = EntidadEsPadre(pEntidad);
            string modificadorSobrescritura = "virtual";

            if (!entidadEsPadre)
            {
                modificadorSobrescritura = "override";
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificadorSobrescritura} string RdfType {{ get {{ return \"{pEntidad.TipoEntidad}\"; }} }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificadorSobrescritura} string RdfsLabel {{ get {{ return \"{pEntidad.TipoEntidad}\"; }} }}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificadorSobrescritura} string OntologyURL {{ get {{ return \"{grafoUrl}Ontologia/{nombreOnto.Replace("Ontology", "").ToLower()}.owl\"; }} }}");
            Clase.AppendLine();

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
            if (!entidadEsPadre)
            {
                padre = ontologia.Entidades.Find(ent => ent.TipoEntidad.Equals(pEntidad.Superclases[0]));
            }

            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (EsMultiIdiomaConfig && propiedad.Rango.ToLower().Equals("http://www.w3.org/2001/XMLSchema#string") && !propListiedadesMultidioma.ContainsKey($"{pEntidad.TipoEntidadGeneracionClases}|{propiedad.Nombre}"))
                {
                    propListiedadesMultidioma.Add($"{pEntidad.TipoEntidadGeneracionClases}|{propiedad.Nombre}", true);
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
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}[LABEL(LanguageEnum.{pLan},\"{dicLan[pLan].Replace("\"", "\\\"")}\")]");
                        }
                        if (!propMultiidioma)
                        {
                            break;
                        }
                    }
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}[RDFProperty(\"{propiedad.Nombre.Replace("\"", "\\\"")}\")]");
                    if (propiedad.Tipo.ToString().Equals("ObjectProperty"))
                    {
                        if (ObtenerUsingExternos(propiedad).Count == 0 && !ontologia.Entidades.Exists(x => x.TipoEntidad.Equals(propiedad.Rango)) && !EsPersonaGnoss(propiedad))
                        {
                            throw new ExcepcionGeneral($"la propiedad {propiedad.Nombre} de la entidad {propiedad.ElementoOntologia.TipoEntidad} no esta configurado correctamente. Revisa que el grafo del selector de entidad y la definición de la propiedad sea correcta.");
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
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.required}");
                }
                if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
                {
                    useNew = true;
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} {(useNew ? "new" : "")} {UtilCadenasOntology.ObtenerNombreClase(pPropiedad.Rango, dicPref, mGenerarClaseConPrefijo)} {nombrePropiedad}  {constantes.getSet} ");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} string Id{nombrePropiedad}  {constantes.getSet} ");
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
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.maxLeng}({pPropiedad.CardinalidadMaxima})]");
                }
                if (!pPropiedad.CardinalidadMinima.Equals(0))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.minLeng}({pPropiedad.CardinalidadMinima})]");
                }
                if (pPadre != null && pPadre.Propiedades.Exists(propiedad => propiedad.Nombre.Equals(pPropiedad.Nombre)))
                {
                    usarNew = true;
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} {(usarNew ? "new" : "")} List<{UtilCadenasOntology.ObtenerNombreClase(pPropiedad.Rango, dicPref, mGenerarClaseConPrefijo)}> {nombrePropiedad} {constantes.getSet}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} List<string> Ids{nombrePropiedad} {constantes.getSet}");
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
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} {(usarNew ? "new" : "")} {pRango} {nombrePropiedad} {constantes.getSet}");
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
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.maxLeng}({pPropiedad.CardinalidadMaxima})]");
            }
            if (!pPropiedad.CardinalidadMinima.Equals(0))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.minLeng}({pPropiedad.CardinalidadMinima})]");
            }
            if (pRango.Contains("Dictionary<"))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} {(usarNew ? "new" : "")} {pRango} {nombrePropiedad} {constantes.getSet}");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{constantes.publicSolo} {(usarNew ? "new" : "")} List<{pRango}> {nombrePropiedad} {constantes.getSet}");
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
                listaEntidades.Add(pPropiedad);
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
            //Se comprueba si el rango de la propiedad apunta a a una entidad de la misma ontología, que no sea una entidad del mismo tipo o con herencia de del mismo padre
            if (ontologia.Entidades.Exists(x => x.TipoEntidad.Equals(pPropiedad.Rango)) && !pPropiedad.Rango.Equals(ontologia.ObtenerEntidadPrincipal().TipoEntidad) && !RangoPropiedadApuntaHermana(pPropiedad.Rango, pPadre))
            {
                CreacionPropiedadObjetosEntidad(pPropiedad, UtilCadenasOntology.ObtenerNombreClase(pPropiedad.Rango, dicPref, mGenerarClaseConPrefijo), UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre), pPadre);
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
                        }
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
                            propListiedadesMultidioma.Add($"{pEntidad.TipoEntidadGeneracionClases}|{pPropiedad.Nombre}", GetMultiIdiomaPropiedad(pEntidad, pPropiedad));
                        }
                    }
                    else if (cadena)
                    {
                        propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.IntSimple);
                    }
                    if (mMassiveOntologyClass.EsPropiedadMultiIdioma(pPropiedad.Nombre, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases))
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
                            propListiedadesMultidioma.Add($"{pEntidad.TipoEntidadGeneracionClases}|{pPropiedad.Nombre}", GetMultiIdiomaPropiedad(pEntidad, pPropiedad));
                        }
                    }
                    else if (cadena)
                    {
                        propListiedadesTipo.Add(pPropiedad.Nombre, PropiedadTipo.IntMultiple);
                    }
                    if (mMassiveOntologyClass.EsPropiedadMultiIdioma(pPropiedad.Nombre, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases))
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
        public string ObtenerRangoDePropiedad(Propiedad pPropiedad, bool pEsUsing)
        {
            string clase = string.Empty;

            if (EspefEntidad != null)
            {
                XmlNode nodo = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/Grafo")?[0];
                if (nodo != null)
                {
                    string nombre = nodo.InnerText.Substring(0, nodo.InnerText.LastIndexOf("."));
                    if (nombresOntologia.Exists(n => n.Equals(nombre, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        XmlNode node = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/PropsEntLectura/Propiedad/@EntidadID")?[0];
                        if (node != null)
                        {
                            if (pEsUsing)
                            {
                                clase = UtilCadenasOntology.ObtenerNombreClase(node.InnerText, dicPref, mGenerarClaseConPrefijo);
                            }
                            else
                            {
                                clase = node.InnerText;
                            }
                        }
                        else
                        {
                            clase = CompletarTipoSeleccion(pPropiedad, pEsUsing);
                        }
                    }
                    else
                    {
                        clase = CompletarTipoSeleccion(pPropiedad, pEsUsing);
                    }
                }
                else
                {
                    clase = CompletarTipoSeleccion(pPropiedad, pEsUsing);

                }
                return clase;
            }
            else
            {
                return "object";
            }
        }

        public string CompletarTipoSeleccion(Propiedad pPropiedad, bool pUsing)
        {
            string clase = "object";

            XmlNodeList tipoSeleccion = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/TipoSeleccion");
            if (tipoSeleccion != null && tipoSeleccion.Count > 0 && tipoSeleccion[0].InnerText.Equals("Tesauro"))
            {
                if (pUsing)
                {
                    if (mGenerarClaseConPrefijo)
                    {
                        clase = "Skos_Concept";
                    }
                    else
                    {
                        clase = "Concept";
                    }
                }
                else
                {
                    clase = "http://www.w3.org/2008/05/skos#Concept";
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
                nombrePropTitulo = $"this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, nodoTitulo.InnerText))}_{UtilCadenasOntology.ObtenerNombreProp(nodoTitulo.InnerText)}";
            }
            if (esPrincipal)
            {
                XmlNode nodoDescripcion = ConfiguracionGeneral.SelectSingleNode("DescripcionDoc");
                if (nodoDescripcion != null)
                {
                    nombrePropDescripcionEntera = nodoDescripcion.InnerText;
                    nombrePropDescripcion = $"this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, nodoDescripcion.InnerText))}_{UtilCadenasOntology.ObtenerNombreProp(nodoDescripcion.InnerText)}";
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
            string graf = string.Empty;

            if (EspefEntidad != null)
            {
                XmlNode nodeGrafo = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/Grafo");

                if (nodeGrafo != null)
                {
                    string nombreGrafoCorrecto = nombresOntologia.Find(n => n.Equals(nodeGrafo.InnerText.Replace(".owl", ""), StringComparison.InvariantCultureIgnoreCase));
                    if (!string.IsNullOrEmpty(nombreGrafoCorrecto))
                    {
                        graf = UtilCadenas.PrimerCaracterAMayuscula(nombreGrafoCorrecto).Split('.')[0] + "Ontology";
                        usings.Add($"using {ObtenerRangoDePropiedad(pPropiedad, true)} = {graf}.{ObtenerRangoDePropiedad(pPropiedad, true)};");
                    }

                    XmlNode nodeEntidadSolicitada = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/UrlTipoEntSolicitada");

                    if (nodeEntidadSolicitada != null)
                    {
                        string[] urlEntidadesSolicitadas = nodeEntidadSolicitada.InnerText.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string urlEntidad in urlEntidadesSolicitadas)
                        {
                            string nombrePropiedad = UtilCadenasOntology.ObtenerNombreClase(urlEntidad, dicPref, mGenerarClaseConPrefijo);
                            usings.Add($"using {nombrePropiedad} = {graf}.{nombrePropiedad};");
                        }
                    }
                }
            }

            return usings;
        }

        public bool EsPersonaGnoss(Propiedad pPropiedad)
        {
            if (EspefPropiedad != null)
            {
                XmlNode nodeGrafo = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/SeleccionEntidad/TipoSeleccion");
                if (nodeGrafo != null && nodeGrafo.InnerText.Equals(PERSONA_GNOSS))
                {
                    return true;
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
            if (!ontologia.EntidadesAuxiliares.Exists(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
            {
                string tipoOntologyResource = "ComplexOntologyResource";
                string modificador = "virtual";
                if (!EntidadEsPadre(pEntidad))
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
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {modificador} {tipoOntologyResource} ToGnossApiResource(ResourceApi resourceAPI)");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return ToGnossApiResource(resourceAPI, new List<string>());");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                    Clase.AppendLine("");
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
                if (listaEntidades.Any() || !EntidadEsPadre(pEntidad))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetEntities();");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetProperties();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(idrecurso.Equals(Guid.Empty) && idarticulo.Equals(Guid.Empty))");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ontology = new Ontology(resourceAPI.GraphsUrl, OntologyURL, RdfType, RdfsLabel, prefList, propList, entList);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}ontology = new Ontology(resourceAPI.GraphsUrl, OntologyURL, RdfType, RdfsLabel, prefList, propList, entList,idrecurso,idarticulo);");
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
                if (!EntidadEsPadre(pEntidad))
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetEntities();");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GetProperties();");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SecondaryOntology ontology = new SecondaryOntology(resourceAPI.GraphsUrl, OntologyURL, \"{pEntidad.TipoEntidad}\", \"{pEntidad.TipoEntidad}\", prefList, propList,identificador,listSecondaryEntity, {(listaEntidades.Any() ? "entList" : "null")});");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.SecondaryOntology = ontology;");
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddImages(resource);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}AddFiles(resource);");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}return resource;");
        }

        public void CrearGetURI(ElementoOntologia pEntidad)
        {
            if (!ontologia.EntidadesAuxiliares.Exists(entidadNombre => entidadNombre.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
            {
                string modificador = "override";

                if (!EntidadEsPadre(pEntidad))
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

            if (pEntidad.Propiedades.Exists(item => item.NombreConNamespace.Equals(listaPropiedadesAnidadas[pContadorLlamadas])))
            {
                ElementoOntologia claseRepresentaPropiedad = pEntidad.EntidadesRelacionadas.Find(item => item.TipoEntidad.Equals(UtilCadenas.PrimerCaracterAMayuscula(pPropiedad.TipoEntidadRepresenta)));
                Propiedad propiedadHija = claseRepresentaPropiedad.Propiedades.Find(item => item.NombreConNamespace.Equals(listaPropiedadesAnidadas[pContadorLlamadas]));

                string nombreVariableActual = $"{pNombreVariableEntidadActual}.{UtilCadenas.PrimerCaracterAMayuscula(listaPropiedadesAnidadas[pContadorLlamadas]).Replace(":", "_")}";
                if (propiedadHija.ValorUnico)
                {
                    if (mMassiveOntologyClass.EsPropiedadMultiIdioma(propiedadHija.Nombre, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases) || mMassiveOntologyClass.EsPropiedadExternaMultiIdioma(claseRepresentaPropiedad, propiedadHija, listaObjetosPropiedad))
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}foreach (LanguageEnum idioma in {nombreVariableActual}.Keys)");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}search += \"{{idioma.ToString().Replace('_','-')}} \"");
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
                    if (mMassiveOntologyClass.EsPropiedadMultiIdioma(propiedadHija.Nombre, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases) || mMassiveOntologyClass.EsPropiedadExternaMultiIdioma(claseRepresentaPropiedad, propiedadHija, listaObjetosPropiedad))
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
            if ((!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count) && EntidadEsPadre(pEntidad))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal void AddResourceTitle(ComplexOntologyResource resource)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                if (esPrincipal)
                {
                    Propiedad propiedad = pEntidad.Propiedades.First(item => item.NombreFormatoUri.Equals(nombrePropTituloEntero));
                    if (mMassiveOntologyClass.EsPropiedadMultiIdioma(nombrePropTituloEntero, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases))
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}List<Multilanguage> multiTitleList = new List<Multilanguage>();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach (LanguageEnum idioma in {nombrePropTitulo}.Keys)");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        if (propiedad.ValorUnico)
                        {

                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}multiTitleList.Add(new Multilanguage({nombrePropTitulo}[idioma], idioma.ToString().Replace('_','-')));");
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
                        if (UtilCadenasOntology.ObtenerNombreProp(propiedad.Rango).Equals("date") || UtilCadenasOntology.ObtenerNombreProp(propiedad.Rango).Equals("dateTime"))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Title = {nombrePropTitulo}.ToString(\"yyyyMMddHHmmss\");");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Title = {nombrePropTitulo};");
                        }
                    }
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
        }

        public void AgregarDescripcionRecurso(ElementoOntologia pEntidad)
        {
            if ((!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count) && EntidadEsPadre(pEntidad) && !string.IsNullOrEmpty(this.nombrePropDescripcionEntera))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}internal void AddResourceDescription(ComplexOntologyResource resource)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Propiedad propiedad = pEntidad.Propiedades.Find(item => item.Nombre.Equals(nombrePropDescripcionEntera));
                if (propiedad != null)
                {
                    if (mMassiveOntologyClass.EsPropiedadMultiIdioma(this.nombrePropDescripcionEntera, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases))
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}List<Multilanguage> listMultilanguageDescription = new List<Multilanguage>();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}foreach (LanguageEnum idioma in {nombrePropDescripcion}.Keys)");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                        if (propiedad.ValorUnico)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}listMultilanguageDescription.Add(new Multilanguage({nombrePropDescripcion}[idioma], idioma.ToString().Replace('_','-')));");
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
                        if (UtilCadenasOntology.ObtenerNombreProp(propiedad.Rango).Equals("date") || UtilCadenasOntology.ObtenerNombreProp(propiedad.Rango).Equals("dateTime"))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Description = {nombrePropDescripcion}.ToString(\"yyyyMMddHHmmss\");");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}resource.Description = {nombrePropDescripcion};");
                        }
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
                    string prefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Key));
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
                        if (mMassiveOntologyClass.EsPropiedadMultiIdioma(propiedad.Key, propListiedadesMultidioma, pEntidad.TipoEntidadGeneracionClases))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux} != null)");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (LanguageEnum idioma in {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}.Keys)");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}propList.Add(new {tipoPropiedad}(\"{ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)}\", {thi}{id}{prefijoPropiedad}_{nombrePropiedad}{aux}[idioma], idioma.ToString().Replace('_','-')));");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                            Propiedad propiedadCompleta = pEntidad.Propiedades.Find(item => item.Nombre.Equals(propiedad.Key));
                            if (propiedadCompleta != null && (propiedadCompleta.CardinalidadMinima > 0 || propiedadCompleta.FunctionalProperty))
                            {
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}else");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}throw new GnossAPIException($\"La propiedad {ObtenerPrefijoYPropiedad(dicPref, propiedad.Key)} debe tener al menos un valor en el recurso: {{resourceID}}\");");
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
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
                string rango = UtilCadenasOntology.ObtenerNombreClase(prop.Rango, dicPref, mGenerarClaseConPrefijo);
                if (!rango.Equals("object"))
                {
                    string obtenerPrefijo = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre));
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
                                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{obtenerPrefijo}_{nombreProp}.Entity = entity{obtenerPrefijo}_{nombreProp};");
                            }
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}entList.Add(entity{obtenerPrefijo}_{nombreProp});");
                        }
                    }
                    else
                    {
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({obtenerPrefijo}_{nombreProp}!=null){{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach({rango} prop in {obtenerPrefijo}_{nombreProp}){{");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.GetProperties();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.GetEntities();");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}OntologyEntity entity{rango} = new OntologyEntity(\"{prop.Rango}\", \"{prop.Rango}\", \"{ObtenerPrefijoYPropiedad(dicPref, prop.Nombre)}\", prop.propList, prop.entList);");
                        Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}entList.Add(entity{rango});");
                        if (ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(prop.Rango)))
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}prop.Entity = entity{rango};");
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
            ObjetoPropiedad objeto = listaObjetosPropiedad.Find(x => x.NombrePropiedad.Equals(pPropiedad.Nombre) && x.NombreEntidad.Equals(pEntidad.TipoEntidad) && x.NombreOntologia.ToLower().Equals(mRdfType.ToLower()));
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
            return $"{UtilCadenasOntology.ObtenerPrefijo(pDicPref, pRang)}:{UtilCadenasOntology.ObtenerNombrePropSinNamespace(pRang)}";
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

                        Propiedad propiedadImagen = pEntidad.Propiedades.Find(propiedad => propiedad.Nombre.Equals(attrID.Value));
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
            if (EspefPropiedad != null && (EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"Archivo\"]/@ID") != null || EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"ArchivoLink\"]/@ID") != null))
            {
                XmlNodeList archivos = EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"Archivo\"]/@ID");
                XmlNodeList archivosLink = EspefPropiedad.SelectNodes($"Propiedad[@EntidadID=\"{pEntidad.TipoEntidad}\"][TipoCampo=\"ArchivoLink\"]/@ID");

                if (archivos != null)
                {
                    foreach (XmlAttribute archivo in archivos)
                    {
                        Propiedad propiedadArchivo = pEntidad.Propiedades.Find(propiedad => propiedad.Nombre.Equals(archivo.Value));

                        if (propiedadArchivo != null)
                        {
                            propListEntidadesArchivos.Add(propiedadArchivo);
                        }
                    }
                }

                if (archivosLink != null)
                {
                    foreach (XmlAttribute archivo in archivosLink)
                    {
                        Propiedad propiedadArchivo = pEntidad.Propiedades.Find(propiedad => propiedad.Nombre.Equals(archivo.Value));

                        if (propiedadArchivo != null)
                        {
                            propListEntidadesArchivos.Add(propiedadArchivo);
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
                            if (EntidadEsPadre(pEntidad) || propiedad.Dominio.Contains(pEntidad.TipoEntidad))
                            {
                                if (propiedad.ValorUnico)
                                {
                                    string nombre = $"this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre)}";
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (!string.IsNullOrEmpty({nombre}))");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                                    AnadirImagenesSimples(propiedad, pEntidad, nombre);
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                                }
                                else
                                {
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre)} != null)");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach(string prop in this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(propiedad.Nombre)})");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
                                    AnadirImagenesMultiples(propiedad, pEntidad, "prop");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
                                }
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
            foreach (List<ConfiguracionAccionImagen> listaAcciones in ImageAction(pPropiedad).Values)
            {
                foreach (ConfiguracionAccionImagen accion in listaAcciones)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}actionList{nombreProp}.Add(new ImageAction({accion.Ancho},{accion.Alto}, {accion.Tipo}, 100));");
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
            foreach (List<ConfiguracionAccionImagen> listaAcciones in ImageAction(propiedad).Values)
            {
                foreach (ConfiguracionAccionImagen accion in listaAcciones)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}actionList{nombreProp}.Add(new ImageAction({accion.Ancho},{accion.Alto}, {accion.Tipo}, 100));");
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
                    ObjetoPropiedad objeto = listaObjetosPropiedad.Find(x => x.NombrePropiedad.Equals(propiedad.Nombre) && x.NombreEntidad.Equals(propiedad.ElementoOntologia.TipoEntidad));

                    if (objeto != null)
                    {
                        string prefijoPropiedad = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, objeto.NombrePropiedad));
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
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}using (FileStream file{nombre.Substring(nombre.LastIndexOf(".") + 1)} = File.Open({nombre}{idioma}, FileMode.Open, FileAccess.Read))");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}long length{nombre.Substring(nombre.LastIndexOf(".") + 1)} = file{nombre.Substring(nombre.LastIndexOf(".") + 1)}.Length;");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}byte[] data{nombre.Substring(nombre.LastIndexOf(".") + 1)} = new byte[length{nombre.Substring(nombre.LastIndexOf(".") + 1)}];");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}file{nombre.Substring(nombre.LastIndexOf(".") + 1)}.Read(data{nombre.Substring(nombre.LastIndexOf(".") + 1)}, 0, Convert.ToInt32(length{nombre.Substring(nombre.LastIndexOf(".") + 1)}));");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}string name{nombre.Substring(nombre.LastIndexOf(".") + 1)} = {nombre.Substring(nombre.LastIndexOf(".") + 1)}{idioma};");
            if (objeto.Tipo.Equals(ObjetoPropiedad.TipoObjeto.ArchivoLink))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachDownloadableFile(data{nombre.Substring(nombre.LastIndexOf(".") + 1)}, \"{UtilCadenasOntology.ObtenerPrefijo(dicPref, objeto.NombrePropiedad)}:{UtilCadenasOntology.ObtenerNombreProp(objeto.NombrePropiedad)}\", name{nombre.Substring(nombre.LastIndexOf(".") + 1)});");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}pResource.AttachFile(data{nombre.Substring(nombre.LastIndexOf(".") + 1)}, \"{UtilCadenasOntology.ObtenerPrefijo(dicPref, objeto.NombrePropiedad)}:{UtilCadenasOntology.ObtenerNombreProp(objeto.NombrePropiedad)}\", name{nombre.Substring(nombre.LastIndexOf(".") + 1)});");
            }
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");

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
                foreach (Propiedad prop in listaEntidades)
                {
                    if (!prop.Rango.Equals("object") && !propiedadesYaAnadidas.Contains(prop.Nombre))
                    {
                        if (prop.ValorUnico)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}.AddImages(pResource);");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)} != null)");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach ({UtilCadenasOntology.ObtenerNombreProp(prop.Rango)} prop in {UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)})");
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
                foreach (Propiedad prop in listaEntidades)
                {
                    if (!prop.Rango.Equals("object") && !propYaAnadidas.Contains(prop.Nombre))
                    {
                        if (prop.ValorUnico)
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}this.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}.AddFiles(pResource);");
                        }
                        else
                        {
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if({UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)} != null)");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
                            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach({UtilCadenasOntology.ObtenerNombreClase(prop.Rango, dicPref, mGenerarClaseConPrefijo)} prop in {UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)})");
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
        /// Obtiene todas las configuraciones de las redimensiones para las propiedades de tipo Imagen en el XML del objeto de conocimiento.
        /// </summary>
        /// <param name="propiedad">Propiedad de la cual queremos obtener las configuraciones de la generación de redimensiones</param>
        /// <returns></returns>
        public Dictionary<string, List<ConfiguracionAccionImagen>> ImageAction(Propiedad propiedad)
        {
            Dictionary<string, List<ConfiguracionAccionImagen>> accionesImagen = new Dictionary<string, List<ConfiguracionAccionImagen>>();
            List<ConfiguracionAccionImagen> listaAcciones = new List<ConfiguracionAccionImagen>();

            XmlNodeList accionesXML = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{propiedad.Nombre}\" and @EntidadID=\"{propiedad.ElementoOntologia}\"]/ImgMiniVP");

            if (accionesXML.Count > 0)
            {
                XmlNode acc = accionesXML.Item(0);
                if (acc != null)
                {
                    foreach (XmlElement size in acc.SelectNodes("Size"))
                    {
                        listaAcciones.Add(ObtenerAccionImagenConfigurada(size));
                    }
                    accionesImagen.Add(propiedad.Nombre, listaAcciones);
                }
            }
            return accionesImagen;
        }

        /// <summary>
        /// Obtiene la configuracion de la accion indicada en el XML para generar una redimensión de la imagen configurada.
        /// </summary>
        /// <param name="pSeccionSize">Sección del xml obtenida a partir del tag Size</param>
        /// <returns></returns>
        private ConfiguracionAccionImagen ObtenerAccionImagenConfigurada(XmlElement pSeccionSize)
        {
            ConfiguracionAccionImagen configuracionAccionImagen = new ConfiguracionAccionImagen();
            switch (pSeccionSize.GetAttribute("Tipo"))
            {
                case "RecorteCuadrado":
                    configuracionAccionImagen.Tipo = "ImageTransformationType.Crop";
                    break;
                default:
                    configuracionAccionImagen.Tipo = "ImageTransformationType.ResizeToWidth";
                    break;
            }

            XmlNodeList alto = pSeccionSize.GetElementsByTagName("Alto");
            if (alto?.Count > 0)
            {
                int auxAlto = 0;
                int.TryParse(alto[0].InnerText, out auxAlto);
                configuracionAccionImagen.Alto = auxAlto;
            }

            XmlNodeList ancho = pSeccionSize.GetElementsByTagName("Ancho");
            if (ancho?.Count > 0)
            {
                int auxAncho = 0;
                int.TryParse(ancho[0].InnerText, out auxAncho);
                configuracionAccionImagen.Ancho = auxAncho;
            }

            return configuracionAccionImagen;
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
            if (propListImagen.Exists(prop2 => prop2.Nombre.Equals(propiedad)))
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
            if (propListArchivo.Exists(prop2 => prop2.Nombre.Equals(propiedad)))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Genera un constructor vacío para las entidades principales, para las secundarias genera el constructor solicitandole la URI.
        /// Si hay herencia en las secundarias pasa la URI a la clase padre.
        /// </summary>
        /// <param name="pEntidad"></param>
        public void ConstructorSencillo(ElementoOntologia pEntidad)
        {
            if (esPrincipal)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}() : base() {{ }} ");
            }
            else if (!EntidadEsPadre(pEntidad))
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}(string pIdentificador) : base(pIdentificador)");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}(string pIdentificador) : base()");
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
            bool entidadEsPadre = EntidadEsPadre(pEntidad);

            if (esPrincipal && (!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) || ontologia.EntidadesAuxiliares.Count == ontologia.Entidades.Count))
            {
                if (entidadEsPadre)
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()");
                }
                else
                {
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}(SemanticResourceModel pSemCmsModel, LanguageEnum idiomaUsuario) : base(pSemCmsModel,idiomaUsuario)");
                }
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}GNOSSID = pSemCmsModel.RootEntities[0].Entity.Uri;");
                RellenarConstructorInverso(pEntidad);
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}}}");
                Clase.AppendLine();
            }

            if (entidadEsPadre)
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base()");
            }
            else
            {
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(2)}public {UtilCadenasOntology.ObtenerNombreClase(pEntidad.TipoEntidad, dicPref, mGenerarClaseConPrefijo)}(SemanticEntityModel pSemCmsModel, LanguageEnum idiomaUsuario) : base(pSemCmsModel,idiomaUsuario)");
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
                    pPropiedad.Rango = ObtenerRangoDePropiedad(pPropiedad, false);
                }
                if (!pPropiedad.Rango.Equals("object"))
                {
                    ElementoOntologia padre = null;
                    if (!EntidadEsPadre(pEntidad))
                    {
                        padre = ontologia.Entidades.Find(ent => ent.TipoEntidad.Equals(pEntidad.Superclases[0]));
                    }
                    if (padre == null || !padre.Propiedades.Exists(x => x.Nombre.Equals(pPropiedad.Nombre)))
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
            string[] entidadesSolicitadas;

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{prefijoPropiedad}_{nombreProp} = new List<{UtilCadenasOntology.ObtenerNombreClase(pPropiedad.Rango, dicPref, mGenerarClaseConPrefijo).Trim()}>();");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombreProp} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if(prop{prefijoPropiedad}_{nombreProp} != null && prop{prefijoPropiedad}_{nombreProp}.PropertyValues.Count > 0)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}foreach (SemanticPropertyModel.PropertyValue propValue in prop{prefijoPropiedad}_{nombreProp}.PropertyValues)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}if(propValue.RelatedEntity!=null){{");

            XmlNode urlEntidadExterna = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/UrlTipoEntSolicitada")?[0];

            if (urlEntidadExterna != null)
            {
                entidadesSolicitadas = urlEntidadExterna.InnerText.Split(',', StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                entidadesSolicitadas = new string[] { pPropiedad.Rango };
            }

            bool primeraIteracion = true;
            foreach (string entidad in entidadesSolicitadas)
            {
                string nombreRango = UtilCadenasOntology.ObtenerNombreClase(entidad, dicPref, mGenerarClaseConPrefijo);

                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{(primeraIteracion ? "if" : "else if")}(IsPropertyOfType(\"{entidad.Trim().Trim('\r').Trim('\n')}\", pSemCmsModel, propValue.RelatedEntity.Entity.TipoEntidad))");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}{nombreRango} {UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre)}_{nombreProp} = new {nombreRango}(propValue.RelatedEntity,idiomaUsuario);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(7)}{prefijoPropiedad}_{nombreProp}.Add({UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre)}_{nombreProp});");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}}}");
                primeraIteracion = false;
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
        }


        public void ConstructorInversoPropiedadObjetoValorUnico(Propiedad pPropiedad)
        {
            string prefijoPropiedad = ObtenerPrefijoPropiedad(pPropiedad).Trim();
            string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre).Trim();

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}SemanticPropertyModel prop{prefijoPropiedad}_{nombrePropiedad} = pSemCmsModel.GetPropertyByPath(\"{pPropiedad.Nombre}\");");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}if (prop{prefijoPropiedad}_{nombrePropiedad} != null && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues.Count > 0 && prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues[0].RelatedEntity != null)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}{{");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}switch(prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues[0].RelatedEntity.Entity.TipoEntidad)");
            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}{{");
            XmlNode nodo = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/UrlTipoEntSolicitada")?[0];
            if (nodo != null)
            {
                string[] entidadesSolicitadas = nodo.InnerText.Split(',', StringSplitOptions.RemoveEmptyEntries);

                foreach (string entidad in entidadesSolicitadas)
                {
                    string nombreRango = UtilCadenasOntology.ObtenerNombreClase(entidad, dicPref, mGenerarClaseConPrefijo).Trim();
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}case \"{entidad}\":");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}{prefijoPropiedad}_{nombrePropiedad} = new {nombreRango}(prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues[0].RelatedEntity,idiomaUsuario);");
                    Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}break;");
                }
            }
            else
            {
                string nombreRango = UtilCadenasOntology.ObtenerNombreClase(pPropiedad.Rango, dicPref, mGenerarClaseConPrefijo).Trim();
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}case \"{pPropiedad.Rango}\":");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{prefijoPropiedad}_{nombrePropiedad} = new {nombreRango}(prop{prefijoPropiedad}_{nombrePropiedad}.PropertyValues[0].RelatedEntity,idiomaUsuario);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}break;");
            }

            Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
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
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}if(DateTime.TryParseExact(propValue.Value, formatosFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fecha))");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}{{");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(6)}this.{prefijoPropiedad}_{nombrePropiedad}.Add(fecha);");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(5)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(4)}}}");
                Clase.AppendLine($"{UtilCadenasOntology.Tabs(3)}}}");
            }
        }

        public bool GuardarFechaComoEntero(Propiedad pPropiedad)
        {
            XmlNode elementos = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]");
            if (elementos != null && elementos.SelectNodes("GuardarFechaComoEntero").Count > 0)
            {
                return true;
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
            string pref = UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre).Trim();
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

        /// <summary>
        /// Dado el rango de una propiedad y su padre, comprueba que el rango apunte a uno de los hijos de mi padre (Ambas entidades heredan del mismo elemento). 
        /// </summary>
        /// <param name="pRango">Rango de la propiedad a comprobar</param>
        /// <param name="pPadre">Elemento de la ontología padre de la propiedad a comprobar. En caso de no tener herencia será null</param>
        /// <returns>True o false en función de si el rango apunta a una propiedad con el mismo padre (hermana)</returns>
        private bool RangoPropiedadApuntaHermana(string pRango, ElementoOntologia pPadre)
        {
            return pPadre != null && pPadre.Subclases.Exists(hijo => hijo == pRango);
        }

        /// <summary>
        /// Nos indica si la entidad pasada por parámetro es una entidad padre (no hereda de ninguna otra que no sea Thing)
        /// </summary>
        /// <param name="pEntidad">Entidad que queremos comprobar si es padre o no</param>
        /// <returns>True o False en función de si la entidad es padre o no</returns>
        private bool EntidadEsPadre(ElementoOntologia pEntidad)
        {
            return pEntidad.Superclases == null || pEntidad.Superclases.Count == 0 || pEntidad.Superclases.Exists(item => item.Contains(URI_THING));
        }
    }
}