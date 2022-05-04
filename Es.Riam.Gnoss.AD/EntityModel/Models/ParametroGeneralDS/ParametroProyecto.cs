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
    [Table("ParametroProyecto")]
    public partial class ParametroProyecto
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [StringLength(100)]
        public string Parametro { get; set; }

        public string Valor { get; set; }

        public ParametroProyecto()
        {

        }

        public ParametroProyecto(Guid organizacionID,Guid clave, string nombreParametro,string valor)
        {
            OrganizacionID = organizacionID;
            ProyectoID = clave;
            Parametro=nombreParametro;
            Valor = valor;
        }
    }
}
