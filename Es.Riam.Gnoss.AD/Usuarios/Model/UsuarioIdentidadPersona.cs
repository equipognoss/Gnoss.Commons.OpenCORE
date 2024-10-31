using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.Usuarios.Model
{
    [NotMapped]
    [Serializable]
   public class UsuarioIdentidadPersona
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public Guid UsuarioID { get; set; }
        public bool? EstaBloqueado { get; set; }
        public string NombreCortoUsu { get; set; }
        public Guid PersonaID { get; set; }
        public short Tipo { get; set; }
        public string Foto { get; set; }
        public bool TwoFactorAuthentication { get; set; }
        public string Login { get; set; }
    }
}
