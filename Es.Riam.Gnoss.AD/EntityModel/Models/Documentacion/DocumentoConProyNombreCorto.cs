using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion
{
    [Serializable]
    [NotMapped]
    public class DocumentoConProyNombreCortoProyRelacionadoID
    {
        public Documento Documento { get; set; }
        public string ProyectoNombreCorto { get; set; }
        public string ProyectoRelacionadoNombreCorto { get; set; }
        public Guid ProyectoRelacionadoID { get; set; }
    }
}
