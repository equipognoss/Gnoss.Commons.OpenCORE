using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Asistente
{
    public class AsistenteConfigIdentidad
    {
        public Guid AsistenteID { get; set; }
        public Guid IdentidadID { get; set; }
        public bool AsistentePorDefecto { get; set; }

        public virtual Asistente Asistente { get; set; }
        public virtual IdentidadDS.Identidad Identidad {  get; set; }
    }
}
