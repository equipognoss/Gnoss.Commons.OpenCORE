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
    [Table("Tipologia")]
    public partial class Tipologia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Tipologia()
        {
            DocumentoTipologia = new HashSet<DocumentoTipologia>();
        }

        [Column(Order = 0)]
        public Guid TipologiaID { get; set; }

        [Column(Order = 1)]
        public Guid AtributoID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; }

        public int Tipo { get; set; }

        public int Orden { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocumentoTipologia> DocumentoTipologia { get; set; }
    }
}
