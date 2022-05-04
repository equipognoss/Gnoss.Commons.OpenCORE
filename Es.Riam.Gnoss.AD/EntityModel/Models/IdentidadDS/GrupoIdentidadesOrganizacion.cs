using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Table("GrupoIdentidadesOrganizacion")]
    [Serializable]
    public partial class GrupoIdentidadesOrganizacion
    {
        [Column(Order = 0)]
        public Guid GrupoID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        public virtual GrupoIdentidades GrupoIdentidades { get; set; }
    }
}
