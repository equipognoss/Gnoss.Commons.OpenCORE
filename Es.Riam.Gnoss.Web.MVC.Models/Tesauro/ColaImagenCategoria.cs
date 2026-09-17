using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Tesauro
{
    public class ColaImagenCategoria
    {
        public Guid CategoriaID { get; set; }
        public string NombreImagenOriginal { get; set; }
        public Guid ProyectoID { get; set; }
    }
}
