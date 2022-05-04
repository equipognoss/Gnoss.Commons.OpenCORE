using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
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
    [Table("GeneralRolUsuario")]
    public partial class GeneralRolUsuario
    {
        [Key]
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(16)]
        public string RolPermitido { get; set; }

        [StringLength(16)]
        public string RolDenegado { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }

    }
}
