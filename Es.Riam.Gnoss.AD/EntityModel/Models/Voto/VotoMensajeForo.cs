using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Voto
{
    [Serializable]
    [Table("VotoMensajeForo")]
    public partial class VotoMensajeForo
    {
        [Column(Order = 0)]
        public Guid VotoID { get; set; }

        [Column(Order = 1)]
        public Guid ForoID { get; set; }

        [Column(Order = 2)]
        public Guid CategoriaForoID { get; set; }

        [Column(Order = 3)]
        public Guid TemaID { get; set; }

        [Column(Order = 4)]
        public Guid MensajeForoID { get; set; }

        public virtual Voto Voto { get; set; }
    }
}
