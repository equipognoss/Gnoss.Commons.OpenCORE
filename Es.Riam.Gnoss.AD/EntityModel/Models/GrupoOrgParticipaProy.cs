namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("GrupoOrgParticipaProy")]
    public partial class GrupoOrgParticipaProy
    {
        [Column(Order = 0)]
        public Guid GrupoID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        public DateTime FechaAlta { get; set; }

        public short TipoPerfil { get; set; }
    }
}
