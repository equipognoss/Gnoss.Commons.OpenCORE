using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.OrganizacionDS
{
    [Serializable]
    [Table("PersonaOcupacionFormaSec")]
    public partial class PersonaOcupacionFormaSec
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        public Guid EstructuraID { get; set; }

        [Column(Order = 3)]
        public Guid ElementoEstructuraID { get; set; }

        [Column(Order = 4)]
        public Guid PersonaID { get; set; }

        public Guid? OrganizacionPersonalID { get; set; }

        public int Dedicacion { get; set; }

        public short Orden { get; set; }
    }
}
