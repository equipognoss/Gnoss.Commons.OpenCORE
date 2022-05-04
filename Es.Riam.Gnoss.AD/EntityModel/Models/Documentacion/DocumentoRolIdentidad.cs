using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoRolIdentidad")]
    public partial class DocumentoRolIdentidad
    {
        [Column(Order = 0)]
        [ForeignKey("Documento")]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid PerfilID { get; set; }

        public bool Editor { get; set; }

        public virtual Documento Documento { get; set; }
    }
}
