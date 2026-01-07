using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.MVC;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.PersonaDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Roles;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Suscripcion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.Identidad;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Suscripcion;
using Es.Riam.Gnoss.CL.Tesauro;
using Es.Riam.Gnoss.Elementos.Amigos;
using Es.Riam.Gnoss.Elementos.Documentacion;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.MVC;
using Es.Riam.Gnoss.Logica.Notificacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Suscripcion;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Recursos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.UtilServiciosWeb;
using Es.Riam.Gnoss.Web.Controles;
using Es.Riam.Gnoss.Web.Controles.Proyectos;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Es.Riam.Gnoss.Web.MVC.Controles.Controladores
{
    public class ControladorProyectoMVC : ControladorBase
    {
        #region Variables Miembro

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;

        private UtilIdiomas mUtilIdiomas;
        private string mBaseURL;
        private string mBaseURLIdioma;
        private List<string> mBaseURLsContent;
        private int? mIndiceActualBaseUrlContent;
        private string mBaseURLStatic;
        private Elementos.ServiciosGenerales.Proyecto mProyecto;
        private Guid mProyectoOrigenID;
        private ParametroGeneral mParametrosGeneralesRow;
        private Dictionary<short, Dictionary<Guid, string>> mTipoDocumentoImagenPorDefecto;
        private Identidad mIdentidadActual;
        private bool mEsBot;
        private DocumentacionCN mDocumentacionCN;

        private IdentidadCL mIdentidadCL;
        private IdentidadCN mIdentidadCN;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private MVCCN mMVCCN;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="pUtilIdiomas">UtilIdiomas para obtener los Modelos</param>
        /// <param name="pBaseURL">BaseURL</param>
        /// <param name="pBaseURLContent">BaseURLContent</param>
        /// <param name="pBaseURLStatic">BaseURLStatic</param>
        /// <param name="pProyecto">Proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad Actual</param>
        /// <param name="pEsBot"></param>
        public ControladorProyectoMVC(UtilIdiomas pUtilIdiomas, string pBaseURL, string pBaseURLContent, string pBaseURLStatic, Elementos.ServiciosGenerales.Proyecto pProyecto, ParametroGeneral pParametrosGenerales, Identidad pIdentidadActual, bool pEsBot, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorProyectoMVC> logger, ILoggerFactory loggerFactory)
            : this(pUtilIdiomas, pBaseURL, new List<string>(), pBaseURLStatic, pProyecto, Guid.Empty, pParametrosGenerales, pIdentidadActual, pEsBot, loggingService, entityContext, configService, httpContextAccessor, redisCacheWrapper, virtuosoAD, gnossCache, entityContextBASE, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mBaseURLsContent.Add(pBaseURLContent);
        }

        /// <summary>
        /// Constructor del controlador
        /// </summary>
        /// <param name="pUtilIdiomas">UtilIdiomas para obtener los Modelos</param>
        /// <param name="pBaseURL">BaseURL</param>
        /// <param name="pBaseURLsContent">BaseURLsContent</param>
        /// <param name="pBaseURLStatic">BaseURLStatic</param>
        /// <param name="pProyecto">Proyecto actual</param>
        /// <param name="pProyectoOrigenID">ID del proyecto origen de los datos</param>
        /// <param name="pParametrosGeneralesRow">Parametros generales del proyecto actual</param>
        /// <param name="pIdentidadActual">Identidad Actual</param>
        /// <param name="pEsBot"></param>
        public ControladorProyectoMVC(UtilIdiomas pUtilIdiomas, string pBaseURL, List<string> pBaseURLsContent, string pBaseURLStatic, Elementos.ServiciosGenerales.Proyecto pProyecto, Guid pProyectoOrigenID, ParametroGeneral pParametrosGeneralesRow, Identidad pIdentidadActual, bool pEsBot, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IHttpContextAccessor httpContextAccessor, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, GnossCache gnossCache, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<ControladorProyectoMVC> logger, ILoggerFactory loggerFactory)
            : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mUtilIdiomas = pUtilIdiomas;
            mBaseURL = pBaseURL;
            mBaseURLIdioma = pBaseURL;
            if ((!string.IsNullOrEmpty(pParametrosGeneralesRow.IdiomaDefecto) && mUtilIdiomas.LanguageCode != pParametrosGeneralesRow.IdiomaDefecto) || (string.IsNullOrEmpty(pParametrosGeneralesRow.IdiomaDefecto) && !mUtilIdiomas.LanguageCode.Equals("es")))
            {
                mBaseURLIdioma = pBaseURL + "/" + mUtilIdiomas.LanguageCode;
            }

            mBaseURLsContent = pBaseURLsContent;
            mBaseURLStatic = pBaseURLStatic;
            mProyecto = pProyecto;
            mProyectoOrigenID = pProyectoOrigenID;
            mParametrosGeneralesRow = pParametrosGeneralesRow;
            mIdentidadActual = pIdentidadActual;
            mEsBot = pEsBot;
            mDocumentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
            mIdentidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            mIdentidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
            mMVCCN = new MVCCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCCN>(), mLoggerFactory);
        }

        #endregion

        #region Métodos públicos para obtener Modelos

        /// <summary>
        /// Obtiene los modelos de las comunidades
        /// </summary>
        /// <param name="pListaComunidadesID">Identificadores de las comunidadesrecursos</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <returns></returns>
        public Dictionary<Guid, CommunityModel> ObtenerComunidadesPorID(List<Guid> pListaComunidadesID, string pUrlBaseUrlBusqueda)
        {
            Dictionary<Guid, CommunityModel> listaComunidades = new Dictionary<Guid, CommunityModel>();

            ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
            //Dictionary<Guid, int?> contadoresRecursosComunidades = proyCL.ObtenerContadorRecursosComunidades(pListaComunidadesID); 
            Dictionary<Guid, int?> contadoresRecursosComunidades = new Dictionary<Guid, int?>();
            //Dictionary<Guid, int?> contadoresPersonasComunidades = proyCL.ObtenerContadorPersonasYOrganizacionesComunidades(pListaComunidadesID);
            Dictionary<Guid, int?> contadoresPersonasComunidades = new Dictionary<Guid, int?>();
            proyCL.Dispose();
            //Obtiene de BBDD las comunidades
            List<ObtenerComunidades> listaComunidadesObtenidas = mMVCCN.ObtenerComunidadesPorID(pListaComunidadesID);
            Dictionary<Guid, CommunityModel> listaComunidadesBBDD = new Dictionary<Guid, CommunityModel>();
            if (listaComunidadesObtenidas != null)
            {
                foreach (ObtenerComunidades comunidadObtenida in listaComunidadesObtenidas)
                {
                    Guid clave = comunidadObtenida.ProyectoID;
                    string Nombre = comunidadObtenida.Nombre;
                    string NombreCorto = comunidadObtenida.NombreCorto;
                    string NombrePresentacion = "";
                    if (!string.IsNullOrEmpty(comunidadObtenida.NombrePresentacion))
                    {
                        NombrePresentacion = comunidadObtenida.NombrePresentacion;
                    }
                    else
                    {
                        NombrePresentacion = Nombre;
                    }
                    string urlIntragnoss = ParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList().First().Valor;

                    string Descripcion = "";
                    if (comunidadObtenida.Descripcion != null)
                    {
                        Descripcion = comunidadObtenida.Descripcion;
                    }
                    //int NumeroOrg = comunidadObtenida.NumeroOrgRegistradas.Value;
                    List<string> recursosTipoOrg = new List<string>();
                    recursosTipoOrg.Add("Organizacion");
                    int NumeroOrg = ObtenerContadorComunidad(urlIntragnoss, clave.ToString(), TipoProyecto.Comunidad, recursosTipoOrg);

                    List<string> recursosTipoPersona = new List<string>();
                    recursosTipoPersona.Add("Persona");
                    int NumeroPers = ObtenerContadorComunidad(urlIntragnoss, clave.ToString(), TipoProyecto.Comunidad, recursosTipoPersona);

                    //if (contadoresPersonasComunidades.ContainsKey(clave) && contadoresPersonasComunidades[clave] != null)
                    //{
                    //    NumeroPers = (int)contadoresPersonasComunidades[clave];
                    //}
                    //else
                    //{
                    //    NumeroPers = comunidadObtenida.NumeroMiembros.Value;
                    //}
                    List<string> recursosTipoRecurso = new List<string>();
                    recursosTipoRecurso.Add("Recurso");
                    int NumeroRecursos = ObtenerContadorComunidad(urlIntragnoss, clave.ToString(), TipoProyecto.Comunidad, recursosTipoRecurso);
                    //if (contadoresRecursosComunidades.ContainsKey(clave) && contadoresRecursosComunidades[clave] != null)
                    //{
                    //    NumeroRecursos = (int)contadoresRecursosComunidades[clave];
                    //}
                    //else
                    //{
                    //    NumeroRecursos = comunidadObtenida.NumeroRecursos.Value;
                    //}

                    short TipoAcceso = comunidadObtenida.TipoAcceso;
                    string Tags = "";
                    if (comunidadObtenida.Tags != null)
                    {
                        Tags = comunidadObtenida.Tags;
                    }

                    string NombreImagen = "";
                    if (comunidadObtenida.NombreImagenPeque != null)
                    {
                        NombreImagen = comunidadObtenida.NombreImagenPeque;
                    }

                    CommunityModel comunidad = new CommunityModel();
                    comunidad.Key = clave;
                    comunidad.Name = Nombre;
                    comunidad.ShortName = NombreCorto;
                    comunidad.PresentationName = NombrePresentacion;
                    comunidad.Description = UtilCadenas.ObtenerTextoDeIdioma(Descripcion, mUtilIdiomas.LanguageCode, null);
                    comunidad.OpenDate = (DateTime)comunidadObtenida.FechaInicio;

                    comunidad.NumberOfOrganizations = NumeroOrg;
                    comunidad.NumberOfPerson = NumeroPers;
                    comunidad.NumberOfResources = NumeroRecursos;

                    comunidad.AccessType = (CommunityModel.TypeAccessProject)TipoAcceso;

                    comunidad.Tags = new List<string>(Tags.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    string urlFoto = $"{BaseURLContent}/{UtilArchivos.ContentImagenes}/{UtilArchivos.ContentImagenesProyectos}/{NombreImagen}";
                    comunidad.Logo = urlFoto;
                    if (string.IsNullOrEmpty(NombreImagen) || NombreImagen.Equals("peque"))
                    {
                        urlFoto = $"{BaseURLStatic}/img/{UtilArchivos.ContentImgIconos}/{UtilArchivos.ContentImagenesProyectos}/anonimo_peque.png";
                        comunidad.Logo = CargarImagenSup(clave);
                        if (string.IsNullOrEmpty(comunidad.Logo))
                        {
                            comunidad.Logo = urlFoto;
                        }
                    }
                    /*string urlLogo = BaseURLContent + "/" + UtilArchivos.ContentImagenes + "/Proyectos/" + NombreImagen;

                    if (string.IsNullOrEmpty(NombreImagen) || NombreImagen == "peque")
                    {
                        urlLogo = BaseURLStatic + "/img/Iconos/Proyectos/anonimo_peque.png";
                    }

                    comunidad.Logo = urlLogo;*/

                    listaComunidades.Add(clave, comunidad);
                }
            }

            foreach (CommunityModel comunidad in listaComunidades.Values)
            {
                comunidad.UrlSearch = pUrlBaseUrlBusqueda;
                comunidad.Url = UrlsSemanticas.ObtenerURLComunidad(mUtilIdiomas, BaseURLIdioma, comunidad.ShortName);
            }

            List<ObtenerTesauroProyectoMVC> listaTesauroProyectoMVC = mMVCCN.ObtenerCategoriasComunidadesPorID(pListaComunidadesID);

            if (listaTesauroProyectoMVC != null)
            {
                foreach (ObtenerTesauroProyectoMVC tesauroProyectoMVC in listaTesauroProyectoMVC)
                {
                    Guid claveProyecto = tesauroProyectoMVC.ProyectoID;
                    Guid claveCategoria = tesauroProyectoMVC.CategoriaTesauroID;
                    string NombreCategoria = tesauroProyectoMVC.Nombre;

                    CommunityModel comunidad = listaComunidades[claveProyecto];
                    if (comunidad.Categories == null)
                    {
                        comunidad.Categories = new List<CategoryModel>();
                    }
                    CategoryModel categoria = new CategoryModel();
                    categoria.Key = claveCategoria;
                    categoria.Name = NombreCategoria;
                    categoria.LanguageName = NombreCategoria;
                    categoria.Lang = mUtilIdiomas.LanguageCode;
                    comunidad.Categories.Add(categoria);
                }
            }

            return listaComunidades;
        }

        private int ObtenerContadorComunidad(string pUrlIntragnoss, string pProyecto, TipoProyecto pTipoProyecto, List<string> pRecursosTipo)
        {
            int resultado = 0;
            Dictionary<string, List<string>> listaFiltrosPers = new Dictionary<string, List<string>>();
            listaFiltrosPers.Add("rdf:type", pRecursosTipo);
            FacetadoDS facDS = new FacetadoDS();
            FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, pProyecto, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);

            facCN.ObtieneNumeroResultados(facDS, "RecursosBusqueda", listaFiltrosPers, new List<string>(), false, true, false, UsuarioAD.Invitado.ToString(), new List<string>(), "", pTipoProyecto, true, true, TiposAlgoritmoTransformacion.Ninguno, null);

            if ((facDS.Tables.Contains("NResultadosBusqueda")) && (facDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
            {
                object numeroResultados = facDS.Tables["NResultadosBusqueda"].Rows[0][0];
                if (numeroResultados is long)
                {
                    resultado = (int)(long)numeroResultados;
                }
                else if (numeroResultados is int)
                {
                    resultado = (int)numeroResultados;
                }
                else if (numeroResultados is string)
                {
                    int numeroResultadosInt;
                    int.TryParse((string)numeroResultados, out numeroResultadosInt);
                    resultado = numeroResultadosInt;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el modelo del recurs del proyecto en función de la identidad
        /// </summary>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <param name="pPestanyaID">ID de la pestanya (puesde ser nulo)</param>
        /// <param name="pObtenerValoresPropiedadesSemanticas">Indica si debe obtener las propiedades semánticas</param>
        /// <returns></returns>
        public ResourceModel ObtenerRecursoPorID(Guid pRecursoID, string pUrlBaseUrlBusqueda, Guid? pPestanyaID, bool pObtenerValoresPropiedadesSemanticas)
        {
            List<Guid> listaRecursosID = new List<Guid>();
            listaRecursosID.Add(pRecursoID);
            Dictionary<Guid, ResourceModel> recursosAux = ObtenerRecursosPorID(listaRecursosID, pUrlBaseUrlBusqueda, pPestanyaID, pObtenerValoresPropiedadesSemanticas);
            if (recursosAux.Count == 1)
            {
                return recursosAux[pRecursoID];
            }
            return null;
        }

        /// <summary>
        /// Obtiene los modelos de los recursos del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaRecursosID">Identificadores de los recursos</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <param name="pPestanyaID">ID de la pestanya (puesde ser nulo)</param>
        /// <param name="pObtenerValoresPropiedadesSemanticas">Indica si debe obtener las propiedades semánticas</param>
        /// <param name="pObtenerIdentidades">Indica si debe obtener las identidades de los publicadores</param>
        /// <param name="pObtenerDatosExtraIdentidades">Indica si debe obtener los datos extra de las identidades de los publicadores</param>
        /// <returns></returns>
        public Dictionary<Guid, ResourceModel> ObtenerRecursosPorID(List<Guid> pListaRecursosID, string pUrlBaseUrlBusqueda, Guid? pPestanyaID, bool pObtenerValoresPropiedadesSemanticas, bool pObtenerIdentidades = true, bool pObtenerDatosExtraIdentidades = false, bool pObtenerUltimaVersion = false, bool pEsFichaRecurso = false)
        {
            return ObtenerRecursosPorID(pListaRecursosID, pUrlBaseUrlBusqueda, Controladores.EspacioPersonal.No, pPestanyaID, pObtenerValoresPropiedadesSemanticas, pObtenerIdentidades, pObtenerDatosExtraIdentidades, pObtenerUltimaVersion, pEsFichaRecurso);
        }

        /// <summary>
        /// Obtiene los modelos de los recursos del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaRecursosID">Identificadores de los recursos</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <param name="pEspacioPersonal">Indica si el recurso se está viendo desde una base de recursos personal</param>
        /// <param name="pPestanyaID">ID de la pestanya (puesde ser nulo)</param>
        /// <param name="pObtenerValoresPropiedadesSemanticas">Indica si debe obtener las propiedades semánticas</param>
        /// <param name="pObtenerIdentidades">Indica si debe obtener las identidades de los publicadores</param>
        /// <param name="pObtenerDatosExtraIdentidades">Indica si debe obtener los datos extra de las identidades de los publicadores</param>
        /// <returns></returns>
        public Dictionary<Guid, ResourceModel> ObtenerRecursosPorID(List<Guid> pListaRecursosID, string pUrlBaseUrlBusqueda, EspacioPersonal pEspacioPersonal, Guid? pPestanyaID, bool pObtenerValoresPropiedadesSemanticas, bool pObtenerIdentidades = true, bool pObtenerDatosExtraIdentidades = false, bool pObtenerUltimaVersion = false, bool pEsFichaRecurso = false)
        {
            KeyValuePair<Guid?, bool> baseRecursosPersonal = new KeyValuePair<Guid?, bool>(null, false);

            if (pEspacioPersonal != Controladores.EspacioPersonal.No)
            {
                baseRecursosPersonal = ObtenerBaseRecursosPersonalID(pEspacioPersonal);
            }

            //Procesamos los modelos para su presentación
            Dictionary<Guid, ResourceModel> listaRecursos = ObtenerRecursosPorIDInt(pListaRecursosID, pUrlBaseUrlBusqueda, baseRecursosPersonal, pObtenerIdentidades, pObtenerDatosExtraIdentidades, pObtenerUltimaVersion: pObtenerUltimaVersion, pEsFichaRecurso: pEsFichaRecurso);
            Dictionary<Guid, ResourceModel> listaRecursosTemp = new Dictionary<Guid, ResourceModel>();
            Dictionary<Guid, ResourceModel> listaRecursosDevolver = new Dictionary<Guid, ResourceModel>();
            foreach (Guid id in listaRecursos.Keys)
            {
                ResourceModel ficha = (ResourceModel)listaRecursos[id].Clone();
                ficha.Title = UtilCadenas.ObtenerTextoDeIdioma(ficha.Title, mUtilIdiomas.LanguageCode, null);
                ficha.Description = UtilCadenas.ObtenerTextoDeIdioma(ficha.Description, mUtilIdiomas.LanguageCode, null);
                ficha.UrlSearch = pUrlBaseUrlBusqueda;

                // Comprobación de si el modelo puede votarse
                ficha.AllowVotes = ParametrosGeneralesRow.VotacionesDisponibles;

                if (ficha.Categories == null)
                {
                    ficha.Categories = new List<CategoryModel>();
                }

                if (!string.IsNullOrEmpty(ficha.RdfType))
                {
                    string rdfTypeName = "";
                    List<OntologiaProyecto> filasOntologias = null;
                    //Select("OntologiaProyecto = '" + ficha.RdfType + "' and ProyectoID = '" + ficha.ProjectID + "'");
                    bool tieneProyectoSuperiorId = mProyecto.FilaProyecto.ProyectoSuperiorID.HasValue;
                    if (tieneProyectoSuperiorId)
                    {
                        FacetaCN facCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                        filasOntologias = facCN.ObtenerOntologias(mProyecto.FilaProyecto.ProyectoSuperiorID.Value).Where(ontologia => ontologia.OntologiaProyecto1.Equals(ficha.RdfType) && ontologia.ProyectoID.Equals(mProyecto.FilaProyecto.ProyectoSuperiorID.Value)).ToList();
                        facCN.Dispose();
                    }
                    else
                    {
                        filasOntologias = mProyecto.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto.Where(ontologia => ontologia.OntologiaProyecto1.Equals(ficha.RdfType) && ontologia.ProyectoID.Equals(ficha.ProjectID)).ToList();
                    }

                    if (filasOntologias.Count > 0)
                    {
                        rdfTypeName = UtilCadenas.ObtenerTextoDeIdioma((filasOntologias.First()).NombreOnt, mUtilIdiomas.LanguageCode, mParametrosGeneralesRow.IdiomaDefecto);
                    }
                    else
                    {
                        if (tieneProyectoSuperiorId)
                        {
                            FacetaCN facCN = new FacetaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCN>(), mLoggerFactory);
                            filasOntologias = facCN.ObtenerOntologias(mProyecto.FilaProyecto.ProyectoSuperiorID.Value).Where(ontologia => ontologia.OntologiaProyecto1.Equals(ficha.RdfType)).ToList();
                            facCN.Dispose();
                        }
                        else
                        {
                            filasOntologias = mProyecto.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto.Where(ontologia => ontologia.OntologiaProyecto1.Equals(ficha.RdfType)).ToList();
                        }

                        if (filasOntologias.Count > 0)
                        {
                            rdfTypeName = UtilCadenas.ObtenerTextoDeIdioma(filasOntologias.First().NombreOnt, mUtilIdiomas.LanguageCode, mParametrosGeneralesRow.IdiomaDefecto);
                        }
                    }

                    ficha.RdfTypeName = rdfTypeName;
                }
                else
                {
                    switch (ficha.TypeDocument)
                    {
                        case ResourceModel.DocumentType.Audio:
                        case ResourceModel.DocumentType.AudioBrightcove:
                        case ResourceModel.DocumentType.AudioTOP:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCAUDIO");
                            break;
                        case ResourceModel.DocumentType.Blog:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCBLOG");
                            break;
                        case ResourceModel.DocumentType.DafoProyecto:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDAFO");
                            break;
                        case ResourceModel.DocumentType.Debate:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDEBATE");
                            break;
                        case ResourceModel.DocumentType.Encuesta:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCENCUESTA");
                            CargarEncuesta(ficha);
                            break;
                        case ResourceModel.DocumentType.EntradaBlog:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCENTRADABLOG");
                            break;
                        case ResourceModel.DocumentType.EntradaBlogTemporal:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCENTRADABLOGTEMP");
                            break;
                        case ResourceModel.DocumentType.FicheroServidor:
                            switch (ficha.NameImage)
                            {
                                case "video":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCVIDEO");
                                    break;
                                case "audio":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCAUDIO");
                                    break;
                                case "documento":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCTEXTO");
                                    break;
                                case "presentacion":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCPRESENTACION");
                                    break;
                                case "xls":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCEXCEL");
                                    break;
                                case "pdf":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCPDF");
                                    break;
                                case "zip":
                                    ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCDOCARCHIVOCOMPRIMIDO");
                                    break;
                                default:
                                    ficha.RdfTypeName = ficha.NameImage;
                                    break;
                            }
                            break;
                        case ResourceModel.DocumentType.Hipervinculo:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCHIPERVINCULO");
                            break;
                        case ResourceModel.DocumentType.Imagen:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCIMAGEN");
                            break;
                        case ResourceModel.DocumentType.ImagenWiki:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCIMGWIKI");
                            break;
                        case ResourceModel.DocumentType.Newsletter:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCNEWSLETTER");
                            break;
                        case ResourceModel.DocumentType.Nota:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCNOTA");
                            break;
                        case ResourceModel.DocumentType.Pregunta:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCPREGUNTA");
                            break;
                        case ResourceModel.DocumentType.Video:
                        case ResourceModel.DocumentType.VideoBrightcove:
                        case ResourceModel.DocumentType.VideoTOP:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCVIDEO");
                            break;
                        case ResourceModel.DocumentType.Wiki:
                        case ResourceModel.DocumentType.WikiTemporal:
                            ficha.RdfTypeName = mUtilIdiomas.GetText("COMBUSQUEDAAVANZADA", "TIPODOCWIKI");
                            break;
                    }
                }

                if (ficha.Categories != null)
                {
                    foreach (CategoryModel categoria in ficha.Categories)
                    {
                        categoria.Lang = mUtilIdiomas.LanguageCode;
                    }
                }

                listaRecursosDevolver.Add(id, ficha);
            }

            //listaRecursosDevolver = (Dictionary<Guid, ResourceModel>)ProcesarModeloParaPresentacion(listaRecursosTemp, pIdioma);

            if (pEspacioPersonal != Controladores.EspacioPersonal.No)
            {
                CompletarCargaRecursosEspacioPersonal(listaRecursosDevolver, pEspacioPersonal);
            }

            if (pObtenerValoresPropiedadesSemanticas)
            {
                ObtenerValoresPropiedadesSemanticas(listaRecursosDevolver, pUrlBaseUrlBusqueda, pPestanyaID);
            }

            return listaRecursosDevolver;
        }

        public void CargarEncuesta(ResourceModel pFichaRecurso)
        {
            if (pFichaRecurso.TypeDocument.Equals(ResourceModel.DocumentType.Encuesta))
            {
                if (pFichaRecurso.Poll == null)
                {
                    pFichaRecurso.Poll = new ResourceModel.PollModel();
                }

                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                DataWrapperDocumentacion dataWrapperDocumentacion = docCN.ObtenerOpcionesEncuesta(pFichaRecurso.Key);

                //No tenemos acceso desde el servicio de contextos a el usuario actual
                bool yaVotado = false;

                //if (!pIdentidad.EsIdentidadInvitada)
                //{
                //    yaVotado = dataWrapperDocumentacion.ListaDocumentoRespuestaVoto.Where(item => item.IdentidadID.Equals(pIdentidad.Clave)).Count() > 0;
                //}

                pFichaRecurso.Poll.Voted = yaVotado;

                pFichaRecurso.Poll.ViewPollResults = yaVotado;

                if (!pFichaRecurso.FullyLoaded || pFichaRecurso.Poll.PollOptions == null)
                {
                    pFichaRecurso.Poll.PollOptions = new List<ResourceModel.PollModel.PollOptionsModel>();

                    Dictionary<Guid, RespuestaRecurso> listaRespuestas = new Dictionary<Guid, RespuestaRecurso>();

                    foreach (DocumentoRespuesta filaRespuesta in dataWrapperDocumentacion.ListaDocumentoRespuesta.Where(item => item.DocumentoID.Equals(pFichaRecurso.Key)))
                    {
                        ResourceModel.PollModel.PollOptionsModel opcionEncuesta = new ResourceModel.PollModel.PollOptionsModel();
                        opcionEncuesta.Key = filaRespuesta.RespuestaID;
                        opcionEncuesta.Name = filaRespuesta.Descripcion;
                        opcionEncuesta.NumberOfVotes = filaRespuesta.NumVotos;
                        pFichaRecurso.Poll.PollOptions.Add(opcionEncuesta);
                    }
                }
            }
        }

        public void ObtenerValoresPropiedadesSemanticas(Dictionary<Guid, ResourceModel> pListaRecursosModel, string pBaseUrlBusqueda, Guid? pPestanyaID)
        {
            DataWrapperProyecto dataWrapperProyectoPresentacion = null;
            FacetadoDS facDSPresentacion = null;
            FacetadoDS facDSPresentacionPersonalizado = null;
            Dictionary<Guid, Guid> DocumentoElementoVinculadoID = new Dictionary<Guid, Guid>();
            DataWrapperFacetas facetaDWConfiguracionOntologia = new DataWrapperFacetas();

            Dictionary<string, List<string>> informacionOntologias = new UtilServiciosFacetas(mLoggingService, mEntityContext, mConfigService, mRedisCacheWrapper, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UtilServiciosFacetas>(), mLoggerFactory).ObtenerInformacionOntologias(mProyecto.FilaProyecto.OrganizacionID, mProyecto.Clave, facetaDWConfiguracionOntologia);

            FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetaCL>(), mLoggerFactory);
            GestionFacetas gestorFacetas = new GestionFacetas(facetaCL.ObtenerTodasFacetasDeProyecto(null, mProyecto.FilaProyecto.OrganizacionID, mProyecto.Clave, false));

            if (pListaRecursosModel.Count > 0)
            {
                var FichasRecursoSemanticas = pListaRecursosModel.Values.Where((FichaRecursoModel, indice) => FichaRecursoModel.TypeDocument == ResourceModel.DocumentType.Semantico);
                List<Guid> listaRecursosSem = new List<Guid>();
                foreach (ResourceModel fichaRecurso in FichasRecursoSemanticas)
                {
                    listaRecursosSem.Add(fichaRecurso.Key);
                }

                if (listaRecursosSem.Count > 0)
                {
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                    DocumentoElementoVinculadoID = docCN.ObtenerElementoVinculadoIDPorDocumentoID(listaRecursosSem);

                    Dictionary<Guid, Guid> ontologiasEnProyecto = docCN.ObtenerElementoVinculadoIDDeOtroProyectoConMismoEnlace(DocumentoElementoVinculadoID.Values.Distinct().ToList(), mProyecto.Clave);

                    //Modificar en DocumentoElementoVinculadoID si existe en ontologiasEnProyecto
                    if (ontologiasEnProyecto.Count > 0)
                    {
                        foreach (Guid ontologia in ontologiasEnProyecto.Keys)
                        {
                            var documentosConOntologiaDeOtroProyecto = DocumentoElementoVinculadoID.Where(par => par.Value.Equals(ontologia)).ToDictionary(par => par.Key).Keys;
                            foreach (Guid documentoConOntologia in documentosConOntologiaDeOtroProyecto)
                            {
                                DocumentoElementoVinculadoID[documentoConOntologia] = ontologiasEnProyecto[ontologia];
                            }
                        }
                    }

                    docCN.Dispose();
                    facDSPresentacion = ObtenerPresentacionFacetado(DocumentoElementoVinculadoID, mProyecto.Clave, mProyectoOrigenID, ref dataWrapperProyectoPresentacion, TipoFichaResultados.Completa, mUtilIdiomas.LanguageCode, pPestanyaID, facetaDWConfiguracionOntologia, informacionOntologias);
                    facDSPresentacionPersonalizado = ObtenerPresentacionFacetadoPersonalizado(DocumentoElementoVinculadoID, mProyecto.Clave, mProyectoOrigenID, dataWrapperProyectoPresentacion, mUtilIdiomas.LanguageCode, pPestanyaID, facetaDWConfiguracionOntologia, informacionOntologias);
                }
            }

            //Preparamos FacetadoDS eliminado las '@' al final de las propiedades
            if (facDSPresentacion != null && facDSPresentacion.Tables["SelectPropEnt"] != null)
            {
                foreach (DataRow fila in facDSPresentacion.Tables["SelectPropEnt"].Rows)
                {
                    while (fila["p"].ToString().EndsWith("@@@"))
                    {
                        fila["p"] = fila["p"].ToString().Substring(0, fila["p"].ToString().LastIndexOf("@@@"));
                    }
                }
            }

            Dictionary<Guid, Dictionary<string, List<string>>> listaPropiedadesRecurso = new Dictionary<Guid, Dictionary<string, List<string>>>();
            if (facDSPresentacion != null && facDSPresentacion.Tables["SelectPropEnt"] != null)
            {
                foreach (DataRow fila in facDSPresentacion.Tables["SelectPropEnt"].Rows)
                {
                    string s = fila["s"].ToString();
                    string p = fila["p"].ToString();
                    string o = fila["o"].ToString();
                    if (facDSPresentacion.Tables["SelectPropEnt"].Columns.IndexOf("idioma") > 0 && !string.IsNullOrEmpty(fila["idioma"].ToString().Trim()))
                    {
                        o += "@" + fila["idioma"].ToString();
                    }

                    Guid idRecurso = Guid.Empty;
                    if (Guid.TryParse(s.Substring(s.LastIndexOf("/") + 1), out idRecurso))
                    {
                        Dictionary<string, List<string>> propiedadesRecurso = new Dictionary<string, List<string>>();
                        if (listaPropiedadesRecurso.ContainsKey(idRecurso))
                        {
                            propiedadesRecurso = listaPropiedadesRecurso[idRecurso];
                        }
                        else
                        {
                            listaPropiedadesRecurso.Add(idRecurso, propiedadesRecurso);
                        }

                        List<string> listaValoresPropiedad = new List<string>();
                        if (propiedadesRecurso.ContainsKey(p))
                        {
                            listaValoresPropiedad = propiedadesRecurso[p];
                        }
                        else
                        {
                            propiedadesRecurso.Add(p, listaValoresPropiedad);
                        }

                        if (!propiedadesRecurso[p].Contains(o))
                        {
                            propiedadesRecurso[p].Add(o);
                        }
                    }
                }
            }


            foreach (Guid idRecurso in pListaRecursosModel.Keys)
            {
                ResourceModel fichaRecurso = pListaRecursosModel[idRecurso];

                if (fichaRecurso.TypeDocument == ResourceModel.DocumentType.Semantico && dataWrapperProyectoPresentacion != null)
                {
                    bool configuracionPersonalizada = false;
                    bool configuracionPersonalizadaDS = false;
                    foreach (PresentacionListadoSemantico filaListado in dataWrapperProyectoPresentacion.ListaPresentacionListadoSemantico)
                    {
                        if (DocumentoElementoVinculadoID.ContainsKey(idRecurso) && filaListado.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                        {
                            configuracionPersonalizada = true;
                            break;
                        }
                    }

                    foreach (PresentacionMosaicoSemantico filaMosaico in dataWrapperProyectoPresentacion.ListaPresentacionMosaicoSemantico)
                    {
                        if (DocumentoElementoVinculadoID.ContainsKey(idRecurso) && filaMosaico.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                        {
                            configuracionPersonalizada = true;
                            break;
                        }
                    }

                    foreach (RecursosRelacionadosPresentacion filaContexto in dataWrapperProyectoPresentacion.ListaRecursosRelacionadosPresentacion)
                    {
                        if (DocumentoElementoVinculadoID.ContainsKey(idRecurso) && filaContexto.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                        {
                            configuracionPersonalizada = true;
                            break;
                        }
                    }

                    foreach (PresentacionMapaSemantico filaMapa in dataWrapperProyectoPresentacion.ListaPresentacionMapaSemantico)
                    {
                        if (DocumentoElementoVinculadoID.ContainsKey(idRecurso) && filaMapa.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                        {
                            configuracionPersonalizada = true;
                            break;
                        }
                    }

                    foreach (PresentacionPersonalizadoSemantico filaPersonalizado in dataWrapperProyectoPresentacion.ListaPresentacionPersonalizadoSemantico)
                    {
                        if (DocumentoElementoVinculadoID.ContainsKey(idRecurso) && filaPersonalizado.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                        {
                            configuracionPersonalizadaDS = true;
                            break;
                        }
                    }

                    if (configuracionPersonalizada || configuracionPersonalizadaDS)
                    {
                        fichaRecurso.ViewSettings = new ViewSettingResorceModel();
                        if (configuracionPersonalizada)
                        {
                            #region Pintar propiedades semánticas y/o descripcion, etiquetas, categorias

                            fichaRecurso.ViewSettings.ListView = GenerarViewResourceModel(TipoVista.Lista, DocumentoElementoVinculadoID[idRecurso], dataWrapperProyectoPresentacion, String.Empty);
                            fichaRecurso.ViewSettings.MosaicView = GenerarViewResourceModel(TipoVista.Mosaico, DocumentoElementoVinculadoID[idRecurso], dataWrapperProyectoPresentacion, String.Empty);
                            fichaRecurso.ViewSettings.MapView = GenerarViewResourceModel(TipoVista.Mapa, DocumentoElementoVinculadoID[idRecurso], dataWrapperProyectoPresentacion, String.Empty);
                            fichaRecurso.ViewSettings.ContextView = GenerarViewResourceModel(TipoVista.Contexto, DocumentoElementoVinculadoID[idRecurso], dataWrapperProyectoPresentacion, String.Empty);

                            TipoVista vistaDefecto = TipoVista.Lista;

                            if (pPestanyaID.HasValue)
                            {
                                string vistasDisponibles = dataWrapperProyectoPresentacion.ListaProyectoPestanyaBusqueda.Where(item => item.PestanyaID.Equals(pPestanyaID.Value)).Select(item => item.VistaDisponible).FirstOrDefault();

                                if (!string.IsNullOrWhiteSpace(vistasDisponibles))
                                {
                                    int posicionVistaDefecto = vistasDisponibles.IndexOf("2");

                                    if (posicionVistaDefecto != -1)
                                    {
                                        vistaDefecto = (TipoVista)posicionVistaDefecto;
                                    }
                                }
                            }

                            #region vista listado

                            string resultadoPintar = "";

                            foreach (PresentacionListadoSemantico filaPresentacionListado in dataWrapperProyectoPresentacion.ListaPresentacionListadoSemantico)
                            {
                                if (filaPresentacionListado.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                                {
                                    string[] propiedades = filaPresentacionListado.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                                    resultadoPintar += PintarPropiedadesSemDeVista(ref fichaRecurso, filaPresentacionListado.Nombre, propiedades, listaPropiedadesRecurso, idRecurso, pBaseUrlBusqueda, informacionOntologias, gestorFacetas);
                                }
                            }
                            fichaRecurso.ViewSettings.ListView.InfoExtra = resultadoPintar;

                            #endregion

                            #region vista mosaico

                            string resultadoPintarMosaico = "";

                            foreach (PresentacionMosaicoSemantico filaPresentacionMosaico in dataWrapperProyectoPresentacion.ListaPresentacionMosaicoSemantico)
                            {
                                if (filaPresentacionMosaico.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                                {
                                    string[] propiedades = filaPresentacionMosaico.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                                    resultadoPintarMosaico += PintarPropiedadesSemDeVista(ref fichaRecurso, filaPresentacionMosaico.Nombre, propiedades, listaPropiedadesRecurso, idRecurso, pBaseUrlBusqueda, informacionOntologias, gestorFacetas);
                                }
                            }
                            fichaRecurso.ViewSettings.MosaicView.InfoExtra = resultadoPintarMosaico;

                            #endregion

                            #region Vista mapa

                            string resultadoPintarMapa = "";

                            foreach (PresentacionMapaSemantico filaPresentacionMapa in dataWrapperProyectoPresentacion.ListaPresentacionMapaSemantico)
                            {
                                if (filaPresentacionMapa.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                                {
                                    string[] propiedades = filaPresentacionMapa.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                                    resultadoPintarMapa += PintarPropiedadesSemDeVista(ref fichaRecurso, filaPresentacionMapa.Nombre, propiedades, listaPropiedadesRecurso, idRecurso, pBaseUrlBusqueda, informacionOntologias, gestorFacetas);
                                }
                            }
                            fichaRecurso.ViewSettings.MapView.InfoExtra = resultadoPintarMapa;

                            #endregion

                            #region Vista contexto

                            string resultadoPintarContexto = "";
                            foreach (RecursosRelacionadosPresentacion filaPresentacionContexto in dataWrapperProyectoPresentacion.ListaRecursosRelacionadosPresentacion)
                            {
                                if (filaPresentacionContexto.OntologiaID == DocumentoElementoVinculadoID[idRecurso])
                                {
                                    string[] propiedades = filaPresentacionContexto.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                                    resultadoPintarContexto += PintarPropiedadesSemDeVista(ref fichaRecurso, filaPresentacionContexto.Nombre, propiedades, listaPropiedadesRecurso, idRecurso, pBaseUrlBusqueda, informacionOntologias, gestorFacetas);
                                }
                            }
                            fichaRecurso.ViewSettings.ContextView.InfoExtra = resultadoPintarContexto;

                            #endregion

                            fichaRecurso.ViewSettings.VistaDefecto = vistaDefecto;

                            #endregion
                        }
                        if (configuracionPersonalizadaDS)
                        {
                            fichaRecurso.ViewSettings.CustomSemanticProperties = new List<CustomSemanticPropertiesModel>();
                            foreach (DataTable tabla in facDSPresentacionPersonalizado.Tables)
                            {
                                DataRow[] filas = tabla.Select("s='http://gnoss/" + idRecurso.ToString().ToUpper() + "'");
                                if (filas.Count() > 0)
                                {
                                    CustomSemanticPropertiesModel customSemanticPropertiesModel = new CustomSemanticPropertiesModel();
                                    customSemanticPropertiesModel.Id = tabla.TableName;
                                    customSemanticPropertiesModel.Rows = new List<Dictionary<string, string>>();
                                    fichaRecurso.ViewSettings.CustomSemanticProperties.Add(customSemanticPropertiesModel);
                                    foreach (DataRow fila in filas)
                                    {
                                        Dictionary<string, string> filaCustomSemanticProperties = new Dictionary<string, string>();
                                        foreach (DataColumn column in tabla.Columns)
                                        {

                                            filaCustomSemanticProperties.Add(column.ColumnName, fila[column.ColumnName].ToString());
                                        }
                                        customSemanticPropertiesModel.Rows.Add(filaCustomSemanticProperties);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el dataSet con la configuración de la presentación de los resultados.
        /// </summary>
        /// <param name="pListaRecursosSemanticos">Lista recursos semanticos con IDrecurso/idelementovinculado</param>
        /// <returns>DataSet con la configuración de la presentación de los resultados</returns>
        private FacetadoDS ObtenerPresentacionFacetado(Dictionary<Guid, Guid> pListaRecursosSemanticos, Guid pProyectoSeleccionado, Guid pProyectoOrigenID, ref DataWrapperProyecto pProyDSPresentacion, TipoFichaResultados pTipoFichaResultados, string pIdioma, Guid? pPestanyaID, DataWrapperFacetas pConfiguracionOntologia, Dictionary<string, List<string>> pInformacionOntologias)
        {
            #region Obtener propiedades de documentos semanticos para listado
            FacetadoDS facDSPresentacion = null;
            List<string> listaPropiedades = new List<string>();
            List<Guid> listaDocsSem = new List<Guid>();
            List<Guid> listaDocsSemObtenerCache = new List<Guid>();
            List<string> listaGrafos = new List<string>();

            if (pProyectoSeleccionado != ProyectoAD.MyGnoss)
            {
                Guid ProyectoOrigen = pProyectoSeleccionado;
                if (pProyectoOrigenID != Guid.Empty)
                {
                    ProyectoOrigen = pProyectoOrigenID;
                }

                // Cargamos presentacion listado o Contexto
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                // Hago una copia porque el dataset está en caché local y si varios hilos modifican a la vez el mismo dataset puede dar problemas del tipo: 
                // "DataTable internal index is corrupted"
                pProyDSPresentacion = new DataWrapperProyecto();
                pProyDSPresentacion.Merge(proyCL.ObtenerPresentacionSemantico(ProyectoOrigen))/*..Copy()*/;
                proyCL.Dispose();
                Elementos.ServiciosGenerales.ProyectoPestanyaMenu.ProcesarPresentacionPestanyas(ref pProyDSPresentacion, pPestanyaID);


                List<Guid> listaIdOnt = new List<Guid>();
                foreach (Guid idontologia in pListaRecursosSemanticos.Values)
                {
                    if (!listaIdOnt.Contains(idontologia))
                    {
                        listaIdOnt.Add(idontologia);
                    }
                }
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                Dictionary<Guid, string> listaEnlacesDocumento = docCN.ObtenerEnlacesDocumentosPorDocumentoID(listaIdOnt);
                docCN.Dispose();

                foreach (Guid idRecurso in pListaRecursosSemanticos.Keys)
                {
                    listaDocsSem.Add(idRecurso);
                    bool obtenerDeCache = true;
                    if (pConfiguracionOntologia != null)
                    {
                        foreach (OntologiaProyecto myrow in pConfiguracionOntologia.ListaOntologiaProyecto)
                        {
                            if ((myrow.OntologiaProyecto1 + ".owl").Equals(listaEnlacesDocumento[pListaRecursosSemanticos[idRecurso]]) && !myrow.CachearDatosSemanticos)
                            {
                                obtenerDeCache = false;
                            }
                        }
                    }
                    if (obtenerDeCache)
                    {
                        listaDocsSemObtenerCache.Add(idRecurso);
                    }

                    #region Cargamos las propiedades que queremos mostrar

                    #region Listado
                    foreach (PresentacionListadoSemantico filaPresentacionListado in pProyDSPresentacion.ListaPresentacionListadoSemantico)
                    {
                        if (filaPresentacionListado.Propiedad != "etiquetas" && filaPresentacionListado.Propiedad != "categorias" && filaPresentacionListado.Propiedad != "descripcion" && filaPresentacionListado.Propiedad != "publicador")
                        {
                            if (filaPresentacionListado.Ontologia != "" && pListaRecursosSemanticos[idRecurso] == filaPresentacionListado.OntologiaID)
                            {
                                string[] propiedades = filaPresentacionListado.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string propiedad in propiedades)
                                {

                                    if (!listaPropiedades.Contains(propiedad))
                                    {
                                        listaPropiedades.Add(propiedad);
                                    }

                                    int posicionBarra = filaPresentacionListado.Ontologia.LastIndexOf("/");
                                    string grafo = filaPresentacionListado.Ontologia.Substring(posicionBarra + 1);
                                    if (grafo.Contains("#"))
                                    {
                                        grafo = grafo.Substring(0, grafo.IndexOf("#"));
                                    }

                                    if (!listaGrafos.Contains(grafo))
                                    {
                                        listaGrafos.Add(grafo);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Mosaico
                    foreach (PresentacionMosaicoSemantico filaPresentacionMosaico in pProyDSPresentacion.ListaPresentacionMosaicoSemantico)
                    {
                        if (filaPresentacionMosaico.Propiedad != "etiquetas" && filaPresentacionMosaico.Propiedad != "categorias" && filaPresentacionMosaico.Propiedad != "descripcion" && filaPresentacionMosaico.Propiedad != "publicador")
                        {
                            if (filaPresentacionMosaico.Ontologia != "" && pListaRecursosSemanticos[idRecurso] == filaPresentacionMosaico.OntologiaID)
                            {
                                string[] propiedades = filaPresentacionMosaico.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string propiedad in propiedades)
                                {
                                    if (!listaPropiedades.Contains(propiedad))
                                    {
                                        listaPropiedades.Add(propiedad);
                                    }

                                    int posicionBarra = filaPresentacionMosaico.Ontologia.LastIndexOf("/");
                                    string grafo = filaPresentacionMosaico.Ontologia.Substring(posicionBarra + 1);
                                    if (grafo.Contains("#"))
                                    {
                                        grafo = grafo.Substring(0, grafo.IndexOf("#"));
                                    }

                                    if (!listaGrafos.Contains(grafo))
                                    {
                                        listaGrafos.Add(grafo);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Contexto
                    foreach (RecursosRelacionadosPresentacion filaPresentacionContexto in pProyDSPresentacion.ListaRecursosRelacionadosPresentacion)
                    {
                        if (filaPresentacionContexto.Propiedad != "etiquetas" && filaPresentacionContexto.Propiedad != "categorias" && filaPresentacionContexto.Propiedad != "descripcion" && filaPresentacionContexto.Propiedad != "publicador")
                        {
                            if (filaPresentacionContexto.Ontologia != "" && pListaRecursosSemanticos[idRecurso] == filaPresentacionContexto.OntologiaID)
                            {
                                string[] propiedades = filaPresentacionContexto.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string propiedad in propiedades)
                                {
                                    if (!listaPropiedades.Contains(propiedad))
                                    {
                                        listaPropiedades.Add(propiedad);
                                    }

                                    int posicionBarra = filaPresentacionContexto.Ontologia.LastIndexOf("/");
                                    string grafo = filaPresentacionContexto.Ontologia.Substring(posicionBarra + 1);
                                    if (grafo.Contains("#"))
                                    {
                                        grafo = grafo.Substring(0, grafo.IndexOf("#"));
                                    }

                                    if (!listaGrafos.Contains(grafo))
                                    {
                                        listaGrafos.Add(grafo);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Mapa
                    foreach (PresentacionMapaSemantico filaPresentacionMapa in pProyDSPresentacion.ListaPresentacionMapaSemantico)
                    {
                        if (filaPresentacionMapa.Ontologia != "" && pListaRecursosSemanticos[idRecurso] == filaPresentacionMapa.OntologiaID)
                        {
                            string[] propiedades = filaPresentacionMapa.Propiedad.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (string propiedad in propiedades)
                            {
                                if (!listaPropiedades.Contains(propiedad))
                                {
                                    listaPropiedades.Add(propiedad);
                                }


                                int posicionBarra = filaPresentacionMapa.Ontologia.LastIndexOf("/");
                                string grafo = filaPresentacionMapa.Ontologia.Substring(posicionBarra + 1);
                                if (grafo.Contains("#"))
                                {
                                    grafo = grafo.Substring(0, grafo.IndexOf("#"));
                                }

                                if (!listaGrafos.Contains(grafo))
                                {
                                    listaGrafos.Add(grafo);
                                }
                            }
                        }
                    }
                    #endregion

                    #endregion
                }

                if (pProyDSPresentacion != null && listaPropiedades.Count > 0 && listaGrafos.Count > 0)
                {
                    facDSPresentacion = new FacetadoDS();

                    Dictionary<Guid, FacetadoDS> propsFacetado = new Dictionary<Guid, FacetadoDS>();
                    if (listaDocsSemObtenerCache.Count > 0)
                    {
                        DocumentacionCL documentacionCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                        propsFacetado = documentacionCL.ObtenerFichasRecursosPropiedadesSemanticasMVC(listaDocsSemObtenerCache, ProyectoOrigen, pIdioma);
                        documentacionCL.Dispose();
                    }

                    foreach (Guid idRecurso in propsFacetado.Keys)
                    {
                        facDSPresentacion.Merge(propsFacetado[idRecurso]);
                        listaDocsSem.Remove(idRecurso);
                    }

                    //string urlIntragnoss = ParametrosAplicacion.ParametroAplicacion.FindByParametro("UrlIntragnoss").Valor;
                    string urlIntragnoss = ParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList().First().Valor;
                    FacetadoCN facetadoCN = new FacetadoCN(urlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.InformacionOntologias = pInformacionOntologias;

                    if (listaDocsSem.Count > 0)
                    {
                        foreach (string grafo in listaGrafos)
                        {
                            facDSPresentacion.Merge(facetadoCN.ObtenerValoresPropiedadesEntidadesPorDocumentoID(ProyectoOrigen.ToString(), listaDocsSem, listaPropiedades, pIdioma, true));
                        }
                        Dictionary<Guid, FacetadoDS> propsFacetadoObtenidas = new Dictionary<Guid, FacetadoDS>();
                        foreach (Guid idRecurso in listaDocsSemObtenerCache)
                        {
                            propsFacetadoObtenidas.Add(idRecurso, new FacetadoDS());
                            propsFacetadoObtenidas[idRecurso].Tables.Add("SelectPropEnt");
                            propsFacetadoObtenidas[idRecurso].Tables["SelectPropEnt"].Columns.Add("s");
                            propsFacetadoObtenidas[idRecurso].Tables["SelectPropEnt"].Columns.Add("p");
                            propsFacetadoObtenidas[idRecurso].Tables["SelectPropEnt"].Columns.Add("o");
                            propsFacetadoObtenidas[idRecurso].Tables["SelectPropEnt"].Columns.Add("idioma");
                            foreach (DataRow fila in facDSPresentacion.Tables["SelectPropEnt"].Select("s='http://gnoss/" + idRecurso.ToString().ToUpper() + "'"))
                            {
                                propsFacetadoObtenidas[idRecurso].Tables["SelectPropEnt"].Rows.Add(fila["s"], fila["p"], fila["o"], fila["idioma"]);
                            }
                        }
                        if (propsFacetadoObtenidas.Count > 0)
                        {
                            DocumentacionCL documentacionCL2 = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                            documentacionCL2.AgregarFichasRecursosPropiedadesSemanticasMVC(propsFacetadoObtenidas, ProyectoOrigen, pIdioma);
                            documentacionCL2.Dispose();
                        }
                    }
                }
            }
            #endregion
            return facDSPresentacion;
        }

        /// <summary>
        /// Obtiene el dataSet con la configuración de la presentación de los resultados.
        /// </summary>
        /// <param name="pListaRecursosSemanticos">Lista recursos semanticos con IDrecurso/idelementovinculado</param>
        /// <returns>DataSet con la configuración de la presentación de los resultados</returns>
        private FacetadoDS ObtenerPresentacionFacetadoPersonalizado(Dictionary<Guid, Guid> pListaRecursosSemanticos, Guid pProyectoSeleccionado, Guid pProyectoOrigenID, DataWrapperProyecto pProyDSPresentacion, string pIdioma, Guid? pPestanyaID, DataWrapperFacetas pConfiguracionOntologia, Dictionary<string, List<string>> pInformacionOntologias)
        {
            #region Obtener propiedades personalizadas de documentos semanticos para listado
            FacetadoDS facDSPresentacion = new FacetadoDS();
            if (pProyectoSeleccionado != ProyectoAD.MyGnoss)
            {
                Guid ProyectoOrigen = pProyectoSeleccionado;
                if (pProyectoOrigenID != Guid.Empty)
                {
                    ProyectoOrigen = pProyectoOrigenID;
                }
                List<Guid> listaIdOnt = new List<Guid>();
                foreach (Guid idontologia in pListaRecursosSemanticos.Values)
                {
                    if (!listaIdOnt.Contains(idontologia))
                    {
                        listaIdOnt.Add(idontologia);
                    }
                }
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                Dictionary<Guid, string> listaEnlacesDocumento = docCN.ObtenerEnlacesDocumentosPorDocumentoID(listaIdOnt);
                docCN.Dispose();

                List<Guid> listaDocsSemObtenerCache = new List<Guid>();
                Dictionary<Guid, HashSet<Guid>> listaRecursosOntologias = new Dictionary<Guid, HashSet<Guid>>();
                foreach (Guid idRecurso in pListaRecursosSemanticos.Keys)
                {
                    if (pConfiguracionOntologia != null)
                    {
                        foreach (OntologiaProyecto myrow in pConfiguracionOntologia.ListaOntologiaProyecto)
                        {
                            if ((myrow.OntologiaProyecto1 + ".owl").Equals(listaEnlacesDocumento[pListaRecursosSemanticos[idRecurso]]))
                            {
                                //Sólo lo añadimos a la lista si tiene cosas cionfiguradas
                                if (pProyDSPresentacion.ListaPresentacionPersonalizadoSemantico.Exists(x => x.OntologiaID == pListaRecursosSemanticos[idRecurso]))
                                {
                                    if (!listaRecursosOntologias.ContainsKey(pListaRecursosSemanticos[idRecurso]))
                                    {
                                        listaRecursosOntologias.Add(pListaRecursosSemanticos[idRecurso], new HashSet<Guid>());
                                    }
                                    listaRecursosOntologias[pListaRecursosSemanticos[idRecurso]].Add(idRecurso);
                                    if (myrow.CachearDatosSemanticos)
                                    {
                                        listaDocsSemObtenerCache.Add(idRecurso);
                                    }
                                }
                            }
                        }

                    }

                }

                #region Obtenemos los datos de cache

                Dictionary<Guid, FacetadoDS> propsFacetado = new Dictionary<Guid, FacetadoDS>();
                if (listaDocsSemObtenerCache.Count > 0)
                {
                    DocumentacionCL documentacionCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    propsFacetado = documentacionCL.ObtenerFichasRecursosPropiedadesPersonalizadasSemanticasMVC(listaDocsSemObtenerCache, ProyectoOrigen, pIdioma);
                    documentacionCL.Dispose();
                }

                foreach (Guid idRecursoCache in propsFacetado.Keys)
                {
                    facDSPresentacion.Merge(propsFacetado[idRecursoCache]);
                    foreach (Guid idOntologia in listaRecursosOntologias.Keys)
                    {
                        listaRecursosOntologias[idOntologia].Remove(idRecursoCache);
                    }
                }
                listaRecursosOntologias = listaRecursosOntologias.Where(f => f.Value.Count > 0).ToDictionary(x => x.Key, x => x.Value);

                #endregion

                if (listaRecursosOntologias.Count > 0)
                {
                    //string urlIntragnoss = ParametrosAplicacion.ParametroAplicacion.FindByParametro("UrlIntragnoss").Valor;
                    string urlIntragnoss = ParametrosAplicacion.Where(parametro => parametro.Parametro.Equals("UrlIntragnoss")).ToList().First().Valor;
                    FacetadoCN facetadoCN = new FacetadoCN(urlIntragnoss, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                    facetadoCN.InformacionOntologias = pInformacionOntologias;
                    foreach (Guid idOntologia in listaRecursosOntologias.Keys)
                    {
                        if (pProyDSPresentacion.ListaPresentacionPersonalizadoSemantico.Exists(x => x.OntologiaID == idOntologia))
                        {
                            foreach (PresentacionPersonalizadoSemantico presentacionPersonalizadoSemantico in pProyDSPresentacion.ListaPresentacionPersonalizadoSemantico.Where(x => x.OntologiaID == idOntologia))
                            {
                                facDSPresentacion.Merge(facetadoCN.ObtenerValoresPropiedadesPersonalizadasEntidadesPorDocumentoID(ProyectoOrigen.ToString(), listaRecursosOntologias[idOntologia].ToList(), presentacionPersonalizadoSemantico.ID, presentacionPersonalizadoSemantico.Select, presentacionPersonalizadoSemantico.Where, pIdioma));
                            }
                        }
                    }
                }

                Dictionary<Guid, FacetadoDS> propsFacetadoObtenidas = new Dictionary<Guid, FacetadoDS>();
                foreach (Guid idRecurso in listaDocsSemObtenerCache)
                {
                    propsFacetadoObtenidas.Add(idRecurso, new FacetadoDS());
                    foreach (DataTable tabla in facDSPresentacion.Tables)
                    {
                        if (!propsFacetadoObtenidas[idRecurso].Tables.Contains(tabla.TableName))
                        {
                            propsFacetadoObtenidas[idRecurso].Tables.Add(tabla.TableName);
                            foreach (DataColumn column in tabla.Columns)
                            {
                                propsFacetadoObtenidas[idRecurso].Tables[tabla.TableName].Columns.Add(column.ColumnName);
                            }
                        }
                        foreach (DataRow fila in tabla.Select("s='http://gnoss/" + idRecurso.ToString().ToUpper() + "'"))
                        {
                            propsFacetadoObtenidas[idRecurso].Tables[tabla.TableName].Rows.Add(fila.ItemArray);
                        }
                    }
                }
                if (propsFacetadoObtenidas.Count > 0)
                {
                    DocumentacionCL documentacionCL2 = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                    documentacionCL2.AgregarFichasRecursosPropiedadesPersonalizadasSemanticasMVC(propsFacetadoObtenidas, ProyectoOrigen, pIdioma);
                    documentacionCL2.Dispose();
                }
            }
            #endregion
            return facDSPresentacion;
        }

        /// <summary>
        /// Comprueba la visibilidad del tipo de vista de un recurso.
        /// </summary>
        /// <param name="pTipoVista">0 si es listado, 1 si es mosaico, 2 si es mapa, 3 si es contexto(recursorelacoinadopresentacion)</param>
        /// <param name="pElemento"></param>
        private bool VisibilidadTipoVista(TipoVista pTipoVista, string pElemento, Guid pOntologiaID, DataWrapperProyecto pDataWrapperProyecto)
        {
            bool visible = true;

            if (pDataWrapperProyecto != null)
            {
                visible = false;

                if (pTipoVista == TipoVista.Lista)
                {
                    visible = pDataWrapperProyecto.ListaPresentacionListadoSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(pElemento)).ToList().Count > 0;
                }
                else if (pTipoVista == TipoVista.Mosaico)
                {
                    visible = pDataWrapperProyecto.ListaPresentacionMosaicoSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(pElemento)).ToList().Count > 0;
                }
                else if (pTipoVista == TipoVista.Mapa)
                {
                    visible = pDataWrapperProyecto.ListaPresentacionMapaSemantico.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(pElemento)).ToList().Count > 0;
                }
                else if (pTipoVista == TipoVista.Contexto)
                {
                    visible = pDataWrapperProyecto.ListaRecursosRelacionadosPresentacion.Where(presentacion => presentacion.OntologiaID.Equals(pOntologiaID) && presentacion.Propiedad.Equals(pElemento)).ToList().Count > 0;
                }
            }

            return visible;
        }



        /// <summary>
        /// Pinta las propiedades semánticas de un tipo de vista.
        /// </summary>
        /// <param name="pDiv">Div del tipo de vista</param>
        /// <param name="pPropPintar">Propiedad a pintar</param>
        /// <param name="pPropiedades">Lista de propiedades</param>
        private string PintarPropiedadesSemDeVista(ref ResourceModel pFichaRecurso, string pPropPintar, string[] pPropiedades, Dictionary<Guid, Dictionary<string, List<string>>> pListaPropiedadesRecurso, Guid pDcomumentoID, string pBaseURLbusqueda, Dictionary<string, List<string>> pInformacionOntologias, GestionFacetas pGestorFacetas)
        {
            #region vista

            string[] propiedades = pPropiedades;
            bool pintar = false;
            string propiedadAPintar = UtilCadenas.ObtenerTextoDeIdioma(pPropPintar, mUtilIdiomas.LanguageCode, mParametrosGeneralesRow.IdiomaDefecto);

            if (string.IsNullOrEmpty(propiedadAPintar))
            {
                propiedadAPintar = "";
            }

            Dictionary<string, List<string>> dicNombreAbreviatura = UtilServiciosFacetas.ObtenerInformacionOntologiasSinArroba(UrlIntragnoss, pInformacionOntologias);

            foreach (string key in FacetadoAD.ListaNamespacesBasicos.Keys)
            {
                if (!dicNombreAbreviatura.ContainsKey(FacetadoAD.ListaNamespacesBasicos[key]))
                {
                    dicNombreAbreviatura.Add(FacetadoAD.ListaNamespacesBasicos[key], new List<string>());
                }
                if (!dicNombreAbreviatura[FacetadoAD.ListaNamespacesBasicos[key]].Contains(key))
                {
                    dicNombreAbreviatura[FacetadoAD.ListaNamespacesBasicos[key]].Add(key);
                }
            }

            int i = propiedades.Length - 1;
            foreach (string propMulti in propiedades.Reverse())
            {
                string[] propsMulti = propMulti.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string propint in propsMulti)
                {
                    string propiedadAux = propint.Replace("RRR", "@@@");
                    propiedadAux = propiedadAux.Replace("[MultiIdioma]", "");

                    FacetaMayuscula tipoMayusculas = FacetaMayuscula.Nada;
                    bool transformacionFecha = false;
                    bool facetaBooleana = false;

                    if (pGestorFacetas.ListaFacetasPorClave.ContainsKey(propiedadAux))
                    {
                        tipoMayusculas = pGestorFacetas.ListaFacetasPorClave[propiedadAux].Mayusculas;
                        transformacionFecha = pGestorFacetas.ListaFacetasPorClave[propiedadAux].AlgoritmoTransformacion == TiposAlgoritmoTransformacion.Fechas || pGestorFacetas.ListaFacetasPorClave[propiedadAux].AlgoritmoTransformacion == TiposAlgoritmoTransformacion.Calendario || pGestorFacetas.ListaFacetasPorClave[propiedadAux].AlgoritmoTransformacion == TiposAlgoritmoTransformacion.CalendarioConRangos;
                        facetaBooleana = (pGestorFacetas.ListaFacetasPorClave[propiedadAux].AlgoritmoTransformacion == TiposAlgoritmoTransformacion.Booleano);
                    }

                    string valorProp = SustituirPrefijos(propiedadAux, dicNombreAbreviatura);

                    int indice = 0;
                    if (propsMulti.Length > 1)
                    {
                        valorProp = "";
                        int l = 0;
                        foreach (string prop in propsMulti)
                        {
                            if (l > 0)
                            {
                                valorProp += "||";
                            }
                            valorProp += SustituirPrefijos(prop.Replace("[MultiIdioma]", "").Replace("RRR", "@@@"), dicNombreAbreviatura);
                            if (propiedadAux.Contains(prop.Replace("[MultiIdioma]", "").Replace("RRR", "@@@")))
                            {
                                indice = l;
                            }
                            l++;
                        }
                    }

                    if (pListaPropiedadesRecurso.ContainsKey(pDcomumentoID) && pListaPropiedadesRecurso[pDcomumentoID].ContainsKey(valorProp))
                    {
                        string objeto = "";
                        int contPropActual = 0;

                        foreach (string valor in pListaPropiedadesRecurso[pDcomumentoID][valorProp])
                        {
                            if (valor.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries).ToList().Count > 0)
                            {
                                //NO hacer nada

                                string idioma = "";
                                string objetoTemp = valor.Split(new string[] { "||" }, StringSplitOptions.None)[indice];

                                int indiceIdioma = -1;

                                if (objetoTemp.Length > 3 && objetoTemp[objetoTemp.Length - 3] == '@')
                                {
                                    // Clave de idioma de dos caracteres. Ej: en
                                    indiceIdioma = 3;
                                }
                                else if (objetoTemp.Length > 6 && objetoTemp[objetoTemp.Length - 6] == '@' && objetoTemp[objetoTemp.Length - 3] == '-')
                                {
                                    // Clave de idioma de cuatro caracteres (idioma + región). Ej: en-us
                                    indiceIdioma = 6;
                                }

                                if (indiceIdioma > 0)
                                {
                                    objetoTemp = objetoTemp.Substring(0, valor.LastIndexOf('@'));
                                }
                                string nombre = objetoTemp;
                                string filtro = objetoTemp;
                                if (indiceIdioma > 0)
                                {
                                    idioma = valor.Substring(valor.LastIndexOf('@'));
                                }

                                if (propiedadAux.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
                                {
                                    nombre = FacetaAD.ObtenerTextoSubTipoDeIdioma(nombre, pGestorFacetas.FacetasDW.ListaOntologiaProyecto, mUtilIdiomas.LanguageCode);
                                    filtro = FacetaAD.ObtenerValorAplicandoNamespaces(filtro, pGestorFacetas.FacetasDW.ListaOntologiaProyecto, false);
                                }

                                switch (tipoMayusculas)
                                {
                                    case FacetaMayuscula.MayusculasTodasPalabras:
                                        nombre = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(nombre);
                                        break;
                                    case FacetaMayuscula.MayusculasTodoMenosArticulos:
                                        nombre = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(nombre);
                                        break;
                                    case FacetaMayuscula.MayusculasPrimeraPalabra:
                                        nombre = UtilCadenas.ConvertirPrimeraLetraDeFraseAMayúsculas(nombre);
                                        break;
                                    case FacetaMayuscula.MayusculasTodasLetras:
                                        nombre = UtilCadenas.ConvertirAMayúsculas(nombre);
                                        break;
                                }

                                string Enlace = "";

                                bool pintarEnlace = false;
                                if (!mEsBot)
                                {
                                    #region Enlace
                                    //Hay que pintar enlace
                                    if (dicNombreAbreviatura.Count != 0)
                                    {

                                        if (propiedadAPintar.Contains("#" + i) || (propiedades.Length == 1 && propiedadAPintar.Contains("#")))
                                        {
                                            pintarEnlace = true;
                                        }

                                        Enlace = pBaseURLbusqueda;

                                        string[] valorProps = valorProp.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

                                        Enlace += "?";
                                        int cont = 0;
                                        foreach (string prop in valorProps)
                                        {
                                            if (cont > 0)
                                            {
                                                Enlace += "@@@";
                                            }

                                            if (prop.Contains("#"))
                                            {
                                                if (dicNombreAbreviatura.ContainsKey(prop.Substring(0, prop.IndexOf("#") + 1)))
                                                {
                                                    Enlace += dicNombreAbreviatura[prop.Substring(0, prop.IndexOf("#") + 1)][0] + ":" + prop.Substring(prop.IndexOf("#") + 1);
                                                }
                                                else
                                                {
                                                    Enlace += prop;
                                                }
                                            }
                                            else
                                            {
                                                if (dicNombreAbreviatura.ContainsKey(prop.Substring(0, prop.LastIndexOf("/") + 1)))
                                                {
                                                    Enlace += dicNombreAbreviatura[prop.Substring(0, prop.LastIndexOf("/") + 1)][0] + ":" + prop.Substring(prop.LastIndexOf("/") + 1);
                                                }
                                                else
                                                {
                                                    Enlace += prop;
                                                }
                                            }

                                            cont++;
                                        }
                                        Enlace += "=" + filtro + idioma;

                                    }
                                    #endregion
                                }

                                string fechaOriginal = "";
                                if (transformacionFecha)
                                {
                                    fechaOriginal = nombre;
                                    nombre = nombre.Substring(6, 2) + "/" + nombre.Substring(4, 2) + "/" + nombre.Substring(0, 4);
                                }
                                else if (facetaBooleana)
                                {
                                    if (nombre.ToLower() == "true")
                                    {
                                        nombre = mUtilIdiomas.GetText("COMMON", "SI");
                                    }
                                    else if (nombre.ToLower() == "false")
                                    {
                                        nombre = mUtilIdiomas.GetText("COMMON", "NO");
                                    }
                                }

                                if (pintarEnlace && Enlace != "")
                                {

                                    if (!objeto.Contains(nombre))
                                    {
                                        objeto += "<a href=\"" + Enlace + "\">" + nombre + "</a>, ";
                                    }
                                }
                                else
                                {
                                    if (!objeto.Contains(nombre))
                                    {
                                        objeto += nombre + ", ";
                                    }
                                }

                                if (transformacionFecha)
                                {
                                    nombre = fechaOriginal;
                                }

                                if (propsMulti.Length > 1)
                                {
                                    propiedadAux = propMulti.Replace("[MultiIdioma]", "");
                                    if (pFichaRecurso.ViewSettings.SemanticProperties == null)
                                    {
                                        pFichaRecurso.ViewSettings.SemanticProperties = new Dictionary<string, List<SemanticPropertieModel>>();
                                    }
                                    if (pFichaRecurso.ViewSettings.SemanticProperties.ContainsKey(propiedadAux))
                                    {
                                        if (indice > 0 && pFichaRecurso.ViewSettings.SemanticProperties.ContainsKey(propiedadAux) && pFichaRecurso.ViewSettings.SemanticProperties[propiedadAux].Count > contPropActual)
                                        {
                                            pFichaRecurso.ViewSettings.SemanticProperties[propiedadAux][contPropActual].Name += "||" + nombre;
                                            pFichaRecurso.ViewSettings.SemanticProperties[propiedadAux][contPropActual].Url += "||" + Enlace;
                                        }
                                        else
                                        {
                                            SemanticPropertieModel propiedad = new SemanticPropertieModel();
                                            propiedad.Name = nombre;
                                            propiedad.Url = Enlace;
                                            pFichaRecurso.ViewSettings.SemanticProperties[propiedadAux].Add(propiedad);
                                        }
                                    }
                                    else if (!pFichaRecurso.ViewSettings.SemanticProperties.ContainsKey(propiedadAux))
                                    {
                                        List<SemanticPropertieModel> lista = new List<SemanticPropertieModel>();
                                        SemanticPropertieModel propiedad = new SemanticPropertieModel();
                                        propiedad.Name = nombre;
                                        propiedad.Url = Enlace;
                                        lista.Add(propiedad);
                                        pFichaRecurso.ViewSettings.SemanticProperties.Add(propiedadAux, lista);
                                    }
                                }
                                else
                                {
                                    if (pFichaRecurso.ViewSettings.SemanticProperties == null)
                                    {
                                        pFichaRecurso.ViewSettings.SemanticProperties = new Dictionary<string, List<SemanticPropertieModel>>();
                                    }
                                    if (pFichaRecurso.ViewSettings.SemanticProperties.ContainsKey(propiedadAux))
                                    {
                                        SemanticPropertieModel valorPropiedadAnterior = pFichaRecurso.ViewSettings.SemanticProperties[propiedadAux].Find(propiedad => propiedad.Name == nombre && propiedad.Url == Enlace);
                                        if (valorPropiedadAnterior == null)
                                        {
                                            SemanticPropertieModel valorPropiedad = new SemanticPropertieModel();
                                            valorPropiedad.Name = nombre;
                                            valorPropiedad.Url = Enlace;
                                            pFichaRecurso.ViewSettings.SemanticProperties[propiedadAux].Add(valorPropiedad);
                                        }
                                    }
                                    else if (!pFichaRecurso.ViewSettings.SemanticProperties.ContainsKey(propiedadAux))
                                    {
                                        List<SemanticPropertieModel> lista = new List<SemanticPropertieModel>();
                                        SemanticPropertieModel valorPropiedad = new SemanticPropertieModel();
                                        valorPropiedad.Name = nombre;
                                        valorPropiedad.Url = Enlace;
                                        lista.Add(valorPropiedad);
                                        pFichaRecurso.ViewSettings.SemanticProperties.Add(propiedadAux, lista);
                                    }
                                }
                                contPropActual++;
                            }
                        }
                        if (objeto != "")
                        {
                            objeto = objeto.Substring(0, objeto.Length - 2);
                        }


                        if (propiedades.Length == 1)
                        {
                            if (propiedadAPintar.Contains("#"))
                            {
                                propiedadAPintar = Regex.Replace(propiedadAPintar, "#\\d?", objeto);
                            }
                            else
                            {
                                propiedadAPintar = Regex.Replace(propiedadAPintar, "@\\d?", objeto);
                            }
                            pintar = true;

                        }
                        else
                        {
                            if (propiedadAPintar.Contains("#"))
                            {
                                propiedadAPintar = propiedadAPintar.Replace($"#{i.ToString()}", objeto);
                            }
                            else
                            {
                                propiedadAPintar = propiedadAPintar.Replace("#", "@");
                            }

                            propiedadAPintar = propiedadAPintar.Replace($"@{i.ToString()}", objeto);
                            pintar = true;
                        }
                    }
                    else
                    {
                        string[] trozos = propiedadAPintar.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                        if (trozos.Length > 1)
                        {
                            foreach (string trozo in trozos)
                            {
                                if (trozo.Contains("#" + i.ToString()) || trozo.Contains("@" + i.ToString()))
                                {
                                    propiedadAPintar = propiedadAPintar.Replace(trozo, "");
                                }
                            }
                        }
                        else
                        {
                            propiedadAPintar = propiedadAPintar.Replace("#" + i.ToString(), "");
                            propiedadAPintar = propiedadAPintar.Replace("@" + i.ToString(), "");
                        }
                    }
                    i--;
                }
            }

            propiedadAPintar = propiedadAPintar.Replace("||", "");

            if (pintar)
            {
                return propiedadAPintar;
            }

            return null;
            #endregion
        }

        private string SustituirPrefijos(string pPropiedad, Dictionary<string, List<string>> pDicNombreAbreviatura)
        {
            string valorProp = pPropiedad;
            List<string> prefijos = new List<string>();
            string[] delimiters = new string[] { "@@@" };
            string[] props = pPropiedad.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (string prop in props)
            {
                if (!prop.StartsWith("http:") && prop.Contains(":"))
                {
                    prefijos.Add(prop.Substring(0, prop.IndexOf(":")));
                }
            }
            if (prefijos.Count > 0)
            {
                foreach (string Nombre in pDicNombreAbreviatura.Keys)
                {
                    foreach (string ns in pDicNombreAbreviatura[Nombre])
                    {
                        if (prefijos.Contains(ns))
                        {
                            valorProp = valorProp.Replace("@" + ns + ":", "@" + Nombre);
                            if (valorProp.StartsWith(ns + ":"))
                            {
                                valorProp = Nombre + valorProp.Substring(ns.Length + 1);
                            }
                        }
                    }
                }
            }
            return valorProp;
        }

        /// <summary>
        /// Obtiene los modelos de los recursos del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaRecursosID">Identificadores de los recursos</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <param name="pIdentidadActual">IdentidadActual (null = usuario invitado)</param>
        /// <returns></returns>
        public Dictionary<Guid, ResourceModel> ObtenerRecursosPorIDSinProcesarIdioma(List<Guid> pListaRecursosID, string pUrlBaseUrlBusqueda)
        {
            return ObtenerRecursosPorIDSinProcesarIdioma(pListaRecursosID, pUrlBaseUrlBusqueda, new KeyValuePair<Guid?, bool>(null, false));
        }

        /// <summary>
        /// Obtiene los modelos de los recursos del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaRecursosID">Identificadores de los recursos</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <param name="pIdentidadActual">IdentidadActual (null = usuario invitado)</param>
        /// <param name="pBaseRecursosPersonalID">ID de la base de recurso personal en caso de que estemos en uno, NULL para comunidades o MyGnoss y un booleano que indica si la BR es de organización o no</param>
        /// <returns></returns>
        public Dictionary<Guid, ResourceModel> ObtenerRecursosPorIDSinProcesarIdioma(List<Guid> pListaRecursosID, string pUrlBaseUrlBusqueda, KeyValuePair<Guid?, bool> pBaseRecursosPersonalID, Guid? pProyectoID = null, bool pEsFichaRecurso = false)
        {
            //Procesamos los modelos para su presentación
            Dictionary<Guid, ResourceModel> listaRecursos = ObtenerRecursosPorIDInt(pListaRecursosID, pUrlBaseUrlBusqueda, pBaseRecursosPersonalID, true, false, pProyectoID, pEsFichaRecurso: pEsFichaRecurso);
            Dictionary<Guid, ResourceModel> listaRecursosDevolver = new Dictionary<Guid, ResourceModel>();
            foreach (Guid id in listaRecursos.Keys)
            {
                ResourceModel ficha = listaRecursos[id];
                ficha.UrlSearch = pUrlBaseUrlBusqueda.TrimEnd('/');
                if (ficha.Categories == null)
                {
                    ficha.Categories = new List<CategoryModel>();
                }

                listaRecursosDevolver.Add(id, ficha);
            }
            return listaRecursosDevolver;
        }

        /// <summary>
        /// Obtiene los modelos de los recursos del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaRecursosID">Identificadores de los recursos</param>
        /// <param name="pUrlBaseUrlBusqueda">BaseUrlBusqueda</param>
        /// <param name="pIdentidadActual">IdentidadActual (null = usuario invitado)</param>
        /// <param name="pBaseRecursosPersonalID">ID de la base de recurso personal en caso de que estemos en uno, NULL para comunidades o MyGnoss y un booleano que indica si la BR es de organización o no</param>
        /// <returns></returns>
        private Dictionary<Guid, ResourceModel> ObtenerRecursosPorIDInt(List<Guid> pListaRecursosID, string pUrlBaseUrlBusqueda, KeyValuePair<Guid?, bool> pBaseRecursosPersonalID, bool pObtenerIdentidades = true, bool pObtenerDatosExtraIdentidades = false, Guid? pProyectoID = null, bool pObtenerUltimaVersion = false, bool pEsFichaRecurso = false)
        {
            Dictionary<Guid, Guid> listaRecursosABuscar = new Dictionary<Guid, Guid>();

            if (pObtenerUltimaVersion)
            {
                foreach (Guid recursoID in pListaRecursosID)
                {
                    DataWrapperDocumentacion dwDoc = mDocumentacionCN.ObtenerVersionesDocumentoPorID(recursoID);
                    Guid ultimaVersionID = dwDoc.ListaVersionDocumento.OrderByDescending(doc => doc.Version).FirstOrDefault()?.DocumentoID ?? recursoID;
                    if (!listaRecursosABuscar.ContainsKey(ultimaVersionID))
                    {
                        listaRecursosABuscar.Add(ultimaVersionID, recursoID);
                    }
                }
            }
            else
            {
                foreach (Guid recursoID in pListaRecursosID)
                {
                    if (!listaRecursosABuscar.ContainsKey(recursoID))
                    {
                        listaRecursosABuscar.Add(recursoID, recursoID);
                    }
                }
            }

            //Obtiene los modelos de caché
            DocumentacionCL documentacionCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            Guid proyectoID = mProyecto.Clave;
            Guid? proyConexion = mConfigService.ObtenerProyectoConexion();
            if ((proyConexion != null && !proyConexion.Equals(Guid.Empty)) && proyConexion.Equals(mProyecto.Clave) && pProyectoID != null && pProyectoID.Equals(ProyectoAD.MetaProyecto))
            {
                proyectoID = pProyectoID.Value;
            }
            Dictionary<Guid, ResourceModel> listaRecursos = documentacionCL.ObtenerFichasRecursoMVC(listaRecursosABuscar.Keys.ToList(), proyectoID);
            documentacionCL.Dispose();

            List<Guid> listaRecursosPendientes = new List<Guid>();
            foreach (Guid idRecurso in listaRecursos.Keys)
            {
                if (listaRecursos[idRecurso] == null || listaRecursos[idRecurso].UrlPreview == null)
                {
                    listaRecursosPendientes.Add(idRecurso);
                }
            }

            if (mProyectoOrigenID != Guid.Empty)
            {
                proyectoID = mProyectoOrigenID;
            }

            //Obtiene de BBDD los recursos que no estaban caché y los procesa para almacenarlos en caché
            List<ObtenerRecursoMVC> listaRecursosMVC = mMVCCN.ObtenerRecursosPorID(listaRecursosPendientes, proyectoID);
            #region Leemos el recurso

            Dictionary<Guid, Tuple<int?, string, Guid?, string>> docImagenNomCatElemID = new Dictionary<Guid, Tuple<int?, string, Guid?, string>>();
            Dictionary<Guid, ResourceModel> listaRecursosBBDD = new Dictionary<Guid, ResourceModel>();
            if (listaRecursosMVC != null)
            {

                foreach (ObtenerRecursoMVC recursoMVC in listaRecursosMVC)
                {
                    Guid idRecruso = recursoMVC.DocumentoID;
                    if (!listaRecursosBBDD.ContainsKey(idRecruso))
                    {
                        string Titulo = HttpUtility.HtmlDecode(recursoMVC.Titulo);
                        string Descripcion = null;

                        if (recursoMVC.Descripcion != null)
                        {
                            Descripcion = recursoMVC.Descripcion;
                        }

                        TiposDocumentacion tipoDocumento = (TiposDocumentacion)recursoMVC.Tipo;

                        string Enlace = null;
                        if (recursoMVC.Enlace != null)
                        {
                            Enlace = recursoMVC.Enlace;
                        }

                        string Tags = "";
                        if (recursoMVC.Tags != null)
                        {
                            Tags = recursoMVC.Tags;
                        }

                        Guid idIdentidadPublicador = recursoMVC.IdentidadPublicacionID.Value;
                        TipoPublicacion tipoPublicacion = (TipoPublicacion)recursoMVC.TipoPublicacion;
                        DateTime fechaPublicacion = recursoMVC.FechaPublicacion.Value;
                        int? VersionFoto = null;
                        if (recursoMVC.VersionFotoDocumento.HasValue)
                        {
                            VersionFoto = recursoMVC.VersionFotoDocumento.Value;
                            if (VersionFoto < 0)
                            {
                                VersionFoto = null;
                            }
                        }
                        string nombreCategoriaDoc = "";
                        if (recursoMVC.NombreCategoriaDoc != null)
                        {
                            nombreCategoriaDoc = recursoMVC.NombreCategoriaDoc;
                        }
                        Guid? elementoVinculadoID = null;
                        if (recursoMVC.ElementoVinculadoID.HasValue)
                        {
                            elementoVinculadoID = recursoMVC.ElementoVinculadoID.Value;
                        }

                        string rdfType = "";
                        if (recursoMVC.DocOntolologiaEnlace != null)
                        {
                            rdfType = recursoMVC.DocOntolologiaEnlace.Replace(".owl", "");
                            //rdfType = recursoMVC.Enlace.Replace(".owl", "");
                        }

                        bool privado = recursoMVC.PrivadoEditores;

                        int numeroComentarios = recursoMVC.NumeroComentarios;
                        int numeroVisitas = recursoMVC.NumeroConsultas;
                        int numeroVotos = recursoMVC.NumeroVotos;

                        bool compartirPermitido = recursoMVC.CompartirPermitido;
                        bool borrador = recursoMVC.Borrador;
                        bool ultimaVersion = recursoMVC.UltimaVersion;
                        Guid docProyectoID = recursoMVC.ProyectoID.Value;
                        int numeroDescargas = recursoMVC.NumeroDescargas;

                        string nombreEntidadVinculada = "";
                        if (recursoMVC.NombreElementoVinculado != null)
                        {
                            nombreEntidadVinculada = recursoMVC.NombreElementoVinculado;
                        }

                        ResourceModel recurso = new ResourceModel();
                        recurso.Key = idRecruso;
                        recurso.Title = Titulo;
                        recurso.Description = Descripcion;
                        recurso.TypeDocument = (ResourceModel.DocumentType)tipoDocumento;
                        recurso.RdfType = rdfType;
                        recurso.Private = privado;
                        recurso.NumComments = numeroComentarios;
                        recurso.NumVisits = numeroVisitas;
                        recurso.NumVotes = numeroVotos;
                        recurso.NumDownloads = numeroDescargas;
                        recurso.Link = Enlace;
                        recurso.EstadoID = recursoMVC.EstadoID;

                        recurso.AllowShare = compartirPermitido;
                        recurso.IsDraft = borrador;
                        recurso.IsLastVersion = ultimaVersion;
                        recurso.ProjectID = docProyectoID;

                        if (elementoVinculadoID.HasValue)
                        {
                            recurso.ItemLinked = elementoVinculadoID.Value;
                        }

                        #region NombreImagen

                        string nombreImagen = "digital";
                        switch (recurso.TypeDocument)
                        {
                            case ResourceModel.DocumentType.DafoProyecto:
                                nombreImagen = "dafo";
                                break;
                            case ResourceModel.DocumentType.Debate:
                                nombreImagen = "debate";
                                break;
                            case ResourceModel.DocumentType.Encuesta:
                                nombreImagen = "encuesta";
                                break;
                            case ResourceModel.DocumentType.EntradaBlog:
                            case ResourceModel.DocumentType.EntradaBlogTemporal:
                            case ResourceModel.DocumentType.Blog:
                                nombreImagen = "blog";
                                break;
                            case ResourceModel.DocumentType.Hipervinculo:
                                nombreImagen = "hipervinculo";
                                break;
                            case ResourceModel.DocumentType.Imagen:
                                nombreImagen = "imagen";
                                break;
                            case ResourceModel.DocumentType.Newsletter:
                                nombreImagen = "newsletter";
                                break;
                            case ResourceModel.DocumentType.Nota:
                                nombreImagen = "digital";
                                break;
                            case ResourceModel.DocumentType.Pregunta:
                                nombreImagen = "question";
                                break;
                            case ResourceModel.DocumentType.Video:
                            case ResourceModel.DocumentType.VideoBrightcove:
                            case ResourceModel.DocumentType.VideoTOP:
                                nombreImagen = "video";
                                break;
                            case ResourceModel.DocumentType.Wiki:
                            case ResourceModel.DocumentType.ImagenWiki:
                            case ResourceModel.DocumentType.WikiTemporal:
                                nombreImagen = "wiki";
                                break;
                            case ResourceModel.DocumentType.AudioBrightcove:
                            case ResourceModel.DocumentType.AudioTOP:
                                nombreImagen = "audio";
                                break;
                        }

                        string extension = "";
                        if (EsVideoIncrustado(recurso.TypeDocument, Enlace))
                        {
                            extension = "flv";
                        }
                        else if (EsPresentacionIncrustada(Enlace))
                        {
                            extension = "ppt";
                        }
                        else if (EsAudioIncrustado(recurso.TypeDocument))
                        {
                            extension = "mp3";
                        }
                        else if (recurso.TypeDocument == ResourceModel.DocumentType.FicheroServidor || recurso.TypeDocument == ResourceModel.DocumentType.Video)
                        {
                            string nombreDocumento = Enlace;
                            if (nombreDocumento.Contains("."))
                            {
                                extension = nombreDocumento.Substring(nombreDocumento.LastIndexOf(".") + 1);
                            }
                        }
                        else if (recurso.TypeDocument == ResourceModel.DocumentType.Imagen)
                        {
                            extension = ".jpg";
                            if (nombreEntidadVinculada == "Wiki2")
                            {
                                // Enlace por alguna razón llega vacío y provoca un error
                                if (!String.IsNullOrEmpty(Enlace))
                                {
                                    extension = Enlace.Substring(Enlace.LastIndexOf('.'));
                                }
                            }
                        }

                        if (nombreImagen == "digital" && recurso.TypeDocument != ResourceModel.DocumentType.ReferenciaADoc && recurso.TypeDocument != ResourceModel.DocumentType.Nota && recurso.TypeDocument != ResourceModel.DocumentType.Newsletter)
                        {
                            switch (extension)
                            {
                                case "mpg":
                                case "mpge":
                                case "avi":
                                case "wma":
                                case "wmp":
                                case "flv":
                                    nombreImagen = "video";
                                    break;
                                case "wav":
                                case "mp2":
                                case "mp3":
                                    nombreImagen = "audio";
                                    break;
                                case "doc":
                                case "docx":
                                case "docm":
                                case "dot":
                                case "dotx":
                                case "dotm":
                                case "txt":
                                case "rtf":
                                    nombreImagen = "documento";
                                    break;
                                case "ppt":
                                case "pptx":
                                case "pptm":
                                case "pps":
                                case "ppsx":
                                case "ppsm":
                                case "pot":
                                case "potx":
                                case "potm":
                                    nombreImagen = "presentacion";
                                    break;
                                case "xls":
                                case "xlsx":
                                case "xlsb":
                                case "xlsm":
                                case "xlt":
                                case "xltx":
                                case "xltm":
                                case "csv":
                                    nombreImagen = "xls";
                                    break;
                                case "pdf":
                                    nombreImagen = "pdf";
                                    break;
                                case "zip":
                                case "rar":
                                case "zip7":
                                    nombreImagen = "zip";
                                    break;
                                default:
                                    nombreImagen = extension;
                                    break;
                            }
                        }

                        recurso.NameImage = nombreImagen;
                        #endregion

                        #region Tags
                        recurso.Tags = new List<string>();
                        string[] etiquetas = Tags.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tag in etiquetas)
                        {
                            recurso.Tags.Add(tag.Trim());
                        }
                        #endregion

                        recurso.PublisherKey = idIdentidadPublicador;
                        recurso.TypePublication = (ResourceModel.PublicationType)tipoPublicacion;
                        recurso.PublishDate = fechaPublicacion;
                        recurso.FullyLoaded = false;
                        recurso.AllowVotes = mParametrosGeneralesRow.VotacionesDisponibles;

                        DateTime? fechaModificacion = recursoMVC.FechaModificacion;
                        if (fechaModificacion.HasValue && !fechaModificacion.Value.Equals(DateTime.MinValue))
                        {
                            recurso.ModificationDate = fechaModificacion.Value;
                        }
                        else
                        {
                            recurso.ModificationDate = fechaPublicacion;
                        }

                        listaRecursos[recurso.Key] = recurso;
                        listaRecursosBBDD.Add(recurso.Key, recurso);
                        docImagenNomCatElemID.Add(recurso.Key, new Tuple<int?, string, Guid?, string>(VersionFoto, nombreCategoriaDoc, elementoVinculadoID, extension));
                    }
                }


            }

            foreach (Guid idRecurso in listaRecursosBBDD.Keys)
            {
                listaRecursosBBDD[idRecurso].UrlPreview = ImagenRecurso((TiposDocumentacion)(listaRecursosBBDD[idRecurso].TypeDocument), idRecurso, docImagenNomCatElemID[idRecurso].Item1, docImagenNomCatElemID[idRecurso].Item2, docImagenNomCatElemID[idRecurso].Item3, docImagenNomCatElemID[idRecurso].Item4);

            }

            #endregion

            // Comprobamos si hay recursos vacios o si el usuario actual no tiene permisos para visualizar 
            // el recurso en caso de que su estado sea privado
            List<Guid> recursosVacios = new List<Guid>();
            foreach (Guid idRecurso in listaRecursos.Keys)
            {
                if (listaRecursos[idRecurso] == null)
                {
                    recursosVacios.Add(idRecurso);
                }
                else
                {
                    ResourceModel recursoModel = listaRecursos[idRecurso];
                    if (recursoModel.EstadoID.HasValue)
                    {
                        Guid estadoID = (Guid)recursoModel.EstadoID;
                        bool tienePermisos = true;

                        FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory);

                        // Se comprueba si la identidad actual tiene permisos de lectura
                        if (!flujosCN.ComprobarEstadoEsPublico((Guid)recursoModel.EstadoID))
                        {
                            recursoModel.Private = true;
                            // Comprobar si la identidad actual tiene permisos de lectura / edicion
                            tienePermisos = flujosCN.ComprobarIdentidadTienePermisoLecturaEnEstado(estadoID, mIdentidadActual.Clave, recursoModel.OriginalKey) || flujosCN.ComprobarIdentidadTienePermisoEdicionEnEstado(estadoID, mIdentidadActual.Clave, recursoModel.OriginalKey);
                        }
                        else
                        {
                            recursoModel.Private = false;
                        }

                        // Si el estado es privado y el usuario no tiene permisos se omite el resultado
                        if (!tienePermisos) recursosVacios.Add(idRecurso);
                    }
                }
            }

            foreach (Guid idRecurso in recursosVacios)
            {
                listaRecursos.Remove(idRecurso);
                listaRecursosBBDD.Remove(idRecurso);
            }

            if (listaRecursos.Count > 0)
            {
                //Obtiene de BBDD las categorías de los recursos que no estaban caché y las procesa para almacenarlas en caché
                List<ObtenerCategoriaDeRecursos> categorias = null;

                if (!pBaseRecursosPersonalID.Key.HasValue)
                {
                    categorias = new List<ObtenerCategoriaDeRecursos>();
                    //TODO SANTI CATEGORIA
                    Dictionary<Guid, List<Guid>> listaCategorias = mMVCCN.ObtenerDiccionarioCategoriaDeTesauroPorID(listaRecursosPendientes);
                    TesauroCL tesauroCL = new TesauroCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCL>(), mLoggerFactory);
                    GestionTesauro gestorTesauro = new GestionTesauro(tesauroCL.ObtenerTesauroDeProyecto(mProyecto.Clave), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);

                    foreach (Guid documentoID in listaCategorias.Keys)
                    {
                        foreach (Guid categoriaID in listaCategorias[documentoID])
                        {
                            if (gestorTesauro.ListaCategoriasTesauro.ContainsKey(categoriaID))
                            {
                                Elementos.Tesauro.CategoriaTesauro catTesauro = gestorTesauro.ListaCategoriasTesauro[categoriaID];

                                ObtenerCategoriaDeRecursos cat = new ObtenerCategoriaDeRecursos();
                                cat.CategoriaTesauroID = catTesauro.FilaCategoria.CategoriaTesauroID;
                                cat.CategoriaSuperiorID = catTesauro.FilaCategoria.CatTesauroAgCatTesauroInferior.FirstOrDefault()?.CategoriaSuperiorID;
                                //cat.CategoriaSuperiorID = catTesauro.FilaCategoria.CatTesauroAgCatTesauroSuperior.FirstOrDefault()?.CategoriaTesauro.TesauroID;
                                cat.DocumentoID = documentoID;
                                cat.Nombre = catTesauro.Nombre["es"];

                                categorias.Add(cat);
                            }

                        }
                    }

                    //categorias = mMVCCN.ObtenerCategoriasDeRecursosPorID(listaRecursosPendientes, mProyecto.Clave);

                }
                else
                {
                    categorias = mMVCCN.ObtenerCategoriasDeRecursosPorIDEspacioPersonal(new List<Guid>(listaRecursos.Keys), pBaseRecursosPersonalID.Key.Value, pBaseRecursosPersonalID.Value);
                }

                #region Leemos las categorias

                if (categorias != null)
                {
                    foreach (ObtenerCategoriaDeRecursos categoria in categorias)
                    {
                        Guid idRecurso = categoria.DocumentoID;
                        if (listaRecursos.ContainsKey(idRecurso))
                        {
                            Guid idCategoria = categoria.CategoriaTesauroID;
                            string nombreCategoria = categoria.Nombre;
                            Guid idCategoriaPadre = Guid.Empty;
                            if (categoria.CategoriaSuperiorID.HasValue && !categoria.CategoriaSuperiorID.Value.Equals(Guid.Empty))
                            {
                                idCategoriaPadre = categoria.CategoriaSuperiorID.Value;
                            }

                            CategoryModel categoriaTesauro = new CategoryModel();
                            categoriaTesauro.Key = idCategoria;
                            categoriaTesauro.Name = nombreCategoria;
                            categoriaTesauro.LanguageName = nombreCategoria;
                            categoriaTesauro.ParentCategoryKey = idCategoriaPadre;

                            if (listaRecursos[idRecurso].Categories == null)
                            {
                                listaRecursos[idRecurso].Categories = new List<CategoryModel>();
                            }
                            if (!listaRecursos[idRecurso].Categories.Contains(categoriaTesauro))
                            {
                                listaRecursos[idRecurso].Categories.Add(categoriaTesauro);
                            }
                        }
                    }
                }
            }

            #endregion

            if (listaRecursosBBDD.Count > 0 && !pBaseRecursosPersonalID.Key.HasValue)
            {
                //Si hay recursos obtenidos de BBDD los almacenamos en caché
                DocumentacionCL documentacionCL2 = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
                documentacionCL2.AgregarFichasRecursoMVC(listaRecursosBBDD, mProyecto.Clave);
                documentacionCL2.Dispose();
            }

            DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
            mDocumentacionCN.ObtenerVersionDocumentosPorIDs(dwDocumentacion, listaRecursos.Keys.ToList(), true);

            foreach (Guid idRecurso in listaRecursos.Keys)
            {
                bool sinVersiones = dwDocumentacion.ListaVersionDocumento.Where(doc => doc.DocumentoID.Equals(idRecurso) || doc.DocumentoOriginalID.Equals(idRecurso)).Count() == 0;
                listaRecursos[idRecurso].OriginalKey = !sinVersiones ? dwDocumentacion.ListaVersionDocumento.Where(doc => doc.DocumentoID.Equals(idRecurso) || doc.DocumentoOriginalID.Equals(idRecurso)).FirstOrDefault().DocumentoOriginalID : idRecurso;

				using (DocumentacionCN documentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory))
				{
					bool esMejora = documentacionCN.ComprobarSiDocumentoEsUnaMejora(idRecurso);
					if (esMejora && !pEsFichaRecurso)
					{
                        Guid ultimaVersion = documentacionCN.ObtenerUltimaVersionDeDocumento(listaRecursos[idRecurso].OriginalKey);
                        if (ultimaVersion.Equals(Guid.Empty))
                        {
                            ultimaVersion = listaRecursos[idRecurso].OriginalKey;
                        }
                        if (ultimaVersion != Guid.Empty)
                        {
							AD.EntityModel.Models.Documentacion.Documento doc = documentacionCN.ObtenerDocumentoPorIdentificador(ultimaVersion);
							listaRecursos[idRecurso].Title = doc.Titulo;
							listaRecursos[idRecurso].Description = doc.Descripcion;
						}                        
					}
				}
                
				string nombreCortoProy = "";
                if (!mProyecto.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    nombreCortoProy = mProyecto.NombreCorto;
                }

				if (mProyecto.Clave.Equals(ProyectoAD.MetaProyecto) && !listaRecursos[idRecurso].ProjectID.Equals(ProyectoAD.MetaProyecto))//&& !pBaseRecursosPersonalID.Key.HasValue
                {
                    listaRecursos[idRecurso].CompletCardLink = GnossUrlsSemanticas.GetURLBaseRecursosRecursoInvitadoConIDS(mBaseURLIdioma, UrlPerfil(mIdentidadActual), mUtilIdiomas, ObtenerNombreSemantico(listaRecursos[idRecurso].Title, mUtilIdiomas.LanguageCode), listaRecursos[idRecurso].Key);
                    listaRecursos[idRecurso].CompleteOriginalCardLink = GnossUrlsSemanticas.GetURLBaseRecursosRecursoInvitadoConIDS(mBaseURLIdioma, UrlPerfil(mIdentidadActual), mUtilIdiomas, ObtenerNombreSemantico(listaRecursos[idRecurso].Title, mUtilIdiomas.LanguageCode), listaRecursos[idRecurso].OriginalKey);
                }
                else
                {
                    listaRecursos[idRecurso].CompletCardLink = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, nombreCortoProy, UrlPerfil(mIdentidadActual), ObtenerNombreSemantico(listaRecursos[idRecurso].Title, mUtilIdiomas.LanguageCode), listaRecursos[idRecurso].Key, listaRecursos[idRecurso].ItemLinked, (TiposDocumentacion)(short)listaRecursos[idRecurso].TypeDocument, (pBaseRecursosPersonalID.Key.HasValue && pBaseRecursosPersonalID.Value));

                    listaRecursos[idRecurso].CompleteOriginalCardLink = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, nombreCortoProy, UrlPerfil(mIdentidadActual), ObtenerNombreSemantico(listaRecursos[idRecurso].Title, mUtilIdiomas.LanguageCode), listaRecursos[idRecurso].OriginalKey, listaRecursos[idRecurso].ItemLinked, (TiposDocumentacion)(short)listaRecursos[idRecurso].TypeDocument, (pBaseRecursosPersonalID.Key.HasValue && pBaseRecursosPersonalID.Value));

                    listaRecursos[idRecurso].EditCardLink = UrlsSemanticas.GetURLBaseRecursosEditarDocumento(mBaseURLIdioma, mUtilIdiomas, nombreCortoProy, UrlPerfil(mIdentidadActual), ObtenerNombreSemantico(listaRecursos[idRecurso].Title, mUtilIdiomas.LanguageCode), 1, (pBaseRecursosPersonalID.Key.HasValue && pBaseRecursosPersonalID.Value), listaRecursos[idRecurso].Key);
                }

                listaRecursos[idRecurso].UrlPreview = listaRecursos[idRecurso].UrlPreview.Replace("BASEURLCONTENTREPLACE", BaseURLContent);

                listaRecursos[idRecurso].VersionCardLink = !sinVersiones ? $"{listaRecursos[idRecurso].CompleteOriginalCardLink}/{listaRecursos[idRecurso].Key}" : listaRecursos[idRecurso].CompletCardLink;

                Guid? versionIDMejoraActiva = dwDocumentacion.ListaVersionDocumento.FirstOrDefault(doc => doc.EsMejora && doc.EstadoVersion == (short)EstadoVersion.Pendiente && doc.DocumentoOriginalID.Equals(listaRecursos[idRecurso].OriginalKey))?.DocumentoID;

                listaRecursos[idRecurso].ImprovementCardLink = versionIDMejoraActiva.HasValue? $"{listaRecursos[idRecurso].CompleteOriginalCardLink}/{versionIDMejoraActiva}" : listaRecursos[idRecurso].CompletCardLink;

                listaRecursos[idRecurso].ListActions = new ResourceModel.UrlActions();
                EstablecerUrlAccionesReecurso(listaRecursos[idRecurso], mProyecto.NombreCorto);

                listaRecursos[idRecurso].Actions = new ResourceModel.ActionsModel();
            }
            

            if (mIdentidadActual != null && !mIdentidadActual.Clave.Equals(UsuarioAD.Invitado))
            {
                //Calculamos si los recursos están votados(+/-) o no por el usuario
                Dictionary<Guid, double> dicEstaVotadoDocumento = mDocumentacionCN.ObtenerVotoRecurso(listaRecursos.Keys.ToList(), mIdentidadActual.Clave);

                if (dicEstaVotadoDocumento != null)
                {
                    foreach (Guid docID in listaRecursos.Keys)
                    {
                        listaRecursos[docID].Votes = new VotesModel();
                        if (dicEstaVotadoDocumento.ContainsKey(docID))
                        {
                            if (dicEstaVotadoDocumento[docID] == -1)
                            {
                                listaRecursos[docID].Votes.IsVotedNegative = true;
                            }
                            else if (dicEstaVotadoDocumento[docID] == 1)
                            {
                                listaRecursos[docID].Votes.IsVotedPositive = true;
                            }
                        }



                        listaRecursos[docID].Votes.UrlVotePositive = listaRecursos[docID].CompletCardLink + "/vote-positive";
                        listaRecursos[docID].Votes.UrlVoteNegative = listaRecursos[docID].CompletCardLink + "/vote-negative";
                        listaRecursos[docID].Votes.UrlDeleteVote = listaRecursos[docID].CompletCardLink + "/delete-vote";
                    }
                }

                List<Guid> recursosCompartidosEnBRUsuario = new List<Guid>();
                if (!mIdentidadActual.Clave.Equals(UsuarioAD.Invitado))
                {
                    if (mIdentidadActual.Persona != null)
                    {
                        recursosCompartidosEnBRUsuario = mDocumentacionCN.ObtenerRecursosCompartidosEnBRUsuario(listaRecursos.Keys.ToList(), mIdentidadActual.Persona.UsuarioID);
                    }
                    else if (mIdentidadActual.OrganizacionID.HasValue)
                    {
                        recursosCompartidosEnBRUsuario = mDocumentacionCN.ObtenerRecursosCompartidosEnBROrganizacion(listaRecursos.Keys.ToList(), mIdentidadActual.OrganizacionID.Value);
                    }
                }

                foreach (Guid docID in listaRecursos.Keys)
                {
                    listaRecursos[docID].Actions.AddToMyPersonalSpace = false;

                    listaRecursos[docID].IsInMyPersonalSpace = recursosCompartidosEnBRUsuario.Contains(docID);

                    if (!recursosCompartidosEnBRUsuario.Contains(docID) && mParametrosGeneralesRow != null && mParametrosGeneralesRow.CompartirRecursosPermitido && listaRecursos[docID].AllowShare && !listaRecursos[docID].IsDraft && listaRecursos[docID].IsLastVersion)
                    {
                        listaRecursos[docID].Actions.AddToMyPersonalSpace = true;

                    }
                }
            }

            if (pObtenerIdentidades)
            {
                //Obtiene los publicadores de los recursos y los agregamos a la ficha de los recursos
                #region Obtenemos los publicadores

                List<Guid> listaIdentidades = new List<Guid>();
                foreach (Guid id in listaRecursos.Keys)
                {
                    if (!listaIdentidades.Contains(listaRecursos[id].PublisherKey))
                    {
                        listaIdentidades.Add(listaRecursos[id].PublisherKey);
                    }
                }
                Dictionary<Guid, ProfileModel> identidadesPublicadores = ObtenerIdentidadesPorID(listaIdentidades, pObtenerDatosExtraIdentidades);
                foreach (Guid id in listaRecursos.Keys)
                {
                    listaRecursos[id].Publisher = identidadesPublicadores[listaRecursos[id].PublisherKey];
                    //Asignar isOwnedAuthor si el usuario actual es el publicador del recurso.
                    if (mIdentidadActual != null)
                    {
                        if (listaRecursos[id].Publisher.Key.ToString() == mIdentidadActual.Clave.ToString())
                        {
                            listaRecursos[id].Votes.IsOwnedAuthor = true;
                        }
                    }
                }

                #endregion
            }

            // Si estamos obteniendo la ultima version de los recursos una vez obtenido los datos de BD necesitamos devolver la lista de IDs originales
            // que ha pedido el servicio de resultados.
            if (pObtenerUltimaVersion)
            {
                Dictionary<Guid, ResourceModel> listaResultados = new Dictionary<Guid, ResourceModel>();
                foreach (var fila in listaRecursosABuscar)
                {
                    Guid idVersion = fila.Key;
                    Guid idOringal = fila.Value;
                    if (listaRecursos.ContainsKey(idVersion))
                    {
                        listaResultados.Add(idOringal, listaRecursos[idVersion]);
                    }
                }

                return listaResultados;
            }
            return listaRecursos;
        }

        /// <summary>
        /// Obtiene el ID de la base de recursos personal de una identidad.
        /// </summary>
        /// <param name="pEspacioPersonal">Indica si el recurso se está viendo desde una base de recursos personal</param>
        /// <returns>ID de la base de recursos personal de una identidad</returns>
        private KeyValuePair<Guid?, bool> ObtenerBaseRecursosPersonalID(EspacioPersonal pEspacioPersonal)
        {
            if (pEspacioPersonal == Controladores.EspacioPersonal.Organizacion && mIdentidadActual.Tipo != TiposIdentidad.Organizacion)
            {
                mIdentidadActual = mIdentidadActual.IdentidadOrganizacion;
            }

            Guid brID = Guid.Empty;
            bool org = false;
            DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

            if (mIdentidadActual.Tipo == TiposIdentidad.Organizacion)
            {
                brID = docCN.ObtenerBaseRecursosIDOrganizacion(mIdentidadActual.OrganizacionID.Value);
                org = true;
            }
            else
            {
                brID = docCN.ObtenerBaseRecursosIDUsuario(mIdentidadActual.Persona.UsuarioID);
            }

            docCN.Dispose();

            return new KeyValuePair<Guid?, bool>(brID, org);
        }

        /// <summary>
        /// Completa la carga de los modelos de recursos para el espacio personal.
        /// </summary>
        /// <param name="pListaRecursos">Lista recursos</param>
        /// <param name="pEspacioPersonal">Indica si el recurso se está viendo desde una base de recursos personal</param>
        private void CompletarCargaRecursosEspacioPersonal(Dictionary<Guid, ResourceModel> pListaRecursos, EspacioPersonal pEspacioPersonal)
        {
            Identidad identidadAux = mIdentidadActual;
            if (pEspacioPersonal == Controladores.EspacioPersonal.Organizacion && mIdentidadActual.Tipo != TiposIdentidad.Organizacion)
            {
                identidadAux = mIdentidadActual.IdentidadOrganizacion;
            }

            TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
            GestionTesauro gestorTesauro = null;
            Guid clavePublica = Guid.Empty;

            if (identidadAux.Tipo == TiposIdentidad.Organizacion)
            {
                gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion(mIdentidadActual.OrganizacionID.Value), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                clavePublica = gestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault().CategoriaTesauroPublicoID.Value;
            }
            else
            {
                gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuario(mIdentidadActual.Persona.UsuarioID), mLoggingService, mEntityContext, mLoggerFactory.CreateLogger<GestionTesauro>(), mLoggerFactory);
                clavePublica = (gestorTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault()).CategoriaTesauroPublicoID.Value;

            }

            tesauroCN.Dispose();

            foreach (ResourceModel recModel in pListaRecursos.Values)
            {
                recModel.Private = true;

                foreach (CategoryModel catModel in recModel.Categories)
                {
                    if (gestorTesauro.ListaCategoriasTesauro[catModel.Key].PadreNivelRaiz.Clave == clavePublica)
                    {
                        recModel.Private = false;
                        break;
                    }
                }
            }

            gestorTesauro.Dispose();
        }

        /// <summary>
        /// Establece las urls de la lista de acciones del un recurso
        /// </summary>
        /// <param name="pRecurso"></param>
        public void EstablecerUrlAccionesReecurso(ResourceModel pRecurso, string pNombreCortoComunidad)
        {
            pRecurso.ListActions = new ResourceModel.UrlActions();

            pRecurso.ListActions.UrlLoadGraph = ObtenerUrlRecursoParaPeticionServicioContextos(UrlServicioContextos, pRecurso.CompletCardLink, pRecurso.Key, pNombreCortoComunidad) + "/load-graph";

            if (!string.IsNullOrEmpty(IdiomaUsuario) && !string.IsNullOrEmpty(UrlServicioContextos))
            {
                pRecurso.ListActions.UrlLoadGraph += "/" + IdiomaUsuario;
            }

            pRecurso.ListActions.UrlLoadMoreEntitiesSelector = pRecurso.CompletCardLink + "/load-more-entities-selector";
            pRecurso.ListActions.UrlActionSemCms = pRecurso.CompletCardLink + "/actionsemcms";
            pRecurso.ListActions.UrlCallbackGraph = pRecurso.CompletCardLink + "/callback-graph";
            pRecurso.ListActions.UrlCreateComment = pRecurso.CompletCardLink + "/create-comment";
            pRecurso.ListActions.UrlLoadLinkedResources = pRecurso.CompletCardLink + "/load-linked-resources";
            pRecurso.ListActions.UrlLinkResource = pRecurso.CompletCardLink + "/link-resource";
            pRecurso.ListActions.UrlLinkResourceSP = pRecurso.CompletCardLink + "/link-resourceSP";
            pRecurso.ListActions.UrlUnlinkResourceSP = pRecurso.CompletCardLink + $"/{UtilIdiomas.GetText("URLSEM", "DESVINCULARSHAREPOINT")}";
            pRecurso.ListActions.UrlAddMetaTitle = pRecurso.CompletCardLink + "/add-metatitle";
            pRecurso.ListActions.UrlAddMetaDescripcion = pRecurso.CompletCardLink + "/add-metadescription";
            pRecurso.ListActions.UrlUnLinkResource = pRecurso.CompletCardLink + "/unlink-resource";
            pRecurso.ListActions.UrlAddToPersonalSpace = pRecurso.CompletCardLink + "/add-personal-space";
            pRecurso.ListActions.UrlAddToPersonalSpacePrivate = pRecurso.CompletCardLink + "/add-personal-space-private";
            pRecurso.ListActions.UrlAddACategoryToPersonalSpace = pRecurso.CompletCardLink + "/add-category-to-personal-space";
            pRecurso.ListActions.UrlAddTags = pRecurso.CompletCardLink + "/add-tags";
            pRecurso.ListActions.UrlAddCategories = pRecurso.CompletCardLink + "/add-categories";
            pRecurso.ListActions.UrlLockComments = pRecurso.CompletCardLink + "/lock-comments";
            pRecurso.ListActions.UrlUnlockComments = pRecurso.CompletCardLink + "/unlock-comments";
            pRecurso.ListActions.UrlRestoreVersion = pRecurso.VersionCardLink + "/restore-version";
            pRecurso.ListActions.UrlDeleteVersion = pRecurso.VersionCardLink + "/delete-version";
            pRecurso.ListActions.UrlStartImprovement = pRecurso.CompletCardLink + "/start-improvement";
            pRecurso.ListActions.UrlApplyImprovement = pRecurso.VersionCardLink + "/apply-improvement";
            pRecurso.ListActions.UrlCancelImprovement = pRecurso.VersionCardLink + "/cancel-improvement";
            pRecurso.ListActions.UrlReportPage = pRecurso.CompletCardLink + "/report-page";
            pRecurso.ListActions.UrlShare = pRecurso.CompletCardLink + "/share";
            pRecurso.ListActions.UrlDuplicate = "/" + "duplicate-resource" + "/" + pRecurso.Key;
            pRecurso.ListActions.UrlUnshare = pRecurso.CompletCardLink + "/unshare";
            pRecurso.ListActions.UrlDelete = pRecurso.CompletCardLink + "/delete";
            pRecurso.ListActions.UrlDeleteSelective = pRecurso.CompletCardLink + "/delete-selective";
            pRecurso.ListActions.UrlCertify = pRecurso.CompletCardLink + "/certify";
            pRecurso.ListActions.UrlSendNewsletter = pRecurso.CompletCardLink + "/send-newsletter";
            pRecurso.ListActions.UrlSendNewsletterGroups = pRecurso.CompletCardLink + "/send-newsletter-groups";

            pRecurso.ListActions.UrlLoadActionLinkResource = pRecurso.CompletCardLink + "/load-action/link-resource";
            pRecurso.ListActions.UrlLoadActionLinkResourceSP = pRecurso.CompletCardLink + "/load-action/link-resourceSP";
            pRecurso.ListActions.UrlLoadActionUnlinkResourceSP = pRecurso.CompletCardLink + "/load-action/unlink-resourceSP";
            pRecurso.ListActions.UrlLoadActionAddToPersonalSpace = pRecurso.CompletCardLink + "/load-action/add-personal-space";
            pRecurso.ListActions.UrlLoadActionAddTags = pRecurso.CompletCardLink + "/load-action/add-tags";
            pRecurso.ListActions.UrlLoadActionHistory = pRecurso.VersionCardLink + "/load-action/history";
            pRecurso.ListActions.UrlLoadActionAddCategories = pRecurso.CompletCardLink + "/load-action/add-categories";
            pRecurso.ListActions.UrlLoadActionRestoreVersion = pRecurso.CompletCardLink + "/load-action/restore-version";
            pRecurso.ListActions.UrlLoadActionDeleteVersion = pRecurso.CompletCardLink + "/load-action/delete-version";
            pRecurso.ListActions.UrlLoadActionReportPage = pRecurso.CompletCardLink + "/load-action/report-page";
            pRecurso.ListActions.UrlLoadActionDelete = pRecurso.CompletCardLink + "/load-action/delete";
            pRecurso.ListActions.UrlLoadActionDeleteSelective = pRecurso.CompletCardLink + "/load-action/delete-selective";
            pRecurso.ListActions.UrlLoadActionCertify = pRecurso.CompletCardLink + "/load-action/certify";
            pRecurso.ListActions.UrlLoadActionShare = pRecurso.CompletCardLink + "/load-action/share";
            pRecurso.ListActions.UrlLoadActionShareChange = pRecurso.CompletCardLink + "/load-action/share-change";
            pRecurso.ListActions.UrlLoadActionDuplicate = pRecurso.CompletCardLink + "/load-action/duplicate";
            pRecurso.ListActions.UrlLoadActionSendNewsletter = pRecurso.CompletCardLink + "/load-action/send-newsletter";
            pRecurso.ListActions.UrlLoadActionSendNewsletterGroups = pRecurso.CompletCardLink + "/load-action/send-newsletter-groups";
            pRecurso.ListActions.UrlLoadActionLockComments = pRecurso.CompletCardLink + "/load-action/lock-unlock-comments";
            pRecurso.ListActions.UrlLoadActionSendLink = pRecurso.CompletCardLink + "/load-action/send-link";
            pRecurso.ListActions.UrlLoadActionAddMetaTitle = pRecurso.CompletCardLink + "/load-action/add-metatitle";
            pRecurso.ListActions.UrlLoadActionAddMetaDescripcion = pRecurso.CompletCardLink + "/load-action/add-metadescription";
            pRecurso.ListActions.UrlTransitionModal = pRecurso.CompletCardLink + "/load-action/transition";
            pRecurso.ListActions.UrlTransition = pRecurso.CompletCardLink + "/transition-state";
            pRecurso.ListActions.UrlTransitionHistory = pRecurso.CompletCardLink + "/load-action/transition-history";
			pRecurso.ListActions.UrlLoadActionStartImprovement = pRecurso.CompletCardLink + "/load-action/load-modal-start-improvement";
            pRecurso.ListActions.UrlLoadActionApplyImprovement = pRecurso.CompletCardLink + "/load-action/load-modal-apply-improvement";
            pRecurso.ListActions.UrlLoadActionCancelImprovement = pRecurso.CompletCardLink + "/load-action/load-modal-cancel-improvement";
			pRecurso.ListActions.UrlImprovement = pRecurso.ImprovementCardLink;
        }

        public Dictionary<Guid, List<ResourceEventModel>> ObtenerEventosDeRecursosPorID(List<Guid> pListaRecursosID)
        {
            DocumentacionCL documentacionCL = new DocumentacionCL("acid", "recursos", mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);
            Dictionary<Guid, List<ResourceEventModel>> listaEventos = documentacionCL.ObtenerEventosRecursoMVC(pListaRecursosID, mProyecto.Clave);
            documentacionCL.Dispose();

            List<Guid> listaEventosPendientes = new List<Guid>();
            foreach (Guid idRecurso in listaEventos.Keys)
            {
                if (listaEventos[idRecurso] == null)
                {
                    listaEventosPendientes.Add(idRecurso);
                }
            }

            Dictionary<Guid, List<ResourceEventModel>> listaEventosBBDD = new Dictionary<Guid, List<ResourceEventModel>>();
            //Obtiene de BBDD los recursos que no estaban caché y los procesa para almacenarlos en caché
            List<ObtenerRecursosEvento> listaRecursosEvento = mMVCCN.ObtenerEventosRecursosPorID(listaEventosPendientes, mProyecto.Clave);

            Dictionary<Guid, List<Guid>> listaComentariosDocumento = new Dictionary<Guid, List<Guid>>();
            List<Guid> listaComentarios = new List<Guid>();

            if (listaRecursosEvento != null)
            {
                foreach (ObtenerRecursosEvento recurso in listaRecursosEvento)
                {
                    Guid idRecruso = recurso.DocumentoID;
                    int tipoEvento = recurso.TipoEvento;
                    DateTime fechaEvento = recurso.FechaEvento.Value;
                    string Evento = recurso.Evento;

                    ResourceEventModel eventoRecurso = new ResourceEventModel();

                    switch (tipoEvento)
                    {
                        case 0:
                            break;
                        case 1:
                            eventoRecurso = new ResourceEventCertifyModel();
                            ((ResourceEventCertifyModel)eventoRecurso).Description = Evento;
                            break;
                        case 2:
                            eventoRecurso = new ResourceEventCommentModel();
                            Guid comentarioID = new Guid(Evento);
                            ((ResourceEventCommentModel)eventoRecurso).CommentKey = comentarioID;
                            if (!listaComentariosDocumento.ContainsKey(idRecruso))
                            {
                                listaComentariosDocumento.Add(idRecruso, new List<Guid>());
                            }
                            listaComentariosDocumento[idRecruso].Add(comentarioID);
                            listaComentarios.Add(comentarioID);
                            break;
                    }

                    eventoRecurso.Type = (ResourceEventModel.EventType)tipoEvento;
                    eventoRecurso.Date = fechaEvento;

                    if (listaEventos[idRecruso] == null)
                    {
                        listaEventos[idRecruso] = new List<ResourceEventModel>();
                    }

                    if (listaEventos[idRecruso].Count < 3)
                    {
                        listaEventos[idRecruso].Add(eventoRecurso);
                    }

                    if (!listaEventosBBDD.ContainsKey(idRecruso))
                    {
                        listaEventosBBDD.Add(idRecruso, new List<ResourceEventModel>());
                    }

                    if (listaEventosBBDD[idRecruso].Count < 3)
                    {
                        listaEventosBBDD[idRecruso].Add(eventoRecurso);
                    }
                }
            }

            Dictionary<Guid, CommentModel> listaComentariosModel = ObtenerComentariosDeRecursosPorID(listaComentarios);

            foreach (Guid docConCommentario in listaComentariosDocumento.Keys)
            {
                foreach (ResourceEventModel evento in listaEventosBBDD[docConCommentario])
                {
                    if (evento.Type == ResourceEventModel.EventType.Commented)
                    {
                        Guid comentarioid = ((ResourceEventCommentModel)evento).CommentKey;
                        ((ResourceEventCommentModel)evento).Comment = listaComentariosModel[comentarioid];
                    }
                }
            }

            return listaEventos;
        }

        /// <summary>
        /// Obtiene los modelos de una lista de comentarios
        /// </summary>
        /// <param name="pListaComentarios">Indetificadores de los comentarios</param>
        /// <returns></returns>
        public Dictionary<Guid, CommentModel> ObtenerComentariosDeRecursosPorID(List<Guid> pListaComentarios)
        {
            //Obtiene de BBDD los grupos que no estaban caché y los procesa para almacenarlos en caché
            List<AD.EntityModel.Models.Comentario.Comentario> comentarios = mMVCCN.ObtenerComentariosDeRecursosPorID(pListaComentarios);
            #region Leemos las identidades

            Dictionary<Guid, CommentModel> listaComentarios = new Dictionary<Guid, CommentModel>();

            Dictionary<Guid, Guid> listaComentarioPublicador = new Dictionary<Guid, Guid>();

            if (comentarios != null)
            {
                foreach (AD.EntityModel.Models.Comentario.Comentario comentario in comentarios)
                {
                    Guid clave = comentario.ComentarioID;
                    Guid publicadorID = comentario.IdentidadID;
                    DateTime fecha = comentario.Fecha;
                    string descripcion = comentario.Descripcion;

                    listaComentarioPublicador.Add(clave, publicadorID);
                    CommentModel fichaComentario = new CommentModel();
                    fichaComentario.Key = clave;
                    fichaComentario.Title = descripcion;
                    fichaComentario.PublishDate = fecha;

                    listaComentarios[clave] = fichaComentario;
                }
            }

            #endregion

            #region Obtenemos los remitentes y destinatarios

            List<Guid> listaIdentidades = new List<Guid>();
            foreach (Guid id in listaComentarioPublicador.Values)
            {
                if (!listaIdentidades.Contains(id))
                {
                    listaIdentidades.Add(id);
                }
            }
            Dictionary<Guid, ProfileModel> identidadesPublicadores = ObtenerIdentidadesPorID(listaIdentidades, false);
            foreach (Guid id in listaComentarioPublicador.Keys)
            {
                listaComentarios[id].PublisherCard = identidadesPublicadores[listaComentarioPublicador[id]];
            }

            #endregion

            return listaComentarios;
        }

        /// <summary>
        /// Obtiene los modelos de las identidades del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaIdentidades">Indetificadores de las identidades</param>
        /// <param name="pObtenerDatosExtraIdentidades">Especifica si se deben obtener los datos extra de las identidades</param>
        /// <returns></returns>
        public ProfileModel ObtenerIdentidadPorID(Guid pIdentidadID, bool pObtenerDatosExtraIdentidades = false)
        {
            List<Guid> listaIDs = new List<Guid>();
            listaIDs.Add(pIdentidadID);

            Dictionary<Guid, ProfileModel> listaResultados = ObtenerIdentidadesPorID(listaIDs, pObtenerDatosExtraIdentidades);

            if (listaResultados.ContainsKey(pIdentidadID))
            {
                return listaResultados[pIdentidadID];
            }

            return null;
        }



        /// <summary>
        /// Obtiene los modelos de las identidades del proyecto en función del usuario
        /// </summary>
        /// <param name="pListaUsuarios">Indetificadores de los usuarios</param>
        /// <param name="pObtenerDatosExtraIdentidades">Especifica si se deben obtener los datos extra de las identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, ProfileModel> ObtenerIdentidadesPorIDUsuarios(List<Guid> pListaUsuarios, bool pObtenerDatosExtraIdentidades = false)
        {
            return ObtenerIdentidadesPorID(mIdentidadCN.ObtenerIdentidadesIDDeusuariosEnProyecto(mProyecto.Clave, pListaUsuarios), pObtenerDatosExtraIdentidades);
        }

        /// <summary>
        /// Obtiene los modelos de las identidades del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaIdentidades">Indetificadores de las identidades</param>
        /// <param name="pObtenerDatosExtraIdentidades">Especifica si se deben obtener los datos extra de las identidades</param>
        /// <returns></returns>
        public Dictionary<Guid, ProfileModel> ObtenerIdentidadesPorID(List<Guid> pListaIdentidades, bool pObtenerDatosExtraIdentidades = false)
        {
            //Obtiene los modelos de caché
            Dictionary<Guid, ProfileModel> listaIdentidades = mIdentidadCL.ObtenerFichasIdentiadesMVC(pListaIdentidades);

            List<Guid> listaIdentidadesPendientes = new List<Guid>();
            foreach (Guid idIdentidad in listaIdentidades.Keys)
            {
                if (listaIdentidades[idIdentidad] == null)
                {
                    listaIdentidadesPendientes.Add(idIdentidad);
                }
            }

            //Obtiene de BBDD las identidades que no estaban caché y los procesa para almacenarlos en caché
            List<AD.EntityModel.Models.IdentidadDS.IdentidadMVC> listaIdentidadMVC = mMVCCN.ObtenerIdentidadesPorID(listaIdentidadesPendientes);
            #region Leemos las identidades

            Dictionary<Guid, ProfileModel> listaIdentidadesBBDD = new Dictionary<Guid, ProfileModel>();
            if (listaIdentidadMVC != null)
            {
                foreach (AD.EntityModel.Models.IdentidadDS.IdentidadMVC identidadMVC in listaIdentidadMVC)
                {
                    Guid clave = identidadMVC.IdentidadID;
                    ProfileType tipoIdentidad = (ProfileType)identidadMVC.Tipo;

                    string nombreCortoOrg = identidadMVC.NombreCortoOrg;
                    string nombreOrg = identidadMVC.NombreOrganizacion;
                    string nombreCortoUsu = identidadMVC.NombreCortoUsu;
                    string nombrePerfil = identidadMVC.NombrePerfil;

                    Guid? organizacionID = null;
                    if (identidadMVC.OrganizacionID.HasValue)
                    {
                        organizacionID = identidadMVC.OrganizacionID.Value;
                    }
                    Guid? personaID = null;
                    if (identidadMVC.PersonaID.HasValue)
                    {
                        personaID = identidadMVC.PersonaID.Value;
                    }

                    string urlFoto = identidadMVC.Foto;

                    string tags = identidadMVC.Tags;

                    int numConexiones = numConexiones = identidadMVC.NumConnexiones.Value;

                    DateTime fechaNacimiento = DateTime.MinValue;
                    if (identidadMVC.FechaNacimiento.HasValue)
                    {
                        fechaNacimiento = identidadMVC.FechaNacimiento.Value;
                    }
                    bool expulsado = false;
                    if (identidadMVC.FechaExpulsion.HasValue)
                    {
                        expulsado = true;
                    }
                    bool bloqueado = false;
                    if (identidadMVC.FechaBaja.HasValue)
                    {
                        bloqueado = true;
                    }

                    ProfileModel fichaIdentidad = new ProfileModel();
                    fichaIdentidad.Key = clave;
                    fichaIdentidad.TypeProfile = tipoIdentidad;
                    fichaIdentidad.ConnectionCounter = numConexiones;
                    fichaIdentidad.BornDate = fechaNacimiento;
                    fichaIdentidad.isExpelled = expulsado;
                    fichaIdentidad.isBlocked = bloqueado;
                    fichaIdentidad.isSubscribedToNewsletter = identidadMVC.RecibirNewsLetter;
                    fichaIdentidad.TieneEmailTutor = identidadMVC.TieneEmailTutor;
                    fichaIdentidad.Rol = UserRol.User;
                    if (!tipoIdentidad.Equals(TiposIdentidad.Organizacion) && identidadMVC.PersonaID.HasValue)
                    {
                        var filaAdmin = ProyectoSeleccionado.FilaProyecto.AdministradorProyecto.FirstOrDefault(f => f.UsuarioID.Equals(identidadMVC.UsuarioID));
                        if (filaAdmin != null)
                        {
                            fichaIdentidad.Rol = (UserRol)filaAdmin.Tipo;
                        }
                    }

                    //fichaIdentidad.Tags = new List<string>();
                    //fichaIdentidad.Tags.AddRange(tags.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries));

                    if (organizacionID.HasValue)
                    {
                        fichaIdentidad.KeyOrganization = organizacionID.Value;
                    }

                    if (personaID.HasValue)
                    {
                        fichaIdentidad.KeyPerson = personaID.Value;
                    }

                    string linkOrg = "";

                    if (!string.IsNullOrEmpty(urlFoto) && !urlFoto.Equals("sinfoto"))
                    {
                        fichaIdentidad.UrlFoto = "/" + UtilArchivos.ContentImagenes + urlFoto;
                    }

                    if (fichaIdentidad.TypeProfile == ProfileType.ProfessionalPersonal || fichaIdentidad.TypeProfile == ProfileType.ProfessionalCorporate || fichaIdentidad.TypeProfile == ProfileType.Organization)
                    {
                        linkOrg = "URLORGANIZACIONREPLACE" + "/" + nombreCortoOrg;

                        fichaIdentidad.NameOrganization = nombreOrg;
                        fichaIdentidad.UrlOrganization = linkOrg;
                        //fichaIdentidad.NamePerson = nombrePerfil;
                    }
                    if (fichaIdentidad.TypeProfile == ProfileType.Personal || fichaIdentidad.TypeProfile == ProfileType.Teacher || fichaIdentidad.TypeProfile == ProfileType.ProfessionalPersonal)
                    {
                        fichaIdentidad.NamePerson = nombrePerfil;

                        if (personaID.HasValue)
                        {
                            PersonaCN personaCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PersonaCN>(), mLoggerFactory);
                            AD.EntityModel.Models.PersonaDS.Persona persona = personaCN.ObtenerFilaPersonaPorID(personaID.Value);

                            if (persona != null)
                            {
                                if (!string.IsNullOrEmpty(persona.Apellidos))
                                {
                                    fichaIdentidad.NamePerson = $"{persona.Nombre} {persona.Apellidos}";
                                }
                                else
                                {
                                    fichaIdentidad.NamePerson = $"{persona.Nombre}";
                                }
                            }
                        }

                        string linkPers = "URLPERSONAREPLACE" + "/" + nombreCortoUsu;

                        if (fichaIdentidad.TypeProfile == ProfileType.ProfessionalPersonal)
                        {
                            fichaIdentidad.UrlPerson = linkOrg + "/" + linkPers;
                        }
                        else
                        {
                            fichaIdentidad.UrlPerson = linkPers;
                        }
                    }

                    listaIdentidades[fichaIdentidad.Key] = fichaIdentidad;
                    listaIdentidadesBBDD.Add(fichaIdentidad.Key, fichaIdentidad);
                }


            }


            #endregion

            List<Guid> identidadesVacias = new List<Guid>();
            foreach (Guid idIdentidad in listaIdentidades.Keys)
            {
                if (listaIdentidades[idIdentidad] == null)
                {
                    identidadesVacias.Add(idIdentidad);
                }
            }

            foreach (Guid idIdentidad in identidadesVacias)
            {
                listaIdentidades.Remove(idIdentidad);
                listaIdentidadesBBDD.Remove(idIdentidad);
            }

            if (listaIdentidadesBBDD.Count > 0)
            {
                //Si hay identidades obtenidas de BBDD los almacenamos en caché
                mIdentidadCL.GuardarFichasIdentidadesMVC(listaIdentidadesBBDD);
            }

            string urlBaseEnlaces = BaseURLIdioma + UrlPerfil(mIdentidadActual).TrimEnd('/');
            if (mProyecto.Clave != ProyectoAD.MetaProyecto)
            {
                string urlComunidad = UrlsSemanticas.ObtenerURLComunidad(mUtilIdiomas, BaseURLIdioma, mProyecto.NombreCorto);
                urlBaseEnlaces = urlComunidad;
            }

            List<Guid> listaIdentSeguidas = null;
            if (mIdentidadActual != null && !mIdentidadActual.Clave.Equals(UsuarioAD.Invitado))
            {
                SuscripcionCN suscripcionCN = new SuscripcionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SuscripcionCN>(), mLoggerFactory);
                List<Guid> listaIdentidadesSuscritasPerfil = suscripcionCN.ComprobarListaIdentidadesSuscritasPerfil(mIdentidadActual.PerfilID, listaIdentidades.Keys.ToList());
                listaIdentSeguidas = listaIdentidadesSuscritasPerfil;
            }

            Dictionary<Guid, ProfileModel> listaIdentidadesDevolver = new Dictionary<Guid, ProfileModel>();
            foreach (Guid id in listaIdentidades.Keys)
            {
                ProfileModel ficha = listaIdentidades[id];

                if (!string.IsNullOrEmpty(ficha.UrlOrganization) && !Uri.IsWellFormedUriString(ficha.UrlOrganization, UriKind.Absolute))
                {
                    ficha.UrlOrganization = urlBaseEnlaces + "/" + ficha.UrlOrganization.Replace("URLORGANIZACIONREPLACE", mUtilIdiomas.GetText("URLSEM", "ORGANIZACION"));
                }
                if (!string.IsNullOrEmpty(ficha.UrlPerson) && !Uri.IsWellFormedUriString(ficha.UrlPerson, UriKind.Absolute))
                {
                    ficha.UrlPerson = urlBaseEnlaces + "/" + ficha.UrlPerson.Replace("URLORGANIZACIONREPLACE", mUtilIdiomas.GetText("URLSEM", "ORGANIZACION")).Replace("URLPERSONAREPLACE", mUtilIdiomas.GetText("URLSEM", "PERSONA"));
                }

                if (IdentidadActual == null || !IdentidadActual.Clave.Equals(id))
                {
                    ficha.ListActions = new ProfileModel.UrlActions();
                    ficha.ListActions.UrlFollow = ficha.UrlPerson + "/follow";
                    ficha.ListActions.UrlUnfollow = ficha.UrlPerson + "/unfollow";
                }

                if (listaIdentSeguidas != null)
                {
                    ficha.Actions = new ProfileModel.ActionsModel();
                    if (listaIdentSeguidas.Contains(id))
                    {
                        ficha.Actions.FollowingProfile = true;
                    }
                    else
                    {
                        ficha.Actions.FollowingProfile = false;
                    }
                }

                if (ficha.TypeProfile == ProfileType.ProfessionalCorporate || ficha.TypeProfile == ProfileType.Organization)
                {
                    if (ficha.ListActions == null)
                    {
                        ficha.ListActions = new ProfileModel.UrlActions();
                    }
                    ficha.ListActions.UrlFollow = ficha.UrlOrganization + "/follow";
                    ficha.ListActions.UrlUnfollow = ficha.UrlOrganization + "/unfollow";

                    if (mIdentidadActual == null || !mIdentidadActual.OrganizacionID.HasValue || (mIdentidadActual.OrganizacionID.HasValue && ficha.KeyOrganization != mIdentidadActual.OrganizacionID))
                    {
                        ficha.NamePerson = "";
                    }
                }

                ficha.Roles = new List<string>();
                if (mProyecto != null && mProyecto.Clave.Equals(ProyectoAD.MetaProyecto))
                {
                    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                    UsuarioCN usuCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<UsuarioCN>(), mLoggerFactory);
                    Guid usuarioID = usuCN.ObtenerGuidUsuarioIDporIdentidadID(id);
                    List<RolEcosistema> rolesUsuario = proyCN.ObtenerRolesAdministracionEcosistemaDeUsuario(usuarioID);
                    if (rolesUsuario != null && rolesUsuario.Count > 0)
                    {
                        foreach (RolEcosistema rol in rolesUsuario)
                        {
                            ficha.Roles.Add(UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, mUtilIdiomas.LanguageCode, null));
                        }
                        ficha.Roles = ficha.Roles.Order().ToList();
                    }

                    proyCN.Dispose();
                    usuCN.Dispose();
                }
                else
                {
                    //IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);					
                    //List<Rol> rolesIdentidad = identidadCN.ObtenerRolesDeIdentidad(id);
                    UtilPermisos utilPermisos = new UtilPermisos(mEntityContext, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<UtilPermisos>(), mLoggerFactory);
                    List<Rol> rolesIdentidad = utilPermisos.ObtenerRolesIdentidad(id, id);

                    if (rolesIdentidad != null && rolesIdentidad.Count > 0)
                    {
                        foreach (Rol rol in rolesIdentidad)
                        {
                            ficha.Roles.Add(UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, mUtilIdiomas.LanguageCode, null));
                        }
                        ficha.Roles = ficha.Roles.Order().ToList();
                    }
                    //identidadCN.Dispose();
                }




                listaIdentidadesDevolver.Add(id, ficha);
            }

            if (pObtenerDatosExtraIdentidades)
            {
                ObtenerInfoExtraIdentidadesPorID(listaIdentidadesDevolver);
            }

            return listaIdentidadesDevolver;
        }

        public void ObtenerInfoExtraIdentidadesPorID(Dictionary<Guid, ProfileModel> pListaModelosIdentidad)
        {
            List<Guid> listaIdentidadesID = new List<Guid>();
            foreach (Guid identidadID in pListaModelosIdentidad.Keys)
            {
                listaIdentidadesID.Add(identidadID);
            }

            //Obtiene los modelos de caché
            Dictionary<Guid, ProfileModel.ExtraInfoProfileModel> listaIdentidades = mIdentidadCL.ObtenerInfoExtraFichasIdentiadesMVC(listaIdentidadesID);

            List<Guid> listaIdentidadesPendientes = new List<Guid>();
            foreach (Guid idIdentidad in listaIdentidades.Keys)
            {
                if (listaIdentidades[idIdentidad] == null)
                {
                    listaIdentidadesPendientes.Add(idIdentidad);

                    pListaModelosIdentidad[idIdentidad].ExtraInfo = new ProfileModel.ExtraInfoProfileModel();
                    pListaModelosIdentidad[idIdentidad].ExtraInfo.ExtraData = new Dictionary<string, string>();
                    pListaModelosIdentidad[idIdentidad].ExtraInfo.IdentityResourceCounter = new ProfileModel.IdentityResourceCounter();
                    pListaModelosIdentidad[idIdentidad].ExtraInfo.IdentityResourceCounter.ResourceTypeCounter = new List<Tuple<ResourceModel.DocumentType, string, int, int, int>>();
                }
                else
                {
                    pListaModelosIdentidad[idIdentidad].ExtraInfo = listaIdentidades[idIdentidad];
                }
            }

            if (listaIdentidadesPendientes.Count > 0)
            {
                //Obtiene de BBDD los datosextra de las identidades que no estaban caché y los procesa para almacenarlos en caché
                List<DatoExtraFichasdentidad> listaDatosExtra = null;
                try
                {
                    listaDatosExtra = mMVCCN.ObtenerDatosExtraFichasIdentidadesPorIdentidadesID(listaIdentidadesPendientes);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, " 20160209 Error mal controlado Identidades.", mlogger);
                }

                if (listaDatosExtra != null)
                {
                    foreach (DatoExtraFichasdentidad datoExtra in listaDatosExtra)
                    {
                        Guid clave = datoExtra.IdentidadID;
                        string titulo = datoExtra.Titulo;
                        string opcion = datoExtra.Opcion;

                        pListaModelosIdentidad[clave].ExtraInfo.ExtraData.Add(titulo, opcion);
                    }
                }

                //Obtiene de BBDD los contadores de las identidades que no estaban caché y los procesa para almacenarlos en caché
                List<AD.EntityModel.Models.IdentidadDS.IdentidadContadoresRecursos> listaIdentidadContadoresRecursos = mMVCCN.ObtenerContadoresRecursosIdentiadesPorID(listaIdentidadesPendientes);
                if (listaIdentidadContadoresRecursos != null)
                {
                    foreach (AD.EntityModel.Models.IdentidadDS.IdentidadContadoresRecursos identidadContadorRecursos in listaIdentidadContadoresRecursos)
                    {
                        Guid clave = identidadContadorRecursos.IdentidadID;
                        ResourceModel.DocumentType tipo = (ResourceModel.DocumentType)identidadContadorRecursos.Tipo;
                        string nombresem = identidadContadorRecursos.NombreSem;
                        int publicados = identidadContadorRecursos.Publicados;
                        int compartidos = identidadContadorRecursos.Compartidos;
                        int comentarios = identidadContadorRecursos.Comentarios;

                        pListaModelosIdentidad[clave].ExtraInfo.IdentityResourceCounter.ResourceTypeCounter.Add(new Tuple<ResourceModel.DocumentType, string, int, int, int>(tipo, nombresem, publicados, compartidos, comentarios));
                    }
                }

                FacetadoCN facetadoCN = new FacetadoCN(UrlIntragnoss, mProyecto.Clave.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);

                FacetadoDS facetadoDS = facetadoCN.ObtieneInformacionPersonas(mProyecto.Clave, listaIdentidadesPendientes);

                foreach (Guid idIdentidad in listaIdentidadesPendientes)
                {
                    DataRow[] filasInfoExtra = facetadoDS.Tables["DatosPersonas"].Select("s='http://gnoss/" + idIdentidad + "'");

                    if (filasInfoExtra.Length > 0)
                    {
                        bool paisCargado = false;
                        bool provinciaCargada = false;

                        foreach (DataRow filaInfo in filasInfoExtra)
                        {
                            //pListaModelosIdentidad[idIdentidad].ExtraInfo.

                            if (!filaInfo.IsNull("countryname") && !string.IsNullOrEmpty(filaInfo["countryname"].ToString()) && !paisCargado)
                            {
                                string pais = filaInfo["countryname"].ToString();
                                pListaModelosIdentidad[idIdentidad].ExtraInfo.Country = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(pais);
                                paisCargado = true;
                            }

                            //Cargo el nombre de la provincia o estado
                            if (!filaInfo.IsNull("ProvinceOrState") && !string.IsNullOrEmpty(filaInfo["ProvinceOrState"].ToString()) && !provinciaCargada)
                            {
                                string provincia = filaInfo["ProvinceOrState"].ToString();
                                pListaModelosIdentidad[idIdentidad].ExtraInfo.ProvinceOrState = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(provincia);
                                provinciaCargada = true;
                            }

                            //Cargo el nombre de la localidad
                            if (!filaInfo.IsNull("Locality") && !string.IsNullOrEmpty(filaInfo["Locality"].ToString()) && !provinciaCargada)
                            {
                                string localidad = filaInfo["Locality"].ToString();
                                pListaModelosIdentidad[idIdentidad].ExtraInfo.Locality = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculas(localidad);
                                provinciaCargada = true;
                            }

                            ////Cargo la Descripción del CV
                            if (!filaInfo.IsNull("ExecutiveSummary") && !string.IsNullOrEmpty(filaInfo["ExecutiveSummary"].ToString()))
                            {
                                pListaModelosIdentidad[idIdentidad].ExtraInfo.Description = filaInfo["ExecutiveSummary"].ToString();
                            }

                            //Cargo los Puestos Actuales
                            if (!filaInfo.IsNull("PositionTitleEmpresaActual") && !string.IsNullOrEmpty(filaInfo["PositionTitleEmpresaActual"].ToString()))
                            {
                                if (pListaModelosIdentidad[idIdentidad].ExtraInfo.Puestos == null)
                                {
                                    pListaModelosIdentidad[idIdentidad].ExtraInfo.Puestos = new List<string>();
                                }

                                string puesto = filaInfo["PositionTitleEmpresaActual"].ToString();
                                if (!pListaModelosIdentidad[idIdentidad].ExtraInfo.Puestos.Contains(puesto))
                                {
                                    pListaModelosIdentidad[idIdentidad].ExtraInfo.Puestos.Add(puesto);
                                }
                            }

                            ////Cargo los Puestos Actuales
                            //if (!filaInfo.IsNull("OrganizationNameEmpresaActual") && !string.IsNullOrEmpty(filaInfo["OrganizationNameEmpresaActual"].ToString()))
                            //{
                            //    empresaActual = filaInfo["OrganizationNameEmpresaActual"].ToString();
                            //}

                            ////Cargo los Tags de el CV de esta identidad
                            if (!filaInfo.IsNull("Tag") && !string.IsNullOrEmpty(filaInfo["Tag"].ToString()))
                            {
                                if (pListaModelosIdentidad[idIdentidad].ExtraInfo.Tags == null)
                                {
                                    pListaModelosIdentidad[idIdentidad].ExtraInfo.Tags = new List<string>();
                                }

                                string tag = filaInfo["Tag"].ToString();
                                if (!pListaModelosIdentidad[idIdentidad].ExtraInfo.Tags.Contains(tag))
                                {
                                    pListaModelosIdentidad[idIdentidad].ExtraInfo.Tags.Add(tag);
                                }
                            }
                        }
                    }
                }

                Dictionary<Guid, ProfileModel.ExtraInfoProfileModel> listaGuardar = new Dictionary<Guid, ProfileModel.ExtraInfoProfileModel>();

                foreach (Guid idIdentidad in listaIdentidadesPendientes)
                {
                    listaGuardar.Add(idIdentidad, pListaModelosIdentidad[idIdentidad].ExtraInfo);
                }

                if (listaGuardar.Count > 0)
                {
                    //Si hay identidades obtenidas de BBDD los almacenamos en caché
                    mIdentidadCL.GuardarInfoExtraFichasIdentidadesMVC(listaGuardar);
                }
            }
        }

        /// <summary>
        /// Obtiene los modelos de los grupos del proyecto en función de la identidad
        /// </summary>
        /// <param name="pListaGrupos">Indetificadores de los grupos</param>
        /// <returns></returns>
        public Dictionary<Guid, GroupCardModel> ObtenerGruposPorID(List<Guid> pListaGrupos)
        {
            //Obtiene los modelos de caché
            IdentidadCL identidadCL = new IdentidadCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCL>(), mLoggerFactory);
            Dictionary<Guid, GroupCardModel> listaGrupos = identidadCL.ObtenerFichasGruposMVC(pListaGrupos);

            List<Guid> listaGruposPendientes = new List<Guid>();
            foreach (Guid idGrupo in listaGrupos.Keys)
            {
                if (listaGrupos[idGrupo] == null || string.IsNullOrEmpty(listaGrupos[idGrupo].ProyectShortName))
                {
                    listaGruposPendientes.Add(idGrupo);
                }
            }

            //Obtiene de BBDD los grupos que no estaban caché y los procesa para almacenarlos en caché
            MVCCN MVCCN = new MVCCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<MVCCN>(), mLoggerFactory);
            List<GrupoIdentidadesPorId> listaGrupoIdentidades = MVCCN.ObtenerGruposPorID(listaGruposPendientes);
            #region Leemos las identidades

            Dictionary<Guid, GroupCardModel> listaGruposBBDD = new Dictionary<Guid, GroupCardModel>();
            if (listaGrupoIdentidades != null)
            {
                foreach (GrupoIdentidadesPorId grupoIdentidad in listaGrupoIdentidades)
                {
                    Guid clave = grupoIdentidad.GrupoID;
                    string nombreGrupo = grupoIdentidad.Nombre;
                    string nombreCortoGrupo = grupoIdentidad.NombreCorto;

                    Guid? proyectoID = null;
                    string nombreCortoProy = "";

                    if (grupoIdentidad.ProyectoID.HasValue)
                    {
                        proyectoID = grupoIdentidad.ProyectoID;
                        nombreCortoProy = grupoIdentidad.ProyectoNombreCorto;
                    }

                    Guid? organizacionID = null;
                    if (grupoIdentidad.OrganizacionID.HasValue)
                    {
                        organizacionID = grupoIdentidad.OrganizacionID;
                    }

                    GroupCardModel fichaGrupo = new GroupCardModel();
                    fichaGrupo.Clave = clave;
                    fichaGrupo.Name = nombreGrupo;

                    //string linkGrupo = "";

                    if (proyectoID.HasValue)
                    {
                        fichaGrupo.ProyectKey = proyectoID.Value;
                        fichaGrupo.ProyectShortName = nombreCortoProy;

                        fichaGrupo.GroupType = GroupCardModel.GroupTypes.Community;
                        fichaGrupo.CompleteName = nombreGrupo + " " + ConstantesDeSeparacion.SEPARACION_CONCATENADOR + " " + nombreCortoProy;
                    }
                    else if (organizacionID.HasValue)
                    {
                        fichaGrupo.GroupType = GroupCardModel.GroupTypes.Organization;
                    }

                    fichaGrupo.ShortName = nombreCortoGrupo;

                    listaGrupos[fichaGrupo.Clave] = fichaGrupo;
                    listaGruposBBDD.Add(fichaGrupo.Clave, fichaGrupo);
                }

            }


            #endregion

            List<Guid> listaGruposVacios = new List<Guid>();
            foreach (Guid idGrupo in listaGrupos.Keys)
            {
                if (listaGrupos[idGrupo] == null)
                {
                    listaGruposVacios.Add(idGrupo);
                }
            }

            foreach (Guid idIdentidad in listaGruposVacios)
            {
                listaGrupos.Remove(idIdentidad);
                listaGruposBBDD.Remove(idIdentidad);
            }

            if (listaGruposBBDD.Count > 0)
            {
                //Si hay identidades obtenidas de BBDD los almacenamos en caché
                mIdentidadCL.GuardarFichasGruposMVC(listaGruposBBDD);
            }

            #region Obtenemos propiedades que no se van a guardar en cache

            List<string> listaNombresCortosComunidad = new List<string>();
            foreach (Guid idGrupo in listaGrupos.Keys)
            {
                if (listaGrupos[idGrupo].GroupType == GroupCardModel.GroupTypes.Community)
                {
                    if (!listaNombresCortosComunidad.Contains(listaGrupos[idGrupo].ProyectShortName))
                    {
                        listaNombresCortosComunidad.Add(listaGrupos[idGrupo].ProyectShortName);
                    }
                }
            }

            if (listaNombresCortosComunidad.Count > 0)
            {
                Dictionary<string, string> listaUrlsComunidades = UrlsSemanticas.ObtenerURLComunidades(mUtilIdiomas, BaseURLIdioma, listaNombresCortosComunidad);

                foreach (Guid idGrupo in listaGrupos.Keys)
                {
                    if (listaGrupos[idGrupo].GroupType == GroupCardModel.GroupTypes.Community)
                    {
                        listaGrupos[idGrupo].UrlGroup = listaUrlsComunidades[listaGrupos[idGrupo].ProyectShortName] + "/" + mUtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + listaGrupos[idGrupo].ShortName;
                    }
                }
            }

            foreach (Guid idGrupo in listaGrupos.Keys)
            {
                if (listaGrupos[idGrupo].GroupType == GroupCardModel.GroupTypes.Organization)
                {
                    listaGrupos[idGrupo].UrlGroup = UrlPerfil(mIdentidadActual) + mUtilIdiomas.GetText("URLSEM", "ADMINISTRACION") + "/" + mUtilIdiomas.GetText("URLSEM", "GRUPO") + "/" + listaGrupos[idGrupo].ShortName;
                }

                listaGrupos[idGrupo].Roles = new List<string>();
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                List<Rol> rolesGrupo = proyectoCN.ObtenerRolesDeGrupo(idGrupo);
                listaGrupos[idGrupo].Roles = new List<string>();

                if (rolesGrupo != null && rolesGrupo.Count > 0)
                {
                    foreach (Rol rol in rolesGrupo)
                    {
                        listaGrupos[idGrupo].Roles.Add(UtilCadenas.ObtenerTextoDeIdioma(rol.Nombre, mUtilIdiomas.LanguageCode, null));
                    }
                    listaGrupos[idGrupo].Roles = listaGrupos[idGrupo].Roles.Order().ToList();
                }
                proyectoCN.Dispose();
            }

            #endregion

            return listaGrupos;
        }

        /// <summary>
        /// Obtiene los modelos de un mensaje
        /// </summary>
        /// <param name="pMensajeID">Indetificadores del mensaje</param>
        /// <param name="pIdentidad">Identidad propietaria del mensaje</param>
        /// <param name="pParametrosBusqueda">Parámetros de la búsqueda</param>
        /// <returns></returns>
        public MessageModel ObtenerMensajePorID(Guid pMensajeID, string pParametrosBusqueda, Identidad pIdentidad)
        {
            List<Guid> listaMensajesID = new List<Guid>();
            listaMensajesID.Add(pMensajeID);
            Dictionary<Guid, MessageModel> listaMensajes = ObtenerMensajesPorID(listaMensajesID, pParametrosBusqueda, pIdentidad);
            if (listaMensajes.Count == 1)
            {
                return listaMensajes[pMensajeID];
            }
            return null;
        }

        /// <summary>
        /// Obtiene los modelos de una lista de mensajes
        /// </summary>
        /// <param name="pListaMensajes">Indetificadores de los mensajes</param>
        /// <param name="pIdentidadActual">IdentidadActual (null = usuario invitado)</param>
        /// <returns></returns>
        public Dictionary<Guid, MessageModel> ObtenerMensajesPorID(List<Guid> pListaMensajes, string pParametrosBusqueda, Identidad pIdentidad)
        {
            //Obtiene de BBDD los grupos que no estaban caché y los procesa para almacenarlos en caché
            IDataReader reader = mMVCCN.ObtenerMensajesPorID(pListaMensajes, pIdentidad.Clave, null);
            #region Leemos las identidades

            Dictionary<Guid, MessageModel> listaMensajes = new Dictionary<Guid, MessageModel>();
            if (reader != null)
            {
                try
                {
                    while (reader.Read())
                    {
                        Guid clave = reader.GetGuid(0);
                        Guid destinatario = reader.GetGuid(1);
                        Guid autor = reader.GetGuid(2);
                        string asunto = reader.GetString(3);
                        string cuerpo = reader.GetString(4);
                        DateTime fecha = reader.GetDateTime(5);
                        bool leido = reader.GetBoolean(6);
                        //bool eliminado = reader.GetBoolean(7);
                        bool papelera = reader.GetBoolean(8);
                        string destinatariosIDs = reader.GetString(9);
                        //string destinatariosNombres = reader.GetString(10);

                        //Guid? conversacion = null;
                        //if (!reader.IsDBNull(11))
                        //{
                        //    conversacion = reader.GetGuid(11);
                        //}

                        MessageModel fichaMensaje = new MessageModel();
                        fichaMensaje.Key = clave;
                        fichaMensaje.Subject = asunto;
                        //Microsoft.Security.Application.Sanitizer.GetSafeHtmlFragment(asunto)
                        fichaMensaje.Body = cuerpo;
                        fichaMensaje.ShippingDate = fecha;
                        fichaMensaje.Readed = leido;
                        fichaMensaje.SenderKey = autor;

                        fichaMensaje.Sent = destinatario == Guid.Empty;
                        fichaMensaje.Received = destinatario != Guid.Empty;
                        fichaMensaje.Deleted = papelera;
                        fichaMensaje.ReceiverKey = destinatario;
                        fichaMensaje.ReceiversKey = new List<Guid>();
                        fichaMensaje.ReceiversGroupKey = new List<Guid>();
                        //if (destinatario != Guid.Empty)
                        //{
                        //    fichaMensaje.ReceiversKey.Add(destinatario);
                        //}
                        if (!string.IsNullOrEmpty(destinatariosIDs))
                        {
                            string[] listaDestinatarios = destinatariosIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string dest in listaDestinatarios)
                            {
                                Guid destID = Guid.Empty;
                                bool esGrupo = false;
                                string destAux = dest;


                                if (dest.StartsWith("g_"))
                                {
                                    esGrupo = true;
                                    destAux = dest.Substring(2);
                                }

                                Guid.TryParse(destAux, out destID);
                                if (destID != Guid.Empty)
                                {
                                    if (esGrupo)
                                    {
                                        if (!fichaMensaje.ReceiversGroupKey.Contains(destID))
                                        {
                                            fichaMensaje.ReceiversGroupKey.Add(destID);
                                        }
                                    }
                                    else if (!fichaMensaje.ReceiversKey.Contains(destID))
                                    {
                                        fichaMensaje.ReceiversKey.Add(destID);
                                    }
                                }
                            }
                        }

                        listaMensajes[clave] = fichaMensaje;
                    }
                }
                finally
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            #endregion


            #region Obtenemos los remitentes y destinatarios

            string url = BaseURLIdioma + UrlPerfil(pIdentidad);

            if (pIdentidad.Tipo != TiposIdentidad.Organizacion)
            {
                url += mUtilIdiomas.GetText("URLSEM", "MENSAJES");
            }
            else
            {
                url += mUtilIdiomas.GetText("URLSEM", "MENSAJESORG");
            }

            List<Guid> listaIdentidades = new List<Guid>();
            List<Guid> listaGrupos = new List<Guid>();
            foreach (Guid id in listaMensajes.Keys)
            {
                listaMensajes[id].Url = url + "?mensaje=" + listaMensajes[id].Key + "&" + pParametrosBusqueda;

                if (!listaIdentidades.Contains(listaMensajes[id].SenderKey))
                {
                    listaIdentidades.Add(listaMensajes[id].SenderKey);
                }
                foreach (Guid receiverID in listaMensajes[id].ReceiversKey)
                {
                    if (!listaIdentidades.Contains(receiverID))
                    {
                        listaIdentidades.Add(receiverID);
                    }
                }
                foreach (Guid receiverGroupID in listaMensajes[id].ReceiversGroupKey)
                {
                    if (!listaGrupos.Contains(receiverGroupID))
                    {
                        listaGrupos.Add(receiverGroupID);
                    }
                }
            }
            Dictionary<Guid, ProfileModel> identidadesPublicadores = ObtenerIdentidadesPorID(listaIdentidades, false);
            Dictionary<Guid, GroupCardModel> grupos = ObtenerGruposPorID(listaGrupos);
            foreach (Guid id in listaMensajes.Keys)
            {
                if (listaMensajes[id].SenderKey != Guid.Empty && identidadesPublicadores.ContainsKey(listaMensajes[id].SenderKey))
                {
                    listaMensajes[id].Sender = identidadesPublicadores[listaMensajes[id].SenderKey];
                }
                listaMensajes[id].Receivers = new List<ProfileModel>();
                foreach (Guid receiverID in listaMensajes[id].ReceiversKey)
                {
                    if (identidadesPublicadores.ContainsKey(receiverID) && !listaMensajes[id].Receivers.Contains(identidadesPublicadores[receiverID]))
                    {
                        listaMensajes[id].Receivers.Add(identidadesPublicadores[receiverID]);
                    }
                }
                listaMensajes[id].ReceiversGroup = new List<GroupCardModel>();
                foreach (Guid receiverGroupID in listaMensajes[id].ReceiversGroupKey)
                {
                    if (grupos.ContainsKey(receiverGroupID) && !listaMensajes[id].ReceiversGroup.Contains(grupos[receiverGroupID]))
                    {
                        listaMensajes[id].ReceiversGroup.Add(grupos[receiverGroupID]);
                    }
                }
            }

            #endregion

            return listaMensajes;
        }

        /// <summary>
        /// Obtiene los modelos de una lista de comentarios
        /// </summary>
        /// <param name="pListaComentarios">Indetificadores de los comentarios</param>
        /// <returns></returns>
        public Dictionary<Guid, CommentSearchModel> ObtenerComentariosPorID(List<Guid> pListaComentarios, Guid pGrafo)
        {
            //Obtiene de BBDD los grupos que no estaban caché y los procesa para almacenarlos en caché
            List<ComentarioDocumentoProyecto> listaComentariosDocumentoProyecto = mMVCCN.ObtenerComentariosPorID(pListaComentarios, mIdentidadActual.Clave);
            #region Leemos las identidades

            Dictionary<Guid, CommentSearchModel> listaComentarios = new Dictionary<Guid, CommentSearchModel>();

            Dictionary<Guid, Tuple<Guid, string, int, TiposDocumentacion, string, Guid?, string>> listaComentarioDocumentoProy = new Dictionary<Guid, Tuple<Guid, string, int, TiposDocumentacion, string, Guid?, string>>();

            if (listaComentariosDocumentoProyecto != null)
            {
                foreach (ComentarioDocumentoProyecto comentarioDocumentoProyecto in listaComentariosDocumentoProyecto)
                {
                    Guid clave = comentarioDocumentoProyecto.ComentarioID;
                    Guid publicadorID = comentarioDocumentoProyecto.IdentidadID;
                    Guid documentoID = comentarioDocumentoProyecto.DocumentoID;
                    Guid proyectoID = comentarioDocumentoProyecto.ProyectoID;
                    DateTime fecha = comentarioDocumentoProyecto.Fecha;
                    string descripcion = comentarioDocumentoProyecto.Descripcion;

                    string nombreCortoProy = "";
                    if (proyectoID != ProyectoAD.MetaProyecto)
                    {
                        nombreCortoProy = comentarioDocumentoProyecto.NombreCorto;
                    }

                    string tituloDocumento = comentarioDocumentoProyecto.Titulo;

                    int versionImagen = 0;
                    if (comentarioDocumentoProyecto.VersionFotoDocumento.HasValue)
                    {
                        versionImagen = comentarioDocumentoProyecto.VersionFotoDocumento.Value;
                    }

                    TiposDocumentacion tipoDocumento = (TiposDocumentacion)comentarioDocumentoProyecto.Tipo;

                    string nombreCategoriaDoc = "";
                    if (comentarioDocumentoProyecto.NombreCategoriaDoc != null)
                    {
                        nombreCategoriaDoc = comentarioDocumentoProyecto.NombreCategoriaDoc;
                    }

                    Guid? ontologia = null;
                    if (comentarioDocumentoProyecto.ElementoVinculadoID.HasValue)
                    {
                        ontologia = comentarioDocumentoProyecto.ElementoVinculadoID.Value;
                    }

                    string nombreEntidadVinculada = "";
                    if (comentarioDocumentoProyecto.NombreElementoVinculado != null)
                    {
                        nombreEntidadVinculada = comentarioDocumentoProyecto.NombreElementoVinculado;
                    }

                    string extension = "";
                    if (tipoDocumento == TiposDocumentacion.Imagen)
                    {
                        string enlace = "";
                        if (comentarioDocumentoProyecto.Enlace != null)
                        {
                            enlace = comentarioDocumentoProyecto.Enlace;
                        }

                        extension = ".jpg";
                        if (nombreEntidadVinculada == "Wiki2")
                        {
                            extension = enlace.Substring(enlace.LastIndexOf('.'));
                        }
                    }

                    listaComentarioDocumentoProy.Add(clave, new Tuple<Guid, string, int, TiposDocumentacion, string, Guid?, string>(documentoID, nombreCortoProy, versionImagen, tipoDocumento, nombreCategoriaDoc, ontologia, extension));
                    CommentSearchModel fichaComentario = new CommentSearchModel();
                    fichaComentario.Key = clave;
                    fichaComentario.Description = descripcion;
                    fichaComentario.ResourceTitle = tituloDocumento;
                    fichaComentario.DateComment = fecha;
                    fichaComentario.Readed = true;
                    fichaComentario.PublisherKey = publicadorID;

                    listaComentarios[clave] = fichaComentario;
                }
            }

            #endregion

            foreach (CommentSearchModel fichaComentario in listaComentarios.Values)
            {
                Guid documentoID = listaComentarioDocumentoProy[fichaComentario.Key].Item1;
                Guid documentoVersionOriginalID = Guid.Empty;
                VersionDocumento versionDocumento = mDocumentacionCN.ObtenerVersionesDocumentoPorID(documentoID).ListaVersionDocumento.FirstOrDefault();
                if (versionDocumento != null)
                {
                    documentoVersionOriginalID = versionDocumento.DocumentoOriginalID;
                }
                documentoVersionOriginalID = documentoVersionOriginalID == Guid.Empty ? documentoID : documentoVersionOriginalID;
                string nombreCortoProy = listaComentarioDocumentoProy[fichaComentario.Key].Item2;
                int versionFoto = listaComentarioDocumentoProy[fichaComentario.Key].Item3;
                TiposDocumentacion tipoDoc = listaComentarioDocumentoProy[fichaComentario.Key].Item4;
                string nombreCategoriaDoc = listaComentarioDocumentoProy[fichaComentario.Key].Item5;
                Guid? ontologia = listaComentarioDocumentoProy[fichaComentario.Key].Item6;
                string extension = listaComentarioDocumentoProy[fichaComentario.Key].Item7;

                fichaComentario.ResourceUrl = UrlsSemanticas.GetURLBaseRecursosFichaConIDs(mBaseURLIdioma, mUtilIdiomas, nombreCortoProy, "/", fichaComentario.ResourceTitle, documentoVersionOriginalID, null, false);

                if (versionFoto > 0)
                {
                    fichaComentario.ResourceImageUrl = ImagenRecurso(tipoDoc, documentoID, versionFoto, nombreCategoriaDoc, ontologia, extension).Replace("BASEURLCONTENTREPLACE", BaseURLContent);
                }
            }

            #region Obtenemos los remitentes y destinatarios

            List<Guid> listaIdentidades = new List<Guid>();
            foreach (Guid id in listaComentarios.Keys)
            {
                if (!listaIdentidades.Contains(listaComentarios[id].PublisherKey))
                {
                    listaIdentidades.Add(listaComentarios[id].PublisherKey);
                }
            }
            Dictionary<Guid, ProfileModel> identidadesPublicadores = ObtenerIdentidadesPorID(listaIdentidades, false);
            foreach (Guid id in listaComentarios.Keys)
            {
                if (listaComentarios[id].PublisherKey != Guid.Empty)
                {
                    listaComentarios[id].Publisher = identidadesPublicadores[listaComentarios[id].PublisherKey];
                }
            }

            #endregion

            if (pGrafo != Guid.Empty)
            {
                FacetadoCN facetadoCN = new FacetadoCN("acidHome_Master", UrlIntragnoss, "", ReplicacionAD.COLA_REPLICACION_MASTER_HOME, mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
                Dictionary<Guid, bool> listaComentariosConLeido = facetadoCN.ObtenerLeidoListaComentarios(pGrafo, pListaComentarios);

                foreach (Guid id in listaComentarios.Keys)
                {
                    if (listaComentariosConLeido.ContainsKey(id))
                    {
                        listaComentarios[id].Readed = listaComentariosConLeido[id];
                    }
                }
            }

            return listaComentarios;
        }

        /// <summary>
        /// Obtiene los modelos de una lista de invitaciones
        /// </summary>
        /// <param name="pListaComentarios">Indetificadores de las invitaciones</param>
        /// <returns></returns>
        public Dictionary<Guid, InvitationModel> ObtenerInvitacionesPorID(List<Guid> pListaInvitaciones)
        {
            NotificacionCN notificacionCN = new NotificacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<NotificacionCN>(), mLoggerFactory);
            DataWrapperNotificacion notificacionDW = notificacionCN.ObtenerInvitacionesPorIDConNombreCorto(pListaInvitaciones);

            GestionNotificaciones gestorNotificaciones = new GestionNotificaciones(notificacionDW, mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GestionNotificaciones>(), mLoggerFactory);

            #region Leemos las identidades

            Dictionary<Guid, InvitationModel> listaInvitaciones = new Dictionary<Guid, InvitationModel>();

            foreach (AD.EntityModel.Models.Notificacion.Invitacion invitacion in notificacionDW.ListaInvitacion)
            {
                InvitationModel fichaInvitacion = new InvitationModel();

                fichaInvitacion.Key = invitacion.InvitacionID;
                fichaInvitacion.Subject = gestorNotificaciones.ListaNotificaciones[invitacion.NotificacionID].Asunto(mUtilIdiomas.LanguageCode);
                fichaInvitacion.Body = gestorNotificaciones.ListaNotificaciones[invitacion.NotificacionID].Mensaje(mUtilIdiomas.LanguageCode);

                switch ((AD.Notificacion.TiposNotificacion)invitacion.TipoInvitacion)
                {
                    case AD.Notificacion.TiposNotificacion.InvitacionUsuarioAOrgCorp:
                    case AD.Notificacion.TiposNotificacion.InvitacionUsuarioAOrgPers:
                        fichaInvitacion.Type = 0;
                        break;
                    case AD.Notificacion.TiposNotificacion.InvitacionContacto:
                    case AD.Notificacion.TiposNotificacion.InvitacionContactoOrg:
                        fichaInvitacion.Type = 1;
                        break;
                    default:
                        fichaInvitacion.Type = 2;
                        break;
                }
                fichaInvitacion.SenderKey = invitacion.IdentidadOrigenID;

                listaInvitaciones[invitacion.InvitacionID] = fichaInvitacion;
            }
            //foreach (Invitacion invitacion in gestorNotificaciones.ListaInvitaciones.Values)
            //{
            //    InvitationModel fichaInvitacion = new InvitationModel();

            //    fichaInvitacion.Key = invitacion.Clave;
            //    fichaInvitacion.Subject = invitacion.Notificacion.Asunto(pIdioma);
            //    fichaInvitacion.Body = invitacion.Notificacion.Mensaje(pIdioma);

            //    switch (invitacion.TipoInvitacion)
            //    {
            //        case AD.Notificacion.TiposNotificacion.InvitacionUsuarioAOrgCorp:
            //        case AD.Notificacion.TiposNotificacion.InvitacionUsuarioAOrgPers:
            //            fichaInvitacion.Type = 0;
            //            break;
            //        case AD.Notificacion.TiposNotificacion.InvitacionContacto:
            //        case AD.Notificacion.TiposNotificacion.InvitacionContactoOrg:
            //            fichaInvitacion.Type = 1;
            //            break;
            //        default:
            //            fichaInvitacion.Type = 2;
            //            break;
            //    }
            //    fichaInvitacion.SenderKey = invitacion.FilaInvitacion.IdentidadOrigenID;

            //    listaInvitaciones[invitacion.Clave] = fichaInvitacion;
            //}

            #endregion

            #region Obtenemos los remitentes y destinatarios

            List<Guid> listaIdentidades = new List<Guid>();
            foreach (Guid id in listaInvitaciones.Keys)
            {
                if (!listaIdentidades.Contains(listaInvitaciones[id].SenderKey))
                {
                    listaIdentidades.Add(listaInvitaciones[id].SenderKey);
                }
            }
            Dictionary<Guid, ProfileModel> identidadesPublicadores = ObtenerIdentidadesPorID(listaIdentidades, false);
            foreach (Guid id in listaInvitaciones.Keys)
            {
                if (listaInvitaciones[id].SenderKey != Guid.Empty)
                {
                    listaInvitaciones[id].Sender = identidadesPublicadores[listaInvitaciones[id].SenderKey];
                }
            }

            #endregion

            return listaInvitaciones;
        }

        /// <summary>
        /// Obtiene los modelos de una lista de contactos
        /// </summary>
        /// <param name="pListaComentarios">Indetificadores de las personas</param>
        /// <param name="pListaComentarios">Indetificadores de las organizaciones</param>
        /// <param name="pListaComentarios">Indetificadores de los grupos</param>
        /// <returns></returns>
        public Dictionary<Guid, ContactModel> ObtenerContactosPorID(List<Guid> pListaPersonas, List<Guid> pListaOrganizaciones, List<Guid> pListaGrupos)
        {
            List<ContactosPorID> listaContactosPorID = mMVCCN.ObtenerContactosPorID(pListaPersonas, pListaOrganizaciones, pListaGrupos, mIdentidadActual.Clave);

            Dictionary<Guid, ContactModel> listaContactos = new Dictionary<Guid, ContactModel>();

            if (listaContactosPorID != null)
            {
                foreach (ContactosPorID contacto in listaContactosPorID)
                {
                    Guid clave = contacto.IdentidadID;
                    int tipoContacto = contacto.TipoContacto;
                    string nombre = contacto.Nombre;
                    string nombreCortoUsu = "";
                    if (contacto.NombreCortoUsu != null)
                    {
                        nombreCortoUsu = contacto.NombreCortoUsu;
                    }
                    string nombreCortoOrg = "";
                    if (contacto.NombreCortoOrg != null)
                    {
                        nombreCortoOrg = contacto.NombreCortoOrg;
                    }
                    string urlFoto = contacto.Foto;


                    ContactModel fichaContacto = new ContactModel();
                    fichaContacto.Key = clave;
                    fichaContacto.Type = (ContactModel.ContactType)tipoContacto;
                    fichaContacto.Name = nombre;

                    if (tipoContacto != 2)
                    {
                        string url = UrlsSemanticas.GetURLPerfilPersonaOOrgEnProyecto(mBaseURL, mUtilIdiomas, UrlPerfil(mIdentidadActual), null, nombreCortoUsu, nombreCortoOrg);
                        fichaContacto.Url = url;

                        if (urlFoto != "sinfoto")
                        {
                            fichaContacto.Foto = urlFoto;
                        }
                    }

                    listaContactos[clave] = fichaContacto;
                }
            }

            List<ParticipantesGrupoContactos> listaParticpantesGrupos = mMVCCN.ObtenerParticipantesGruposContactosPorID(pListaPersonas, pListaOrganizaciones, pListaGrupos, mIdentidadActual.Clave);

            if (listaParticpantesGrupos != null)
            {
                foreach (ParticipantesGrupoContactos participante in listaParticpantesGrupos)
                {
                    Guid GrupoID = participante.GrupoID;
                    Guid IdentidadID = participante.IdentidadID;
                    int tipo = participante.Tipo;
                    string nombre = participante.Nombre;

                    if (tipo == 0)
                    {
                        if (listaContactos.ContainsKey(IdentidadID))
                        {
                            if (listaContactos[IdentidadID].ListGroups == null)
                            {
                                listaContactos[IdentidadID].ListGroups = new Dictionary<string, string>();
                            }
                            if (!listaContactos[IdentidadID].ListGroups.ContainsKey(GrupoID.ToString()))
                            {
                                listaContactos[IdentidadID].ListGroups.Add(GrupoID.ToString(), nombre);
                            }
                        }
                    }
                    else
                    {
                        if (listaContactos.ContainsKey(GrupoID))
                        {
                            if (listaContactos[GrupoID].ListMembers == null)
                            {
                                listaContactos[GrupoID].ListMembers = new Dictionary<string, string>();
                            }
                            if (!listaContactos[GrupoID].ListMembers.ContainsKey(IdentidadID.ToString()))
                            {
                                listaContactos[GrupoID].ListMembers.Add(IdentidadID.ToString(), nombre);
                            }
                        }
                    }
                }
            }
            return listaContactos;
        }

        public Dictionary<Guid, PaginaCMSModel> ObtenerPaginasCMSPorID(List<Guid> pListaPaginasCMSID, Guid pProyectoID)
        {
            Dictionary<Guid, PaginaCMSModel> dicPaginasCMSModels = new Dictionary<Guid, PaginaCMSModel>();

            // Obtener los datos de la pestanya
            ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            DataWrapperProyecto dataWrapperProyecto = new DataWrapperProyecto();
            proyCN.ObtenerPestanyasProyecto(pProyectoID, dataWrapperProyecto);

            foreach (Guid pestanyaID in pListaPaginasCMSID)
            {
                List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> pestanyasMenu = dataWrapperProyecto.ListaProyectoPestanyaMenu.Where(proy => proy.PestanyaID.Equals(pestanyaID)).ToList();
                List<ProyectoPestanyaBusqueda> pestanyasBusqueda = dataWrapperProyecto.ListaProyectoPestanyaBusqueda.Where(proy => proy.PestanyaID.Equals(pestanyaID)).ToList();
                if (pestanyasMenu.Count > 0)
                {
                    AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu filaPestanyaMenu = pestanyasMenu.First();
                    PaginaCMSModel paginaCMSModel = new PaginaCMSModel();
                    paginaCMSModel.Key = pestanyaID;
                    string titulo = UtilCadenas.ObtenerTextoDeIdioma(filaPestanyaMenu.Titulo, mUtilIdiomas.LanguageCode, null);
                    if (string.IsNullOrEmpty(titulo))
                    {
                        titulo = UtilCadenas.ObtenerTextoDeIdioma(filaPestanyaMenu.Nombre, mUtilIdiomas.LanguageCode, null);
                    }
                    paginaCMSModel.Title = titulo;
                    paginaCMSModel.Url = BaseURLIdioma + "/" + UtilCadenas.ObtenerTextoDeIdioma(filaPestanyaMenu.Ruta, mUtilIdiomas.LanguageCode, null);

                    paginaCMSModel.Languages = new List<string>();
                    if (!string.IsNullOrEmpty(filaPestanyaMenu.IdiomasDisponibles))
                    {
                        string[] separador = { "|||" };
                        foreach (string idioma in filaPestanyaMenu.IdiomasDisponibles.Split(separador, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string tempIdioma = idioma;
                            if (tempIdioma.Contains("@"))
                            {
                                tempIdioma = tempIdioma.Substring(tempIdioma.IndexOf("@") + 1);
                            }

                            paginaCMSModel.Languages.Add(tempIdioma);
                        }
                    }

                    dicPaginasCMSModels.Add(pestanyaID, paginaCMSModel);
                }
            }

            return dicPaginasCMSModels;
        }

        #endregion

        #region Métodos auxiliares para la generación de los modelos

        /// <summary>
        /// Obtiene la identidad actual del usuario para la url-semantica, vacio si estas en una comunidad y "/" si estas en modo personal
        /// </summary>
        private string UrlPerfil(Identidad pIdentidad)
        {
            string urlPerfil = "/";

            if (pIdentidad != null && (pIdentidad.TrabajaConOrganizacion || pIdentidad.EsIdentidadProfesor))
            {
                string nombreCorto = pIdentidad.PerfilUsuario.NombreCortoOrg;

                if (pIdentidad.EsIdentidadProfesor)
                {
                    nombreCorto = pIdentidad.PerfilUsuario.NombreCortoUsu;
                }

                urlPerfil += mUtilIdiomas.GetText("URLSEM", "IDENTIDAD") + "/" + nombreCorto + "/";
            }
            return urlPerfil;
        }

        #region Imágenes

        /// <summary>
        /// Obtiene la imagen de un recurso
        /// </summary>
        /// <param name="pTipoDocumentacion">Tipo de  documento</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <param name="pVersionFoto">Versión de la foto del documento</param>
        /// <param name="pNombreCategoriaDoc">NombreCategoria (para recursos semanticos)</param>
        /// <param name="pOntologiaID">Identificador de la ontología (en caso de que sea semántico)</param>
        /// <returns>URL de la imagen</returns>
        private string ImagenRecurso(TiposDocumentacion pTipoDocumentacion, Guid pDocumentoID, int? pVersionFoto, string pNombreCategoriaDoc, Guid? pOntologiaID, string pExtension)
        {
            string urlImagen = "";
            Guid ontologiaID = Guid.Empty;

            if ((pTipoDocumentacion == TiposDocumentacion.Hipervinculo || pTipoDocumentacion == TiposDocumentacion.Video || pTipoDocumentacion == TiposDocumentacion.VideoBrightcove || pTipoDocumentacion == TiposDocumentacion.VideoTOP || pTipoDocumentacion == TiposDocumentacion.FicheroServidor || pTipoDocumentacion == TiposDocumentacion.Nota))
            {
                string fileName = HttpUtility.UrlEncode(pDocumentoID.ToString()) + ".jpg";
                if (pVersionFoto.HasValue)
                {
                    urlImagen = "BASEURLCONTENTREPLACE" + "/" + UtilArchivos.ContentImagenesEnlaces + "/" + UtilArchivos.DirectorioDocumento(pDocumentoID) + "/" + fileName + "?" + pVersionFoto.Value;
                }
            }
            else if (pTipoDocumentacion == TiposDocumentacion.Imagen)
            {
                string fileName = HttpUtility.UrlEncode(pDocumentoID.ToString()) + "_peque" + pExtension;
                if (pVersionFoto.HasValue)
                {
                    urlImagen = "BASEURLCONTENTREPLACE" + "/" + UtilArchivos.ContentImagenes + "/" + UtilArchivos.ContentImagenesDocumentos + "/Miniatura/" + UtilArchivos.DirectorioDocumento(pDocumentoID) + "/" + fileName + "?" + pVersionFoto.Value;
                }
            }
            else if ((pTipoDocumentacion == TiposDocumentacion.Semantico) && !string.IsNullOrEmpty(pNombreCategoriaDoc))
            {
                if (pNombreCategoriaDoc.Contains(',') && pNombreCategoriaDoc.Contains('.'))
                {
                    string fileName = pNombreCategoriaDoc;

                    string extension = fileName.Substring(fileName.LastIndexOf('.'));

                    if (fileName.Contains("|"))
                    {
                        fileName = fileName.Split('|')[0];
                    }

                    return "BASEURLCONTENTREPLACE" + "/" + fileName.Substring(fileName.LastIndexOf(",") + 1).Replace(extension, "_" + fileName.Substring(0, fileName.IndexOf(",")) + extension);
                }
            }

            if (pTipoDocumentacion == TiposDocumentacion.Semantico && pOntologiaID.HasValue)
            {
                ontologiaID = pOntologiaID.Value;
            }


            if (string.IsNullOrEmpty(urlImagen))
            {
                urlImagen = ObtenerUrlImagenPorDefectoDocumento((short)pTipoDocumentacion, ontologiaID);
            }

            return urlImagen;
        }

        /// <summary>
        /// Obtiene la imagen por defecto del tipo de documento indicado.
        /// </summary>
        /// <param name="pTipoDocumento">Tipo de documento</param>
        /// <param name="pOntologiaID">ID de la ontología del tipo si la tiene</param>
        /// <returns>Imagen por defecto del tipo de documento indicado</returns>
        protected string ObtenerUrlImagenPorDefectoDocumento(short pTipoDocumento, Guid? pOntologiaID)
        {
            Guid identificadorOntologia = Guid.Empty;
            if (pOntologiaID.HasValue)
            {
                identificadorOntologia = pOntologiaID.Value;
            }
            if (TipoDocumentoImagenPorDefecto != null)
            {
                if (TipoDocumentoImagenPorDefecto.ContainsKey(pTipoDocumento) && TipoDocumentoImagenPorDefecto[pTipoDocumento].ContainsKey(identificadorOntologia))
                {
                    return "BASEURLCONTENTREPLACE" + "/" + TipoDocumentoImagenPorDefecto[pTipoDocumento][identificadorOntologia];
                }
                else if (pOntologiaID != Guid.Empty && TipoDocumentoImagenPorDefecto.ContainsKey(pTipoDocumento) && TipoDocumentoImagenPorDefecto[pTipoDocumento].ContainsKey(Guid.Empty))
                {
                    return "BASEURLCONTENTREPLACE" + "/" + TipoDocumentoImagenPorDefecto[pTipoDocumento][Guid.Empty];
                }
                else if (TipoDocumentoImagenPorDefecto.ContainsKey(-1))
                {
                    return "BASEURLCONTENTREPLACE" + "/" + TipoDocumentoImagenPorDefecto[-1][Guid.Empty];
                }
            }

            return "";
        }

        #endregion

        /// <summary>
        /// Obtiene el nombre semántico
        /// </summary>
        /// <param name="pNombre">Nombre</param>
        /// <returns>Nombre semántico</returns>
        private string ObtenerNombreSemantico(string pNombre, string pIdioma)
        {
            pNombre = UtilCadenas.EliminarCaracteresUrlSem(UtilCadenas.ObtenerTextoDeIdioma(pNombre, pIdioma, null));

            //Elimino los puntos del final:
            while (pNombre.Length > 0 && pNombre[pNombre.Length - 1] == '.')
            {
                pNombre = pNombre.Remove(pNombre.Length - 1);
            }

            if (string.IsNullOrEmpty(pNombre))
            {
                pNombre = "no-name";
            }

            return pNombre;
        }

        public static string ObtenerUrlRecursoParaPeticionServicioContextos(string pUrlServicioContextos, string pUrlRecurso, Guid pResourceID, string pCommunityShortName)
        {
            string resultado = pUrlRecurso;

            if (!string.IsNullOrEmpty(pUrlServicioContextos))
            {
                resultado = string.Concat(pUrlServicioContextos, "/", pCommunityShortName, "/", pResourceID.ToString());
            }

            return resultado;
        }

        #region Tipos de recursos

        private bool EsVideoIncrustado(ResourceModel.DocumentType pTipoDocumentacion, string pEnlace)
        {
            if (!string.IsNullOrEmpty(pEnlace))
            {
                string enlaceSinHttp = pEnlace;
                if (enlaceSinHttp.StartsWith("http://")) { enlaceSinHttp = enlaceSinHttp.Substring(7); }
                if (enlaceSinHttp.StartsWith("https://")) { enlaceSinHttp = enlaceSinHttp.Substring(8); }
                if (enlaceSinHttp.StartsWith("www.")) { enlaceSinHttp = enlaceSinHttp.Substring(4); }

                if ((enlaceSinHttp.StartsWith("youtube.com") && pEnlace.Contains("/watch?")) || enlaceSinHttp.StartsWith("youtube.com") || pEnlace.StartsWith("youtu.be/"))
                {
                    string v = "";
                    if (enlaceSinHttp.StartsWith("youtu.be/"))
                    {
                        v = pEnlace.Replace("youtu.be/", "");

                        if (v.Contains("/"))
                        {
                            v = v.Substring(0, v.IndexOf("/"));
                        }
                    }
                    else
                    {
                        v = System.Web.HttpUtility.ParseQueryString(new Uri(pEnlace).Query).Get("v");
                    }

                    return (!string.IsNullOrEmpty(v));
                }
                else if (enlaceSinHttp.StartsWith("vimeo.com"))
                {
                    string v = (new Uri(pEnlace)).AbsolutePath;
                    int idVideo;
                    int inicio = v.LastIndexOf("/");
                    return (int.TryParse(v.Substring(inicio + 1, v.Length - inicio - 1), out idVideo));
                }
                else if (pEnlace.StartsWith("http://www.ted.com/talks/") || pEnlace.StartsWith("www.ted.com/talks/") || pEnlace.StartsWith("ted.com/talks/") || pEnlace.StartsWith("http://tedxtalks.ted.com/video/") || pEnlace.StartsWith("tedxtalks.ted.com/video/"))
                {
                    return true;
                }
            }
            if (pTipoDocumentacion == ResourceModel.DocumentType.VideoBrightcove)
            {
                return true;
            }
            if (pTipoDocumentacion == ResourceModel.DocumentType.VideoTOP)
            {
                return true;
            }
            return false;
        }

        private bool EsAudioIncrustado(ResourceModel.DocumentType pTipoDocumentacion)
        {
            if (pTipoDocumentacion == ResourceModel.DocumentType.AudioBrightcove || pTipoDocumentacion == ResourceModel.DocumentType.AudioTOP)
            {
                return true;
            }
            return false;
        }

        private bool EsPresentacionIncrustada(string pEnlace)
        {
            if (!string.IsNullOrEmpty(pEnlace) && (pEnlace.StartsWith("http://www.slideshare.net") || pEnlace.StartsWith("http://es.slideshare.net") || pEnlace.StartsWith("www.slideshare.net") || pEnlace.StartsWith("https://www.slideshare.net") || pEnlace.StartsWith("https://es.slideshare.net") || pEnlace.Contains("slideshare.net")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool EsFicheroDigital(ResourceModel.DocumentType pTipoDocumentacion, string pEnlace)
        {
            return ((pTipoDocumentacion == ResourceModel.DocumentType.FicheroServidor || pTipoDocumentacion == ResourceModel.DocumentType.Video || pTipoDocumentacion == ResourceModel.DocumentType.Imagen) && !EsVideoIncrustado(pTipoDocumentacion, pEnlace) && !EsPresentacionIncrustada(pEnlace));
        }

        #endregion

        /// <summary>
        /// Procesa los modelos para obtener el HTML que se corresponda (multiidioma)
        /// </summary>
        /// <param name="pModel">Modelo para procesar</param>
        /// <returns>Modelo procesado</returns>
        private object ProcesarModeloParaPresentacion(object pModel, string pIdioma)
        {
            //JavaScriptSerializer serializador = new JavaScriptSerializer(new SimpleTypeResolver());
            if (pModel is ResourceModel)
            {
                if (((ResourceModel)pModel).Shareds != null)
                {
                    foreach (CategoryModel categoria in ((ResourceModel)pModel).Categories)
                    {
                        categoria.Lang = pIdioma;
                    }
                }

                //Al serializar una fecha, esta se convierte utc, perdemos horas... 
                //No he conseguido 
                //pModel = serializador.Deserialize<ResourceModel>(ReemplazarClavesCacheHTML(serializador.Serialize((ResourceModel)pModel)));
            }
            return pModel;
        }

        private ViewResorceModel GenerarViewResourceModel(TipoVista pTipoVista, Guid pOntologiaID, DataWrapperProyecto pDataWrapperProyecto, string pInfoExtra)
        {
            return new ViewResorceModel()
            {
                ShowDescription = VisibilidadTipoVista(pTipoVista, "descripcion", pOntologiaID, pDataWrapperProyecto),
                ShowCategories = VisibilidadTipoVista(pTipoVista, "etiquetas", pOntologiaID, pDataWrapperProyecto),
                ShowTags = VisibilidadTipoVista(pTipoVista, "categorias", pOntologiaID, pDataWrapperProyecto),
                ShowPublisher = VisibilidadTipoVista(pTipoVista, "publicador", pOntologiaID, pDataWrapperProyecto),
                InfoExtra = pInfoExtra,
            };
        }

        #endregion

        #region Propiedades

        public Dictionary<short, Dictionary<Guid, string>> TipoDocumentoImagenPorDefecto
        {
            get
            {
                if (mTipoDocumentoImagenPorDefecto == null)
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCL>(), mLoggerFactory);
                    mTipoDocumentoImagenPorDefecto = proyCL.ObtenerTipoDocImagenPorDefecto(mProyecto.Clave);
                    proyCL.Dispose();
                }
                return mTipoDocumentoImagenPorDefecto;
            }
        }

        /// <summary>
        /// Obtiene URL content
        /// </summary>
        public string BaseURLContent
        {
            get
            {
                string urlContent = "";
                if (mBaseURLsContent != null)
                {
                    if (mBaseURLsContent.Count > 0)
                    {

                        if (!mIndiceActualBaseUrlContent.HasValue)
                        {
                            Random rnd = new Random();
                            mIndiceActualBaseUrlContent = rnd.Next(mBaseURLsContent.Count);
                        }

                        urlContent = mBaseURLsContent[mIndiceActualBaseUrlContent.Value];

                        //reinicializo el índice
                        mIndiceActualBaseUrlContent++;
                        if (mBaseURLsContent.Count == mIndiceActualBaseUrlContent)
                        {
                            mIndiceActualBaseUrlContent = 0;
                        }
                    }
                }

                return urlContent;
            }
            set
            {
                if (mBaseURLsContent == null)
                {
                    mBaseURLsContent = new List<string>();
                }
                mBaseURLsContent.Add(value);
            }
        }

        /// <summary>
        /// Obtiene URL static
        /// </summary>
        public string BaseURLStatic
        {
            get
            {
                return mBaseURLStatic;
            }
        }

        public string BaseURL
        {
            get
            {
                string baseUrl = mBaseURL;
                if (mProyecto.Clave != ProyectoAD.MetaProyecto)
                {
                    //Reemplazo marcas por su valor:
                    string urlComunidad = UrlsSemanticas.ObtenerURLComunidad(mUtilIdiomas, mBaseURLIdioma, mProyecto.NombreCorto);
                    baseUrl = UtilDominios.ObtenerDominioUrl(new Uri(urlComunidad), true);
                }

                return baseUrl;
            }
        }

        public string BaseURLIdioma
        {
            get
            {
                string baseUrlIdioma = BaseURL;

                if (mParametrosGeneralesRow == null)
                {
                    if (mUtilIdiomas.LanguageCode != "es")
                    {
                        baseUrlIdioma += "/" + mUtilIdiomas.LanguageCode;
                    }
                }
                else if (!string.IsNullOrEmpty(mParametrosGeneralesRow.IdiomaDefecto))
                {
                    if (mUtilIdiomas.LanguageCode != mParametrosGeneralesRow.IdiomaDefecto)
                    {
                        baseUrlIdioma += "/" + mUtilIdiomas.LanguageCode;
                    }
                }

                return baseUrlIdioma;
            }
        }

        //private ParametroAplicacionDS mParametrosAplicacionDS = null;
        private List<AD.EntityModel.ParametroAplicacion> mParametrosAplicacionDS = null;
        public List<AD.EntityModel.ParametroAplicacion> ParametrosAplicacion
        {
            get
            {
                if (mParametrosAplicacionDS == null)
                {
                    ParametroAplicacionCL paramAplicacionCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
                    mParametrosAplicacionDS = paramAplicacionCL.ObtenerParametrosAplicacionPorContext();
                }
                return mParametrosAplicacionDS;
            }
        }

        #endregion
    }


    /// <summary>
    /// Enumeración que indica si  se carga una base de recursos personal.
    /// </summary>
    public enum EspacioPersonal
    {
        /// <summary>
        /// Indica que no es espacio personal.
        /// </summary>
        No = 0,
        /// <summary>
        /// Indica que es el espacio personal de un usuario.
        /// </summary>
        Usuario = 1,
        /// <summary>
        /// Indica que es el espacio personal de una organización.
        /// </summary>
        Organizacion = 2
    }
}
