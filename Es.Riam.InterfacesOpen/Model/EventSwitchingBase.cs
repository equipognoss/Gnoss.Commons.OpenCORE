
namespace Es.Riam.InterfacesOpen.Model
{
    public enum ActionTypeExternalEvent
    {
        Create,
        Update,
        Delete,
        ChangeState,
        StartImprovement,
        CancelImprovement,
        ApplyImporvement,
        Publish,
        SaveDraft
    }
    
    public abstract class EventSwitchingBase
    {
        /// <summary>
        /// Indica la routingKey que usara el servicio de eventos
        /// para enviarlo a la cola correspondiente
        /// </summary>
        public string Type { get; }
        protected EventSwitchingBase(string eventType)
        {
            Type = eventType;
        }
    }
}
