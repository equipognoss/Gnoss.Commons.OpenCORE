using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models
{
    public class CrearCookieLoginModel
    {
        public Guid? usuarioID { get; set; }
        public bool MantenerConectado { get; set; }
        public string loginUsuario { get; set; }
        public string idioma { get; set; }
        public Guid personaID { get; set; }
        public string nombreCorto { get; set; }
        public string token { get; set; }
        public string redirect { get; set; }
        public bool? entrando { get; set; }
        public bool? eliminarCookie { get; set; }
    }
}
