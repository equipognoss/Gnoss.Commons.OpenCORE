namespace Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud
{
    using Notificacion;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Serializable]
    [Table("Solicitud")]
    public partial class Solicitud
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Solicitud()
        {
            DatoExtraEcosistemaOpcionSolicitud = new HashSet<DatoExtraEcosistemaOpcionSolicitud>();
            DatoExtraEcosistemaVirtuosoSolicitud = new HashSet<DatoExtraEcosistemaVirtuosoSolicitud>();
            DatoExtraProyectoOpcionSolicitud = new HashSet<DatoExtraProyectoOpcionSolicitud>();
            SolicitudOrganizacion = new HashSet<SolicitudOrganizacion>();
            SolicitudGrupo = new HashSet<SolicitudGrupo>();
            SolicitudUsuario = new HashSet<SolicitudUsuario>();
            SolicitudNuevaOrganizacion = new HashSet<SolicitudNuevaOrganizacion>();
            SolicitudNuevoProfesor = new HashSet<SolicitudNuevoProfesor>();
            NotificacionSolicitud = new HashSet<NotificacionSolicitud>();
        }

        public Guid SolicitudID { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public short Estado { get; set; }

        public Guid OrganizacionID { get; set; }

        public Guid ProyectoID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraEcosistemaOpcionSolicitud> DatoExtraEcosistemaOpcionSolicitud { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraEcosistemaVirtuosoSolicitud> DatoExtraEcosistemaVirtuosoSolicitud { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatoExtraProyectoOpcionSolicitud> DatoExtraProyectoOpcionSolicitud { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudOrganizacion> SolicitudOrganizacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudGrupo> SolicitudGrupo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudUsuario> SolicitudUsuario { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudNuevaOrganizacion> SolicitudNuevaOrganizacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SolicitudNuevoProfesor> SolicitudNuevoProfesor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificacionSolicitud> NotificacionSolicitud { get; set; }
    }
}
