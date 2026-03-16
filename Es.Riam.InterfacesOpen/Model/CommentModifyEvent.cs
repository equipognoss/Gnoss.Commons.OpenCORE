using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class CommentModifyEvent : EventSwitchingBase
    {
        public Guid ResourceId { get; set; }
        public Guid CommentId { get; set; }
        public Guid ParentCommentId { get; set; }

        public CommentModifyEvent() : base("comentario") { }

        public CommentModifyEvent(Guid pProjectId, Guid pIdRecurso, Guid pIdComentario, Guid pIdComentarioPadre, Guid pUserId, DateTime pDate) : base(pProjectId, pUserId, pDate, "comentario")
        {
            ResourceId = pIdRecurso;
            CommentId = pIdComentario;
            ParentCommentId = pIdComentarioPadre;
        }

    }
}
