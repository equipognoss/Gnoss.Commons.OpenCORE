using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("VersionDocumento")]
    public partial class VersionDocumento
    {
        [Key]
        public Guid DocumentoID { get; set; }

        public int Version { get; set; }

        public Guid DocumentoOriginalID { get; set; }

        public Guid IdentidadID { get; set; }

        public virtual Documento Documento { get; set; }
    }
}
