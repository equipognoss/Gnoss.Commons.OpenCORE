namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TextosPersonalizadosPlataforma")]
    public partial class TextosPersonalizadosPlataforma
    {
        [Key]
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string TextoID { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(10)]
        public string Language { get; set; }

        public string Texto { get; set; }
    }
}
