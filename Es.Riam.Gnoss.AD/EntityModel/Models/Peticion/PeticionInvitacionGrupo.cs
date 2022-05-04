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
    [Table("PeticionInvitacionGrupo")]
    public partial class PeticionInvitacionGrupo
    {
        [Key]
        public Guid PeticionID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [Required]
        public string GruposID { get; set; }

        public virtual Peticion Peticion { get; set; }
    }
}
