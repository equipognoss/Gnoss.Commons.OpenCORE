using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS
{
    [Serializable]
    [NotMapped]
    public class IdentidadesDeUsuariosDeOrganizacionDeProyectoYDeMetaProyecto
    {
        public Guid? PersonaID { get; set; }
        public Guid? OrganizacionID { get; set; }
        public Guid ProyectoID { get; set; }
        public Guid IdentidadID { get; set; }
        public string NombrePerfil { get; set; }
        public short TipoParticipacion { get; set; }
        public short TipoAdministracion { get; set; }
    }
}
