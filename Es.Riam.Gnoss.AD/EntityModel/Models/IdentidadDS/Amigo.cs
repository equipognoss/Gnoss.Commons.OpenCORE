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
    [Table("Amigo")]
    public partial class Amigo
    {
        public bool EsFanMutuo { get; set; }

        public DateTime Fecha { get; set; }

        [Column(Order = 0)]
        public Guid IdentidadAmigoID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }
    }
}
