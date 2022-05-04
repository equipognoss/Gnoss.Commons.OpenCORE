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
    public class EmailsMiembrosDeProyecto
    {// objeto.ProyectoUsuarioIdentidad.IdentidadID, objeto.Perfil.PersonaID, objeto.Persona.Nombre, Email=objeto.PersonaVinculoOrganizacion.EmailTrabajo 
        public Guid IdentidadID { get; set; }
        public Guid? PersonaID { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }

    }
}
