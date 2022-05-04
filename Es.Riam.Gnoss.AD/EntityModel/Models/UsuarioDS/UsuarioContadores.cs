using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    public partial class UsuarioContadores
    {
        [Key]
        public Guid UsuarioID { get; set; }

        public int NumeroAccesos { get; set; }

        public DateTime? FechaUltimaVisita { get; set; }

        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}
