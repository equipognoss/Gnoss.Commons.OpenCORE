using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Elementos.Comentario;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles.ServiciosGenerales;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Metagnoss.ExportarImportar;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

/// <summary>
/// Clase para construir urls semánticas
/// </summary>
public class GnossUrlsSemanticas
{
    #region Miembros

    /// <summary>
    /// Almacena una lista del tipo nombreCortoProyecto -> URL del proyecto por idioma. ej => iphone, es, preiphone.gnoss.com
    /// </summary>
    private static ConcurrentDictionary<string, Dictionary<string, string>> mListaURLComunidades = new ConcurrentDictionary<string, Dictionary<string, string>>();
    private object BLOQUEO_ListaURLComunidades = new object();

    /// <summary>
    /// Almacena una lista con los nombres cortos de las comunidades que no tienen nombrecorto en la URL
    /// </summary>
    private static List<string> mListaNombreCortoComunidadesSinNombreCortoEnURL = null;
    private object BLOQUEO_ListaNombreCortoComunidadesSinNombreCortoEnURL = new object();

    /// <summary>
    /// Almacena una lista del tipo nombreCortoProyecto -> idOntologia -> nombreCortoOntolgia
    /// </summary>
    private static ConcurrentDictionary<string, Dictionary<Guid, string>> mListaURLOntologiasPorProyecto = new ConcurrentDictionary<string, Dictionary<Guid, string>>();
    private object BLOQUEO_ListaURLOntologiasPorProyecto = new object();

    /// <summary>
    /// Almacena una lista del tipo nombreCortoProyecto -> nombreOntologia(*.owl) -> idOntologia
    /// </summary>
    private static ConcurrentDictionary<string, Dictionary<string, Guid>> mListaIDsOntologiasPorProyecto = new ConcurrentDictionary<string, Dictionary<string, Guid>>();
    private object BLOQUEO_ListaIDsOntologiasPorProyecto = new object();

    public static string IdiomaPrincipalDominio = "es";

    private LoggingService mLoggingService;
    private EntityContext mEntityContext;
    private ConfigService mConfigService;
    private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
    private ILogger mlogger;
    private ILoggerFactory mLoggerFactory;
    #endregion

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    public GnossUrlsSemanticas(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<GnossUrlsSemanticas> logger, ILoggerFactory loggerFactory)
    {
        mLoggingService = loggingService;
        mEntityContext = entityContext;
        mConfigService = configService;
        mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        mlogger = logger;
        mLoggerFactory = loggerFactory;
    }

    #endregion

    #region Constantes

    /// <summary>
    /// Constante que representa la región del archivo de idiomas relacionado con las URLs semánticas
    /// </summary>
    private static string URLSEM = "URLSEM";

    #endregion

    #region Constructores de Url's

    #region Anyadir Gnoss Curriculum Linkedin

