using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Sitemaps
{
    [Serializable]
    [Table("SitemapsIndex")]
    public partial class SitemapsIndex
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(255)]
        public string Dominio { get; set; }

        [Column(Order = 1)]
        public string Sitemap { get; set; }

        [Column(Order = 2)]
        public string Robots { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sitemaps> Indexlist { get; set; }
    }
}
