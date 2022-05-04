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
    [Table("ProyRolUsuClausulaReg")]
    public partial class ProyRolUsuClausulaReg
    {
        [Column(Order = 0)]
        public Guid ClausulaID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionGnossID { get; set; }

        [Column(Order = 3)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 4)]
        public Guid UsuarioID { get; set; }

        public bool Valor { get; set; }

        public virtual ClausulaRegistro ClausulaRegistro { get; set; }

        public virtual ProyectoRolUsuario ProyectoRolUsuario { get; set; }
    }
}