    /// <summary>
    /// Devuelve la URL relacionada con la página de Anyadir Gnoss Curriculum Linkedin
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAnyadirGnossCurriculumLinkedin(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "ANYADIRGNOSSCURRICULUMLINKEDIN");
    }

    #endregion

    #region Pies de página

    /// <summary>
    /// Devuelve la URL relacionada con la página de RIAM
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string ObtenerUrlBaseIdentidad(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas);
    }
    /// <summary>
    /// Devuelve la URL relacionada con la página de RIAM
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreCortoIdentidad">pNombreCortoOrg</param>
    /// <returns>URL semantizada</returns>
    public static string ObtenerUrlBaseIdentidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreCortoOrg)
    {
        return pBaseURL + GetUrlPerfil(pNombreCortoOrg, pUtilIdiomas);
    }

    /// <summary>
    /// Devuelve la URL relacionada con la página de RIAM
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLRiam(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "RIAMI+LLAB");
    }

    /// <summary>
    /// Devuelve la URL relacionada con la página de privacidad
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLPoliticaPrivacidad(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "POLITICAPRIVACIDAD");
    }

    /// <summary>
    /// Devuelve la URL relacionada con las condiciones de uso
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLCondicionesUso(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "CONDICIONESUSO");
    }

    /// <summary>
    /// Devuelve la URL relacionada con la página de contacto
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLContacto(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "CONTACTO");
    }

    /// <summary>
    /// Devuelve la URL relacionada con la página de acerca de gnoss
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAcercaGnoss(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "ACERCAGNOSS");
    }

    /// <summary>
    /// Devuelve la URL relacionada con la página de aprende con gnoss
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAprendeGnoss(string pBaseURL, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + "/" + pUtilIdiomas.GetText(URLSEM, "APRENDEGNOSS");
    }

    #endregion

    #region Administrar solicitudes

    /// <summary>
    /// Devuelve la URL relacionada con la administración de solicitudes de usuario
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pFilaSolicitud"></param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAdministrarSolicitudesUsuario(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, SolicitudNuevoUsuario pFilaSolicitud)
    {
        if (pFilaSolicitud == null)
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUDESUSUARIO");
        else
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUDESUSUARIO") + "/" + pFilaSolicitud.SolicitudID;
    }

    /// <summary>
    /// Devuelve la URL relacionada con la administración de solicitudes de organizacion
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pFilaSolicitud"></param>
    /// <param name="pEsClase">Verdad si las solicitudes que se quieren obtener son de clase</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAdministrarSolicitudesOrganizacion(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, SolicitudNuevaOrganizacion pFilaSolicitud, bool pEsClase)
    {
        string adminOrg = pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUDEORGANIZACION");

        if (pEsClase)
        {
            adminOrg = pUtilIdiomas.GetText(URLSEM, "ADMINSTRARSOLICITUDESCLASE");
        }

        if (pFilaSolicitud == null)
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + adminOrg;
        else
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + adminOrg + "/" + pFilaSolicitud.SolicitudID;
    }

    /// <summary>
    /// Devuelve la URL relacionada con la administración de solicitudes de usuario de acceso a proyecto
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pFilaSolicitud">Datarow de Solicitud (sin tipar ya que puede ser de varias tablas)</param>
    /// <param name="pNombreCortoProyecto">Comunidad donde se solicita el acceso</param>
    /// <returns>URL semantizada</returns>
    public string GetURLAdministrarSolicitudesAccesoProyecto(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, string pNombreCortoProyecto, SolicitudUsuario pFilaSolicitud)
    {
        if (pFilaSolicitud == null && string.IsNullOrEmpty(pNombreCortoProyecto))
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUACCESOPROYECTO");
        else
        {

            string comienzoUrl = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreCortoProyecto) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUACCESOPROYECTO");

            if (pFilaSolicitud == null)
                return comienzoUrl;
            else
                return comienzoUrl + "/" + pFilaSolicitud.SolicitudID.ToString();
        }
    }

    /// <summary>
    /// Devuelve la URL relacionada con la administración de solicitudes de usuario de acceso a proyecto (para organizaciones)
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pFilaSolicitud">Datarow de Solicitud (sin tipar ya que puede ser de varias tablas)</param>
    /// <param name="pNombreCortoProyecto">Comunidad donde se solicita el acceso</param>
    /// <returns>URL semantizada</returns>
    public string GetURLAdministrarSolicitudesAccesoProyectoOrganizacion(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto, SolicitudOrganizacion pFilaSolicitud)
    {
        string comienzoUrl = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreCortoProyecto) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUACCESOPROYECTOORG");

        if (pFilaSolicitud == null)
            return comienzoUrl;
        else
            return $"{comienzoUrl}/{pFilaSolicitud.SolicitudID}";
    }

    /// <summary>
    /// Devuelve la URL relacionada con la administración de solicitudes de comunidades
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pFilaPeticion"></param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAdministrarSolicitudesComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, PeticionNuevoProyecto pFilaPeticion)
    {
        if (pFilaPeticion == null)
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUDESCOMUNIDAD");
        else
            return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARSOLICITUDESCOMUNIDAD") + "/" + pFilaPeticion.PeticionID;
    }

    #endregion

    #region Bandeja mensajes

    /// <summary>
    /// Devuelve la URL relacionada con la bandeja de mensajes
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pTipoBandeja">Tipo de bandeja de correo</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLBandejaMensajes(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "MENSAJES");
    }

    /// <summary>
    /// Devuelve la URL para enviar un mensaje a un grupo
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pGrupoID">ID del grupo al que enviar el mensaje</param>
    /// <param name="pEsOrganizacion">TRUE si es una organización</param>
    public static string GetURLEnviarMensajeGrupo(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Guid pGrupoID, bool pEsOrganizacion)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        string texto = pUtilIdiomas.GetText("URLSEM", "ENVIARMENSAJES");

        if (pEsOrganizacion)
        {
            texto = pUtilIdiomas.GetText("URLSEM", "ENVIARMENSAJESORG");
        }

        return pBaseURL + pUrlPerfil + texto + "/" + pUtilIdiomas.GetText(URLSEM, "GRUPO") + "/" + pGrupoID.ToString();
    }

    /// <summary>
    /// Devuelve la URL para enviar mensajes a un destinatario.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidad">Identidad a la que mandamos el mensaje</param>
    public static string GetURLEnviarMensajeDestinatario(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        string url = pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "ENVIARMENSAJES");

        if (pIdentidad.TrabajaConOrganizacion)
        {
            url += "/" + pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoOrg;
        }
        if (pIdentidad.ModoPersonal)
        {

            url += "/" + pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoUsu;
        }

        return url;
    }

    #endregion

    #region Bandeja Subscripciones

    /// <summary>
    /// Devuelve la URL relacionada con la bandeja de subscripciones
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pTipoBandeja">Tipo de bandeja de correo</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLBandejaSuscripciones(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, string pTipoBandeja)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, pTipoBandeja);
    }

    /// <summary>
    /// Devuelve la URL relacionada con la bandeja de subscripciones
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pTipoBandeja">Tipo de bandeja de correo</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLBandejaSuscripciones(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, string pTipoBandeja, string pPagina)
    {
        string url = pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, pTipoBandeja);
        if (pPagina != null)
        {
            url += "/" + pUtilIdiomas.GetText("URLSEM", "PAGINA") + "/" + pPagina;
        }
        return url;
    }


    #endregion

    #region Aceptar invitaciones

    /// <summary>
    /// Devuelve la URL relacionada con la aceptación de una invitación
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLAceptarInvitacion(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURLIdioma + "/" + pUtilIdiomas.GetText(URLSEM, "ACEPTARINVITACION") + "/" + pUtilIdiomas.GetText(URLSEM, "PETICION");
    }

    /// <summary>
    /// Devuelve la URL relacionada con la aceptación de una invitación en un proyecto
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <returns>URL semantizada</returns>
    public string GetURLAceptarInvitacionEnProyecto(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCortoProyecto) + "/" + pUtilIdiomas.GetText(URLSEM, "ACEPTARINVITACION") + "/" + pUtilIdiomas.GetText(URLSEM, "PETICION");
    }

    /// <summary>
    /// Devuelve la URL relacionada con solicitar acceso a una comunidad.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pProyectoID">Nombre corto de la comunidad</param>
    /// <returns>URL semantizada</returns>
    public string GetURLSolicitarAccesoComunidad(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCortoProyecto) + "/" + pUtilIdiomas.GetText(URLSEM, "SOLICITARACCESO");
    }



    /// <summary>
    /// Devuelve la URL relacionada con hacerse miembro de una comunidad.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pProyectoID">Nombre corto de la comunidad</param>
    /// <returns>URL semantizada</returns>
    public string GetURLHacerseMiembroComunidad(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto)
    {
        return GetURLHacerseMiembroComunidad(pBaseURLIdioma, pUtilIdiomas, pNombreCortoProyecto, false);
    }

    /// <summary>
    /// Devuelve la URL relacionada con hacerse miembro de una comunidad.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreCortoProyecto">Nombre corto de la comunidad</param>
    /// <param name="pSoloLogin">Verdad si solo se quiere llevar a hacer login al usuario, falso si se le quiere preguntar si tiene cuenta en gnoss</param>
    /// <returns>URL semantizada</returns>
    public string GetURLHacerseMiembroComunidad(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto, bool pSoloLogin)
    {
        string url = ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCortoProyecto);

        if (pSoloLogin)
        {
            url += "/" + pUtilIdiomas.GetText(URLSEM, "LOGIN");
        }
        else
        {
            url += "/" + pUtilIdiomas.GetText(URLSEM, "HAZTEMIEMBRO");
        }

        return url;
    }

    /// <summary>
    /// Devuelve la URL relacionada con la aceptación de una invitación a un evento
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <returns>URL semantizada</returns>
    public string GetURLAceptarInvitacionEventoEnProyecto(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto, Guid pEventoID)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCortoProyecto) + "/" + pUtilIdiomas.GetText(URLSEM, "ACEPTARINVITACION") + "/" + pUtilIdiomas.GetText(URLSEM, "EVENTO") + "/" + pEventoID;
    }

    /// <summary>
    /// Devuelve la URL relacionada con la aceptación de una invitación a un evento
    /// </summary>
    /// <param name="pBaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <returns>URL semantizada</returns>
    public string GetURLEventoEnHomeProyecto(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProyecto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCortoProyecto) + "?ComponenteBienvenidaID=";
    }
    #endregion

    #region Invitaciones

    /// <summary>
    /// Devuelve la URL relacionada con la invitación a Gnoss
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLInvitacionGnoss(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "INVITARAGNOSS");
    }

    /// <summary>
    /// Devuelve la URL relacionada con la invitación a una comunidad en concreto
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pComunidad">Proyecto donde invitar</param>
    /// <returns>URL semantizada</returns>
    public string GetURLInvitacionComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreCortoComunidad)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreCortoComunidad) + "/" + pUtilIdiomas.GetText(URLSEM, "INVITARACOMUNIDAD");
    }

    public static string GetURLBandejaInvitaciones(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText("URLSEM", "INVITACIONES");
    }

    #endregion

    #region Bandeja Comentarios

    public static string GetURLBandejaComentarios(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText("URLSEM", "COMENTARIOS");
    }

    #endregion

    #region Bandeja Notificaciones

    public static string GetURLBandejaNotificaciones(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText("URLSEM", "NOTIFICACIONES");
    }

    #endregion

    #region Bios

    /// <summary>
    /// Devuelve la URL de una bio propia
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidad">Identidad a la que pertenece la bio</param>
    /// <param name="pBioID">Identificador la bio</param>
    public string GetURLBioUrlPropia(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad, Guid pBioID)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        string ruta = "";

        if (pIdentidad.ElementoPublico is Organizacion)
        {
            ruta = ObtenerURLOrganizacionOClase(pUtilIdiomas, pIdentidad.OrganizacionID.Value) + "/" + pIdentidad.PerfilUsuario.NombreCortoOrg + "/";
        }
        else
        {
            ruta = pUtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + pIdentidad.PerfilUsuario.NombreCortoUsu + "/";
        }

        ruta += pUtilIdiomas.GetText("URLSEM", "VERBIO") + "/" + pBioID;

        return pBaseURL + pUrlPerfil + ruta;
    }

    /// <summary>
    /// Devuelve la URL para enviar una url de bio por correo
    /// </summary>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidad">Identidad a la que pertenece la bio</param>
    /// <param name="pBioID">Identificador la bio</param>
    /// <param name="pNombreSemBio">Nombre corto de la bio</param>
    public string GetURLBioUrlPropiaEnviarPorCorreo(UtilIdiomas pUtilIdiomas, Identidad pIdentidad, Guid pBioID, string pNombreSemBio)
    {
        string ruta = "";


        if (pIdentidad.ElementoPublico is Organizacion)
        {
            ruta = ObtenerURLOrganizacionOClase(pUtilIdiomas, pIdentidad.OrganizacionID.Value) + "/" + pIdentidad.PerfilUsuario.NombreCortoOrg + "/";
        }
        else
        {
            ruta = pUtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + pIdentidad.PerfilUsuario.NombreCortoUsu + "/";
        }

        ruta += pUtilIdiomas.GetText("URLSEM", "VERBIO") + "/" + pBioID;

        return "Ficha_EnviarPorCorreo&" + ruta + "&" + pNombreSemBio + "&" + "";
    }

    #endregion

    #region Documentación

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organización si la hay</param>
    public string GetURLBaseRecursos(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursos(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pIdentidadOrganizacion, null);
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organización si la hay</param>
    public string GetURLBaseRecursos(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion, string pNombreOntoSem)
    {
        return GetURLBaseRecursos(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pIdentidadOrganizacion, pNombreOntoSem, -1);
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento semántico (Tiene que existir la pestaña).
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organización si la hay</param>
    public string GetURLBaseRecursos(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion, string pNombreOntoSem, short pTipoBusqueda)
    {
        if (!string.IsNullOrEmpty(pNombreProy) && pNombreProy != pUtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA"))
        {
            string pagina = pUtilIdiomas.GetText(URLSEM, "RECURSOS");
            if (!string.IsNullOrEmpty(pNombreOntoSem))
            {
                pagina = pNombreOntoSem;
            }

            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pagina;
        }
        else if (pIdentidadOrganizacion)
        {
            string url = pBaseURL + pUrlPerfil;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return url + pUtilIdiomas.GetText(URLSEM, "RECURSOSORG");
        }
        else if (pTipoBusqueda != -1 && (TipoBusqueda)pTipoBusqueda == TipoBusqueda.Contribuciones)
        {
            string url = pBaseURL + pUrlPerfil;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return url + pUtilIdiomas.GetText(URLSEM, "MISCONTRIBUCIONES");
        }
        else
        {
            string url = pBaseURL + pUrlPerfil;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return url + pUtilIdiomas.GetText(URLSEM, "MISRECURSOS");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreCortoProy">Nombre corto del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pDocumento">Documento para ver</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosFicha(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreCortoProy, string pUrlPerfil, Documento pDocumento, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosFichaConIDs(pBaseURL, pUtilIdiomas, pNombreCortoProy, pUrlPerfil, pDocumento.Titulo, pDocumento.Clave, pDocumento.ElementoVinculadoID, pDocumento.TipoDocumentacion, pIdentidadOrganizacion);
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento</param>
    /// <param name="pDocumentoID">Documento para ver</param>
    /// <param name="pOntologiaID">Identificador de la ontologia para construir la URL (NULL si no pertenece a una ontología)</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosFichaConIDs(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreDocumento, Guid pDocumentoID, Guid? pOntologiaID, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosFichaConIDs(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pNombreDocumento, pDocumentoID, pOntologiaID, TiposDocumentacion.Nota, pIdentidadOrganizacion);
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento</param>
    /// <param name="pDocumentoID">Documento para ver</param>   
    /// <param name="pOntologia">Nombre de la ontologia (*.owl) para construir la URL (NULL si no pertenece a una ontología) </param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosFichaConIDs(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreDocumento, string pOntologia, Guid pDocumentoID, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosFichaConIDs(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pNombreDocumento, pOntologia, pDocumentoID, TiposDocumentacion.Nota, pIdentidadOrganizacion);
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento (null para obtenerlo desde la base de datos)</param>
    /// <param name="pDocumentoID">Documento para ver</param>
    /// <param name="pOntologiaID">Identificador de la ontologia para construir la URL (NULL si no pertenece a una ontología)</param>
    /// <param name="pTipoDocumento">Tipo de documento</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosFichaConIDs(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreDocumento, Guid pDocumentoID, Guid? pOntologiaID, TiposDocumentacion pTipoDocumento, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosFichaConIDs(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pNombreDocumento, pDocumentoID, pOntologiaID, null, pTipoDocumento, null, pIdentidadOrganizacion);
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento (null para obtenerlo desde la base de datos)</param>
    /// <param name="pOntologia">Nombre de la ontologia (*.owl) para construir la URL (NULL si no pertenece a una ontología) </param>
    /// <param name="pDocumentoID">Documento para ver</param>        
    /// <param name="pTipoDocumento">Tipo de documento</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosFichaConIDs(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreDocumento, string pOntologia, Guid pDocumentoID, TiposDocumentacion pTipoDocumento, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosFichaConIDs(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pNombreDocumento, pDocumentoID, null, pOntologia, pTipoDocumento, null, pIdentidadOrganizacion);
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento (null para obtenerlo desde la base de datos)</param>
    /// <param name="pDocumentoID">Documento para ver</param>
    /// <param name="pOntologiaID">Identificador de la ontologia para construir la URL (NULL si no pertenece a una ontología)</param>
    /// <param name="pOntologia">Nombre de la ontologia (*.owl) para construir la URL (NULL si no pertenece a una ontología) (si se especifica pOntologiaID no hace falta especificar este campo)</param>
    /// <param name="pTipoDocumento">Tipo de documento</param>
    /// <param name="pFicheroConfiguracion">Fichero de configuración que debe usarse</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosFichaConIDs(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreDocumento, Guid pDocumentoID, Guid? pOntologiaID, string pOntologia, TiposDocumentacion pTipoDocumento, string pFicheroConfiguracion, bool pIdentidadOrganizacion)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        string claveTipoDoc = "RECURSO";
        if (pTipoDocumento == TiposDocumentacion.Pregunta)
        {
            claveTipoDoc = "PREGUNTA";
        }
        else if (pTipoDocumento == TiposDocumentacion.Debate)
        {
            claveTipoDoc = "DEBATE";
        }
        else if (pTipoDocumento == TiposDocumentacion.Encuesta)
        {
            claveTipoDoc = "ENCUESTA";
        }

        if (pIdentidadOrganizacion)
        {
            claveTipoDoc += "ORG";
        }

        string tipoDoc = pUtilIdiomas.GetText(URLSEM, claveTipoDoc);

        if (pOntologiaID.HasValue && !pOntologiaID.Equals(Guid.Empty))
        {
            tipoDoc = NombreCortoOntologia(pOntologiaID.Value, pNombreProy, pUtilIdiomas);
        }
        else if (!string.IsNullOrEmpty(pOntologia))
        {
            tipoDoc = NombreCortoOntologia(pOntologia, pNombreProy, pUtilIdiomas);
        }

        if (string.IsNullOrEmpty(pNombreDocumento))
        {
            //Obtengo el nombre del documento
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            pNombreDocumento = docCN.ObtenerTituloDocumentoPorID(pDocumentoID);
            docCN.Dispose();
        }

        pNombreDocumento = UtilCadenas.ObtenerTextoDeIdioma(pNombreDocumento, pUtilIdiomas.LanguageCode, null);
        pNombreDocumento = UtilCadenas.EliminarCaracteresUrlSem(pNombreDocumento);

        //Elimino los puntos del final:
        pNombreDocumento = pNombreDocumento.Trim().TrimEnd('.');

        if (string.IsNullOrEmpty(pNombreDocumento))
        {
            pNombreDocumento = "no-name";
        }

        if (!string.IsNullOrEmpty(pNombreProy))
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy, pFicheroConfiguracion) + "/" + tipoDoc + "/" + pNombreDocumento + "/" + pDocumentoID.ToString();
        }
        else
        {
            if (!pUtilIdiomas.LanguageCode.Equals("es") && !pBaseURL.EndsWith(pUtilIdiomas.LanguageCode))
            {
                pBaseURL += "/" + pUtilIdiomas.LanguageCode;
            }
            return pBaseURL + pUrlPerfil + tipoDoc + "/" + pNombreDocumento + "/" + pDocumentoID.ToString();
        }
    }

    private string NombreCortoOntologia(string pOntologia, string pNombreCortoProy, UtilIdiomas pUtilIdiomas)
    {
        if (!mListaIDsOntologiasPorProyecto.ContainsKey(pNombreCortoProy))
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<string, Guid> idOntologiasDeProyecto = proyCN.ObtenerOntologiasConIDPorNombreCortoProy(pNombreCortoProy);
            proyCN.Dispose();

            lock (BLOQUEO_ListaIDsOntologiasPorProyecto)
            {
                if (!mListaIDsOntologiasPorProyecto.ContainsKey(pNombreCortoProy))
                {
                    mListaIDsOntologiasPorProyecto.TryAdd(pNombreCortoProy, idOntologiasDeProyecto);
                }
            }
        }
        Guid idOntologia = Guid.Empty;
        if (mListaIDsOntologiasPorProyecto.ContainsKey(pNombreCortoProy) && mListaIDsOntologiasPorProyecto[pNombreCortoProy].ContainsKey(pOntologia))
        {
            idOntologia = mListaIDsOntologiasPorProyecto[pNombreCortoProy][pOntologia];
        }
        return NombreCortoOntologia(idOntologia, pNombreCortoProy, pUtilIdiomas);
    }

    private string NombreCortoOntologia(Guid pOntologiaID, string pNombreCortoProy, UtilIdiomas pUtilIdiomas)
    {
        if (!mListaURLOntologiasPorProyecto.ContainsKey(pNombreCortoProy))
        {
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Dictionary<string, Dictionary<Guid, string>> urlOntologiasDeProyecto = proyCN.ObtenerNombresCortosProyectosConNombresCortosOntologias(pNombreCortoProy);
            proyCN.Dispose();

            lock (BLOQUEO_ListaURLOntologiasPorProyecto)
            {
                if (!mListaURLOntologiasPorProyecto.ContainsKey(pNombreCortoProy))
                {
                    if (urlOntologiasDeProyecto.ContainsKey(pNombreCortoProy))
                    {
                        mListaURLOntologiasPorProyecto.TryAdd(pNombreCortoProy, urlOntologiasDeProyecto[pNombreCortoProy]);
                    }
                    else
                    {
                        mListaURLOntologiasPorProyecto.TryAdd(pNombreCortoProy, new Dictionary<Guid, string>());
                    }
                }
            }
        }
        if (mListaURLOntologiasPorProyecto.ContainsKey(pNombreCortoProy) && mListaURLOntologiasPorProyecto[pNombreCortoProy].ContainsKey(pOntologiaID))
        {
            return UtilCadenas.ObtenerTextoDeIdioma(mListaURLOntologiasPorProyecto[pNombreCortoProy][pOntologiaID], pUtilIdiomas.LanguageCode, null);
        }
        else
        {
            return pUtilIdiomas.GetText(URLSEM, "RECURSO");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de myGnoss de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidad">Identidad de la ficha</param>
    /// <param name="pDocumento">Documento para ver</param>
    public static string GetURLBaseRecursosFichaMyGnoss(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad, Documento pDocumento)
    {
        string urlRedirect = pBaseURL + pUrlPerfil;

        if (pIdentidad.TrabajaConOrganizacion)
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoOrg + "/";
        }
        if (pIdentidad.ModoPersonal)
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoUsu + "/";
        }
        urlRedirect += pUtilIdiomas.GetText(URLSEM, "RECURSO") + "/" + pDocumento.NombreSem(pUtilIdiomas.LanguageCode) + "/" + pDocumento.Clave;

        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de myGnoss de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidad">Contiene la Identidad del propietario/creador del recurso</param>
    /// <param name="pNombreSemDocumento">Nombre parseado para las url semanticas del documento</param>
    /// <param name="pDocumentoID">Id del documento</param>
    public static string GetURLBaseRecursosFichaMyGnoss(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad, string pNombreSemDocumento, Guid pDocumentoID)
    {
        string urlRedirect = pBaseURL + pUrlPerfil;

        if (pIdentidad.TrabajaConOrganizacion)
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoOrg;
        }
        if (pIdentidad.ModoPersonal)
        {

            urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoUsu;
        }
        urlRedirect += "/" + pUtilIdiomas.GetText(URLSEM, "RECURSO") + "/" + pNombreSemDocumento + "/" + pDocumentoID.ToString();

        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de myGnoss de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreCortoUsu">Nombre corto del usuario</param>
    /// <param name="pNombreSemDocumento">Nombre parseado para las url semanticas del documento</param>
    /// <param name="pDocumentoID">Id del documento</param>
    public static string GetURLBaseRecursosFichaMyGnossDePersona(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreCortoUsu, string pNombreSemDocumento, Guid pDocumentoID)
    {
        string urlRedirect = pBaseURL + pUrlPerfil;
        urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pNombreCortoUsu;

        if (pNombreSemDocumento != null && pDocumentoID != Guid.Empty)
        {
            urlRedirect += "/" + pUtilIdiomas.GetText(URLSEM, "RECURSO") + "/" + pNombreSemDocumento + "/" + pDocumentoID.ToString();
        }
        else
        {
            urlRedirect += "/" + pUtilIdiomas.GetText(URLSEM, "RECURSOS");
        }

        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de myGnoss de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreCortoUsu">Nombre corto del usuario</param>
    /// <param name="pNombreSemDocumento">Nombre parseado para las url semanticas del documento</param>
    /// <param name="pDocumentoID">Id del documento</param>
    public static string GetURLBaseRecursosFichaMyGnossDePersonaEnviarEnlace(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreCortoUsu, string pNombreSemDocumento, Guid pDocumentoID)
    {
        return GetURLBaseRecursosFichaMyGnossDePersona(pBaseURL, pUtilIdiomas, pUrlPerfil, pNombreCortoUsu, pNombreSemDocumento, pDocumentoID) + "/" + pUtilIdiomas.GetText(URLSEM, "ENVIARPORCORREO");
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de myGnoss de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidad">Identidad de la organización</param>
    /// <param name="pNombreSemDocumento">Nombre parseado para las url semanticas del documento</param>
    /// <param name="pDocumentoID">Id del documento</param>
    public static string GetURLBaseRecursosFichaMyGnossDeOrganizacionEnviarEnlace(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad, string pNombreSemDocumento, Guid pDocumentoID)
    {
        return GetURLBaseRecursosFichaMyGnoss(pBaseURL, pUtilIdiomas, pUrlPerfil, pIdentidad, pNombreSemDocumento, pDocumentoID) + "/" + pUtilIdiomas.GetText(URLSEM, "ENVIARPORCORREO");
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pURLPredef">Url predefinida</param>
    /// <param name="pDocumento">Documento para ver</param>
    public static string GetURLBaseRecursosFichaConURLPredefinida(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pURLPredef, Documento pDocumento, bool pEsUsuarioInvitado)
    {
        if (pEsUsuarioInvitado)
        {
            return GetURLBaseRecursosRecursoInvitado(pBaseURL, pUrlPerfil, pUtilIdiomas, pDocumento);
        }
        else
        {
            string concatenador = "/";

            if (!pURLPredef.Contains(pUtilIdiomas.GetText("URLSEM", "MIPERFILRECURSOS")))
            {
                pURLPredef = pURLPredef.Replace(pUtilIdiomas.GetText("URLSEM", "RECURSOSORG"), "").Replace(pUtilIdiomas.GetText("URLSEM", "MISRECURSOS"), "").Replace(pUtilIdiomas.GetText("URLSEM", "RECURSOS"), "");
                concatenador = "";
            }
            return pBaseURL + pUrlPerfil + pURLPredef + concatenador + pUtilIdiomas.GetText("URLSEM", "RECURSO") + "/" + pDocumento.NombreSem(pUtilIdiomas.LanguageCode) + "/" + pDocumento.Clave;
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a enviar un enlace de un recuso.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento (null para obtenerlo desde la base de datos)</param>
    /// <param name="pDocumentoID">Documento para ver</param>
    /// <param name="pTipoDocumento">Tipo de documento</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosEnviarEnlace(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreDocumento, Guid pDocumentoID, TiposDocumentacion pTipoDocumento, bool pIdentidadOrganizacion)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        string claveTipoDoc = "RECURSO";
        if (pTipoDocumento == TiposDocumentacion.Pregunta)
        {
            claveTipoDoc = "PREGUNTA";
        }
        else if (pTipoDocumento == TiposDocumentacion.Debate)
        {
            claveTipoDoc = "DEBATE";
        }
        else if (pTipoDocumento == TiposDocumentacion.Encuesta)
        {
            claveTipoDoc = "ENCUESTA";
        }

        if (pIdentidadOrganizacion)
        {
            claveTipoDoc += "ORG";
        }

        string tipoDoc = pUtilIdiomas.GetText(URLSEM, claveTipoDoc);

        if (string.IsNullOrEmpty(pNombreDocumento))
        {
            //Obtengo el nombre del documento
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            pNombreDocumento = docCN.ObtenerTituloDocumentoPorID(pDocumentoID);

            pNombreDocumento = UtilCadenas.EliminarCaracteresUrlSem(pNombreDocumento);

            //Elimino los puntos del final:
            while (pNombreDocumento.Length > 0 && pNombreDocumento[pNombreDocumento.Length - 1] == '.')
            {
                pNombreDocumento = pNombreDocumento.Remove(pNombreDocumento.Length - 1);
            }

            if (string.IsNullOrEmpty(pNombreDocumento))
            {
                pNombreDocumento = "no-name";
            }
        }

        if (pNombreProy != null && pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + tipoDoc + "/" + pNombreDocumento + "/" + pDocumentoID.ToString() + "/" + pUtilIdiomas.GetText(URLSEM, "ENVIARPORCORREO");
        }
        else
        {
            return pBaseURL + pUrlPerfil + tipoDoc + "/" + pNombreDocumento + "/" + pDocumentoID.ToString() + "/" + pUtilIdiomas.GetText(URLSEM, "ENVIARPORCORREO");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento como usuario invitado.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumento">Documento para ver</param>
    public static string GetURLBaseRecursosRecursoInvitado(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas, Documento pDocumento)
    {
        return GetURLBaseRecursosRecursoInvitadoConIDS(pBaseURL, pUrlPerfil, pUtilIdiomas, pDocumento.NombreSem(pUtilIdiomas.LanguageCode), pDocumento.Clave);
    }

    /// <summary>
    /// Devuelve la URL para ir a la ficha de un documento como usuario invitado.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pNombreDocumento">Nombre sem del documento</param>
    /// <param name="pDocumentoID">ID del documento</param>
    public static string GetURLBaseRecursosRecursoInvitadoConIDS(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas, string pNombreDocumento, Guid pDocumentoID)
    {
        if (string.IsNullOrEmpty(pUrlPerfil))
        {
            pUrlPerfil = "/";
        }
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "RECURSOINVITADO") + "/" + pNombreDocumento + "/" + pDocumentoID;
    }

    /// <summary>
    /// Devuelve la URL para ir a recuperar un documento temporal.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumento">Documento para ver</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public static string GetURLBaseRecursosRecuperarArchivoTemporal(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas, Documento pDocumento, bool pIdentidadOrganizacion)
    {
        if (pIdentidadOrganizacion)
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSOTEMPORALORG") + "/" + pDocumento.NombreSem(pUtilIdiomas.LanguageCode) + "/" + pDocumento.Clave;
        }
        else
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSOTEMPORAL") + "/" + pDocumento.NombreSem(pUtilIdiomas.LanguageCode) + "/" + pDocumento.Clave;
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a editar un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumento">Documento para ver</param>
    /// <param name="pCreandoNuevaVersion">Indica 0 si no hay versión, 1 si hay que crear versión, 2 si la versión ha fallado</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    public string GetURLBaseRecursosEditarDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Documento pDocumento, int pCreandoNuevaVersion, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosEditarDocumento(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pDocumento.NombreSem(pUtilIdiomas.LanguageCode), pCreandoNuevaVersion, pIdentidadOrganizacion, pDocumento.Clave);
    }

    /// <summary>
    /// Devuelve la URL para ir a editar un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreSemDocumento"></param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumento">Documento para ver</param>
    /// <param name="pCreandoNuevaVersion">Indica 0 si no hay versión, 1 si hay que crear versión, 2 si la versión ha fallado</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    /// <param name="pDocumentoID">ID del documento</param>
    public string GetURLBaseRecursosEditarDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreSemDocumento, int pCreandoNuevaVersion, bool pIdentidadOrganizacion, Guid pDocumentoID)
    {
        string direccion = "";
        string claveDirecc = "";

        if (pCreandoNuevaVersion == 0)
        {
            claveDirecc = "EDITARRECURSO";
        }
        else if (pCreandoNuevaVersion == 1)
        {
            claveDirecc = "VERSIONANDORECURSO";
        }
        else if (pCreandoNuevaVersion == 2)
        {
            claveDirecc = "FALLOVERSIONANDORECURSO";
        }

        if (pIdentidadOrganizacion)
        {
            claveDirecc += "ORG";
        }

        if (string.IsNullOrEmpty(pNombreProy) || pNombreProy.ToLower().Equals("mygnoss"))
        {
            claveDirecc += "GNOSS";
        }

        direccion = pUtilIdiomas.GetText(URLSEM, claveDirecc);

        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + direccion + "/" + pNombreSemDocumento + "/" + pDocumentoID.ToString();

        }
        else
        {
            return pBaseURL + pUrlPerfil + direccion + "/" + pNombreSemDocumento + "/" + pDocumentoID.ToString();
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a subir un nuevo documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    public string GetURLBaseRecursosSubir(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion)
    {
        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSO");
        }
        else
        {
            if (pIdentidadOrganizacion)
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSOORG");
            }
            else
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSO");
            }
        }
    }

    public static string GetURLAdministrarPerfilCategorias(string pBaseURLIdioma, string pUrlPerfil, UtilIdiomas pUtilIdiomas, bool pIdentidadOrganizacion)
    {
        if (!pIdentidadOrganizacion)
        {
            return pBaseURLIdioma + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCATEGORIASPERFIL");
        }
        else
        {
            return pBaseURLIdioma + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCATEGORIASPERFILORG");
        }
    }

    public static string GetURLImportarDelicious(string pBaseURLIdioma, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURLIdioma + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "IMPORTARDELICIOUS");
    }

    /// <summary>
    /// Devuelve la URL para ir a subir un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pTipoDoc">Tipo del documento</param>
    /// <param name="pElementoVinculadoID">Elemento vinculado al documento</param>
    public string GetURLBaseRecursosSubirConAtributos(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pTipoDoc, string pElementoVinculadoID)
    {
        string parametrosExtra = "";

        if (pTipoDoc != null)
        {
            parametrosExtra = "/" + pDocumentoID.ToString() + "/" + pTipoDoc;

            if (pElementoVinculadoID != null)
            {
                parametrosExtra += "/" + pElementoVinculadoID;
            }
        }

        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSOCREADO") + parametrosExtra;
        }
        else
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "SUBIRRECURSOCREADO") + parametrosExtra;
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a subir un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pTipoDoc">Tipo del documento</param>
    /// <param name="pElementoVinculadoID">Elemento vinculado al documento</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLBaseRecursosSubirRecurso(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pTipoDoc, string pSize, bool pIdentidadOrganizacion)
    {
        string parametrosExtra = "";

        if (pTipoDoc != null)
        {
            parametrosExtra = "/" + pDocumentoID.ToString() + "/" + pTipoDoc;
            if (pSize != null)
            {
                parametrosExtra += "/" + pSize;
            }
        }

        string claveSubirRec = "SUBIRRECURSO2";

        if (pTipoDoc == ((short)TiposDocumentacion.Pregunta).ToString())
        {
            claveSubirRec = "SUBIRPREGUNTA";
        }
        else if (pTipoDoc == ((short)TiposDocumentacion.Encuesta).ToString())
        {
            claveSubirRec = "SUBIRENCUESTA";
        }
        else if (pTipoDoc == ((short)TiposDocumentacion.Debate).ToString())
        {
            claveSubirRec = "SUBIRDEBATE";
        }

        if (pIdentidadOrganizacion)
        {
            claveSubirRec += "ORG";
        }

        if (string.IsNullOrEmpty(pNombreProy) || pNombreProy.ToLower().Equals("mygnoss"))
        {
            claveSubirRec += "GNOSS";
        }

        string subirRecurso = pUtilIdiomas.GetText(URLSEM, claveSubirRec);

        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + subirRecurso + parametrosExtra;
        }
        else
        {
            return pBaseURL + pUrlPerfil + subirRecurso + parametrosExtra;
        }
    }

    /// <summary>
    /// Devuelve la URL para ir crear una versión de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pElementoVinculadoID">Elemento vinculado al documento</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLBaseRecursosCrearDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pOntologiaID, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosCrearDocumento(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pDocumentoID, pOntologiaID, pIdentidadOrganizacion, false);
    }

    /// <summary>
    /// Devuelve la URL para ir crear una versión de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pElementoVinculadoID">Elemento vinculado al documento</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLBaseRecursosCrearDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pOntologiaID, bool pIdentidadOrganizacion, bool pVirtual)
    {
        string parametrosExtra = "";

        if (pOntologiaID != null)
        {
            parametrosExtra = "/" + pDocumentoID.ToString() + "/" + pOntologiaID;
        }

        if (pNombreProy != "")
        {
            if (pVirtual)
            {
                return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTOVIRTUAL") + parametrosExtra;
            }
            else
            {
                return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTO") + parametrosExtra;
            }
        }
        else
        {
            if (pVirtual)
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTOVIRTUAL") + parametrosExtra;
            }
            else
            {
                if (pIdentidadOrganizacion)
                {
                    return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTOORG") + parametrosExtra;
                }
                else
                {
                    return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTO") + parametrosExtra;
                }
            }
        }
    }

    /// <summary>
    /// Devuelve la URL para ir crear una versión de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pElementoVinculadoID">Elemento vinculado al documento</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLBaseRecursosCrearDocumentoMultiple(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pOntologiaID, bool pIdentidadOrganizacion)
    {
        string parametrosExtra = "";

        if (pOntologiaID != null)
        {
            parametrosExtra = "/" + pDocumentoID.ToString() + "/" + pOntologiaID;
        }

        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTOMULTIPLE") + parametrosExtra;
        }
        else
        {
            if (pIdentidadOrganizacion)
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTOORGMULTIPLE") + parametrosExtra;
            }
            else
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "CREARDOCUMENTOMULTIPLE") + parametrosExtra;
            }
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a ver un documento creado.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pOntologiaID">Identificador de la ontología del doc semántico</param>
    /// <param name="pEditarDirectamente">Indica si el doc Sem se debe editar directamente sin pasar por la vista previa</param>
    /// <param name="pIdentidadOrganizacion">Indica si hay identidad de la organzación o no</param>
    public string GetURLBaseRecursosVerDocumentoCreado(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pOntologiaID, bool pEditarDirectamente, bool pIdentidadOrganizacion)
    {
        return GetURLBaseRecursosVerDocumentoCreado(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pDocumentoID, pOntologiaID, pEditarDirectamente, pIdentidadOrganizacion, false);
    }

    /// <summary>
    /// Devuelve la URL para ir a ver un documento creado.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pOntologiaID">Identificador de la ontología del doc semántico</param>
    /// <param name="pEditarDirectamente">Indica si el doc Sem se debe editar directamente sin pasar por la vista previa</param>
    /// <param name="pIdentidadOrganizacion">Indica si hay identidad de la organzación o no</param>
    /// <param name="pVirtual">Indica si es virtual</param>
    public string GetURLBaseRecursosVerDocumentoCreado(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pOntologiaID, bool pEditarDirectamente, bool pIdentidadOrganizacion, bool pVirtual)
    {
        string parametrosExtra = "";

        if (pOntologiaID != null)
        {
            parametrosExtra = "/" + pDocumentoID.ToString() + "/" + pOntologiaID;
        }

        string clavePag = "VERDOCUMENTOCREADO";

        if (pEditarDirectamente)
        {
            clavePag = "EDITARDOCUMENTOCREADO";
        }
        if (pVirtual)
        {
            clavePag = "EDITARDOCUMENTOVIRTUAL";
        }

        if (pIdentidadOrganizacion)
        {
            clavePag += "ORG";
        }

        string pagina = pUtilIdiomas.GetText(URLSEM, clavePag);

        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pagina + parametrosExtra;
        }
        else
        {
            return pBaseURL + pUrlPerfil + pagina + parametrosExtra;
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a versionar un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Indica si hay identidad de la organzación o no</param>
    /// <param name="pDocumentoID">Identificador del documento</param>
    /// <param name="pOntologiaID">Identificador de la ontología del doc semántico</param>
    /// <param name="pDocumentoOriginalID">Documento del cual se hace la versión</param>
    /// <param name="pIdentidadOrganizacion">Indica si hay identidad de la organzación o no</param>
    public string GetURLBaseRecursosVersionarDocumentoCreado(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, Guid pDocumentoID, string pOntologiaID, Guid pDocumentoOriginalID, bool pIdentidadOrganizacion)
    {
        string parametrosExtra = "";

        if (pOntologiaID != null)
        {
            parametrosExtra = "/" + pDocumentoID.ToString() + "/" + pOntologiaID + "/" + pDocumentoOriginalID;
        }

        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "VERSIONARDOCUMENTOCREADO") + parametrosExtra;
        }
        else
        {
            if (pIdentidadOrganizacion)
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "VERSIONARDOCUMENTOCREADOORG") + parametrosExtra;
            }
            else
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "VERSIONARDOCUMENTOCREADO") + parametrosExtra;
            }
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a el tag un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pTag">Tag por el que se deberá realizar un búsqueda</param>
    public string GetURLBaseRecursosTag(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion, string pTag)
    {
        string tag = "/";

        if (pTag != null)
        {
            tag = "/" + pTag;
            tag = SustituirCaracteresTagBusqueda(tag);
        }

        if (pNombreProy != null && pNombreProy != "")
        {
            //ObtenerURLComunidad(
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "RECURSOS") + "/" + pUtilIdiomas.GetText(URLSEM, "TAG") + tag;
        }
        else if (pIdentidadOrganizacion)
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "RECURSOSORG") + "/" + pUtilIdiomas.GetText(URLSEM, "TAG") + tag;
        }
        else
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "MISRECURSOS") + "/" + pUtilIdiomas.GetText(URLSEM, "TAG") + tag;
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a el tag de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pTag">Tag por el que se deberá realizar un búsqueda</param>
    public static string GetURLBaseRecursosTagConURLPredefinida(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pUrlPredefinida, string pTag)
    {
        string tag = "/";

        if (pTag != null)
        {
            tag = "/" + pTag;
            tag = SustituirCaracteresTagBusqueda(tag);
        }
        return pBaseURL + pUrlPerfil + pUrlPredefinida + "/" + pUtilIdiomas.GetText("URLSEM", "TAG") + tag;
    }



    /// <summary>
    /// Devuelve la URL para ir a el tag de un documento de contribuciones.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pTag">Tag por el que se deberá realizar un búsqueda</param>
    public string GetURLBaseRecursosTagEnContribuciones(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pTag)
    {
        return GetURLTagEnBusquedaAvanzada(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pTag);
    }

    /// <summary>
    /// Devuelve la URL para ir a la categoría de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pCategoria">Categoría del tesauro</param>
    /// <param name="pMyGnoss">Indica si la categoría pertenece al tesauro de MyGnoss</param>
    public string GetURLBaseRecursosCategoriaDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion, CategoriaTesauro pCategoria, string pPestanya)
    {
        return GetURLDetalleCategoria(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pCategoria.NombreSem[pUtilIdiomas.LanguageCode], pCategoria.Clave, pIdentidadOrganizacion, null, pPestanya);
    }

    /// <summary>
    /// Devuelve la URL para ir a la categoría de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pNombreCategoria">Nombre Sem de la categoría</param>
    /// <param name="pCategoriaID">Identificador de la categoría</param>
    /// <param name="pMyGnoss">Indica si la categoría pertenece al tesauro de MyGnoss</param>
    public string GetURLBaseRecursosCategoriaDocumentoConIDs(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion, string pNombreCategoria, Guid pCategoriaID, string pPestanya)
    {
        return GetURLDetalleCategoria(pBaseURL, pUtilIdiomas, pNombreProy, pUrlPerfil, pNombreCategoria, pCategoriaID, pIdentidadOrganizacion, null, pPestanya);
    }

    ///// <summary>
    ///// Devuelve la URL para ir a una categoria del tesauro personal de una identidad.
    ///// </summary>
    ///// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    ///// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    ///// <param name="pUrlPerfil">URL del perfil de la persona</param>
    ///// <param name="pIdentidadAutor">Identidad del autor de la categoria</param>
    ///// <param name="pNombreCategoria">Nombre Sem de la categoría</param>
    ///// <param name="pCategoriaID">Identificador de la categoría</param>
    //public static string GetURLBaseRecursosCategoriaDocumentoConIDsDeMyGnoss(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidadAutor, string pNombreCategoria, Guid pCategoriaID)
    //{
    //    string urlRedirect = pBaseURL + pUrlPerfil;

    //    if (pIdentidadAutor.TrabajaConOrganizacion)
    //    {
    //        urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pIdentidadAutor.PerfilUsuario.FilaPerfil.NombreCortoOrg;
    //    }

    //    if (pIdentidadAutor.ModoPersonal)
    //    {
    //        urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pIdentidadAutor.PerfilUsuario.FilaPerfil.NombreCortoUsu;
    //    }
    //    urlRedirect += "/" + pUtilIdiomas.GetText(URLSEM, "RECURSOS") + "/" + pUtilIdiomas.GetText(URLSEM, "CATEGORIA") + "/" + pNombreCategoria + "/" + pCategoriaID.ToString();

    //    return urlRedirect;
    //}

    public string ObtenerUrlMiPerfil(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil)
    {
        string url = "";

        if (pNombreProy != "")
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy);
        }
        else if (pUrlPerfil != "/")
        {
            url = pBaseURL + pUrlPerfil;
        }

        if (!url.EndsWith("/"))
        {
            url += "/";
        }

        return url + pUtilIdiomas.GetText(URLSEM, "MIPERFIL");
    }

    /// <summary>
    /// Devuelve la URL para ir a la categoría de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    /// <param name="pNombreCategoria">Nombre Sem de la categoría</param>
    /// <param name="pCategoriaID">Identificador de la categoría</param>
    public string GetURLBaseRecursosCategoriaDocumentoConUrlPropia(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreCategoria, Guid pCategoriaID, string pURLPropia)
    {
        return pBaseURL + pUrlPerfil + pURLPropia + "/" + pUtilIdiomas.GetText("URLSEM", "CATEGORIA") + "/" + pNombreCategoria + "/" + pCategoriaID.ToString();
    }

    /// <summary>
    /// Devuelve la URL para ir al incio de base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLBaseRecursosInicio(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, bool pIdentidadOrganizacion)
    {
        if (pNombreProy != "")
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy);
        }
        else if (pIdentidadOrganizacion)
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "PERFIL");
        }
        else
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "MIPERFIL");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a ver el historial de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreProy">Nombre del proyectos donde se desea ir</param>
    /// <param name="pNombreDocumento">Nombre semantico del documento</param>
    /// <param name="pDocumentoID">Documento para ver</param>
    /// <param name="pEntidadVinculada">Entidad a la que pertenece el documento, NULL si es un documento vinculado a la WEB</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    /// <returns>URL</returns>
    public string GetURLHistorialDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreProy, string pNombreDocumento, Guid pDocumentoID, bool pIdentidadOrganizacion)
    {
        string url = "";

        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        if (pNombreProy != null && pNombreProy != "")
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
        }
        else
        {
            url = pBaseURL + pUrlPerfil;
        }

        string pagina = "HISTORIALVERSIONES";

        if (pIdentidadOrganizacion)
        {
            pagina += "ORG";
        }

        return url + pUtilIdiomas.GetText(URLSEM, pagina) + "/" + pNombreDocumento + "/" + pDocumentoID;
    }

    /// <summary>
    /// Devuelve la URL para ir a ver una comparación entre dos versiones de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreProy">Nombre del proyectos donde se desea ir</param>
    /// <param name="pEntidadVinculada">Entidad a la que pertenece el documento, NULL si es un documento vinculado a la WEB</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad es de organización o no</param>
    /// <returns>URL</returns>
    public string GetURLCompararVersionesDocumento(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreProy, bool pIdentidadOrganizacion)
    {
        string url = "";

        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        if (pNombreProy != null && pNombreProy != "")
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy);
        }
        else
        {
            url = pBaseURL + pUrlPerfil;
        }

        if (pIdentidadOrganizacion)
        {
            return url + pUtilIdiomas.GetText(URLSEM, "COMPARARVERSIONESORG");
        }
        else
        {
            return url + pUtilIdiomas.GetText(URLSEM, "COMPARARVERSIONES");
        }
    }

    #endregion

    #region Tags

    /// <summary>
    /// Devuelve la URL para ir a la busqueda avanzada con un tag seleccionado .
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto para la busqueda avanzada si estamos en una comunidad, si estamos en MyGnoss vacio</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pTag">Tag por el que se deberá realizar un búsqueda</param>
    public string GetURLTagEnBusquedaAvanzada(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pTag)
    {
        string url = "";

        if (pNombreProy != "" && pNombreProy != null)
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
        }
        else
        {
            url = pBaseURL + pUrlPerfil;
        }
        url += pUtilIdiomas.GetText(URLSEM, "BUSQUEDAAVANZADA") + "/" + pUtilIdiomas.GetText(URLSEM, "TAG") + "/";

        if (pTag != null && pTag != "")
        {
            pTag = SustituirCaracteresTagBusqueda(pTag);
            url += pTag;
        }
        return url;
    }

    /// <summary>
    /// Devuelve la URL para ir a la busqueda avanzada con un tag seleccionado .
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto para la busqueda avanzada si estamos en una comunidad, si estamos en MyGnoss vacio</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pTag">Tag por el que se deberá realizar un búsqueda</param>
    public string GetURLTagEnPersonasYOrganizaciones(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pTag)
    {
        string url = "";

        if (pNombreProy != "" && pNombreProy != null)
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
        }
        else
        {
            url = pBaseURL + pUrlPerfil;
        }
        url += pUtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES") + "/" + pUtilIdiomas.GetText(URLSEM, "TAG") + "/";

        if (pTag != null && pTag != "")
        {
            pTag = SustituirCaracteresTagBusqueda(pTag);
            url += pTag;
        }
        return url;
    }

    #endregion

    #region Perfiles personas y organizaciones

    /// <summary>
    /// Devuelve la URL para ir a la pagina de personas y organizaciones.
    /// </summary>
    /// <param name="pbaseURLIdioma">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pTag">tag que se quiere buscar, "" si no se quiere buscar nada</param>
    public string ObtenerURLPersonasYOrganizaciones(string pbaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pTag)
    {
        string url = pbaseURLIdioma + pUrlPerfil;
        if (pNombreProy != null && pNombreProy != "")
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pbaseURLIdioma, pNombreProy) + "/";
        }
        url += pUtilIdiomas.GetText("URLSEM", "PERSONASYORGANIZACIONES");

        if (pTag != "")
        {
            url += "/" + pUtilIdiomas.GetText("URLSEM", "TAG") + "/" + pTag;
        }
        return url;
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLPerfilPersonaOOrg(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad)
    {
        return GetURLPerfilPersonaOOrgEnProyecto(pBaseURL, pUtilIdiomas, pUrlPerfil, pIdentidad, "");
    }

    /// <summary>
    /// Devuelve la URL para ir a el perfil de una persona o una organización.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto</param>
    /// <param name="pNombreCortoUsu">Nombre corto del usuario (si es una org -> null o "")</param>
    /// <param name="pNombreCortoOrg">Nombre corto de la organzación (si no tiene org -> null o "")</param>
    public string GetURLPerfilPersonaOOrgEnProyecto(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreProy, string pNombreCortoUsu, string pNombreCortoOrg)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }
        string urlRedirect = pBaseURL + pUrlPerfil;

        if (pUrlPerfil[pUrlPerfil.Length - 1] != '/')
        {
            urlRedirect += "/";
        }

        if (pNombreProy != null && pNombreProy != "")
        {
            urlRedirect = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
            pUrlPerfil = "/";
        }

        if (!string.IsNullOrEmpty(pNombreCortoOrg))
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pNombreCortoOrg + "/";
        }
        if (!string.IsNullOrEmpty(pNombreCortoUsu))
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pNombreCortoUsu + "/";
        }

        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLPerfilPersonaOOrgEnProyecto(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, Identidad pIdentidad, string pNombreProy)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }
        string urlRedirect = pBaseURL + pUrlPerfil;
        if (pNombreProy != null && pNombreProy != "")
        {
            urlRedirect = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
            pUrlPerfil = "/";
        }

        if (pIdentidad.TrabajaConOrganizacion)
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoOrg + "/";
        }
        if (pIdentidad.ModoPersonal)
        {

            urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pIdentidad.PerfilUsuario.FilaPerfil.NombreCortoUsu + "/";
        }

        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLPerfilPersonaOOrgEnProyecto(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreProy, string nombreCortoUsuario, string nombreCortoOrganizacion, ProfileType tipoPerfil)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }
        string urlRedirect = pBaseURL + pUrlPerfil;
        if (pNombreProy != null && pNombreProy != "")
        {
            urlRedirect = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
            pUrlPerfil = "/";
        }

        if (tipoPerfil.Equals(ProfileType.Organization) || tipoPerfil.Equals(ProfileType.ProfessionalCorporate) || tipoPerfil.Equals(ProfileType.ProfessionalPersonal))
        {
            urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + nombreCortoOrganizacion + "/";
        }
        if (tipoPerfil.Equals(ProfileType.ProfessionalPersonal) || tipoPerfil.Equals(ProfileType.Personal))
        {

            urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + nombreCortoUsuario + "/";
        }

        return urlRedirect;
    }

    /// <summary>
    /// Obtiene la url para acceder al perfil personal de una persona
    /// </summary>

    /// <param name="pBaseURL">baseUrlIdioma</param>
    /// <param name="pUtilIdiomas">utilIdiomas actual</param>
    /// <param name="pUrlPerfil">UrlPerfil actual</param>
    /// <param name="pNombreCortoUsuario">NombreCortoUsu del perfil personal de la persona que queremos acceder</param>
    /// <returns>Url para acceder al perfil personal de una persona</returns>
    public string GetURLPerfilPersonalPersona(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreCortoUsuario)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }
        string urlRedirect = pBaseURLIdioma + pUrlPerfil;
        urlRedirect += pUtilIdiomas.GetText(URLSEM, "PERSONA") + "/" + pNombreCortoUsuario + "/";
        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Contiene la Identidad de la organzación si la hay</param>
    public string GetURLPerfilOrg(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pNombreCortoOrg)
    {
        if (pUrlPerfil == null || pUrlPerfil == "")
        {
            pUrlPerfil = "/";
        }

        string urlRedirect = pBaseURL + pUrlPerfil;
        urlRedirect += pUtilIdiomas.GetText(URLSEM, "ORGANIZACION") + "/" + pNombreCortoOrg;

        return urlRedirect;
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pNombreProy">Nombre corto del proyecto</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad de la que se desea obtener la url de perfil</param>
    public string GetURLPerfilDeIdentidad(string pBaseURL, string pNombreProy, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        string url = pBaseURL + "/";

        if (pNombreProy != null && pNombreProy != "")
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
        }

        if (pIdentidad.TrabajaConOrganizacion)
        {
            url += ObtenerURLOrganizacionOClase(pUtilIdiomas, pIdentidad.OrganizacionID.Value) + "/" + pIdentidad.PerfilUsuario.NombreCortoOrg + "/";
        }

        if (pIdentidad.ModoPersonal)
        {
            url += pUtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + pIdentidad.PerfilUsuario.NombreCortoUsu + "/";
        }
        return url;

    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pNombreProy">Nombre corto del proyecto</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad de la que se desea obtener la url de perfil</param>
    public string GetURLPerfilDeIdentidad(string pBaseURL, string pUrlPerfil, string pNombreProy, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, Proyecto pProyectoSeleccionado, Guid pProyectoPrincipalUnico, bool pPerfilGlobalEnComunidadPrincipal)
    {
        string url = pBaseURL + "/";

        if (!string.IsNullOrEmpty(pNombreProy) && (!pProyectoSeleccionado.Clave.Equals(pProyectoPrincipalUnico) || !pPerfilGlobalEnComunidadPrincipal))
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/";
        }
        else
        {
            url = pBaseURL + pUrlPerfil;
        }

        if (pIdentidad.TrabajaConOrganizacion)
        {
            url += ObtenerURLOrganizacionOClase(pUtilIdiomas, pIdentidad.OrganizacionID.Value) + "/" + pIdentidad.PerfilUsuario.NombreCortoOrg + "/";
        }

        if (pIdentidad.ModoPersonal)
        {
            url += pUtilIdiomas.GetText("URLSEM", "PERSONA") + "/" + pIdentidad.PerfilUsuario.NombreCortoUsu + "/";
        }
        return url;
    }

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad de la que se desea obtener la url de perfil</param>
    public string GetURLPerfilDeIdentidad(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return GetURLPerfilDeIdentidad(pBaseURL, null, pUtilIdiomas, pIdentidad);
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public string GetURLEditarPerfil(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return GetURLEditarPerfil(pBaseURL, pUrlPerfil, pUtilIdiomas, null);
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto</param>
    public string GetURLEditarPerfil(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        if (string.IsNullOrEmpty(pNombreProy))
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFIL");
        }
        else
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText("URLSEM", "EDITARPERFIL");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto</param>
    public string GetURLEditarPerfilOrg(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        if (string.IsNullOrEmpty(pNombreProy))
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILORG");
        }
        else
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILORG");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/privacidad 
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilPrivacidad(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILPRIVACIDAD");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/notificaciones
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilNotificacion(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILNOTIFICACION");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/redes-sociales
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilRedesSociales(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILREDESSOCIALES");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/gadgets
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilGadgets(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILGADGETS");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/gadgets
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilGadgetsOrg(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILGADGETSORG");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil de organizacion
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilOrg(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILORG");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/privacidad de organizacion
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilPrivacidadOrg(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILPRIVACIDADORG");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/notificacion de organizacion
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilNotificacionOrg(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILNOTIFICACIONORG");
    }

    /// <summary>
    /// Devuelve la URL para ir a la edicion de un perfil/redes-sociales de organizacion
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLEditarPerfilRedesSocialesOrg(string pBaseURL, string pUrlPerfil, UtilIdiomas pUtilIdiomas)
    {
        return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "EDITARPERFILREDESSOCIALESORG");
    }

    #endregion

    #region Metabuscador

    /// <summary>
    /// Devuelve la URL relacionada con la búsqueda en el metabuscador por un tag
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pTag">Tag a buscar</param>
    /// <returns>URL semantizada</returns>
    public static string GetURLMetaBuscador(string pBaseURL, UtilIdiomas pUtilIdiomas, Identidad pIdentidad, string pTag)
    {
        if (pTag != null)
        {
            pTag = SustituirCaracteresTagBusqueda(pTag);
        }

        return pBaseURL + GetUrlPerfil(pIdentidad, pUtilIdiomas) + pUtilIdiomas.GetText(URLSEM, "BUSQUEDAAVANZADA") + "/" + pUtilIdiomas.GetText(URLSEM, "TAG") + "/" + pTag;
    }

    #endregion

    #region Comunidad

    /// <summary>
    /// Obtiene la URL de una comunidad
    /// </summary>
    /// <param name="pPagina">Página de origen</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>La url completa del proyecto</returns>
    public string ObtenerURLComunidad(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto, null);
    }

    /// <summary>
    /// Obtiene la URL de una serie de comunidades
    /// </summary>
    /// <param name="pUtilIdiomas">Página de origen</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombresCortos">Nombres cortos de los proyectos</param>
    /// <returns>Dictionary con clave nombrecorto y valor la url completa del proyecto</returns>
    public Dictionary<string, string> ObtenerURLComunidades(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, List<string> pNombresCortos)
    {
        return ObtenerURLComunidades(pUtilIdiomas, pBaseURLIdioma, pNombresCortos, null);
    }

    /// <summary>
    /// Obtiene la URL de una organización o clase pasada como parámetro
    /// </summary>
    /// <param name="pUtilIdiomas">Utilidades de idioma</param>
    /// <param name="pOrganizacionID">Identificador de organización</param>
    /// <returns>Url de la organización</returns>
    public string ObtenerURLOrganizacionOClase(UtilIdiomas pUtilIdiomas, Guid pOrganizacionID)
    {
        string linkOrg = pUtilIdiomas.GetText("URLSEM", "ORGANIZACION");
        return linkOrg;

    }

    /// <summary>
    /// Obtiene la URL de una comunidad
    /// </summary>
    /// <param name="pPagina">Página de origen</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>La url completa del proyecto</returns>
    public string ObtenerURLComunidad(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto, string pFicheroConfiguracion)
    {
        string url = pBaseURLIdioma;

        if ((pUtilIdiomas != null) && (pNombreCorto != null) && (!pNombreCorto.Trim().Equals(string.Empty)) && (pBaseURLIdioma != null))
        {
            Uri urlValida = null;
            if (Uri.TryCreate(pBaseURLIdioma, UriKind.Absolute, out urlValida))
            {
                pBaseURLIdioma = UtilDominios.ObtenerDominioUrl(urlValida, true);
                string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();
                if (!string.IsNullOrEmpty(rutaEjecucionWeb))
                {
                    pBaseURLIdioma = $"{pBaseURLIdioma}/{rutaEjecucionWeb.Trim('/')}";
                }
            }

            url = ObtenerURLDominioComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto, pFicheroConfiguracion);

            ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);

            Guid? proyectoID = mConfigService.ObtenerProyectoConexion();
            if (proyectoID == null || proyectoID.Equals(Guid.Empty))
            {
                Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS.Proyecto proyectoActual = proyectoCN.ObtenerProyectoPorNombreCorto(pNombreCorto);

                string valor = paramCN.ObtenerParametroAplicacion(proyectoActual.URLPropia);

                if (!string.IsNullOrEmpty(valor))
                {
                    proyectoID = new Guid(valor);
                }
            }

            if ((url.EndsWith("depuracion.net") || url.Contains("localhost")) && !proyectoID.HasValue)
            {
                ////Fernando : Si estoy depurando y no tengo un dominio configurado, que me funcionen las urls de la comunidad

                mListaNombreCortoComunidadesSinNombreCortoEnURL = new List<string>();
            }
            else
            {
                if (mListaNombreCortoComunidadesSinNombreCortoEnURL == null)
                {
                    lock (BLOQUEO_ListaNombreCortoComunidadesSinNombreCortoEnURL)
                    {
                        if (mListaNombreCortoComunidadesSinNombreCortoEnURL == null)
                        {
                            ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
                            mListaNombreCortoComunidadesSinNombreCortoEnURL = parametroCN.ObtenerNombresDeProyectosSinNombreCortoEnURL();
                            paramCN.Dispose();
                        }
                    }
                }
            }
            ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
            string dominioPaginasAdministracion = parametroAplicacionCN.ObtenerParametroAplicacion(TiposParametrosAplicacion.DominioPaginasAdministracion);
            if (!string.IsNullOrEmpty(dominioPaginasAdministracion))
            {
                url = $"{pBaseURLIdioma}{ConstruirURLComunidad(pUtilIdiomas, pNombreCorto)}";
                mLoggingService.AgregarEntrada($"Url de comunidad: {url}");
            }
            else
            {
                url += ConstruirURLComunidad(pUtilIdiomas, pNombreCorto);
            }            
        }
        return url;
    }

    private static string ConstruirURLComunidad(UtilIdiomas pUtilIdiomas, string pNombreCorto)
    {
        string urlCom = "";

        if (!mListaNombreCortoComunidadesSinNombreCortoEnURL.Contains(pNombreCorto))
        {
            urlCom = "/" + pUtilIdiomas.GetText("URLSEM", "COMUNIDAD") + "/" + pNombreCorto;
        }

        string idiomaActual = pUtilIdiomas.LanguageCode;

        bool mostrarIdiomaURL = true;

        if (!mListaURLComunidades.ContainsKey(pNombreCorto) && idiomaActual.Equals("es"))
        {
            //Si la lista de urls propias de la comunidad NO contiene el nombrecorto, hacemos el comportamiento por defecto(mostramos el idioma siempre que no sea español)
            mostrarIdiomaURL = false;
        }
        else if (mListaURLComunidades.ContainsKey(pNombreCorto) && mListaURLComunidades[pNombreCorto].ContainsKey(idiomaActual))
        {
            string dominioIdiomaActual = mListaURLComunidades[pNombreCorto][idiomaActual];
            //Si la lista de urls propias de la comunidad contiene el nombrecorto y ademas tenemos una url propia para ese idioma, el primer idioma configurado para esa url no muestra el idioma en la url
            if (mListaURLComunidades[pNombreCorto].First(u => u.Value == dominioIdiomaActual).Key.Equals(idiomaActual))
            {
                mostrarIdiomaURL = false;
            }
        }

        if (mostrarIdiomaURL)
        {
            urlCom = "/" + pUtilIdiomas.LanguageCode + urlCom;
        }
        return urlCom;
    }

    private void CargarUrlComunidad(string pNombreCorto, string pFicheroConfiguracion = null)
    {
        if (!mListaURLComunidades.ContainsKey(pNombreCorto))
        {
            try
            {
                //Compruebo si el proyecto tiene URL propia
                ProyectoCN proyCN;

                if (pFicheroConfiguracion == null || pFicheroConfiguracion == string.Empty)
                {
                    proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                }
                else
                {
                    proyCN = new ProyectoCN(pFicheroConfiguracion, mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                }
                string url = proyCN.ObtenerURLPropiaProyectoPorNombreCorto(pNombreCorto);
                proyCN.Dispose();

                lock (BLOQUEO_ListaURLComunidades)
                {
                    if (!mListaURLComunidades.ContainsKey(pNombreCorto))
                    {
                        if (!string.IsNullOrEmpty(url))
                        {
                            if (!mListaURLComunidades.ContainsKey(pNombreCorto))
                            {
                                mListaURLComunidades.TryAdd(pNombreCorto, new Dictionary<string, string>());

                                string[] urls = url.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string urlPropia in urls)
                                {
                                    string idioma = IdiomaPrincipalDominio;
                                    string[] urlIdioma = urlPropia.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                                    url = UtilDominios.DomainToPunyCode(urlIdioma[0]);
                                    if (urlIdioma.Length > 1)
                                    { idioma = urlIdioma[1]; }

                                    if (!mListaURLComunidades[pNombreCorto].ContainsKey(idioma))
                                    {
                                        mListaURLComunidades[pNombreCorto].Add(idioma, url);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!mListaURLComunidades.ContainsKey(pNombreCorto))
                            {
                                mListaURLComunidades.TryAdd(pNombreCorto, new Dictionary<string, string>());

                                string idioma = "es";

                                mListaURLComunidades[pNombreCorto].Add(idioma, url);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }
    }

    public string ObtenerIdiomaDefectoDeUrlComunidad(string pDominio, string pNombreCorto)
    {
        CargarUrlComunidad(pNombreCorto);
        string idiomaDefecto = "es";

        if (mListaURLComunidades.ContainsKey(pNombreCorto))
        {
            var listaUrls = mListaURLComunidades[pNombreCorto];

            if (listaUrls.ContainsValue(pDominio))
            {
                idiomaDefecto = listaUrls.FirstOrDefault(item => item.Value.Equals(pDominio)).Key;
            }
            else
            {
                idiomaDefecto = listaUrls.Keys.First();
            }
        }

        return idiomaDefecto;
    }

    /// <summary>
    /// Obtiene el dominio de una comunidad
    /// </summary>
    /// <param name="pPagina">Página de origen</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>La url completa del proyecto</returns>
    public string ObtenerURLDominioComunidad(UtilIdiomas pUtilIdiomas, string pBaseURL, string pNombreCorto, string pFicheroConfiguracion)
    {
        string url = "";

        if ((pUtilIdiomas != null) && (pNombreCorto != null) && (!pNombreCorto.Trim().Equals(string.Empty)) && (pBaseURL != null))
        {
            if (pUtilIdiomas.TraerDeCache)
            {
                url = "@#@$DOMINIOCOMUNIDAD,|," + pNombreCorto + "$@#@";
            }
            else
            {
                CargarUrlComunidad(pNombreCorto, pFicheroConfiguracion);
            }

            if (!pBaseURL.Contains("depuracion") && !pBaseURL.Contains("localhost"))
            {
                string rutaEjecucionWeb = mConfigService.ObtenerRutaEjecucionWeb();
                //Cojo la URL de la lista de comunidades
                if (mListaURLComunidades[pNombreCorto].ContainsKey(pUtilIdiomas.LanguageCode))
                {
                    url = $"{mListaURLComunidades[pNombreCorto][pUtilIdiomas.LanguageCode]}/{rutaEjecucionWeb}".TrimEnd('/');
                }
                else
                {
                    url = $"{mListaURLComunidades[pNombreCorto].First().Value}/{rutaEjecucionWeb}".TrimEnd('/');
                }
            }
            else
            {
                url = pBaseURL;
            }
        }

        if ((url == null) || url.Trim().Equals(string.Empty))
        {
            //Si no tiene URL propia la compongo de la manera habitual
            url = mConfigService.ObtenerUrlBase();

            if (string.IsNullOrEmpty(url))
            {
                url = pBaseURL;
            }
        }
        return url;
    }


    /// <summary>
    /// Obtiene la URL de una serie de comunidades
    /// </summary>
    /// <param name="pUtilIdiomas">Página de origen</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombresCortos">Nombres cortos de los proyectos</param>
    /// <returns>Dictionary con clave nombrecorto y valor la url completa del proyecto</returns>
    public Dictionary<string, string> ObtenerURLComunidades(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, List<string> pNombresCortos, string pFicheroConfiguracion)
    {
        Dictionary<string, string> urlComunidades = new Dictionary<string, string>();

        try
        {
            //Compruebo si el proyecto tiene URL propia
            ProyectoCN proyCN;

            if (pFicheroConfiguracion == null || pFicheroConfiguracion == string.Empty)
            {
                proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            }
            else
            {
                proyCN = new ProyectoCN(pFicheroConfiguracion, mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            }

            Dictionary<string, string> urlComunidadesAModificar = new Dictionary<string, string>();

            var nombresCortos = pNombresCortos.Except(mListaURLComunidades.Keys).ToList();

            urlComunidadesAModificar = proyCN.ObtenerURLSPropiasProyectosPorNombresCortos(nombresCortos);
            proyCN.Dispose();

            foreach (string nombreCorto in urlComunidadesAModificar.Keys)
            {
                if (!mListaURLComunidades.ContainsKey(nombreCorto))
                {
                    string url = urlComunidadesAModificar[nombreCorto];

                    lock (BLOQUEO_ListaURLComunidades)
                    {
                        if (!mListaURLComunidades.ContainsKey(nombreCorto))
                        {
                            mListaURLComunidades.TryAdd(nombreCorto, new Dictionary<string, string>());

                            string[] urls = url.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string urlPropia in urls)
                            {
                                string idioma = IdiomaPrincipalDominio;
                                string[] urlIdioma = urlPropia.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
                                url = UtilDominios.DomainToPunyCode(urlIdioma[0]);
                                if (urlIdioma.Length > 1) { idioma = urlIdioma[1]; }

                                if (!mListaURLComunidades[nombreCorto].ContainsKey(idioma))
                                {
                                    mListaURLComunidades[nombreCorto].Add(idioma, url);
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            mLoggingService.GuardarLogError(ex, mlogger );
        }

        if (mListaNombreCortoComunidadesSinNombreCortoEnURL == null)
        {
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<ParametroCN>(), mLoggerFactory);
            mListaNombreCortoComunidadesSinNombreCortoEnURL = paramCN.ObtenerNombresDeProyectosSinNombreCortoEnURL();
            paramCN.Dispose();
        }

        foreach (string nombreCorto in pNombresCortos)
        {
            if ((!urlComunidades.ContainsKey(nombreCorto) || (urlComunidades.ContainsKey(nombreCorto) && urlComunidades[nombreCorto].Trim().Equals(string.Empty))))
            {
                string urlCom = ObtenerURLDominioComunidad(pUtilIdiomas, pBaseURLIdioma, nombreCorto, pFicheroConfiguracion) + ConstruirURLComunidad(pUtilIdiomas, nombreCorto);

                urlComunidades[nombreCorto] = urlCom;
            }
        }

        return urlComunidades;
    }

    /// <summary>
    /// Obtiene la URL de una de las paginas de Administracion de comunidades
    /// </summary>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <param name="pNombreSemPaginaDestino">Nombre de la clave del archivo de idioma de la cabecera de la pagina de administracion a la que queremos ir</param>
    /// <returns></returns>
    public string ObtenerURLAdministracionComunidad(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto, string pNombreSemPaginaDestino)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", pNombreSemPaginaDestino);
    }

    /// <summary>
    /// Obtiene la URL de la politica de una comunidad
    /// </summary>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns></returns>
    public string ObtenerURLPoliticaComunidad(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "POLITICACOMUNIDAD");
    }

    #endregion

    #region Suscripciones

    /// <summary>
    /// Obtiene la url para administrar una suscripcion a comunidad
    /// </summary>
    /// <param name="pBaseUrl">BaseUrl con idioma</param>
    /// <param name="pUtilIdiomas">utilidiomas actual</param>
    /// <param name="pIdentidad">Identidad actual o con la que se accede a la comunidad</param>
    /// <param name="pNombreProy">Nombre semántico de la comunidad para la que se administra la suscripcion</param>
    /// <returns></returns>
    public string GetUrlAdministrarSuscripcionComunidad(string pBaseUrl, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseUrl, pNombreProy) + "/" + pUtilIdiomas.GetText("URLSEM", "ADMINISTRARSUSCRIPCIONCOMUNIDAD");
    }

    /// <summary>
    /// Obtiene la url para administrar una suscripcion a un usuario
    /// </summary>
    /// <param name="pBaseUrlIdioma">BaseUrl con idioma</param>
    /// <param name="pUtilIdiomas">utilidiomas actual</param>
    /// <param name="pNombreCortoUsuario">Nombre corto del usuario de la suscripción</param>
    /// <param name="pNombreProy">Nombre semántico de la comunidad en la que se administra la suscripcion (NULL si lo queremos hacer en MyGnoss)</param>
    /// <param name="pNombreCortoOrg">Nombre corto de la organizacion de la suscripción</param>
    /// <param name="pCheck">Si queremos que se chekeen todas</param>
    /// <returns></returns>
    public string GetUrlAdministrarSuscripcionIdentidad(string pBaseUrlIdioma, string pUrlPerfil, UtilIdiomas pUtilIdiomas, string pNombreCortoUsuario, string pNombreCortoOrg, string pNombreProy, bool pCheck)
    {
        string enlace = "";

        if (pNombreCortoOrg != null && pNombreCortoOrg != "")
        {
            pNombreCortoOrg += "/";
        }
        else
        {
            pNombreCortoOrg = "";
        }

        if (pNombreProy == null || pNombreProy == "")
        {
            enlace = pBaseUrlIdioma + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "ADMINISTRARSUSCRIPCIONIDENTIDAD") + "/" + pNombreCortoOrg + pNombreCortoUsuario;
        }
        else
        {
            enlace = ObtenerURLComunidad(pUtilIdiomas, pBaseUrlIdioma, pNombreProy) + "/" + pUtilIdiomas.GetText("URLSEM", "ADMINISTRARSUSCRIPCIONIDENTIDAD") + "/" + pNombreCortoOrg + pNombreCortoUsuario;
        }

        if (pCheck)
        {
            enlace += "/check";
        }

        return enlace;
    }

    /// <summary>
    /// Obtiene la url para administrar suscripciones
    /// </summary>
    /// <param name="pBaseUrl">BaseUrl con idioma</param>
    /// <param name="pUtilIdiomas">utilidiomas actual</param>
    /// <param name="pIdentidad">Identidad actual o con la que se accede a la comunidad</param>
    /// <param name="pUrlPerfil">Url del perfil con el que estamos conectados</param>
    /// <returns></returns>
    public static string GetUrlAdministrarSuscripciones(string pBaseUrl, UtilIdiomas pUtilIdiomas, string pUrlPerfil)
    {
        return pBaseUrl + pUrlPerfil + pUtilIdiomas.GetText("URLSEM", "ADMINISTRARSUSCRIPCIONES");
    }

    #endregion

    #region Categorías del tesauro

    /// <summary>
    /// Devuelve la URL para ir a la categoría de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pNombreCategoria">Nombre Sem de la categoría</param>
    /// <param name="pCategoriaID">Identificador de la categoría</param>
    /// <param name="pMyGnoss">Indica si la categoría pertenece al tesauro de MyGnoss</param>
    /// <param name="pTesauroOrganizacion">Indica si el tesauro es de organización</param>
    /// <param name="pFicheroConfiguracion">Fichero de configuración</param>
    public string GetURLDetalleCategoria(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pUrlPerfil, string pNombreCategoria, Guid pCategoriaID, bool pTesauroOrganizacion, string pFicheroConfiguracion, string pPestanya)
    {
        string url = "";
        string pestanyaRecurso = pUtilIdiomas.GetText(URLSEM, "MISRECURSOS");
        if (!string.IsNullOrEmpty(pPestanya))
        {
            pestanyaRecurso = pPestanya;
        }
        else
        {
            if (string.IsNullOrEmpty(pNombreProy))
            {
                pestanyaRecurso = pUtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");
            }
            else if (!string.IsNullOrEmpty(pNombreProy))
            {
                pestanyaRecurso = pUtilIdiomas.GetText(URLSEM, "RECURSOS");
            }
            else if (pTesauroOrganizacion)
            {
                pestanyaRecurso = pUtilIdiomas.GetText(URLSEM, "RECURSOSORG");
            }
        }

        if (string.IsNullOrEmpty(pNombreProy))
        {
            url = pBaseURL + "/" + pestanyaRecurso + "/" + pUtilIdiomas.GetText(URLSEM, "CATEGORIA");
        }
        else if (!string.IsNullOrEmpty(pNombreProy))
        {
            url = ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy, pFicheroConfiguracion) + "/" + pestanyaRecurso + "/" + pUtilIdiomas.GetText(URLSEM, "CATEGORIA");
        }
        else if (pTesauroOrganizacion)
        {
            url = pBaseURL + pUrlPerfil + pestanyaRecurso + "/" + pUtilIdiomas.GetText(URLSEM, "CATEGORIA");
        }
        else
        {
            url = pBaseURL + pUrlPerfil + pestanyaRecurso + "/" + pUtilIdiomas.GetText(URLSEM, "CATEGORIA");
        }

        if (!string.IsNullOrEmpty(pNombreCategoria) && pCategoriaID != Guid.Empty)
        {
            url += "/" + pNombreCategoria + "/" + pCategoriaID.ToString();
        }

        return url;
    }

    #endregion

    #region Administrar comunidad

    /// <summary>
    /// Obtiene la url para acceder a la página de administración de miembros de una comunidad
    /// </summary>
    /// <param name="pBaseUrlIdioma">BaseUrlIdioma actual</param>
    /// <param name="pUtilIdiomas">UtilIdiomas actual</param>
    /// <param name="pNombreCortoProy">Nombre corto del proyecto que queremos administrar sus miembros</param>
    /// <returns>url completa para acceder a la página</returns>
    public string GetURLAdministrarMiembrosComunidad(string pBaseUrlIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseUrlIdioma, pNombreCortoProy) + "/" + pUtilIdiomas.GetText(URLSEM, "COMADMINCOMMIEMBROS");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de administración de el tesauro de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pLimpiarSesion">Indica si se deben limpiar de la sesión las variables con las que se gestiona el tesauro</param>
    /// <returns>URL para ir a la pantalla de administración de el tesauro de una comunidad</returns>
    public string GetURLAdministrarCategoriasComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, bool pLimpiarSesion)
    {
        if (pLimpiarSesion)
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCATEGORIASCOM");
        }
        else
        {
            return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCATEGORIASCOMRECUPERAR");
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de administración de los tesauros semánticos de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <returns>URL para ir a la pantalla de administración de los tesauros semánticos de una comunidad</returns>
    public string GetURLAdministrarTesSemComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINTESSEM");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de administración de las entidades semánticas de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <returns>URL para ir a la pantalla de administración de entidades secundarias de una comunidad</returns>
    public string GetURLAdministrarEntSecundariasComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINENTSECUND");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de administración de los grafos simples de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <returns>URL para ir a la pantalla de administración de los grafos simples de una comunidad</returns>
    public string GetURLAdministrarGrafosSimplesComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINGRAFSIMPLE");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de administración de ning de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>    
    /// <returns>URL para ir a la pantalla de administración de ning de una comunidad</returns>
    public string GetURLAdministrarNingComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCOMUNIDADNING");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de administración de fuentes de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>    
    /// <returns>URL para ir a la pantalla de administración de el tesauro de una comunidad</returns>
    public string GetURLAdministrarFuentesComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCOMUNIDADFUENTES");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de edicion de una fuente de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>    
    /// <param name="pNombreFuente">Nombre de la fuente</param>
    /// <param name="pFuenteID">ID de la fuente</param>
    /// <returns>URL para ir a la pantalla de administración de el tesauro de una comunidad</returns>
    public string GetURLEditarFuenteComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pNombreFuente, Guid pFuenteID)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCOMUNIDADFUENTE") + "/" + UtilCadenas.EliminarCaracteresUrlSem(pNombreFuente) + "/" + pFuenteID.ToString();
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de revisión de recursos externos
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>   
    /// <returns>URL para ir a la pantalla de revisión de recursos externos</returns>
    public string GetURLRevisarRecursosExternosComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "REVISARRECURSOSEXTERNOS");
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de revisión de un recurso externo
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>    
    /// <param name="pNombreRecurso">Nombre del reurso</param>
    /// <param name="pRecursoID">ID del recurso</param>
    /// <returns>URL para ir a la pantalla de revisión de un recurso externo</returns>
    public string GetURLRevisarRecursoExternoComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy, string pNombreRecurso, Guid pRecursoID)
    {
        string nombreRecurso = UtilCadenas.EliminarCaracteresUrlSem(pNombreRecurso);
        if (nombreRecurso == "")
        {
            nombreRecurso = "no-name";
        }
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "REVISARRECURSOEXTERNO") + "/" + nombreRecurso + "/" + pRecursoID.ToString();
    }


    /// <summary>
    /// Devuelve la URL para ir a la pantalla de edicion de un marcador
    /// </summary>
    /// <param name="pBaseUrl">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreMarcador">Nombre del marcadr</param>
    /// <param name="pMarcadorID">ID del marcador</param>
    /// <returns></returns>
    public string GetURLRevisarMarcador(string pBaseUrl, UtilIdiomas pUtilIdiomas, string pNombreMarcador, Guid pMarcadorID)
    {
        string nombreMarcador = UtilCadenas.EliminarCaracteresUrlSem(pNombreMarcador);
        if (nombreMarcador == "")
        {
            nombreMarcador = "no-name";
        }

        return pBaseUrl + "/" + pUtilIdiomas.GetText("URLSEM", "MISMARCADORES") + "/" + nombreMarcador + "/" + pMarcadorID;
    }

    /// <summary>
    /// Devuelve la URL para ir a la pantalla de creacion de una fuente de una comunidad.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>    
    /// <returns>URL para ir a la pantalla de administración de el tesauro de una comunidad</returns>
    public string GetURLCrearFuenteComunidad(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "ADMINISTRARCOMUNIDADFUENTE");
    }

    #endregion

    #region Wiki

    /// <summary>
    /// Devuelve la URL relacionada con la wiki de una comunidad
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pIdentidad">Identidad actual del usuario</param>
    /// <param name="pNombreProy">Nombre del proyecto a administrar la Wiki</param>
    /// <returns>URL semantizada</returns>
    public string GetURLWikiAdmin(string pBaseURL, UtilIdiomas pUtilIdiomas, string pNombreProy)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURL, pNombreProy) + "/" + pUtilIdiomas.GetText(URLSEM, "WIKIHISTORIAL");
    }

    #endregion

    #region Preguntas Debates Y Debates

    /// <summary>
    /// Obtiene la URL de preguntas
    /// </summary>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>Url</returns>
    public string ObtenerURLPreguntas(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "PREGUNTAS");
    }

    /// <summary>
    /// Obtiene la URL de encuestas
    /// </summary>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>Url</returns>
    public string ObtenerURLEncuestas(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "ENCUESTAS");
    }

    /// <summary>
    /// Obtiene la URL de Debates
    /// </summary>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>Url</returns>
    public string ObtenerURLDebates(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "DEBATES");
    }

    /// <summary>
    /// Obtiene la URL de Blogs
    /// </summary>
    /// <param name="pUtilIdiomas">Util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pNombreCorto">Nombre corto del proyecto</param>
    /// <returns>URL</returns>
    public string ObtenerURLBlogs(UtilIdiomas pUtilIdiomas, string pBaseURLIdioma, string pNombreCorto)
    {
        return ObtenerURLComunidad(pUtilIdiomas, pBaseURLIdioma, pNombreCorto) + "/" + pUtilIdiomas.GetText("URLSEM", "BLOGS");
    }

    #endregion

    #region Métodos privados

    /// <summary>
    /// Devuelve la URL de la identidad donde estamos conectados
    /// </summary>
    /// <param name="pIdentidad">Identidad actual</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <returns>Cadena que representa la URL</returns>
    private static string GetUrlPerfil(Identidad pIdentidad, UtilIdiomas pUtilIdiomas)
    {
        string resultado = "/";
        if (pIdentidad != null && pIdentidad.TrabajaConOrganizacion)
        {
            resultado += pUtilIdiomas.GetText(URLSEM, "IDENTIDAD") + "/" + pIdentidad.PerfilUsuario.NombreCortoOrg + "/";
        }
        else if (pIdentidad != null && pIdentidad.EsIdentidadProfesor)
        {
            resultado += pUtilIdiomas.GetText(URLSEM, "IDENTIDAD") + "/" + pIdentidad.PerfilUsuario.NombreCortoUsu + "/";
        }
        return resultado;
    }

    /// <summary>
    /// Devuelve la URL de la identidad donde estamos conectados
    /// </summary>
    /// <param name="pNombreCortoOrg">NombreCorto de la organizacion</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <returns>Cadena que representa la URL</returns>
    private static string GetUrlPerfil(string pNombreCortoOrg, UtilIdiomas pUtilIdiomas)
    {
        string resultado = "/" + pUtilIdiomas.GetText(URLSEM, "IDENTIDAD") + "/" + pNombreCortoOrg + "/";

        return resultado;
    }

    #endregion

    #region AddToGnoss

    /// <summary>
    /// Devuelve la URL para ir a actualizar la herramienta add to Gnoss del navegador.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    public static string GetURLActualizarAddToGnossNavegador(string pBaseURL, UtilIdiomas pUtilIdiomas)
    {
        if (pBaseURL.Length > 0 && pBaseURL[pBaseURL.Length - 1] != '/')
        {
            pBaseURL += "/";
        }
        return pBaseURL + pUtilIdiomas.GetText("URLSEM", "ACTUALIZARADDTOGNOSSNAVG");
    }

    #endregion

    #region Curriculum

    /// <summary>
    /// Devuelve la URL para ir a la base de recursos de un documento.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad actual es de organzación</param>
    public static string GetURLCurriculumSemanticoEditar(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, bool pIdentidadOrganizacion)
    {
        return GetURLCurriculumSemanticoEditar(pBaseURL, pUtilIdiomas, pUrlPerfil, pIdentidadOrganizacion, null, Guid.Empty);
    }

    /// <summary>
    /// Devuelve la URL para ir a editar un CV semántico.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad actual es de organzación</param>
    /// <param name="pNombreBioSem">Nombre semántico de la bio</param>
    /// <param name="pBioID">ID de la bio</param>
    public static string GetURLCurriculumSemanticoEditar(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, bool pIdentidadOrganizacion, string pNombreBioSem, Guid pBioID)
    {
        if (pBioID == Guid.Empty)
        {
            if (pIdentidadOrganizacion)
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "EDITARTRAYECTORIASEM") + "/" + pUtilIdiomas.GetText(URLSEM, "NUEVABIOSEMANTICA") + "/" + Guid.NewGuid() + "/" + Guid.NewGuid();
            }
            else
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "EDITARBIOSSEM") + "/" + pUtilIdiomas.GetText(URLSEM, "NUEVABIOSEMANTICA") + "/" + Guid.NewGuid() + "/" + Guid.NewGuid();
            }
        }
        else
        {
            if (pIdentidadOrganizacion)
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "EDITARTRAYECTORIASEM") + "/" + pNombreBioSem + "/" + pBioID.ToString();
            }
            else
            {
                return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "EDITARBIOSSEM") + "/" + pNombreBioSem + "/" + pBioID.ToString();
            }
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a la vista previa de un CV semántico.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad actual es de organzación</param>
    /// <param name="pNombreBioSem">Nombre semántico de la bio</param>
    /// <param name="pBioID">ID de la bio</param>
    public static string GetURLCurriculumSemanticoVistaPrevia(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, bool pIdentidadOrganizacion, string pNombreBioSem, Guid pBioID)
    {
        if (pIdentidadOrganizacion)
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "VISTAPREVIATRAYECTORIASEM") + "/" + pNombreBioSem + "/" + pBioID.ToString();
        }
        else
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "VISTAPREVIABIOSEM") + "/" + pNombreBioSem + "/" + pBioID.ToString();
        }
    }

    /// <summary>
    /// Devuelve la URL para ir a editar un nuevo CV semántico.
    /// </summary>
    /// <param name="pBaseURL">URL de la página donde se encuentre el usuario</param>
    /// <param name="pUtilIdiomas">Funciones para obtener cadenas en diferentes idiomas</param>
    /// <param name="pNombreProy">Nombre del proyecto de la BR</param>
    /// <param name="pUrlPerfil">URL del perfil de la persona</param>
    /// <param name="pIdentidadOrganizacion">Indica si la identidad actual es de organzación</param>
    /// <param name="pNuevaBioID">ID de la bio</param>
    /// <param name="pNuevoDocID">ID de documento asociado a la bio</param>
    public static string GetURLCurriculumSemanticoNuevo(string pBaseURL, UtilIdiomas pUtilIdiomas, string pUrlPerfil, bool pIdentidadOrganizacion, Guid pNuevaBioID, Guid pNuevoDocID)
    {
        if (pIdentidadOrganizacion)
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "EDITARTRAYECTORIASEM") + "/" + pUtilIdiomas.GetText(URLSEM, "NUEVABIOSEMANTICA") + "/" + pNuevaBioID + "/" + pNuevoDocID;
        }
        else
        {
            return pBaseURL + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "EDITARBIOSSEM") + "/" + pUtilIdiomas.GetText(URLSEM, "NUEVABIOSEMANTICA") + "/" + pNuevaBioID + "/" + pNuevoDocID;
        }
    }

    #endregion

    #region Clases

    /// <summary>
    /// Obtiene la URL de Mis clases de la identidad pasada como parámetro.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pIdentidad">Identidad que es profesor</param>
    /// <returns>Url</returns>
    public static string GetUrlMisClases(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURLIdioma + "/" + pUtilIdiomas.GetText(URLSEM, "IDENTIDAD") + "/" + pIdentidad.IdentidadProfesorMyGnoss.PerfilUsuario.NombreCortoUsu + "/" + pUtilIdiomas.GetText(URLSEM, "VERCLASESADMINISTRO");
    }

    /// <summary>
    /// Obtiene la URL de Mis asignaturas de la identidad pasada como parámetro.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pIdentidad">Identidad que es profesor</param>
    /// <returns>Url</returns>
    public static string GetUrlMisAsignaturas(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, Identidad pIdentidad)
    {
        return pBaseURLIdioma + "/" + pUtilIdiomas.GetText(URLSEM, "IDENTIDAD") + "/" + pIdentidad.IdentidadProfesorMyGnoss.PerfilUsuario.NombreCortoUsu + "/" + pUtilIdiomas.GetText(URLSEM, "MISCOMUNIDADES");
    }

    /// <summary>
    /// Obtiene la URL de Mis asignaturas de la identidad pasada como parámetro.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pNombreCortoClase">Nombre corto de la clase</param>
    /// <returns>Url</returns>
    public static string GetUrlClase(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pNombreCortoClase)
    {
        return pBaseURLIdioma + "/" + pUtilIdiomas.GetText(URLSEM, "IDENTIDAD") + "/" + pNombreCortoClase + "/" + pUtilIdiomas.GetText(URLSEM, "PERFIL");
    }

    /// <summary>
    /// Obtiene la URL de Alumnos(usuarios) de la identidad profesor pasada como parámetro.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <param name="pIdentidad">Identidad que es profesor</param>
    /// <returns>Url</returns>
    public static string GetUrlAlumnos(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pUrlPerfil)
    {
        return pBaseURLIdioma + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "USUARIOS");
    }

    /// <summary>
    /// Obtiene la URL de Alumnos(usuarios) de la identidad profesor pasada como parámetro.
    /// </summary>
    /// <param name="pBaseURLIdioma">URL base (sin "/" al final)</param>
    /// <param name="pUtilIdiomas">util idiomas</param>
    /// <returns>Url</returns>
    public static string GetUrlAdministrarOrg(string pBaseURLIdioma, UtilIdiomas pUtilIdiomas, string pUrlPerfil, string pTipoPagina)
    {
        if (string.IsNullOrEmpty(pTipoPagina))
        {
            pTipoPagina = pUtilIdiomas.GetText(URLSEM, "USUARIOS");
        }
        return pBaseURLIdioma + pUrlPerfil + pUtilIdiomas.GetText(URLSEM, "ADMINISTRACION") + "/" + pTipoPagina;
    }

    #endregion

    #endregion

    #region Métodos estáticos

    /// <summary>
    /// Obtiene la verdadera cadena de texto del tag de la URL.
    /// </summary>
    /// <param name="pPage">Página en la que estamos</param>
    /// <param name="pUsarCaracteresEscape">Verdad si se deben usar carácteres de escape (si se va a usar dentro de un script)</param>
    /// <returns>Verdaro tag buscado</returns>
    public static string ObtenerTagBusqueda(string pTags, bool pUsarCaracteresEscape)
    {
        string cadena = HttpUtility.UrlDecode(pTags).Replace(UtilWeb.SUSTITUTO_ANPERSAN, "&");

        if (pUsarCaracteresEscape)
        {
            cadena = cadena.Replace("'", "\\'");
        }

        return cadena;
    }

    /// <summary>
    /// Obtiene la verdadera cadena de texto del tag de la URL.
    /// </summary>
    /// <param name="pTag">Tag por el que buscamos</param>
    /// <returns>Verdaro tag buscado</returns>
    public static string SustituirCaracteresTagBusqueda(string pTag)
    {
        return HttpUtility.UrlEncode(UtilCadenas.LimpiarTag(pTag.Replace("&", UtilWeb.SUSTITUTO_ANPERSAN)));
    }

    #endregion
}

