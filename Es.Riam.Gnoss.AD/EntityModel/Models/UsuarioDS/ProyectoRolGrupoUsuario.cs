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
    [Table("ProyectoRolGrupoUsuario")]
    public partial class ProyectoRolGrupoUsuario
    {
        [Column(Order = 0)]
        public Guid OrganizacionGnossID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid GrupoUsuarioID { get; set; }

        [Required]
        [StringLength(16)]
        public string RolPermitido { get; set; }

        [StringLength(16)]
        public string RolDenegado { get; set; }
    }
}
