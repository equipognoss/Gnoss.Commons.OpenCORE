using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    public class DocumentoVinculadoDocumento
    {
        public Guid DocumentoID { get; set; }
        public Guid? ProyectoID { get; set; }
        public DateTime Fecha { get; set; }
    }
}
