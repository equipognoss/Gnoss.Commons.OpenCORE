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
    [Table("DocumentoEnEdicion")]
    public partial class DocumentoEnEdicion
    {
        [Key]
        public Guid DocumentoID { get; set; }

        public Guid IdentidadID { get; set; }

        public DateTime FechaEdicion { get; set; }
    }
}
