using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoGrupoUsuario")]
    public partial class DocumentoGrupoUsuario
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid GrupoUsuarioID { get; set; }

        public bool Editor { get; set; }
    }
}
