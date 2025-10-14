using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open.Model
{
    public class ModificarComentarios
    {
        public Guid IdProyecto { get; set; }
        public Guid IdRecurso { get; set; }
        public Guid IdComentario { get; set; }
        public Guid IdComentarioPadre { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime fechaComentario { get; set; }
        public string type { get; set; }

        public ModificarComentarios()
        {
            type = "comentario";
        }

        public ModificarComentarios(Guid pIdProyecto, Guid pIdRecurso, Guid pIdComentario, Guid pIdComentarioPadre, Guid pIdUsuario, DateTime pFechaComentario)
        {
            IdProyecto = pIdProyecto;
            IdRecurso = pIdRecurso;
            IdComentario = pIdComentario;
            IdComentarioPadre = pIdComentarioPadre;
            IdUsuario = pIdUsuario;
            fechaComentario = pFechaComentario;
            type = "comentario";
        }

    }
}
