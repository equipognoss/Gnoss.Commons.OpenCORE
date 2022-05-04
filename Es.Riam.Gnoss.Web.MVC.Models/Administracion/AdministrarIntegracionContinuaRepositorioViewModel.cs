using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarIntegracionContinuaRepositorioViewModel : PeticionApiBasica
    {
        public string Repositorio { get; set; }
        public bool RepositorioActualizado { get; set; }

    }
}
