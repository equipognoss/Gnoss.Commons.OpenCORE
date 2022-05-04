using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    public partial class ContadorPerfil
    {
        [Key]
        public Guid PerfilID { get; set; }

        public int NumComentarios { get; set; }

        public int NumComentContribuciones { get; set; }

        public int NumComentMisRec { get; set; }

        public int NumComentBlog { get; set; }

        public int NumNuevosMensajes { get; set; }

        public int NuevosComentarios { get; set; }

        public DateTime? FechaVisitaComentarios { get; set; }

        public int NuevasInvitaciones { get; set; }

        public int NuevasSuscripciones { get; set; }

        public DateTime? FechaVisitaSuscripciones { get; set; }

        public int NumMensajesSinLeer { get; set; }

        public int NumInvitacionesSinLeer { get; set; }

        public int NumSuscripcionesSinLeer { get; set; }

        public int NumComentariosSinLeer { get; set; }
    }
}
