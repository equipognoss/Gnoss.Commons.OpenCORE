using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEjecutable
    {

        public string Output { get; set; }
        public string Error { get; set; }
        public Guid ApplicacionID { get; set; }
        public Guid ApplicacionEjecucionID { get; set; }
        public short Estado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

    }
}
