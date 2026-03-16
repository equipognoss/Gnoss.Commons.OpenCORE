using System;

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

        /// <summary>
        /// Indica el proyecto donde se ha generado el eventoo
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Indica el identificador del usuario que ha generado el evento
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Indica la fecha en la que se ha generado el evento
        /// </summary>
        public DateTime Date { get; set; }

        protected EventSwitchingBase(string pEventType)
        {
            Type = pEventType;
        }
        protected EventSwitchingBase(Guid pProjectId, Guid pUserId, DateTime pDate, string pEventType) : this(pEventType)
        {
            ProjectId = pProjectId;
            UserId = pUserId;
            Date = pDate;
        }
    }
}
