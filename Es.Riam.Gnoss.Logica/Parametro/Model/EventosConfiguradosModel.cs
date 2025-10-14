using Newtonsoft.Json;

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
    }
}
