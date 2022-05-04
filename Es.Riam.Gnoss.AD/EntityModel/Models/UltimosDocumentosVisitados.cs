using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models
{
    public class UltimosDocumentosVisitados
    {
        [Key]
        public Guid ProyectoID { get; set; }
        public string Documentos { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
