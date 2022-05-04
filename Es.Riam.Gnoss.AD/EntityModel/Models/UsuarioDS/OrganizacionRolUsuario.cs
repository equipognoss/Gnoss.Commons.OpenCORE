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
    [Table("OrganizacionRolUsuario")]
    public partial class OrganizacionRolUsuario
    {
        [Column(Order = 0)]
        public Guid UsuarioID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Required]
        [StringLength(16)]
        public string RolPermitido { get; set; }

        [StringLength(16)]
        public string RolDenegado { get; set; }
        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}
