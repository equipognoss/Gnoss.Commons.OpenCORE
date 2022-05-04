using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [Table("DocumentoAtributoBiblio")]
    public partial class DocumentoAtributoBiblio
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid AtributoID { get; set; }

        public string Valor { get; set; }

        [Column(Order = 2)]
        public Guid FichaBibliograficaID { get; set; }

        public virtual AtributoFichaBibliografica AtributoFichaBibliografica { get; set; }
    }
}
