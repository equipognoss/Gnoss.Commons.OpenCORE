using System;

namespace Es.Riam.InterfacesOpen.Model
{
    public class CmsPageEvent : EventSwitchingBase
    {
        public Guid PageId { get; set; }
        public string Transition { get; set; }
        public string SourceState { get; set; }
        public string TargetState { get; set; }
        public ActionTypeExternalEvent ActionType { get; set; }

        public CmsPageEvent(Guid pProjectId, Guid pUserId, DateTime pDate) : base(pProjectId, pUserId, pDate, "paginaCms") { }

        public CmsPageEvent(Guid pPageId, Guid pProjectId, Guid pUserId, DateTime pDate, string pTransitionName, string pSourceState, string pTargetState, ActionTypeExternalEvent pActionType) : this(pPageId, pProjectId, pUserId, pDate, pActionType)
        {
            SourceState = pSourceState;
            TargetState = pTargetState;
            Transition = pTransitionName;
        }

        public CmsPageEvent(Guid pPageId, Guid pProjectId, Guid pUserId, DateTime pDate, ActionTypeExternalEvent pActionType) : this(pProjectId, pUserId, pDate)
        {
            PageId = pPageId;
            ActionType = pActionType;
        }
    }
}