using Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS;
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
    [Table("AdministradorOrganizacion")]
    public partial class AdministradorOrganizacion
    {
        [Column(Order = 0)]
        public Guid UsuarioID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Tipo { get; set; }
        public virtual Usuario Usuario { get; set; }

        public virtual Organizacion Organizacion { get; set; }
    }
}
