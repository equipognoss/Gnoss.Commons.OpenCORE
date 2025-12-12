using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Flujos
{
    public class ColaProcesarFlujo
    {
        public ColaProcesarFlujo() { }


        public Guid FlujoID { get; set; }
        public Guid ProyectoID { get; set; }
        public Guid? EstadoID { get; set; }
        public Guid UsuarioID { get; set; }
        public List<Guid> OntologiasAfectadas { get; set; }
        public List<TiposContenidos> TiposAfectados { get; set; }
        public bool EliminarEstado { get; set; }
        public bool EliminarFlujo { get; set; }
    }
}
