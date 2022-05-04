using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PerfilIdentidad")]
    public class PerfilIdentidad
    {
        public Perfil Perfil { get; set; }
        public int NumConnexiones { get; set; }
    }
}
