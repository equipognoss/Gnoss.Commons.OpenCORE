using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Routes
{
    public class PestanyaRouteModel
    {
        public Guid PestanyaID { get; set; }
        public short TipoPestanya { get; set; }
        public string RutaPestanya { get; set; }
        public string Idioma { get; set; }
    }
}
