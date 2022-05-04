using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Peticion
{
    [Table("PeticionOrgInvitaPers")]
    [Serializable]
    public partial class PeticionOrgInvitaPers
    {
        [Key]
        public Guid PeticionID { get; set; }

        public Guid OrganizacionID { get; set; }

        [StringLength(255)]
        public string Cargo { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public virtual Peticion Peticion { get; set; }
    }
}
