namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CategoriaTesauro")]
    public partial class CategoriaTesauro
    {
        [Key]
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid CategoriaTesauroID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        public short Orden { get; set; }

        public int NumeroRecursos { get; set; }

        public int NumeroPreguntas { get; set; }

        public int NumeroDebates { get; set; }

        public int NumeroDafos { get; set; }

        public bool TieneFoto { get; set; }

        public short VersionFoto { get; set; }

        public short Estructurante { get; set; }
    }
}
