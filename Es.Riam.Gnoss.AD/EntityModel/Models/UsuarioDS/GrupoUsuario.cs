using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("GrupoUsuario")]
    public partial class GrupoUsuario
    {
        public Guid GrupoUsuarioID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; }
    }

    public partial class GrupoUsuarioConRoles
    {
        public string NombreGrupo { get; set; }
        public List<Rol> RolesGrupo { get; set; }
    }
}
