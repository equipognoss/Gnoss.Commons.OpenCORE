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
    [Table("CMSRolGrupoIdentidades")]
    public partial class CMSRolGrupoIdentidades
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Ubicacion { get; set; }

        [Column(Order = 3)]
        public Guid GrupoID { get; set; }
    }
}
