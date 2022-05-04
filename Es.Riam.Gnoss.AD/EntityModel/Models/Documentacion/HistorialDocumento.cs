using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("HistorialDocumento")]
    public partial class HistorialDocumento
    {
        public Guid HistorialDocumentoID { get; set; }

        public Guid DocumentoID { get; set; }

        public Guid IdentidadID { get; set; }

        public DateTime Fecha { get; set; }

        public string TagNombre { get; set; }

        public Guid? CategoriaTesauroID { get; set; }

        public int Accion { get; set; }

        public Guid ProyectoID { get; set; }

        public virtual Documento Documento { get; set; }
    }
}
