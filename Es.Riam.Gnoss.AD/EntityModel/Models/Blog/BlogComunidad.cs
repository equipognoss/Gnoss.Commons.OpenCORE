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
    [Table("BlogComunidad")]
    public partial class BlogComunidad
    {
        [Column(Order = 0)]
        public Guid BlogID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid OrganizacionID { get; set; }

        public bool Compartido { get; set; }

        public virtual Blog Blog { get; set; }
    }
}
