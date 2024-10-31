using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC
{
    public class ConfiguracionOCModel
    {
        private XmlSerializer xmlSerializer;
        private string xmlPath;
        private XMLModel xmlFile;

        private XmlSerializer owlSerializer;
        private string owlPath;
        private RDF owlFile;

        private List<Property> properties;
        private Dictionary<string, string> prefixes;
        private List<string> listaOntologias;
        private Dictionary<string, Dictionary<string, List<string>>> listaOntologiasPropiedades;

        public XmlSerializer Serializer { get => xmlSerializer; set => xmlSerializer = value; }

        public string XmlPath { get => xmlPath; set => xmlPath = value; }
        public XMLModel XmlFile { get => xmlFile; set => xmlFile = value; }
        public XmlSerializer OwlSerializer { get => owlSerializer; set => owlSerializer = value; }
        public string OwlPath { get => owlPath; set => owlPath = value; }
        public RDF OwlFile { get => owlFile; set => owlFile = value; }
        public List<Property> Properties { get => properties; set => properties = value; }
        public Dictionary<string, string> Prefixes { get => prefixes; set => prefixes=value; }
        public List<string> ListaOntologias { get; set; }
        public Dictionary<string, Dictionary<string, List<string>>> ListaOntologiasPropiedades = new Dictionary<string, Dictionary<string, List<string>>>();

        public ConfiguracionOCModel(XmlReader pXmlReader, XmlReader pOwlReader)
        {
            xmlSerializer = new XmlSerializer(typeof(XMLModel));
            xmlFile = new XMLModel();

            using (XmlReader xmlReader = pXmlReader)
            {
                xmlFile = (XMLModel)xmlSerializer.Deserialize(xmlReader);
            }

            owlSerializer = new XmlSerializer(typeof(RDF));
            owlFile = new RDF();

            using (XmlReader owlReader = pOwlReader)
            {
                owlFile = (RDF)owlSerializer.Deserialize(owlReader);
            }

            properties = new List<Property>();
            Prefixes = new Dictionary<string, string> ();
            string entidadImagenPrincipal = xmlFile.ConfiguracionGeneral.ImagenDoc != null ? xmlFile.ConfiguracionGeneral.ImagenDoc.EntidadID : "";
            string propiedadImagenPrincipal = xmlFile.ConfiguracionGeneral.ImagenDoc != null ? xmlFile.ConfiguracionGeneral.ImagenDoc.Text : "";
            storeProperties(entidadImagenPrincipal, propiedadImagenPrincipal);
            ListaOntologias = new List<string> ();
        }
        private void storeProperties(string entidadIDImagen, string propiedadImagen)
        {
            List<ObjectProperty> objectProperties = OwlFile.ObjectProperty;
            List<DatatypeProperty> dataTypeProperties = OwlFile.DatatypeProperty;
            List<FunctionalProperty> functionalProperties = OwlFile.FunctionalProperty;

            foreach (PropiedadXML xmlProperty in xmlFile.EspefPropiedad.Propiedad)
            {
                //Miramos en qué lista está la propiedad
                if (objectProperties.Where(x => x.About.Equals(xmlProperty.ID)).ToList().Count > 0)
                {
                    string range = objectProperties.Where(x => x.About.Equals(xmlProperty.ID) && x.Range != null).Select(x => x.Range.Resource).FirstOrDefault();
                    if (!string.IsNullOrEmpty(range))
                    {
                        Properties.Add(createAuxiliarProperty(xmlProperty, objectProperties, functionalProperties));
                    }
                    else
                    {
                        Properties.Add(createExternalProperty(xmlProperty));
                    }
                }
                else if (dataTypeProperties.Where(x => x.About.Equals(xmlProperty.ID)).ToList().Count > 0)
                {
                    string range = "";
                    try
                    {
						range = dataTypeProperties.Where(x => x.About.Equals(xmlProperty.ID)).Select(x => x.Range.Resource).FirstOrDefault();
					}
                    catch
                    {
                        //La propiedad no tiene rango definido
                        range = "";
                    }
                    
                    switch (range)
                    {
                        case "http://www.w3.org/2001/XMLSchema#int":
                            Properties.Add(createNumericalProperty(xmlProperty));
                            break;
                        case "http://www.w3.org/2001/XMLSchema#boolean":
                            Properties.Add(createBooleanProperty(xmlProperty));
                            break;
                        case "http://www.w3.org/2001/XMLSchema#string":
                            Properties.Add(createStringProperty(xmlProperty, entidadIDImagen, propiedadImagen));
                            break;
                        case "http://www.w3.org/2001/XMLSchema#date":
                            Properties.Add(createDateProperty(xmlProperty));
                            break;
                        default: //no tiene rango definido
                            Properties.Add(createProperty(xmlProperty));
                            break;
                    }
                }
                else if (functionalProperties.Where(x => x.About.Equals(xmlProperty.ID)).ToList().Count > 0)
                {
                    string type = functionalProperties.Where(x => x.About.Equals(xmlProperty.ID)).Select(x => x.Type.Resource).FirstOrDefault();
                    string range = functionalProperties.Where(x => x.About.Equals(xmlProperty.ID) && x.Range != null).Select(x => x.Range.Resource).FirstOrDefault();
                    switch (type)
                    {
                        case "http://www.w3.org/2002/07/owl#DatatypeProperty":
                            switch (range)
                            {
                                case "http://www.w3.org/2001/XMLSchema#int":
                                    Properties.Add(createNumericalProperty(xmlProperty, true));
                                    break;
                                case "http://www.w3.org/2001/XMLSchema#boolean":
                                    Properties.Add(createBooleanProperty(xmlProperty, true));
                                    break;
                                case "http://www.w3.org/2001/XMLSchema#string":
                                    Properties.Add(createStringProperty(xmlProperty, entidadIDImagen, propiedadImagen, true));
                                    break;
								case "http://www.w3.org/2001/XMLSchema#date":
									Properties.Add(createDateProperty(xmlProperty));
									break;
								default: //no tiene rango definido
									Properties.Add(createProperty(xmlProperty));
									break;
							}
                            break;
                        default:
                            if (!string.IsNullOrEmpty(range))
                            {
                                Properties.Add(createAuxiliarProperty(xmlProperty, objectProperties, functionalProperties, true));
                            }
                            else
                            {
                                Properties.Add(createExternalProperty(xmlProperty, true));
                            }
                            break;
                    }
                }
            }
        }

        private List<string> createListaString(ValoresCombo valoresCombo)
        {
            List<string> lista = new List<string>();
            foreach (string valor in valoresCombo.valorCombo)
            {
                lista.Add(valor);
            }
            return lista;
        }

		private Property createProperty(PropiedadXML xml, bool functional = false)
		{
			return new Property
			{
				Name = xml.AtrNombre.Text,
				NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
				Domain = xml.EntidadID,
				Uri = xml.ID,
				Functional = functional
			};
		}


		private NumericalProperty createNumericalProperty(PropiedadXML xml, bool functional = false)
        {
            return new NumericalProperty
            {
                Name = xml.AtrNombre.Text,
                NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
                Domain = xml.EntidadID,
                Uri = xml.ID,
                Functional = functional,
                Range = "http://www.w3.org/2001/XMLSchema#int"
            };
        }

        private BooleanProperty createBooleanProperty(PropiedadXML xml, bool functional = false)
        {
            return new BooleanProperty
            {
                Name = xml.AtrNombre.Text,
                NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
                Uri = xml.ID,
                Domain = xml.EntidadID,
                Functional = functional,
                Range = "http://www.w3.org/2001/XMLSchema#boolean"
            };
        }

        private DateProperty createDateProperty(PropiedadXML xml, bool functional = false)
        {
            return new DateProperty
            {
                Name = xml.AtrNombre.Text,
                NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
                Uri = xml.ID,
                Domain = xml.EntidadID,
                Functional = functional,
                Range = "http://www.w3.org/2001/XMLSchema#date"
            };
        }

        private StringProperty createStringProperty(PropiedadXML xml, string entidadIDImagen, string propiedadImagen, bool functional = false)
        {
            List<List<KeyValuePair<string, string>>> miniaturas = new List<List<KeyValuePair<string, string>>>();
            if (xml.ImgMiniVP != null)
            {
                foreach (Size m in xml.ImgMiniVP.Size)
                {
                    List<KeyValuePair<string,string>> miniatura = new List<KeyValuePair<string, string>>();
                    KeyValuePair<string,string> ancho = new KeyValuePair<string, string> ( "ancho", m.Ancho.ToString() );
                    KeyValuePair<string,string> alto = new KeyValuePair<string, string> ( "alto", m.Alto.ToString() );
                    KeyValuePair<string, string> tipo = new KeyValuePair<string, string>();
                    if (m.Tipo != null)
                    {
                        tipo = new KeyValuePair<string, string>("tipo", m.Tipo);
                    }
                    else
                    {
                        tipo = new KeyValuePair<string, string>("tipo", "Redimensión");
                    }
                    miniatura.Add( ancho);
                    miniatura.Add( alto );
                    miniatura.Add( tipo );
                    miniaturas.Add(miniatura);
                }
            }
            List<KeyValuePair<string, string>> openSeaDragon = null;
            if (xml.UsarOpenSeaDragon != null)
            {
                openSeaDragon = new List<KeyValuePair<string, string>>();
                KeyValuePair<string, string> propiedadAncho = new KeyValuePair<string, string>("PropiedadAnchoID", xml.UsarOpenSeaDragon.PropiedadAnchoID);
                KeyValuePair<string, string> propiedadAlto = new KeyValuePair<string, string>("PropiedadAltoID", xml.UsarOpenSeaDragon.PropiedadAltoID);
                openSeaDragon.Add(propiedadAncho);
                openSeaDragon.Add(propiedadAlto);

            }
            return new StringProperty
            {
                Name = xml.AtrNombre.Text,
                NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
                Uri = xml.ID,
                Domain = xml.EntidadID,
                Functional = functional,
                Range = "http://www.w3.org/2001/XMLSchema#string",
                TypeString = xml.TipoCampo != null ? xml.TipoCampo.ToString() : "String",
                ValoresCombo = xml.valoresCombo != null ? createListaString(xml.valoresCombo) : null,
                ValorDefecto = xml.valoresCombo != null ? xml.valoresCombo.valorDefecto : "",
                EsPrincipal = entidadIDImagen.Equals(xml.EntidadID) && propiedadImagen.Equals(xml.ID),
                Miniaturas = miniaturas,
                UsarOpenSeaDragon = openSeaDragon,
                MultiIdioma = xml.MultiIdioma != null ? xml.MultiIdioma : ""
            };
        }

        

        private AuxiliarProperty createAuxiliarProperty(PropiedadXML xml, List<ObjectProperty> objectProperties, List<FunctionalProperty> functionalProperties, bool functional = false)
        {
            string range = functionalProperties.Where(x => x.About.Equals(xml.ID) && x.Range != null).Select(x => x.Range.Resource).FirstOrDefault();
            if (string.IsNullOrEmpty(range))
            {
                range = objectProperties.Where(x => x.About.Equals(xml.ID)).Select(x => x.Range.Resource).FirstOrDefault();
            }
            return new AuxiliarProperty
            {
                Name = xml.AtrNombre.Text,
                NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
                Uri = xml.ID,
                Functional = functional,
                Range = range,
                Domain = xml.EntidadID,
                ReferredProperty = ""
            };
        }

        private ExternalProperty createExternalProperty(PropiedadXML xml, bool functional = false)
        {
            return new ExternalProperty
            {
                Name = xml.AtrNombre.Text,
                NameLectura = xml.AtrNombreLectura.FirstOrDefault().Text,
                Uri = xml.ID,
                Functional = functional,
                Domain = xml.EntidadID,
                NewTab = xml.SeleccionEntidad != null ? (xml.SeleccionEntidad.NuevaPestanya ? "true" : "false") : "",
                TypeSelection = xml.SeleccionEntidad != null ? xml.SeleccionEntidad.TipoSeleccion : "",
                ReferredGraph = xml.SeleccionEntidad != null ? xml.SeleccionEntidad.Grafo : "",
                ReferredEntity = xml.SeleccionEntidad != null ? xml.SeleccionEntidad.UrlTipoEntSolicitada : "",
                PropEdicion = xml.SeleccionEntidad != null && xml.SeleccionEntidad.PropsEntEdicion != null ? xml.SeleccionEntidad.PropsEntEdicion.NameProp : "",
                PropsLectura = xml.SeleccionEntidad != null && xml.SeleccionEntidad.PropsEntLectura != null ? xml.SeleccionEntidad.PropsEntLectura.Propiedad.Select(x => x.ID).ToList() : new List<string>(),
                Reciproca = xml.SeleccionEntidad != null && xml.SeleccionEntidad.Reciproca != null
            };


        }
        

    }
}
