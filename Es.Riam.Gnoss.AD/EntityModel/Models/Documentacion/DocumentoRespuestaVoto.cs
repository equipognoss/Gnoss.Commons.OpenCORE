using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoRespuestaVoto")]
    public partial class DocumentoRespuestaVoto
    {
        public Guid RespuestaID { get; set; }

        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid IdentidadID { get; set; }

        public virtual Documento Documento { get; set; }
        public virtual DocumentoRespuesta DocumentoRespuesta { get; set; }
    }
}
