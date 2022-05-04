using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [NotMapped]
    [Serializable]
    public class ProyectoConAdministrado
    {
        public ProyectoConAdministrado()
        {

        }
        public Guid OrganizacionID {
            get;set;
        }
        public Guid ProyectoID
        {
            get;set;
        }
        public string Nombre
        {
            get; set;
        }
        public int Administrado
        {
            get;set;
        }
    }
}
