using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public partial class ProyectoPestanyaRolGrupoIdentidades
    {
        [Column(Order = 0)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 1)]
        [StringLength(400)]
        public string Nombre { get; set; }

        [Column(Order = 2)]
        public Guid GrupoID { get; set; }

        public virtual ProyectoPestanya ProyectoPestanya { get; set; }
    }
}
