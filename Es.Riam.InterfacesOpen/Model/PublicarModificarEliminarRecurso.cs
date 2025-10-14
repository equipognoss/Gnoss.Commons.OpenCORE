using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open.Model
{
    public class PublicarModificarEliminarRecurso
    {
        public Guid IdProyecto { get; set; }
        public Guid IdRecurso { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime fechaRecurso { get; set; }
        public string TipoAccion { get; set; }
        public string type { get; set; }

        public PublicarModificarEliminarRecurso()
        {
            type = "recurso";
        }

        public PublicarModificarEliminarRecurso(Guid pIdProyecto, Guid pIdRecurso, Guid pIdUsuario, DateTime pFechaRecurso)
        {
            IdProyecto = pIdProyecto;
            IdRecurso = pIdRecurso;
            IdUsuario = pIdUsuario;
            fechaRecurso = pFechaRecurso;
            type = "recurso";
            
        }
    }
}
