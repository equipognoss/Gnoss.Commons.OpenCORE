using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("SeccionProyCatalogo")]
    public partial class SeccionProyCatalogo
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionBusquedaID { get; set; }

        [Column(Order = 3)]
        public Guid ProyectoBusquedaID { get; set; }

        [Column(Order = 4)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Orden { get; set; }

        [Required]
        [StringLength(550)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Faceta { get; set; }

        [StringLength(200)]
        public string Filtro { get; set; }

        public short NumeroResultados { get; set; }

        public short Tipo { get; set; }
    }
}
