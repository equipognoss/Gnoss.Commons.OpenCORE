using BeetleX.Redis;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders.Physical;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using BeetleX.Clients;
using Es.Riam.Gnoss.AD.Facetado;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.CL
{
    /// <summary>
    /// Clase que contiene variables estaticas con los prefijos de los nombres de la cache
    /// </summary>
    public static class NombresCL
    {
        /// <summary>
        /// Prefijo "Tesauro"
        /// </summary>
        public static readonly string TESAURO = "Tesauro";

        /// <summary>
        /// Prefijo "EstadosCiviles"
        /// </summary>
        public static readonly string ESTADOSCIVILES = "EstadosCiviles";

        /// <summary>
        /// Prefijo "Paises"
        /// </summary>
        public static readonly string PAISES = "Paises";

        /// <summary>
        /// Prefijo "ParametroAplicacion"
        /// </summary>
        public static readonly string PARAMETOAPLICACION = "ParametroAplicacion";

        /// <summary>
        /// Prefijo "ParametroGeneralDS"
        /// </summary>
        public static readonly string PARAMETROGENERAL = "ParametroGeneral";

        /// <summary>
        /// Prefijo "TodosParametrosGeneralesLigera"
        /// </summary>
        public static readonly string TODOSPARAMETROSGENERALESLIGERA = "TodosParametrosGeneralesLigera";

        /// <summary>
        /// Prefijo "TieneProyectoImagenHome"
        /// </summary>
        public static readonly string TIENEPROYECTOIMAGENHOME = "TieneProyectoImagenHome";

        /// <summary>
        /// Prefijo "Facetado"
        /// </summary>
        public static readonly string FACETADO = "Facetado";

        /// <summary>
        /// Prefijo "TABLACONFIGURACIONFACETADO"
        /// </summary>
        public static readonly string TABLACONFIGURACIONFACETADO = "TablaConfigFacetado";

        /// <summary>
        /// Prefijo "CONFIGURACIONFACETAS"
        /// </summary>
        public static readonly string CONFIGURACIONFACETAS = "ConfigFacetas";

        /// <summary>
        /// Prefijo "ParametroAplicacion"
        /// </summary>
        public static readonly string LINKEDOPENDATA = "LOD";

        /// <summary>
        /// Prefijo "CMS"
        /// </summary>
        public static readonly string CMS = "CMS";

        /// <summary>
        /// Prefijo "CMS"
        /// </summary>
        public static readonly string TRAZAS = "TRAZAS";

        /// <summary>
        /// Prefijo "MVC"
        /// </summary>
        public static readonly string MVCModel = "MVC";

        /// <summary>
        /// Prefijo "ExportacionBusqueda"
        /// </summary>
        public static readonly string EXPORTACIONBUSQUEDA = "ExportacionBusqueda";

        #region AMIGOS

        /// <summary>
        /// Prefijo "Amigos"
        /// </summary>
        public static readonly string AMIGOS = "Amigos";

        /// <summary>
        /// Prefijo "Org"
        /// </summary>
        public static readonly string AMIGOSORG = "Org";

        /// <summary>
        /// Prefijo "Autocompletar"
        /// </summary>
        public static readonly string AUTOCOMPLETARINVALIDARCACHE = "AutocompletarInvalidarCache";

        /// <summary>
        /// Prefijo "Org"
        /// </summary>
        public static readonly string AMIGOSORGEIDENTPROYPRIV = "OrgEIdentPryPri";

        /// <summary>
        /// Prefijo "Per"
        /// </summary>
        public static readonly string AMIGOSPER = "Per";

        /// <summary>
        /// Prefijo "Per"
        /// </summary>
        public static readonly string AMIGOSPEREIDENTPROYPRIV = "PerEIdentPryPri";

        public static readonly string IDENTIDADDS = "IdentidadDS";

        public static readonly string PERSONADS = "PersonaDS";

        public static readonly string TAGDS = "TagDS";

        public static readonly string USUARIODS = "UsuarioDS";

        public static readonly string ORGANIZACIONDS = "OrganizacionDS";

        public static readonly string AMIGOSDS = "AmigosDS";

        public static readonly string AMIGOSENPROYECTO = "AmigosEnProyecto";

        public static readonly string CONTACTOSRECOMEN = "ContactosRecomen";

        public static readonly string COMUNIDADESRECOMEN = "ComunidadesRecomen";

        #endregion

        #region DOCUMENTACION

        /// <summary>
        /// Prefijo "Documentacion"
        /// </summary>
        public static readonly string DOCUMENTACION = "Documentacion";

        /// <summary>
        /// Prefijo "FichaRecursoMVC"
        /// </summary>
        public static readonly string FICHARECURSOMVC = "FichaRecursoMVC3";

        /// <summary>
        /// Prefijo "RecursosComunidad"
        /// </summary>
        public static readonly string RECURSOSCOMUNIDAD = "RecursosComunidad";

        /// <summary>
        /// Prefijo "PrimerosRecursos"
        /// </summary>
        public static readonly string PRIMEROSRECURSOS = "PrimerosRecursos";

        /// <summary>
        /// Prefijo "RssComunidad"
        /// </summary>
        public static readonly string RSSSCOMUNIDAD = "RssComunidad";

        /// <summary>
        /// Prefijo "TotalResultadosRecursos"
        /// </summary>
        public static readonly string TOTALRESULTADOSRECURSOS = "TotalResultadosRecursos";

        /// <summary>
        /// Prefijo "PerfilesConRecursosPrivados"
        /// </summary>
        public static readonly string PERFILESCONRECURSOSPRIVADOS = "PerfilesConRecursosPrivados";

        /// <summary>
        /// Prefijo "PrimerosDebates"
        /// </summary>
        public static readonly string PRIMEROSDEBATES = "PrimerosDebates";

        /// <summary>
        /// Prefijo "PrimerasPersonas"
        /// </summary>
        public static readonly string PRIMERASPERSONAS = "PrimerasPersonas";

        /// <summary>
        /// Prefijo "TotalResultadosDebates"
        /// </summary>
        public static readonly string TOTALRESULTADOSDEBATES = "TotalResultadosDebates";

        /// <summary>
        /// Prefijo "PerfilesConDebatesPrivados"
        /// </summary>
        public static readonly string PERFILESCONDEBATESPRIVADOS = "PerfilesConDebatesPrivados";

        /// <summary>
        /// Prefijo "PrimerasPreguntas"
        /// </summary>
        public static readonly string PRIMERASPREGUNTAS = "PrimerasPreguntas";

        /// <summary>
        /// Prefijo "TotalResultadosPreguntas"
        /// </summary>
        public static readonly string TOTALRESULTADOSPREGUNTAS = "TotalResultadosPreguntas";

        /// <summary>
        /// Prefijo "RecursosRelacionadosContextos"
        /// </summary>
        public static readonly string RECURSOSRELACIONADOSCONTEXTOS = "Cont";

        /// <summary>
        /// Prefijo "RecursosRelacionados"
        /// </summary>
        public static readonly string RECURSOSRELACIONADOS = "RecursosRelacionados";

        /// <summary>
        /// Prefijo "RecursosRelacionados"
        /// </summary>
        public static readonly string NUMRECURSOSRELACIONADOS = "NumRecursosRelacionados";

        /// <summary>
        /// Prefijo "QueEstaPasando"
        /// </summary>
        public static readonly string QUEESTAPASANDO = "QueEstaPasando";

        /// <summary>
        /// Prefijo "QueSeEstaLeyendo"
        /// </summary>
        public static readonly string QUESEESTALEYENDO = "QueSeEstaLeyendo";

        /// <summary>
        /// Prefijo "QueSeEstaLeyendo"
        /// </summary>
        public static readonly string AUTOCOMPETIQUETAS = "autocompletaretiquetas";

        #endregion

        #region LIVE

        /// <summary>
        /// Prefijo "Live"
        /// </summary>
        public static readonly string LIVE = "Live";

        /// <summary>
        /// Prefijo "OrgsConRecursos"
        /// </summary>
        public static readonly string ORGSCONRECURSOS = "OrgsConRecursos";

        /// <summary>
        /// Prefijo "IdenConPrivados"
        /// </summary>
        public static readonly string IDENCONPRIVADOS = "IdenConPrivados";

        /// <summary>
        /// Prefijo "OrgID"
        /// </summary>
        public static readonly string ORGID = "OrgID";

        /// <summary>
        /// Prefijo "IdenID"
        /// </summary>
        public static readonly string IDENID = "IdenID";

        public static readonly string INVITADOGNOSS = "InvitadoGnoss";

        public static readonly string INVITADOCOM = "InvitadoCom";

        #endregion

        #region ORGANIZACION

        /// <summary>
        /// Prefijo "Organizacion"
        /// </summary>
        public static readonly string ORGANIZACION = "Organizacion";

        /// <summary>
        /// Prefijo "TablaBaseOrganizacionID"
        /// </summary>
        public static readonly string TABLABASEORGANIZACIONID = "TablaBaseOrganizacionID";

        #endregion

        #region PERSONAS

        /// <summary>
        /// Prefijo "Personas"
        /// </summary>
        public static readonly string PERSONAS = "Personas";

        /// <summary>
        /// Prefijo "Todas"
        /// </summary>
        public static readonly string TODAS = "Todas";

        /// <summary>
        /// Prefijo "PersonasAcceden"
        /// </summary>
        public static readonly string PERSONASACCEDEN = "PersonasAcceden";

        /// <summary>
        /// Prefijo "NotificacionPendiente"
        /// </summary>
        public static readonly string NOTIFICACIONPENDIENTE = "NotificacionPendiente";

        #endregion

        #region PROYECTO

        /// <summary>
        /// Prefijo "Proyecto"
        /// </summary>
        public static readonly string PROYECTO = "Proyecto";

        /// <summary>
        /// Prefijo "TablaBaseProyectoID"
        /// </summary>
        public static readonly string TABLABASEPROYECTOID = "TablaBaseProyectoID";

        /// <summary>
        /// Prefijo "ProyectoEventoAccion"
        /// </summary>
        public static readonly string PROYECTOEVENTOACCION = "ProyectoEventoAccion";

        /// <summary>
        /// Prefijo "ProyectosAccionesExternas"
        /// </summary>
        public static readonly string PROYECTOSACCIONESEXTERNAS = "ProyectosAccionesExternas";

        /// <summary>
        /// Prefijo "NivelesCertificacionRecurso"
        /// </summary>
        public static readonly string NIVELESCERTIFICACIONRECURSO = "NivelesCertificacionRecurso";

        /// <summary>
        /// Prefijo "NombreCortoProyecto"
        /// </summary>
        public static readonly string NOMBRECORTOPROYECTO = "NombreCortoProyecto";

        /// <summary>
        /// Prefijo "BRID"
        /// </summary>
        public static readonly string BASERECURSOSID = "BRID";

        /// <summary>
        /// Prefijo "ProyectoAutoPromocion"
        /// </summary>
        public static readonly string PROYECTOAUTOPROMOCION = "ProyectoAutoPromocion";

        /// <summary>
        /// Prefijo "HTMLAdministradoresProyecto"
        /// </summary>
        public static readonly string HTMLADMINISTRADORESPROYECTO = "HTMLAdministradoresProyecto";

        /// <summary>
        /// Prefijo "ProyectosPorID"
        /// </summary>
        public static readonly string PROYECTOSPORID = "ProyectosPorID";

        /// <summary>
        /// Prefijo "ClaveReinicio"
        /// </summary>
        public static readonly string CLAVEREINICIO = "ClaveReinicio";

        /// <summary>
        /// Prefijo "TipoAcceso"
        /// </summary>
        public static readonly string USUARIOBLOQUEADOPROYECTO = "UsuarioBloqueadoProyecto";

        /// <summary>
        /// Prefijo "TipoAcceso"
        /// </summary>
        public static readonly string TIPOACCESO = "TipoAcceso";

        /// <summary>
        /// Prefijo "MisComunidades"
        /// </summary>
        public static readonly string MISCOMUNIDADES = "MisComunidades";

        /// <summary>
        /// Prefijo "ClausulasRegitroProyecto"
        /// </summary>
        public static readonly string CLAUSULASREGITROPROYECTO = "ClausulasRegitroProyecto";

        /// <summary>
        /// Prefijo "ClausulasRegitroProyecto"
        /// </summary>
        public static readonly string POLITICACOOKIESPROYECTO = "PoliticaCookiesProyecto";

        /// <summary>
        /// Prefijo "ListaProyectosPerfil"
        /// </summary>
        public static readonly string LISTAPROYECTOSPERFIL = "ListaProyectosPerfil";

        /// <summary>
        /// Prefijo "ListaProyectosUsuario"
        /// </summary>
        public static readonly string LISTAPROYECTOSUSUARIO = "ListaProyectosUsuario";

        #endregion

        #region TAG

        /// <summary>
        /// Prefijo "Tag"
        /// </summary>
        public static readonly string TAG = "Tag";

        #endregion

        #region HOME

        /// <summary>
        /// Prefijo "Home_Comunidades_Destacadas_Html"
        /// </summary>
        public static readonly string HOMECOMUNIDADESDESTACADASHTML = "Home_Comunidades_Destacadas_Html";

        /// <summary>
        /// Prefijo "Home_Comunidades_Destacadas_DS"
        /// </summary>
        public static readonly string HOMECOMUNIDADESDESTACADASDS = "Home_Comunidades_Destacadas_DS";

        /// <summary>
        /// Prefijo "Home_Recursos_Destacados_Html"
        /// </summary>
        public static readonly string HOMERECURSOSDESTACADOSHTML = "Home_Recursos_Destacados_Html";

        /// <summary>
        /// Prefijo "Home_Recursos_Destacados_DS"
        /// </summary>
        public static readonly string HOMERECURSOSDESTACADOSDS = "Home_Recursos_Destacados_DS";

        /// <summary>
        /// Prefijo "Home_Usuarios_Destacados_DS"
        /// </summary>
        public static readonly string HOMEUSUARIOSDESTACADOSDS = "Home_Usuarios_Destacados_DS";

        #endregion

        #region TAREAS
        public static readonly string CONTADORTAREA = "ContadorTarea";
        #endregion

        #region Identidad
        /// <summary>
        /// Prefijo "FichaIdentidadMVC"
        /// </summary>
        public static readonly string FichaIdentidadMVC = "FichaIdentidadMVC3";

        /// <summary>
        /// Prefijo "InfoExtraFichaIdentidadMVC"
        /// </summary>
        public static readonly string InfoExtraFichaIdentidadMVC = "InfoExtraFichaIdentidadMVC3";

        /// <summary>
        /// Prefijo "IdentidadActual"
        /// </summary>
        public static readonly string IdentidadActual = "IdentidadActual";
        #endregion

        #region Perfil
        /// <summary>
        /// Prefijo "PerfilMVC"
        /// </summary>
        public static readonly string PerfilMVC = "PerfilMVC";
        #endregion
    }

    /// <summary>
    /// Tipos de clausulas adicionales para el registro de un proyecto.
    /// </summary>
    public enum TipoAccesoRedis
    {
        /// <summary>
        /// Lectura.
        /// </summary>
        Lectura = 0,
        /// <summary>
        /// Escritura.
        /// </summary>
        Escritura = 1,
        /// <summary>
        /// Comprobacion de si existe alguna clave.
        /// </summary>
        Comprobacion = 2,
        /// <summary>
        /// Lectura de varios objetos en cach�.
        /// </summary>
        LecturaVariosObjetos = 3,
        /// <summary>
        /// Lectura de un diccionario de objetos por clave.
        /// </summary>
        LecturaDiccionarioObjetos = 4,
        /// <summary>
        /// Obtiene un rango de elementos.
        /// </summary>
        Rango = 5,
        /// <summary>
        /// Obtiene un rango de elementos ordenados por score.
        /// </summary>
        RangoPorScore = 6,
        /// <summary>
        /// Lectura del n�mero de elementos.
        /// </summary>
        LecturaNumElementos = 7,
        /// <summary>
        /// Eliminacion de elementos del sorted set.
        /// </summary>
        EliminarElementosSortedSet = 8,
        /// <summary>
        /// Clonacci�n del sorted set.
        /// </summary>
        ClonarSortedSet = 9,
        /// <summary>
        /// Lectura de una lista de objetos.
        /// </summary>
        LecturaListaObjetos = 10,
        /// <summary>
        /// Cambio de la caducidad de una clave.
        /// </summary>
        CaducidadAObjeto = 11,
        /// <summary>
        /// Renombrado de una clave de cach�
        /// </summary>
        RenombrarClave = 12,
        /// <summary>
        /// Le agregamos una fecha de caducidad a un objeto.
        /// </summary>
        CaducidadAObjeto_Fecha = 13,
        /// <summary>
        /// Escritura de un objeto por una clave y un score.
        /// </summary>
        EscrituraPorScore = 14,
        /// <summary>
        /// Obtener el score de un objeto.
        /// </summary>
        ObtenerScoreObjeto = 15,
        /// <summary>
        /// Borrado de elementos de la cach�.
        /// </summary>
        Borrado = 16,
        /// <summary>
        /// Escritura de varios objetos en cach�.
        /// </summary>
        EscrituraVariosObjetos = 17,
        /// <summary>
        /// Escritura de varios objetos en cach�.
        /// </summary>
        BorradoVarios = 18,
    }

    /// <summary>
    /// Tipos de clausulas adicionales para el registro de un proyecto.
    /// </summary>
    public enum TipoCacheLocalRedis
    {
        /// <summary>
        /// Diccionario con las rutas registradas
        /// </summary>
        RutasPestanyas = 8,
        /// <summary>
        /// Clave de cach� para recalcular las rutas registradas
        /// </summary>
        RecalcularRutas = 9,
        /// <summary>
        /// Clave de cach� para recalcular los Idiomas de la plataforma
        /// </summary>
        IdiomasPlataforma = 10
    }

    public enum EstadoLoginUsuario
    {
        PuedeIntentarHacerLogin = 1,
        Bloqueado = 2
    }

    public enum UsoCacheLocal
    {
        Nunca = 1,
        SiElServicioLoUsanPocosProyectos = 2,
        Siempre = 3
    }

    [System.ComponentModel.DataObject]
    public abstract class BaseCL : IDisposable
    {
        #region Constantes

        //Redis default port
        private const int REDIS_DEFAULT_PORT = 6379;

        /// <summary>
        /// Duraci�n de la Cach� en segundos
        /// </summary>
        public const double DURACION_CACHE_UN_MES = 60 * 60 * 24 * 30;
        public const double DURACION_CACHE_TRES_DIAS = 60 * 60 * 72;
        public const double DURACION_CACHE_UN_DIA = 60 * 60 * 24;
        public const double DURACION_CACHE_CUATRO_HORAS = 60 * 60 * 4;


        public static readonly string CLAVE_REFRESCO_CACHE_LOCAL = "ClaveRefrescoCacheLocal_";
        public static readonly string CLAVE_DICCIONARIO_REFRESCO_CACHE_VISTAS = "ClaveDiccionarioRefrescoCacheVistas_";
        public static readonly string CLAVE_REFRESCO_CACHE_VISTAS = "ClaveRefrescoCacheVistas_";
        public static readonly string CLAVE_REFRESCO_CACHE_TRADUCCIONES = "ClaveRefrescoCacheTraducciones_";
        public static readonly string CLAVE_REFRESCO_RUTAS_PESTANYAS = "ClaveRefrescoRutasPestanyas_";
        public static readonly string CLAVE_RECALCULO_RUTAS = "ClaveRecalcularRutas";


        //public static string CLAVE_DICCIONARIO_CACHE_DEPENDENCY = "DiccionarioCacheDependencyTipo_";
        //public static string CLAVE_DICCIONARIO_CACHE_LOCAL = "DiccionarioVersionesCacheLocal2_";
        //public static string CLAVE_DICCIONARIO_CACHE_LOCAL_VISTAS = "DiccionarioVersionesCacheLocal_Vistas";
        //public static string CLAVE_DICCIONARIO_CACHE_LOCAL_TRADUCCIONES = "DiccionarioVersionesCacheLocal_Traducciones";
        public static string CLAVE_PROYECTO_DOMINIO_MULTIPLE_CACHE_LOCAL = "ProyectoDominioMultiple_";
        public static UsoCacheLocal UsarCacheLocal = UsoCacheLocal.Nunca;

        #endregion

        #region Miembros

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro ComunidadPadreEcosistemaID (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreProyectoPadreEcositema = null;

        /// <summary>
        /// Nombre del proyecto padre del ecosistema configurado en BD con el parametro NombreCortoProyectoPadreEcosistema (comunidad/nombrecorto)
        /// </summary>
        private static string mNombreCortoProyectoPadreEcosistema = null;

        /// <summary>
        /// ProyectoID padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        private static Guid? mPadreEcosistemaProyectoID = null;

        /// <summary>
        /// Cliente de Redis
        /// </summary>
        private RedisDB mClienteRedisLectura;

        /// <summary>
        /// Cliente de Redis
        /// </summary>
        private RedisDB mClienteRedisEscritura;

        /// <summary>
        /// Fichero de configuraci�n de la base de datos
        /// </summary>
        protected string mFicheroConfiguracionBD = "";

        /// <summary>
        /// Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE
        /// </summary>
        protected bool mUsarVariableEstatica;

        public static string DominioEstatico = "";

        protected string mDominio;

        protected string mPoolName = "";

        protected static string mListaClavesERROR = "";

        private bool mUsarClienteEscritura = false;

        //En el caso del servicio live espiec�fico, cambiarlo para que no use hilos y para que lance excepciones...
        public static bool? mUsarHilos = null;
        public static bool mLanzarExcepciones = false;

        public static int mDecimasSegundoEsperaRedis = 2;

        public static ConcurrentDictionary<string, DateTime> mServidoresRedisCaidos = new ConcurrentDictionary<string, DateTime>();
        public static int mNumCaidasConsecutivas = 0;

        //private static ConcurrentDictionary<string, PooledRedisClientManager> mListaPoolPorIP = new ConcurrentDictionary<string, PooledRedisClientManager>();

        private int? mTamanioPool = null;

        //Bloque de cache cuando est� configurado la cache y redis, para que no se pueda leer a la vez en redis con la misma conexion
        private object mBloqueoCache = new object();

        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region atributos

        protected ConfigService _configService;
        private EntityContext _entityContext;
        protected LoggingService _loggingService;
        private RedisCacheWrapper _redisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin par�metros
        /// </summary>
        public BaseCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<BaseCL> logger, ILoggerFactory loggerFactory)
        {
            _redisCacheWrapper = redisCacheWrapper;
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mlogger = logger;
            this.mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuraci�n</param>
        public BaseCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<BaseCL> logger,ILoggerFactory loggerFactory)
        {
            _redisCacheWrapper = redisCacheWrapper;
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;
            mlogger = logger;
            mLoggerFactory=loggerFactory;
            if (pFicheroConfiguracionBD != null)
            {
                mFicheroConfiguracionBD = pFicheroConfiguracionBD;
            }
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuraci�n</param>
        /// <param name="pPoolName">Nombre del pool</param>
        public BaseCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<BaseCL> logger,ILoggerFactory loggerFactory)
        {
            _redisCacheWrapper = redisCacheWrapper;
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if (pFicheroConfiguracionBD != null)
            {
                mFicheroConfiguracionBD = pFicheroConfiguracionBD;
            }
            if (pPoolName != null)
            {
                mPoolName = pPoolName;
            }
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Comprueba si dos dataset tienen la misma estructura
        /// </summary>
        /// <param name="pDataSetCache">DataSet de cach�</param>
        /// <param name="pDataSetLocal">DataSet local</param>
        /// <returns></returns>
        public bool ComprobarEstructuraDataSet(DataSet pDataSetCache, DataSet pDataSetLocal)
        {
            if (pDataSetCache != null)
            {
                //Solo hay que recargar si en cache falta algun campo de alguna tabla
                foreach (DataTable tabla in pDataSetLocal.Tables)
                {
                    if (pDataSetCache.Tables[tabla.TableName] == null)
                    {
                        AgregarEntradaTraza(string.Format("No sirve el data set de cach� porque no contiene la tabla {0}. Tipo: {1}", tabla.TableName, pDataSetCache.GetType().FullName));
                        return false;
                    }
                    foreach (DataColumn columna in tabla.Columns)
                    {
                        if (!pDataSetCache.Tables[tabla.TableName].Columns.Contains(columna.ColumnName))
                        {
                            AgregarEntradaTraza(string.Format("No sirve el data set de cach� porque no contiene la columna {0} en la tabla {1}. Tipo: {2}", columna.ColumnName, tabla.TableName, pDataSetCache.GetType().FullName));
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public List<string> ObtenerClavesCacheQueContengaCadena(string pCadena)
        {
            string rawKey = "*" + pCadena.ToLower() + "*";
            rawKey = ObtenerClaveCache("*" + pCadena.ToLower() + "*");
            object objeto = InteractuarRedis(TipoAccesoRedis.LecturaListaObjetos, rawKey);
            List<string> claves = new List<string>();
            if (objeto != null)
            {
                claves = (List<string>)objeto;
            }
            return claves;
        }

        /// <summary>
        /// Obtiene la clave
        /// </summary>
        /// <param name="pCacheKey">Clave de cache</param>
        /// <returns></returns>
        public string ObtenerClaveCache(string pCacheKey)
        {
            string clave = "";

            if (!string.IsNullOrEmpty(mDominio))
            {
                clave += mDominio;
            }
            else
            {
                clave += DominioEstatico;
            }

            clave += "_" + ClaveCache[0];

            clave += VersionCache;

            if (!string.IsNullOrEmpty(pCacheKey))
            {
                if (!pCacheKey.StartsWith(clave))
                {
                    clave += "_" + pCacheKey;
                }
                else
                {
                    clave = pCacheKey;
                }
            }

            return clave;
        }

        protected virtual string VersionCache
        {
            get
            {
                return "_5.18.0.0";
            }
        }

        public List<object> ObtenerVariosObjetosDeCache(params string[] pRawKey)
        {
            List<object> listaResultados = new List<object>();
            for (int i = 0; i < pRawKey.Length; i++)
            {
                pRawKey[i] = pRawKey[i].ToLower();
            }

            object objeto = InteractuarRedis(TipoAccesoRedis.LecturaVariosObjetos, "", pRawKey);
            if (objeto != null)
            {
                listaResultados = (List<object>)objeto;
            }

            return listaResultados;
        }

        protected Dictionary<string, object> ObtenerListaObjetosCache(string[] pListaClaves)
        {
            for (int i = 0; i < pListaClaves.Length; i++)
            {
                pListaClaves[i] = pListaClaves[i].ToLower();
            }

            Dictionary<string, object> resultado = new Dictionary<string, object>();

            object objeto = InteractuarRedis(TipoAccesoRedis.LecturaDiccionarioObjetos, "", pListaClaves);
            if (objeto != null)
            {
                resultado = (Dictionary<string, object>)objeto;
            }

            return resultado;
        }

        protected void AgregarListaObjetosCache(string[] pListaClaves, object[] pListaObjetos)
        {
            AgregarListaObjetosCache(pListaClaves, pListaObjetos, true);
        }

        protected void AgregarListaObjetosCache(string[] pListaClaves, object[] pListaObjetos, bool pGenerarClave, double pDuracion = 0)
        {
            string[] clavesOriginales = pListaClaves;
            object[] objetosOriginales = pListaObjetos;

            Dictionary<string, object> claveObjeto = new Dictionary<string, object>();

            if (pGenerarClave)
            {
                for (int i = 0; i < clavesOriginales.Length; i++)
                {
                    pListaClaves[i] = ObtenerClaveCache(clavesOriginales[i]).ToLower();

                    claveObjeto.Add(pListaClaves[i], "");
                }
            }
            else
            {
                for (int i = 0; i < pListaClaves.Length; i++)
                {
                    claveObjeto.Add(pListaClaves[i], "");
                }
            }

            for (int i = 0; i < objetosOriginales.Length; i++)
            {
                if (objetosOriginales[i] is DataSet objetoOriginal)
                {
                    switch (objetoOriginal.DataSetName)
                    {
                        case "ProyectoDS":
                        case "IdentidadDS":
                        case "PersonaDS":
                        case "OrganizacionDS":
                        case "AmigosDS":
                        case "DocumentacionDS":
                        case "TesauroDS":
                            AgregarEntradaTraza("Redis. Obtengo xml DataSet");
                            string xmlDataSet = (objetoOriginal).GetXml().Replace("\n", "").Replace("\r", "").Replace(">    <", "><").Replace(">  <", "><");
                            pListaObjetos[i] = "[DATASET]" + Zip(xmlDataSet);
                            break;
                    }
                }
                claveObjeto[pListaClaves[i]] = pListaObjetos[i];
            }

            object[] parametrosExtra = new object[2];
            parametrosExtra[0] = pDuracion;
            parametrosExtra[1] = claveObjeto;
            InteractuarRedis(TipoAccesoRedis.EscrituraVariosObjetos, "", parametrosExtra);
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pTipo">Especifica el tipo con el que se deserializar�</param>
        /// <returns>Objeto</returns>
        public object ObtenerObjetoDeCache(string pRawKey, Type pTipo)
        {
            return ObtenerObjetoDeCache(pRawKey, true, pTipo);
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        /// <param name="pTipo">Especifica el tipo con el que se deserializar�</param>
        /// <returns>Objeto</returns>
        public object ObtenerObjetoDeCache(string pRawKey, bool pGenerarClave, Type pTipo)
        {
            if (pGenerarClave && string.IsNullOrEmpty(mDominio) && string.IsNullOrEmpty(DominioEstatico))
            {
                return null;
            }

            string rawKeyOriginal = pRawKey;

            if (pGenerarClave)
            {
                pRawKey = ObtenerClaveCache(pRawKey);
            }
            pRawKey = pRawKey.ToLower();

            object resultado = InteractuarRedis(TipoAccesoRedis.Lectura, pRawKey, rawKeyOriginal, pGenerarClave, pTipo);

            return resultado;
        }

        /// <summary>
        /// obtiene el numero de elementos del sorted set
        /// </summary>
        /// <param name="pRawKey"></param>
        /// <returns>entero</returns>
        public int ObtenerNumElementosDeSortedSet(string pRawKey)
        {
            pRawKey = ObtenerClaveCache(pRawKey).ToLower();

            int numElementos = 0;
            object objeto = InteractuarRedis(TipoAccesoRedis.LecturaNumElementos, pRawKey);
            if (objeto != null)
            {
                numElementos = Convert.ToInt32(objeto);
            }

            return numElementos;
        }

        /// <summary>
        /// elimina elementos del sorted set desde la posicion de Inicio hasta la posicion fin
        /// </summary>
        /// <param name="pRawKey"></param>
        /// <param name="pInicio"></param>
        /// <param name="pFin"></param>
        public void EliminaElementosDeSortedSet(string pRawKey, int pInicio, int pFin)
        {
            pRawKey = ObtenerClaveCache(pRawKey).ToLower();

            InteractuarRedis(TipoAccesoRedis.EliminarElementosSortedSet, pRawKey, pInicio, pFin);
        }

        public async void FlushDB()
        {
            FLUSHDB cmd = new FLUSHDB();
            Result result = await ClienteRedisLectura.Execute(cmd, typeof(string));
            if (result.IsError)
            {
                throw new RedisException(result.Messge);
            }
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        /// <returns>Objeto</returns>
        protected List<object> ObtenerRangoDeSortedList(string pRawKey, int pNumeroElementos)
        {
            return ObtenerRangoDeSortedList(pRawKey, 1, pNumeroElementos);
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        /// <returns>Objeto</returns>
        protected List<object> ObtenerRangoDeSortedList(string pRawKey, int pInicio, int pFin)
        {
            string rawKeyOriginal = pRawKey;
            pRawKey = ObtenerClaveCache(pRawKey).ToLower();

            List<object> listaResultado = new List<object>();
            object objetoIR = InteractuarRedis(TipoAccesoRedis.Rango, pRawKey, rawKeyOriginal, pInicio, pFin);

            List<(double, string)> objetosCache = null;
            if (objetoIR != null)
            {
                objetosCache = (List<(double, string)>)objetoIR;
            }

            if (objetosCache != null && objetosCache.Count > 0)
            {
                AgregarEntradaTraza("Redis. Existe " + pRawKey);
                foreach ((double, string) objetoCache in objetosCache)
                {
                    object objeto = objetoCache.Item2;
                    AgregarEntradaTraza("Redis. Obtenido: " + pRawKey);
                    listaResultado.Add(objeto);
                }
            }
            else
            {
                AgregarEntradaTraza("Redis. No est� en cach�: " + pRawKey);
            }

            if (listaResultado != null)
            {
                listaResultado.Reverse();
            }

            return listaResultado;
        }

        /// <summary>
        /// Obtiene un objeto de la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        /// <returns>Objeto</returns>
        protected List<object> ObtenerRangoDeSortedListPorScore(string pRawKey, int pMinScore, int pNumElementos)
        {
            string rawKeyOriginal = pRawKey;
            pRawKey = ObtenerClaveCache(pRawKey).ToLower();

            List<object> listaResultado = new List<object>();

            object objetoIR = InteractuarRedis(TipoAccesoRedis.RangoPorScore, pRawKey, rawKeyOriginal, pMinScore, pNumElementos);

            List<(double, string)> objetosCache = null;
            if (objetoIR != null)
            {
                objetosCache = (List<(double, string)>)objetoIR;
            }

            if (objetosCache != null && objetosCache.Count > 0)
            {
                AgregarEntradaTraza("Redis. Existe " + pRawKey);
                foreach ((double, string) objetoCache in objetosCache)
                {
                    object objeto = objetoCache.Item2;
                    AgregarEntradaTraza("Redis. Obtenido: " + pRawKey);
                    listaResultado.Add(objeto);
                }
            }
            else
            {
                AgregarEntradaTraza("Redis. No est� en cach�: " + pRawKey);
            }


            if (listaResultado != null)
            {
                listaResultado.Reverse();
            }

            return listaResultado;
        }

        /// <summary>
        /// Comprueba si existe una clave de cach�.
        /// </summary>
        /// <param name="pRawKey">Clav� de cach� que se desea buscar.</param>
        /// <returns>TRUE si la clave existe, FALSE en caso contrario.</returns>
        public bool ExisteClaveEnCache(string pRawKey)
        {
            bool existe = false;
            string rawKeyOriginal = pRawKey;

            pRawKey = ObtenerClaveCache(pRawKey);
            pRawKey = pRawKey.ToLower();

            object booleano = InteractuarRedis(TipoAccesoRedis.Comprobacion, pRawKey, rawKeyOriginal);

            if (booleano != null)
            {
                existe = true;
                if (booleano.Equals(0) || booleano.Equals((long)0) || booleano.Equals(false))
                {
                    existe = false;
                }
            }


            return existe;
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pDuracion">Duraci�n del objeto de cach� en segundos</param>
        public void AgregarCaducidadAObjetoCache(string pRawKey, double pDuracion)
        {
            InteractuarRedis(TipoAccesoRedis.CaducidadAObjeto, pRawKey, pDuracion);
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        public string AgregarObjetoCache(string pRawKey, object pObjeto)
        {
            return AgregarObjetoCache(pRawKey, pObjeto, 0);
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pDuracion">Duraci�n del objeto de cach� en segundos</param>
        public string AgregarObjetoCache(string pRawKey, object pObjeto, double pDuracion)
        {
            return AgregarObjetoCache(pRawKey, pObjeto, pDuracion, true);
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pDuracion">Duraci�n del objeto de cach� en segundos</param>
        public string AgregarObjetoCache(string pRawKey, object pObjeto, double pDuracion, bool pGenerarClave)
        {
            if (pGenerarClave && string.IsNullOrEmpty(mDominio) && string.IsNullOrEmpty(DominioEstatico))
            {
                return string.Empty;
            }

            //pDuracion = 0;
            string rawKeyOriginal = pRawKey;
            object objetoOriginal = pObjeto;

            if (pGenerarClave)
            {
                pRawKey = ObtenerClaveCache(pRawKey);
            }
            pRawKey = pRawKey.ToLower();

            if (pObjeto != null)
            {
                if (pObjeto is DataSet)
                {
                    switch (((DataSet)pObjeto).DataSetName)
                    {
                        case "ProyectoDS":
                        case "IdentidadDS":
                        case "PersonaDS":
                        case "OrganizacionDS":
                        case "AmigosDS":
                        case "DocumentacionDS":
                        case "TesauroDS":
                            AgregarEntradaTraza("Redis. Obtengo xml DataSet");
                            string xmlDataSet = UtilCadenas.PasarDataSetToString((DataSet)pObjeto);

                            pObjeto = "[DATASET]" + Zip(xmlDataSet);
                            break;
                    }
                }

                InteractuarRedis(TipoAccesoRedis.Escritura, pRawKey, rawKeyOriginal, pObjeto, pDuracion);
            }

            return pRawKey;
        }

        /// <summary>
        /// Comprueba si una clave de cach� existe.
        /// </summary>
        /// <param name="pRawKey">Clave de cach� que se va a comprobar si existe.</param>
        protected bool ComprobarSiClaveExiste(string pRawKey)
        {
            //pDuracion = 0;
            pRawKey = ObtenerClaveCache(pRawKey);
            pRawKey = pRawKey.ToLower();

            bool existe = false;
            try
            {
                AgregarEntradaTraza("Redis. Compruebo si la clave '" + pRawKey + "' existe");

                object objeto = InteractuarRedis(TipoAccesoRedis.Comprobacion, pRawKey, pRawKey);
                if (objeto != null)
                {
                    existe = (Int64)objeto == 1;
                }
                //existe = (ClienteRedisEscritura.Exists(pRawKey) > 0);
                AgregarEntradaTraza("Redis. El objeto con la clave '" + pRawKey + "' existe.");
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, null,mlogger ,true);
                //CerrarConexionCache();
            }

            return existe;
        }

        /// <summary>
        /// Renombrado de una clave de cach� por otra
        /// </summary>
        /// <param name="pRawKey">Clav� de cache vieja</param>
        /// <param name="pRawKeyNueva">Clav� de cache Nueva</param>
        protected void RenombrarClave(string pRawKey, string pRawKeyNueva)
        {
            //pDuracion = 0;
            pRawKey = ObtenerClaveCache(pRawKey);
            pRawKey = pRawKey.ToLower();

            pRawKeyNueva = ObtenerClaveCache(pRawKeyNueva);
            pRawKeyNueva = pRawKeyNueva.ToLower();

            InteractuarRedis(TipoAccesoRedis.RenombrarClave, pRawKey, pRawKeyNueva);
        }

        protected int ClonarSortedSet(string pRawKeyOrigen, string pRawKeyDestino)
        {
            pRawKeyOrigen = ObtenerClaveCache(pRawKeyOrigen);
            pRawKeyOrigen = pRawKeyOrigen.ToLower();

            pRawKeyDestino = ObtenerClaveCache(pRawKeyDestino);
            pRawKeyDestino = pRawKeyDestino.ToLower();

            object objeto = InteractuarRedis(TipoAccesoRedis.ClonarSortedSet, pRawKeyOrigen, pRawKeyDestino);

            int score = 0;
            if (objeto != null)
            {
                score = (int)objeto;
            }

            return score;
        }

        /// <summary>
        /// Agrega un objeto a la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pScore">Score de este objeto</param>
        /// <returns>Score asignado</returns>
        protected int AgregarObjetoASortedSet(string pRawKey, object pObjeto, int pScore)
        {
            pRawKey = ObtenerClaveCache(pRawKey).ToLower();

            byte[] bytesObjeto = ObjectToByteArray(pObjeto);
            AgregarEntradaTraza("Redis. Agrego a cach�" + pRawKey);

            if (pScore.Equals(-1))
            {
                List<(double, string)> ultimoObjeto = null;

                //ultimoObjeto = ClienteRedisEscritura.ZRange(pRawKey, -1, -1);
                object objeto = InteractuarRedis(TipoAccesoRedis.Rango, pRawKey, pRawKey, 1, 1);
                if (objeto != null)
                {
                    ultimoObjeto = (List<(double, string)>)objeto;
                }

                AgregarEntradaTraza("Redis. Existe " + pRawKey);

                if (ultimoObjeto != null && ultimoObjeto.Count > 0)
                {
                    //pScore = (int)ClienteRedisEscritura.ZScore(pRawKey, ultimoObjeto[0]) + 1;
                    object objeto2 = InteractuarRedis(TipoAccesoRedis.ObtenerScoreObjeto, pRawKey, ultimoObjeto[0]);
                    pScore = (int)(double)objeto2 + 1;
                }
                else
                {
                    pScore = 1;
                }
            }

            //ClienteRedisEscritura.ZAdd(pRawKey, pScore, bytesObjeto);
            InteractuarRedis(TipoAccesoRedis.EscrituraPorScore, pRawKey, pScore, pObjeto);

            AgregarEntradaTraza("Redis. Agregado: " + pRawKey);

            return pScore;
        }

        /// <summary>
        /// Elimina un objeto de la cach�
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <returns>Score asignado</returns>
        protected bool EliminarObjetoDeSortedSet(string pRawKey, string pObjeto)
        {
            bool correcto = false;
            pRawKey = ObtenerClaveCache(pRawKey);
            pRawKey = pRawKey.ToLower();
            try
            {
                AgregarEntradaTraza("Redis. Elimino de cach� " + pRawKey);
                ClienteRedisEscritura.CreateSequence(pRawKey).ZRem(pObjeto);

                AgregarEntradaTraza("Redis. Eliminado: " + pRawKey);
                correcto = true;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, mlogger);
                throw;
            }

            return correcto;
        }

        /// <summary>
        /// Invalida la cach�
        /// </summary>
        public void InvalidarCache()
        {
            // Remove the cache dependency
            InvalidarCache(ClaveCache[0], true);
        }

        /// <summary>
        /// Quita un elemento de la cach�
        /// </summary>
        /// <param name="pClaveCache">Clave del elemento que se va a quitar</param>
        public void InvalidarCache(string pClaveCache)
        {
            InvalidarCache(pClaveCache, true);
        }

        /// <summary>
        /// Quita un elemento de la cach�
        /// </summary>
        /// <param name="pClaveCache">Clave del elemento que se va a quitar</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        public void InvalidarCache(string pClaveCache, bool pGenerarClave)
        {
            InvalidarCache(pClaveCache, pGenerarClave, true);
        }

        /// <summary>
        /// Quita un elemento de la cach�
        /// </summary>
        /// <param name="pClaveCache">Clave del elemento que se va a quitar</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        public void InvalidarCache(string pClaveCache, bool pGenerarClave, bool pReintentar)
        {
            string claveCacheOriginal = pClaveCache;
            if (pGenerarClave)
            {
                pClaveCache = ObtenerClaveCache(pClaveCache);
            }
            pClaveCache = pClaveCache.ToLower();

            InteractuarRedis(TipoAccesoRedis.Borrado, pClaveCache, claveCacheOriginal, pGenerarClave, pReintentar);

            AgregarEntradaTraza("Redis. Invalidado: " + pClaveCache);
        }

        /// <summary>
        /// Quita un elemento de la cach�
        /// </summary>
        /// <param name="pClaveCache">Clave del elemento que se va a quitar</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        public void InvalidarCache(List<string> pClavesCache)
        {
            InteractuarRedis(TipoAccesoRedis.BorradoVarios, "", pClavesCache);

            AgregarEntradaTraza("Redis. Invalidadas varias claves: " + pClavesCache.Count);
        }

        public void InvalidarCacheQueContengaCadena(string pCadena, bool generarClaves = true)
        {
            string rawKey = "*" + pCadena.ToLower() + "*";
            if (generarClaves)
            {
                rawKey = ObtenerClaveCache("*" + pCadena.ToLower() + "*");
            }
            object objeto = InteractuarRedis(TipoAccesoRedis.LecturaListaObjetos, rawKey);
            List<string> claves = new List<string>();
            if (objeto != null)
            {
                claves = (List<string>)objeto;
            }

            InvalidarCachesMultiples(claves);
        }

        /// <summary>
        /// Quita un elemento de la cach�
        /// </summary>
        /// <param name="pListaClavesCache">Claves de los elementos que se van a quitar</param>
        public void InvalidarCachesMultiples(List<string> pListaClavesCache)
        {
            try
            {
                AgregarEntradaTraza("Redis. Invalido cach�s multiples");
                //foreach (string clave in pListaClavesCache)
                //{
                //    var t = ClienteRedisEscritura.Del(clave.ToLower()).Result;
                //}
                if (pListaClavesCache.Any())
                {
                    var t = ClienteRedisEscritura.Del(pListaClavesCache.Select(x => x.ToLower()).ToArray()).Result;
                }
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, null, mlogger, true);

                //CerrarConexionCache();
            }

            AgregarEntradaTraza("Redis. Invalidadas cach�s multiples");
        }

        /// <summary>
        /// Convierte un object en un array de byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            try
            {
                var options = new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All,
                };
                //string json = JsonSerializer.Serialize(obj, options);
                string json = JsonConvert.SerializeObject(obj, options);
                return BsonDocument.Parse(json).ToBson();
            }
            catch (Exception ex)
            {
                return MessagePackSerializer.Serialize(obj);
            }
        }

        /// <summary>
        /// Convierte un array de byte en un object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        protected object ByteArrayToObject(byte[] arrBytes, Type pType = null)
        {
            object obj = null;

            if(pType == null)
            {
                pType = typeof(object);
            }

            if (arrBytes != null)
            {
                try
                {
                    BsonDocument bsonDoc = BsonSerializer.Deserialize<BsonDocument>(arrBytes);
                    string json = bsonDoc.ToJson();
                    var options = new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                        TypeNameHandling = TypeNameHandling.All,
                    };
                    obj = JsonConvert.DeserializeObject(json, pType, options);
                }
                catch (Exception ex)
                {
                    // Excepciones que salta al intentar deserializar con bson
                    // un dato serializado con messagepack
                    if(ex is EndOfStreamException || ex is FormatException)
                    {
                        try
                        {
                            obj = MessagePackSerializer.Deserialize(pType, arrBytes);
                        }
                        catch
                        {
                            //Ha Cambiado el modelo de datos de la cache, devolvemos null para que lo obtenga de Base de datos
                            return null;
                        }
                    }
                    else
                    {
                        _loggingService.GuardarLogError(ex, "Error al deserializar con BSON una cache",mlogger);
                        return null;
                    }
                }
            }
            return obj;
        }

        public static string Zip(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            MemoryStream ms = new MemoryStream();

            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;

            byte[] compressed = new byte[ms.Length];

            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];

            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);

            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);

            ms.Close();

            return Convert.ToBase64String(gzBuffer);
        }

        public static string UnZip(string compressedText)
        {
            byte[] gzBuffer = Convert.FromBase64String(compressedText);

            MemoryStream ms = null;

            try
            {
                ms = new MemoryStream();

                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;

                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                }
            }
        }




        public class PoolRedis
        {
            public string NombreConexion { get; set; }
            public List<ConexionRedis> ListaConexiones { get; set; }
            public class ConexionRedis
            {
                public string IP { get; set; }
                public short BD { get; set; }
                public int TimeOut { get; set; }
                public bool Caido { get; set; }
            }
        }


        protected void AgregarEntradaTraza(string pMensaje)
        {
            _loggingService.AgregarEntrada(pMensaje);
        }

        private bool EsAccionLectura(TipoAccesoRedis pTipoAccesoRedis)
        {
            return (pTipoAccesoRedis.Equals(TipoAccesoRedis.Comprobacion) || pTipoAccesoRedis.Equals(TipoAccesoRedis.Lectura) || pTipoAccesoRedis.Equals(TipoAccesoRedis.LecturaDiccionarioObjetos) || pTipoAccesoRedis.Equals(TipoAccesoRedis.LecturaListaObjetos) || pTipoAccesoRedis.Equals(TipoAccesoRedis.LecturaNumElementos) || pTipoAccesoRedis.Equals(TipoAccesoRedis.LecturaVariosObjetos) || pTipoAccesoRedis.Equals(TipoAccesoRedis.ObtenerScoreObjeto) || pTipoAccesoRedis.Equals(TipoAccesoRedis.Rango) || pTipoAccesoRedis.Equals(TipoAccesoRedis.RangoPorScore));
        }

        /// <summary>
        /// M�todo para interactuar con redis creando un hilo y aguardando un m�ximo de 2 d�cimas de segundo antes de abortar la conexi�n con redis.
        /// </summary>
        private object InteractuarRedis(TipoAccesoRedis pTipoAccesoRedis, string pRawKey, params object[] pParametrosExtra)
        {
            bool terminado = false;
            object resultado = null;

            DateTime inicioHilo = DateTime.Now;
            DateTime finHilo = DateTime.Now;

            pRawKey = pRawKey.ToLower();

            //Elimina el redis del diccionario "mDicRedisCaidos" si ha pasado 1 minuto.
            ComprobarEstadoRedis();

            //if (ClienteRedisLectura != null && mServidoresRedisCaidos.ContainsKey(ClienteRedisLectura.Host))
            {
                //Tratamos de cambiar el Host del cliente de lectura y escritura
                //mClienteRedisLectura = null;
                //UtilPeticion.EliminarObjetoDePeticion("mClienteRedisLectura" + mPoolName);
            }

            RedisDB clienteRedis = ClienteRedisLectura;

            if (!EsAccionLectura(pTipoAccesoRedis))
            {
                clienteRedis = ClienteRedisEscritura;
            }

            if (clienteRedis != null)
            {
                //TODO Revisar el HOST
                //if (mNumCaidasConsecutivas <= 3 && !mServidoresRedisCaidos.ContainsKey(clienteRedis.Host))
                {
                    Thread t = null;
                    if (!UsarHilos)
                    {
                        lock (mBloqueoCache)
                        {
                            resultado = ConsultasRedisPorTipo(pTipoAccesoRedis, pRawKey, out terminado, pParametrosExtra);
                        }
                    }
                    else
                    {
                        // Uso de threadpool con queues. http://redproyectos.gnoss.com/comunidad/desarrollo-de-gnoss-sistema-s/recurso/hilos-uso-de-threadpools-con-esperas/57a11fe2-7e40-4d34-8d17-c2c4443a43d9
                        Action wrappedAction = () =>
                        {
                            resultado = ConsultasRedisPorTipo(pTipoAccesoRedis, pRawKey, out terminado, pParametrosExtra);
                            finHilo = DateTime.Now;
                        };

                        //Usar hilo normal, si no termina se hace un abort.
                        t = new Thread(new ThreadStart(wrappedAction));
                        t.Start();
                        t.Join(mDecimasSegundoEsperaRedis * 100);
                    }

                    if (!terminado)
                    {
                        //Cancelamos el hilo.
                        AgregarEntradaTraza("Redis. Acci�n redis '" + pTipoAccesoRedis + "' superior a " + mDecimasSegundoEsperaRedis * 100 + " milisegundos. " + pRawKey);
                    }
                    else
                    {
                        AgregarEntradaTraza("Redis. Acci�n redis '" + pTipoAccesoRedis + "' en " + (inicioHilo - finHilo) + " segundos. " + pRawKey);
                    }
                }
            }
            else if (mLanzarExcepciones)
            {
                throw new RedisException("No se ha podido establecer una conexi�n a Redis.");
            }

            return resultado;
        }

        private void ComprobarSiHayQueUsarHilos()
        {
            string usarHilos = _configService.ObtenerUsarHilosInteractuarRedis();
            bool resultado = false;

            if (!string.IsNullOrEmpty(usarHilos))
            {
                bool.TryParse(usarHilos, out resultado);
            }

            AgregarEntradaTraza("Redis. Uso de hilos = " + usarHilos);
            mUsarHilos = resultado;
        }

        /// <summary>
        /// Comprueba si ya ha pasado 1 minuto y se puede quitar el redis que estaba ca�do del diccionario.
        /// </summary>
        private void ComprobarEstadoRedis()
        {
            //Revisamos el estado de los redis, si ha pasado m�s de un minuto los quitamos del diccionario.
            List<string> tempServerList = new List<string>();

            foreach (string nodoIP in mServidoresRedisCaidos.Keys)
            {
                if (!string.IsNullOrEmpty(nodoIP) && mServidoresRedisCaidos[nodoIP].AddMinutes(1) <= DateTime.Now)
                {
                    tempServerList.Add(nodoIP);
                }
            }

            foreach (string tempIP in tempServerList)
            {
                DateTime fecha = new DateTime();
                mServidoresRedisCaidos.TryRemove(tempIP, out fecha);
                mNumCaidasConsecutivas = 0;
                AgregarEntradaTraza("Redis. Volvemos a consultar a Redis '" + tempIP + "'");
            }
        }

        private object ConsultasRedisPorTipo(TipoAccesoRedis pTipoAccesoRedis, string pRawKey, out bool terminado, params object[] pParametrosExtra)
        {
            object resultado = null;

            try
            {
                string pRawKeyOriginal = "";
                bool pGenerarClave = false;
                double pDuracion = 0;
                int pInicio = 0;
                int pFin = 0;
                object pObjeto = null;                
                Type[] ptype = null;
                string pObjetoString = null;
                string[] pRawkeyArray = null;
                bool pReintentar = false;
                Type pTipo = null;

                switch (pTipoAccesoRedis)
                {
                    case TipoAccesoRedis.Comprobacion:
                        pRawKeyOriginal = pParametrosExtra[0].ToString();
                        resultado = InteractuarRedis_Comprobacion(pRawKey, pRawKeyOriginal);
                        break;

                    case TipoAccesoRedis.Lectura:
                        if (pParametrosExtra[0] != null)
                        {
                            pRawKeyOriginal = pParametrosExtra[0].ToString();
                        }
                        if (pParametrosExtra[2] != null)
                        {
                            pTipo = pParametrosExtra[2] as Type;
                        }
                        pGenerarClave = (bool)pParametrosExtra[1];
                        resultado = InteractuarRedis_Lectura(pRawKey, pRawKeyOriginal, pGenerarClave, pTipo);
                        break;
                    case TipoAccesoRedis.Escritura:
                        if (pParametrosExtra[0] != null)
                        {
                            pRawKeyOriginal = pParametrosExtra[0].ToString();
                        }
                        pObjeto = pParametrosExtra[1];
                        pDuracion = double.Parse(pParametrosExtra[2].ToString());
                        InteractuarRedis_Escritura(pRawKey, pRawKeyOriginal, pObjeto, pDuracion);
                        break;
                    case TipoAccesoRedis.LecturaVariosObjetos:
                        pRawkeyArray = (string[])pParametrosExtra[0];
                        ptype = (Type[])pParametrosExtra[1];
                        resultado = InteractuarRedis_LecturaVariosObjetos(pRawkeyArray, ptype);
                        break;
                    case TipoAccesoRedis.EscrituraVariosObjetos:
                        InteractuarRedis_EscrituraDiccionarioObjetos((double)pParametrosExtra[0], (Dictionary<string, object>)pParametrosExtra[1]);
                        break;
                    case TipoAccesoRedis.LecturaDiccionarioObjetos:
                        pRawkeyArray = (string[])pParametrosExtra;
                        resultado = InteractuarRedis_LecturaDiccionarioObjetos(pRawkeyArray);
                        break;
                    case TipoAccesoRedis.Rango:
                        pRawKeyOriginal = pParametrosExtra[0].ToString();
                        pInicio = int.Parse(pParametrosExtra[1].ToString());
                        pFin = int.Parse(pParametrosExtra[2].ToString());
                        resultado = InteractuarRedis_Rango(pRawKey, pRawKeyOriginal, pInicio, pFin);
                        break;
                    case TipoAccesoRedis.RangoPorScore:
                        pRawKeyOriginal = pParametrosExtra[0].ToString();
                        pInicio = int.Parse(pParametrosExtra[1].ToString());
                        pFin = int.Parse(pParametrosExtra[2].ToString());
                        resultado = InteractuarRedis_RangoPorScore(pRawKey, pRawKeyOriginal, pInicio, pFin);
                        break;
                    case TipoAccesoRedis.LecturaNumElementos:
                        resultado = ClienteRedisLectura.CreateSequence(pRawKey).ZCard().Result;
                        break;
                    case TipoAccesoRedis.EliminarElementosSortedSet:
                        pInicio = int.Parse(pParametrosExtra[0].ToString());
                        pFin = int.Parse(pParametrosExtra[1].ToString());
                        ClienteRedisEscritura.CreateSequence(pRawKey).ZRemRangeByRank(pInicio, pFin);
                        break;
                    case TipoAccesoRedis.ClonarSortedSet:
                        int score = 0;
                        string rawKeyOrigen = pRawKey;
                        string rawKeyDestino = pParametrosExtra[0].ToString();
                        List<(double, string)> bytesCache = ClienteRedisLectura.CreateSequence(rawKeyOrigen).ZRange(0, -1).Result;
                        foreach ((double, string) elemento in bytesCache)
                        {
                            resultado = ClienteRedisEscritura.CreateSequence(rawKeyDestino).ZAdd((score, elemento.Item2)).Result;
                            score++;
                        }
                        resultado = score;
                        break;
                    case TipoAccesoRedis.LecturaListaObjetos:
                        resultado = ClienteRedisLectura.Keys(pRawKey).Result.ToList();
                        break;
                    case TipoAccesoRedis.CaducidadAObjeto:
                        pDuracion = double.Parse(pParametrosExtra[0].ToString());
                        ClienteRedisEscritura.Expire(pRawKey, (int)pDuracion);
                        break;
                    case TipoAccesoRedis.RenombrarClave:
                        string pRawKeyNueva = pParametrosExtra[0].ToString();
                        InteractuarRedis_RenombrarClave(pRawKey, pRawKeyNueva);
                        break;
                    case TipoAccesoRedis.EscrituraPorScore:
                        double pScore = double.Parse(pParametrosExtra[0].ToString());
                        pObjetoString = pParametrosExtra[1].ToString();
                        resultado = ClienteRedisEscritura.CreateSequence(pRawKey).ZAdd((pScore, pObjetoString)).Result;
                        break;
                    case TipoAccesoRedis.ObtenerScoreObjeto:
                        pObjetoString = pParametrosExtra[0].ToString();
                        resultado = ClienteRedisEscritura.CreateSequence(pRawKey).ZScore((((double, string))pParametrosExtra[0]).Item2).Result;
                        break;
                    case TipoAccesoRedis.Borrado:
                        if (pParametrosExtra[0] != null)
                        {
                            pRawKeyOriginal = pParametrosExtra[0].ToString();
                        }
                        pGenerarClave = (bool)pParametrosExtra[1];
                        pReintentar = (bool)pParametrosExtra[2];
                        InteractuarRedis_Borrado(pRawKey, pRawKeyOriginal, pGenerarClave, pReintentar);
                        break;
                    case TipoAccesoRedis.BorradoVarios:
                        List<string> rawKeys = (List<string>)pParametrosExtra[0];
                        InteractuarRedis_BorradoVariasClaves(rawKeys);
                        break;
                }
                mNumCaidasConsecutivas = 0;
            }
            catch (Exception ex)
            {
                //Llamamos a un m�todo diferente para no mezclar los errores de redis con los de la web.
                _loggingService.GuardarLogErrorRedis(ex, "La key que intenta obtener es la siguiente: " + pRawKey, true);
                //CerrarConexionCache();
                mNumCaidasConsecutivas++;

                if (mLanzarExcepciones)
                {
                    throw;
                }
            }
            finally
            {
                terminado = true;
                if (mNumCaidasConsecutivas >= 3)
                {
                    if (ClienteRedisLectura != null && ClienteRedisEscritura != null)
                    {
                        //if (!mServidoresRedisCaidos.ContainsKey(ClienteRedisLectura.Host))
                        {
                            try
                            {
                                //mServidoresRedisCaidos.TryAdd(ClienteRedisLectura.Host, DateTime.Now);
                            }
                            catch { }
                        }
                    }
                }

            }

            return resultado;
        }

        private void InteractuarRedis_BorradoVariasClaves(List<string> pRawKeys)
        {
            AgregarEntradaTraza("Redis. Invalido cach� de varias claves: " + pRawKeys.Count);
            foreach (string key in pRawKeys)
            {
                ClienteRedisEscritura.Del(key);
            }
        }

        private void InteractuarRedis_Borrado(string pRawKey, string pRawKeyOriginal, bool pGenerarClave, bool pReintentar)
        {
            AgregarEntradaTraza("Redis. Invalido cach� " + pRawKey);
            long resultado = ClienteRedisEscritura.Del(pRawKey).Result;
        }

        private void InteractuarRedis_RenombrarClave(string pRawKey, string pRawKeyNueva)
        {
            AgregarEntradaTraza("Redis. Renombro la calve '" + pRawKey + "' por '" + pRawKeyNueva + "'");
            ClienteRedisEscritura.Rename(pRawKey, pRawKeyNueva);
            AgregarEntradaTraza("Redis. Remplazo con �xito.");
        }

        private object InteractuarRedis_RangoPorScore(string pRawKey, string pRawKeyOriginal, int pMinScore, int pNumElementos)
        {
            List<(double, string)> bytesCache = ClienteRedisLectura.CreateSequence(pRawKey).ZRangeByScore(pMinScore.ToString(), pNumElementos.ToString()).Result;
            return bytesCache;
        }

        private object InteractuarRedis_Rango(string pRawKey, string pRawKeyOriginal, int pInicio, int pFin)
        {
            List<(double, string)> bytesCache = ClienteRedisLectura.CreateSequence(pRawKey).ZRange(-pFin, -pInicio).Result;
            return bytesCache;
        }

        private object InteractuarRedis_LecturaDiccionarioObjetos(string[] pRawkeyArray)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();

            AgregarEntradaTraza("Redis. Obtengo lista de claves de cach� " + pRawkeyArray[0] + " ...");
            Type[] tipos = new Type[pRawkeyArray.Length];
            for (int i = 0; i < tipos.Length; i++)
            {
                tipos[i] = typeof(byte[]);
            }

            object[] objetosCache = ClienteRedisLectura.MGet(pRawkeyArray, tipos).Result;

            for (int cont = 0; cont < pRawkeyArray.Length; cont++)
            {
                string clave = pRawkeyArray[cont];
                object objetoCache = objetosCache[cont];
                if (objetoCache is byte[] bytesObjeto)
                {
                    objetoCache = ByteArrayToObject(bytesObjeto);
                }

                if (!resultado.ContainsKey(clave))
                {
                    resultado.Add(clave, objetoCache);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Escribe varios objetos en Redis a partir de un diccionario clave-valor donde la clave es la clave de Redis y el valor el objeto a almacenar.
        /// </summary>
        /// <param name="pDuracion">Duraci�n que queremos establecer a cada elemento de la cach�</param>
        /// <param name="pClaveValor">Diccionario donde la clave es la clave de Redis y el valor es el objeto a almacenar</param>
        private void InteractuarRedis_EscrituraDiccionarioObjetos(double pDuracion, Dictionary<string, object> pClaveValor)
        {
            (string, object)[] listaObjetos = new (string, object)[pClaveValor.Keys.Count];
            
            int i = 0;
            foreach (string clave in pClaveValor.Keys)
            {
                listaObjetos[i].Item1 = clave;
                listaObjetos[i].Item2 = ObjectToByteArray(pClaveValor[clave]);

                i++;
            }

            ClienteRedisEscritura.MSet(listaObjetos);

            if (pDuracion > 0)
            {
                foreach (string clave in pClaveValor.Keys)
                {
                    ClienteRedisEscritura.Expire(clave, (int)pDuracion);
                }
            }
        }

        private object InteractuarRedis_LecturaVariosObjetos(string[] pRawkeyArray, Type[] ptypes)
        {
            List<object> listaResultados = new List<object>();
            object[] objetosCache = ClienteRedisLectura.MGet(pRawkeyArray, [typeof(byte[])]).Result;
            AgregarEntradaTraza("Redis. Obtengo clave de cach�");
            if (objetosCache != null && objetosCache.Length > 0)
            {
                AgregarEntradaTraza("Redis. Existe");
                int index = 0;
                foreach (object objeto in objetosCache)
                {
                    if (objeto is byte[] bytesObjeto)
                    {                        
                        object objetoCache = ByteArrayToObject(bytesObjeto);
                        AgregarEntradaTraza("Redis. Obtenido: " + pRawkeyArray);
                        listaResultados.Add(objetoCache);
                    }
                    index++;
                }
            }
            else
            {
                AgregarEntradaTraza("Redis. No est� en cach�: " + pRawkeyArray);
            }

            return listaResultados;
        }

        private void InteractuarRedis_Escritura(string pRawKey, string pRawKeyOriginal, object pObjetoCache, double pDuracion)
        {
            AgregarEntradaTraza("Redis. Agrego a cach� " + pRawKey);
            if (pDuracion > 0)
            {
                ClienteRedisEscritura.Set(pRawKey, ObjectToByteArray(pObjetoCache), (int)pDuracion);
            }
            else
            {
                string resultado = ClienteRedisEscritura.Set(pRawKey, ObjectToByteArray(pObjetoCache)).Result;
            }
            AgregarEntradaTraza("Redis. Agregado con expiracion: " + pRawKey);
        }

        private object InteractuarRedis_Lectura(string pRawKey, string pRawKeyOriginal, bool pGenerarClave, Type pTipo)
        {
            object resultado = null;

            AgregarEntradaTraza("Redis. Obtengo clave de cach� " + pRawKey);

            byte[] bytesCache = null;
            try
            {
                bytesCache = ClienteRedisLectura.Get<byte[]>(pRawKey).Result;
            }
            catch (RedisException redisException)
            {
                _loggingService.GuardarLogError($"Error al obtener la clave {pRawKey} de cache. \nError: {redisException}", mlogger);
            }

            if (bytesCache != null && bytesCache.Length > 0)
            {
                AgregarEntradaTraza("Redis. Existe " + pRawKey);
                resultado = ByteArrayToObject(bytesCache, pTipo);
                if (resultado is KeyValuePair<object, double>)
                {
                    resultado = ((KeyValuePair<object, double>)resultado).Key;
                }
                AgregarEntradaTraza("Redis. Obtenido: " + pRawKey);

                if (resultado is string && resultado.ToString().StartsWith("[DATASET]"))
                {
                    string xmlDataSet = resultado.ToString().Replace("[DATASET]", "");
                    xmlDataSet = UnZip(xmlDataSet);

                    string tipoDataSet = xmlDataSet.Substring(1, xmlDataSet.IndexOf(" xmlns=") - 1);
                    DataSet ds = new DataSet();

                    StringReader reader = new StringReader(xmlDataSet);
                    try
                    {
                        ds.ReadXml(reader);
                        ds.AcceptChanges();
                        resultado = ds;
                    }
                    catch
                    {
                        resultado = null;
                    }
                    reader.Close();
                    reader.Dispose();
                    AgregarEntradaTraza("Redis. DataSet recuperado desde string");
                }
            }
            else
            {
                AgregarEntradaTraza("Redis. No est� en cach�: " + pRawKey);
            }

            return resultado;
        }

        private object InteractuarRedis_Comprobacion(string pRawKey, string pRawKeyOriginal)
        {
            AgregarEntradaTraza("Redis. Compruebo existe clave" + pRawKey);
            object resultado = ClienteRedisLectura.Exists(pRawKey).Result;
            AgregarEntradaTraza("Redis. Comprobado: " + pRawKey);

            return resultado;
        }

        #region Cache Local

        /// <summary>
        /// Devuelve el dato correspondiente a la clave de cach� local
        /// </summary>
        /// <param name="pRawKey">Clave de cach� que se va a buscar en local</param>
        /// <returns>Objeto solicitado</returns>
        public object ObtenerObjetoDeCacheLocal(string pRawKey)
        {
            if (!UsarCacheLocal.Equals(UsoCacheLocal.Nunca) && EjecucionDeAplicacionWeb)
            {

                pRawKey = ObtenerClaveCache(pRawKey);


                _loggingService.AgregarEntrada("CacheLocal: Obtenemos " + pRawKey);

                object objeto = _redisCacheWrapper.Cache.Get(pRawKey);

                if (objeto != null)
                {
                    _loggingService.AgregarEntrada("CacheLocal: Existe " + pRawKey);
                }
                else
                {
                    _loggingService.AgregarEntrada("CacheLocal: NO Existe " + pRawKey);
                }

                return objeto;
            }

            return null;
        }

        /// <summary>
        /// Agrega en la cach� local el dato pDato con la clave de cach� pRawKey
        /// </summary>
        /// <param name="pRawKey">Clave de cach� que va a guardar el objeto</param>
        /// <param name="pDato">Dato que se va a guardar con la clave de cach�.</param>
        public void AgregarObjetoCacheLocal(Guid pProyectoID, string pRawKey, object pDato, bool pObtenerParametroSiempre = false, DateTime? pExpirationDate = null)
        {
            //if (pRawKey == null)
            //{
            pRawKey = this.ObtenerClaveCache(pRawKey);
            //}
            if (!pExpirationDate.HasValue)
            {
                pExpirationDate = DateTime.Now.AddYears(1);
            }

            if (!UsarCacheLocal.Equals(UsoCacheLocal.Nunca) && EjecucionDeAplicacionWeb && pDato != null && (pObtenerParametroSiempre || UsarCacheLocal.Equals(UsoCacheLocal.Siempre) || _configService.ObtenerUsarCacheLocal()))
            {
                _loggingService.AgregarEntrada("CacheLocal: A�ado " + pRawKey);

                string fileName = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}config{Path.DirectorySeparatorChar}versionCacheLocal{Path.DirectorySeparatorChar}{pProyectoID}.config";
                FileInfo fileInfo = new FileInfo(fileName);

                if (!fileInfo.Directory.Exists)
                {
                    fileInfo.Directory.Create();
                }
                if (!fileInfo.Exists)
                {
                    fileInfo.Create();
                }

                PollingFileChangeToken cacheDep = new PollingFileChangeToken(fileInfo);
                //CacheDependency cacheDep = new CacheDependency(fileName);
                _redisCacheWrapper.Cache.Set(pRawKey, pDato, cacheDep);
                //System.Web.HttpContext.Current.Cache.Insert(pRawKey, pDato, cacheDep, pExpirationDate.Value, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
        }

        public static void ActualizarCacheDependencyCacheLocal(Guid pProyectoID, string pValor)
        {
            try
            {
                string ruta = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}{Path.DirectorySeparatorChar}config{Path.DirectorySeparatorChar}versionCacheLocal";
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                string fileName = $"{ruta}{Path.DirectorySeparatorChar}{pProyectoID}.config";
                using (StreamWriter sw = new StreamWriter(fileName))
                {
                    sw.Write(pValor);
                    sw.Close();
                }
            }
            catch
            {

            }
        }

        public void InvalidarCacheLocal(string pRawKey)
        {
            pRawKey = ObtenerClaveCache(pRawKey);
            _redisCacheWrapper.Cache.Remove(pRawKey);
        }

        //public ConcurrentDictionary<string, RiamCacheDependency> ObtenerDiccionarioCacheDependencyPorTipo(TipoCacheLocalRedis pTipo)
        //{
        //    return ObtenerObjetoDeCacheLocal(CLAVE_DICCIONARIO_CACHE_DEPENDENCY + (int)pTipo) as ConcurrentDictionary<string, RiamCacheDependency>;
        //}

        #endregion Cache Local

        #endregion

        private RedisDB ObtenerClienteRedisParaIP(string pIP, int db)
        {
            _loggingService.AgregarEntrada("BaseCL_ObtenerClienteRedisParaIP_BEGIN");
            RedisDB clienteRedis = null;
            string password = null;
            int port = 6379;
            int p = 0;
            if (pIP.Contains("|"))
            {
                // El servidor tiene contrase�a
                string[] ipPassword = pIP.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                pIP = ipPassword[0];

                if (ipPassword.Length > 1)
                {
                    password = ipPassword[1];
                }
            }
            if (pIP.Contains(':'))
            {
                string[] ipPort = pIP.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                pIP = ipPort[0];
                if (ipPort.Length > 1 && int.TryParse(ipPort[1], out p))
                {
                    port = p;
                }
            }

            if (!string.IsNullOrEmpty(password))
            {
                //clienteRedis = new RedisDB(pIP, REDIS_DEFAULT_PORT, password);
                clienteRedis = new RedisDB(db, new JsonFormater());
                clienteRedis.AutoPing = false;
                //RedisCacheWrapper.GuardarLog($"Conexion creada: {pIP} {db}");
                clienteRedis.Host.AddWriteHost(pIP, port).Password = password;
                //clienteRedis.Host.AddWriteHost(pIP).MaxConnections = 5000;
                //clienteRedis.Host.AddReadHost(pIP, port).Password = password;
                //clienteRedis.Host.AddReadHost(pIP).MaxConnections = 5000;
            }
            else
            {
                clienteRedis = new RedisDB(db, new JsonFormater());
                clienteRedis.AutoPing = false;
                //RedisCacheWrapper.GuardarLog($"Conexion creada: {pIP} {db}");
                clienteRedis.Host.AddWriteHost(pIP, port);
                //clienteRedis.Host.AddReadHost(pIP, port);
            }

            _loggingService.AgregarEntrada("BaseCL_ObtenerClienteRedisParaIP_FIN");
            return clienteRedis;
        }

        private void ObtenerTamanioPoolRedis()
        {
            mTamanioPool = 50;
            int tamanio = 0;

            try
            {
                //Si no hab�a configuraci�n para este dominio, o no estamos en una aplicaci�n Web, cojo la configuraci�n por defecto
                ParametroAplicacion filaParametroTamanioRedis = _entityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == ParametroAD.TamanioPoolRedis);
                //ParametroAplicacionDS.ParametroAplicacionRow filaParametroTamanioRedis = paramAplicDS.ParametroAplicacion.FindByParametro(ParametroAD.TamanioPoolRedis);

                if (filaParametroTamanioRedis != null)
                {
                    int.TryParse(filaParametroTamanioRedis.Valor, out tamanio);
                }

            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, null, mlogger,true);
            }

            if (tamanio > 0)
            {
                mTamanioPool = tamanio;
            }
        }

        #region Propiedades

        /// <summary>
        /// Cliente de Redis
        /// </summary>
        public RedisDB ClienteRedisLectura
        {
            get
            {
                try
                {
                    // Se saca fuera para que no se compruebe siempre el nodoDB de la petici�n antes de devolverla
                    if (mPoolName == "")
                    {
                        mPoolName = "redis";
                    }
                    mPoolName = mPoolName.Replace("acid", "redis");
                    mPoolName = mPoolName.Replace("_Master", "");

                    string nodoIPMaster = _configService.ObtenerConexionRedisIPMaster(mPoolName);
                    string nodoIPRead = _configService.ObtenerConexionRedisIPRead(mPoolName);
                    int nodoDB = _configService.ObtenerConexionRedisBD(mPoolName);
                    int redisTimeOut = _configService.ObtenerConexionRedisTimeout(mPoolName);

                    if (mUsarClienteEscritura)
                    {
                        var cliente = ClienteRedisEscritura;
                        return cliente;
                    }

                    if (mClienteRedisLectura == null)
                    {
                        //BeetleX.Buffers.BufferPool.BUFFER_SIZE = 2400000;
                        BeetleX.Buffers.BufferPool.POOL_MAX_SIZE = 204800;
                        string cadenaPeticion = nodoIPMaster;
                        Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                        mClienteRedisLectura = _redisCacheWrapper.RedisLectura(mPoolName);
                        _loggingService.AgregarEntrada("ClienteRedisLectura: Para '" + mPoolName + "' �existe en petici�n actual?" + (mClienteRedisLectura != null));

                        if (mClienteRedisLectura == null)
                        {
                            try
                            {
                                mClienteRedisLectura = ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                                mClienteRedisLectura.DB = nodoDB;
                                //nodo master
                                string password = null;
                                int portMaster = 6379;
                                int pM = 0;
                                if (nodoIPMaster.Contains("|"))
                                {
                                    // El servidor tiene contrase�a
                                    string[] ipPassword = nodoIPMaster.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                                    nodoIPMaster = ipPassword[0];

                                    if (ipPassword.Length > 1)
                                    {
                                        password = ipPassword[1];
                                    }
                                }
                                if (nodoIPMaster.Contains(':'))
                                {
                                    string[] ipPort = nodoIPMaster.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    nodoIPMaster = ipPort[0];
                                    if (ipPort.Length > 1 && int.TryParse(ipPort[1], out pM))
                                    {
                                        portMaster = pM;
                                    }
                                }
                                ///Fin nodo master
                                if (!string.IsNullOrEmpty(nodoIPRead))
                                {
                                    int port = 6379;
                                    //nodo lectura
                                    if (nodoIPRead.Contains(':'))
                                    {
                                        string[] ipPort = nodoIPRead.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                        nodoIPRead = ipPort[0];
                                        int p = 0;
                                        if (ipPort.Length > 1 && int.TryParse(ipPort[1], out p))
                                        {
                                            port = p;
                                        }

                                    }
                                    //Fin nodo lectura
                                    Random random = new Random();
                                    int num = random.Next(101);
                                    if (num % 2 == 0)
                                    {
                                        if (!string.IsNullOrEmpty(password))
                                        {
                                            mClienteRedisLectura.Host.AddReadHost(nodoIPMaster, portMaster).Password = password;
                                        }
                                        else
                                        {
                                            mClienteRedisLectura.Host.AddReadHost(nodoIPMaster, portMaster);
                                        }
                                        mClienteRedisLectura.Host.AddReadHost(nodoIPRead, port);
                                    }
                                    else
                                    {
                                        mClienteRedisLectura.Host.AddReadHost(nodoIPRead, port);
                                        if (!string.IsNullOrEmpty(password))
                                        {
                                            mClienteRedisLectura.Host.AddReadHost(nodoIPMaster, portMaster).Password = password;
                                        }
                                        else
                                        {
                                            mClienteRedisLectura.Host.AddReadHost(nodoIPMaster, portMaster);
                                        }
                                    }

                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(password))
                                    {
                                        mClienteRedisLectura.Host.AddReadHost(nodoIPMaster, portMaster).Password = password;
                                    }
                                    else
                                    {
                                        mClienteRedisLectura.Host.AddReadHost(nodoIPMaster, portMaster);
                                    }
                                }


                                _loggingService.AgregarEntradaDependencia($"ClienteRedisLectura: cargamos para la ip '{nodoIPMaster}' y BD '{nodoDB}'", false, "Cliente Redis Lectura", sw, true);

                                _redisCacheWrapper.AddRedisLectura(mPoolName, mClienteRedisLectura);


                                var result = mClienteRedisLectura.Execute(new SETNAME(), typeof(string));
                                result.Wait();
                                _loggingService.AgregarEntrada("ClienteRedisLectura: Agregado a la petici�n actual");
                            }
                            catch (Exception ex)
                            {
                                _loggingService.AgregarEntradaDependencia($"ClienteRedisLectura: Error al conectar con redis {mPoolName} a la IP {nodoIPMaster} con puerto {nodoDB}", false, "Cliente Redis Lectura", sw, false);
                                _loggingService.GuardarLogError(ex, string.Format("Error al conectar con redis {0}, a la IP {1} con puerto {2}",mPoolName, nodoIPMaster, nodoDB),mlogger, true);
                            }
                        }
                    }

                    if (!mClienteRedisLectura.DB.Equals(nodoDB))
                    {
                        mClienteRedisLectura = ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                        var result = mClienteRedisLectura.Execute(new SETNAME(), typeof(string));
                        result.Wait();
                        _redisCacheWrapper.AddRedisLectura(mPoolName, mClienteRedisLectura);
                    }

                    return mClienteRedisLectura;
                }
                catch (Exception ex)
                {
                    _loggingService.AgregarEntrada($"ClienteRedisLectura: Error al conectar con redis {mPoolName}");
                    _loggingService.GuardarLogError(ex, $"Error al conectar con redis {mPoolName}", mlogger,true);
                    return null;
                }
            }
        }

        public string NombreProyectoPadreEcositema
        {
            get
            {
                if (mNombreProyectoPadreEcositema == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string ComunidadPadreEcosistemaID = paramCN.ObtenerParametroAplicacion("ComunidadPadreEcosistemaID");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(ComunidadPadreEcosistemaID))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreProyectoPadreEcositema = proyCN.ObtenerNombreCortoProyecto(Guid.Parse(ComunidadPadreEcosistemaID));
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.", mlogger);
                            mNombreProyectoPadreEcositema = "";
                        }
                    }
                    if (mNombreProyectoPadreEcositema == null)
                    {
                        mNombreProyectoPadreEcositema = "";
                    }
                }
                return mNombreProyectoPadreEcositema;
            }
        }

        public string NombreCortoProyectoPadreEcositema
        {
            get
            {
                if (mNombreCortoProyectoPadreEcosistema == null)
                {
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCN>(), mLoggerFactory);
                    string NombreCortoEcosistema = paramCN.ObtenerParametroAplicacion("NombreCortoProyectoPadreEcositema");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(NombreCortoEcosistema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mNombreCortoProyectoPadreEcosistema = NombreCortoEcosistema;
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.", mlogger);
                            mNombreCortoProyectoPadreEcosistema = "";
                        }
                    }
                    if (mNombreCortoProyectoPadreEcosistema == null)
                    {
                        mNombreCortoProyectoPadreEcosistema = "";
                    }
                }
                return mNombreCortoProyectoPadreEcosistema;
            }
        }

        /// <summary>
        /// Nombre del proyecto padre del ecosistema (comunidad/nombrecorto)
        /// </summary>
        public Guid? ProyectoIDPadreEcosistema
        {
            get
            {
                if (mPadreEcosistemaProyectoID == null)
                {
                    if (!string.IsNullOrEmpty(NombreCortoProyectoPadreEcositema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreCortoProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.", mlogger);
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (!string.IsNullOrEmpty(NombreProyectoPadreEcositema) && (mPadreEcosistemaProyectoID == null || (mPadreEcosistemaProyectoID != null && mPadreEcosistemaProyectoID.Value.Equals(Guid.Empty))))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.", mlogger);
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (mPadreEcosistemaProyectoID == null)
                    {
                        mPadreEcosistemaProyectoID = Guid.Empty;
                    }
                }
                return mPadreEcosistemaProyectoID;
            }
        }

        /// <summary>
        /// Cliente de Redis
        /// </summary>
        public RedisDB ClienteRedisEscritura
        {
            get
            {
                try
                {
                    if (mPoolName == "")
                    {
                        mPoolName = "redis";
                    }

                    // Se saca fuera para que no se compruebe siempre el nodoDB de la petici�n antes de devolverla
                    string poolNameEscritura = mPoolName.Replace("acid", "redis");

                    string nodoIPMaster = _configService.ObtenerConexionRedisIPMaster(mPoolName);
                    int nodoDB = _configService.ObtenerConexionRedisBD(mPoolName);
                    int redisTimeOut = _configService.ObtenerConexionRedisTimeout(mPoolName);

                    if (mClienteRedisEscritura == null)
                    {
                        //BeetleX.Buffers.BufferPool.BUFFER_SIZE = 2400000;
                        BeetleX.Buffers.BufferPool.POOL_MAX_SIZE = 204800;
                        string cadenaPeticion = nodoIPMaster;
                        Stopwatch sw = LoggingService.IniciarRelojTelemetria();

                        mClienteRedisEscritura = _redisCacheWrapper.RedisEscritura(mPoolName);
                        _loggingService.AgregarEntrada("ClienteRedisEscritura: Para '" + mPoolName + "' �existe en petici�n actual?" + (mClienteRedisEscritura != null));

                        if (mClienteRedisEscritura == null)
                        {
                            try
                            {
                                _loggingService.AgregarEntrada("ClienteRedisEscritura: cargamos para la ip '" + nodoIPMaster + "' y BD '" + nodoDB + "'");

                                mClienteRedisEscritura = (RedisDB)ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                                //mClienteRedisEscritura.Host.GetWriteHost().
                                //mClienteRedisEscritura.ConnectTimeout = redisTimeOut;
                                //mClienteRedisEscritura.RetryCount = 3;
                                //mClienteRedisEscritura.DB = nodoDB;

                                _loggingService.AgregarEntradaDependencia($"ClienteRedisEscritura: cargamos para la ip '{nodoIPMaster}' y BD '{nodoDB}'", false, "Cliente Redis Escritura", sw, true);

                                
                                var result = mClienteRedisEscritura.Execute(new SETNAME(), typeof(string));
                                result.Wait();
                                _redisCacheWrapper.AddRedisEscritura(mPoolName, mClienteRedisEscritura);
                                _loggingService.AgregarEntrada("ClienteRedisEscritura: Agregado a la petici�n actual");
                            }
                            catch (Exception ex)
                            {
                                _loggingService.AgregarEntradaDependencia($"ClienteRedisEscritura: Error al conectar con redis {mPoolName} a la IP {nodoIPMaster} con puerto {nodoDB}", false, "Cliente Redis Escritura", sw, false);
                                _loggingService.GuardarLogError(ex, null, mlogger,true);
                            }
                        }
                    }

                    if (!mClienteRedisEscritura.DB.Equals(nodoDB))
                    {
                        mClienteRedisEscritura = ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                        var result = mClienteRedisLectura.Execute(new SETNAME(), typeof(string));
                        result.Wait();
                        _redisCacheWrapper.AddRedisEscritura(mPoolName, mClienteRedisEscritura);
                    }

                    return mClienteRedisEscritura;
                }
                catch (Exception ex)
                {
                    _loggingService.AgregarEntrada($"ClienteRedisEscritura: Error al conectar con redis {mPoolName}");
                    _loggingService.GuardarLogError(ex, $"Error al conectar con redis {mPoolName}", mlogger,true);
                    return null;
                }
            }
        }

        /// <summary>
        /// Devuelve la clave para la cach�
        /// </summary>
        public virtual string[] ClaveCache
        {
            get
            {
                throw new Exception("Debe ser implementado en los hijos");
            }
        }

        /// <summary>
        /// Obtiene o establece el dominio sobre el que se genera la cache
        /// </summary>
        public virtual string Dominio
        {
            get
            {
                return mDominio;
            }
            set
            {
                mDominio = value;
            }
        }


        private bool UsarHilos
        {
            get
            {
                if (!mUsarHilos.HasValue)
                {
                    ComprobarSiHayQueUsarHilos();
                }
                return mUsarHilos.Value;
            }
        }

        /// <summary>
        /// Obtiene si la aplicaci�n actual es un entorno Web. 
        /// </summary>
        protected bool EjecucionDeAplicacionWeb
        {
            get
            {
                return _redisCacheWrapper != null && _redisCacheWrapper.Cache != null;
            }
        }

        /// <summary>
        /// Obtiene el tama�o del pool de conexiones hacia Redis
        /// </summary>
        public int TamanioPool
        {
            get
            {
                if (!mTamanioPool.HasValue)
                {
                    ObtenerTamanioPoolRedis();
                }
                return mTamanioPool.Value;
            }
        }

        public bool EsHijoEcosistemaProyecto(Guid pProyectoIDPadre)
        {
            if (ProyectoIDPadreEcosistema.HasValue)
            {
                if (pProyectoIDPadre.Equals(ProyectoIDPadreEcosistema.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TieneComunidadPadreConfigurada(Guid pProyectoID)
        {
            bool tieneComunidadPadreConfigurada = false;
            ProyectoCN proCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
            Guid proyectoSuperiorID = proCN.ObtenerProyectoSuperiorIDDeProyectoID(pProyectoID);
            if (proyectoSuperiorID != Guid.Empty && EsHijoEcosistemaProyecto(proyectoSuperiorID))
            {
                tieneComunidadPadreConfigurada = true;
            }
            return tieneComunidadPadreConfigurada;
        }


        /// <summary>
        /// Establece el dominio de la cache.
        /// </summary>
        /// <param name="pUrlIntragnoss">UrlIntragnoss</param>
        public void EstablecerDominioCache(string pUrlIntragnoss)
        {
            string dominio = pUrlIntragnoss;

            dominio = dominio.Replace("http://", "").Replace("https://", "").Replace("www.", "");

            if (dominio[dominio.Length - 1] == '/')
            {
                dominio = dominio.Substring(0, dominio.Length - 1);
            }

            this.Dominio = dominio;
        }

        public void VersionarCacheLocal(Guid pProyectoID)
        {
            GnossCache gnossCache = new GnossCache(_entityContext, _loggingService, _redisCacheWrapper, _configService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<GnossCache>(), mLoggerFactory);

            gnossCache.VersionarCacheLocal(pProyectoID);
        }

        public bool UsarClienteEscritura
        {
            get
            {
                return mUsarClienteEscritura;
            }
            set
            {
                mUsarClienteEscritura = value;
            }
        }

        #endregion

        #region Miembros de IDisposable

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~BaseCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se est� llamando desde el Dispose()</param>
        protected virtual void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                this.mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                        //_redisCacheWrapper.CerrarConexionesEscritura();
                        //_redisCacheWrapper.CerrarConexionesLectura();
                        //ClienteRedisLectura.Dispose();
                        //ClienteRedisEscritura.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _loggingService.GuardarLogError(ex, null, mlogger,true);
                }
            }
        }

        #endregion

        #endregion
    }

    //Realizacion de metodo de flushdb para redis
    public class FLUSHDB : Command
    {
        public override bool Read => false;

        public override string Name => "FLUSHDB";
    }

    //Realizacion de metodo de CLIENT SETNAME para redis
    public class SETNAME : Command
    {
        private string _name;
        public override bool Read => false;

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = $"{AppDomain.CurrentDomain.FriendlyName}_{Environment.GetEnvironmentVariable("COMPOSE_PROJECT_NAME")}_{Guid.NewGuid()}";
                }
                return _name;
            }

        }
        public SETNAME()
        {
            AddText("CLIENT");
            AddText("SETNAME");
        }
    }
}
