using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("VersionDocumento")]
    public partial class VersionDocumento
    {
        [Key]
        public Guid DocumentoID { get; set; }

        public int Version { get; set; }

        public Guid DocumentoOriginalID { get; set; }

        public Guid IdentidadID { get; set; }

        public bool EsMejora {  get; set; }

        public Guid? EstadoID { get; set; }

        public short EstadoVersion {  get; set; }

        public Guid? MejoraID { get; set; }

        public virtual Documento Documento { get; set; }

        public virtual Estado Estado { get; set; }
    }
}
