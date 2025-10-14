using Es.Riam.Open.Model;

namespace Es.Riam.InterfacesOpen
{
    public interface IPublishEvents
    {
        public abstract void PublishComments(ModificarComentarios modelo);
        public abstract void PublishResource(PublicarModificarEliminarRecurso modelo,string tipo);
        public abstract void PublishUser(ModificarUsuarios modelo);
        public abstract void CrearCola();
    }
}
