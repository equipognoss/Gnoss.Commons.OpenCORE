using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Carga
{
    [Table("CargaMasivaConfiguracion")]
    public partial class CargaMasivaConfiguracion
    {
        [Key]
        public Guid ProyectoID { get; set; }

        public DateTime FechaModificacion { get; set; }

        public Guid IdentidadID { get; set; }
    }
}
