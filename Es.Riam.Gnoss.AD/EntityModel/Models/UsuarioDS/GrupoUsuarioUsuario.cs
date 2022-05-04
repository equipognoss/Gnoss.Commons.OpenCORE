using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("GrupoUsuarioUsuario")]
    public partial class GrupoUsuarioUsuario
    {
        [Column(Order = 0)]
        public Guid UsuarioID { get; set; }

        [Column(Order = 1)]
        public Guid GrupoUsuarioID { get; set; }
        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}
