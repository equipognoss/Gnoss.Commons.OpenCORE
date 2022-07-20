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
    [Table("DocumentoNewsletter")]
    public partial class DocumentoNewsletter
    {
        [Key]
        public Guid DocumentoID { get; set; }

        public string NewsletterTemporal { get; set; }

        public string Newsletter { get; set; }
    }
}
