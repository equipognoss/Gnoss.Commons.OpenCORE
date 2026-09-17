using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class TraducirRecursosViewModel
    {
        public Dictionary<Guid, string> ObjetosConocimiento { get; set; }
        public Dictionary<string, string> IdiomasDisponibles { get; set; }
        public bool TraductorConfigurado { get; set; }
    }
}
