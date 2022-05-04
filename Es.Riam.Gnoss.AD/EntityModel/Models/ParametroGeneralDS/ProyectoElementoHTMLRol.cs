using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS
{
    [Serializable]
    [Table("ProyectoElementoHTMLRol")]
    public partial class ProyectoElementoHTMLRol
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ElementoHeadID { get; set; }

        [Column(Order = 3)]
        public Guid GrupoID { get; set; }

        public virtual ProyectoElementoHtml ProyectoElementoHtml { get; set; }
    }
}
