namespace Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("ConfiguracionGnossPersona")]
    public partial class ConfiguracionGnossPersona
    {
        [Key]
        public Guid PersonaID { get; set; }

        public bool SolicitudesContacto { get; set; }

        public bool MensajesGnoss { get; set; }

        public bool ComentariosRecursos { get; set; }

        public bool InvitacionComunidad { get; set; }

        public bool InvitacionOrganizacion { get; set; }

        public short BoletinSuscripcion { get; set; }

        public bool VerAmigos { get; set; }

        public bool VerAmigosExterno { get; set; }

        public bool VerRecursos { get; set; }

        public bool VerRecursosExterno { get; set; }

        public bool EnviarEnlaces { get; set; }
        [Required]
        [ForeignKey("PersonaID")]
        public virtual Persona Persona { get; set; }
    }
}
