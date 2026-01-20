using Es.Riam.InterfacesOpen.Model;
using Es.Riam.Open.Model;

namespace Es.Riam.InterfacesOpen
{
    public interface IPublishEvents
    {
        public abstract void PublishComments(ModificarComentarios pModelo);
        public abstract void PublishResource(PublicarModificarEliminarRecurso pModelo, string pTipo);
        public abstract void PublishUser(ModificarUsuarios pModelo);
        public abstract void PublishTranslation(TranslationError pModelo);
        public abstract void CrearCola();
    }
}
