using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
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
    [Table("AdministradorGeneral")]
    public partial class AdministradorGeneral
    {
        [Key]
        public Guid UsuarioID { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
