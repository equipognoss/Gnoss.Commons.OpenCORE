using System;

namespace Es.Riam.InterfacesOpen.Model
{
    public class CmsComponentEvent : EventSwitchingBase
    {
        public Guid ComponentId { get; set; }
        public string Transition { get; set; }
        public string SourceState { get; set; }
        public string TargetState { get; set; }
        public ActionTypeExternalEvent ActionType { get; set; }

        public CmsComponentEvent(Guid pProjectId, Guid pUserId, DateTime pDate) : base(pProjectId, pUserId, pDate, "componenteCms") { }

        public CmsComponentEvent(Guid pComponentId, Guid pProjectId, Guid pUserId, DateTime pDate, string transitionName, string sourceState, string targetState, ActionTypeExternalEvent pActionType) : this(pComponentId, pProjectId, pUserId, pDate, pActionType)
        {
            SourceState = sourceState;
            TargetState = targetState;
            Transition = transitionName; 
        }

        public CmsComponentEvent(Guid pComponentId, Guid pProjectId, Guid pUserId, DateTime pDate, ActionTypeExternalEvent pActionType) : this(pProjectId, pUserId, pDate)
        {
            ComponentId = pComponentId;
            ActionType = pActionType;
        }
    }
}
