using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class ModificarComentarios : EventSwitchingBase
    {
        public Guid IdProyecto { get; set; }
        public Guid IdRecurso { get; set; }
        public Guid IdComentario { get; set; }
        public Guid IdComentarioPadre { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime fechaComentario { get; set; }

        public ModificarComentarios() : base("comentario") { }

        public ModificarComentarios(Guid pIdProyecto, Guid pIdRecurso, Guid pIdComentario, Guid pIdComentarioPadre, Guid pIdUsuario, DateTime pFechaComentario) : base("comentario")
        {
            IdProyecto = pIdProyecto;
            IdRecurso = pIdRecurso;
            IdComentario = pIdComentario;
            IdComentarioPadre = pIdComentarioPadre;
            IdUsuario = pIdUsuario;
            fechaComentario = pFechaComentario;
        }

    }
}
