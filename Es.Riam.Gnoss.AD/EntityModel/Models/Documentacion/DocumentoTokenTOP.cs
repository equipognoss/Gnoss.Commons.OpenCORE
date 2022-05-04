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
    [Table("DocumentoTokenTOP")]
    public partial class DocumentoTokenTOP
    {
        [Key]
        public Guid TokenID { get; set; }

        public Guid ProyectoID { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid UsuarioID { get; set; }

        public Guid DocumentoID { get; set; }

        public DateTime FechaCreacion { get; set; }

        public short Estado { get; set; }

        public string NombreArchivo { get; set; }
    }
}
