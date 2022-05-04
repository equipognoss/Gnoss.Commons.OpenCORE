using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    [Serializable]
    [Table("SolicitudNuevaOrgEmp")]
    public partial class SolicitudNuevaOrgEmp
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioAdminID { get; set; }

        [StringLength(9)]
        public string CIF { get; set; }

        public short Tipo { get; set; }

        public DateTime? FechaFundacion { get; set; }

        public int Empleados { get; set; }

        public short Sector { get; set; }

        public virtual SolicitudNuevaOrganizacion SolicitudNuevaOrganizacion { get; set; }
    }
}
