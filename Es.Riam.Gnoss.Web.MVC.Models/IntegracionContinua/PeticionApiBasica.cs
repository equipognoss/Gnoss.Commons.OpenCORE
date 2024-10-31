using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua
{
    [Serializable]
    public class PeticionApiBasica
    {
        public Guid Entorno { get; set; }
        public Guid Usuario { get; set; }
        public Guid Proyeto { get; set; }
        public string Error { get; set; }
    }
}
