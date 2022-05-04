using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documento
{
    [Serializable]
    [Table("DocumentoRolIdentidad")]
    public partial class DocumentoRolIdentidad
    {
        [Key]
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid PerfilID { get; set; }

        public bool Editor { get; set; }

        public virtual Documento Documento { get; set; }
    }
}
