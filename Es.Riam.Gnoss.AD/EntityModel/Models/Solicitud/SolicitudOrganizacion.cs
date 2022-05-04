namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("SolicitudOrganizacion")]
    public partial class SolicitudOrganizacion
    {
        [Column(Order = 0)]
        public Guid SolicitudID { get; set; }

        [Column(Order = 1)]
        public Guid OrganizacionID { get; set; }

        public virtual Solicitud Solicitud { get; set; }
    }
}
