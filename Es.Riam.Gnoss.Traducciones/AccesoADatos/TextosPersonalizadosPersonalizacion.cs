namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TextosPersonalizadosPersonalizacion")]
    public partial class TextosPersonalizadosPersonalizacion
    {
        [Key]
        [Column(Order = 0)]
        public Guid PersonalizacionID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string TextoID { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string Language { get; set; }

        public string Texto { get; set; }
    }
}
