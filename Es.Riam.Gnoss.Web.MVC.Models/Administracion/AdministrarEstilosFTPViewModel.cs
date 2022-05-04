using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarEstilosFTPViewModel : PeticionApiBasica
    {
        public string IP { get; set; }
        public string Puerto { get; set; }

    }
}
