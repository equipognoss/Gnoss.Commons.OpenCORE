using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro
{
    [Serializable]
    [Table("TesauroUsuario")]
    public partial class TesauroUsuario
    {
        
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioID { get; set; }

        public Guid? CategoriaTesauroPublicoID { get; set; }

        public Guid? CategoriaTesauroPrivadoID { get; set; }

        public Guid? CategoriaTesauroMisImagenesID { get; set; }

        public Guid? CategoriaTesauroMisVideosID { get; set; }

        public virtual Tesauro Tesauro { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
