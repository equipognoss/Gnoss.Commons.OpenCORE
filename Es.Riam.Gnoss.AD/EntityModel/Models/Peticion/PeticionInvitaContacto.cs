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
    [Table("PeticionInvitaContacto")]
    public partial class PeticionInvitaContacto
    {
        [Key]
        public Guid PeticionID { get; set; }

        public Guid IdentidadID { get; set; }

        public virtual Peticion Peticion { get; set; }
    }
}
