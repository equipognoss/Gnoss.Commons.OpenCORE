using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Xml;
using System.Collections;
using System.Runtime.Serialization;
using System.Linq;

namespace Es.Riam.Semantica.OWL
{
    /// <summary>
    /// Clase que gestiona todo lo relacionado con la lectura y escritura de OWL.
    /// </summary>
    [Serializable]
    public class GestionOWL : ISerializable
    {
        #region Miembros

        #region Estáticos

        /// <summary>
        /// Diccionario que almacena todos los tipos de datos C# en formato XML.
        /// </summary>
        public static Dictionary<string, string> mTipoDatos = new Dictionary<string,string>();

        /// <summary>
        /// Lista de entidades que ya han sido incluidas en el fichero
        /// </summary>
        private static List<string> ListaEntidadesCreadas = new List<string>();

        /// <summary>
        /// Url de la ontología.
        /// </summary>
        protected static string mUrlOntologia = "";

        /// <summary>
        /// Namespace de la ontología.
        /// </summary>
        protected static string mNamespaceOntologia = "";

        /// <summary>
        /// Namespace para la ontología de currículum de Gnoss.
        /// </summary>
        public static string NAMESPACE_ONTO_HR_XML = "hr-xml";

        /// <summary>
        /// Namespace para la ontología de currículum de Gnoss.
        /// </summary>
        public static string NAMESPACE_ONTO_GNOSS = "gnossonto";

        /// <summary>
        /// Namespace para la ontología de licencia Creative Commons.
        /// </summary>
        public static string NAMESPACE_LICENCIA_CC = "cc";

        /// <summary>
        /// URL para la ontología de licencia Creative Commons.
        /// </summary>
        public static string URL_LICENCIA_CC = "http://creativecommons.org/ns#";


        /// <summary>
        /// Fichero de configuración de la BD.
        /// </summary>
        public static string FicheroConfiguracionBD = "";

        #endregion

        #region Constantes

        /// <summary>
        /// Propiedad tipo de RDF.
        /// </summary>
        public const string PropTipoRdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        /// <summary>
        /// Propiedad label de RDF.
        /// </summary>
        public const string PropLabelRdf = "http://www.w3.org/2000/01/rdf-schema#label";

        /// <summary>
        /// Propiedad borrador GNOSS de RDF.
        /// </summary>
        public const string PropBorradorGnossRdf = "http://gnoss/esborrador";

        #endregion

        protected static string mURLIntragnoss = null;

        /// <summary>
        /// Ontología que se está manejando.
        /// </summary>
        private Ontologia mOntologia;

        /// <summary>
        /// Indica que no se permiten nuevos elementos que no estén en la ontología si es distinta de NULL. Contiene los errores en el RDF.
        /// </summary>
        private List<string> mNoPermitirNuevosElementos;

        /// <summary>
        /// Namespaces del RDF que se está leyendo.
        /// </summary>
        private Dictionary<string, string> mNamespacesRDFLeyendo;

        #endregion

        #region Constructor

        public static Dictionary<string, string> TipoDatos
        {
            get
            {
                if (mTipoDatos.Count == 0)
                {
                    //Conversion de tipos de datos C# a tipos de datos OWL

                    mTipoDatos.Add("", "http://www.w3.org/2001/XMLSchema#string"); //El sin rango se interpretará como string
                    mTipoDatos.Add("string", "http://www.w3.org/2001/XMLSchema#string");
                    mTipoDatos.Add("int", "http://www.w3.org/2001/XMLSchema#int");
                    mTipoDatos.Add("integer", "http://www.w3.org/2001/XMLSchema#integer");
                    mTipoDatos.Add("float", "http://www.w3.org/2001/XMLSchema#float");
                    mTipoDatos.Add("boolean", "http://www.w3.org/2001/XMLSchema#boolean");
                    mTipoDatos.Add("bool", "http://www.w3.org/2001/XMLSchema#bool");
                    mTipoDatos.Add("byte", "http://www.w3.org/2001/XMLSchema#byte");
                    mTipoDatos.Add("date", "http://www.w3.org/2001/XMLSchema#date");
                    mTipoDatos.Add("dateTime", "http://www.w3.org/2001/XMLSchema#dateTime");
                    mTipoDatos.Add("long", "http://www.w3.org/2001/XMLSchema#long");
                    mTipoDatos.Add("short", "http://www.w3.org/2001/XMLSchema#short");
                    mTipoDatos.Add("base64Binary", "http://www.w3.org/2001/XMLSchema#base64Binary");
                    mTipoDatos.Add("Literal", "http://www.w3.org/2000/01/rdf-schema#Literal");
                    mTipoDatos.Add("double", "http://www.w3.org/2001/XMLSchema#double");
                    mTipoDatos.Add("nonNegativeInteger", "http://www.w3.org/2001/XMLSchema#nonNegativeInteger");
                    mTipoDatos.Add("gYear", "http://www.w3.org/2001/XMLSchema#gYear");
                    mTipoDatos.Add("gYearMonth", "http://www.w3.org/2001/XMLSchema#gYearMonth");
                    mTipoDatos.Add("positiveInteger", "http://www.w3.org/2001/XMLSchema#positiveInteger");
                    mTipoDatos.Add("time", "http://www.w3.org/2001/XMLSchema#time");
                    mTipoDatos.Add("gMonthDay", "http://www.w3.org/2001/XMLSchema#gMonthDay");
                    mTipoDatos.Add("gDay", "http://www.w3.org/2001/XMLSchema#gDay");
                    mTipoDatos.Add("gMonth", "http://www.w3.org/2001/XMLSchema#gMonth");
                    mTipoDatos.Add("hexBinary", "http://www.w3.org/2001/XMLSchema#hexBinary");
                    mTipoDatos.Add("anyURI", "http://www.w3.org/2001/XMLSchema#anyURI");
                    mTipoDatos.Add("QName", "http://www.w3.org/2001/XMLSchema#QName");
                    mTipoDatos.Add("NOTATION", "http://www.w3.org/2001/XMLSchema#NOTATION");
                    mTipoDatos.Add("decimal", "http://www.w3.org/2001/XMLSchema#decimal");
                    mTipoDatos.Add("normalizedString", "http://www.w3.org/2001/XMLSchema#normalizedString");
                    mTipoDatos.Add("token", "http://www.w3.org/2001/XMLSchema#token");
                    mTipoDatos.Add("nonPositiveInteger", "http://www.w3.org/2001/XMLSchema#nonPositiveInteger");
                    mTipoDatos.Add("language", "http://www.w3.org/2001/XMLSchema#language");
                    mTipoDatos.Add("Name", "http://www.w3.org/2001/XMLSchema#Name");
                    mTipoDatos.Add("NMTOKEN", "http://www.w3.org/2001/XMLSchema#NMTOKEN");
                    mTipoDatos.Add("NCName", "http://www.w3.org/2001/XMLSchema#NCName");
                    mTipoDatos.Add("NMTOKENS", "http://www.w3.org/2001/XMLSchema#NMTOKENS");
                    mTipoDatos.Add("ID", "http://www.w3.org/2001/XMLSchema#ID");
                    mTipoDatos.Add("IDREF", "http://www.w3.org/2001/XMLSchema#IDREF");
                    mTipoDatos.Add("IDREFS", "http://www.w3.org/2001/XMLSchema#IDREFS");
                    mTipoDatos.Add("ENTITY", "http://www.w3.org/2001/XMLSchema#ENTITY");
                    mTipoDatos.Add("ENTITIES", "http://www.w3.org/2001/XMLSchema#ENTITIES");
                    mTipoDatos.Add("negativeInteger", "http://www.w3.org/2001/XMLSchema#negativeInteger");
                    mTipoDatos.Add("unsignedLong", "http://www.w3.org/2001/XMLSchema#unsignedLong");
                    mTipoDatos.Add("unsignedInt", "http://www.w3.org/2001/XMLSchema#unsignedInt");
                    mTipoDatos.Add("unsignedShort", "http://www.w3.org/2001/XMLSchema#unsignedShort");
                    mTipoDatos.Add("unsignedByte", "http://www.w3.org/2001/XMLSchema#unsignedByte");
                    mTipoDatos.Add("Competencia", "Competencia");
                    mTipoDatos.Add("EscalaMetas", "EscalaMetas");
                    mTipoDatos.Add("Estructura", "Estructura");
                    mTipoDatos.Add("Indicador", "Indicador");
                    mTipoDatos.Add("ModoMetrica", "ModoMetrica");
                    mTipoDatos.Add("ModoLibro", "ModoLibro");
                    mTipoDatos.Add("EjeMeta", "EjeMetas");
                    mTipoDatos.Add("FilaMeta", "FilaMeta");
                    mTipoDatos.Add("ElementoModo", "ElementoModo");
                    mTipoDatos.Add("CeldaMeta", "CeldaMeta");
                    mTipoDatos.Add("Nivel", "Nivel");
                    mTipoDatos.Add("Objetivo", "Objetivo");
                    mTipoDatos.Add("Proceso", "Proceso");
                    mTipoDatos.Add("Proyecto", "Proyecto");
                    mTipoDatos.Add("TipoProceso", "TipoProceso");
                    mTipoDatos.Add("MiniLibro", "MiniLibro");
                    mTipoDatos.Add("ElementoEstructura", "ElementoEstructura");
                    mTipoDatos.Add("EntidadNoExportable", "EntidadNoExportable");
                    mTipoDatos.Add("MetaEstructura", "MetaEstructura");
                    mTipoDatos.Add("Norma", "Norma");
                    mTipoDatos.Add("FiguraProfesional", "FiguraProfesional");
                    mTipoDatos.Add("Forma", "Forma");
                    mTipoDatos.Add("PersonaOcupacionFigura", "PersonaOcupacionFigura");
                    mTipoDatos.Add("Dimension", "Dimension");
                    mTipoDatos.Add("TipoDimensionCompetencia", "TipoDimensionCompetencia");
                    mTipoDatos.Add("EntidadExportable", "EntidadExportable");
                    mTipoDatos.Add("Libro", "Libro");
                    mTipoDatos.Add("Metrica", "Metrica");
                    mTipoDatos.Add("SubEstructura", "SubEstructura");
                    mTipoDatos.Add("Salida", "Salida");
                }
                return mTipoDatos;
            }
        }

