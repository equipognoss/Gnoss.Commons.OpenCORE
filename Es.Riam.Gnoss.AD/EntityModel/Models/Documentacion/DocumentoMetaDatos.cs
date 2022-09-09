using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Table("DocumentoMetaDatos")]
    [Serializable]
    public partial class DocumentoMetaDatos
    {
        [Key]
        public Guid DocumentoID { get; set; }
        [StringLength(1000)]
        public string MetaTitulo { get; set; }
        [StringLength(1000)]
        public string MetaDescripcion { get; set; }

        public virtual Documento Documento { get; set; }
    }
}