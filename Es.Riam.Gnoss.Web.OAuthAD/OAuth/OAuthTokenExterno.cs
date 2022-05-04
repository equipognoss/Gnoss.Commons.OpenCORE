namespace Es.Riam.Gnoss.OAuthAD.OAuth
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OAuthTokenExterno")]
    public partial class OAuthTokenExterno
    {
        [Key]
        public int TokenId { get; set; }

        [Required]
        [StringLength(50)]
        public string Token { get; set; }

        [Required]
        [StringLength(50)]
        public string TokenSecret { get; set; }

        public int State { get; set; }

        public DateTime IssueDate { get; set; }

        public int? TokenVinculadoId { get; set; }

        public Guid UsuarioID { get; set; }

        public virtual OAuthToken OAuthToken { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
