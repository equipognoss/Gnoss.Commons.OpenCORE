using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("TipoDocDispRolUsuarioProy")]
    public partial class TipoDocDispRolUsuarioProy
    {

        public TipoDocDispRolUsuarioProy()
        {

        }

        public TipoDocDispRolUsuarioProy(Guid organizacionID, Guid proyectoID, short tipoDocumento, short rolUsuario)
        {
            OrganizacionID = organizacionID;
            ProyectoID = proyectoID;
            TipoDocumento = tipoDocumento;
            RolUsuario = rolUsuario;
        }
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short TipoDocumento { get; set; }

        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short RolUsuario { get; set; }
    }
}
