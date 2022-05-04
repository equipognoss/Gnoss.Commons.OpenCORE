namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("RedireccionRegistroRuta")]
    public partial class RedireccionRegistroRuta
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RedireccionRegistroRuta()
        {
            RedireccionValorParametro = new HashSet<RedireccionValorParametro>();
        }

        [Key]
        public Guid RedireccionID { get; set; }

        [Required]
        public string UrlOrigen { get; set; }

        [Required]
        [StringLength(250)]
        public string Dominio { get; set; }

        [StringLength(250)]
        public string NombreParametro { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RedireccionValorParametro> RedireccionValorParametro { get; set; }
    }
}
