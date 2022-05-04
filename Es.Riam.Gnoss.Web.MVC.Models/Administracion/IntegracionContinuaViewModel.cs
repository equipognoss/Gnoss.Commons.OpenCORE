using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{

    public class IntegracionContinuaViewModel
    {

        public Guid ProyectoID { get; set; }
        public short TipoObjeto { get; set; }
        public string ObjetoPropiedad { get; set; }
        public short TipoPropiedad { get; set; }
        public string ValorPropiedad { get; set; }
        public string ValorPropiedadDestino { get; set; }
        public bool MismoValor { get; set; }
        public bool Revisada { get; set; }
        public string Nombre { get; set; }
    }

}
