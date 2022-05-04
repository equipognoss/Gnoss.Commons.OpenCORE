using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Blog
{
    [Serializable]
    [Table("BlogAgCatTesauro")]
    public partial class BlogAgCatTesauro
    {
        [Column(Order = 0)]
        public Guid TesauroID { get; set; }

        [Column(Order = 1)]
        public Guid CategoriaTesauroID { get; set; }

        [Column(Order = 2)]
        public Guid BlogID { get; set; }

        public DateTime Fecha { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
