using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarIntegracionContinuaViewModel : PeticionApiBasica
    {
        public string Develop { get; set; }
        public string Release { get; set; }
        public string Master { get; set; }
        public string UrlRepo { get; set; }
        public string RepoClave { get; set; }
        public string RepoSecret { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string Repositorio { get; set; }
        public string NombreCortoProyecto { get; set; }
        public Guid EntornoIDDevelop { get; set; }
        public Guid EntornoIDRelease { get; set; }
        public Guid EntornoIDMaster { get; set; }
        public string URLAPIIntegracionContinua { get; set; }
        public bool Iniciado { get; set; }
        public string NombrePlataforma { get; set; }

        public string UrlLogin { get; set; }

    }
}
