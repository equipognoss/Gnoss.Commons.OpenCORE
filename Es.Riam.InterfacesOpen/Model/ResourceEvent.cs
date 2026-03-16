using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class ResourceEvent : EventSwitchingBase
    {
        public Guid ResourceId { get; set; }
        public Guid OriginalResourceId { get; set; }
        public ActionTypeExternalEvent ActionType { get; set; }
        public string ImprovementLink { get; set; }
        public string Transition { get; set; }
        public string SourceStatus { get; set; }
        public string TargetStatus { get; set; }

        public ResourceEvent() : base("recurso") { }
        public ResourceEvent(Guid pProjectId, Guid pResourceId, Guid pOriginalResourceId, Guid pUserId, DateTime pDate) : base(pProjectId, pUserId, pDate, "recurso")
        {
            ResourceId = pResourceId;
            OriginalResourceId = pOriginalResourceId;
        }
    }
}
