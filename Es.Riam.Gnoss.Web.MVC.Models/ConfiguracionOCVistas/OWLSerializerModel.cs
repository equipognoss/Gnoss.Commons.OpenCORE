using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOCVistas
{
    public class OWLSerializerModel
    {
    }

    [XmlRoot(ElementName = "Ontology", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class Ontology
    {

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }
    }

    [XmlRoot(ElementName = "Class", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class Class
    {

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }
    }

    [XmlRoot(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
    public class Domain
    {

        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }
    }

    [XmlRoot(ElementName = "ObjectProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class ObjectProperty
    {

        [XmlElement(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Domain Domain { get; set; }

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }

        [XmlElement(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Range Range { get; set; }
    }

    [XmlRoot(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
    public class Range
    {

        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }
    }

    [XmlRoot(ElementName = "DatatypeProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class DatatypeProperty
    {

        [XmlElement(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Range Range { get; set; }

        [XmlElement(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Domain Domain { get; set; }

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }
    }

    [XmlRoot(ElementName = "type", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public class Type
    {
        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }

        [XmlAttribute(AttributeName = "prueba", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Prueba { get; set; }
    }

    [XmlRoot(ElementName = "FunctionalProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class FunctionalProperty
    {
        [XmlElement(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Range Range { get; set; }

        [XmlElement(ElementName = "type", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public Type Type { get; set; }

        [XmlElement(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Domain Domain { get; set; }

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }
    }

    [XmlRoot(ElementName = "RDF", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public class RDF
    {

        [XmlElement(ElementName = "Ontology", Namespace = "http://www.w3.org/2002/07/owl#")]
        public Ontology Ontology { get; set; }

        [XmlElement(ElementName = "Class", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<Class> Class { get; set; }

        [XmlElement(ElementName = "ObjectProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<ObjectProperty> ObjectProperty { get; set; }

        [XmlElement(ElementName = "DatatypeProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<DatatypeProperty> DatatypeProperty { get; set; }

        [XmlElement(ElementName = "FunctionalProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<FunctionalProperty> FunctionalProperty { get; set; }

        [XmlAttribute(AttributeName = "rdf", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Rdf { get; set; }

        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }

        [XmlAttribute(AttributeName = "rdfs", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Rdfs { get; set; }

        [XmlAttribute(AttributeName = "owl", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Owl { get; set; }

        [XmlAttribute(AttributeName = "xmlns", Namespace = "")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "base", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Base { get; set; }
    }
}

