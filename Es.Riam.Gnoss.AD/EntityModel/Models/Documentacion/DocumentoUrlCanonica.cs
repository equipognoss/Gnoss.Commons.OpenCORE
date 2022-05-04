using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoUrlCanonica")]
    public partial class DocumentoUrlCanonica
    {
        [Key]
        public Guid DocumentoID { get; set; }

        [Required]
        [StringLength(250)]
        public string UrlCanonica { get; set; }
    }
}
