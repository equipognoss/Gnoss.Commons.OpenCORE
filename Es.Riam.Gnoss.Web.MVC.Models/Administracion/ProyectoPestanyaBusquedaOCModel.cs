using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class ProyectoPestanyaBusquedaOCModel
    {
        public Guid PestanyaID { get; set; }
        public string NombrePestanya { get; set; }
        public string Url {  get; set; }
        public short TipoAutocompletar {  get; set; }
        public List<PesoSubtiposOntoModel> PesosSubtiposOntos { get; set; }
    }
}
