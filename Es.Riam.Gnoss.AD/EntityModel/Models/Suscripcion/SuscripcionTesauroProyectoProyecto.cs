﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Suscripcion
{
    [Serializable]
    [NotMapped]
    public partial class SuscripcionTesauroProyectoProyecto
    {
        [Key]
        public Guid SuscripcionID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid TesauroID { get; set; }

        public string NombreProyecto { get; set; }

        public string NombreCortoProyecto { get; set; }

        public virtual Suscripcion Suscripcion { get; set; }
    }
}
