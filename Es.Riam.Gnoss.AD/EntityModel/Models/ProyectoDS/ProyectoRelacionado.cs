using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoRelacionado")]
    public partial class ProyectoRelacionado
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionRelacionadaID { get; set; }

        [Column(Order = 3)]
        public Guid ProyectoRelacionadoID { get; set; }

        public short Orden { get; set; }

        public virtual Proyecto Proyecto { get; set; }

        public virtual Proyecto Proyecto1 { get; set; }
    }
}
