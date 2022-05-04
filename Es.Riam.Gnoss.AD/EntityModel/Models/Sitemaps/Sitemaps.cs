using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Sitemaps
{
    [Serializable]
    [Table("Sitemaps")]
    public partial class Sitemaps
    {
        [Column(Order = 0)]
        [StringLength(255)]
        public string Dominio { get; set; }
        [Column(Order = 1)]
        public string SitemapIndexName { get; set; }
        [Column(Order = 2)]
        public string SitemapContent { get; set; }

        public SitemapsIndex SitemapIndex { get; set; }
    }
}
