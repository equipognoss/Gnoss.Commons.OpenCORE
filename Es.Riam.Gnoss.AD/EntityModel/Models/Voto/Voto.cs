using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Voto
{
    [Serializable]
    [Table("Voto")]
    public partial class Voto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Voto()
        {
            VotoDocumento = new HashSet<VotoDocumento>();
            VotoComentario = new HashSet<VotoComentario>();
            VotoEntradaBlog = new HashSet<VotoEntradaBlog>();
            VotoMensajeForo = new HashSet<VotoMensajeForo>();   
        }

        public Guid VotoID { get; set; }

        public Guid IdentidadID { get; set; }

        public Guid ElementoID { get; set; }

        public Guid IdentidadVotadaID { get; set; }

        [Column("Voto")]
        public double Voto1 { get; set; }

        public short Tipo { get; set; }

        public DateTime? FechaVotacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VotoDocumento> VotoDocumento { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VotoComentario> VotoComentario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VotoEntradaBlog> VotoEntradaBlog { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VotoMensajeForo> VotoMensajeForo { get; set; }

    }
}
