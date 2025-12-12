using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.EntityModelBASE.Models;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Notificacion
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración para los tipos de notificaciones
    /// </summary>
    public enum TiposNotificacion
    {

        #region Solicitudes 20..39

        /// <summary>
        /// Solicitud pendiente
        /// </summary>
        SolicitudPendiente = 20,//2
        /// <summary>
        /// Solicitud de acceso a proyecto aceptada
        /// </summary>
        SolicitudAceptadaAccesoProyecto = 21,//4,
        /// <summary>
        /// Solicitud de acceso a proyecto rechazada
        /// </summary>
        SolicitudRechazada = 22,//5,
        /// <summary>
        /// Solicitud de aviso
        /// </summary>
        SolicitudAviso = 23,//6,
        /// <summary>
        /// Solicitud conjunta de nueva organización y nuevo usuario
        /// </summary>
        SolicitudNuevaOrganizacionYUsuario = 24,//16,       
        /// <summary>
        /// Solicitud de nuevo usuario
        /// </summary>
        SolicitudNuevoUsuario = 26,//18,  
        /// <summary>
        /// Solicitud de nuevo usuario
        /// </summary>
        SolicitudNuevoUsuarioTutor = 27,//18, 
        /// <summary>
        /// Solicitud de creación de nueva comunidad
        /// </summary>
        SolicitudCreacionComunidad = 30,
        /// <summary>
        /// Solicitud aceptada de creación de nueva comunidad
        /// </summary>
        SolicitudAceptadaNuevaComunidad = 31,
        /// <summary>
        /// Solicitud rechazada de creación de nueva comunidad
        /// </summary>
        SolicitudRechazadaNuevaComunidad = 32,
        /// <summary>
        /// Solicitud de nuevo profesor
        /// </summary>
        SolicitudNuevoProfesor = 33,
        /// <summary>
        /// Solicitud aceptada de registro de clase.
        /// </summary>
        SolicitudAceptadaNuevaClase = 35,
        /// <summary>
        /// Solicitud aceptada de registro de clases.
        /// </summary>
        SolicitudAceptadaNuevaClases = 36,
        /// <summary>
        /// Notificación al tutor o padres del registro de un menor
        /// </summary>
        SolicitudNuevoUsuarioNotificacionTutor = 37,

        #endregion

        #region Invitaciones 40..59

        /// <summary>
        /// Notificación de invitación pendiente
        /// </summary>
        InvitacionPendiente = 40,//11,
        /// <summary>
        /// Notificación de invitación para ser contacto
        /// </summary>
        InvitacionContacto = 41,//15,
        /// <summary>
        /// Notificación de confirmación de acceso a comunidad para registros parciales
        /// </summary>
        InvitacionRegistroParcialComunidad = 42,
        /// <summary>
        /// Notificación de invitación a un usuario para ser miembro de comunidad
        /// </summary>
        InvitacionUsuarioACom = 43,
        /// <summary>
        /// Notificación de invitación a correo externo para ser miembro de comunidad
        /// </summary>
        InvitacionExternoACom = 44,
        /// <summary>
        /// Notificación de invitación a un usuario para ser miembro de una organización en modo corporativo
        /// </summary>
        InvitacionUsuarioAOrgCorp = 45,
        /// <summary>
        /// Notificación de invitación a un usuario para ser miembro de una organización en modo personal
        /// </summary>
        InvitacionUsuarioAOrgPers = 46,
        /// <summary>
        /// Notificación de invitación a correo externo para ser miembro de una organización en modo corporativo
        /// </summary>
        InvitacionExternoAOrgCorp = 47,
        /// <summary>
        /// Notificación de invitación a correo externo para ser miembro de una organización en modo personal
        /// </summary>
        InvitacionExternoAOrgPers = 48,
        /// <summary>
        /// Notificación de invitación a correo de un usuario para ser miembro de comunidad
        /// </summary>
        InvitacionCorreoUsuACom = 49,
        /// <summary>
        /// Notificación de invitación a correo de un usuario para ser miembro de una organización en modo corporativo
        /// </summary>
        InvitacionCorreoUsuAOrgCorp = 50,
        /// <summary>
        /// Notificación de invitación a correo de un usuario para ser miembro de una organización en modo personal
        /// </summary>
        InvitacionCorreoUsuAOrgPers = 51,
        /// <summary>
        /// Notificación de invitación por correo para ser contacto
        /// </summary>
        InvitacionCorreoContacto = 52,
        /// <summary>
        /// Notificación de confirmación de solicitud de contacto
        /// </summary>
        ConfirmacionContacto = 54,
        /// <summary>
        /// Notificación de eliminación de contacto
        /// </summary>
        EliminarContacto = 56,
        /// <summary>
        /// Notificación de invitación para ser contacto de una organización
        /// </summary>
        InvitacionContactoOrg = 58,
        /// <summary>
        /// Notificación de cambio de contacto a una organización
        /// </summary>
        CambioContactoAOrg = 59,

        #endregion

        #region Suscripciones 60..79

        /// <summary>
        /// Notificación de boletín de suscripciones
        /// </summary>
        BoletinSuscripcion = 60,//666,

        /// <summary>
        /// Notificación de persona suscrita a tu perfil
        /// </summary>
        SeguirPerfil = 61,

        /// <summary>
        /// Notificación de persona suscrita a tu perfil en comunidad
        /// </summary>
        SeguirPerfilComunidad = 62,

        #endregion

        #region Peticiones 80..99

        /// <summary>
        /// Notifica una petición de cambio de password
        /// </summary>
        PeticionCambioPassword = 80,//12,

        /// <summary>
        /// Notifica una petición de autenticación de doble factor
        /// </summary>
        PeticionAutenticacionDobleFactor = 81,

        #endregion

        #region Sugerencia 100..119

        #endregion

        #region Avisos 120..139

        /// <summary>
        /// Notificación de aviso por correo de nuevo contacto
        /// </summary>
        AvisoCorreoNuevoContacto = 121,
        /// <summary>
        /// Notificación de aviso de cierre temporal de una comunidad
        /// </summary>
        AvisoCierreTemp = 122,
        /// <summary>
        /// Notificación de aviso de cierre definitivo de una comunidad
        /// </summary>
        AvisoCierreDefinitivo = 123,
        /// <summary>
        /// Notificación de aviso de apertura de una comunidad
        /// </summary>
        AvisoAperturaComunidad = 124,
        /// <summary>
        /// Notificación de aviso de reapertura de una comunidad
        /// </summary>
        AvisoReaperturaComunidad = 125,
        /// <summary>
        /// Notificación de aviso de expulsión de un usuario de una comunidad
        /// </summary>
        AvisoExpulsionUsuarioDeComunidad = 126,
        /// <summary>
        /// Notificación de aviso de corrección de identidad
        /// </summary>
        AvisoCorreccionDeIdentidad = 127,
        /// <summary>
        /// Notificación de aviso de corrección de identidad definitivo
        /// </summary>
        AvisoCorreccionDeIdentidadDefinitivo = 128,
        /// <summary>
        /// Notificación de aviso de eliminación de usuario
        /// </summary>
        AvisoEliminacionDeUsuario = 129,
        /// <summary>
        /// Notificación de aviso de registro en evento de comunidad
        /// </summary>
        AvisoRegistroEventoComunidad = 130,
        /// <summary>
        /// Notificación de aviso por correo de nuevo contacto
        /// </summary>
        AvisoCorreoBienvenidaProyecto = 131,

        AvisoCambioEstadoRecurso = 132,

        AvisoMejoraAplicada = 133,
        AvisoMejoraCancelada = 134,

        #endregion

        #region Envio Enlace 140..149

        /// <summary>
        /// Notificación para envío de enlace
        /// </summary>
        NotificacionEnlaceExterno = 140,
        /// <summary>
        /// Notificación para envío de enlace
        /// </summary>
        NotificacionEnlaceReporteRecurso = 141,

        #endregion

        #region Invitaciones (2ª parte) 150..159

        /// <summary>
        /// Notificación de contacto eliminado
        /// </summary>
        ContactoEliminado = 150,
        /// <summary>
        /// Invitación de un profesor al correo externo de un alumno para que participe en una clase
        /// </summary>
        InvitacionExternoAClase = 151,
        /// <summary>
        /// Notificación de invitación a un usuario para ser miembro de comunidad y pertenecer a grupos
        /// </summary>
        InvitacionUsuarioAComYGrupo = 152,
        /// <summary>
        /// Notificación de invitación a correo externo para ser miembro de comunidad y pertenecer a grupos
        /// </summary>
        InvitacionExternoAComYGrupo = 153,

        /// <summary>
        /// Notificación de invitación a correo externo para ser miembro de comunidad y pertenecer a grupos
        /// </summary>
        InvitacionExternoAClaseYGrupo = 154,

        #endregion

        #region Comentarios 160..179

        /// <summary>
        /// Notificación de comentario hecho a un documento
        /// </summary>
        ComentarioDocumento = 160,
        /// <summary>
        /// Notificación por correo de comentario hecho a un documento
        /// </summary>
        ComentarioDocumentoCorreo = 161,

        #endregion

        #region Noticias 180..199

        /// <summary>
        /// Notificación genérica
        /// </summary>
        NotificacionGenerica = 180,
        /// <summary>
        /// Notificación de nueva categoría sugerida en comunidad
        /// </summary>
        CategoriaSugeridaEnComunidad = 181,
        /// <summary>
        /// Notificación correo tutor
        /// </summary>
        MensajeTutor = 182,

        #endregion

        #region Estado de envíos a Twitter 200..219

        /// <summary>
        /// Envío incorrecto a canal Twitter 
        /// </summary>
        NotificacionEnvioTwitterIncorrecto = 200,

        #endregion

        #region Notificaciones inicio sesión 400..420

        /// <summary>
        /// Cambio de condiciones de uso
        /// </summary>
        CambioCondicionesUso = 400,

        #endregion

        #region Especiales bajo demanda 500..599

        /// <summary>
        /// Solicitud de nuevo usuario para las comunidades de Corporate Excellence.
        /// </summary>
        SolicitudNuevoUsuarioCorporateExcellence = 500

        #endregion
    }

    /// <summary>
    /// Enumeración que indica el estado de una invitación
    /// </summary>
    public enum EstadoInvitacion
    {
        /// <summary>
        /// Pendiente
        /// </summary>
        Pendiente = 0,
        /// <summary>
        /// Aceptada
        /// </summary>
        Aceptada = 1,
        /// <summary>
        /// Rechazada
        /// </summary>
        Rechazada = 2
    }

    /// <summary>
    /// Enumeración que indica el estado de un envío
    /// </summary>
    public enum EstadoEnvio
    {
        /// <summary>
        /// Pendiente
        /// </summary>
        Pendiente = 0,
        /// <summary>
        /// Enviado
        /// </summary>
        Enviado = 1,
        /// <summary>
        /// Error en el envío
        /// </summary>
        Error = 2,
        /// <summary>
        /// Envío cancelado
        /// </summary>
        Cancelado = 3
    }

    #endregion

    public class JoinNotificacionNotificacionAlertaPersona
    {
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionAlertaPersona NotificacionAlertaPersona { get; set; }
    }

    public class JoinNotificacionNotificacionSolicitud
    {
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSolicitud NotificacionSolicitud { get; set; }
    }

    public class JoinNotificacionParametroNotificacion
    {
        public NotificacionParametro NotificacionParametro { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
    }

    public class JoinNotificacionParametroNotificacionNotificacionSolicitud
    {
        public NotificacionParametro NotificacionParametro { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSolicitud NotificacionSolicitud { get; set; }
    }

    public class JoinNotificacionParametroPersonaNotificacion
    {
        public NotificacionParametroPersona NotificacionParametroPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
    }

    public class JoinNotificacionParametroPersonaNotificacionNotificacionSolicitud
    {
        public NotificacionParametroPersona NotificacionParametroPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSolicitud NotificacionSolicitud { get; set; }
    }

    public class JoinNotificacionAlertaPersonaNotificacion
    {
        public NotificacionAlertaPersona NotificacionAlertaPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
    }

    public class JoinNotificacionAlertaPersonaNotificacionNotificacionSolicitud
    {
        public NotificacionAlertaPersona NotificacionAlertaPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSolicitud NotificacionSolicitud { get; set; }
    }

    public class JoinNotificacionCorreoPersonaNotificacion
    {
        public NotificacionCorreoPersona NotificacionCorreoPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
    }

    public class JoinNotificacionCorreoPersonaNotificacionNotificacionSolicitud
    {
        public NotificacionCorreoPersona NotificacionCorreoPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSolicitud NotificacionSolicitud { get; set; }
    }

    public class JoinNotificacionNotificacionSuscripcion
    {
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
    }

    public class JoinNotificacionParametroNotificacionNotificacionSuscripcion
    {
        public NotificacionParametro NotificacionParametro { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
    }

    public class JoinNotificacionParametroPersonaNotificacionNotificacionSuscripcion
    {
        public NotificacionParametroPersona NotificacionParametroPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
    }

    public class JoinNotificacionAlertaPersonaNotificacionNotificacionSuscripcion
    {
        public NotificacionAlertaPersona NotificacionAlertaPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
    }
    public class JoinNotificacionCorreoPersonaNotificacionNotificacionSuscripcion
    {
        public NotificacionCorreoPersona NotificacionCorreoPersona { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
    }

    public class JoinNotificacionSuscripcionSuscripcion
    {
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
    }

    public class JoinNotificacionSuscripcionSuscripcionIdentidad
    {
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinNotificacionSuscripcionSuscripcionIdentidadPerfil
    {
        public NotificacionSuscripcion NotificacionSuscripcion { get; set; }
        public EntityModel.Models.Suscripcion.Suscripcion Suscripcion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
        public Perfil Perfil { get; set; }
    }

    public class JoinNotificacionNotificacionCorreoPersona
    {
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionCorreoPersona NotificacionCorreoPersona { get; set; }
    }

    public class JoinInvitacionIdentidad
    {
        public Invitacion Invitacion { get; set; }
        public EntityModel.Models.IdentidadDS.Identidad Identidad { get; set; }
    }

    public class JoinNotificacionInvitacion
    {
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public Invitacion Invitacion { get; set; }
    }

    public class JoinNotificacionParametroNotificacionInvitacion
    {
        public NotificacionParametro NotificacionParametro { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public Invitacion Invitacion { get; set; }
    }

    public class JoinNotificacionNotificacionEnvioMasivo
    {
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionEnvioMasivo NotificacionEnvioMasivo { get; set; }
    }

    public class JoinNotificacionParametroNotificacionNotificacionEnvioMasivo
    {
        public NotificacionParametro NotificacionParametro { get; set; }
        public EntityModel.Models.Notificacion.Notificacion Notificacion { get; set; }
        public NotificacionEnvioMasivo NotificacionEnvioMasivo { get; set; }
    }

    //INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionParametro.NotificacionID INNER JOIN NotificacionEnvioMasivo ON NotificacionEnvioMasivo.NotificacionID = Notificacion.NotificacionID

    public static class Joins
    {
        public static IQueryable<JoinNotificacionParametroNotificacionNotificacionEnvioMasivo> JoinNotificacionEnvioMasivo(this IQueryable<JoinNotificacionParametroNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionEnvioMasivo, item => item.Notificacion.NotificacionID, notificacionEnvioMasivo => notificacionEnvioMasivo.NotificacionID, (item, notificacionEnvioMasivo) => new JoinNotificacionParametroNotificacionNotificacionEnvioMasivo
            {
                NotificacionEnvioMasivo = notificacionEnvioMasivo,
                Notificacion = item.Notificacion,
                NotificacionParametro = item.NotificacionParametro
            });
        }

        public static IQueryable<JoinNotificacionNotificacionEnvioMasivo> JoinNotificacionEnvioMasivo(this IQueryable<EntityModel.Models.Notificacion.Notificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionEnvioMasivo, notificacion => notificacion.NotificacionID, notificacionEnvioMasivo => notificacionEnvioMasivo.NotificacionID, (notificacion, notificacionEnvioMasivo) => new JoinNotificacionNotificacionEnvioMasivo
            {
                NotificacionEnvioMasivo = notificacionEnvioMasivo,
                Notificacion = notificacion
            });
        }

        public static IQueryable<JoinNotificacionNotificacionCorreoPersona> JoinNotificacionCorreoPersona(this IQueryable<EntityModel.Models.Notificacion.Notificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionCorreoPersona, notificacion => notificacion.NotificacionID, notificacionCorreoPersona => notificacionCorreoPersona.NotificacionID, (notificacion, notificacionCorreoPersona) => new JoinNotificacionNotificacionCorreoPersona
            {
                NotificacionCorreoPersona = notificacionCorreoPersona,
                Notificacion = notificacion
            });
        }

        public static IQueryable<JoinNotificacionParametroNotificacionInvitacion> JoinInvitacion(this IQueryable<JoinNotificacionParametroNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Invitacion, item => item.Notificacion.NotificacionID, invitacion => invitacion.NotificacionID, (item, invitacion) => new JoinNotificacionParametroNotificacionInvitacion
            {
                Invitacion = invitacion,
                Notificacion = item.Notificacion,
                NotificacionParametro = item.NotificacionParametro
            });
        }

        public static IQueryable<JoinNotificacionInvitacion> JoinInvitacion(this IQueryable<EntityModel.Models.Notificacion.Notificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Invitacion, notificacion => notificacion.NotificacionID, invitacion => invitacion.NotificacionID, (notificacion, invitacion) => new JoinNotificacionInvitacion
            {
                Invitacion = invitacion,
                Notificacion = notificacion
            });
        }

        public static IQueryable<JoinInvitacionIdentidad> JoinIdentidad(this IQueryable<Invitacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, invitacion => invitacion.IdentidadDestinoID, identidad => identidad.IdentidadID, (invitacion, identidad) => new JoinInvitacionIdentidad
            {
                Identidad = identidad,
                Invitacion = invitacion
            });
        }

        public static IQueryable<JoinNotificacionSuscripcionSuscripcionIdentidadPerfil> JoinPerfil(this IQueryable<JoinNotificacionSuscripcionSuscripcionIdentidad> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new JoinNotificacionSuscripcionSuscripcionIdentidadPerfil
            {
                Perfil = perfil,
                NotificacionSuscripcion = item.NotificacionSuscripcion,
                Suscripcion = item.Suscripcion,
                Identidad = item.Identidad
            });
        }

        public static IQueryable<JoinNotificacionSuscripcionSuscripcionIdentidad> JoinIdentidad(this IQueryable<JoinNotificacionSuscripcionSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Identidad, item => item.Suscripcion.IdentidadID, identidad => identidad.IdentidadID, (item, identidad) => new JoinNotificacionSuscripcionSuscripcionIdentidad
            {
                Identidad = identidad,
                NotificacionSuscripcion = item.NotificacionSuscripcion,
                Suscripcion = item.Suscripcion
            });
        }

        public static IQueryable<JoinNotificacionSuscripcionSuscripcion> JoinSuscripcion(this IQueryable<NotificacionSuscripcion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Suscripcion, notificacionSuscripcion => notificacionSuscripcion.SuscripcionID, suscripcion => suscripcion.SuscripcionID, (notificacionSuscripcion, suscripcion) => new JoinNotificacionSuscripcionSuscripcion
            {
                Suscripcion = suscripcion,
                NotificacionSuscripcion = notificacionSuscripcion
            });
        }

        public static IQueryable<JoinNotificacionCorreoPersonaNotificacionNotificacionSuscripcion> JoinNotificacionSuscripcion(this IQueryable<JoinNotificacionCorreoPersonaNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSuscripcion, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSuscripcion) => new JoinNotificacionCorreoPersonaNotificacionNotificacionSuscripcion
            {
                NotificacionSuscripcion = notificacionSuscripcion,
                Notificacion = item.Notificacion,
                NotificacionCorreoPersona = item.NotificacionCorreoPersona
            });
        }

        public static IQueryable<JoinNotificacionAlertaPersonaNotificacionNotificacionSuscripcion> JoinNotificacionSuscripcion(this IQueryable<JoinNotificacionAlertaPersonaNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSuscripcion, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSuscripcion) => new JoinNotificacionAlertaPersonaNotificacionNotificacionSuscripcion
            {
                NotificacionSuscripcion = notificacionSuscripcion,
                Notificacion = item.Notificacion,
                NotificacionAlertaPersona = item.NotificacionAlertaPersona
            });
        }

        public static IQueryable<JoinNotificacionParametroPersonaNotificacionNotificacionSuscripcion> JoinNotificacionSuscripcion(this IQueryable<JoinNotificacionParametroPersonaNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSuscripcion, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSuscripcion) => new JoinNotificacionParametroPersonaNotificacionNotificacionSuscripcion
            {
                NotificacionSuscripcion = notificacionSuscripcion,
                Notificacion = item.Notificacion,
                NotificacionParametroPersona = item.NotificacionParametroPersona
            });
        }

        public static IQueryable<JoinNotificacionParametroNotificacionNotificacionSuscripcion> JoinNotificacionSuscripcion(this IQueryable<JoinNotificacionParametroNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSuscripcion, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSuscripcion) => new JoinNotificacionParametroNotificacionNotificacionSuscripcion
            {
                NotificacionSuscripcion = notificacionSuscripcion,
                Notificacion = item.Notificacion,
                NotificacionParametro = item.NotificacionParametro
            });
        }

        public static IQueryable<JoinNotificacionNotificacionSuscripcion> JoinNotificacionSuscripcion(this IQueryable<EntityModel.Models.Notificacion.Notificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSuscripcion, notificacion => notificacion.NotificacionID, notificacionSuscripcion => notificacionSuscripcion.NotificacionID, (notificacion, notificacionSuscripcion) => new JoinNotificacionNotificacionSuscripcion
            {
                NotificacionSuscripcion = notificacionSuscripcion,
                Notificacion = notificacion
            });
        }

        public static IQueryable<JoinNotificacionCorreoPersonaNotificacionNotificacionSolicitud> JoinNotificacionSolicitud(this IQueryable<JoinNotificacionCorreoPersonaNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSolicitud, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSolicitud) => new JoinNotificacionCorreoPersonaNotificacionNotificacionSolicitud
            {
                NotificacionSolicitud = notificacionSolicitud,
                Notificacion = item.Notificacion,
                NotificacionCorreoPersona = item.NotificacionCorreoPersona
            });
        }

        public static IQueryable<JoinNotificacionCorreoPersonaNotificacion> JoinNotificacion(this IQueryable<NotificacionCorreoPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Notificacion, notificacionCorreoPersona => notificacionCorreoPersona.NotificacionID, notificacion => notificacion.NotificacionID, (notificacionCorreoPersona, notificacion) => new JoinNotificacionCorreoPersonaNotificacion
            {
                Notificacion = notificacion,
                NotificacionCorreoPersona = notificacionCorreoPersona
            });
        }

        public static IQueryable<JoinNotificacionAlertaPersonaNotificacionNotificacionSolicitud> JoinNotificacionSolicitud(this IQueryable<JoinNotificacionAlertaPersonaNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSolicitud, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSolicitud) => new JoinNotificacionAlertaPersonaNotificacionNotificacionSolicitud
            {
                NotificacionSolicitud = notificacionSolicitud,
                Notificacion = item.Notificacion,
                NotificacionAlertaPersona = item.NotificacionAlertaPersona
            });
        }

        public static IQueryable<JoinNotificacionAlertaPersonaNotificacion> JoinNotificacion(this IQueryable<NotificacionAlertaPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Notificacion, NotificacionAlertaPersona => NotificacionAlertaPersona.NotificacionID, notificacion => notificacion.NotificacionID, (notificacionAlertaPersona, notificacion) => new JoinNotificacionAlertaPersonaNotificacion
            {
                Notificacion = notificacion,
                NotificacionAlertaPersona = notificacionAlertaPersona
            });
        }

        public static IQueryable<JoinNotificacionParametroPersonaNotificacion> JoinNotificacion(this IQueryable<NotificacionParametroPersona> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Notificacion, notificacionParametroPersona => notificacionParametroPersona.NotificacionID, notificacion => notificacion.NotificacionID, (notificacionParametroPersona, notificacion) => new JoinNotificacionParametroPersonaNotificacion
            {
                Notificacion = notificacion,
                NotificacionParametroPersona = notificacionParametroPersona
            });
        }

        public static IQueryable<JoinNotificacionParametroPersonaNotificacionNotificacionSolicitud> JoinNotificacionSolicitud(this IQueryable<JoinNotificacionParametroPersonaNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSolicitud, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSolicitud) => new JoinNotificacionParametroPersonaNotificacionNotificacionSolicitud
            {
                NotificacionSolicitud = notificacionSolicitud,
                Notificacion = item.Notificacion,
                NotificacionParametroPersona = item.NotificacionParametroPersona
            });
        }

        public static IQueryable<JoinNotificacionParametroNotificacionNotificacionSolicitud> JoinNotificacionSolicitud(this IQueryable<JoinNotificacionParametroNotificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSolicitud, item => item.Notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (item, notificacionSolicitud) => new JoinNotificacionParametroNotificacionNotificacionSolicitud
            {
                NotificacionSolicitud = notificacionSolicitud,
                Notificacion = item.Notificacion,
                NotificacionParametro = item.NotificacionParametro
            });
        }

        public static IQueryable<JoinNotificacionParametroNotificacion> JoinNotificacion(this IQueryable<NotificacionParametro> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Notificacion, notificacionParametro => notificacionParametro.NotificacionID, notificacion => notificacion.NotificacionID, (notificacionParametro, notificacion) => new JoinNotificacionParametroNotificacion
            {
                Notificacion = notificacion,
                NotificacionParametro = notificacionParametro
            });
        }

        public static IQueryable<JoinNotificacionNotificacionSolicitud> JoinNotificacionSolicitud(this IQueryable<EntityModel.Models.Notificacion.Notificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionSolicitud, notificacion => notificacion.NotificacionID, notificacionSolicitud => notificacionSolicitud.NotificacionID, (notificacion, notificacionSolicitud) => new JoinNotificacionNotificacionSolicitud
            {
                Notificacion = notificacion,
                NotificacionSolicitud = notificacionSolicitud
            });
        }

        public static IQueryable<JoinNotificacionNotificacionAlertaPersona> JoinNotificacionAlertaPersona(this IQueryable<EntityModel.Models.Notificacion.Notificacion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.NotificacionAlertaPersona, notificacion => notificacion.NotificacionID, notificacionAlertaPersona => notificacionAlertaPersona.NotificacionID, (notificacion, notificacionAlertaPersona) => new JoinNotificacionNotificacionAlertaPersona
            {
                NotificacionAlertaPersona = notificacionAlertaPersona,
                Notificacion = notificacion
            });
        }
    }

    /// <summary>
    /// DataAdapter de notificaciones
    /// </summary>
    public class NotificacionAD : BaseAD
    {
        private EntityContext mEntityContext;
        private EntityContextBASE mEntityContextBase;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        //Constructores añadidos para uso del Servicio de envío asíncrono de correo
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public NotificacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<NotificacionAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mEntityContext = entityContext;
            mEntityContextBase = entityContextBASE;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();
        }

        public NotificacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<NotificacionAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public NotificacionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<NotificacionAD> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectNotificacionAlertaPersona;
        private string sqlSelectNotificacionCorreoPersona;
        private string sqlSelectNotificacionSolicitud;
        private string sqlSelectNotificacionSuscripcion;
        private string sqlSelectNotificacion;
        private string sqlSelectNotificacionDafo;
        private string sqlSelectNotificacionParametro;
        private string sqlSelectNotificacionParametroPersona;
        private string sqlSelectNotificacionesDeDafo;
        private string sqlSelectNotificacionesAlertasPersonaDeDafo;
        private string sqlSelectNotificacionesCorreosPersonaDeDafo;
        private string sqlSelectNotificacionesParametroDeDafo;
        private string sqlSelectNotificacionDafoDeDafo;
        private string sqlSelectAlertasDePersona;
        private string sqlSelectNotificacionesDePersona;
        private string sqlSelectNotificacionesAlertaPersonaDePersona;
        private string sqlSelectNotificacionesParametroDePersona;
        private string sqlUpdateAlertasCaducasDePersonaALeidas;
        private string sqlUpdateAlertasDafoPersonaDePersonaALeida;
        private string sqlUpdateAlertasDafoFactorNuevoANoLeidas;
        private string sqlSelectInvitacion;
        private string sqlSelectInvitacionSimple;
        private string sqlSelectNotificacionesReaperturaDeProyecto;
        private string sqlSelectNotificacionesCorreosPersonaReaperturaDeProyecto;
        private string sqlSelectNotificacionParametroReaperturaDeProyecto;
        private string sqlSelectNotificacionEnvioMasivo;
        private string sqlSelectEmailIncorrecto;

        private string sqlSelectColaTwitter;

        #endregion

        #region DataAdapter

        #region NotificacionDataAdapter

        private string sqlNotificacionInsert;
        private string sqlNotificacionDelete;
        private string sqlNotificacionModify;

        #endregion

        #region NotificacionAlertaPersonaDataAdapter

        private string sqlNotificacionAlertaPersonaInsert;
        private string sqlNotificacionAlertaPersonaDelete;
        private string sqlNotificacionAlertaPersonaModify;

        #endregion

        #region NotificacionCorreoPersonaDataAdapter

        private string sqlNotificacionCorreoPersonaInsert;
        private string sqlNotificacionCorreoPersonaDelete;
        private string sqlNotificacionCorreoPersonaModify;

        #endregion

        #region NotificacionParametroDataAdapter

        private string sqlNotificacionParametroInsert;
        private string sqlNotificacionParametroDelete;
        private string sqlNotificacionParametroModify;

        #endregion

        #region NotificacionDataDafoAdapter

        private string sqlNotificacionDafoInsert;
        private string sqlNotificacionDafoDelete;
        private string sqlNotificacionDafoModify;

        #endregion

        #region NotificacionSolicitudDataAdapter

        private string sqlNotificacionSolicitudInsert;
        private string sqlNotificacionSolicitudDelete;
        private string sqlNotificacionSolicitudModify;

        #endregion

        #region NotificacionSuscripcionDataAdapter

        private string sqlNotificacionSuscripcionInsert;
        private string sqlNotificacionSuscripcionModify;
        private string sqlNotificacionSuscripcionDelete;

        #endregion

        #region NotificacionParametroPersonaDataAdapter

        private string sqlNotificacionParametroPersonaInsert;
        private string sqlNotificacionParametroPersonaDelete;
        private string sqlNotificacionParametroPersonaModify;

        #endregion

        #region NotificacionEnvioMasivo

        private string sqlNotificacionEnvioMasivoInsert;
        private string sqlNotificacionEnvioMasivoDelete;
        private string sqlNotificacionEnvioMasivoModify;

        #endregion

        #region Invitacion

        private string sqlInvitacionInsert;
        private string sqlInvitacionDelete;
        private string sqlInvitacionModify;

        #endregion

        #region EmailIncorrecto

        private string sqlEmailIncorrectoInsert;
        private string sqlEmailIncorrectoDelete;
        private string sqlEmailIncorrectoModify;

        #endregion

        #region ColaTwitter
        private string sqlColaTwitterInsert;
        private string sqlColaTwitterDelete;
        private string sqlColaTwitterModify;
        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Elimina una notificación de solicitud cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pNotificacionID">Identificador de notificación</param>
        public void EliminarNotificacionSolicitud(Guid pNotificacionID)
        {
            NotificacionSolicitud notificacionSolicitud = mEntityContext.NotificacionSolicitud.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            mEntityContext.EliminarElemento(notificacionSolicitud);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Elimina una notificación de correo a persona cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pNotificacionID">Identificador de notificación</param>
        public void EliminarNotificacionCorreoPersona(Guid pNotificacionID)
        {
            NotificacionCorreoPersona notificacionCorreoPersona = mEntityContext.NotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            mEntityContext.EliminarElemento(notificacionCorreoPersona);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Elimina una notificación de parámetros de persona cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pNotificacionID">Identificador de notificación</param>
        public void EliminarNotificacionParametroPersona(Guid pNotificacionID)
        {
            NotificacionParametroPersona notificacionParametroPersona = mEntityContext.NotificacionParametroPersona.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            mEntityContext.EliminarElemento(notificacionParametroPersona);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Elimina una notificación de parámetro cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pNotificacionID">Identificador de notificación</param>
        public void EliminarNotificacionParametro(Guid pNotificacionID)
        {
            NotificacionParametro notificacionParametro = mEntityContext.NotificacionParametro.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            mEntityContext.NotificacionParametro.Remove(notificacionParametro);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Elimina una notificación cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pNotificacionID">Identificador de notificación</param>
        public void EliminarNotificacion(Guid pNotificacionID)
        {
            EntityModel.Models.Notificacion.Notificacion notificacion = mEntityContext.Notificacion.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            mEntityContext.EliminarElemento(notificacion);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Elimina una notificación de alerta a persona cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pNotificacionID">Identificador de notificación</param>
        public void EliminarNotificacionAlertaPersona(Guid pNotificacionID)
        {
            NotificacionAlertaPersona notificacionAlertaPersona = mEntityContext.NotificacionAlertaPersona.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            mEntityContext.EliminarElemento(notificacionAlertaPersona);

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Actualiza la tabla NotificacionAlertaPersona y pone una fecha lectura a la notificiación de la persona que sea con un mensaje id igual al pasado
        /// </summary>
        /// <param name="pPersonaID">Guid de la persona afectada</param>
        /// <param name="pMensajeID">Entero referente a la notificación que se actualiza</param>
        public void MarcarNotificacionPersonaLeida(Guid pPersonaID, int pMensajeID)
        {
            Guid notificacionID = mEntityContext.Notificacion.JoinNotificacionAlertaPersona().Where(item => item.Notificacion.MensajeID.Equals(pMensajeID) && item.NotificacionAlertaPersona.PersonaID.Equals(pPersonaID)).Select(item => item.Notificacion.NotificacionID).FirstOrDefault();


            NotificacionAlertaPersona notificacionAlertaPersona = mEntityContext.Notificacion.JoinNotificacionAlertaPersona().Where(item => item.Notificacion.NotificacionID.Equals(notificacionID) && item.NotificacionAlertaPersona.PersonaID.Equals(pPersonaID)).Select(item => item.NotificacionAlertaPersona).FirstOrDefault();

            notificacionAlertaPersona.FechaLectura = DateTime.Parse(IBD.CapturarFechaSinHora());

            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene todas las notificaciones relacionadas con una lista de solicitudes pasada como parámetro
        /// </summary>
        public DataWrapperNotificacion ObtenerNotificacionesDeSolicitudes(List<Guid> pLista)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            if (pLista.Count > 0)
            {
                //Notificacion
                notificacionDW.ListaNotificacion = mEntityContext.Notificacion.JoinNotificacionSolicitud().Where(item => pLista.Contains(item.NotificacionSolicitud.SolicitudID)).Select(item => item.Notificacion).ToList();

                //NotificacionSolicitud
                notificacionDW.ListaNotificacionSolicitud = mEntityContext.NotificacionSolicitud.Where(item => pLista.Contains(item.SolicitudID)).ToList();

                //NotificacionParametro
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.JoinNotificacion().JoinNotificacionSolicitud().Where(item => pLista.Contains(item.NotificacionSolicitud.SolicitudID)).Select(item => item.NotificacionParametro).ToList();

                //NotificacionParametroPersona
                notificacionDW.ListaNotificacionParametroPersona = mEntityContext.NotificacionParametroPersona.JoinNotificacion().JoinNotificacionSolicitud().Where(item => pLista.Contains(item.NotificacionSolicitud.SolicitudID)).Select(item => item.NotificacionParametroPersona).ToList();

                //NotificacionAlertaPersona
                notificacionDW.ListaNotificacionAlertaPersona = mEntityContext.NotificacionAlertaPersona.JoinNotificacion().JoinNotificacionSolicitud().Where(item => pLista.Contains(item.NotificacionSolicitud.SolicitudID)).Select(item => item.NotificacionAlertaPersona).ToList();

                //NotificacionCorreoPersona
                notificacionDW.ListaNotificacionCorreoPersona = mEntityContext.NotificacionCorreoPersona.JoinNotificacion().JoinNotificacionSolicitud().Where(item => pLista.Contains(item.NotificacionSolicitud.SolicitudID)).Select(item => item.NotificacionCorreoPersona).ToList();
            }

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene todas las notificaciones relacionadas con una lista de suscripciones pasada como parámetro
        /// </summary>
        /// <param name="pListaSuscripciones"></param>
        /// <returns>Dataset de notificaciones</returns>
        public DataWrapperNotificacion ObtenerNotificacionesDeSuscripciones(List<Guid> pListaSuscripciones)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            if (pListaSuscripciones.Count > 0)
            {
                //Notificacion
                notificacionDW.ListaNotificacion = mEntityContext.Notificacion.JoinNotificacionSuscripcion().Where(item => pListaSuscripciones.Contains(item.NotificacionSuscripcion.SuscripcionID)).Select(item => item.Notificacion).ToList();

                //NotificacionSuscripcion
                notificacionDW.ListaNotificacionSuscripcion = mEntityContext.NotificacionSuscripcion.Where(item => pListaSuscripciones.Contains(item.SuscripcionID)).ToList();

                //NotificacionParametro
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.JoinNotificacion().JoinNotificacionSuscripcion().Where(item => pListaSuscripciones.Contains(item.NotificacionSuscripcion.SuscripcionID)).Select(item => item.NotificacionParametro).ToList();

                //NotificacionParametroPersona
                notificacionDW.ListaNotificacionParametroPersona = mEntityContext.NotificacionParametroPersona.JoinNotificacion().JoinNotificacionSuscripcion().Where(item => pListaSuscripciones.Contains(item.NotificacionSuscripcion.SuscripcionID)).Select(item => item.NotificacionParametroPersona).ToList();

                //NotificacionAlertaPersona
                notificacionDW.ListaNotificacionAlertaPersona = mEntityContext.NotificacionAlertaPersona.JoinNotificacion().JoinNotificacionSuscripcion().Where(item => pListaSuscripciones.Contains(item.NotificacionSuscripcion.SuscripcionID)).Select(item => item.NotificacionAlertaPersona).ToList();

                //NotificacionCorreoPersona
                notificacionDW.ListaNotificacionCorreoPersona = mEntityContext.NotificacionCorreoPersona.JoinNotificacion().JoinNotificacionSuscripcion().Where(item => pListaSuscripciones.Contains(item.NotificacionSuscripcion.SuscripcionID)).Select(item => item.NotificacionCorreoPersona).ToList();
            }

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene todas las notificaciones de las suscripciones "NotificacionSuscripcion" 
        /// de todas las identidades de la persona pasada por parámetro
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <returns>Dataset de notificaciones</returns>
        public DataWrapperNotificacion ObtenerNotificacionesDeSuscripcionesDeUsuario(Guid pPersonaID)
        {
            DataWrapperNotificacion notDW = new DataWrapperNotificacion();

            //NotificacionSuscripcion
            notDW.ListaNotificacionSuscripcion = mEntityContext.NotificacionSuscripcion.JoinSuscripcion().JoinIdentidad().JoinPerfil().Where(item => item.Perfil.PersonaID.Value.Equals(pPersonaID)).Select(item => item.NotificacionSuscripcion).ToList();

            return notDW;
        }

        /// <summary>
        /// Actualiza la base de datos de Entity
        /// </summary>
        public void ActualizarNotificacionEntity()
        {
            base.ActualizarBaseDeDatosEntityContext();
        }


        /// <summary>
        /// Obtiene las notificaciones pendientes de enviar
        /// </summary>
        /// <param name="pCargarFallidas">Indica si es necesario cargar las notificaciones fallidas</param>
        /// <returns>Dataset de notificaciones cargado con las tablas 
        /// Notificacion,NotificacionCorreoPersona,NotificacionParametro,NotificacionParametrosPersona</returns>
        public DataWrapperNotificacion ObtenerEnvioNotificaciones(bool pCargarFallidas)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            //Notificacion
            var queryNotificaciones = mEntityContext.NotificacionCorreoPersona.JoinNotificacion().Where(item => DateTime.Now >= item.Notificacion.FechaNotificacion && (item.Notificacion.FechaFin.Value > DateTime.Now || !item.Notificacion.FechaFin.HasValue) && item.NotificacionCorreoPersona.EstadoEnvio.Value.Equals((short)EstadoEnvio.Pendiente) && !item.NotificacionCorreoPersona.EnviadoRabbit);

            if (pCargarFallidas)
            {
                queryNotificaciones = queryNotificaciones.Where(item => item.NotificacionCorreoPersona.EstadoEnvio.Value.Equals((short)EstadoEnvio.Error));
            }

            notificacionDW.ListaNotificacion = queryNotificaciones.OrderBy(item => item.Notificacion.FechaFin.Value).Select(item => item.Notificacion).Take(10).ToList();

            if (notificacionDW.ListaNotificacion.Count > 0)
            {
                var listaNotificacionID = notificacionDW.ListaNotificacion.Select(item => item.NotificacionID);

                //NotificacionCorreoPersona
                notificacionDW.ListaNotificacionCorreoPersona = mEntityContext.NotificacionCorreoPersona.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //NotificacionAlertaPersona
                notificacionDW.ListaNotificacionAlertaPersona = mEntityContext.NotificacionAlertaPersona.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //NotificacionParametros
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //NotificacionParametrosPersona
                notificacionDW.ListaNotificacionParametroPersona = mEntityContext.NotificacionParametroPersona.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //NotificacionSolicitud
                notificacionDW.ListaNotificacionSolicitud = mEntityContext.NotificacionSolicitud.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //NotificacionSuscripcion
                notificacionDW.ListaNotificacionSuscripcion = mEntityContext.NotificacionSuscripcion.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //Invitacion
                notificacionDW.ListaInvitacion = mEntityContext.Invitacion.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();
            }

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene las notificaciones pendientes de enviar
        /// </summary>
        /// <param name="pCargarFallidas">Indica si es necesario cargar las notificaciones fallidas</param>
        /// <returns>Dataset de notificaciones cargado con las tablas 
        /// Notificacion,NotificacionCorreoPersona,NotificacionParametro,NotificacionParametrosPersona</returns>
        public DataWrapperNotificacion ObtenerEnvioNotificacionesRabbitMQ(Guid pNotificacionID)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            //Notificacion
            notificacionDW.ListaNotificacion = mEntityContext.Notificacion.Where(item => item.NotificacionID.Equals(pNotificacionID)).ToList();

            if (notificacionDW.ListaNotificacion.Count > 0)
            {
                //NotificacionCorreoPersona
                notificacionDW.ListaNotificacionCorreoPersona = mEntityContext.NotificacionCorreoPersona.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();

                //NotificacionAlertaPersona
                notificacionDW.ListaNotificacionAlertaPersona = mEntityContext.NotificacionAlertaPersona.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();

                //NotificacionParametros
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();

                //NotificacionParametrosPersona
                notificacionDW.ListaNotificacionParametroPersona = mEntityContext.NotificacionParametroPersona.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();

                //NotificacionSolicitud
                notificacionDW.ListaNotificacionSolicitud = mEntityContext.NotificacionSolicitud.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();

                //NotificacionSuscripcion
                notificacionDW.ListaNotificacionSuscripcion = mEntityContext.NotificacionSuscripcion.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();

                //Invitacion
                notificacionDW.ListaInvitacion = mEntityContext.Invitacion.Where(item => pNotificacionID.Equals(item.NotificacionID)).ToList();
            }

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene las invitaciones pendientes de MyGnoss para una persona
        /// </summary>
        /// <param name="pPerfilPersonaID">Identificador del perfil de la persona para la que se obtienen las invitaciones</param>
        /// <param name="pPersonaID">Identificador de persona para la que se obtienen las invitaciones</param>
        /// <param name="pOrganizacionID">Identificador de organización en la que se encuentra la persona, o NULL si está en modo personal</param>
        /// <param name="pEsAdmin">TRUE si es administrador de la organización en la que se encuentra</param>
        /// <returns>Dataset de notificaciones cargado con las invitaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionesPendientesDeMyGnoss(Guid? pPerfilPersonaID, Guid? pPersonaID, Guid? pOrganizacionID, bool pEsAdmin)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            //Invitacion
            var query = mEntityContext.Invitacion.JoinIdentidad().GroupJoin(mEntityContext.Organizacion, item => item.Invitacion.ElementoVinculadoID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new
            {
                Organizacion = organizacion,
                Identidad = item.Identidad,
                Invitacion = item.Invitacion
            }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new
            {
                Organizacion = y,
                Identidad = x.Identidad,
                Invitacion = x.Invitacion
            }).Join(mEntityContext.Perfil, item => item.Identidad.PerfilID, perfil => perfil.PerfilID, (item, perfil) => new
            {
                Organizacion = item.Organizacion,
                Identidad = item.Identidad,
                Invitacion = item.Invitacion,
                Perfil = perfil
            }
            ).Where(item => item.Invitacion.Estado.Equals((short)EstadoInvitacion.Pendiente));

            if (pOrganizacionID.HasValue)
            {
                if (pEsAdmin)
                {
                    query = query.Where(item => item.Perfil.OrganizacionID.Equals(pOrganizacionID) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Organizacion));
                }
                else
                {
                    query = query.Where(item => item.Perfil.PersonaID.Equals(pPersonaID) && item.Perfil.OrganizacionID.Value.Equals(pOrganizacionID));
                }
            }
            else
            {
                query = query.Where(item => item.Perfil.PerfilID.Equals(pPerfilPersonaID) && item.Perfil.PersonaID.Value.Equals(pPersonaID) && item.Identidad.Tipo.Equals((short)TiposIdentidad.Personal));
            }

            if (notificacionDW.ListaInvitacion.Count > 0)
            {
                var listaNotificacionID = notificacionDW.ListaInvitacion.Select(item => item.NotificacionID);

                //Notificacion
                notificacionDW.ListaNotificacion = mEntityContext.Notificacion.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();

                //NotificacionParametro
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.Where(item => listaNotificacionID.Contains(item.NotificacionID)).ToList();
            }

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene el número de invitaciones pendientes de MyGnoss para una persona
        /// </summary>
        /// <param name="pPersonaID">Identificador de persona para la que se obtienen las invitaciones, o NULL si es una organización</param>
        /// <param name="pOrganizacionID">Identificador de organización en la que se encuentra la persona, o NULL si está en modo personal</param>
        /// <param name="pEsAdmin">TRUE si es administrador de la organización en la que se encuentra</param>
        /// <returns>Número de invitaciones pendientes de ese perfil(persona,organización)</returns>
        public int ObtenerNumeroInvitacionesPendientesDeMyGnoss(Guid? pPersonaID, Guid? pOrganizacionID, bool pEsAdmin)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            var temp = mEntityContext.PerfilOrganizacion.JoinIdentidad().Where(item => item.PerfilOrganizacion.OrganizacionID.Equals(pOrganizacionID.Value)).Select(item => new
            {
                IdentidadID = item.Identidad.IdentidadID,
                OrganizacionID = item.PerfilOrganizacion.OrganizacionID
            });

            if (pOrganizacionID.HasValue)
            {
                if (pEsAdmin)
                {
                    //Invitacion
                    notificacionDW.ListaInvitacion = mEntityContext.Invitacion.JoinIdentidad().GroupJoin(mEntityContext.Organizacion, item => item.Invitacion.ElementoVinculadoID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new
                    {
                        Organizacion = organizacion,
                        Identidad = item.Identidad,
                        Invitacion = item.Invitacion
                    }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new
                    {
                        Organizacion = y,
                        Identidad = x.Identidad,
                        Invitacion = x.Invitacion
                    }).Join(temp, item => item.Invitacion.IdentidadDestinoID, temporal => temporal.IdentidadID, (item, temporal) => new
                    {
                        Organizacion = item.Organizacion,
                        Identidad = item.Identidad,
                        IdentidadIDAdmin = temporal.IdentidadID,
                        OrganizacionIDAdmin = temporal.OrganizacionID,
                        Invitacion = item.Invitacion
                    }).Where(item => item.Invitacion.Estado.Equals((short)EstadoInvitacion.Pendiente) && item.OrganizacionIDAdmin.Equals(pOrganizacionID.Value)).Select(item => item.Invitacion).ToList();
                }
                else
                {
                    //Invitacion
                    var select = mEntityContext.Invitacion.JoinIdentidad().GroupJoin(mEntityContext.Organizacion, item => item.Invitacion.ElementoVinculadoID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new
                    {
                        Organizacion = organizacion,
                        Identidad = item.Identidad,
                        Invitacion = item.Invitacion
                    }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new
                    {
                        Organizacion = y,
                        Identidad = x.Identidad,
                        Invitacion = x.Invitacion
                    }).Join(mEntityContext.PerfilPersonaOrg, item => item.Identidad.PerfilID, perfilPersonaOrg => perfilPersonaOrg.PerfilID, (item, perfilPersonaOrg) => new
                    {
                        Identidad = item.Identidad,
                        PerfilPersonaOrg = perfilPersonaOrg,
                        Invitacion = item.Invitacion,
                        Organizacion = item.Organizacion
                    }).Where(item => item.Invitacion.Estado.Equals((short)EstadoInvitacion.Pendiente) && item.PerfilPersonaOrg.OrganizacionID.Equals(pOrganizacionID.Value));
                    if (pPersonaID.HasValue)
                    {
                        select = select.Where(item => item.PerfilPersonaOrg.PersonaID.Equals(pPersonaID.Value));
                    }
                    notificacionDW.ListaInvitacion = select.Select(item => item.Invitacion).ToList();
                }
            }
            else
            {
                //Invitacion
                var selectSinOrganizacion = mEntityContext.Invitacion.JoinIdentidad().GroupJoin(mEntityContext.Organizacion, item => item.Invitacion.ElementoVinculadoID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new
                {
                    Organizacion = organizacion,
                    Identidad = item.Identidad,
                    Invitacion = item.Invitacion
                }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new
                {
                    Organizacion = y,
                    Identidad = x.Identidad,
                    Invitacion = x.Invitacion
                }).Join(mEntityContext.PerfilPersona, item => item.Identidad.PerfilID, perfilPersona => perfilPersona.PerfilID, (item, perfilPersona) => new
                {
                    Organizacion = item.Organizacion,
                    Identidad = item.Identidad,
                    Invitacion = item.Invitacion,
                    PerfilPersona = perfilPersona
                }).Where(item => item.Invitacion.Estado.Equals((short)EstadoInvitacion.Pendiente));
                if (pPersonaID.HasValue)
                {
                    selectSinOrganizacion = selectSinOrganizacion.Where(item => item.PerfilPersona.PersonaID.Equals(pPersonaID.Value));
                }

                notificacionDW.ListaInvitacion = selectSinOrganizacion.Select(item => item.Invitacion).ToList();
            }

            return notificacionDW.ListaInvitacion.Count;
        }

        /// <summary>
        /// Obtiene invitaciones por id.
        /// </summary>
        /// <param name="pInvitacionesID">Claves de las invitaciones que queremos cargar</param>
        /// <returns>DataSet de notificaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionesPorID(List<Guid> pInvitacionesID)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            if (pInvitacionesID.Count > 0)
            {
                //Invitacion 
                notificacionDW.ListaInvitacion = mEntityContext.Invitacion.Where(item => pInvitacionesID.Contains(item.InvitacionID)).ToList();

                //Notificacion
                notificacionDW.ListaNotificacion = mEntityContext.Notificacion.JoinInvitacion().Where(item => pInvitacionesID.Contains(item.Invitacion.InvitacionID)).Select(item => item.Notificacion).ToList();

                //NotificacionParametro
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.JoinNotificacion().JoinInvitacion().Where(item => pInvitacionesID.Contains(item.Invitacion.InvitacionID)).Select(item => item.NotificacionParametro).ToList();
            }

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene invitaciones por id.
        /// </summary>
        /// <param name="pInvitacionesID">Claves de las invitaciones que queremos cargar</param>
        /// <returns>DataSet de notificaciones</returns>
        public DataWrapperNotificacion ObtenerInvitacionesPorIDConNombreCorto(List<Guid> pInvitacionesID)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            if (pInvitacionesID.Count > 0)
            {
                //Invitacion 
                notificacionDW.ListaInvitacion = mEntityContext.Invitacion.JoinIdentidad().GroupJoin(mEntityContext.Organizacion, item => item.Invitacion.ElementoVinculadoID, organizacion => organizacion.OrganizacionID, (item, organizacion) => new
                {
                    Organizacion = organizacion,
                    Identidad = item.Identidad,
                    Invitacion = item.Invitacion
                }).SelectMany(item => item.Organizacion.DefaultIfEmpty(), (x, y) => new
                {
                    Organizacion = y,
                    Identidad = x.Identidad,
                    Invitacion = x.Invitacion
                }).Where(item => pInvitacionesID.Contains(item.Invitacion.InvitacionID)).Select(item => item.Invitacion).ToList();

                //Notificacion
                notificacionDW.ListaNotificacion = mEntityContext.Notificacion.JoinInvitacion().Where(item => pInvitacionesID.Contains(item.Invitacion.InvitacionID)).Select(item => item.Notificacion).ToList();

                //NotificacionParametro
                notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.JoinNotificacion().JoinInvitacion().Where(item => pInvitacionesID.Contains(item.Invitacion.InvitacionID)).Select(item => item.NotificacionParametro).ToList();
            }

            return notificacionDW;
        }


        /// <summary>
        /// Comprueba si la persona indicada en la organización indicada tiene generado el boletín de suscripciones a partir de una fecha
        /// </summary>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <param name="pOrganizacionID">Identificador de la organización de la persona, o NULL si es personal</param>
        /// <param name="pFecha">Fecha a partir de la cual se comprueba si tiene boletín</param>
        /// <returns>TRUE si existe boletin posterior a la fecha para la persona indicada</returns>
        public bool TienePerfilBoletinPosteriorAFecha(Guid pPersonaID, Guid? pOrganizacionID, DateTime pFecha)
        {
            if (pOrganizacionID.HasValue)
            {
                return mEntityContext.Notificacion.JoinNotificacionCorreoPersona().Any(item => item.Notificacion.MensajeID.Equals((short)TiposNotificacion.BoletinSuscripcion) && item.Notificacion.FechaNotificacion >= pFecha && item.NotificacionCorreoPersona.PersonaID.Value.Equals(pPersonaID) && item.NotificacionCorreoPersona.OrganizacionPersonaID.Value.Equals(pOrganizacionID.Value));
            }
            else
            {
                return mEntityContext.Notificacion.JoinNotificacionCorreoPersona().Any(item => item.Notificacion.MensajeID.Equals((short)TiposNotificacion.BoletinSuscripcion) && item.Notificacion.FechaNotificacion >= pFecha && item.NotificacionCorreoPersona.PersonaID.Value.Equals(pPersonaID) && !item.NotificacionCorreoPersona.OrganizacionPersonaID.HasValue);
            }
        }

        /// <summary>
        /// Obtiene las notificaciones que todavía no se han anviado a los administradores de un proyecto para que reabran el proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Dataset de notificaciones</returns>
        public DataWrapperNotificacion ObtenerNotificacionesReaperturaDeProyecto(Guid pProyectoID)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            //Notificacion
            notificacionDW.ListaNotificacion = mEntityContext.Notificacion.Where(item => item.ProyectoID.Value.Equals(pProyectoID) && item.MensajeID.Equals((short)TiposNotificacion
                .AvisoReaperturaComunidad)).ToList();

            //NotificacionCorreoPersona
            notificacionDW.ListaNotificacionCorreoPersona = mEntityContext.NotificacionCorreoPersona.JoinNotificacion().Where(item => item.Notificacion.ProyectoID.Value.Equals(pProyectoID) && item.Notificacion.MensajeID.Equals((short)TiposNotificacion.AvisoReaperturaComunidad)).Select(item => item.NotificacionCorreoPersona).ToList();

            //NotificacionParametro
            notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.JoinNotificacion().Where(item => item.Notificacion.ProyectoID.Value.Equals(pProyectoID) && item.Notificacion.MensajeID.Equals((short)TiposNotificacion.AvisoReaperturaComunidad)).Select(item => item.NotificacionParametro).ToList();

            return notificacionDW;
        }

        /// <summary>
        /// Indica si se ha leido la notificacion del mensaje, pMensajeID por la persona, pPersonaID
        /// </summary>
        /// <param name="pPersonaID">Id de la persona</param>
        /// <param name="pMensajeID">Id del mensaje</param>
        /// <returns>TRUE si no se ha leido la notificación, FALSE si se ha leido la notificación</returns>
        public bool TieneNotificacionSinLeer(Guid pPersonaID, int pMensajeID)
        {
            return mEntityContext.NotificacionAlertaPersona.JoinNotificacion().Any(item => !item.NotificacionAlertaPersona.FechaLectura.HasValue && item.Notificacion.MensajeID.Equals((short)pMensajeID) && item.NotificacionAlertaPersona.PersonaID.Equals(pPersonaID));
        }

        /// <summary>
        /// Obtiene todas las notificaciones de envíos masivos pendientes de procesar
        /// </summary>
        /// <returns>Dataset de notificaciones</returns>
        public DataWrapperNotificacion ObtenerNotificacionesEnviosMasivos()
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            //Notificacion
            notificacionDW.ListaNotificacion = mEntityContext.Notificacion.JoinNotificacionEnvioMasivo().Where(item => item.NotificacionEnvioMasivo.EstadoEnvio.Equals((int)EstadoEnvio.Pendiente) && DateTime.Now >= item.Notificacion.FechaNotificacion && (DateTime.Now <= item.Notificacion.FechaFin || !item.Notificacion.FechaFin.HasValue)).Select(item => item.Notificacion).Distinct().ToList();

            //NotificacionEnvioMasivo
            notificacionDW.ListaNotificacionEnvioMasivo = mEntityContext.Notificacion.JoinNotificacionEnvioMasivo().Where(item => item.NotificacionEnvioMasivo.EstadoEnvio.Equals((int)EstadoEnvio.Pendiente) && DateTime.Now >= item.Notificacion.FechaNotificacion && (DateTime.Now <= item.Notificacion.FechaFin || !item.Notificacion.FechaFin.HasValue)).OrderBy(item => item.NotificacionEnvioMasivo.Prioridad).Select(item => item.NotificacionEnvioMasivo).ToList().Distinct().ToList();

            //NotificacionParametros
            notificacionDW.ListaNotificacionParametro = mEntityContext.NotificacionParametro.JoinNotificacion().JoinNotificacionEnvioMasivo().Where(item => item.NotificacionEnvioMasivo.EstadoEnvio.Equals((int)EstadoEnvio.Pendiente) && DateTime.Now >= item.Notificacion.FechaNotificacion && (DateTime.Now <= item.Notificacion.FechaFin || !item.Notificacion.FechaFin.HasValue)).OrderBy(item => item.NotificacionEnvioMasivo.Prioridad).OrderBy(item => item.Notificacion.FechaFin.Value).Select(item => item.NotificacionParametro).ToList();

            //EmailIncorrecto
            notificacionDW.ListaEmailIncorrecto = mEntityContext.EmailIncorrecto.ToList();

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene la cola de procesado de twitter de un consumerKey
        /// </summary>
        /// <param name="pNumElementos"></param>
        /// <returns></returns>
        public DataWrapperNotificacion ObtenerColaTwitter(int pNumElementos)
        {
            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            notificacionDW.ListaColaTwitter = mEntityContext.ColaTwitter.Where(item => item.NumIntentos < 3).OrderBy(item => item.ColaID).Distinct().ToList();

            return notificacionDW;
        }

        /// <summary>
        /// Obtiene la lista de ConsumerKey de la cola de Twitter
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> ObtenerListaConsumerKeyTwitter()
        {
            Dictionary<string, string> listaConsumerKey = new Dictionary<string, string>();

            DataWrapperNotificacion notificacionDW = new DataWrapperNotificacion();

            notificacionDW.ListaColaTwitter = mEntityContext.ColaTwitter.Where(item => item.NumIntentos < 3).ToList();

            foreach (ColaTwitter filaColaTwitter in notificacionDW.ListaColaTwitter)
            {
                listaConsumerKey.Add(filaColaTwitter.ConsumerKey, filaColaTwitter.ConsumerSecret);
            }

            return listaConsumerKey;
        }

        /// <summary>
        /// Actualiza la notificación ID pasada como parámetro con el estado y la fecha de envio.
        /// </summary>
        /// <param name="pNotificacionID">ID de la notificación</param>
        /// <param name="pEstadoEnvio">Estado del envio de la notificación</param>
        /// <param name="pFechaEnvio">Fecha en la que se ha enviado la notificación</param>
        public void ActualizarNotificacion(Guid pNotificacionID, short pEstadoEnvio, DateTime pFechaEnvio)
        {
            NotificacionEnvioMasivo notificacion = mEntityContext.NotificacionEnvioMasivo.Where(item => item.NotificacionID.Equals(pNotificacionID)).FirstOrDefault();

            notificacion.EstadoEnvio = pEstadoEnvio;

            ActualizarBaseDeDatosEntityContext();
        }

        public List<ColaCorreoDestinatario> ObtenerColaCorreoDestinatariosPorCorreoID(int pCorreoID)
        {
            return mEntityContextBase.ColaCorreoDestinatario.Where(item => item.CorreoID == pCorreoID).ToList();
        }

        #endregion

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas

            #region Select Generales

            this.sqlSelectNotificacionAlertaPersona = "SELECT " + IBD.CargarGuid("NotificacionAlertaPersona.NotificacionID") + ", " + IBD.CargarGuid("NotificacionAlertaPersona.PersonaID") + ", NotificacionAlertaPersona.FechaLectura FROM NotificacionAlertaPersona";

            this.sqlSelectNotificacionCorreoPersona = "SELECT " + IBD.CargarGuid("NotificacionCorreoPersona.NotificacionID") + ", NotificacionCorreoPersona.EmailEnvio, " + IBD.CargarGuid("NotificacionCorreoPersona.PersonaID") + ", NotificacionCorreoPersona.EstadoEnvio, NotificacionCorreoPersona.FechaEnvio, " + IBD.CargarGuid("NotificacionCorreoPersona.OrganizacionPersonaID") + " FROM NotificacionCorreoPersona";

            this.sqlSelectNotificacion = "SELECT " + IBD.CargarGuid("Notificacion.NotificacionID") + ", Notificacion.FechaFin, Notificacion.MensajeID, Notificacion.FechaNotificacion, " + IBD.CargarGuid("Notificacion.OrganizacionID") + ", " + IBD.CargarGuid("Notificacion.ProyectoID") + ", Notificacion.Idioma FROM Notificacion";

            this.sqlSelectNotificacionSolicitud = "SELECT " + IBD.CargarGuid("NotificacionSolicitud.NotificacionID") + ", " + IBD.CargarGuid("NotificacionSolicitud.SolicitudID") + ", " + IBD.CargarGuid("NotificacionSolicitud.OrganizacionID") + ", " + IBD.CargarGuid("NotificacionSolicitud.ProyectoID") + " FROM NotificacionSolicitud";

            this.sqlSelectNotificacionSuscripcion = "SELECT " + IBD.CargarGuid("NotificacionSuscripcion.NotificacionID") + ", " + IBD.CargarGuid("NotificacionSuscripcion.SuscripcionID") + " FROM NotificacionSuscripcion";

            this.sqlSelectNotificacionDafo = "SELECT " + IBD.CargarGuid("NotificacionDafo.NotificacionID") + ", " + IBD.CargarGuid("NotificacionDafo.DafoID") + ", " + IBD.CargarGuid("NotificacionDafo.OrganizacionID") + ", " + IBD.CargarGuid("NotificacionDafo.ProyectoID") + " FROM NotificacionDafo";

            this.sqlSelectNotificacionParametro = "SELECT " + IBD.CargarGuid("NotificacionParametro.NotificacionID") + ", ParametroID, Valor FROM NotificacionParametro ";

            this.sqlSelectNotificacionParametroPersona = "SELECT " + IBD.CargarGuid("NotificacionParametroPersona.NotificacionID") + ", ParametroID, " + IBD.CargarGuid("NotificacionParametroPersona.PersonaID") + ", Valor FROM NotificacionParametroPersona ";

            this.sqlSelectNotificacionEnvioMasivo = "SELECT " + IBD.CargarGuid("NotificacionEnvioMasivo.NotificacionID") + ", NotificacionEnvioMasivo.Destinatarios, NotificacionEnvioMasivo.Prioridad, NotificacionEnvioMasivo.EstadoEnvio, NotificacionEnvioMasivo.FechaEnvio FROM NotificacionEnvioMasivo";

            this.sqlSelectInvitacion = "SELECT " + IBD.CargarGuid("InvitacionID") + ", TipoInvitacion, Estado, FechaInvitacion, FechaProcesado, " + IBD.CargarGuid("NotificacionID") + ", " + IBD.CargarGuid("IdentidadOrigenID") + ", " + IBD.CargarGuid("IdentidadDestinoID") + ", " + IBD.CargarGuid("ElementoVinculadoID") + " FROM Invitacion";

            this.sqlSelectInvitacionSimple = "SELECT " + IBD.CargarGuid("InvitacionID") + ", TipoInvitacion, Estado, FechaInvitacion, FechaProcesado, " + IBD.CargarGuid("NotificacionID") + ", " + IBD.CargarGuid("IdentidadOrigenID") + ", " + IBD.CargarGuid("IdentidadDestinoID") + ", " + IBD.CargarGuid("ElementoVinculadoID");

            this.sqlSelectEmailIncorrecto = "SELECT Email FROM EmailIncorrecto";

            #endregion

            this.sqlSelectNotificacionesDeDafo = sqlSelectNotificacion + " INNER JOIN NotificacionDafo ON Notificacion.NotificacionID = NotificacionDafo.NotificacionID WHERE NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID");

            this.sqlSelectNotificacionesAlertasPersonaDeDafo = sqlSelectNotificacionAlertaPersona + " INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionAlertaPersona.NotificacionID INNER JOIN NotificacionDafo ON Notificacion.NotificacionID = NotificacionDafo.NotificacionID WHERE NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID");

            this.sqlSelectNotificacionesCorreosPersonaDeDafo = sqlSelectNotificacionCorreoPersona + " INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionCorreoPersona.NotificacionID INNER JOIN NotificacionDafo ON Notificacion.NotificacionID = NotificacionDafo.NotificacionID WHERE NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID");

            this.sqlSelectNotificacionesParametroDeDafo = sqlSelectNotificacionParametro + " INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionParametro.NotificacionID INNER JOIN NotificacionDafo ON Notificacion.NotificacionID = NotificacionDafo.NotificacionID WHERE NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID");

            this.sqlSelectNotificacionDafoDeDafo = sqlSelectNotificacionDafo + " WHERE NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID");


            this.sqlSelectAlertasDePersona = "SELECT DISTINCT Alerta.AlertaID, Alerta.MensajeID, Alerta.Caduca, Alerta.DiasVigencia FROM Mensaje INNER JOIN Alerta ON Mensaje.MensajeID = Alerta.MensajeID INNER JOIN Notificacion ON Notificacion.MensajeID = Mensaje.MensajeID INNER JOIN NotificacionAlertaPersona ON Notificacion.NotificacionID = NotificacionAlertaPersona.NotificacionID AND NotificacionAlertaPersona.AlertaID = Alerta.AlertaID WHERE NotificacionAlertaPersona.PersonaID = " + IBD.GuidParamValor("PersonaID") + " AND NotificacionAlertaPersona.FechaLectura is null AND Notificacion.FechaNotificacion <= " + IBD.CapturarFechaSinHora() + " AND (Notificacion.FechaFin is null OR Notificacion.FechaFin >= " + IBD.CapturarFechaSinHora() + ")";

            this.sqlSelectNotificacionesDePersona = sqlSelectNotificacion + " INNER JOIN Mensaje ON Mensaje.MensajeID = Notificacion.MensajeID INNER JOIN Alerta ON Notificacion.MensajeID = Alerta.MensajeID INNER JOIN NotificacionAlertaPersona ON Notificacion.NotificacionID = NotificacionAlertaPersona.NotificacionID AND NotificacionAlertaPersona.AlertaID = Alerta.AlertaID WHERE NotificacionAlertaPersona.PersonaID = " + IBD.GuidParamValor("PersonaID") + " AND Notificacion.OrganizacionID = " + IBD.GuidParamValor("OrganizacionID") + " AND Notificacion.ProyectoID = " + IBD.GuidParamValor("ProyectoID") + " AND NotificacionAlertaPersona.FechaLectura is null AND Notificacion.FechaNotificacion <= " + IBD.CapturarFechaSinHora() + " AND (Notificacion.FechaFin is null OR Notificacion.FechaFin >= " + IBD.CapturarFechaSinHora() + ")";

            this.sqlSelectNotificacionesAlertaPersonaDePersona = sqlSelectNotificacionAlertaPersona + " INNER JOIN Alerta ON NotificacionAlertaPersona.AlertaID = Alerta.AlertaID INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionAlertaPersona.NotificacionID INNER JOIN Mensaje ON Mensaje.MensajeID = Alerta.MensajeID WHERE NotificacionAlertaPersona.PersonaID = " + IBD.GuidParamValor("PersonaID") + " AND NotificacionAlertaPersona.FechaLectura is null AND Notificacion.FechaNotificacion <= " + IBD.CapturarFechaSinHora() + " AND (Notificacion.FechaFin is null OR Notificacion.FechaFin >= " + IBD.CapturarFechaSinHora() + ")";

            this.sqlSelectNotificacionesParametroDePersona = sqlSelectNotificacionParametro + " INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionParametro.NotificacionID INNER JOIN Mensaje ON Notificacion.MensajeID = Mensaje.MensajeID INNER JOIN Alerta ON Mensaje.MensajeID = Alerta.MensajeID INNER JOIN NotificacionAlertaPersona ON NotificacionAlertaPersona.AlertaID = Alerta.AlertaID WHERE NotificacionAlertaPersona.PersonaID = " + IBD.GuidParamValor("PersonaID") + " AND NotificacionAlertaPersona.FechaLectura is null AND Notificacion.FechaNotificacion <= " + IBD.CapturarFechaSinHora() + " AND (Notificacion.FechaFin is null OR Notificacion.FechaFin >= " + IBD.CapturarFechaSinHora() + ")";

            this.sqlUpdateAlertasCaducasDePersonaALeidas = "UPDATE NotificacionAlertaPersona SET FechaLectura = " + IBD.CapturarFechaSinHora() + " WHERE PersonaID = " + IBD.GuidParamValor("personaID") + "  AND FechaLectura is null AND NotificacionID IN (SELECT Notificacion.NotificacionID FROM Notificacion INNER JOIN Mensaje ON Notificacion.MensajeID = Mensaje.MensajeID INNER JOIN Alerta ON Alerta.MensajeID = Mensaje.MensajeID WHERE Notificacion.FechaNotificacion < " + IBD.CapturarFechaSinHora() + " AND Alerta.Caduca = 1)";

            this.sqlUpdateAlertasDafoPersonaDePersonaALeida = "UPDATE NotificacionAlertaPersona SET FechaLectura = " + IBD.CapturarFechaSinHora() + " WHERE PersonaID = " + IBD.GuidParamValor("personaID") + " AND NotificacionID IN (SELECT Notificacion.NotificacionID FROM Notificacion INNER JOIN NotificacionDafo ON Notificacion.NotificacionID = NotificacionDafo.NotificacionID WHERE FechaNotificacion < " + IBD.CapturarFechaSinHora() + " AND NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID") + " AND Notificacion.MensajeID = " + IBD.ToParam("tipoNotificacion") + ")";

            this.sqlUpdateAlertasDafoFactorNuevoANoLeidas = "UPDATE NotificacionAlertaPersona SET FechaLectura = null WHERE NOT(PersonaID = " + IBD.GuidParamValor("personaID") + ") AND NotificacionID IN (SELECT Notificacion.NotificacionID FROM Notificacion INNER JOIN NotificacionDafo ON Notificacion.NotificacionID = NotificacionDafo.NotificacionID WHERE FechaNotificacion < " + IBD.CapturarFechaSinHora() + " AND NotificacionDafo.DafoID = " + IBD.GuidParamValor("dafoID") + " AND MensajeID = " + IBD.ToParam("tipoNotificacion") + ")";

            this.sqlSelectNotificacionesReaperturaDeProyecto = sqlSelectNotificacion + " WHERE Notificacion.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Notificacion.MensajeID = " + (short)TiposNotificacion.AvisoReaperturaComunidad;

            this.sqlSelectNotificacionesCorreosPersonaReaperturaDeProyecto = sqlSelectNotificacionCorreoPersona + " INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionCorreoPersona.NotificacionID  WHERE Notificacion.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Notificacion.MensajeID = " + (short)TiposNotificacion.AvisoReaperturaComunidad;

            this.sqlSelectNotificacionParametroReaperturaDeProyecto = sqlSelectNotificacionParametro + " INNER JOIN Notificacion ON Notificacion.NotificacionID = NotificacionParametro.NotificacionID  WHERE Notificacion.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Notificacion.MensajeID = " + (short)TiposNotificacion.AvisoReaperturaComunidad;

            this.sqlSelectColaTwitter = "SELECT ColaTwitter.ColaID, ColaTwitter.PerfilID, ColaTwitter.TokenTwitter, ColaTwitter.TokenSecretoTwitter, ColaTwitter.Mensaje, ColaTwitter.Enlace, ColaTwitter.ConsumerKey, ColaTwitter.ConsumerSecret, ColaTwitter.NumIntentos FROM ColaTwitter";

            #endregion

            #region DataAdapter

            #region NotificacionAlertaPersona

            this.sqlNotificacionAlertaPersonaInsert = IBD.ReplaceParam("INSERT INTO NotificacionAlertaPersona (NotificacionID, PersonaID, FechaLectura) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", " + IBD.GuidParamColumnaTabla("PersonaID") + ", @FechaLectura)");

            this.sqlNotificacionAlertaPersonaDelete = IBD.ReplaceParam("DELETE FROM NotificacionAlertaPersona WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (FechaLectura = @O_FechaLectura OR @O_FechaLectura IS NULL AND FechaLectura IS NULL)");

            this.sqlNotificacionAlertaPersonaModify = IBD.ReplaceParam("UPDATE NotificacionAlertaPersona SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", FechaLectura = @FechaLectura WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND (FechaLectura = @O_FechaLectura OR @O_FechaLectura IS NULL AND FechaLectura IS NULL)");

            #endregion

            #region NotificacionCorreoPersona

            this.sqlNotificacionCorreoPersonaInsert = IBD.ReplaceParam("INSERT INTO NotificacionCorreoPersona (NotificacionID, EmailEnvio, PersonaID, EstadoEnvio, FechaEnvio, OrganizacionPersonaID) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", @EmailEnvio, " + IBD.GuidParamColumnaTabla("PersonaID") + ", @EstadoEnvio, @FechaEnvio, " + IBD.GuidParamColumnaTabla("OrganizacionPersonaID") + ")");

            this.sqlNotificacionCorreoPersonaDelete = IBD.ReplaceParam("DELETE FROM NotificacionCorreoPersona WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (EmailEnvio = @O_EmailEnvio) AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + " OR " + IBD.GuidParamColumnaTabla("O_PersonaID") + " IS NULL AND PersonaID IS NULL) AND (EstadoEnvio = @O_EstadoEnvio OR @O_EstadoEnvio IS NULL AND EstadoEnvio IS NULL) AND (FechaEnvio = @O_FechaEnvio OR @O_FechaEnvio IS NULL AND FechaEnvio IS NULL) AND (OrganizacionPersonaID = " + IBD.GuidParamColumnaTabla("O_OrganizacionPersonaID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionPersonaID") + " IS NULL AND OrganizacionPersonaID IS NULL)");

            this.sqlNotificacionCorreoPersonaModify = IBD.ReplaceParam("UPDATE NotificacionCorreoPersona SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", EmailEnvio = @EmailEnvio, PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", EstadoEnvio = @EstadoEnvio, FechaEnvio = @FechaEnvio, OrganizacionPersonaID = " + IBD.GuidParamColumnaTabla("OrganizacionPersonaID") + " WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (EmailEnvio = @O_EmailEnvio) AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + " OR " + IBD.GuidParamColumnaTabla("O_PersonaID") + " IS NULL AND PersonaID IS NULL) AND (EstadoEnvio = @O_EstadoEnvio OR @O_EstadoEnvio IS NULL AND EstadoEnvio IS NULL) AND (FechaEnvio = @O_FechaEnvio OR @O_FechaEnvio IS NULL AND FechaEnvio IS NULL) AND (OrganizacionPersonaID = " + IBD.GuidParamColumnaTabla("O_OrganizacionPersonaID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionPersonaID") + " IS NULL AND OrganizacionPersonaID IS NULL)");

            #endregion

            #region Notificacion

            this.sqlNotificacionInsert = IBD.ReplaceParam("INSERT INTO Notificacion (NotificacionID, FechaFin, MensajeID, FechaNotificacion, OrganizacionID, ProyectoID, Idioma) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", @FechaFin, @MensajeID, @FechaNotificacion, " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Idioma)");

            this.sqlNotificacionDelete = IBD.ReplaceParam("DELETE FROM Notificacion WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (FechaFin = @O_FechaFin OR @O_FechaFin IS NULL AND FechaFin IS NULL) AND (MensajeID = @O_MensajeID) AND (FechaNotificacion = @O_FechaNotificacion) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " IS NULL AND OrganizacionID IS NULL) AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " OR " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " IS NULL AND ProyectoID IS NULL) AND (Idioma = @O_Idioma OR @O_Idioma IS NULL AND Idioma IS NULL)");

            this.sqlNotificacionModify = IBD.ReplaceParam("UPDATE Notificacion SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", FechaFin = @FechaFin, MensajeID = @MensajeID, FechaNotificacion = @FechaNotificacion, OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Idioma = @Idioma WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (FechaFin = @O_FechaFin OR @O_FechaFin IS NULL AND FechaFin IS NULL) AND (MensajeID = @O_MensajeID) AND (FechaNotificacion = @O_FechaNotificacion) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " OR " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + " IS NULL AND OrganizacionID IS NULL) AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " OR " + IBD.GuidParamColumnaTabla("O_ProyectoID") + " IS NULL AND ProyectoID IS NULL) AND (Idioma = @O_Idioma OR @O_Idioma IS NULL AND Idioma IS NULL)");

            #endregion

            #region NotificacionSolicitud

            this.sqlNotificacionSolicitudInsert = IBD.ReplaceParam("INSERT INTO NotificacionSolicitud (NotificacionID, SolicitudID, OrganizacionID, ProyectoID) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", " + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ")");

            this.sqlNotificacionSolicitudDelete = IBD.ReplaceParam("DELETE FROM NotificacionSolicitud WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            this.sqlNotificacionSolicitudModify = IBD.ReplaceParam("UPDATE NotificacionSolicitud SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region NotificacionSuscripcion

            this.sqlNotificacionSuscripcionInsert = IBD.ReplaceParam("INSERT INTO NotificacionSuscripcion (NotificacionID, SuscripcionID) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", " + IBD.GuidParamColumnaTabla("SuscripcionID") + ")");

            this.sqlNotificacionSuscripcionDelete = IBD.ReplaceParam("DELETE FROM NotificacionSuscripcion WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ")");

            this.sqlNotificacionSuscripcionModify = IBD.ReplaceParam("UPDATE NotificacionSuscripcion SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + " WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ")");

            #endregion

            #region NotificacionDafo

            this.sqlNotificacionDafoInsert = IBD.ReplaceParam("INSERT INTO NotificacionDafo (NotificacionID, DafoID, OrganizacionID, ProyectoID) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", " + IBD.GuidParamColumnaTabla("DafoID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ")");

            this.sqlNotificacionDafoDelete = IBD.ReplaceParam("DELETE FROM NotificacionDafo WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (DafoID = " + IBD.GuidParamColumnaTabla("O_DafoID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            this.sqlNotificacionDafoModify = IBD.ReplaceParam("UPDATE NotificacionDafo SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", DafoID = " + IBD.GuidParamColumnaTabla("DafoID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (DafoID = " + IBD.GuidParamColumnaTabla("O_DafoID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region NotificacionParametro

            this.sqlNotificacionParametroInsert = IBD.ReplaceParam("INSERT INTO NotificacionParametro (NotificacionID, ParametroID, Valor) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", @ParametroID, @Valor)");

            this.sqlNotificacionParametroDelete = IBD.ReplaceParam("DELETE FROM NotificacionParametro WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (ParametroID = @O_ParametroID) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Valor", false));

            this.sqlNotificacionParametroModify = IBD.ReplaceParam("UPDATE NotificacionParametro SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", ParametroID = @ParametroID, Valor = @Valor WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (ParametroID = @O_ParametroID) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Valor", false));

            #endregion

            #region NotificacionParametroPersona

            this.sqlNotificacionParametroPersonaInsert = IBD.ReplaceParam("INSERT INTO NotificacionParametroPersona (NotificacionID, ParametroID, PersonaID, Valor) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", @ParametroID, " + IBD.GuidParamColumnaTabla("PersonaID") + ", @Valor)");

            this.sqlNotificacionParametroPersonaDelete = IBD.ReplaceParam("DELETE FROM NotificacionParametroPersona WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (ParametroID = @O_ParametroID) AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Valor", false));

            this.sqlNotificacionParametroPersonaModify = IBD.ReplaceParam("UPDATE NotificacionParametroPersona SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", ParametroID = @ParametroID, PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", Valor = @Valor WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (ParametroID = @O_ParametroID) AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ") AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Valor", false));

            #endregion

            #region NotificacionEnvioMasivo

            this.sqlNotificacionEnvioMasivoInsert = IBD.ReplaceParam("INSERT INTO NotificacionEnvioMasivo (NotificacionID, Destinatarios, Prioridad, EstadoEnvio, FechaEnvio) VALUES (" + IBD.GuidParamColumnaTabla("NotificacionID") + ", @Destinatarios, @Prioridad, @EstadoEnvio, @FechaEnvio)");

            this.sqlNotificacionEnvioMasivoDelete = IBD.ReplaceParam("DELETE FROM NotificacionEnvioMasivo WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (Destinatarios = @O_Destinatarios) AND (Prioridad = @O_Prioridad) AND (EstadoEnvio = @O_EstadoEnvio) AND (FechaEnvio = @O_FechaEnvio OR @O_FechaEnvio IS NULL AND FechaEnvio IS NULL)");

            this.sqlNotificacionEnvioMasivoModify = IBD.ReplaceParam("UPDATE NotificacionEnvioMasivo SET NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", Destinatarios = @Destinatarios, Prioridad = @Prioridad, EstadoEnvio = @EstadoEnvio, FechaEnvio = @FechaEnvio WHERE (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (Destinatarios = @O_Destinatarios) AND (Prioridad = @O_Prioridad) AND (EstadoEnvio = @O_EstadoEnvio) AND (FechaEnvio = @O_FechaEnvio OR @O_FechaEnvio IS NULL AND FechaEnvio IS NULL)");

            #endregion

            #region Invitacion

            this.sqlInvitacionInsert = IBD.ReplaceParam("INSERT INTO Invitacion (InvitacionID, TipoInvitacion, Estado, FechaInvitacion, FechaProcesado, NotificacionID, IdentidadOrigenID, IdentidadDestinoID, ElementoVinculadoID) VALUES (" + IBD.GuidParamColumnaTabla("InvitacionID") + ", @TipoInvitacion, @Estado, @FechaInvitacion, @FechaProcesado, " + IBD.GuidParamColumnaTabla("NotificacionID") + ", " + IBD.GuidParamColumnaTabla("IdentidadOrigenID") + ", " + IBD.GuidParamColumnaTabla("IdentidadDestinoID") + ", " + IBD.GuidParamColumnaTabla("ElementoVinculadoID") + ")");

            this.sqlInvitacionDelete = IBD.ReplaceParam("DELETE FROM Invitacion WHERE (InvitacionID = " + IBD.GuidParamColumnaTabla("O_InvitacionID") + ") AND (TipoInvitacion = @O_TipoInvitacion) AND (Estado = @O_Estado) AND (FechaInvitacion = @O_FechaInvitacion) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL) AND (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (IdentidadOrigenID = " + IBD.GuidParamColumnaTabla("O_IdentidadOrigenID") + ") AND (IdentidadDestinoID = " + IBD.GuidParamColumnaTabla("O_IdentidadDestinoID") + ") AND (ElementoVinculadoID = " + IBD.GuidParamColumnaTabla("O_ElementoVinculadoID") + " OR " + IBD.GuidParamColumnaTabla("O_ElementoVinculadoID") + " IS NULL AND ElementoVinculadoID IS NULL)");

            this.sqlInvitacionModify = IBD.ReplaceParam("UPDATE Invitacion SET InvitacionID = " + IBD.GuidParamColumnaTabla("InvitacionID") + ", TipoInvitacion = @TipoInvitacion, Estado = @Estado, FechaInvitacion = @FechaInvitacion, FechaProcesado = @FechaProcesado, NotificacionID = " + IBD.GuidParamColumnaTabla("NotificacionID") + ", IdentidadOrigenID = " + IBD.GuidParamColumnaTabla("IdentidadOrigenID") + ", IdentidadDestinoID = " + IBD.GuidParamColumnaTabla("IdentidadDestinoID") + ", ElementoVinculadoID = " + IBD.GuidParamColumnaTabla("ElementoVinculadoID") + " WHERE (InvitacionID = " + IBD.GuidParamColumnaTabla("O_InvitacionID") + ") AND (TipoInvitacion = @O_TipoInvitacion) AND (Estado = @O_Estado) AND (FechaInvitacion = @O_FechaInvitacion) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL) AND (NotificacionID = " + IBD.GuidParamColumnaTabla("O_NotificacionID") + ") AND (IdentidadOrigenID = " + IBD.GuidParamColumnaTabla("O_IdentidadOrigenID") + ") AND (IdentidadDestinoID = " + IBD.GuidParamColumnaTabla("O_IdentidadDestinoID") + ") AND (ElementoVinculadoID = " + IBD.GuidParamColumnaTabla("O_ElementoVinculadoID") + " OR " + IBD.GuidParamColumnaTabla("O_ElementoVinculadoID") + " IS NULL AND ElementoVinculadoID IS NULL)");

            #endregion

            #region EmailIncorrecto

            this.sqlEmailIncorrectoInsert = IBD.ReplaceParam("INSERT INTO EmailIncorrecto VALUES (@Email)");

            this.sqlEmailIncorrectoDelete = IBD.ReplaceParam("DELETE FROM EmailIncorrecto WHERE Email = @O_Email");

            this.sqlEmailIncorrectoModify = IBD.ReplaceParam("UPDATE EmailIncorrecto SET Email = @Email WHERE Email = @O_Email");

            #endregion

            #region ColaTwitter
            this.sqlColaTwitterInsert = IBD.ReplaceParam("INSERT INTO ColaTwitter (PerfilID, TokenTwitter, TokenSecretoTwitter, Mensaje, Enlace, ConsumerKey, ConsumerSecret, NumIntentos) VALUES (@PerfilID, @TokenTwitter, @TokenSecretoTwitter, @Mensaje, @Enlace, @ConsumerKey, @ConsumerSecret, @NumIntentos)");
            this.sqlColaTwitterDelete = IBD.ReplaceParam("DELETE FROM ColaTwitter WHERE (ColaId = @Original_ColaId) AND (PerfilID = @Original_PerfilID) AND (TokenTwitter = @Original_TokenTwitter) AND (TokenSecretoTwitter = @Original_TokenSecretoTwitter) AND (Mensaje = @Original_Mensaje) AND (Enlace = @Original_Enlace) AND (ConsumerKey = @Original_ConsumerKey) AND (ConsumerSecret = @Original_ConsumerSecret) AND (NumIntentos = @Original_NumIntentos)");
            this.sqlColaTwitterModify = IBD.ReplaceParam("UPDATE ColaTwitter SET PerfilID = @PerfilID, TokenTwitter = @TokenTwitter, TokenSecretoTwitter = @TokenSecretoTwitter, Mensaje = @Mensaje, Enlace = @Enlace, ConsumerKey = @ConsumerKey, ConsumerSecret = @ConsumerSecret, NumIntentos = @NumIntentos WHERE (ColaId = @Original_ColaId) AND (PerfilID = @Original_PerfilID) AND (TokenTwitter = @Original_TokenTwitter) AND (TokenSecretoTwitter = @Original_TokenSecretoTwitter) AND (Mensaje = @Original_Mensaje) AND (Enlace = @Original_Enlace) AND (ConsumerKey = @Original_ConsumerKey) AND (ConsumerSecret = @Original_ConsumerSecret) AND (NumIntentos = @Original_NumIntentos)");

            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
