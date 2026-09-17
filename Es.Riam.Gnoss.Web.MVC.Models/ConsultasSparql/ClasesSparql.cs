using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.ConsultasSparql
{
    public class Root
    {
        public Head head { get; set; }
        public Results results { get; set; }
    }
    public class Head
    {
        public List<object> link { get; set; }
        public List<string> vars { get; set; }
    }
    public class Results
    {
        public bool distinct { get; set; }
        public bool ordered { get; set; }
        public List<Dictionary<string, Data>> bindings { get; set; }
    }
    public class Data
    {
        public string type { get; set; }
        public string value { get; set; }
        public string datatype { get; set; }
    }
}
