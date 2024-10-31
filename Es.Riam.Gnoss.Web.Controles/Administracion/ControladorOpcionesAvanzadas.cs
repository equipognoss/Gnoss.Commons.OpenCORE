using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSName;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles.ParametroGeneralDSName;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorOpcionesAvanzadas
    {
        private Proyecto ProyectoSeleccionado = null;
        private Dictionary<string, string> mParametroProyecto = null;

        //private ParametroGeneralDS mParametrosGeneralesDS;
        //private ParametroGeneralDS.ParametroGeneralRow mFilaParametrosGenerales = null;
        private GestorParametroGeneral mParametrosGeneralesDS;
        private ParametroGeneral mFilaParametrosGenerales = null;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        private List<ParametroAplicacion> mParametroAplicacion;
        private Dictionary<string, string> mListaParametrosAplicacion;
        private static string[] LISTA_IDIOMAS = { "es", "en", "pt", "ca", "eu", "gl", "fr", "de", "it" };

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public ControladorOpcionesAvanzadas(Proyecto pProyecto, LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, EntityContextBASE entityContextBASE, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;

            mHttpContextAccessor = httpContextAccessor;
            mRedisCacheWrapper = redisCacheWrapper;
            mEntityContextBASE = entityContextBASE;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            ProyectoSeleccionado = pProyecto;
        }

        #endregion

        #region Métodos de carga

        public AdministrarOpcionesAvanzadasViewModel CargarOpcionesAvanzadas(bool pEsAdministradorEcosistema = false)
        {
            AdministrarOpcionesAvanzadasViewModel mPaginaModel = new AdministrarOpcionesAvanzadasViewModel();
            if (!pEsAdministradorEcosistema)
            {
                mPaginaModel.CMSActivado = FilaParametrosGenerales.CMSDisponible;
                if (FilaParametrosGenerales.CMSDisponible)
                {
                    mPaginaModel.GruposVisibilidadAbierto = CargarGruposVisibilidadAbierto();

                    mPaginaModel.AutocompletarSiempreVirtuoso = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "ConfigBBDDAutocompletarProyecto");
                    mPaginaModel.MostrarAccionesListados = FilaParametrosGenerales.MostrarAccionesEnListados;
                    mPaginaModel.IncluirGoogleSearch = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "IncluirGoogleSearch");

                    string ontologiaPatron = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.ProyectoIDPatronOntologias);
                    mPaginaModel.OntologiaOtroProyecto = string.IsNullOrEmpty(ontologiaPatron) ? Guid.Empty : new Guid(ontologiaPatron);
                }

                // Buscar en todo el ecosistema y el proyecto
                bool buscarTodoEcosistema = true;
                bool buscarTodoProyecto = true;
                if (ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Count > 0)
                {
                    ConfiguracionAmbitoBusquedaProyecto filaAmbitoBusqueda = ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto[0];
                    buscarTodoEcosistema = filaAmbitoBusqueda.TodoGnoss;
                    buscarTodoProyecto = filaAmbitoBusqueda.Metabusqueda;
                }
                mPaginaModel.BuscarTodoEcosistema = buscarTodoEcosistema;
                mPaginaModel.BuscarTodoProyecto = buscarTodoProyecto;
                mPaginaModel.PestanyasSeleccionadas = HayPestanyaSeleccionada(mPaginaModel);

                mPaginaModel.PermitirRecursosPrivados = FilaParametrosGenerales.PermitirRecursosPrivados;
                mPaginaModel.InvitacionesDisponibles = FilaParametrosGenerales.InvitacionesDisponibles;
                mPaginaModel.VotacionesDisponibles = FilaParametrosGenerales.VotacionesDisponibles;
                mPaginaModel.PermitirVotacionesNegativas = FilaParametrosGenerales.PermitirVotacionesNegativas;
                mPaginaModel.MostrarVotaciones = FilaParametrosGenerales.VerVotaciones;
                mPaginaModel.ComentariosDisponibles = FilaParametrosGenerales.ComentariosDisponibles;
                mPaginaModel.SupervisoresPuedenAdministrarGrupos = FilaParametrosGenerales.SupervisoresAdminGrupos;
                mPaginaModel.CuentaTwitter = ProyectoSeleccionado.FilaProyecto.UsuarioTwitter;
                mPaginaModel.HasTagTwitter = ProyectoSeleccionado.FilaProyecto.TagTwitter;
                //mPaginaModel.RobotsBusqueda = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.RobotsComunidad);
                mPaginaModel.NumeroCaracteresDescripcionSuscripcion = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.NumeroCaracteresDescripcion);
                mPaginaModel.ParametrosExtraYoutube = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.ParametrosExtraYoutube);
                mPaginaModel.CompartirRecursoPermitido = FilaParametrosGenerales.CompartirRecursosPermitido;
            }

            //if (!FilaParametrosGenerales.IsCodigoGoogleAnalyticsNull())
            //if (!(FilaParametrosGenerales.CodigoGoogleAnalytics==null))
            //{
            //    mPaginaModel.CodigoGoogleAnalytics = FilaParametrosGenerales.CodigoGoogleAnalytics;
            //}
            ////if (!FilaParametrosGenerales.IsScriptGoogleAnalyticsNull())
            //if (!(FilaParametrosGenerales.ScriptGoogleAnalytics==null))
            //{
            //    mPaginaModel.ScriptGoogleAnalytics = FilaParametrosGenerales.ScriptGoogleAnalytics;
            //}

            // Correo
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
            ConfiguracionEnvioCorreo filaConfiguracionEnvioCorreo = paramCN.ObtenerFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);
            paramCN.Dispose();

            if (filaConfiguracionEnvioCorreo != null)
            {
                mPaginaModel.ConfiguracionCorreo = new ConfiguradorCorreo();

                mPaginaModel.ConfiguracionCorreo.Email = filaConfiguracionEnvioCorreo.email;
                mPaginaModel.ConfiguracionCorreo.SMTP = filaConfiguracionEnvioCorreo.smtp;
                mPaginaModel.ConfiguracionCorreo.Port = filaConfiguracionEnvioCorreo.puerto;
                mPaginaModel.ConfiguracionCorreo.User = filaConfiguracionEnvioCorreo.usuario;
                mPaginaModel.ConfiguracionCorreo.Type = filaConfiguracionEnvioCorreo.tipo;
                mPaginaModel.ConfiguracionCorreo.SSL = filaConfiguracionEnvioCorreo.SSL.HasValue && filaConfiguracionEnvioCorreo.SSL.Value == true;
                mPaginaModel.ConfiguracionCorreo.SuggestEmail = filaConfiguracionEnvioCorreo.emailsugerencias;
                mPaginaModel.ConfiguracionCorreo.Password = filaConfiguracionEnvioCorreo.clave;
            }
            return mPaginaModel;
        }

        public void CargarBuzonCorreo(AdministrarOpcionesAvanzadasViewModel pOpcionesAvanzadasModel)
        {
            ParametroCN paramCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
            ConfiguracionEnvioCorreo filaConfiguracionEnvioCorreo = paramCN.ObtenerFilaConfiguracionEnvioCorreo(ProyectoSeleccionado.Clave);
            paramCN.Dispose();

            if (filaConfiguracionEnvioCorreo != null)
            {
                pOpcionesAvanzadasModel.ConfiguracionCorreo = new ConfiguradorCorreo();

                pOpcionesAvanzadasModel.ConfiguracionCorreo.Email = filaConfiguracionEnvioCorreo.email;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.SMTP = filaConfiguracionEnvioCorreo.smtp;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.Port = filaConfiguracionEnvioCorreo.puerto;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.User = filaConfiguracionEnvioCorreo.usuario;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.Type = filaConfiguracionEnvioCorreo.tipo;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.SSL = filaConfiguracionEnvioCorreo.SSL.HasValue && filaConfiguracionEnvioCorreo.SSL.Value == true;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.SuggestEmail = filaConfiguracionEnvioCorreo.emailsugerencias;
                pOpcionesAvanzadasModel.ConfiguracionCorreo.Password = filaConfiguracionEnvioCorreo.clave;
            }
        }

        public AdministrarOpcionesAvanzadasPlataformaViewModel CargarOpcionesAvanzadasEcosistema()
        {
            AdministrarOpcionesAvanzadasPlataformaViewModel opciones = new AdministrarOpcionesAvanzadasPlataformaViewModel();

            //Parámetros string
            opciones.CodigoGoogleAnalyticsProyecto = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto);
            opciones.DominiosPermitidosCORS = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosPermitidosCORS);
            opciones.ConexionEntornoPreproduccion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ConexionEntornoPreproduccion);
            opciones.CorreoSolicitudes = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CorreoSolicitudes);
            opciones.CorreoSugerencias = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.CorreoSugerencias);
            opciones.DominiosSinPalco = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosSinPalco);
            opciones.HashTagEntorno = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.HashTagEntorno);

            string idiomas = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Idiomas);
            if (string.IsNullOrEmpty(idiomas))
            {
                idiomas = mConfigService.ObtenerListaIdiomas().ToString();
            }
            opciones.Idiomas = idiomas;

            CargarModeloIdiomasPersonalizados(idiomas, opciones);

            opciones.LoginFacebook = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginFacebook);
            opciones.LoginGoogle = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginGoogle);
            opciones.LoginTwitter = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginTwitter);
            opciones.NombreEspacioPersonal = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.NombreEspacioPersonal);
            opciones.Copyright = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.Copyright);
            opciones.VisibilidadPerfil = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.VisibilidadPerfil);
            opciones.OntologiasNoLive = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.OntologiasNoLive);
            opciones.ImplementationKey = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ImplementationKey);
            opciones.UrlHomeConectado = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlHomeConectado);
            opciones.GoogleRecaptchaSecret = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GoogleRecaptchaSecret);
            opciones.DominiosEmailLoginRedesSociales = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DominiosEmailLoginRedesSociales);
            opciones.UrlsPropiasProyecto = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlsPropiasProyecto);
            opciones.DuracionCookieUsuario = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.DuracionCookieUsuario);
            opciones.ExtensionesImagenesCMSMultimedia = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ExtensionesImagenesCMSMultimedia);
            opciones.ExtensionesDocumentosCMSMultimedia = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ExtensionesDocumentosCMSMultimedia);

            opciones.ipFTP = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ipFTP);
            opciones.UrlContent = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlContent);
            opciones.UrlIntragnoss = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlIntragnoss);
            opciones.UrlIntragnossServicios = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlIntragnossServicios);
            opciones.UrlBaseService = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.UrlBaseService);
            opciones.ScriptGoogleAnalytics = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ScriptGoogleAnalytics);
            opciones.ComunidadesExcluidaPersonalizacion = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion);
            opciones.LoginUnicoUsuariosExcluidos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos);
            opciones.PasosAsistenteCreacionComunidad = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.PasosAsistenteCreacionComunidad);
            opciones.GrafoMetaBusquedaRecursos = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaRecursos);
            opciones.GrafoMetaBusquedaPerYOrg = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaPerYOrg);
            opciones.GrafoMetaBusquedaComunidades = ControladorProyecto.ObtenerParametroString(ListaParametrosAplicacion, TiposParametrosAplicacion.GrafoMetaBusquedaComunidades);
            //Parámetros booleanos
            opciones.EcosistemaSinBandejaSuscripciones = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones, true);
            opciones.EcosistemaSinContactos = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinContactos, true);
            opciones.VersionFotoDocumentoNegativo = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.VersionFotoDocumentoNegativo);
            opciones.CVUnicoPorPerfil = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.CVUnicoPorPerfil);
            opciones.DatosDemograficosPerfil = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.DatosDemograficosPerfil, true);
            opciones.EcosistemaSinMetaproyecto = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinMetaProyecto);
            opciones.PanelMensajeImportarContactos = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PanelMensajeImportarContactos, true);
            opciones.PerfilGlobalEnComunidadPrincipal = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal);
            opciones.PestanyaImportarContactosCorreo = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PestanyaImportarContactosCorreo, true);
            opciones.RegistroAutomaticoEcosistema = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.RegistroAutomaticoEcosistema, true);
            opciones.SeguirEnTodaLaActividad = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.SeguirEnTodaLaActividad);
            opciones.EcosistemaSinHomeUsuario = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EcosistemaSinHomeUsuario, true);
            opciones.MostrarGruposIDEnHtml = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.MostrarGruposIDEnHtml);
            opciones.UsarSoloCategoriasPrivadasEnEspacioPersonal = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.UsarSoloCategoriasPrivadasEnEspacioPersonal);
            opciones.NotificacionesAgrupadas = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.NotificacionesAgrupadas);
            opciones.RecibirNewsletterDefecto = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.RecibirNewsletterDefecto);
            opciones.PerfilPersonalDisponible = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.PerfilPersonalDisponible, true);
            opciones.GenerarGrafoContribuciones = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.GenerarGrafoContribuciones, true);
            opciones.MantenerSesionActiva = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.MantenerSesionActiva, true);
            opciones.NoEnviarCorreoSeguirPerfil = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil, false);
            opciones.LoginUnicoPorUsuario = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.LoginUnicoPorUsuario, true);
            opciones.EnviarNotificacionesDeSuscripciones = ControladorProyecto.ObtenerParametroBooleano(ListaParametrosAplicacion, TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones, true);


            //Parámetros Int
            opciones.EdadLimiteRegistroEcosistema = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.EdadLimiteRegistroEcosistema);
            opciones.SegundosMaxSesionBloqueada = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.SegundosMaxSesionBloqueada);
            opciones.TamanioPoolRedis = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.TamanioPoolRedis);
            opciones.UbicacionLogs = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.UbicacionLogs);
            opciones.UbicacionTrazas = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.UbicacionTrazas);

            opciones.puertoFTP = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.puertoFTP);
            opciones.VersionJSEcosistema = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.VersionJSEcosistema);
            opciones.VersionCSSEcosistema = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.VersionCSSEcosistema);
            opciones.AceptacionComunidadesAutomatica = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.AceptacionComunidadesAutomatica);
            opciones.TipoCabecera = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.TipoCabecera);
            opciones.TamanioPoolRedis = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.TamanioPoolRedis);
            opciones.usarHTTPSParaDominioPrincipal = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.usarHTTPSParaDominioPrincipal);
            opciones.CargarIdentidadesDeProyectosPrivadosComoAmigos = ControladorProyecto.ObtenerParametroInt(ListaParametrosAplicacion, TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos);

            return opciones;
        }

        public void InvalidarCachesEcosistema()
        {
            ParametroAplicacionCL parametroAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            parametroAplicacionCL.InvalidarCacheParametrosAplicacion();
            parametroAplicacionCL.Dispose();
            if (mGnossCache == null)
            {
                mGnossCache = new GnossCache(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            mGnossCache.VersionarCacheLocal(ProyectoSeleccionado.Clave);
            mGnossCache.VersionarCacheLocal(Guid.Empty);
        }

        public void GuardarOpcionesAvanzadasEcosistema(AdministrarOpcionesAvanzadasPlataformaViewModel pOptions)
        {
            if (!string.IsNullOrEmpty(pOptions.UrlBaseService))
            {
                if (pOptions.UrlBaseService.Contains("{ServiceName}"))
                {
                    GuardarParametroString(TiposParametrosAplicacion.UrlBaseService, pOptions.UrlBaseService);
                }
                else
                {
                    throw new Exception("La UrlBaseService no contiene {ServiceName}");
                }
            }

            GuardarParametroString(TiposParametrosAplicacion.CodigoGoogleAnalyticsProyecto, pOptions.CodigoGoogleAnalyticsProyecto);
            GuardarParametroString(TiposParametrosAplicacion.DominiosPermitidosCORS, pOptions.DominiosPermitidosCORS);
            GuardarParametroString(TiposParametrosAplicacion.ConexionEntornoPreproduccion, pOptions.ConexionEntornoPreproduccion);
            GuardarParametroString(TiposParametrosAplicacion.Copyright, pOptions.Copyright);
            GuardarParametroString(TiposParametrosAplicacion.CorreoSolicitudes, pOptions.CorreoSolicitudes);
            GuardarParametroString(TiposParametrosAplicacion.CorreoSugerencias, pOptions.CorreoSugerencias);
            GuardarParametroString(TiposParametrosAplicacion.DominiosEmailLoginRedesSociales, pOptions.DominiosEmailLoginRedesSociales);
            if (!string.IsNullOrEmpty(pOptions.UrlsPropiasProyecto))
            {
                GuardarParametroString(TiposParametrosAplicacion.UrlsPropiasProyecto, pOptions.UrlsPropiasProyecto);
            }
            GuardarParametroString(TiposParametrosAplicacion.DominiosSinPalco, pOptions.DominiosSinPalco);
            GuardarParametroString(TiposParametrosAplicacion.GoogleRecaptchaSecret, pOptions.GoogleRecaptchaSecret);
            GuardarParametroString(TiposParametrosAplicacion.HashTagEntorno, pOptions.HashTagEntorno);
            GuardarParametroString(TiposParametrosAplicacion.Idiomas, pOptions.Idiomas);
            GuardarParametroString(TiposParametrosAplicacion.ImplementationKey, pOptions.ImplementationKey);
            GuardarParametroString(TiposParametrosAplicacion.LoginFacebook, pOptions.LoginFacebook);
            GuardarParametroString(TiposParametrosAplicacion.LoginGoogle, pOptions.LoginGoogle);
            GuardarParametroString(TiposParametrosAplicacion.LoginTwitter, pOptions.LoginTwitter);
            GuardarParametroString(TiposParametrosAplicacion.NombreEspacioPersonal, pOptions.NombreEspacioPersonal);
            GuardarParametroString(TiposParametrosAplicacion.OntologiasNoLive, pOptions.OntologiasNoLive);
            GuardarParametroString(TiposParametrosAplicacion.UrlHomeConectado, pOptions.UrlHomeConectado);
            GuardarParametroString(TiposParametrosAplicacion.VisibilidadPerfil, pOptions.VisibilidadPerfil);
            GuardarParametroString(TiposParametrosAplicacion.DuracionCookieUsuario, pOptions.DuracionCookieUsuario);
            GuardarParametroString(TiposParametrosAplicacion.ExtensionesImagenesCMSMultimedia, pOptions.ExtensionesImagenesCMSMultimedia);
            GuardarParametroString(TiposParametrosAplicacion.ExtensionesDocumentosCMSMultimedia, pOptions.ExtensionesDocumentosCMSMultimedia);


            GuardarParametroString(TiposParametrosAplicacion.ipFTP, pOptions.ipFTP);
            GuardarParametroString(TiposParametrosAplicacion.UrlContent, pOptions.UrlContent);
            GuardarParametroString(TiposParametrosAplicacion.UrlIntragnoss, pOptions.UrlIntragnoss);
            GuardarParametroString(TiposParametrosAplicacion.UrlIntragnossServicios, pOptions.UrlIntragnossServicios);

            GuardarParametroString(TiposParametrosAplicacion.ScriptGoogleAnalytics, pOptions.ScriptGoogleAnalytics);
            GuardarParametroString(TiposParametrosAplicacion.ComunidadesExcluidaPersonalizacion, pOptions.ComunidadesExcluidaPersonalizacion);
            GuardarParametroString(TiposParametrosAplicacion.LoginUnicoUsuariosExcluidos, pOptions.LoginUnicoUsuariosExcluidos);
            GuardarParametroString(TiposParametrosAplicacion.PasosAsistenteCreacionComunidad, pOptions.PasosAsistenteCreacionComunidad);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaRecursos, pOptions.GrafoMetaBusquedaRecursos);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaPerYOrg, pOptions.GrafoMetaBusquedaPerYOrg);
            GuardarParametroString(TiposParametrosAplicacion.GrafoMetaBusquedaComunidades, pOptions.GrafoMetaBusquedaComunidades);
            GuardarParametroString(TiposParametrosAplicacion.EdadLimiteRegistroEcosistema, pOptions.EdadLimiteRegistroEcosistema > 0 ? pOptions.EdadLimiteRegistroEcosistema.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.SegundosMaxSesionBloqueada, pOptions.SegundosMaxSesionBloqueada > 0 ? pOptions.SegundosMaxSesionBloqueada.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.UbicacionLogs, pOptions.UbicacionLogs > 0 ? pOptions.UbicacionLogs.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.UbicacionTrazas, pOptions.UbicacionTrazas > 0 ? pOptions.UbicacionTrazas.ToString() : null);

            GuardarParametroString(TiposParametrosAplicacion.puertoFTP, pOptions.puertoFTP > 0 ? pOptions.puertoFTP.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.VersionJSEcosistema, pOptions.VersionJSEcosistema > 0 ? pOptions.VersionJSEcosistema.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.VersionCSSEcosistema, pOptions.VersionCSSEcosistema > 0 ? pOptions.VersionCSSEcosistema.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.AceptacionComunidadesAutomatica, pOptions.AceptacionComunidadesAutomatica > 0 ? pOptions.AceptacionComunidadesAutomatica.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.TipoCabecera, pOptions.TipoCabecera > 0 ? pOptions.TipoCabecera.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.TamanioPoolRedis, pOptions.TamanioPoolRedis > 0 ? pOptions.TamanioPoolRedis.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.usarHTTPSParaDominioPrincipal, pOptions.usarHTTPSParaDominioPrincipal > 0 ? pOptions.usarHTTPSParaDominioPrincipal.ToString() : null);
            GuardarParametroString(TiposParametrosAplicacion.CargarIdentidadesDeProyectosPrivadosComoAmigos, pOptions.CargarIdentidadesDeProyectosPrivadosComoAmigos > 0 ? pOptions.CargarIdentidadesDeProyectosPrivadosComoAmigos.ToString() : null);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinBandejaSuscripciones, pOptions.EcosistemaSinBandejaSuscripciones, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinContactos, pOptions.EcosistemaSinContactos, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.VersionFotoDocumentoNegativo, pOptions.VersionFotoDocumentoNegativo);
            GuardarParametroBooleano(TiposParametrosAplicacion.CVUnicoPorPerfil, pOptions.CVUnicoPorPerfil);
            GuardarParametroBooleano(TiposParametrosAplicacion.DatosDemograficosPerfil, pOptions.DatosDemograficosPerfil, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinMetaProyecto, pOptions.EcosistemaSinMetaproyecto);
            GuardarParametroBooleano(TiposParametrosAplicacion.PanelMensajeImportarContactos, pOptions.PanelMensajeImportarContactos, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.PerfilGlobalEnComunidadPrincipal, pOptions.PerfilGlobalEnComunidadPrincipal);
            GuardarParametroBooleano(TiposParametrosAplicacion.PestanyaImportarContactosCorreo, pOptions.PestanyaImportarContactosCorreo, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.RegistroAutomaticoEcosistema, pOptions.RegistroAutomaticoEcosistema, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.SeguirEnTodaLaActividad, pOptions.SeguirEnTodaLaActividad);
            GuardarParametroBooleano(TiposParametrosAplicacion.MostrarGruposIDEnHtml, pOptions.MostrarGruposIDEnHtml);
            GuardarParametroBooleano(TiposParametrosAplicacion.UsarSoloCategoriasPrivadasEnEspacioPersonal, pOptions.UsarSoloCategoriasPrivadasEnEspacioPersonal);
            GuardarParametroBooleano(TiposParametrosAplicacion.EcosistemaSinHomeUsuario, pOptions.EcosistemaSinHomeUsuario, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.NotificacionesAgrupadas, pOptions.NotificacionesAgrupadas);
            GuardarParametroBooleano(TiposParametrosAplicacion.RecibirNewsletterDefecto, pOptions.RecibirNewsletterDefecto);
            GuardarParametroBooleano(TiposParametrosAplicacion.PerfilPersonalDisponible, pOptions.PerfilPersonalDisponible, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.GenerarGrafoContribuciones, pOptions.GenerarGrafoContribuciones, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.MantenerSesionActiva, pOptions.MantenerSesionActiva, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.NoEnviarCorreoSeguirPerfil, pOptions.NoEnviarCorreoSeguirPerfil, false);
            GuardarParametroBooleano(TiposParametrosAplicacion.LoginUnicoPorUsuario, pOptions.LoginUnicoPorUsuario, true);
            GuardarParametroBooleano(TiposParametrosAplicacion.EnviarNotificacionesDeSuscripciones, pOptions.EnviarNotificacionesDeSuscripciones, true);
        }

        private void GuardarParametroString(string pNombreParametro, string pValor)
        {
            AD.EntityModel.ParametroAplicacion filaParametro = mEntityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(pNombreParametro));
            if (!string.IsNullOrEmpty(pValor))
            {
                if (filaParametro != null && !filaParametro.Valor.Equals(pValor))
                {
                    filaParametro.Valor = pValor;
                }
                else if (filaParametro == null)
                {
                    mEntityContext.ParametroAplicacion.Add(new ParametroAplicacion(pNombreParametro, pValor));
                }
            }
            else if (filaParametro != null)
            {
                mEntityContext.ParametroAplicacion.Remove(filaParametro);
            }
        }

        private void GuardarParametroBooleano(string pNombreParametro, bool pValor, bool pValorPorDefecto = false, string pValorTrue = "true", string pValorFalse = "false")
        {
            if (pValor != pValorPorDefecto)
            {
                GuardarParametroString(pNombreParametro, pValor ? pValorTrue : pValorFalse);
            }
            else
            {
                GuardarParametroString(pNombreParametro, null);
            }
        }

        // Carga los idiomas personalizas en el modelo
        private void CargarModeloIdiomasPersonalizados(string pIdiomas, AdministrarOpcionesAvanzadasPlataformaViewModel pOpciones)
        {
            string idiomasPersonalizados = "";

            string[] idiomas = pIdiomas.Split("&&&");
            foreach (string idioma in idiomas)
            {
                string clave = idioma.Substring(0, 2);
                if (!LISTA_IDIOMAS.Contains(clave))
                {
                    idiomasPersonalizados += $"{idioma}&&&";
                }
            }
            if (!string.IsNullOrEmpty(idiomasPersonalizados))
            {
                // ELiminar las tres ultimos caracteres
                pOpciones.IdiomasPersonalizados = idiomasPersonalizados.Substring(0, idiomasPersonalizados.Length - 3);
            }
        }

        private Dictionary<string, string> ListaParametrosAplicacion
        {
            get
            {
                if (mListaParametrosAplicacion == null)
                {
                    mListaParametrosAplicacion = ParametroAplicacion.ToDictionary(parametro => parametro.Parametro, parametro => parametro.Valor);
                }
                return mListaParametrosAplicacion;
            }
        }

        private List<ParametroAplicacion> ParametroAplicacion
        {

            get
            {
                if (mParametroAplicacion == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mParametroAplicacion = mEntityContext.ParametroAplicacion.ToList();
                }
                return mParametroAplicacion;
            }
        }

        public void CargarInteraccionSocial(AdministrarOpcionesAvanzadasViewModel pOpcionesAvanzadasModel)
        {
            pOpcionesAvanzadasModel.CMSActivado = FilaParametrosGenerales.CMSDisponible;
            if (FilaParametrosGenerales.CMSDisponible)
            {
                pOpcionesAvanzadasModel.GruposVisibilidadAbierto = CargarGruposVisibilidadAbierto();

                pOpcionesAvanzadasModel.AutocompletarSiempreVirtuoso = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "ConfigBBDDAutocompletarProyecto");
                pOpcionesAvanzadasModel.MostrarAccionesListados = FilaParametrosGenerales.MostrarAccionesEnListados;
                pOpcionesAvanzadasModel.IncluirGoogleSearch = ControladorProyecto.ObtenerParametroBooleano(ParametroProyecto, "IncluirGoogleSearch");

                string ontologiaPatron = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.ProyectoIDPatronOntologias);
                pOpcionesAvanzadasModel.OntologiaOtroProyecto = string.IsNullOrEmpty(ontologiaPatron) ? Guid.Empty : new Guid(ontologiaPatron);
            }

            // Buscar en todo el ecosistema y el proyecto
            bool buscarTodoEcosistema = true;
            bool buscarTodoProyecto = true;
            if (ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Count > 0)
            {
                ConfiguracionAmbitoBusquedaProyecto filaAmbitoBusqueda = ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto[0];
                buscarTodoEcosistema = filaAmbitoBusqueda.TodoGnoss;
                buscarTodoProyecto = filaAmbitoBusqueda.Metabusqueda;
            }
            pOpcionesAvanzadasModel.BuscarTodoEcosistema = buscarTodoEcosistema;
            pOpcionesAvanzadasModel.BuscarTodoProyecto = buscarTodoProyecto;
            pOpcionesAvanzadasModel.PestanyasSeleccionadas = HayPestanyaSeleccionada(pOpcionesAvanzadasModel);

            pOpcionesAvanzadasModel.PermitirRecursosPrivados = FilaParametrosGenerales.PermitirRecursosPrivados;
            pOpcionesAvanzadasModel.InvitacionesDisponibles = FilaParametrosGenerales.InvitacionesDisponibles;
            pOpcionesAvanzadasModel.VotacionesDisponibles = FilaParametrosGenerales.VotacionesDisponibles;
            pOpcionesAvanzadasModel.PermitirVotacionesNegativas = FilaParametrosGenerales.PermitirVotacionesNegativas;
            pOpcionesAvanzadasModel.MostrarVotaciones = FilaParametrosGenerales.VerVotaciones;
            pOpcionesAvanzadasModel.ComentariosDisponibles = FilaParametrosGenerales.ComentariosDisponibles;
            pOpcionesAvanzadasModel.SupervisoresPuedenAdministrarGrupos = FilaParametrosGenerales.SupervisoresAdminGrupos;
            pOpcionesAvanzadasModel.CuentaTwitter = ProyectoSeleccionado.FilaProyecto.UsuarioTwitter;
            pOpcionesAvanzadasModel.HasTagTwitter = ProyectoSeleccionado.FilaProyecto.TagTwitter;
            pOpcionesAvanzadasModel.NumeroCaracteresDescripcionSuscripcion = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.NumeroCaracteresDescripcion);
            pOpcionesAvanzadasModel.ParametrosExtraYoutube = ControladorProyecto.ObtenerParametroString(ParametroProyecto, ParametroAD.ParametrosExtraYoutube);
            pOpcionesAvanzadasModel.CompartirRecursoPermitido = FilaParametrosGenerales.CompartirRecursosPermitido;
        }

        private Dictionary<Guid, string> CargarGruposVisibilidadAbierto()
        {
            string gruposVisibilidadAbierto = ControladorProyecto.ObtenerParametroString(ParametroProyecto, "GruposPermitidosSeleccionarPrivacidadRecursoAbierto");
            List<Guid> listaGrupos = new List<Guid>();

            if (!string.IsNullOrEmpty(gruposVisibilidadAbierto))
            {
                string[] arrayGrupos = gruposVisibilidadAbierto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string grupo in arrayGrupos)
                {
                    Guid grupoID = Guid.Empty;
                    Guid.TryParse(grupo, out grupoID);

                    if (grupoID != Guid.Empty)
                    {
                        listaGrupos.Add(grupoID);
                    }
                }
            }

            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, null);
            Dictionary<Guid, string> nombresGrupos = identidadCN.ObtenerNombresDeGrupos(listaGrupos);
            identidadCN.Dispose();

            return nombresGrupos;
        }

        #endregion

        #region Métodos de Guardado

        public void GuardarOpcionesAvanzadas(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            PasarDatosADataSet(pOptions);
            mEntityContext.NoConfirmarTransacciones = true;
            try
            {
                mEntityContext.SaveChanges();
                                
                mEntityContext.TerminarTransaccionesPendientes(true);
            }
            catch
            {
                mEntityContext.TerminarTransaccionesPendientes(false);
                throw;
            }
        }

        

        private void PasarDatosADataSet(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ControladorProyecto controlador = new ControladorProyecto(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mGnossCache, mEntityContextBASE, mVirtuosoAD, mHttpContextAccessor, null);

            if (FilaParametrosGenerales.CMSDisponible)
            {
                string gruposVisibilidadAbierto = "";
                if (pOptions.GruposVisibilidadAbierto != null)
                {
                    foreach (Guid grupo in pOptions.GruposVisibilidadAbierto.Keys)
                    {
                        gruposVisibilidadAbierto += grupo.ToString() + "|||";
                    }
                }
                controlador.GuardarParametroString(ParametrosGeneralesDS, "GruposPermitidosSeleccionarPrivacidadRecursoAbierto", gruposVisibilidadAbierto);

                string ontologiaPatron = pOptions.OntologiaOtroProyecto == Guid.Empty ? "" : pOptions.OntologiaOtroProyecto.ToString();
                controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.ProyectoIDPatronOntologias, ontologiaPatron);

                FilaParametrosGenerales.MostrarAccionesEnListados = pOptions.MostrarAccionesListados;
                controlador.GuardarParametroBooleano(ParametrosGeneralesDS, "IncluirGoogleSearch", pOptions.IncluirGoogleSearch);
                controlador.GuardarParametroBooleano(ParametrosGeneralesDS, "ConfigBBDDAutocompletarProyecto", pOptions.AutocompletarSiempreVirtuoso);
            }

            //Guardar la opción seleccionada
           
            PestanyasBusquedaProyecto(pOptions);

            FilaParametrosGenerales.PermitirRecursosPrivados = pOptions.PermitirRecursosPrivados;
            FilaParametrosGenerales.InvitacionesDisponibles = pOptions.InvitacionesDisponibles;
            FilaParametrosGenerales.VotacionesDisponibles = pOptions.VotacionesDisponibles;
            FilaParametrosGenerales.PermitirVotacionesNegativas = pOptions.PermitirVotacionesNegativas;
            FilaParametrosGenerales.VerVotaciones = pOptions.MostrarVotaciones;
            FilaParametrosGenerales.ComentariosDisponibles = pOptions.ComentariosDisponibles;
            FilaParametrosGenerales.SupervisoresAdminGrupos = pOptions.SupervisoresPuedenAdministrarGrupos;
            FilaParametrosGenerales.CompartirRecursosPermitido = pOptions.CompartirRecursoPermitido;

            //if (!string.IsNullOrEmpty(pOptions.CodigoGoogleAnalytics))
            //{
            //    FilaParametrosGenerales.CodigoGoogleAnalytics = pOptions.CodigoGoogleAnalytics;
            //}
            ////else if (!FilaParametrosGenerales.IsCodigoGoogleAnalyticsNull())
            //else if (!(FilaParametrosGenerales.CodigoGoogleAnalytics==null))
            //{
            //    FilaParametrosGenerales.CodigoGoogleAnalytics = null;
            //    //FilaParametrosGenerales.SetCodigoGoogleAnalyticsNull();
            //}

            //if (!string.IsNullOrEmpty(pOptions.ScriptGoogleAnalytics))
            //{
            //    FilaParametrosGenerales.ScriptGoogleAnalytics = pOptions.ScriptGoogleAnalytics;
            //}
            ////else if (!FilaParametrosGenerales.IsScriptGoogleAnalyticsNull())
            //else if (!(FilaParametrosGenerales.ScriptGoogleAnalytics==null))
            //{
            //    FilaParametrosGenerales.ScriptGoogleAnalytics = null;
            //}

            //controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.RobotsComunidad, pOptions.RobotsBusqueda);
            controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.NumeroCaracteresDescripcion, pOptions.NumeroCaracteresDescripcionSuscripcion);
            controlador.GuardarParametroString(ParametrosGeneralesDS, ParametroAD.ParametrosExtraYoutube, pOptions.ParametrosExtraYoutube);

            PasarDatosAmbitoBusquedaADataSet(pOptions);
            PasarDatosTwitterADataSet(pOptions);
        }

        private void PasarDatosAmbitoBusquedaADataSet(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ConfiguracionAmbitoBusquedaProyecto filaAmbitoBusqueda;
            if (ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Count > 0)
            {
                filaAmbitoBusqueda = ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto[0];
                filaAmbitoBusqueda.TodoGnoss = pOptions.BuscarTodoEcosistema;
                filaAmbitoBusqueda.Metabusqueda = pOptions.BuscarTodoProyecto;
            }
            else 
            {
                filaAmbitoBusqueda = new ConfiguracionAmbitoBusquedaProyecto();
                filaAmbitoBusqueda.ProyectoID = ProyectoSeleccionado.Clave;
                filaAmbitoBusqueda.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                filaAmbitoBusqueda.TodoGnoss = pOptions.BuscarTodoEcosistema;
                filaAmbitoBusqueda.Metabusqueda = pOptions.BuscarTodoProyecto;

                if (pOptions.PestanyasSeleccionadas.HasValue && !pOptions.PestanyasSeleccionadas.Value.Equals(Guid.Empty))
                {
                    filaAmbitoBusqueda.PestanyaDefectoID = pOptions.PestanyasSeleccionadas;
                }

                ParametrosGeneralesDS.ListaConfiguracionAmbitoBusquedaProyecto.Add(filaAmbitoBusqueda);
                ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                gestorController.addAmbitoBusqueda(filaAmbitoBusqueda);
                //gestorController.saveChanges();
                //filaAmbitoBusqueda.SetPestanyaDefectoIDNull();
                //ParametrosGeneralesDS.ConfiguracionAmbitoBusquedaProyecto.AddConfiguracionAmbitoBusquedaProyectoRow(filaAmbitoBusqueda);
            }
        }

        private void PasarDatosTwitterADataSet(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            if (string.IsNullOrEmpty(pOptions.CuentaTwitter))
            {
                ProyectoSeleccionado.FilaProyecto.TieneTwitter = false;
                ProyectoSeleccionado.FilaProyecto.UsuarioTwitter = null;
                ProyectoSeleccionado.FilaProyecto.TagTwitter = null;

                ProyectoSeleccionado.FilaProyecto.TokenTwitter = null;
                ProyectoSeleccionado.FilaProyecto.TokenSecretoTwitter = null;
            }
            else
            {
                ProyectoSeleccionado.FilaProyecto.TieneTwitter = true;
                ProyectoSeleccionado.FilaProyecto.UsuarioTwitter = pOptions.CuentaTwitter;
                ProyectoSeleccionado.FilaProyecto.TagTwitter = pOptions.HasTagTwitter;

                if (!string.IsNullOrEmpty(pOptions.TokenTwitter))
                {
                    ProyectoSeleccionado.FilaProyecto.TokenTwitter = pOptions.TokenTwitter;
                    ProyectoSeleccionado.FilaProyecto.TokenSecretoTwitter = pOptions.TokenSecretTwitter;
                }
            }
        }
        private void PestanyasBusquedaProyecto(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            if (pOptions.PestanyasSeleccionadas.HasValue && !pOptions.PestanyasSeleccionadas.Value.Equals(Guid.Empty))
            {
                ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                ConfiguracionAmbitoBusquedaProyecto confBusqueda = gestorController.ObtenerConfiguracionAmbitoBusqueda(ProyectoSeleccionado.Clave);
                confBusqueda.PestanyaDefectoID = pOptions.PestanyasSeleccionadas;
            }
        }
        private Guid? HayPestanyaSeleccionada(AdministrarOpcionesAvanzadasViewModel pOptions)
        {
            ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
            ConfiguracionAmbitoBusquedaProyecto confBusqueda = gestorController.ObtenerConfiguracionAmbitoBusqueda(ProyectoSeleccionado.Clave);
            if (confBusqueda != null)
            {
                return confBusqueda.PestanyaDefectoID;
            }
            else
            {
                return null;
            }
            
        }

        #endregion

        #region Invalidar Cache

        public void InvalidarCaches()
        {
            ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, null);
            paramCL.InvalidarCacheParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, null);
            proyCL.InvalidarParametrosProyecto(ProyectoSeleccionado.Clave, ProyectoSeleccionado.FilaProyecto.OrganizacionID);
            proyCL.InvalidarFilaProyecto(ProyectoSeleccionado.Clave);

            proyCL.InvalidarComunidadMVC(ProyectoSeleccionado.Clave);
            proyCL.InvalidarCabeceraMVC(ProyectoSeleccionado.Clave);

            proyCL.Dispose();
        }

        #endregion

        #region Propiedades

        private ParametroGeneral FilaParametrosGenerales
        {
            get
            {
                if (mFilaParametrosGenerales == null)
                {
                    //mFilaParametrosGenerales = ParametrosGeneralesDS.ParametroGeneral.FindByOrganizacionIDProyectoID(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave);
                    mFilaParametrosGenerales = ParametrosGeneralesDS.ListaParametroGeneral.Find(parametrosGenerales=> parametrosGenerales.OrganizacionID.Equals(ProyectoSeleccionado.FilaProyecto.OrganizacionID) && parametrosGenerales.ProyectoID.Equals(ProyectoSeleccionado.Clave));
                }
                return mFilaParametrosGenerales;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        private Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ParametroCN parametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, null);
                    mParametroProyecto = parametroCN.ObtenerParametrosProyecto(ProyectoSeleccionado.Clave);
                    parametroCN.Dispose();
                }

                return mParametroProyecto;
            }
        }

        /// <summary>
        /// Obtiene el dataset de parámetros generales
        /// </summary>
        // private ParametroGeneralDS ParametrosGeneralesDS
        private GestorParametroGeneral ParametrosGeneralesDS
        {
            get
            {
                if (mParametrosGeneralesDS == null)
                {
                    //ParametroGeneralCN paramCN = new ParametroGeneralCN();
                    //mParametrosGeneralesDS = paramCN.ObtenerParametrosGeneralesDeProyecto(ProyectoSeleccionado.Clave);
                    //paramCN.Dispose();
                    ParametroGeneralGBD gestorController = new ParametroGeneralGBD(mEntityContext);
                    mParametrosGeneralesDS = new GestorParametroGeneral();
                    mParametrosGeneralesDS = gestorController.ObtenerParametrosGeneralesDeProyecto(mParametrosGeneralesDS, ProyectoSeleccionado.Clave); 
                    foreach (string parametro in ParametroProyecto.Keys)
                    {
                        ParametroProyecto parametroProyecto = new ParametroProyecto(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, parametro, ParametroProyecto[parametro]);
                        //mParametrosGeneralesDS.ParametroProyecto.AddParametroProyectoRow(ProyectoSeleccionado.FilaProyecto.OrganizacionID, ProyectoSeleccionado.Clave, parametro, ParametroProyecto[parametro]);
                        mParametrosGeneralesDS.ListaParametroProyecto.Add(parametroProyecto);
                    }
                    //gestorController.saveChanges();
                    //mParametrosGeneralesDS.ParametroProyecto.AcceptChanges();
                }
                return mParametrosGeneralesDS;
            }
        }

        #endregion
    }
}
