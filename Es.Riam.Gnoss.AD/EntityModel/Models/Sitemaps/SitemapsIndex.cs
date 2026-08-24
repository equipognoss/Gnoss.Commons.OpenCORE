using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Sitemaps
{
    /// <summary>
    /// Registro de un dominio con sitemaps activados. 
    /// Esta fila solo guarda el registro del dominio, el robots.txt asociado, y la fecha de la última generación completada.
    /// </summary>
    [Serializable]
    [Table("SitemapsIndex")]
    public partial class SitemapsIndex
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string Dominio { get; set; }

        [Column(Order = 1)]
        public DateTime? GeneratedAt { get; set; }

        [Column(Order = 2)]
        public string Robots { get; set; }
    }
}
