using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraEcosistema")]
    public partial class DatoExtraEcosistema
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatoExtraEcosistema()
        {
            DatoExtraEcosistemaOpcion = new HashSet<DatoExtraEcosistemaOpcion>();
        }

        [Key]
        public Guid DatoExtraID { get; set; }

        public int Orden { get; set; }

        [Required]
        [StringLength(500)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string PredicadoRDF { get; set; }

        public bool Obligatorio { get; set; }

        public bool Paso1Registro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraEcosistemaOpcion> DatoExtraEcosistemaOpcion { get; set; }
    }
}
