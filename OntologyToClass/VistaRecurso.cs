using Es.Riam.Gnoss.Util.GeneradorClases;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.GeneradorClases;
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
    public class VistaRecurso
    {
        private XmlDocument doc;
        string nombreOWL;
        byte[] contentXML;
        bool tipoOntologia;
        string nombreProyecto;
        Ontologia ontologia;
        Dictionary<string, string> dicPref;
        private bool? mEsMultiIdiomaConfig;
        StringBuilder Vista { get; set; }
        string carpetaPadre;
        string directorio;
        public readonly Guid proyID;
        public readonly string nombreCortoProy;
        List<Propiedad> propiedadesExternas = new List<Propiedad>();
        List<ObjetoPropiedad> listaObjetosPropiedad = new List<ObjetoPropiedad>();
        Dictionary<string, List<PropiedadesAMostrar>> diccionarioPropiedades = new Dictionary<string, List<PropiedadesAMostrar>>();
        private LoggingService mLoggingService;
        public VistaRecurso(string nombreOWL, Ontologia contentOWL, byte[] contentXML, bool tipoOntologia, string nombreProyecto, string carpetaPadre, string directorio, string pNombreCortoProy, Guid pProyID, Dictionary<string, string> dicPref, List<ObjetoPropiedad> listaObjetosPropiedad, Dictionary<string, List<PropiedadesAMostrar>> diccionarioPropiedades, LoggingService loggingService)
        {
            nombreCortoProy = pNombreCortoProy;
            proyID = pProyID;
            this.nombreOWL = nombreOWL;
            this.diccionarioPropiedades = diccionarioPropiedades;
            this.listaObjetosPropiedad = listaObjetosPropiedad;
            this.directorio = directorio;
            this.contentXML = contentXML;
            this.tipoOntologia = tipoOntologia;
            this.ontologia = contentOWL;
            this.dicPref = dicPref;
            this.Vista = new StringBuilder();
            this.propiedadesExternas = new List<Propiedad>();
            this.nombreProyecto = nombreProyecto;
            this.carpetaPadre = carpetaPadre;
            mLoggingService = loggingService;
            this.doc = new XmlDocument();
            if (contentXML != null)
            {
                MemoryStream ms = new MemoryStream(contentXML);
                doc.Load(ms);
            }
            //XmlDocument doc = new XmlDocument();
            //string xml = Encoding.UTF8.GetString(contentXML);
            //doc.LoadXml(xml);
            //ontologia.LeerOntologia();
        }

        public void EscribirVista()
        {
            string[] nombOnt = nombreOWL.Split('\\');
            string nombreOnt = nombOnt[nombOnt.Length - 1];
            string nombreOnto = UtilCadenas.PrimerCaracterAMayuscula(nombreOnt.Split('.')[0]);
            string ruta = Path.Combine(directorio, carpetaPadre, "VistasGeneradas");
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
            File.WriteAllText(Path.Combine(ruta, $"{nombreOnto}.cshtml"), CrearVistas());
        }

        public string CrearVistas()
        {
            ElementoOntologia entidadPrincipal = ObtenerEntidadPrincipal();
            EscribirConfiguración(entidadPrincipal);
            EscribirHTML(entidadPrincipal, false);
            EscribirPintarFunction();
            EscribirPintarHelpersExternos();
            Vista.AppendLine("}");
            return Vista.ToString();
        }

        private XmlNode mEspefPropiedad = null;
        private XmlNode mConfigGeneral = null;
        private XmlNode mEspefEntidad = null;
        public XmlNode EspefPropiedad
        {
            get
            {
                if (mEspefPropiedad == null)
                {
                    XmlNodeList listaEspefPropiedad = doc.DocumentElement.SelectNodes("EspefPropiedad");

                    if (listaEspefPropiedad.Count > 1)
                    {
                        foreach (XmlNode espef in listaEspefPropiedad)
                        {
                            if (espef.Attributes.Count > 0 && ((espef.Attributes["ProyectoID"] != null && espef.Attributes["ProyectoID"].InnerText.ToLower().Equals(proyID.ToString().Replace("{", "\"").Replace("}", "\""))) || (espef.Attributes["Proyecto"] != null && espef.Attributes["Proyecto"].InnerText.ToLower().Equals(nombreCortoProy))))
                            {
                                mEspefPropiedad = espef;
                                return mEspefPropiedad;
                            }
                        }
                        foreach (XmlNode espef in listaEspefPropiedad)
                        {
                            if (espef.Attributes.Count == 0)
                            {
                                mEspefPropiedad = espef;
                                return mEspefPropiedad;
                            }
                        }
                    }
                    mEspefPropiedad = listaEspefPropiedad.Item(0);
                }
                return mEspefPropiedad;
            }
        }

        public XmlNode ConfiguracionGeneral
        {
            get
            {
                if (mConfigGeneral == null)
                {
                    XmlNodeList listaconfig = doc.DocumentElement.SelectNodes("ConfiguracionGeneral");

                    if (listaconfig.Count > 1)
                    {
                        foreach (XmlNode config in listaconfig)
                        {
                            if (config.Attributes.Count > 0 && ((config.Attributes["ProyectoID"] != null && config.Attributes["ProyectoID"].InnerText.ToLower().Equals(proyID)) || (config.Attributes["Proyecto"] != null && config.Attributes["Proyecto"].InnerText.ToLower().Equals(nombreCortoProy))))
                            {
                                mConfigGeneral = config;
                                return mConfigGeneral;
                            }
                        }
                        foreach (XmlNode config in listaconfig)
                        {
                            if (config.Attributes.Count == 0)
                            {
                                mConfigGeneral = config;
                                return mConfigGeneral;
                            }
                        }
                    }
                    mConfigGeneral = listaconfig.Item(0);
                }
                return mConfigGeneral;
            }
        }

        public XmlNode EspefEntidad
        {
            get
            {
                if (mEspefEntidad == null)
                {
                    XmlNodeList listaespef = doc.DocumentElement.SelectNodes("EspefEntidad");

                    if (listaespef.Count > 0)
                    {
                        foreach (XmlNode espef in listaespef)
                        {


                            if (espef.Attributes.Count > 0 && ((espef.Attributes["ProyectoID"] != null && espef.Attributes["ProyectoID"].InnerText.ToLower().Equals(proyID)) || (espef.Attributes["Proyecto"] != null && espef.Attributes["Proyecto"].InnerText.ToLower().Equals(nombreCortoProy))))
                            {
                                mEspefEntidad = espef;
                                return espef;
                            }
                        }
                        foreach (XmlNode espef in listaespef)
                        {
                            if (espef.Attributes.Count < 1)
                            {
                                mEspefEntidad = espef;
                                return espef;
                            }
                        }
                    }
                    mEspefEntidad = listaespef.Item(0);
                }
                return mEspefEntidad;
            }
        }

        public bool EsMultiIdiomaConfig
        {
            get
            {
                if (!mEsMultiIdiomaConfig.HasValue)
                {
                    if (ConfiguracionGeneral.SelectSingleNode("MultiIdioma") != null && ConfiguracionGeneral.SelectSingleNode("MultiIdioma").InnerText.Equals("false"))
                    {
                        mEsMultiIdiomaConfig = false;
                    }
                    else
                    {
                        mEsMultiIdiomaConfig = true;
                    }
                }
                return mEsMultiIdiomaConfig.Value;
            }
        }

        public bool GetMultiIdiomaPropiedad(ElementoOntologia pEntidad, Propiedad pPropiedad)
        {
            if (EsMultiIdiomaConfig)
            {

                XmlNode elementoPropiedad = EspefPropiedad.SelectSingleNode($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pEntidad.TipoEntidad}\"]");
                if (elementoPropiedad != null)
                {
                    if (elementoPropiedad.SelectNodes("MultiIdioma").Count == 0 && pPropiedad.Rango.Equals("http://www.w3.org/2001/XMLSchema#string"))
                    {
                        if (elementoPropiedad.SelectNodes("TipoCampo").Count == 0 || elementoPropiedad.SelectSingleNode("TipoCampo").InnerText.Equals("TextArea") || elementoPropiedad.SelectSingleNode("TipoCampo").InnerText.Equals("TextEditor"))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void EscribirUsingsRelacionados(ElementoOntologia pEntidad)
        {
            List<string> ontologias = new List<string>();
            XmlNodeList propiedad = EspefPropiedad.SelectNodes($"Propiedad/SeleccionEntidad/Grafo");

            foreach (XmlNode nodo in propiedad)
            {
                ontologias.Add(nodo.InnerText);
            }
            foreach (string nombreontologia in ontologias)
            {
                string nombreonto = nombreontologia.Substring(0, nombreontologia.LastIndexOf('.'));
                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@using { UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerNombreProp(nombreonto))}Ontology;");
            }
        }

        public void EscribirConfiguración(ElementoOntologia pEntidad)
        {
            //  Vista.AppendLine($"@*[security ||| {nombreOWL.ToLower()} ||| larioja] *@");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@using {UtilCadenas.PrimerCaracterAMayuscula(nombreOWL.Split('.')[0].Substring(nombreOWL.Split('.')[0].LastIndexOf("\\") + 1))}Ontology;");
            EscribirUsingsRelacionados(pEntidad);
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@using GnossBase;");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@model ResourceViewModel");
            Vista.AppendLine($@"@{{");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}ResourceModel FichaDocumento = Model.Resource;");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}CommunityModel Comunidad = Html.GetComunidad();");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}UserIdentityModel IdentidadActual = Html.GetIdentidadActual();");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}Html.AddBodyClass("" {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}Model "");");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}if (FichaDocumento.ItemLinkedFotoVersion != 0 && !string.IsNullOrEmpty(FichaDocumento.ItemLinked.ToString()) && FichaDocumento.ItemLinked != Guid.Empty)");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}{{");

            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}string urlPersonalizacion = $""{{Html.GetBaseUrlContent()}}/{{Es.Riam.Util.UtilArchivos.ContentOntologias}}/Archivos/{{FichaDocumento.ItemLinked.ToString().Substring(0, 3)}}"";");


            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}Html.GetListaJS().Add($""{{urlPersonalizacion}}/{{FichaDocumento.ItemLinked.ToString()}}.js?v= {{FichaDocumento.ItemLinkedFotoVersion}}"");");

            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}Html.GetListaCSS().Add($""{{urlPersonalizacion}}/{{FichaDocumento.ItemLinked.ToString()}}.css?v= {{FichaDocumento.ItemLinkedFotoVersion}}"");");

            //Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}Html.GetListaCSS().Add(Html.GetBaseUrlContent() + "" / "" + Es.Riam.Util.UtilArchivos.ContentOntologias + "" / Archivos / "" + FichaDocumento.ItemLinked.ToString().Substring(0, 3) + "" / "" + FichaDocumento.ItemLinked.ToString() + "".css ? v = "" + FichaDocumento.ItemLinkedFotoVersion);");

            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}}}");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}SemanticResourceModel semCmsModel = Model.SemanticFrom;");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)} GnossOCBase.LanguageEnum idiomaUsuario = (GnossOCBase.LanguageEnum)Enum.Parse(typeof(GnossOCBase.LanguageEnum), Html.GetUtilIdiomas().LanguageCode.ToLower());");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)} p{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)} = new {UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}(semCmsModel,idiomaUsuario);");
            // Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}GnossOCBase.LanguageEnum idiomaUsuario = GnossOCBase.LanguageEnum.es;");
            Vista.AppendLine($@"}}");
        }

        public ElementoOntologia ObtenerEntidadPrincipal()
        {
            foreach (ElementoOntologia pEntidad in this.ontologia.Entidades)
            {
                if (ontologia.Entidades.Count == ontologia.EntidadesAuxiliares.Count)
                {
                    return pEntidad;
                }
                if (!ontologia.EntidadesAuxiliares.Exists(entidad => entidad.TipoEntidad.Equals(pEntidad.TipoEntidad)) && !pEntidad.Superclases.Any(s => !s.Contains("Thing")))
                {
                    return pEntidad;
                }
            }
            return null;
        }

        public Dictionary<Propiedad, Dictionary<string, string>> AtrNombreLectura(ElementoOntologia pEntidad)
        {
            Dictionary<Propiedad, Dictionary<string, string>> diccionarioAttributos = new Dictionary<Propiedad, Dictionary<string, string>>();
            Dictionary<string, string> diccionarioIdioma;
            foreach (Propiedad pPropiedad in pEntidad.Propiedades)
            {
                diccionarioIdioma = new Dictionary<string, string>();
                XmlNodeList atributosLectura = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pEntidad.TipoEntidad}\"]/AtrNombreLectura");
                foreach (XmlElement lectura in atributosLectura)
                {
                    foreach (XmlAttribute atribu in lectura.Attributes)
                    {
                        if (atribu.Name.Equals("xml:lang"))
                        {
                            if (!diccionarioIdioma.ContainsKey(atribu.InnerText))
                            {
                                diccionarioIdioma.Add(atribu.InnerText, lectura.InnerText);
                            }
                        }
                    }
                }
                diccionarioAttributos.Add(pPropiedad, diccionarioIdioma);
            }
            return diccionarioAttributos;
        }

        public void EscribirContenido(XmlNode pOrdenEntidad, ElementoOntologia pEntidad)
        {
            List<Propiedad> listaPropiedades = pEntidad.Propiedades;
            if (pOrdenEntidad != null)
            {
                XmlNodeList grupos = pOrdenEntidad.ChildNodes;
                if (grupos.Count > 0)
                {
                    foreach (XmlNode grupo in grupos)
                    {
                        if (grupo is XmlElement)
                        {
                            if (grupo.Name.Equals("Grupo"))
                            {
                                if (grupo.Attributes[0].Name.Equals("classLectura"))
                                {
                                    Vista.AppendLine($"<div class=\"{grupo.Attributes[0].Value}\">");
                                    EscribirContenido(grupo, pEntidad);
                                    Vista.AppendLine($"</div>");
                                }
                                else if (grupo.Attributes[0].Name.Equals("Tipo"))
                                {
                                    string prop = grupo.InnerText.ToString();
                                    Propiedad propiedad = listaPropiedades.Find(x => x.Nombre.Equals(prop));
                                    if (propiedad.Tipo.ToString().Equals("DatatypeProperty"))
                                    {
                                        if (grupo.Attributes[0].Name.Equals("Tipo"))
                                        {
                                            if (grupo.Attributes[0].Value.Equals("titulo"))
                                            {
                                                Vista.AppendLine($"{UtilCadenasOntology.Tabs(2)}<h2>");
                                                EscribirPropiedad(propiedad);
                                                Vista.AppendLine($"</h2>");

                                            }
                                            else if (grupo.Attributes[0].Value.Equals("subtitulo"))
                                            {
                                                Vista.AppendLine($"{UtilCadenasOntology.Tabs(2)}<h3>");
                                                EscribirPropiedad(propiedad);
                                                Vista.AppendLine($"</h3>");
                                            }
                                            else
                                            {
                                                EscribirPropiedad(propiedad);
                                            }
                                        }
                                        else
                                        {
                                            EscribirPropiedad(propiedad);
                                        }
                                    }
                                    else if (propiedad.Tipo.ToString().Equals("ObjectProperty"))
                                    {
                                        EscribirAuxiliar(propiedad);
                                    }
                                }
                            }
                        }
                        if (grupo.Name.Equals("NameProp"))
                        {

                            string prop = grupo.InnerText.ToString();
                            Propiedad propiedad = listaPropiedades.Find(x => x.Nombre.Equals(prop));
                            if (propiedad.Tipo.ToString().Equals("DatatypeProperty"))
                            {
                                EscribirPropiedad(propiedad);
                            }
                            else if (propiedad.Tipo.ToString().Equals("ObjectProperty"))
                            {
                                EscribirAuxiliar(propiedad);
                            }
                        }
                    }
                }
            }
        }

        public void EscribirAuxiliar(Propiedad pPropiedad)
        {
            Vista.AppendLine($"Pintar{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango))}({UtilCadenasOntology.ObtenerNombreProp(pPropiedad.ElementoOntologia.TipoEntidad)}.{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)},idiomaUsuario);");
        }
        /*public void EscribirEntidades(ElementoOntologia pEntidad, bool esHelper)
        {

            XmlNodeList entidades = EspefEntidad.SelectNodes($"Entidad[@ID=\"{pEntidad.TipoEntidad}\"]");

            if (entidades != null)
            {
                if (entidades.Count > 1)
                {
                    foreach (XmlNode entidad in entidades)
                    {
                        if (entidad.Attributes.Count > 1 && ((entidad.Attributes["ProyectoID"] != null && entidad.Attributes["ProyectoID"].InnerText.ToLower().Equals(proyID.ToString().Replace("{", "\"").Replace("}", "\""))) || (entidad.Attributes["Proyecto"] != null && entidad.Attributes["Proyecto"].InnerText.ToLower().Equals(nombreCortoProy))))
                        {
                            XmlNode ordenEntidad = entidad.SelectSingleNode($"OrdenEntidadLectura");
                            EscribirContenido(ordenEntidad, pEntidad);
                        }
                    }
                }
                else
                {
                    XmlNode ordenEntidad = entidades[0].SelectSingleNode($"OrdenEntidadLectura");
                    EscribirContenido(ordenEntidad, pEntidad);
                }

            }
        }*/

        public void EscribirPropiedad(Propiedad pPropiedad)
        {
            Vista.AppendLine($"<span class=\"value\" about=\"@semCmsModel.RootEntities.First().AboutRDFA\" property=\"{UtilCadenasOntology.ObtenerPrefijo(ontologia.NamespacesDefinidos, pPropiedad.Nombre, mLoggingService)}:{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre))}\"> @{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.ElementoOntologia.TipoEntidad).ToLower()}.{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)} </span>");
        }

        public void EscribirHTML(ElementoOntologia pEntidad, bool esHelper)
        {
            Dictionary<Propiedad, Dictionary<string, string>> dicAtributos = AtrNombreLectura(pEntidad);
            bool esTesauroSemantico = pEntidad.EsEntidadPathTesSemantico;
            if (!esHelper)
            {
                Vista.AppendLine($"<div class=\"row\">");
                Vista.AppendLine($"<div class=\"col01 col col-12 col-lg-9\">");

                //               EscribirEntidades(pEntidad, esHelper);
            }
            Vista.AppendLine($"<div typeof=\"{ pEntidad.TipoEntidad}:{ UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad).ToLower()}\">");


            XmlNode entidad = EspefEntidad.SelectSingleNode($"Entidad[@ID=\"{pEntidad.TipoEntidad}\"]");

            if (entidad != null)
            {
                XmlNode orden = entidad.SelectSingleNode("OrdenEntidadLectura");
                if (orden == null)
                {
                    orden = entidad.SelectSingleNode("OrdenEntidad");
                }

                if (orden != null)
                {

                    ProcesarNodo(orden.ChildNodes, dicAtributos, pEntidad);
                    Vista.AppendLine();
                }

                if (!esHelper)
                {
                    Vista.AppendLine($"</div>");
                    Vista.AppendLine($"</div>");
                }
                Vista.AppendLine($"</div>");
                Vista.AppendLine();
            }
        }

        private void ProcesarNodo(XmlNodeList nodos, Dictionary<Propiedad, Dictionary<string, string>> pDicAtributos, ElementoOntologia pEntidad)
        {
            foreach (XmlNode nodo in nodos)
            {
                if (nodo.Name.Equals("Grupo"))
                {
                    if (nodo.Attributes.GetNamedItem("class") != null)
                    {
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}<div class=""{nodo.Attributes.GetNamedItem("class").Value}"">");
                        ProcesarNodo(nodo.ChildNodes, pDicAtributos, pEntidad);
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}</div>");
                    }
                    else if (nodo.Attributes.GetNamedItem("classLectura") != null)
                    {
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}<div class=""{nodo.Attributes.GetNamedItem("classLectura").Value}"">");
                        ProcesarNodo(nodo.ChildNodes, pDicAtributos, pEntidad);
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}</div>");
                    }
                    else if (nodo.Attributes.GetNamedItem("Tipo") != null)
                    {
                        if (nodo.Attributes[0].Value.Equals("titulo"))
                        {
                            XmlNode titulo = nodo.SelectSingleNode("NameProp");
                            if (titulo != null)
                            {
                                Propiedad prop = pEntidad.Propiedades.Find(x => x.Nombre.Equals(titulo.InnerText));
                                if (prop != null)
                                {
                                    Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}<h2>@p{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)/*.ToLower()*/}.{ObtenerPrefijoPropiedad(prop)}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}</h2>");
                                }
                            }
                        }
                        else if (nodo.Attributes[0].Value.Equals("subtitulo"))
                        {
                            XmlNode titulo = nodo.SelectSingleNode("NameProp");
                            if (titulo != null)
                            {
                                Propiedad prop = pEntidad.Propiedades.Find(x => x.Nombre.Equals(titulo.InnerText));
                                if (prop != null)
                                {
                                    Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}<h3>@p{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)/*.ToLower()*/}.{ObtenerPrefijoPropiedad(prop)}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}</h3>");
                                }
                            }
                        }
                    }
                }
                else if (nodo.Name.Equals("NameProp"))
                {
                    EscribirElementos(nodo, pDicAtributos, pEntidad);
                }
            }
        }

        public List<Propiedad> ObtenerPropiedadesObjectProperty(ElementoOntologia pEntidad)
        {
            List<Propiedad> listaPropiedades = new List<Propiedad>();
            foreach (Propiedad propiedad in pEntidad.Propiedades)
            {
                if (propiedad.Tipo.ToString().Equals("ObjectProperty"))
                {
                    listaPropiedades.Add(propiedad);
                }
            }
            return listaPropiedades;
        }

        //public void EscribirPintarHelpers()
        //{
        //    foreach (ElementoOntologia pEntidad in ontologia.Entidades)
        //    {
        //        if (!pEntidad.TipoEntidad.Equals(ObtenerEntidadPrincipal().TipoEntidad))
        //        {
        //            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@helper Pintar{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}({UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)} p{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)},GnossOCBase.LanguageEnum idiomaUsuario){{");
        //            EscribirHTML(pEntidad, true);
        //            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}}}");
        //        }
        //    }
        //}

        public void EscribirPintarFunction()
        {
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@functions");
            Vista.AppendLine("{");
            foreach (ElementoOntologia pEntidad in ontologia.Entidades)
            {
                if (!pEntidad.TipoEntidad.Equals(ObtenerEntidadPrincipal().TipoEntidad))
                {
                    Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)} void Pintar{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)}({UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)} p{UtilCadenasOntology.ObtenerNombreProp(pEntidad.TipoEntidad)},GnossOCBase.LanguageEnum idiomaUsuario, Observation pObservation){{");
                    EscribirHTML(pEntidad, true);
                    Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}}}");
                }
            }
            Vista.AppendLine("}");
        }

        private void EscribirPintarHelpersExternos()
        {
            StringBuilder backup = new StringBuilder();
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(0)}@functions");
            Vista.AppendLine("{");
            foreach (Propiedad prop in propiedadesExternas)
            {

                ObjetoPropiedad objetoPropiedad = listaObjetosPropiedad.FirstOrDefault(x => x.NombreEntidad.Equals(prop.ElementoOntologia.TipoEntidad) && x.NombrePropiedad.Equals(prop.Nombre));
                string rango = UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad.Rango);
                if (!backup.ToString().Contains($"void Pintar{rango}({rango} p{rango}, GnossOCBase.LanguageEnum idiomaUsuario")) 
                {
                    backup.AppendLine("void Pintar" + rango + "(" + rango + " p" + rango + ", GnossOCBase.LanguageEnum idiomaUsuario) {");
                    Vista.AppendLine("void Pintar" + rango + "(" + rango + " p" + rango + ", GnossOCBase.LanguageEnum idiomaUsuario) {");
                    XmlNodeList propiedades = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{prop.Nombre}\" and @EntidadID=\"{prop.ElementoOntologia}\"]/SeleccionEntidad/PropsEntLectura/Propiedad");
                    Vista.AppendLine();
                    foreach (XmlNode propiedad in propiedades)
                    {
                        PintarObjetoExterno(propiedad, objetoPropiedad, rango);
                    }
                    Vista.AppendLine("}");
                }
            }
            Vista.AppendLine("}");
        }

        public void PintarObjetoExterno(XmlNode propiedad, ObjetoPropiedad objetoPropiedad, string pNombrePadre)
        {
            string idioma = "";

            string nombrePrefijo = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, propiedad.Attributes[0].InnerText, mLoggingService));
            string nombrePropiedad = UtilCadenasOntology.ObtenerNombreProp(propiedad.Attributes[0].InnerText);
            string nombreEntidad = UtilCadenasOntology.ObtenerNombreProp(propiedad.Attributes[1].InnerText);
            string rango = pNombrePadre;

            ObjetoPropiedad objetoPropiedad1 = listaObjetosPropiedad.FirstOrDefault(x => x.NombreEntidad.Equals(propiedad.Attributes[1].InnerText) && x.NombrePropiedad.Equals(propiedad.Attributes[0].InnerText));
            if (objetoPropiedad1 != null)
            {
                if (objetoPropiedad1.Multiidioma)
                {
                    idioma = "[idiomaUsuario]";
                }
                Vista.AppendLine($"if(p{rango}.{nombrePrefijo}_{nombrePropiedad}{idioma}!=null)");
                Vista.AppendLine("{");
                if (objetoPropiedad1.Rango != "http://www.w3.org/2001/XMLSchema#date" && objetoPropiedad1.Rango != "http://www.w3.org/2001/XMLSchema#string" && objetoPropiedad1.Rango != "http://www.w3.org/2001/XMLSchema#date" && objetoPropiedad1.Rango != "http://www.w3.org/2001/XMLSchema#int")
                {
                    XmlNodeList propiedadesHijas = propiedad.SelectNodes($"SeleccionEntidad/PropsEntLectura/Propiedad");

                    string rangoHijas = UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad1.Rango);
                    string nombrepref = "";
                    if (propiedadesHijas.Count > 0)
                    {
                        nombrepref = UtilCadenas.PrimerCaracterAMayuscula(listaObjetosPropiedad.FirstOrDefault(x => x.NombreEntidad.Equals(objetoPropiedad1.Rango)).NombreOntologia);
                    }
                    else
                    {
                        propiedadesHijas = propiedad.SelectNodes($"Propiedad");
                        if (propiedadesHijas.Count > 0)
                        {
                            nombrepref = UtilCadenas.PrimerCaracterAMayuscula(objetoPropiedad1.NombreOntologia);
                        }
                    }

                    if (propiedadesHijas.Count > 0)
                    {
                        if (!objetoPropiedad1.Multivaluada)
                        {
                            foreach (XmlNode propiedad2 in propiedadesHijas)
                            {
                                PintarObjetoExterno(propiedad2, objetoPropiedad1, $"{rango}.{nombrePrefijo}_{nombrePropiedad}");
                            }
                        }
                        else
                        {
                            string noment = UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad1.NombreEntidad);
                            string nombpref2 = UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, objetoPropiedad1.NombrePropiedad, mLoggingService));
                            string nombprop = UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad1.NombrePropiedad);

                            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}foreach({nombrepref}Ontology.{rangoHijas} p{rangoHijas} in  p{noment}.{nombpref2}_{nombprop})");

                            Vista.AppendLine("{");
                            foreach (XmlNode propiedad3 in propiedadesHijas)
                            {
                                PintarObjetoExterno(propiedad3, objetoPropiedad1, rangoHijas);
                            }
                            Vista.AppendLine("}");
                        }
                    }
                }
                else
                {
                    if (!objetoPropiedad1.Multivaluada)
                    {
                        Vista.AppendLine("<div>");
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}<strong>@p{rango}.GetLabel(nameof(p{rango}.{nombrePrefijo}_{nombrePropiedad}),idiomaUsuario)</strong>");
                        Vista.AppendLine("<span>");
                        string p = "";
                        if (!rango.StartsWith("@"))
                        {
                            p = "p";
                        }
                        pintarTipo(objetoPropiedad1, $"{p}{rango}.GetPropertyURI(nameof(p{rango}.{nombrePrefijo}_{nombrePropiedad}))", $"p{rango}.{nombrePrefijo}_{nombrePropiedad}", idioma);
                        Vista.AppendLine("</span>");
                        Vista.AppendLine("</div>");
                    }
                    else
                    {
                        string nombreOnto = UtilCadenas.PrimerCaracterAMayuscula(objetoPropiedad1.NombreOntologia);
                        string rang = UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad1.Rango);
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}foreach({nombreOnto}Ontology.{rang} p{rang} in  p{UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad1.NombreEntidad)}.{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, objetoPropiedad1.NombrePropiedad, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(objetoPropiedad1.NombrePropiedad)})");
                        Vista.AppendLine("{");
                        Vista.AppendLine("<div>");
                        Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}<strong>@p{rang}.GetLabel(nameof(p{rang}.{nombrePrefijo}_{nombrePropiedad}),idiomaUsuario)</strong>");
                        Vista.AppendLine("<span>");
                        pintarTipo(objetoPropiedad1, $"prop.GetPropertyURI(nameof(p{rang}.{nombrePrefijo}_{nombrePropiedad}))", $"p{rang}.{nombrePrefijo}_{nombrePropiedad} ", idioma);

                        Vista.AppendLine("</span>");
                        Vista.AppendLine("</div>");
                    }
                }
                Vista.AppendLine("}");
            }
        }

        public void pintarTipo(ObjetoPropiedad objeto, string pProperty, string dentro, string idioma)
        {
            switch (objeto.Tipo)
            {
                case ObjetoPropiedad.TipoObjeto.textEditor:
                    Vista.AppendLine($"<p property=\"@{pProperty}\"> @Html.Raw({dentro}{idioma}) </p>");
                    break;
                case ObjetoPropiedad.TipoObjeto.link:
                    Vista.AppendLine($"<a property=\"@{pProperty}\"  href=\"@{dentro}\">@{dentro}{idioma} </a>");
                    break;
                case ObjetoPropiedad.TipoObjeto.image:
                    Vista.AppendLine($"<img property=\"@{pProperty}\" src=\"@Html.GetBaseUrlContent()/@{dentro}{idioma}\"/>");
                    break;
                case ObjetoPropiedad.TipoObjeto.other:
                    Vista.AppendLine($"<p property =\"@{pProperty}\"> @{dentro}{idioma} </p>");
                    break;
            }
        }
        /* public bool TieneTipoCampo(Propiedad pPropiedad)
         {
             XmlNode node = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/TipoCampo")?[0];
             if (node != null)
             {
                 if (!node.InnerText.Equals("Imagen"))
                 {
                     return true;
                 }
             }
             return false;
         }*/
        private string ObtenerPrefijoPropiedad(Propiedad pPropiedad)
        {
            return UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, pPropiedad.Nombre, mLoggingService));
        }

        public void TratarTipoCampo(Propiedad pPropiedad, string pProperty)
        {
            string elemento = "";
            XmlNode node = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/TipoCampo")?[0];
            string nombreEntidad = $"p{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.ElementoOntologia.TipoEntidad)}";
            string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)}";
            string idiomaUsuario = "";

            if (GetMultiIdiomaPropiedad(pPropiedad.ElementoOntologia, pPropiedad))
            {
                idiomaUsuario = "[(int)idiomaUsuario]";
            }
            if (node != null)
            {

                switch (node.InnerText)
                {
                    case "TextEditor":
                        elemento = $"{UtilCadenasOntology.Tabs(2)}<p property=\"{pProperty}\"> @Html.Raw({nombreEntidad}.{nombrePropiedad}{idiomaUsuario}) </p>";
                        break;
                    case "Link":
                        elemento = $"{UtilCadenasOntology.Tabs(2)}<a property=\"{pProperty}\"  href=\"@{nombreEntidad}.{nombrePropiedad}{idiomaUsuario}\">@{nombreEntidad}.GetLabel(nameof({nombreEntidad}.{nombrePropiedad}),idiomaUsuario) </a>";
                        break;
                    case "EmbebedLink":
                        elemento = $"{UtilCadenasOntology.Tabs(2)}<iframe width=\"560\" height=\"315\" src=\"@{nombreEntidad}.{nombrePropiedad}{idiomaUsuario}\"  frameborder=\"0\" allowfullscreen=\"\"> </iframe>";
                        break;
                    case "Imagen":
                        elemento = $"{UtilCadenasOntology.Tabs(2)}<img property=\"{pProperty}\" src=\"@Html.GetBaseUrlContent()/@{nombreEntidad}.{nombrePropiedad}{idiomaUsuario}\" />";
                        break;
                }
            }
            else
            {
                elemento = $"{UtilCadenasOntology.Tabs(2)}<p property=\"{pProperty}\"> @{nombreEntidad}.{nombrePropiedad}{idiomaUsuario} </p>";
            }
            Vista.AppendLine(elemento);
        }

        public bool EsEnlace(Propiedad pPropiedad)
        {
            string nombreRango = $"{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango)}";
            string nombreEntidad = $"{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.ElementoOntologia.TipoEntidad).ToLower()}";
            string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)}";
            XmlNode node = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/TipoCampo")?[0];
            if (node != null)
            {
                if (node.InnerText.Equals("Link"))
                {
                    return true;
                }
            }
            return false;
        }

        public void TratarTipoCampoMultivaluado(Propiedad pPropiedad, string pProperty)
        {
            string elemento = "";
            string nombreRango = $"{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Rango)}";
            string nombreEntidad = $"p{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.ElementoOntologia.TipoEntidad)}";
            string nombrePropiedad = $"{ObtenerPrefijoPropiedad(pPropiedad)}_{UtilCadenasOntology.ObtenerNombreProp(pPropiedad.Nombre)}";
            XmlNode node = EspefPropiedad.SelectNodes($"Propiedad[@ID=\"{pPropiedad.Nombre}\" and @EntidadID=\"{pPropiedad.ElementoOntologia}\"]/TipoCampo")?[0];
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}@foreach({nombreRango} prop in {nombreEntidad}.{nombrePropiedad})");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}{{");
            if (node != null)
            {
                switch (node.InnerText)
                {
                    case "TextEditor":
                        elemento = $"<p property=\"p{pProperty}\"> @Html.Raw(prop) </p>";
                        break;
                    case "Link":
                        elemento = $"<a property=\"p{pProperty}\"  href=\"@prop\">@{nombreEntidad}.GetLabel(nameof(prop),idiomaUsuario) </a>";
                        break;
                    case "EmbebedLink":
                        elemento = $"<iframe width=\"560\" height=\"315\" src=\"@prop\"  frameborder=\"0\" allowfullscreen=\"\"> </iframe>";
                        break;
                    case "Imagen":
                        elemento = $"<img property=\"p{pProperty}\" src=\"@Html.GetBaseUrlContent()/@prop\"/>";
                        break;
                }
            }
            else
            {
                string p = "";
                if (!pProperty.StartsWith("@"))
                {
                    p = "p";
                }
                elemento = $"<p property=\"{p}{pProperty}\"> @prop </p>";
            }
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(3)}{elemento}");
            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}}}");
        }

        public string ObtenerPrefijoYPropiedad(string rang)
        {
            return UtilCadenasOntology.ObtenerPrefijo(ontologia.NamespacesDefinidos, rang, mLoggingService) + ":" + UtilCadenasOntology.ObtenerNombreProp(rang);
        }
        public void EscribirElementos(XmlNode nodo, Dictionary<Propiedad, Dictionary<string, string>> pDicAtributos, ElementoOntologia pEntidad)
        {
            if (nodo.Name.Equals("NameProp"))
            {
                Propiedad prop = pEntidad.Propiedades.Find(x => x.Nombre.Equals(nodo.InnerText));
                List<Propiedad> listaPropiedadesObject = ObtenerPropiedadesObjectProperty(pEntidad);

                if (prop != null)
                {
                    string nombrePropiedad = $"{UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}";
                    string nombreRango = $"{UtilCadenasOntology.ObtenerNombreProp(prop.Rango)}";
                    string nombreEntidad = $"p{UtilCadenasOntology.ObtenerNombreProp(prop.ElementoOntologia.TipoEntidad)}";
                    if (!prop.FunctionalProperty)
                    {
                        if (prop.Rango.Equals("http://www.w3.org/2001/XMLSchema#string") && prop.ValorUnico)
                        {
                            Vista.AppendLine($"@if(!string.IsNullOrEmpty({nombreEntidad}.{nombrePropiedad}))");
                        }
                        else
                        {
                            Vista.AppendLine($"@if({nombreEntidad}.{nombrePropiedad}!=null)");
                        }
                        Vista.AppendLine("{");
                    }
                    Vista.AppendLine("<div>");
                    if (listaPropiedadesObject.Contains(prop))
                    {
                        XmlNodeList externa = EspefPropiedad.SelectNodes($"Propiedad[@ID =\"{prop.Nombre}\" and @EntidadID=\"{prop.ElementoOntologia}\"]/SeleccionEntidad");
                        if (externa.Count > 0)
                        {
                            propiedadesExternas.Add(prop);
                        }
                        if (prop.ValorUnico)
                        {
                            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}Pintar{nombreRango}({nombreEntidad}.{nombrePropiedad},idiomaUsuario);");
                        }
                        else
                        {
                            ElementoOntologia entidadAux = ontologia.Entidades.Find(x => x.TipoEntidad.Equals(prop.Rango.ToString()));
                            if (entidadAux != null && GetMultiIdiomaPropiedad(entidadAux, prop))
                            {
                                if (nombreRango.Equals("string"))
                                {
                                    Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}@foreach({nombreRango} prop in {nombreEntidad}.{nombrePropiedad}[idiomaUsuario])");
                                }
                                else
                                {
                                    Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}@foreach({nombreRango} prop in {nombreEntidad}.{nombrePropiedad})");
                                }
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}{{");
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}Pintar{nombreEntidad}(prop,idiomaUsuario);");
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}}}");
                            }
                            else
                            {
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}@foreach({nombreRango} prop in {nombreEntidad}.{nombrePropiedad})");
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}{{");
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}Pintar{nombreRango}(prop,idiomaUsuario);");
                                Vista.AppendLine($@"{UtilCadenasOntology.Tabs(1)}}}");
                            }
                        }
                    }
                    else
                    {
                        if (!EsEnlace(prop))
                        {
                            Vista.AppendLine($@"{UtilCadenasOntology.Tabs(2)}<strong>@{nombreEntidad}.GetLabel(nameof({nombreEntidad}.{nombrePropiedad}),idiomaUsuario)</strong>");
                        }
                        if (GetMultiIdiomaPropiedad(pEntidad, prop) && nombreRango.Equals("string"))
                        {
                            nombrePropiedad += "[idiomaUsuario]";
                        }

                        string propertyValue = $"@{nombreEntidad}.GetPropertyURI(nameof({nombreEntidad}.{ UtilCadenas.PrimerCaracterAMayuscula(UtilCadenasOntology.ObtenerPrefijo(dicPref, prop.Nombre, mLoggingService))}_{ UtilCadenasOntology.ObtenerNombreProp(prop.Nombre)}))";
                        Vista.AppendLine("<span class=\"values\">");
                        if (prop.ValorUnico)
                        {
                            TratarTipoCampo(prop, propertyValue);
                        }
                        else
                        {
                            TratarTipoCampoMultivaluado(prop, propertyValue);
                        }
                        Vista.AppendLine("</span>");
                    }
                    Vista.AppendLine("</div>");
                    if (!prop.FunctionalProperty)
                    {
                        Vista.AppendLine("}");
                    }
                }
            }
        }
    }
}

