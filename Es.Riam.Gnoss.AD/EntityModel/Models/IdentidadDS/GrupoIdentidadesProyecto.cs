using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("GrupoIdentidadesProyecto")]
    public partial class GrupoIdentidadesProyecto
    {
        [Column(Order = 0)]
        public Guid GrupoID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 2)]
        public Guid ProyectoID { get; set; }

        public virtual GrupoIdentidades GrupoIdentidades { get; set; }
    }
}
