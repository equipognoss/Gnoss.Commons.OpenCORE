using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Comentario
{
    [Serializable]
    [Table("VotoComentario")]
    public partial class VotoComentario
    {
        [Column(Order = 0)]
        public Guid VotoID { get; set; }

        [Column(Order = 1)]
        public Guid ComentarioID { get; set; }

        public virtual Comentario Comentario { get; set; }

        public virtual Voto.Voto Voto { get; set; }
    }
}
