using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [NotMapped]
    public class EmailAdminProyecto
    {
        public EmailAdminProyecto()
        {

        }

        public Guid IdentidadID
        {
            get; set;
        }
        public Guid? PersonaID
        {
            get; set;
        }
        public string Nombre
        {
            get; set;
        }
        public string Email
        {
            get; set;
        }
    }
}
