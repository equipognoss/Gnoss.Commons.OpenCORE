using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    [Serializable]
    [Table("SolicitudNuevoProfesor")]
    public partial class SolicitudNuevoProfesor
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioID { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string CentroEstudios { get; set; }

        [StringLength(255)]
        public string AreaEstudios { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
