using Es.Riam.InterfacesOpen.Model;
using Es.Riam.Open.Model;

namespace Es.Riam.InterfacesOpen
{
    public interface IPublishEvents
    {
        public abstract void PublishComments(CommentModifyEvent pModelo);
        public abstract void PublishResource(ResourceEvent pModelo, ActionTypeExternalEvent pTipo);
        public abstract void PublishUser(UserModifyEvent pModelo);
        public abstract void PublishTranslation(TranslationError pModelo);
        public abstract void PublishPageCms(CmsPageEvent pModelo);
        public abstract void PublishComponentCms(CmsComponentEvent pModelo);
        public abstract void CrearCola();
    }
}
