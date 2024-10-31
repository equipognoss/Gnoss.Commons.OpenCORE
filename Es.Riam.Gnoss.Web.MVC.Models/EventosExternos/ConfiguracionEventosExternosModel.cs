using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.EventosExternos
{
    public class ConfiguracionEventosExternosModel
    {
        public string Usuario {  get; set; }
        public string Password {  get; set; }
        public bool PublicarRecursos { get; set; }
        public bool PublicarComentarios { get; set; }
        public bool PublicarUsuarios { get; set; }
    }
}
