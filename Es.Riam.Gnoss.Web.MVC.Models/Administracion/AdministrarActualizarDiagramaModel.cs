using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarActualizarDiagramaModel
    {
        public Dictionary<string, string> ClasesRenombradas { get; set; }
        public Dictionary<string, List<PropiedadClase>> PropiedadesClases { get; set; }
        public List<Relacion> Relaciones { get; set; }
    }

    public class PropiedadClase
    {
        public string NombrePropiedad { get; set; }
        public string Cardinalidad { get; set; }
        public string Rango { get; set; }
    }

    public class Relacion
    {
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Tipo { get; set; }
        public string Predicado { get; set; }
    }
}
