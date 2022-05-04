using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [Table("ProyectoEventoParticipante")]
    public partial class ProyectoEventoParticipante
    {
        [Column(Order = 0)]
        public Guid EventoID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }

        public DateTime Fecha { get; set; }

        public virtual ProyectoEvento ProyectoEvento { get; set; }
    }
}
