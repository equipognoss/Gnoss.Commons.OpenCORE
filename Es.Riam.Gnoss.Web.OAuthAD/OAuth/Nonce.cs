namespace Es.Riam.Gnoss.OAuthAD.OAuth
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Nonce")]
    public partial class Nonce
    {
        [Column(Order = 0)]
        [StringLength(4000)]
        public string Context { get; set; }

        [Column(Order = 1)]
        [StringLength(4000)]
        public string Code { get; set; }

        [Column(Order = 2)]
        public DateTime Timestamp { get; set; }
    }
}
