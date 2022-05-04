using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion
{
    [Serializable]
    [Table("Notificacion")]
    public partial class Notificacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Notificacion()
        {
            Invitacion = new HashSet<Invitacion>();
            NotificacionAlertaPersona = new HashSet<NotificacionAlertaPersona>();
            NotificacionCorreoPersona = new HashSet<NotificacionCorreoPersona>();
            NotificacionSuscripcion = new HashSet<NotificacionSuscripcion>();
            NotificacionParametro = new HashSet<NotificacionParametro>();
            NotificacionParametroPersona = new HashSet<NotificacionParametroPersona>();
            NotificacionSolicitud = new HashSet<NotificacionSolicitud>();
        }

        public Guid NotificacionID { get; set; }

        [StringLength(5)]
        public string Idioma { get; set; }

        public short MensajeID { get; set; }

        public DateTime? FechaFin { get; set; }

        public DateTime FechaNotificacion { get; set; }

        public Guid? OrganizacionID { get; set; }

        public Guid? ProyectoID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invitacion> Invitacion { get; set; }

        public virtual NotificacionEnvioMasivo NotificacionEnvioMasivo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionAlertaPersona> NotificacionAlertaPersona { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionCorreoPersona> NotificacionCorreoPersona { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionSuscripcion> NotificacionSuscripcion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionParametro> NotificacionParametro { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionParametroPersona> NotificacionParametroPersona { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionSolicitud> NotificacionSolicitud { get; set; }
    }
}
