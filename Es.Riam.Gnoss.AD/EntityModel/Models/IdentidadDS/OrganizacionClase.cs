using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [Table("OrganizacionClase")]
    public partial class OrganizacionClase
    {
        [Key]
        public Guid OrganizacionID { get; set; }

        [Required]
        [StringLength(250)]
        public string Centro { get; set; }

        [Required]
        [StringLength(250)]
        public string Asignatura { get; set; }

        [Required]
        [StringLength(10)]
        public string Curso { get; set; }

        [StringLength(50)]
        public string Grupo { get; set; }

        [Required]
        [StringLength(10)]
        public string CursoAcademico { get; set; }

        [Required]
        [StringLength(10)]
        public string NombreCortoCentro { get; set; }

        [Required]
        [StringLength(10)]
        public string NombreCortoAsig { get; set; }

        public short TipoClase { get; set; }
    }
}
