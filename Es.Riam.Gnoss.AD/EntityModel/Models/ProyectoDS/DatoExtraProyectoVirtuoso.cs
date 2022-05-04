using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("DatoExtraProyectoVirtuoso")]
    public partial class DatoExtraProyectoVirtuoso
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatoExtraProyectoVirtuoso()
        {
            DatoExtraProyectoVirtuosoIdentidad = new HashSet<DatoExtraProyectoVirtuosoIdentidad>();
            DatoExtraProyectoVirtuosoSolicitud = new HashSet<DatoExtraProyectoVirtuosoSolicitud>();
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

        [Required]
        [StringLength(500)]
        public string InputID { get; set; }

        [Required]
        [StringLength(500)]
        public string InputsSuperiores { get; set; }

        [Required]
        [StringLength(500)]
        public string QueryVirtuoso { get; set; }

        [Required]
        [StringLength(500)]
        public string ConexionBD { get; set; }

        public bool Obligatorio { get; set; }

        public bool Paso1Registro { get; set; }

        public bool VisibilidadFichaPerfil { get; set; }

        [Required]
        [StringLength(500)]
        public string PredicadoRDF { get; set; }

        [Required]
        [StringLength(500)]
        public string NombreCampo { get; set; }

        [StringLength(500)]
        public string EstructuraHTMLFicha { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraProyectoVirtuosoIdentidad> DatoExtraProyectoVirtuosoIdentidad { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraProyectoVirtuosoSolicitud> DatoExtraProyectoVirtuosoSolicitud { get; set; }
    }
}