        /// <summary>
        /// Constructor sin parámetros.
        /// </summary>
        public GestionOWL()
        {
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected GestionOWL(SerializationInfo info, StreamingContext context)
        {
            mNamespaceOntologia = (string)info.GetValue("NamespaceOntologia", typeof(string));
            mNamespacesRDFLeyendo = (Dictionary<string, string>)info.GetValue("NamespacesRDFLeyendo", typeof(Dictionary<string, string>));
            mNoPermitirNuevosElementos = (List<string>)info.GetValue("NoPermitirNuevosElementos", typeof(List<string>));
            mTipoDatos = (Dictionary<string, string>)info.GetValue("TipoDatos", typeof(Dictionary<string, string>));
            mURLIntragnoss = (string)info.GetValue("URLIntragnoss", typeof(string));
            mUrlOntologia = (string)info.GetValue("UrlOntologia", typeof(string));
        }
        
        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve o establece la Url de la ontología.
        /// </summary>
        public string UrlOntologia
        {
            get
            {
                return mUrlOntologia;
            }
            set
            {
                mUrlOntologia = value;
            }
        }

        /// <summary>
        /// Devuelve o establece el namespace de la ontología.
        /// </summary>
        public string NamespaceOntologia
        {
            get
            {
                return mNamespaceOntologia;
            }
            set
            {
                mNamespaceOntologia = value;
            }
        }

        /// <summary>
        /// Obtiene la URL de la intranet de GNOSS
        /// </summary>
        public static string URLIntragnoss
        {
            get
            {
                if (mURLIntragnoss == null)
                {
                    throw new Exception("No se ha definido GestionOWL.URLIntragnoss.");
                }
                return mURLIntragnoss;
            }
            set
            {
                mURLIntragnoss = value;
            }
        }

        /// <summary>
        /// Ontología que se está manejando.
        /// </summary>
        public Ontologia Ontologia
        {
            get
            {
                return mOntologia;
            }
            set
            {
                mOntologia = value;
            }
        }

        /// <summary>
        /// Indica que no se permiten nuevos elementos que no estén en la ontología si es distinta de NULL. Contiene los errores en el RDF.
        /// </summary>
        private List<string> NoPermitirNuevosElementos
        {
            get
            {
                return mNoPermitirNuevosElementos;
            }
            set
            {
                mNoPermitirNuevosElementos = value;
            }
        }

        /// <summary>
        /// Namespaces del RDF que se está leyendo.
        /// </summary>
        public Dictionary<string, string> NamespacesRDFLeyendo
        {
            get
            {
                if (mNamespacesRDFLeyendo == null)
                {
                    mNamespacesRDFLeyendo = new Dictionary<string, string>();
                }

                return mNamespacesRDFLeyendo;
            }
            set
            {
                mNamespacesRDFLeyendo = value;
            }
        }

        #endregion

        #region Métodos generales

        #region Pasar a OWL

        /// <summary>
        /// Obtiene la url base para un tipo de entidad
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <returns></returns>
        public static string ObtenerUrlEntidad(string pTipoEntidad)
        {
            string url = URLIntragnoss;
            return url + "items/";
        }

        /// <summary>
        /// Exporta una entidad completa, con todas sus relaciones, en un fichero GNOSS.
        /// </summary>
        /// <param name="pNombreFicheroGNOSS">Nombre del fichero en el que se realizará la exportación</param>
        /// <param name="pOntologia">Ontología que sigue la entidad</param>
        /// <param name="pEntidad">Entidad que se desea exportar</param>
        /// <param name="pEntidadesRelacionadas">Lista de entidades relcionadas con la entidad que se va a exportar</param>
        /// <param name="pMetadatos">Metadatos de la organización</param>
        /// <returns>El Stream que apunta al fichero</returns>
        public Stream PasarOWL(string pNombreFicheroGNOSS, Ontologia pOntologia, ElementoOntologia pEntidad, List<ElementoOntologia> pEntidadesRelacionadas, Dictionary<string, string> pMetadatos, bool pBusqueda)
        {
            return PasarOWL(pNombreFicheroGNOSS, pOntologia, pEntidad, pEntidadesRelacionadas, pMetadatos, null, pBusqueda);
        }

        /// <summary>
        /// Exporta una entidad completa, con todas sus relaciones, en un fichero GNOSS.
        /// </summary>
        /// <param name="pNombreFicheroGNOSS">Nombre del fichero en el que se realizará la exportación</param>
        /// <param name="pOntologia">Ontología que sigue la entidad</param>
        /// <param name="pEntidad">Entidad que se desea exportar</param>
        /// <param name="pEntidadesRelacionadas">Lista de entidades relacionadas con la entidad que se va a exportar</param>
        /// <param name="pMetadatos">Metadatos de la organización</param>
        /// <param name="pTipoElementoExportar">Tipo del elemento que se desea exportar</param>
        /// <returns>El Stream que apunta al fichero</returns>
        public Stream PasarOWL(string pNombreFicheroGNOSS, Ontologia pOntologia, ElementoOntologia pEntidad, List<ElementoOntologia> pEntidadesRelacionadas, Dictionary<string, string> pMetadatos, List<string> pTipoElementoExportar, bool pBusqueda)
        {
            List<ElementoOntologia> listaEntidadesPricipales = new List<ElementoOntologia>();
            listaEntidadesPricipales.Add(pEntidad);
            
            return PasarOWL(pNombreFicheroGNOSS, pOntologia, listaEntidadesPricipales, pEntidadesRelacionadas, pMetadatos, pTipoElementoExportar, pBusqueda);
        }

        /// <summary>
        /// Exporta una entidad completa, con todas sus relaciones, en un fichero GNOSS.
        /// </summary>
        /// <param name="pNombreFicheroGNOSS">nombre del fichero en el que se realizará la exportación</param>
        /// <param name="pOntologia">Ontología que sigue la entidad.</param>
        /// <param name="pEntidades">Entidades pricipales a exportar.</param>
        /// <param name="pEntidadesRelacionadas">lista de entidades relcionadas con la entidad que se va a exportar.</param>
        /// <param name="pMetadatos">Metadatos de la organización</param>
        /// <returns>El Stream que apunta al fichero.</returns>
        public Stream PasarOWL(string pNombreFicheroGNOSS, Ontologia pOntologia, List<ElementoOntologia> pEntidades, List<ElementoOntologia> pEntidadesRelacionadas, Dictionary<string, string> pMetadatos, bool pBusqueda = false)
        {
            return PasarOWL(pNombreFicheroGNOSS, pOntologia, pEntidades, pEntidadesRelacionadas, pMetadatos, null, pBusqueda);
        }

        /// <summary>
        /// Exporta una entidad completa, con todas sus relaciones, en un fichero GNOSS.
        /// </summary>
        /// <param name="pNombreFicheroGNOSS">Nombre del fichero en el que se realizará la exportación</param>
        /// <param name="pOntologia">Ontología que sigue la entidad</param>
        /// <param name="pEntidades">Entidades pricipales para exportar</param>
        /// <param name="pEntidadesRelacionadas">Lista de entidades relcionadas con la entidad que se va a exportar</param>
        /// <param name="pMetadatos">Metadatos de la organización</param>
        /// <param name="pTipoElementoExportar">Tipo del elemento que se desea exportar</param>
        /// <returns>El Stream que apunta al fichero</returns>
        public Stream PasarOWL(string pNombreFicheroGNOSS, Ontologia pOntologia, List<ElementoOntologia> pEntidades, List<ElementoOntologia> pEntidadesRelacionadas, Dictionary<string,string> pMetadatos, List<string> pTipoElementoExportar, bool pBusqueda)
        {
            //Inicializo el gestor
            InicializarGestionOWL();

            XmlTextWriter ficheroXML = null;
            Stream stream = null;
            
            //Creo el fichero
            if (pNombreFicheroGNOSS != null)
            {
                ficheroXML = new XmlTextWriter(pNombreFicheroGNOSS, null);
            }
            else
            {
                stream = new MemoryStream();
                ficheroXML = new XmlTextWriter(stream, null);
            }
            ficheroXML.Formatting = Formatting.Indented;
            ficheroXML.WriteStartDocument();
            
            //Crea los metadatos y el comienzo del fichero.
            CrearCabecerasOWL(ficheroXML, pMetadatos, pOntologia);
            
            //Crea las entidades principales en el fichero y sus entidades relacionadas.
            foreach (ElementoOntologia entidadPrinc in pEntidades)
            {
                CrearEntidadOWL(ficheroXML, entidadPrinc, pTipoElementoExportar);
            }

            if (pMetadatos != null)
            {
                CrearMetadatosOWL(ficheroXML, pMetadatos);
            }

            if (!pBusqueda)
            {
                if (pOntologia.RDFCVSemIncluido != null)
                {
                    IncluirRDFCVSemantico(ficheroXML, pOntologia);
                }
            }

            //Escribo el final del elemento raíz del archivo.
            ficheroXML.WriteEndElement();
            //Escribir en el fichero y cerrarlo.
            ficheroXML.Flush();

            if (pNombreFicheroGNOSS != null) //Solo cerramos el stream si hay fichero, ya que si no, hay que usar el stream.
            {
                ficheroXML.Close();
            }
            return stream;
        }

        /// <summary>
        /// Crea los metadatos de la organización y los almacena en un fichero GNOSS
        /// </summary>
        /// <param name="pFicheroXML">Fichero XML</param>
        /// <param name="pMetadatos">Metadatos de la organización</param>
        public static void CrearMetadatosOWL(XmlTextWriter pFicheroXML, Dictionary<string, string> pMetadatos)
        {
            //IEnumerator claves = pMetadatos.Keys.GetEnumerator();
            ////Creo nodo raíz
            //pFicheroXML.WriteStartElement("gnoss:Metadatos");
            ////escribo los metadatos como atributos del nodo raíz
            //while (claves.MoveNext())
            //{
            //    string clave = (string)claves.Current;
            //    pFicheroXML.WriteAttributeString(clave,pMetadatos[clave]);
            //}
            //pFicheroXML.WriteEndElement();
        }

        /// <summary>
        /// Crea una entidad en formato OWL en un fichero GNOSS.
        /// </summary>
        /// <param name="pFicheroXML">fichero en el que se almacenará la entidad</param>
        /// <param name="pEntidad">entidad que se desea exportar.</param>
        public static void CrearEntidadOWL(XmlTextWriter pFicheroXML, ElementoOntologia pEntidad)
        { 
            CrearEntidadOWL(pFicheroXML, pEntidad, null);
        }

        /// <summary>
        /// Quita todos los bloques CData de una cadena de texto
        /// </summary>
        /// <param name="pTexto">Texto del que se quieren eliminar los bloques CData</param>
        /// <returns></returns>
        private static string EliminarBloquesCDataDeTexto(string pTexto)
        {
            string textoMod = pTexto;
            while (textoMod.Contains("]]>"))
            {
                int inicio = textoMod.IndexOf("<![CDATA[");
                int fin = textoMod.IndexOf("]]>");
                if (inicio == -1)
                {
                    inicio = fin;
                }
                fin = fin - inicio + 3;
                textoMod = textoMod.Remove(inicio, fin);
            }
            textoMod = textoMod.Replace("\r", "").Replace("\n", "");
            return textoMod;
        }

        /// <summary>
        /// Crea una entidad en formato OWL en un fichero GNOSS.
        /// </summary>
        /// <param name="pFicheroXML">Fichero en el que se almacenará la entidad</param>
        /// <param name="pEntidad">Entidad que se desea exportar</param>
        /// <param name="pTipoElementoExportar">Tipo del elemento que se desea exportar</param>
        public static void CrearEntidadOWL(XmlTextWriter pFicheroXML, ElementoOntologia pEntidad, List<string> pTipoElementoExportar)
        {
            if ((!ListaEntidadesCreadas.Contains(pEntidad.ID)) && (pEntidad.EntidadValida))
            {
                //Incluyo la entidad en la lista de creadas
                ListaEntidadesCreadas.Add(pEntidad.ID);

                pFicheroXML.WriteStartElement(pEntidad.TipoEntidadCrearRdf);
                //pFicheroXML.WriteAttributeString("rdf:about", ObtenerUrlEntidad(pEntidad.TipoEntidad) + pEntidad.ID);
                pFicheroXML.WriteAttributeString("rdf:about", pEntidad.Uri);
                
                pFicheroXML.WriteStartElement("rdfs:label");
                pFicheroXML.WriteAttributeString("rdf:datatype", "http://www.w3.org/2000/01/rdf-schema#Literal");
                pFicheroXML.WriteAttributeString("xmlns:xml", "http://www.w3.org/XML/1998/namespace");
                pFicheroXML.WriteAttributeString("xml:space", "preserve");
                pFicheroXML.WriteCData(EliminarBloquesCDataDeTexto(pEntidad.Descripcion));
                pFicheroXML.WriteEndElement();

                List<Propiedad> atributos = pEntidad.ObtenerAtributos();

                foreach (Propiedad propiedad in atributos)
                {
                    if (propiedad.Seleccionada)
                    {
                        if (propiedad.FunctionalProperty && propiedad.UnicoValor.Key != null)
                        {
                            string tipoDato = "";
                            if (TipoDatos.ContainsKey(propiedad.RangoRelativo))
                            {
                                tipoDato = TipoDatos[propiedad.RangoRelativo];
                            }
                            else
                            {
                                tipoDato = propiedad.Rango;
                            }
                            pFicheroXML.WriteStartElement(propiedad.NombreConNamespace, null);

                            string valor = propiedad.UnicoValor.Key;
                            Uri uriAux = null;

                            if (!valor.Contains("http://") || valor.Contains(" ") || valor.Contains(",") || !Uri.TryCreate(valor, UriKind.Absolute, out uriAux))
                            {
                                pFicheroXML.WriteAttributeString("rdf:datatype", tipoDato);

                                if ((tipoDato.EndsWith("Literal") || tipoDato.EndsWith("string")) && !propiedad.NombreConNamespace.EndsWith("created"))
                                {
                                    pFicheroXML.WriteCData(EliminarBloquesCDataDeTexto(valor));
                                }
                                else
                                {
                                    pFicheroXML.WriteValue(valor);
                                }
                            }
                            else
                            {
                                pFicheroXML.WriteAttributeString("rdf:resource", valor);
                            }

                            pFicheroXML.WriteEndElement();
                        }
                        else
                        {
                            //mientras queden atributos de la misma propiedad por pasar
                            foreach (string valor in propiedad.ListaValores.Keys)
                            {
                                if (!string.IsNullOrEmpty(valor))
                                {
                                    //if ((propiedad.ValoresSeleccionados.ContainsKey(valor)) && (propiedad.ValoresSeleccionados[valor]))
                                    {
                                        string tipoDato = "";
                                        if (TipoDatos.ContainsKey(propiedad.RangoRelativo))
                                        {
                                            tipoDato = TipoDatos[propiedad.RangoRelativo];
                                        }
                                        else
                                        {
                                            tipoDato = propiedad.Rango;
                                        }
                                        pFicheroXML.WriteStartElement(propiedad.NombreConNamespace, null);
                                        Uri uriAux = null;

                                        if (!valor.Contains("http://") || valor.Contains(" ") || valor.Contains(",") || !Uri.TryCreate(valor, UriKind.Absolute, out uriAux))
                                        {
                                            pFicheroXML.WriteAttributeString("rdf:datatype", tipoDato);

                                            if ((tipoDato.EndsWith("Literal") || tipoDato.EndsWith("string")) && !propiedad.NombreConNamespace.EndsWith("created") && (!valor.Contains("http://") || valor.Contains(" ")))
                                            {
                                                pFicheroXML.WriteCData(EliminarBloquesCDataDeTexto(valor));
                                            }
                                            else
                                            {
                                                pFicheroXML.WriteValue(valor);
                                            }
                                        }
                                        else
                                        {
                                            pFicheroXML.WriteAttributeString("rdf:resource", valor);
                                        }
                                        pFicheroXML.WriteEndElement();
                                    }
                                }
                            }
                        }

                        foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                        {
                            foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                            {
                                if (!string.IsNullOrEmpty(valor))
                                {
                                    string tipoDato = "";
                                    if (TipoDatos.ContainsKey(propiedad.RangoRelativo))
                                    {
                                        tipoDato = TipoDatos[propiedad.RangoRelativo];
                                    }
                                    else
                                    {
                                        tipoDato = propiedad.Rango;
                                    }
                                    pFicheroXML.WriteStartElement(propiedad.NombreConNamespace, null);
                                    Uri uriAux = null;

                                    if (!valor.Contains("http://") || valor.Contains(" ") || valor.Contains(",") || !Uri.TryCreate(valor, UriKind.Absolute, out uriAux))
                                    {
                                        pFicheroXML.WriteAttributeString("rdf:datatype", tipoDato);
                                        pFicheroXML.WriteAttributeString("xml:lang", idioma);

                                        if ((tipoDato.EndsWith("Literal") || tipoDato.EndsWith("string")) && !propiedad.NombreConNamespace.EndsWith("created") && (!valor.Contains("http://") || valor.Contains(" ")))
                                        {
                                            pFicheroXML.WriteCData(EliminarBloquesCDataDeTexto(valor));
                                        }
                                        else
                                        {
                                            pFicheroXML.WriteValue(valor);
                                        }
                                    }
                                    else
                                    {
                                        pFicheroXML.WriteAttributeString("rdf:resource", valor);
                                        pFicheroXML.WriteAttributeString("xml:lang", idioma);
                                    }
                                    
                                    pFicheroXML.WriteEndElement();
                                }
                            }
                        }
                    }
                }
                //creo las relaciones con otras entidades.
                List<Propiedad> entidades = pEntidad.ObtenerEntidadesRelacionadas();
                
                foreach (Propiedad propiedad in entidades)
                {
                    if (propiedad.Seleccionada)
                    {
                        if (propiedad.FunctionalProperty)
                        {
                            if (!string.IsNullOrEmpty(propiedad.UnicoValor.Key))
                            {
                                pFicheroXML.WriteStartElement(propiedad.NombreConNamespace);//Antes propiedad.Nombre
                                //pFicheroXML.WriteAttributeString("rdf:resource", ObtenerUrlEntidad(propiedad.UnicoValor.Value.TipoEntidad) + propiedad.UnicoValor.Key);
                                if (propiedad.UnicoValor.Value != null)
                                {
                                    pFicheroXML.WriteAttributeString("rdf:resource", propiedad.UnicoValor.Value.Uri);
                                }
                                else if (!string.IsNullOrEmpty(propiedad.UnicoValor.Key))
                                {
                                    //if (string.IsNullOrEmpty(propiedad.Rango) || !propiedad.Ontologia.TiposEntidades.Contains(propiedad.Rango))
                                    //{
                                    //    pFicheroXML.WriteAttributeString("rdf:resource", propiedad.UnicoValor.Key);
                                    //}
                                    //else
                                    //{
                                    //    pFicheroXML.WriteAttributeString("rdf:about", propiedad.UnicoValor.Key);
                                    //}

                                    string resource = propiedad.UnicoValor.Key;

                                    if (resource != null && !resource.StartsWith("http"))
                                    {
                                        resource = URLIntragnoss + "items/" + resource;
                                    }

                                    pFicheroXML.WriteAttributeString("rdf:resource", resource);
                                }

                                pFicheroXML.WriteEndElement();
                            }
                        }
                        else
                        {
                            //mientras que queden entidades por pasar
                            string[] listaClaves = new string[propiedad.ListaValores.Keys.Count];
                            propiedad.ListaValores.Keys.CopyTo(listaClaves, 0);
                            
                            foreach (string valor in listaClaves)
                            {
                                if (!string.IsNullOrEmpty(valor))
                                {
                                    ElementoOntologia entidadRelacionada = propiedad.ListaValores[valor];

                                    if (/*(propiedad.ValoresSeleccionados[valor]) &&*/ ((entidadRelacionada == null) || (entidadRelacionada.EntidadValida)))
                                    {
                                        pFicheroXML.WriteStartElement(propiedad.NombreConNamespace);
                                        if (entidadRelacionada != null)
                                        {
                                            pFicheroXML.WriteAttributeString("rdf:resource", entidadRelacionada.Uri);
                                        }
                                        else
                                        {
                                            //if (string.IsNullOrEmpty(propiedad.Rango) || !propiedad.Ontologia.TiposEntidades.Contains(propiedad.Rango))
                                            //{
                                            //    pFicheroXML.WriteAttributeString("rdf:resource", valor);
                                            //}
                                            //else
                                            //{
                                            //    pFicheroXML.WriteAttributeString("rdf:about", valor);
                                            //}

                                            string resource = valor;

                                            if (resource != null && !resource.StartsWith("http"))
                                            {
                                                resource = URLIntragnoss + "items/" + resource;
                                            }

                                            pFicheroXML.WriteAttributeString("rdf:resource", resource);
                                        }
                                        pFicheroXML.WriteEndElement();
                                    }
                                    //si la propiedad no estaba seleccionada la elimino.
                                    else
                                    {
                                        UtilSemantica.EliminarEntidadRelacionada(entidadRelacionada, pEntidad, propiedad);
                                    }
                                }
                            }
                        }

                        foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                        {
                            foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                            {
                                if (!string.IsNullOrEmpty(valor))
                                {
                                    ElementoOntologia entidadRelacionada = propiedad.ListaValores[valor];

                                    if (entidadRelacionada == null || entidadRelacionada.EntidadValida)
                                    {
                                        pFicheroXML.WriteStartElement(propiedad.NombreConNamespace);

                                        if (entidadRelacionada != null)
                                        {
                                            pFicheroXML.WriteAttributeString("rdf:resource", entidadRelacionada.Uri);
                                        }
                                        else
                                        {
                                            string resource = valor;

                                            if (resource != null && !resource.StartsWith("http"))
                                            {
                                                resource = URLIntragnoss + "items/" + resource;
                                            }

                                            pFicheroXML.WriteAttributeString("rdf:resource", resource);
                                        }

                                        pFicheroXML.WriteAttributeString("xml:lang", idioma);
                                        pFicheroXML.WriteEndElement();
                                    }
                                    //si la propiedad no estaba seleccionada la elimino.
                                    else
                                    {
                                        UtilSemantica.EliminarEntidadRelacionada(entidadRelacionada, pEntidad, propiedad);
                                    }
                                }
                            }
                        }
                    }
                    //elimino la entidad con la que ésta propiedad la vinculaba.
                    else
                    {
                        string[] array = new string[propiedad.ListaValores.Keys.Count];
                        propiedad.ListaValores.Keys.CopyTo(array, 0);
                        
                        foreach(string valor in array)
                        {
                            UtilSemantica.EliminarEntidadRelacionada(valor, pEntidad, propiedad);
                        }
                    }
                }
                //en ElementoOntologia creamos una lista de string que contine para cada 

                foreach (string entidad in pEntidad.OWLSameAs)
                {

                    pFicheroXML.WriteStartElement("owl:sameAs");
                    pFicheroXML.WriteAttributeString("rdf:resource", entidad);
                    pFicheroXML.WriteEndElement();
                }


                pFicheroXML.WriteEndElement();
                
                foreach (ElementoOntologia entidad in pEntidad.EntidadesRelacionadas)
                {
                    if (pTipoElementoExportar == null || pTipoElementoExportar.Contains(entidad.TipoEntidad))
                    {
                        CrearEntidadOWL(pFicheroXML, entidad, pTipoElementoExportar);
                    }
                }
            }
        }

        /// <summary>
        /// Crea las cabeceras XML que llevarán todos los archivos .GNOSS
        /// </summary>
        /// <param name="pFicheroXML">fichero en el que se almacenarán las cabeceras</param>
        /// <param name="pMetadatos">Metadatos de la organización</param>
        /// <param name="pNamespacesDefinidos">Namespaces definidos en las ontologías leídas</param>
        public static void CrearCabecerasOWL(XmlTextWriter pFicheroXML, Dictionary<string, string> pMetadatos, Ontologia pOntologia)
        {
            List<string> namespacesAgregados = new List<string>();

            //Cabecera que llevaran todos los archivos.
            pFicheroXML.WriteStartElement("rdf:RDF");
            pFicheroXML.WriteAttributeString("xmlns:gnoss", "http://gnoss.com/gnoss.owl#");
            pFicheroXML.WriteAttributeString("xmlns:rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
            pFicheroXML.WriteAttributeString("xmlns:xsd", "http://www.w3.org/2001/XMLSchema#");
            pFicheroXML.WriteAttributeString("xmlns:rdfs", "http://www.w3.org/2000/01/rdf-schema#");
            pFicheroXML.WriteAttributeString("xmlns:owl", "http://www.w3.org/2002/07/owl#");
            //pFicheroXML.WriteAttributeString("xml:base", "http://www.gnoss.net/");
            /*pFicheroXML.WriteStartElement("owl:Ontology");
            pFicheroXML.WriteAttributeString("rdf:about", null);
            pFicheroXML.WriteEndElement();*/

            namespacesAgregados.Add("gnoss");
            namespacesAgregados.Add("rdf");
            namespacesAgregados.Add("xsd");
            namespacesAgregados.Add("rdfs");
            namespacesAgregados.Add("owl");

            foreach (string urlNamespaceDef in pOntologia.NamespacesDefinidos.Keys)
            {
                if (pOntologia.NamespacesDefinidos[urlNamespaceDef] != "RDF" && pOntologia.NamespacesDefinidos[urlNamespaceDef] != "gnoss" && pOntologia.NamespacesDefinidos[urlNamespaceDef] != "rdf" && pOntologia.NamespacesDefinidos[urlNamespaceDef] != "xsd" && pOntologia.NamespacesDefinidos[urlNamespaceDef] != "rdfs" && pOntologia.NamespacesDefinidos[urlNamespaceDef] != "owl" && !namespacesAgregados.Contains(pOntologia.NamespacesDefinidos[urlNamespaceDef]))
                {
                    pFicheroXML.WriteAttributeString("xmlns:" + pOntologia.NamespacesDefinidos[urlNamespaceDef], urlNamespaceDef);
                    namespacesAgregados.Add(pOntologia.NamespacesDefinidos[urlNamespaceDef]);
                }
            }

            if (!string.IsNullOrEmpty(mNamespaceOntologia) && !string.IsNullOrEmpty(mUrlOntologia) && !namespacesAgregados.Contains(mNamespaceOntologia))
            {
                pFicheroXML.WriteAttributeString("xmlns:" + mNamespaceOntologia, mUrlOntologia);
                namespacesAgregados.Add(mNamespaceOntologia);
            }

            foreach (string key in pOntologia.NamespacesDefinidosExtra.Keys)
            {
                if (!namespacesAgregados.Contains(key))
                {
                    pFicheroXML.WriteAttributeString("xmlns:" + key, pOntologia.NamespacesDefinidosExtra[key]);
                }
            }
        }

        #endregion

        #region Ontología

        /// <summary>
        /// Lee todas las entidades contenidas en la ontología.
        /// </summary>
        /// <param name="pContenidoOntologia">Contenido de la ontología.</param>
        /// <returns></returns>
        public List<ElementoOntologia> LeerEntidades(string pContenidoOntologia) 
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(pContenidoOntologia));
            List<ElementoOntologia> entidades = new List<ElementoOntologia>();
            bool readerActivo = true;

            //pasar de las cabeceras.
            readerActivo = reader.Read();
            while ((readerActivo) && (!reader.Name.Equals("owl:Class")))
            {
                readerActivo = reader.Read();
            }

            while (readerActivo)
            {
                //mientras que haya clases en el archivo
                while (readerActivo && reader.Name.Equals("owl:Class"))
                {
                    readerActivo = LeerEntidad(reader, entidades);
                }

                //posicion al reader en el siguiente elemento
                while (readerActivo && (!reader.NodeType.Equals(XmlNodeType.Element) || !reader.Name.Equals("owl:Class")))
                {
                    readerActivo = reader.Read();
                }
            }

            reader.Close();
            return entidades;
        }

