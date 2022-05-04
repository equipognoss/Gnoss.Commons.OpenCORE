using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("UsuarioRedirect")]
    public partial class UsuarioRedirect
    {
        [Key]
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(200)]
        public string UrlRedirect { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
