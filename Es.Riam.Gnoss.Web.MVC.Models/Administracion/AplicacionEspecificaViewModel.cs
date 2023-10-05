using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecificaViewModel
    {
        public List<AplicacionEspecifica> aplicacionEspecificas { get; set; }
        public List<AplicacionEjecutable> aplicacionEjecutables { get; set; }
        public List<AplicacionEspecifica> Aplicaciones { get; set; }
        public List<Variables> Variables { get; set; }
        public List<Volumen> Volumenes { get; set; }
        public List<VolumenAplicacion> VolumenesAplicacion { get; set; }

        public string UrlRepositorio { get; set; }

    }
}
