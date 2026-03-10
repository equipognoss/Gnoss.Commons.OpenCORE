using System;

namespace Es.Riam.InterfacesOpen.Model
{
    public class CmsComponentEvent : EventSwitchingBase
    {
        public Guid ComponentID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid UserID { get; set; }
        public DateTime Date { get; set; }
        public string Transition { get; set; }
        public string SourceState { get; set; }
        public string TargetState { get; set; }
        public ActionTypeExternalEvent ActionType { get; set; }

        public CmsComponentEvent() : base("componenteCms") { }

        public CmsComponentEvent(Guid pComponentID, Guid pProjectID, Guid pUserID, DateTime pDate, string transitionName, string sourceState, string targetState, ActionTypeExternalEvent pActionType) : this(pComponentID, pProjectID, pUserID, pDate, pActionType)
        {
            SourceState = sourceState;
            TargetState = targetState;
            Transition = transitionName;
        }

        public CmsComponentEvent(Guid componentID, Guid projectID, Guid userID, DateTime date, ActionTypeExternalEvent actionType) : this()
        {
            ComponentID = componentID;
            ProjectID = projectID;
            UserID = userID;
            Date = date;
            ActionType = actionType;
        }
    }
}
