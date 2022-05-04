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
    [Table("DocumentoTipologia")]
    public partial class DocumentoTipologia
    {
        [Column(Order = 0)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 1)]
        public Guid TipologiaID { get; set; }

        [Column(Order = 2)]
        public Guid AtributoID { get; set; }

        public string Valor { get; set; }

        public virtual Tipologia Tipologia { get; set; }
    }
}
