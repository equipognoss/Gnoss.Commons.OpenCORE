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
    public class DocumentoTieneImagenConNombresCortoProy : Documento
    {   
        public bool TieneImagen { get; set; }
        public Guid ProyectoRelacionadoID { get; set; }
        public string ProyectoRelacionadoNombreCorto { get; set; }
    }
}
