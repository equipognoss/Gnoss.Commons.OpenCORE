
namespace Es.Riam.Gnoss.Logica.Parametro.Model
{
    public class EventosConfiguradosModel
    {
        public bool EventsActive { get; set; }
        public EventsConfiguration EventsConfiguration { get; set; }
    }

    public class EventsConfiguration
    {
        public bool ResourcesActive { get; set; }
        public bool CommentsActive { get; set; }
        public bool UsersActive { get; set; }
        public bool TranslationsActive { get; set; }
        public bool PagesCmsActive { get; set; }
        public bool ComponentsCmsActive { get; set; }
    }

    public enum TipoEventoExterno
    {
        Resource,
        Comment,
        User,
        Translation,
        PageCms,
        ComponentCms
    }
}
