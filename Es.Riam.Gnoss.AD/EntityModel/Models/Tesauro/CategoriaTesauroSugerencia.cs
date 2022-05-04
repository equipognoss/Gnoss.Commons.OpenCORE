using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("CategoriaTesauroSugerencia")]
    public partial class CategoriaTesauroSugerencia
    {
        [Key]
        public Guid SugerenciaID { get; set; }

        public Guid TesauroSugerenciaID { get; set; }

        public Guid? TesauroCatPadreID { get; set; }

        public Guid? CategoriaTesauroPadreID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Nombre { get; set; }

        public Guid IdentidadID { get; set; }

        public short Estado { get; set; }

        public Guid? CategoriaTesauroAceptadaID { get; set; }

        public virtual CategoriaTesauro CategoriaTesauro { get; set; }

        public virtual Tesauro Tesauro { get; set; }
    }
}
