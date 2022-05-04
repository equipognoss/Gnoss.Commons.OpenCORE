using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("GeneralRolGrupoUsuario")]
    public partial class GeneralRolGrupoUsuario
    {
        [Key]
        public Guid GrupoUsuarioID { get; set; }

        [Required]
        [StringLength(16)]
        public string RolPermitido { get; set; }

        [StringLength(16)]
        public string RolDenegado { get; set; }
    }
}
