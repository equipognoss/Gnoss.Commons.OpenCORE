using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AplicacionEspecificaViewModelEditar
    {
        public List<AplicacionEspecifica> aplicacionEspecificas { get; set; }
        public List<Variables> Variables { get; set; }
        public List<Volumen> Volumenes { get; set; }
        public List<VolumenAplicacion> VolumenesAplicacion { get; set; }

    }
}
