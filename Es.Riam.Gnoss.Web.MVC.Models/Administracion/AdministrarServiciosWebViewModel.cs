using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarServiciosWebViewModel : PeticionApiBasica
    {
        public List<ProyectoServicioWeb> ServiciosWeb { get; set; }

    }
}