        /// <summary>
        /// Lee una entidad del fichero de ontología.
        /// </summary>
        /// <param name="pReader">Variable que recorre el fichero de la ontología.</param>
        /// <param name="pEntidades">Lista de entidades a la que se a de añadir la nuenva entidad.</param>
        /// <returns></returns>
        private bool LeerEntidad(XmlTextReader pReader, List<ElementoOntologia> pEntidades)
        {
            bool readerActivo = true;

            ElementoOntologia entidad = null;

            if (!pReader.NodeType.Equals(XmlNodeType.Whitespace))
            {
                int profundidad = pReader.Depth;

                if (pReader.HasAttributes)//rdf:ID o rdf:about
                {
                    //leo el ID
                    entidad = LeerEntidadID(pReader, ref readerActivo, pEntidades);
                    
                    while ((readerActivo) && (pReader.Depth > profundidad))
                    {
                        if ((!pReader.NodeType.Equals(XmlNodeType.Whitespace)) && (!pReader.NodeType.Equals(XmlNodeType.EndElement)) && (!pReader.NodeType.Equals(XmlNodeType.Comment)))
                        {
                            if (pReader.Name.Equals("rdfs:subClassOf"))
                            {
                                if (pReader.HasAttributes)//superclase
                                {
                                    pReader.MoveToAttribute("rdf:resource");
                                    string valor = pReader["rdf:resource"];

                                    //Quitamos dichosa '#'
                                    if (valor.Length > 0 && valor[0] == '#')
                                    {
                                        valor = valor.Substring(1);
                                    }
                                    entidad.Superclases.Add(valor);
                                    readerActivo = pReader.Read();
                                    AsignarSubclase(pEntidades, entidad.TipoEntidad, entidad.Superclases[entidad.Superclases.Count - 1]);
                                }
                                else
                                {
                                    pReader.Read();
                                    while (pReader.NodeType.Equals(XmlNodeType.Whitespace))
                                    {
                                        pReader.Read();
                                    }
                                    if (pReader.Name.Equals("owl:Restriction"))//restriccion
                                        LeerRestriccion(pReader, ref profundidad, ref readerActivo, entidad, pEntidades);
                                    else//superclase definida aqui 
                                    {
                                        if (pReader.HasAttributes)//rdf:ID
                                        {
                                            pReader.MoveToAttribute(0);
                                            if (pReader.Name.Equals("rdf:ID"))
                                            {
                                                entidad.Superclases.Add(pReader["rdf:ID"]);
                                                pEntidades.Add(CrearElementoOntologia(pReader["rdf:ID"], mUrlOntologia, mNamespaceOntologia));
                                            }
                                            else//rdf:about
                                            {
                                                string valor = pReader["rdf:about"];

                                                //Quitamos dichosa '#'
                                                if (valor.Length > 0 && valor[0] == '#')
                                                {
                                                    valor = valor.Substring(1);
                                                }

                                                entidad.Superclases.Add(valor);

                                                if (!pEntidades.Any(item => item.TipoEntidad.Equals(valor)))
                                                {
                                                    // Es la primera vez que aparece la clase, la creo porque esta es la definición de la misma (luego no vuelve a aparecer en el OWL)
                                                    pEntidades.Add(CrearElementoOntologia(valor, mUrlOntologia, mNamespaceOntologia));
                                                }
                                            }
                                            
                                            readerActivo = pReader.Read();
                                            AsignarSubclase(pEntidades, entidad.TipoEntidad, entidad.Superclases[entidad.Superclases.Count - 1]);
                                        }
                                    }
                                }
                            }
                            else if (pReader.Name.Equals("especializacion"))//especializacion
                            {
                                //leo la especialización
                                pReader.Read();
                                entidad.Especializaciones.Add(pReader.Value);
                                pReader.Read();
                            }
                            else if (pReader.Name.Equals("generalizacion"))//especializacion
                            {
                                //Leo la generalización
                                pReader.Read();
                                entidad.Generalizacion = pReader.Value;
                                pReader.Read();
                            }
                            else if (pReader.Name.Equals("rdfs:label"))
                            {
                                string idioma = null;

                                if (pReader.HasAttributes)
                                {
                                    idioma = pReader.GetAttribute("xml:lang");
                                }

                                //Leo label
                                pReader.Read();
                                if (idioma == null)
                                {
                                    entidad.Label = pReader.Value;
                                }
                                else if (!entidad.LabelIdioma.ContainsKey(idioma))
                                {
                                    entidad.LabelIdioma.Add(idioma, pReader.Value);
                                }

                                pReader.Read();
                            }
                            else
                            {
                                //Leemos para evitar un bucle infinito:
                                pReader.Read();
                            }
                        }
                        //posicion al reader en el siguiente elemento
                        while ((readerActivo) && ((pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment))))
                        {
                            readerActivo = pReader.Read();
                        }
                    }
                }
                else//union of
                {
                    while (!pReader.Name.Equals("owl:unionOf"))
                    {
                        readerActivo = pReader.Read();
                    }
                    //recorro todas las clases.
                    pReader.Read();
                    while (!pReader.Name.Equals("owl:Class"))
                    {
                        readerActivo = pReader.Read();
                    }
                }
            }
            //posicion al reader en el siguiente elemento
            while (readerActivo && ((pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment))))
            {
                readerActivo = pReader.Read();
            }

