using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    public class UsuarioPerfilIdentidad
    {
        public Guid UsuarioID { get; set; }
        public Guid PerfilID { get; set; }
        public Guid IdentidadID { get; set; }
        public string NombrePerfil { get; set; }
    }
}
