using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecificaViewModel
    {
        public List<AplicacionEspecifica> AplicacionEspecificas { get; set; }
        public List<AplicacionEjecutable> AplicacionEjecutables { get; set; }
        public List<AplicacionEspecifica> Aplicaciones { get; set; }
        public List<Variables> Variables { get; set; }
        public List<Volumen> VolumenesCompartidos { get; set; }
        public List<VolumenAplicacion> VolumenesAplicacion { get; set; }
    }
}
