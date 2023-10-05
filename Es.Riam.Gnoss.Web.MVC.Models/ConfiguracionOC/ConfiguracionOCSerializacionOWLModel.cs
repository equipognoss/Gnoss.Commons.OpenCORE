using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC
{
    public class ConfiguracionOCSerializacionOWLModel
    {
    }

    [XmlRoot(ElementName = "maxCardinality", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class MaxCardinality
    {

        [XmlAttribute(AttributeName = "datatype", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Datatype { get; set; }

        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "DatatypeProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class DatatypeProperty
    {

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }

        [XmlElement(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Range Range { get; set; }

        [XmlElement(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Domain Domain { get; set; }

        [XmlElement(ElementName = "label", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Label Label { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "onProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class OnProperty
    {

        [XmlElement(ElementName = "DatatypeProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public DatatypeProperty DatatypeProperty { get; set; }

        [XmlElement(ElementName = "ObjectProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public ObjectProperty ObjectProperty { get; set; }
    }

    [XmlRoot(ElementName = "Restriction", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class Restriction
    {

        [XmlElement(ElementName = "maxCardinality", Namespace = "http://www.w3.org/2002/07/owl#")]
        public MaxCardinality MaxCardinality { get; set; }

        [XmlElement(ElementName = "onProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public OnProperty OnProperty { get; set; }

        [XmlElement(ElementName = "minCardinality", Namespace = "http://www.w3.org/2002/07/owl#")]
        public MinCardinality MinCardinality { get; set; }
    }

    [XmlRoot(ElementName = "subClassOf", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
    public class SubClassOf
    {

        [XmlElement(ElementName = "Restriction", Namespace = "http://www.w3.org/2002/07/owl#")]
        public Restriction Restriction { get; set; }

        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }
    }

    [XmlRoot(ElementName = "Class", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class Class
    {

        [XmlElement(ElementName = "subClassOf", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public List<SubClassOf> SubClassOf { get; set; }

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlElement(ElementName = "unionOf")]
        public UnionOf UnionOf { get; set; }
    }

    [XmlRoot(ElementName = "ObjectProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class ObjectProperty
    {

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }

        [XmlElement(ElementName = "range", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public Range Range { get; set; }

        [XmlElement(ElementName = "domain", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public Domain Domain { get; set; }
    }

    [XmlRoot(ElementName = "minCardinality", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class MinCardinality
    {

        [XmlAttribute(AttributeName = "datatype", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Datatype { get; set; }

        [XmlText]
        public int Text { get; set; }
    }

    [XmlRoot(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
    public class Range
    {

        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }
    }

    [XmlRoot(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
    public class Domain
    {

        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }

        [XmlElement(ElementName = "Class", Namespace = "http://www.w3.org/2002/07/owl#")]
        public Class Class { get; set; }
    }

    [XmlRoot(ElementName = "label", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
    public class Label
    {

        [XmlAttribute(AttributeName = "datatype", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Datatype { get; set; }

        [XmlText]
        public string Text { get; set; }

        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
    }

    [XmlRoot(ElementName = "type")]
    public class TypeOWL
    {

        [XmlAttribute(AttributeName = "resource", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string Resource { get; set; }
    }

    [XmlRoot(ElementName = "FunctionalProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
    public class FunctionalProperty
    {

        [XmlElement(ElementName = "domain", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Domain Domain { get; set; }

        [XmlElement(ElementName = "type", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public TypeOWL Type { get; set; }

        [XmlAttribute(AttributeName = "about", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
        public string About { get; set; }   

        [XmlElement(ElementName = "label", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Label Label { get; set; }

        [XmlElement(ElementName = "range", Namespace = "http://www.w3.org/2000/01/rdf-schema#")]
        public Range Range { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "unionOf")]
    public class UnionOf
    {

        [XmlElement(ElementName = "Class", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<Class> Class { get; set; }

        [XmlAttribute(AttributeName = "parseType")]
        public string ParseType { get; set; }
    }

    [XmlRoot(ElementName = "RDF", Namespace = "http://www.w3.org/1999/02/22-rdf-syntax-ns#")]
    public class RDF
    {
        [XmlElement(ElementName = "Class", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<Class> Class { get; set; }

        [XmlElement(ElementName = "ObjectProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<ObjectProperty> ObjectProperty { get; set; }

        [XmlElement(ElementName = "DatatypeProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<DatatypeProperty> DatatypeProperty { get; set; }

        [XmlElement(ElementName = "FunctionalProperty", Namespace = "http://www.w3.org/2002/07/owl#")]
        public List<FunctionalProperty> FunctionalProperty { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
