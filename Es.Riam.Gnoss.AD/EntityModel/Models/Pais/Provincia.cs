using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Pais
{
    [Serializable]
    [Table("Provincia")]
    public partial class Provincia
    {
        public Guid ProvinciaID { get; set; }

        public Guid PaisID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(5)]
        public string CP { get; set; }

        public virtual Pais Pais { get; set; }
    }
}
