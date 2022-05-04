using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PerfilPersona")]
    public partial class PerfilPersona
    {
        public Guid PersonaID { get; set; }

        [Key]
        public Guid PerfilID { get; set; }
        public virtual Perfil Perfil { get; set; }
    }
}
