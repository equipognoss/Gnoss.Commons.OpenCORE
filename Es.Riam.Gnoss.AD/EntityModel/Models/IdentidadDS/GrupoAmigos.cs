using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("GrupoAmigos")]
    public class GrupoAmigos
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GrupoAmigos()
        {
            AmigoAgGrupo = new HashSet<AmigoAgGrupo>();
        }

        [Key]
        public Guid GrupoID { get; set; }

        [Required]
        [StringLength(250)]
        public string Nombre { get; set; }

        public DateTime Fecha { get; set; }

        public Guid IdentidadID { get; set; }

        public int Tipo { get; set; }

        public bool Automatico { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AmigoAgGrupo> AmigoAgGrupo { get; set; }
        [NotMapped]
        public string NombreBusqueda { get; set; }
    }
}
