using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("BaseRecursosUsuario")]
    public partial class BaseRecursosUsuario
    {
        [Column(Order = 0)]
        public Guid BaseRecursosID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioID { get; set; }

        public double? EspacioMaxMyGnossMB { get; set; }

        public double? EspacioActualMyGnossMB { get; set; }

        public virtual BaseRecursos BaseRecursos { get; set; }
        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}
