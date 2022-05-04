using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.MVC
{
    [Serializable]
    [Table("CorreoInterno")]
    public partial class CorreoInterno
    {
        [Key]
        public Guid CorreoID { get; set; }

        public Guid Autor { get; set; }

        [Required]
        [StringLength(255)]
        public string Asunto { get; set; }

        public string Cuerpo { get; set; }

        public DateTime Fecha { get; set; }

        public bool Eliminado { get; set; }

        public bool EnPapelera { get; set; }

        public Guid? ConversacionID { get; set; }
    }
}
