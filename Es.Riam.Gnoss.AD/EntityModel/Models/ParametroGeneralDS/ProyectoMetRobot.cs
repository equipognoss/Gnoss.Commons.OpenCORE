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
    [Table("ProyectoMetaRobots")]
    public partial class ProyectoMetaRobots
    {
        [Column(Order = 0)]
        public Guid OrganizacionID { get; set; }

        [Column(Order = 1)]
        public Guid ProyectoID { get; set; }

        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Tipo { get; set; }

        [StringLength(50)]
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            ProyectoMetaRobots objetoParametro = null;
            if (obj.GetType() != typeof(ProyectoMetaRobots))
            {
                return false;
            }
            else if (ReferenceEquals(null, obj))
            {
                return false;
            }
            else
            {
                objetoParametro = (ProyectoMetaRobots)obj;
            }

            if (OrganizacionID.Equals(objetoParametro.OrganizacionID) && ProyectoID.Equals(objetoParametro.ProyectoID) && Tipo.Equals(objetoParametro.Tipo) && Content.Equals(objetoParametro.Content))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
