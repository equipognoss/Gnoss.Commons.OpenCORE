using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    [Serializable]
    public class AdministrarRamasViewModel : PeticionApiBasica
    {
        public string Nombre { get; set; }
        public string Estado { get; set; }
        public bool EsFusionable { get; set; }
        public List<string> TodasLasRamasRepositorio { get; set; }
        public bool PeticionCreacion { get; set; }
        public bool PeticionCerrar { get; set; }

        public AdministrarRamasViewModel()
        {
            TodasLasRamasRepositorio = new List<string>();
        }
    }
}

