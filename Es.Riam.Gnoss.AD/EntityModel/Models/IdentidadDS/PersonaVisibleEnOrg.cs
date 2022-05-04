using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("PersonaVisibleEnOrg")]
    public partial class PersonaVisibleEnOrg
    {
        [Column(Order = 0)]
        public Guid PersonaID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        public int Orden { get; set; }
    }
}
