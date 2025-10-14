using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.CMS
{
    [Serializable]
    [Table("CMSComponenteVersion")]
    public partial class CMSComponenteVersion
    {
        [Key]
        public Guid VersionID { get; set; }

        public Guid ComponenteID { get; set; }

        public Guid IdentidadID { get; set; }

        public Guid VersionAnterior { get; set; }

        public DateTime Fecha { get; set; }

        public string Comentario { get; set; }

        public string ModeloJSON { get; set; }
        [ForeignKey("ComponenteID")]
        public virtual CMSComponente CMSComponente {  get; set; }
    }
}
