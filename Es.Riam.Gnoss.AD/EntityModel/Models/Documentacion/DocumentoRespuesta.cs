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
    [Table("DocumentoRespuesta")]
    public partial class DocumentoRespuesta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentoRespuesta()
        {
            DocumentoRespuestaVoto = new HashSet<DocumentoRespuestaVoto>();
        }

        [Key]
        public Guid RespuestaID { get; set; }

        public Guid DocumentoID { get; set; }

        [Required]
        public string Descripcion { get; set; }

        public int NumVotos { get; set; }

        public short Orden { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoRespuestaVoto> DocumentoRespuestaVoto { get; set; }
    }
}
