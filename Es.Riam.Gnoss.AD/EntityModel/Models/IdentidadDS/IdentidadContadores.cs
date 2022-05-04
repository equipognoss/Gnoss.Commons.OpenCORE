using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("IdentidadContadores")]
    public partial class IdentidadContadores
    {
        [Key]
        public Guid IdentidadID { get; set; }

        public int NumeroVisitas { get; set; }

        public int NumeroDescargas { get; set; }

        [ForeignKey("IdentidadID")]
        public virtual Identidad Identidad { get; set; }
    }
}
