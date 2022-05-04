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
    [Table("OrganizacionGnoss")]
    public partial class OrganizacionGnoss
    {
        [Key]
        public Guid OrganizacionID { get; set; }

        public virtual Organizacion Organizacion { get; set; }
    }
}
