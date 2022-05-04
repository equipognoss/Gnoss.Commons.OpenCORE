using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    public class UsuarioPerfil
    {
        public Guid UsuarioID { get; set; }
        public Guid? OrganizacionID { get; set; }
    }
}
