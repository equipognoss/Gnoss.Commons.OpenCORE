using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{

    /// <summary>
    /// View model para el controlador de administrar opciones avanzadas de la plataforma
    /// </summary>
    public class AdministrarOpcionesAvanzadasPlataformaViewModel
    {

        /// <summary>
        /// Código de google analytics que se le va a asignar a cada proyecto nuevo según su tipo 
        /// Formato: tipoProyecto=codigo|tipoProyecto=codigo
        /// Ejemplo: 0=UA-XXXXXXXX-1|2=UA-YYYYYYYY-1
        /// Tipos de proyecto: 0 publico, 1 privado, 2 acceso restringido, 3 reservado
        /// </summary>
        public string CodigoGoogleAnalyticsProyecto { get; set; }

        /// <summary>
        /// URL de aplicaciones web externas 
        /// </summary>
        public string DominiosPermitidosCORS { get; set; }

        /// <summary>
        /// URL en la que van a estar disponibles las comunidades nuevas según su tipo 
        /// Formato: tipoProyecto=codigo|tipoProyecto=codigo
        /// Ejemplo: 0=http://prered.gnoss.com|2=http://prered.gnoss.com|1=https://preredprivada.gnoss.com|3=https://preredprivada.gnoss.com
        /// Tipos de proyecto: 0 publico, 1 privado, 2 acceso restringido, 3 reservado
        /// </summary>
        public string UrlsPropiasProyecto { get; set; }

        /// <summary>
        /// Conexión al API del entorno de preproducción para los pasos a producción
        /// Formato: UrlAPI|ConsumerKey|ConsumerSecret|Token|TokenSecret
        /// Ej: http://preapi.gnoss.com|CC1166B9779C415DBD44C4CAEAC1DC91|C50C327F1B7F4974B1E14C3CFFA8E628|7FqSvpmpuV1TsTPksK3C9Sy9hfw=|2aP5EjyczhARV9Ys04oH/xIl860=
        /// </summary>
        public string ConexionEntornoPreproduccion { get; set; }

        /// <summary>
        /// Correo al que van a llegar las solicitudes de nuevas comunidades
        /// </summary>
        public string CorreoSolicitudes { get; set; }

        /// <summary>
        /// Correo de sugerencias
        /// </summary>
        public string CorreoSugerencias { get; set; }

        /// <summary>
        /// Dominios que no pueden lleva un iframe, porque sus servidores lo rechazan
        /// Ej: scribd.com|prezi.com|sites.google.com
        /// </summary>
        public string DominiosSinPalco { get; set; }

        /// <summary>
        /// Hash tag que ira en todas las publicaciones en twitter que se hagan de esta comunidad
        /// Ej: #gnoss
        /// </summary>
        public string HashTagEntorno { get; set; }

        /// <summary>
        /// Lista de idiomas disponibles en la plataforma
        /// Ej: es|Español&&&en|English&&&pt|Portuguese&&&ca|Català&&&eu|Euskera&&&gl|Galego&&&fr|Français&&&de|Deutsch&&&it|Italiano
        /// </summary>
        public string Idiomas { get; set; }

        /// <summary>
        /// Lista de idiomas personalizados configurados en la plataforma
        /// </summary>
        public string IdiomasPersonalizados { get; set; }

        /// <summary>
        /// Lista de extensiones de imágenes permitidas en la plataforma para la edición de componentes multimedia
        /// Ej: .jpg&&&.jpeg&&&.png&&&.gif
        /// </summary>
        public string ExtensionesImagenesCMSMultimedia { get; set; }

        /// <summary>
        /// Lista de extensiones de documentos permitidas en la plataforma para la edición de componentes multimedia
        /// Ej: .pdf&&&.txt&&&.doc&&&.docx
        /// </summary>
        public string ExtensionesDocumentosCMSMultimedia { get; set; }

        /// <summary>
        /// Datos de la aplicación de facebook para hacer login en la plataforma
        /// Ej: id|||579839168701703@@@clientsecret|||b959a4481d0e88182834e4149b5d47c7
        /// </summary>
        public string LoginFacebook { get; set; }

        /// <summary>
        /// Datos de la aplicación de Google para hacer login en la plataforma        
        /// </summary>
        public string LoginGoogle { get; set; }

        /// <summary>
        /// Datos de la aplicación de Twitter para hacer login en la plataforma
        /// Ej: consumerkey|||lUwbKMGlYF9H6lPNtm4Gkj4DY@@@consumersecret|||2vY81oR1GFCAL818jafYyRglC8NBZaDYUl47xf1Z1HN5sUA6jf
        /// </summary>
        public string LoginTwitter { get; set; }

        /// <summary>
        /// Nombre que se le va a dar al espacio personal del usuario
        /// Ej: Mi espacio personal@es|||My personal space@en|||Meu espaco pessoal@pt
        /// </summary>
        public string NombreEspacioPersonal { get; set; }

        /// <summary>
        /// Copyright de la plataforma
        /// Ej: RIAM I+L lab
        /// </summary>
        public string Copyright { get; set; }

        /// <summary>
        /// Cadena de 4 digitos, los dos primeros configuran visibilidad en la comunidad, los dos últimos visibilidad en todo internet. De cada pareja de digitos, el primero indica si el check es visible y el segundo si está marcado
        /// Ej.: 0111 -> es visible únicamente el check de "visible en todo internet" pero ambos están marcados
        /// </summary>
        public string VisibilidadPerfil { get; set; }

        /// <summary>
        /// Indica las ontologías que no deben ir al Live. Separadas por '|'
        /// Ej: LearningOpportunitySpecification.owl|LearningOpportunityInstance.owl|request.owl|frequentRequest.owl
        /// </summary>
        public string OntologiasNoLive { get; set; }

        /// <summary>
        /// Identificador de application insights para subir la telemetría de la aplicación
        /// Ej: bd094430-818d-4b03-a0fc-72850e6ffc31
        /// </summary>
        public string ImplementationKey { get; set; }

        /// <summary>
        /// Define la URL a la que se le va a redireccionar al usuario tras el login. Puede ser una url absoluta, una url que exista en el archivo de idiomas, o una url de una página del CMS. 
        /// Ej: http://www.gnoss.com/home
        /// </summary>
        public string UrlHomeConectado { get; set; }

        /// <summary>
        /// Indica el token secreto de Google Recaptcha. Si se configura aquí y se añade a una vista, el servidor validará en esa página que la petición llega con un token Google Recaptcha correcto
        /// Ej: 6Le3TyUTAAAAAIwjDX8D6XLuJAs9LG-yBK7icasc
        /// </summary>
        public string GoogleRecaptchaSecret { get; set; }

        /// <summary>
        /// Cuentas válidas para hacer login con redes sociales. Si la cuenta de email con la que se han logueado no es una de las especificadas aquí, no se logueará al usuario
        /// Ej: @opendeusto.es,@deusto.es
        /// </summary>
        public string DominiosEmailLoginRedesSociales { get; set; }

        /// <summary>
        /// Define si los usuarios de esta plataforma van a tener disponible la bandeja de suscripciones
        /// </summary>
        public bool EcosistemaSinBandejaSuscripciones { get; set; }

        /// <summary>
        /// Define si los usuarios de esta plataforma van a tener disponible la opción de hacerse contacto
        /// </summary>
        public bool EcosistemaSinContactos { get; set; }

        /// <summary>
        /// Define si la versión de la foto de un recurso puede ser negativa. Esto sucede cuando se elimina la imagen de un recurso 
        /// </summary>
        public bool VersionFotoDocumentoNegativo { get; set; }

        /// <summary>
        /// Define si los usuarios van a tener un único CV por perfil, en vez de uno por comunidad
        /// </summary>
        public bool CVUnicoPorPerfil { get; set; }

        /// <summary>
        /// Especifíca si los usuarios deben rellenar sus datos de ciudad, provincia y país en el registro
        /// </summary>
        public bool DatosDemograficosPerfil { get; set; }

        /// <summary>
        /// Especifíca que la plataforma actual no tiene un metaproyecto (mygnoss)
        /// </summary>
        public bool EcosistemaSinMetaproyecto { get; set; }

        /// <summary>
        /// Indica si se puede personalizar el mensaje que se envía al invitar a un usuario a la comunidad
        /// </summary>
        public bool PanelMensajeImportarContactos { get; set; }

        /// <summary>
        /// Obtiene si los enlaces tienen que apuntar al perfil global en la comunidad principal
        /// </summary>
        public bool PerfilGlobalEnComunidadPrincipal { get; set; }

        /// <summary>
        /// Indica si se puede invitar por email en las comunidades de la plataforma
        /// </summary>
        public bool PestanyaImportarContactosCorreo { get; set; }

        /// <summary>
        /// Indica si debe aceptar automáticamente los registros en el ecosistema o mantenerlos en espera hasta que se acepte la solicitud por el administrador
        /// </summary>
        public bool RegistroAutomaticoEcosistema { get; set; }

        /// <summary>
        /// Define si los usuarios pueden seguir a otros usuarios por comunidades o una vez que se empieza a seguir a un usuario, se le sigue en todas sus comunidades
        /// </summary>
        public bool SeguirEnTodaLaActividad { get; set; }

        /// <summary>
        /// Especifica si los usuarios sólo pueden usar las categorías privadas de su espacio personal para subir contenido
        /// </summary>
        public bool UsarSoloCategoriasPrivadasEnEspacioPersonal { get; set; }

        /// <summary>
        /// Especifica si se va a generar la home del usuario conectado o no
        /// </summary>
        public bool EcosistemaSinHomeUsuario { get; set; }

        /// <summary>
        /// Indica si las notificaciones de invitaciones y comentarios llegan a una misma bandeja
        /// </summary>
        public bool NotificacionesAgrupadas { get; set; }

        /// <summary>
        /// Define si los usuarios van a estar suscritos por defecto a la newsletter o no en todas las comunidades de la plataforma
        /// </summary>
        public bool RecibirNewsletterDefecto { get; set; }

        /// <summary>
        /// Indica si los usuarios de esta plataforma van a tener perfil personal o no
        /// </summary>
        public bool PerfilPersonalDisponible { get; set; }

        /// <summary>
        /// Especifica si se debe cargar un input hidden en el HTML con los grupos en los que participa el usuario
        /// </summary>
        public bool MostrarGruposIDEnHtml { get; set; }

        /// <summary>
        /// Especifica si se deben generar los grafos de contirbuciones de los usuarios
        /// </summary>
        public bool GenerarGrafoContribuciones { get; set; }

        /// <summary>
        /// Indica si se quiere reiniciar la aplicacion ahora o mas tarde manualmente
        /// </summary>
        public bool ReiniciarAplicacion { get; set; }

        /// <summary>
        /// Indica si el numero de elementos visibles de una faceta debe ser estrictamente el especificado en la administracion de facetas
        /// </summary>
        public bool NumElementosVisiblesEstrictoFacetas { get; set; }

        /// <summary>
        /// Define la edad mínima con la que los usuarios pueden registrarse en la plataforma
        /// </summary>
        public int EdadLimiteRegistroEcosistema { get; set; }

        /// <summary>
        /// Especifíca el número máximo de segundos que la sesión debe permanecer bloqueada. Si una petición está esperando más de lo especifiado aquí, iniciará una nueva sesión
        /// </summary>
        public int SegundosMaxSesionBloqueada { get; set; }

        /// <summary>
        /// Especifíca el número de conexiones máximo que tendrá el pool de conexiones de Redis
        /// </summary>
        public int TamanioPoolRedis { get; set; }

        /// <summary>
        /// Define en dónde se van a guardar los logs 
        /// 0 Archivo de texto, 1 Application Insights, 2 Ambos
        /// </summary>
        public int UbicacionLogs { get; set; }

        /// <summary>
        /// Define en dónde se van a guardar las trazas 
        /// 0 Archivo de texto, 1 Application Insights, 2 Ambos
        /// </summary>
        public int UbicacionTrazas { get; set; }

        /// <summary>
        /// Define el identificador de la personalización que se va a usar para personalizar todas las comunidades de la plataforma (salvo las excepciones)
        /// </summary>
        public Guid PersonalizacionEcosistemaID { get; set; }

        /// <summary>
        /// Define el proyecto principal de un ecosistema sin metaproyecto
        /// </summary>
        public Guid ComunidadPrincipalID { get; set; }

        /// <summary>
        /// Define la duración de la cookie de usuario al hacer login
        /// </summary>
        public string DuracionCookieUsuario { get; set; }

        /// <summary>
        /// Indica si hay que mantener activa la sesión del usuario mientras permanece abierta la pestaña del navegador
        /// </summary>
        public bool MantenerSesionActiva { get; set; }

        /// <summary>
        /// Indica si se va a bloquear un enviar un correo de notificación al usuario cuando se sigue a su perfil, por defecto esta desactivado para que si se envíe el correo
        /// </summary>
        public bool NoEnviarCorreoSeguirPerfil { get; set; }

        [Obsolete]
        public string ConexionEntornoProduccion { get; set; }
        [Obsolete]
        public string CadenaConexionGrafoUsuarios { get; set; }
        [Obsolete]
        public string CategoriasClase { get; set; }
        [Obsolete]
        public string ClaseFichaHome { get; set; }
        [Obsolete]
        public string CorreoErroresServicios { get; set; }

        public string CorreoErroresVistas { get; set; }
        public string GnossEduca { get; set; }
        [Obsolete]
        public string GnossUniversidad20 { get; set; }
        public string ipFTP { get; set; }
        [Obsolete]
        public string ListaNegraIdentidades { get; set; }
        public string PasswordSmtp { get; set; }
        public string PersonalizacionEcosistemaExcepciones { get; set; }
        public string PresentacionBandejas { get; set; }
        [Obsolete]
        public string ProyectosRelClase { get; set; }
        public string ScriptGoogleAnalytics { get; set; }
        public string ServicioWebDocumentacion { get; set; }
        public string UrlContent { get; set; }
        [Obsolete]
        public string URLGrafoVirtuoso { get; set; }
        public string UrlIntragnoss { get; set; }
        public string UrlIntragnossServicios { get; set; }
        public string UrlBaseService { get; set; }
        public string PintarPiePaginaSolicitudes { get; set; }
        public string GrafoMetaBusquedaRecursos { get; set; }
        public string GrafoMetaBusquedaPerYOrg { get; set; }
        public string GrafoMetaBusquedaComunidades { get; set; }
        public string ComunidadesExcluidaPersonalizacion { get; set; }
        public bool EnviarNotificacionesDeSuscripciones { get; set; }
        public string IPRecargarComponentes { get; set; }
        public string GoogleDrive { get; set; }
        public int CargarIdentidadesDeProyectosPrivadosComoAmigos { get; set; }
        public bool LoginUnicoPorUsuario { get; set; }
        public string LoginUnicoUsuariosExcluidos { get; set; }
        public int AceptacionComunidadesAutomatica { get; set; }
        public string PasosAsistenteCreacionComunidad { get; set; }
        public int puertoFTP { get; set; }
        public int TipoCabecera { get; set; }
        public int usarHTTPSParaDominioPrincipal { get; set; }
        public int VersionJSEcosistema { get; set; }
        public int VersionCSSEcosistema { get; set; }

    }
}
