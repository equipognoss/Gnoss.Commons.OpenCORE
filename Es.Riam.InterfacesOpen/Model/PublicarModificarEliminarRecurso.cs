using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class PublicarModificarEliminarRecurso : EventSwitchingBase
    {
        public Guid IdProyecto { get; set; }
        public Guid IdRecurso { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime fechaRecurso { get; set; }
        public string TipoAccion { get; set; }

        public PublicarModificarEliminarRecurso() : base("recurso") { }
        public PublicarModificarEliminarRecurso(Guid pIdProyecto, Guid pIdRecurso, Guid pIdUsuario, DateTime pFechaRecurso) : base("recurso")
        {
            IdProyecto = pIdProyecto;
            IdRecurso = pIdRecurso;
            IdUsuario = pIdUsuario;
            fechaRecurso = pFechaRecurso;
        }
    }
}
