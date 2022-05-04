namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ClausulaRegistro")]
    public partial class ClausulaRegistro
    {
        [Key]
        [Column(Order = 0)]
        public Guid ClausulaID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        [Required]
        public string Texto { get; set; }

        public short Tipo { get; set; }

        public int Orden { get; set; }
    }
}
