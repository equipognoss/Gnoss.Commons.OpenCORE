using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS
{
    [Serializable]
    [Table("OrganizacionEmpresa")]
    public partial class OrganizacionEmpresa
    {
        [Key]
        public Guid OrganizacionID { get; set; }

        [StringLength(9)]
        public string CIF { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public int? Empleados { get; set; }

        public short TipoOrganizacion { get; set; }

        public short SectorOrganizacion { get; set; }
        [ForeignKey("OrganizacionID")]
        public virtual Organizacion Organizacion { get; set; }
    }
}
