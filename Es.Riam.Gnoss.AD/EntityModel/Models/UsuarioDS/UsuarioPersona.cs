using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [NotMapped]
   public class UsuarioPersona
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public Guid UsuarioID { get; set; }
        public string Login { get; set; }
    }
}
