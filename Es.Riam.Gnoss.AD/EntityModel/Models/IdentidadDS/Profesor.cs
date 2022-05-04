using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("Profesor")]
    public partial class Profesor
    {
        [Column(Order = 0)]
        public Guid ProfesorID { get; set; }

        [Column(Order = 1)]
        public Guid PerfilID { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string CentroEstudios { get; set; }

        [StringLength(255)]
        public string AreaEstudios { get; set; }
        public virtual Perfil Perfil { get; set; }
    }
}
