namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TesauroProyecto")]
    public partial class TesauroProyecto
    {
        [Key]
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [StringLength(50)]
        public string IdiomaDefecto { get; set; }
    }
}
