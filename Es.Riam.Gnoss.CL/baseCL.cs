using BeetleX.Redis;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.CL
{
    /// <summary>
    /// Clase que contiene variables estaticas con los prefijos de los nombres de la cache
    /// </summary>
    public class NombresCL
    {
        /// <summary>
        /// Prefijo "Tesauro"
        /// </summary>
        public static string TESAURO = "Tesauro";

        /// <summary>
        /// Prefijo "EstadosCiviles"
        /// </summary>
        public static string ESTADOSCIVILES = "EstadosCiviles";

        /// <summary>
        /// Prefijo "Paises"
        /// </summary>
        public static string PAISES = "Paises";

        /// <summary>
        /// Prefijo "ParametroAplicacion"
        /// </summary>
        public static string PARAMETOAPLICACION = "ParametroAplicacion";

        /// <summary>
        /// Prefijo "ParametroGeneralDS"
        /// </summary>
        public static string PARAMETROGENERAL = "ParametroGeneral";

        /// <summary>
        /// Prefijo "TodosParametrosGeneralesLigera"
        /// </summary>
        public static string TODOSPARAMETROSGENERALESLIGERA = "TodosParametrosGeneralesLigera";

        /// <summary>
        /// Prefijo "TieneProyectoImagenHome"
        /// </summary>
        public static string TIENEPROYECTOIMAGENHOME = "TieneProyectoImagenHome";

        /// <summary>
        /// Prefijo "Facetado"
        /// </summary>
        public static string FACETADO = "Facetado";

        /// <summary>
        /// Prefijo "TABLACONFIGURACIONFACETADO"
        /// </summary>
        public static string TABLACONFIGURACIONFACETADO = "TablaConfigFacetado";

        /// <summary>
        /// Prefijo "CONFIGURACIONFACETAS"
        /// </summary>
        public static string CONFIGURACIONFACETAS = "ConfigFacetas";

        /// <summary>
        /// Prefijo "ParametroAplicacion"
        /// </summary>
        public static string LINKEDOPENDATA = "LOD";

        /// <summary>
        /// Prefijo "CMS"
        /// </summary>
        public static string CMS = "CMS";

        /// <summary>
        /// Prefijo "MVC"
        /// </summary>
        public static string MVCModel = "MVC";

        /// <summary>
        /// Prefijo "ExportacionBusqueda"
        /// </summary>
        public static string EXPORTACIONBUSQUEDA = "ExportacionBusqueda";

        #region AMIGOS

        /// <summary>
        /// Prefijo "Amigos"
        /// </summary>
        public static string AMIGOS = "Amigos";

        /// <summary>
        /// Prefijo "Org"
        /// </summary>
        public static string AMIGOSORG = "Org";

        /// <summary>
        /// Prefijo "Autocompletar"
        /// </summary>
        public static string AUTOCOMPLETARINVALIDARCACHE = "AutocompletarInvalidarCache";

        /// <summary>
        /// Prefijo "Org"
        /// </summary>
        public static string AMIGOSORGEIDENTPROYPRIV = "OrgEIdentPryPri";

        /// <summary>
        /// Prefijo "Per"
        /// </summary>
        public static string AMIGOSPER = "Per";

        /// <summary>
        /// Prefijo "Per"
        /// </summary>
        public static string AMIGOSPEREIDENTPROYPRIV = "PerEIdentPryPri";

        public static string IDENTIDADDS = "IdentidadDS";

        public static string PERSONADS = "PersonaDS";

        public static string TAGDS = "TagDS";

        public static string USUARIODS = "UsuarioDS";

        public static string ORGANIZACIONDS = "OrganizacionDS";

        public static string AMIGOSDS = "AmigosDS";

        public static string AMIGOSENPROYECTO = "AmigosEnProyecto";

        public static string CONTACTOSRECOMEN = "ContactosRecomen";

        public static string COMUNIDADESRECOMEN = "ComunidadesRecomen";

        #endregion

        #region DOCUMENTACION

        /// <summary>
        /// Prefijo "Documentacion"
        /// </summary>
        public static string DOCUMENTACION = "Documentacion";

        /// <summary>
        /// Prefijo "FichaRecursoMVC"
        /// </summary>
        public static string FICHARECURSOMVC = "FichaRecursoMVC3";

        /// <summary>
        /// Prefijo "RecursosComunidad"
        /// </summary>
        public static string RECURSOSCOMUNIDAD = "RecursosComunidad";

        /// <summary>
        /// Prefijo "PrimerosRecursos"
        /// </summary>
        public static string PRIMEROSRECURSOS = "PrimerosRecursos";

        /// <summary>
        /// Prefijo "RssComunidad"
        /// </summary>
        public static string RSSSCOMUNIDAD = "RssComunidad";

        /// <summary>
        /// Prefijo "TotalResultadosRecursos"
        /// </summary>
        public static string TOTALRESULTADOSRECURSOS = "TotalResultadosRecursos";

        /// <summary>
        /// Prefijo "PerfilesConRecursosPrivados"
        /// </summary>
        public static string PERFILESCONRECURSOSPRIVADOS = "PerfilesConRecursosPrivados";

        /// <summary>
        /// Prefijo "PrimerosDebates"
        /// </summary>
        public static string PRIMEROSDEBATES = "PrimerosDebates";

        /// <summary>
        /// Prefijo "PrimerasPersonas"
        /// </summary>
        public static string PRIMERASPERSONAS = "PrimerasPersonas";

        /// <summary>
        /// Prefijo "TotalResultadosDebates"
        /// </summary>
        public static string TOTALRESULTADOSDEBATES = "TotalResultadosDebates";

        /// <summary>
        /// Prefijo "PerfilesConDebatesPrivados"
        /// </summary>
        public static string PERFILESCONDEBATESPRIVADOS = "PerfilesConDebatesPrivados";

        /// <summary>
        /// Prefijo "PrimerasPreguntas"
        /// </summary>
        public static string PRIMERASPREGUNTAS = "PrimerasPreguntas";

        /// <summary>
        /// Prefijo "TotalResultadosPreguntas"
        /// </summary>
        public static string TOTALRESULTADOSPREGUNTAS = "TotalResultadosPreguntas";

        /// <summary>
        /// Prefijo "RecursosRelacionadosContextos"
        /// </summary>
        public static string RECURSOSRELACIONADOSCONTEXTOS = "Cont";

        /// <summary>
        /// Prefijo "RecursosRelacionados"
        /// </summary>
        public static string RECURSOSRELACIONADOS = "RecursosRelacionados";

        /// <summary>
        /// Prefijo "RecursosRelacionados"
        /// </summary>
        public static string NUMRECURSOSRELACIONADOS = "NumRecursosRelacionados";

        /// <summary>
        /// Prefijo "QueEstaPasando"
        /// </summary>
        public static string QUEESTAPASANDO = "QueEstaPasando";

        /// <summary>
        /// Prefijo "QueSeEstaLeyendo"
        /// </summary>
        public static string QUESEESTALEYENDO = "QueSeEstaLeyendo";

        /// <summary>
        /// Prefijo "QueSeEstaLeyendo"
        /// </summary>
        public static string AUTOCOMPETIQUETAS = "autocompletaretiquetas";

        #endregion

        #region LIVE

        /// <summary>
        /// Prefijo "Live"
        /// </summary>
        public static string LIVE = "Live";

        /// <summary>
        /// Prefijo "OrgsConRecursos"
        /// </summary>
        public static string ORGSCONRECURSOS = "OrgsConRecursos";

        /// <summary>
        /// Prefijo "IdenConPrivados"
        /// </summary>
        public static string IDENCONPRIVADOS = "IdenConPrivados";

        /// <summary>
        /// Prefijo "OrgID"
        /// </summary>
        public static string ORGID = "OrgID";

        /// <summary>
        /// Prefijo "IdenID"
        /// </summary>
        public static string IDENID = "IdenID";

        public static string INVITADOGNOSS = "InvitadoGnoss";

        public static string INVITADOCOM = "InvitadoCom";

        #endregion

        #region METABUSCADOR

        ///// <summary>
        ///// Prefijo "Metabuscador"
        ///// </summary>
        //public static string METABUSCADOR = "Metabuscador";

        ///// <summary>
        ///// Prefijo "Inv"
        ///// </summary>
        //public static string INVITADO = "Inv";

        ///// <summary>
        ///// Prefijo "PerComunidad"
        ///// </summary>
        //public static string PERSONACOMUNIDAD = "PerComunidad";

        ///// <summary>
        ///// Prefijo "OrgComunidad"
        ///// </summary>
        //public static string ORGANIZACIONCOMUNIDAD = "OrgComunidad";

        ///// <summary>
        ///// Prefijo "CVPerComunidad"
        ///// </summary>
        //public static string CVPERSONACOMUNIDAD = "CVPerComunidad";

        ///// <summary>
        ///// Prefijo "CVOrgComunidad"
        ///// </summary>
        //public static string CVORGANIZACIONCOMUNIDAD = "CVOrgComunidad";

        ///// <summary>
        ///// Prefijo "Iden_PerOrg_Com"
        ///// </summary>
        //public static string IDENTIDADESPERORGCOMUNIDAD = "Iden_PerOrg_Com";

        ///// <summary>
        ///// Prefijo "Num_PerOrg"
        ///// </summary>
        //public static string NUMRESULTADOSPERORG = "Num_PerOrg";

        ///// <summary>
        ///// Prefijo "ContadorTags_PerOrg"
        ///// </summary>
        //public static string CONTADORTAGSPERORG = "ContadorTags_PerOrg";

        ///// <summary>
        ///// Prefijo "OrdenElem_PerOrg"
        ///// </summary>
        //public static string ORDENELEMPERORG = "OrdenElem_PerOrg";

        #endregion

        #region ORGANIZACION

        /// <summary>
        /// Prefijo "Organizacion"
        /// </summary>
        public static string ORGANIZACION = "Organizacion";

        /// <summary>
        /// Prefijo "TablaBaseOrganizacionID"
        /// </summary>
        public static string TABLABASEORGANIZACIONID = "TablaBaseOrganizacionID";

        #endregion

        #region PERSONAS

        /// <summary>
        /// Prefijo "Personas"
        /// </summary>
        public static string PERSONAS = "Personas";

        /// <summary>
        /// Prefijo "Todas"
        /// </summary>
        public static string TODAS = "Todas";

        /// <summary>
        /// Prefijo "PersonasAcceden"
        /// </summary>
        public static string PERSONASACCEDEN = "PersonasAcceden";

        /// <summary>
        /// Prefijo "NotificacionPendiente"
        /// </summary>
        public static string NOTIFICACIONPENDIENTE = "NotificacionPendiente";

        #endregion

        #region PROYECTO

        /// <summary>
        /// Prefijo "Proyecto"
        /// </summary>
        public static string PROYECTO = "Proyecto";

        /// <summary>
        /// Prefijo "TablaBaseProyectoID"
        /// </summary>
        public static string TABLABASEPROYECTOID = "TablaBaseProyectoID";

        /// <summary>
        /// Prefijo "ProyectoEventoAccion"
        /// </summary>
        public static string PROYECTOEVENTOACCION = "ProyectoEventoAccion";

        /// <summary>
        /// Prefijo "ProyectosAccionesExternas"
        /// </summary>
        public static string PROYECTOSACCIONESEXTERNAS = "ProyectosAccionesExternas";

        /// <summary>
        /// Prefijo "NivelesCertificacionRecurso"
        /// </summary>
        public static string NIVELESCERTIFICACIONRECURSO = "NivelesCertificacionRecurso";

        /// <summary>
        /// Prefijo "NombreCortoProyecto"
        /// </summary>
        public static string NOMBRECORTOPROYECTO = "NombreCortoProyecto";

        /// <summary>
        /// Prefijo "BRID"
        /// </summary>
        public static string BASERECURSOSID = "BRID";

        /// <summary>
        /// Prefijo "ProyectoAutoPromocion"
        /// </summary>
        public static string PROYECTOAUTOPROMOCION = "ProyectoAutoPromocion";

        /// <summary>
        /// Prefijo "HTMLAdministradoresProyecto"
        /// </summary>
        public static string HTMLADMINISTRADORESPROYECTO = "HTMLAdministradoresProyecto";

        /// <summary>
        /// Prefijo "ProyectosPorID"
        /// </summary>
        public static string PROYECTOSPORID = "ProyectosPorID";

        /// <summary>
        /// Prefijo "TipoAcceso"
        /// </summary>
        public static string USUARIOBLOQUEADOPROYECTO = "UsuarioBloqueadoProyecto";

        /// <summary>
        /// Prefijo "TipoAcceso"
        /// </summary>
        public static string TIPOACCESO = "TipoAcceso";

        /// <summary>
        /// Prefijo "MisComunidades"
        /// </summary>
        public static string MISCOMUNIDADES = "MisComunidades";

        /// <summary>
        /// Prefijo "ClausulasRegitroProyecto"
        /// </summary>
        public static string CLAUSULASREGITROPROYECTO = "ClausulasRegitroProyecto";

        /// <summary>
        /// Prefijo "ClausulasRegitroProyecto"
        /// </summary>
        public static string POLITICACOOKIESPROYECTO = "PoliticaCookiesProyecto";

        /// <summary>
        /// Prefijo "ListaProyectosPerfil"
        /// </summary>
        public static string LISTAPROYECTOSPERFIL = "ListaProyectosPerfil";

        /// <summary>
        /// Prefijo "ListaProyectosUsuario"
        /// </summary>
        public static string LISTAPROYECTOSUSUARIO = "ListaProyectosUsuario";

        #endregion

        #region TAG

        /// <summary>
        /// Prefijo "Tag"
        /// </summary>
        public static string TAG = "Tag";

        #endregion

        #region HOME

        /// <summary>
        /// Prefijo "Home_Comunidades_Destacadas_Html"
        /// </summary>
        public static string HOMECOMUNIDADESDESTACADASHTML = "Home_Comunidades_Destacadas_Html";

        /// <summary>
        /// Prefijo "Home_Comunidades_Destacadas_DS"
        /// </summary>
        public static string HOMECOMUNIDADESDESTACADASDS = "Home_Comunidades_Destacadas_DS";

        /// <summary>
        /// Prefijo "Home_Recursos_Destacados_Html"
        /// </summary>
        public static string HOMERECURSOSDESTACADOSHTML = "Home_Recursos_Destacados_Html";

        /// <summary>
        /// Prefijo "Home_Recursos_Destacados_DS"
        /// </summary>
        public static string HOMERECURSOSDESTACADOSDS = "Home_Recursos_Destacados_DS";

        /// <summary>
        /// Prefijo "Home_Usuarios_Destacados_DS"
        /// </summary>
        public static string HOMEUSUARIOSDESTACADOSDS = "Home_Usuarios_Destacados_DS";

        #endregion

        /// <summary>
        /// Prefijo "FichaIdentidadMVC"
        /// </summary>
        public static string FichaIdentidadMVC = "FichaIdentidadMVC3";

        /// <summary>
        /// Prefijo "InfoExtraFichaIdentidadMVC"
        /// </summary>
        public static string InfoExtraFichaIdentidadMVC = "InfoExtraFichaIdentidadMVC3";
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
        /// Lectura de varios objetos en caché.
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
        /// Lectura del número de elementos.
        /// </summary>
        LecturaNumElementos = 7,
        /// <summary>
        /// Eliminacion de elementos del sorted set.
        /// </summary>
        EliminarElementosSortedSet = 8,
        /// <summary>
        /// Clonacción del sorted set.
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
        /// Renombrado de una clave de caché
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
        /// Borrado de elementos de la caché.
        /// </summary>
        Borrado = 16,
        /// <summary>
        /// Escritura de varios objetos en caché.
        /// </summary>
        EscrituraVariosObjetos = 17,
        /// <summary>
        /// Escritura de varios objetos en caché.
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
        /// Clave de caché para recalcular las rutas registradas
        /// </summary>
        RecalcularRutas = 9,
        /// <summary>
        /// Clave de caché para recalcular los Idiomas de la plataforma
        /// </summary>
        IdiomasPlataforma = 10
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
        /// Duración de la Caché en segundos
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
        /// Fichero de configuración de la base de datos
        /// </summary>
        protected string mFicheroConfiguracionBD = "";

        /// <summary>
        /// Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE
        /// </summary>
        protected bool mUsarVariableEstatica;

        public static string DominioEstatico = "";

        protected string mDominio;

        protected string mPoolName = "";

        protected static string mListaClavesERROR = "";

        private bool mUsarClienteEscritura = false;

        //En el caso del servicio live espiecífico, cambiarlo para que no use hilos y para que lance excepciones...
        public static bool? mUsarHilos = null;
        public static bool mLanzarExcepciones = false;

        public static int mDecimasSegundoEsperaRedis = 2;

        public static ConcurrentDictionary<string, DateTime> mServidoresRedisCaidos = new ConcurrentDictionary<string, DateTime>();
        public static int mNumCaidasConsecutivas = 0;

        //private static ConcurrentDictionary<string, PooledRedisClientManager> mListaPoolPorIP = new ConcurrentDictionary<string, PooledRedisClientManager>();

        private int? mTamanioPool = null;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region atributos

        private ConfigService _configService;
        private EntityContext _entityContext;
        private LoggingService _loggingService;
        private RedisCacheWrapper _redisCacheWrapper;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public BaseCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            _redisCacheWrapper = redisCacheWrapper;
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        public BaseCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            _redisCacheWrapper = redisCacheWrapper;
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;

            if (pFicheroConfiguracionBD != null)
            {
                mFicheroConfiguracionBD = pFicheroConfiguracionBD;
            }
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pPoolName">Nombre del pool</param>
        public BaseCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            _redisCacheWrapper = redisCacheWrapper;
            _configService = configService;
            _entityContext = entityContext;
            _loggingService = loggingService;

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
        /// <param name="pDataSetCache">DataSet de caché</param>
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
                        AgregarEntradaTraza(string.Format("No sirve el data set de caché porque no contiene la tabla {0}. Tipo: {1}", tabla.TableName, pDataSetCache.GetType().FullName));
                        return false;
                    }
                    foreach (DataColumn columna in tabla.Columns)
                    {
                        if (!pDataSetCache.Tables[tabla.TableName].Columns.Contains(columna.ColumnName))
                        {
                            AgregarEntradaTraza(string.Format("No sirve el data set de caché porque no contiene la columna {0} en la tabla {1}. Tipo: {2}", columna.ColumnName, tabla.TableName, pDataSetCache.GetType().FullName));
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
                return "_5.0.0.0";
            }
        }

        protected List<object> ObtenerVariosObjetosDeCache(params string[] pRawKey)
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

        protected Dictionary<string, object> ObtenerListaObjetosCache(string[] pListaClaves, Type pTipo)
        {
            Type[] tipos = new Type[pListaClaves.Length];
            for (int i = 0; i < pListaClaves.Length; i++)
            {
                pListaClaves[i] = pListaClaves[i].ToLower();
                tipos[i] = pTipo;
            }

            Dictionary<string, object> resultado = new Dictionary<string, object>();



            object objeto = InteractuarRedis(TipoAccesoRedis.LecturaDiccionarioObjetos, "", pListaClaves, tipos);
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

            (string, object)[] test = new (string, object)[pListaClaves.Length];

            if (pGenerarClave)
            {
                int i = 0;
                foreach (string claveoriginal in clavesOriginales)
                {
                    pListaClaves[i] = ObtenerClaveCache(clavesOriginales[i]).ToLower();

                    test[i] = (pListaClaves[i], "");
                    i++;
                }

            }

            int j = 0;
            foreach (object objetoOriginal in objetosOriginales)
            {
                if (objetoOriginal is DataSet)
                {
                    switch (((DataSet)objetoOriginal).DataSetName)
                    {
                        case "ProyectoDS":
                        case "IdentidadDS":
                        case "PersonaDS":
                        case "OrganizacionDS":
                        case "AmigosDS":
                        case "DocumentacionDS":
                        case "TesauroDS":
                            AgregarEntradaTraza("Redis. Obtengo xml DataSet");
                            string xmlDataSet = ((DataSet)objetoOriginal).GetXml().Replace("\n", "").Replace("\r", "").Replace(">    <", "><").Replace(">  <", "><");
                            pListaObjetos[j] = "[DATASET]" + Zip(xmlDataSet);
                            break;
                    }
                }
                test[j] = (test[j].Item1, pListaObjetos[j]);
                j++;
            }

            object[] parametrosExtra = new object[4];
            parametrosExtra[0] = pListaClaves;
            parametrosExtra[1] = pListaObjetos;
            parametrosExtra[2] = pDuracion;
            parametrosExtra[3] = test;
            InteractuarRedis(TipoAccesoRedis.EscrituraVariosObjetos, "", parametrosExtra);
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <returns>Objeto</returns>
        public object ObtenerObjetoDeCache(string pRawKey)
        {
            return ObtenerObjetoDeCache(pRawKey, true);
        }

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        /// <returns>Objeto</returns>
        public object ObtenerObjetoDeCache(string pRawKey, bool pGenerarClave)
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

            object resultado = InteractuarRedis(TipoAccesoRedis.Lectura, pRawKey, rawKeyOriginal, pGenerarClave);

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

        /// <summary>
        /// Obtiene un objeto de la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        /// <returns>Objeto</returns>
        protected List<object> ObtenerRangoDeSortedList(string pRawKey, int pNumeroElementos)
        {
            return ObtenerRangoDeSortedList(pRawKey, 1, pNumeroElementos);
        }

        /// <summary>
        /// Obtiene un objeto de la caché
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
                AgregarEntradaTraza("Redis. No está en caché: " + pRawKey);
            }

            if (listaResultado != null)
            {
                listaResultado.Reverse();
            }

            return listaResultado;
        }
        
        /// <summary>
        /// Obtiene un objeto de la caché
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
                AgregarEntradaTraza("Redis. No está en caché: " + pRawKey);
            }


            if (listaResultado != null)
            {
                listaResultado.Reverse();
            }

            return listaResultado;
        }

        /// <summary>
        /// Comprueba si existe una clave de caché.
        /// </summary>
        /// <param name="pRawKey">Clavé de caché que se desea buscar.</param>
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
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pDuracion">Duración del objeto de caché en segundos</param>
        public void AgregarCaducidadAObjetoCache(string pRawKey, double pDuracion)
        {
            InteractuarRedis(TipoAccesoRedis.CaducidadAObjeto, pRawKey, pDuracion);
        }

        /// <summary>
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        public string AgregarObjetoCache(string pRawKey, object pObjeto)
        {
            return AgregarObjetoCache(pRawKey, pObjeto, 0);
        }

        /// <summary>
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pDuracion">Duración del objeto de caché en segundos</param>
        public string AgregarObjetoCache(string pRawKey, object pObjeto, double pDuracion)
        {
            return AgregarObjetoCache(pRawKey, pObjeto, pDuracion, true);
        }

        /// <summary>
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pDuracion">Duración del objeto de caché en segundos</param>
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
        /// Comprueba si una clave de caché existe.
        /// </summary>
        /// <param name="pRawKey">Clave de caché que se va a comprobar si existe.</param>
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
                _loggingService.GuardarLogError(ex, null, true);
                //CerrarConexionCache();
            }

            return existe;
        }

        /// <summary>
        /// Renombrado de una clave de caché por otra
        /// </summary>
        /// <param name="pRawKey">Clavé de cache vieja</param>
        /// <param name="pRawKeyNueva">Clavé de cache Nueva</param>
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
        /// Agrega un objeto a la caché
        /// </summary>
        /// <param name="pRawKey">Clave del objeto</param>
        /// <param name="pObjeto">Objeto a agregar</param>
        /// <param name="pScore">Score de este objeto</param>
        /// <returns>Score asignado</returns>
        protected int AgregarObjetoASortedSet(string pRawKey, object pObjeto, int pScore)
        {
            pRawKey = ObtenerClaveCache(pRawKey).ToLower();

            byte[] bytesObjeto = ObjectToByteArray(pObjeto);

            AgregarEntradaTraza("Redis. Agrego a caché" + pRawKey);

            if (pScore.Equals(-1))
            {
                List<(double, string)> ultimoObjeto = null;

                //ultimoObjeto = ClienteRedisEscritura.ZRange(pRawKey, -1, -1);
                object objeto = InteractuarRedis(TipoAccesoRedis.Rango, pRawKey, pRawKey, 1, 1);
                if (objeto != null)
                {
                    ultimoObjeto = (List<(double,string)>)objeto;
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
        /// Elimina un objeto de la caché
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
                AgregarEntradaTraza("Redis. Elimino de caché " + pRawKey);
                ClienteRedisEscritura.CreateSequence(pRawKey).ZRem(pObjeto);

                AgregarEntradaTraza("Redis. Eliminado: " + pRawKey);
                correcto = true;
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, null, true);
                throw;
            }

            return correcto;
        }

        /// <summary>
        /// Invalida la caché
        /// </summary>
        public void InvalidarCache()
        {
            // Remove the cache dependency
            InvalidarCache(ClaveCache[0], true);
        }

        /// <summary>
        /// Quita un elemento de la caché
        /// </summary>
        /// <param name="pClaveCache">Clave del elemento que se va a quitar</param>
        public void InvalidarCache(string pClaveCache)
        {
            InvalidarCache(pClaveCache, true);
        }

        /// <summary>
        /// Quita un elemento de la caché
        /// </summary>
        /// <param name="pClaveCache">Clave del elemento que se va a quitar</param>
        /// <param name="pGenerarClave">Indica si se debe generar la clave, o coger la pasada como parametro</param>
        public void InvalidarCache(string pClaveCache, bool pGenerarClave)
        {
            InvalidarCache(pClaveCache, pGenerarClave, true);
        }

        /// <summary>
        /// Quita un elemento de la caché
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
        /// Quita un elemento de la caché
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
        /// Quita un elemento de la caché
        /// </summary>
        /// <param name="pListaClavesCache">Claves de los elementos que se van a quitar</param>
        public void InvalidarCachesMultiples(List<string> pListaClavesCache)
        {
            try
            {
                AgregarEntradaTraza("Redis. Invalido cachés multiples");
                foreach (string clave in pListaClavesCache)
                {
                    var t = ClienteRedisEscritura.Del(clave.ToLower()).Result;
                }
            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, null, true);

                //CerrarConexionCache();
            }

            AgregarEntradaTraza("Redis. Invalidadas cachés multiples");
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
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        /// <summary>
        /// Convierte un array de byte en un object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        protected object ByteArrayToObject(byte[] arrBytes)
        {
            object obj = null;
            if (arrBytes != null)
            {
                try
                {
                    MemoryStream memStream = new MemoryStream();
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(arrBytes, 0, arrBytes.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    obj = (object)binForm.Deserialize(memStream);
                }
                catch (Exception ex)
                {
                    //Ha Cambiado el modelo de datos de la cache, devolvemos null para que lo obtenga de Base de datos
                    return null;
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
        /// Método para interactuar con redis creando un hilo y aguardando un máximo de 2 décimas de segundo antes de abortar la conexión con redis.
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
                        resultado = ConsultasRedisPorTipo(pTipoAccesoRedis, pRawKey, out terminado, pParametrosExtra);
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
                        AgregarEntradaTraza("Redis. Acción redis '" + pTipoAccesoRedis + "' superior a " + mDecimasSegundoEsperaRedis * 100 + " milisegundos. " + pRawKey);
                    }
                    else
                    {
                        AgregarEntradaTraza("Redis. Acción redis '" + pTipoAccesoRedis + "' en " + (inicioHilo - finHilo) + " segundos. " + pRawKey);
                    }
                }
                /*else
                {
                    AgregarEntradaTraza("Redis '" + clienteRedis.Host + "' no ha respondido a nuestras peticiones 3 veces consecutivas, abortamos las querys 1 min.");
                }*/
            }
            else if (mLanzarExcepciones)
            {
                throw new RedisException("No se ha podido establecer una conexión a Redis.");
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
        /// Comprueba si ya ha pasado 1 minuto y se puede quitar el redis que estaba caído del diccionario.
        /// </summary>
        private void ComprobarEstadoRedis()
        {
            //Revisamos el estado de los redis, si ha pasado más de un minuto los quitamos del diccionario.
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
                object[] pObjetoArray = null;
                byte[] pObjetoBytes = null;
                (string, object)[] pArrayObjeto = null;
                Type[] ptype = null;
                string pObjetoString = null;
                string[] pRawkeyArray = null;
                bool pReintentar = false;
                string crear = null;

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
                        pGenerarClave = (bool)pParametrosExtra[1];
                        resultado = InteractuarRedis_Lectura(pRawKey, pRawKeyOriginal, pGenerarClave);
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
                        pRawkeyArray = (string[])(pParametrosExtra[0]);
                        pObjetoArray = (object[])(pParametrosExtra[1]);
                        if (pParametrosExtra.Length > 2)
                        {
                            pDuracion = (double)(pParametrosExtra[2]);
                        }
                        pArrayObjeto = ((string, object)[])(pParametrosExtra[3]);
                        InteractuarRedis_EscrituraDiccionarioObjetos(pRawkeyArray, pObjetoArray, pDuracion, pArrayObjeto);
                        break;
                    case TipoAccesoRedis.LecturaDiccionarioObjetos:
                        pRawkeyArray = (string[])pParametrosExtra[0];
                        ptype = (Type[])pParametrosExtra[1];
                        resultado = InteractuarRedis_LecturaDiccionarioObjetos(pRawkeyArray, ptype);
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
                        //var t = ClienteRedisLectura.CreateSequence(pRawKey).ZCard().Result;
                        //t.AsTask().Wait(500);
                        //if (!t.IsCompleted)
                        //{
                        //    return null;
                        //}
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
                        //var task1 = ClienteRedisLectura.CreateSequence(rawKeyOrigen).ZRange(0, -1);
                        //task1.AsTask().Wait(500);
                        //if (!task1.IsCompleted)
                        //{
                        //    return null;
                        //}
                        List<(double, string)> bytesCache = ClienteRedisLectura.CreateSequence(rawKeyOrigen).ZRange(0, -1).Result;
                        foreach ((double, string) elemento in bytesCache)
                        {
                            //resultado = ClienteRedisEscritura.CreateSequence(pRawKey).ZAdd((pScore, pObjetoString)).Result;
                            resultado = ClienteRedisEscritura.CreateSequence(rawKeyDestino).ZAdd((score,elemento.Item2)).Result;
                            score++;
                        }

                        resultado = score;
                        break;
                    case TipoAccesoRedis.LecturaListaObjetos:
                        /*var task3 = ClienteRedisLectura.Get<string>(pRawKey);
                        task3.AsTask().Wait(500);
                        if (!task3.IsCompleted)
                        {
                            return null;
                        }*/
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
                        //resultado = 1;
                        break;
                    case TipoAccesoRedis.ObtenerScoreObjeto:
                        pObjetoString = pParametrosExtra[0].ToString();
                        resultado = ClienteRedisEscritura.CreateSequence(pRawKey).ZScore((((double, string))pParametrosExtra[0]).Item2).Result;
                        //resultado = 1;
                        break;
                    case TipoAccesoRedis.Borrado:
                        pRawKeyOriginal = pParametrosExtra[0].ToString();
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
                //Llamamos a un método diferente para no mezclar los errores de redis con los de la web.
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
            AgregarEntradaTraza("Redis. Invalido caché de varias claves: " + pRawKeys.Count);
            foreach (string key in pRawKeys)
            {
                ClienteRedisEscritura.Del(key);
            }
        }

        private void InteractuarRedis_Borrado(string pRawKey, string pRawKeyOriginal, bool pGenerarClave, bool pReintentar)
        {
            AgregarEntradaTraza("Redis. Invalido caché " + pRawKey);
            long resultado = ClienteRedisEscritura.Del(pRawKey).Result;
        }

        private void InteractuarRedis_RenombrarClave(string pRawKey, string pRawKeyNueva)
        {
            AgregarEntradaTraza("Redis. Renombro la calve '" + pRawKey + "' por '" + pRawKeyNueva + "'");
            ClienteRedisEscritura.Rename(pRawKey, pRawKeyNueva);
            AgregarEntradaTraza("Redis. Remplazo con Éxito.");
        }

        private object InteractuarRedis_RangoPorScore(string pRawKey, string pRawKeyOriginal, int pMinScore, int pNumElementos)
        {
            //var t = ClienteRedisLectura.CreateSequence(pRawKey).ZRangeByScore(pMinScore.ToString(), pNumElementos.ToString());
            //t.AsTask().Wait(500);
            //if (!t.IsCompleted)
            //{
            //    return null;
            //}
            List<(double, string)> bytesCache = ClienteRedisLectura.CreateSequence(pRawKey).ZRangeByScore(pMinScore.ToString(), pNumElementos.ToString()).Result;

            return bytesCache;
        }

        private object InteractuarRedis_Rango(string pRawKey, string pRawKeyOriginal, int pInicio, int pFin)
        {
            //var t = ClienteRedisLectura.CreateSequence(pRawKey).ZRange(-pFin, -pInicio);
            //t.AsTask().Wait(500);
            //if (!t.IsCompleted)
            //{
            //    return null;
            //}
            List<(double, string)> bytesCache = ClienteRedisLectura.CreateSequence(pRawKey).ZRange(-pFin, -pInicio).Result;
            return bytesCache;
        }

        private object InteractuarRedis_LecturaDiccionarioObjetos(string[] pRawkeyArray, Type[] ptypes)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();

            AgregarEntradaTraza("Redis. Obtengo lista de claves de caché " + pRawkeyArray[0] + " ...");
            //var t = ClienteRedisLectura.MGet(pRawkeyArray, ptypes);
            //t.AsTask().Wait(1000);
            //if (!t.IsCompleted)
            //{
            //    return null;
            //}
            object[] objetosCache = ClienteRedisLectura.MGet(pRawkeyArray, ptypes).Result;

            for (int cont = 0; cont < pRawkeyArray.Length; cont++)
            {
                string clave = pRawkeyArray[cont];
                object objetoCache = objetosCache[cont];
                if (objetoCache != null && objetoCache is byte[])
                {
                    byte[] bytesObjeto = (byte[])objetoCache;
                    objetoCache = (object)ByteArrayToObject(bytesObjeto);
                }

                if (!resultado.ContainsKey(clave))
                {
                    resultado.Add(clave, objetoCache);
                }
            }


            return resultado;
        }

        private void InteractuarRedis_EscrituraDiccionarioObjetos(string[] pRawkeyArray, object[] pObjetosCache, double pDuracion, (string, object)[] pObjeto)
        {
            AgregarEntradaTraza("Redis. Agrego lista de claves de caché " + pRawkeyArray[0] + " ...");
            //byte[][] listaObjetos = new byte[pObjetosCache.Length][];
            int i = 0;
            (string, object)[] listaObjetos = new (string, object)[pObjeto.Length];
            foreach ((string, object) objeto in pObjeto)
            {
                listaObjetos[i].Item1 = objeto.Item1;
                listaObjetos[i].Item2 = ObjectToByteArray(objeto.Item2);
                i++;
            }
            //ClienteRedisEscritura.MSet(pRawkeyArray, listaObjetos);
            ClienteRedisEscritura.MSet(pObjeto);



            foreach (string rawKey in pRawkeyArray)
            {
                if (pDuracion > 0)
                {
                    ClienteRedisEscritura.Expire(rawKey, (int)pDuracion);
                }
            }

            AgregarEntradaTraza("Redis. Agregado lista de claves de caché " + pRawkeyArray[0] + " ...");
        }

        private object InteractuarRedis_LecturaVariosObjetos(string[] pRawkeyArray, Type[] ptypes)
        {
            List<object> listaResultados = new List<object>();
            //var t = ClienteRedisLectura.MGet(pRawkeyArray, ptypes);
            //t.AsTask().Wait(1000);
            //if (!t.IsCompleted)
            //{
            //    return null;
            //}
            object[] objetosCache = ClienteRedisLectura.MGet(pRawkeyArray, ptypes).Result;
            AgregarEntradaTraza("Redis. Obtengo clave de caché");
            if (objetosCache != null && objetosCache.Length > 0)
            {
                AgregarEntradaTraza("Redis. Existe");
                foreach (object objeto in objetosCache)
                {
                    if (objeto != null && objeto is byte[])
                    {
                        byte[] bytesObjeto = (byte[])objeto;
                        object objetoCache = (object)ByteArrayToObject(bytesObjeto);
                        AgregarEntradaTraza("Redis. Obtenido: " + pRawkeyArray);
                        listaResultados.Add(objetoCache);
                    }
                }
            }
            else
            {
                AgregarEntradaTraza("Redis. No está en caché: " + pRawkeyArray);
            }

            return listaResultados;
        }

        private void InteractuarRedis_Escritura(string pRawKey, string pRawKeyOriginal, object pObjetoCache, double pDuracion)
        {
            AgregarEntradaTraza("Redis. Agrego a caché " + pRawKey);
            string insertar = "";
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

        private object InteractuarRedis_Lectura(string pRawKey, string pRawKeyOriginal, bool pGenerarClave)
        {
            object resultado = null;

            AgregarEntradaTraza("Redis. Obtengo clave de caché " + pRawKey);
            //var t = ClienteRedisLectura.Get<byte[]>(pRawKey);
            ////var task = Task.Run<byte[]>(async () => { return await ClienteRedisLectura.Get<byte[]>(pRawKey); });
            ////task.Wait(500);
            //t.AsTask().Wait(1000);
            ////t.AsTask().
            //if (!t.IsCompleted)
            //{
            //    return null;
            //}
            byte[] bytesCache = null;
            try
            {
                 bytesCache = ClienteRedisLectura.Get<byte[]>(pRawKey).Result;
            }
            catch (RedisException redisException)
            {
                _loggingService.GuardarLogError($"Error al obtener la clave {pRawKey} de cache. \nError: {redisException}");
            }
            
            if (bytesCache != null && bytesCache.Length > 0)
            {
                AgregarEntradaTraza("Redis. Existe " + pRawKey);
                resultado = ByteArrayToObject(bytesCache);
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
                AgregarEntradaTraza("Redis. No está en caché: " + pRawKey);
            }

            return resultado;
        }

        private static object BLOQUEO_INICIAR_CONEXION = new object();

        private object InteractuarRedis_Comprobacion(string pRawKey, string pRawKeyOriginal)
        {
            AgregarEntradaTraza("Redis. Compruebo existe clave" + pRawKey);
            object resultado = ClienteRedisLectura.Exists(pRawKey).Result;
            AgregarEntradaTraza("Redis. Comprobado: " + pRawKey);

            return resultado;
        }

        #region Cache Local

        /// <summary>
        /// Devuelve el dato correspondiente a la clave de caché local
        /// </summary>
        /// <param name="pRawKey">Clave de caché que se va a buscar en local</param>
        /// <returns>Objeto solicitado</returns>
        public object ObtenerObjetoDeCacheLocal(string pRawKey)
        {
            if (!UsarCacheLocal.Equals(UsoCacheLocal.Nunca) && EjecucionDeAplicacionWeb)
            {
                if (pRawKey == null)
                {
                    pRawKey = this.ObtenerClaveCache(pRawKey);
                }

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
        /// Agrega en la caché local el dato pDato con la clave de caché pRawKey
        /// </summary>
        /// <param name="pRawKey">Clave de caché que va a guardar el objeto</param>
        /// <param name="pDato">Dato que se va a guardar con la clave de caché.</param>
        public void AgregarObjetoCacheLocal(Guid pProyectoID, string pRawKey, object pDato, bool pObtenerParametroSiempre = false, DateTime? pExpirationDate = null)
        {
            if (pRawKey == null)
            {
                pRawKey = this.ObtenerClaveCache(pRawKey);
            }
            if (!pExpirationDate.HasValue)
            {
                pExpirationDate = DateTime.Now.AddYears(1);
            }

            if (!UsarCacheLocal.Equals(UsoCacheLocal.Nunca) && EjecucionDeAplicacionWeb && pDato != null && (pObtenerParametroSiempre || UsarCacheLocal.Equals(UsoCacheLocal.Siempre)))
            {
                _loggingService.AgregarEntrada("CacheLocal: Añado " + pRawKey);

                string fileName = $"{AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/config/versionCacheLocal/{pProyectoID}.config";
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
                string ruta = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}/config/versionCacheLocal";
                if (!Directory.Exists(ruta))
                {
                    Directory.CreateDirectory(ruta);
                }

                string fileName = $"{ruta}/{pProyectoID}.config";
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

            if (pIP.Contains("|"))
            {
                // El servidor tiene contraseña
                string[] ipPassword = pIP.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                pIP = ipPassword[0];

                if (ipPassword.Length > 1)
                {
                    password = ipPassword[1];
                }
            }

            if (!string.IsNullOrEmpty(password))
            {
                //clienteRedis = new RedisDB(pIP, REDIS_DEFAULT_PORT, password);
                clienteRedis = new RedisDB(db, new JsonFormater());
                clienteRedis.AutoPing = false;
                //RedisCacheWrapper.GuardarLog($"Conexion creada: {pIP} {db}");
                clienteRedis.Host.AddWriteHost(pIP).Password = password;
                //clienteRedis.Host.AddWriteHost(pIP).MaxConnections = 5000;
                clienteRedis.Host.AddReadHost(pIP).Password = password;
                //clienteRedis.Host.AddReadHost(pIP).MaxConnections = 5000;
            }
            else
            {
                clienteRedis = new RedisDB(db, new JsonFormater());
                clienteRedis.AutoPing = false;
                //RedisCacheWrapper.GuardarLog($"Conexion creada: {pIP} {db}");
                clienteRedis.Host.AddWriteHost(pIP);
                clienteRedis.Host.AddReadHost(pIP);
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


                //Si no había configuración para este dominio, o no estamos en una aplicación Web, cojo la configuración por defecto
                ParametroAplicacion filaParametroTamanioRedis = _entityContext.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro == ParametroAD.TamanioPoolRedis);
                //ParametroAplicacionDS.ParametroAplicacionRow filaParametroTamanioRedis = paramAplicDS.ParametroAplicacion.FindByParametro(ParametroAD.TamanioPoolRedis);

                if (filaParametroTamanioRedis != null)
                {
                    int.TryParse(filaParametroTamanioRedis.Valor, out tamanio);
                }

            }
            catch (Exception ex)
            {
                _loggingService.GuardarLogError(ex, null, true);
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
                    // Se saca fuera para que no se compruebe siempre el nodoDB de la petición antes de devolverla
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
                    //TODO DELETE
                    //nodoDB = 0;
                    //Fin delete
                    if (mClienteRedisLectura == null)
                    {
                        //BeetleX.Buffers.BufferPool.BUFFER_SIZE = 2400000;
                        string cadenaPeticion = nodoIPMaster;
                        Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                        mClienteRedisLectura = _redisCacheWrapper.RedisLectura(mPoolName);
                        _loggingService.AgregarEntrada("ClienteRedisLectura: Para '" + mPoolName + "' ¿existe en petición actual?" + (mClienteRedisLectura != null));

                        if (mClienteRedisLectura == null)
                        {
                            try
                            {
                                mClienteRedisLectura = ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                                //mClienteRedisLectura = new RedisClient(nodoIP);
                                //mClienteRedisLectura.ConnectTimeout = redisTimeOut;
                                //mClienteRedisLectura.RetryCount = 3;
                                mClienteRedisLectura.DB = nodoDB;
                                if (!string.IsNullOrEmpty(nodoIPRead))
                                {
                                    mClienteRedisLectura.Host.AddReadHost(nodoIPRead);
                                }
                                

                                _loggingService.AgregarEntradaDependencia($"ClienteRedisLectura: cargamos para la ip '{nodoIPMaster}' y BD '{nodoDB}'", false, "Cliente Redis Lectura", sw, true);

                                _redisCacheWrapper.AddRedisLectura(mPoolName, mClienteRedisLectura);
                                _loggingService.AgregarEntrada("ClienteRedisLectura: Agregado a la petición actual");
                            }
                            catch (Exception ex)
                            {
                                _loggingService.AgregarEntradaDependencia($"ClienteRedisLectura: Error al conectar con redis {mPoolName} a la IP {nodoIPMaster} con puerto {nodoDB}", false, "Cliente Redis Lectura", sw, false);
                                _loggingService.GuardarLogError(ex, string.Format("Error al conectar con redis {0}, a la IP {1} con puerto {2}", mPoolName, nodoIPMaster, nodoDB), true);
                            }
                        }
                    }

                    //Este código es inutil porque la conexión ya esta creada, por lo que si no es igual habria que desecharla ->  HECHO
                    if (!mClienteRedisLectura.DB.Equals(nodoDB))
                    {
                        mClienteRedisLectura = ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                        _redisCacheWrapper.AddRedisLectura(mPoolName, mClienteRedisLectura);
                    }

                    return mClienteRedisLectura;
                }
                catch (Exception ex)
                {
                    _loggingService.AgregarEntrada($"ClienteRedisLectura: Error al conectar con redis { mPoolName}");
                    _loggingService.GuardarLogError(ex, $"Error al conectar con redis {mPoolName}", true);
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
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                    string ComunidadPadreEcosistemaID = paramCN.ObtenerParametroAplicacion("ComunidadPadreEcosistemaID");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(ComunidadPadreEcosistemaID))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                            mNombreProyectoPadreEcositema = proyCN.ObtenerNombreCortoProyecto(Guid.Parse(ComunidadPadreEcosistemaID));
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.");
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
                    ParametroAplicacionCN paramCN = new ParametroAplicacionCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                    string NombreCortoEcosistema = paramCN.ObtenerParametroAplicacion("NombreCortoProyectoPadreEcositema");
                    paramCN.Dispose();
                    if (!string.IsNullOrEmpty(NombreCortoEcosistema))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                            mNombreCortoProyectoPadreEcosistema = NombreCortoEcosistema;
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.");
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
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreCortoProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro NombreCortoProyectoPadreEcositema no esta bien configurado.");
                            mPadreEcosistemaProyectoID = Guid.Empty;
                        }
                    }

                    if (!string.IsNullOrEmpty(NombreProyectoPadreEcositema) && (mPadreEcosistemaProyectoID == null || (mPadreEcosistemaProyectoID != null && mPadreEcosistemaProyectoID.Value.Equals(Guid.Empty))))
                    {
                        try
                        {
                            ProyectoCN proyCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
                            mPadreEcosistemaProyectoID = proyCN.ObtenerProyectoIDPorNombreCorto(NombreProyectoPadreEcositema);
                            proyCN.Dispose();
                        }
                        catch
                        {
                            _loggingService.GuardarLogError("El parametro ComunidadPadreEcosistemaID no esta bien configurado.");
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

                    // Se saca fuera para que no se compruebe siempre el nodoDB de la petición antes de devolverla
                    string poolNameEscritura = mPoolName.Replace("acid", "redis");

                    string nodoIPMaster = _configService.ObtenerConexionRedisIPMaster(mPoolName);
                    int nodoDB = _configService.ObtenerConexionRedisBD(mPoolName);
                    int redisTimeOut = _configService.ObtenerConexionRedisTimeout(mPoolName);
                    //TODO DELETE
                    //nodoDB = 0;
                    //Fin delete
                    if (mClienteRedisEscritura == null)
                    {
                        //BeetleX.Buffers.BufferPool.BUFFER_SIZE = 2400000;
                        string cadenaPeticion = nodoIPMaster;
                        Stopwatch sw = LoggingService.IniciarRelojTelemetria();

                        mClienteRedisEscritura = _redisCacheWrapper.RedisEscritura(mPoolName);
                        _loggingService.AgregarEntrada("ClienteRedisEscritura: Para '" + mPoolName + "' ¿existe en petición actual?" + (mClienteRedisEscritura != null));

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

                                _redisCacheWrapper.AddRedisEscritura(mPoolName, mClienteRedisEscritura);
                                _loggingService.AgregarEntrada("ClienteRedisEscritura: Agregado a la petición actual");
                            }
                            catch (Exception ex)
                            {
                                _loggingService.AgregarEntradaDependencia($"ClienteRedisEscritura: Error al conectar con redis {mPoolName} a la IP {nodoIPMaster} con puerto {nodoDB}", false, "Cliente Redis Escritura", sw, false);
                                _loggingService.GuardarLogError(ex, null, true);
                            }
                        }
                    }

                    //Este código es inutil porque la conexión ya esta creada, por lo que si no es igual habria que desecharla -> HECHO
                    if (!mClienteRedisEscritura.DB.Equals(nodoDB))
                    {
                        mClienteRedisEscritura = ObtenerClienteRedisParaIP(nodoIPMaster, nodoDB);
                        _redisCacheWrapper.AddRedisEscritura(mPoolName, mClienteRedisEscritura);
                    }

                    return mClienteRedisEscritura;
                }
                catch (Exception ex)
                {
                    _loggingService.AgregarEntrada($"ClienteRedisEscritura: Error al conectar con redis { mPoolName}");
                    _loggingService.GuardarLogError(ex, $"Error al conectar con redis {mPoolName}", true);
                    return null;
                }
            }
        }

        /// <summary>
        /// Devuelve la clave para la caché
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
        /// Obtiene si la aplicación actual es un entorno Web. 
        /// </summary>
        protected bool EjecucionDeAplicacionWeb
        {
            get
            {
                return _redisCacheWrapper.Cache != null;
            }
        }

        /// <summary>
        /// Obtiene el tamaño del pool de conexiones hacia Redis
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
            ProyectoCN proCN = new ProyectoCN(_entityContext, _loggingService, _configService, mServicesUtilVirtuosoAndReplication);
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
            AgregarObjetoCache(GnossCacheCL.CLAVE_REFRESCO_CACHE_LOCAL + pProyectoID, Guid.NewGuid());
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
        /// Determina si está disposed
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
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
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
                    _loggingService.GuardarLogError(ex, null, true);
                }
            }
        }

        #endregion

        #endregion
    }
}
