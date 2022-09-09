using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.ParametroAplicacion
{
    /// <summary>
    /// Clase en la que est�n almacenados los nombre de los par�metros de la tabla ParametroAplicacion
    /// </summary>
    public class TiposParametrosAplicacion
    {
        #region Variables est�ticas

        /// <summary>
        /// Url de Intragnoss.
        /// </summary>
        public static string UrlIntragnoss { get { return "UrlIntragnoss"; } }

        /// <summary>
        /// Url de Intragnoss.
        /// </summary>
        public static string UrlIntragnossServicios { get { return "UrlIntragnossServicios"; } }

        /// <summary>
        /// Url de configuracion de la p�gina
        /// </summary>
        public static string UrlBaseService { get { return "UrlBaseService"; } }
        /// <summary>
        /// Url del servicio Web para la gesti�n de documentos.
        /// </summary>
        public static string UrlServicioWebDocumentacion { get { return "ServicioWebDocumentacion"; } }

        /// <summary>
        /// Nombre del servidor SMTP.
        /// </summary>
        public static string ServidorSMTP { get { return "ServidorSmtp"; } }

        /// <summary>
        /// Puerto del servidor SMTP.
        /// </summary>
        public static string PuertoSMTP { get { return "PuertoSmtp"; } }

        /// <summary>
        /// Usuario del servidor SMTP.
        /// </summary>
        public static string UsuarioSMTP { get { return "UsuarioSmtp"; } }

        /// <summary>
        /// Contrase�a del servidor SMTP.
        /// </summary>
        public static string PasswordSMTP { get { return "PasswordSmtp"; } }

        /// <summary>
        /// Url del actulizador.
        /// </summary>
        public static string UrlActualizadorGnoss { get { return "UrlActualizadorGnoss"; } }

        /// <summary>
        /// Correo para errores
        /// </summary>
        public static string CorreoErrores { get { return "CorreoErrores"; } }

        /// <summary>
        /// Correo para solicitudes
        /// </summary>
        public static string CorreoSolicitudes { get { return "CorreoSolicitudes"; } }

        /// <summary>
        /// Correo para sugerencias
        /// </summary>
        public static string CorreoSugerencias { get { return "CorreoSugerencias"; } }

        /// <summary>
        /// Correo para atencion al cliente
        /// </summary>
        public static string CorreoAtencionCliente { get { return "CorreoAtencionCliente"; } }

        /// <summary>
        /// Correo para solicitudes
        /// </summary>
        public static string UrlsPropiasProyecto { get { return "UrlsPropiasProyecto"; } }

        /// <summary>
        /// Correo para solicitudes
        /// </summary>
        public static string CodigoGoogleAnalyticsProyecto { get { return "CodigoGoogleAnalyticsProyecto"; } }

        /// <summary>
        /// Copyright
        /// </summary>
        public static string Copyright { get { return "copyright"; } }

        /// <summary>
        /// Registro, mostrar los datos demogr�ficos (Poblaci�n, localidad, pa�s) TRUE se muestra, FALSE no.
        /// </summary>
        public static string DatosDemograficosPerfil { get { return "DatosDemograficosPerfil"; } }

        /// <summary>
        /// Obtiene si hay que mostrar la pestanya de importar contactos por el email.
        /// </summary>
        public static string PestanyaImportarContactosCorreo { get { return "PestanyaImportarContactosCorreo"; } }

        /// <summary>
        /// Devuelve si hay que mostrar el panel con el mensaje que se va a enviar a la hora de importar contactos.
        /// </summary>
        public static string PanelMensajeImportarContactos { get { return "PanelMensajeImportarContactos"; } }

        /// <summary>
        /// Registro, TRUE si se permite la aceptaci�n directa del usuario, FALSE si se desea mantener la solicitud y enviar un mensaje/esperar a la confirmaci�n de alg�n administrador.
        /// </summary>
        public static string RegistroAutomaticoEcosistema { get { return "RegistroAutomaticoEcosistema"; } }

        /// <summary>
        /// Seguir, TRUE si cuando se le sigue a alguien se le sigue en toda/ninguna actividad, FALSE si se puede seleccionar si se le sigue por proyecto , espacio personal o todo
        /// </summary>
        public static string SeguirEnTodaLaActividad { get { return "SeguirEnTodaLaActividad"; } }

        /// <summary>
        /// P�gina de solicitudes en el ecosistema, TRUE = Se pinta el pie de gnoss, False = no se pinta.
        /// </summary>
        public static string PintarPiePaginaSolicitudes { get { return "PintarPiePaginaSolicitudes"; } }

        /// <summary>
        /// Versi�n del JS del ecosistema
        /// </summary>
        public static string VersionJSEcosistema { get { return "VersionJSEcosistema"; } }

        /// <summary>
        /// Versi�n del CSS del ecosistema
        /// </summary>
        public static string VersionCSSEcosistema { get { return "VersionCSSEcosistema"; } }

        /// <summary>
        /// Indica si tiene un CV unico por perfil
        /// </summary>
        public static string CVUnicoPorPerfil { get { return "CVUnicoPorPerfil"; } }

        /// <summary>
        /// Tiempo de Redis antes de que de un timeout
        /// </summary>
        public static string RedisTimeOut { get { return "RedisTimeOut"; } }

        /// <summary>
        /// Indica si los enlaces tienen que apuntar al perfil global en la comunidad principal
        /// </summary>
        public static string PerfilGlobalEnComunidadPrincipal { get { return "PerfilGlobalEnComunidadPrincipal"; } }

        /// <summary>
        /// Indica el proyecto principal de un ecosistema sin metaproyecto
        /// </summary>
        public static string ComunidadPrincipalID { get { return "ComunidadPrincipalID"; } }

        /// <summary>
        /// Indica el grafo en el que se debe insertar los recursos para la metabusqueda si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public static string GrafoMetaBusquedaRecursos { get { return "GrafoMetaBusquedaRecursos"; } }

        /// <summary>
        /// Indica el grafo en el que se debe insertar las personas y las organizaciones para la metabusqueda si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public static string GrafoMetaBusquedaPerYOrg { get { return "GrafoMetaBusquedaPerYOrg"; } }

        /// <summary>
        /// Indica el grafo en el que se debe insertar los proyectos para la metabusqueda si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public static string GrafoMetaBusquedaComunidades { get { return "GrafoMetaBusquedaComunidades"; } }

        /// <summary>
        /// Indica si el ecosistema tiene una personalizacion de vistas
        /// </summary>
        public static string PersonalizacionEcosistemaID { get { return "PersonalizacionEcosistemaID"; } }

        /// <summary>
        /// Indica si el ecosistema tiene una personalizacion de vistas
        /// </summary>
        public static string UrlServicioIntegracionEntornos { get { return "UrlServicioIntegracionEntornos"; } }

        /// <summary>
        /// Indica si el ecosistema tiene una personalizacion de vistas
        /// </summary>
        public static string ComunidadesExcluidaPersonalizacion { get { return "PersonalizacionEcosistemaExcepciones"; } }

        /// <summary>
        /// Indica si se trata de un ecosistema sin metaproyecto
        /// </summary>
        public static string EcosistemaSinMetaProyecto { get { return "EcosistemaSinMetaProyecto"; } }

        /// <summary>
        /// Indica si se trata de un ecosistema sin contactos
        /// </summary>
        public static string EcosistemaSinContactos { get { return "EcosistemaSinContactos"; } }

        /// <summary>
        /// Indica si se trata de un ecosistema sin Bandeja de Suscripcione
        /// </summary>
        public static string EcosistemaSinBandejaSuscripciones { get { return "EcosistemaSinBandejaSuscripciones"; } }

        /// <summary>
        /// Indica si se tienen que enviar las notificaciones de las suscripciones (si no se especifica es falso)
        /// </summary>
        public static string EnviarNotificacionesDeSuscripciones { get { return "EnviarNotificacionesDeSuscripciones"; } }

        /// <summary>
        /// Indica el tipo de cabecera
        /// </summary>
        public static string TipoCabecera { get { return "TipoCabecera"; } }

        /// <summary>
        /// Indica que la bandeja de invitaciones y comentarios se agrupas en una p�gina de Notificaciones
        /// </summary>
        public static string NotificacionesAgrupadas { get { return "NotificacionesAgrupadas"; } }

        /// <summary>
        /// Indica que la UrlHomeConectado
        /// </summary>
        public static string UrlHomeConectado { get { return "UrlHomeConectado"; } }

        /// <summary>
        /// Indica la IP que va a tener accesible recagar componentes en comunidades privadas
        /// </summary>
        public static string IPRecargarComponentes { get { return "IPRecargarComponentes"; } }

        /// <summary>
        /// Indica las ontolog�as que no deben ir al Live separadas por '|'.
        /// </summary>
        public static string OntologiasNoLive { get { return "OntologiasNoLive"; } }

        /// <summary>
        /// Indica se debe pintar un input auxiliar con los IDs de los grupos a los que pertenece el perfil conectado (inpt_gruposPerfilID).
        /// </summary>
        public static string MostrarGruposIDEnHtml { get { return "MostrarGruposIDEnHtml"; } }

        /// <summary>
        /// Indica si en el ecosistema se debe recibir la newsletter por defectoo
        /// </summary>
        public static string RecibirNewsletterDefecto { get { return "RecibirNewsletterDefecto"; } }

        /// <summary>
        /// Indica se debe pintar un input auxiliar con los IDs de los grupos a los que pertenece el perfil conectado (inpt_gruposPerfilID).
        /// </summary>
        public static string ConexionEntornoPreproduccion { get { return "ConexionEntornoPreproduccion"; } }

        /// Indica se debe pintar un input auxiliar con los IDs de los grupos a los que pertenece el perfil conectado (inpt_gruposPerfilID).
        /// </summary>
        public static string ConexionEntornoProduccion { get { return "ConexionEntornoPreproduccion"; } }

        /// <summary>
        /// Indica los dominios de las cuentas de correo permitidos para el login con redes sociales
        /// </summary>
        public static string DominiosEmailLoginRedesSociales { get { return "DominiosEmailLoginRedesSociales"; } }

        /// <summary>
        /// Indica si se va a generar el grafo de contribuciones de los usuarios o no
        /// </summary>
        public static string GenerarGrafoContribuciones { get { return "GenerarGrafoContribuciones"; } }

        /// <summary>
        /// Indica el token secreto para validar peticiones que lleguen con recaptcha
        /// </summary>
        public static string GoogleRecaptchaSecret { get { return "GoogleRecaptchaSecret"; } }

        /// <summary>
        /// Indica si se tiene que guardar el valor en negativo de la versi�n de la foto al eliminarla de la descripci�n del recurso
        /// </summary>
        public static string VersionFotoDocumentoNegativo { get { return "VersionFotoDocumentoNegativo"; } }

        /// <summary>
        /// 
        /// </summary>
        public static string GoogleDrive { get { return "GoogleDrive"; } }

        /// <summary>
        /// 
        /// </summary>
        public static string CargarIdentidadesDeProyectosPrivadosComoAmigos { get { return "CargarIdentidadesDeProyectosPrivadosComoAmigos"; } }

        /// <summary>
        /// Cadena de 4 digitos, los dos primeros configuran visibilidad en la comunidad, los dos �ltimos visibilidad en todo internet. De cada pareja de digitos, el primero indica si el check es visible y el segundo si est� marcado
        /// Ej.: 0111 -> es visible �nicamente el check de "visible en todo internet" pero ambos est�n marcados
        /// </summary>
        public static string VisibilidadPerfil { get { return "VisibilidadPerfil"; } }

        /// <summary>
        /// Identificador de application insights para subir la telemetr�a de la aplicaci�n
        /// Ej: bd094430-818d-4b03-a0fc-72850e6ffc31
        /// </summary>
        public static string ImplementationKey { get { return "ImplementationKey"; } }

        /// <summary>
        /// Nombre que se le va a dar al espacio personal del usuario
        /// Ej: Mi espacio personal@es|||My personal space@en|||Meu espaco pessoal@pt
        /// </summary>
        public static string NombreEspacioPersonal { get { return "NombreEspacioPersonal"; } }

        /// <summary>
        /// Datos de la aplicaci�n de Twitter para hacer login en la plataforma
        /// Ej: consumerkey|||lUwbKMGlYF9H6lPNtm4Gkj4DY@@@consumersecret|||2vY81oR1GFCAL818jafYyRglC8NBZaDYUl47xf1Z1HN5sUA6jf
        /// </summary>
        public static string LoginTwitter { get { return "loginTwitter"; } }

        /// <summary>
        /// Datos de la aplicaci�n de Google para hacer login en la plataforma
        /// Ej: id|||880006438141-r84hn261rtct92nrt3d5adjlod4umacn.apps.googleusercontent.com@@@clientsecret|||0d8iEeZDl-qV0XRiTnCCB54n
        /// </summary>
        public static string LoginGoogle { get { return "loginGoogle"; } }

        /// <summary>
        /// Datos de la aplicaci�n de facebook para hacer login en la plataforma
        /// Ej: id|||579839168701703@@@clientsecret|||b959a4481d0e88182834e4149b5d47c7
        /// </summary>
        public static string LoginFacebook { get { return "loginFacebook"; } }

        /// <summary>
        /// Lista de idiomas disponibles en la plataforma
        /// Ej: es|Espa�ol&&&en|English&&&pt|Portuguese&&&ca|Catal�&&&eu|Euskera&&&gl|Galego&&&fr|Fran�ais&&&de|Deutsch&&&it|Italiano
        /// </summary>
        public static string Idiomas { get { return "Idiomas"; } }

        /// <summary>
        /// Lista de extensiones de im�genes permitidas en la plataforma para la edici�n de componentes multimedia
        /// Ej: .jpg&&&.jpeg&&&.png&&&.gif
        /// </summary>
        public static string ExtensionesImagenesCMSMultimedia { get { return "ExtensionesImagenesCMSMultimedia"; } }

        /// <summary>
        /// Lista de extensiones de documentos permitidas en la plataforma para la edici�n de componentes multimedia
        /// Ej: .pdf&&&.txt&&&.doc&&&.docx
        /// </summary>
        public static string ExtensionesDocumentosCMSMultimedia { get { return "ExtensionesDocumentosCMSMultimedia"; } }

        /// <summary>
        /// Hash tag que ira en todas las publicaciones en twitter que se hagan de esta comunidad
        /// Ej: #gnoss
        /// </summary>
        public static string HashTagEntorno { get { return "HashTagEntorno"; } }

        /// <summary>
        /// Dominios que no pueden lleva un iframe, porque sus servidores lo rechazan
        /// Ej: scribd.com|prezi.com|sites.google.com
        /// </summary>
        public static string DominiosSinPalco { get { return "DominiosSinPalco"; } }

        /// <summary>
        /// Especifica si los usuarios s�lo pueden usar las categor�as privadas de su espacio personal para subir contenido
        /// </summary>
        public static string UsarSoloCategoriasPrivadasEnEspacioPersonal { get { return "UsarSoloCategoriasPrivadasEnEspacioPersonal"; } }


        /// <summary>
        /// Indica si los usuarios de esta plataforma van a tener perfil personal o no
        /// </summary>
        public static string PerfilPersonalDisponible { get { return "PerfilPersonalDisponible"; } }

        /// <summary>
        /// Especifica si se va a generar la home del usuario conectado o no
        /// </summary>
        public static string EcosistemaSinHomeUsuario { get { return "EcosistemaSinHomeUsuario"; } }

        /// <summary>
        /// Define la edad m�nima con la que los usuarios pueden registrarse en la plataforma
        /// </summary>
        public static string EdadLimiteRegistroEcosistema { get { return "EdadLimiteRegistroEcosistema"; } }

        /// <summary>
        /// Especif�ca el n�mero m�ximo de segundos que la sesi�n debe permanecer bloqueada. Si una petici�n est� esperando m�s de lo especifiado aqu�, iniciar� una nueva sesi�n
        /// </summary>
        public static string SegundosMaxSesionBloqueada { get { return "SegundosMaxSesionBloqueada"; } }

        /// <summary>
        /// Especif�ca el n�mero de conexiones m�ximo que tendr� el pool de conexiones de Redis
        /// </summary>
        public static string TamanioPoolRedis { get { return "TamanioPoolRedis"; } }

        /// <summary>
        /// Define en d�nde se van a guardar los logs 
        /// 0 Archivo de texto, 1 Application Insights, 2 Ambos
        /// </summary>
        public static string UbicacionLogs { get { return "UbicacionLogs"; } }

        /// <summary>
        /// Define en d�nde se van a guardar las trazas 
        /// 0 Archivo de texto, 1 Application Insights, 2 Ambos
        /// </summary>
        public static string UbicacionTrazas { get { return "UbicacionTrazas"; } }

        /// <summary>
        /// Define la duraci�n de la cookie de usuario al hacer login
        /// </summary>
        public static string DuracionCookieUsuario { get { return "DuracionCookieUsuario"; } }

        /// <summary>
        /// Indica si hay que mantener activa la sesi�n del usuario mientras permanece abierta la pesta�a del navegador
        /// </summary>
        public static string MantenerSesionActiva { get { return "MantenerSesionActiva"; } }

        /// <summary>
        /// Indica si el usuario s�lo se puede estar logueado en un s�lo navegador al mismo tiempo
        /// </summary>
        public static string LoginUnicoPorUsuario { get { return "LoginUnicoPorUsuario"; } }

        /// <summary>
        /// Indica si el usuario se le desconecta de otros navegadores
        /// </summary>
        public static string DesconexionUsuarios { get { return "DesconexionUsuarios"; } }


        /// <summary>
        /// Indica los usuarios que est�n excluidos de tener un login �nico
        /// </summary>
        public static string LoginUnicoUsuariosExcluidos { get { return "LoginUnicoUsuariosExcluidos"; } }

        /// <summary>
        /// Indica si la aceptaci�n de comunidades puede ser autom�tica
        /// </summary>
        public static string AceptacionComunidadesAutomatica { get { return "AceptacionComunidadesAutomatica"; } }

        /// <summary>
        /// Indica las p�ginas por las que va a pasar el asistente de creaci�n de comunidades (ej: administrar-categorias-comunidad-metaproyecto|invitar-a-comunidad|administrar-categorias)
        /// </summary>
        public static string PasosAsistenteCreacionComunidad { get { return "PasosAsistenteCreacionComunidad"; } }

        public static string EntornoIntegracionContinua { get { return "EntornoIntegracionContinua"; } }

        /// <summary>
        /// Indica si tiene que comprobar los repetidos.
        /// </summary>
        public static string ComprobarRepetidos { get { return "ComprobarRepetidos"; } }

        public static string NombrePlataforma { get { return "NombrePlataforma"; } }

        public static string EdicionMultiple { get { return "EdicionMultiple"; } }

        public static string CadenaConexionGrafoUsuarios { get { return "CadenaConexionGrafoUsuarios"; } }
        public static string CategoriasClase { get { return "CategoriasClase"; } }
        public static string ClaseFichaHome { get { return "ClaseFichaHome"; } }
        public static string CorreoErroresServicios { get { return "CorreoErroresServicios"; } }
        public static string CorreoErroresVistas { get { return "CorreoErroresVistas"; } }
        public static string GnossEduca { get { return "GnossEduca"; } }
        public static string GnossUniversidad20 { get { return "GnossUniversidad20"; } }
        public static string ipFTP { get { return "ipFTP"; } }
        public static string ListaNegraIdentidades { get { return "ListaNegraIdentidades"; } }
        public static string PasswordSmtp { get { return "PasswordSmtp"; } }
        public static string PresentacionBandejas { get { return "PresentacionBandejas"; } }
        public static string ProyectosRelClase { get { return "ProyectosRelClase"; } }
        public static string ScriptGoogleAnalytics { get { return "ScriptGoogleAnalytics"; } }
        public static string ScriptGoogleAnalytics_nuevo { get { return "ScriptGoogleAnalytics_nuevo"; } }
        public static string ServicioWebDocumentacion { get { return "ServicioWebDocumentacion"; } }
        public static string UrlContent { get { return "UrlContent"; } }
        public static string URLGrafoVirtuoso { get { return "URLGrafoVirtuoso"; } }
        public static string puertoFTP { get { return "puertoFTP"; } }
        public static string usarHTTPSParaDominioPrincipal { get { return "usarHTTPSParaDominioPrincipal"; } }

        public static string VistasV1 { get { return "VistasV1"; } }
        public static string VistasV2 { get { return "VistasV2"; } }

        public static string Replicacion { get { return "Replicacion"; } }

        public static string AzureStorage { get { return "AzureStorage"; } }
        public static string URIGrafoDbpedia { get { return "URIGrafoDbpedia"; } }
        public static string LecturaAumentada { get { return "LecturaAumentada"; } }

        public static string ConfiguradoColaGeneradorAutocompletar { get { return "ConfiguradoColaGeneradorAutocompletar"; } }

        public static string ConfiguradoColaLinkedData { get { return "ConfiguradoColaLinkedData"; } }

        public static string EscaparComillasDobles { get { return "EscaparComillasDobles"; } }
        public static string UsarHilosEnFacetas { get { return "UsarHilosEnFacetas"; } }
        #endregion
    }

    /// <summary>
    /// Tipos de conexiones
    /// </summary>
    public enum TipoConexion
    {
        /// <summary>
        /// SQLServer
        /// </summary>
        SQLServer = 0,
        /// <summary>
        /// Virtuoso
        /// </summary>
        Virtuoso = 1,
        /// <summary>
        /// Redis
        /// </summary>
        Redis = 2,
        /// <summary>
        /// RedisChat
        /// </summary>
        RedisChat = 3,
        /// <summary>
        /// Virtuoso HA-PROXY
        /// </summary>
        Virtuoso_HA_PROXY = 4,
        /// <summary>
        /// RabbitMQ
        /// </summary>
        RabbitMQ = 5,
        /// <summary>
        /// Logstash
        /// </summary>
        Logstash = 6
    }

    public enum TipoConexionPruebas
    {
        /// <summary>
        /// Conexi�n a SQL
        /// </summary>
        SQL = 0,
        /// <summary>
        /// Conexi�n a Oracle
        /// </summary>
        Oracle = 1
    }

    /// <summary>
    /// Tipos de b�squeda
    /// </summary>
    public enum TipoAccionExterna
    {
        /// <summary>
        /// Login en el servicio externo y si no existe en la aplicaci�n, se registra
        /// </summary>
        LoginConRegistro = 0,
        /// <summary>
        /// Cuando se registra en la aplicaci�n, se da de alta en el servicio externo
        /// </summary>
        Registro = 1,
        /// <summary>
        /// Cuando se edita en la aplicaci�n, se edita en el servicio externo
        /// </summary>
        Edicion = 2,
        /// <summary>
        /// Cuando intenta hacer un registro o solicitud se valida antes
        /// </summary>
        ValidacionRegistro = 3,
        /// <summary>
        /// Cuando se edita la password validando que se loguea en el servicio externo
        /// </summary>
        EditarPasswordConLogin = 4,
        /// <summary>
        /// Cuando se solicita un registro, se da de alta en el servicio externo (sin activar)
        /// </summary>
        SolicitudRegistro = 5,
        /// <summary>
        /// Cuando se acepta un registro, se activa la solicitud da de alta en el servicio externo
        /// </summary>
        ActivacionRegistro = 6,
        /// <summary>
        /// Cuando se edita la password sin validar
        /// </summary>
        EditarPassword = 7,
        /// <summary>
        /// Registro tras crear el usuario con el logi con registro
        /// </summary>
        RegistroTrasLoginConRegistro = 8,
        /// <summary>
        /// Invalida un usuario
        /// </summary>
        InvalidarUsuario = 9,
        /// <summary>
        /// Crear un recurso
        /// </summary>
        CrearRecurso = 10,
        /// <summary>
        /// Editar un recurso
        /// </summary>
        EditarRecurso = 11,
        /// <summary>
        /// Eliminar un recurso
        /// </summary>
        EliminarRecurso = 12
    }

    /// <summary>
    /// Clase que sirve a la aplicaci�n para comunicarse con la BD, para interactuar con la tabla ParametroAplicacion.
    /// </summary>
    public class ParametroAplicacionAD : BaseAD
    {
        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// Constructor a partir del fichero de configuraci�n de la conexi�n a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de la conexi�n a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroAplicacionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        /// <summary>
        /// Constructor sin par�metros
        /// </summary>
        public ParametroAplicacionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
        }

        #endregion

        #region M�todos generales

        #region P�blicos

        /// <summary>
        /// Obtiene el valor de la url de Intragnoss
        /// </summary>
        /// <returns></returns>
        public string ObtenerUrl()
        {
            return mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("UrlIntragnoss")).First().Valor;
        }

        /// <summary>
        /// Obtiene el valor de la UrlContent de la tabla "ParametroProyecto", y si es vac�a, de la tabla "ParametroAplicacion"
        /// </summary>
        /// <returns>Cadena de texto con la url de la intranet de Gnoss</returns>
        public string ObtenerUrlContent(Guid pProyectoID)
        {
            string salida = null;
            string url = string.Empty;

            try
            {
                if (!pProyectoID.Equals(Guid.Empty))
                {
                    salida = mEntityContext.ParametroProyecto.Where(parametroProy => parametroProy.ProyectoID.Equals(pProyectoID) && parametroProy.Parametro.Equals("UrlContent")).Select(item => item.Valor).FirstOrDefault();
                    if (salida != null)
                    {
                        url = salida;
                    }
                }

                if (string.IsNullOrEmpty(url))
                {
                    salida = mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("UrlContent")).Select(item => item.Valor).FirstOrDefault();

                    if (salida != null)
                    {
                        url = salida;
                    }
                }
            }
            finally
            {
            }
            return url;
        }

        /// <summary>
        /// Obtiene el valor de la url de Intragnoss
        /// </summary>
        /// <returns></returns>
        public string ObtenerCorreoSugerencias()
        {
            return mEntityContext.ParametroAplicacion.Where(parametroApp => parametroApp.Parametro.Equals("CorreoSugerencias")).Select(item => item.Valor).FirstOrDefault();

        }

        /// <summary>
        /// Obtiene una lista con los proyectos y su personalizacion
        /// </summary>
        /// <returns>Lista con los proyectos y su personalizacion</returns>
        public Dictionary<Guid, string> ObtenerProyectosRegistroObligatorio()
        {
            Dictionary<Guid, string> listaProyectosConVistas = new Dictionary<Guid, string>();

            var consulta = mEntityContext.Proyecto.Join(mEntityContext.ProyectoRegistroObligatorio, proyecto => proyecto.ProyectoID, proyRegis => proyRegis.ProyectoID, (proyecto, proyRegis) => new
            {
                Proyecto = proyecto,
                ProyectoRegistroObligatorio = proyRegis
            }).Select(objeto => new { objeto.Proyecto.NombreCorto, objeto.Proyecto.ProyectoID }).ToList();


            foreach (var fila in consulta)
            {
                Guid proyectoID = fila.ProyectoID;
                string nombreProy = fila.NombreCorto;

                if (!listaProyectosConVistas.ContainsKey(proyectoID))
                {
                    listaProyectosConVistas.Add(proyectoID, nombreProy);
                }
            }

            return listaProyectosConVistas;
        }

        public List<Guid> ObtenerProyectosSinRegistroObligatorio(Guid idProyecto)
        {
            return mEntityContext.Proyecto.Join(mEntityContext.ProyectoSinRegistroObligatorio, proyecto => proyecto.ProyectoID, proySinRegis => proySinRegis.ProyectoID, (proyecto, proySinRegis) => new
            {
                Proyecto = proyecto,
                ProyectoSinRegistroObligatorio = proySinRegis
            }).Where(objeto => objeto.Proyecto.ProyectoID.Equals(idProyecto)).Select(objeto => objeto.ProyectoSinRegistroObligatorio.ProyectoSinRegistroID).ToList();

        }


        public void ActualizarParametrosAplicacion()
        {
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Devuelve un dataset con todos los par�metros de la BD de las tablas ParametroAplicacion, ProyectoRegistroObligatorio y ProyectoSinRegistroObligatorio.
        /// </summary>
        /// <returns>Dataset con los par�metros de la aplicaci�n</returns>
        //public DataRow[] ObtenerFilasConfiguracionBBDD(string pNombreConexion, params short[] pTipoConexion)
        public List<ConfiguracionBBDD> ObtenerFilasConfiguracionBBDD(string pNombreConexion, EntityContext pContext, params short[] pTipoConexion)
        {

            List<ConfiguracionBBDD> listaConfiguracionBBDD = null;
            if (pContext == null)
            {
                pContext = mEntityContext;
            }
            //if (pTipoConexion.Length > 0)
            //{
            //string sqlConfiguracionBBDD = "SELECT NumConexion, NombreConexion, TipoConexion, Conexion, DatosExtra, ConectionTimeOut, EsMaster, LecturaPermitida FROM ConfiguracionBBDD WHERE NombreConexion = " + IBD.ToParam("NombreConexion");

            if (pTipoConexion.Length > 1)
            {
                listaConfiguracionBBDD = pContext.ConfiguracionBBDD.Where(config => config.NombreConexion.Equals(pNombreConexion) && pTipoConexion.Contains((short)config.TipoConexion)).ToList();
                //sqlConfiguracionBBDD += " AND TipoConexion in (";

                //string concatenador = "";
                //foreach (short tipoConexion in pTipoConexion)
                //{
                //    sqlConfiguracionBBDD += concatenador + tipoConexion;
                //    concatenador = " ,";
                //}
                //sqlConfiguracionBBDD += ")";
            }
            else if (pTipoConexion.Length > 0)
            {
                // sqlConfiguracionBBDD += " AND TipoConexion = " + pTipoConexion[0];

                listaConfiguracionBBDD = pContext.ConfiguracionBBDD.Where(config => config.NombreConexion.Equals(pNombreConexion) && config.TipoConexion.Equals(pTipoConexion.FirstOrDefault())).ToList();
            }


            // DbCommand commandsql = ObtenerComando(sqlConfiguracionBBDD);
            // AgregarParametro(commandsql, IBD.ToParam("NombreConexion"), DbType.String, pNombreConexion);

            // ParametroAplicacionDS parametroAplicacionDS = new ParametroAplicacionDS();
            //CargarDataSet(commandsql, parametroAplicacionDS, "ConfiguracionBBDD");

            //return parametroAplicacionDS.ConfiguracionBBDD.Select();
            //}
            return listaConfiguracionBBDD;
        }

        public List<ConfiguracionBBDD> ObtenerConfiguracionBBDD()
        {
            return mEntityContext.ConfiguracionBBDD.ToList();
        }

        public bool ObtenerVisibilidadUsuariosActivosProyecto(Guid pOrganizacionID, Guid pPoryectoID)
        {
            bool visibilidadUsuariosActivos = false;
            EntityContext context = mEntityContext;
            ProyectoRegistroObligatorio fila = context.ProyectoRegistroObligatorio.FirstOrDefault(proyectoObligatorio => proyectoObligatorio.OrganizacionID.Equals(pOrganizacionID) && proyectoObligatorio.ProyectoID.Equals(pPoryectoID));

            if (fila != null)
            {
                visibilidadUsuariosActivos = fila.VisibilidadUsuariosActivos == 1;
            }

            return visibilidadUsuariosActivos;
        }

        /// <summary>
        /// Obtiene las filas de los proyectos en los que es necesario registrar al usuario que se est� registrando.
        /// </summary>
        /// <param name="pOrganizacionRegistroUsuario">Identificador de la organizaci�n en la que se est� registrando el usuario</param>
        /// <param name="pProyectoRegistroUsuario">Identificador de la comunidad en la que se est� registrando el usuario</param>
        /// <returns></returns>
        //public List<ParametroAplicacionDS.ProyectoRegistroObligatorioRow> ObtenerFilasProyectosARegistrarUsuario(Guid pOrganizacionRegistroUsuario, Guid pProyectoRegistroUsuario)
        public List<ProyectoRegistroObligatorio> ObtenerFilasProyectosARegistrarUsuario(Guid pOrganizacionRegistroUsuario, Guid pProyectoRegistroUsuario)
        {
            EntityContext context = mEntityContext;
            List<Guid> filasProyectoSinRegistroObligatorio = context.ProyectoSinRegistroObligatorio.Where(proyecto => proyecto.OrganizacionID.Equals(pOrganizacionRegistroUsuario) && proyecto.ProyectoID.Equals(pProyectoRegistroUsuario)).Select(proyecto => proyecto.ProyectoSinRegistroID).ToList();
            List<ProyectoRegistroObligatorio> filasProyectosARegistrarUsuario = context.ProyectoRegistroObligatorio.Where(proyecto => !filasProyectoSinRegistroObligatorio.Contains(proyecto.ProyectoID)).ToList();

            return filasProyectosARegistrarUsuario;
        }

        /// <summary>
        /// obtiene el  valor de un parametro en la base de datos
        /// </summary>
        /// <param name="parametro">Nombre del parametro</param>
        /// <param name="valor">Valor que tiene el parametro</param>
        public string ObtenerParametroAplicacion(string parametro)
        {
            return mEntityContext.ParametroAplicacion.Where(proy => proy.Parametro.Equals(parametro)).Select(item => item.Valor).FirstOrDefault();
        }

        public EntityModel.ParametroAplicacion ObtenerFilaParametroAplicacion(string parametro)
        {
            EntityModel.ParametroAplicacion param = mEntityContext.ParametroAplicacion.Where(proy => proy.Parametro.Equals(parametro)).FirstOrDefault();
            return param;
        }

        /// <summary>
        /// Modifica el valor de un parametro en la base de datos
        /// </summary>
        /// <param name="pParametro">Nombre del parametro</param>
        /// <param name="pValor">Valor que tiene el parametro</param>
        public void ActualizarParametroAplicacion(string pParametro, string pValor)
        {
            EntityModel.ParametroAplicacion parametroAplicacion = mEntityContext.ParametroAplicacion.Where(proy => proy.Parametro.Equals(pParametro)).FirstOrDefault();

            if (!string.IsNullOrEmpty(pValor))
            {
                if (parametroAplicacion != null && !parametroAplicacion.Valor.Equals(pValor))
                {
                    //Si el par�metro existe en base de datos y el valor es distinto lo actualizo
                    parametroAplicacion.Valor = pValor;
                }
                else if (parametroAplicacion == null)
                {
                    //Si el par�metro no existe en base de datos lo a�ado
                    EntityModel.ParametroAplicacion parametroAplicacionNuevo = new EntityModel.ParametroAplicacion();
                    parametroAplicacionNuevo.Parametro = pParametro;
                    parametroAplicacionNuevo.Valor = pValor;
                    mEntityContext.ParametroAplicacion.Add(parametroAplicacionNuevo);
                }
            }
            else if (parametroAplicacion != null)
            {
                //Si el valor nuevo para el parametro esta vac�o, y exist�a este parametro en base de datos lo elimino
                mEntityContext.ParametroAplicacion.Remove(parametroAplicacion);
            }
        }

        public string ObtenerParametroBusquedaPorTextoLibrePersonalizado()
        {
            var fila = mEntityContext.ParametroAplicacion.FirstOrDefault(parametroApp => parametroApp.Parametro.Equals("BusquedaPorTextoLibrePersonalizada"));
            if (fila != null)
            {
                return fila.Valor;
            }

            return null;
        }

        public string ObtenerUsarMismaVariablesParaEntidadesfacetas(Guid pProyectoID)
        {
            return mEntityContext.ParametroProyecto.Where(item => item.ProyectoID.Equals(pProyectoID) && item.Parametro.Equals(ParametroAD.UsarMismsaVariablesParaEntidadesEnFacetas)).Select(item => item.Valor).FirstOrDefault();
        }

        #endregion

        #endregion
    }
}
