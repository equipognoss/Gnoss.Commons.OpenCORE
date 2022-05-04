using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PerfilPersonaOrg")]
    public partial class PerfilPersonaOrg
    {
        [Column(Order = 0)]
        public Guid PersonaID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid PerfilID { get; set; }
        public virtual Perfil Perfil { get; set; }
    }
}
