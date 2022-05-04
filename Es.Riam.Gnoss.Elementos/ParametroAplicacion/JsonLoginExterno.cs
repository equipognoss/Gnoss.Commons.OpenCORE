using System;

namespace Es.Riam.Gnoss.Elementos.ParametroAplicacion
{
    public enum TipoEstadoLoginExterno
    {
        LoginOK,
        LoginKO
    }

    public class JsonLoginExterno
    {
        public TipoEstadoLoginExterno EstadoLogin { get; set; }
        public Guid UsuarioID { get; set; }
        public bool Loguear { get; set; }
        public string InfoExtra { get; set; }
    }
}
