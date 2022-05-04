using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public partial class DocumentoRolGrupoIdentidades
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid GrupoID { get; set; }

        public bool Editor { get; set; }
        public virtual Documento Documento { get; set; }
    }
}
