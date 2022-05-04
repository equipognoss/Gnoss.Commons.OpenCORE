using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Peticion
{
    [Serializable]
    [Table("Peticion")]
    public partial class Peticion
    {
        [Key]
        public Guid PeticionID { get; set; }

        public DateTime FechaPeticion { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public short Tipo { get; set; }

        public short Estado { get; set; }

        public Guid? UsuarioID { get; set; }

        public virtual PeticionInvitaContacto PeticionInvitaContacto { get; set; }

        public virtual PeticionInvitacionComunidad PeticionInvitacionComunidad { get; set; }

        public virtual PeticionOrgInvitaPers PeticionOrgInvitaPers { get; set; }

        public virtual PeticionInvitacionGrupo PeticionInvitacionGrupo { get; set; }

        public virtual PeticionNuevoProyecto PeticionNuevoProyecto { get; set; }
    }
}
