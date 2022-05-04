using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoSearchPersonalizado")]
    public partial class ProyectoSearchPersonalizado
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(100)]
        public string NombreFiltro { get; set; }

        public string WhereSPARQL { get; set; }

        public string OrderBySPARQL { get; set; }

        public string WhereFacetasSPARQL { get; set; }

        public bool OmitirRdfType { get; set; }

        public virtual Proyecto Proyecto { get; set; }
    }
}