            //Miro si hay alguna expecialización mediante elementos con nombre la clase definida:
            while (entidad != null && readerActivo && pReader.NodeType.Equals(XmlNodeType.Element) && pReader.Name.Equals(entidad.TipoEntidad))
            {
                //leo la especialización
                if (pReader.HasAttributes)
                {
                    ElementoOntologia entidadEspecializada = null;

                    //string entidadEspecializa = pReader["rdf:ID"];

                    entidadEspecializada = LeerEntidadID(pReader, ref readerActivo, pEntidades);
                    //entidad.Especializaciones.Add(entidadEspecializa);

                    //pReader.Read();

                    entidadEspecializada.Superclases.Add(entidad.TipoEntidad);
                    AsignarSubclase(pEntidades, entidadEspecializada.TipoEntidad, entidadEspecializada.Superclases[entidadEspecializada.Superclases.Count - 1]);
                }
            }

            return readerActivo;
        }

        /// <summary>
        /// Lee el ID de la entidad.
        /// </summary>
        /// <param name="pReader">Variable que recorre el fichero de la ontología.</param>
        /// <param name="pReaderActivo">Indica si se ha llegado al final de la ontología.</param>
        /// <param name="pEntidades">Lista de entidades a la que se agregará la nueva entidad.</param>
        /// <returns></returns>
        private ElementoOntologia LeerEntidadID(XmlTextReader pReader, ref bool pReaderActivo, List<ElementoOntologia> pEntidades)
        {
            ElementoOntologia entidad = null;
            
            if (pReader.GetAttribute("rdf:ID") != null)//leo el ID y creo la nueva entidad.
            {
                pReader.MoveToAttribute("rdf:ID");
                
                if (pReader.Name.Equals("rdf:ID"))
                {
                    string entidadID = ObtenerIDElementoSinNamespacesReferencia(pReader.Value);

                    entidad = CrearElementoOntologia(entidadID, mUrlOntologia, mNamespaceOntologia);
                    pEntidades.Add(entidad);
                    pReaderActivo = pReader.Read();
                }
            }
            else
            {
                //el atributo era rdf:about, no rdf:ID, porque era una entidad ya definida.
                pReader.MoveToAttribute("rdf:about");

                string entidadID = ObtenerIDElementoSinNamespacesReferencia(pReader.Value);

                //Elimino el '#' que se puede quedar al principio:
                if (entidadID.Length > 0 && entidadID[0] == '#')
                {
                    entidadID = entidadID.Substring(1);
                }

                foreach (ElementoOntologia ent in pEntidades)
                {
                    //if (ent.TipoEntidad.Equals(entidadID.Substring(entidadID.IndexOf('#') + 1)))
                    if (ent.TipoEntidad.Equals(entidadID))
                    {
                        entidad = ent;
                        break;
                    }
                }
                if (entidad == null)
                {
                    entidad = CrearElementoOntologia(entidadID, mUrlOntologia, mNamespaceOntologia);
                    pEntidades.Add(entidad);
                }
                pReaderActivo = pReader.Read();
            }
            //Leo las superclases y restricciones de la entidad.
            while ((pReaderActivo) && ((pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment))))
            {
                pReaderActivo = pReader.Read();
            }

            return entidad;
        }

        /// <summary>
        /// Obtiene el verdadero ID de un elemento si este contiene un namespace referencia a otro.
        /// </summary>
        /// <param name="pElementoID">ID del elemento con namespace referencia a otro</param>
        /// <returns>ID de un elemento con el namespace referencia a otro sustituido por el verdadero valor</returns>
        private string ObtenerIDElementoSinNamespacesReferencia(string pElementoID)
        {
            //Si la entidad posee un namespace referencia lo sustituimos por el valor verdadero:
            if (pElementoID.Length > 0 && pElementoID[0] == '&' && Ontologia != null)
            {
                string namespaceReferencia = pElementoID.Substring(0, pElementoID.IndexOf(";") + 1);

                if (Ontologia.ValorVerdaderNamespacesReferencia.ContainsKey(namespaceReferencia))
                {
                    return Ontologia.NamespacesDefinidos[Ontologia.ValorVerdaderNamespacesReferencia[namespaceReferencia]] + pElementoID.Replace(namespaceReferencia, "");
                }
            }

            return pElementoID;
        }

        /// <summary>
        /// Lee una restricción de una entidad que afecta a una propiedad concreta.
        /// </summary>
        /// <param name="pReader">Variable que recorre el fichero de la ontología.</param>
        /// <param name="pProfundidad">Profundidad del comienzo de la restricción.</param>
        /// <param name="pReaderActivo">Indica si se ha alcanzado el final de la ontología.</param>
        /// <param name="pEntidad">Entidad que posee la restricción.</param>
        /// <param name="pEntidades">Lista de entidades que contiene la entidad.</param>
        /// <returns></returns>
        private bool LeerRestriccion(XmlTextReader pReader, ref int pProfundidad, ref bool pReaderActivo, ElementoOntologia pEntidad, List<ElementoOntologia> pEntidades)
        {
            string tipoRestriccion;
            string valor;
            string propiedad = null;
            
            while ((pReader.Name.Equals("owl:Restriction")) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
            {
                pReader.Read();//paso al elemento owl:restriction
            }
            if (pReader.Name.Equals("owl:onProperty"))//primero la propiedad sobre la que hace referencia.
            {
                if (pReader.GetAttribute("rdf:resource") != null)//rdf:about
                {
                    pReader.MoveToAttribute("rdf:resource");
                    //propiedad = pReader["rdf:resource"].Substring(pReader["rdf:resource"].IndexOf('#') + 1);
                    propiedad = pReader["rdf:resource"];
                    pReaderActivo = pReader.Read();
                }

                while ((pReader.Name.Equals("owl:onProperty")) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
                {
                    pReader.Read();//paso al elemento owl:onProperty
                }
                if (pReader.GetAttribute("rdf:about") != null)//rdf:about
                {
                    pReader.MoveToAttribute("rdf:about");
                    //propiedad = pReader["rdf:about"].Substring(pReader["rdf:about"].IndexOf('#') + 1);
                    propiedad = pReader["rdf:about"];
                    pReaderActivo = pReader.Read();
                }
                else if (pReader.GetAttribute("rdf:ID") != null)//rdf:ID
                {
                    pReader.MoveToAttribute("rdf:ID");
                    //propiedad = pReader["rdf:ID"].Substring(pReader["rdf:ID"].IndexOf('#') + 1);
                    propiedad = pReader["rdf:ID"];
                    pReaderActivo = pReader.Read();
                }
                while ((pReader.NodeType.Equals(XmlNodeType.Whitespace))||(pReader.NodeType.Equals(XmlNodeType.EndElement)))
                {
                    pReader.Read();
                }
                tipoRestriccion = pReader.Name.Substring(4);
                
                if ((tipoRestriccion.Equals("allValuesFrom")) || (tipoRestriccion.Equals("someValuesFrom")))
                {
                    while (!pReader.Name.Equals("owl:Class"))
                    {
                        pReader.Read();
                    }
                    if (pReader.GetAttribute("rdf:about") != null)//rdf:about
                    {
                        pReader.MoveToAttribute("rdf:about");
                        valor = pReader["rdf:about"];
                    }
                    else//rdf:ID
                    {
                        pReader.MoveToAttribute("rdf:ID");
                        valor = pReader["rdf:ID"];
                    }
                }
                else
                {
                    pReader.Read();
                    valor = pReader.Value;
                }
                pReaderActivo = pReader.Read();
            }
            else//primero el tipo de restricción
            {
                tipoRestriccion = pReader.Name.Substring(4);
                pReader.Read();
                valor = pReader.Value;
                
                if (string.IsNullOrEmpty(valor))//el valor es una entidad.
                {
                    while ((!pReader.Name.Equals("owl:Class")) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
                    {
                        pReader.Read();//paso al elemento owl:Class
                    }
                    if (pReader.HasAttributes)
                    {
                        if (pReader.GetAttribute("rdf:about") != null)
                        {
                            pReader.MoveToAttribute("rdf:about");
                            //valor = pReader.GetAttribute("rdf:about").Substring(pReader.GetAttribute("rdf:about").IndexOf('#') + 1);
                            valor = pReader.GetAttribute("rdf:about");
                        }
                        else
                        {
                            pReader.MoveToAttribute("rdf:ID");
                            valor = pReader.GetAttribute("rdf:ID");
                            pEntidades.Add(CrearElementoOntologia(pReader.GetAttribute("rdf:ID"),mUrlOntologia,mNamespaceOntologia));
                        }
                    }
                }
                pReader.Read();
                
                while ((pReader.Name.Equals("owl:onProperty")) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.EndElement)))
                {
                    pReader.Read();//paso al elemento owl:onProperty
                }
                if (pReader.GetAttribute("rdf:about") != null)//rdf:about
                {
                    pReader.MoveToAttribute("rdf:about");
                    propiedad = pReader["rdf:about"];
                    pReaderActivo = pReader.Read();
                }
                else//rdf:ID
                {
                    pReader.MoveToAttribute("rdf:ID");
                    propiedad = pReader["rdf:ID"];
                    pReaderActivo = pReader.Read();
                }
            }

            //Eliminamos la '#' molesta:
            if (propiedad != null && propiedad.Length > 0 && propiedad[0] == '#')
            {
                propiedad = propiedad.Substring(1);
            }

            Restriccion restriccion = new Restriccion(propiedad, UtilTipoRestriccion.getTipoRestriccion(tipoRestriccion), valor);
            pEntidad.AddRestriccion(restriccion);

            return pReaderActivo;
        }

        /// <summary>
        /// Asigna a una clase una de sus subclases
        /// </summary>
        /// <param name="pEntidades">Lista de todas las entidades de la ontología.</param>
        /// <param name="pSubclase">Entidad subclase de otra.</param>
        /// <param name="pSuperclase">Entidad padre de la subclase.</param>
        private static void AsignarSubclase(List<ElementoOntologia> pEntidades, string pSubclase, string pSuperclase)
        {
            for (int contador = 0; contador < pEntidades.Count;contador++ )
            {
                if (pEntidades[contador].TipoEntidad.Equals(pSuperclase))
                {
                    pEntidades[contador].Subclases.Add(pSubclase);
                    break;
                }
            }
        }

        /// <summary>
        /// Lee todas las propiedades desde un fichero de ontología. 
        /// </summary>
        /// <param name="pContenidoOntologia">Contenido de la ontología.</param>
        /// <returns>Lista de propiedades</returns>
        public List<Propiedad> LeerPropiedades(string pContenidoOntologia) 
        {
            List<Propiedad> propiedades = new List<Propiedad>();

            XmlTextReader reader = new XmlTextReader(new StringReader(pContenidoOntologia));
            bool readerActivo = true;
            Propiedad propiedad=null;

            //Alcanzar el punto en el que comienzan las propiedades
            readerActivo = reader.Read();

            while ((readerActivo) && ((!reader.Name.Equals("owl:DatatypeProperty"))||(!(reader.Depth==1))) 
                && ((!reader.Name.Equals("owl:ObjectProperty"))||(!(reader.Depth==1)))
                && ((!reader.Name.Equals("owl:FunctionalProperty")) || (!(reader.Depth == 1))))
            {
                readerActivo = reader.Read();
            }

            while (readerActivo)
            {
                if (!reader.NodeType.Equals(XmlNodeType.Whitespace))
                {
                    string nombre="";
                    TipoPropiedad tipoPropiedad=TipoPropiedad.DatatypeProperty;

                    if (!reader.Name.Equals("owl:FunctionalProperty"))
                        tipoPropiedad = UtilTipoPropiedad.getTipoPropiedad(reader.Name.Substring(reader.Name.IndexOf(':') + 1));
                    
                    //Si hay atributos, leo el id (nombre) de la propiedad.
                    if (reader.HasAttributes)
                    {
                        int profundidad = reader.Depth;

                        //almaceno el id del elemento
                        if (reader.GetAttribute("rdf:ID") != null)
                        {
                            reader.MoveToAttribute("rdf:ID");
                            nombre = reader["rdf:ID"];
                            //Leo el dominio, el rango y los tipos de propiedad
                            readerActivo = LeerPropiedad(reader, out propiedad, profundidad, nombre, tipoPropiedad, propiedades);
                            propiedades.Add(propiedad);
                        }
                        else//rdf:about
                        {
                            bool encontrado = false;
                            reader.MoveToAttribute("rdf:about");
                            if (reader["rdf:about"] != null)
                            {
                                //nombre = reader["rdf:about"].Substring(reader["rdf:about"].IndexOf('#') + 1);
                                nombre = reader["rdf:about"];
                            }

                            if (nombre != "")
                            {
                                foreach (Propiedad prop in propiedades)
                                {
                                    if (prop.Nombre.Equals(nombre))
                                    {
                                        propiedad = prop;
                                        encontrado = true;
                                        break;
                                    }
                                }
                                if (encontrado)
                                    readerActivo = LeerPropiedad(reader, out propiedad, profundidad, nombre, tipoPropiedad, propiedades);
                                else
                                {
                                    readerActivo = LeerPropiedad(reader, out propiedad, profundidad, nombre, tipoPropiedad, propiedades);
                                    propiedades.Add(propiedad);
                                }
                            }
                            else
                            {
                                //posiciono al reader en el siguiente elemento.
                                readerActivo = reader.Read();
                                PosicionarReaderSiguienteElemento(reader, ref readerActivo);
                            }
                        }

                    }
                    else
                    {
                        readerActivo = reader.Read();//Evito posible bucle infinito
                    }
                }
                //posiciono al reader en el siguiente elemento.
                PosicionarReaderSiguienteElemento(reader, ref readerActivo);
                //while ((readerActivo) && ((reader.NodeType.Equals(XmlNodeType.EndElement)) || (reader.NodeType.Equals(XmlNodeType.Whitespace)) || (reader.NodeType.Equals(XmlNodeType.Comment))))
                //{
                //    readerActivo=reader.Read();
                //}
            }
            reader.Close();
            return propiedades;
        }

        /// <summary>
        /// Lee una determinada propiedad de la ontología.
        /// </summary>
        /// <param name="pReader">Variable que recorre el fichero de la ontología.</param>
        /// <param name="pPropiedad">Propiedad que se desea leer.</param>
        /// <param name="pProfundidad">Profundidad a la que se encuentra la propiedad.</param>
        /// <param name="pNombre">Nombre de la propiedad a leer.</param>
        /// <param name="pTipoPropiedad">Tipo de la entidad a leer.</param>
        /// <param name="pListaPropiedades">Lista de propiedades</param>
        private bool LeerPropiedad(XmlTextReader pReader, out Propiedad pPropiedad,int pProfundidad, string pNombre,TipoPropiedad pTipoPropiedad, IList<Propiedad> pListaPropiedades)
        {
            List<string> dominio = new List<string>();
            string rango = "";
            bool functionalProperty = false;
            string nombreReal = pNombre;
            bool visible=true;
            Propiedad propiedadInversa = null;
            Propiedad propiedadEquivalente = null;
            List<string> listaSuperPropiedades = new List<string>();
            List<string> listaValoresPermitidos = new List<string>(); ;
            bool readerActivo = pReader.Read();
            string label = null;
            Dictionary<string, string> labelIdioma = new Dictionary<string, string>();

            while (pReader.NodeType.Equals(XmlNodeType.Whitespace))
            {
                readerActivo = pReader.Read();
            }

            //Mientras el reader esté activo y la profundidad del nodo sea mayor que la del nodo raíz de la propiedad
            while ((readerActivo) && (pReader.Depth > pProfundidad))
            { 
                if (!pReader.NodeType.Equals(XmlNodeType.Whitespace))
                {
                    if (pReader.Name.Equals("rdfs:domain"))//lista de tipos de entidad a los que puede pertenecer la propiedad.
                    {
                        if (pReader.HasAttributes)//solo un dominio
                        {
                            pReader.MoveToAttribute(0);
                            //dominio.Add(pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#') + 1));
                            dominio.Add(pReader["rdf:resource"]);
                            readerActivo = pReader.Read();
                        }
                        else//varios dominios
                        {
                            //buscamos el comienzo de la colección
                            while(!pReader.Name.Equals("owl:unionOf"))
                            {
                                readerActivo = pReader.Read();
                            }
                            //busco el primer dominio.
                            pReader.Read();
                            while (!pReader.Name.Equals("owl:Class"))
                            {
                                readerActivo = pReader.Read();
                            }
                            //leo todos los dominios
                            while (!pReader.NodeType.Equals(XmlNodeType.EndElement))
                            {
                                if (pReader.Name.Equals("owl:Class"))
                                {
                                    pReader.MoveToAttribute("rdf:about");
                                    //dominio.Add(pReader["rdf:about"].Substring(pReader["rdf:about"].LastIndexOf('#') + 1));
                                    dominio.Add(pReader["rdf:about"]);
                                }
                                readerActivo = pReader.Read();
                            }
                        }
                        //dejar pReader en la posicion del siguiente elemento
                        
                    }
                    else if (pReader.Name.Equals("rdfs:range"))//tipo de datos de la propiedad
                    {
                        if (pReader.HasAttributes)
                        {
                            pReader.MoveToAttribute("rdf:resource");
                            //rango = pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#') + 1);
                            rango = pReader["rdf:resource"];
                            readerActivo = pReader.Read();
                        }
                        //TODO: JAVIER -> Este if y el else los añado pero con poco conocimiento de lo que hago.
                        else 
                        {//Propiedad One OF
                            string rangoAux = null;
                            //buscamos el que contiene un One Of

                            while (!pReader.Name.Equals("owl:DataRange") && !pReader.Name.Equals("owl:Class"))
                            {
                                readerActivo = pReader.Read();
                            }

                            if (pReader.Name.Equals("owl:DataRange"))
                            {
                                while (!pReader.Name.Equals("owl:oneOf"))
                                {
                                    readerActivo = pReader.Read();
                                }

                                while ((readerActivo) && (!pReader.NodeType.Equals(XmlNodeType.EndElement)) && (!pReader.NodeType.Equals(XmlNodeType.Whitespace)) && (!pReader.NodeType.Equals(XmlNodeType.Comment)))
                                {
                                    bool rangoFinalizado = false;
                                    while (readerActivo && !rangoFinalizado && !pReader.Name.Equals("rdf:first"))
                                    {
                                        readerActivo = pReader.Read();

                                        if (pReader.NodeType.Equals(XmlNodeType.EndElement) && pReader.Name.Equals("rdfs:range"))
                                        {
                                            rangoFinalizado = true;
                                        }
                                    }

                                    if (rangoFinalizado)
                                    {
                                        break;
                                    }

                                    if (rangoAux == null)
                                    {
                                        if (pReader["rdf:resource"] != null)
                                            //rangoAux = pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#') + 1);
                                            rangoAux = pReader["rdf:resource"];
                                        else
                                            //rangoAux = pReader["rdf:datatype"].Substring(pReader["rdf:datatype"].LastIndexOf('#') + 1);
                                            rangoAux = pReader["rdf:datatype"];
                                    }
                                    string componente = pReader.ReadString();
                                    listaValoresPermitidos.Add(componente);
                                    readerActivo = pReader.Read();

                                    while ((readerActivo) && !pReader.Name.Equals("owl:oneOf") && ((pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment))))
                                    {
                                        readerActivo = pReader.Read();
                                    }
                                }
                                rango = rangoAux;
                            }
                            else
                            {
                                while ((readerActivo) && ((!pReader.Name.Equals("owl:Class") || (!pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment)))))
                                {
                                    readerActivo = pReader.Read();
                                }
                            }
                        }
                    }
                    else if (pReader.Name.Equals("rdf:type"))//tipo de propiedad
                    {
                        //también tratar inverse,transitive...
                        pReader.MoveToAttribute("rdf:resource");
                        string tipo = pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#')+1);
                        switch (tipo)
                        {
                            case "FunctionalProperty":
                                functionalProperty = true;
                                break;
                            case "DatatypeProperty":
                                functionalProperty = true;
                                pTipoPropiedad = TipoPropiedad.DatatypeProperty;
                                break;
                            case "ObjectProperty":
                                functionalProperty = true;
                                pTipoPropiedad = TipoPropiedad.ObjectProperty;
                                break;
                        }
                        readerActivo=pReader.Read();
                    }
                    else if (pReader.Name.Equals("owl:inverseOf"))
                    {
                        if (pReader.HasAttributes)//rdf:resourcce
                        {
                            pReader.MoveToAttribute("rdf:resource");
                            //string nombrePropiedad = pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#')+1);
                            string nombrePropiedad = pReader["rdf:resource"];
                            propiedadInversa = UtilSemantica.ObtenerPropiedadDeNombre(nombrePropiedad, pListaPropiedades);
                        }
                        else//propiedad definida aqui.
                        {
                            pReader.Read();
                            while (pReader.NodeType.Equals(XmlNodeType.Whitespace))
                            {
                                pReader.Read();
                            }
                            TipoPropiedad tipoPropiedad = UtilTipoPropiedad.getTipoPropiedad(pReader.Name.Substring(pReader.Name.IndexOf(':') + 1));
                            if (pReader.HasAttributes)
                            {
                                pReader.MoveToAttribute("rdf:ID");
                                string nombre = pReader["rdf:ID"];
                                propiedadInversa = CrearPropiedad(nombre, tipoPropiedad);
                            }
                        }
                        pReader.Read();
                    }
                    else if (pReader.Name.Equals("owl:equivalentProperty"))
                    {
                        if (pReader.HasAttributes)//rdf:resourcce
                        {
                            pReader.MoveToAttribute("rdf:resource");
                            //string nombrePropiedad = pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#') + 1);
                            string nombrePropiedad = pReader["rdf:resource"];
                            propiedadEquivalente = UtilSemantica.ObtenerPropiedadDeNombre(nombrePropiedad, pListaPropiedades);
                        }
                        else//propiedad definida aqui.
                        {
                            pReader.Read();
                            while (pReader.NodeType.Equals(XmlNodeType.Whitespace))
                            {
                                pReader.Read();
                            }
                            TipoPropiedad tipoPropiedad = UtilTipoPropiedad.getTipoPropiedad(pReader.Name.Substring(pReader.Name.IndexOf(':') + 1));
                            if (pReader.HasAttributes)
                            {
                                pReader.MoveToAttribute("rdf:ID");
                                string nombre = pReader["rdf:ID"];
                                propiedadEquivalente = CrearPropiedad(nombre, tipoPropiedad);
                            }
                        }
                        pReader.Read();
                    }
                    else if (pReader.Name.Equals("rdfs:subPropertyOf"))
                    {
                        if (pReader.HasAttributes)//rdf:resourcce
                        {
                            pReader.MoveToAttribute("rdf:resource");
                            //string nombrePropiedad = pReader["rdf:resource"].Substring(pReader["rdf:resource"].LastIndexOf('#') + 1);
                            string nombrePropiedad = pReader["rdf:resource"];

                            //Limpio las '#' que se quedan delante estorbando:
                            if (nombrePropiedad.Length > 0 && nombrePropiedad[0] == '#')
                            {
                                nombrePropiedad = nombrePropiedad.Substring(1);
                            }

                            listaSuperPropiedades.Add(nombrePropiedad);
                        }

                        pReader.Read();
                    }
                    else if (pReader.Name.Equals("NombreReal"))//nombre real de la propiedad
                    {
                        pReader.Read();
                        nombreReal = pReader.Value;
                        pReader.Read();
                    }
                    else if (pReader.Name.Equals("Visible"))//nombre real de la propiedad
                    {
                        pReader.Read();
                        visible = bool.Parse(pReader.Value);
                        pReader.Read();
                    }
                    else if (pReader.Name.Equals("rdfs:label"))
                    {
                        string idioma = null;

                        if (pReader.HasAttributes)
                        {
                            idioma = pReader.GetAttribute("xml:lang");
                        }

                        //Leo label
                        pReader.Read();
                        if (idioma == null)
                        {
                            label = pReader.Value;
                        }
                        else if (!labelIdioma.ContainsKey(idioma))
                        {
                            labelIdioma.Add(idioma, pReader.Value);
                        }
                        pReader.Read();
                    }
                    else
                    {
                        //Leemos para evitar un bucle infinito:
                        pReader.Read();
                    }
                }
                //posicion al reader en el siguiente elemento
                while ((readerActivo) && ((pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment))))
                {
                    readerActivo = pReader.Read();
                }
            }

            //Si la entidad posee un namespace referencia lo sustituimos por el valor verdadero:
            if (Ontologia != null)
            {
                rango = ObtenerIDElementoSinNamespacesReferencia(rango);

                //Limpio las '#' que se quedan delante estorbando:
                if (rango.Length > 0 && rango[0] == '#')
                {
                    rango = rango.Substring(1);
                }

                for (int i = 0; i < dominio.Count; i++)
                {
                    dominio[i] = ObtenerIDElementoSinNamespacesReferencia(dominio[i]);

                    //Limpio las '#' que se quedan delante estorbando:
                    if (dominio[i].Length > 0 && dominio[i][0] == '#')
                    {
                        dominio[i] = dominio[i].Substring(1);
                    }
                }
            }

            //Limpio las '#' que se quedan delante estorbando:
            if (pNombre.Length > 0 && pNombre[0] == '#')
            {
                pNombre = pNombre.Substring(1);
            }

            pPropiedad = CrearPropiedad(pNombre, pTipoPropiedad);
            pPropiedad.Dominio = dominio;
            pPropiedad.Rango = rango;
            pPropiedad.FunctionalProperty = functionalProperty;
            pPropiedad.NombreReal = nombreReal;
            pPropiedad.Visible = visible;
            pPropiedad.PropiedadInversa = propiedadInversa;
            pPropiedad.ListaValoresPermitidos = listaValoresPermitidos;
            pPropiedad.SuperPropiedades = listaSuperPropiedades;
            pPropiedad.Label = label;
            pPropiedad.LabelIdioma = labelIdioma;

            return readerActivo;
        }

        /// <summary>
        /// Crea una nueva propiedad a partir del nombre y tipo pasados por parámetro
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad que se va a crear</param>
        /// <param name="pTipoPropiedad">Tipo de la propiedad que se va a crear</param>
        /// <returns>Nueva propiedad</returns>
        public virtual Propiedad CrearPropiedad(string pNombrePropiedad, TipoPropiedad pTipoPropiedad)
        {
            return new Propiedad(pNombrePropiedad, pTipoPropiedad, Ontologia);
        }

        /// <summary>
        /// Crea una nueva propiedad a partir de otra pasada por parámetro
        /// </summary>
        /// <param name="pPropiedad">Propiedad original</param>
        /// <returns>Nueva propiedad</returns>
        public virtual Propiedad CrearPropiedad(Propiedad pPropiedad)
        {
            return new Propiedad(pPropiedad);
        }

        #endregion

        #region Leer OWL

        /// <summary>
        /// Lee los metadatos incluidos en un fichero gnoss
        /// </summary>
        /// <param name="pOntologia">Ontología a seguir.</param>
        /// <param name="pTextoFichero">Texto del fichero.</param>
        /// <returns></returns>
        public static Dictionary<string, string> LeerMetadatos(Ontologia pOntologia, string pTextoFichero)
        {
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(pTextoFichero));
            Dictionary<string, string> metadatos = new Dictionary<string, string>();
            //Busca el nodo metadatos.
            try
            {
                bool readerActivo = reader.Read();
                while ((readerActivo) && (!reader.Name.Equals("gnoss:Metadatos")))
                {
                    readerActivo = reader.Read();
                }
                if (reader.HasAttributes)
                {
                    //Recorro los atributos.
                    int numeroAributos = reader.AttributeCount;
                    for (int i = 0; i < numeroAributos; i++)
                    {
                        reader.MoveToAttribute(i);
                        if (reader.Name.Equals("ProcesoCertificado"))
                        {
                            if (reader.Value.Equals("False"))
                                break;
                        }
                        metadatos.Add(reader.Name, reader.Value);
                    }
                }
            }
            catch (Exception)
            {
                if (reader != null)
                    reader.Close();
                
                throw new Exception("El fichero que está intentando importar no es válido.");
            }

            //reader.Close();
            return metadatos;
        }

        /// <summary>
        /// Lee los namespaces incluidos en un fichero de la ontología.
        /// </summary>
        /// <param name="pOntologia">Ontología a seguir.</param>
        /// <param name="pTextoFichero">Texto del fichero.</param>
        /// <returns>Lista con los namespaces definidos para la ontología, clave-valor</returns>
        public static Dictionary<string, string> LeerNamespaces(Ontologia pOntologia, string pTextoFichero)
        {
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(pTextoFichero));
            Dictionary<string, string> namespaces = new Dictionary<string, string>();

            bool readerActivo = reader.Read();
            while (readerActivo && (!reader.Name.Equals("rdf:RDF") || !reader.NodeType.Equals(XmlNodeType.Element)))
            {
                readerActivo = reader.Read();
            }
            if (readerActivo && reader.HasAttributes)
            {
                //Recorro los atributos.
                int numeroAributos = reader.AttributeCount;
                for (int i = 0; i < numeroAributos; i++)
                {
                    reader.MoveToAttribute(i);
                    if (reader.Name.Contains("xmlns:"))
                    {
                        if (!namespaces.ContainsKey(reader.Value))
                        {
                            namespaces.Add(reader.Value, reader.Name.Replace("xmlns:", ""));
                        }
                    }
                }
            }
           

            reader.Close();
            return namespaces;
        }

        /// <summary>
        /// Lee las ontologías importadas en un fichero de la ontología.
        /// </summary>
        /// <param name="pOntologia">Ontología a seguir.</param>
        /// <param name="pTextoFichero">Texto del fichero.</param>
        public static void LeerOntologiasImportadas(Ontologia pOntologia, string pTextoFichero)
        {
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(pTextoFichero));

            bool readerActivo = reader.Read();

            while (readerActivo)
            {
                while (readerActivo && !reader.Name.Equals("owl:imports") && !reader.Name.Equals("owl:Class"))
                {
                    readerActivo = reader.Read();
                }

                if (reader.Name.Equals("owl:Class"))
                {
                    break;
                }

                if (readerActivo && reader.Name.Equals("owl:imports") && reader.HasAttributes)
                {
                    //Recorro los atributos.
                    int numeroAributos = reader.AttributeCount;
                    for (int i = 0; i < numeroAributos; i++)
                    {
                        reader.MoveToAttribute(i);
                        if (reader.Name.Equals("rdf:resource"))
                        {
                            if (!pOntologia.UrlOntologiasImportadas.Contains(reader.Value))
                            {
                                pOntologia.UrlOntologiasImportadas.Add(reader.Value);
                            }
                        }
                    }
                }

                readerActivo = reader.Read();
            }

            reader.Close();
        }

        /// <summary>
        /// Invierte la clave de los namespaces que están almacenados en un diccionario.
        /// </summary>
        /// <param name="pNamespaces">Diccionario con namespaces</param>
        /// <returns>Diccionario con los namespaces con la clave invertida</returns>
        public static Dictionary<string, string> InvertirNamespaces(Dictionary<string, string> pNamespaces)
        {
            Dictionary<string, string> namesInv = new Dictionary<string, string>();

            foreach (string key in pNamespaces.Keys)
            {
                if (!namesInv.ContainsKey(pNamespaces[key]))
                {
                    namesInv.Add(pNamespaces[key], key);
                }
            }

            return namesInv;
        }

        /// <summary>
        /// Lee todas las entidades contenidas en un fichero gnoss.
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pTextoFichero">Texto del fichero.</param>
        /// <returns></returns>
        public ElementoOntologia LeerFicheroRDF(Ontologia pOntologia, string pTextoFichero)
        {
            List<ElementoOntologia> listaElementos = LeerFicheroRDF(pOntologia, pTextoFichero, false);

            if (listaElementos.Count > 0)
            {
                return listaElementos[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Lee todas las entidades contenidas en un fichero gnoss.
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pTextoFichero">Texto del fichero.</param>
        /// <param name="pTraerEntidadesSuperiores">Indica si se deben traer las entidades superiores si es TRUE, o la entidad principal si es FALSE</param>
        /// <returns></returns>
        public List<ElementoOntologia> LeerFicheroRDF(Ontologia pOntologia, string pTextoFichero, bool pTraerEntidadesSuperiores)
        {
            return LeerFicheroRDF(pOntologia, pTextoFichero, pTraerEntidadesSuperiores, false);
        }

        /// <summary>
        /// Lee todas las entidades contenidas en un fichero gnoss.
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pTextoFichero">Texto del fichero.</param>
        /// <param name="pTraerEntidadesSuperiores">Indica si se deben traer las entidades superiores si es TRUE, o la entidad principal si es FALSE</param>
        /// <param name="pValidarRdf">Indica si se debe valirdar el RDF al leerlo</param>
        /// <returns></returns>
        public List<ElementoOntologia> LeerFicheroRDF(Ontologia pOntologia, string pTextoFichero, bool pTraerEntidadesSuperiores, bool pValidarRdf)
        {
            if (pValidarRdf)
            {
                NoPermitirNuevosElementos = new List<string>();
                pOntologia.GestorOWL.NoPermitirNuevosElementos = new List<string>();
            }

            NamespacesRDFLeyendo = InvertirNamespaces(LeerNamespaces(pOntologia, pTextoFichero));
            XmlTextReader reader = new XmlTextReader(new System.IO.StringReader(pTextoFichero));

            List<ElementoOntologia> listaEntidadesSupOPrinc = new List<ElementoOntologia>();
            List<ElementoOntologia> listaEntidades;
            try
            {
                //Busco el nodo ontology
                bool readerActivo = reader.Read();
                while ((readerActivo) && (!reader.Name.Equals("rdf:RDF")))
                {
                    readerActivo = reader.Read();
                }

                readerActivo = reader.Read();
                while ((readerActivo) && (reader.NodeType.Equals(XmlNodeType.Whitespace)))
                {
                    readerActivo = reader.Read();
                }
                //Leo todas las entidades del fichero
                listaEntidades = LeerEntidades(pOntologia, reader);
                Dictionary<string, ElementoOntologia> entidadesPorId = listaEntidades.ToDictionary(item => item.ID, item => item);
                Dictionary<string, ElementoOntologia> entidadesPorUri = listaEntidades.ToDictionary(item => item.Uri, item => item);
                //Leo las entidades relacionadas con cada entidad secundaria
                foreach (ElementoOntologia entidad in listaEntidades)
                {
                    BuscarEntidadesRelacionadas(entidad, entidadesPorId, entidadesPorUri);
                }

                if (!pTraerEntidadesSuperiores)
                {
                    listaEntidadesSupOPrinc.Add(ObtenerEntidadPrincipal(listaEntidades));
                }
                else
                {
                    listaEntidadesSupOPrinc.AddRange(ObtenerElementosContenedorSuperiorOHerencias(listaEntidades));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("El fichero que está intentando importar no es válido:" + Environment.NewLine + ex.Message);
            }
            finally
            {
                reader.Close();
            }

            if (pValidarRdf)
            {
                NoPermitirNuevosElementos.AddRange(pOntologia.GestorOWL.NoPermitirNuevosElementos);

                if (NoPermitirNuevosElementos.Count > 0)
                {
                    string error = $"El RDF contiene elementos no permitidos en la ontología {pOntologia.GestorOWL.UrlOntologia}: {Environment.NewLine}";

                    foreach (string errorRdf in NoPermitirNuevosElementos)
                    {
                        error = string.Concat(error, errorRdf + Environment.NewLine);
                    }

                    throw new Exception(error);
                }

                NoPermitirNuevosElementos = null;
                pOntologia.GestorOWL.NoPermitirNuevosElementos = null;
            }

            return listaEntidadesSupOPrinc;
        }

        /// <summary>
        /// Busca en la lista de entidades extraída del fichero todas las entidades relacionadas con la entidad dada
        /// </summary>
        /// <param name="pEntidad">Entidad principal</param>
        /// <param name="pListaEntidades">Lista de entidades que contiene las entidades relacionadas.</param>
        private void BuscarEntidadesRelacionadas(ElementoOntologia pEntidad, Dictionary<string, ElementoOntologia> pEntidadesPorId, Dictionary<string, ElementoOntologia> pEntidadesPorUri)
        {
            List<Propiedad> entidades = pEntidad.ObtenerEntidadesRelacionadas();
            ElementoOntologia entidad = null;

            foreach (Propiedad propiedad in entidades)
            {
                propiedad.ElementoOntologia = pEntidad;
                if (!propiedad.FunctionalProperty)
                {
                    string[] claves = new string[propiedad.ListaValores.Keys.Count];
                    propiedad.ListaValores.Keys.CopyTo(claves, 0);
                    foreach (string valor in claves)
                    {
                        //Obtengo la entidad
                        //entidad = UtilImportarExportar.ObtenerEntidadPorID(valor, pListaEntidades);
                        entidad = UtilSemantica.ObtenerEntidadPorID(valor, pEntidadesPorId, pEntidadesPorUri);
                        //UtilImportarExportar.ObtenerNombreRealPropiedad(pEntidad, entidad, propiedad);
                        propiedad.ListaValores[valor] = entidad;
                        BuscarEntidadRelacionada(pEntidad, entidad, propiedad);
                    }
                }
                else
                {
                    // En CRFP se crean recursos sin todas sus propiedades funcionales. 
                    // Quito esta comprobación, aunque debería haber un parámetro para que se compruebe por defecto, salvo que esa configuración diga lo contrario. TODO
                    //if (string.IsNullOrEmpty(propiedad.UnicoValor.Key))
                    //{
                    //    throw new Exception($"La propiedad {propiedad.Nombre} es funcional y debe tener valor obligatoriamente");
                    //}
                    //entidad = UtilImportarExportar.ObtenerEntidadPorID(propiedad.UnicoValor.Key, pListaEntidades);
                    entidad = UtilSemantica.ObtenerEntidadPorID(propiedad.UnicoValor.Key, pEntidadesPorId, pEntidadesPorUri);
                    //UtilImportarExportar.ObtenerNombreRealPropiedad(pEntidad, entidad, propiedad);
                    propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(propiedad.UnicoValor.Key, entidad);
                    BuscarEntidadRelacionada(pEntidad, entidad, propiedad);
                }

                foreach (string idioma in propiedad.ListaValoresIdioma.Keys)
                {
                    foreach (string valor in propiedad.ListaValoresIdioma[idioma].Keys)
                    {
                        entidad = UtilSemantica.ObtenerEntidadPorID(valor, pEntidadesPorId, pEntidadesPorUri);
                        propiedad.ListaValoresIdioma[idioma][valor] = entidad;
                        BuscarEntidadRelacionada(pEntidad, entidad, propiedad);
                    }
                }
            }
        }

        /// <summary>
        /// Busca una entidad relacionada
        /// </summary>
        /// <param name="pEntidadPadre">Entidad padre</param>
        /// <param name="pEntidadRelacionada">Entidad relacionada</param>
        /// <param name="pPropiedad">Propiedad</param>
        protected virtual void BuscarEntidadRelacionada(ElementoOntologia pEntidadPadre, ElementoOntologia pEntidadRelacionada, Propiedad pPropiedad)
        {
            if (pEntidadRelacionada != null)
            {
                //La asigno a la principal
                pEntidadPadre.EntidadesRelacionadas.Add(pEntidadRelacionada);
                if (pPropiedad.PropiedadInversa != null)
                {
                    //Registro la propiedad en ambas entidades como opuesta una de la otra.
                    Propiedad propiedadInversa = UtilSemantica.ObtenerPropiedadDeNombre(pPropiedad.PropiedadInversa.Nombre, pEntidadRelacionada.Propiedades);
                    if ((propiedadInversa != null) && (!pPropiedad.ListaPropiedadesInversas.Contains(propiedadInversa)))
                    {
                        pPropiedad.ListaPropiedadesInversas.Add(propiedadInversa);
                        propiedadInversa.ListaPropiedadesInversas.Add(pPropiedad);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la entidad principal del fichero.
        /// </summary>
        /// <param name="pListaEntidades">Lista de entidades leída del fichero.</param>
        /// <returns></returns>
        protected static ElementoOntologia ObtenerEntidadPrincipal(List<ElementoOntologia> pListaEntidades)
        {
            //Ahora es la primera que se encuentra en el documento, pero es mejor coger el ID del DOCTYPE
            return pListaEntidades[0];
        }

        /// <summary>
        /// Obtiene la entidad superior la cual contiene jerárquicamente a todas las demás y no es contenida por ninguna.
        /// </summary>
        /// <param name="pListaEntidades">Lista de entidades leída del fichero.</param>
        /// <returns>Lista con las entidades superiores</returns>
        public static List<ElementoOntologia> ObtenerElementosContenedorSuperior(List<ElementoOntologia> pListaEntidades)
        {
            List<ElementoOntologia> listaEntidadesSuperiores = new List<ElementoOntologia>();

            foreach (ElementoOntologia entidad in pListaEntidades)
            {
                if (entidad.SuperclasesUtiles.Count == 0 && !PerteneceEntidadAlgunRangoPropiedades(entidad.TipoEntidad, pListaEntidades))
                {
                    listaEntidadesSuperiores.Add(entidad);
                }
            }
            return listaEntidadesSuperiores;
        }

        /// <summary>
        /// Obtiene la entidad superior la cual contiene jerárquicamente a todas las demás y no es contenida por ninguna, aun siendo entidad hija de otra.
        /// </summary>
        /// <param name="pListaEntidades">Lista de entidades leída del fichero.</param>
        /// <returns>Lista con las entidades superiores</returns>
        public static List<ElementoOntologia> ObtenerElementosContenedorSuperiorOHerencias(List<ElementoOntologia> pListaEntidades)
        {
            List<ElementoOntologia> listaEntidadesSuperiores = new List<ElementoOntologia>();

            foreach (ElementoOntologia entidad in pListaEntidades)
            {
                if (!PerteneceEntidadAlgunRangoPropiedades(entidad.TipoEntidad, pListaEntidades))
                {
                    if (entidad.SuperclasesUtiles.Count == 0)
                    {
                        listaEntidadesSuperiores.Add(entidad);
                    }
                    else
                    {
                        foreach (string superClase in entidad.SuperclasesUtiles)
                        {
                            if (!PerteneceEntidadAlgunRangoPropiedades(superClase, pListaEntidades))
                            {//Si la entidad padré no es hija de nadie es entidad principal.
                                listaEntidadesSuperiores.Add(entidad);
                                break;
                            }
                        }
                    }
                }
            }
            return listaEntidadesSuperiores;
        }

        /// <summary>
        /// Compueba si un tipo de entidad pertenece a un ragno de alguna propiedad.
        /// </summary>
        /// <param name="pTipoEntidad">Tipo de entidad a comprobar</param>
        /// <param name="pListaEntidades">Lista de entidades para comprobar</param>
        /// <returns>True si el tipo de entidad pertenece a algún rango, false en caso contrario</returns>
        private static bool PerteneceEntidadAlgunRangoPropiedades(string pTipoEntidad, List<ElementoOntologia> pListaEntidades)
        {
            foreach (ElementoOntologia entidad in pListaEntidades)
            {
                if (entidad.TipoEntidad != pTipoEntidad)
                {
                    foreach (Propiedad propiedad in entidad.Propiedades)
                    {
                        if (pTipoEntidad == propiedad.Rango)
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Construye una entidad a partir de otra.
        /// </summary>
        /// <param name="pEntidad">entidad a partir de la cual se creará la nueva entidad.</param>
        public virtual ElementoOntologia CrearElementoOntologia(ElementoOntologia pEntidad)
        {
            return new ElementoOntologia(pEntidad);
        }

        /// <summary>
        /// Crea una entidad a partir de un tipo de entidad.
        /// </summary>
        /// <param name="pTipoEntidad">tipo de entidad.</param>
        /// <param name="pNamespaceOntologia">Namespace de la ontología</param>
        /// <param name="pUrlOntologia">Url de la ontología</param>
        public virtual ElementoOntologia CrearElementoOntologia(string pTipoEntidad, string pUrlOntologia, string pNamespaceOntologia)
        {
            if (NoPermitirNuevosElementos != null && !Ontologia.TiposEntidades.Contains(pTipoEntidad))
            {
                NoPermitirNuevosElementos.Add("El tipo de entidad '" + pTipoEntidad + "' no pertenece a la ontología.");
            }

            return new ElementoOntologia(pTipoEntidad, pUrlOntologia, pNamespaceOntologia, Ontologia);
        }

        /// <summary>
        /// Lee todas las entidades del fichero gnoss.
        /// </summary>
        /// <param name="pOntologia">Ontología</param>
        /// <param name="pReader">reader del fichero</param>
        /// <returns></returns>
        protected List<ElementoOntologia> LeerEntidades(Ontologia pOntologia, XmlTextReader pReader)
        {
            List<ElementoOntologia> listaEntidades = new List<ElementoOntologia>();
            bool readerActivo = true;

            while (readerActivo)
            {
                if (pReader.Name.Equals("gnoss:Metadatos"))
                    pReader.Read();
                else
                {
                    ElementoOntologia entidad = null;
                    //int profundidad = pReader.Depth;

                    //El reader está apuntando a la siguiente entidad.
                    if (pReader.Name.Equals("rdf:Description"))
                    {
                        //Es una etiqueta, por lo que vendrán etiqueta/valor:
                        entidad = LeerEtiquetaRDFEntidad(pOntologia, pReader);
                    }
                    else if (!string.IsNullOrEmpty(pReader.Name) && !pReader.NodeType.Equals(XmlNodeType.None))
                    {
                        //Es un elemento, por lo que vendrán elemento/atributo:
                        entidad = LeerElementoRDFEntidad(pOntologia, pReader);
                    }
                    else
                    {
                        //El reader apunta a un elemento vacío, sigo leyendo
                        readerActivo = pReader.Read();
                    }

                    if (entidad != null)
                    {
                        listaEntidades.Add(entidad);
                    }
                }

                while ((readerActivo) && ((pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.EndElement))))
                {
                    readerActivo = pReader.Read();
                }
            }
            return listaEntidades;
        }

        private ElementoOntologia LeerEtiquetaRDFEntidad(Ontologia pOntologia, XmlTextReader pReader)
        { 
            //Lee etiquetas description
            string entidadID = null;
            string rdfAbout = null;
            int profundidad = pReader.Depth;
            if (pReader.HasAttributes)
            {
                pReader.MoveToAttribute("rdf:about");
                rdfAbout = pReader["rdf:about"];

                string ultimoCaracter = "/";
                if (rdfAbout.Contains("="))
                    ultimoCaracter = "=";

                entidadID = rdfAbout.Substring(rdfAbout.LastIndexOf(ultimoCaracter) + 1);
            }

            bool readerActivo = pReader.Read();
            while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
            {
                readerActivo = pReader.Read();
            }

            string nombreEntidad = null;
            if (pReader.Depth > profundidad)
            {
                //leo propiedades
                while ((readerActivo) && (pReader.Depth > profundidad))
                {
                    string nombrePropiedad = pReader.Name;
                    if (nombrePropiedad.Equals("rdf:type"))
                    {
                        readerActivo = pReader.Read();
                        nombreEntidad = pReader.Value;
                        break;
                    }
                    else
                    {
                        readerActivo = pReader.Read();
                    }
                }
            }

            if (string.IsNullOrEmpty(nombreEntidad))
            {
                throw new Exception($"La entidad {rdfAbout} no tiene rdf:type. ");
            }

            ElementoOntologia entidad = CrearElementoOntologia(pOntologia.GetEntidadTipo(nombreEntidad));
            entidad.ID = entidadID;
            entidad.Ontologia = pOntologia;

            readerActivo = pReader.Read();
            while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
            {
                readerActivo = pReader.Read();
            }

            if (pReader.Depth > profundidad)
            {
                //leo propiedades
                while ((readerActivo) && (pReader.Depth > profundidad))
                {
                    string nombrePropiedad = pReader.Name;
                    Propiedad propiedad = UtilSemantica.ObtenerPropiedadDeNombre(DesNamespacear(nombrePropiedad), entidad.Propiedades);

                    if (propiedad == null)
                    {
                        propiedad = UtilSemantica.ObtenerPropiedadDeNombre(nombrePropiedad, entidad.Propiedades);
                    }

                    if (propiedad != null)
                    {
                        propiedad.Ontologia = pOntologia;
                        propiedad.Visible = true;
                        while ((readerActivo) && (pReader.Depth > profundidad) && (pReader.Name.Equals(nombrePropiedad)))
                        {
                            if (!pReader.NodeType.Equals(XmlNodeType.EndElement))
                            {
                                string idioma = null;

                                if (propiedad.Tipo.Equals(TipoPropiedad.DatatypeProperty) && pReader.HasAttributes)
                                {
                                    idioma = pReader.GetAttribute("xml:lang");
                                }

                                pReader.Read();

                                string valor = pReader.Value;

                                //Eliminamos valores tabulaciones, saltos de linea, etc:
                                if (valor.Trim() == "")
                                {
                                    valor = "";
                                }

                                valor = ComprobarValorPropiedad(valor, propiedad);

                                if (propiedad.FunctionalProperty)
                                {
                                    if (propiedad.Tipo.Equals(TipoPropiedad.ObjectProperty) && !string.IsNullOrEmpty(propiedad.Rango) && pOntologia.TiposEntidades.Contains(propiedad.Rango))
                                    {
                                        string ultimoCaracter = "/";
                                        if (valor.Contains("="))
                                            ultimoCaracter = "=";

                                        valor = valor.Substring(valor.LastIndexOf(ultimoCaracter) + 1);
                                    }

                                    if (string.IsNullOrEmpty(idioma))
                                    {
                                        propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                                    }
                                    else
                                    {
                                        if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                                        {
                                            propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                        }

                                        if (!propiedad.ListaValoresIdioma[idioma].ContainsKey(valor))
                                        {
                                            propiedad.ListaValoresIdioma[idioma].Add(valor, null);
                                        }
                                    }
                                }
                                else
                                {
                                    if (propiedad.Tipo.Equals(TipoPropiedad.ObjectProperty) && !string.IsNullOrEmpty(propiedad.Rango) && pOntologia.TiposEntidades.Contains(propiedad.Rango))
                                    {
                                        string ultimoCaracter = "/";
                                        if (valor.Contains("="))
                                            ultimoCaracter = "=";

                                        valor = valor.Substring(valor.LastIndexOf(ultimoCaracter) + 1);
                                    }

                                    if (string.IsNullOrEmpty(idioma))
                                    {
                                        if (!propiedad.ListaValores.ContainsKey(valor))
                                        {
                                            propiedad.ListaValores.Add(valor, null);
                                        }
                                    }
                                    else
                                    {
                                        if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                                        {
                                            propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                        }

                                        if (!propiedad.ListaValoresIdioma[idioma].ContainsKey(valor))
                                        {
                                            propiedad.ListaValoresIdioma[idioma].Add(valor, null);
                                        }
                                    }
                                }

                                if (((propiedad.Nombre.ToLower().Equals("nombre")) || (propiedad.Nombre.ToLower().Equals("descripcion")) || (propiedad.Nombre.ToLower().Equals("titulo")) || (propiedad.Nombre.ToLower().Equals("descripción")) || (propiedad.Nombre.ToLower().Equals("título")) || (propiedad.Nombre.ToLower().Contains("nombreperfil"))) && (!string.IsNullOrEmpty(pReader.Value.Trim())))
                                {
                                    entidad.Descripcion = pReader.Value;
                                }
                            }

                            readerActivo = pReader.Read();
                            while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
                            {
                                readerActivo = pReader.Read();
                            }
                        }
                    }
                    else
                    {
                        if (NoPermitirNuevosElementos != null && nombrePropiedad != "rdfs:label" && nombrePropiedad != "rdf:type")
                        {
                            NoPermitirNuevosElementos.Add("La propiedad '" + nombrePropiedad + "' no pertenece a la entidad '" + entidad.TipoEntidad + "'.");
                        }

                        while ((readerActivo) && (!pReader.NodeType.Equals(XmlNodeType.EndElement)))
                        {
                            readerActivo = pReader.Read();
                        }
                        readerActivo = pReader.Read();
                        while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
                        {
                            readerActivo = pReader.Read();
                        }
                    }
                }
            }
            return entidad;
        }

        private ElementoOntologia LeerElementoRDFEntidad(Ontologia pOntologia, XmlTextReader pReader)
        {
            ElementoOntologia entidad = CrearElementoOntologia(pOntologia.GetEntidadTipo(pReader.Name));
            entidad.Ontologia = pOntologia;

            int profundidad = pReader.Depth;
            if (pReader.HasAttributes)
            {
                pReader.MoveToAttribute("rdf:about");
                entidad.ID = pReader["rdf:about"];

                string ultimoCaracter = "/";
                if (entidad.ID.Contains("="))
                    ultimoCaracter = "=";

                entidad.ID = entidad.ID.Substring(entidad.ID.LastIndexOf(ultimoCaracter) + 1);
            }

            bool readerActivo = pReader.Read();
            while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
            {
                readerActivo = pReader.Read();
            }

            if (pReader.Depth > profundidad)
            {
                //leo propiedades
                while ((readerActivo) && (pReader.Depth > profundidad))
                {
                    string nombrePropiedad = pReader.Name;
                    Propiedad propiedad = UtilSemantica.ObtenerPropiedadDeNombre(DesNamespacear(nombrePropiedad), entidad.Propiedades);

                    if (propiedad == null)
                    {
                        propiedad = UtilSemantica.ObtenerPropiedadDeNombre(nombrePropiedad, entidad.Propiedades);
                    }

                    if (propiedad != null)
                    {
                        propiedad.Ontologia = pOntologia;
                        propiedad.Visible = true;
                        while ((readerActivo) && (pReader.Depth > profundidad) && (pReader.Name.Equals(nombrePropiedad)))
                        {
                            if (!pReader.NodeType.Equals(XmlNodeType.EndElement))
                            {
                                string idioma = null;

                                if (propiedad.Tipo.Equals(TipoPropiedad.DatatypeProperty) && pReader.HasAttributes)
                                {
                                    idioma = pReader.GetAttribute("xml:lang");
                                }

                                pReader.MoveToAttribute(0);
                                if (pReader.Name.Equals("rdf:resource"))
                                {
                                    string valor = pReader.Value;

                                    //Eliminamos valores tabulaciones, saltos de linea, etc:
                                    if (valor.Trim() == "")
                                    {
                                        valor = "";
                                    }

                                    valor = ComprobarValorPropiedad(valor, propiedad);

                                    if (propiedad.FunctionalProperty)
                                    {
                                        if (propiedad.Tipo.Equals(TipoPropiedad.ObjectProperty) && !string.IsNullOrEmpty(propiedad.Rango) && pOntologia.TiposEntidades.Contains(propiedad.Rango))
                                        {
                                            string ultimoCaracter = "/";
                                            if (valor.Contains("="))
                                                ultimoCaracter = "=";

                                            valor = valor.Substring(valor.LastIndexOf(ultimoCaracter) + 1);
                                        }

                                        if (string.IsNullOrEmpty(idioma))
                                        {
                                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                                        }
                                        else
                                        {
                                            if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                                            {
                                                propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                            }

                                            if (!propiedad.ListaValoresIdioma[idioma].ContainsKey(valor))
                                            {
                                                propiedad.ListaValoresIdioma[idioma].Add(valor, null);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (propiedad.Tipo.Equals(TipoPropiedad.ObjectProperty) && !string.IsNullOrEmpty(propiedad.Rango) && pOntologia.TiposEntidades.Contains(propiedad.Rango))
                                        {
                                            string ultimoCaracter = "/";
                                            if (valor.Contains("="))
                                                ultimoCaracter = "=";

                                            valor = valor.Substring(valor.LastIndexOf(ultimoCaracter) + 1);
                                        }

                                        if (string.IsNullOrEmpty(idioma))
                                        {
                                            if (!propiedad.ListaValores.ContainsKey(valor))
                                            {
                                                propiedad.ListaValores.Add(valor, null);
                                            }
                                        }
                                        else
                                        {
                                            if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                                            {
                                                propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                            }

                                            if (!propiedad.ListaValoresIdioma[idioma].ContainsKey(valor))
                                            {
                                                propiedad.ListaValoresIdioma[idioma].Add(valor, null);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    pReader.Read();
                                    string valor = pReader.Value;

                                    valor = ComprobarValorPropiedad(valor, propiedad);

                                    if (string.IsNullOrEmpty(idioma))
                                    {
                                        if (propiedad.FunctionalProperty)
                                        {
                                            propiedad.UnicoValor = new KeyValuePair<string, ElementoOntologia>(valor, null);
                                        }
                                        else if (!propiedad.ListaValores.ContainsKey(valor))
                                        {
                                            propiedad.ListaValores.Add(valor, null);
                                        }
                                    }
                                    else
                                    {
                                        if (!propiedad.ListaValoresIdioma.ContainsKey(idioma))
                                        {
                                            propiedad.ListaValoresIdioma.Add(idioma, new Dictionary<string, ElementoOntologia>());
                                        }

                                        if (!propiedad.ListaValoresIdioma[idioma].ContainsKey(valor))
                                        {
                                            propiedad.ListaValoresIdioma[idioma].Add(valor, null);
                                        }
                                    }

                                    if (((propiedad.Nombre.ToLower().Equals("nombre")) || (propiedad.Nombre.ToLower().Equals("descripcion")) || (propiedad.Nombre.ToLower().Equals("titulo")) || (propiedad.Nombre.ToLower().Equals("descripción")) || (propiedad.Nombre.ToLower().Equals("título")) || (propiedad.Nombre.ToLower().Contains("nombreperfil"))) && (!string.IsNullOrEmpty(valor.Trim())))
                                    {
                                        entidad.Descripcion = valor;
                                    }
                                }
                            }

                            readerActivo = pReader.Read();
                            while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
                            {
                                readerActivo = pReader.Read();
                            }
                        }
                    }
                    else
                    {
                        if (NoPermitirNuevosElementos != null && nombrePropiedad != "rdfs:label" && nombrePropiedad != "rdf:type")
                        {
                            NoPermitirNuevosElementos.Add("La propiedad '" + nombrePropiedad + "' no pertenece a la entidad '" + entidad.TipoEntidad + "'.");
                        }

                        readerActivo = pReader.Read();
                        while ((readerActivo) && (!pReader.NodeType.Equals(XmlNodeType.EndElement)))
                        {
                            readerActivo = pReader.Read();
                        }
                        readerActivo = pReader.Read();
                        while ((readerActivo) && (pReader.NodeType.Equals(XmlNodeType.Whitespace)))
                        {
                            readerActivo = pReader.Read();
                        }
                    }
                }
            }
            return entidad;
        }

        /// <summary>
        /// Comprueba si el valor de una propiedad es correcto.
        /// </summary>
        /// <param name="pValor">Valor</param>
        /// <param name="pPropiedad">Propiedad</param>
        public string ComprobarValorPropiedad(string pValor, Propiedad pPropiedad)
        {
            if (NoPermitirNuevosElementos != null && pPropiedad.Tipo == TipoPropiedad.DatatypeProperty)
            {
                if (pPropiedad.RangoEsFecha)
                {
                    bool fechaOK = true;

                    try
                    {
                        string separador = null;
                        pValor = pValor.Trim();
                        int longitudMaxima = 14;

                        if (pValor.StartsWith("-"))
                        {
                            // Si empieza por un guión, es una fecha negativa
                            // separador se deja a null, la fecha debería llegar en el formato yyyyMMddHHmmss
                            longitudMaxima = 15;
                        }
                        else if (pValor.Contains("/"))
                        {
                            separador = "/";
                        }
                        else if (pValor.Contains("-"))
                        {
                            separador = "-";
                        }

                        if (!string.IsNullOrEmpty(separador))
                        {
                            string fecha = pValor;
                            string horaStr = string.Empty;
                            int hora = 0;
                            int min = 0;
                            int seg = 0;

                            if (pValor.Contains(" "))
                            {
                                horaStr = fecha.Substring(fecha.IndexOf(" ") + 1);
                                string[] horas = horaStr.Split(':');
                                fecha = fecha.Substring(0, fecha.IndexOf(" "));

                                hora = -1;
                                min = -1;
                                seg = -1;

                                if (!int.TryParse(horas[0], out hora) || !int.TryParse(horas[1], out min) || !int.TryParse(horas[2], out seg) || horas[0].Length != 2 || horas[1].Length != 2 || horas[2].Length != 2)
                                {
                                    fechaOK = false;
                                }
                            }

                            bool fechaInvertida = false;
                            string[] fechas = fecha.Split(separador[0]);

                            int anio = -1;
                            int mes = -1;
                            int dia = -1;

                            int posicionAnio = 2;
                            int posicionMes = 1;
                            int posicionDia = 0;

                            if (fechas[posicionDia].Length == 4)
                            {
                                fechaInvertida = true;
                                //La fecha está en formato yyyy/mm/dd
                                posicionAnio = 0;
                                posicionDia = 2;
                            }

                            if(fechas[posicionDia].Length < 2)
                            {
                                fechas[posicionDia] = "0" + fechas[posicionDia];
                            }
                            if (fechas[posicionMes].Length < 2)
                            {
                                fechas[posicionMes] = "0" + fechas[posicionMes];
                            }

                            if (!string.IsNullOrEmpty(horaStr))
                            {
                                horaStr = " " + horaStr;
                            }

                            if (fechaInvertida)
                            {
                                //formato 'yyyy/MM/dd', 'yyyy/MM/dd hh:mm:ss', 'yyyy-MM-dd', 'yyyy-MM-dd hh:mm:ss'
                                fecha = fechas[posicionAnio] + separador + fechas[posicionMes] + separador + fechas[posicionDia] + horaStr;
                            }
                            else
                            {
                                //formato 'dd/MM/yyyy', 'dd/MM/yyyy hh:mm:ss', 'dd-MM-yyyy', 'dd-MM-yyyy hh:mm:ss'
                                fecha = fechas[posicionDia] + separador + fechas[posicionMes] + separador + fechas[posicionAnio] + horaStr;
                            }
                            
                            if (!int.TryParse(fechas[posicionAnio], out anio) || !int.TryParse(fechas[posicionMes], out mes) || !int.TryParse(fechas[posicionDia], out dia) || fechas[posicionDia].Length != 2 || fechas[posicionMes].Length != 2 || fechas[posicionAnio].Length != 4)
                            {
                                fechaOK = false;
                            }

                            DateTime datetime = new DateTime(anio, mes, dia, hora, min, seg);

                            if (datetime.Year > 0 && fechaOK)
                            {
                                pValor = fecha;
                            }
                        }
                        else
                        {
                            long num = 0;
                            if (pValor.Length != longitudMaxima || !long.TryParse(pValor, out num))
                            {
                                fechaOK = false;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        fechaOK = false;
                    }

                    if (!fechaOK)
                    {
                        NoPermitirNuevosElementos.Add("La propiedad '" + pPropiedad.Nombre + "' con valor '" + pValor + "' no tiene un formato de fecha correcto. Debe ser uno de los siguientes: 'dd/MM/yyyy', 'dd/MM/yyyy hh:mm:ss', 'dd-MM-yyyy', 'dd-MM-yyyy hh:mm:ss', 'yyyy/MM/dd', 'yyyy/MM/dd hh:mm:ss', 'yyyy-MM-dd', 'yyyy-MM-dd hh:mm:ss', 'yyyyMMddhhmmss', ");
                    }
                }
                else if (pPropiedad.RangoEsEntero)
                {
                    long num = 0;
                    if (!long.TryParse(pValor, out num))
                    {
                        NoPermitirNuevosElementos.Add("La propiedad '" + pPropiedad.Nombre + "' con valor '" + pValor + "' debe ser un entero.");
                    }
                }
                else if (pPropiedad.RangoEsFloat)
                {
                    double num = 0;
                    if (!double.TryParse(pValor, out num))
                    {
                        NoPermitirNuevosElementos.Add("La propiedad '" + pPropiedad.Nombre + "' con valor '" + pValor + "' debe ser un número real.");
                    }
                }
            }

            return pValor;
        }

        /// <summary>
        /// Sustituye el namespace
        /// </summary>
        /// <param name="pPropiedad"></param>
        /// <returns></returns>
        public string DesNamespacear(string pPropiedad)
        {
            if (pPropiedad.Contains(":"))
            {
                string[] namesProp = pPropiedad.Split(':');

                if (NamespacesRDFLeyendo.ContainsKey(namesProp[0]))
                {
                    return NamespacesRDFLeyendo[namesProp[0]] + namesProp[1];
                }
            }

            return pPropiedad;
        }

        /// <summary>
        /// Incluye el RDF de un CV semántico en el actual.
        /// </summary>
        /// <param name="pFicheroXML">Fichero en el que se almacenará la entidad</param>
        /// <param name="pOntologia">Ontología</param>
        private static void IncluirRDFCVSemantico(XmlTextWriter pFicheroXML, Ontologia pOntologia)
        {
            pFicheroXML.WriteRaw(pOntologia.RDFCVSemIncluido);
        }

        #endregion

        #region Métodos privados estáticos

        /// <summary>
        /// Inicializa el gestor de OWL
        /// </summary>
        private static void InicializarGestionOWL()
        {
            GestionOWL.ListaEntidadesCreadas = null; ;
            GestionOWL.ListaEntidadesCreadas = new List<string>();
        }

        /// <summary>
        /// Posiciona al reader en el siguiente elemento.
        /// </summary>
        /// <param name="pReader">Reader XML</param>
        /// <param name="pReaderActivo">Indica si el reader está activo</param>
        /// <returns>TRUE si ha elementos para seguir leyendo, FALSE en caso contrario</returns>
        private static void PosicionarReaderSiguienteElemento(XmlTextReader pReader, ref bool pReaderActivo)
        {
            while ((pReaderActivo) && ((pReader.NodeType.Equals(XmlNodeType.EndElement)) || (pReader.NodeType.Equals(XmlNodeType.Whitespace)) || (pReader.NodeType.Equals(XmlNodeType.Comment))))
            {
                pReaderActivo = pReader.Read();
            }
        }

        #endregion

        #region Métodos publicos estáticos

        /// <summary>
        /// Recorre el listado de propiedades buscando la propiedad indicada
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad a recuperar</param>
        /// <param name="pElemento">Elemento contenedor de la propiedad</param>
        /// <returns>Propiedad buscada</returns>
        public static Propiedad ObtenerPropiedad(string pNombrePropiedad, ElementoOntologia pElemento)
        {
            if (pElemento != null)
            {
                //1º recorro todas las de la entidad actual por si la tiene esta.
                foreach (Propiedad propiedad in pElemento.Propiedades)
                {
                    if (propiedad.Nombre == pNombrePropiedad)
                    {
                        return propiedad;
                    }
                }

                //2º recorro todas las entidades hijas por si está en estas.
                foreach (Propiedad propiedad in pElemento.Propiedades)
                {
                    if (propiedad.Tipo == TipoPropiedad.ObjectProperty)
                    {
                        if (propiedad.FunctionalProperty)
                        {
                            Propiedad propiedadAux = ObtenerPropiedad(pNombrePropiedad, propiedad.UnicoValor.Value);
                            if (propiedadAux != null)
                            {
                                return propiedadAux;
                            }
                        }
                        else
                        {
                            foreach (ElementoOntologia elemento in propiedad.ListaValores.Values)
                            {
                                Propiedad propiedadAux = ObtenerPropiedad(pNombrePropiedad, elemento);
                                if (propiedadAux != null)
                                {
                                    return propiedadAux;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Recorre el listado de propiedades buscando la propiedad indicada de un tipo de entidad en particular.
        /// </summary>
        /// <param name="pNombrePropiedad">Nombre de la propiedad a recuperar</param>
        /// <param name="pTipoEntidad">Tipo de entidad</param>
        /// <param name="pElemento">Elemento contenedor de la propiedad</param>
        /// <returns>Propiedad buscada</returns>
        public static Propiedad ObtenerPropiedadDeTipoEntidad(string pNombrePropiedad, string pTipoEntidad, ElementoOntologia pElemento)
        {
            return Es.Riam.Semantica.Plantillas.EstiloPlantilla.ObtenerPropiedadACualquierNivelPorNombre(pNombrePropiedad, pTipoEntidad, pElemento);
        }

        /// <summary>
        /// Separa la URL de su elemento.
        /// </summary>
        /// <param name="pElemento">Elemento</param>
        /// <returns>Par con la url y el elemento</returns>
        public static KeyValuePair<string, string> SepararURLDeElemento(string pElemento)
        {
            string urlElemento = null;
            string elemento = null;

            if (pElemento.Contains("#"))
            {
                urlElemento = pElemento.Substring(0, pElemento.IndexOf("#") + 1);
                elemento = pElemento.Substring(pElemento.IndexOf("#") + 1);
            }
            else if (pElemento.Contains("/"))
            {
                urlElemento = pElemento.Substring(0, pElemento.LastIndexOf("/") + 1);
                elemento = pElemento.Substring(pElemento.LastIndexOf("/") + 1);
            }

            return new KeyValuePair<string, string>(urlElemento, elemento);
        }

        #endregion

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("NamespaceOntologia", mNamespaceOntologia);
            info.AddValue("NamespacesRDFLeyendo", mNamespacesRDFLeyendo);
            info.AddValue("NoPermitirNuevosElementos", mNoPermitirNuevosElementos);
            info.AddValue("TipoDatos", mTipoDatos);
            info.AddValue("URLIntragnoss", mURLIntragnoss);
            info.AddValue("UrlOntologia", mUrlOntologia);
        }

        #endregion
    }
}
