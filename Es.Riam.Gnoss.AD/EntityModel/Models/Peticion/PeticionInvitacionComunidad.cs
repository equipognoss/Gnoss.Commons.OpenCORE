using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Peticion
{
    [Serializable]
    [Table("PeticionInvitacionComunidad")]
    public partial class PeticionInvitacionComunidad
    {
        [Key]
        public Guid PeticionID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [StringLength(50)]
        public string NingID { get; set; }

        public virtual Peticion Peticion { get; set; }
    }
}
