namespace Traduccion.AccesoADatos
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CMSPropiedadComponente")]
    public partial class CMSPropiedadComponente
    {
        [Key]
        [Column(Order = 0)]
        public Guid ComponenteID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoPropiedadComponente { get; set; }

        [Required]
        public string ValorPropiedad { get; set; }

        public virtual CMSComponente CMSComponente { get; set; }
    }
}
