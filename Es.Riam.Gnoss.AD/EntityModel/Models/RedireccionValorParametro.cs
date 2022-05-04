namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("RedireccionValorParametro")]
    public partial class RedireccionValorParametro
    {
        [Column(Order = 0)]
        public Guid RedireccionID { get; set; }

        [Column(Order = 1)]
        [StringLength(250)]
        public string ValorParametro { get; set; }

        [Required]
        public string UrlRedireccion { get; set; }

        public bool MantenerFiltros { get; set; }

        [Required]
        public short OrdenPresentacion { get; set; }

        public virtual RedireccionRegistroRuta RedireccionRegistroRuta { get; set; }
    }
}
