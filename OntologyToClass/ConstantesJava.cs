using Es.Riam.Gnoss.Util.GeneradorClases;

namespace OntologiaAClase
{
    class ConstantesJava
    {
        public string publicClass = "public class";
        public string required = "[Required]";
        public string publicList = "public List";
        public string maxLeng = "[MaxLength";
        public string minLeng = "[MinLength";
        public string publicSolo = "public";
        public string namespac = "namespace";

        internal string getter(string tipo, string nombrePropiedad) {
            return $"public {tipo} get{nombrePropiedad}(){{\n{UtilCadenasOntology.Tabs(3)} return this.{nombrePropiedad}; \n}}";
        }

        internal string setter(string tipo, string nombrePropiedad) {
            return $"public void set{nombrePropiedad}({tipo} tipo){{\n{UtilCadenasOntology.Tabs(3)}this.{nombrePropiedad} = tipo; \n}}";
        }
    }
}
