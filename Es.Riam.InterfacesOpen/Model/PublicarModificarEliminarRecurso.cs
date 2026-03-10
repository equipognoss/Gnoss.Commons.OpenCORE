using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class PublicarModificarEliminarRecurso : EventSwitchingBase
    {
        public Guid IdProyecto { get; set; }
        public Guid IdRecurso { get; set; }
        public Guid IdRecursoOriginal { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime FechaRecurso { get; set; }
        public ActionTypeExternalEvent TipoAccion { get; set; }
        public string EnlaceMejora { get; set; }
        public string Transicion { get; set; }
        public string EstadoOrigen { get; set; }
        public string EstadoDestino { get; set; }

        public PublicarModificarEliminarRecurso() : base("recurso") { }
        public PublicarModificarEliminarRecurso(Guid pIdProyecto, Guid pIdRecurso, Guid pIdRecursoOriginal, Guid pIdUsuario, DateTime pFechaRecurso) : base("recurso")
        {
            IdProyecto = pIdProyecto;
            IdRecurso = pIdRecurso;
            IdRecursoOriginal = pIdRecursoOriginal;
            IdUsuario = pIdUsuario;
            FechaRecurso = pFechaRecurso;
        }
    }
}
