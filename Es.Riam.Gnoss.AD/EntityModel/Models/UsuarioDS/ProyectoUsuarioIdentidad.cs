using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS
{
    [Serializable]
    [Table("ProyectoUsuarioIdentidad")]
    public partial class ProyectoUsuarioIdentidad
    {
        [Column(Order = 0)]
        public Guid IdentidadID { get; set; }

        [Column(Order = 1)]
        public Guid UsuarioID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionGnossID { get; set; }

        [Column(Order = 3)]
        public Guid ProyectoID { get; set; }

        public DateTime? FechaEntrada { get; set; }

        public int? Reputacion { get; set; }
        public virtual IdentidadDS.Identidad Identidad { get; set; }

        public virtual Proyecto Proyecto { get; set; }

        public virtual Usuario Usuario { get; set; }

    }
}
