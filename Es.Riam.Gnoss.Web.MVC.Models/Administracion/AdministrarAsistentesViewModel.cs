using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarAsistentesViewModel
    {
        public List<AsistenteViewModel> Asistentes { get; set; } = new List<AsistenteViewModel>();
        public Dictionary<string, string> Idiomas { get; set; }
        public string IdiomaPorDefecto { get; set; }
    }

    public class AsistenteViewModel
    {
        public Guid AsistenteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Token {  get; set; }
        public string HostAsistente { get; set; }
        public string Icono { get; set; }
        public bool Activo { get; set; }
        public bool Nuevo { get; set; }
        public Dictionary<Guid, string> Roles { get; set; } = new Dictionary<Guid, string>();
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
        public Dictionary<Guid, string> RolesDisponibles { get; set; } = new Dictionary<Guid, string>();
    }
}
