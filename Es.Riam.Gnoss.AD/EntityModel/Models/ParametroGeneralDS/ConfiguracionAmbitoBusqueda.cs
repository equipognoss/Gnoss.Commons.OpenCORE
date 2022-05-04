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
    [Table("ConfiguracionAmbitoBusqueda")]
    public partial class ConfiguracionAmbitoBusqueda 
    { 
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        public bool Metabusqueda { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreAmbitoTodaComunidad { get; set; }

        public bool TodoGnoss { get; set; }

        [Required]
        [StringLength(150)]
        public string DefectoHome { get; set; }
    }
}
