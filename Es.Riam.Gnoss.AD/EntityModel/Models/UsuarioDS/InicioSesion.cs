using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("InicioSesion")]
    public partial class InicioSesion
    {
        [Key]
        public Guid UsuarioID { get; set; }

        public Guid? OrganizacionGnossID { get; set; }

        public Guid? PersonaID { get; set; }

        public Guid? ProyectoID { get; set; }
        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }
    }
}
