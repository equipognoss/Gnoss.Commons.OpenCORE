namespace Es.Riam.Gnoss.OAuthAD.OAuth
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PinToken")]
    public partial class PinToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TokenId { get; set; }

        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(50)]
        public string Pin { get; set; }

        public virtual OAuthToken OAuthToken { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
