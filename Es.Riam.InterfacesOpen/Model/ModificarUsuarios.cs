using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open.Model
{
    public class ModificarUsuarios
    {
        public Guid IdProyecto { get; set; }
        public Guid IdUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string type { get; set; }

        public ModificarUsuarios()
        {
            type = "usuario";
        }

        public ModificarUsuarios(Guid pIdProyecto, Guid pIdUsuario, DateTime pFechaRegistro)
        {
            IdProyecto = pIdProyecto;
            IdUsuario = pIdUsuario;
            FechaRegistro = pFechaRegistro;
            type = "usuario";
        }
    }
}
