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
    [Table("DocumentoComentario")]
    public partial class DocumentoComentario
    {
        [Column(Order = 0)]
        public Guid ComentarioID { get; set; }

        [Column(Order = 1)]
        public Guid DocumentoID { get; set; }

        public Guid? ProyectoID { get; set; }

        [ForeignKey("ComentarioID")]
        public virtual Comentario.Comentario Comentario { get; set; }
    }
}
