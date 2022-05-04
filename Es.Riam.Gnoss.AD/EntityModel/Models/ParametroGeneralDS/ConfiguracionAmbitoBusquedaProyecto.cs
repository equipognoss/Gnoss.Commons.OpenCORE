using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.Elementos.ParametroGeneralDSName
{
    [Serializable]
    [Table("ConfiguracionAmbitoBusquedaProyecto")]
    public partial class ConfiguracionAmbitoBusquedaProyecto
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        public bool Metabusqueda { get; set; }

        public bool TodoGnoss { get; set; }

        public Guid? PestanyaDefectoID { get; set; }
    }
}
