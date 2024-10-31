using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraProyecto")]
    public partial class DatoExtraProyecto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatoExtraProyecto()
        {
            DatoExtraProyectoOpcion = new HashSet<DatoExtraProyectoOpcion>();
        }

        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid DatoExtraID { get; set; }

        public int Orden { get; set; }

        [Required]
        [StringLength(500)]
        public string Titulo { get; set; }

        public string NombreCorto { get; set; }

        [Required]
        [StringLength(500)]
        public string PredicadoRDF { get; set; }

        public bool Obligatorio { get; set; }

        public bool Paso1Registro { get; set; }

        public bool VisiblePerfil { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraProyectoOpcion> DatoExtraProyectoOpcion { get; set; }
    }
}
