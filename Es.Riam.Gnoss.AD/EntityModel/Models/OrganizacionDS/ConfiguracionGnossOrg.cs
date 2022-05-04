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
    [Table("ConfiguracionGnossOrg")]
    public partial class ConfiguracionGnossOrg
    {
        [Key]
        public Guid OrganizacionID { get; set; }

        public bool VerRecursos { get; set; }

        public bool VerRecursosExterno { get; set; }

        public short VisibilidadContactos { get; set; }
        public virtual Organizacion Organizacion { get; set; }
    }
}