/// <summary>
/// Clase para generar las URLs de los RDF
/// </summary>
public class GnossGeneradorUrlsRDF : IGeneradorURL
{
    #region Miembros

    private UtilIdiomas mUtilIdiomas;
    private string mUrlBase;
    private string mUrlBaseContent;
    private string mUrlPerfil;
    private string mUrlComunidad;

    /// <summary>
    /// URL de la página actual.
    /// </summary>
    private string mUrlActual;

    /// <summary>
    /// URL de la página actual sin query
    /// </summary>
    private string mUrlActualSinQuery;


    /// <summary>
    /// Pestanya del proyecto actual donde se encuentra el usuario.
    /// </summary>
    private string mPestanya;

    /// <summary>
    /// Almacena la URL de cada elemento (de la manera elementoID -> url), de los elementos que de por sí no tiene URL, asi que le doy la misma URL que a su propietario (comentarios de un documento, dafos, etc)
    /// </summary>
    private Dictionary<Guid, string> mListaUrlsElementosSinPagina;

    private GnossUrlsSemanticas mGnossUrlsSemanticas;
    private LoggingService mLoggingService;
    private EntityContext mEntityContext;
    private IHttpContextAccessor mHttpContextAccessor;
    private ConfigService mConfigService;
    private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
    private ILogger mlogger;
    private ILoggerFactory mLoggerFactory;
    #endregion

