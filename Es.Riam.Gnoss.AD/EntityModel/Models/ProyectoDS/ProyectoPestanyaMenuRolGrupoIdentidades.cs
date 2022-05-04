using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    public partial class ProyectoPestanyaMenuRolGrupoIdentidades
    {
        [Column(Order = 0)]
        public Guid PestanyaID { get; set; }

        [Column(Order = 1)]
        public Guid GrupoID { get; set; }

        public virtual ProyectoPestanyaMenu ProyectoPestanyaMenu { get; set; }
    }
}
