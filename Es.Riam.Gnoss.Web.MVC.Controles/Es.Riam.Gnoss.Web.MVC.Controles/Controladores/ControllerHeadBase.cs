﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ControllerHeadBase : ControladorBase
    {
        protected ControllerBaseGnoss ControllerBase
        {
            get; set;
        }

        protected dynamic ViewBag;

        protected Identidad IdentidadActual;
        protected Proyecto ProyectoSeleccionado;
        protected Proyecto ProyectoVirtual;

        protected EntityContext mEntityContext;
        protected LoggingService mLoggingService;
        protected VirtuosoAD mVirtuosoAD;
        protected ConfigService mConfigService;
        protected IHttpContextAccessor mHttpContextAccessor;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected GnossCache mGnossCache;
        protected EntityContextBASE mEntityContextBASE;

        public ControllerHeadBase(ControllerBaseGnoss pControllerBase, LoggingService loggingService, ConfigService configService, EntityContext entityContext, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextBASE = entityContextBASE;
            mLoggingService = loggingService;
            mVirtuosoAD = virtuosoAD;
            mConfigService = configService;
            mEntityContext = entityContext;
            mRedisCacheWrapper = redisCacheWrapper;
            mHttpContextAccessor = httpContextAccessor;
            mGnossCache = gnossCache;

            ControllerBase = pControllerBase;

            ViewBag = pControllerBase.ViewBag;

            IdentidadActual = ControllerBase.IdentidadActual;
            ProyectoSeleccionado = ControllerBase.ProyectoSeleccionado;
            ProyectoVirtual = ControllerBase.ProyectoVirtual;
        }

        public void CargarDatosHead()
        {
            ViewBag.Version = ControllerBase.Version;
            ViewBag.JSYCSSunificado = JSYCSSunificado;
            ViewBag.GoogleAnalytics = CodigoCompletoGoogleAnalytics;
            ViewBag.GoogleAnalyticsCode = CodigoGooleAnalytics;
            ViewBag.ListaCSS = new List<string>();

            ViewBag.ListaJS = new List<string>();
            ViewBag.BusquedasXml = new List<string>();
            ViewBag.ListaMetas = new List<KeyValuePair<string, string>>();
            ViewBag.ListaMetasComplejas = new List<Dictionary<string, string>>();

            ObtenerListaInputsHidden();
        }


        /// <summary>
        /// Obtiene un token para que el usuario se pueda loguear
        /// </summary>
        public string TokenLoginUsuario
        {
            get
            {
                if (!mHttpContextAccessor.HttpContext.Session.Keys.Contains("tokenCookie"))
                {
                    string ticks = DateTime.Now.Ticks.ToString();
                    mHttpContextAccessor.HttpContext.Session.Set("tokenCookie", ticks);
                }
                return mHttpContextAccessor.HttpContext.Session.Get<string>("tokenCookie");
            }
        }

        protected virtual void ObtenerListaInputsHidden()
        {
            
            ViewBag.ListaInputHidden = new List<KeyValuePair<string, string>>();
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_proyID", ProyectoSeleccionado.Clave.ToString()));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_usuarioID", ControllerBase.UsuarioActual.UsuarioID.ToString()));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_bool_esMyGnoss", ProyectoSeleccionado.Clave.Equals(ProyectoAD.MetaProyecto).ToString()));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_bool_estaEnProyecto", (!ControllerBase.UsuarioActual.EsIdentidadInvitada).ToString()));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_bool_esUsuarioInvitado", ControllerBase.UsuarioActual.EsUsuarioInvitado.ToString()));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_perfilID", IdentidadActual.PerfilID.ToString()));
            

            string uridbpedia = GestorParametrosAplicacion.ParametroAplicacion.FirstOrDefault(item => item.Parametro.Equals(TiposParametrosAplicacion.URIGrafoDbpedia))?.Valor;
            if (!string.IsNullOrEmpty(uridbpedia))
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_proyIDDbpedia", uridbpedia));
            }
            Guid guidParse = Guid.Empty;
            if (ControllerBase.RequestParams("organizacion") != null && Guid.TryParse(ControllerBase.RequestParams("organizacion"), out guidParse) && IdentidadActual.IdentidadOrganizacion != null)
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_identidadID", IdentidadActual.IdentidadOrganizacion.Clave.ToString().ToUpper()));
            }
            else
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_identidadID", IdentidadActual.Clave.ToString().ToUpper()));
            }

            if (IdentidadActual.IdentidadOrganizacion != null)
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_perfilOrgID", IdentidadActual.IdentidadOrganizacion.PerfilID.ToString()));
            }
            else
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_perfilOrgID", Guid.Empty.ToString()));
            }

            if (!ControllerBase.UsuarioActual.EsIdentidadInvitada)
            {
                bool MostrarGruposIDEnHtml = false;
                //"Parametro='" + TiposParametrosAplicacion.MostrarGruposIDEnHtml + "'"
                List<ParametroAplicacion> filasParam = ControllerBase.ParametrosAplicacionDS.Where(objeto => objeto.Parametro.Equals(TiposParametrosAplicacion.MostrarGruposIDEnHtml)).ToList();
                if (filasParam.Count > 0 && filasParam.First().Valor.ToLower() == "true")
                {
                    MostrarGruposIDEnHtml = true;
                }
                else if (ControllerBase.ParametroProyecto != null && ControllerBase.ParametroProyecto.ContainsKey(TiposParametrosAplicacion.MostrarGruposIDEnHtml) && !string.IsNullOrEmpty(ControllerBase.ParametroProyecto[TiposParametrosAplicacion.MostrarGruposIDEnHtml]) && ControllerBase.ParametroProyecto[TiposParametrosAplicacion.MostrarGruposIDEnHtml].Equals("true"))
                {
                    MostrarGruposIDEnHtml = true;
                }

                if (MostrarGruposIDEnHtml)
                {
                    string grupos = "";
                    string nombres = "";

                    IdentidadCN identCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    Dictionary<Guid, string> gruposPerfil = identCN.ObtenerGruposIDParticipaPerfil(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);


                    foreach (Guid grupoID in gruposPerfil.Keys)
                    {
                        grupos += grupoID + ",";
                        nombres += gruposPerfil[grupoID] + ",";
                    }

                    identCN.Dispose();
                    ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_gruposPerfilID", grupos));
                    ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_nombreGruposPerfilID", nombres));
                }
            }

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            DataWrapperUsuario dataWrapperUsuario = proyCL.ObtenerPoliticaCookiesProyecto(ControllerBase.ProyectoSeleccionado.Clave);
            proyCL.Dispose();

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_Cookies", (dataWrapperUsuario.ListaClausulaRegistro.Count > 0).ToString()));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_Idioma", ControllerBase.UtilIdiomas.LanguageCode));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_baseUrlBusqueda", UrlsSemanticas.ObtenerURLComunidad(ControllerBase.UtilIdiomas, ControllerBase.BaseURLIdioma, ProyectoVirtual.NombreCorto)));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_baseURL", ControllerBase.BaseURL));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_baseURLContent", ControllerBase.BaseURLContent));
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_baseURLStatic", ControllerBase.BaseURLStatic));

            if (IdentidadActual.OrganizacionID.HasValue)
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_organizacionID", IdentidadActual.OrganizacionID.Value.ToString()));
            }
            else
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_organizacionID", ProyectoAD.MyGnoss.ToString()));
            }

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_AdministradorOrg", ControllerBase.EsAdministrador(IdentidadActual).ToString()));


            string[] delimiter = { "|" };

            string proyecto = "";
            proyecto = "&proyectoID=" + ProyectoVirtual.Clave;

            if (ProyectoVirtual.TipoAcceso.Equals(TipoAcceso.Privado) || ProyectoVirtual.TipoAcceso.Equals(TipoAcceso.Reservado))
            {
                proyecto += "&esProyectoPrivado=true";
            }

            //string UrlRedirectLogin = mControllerBase.ObtenerUrlHomeConectado();
            string UrlRedirectLogin = "";
            string UrlParametros = "";

            if (!string.IsNullOrEmpty(ControllerBase.RequestParams("redirect")))
            {
                if (ControllerBase.BaseURL.Contains("depuracion.net"))
                {
                    UrlRedirectLogin = ControllerBase.BaseURL;
                }
                else
                {
                    if (ProyectoVirtual.Clave.Equals(ProyectoAD.MetaProyecto))
                    {
                        UrlRedirectLogin = ControllerBase.BaseURLIdioma.TrimEnd('/');
                    }
                    else
                    {
                        UrlRedirectLogin = ProyectoVirtual.UrlPropia(IdiomaUsuario).TrimEnd('/');
                    }
                }

                string cadenaRedirect = "/redirect/";

                if (mHttpContextAccessor.HttpContext.Request.Path.ToString().Contains(cadenaRedirect))
                {
                    //Se está redireccionando así mismo, extraigo la url de redirect:
                    string url = mHttpContextAccessor.HttpContext.Request.Path.ToString();
                    url = url.Substring(url.IndexOf(cadenaRedirect) + cadenaRedirect.Length);

                    if ((!url.StartsWith("http://")) && (!url.StartsWith("https://")))
                    {
                        string dominioComunidad = ProyectoSeleccionado.UrlPropia(IdiomaUsuario);
                        if (!url.StartsWith("/"))
                        {
                            url = "/" + url;
                        }
                        url = dominioComunidad + url;
                    }

                    UrlRedirectLogin = url;
                }
                else if (ControllerBase.RequestParams("redirect").StartsWith("http://") || ControllerBase.RequestParams("redirect").StartsWith("https://"))
                {
                    UrlRedirectLogin = ControllerBase.RequestParams("redirect");
                }
                else
                {
                    UrlRedirectLogin = UrlRedirectLogin + "/" + ControllerBase.RequestParams("redirect");
                }
                try
                {
                    if (!string.IsNullOrEmpty(UrlRedirectLogin)) 
                    {
                        string hostRedirect = new Uri(UrlRedirectLogin).Host;
                        string hostProyecto = new Uri(ProyectoSeleccionado.UrlPropia(IdiomaUsuario)).Host;
                        string urlRedirectLimpio = UtilCadenas.LimpiarInyeccionCodigo(UrlRedirectLogin);
                        if (!hostRedirect.Equals(hostProyecto) || !urlRedirectLimpio.Equals(UrlRedirectLogin))
                        {
                            UrlRedirectLogin = "";
                        }
                    }
                   
                }
                catch(Exception ex)
                {
                    UrlRedirectLogin = "";
                }
                
               
                //Comprobamos si el redirect tiene parámetros
                if (!UrlRedirectLogin.Contains("?") && mHttpContextAccessor.HttpContext.Request.QueryString.HasValue)
                {
                    UrlParametros = ObtenerParametrosParaRedireccio();
                }
            }

            if (!string.IsNullOrEmpty(UrlRedirectLogin))
            {
                UrlRedirectLogin = $"&redirect={HttpUtility.UrlEncode(UrlRedirectLogin)}";
            }

            UrlRedirectLogin += UrlParametros;

            string idioma = $"&idioma={IdiomaUsuario}";

            string baseURL = $"&baseURL={BaseURLIdioma}";

            ViewBag.UrlActionLogin = $"{UrlServicioLogin}/login?token={HttpUtility.UrlEncode(TokenLoginUsuario)}{UrlRedirectLogin}{proyecto}{idioma}{baseURL}";
            
            if(string.IsNullOrEmpty(UrlRedirectLogin))
            {
                GnossUrlsSemanticas gnossUrlsSemanticas = new GnossUrlsSemanticas(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                string UrlRedirect = gnossUrlsSemanticas.ObtenerURLComunidad(new Recursos.UtilIdiomas(IdiomaUsuario, mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper), BaseURLIdioma, ProyectoVirtual.NombreCorto);
                ViewBag.UrlActionTwoFactorAuthentication = $"{UrlServicioLogin}/externallogin?loginToken={HttpUtility.UrlEncode(TokenLoginUsuario)}&redirect={UrlRedirect}&proyectoID={ProyectoVirtual.Clave}";
            }
            else
            {
                ViewBag.UrlActionTwoFactorAuthentication = $"{UrlServicioLogin}/externallogin?loginToken={HttpUtility.UrlEncode(TokenLoginUsuario)}{UrlRedirectLogin}&proyectoID={ProyectoVirtual.Clave}";
            }

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_UrlLogin", ViewBag.UrlActionLogin));

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_UrlLoginCookie", UrlServicioLogin));

            //EtiquetadoAutomático
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_usarMasterParaLectura", ControllerBase.UsuarioActual.UsarMasterParaLectura.ToString()));
            string inpt_UrlServicioFacetas = mConfigService.ObtenerUrlServicioFacetasExterno();
            string inpt_UrlServicioResultados = mConfigService.ObtenerUrlServicioResultadosExterno();

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_UrlServicioFacetas", inpt_UrlServicioFacetas));

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_UrlServicioResultados", inpt_UrlServicioResultados));


            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_UrlServicioContextos", mConfigService.ObtenerUrlServicio("contextosHome")));

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_Notificaciones", mConfigService.ObtenerVapidPublicKey()));

            if (!string.IsNullOrEmpty(UrlServicioContextos))
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_UrlServicioContextosCMSExternalService", UrlServicioContextos));
            }

            string inpt_UrlServicioAutocompletar = mConfigService.ObtenerUrlServicio("autocompletar");
            

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_urlServicioAutocompletar", inpt_UrlServicioAutocompletar));


            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_urlServicioAutocompletarEtiquetas", mConfigService.ObtenerUrlServicio("autocompletarEtiquetas")));

            string inpt_urlEtiquetadoAutomatico = "";
            //inpt_urlEtiquetadoAutomatico = $"{mConfigService.ObtenerUrlServicio("etiquetadoAutomatico")}/EtiquetadoAutomatico.asmx";
            inpt_urlEtiquetadoAutomatico = $"{mConfigService.ObtenerUrlServicio("etiquetadoAutomatico")}";

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_urlEtiquetadoAutomatico", inpt_urlEtiquetadoAutomatico));

            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_serverTime", DateTime.Now.ToString("yyyy/MM/dd HH:mm")));

            string inputtipo = "";
            if (ControllerBase.ProyectoOrigenBusquedaID == Guid.Empty)
            {
                inputtipo = "tipoBusquedaAutoCompl";
            }
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_tipo", inputtipo));
            ParametroAplicacion parametroAplicacion = GestorParametrosAplicacion.ParametroAplicacion.Where(parametro => parametro.Parametro.Equals(ParametroAD.PermitirEnlazarDocumentosOneDrive)).FirstOrDefault();
            if (parametroAplicacion != null)
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_oneDrivePermitido", parametroAplicacion.Valor));
            }
            else
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_oneDrivePermitido", "False"));
            }
            // TODO: Fernando, Buscador de google
            //HtmlInputHidden inputSearch = listaControlInputsHidden.Find(input => input.Name.EndsWith("inpt_ub_" + ddlCategorias.Items[0].Value));
            //if (inputSearch != null)
            //{
            //    UrlSearchProyecto = inputSearch.Value.Substring(inputSearch.Value.IndexOf("@") + 1);
            //}

            //ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_urlServicioAutocompletarEtiquetas", Conexion.ObtenerUrlServicioAutocompletarEtiquetas(ProyectoSeleccionado.Clave, ProyectoSeleccionado.UrlPropia)));

            if (ControllerBase.ParametroProyecto != null && ControllerBase.ParametroProyecto.ContainsKey("ConfigBBDDAutocompletarProyecto") && ControllerBase.ParametroProyecto["ConfigBBDDAutocompletarProyecto"] != "0")
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_AutocompletarProyectoVirtuoso", ControllerBase.ParametroProyecto["ConfigBBDDAutocompletarProyecto"]));
            }
            bool mantenerSesionActiva = true;
            List<ParametroAplicacion> filasParamMantenerSesion = ControllerBase.ParametrosAplicacionDS.Where(objeto => objeto.Parametro.Equals(TiposParametrosAplicacion.MantenerSesionActiva)).ToList();
            if (filasParamMantenerSesion.Count > 0 && (filasParamMantenerSesion[0]).Valor.ToLower() == "false")
            {
                mantenerSesionActiva = false;
            }
            ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_MantenerSesionActiva", mantenerSesionActiva.ToString()));

            if (!mantenerSesionActiva)
            {
                List<ParametroAplicacion> filasParamDuracionCookieUsuario = ControllerBase.ParametrosAplicacionDS.Where(objeto => objeto.Parametro.Equals(TiposParametrosAplicacion.DuracionCookieUsuario)).ToList();
                if (filasParamDuracionCookieUsuario.Count > 0 && !string.IsNullOrEmpty(filasParamDuracionCookieUsuario.First().Valor))
                {
                    ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_DuracionCookieUsuario", filasParamDuracionCookieUsuario.First().Valor));
                }
            }

            bool matomoConfigurado = !string.IsNullOrEmpty(mConfigService.ObtenerUrlMatomo());
            if(matomoConfigurado)
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_matomoConfigurado", "True"));
            }
            else
            {
                ViewBag.ListaInputHidden.Add(new KeyValuePair<string, string>("inpt_matomoConfigurado", "False"));
            }
        }

        /// <summary>
        /// Metodo para añadirele los parametros necesarios a la url de redireccion si los hay
        /// </summary>
        /// <param name="UrlRedirectLogin">Url redireccion</param>
        /// <returns></returns>
        private string ObtenerParametrosParaRedireccio()
        {
            string parametros = mHttpContextAccessor.HttpContext.Request.QueryString.ToString();

            if (!string.IsNullOrEmpty(parametros.Trim()))
            {
                //Comprobamos si en los parámetros adicionales se incluye el propio redirect
                if (parametros.Contains("?redirect") || parametros.Contains("&redirect"))
                {
                    int comienzoParametroRedirect = parametros.IndexOf("?redirect");

                    //Si contiene redirect y comienza por redirect
                    if (comienzoParametroRedirect != -1)
                    {
                        int finParametroRedirect = parametros.IndexOf("&");
                        //Comprobamos si hay algún parámetro más a parte del redirect
                        if (finParametroRedirect != -1)
                        {
                            //Si hay más parámetros, eliminamos el parámetro redirect y dejamos el resto
                            parametros = parametros.Replace(parametros.Substring(comienzoParametroRedirect, finParametroRedirect), "");
                        }
                        else
                        {
                            //Si no hay más parámetros que el redirect lo eliminamos
                            parametros = string.Empty;
                        }
                    }
                    else
                    {
                        //Si el redirect no es el primer parámetro obtenemos el siguiente en caso de haberlo
                        comienzoParametroRedirect = parametros.IndexOf("&redirect");

                        int siguieteParametro = parametros.Substring(comienzoParametroRedirect + 1).IndexOf("&");

                        if (siguieteParametro != -1)
                        {
                            //Si hay parámetro después del redirect eliminamos el redirect
                            parametros = parametros.Replace(parametros.Substring(comienzoParametroRedirect, siguieteParametro + 1), "");
                        }
                        else
                        {
                            //Si no hay parámetros despúes del redirect, nos quedamos con los parámetros anteriores
                            parametros = parametros.Substring(0, comienzoParametroRedirect);
                        }
                    }
                }
                //Cambiamos los ? por &
                parametros.Replace('?', '&');
            }

            return parametros;
        }
    }
}
