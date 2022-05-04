using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documento
{
    [Serializable]
    public partial class DocumentoWebVinBaseRecursos
    {
        [Key]
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid BaseRecursosID { get; set; }

        public Guid? IdentidadPublicacionID { get; set; }

        public DateTime? FechaPublicacion { get; set; }

        public bool? Compartido { get; set; }

        public bool Eliminado { get; set; }

        public int NumeroComentarios { get; set; }

        public int NumeroDescargas { get; set; }

        public int NumeroConsultas { get; set; }

        public int NumeroVotos { get; set; }

        [StringLength(300)]
        public string NombrePublicador { get; set; }

        [StringLength(50)]
        public string NombreCortoPublicador { get; set; }

        public short? TipoIdentidadPublicador { get; set; }

        public Guid? PublicadorOrgID { get; set; }

        [StringLength(255)]
        public string NombreOrgPublicador { get; set; }

        [StringLength(50)]
        public string NombreCortoOrgPublicador { get; set; }

        public bool? IdentPubVisibleExt { get; set; }

        public bool PermiteComentarios { get; set; }

        public Guid? NivelCertificacionID { get; set; }

        public int? Rank { get; set; }

        public double? Rank_Tiempo { get; set; }

        public bool IndexarRecurso { get; set; }

        public bool PrivadoEditores { get; set; }

        public short TipoPublicacion { get; set; }

        public bool LinkAComunidadOrigen { get; set; }

        public DateTime? FechaCertificacion { get; set; }

        public DateTime? FechaUltimaVisita { get; set; }

        public virtual Documento Documento { get; set; }
    }
}