    #region Constructores

    /// <summary>
    /// Constructor de la clase
    /// </summary>
    /// <param name="pUtilIdiomas">Util idiomas</param>
    /// <param name="pBaseURLIdioma">URL base (con http:...)</param>
    /// <param name="pUrlPerfil">Url del perfil de la persona actual</param>
    /// <param name="pUrlComunidad">Url de la comunidad a la que el usuario está conectado</param>
    public GnossGeneradorUrlsRDF(UtilIdiomas pUtilIdiomas, string pBaseURL, string pBaseURLContent, string pUrlPerfil, string pUrlComunidad, string pUrlActual, string pUrlActualSinQuery, string pPestanya, GnossUrlsSemanticas gnossUrlsSemanticas, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, ILogger<GnossGeneradorUrlsRDF> logger, ILoggerFactory loggerFactory)
    {
        mUrlBase = pBaseURL;
        mUrlBaseContent = pBaseURLContent;
        mUrlComunidad = pUrlComunidad;
        mUrlPerfil = pUrlPerfil;
        mUtilIdiomas = pUtilIdiomas;
        mUrlActual = pUrlActual;
        mUrlActualSinQuery = pUrlActualSinQuery;

        mPestanya = pPestanya;
        mlogger = logger;
        mLoggerFactory = loggerFactory;
        mListaUrlsElementosSinPagina = new Dictionary<Guid, string>();

        mGnossUrlsSemanticas = gnossUrlsSemanticas;
        mLoggingService = loggingService;
        mEntityContext = entityContext;
        mConfigService = configService;
        mHttpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Miembros de IGeneradorURL

    public string ObtenerUrlRecurso(Documento pDocumento)
    {
        string url = mGnossUrlsSemanticas.GetURLBaseRecursosFicha(mUrlBase, mUtilIdiomas, mUrlComunidad, mUrlPerfil, pDocumento, false);

        foreach (Comentario comentario in pDocumento.Comentarios)
        {
            if (!mListaUrlsElementosSinPagina.ContainsKey(comentario.Clave))
            {
                mListaUrlsElementosSinPagina.Add(comentario.Clave, url);
            }
        }

        return url;
    }

    /// <summary>
    /// Obtiene la URL de descarga del recurso pasado por parámetro
    /// </summary>
    /// <param name="pDocumento">Documento</param>
    /// <returns>URL del recurso</returns>
    public string ObtenerUrlDescargaRecurso(Documento pDocumento)
    {
        string url = "";
        bool dePersona = false;
        bool deOrg = false;
        string idPersonaOOrg = "";

        if (pDocumento.ProyectoID.Equals(ProyectoAD.MetaProyecto))
        {
            //Es de persona o de organización  //"IdentidadPublicacionID = '" + pDocumento.FilaDocumento.CreadorID + "'"
            List<Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos> filas = pDocumento.GestorDocumental.DataWrapperDocumentacion.ListaDocumentoWebVinBaseRecursos.Where(docweb => docweb.IdentidadPublicacionID.HasValue && docweb.IdentidadPublicacionID.Value.Equals(pDocumento.FilaDocumento.CreadorID)).ToList();

            if ((filas != null) && (filas.Count > 0))
            {
                IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                idPersonaOOrg = identCN.ObtenerPersonaOOrganizacionIDDeIdentidad(pDocumento.FilaDocumento.CreadorID).ToString();
                identCN.Dispose();

                Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.DocumentoWebVinBaseRecursos filaVinDoc = filas.First();
                if (!filaVinDoc.PublicadorOrgID.HasValue)
                {
                    dePersona = true;
                }
                else
                {
                    deOrg = true;
                }
            }
        }

        if (pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.FicheroServidor))
        {

            if (!string.IsNullOrEmpty(pDocumento.Enlace) && Uri.IsWellFormedUriString(pDocumento.Enlace, UriKind.Absolute))
            {
                url = pDocumento.Enlace;
            }
            else
            {
                // + "&personaID=" + PersonaPublicadorID
                string carpetaPersonaOOrg = "";

                if (dePersona)
                {
                    carpetaPersonaOOrg = "&personaID=" + idPersonaOOrg;
                }
                else if (deOrg)
                {
                    carpetaPersonaOOrg = "&org=" + idPersonaOOrg;
                }
                else
                {
                    carpetaPersonaOOrg = "&org=" + pDocumento.FilaDocumento.OrganizacionID + "&proy=" + pDocumento.FilaDocumento.ProyectoID;
                }

                url = "//" + mHttpContextAccessor.HttpContext.Request.Host + "/download-file?tipo=" + TipoEntidadVinculadaDocumentoTexto.BASE_RECURSOS + "&doc=" + pDocumento.Clave + "&nombre=" + HttpUtility.UrlEncode(pDocumento.NombreDocumento) + "&ext=" + pDocumento.Extension + carpetaPersonaOOrg + "&dscr=true";
            }
        }
        else if (pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Imagen))
        {
            string carpetaPersonaOOrg = "";

            if (dePersona)
            {
                carpetaPersonaOOrg = UtilArchivos.ContentImagenesPersonas + "/" + idPersonaOOrg.ToString().ToLower() + "/";
            }
            else if (deOrg)
            {
                carpetaPersonaOOrg = UtilArchivos.ContentImagenesOrganizaciones + "/" + idPersonaOOrg.ToString().ToLower() + "/";
            }
            else
            {
                carpetaPersonaOOrg = UtilArchivos.DirectorioDocumento(pDocumento.Clave) + "/";
            }

            url = mUrlBaseContent + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesDocumentos + "/" + carpetaPersonaOOrg + pDocumento.Clave.ToString().ToLower() + ".jpg";
        }
        else if (pDocumento.TipoDocumentacion.Equals(TiposDocumentacion.Video))
        {
            string carpetaPersonaOOrg = "";

            if (dePersona)
            {
                carpetaPersonaOOrg = "VideosPersonales/" + idPersonaOOrg + "/";
            }
            else if (deOrg)
            {
                carpetaPersonaOOrg = "VideosOrganizaciones/" + idPersonaOOrg + "/";
            }

            //Si es un vídeo diferente de brigthcove y top debemos poner su enclace (youtube, vimeo, ted...)
            if (!string.IsNullOrEmpty(pDocumento.Enlace) && Uri.IsWellFormedUriString(pDocumento.Enlace, UriKind.Absolute) && pDocumento.TipoDocumentacion != TiposDocumentacion.VideoBrightcove && pDocumento.TipoDocumentacion != TiposDocumentacion.VideoTOP)
            {
                url = pDocumento.Enlace;
            }
            else
            {
                //Si es un vídeo de brightcove o top ponemos su enlace normal.
                url = mUrlBaseContent + "/" + UtilArchivos.ContentVideos + "/" + carpetaPersonaOOrg + pDocumento.Clave + ".flv";
            }
        }

