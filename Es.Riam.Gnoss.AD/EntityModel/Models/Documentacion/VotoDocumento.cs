using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("VotoDocumento")]
    public partial class VotoDocumento
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        public Guid? ProyectoID { get; set; }

        [Column(Order = 1)]
        public Guid VotoID { get; set; }

        public virtual Documento Documento { get; set; }

        public virtual Voto.Voto Voto { get; set; }
    }
}
