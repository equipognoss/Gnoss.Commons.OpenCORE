using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.Live;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ActividadReciente : ControllerBaseGnoss
    {
        private bool mEsPaginaSuscripciones = false;

        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private EntityContext mEntityContext;
        private IHttpContextAccessor mHttpContextAccessor;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private GnossCache mGnossCache;
        private EntityContextBASE mEntityContextBASE;

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pController">Controller</param>
        public ActividadReciente(bool pEsPaginaSuscripciones, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, EntityContextBASE entityContextBASE, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
               : this(loggingService, entityContext, configService, httpContextAccessor, redisCacheWrapper, virtuosoAD, gnossCache, entityContextBASE, viewEngine, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication)
        {
            mEsPaginaSuscripciones = pEsPaginaSuscripciones;
        }

        /// <summary>
        /// Constructor a partir de la página que contiene al controlador
        /// </summary>
        /// <param name="pController">Controller</param>
        public ActividadReciente(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, EntityContextBASE entityContextBASE, ICompositeViewEngine viewEngine, IUtilServicioIntegracionContinua utilServicioIntegracionContinua, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(httpContextAccessor, entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD, gnossCache, viewEngine, utilServicioIntegracionContinua, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mHttpContextAccessor = httpContextAccessor;
            mRedisCacheWrapper = redisCacheWrapper;
            mGnossCache = gnossCache;
            mEntityContextBASE = entityContextBASE;
        }

        public RecentActivity ObtenerActividadReciente(int pNumPagina, int pNumElementos, TipoActividadReciente pTipo, Guid? pPerfilIDPagina, bool pEsOrganizacion)
        {
            return ObtenerActividadReciente(pNumPagina, pNumElementos, pTipo, pPerfilIDPagina, pEsOrganizacion, Guid.Empty);
        }

        public RecentActivity ObtenerActividadReciente(int pNumPagina, int pNumElementos, TipoActividadReciente pTipo, Guid? pPerfilIDPagina, bool pEsOrganizacion, Guid pComponenteID)
        {
            List<string> ActividadRecienteItems = new List<string>();

            LiveUsuariosCL liveUsuariosCL = new LiveUsuariosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
            List<object> listaElementosLive = new List<object>();

            Guid proyectoID = ProyectoVirtual.Clave;

            Guid? usuarioID = null;
            if (UsuarioActual.UsuarioID != UsuarioAD.Invitado)
            {
                bool tienePrivados = false;
                if (!pPerfilIDPagina.HasValue)
                {
                    FacetadoCL facetadoCL = new FacetadoCL(ParametrosAplicacionDS.Find(parametro => parametro.Parametro.Equals("UrlIntragnoss")).Valor, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    tienePrivados = facetadoCL.TienePrivados(proyectoID, IdentidadActual.PerfilID);
                    facetadoCL.Dispose();
                }

                if (tienePrivados)
                {
                    usuarioID = UsuarioActual.UsuarioID;
                }
            }

            int inicio = ((pNumPagina - 1) * pNumElementos) + 1;
            int fin = (pNumPagina) * pNumElementos;

            if (pTipo == TipoActividadReciente.HomeUsuario && UsuarioActual.UsuarioID != UsuarioAD.Invitado)
            {
                //Obtengo los proyectos para los que este usuario no quiere que se le muestren actualizaciones 
                List<AD.EntityModel.Models.IdentidadDS.Identidad> filasIdentidadSinActualizacion = IdentidadActual.GestorIdentidades.DataWrapperIdentidad.ListaIdentidad.Where(identidad => identidad.ActualizaHome == false && identidad.PerfilID.Equals(IdentidadActual.PerfilID)).ToList();

                List<Guid> listaProyectosSinActualizacion = new List<Guid>();
                foreach (AD.EntityModel.Models.IdentidadDS.Identidad filaIdentidad in filasIdentidadSinActualizacion)
                {
                    listaProyectosSinActualizacion.Add(filaIdentidad.ProyectoID);
                }

                //Obtengo la caché para la home de este usuario
                listaElementosLive = liveUsuariosCL.ObtenerLivePerfilUsuario(UsuarioActual.UsuarioID, IdentidadActual.PerfilID, UtilIdiomas.LanguageCode, listaProyectosSinActualizacion, inicio, fin);
                if (listaElementosLive.Count == 0 && EsEcosistemaSinMetaProyecto && ProyectoPrincipalUnico != ProyectoAD.MetaProyecto)
                {
                    listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuarioInvitado(ProyectoPrincipalUnico, UtilIdiomas.LanguageCode, inicio, fin);

                    liveUsuariosCL.ClonarLiveProyectoAHomeUsu(UsuarioActual.UsuarioID, IdentidadActual.PerfilID, ProyectoPrincipalUnico);
                }
            }
            else if (pTipo == TipoActividadReciente.HomeProyecto && proyectoID != ProyectoAD.MetaProyecto)
            {
                if (UsuarioActual.UsuarioID == UsuarioAD.Invitado || UsuarioActual.EsIdentidadInvitada)
                {
                    //Obtengo la caché para la home de este proyecto
                    listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuarioInvitado(proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                }
                else
                {
                    //Obtengo la caché para la home de este proyecto
                    listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuario(usuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);

                    if (!listaElementosLive.Any() && Comunidad.NumberOfResources > 0)
                    {
                        if (usuarioID.HasValue)
                        {
                            liveUsuariosCL.ClonarLiveProyectoAUsu(usuarioID.Value, proyectoID);
                            listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuario(usuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                        }
                        if (!listaElementosLive.Any())
                        {
                            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                            List<Guid> listaDocumentos = docCN.ObtenerDocumentosIDActividadRecienteEnProyecto(proyectoID);
                            if (listaDocumentos.Count > 100)
                            {
                                listaDocumentos = listaDocumentos.GetRange(0, 100);
                            }

                            int score = 1;
                            for (int i = listaDocumentos.Count - 1; i >= 0; i--)
                            {
                                string clave = "0_" + listaDocumentos[i].ToString() + "_" + proyectoID;
                                score = liveUsuariosCL.AgregarLiveProyectoUsuario(null, proyectoID, clave, score) + 1;
                            }
                            listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuario(null, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                        }

                        if (!listaElementosLive.Any() && usuarioID.HasValue)
                        {
                            ClonarActividadRecienteUsuarioGrupo(usuarioID.Value, proyectoID);
                            listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuario(usuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                        }
                    }
                }
            }
            else if (pTipo == TipoActividadReciente.PerfilProyecto && pPerfilIDPagina.HasValue)
            {
                if (pEsOrganizacion)
                {
                    //Obtengo la caché para el perfil de este usuario
                    listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoPerfilOrg(null, pPerfilIDPagina.Value, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                }
                else
                {
                    //Obtengo la caché para el perfil de este usuario
                    listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoPerfilUsuario(null, pPerfilIDPagina.Value, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                }
            }
            else if (pTipo == TipoActividadReciente.SuscripcionProyecto && UsuarioActual.UsuarioID != UsuarioAD.Invitado)
            {
                //Obtengo la caché para las suscripciones de este usuario en este proyecto
                listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuarioSuscripciones(UsuarioActual.UsuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);

                if (!listaElementosLive.Any())
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    List<Guid> listaDocumentos = docCN.ObtenerDocumentosIDSuscripcionPerfilEnProyecto(IdentidadActual.PerfilID, proyectoID, 100);
                    docCN.Dispose();

                    int score = 1;
                    for (int i = listaDocumentos.Count - 1; i >= 0; i--)
                    {
                        string clave = "0_" + listaDocumentos[i].ToString() + "_" + proyectoID;
                        score = liveUsuariosCL.AgregarLiveProyectoUsuarioSuscripciones(UsuarioActual.UsuarioID, proyectoID, clave, score) + 1;
                    }
                }
            }
            else if (pTipo == TipoActividadReciente.Suscripcion && UsuarioActual.UsuarioID != UsuarioAD.Invitado)
            {
                //Obtengo la caché para las suscripciones de este usuario
                listaElementosLive = liveUsuariosCL.ObtenerLiveUsuarioSuscripciones(UsuarioActual.UsuarioID, UtilIdiomas.LanguageCode, inicio, fin);
            }
            else if (pTipo == TipoActividadReciente.SuscripcionSiNoHomeProyecto)
            {
                if (!UsuarioActual.EsIdentidadInvitada)
                {
                    //Obtengo la caché para las suscripciones de este usuario en este proyecto
                    listaElementosLive = liveUsuariosCL.ObtenerLiveProyectoUsuarioSuscripciones(UsuarioActual.UsuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                }

                if (inicio == 1 && listaElementosLive.Count < fin)
                {
                    List<object> listaElementosLive2 = new List<object>();
                    if (UsuarioActual.UsuarioID == UsuarioAD.Invitado || UsuarioActual.EsIdentidadInvitada)
                    {
                        //Obtengo la caché para la home de este proyecto
                        listaElementosLive2 = liveUsuariosCL.ObtenerLiveProyectoUsuarioInvitado(proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                    }
                    else
                    {
                        //Obtengo la caché para la home de este proyecto
                        listaElementosLive2 = liveUsuariosCL.ObtenerLiveProyectoUsuario(usuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                        if (usuarioID.HasValue && listaElementosLive.Count == 0)
                        {
                            ClonarActividadRecienteUsuarioGrupo(usuarioID.Value, proyectoID);
                            listaElementosLive2 = liveUsuariosCL.ObtenerLiveProyectoUsuario(usuarioID, proyectoID, UtilIdiomas.LanguageCode, inicio, fin);
                        }
                    }
                    foreach (object elementoLive in listaElementosLive2)
                    {
                        if (!listaElementosLive.Contains(elementoLive))
                        {
                            listaElementosLive.Add(elementoLive);
                        }
                    }
                    pTipo = TipoActividadReciente.HomeProyecto;
                }
            }

            List<RecentActivityItem> listaItems = ObtenerItemsActividadReciente(listaElementosLive, mEsPaginaSuscripciones);

            RecentActivity actividadReciente = new RecentActivity();
            actividadReciente.UrlLoadMoreActivity = ObtenerUrlCargarMasActividad();
            actividadReciente.TypeActivity = (int)pTipo;
            actividadReciente.RecentActivityItems = listaItems;
            actividadReciente.NumPage = pNumPagina;
            actividadReciente.NumItemsPage = pNumElementos;
            actividadReciente.ComponentKey = pComponenteID;
            actividadReciente.ProfileKey = pPerfilIDPagina;

            return actividadReciente;
        }

        private void ClonarActividadRecienteUsuarioGrupo(Guid pUsuarioID, Guid pProyectoID)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

            Dictionary<Guid, string> grupos = identidadCN.ObtenerGruposIDParticipaPerfil(IdentidadActual.Clave, IdentidadActual.IdentidadMyGNOSS.Clave);
            if (grupos.Count > 0)
            {
                LiveUsuariosCL liveUsuariosCL = new LiveUsuariosCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);

                Guid grupoID = grupos.Keys.First();
                int score = liveUsuariosCL.ClonarLiveGrupoProyectoAUsu(pUsuarioID, pProyectoID, grupoID);

                if (score == 0)
                {
                    score = liveUsuariosCL.ClonarLiveProyectoAGrupo(pProyectoID, grupoID);
                    if (score != 0)
                    {
                        liveUsuariosCL.ClonarLiveGrupoProyectoAUsu(pUsuarioID, pProyectoID, grupoID);
                    }
                }

                liveUsuariosCL.Dispose();
            }
            identidadCN.Dispose();
        }

        public bool TienePrivadosComunesConPerfilPagina(Guid? pProyectoID, Guid pPerfilPaginaID, Guid pPerfilActualID, int pTipoDocumento)
        {
            bool tieneRecursosPrivadosComunesAlPerfilPagina = false;
            bool tieneGruposConRecursosPrivadosComunesAlPerfilPagina = false;

            //Comprobar si se ese grupo edita o es lector de algún documento del perfil de la página.
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            tieneGruposConRecursosPrivadosComunesAlPerfilPagina = identidadCN.TienePerfilGruposConRecursosPrivadosEnComunConElPerfilPagina(pProyectoID, pPerfilActualID, pPerfilPaginaID);
            identidadCN.Dispose();

            //Comprobar que el usuario no es lector de algún documento del perfil de la página.
            DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            tieneRecursosPrivadosComunesAlPerfilPagina = documentacionCN.TienePerfilRecursosPrivadosEnComunConElPerfilPagina(pProyectoID, pPerfilActualID, pPerfilPaginaID);
            documentacionCN.Dispose();

            return tieneRecursosPrivadosComunesAlPerfilPagina || tieneGruposConRecursosPrivadosComunesAlPerfilPagina;
        }

        private string ObtenerUrlCargarMasActividad()
        {
            string url = BaseURLIdioma;

            if (ProyectoSeleccionado.Clave != ProyectoAD.MetaProyecto)
            {
                url = UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, ProyectoSeleccionado.NombreCorto);
            }

            return url + "/load-more-activity";
        }

        private List<RecentActivityItem> ObtenerItemsActividadReciente(List<object> pListaResultados, bool pEsPaginaSuscripciones)
        {
            List<RecentActivityItem> items = new List<RecentActivityItem>();

            if (pListaResultados != null)
            {
                Dictionary<Guid, List<Guid>> listaClavesRecursosProyecto = new Dictionary<Guid, List<Guid>>();
                Dictionary<Guid, List<Guid>> listaClavesPerfilesProyecto = new Dictionary<Guid, List<Guid>>();
                Dictionary<Guid, Proyecto> listaProyectos = new Dictionary<Guid, Proyecto>();
                Dictionary<Guid, ParametroGeneral> listaParametrosGenerales = new Dictionary<Guid, ParametroGeneral>();

                int cont = 0;
                foreach (string id in pListaResultados)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        string[] parametros = id.Split('_');

                        RecentActivityItem item = new RecentActivityItem();
                        Guid proyID = Guid.Empty;

                        if (parametros[0] == "0" || parametros[0] == "1" || parametros[0] == "2")
                        {
                            Guid docID = new Guid(parametros[1]);
                            proyID = new Guid(parametros[2]);

                            if (!listaClavesRecursosProyecto.ContainsKey(proyID))
                            {
                                listaClavesRecursosProyecto.Add(proyID, new List<Guid>());
                            }
                            if (!listaClavesRecursosProyecto[proyID].Contains(docID))
                            {
                                listaClavesRecursosProyecto[proyID].Add(docID);
                            }
                            item = new RecentActivityResourceItem();
                        }
                        else if (parametros[0] == "4")
                        {
                            Guid perfilID = new Guid(parametros[1]);
                            proyID = new Guid(parametros[2]);

                            if (!listaClavesPerfilesProyecto.ContainsKey(proyID))
                            {
                                listaClavesPerfilesProyecto.Add(proyID, new List<Guid>());
                            }

                            listaClavesPerfilesProyecto[proyID].Add(perfilID);

                            item = new RecentActivityMemberItem();
                        }
                        else if (parametros[0] == "12")
                        {
                            proyID = new Guid(parametros[1]);
                            int numNuevosMiembros = 0;

                            if (parametros.Length > 2)
                            {
                                numNuevosMiembros = int.Parse(parametros[2]);
                            }
                        }

                        Proyecto proy = ProyectoVirtual;
                        ParametroGeneral parametrosGenerales = ParametrosGeneralesVirtualRow;
                        if (!ProyectoVirtual.Clave.Equals(proyID))
                        {
                            if (listaProyectos.ContainsKey(proyID))
                            {
                                proy = listaProyectos[proyID];
                            }
                            else
                            {
                                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                                GestionProyecto gestProy = new GestionProyecto(proyCL.ObtenerProyectoPorID(proyID), mLoggingService, mEntityContext);
                                if (gestProy.ListaProyectos.ContainsKey(proyID))
                                {
                                    proy = gestProy.ListaProyectos[proyID];
                                    listaProyectos.Add(proyID, proy);
                                }

                                ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                                parametrosGenerales = paramCL.ObtenerParametrosGeneralesDeProyecto(proyID).ListaParametroGeneral.FirstOrDefault();
                                if (parametrosGenerales != null)
                                {
                                    listaParametrosGenerales.Add(proyID, parametrosGenerales);
                                }
                            }
                        }

                        if (!proy.Clave.Equals(ProyectoAD.MetaProyecto))
                        {
                            item.UrlCommunity = UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proy.NombreCorto);
                            item.NameCommunity = UtilCadenas.ObtenerTextoDeIdioma(proy.Nombre, UtilIdiomas.LanguageCode, parametrosGenerales.IdiomaDefecto);
                        }

                        item.Key = id.Replace("_leido", "");
                        item.Readed = id.Contains("_leido");

                        items.Add(item);

                        cont++;
                    }
                }
                if (listaClavesRecursosProyecto.Count > 0)
                {
                    foreach (Guid proyectoID in listaClavesRecursosProyecto.Keys)
                    {
                        Proyecto proy = ProyectoVirtual;
                        ParametroGeneral parametrosGenerales = ParametrosGeneralesVirtualRow;
                        if (!ProyectoVirtual.Clave.Equals(proyectoID))
                        {
                            proy = listaProyectos[proyectoID];
                            parametrosGenerales = listaParametrosGenerales[proyectoID];
                        }

                        Identidad identidadEnProy = IdentidadActual;
                        Guid identidadID = IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID);
                        if (identidadID != Guid.Empty && IdentidadActual.GestorIdentidades.ListaIdentidades.ContainsKey(identidadID))
                        {
                            identidadEnProy = IdentidadActual.GestorIdentidades.ListaIdentidades[IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID)];
                        }
                        ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLContent, BaseURLStatic, proy, parametrosGenerales, identidadEnProy, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

                        string urlBusqueda = "";

                        if (proyectoID.Equals(ProyectoAD.MetaProyecto))
                        {
                            urlBusqueda = BaseURLIdioma + UrlPerfil;
                        }
                        else
                        {
                            urlBusqueda = UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proy.NombreCorto) + "/";
                        }
                        urlBusqueda += UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

                        Dictionary<Guid, ResourceModel> listaRecursosModel = controladorMVC.ObtenerRecursosPorID(listaClavesRecursosProyecto[proyectoID], urlBusqueda, null, false);

                        Dictionary<Guid, List<ResourceEventModel>> listaEventosModel = new Dictionary<Guid, List<ResourceEventModel>>();

                        if (!pEsPaginaSuscripciones)
                        {
                            listaEventosModel = controladorMVC.ObtenerEventosDeRecursosPorID(listaClavesRecursosProyecto[proyectoID]);
                        }

                        foreach (Guid docID in listaRecursosModel.Keys)
                        {
                            ResourceModel.DocumentType tipoRecurso = listaRecursosModel[docID].TypeDocument;

                            int tipo = 0;
                            if (tipoRecurso == ResourceModel.DocumentType.Pregunta)
                            {
                                tipo = 1;
                            }
                            else if (tipoRecurso == ResourceModel.DocumentType.Debate)
                            {
                                tipo = 2;
                            }

                            string claveItem = tipo.ToString() + "_" + docID.ToString() + "_" + proyectoID.ToString();

                            RecentActivityResourceItem itemRecurso = (RecentActivityResourceItem)items.Find(item => item.Key.StartsWith(claveItem));

                            if (itemRecurso != null)
                            {
                                itemRecurso.Resource = listaRecursosModel[docID];

                                if (listaEventosModel.ContainsKey(docID))
                                {
                                    itemRecurso.Events = listaEventosModel[docID];
                                }
                            }
                        }
                    }
                }
                if (listaClavesPerfilesProyecto.Count > 0)
                {
                    foreach (Guid proyectoID in listaClavesPerfilesProyecto.Keys)
                    {
                        Proyecto proy = ProyectoVirtual;
                        ParametroGeneral parametrosGenerales = ParametrosGeneralesVirtualRow;
                        if (!ProyectoVirtual.Clave.Equals(proyectoID))
                        {
                            proy = listaProyectos[proyectoID];
                            parametrosGenerales = listaParametrosGenerales[proyectoID];
                        }

                        Identidad identidadEnProy = IdentidadActual;
                        Guid identidadID = IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID);
                        if (identidadID != Guid.Empty && IdentidadActual.GestorIdentidades.ListaIdentidades.ContainsKey(identidadID))
                        {
                            identidadEnProy = IdentidadActual.GestorIdentidades.ListaIdentidades[IdentidadActual.ObtenerIdentidadEnProyectoDeIdentidad(proyectoID)];
                        }

                        ControladorProyectoMVC controladorMVC = new ControladorProyectoMVC(UtilIdiomas, BaseURL, BaseURLContent, BaseURLStatic, proy, parametrosGenerales, identidadEnProy, EsBot, mLoggingService, mEntityContext, mConfigService, mHttpContextAccessor, mRedisCacheWrapper, mVirtuosoAD, mGnossCache, mEntityContextBASE, mServicesUtilVirtuosoAndReplication);

                        string urlBusqueda = UrlsSemanticas.ObtenerURLComunidad(UtilIdiomas, BaseURLIdioma, proy.NombreCorto) + "/" + UtilIdiomas.GetText("URLSEM", "BUSQUEDAAVANZADA");

                        IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                        Dictionary<Guid, Guid> listaIdentidadesPerfil = identidadCN.ObtenerIdentidadesIDyPerfilEnProyecto(proyectoID, listaClavesPerfilesProyecto[proyectoID]);

                        List<Guid> listaIdentidades = new List<Guid>();
                        foreach (Guid identID in listaIdentidadesPerfil.Values)
                        {
                            listaIdentidades.Add(identID);
                        }

                        Dictionary<Guid, ProfileModel> listaPerfilesModel = controladorMVC.ObtenerIdentidadesPorID(listaIdentidades);

                        foreach (Guid perfilID in listaIdentidadesPerfil.Keys)
                        {
                            string claveItem = "4_" + perfilID.ToString() + "_" + proyectoID.ToString();

                            RecentActivityMemberItem itemPerfil = (RecentActivityMemberItem)items.Find(item => item.Key.StartsWith(claveItem));

                            if (itemPerfil != null)
                            {
                                itemPerfil.Profile = listaPerfilesModel[listaIdentidadesPerfil[perfilID]];
                            }
                        }
                    }
                }
            }
            return items;
        }

    }
}
