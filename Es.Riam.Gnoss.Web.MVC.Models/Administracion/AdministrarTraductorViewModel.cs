using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarTraductorViewModel
    {
        public Guid ProyectoID { get; set; }
        public string Token { get; set; }
        public string Endpoint { get; set; }
        public string Nivel { get; set; }
        public string Prompt { get; set; }
        public bool Activo { get; set; }     
        public string TextoTraducir { get; set; }
        public string IdiomaOrigen { get; set; }
        public string IdiomaTraducir { get; set; }
    }
}
