using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Peticiones;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Web;
using System.Xml;

namespace Es.Riam.Gnoss.Elementos.Notificacion
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración para las claves de los parámetros de las enumeraciones
    /// </summary>
    public enum ClavesParametro
    {
        #region Nombres identificativos para poder leer mejor los mensajes (Entre los valores 0 y 20000)

        /// <summary>
        /// Nombre de DAFO
        /// </summary>
        NombreDafo = 0,
        /// <summary>
        /// Identificador de DAFO
        /// </summary>
        ClaveDafo = 1,
        /// <summary>
        /// Nombre de proyecto
        /// </summary>
        NombreProyecto = 2,
        /// <summary>
        /// Nombre de usuario en GNOSS
        /// </summary>
        Login = 3,
        /// <summary>
        /// Contraseña de acceso a GNOSS
        /// </summary>
        Password = 4,
        /// <summary>
        /// Nombre de persona
        /// </summary>
        NombrePersona = 5,
        /// <summary>
        /// Identificador de proyecto
        /// </summary>
        ProyectoID = 6,
        /// <summary>
        /// Identificador de organización
        /// </summary>
        OrganizacionID = 7,
        /// <summary>
        /// Notas
        /// </summary>
        Notas = 8,
        /// <summary>
        /// Identificador de usuario
        /// </summary>
        UsuarioID = 9,
        /// <summary>
        /// Identificador de petición
        /// </summary>
        PeticionID = 10,
        /// <summary>
        /// Texto
        /// </summary>
        Texto = 11,
        /// <summary>
        /// Nombre de organización
        /// </summary>
        NombreOrganizacion = 12,
        /// <summary>
        /// Asunto de un mensaje de correo
        /// </summary>
        Asunto = 13,
        /// <summary>
        /// Logotipo de GNOSS
        /// </summary>
        LogoGnoss = 14,
        /// <summary>
        /// Últimas publicaciones
        /// </summary>
        UltimasPublicaciones = 15,
        /// <summary>
        /// Descripción de un proyecto
        /// </summary>
        DescripcionProyecto = 16,
        /// <summary>
        /// Nombre del remitente de un mensaje de correo
        /// </summary>
        NombreRemitente = 17,
        /// <summary>
        /// Identidad del remitente de un mensaje de correo
        /// </summary>
        IdentidadRemitente = 18,
        /// <summary>
        /// Identidad de una persona
        /// </summary>
        IdentidadPersona = 19,
        /// <summary>
        /// Identificador de un mensaje de correo
        /// </summary>
        CorreoID = 20,
        /// <summary>
        /// Url de un enlace
        /// </summary>
        UrlEnlace = 21,
        /// <summary>
        /// Nombre de un documento
        /// </summary>
        NombreDoc = 22,
        /// <summary>
        /// Visibilidad de un DAFO
        /// </summary>
        VisibilidadDafo = 24,
        /// <summary>
        /// Visibilidad de los participantes de un DAFO
        /// </summary>
        VisibilidadParticipantesDafo = 25,
        /// <summary>
        /// Visibilidad de las valoraciones de un DAFO
        /// </summary>
        VisibilidadValoracionesDafo = 26,
        /// <summary>
        /// Otro texto
        /// </summary>
        Texto2 = 27,
        /// <summary>
        /// Día de una fecha (Se introduce por separado día, mes y año xq es diferente en español(día/mes/año) que en inglés(mes/día/año))
        /// </summary>
        Fecha_dia = 28,
        /// <summary>
        /// Mes de una fecha
        /// </summary>
        Fecha_mes = 29,
        /// <summary>
        /// Año de un fecha
        /// </summary>
        Fecha_anio = 30,
        /// <summary>
        /// Alias de una clase
        /// </summary>
        AliasClase = 31,
        /// <summary>
        /// Nombre de asignatura
        /// </summary>
        NombreAsignatura = 32,
        /// <summary>
        /// Newsletter completa
        /// </summary>
        NewsletterCompleta = 33,
        /// <summary>
        /// Enlace de la baja de la newsletter
        /// </summary>
        BajaNewsletter = 34,
        /// <summary>
        /// Nombres de grupos
        /// </summary>
        NombresGrupos = 35,
        /// <summary>
        /// Nombre de evento
        /// </summary>
        NombreEvento = 36,
        /// <summary>
        /// Cuerpo del mensaje del evento
        /// </summary>
        CuerpoMensajeEvento = 37,
        /// <summary>
        /// Token de seguridad de login doble factor
        /// </summary>
        TokenSeguridad = 38,

        Comentario = 39,

        NombreEstadoOrigen = 40,

        NombreEstadoDestino = 41,
        #endregion

        #region Parámetros no parametrizables (Entre los valores 20000 y 21000)

        /// <summary>
        /// Url de la intranet de GNOSS
        /// </summary>
        BaseUrl = 20000

        #endregion
    }

    /// <summary>
    /// Origienes de las notificaciones
    /// </summary>
    public enum OrigenesNotificacion
    {
        /// <summary>
        /// DAFO
        /// </summary>
        Dafo = 0,
        /// <summary>
        /// Solicitud
        /// </summary>
        Solicitud,
        /// <summary>
        /// Suscripción
        /// </summary>
        Suscripcion,
        /// <summary>
        /// Invitación
        /// </summary>
        Invitacion,
        /// <summary>
        /// Cambio de contraseña
        /// </summary>
        CambioContrasenia,
        /// <summary>
        /// Sugerencia
        /// </summary>
        Sugerencia,
        /// <summary>
        /// MyGNOSS
        /// </summary>
        MyGNOSS,
        /// <summary>
        /// Comentario
        /// </summary>
        Comentario,
        /// <summary>
        /// Noticia
        /// </summary>
        Noticia
    }

    #endregion

    /// <summary>
    /// Gestor de notificaciones
    /// </summary>
    [Serializable]
    public class GestionNotificaciones : GestionGnoss, ISerializable
    {
        #region Miembros

        /// <summary>
        /// Array con los idiomas
        /// </summary>
        private string[] mIdiomas;

        /// <summary>
        /// Lista de notificaciones
        /// </summary>
        private Dictionary<Guid, Notificacion> mListaNotificaciones;

        /// <summary>
        /// Lista de invitaciones
        /// </summary>
        private Dictionary<Guid, Invitacion.Invitacion> mListaInvitaciones;

        /// <summary>
        /// Gestor de identidades (para las invitaciones)
        /// </summary>
        private GestionIdentidades mGestorIdentidades;

        /// <summary>
        /// Gestor de peticiones
        /// </summary>
        private GestionPeticiones mGestorPeticiones;

        /// <summary>
        /// Lista de mensajes con el formato Dictionary(idioma, Dictionary(mensajeID, KeyValuePair(asunto, Texto)))
        /// </summary>
        private Dictionary<string, Dictionary<int, KeyValuePair<string, string>>> mListaMensajes;

        private Dictionary<int, KeyValuePair<string, string>> mListaMensajesDefecto;

        /// <summary>
        /// Lista con los formatos extra para el correo (cabecera y pie), del tipo Dictionary(idioma, Dictionary(nombre fragmento ["cabecera","pie"], string formato))
        /// </summary>
        private Dictionary<string, Dictionary<string, string>> mListaFormatosCorreo;

        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #region  Miembros estáticos

        private static string mTextoTodos = "Todos";
        private static string mTextoComunidad = "Comunidad";
        private static string mTextoParticipantes = "Participantes";
        private static string mTextoPrivada = "privada";
        private static string mTextoPublica = "pública";
        private static string mTextoVisible = "visible";
        private static string mTextoNoVisible = "no visible";
        private static bool? mHayConexionRabbit = null;

        #endregion

        #endregion

        #region Constructores

        public GestionNotificaciones() { }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        protected GestionNotificaciones(SerializationInfo pInfo, StreamingContext pContext)
            : base(pInfo, pContext)
        {
            mGestorPeticiones = (GestionPeticiones)pInfo.GetValue("GestorPeticiones", typeof(GestionPeticiones));
            mGestorIdentidades = (GestionIdentidades)pInfo.GetValue("GestorIdentidades", typeof(GestionIdentidades));
        }

        /// <summary>
        /// Crea el gestor de notificaciones SIN gestor de correo y SIN gestor de identidades (para las invitaciones)
        /// </summary>
        /// <param name="pNotificacionDW">Dataset de notificaciones</param>
        public GestionNotificaciones(DataWrapperNotificacion pNotificacionDW, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GestionNotificaciones> logger,ILoggerFactory loggerFactory)
            : base(pNotificacionDW)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Crea el gestor de notificaciones y el gestor de identidades SIN gestor de correo
        /// </summary>
        /// <param name="pNotificacionDW">Dataset de notificaciones</param>
        /// <param name="pGestorIdentidades">Gestor de identidades</param>
        public GestionNotificaciones(DataWrapperNotificacion pNotificacionDW, GestionIdentidades pGestorIdentidades, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GestionNotificaciones> logger,ILoggerFactory loggerFactory)
            : base(pNotificacionDW)
        {
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mGestorIdentidades = pGestorIdentidades;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Métodos generales

        #region Públicos

        #region Contactos y amigos

        /// <summary>
        /// Agrega una notificación para generar un correo advirtiendo de que ha habido un cambio en un contacto
        /// </summary>
        /// <param name="pNombreContactoMovido">Nombre del contacto que ha desaparecido</param>
        /// <param name="pNombreOrganizacion">Nombre de la organización que ocupará el lugar de ese contacto</param>
        /// <param name="pEmailDestinatario">Email del destinatario</param>
        /// <param name="pPersonaID">Identificador de persona del destinatario</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        public void AgregarNotificacionCambioContacto(string pNombreContactoMovido, string pNombreOrganizacion, string pEmailDestinatario, Guid pPersonaID, string pNombreDestinatario, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.CambioContactoAOrg, fechaHoy, fechaHoy.AddDays(1), null, null);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreContactoMovido);
            listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pNombreOrganizacion);
            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pEmailDestinatario, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificación para generar un correo advirtiendo de que se ha eliminado un contacto
        /// </summary>
        /// <param name="pNombreContactoEliminado">Nombre del contacto que se va a eliminar</param>
        /// <param name="pNombreOrganizacion">Nombre de la organización que ocupará el lugar de ese contacto</param>
        /// <param name="pEmailDestinatario">Email del destinatario</param>
        /// <param name="pPersonaID">Identificador de persona del destinatario</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        public void AgregarNotificacionContactoEliminado(string pNombreContactoEliminado, string pNombreOrganizacion, string pEmailDestinatario, Guid pPersonaID, string pNombreDestinatario, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.ContactoEliminado, fechaHoy, fechaHoy.AddDays(1), null, null);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreContactoEliminado);
            listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pNombreOrganizacion);
            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pEmailDestinatario, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificación de invitación para ser contacto
        /// </summary>
        /// <param name="pIdentidadOrigen">Identidad de quien realiza la invitación</param>
        /// <param name="pIdentidadDestino">Identidad de quien recibe la invitación</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pUrlIntragnoss">URL de la intranet de GNOSS</param>
        public Guid AgregarNotificacionInvitacionContacto(Identidad.Identidad pIdentidadOrigen, Identidad.Identidad pIdentidadDestino, TiposNotificacion pTipoNotificacion, string pUrlIntragnoss, Elementos.ServiciosGenerales.Proyecto pProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, fechaHoy, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);
            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            AD.EntityModel.Models.Notificacion.Invitacion filaInvitacion = new AD.EntityModel.Models.Notificacion.Invitacion();
            filaInvitacion.IdentidadOrigenID = pIdentidadOrigen.Clave;
            filaInvitacion.IdentidadDestinoID = pIdentidadDestino.Clave;
            filaInvitacion.NotificacionID = filaNotificacion.NotificacionID;
            filaInvitacion.InvitacionID = Guid.NewGuid();
            filaInvitacion.Estado = (int)EstadoInvitacion.Pendiente;
            filaInvitacion.FechaInvitacion = fechaHoy;
            filaInvitacion.TipoInvitacion = (short)pTipoNotificacion;

            NotificacionDW.ListaInvitacion.Add(filaInvitacion);
            mEntityContext.Invitacion.Add(filaInvitacion);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();
            listaParametros.Add((short)ClavesParametro.NombreRemitente, pIdentidadOrigen.Nombre(pIdentidadDestino.Clave));
            listaParametros.Add((short)ClavesParametro.NombrePersona, pIdentidadDestino.NombreCorto);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            TiposNotificacion pTipoCorreo = TiposNotificacion.InvitacionCorreoContacto;

            AgregarNotificacionCorreo(pIdentidadOrigen, pIdentidadDestino, pTipoCorreo, pUrlIntragnoss, pProyecto, pLanguageCode);

            return filaInvitacion.InvitacionID;
        }

        /// <summary>
        /// Acepta las notificaciones de invitacion entre dos contactos
        /// </summary>
        /// <param name="pIdentidadOrigen">Identidad de quien realiza la invitación</param>
        /// <param name="pIdentidadDestino">Identidad de quien recibe la invitación</param>
        public void AceptarInvitacionesEntreContactos(Identidad.Identidad pIdentidadOrigen, Identidad.Identidad pIdentidadDestino)
        {
            List<AD.EntityModel.Models.Notificacion.Invitacion> invitaciones = NotificacionDW.ListaInvitacion.Where(item => (item.IdentidadOrigenID.Equals(pIdentidadOrigen.Clave) && item.IdentidadDestinoID.Equals(pIdentidadDestino.Clave)) || (item.IdentidadOrigenID.Equals(pIdentidadDestino.Clave) && item.IdentidadDestinoID.Equals(pIdentidadOrigen.Clave))).ToList();

            foreach (AD.EntityModel.Models.Notificacion.Invitacion filaInvitacion in invitaciones)
            {
                filaInvitacion.Estado = (short)EstadoInvitacion.Aceptada;
            }
        }

        /// <summary>
        /// Elimina una notificación de contacto
        /// </summary>
        /// <param name="pIdentidadOrigen">Identidad de quien realiza la invitación</param>
        /// <param name="pIdentidadDestino">Identidad de quien recibe la invitación</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        public void EliminarNotificacionContacto(Identidad.Identidad pIdentidadOrigen, Identidad.Identidad pIdentidadDestino, TiposNotificacion pTipoNotificacion, ServiciosGenerales.Proyecto pProyecto, string pLanguageCode)
        {
            AgregarNotificacionCorreo(pIdentidadOrigen, pIdentidadDestino, pTipoNotificacion, "", pProyecto, pLanguageCode);

            List<AD.EntityModel.Models.Notificacion.Invitacion> invitaciones = NotificacionDW.ListaInvitacion.Where(item => (item.IdentidadOrigenID.Equals(pIdentidadOrigen.Clave) && item.IdentidadDestinoID.Equals(pIdentidadDestino.Clave)) || (item.IdentidadOrigenID.Equals(pIdentidadDestino.Clave) && item.IdentidadDestinoID.Equals(pIdentidadOrigen.Clave))).ToList();

            foreach (AD.EntityModel.Models.Notificacion.Invitacion filaInvitacion in invitaciones)
            {
                filaInvitacion.Estado = (short)EstadoInvitacion.Rechazada;
            }
        }

        #endregion

        /// <summary>
        /// Envía un correo con un mensaje de error
        /// </summary>
        /// <param name="pMensajeError">Mensaje de error</param>
        /// <param name="pVersion">Versión</param>
        /// <returns></returns>
        public static bool EnviarCorreoError(string pMensajeError, string pVersion, List<AD.EntityModel.ParametroAplicacion> pParametrosApliacion)
        {
            return EnviarCorreoError(pMensajeError, pVersion, null, pParametrosApliacion);
        }

        /// <summary>
        /// Envía un correo con un mensaje de error
        /// </summary>
        /// <param name="pMensajeError">Mensaje de error</param>
        /// <param name="pVersion">Versión</param>
        /// <param name="pCorreoDestinatario">Clave del parámetro que indica la dirección de correo del destinatario (null para enviarla a CorreoErrores)</param>
        /// <returns></returns>
        public static bool EnviarCorreoError(string pMensajeError, string pVersion, string pClaveCorreoDestinatario, List<AD.EntityModel.ParametroAplicacion> filasConfiguracion)
        {
            //ParametroAplicacionDS.ParametroAplicacionRow[] filasConfiguracion = (ParametroAplicacionDS.ParametroAplicacionRow[])pParametroAplicacionDS.ParametroAplicacion.Select();

            Dictionary<string, string> parametros = new Dictionary<string, string>();

            foreach (AD.EntityModel.ParametroAplicacion fila in filasConfiguracion)
            {
                parametros.Add(fila.Parametro, fila.Valor);
            }

            if (string.IsNullOrEmpty(pClaveCorreoDestinatario) || !parametros.ContainsKey(pClaveCorreoDestinatario))
            {
                pClaveCorreoDestinatario = "CorreoErrores";
            }

            if (parametros.Count > 0)
            {
                try
                {
                    return UtilCorreo.EnviarCorreoError(pMensajeError, pVersion, (string)parametros["CorreoErrores"], (string)parametros[pClaveCorreoDestinatario], (string)parametros["ServidorSmtp"], int.Parse(parametros["PuertoSmtp"]), (string)parametros["UsuarioSmtp"], (string)parametros["PasswordSmtp"]);
                }
                catch (Exception)
                {
                    return UtilCorreo.EnviarCorreoError(pMensajeError, pVersion, null, null, null, -1, null, null);
                }
            }
            else
            {
                return UtilCorreo.EnviarCorreoError(pMensajeError, pVersion, null, null, null, -1, null, null);
            }
        }

        /// <summary>
        /// Agrega notificaciones a las personas sobre una notificación
        /// </summary>
        /// <param name="pNotificacionRow">Fila de notificación</param>
        /// <param name="pListaPersonas">Lista de personas</param>
        public void AgregarPersonasAAlerta(AD.EntityModel.Models.Notificacion.Notificacion pNotificacionRow, List<Persona> pListaPersonas)
        {
            foreach (Persona persona in pListaPersonas)
            {
                AgregarAlertaPersonaRow(pNotificacionRow, persona);
            }
        }

        /// <summary>
        /// Agrega notificaciones a las personas sobre una notificación
        /// </summary>
        /// <param name="pNotificacion">Notificacion</param>
        /// <param name="pListaPersonas">Lista de personas</param>
        /// <param name="pRemitente">Remitente</param>
        /// <param name="pMascaraRemitente">Máscara del remitente</param>
        /// <param name="pEmailRespuesta">Email de respuesta</param>
        /// <param name="pMascaraEmailRespuesta">Máscara del email de respuesta</param>
        /// <param name="pFormatoHTML">TRUE si el correo se escribe en formato HTML</param>
        public void AgregarPersonasACorreo(Notificacion pNotificacion, IList<Persona> pListaPersonas, string pRemitente, string pMascaraRemitente, string pEmailRespuesta, string pMascaraEmailRespuesta, bool pFormatoHTML)
        {
            foreach (Persona persona in pListaPersonas)
            {
                string correo = string.Empty;

                if (!string.IsNullOrEmpty(persona.FilaPersona.Email))
                {
                    correo = persona.FilaPersona.Email.ToLower();
                }
                if (!correo.Equals(string.Empty))
                {
                    AgregarCorreoPersonaRow(pNotificacion.FilaNotificacion, correo, persona.Clave);
                }
            }
        }

        /// <summary>
        /// Agrega la notificacion necesaria para enviar por correo el boletín de suscripciones
        /// </summary>
        /// <param name="PersonaID">Identificador de persona a la que se le genera la notificación</param>
        /// <param name="OrganizacionID">Identificador de organización de la persona a la que se genera</param>
        /// <param name="pUltimasPublicaciones">Cadena de texto con las últimas publicaciones</param>
        /// <param name="pNombrePersona">Nombre de la persona</param>
        /// <param name="pEmailEnvio">Email que se usa para realizar el envío</param>
        public void AgregarNotificacionBoletinSuscripcion(Guid PersonaID, Guid? OrganizacionID, string pUltimasPublicaciones, string pNombrePersona, string pEmailEnvio, Guid? pOrganizacionProyectoID, Guid pProyectoID, string pNombreProyecto, string pLanguageCode)
        {
            if (!pUltimasPublicaciones.Equals(string.Empty))
            {
                DateTime fechaNotificacion = DateTime.Now;

                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.BoletinSuscripcion, fechaNotificacion, fechaNotificacion.AddDays(1), pOrganizacionProyectoID, pProyectoID);

                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.NombrePersona, pNombrePersona);
                listaParametros.Add((short)ClavesParametro.UltimasPublicaciones, pUltimasPublicaciones);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                //NotificacionCorreoPersona            
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pEmailEnvio, PersonaID, OrganizacionID);
            }
        }

        #region Solicitudes

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pNombrePersona">Nombre de persona</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersona">Persona a la que le afecta la notificación (puede ser NULL)</param>
        /// <param name="pUrlIntragnoss">Url de INTRAGnoss</param>
        /// <param name="pIdioma">Idioma de la notificación</param>
        /// <param name="pNombreAsignatura">Nombre de la asignatura</param>
        public void AgregarNotificacionSolicitudEntradaGNOSS(Guid pSolicitudID, string pNombrePersona, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersona, string pUrlIntragnoss, string pIdioma, string pNombreAsignatura)
        {
            AgregarNotificacionSolicitudEntradaGNOSS(pSolicitudID, pNombrePersona, "", pTipoNotificacion, pCorreoPersonaNotificacion, pPersona, pUrlIntragnoss, pIdioma, "", pNombreAsignatura);
        }

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pNombrePersona">Nombre de persona</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersona">Persona a la que le afecta la notificación (puede ser NULL)</param>
        /// <param name="pUrlIntragnoss">Url de INTRAGnoss</param>
        /// <param name="pIdioma">Idioma de la notificación</param>
        public void AgregarNotificacionSolicitudEntradaGNOSS(Guid pSolicitudID, string pNombrePersona, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersona, string pUrlIntragnoss, string pIdioma)
        {
            AgregarNotificacionSolicitudEntradaGNOSS(pSolicitudID, pNombrePersona, null, pTipoNotificacion, pCorreoPersonaNotificacion, pPersona, pUrlIntragnoss, pIdioma);
        }

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pNombrePersona">Nombre de persona</param>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersona">Persona a la que le afecta la notificación (puede ser NULL)</param>
        /// <param name="pUrlIntragnoss">Url de INTRAGnoss</param>
        /// <param name="pIdioma">Idioma</param>
        public void AgregarNotificacionSolicitudEntradaGNOSS(Guid pSolicitudID, string pNombrePersona, string pNombreOrganizacion, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersona, string pUrlIntragnoss, string pIdioma)
        {
            AgregarNotificacionSolicitudEntradaGNOSS(pSolicitudID, pNombrePersona, pNombreOrganizacion, pTipoNotificacion, pCorreoPersonaNotificacion, pPersona, pUrlIntragnoss, pIdioma, null);
        }

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pNombrePersona">Nombre de persona</param>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersona">Persona a la que le afecta la notificación (puede ser NULL)</param>
        /// <param name="pUrlIntragnoss">Url de INTRAGnoss</param>
        /// <param name="pIdioma">Idioma</param>
        /// <param name="pEnlace">Enlace web</param>
        public void AgregarNotificacionSolicitudEntradaGNOSS(Guid pSolicitudID, string pNombrePersona, string pNombreOrganizacion, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersona, string pUrlIntragnoss, string pIdioma, string pEnlace)
        {
            AgregarNotificacionSolicitudEntradaGNOSS(pSolicitudID, pNombrePersona, pNombreOrganizacion, pTipoNotificacion, pCorreoPersonaNotificacion, pPersona, pUrlIntragnoss, pIdioma, pEnlace, "");
        }

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pNombrePersona">Nombre de persona</param>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersona">Persona a la que le afecta la notificación (puede ser NULL)</param>
        /// <param name="pUrlIntragnoss">Url de INTRAGnoss</param>
        /// <param name="pIdioma">Idioma</param>
        /// <param name="pEnlace">Enlace web</param>
        /// <param name="pNombreAsignatura">Nombre de asignatura</param>
        public void AgregarNotificacionSolicitudEntradaGNOSS(Guid pSolicitudID, string pNombrePersona, string pNombreOrganizacion, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersona, string pUrlIntragnoss, string pIdioma, string pEnlace, string pNombreAsignatura)
        {
            AgregarNotificacionSolicitudEntradaGNOSS(pSolicitudID, pNombrePersona, pNombreOrganizacion, pTipoNotificacion, pCorreoPersonaNotificacion, pPersona, pUrlIntragnoss, pIdioma, pEnlace, pNombreAsignatura, ProyectoAD.MetaProyecto);
        }

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pNombrePersona">Nombre de persona</param>
        /// <param name="pNombreOrganizacion">Nombre de organización</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersona">Persona a la que le afecta la notificación (puede ser NULL)</param>
        /// <param name="pUrlIntragnoss">Url de INTRAGnoss</param>
        /// <param name="pIdioma">Idioma</param>
        /// <param name="pEnlace">Enlace web</param>
        /// <param name="pNombreAsignatura">Nombre de asignatura</param>
        /// <param name="pProyectoRegistroID">ID del proyecto de donde debe proceder la notificación</param>
        public void AgregarNotificacionSolicitudEntradaGNOSS(Guid pSolicitudID, string pNombrePersona, string pNombreOrganizacion, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersona, string pUrlIntragnoss, string pIdioma, string pEnlace, string pNombreAsignatura, Guid pProyectNotificacionID)
        {
            DateTime fechaHoy = DateTime.Now;

            string codigo = "Fecha:   " + fechaHoy + "\n\n";
            string emailDestinatario = pCorreoPersonaNotificacion;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, fechaHoy, ProyectoAD.MetaOrganizacion, pProyectNotificacionID);
            notificacion.FilaNotificacion.Idioma = pIdioma;

            //Fila de NotificacionSolicitud
            AD.EntityModel.Models.Notificacion.NotificacionSolicitud filaNotificacionSolicitud = new AD.EntityModel.Models.Notificacion.NotificacionSolicitud();

            filaNotificacionSolicitud.NotificacionID = notificacion.FilaNotificacion.NotificacionID;
            filaNotificacionSolicitud.SolicitudID = pSolicitudID;
            filaNotificacionSolicitud.OrganizacionID = ProyectoAD.MetaOrganizacion;
            filaNotificacionSolicitud.ProyectoID = pProyectNotificacionID;

            NotificacionDW.ListaNotificacionSolicitud.Add(filaNotificacionSolicitud);
            mEntityContext.NotificacionSolicitud.Add(filaNotificacionSolicitud);

            Guid? idPersona = null;

            if (pPersona != null)
            {
                idPersona = pPersona.Clave;
            }
            //NotificacionCorreoPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pCorreoPersonaNotificacion, idPersona, pPersona);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();
            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombrePersona);

            if (pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevoUsuario) || pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevaOrganizacionYUsuario) || pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevoProfesor) || pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevoUsuarioCorporateExcellence) || pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevoUsuarioTutor))
            {
                listaParametros.Add((short)ClavesParametro.UrlEnlace, pEnlace);
            }

            if ((pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevaOrganizacionYUsuario)))
            {
                string nombreOrg = "";

                if (pNombreOrganizacion != null)
                {
                    nombreOrg = pNombreOrganizacion;
                }
                listaParametros.Add((short)ClavesParametro.NombreOrganizacion, nombreOrg);
            }

            if ((pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevoUsuarioTutor)))
            {
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreAsignatura);
                listaParametros.Add((short)ClavesParametro.IdentidadRemitente, pNombreOrganizacion);
            }

            if (pTipoNotificacion.Equals(TiposNotificacion.SolicitudNuevoUsuarioNotificacionTutor))
            {
                listaParametros.Add((short)ClavesParametro.NombreAsignatura, pNombreAsignatura);
            }
            AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);


            if (!ListaNotificaciones.ContainsKey(notificacion.Clave))
            {
                ListaNotificaciones.Add(notificacion.Clave, notificacion);
            }
        }


        public void AgregarNotificacionRegistroParcialComunidad(Guid pSolicitudID, string pNombrePersona, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Guid? pPersonaID, string pUrlIntragnoss, string pIdioma, string pNombreCortoComunidad, string pEnlace, Guid pProyectNotificacionID)
        {
            DateTime fechaHoy = DateTime.Now;

            string codigo = "Fecha:   " + fechaHoy + "\n\n";
            string emailDestinatario = pCorreoPersonaNotificacion;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, fechaHoy, ProyectoAD.MetaOrganizacion, pProyectNotificacionID);
            notificacion.FilaNotificacion.Idioma = pIdioma;

            //Fila de NotificacionSolicitud
            AD.EntityModel.Models.Notificacion.NotificacionSolicitud filaNotificacionSolicitud = new AD.EntityModel.Models.Notificacion.NotificacionSolicitud();

            filaNotificacionSolicitud.NotificacionID = notificacion.FilaNotificacion.NotificacionID;
            filaNotificacionSolicitud.SolicitudID = pSolicitudID;
            filaNotificacionSolicitud.OrganizacionID = ProyectoAD.MetaOrganizacion;
            filaNotificacionSolicitud.ProyectoID = pProyectNotificacionID;

            NotificacionDW.ListaNotificacionSolicitud.Add(filaNotificacionSolicitud);
            mEntityContext.NotificacionSolicitud.Add(filaNotificacionSolicitud);


            Guid? idPersona = null;

            if (pPersonaID.HasValue)
            {
                idPersona = pPersonaID.Value;
            }
            //NotificacionCorreoPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pCorreoPersonaNotificacion, idPersona);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();
            if (pTipoNotificacion.Equals(TiposNotificacion.InvitacionRegistroParcialComunidad))
            {
                listaParametros.Add((short)ClavesParametro.NombrePersona, pNombrePersona);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreCortoComunidad);
                listaParametros.Add((short)ClavesParametro.UrlEnlace, pEnlace);
            }

            AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

            if (!ListaNotificaciones.ContainsKey(notificacion.Clave))
            {
                ListaNotificaciones.Add(notificacion.Clave, notificacion);
            }
        }

        /// <summary>
        /// Agrega una notificación de solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la solicitud</param>
        /// <param name="pFechaEnvio">Fecha de envío de la solicitud</param>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <param name="pLogin">Login del usuario</param>
        /// <param name="pPassword">Contraseña del usuario</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo electrónico de la persona a la que le afecta la notificación</param>
        /// <param name="pPersonaNotificacion">Persona a la que le afecta la notificación</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void AgregarNotificacionSolicitud(Guid pSolicitudID, DateTime pFechaEnvio, string pNombreProyecto, string pLogin, string pPassword, TiposNotificacion pTipoNotificacion, string pCorreoPersonaNotificacion, Persona pPersonaNotificacion, Guid pOrganizacionID, Guid pProyectoID, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            string codigo = "Fecha:   " + DateTime.Now + "\n\n";

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, pFechaEnvio, pOrganizacionID, pProyectoID);

            //Fila de NotificacionSolicitud
            AD.EntityModel.Models.Notificacion.NotificacionSolicitud filaNotificacionSolicitud = new AD.EntityModel.Models.Notificacion.NotificacionSolicitud();

            filaNotificacionSolicitud.NotificacionID = notificacion.FilaNotificacion.NotificacionID;
            filaNotificacionSolicitud.SolicitudID = pSolicitudID;
            filaNotificacionSolicitud.OrganizacionID = pOrganizacionID;
            filaNotificacionSolicitud.ProyectoID = pProyectoID;

            NotificacionDW.ListaNotificacionSolicitud.Add(filaNotificacionSolicitud);
            mEntityContext.NotificacionSolicitud.Add(filaNotificacionSolicitud);

            //NotificacionCorreoPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pCorreoPersonaNotificacion, pPersonaNotificacion.Clave, pPersonaNotificacion);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);

            switch (pTipoNotificacion)
            {
                case TiposNotificacion.SolicitudAceptadaAccesoProyecto:
                    {
                        listaParametros.Add((short)ClavesParametro.Login, pLogin);
                        break;
                    }
                case TiposNotificacion.SolicitudAviso:
                    {
                        listaParametros.Add((short)ClavesParametro.Login, pLogin);
                        break;
                    }
                case TiposNotificacion.SolicitudPendiente:
                    {
                        break;
                    }
            }
            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);
        }

        /// <summary>
        /// 
        /// </summary>      
        /// <param name="pPersonaNotificacion">Persona a la que le afecta la notificación</param>
        /// <param name="pUltimoAviso">Indica si se trata de una notificación a un usuario que lleva mas de 3 días sin corregir su identidad</param>       
        public void AgregarNotificacionCorreccionDeIdentidad(Persona pPersonaNotificacion, bool pUltimoAviso, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            string codigo = "Fecha:   " + DateTime.Now + "\n\n";
            string emailDestinatario = pPersonaNotificacion.FilaPersona.Email;

            Notificacion notificacion;

            //Fila de la notificación
            if (pUltimoAviso)
            {
                notificacion = AgregarNotificacion(TiposNotificacion.AvisoCorreccionDeIdentidadDefinitivo, fechaHoy, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto);
            }
            else
            {
                notificacion = AgregarNotificacion(TiposNotificacion.AvisoCorreccionDeIdentidad, fechaHoy, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto);
            }

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pPersonaNotificacion.Nombre);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //NotificacionCorreoPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, emailDestinatario, pPersonaNotificacion.Clave);
        }

        /// <summary>
        /// 
        /// </summary>      
        /// <param name="pPersonaNotificacion">Persona a la que le afecta la notificación</param>
        public void AgregarNotificacionEliminacionDeUsuario(Persona pPersonaNotificacion, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            string codigo = "Fecha:   " + DateTime.Now + "\n\n";
            string emailDestinatario = pPersonaNotificacion.FilaPersona.Email;

            Notificacion notificacion;

            notificacion = AgregarNotificacion(TiposNotificacion.AvisoEliminacionDeUsuario, fechaHoy, ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pPersonaNotificacion.Nombre);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //NotificacionCorreoPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, emailDestinatario, pPersonaNotificacion.Clave);
        }

        /// <summary>
        /// Agrega una notificación confirmando el registro en un evento de comunidad
        /// </summary>
        /// <param name="pNombreUsuario">Nombre del usuario</param>
        /// <param name="pNombreEvento">Nombre del evento</param>
        /// <param name="pCuerpoMensaje">Mensaje cuerpo</param>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pOrganizacionID">Identificador de organización del proyecto</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pNombreComunidad">Nombre de la comunidad solicitada</param>
        /// <param name="pDireccionCorreo">Dirección de correo a la que se envía la notificación</param>
        public void AgregarNotificacionConfirmacionEventoComunidad(string pNombreUsuario, string pNombreEvento, string pCuerpoMensaje, Guid pPersonaID, Guid pOrganizacionID, Guid pProyectoID, string pNombreComunidad, string pDireccionCorreo, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.AvisoRegistroEventoComunidad, fechaHoy, fechaHoy.AddDays(1), pOrganizacionID, pProyectoID);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreUsuario);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreComunidad);
            listaParametros.Add((short)ClavesParametro.NombreEvento, pNombreEvento);
            listaParametros.Add((short)ClavesParametro.CuerpoMensajeEvento, pCuerpoMensaje);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDireccionCorreo, pPersonaID);
        }


        /// <summary>
        /// Agrega una notificación de una solicitud para crear una comunidad
        /// </summary>
        /// <param name="pIdentidadRemitente">Identidad del remitente de la solicitud</param>
        /// <param name="pUrlIntraGnoss">URL de intragnoss</param>
        /// <param name="pAsunto">Asunto de la solicitud</param>
        /// <param name="pDescripcion">Descripción de la solicitud</param>
        /// <param name="pDireccionCorreo">Dirección de correo a la que se envía la solicitud</param>
        public void AgregarNotificacionSolicitudCrearComunidad(Identidad.Identidad pIdentidadRemitente, string pBaseUrl, string pAsunto, string pDescripcion, string pDireccionCorreo, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.SolicitudCreacionComunidad, fechaHoy, fechaHoy.AddDays(1), null, null);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            string UrlPerfilRemitente = "/";

            if (pIdentidadRemitente.TrabajaConOrganizacion)
            {
                UrlPerfilRemitente += "organizacion/" + pIdentidadRemitente.PerfilUsuario.NombreCortoOrg + "/";
            }
            if (pIdentidadRemitente.ModoPersonal)
            {
                UrlPerfilRemitente += "perfil/" + pIdentidadRemitente.PerfilUsuario.NombreCortoUsu + "/";
            }


            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombreRemitente, pIdentidadRemitente.Nombre());
            listaParametros.Add((short)ClavesParametro.IdentidadPersona, "");
            listaParametros.Add((short)ClavesParametro.IdentidadRemitente, UrlPerfilRemitente);
            listaParametros.Add((short)ClavesParametro.Asunto, pAsunto);
            listaParametros.Add((short)ClavesParametro.Texto, pDescripcion);
            listaParametros.Add((short)ClavesParametro.BaseUrl, pBaseUrl);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDireccionCorreo, null);
        }

        /// <summary>
        /// Agrega una notificación confirmando la aceptación de una nueva comunidad
        /// </summary>
        /// <param name="pNombreSolicitante">Nombre del solicitante</param>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pNombreComunidad">Nombre de la comunidad solicitada</param>
        /// <param name="pDireccionCorreo">Dirección de correo a la que se envía la notificación</param>
        /// <param name="pEnlace">enlace a la comunidad nueva</param>
        public void AgregarNotificacionSolicitudAceptadaNuevaComunidad(string pNombreSolicitante, Guid pPersonaID, Guid pProyectoID, Guid pOrganizacionID, string pNombreComunidad, string pDireccionCorreo, string pEnlace, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.SolicitudAceptadaNuevaComunidad, fechaHoy, fechaHoy.AddDays(1), pOrganizacionID, pProyectoID);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreSolicitante);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreComunidad);
            listaParametros.Add((short)ClavesParametro.UrlEnlace, pEnlace);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDireccionCorreo, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificación confirmando el rechazo de una nueva comunidad
        /// </summary>
        /// <param name="pNombreSolicitante">Nombre del solicitante</param>
        /// <param name="pPersonaID">Identificador de persona</param>
        /// <param name="pNombreComunidad">Nombre de la comunidad solicitada</param>
        /// <param name="pDireccionCorreo">Dirección de correo a la que se envía la notificación</param>
        public void AgregarNotificacionSolicitudRechazadaNuevaComunidad(string pNombreSolicitante, Guid pPersonaID, string pNombreComunidad, string pDireccionCorreo, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.SolicitudRechazadaNuevaComunidad, fechaHoy, fechaHoy.AddDays(1), null, null);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreSolicitante);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreComunidad);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDireccionCorreo, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificación confirmando la aceptación de una nueva clase.
        /// </summary>
        /// <param name="pNombreSolicitante">Nombre del solicitante</param>
        /// <param name="pTipoNotificacion"></param>
        /// <param name="pPersona">Persona solicitante</param>
        /// <param name="pBaseUrl">Url base</param>
        /// <param name="pNombreClasesConLinks">Nombre de la/las clase/clases solicitada/as</param>
        /// <param name="pNombreAsignatura">Nombre de la asignatura solicitada</param>
        /// <param name="pEnlaceAsignatura">Link a la asignatura</param>
        /// <param name="pEnlaceAClases">Enlace a las clases</param>
        /// <param name="pEnlaceAComunidades">Enlace a las comunidades</param>
        public void AgregarNotificacionSolicitudAceptadaNuevaClase(string pNombreSolicitante, TiposNotificacion pTipoNotificacion, Persona pPersona, string pBaseUrl, Dictionary<string, string> pNombreClasesConLinks, string pNombreAsignatura, string pEnlaceAsignatura, string pEnlaceAClases, string pEnlaceAComunidades, string pLanguageCode)
        {
            if (pNombreClasesConLinks.Count > 1 && pTipoNotificacion != TiposNotificacion.SolicitudAceptadaNuevaClases)
            {
                AgregarNotificacionSolicitudAceptadaNuevaClase(pNombreSolicitante, TiposNotificacion.SolicitudAceptadaNuevaClases, pPersona, pBaseUrl, pNombreClasesConLinks, pNombreAsignatura, pEnlaceAsignatura, pEnlaceAClases, pEnlaceAComunidades, pLanguageCode);
                return;
            }
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, fechaHoy, fechaHoy.AddDays(1), ProyectoAD.MetaOrganizacion, ProyectoAD.MetaProyecto);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            //Creo los nombre de las clases:
            string htmlClases = "";
            string htmlUltimaClase = "";

            if (pNombreClasesConLinks.Count == 1)
            {
                foreach (string nombreClase in pNombreClasesConLinks.Keys)
                {
                    htmlClases += "<a herf=\"" + pNombreClasesConLinks[nombreClase] + "\" target=\"_blank\" style=\"cursor:pointer;\">" + nombreClase + "</a>";
                }
            }
            else
            {
                int count = 0;
                foreach (string nombreClase in pNombreClasesConLinks.Keys)
                {
                    string clase = "<a href=\"" + pNombreClasesConLinks[nombreClase] + "\" target=\"_blank\" style=\"cursor:pointer;\">" + nombreClase + "</a>";
                    count++;

                    if (count < pNombreClasesConLinks.Count)
                    {
                        htmlClases += clase;

                        if (count < pNombreClasesConLinks.Count - 1)
                        {
                            htmlClases += ", ";
                        }
                    }
                    else
                    {
                        htmlUltimaClase = clase;
                    }
                }
            }

            string htmlAsignatura = "<a href=\"" + pEnlaceAsignatura + "\" target=\"_blank\" style=\"cursor:pointer;\">" + pNombreAsignatura + "</a>";

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreSolicitante);
            listaParametros.Add((short)ClavesParametro.NombreOrganizacion, htmlClases);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, htmlAsignatura);
            listaParametros.Add((short)ClavesParametro.Texto, pEnlaceAClases);
            listaParametros.Add((short)ClavesParametro.Texto2, pEnlaceAComunidades);

            if (pTipoNotificacion == TiposNotificacion.SolicitudAceptadaNuevaClases)
            {
                listaParametros.Add((short)ClavesParametro.UrlEnlace, htmlUltimaClase);
            }

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pPersona.Email, pPersona.Clave);
        }

        #endregion

        /// <summary>
        /// Agrega una notificación de petición de cambio de password
        /// </summary>
        /// <param name="pPersona">Persona</param>
        /// <param name="pLink">Link a la pagina de cambio de password</param>
        /// <param name="pOrganizacionID">OrganizacionID</param>
        /// <param name="pProyectoID">ProyectoID</param>
        public void AgregarNotificacionPeticionCambioPassword(Persona pPersona, string pLink, Elementos.ServiciosGenerales.Proyecto pProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.PeticionCambioPassword, fechaHoy, fechaHoy.AddDays(7), pProyecto.FilaProyecto.OrganizacionID, pProyecto.FilaProyecto.ProyectoID);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
            filaNotificacion.Idioma = pLanguageCode;
            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.UrlEnlace, pLink);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            if (!string.IsNullOrEmpty(pPersona.FilaPersona.Email))
            {
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pPersona.FilaPersona.Email, pPersona.Clave);
            }
            else
            {
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pPersona.FilaPersona.EmailTutor, pPersona.Clave);
            }
        }

        /// <summary>
        /// Agrega una notificación de petición de autenticacion de doble factor
        /// </summary>
        public void AgregarNotificacionPeticionAutenticacionDobleFactor(Persona pPersona, string pToken, Elementos.ServiciosGenerales.Proyecto pProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.PeticionAutenticacionDobleFactor, fechaHoy, fechaHoy.AddDays(7), pProyecto.FilaProyecto.OrganizacionID, pProyecto.FilaProyecto.ProyectoID);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.TokenSeguridad, pToken);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            if (!string.IsNullOrEmpty(pPersona.FilaPersona.Email))
            {
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pPersona.FilaPersona.Email, pPersona.Clave);
            }
            else
            {
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pPersona.FilaPersona.EmailTutor, pPersona.Clave);
            }
        }

        #region Invitaciones

        /// <summary>
        /// Agrega una notificación de invitación externa
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <param name="pNotas">Notas</param>
        /// <param name="pNombreRemitente">Nombre del remitente</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pFechaComienzoNotificacion">Fecha de comienzo de la notificación</param>
        /// <param name="pDestinatarios">Destinatarios</param>
        /// <param name="pURLIntragnoss">URL de la intranet de GNOSS</param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionInvitacionExterna(Guid pOrganizacionID, Guid pProyectoID, string pNombreProyecto, string pNotas, string pNombreRemitente, TiposNotificacion pTipoNotificacion, DateTime pFechaComienzoNotificacion, IList<string> pDestinatarios, string pBaseUrl, string pIdioma)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, pFechaComienzoNotificacion, pFechaComienzoNotificacion.AddDays(1), pOrganizacionID, pProyectoID);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
            filaNotificacion.Idioma = pIdioma;

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            switch (pTipoNotificacion)
            {
                case TiposNotificacion.InvitacionPendiente:
                    listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreRemitente);
                    listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
                    listaParametros.Add((short)ClavesParametro.ProyectoID, pProyectoID.ToString());
                    listaParametros.Add((short)ClavesParametro.OrganizacionID, pOrganizacionID.ToString());
                    listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                    listaParametros.Add((short)ClavesParametro.BaseUrl, pBaseUrl);
                    break;
            }
            AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

            //Filas de NotificacionPersona
            foreach (string destinatario in pDestinatarios)
            {
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        public void AgregarNotificacionSolicitudRechazada(Guid? pOrganizacionID, Guid? pProyectoID, string pNombreProyecto, string pMensaje, string pNombreRemitente, TiposNotificacion pTipoNotificacion, List<string> pDestinatarios, string pURLIntragnoss, string pIdioma)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, fechaHoy, fechaHoy.AddDays(1), pOrganizacionID, pProyectoID);

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
            filaNotificacion.Idioma = pIdioma;

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            switch (pTipoNotificacion)
            {
                case TiposNotificacion.SolicitudRechazada:
                    //listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreRemitente);
                    listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
                    listaParametros.Add((short)ClavesParametro.ProyectoID, pProyectoID.Value.ToString());
                    listaParametros.Add((short)ClavesParametro.OrganizacionID, pOrganizacionID.Value.ToString());
                    listaParametros.Add((short)ClavesParametro.Notas, pMensaje);
                    //listaParametros.Add((short)ClavesParametro.UrlIntragnoss, pURLIntragnoss);
                    break;
            }
            AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

            //Filas de NotificacionPersona
            foreach (string destinatario in pDestinatarios)
            {
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Crea una nueva notificación de invitación de usuario a organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pNombreOrg">Nombre de organización</param>
        /// <param name="pNotas">Notas</param>
        /// <param name="pRemitente">Remitente del mensaje</param>
        /// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <param name="pURLIntragnoss">URL de la intranet de GNOSS</param>
        /// <param name="pGenerarInvitaciones">TRUE si genera invitaciones</param>
        /// <param name="pOrgEsPersonal">TRUE si la organización trabaja en modo personal</param>
        /// <param name="pEnviarCorreo">TRUE si envía correo</param>
        public Dictionary<Guid, Guid> AgregarNotificacionInvitacionUsuarioAOrg(Guid pOrganizacionID, string pNombreOrg, string pNotas, Identidad.Identidad pRemitente, DateTime pFechaComienzo, List<Identidad.Identidad> pListaIdentidades, string pBaseUrl, bool pGenerarInvitaciones, bool pOrgEsPersonal, bool pEnviarCorreo, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            Dictionary<Guid, Guid> listaInvitacionesPorIdentidad = new Dictionary<Guid, Guid>();

            foreach (Identidad.Identidad destinatario in pListaIdentidades)
            {
                //Fila de la notificación
                Notificacion notificacion;

                if (pOrgEsPersonal)
                {
                    notificacion = AgregarNotificacion(TiposNotificacion.InvitacionUsuarioAOrgPers, pFechaComienzo, pFechaComienzo.AddDays(1), pOrganizacionID, null);
                }
                else
                {
                    notificacion = AgregarNotificacion(TiposNotificacion.InvitacionUsuarioAOrgCorp, pFechaComienzo, pFechaComienzo.AddDays(1), pOrganizacionID, null);
                }
                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pNombreOrg);
                //listaParametros.Add((short)ClavesParametro.NombrePersona, destinatario.Nombre);
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                listaParametros.Add((short)ClavesParametro.BaseUrl, pBaseUrl);

                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                //Filas de NotificacionPersona
                if (pGenerarInvitaciones)
                {
                    AD.EntityModel.Models.Notificacion.Invitacion filaInv = new AD.EntityModel.Models.Notificacion.Invitacion();
                    filaInv.NotificacionID = notificacion.FilaNotificacion.NotificacionID;
                    filaInv.InvitacionID = Guid.NewGuid();
                    filaInv.IdentidadDestinoID = destinatario.Clave;
                    filaInv.IdentidadOrigenID = pRemitente.Clave;
                    filaInv.FechaInvitacion = DateTime.Now;
                    filaInv.Estado = (int)EstadoInvitacion.Pendiente;
                    filaInv.TipoInvitacion = notificacion.FilaNotificacion.MensajeID;
                    filaInv.ElementoVinculadoID = pOrganizacionID;
                    NotificacionDW.ListaInvitacion.Add(filaInv);
                    mEntityContext.Invitacion.Add(filaInv);

                    listaInvitacionesPorIdentidad.Add(filaInv.InvitacionID, filaInv.IdentidadDestinoID);
                }
                if (pEnviarCorreo)
                {
                    Notificacion notificacionCorreo;

                    if (pOrgEsPersonal)
                    {
                        notificacionCorreo = AgregarNotificacion(TiposNotificacion.InvitacionCorreoUsuAOrgPers, pFechaComienzo, pFechaComienzo.AddDays(1), pOrganizacionID, null);
                    }
                    else
                    {
                        notificacionCorreo = AgregarNotificacion(TiposNotificacion.InvitacionCorreoUsuAOrgCorp, pFechaComienzo, pFechaComienzo.AddDays(1), pOrganizacionID, null);
                    }
                    Dictionary<short, string> listaParametrosCorreo = new Dictionary<short, string>();
                    listaParametrosCorreo.Add((short)ClavesParametro.NombreOrganizacion, pNombreOrg);
                    //listaParametrosCorreo.Add((short)ClavesParametro.NombrePersona, destinatario.Nombre);
                    listaParametrosCorreo.Add((short)ClavesParametro.BaseUrl, pBaseUrl);
                    listaParametrosCorreo.Add((short)ClavesParametro.Notas, pNotas);

                    AgregarParametrosNotificacion(notificacionCorreo, listaParametrosCorreo, pLanguageCode);

                    AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, destinatario.Email, destinatario.Persona.Clave);
                }
            }

            return listaInvitacionesPorIdentidad;
        }

        /// <summary>
        /// Crea una nueva notificación de invitación de usuario externo a organización
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <param name="pNombreOrg">Nombre de organización</param>
        /// <param name="pNotas">Notas</param>
        /// <param name="pNombreRemitente">Remitente del mensaje</param>
        /// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
        /// <param name="pListaCorreos">Lista de direcciones de correo</param>
        /// <param name="pUrlInvitacion">URL de la invitación</param>
        /// <param name="pOrgEsPersonal">TRUE si la organización trabaja en modo personal</param>
        public void AgregarNotificacionInvitacionExternoAOrg(Guid pOrganizacionID, string pNombreOrg, string pNotas, string pNombreRemitente, DateTime pFechaComienzo, List<string> pListaCorreos, string pUrlInvitacion, bool pOrgEsPersonal, string pLanguageCode)
        {
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion;

                if (pOrgEsPersonal)
                {
                    notificacion = AgregarNotificacion(TiposNotificacion.InvitacionExternoAOrgPers, pFechaComienzo, pFechaComienzo.AddDays(1), pOrganizacionID, null);
                }
                else
                {
                    notificacion = AgregarNotificacion(TiposNotificacion.InvitacionExternoAOrgCorp, pFechaComienzo, pFechaComienzo.AddDays(1), pOrganizacionID, null);
                }
                AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
                AD.EntityModel.Models.Peticion.Peticion filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoOrg(pOrganizacionID).FilaPeticion;

                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlInvitacion + "/" + filaPeticion.PeticionID.ToString());
                listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pNombreOrg);
                //listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.OrganizacionID, pOrganizacionID.ToString());
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);

                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Crea una nueva notificación de invitación de un profesor al correo externo de un alumno para participar en una clase
        /// </summary>
        /// <param name="pFilaOrganizacion">Fila de organización (contiene una fila hija que es la clase)</param>
        /// <param name="pNombreRemitente">Nombre del profesor remitente</param>
        /// <param name="pFechaComienzo">Fecha de comienzo para la notificación</param>
        /// <param name="pListaCorreos">Lista de direcciones de correo de los destinatarios</param>
        /// <param name="pUrlInvitacion">URL para aceptar la invitación</param>
        /// <param name="pNotas">Notas introducidas por el usuario</param>
        public void AgregarNotificacionInvitacionExternoAClase(AD.EntityModel.Models.OrganizacionDS.Organizacion pFilaOrganizacion, string pNombreRemitente, DateTime pFechaComienzo, List<string> pListaCorreos, string pUrlInvitacion, string pNotas, string pLanguageCode)
        {
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }
            string nombreAsignatura = "";


            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.InvitacionExternoAClase, pFechaComienzo, pFechaComienzo.AddDays(1), pFilaOrganizacion.OrganizacionID, null);

                AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
                AD.EntityModel.Models.Peticion.Peticion filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoOrg(pFilaOrganizacion.OrganizacionID).FilaPeticion;

                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlInvitacion + "/" + filaPeticion.PeticionID.ToString());
                listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pFilaOrganizacion.Nombre);
                listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.AliasClase, pFilaOrganizacion.Alias);
                listaParametros.Add((short)ClavesParametro.NombreAsignatura, nombreAsignatura);
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);


                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Crea una nueva notificación de invitación de usuario a comunidad
        /// </summary>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <param name="pOrganizacionID">Identificador de org del proyecto</param>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <param name="pDescripcionProyecto">Descripción de proyecto</param>
        /// <param name="pNotas">Notas</param>
        /// <param name="pRemitente">Remitente del mensaje</param>
        /// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
        /// <param name="pListaIdentidades">Lista de identidades</param>
        /// <param name="pURLIntragnoss">URL de la intranet de GNOSS</param>
        /// <param name="pGenerarInvitaciones">TRUE si genera invitaciones</param>
        /// <param name="pEnviarCorreo">TRUE si envía correo</param>
        /// <param name="pListaGrupos">Lista de grupos de la comunidad a los que dar acceso</param>
        public Dictionary<Guid, Guid> AgregarNotificacionInvitacionUsuarioACom(Es.Riam.Gnoss.Elementos.ServiciosGenerales.Proyecto pProyecto, string pDescripcionProyecto, string pNotas, Identidad.Identidad pRemitente, DateTime pFechaComienzo, List<Identidad.Identidad> pListaIdentidades, string pBaseUrl, bool pGenerarInvitaciones, bool pEnviarCorreo, List<Guid> pListaGrupos, string pLanguageCode)
        {
            Dictionary<Guid, Guid> listaInvitacionesPorIdentidad = new Dictionary<Guid, Guid>();
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            string coma = "";
            string nombresGrupos = "";
            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupo in identidadCN.ObtenerGruposPorIDGrupo(pListaGrupos, false).ListaGrupoIdentidades)
            {
                nombresGrupos += coma + filaGrupo.Nombre;
                coma = ", ";
            }
            identidadCN.Dispose();


            foreach (Identidad.Identidad destinatario in pListaIdentidades)
            {
                //Fila de la notificación
                Notificacion notificacion = null;


                Guid elementoVinculado = pProyecto.Clave;
                if (pListaGrupos.Count > 0)
                {
                    notificacion = AgregarNotificacion(TiposNotificacion.InvitacionUsuarioAComYGrupo, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto.Clave);

                    Peticion peticion = null;

                    peticion = GestorPeticiones.AgregarPeticionDeAccesoGrupoYComUsuario(pListaGrupos, pProyecto.Clave, pProyecto.FilaProyecto.OrganizacionID);
                    elementoVinculado = peticion.Clave;

                }
                else
                {
                    notificacion = AgregarNotificacion(TiposNotificacion.InvitacionUsuarioACom, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto.Clave);
                }

                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.NombreRemitente, pRemitente.Nombre());
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);
                listaParametros.Add((short)ClavesParametro.NombrePersona, destinatario.NombreCorto);
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                listaParametros.Add((short)ClavesParametro.DescripcionProyecto, pDescripcionProyecto);
                listaParametros.Add((short)ClavesParametro.BaseUrl, pBaseUrl);
                if (pListaGrupos.Count > 0)
                {
                    listaParametros.Add((short)ClavesParametro.NombresGrupos, nombresGrupos);
                }

                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                if (pGenerarInvitaciones)
                {
                    AD.EntityModel.Models.Notificacion.Invitacion filaInv = new AD.EntityModel.Models.Notificacion.Invitacion();
                    filaInv.NotificacionID = notificacion.FilaNotificacion.NotificacionID;
                    filaInv.InvitacionID = Guid.NewGuid();
                    filaInv.IdentidadDestinoID = destinatario.Clave;
                    filaInv.IdentidadOrigenID = pRemitente.Clave;
                    filaInv.FechaInvitacion = DateTime.Now;
                    filaInv.ElementoVinculadoID = elementoVinculado;
                    filaInv.Estado = (int)EstadoInvitacion.Pendiente;
                    filaInv.TipoInvitacion = notificacion.FilaNotificacion.MensajeID;
                    NotificacionDW.ListaInvitacion.Add(filaInv);
                    mEntityContext.Invitacion.Add(filaInv);

                    listaInvitacionesPorIdentidad.Add(filaInv.InvitacionID, filaInv.IdentidadDestinoID);
                }

                if (pEnviarCorreo)
                {
                    Notificacion notificacionCorreo = AgregarNotificacion(TiposNotificacion.InvitacionCorreoUsuACom, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto.Clave);

                    Dictionary<short, string> listaParametrosCorreo = new Dictionary<short, string>();
                    listaParametrosCorreo.Add((short)ClavesParametro.NombreRemitente, pRemitente.Nombre(destinatario.Clave));
                    listaParametrosCorreo.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);
                    listaParametrosCorreo.Add((short)ClavesParametro.NombrePersona, destinatario.Nombre(pRemitente.Clave));
                    listaParametrosCorreo.Add((short)ClavesParametro.Notas, pNotas);
                    listaParametrosCorreo.Add((short)ClavesParametro.BaseUrl, pBaseUrl);

                    AgregarParametrosNotificacion(notificacionCorreo, listaParametrosCorreo, pLanguageCode);

                    if (destinatario.EsOrganizacion)
                    {
                        AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, destinatario.Email, null);
                    }
                    else
                    {
                        AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, destinatario.Email, destinatario.Persona.Clave);
                    }
                }
            }

            return listaInvitacionesPorIdentidad;
        }

		public void EnviarCorreoAvisoCambioDeEstado(Guid pTransicion, Guid pProyectoID, string pComentario, string pEnlace, string pTitulo, string pPersonaRealizaCambio)
		{
            FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);            
            Guid estadoOrigen = flujosCN.ObtenerEstadoOrigenTransicion(pTransicion);
            Guid estadoDestino = flujosCN.ObtenerEstadoDestinoTransicion(pTransicion);
            string nombreEstadoOrigen = flujosCN.ObtenerNombreDeEstado(estadoOrigen);
            string nombreEstadoDestino = flujosCN.ObtenerNombreDeEstado(estadoDestino);
            List<Guid> editoresOrigen = flujosCN.ObtenerLectoresYEditoresDeEstado(estadoOrigen);
            List<Guid> editoresDestino = flujosCN.ObtenerLectoresYEditoresDeEstado(estadoDestino);
			PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
            DataWrapperPersona dwPersonaOrigen = personaCN.ObtenerPersonasPorIdentidadesCargaLigera(editoresOrigen);
            DataWrapperPersona dwPersonaDestino = personaCN.ObtenerPersonasPorIdentidadesCargaLigera(editoresDestino);
            List<AD.EntityModel.Models.PersonaDS.Persona> personas = dwPersonaOrigen.ListaPersona.Union(dwPersonaDestino.ListaPersona).ToList();

			foreach (AD.EntityModel.Models.PersonaDS.Persona persona in personas)
            {
				Notificacion notificacionCorreo = AgregarNotificacion(TiposNotificacion.AvisoCambioEstadoRecurso, DateTime.Now, DateTime.Now.AddDays(1), null, pProyectoID);

				Dictionary<short, string> listaParametrosCorreo = new Dictionary<short, string>();
				listaParametrosCorreo.Add((short)ClavesParametro.NombrePersona, pPersonaRealizaCambio);
				listaParametrosCorreo.Add((short)ClavesParametro.Comentario, HttpUtility.UrlDecode(pComentario));
				listaParametrosCorreo.Add((short)ClavesParametro.UrlEnlace, pEnlace);
				listaParametrosCorreo.Add((short)ClavesParametro.NombreEstadoOrigen, nombreEstadoOrigen);
				listaParametrosCorreo.Add((short)ClavesParametro.NombreEstadoDestino, nombreEstadoDestino);
				listaParametrosCorreo.Add((short)ClavesParametro.NombreDoc, pTitulo);

				AgregarParametrosNotificacion(notificacionCorreo, listaParametrosCorreo, persona.Idioma);
				AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, persona.Email, persona.PersonaID);
			}			
		}

		/// <summary>
		/// Crea una nueva notificación de invitación de usuario externo a comunidad
		/// </summary>
		/// <param name="pOrganizacionID">Identificador de la organización</param>
		/// <param name="pProyecto">Identificador de proyecto</param>
		/// <param name="pNombreProyecto">Nombre de proyecto</param>
		/// <param name="pDescripcionProyecto">Descripción de proyecto</param>
		/// <param name="pNotas">Notas</param>
		/// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
		/// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
		/// <param name="pListaCorreos">Lista de direcciones de correo</param>
		/// <param name="pUrlInvitacion">URL de la invitación</param>
		/// <param name="pIdioma">Idioma para la notificación</param>
		public void AgregarNotificacionInvitacionExternoACom(ServiciosGenerales.Proyecto pProyecto, string pDescripcionProyecto, string pNotas, string pNombreRemitente, DateTime pFechaComienzo, List<string> pListaCorreos, string pUrlInvitacion, string pIdioma, List<Guid> pListaGrupos)
        {
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            string coma = "";
            string nombresGrupos = "";
            foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades filaGrupo in identidadCN.ObtenerGruposPorIDGrupo(pListaGrupos, false).ListaGrupoIdentidades)
            {
                nombresGrupos += coma + filaGrupo.Nombre;
                coma = ", ";
            }
            identidadCN.Dispose();

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = null;
                AD.EntityModel.Models.Peticion.Peticion filaPeticion = null;
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();
                if (pListaGrupos.Count > 0)
                {
                    TiposNotificacion tipoNotificacion = TiposNotificacion.InvitacionExternoAComYGrupo;
                    if (pProyecto.TipoProyecto.Equals(TipoProyecto.EducacionExpandida))
                    {
                        tipoNotificacion = TiposNotificacion.InvitacionExternoAClaseYGrupo;
                        listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pProyecto.Nombre);
                    }

                    notificacion = AgregarNotificacion(tipoNotificacion, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto.Clave);
                    filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoComunidadYGrupo(pListaGrupos, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave).FilaPeticion;
                }
                else
                {
                    TiposNotificacion tipoNotificacion = TiposNotificacion.InvitacionExternoACom;
                    if (pProyecto.TipoProyecto.Equals(TipoProyecto.EducacionExpandida))
                    {
                        tipoNotificacion = TiposNotificacion.InvitacionExternoAClase;
                        listaParametros.Add((short)ClavesParametro.NombreOrganizacion, pProyecto.Nombre);
                    }
                    notificacion = AgregarNotificacion(tipoNotificacion, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto.Clave);
                    filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoComunidad(pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave).FilaPeticion;
                }
                AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
                filaNotificacion.Idioma = pIdioma;

                //Filas de NotificacionParametro
                listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlInvitacion + "/" + filaPeticion.PeticionID.ToString());
                listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.ProyectoID, pProyecto.ToString());
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);
                if (pListaGrupos.Count > 0)
                {
                    listaParametros.Add((short)ClavesParametro.NombresGrupos, nombresGrupos);
                }

                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Crea una nueva notificación de invitación de usuario externo a comunidad proveniente de Ning
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <param name="pDescripcionProyecto">Descripción de proyecto</param>
        /// <param name="pNotas">Notas</param>
        /// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
        /// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
        /// <param name="pListaCorreosIdNing">Lista de direcciones de correo e identificadores de Ning</param>
        /// <param name="pUrlInvitacion">URL de la invitación</param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionInvitacionExternoAComProvenienteDeNing(Guid pOrganizacionID, Guid pProyecto, string pNombreProyecto, string pDescripcionProyecto, string pNotas, string pNombreRemitente, DateTime pFechaComienzo, Dictionary<string, string> pListaCorreosIdNing, string pUrlInvitacion, string pIdioma)
        {
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }

            foreach (string destinatario in pListaCorreosIdNing.Keys)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.InvitacionExternoACom, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto);
                AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
                filaNotificacion.Idioma = pIdioma;
                AD.EntityModel.Models.Peticion.Peticion filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoComunidadNing(pOrganizacionID, pProyecto, pListaCorreosIdNing[destinatario]).FilaPeticion;
                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlInvitacion + "/" + filaPeticion.PeticionID.ToString());
                listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.ProyectoID, pProyecto.ToString());
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);

                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Crea una nueva notificación de invitación de usuario externo a unirse a GNOSS y ser contacto
        /// </summary>
        /// <param name="pIdentidadID">Identidad que esta enviando la invitación</param> 
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto al que se quiere dar acceso a un usuario</param>
        /// <param name="pProyectoID">Identificador del proyecto al que se quiere dar acceso a un usuario</param>       
        /// <param name="pNotas">Notas</param>
        /// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
        /// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
        /// <param name="pListaCorreos">Lista de direcciones de correo</param>
        /// <param name="pUrlInvitacion">URL de la invitación</param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionInvitacionExternoAContacto(Guid pIdentidadID, Guid pOrganizacionID, Guid pProyectoID, string pNotas, string pNombreRemitente, DateTime pFechaComienzo, List<string> pListaCorreos, string pUrlInvitacion, string pIdioma)
        {
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.InvitacionPendiente, pFechaComienzo, pFechaComienzo.AddDays(1), null, null);
                AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
                filaNotificacion.Idioma = pIdioma;
                AD.EntityModel.Models.Peticion.Peticion filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoContacto(pIdentidadID, pOrganizacionID, pProyectoID).FilaPeticion;
                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();
                listaParametros.Add((short)ClavesParametro.IdentidadPersona, pIdentidadID.ToString());
                listaParametros.Add((short)ClavesParametro.BaseUrl, pUrlInvitacion + "/" + filaPeticion.PeticionID.ToString());
                listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);


                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }


        /// <summary>
        /// Crea una nueva notificación de invitación de usuario externo a gnoss
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <param name="pNombreProyecto">Nombre de proyecto</param>
        /// <param name="pDescripcionProyecto">Descripción de proyecto</param>
        /// <param name="pNotas">Notas</param>
        /// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
        /// <param name="pFechaComienzo">Fecha de comienzo de la notificación</param>
        /// <param name="pListaCorreos">Lista de direcciones de correo</param>
        /// <param name="pUrlInvitacion">URL de la invitación</param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionInvitacionExternoAGnoss(Guid pOrganizacionID, Guid pProyecto, string pNombreProyecto, string pDescripcionProyecto, string pNotas, string pNombreRemitente, DateTime pFechaComienzo, List<string> pListaCorreos, string pUrlInvitacion, string pIdioma)
        {
            if (GestorPeticiones == null)
            {
                DataWrapperPeticion petDW = new DataWrapperPeticion();
                GestorPeticiones = new GestionPeticiones(petDW, mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionPeticiones>(), mLoggerFactory);
                GestorPeticiones.CargarPeticiones();
            }

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.InvitacionPendiente, pFechaComienzo, pFechaComienzo.AddDays(1), null, pProyecto);
                AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = notificacion.FilaNotificacion;
                filaNotificacion.Idioma = pIdioma;
                AD.EntityModel.Models.Peticion.Peticion filaPeticion = GestorPeticiones.AgregarPeticionDeAccesoComunidad(pOrganizacionID, pProyecto).FilaPeticion;
                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
                listaParametros.Add((short)ClavesParametro.ProyectoID, pProyecto.ToString());
                listaParametros.Add((short)ClavesParametro.OrganizacionID, pOrganizacionID.ToString());
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                listaParametros.Add((short)ClavesParametro.BaseUrl, pUrlInvitacion + "/" + filaPeticion.PeticionID.ToString());

                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        #endregion

        #region Envíos a Twitter

        /// <summary>
        /// Agrega notificaciones para avisar del envío incorrecto de mensajes a Twitter
        /// </summary>
        /// <param name="pTipoNotificacion">Tipo de notificación (debe ser NotificacionEnvioTwitterIncorrecto)</param>
        /// <param name="pListaIdentidades">Lista de identidades a las que va dirigida la notificación</param>
        /// <param name="pNombreProyecto">Nombre del proyecto cuyo canal de Twitter ha dado problemas</param>
        public void AgregarNotificacionEnvioTwitterIncorrecto(TiposNotificacion pTipoNotificacion, List<Identidad.Identidad> pListaIdentidades, string pNombreProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaFin = fechaHoy.AddDays(1);

            foreach (Identidad.Identidad identidadDestinatario in pListaIdentidades)
            {
                Notificacion notificacion = AgregarNotificacion(pTipoNotificacion, fechaHoy, fechaFin, null, null);

                //Agregamos los parámetros a la notificación
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                //Agregamos las filas de NotificacionPersona
                Guid? personaID = null;
                Guid? organizacionID = identidadDestinatario.OrganizacionID;

                if (identidadDestinatario.Persona != null)
                {
                    personaID = identidadDestinatario.Persona.Clave;
                }
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, identidadDestinatario.Email, personaID, organizacionID);
            }
        }

        #endregion

        /// <summary>
        /// Agrega una notificacion de un enlace interno a un correo externo
        /// </summary>
        /// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
        /// <param name="pListaCorreos">Lista de correos</param>
        /// <param name="pNotas">Texto adicional del mensaje</param>
        /// <param name="pUrlEnlace">Url del enlace</param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionReporteRecurso(List<string> pListaCorreos, string pMensaje, string pNombreRemitente, string pUrlRemitente, string pNombreDocumento, string pUrlEnlace, string pIdioma, Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            DateTime fechaHoy = DateTime.Now;

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.NotificacionEnlaceReporteRecurso, fechaHoy, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);
                notificacion.FilaNotificacion.Idioma = pIdioma;
                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.IdentidadRemitente, pUrlRemitente);
                listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlEnlace);
                listaParametros.Add((short)ClavesParametro.Notas, pMensaje);
                listaParametros.Add((short)ClavesParametro.NombreDoc, pNombreDocumento);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);

                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Agrega una notificacion de un enlace interno a un correo externo
        /// </summary>
        /// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
        /// <param name="pListaCorreos">Lista de correos</param>
        /// <param name="pNotas">Texto adicional del mensaje</param>
        /// <param name="pUrlEnlace">Url del enlace</param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionEnlaceExterno(string pNombreRemitente, List<string> pListaCorreos, string pNotas, string pUrlEnlace, string pNombreDocumento, string pIdioma, Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            DateTime fechaHoy = DateTime.Now;

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.NotificacionEnlaceExterno, fechaHoy, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);
                notificacion.FilaNotificacion.Idioma = pIdioma;
                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreRemitente);
                listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlEnlace);
                listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                listaParametros.Add((short)ClavesParametro.NombreDoc, pNombreDocumento);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);

                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Agrega una notificacion de un enlace interno a un correo externo
        /// </summary>
        /// <param name="pNombreRemitente">Nombre del remitente del mensaje</param>
        /// <param name="pListaIdentidades">Lista de identidades correos</param>
        /// <param name="pNotas">Texto adicional del mensaje</param>
        /// <param name="pUrlEnlace">Url del enlace</param>
        /// <param name="pNombreDocumento"></param>
        /// <param name="pIdioma">Idioma para la notificación</param>
        public void AgregarNotificacionEnlace(string pNombreRemitente, List<Identidad.Identidad> pListaIdentidades, string pNotas, string pUrlEnlace, string pNombreDocumento, string pIdioma, Elementos.ServiciosGenerales.Proyecto pProyecto)
        {
            DateTime fechaHoy = DateTime.Now;

            foreach (Identidad.Identidad destinatario in pListaIdentidades)
            {
                Guid? personaid = null;
                Guid? organizacionID = null;
                string email = "";
                if (destinatario.Persona != null && !string.IsNullOrEmpty(destinatario.Persona.Email))
                {
                    personaid = destinatario.Persona.Clave;
                    email = destinatario.Persona.Email;
                }
                else if (destinatario.EsOrganizacion && destinatario.OrganizacionPerfil != null && !string.IsNullOrEmpty(destinatario.OrganizacionPerfil.FilaOrganizacion.Email))
                {
                    organizacionID = destinatario.OrganizacionPerfil.Clave;
                    email = destinatario.OrganizacionPerfil.FilaOrganizacion.Email;
                }

                if (email != "")
                {

                    //Fila de la notificación
                    Notificacion notificacion = AgregarNotificacion(TiposNotificacion.NotificacionEnlaceExterno, fechaHoy, pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);
                    notificacion.FilaNotificacion.Idioma = pIdioma;
                    //Filas de NotificacionParametro
                    Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                    listaParametros.Add((short)ClavesParametro.NombreRemitente, pNombreRemitente);
                    listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlEnlace);
                    listaParametros.Add((short)ClavesParametro.Notas, pNotas);
                    listaParametros.Add((short)ClavesParametro.NombreDoc, pNombreDocumento);
                    listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);

                    AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                    //Filas de NotificacionPersona
                    AgregarCorreoPersonaRow(notificacion.FilaNotificacion, email, personaid, organizacionID);
                }
            }
        }

        /// <summary>
        /// Agrega una notificacion de un enlace interno a un correo externo
        /// </summary>
        /// <param name="pListaCorreos">Lista de correos</param>
        /// <param name="pTexto">Texto del mensaje</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        public void AgregarNotificacionGenerica(List<string> pListaCorreos, string pTexto, string pAsunto, string pLanguageCode, Guid? pOrganizacionID = null, Guid? pProyectoID = null)
        {
            DateTime fechaHoy = DateTime.Now;
            Guid organizacionID = Guid.Empty;
            Guid proyectoID = Guid.Empty;

            if (pOrganizacionID.HasValue)
            {
                organizacionID = pOrganizacionID.Value;
            }
            if (pProyectoID.HasValue)
            {
                proyectoID = pProyectoID.Value;
            }

            foreach (string destinatario in pListaCorreos)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.NotificacionGenerica, fechaHoy, organizacionID, proyectoID);

                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.Texto, pTexto);
                listaParametros.Add((short)ClavesParametro.Asunto, pAsunto);

                AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                //Filas de NotificacionPersona
                AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
            }
        }

        /// <summary>
        /// Agrega una notificacion de un enlace interno a un correo externo
        /// </summary>
        /// <param name="pListaCorreos">Lista de correos</param>
        /// <param name="pTexto">Texto del mensaje</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        public void AgregarNotificacionEnvioMasivo(List<string> pListaCorreos, string pTexto, string pAsunto, Guid pOrganizacionID, Guid pProyectoID, bool pNewsletterCompleta, string pEnlaceBaja)
        {
            AgregarNotificacionEnvioMasivo(pListaCorreos, pTexto, pAsunto, pOrganizacionID, pProyectoID, pNewsletterCompleta, pEnlaceBaja, null);
        }

        public void AgregarNotificacionEnvioMasivo(List<string> pListaCorreos, string pTexto, string pAsunto, Guid pOrganizacionID, Guid pProyectoID, bool pNewsletterCompleta, string pEnlaceBaja, string pIdioma)
        {
            DateTime fechaHoy = DateTime.Now;

            //Añado de 100 correo en 100 correos:
            int numMaxCorreos = mConfigService.ObtenerNumeroPersonasEnvioNewsletter();

            if (numMaxCorreos == -1)
            {
                numMaxCorreos = 50;
            }

            List<string> listaCorreoAgrupados100 = new List<string>();
            int count = 0;
            string correos = "";
            foreach (string destinatario in pListaCorreos)
            {
                correos += destinatario + ",";
                count++;

                if (count >= numMaxCorreos)
                {
                    correos = correos.Substring(0, correos.Length - 1);
                    listaCorreoAgrupados100.Add(correos);
                    correos = "";
                    count = 0;
                }
            }

            if (correos != "") //Quedan menos de 100, hay que agregarlos también.
            {
                correos = correos.Substring(0, correos.Length - 1);
                listaCorreoAgrupados100.Add(correos);
            }

            foreach (string destinatarios in listaCorreoAgrupados100)
            {
                //Fila de la notificación
                Notificacion notificacion = AgregarNotificacion(TiposNotificacion.NotificacionGenerica, fechaHoy, pOrganizacionID, pProyectoID, pIdioma);

                //Filas de NotificacionParametro
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                listaParametros.Add((short)ClavesParametro.Texto, pTexto);
                listaParametros.Add((short)ClavesParametro.Asunto, pAsunto);

                if (pNewsletterCompleta)
                {
                    listaParametros.Add((short)ClavesParametro.NewsletterCompleta, "true");
                }
                else
                {
                    listaParametros.Add((short)ClavesParametro.BajaNewsletter, pEnlaceBaja);
                }

                AgregarParametrosNotificacion(notificacion, listaParametros, pIdioma);

                //Filas de NotificacionEnvioMasivo
                AgregarCorreoEnvioMasivoRow(notificacion.FilaNotificacion, destinatarios);
            }
        }



        /// <summary>
        /// Agrega una notificacion de un enlace interno a un correo externo
        /// </summary>
        /// <param name="pListaCorreos">Lista de correos</param>
        /// <param name="pNombreProyecto">Nombre del proyecto donde se ha agregado una categoría</param>
        /// <param name="pUrlAdministracionCategorias">URL de administración de categorías</param>
        public void AgregarNotificacionCategoriaSugeridaEnComunidad(List<string> pListaCorreos, string pNombreProyecto, string pUrlAdministracionCategorias, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            foreach (string destinatario in pListaCorreos)
            {
                if (!string.IsNullOrEmpty(destinatario))
                {
                    //Fila de la notificación
                    Notificacion notificacion = AgregarNotificacion(TiposNotificacion.CategoriaSugeridaEnComunidad, fechaHoy, Guid.Empty, Guid.Empty);
                    //Filas de NotificacionParametro
                    Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                    listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
                    listaParametros.Add((short)ClavesParametro.UrlEnlace, pUrlAdministracionCategorias);

                    AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

                    //Filas de NotificacionPersona
                    AgregarCorreoPersonaRow(notificacion.FilaNotificacion, destinatario, null);
                }
            }
        }

        /// <summary>
        /// Agrega una notificación de invitación por correo
        /// </summary>
        /// <param name="pIdentidadOrigen">Identidad de quien realiza la invitación</param>
        /// <param name="pIdentidadDestino">Identidad de quien recibe la invitación</param>
        /// <param name="pTipoNotificacion">Tipo de notificacion</param>
        /// <param name="urlIntragnoss">URL de la intranet de GNOSS (No le pasamos el idioma porque lo coge el archivo segun el destinatario)</param>
        public void AgregarNotificacionCorreo(Identidad.Identidad pIdentidadOrigen, Identidad.Identidad pIdentidadDestino, TiposNotificacion pTipoNotificacion, string pBaseUrl, Elementos.ServiciosGenerales.Proyecto pProyecto, string pLanguageCode)
        {
            AgregarNotificacionCorreo(pIdentidadOrigen, pIdentidadDestino, pTipoNotificacion, null, pBaseUrl, pProyecto, pLanguageCode);
        }

        /// <summary>
        /// Agrega una notificación de invitación por correo
        /// </summary>
        /// <param name="pIdentidadOrigen">Identidad de quien realiza la invitación</param>
        /// <param name="pIdentidadDestino">Identidad de quien recibe la invitación</param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pNombreDocumento">Nombre de documento</param>
        /// <param name="urlIntragnoss">URL de la intranet de GNOSS (No le pasamos el idioma porque lo coge el archivo segun el destinatario)</param>
        public void AgregarNotificacionCorreo(Identidad.Identidad pIdentidadOrigen, Identidad.Identidad pIdentidadDestino, TiposNotificacion pTipoNotificacion, string pNombreDocumento, string pBaseUrl, ServiciosGenerales.Proyecto pProyecto, string pLanguageCode, bool pEsEcosistemaSinMetaProyecto = false)
        {
            Guid? personaID = null, organizacionID = null;
            string email = string.Empty;

            if (pIdentidadDestino.Persona != null && !string.IsNullOrEmpty(pIdentidadDestino.Persona.Email))
            {
                personaID = pIdentidadDestino.Persona.Clave;
                email = pIdentidadDestino.Persona.Email;
            }
            else if (pIdentidadDestino.EsOrganizacion && pIdentidadDestino.OrganizacionPerfil != null && !string.IsNullOrEmpty(pIdentidadDestino.OrganizacionPerfil.FilaOrganizacion.Email))
            {
                organizacionID = pIdentidadDestino.OrganizacionPerfil.Clave;
                email = pIdentidadDestino.OrganizacionPerfil.FilaOrganizacion.Email;
            }

            if (!string.IsNullOrEmpty(email))
            {
                DateTime fechaHoy = DateTime.Now;
                string UrlPerfilPersona = string.Empty;
                string UrlPerfilRemitente = string.Empty;

                if (pIdentidadDestino.TrabajaConOrganizacion)
                {
                    UrlPerfilPersona += $"identidad/{pIdentidadDestino.PerfilUsuario.NombreCortoOrg}/";
                }
                else if (pIdentidadDestino.EsIdentidadProfesor)
                {
                    UrlPerfilPersona += $"identidad/{pIdentidadDestino.PerfilUsuario.NombreCortoUsu}/";
                }

                if (pIdentidadOrigen != null)
                {
                    if (pIdentidadOrigen.EsIdentidadProfesor)
                    {
                        UrlPerfilRemitente += $"perfil/{pIdentidadOrigen.PerfilUsuario}/";
                    }

                    if (pIdentidadOrigen.TrabajaConOrganizacion)
                    {
                        UrlPerfilRemitente += $"organizacion/{pIdentidadOrigen.PerfilUsuario.NombreCortoOrg}/";
                    }

                    if (pIdentidadOrigen.ModoPersonal)
                    {
                        UrlPerfilRemitente += $"perfil/{pIdentidadOrigen.PerfilUsuario.NombreCortoUsu}/";
                    }
                }

                Guid? orgID = null;
                Guid? proyectoID = null;

                if (!pEsEcosistemaSinMetaProyecto || pProyecto.FilaProyecto != null)
                {
                    orgID = pProyecto.FilaProyecto.OrganizacionID;
                    proyectoID = pProyecto.FilaProyecto.ProyectoID;
                }

                Notificacion notificacionCorreo = AgregarNotificacion(pTipoNotificacion, fechaHoy, fechaHoy.AddDays(1), orgID, proyectoID);
                Dictionary<short, string> listaParametros = new Dictionary<short, string>();

                if (pIdentidadOrigen != null)
                {
                    listaParametros.Add((short)ClavesParametro.NombreRemitente, pIdentidadOrigen.Nombre(pIdentidadDestino.Clave));
                    listaParametros.Add((short)ClavesParametro.IdentidadRemitente, UrlPerfilRemitente);
                }
                listaParametros.Add((short)ClavesParametro.NombrePersona, pIdentidadDestino.NombreCorto);
                listaParametros.Add((short)ClavesParametro.IdentidadPersona, UrlPerfilPersona);
                listaParametros.Add((short)ClavesParametro.BaseUrl, pBaseUrl);
                listaParametros.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);
                if (pNombreDocumento != null)
                {
                    listaParametros.Add((short)ClavesParametro.NombreDoc, pNombreDocumento);
                }

                AgregarParametrosNotificacion(notificacionCorreo, listaParametros, pLanguageCode);

                AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, email, personaID, organizacionID);
            }
        }

        /// <summary>
        /// Agrega una notificación de aviso
        /// </summary>
        /// <param name="pIdentidadOrigen">Identidad de quien envia el correo</param>
        /// <param name="pIdentidadDestino">Identidad de quien recibe el correo</param>
        /// <param name="pCuerpo">Cuerpo del correo</param>
        /// <param name="pAsunto"></param>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pCorreoID">Identificador del correo</param>
        /// <param name="pURLIntragnoss">pURLIntragnoss</param>
        public void AgregarNotificacionAvisoNuevoCorreo(string pIdentidadOrigenNombre, Identidad.Identidad pIdentidadDestino, string pCuerpo, string pAsunto, TiposNotificacion pTipoNotificacion, Guid pCorreoID, string pBaseUrl, ServiciosGenerales.Proyecto pProyecto, string pLanguageCode, bool esMensajeTutor)
        {
            DateTime fechaHoy = DateTime.Now;

            Notificacion notificacionCorreo = AgregarNotificacion(pTipoNotificacion, fechaHoy, fechaHoy.AddDays(1), pProyecto.FilaProyecto.OrganizacionID, pProyecto.Clave);

            Dictionary<short, string> listaParametrosCorreo = new Dictionary<short, string>();
            listaParametrosCorreo.Add((short)ClavesParametro.NombreRemitente, pIdentidadOrigenNombre);
            listaParametrosCorreo.Add((short)ClavesParametro.NombrePersona, pIdentidadDestino.Nombre());
            listaParametrosCorreo.Add((short)ClavesParametro.Texto, pCuerpo);
            listaParametrosCorreo.Add((short)ClavesParametro.Asunto, pAsunto);
            listaParametrosCorreo.Add((short)ClavesParametro.NombreProyecto, pProyecto.Nombre);
            if (!esMensajeTutor)
            {
                listaParametrosCorreo.Add((short)ClavesParametro.CorreoID, pCorreoID.ToString());
                listaParametrosCorreo.Add((short)ClavesParametro.BaseUrl, pBaseUrl);
            }
            AgregarParametrosNotificacion(notificacionCorreo, listaParametrosCorreo, pLanguageCode);

            string emailTutor = "";
            if (!string.IsNullOrEmpty(pIdentidadDestino.Persona.FilaPersona.EmailTutor))
            {
                emailTutor = pIdentidadDestino.Persona.FilaPersona.EmailTutor;
            }

            if (!(esMensajeTutor && string.IsNullOrEmpty(emailTutor)))
            {
                if (pIdentidadDestino.EsOrganizacion)
                {
                    if (esMensajeTutor)
                    {
                        AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, emailTutor, pIdentidadDestino.OrganizacionID);
                    }
                    else
                    {
                        AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, pIdentidadDestino.Email, pIdentidadDestino.OrganizacionID);
                    }

                }
                else
                {
                    if (pIdentidadDestino.TrabajaConOrganizacion)
                    {
                        if (esMensajeTutor)
                        {
                            AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, emailTutor, pIdentidadDestino.PersonaID, pIdentidadDestino.OrganizacionID);
                        }
                        else
                        {
                            AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, pIdentidadDestino.Email, pIdentidadDestino.PersonaID, pIdentidadDestino.OrganizacionID);
                        }
                    }
                    else
                    {
                        if (esMensajeTutor)
                        {
                            AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, emailTutor, pIdentidadDestino.PersonaID);
                        }
                        else
                        {
                            AgregarCorreoPersonaRow(notificacionCorreo.FilaNotificacion, pIdentidadDestino.Email, pIdentidadDestino.PersonaID);
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Agrega una notificación de comentario a un documento
        /// </summary>
        /// <param name="pDocumento">Documento al que se realiza el comentario</param>
        /// <param name="pIdentidad">Identidad de quien escribe el comentario</param>
        /// <param name="urlIntragnoss">URL de la intranet de GNOSS</param>
        /// <param name="pEnlace"></param>
        public void AgregarNotificacionComentarioDocumento(Documento pDocumento, Identidad.Identidad pIdentidad, string urlIntragnoss, string pEnlace, Elementos.ServiciosGenerales.Proyecto pProyecto, string pLanguageCode, bool pEsEcosistemaSinMetaProyecto = false)
        {
            foreach (EditorRecurso Editor in pDocumento.ListaPerfilesEditores.Values)
            {
                if (Editor.FilaEditor.Editor)
                {
                    Identidad.Identidad identidad = null;
                    try
                    {
                        identidad = Editor.ObtenerIdentidadEditorEnProyecto(pIdentidad.FilaIdentidad.ProyectoID);

                        //Obtenemos la configuración del creador del recurso
                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                        Logica.ServiciosGenerales.PersonaCN personaCN = new Logica.ServiciosGenerales.PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                        DataWrapperIdentidad dataWrapper = identidadCN.ObtenerIdentidadesPorID(new List<Guid> { pDocumento.CreadorID }, false);
                        Guid perfilIDCreador = dataWrapper.ListaPerfil.First().PerfilID;
                        Guid personaIDCreadorRecurso = personaCN.ObtenerPersonaIDPorPerfil(perfilIDCreador);
                        AD.EntityModel.Models.PersonaDS.ConfiguracionGnossPersona configuracionGnossPersona = personaCN.ObtenerConfiguracionPersonaPorID(personaIDCreadorRecurso);

                        if (identidad != null && configuracionGnossPersona.ComentariosRecursos)
                        {
                            //Si el editor es el creador del comentario no hay que mandar la notificacion
                            if (identidad.PerfilID != pIdentidad.PerfilID)
                            {
                                AgregarNotificacionCorreo(pIdentidad, identidad, TiposNotificacion.ComentarioDocumentoCorreo, pDocumento.Titulo, urlIntragnoss, pProyecto, pLanguageCode, pEsEcosistemaSinMetaProyecto);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        mLoggingService.GuardarLogError(ex, $"No se pudo enviar la notificación del documento {pDocumento.Clave} al editor {identidad?.Clave}",mlogger);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el texto de un parámetro de notificación a partir de su clave
        /// </summary>
        /// <param name="pClave">Clave de parámetro de notificación</param>
        /// <returns>Texto de parámetro de notificación</returns>
        public static string TextoDeParametro(short pClave)
        {
            if (pClave < 20000)
            {
                return "<#" + ((ClavesParametro)Enum.Parse(typeof(ClavesParametro), pClave.ToString())).ToString() + "#>";
            }
            else
            {
                return "<@" + ((ClavesParametro)Enum.Parse(typeof(ClavesParametro), pClave.ToString())).ToString() + "@>";
            }
        }

        /// <summary>
        /// Carga la lista de los mensajes obteniendolos del xml
        /// </summary>
        public Dictionary<int, KeyValuePair<string, string>> CargarListaMensajes(string pIdioma)
        {
            if (mListaMensajes == null)
            {
                mListaMensajes = new Dictionary<string, Dictionary<int, KeyValuePair<string, string>>>();

                XmlDocument docDefecto = ObtenerMensajesDeIdioma("es", false);

                mListaMensajesDefecto = new Dictionary<int, KeyValuePair<string, string>>();
                XmlElement mensajesDefecto = docDefecto["correos"]["mensajes"];
                foreach (XmlNode paramNode in mensajesDefecto.ChildNodes)
                {
                    mListaMensajesDefecto.Add(int.Parse(paramNode.Attributes["id"].Value), new KeyValuePair<string, string>(paramNode["asunto"].InnerXml.Replace("{", "<").Replace("}", ">"), paramNode["texto"].InnerXml.Replace("{", "<").Replace("}", ">")));
                }
            }
            if (!mListaMensajes.ContainsKey(pIdioma))
            {
                mListaMensajes.Add(pIdioma, new Dictionary<int, KeyValuePair<string, string>>());
                XmlDocument doc = ObtenerMensajesDeIdioma(pIdioma, false);

                if (doc == null)
                {
                    doc = ObtenerMensajesDeIdioma("es", false);
                }

                XmlElement mensajes = doc["correos"]["mensajes"];

                foreach (XmlNode paramNode in mensajes.ChildNodes)
                {
                    if (paramNode["asunto"] != null && paramNode["texto"] != null)
                    {
                        mListaMensajes[pIdioma].Add(int.Parse(paramNode.Attributes["id"].Value), new KeyValuePair<string, string>(paramNode["asunto"].InnerXml.Replace("{", "<").Replace("}", ">"), paramNode["texto"].InnerXml.Replace("{", "<").Replace("}", ">")));
                    }
                    else
                    {
                        mListaMensajes[pIdioma].Add(int.Parse(paramNode.Attributes["id"].Value), new KeyValuePair<string, string>(mListaMensajesDefecto[int.Parse(paramNode.Attributes["id"].Value)].Key, mListaMensajesDefecto[int.Parse(paramNode.Attributes["id"].Value)].Value));
                    }
                }
            }
            return mListaMensajes[pIdioma];
        }

        /// <summary>
        /// Carga la lista de los formatos de correo (cabecera, pie) obteniendolos del xml
        /// </summary>
        public Dictionary<string, string> CargarListaFormatosCorreo(string pIdioma)
        {
            if (mListaFormatosCorreo == null)
            {
                mListaFormatosCorreo = new Dictionary<string, Dictionary<string, string>>();
            }
            if (!mListaFormatosCorreo.ContainsKey(pIdioma))
            {
                XmlDocument docDefecto = ObtenerMensajesDeIdioma("es");

                mListaFormatosCorreo.Add(pIdioma, new Dictionary<string, string>());
                XmlDocument doc = ObtenerMensajesDeIdioma(pIdioma);

                if (doc == null)
                {
                    // No hay fichero para este idioma, le asigno el de por defecto
                    doc = docDefecto;
                }

                XmlElement mensajes = doc["correos"]["formatoCorreo"];

                foreach (XmlNode paramNode in mensajes.ChildNodes)
                {
                    if (string.IsNullOrEmpty(paramNode.InnerXml))
                    {
                        mListaFormatosCorreo[pIdioma].Add(paramNode.Name, docDefecto["correos"]["formatoCorreo"][paramNode.Name].InnerXml.Replace('{', '<').Replace('}', '>'));
                    }
                    else
                    {
                        mListaFormatosCorreo[pIdioma].Add(paramNode.Name, paramNode.InnerXml.Replace('{', '<').Replace('}', '>'));
                    }
                }
            }

            return mListaFormatosCorreo[pIdioma];
        }

        private XmlDocument ObtenerMensajesDeIdioma(string pIdioma, bool pMezclarConPersonalizado = true)
        {
            XmlDocument doc = null;

            string ficheroIdioma = ObtenerContenidoTextosMensajes(pIdioma);

            if (!string.IsNullOrEmpty(ficheroIdioma))
            {
                doc = new XmlDocument();
                doc.Load(new StringReader(ficheroIdioma));

                string ficheroIdiomaPersonalizado = ObtenerContenidoTextosMensajesPersonalizados(pIdioma);

                if (pMezclarConPersonalizado && !string.IsNullOrEmpty(ficheroIdiomaPersonalizado))
                {
                    XmlDocument xmlPersonalizado = new XmlDocument();
                    xmlPersonalizado.Load(new StringReader(ficheroIdiomaPersonalizado));
                    MezclaDocumentosXML(doc, xmlPersonalizado);
                }
            }

            return doc;
        }

        public static string ObtenerContenidoTextosMensajes(string pIdioma)
        {
            ResourceManager rm = new ResourceManager("Es.Riam.Gnoss.Elementos.Properties.Resources", Assembly.GetExecutingAssembly());

            string nombreFichero = "Mensajes_" + pIdioma;
            string ficheroIdioma = rm.GetString(nombreFichero);

            if (string.IsNullOrEmpty(ficheroIdioma) && !string.IsNullOrEmpty(pIdioma) && pIdioma.Length > 2)
            {
                pIdioma = pIdioma.Substring(0, 2);
                ficheroIdioma = rm.GetString(nombreFichero);
            }
            return ficheroIdioma;
        }
        public static string ObtenerContenidoTextosMensajesPersonalizados(string pIdioma)
        {
            ResourceManager rm = new ResourceManager("Es.Riam.Gnoss.Elementos.Properties.Resources", Assembly.GetExecutingAssembly());

            string nombreFichero = "Mensajes_" + pIdioma + "_personalizado";

            string ficheroIdioma = rm.GetString(nombreFichero);

            if (string.IsNullOrEmpty(ficheroIdioma) && !string.IsNullOrEmpty(pIdioma) && pIdioma.Length > 2)
            {
                pIdioma = pIdioma.Substring(0, 2);
                ficheroIdioma = rm.GetString(nombreFichero);
            }
            return ficheroIdioma;
        }

        #region Notificaciones de comunidad

        /// <summary>
        /// Agrega una notificación para avisar a un miembro de una comunidad de su expulsión de la misma
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización creadora del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDestinatario">Email del destinatario</param>
        /// <param name="pPersonaID">Identificador de la persona a la que enviamos la notificación</param>
        /// <param name="pFechaGeneracionDeLaNotificacion">Fecha de cuando la notificación se hará efectiva</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        /// <param name="pMotivoExpulsion">Descripción del motivo de la expulsión</param>
        public void AgregarNotificacionExpulsionUsuarioDeComunidad(Guid pOrganizacionID, Guid pProyectoID, string pDestinatario, Guid pPersonaID, DateTime pFechaGeneracionDeLaNotificacion, string pNombreDestinatario, string pNombreProyecto, string pMotivoExpulsion, string pLanguageCode)
        {
            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.AvisoExpulsionUsuarioDeComunidad, pFechaGeneracionDeLaNotificacion, pFechaGeneracionDeLaNotificacion.AddDays(1), pOrganizacionID, pProyectoID);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);
            listaParametros.Add((short)ClavesParametro.Notas, pMotivoExpulsion);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDestinatario, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificación para avisar a un miembro de una comunidad del cierre temporal de la misma
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion creadora del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDestinatario">Email del destinatario</param>
        /// <param name="pMotivo">Motivo de cierre de la comunidad</param>
        /// <param name="pPersonaID">Identificador de la persona a la que enviamos la notificación</param>
        /// <param name="pFecha">Fecha de reapertura de la comunidad</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        public void AgregarNotificacionCierreComunidadTemporal(Guid pOrganizacionID, Guid pProyectoID, string pDestinatario, string pMotivo, Guid pPersonaID, string pFecha, string pNombreDestinatario, string pNombreProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.AvisoCierreTemp, fechaHoy, fechaHoy.AddDays(1), pOrganizacionID, pProyectoID);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);
            listaParametros.Add((short)ClavesParametro.Texto, pMotivo);

            string[] fecha = pFecha.Split('/');
            string fecha_dia = fecha[0];
            string fecha_mes = fecha[1];
            string fecha_anio = fecha[2];
            listaParametros.Add((short)ClavesParametro.Fecha_dia, fecha_dia);
            listaParametros.Add((short)ClavesParametro.Fecha_mes, fecha_mes);
            listaParametros.Add((short)ClavesParametro.Fecha_anio, fecha_anio);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDestinatario, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificacion para enviarsela a un administrador de una comunidad para avisarle de que tiene que reabrirla, tras cumplirse el plazo del cierre temporal de la misma
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion creadora del proyecto</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pDestinatario">e-mail del destinatario</param>
        /// <param name="pPersonaID">Clave de la persona a lña que enviamos la notificacion</param>
        /// <param name="pFechaGeneracionDeLaNotificacion">Fecha de cuando se mandara el aviso al adminstrador</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        public void AgregarNotificacionAdministradorComunidadParaReabrir(Guid pOrganizacionID, Guid pProyectoID, string pDestinatario, Guid pPersonaID, DateTime pFechaGeneracionDeLaNotificacion, string pNombreDestinatario, string pNombreProyecto, string pLanguageCode)
        {
            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.AvisoReaperturaComunidad, pFechaGeneracionDeLaNotificacion, pFechaGeneracionDeLaNotificacion.AddDays(1), pOrganizacionID, pProyectoID);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDestinatario, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificacion para enviarsela a un miembro de una comunidad para avisarle del cierre definitivo de la misma
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion creadora del proyecto</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pDestinatario">e-mail del destinatario</param>
        /// <param name="pPersonaID">Clave de la persona a lña que enviamos la notificacion</param>
        /// <param name="pFechaFin">Fecha final de cierre de la comunidad (fecha en la q el administrador decidio cerrarla + Dias de gracia)</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        public void AgregarNotificacionCierreComunidadDefinitivo(Guid pOrganizacionID, Guid pProyectoID, string pDestinatario, string pFechaFin, Guid pPersonaID, string pNombreDestinatario, string pNombreProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.AvisoCierreDefinitivo, fechaHoy, fechaHoy.AddDays(1), pOrganizacionID, pProyectoID);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);

            string[] fecha = pFechaFin.Split('/');
            string fecha_dia = fecha[0];
            string fecha_mes = fecha[1];
            string fecha_anio = fecha[2];
            listaParametros.Add((short)ClavesParametro.Fecha_dia, fecha_dia);
            listaParametros.Add((short)ClavesParametro.Fecha_mes, fecha_mes);
            listaParametros.Add((short)ClavesParametro.Fecha_anio, fecha_anio);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDestinatario, pPersonaID);
        }

        /// <summary>
        /// Agrega una notificacion para enviarsela a un administrador de una comunidad para avisarle que tiene que reabrir la comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Clave de la organizacion creadora del proyecto</param>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pDestinatario">e-mail del destinatario</param>
        /// <param name="pPersonaID">Clave de la persona a lña que enviamos la notificacion</param>
        /// <param name="pNombreDestinatario">Nombre del destinatario</param>
        /// <param name="pNombreProyecto">Nombre del proyecto</param>
        public void AgregarNotificacionAperturaComunidad(Guid pOrganizacionID, Guid pProyectoID, string pDestinatario, Guid pPersonaID, string pNombreDestinatario, string pNombreProyecto, string pLanguageCode)
        {
            DateTime fechaHoy = DateTime.Now;

            //Fila de la notificación
            Notificacion notificacion = AgregarNotificacion(TiposNotificacion.AvisoAperturaComunidad, fechaHoy, fechaHoy.AddDays(1), pOrganizacionID, pProyectoID);

            //Filas de NotificacionParametro
            Dictionary<short, string> listaParametros = new Dictionary<short, string>();

            listaParametros.Add((short)ClavesParametro.NombrePersona, pNombreDestinatario);
            listaParametros.Add((short)ClavesParametro.NombreProyecto, pNombreProyecto);

            AgregarParametrosNotificacion(notificacion, listaParametros, pLanguageCode);

            //Filas de NotificacionPersona
            AgregarCorreoPersonaRow(notificacion.FilaNotificacion, pDestinatario, pPersonaID);
        }

        #endregion

        /// <summary>
        /// Elimina las notificaciones a correos de personas pasadas en la lista
        /// </summary>
        /// <param name="pListaNotificaciones">Lista con los identificadores de notificación que queremos eliminar</param>
        public void EliminarNotificacionACorreosPersonas(List<Guid> pListaNotificaciones)
        {
            foreach (Guid ClaveNotificacion in pListaNotificaciones)
            {
                List<short> listaParametros = new List<short>();

                //Filas de NotificacionParametro
                foreach (AD.EntityModel.Models.Notificacion.NotificacionParametro filaNotificacionParametro in this.NotificacionDW.ListaNotificacionParametro.Where(item => item.NotificacionID.Equals(ClaveNotificacion)))
                {
                    listaParametros.Add(filaNotificacionParametro.ParametroID);
                }

                if (listaParametros.Count > 0)
                {
                    foreach (short ClaveParametro in listaParametros)
                    {
                        AD.EntityModel.Models.Notificacion.NotificacionParametro notificacionParametro = this.NotificacionDW.ListaNotificacionParametro.Where(item => item.NotificacionID.Equals(ClaveNotificacion) && item.ParametroID.Equals(ClaveParametro)).FirstOrDefault();

                        this.NotificacionDW.ListaNotificacionParametro.Remove(notificacionParametro);
                        mEntityContext.EliminarElemento(notificacionParametro);

                    }
                }
                //Filas de NotificacionCorreoPersona
                if (this.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(ClaveNotificacion)).Count() > 0)
                {
                    AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona notifCorrPers = this.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(ClaveNotificacion)).FirstOrDefault();

                    this.NotificacionDW.ListaNotificacionCorreoPersona.Remove(notifCorrPers);
                    mEntityContext.EliminarElemento(notifCorrPers);

                }
                //Fila de la notificación
                if (this.NotificacionDW.ListaNotificacion.Any(item => item.NotificacionID.Equals(ClaveNotificacion)))
                {
                    AD.EntityModel.Models.Notificacion.Notificacion notificacion = this.NotificacionDW.ListaNotificacion.Where(item => item.NotificacionID.Equals(ClaveNotificacion)).FirstOrDefault();

                    this.NotificacionDW.ListaNotificacion.Remove(notificacion);
                    mEntityContext.EliminarElemento(notificacion);
                }
            }
        }        

        #region Métodos de comparaciones

        /// <summary>
        /// Compara dos invitaciones por fecha
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorFecha(Invitacion.Invitacion pInvitacionX, Invitacion.Invitacion pInvitacionY)
        {
            return pInvitacionY.FilaInvitacion.FechaInvitacion.CompareTo(pInvitacionX.FilaInvitacion.FechaInvitacion);
        }

        /// <summary>
        /// Compara dos invitaciones por fecha de forma descendente
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorFechaDesc(Invitacion.Invitacion pInvitacionX, Invitacion.Invitacion pInvitacionY)
        {
            return pInvitacionX.FilaInvitacion.FechaInvitacion.CompareTo(pInvitacionY.FilaInvitacion.FechaInvitacion);
        }

        /// <summary>
        /// Compara dos invitaciones por autor
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorAutor(Invitacion.Invitacion pInvitacionX, Invitacion.Invitacion pInvitacionY)
        {
            return pInvitacionX.IdentidadOrigen.Nombre().CompareTo(pInvitacionY.IdentidadOrigen.Nombre());
        }

        /// <summary>
        /// Compara dos invitaciones por autor de forma descendente
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorAutorDesc(Invitacion.Invitacion pInvitacionX, Invitacion.Invitacion pInvitacionY)
        {
            return pInvitacionY.IdentidadOrigen.Nombre().CompareTo(pInvitacionX.IdentidadOrigen.Nombre());
        }

        /// <summary>
        /// Compara dos invitaciones por fecha
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorFecha(ElementoGnoss pInvitacionX, ElementoGnoss pInvitacionY)
        {
            if (pInvitacionX is Invitacion.Invitacion && pInvitacionY is Invitacion.Invitacion)
            {
                return GestionNotificaciones.CompararInvitacionesPorFecha((Invitacion.Invitacion)pInvitacionX, (Invitacion.Invitacion)pInvitacionY);
            }
            else
            {
                return pInvitacionX.Nombre.CompareTo(pInvitacionY.Nombre);
            }
        }

        /// <summary>
        /// Compara dos invitaciones por fecha de forma descendente
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorFechaDesc(ElementoGnoss pInvitacionX, ElementoGnoss pInvitacionY)
        {
            if (pInvitacionX is Invitacion.Invitacion && pInvitacionY is Invitacion.Invitacion)
            {
                return GestionNotificaciones.CompararInvitacionesPorFechaDesc((Invitacion.Invitacion)pInvitacionX, (Invitacion.Invitacion)pInvitacionY);
            }
            else
            {
                return pInvitacionX.Nombre.CompareTo(pInvitacionY.Nombre);
            }
        }

        /// <summary>
        /// Compara dos invitaciones por autor
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorAutor(ElementoGnoss pInvitacionX, ElementoGnoss pInvitacionY)
        {
            if (pInvitacionX is Invitacion.Invitacion && pInvitacionY is Invitacion.Invitacion)
            {
                return GestionNotificaciones.CompararInvitacionesPorAutor((Invitacion.Invitacion)pInvitacionX, (Invitacion.Invitacion)pInvitacionY);
            }
            else
            {
                return pInvitacionX.Nombre.CompareTo(pInvitacionY.Nombre);
            }
        }

        /// <summary>
        /// Compara dos invitaciones por autor de forma descendente
        /// </summary>
        /// <param name="pInvitacionX">Invitación x</param>
        /// <param name="pInvitacionY">Invitación y</param>
        /// <returns></returns>
        public static int CompararInvitacionesPorAutorDesc(ElementoGnoss pInvitacionX, ElementoGnoss pInvitacionY)
        {
            if (pInvitacionX is Invitacion.Invitacion && pInvitacionY is Invitacion.Invitacion)
            {
                return GestionNotificaciones.CompararInvitacionesPorAutorDesc((Invitacion.Invitacion)pInvitacionX, (Invitacion.Invitacion)pInvitacionY);
            }
            else
            {
                return pInvitacionX.Nombre.CompareTo(pInvitacionY.Nombre);
            }
        }

        #endregion

        #endregion

        #region Privados


        private void MezclaDocumentosXML(XmlDocument pXmlDefecto, XmlDocument pXmlPersonalizado)
        {
            XmlNodeList mensajes = pXmlPersonalizado.SelectNodes("/correos/mensajes/mensaje");
            foreach (XmlNode nodo in mensajes)
            {
                pXmlDefecto.DocumentElement["mensajes"].InnerXml += nodo.OuterXml;
            }
        }

        /// <summary>
        /// Agrega una lista de parámetros para una notificación
        /// </summary>
        /// <param name="pNotificacion">Notificación</param>
        /// <param name="pListaParametros">Lista de parámetros</param>
        private void AgregarParametrosNotificacion(Notificacion pNotificacion, Dictionary<short, string> pListaParametros, string pLanguageCode)
        {
            foreach (short clave in pListaParametros.Keys)
            {
                pNotificacion.ListaParametrosParametrizables.Add(TextoDeParametro(clave), pListaParametros[clave]);

                AD.EntityModel.Models.Notificacion.NotificacionParametro filaParametro = new AD.EntityModel.Models.Notificacion.NotificacionParametro();

                filaParametro.NotificacionID = pNotificacion.FilaNotificacion.NotificacionID;
                filaParametro.ParametroID = clave;
                if (ParametrosGenerales == null)
                {
                    ParametrosGenerales = mEntityContext.ParametroGeneral.FirstOrDefault();
                }
                string valor = UtilCadenas.ObtenerTextoDeIdioma(pListaParametros[clave], pLanguageCode, ParametrosGenerales.IdiomaDefecto);

                filaParametro.Valor = valor;

                NotificacionDW.ListaNotificacionParametro.Add(filaParametro);
                mEntityContext.NotificacionParametro.Add(filaParametro);
            }
        }

        /// <summary>
        /// Agrega una lista de parámetros para una notificación a la persona indicada
        /// </summary>
        /// <param name="pNotificacion">Notificación</param>
        /// <param name="pPersona">Persona</param>
        /// <param name="pListaParametros">Lista de parámetros</param>
        private void AgregarParametrosPersonaNotificacion(Notificacion pNotificacion, Persona pPersona, Dictionary<short, string> pListaParametros)
        {
            foreach (short clave in pListaParametros.Keys)
            {
                pNotificacion.ListaParametrosPersonaParametrizables.Add(TextoDeParametro(clave), pListaParametros[clave]);

                AD.EntityModel.Models.Notificacion.NotificacionParametroPersona filaParametroPersona = new AD.EntityModel.Models.Notificacion.NotificacionParametroPersona();

                filaParametroPersona.NotificacionID = pNotificacion.FilaNotificacion.NotificacionID;
                filaParametroPersona.ParametroID = clave;
                filaParametroPersona.Valor = pListaParametros[clave];
                filaParametroPersona.PersonaID = pPersona.Clave;

                NotificacionDW.ListaNotificacionParametroPersona.Add(filaParametroPersona);
                mEntityContext.NotificacionParametroPersona.Add(filaParametroPersona);
            }
        }

        /// <summary>
        /// Agrega una fila de NotificacionAlertaPersona al dataset
        /// </summary>
        /// <param name="pNotificacionRow">Notificacion</param>
        /// <param name="pPersona">Persona que tiene la notificación</param>
        /// <returns></returns>
        private AD.EntityModel.Models.Notificacion.NotificacionAlertaPersona AgregarAlertaPersonaRow(AD.EntityModel.Models.Notificacion.Notificacion pNotificacionRow, Persona pPersona)
        {
            if (NotificacionDW.ListaNotificacionAlertaPersona.Where(item => item.NotificacionID.Equals(pNotificacionRow.NotificacionID) && item.PersonaID.Equals(pPersona.Clave)).Count() == 0)
            {
                AD.EntityModel.Models.Notificacion.NotificacionAlertaPersona filaPersona = new AD.EntityModel.Models.Notificacion.NotificacionAlertaPersona();
                filaPersona.PersonaID = pPersona.Clave;
                filaPersona.NotificacionID = pNotificacionRow.NotificacionID;
                filaPersona.FechaLectura = null;

                NotificacionDW.ListaNotificacionAlertaPersona.Add(filaPersona);
                mEntityContext.NotificacionAlertaPersona.Add(filaPersona);

                return filaPersona;
            }
            return null;
        }

        /// <summary>
        /// Agrega una fila de NotificacionCorreoPersona al dataset
        /// </summary>
        /// <param name="pNotificacionRow">Fila de motificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo de la persona a la que se le notifica</param>
        /// <param name="pPersonaNotificacionID">Identificador de la persona que tiene la notificación (NULL si la persona todavía no existe en la base de datos)</param>
        /// <returns></returns>
        private void AgregarCorreoPersonaRow(AD.EntityModel.Models.Notificacion.Notificacion pNotificacionRow, string pCorreoPersonaNotificacion, Guid? pPersonaNotificacionID, Persona pPersona = null)
        {
            if (this.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(pNotificacionRow.NotificacionID) && item.EmailEnvio.Equals(pCorreoPersonaNotificacion)).Count() == 0)
            {
                AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona filaPersona = new AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona();
                string emailDestinatario = pCorreoPersonaNotificacion;
                if (string.IsNullOrEmpty(pCorreoPersonaNotificacion) && pPersona != null && !string.IsNullOrEmpty(pPersona.FilaPersona.EmailTutor))
                {
                    pCorreoPersonaNotificacion = pPersona.FilaPersona.EmailTutor;
                }


                if (pPersonaNotificacionID.HasValue)
                {
                    filaPersona.PersonaID = pPersonaNotificacionID.Value;
                }
                filaPersona.NotificacionID = pNotificacionRow.NotificacionID;
                filaPersona.EstadoEnvio = (short)EstadoEnvio.Pendiente;
                filaPersona.FechaEnvio = null;
                filaPersona.EmailEnvio = pCorreoPersonaNotificacion;

                NotificacionDW.ListaNotificacionCorreoPersona.Add(filaPersona);
                mEntityContext.NotificacionCorreoPersona.Add(filaPersona);
            }
        }

        /// <summary>
        /// Agrega una fila de NotificacionCorreoPersona al dataset
        /// </summary>
        /// <param name="pNotificacionRow">Fila de motificación</param>
        /// <param name="pCorreoPersonaNotificacion">Correo de la persona a la que se le notifica</param>
        /// <param name="pPersonaNotificacionID">Identificador de la persona que tiene la notificación (NULL si la persona todavía no existe en la base de datos)</param>
        /// <param name="pOrganizacionPersonaID">Identificador de la organización que tiene la notificación (NULL si la persona todavía no existe en la base de datos)</param>
        private void AgregarCorreoPersonaRow(AD.EntityModel.Models.Notificacion.Notificacion pNotificacionRow, string pCorreoPersonaNotificacion, Guid? pPersonaNotificacionID, Guid? pOrganizacionPersonaID)
        {
            if ((this.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(pNotificacionRow.NotificacionID)).Count() == 0) || (this.NotificacionDW.ListaNotificacionCorreoPersona.Where(item => item.NotificacionID.Equals(pNotificacionRow.NotificacionID) && item.EmailEnvio.Equals(pCorreoPersonaNotificacion)).Count()) == 0)
            {
                AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona filaPersona = new AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona();

                if (pPersonaNotificacionID.HasValue)
                {
                    filaPersona.PersonaID = pPersonaNotificacionID.Value;
                }
                else
                {
                    filaPersona.PersonaID = null;
                }

                if (pOrganizacionPersonaID.HasValue)
                {
                    filaPersona.OrganizacionPersonaID = pOrganizacionPersonaID.Value;
                }
                else
                {
                    filaPersona.OrganizacionPersonaID = null;
                }
                filaPersona.NotificacionID = pNotificacionRow.NotificacionID;
                filaPersona.EstadoEnvio = (short)EstadoEnvio.Pendiente;
                filaPersona.FechaEnvio = null;
                filaPersona.EmailEnvio = pCorreoPersonaNotificacion;

                NotificacionDW.ListaNotificacionCorreoPersona.Add(filaPersona);
                mEntityContext.NotificacionCorreoPersona.Add(filaPersona);
            }
        }

        /// <summary>
        /// Agrega un DataRow de NotificacionCorreoPersona al dataSet
        /// </summary>
        /// <param name="pNotificacionRow">Notificacion</param>
        /// <param name="pCorreoPersonaNotificacion">Correo de la persona a la que se le notifica</param>
        /// <param name="pPersonaNotificacionID">Clave de la persona a la que se notifica</param>
        private void AgregarCorreoPersonaRow(AD.EntityModel.Models.Notificacion.Notificacion pNotificacionRow, string pCorreoPersonaNotificacion, Guid pPersonaNotificacionID)
        {
            if (!NotificacionDW.ListaNotificacionCorreoPersona.Any(item => item.NotificacionID.Equals(pNotificacionRow.NotificacionID) && item.EmailEnvio.Equals(pCorreoPersonaNotificacion)))
            {
                AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona filaPersona = new AD.EntityModel.Models.Notificacion.NotificacionCorreoPersona();

                filaPersona.PersonaID = pPersonaNotificacionID;
                filaPersona.NotificacionID = pNotificacionRow.NotificacionID;
                filaPersona.EstadoEnvio = (short)EstadoEnvio.Pendiente;
                filaPersona.FechaEnvio = null;
                filaPersona.EmailEnvio = pCorreoPersonaNotificacion;

                if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN))
                {
                    filaPersona.EnviadoRabbit = true;
                }

                NotificacionDW.ListaNotificacionCorreoPersona.Add(filaPersona);
                mEntityContext.NotificacionCorreoPersona.Add(filaPersona);
            }
        }

        /// <summary>
        /// Agrega un DataRow de NotificacionCorreoPersona al dataSet
        /// </summary>
        /// <param name="pNotificacionRow">Notificacion</param>
        /// <param name="pCorreosNotificacion">Correso de las personas a la que se les notifica</param>
        private void AgregarCorreoEnvioMasivoRow(AD.EntityModel.Models.Notificacion.Notificacion pNotificacionRow, string pCorreosNotificacion)
        {
            if (this.NotificacionDW.ListaNotificacionEnvioMasivo.Where(item => item.NotificacionID.Equals(pNotificacionRow.NotificacionID) && item.Destinatarios.Equals(pCorreosNotificacion)).Count() == 0)
            {
                AD.EntityModel.Models.Notificacion.NotificacionEnvioMasivo filaEnvioMasivo = new AD.EntityModel.Models.Notificacion.NotificacionEnvioMasivo();

                filaEnvioMasivo.NotificacionID = pNotificacionRow.NotificacionID;
                filaEnvioMasivo.Destinatarios = pCorreosNotificacion;
                filaEnvioMasivo.Prioridad = 1;
                filaEnvioMasivo.EstadoEnvio = (short)EstadoEnvio.Pendiente;
                filaEnvioMasivo.FechaEnvio = null;

                NotificacionDW.ListaNotificacionEnvioMasivo.Add(filaEnvioMasivo);
                mEntityContext.NotificacionEnvioMasivo.Add(filaEnvioMasivo);
            }
        }

        /// <summary>
        /// Crea una nueva notificación
        /// </summary>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pFechaComienzoNotificacion">Fecha de comienzo de la notificación</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Notificación nueva</returns>
        private Notificacion AgregarNotificacion(TiposNotificacion pTipoNotificacion, DateTime pFechaComienzoNotificacion, Guid pOrganizacionID, Guid pProyectoID)
        {
            return AgregarNotificacion(pTipoNotificacion, pFechaComienzoNotificacion, pOrganizacionID, pProyectoID, null);
        }

        private Notificacion AgregarNotificacion(TiposNotificacion pTipoNotificacion, DateTime pFechaComienzoNotificacion, Guid pOrganizacionID, Guid pProyectoID, string pIdioma)
        {
            short mensajeID = (short)pTipoNotificacion;

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = new AD.EntityModel.Models.Notificacion.Notificacion();

            filaNotificacion.NotificacionID = Guid.NewGuid();
            filaNotificacion.MensajeID = mensajeID;
            filaNotificacion.FechaNotificacion = pFechaComienzoNotificacion;
            filaNotificacion.FechaFin = null;
            filaNotificacion.OrganizacionID = pOrganizacionID;
            filaNotificacion.ProyectoID = pProyectoID;
            filaNotificacion.FechaFin = pFechaComienzoNotificacion.AddDays(1);
            filaNotificacion.Idioma = pIdioma;

            NotificacionDW.ListaNotificacion.Add(filaNotificacion);
            mEntityContext.Notificacion.Add(filaNotificacion);

            Notificacion notificacion = new Notificacion(filaNotificacion, this);

            if (!this.ListaNotificaciones.ContainsKey(notificacion.FilaNotificacion.NotificacionID))
            {
                this.ListaNotificaciones.Add(notificacion.FilaNotificacion.NotificacionID, notificacion);
            }

            return notificacion;
        }

        /// <summary>
        /// Crea una nueva notificación
        /// </summary>
        /// <param name="pTipoNotificacion">Tipo de notificación</param>
        /// <param name="pFechaComienzoNotificacion">Fecha de comienzo de la notificación</param>
        /// <param name="pFechaFinNotificacion">Fecha de fin de la notificación</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Notificación nueva</returns>
        private Notificacion AgregarNotificacion(TiposNotificacion pTipoNotificacion, DateTime pFechaComienzoNotificacion, DateTime pFechaFinNotificacion, Guid? pOrganizacionID, Guid? pProyectoID)
        {
            short mensajeID = (short)pTipoNotificacion;

            AD.EntityModel.Models.Notificacion.Notificacion filaNotificacion = new AD.EntityModel.Models.Notificacion.Notificacion();

            filaNotificacion.NotificacionID = Guid.NewGuid();
            filaNotificacion.MensajeID = mensajeID;
            filaNotificacion.FechaNotificacion = pFechaComienzoNotificacion;
            filaNotificacion.FechaFin = pFechaFinNotificacion;

            if (pOrganizacionID.HasValue)
            {
                filaNotificacion.OrganizacionID = pOrganizacionID.Value;
            }

            if (pProyectoID.HasValue)
            {
                filaNotificacion.ProyectoID = pProyectoID.Value;
            }
            NotificacionDW.ListaNotificacion.Add(filaNotificacion);
            mEntityContext.Notificacion.Add(filaNotificacion);

            Notificacion notificacion = new Notificacion(filaNotificacion, this);

            if (!this.ListaNotificaciones.ContainsKey(notificacion.FilaNotificacion.NotificacionID))
            {
                this.ListaNotificaciones.Add(notificacion.FilaNotificacion.NotificacionID, notificacion);
            }
            return notificacion;
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene o establece la lista con todos los mensajes multiIdioma 
        /// con el formato Dictionary(idioma, Dictionary(mensajeID, KeyValuePair(Asunto, Texto)))
        /// </summary>
        public Dictionary<int, KeyValuePair<string, string>> ListaMensajes(string pIdioma)
        {
            return CargarListaMensajes(pIdioma);
        }

        /// <summary>
        /// Obtiene o establece la lista con los formatos extra para el correo (cabecera y pie), 
        /// del tipo Dictionary(idioma, Dictionary(nombre fragmento ["cabecera","pie"], string formato))
        /// </summary>
        public Dictionary<string, string> ListaFormatosCorreo(string pIdioma)
        {
            return CargarListaFormatosCorreo(pIdioma);
        }

        /// <summary>
        /// Obtiene el dataset de notificaciones
        /// </summary>
        public DataWrapperNotificacion NotificacionDW
        {
            get
            {
                return (DataWrapperNotificacion)DataWrapper;
            }
        }


        /// <summary>
        /// Obtiene la lista de notificaciones
        /// </summary>
        public Dictionary<Guid, Notificacion> ListaNotificaciones
        {
            get
            {
                if (mListaNotificaciones == null)
                {
                    mListaNotificaciones = new Dictionary<Guid, Notificacion>();

                    foreach (AD.EntityModel.Models.Notificacion.Notificacion notificacion in NotificacionDW.ListaNotificacion)
                    {
                        mListaNotificaciones.Add(notificacion.NotificacionID, new Notificacion(notificacion, this));
                    }
                }
                return mListaNotificaciones;
            }
        }

        /// <summary>
        /// Obtiene la lista de invitaciones
        /// </summary>
        public Dictionary<Guid, Invitacion.Invitacion> ListaInvitaciones
        {
            get
            {
                if (mListaInvitaciones == null)
                {
                    mListaInvitaciones = new Dictionary<Guid, Invitacion.Invitacion>();

                    foreach (AD.EntityModel.Models.Notificacion.Invitacion invitacion in NotificacionDW.ListaInvitacion)
                    {
                        if ((mGestorIdentidades != null) && (mGestorIdentidades.ListaIdentidades.ContainsKey(invitacion.IdentidadOrigenID)) && (ListaNotificaciones.ContainsKey(invitacion.NotificacionID)))
                        {
                            mListaInvitaciones.Add(invitacion.InvitacionID, new Invitacion.Invitacion(invitacion, ListaNotificaciones[invitacion.NotificacionID], mGestorIdentidades.ListaIdentidades[invitacion.IdentidadOrigenID]));
                        }
                    }
                }
                return mListaInvitaciones;
            }
        }

        /// <summary>
        /// Obtiene o establece el gestor de peticiones para las notificaciones que llevan una petición asociada
        /// </summary>
        public GestionPeticiones GestorPeticiones
        {
            get
            {
                return mGestorPeticiones;
            }
            set
            {
                mGestorPeticiones = value;
            }
        }

        #region Propiedades estáticas de textos

        /// <summary>
        /// Obtiene o establece el texto de Todos
        /// </summary>
        public static string TextoTodos
        {
            get
            {
                return mTextoTodos;
            }
            set
            {
                mTextoTodos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de Todos
        /// </summary>
        public static string TextoUsuariosGnoss
        {
            get
            {
                return mTextoTodos;
            }
            set
            {
                mTextoTodos = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de Comunidad
        /// </summary>
        public static string TextoComunidad
        {
            get
            {
                return mTextoComunidad;
            }
            set
            {
                mTextoComunidad = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de Participantes
        /// </summary>
        public static string TextoParticipantes
        {
            get
            {
                return mTextoParticipantes;
            }
            set
            {
                mTextoParticipantes = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de Privada
        /// </summary>
        public static string TextoPrivada
        {
            get
            {
                return mTextoPrivada;
            }
            set
            {
                mTextoPrivada = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de Publica
        /// </summary>
        public static string TextoPublica
        {
            get
            {
                return mTextoPublica;
            }
            set
            {
                mTextoPublica = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de Visible
        /// </summary>
        public static string TextoVisible
        {
            get
            {
                return mTextoVisible;
            }
            set
            {
                mTextoVisible = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto de NoVisible
        /// </summary>
        public static string TextoNoVisible
        {
            get
            {
                return mTextoNoVisible;
            }
            set
            {
                mTextoNoVisible = value;
            }
        }

        #endregion

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Método para serializar el objeto
        /// </summary>
        /// <param name="pInfo">Datos serializados</param>
        /// <param name="pContext">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo pInfo, StreamingContext pContext)
        {
            base.GetObjectData(pInfo, pContext);

            pInfo.AddValue("GestorPeticiones", GestorPeticiones);
            pInfo.AddValue("GestorIdentidades", mGestorIdentidades);
        }

        #endregion
    }
}