        return url;
    }

    /// <summary>
    /// Obtiene la URL de un documento.
    /// </summary>
    /// <param name="pDocumentoID">Identificador de un documento</param>
    /// <param name="pGestorDocumental">gestor documentación</param>
    /// <returns>Url del recurso</returns>
    public string ObtenerUrlRecurso(Guid pDocumentoID, GestorDocumental pGestorDocumental)
    {//"DocumentoID='" + pDocumentoID + "'"
        Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion.Documento filaDoc = pGestorDocumental.DataWrapperDocumentacion.ListaDocumento.Where(doc => doc.DocumentoID.Equals(pDocumentoID)).FirstOrDefault();
        Guid? ontolgoiaID = null;
        if (filaDoc.ElementoVinculadoID.HasValue)
        {
            ontolgoiaID = filaDoc.ElementoVinculadoID.Value;
        }
        string url = mGnossUrlsSemanticas.GetURLBaseRecursosFichaConIDs(mUrlBase, mUtilIdiomas, mUrlComunidad, mUrlPerfil, UtilCadenas.EliminarCaracteresUrlSem(filaDoc.Titulo), pDocumentoID, ontolgoiaID, false);

        foreach (Comentario comentario in pGestorDocumental.ListaDocumentos[pDocumentoID].Comentarios)
        {
            if (!mListaUrlsElementosSinPagina.ContainsKey(comentario.Clave))
            {
                mListaUrlsElementosSinPagina.Add(comentario.Clave, url);
            }
        }

        return url;
    }

    /// <summary>
    /// Obtiene la URL de un tag.
    /// </summary>
    /// <param name="pTag">Tag</param>
    /// <returns>Url del tag</returns>
    public string ObtenerUrlTag(string pTag)
    {
        string url = "";

        if (mUrlComunidad != "")
        {
            url += mGnossUrlsSemanticas.ObtenerURLComunidad(mUtilIdiomas, mUrlBase, mUrlComunidad);
        }
        else
        {
            url += mUrlBase;
        }

        if (!string.IsNullOrEmpty(mPestanya))
        {
            url += "/" + mPestanya + "/" + mUtilIdiomas.GetText("URLSEM", "TAG") + "/" + GnossUrlsSemanticas.SustituirCaracteresTagBusqueda(pTag);
        }
        else
        {
            url += "/" + mUtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA") + "/" + mUtilIdiomas.GetText("URLSEM", "TAG") + "/" + GnossUrlsSemanticas.SustituirCaracteresTagBusqueda(pTag);
        }

        return url;
    }

    public string ObtenerUrlPersona(Persona pPersona)
    {
        string nombreCortoPersona = pPersona.Nombre;

        if (pPersona.FilaPersona.UsuarioID.HasValue)
        {
            UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, null, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
            nombreCortoPersona = usuCN.ObtenerNombreCortoUsuarioPorID(pPersona.FilaPersona.UsuarioID.Value);
            usuCN.Dispose();
        }

        return mGnossUrlsSemanticas.GetURLPerfilPersonalPersona(mUrlBase, mUtilIdiomas, mUrlPerfil, nombreCortoPersona);
    }

    public string ObtenerUrlComentario(Guid pComentarioID)
    {
        if (mListaUrlsElementosSinPagina.ContainsKey(pComentarioID))
        {
            return mListaUrlsElementosSinPagina[pComentarioID] + "/comentario/" + pComentarioID;
        }
        else
        {
            return mUrlBase + "comentario/" + pComentarioID;
        }
    }

    public string ObtenerUrlIdentidad(Identidad pIdentidad)
    {
        ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
        string nombreCortoProy = proyCN.ObtenerNombreCortoProyecto(pIdentidad.FilaIdentidad.ProyectoID);
        proyCN.Dispose();

        return mGnossUrlsSemanticas.GetURLPerfilDeIdentidad(mUrlBase, nombreCortoProy, mUtilIdiomas, pIdentidad);
    }

    public string ObtenerUrlComunidad(string pNombreCortoProyecto)
    {
        return mGnossUrlsSemanticas.ObtenerURLComunidad(mUtilIdiomas, mUrlBase, pNombreCortoProyecto);
    }

    public string ObtenerUrlBiografia(Guid pBioID, string pTipoBio)
    {
        if (mListaUrlsElementosSinPagina.ContainsKey(pBioID))
        {
            return mListaUrlsElementosSinPagina[pBioID] + pTipoBio + "/" + pBioID;
        }
        else
        {
            return mUrlBase + pTipoBio + "/" + pBioID;
        }
    }

    /// <summary>
    /// Obtiene la url para una categoria de tesauro.
    /// </summary>
    /// <param name="pCategoriaTesauro">Categoria de tesauro</param>
    /// <returns>URL</returns>
    public string ObtenerUrlCategoriaTesauro(CategoriaTesauro pCategoriaTesauro)
    {
        return mGnossUrlsSemanticas.GetURLBaseRecursosCategoriaDocumento(mUrlBase, mUtilIdiomas, mUrlComunidad, mUrlPerfil, false, pCategoriaTesauro, mPestanya);
    }

    /// <summary>
    /// Devuelve la url base.
    /// </summary>
    public string UrlBase()
    {
        return mUrlBase;
    }

    /// <summary>
    /// Devuelve la url de la página actual.
    /// </summary>
    /// <returns></returns>
    public string UrlActual()
    {
        string url = mUrlActual;

        if (url.Contains("?"))
        {
            url = url.Substring(0, url.IndexOf("?"));
        }

        return url;
    }

    /// <summary>
    /// Devuelve la url de la página actual sin query.
    /// </summary>
    /// <returns></returns>
    public string UrlActualSinQuery()
    {
        return mUrlActualSinQuery;
    }

    #endregion
}
