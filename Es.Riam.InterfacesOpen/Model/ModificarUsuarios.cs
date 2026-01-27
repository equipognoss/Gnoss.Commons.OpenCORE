using Es.Riam.InterfacesOpen.Model;
using System;

namespace Es.Riam.Open.Model
{
    public class ModificarUsuarios : EventSwitchingBase
    {
        public Guid IdProyecto { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }

        public ModificarUsuarios() : base("usuario") { }

        public ModificarUsuarios(Guid pIdProyecto, Guid pIdUsuario, DateTime pFechaRegistro) : base("usuario")
        {
            IdProyecto = pIdProyecto;
            IdUsuario = pIdUsuario;
            FechaRegistro = pFechaRegistro;
        }
    }
}
