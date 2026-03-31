using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Asistente
{
    public class RolAsistente
    {
        public Guid AsistenteID {  get; set; }
        public Guid RolID { get; set; }

        public virtual Asistente Asistente { get; set; }
        public virtual Rol Rol { get; set; }
    }
}
