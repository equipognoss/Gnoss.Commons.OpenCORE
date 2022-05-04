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
    [Table("AtributoFichaBibliografica")]
    public partial class AtributoFichaBibliografica
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AtributoFichaBibliografica()
        {
            DocumentoAtributoBiblio = new HashSet<DocumentoAtributoBiblio>();
        }

        [Column(Order = 0)]
        public Guid AtributoID { get; set; }

        [Column(Order = 1)]
        public Guid FichaBibliograficaID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; }

        public int Tipo { get; set; }

        public int Orden { get; set; }

        public int Longitud { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoAtributoBiblio> DocumentoAtributoBiblio { get; set; }

        public virtual FichaBibliografica FichaBibliografica { get; set; }
    }
}
