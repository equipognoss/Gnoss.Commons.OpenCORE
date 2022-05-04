namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BaseRecursosProyecto")]
    public partial class BaseRecursosProyecto
    {
        [Key]
        [Column(Order = 0)]
        public Guid BaseRecursosID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }
    }
}
