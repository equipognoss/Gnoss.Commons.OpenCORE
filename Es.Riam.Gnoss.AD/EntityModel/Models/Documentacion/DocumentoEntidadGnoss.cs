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
    [Table("DocumentoEntidadGnoss")]
    public partial class DocumentoEntidadGnoss
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid EntidadGnossID { get; set; }

        [Column(Order = 3)]
        public Guid DocumentoID { get; set; }

        [Column(Order = 4)]
        public Guid CategoriaDocumentacionID { get; set; }
    }
}
