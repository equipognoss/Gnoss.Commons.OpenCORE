﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{

    [Serializable]
    [Table("CMSComponenteRolGrupoIdentidades")]
    public partial class CMSComponenteRolGrupoIdentidades
    {
        [Column(Order = 0)]
        public Guid ComponenteID { get; set; }

        [Column(Order = 1)]
        public Guid GrupoID { get; set; }

        public virtual CMSComponente CMSComponente { get; set; }
    }
}
