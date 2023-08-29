using Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOCVistas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC.ConfiguracionOCModel;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConfiguracionOC
{
    public class ConfiguracionOCPropiedadesModel
    {
    }
    public class Property
    {
        private protected string _name;
        private protected string _nameLectura;
        private protected string _domain;
        private protected string _uri;
        private protected bool _functional;
        private string _range;

        public string Name { get => _name; set => _name = value; }
        public string NameLectura { get => _nameLectura; set => _nameLectura = value; }
        public string Uri { get => _uri; set => _uri = value; }
        public bool Functional { get => _functional; set => _functional=value; }
        public string Domain { get => _domain; set => _domain = value; }
        public string Range { get => _range; set => _range=value; }
    }

    public class ObjectTypeProperty : Property
    {
        private string _referredProperty;
        private string _referredEntity;

        public string ReferredProperty { get => _referredProperty; set => _referredProperty = value; }
        public string ReferredEntity { get => _referredEntity; set => _referredEntity = value; }
    }
    public class AuxiliarProperty : ObjectTypeProperty
    {
    }
    public class ExternalProperty : ObjectTypeProperty
    {
        private string _referredGraph;
        private string _type;
        private bool _reciproca;
        private string _query;
        private string _extraWhere;
        
        private string _newTab;
        private string _typeSelection;
        private string _prosEdicion;
        private List<string> _propsLectura;

        public string ReferredGraph { get => _referredGraph; set => _referredGraph = value; }
        public string Type { get => _type; set => _type = value; }
        public bool Reciproca { get => _reciproca; set => _reciproca = value; }
        public string Query { get => _query; set => _query = value; }
        public string ExtraWhere { get => _extraWhere; set => _extraWhere = value; }
        
        public string NewTab { get => _newTab; set => _newTab = value; }
        public string TypeSelection { get => _typeSelection; set => _typeSelection = value; }
        public string PropEdicion { get => _prosEdicion; set => _prosEdicion = value; }
        public List<string> PropsLectura { get => _propsLectura; set => _propsLectura=value; }
    }

    public class DataTypeProperty : Property
    {
    }

    public class BooleanProperty : DataTypeProperty
    {
    }
    public class DateProperty : DataTypeProperty
    {
    }
    public class LinkProperty : DataTypeProperty
    {
    }
    public class NumericalProperty : DataTypeProperty
    {
    }
    public class StringProperty : DataTypeProperty
    {
        private string _typeString;
        private string _valorDefecto;
        private List<string> _valoresCombo;
        private bool _esPrincipal;
        private string _multiIdioma;
        private List<List<KeyValuePair<string, string>>> _miniaturas;
        private List<KeyValuePair<string, string>> _usarOpenSeaDragon;

        public string TypeString { get => _typeString; set => _typeString = value; }
        public List<string> ValoresCombo { get => _valoresCombo; set => _valoresCombo=value; }
        public string ValorDefecto { get => _valorDefecto; set => _valorDefecto=value; }
        public bool EsPrincipal { get => _esPrincipal; set => _esPrincipal=value; }
        public List<List<KeyValuePair<string, string>>> Miniaturas { get => _miniaturas; set => _miniaturas=value; }
        public List<KeyValuePair<string, string>> UsarOpenSeaDragon { get => _usarOpenSeaDragon; set => _usarOpenSeaDragon=value; }
        public string MultiIdioma { get => _multiIdioma; set => _multiIdioma=value; }
    }

}
