using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PermisoGrupoAmigoOrg")]
    public partial class PermisoGrupoAmigoOrg
    {
        [Column(Order = 0)]
        public Guid IdentidadOrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadUsuarioID { get; set; }

        public bool PermisoEdicion { get; set; }

        [Column(Order = 2)]
        public Guid GrupoID { get; set; }
    }
}
