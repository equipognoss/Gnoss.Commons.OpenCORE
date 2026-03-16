using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class UserModifyEvent : EventSwitchingBase
    {
        public UserModifyEvent() : base("usuario") { }

        public UserModifyEvent(Guid pProjectId, Guid pUserId, DateTime pDate) : base(pProjectId, pUserId, pDate, "usuario") { }
    }
}
