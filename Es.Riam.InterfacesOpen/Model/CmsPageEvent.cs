using System;

namespace Es.Riam.InterfacesOpen.Model
{
    public class CmsPageEvent : EventSwitchingBase
    {
        public Guid PageID { get; set; }
        public Guid ProjectID { get; set; }
        public Guid UserID { get; set; }
        public DateTime Date { get; set; }
        public string Transition { get; set; }
        public string SourceState { get; set; }
        public string TargetState { get; set; }
        public ActionTypeExternalEvent ActionType { get; set; }

        public CmsPageEvent() : base("paginaCms") { }

        public CmsPageEvent(Guid pPageID, Guid pProjectID, Guid pUserID, DateTime pDate, string pTransitionName, string pSourceState, string pTargetState, ActionTypeExternalEvent pActionType) : this(pPageID, pProjectID, pUserID, pDate, pActionType)
        {
            SourceState = pSourceState;
            TargetState = pTargetState;
            Transition = pTransitionName;
        }

        public CmsPageEvent(Guid pPageID, Guid pProjectID, Guid pUserID, DateTime pDate, ActionTypeExternalEvent pActionType) : this()
        {
            PageID = pPageID;
            ProjectID = pProjectID;
            UserID = pUserID;
            Date = pDate;
            ActionType = pActionType;
        }
    }
}