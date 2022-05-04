using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS
{
    [Serializable]
    [Table("OrganizacionParticipaProy")]
    public partial class OrganizacionParticipaProy
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        public DateTime FechaInicio { get; set; }

        public Guid IdentidadID { get; set; }

        public bool EstaBloqueada { get; set; }

        public short RegistroAutomatico { get; set; }

        public virtual Organizacion Organizacion { get; set; }

        [ForeignKey("IdentidadID")]
        public virtual Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }
}
