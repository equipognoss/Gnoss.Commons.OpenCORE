using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS
{
    [Serializable]
    [NotMapped]
    public partial class UsuarioAdministradorComunidad
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsuarioAdministradorComunidad()
        {
           
        }

        public Guid OrganizacionID
        {
            get; set;
        }

        public Guid ProyectoID
        {
            get; set;
        }

        public string Nombre
        {
            get; set;
        }

        public short? Tipo
        {
            get; set;
        }

        public int Administrador
        {
            get; set;
        }
    }
}
