using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Es.Riam.Util.AnalisisSintactico;
using LumenWorks.Framework.IO.Csv;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenLink.Data.Virtuoso;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.AbstractsOpen;
using System.Collections.Concurrent;
using Es.Riam.Semantica.Plantillas;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Semantica.OWL;
using System.Xml;
using Es.Riam.Gnoss.Util.Seguridad;
using Microsoft.Exchange.WebServices.Data;
using System.Net.Http;
using Es.Riam.Gnoss.Web.MVC.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Es.Riam.Gnoss.AD.Facetado
{

    /// <summary>
    /// Tipos de búsqueda
    /// </summary>
    public enum TipoBusqueda
    {
        /// <summary>
        /// Recursos
        /// </summary>
        Recursos = 0,
        /// <summary>
        /// Debates
        /// </summary>
        Debates,
        /// <summary>
        /// Preguntas
        /// </summary>
        Preguntas,
        /// <summary>
        /// Encuestas
        /// </summary>
        Encuestas,
        /// <summary>
        /// Dafos
        /// </summary>
        Dafos,
        /// <summary>
        /// Personas y organizaciones
        /// </summary>
        PersonasYOrganizaciones,
        /// <summary>
        /// Búsqueda avanzada
        /// </summary>
        BusquedaAvanzada,
        /// <summary>
        /// Comunidades
        /// </summary>
        Comunidades,
        /// <summary>
        /// Blogs
        /// </summary>
        Blogs,
        /// <summary>
        /// ArticuloBlogs
        /// </summary>
        ArticuloBlogs,
        /// <summary>
        /// EditarRecursosPerfil
        /// </summary>
        EditarRecursosPerfil,
        /// <summary>
        /// Contribuciones
        /// </summary>
        Contribuciones,
        /// <summary>
        /// Mensajes.
        /// </summary>
        Mensajes,
        /// <summary>
        /// Comentarios.
        /// </summary>
        Comentarios,
        /// <summary>
        /// Invitaciones.
        /// </summary>
        Invitaciones,
        /// <summary>
        /// Suscripciones.
        /// </summary>
        Suscripciones,
        /// <summary>
        /// Contactos
        /// </summary>
        Contactos,
        /// <summary>
        /// Recomendaciones.
        /// </summary>
        Recomendaciones,
        /// <summary>
        /// Recomendaciones de proyectos.
        /// </summary>
        RecomendacionesProys,
        /// <summary>
        /// VerRecursosPerfil
        /// </summary>
        VerRecursosPerfil,
        /// <summary>
        /// Notificaciones (comentarios e invitaciones juntas)
        /// </summary>
        Notificaciones

    }

    /// <summary>
    /// Tipo de ficha para pintar los resultados de las búsquedas.
    /// </summary>
    public enum TipoFichaResultados
    {
        /// <summary>
        /// Todos los datos.
        /// </summary>
        Completa = 0,
        /// <summary>
        /// Solo se debe pintar el título y el icono del elemento.
        /// </summary>
        SoloIconoYTitulo = 1,
        /// <summary>
        /// Ficha para los contextos
        /// </summary>
        Contexto = 2,
        /// <summary>
        /// Ficha con el Título y la descripción recortada con el 1º parrafo que tenga algo y sin html.
        /// </summary>
        TituloYDescRecPrimerPSinHTML = 3,
        /// <summary>
        /// Ficha en XML
        /// </summary>
        XML = 4,
        /// <summary>
        /// Descripción completa
        /// </summary>
        DescripcionCompleta = 5
    }

    /// <summary>
    /// Configuración extra para los grafos de las fichas de los recursos.
    /// </summary>
    public enum ConfiguracionGrafoFichaRec
    {
        /// <summary>
        /// Para agrupar tipos
        /// </summary>
        AgruparTipo = 0,
        /// <summary>
        /// Para agrupar subtipos
        /// </summary>
        AgruparSubTipo = 1,
        /// <summary>
        /// Tipos recíprocos
        /// </summary>
        TiposReciprocos = 2,
        /// <summary>
        /// Nodos maxímos por recurso
        /// </summary>
        NodosMaxRecurso = 3,
        /// <summary>
        /// Pestaña de búsqueda
        /// </summary>
        PestanyaBusqueda = 4,
        /// <summary>
        /// Para excluir una propiedad de relación de grafos excepto para ciertos tipos principales.
        /// </summary>
        SuprimirPropExceptoTipoPrinc = 5,
        /// <summary>
        /// Para excluir una propiedad de relación de grafos excepto para ciertos subtipos principales.
        /// </summary>
        SuprimirPropExceptoSubTipoPrinc = 6
    }

    /// <summary>
    /// DataAdapter para el facetado (Virtuoso)
    /// </summary>
    public class FacetadoAD : BaseAD
    {

        #region Constantes

        #region Privados

        private const string SOLO_TRAER_RDFTYPE = "?s rdf:type ?rdftype. ";

        private const string TIPOS_METABUSCADOR = " ?s rdf:type ?rdftype. FILTER (?rdftype in ('Recurso', 'RecursoPerfilPersonal', 'Persona', 'Organizacion', 'Dafo', 'Pregunta', 'Debate', 'Encuesta','Comunidad', 'comunidad no educativa', 'comunidad educativa'  ))";

        private const string TIPOS_METABUSCADOR_USUARIO_INVITADO = " ?s rdf:type ?rdftype. FILTER (?rdftype in ('Recurso', 'RecursoPerfilPersonal', 'Dafo', 'Pregunta', 'Debate', 'Encuesta','Comunidad', 'comunidad no educativa', 'comunidad educativa'   ))";

        private const string TIPOS_METABUSCADOR_MYGNOSS = " ?s rdf:type ?rdftype. FILTER (?rdftype in ('Recurso', 'RecursoPerfilPersonal', 'Persona', 'Organizacion', 'Dafo', 'Pregunta', 'Debate', 'Encuesta','Blog','Comunidad', 'comunidad no educativa', 'comunidad educativa'    ))";

        public static readonly string[] TIPOS_GEOMETRIA = new string[] { "linestring", "multilinestring", "multipoint", "multipolygon", "polygon", "point", "geometrycollection", "multicurve" };

        private static List<string> CARACTERES_REGEX_ACENTOS = new List<string>() { "aáàäâ", "eéèëê", "iíìïî", "oóòöô", "uúùüû", "cç", "nñ" };

        private static char[] SEPARADORES_PALABRAS = { ' ', '.', ',', ':', ';', '-', '/', '(', ')' };

        #endregion

        /// <summary>
        /// Recursos
        /// </summary>
        public const string BUSQUEDA_RECURSOS = "Recurso";

        /// <summary>
        /// Cine
        /// </summary>
        public const string BUSQUEDA_PELICULAS = "cine";

        /// <summary>
        /// Receta
        /// </summary>
        public const string BUSQUEDA_RECETAS = "arecipe";

        /// <summary>
        /// Factura
        /// </summary>
        public const string BUSQUEDA_FACTURAS = "invoice";

        /// <summary>
        /// Factura
        /// </summary>
        public const string BUSQUEDA_PRODUCTOGARNICA = "productGarnica";

        /// <summary>
        /// Unidad didáctica.
        /// </summary>
        public const string BUSQUEDA_UNIDADDIDACTICA = "unidadDidactica";

        /// <summary>
        /// Debates
        /// </summary>
        public const string BUSQUEDA_DEBATES = "Debate";

        /// <summary>
        /// Preguntas
        /// </summary>
        public const string BUSQUEDA_PREGUNTAS = "Pregunta";

        /// <summary>
        /// Preguntas
        /// </summary>
        public const string BUSQUEDA_ENCUESTAS = "Encuesta";

        /// <summary>
        /// Recursos de los perfiles personales
        /// </summary>
        public const string BUSQUEDA_RECURSOS_PERSONALES = "RecursoPerfilPersonal";

        /// <summary>
        /// Personas y organizaciones
        /// </summary>
        public const string BUSQUEDA_PERSONASYORG = "PerYOrg";

        /// <summary>
        /// Personas
        /// </summary>
        public const string BUSQUEDA_PERSONA = "Persona";

        /// <summary>
        /// Grupo
        /// </summary>
        public const string BUSQUEDA_GRUPO = "Grupo";

        /// <summary>
        /// Identificador de las páginas del CMS
        /// </summary>
        public const string PAGINA_CMS = "PaginaCMS";
        /// <summary>
        /// Identificador de los componentes CMS
        /// </summary>
        public const string COMPONENTE_CMS = "ComponenteCMS";

        /// <summary>
        /// Profesores
        /// </summary>
        public const string BUSQUEDA_PROFESOR = "profesor";

        /// <summary>
        /// Alumnos
        /// </summary>
        public const string BUSQUEDA_ALUMNO = "alumno";

        /// <summary>
        /// Organizaciones
        /// </summary>
        public const string BUSQUEDA_ORGANIZACION = "Organizacion";

        /// <summary>
        /// Clase
        /// </summary>
        public const string BUSQUEDA_CLASE = "clase";

        /// <summary>
        /// Clase de universidad
        /// </summary>
        public const string BUSQUEDA_CLASE_UNIVERSIDAD = "clase de universidad";

        /// <summary>
        /// Clase de secundaria
        /// </summary>
        public const string BUSQUEDA_CLASE_SECUNDARIA = "clase de secundaria";

        /// <summary>
        /// Dafos
        /// </summary>
        public const string BUSQUEDA_DAFOS = "Dafo";

        /// <summary>
        /// Blogs
        /// </summary>
        public const string BUSQUEDA_BLOGS = "Blog";

        /// ArticuloBlogs
        /// </summary>
        public const string BUSQUEDA_ARTICULOSBLOG = "ArticuloBlog";

        /// <summary>
        /// Comunidades
        /// </summary>
        public const string BUSQUEDA_COMUNIDADES = "Comunidad";

        /// <summary>
        /// Comunidades recomendadas
        /// </summary>
        public const string BUSQUEDA_COMUNIDADES_RECOMENDADAS = "Comunidad recomendada";

        /// <summary>
        /// comunidad educativa
        /// </summary>
        public const string BUSQUEDA_COMUNIDAD_EDUCATIVA = "Comunidad educativa";

        /// <summary>
        /// comunidad no educativa
        /// </summary>
        public const string BUSQUEDA_COMUNIDAD_NO_EDUCATIVA = "Comunidad no educativa";

        /// <summary>
        /// Metabuscador
        /// </summary>
        public const string BUSQUEDA_AVANZADA = "Meta";

        /// <summary>
        /// Contribuciones Preguntas
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_PREGUNTA = "Contribuciones en Preguntas";

        /// <summary>
        /// Contribuciones Debates
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_DEBATE = "Contribuciones en Debates";

        /// <summary>
        /// Contribuciones Factores de Dato
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_FACTORDAFO = "Contribuciones en Factores de dafo";

        /// <summary>
        /// Contribuciones Comentarios
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_ENCUESTA = "Contribuciones en Encuestas";

        /// <summary>
        /// Contribuciones Comentarios
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMENTARIOS = "Comentarios";

        /// <summary>
        /// Contribuciones Comentarios en Recursos
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMRECURSOS = "Comentarios en recursos";

        /// <summary>
        /// Contribuciones Comentarios en Preguntas
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS = "Comentarios en preguntas";

        /// <summary>
        /// Contribuciones Comentarios en Debates
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMDEBATES = "Comentarios en debates";

        /// <summary>
        /// Contribuciones Comentarios en Encuestas
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMENCUESTAS = "Comentarios en encuestas";

        /// <summary>
        /// Contribuciones Comentarios en Encuestas
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMFACTORDAFO = "Comentarios en factor de dafo";

        /// <summary>
        /// Contribuciones Comentarios en Encuestas
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMARTICULOBLOG = "Comentarios en artículo de blog";

        /// <summary>
        /// Contribuciones Recursos
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_RECURSOS = "Contribuciones en Recursos";

        /// <summary>
        /// Contribuciones Compartido
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_COMPARTIDO = "Recurso Compartido";

        /// <summary>
        /// Contribuciones Publicado
        /// </summary>
        public const string BUSQUEDA_CONTRIBUCIONES_PUBLICADO = "Recurso Publicado";

        /// <summary>
        /// Recursos
        /// </summary>
        public const string BUSQUEDA_RECURSOS_PERFIL = "RecursoPerfilPersonal";

        /// <summary>
        /// Recursos
        /// </summary>
        public const string BUSQUEDA_MENSAJES = "Mensaje";

        /// <summary>
        /// Comentarios
        /// </summary>
        public const string BUSQUEDA_COMENTARIOS = "Comentario";

        /// <summary>
        /// Invitaciones.
        /// </summary>
        public const string BUSQUEDA_INVITACIONES = "Invitacion";

        /// <summary>
        /// Suscripciones.
        /// </summary>
        public const string BUSQUEDA_SUSCRIPCIONES = "Suscripcion";

        /// <summary>
        /// Contacto Personal
        /// </summary>
        public const string BUSQUEDA_CONTACTOS_PERSONAL = "ContactoPer";

        /// <summary>
        /// Contacto Organizacion    
        /// </summary>
        public const string BUSQUEDA_CONTACTOS_ORGANIZACION = "ContactoOrg";

        /// <summary>
        /// Contacto Grupo
        /// </summary>
        public const string BUSQUEDA_CONTACTOS_GRUPO = "ContactoGrupo";

        /// <summary>
        /// Contactos
        /// </summary>
        public const string BUSQUEDA_CONTACTOS = "Contactos";

        public const string BUSQUEDA_MIS_RECURSOS = "Mis Recursos";

        /// <summary>
        /// Indica el valor sin foto de virtuoso.
        /// </summary>
        public const string SIN_FOTO = "sinfoto";

        /// <summary>
        /// Texto para el filtro sin especificar
        /// </summary>
        public const string FILTRO_SIN_ESPECIFICAR = "not-specified";

        public const string RDF_TYPE = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        public const string PARTE_FILTRO_RECIPROCO = "filtroReciproco";

        public const string FACETA_CONDICIONADA = "@@COND@@";

        public const string NOTHING = "@nothing";

        public const string XSD_STRING = "http://www.w3.org/2001/XMLSchema#string";

        public const string MANDATORY = "Mandatory";

        private const string SIGNOS_ELIMINAR_SEARCH = ",.;¿?!¡:";

        #endregion

        #region Miembros estáticos

        /// <summary>
        /// Lista de tipos para el base.
        /// </summary>
        private static List<string> mListaTiposBase;

        /// <summary>
        /// Configuración de servidores para los grafos
        /// </summary>
        private static DataWrapperFacetas mServidoresGrafo = null;

        /// <summary>
        /// Verdad si existe el fichero bd.config con el elemento acidMaster, falso en caso contrario
        /// </summary>

        //Búsqueda con/sin acentos
        private const string consignos = "âáàäêéèëîíìïôóòöûúüùñÂÁÀÄÊÉÈËÎÍÌÏÔÓÒÖÛÚÙÜÑçÇ";
        private const string sinsignos = "aaaaeeeeiiiioooouuuunAAAAEEEEIIIIOOOOUUUUNcC";

        /// <summary>
        /// Idiomas que tienenen como idioma secundario el español
        /// </summary>
        private static readonly List<string> SECUNDARIO_ESPANNOL = new List<string>(new string[] { "ca", "gl", "eu" });

        /// <summary>
        /// Indica si se trata de una petición de facetas
        /// </summary>
        private static bool mEsPeticionFacetas = false;
        private static bool mUsarPrivacidadEnFacetasYResultados = true;

        private static bool mEscaparComillasDoblesPorHilo = false;

        private static string mEvaluarFiltrosFacetasEnOrden = "";

        private static readonly string[] mTiposBusquedasEspacioPersonal = new string[] { BUSQUEDA_RECURSOS_PERFIL, BUSQUEDA_CONTRIBUCIONES_RECURSOS };

        #endregion

        #region Miembros

        /// <summary>
        /// Namespaces necesarios para consultar virtuoso
        /// </summary>
        private string mNamespacesVirtuoso = "";

        /// <summary>
        /// Variable para el bloqueo de los namespaces
        /// </summary>
        private readonly object mBloqueoNamespacesVirtuoso = new object();

        /// <summary>
        /// Url de la Intranet
        /// </summary>
        private readonly string mUrlIntranet;

        /// <summary>
        /// Lista de las comunidades privadas de un usuario
        /// </summary>
        private List<Guid> mListaComunidadesPrivadasUsuario = new List<Guid>();


        /// <summary>
        /// Lista con los nombres de ontologias del proyecto donde se esta buscando
        /// </summary>
        private List<string> mListaItemsBusquedaExtra = new List<string>();


        /// <summary>
        /// Lista con las propiedades que serán mostradas con rangos
        /// </summary>
        private List<string> mPropiedadesRango;


        /// <summary>
        /// Lista con las propiedades que serán mostradas con fecha
        /// </summary>
        private List<string> mPropiedadesFecha;


        /// <summary>
        /// Diccionario con los nombres de ontologias del proyecto donde se esta buscando y su prefijo
        /// </summary>
        private Dictionary<string, List<string>> mInformacionOntologias;

        /// <summary>
        /// Objetos por los cuales se va a filtrar y no se va a cambiar su parametro de busqueda. EJ: cotecmembership0, cotecmembership1, cotecmembership3 en la busqueda
        /// </summary>
        private string mMandatoryRelacion;

        /// <summary>
        /// Almacena la cadena de conexión a la base de datos BASE para insertar en la cola de replicación las transacciones de actualización
        /// </summary>
        private string mCadenaConexionBase = "base";



        /// <summary>
        /// Variable para saber cuando hay que enviar filas al servicio de replicación de virtuoso
        /// </summary>
        private static bool? mReplicacion = true;

        /// <summary>
        /// Directorio de escritura de triples especial para DBLP
        /// </summary>
        private static string DirectorioEscrituraNT = "";

        /// <summary>
        /// Condición extra para la consulta de facetas.
        /// </summary>
        private string mCondicionExtraFacetas;

        private readonly ConcurrentDictionary<Guid, List<Guid>> mListaGruposPorIdentidad = new ConcurrentDictionary<Guid, List<Guid>>();

        /// <summary>
        /// Lista que indica que identidades tienes grupos de comunidad y cuales no.
        /// </summary>
        private ConcurrentDictionary<Guid, bool> mIdentidadTieneGruposComunidad;

        bool mConsultaNumeroResultados = false;

        bool mConsultaResultados = false;

        private static Dictionary<string, bool> mDiccionarioFacetasExcluyentes = new Dictionary<string, bool>();

        private DataWrapperFacetas mFacetaDW;

        private int mNumeroReciproca = 0;

        private static string mMaxExecutionPlanLimit;

        private readonly VirtuosoAD mVirtuosoAD;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public FacetadoAD(string pUrlIntragnoss, LoggingService loggingService, EntityContext entityContext, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mUrlIntranet = pUrlIntragnoss;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }


        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pTipoBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        public FacetadoAD(string pTipoBD, string pUrlIntragnoss, LoggingService loggingService, EntityContext entityContext, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoAD> logger,ILoggerFactory loggerFactory)
            : base(pTipoBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mUrlIntranet = pUrlIntragnoss;
            mlogger = logger;
            mloggerFactory= loggerFactory;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pTipoBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pTablaReplica">Tabla donde se va a insertar la consulta ("ColaReplicacionMaster" o "ColaReplicacionMasterHome")</param>
        public FacetadoAD(string pTipoBD, string pUrlIntragnoss, string pTablaReplica, LoggingService loggingService, EntityContext entityContext, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoAD> logger,ILoggerFactory loggerFactory)
            : base(pTipoBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mVirtuosoAD = virtuosoAD;
            mUrlIntranet = pUrlIntragnoss;
            mlogger = logger;
            mloggerFactory = loggerFactory;
            if (!string.IsNullOrEmpty(pTablaReplica)) mServicesUtilVirtuosoAndReplication.TablaReplicacion = pTablaReplica;
        }

        /// <summary>
        /// Constructor para el usuario que tiene permiso de ver todas las personas
        /// </summary>
        /// <param name="pUrlIntragnoss">URL de intragnoss</param>
        /// <param name="pObtenerPrivados">Verdad si el usuario actual puede ver los privados</param>
        public FacetadoAD(string pUrlIntragnoss, bool pObtenerPrivados, LoggingService loggingService, EntityContext entityContext, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesBidirectionalReplication, ILogger<FacetadoAD> logger,ILoggerFactory loggerFactory)
            : this(pUrlIntragnoss, loggingService, entityContext, configService, virtuosoAD, servicesBidirectionalReplication,logger,loggerFactory)
        {
            ObtenerPrivados = pObtenerPrivados;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }

        #endregion

        #region Propiedades

        public static bool Replicacion
        {
            get
            {
                if (!mReplicacion.HasValue)
                {
                    bool replicacion = true;
                    string pathFicheroReplicacionTimeout = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "replicacion.config");
                    if (File.Exists(pathFicheroReplicacionTimeout))
                    {
                        string contenido = File.ReadAllText(pathFicheroReplicacionTimeout).Trim();
                        bool.TryParse(contenido, out replicacion);
                    }
                    mReplicacion = replicacion;
                }
                return mReplicacion.Value;
            }
            set { mReplicacion = value; }
        }

        public static bool EscaparComillasDoblesEstatica { get; set; }

        /// <summary>
        /// Lista de tipos para el base.
        /// </summary>
        public static List<string> ListaTiposBase
        {
            get
            {
                if (mListaTiposBase == null)
                {
                    mListaTiposBase = new List<string>
                    {
                        "\"" + FacetadoAD.BUSQUEDA_BLOGS + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_COMUNIDAD_EDUCATIVA + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_COMUNIDAD_NO_EDUCATIVA + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_COMUNIDADES + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_DAFOS + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_DEBATES + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_ORGANIZACION + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_PERSONA + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_PREGUNTAS + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_ENCUESTAS + "\" .",
                        "\"" + FacetadoAD.BUSQUEDA_RECURSOS + "\" ."
                    };
                }

                return mListaTiposBase;
            }
        }

        public List<Guid> ListaComunidadesPrivadasUsuario
        {
            get
            {
                if (mListaComunidadesPrivadasUsuario == null)
                {
                    mListaComunidadesPrivadasUsuario = new List<Guid>();
                }
                return mListaComunidadesPrivadasUsuario;
            }
            set
            {
                mListaComunidadesPrivadasUsuario = value;
            }
        }

        public Dictionary<string, List<string>> InformacionOntologias
        {
            get
            {
                if (mInformacionOntologias == null)
                {
                    mInformacionOntologias = new Dictionary<string, List<string>>();
                }
                return mInformacionOntologias;
            }
            set
            {
                mInformacionOntologias = value;
            }
        }

        public string MandatoryRelacion
        {
            get
            {
                if (mMandatoryRelacion == null)
                {
                    mMandatoryRelacion = "";
                }
                return mMandatoryRelacion;
            }
            set
            {
                mMandatoryRelacion = value;
            }
        }

        public List<string> ListaItemsBusquedaExtra
        {
            get
            {
                if (mListaItemsBusquedaExtra == null)
                {
                    mListaItemsBusquedaExtra = new List<string>();
                }
                return mListaItemsBusquedaExtra;
            }
            set
            {
                mListaItemsBusquedaExtra = value;
            }
        }

        public List<string> PropiedadesRango
        {
            get
            {
                if (mPropiedadesRango == null)
                {
                    mPropiedadesRango = new List<string>();
                }
                return mPropiedadesRango;
            }
            set
            {
                mPropiedadesRango = value;
            }
        }

        public List<string> PropiedadesFecha
        {
            get
            {
                if (mPropiedadesFecha == null)
                {
                    mPropiedadesFecha = new List<string>();
                }
                return mPropiedadesFecha;
            }
            set
            {
                mPropiedadesFecha = value;
            }
        }

        private static Dictionary<string, string> mListaNamespacesBasicos = new Dictionary<string, string>();

        public static Dictionary<string, string> ListaNamespacesBasicos
        {
            get
            {
                if (mListaNamespacesBasicos.Count == 0)
                {
                    lock (mListaNamespacesBasicos)
                    {
                        // Doble comprobación, por si el proceso ha quedado bloqueado esperando a que otro proceso cargue el listado
                        if (mListaNamespacesBasicos.Count == 0)
                        {
                            Dictionary<string, string> namespacesBasicos = new Dictionary<string, string>();
                            namespacesBasicos.Add("gnoss", "http://gnoss/");
                            namespacesBasicos.Add("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                            namespacesBasicos.Add("sioc", "http://rdfs.org/sioc/ns#");
                            namespacesBasicos.Add("sioc_t", "http://rdfs.org/sioc/types#");
                            namespacesBasicos.Add("vcard", "http://www.w3.org/2006/vcard/ns#");
                            namespacesBasicos.Add("oc", "http://d.opencalais.com/1/type/er/Geo/");
                            namespacesBasicos.Add("dc", "http://purl.org/dc/terms/");
                            namespacesBasicos.Add("skos", "http://www.w3.org/2004/02/skos/core#");
                            namespacesBasicos.Add("foaf", "http://xmlns.com/foaf/0.1/");
                            namespacesBasicos.Add("nmo", "http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#");
                            namespacesBasicos.Add("swrc", "http://swrc.ontoware.org/ontology#");
                            namespacesBasicos.Add("dce", "http://purl.org/dc/elements/1.1/");

                            mListaNamespacesBasicos = namespacesBasicos;
                        }
                    }
                }
                return mListaNamespacesBasicos;
            }
        }

        private static StringBuilder mNamespacesBasicos = new StringBuilder(" prefix gnoss: <http://gnoss/> prefix rdf: <http://www.w3.org/1999/02/22-rdf-syntax-ns#> prefix sioc: <http://rdfs.org/sioc/ns#> prefix sioc_t: <http://rdfs.org/sioc/types#> prefix vcard: <http://www.w3.org/2006/vcard/ns#> prefix oc: <http://d.opencalais.com/1/type/er/Geo/> prefix dc: <http://purl.org/dc/terms/> prefix skos: <http://www.w3.org/2004/02/skos/core#> prefix foaf: <http://xmlns.com/foaf/0.1/> prefix nmo: <http://www.semanticdesktop.org/ontologies/2007/03/22/nmo#> prefix swrc: <http://swrc.ontoware.org/ontology#> prefix dce: <http://purl.org/dc/elements/1.1/>");

        /// <summary>
        /// Obtiene los namespaces para realizar consultas en virtuoso
        /// </summary>
        public string NamespacesBasicos
        {
            get
            {
                if (mNamespacesBasicos == null ||  mNamespacesBasicos.Length == 0)
                {
                    foreach (string key in ListaNamespacesBasicos.Keys)
                    {
                        mNamespacesBasicos.Append($"prefix {key}:<{ListaNamespacesBasicos[key]}>");
                    }
                }
                return mNamespacesBasicos.ToString();
            }
        }

        private Dictionary<string, string> mListaTodosNamespaces = new Dictionary<string, string>();

        public Dictionary<string, string> ListaTodosNamespaces
        {
            get
            {
                if (mListaTodosNamespaces.Count == 0)
                {
                    lock (mListaTodosNamespaces)
                    {
                        if (mListaTodosNamespaces.Count == 0)
                        {
                            Dictionary<string, string> listaTodosNamespaces = new Dictionary<string, string>();
                            foreach (KeyValuePair<string, List<string>> ontologia in InformacionOntologias)
                            {
                                foreach (string ns in ontologia.Value)
                                {
                                    if (!listaTodosNamespaces.ContainsKey(ns))
                                    {
                                        if (!ontologia.Key.Contains("@"))
                                        {
                                            listaTodosNamespaces.Add(ns, mUrlIntranet + "Ontologia/" + ontologia.Key + ".owl#");
                                        }
                                        else
                                        {
                                            listaTodosNamespaces.Add(ns, ontologia.Key.Replace("@", ""));
                                        }
                                    }
                                }
                            }

                            foreach (string key in ListaNamespacesBasicos.Keys)
                            {
                                if (!listaTodosNamespaces.ContainsKey(key))
                                {
                                    listaTodosNamespaces.Add(key, ListaNamespacesBasicos[key]);
                                }
                            }

                            mListaTodosNamespaces = listaTodosNamespaces;
                        }
                    }
                }
                return mListaTodosNamespaces;
            }
        }

        /// <summary>
        /// Obtiene los namespaces para realizar consultas en virtuoso
        /// </summary>
        public string NamespacesVirtuosoLectura
        {
            get
            {
                if (string.IsNullOrEmpty(mNamespacesVirtuoso))
                {
                    lock (mBloqueoNamespacesVirtuoso)
                    {
                        if (string.IsNullOrEmpty(mNamespacesVirtuoso))
                        {
                            string namespacesVirtuoso = "SPARQL ";

                            foreach (string clave in ListaTodosNamespaces.Keys)
                            {
                                namespacesVirtuoso += $"prefix {clave}:<{ListaTodosNamespaces[clave]}>";
                            }

                            namespacesVirtuoso += $"prefix cv:<{mUrlIntranet}Ontologia/Curriculum.owl#>";

                            mNamespacesVirtuoso = namespacesVirtuoso;
                        }
                    }
                }

                return mNamespacesVirtuoso;
            }
        }

        /// <summary>
        /// Obtiene los namespaces para realizar escrituras en virtuoso
        /// </summary>
        public string NamespacesVrituosoEscritura
        {
            get
            {
                return UtilCadenas.PasarAUtf8(NamespacesVirtuosoLectura);
            }
        }



        /// <summary>
        /// Verdad si el usuario actual tiene permiso para ver los privados
        /// </summary>
        public bool ObtenerPrivados
        {
            get;
            set;
        }

        public static bool UsarRegexParaBusquedaPorTextoLibre { get; set; } = false;

        public bool UsuarioTieneRecursosPrivados { get; set; }

        public bool ObtenerContadorDeFaceta { get; set; } = true;

        /// <summary>
        /// Obtiene la conexión a virtuoso
        /// </summary>
        private VirtuosoConnection ParentConnection
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.ParentConnection;
            }
        }

        private string FicheroConfiguracionMaster
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.FicheroConfiguracionMaster;
            }
        }

        public bool UsarClienteTradicional
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.UsarClienteTradicional;
            }
            set
            {
                mServicesUtilVirtuosoAndReplication.UsarClienteTradicional = value;
            }
        }


        /// <summary>
        /// Transacción actual
        /// </summary>
        public DbTransaction TransaccionVirtuoso
        {
            get { return mServicesUtilVirtuosoAndReplication.TransaccionVirtuoso; }
        }



        /// <summary>
        /// Obtiene o establece la configuración de servidores para los grafos
        /// </summary>
        public DataWrapperFacetas ServidoresGrafo
        {
            get
            {
                if (mServidoresGrafo == null)
                {
                    try
                    {
                        FacetaAD facetaAD = new FacetaAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<FacetaAD>(),mloggerFactory);
                        mServidoresGrafo = facetaAD.ObtenerConfiguracionGrafoConexion();
                        facetaAD.Dispose();
                    }
                    catch (Exception)
                    {
                        if (mServidoresGrafo == null)
                        {
                            mServidoresGrafo = new DataWrapperFacetas();
                        }
                    }
                }
                return mServidoresGrafo;
            }
            set
            {
                mServidoresGrafo = value;
            }
        }

        /// <summary>
        /// Condición extra para la consulta de facetas.
        /// </summary>
        public string CondicionExtraFacetas
        {
            get
            {
                return mCondicionExtraFacetas;
            }
            set
            {
                mCondicionExtraFacetas = value;
            }
        }

        public Dictionary<string, bool> DiccionarioFacetasExcluyentes
        {
            get
            {
                return mDiccionarioFacetasExcluyentes;
            }
            set
            {
                mDiccionarioFacetasExcluyentes = value;
            }
        }

        public DataWrapperFacetas FacetaDW
        {
            get
            {
                return mFacetaDW;
            }
            set
            {
                mFacetaDW = value;
            }
        }

        private int NumeroReciproca
        {
            get
            {
                return mNumeroReciproca++;
            }
        }

        public static bool EsPeticionFacetas
        {
            get
            {
                return mEsPeticionFacetas;
            }
            set
            {
                mEsPeticionFacetas = value;
            }
        }

        public static bool UsarPrivacidadEnFacetasYResultados
        {
            get
            {
                return mUsarPrivacidadEnFacetasYResultados;
            }
            set
            {
                mUsarPrivacidadEnFacetasYResultados = value;
            }
        }

        public static bool EscaparComillasDoblesPorHilo
        {
            get
            {
                return mEscaparComillasDoblesPorHilo;
            }
            set
            {
                mEscaparComillasDoblesPorHilo = value;
            }
        }

        public static bool EscaparComillasDobles
        {
            get
            {
                return EscaparComillasDoblesEstatica || EscaparComillasDoblesPorHilo;
            }
        }

        public string EvaluarFiltrosFacetasEnOrden
        {
            get
            {
                if (mConfigService.ObtenerEvaluarFiltrosFacetasEnOrden())
                {
                    mEvaluarFiltrosFacetasEnOrden = $"DEFINE sql:select-option \"order\"";
                }

                return mEvaluarFiltrosFacetasEnOrden;
            }
            set
            {
                mEvaluarFiltrosFacetasEnOrden = value;
            }
        }

        #endregion
        public bool UsarMismsaVariablesParaEntidadesEnFacetas
        {
            get;
            set;
        }

        public bool ObtenerSoloConsulta { get; set; } = false;

        private static string MaxExecutionPlanLimit
        {
            get
            {
                if (mMaxExecutionPlanLimit == null)
                {
                    mMaxExecutionPlanLimit = "";
                    string pathFicheroMaxExecutionPlanLimit = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "maxExecutionPlanLimit.config");
                    if (File.Exists(pathFicheroMaxExecutionPlanLimit))
                    {
                        string contenido = File.ReadAllText(pathFicheroMaxExecutionPlanLimit).Trim();
                        if (!string.IsNullOrWhiteSpace(contenido))
                        {
                            mMaxExecutionPlanLimit = $"DEFINE sql:max-allowed-line-count {contenido}";
                        }
                    }
                }
                return mMaxExecutionPlanLimit;
            }
        }

        #region Métodos generales

        #region Métodos de conexión con virtuoso

        /// <summary>
        /// Cierra todas las conexiones abiertas a virtuoso
        /// </summary>
        public void CerrarConexion()
        {
            CerrarConexionGrafo();
        }

        /// <summary>
        /// Cierra una conexión existente a virtuoso que ha sido previamente abierta
        /// </summary>
        /// <param name="pClaveConexion">Clave con la que se ha almacenado la conexión para esta petición</param>
        private void CerrarConexionGrafo()
        {
            try
            {
                VirtuosoConnection conexion = mVirtuosoAD.Connection;

                if (conexion != null && conexion.State != ConnectionState.Closed)
                {
                    conexion.Close();
                    conexion.Dispose();
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
            }
        }

        /// <summary>
        /// Obtiene la conexión para un grafo concreto
        /// </summary>
        /// <param name="pGrafo">Grafo en el que se va a leer o escribir</param>
        /// <param name="pEscritura">Verdad si es una petición de escritura</param>
        /// <returns></returns>
        private VirtuosoConnection ObtenerConexionParaGrafo(string pGrafo, bool pEscritura, string pCadenaConexion = null)
        {
            VirtuosoConnection conexion = null;
            if (!string.IsNullOrEmpty(pCadenaConexion))
            {
                conexion = new VirtuosoConnection(pCadenaConexion);
            }
            else if (!string.IsNullOrEmpty(pGrafo))
            {
                conexion = mVirtuosoAD.Connection;
                if (conexion == null)
                {
                    ConfiguracionConexionGrafo filaConfigGrafo = ServidoresGrafo.ListaConfiguracionConexionGrafo.Where(item => item.Grafo.Equals(pGrafo.ToLower())).FirstOrDefault();
                    if (filaConfigGrafo != null)
                    {
                        conexion = new VirtuosoConnection(filaConfigGrafo.CadenaConexion);
                        mVirtuosoAD.Connection = conexion;
                        //Agrego el grafo a una lista de grafo para luego cerrar las conexiones
                        List<string> listaGrafos = mVirtuosoAD.ListaGrafos;
                        if (listaGrafos == null)
                        {
                            listaGrafos = new List<string>();
                            mVirtuosoAD.ListaGrafos = listaGrafos;
                        }
                        if (!listaGrafos.Contains(pGrafo))
                        {
                            listaGrafos.Add(pGrafo);
                        }
                    }
                }
            }

            if (conexion == null)
            {
                //No tiene un virtuoso propio, devuelvo el común
                if (pEscritura)
                {
                    conexion = mServicesUtilVirtuosoAndReplication.ParentConnectionMaster;
                }
                else
                {
                    conexion = ParentConnection;
                }
            }

            if (conexion != null && !conexion.State.Equals(ConnectionState.Open))
            {
                QuickOpen(conexion);
                mLoggingService.AgregarEntrada("Conexión abierta: " + conexion.ConnectionString);
            }
            return conexion;
        }

        public int ObtenerValorSegundosParametroAplicacion()
        {
            string valor = mEntityContext.ParametroAplicacion.Where(item => item.Parametro.Equals("SegundosConsultaCostosaSparql")).Select(item => item.Valor).FirstOrDefault();
            int segundos = 1;
            if (!string.IsNullOrEmpty(valor))
            {
                int.TryParse(valor, out segundos);
            }
            return segundos;
        }

        /// <summary>
        /// Lee datos desde virtuoso (mediante una consulta SELECT)
        /// </summary>
        /// <param name="pQuery">Consulta a ejecutar (select)</param>
        /// <param name="pNombreTabla">Nombre de la tabla a cargar en el dataset</param>
        /// <returns></returns>
        public FacetadoDS LeerDeVirtuoso(string pQuery, string pNombreTabla, string pGrafo, bool pDesdeApi = false)
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            LeerDeVirtuoso(pQuery, pNombreTabla, facetadoDS, pGrafo, pDesdeApi);

            return facetadoDS;
        }

        /// <summary>
        /// Lee datos desde virtuoso (mediante una consulta SELECT)
        /// </summary>
        /// <param name="pQuery">Consulta a ejecutar (select)</param>
        /// <param name="pNombreTabla">Nombre de la tabla a cargar en el dataset</param>
        /// <param name="pFacetadoDS">DataSet a cargar</param>
        /// <returns></returns>
        public void LeerDeVirtuoso(string pQuery, string pNombreTabla, FacetadoDS pFacetadoDS, string pGrafo, bool pDesdeApi = false)
        {
            LeerDeVirtuoso(pQuery, pNombreTabla, pFacetadoDS, pGrafo, false, pDesdeApi);
        }

        /// <summary>
        /// Lee datos desde virtuoso (mediante una consulta SELECT)
        /// </summary>
        /// <param name="pQuery">Consulta a ejecutar (select)</param>
        /// <param name="pNombreTabla">Nombre de la tabla a cargar en el dataset</param>
        /// <param name="pFacetadoDS">DataSet a cargar</param>
        /// <returns></returns>
        public void LeerDeVirtuoso(string pQuery, string pNombreTabla, FacetadoDS pFacetadoDS, string pGrafo, bool pUsarHilos, bool pDesdeApi = false)
        {
            VirtuosoConnectionData virtuosoConnectionData;
            if (pDesdeApi && mConfigService.CheckBidirectionalReplicationIsActive())
            {
                string afinidad = mServicesUtilVirtuosoAndReplication.ConexionAfinidad;
                virtuosoConnectionData = mConfigService.ObtenerVirutosoEscritura(afinidad);
            }
            else if (FicheroConfiguracionMaster.ToLower().Contains("home"))
            {
                virtuosoConnectionData = mConfigService.ObtenerVirtuosoConnectionStringHome();
            }
            else
            {
                virtuosoConnectionData = mConfigService.ObtenerVirtuosoConnectionString();
            }

            //Quito el inicio de SPARQL
            if (pQuery.Trim().ToUpper().StartsWith("SPARQL"))
            {
                pQuery = pQuery.Trim().Substring(6);
            }
            pQuery = $"{MaxExecutionPlanLimit}{pQuery}";

            if (!string.IsNullOrEmpty(EvaluarFiltrosFacetasEnOrden))
            {
                pQuery = $"{EvaluarFiltrosFacetasEnOrden} {pQuery}";
            }


            NameValueCollection parametros = GenerarParametrosParaQuery(pQuery);

            AgregarEntradaTraza("Leo de virtuoso " + virtuosoConnectionData.Ip + ". " + pQuery);

            try
            {
                LeerDeVirtuoso_WebClient(virtuosoConnectionData, pNombreTabla, pFacetadoDS, pQuery, parametros);
            }
            catch (ExcepcionDeBaseDeDatos)
            {
                throw;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);

                Thread.Sleep(1000);

                if (mServicesUtilVirtuosoAndReplication.ControlarErrorVirtuosoConection())
                {
                    LeerDeVirtuoso(pQuery, pNombreTabla, pFacetadoDS, pGrafo, pUsarHilos, pDesdeApi);
                    mLoggingService.GuardarLogError("Consulta ejecutada correctamente en el segundo intento",mlogger);
                    return;
                }
                else
                {
                    AgregarEntradaTraza("VIRTUOSO ERROR : Se ha superado el tiempo establecido para realizar la consulta a virtuoso.");
                    throw;
                }
            }
            AgregarEntradaTraza("Leído de virtuoso.");
        }

        private int ObtenerNumeroReplicacionesPendienes(string nombreConexion)
        {
            int numReplicacionesPendientes = new ReplicacionAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<ReplicacionAD>(),mloggerFactory).ContarReplicacionesPendientesEnReplica(nombreConexion);

            return numReplicacionesPendientes;
        }

        private NameValueCollection GenerarParametrosParaQuery(string pQuery)
        {
            NameValueCollection parametros = new NameValueCollection();
            parametros.Add("query", pQuery);
            parametros.Add("timeout", TimeOutVirtuoso.ToString());
            parametros.Add("format", "text/csv");

            return parametros;
        }

        /// <summary>
        /// Lee datos desde virtuoso (mediante una consulta SELECT)
        /// </summary>
        /// <param name="pQuery">Consulta a ejecutar (select)</param>
        /// <param name="pNombreTabla">Nombre de la tabla a cargar en el dataset</param>
        /// <returns></returns>
        public FacetadoDS LeerDeVirtuosoPorIP(VirtuosoConnectionData pVirtuosoConnectionData, string pQuery, string pNombreTabla)
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            NameValueCollection parametros = GenerarParametrosParaQuery(pQuery);

            LeerDeVirtuoso_WebClient(pVirtuosoConnectionData, pNombreTabla, facetadoDS, pQuery, parametros);

            return facetadoDS;
        }

        public VirtuosoConnectionData ObtenerCadenaConexionLeerDeVirtuoso(bool pUsarConexionAfinidad)
        {
            VirtuosoConnectionData virtuosoConnectionData;
            if (pUsarConexionAfinidad && mConfigService.CheckBidirectionalReplicationIsActive())
            {
                string afinidad = mServicesUtilVirtuosoAndReplication.ConexionAfinidad;
                virtuosoConnectionData = mConfigService.ObtenerVirutosoEscritura(afinidad);
            }
            else
            {
                if (FicheroConfiguracionMaster.ToLower().Contains("home"))
                {
                    virtuosoConnectionData = mConfigService.ObtenerVirtuosoConnectionStringHome();
                }
                else
                {
                    virtuosoConnectionData = mConfigService.ObtenerVirtuosoConnectionString();
                }
            }
            return virtuosoConnectionData;
        }

        public string LeerDeVirtuosoJSON(string pQuery, string pNombreTabla, string pGrafo, bool pUsarConexionAfinidad = false)
        {
            string JSON = "";

            VirtuosoConnectionData virtuosoConnectionData = ObtenerCadenaConexionLeerDeVirtuoso(pUsarConexionAfinidad);

            //Quito el inicio de SPARQL
            if (pQuery.Trim().ToUpper().StartsWith("SPARQL"))
            {
                pQuery = pQuery.Trim().Substring(6);
            }

            if (!string.IsNullOrEmpty(EvaluarFiltrosFacetasEnOrden))
            {
                pQuery = $"{EvaluarFiltrosFacetasEnOrden} {pQuery}";
            }

            NameValueCollection parametros = new NameValueCollection();
            parametros.Add("query", pQuery);
            parametros.Add("timeout", TimeOutVirtuoso.ToString());
            parametros.Add("format", "application/sparql-results+json");

            AgregarEntradaTraza($"Leo de virtuoso {virtuosoConnectionData.Ip}. \n {pQuery}");

            JSON = LeerDeVirtuoso_WebClientJSON(virtuosoConnectionData, pQuery, parametros);

            AgregarEntradaTraza($"Leído de virtuoso: {virtuosoConnectionData.Ip}. \n{pQuery}");

            return JSON;
        }

        public string LeerDeVirtuosoCSV(string pQuery, string pNombreTabla, string pGrafo, bool pUsarConexionAfinidad = false)
        {
            VirtuosoConnectionData virtuosoConnectionData = ObtenerCadenaConexionLeerDeVirtuoso(pUsarConexionAfinidad);

            //Quito el inicio de SPARQL
            if (pQuery.Trim().ToUpper().StartsWith("SPARQL"))
            {
                pQuery = pQuery.Trim().Substring(6);
            }

            if (!string.IsNullOrEmpty(EvaluarFiltrosFacetasEnOrden))
            {
                pQuery = $"{EvaluarFiltrosFacetasEnOrden} {pQuery}";
            }

            NameValueCollection parametros = null;

            AgregarEntradaTraza($"Leo de virtuoso {virtuosoConnectionData.Ip}. \n {pQuery}");

            string csv = LeerDeVirtuoso_WebClientCSV(virtuosoConnectionData, pNombreTabla, pQuery, parametros);

            AgregarEntradaTraza($"Leído de virtuoso: {virtuosoConnectionData.Ip}. \n{pQuery}");

            return csv;
        }

        public string LeerDeVirtuoso_WebClientCSV(VirtuosoConnectionData pVirtuosoConnectionData, string pNombreTabla, string pQuery, NameValueCollection pParametros = null)
        {
            string responseCSV = "";
            if (pParametros == null)
            {
                pParametros = new NameValueCollection();
                pParametros.Add("query", pQuery);
                pParametros.Add("timeout", TimeOutVirtuoso.ToString());
                pParametros.Add("format", "text/csv");
            }

            AgregarEntradaTraza("LecturaWebClient: Inicio");

            //Creamos un método para que si hay algún error devuelva el error, no un DS vacio.
            //Esto se necesita para el método ServidorOperativo (Base, Base Usuarios, RefrescoCacheMensajes...)

            WebClient webClient = new RiamWebClient(600);
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

            string url = pVirtuosoConnectionData.SparqlEndpoint;

            if (string.IsNullOrEmpty(pVirtuosoConnectionData.ReadUser))
            {
                throw new Exception($"La conexión {pVirtuosoConnectionData.Name} con IP {pVirtuosoConnectionData.Ip} NO es de lectura {pVirtuosoConnectionData.VirtuosoConnectionType} y no puede ejecutar la consulta: {Environment.NewLine}{pQuery}");
            }

            if (!pVirtuosoConnectionData.ReadUser.Equals("dba"))
            {
                url = pVirtuosoConnectionData.AuthSparqlEndpoint;
                var credentialCache = new CredentialCache();
                credentialCache.Add(
                new Uri(url), // request url
                  "Digest", // authentication type
                  new NetworkCredential(pVirtuosoConnectionData.ReadUser, pVirtuosoConnectionData.ReadUserPassword) // credentials
                );

                webClient.Credentials = credentialCache;
            }

            WebException webExceptionAuxiliar = null;

            DateTime horaInicio = DateTime.Now;
            int milisegundos = 0;
            string error = null;

            try
            {
                byte[] responseArray = webClient.UploadValues(url, "POST", pParametros);

                responseCSV = Encoding.UTF8.GetString(responseArray);

                if (mConfigService.ObtenerProcesarStringGrafo())
                {
                    responseCSV = responseCSV.Replace("''", "\"\"");
                }

                return responseCSV;
            }
            catch (WebException webException)
            {
                string respuesta = "";

                try
                {
                    if (webException.Response != null)
                    {
                        //Intento recuperar información del error
                        StreamReader dataStream = new StreamReader(webException.Response.GetResponseStream());
                        respuesta = dataStream.ReadToEnd();
                    }
                }
                catch 
                {
                    //Si falla la lectura no rompemos el proceso, pero guardamos el error.
                }

                respuesta += $"\n\nQuery: {pQuery}";
                respuesta += $"\n\nUrl: {url}";

                mLoggingService.GuardarLogError(webException, respuesta, mlogger);

                if ((webException.Status.Equals(WebExceptionStatus.ProtocolError) && !((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.BadRequest) && !((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.InternalServerError)) || webException.Message.Contains("(503)") || webException.Message.Contains("(404)") || (webException.Response != null && ((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.NotFound)))
                {
                    // Es un error de checkpoint o de que virtuoso se ha caído, si hay más servidores, reintentamos la consulta
                    throw new ExcepcionCheckpointVirtuoso();
                }
                else if (((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.InternalServerError) && respuesta.Contains("SR171: Transaction timed out"))
                {
                    mLoggingService.GuardarLog($"\n\nMessage: {respuesta}\n\nQuery: {pQuery}\n\nUrl: {url}",mlogger, $"{LoggingService.RUTA_DIRECTORIO_ERROR}\\virtuosoTransactionTimedOut_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
                    throw new ExcepcionConectionFailVirtuoso("SR171: Transaction timed out");
                }
                else if (webException.Status.Equals(WebExceptionStatus.ConnectFailure))
                {
                    throw new ExcepcionConectionFailVirtuoso();
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pQuery, respuesta, webException);
                }
            }

        }
        public void LeerDeVirtuoso_WebClient(VirtuosoConnectionData pVirtuosoConnectionData, string pNombreTabla, FacetadoDS pFacetadoDS, string pQuery, NameValueCollection pParametros = null)
        {
            if (pParametros == null)
            {
                pParametros = new NameValueCollection();
                pParametros.Add("query", pQuery);
                pParametros.Add("timeout", TimeOutVirtuoso.ToString());
                pParametros.Add("format", "text/csv");
            }

            AgregarEntradaTraza("LecturaWebClient: Inicio");

            //Creamos un método para que si hay algún error devuelva el error, no un DS vacio.
            //Esto se necesita para el método ServidorOperativo (Base, Base Usuarios, RefrescoCacheMensajes...)

            WebClient webClient = new RiamWebClient(600);
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

            string url = pVirtuosoConnectionData.SparqlEndpoint;

            if (string.IsNullOrEmpty(pVirtuosoConnectionData.ReadUser))
            {
                throw new ExcepcionConectionFailVirtuoso($"La conexión {pVirtuosoConnectionData.Name} con IP {pVirtuosoConnectionData.Ip} NO es de lectura {pVirtuosoConnectionData.VirtuosoConnectionType} y no puede ejecutar la consulta: {Environment.NewLine}{pQuery}");
            }

            if (!pVirtuosoConnectionData.ReadUser.Equals("dba"))
            {
                url = pVirtuosoConnectionData.AuthSparqlEndpoint;
                var credentialCache = new CredentialCache();
                credentialCache.Add(
                new Uri(url), // request url
                  "Digest", // authentication type
                  new NetworkCredential(pVirtuosoConnectionData.ReadUser, pVirtuosoConnectionData.ReadUserPassword) // credentials
                );

                webClient.Credentials = credentialCache;
            }

            WebException webExceptionAuxiliar = null;

            try
            {
                byte[] responseArray = webClient.UploadValues(url, "POST", pParametros);

                string respuesta = Encoding.UTF8.GetString(responseArray);

                AgregarEntradaTraza("LecturaWebClient: Respuesta obtenida de virtuoso, convertimos el CSV");

                if (mConfigService.ObtenerProcesarStringGrafo())
                {
                    respuesta = respuesta.Replace("''", "\"\"");
                }

                lock (pFacetadoDS)
                {
                    LeerResultadosCSV(respuesta, pNombreTabla, pFacetadoDS);
                }

                AgregarEntradaTraza("LecturaWebClient: Fin conversion del CSV");
            }
            catch (WebException webException)
            {
                string respuesta = "";

                try
                {
                    if (webException.Response != null)
                    {
                        //Intento recuperar información del error
                        StreamReader dataStream = new StreamReader(webException.Response.GetResponseStream());
                        respuesta = dataStream.ReadToEnd();
                    }
                }
                catch
                {
                    //Si falla la lectura no rompemos el proceso
                }

                respuesta += $"\n\nQuery: {pQuery}";
                respuesta += $"\n\nUrl: {url}";

                mLoggingService.GuardarLogError(webException, respuesta, mlogger);

                if ((webException.Status.Equals(WebExceptionStatus.ProtocolError) && !((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.BadRequest) && !((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.InternalServerError)) || webException.Message.Contains("(503)") || webException.Message.Contains("(404)") || (webException.Response != null && ((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.NotFound)))
                {
                    // Es un error de checkpoint o de que virtuoso se ha caído, si hay más servidores, reintentamos la consulta
                    throw new ExcepcionCheckpointVirtuoso();
                }
                else if (((HttpWebResponse)webException.Response).StatusCode.Equals(HttpStatusCode.InternalServerError) && respuesta.Contains("SR171: Transaction timed out"))
                {
                    mLoggingService.GuardarLog($"\n\nMessage: {respuesta}\n\nQuery: {pQuery}\n\nUrl: {url}", mlogger, $"{LoggingService.RUTA_DIRECTORIO_ERROR}\\virtuosoTransactionTimedOut_{DateTime.Now.ToString("yyyy-MM-dd")}.txt");
                    throw new ExcepcionConectionFailVirtuoso("SR171: Transaction timed out");
                }
                else if (webException.Status.Equals(WebExceptionStatus.ConnectFailure))
                {
                    throw new ExcepcionConectionFailVirtuoso();
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pQuery, respuesta, webException);
                }
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"\n\nQuery: {pQuery}\n\nUrl: {url}",mlogger);
                throw;
            }
            finally
            {

                try
                {
                    if (webExceptionAuxiliar != null)
                    {
                        webExceptionAuxiliar.Response.Close();
                    }
                }
                catch
                {
                    //Si falla al cerrar el stream no rompemos el proceso
                }

                webClient.Dispose();
            }

            AgregarEntradaTraza("LecturaWebClient: Fin");
        }

        public string LeerDeVirtuoso_WebClientHTML(string pQuery, string pGrafo, string format = "text/html")
        {
            VirtuosoConnectionData virtuosoConnectionData = mConfigService.ObtenerVirtuosoConnectionString();
            if (FicheroConfiguracionMaster.ToLower().Contains("home"))
            {
                virtuosoConnectionData = mConfigService.ObtenerVirtuosoConnectionStringHome();
            }

            NameValueCollection parametros = new NameValueCollection();
            parametros.Add("query", pQuery);
            parametros.Add("timeout", TimeOutVirtuoso.ToString());
            parametros.Add("format", format);
            string respuesta = string.Empty;
            AgregarEntradaTraza("LecturaWebClient: Inicio");
            using (WebClient webClient = new RiamWebClient(600))
            {
                webClient.Encoding = Encoding.UTF8;
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");     

                string url = virtuosoConnectionData.SparqlEndpoint;

                if (string.IsNullOrEmpty(virtuosoConnectionData.ReadUser))
                {
                    throw new ExcepcionConectionFailVirtuoso($"La conexión {virtuosoConnectionData.Name} con IP {virtuosoConnectionData.Ip} NO es de lectura {virtuosoConnectionData.VirtuosoConnectionType} y no puede ejecutar la consulta: {Environment.NewLine}{pQuery}");
                }

                if (!virtuosoConnectionData.ReadUser.Equals("dba"))
                {
                    url = virtuosoConnectionData.AuthSparqlEndpoint;
                    var credentialCache = new CredentialCache();
                    credentialCache.Add(
                    new Uri(url), // request url
                      "Digest", // authentication type
                      new NetworkCredential(virtuosoConnectionData.ReadUser, virtuosoConnectionData.ReadUserPassword) // credentials
                    );

                    webClient.Credentials = credentialCache;
                }

                try
                {
                    byte[] responseArray = webClient.UploadValues(url, "POST", parametros);
                    respuesta = Encoding.UTF8.GetString(responseArray);

                }
                catch (WebException webException)
                {
                    HttpStatusCode status = ((HttpWebResponse)webException.Response).StatusCode;
                    string errorString = string.Empty;
                    
                    try
                    {
                        if (webException.Response != null)
                        {
                            //Intento recuperar información del error
                            StreamReader dataStream = new StreamReader(webException.Response.GetResponseStream());
                            errorString = dataStream.ReadToEnd();
                            webException.Response.Close();
                        }
                    }
                    catch 
                    {
                        //Si falla la lectura no rompemos el proceso
                    }

                    mLoggingService.GuardarLogError(webException, errorString, mlogger);

                    if ((webException.Status.Equals(WebExceptionStatus.ProtocolError) && !status.Equals(HttpStatusCode.BadRequest)) || webException.Message.Contains("(503)") || webException.Message.Contains("(404)") || (webException.Response != null && status.Equals(HttpStatusCode.NotFound)))
                    {
                        // Es un error de checkpoint o de que virtuoso se ha caído, si hay más servidores, reintentamos la consulta
                        throw new ExcepcionCheckpointVirtuoso();
                    }
                    else if (webException.Status.Equals(WebExceptionStatus.ConnectFailure))
                    {
                        throw new ExcepcionConectionFailVirtuoso();
                    }
                    else
                    {
                        throw new ExcepcionDeBaseDeDatos(pQuery, errorString, webException);
                    }
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex, $"\n\nQuery: {pQuery}\n\nUrl: {virtuosoConnectionData.SparqlEndpoint}", mlogger);
                    throw new ExcepcionDeBaseDeDatos(pQuery, ex);
                }
            }
            AgregarEntradaTraza("LecturaWebClient: Fin");
            return respuesta;
        }

        public IDataReader EjecutarDataReader(string pQuery, string pGrafo)
        {
            VirtuosoCommand myCommand = null;
            VirtuosoConnection conexion = null;
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                conexion = ObtenerConexionParaGrafo(pGrafo, true);

                myCommand = new VirtuosoCommand(pQuery, conexion);
                myCommand.CommandTimeout = TimeOutVirtuoso;

                IDataReader reader = myCommand.ExecuteReader();

                return reader;
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia($"Leo de virtuoso. Conexion del grafo: {pGrafo}. Consulta: {pQuery}", false, "Leer de Virtuoso", sw, false);
                throw new ExcepcionDeBaseDeDatos(pQuery, ex);
            }
            finally
            {
                try
                {
                    if (myCommand != null)
                    {
                        myCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }
        }

        public void LeerDeVirtuosoClienteTradicional(string pQuery, string pNombreTablaDS, FacetadoDS pFacetadoDS, string pGrafo)
        {
            VirtuosoCommand myCommand = null;
            VirtuosoDataAdapter myAdapter = null;
            VirtuosoConnection conexion = null;

            Stopwatch sw = LoggingService.IniciarRelojTelemetria();

            try
            {
                conexion = ObtenerConexionParaGrafo(pGrafo, true);

                myCommand = new VirtuosoCommand(pQuery, conexion);
                myAdapter = new VirtuosoDataAdapter(myCommand);
                myCommand.CommandTimeout = TimeOutVirtuoso;

                if (pFacetadoDS != null && pFacetadoDS.Tables.Contains(pNombreTablaDS))
                {
                    pFacetadoDS.Tables[pNombreTablaDS].Clear();
                }

                myAdapter.Fill(pFacetadoDS, pNombreTablaDS);
                mLoggingService.AgregarEntradaDependencia($"Leo de virtuoso. Conexion: {conexion.ConnectionString}. Consulta: {pQuery}", false, "Leer de Virtuoso", sw, true);
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia($"Leo de virtuoso. Conexion: {conexion.ConnectionString}. Consulta: {pQuery}", false, "Leer de Virtuoso", sw, false);
                throw new ExcepcionDeBaseDeDatos(pQuery, ex);
            }
            finally
            {
                try
                {
                    if (myAdapter != null)
                    {
                        myAdapter.Dispose();
                    }

                    if (myCommand != null)
                    {
                        myCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }
        }

        private string LeerDeVirtuoso_WebClientJSON(VirtuosoConnectionData pVirtuosoConnectionData, string pQuery, NameValueCollection pParametros)
        {
            string JSON = "";

            AgregarEntradaTraza("LecturaWebClientJSON: Inicio");

            //Creamos un método para que si hay algún error devuelva el error, no un DS vacio.
            //Esto se necesita para el método ServidorOperativo (Base, Base Usuarios, RefrescoCacheMensajes...)

            WebClient webClient = new RiamWebClient(600);
            webClient.Encoding = Encoding.UTF8;
            webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");

            string url = pVirtuosoConnectionData.SparqlEndpoint;

            if (string.IsNullOrEmpty(pVirtuosoConnectionData.ReadUser))
            {
                throw new ExcepcionConectionFailVirtuoso($"La conexión {pVirtuosoConnectionData.Name} con IP {pVirtuosoConnectionData.Ip} NO es de lectura {pVirtuosoConnectionData.VirtuosoConnectionType} y no puede ejecutar la consulta: {Environment.NewLine}{pQuery}");
            }

            if (!pVirtuosoConnectionData.ReadUser.Equals("dba"))
            {
                url = pVirtuosoConnectionData.AuthSparqlEndpoint;
                var credentialCache = new CredentialCache();
                credentialCache.Add(
                new Uri(url), // request url
                  "Digest", // authentication type
                  new NetworkCredential(pVirtuosoConnectionData.ReadUser, pVirtuosoConnectionData.ReadUserPassword) // credentials
                );

                webClient.Credentials = credentialCache;
            }

            string error = null;

            try
            {
                byte[] responseArray = webClient.UploadValues(url, "POST", pParametros);

                JSON = Encoding.UTF8.GetString(responseArray);

                AgregarEntradaTraza("LecturaWebClientJSON: Respuesta obtenida de virtuoso, en formatoJSON");

            }
            catch (WebException webException)
            {
                string respuesta = "";

                try
                {
                    //Intento recuperar información del error
                    StreamReader dataStream = new StreamReader(webException.Response.GetResponseStream());
                    respuesta = dataStream.ReadToEnd();

                    if (mConfigService.ObtenerProcesarStringGrafo())
                    {
                        respuesta = respuesta.Replace("''", "\\\"");
                    }

                    webException.Response.Close();
                }
                catch 
                {
                    //Si no consigo obtener información del error no se almacena
                }

                respuesta += "\n\nQuery: " + pQuery;
                respuesta += "\n\nUrl: " + url;


                mLoggingService.GuardarLogError(webException, respuesta, mlogger);

                if (webException.Status.Equals(WebExceptionStatus.ConnectFailure) || webException.Status.Equals(WebExceptionStatus.ConnectionClosed) || webException.Message.Contains("(503)"))
                {
                    //Es un error de checkpoint o de que virtuoso se ha caído, si hay más servidores, reintentamos la consulta
                    throw new ExcepcionCheckpointVirtuoso();
                }
                else
                {
                    throw new ExcepcionDeBaseDeDatos(pQuery, webException);
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;

                mLoggingService.GuardarLogError(ex, "\n\nQuery: " + pQuery + "\n\nUrl: " + url, mlogger);
                throw new ExcepcionDeBaseDeDatos(pQuery, ex);
            }
            finally
            {
                webClient.Dispose();
            }

            AgregarEntradaTraza("LecturaWebClientJSON: Fin");

            return JSON;
        }

        public void LeerResultadosCSV(string pResultados, string pNombreTabla, FacetadoDS pFacetadoDS)
        {
            if (pFacetadoDS != null && pFacetadoDS.Tables.Contains(pNombreTabla))
            {
                pFacetadoDS.Tables[pNombreTabla].Clear();
            }
            else if (pFacetadoDS != null && !pFacetadoDS.Tables.Contains(pNombreTabla))
            {
                pFacetadoDS.Tables.Add(new DataTable(pNombreTabla));
            }

            string[] lineas = pResultados.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (pFacetadoDS != null && !string.IsNullOrEmpty(pResultados) && lineas.Length > 1)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(pResultados);
                MemoryStream stream = new MemoryStream(byteArray);
                DataTable csvTable = pFacetadoDS.Tables[pNombreTabla];
                using (CsvReader csvReader = new CsvReader(new StreamReader(stream), true))
                {
                    csvTable.Load(csvReader);
                    foreach (System.Data.DataColumn col in csvTable.Columns)
                    {
                        col.ReadOnly = false;
                    }
                }
            }
            else if (lineas.Length == 1)
            {
                // Creo las columnas en la tabla porque si no hay métodos que fallan
                string[] columnas = lineas[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string columna in columnas)
                {
                    string nombreCol = columna.Trim('\"');

                    if (pFacetadoDS != null && !pFacetadoDS.Tables[pNombreTabla].Columns.Contains(nombreCol))
                    {
                        pFacetadoDS.Tables[pNombreTabla].Columns.Add(nombreCol);
                    }
                }
            }
        }

        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pGrafo">Grafo que se va a actualizar</param>
        public int ActualizarVirtuoso(string pQuery, string pGrafo)
        {
            return ActualizarVirtuoso(pQuery, pGrafo, 0);
        }

        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pGrafo">Grafo que se va a actualizar</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public int ActualizarVirtuoso(string pQuery, string pGrafo, short pPrioridad)
        {
            VirtuosoConnection conexion = ObtenerConexionParaGrafo(pGrafo, true);
            return mServicesUtilVirtuosoAndReplication.ActualizarVirtuoso(pQuery, pGrafo, mConfigService.ObtenerReplicacionActivada(), pPrioridad, conexion, true, null, 0);
        }

        /// <summary>
        /// Actualiza virtuoso (mediante un insert / update / delete)
        /// </summary>
        /// <param name="pQuery">Query a ejecutar (insert / update / delete)</param>
        /// <param name="pGrafo">Grafo que se va a actualizar</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public int ActualizarVirtuoso(string pQuery, string pGrafo, bool pReplicar, short pPrioridad)
        {
            VirtuosoConnection conexion = ObtenerConexionParaGrafo(pGrafo, true);
            return mServicesUtilVirtuosoAndReplication.ActualizarVirtuoso(pQuery, pGrafo, pReplicar, pPrioridad, conexion);
        }

        /// <summary>
        /// Inicia una transacción, si no estaba ya iniciada
        /// </summary>
        /// <returns>True si ha inicado la transacción, false si ya estaba iniciada</returns>
        public override bool IniciarTransaccion(bool pIniciarTransaccionEntity = false)
        {
            return mServicesUtilVirtuosoAndReplication.IniciarTransaccion(pIniciarTransaccionEntity);
        }

        /// <summary>
        /// Terminamos la transaccción
        /// </summary>
        /// <param name="pExito"></param>
        public override void TerminarTransaccion(bool pExito)
        {
            mServicesUtilVirtuosoAndReplication.TerminarTransaccion(pExito);
        }
        /// <summary>
        /// Obtiene o establece la cadena de conexión a la base de datos BASE para insertar en la cola de replicación las transacciones de actualización
        /// </summary>
        public string CadenaConexionBase
        {
            get
            {
                if (!string.IsNullOrEmpty(mServicesUtilVirtuosoAndReplication.FicheroConfiguracion) && mServicesUtilVirtuosoAndReplication.FicheroConfiguracion.Contains("/") && !mCadenaConexionBase.Contains("/"))
                {
                    return mServicesUtilVirtuosoAndReplication.FicheroConfiguracion.Substring(0, mServicesUtilVirtuosoAndReplication.FicheroConfiguracion.LastIndexOf("/") + 1) + mCadenaConexionBase;
                }
                else
                {
                    return mCadenaConexionBase;
                }
            }
            set
            {
                this.mCadenaConexionBase = value;
            }
        }

        public int TimeOutVirtuoso
        {
            get
            {
                return mServicesUtilVirtuosoAndReplication.GetTimeOutVirtuoso();
            }
            set
            {
                mServicesUtilVirtuosoAndReplication.SetTimeOutVirtuoso(value);
            }
        }

        /// <summary>
        /// Realizamos una consulta sencilla al servidor para saber si ya está operativo
        /// </summary>
        /// <returns>TRUE = Servidor Levantado y operativo</returns>
        public bool ServidorOperativo()
        {
            bool servidorOperativo = false;

            try
            {
                //Hacemos una consulta sencilla a virtuoso:
                string query = "SPARQL ASK {?s ?p ?o.}";
                LeerDeVirtuoso(query, "TablaDePrueba", null);
                servidorOperativo = true;
            }
            catch (Exception)
            {
                servidorOperativo = false;
                CerrarConexion();
                //Cerramos las conexiones
                mServicesUtilVirtuosoAndReplication.Connection = null;
                mServicesUtilVirtuosoAndReplication.ConexionMaster = null;
            }

            return servidorOperativo;
        }

        #endregion

        #region Funciones Obtención Facetas

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerContadoresRecursosAgrupadosParaFacetaRangos(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil)
        {
            int numeroAuxiliarVariablesIguales;
            string nombreFacetaSinPrefijo = "";
            string ultimaFacetaAux = "";
            string consultaReciproca = string.Empty;
            ObtenerDatosFiltroReciproco(out consultaReciproca, pClaveFaceta, out pClaveFaceta);

            string from = ObtenerFrom(pProyectoID);

            string where = " WHERE {";
            where += ObtenerWhereQuery(pProyectoID, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pTipoDisenio, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pExcluirPersonas, true, true, 0, pTipoPropiedadesFaceta, TiposAlgoritmoTransformacion.Rangos, out numeroAuxiliarVariablesIguales, out nombreFacetaSinPrefijo, pFiltrosSearchPersonalizados, consultaReciproca, pInmutable, pEsMovil, out ultimaFacetaAux);

            string nombreVariablePrincipal = nombreFacetaSinPrefijo + "2000";
            string nombreVariableSecundario = nombreFacetaSinPrefijo + "3000";
            where += " BIND(xsd:integer(?" + nombreVariablePrincipal + ") as ?" + nombreVariableSecundario + ")";

            where += " } ";

            string select = "SELECT bif:length(str(?" + nombreVariableSecundario + ")) AS ?Indice COUNT(distinct ?s)AS ?Cantidad ";

            string groupByOrderBy = " GROUP BY bif:length(str(?" + nombreVariableSecundario + ")) ";
            groupByOrderBy += " ORDER BY bif:length(str(?" + nombreVariableSecundario + ")) ";

            string query = NamespacesVirtuosoLectura + select + from + where + groupByOrderBy;

            LeerDeVirtuoso(query, pClaveFaceta, pFacetadoDS, pProyectoID, pUsarHilos);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerFacetaMultiple(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, string pConsulta)
        {
            string consultaReciproca = string.Empty;
            ObtenerDatosFiltroReciproco(out consultaReciproca, pClaveFaceta, out pClaveFaceta);

            string from = ObtenerFrom(pProyectoID);

            string consulta = pConsulta.Replace("@@FROM@@", from);

            string where = ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, pPermitirRecursosPrivados);

            where += ObtenerParteFiltros(pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pEsMovil);

            consulta = consulta.Replace("@@FILTERS@@", where);

            string query = NamespacesVirtuosoLectura + consulta;

            LeerDeVirtuoso(query, pClaveFaceta, pFacetadoDS, pProyectoID, pUsarHilos);
        }

        public void ObtenerDatosFiltroReciproco(out string consultaReciproca, string pClaveFacetaOriginal, out string pClaveFacetaModificado)
        {
            consultaReciproca = string.Empty;
            pClaveFacetaModificado = pClaveFacetaOriginal;
            if (pClaveFacetaModificado.Contains("{"))
            {
                consultaReciproca = pClaveFacetaModificado.Substring(pClaveFacetaModificado.IndexOf("{") + 1);

                // Borramos de la clave Faceta el contenido del "{}"
                pClaveFacetaModificado = pClaveFacetaModificado.Remove(pClaveFacetaModificado.IndexOf("{"), consultaReciproca.LastIndexOf("}") + 2);
                if (pClaveFacetaModificado.StartsWith("@@@"))
                {
                    pClaveFacetaModificado = pClaveFacetaModificado.Substring(3);
                }

                consultaReciproca = consultaReciproca.Substring(0, consultaReciproca.LastIndexOf("}"));
            }
        }

        public void ObtenerSubrangosDeCantidad(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, int pNumCifrasCantidad, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil)
        {
            int numeroAuxiliarVariablesIguales;
            string nombreFacetaSinPrefijo = "";
            string consultaExtraFiltro = "";
            string consultaReciproca = string.Empty;

            ObtenerDatosFiltroReciproco(out consultaReciproca, pClaveFaceta, out pClaveFaceta);

            string from = ObtenerFrom(pProyectoID);

            string where = " WHERE {";

            where += ObtenerWhereQuery(pProyectoID, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pTipoDisenio, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pExcluirPersonas, true, true, 0, pTipoPropiedadesFaceta, TiposAlgoritmoTransformacion.Rangos, out numeroAuxiliarVariablesIguales, out nombreFacetaSinPrefijo, pFiltrosSearchPersonalizados, consultaReciproca, pInmutable, pEsMovil, out consultaExtraFiltro);

            string nombreVariablePrincipal = nombreFacetaSinPrefijo + "2000";

            string nombreVariableSecundario = nombreFacetaSinPrefijo + "3000";
            where += " BIND(xsd:integer(?" + nombreVariablePrincipal + ") as ?" + nombreVariableSecundario + ")";

            where += " FILTER(bif:length(str(?" + nombreVariableSecundario + ")) = " + pNumCifrasCantidad;
            where += ") ";

            where += " } ";

            string select = " SELECT substr(str(?" + nombreVariableSecundario + "), 1, 1) AS ?Indice COUNT(DISTINCT ?s) AS ?Cantidad ";

            string groupByOrderBy = " GROUP BY substr(str(?" + nombreVariableSecundario + "), 1, 1) ";
            groupByOrderBy += " ORDER BY substr(str(?" + nombreVariableSecundario + "), 1, 1) ";

            string query = NamespacesVirtuosoLectura + select + from + where + groupByOrderBy;

            LeerDeVirtuoso(query, pClaveFaceta, pFacetadoDS, pProyectoID, pUsarHilos);
        }

        private static void ObtenerExtraConsultaFiltro(ref string pFiltroContextoWhere, out string pFiltroExtraCons, out string pConsultaExtraFiltro, out string pIdioma)
        {
            pFiltroExtraCons = null;
            pConsultaExtraFiltro = null;
            pIdioma = null;

            if (pFiltroContextoWhere != null)
            {
                if (pFiltroContextoWhere.Contains("AgreAFiltro="))
                {
                    string extraConsultaFiltro = pFiltroContextoWhere.Substring(pFiltroContextoWhere.IndexOf("AgreAFiltro="));
                    extraConsultaFiltro = extraConsultaFiltro.Substring(0, extraConsultaFiltro.IndexOf("|") + 1);
                    pFiltroContextoWhere = pFiltroContextoWhere.Replace(extraConsultaFiltro, "");
                    pFiltroExtraCons = extraConsultaFiltro.Substring(extraConsultaFiltro.IndexOf("=") + 1);
                    pConsultaExtraFiltro = pFiltroExtraCons.Substring(pFiltroExtraCons.IndexOf(",") + 1);
                    pFiltroExtraCons = pFiltroExtraCons.Substring(0, pFiltroExtraCons.IndexOf(","));
                    pConsultaExtraFiltro = pConsultaExtraFiltro.Substring(0, pConsultaExtraFiltro.IndexOf("|"));
                }
                else if (pFiltroContextoWhere.Contains("idioma="))
                {
                    pIdioma = pFiltroContextoWhere.Substring(pFiltroContextoWhere.IndexOf("idioma="));
                    pIdioma = pIdioma.Substring(0, pIdioma.IndexOf("|") + 1);
                    pFiltroContextoWhere = pFiltroContextoWhere.Replace(pIdioma, "");
                    pIdioma = pIdioma.Substring(pIdioma.IndexOf("=") + 1);
                    pIdioma = pIdioma.Substring(0, pIdioma.IndexOf("|"));
                }
            }
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, bool pExcluyente, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos = null, Guid pPestanyaID = new Guid())
        {
            ObtenerFaceta(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, TipoProyecto.Catalogo, pExcluyente, pInmutable, pEsMovil, pPestanyaID, pListaExcluidos);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluyente, bool pInmutable, bool pEsMovil, Guid pPestanyaID = new Guid(), List<Guid> pListaExcluidos = null)
        {
            ObtenerFaceta(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, false, null, pExcluyente, pInmutable, pEsMovil, pListaExcluidos, pPestanyaID);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos = null, Guid pPestanyaID = new Guid())
        {
            ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, false, pInmutable, pEsMovil, pPestanyaID, pListaExcluidos);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pInmutable, bool pEsMovil, Guid pPestanyaID = new Guid(), List<Guid> pListaExcluidos = null)
        {
            ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pUsarHilos, false, pInmutable, pEsMovil, pPestanyaID, pListaExcluidos);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pInmutable, bool pEsMovil, Guid pPestanyaID = new Guid(), List<Guid> pListaExcluidos = null)
        {
            ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pUsarHilos, pExcluirPersonas, true, true, 0, TipoPropiedadFaceta.NULL, null, pInmutable, pEsMovil, pListaExcluidos, pPestanyaID);
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos = null, Guid pPestanyaID = new Guid())
        {
            int numeroAuxiliarVariablesIguales = 2;
            string nombreFacetaSinPrefijo = "";
            string ultimaFacetaAux = "";
            string consultaReciproca = string.Empty;
            if (pListaFiltros.ContainsKey("autocompletar"))
            {
                ultimaFacetaAux = "autocompletar";
            }
            ObtenerDatosFiltroReciproco(out consultaReciproca, pClaveFaceta, out pClaveFaceta);

            string query = ObtenerConsultaDeFaceta(numeroAuxiliarVariablesIguales, nombreFacetaSinPrefijo, consultaReciproca, ultimaFacetaAux, pProyectoID, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados, pInmutable, pEsMovil, pListaExcluidos, pPestanyaID);

            if (ObtenerSoloConsulta)
            {
                // Guardar la consulta en el dataset
                if (!pFacetadoDS.Tables.Contains(pClaveFaceta))
                {
                    pFacetadoDS.Tables.Add(pClaveFaceta);
                    pFacetadoDS.Tables[pClaveFaceta].Columns.Add("Consulta");
                    pFacetadoDS.Tables[pClaveFaceta].Columns.Add("Numero");
                }
                if (query.Trim().ToUpper().StartsWith("SPARQL")) // Eliminar el SPARQL inicial
                {
                    query = query.Trim().Substring(6);
                }
                pFacetadoDS.Tables[pClaveFaceta].Rows.Add(query, 0);
            }
            else
            {
                //Paginamos la consulta
                int iterator = 0;
                do
                {
                    string queryAux = query;
                    //Si no se ha definido ni limite ni offset previamente en la consulta, se establece un limit y offset por defecto para paginar
                    if (pLimite == 0 && pInicio == 0)
                    {
                        queryAux = GetPaginatedQueryFromQuery(query, iterator, pProyectoID);
                    }

                    LeerDeVirtuoso(queryAux, pClaveFaceta, pFacetadoDS, pProyectoID, pUsarHilos);
                    iterator++;
                }
                while (pFacetadoDS.Tables[pClaveFaceta].Rows.Count > 0 && pFacetadoDS.Tables[pClaveFaceta].Rows.Count == 10000 * iterator);
            }
        }

        public string ObtenerConsultaDeFaceta(int pNumeroAuxiliarVariablesIguales, string pNombreFacetaSinPrefijo, string pConsultaReciproca, string pUltimaFacetaAux,
            string pProyectoID, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos = null, Guid pPestanyaID = new Guid())
        {
            string from = ObtenerFrom(pProyectoID);

            string where = " WHERE { ";
            where += ObtenerWhereQuery(pProyectoID, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pTipoDisenio, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluyente, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, TiposAlgoritmoTransformacion.Ninguno, out pNumeroAuxiliarVariablesIguales, out pNombreFacetaSinPrefijo, pFiltrosSearchPersonalizados, pConsultaReciproca, pInmutable, pEsMovil, out pUltimaFacetaAux, pListaExcluidos, pPestanyaID);
            where += " } ";

            string orderGroupBy = ObtenerGroupByOrderByYLimites(pClaveFaceta, pListaFiltros, pEstaEnMyGnoss, pTipoDisenio, pInicio, pLimite, pEsRango, pListaRangos, pTipoPropiedadesFaceta, 2000, pNombreFacetaSinPrefijo, pUltimaFacetaAux);
            string select = ObtenerSelectQueryObtenerFaceta(pEsRango, pNombreFacetaSinPrefijo, 2000, pListaRangos, pTipoPropiedadesFaceta, pTipoDisenio, pListaFiltros, pEstaEnMyGnoss, pClaveFaceta, pUltimaFacetaAux);
            string query = NamespacesVirtuosoLectura + select + from + where + orderGroupBy;

            return query;
        }

        /// <summary>
        /// Obtiene una faceta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerTituloFacetas(string pProyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, List<int> pListaRangos, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, Dictionary<string, int> pListaFacetas, Dictionary<string, string> pListaFacetasExtraContexto)
        {
            string from = ObtenerFrom(pProyectoID);

            StringBuilder where = new StringBuilder(" {");
            
            string condicionesWhere = ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, pExcluirPersonas, pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion.Ninguno, pFiltrosSearchPersonalizados, false);

            where.Append(condicionesWhere);

            where.Append(" } ");

            string union = "";

            int indice = 1;
            Dictionary<string, List<string>> filtrosUsadosFacetaActual = new Dictionary<string, List<string>>();
            List<string> listaFiltrosUsados = new List<string>();

            foreach (string faceta in pListaFacetas.Keys)
            {
                where.Append($"{union} {{");

                int reciproca = pListaFacetas[faceta];

                string[] niveles = faceta.Split(new string[1] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

                if (niveles.Length == 1)
                {
                    where.Append($"?s {faceta} ?{QuitaPrefijo(faceta)}. ");
                }
                else
                {
                    string ultimaFacetaAux = "";
                    where.Append(ObtenerParteReciprocaQuery(niveles, reciproca, where.ToString(), ref ultimaFacetaAux, filtrosUsadosFacetaActual, listaFiltrosUsados));
                }

                if (pListaFacetasExtraContexto.ContainsKey(faceta) && !string.IsNullOrEmpty(pListaFacetasExtraContexto[faceta]))
                {
                    string infoExtraContexto = pListaFacetasExtraContexto[faceta];


                    string filtroExtraCons = null;
                    string consultaExtraFiltro = null;
                    string idioma = null;

                    ObtenerExtraConsultaFiltro(ref infoExtraContexto, out filtroExtraCons, out consultaExtraFiltro, out idioma);

                    if (faceta == filtroExtraCons || !string.IsNullOrEmpty(idioma))
                    {
                        where.Append(ObtenerFiltroExtraFaceta(faceta, where.ToString(), filtroExtraCons, consultaExtraFiltro, idioma));
                    }
                }

                where.Append($"bind('{faceta}' as ?propiedad). ");
                where.Append($"bind('{indice++}' as ?orden). ");
                where.Append("}");
                union = " UNION ";
            }

            where = new StringBuilder($"WHERE {{ {where} }} ");

            string query = NamespacesVirtuosoLectura + " SELECT distinct ?propiedad ?orden " + from + where + " order by ?orden";

            LeerDeVirtuoso(query, "Facetas", pFacetadoDS, pProyectoID, pUsarHilos);
        }


        private string ObtenerGroupByOrderByYLimites(string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, bool pEstaEnMyGnoss, TipoDisenio pTipoDisenio, int pInicio, int pLimite, bool pEsRango, List<int> pListaRangos, TipoPropiedadFaceta pTipoPropiedadesFaceta, int pNumeroAuxiliarVariablesIguales, string pNombreFacetaSinPrefijo, string pConsultaExtraFiltro)
        {
            string query = "";

            string nombreVariable = pNombreFacetaSinPrefijo + pNumeroAuxiliarVariablesIguales;
            if (!string.IsNullOrEmpty(pConsultaExtraFiltro))
            {
                nombreVariable = pConsultaExtraFiltro;
            }

            if (!(pEsRango && pListaRangos != null && pListaRangos.Count > 0))
            {
                #region Orden
                if (pEstaEnMyGnoss && (!pListaFiltros.ContainsKey("rdf:type") || !pListaFiltros["rdf:type"].Contains("Comunidad")) && pListaFiltros.ContainsKey("autocompletar") && pClaveFaceta.Contains("Tag") || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMinMax))
                {
                    //query += "}  "; //order by  (?" + facetaSinPrefijo + "2)
                }
                else
                {
                    if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses))
                    {
                        query += $" group by  ?{nombreVariable}fecha ";
                    }
                    else if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo))
                    {
                        query += " group by ?a ?positive } group by ?a ?positive ";
                    }
                    else
                    {
                        query += $" group by ?{nombreVariable} ";
                    }

                    // A la faceta de categorías no le hace falta orden, porque se ordena según el tesauro de la comunidad
                    if (!pClaveFaceta.Equals("skos:ConceptID"))
                    {
                        switch (pTipoDisenio)
                        {
                            case TipoDisenio.ListaMayorAMenor:
                                if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha))
                                {
                                    query += $"order by  desc(?{nombreVariable}fecha) ";
                                }
                                else if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses))
                                {
                                    query += $"order by  asc(?{nombreVariable}fecha) ";
                                }
                                else
                                {
                                    query += $"order by desc (?{nombreVariable}) {(ObtenerContadorDeFaceta ? "desc (?a)" : "")} ";
                                }
                                break;
                            case TipoDisenio.ListaOrdCantidad:
                                query += $"order by desc (?a) asc (bif:rdf_collation_order_string('DB.DBA.LEXICAL_ACUTE',lcase(xsd:string(?{nombreVariable}))))";
                                break;
                            default:
                                if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses))
                                {
                                    query += $"order by  desc(?{nombreVariable}fecha) ";
                                }
                                else if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo))
                                {
                                    query += "order by ?positive ?a";
                                }
                                else
                                {
                                    query += $"order by  bif:rdf_collation_order_string('DB.DBA.LEXICAL_ACUTE',lcase(xsd:string(?{nombreVariable}))) desc (?a) ";
                                }
                                break;
                        }
                    }
                }
                #endregion

                if (pLimite > 0 && !pEsRango)
                {
                    query += " LIMIT " + pLimite;
                }

                if (pInicio != 0 && !pEsRango)
                {
                    query += " OFFSET " + pInicio;
                }
            }

            return query;
        }

        private string ObtenerWhereQuery(string pProyectoID, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, string pIdentidadID, TipoDisenio pTipoDisenio, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pExcluirPersonas, bool pObtenerRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, out int pNumeroAuxiliarVariablesIguales, out string nombreFacetaSinPrefijo, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, string pConsultaReciproca, bool pInmutable, bool pEsMovil, out string pUltimaFacetaAux, List<Guid> pListaExcluidos = null, Guid pPestanyaID = new Guid())
        {
            #region Extra consulta filtro

            string filtroExtraCons = null;
            string consultaExtraFiltro = null;
            string idioma = null;

            ObtenerExtraConsultaFiltro(ref pFiltroContextoWhere, out filtroExtraCons, out consultaExtraFiltro, out idioma);

            #endregion

            pNumeroAuxiliarVariablesIguales = 2;
            nombreFacetaSinPrefijo = QuitaPrefijo(pClaveFaceta);
            string[] LNiveles = null;
            string[] stringSeparators = new string[] { "@@@" };
            if (pClaveFaceta.Contains("@@@"))
            {
                LNiveles = pClaveFaceta.Split(stringSeparators, StringSplitOptions.None);

                nombreFacetaSinPrefijo = QuitaPrefijo(LNiveles[LNiveles.Length - 1]);
            }

            string query = "";

            if (pEsRango && (pListaRangos == null || pListaRangos.Count == 0) && (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses)))
            {
                query += ObtenerCondicionFacetaParaSelect(pClaveFaceta, pListaFiltros, pReciproca, LNiveles, nombreFacetaSinPrefijo, pExcluyente, pEsRango, pListaRangos, pTipoPropiedadesFaceta, pEstaEnMyGnoss, pProyectoID, pTipoDisenio, idioma, ref pNumeroAuxiliarVariablesIguales, consultaExtraFiltro, filtroExtraCons, query);
            }

            bool busquedaMensajesOComent = (pListaFiltros.ContainsKey("rdf:type") && (pListaFiltros["rdf:type"].Count > 0 && (pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_MENSAJES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_COMENTARIOS || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_INVITACIONES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_SUSCRIPCIONES)));

            if (!busquedaMensajesOComent && !pProyectoID.Contains("contactos"))
            {
                query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, pObtenerRecursosPrivados);
            }

            bool esRecomendacion = false;

            if (pProyectoID.Contains("contactos"))
            {
                if (!pListaFiltros.ContainsKey("gnoss:RecPer"))
                {
                    query += "?s <http://xmlns.com/foaf/0.1/knows> <http://gnoss/" + pIdentidadID.ToUpper() + ">. ";
                }
                else
                {
                    query += "<http://gnoss/" + pIdentidadID.ToUpper() + "> <http://gnoss/RecPer>  ?s12. ?s12 <http://gnoss/RecID> ?s. ?s12 <http://gnoss/RecNum> ?s2n. FILTER (?s!=<http://gnoss/" + pIdentidadID.ToUpper() + ">)  MINUS {<http://gnoss/" + pIdentidadID.ToUpper() + "> <http://gnoss/Ignora> ?s.} ";
                    pListaFiltros.Remove("gnoss:RecPer");
                    esRecomendacion = true;
                }
            }

            query += ObtenerParteFiltros(pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, pExcluirPersonas, pOmitirPalabrasNoRelevantesSearch, pTipoAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pInmutable, out pUltimaFacetaAux, pEsMovil, pPestanyaID);

            if (esRecomendacion)
            {
                pListaFiltros.Add("gnoss:RecPer", new List<string>());
                pListaFiltros["gnoss:RecPer"].Add(pIdentidadID.ToString().ToUpper());
            }

            if (!pEsRango || (pListaRangos != null && pListaRangos.Count > 0) || (!pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) && !pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses)))
            {
                query += ObtenerCondicionFacetaParaSelect(pClaveFaceta, pListaFiltros, pReciproca, LNiveles, nombreFacetaSinPrefijo, pExcluyente, pEsRango, pListaRangos, pTipoPropiedadesFaceta, pEstaEnMyGnoss, pProyectoID, pTipoDisenio, idioma, ref pNumeroAuxiliarVariablesIguales, consultaExtraFiltro, filtroExtraCons, query);
            }

            if (!string.IsNullOrEmpty(pConsultaReciproca))
            {
                query += pConsultaReciproca;
            }

            if (pListaExcluidos != null && pListaExcluidos.Count > 0)
            {
                query += ObtenerFiltroRecursosExcluidos(pListaExcluidos);
            }

            return query;
        }

        private string ObtenerCondicionFacetaParaSelect(string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, int pReciproca, string[] pLNiveles, string pNombreFacetaSinPrefijo, bool pExcluyente, bool pEsRango, List<int> pListaRangos, TipoPropiedadFaceta pTipoPropiedadesFaceta, bool pEstaEnMyGnoss, string pProyectoID, TipoDisenio pTipoDisenio, string idioma, ref int pNumeroAuxiliarVariablesIguales, string consultaExtraFiltro, string filtroExtraCons, string pQuery)
        {
            StringBuilder query = new StringBuilder();
            string ultimaVariable = $"?{pNombreFacetaSinPrefijo}2000";

            if (!string.IsNullOrEmpty(pClaveFaceta) && pClaveFaceta.Substring(0, 2).Equals("cv"))
            {
                query.Append("?s gnoss:hasCv ?CV . ");
                query.Append($"?CV {pClaveFaceta} ?{pNombreFacetaSinPrefijo}2000. ");
            }
            else
            {
                //El siguiente código solo debe hacerlo si no es autocompletar porque de lo contrario traerá datos que sobran, además de hacer que la consultas de tesauros semánticos vayan horriblemente lentas.
                //Ej: Si al autocompletar entrase por este if, y estamos buscando los valores de la faceta 'coche' que empicen por 'm', en vez de traer los coches que empiezan por 'm', traería todos los coches de los sujetos que tienen algún coche que empieza por 'm'.
                if (!pListaFiltros.ContainsKey("autocompletar"))
                {
                    if (!string.IsNullOrEmpty(pClaveFaceta) && pClaveFaceta.Contains("@@@"))
                    {
                        StringBuilder nombreObjeto = new StringBuilder(QuitaPrefijo(pLNiveles[0]));

                        if (FacetaAuxEsCoordenada(pLNiveles[0]))
                        {
                            // Si son coordenadas y comparten el mismo inicio, añadir el texto '_Coord' al final del filtro
                            nombreObjeto.Append("_Coord");
                        }

                        //Facetas excluyentes.
                        //Excluyente: http://pruebas.gnoss.net/comunidad/aegp2/empresas#company:product@@@company:productSubgroup=hombre
                        //NO Excluyente: http://comunidades.gnoss.net/comunidad/publicacionesDeusto3/Publications#dc:creator@@@publication:autorID@@@foaf:name=diego%20l%C3%B3pez%20de%20ipi%C3%B1a%20gonz%C3%A1lez%20de%20artaza
                        FacetaAD facAD = new FacetaAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<FacetaAD>(), mloggerFactory);
                        string sujetoReciproco = $"?reciproca{NumeroReciproca}";

                        if (pExcluyente)
                        {
                            //Las facetas excluidas no concatenan grafos: 
                            //?asociación product ?producto. ?producto nombreProducto ? nombreProducto

                            //Por defecto se añade un 0, por la limpieza en el código de variables de método "ObtenerParteFiltros" y así se mantiene la exclusividad de las variables.
                            int i = 0;

                            while (query.ToString().Contains(nombreObjeto.ToString() + (i + 1)))
                            {
                                i++;
                            }

                            nombreObjeto.Append(i);
                            
                            //Solamente añadimos 0 en el caso de que la faceta sea excluyente y de tipo TesauroSemántico. Actualmente no lo es =S
                            while (query.ToString().Contains(nombreObjeto.ToString()) && !string.IsNullOrEmpty(filtroExtraCons) && !string.IsNullOrEmpty(consultaExtraFiltro))
                            {
                                //Se da el caso de que puede haber variables declaradas con un solo 0 y entrar en conflicto en la query.
                                nombreObjeto.Append("0");
                            }

                            if (pReciproca == 1)
                            {
                                query.Append($" {sujetoReciproco} {pLNiveles[0]} ?s. ");
                            }
                            else if (pReciproca > 0)
                            {
                                query.Append($" {sujetoReciproco} {pLNiveles[0]} ?{nombreObjeto}. ");
                            }
                            else
                            {
                                query.Append($" ?s {pLNiveles[0]} ?{nombreObjeto}. ");
                            }
                        }
                        else
                        {
                            //Las facetas no excluidas siguen concatenan grafos:
                            //?publicacion autor ?autor. ?autor2 nombre ?nombre

                            if (!QuitaPrefijo(MandatoryRelacion).Contains(nombreObjeto.ToString()))
                            {
                                if (!UsarMismsaVariablesParaEntidadesEnFacetas)
                                {
                                    nombreObjeto.Append("_2");
                                }
                                else
                                {
                                    nombreObjeto.Append("0");
                                }
                            }
                            else
                            {
                                nombreObjeto.Append(MANDATORY);
                            }

                            if (pReciproca == 1)
                            {
                                query.Append($" {sujetoReciproco} {pLNiveles[0]} ?s. ");
                            }
                            else if (pReciproca > 0)
                            {
                                query.Append($" {sujetoReciproco} {pLNiveles[0]} ?{nombreObjeto}. ");
                            }
                            else if (!pQuery.Contains($" ?s {pLNiveles[0]} ?{nombreObjeto}. "))
                            {
                                query.Append($" ?s {pLNiveles[0]} ?{nombreObjeto}. ");
                            }
                        }
                        facAD.Dispose();

                        string cadenaNombreObjeto = nombreObjeto.ToString();

                        for (int i = 0; i < pLNiveles.Length - 1; i++)
                        {
                            string nombreObjetoAnterior = cadenaNombreObjeto;

                            int numVariablesIguales = 0;

                            if (i.Equals(pLNiveles.Length - 2))
                            {
                                numVariablesIguales = 2000;
                            }
                            else if (UsarMismsaVariablesParaEntidadesEnFacetas)
                            {
                                numVariablesIguales = 0;
                            }
                            else
                            {
                                while (query.ToString().Contains($" ?{QuitaPrefijo(pLNiveles[i + 1])}{pNumeroAuxiliarVariablesIguales}. "))
                                {
                                    pNumeroAuxiliarVariablesIguales++;
                                }
                                numVariablesIguales = pNumeroAuxiliarVariablesIguales;
                            }

                            cadenaNombreObjeto = QuitaPrefijo(pLNiveles[i + 1]) + numVariablesIguales;

                            if (i == pReciproca - 2)
                            {
                                query.Append($" ?{nombreObjetoAnterior} {pLNiveles[i + 1]} ?s . ");
                                ultimaVariable = $"?{nombreObjetoAnterior}";
                            }
                            else if (i == pReciproca - 1)
                            {
                                query.Append($" {sujetoReciproco} {pLNiveles[i + 1]} ?{cadenaNombreObjeto}. ");
                            }
                            else
                            {
                                string condicion = $" ?{nombreObjetoAnterior} {pLNiveles[i + 1]} ?{cadenaNombreObjeto}. ";
                                if (!pQuery.Contains(condicion))
                                {
                                    query.Append(condicion);
                                }
                                ultimaVariable = $"?{cadenaNombreObjeto}";
                            }
                        }
                    }
                    else
                    {

                        query.Append($"?s {pClaveFaceta} ?{pNombreFacetaSinPrefijo}2000. ");
                    }
                }

                if (pClaveFaceta == filtroExtraCons || !string.IsNullOrEmpty(idioma))
                {
                    query.Append(ObtenerFiltroExtraFaceta(pClaveFaceta, query.ToString(), filtroExtraCons, consultaExtraFiltro, idioma));
                }
            }

            if (!string.IsNullOrEmpty(CondicionExtraFacetas))
            {
                query.Append(CondicionExtraFacetas);
            }

            if (pProyectoID.Contains("contribuciones/") && !string.IsNullOrEmpty(pClaveFaceta) && pClaveFaceta.Equals("skos:ConceptID") && !pEstaEnMyGnoss)
            {
                query.Append($"?{pNombreFacetaSinPrefijo}2000 sioc:has_space {pListaFiltros["sioc:has_space"][0].ToString()}. ");
            }

            if (pEsRango && pListaRangos != null && pListaRangos.Count > 0)
            {
                string nombreVariable = $"{pNombreFacetaSinPrefijo}2000";
                string rangoMayor = pListaRangos[0].ToString();
                string filtroRangoMenor = $"AND ?{nombreVariable} >= ";

                string rangoMenor = (pListaRangos[0] - 1).ToString();

                if (pListaRangos.Count > 1)
                {
                    //cojo el rango menor
                    if (pListaRangos[1] > pListaRangos[0])
                    {
                        rangoMayor = pListaRangos[1].ToString();
                        rangoMenor = pListaRangos[0].ToString();
                    }
                    else
                    {
                        rangoMenor = pListaRangos[1].ToString();
                    }
                }

                rangoMayor = ObtenerComplementoRango(pTipoPropiedadesFaceta, rangoMayor, pListaFiltros.ContainsKey(pClaveFaceta));
                
                if (!string.IsNullOrEmpty(rangoMenor))
                {
                    rangoMenor = ObtenerComplementoRango(pTipoPropiedadesFaceta, rangoMenor, pListaFiltros.ContainsKey(pClaveFaceta));
                }

                if ((pTipoDisenio.Equals(TipoDisenio.Calendario) || pTipoDisenio.Equals(TipoDisenio.CalendarioConRangos)) && pListaRangos.Count == 1)
                {
                    //En caso de ser un calendario y recibir solo 1 parametro, componemos el filtro poniendolo como el mayor
                    query.Append($" FILTER({filtroRangoMenor.Replace("AND", "")}{rangoMenor}) ");
                }
                else if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo))
                {
                    if (rangoMayor.StartsWith('-'))
                    {
                        query.Append($" FILTER(?{pNombreFacetaSinPrefijo}2000 <= {rangoMayor} AND ?{nombreVariable} > {rangoMenor}) ");
                    }
                    else
                    {
                        query.Append($" FILTER(?{pNombreFacetaSinPrefijo}2000 < {rangoMayor} AND ?{nombreVariable} >= {rangoMenor}) ");
                    }
                }
                else
                {
                    query.Append($" FILTER(?{pNombreFacetaSinPrefijo}2000 <= {rangoMayor} {filtroRangoMenor}{rangoMenor}) ");
                }
            }
            else if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMinMax))
            {
                query.Append($"BIND(xsd:int(xsd:integer({ultimaVariable}) / 10000000000) AS {ultimaVariable}fecha)");
            }
            else if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses))
            {
                query.Append($"BIND(xsd:int(xsd:integer({ultimaVariable}) / 100000000) AS {ultimaVariable}fecha)");
            }
            else if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo) && !pListaFiltros.ContainsKey(pClaveFaceta))
            {
                query.Append($"BIND(if(xsd:integer({ultimaVariable}) > 0,-10000000000,10000000000) AS ?anno)");
                query.Append($"BIND(abs(xsd:integer({ultimaVariable}) + ?anno) / 1000000000000 AS ?a) .");
                query.Append($"BIND(xsd:integer({ultimaVariable}) > 0 as ?positive)");
            }
            else if (pEsRango && pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo))
            {
                query.Append($"BIND(if(xsd:integer({ultimaVariable}) > 0,-10000000000,10000000000) AS ?anno)");
                query.Append($"BIND(abs(xsd:integer({ultimaVariable}) + ?anno) / 100000000000 AS ?a) .");
                query.Append($"BIND(xsd:integer({ultimaVariable}) > 0 as ?positive)");
            }

            return query.ToString();
        }

        /// <summary>
        /// Obtiene el filtro extra de una faceta
        /// </summary>
        /// <param name="pClaveFaceta">Clave de la faceta</param>
        /// <param name="pQuery">query en construcción</param>
        /// <param name="pFiltroExtraCons">faceta a la que se aplica el filtro extra</param>
        /// <param name="pConsultaExtraFiltro">consulta extra</param>
        /// <param name="pIdioma">idioma</param>
        /// <returns></returns>
        private static string ObtenerFiltroExtraFaceta(string pClaveFaceta, string pQuery, string pFiltroExtraCons, string pConsultaExtraFiltro, string pIdioma)
        {
            pQuery = pQuery.Trim();

            string filtroExtraFaceta = "";

            string ultimoObjeto = "";

            while (pQuery.Contains(" ") && (ultimoObjeto.Length < 2 || ultimoObjeto[0] != '?' || ultimoObjeto[ultimoObjeto.Length - 1] != '.'))
            {
                ultimoObjeto = pQuery.Substring(pQuery.LastIndexOf(" ") + 1);
                pQuery = pQuery.Substring(0, pQuery.LastIndexOf(" "));
            }

            if (ultimoObjeto[ultimoObjeto.Length - 1] == '.')
            {
                ultimoObjeto = ultimoObjeto.Substring(0, ultimoObjeto.Length - 1);
            }

            while (pQuery[pQuery.Length - 1] == ' ')
            {
                pQuery = pQuery.Substring(0, pQuery.Length - 1);
            }

            string ultimoPedicado = pQuery.Substring(pQuery.LastIndexOf(" ") + 1);
            pQuery = pQuery.Substring(0, pQuery.LastIndexOf(" "));

            while (pQuery[pQuery.Length - 1] == ' ')
            {
                pQuery = pQuery.Substring(0, pQuery.Length - 1);
            }

            string ultimoSujeto = pQuery.Substring(pQuery.LastIndexOf(" ") + 1);

            if (pClaveFaceta == pFiltroExtraCons)
            {
                if (pConsultaExtraFiltro.Contains("@o@") || pConsultaExtraFiltro.Contains("@p@") || pConsultaExtraFiltro.Contains("@s@"))
                {
                    filtroExtraFaceta += $" {pConsultaExtraFiltro.Replace("@o@", ultimoObjeto).Replace("@p@", ultimoPedicado).Replace("@s@", ultimoSujeto)} ";
                }
                else
                {
                    filtroExtraFaceta += $" {pConsultaExtraFiltro} {ultimoObjeto}. }}";
                }
            }
            else if (!ultimoObjeto.Equals("?rdftype"))
            {
                filtroExtraFaceta += $" Filter (lang({ultimoObjeto})='{pIdioma}')";
            }

            return filtroExtraFaceta;
        }

        private static string ObtenerComplementoRango(TipoPropiedadFaceta pTipoPropiedadesFaceta, string pRango, bool pYaHayFiltroFaceta)
        {
            if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Calendario) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.CalendarioConRangos) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo))
            {
                if (pRango.Length == 1)
                {
                    while (pRango.Length < 3)
                    {
                        pRango += "0";
                    }
                }
                else
                {
                    if (pYaHayFiltroFaceta)
                    {
                        //Si contiene un filtro por la faceta Siglo, hay que filtrar por décadas
                        while (pRango.Length < 2)
                        {
                            pRango += "0";
                        }
                    }
                    else
                    {
                        while (pRango.Length < 4)
                        {
                            pRango += "0";
                        }
                    }
                }

                //Rango menor autocompletamos con 00 00 00
                if (pRango.Length <= 4)
                {
                    //SOLO Año
                    pRango += "0000000000";
                }

                if (pRango.Length == 6)
                {
                    //Año y Mes
                    pRango += "00000000";
                }

                if (pRango.Length == 8)
                {
                    //Año y Mes y día
                    pRango += "000000";
                }                
            }
            
            return pRango;
        }

        private string ObtenerSelectQueryObtenerFaceta(bool pEsRango, string pNombreFacetaSinPrefijo, int pNumeroAuxiliarVariablesIguales, List<int> pListaRangos, TipoPropiedadFaceta pTipoPropiedadesFaceta, TipoDisenio pTipoDisenio, Dictionary<string, List<string>> pListaFiltros, bool pEstaEnMyGnoss, string pClaveFaceta, string pConsultaExtraFiltros)
        {
            string select = "";
            string nombreVariable = pNombreFacetaSinPrefijo + pNumeroAuxiliarVariablesIguales;
            if (!string.IsNullOrEmpty(pConsultaExtraFiltros))
            {
                nombreVariable = pConsultaExtraFiltros;
            }
            string selectContador = "count(distinct ?s)";

            if (pEsRango)
            {
                if (pListaRangos == null || pListaRangos.Count == 0)
                {
                    if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo))
                    {
                        //Siglos y décadas
                        select += $"select ?a ?positive {(ObtenerContadorDeFaceta ? "sum(?recursos)" : string.Empty)} WHERE {{ ";
                        selectContador += " as ?recursos";
                        select += $"select ?a ?positive {(ObtenerContadorDeFaceta ? selectContador : string.Empty)} ";
                    }
                    else if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Fecha) || pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMeses))
                    {
                        select += $"select ?{nombreVariable}fecha {(ObtenerContadorDeFaceta ? $"({selectContador}) AS ?a" : "")}  ";
                    }
                    else if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.FechaMinMax))
                    {
                        select += $"select (MIN(?{nombreVariable}fecha) as ?fechaMin) (MAX(?{nombreVariable}fecha) as ?fechaMax)";
                    }
                    else
                    {
                        //Fecha normal
                        select += $"select distinct xsd:int(xsd:integer(?{nombreVariable})  / 10000000000)  AS ?a ";
                    }

                }
                else if (pTipoDisenio.Equals(TipoDisenio.Calendario) || pTipoDisenio.Equals(TipoDisenio.CalendarioConRangos))
                {
                    select += $"select ?{nombreVariable} {(ObtenerContadorDeFaceta ? $"({selectContador}) AS ?a" : "")}  ";
                }
                else
                {
                    if (pTipoPropiedadesFaceta.Equals(TipoPropiedadFaceta.Siglo) && pListaFiltros.ContainsKey(pClaveFaceta))
                    {
                        if (pListaFiltros[pClaveFaceta].Count > 1)
                        {
                            //Para los años hay que restar 1 al año propuesto (si es negativo, sumar 1)
                            if (pListaRangos[0] < 0)
                            {
                                select += $"select {((pListaRangos[0]) + 1)} {(ObtenerContadorDeFaceta ? $"({selectContador}) AS ?a" : "")} ";
                            }
                            else
                            {
                                select += $"select {(pListaRangos[0])} {(ObtenerContadorDeFaceta ? $"({selectContador}) AS ?a" : "")} ";
                            }
                        }
                        else
                        {
                            select += $"select {(pListaRangos[0])} {(ObtenerContadorDeFaceta ? $"({selectContador}) AS ?a" : "")} ";
                        }
                    }
                    else
                    {
                        select += $"select {((pListaRangos[0]) - 1)} {(ObtenerContadorDeFaceta ? $"({selectContador}) AS ?a" : "")} ";
                    }
                }
            }
            else if (!ObtenerContadorDeFaceta)
            {
                select += $"select distinct ?{nombreVariable} ";
            }
            else
            {
                select += $"select ?{nombreVariable} (count(distinct ?s)) AS ?a  ";
            }

            return select;
        }

        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFacetaSinOrdenDBLP(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluyente, bool pUsarHilos)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            string nombreFacetaSinPrefijo = QuitaPrefijo(pClaveFaceta);
            string[] LNiveles = null;
            string[] stringSeparators = new string[] { "@@@" };

            if (pClaveFaceta.Contains("@@@"))
            {
                LNiveles = pClaveFaceta.Split(stringSeparators, StringSplitOptions.None);

                nombreFacetaSinPrefijo = QuitaPrefijo(LNiveles[LNiveles.Length - 1]);
            }

            if (pEsRango)
            {
                if (pListaRangos == null || pListaRangos.Count == 0)
                {
                    query.Append($"select distinct xsd:int(xsd:integer(?{nombreFacetaSinPrefijo}2)  / 10000000000)  AS ?a ");
                }
                else
                {
                    query.Append($"select {(pListaRangos[0] - 1)} count(?s) AS ?a ");
                }
            }
            else if (pListaFiltros.ContainsKey("listAdded") || pEstaEnMyGnoss && (!pListaFiltros.ContainsKey("rdf:type") || !pListaFiltros["rdf:type"].Contains("Comunidad")) && pListaFiltros.ContainsKey("autocompletar") && pClaveFaceta.Contains("Tag"))
            {
                query.Append("select distinct ");
            }
            else
            {
                query.Append($"select ?{nombreFacetaSinPrefijo}2");
            }
            query.Append(ObtenerFrom(pProyectoID));

            query.Append("WHERE ");
            query.Append("{ ");
            
            bool esRecomendacion = false;

            if (pProyectoID.Contains("contactos"))
            {
                if (!pListaFiltros.ContainsKey("gnoss:RecPer"))
                {
                    query.Append($"?s <http://xmlns.com/foaf/0.1/knows> <http://gnoss/{pIdentidadID.ToUpper()}>. ");
                }
                else
                {
                    query.Append($"<http://gnoss/{pIdentidadID.ToUpper()}> <http://gnoss/RecPer>  ?s12. ?s12 <http://gnoss/RecID> ?s. ?s12 <http://gnoss/RecNum> ?s2n. FILTER (?s!=<http://gnoss/{pIdentidadID.ToUpper()}>)  MINUS {{<http://gnoss/{pIdentidadID.ToUpper()}> <http://gnoss/Ignora> ?s. }} ");
                    pListaFiltros.Remove("gnoss:RecPer");
                    esRecomendacion = true;
                }
            }

            query.Append(ObtenerParteFiltros(pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto));

            if (esRecomendacion)
            {
                pListaFiltros.Add("gnoss:RecPer", new List<string>());
                pListaFiltros["gnoss:RecPer"].Add(pIdentidadID.ToString().ToUpper());
            }

            if (pClaveFaceta.Substring(0, 2).Equals("cv"))
            {
                query.Append("?s gnoss:hasCv ?CV . ");
                query.Append($"?CV {pClaveFaceta} ?{nombreFacetaSinPrefijo}2. ");
            }
            else
            {
                if (pClaveFaceta.Contains("@@@"))
                {
                    string nombreObjeto = QuitaPrefijo(LNiveles[0]);
                    
                    //Facetas excluyentes.
                    //Excluyente: http://pruebas.gnoss.net/comunidad/aegp2/empresas#company:product@@@company:productSubgroup=hombre
                    //NO Excluyente: http://comunidades.gnoss.net/comunidad/publicacionesDeusto3/Publications#dc:creator@@@publication:autorID@@@foaf:name=diego%20l%C3%B3pez%20de%20ipi%C3%B1a%20gonz%C3%A1lez%20de%20artaza
                    FacetaAD facAD = new FacetaAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<FacetaAD>(), mloggerFactory);
                    if (pExcluyente)
                    {
                        //Las facetas excluidas no concatenan grafos: 
                        //?asociación product ?producto. ?producto nombreProducto ? nombreProducto
                        //Por defecto se añade un 0, por la limpieza en el código de variables de método "ObtenerParteFiltros" y así se mantiene la exclusividad de las variables.
                        nombreObjeto += "0";
                        query.Append($"?s {LNiveles[0]} ?{nombreObjeto}. ");
                    }
                    else
                    {
                        //Las facetas no excluidas siguen concatenan grafos:
                        //?publicacion autor ?autor. ?autor2 nombre ?nombre

                        nombreObjeto += "2";
                        query.Append($"?s {LNiveles[0]} ?{nombreObjeto}. ");
                    }
                    facAD.Dispose();

                    for (int i = 0; i < LNiveles.Length - 1; i++)
                    {
                        string nombreObjetoAnterior = nombreObjeto;
                        nombreObjeto = QuitaPrefijo(LNiveles[i + 1]) + "2";
                        query.Append($" ?{nombreObjetoAnterior} {LNiveles[i + 1]} ?{nombreObjeto}. ");
                    }
                }
                else
                {

                    query.Append($"?s {pClaveFaceta} ?{nombreFacetaSinPrefijo}2. ");
                }
            }

            if (!string.IsNullOrEmpty(CondicionExtraFacetas))
            {
                query.Append(CondicionExtraFacetas);
            }

            if ((pClaveFaceta.Equals("skos:ConceptID")) && pEstaEnMyGnoss)
            {
                query.Append($"?{nombreFacetaSinPrefijo}2 sioc:has_space ?has_space. ");
            }

            if (pProyectoID.Contains("contribuciones/") && pClaveFaceta.Equals("skos:ConceptID") && !pEstaEnMyGnoss)
            {
                query.Append($"?{nombreFacetaSinPrefijo}2 sioc:has_space {pListaFiltros["sioc:has_space"][0].ToString()}. ");
            }

            if (pEsRango && pListaRangos != null && pListaRangos.Count > 0)
            {
                string nombreVariable = pClaveFaceta.Replace(":", "") + "2";
                string rangoMayor = pListaRangos[0].ToString();
                string filtroRangoMenor = $"AND ?{nombreVariable} >= ";
                StringBuilder rangoMenor = new StringBuilder((pListaRangos[0] - 1).ToString());

                if (pListaRangos.Count > 1)
                {
                    //cojo el rango menor
                    rangoMenor = new StringBuilder(pListaRangos[1].ToString());

                    if (rangoMenor.Equals("-1"))
                    {
                        //Solo hay límite por arriba
                        filtroRangoMenor = "";
                        rangoMenor.Clear();
                        query = query.Replace($"select {pListaRangos[0] - 1} count(?s) ", $"select -{pListaRangos[0]} count(?s) ");
                    }
                }

                if (rangoMayor.Length == 4)
                {
                    //SOLO Año
                    rangoMayor += "0131235959";
                }

                if (rangoMayor.Length == 6)
                {
                    //Año y Mes
                    rangoMayor += "31235959";
                }

                if (rangoMayor.Length == 8)
                {
                    //Año y Mes y día
                    rangoMayor += "235959";
                }

                if (rangoMenor.Length > 0)
                {
                    while (rangoMenor.Length < 14)
                    {
                        rangoMenor.Append("0");
                    }
                }

                query.Append($" FILTER(?{nombreVariable} <= {rangoMayor} {filtroRangoMenor}{rangoMenor})}} ");
            }
            else
            {
                query.Append("}  ");

                if (pLimite > 0 && !pEsRango)
                {
                    query.Append($" LIMIT {pLimite}");
                }

                if (pInicio != 0 && !pEsRango)
                {
                    query.Append($" OFFSET {pInicio}");
                }
            }

            LeerDeVirtuoso(query.ToString(), pClaveFaceta, pFacetadoDS, pProyectoID, pUsarHilos);
        }

        /// <summary>
        /// Obtiene una faceta concreta para ciertas factas de DBLP.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFacetaEspecialDBLPJournalPartOF(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere)
        {
            string query = NamespacesVirtuosoLectura;

            query += "select distinct ?partOfPeso ";
            query += ObtenerFrom(pProyectoID);

            query += "WHERE ";
            query += "{ ";

            string filtro = pListaFiltros["autocompletar"][0];
            pListaFiltros.Remove("autocompletar");

            query += ObtenerParteFiltros(pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, TipoProyecto.CatalogoNoSocial);
            query += " ?s ";

            if (pNombreFaceta == "sioc_t:Tag")
            {
                query += "<http://rdfs.org/sioc/types#TagPeso>";
            }
            else if (pNombreFaceta == "dc:creator@@@foaf:name")
            {
                query += "<http://purl.org/dc/terms/creatorPeso>";
            }
            else if (pNombreFaceta == "swrc:journal@@@dc:title")
            {
                query += "<http://swrc.ontoware.org/ontology#journalPeso>";
            }
            else if (pNombreFaceta == "dc:partOf")
            {
                query += "<http://purl.org/dc/terms/partOfPeso>";
            }

            query += " ?partOfPeso. FILTER(?partOfPeso LIKE '% " + filtro + "%')";

            #region Orden

            query += " } order by desc (?partOfPeso)";// desc

            #endregion

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }

            LeerDeVirtuoso(query, pNombreFaceta, pFacetadoDS, pProyectoID);
        }
        /// <summary>
        /// Obtiene una faceta concreta para ciertas factas de DBLP.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta que se debe cargar</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss, no en una comunidad</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pOrden">Orden de los resultados</param>
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">límite de resultados</param>
        /// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
        /// <param name="pFiltroContextoWhere">Filtros de contexto</param>
        /// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
        /// <param name="pSemanticos">Lista de formularios semánticos</param>
        public void ObtenerFacetaEspecialDBLP(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere)
        {
            string query = NamespacesVirtuosoLectura;

            query += "select distinct ?partOfPeso ";
            query += ObtenerFrom(pProyectoID);

            query += "WHERE ";
            query += "{ ";

            string filtro = pListaFiltros["autocompletar"][0];
            pListaFiltros.Remove("autocompletar");

            query += ObtenerParteFiltros(pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, TipoProyecto.CatalogoNoSocial);
            query += " ?s ";

            if (pNombreFaceta == "sioc_t:Tag")
            {
                query += "  sioc_t:TagID ?TagID.  ?TagID sioc_t:TagName ?partOfPeso.   ?TagID";
                query += "  sioc_t:TagAut" + filtro.Length.ToString() + " ";
                query += "  ?TagAut2. ";
            }
            else if (pNombreFaceta == "dc:creator@@@foaf:name")
            {
                query += "  <http://purl.org/dc/terms/creatorID> ?AutorID.  ?AutorID <http://purl.org/dc/terms/creatorName> ?partOfPeso.   ?AutorID";
                query += "  <http://purl.org/dc/terms/creatorAut" + filtro.Length.ToString() + ">";
                query += "  ?Autor2. ";
            }
            else if (pNombreFaceta == "swrc:journal@@@dc:title")
            {
                query += "<http://swrc.ontoware.org/ontology#journalPeso>";
            }
            else if (pNombreFaceta == "dc:partOf")
            {
                query += "<http://purl.org/dc/terms/partOfPeso>";
            }


            if (pNombreFaceta == "sioc_t:Tag")
            {
                query += "  FILTER(?TagAut2='" + filtro + "')";
            }
            else if (pNombreFaceta == "dc:creator@@@foaf:name")
            {

                query += "  FILTER(?Autor2='" + filtro + "')";
            }
            else
            {
                query += " ?partOfPeso. FILTER(?partOfPeso LIKE '% " + filtro + "%')";

            }
            #region Orden


            query += " } order by desc (?partOfPeso)";// desc

            #endregion

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }

            LeerDeVirtuoso(query, pNombreFaceta, pFacetadoDS, pProyectoID);
        }


        public void ObtenerResultadosBusqueda(string pProyectoID, bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, bool pEsMovil, List<Guid> pListaExcluidos, string pLanguageCode, bool pUsarAfinidad = false)
        {
            ObtenerResultadosBusqueda(pProyectoID, pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, TipoProyecto.Catalogo, "", "", pEsMovil, pListaExcluidos, pLanguageCode, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pTipoProyecto">Tipo del proyecto para el que se está haciendo la consulta</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        public void ObtenerResultadosBusqueda(string pProyectoID, bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, bool pEsMovil, List<Guid> pListaExcluidos, string pLanguageCode, bool pUsarAfinidad = false)
        {
            ObtenerResultadosBusqueda(pProyectoID, pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, TipoProyecto.Catalogo, "", "", true, pEsMovil, pListaExcluidos, pLanguageCode, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pTipoProyecto">Tipo del proyecto para el que se está haciendo la consulta</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerResultadosBusqueda(string pProyectoID, bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, bool pEsMovil, List<Guid> pListaExcluidos, string pLanguageCode, bool pUsarAfinidad = false)
        {
            ObtenerResultadosBusqueda(pProyectoID, pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pPermitirRecursosPrivados, true, TiposAlgoritmoTransformacion.Ninguno, null, pEsMovil, pListaExcluidos, pLanguageCode, pUsarAfinidad);
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pTipoProyecto">Tipo del proyecto para el que se está haciendo la consulta</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtenerResultadosBusqueda(string pProyectoID, bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, bool pObtenerRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, List<Guid> pListaExcluidos, string pLanguageCode, bool pUsarAfinidad = false)
        {
            string[] filtrosContextos = null;
            if (!string.IsNullOrEmpty(pFiltroContextoOrderBy))
            {
                filtrosContextos = GenerarOrderByContextos(pFiltroContextoOrderBy);
            }

            this.mConsultaResultados = true;

            if (mListaItemsBusquedaExtra == null)
            {
                mListaItemsBusquedaExtra = pListaFiltrosExtra;
            }

            if (string.IsNullOrEmpty(pTipoFiltro) && string.IsNullOrEmpty(pFiltroContextoSelect) && string.IsNullOrEmpty(pFiltroContextoWhere) && string.IsNullOrEmpty(pFiltroContextoOrderBy))
            {
                pTipoFiltro = "gnoss:hasfechapublicacion";
            }

            string select = " select ";
            bool hayFiltroSearch = pListaFiltros.ContainsKey("search");

            if (pListaFiltros.Count > 0)
            {
                if (pListaFiltros.Keys.Any(item => item.Contains("@@@")))
                {
                    select += " distinct ";
                }
            }

            if (!string.IsNullOrEmpty(pFiltroContextoSelect) && !string.IsNullOrEmpty(pFiltroContextoWhere))
            {
                if (pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso))
                {
                    select += " ?s '" + pListaFiltros["rdf:type"][0] + "' as ?rdftype " + pFiltroContextoSelect + " as ?a ";
                }
                else
                {
                    select += " ?s ?rdftype " + pFiltroContextoSelect + " as ?a ";
                }
            }
            else if (pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso))
            {
                string tipo = pListaFiltros["rdf:type"][0];
                select += " ?s '" + tipo + "' as ?rdftype";
            }
            else
            {
                if (pProyectoID.Contains("contribuciones/"))
                {
                    if (!select.Contains("distinct"))
                    {
                        select += " distinct ";
                    }

                    select += " ?s (bif:substring(STR(?rdftype), 1, 7) AS ?rdftype)   ";
                }
                else
                {
                    if (!select.Contains("distinct"))
                    {
                        select += " distinct ";
                    }
                    select += " ?s ?rdftype ";
                }
            }

            string query = ObtenerFrom(pProyectoID);

            query += " WHERE { ";

            bool busquedaMensajesOComent = (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Count > 0 && (pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_MENSAJES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_COMENTARIOS || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_INVITACIONES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_SUSCRIPCIONES));

            query += "{";

            if (!busquedaMensajesOComent && !pProyectoID.Contains("contactos") && !pProyectoID.Contains("contribuciones"))
            {
                query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, pObtenerRecursosPrivados);
            }

            bool esRecomendacion = false;

            if (pProyectoID.Contains("contactos"))
            {
                query += " { ";

                if (!pListaFiltros.ContainsKey("gnoss:RecPer"))
                {
                    query += "?s <http://xmlns.com/foaf/0.1/knows> <http://gnoss/" + pIdentidadID + ">. ";
                }
                else
                {
                    query += "<http://gnoss/" + pIdentidadID + "> <http://gnoss/RecPer>  ?s12. ?s12 <http://gnoss/RecID> ?s. ?s12 <http://gnoss/RecNum> ?s2n. FILTER (?s!=<http://gnoss/" + pIdentidadID + ">)  MINUS {<http://gnoss/" + pIdentidadID + "> <http://gnoss/Ignora> ?s.} ";
                    pListaFiltros.Remove("gnoss:RecPer");
                    esRecomendacion = true;

                    if (!select.Contains("distinct"))
                    {
                        select = select.Replace("select", "select distinct ");
                    }
                }
            }

            StringBuilder filtros = new StringBuilder(ObtenerParteFiltros("", new Dictionary<string, List<string>>(pListaFiltros), pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, false, pOmitirPalabrasNoRelevantesSearch, pTipoAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pEsMovil));

            if (!string.IsNullOrEmpty(pResultadosEliminar))
            {
                string[] resultados = pResultadosEliminar.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                filtros.Append(" FILTER (");
                string and = "";
                foreach (string resultado in resultados)
                {
                    filtros.Append($"{and}?s!=gnoss:{resultado.ToUpper()}");
                    and = " and ";
                }

                filtros.Append(")");
            }

            filtros.Append(" } ");

            if (esRecomendacion)
            {
                pListaFiltros.Add("gnoss:RecPer", new List<string>());
                pListaFiltros["gnoss:RecPer"].Add(pIdentidadID.ToString().ToUpper());
            }

            StringBuilder optional = new StringBuilder();

            List<string> tiposFiltro = new List<string>();
            Dictionary<string, string> condicionesAdicionalesPorTipoFiltro = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(pTipoFiltro))
            {
                if (pTipoFiltro.Contains("###ClausulaOrdenPersonalizado###"))
                {
                    tiposFiltro.Add(pTipoFiltro);
                }
                else
                {
                    tiposFiltro = pTipoFiltro.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    for (int i = 0; i < tiposFiltro.Count; i++)
                    {
                        int indiceCondicion = tiposFiltro[i].IndexOf('[');
                        if (indiceCondicion > 0)
                        {
                            // Lleva una condición adicional, la separo del filtro
                            string filtro = tiposFiltro[i].Substring(0, indiceCondicion);
                            string condicion = tiposFiltro[i].Substring(indiceCondicion + 1).TrimEnd(']');

                            tiposFiltro[i] = filtro;
                            condicionesAdicionalesPorTipoFiltro.Add(filtro, condicion);
                        }
                    }
                }
            }

            List<DatosPropiedadOntologia> datosPropiedadesOntologias = new List<DatosPropiedadOntologia>();
            CargarDatosPropiedadesOntologias(pListaFiltros, tiposFiltro, datosPropiedadesOntologias);

            foreach (string tipoFiltro in tiposFiltro)
            {
                string ultimoFiltro;
                bool multiIdioma = PropiedadFiltroEsMultiIdioma(datosPropiedadesOntologias, tipoFiltro);

                //order by
                //Apaño Ineverycrea, problema con las fechas traidas a través del optional desde virtuoso.
                //Posiblemente por algún tema de la compilación de virtuoso porque debería funcionar bien.
                if (pListaFiltros.ContainsKey("rdf:type") && !(pListaFiltros["rdf:type"].Contains(TiposResultadosMetaBuscador.Comentario.ToString()) || pListaFiltros["rdf:type"].Contains(TiposResultadosMetaBuscador.Invitacion.ToString())))
                {
                    //order by
                    if (!string.IsNullOrEmpty(tipoFiltro) && pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Contains(BUSQUEDA_RECURSOS))
                    {
                        if (!filtros.ToString().Contains("?s " + tipoFiltro + " ?" + QuitaPrefijo(tipoFiltro) + " ") && !QuitaPrefijo(tipoFiltro).Contains("relevancia") && !tipoFiltro.Contains("###ClausulaOrdenPersonalizado###") && condicionesAdicionalesPorTipoFiltro.ContainsKey(tipoFiltro))
                        {
                            optional.Append($"OPTIONAL {{ ?s {tipoFiltro} ?{QuitaPrefijo(tipoFiltro)} . {ObtenerCondicionAdicionalTipoFiltro(tipoFiltro, condicionesAdicionalesPorTipoFiltro, false)} }} ");
                        }
                        else if (condicionesAdicionalesPorTipoFiltro.ContainsKey(tipoFiltro))
                        {
                            optional.Append($" FILTER({condicionesAdicionalesPorTipoFiltro[tipoFiltro]})");
                        }
                    }

                    if (!string.IsNullOrEmpty(tipoFiltro) && pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Contains(BUSQUEDA_MENSAJES))
                    {
                        if (!filtros.ToString().Contains("?s " + tipoFiltro + " ?" + QuitaPrefijo(tipoFiltro) + " ") && !QuitaPrefijo(tipoFiltro).Contains("relevancia"))
                        {
                            optional.Append($"?s {tipoFiltro} ?{QuitaPrefijo(tipoFiltro)} . {ObtenerCondicionAdicionalTipoFiltro(tipoFiltro, condicionesAdicionalesPorTipoFiltro, false)}");
                        }
                        else if (condicionesAdicionalesPorTipoFiltro.ContainsKey(tipoFiltro))
                        {
                            optional.Append($" FILTER({condicionesAdicionalesPorTipoFiltro[tipoFiltro]})");
                        }
                    }

                    if (!pProyectoID.Contains("contactos"))
                    {
                        if (!string.IsNullOrEmpty(tipoFiltro) && !filtros.ToString().Contains("?s " + tipoFiltro + " ?" + QuitaPrefijo(tipoFiltro) + " "))
                        {
                            if (hayFiltroSearch)
                            {
                                if (!busquedaMensajesOComent)
                                {
                                    if (tipoFiltro != "gnoss:haspopularidad" && tipoFiltro != "gnoss:hasnumerorecursos" && tipoFiltro != "gnoss:hasfechapublicacion" && !tipoFiltro.Equals("gnoss:relevancia"))
                                    {
                                        optional.Append(ObtenerFiltroOptional(tipoFiltro, query, out ultimoFiltro, multiIdioma, condicionesAdicionalesPorTipoFiltro, pLanguageCode));
                                    }
                                    else if (tipoFiltro.Equals("gnoss:relevancia"))
                                    {
                                        optional.Append(" OPTIONAL { ?s gnoss:hasfechapublicacion ?gnosshasfechapublicacion .  }  ");
                                    }
                                }
                            }
                            else if (!tipoFiltro.Equals("gnoss:relevancia"))
                            {
                                if (!pListaFiltros.ContainsKey(tipoFiltro))
                                {
                                    if (!optional.ToString().Contains("?s " + tipoFiltro))
                                    {
                                        optional.Append(ObtenerFiltroOptional(tipoFiltro, query, out ultimoFiltro, multiIdioma, condicionesAdicionalesPorTipoFiltro, pLanguageCode));
                                    }
                                    else if (condicionesAdicionalesPorTipoFiltro.ContainsKey(tipoFiltro))
                                    {
                                        optional.Append($" FILTER({condicionesAdicionalesPorTipoFiltro[tipoFiltro]})");
                                    }
                                }
                                else if (condicionesAdicionalesPorTipoFiltro.ContainsKey(tipoFiltro))
                                {
                                    optional.Append($" FILTER({condicionesAdicionalesPorTipoFiltro[tipoFiltro]})");
                                }
                            }
                            else if (!ExisteOrderBySearchPersonalizado(pListaFiltros, pFiltrosSearchPersonalizados))
                            {
                                optional.Append("OPTIONAL { ?s gnoss:haspopularidad ?gnosshaspopularidad. ?s gnoss:hasnumerorecursos ?gnosshasnumerorecursos. ?s gnoss:hasfechapublicacion ?gnosshasfechapublicacion . ?s foaf:firstname ?foaffirstname .} ");
                            }
                        }
                    }
                    else
                    {
                        filtros.Append(" } ");

                        if (!esRecomendacion)
                        {
                            filtros.Append($" OPTIONAL {{ ?s {tipoFiltro} ?{QuitaPrefijo(tipoFiltro)} . {ObtenerCondicionAdicionalTipoFiltro(tipoFiltro, condicionesAdicionalesPorTipoFiltro, false)}}} ");
                        }
                    }
                }
                else
                {
                    filtros.Append($" OPTIONAL {{ ?s {tipoFiltro} ?{QuitaPrefijo(tipoFiltro)} . {ObtenerCondicionAdicionalTipoFiltro(tipoFiltro, condicionesAdicionalesPorTipoFiltro, false)}}}");
                }

                if (pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso) && optional.Length > 0)
                {
                    filtros = new StringBuilder($" {{ {filtros} }} ");
                }
            }

            string optionalFiltrosOrdenFacetas = string.Empty;
            bool priorizarFacetasEnOrden = FacetaAD.PriorizarFacetasEnOrden(mFacetaDW, pListaFiltros) && QuitaPrefijo(pTipoFiltro).Contains("relevancia");

            if (priorizarFacetasEnOrden)
            {
                List<string> valoresFiltrosOrdenFacetas = new List<string>();
                List<string> facetasAfectanOrden = mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Where(f => f.PriorizarOrdenResultados).Select(f => f.Faceta).ToList();

                foreach (string facetaAfectaOrden in facetasAfectanOrden.Where(item => pListaFiltros.ContainsKey(item)))
                {
                    foreach (string valorFiltro in pListaFiltros[facetaAfectaOrden])
                    {
                        string valorSinIdioma = string.Empty;
                        if (UtilCadenas.EsMultiIdioma(valorFiltro))
                        {
                            valorSinIdioma = valorFiltro.Substring(0, valorFiltro.LastIndexOf("@"));
                        }

                        string[] partes = valorSinIdioma.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string parte in partes)
                        {
                            if (!valoresFiltrosOrdenFacetas.Contains(parte))
                            {
                                valoresFiltrosOrdenFacetas.Add(parte);
                            }
                        }
                    }
                }

                if (valoresFiltrosOrdenFacetas.Count > 0)
                {
                    string union = "";
                    StringBuilder valoresFiltro = new StringBuilder();
                    foreach (string valor in valoresFiltrosOrdenFacetas)
                    {
                        valoresFiltro.Append($"{union} {{ ?s <http://xmlns.com/foaf/0.1/firstName> ?titulo. ?titulo bif:contains\"'{valor}'\" Option(score ?scoretitle) }}");
                        union = " UNION ";
                    }

                    if (valoresFiltro.Length > 0)
                    {
                        optionalFiltrosOrdenFacetas = " OPTIONAL { " + valoresFiltro.ToString() + " } ";
                    }
                }
            }

            query += filtros + optionalFiltrosOrdenFacetas + optional;

            if (filtrosContextos != null)
            {
                query += filtrosContextos[0];
            }

            // resultados excluidos
            if (pListaExcluidos != null && pListaExcluidos.Count > 0)
            {
                string filtroExcluidos = ObtenerFiltroRecursosExcluidos(pListaExcluidos);
                query = $" {query}{filtroExcluidos}";
            }

            query += " } ";

            //order by
            StringBuilder orderBy = new StringBuilder();

            bool haySubconsultaScoreTitle = false;

            if (hayFiltroSearch && !mConsultaNumeroResultados && (QuitaPrefijo(pTipoFiltro).Contains("relevancia")))
            {
                if (query.Contains("?scoreTitle") && query.Contains("?scoreSearch"))
                {
                    orderBy.Append(" desc(sum(?scoreTitle)) desc(sum(?scoreSearch)) ");
                }
                else
                {
                    orderBy.Append(" desc(?scoreTitle) ");
                }

                select = $" {select} where {{{select}";
                haySubconsultaScoreTitle = true;
            }

            bool haySearchPersonalizado = false;
            if (pFiltrosSearchPersonalizados != null && !string.IsNullOrEmpty(pTipoFiltro) && (QuitaPrefijo(pTipoFiltro).Contains("relevancia")))
            {
                foreach (string claveSearch in pFiltrosSearchPersonalizados.Keys)
                {
                    foreach (string claveFiltro in pListaFiltros.Keys)
                    {
                        if (claveFiltro == claveSearch)
                        {
                            if (!string.IsNullOrEmpty(pFiltrosSearchPersonalizados[claveSearch].Item2))
                            {
                                orderBy = new StringBuilder($" {pFiltrosSearchPersonalizados[claveSearch].Item2} ");
                            }
                            haySearchPersonalizado = true;
                            break;
                        }
                    }
                }
            }

            if (haySearchPersonalizado)
            {
                //Si hay search personalizado no aplicar más ordenes 
            }
            else if (esRecomendacion)
            {
                orderBy.Append(" desc (?s2n)  ");
            }
            else if (!string.IsNullOrEmpty(pFiltroContextoSelect) && !string.IsNullOrEmpty(pFiltroContextoWhere))
            {
                if (filtrosContextos == null)
                {
                    orderBy.Append(" desc (?a) (?s)  ");
                }
                else
                {
                    orderBy.Append(filtrosContextos[1]);
                }
            }
            else if (QuitaPrefijo(pTipoFiltro).Contains("relevancia"))
            {
                orderBy.Append(" desc (?gnosshaspopularidad) desc (?gnosshasnumerorecursos)  desc (?gnosshasfechapublicacion) asc (?foaffirstname) ");
            }
            else
            {
                foreach (string tipoFiltro in tiposFiltro)
                {
                    string ultimoFiltro = QuitaPrefijo(tipoFiltro);

                    if (pDescendente)
                    {
                        orderBy.Append(" desc ");
                    }

                    if (ultimoFiltro.Contains("@@@"))
                    {
                        ultimoFiltro = ultimoFiltro.Substring(ultimoFiltro.LastIndexOf("@@@") + 3);

                        int varAux = 0;
                        while (query.Contains("?" + ultimoFiltro + varAux))
                        {
                            varAux++;
                        }

                        varAux--; //Quitamos uno porque se supone que nos tenemos que quedar con la última aparición de la variable.

                        ultimoFiltro = ultimoFiltro + varAux;
                    }

                    DatosPropiedadOntologia datosPropiedadOntologia = ObtenerDatosPropiedadOntologiaDeFiltro(datosPropiedadesOntologias, tipoFiltro);
                    if (datosPropiedadOntologia != null && (datosPropiedadOntologia.Propiedad.Rango.Equals("xsd:string") || datosPropiedadOntologia.Propiedad.Rango.Equals(XSD_STRING)))
                    {
                        //Si la propiedad por la que se ordena es un string ordenamos teniendo en cuenta los acentos.
                        orderBy.Append($"(bif:rdf_collation_order_string('DB.DBA.LEXICAL_ACUTE', lcase(?{ultimoFiltro}))) ");
                    }
                    else
                    {
                        if (ultimoFiltro.Contains("###ClausulaOrdenPersonalizado###"))
                        {
                            string parteOrderBy = ultimoFiltro.Split("###ClausulaOrdenPersonalizadoSeparador###")[0].Replace("###ClausulaOrdenPersonalizado###", "");
                            if (pDescendente)
                            {
                                string[] listaOrder = parteOrderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < listaOrder.Length; i++)
                                {
                                    if (i == 0)
                                    {
                                        orderBy.Append($"({listaOrder[i]})");
                                    }
                                    else
                                    {
                                        orderBy.Append($" desc({listaOrder[i]})");
                                    }
                                }
                            }
                            else
                            {
                                orderBy.Append($"{parteOrderBy} ");
                            }
                        }
                        else
                        {
                            orderBy.Append($"(?{ultimoFiltro}) ");
                        }
                    }
                }
            }

            if (priorizarFacetasEnOrden)
            {
                orderBy = new StringBuilder($" desc(sum(?scoretitle)) {orderBy}");
            }

            if (orderBy.Length > 0)
            {
                orderBy = new StringBuilder($" order by {orderBy}");
                query += orderBy;
            }

            if (pLimite > 0)
            {
                query += "LIMIT " + pLimite;
            }
            if (pInicio > 0)
            {
                query += " OFFSET " + pInicio;
            }

            if (haySubconsultaScoreTitle)
            {
                query += "}";
            }

            query = NamespacesVirtuosoLectura + pNamespaceExtra + select + query;

            if (ObtenerSoloConsulta)
            {
                if (!pFacetadoDS.Tables.Contains("ConsultasBusqueda"))
                {
                    pFacetadoDS.Tables.Add("ConsultasBusqueda");
                    pFacetadoDS.Tables["ConsultasBusqueda"].Columns.Add("Consulta");
                    pFacetadoDS.Tables["ConsultasBusqueda"].Columns.Add("Numero");
                }
                if (query.Trim().ToUpper().StartsWith("SPARQL")) // Eliminar el SPARQL inicial
                {
                    query = query.Trim().Substring(6);
                }
                pFacetadoDS.Tables["ConsultasBusqueda"].Rows.Add(query, 0);
            }

            LeerDeVirtuoso(query, "RecursosBusqueda", pFacetadoDS, pProyectoID, pUsarAfinidad);

            if (pFiltroContextoPesoMinimo > 0 && !string.IsNullOrEmpty(pFiltroContextoSelect) && !string.IsNullOrEmpty(pFiltroContextoWhere))
            {
                DataRow[] filas = pFacetadoDS.Tables["RecursosBusqueda"].Select("a<" + pFiltroContextoPesoMinimo);
                foreach (DataRow fila in filas)
                {
                    fila.Delete();
                }
            }
        }

        private static bool ExisteOrderBySearchPersonalizado(Dictionary<string, List<string>> pListaFiltros, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados)
        {
            return pListaFiltros.Any(x => pFiltrosSearchPersonalizados.ContainsKey(x.Key) && !string.IsNullOrWhiteSpace(pFiltrosSearchPersonalizados[x.Key].Item2));
        }

        private static string ObtenerCondicionAdicionalTipoFiltro(string pTipoFiltro, Dictionary<string, string> pListaCondicionesAdicionalesTipoFiltro, bool pContieneFilter)
        {
            string condicion = "";

            if (pListaCondicionesAdicionalesTipoFiltro.ContainsKey(pTipoFiltro))
            {
                condicion = pListaCondicionesAdicionalesTipoFiltro[pTipoFiltro];

                if (condicion.StartsWith(')'))
                {
                    // No se quiere añadir algo al filter, es otro tipo de condición (por ejemplo un BIND())
                    if (!pContieneFilter)
                    {
                        condicion = condicion.TrimStart(')');
                        if (condicion.Count(caracter => caracter.Equals('(')) > condicion.Count(caracter => caracter.Equals(')')))
                        {
                            condicion += ")";
                        }
                    }
                }
                else
                {
                    if (pContieneFilter)
                    {
                        condicion = $" AND {condicion}";
                    }
                    else
                    {
                        condicion = $" FILTER({condicion})";
                    }
                }

                if (condicion.Contains("GETDATE()"))
                {
                    condicion = condicion.Replace("GETDATE()", DateTime.Now.ToString("yyyyMMdd") + "000000");
                }
            }

            return condicion;
        }

        private string ObtenerExcepcionesMovil(string pProyectoID)
        {
            string excepcion = "";

            Guid proyectoID;

            if (Guid.TryParse(pProyectoID, out proyectoID))
            {
                ProyectoAD proyAD = new ProyectoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<ProyectoAD>(),mloggerFactory);
                string listaExcepciones = proyAD.ObtenerExcepcionesMovil(proyectoID);

                if (!string.IsNullOrEmpty(listaExcepciones))
                {
                    excepcion = listaExcepciones;
                }
            }
            return excepcion;
        }

        /// <summary>
        /// Generamos una lista a partir del análisis de los xml y owl necesarios con los datos imprescindibles para trabajar con las propiedades definidas en los filtros pasados por parámetro
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros pasados por el usuario</param>
        /// <param name="pTiposFiltro">Array con las Propiedad de las cuales queremos obtener su configuración. Si una propiedad tiene un selector de entidad o apunta a una entidad auxiliar estará concatenada por @@@</param>
        /// <param name="pDatosPropiedadesOntologias">Lista de DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, Xml) que queremos obtener a partir del análisis de la ontología</param>
        private void CargarDatosPropiedadesOntologias(Dictionary<string, List<string>> pListaFiltros, IEnumerable<string> pTiposFiltro, List<DatosPropiedadOntologia> pDatosPropiedadesOntologias)
        {
            foreach (string tipoFiltro in pTiposFiltro)
            {
                string nombreOntologia = null;
                if (pListaFiltros.ContainsKey("rdf:type"))
                {
                    foreach (string ontologia in pListaFiltros["rdf:type"].Where(item => !item.Equals("Recurso")))
                    {
                        nombreOntologia = $"{ontologia}.owl";
                        bool propiedadEncontrada = CargarDatosPropiedadOntologia(nombreOntologia, tipoFiltro, pDatosPropiedadesOntologias);

                        if (propiedadEncontrada)
                        {
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtenemos y analizamos el xml y el owl de la ontología pasada por parámetro para obtener los datos necesarios de la propiedad final definida en el filtro.
        /// Si el filtro tiene varios niveles concatenados con @@@ navegamos entre ellos hasta llegar a la propiedad final.
        /// </summary>
        /// <param name="pNombreOntologia">Nombre de la ontologia a la que pertenece la propiedad actual del filtro</param>
        /// <param name="pTipoFiltro">Propiedad de la cual queremos obtener su configuración. Si la propiedad tiene un selector de entidad o apunta a una entidad auxiliar estará concatenada por @@@</param>
        /// <param name="pListaDatosPropiedadOntologia">Lista de DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, XmlOntologia) que queremos obtener a partir del análisis de la ontología</param>
        /// <returns>True o False en función de si se ha encontrado y cargado la propiedad buscada en la ontología indicada</returns>
        private bool CargarDatosPropiedadOntologia(string pNombreOntologia, string pTipoFiltro, List<DatosPropiedadOntologia> pListaDatosPropiedadOntologia)
        {
            bool propiedadEncontrada = false;
            Documento documento = mEntityContext.Documento.Where(item => item.Enlace == pNombreOntologia && !item.Eliminado && (item.Tipo == (int)TiposDocumentacion.Ontologia || item.Tipo == (int)TiposDocumentacion.OntologiaSecundaria)).FirstOrDefault();
            if (documento != null)
            {
                byte[] bytesOntologias = ObtenerOntologia(documento.DocumentoID);
                byte[] bytesXmlOntologia = ObtenerXmlOntologia(documento.DocumentoID);
                Ontologia ontologia = new Ontologia(bytesOntologias);
                ontologia.LeerOntologia();

                XmlDocument doc = new XmlDocument();
                MemoryStream ms = new MemoryStream(bytesXmlOntologia);
                doc.Load(ms);

                if (!pTipoFiltro.Contains("@@@"))
                {
                    Propiedad propiedad = ObtenerPropiedadDeEntidad(ontologia.Entidades, pTipoFiltro);
                    if (propiedad != null)
                    {
                        pListaDatosPropiedadOntologia.Add(new DatosPropiedadOntologia() { NombrePropiedad = pTipoFiltro, Propiedad = propiedad, RdfType = pNombreOntologia, XmlOntologia = doc });
                        propiedadEncontrada = true;
                    }
                }
                else
                {
                    string propiedadFinal = pTipoFiltro.Substring(pTipoFiltro.IndexOf("@@@") + 3);
                    string propiedadActual = pTipoFiltro.Substring(0, pTipoFiltro.IndexOf("@@@"));

                    string grafo = ObtenerGrafoSelectorEntidad(propiedadActual, ontologia.Entidades, doc);

                    if (!string.IsNullOrEmpty(grafo))
                    {
                        CargarDatosPropiedadOntologia(grafo, propiedadFinal, pListaDatosPropiedadOntologia);
                    }
                    else
                    {
                        propiedadEncontrada = ObtenerDatosPropiedadAuxiliar(propiedadFinal, ontologia.EntidadesAuxiliares, doc, pListaDatosPropiedadOntologia, pNombreOntologia);
                    }
                }
            }

            return propiedadEncontrada;
        }

        /// <summary>
        /// Obtiene el objeto Propiedad de la lista de entidades pasadas por parámetro en función del nombre de la propiedad a buscar
        /// </summary>
        /// <param name="pEntidades">Lista de elementos de la ontología a la que pertenece la propiedad buscada</param>
        /// <param name="pNombrePropiedad">Nombre de la propiedad a buscar</param>
        /// <returns>El objeto Propiedad si es encontrado. En caso de no encontrarlo devolverá null</returns>
        private static Propiedad ObtenerPropiedadDeEntidad(List<ElementoOntologia> pEntidades, string pNombrePropiedad)
        {
            foreach (ElementoOntologia elemento in pEntidades)
            {
                Propiedad propiedadBuscada = elemento.Propiedades.Find(item => item.NombreConNamespace.Equals(pNombrePropiedad));
                if (propiedadBuscada != null)
                {
                    return propiedadBuscada;
                }
            }

            return null;
        }

        /// <summary>
        /// Obtiene los datos de la propiedad final que nos interesan de la lista, según el filtro pasados por parámetro, el cual contendrá una propiedad o una cadena de propiedades concatenadas con @@@.
        /// </summary>
        /// <param name="pDatosPropiedadesOntologias">Lista de DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, XmlOntologia) de la que queremos obtener los datos</param>
        /// <param name="pTipoFiltro">Filtro que contiene una propiedad o una cadena de propiedades concatenadas con @@@ por la que vamos a filtrar para obtener los DatosPropiedadOntologia de la propiedad final.</param>
        /// <returns>El objeto DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, XmlOntologia) perteneciente a la propiedad final del filtro pasado por parámetro. En caso de no encontrarlo devolverá null</returns>
        private static DatosPropiedadOntologia ObtenerDatosPropiedadOntologiaDeFiltro(List<DatosPropiedadOntologia> pDatosPropiedadesOntologias, string pTipoFiltro)
        {
            string[] propiedadesAnidadas = pTipoFiltro.Split("@@@");
            string nombrePropiedad = propiedadesAnidadas[propiedadesAnidadas.Length - 1];

            return pDatosPropiedadesOntologias.Find(item => item.NombrePropiedad.Equals(nombrePropiedad));
        }

        /// <summary>
        /// Dado un filtro que contiene una propiedad o una cadena de propiedades concatenadas con @@@, comprobamos si la propiedad final (propiedad destino) es o no multi idioma.
        /// </summary>
        /// <param name="pDatosPropiedadesOntologias">Lista de DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, XmlOntologia) que necesitamos para comprobar si el filtro es multi idioma</param>
        /// <param name="pTipoFiltro">Filtro que contiene la propiedad de la cual queremos comprobar si es multi idioma</param>
        /// <returns>True o False en función de si la propiedad final del filtro es o no multi idioma</returns>
        private static bool PropiedadFiltroEsMultiIdioma(List<DatosPropiedadOntologia> pDatosPropiedadesOntologias, string pTipoFiltro)
        {
            DatosPropiedadOntologia datosPropiedadOntologia = ObtenerDatosPropiedadOntologiaDeFiltro(pDatosPropiedadesOntologias, pTipoFiltro);
            if (datosPropiedadOntologia == null)
            {
                return false;
            }
            return PropiedadEsMultiIdioma(datosPropiedadOntologia);
        }

        /// <summary>
        /// Comprueba si la propiedad pasada por parámetro es o no multidioma analizando el Xml de la ontología a la cual pertenece la propiedad.
        /// </summary>
        /// <param name="pDatosPropiedadOntologia">DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, XmlOntologia) de la propiedad de la cual queremos saber si es multi idioma o no</param>
        /// <returns>True o False en función de si la propiedad pasada por parámetro es o no multi idioma</returns>
        private static bool PropiedadEsMultiIdioma(DatosPropiedadOntologia pDatosPropiedadOntologia)
        {
            bool propiedadEsMultiIdioma = false;

            Propiedad propiedad = pDatosPropiedadOntologia.Propiedad;
            XmlDocument doc = pDatosPropiedadOntologia.XmlOntologia;

            if (propiedad.Rango.Equals("http://www.w3.org/2001/XMLSchema#string"))
            {
                XmlNode multiidioma = doc.SelectSingleNode("config/ConfiguracionGeneral/MultiIdioma");
                if (multiidioma != null && (multiidioma.InnerText.ToLower().Equals("true") || multiidioma.InnerText.Equals(string.Empty)))
                {
                    XmlNode multiidiomaPropiedad = doc.SelectSingleNode($"config/EspefPropiedad/Propiedad[@ID =\"{propiedad.Nombre}\" and @EntidadID=\"{propiedad.ElementoOntologia.TipoEntidad}\"]/MultiIdioma");

                    if (multiidiomaPropiedad == null)
                    {
                        XmlNode tipoObjeto = doc.SelectSingleNode($"config/EspefPropiedad/Propiedad[@ID =\"{propiedad.Nombre}\" and @EntidadID=\"{propiedad.ElementoOntologia.TipoEntidad}\"]/TipoCampo");
                        if (tipoObjeto == null || (tipoObjeto != null && !tipoObjeto.InnerText.Equals("Link") && !tipoObjeto.InnerText.Equals("Imagen")))
                        {
                            propiedadEsMultiIdioma = true;
                        }
                    }
                    else
                    {
                        if (multiidiomaPropiedad.InnerText.ToLower().Equals("true") || multiidiomaPropiedad.InnerText.Equals(string.Empty))
                        {
                            propiedadEsMultiIdioma = true;
                        }
                    }
                }
            }
            return propiedadEsMultiIdioma;
        }

        /// <summary>
        /// Obtiene el grafo del selector de entidad de la propiedad pasada por parámetro leyendo el xml de su ontología. Si la propiedad no tiene selector de entidad (es una auxiliar) devuelve null.
        /// </summary>
        /// <param name="pPropiedad">Nombre de la propiedad de la cual queremos buscar el grafo del selector de entidad en caso de tenerlo</param>
        /// <param name="pEntidades">Lista de entidades de la ontología a la que pertenece la propiedad de la cual estamos buscando el grafo</param>
        /// <param name="doc">Xml de la ontología a la que pertenece la propiedad</param>
        /// <returns></returns>
        private static string ObtenerGrafoSelectorEntidad(string pPropiedad, List<ElementoOntologia> pEntidades, XmlDocument doc)
        {
            Propiedad propiedad = ObtenerPropiedadDeEntidad(pEntidades, pPropiedad);
            if (propiedad != null)
            {
                XmlNode grafo = doc.SelectSingleNode($"config/EspefPropiedad/Propiedad[@ID =\"{propiedad.Nombre}\" and @EntidadID=\"{propiedad.ElementoOntologia.TipoEntidad}\"]/SeleccionEntidad/Grafo");
                if (grafo != null)
                {
                    return grafo.InnerText;
                }
            }

            return null;
        }

        /// <summary>
        /// Analizamos la propiedad auxiliar de la ontología pasada por parametros para obtener los datos necesarios de la propiedad final
        /// Si la propiedad auxiliar tiene varios niveles concatenados con @@@ navegamos entre ellos hasta llegar a la propiedad final.
        /// </summary>
        /// <param name="pNombrePropiedad">Propiedad auxiliar de la cual queremos obtener su configuración. Si esta propiedad tiene un selector de entidad o apunta a otra entidad auxiliar estará concatenada por @@@</param>
        /// <param name="pEntidades">Lista de elementos auxiliares de la ontología a la que pertenece la propiedad</param>
        /// <param name="doc">Xml de la ontología a la que pertenece la propiedad</param>
        /// <param name="pListaDatosPropiedadOntologia">Lista de DatosPropiedadOntologia (NombrePropiedad, RdfType, Propiedad, Xml) que queremos obtener a partir del análisis de la ontología</param>
        /// <param name="pNombreOntologia">Nombre de la ontología</param>
        private bool ObtenerDatosPropiedadAuxiliar(string pNombrePropiedad, List<ElementoOntologia> pEntidades, XmlDocument doc, List<DatosPropiedadOntologia> pListaDatosPropiedadOntologia, string pNombreOntologia)
        {
            bool propiedadEncontrada = false;
            if (!pNombrePropiedad.Contains("@@@"))
            {
                Propiedad propiedad = ObtenerPropiedadDeEntidad(pEntidades, pNombrePropiedad);
                if (propiedad != null)
                {
                    pListaDatosPropiedadOntologia.Add(new DatosPropiedadOntologia() { NombrePropiedad = pNombrePropiedad, Propiedad = propiedad, RdfType = pNombreOntologia, XmlOntologia = doc });
                    propiedadEncontrada = true;
                }
            }
            else
            {
                string propiedadFinal = pNombrePropiedad.Substring(pNombrePropiedad.IndexOf("@@@") + 3);
                string propiedadActual = pNombrePropiedad.Substring(0, pNombrePropiedad.IndexOf("@@@"));

                string grafo = ObtenerGrafoSelectorEntidad(propiedadActual, pEntidades, doc);

                if (!string.IsNullOrEmpty(grafo))
                {
                    CargarDatosPropiedadOntologia(grafo, propiedadFinal, pListaDatosPropiedadOntologia);
                }
                else
                {
                    ObtenerDatosPropiedadAuxiliar(propiedadFinal, pEntidades, doc, pListaDatosPropiedadOntologia, pNombreOntologia);
                }
            }

            return propiedadEncontrada;
        }

        private byte[] ObtenerOntologia(Guid pOntologiaID)
        {
            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
            byte[] bytesArchivo = null;
            bytesArchivo = fileService.ObtenerOntologiaBytes(pOntologiaID);
            return bytesArchivo;
        }

        private byte[] ObtenerXmlOntologia(Guid pOntologiaID)
        {
            CallFileService fileService = new CallFileService(mConfigService, mLoggingService);
            byte[] bytesArchivo = fileService.ObtenerXmlOntologiaBytes(pOntologiaID);
            return bytesArchivo;
        }

        private static string ObtenerFiltroOptional(string pTipoFiltro, string pQuery, out string pUltimoFiltro, bool pMultiIdioma, Dictionary<string, string> pListaCondicionesTipoFiltro, string pLanguageCode)
        {
            pUltimoFiltro = pTipoFiltro;
            StringBuilder optional = new StringBuilder();

            if (pTipoFiltro.Contains("###ClausulaOrdenPersonalizado###"))
            {
                string clausula = pTipoFiltro.Split("###ClausulaOrdenPersonalizadoSeparador###")[1].Replace("###ClausulaOrdenPersonalizado###", "");
                return $"OPTIONAL {{ {clausula} }} ";
            }

            if (!pMultiIdioma && pListaCondicionesTipoFiltro.ContainsKey(pTipoFiltro) && pListaCondicionesTipoFiltro[pTipoFiltro].Contains("lang("))
            {
                pMultiIdioma = true;
            }

            if (pTipoFiltro.Contains("@@@"))
            {
                string[] delimiter = { "@@@" };

                optional.Append("OPTIONAL {");
                string sujeto = "?s";
                foreach (string filtro in pTipoFiltro.Split(delimiter, StringSplitOptions.RemoveEmptyEntries))
                {
                    string objeto = filtro.Replace(":", "");
                    int varAux = 0;
                    while (pQuery.Contains(objeto + varAux))
                    {
                        varAux++;
                    }
                    objeto = objeto + varAux;
                    pUltimoFiltro = objeto;
                    optional.Append($" {sujeto} {filtro} ?{objeto}. ");
                    sujeto = "?" + objeto;
                }

                if (!pMultiIdioma)
                {
                    //Quitamos los filtros con idioma para que no duplique resultados.
                    optional.Append($" FILTER(lang({sujeto}) = '' {ObtenerCondicionAdicionalTipoFiltro(pTipoFiltro, pListaCondicionesTipoFiltro, true)})");
                }
                else
                {
                    optional.Append($" FILTER(lang({sujeto}) = '{pLanguageCode}' {ObtenerCondicionAdicionalTipoFiltro(pTipoFiltro, pListaCondicionesTipoFiltro, true)})");
                }

                optional.Append(" }");
            }
            else
            {
                optional.Append($"OPTIONAL {{ ?s {pTipoFiltro} ?{QuitaPrefijo(pTipoFiltro)} . ");

                if (!pMultiIdioma)
                {
                    //Quitamos los filtros con idioma para que no duplique resultados.
                    optional.Append($" FILTER(lang(?{QuitaPrefijo(pTipoFiltro)}) = '' {ObtenerCondicionAdicionalTipoFiltro(pTipoFiltro, pListaCondicionesTipoFiltro, true)})");
                }
                else
                {
                    optional.Append($" FILTER(lang(?{QuitaPrefijo(pTipoFiltro)}) = '{pLanguageCode}' {ObtenerCondicionAdicionalTipoFiltro(pTipoFiltro, pListaCondicionesTipoFiltro, false)})");
                }

                optional.Append(" } ");
                pUltimoFiltro = QuitaPrefijo(pTipoFiltro);
            }

            return optional.ToString();
        }

        public void ObtenerResultadosBusquedaTags(string pProyectoID, FacetadoDS pFacetadoDS, List<string> pTagsDocumento, int pInicio, int pLimite, string pNamespaceExtra, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, List<string> pSemanticos, string pIdentidadID, bool pEstaEnMyGnoss, bool pEsInvitado)
        {
            string select = " select distinct ?s ?rdftype count(?Etiqueta) as ?contador ";
            string query = ObtenerFrom(pProyectoID);
            StringBuilder filtros = new StringBuilder();

            query += " WHERE { ";

            if (pListaFiltros.Keys.Contains("sioc_t:Tag"))
            {
                pListaFiltros.Remove("sioc_t:Tag");

                if (pTagsDocumento.Count > 1)
                {
                    filtros.Append(" ?s sioc_t:Tag ?Etiqueta.  FILTER ( ?Etiqueta in (");
                    foreach (string tag in pTagsDocumento)
                    {
                        filtros.Append($"{tag.Replace("\"", "'").Replace("\\", "\\\\")},");
                    }
                    filtros.Remove(filtros.Length - 1, 1);
                    filtros.Append(")");
                }
                if (pTagsDocumento.Count == 1)
                {
                    filtros.Append($" ?s sioc_t:Tag ?Etiqueta.  FILTER ( ?Etiqueta = {pTagsDocumento[0].Replace("\"", "'").Replace("\\", "\\\\")}");
                }
                filtros.Append(")");
            }

            query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, true);

            filtros.Append(ObtenerParteFiltros(pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID));
            query += filtros + " }";

            if (pLimite > 0)
            {
                query += " order by desc (?contador) LIMIT " + pLimite;
            }
            if (pInicio > 0)
            {
                query += " OFFSET " + pInicio;
            }

            query = NamespacesVirtuosoLectura + pNamespaceExtra + select + query;

            LeerDeVirtuoso(query, "RecursosBusqueda", pFacetadoDS, pProyectoID);
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pEsCatalogoNoSocial">Verdad si es un catálogo no social</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        public void ObtenerResultadosBusquedaFormatoMapa(string pProyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, DataWrapperFacetas pFiltroMapaDataWrapper, bool pPermitirRecursosPrivados, TipoBusqueda pTipoBusqueda, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, List<PresentacionMapaSemantico> pListaPresentacionMapaSemantico = null, string pLanguageCode = null)
        {
            var configMapa = pFiltroMapaDataWrapper.ListaFacetaConfigProyMapa.FirstOrDefault();
            bool puntos = false;
            bool rutas = false;
            if (configMapa != null)
            {
                puntos = !string.IsNullOrEmpty(configMapa.PropLatitud) && !string.IsNullOrEmpty(configMapa.PropLongitud);
                rutas = !string.IsNullOrEmpty(configMapa.PropRuta);
            }

            if (rutas)
            {
                FacetadoDS facDSRutas = new FacetadoDS();
                ObtenerResultadosBusquedaFormatoMapaVirtuoso(pFiltroMapaDataWrapper, pTipoBusqueda, pProyectoID, facDSRutas, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pPermitirRecursosPrivados, false, true, pEsMovil, pFiltrosSearchPersonalizados, pListaPresentacionMapaSemantico, pLanguageCode);
                pFacetadoDS.Merge(facDSRutas);
            }

            if (puntos)
            {
                FacetadoDS facDSPuntos = new FacetadoDS();

                ObtenerResultadosBusquedaFormatoMapaVirtuoso(pFiltroMapaDataWrapper, pTipoBusqueda, pProyectoID, facDSPuntos, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pPermitirRecursosPrivados, true, false, pEsMovil, pFiltrosSearchPersonalizados, pListaPresentacionMapaSemantico, pLanguageCode);

                pFacetadoDS.Merge(facDSPuntos);
            }
        }

        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pEsCatalogoNoSocial">Verdad si es un catálogo no social</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        /// <paraparam name="pPuntos">TRUE para que traiga los puntos, FALSE para que traiga las rutas</paraparam>
        public void ObtenerResultadosBusquedaFormatoMapaVirtuoso(DataWrapperFacetas pFiltroMapaDataWrapper, TipoBusqueda pTipoBusqueda, string pProyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, bool pPuntos, bool pRutas, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, List<PresentacionMapaSemantico> pListaPresentacionMapaSemantico = null, string pLanguageCode = null)
        {
            string[] filtrosContextos = null;
            string idioma = pLanguageCode;
            if (string.IsNullOrEmpty(idioma))
            {
                idioma = "es";
            }
            if (!string.IsNullOrEmpty(pFiltroContextoOrderBy))
            {
                filtrosContextos = GenerarOrderByContextos(pFiltroContextoOrderBy);
            }

            if (mListaItemsBusquedaExtra == null)
            {
                mListaItemsBusquedaExtra = pListaFiltrosExtra;
            }

            StringBuilder select = new StringBuilder(" select distinct ");

            if (!string.IsNullOrEmpty(pFiltroContextoSelect) && !string.IsNullOrEmpty(pFiltroContextoWhere))
            {
                if (pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso))
                {
                    select.Append($" ?s '{pListaFiltros["rdf:type"][0]}' as ?rdftype {pFiltroContextoSelect} as ?a ");
                }
                else
                {
                    select.Append($" ?s ?rdftype {pFiltroContextoSelect} as ?a ");
                }
            }
            else if (pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso))
            {
                string tipo = pListaFiltros["rdf:type"][0];
                select.Append($" ?s '{tipo}' as ?rdftype");
            }
            else
            {
                if (pProyectoID.Contains("contribuciones/"))
                {
                    if (!select.ToString().Contains("distinct"))
                    {
                        select.Append(" distinct ");
                    }
                    select.Append(" ?s (bif:substring(STR(?rdftype), 1, 7) AS ?rdftype) ");
                }
                else
                {
                    select.Append(" ?s ?rdftype ");
                }
            }

            StringBuilder query = new StringBuilder(ObtenerFrom(pProyectoID));

            query.Append(" WHERE { ");

            bool busquedaMensajesOComent = (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Count > 0 && (pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_MENSAJES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_COMENTARIOS || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_INVITACIONES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_SUSCRIPCIONES));

            query.Append("{");

            if (!busquedaMensajesOComent && !pProyectoID.Contains("contactos"))
            {
                query.Append(ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, pPermitirRecursosPrivados));
            }

            bool esRecomendacion = false;

            if (pProyectoID.Contains("contactos"))
            {
                query.Append(" { ");

                if (!pListaFiltros.ContainsKey("gnoss:RecPer"))
                {
                    query.Append($"?s <http://xmlns.com/foaf/0.1/knows> <http://gnoss/{pIdentidadID}>. ");
                }
                else
                {
                    query.Append($"<http://gnoss/{pIdentidadID}> <http://gnoss/RecPer>  ?s12. ?s12 <http://gnoss/RecID> ?s. ?s12 <http://gnoss/RecNum> ?s2n. FILTER (?s!=<http://gnoss/{pIdentidadID}>)  MINUS {{<http://gnoss/{pIdentidadID}> <http://gnoss/Ignora> ?s.}} ");
                    pListaFiltros.Remove("gnoss:RecPer");
                    esRecomendacion = true;
                    select = select.Replace("select", "select distinct ");
                }
            }

            StringBuilder filtros = new StringBuilder(ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, false, true, TiposAlgoritmoTransformacion.Ninguno, pFiltrosSearchPersonalizados, pEsMovil));

            if (!string.IsNullOrEmpty(pResultadosEliminar))
            {
                string[] resultados = pResultadosEliminar.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                filtros.Append(" FILTER (");
                string and = "";
                foreach (string resultado in resultados)
                {
                    filtros.Append($"{and}?s!=gnoss:{resultado.ToUpper()}");
                    and = " and ";
                }

                filtros.Append(")");
            }

            filtros.Append(" } ");

            if (esRecomendacion)
            {
                pListaFiltros.Add("gnoss:RecPer", new List<string>());
                pListaFiltros["gnoss:RecPer"].Add(pIdentidadID.ToString().ToUpper());
            }

            string filtroMapas = ObtenerFiltroConsultaMapaProyectoDesdeDataSet(pFiltroMapaDataWrapper, pTipoBusqueda, pPuntos, pRutas, null, query.ToString() + filtros.ToString());

            //Añador parte mapas:
            filtros.Append(filtroMapas);

            query.Append(filtros);

            if (pPuntos)
            {
                select.Append(" ?lat ?long ");
            }
            else
            {
                select.Append(" ?ruta ");
                if (filtroMapas.Contains("?color"))
                {
                    select.Append(" ?color ");
                }
            }

            if (pListaPresentacionMapaSemantico != null)
            {
                int contadorPropiedades = 0;
                int contadorPartes = 0;
                foreach (PresentacionMapaSemantico presentacionMapaSemantico in pListaPresentacionMapaSemantico)
                {
                    //fran
                    string nombreOntologia = mEntityContext.Documento.Where(doc => doc.DocumentoID.Equals(presentacionMapaSemantico.OntologiaID)).Select(d => d.Enlace).FirstOrDefault();
                    List<DatosPropiedadOntologia> datosPropiedadesOntologias = new List<DatosPropiedadOntologia>();
                    CargarDatosPropiedadOntologia(nombreOntologia, presentacionMapaSemantico.Propiedad, datosPropiedadesOntologias);
                    bool multiidioma = PropiedadFiltroEsMultiIdioma(datosPropiedadesOntologias, presentacionMapaSemantico.Propiedad);
                    string[] partesPropiedad = presentacionMapaSemantico.Propiedad.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    select.Append($" ?prop{contadorPropiedades}");
                    int i = 0;
                    foreach (string propiedad in partesPropiedad)
                    {
                        if (i == 0)
                        {
                            if (i < partesPropiedad.Count() - 1)
                            {
                                query.Append($"?s {propiedad} ?parte{contadorPartes + 1}. ");
                            }
                            else
                            {
                                query.Append($"?s {propiedad} ?prop{contadorPropiedades}. ");
                            }
                        }
                        else
                        {
                            if (i < partesPropiedad.Count() - 1)
                            {
                                query.Append($" ?parte{contadorPartes} {propiedad} ?parte{contadorPartes + 1} .");
                            }
                            else
                            {
                                query.Append($" ?parte{contadorPartes} {propiedad} ?prop{contadorPropiedades} .");
                            }
                        }
                        i++;
                        contadorPartes++;
                    }
                    if (multiidioma)
                    {
                        query = new StringBuilder($"{query} FILTER (lang(?prop{contadorPropiedades}) = '{idioma}') ");
                    }
                    contadorPropiedades++;
                }
            }

            if (filtrosContextos != null)
            {
                query.Append(filtrosContextos[0]);
            }

            query.Append(" } ");
            query = new StringBuilder(NamespacesVirtuosoLectura + pNamespaceExtra + select + query);

            LeerDeVirtuoso(query.ToString(), "RecursosBusqueda", pFacetadoDS, pProyectoID);
        }


        /// <summary>
        /// Obtiene los resultados de una búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Indica si el orden es descendente (false si es descendente)</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros del usuario</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda se hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        /// <param name="pListaFiltrosExtra"></param>
        /// <param name="pSemanticos"></param>
        /// <param name="pFiltroContextoSelect"></param>
        /// <param name="pFiltroContextoWhere"></param>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <param name="pEsCatalogoNoSocial">Verdad si es un catálogo no social</param>        
        /// <param name="pNamespaceExtra">NamespacesExtra</param>
        /// <param name="pResultadosEliminar"></param>
        /// <param name="pSelectChart">Select chart</param>
        /// <param name="pFiltroChart">Filtros chart</param>
        public void ObtenerResultadosBusquedaFormatoChart(string pProyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, string pSelectChart, string pFiltroChart, bool pObtenerRecursosPrivados, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados)
        {
            string[] filtrosContextos = null;
            if (!string.IsNullOrEmpty(pFiltroContextoOrderBy))
            {
                filtrosContextos = GenerarOrderByContextos(pFiltroContextoOrderBy);
            }

            if (mListaItemsBusquedaExtra == null)
            {
                mListaItemsBusquedaExtra = pListaFiltrosExtra;
            }

            string[] filtrosChart = pFiltroChart.Split(new string[] { "|||", ")))" }, StringSplitOptions.None);

            string select = " select " + pSelectChart + " ";

            string query = ObtenerFrom(pProyectoID);

            query += " WHERE { ";

            bool busquedaMensajesOComent = (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Count > 0 && (pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_MENSAJES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_COMENTARIOS || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_INVITACIONES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_SUSCRIPCIONES));

            query += "{";

            bool esRecomendacion = false;

            if (pProyectoID.Contains("contactos"))
            {
                query += " { ";

                if (!pListaFiltros.ContainsKey("gnoss:RecPer"))
                {
                    query += "?s <http://xmlns.com/foaf/0.1/knows> <http://gnoss/" + pIdentidadID + ">. ";
                }
                else
                {
                    query += "<http://gnoss/" + pIdentidadID + "> <http://gnoss/RecPer>  ?s12. ?s12 <http://gnoss/RecID> ?s. ?s12 <http://gnoss/RecNum> ?s2n. FILTER (?s!=<http://gnoss/" + pIdentidadID + ">)  MINUS {<http://gnoss/" + pIdentidadID + "> <http://gnoss/Ignora> ?s.} ";
                    pListaFiltros.Remove("gnoss:RecPer");
                    esRecomendacion = true;
                    select = select.Replace("select", "select distinct ");
                }
            }
            StringBuilder filtros = new StringBuilder(ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, false, true, TiposAlgoritmoTransformacion.Ninguno, pFiltrosSearchPersonalizados, pEsMovil));

            if (!string.IsNullOrEmpty(pResultadosEliminar))
            {
                string[] resultados = pResultadosEliminar.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                filtros.Append(" FILTER (");
                string and = "";
                foreach (string resultado in resultados)
                {
                    filtros.Append($"{and}?s!=gnoss:{resultado.ToUpper()}");
                    and = " and ";
                }

                filtros.Append(")");
            }

            filtros.Append(" } ");

            if (esRecomendacion)
            {
                pListaFiltros.Add("gnoss:RecPer", new List<string>());
                pListaFiltros["gnoss:RecPer"].Add(pIdentidadID.ToString().ToUpper());
            }

            //Añador parte where:
            filtros.Append(filtrosChart[0]);

            query += filtros;

            if (filtrosContextos != null)
            {
                query += filtrosContextos[0];
            }

            if (!busquedaMensajesOComent && !pProyectoID.Contains("contactos"))
            {
                query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, pObtenerRecursosPrivados);
            }

            query += " } ";

            if (filtrosChart.Length > 1 && !string.IsNullOrEmpty(filtrosChart[1]))
            {
                query += " group by " + filtrosChart[1];
            }

            if (filtrosChart.Length > 2 && !string.IsNullOrEmpty(filtrosChart[2]))
            {
                query += " order by " + filtrosChart[2];
            }

            if (filtrosChart.Length > 3 && !string.IsNullOrEmpty(filtrosChart[3]))
            {
                query += " limit " + filtrosChart[3];
            }

            query = NamespacesVirtuosoLectura + pNamespaceExtra + select + query;

            LeerDeVirtuoso(query, "RecursosBusqueda", pFacetadoDS, pProyectoID);
        }

        public void ObtenerResultadosBusqueda(string pQuery, string pGrafoID, FacetadoDS pFacetadoDS, int pInicio, int pLimite, string pNamespaceExtra)
        {
            pQuery += " LIMIT " + pLimite;

            if (pInicio > 0)
            {
                pQuery += " OFFSET " + pInicio;
            }
            pQuery = NamespacesVirtuosoLectura + " " + pNamespaceExtra + " " + pQuery;
            LeerDeVirtuoso(pQuery, "RecursosBusqueda", pFacetadoDS, pGrafoID);
        }

        /// <summary>
        /// Genera el order by de los contextos
        /// </summary>
        /// <param name="pFiltroContextoOrderBy"></param>
        /// <returns>Array de string con 2 valores [0]parte del filtro  [1]parte del order by</returns>
        private static string[] GenerarOrderByContextos(string pFiltroContextoOrderBy)
        {
            string[] respuesta = new string[2];
            //1 filtro
            //2 order by

            string[] filas = pFiltroContextoOrderBy.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder filtro = new StringBuilder();
            StringBuilder orderby = new StringBuilder();
            int orden = 0;
            foreach (string fila in filas)
            {
                filtro.Append("OPTIONAL { ");
                string[] stringSeparators2 = new string[] { "||" };
                string[] datos = fila.Split(stringSeparators2, StringSplitOptions.None);
                if (datos.Length == 2)
                {
                    orderby.Append($"{datos[0]} ");
                }
                else if (datos.Length == 1)
                {
                    orderby.Append("asc ");
                }
                string[] stringSeparators = new string[] { "@@@" };

                string[] LNiveles;
                if (datos.Length == 2)
                {
                    LNiveles = datos[1].Split(stringSeparators, StringSplitOptions.None);
                }
                else
                {
                    LNiveles = datos[0].Split(stringSeparators, StringSplitOptions.None);
                }

                if (LNiveles.Length > 1)
                {
                    if (LNiveles[0].StartsWith("http:"))
                    {
                        filtro.Append($"?s ?pContexto0 ?nivelContexto0 . FILTER (?pContexto0=<{LNiveles[0]}> )");
                    }
                    else
                    {
                        filtro.Append($"?s ?pContexto0 ?nivelContexto0 . FILTER (?pContexto0={LNiveles[0]} )");
                    }
                    int i = 0;
                    for (i = 0; i < LNiveles.Length - 2; i++)
                    {
                        if (LNiveles[i + 1].StartsWith("http:"))
                        {
                            filtro.Append($" ?nivelContexto{i} ?pContexto{i + 1} ?nivelContexto{i + 1}. FILTER (?pContexto{i + 1}=<{LNiveles[i + 1]}> )");
                        }
                        else
                        {
                            filtro.Append($" ?nivelContexto{i} ?pContexto{i + 1} ?nivelContexto{i + 1}. FILTER (?pContexto{(i + 1)}=${LNiveles[i + 1]} )");
                        }
                    }
                    if (LNiveles[i + 1].StartsWith("http:"))
                    {
                        filtro.Append($" ?nivelContexto{i} ?pContexto ?oContexto{orden}{i} . FILTER (?pContexto=<{LNiveles[i + 1]}> )");
                        orderby.Append($"(?oContexto{orden}{i}) ");
                    }
                    else
                    {
                        filtro.Append($" ?nivelContexto{i} ?pContexto ?oContexto{orden}{i}. FILTER (?pContexto={LNiveles[i + 1]} )");
                        orderby.Append($"(?oContexto{orden}{i}) ");
                    }

                }
                else
                {
                    if (LNiveles[0].StartsWith("http:"))
                    {
                        filtro.Append($"?s ?pContexto ?oContexto{orden} FILTER (?pContexto in (<{LNiveles[0]}>)) ");
                        orderby.Append($"(?oContexto{orden}) ");
                    }
                    else
                    {
                        filtro.Append($"?s ?pContexto ?oContexto{orden} FILTER (?pContexto in ({LNiveles[0]}) ) ");
                        orderby.Append($"(?oContexto{orden}) ");
                    }
                }

                filtro.Append(" } ");

                orden++;
            }

            respuesta[0] = filtro.ToString();
            respuesta[1] = orderby.ToString();

            return respuesta;
        }

        /// <summary>
        /// Obtiene una lista de elementos para el autocompletar
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si el usuario está buscando en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario es el usuario invitado</param>
        /// <param name="pIdentidadID">Identificador de la identidad con la que está conectado el usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de resultados</param>
        public void ObtenerAutocompletar(string pProyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltrosContexto)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            if (pEstaEnMyGnoss)
            {
                query.Append("select distinct ?Etiqueta ");
            }
            else
            {
                query.Append("select ?Etiqueta (count(?p)) as ?a ");
            }

            query.Append(ObtenerFrom(pProyectoID));

            query.Append("WHERE {");

            if (!(pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Count == 1 && pListaFiltros["rdf:type"][0] == "Mensaje"))
            {
                query.Append(ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, true));
            }
            query.Append(pFiltrosContexto);

            foreach (string Key in pListaFiltros.Keys)
            {
                string keySinPrefijo = QuitaPrefijo(Key);

                EliminarFiltrosInnecesarios(pListaFiltros, Key);

                if ((Key.Equals("autocompletar")) && pListaFiltros[Key].Count > 0)
                {
                    string tagsindescomponer = pListaFiltros["autocompletar"][0].ToString();

                    string filtro = UtilCadenas.ToSparql(tagsindescomponer).ToLower();

                    bool hayPersonasOrganizaciones = EstaBuscandoPersonaOOrganizacion(pListaFiltros);

                    if (hayPersonasOrganizaciones)
                    {

                        query.Append(" { ?s ?p ?Etiqueta. ");
                        query.Append(" FILTER ( ?p=gnoss:hasnombrecompleto and  (?Etiqueta LIKE '" + filtro + "%' or ?Etiqueta LIKE '% " + filtro + "%')  ) } UNION ");
                        query.Append(" { ?s ?p  ?Etiqueta . ");
                        query.Append(" FILTER ( ?p=sioc_t:Tag and  (?Etiqueta LIKE '" + filtro + "%' or ?Etiqueta LIKE '% " + filtro + "%')  ) } UNION ");

                        query.Append(" { ?s ?p ?EtiquetaAux. ?s gnoss:hasnombrecompleto  ?Etiqueta .   ");
                        query.Append($" FILTER ( ?p=gnoss:hasTagTituloDesc and  (?EtiquetaAux LIKE '{filtro}%' or ?EtiquetaAux LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s ?p ?EtiquetaAux. ?s gnoss:hasnombrecompleto  ?Etiqueta .  ");
                        query.Append($" FILTER ( ?p=foaf:firstName and  (?EtiquetaAux LIKE '{filtro}%' or ?EtiquetaAux LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s ?p ?EtiquetaAux.  ?s gnoss:hasnombrecompleto  ?Etiqueta . ");
                        query.Append($" FILTER ( ?p=foaf:familyName and  (?EtiquetaAux LIKE '{filtro}%' or ?EtiquetaAux LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s ?p ?EtiquetaAux. ?s gnoss:hasnombrecompleto  ?Etiqueta .   ");
                        query.Append($" FILTER ( ?p=foaf:group and  (?EtiquetaAux LIKE '{filtro}%' or ?EtiquetaAux LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s skos:ConceptID ?skosConceptID2. ?skosConceptID2 ?p ?Etiqueta. ");
                        query.Append($" FILTER ( ?p=gnoss:CategoryName and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }} UNION");
                        query.Append(" { ?s gnoss:hasCv ?CV. ?CV ?p  ?Etiqueta . ");
                        query.Append($" FILTER ( ?p=cv:OrganizationNameEmpresaActual and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s gnoss:hasCv ?CV. ?CV ?p  ?Etiqueta . ");
                        query.Append($" FILTER ( ?p=cv:PositionTitleEmpresaActual and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s gnoss:hasCv ?CV. ?CV ?p  ?Etiqueta . ");
                        query.Append($" FILTER ( ?p=cv:OrganizationUnitNameTitulaciones and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }}  UNION ");
                        query.Append(" { ?s gnoss:hasCv ?CV. ?CV ?p ?Etiqueta . ");
                        query.Append($" FILTER ( ?p=cv:PositionTitleEmpresaAnterior and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }} ");
                    }
                    else
                    {
                        query.Append(" { ?s ?p ?Etiqueta. ");
                        query.Append($" FILTER ( ?p=sioc_t:Tag and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%')  ) }} UNION ");
                        query.Append(" { ?s ?p ?Etiqueta. ");
                        query.Append($" FILTER ( ?p=gnoss:hasTagTituloDesc and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }} UNION");
                        query.Append(" { ?s skos:ConceptID ?skosConceptID2. ?skosConceptID2 ?p ?Etiqueta. ");
                        query.Append($" FILTER ( ?p=gnoss:CategoryName and  (?Etiqueta LIKE '{filtro}%' or ?Etiqueta LIKE '% {filtro}%') ) }} UNION");

                        foreach (string newsem in pSemanticos)
                        {
                            string sem = newsem;
                            string[] LNiveles = null;
                            string[] stringSeparators = new string[] { "@@@" };

                            if (newsem.Contains("@@@"))
                            {
                                LNiveles = sem.Split(stringSeparators, StringSplitOptions.None);
                                sem = LNiveles[LNiveles.Length - 1];

                                query.Append($"{{ ?s {LNiveles[0]} ?{QuitaPrefijo(LNiveles[0])}2. ");

                                int i = 0;
                                for (i = 0; i < LNiveles.Length - 2; i++)
                                {
                                    query.Append($" ?{QuitaPrefijo(LNiveles[i])}2 {LNiveles[i + 1]} ?{QuitaPrefijo(LNiveles[i + 1])}2. ");
                                }

                                query.Append($" ?{QuitaPrefijo(LNiveles[i])}2 ?p ?Etiqueta. ");
                            }
                            else
                            {
                                query.Append(" { ?s ?p ?Etiqueta. ");
                            }

                            query.Append($" FILTER ( ?p={sem} and  (bif:lower(?Etiqueta) LIKE '{filtro}%' or bif:lower(?Etiqueta) LIKE '% {filtro}%') ) }} UNION");
                        }
                        query.Remove(query.Length - 6, 6);

                    }
                }
                else if ((Key.Equals("rdf:type")) && pListaFiltros[Key].Count > 0)
                {
                    query.Append(" ?s rdf:type ?rdftype ");
                    query.Append(" FILTER (");
                    query.Append($" ?{keySinPrefijo} in (");

                    foreach (string valor in pListaFiltros[Key])
                    {
                        short tipoPropiedadFaceta = (short)TipoPropiedadFaceta.NULL;
                        if (mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(valor)))
                        {
                            tipoPropiedadFaceta = (mFacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.Faceta.Equals(valor)).TipoPropiedad.Value);
                        }

                        query.Append($"{ObtenerValorParaFiltro(Key, valor, tipoPropiedadFaceta)}, ");

                        if (valor.Equals(FacetadoAD.BUSQUEDA_CLASE))
                        {
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD, tipoPropiedadFaceta)}, ");
                        }
                        if (valor.Equals(FacetadoAD.BUSQUEDA_COMUNIDADES))
                        {
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_COMUNIDAD_NO_EDUCATIVA, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_COMUNIDAD_EDUCATIVA, tipoPropiedadFaceta)}, ");
                        }

                        if (valor.Equals(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS))
                        {
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPARTIDO, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_PUBLICADO, tipoPropiedadFaceta)}, ");
                        }

                        if (valor.Equals(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENTARIOS))
                        {
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMRECURSOS, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMDEBATES, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMDEBATES, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENCUESTAS, tipoPropiedadFaceta)}, ");
                            query.Append($"{ObtenerValorParaFiltro(Key, FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMARTICULOBLOG, tipoPropiedadFaceta)}, ");
                        }

                        if (valor.Equals(FacetadoAD.BUSQUEDA_RECURSOS))
                        {
                            foreach (string ontologia in mListaItemsBusquedaExtra)
                            {
                                if (!ontologia.Contains("@"))
                                {
                                    query.Append($"{ObtenerValorParaFiltro(Key, ontologia, tipoPropiedadFaceta)}, ");
                                }
                            }
                        }
                    }
                    query.Remove(query.Length - 2, 2);
                    query.Append(" , ' ') ) ");

                }
                else if (pListaFiltros[Key].Count > 0)
                {
                    if (Key.Contains("@@@"))
                    {
                        string aux = "autocompletar";
                        string[] LNiveles = null;
                        string[] stringSeparators = new string[] { "@@@" };
                        Dictionary<string, List<string>> filtrosUsadosFacetaActual = new Dictionary<string, List<string>>();
                        List<string> listaFiltrosUsados = new List<string>();

                        LNiveles = Key.Split(stringSeparators, StringSplitOptions.None);
                        query.Append(ObtenerParteReciprocaQuery(LNiveles, 0, query.ToString(), ref aux, filtrosUsadosFacetaActual, listaFiltrosUsados, pListaFiltros[Key][0]));
                        keySinPrefijo = aux;
                    }
                    else
                    {
                        query.Append($" ?s {Key} ?{keySinPrefijo}");
                    }
                    query.Append(" FILTER (");

                    if (pListaFiltros[Key].Count > 1)
                    {
                        query.Append($" ?{keySinPrefijo} in (");

                        string coma = "";
                        foreach (string valor in pListaFiltros[Key])
                        {
                            string valorAux = valor;
                            if (!valorAux.Contains("gnoss:"))
                            {
                                if (Key.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
                                {
                                    valorAux = ObtenerRealSubType(valor);
                                }
                                valorAux = $"'{valorAux.Replace("'", "''")}'";
                            }

                            query.Append(coma + valorAux);
                            coma = ", ";
                        }

                        query.Append(")");
                    }
                    else
                    {
                        string valor = pListaFiltros[Key][0];
                        if (!valor.Contains("gnoss:"))
                        {
                            if (Key.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
                            {
                                valor = ObtenerRealSubType(valor);
                            }
                            valor = "'" + valor.Replace("'", "''") + "'";
                        }
                        query.Append($" ?{keySinPrefijo} = {valor}");
                    }

                    query.Append(")");
                }
            }

            //para obtener el typo de recurso en mygnoss o buscador metacomunidad
            if (!query.ToString().Contains("?s rdf:type ?rdftype"))
            {
                if (pProyectoID.Equals(ProyectoAD.MetaProyecto.ToString()))
                {
                    query.Append(TIPOS_METABUSCADOR_MYGNOSS);
                }
                else if (!pEsMiembroComunidad && !pProyectoID.Equals(ProyectoAD.MetaProyecto.ToString()))
                {
                    query.Append(TIPOS_METABUSCADOR_USUARIO_INVITADO);
                }
                else
                {
                    query.Append(TIPOS_METABUSCADOR);
                }

                query.Remove(query.Length - 4, 4);

                foreach (string ontologia in mListaItemsBusquedaExtra)
                {
                    if (!ontologia.Contains("@"))
                    {
                        query.Append($", '{ontologia}' ");
                    }
                }

                query.Append("))");
            }

            query.Append(" } ");

            //order by
            if (pEstaEnMyGnoss)
            {
                query.Append(" ");
            }
            else
            {
                query.Append("order by desc (?a) asc (?foaffirstname) ");
            }
            query.Append("LIMIT {pLimite}");

            if (pInicio > 0)
            {
                query.Append($" OFFSET {pInicio}");
            }

            LeerDeVirtuoso(query.ToString(), "Autocompletar", pFacetadoDS, pProyectoID);
        }

        /// <summary>
        /// Obtiene los mensajes relacionados de un mensaje dado
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pMensajeID">Identificador del mensaje</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pLimite">Límite de los resultados</param>
        /// <param name="pNombreUsuarioActual">Nombre del usuario a descartar de los tags</param>
        public FacetadoDS ObtenerMensajesRelacionados(string pUsuarioID, string pMensajeID, string pIdentidadID, int pLimite, string pNombreUsuarioActual)
        {
            char[] separador = { ' ' };
            pNombreUsuarioActual = pNombreUsuarioActual.ToLower();
            string[] nombreDescompuesto = pNombreUsuarioActual.Split(separador);

            string filtro = "?Tag != '" + pNombreUsuarioActual + "'";

            StringBuilder filtroDesc = new StringBuilder();
            int i = 0;
            while (i < nombreDescompuesto.Length)
            {
                filtroDesc.Append($"?TagDesc != '{nombreDescompuesto[i]}'");
                i++;
                if (i < nombreDescompuesto.Length)
                {
                    filtroDesc.Append(" and ");
                }
            }
            filtroDesc.Append($" and ?TagDesc != '{pNombreUsuarioActual}'");

            string query = NamespacesVirtuosoLectura;
            query += "select distinct ?s (4 * count(distinct ?Tag) + 2 * count(distinct ?TagDesc) + count(distinct ?remitente)) as ?a   ";

            query += ObtenerFrom(pUsuarioID);

            query += "WHERE {";
            query += " ?s gnoss:IdentidadID gnoss:" + pIdentidadID.ToUpper() + ". ?s rdf:type 'Mensaje'. ?s nmo:sentDate ?sentDate. ?s gnoss:hasConversacion ?hasConversacion. gnoss:" + pMensajeID.ToUpper() + " gnoss:hasConversacion ?hasConversacionOriginal. FILTER (?s!=gnoss:" + pMensajeID.ToUpper() + " and ?hasConversacion != ?hasConversacionOriginal) ";

            query += " { gnoss:" + pMensajeID.ToUpper() + " sioc:Tag ?Tag.  ?s sioc:Tag ?Tag. FILTER(" + filtro + ") } UNION ";
            query += " { gnoss:" + pMensajeID.ToUpper() + " gnoss:hasTagDesc ?TagDesc.  ?s gnoss:hasTagDesc ?TagDesc. FILTER(" + filtroDesc + ") } UNION ";

            query += " { gnoss:" + pMensajeID.ToUpper() + " nmo:from ?remitente.  ?s nmo:from ?remitente } ";

            query += " } ";

            //order by
            query += "order by desc (?a) desc (?sentDate) ";
            query += "LIMIT " + pLimite;

            FacetadoDS facetadoDS = new FacetadoDS();

            LeerDeVirtuoso(query, "MensajesRelacionados", facetadoDS, pUsuarioID);

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene las personas recomendadas para otra
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que buscamos personas relacionadas</param>
        /// <param name="pIdentidadID">Identificador de la identidad de la que buscamos personas relacionadas</param>
        /// <param name="pDatosOpcionesExtraRegistro">Datos extra del registro de la persona. El diccionario esta formado de la siguiente manera: predicado -> Lista de Opciones con ese predicado</param>
        /// <param name="pListaCategoriasSuscrita">Lista de categorías a las que está suscrita la persona en esta comunidad</param>
        /// <param name="pLocalidad">Localidad de la persona</param>
        /// <param name="pPais">País de la persona</param>
        /// <param name="pProvincia">Provincia de la persona</param>
        /// <param name="pNumeroPersonas">Numnero de personas</param>
        public FacetadoDS ObtenerPersonasRecomendadas(Guid pProyectoID, Guid pIdentidadID, string pLocalidad, string pProvincia, string pPais, Dictionary<string, string> pDatosOpcionesExtraRegistro, List<Guid> pListaCategoriasSuscrita, int pNumeroPersonas)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            query.Append("SELECT DISTINCT ?s ");
            query.Append(ObtenerFrom(pProyectoID.ToString().ToLower()));
            query.Append(" WHERE {?s rdf:type 'Persona'. ");

            string orderByLocalidad = null;
            string orderByProvincia = null;
            string orderByPais = null;
            //Localidad
            if (!string.IsNullOrEmpty(pLocalidad))
            {
                query.Append($" OPTIONAL {{?s <http://www.w3.org/2006/vcard/ns#locality> ?locality. FILTER(?locality='{pLocalidad.ToLower().Replace("'", "''")} '). bind(1 as ?contadorLocalidad)}} ");
                orderByLocalidad = "4 * COUNT(?contadorLocalidad) + ";
            }

            //Provincia
            if (!string.IsNullOrEmpty(pProvincia))
            {
                query.Append($" OPTIONAL {{?s <http://d.opencalais.com/1/type/er/Geo/ProvinceOrState> ?ProvinceOrState. FILTER(?ProvinceOrState='{pProvincia.ToLower().Replace("'", "''")}'). bind(1 as ?contadorProvincia)}} ");
                orderByProvincia = "3 * COUNT(?contadorProvincia) + ";
            }

            //País
            if (!string.IsNullOrEmpty(pPais))
            {
                query.Append($" OPTIONAL {{?s <http://www.w3.org/2006/vcard/ns#country-name> ?country. FILTER(?country='{pPais.ToLower().Replace("'", "''")}'). bind(1 as ?contadorPais)}} ");
                orderByPais = "3 * COUNT(?contadorPais) + ";
            }

            //A partir de 10 items, la búsqueda funciona muy lenta
            if (pDatosOpcionesExtraRegistro != null && pDatosOpcionesExtraRegistro.Count > 0 && pDatosOpcionesExtraRegistro.Count < 10)
            {
                int contador = 0;
                foreach (string predicado in pDatosOpcionesExtraRegistro.Keys)
                {
                    if (pDatosOpcionesExtraRegistro.Count > 0)
                    {
                        string parametro = "?datoExtra" + contador++;
                        query.Append($" OPTIONAL {{?s <{predicado}> {parametro} . FILTER({parametro}");
                        query.Append($" = '{pDatosOpcionesExtraRegistro[predicado].ToString().ToLower()}'");
                        query.Append(")} ");
                    }
                }
            }

            string orderByInterest = null;

            //A partir de 10 items, la búsqueda funciona muy lenta
            if (pListaCategoriasSuscrita != null && pListaCategoriasSuscrita.Count > 0 && pListaCategoriasSuscrita.Count < 10)
            {
                query.Append(" OPTIONAL {?s <http://xmlns.com/foaf/0.1/interest> ?interest . FILTER(?interest");
                if (pListaCategoriasSuscrita.Count == 1)
                {
                    query.Append($" = <{pListaCategoriasSuscrita[0].ToString().ToUpper()}>");
                }
                else
                {
                    query.Append(" IN (");
                    string coma = "";
                    foreach (Guid categoria in pListaCategoriasSuscrita)
                    {
                        query.Append($"{coma}<{categoria.ToString().ToUpper()}>");
                        coma = ", ";
                    }
                    query.Append(") ");
                }
                query.Append("). bind(1 as ?contadorInterest)} ");

                orderByInterest = "4 * COUNT(?contadorInterest) + ";
            }

            query.Append($" OPTIONAL {{?s <http://xmlns.com/foaf/0.1/knows> ?knows. ?knows <http://xmlns.com/foaf/0.1/knows> ?knows2. bind(1 as ?contadorKnows) }} MINUS {{<http://gnoss/{pIdentidadID.ToString().ToUpper()}> <http://xmlns.com/foaf/0.1/knows> ?s.}} ");
            query.Append(" } ");

            //order by
            // Por cada 200 amigos contamos un punto.
            query.Append($"ORDER BY DESC ({orderByLocalidad}{orderByProvincia}{orderByPais}{orderByInterest}0.005 * COUNT(?contadorKnows )) ");

            // Pongo 500 a pelo porque el minus no funciona en virtuoso 7, 
            query.Append($" LIMIT {pNumeroPersonas}");

            FacetadoDS facetadoDS = new FacetadoDS();

            LeerDeVirtuoso(query.ToString(), "PersonasRelacionadas", facetadoDS, pProyectoID.ToString());

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene las personas recomendadas para otra
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pIdentidadID"></param>
        /// <param name="pSparql"></param>
        /// <param name="pNumeroPersonas"></param>
        public FacetadoDS ObtenerPersonasRecomendadas(Guid pProyectoID, Guid pIdentidadID, Dictionary<string, float> pListaPropiedadesPeso, Dictionary<string, Object> pValorPropiedades, int pNumeroPersonas)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            query.Append("SELECT DISTINCT ?s ");
            query.Append(ObtenerFrom(pProyectoID.ToString().ToLower()));
            query.Append(" WHERE {?s rdf:type 'Persona'. ");

            StringBuilder orderBy = new StringBuilder(" ORDER BY DESC ( ");
            string suma = "";

            int numeroPropiedad = 0;
            foreach (string propiedad in pListaPropiedadesPeso.Keys)
            {
                numeroPropiedad++;
                string nombrePropiedad = "propiedad" + numeroPropiedad;

                if (pValorPropiedades.ContainsKey(propiedad))
                {
                    Object valorPropiedad = pValorPropiedades[propiedad];

                    switch (propiedad)
                    {
                        case "http://xmlns.com/foaf/0.1/interest":
                            List<Guid> listadoIntereses = (List<Guid>)valorPropiedad;
                            if (listadoIntereses.Count > 0)
                            {
                                query.Append($" OPTIONAL {{?s <{propiedad}> ?{nombrePropiedad} . FILTER(?{nombrePropiedad}");
                                query.Append(" IN (");
                                string coma = "";
                                foreach (Guid interes in listadoIntereses)
                                {
                                    query.Append($"{coma}<{interes.ToString().ToUpper()}>");
                                    coma = ", ";
                                }
                                query.Append(") ");
                                query.Append($"). bind(1 as ?contador{nombrePropiedad})}} ");
                            }
                            break;
                        default:
                            string valorString = (string)valorPropiedad;
                            query.Append($" OPTIONAL {{?s <{propiedad}> ?{nombrePropiedad}. FILTER(?{nombrePropiedad}='{valorString.ToString().ToLower().Replace("'", "''")}'). bind(1 as ?contador{nombrePropiedad})}} ");
                            break;
                    }

                    orderBy.Append($"{suma} ({pListaPropiedadesPeso[propiedad]} * COUNT(?contador{nombrePropiedad})) ");
                    suma = "+";
                }
            }

            orderBy.Append(") ");
            query.Append(" } ");
            query.Append(orderBy);
            query.Append(" LIMIT 500");

            FacetadoDS facetadoDS = new FacetadoDS();

            LeerDeVirtuoso(query.ToString(), "PersonasRelacionadas", facetadoDS, pProyectoID.ToString());

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene los recursos relacionados de un recurso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionados(string pProyectoID, string pRecursoID, FacetadoDS pFacetadoDS, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            query.Append("select distinct ?s (count(distinct ?Tag)+ 2* count(distinct ?ConceptID)) as ?a   ");
            query.Append(ObtenerFrom(pProyectoID));
            query.Append("WHERE {");

            query.Append(" ?s gnoss:hasprivacidadCom 'publico'. ");

            if (pEsMiembroComunidad)
            {
                query.Append(" UNION { ?s gnoss:hasprivacidadCom 'publicoreg'. } ");
            }

            query.Append($" {{ gnoss:{pRecursoID.ToUpper()} sioc_t:Tag ?Tag. ?s sioc_t:Tag ?Tag. }} UNION ");
            query.Append($" {{ gnoss:{pRecursoID.ToUpper()} skos:ConceptID ?ConceptID. ?s skos:ConceptID ?ConceptID. }} ");
            query.Append(" ?s rdf:type ?rdftype FILTER (?rdftype in ('Recurso', ");

            if (InformacionOntologias.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> namespacesOntologia in InformacionOntologias.Where(item => !item.Key.Contains("@")))
                {
                    query.Append(ObtenerValorParaFiltro("rdf:type", namespacesOntologia.Key, 0) + ", ");
                }
            }
            else
            {
                query.Append("'', ");
            }

            query.Remove(query.Length - 2, 2);
            query.Append(") ) ");
            query.Append(" OPTIONAL{ ?s gnoss:hasPopularidad ?gnosshasPopularidad.} ");
            query.Append(" FILTER (?s!=gnoss:" + pRecursoID.ToUpper() + ") ");
            query.Append(" } ");
            query.Append("order by desc (?a) desc (?gnosshasPopularidad) ");
            query.Append("LIMIT " + pLimite);

            if (pInicio > 0)
            {
                query.Append($" OFFSET {pInicio}");
            }

            LeerDeVirtuoso(query.ToString(), "RecurosRelacionados", pFacetadoDS, pProyectoID);
        }

        /// <summary>
        /// Obtiene los recursos relacionados de un recurso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionadosNuevo(string pProyectoID, string pRecursoID, FacetadoDS pFacetadoDS, int pInicio, int pLimite, string pTags, string pConceptID, bool pEsCatalogoNoSocial, string pPestanyaRecurso)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            query.Append("select distinct ?s  ");

            if (pEsCatalogoNoSocial)
            {
                query.Append("count(distinct ?Tag)");
            }
            else
            {
                query.Append("(count(distinct ?Tag)+ 2* count(distinct ?ConceptID))");
            }

            query.Append(" as ?a ");
            query.Append(ObtenerFrom(pProyectoID));
            query.Append("WHERE {");

            if (!pEsCatalogoNoSocial)
            {
                Dictionary<string, List<string>> L = new Dictionary<string, List<string>>();

                query.Append(" ?s gnoss:hasprivacidadCom 'publico'. ");
                query.Append(" { ");

                if (pPestanyaRecurso != "" && (pPestanyaRecurso.ToLower().Contains("rdf:type=") || pPestanyaRecurso.ToLower().Contains("gnoss:hastipodoc=")))
                {
                    bool conType = false;
                    if (pPestanyaRecurso.ToLower().Contains("rdf:type="))
                    {
                        query.Append("{ ?s rdf:type ?rdftype FILTER (?rdftype in (");
                        string[] tipos = pPestanyaRecurso.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tipo in tipos.Where(item => item.StartsWith("rdf:type=")))
                        {
                            query.Append($"'{tipo.Substring("rdf:type=".Length)}', ");
                        }
                        query.Remove(query.Length - 2, 2);
                        query.Append(") ) }");
                        conType = true;
                    }
                    if (pPestanyaRecurso.ToLower().Contains("gnoss:hastipodoc="))
                    {
                        if (conType)
                        {
                            query.Append(" UNION ");
                        }
                        query.Append("{ ?s gnoss:hastipodoc ?gnosshastipodoc FILTER (?gnosshastipodoc in (");
                        string[] tipos = pPestanyaRecurso.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tipo in tipos.Where(item => item.StartsWith("gnoss:hastipodoc=")))
                        {
                            query.Append($"'{tipo.Substring("gnoss:hastipodoc=".Length)}', ");
                        }
                        query.Remove(query.Length - 2, 2);
                        query.Append(") ) }");
                    }
                }
                else
                {
                    query.Append(" ?s rdf:type ?rdftype FILTER (?rdftype in (");
                    query.Append("'Recurso', ");
                    if (InformacionOntologias.Count != 0)
                    {
                        foreach (KeyValuePair<string, List<string>> ontologia in InformacionOntologias.Where(item => !item.Key.Contains("@")))
                        {
                            query.Append($"{ObtenerValorParaFiltro("rdf:type", ontologia.Key, 0)}, ");
                        }
                    }
                    query.Remove(query.Length - 2, 2);
                    query.Append(") ) ");
                }
            }

            if (!string.IsNullOrEmpty(pTags))
            {
                query.Append($" ?s sioc_t:Tag ?Tag. FILTER(?Tag in ({pTags})) ");
            }

            if (!pEsCatalogoNoSocial)
            {
                query.Append(" } UNION {");

                if (pPestanyaRecurso != "" && (pPestanyaRecurso.ToLower().Contains("rdf:type=") || pPestanyaRecurso.ToLower().Contains("gnoss:hastipodoc=")))
                {
                    bool conType = false;
                    if (pPestanyaRecurso.ToLower().Contains("rdf:type="))
                    {
                        query.Append("{ ?s rdf:type ?rdftype FILTER (?rdftype in (");
                        string[] tipos = pPestanyaRecurso.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tipo in tipos.Where(item => item.StartsWith("rdf:type=")))
                        {
                            query.Append($"'{tipo.Substring("rdf:type=".Length)}', ");
                        }
                        query.Remove(query.Length - 2, 2);
                        query.Append(") ) }");
                        conType = true;
                    }
                    if (pPestanyaRecurso.ToLower().Contains("gnoss:hastipodoc="))
                    {
                        if (conType)
                        {
                            query.Append(" UNION ");
                        }
                        query.Append("{ ?s gnoss:hastipodoc ?gnosshastipodoc FILTER (?gnosshastipodoc in (");
                        string[] tipos = pPestanyaRecurso.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tipo in tipos.Where(item => item.StartsWith("gnoss:hastipodoc=")))
                        {
                            query.Append($"'{tipo.Substring("gnoss:hastipodoc=".Length)}', ");
                        }
                        query.Remove(query.Length - 2, 2);
                        query.Append(") ) }");
                    }
                }
                else
                {
                    query.Append(" ?s rdf:type ?rdftype FILTER (?rdftype in (");
                    query.Append("'Recurso', ");
                    if (InformacionOntologias.Count != 0)
                    {
                        foreach (KeyValuePair<string, List<string>> ontologia in InformacionOntologias.Where(item => !item.Key.Contains("@")))
                        {
                            query.Append($"{ObtenerValorParaFiltro("rdf:type", ontologia.Key, 0)}, ");
                        }
                    }
                    query.Remove(query.Length - 2, 2);
                    query.Append(") ) ");
                }

                if (!string.IsNullOrEmpty(pConceptID))
                {
                    query.Append($" ?s skos:ConceptID ?ConceptID. FILTER(?ConceptID in ({pConceptID})) }} ");
                }
                else
                {
                    query.Append(" } ");
                }
            }

            query.Append(" OPTIONAL{ ?s gnoss:hasPopularidad ?gnosshasPopularidad.} ");
            query.Append($" FILTER (?s!=gnoss:{pRecursoID.ToUpper()}) ");
            query.Append(" } ");
            query.Append("order by desc (?a) desc (?gnosshasPopularidad) ");
            query.Append($"LIMIT {pLimite}");

            if (pInicio > 0)
            {
                query.Append($" OFFSET {pInicio}");
            }

            LeerDeVirtuoso(query.ToString(), "RecurosRelacionados", pFacetadoDS, pProyectoID);
        }

        /// <summary>
        /// Obtiene los recursos relacionados de un recurso (ordenados de más relacionado a menos relacionado)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto fuente (donde esta el recurso del cual buscamos relacionados) En este caso la comundad de obras de arte</param>
        /// <param name="pProyectoID">Identificador del proyecto destino (donde estan los recursos que queremos relacionar.En este caso didactalia</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionadosOtroProyecto(string pProyectoIDFuente, string pProyectoIDDestino, string pRecursoID, FacetadoDS pFacetadoDS, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            query.Append(" select ?s (count(?o) + 5 * count (?o2)) as ?cantidad ");
            query.Append("WHERE {");
            query.Append(ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoIDFuente, true));
            query.Append($" {{ GRAPH {ObtenerFrom(pProyectoIDFuente).Replace("from", "")}  {{  ?s ?p ?o. ");
            query.Append("?s skos:ConceptID ?skosConceptID. FILTER (?skosConceptID in (gnoss:0AB227A0-FDDE-46CD-8C44-7FB4739E2D5E, gnoss:3A05B9C7-6DFE-4F13-B7FD-226C3C8B3E7C))");
            query.Append(" FILTER (?p=sioc_t:Tag). ?s rdf:type ?rdftype2 FILTER (?rdftype2 in ('Recurso')) } ");
            query.Append($" GRAPH {ObtenerFrom(pProyectoIDDestino).Replace("from", "")}  {{  ?s2 ?p2 ?o FILTER (?p2 in (sioc_t:Tag, obrasArte:Epoca, obrasArte:Objeto, obrasArte:Tecnica)) }} }} UNION ");
            query.Append($" {{ GRAPH {ObtenerFrom(pProyectoIDFuente).Replace("from", "")} {{  ?s ?p ?o2. ");
            query.Append("?s  skos:ConceptID ?skosConceptID. FILTER (?skosConceptID in (gnoss:0AB227A0-FDDE-46CD-8C44-7FB4739E2D5E, gnoss:3A05B9C7-6DFE-4F13-B7FD-226C3C8B3E7C))");
            query.Append(" FILTER (?p=sioc_t:Tag). ?s rdf:type ?rdftype2 FILTER (?rdftype2 in ('Recurso')) } ");
            query.Append($" GRAPH {ObtenerFrom(pProyectoIDDestino).Replace("from", "")} {{ ?s2 ?p2 ?o2 FILTER (?p2 in (obrasArte:AutorBUSCAR, obrasArte:Estilo)) }} }} ");
            query.Append(" ?s2 rdf:type ?rdftype FILTER (?rdftype in ('Recurso', ");

            if (InformacionOntologias.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> ontologia in InformacionOntologias.Where(item => !item.Key.Contains("@")))
                {
                    query.Append($"{ObtenerValorParaFiltro("rdf:type", ontologia.Key, 0)}, ");
                }
            }
            else
            {
                query.Append("'', ");
            }

            query.Remove(query.Length - 2, 2);
            query.Append(") ) ");
            query.Append($" FILTER (?s2=gnoss:{pRecursoID.ToUpper()}) ");
            query.Append("  }");
            query.Append(" order by desc (?cantidad)");
            query.Append($"LIMIT {pLimite}");

            if (pInicio > 0)
            {
                query.Append($" OFFSET {pInicio}");
            }

            LeerDeVirtuoso(query.ToString(), "RecurosRelacionados", pFacetadoDS, pProyectoIDDestino);
        }

        /// <summary>
        /// Obtiene obras de arte de museos de España
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto fuente (donde esta el recurso del cual buscamos relacionados) En este caso la comunidad museos de españa. pruebas: ac741df7-85a0-44bd-86cf-67904d2bb3c2. produccion: 54d10e57-b6d0-47dc-83a8-b1ce7e7936ca</param>
        /// <param name="pProyectoID">Identificador del proyecto destino (donde estan los recursos que queremos relacionar.En este caso obras de arte. Pruebas: cd7ccf45-5905-414a-b515-6baada847eea. Producción:b972490e-4ec8-4c7a-ada4-f2182a36b579</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionadosComunidad1Comunidad2(string pProyectoIDFuente, string pProyectoIDDestino, string pRecursoID, FacetadoDS pFacetadoDS, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, string select, string filtros)
        {
            string[] grupos = null;
            string[] stringSeparators = new string[] { "||" };

            grupos = filtros.Split(stringSeparators, StringSplitOptions.None);

            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            query.Append($"select distinct ?s {select} as ?cantidad ");
            query.Append("WHERE {");

            Dictionary<string, List<string>> L = new Dictionary<string, List<string>>();
            query.Append(ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoIDFuente, false));

            int i = 0;
            for (i = 0; i < grupos.Length - 1; i++)
            {
                string filtro = grupos[i];
                string[] fuentedestino = null;
                stringSeparators = new string[] { "|" };

                fuentedestino = filtro.Split(stringSeparators, StringSplitOptions.None);

                query.Append(" { ");
                query.Append("GRAPH " + ObtenerFrom(pProyectoIDFuente).Replace("from", "") + "  {  ");
                query.Append(fuentedestino[0]);
                query.Append("  } ");
                query.Append(" GRAPH " + ObtenerFrom(pProyectoIDDestino).Replace("from", "") + "  {  ");
                query.Append(fuentedestino[1]);
                query.Append(" } ");
                query.Append("} ");
                query.Append(" UNION ");
            }

            query.Remove(query.Length - 6, 6);
            query.Append($" FILTER (?s2=gnoss:{pRecursoID.ToUpper()}) ");
            query.Append("  }");

            //order by
            query.Append("  order by desc (?cantidad)");
            query.Append($"LIMIT {pLimite}");

            if (pInicio > 0)
            {
                query.Append($" OFFSET {pInicio}");
            }

            LeerDeVirtuoso(query.ToString(), "RecurosRelacionados", pFacetadoDS, pProyectoIDDestino);
        }

        /// <summary>
        /// Obtiene las guias de viaje cuya distancia a una casa rural sea mínima.
        /// </summary>

        /// <param name="pProyectoID">Identificador del proyecto destino (donde estan los recursos que queremos relacionar.En este caso una comunidad de Guias de Viaje</param>   
        /// <param name="pProyectoID">Identificador del proyecto fuente (donde esta el recurso del cual buscamos relacionados) En este la comunidad de alojamientos Pruebas:39526b65-7116-44b7-aebf-f28acea6a017</param>
        /// <param name="pProyectoID">Identificador del proyecto destino (donde estan los recursos que queremos relacionar.En este caso una comunidad de Guias de viaje  Pruebas:df911444-103f-4108-a3ff-faaf8db18339. Producción:68ee3734-db94-4a15-a782-f9de42e83ec3</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pEstaEnMyGnoss">Verdad si la búsqueda si hace en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Límite de los resultados</param>
        public void ObtenerRecursosRelacionadosPorDistancia(string pProyectoIDDestino, float pLatitud, float pLongitud, FacetadoDS pFacetadoDS, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite)
        {
            string query = NamespacesVirtuosoLectura;
            query += " select distinct ?a ((" + pLatitud.ToString().Replace(",", ".") + " - ?lat) * (" + pLatitud.ToString().Replace(",", ".") + "- ?lat ) + (" + pLongitud.ToString().Replace(",", ".") + " - ?long ) * (" + pLongitud.ToString().Replace(",", ".") + " - ?long))  as ?distancia      ";

            query += ObtenerFrom(pProyectoIDDestino);

            query += "WHERE {";
            query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoIDDestino, false);

            query += " {  ?a ?b ?s. ?s ?p ?long .  ?s ?p2 ?lat .FILTER(?p=<http://www.w3.org/2003/01/geo/wgs84_pos#long>  and ?p2=<http://www.w3.org/2003/01/geo/wgs84_pos#lat>)  FILTER (?lat>" + (pLatitud - 0.5).ToString().Replace(",", ".") + " and ?lat<" + (pLatitud + 0.5).ToString().Replace(",", ".") + " and ?long>" + (pLongitud - 0.5).ToString().Replace(",", ".") + " and ?long<" + (pLongitud + 0.5).ToString().Replace(",", ".") + ")   }";

            //order by
            query += "  order by (?distancia) ";
            query += "LIMIT " + pLimite;

            if (pInicio > 0)
            {
                query += " OFFSET " + pInicio;
            }

            LeerDeVirtuoso(query, "RecurosRelacionados", pFacetadoDS, pProyectoIDDestino);
        }

        /// <summary>
        /// Obtiene los recursos de una comunidad que cumplan una determinada regla de compartición y unas reglas de mapeo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto destino</param>
        /// <param name="pRegla">Regla de compartición</param>
        /// <param name="pListaCategoriasMapping">Lista de identificadores de categorias</param>
        /// <param name="pDocumentoID">Identificador del documento</param>
        /// <returns></returns>
        public bool CumpleReglaRecurso(Guid pProyectoID, string pRegla, string pReglaMapping, Guid pDocumentoID)
        {
            string consulta = string.Empty;

            string tipoRegla = pRegla.Substring(0, pRegla.IndexOf('=')).Trim();
            string rdfType = pRegla.Substring(pRegla.IndexOf('=') + 1).Trim();

            switch (tipoRegla)
            {
                case "rdf:type":
                    if (pReglaMapping.ToLower().Equals("ninguna") || pReglaMapping.ToLower().Equals("todas"))
                    {
                        consulta = "sparql ASK " + ObtenerFrom(pProyectoID.ToString()) + " where {?s rdf:type '" + rdfType + "'. FILTER(?s = <http://gnoss/" + pDocumentoID.ToString().ToUpper() + ">)}";
                    }
                    else
                    {
                        consulta = "sparql ASK " + ObtenerFrom(pProyectoID.ToString()) + " where {?s rdf:type '" + rdfType + "'. ?s <http://www.w3.org/2004/02/skos/core#ConceptID> ?categoriaID.  FILTER(?s = <http://gnoss/" + pDocumentoID.ToString().ToUpper() + "> and ?categoriaID = <http://gnoss/" + pReglaMapping.ToUpper() + "> )}";
                    }
                    break;
            }

            FacetadoDS facetadoDS = new FacetadoDS();
            LeerDeVirtuoso(consulta, "Regla", facetadoDS, pProyectoID.ToString());

            if (facetadoDS.Tables.Contains("Regla") && (facetadoDS.Tables["Regla"].Rows.Count > 0))
            {
                object cumpleRegla = facetadoDS.Tables["Regla"].Rows[0][0];
                int resultado;
                int.TryParse(cumpleRegla.ToString(), out resultado);

                return resultado == 1;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pListaDocumentos">Lista de documentos que se quieren cargar</param>
        public void ObtieneInformacionRecursosCatalogoMuseos(Guid pProyectoID, FacetadoDS pFacetadoDS, List<Guid> pListaDocumentos)
        {
            StringBuilder filtroRecursos = new StringBuilder();

            if (pListaDocumentos != null && pListaDocumentos.Count > 0)
            {
                filtroRecursos.Append("FILTER (?s in (");
                string coma = "";
                foreach (Guid docID in pListaDocumentos)
                {
                    filtroRecursos.Append($"{coma}<http://gnoss/{docID.ToString().ToUpper()}>");
                    coma = ", ";
                }
                filtroRecursos.Append("))");
            }

            string query = NamespacesVirtuosoLectura;
            if (!query.Contains("obrasArte"))
            {
                query += "prefix obrasArte:<" + mUrlIntranet + "Ontologia/ontologiaobradearte.owl#>";
            }

            query += " select distinct  ?s   ?url ?autor ?museo ";
            query += ObtenerFrom(pProyectoID.ToString());
            query += " WHERE {  ";
            query += "  {  ?s ?p ?o. OPTIONAL {?s obrasArte:Imagen ?url.}} " + filtroRecursos;
            query += "OPTIONAL{ ?s obrasArte:Autor ?autor  } ";
            query += " OPTIONAL {?s obrasArte:Localizaciones ?obrasArteLocalizaciones. ?obrasArteLocalizaciones  obrasArte:Museo ?museo} ";
            query += " }";

            LeerDeVirtuoso(query, "DatosObraArte", pFacetadoDS, pProyectoID.ToString());
        }


        /// <summary>
        /// Obtiene las personas para una determinada búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pDescendente">Verdad si el orden es descendente</param>
        /// <param name="pFacetadoDS">Data set de facetado</param>
        /// <param name="pTipoFiltro">Tipo de filtro</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si el usuario está haciendo la búsqueda en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pInicio">Inicio de los resultados</param>
        /// <param name="pLimite">Fin de los resultados</param>
        public void ObtenerPersonas(string pProyectoID, bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos)
        {
            string orden = "";

            this.mConsultaResultados = true;

            string select = "select distinct ?s   ?rdftype " + orden + "   "; //sum(distinct ?posicion) as ?a

            bool hayFiltroSearch = pListaFiltros.ContainsKey("search");

            string query = ObtenerFrom(pProyectoID);

            query += "WHERE { ";

            query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, true);

            query += ObtenerParteFiltros(pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID);

            if (!query.Contains("?s " + pTipoFiltro + " ?" + QuitaPrefijo(pTipoFiltro) + " ") && !(QuitaPrefijo(pTipoFiltro).Contains("relevancia")))
            {
                query += " OPTIONAL { ?s " + pTipoFiltro + " ?" + QuitaPrefijo(pTipoFiltro) + " . ";

                if ((QuitaPrefijo(pTipoFiltro).Contains("hasPopularidad")))
                {
                    query += " ?s gnoss:hasnumerorecursos ?gnosshasnumerorecursos. ";
                }
                query += "}";
            }
            query += " } ";

            string orderBy = " order by ";

            if (hayFiltroSearch && !mConsultaNumeroResultados)
            {
                orderBy += " desc(sum(?scoreSearch) + sum(?scoreTitle)) ";
            }

            if (!hayFiltroSearch && QuitaPrefijo(pTipoFiltro).Contains("relevancia"))
            {
                orderBy += " desc (?posicion) desc (?gnosshasfechapublicacion) desc (?gnosshaspopularidad) ";
            }
            else
            {
                if (pDescendente)
                {
                    orderBy += "desc ";
                }

                if (mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(pTipoFiltro)))
                {
                    if (((TipoPropiedadFaceta)(mFacetaDW.ListaFacetaObjetoConocimientoProyecto.FirstOrDefault(item => item.Faceta.Equals(pTipoFiltro))).TipoPropiedad.Value).Equals(TipoPropiedadFaceta.Texto))

                    {
                        orderBy += $"(bif:rdf_collation_order_string('DB.DBA.LEXICAL_ACUTE', lcase(?{QuitaPrefijo(pTipoFiltro)})))  desc (?gnosshasnumerorecursos)   ";
                    }
                    else
                    {
                        orderBy += " (?" + QuitaPrefijo(pTipoFiltro) + ") desc (?gnosshasnumerorecursos)   "; //desc (?a) 
                    }
                }
                else
                {
                    orderBy += " (?" + QuitaPrefijo(pTipoFiltro) + ") desc (?gnosshasnumerorecursos)   "; //desc (?a) 
                }
            }

            query += orderBy;

            if (pLimite > 0)
            {
                query += "LIMIT " + pLimite;
            }

            if (pInicio > 0)
            {
                query += " OFFSET " + pInicio;
            }
            query = NamespacesVirtuosoLectura + select + query;

            LeerDeVirtuoso(query, "RecursosBusqueda", pFacetadoDS, pProyectoID);
        }

        /// <summary>
        /// Obtiene el número de resultados de una determinada búsqueda
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pNombreFaceta">Nombre de la faceta</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pEstaEnMyGnoss">Verdad si el usuario está en MyGnoss</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario es miembro de la comunidad</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado0</param>
        /// <param name="pIdentidadID">Identificador de la identidad del usuario</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados</param>
        public void ObtieneNumeroResultados(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pObtenerRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, List<Guid> pListaExcluidos)
        {
            mConsultaNumeroResultados = true;
            string query = NamespacesVirtuosoLectura;


            query += "select (count(distinct ?s)) ";
            query += ObtenerFrom(pProyectoID);

            query += "WHERE ";
            query += "{ ";

            bool busquedaMensajesOComent = (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Count > 0 && (pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_MENSAJES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_COMENTARIOS || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_INVITACIONES || pListaFiltros["rdf:type"][0] == FacetadoAD.BUSQUEDA_SUSCRIPCIONES));

            if (!busquedaMensajesOComent && !pProyectoID.Contains("contactos"))
            {
                query += ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, pObtenerRecursosPrivados);
            }

            if (pProyectoID.Contains("contactos"))
            {
                if (!pListaFiltros.ContainsKey("gnoss:RecPer"))
                {
                    query += "?s <http://xmlns.com/foaf/0.1/knows> <http://gnoss/" + pIdentidadID + ">. ";
                }
                else
                {
                    query += "<http://gnoss/" + pIdentidadID + "> <http://gnoss/RecPer>  ?s12. ?s12 <http://gnoss/RecID> ?s. ?s12 <http://gnoss/RecNum> ?s2n. FILTER (?s!=<http://gnoss/" + pIdentidadID + ">)  MINUS {<http://gnoss/" + pIdentidadID + "> <http://gnoss/Ignora> ?s.} ";
                    pListaFiltros.Remove("gnoss:RecPer");
                }
            }

            var filtrosSearchPersonalizados = pFiltrosSearchPersonalizados;

            string filtros = ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, false, pOmitirPalabrasNoRelevantesSearch, pTiposAlgoritmoTransformacion, filtrosSearchPersonalizados, pEsMovil, pEsPeticionNumResultados: true);

            if (string.IsNullOrEmpty(filtros.Trim()))
            {
                filtros += "?s rdf:type ?rdftype";
            }

            query += filtros;

            // resultados excluidos
            if (pListaExcluidos != null && pListaExcluidos.Count > 0)
            {
                string filtroExcluidos = ObtenerFiltroRecursosExcluidos(pListaExcluidos);
                query = $" {query}{filtroExcluidos}";
            }

            query += " } ";

            mConsultaNumeroResultados = false;

            if (ObtenerSoloConsulta)
            {
                if (!pFacetadoDS.Tables.Contains("ConsultasBusqueda"))
                {
                    pFacetadoDS.Tables.Add("ConsultasBusqueda");
                    pFacetadoDS.Tables["ConsultasBusqueda"].Columns.Add("Consulta");
                    pFacetadoDS.Tables["ConsultasBusqueda"].Columns.Add("Numero");
                }
                if (query.Trim().ToUpper().StartsWith("SPARQL")) // Eliminar el SPARQL inicial
                {
                    query = query.Trim().Substring(6);
                }
                pFacetadoDS.Tables["ConsultasBusqueda"].Rows.Add(query, 0);
            }
            else
            {
                LeerDeVirtuoso(query, "NResultadosBusqueda", pFacetadoDS, pProyectoID);
            }
        }

        #endregion

        #region Métodos de obtención de información sobre resultados

        /// <summary>
        /// Ordena un diccionario por valor
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
        public Dictionary<string, int> SortMyDictionaryByValue(Dictionary<string, int> myDictionary)
        {
            List<KeyValuePair<string, int>> tempList = new List<KeyValuePair<string, int>>(myDictionary);

            tempList.Sort(delegate (KeyValuePair<string, int> firstPair, KeyValuePair<string, int> secondPair)
                            {
                                return firstPair.Value.CompareTo(secondPair.Value);
                            }
                         );

            Dictionary<string, int> mySortedDictionary = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> pair in tempList)
            {
                mySortedDictionary.Add(pair.Key, pair.Value);
            }

            return mySortedDictionary;
        }

        /// <summary>
        /// Obtiene los perfiles de las personas que quizas conoce otra teniendo en cuenta un filtro
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
        public DataSet PersonasQueQuizaConozcasAlg(Guid pIdentidadMyGnoss, int pInicio, int pLimite)
        {
            FacetadoDS resultadoDS = new FacetadoDS();

            string query = NamespacesVirtuosoLectura;

            query += " select distinct ?c2  count(distinct ?comunidad) as ?a    ?OrganizationNameEmpresaActual ?OrganizationName ?OrganizationNameDisciplina  ?OrganizationNameCentroEstudios ?locality ?ProvinceOrState  ?country    count(distinct ?c) as ?contactosEnComun    ";
            query += " where { ";
            query += " GRAPH " + ObtenerUrlGrafo("contactos") + " {?c2  <http://rdfs.org/sioc/ns#has_space> ?comunidad. ?s  <http://rdfs.org/sioc/ns#has_space> ?comunidad. MINUS {?s <http://xmlns.com/foaf/0.1/knows> ?c2. FILTER(?c2!=<http://gnoss/" + pIdentidadMyGnoss.ToString().ToUpper() + ">)} minus{FILTER(?comunidad in (<http://gnoss/11111111-1111-1111-1111-111111111111>,<http://gnoss/11111111-1111-1111-1111-111111111112>,<http://gnoss/11111111-1111-1111-1111-111111111113>))}} ";
            query += " GRAPH " + ObtenerUrlGrafo("11111111-1111-1111-1111-111111111111") + " { ";
            query += " {?s cv:OrganizationNameEmpresaActual ?OrganizationNameEmpresaActual . ?c2 cv:OrganizationNameEmpresaActual ?OrganizationNameEmpresaActual .} UNION ";
            query += " {?s cv:OrganizationName ?OrganizationName . ?c2 cv:OrganizationName ?OrganizationName .} UNION ";
            query += " {?s cv:OrganizationNameDisciplina ?OrganizationNameDisciplina . ?c2 cv:OrganizationNameDisciplina ?OrganizationNameDisciplina .} UNION   ";
            query += " {?s cv:OrganizationNameCentroEstudios ?OrganizationNameCentroEstudios . ?c2 cv:OrganizationNameCentroEstudios ?OrganizationNameCentroEstudios .} UNION   ";
            query += " {?s vcard:locality ?locality . ?c2 vcard:locality ?locality .} UNION  ";
            query += " {?s oc:ProvinceOrState ?ProvinceOrState . ?c2 oc:ProvinceOrState ?ProvinceOrState .} UNION  ";
            query += " {?s vcard:country-name ?country. ?c2 vcard:country-name ?country.} UNION ";
            query += " {?s foaf:name ?otras} ";

            query += " } ";
            query += " } order by desc (?a)";

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }
            LeerDeVirtuoso(query, "identidad", resultadoDS, "");
            return resultadoDS;
        }

        /// <summary>
        /// Obtiene los perfiles de las personas que quizas conoce otra teniendo en cuenta un filtro
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
        public string PersonasQueQuizaConozcasAlg2(Guid pIdentidadMyGnoss, Dictionary<string, string> d, int pInicio, int pLimite)
        {
            string query = NamespacesVirtuosoLectura;

            query += " select distinct ?c2    ?OrganizationNameEmpresaActual ?OrganizationName ?OrganizationNameDisciplina  ?OrganizationNameCentroEstudios ?locality ?ProvinceOrState  ?country  from " + ObtenerUrlGrafo("contactos") + "   ";

            query += " where { ";

            query += " ?c2  <http://rdfs.org/sioc/ns#has_space> ?comunidad. ?s  <http://rdfs.org/sioc/ns#has_space> ?comunidad. MINUS {?s <http://xmlns.com/foaf/0.1/knows> ?c2. FILTER(?c2!=<http://gnoss/" + pIdentidadMyGnoss.ToString().ToUpper() + ">)} minus{FILTER(?comunidad in (<http://gnoss/11111111-1111-1111-1111-111111111111>,<http://gnoss/11111111-1111-1111-1111-111111111112>,<http://gnoss/11111111-1111-1111-1111-111111111113>))} ";

            if (d.ContainsKey("OrganizationNameEmpresaActual"))
            {

                query += " {?s cv:OrganizationNameEmpresaActual ?OrganizationNameEmpresaActual .FILTER (?OrganizationNameEmpresaActual='" + d["OrganizationNameEmpresaActual"] + "') } UNION ";
            }

            if (d.ContainsKey("OrganizationName"))
            {

                query += " {?s cv:OrganizationName ?OrganizationName .FILTER (?OrganizationName='" + d["OrganizationName"] + "') } UNION ";
            }

            if (d.ContainsKey("OrganizationNameDisciplina"))
            {

                query += " {?s cv:OrganizationNameDisciplina ?OrganizationNameDisciplina .FILTER (?OrganizationNameDisciplina='" + d["OrganizationNameDisciplina"] + "') } UNION ";
            }

            if (d.ContainsKey("OrganizationNameCentroEstudios"))
            {

                query += " {?s cv:OrganizationNameCentroEstudios ?OrganizationNameCentroEstudios .FILTER (?OrganizationNameCentroEstudios='" + d["OrganizationNameCentroEstudios"] + "') } UNION ";
            }

            if (d.ContainsKey("locality"))
            {

                query += " {?s  vcard:locality ?locality .FILTER (?locality='" + d["locality"] + "') } UNION ";
            }

            if (d.ContainsKey("ProvinceOrState"))
            {

                query += " {?s  oc:ProvinceOrState ?ProvinceOrState .FILTER (?ProvinceOrState='" + d["ProvinceOrState"] + "') } UNION ";
            }

            if (d.ContainsKey("ProvinceOrState"))
            {
                query += " {?s  vcard:country-name ?country .FILTER (?country='" + d["country"] + "') } UNION ";
            }

            if (query.Substring(query.Length - 7).Contains("UNION"))
            {
                query = query.Substring(0, query.Length - 7);
            }

            query += " } order by desc (?OrganizationNameEmpresaActual) desc (?OrganizationName) desc  (?OrganizationNameDisciplina) desc   (?OrganizationNameCentroEstudios)desc  (?locality)desc  (?ProvinceOrState) desc  (?country) ";

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }

            return query;
        }

        public DataSet DatosIdentidad(Guid pIdentidadMyGnoss, int pInicio, int pLimite)
        {
            FacetadoDS resultadoDS = new FacetadoDS();

            string query = NamespacesVirtuosoLectura;

            query += $" select distinct  ?OrganizationNameEmpresaActual ?OrganizationName ?OrganizationNameDisciplina ?OrganizationNameCentroEstudios ?locality ?ProvinceOrState  ?country   from  {ObtenerUrlGrafo("11111111-1111-1111-1111-111111111111")} ";
            query += " where {?s ?p ?o. FILTER(?s=<http://gnoss/" + pIdentidadMyGnoss.ToString().ToUpper() + ">)";
            query += " OPTIONAL {?s cv:OrganizationNameEmpresaActual ?OrganizationNameEmpresaActual}  ";
            query += " OPTIONAL {?s cv:OrganizationName ?OrganizationName }  ";
            query += " OPTIONAL {?s cv:OrganizationNameDisciplina ?OrganizationNameDisciplina }    ";
            query += " OPTIONAL {?s cv:OrganizationNameCentroEstudios ?OrganizationNameCentroEstudios}    ";
            query += " OPTIONAL {?s vcard:locality ?locality}   ";
            query += " OPTIONAL {?s oc:ProvinceOrState ?ProvinceOrState }   ";
            query += " OPTIONAL {?s vcard:country-name ?country}  ";
            query += " } ";

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }
            LeerDeVirtuoso(query, "identidad", resultadoDS, "");
            return resultadoDS;
        }

        /// <summary>
        /// Obtiene las comunidades que le pueden interesar a un perfil
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>    
        /// <param name="pInicio">Inicio</param>
        /// <param name="pLimite">Limite</param>
        /// <param name="pResultados">TRUE obtenemos los resultados/ FALSE obtenemos las facetas</param>
        public DataSet ComunidadesQueTePuedanInteresar(Guid pIdentidadMyGnoss, int pInicio, int pLimite, bool pResultados, Dictionary<string, List<string>> pListaFiltros)
        {
            FacetadoDS resultadoDS = new FacetadoDS();

            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            if (pResultados)
            {
                query.Append(" select distinct ?comunidad 'Comunidad' as ?rdftype count(distinct ?c) as ?a ");
            }
            else
            {
                query.Append(" select distinct ?c count(distinct ?comunidad) as ?a ");
            }
            query.Append($" from {ObtenerUrlGrafo("contactos")} ");
            query.Append($" where {{ {{?s <http://xmlns.com/foaf/0.1/knows>  ?c. ?c  <http://rdfs.org/sioc/ns#has_space> ?comunidad. FILTER(?s=<http://gnoss/{pIdentidadMyGnoss.ToString().ToUpper()}> ");

            if (pListaFiltros.ContainsKey("sioc:has_space"))
            {
                string coma = "";
                query.Append(" AND ?c IN(");
                foreach (string filtro in pListaFiltros["sioc:has_space"])
                {
                    query.Append(coma + filtro);
                    coma = ",";
                }
                query.Append(")");
            }

            query.Append(" )  ?comunidad <http://gnoss/hasprivacidadMyGnoss> ?privacidad. FILTER (?privacidad in (\"restringido\",\"publico\" )) ?comunidad <http://gnoss/estadoProyecto> \"Abierto\" .MINUS {?s <http://xmlns.com/foaf/0.1/knows>  ?c. ?c  <http://rdfs.org/sioc/ns#has_space> ?comunidad .?u <http://gnoss/IdentidadID> ?s.  ?u <http://gnoss/IdentidadID> ?i. ?i <http://rdfs.org/sioc/ns#has_space> ?comunidad} FILTER(?comunidad!=<http://gnoss/11111111-1111-1111-1111-111111111111> and ?comunidad!=<http://gnoss/11111111-1111-1111-1111-111111111112> and ?comunidad!=<http://gnoss/11111111-1111-1111-1111-111111111113>) MINUS {?s <http://gnoss/Ignora> ?comunidad.}  } ");
            query.Append(" } order by  desc (?a)");

            if (pLimite > 0)
            {
                query.Append($" LIMIT {pLimite}");
            }

            if (pInicio != 0)
            {
                query.Append($" OFFSET {pInicio}");
            }

            string nombreTabla = "RecursosBusqueda";
            if (!pResultados)
            {
                nombreTabla = "sioc:has_space";
            }
            LeerDeVirtuoso(query.ToString(), nombreTabla, resultadoDS, "");

            return resultadoDS;
        }

        /// <summary>
        /// Obtiene el número de comunidades que le pueden interesar a un perfil
        /// </summary>     
        /// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
        public DataSet NumeroComunidadesQueTePuedanInteresar(Guid pIdentidadMyGnoss, Dictionary<string, List<string>> pListaFiltros)
        {
            FacetadoDS resultadoDS = new FacetadoDS();

            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            query.Append(" select (count(distinct ?comunidad)) ");
            query.Append($" from {ObtenerUrlGrafo("contactos")} ");
            query.Append($" where {{ {{?s <http://xmlns.com/foaf/0.1/knows>  ?c. ?c  <http://rdfs.org/sioc/ns#has_space> ?comunidad. FILTER(?s=<http://gnoss/{pIdentidadMyGnoss.ToString().ToUpper()}> ");

            if (pListaFiltros.ContainsKey("sioc:has_space"))
            {
                string coma = "";
                query.Append(" AND ?c IN(");
                foreach (string filtro in pListaFiltros["sioc:has_space"])
                {
                    query.Append(coma + filtro);
                    coma = ",";
                }
                query.Append(")");
            }

            query.Append(" )  ?comunidad <http://gnoss/hasprivacidadMyGnoss> ?privacidad. FILTER (?privacidad in (\"restringido\",\"publico\" )) MINUS {?s <http://xmlns.com/foaf/0.1/knows>  ?c. ?c  <http://rdfs.org/sioc/ns#has_space> ?comunidad .?u <http://gnoss/IdentidadID> ?s.  ?u <http://gnoss/IdentidadID> ?i. ?i <http://rdfs.org/sioc/ns#has_space> ?comunidad} MINUS {?s <http://gnoss/Ignora> ?comunidad.}  } ");
            query.Append(" } ");

            LeerDeVirtuoso(query.ToString(), "NResultadosBusqueda", resultadoDS, "");

            return resultadoDS;
        }

        /// <summary>
        /// Obtiene los contactos en comun entre dos perfiles
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pPerfil1">Id Perfil1</param>
        /// <param name="pPerfil2">Id Perfil2</param>
        public DataSet ObtenerContactos(Guid pPerfil1, Guid pPerfil2, Guid pProyecto, int pInicio, int pLimite)
        {
            FacetadoDS resultadoDS = new FacetadoDS();

            string query = NamespacesVirtuosoLectura;

            query += " select bif:concat(?Nombre,' ',?Apellido) as ?NombreApellido ?Foto  ?NombreCortoUsu ?NombreCortoOrg ?Tipo  ";
            query += " where { ";
            query += " GRAPH " + ObtenerUrlGrafo("contactos") + " {?s2 foaf:knows ?s3. ?s foaf:knows ?s3.  FILTER (?s=gnoss:" + pPerfil1.ToString().ToUpper() + ") FILTER (?s2=gnoss:" + pPerfil2.ToString().ToUpper() + ") }";
            query += " GRAPH " + ObtenerUrlGrafo(pProyecto.ToString()) + "  {?id gnoss:hasPerfil ?s3. ?id foaf:firstName ?Nombre. ?id foaf:familyName ?Apellido. ?id  gnoss:hasfoto ?Foto. ?id rdf:type ?Tipo. ?id gnoss:nombreCortoUsu ?NombreCortoUsu. OPTIONAL{?id gnoss:nombreCortoOrg ?NombreCortoOrg. } ?id gnoss:hasPopularidad ?popularidad} ";
            query += " } order by desc (?popularidad)";

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }

            LeerDeVirtuoso(query, "perfil", resultadoDS, pProyecto.ToString());

            return resultadoDS;
        }

        /// <summary>
        /// Obtiene los seguidores en comun entre dos perfiles
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pPerfil1">Id Perfil1</param>
        /// <param name="pPerfil2">Id Perfil2</param>
        /// <param name="pProyecto">Id proyecto</param>
        public DataSet ObtenerSeguidoresComunidad(Guid pPerfil1, Guid pPerfil2, Guid pProyecto, int pInicio, int pLimite)
        {
            FacetadoDS resultadoDS = new FacetadoDS();

            string query = NamespacesVirtuosoLectura;
            query += " select bif:concat(?Nombre,' ',?Apellido) as ?NombreApellido ?Foto  ?NombreCortoUsu ?NombreCortoOrg  ?Tipo  ";
            query += " where { ";
            query += " GRAPH " + ObtenerUrlGrafo("seguidores") + " {?s2 gnoss:sigue ?s3. ?s gnoss:sigue ?s3. FILTER (?s=gnoss:" + pPerfil1.ToString().ToUpper() + pProyecto.ToString().ToUpper() + " or ?s=gnoss:" + pPerfil1.ToString().ToUpper() + "11111111-1111-1111-1111-111111111111) FILTER (?s2=gnoss:" + pPerfil2.ToString().ToUpper() + pProyecto.ToString().ToUpper() + " or ?s=gnoss:" + pPerfil2.ToString().ToUpper() + "11111111-1111-1111-1111-111111111111) }";
            query += " GRAPH " + ObtenerUrlGrafo(pProyecto.ToString()) + "  {?id gnoss:hasPerfilProyecto ?s3. ?id foaf:firstName ?Nombre. ?id foaf:familyName ?Apellido. ?id  gnoss:hasfoto ?Foto. ?id rdf:type ?Tipo. ?id gnoss:nombreCortoUsu ?NombreCortoUsu.OPTIONAL{ ?id gnoss:nombreCortoOrg ?NombreCortoOrg. } ?id gnoss:hasPopularidad ?popularidad} ";
            query += " } order by desc (?popularidad)";

            if (pLimite > 0)
            {
                query += " LIMIT " + pLimite;
            }

            if (pInicio != 0)
            {
                query += " OFFSET " + pInicio;
            }

            LeerDeVirtuoso(query, "perfil", resultadoDS, pProyecto.ToString());

            return resultadoDS;
        }

        /// <summary>
        /// Obtiene el ID de un CV desde virtuoso para su eliminación
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pidproyecto">Identificador del proyecto</param>
        /// <param name="pididentidad">Identificador de la identidad</param>
        public void ObtenerIDDocCVDesdeVirtuoso(FacetadoDS pFacetadoDS, string pidproyecto, string pididentidad)
        {
            string query = NamespacesVirtuosoLectura;

            query += " select  ?o    ";
            query += ObtenerFrom(pidproyecto);
            query += " WHERE {  gnoss:" + pididentidad.ToUpper() + " gnoss:hasCv ?o ";
            query += "  }";

            LeerDeVirtuoso(query, "IdsCVs", pFacetadoDS, pidproyecto);
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPersonaID">Identificador de la persona</param>
        /// <returns></returns>
        public FacetadoDS ObtieneInformacionPersonas(Guid pProyectoID, Guid pPersonaID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            List<string> listaIds = new List<string>();
            listaIds.Add("http://gnoss/" + pPersonaID.ToString().ToUpper());
            ObtieneInformacionPersonas(pProyectoID.ToString(), facetadoDS, listaIds);

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPersonasID">Identificador de la persona</param>
        /// <returns></returns>
        public FacetadoDS ObtieneInformacionPersonas(Guid pProyectoID, List<Guid> pPersonasID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            List<string> listaIds = new List<string>();
            foreach (Guid personaID in pPersonasID)
            {
                listaIds.Add("http://gnoss/" + personaID.ToString().ToUpper());
            }

            ObtieneInformacionPersonas(pProyectoID.ToString(), facetadoDS, listaIds);

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneInformacionPersonas(string pProyectoID, FacetadoDS pFacetadoDS)
        {
            if (pFacetadoDS != null && pFacetadoDS.Tables.Count > 0 && pFacetadoDS.Tables["RecursosBusqueda"].Rows.Count > 0 && pFacetadoDS.Tables["RecursosBusqueda"].Select("rdftype='" + FacetadoAD.BUSQUEDA_PERSONA + "' or rdftype='" + FacetadoAD.BUSQUEDA_ORGANIZACION + "' or rdftype='" + FacetadoAD.BUSQUEDA_CLASE + "' or rdftype='" + FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA + "' or rdftype='" + FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD + "' or rdftype='" + FacetadoAD.BUSQUEDA_ALUMNO + "' or rdftype='" + FacetadoAD.BUSQUEDA_PROFESOR + "' or rdftype='" + FacetadoAD.BUSQUEDA_CONTACTOS_PERSONAL + "'").Length > 0)
            {
                List<string> listaIds = new List<string>();

                foreach (DataRow myrow in pFacetadoDS.Tables["RecursosBusqueda"].Rows)
                {
                    if (myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_PERSONA) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_ORGANIZACION) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_CLASE) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_ALUMNO) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_PROFESOR) || myrow["rdftype"].Equals(FacetadoAD.BUSQUEDA_CONTACTOS_PERSONAL))
                    {
                        listaIds.Add((string)myrow[0]);
                    }
                }

                if (listaIds.Count > 0)
                {
                    ObtieneInformacionPersonas(pProyectoID, pFacetadoDS, listaIds);
                }
            }
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        /// <param name="pListaPersonaID">Lista de sujetos de los que queremos obtener la informacion</param>
        public void ObtieneInformacionPersonas(string pProyectoID, FacetadoDS pFacetadoDS, List<string> pListaPersonaID)
        {
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            query.Append(" select  ?s   ?countryname ?ProvinceOrState ?locality ?Foto ?ExecutiveSummary ?PositionTitleEmpresaActual ?OrganizationNameEmpresaActual ?Tag ");
            query.Append(ObtenerFrom(pProyectoID));
            query.Append(" WHERE {  ");
            query.Append("  {  ?s sioc_t:Tag ?Tag.} ");
            query.Append(" UNION {?s  gnoss:hasfoto ?Foto.} ");
            query.Append(" UNION {?s  vcard:country-name ?countryname.} ");
            query.Append("  UNION {?s  oc:ProvinceOrState ?ProvinceOrState.} ");
            query.Append(" UNION {?s  vcard:locality ?locality.} ");
            query.Append(" UNION {?s gnoss:hasCv ?CV. ?CV cv:PositionTitleEmpresaActual ?PositionTitleEmpresaActual.} ");
            query.Append(" UNION {?s gnoss:hasCv ?CV. ?CV cv:OrganizationNameEmpresaActual ?OrganizationNameEmpresaActual.} ");
            query.Append(" UNION {?s gnoss:hasCv ?CV. ?CV cv:ExecutiveSummary ?ExecutiveSummary.} ");
            query.Append("  UNION {?s gnoss:hasExecutiveSummary ?ExecutiveSummary.} ");
            query.Append(" FILTER( ");

            foreach (string id in pListaPersonaID)
            {
                query.Append($" ?s=<{id}> or");
            }

            query.Remove(query.Length - 2, 2);
            query.Append(" ) }");

            LeerDeVirtuoso(query.ToString(), "DatosPersonas", pFacetadoDS, pProyectoID);
        }


        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneInformacionRecursosCatalogo(Guid pProyectoID, FacetadoDS pFacetadoDS)
        {
            string query = NamespacesVirtuosoLectura;
            if (!query.Contains("obrasArte"))
            {
                query += "prefix obrasArte:<http://pruebas.gnoss.net/Ontologia/ontologiaobradearte.owl#>";
            }

            query += " select  ?s   ?url ";
            query += ObtenerFrom(pProyectoID.ToString());
            query += " WHERE {  ";
            query += "  {  ?s obrasArte:Imagen ?url.} ";
            query += " }";

            LeerDeVirtuoso(query, "DatosObraArte", pFacetadoDS, pProyectoID.ToString());
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo grupo de contacto
        /// </summary>
        /// <param name="perfil1">Identidad</param>
        /// <param name="perfil1">idGrupo</param>
        public void InsertarNuevoGrupoContactos(string identidad, string idGrupo, string nombreGrupo)
        {
            InsertarNuevoGrupoContactos(identidad, idGrupo, nombreGrupo, null);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo grupo de contacto
        /// </summary>
        /// <param name="perfil1">Identidad</param>
        /// <param name="perfil1">idGrupo</param>
        /// <param name="nombreGrupo">nombre del grupo</param>
        /// <param name="idProfesor">Id del profesor que crea la clase.</param>
        public void InsertarNuevoGrupoContactos(string identidad, string idGrupo, string nombreGrupo, string idProfesor)
        {
            InsertarNuevoGrupoContactos(identidad, idGrupo, nombreGrupo, idProfesor, true);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo grupo de contacto
        /// </summary>
        /// <param name="perfil1">Identidad</param>
        /// <param name="perfil1">idGrupo</param>
        /// <param name="nombreGrupo">nombre del grupo</param>
        /// <param name="idProfesor">Id del profesor que crea la clase.</param>
        public void InsertarNuevoGrupoContactos(string identidad, string idGrupo, string nombreGrupo, string idProfesor, bool pUsarColaActualizacion)
        {

            string tripletas = GenerarTripleta("<http://gnoss/" + idGrupo.ToUpper() + ">", "<http://xmlns.com/foaf/0.1/knows>", "<http://gnoss/" + identidad.ToUpper() + ">");

            tripletas += GenerarTripleta("<http://gnoss/" + idGrupo.ToUpper() + ">", "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", "\"ContactoGrupo\"");

            tripletas += GenerarTripleta("<http://gnoss/" + idGrupo.ToUpper() + ">", "<http://xmlns.com/foaf/0.1/firstName>", "\"" + nombreGrupo.ToLower() + "\"");

            tripletas += GenerarTripleta("<http://gnoss/" + idGrupo.ToUpper() + ">", "<http://gnoss/hasnombrecompleto>", "\"" + nombreGrupo.ToLower() + "\"");

            if (!string.IsNullOrEmpty(idProfesor))
            {
                tripletas += GenerarTripleta("<http://gnoss/" + idProfesor.ToUpper() + ">", "<http://xmlns.com/foaf/0.1/knows>", "<http://gnoss/" + idGrupo.ToUpper() + ">");

                tripletas += GenerarTripleta("<http://gnoss/" + idGrupo.ToUpper() + ">", "<http://xmlns.com/foaf/0.1/knows>", "<http://gnoss/" + idProfesor.ToUpper() + ">");
            }

            InsertaTripletas("contactos", tripletas, 0, pUsarColaActualizacion);

        }

        /// <summary>
        /// Modifica en virtuoso un contacto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadID">Identificadro de la identidad</param>
        public void ModificarGrupoContactos(string idGrupo, string nombreGrupo)
        {
            string query = NamespacesVrituosoEscritura;

            query += "MODIFY GRAPH <" + mUrlIntranet + "contactos> DELETE {?s ?p ?o. } INSERT { ?s ?p \"" + nombreGrupo + "\".}  where {?s ?p ?o. FILTER (?s=<http://gnoss/" + idGrupo.ToUpper() + "> AND ?p=<http://xmlns.com/foaf/0.1/firstName>)}";

            ActualizarVirtuoso(query, idGrupo);
        }

        /// <summary>
        /// Obtiene si la identidad tiene foto
        /// </summary>
        /// <param name="pIdentidadID">Identidad ID</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Devuelve si la identidad tiene foto</returns>
        public bool ObtenerSiIdentidadTieneFoto(Guid pIdentidadID, Guid pProyectoID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            string query = NamespacesVirtuosoLectura;

            query += " ask ";
            query += ObtenerFrom(pProyectoID.ToString());
            query += " WHERE {  ";
            query += " {?s  gnoss:hasfoto ?Foto.} ";

            query += " FILTER( ";
            query += " ?s=gnoss:" + pIdentidadID.ToString().ToUpper() + "";
            query += " and ?Foto!='" + SIN_FOTO + "'";
            query += " ) }";

            LeerDeVirtuoso(query, "TieneFoto", facetadoDS, pProyectoID.ToString());

            if ((facetadoDS.Tables.Contains("TieneFoto") && (facetadoDS.Tables["TieneFoto"].Rows.Count > 0)))
            {
                object tieneFoto = facetadoDS.Tables["TieneFoto"].Rows[0][0];
                int resultado;
                int.TryParse(tieneFoto.ToString(), out resultado);

                return resultado == 1;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene todos los recursos y dafos que pertenecen a una determinada categoría
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pCategoriaID">Identificador de la categoría</param>
        /// <returns></returns>
        public FacetadoDS ObtieneElementosDeCategoria(string pProyectoID, string pCategoriaID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;

            query += " select  ?s ?type ";
            query += ObtenerFrom(pProyectoID);
            query += " WHERE {  ";
            query += "  {  ?s skos:ConceptID ?ConceptID. ?s rdf:type ?type.} ";
            query += " FILTER( ";
            query += " ?ConceptID=gnoss:" + pCategoriaID.ToString().ToUpper();
            query += " ) }";

            LeerDeVirtuoso(query, "Resultados", facetadoDS, pProyectoID);

            return facetadoDS;
        }

        public void ObtieneDatosAutocompletar(string nombregrafo, string filtro, FacetadoDS pFacetadoDS)
        {
            string query = NamespacesVirtuosoLectura;

            query += " select  ?o ";

            query += "from " + ObtenerUrlGrafo(nombregrafo);

            query += " WHERE {  ";
            query += "  gnoss:" + nombregrafo + " gnoss:has" + nombregrafo + " ?o.  ";

            string caso1 = filtro[0].ToString().ToLower();
            string caso2 = filtro[0].ToString().ToUpper();

            if (filtro.Length > 1)
            {
                caso1 += filtro.Substring(1);
                caso2 += filtro.Substring(1);
            }

            query += " FILTER   (?o LIKE '" + caso1 + "%' or ?o LIKE '% " + caso1 + "%' or ?o LIKE '" + caso2 + "%' or ?o LIKE '% " + caso2 + "%') ";

            query += "  }";

            LeerDeVirtuoso(query, nombregrafo, pFacetadoDS, nombregrafo);
        }

        /// <summary>
        /// Devuelve el RDF de un formulario semántico.
        /// </summary>
        /// <param name="pNombreontologia">Nombre de la ontología</param>
        /// <param name="pIDDocSem">ID del documento semántico</param>
        /// <returns>DataSet con el RDF del documento</returns>
        public FacetadoDS ObtenerRDFXMLdeFormulario(string pNombreontologia, string pIDDocSem, bool pUsarAfinidad = false)
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            StringBuilder queryInternaSB = new StringBuilder();

            queryInternaSB.AppendLine("select ?s ?p ?o lang(?o) as ?idioma ");

            queryInternaSB.AppendLine("from " + ObtenerUrlGrafo(pNombreontologia).ToLower() + " ");

            queryInternaSB.AppendLine(" WHERE  { ?documento ?tieneEntidad ?s.  FILTER (?documento = <" + mUrlIntranet + pIDDocSem.ToLower() + ">) ?s ?p ?o }  ");

            queryInternaSB.AppendLine(" order by ?s ?p ?o ?idioma ");

            string queryInterna = queryInternaSB.ToString();
            string query = $"{NamespacesVirtuosoLectura} {queryInterna} limit 10000";

            LeerDeVirtuoso(query, pNombreontologia, facetadoDS, pNombreontologia, pUsarAfinidad);

            int offset = 0;
            int numResultadosMax = 10000;
            while (facetadoDS.Tables[0].Rows.Count == numResultadosMax)
            {
                offset += 10000;
                numResultadosMax += 10000;

                string queryMasResultados = $"{NamespacesVirtuosoLectura} SELECT ?s ?p ?o ?idioma {{ {queryInterna} }} offset {offset} limit 10000";

                FacetadoDS facetadoDSMasResultados = new FacetadoDS();

                LeerDeVirtuoso(queryMasResultados, pNombreontologia, facetadoDSMasResultados, pNombreontologia);

                facetadoDS.Merge(facetadoDSMasResultados);
            }

            return facetadoDS;
        }

        /// <summary>
        /// Devuelve las primeras categorías de un tesauro semántico.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pPropiedad">Propiedad vinculante</param>
        /// <param name="pTipoEntidadSolicitada">Tipo de entidad solicitada</param>
        /// <param name="pPropSolicitadas">Propiedades de la entidad solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerCatPrimerNivelTesSemanticoFormulario(string pGrafo, string pPropiedad, string pTipoEntidadSolicitada, List<string> pPropSolicitadas)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;

            query += " select distinct ?s ?p ?o lang(?o) as ?idioma ";
            query += "from " + ObtenerUrlGrafo(pGrafo).ToLower() + " ";
            query += " WHERE { ?stt <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> ?ott. ?stt <" + pPropSolicitadas[0] + "> ?ots. ?stt <" + pPropSolicitadas[1] + "> ?s. ?s ?p ?o. FILTER (";

            query += "?ott=";

            if (pTipoEntidadSolicitada.StartsWith("http"))
            {
                query += "<" + pTipoEntidadSolicitada + ">";
            }
            else
            {
                query += "'" + pTipoEntidadSolicitada + "'";
            }

            if (pPropiedad.StartsWith("http"))
            {
                query += " AND ?ots=<" + pPropiedad + ">";
            }
            else
            {
                query += " AND ?ots='" + pPropiedad + "'";
            }

            query += ")}";

            LeerDeVirtuoso(query, "SelectTesSem", facetadoDS, pGrafo);//SelectEnt

            return facetadoDS;
        }

        /// <summary>
        /// Devuelve las categorías hijas de una categoría de un tesauro semántico.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pIDCategoria">ID de la categoría padre</param>
        /// <param name="pPropRelacion">Propiedad de unión entre categorías</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerCatHijasCatTesSemanticoFormulario(string pGrafo, string pIDCategoria, string pPropRelacion)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;

            query += " select distinct ?s ?p ?o lang(?o) as ?idioma ";
            query += "from " + ObtenerUrlGrafo(pGrafo).ToLower() + " ";
            query += " WHERE { <" + pIDCategoria + "> <" + pPropRelacion + "> ?s. ?s ?p ?o. }";

            LeerDeVirtuoso(query, "SelectTesSem", facetadoDS, pGrafo);//SelectEnt

            return facetadoDS;
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntContenedora">Entidad contenedora</param>
        /// <param name="pPropiedad">Propiedad vinculante</param>
        /// <param name="pTipoEntidadSolicitada">Tipo de entidad solicitada</param>
        /// <param name="pPropSolicitadas">Propiedades de la entidad solicitadas</param>
        /// <param name="pFiltro">Filtro</param>
        /// <param name="pExtraWhere">Cadena extra para el where de la consulta</param>
        /// <param name="pIdioma">Idioma</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerRDFXMLSelectorEntidadFormulario(string pGrafo, string pEntContenedora, string pPropiedad, string pTipoEntidadSolicitada, List<string> pPropSolicitadas, string pFiltro, string pExtraWhere, string pIdioma, Guid pIdentidadID, Guid pProyectoID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            bool esEntidadPrincipal = EsEntidadPrincipal(pGrafo);
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            string limiteConfig = null;

            if (pFiltro != null && pFiltro.Contains("|||Limite="))
            {
                limiteConfig = pFiltro.Substring(pFiltro.LastIndexOf("=") + 1);
                pFiltro = pFiltro.Substring(0, pFiltro.IndexOf("|||Limite="));
            }

            StringBuilder whereTipoEntidadSol = new StringBuilder();

            if (!string.IsNullOrEmpty(pTipoEntidadSolicitada))
            {
                if (pTipoEntidadSolicitada.Contains(","))
                {
                    whereTipoEntidadSol.Append("?o IN (");
                    List<string> entidadesSol = new List<string>(pTipoEntidadSolicitada.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                    foreach (string entidadSol in entidadesSol)
                    {
                        if (entidadSol.StartsWith("http://") || entidadSol.StartsWith("https://"))
                        {
                            whereTipoEntidadSol.Append($"<{entidadSol}>,");
                        }
                        else
                        {
                            whereTipoEntidadSol.Append($"'{entidadSol}',");
                        }
                    }

                    whereTipoEntidadSol.Remove(whereTipoEntidadSol.Length - 1, 1);
                    whereTipoEntidadSol.Append(")");
                }
                else
                {
                    if (pTipoEntidadSolicitada.StartsWith("http://") || pTipoEntidadSolicitada.StartsWith("https://"))
                    {
                        whereTipoEntidadSol.Append($"?o = <{pTipoEntidadSolicitada}>");
                    }
                    else
                    {
                        whereTipoEntidadSol.Append($"?o = '{pTipoEntidadSolicitada}'");
                    }
                }
            }

            query.Append(" select distinct ?s ?p ?o ");
            if (string.IsNullOrEmpty(pIdioma))
            {
                query.Append(" lang(?o) as ?idioma ");
            }

            query.Append($"from {ObtenerUrlGrafo(pGrafo).ToLower()} ");

            if (!pIdentidadID.Equals(Guid.Empty) && !pProyectoID.Equals(Guid.Empty) && esEntidadPrincipal)
            {
                query.Append($"from {ObtenerUrlGrafo(pProyectoID.ToString()).ToLower()}");
            }

            if (!string.IsNullOrEmpty(pPropiedad) && pPropSolicitadas.Count > 0 && pPropiedad == pPropSolicitadas[0])
            {
                StringBuilder where = new StringBuilder();
                StringBuilder jerquia = new StringBuilder();

                foreach (string propSol in pPropSolicitadas)
                {
                    if (!propSol.Contains("|"))
                    {
                        where.Append($"?p = <{propSol}> OR ");
                    }
                    else
                    {
                        int count = 0;
                        foreach (string propJerq in propSol.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (jerquia.Length == 0)
                            {
                                jerquia.Append($" ?s ?p{count} ?o{count}. ");
                                where.Append("(");
                            }
                            else
                            {
                                jerquia.Append($"?o{count - 1} ?p{count} ?o{count}. ");
                            }

                            where.Append($"?p{count} =<{propJerq}> AND ");
                            count++;
                        }

                        where.Remove(where.Length - 4, 4);
                        where.Append(") OR ");
                        break;
                    }

                    if (!string.IsNullOrEmpty(pFiltro))
                    {
                        break;
                    }
                }

                if (jerquia == null || jerquia.Length == 0)
                {
                    query.Append($" WHERE {{ ?s ?p ?o. FILTER (({where}");
                }
                else
                {
                    query.Append($" WHERE {{ {jerquia.ToString().Substring(0, jerquia.ToString().LastIndexOf("?o"))} ?o. FILTER (({where}");
                    query = query.Replace("?p0 ", "?p ");
                }

                query.Remove(query.Length - 3, 3);
                query.Append(") ");
                if (!string.IsNullOrEmpty(pFiltro))
                {
                    if (pFiltro[0] == '%')
                    {
                        pFiltro = pFiltro.Substring(1);
                        query.Append($" AND (bif:lower(str(?o)) LIKE '%{UtilCadenas.ToSparql(pFiltro.ToLower())}%') ");
                    }
                    else
                    {
                        pFiltro = UtilCadenas.ToSparql(pFiltro);
                        query.Append($" AND (?o = '{pFiltro}') ");
                    }
                }

                if (!string.IsNullOrEmpty(pIdioma))
                {
                    query.Append($" AND (lang(?o)='{pIdioma}' OR lang(?o)='') ");
                }

                query.Append($" ) {pExtraWhere} }}");
            }
            else
            {
                //"@@@?s2?p2?o2@@@" se sustiuira por ?s2 ?p2 ?o2. si se va a usar en el where, si no se usa se elimina al final.
                query.Append(" WHERE { @@@?s2?p2?o2@@@ ?s ?p ?o. FILTER (");

                if ((!string.IsNullOrEmpty(pEntContenedora) || !string.IsNullOrEmpty(pPropiedad)) && string.IsNullOrEmpty(pTipoEntidadSolicitada))
                {
                    if (pEntContenedora != null)
                    {
                        query.Append($" ?s2 = <{pEntContenedora}> AND");
                    }

                    if (pPropiedad != null)
                    {
                        query.Append($" ?p2 = <{pPropiedad}> AND");
                    }

                    query.Append(" ?o2 = ?s ");
                    query.Append(" AND (");
                    query = query.Replace("@@@?s2?p2?o2@@@", "?s2 ?p2 ?o2.");
                }
                else if (string.IsNullOrEmpty(pEntContenedora) && string.IsNullOrEmpty(pPropiedad) && !string.IsNullOrEmpty(pTipoEntidadSolicitada))
                {
                    query.Append(" ?p2 = <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> AND ");
                    query.Append($"{whereTipoEntidadSol.Replace("?o", "?o2")} ");
                    query.Append(" AND (");
                    query = query.Replace("@@@?s2?p2?o2@@@", "?s ?p2 ?o2.");
                }
                else if (!string.IsNullOrEmpty(pPropiedad) && !string.IsNullOrEmpty(pTipoEntidadSolicitada))
                {
                    if (pPropiedad != "ANY")
                    {
                        query.Append($" ?p2 = <{pPropiedad}> AND");
                    }

                    query.Append($"{whereTipoEntidadSol.Replace("?o", "?o2")} ");
                    query.Append(" AND (");
                    query.Replace("@@@?s2?p2?o2@@@", "?s ?p2 ?o2.");
                }
                else
                {
                    query.Append(" (");
                }

                StringBuilder jerquia = new StringBuilder();

                foreach (string propSol in pPropSolicitadas)
                {
                    if (!propSol.Contains("|"))
                    {
                        query.Append($"?p=<{propSol}> OR ");
                    }
                    else
                    {
                        int count = 0;
                        foreach (string propJerq in propSol.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (jerquia.Length == 0)
                            {
                                jerquia.Append($"?s ?p{count} ?o{count}. ");
                                query.Append("(");
                            }
                            else
                            {
                                jerquia.Append($"?o{(count - 1)} ?p{count} ?o{count}. ");
                            }

                            query.Append($"?p{count} =<{propJerq}> AND ");
                            count++;
                        }

                        query.Remove(query.Length - 4, 4);
                        query.Append(") OR ");
                        break;
                    }

                    if (!string.IsNullOrEmpty(pFiltro) && !pFiltro.StartsWith("orderby|"))
                    {
                        break;
                    }
                }

                query.Remove(query.Length - 3, 3);
                query.Append(") ");
                if (jerquia.Length > 0)
                {
                    query = query.Replace("?s ?p ?o. FILTER", $"{jerquia.ToString().Substring(0, jerquia.ToString().LastIndexOf("?o"))} ?o. FILTER").Replace("?p0 ", "?p ");
                }

                string tipoOrden = null;

                if (!string.IsNullOrEmpty(pFiltro))
                {
                    if (pFiltro.StartsWith("orderby|"))
                    {
                        tipoOrden = pFiltro.Split('|')[1];
                        string prop = pFiltro.Split('|')[2];

                        query.Append($" ) ?s2 <{prop}> ?ordenProp");
                        query = query.Replace("@@@?s2?p2?o2@@@", "?s2 ?p2 ?o2.");
                    }
                    else
                    {
                        if (pFiltro[0] == '%')
                        {
                            pFiltro = pFiltro.Substring(1);
                            query.Append($" AND {ObtenerExpresionRegularParaAutocompletar(pFiltro, "?o")}");
                        }
                        else
                        {
                            pFiltro = UtilCadenas.ToSparql(pFiltro);
                            query.Append($" AND (?o = '{pFiltro}') ");
                        }

                        if (!string.IsNullOrEmpty(pIdioma))
                        {
                            query.Append($" AND (lang(?o)='{pIdioma}' OR lang(?o)='') ");
                        }

                        query.Append(" )");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(pIdioma))
                    {
                        query.Append($" AND (lang(?o)='{pIdioma}' OR lang(?o)='') ");
                    }

                    query.Append(" )");
                }

                query.Append(pExtraWhere);
                if (!pIdentidadID.Equals(Guid.Empty) && !pProyectoID.Equals(Guid.Empty) && esEntidadPrincipal)
                {
                    query.Append(GenerarBloqueGrafoBusqueda(pIdentidadID));
                }

                query.Append(" }");

                if (tipoOrden != null)
                {
                    query.Append($" order by {tipoOrden} (?ordenProp) ");
                }
                else if (!string.IsNullOrEmpty(pFiltro))
                {
                    query.Append(" order by ?o ");
                    if (limiteConfig == null)
                    {
                        query.Append(" LIMIT 15");
                    }
                    else
                    {
                        query.Append($" LIMIT {limiteConfig}");
                    }
                }
            }

            query = query.Replace("@@@?s2?p2?o2@@@", "");

            LeerDeVirtuoso(query.ToString(), "SelectPropEnt", facetadoDS, pGrafo);//SelectEnt

            return facetadoDS;
        }

        /// <summary>
        /// Genera la parte de la consulta para filtrar por privacidad el grafo de búsqueda. Obtiene al partir del triple hasEntidad la URI del grafo de búsqueda
        /// </summary>
        /// <returns>El bloque de la consulta referente al grafo de búsqueda</returns>
        private string GenerarBloqueGrafoBusqueda(Guid pIdentidadID)
        {
            string urlIntragnoss = ObtenerUrlIntragnoss();

            StringBuilder query = new StringBuilder();

            query.AppendLine(" ?sujetoOntologia <http://gnoss/hasEntidad> ?s .");
            query.AppendLine($"bind(IRI(REPLACE(bif:upper(?sujetoOntologia), '{urlIntragnoss.ToUpper()}', 'http://gnoss/')) as ?sujetoBusqueda) .");
            query.AppendLine(GenerarBloquePrivacidadSujetoBusqueda(pIdentidadID));
            return query.ToString();
        }

        /// <summary>
        /// Dentro el bloque del grafo de búqueda genera el filtro de la privacidad a partir del proyecto y la identidad dada por parámetro
        /// </summary>
        /// <param name="pIdentidadID">Identidad del usuario que está llamando al autocompletar</param>
        /// <param name="pProyectoID">Identificador del proyecto donde se está utilizando el autocompletar</param>
        /// <returns></returns>
        private string GenerarBloquePrivacidadSujetoBusqueda(Guid pIdentidadID)
        {
            IdentidadAD idenAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadAD>(), mloggerFactory);
            List<Guid> listaGruposOrganizacion = idenAD.ObtenerGruposDeOrganizacionDeIdentidad(pIdentidadID);
            StringBuilder query = new StringBuilder();

            query.AppendLine("?sujetoBusqueda gnoss:hasprivacidadCom ?privacidad.");
            query.AppendLine("OPTIONAL { ?sujetoBusqueda gnoss:hasparticipanteIdentidadID ?hasparticipanteIdentidadID }");
            query.AppendLine("OPTIONAL {");
            query.AppendLine(" ?sujetoBusqueda gnoss:hasparticipanteIdentidadID ?grupo.");
            query.AppendLine(" OPTIONAL { ?grupo gnoss:hasparticipanteID  ?hasparticipanteIDGrupo } }");
            query.AppendLine($"FILTER(?privacidad in('publico', 'publicoreg') || ?hasparticipanteIdentidadID = gnoss:{pIdentidadID.ToString().ToUpper()} || ?hasparticipanteIDGrupo = gnoss:{pIdentidadID.ToString().ToUpper()}");

            if (listaGruposOrganizacion.Count > 0)
            {
                query.Append(" || ?grupo in (");
                foreach (Guid grupo in listaGruposOrganizacion)
                {
                    query.Append($"gnoss:{grupo.ToString().ToUpper()},");
                }
                query.Remove(query.Length - 1, 1);
                query.Append(")");
            }
            query.Append(")");

            return query.ToString();
        }

        /// <summary>
        /// Obtiene la url intragnoss del entorno de la base de datos
        /// </summary>
        /// <returns></returns>
        private string ObtenerUrlIntragnoss()
        {
            return mEntityContext.ParametroAplicacion.Where(item => item.Parametro == "UrlIntragnoss").Select(item => item.Valor).FirstOrDefault();
        }

        private static string ObtenerExpresionRegularParaAutocompletar(string pFiltro, string objeto)
        {
            StringBuilder regexSB = new StringBuilder();

            if (!string.IsNullOrEmpty(pFiltro))
            {
                string[] palabras = pFiltro.Split(SEPARADORES_PALABRAS, StringSplitOptions.RemoveEmptyEntries);
                string and = "";
                foreach (string palabra in palabras)
                {
                    // Usamos una expresión regular para que la consulta sea insensible a acentos y mayúsculas (modificador i)
                    regexSB.Append($" {and} (regex({objeto},\"{ObtenerFiltroParaExpresionRegularInsensibleAcentos(palabra)}\", \"i\")) ");
                    and = "AND";
                }
            }

            return regexSB.ToString();
        }

        /// <summary>
        /// Reemplaza en un filtro las letras con acentos y que pueden contener acentos por un patrón del tipo [oóòöô], 
        /// para encontrar cualquier palabra que contena o no acentos, independientemente de cómo lo haya escrito el usuario y cómo estén los datos
        /// </summary>
        /// <param name="pFiltro">Filtro del usuario</param>
        /// <returns>Cadena con el patrón necesario para buscar con insensibilidad de acentos
        /// </returns>
        private static string ObtenerFiltroParaExpresionRegularInsensibleAcentos(string pFiltro)
        {
            string filtroParaRegex = pFiltro.ToLower();

            foreach (string caracteres in CARACTERES_REGEX_ACENTOS)
            {
                foreach (char caracter in caracteres.Where(item => filtroParaRegex.Contains(item)))
                {
                    // Sustituyo por la cadena ##REEMPLAZO## primero, porque si no hará cosas raras en palabras como "monzón": 
                    // en la primera iteración dejaría "m[oóòöô]nzón", y en la segunda dejaría "m[o[oóòöô]òöô]nz[oóòöô]n", 
                    // que evidentemente no es lo que queremos.
                    // Para evitarlo, ponemos siempre la cadena reemplazo y cuando sale del bucle, reemplazamos esa cadena por la real: 
                    // "m##REEMPLAZO##nzón", "m##REEMPLAZO##nz##REEMPLAZO##n"
                    filtroParaRegex = filtroParaRegex.Replace(caracter.ToString(), "##REEMPLAZO##");
                }
                // Y aquí finalmente dejamos la cadena que buscamos: "m[oóòöô]nz[oóòöô]n"
                filtroParaRegex = filtroParaRegex.Replace("##REEMPLAZO##", $"[{caracteres}]");
            }

            return filtroParaRegex;

        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntidadID">ID de la entidad origen</param>
        /// <param name="pConsulta">Consulta a realizar</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerRDFXMLSelectorEntidadFormularioPorConsulta(SelectorEntidad pSelectorEntidad, string pEntidadID, string pConsulta, string pIdioma)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura + " " + pConsulta;
            if (!string.IsNullOrEmpty(pEntidadID))
            {
                query = AjustarParametroEntidadIDConsultaSelectoresEnt(query);
                query = query.Replace("@EntidadID@", "= <" + pEntidadID + ">");
            }

            if (pSelectorEntidad.MultiIdioma)
            {
                if (query.Contains("@GETLANG@") && !string.IsNullOrEmpty(pIdioma))
                {
                    query = query.Replace("@GETLANG@", $"'{pIdioma}'");
                }
                else
                {
                    query = GenerarQuerySelectorEntidadMultiidioma(query);
                }
            }

            LeerDeVirtuoso(query, "SelectPropEnt", facetadoDS, pSelectorEntidad.Grafo);//SelectEnt

            if (facetadoDS.Tables["SelectPropEnt"].Columns.Count > 4)//SelectEnt
            {
                facetadoDS = TrasformarTablaEnTresTripletas(facetadoDS, "SelectPropEnt");//SelectEnt
            }

            return facetadoDS;
        }

        /// <summary>
        /// Realiza un consulta a virtuoso.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pConsulta">Consulta</param>
        /// <returns>Resultado de la consulta a virtuoso</returns>
        public FacetadoDS RealizarConsultaAVirtuoso(string pGrafo, string pConsulta)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura + " " + pConsulta;

            LeerDeVirtuoso(query, "Consulta", facetadoDS, pGrafo);

            return facetadoDS;
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar una entidad.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedoras">IDs de la entidad origen</param>
        /// <param name="pConsulta">Consulta a realizar</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesPorConsulta(string pGrafo, List<string> pEntsContenedoras, string pConsulta)
        {
            pConsulta = AjustarParametroEntidadIDConsultaSelectoresEnt(pConsulta);

            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} {pConsulta}";

            if (pEntsContenedoras.Count == 1)
            {
                query = query.Replace("@EntidadID@", $"= <{pEntsContenedoras[0]}>");
            }
            else
            {
                StringBuilder remplazo = new StringBuilder(" in (");

                foreach (string entidadID in pEntsContenedoras)
                {
                    remplazo.Append($"<{entidadID}>,");
                }

                remplazo.Remove(remplazo.Length - 1, 1);
                remplazo.Append(")");

                query = query.Replace("@EntidadID@", remplazo.ToString());
            }

            LeerDeVirtuoso(query, "SelectPropEnt", facetadoDS, pGrafo);

            if (facetadoDS.Tables["SelectPropEnt"].Columns.Count > 4)
            {
                facetadoDS = TrasformarTablaEnTresTripletas(facetadoDS, "SelectPropEnt");
            }

            return facetadoDS;
        }

        /// <summary>
        /// A partir de la consulta sparql dada en un selector de entidad, se devuelve la misma consulta pero añadiendo una propiedad al select 
        /// para obtener el idioma también a parte del sujeto, predicado y objeto
        /// </summary>
        /// <param name="pQuery">Consulta sparql a la que añadir el idioma</param>
        /// <returns>La consulta dada pero con el parámetro en el select del idioma del objeto</returns>
        private static string GenerarQuerySelectorEntidadMultiidioma(string pQuery)
        {
            //Expresión regex que busca que la consulta cumpla un select con tres parámetros, y se captura el último para poder utilizarlo más adelante
            string regexObjeto = "select[ ]+(?:distinct[ ]+)?\\?[a-zA-Z0-9]+[ ]+\\?[a-zA-Z0-9]+[ ]+(\\?[a-zA-Z0-9]+)[ ]+";
            Match matchObjeto = Regex.Match(pQuery, regexObjeto, RegexOptions.IgnoreCase);

            if (matchObjeto.Success)
            {
                string objetoConsulta = matchObjeto.Groups[1].Value;
                string idioma = $" lang({objetoConsulta}) as ?idioma";

                string regexFrom = "(FROM)";
                Match matchFrom = Regex.Match(pQuery, regexFrom, RegexOptions.IgnoreCase);

                if (matchFrom.Success)
                {
                    string from = matchFrom.Groups[1].Value;
                    pQuery = $"{pQuery.Substring(0, pQuery.IndexOf(from))} {idioma} {pQuery.Substring(pQuery.IndexOf(from))}";
                }
                else
                {
                    string regexWhere = "(WHERE)";
                    Match matchWhere = Regex.Match(pQuery, regexWhere, RegexOptions.IgnoreCase);
                    if (matchWhere.Success)
                    {
                        string where = matchWhere.Groups[1].Value;
                        pQuery = $"{pQuery.Substring(0, pQuery.IndexOf(where))} {idioma} {pQuery.Substring(pQuery.IndexOf(where))}";
                    }
                }
            }

            return pQuery;
        }

        /// <summary>
        /// Ajusta el parametro '@EntidadID@' de una consulta de un selector de entidad.
        /// </summary>
        /// <param name="pConsulta">Consulta</param>
        /// <returns>Consulta con el parámetro ajustado</returns>
        private static string AjustarParametroEntidadIDConsultaSelectoresEnt(string pConsulta)
        {
            return AjustarParametroEntidadIDConsultaSelectoresEnt(pConsulta, "@EntidadID@");
        }

        /// <summary>
        /// Ajusta el parametro '@EntidadID@' de una consulta de un selector de entidad.
        /// </summary>
        /// <param name="pConsulta">Consulta</param>
        /// <param name="pParam">Parametro a ajustar</param>
        /// <returns>Consulta con el parámetro ajustado</returns>
        public static string AjustarParametroEntidadIDConsultaSelectoresEnt(string pConsulta, string pParam)
        {
            string consultaFinal = "";

            while (pConsulta.Contains(pParam))
            {
                int indiceParam = pConsulta.IndexOf(pParam);
                string consulta = pConsulta.Substring(0, indiceParam);
                pConsulta = pConsulta.Substring(indiceParam + pParam.Length);

                indiceParam--;

                while (indiceParam > -1 && consulta[indiceParam] == ' ')
                {
                    indiceParam--;
                }

                if (consulta[indiceParam] == '=')
                {
                    indiceParam--;
                }
                else if (consulta.ToLower()[indiceParam] == 'n' && indiceParam > 0 && consulta.ToLower()[indiceParam - 1] == 'i')
                {
                    indiceParam = indiceParam - 2;
                }

                indiceParam++;

                consulta = consulta.Substring(0, indiceParam) + pParam;
                consultaFinal = string.Concat(consultaFinal, consulta);
            }

            return consultaFinal + pConsulta;
        }

        /// <summary>
        /// Devuelve las entidades grafo dependientes que cumplen un filtro y son hijas de un determindado padre.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pTipoEntDep">Tipo de entidad solicitada</param>
        /// <param name="pIDValorPadre">ID del padre de las entidades filtradas</param>
        /// <param name="pFiltro">Filtro</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresGrafoDependientesFormulario(string pGrafo, string pTipoEntDep, string pIDValorPadre, string pFiltro)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;

            query += " select distinct ?s ?o ";
            query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} ";
            query += " WHERE { ?s dc:title ?o. ?s dc:type ";

            if (pTipoEntDep.StartsWith("http://"))
            {
                query += $"<{pTipoEntDep}>.";
            }
            else
            {
                query += $"'{pTipoEntDep}'.";
            }

            if (!string.IsNullOrEmpty(pIDValorPadre))
            {
                query += $" <{pIDValorPadre}> gnoss:hasChild ?s.";
            }

            if (!string.IsNullOrEmpty(pFiltro))
            {
                query += " FILTER (";

                if (pFiltro[0] == '%')
                {
                    pFiltro = pFiltro.Substring(1);
                    query += $" (bif:lower(str(?o)) LIKE '%{UtilCadenas.ToSparql(pFiltro.ToLower())}%') ";
                }
                else
                {
                    pFiltro = UtilCadenas.ToSparql(pFiltro);
                    query += $" (?o = '{pFiltro}') ";
                }

                query += ")";
            }

            query += "}";


            LeerDeVirtuoso(query, "SelectPropEnt", facetadoDS, pGrafo);//SelectEnt

            return facetadoDS;
        }

        /// <summary>
        /// Devuelve las entidades grafo dependientes con sus valores.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntidades">Entidades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresGrafoDependientesDeEntidades(string pGrafo, List<string> pEntidades)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            query.Append(" select distinct ?s ?o ");
            query.Append($"from {ObtenerUrlGrafo(pGrafo).ToLower()} ");
            query.Append(" WHERE { ?s dc:title ?o. ");
            query.Append(" FILTER (?s in (");

            foreach (string id in pEntidades)
            {
                query.Append($"<{id}>,");
            }

            query.Remove(query.Length - 1, 1);
            query.Append("))}");

            LeerDeVirtuoso(query.ToString(), "SelectPropEnt", facetadoDS, pGrafo);//SelectEnt

            return facetadoDS;
        }

        /// <summary>
        /// Transforma las filas de l tabla del dataset con más de 3 columnas a 3 (tripletas).
        /// </summary>
        /// <param name="pFacetadoDS">DataSet original</param>
        /// <param name="pTabla">Nombre de la tabla</param>
        public static FacetadoDS TrasformarTablaEnTresTripletas(FacetadoDS pFacetadoDS, string pTabla)
        {
            FacetadoDS faceAux = new FacetadoDS();
            faceAux.Tables.Add(pTabla);
            faceAux.Tables[pTabla].Columns.Add("s");
            faceAux.Tables[pTabla].Columns.Add("p");
            faceAux.Tables[pTabla].Columns.Add("o");

            foreach (DataRow fila in pFacetadoDS.Tables[pTabla].Rows)
            {
                DataRow filaNueva = faceAux.Tables[pTabla].NewRow();
                filaNueva["s"] = fila[0];
                filaNueva["p"] = fila[1];
                filaNueva["o"] = fila[2];
                faceAux.Tables[pTabla].Rows.Add(filaNueva);

                for (int i = 3; i < pFacetadoDS.Tables[pTabla].Columns.Count; i = i + 2)
                {
                    DataRow filaNueva2 = faceAux.Tables[pTabla].NewRow();
                    filaNueva2["s"] = fila[0];
                    filaNueva2["p"] = fila[i];
                    filaNueva2["o"] = fila[i + 1];
                    faceAux.Tables[pTabla].Rows.Add(filaNueva2);
                }
            }

            pFacetadoDS.Dispose();
            return faceAux;
        }


        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedoras">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <param name="pEntsContenedoras">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidades(string pGrafo, List<string> pEntsContenedoras, List<string> pPropiedades, bool pTraerIdioma, bool pUsarAfinidad = false)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            if (pEntsContenedoras.Count > 0)
            {
                string query = NamespacesVirtuosoLectura;

                query += " select ?s ?p ?o ";

                if (pTraerIdioma)
                {
                    query += "lang(?o) as ?idioma ";
                }

                query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{";

                List<string> listaPropsJerarquia = new List<string>();
                StringBuilder listaPropsNoJeraquia = new StringBuilder();

                foreach (string propSol in pPropiedades)
                {
                    if (!propSol.Contains("|"))
                    {
                        listaPropsNoJeraquia.Append($"{propSol},");
                    }
                    else
                    {
                        List<string> listaPropiedadesPrincipales = new List<string>();
                        if (propSol.Contains(','))
                        {
                            listaPropiedadesPrincipales = propSol.Split(',').ToList();
                        }
                        else
                        {
                            listaPropiedadesPrincipales.Add(propSol);
                        }

                        foreach (string propiedad in listaPropiedadesPrincipales)
                        {
                            string propiedadPrincipal = propiedad;
                            if (propiedad.Contains("|"))
                            {
                                propiedadPrincipal = propiedad.Substring(0, propiedad.IndexOf("|"));
                                listaPropsJerarquia.Add(propiedad);
                            }
                            if (!listaPropsNoJeraquia.ToString().Contains($"{propiedadPrincipal},"))
                            {
                                listaPropsNoJeraquia.Append($"{propiedadPrincipal},");
                            }

                            listaPropsJerarquia.AddRange(ObtenerRelacionesDirectas(propiedad));
                        }
                    }
                }

                int offset = 0;
                while (pEntsContenedoras.Count > offset)
                {
                    FacetadoDS facetadoAux = new FacetadoDS();
                    StringBuilder queryBucle = new StringBuilder(query);
                    if (listaPropsNoJeraquia.Length > 0)
                    {
                        queryBucle.Append($"{ObtenerWhereValoresPropiedadesEntidades(pEntsContenedoras.Skip(offset).Take(1000).ToList(), listaPropsNoJeraquia.ToString())} UNION ");
                    }

                    foreach (string propJer in listaPropsJerarquia)
                    {
                        queryBucle.Append($"{ObtenerWhereValoresPropiedadesEntidades(pEntsContenedoras.Skip(offset).Take(1000).ToList(), propJer)} UNION ");
                    }

                    queryBucle.Remove(queryBucle.Length - 6, 6);
                    queryBucle.Append(" }");

                    LeerDeVirtuoso(queryBucle.ToString(), "SelectPropEnt", facetadoAux, pGrafo, pUsarAfinidad);
                    facetadoDS.Merge(facetadoAux);
                    facetadoAux.Dispose();
                    offset += 1000;
                }
            }
            return facetadoDS;
        }

        private static List<string> ObtenerRelacionesDirectas(string pPropiedad)
        {
            List<string> listaEntidades = new List<string>();
            string[] entidades = pPropiedad.Split('|');
            for (int i = 0; i < entidades.Length - 1; i++)
            {
                listaEntidades.Add($"{entidades[i]}|{entidades[i + 1]}");
            }
            return listaEntidades;
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pTraerIdioma">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesTodas(string pGrafo, List<string> pPropiedades, bool pTraerIdioma, string pIdioma = "es")
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            string namespacesVirtuoso = NamespacesVirtuosoLectura;

            string query = " select ?s ?p ?o ";
            string orderBy = "order by ?s ?p ?o";

            if (pTraerIdioma)
            {
                query += "lang(?o) as ?idioma ";
                orderBy += " ?idioma";
            }

            query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{";

            List<string> listaPropsJerarquia = new List<string>();
            StringBuilder listaPropsNoJeraquia = new StringBuilder();

            foreach (string propSol in pPropiedades)
            {
                if (!propSol.Contains("|"))
                {
                    listaPropsNoJeraquia.Append($"{propSol},");
                }
                else
                {
                    listaPropsNoJeraquia.Append($"{propSol.Substring(0, propSol.IndexOf("|"))},");
                    listaPropsJerarquia.Add(propSol);
                }
            }

            FacetadoDS facetadoAux = new FacetadoDS();

            //Se añade un filtro con el idioma y con el tipo source para solamente traer las taxonomias del idioma que estemos utilizando. (Si no es excesivamente grande la consulta)
            StringBuilder queryBucle = new StringBuilder($"{query} ?s <http://purl.org/dc/elements/1.1/source> ?xx. FILTER(lang(?o)='{pIdioma}' OR lang(?o)='' OR isUri(?o))");

            if (listaPropsNoJeraquia.Length > 0)
            {
                queryBucle.Append($"{ObtenerWhereValoresPropiedadesEntidadesTodas(listaPropsNoJeraquia.ToString())} UNION ");
            }

            foreach (string propJer in listaPropsJerarquia)
            {
                queryBucle.Append($"{ObtenerWhereValoresPropiedadesEntidadesTodas(propJer)} UNION ");
            }

            queryBucle.Remove(queryBucle.Length - 6, 6);
            queryBucle.Append("}");

            queryBucle.Append(orderBy);

            LeerDeVirtuoso(namespacesVirtuoso + queryBucle + " limit 10000", "SelectPropEnt", facetadoAux, pGrafo);
            int filas = facetadoAux.Tables["SelectPropEnt"].Rows.Count;
            facetadoDS.Merge(facetadoAux);
            int iteracion = 1;
            while (filas == 10000)
            {
                string queryBucleWhile = $"{namespacesVirtuoso} select * where {{{queryBucle}}} offset {iteracion * 10000} limit 10000";
                LeerDeVirtuoso(queryBucleWhile, "SelectPropEnt", facetadoAux, pGrafo);
                facetadoDS.Merge(facetadoAux);
                filas = facetadoAux.Tables["SelectPropEnt"].Rows.Count;
                facetadoAux.Dispose();
                iteracion++;
            }

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene la parte del where para la consulta para obtener valores de propiedades de entidades.
        /// </summary>
        /// <param name="pEntsContenedoras">Entidades sujeto</param>
        /// <param name="pPropiedades">Propiedades a traer</param>
        /// <returns>parte del where para la consulta para obtener valores de propiedades de entidades</returns>
        private static string ObtenerWhereValoresPropiedadesEntidadesTodas(string pPropiedades)
        {
            StringBuilder where = new StringBuilder();
            StringBuilder filtros = new StringBuilder();
            string[] propsPorNivel = pPropiedades.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int nivelMaximo = (propsPorNivel.Length - 1);

            int count = 0;

            for (int i = nivelMaximo; i >= 0; i--)
            {
                where.Append($"?s{i} ?p{i}");

                if (i != 0)
                {
                    where.Append($" ?s{(i - 1)}. ");
                }
                else
                {
                    where.Append($" ?o{i}. ");
                }

                string[] propDeNivel = propsPorNivel[count].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                filtros.Append("(");

                foreach (string prop in propDeNivel)
                {
                    filtros.Append($"?p{i}=<{prop}> OR ");
                }

                filtros.Remove(filtros.Length - 3, 3);
                filtros.Append(") AND");
                count++;
            }

            filtros.Remove(filtros.Length - 4, 4);

            where.Append($"FILTER ({filtros})");

            return $"{{{where.Replace("?s0", "?s").Replace("?p0", "?p").Replace("?o0", "?o")}}}";
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pEntsContenedoras">Indica si hay que traer el idoma de los triples</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidades(string pGrafo, List<string> pEntsContenedoras, bool pTraerIdioma)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            query.Append(" select ?s ?p ?o ");

            if (pTraerIdioma)
            {
                query.Append("lang(?o) as ?idioma ");
            }

            query.Append($"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{?s ?p ?o. FILTER (");

            if (pEntsContenedoras.Count == 1)
            {
                query.Append($"?s = <{pEntsContenedoras[0]}> ");
            }
            else
            {
                query.Append("?s in (");

                foreach (string entidadID in pEntsContenedoras)
                {
                    query.Append($"<{entidadID}>,");
                }

                query.Remove(query.Length - 1, 1);
                query.Append(")");
            }

            query.Append(")}");

            LeerDeVirtuoso(query.ToString(), "SelectPropEnt", facetadoDS, pGrafo);

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene la parte del where para la consulta para obtener valores de propiedades de entidades.
        /// </summary>
        /// <param name="pEntsContenedoras">Entidades sujeto</param>
        /// <param name="pPropiedades">Propiedades a traer</param>
        /// <returns>parte del where para la consulta para obtener valores de propiedades de entidades</returns>
        private static string ObtenerWhereValoresPropiedadesEntidades(List<string> pEntsContenedoras, string pPropiedades)
        {
            StringBuilder where = new StringBuilder();
            StringBuilder filtros = new StringBuilder();
            string[] propsPorNivel = pPropiedades.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            int nivelMaximo = (propsPorNivel.Length - 1);

            if (pEntsContenedoras.Count == 1)
            {
                filtros.Append($"?s{nivelMaximo} = <{pEntsContenedoras[0]}> AND ");
            }
            else
            {
                filtros.Append($"?s{nivelMaximo} in (");

                foreach (string entidadID in pEntsContenedoras)
                {
                    filtros.Append($"<{entidadID}>,");
                }

                filtros.Remove(filtros.Length - 1, 1);

                filtros.Append($") AND ");
            }

            int count = 0;

            for (int i = nivelMaximo; i >= 0; i--)
            {
                where.Append($"?s{i} ?p{i}");

                if (i != 0)
                {
                    where.Append($" ?s{(i - 1)}. ");
                }
                else
                {
                    where.Append($" ?o{i}. ");
                }

                string[] propDeNivel = propsPorNivel[count].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                filtros.Append("(");

                foreach (string prop in propDeNivel)
                {
                    filtros.Append($"?p{i}=<{prop}> OR ");
                }

                filtros.Remove(filtros.Length - 3, 3);
                filtros.Append(") AND ");

                count++;
            }

            filtros.Remove(filtros.Length - 4, 4);

            where.Append($"FILTER ({filtros})");

            return $"{{{where.Replace("?s0", "?s").Replace("?p0", "?p").Replace("?o0", "?o")}}}";
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades anidadas.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pEntsContenedora">Entidades contenedoras</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesAnidadas(string pGrafo, List<string> pEntsContenedoras, List<string> pPropiedades, bool pUsarAfinidad = false
)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);

            for (int i = 0; i < pEntsContenedoras.Count; i++)
            {
                string entidadContenedora = pEntsContenedoras[i].Substring(0, pEntsContenedoras[i].LastIndexOf("_"));
                pEntsContenedoras[i] = mUrlIntranet + entidadContenedora.Substring(entidadContenedora.LastIndexOf("_") + 1);
            }

            query.Append(" select ?s ?p ?o ");

            query.Append($"from {ObtenerUrlGrafo(pGrafo).ToLower()} ");

            if (pEntsContenedoras.Count == 1)
            {
                query.Append($" WHERE {{ ?s2 ?p2 ?s. ?s ?p ?o. FILTER (?s2 = <{pEntsContenedoras[0]}> AND (");
            }
            else
            {
                query.Append(" WHERE { ?s2 ?p2 ?s. ?s ?p ?o. FILTER (?s2 in (");

                foreach (string entidadID in pEntsContenedoras)
                {
                    query.Append($"<{entidadID}>,");
                }

                query.Remove(query.Length - 1, 1);
                query.Append(") AND (");
            }

            foreach (string propSol in pPropiedades)
            {
                query.Append($"?p=<{propSol}> OR ");
            }

            query.Remove(query.Length - 3, 3);
            query.Append("))  ");

            query.Append("  }");

            LeerDeVirtuoso(query.ToString(), "SelectPropEnt", facetadoDS, pGrafo, pUsarAfinidad);

            return facetadoDS;
        }

        /// <summary>
        /// Devuelve el trozo del RDF de un formulario semántico perteneciente al filtro para seleccionar unas entidades.
        /// </summary>
        /// <param name="pGrafo">Grafo de la consulta</param>
        /// <param name="pListaDocumentosID">Listado de documentos</param>
        /// <param name="pPropiedades">Propiedades solicitadas</param>
        /// <returns>DataSet con las tripletas</returns>
        public FacetadoDS ObtenerValoresPropiedadesEntidadesPorDocumentoID(string pGrafo, List<Guid> pListaDocumentosID, List<string> pPropiedades, string pIdioma, bool pUsarClienteWeb, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluirPersonas, bool pOmitirPalabrasNoRelevantesSearch, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEstaEnMyGnoss, bool pEsInvitado, string pIdentidadID, bool pEsExportacionExcel = false, bool pUsarAfinidad = false)
        {
            FacetadoDS facetadoDSAux = new FacetadoDS();

            StringBuilder query = new StringBuilder($" from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{");

            string[] LNiveles = null;
            string[] stringSeparators = new string[] { "@@@", "RRR" };

            int maxmaxniveles = 0;
            int propActual = 0;
            Dictionary<string, int> listaNiveles = new Dictionary<string, int>();

            query.Append(" {  ?s rdf:type ?o.    }  ");

            if (pListaFiltros != null)
            {
                query.Append(" { ");

                query.Append(ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, pExcluirPersonas, pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion.Ninguno, pFiltrosSearchPersonalizados, false));

                query.Append(ObtenerBloqueHasPrivacidad(pEstaEnMyGnoss, pEsMiembroComunidad, pIdentidadID, pProyectoID, true));

                query.Append(" } ");
            }

            query.Append(" { ?s rdf:type ?rdfTypeAux } ");

            foreach (string propSolOriginal in pPropiedades)
            {
                query.Append(" UNION  ");
                query.Append("  { ");

                string[] propiedades = propSolOriginal.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                int propAxctualInt = 0;
                Dictionary<string, int> dicIndiceProp = new Dictionary<string, int>();
                foreach (string propSol in propiedades)
                {
                    if (propiedades.Length > 1)
                    {
                        if (propAxctualInt > 0)
                        {
                            query.Append("  OPTIONAL ");
                        }
                        query.Append("  { ");
                    }

                    string propSolAux = propSol;

                    string idioma = "";
                    // Obtenbemos los que no tenga idioma y lo que tenga el idioma actual
                    propSolAux = propSolAux.Replace("[MultiIdioma]", "");
                    if (!string.IsNullOrEmpty(pIdioma))
                    {
                        idioma = " AND ( !isLiteral(?o" + propActual + ") || lang(?o" + propActual + ") in('" + pIdioma.ToLower() + "','') )";
                    }

                    int maxniveles = 0;

                    LNiveles = propSolAux.Split(stringSeparators, StringSplitOptions.None);

                    int reciproca = 0;
                    if (propSol.Contains("RRR"))
                    {
                        foreach (string nivel in LNiveles)
                        {
                            reciproca++;
                            if (propSol.Contains(nivel + "RRR"))
                            {
                                break;
                            }
                        }
                    }

                    StringBuilder nivelAux = new StringBuilder();
                    foreach (string nivel in LNiveles)
                    {
                        if (nivelAux.Length > 0)
                        {
                            nivelAux.Append($"@@@{nivel}");
                        }
                        else
                        {
                            nivelAux = new StringBuilder(nivel);
                        }
                        if (!listaNiveles.ContainsKey(nivelAux.ToString()))
                        {
                            listaNiveles.Add(nivelAux.ToString(), listaNiveles.Count);
                        }
                    }

                    if (propSolAux.Contains("@@@") || propSolAux.Contains("RRR"))
                    {
                        if (reciproca > 0)
                        {
                            if (LNiveles[0].StartsWith("http:"))
                            {
                                int nivelActual = listaNiveles[LNiveles[0]];
                                query.Append($" ?nivel{nivelActual} ?p{nivelActual} ?s. FILTER (?p{nivelActual}=<{LNiveles[0]}> )");
                            }
                            else
                            {
                                int nivelActual = listaNiveles[LNiveles[0]];
                                query.Append($" ?nivel{nivelActual} ?p{nivelActual} ?s. FILTER (?p{nivelActual}={LNiveles[0]} )");
                            }
                        }
                        else
                        {
                            if (LNiveles[0].StartsWith("http:"))
                            {
                                int nivelActual = listaNiveles[LNiveles[0]];
                                query.Append($"?s ?p{nivelActual} ?nivel{nivelActual} . FILTER (?p{nivelActual}=<{LNiveles[0]}> )");
                            }
                            else
                            {
                                int nivelActual = listaNiveles[LNiveles[0]];
                                query.Append($"?s ?p{nivelActual} ?nivel{nivelActual}. FILTER (?p{nivelActual}={LNiveles[0]} )");
                            }
                        }
                        int i = 0;
                        StringBuilder nivelAux2 = new StringBuilder();
                        string nivelAux2MasUno = "";
                        for (i = 0; i < LNiveles.Length - 2; i++)
                        {
                            if (nivelAux2.Length > 0)
                            {
                                nivelAux2.Append($"@@@{LNiveles[i]}");
                            }
                            else
                            {
                                nivelAux2 = new StringBuilder(LNiveles[i]);
                            }
                            nivelAux2MasUno = nivelAux2 + "@@@" + LNiveles[i + 1];

                            int nivelActual = listaNiveles[nivelAux2.ToString()];
                            int nivelMasUno = listaNiveles[nivelAux2MasUno];
                            if (reciproca > i + 1)
                            {
                                if (LNiveles[i + 1].StartsWith("http:"))
                                {
                                    query.Append($" ?nivel{nivelMasUno} ?p{nivelMasUno} ?nivel{nivelActual}. FILTER (?p{nivelMasUno}=<{LNiveles[i + 1]}> )");
                                }
                                else
                                {
                                    query.Append($" ?nivel{nivelMasUno} ?p{nivelMasUno} ?nivel{nivelActual}. FILTER (?p{nivelMasUno}={LNiveles[i + 1]} )");
                                }
                            }
                            else
                            {
                                if (LNiveles[i + 1].StartsWith("http:"))
                                {
                                    query.Append($" ?nivel{nivelActual} ?p{nivelMasUno} ?nivel{nivelMasUno}. FILTER (?p{nivelMasUno}=<{LNiveles[i + 1]}> )");
                                }
                                else
                                {
                                    query.Append($" ?nivel{nivelActual} ?p{nivelMasUno} ?nivel{nivelMasUno}. FILTER (?p{nivelMasUno}={LNiveles[i + 1]} )");
                                }
                            }
                            maxniveles++;
                        }
                        if (nivelAux2.Length > 0)
                        {
                            nivelAux2.Append($"@@@{LNiveles[i]}");
                        }
                        else
                        {
                            nivelAux2 = new StringBuilder(LNiveles[i]);
                        }
                        nivelAux2MasUno = nivelAux2 + "@@@" + LNiveles[i + 1];
                        if (LNiveles[i + 1].StartsWith("http:"))
                        {
                            int nivelActual = listaNiveles[nivelAux2.ToString()];
                            int nivelActualMasUno = listaNiveles[nivelAux2MasUno];
                            query.Append($" ?nivel{nivelActual} ?p{nivelActualMasUno} ?o{propActual}. FILTER (?p{nivelActualMasUno}=<{LNiveles[i + 1]}> {idioma} )");
                        }
                        else
                        {
                            int nivelActual = listaNiveles[nivelAux2.ToString()];
                            int nivelActualMasUno = listaNiveles[nivelAux2MasUno];
                            query.Append($" ?nivel{nivelActual} ?p{nivelActualMasUno} ?o{propActual}. FILTER (?p{nivelActualMasUno}={LNiveles[i + 1]} {idioma} )");
                        }
                        maxniveles++;
                    }
                    else
                    {
                        int nivelActual = listaNiveles[propSolAux];
                        if (propSolAux.StartsWith("http:"))
                        {
                            query.Append($" ?s ?p{nivelActual} ?o{propActual}. FILTER(?p{nivelActual}=<{propSolAux.Replace("@@@", "")}> {idioma} ) ");
                        }
                        else
                        {
                            query.Append($" ?s ?p{nivelActual} ?o{propActual}. FILTER(?p{nivelActual}={propSolAux.Replace("@@@", "")} {idioma} ) ");
                        }
                    }
                    if (maxniveles > maxmaxniveles) { maxmaxniveles = maxniveles; }

                    query.Append(" }  ");
                    dicIndiceProp.Add(propSol, propActual);
                    propActual++;
                    propAxctualInt++;
                }
                if (propiedades.Length > 1)
                {
                    //Establecemos las igualdades entre los finales y los niveles (en caso de que alguna propiedad este contenida en otra)
                    foreach (string propiedad in propiedades)
                    {
                        if (propiedades.ToList().Exists(x => (x == propiedad || x.StartsWith(propiedad + "@@@")) && x != propiedad))
                        {
                            //Si existe alguna propiedad que contenga por completo a otra propiedad tenemos que igualarl la propiedad que hace referencia al nivel correspondiente
                            query.Append($"FILTER(?nivel{listaNiveles[propiedad]} = ?o{dicIndiceProp[propiedad]})");
                        }
                    }
                    query.Append(" } ");
                }
            }

            if (pListaDocumentosID.Any())
            {
                query.Append("  FILTER ( ?s in (");

                foreach (Guid entidadID in pListaDocumentosID)
                {
                    query.Append($"gnoss:{entidadID.ToString().ToUpper()}, ");
                }

                query.Remove(query.Length - 2, 2);
                query.Append(")) ");
            }

            StringBuilder select = new StringBuilder(" select distinct ?s ");
            int propAcumuladas = 0;

            Dictionary<string, string> listaNamespaces = new Dictionary<string, string>();

            foreach (string clave in InformacionOntologias.Keys)
            {
                foreach (string ns in InformacionOntologias[clave])
                {
                    if (clave.StartsWith('@'))
                    {
                        if (!listaNamespaces.ContainsKey(ns))
                        {
                            listaNamespaces.Add(ns, clave.Substring(1));
                        }
                    }
                    else
                    {
                        if (!listaNamespaces.ContainsKey(ns))
                        {
                            listaNamespaces.Add(ns, mUrlIntranet + "Ontologia/" + clave + ".owl#");
                        }
                    }
                }
            }

            foreach (string clave in FacetadoAD.ListaNamespacesBasicos.Keys)
            {
                if (!listaNamespaces.ContainsKey(clave))
                {
                    listaNamespaces.Add(clave, ListaNamespacesBasicos[clave]);
                }
            }

            for (int i = 0; i < pPropiedades.Count; i++)
            {
                #region Propiedad

                StringBuilder tempSelect = new StringBuilder();

                string[] props = pPropiedades[i].Replace("[MultiIdioma]", "").Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string prop in props)
                {
                    string[] niveles = prop.Split(stringSeparators, StringSplitOptions.None);
                    for (int j = 0; j < niveles.Length; j++)
                    {
                        string strNiv = niveles[j];
                        if (strNiv.Contains(":"))
                        {
                            string pref = strNiv.Substring(0, strNiv.IndexOf(":"));
                            if (listaNamespaces.ContainsKey(pref))
                            {
                                strNiv = strNiv.Replace(pref + ":", listaNamespaces[pref]);
                            }
                        }
                        if (tempSelect.Length > 0)
                        {
                            if (j == 0)
                            {
                                tempSelect.Append(strNiv);
                            }
                            else
                            {
                                tempSelect.Append($"@@@{strNiv}");
                            }
                        }
                        else
                        {
                            tempSelect.Append($" '{strNiv}");
                        }
                    }
                    tempSelect.Append("||");
                }
                tempSelect.Remove(tempSelect.Length - 2, 2);
                tempSelect.Append("' ");

                tempSelect.Append($" as ?prop{i}");

                select.Append(tempSelect.ToString());

                #endregion Propiedad

                #region Valor

                if (props.Length > 1)
                {
                    tempSelect = new StringBuilder(" bif:concat(");
                    for (int k = 0; k < props.Length; k++)
                    {
                        tempSelect.Append($" ?o{(i + propAcumuladas + k)}");
                        tempSelect.Append(",'||',");
                    }
                    propAcumuladas += props.Length - 1;
                    tempSelect.Remove(tempSelect.Length - 6, 6);
                    tempSelect.Append(")");

                    if (pUsarClienteWeb)
                    {
                        tempSelect = new StringBuilder($" REPLACE(str({tempSelect}), '\\'\\'', '\"')");
                    }
                }
                else
                {
                    tempSelect = new StringBuilder($" ?o{(i + propAcumuladas)}");

                    if (!pUsarClienteWeb)
                    {
                        tempSelect = new StringBuilder($" REPLACE(str({tempSelect}), '\\'\\'', '\"')");
                    }
                }

                tempSelect.Append($" as ?val{i}");
                tempSelect.Append($" lang(?val{i}) as ?idioma{i}");

                select.Append(tempSelect);

                #endregion Valor

                if (pEsExportacionExcel)
                {
                    #region Relacion

                    tempSelect = new StringBuilder(" ?s");

                    if (props.Length > 0)
                    {
                        tempSelect = new StringBuilder(" bif:concat(?s,'@@@',");

                        foreach (string prop in props)
                        {
                            string[] niveles = prop.Split(stringSeparators, StringSplitOptions.None);
                            StringBuilder nivelAux = new StringBuilder();
                            for (int j = 0; j < niveles.Length - 1; j++)
                            {
                                if (nivelAux.Length > 0)
                                {
                                    nivelAux.Append($"@@@{niveles[j]}");
                                }
                                else
                                {
                                    nivelAux = new StringBuilder(niveles[j]);
                                }
                                tempSelect.Append($"?nivel{listaNiveles[nivelAux.ToString()]},'@@@',");
                            }
                            tempSelect.Remove(tempSelect.Length - 7, 7);
                            tempSelect.Append(",'||',");
                        }
                        tempSelect.Remove(tempSelect.Length - 6, 6);
                        tempSelect.Append(")");

                        if (tempSelect.Equals(" bif:concat(?s)"))
                        {
                            // No concatena nada, le quito la función
                            tempSelect = new StringBuilder(" ?s");
                        }
                    }

                    if (!pUsarClienteWeb && !tempSelect.Equals(" ?s")) // Si se selecciona solo el sujeto, no hace falta el replace
                    {
                        tempSelect = new StringBuilder(" REPLACE(" + tempSelect + ", '\\'\\'', '\"')");
                    }

                    tempSelect.Append($" as ?relacion{i}");

                    select.Append(tempSelect);

                    #endregion Relacion

                    #region Nivel

                    select.Append($" {i} as ?level{i}");

                    #endregion
                }
            }

            query.Append("  }");
            string finalQuery = $"{NamespacesVirtuosoLectura} {select} {query}";

            string nombreTabla = "SelectPropEnt";
            if (pUsarClienteWeb)
            {
                LeerDeVirtuoso(finalQuery, nombreTabla, facetadoDSAux, pGrafo, pUsarAfinidad);
            }
            else
            {
                VirtuosoAD virtuosoAD = new VirtuosoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<VirtuosoAD>(),mloggerFactory);

                // los carácteres especiales deben ir en UTF-8
                UtilCadenas.PasarAUtf8(finalQuery);

                DataSet ds = virtuosoAD.LeerDeVirtuoso(finalQuery, nombreTabla);
                virtuosoAD.Dispose();

                facetadoDSAux.Tables.Add(nombreTabla);
                facetadoDSAux.Tables[nombreTabla].Merge(ds.Tables[nombreTabla]);
            }

            FacetadoDS facetadoDS;
            if (pEsExportacionExcel)
            {
                facetadoDS = ObtenerValoresPropiedasEntidadesPorDocumentoID_Exportacion(facetadoDSAux, pPropiedades, pUsarClienteWeb, pIdioma);
            }
            else
            {
                facetadoDS = ObtenerValoresPropiedasEntidadesPorDocumentoID_Normal(facetadoDSAux, pPropiedades, pUsarClienteWeb, pIdioma);
            }

            return facetadoDS;
        }

        /// <summary>
        ///  Devuelve los datos configurados personalizados
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pListaDocumentosID">Lista de documentos</param>
        /// <param name="pNombreTabla">Nombre de la tabla</param>
        /// <param name="pSelect">Select</param>
        /// <param name="pWhere">Where</param>
        /// <param name="pUsarClienteWeb"></param>
        /// <returns></returns>
        public FacetadoDS ObtenerValoresPropiedadesPersonalizadasEntidadesPorDocumentoID(string pGrafo, List<Guid> pListaDocumentosID, string pNombreTabla, string pSelect, string pWhere, bool pUsarClienteWeb, string pIdioma)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} {pSelect} from {ObtenerUrlGrafo(pGrafo).ToLower()} {pWhere}";
            if (pListaDocumentosID.Any())
            {
                string resourceIds = $"FILTER(?s in(gnoss:{string.Join(",gnoss:", pListaDocumentosID.Select(x => x.ToString().ToUpper()))}))";
                query = query.Replace("<FILTER_RESOURCE_IDS>", resourceIds);
                query = query.Replace("<LANG>", pIdioma);
                if (pUsarClienteWeb)
                {
                    LeerDeVirtuoso(query, pNombreTabla, facetadoDS, pGrafo);
                }
                else
                {
                    VirtuosoAD virtuosoAD = new VirtuosoAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<VirtuosoAD>(),mloggerFactory);

                    // los carácteres especiales deben ir en UTF-8
                    query = UtilCadenas.PasarAUtf8(query);

                    DataSet ds = virtuosoAD.LeerDeVirtuoso(query, pNombreTabla);
                    virtuosoAD.Dispose();

                    facetadoDS.Tables.Add(pNombreTabla);
                    facetadoDS.Tables[pNombreTabla].Merge(ds.Tables[pNombreTabla]);
                }
            }
            return facetadoDS;
        }

        private static FacetadoDS ObtenerValoresPropiedasEntidadesPorDocumentoID_Normal(FacetadoDS facetadoDSAux, List<string> pPropiedades, bool pUsarClienteWeb, string pIdioma)
        {
            Dictionary<string, Dictionary<string, List<KeyValuePair<string, string>>>> listaPropiedadesDoc = new Dictionary<string, Dictionary<string, List<KeyValuePair<string, string>>>>();
            foreach (DataRow fila in facetadoDSAux.Tables["SelectPropEnt"].Rows)
            {
                for (int i = 0; i < pPropiedades.Count; i++)
                {
                    string sujeto = fila["s"].ToString();
                    string propiedad = fila["prop" + i].ToString();
                    string idioma = fila["idioma" + i].ToString();

                    string valor = fila["val" + i].ToString();
                    if (!pUsarClienteWeb)
                    {
                        valor = UtilCadenas.PasarAANSI(valor);
                    }

                    if (!string.IsNullOrEmpty(valor))
                    {
                        if (listaPropiedadesDoc.ContainsKey(sujeto))
                        {
                            if (listaPropiedadesDoc[sujeto].ContainsKey(propiedad))
                            {
                                if (!listaPropiedadesDoc[sujeto][propiedad].Contains(new KeyValuePair<string, string>(valor, idioma)))
                                {
                                    listaPropiedadesDoc[sujeto][propiedad].Add(new KeyValuePair<string, string>(valor, idioma));
                                }
                            }
                            else
                            {
                                List<KeyValuePair<string, string>> valores = new List<KeyValuePair<string, string>>();
                                valores.Add(new KeyValuePair<string, string>(valor, idioma));
                                listaPropiedadesDoc[sujeto].Add(propiedad, valores);
                            }
                        }
                        else
                        {
                            Dictionary<string, List<KeyValuePair<string, string>>> props = new Dictionary<string, List<KeyValuePair<string, string>>>();
                            List<KeyValuePair<string, string>> valores = new List<KeyValuePair<string, string>>();
                            valores.Add(new KeyValuePair<string, string>(valor, idioma));
                            props.Add(propiedad, valores);
                            listaPropiedadesDoc.Add(sujeto, props);
                        }
                    }
                }
            }

            FacetadoDS facetadoDS = new FacetadoDS();
            facetadoDS.Tables.Add("SelectPropEnt");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("s");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("p");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("o");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("idioma");

            foreach (string sujeto in listaPropiedadesDoc.Keys)
            {
                foreach (string propiedad in listaPropiedadesDoc[sujeto].Keys)
                {
                    foreach (KeyValuePair<string, string> valorIdioma in listaPropiedadesDoc[sujeto][propiedad])
                    {
                        if (string.IsNullOrEmpty(valorIdioma.Value) || string.IsNullOrEmpty(pIdioma) || valorIdioma.Value.Equals(pIdioma))
                        {
                            facetadoDS.Tables["SelectPropEnt"].Rows.Add(sujeto, propiedad, valorIdioma.Key, valorIdioma.Value);
                        }
                    }
                }
            }
            return facetadoDS;
        }

        private static FacetadoDS ObtenerValoresPropiedasEntidadesPorDocumentoID_Exportacion(FacetadoDS facetadoDSAux, List<string> pPropiedades, bool pUsarClienteWeb, string pIdioma)
        {
            Dictionary<string, Dictionary<string, List<KeyValuePair<string, string>>>> listaPropiedadesDoc = new Dictionary<string, Dictionary<string, List<KeyValuePair<string, string>>>>();
            foreach (DataRow fila in facetadoDSAux.Tables["SelectPropEnt"].Rows)
            {
                for (int i = 0; i < pPropiedades.Count; i++)
                {
                    string sujeto = fila["s"].ToString();
                    string propiedad = fila["prop" + i].ToString();

                    string valor = fila["val" + i].ToString();
                    if (!pUsarClienteWeb)
                    {
                        valor = UtilCadenas.PasarAANSI(valor);
                    }
                    string relacion = fila["relacion" + i].ToString();
                    string level = fila["level" + i].ToString();
                    relacion = relacion + "|" + level;
                    string idioma = fila["idioma" + i].ToString();
                    relacion = relacion + "|" + idioma;
                    if (!string.IsNullOrEmpty(valor))
                    {
                        if (listaPropiedadesDoc.ContainsKey(sujeto))
                        {
                            if (listaPropiedadesDoc[sujeto].ContainsKey(propiedad))
                            {
                                if (!listaPropiedadesDoc[sujeto][propiedad].Contains(new KeyValuePair<string, string>(valor, relacion)))
                                {
                                    listaPropiedadesDoc[sujeto][propiedad].Add(new KeyValuePair<string, string>(valor, relacion));
                                }
                            }
                            else
                            {
                                List<KeyValuePair<string, string>> valores = new List<KeyValuePair<string, string>>();
                                valores.Add(new KeyValuePair<string, string>(valor, relacion));
                                listaPropiedadesDoc[sujeto].Add(propiedad, valores);
                            }
                        }
                        else
                        {
                            Dictionary<string, List<KeyValuePair<string, string>>> props = new Dictionary<string, List<KeyValuePair<string, string>>>();
                            List<KeyValuePair<string, string>> valores = new List<KeyValuePair<string, string>>();
                            valores.Add(new KeyValuePair<string, string>(valor, relacion));
                            props.Add(propiedad, valores);
                            listaPropiedadesDoc.Add(sujeto, props);
                        }
                    }
                }
            }


            FacetadoDS facetadoDS = new FacetadoDS();
            facetadoDS.Tables.Add("SelectPropEnt");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("s");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("p");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("o");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("relacion");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("level");
            facetadoDS.Tables["SelectPropEnt"].Columns.Add("idioma");

            foreach (string sujeto in listaPropiedadesDoc.Keys)
            {
                foreach (string propiedad in listaPropiedadesDoc[sujeto].Keys)
                {
                    foreach (KeyValuePair<string, string> valorRelacion in listaPropiedadesDoc[sujeto][propiedad])
                    {
                        string valor = valorRelacion.Value;
                        int nivel = 0;
                        string idioma = string.Empty;

                        string[] delimiter = { "|" };
                        if (valor.Contains("|"))
                        {
                            string[] seccionesValorRelaceion = valorRelacion.Value.Split(delimiter, StringSplitOptions.None);
                            valor = seccionesValorRelaceion[0];
                            int.TryParse(seccionesValorRelaceion[1], out nivel);
                            idioma = seccionesValorRelaceion[seccionesValorRelaceion.Length - 1];
                        }

                        // Sólo se añade la fila si el idioma es nulo o es igual al del usuario. 
                        if (string.IsNullOrEmpty(idioma) || idioma.Equals(pIdioma))
                        {
                            facetadoDS.Tables["SelectPropEnt"].Rows.Add(sujeto, propiedad, valorRelacion.Key, valor, nivel, idioma);
                        }
                    }
                }
            }
            return facetadoDS;
        }

        /// <summary>
        /// Obtiene los recursos relacionados con el actual para montar un grafo gráfico.
        /// </summary>
        /// <param name="pGrafo">Grafo de búsqueda</param>
        /// <param name="pDocumentoID">ID del documento actual</param>
        /// <param name="pPropEnlace">Propiedad de enlace entre recursos</param>
        /// <param name="pNodosLimiteNivel">Número de nodos a partir del cual hay que detenerse</param>
        /// <param name="pExtra">Configuración extra</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pTipoRecurso">Tipo del recurso actual</param>
        /// <returns>Array JS con las relacionados con el actual para montar un grafo gráfico</returns>
        public string ObtenerRelacionesGrafoGraficoDeDocumento(string pGrafo, Guid pDocumentoID, string pPropEnlace, int pNodosLimiteNivel, string pExtra, string pIdioma, string pTipoRecurso, string pGrafoDbpedia = null)
        {
            List<string> tiposAgrupar = new List<string>();
            string propAgrupacion = null;
            int numAgrupar = 0;
            List<string> subTiposAgrupar = new List<string>();
            string propAgrupacionSubTipo = null;
            string propSubAgrupacion = null;
            string propSubAgrupacionBusqueda = null;
            int numSubAgrupar = 0;
            FacetadoDS facetadoDS = new FacetadoDS();

            List<string> tiposReciprocos = new List<string>();
            int? numNodosMaxSuj = null;

            List<string> tiposPropsEliminarPorTipo = new List<string>();
            List<string> propsEliminarPorTipo = new List<string>();
            List<string> tiposPropsEliminarPorSubTipo = new List<string>();
            List<string> propsEliminarPorSubTipo = new List<string>();

            if (string.IsNullOrEmpty(pGrafoDbpedia))
            {
                pGrafoDbpedia = pGrafo;
            }

            if (!string.IsNullOrEmpty(pExtra))
            {
                Dictionary<string, string> listaExtras = UtilCadenas.ObtenerPropiedadesDeTexto(pExtra);

                if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.AgruparTipo.ToString()))
                {
                    propAgrupacion = listaExtras[ConfiguracionGrafoFichaRec.AgruparTipo.ToString()].Split('|')[0];
                    tiposAgrupar.AddRange(listaExtras[ConfiguracionGrafoFichaRec.AgruparTipo.ToString()].Split('|')[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    numAgrupar = int.Parse(listaExtras[ConfiguracionGrafoFichaRec.AgruparTipo.ToString()].Split('|')[2]);
                }

                if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.AgruparSubTipo.ToString()))
                {
                    propAgrupacionSubTipo = listaExtras[ConfiguracionGrafoFichaRec.AgruparSubTipo.ToString()].Split('|')[0];
                    propSubAgrupacion = listaExtras[ConfiguracionGrafoFichaRec.AgruparSubTipo.ToString()].Split('|')[1];
                    propSubAgrupacionBusqueda = listaExtras[ConfiguracionGrafoFichaRec.AgruparSubTipo.ToString()].Split('|')[2];
                    subTiposAgrupar.AddRange(listaExtras[ConfiguracionGrafoFichaRec.AgruparSubTipo.ToString()].Split('|')[3].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                    numSubAgrupar = int.Parse(listaExtras[ConfiguracionGrafoFichaRec.AgruparSubTipo.ToString()].Split('|')[4]);
                }

                if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.TiposReciprocos.ToString()))
                {
                    tiposReciprocos.AddRange(listaExtras[ConfiguracionGrafoFichaRec.TiposReciprocos.ToString()].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.NodosMaxRecurso.ToString()))
                {
                    numNodosMaxSuj = int.Parse(listaExtras[ConfiguracionGrafoFichaRec.NodosMaxRecurso.ToString()]);
                }

                if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.SuprimirPropExceptoTipoPrinc.ToString()))
                {
                    foreach (string props in listaExtras[ConfiguracionGrafoFichaRec.SuprimirPropExceptoTipoPrinc.ToString()].Split('|')[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        propsEliminarPorTipo.Add(props);
                    }

                    foreach (string tipo in listaExtras[ConfiguracionGrafoFichaRec.SuprimirPropExceptoTipoPrinc.ToString()].Split('|')[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        tiposPropsEliminarPorTipo.Add(tipo);
                    }
                }

                if (listaExtras.ContainsKey(ConfiguracionGrafoFichaRec.SuprimirPropExceptoSubTipoPrinc.ToString()))
                {
                    foreach (string props in listaExtras[ConfiguracionGrafoFichaRec.SuprimirPropExceptoSubTipoPrinc.ToString()].Split('|')[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        propsEliminarPorSubTipo.Add(props);
                    }

                    foreach (string tipo in listaExtras[ConfiguracionGrafoFichaRec.SuprimirPropExceptoSubTipoPrinc.ToString()].Split('|')[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        tiposPropsEliminarPorSubTipo.Add(tipo);
                    }
                }
            }

            Dictionary<string, KeyValuePair<Dictionary<string, string>, object[]>> nodos = new Dictionary<string, KeyValuePair<Dictionary<string, string>, object[]>>();
            string nodoDocActualID = $"http://gnoss/{pDocumentoID.ToString().ToUpper()}";
            nodos.Add(nodoDocActualID, new KeyValuePair<Dictionary<string, string>, object[]>(new Dictionary<string, string>(), new object[] { pTipoRecurso, 0, new List<string>(), "", 0, null, null, "" }));

            #region Cargo información recurso actual

            //Necesitamos los tipos del recurso actual:

            string queryInfRec = $"{NamespacesVirtuosoLectura} SELECT ?s ?type ?subtype from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{?s rdf:type ?type OPTIONAL {{?s <http://gnoss/type> ?subtype}} FILTER(?s = <{nodoDocActualID}>) }}";
            LeerDeVirtuoso(queryInfRec, "grafo", facetadoDS, pGrafo);

            foreach (DataRow fila in facetadoDS.Tables[0].Rows)
            {
                nodos[nodoDocActualID].Value[0] = (string)fila[1];//Type

                if (!fila.IsNull(2) && !fila[2].GetType().Equals(typeof(DBNull)) && !string.IsNullOrEmpty((string)fila[2]) && !((List<string>)nodos[nodoDocActualID].Value[2]).Contains((string)fila[2]))
                {
                    //Agrega un subTipo del elemento:
                    ((List<string>)nodos[nodoDocActualID].Value[2]).Add((string)fila[2]);
                }
            }

            facetadoDS.Dispose();

            #endregion

            #region Propiedades descartar por tipo y subTipo principal

            List<string> propsDescartarPorTipoSubTipoPrinc = new List<string>();

            if (!tiposPropsEliminarPorTipo.Contains(nodos[nodoDocActualID].Value[0]))
            {
                if (tiposPropsEliminarPorSubTipo.Count > 0)
                {
                    bool tieneSuTipo = ((List<string>)nodos[nodoDocActualID].Value[2]).Any(item => tiposPropsEliminarPorSubTipo.Contains(item));

                    if (!tieneSuTipo)
                    {
                        propsDescartarPorTipoSubTipoPrinc.AddRange(propsEliminarPorSubTipo);
                    }
                }
                else
                {
                    propsDescartarPorTipoSubTipoPrinc.AddRange(propsEliminarPorTipo);
                }
            }

            #endregion

            facetadoDS = null;
            FacetadoDS facetadoDS_SPO = new FacetadoDS();
            FacetadoDS facetadoDS_TYPE = new FacetadoDS();
            FacetadoDS facetadoDS_TYPE_NAME = new FacetadoDS();
            FacetadoDS facetadoDS_NAME = new FacetadoDS();
            bool grafoDbpediaIndependiente = !string.IsNullOrEmpty(pGrafoDbpedia) && !pGrafo.Equals(pGrafoDbpedia);
            string from = $" FROM {ObtenerUrlGrafo(pGrafo).ToLower()} ";
            string parteComun = " { ?o <http://gnoss/hasprivacidadCom> 'publico'. }  UNION { ?o <http://gnoss/hasprivacidadCom> 'publicoreg'. }  ";
            string parteComunGrafoDbpedia = " { ?o <http://gnoss/hasprivacidadCom> 'publico'. }  UNION { ?o <http://gnoss/hasprivacidadCom> 'publicoreg'. }  MINUS {?o <http://dbpedia.org/ontology/excluded> 'true'.} ";

            if (grafoDbpediaIndependiente)
            {
                from += $" FROM {ObtenerUrlGrafo(pGrafoDbpedia).ToLower()} ";
                parteComunGrafoDbpedia += "?o rdf:type \"dbpedia\". ";
            }

            string countRelated = "";
            string optionalRelatedTo = "";
            string orderByRelatedTo = "";
            if (pPropEnlace.Contains("relatedTo"))
            {
                countRelated = " count(?related) as ?contador";
                optionalRelatedTo = " optional {?o <" + pPropEnlace + "> ?related.} ";
                orderByRelatedTo = " order by desc(?contador)";
            }

            string selectConsultaRecipSPO = $"{NamespacesVirtuosoLectura} SELECT distinct ?s ?p ?o {countRelated} {from} WHERE {{{{ ?o ?p ?s. {parteComunGrafoDbpedia} {optionalRelatedTo} ?o <{pPropEnlace}> ?s. ";
            string selectConsultaRecipTipo = $"{NamespacesVirtuosoLectura} SELECT distinct ?o ?type ?subtype {from} WHERE {{{{ ?o rdf:type ?type. OPTIONAL {{?o <http://gnoss/type> ?subtype}} {parteComunGrafoDbpedia} ?o <{pPropEnlace}> ?s. ";
            string selectConsultaRecipTipoNombre = $"{NamespacesVirtuosoLectura} SELECT distinct ?o ?nombresubtype lang(?nombresubtype) as ?idiSubTipe {from} WHERE {{{{ ?o rdf:type ?type. OPTIONAL {{?o <http://gnoss/type> ?subtype}}{parteComunGrafoDbpedia} ?o <{pPropEnlace}> ?s. ";
            string selectConsultaRecipNombre = $"{NamespacesVirtuosoLectura} SELECT distinct ?o ?titulo lang(?titulo) as ?idiTitulo ?nombre lang(?nombre) as ?idiNombre {countRelated} {from} WHERE {{{{ ?o <http://xmlns.com/foaf/0.1/firstName> ?nombre. OPTIONAL{{?o <http://dbpedia.org/ontology/title> ?titulo}}{parteComunGrafoDbpedia} {optionalRelatedTo} ?o <{pPropEnlace}> ?s.";

            string selectConsultaSPO = $"SELECT distinct ?s ?p ?o from {ObtenerUrlGrafo(pGrafoDbpedia).ToLower()} WHERE {{{{ ?s <{pPropEnlace}> ?o. ?s ?p ?o. {parteComunGrafoDbpedia}";
            string selectConsultaTipo = $"SELECT distinct ?o ?type ?subtype from {ObtenerUrlGrafo(pGrafoDbpedia).ToLower()} WHERE {{{{ ?s <{pPropEnlace}> ?o. ?o rdf:type ?type. OPTIONAL {{?o <http://gnoss/type> ?subtype}}{parteComunGrafoDbpedia}";
            string selectConsultaTipoNombre = $"{NamespacesVirtuosoLectura} SELECT distinct ?o ?nombresubtype lang(?nombresubtype) as ?idiSubTipe from {ObtenerUrlGrafo(pGrafoDbpedia).ToLower()} WHERE {{{{?s <{pPropEnlace}> ?o. ?o rdf:type ?type. OPTIONAL {{?o <http://gnoss/type> ?subtype}}" + parteComunGrafoDbpedia;
            string selectConsultaNombre = $"SELECT distinct ?o ?titulo lang(?titulo) as ?idiTitulo ?nombre lang(?nombre) as ?idiNombre from {ObtenerUrlGrafo(pGrafoDbpedia).ToLower()} WHERE {{{{ ?s <{pPropEnlace}> ?o. ?o <http://xmlns.com/foaf/0.1/firstName> ?nombre. OPTIONAL{{?o <http://dbpedia.org/ontology/title> ?titulo}}{parteComunGrafoDbpedia}";

            string selectConsultaSPOComunidad = "";
            string selectConsultaTipoComunidad = "";
            string selectConsultaNombreComunidad = "";
            if (grafoDbpediaIndependiente)
            {
                selectConsultaSPOComunidad = $"SELECT distinct ?s ?p ?o from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{{{ ?s <{pPropEnlace}> ?o. ?s ?p ?o. {parteComun}";
                selectConsultaTipoComunidad = $"SELECT distinct ?o ?type ?subtype from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{{{ ?s <{pPropEnlace}> ?o. ?o rdf:type ?type. {parteComun}";
                selectConsultaNombreComunidad = $"SELECT distinct ?o ?titulo lang(?titulo) as ?idiTitulo ?nombre lang(?nombre) as ?idiNombre from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{{{ ?s <{pPropEnlace}> ?o. ?o <http://xmlns.com/foaf/0.1/firstName> ?nombre. {parteComun}";
            }

            StringBuilder whereSiguienteConsulta = new StringBuilder();
            StringBuilder whereSiguienteConsultaRecip = new StringBuilder();
            if (tiposReciprocos.Count == 0 || !tiposReciprocos.Contains(pTipoRecurso))
            {
                whereSiguienteConsulta.Append($"FILTER(?s = <{nodoDocActualID}>) }}}}");
            }
            else
            {
                whereSiguienteConsultaRecip.Append($"FILTER(?s = <{nodoDocActualID}>) }}}}");
            }

            if (!string.IsNullOrEmpty(propSubAgrupacion))
            {
                selectConsultaTipoNombre += "OPTIONAL {?o <" + propSubAgrupacion + "> ?nombresubtype} ";
                selectConsultaRecipTipoNombre += "OPTIONAL {?o <" + propSubAgrupacion + "> ?nombresubtype} ";
            }

            int nivel = 1;

            List<string> nodosUsados = new List<string>();
            nodosUsados.Add(nodoDocActualID);

            int numNodosIncluAgrup = 1;
            Dictionary<string, List<string>> nodosYSusPadres = new Dictionary<string, List<string>>();
            nodosYSusPadres.Add(nodoDocActualID, new List<string>());

            while (numNodosIncluAgrup < pNodosLimiteNivel)
            {
                string querySPO = null;
                string queryNombre = null;
                string queryTipo = null;
                string queryTipoNombre = null;

                if (whereSiguienteConsulta.Length > 0)
                {
                    querySPO = selectConsultaSPO + whereSiguienteConsulta;
                    queryNombre = selectConsultaNombre + whereSiguienteConsulta;
                    queryTipo = selectConsultaTipo + whereSiguienteConsulta;
                    queryTipoNombre = selectConsultaTipoNombre + whereSiguienteConsulta;

                    if (grafoDbpediaIndependiente)
                    {
                        querySPO = $"{NamespacesVirtuosoLectura} SELECT distinct ?s ?p ?o WHERE {{{{{querySPO}}} UNION {{{selectConsultaSPOComunidad}{whereSiguienteConsulta}}}}}";
                        queryNombre = $"{NamespacesVirtuosoLectura} SELECT distinct ?o ?titulo lang(?titulo) as ?idiTitulo ?nombre lang(?nombre) as ?idiNombre WHERE {{{{{queryNombre}}} UNION {{{selectConsultaNombreComunidad}{whereSiguienteConsulta}}}}}";
                        queryTipo = $"{NamespacesVirtuosoLectura} SELECT distinct ?o ?type ?subtype WHERE {{{{{queryTipo}}} UNION {{{selectConsultaTipoComunidad}{whereSiguienteConsulta}}}}}";
                    }
                    else
                    {
                        querySPO = $"{NamespacesVirtuosoLectura} {querySPO}";
                        queryNombre = $"{NamespacesVirtuosoLectura} {queryNombre}";
                        queryTipo = $"{NamespacesVirtuosoLectura} {queryTipo}";
                    }

                    LeerDeVirtuoso(querySPO, "grafo", facetadoDS_SPO, pGrafo);
                    LeerDeVirtuoso(queryNombre, "grafo", facetadoDS_NAME, pGrafo);
                    LeerDeVirtuoso(queryTipo, "grafo", facetadoDS_TYPE, pGrafo);
                    LeerDeVirtuoso(queryTipoNombre, "grafo", facetadoDS_TYPE_NAME, pGrafo);
                }

                if (whereSiguienteConsultaRecip.Length > 0)
                {
                    FacetadoDS facetadoAuxDS_SPO = new FacetadoDS();
                    querySPO = selectConsultaRecipSPO + whereSiguienteConsultaRecip + orderByRelatedTo;
                    queryNombre = selectConsultaRecipNombre + whereSiguienteConsultaRecip + orderByRelatedTo;

                    LeerDeVirtuoso(querySPO, "grafo", facetadoAuxDS_SPO, pGrafo);
                    facetadoDS_SPO.Merge(facetadoAuxDS_SPO);
                    facetadoAuxDS_SPO.Dispose();

                    FacetadoDS facetadoAuxDS_NAME = new FacetadoDS();
                    LeerDeVirtuoso(queryNombre, "grafo", facetadoAuxDS_NAME, pGrafo);
                    facetadoDS_NAME.Merge(facetadoAuxDS_NAME);
                    facetadoAuxDS_NAME.Dispose();

                    FacetadoDS facetadoAuxDS_TYPE = new FacetadoDS();
                    queryTipo = selectConsultaRecipTipo + whereSiguienteConsultaRecip;
                    LeerDeVirtuoso(queryTipo, "grafo", facetadoAuxDS_TYPE, pGrafo);
                    facetadoDS_TYPE.Merge(facetadoAuxDS_TYPE);
                    facetadoAuxDS_TYPE.Dispose();

                    FacetadoDS facetadoAuxDS_TYPE_NAME = new FacetadoDS();
                    queryTipoNombre = selectConsultaRecipTipoNombre + whereSiguienteConsultaRecip;
                    LeerDeVirtuoso(queryTipoNombre, "grafo", facetadoAuxDS_TYPE_NAME, pGrafo);
                    facetadoDS_TYPE_NAME.Merge(facetadoAuxDS_TYPE_NAME);
                    facetadoAuxDS_TYPE_NAME.Dispose();
                }

                if (facetadoDS_SPO.Tables[0].Rows.Count > 0)
                {
                    whereSiguienteConsulta = new StringBuilder("FILTER(?s in (");
                    whereSiguienteConsultaRecip = new StringBuilder("FILTER(?s in (");
                    List<string> nodosRelAhora = new List<string>();
                    List<string> nodosSujetosAhora = new List<string>();
                    Dictionary<string, List<string>> sujNodosAgrupacion = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> sujNodosSubTiposAgrupacion = new Dictionary<string, List<string>>();
                    List<string> nodosHijosConPropDescartada = new List<string>();

                    foreach (DataRow fila in facetadoDS_SPO.Tables[0].Rows)
                    {
                        string nodoPadreID = (string)fila[0];
                        string propiedad = (string)fila[1];
                        string nodoID = (string)fila[2];

                        DataRow[] filasType = facetadoDS_TYPE.Tables[0].Select("o = '" + nodoID + "'");
                        DataRow[] filasTypeNombre = facetadoDS_TYPE_NAME.Tables[0].Select("o = '" + nodoID + "'");
                        DataRow[] filasNombre = facetadoDS_NAME.Tables[0].Select("o = '" + nodoID + "'");

                        string tipo = "";
                        HashSet<string> lista_subtipo = new HashSet<string>();
                        foreach (DataRow filaTipo in filasType)
                        {
                            tipo = (string)filaTipo[1];
                            if (!Convert.IsDBNull(filaTipo[2]))
                            {
                                lista_subtipo.Add(filaTipo[2].ToString());
                            }
                        }

                        Dictionary<string, string> dic_titulo = new Dictionary<string, string>();
                        foreach (DataRow filaNombre in filasNombre)
                        {
                            if (!Convert.IsDBNull(filaNombre[1]))
                            {
                                string idioma = filaNombre[2].ToString();
                                string nombre = filaNombre[1].ToString();
                                if (!dic_titulo.ContainsKey(idioma))
                                {
                                    dic_titulo.Add(idioma, nombre);
                                }

                            }
                            else if (!Convert.IsDBNull(filaNombre[3]))
                            {
                                string idioma = filaNombre[4].ToString();
                                string nombre = filaNombre[3].ToString();
                                dic_titulo.Add(idioma, nombre);
                            }
                        }
                        string titulo = "";
                        if (dic_titulo.ContainsKey(pIdioma))
                        {
                            titulo = dic_titulo[pIdioma];
                        }
                        else if (SECUNDARIO_ESPANNOL.Contains(pIdioma.ToLower()) && dic_titulo.ContainsKey("es"))
                        {
                            titulo = dic_titulo["es"];
                        }
                        else if (dic_titulo.ContainsKey("en"))
                        {
                            titulo = dic_titulo["en"];
                        }
                        else if (dic_titulo.ContainsKey(""))
                        {
                            titulo = dic_titulo[""];
                        }
                        else if (dic_titulo.Count > 0)
                        {
                            titulo = dic_titulo.Values.ToList()[0];
                        }

                        Dictionary<string, string> dic_nombreSubTipo = new Dictionary<string, string>();
                        foreach (DataRow filaTypeNombre in filasTypeNombre)
                        {
                            string idioma = "";
                            string nombresubtipo = "";
                            if (!Convert.IsDBNull(filaTypeNombre[1]))
                            {
                                nombresubtipo = (string)filaTypeNombre[1];
                            }
                            if (!Convert.IsDBNull(filaTypeNombre[2]))
                            {
                                idioma = (string)filaTypeNombre[2];
                            }
                            if (!(dic_nombreSubTipo.ContainsKey(idioma) && dic_nombreSubTipo.ContainsValue(nombresubtipo)))
                            {
                                dic_nombreSubTipo.Add(idioma, nombresubtipo);
                            }

                        }

                        if (!nodosSujetosAhora.Contains(nodoPadreID))
                        {
                            nodosSujetosAhora.Add(nodoPadreID);
                        }

                        if (!nodos.ContainsKey(nodoID))
                        {
                            numNodosIncluAgrup++;
                            nodos.Add(nodoID, new KeyValuePair<Dictionary<string, string>, object[]>(new Dictionary<string, string>(), new object[8]));
                            nodos[nodoID].Value[0] = tipo;//Type
                            nodos[nodoID].Value[1] = nivel;
                            nodos[nodoID].Value[2] = new List<string>();
                            nodos[nodoID].Value[3] = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(titulo);//Titulo

                            nodosRelAhora.Add(nodoID);

                            if (!nodosYSusPadres.ContainsKey(nodoID))
                            {
                                nodosYSusPadres.Add(nodoID, new List<string>());
                            }
                        }

                        if (lista_subtipo.Count > 0)
                        {
                            //Agrega un subTipo del elemento:
                            ((List<string>)nodos[nodoID].Value[2]).AddRange(lista_subtipo);
                        }

                        if (nodoID == nodoDocActualID)
                        {
                            nodos[nodoID].Value[0] = tipo;//Type
                            nodos[nodoID].Value[3] = UtilCadenas.ConvertirPrimeraLetraPalabraAMayusculasExceptoArticulos(titulo);//Titulo
                        }

                        if (!nodos[nodoPadreID].Key.ContainsKey(nodoID) && (!nodos[nodoID].Key.ContainsKey(nodoPadreID)))
                        {
                            nodos[nodoPadreID].Key.Add(nodoID, propiedad);//Propiedad
                            nodosYSusPadres[nodoID].Add(nodoPadreID);
                        }
                        else if (propiedad != pPropEnlace && nodos[nodoPadreID].Key.ContainsKey(nodoID))
                        {
                            nodos[nodoPadreID].Key[nodoID] = propiedad;//Propiedad
                        }

                        if (propsDescartarPorTipoSubTipoPrinc.Contains(propiedad) && !nodosHijosConPropDescartada.Contains(nodoID))
                        {
                            nodosHijosConPropDescartada.Add(nodoID);
                        }

                        if (tiposAgrupar.Contains(tipo))
                        {
                            string claveAgru = nodoPadreID + "_clav_" + tiposAgrupar[0];

                            if (!sujNodosAgrupacion.ContainsKey(claveAgru))
                            {
                                sujNodosAgrupacion.Add(claveAgru, new List<string>());
                            }

                            if (!sujNodosAgrupacion[claveAgru].Contains(nodoID))
                            {
                                sujNodosAgrupacion[claveAgru].Add(nodoID);
                            }
                        }

                        if (subTiposAgrupar.Contains(tipo) && dic_nombreSubTipo.ContainsKey(pIdioma))
                        {
                            string claveAgru = nodoPadreID + "_clav_" + dic_nombreSubTipo[pIdioma];

                            if (!sujNodosSubTiposAgrupacion.ContainsKey(claveAgru))
                            {
                                sujNodosSubTiposAgrupacion.Add(claveAgru, new List<string>());
                            }

                            if (!sujNodosSubTiposAgrupacion[claveAgru].Contains(nodoID))
                            {
                                sujNodosSubTiposAgrupacion[claveAgru].Add(nodoID);
                            }
                        }
                    }

                    foreach (string clave in sujNodosAgrupacion.Keys)
                    {
                        if (sujNodosAgrupacion[clave].Count >= numAgrupar)
                        {
                            string nodoPadreID = clave.Substring(0, clave.IndexOf("_clav_"));
                            string nuevoNodoRel = sujNodosAgrupacion[clave][0] + "_bis_" + nodoPadreID;
                            nodos.Add(nuevoNodoRel, new KeyValuePair<Dictionary<string, string>, object[]>(new Dictionary<string, string>(), new object[] { tiposAgrupar[0], nivel, new List<string>(), "", null, null, null, "" }));
                            nodos[nodoPadreID].Key.Add(nuevoNodoRel, pPropEnlace);
                            nodos[nuevoNodoRel].Value[4] = sujNodosAgrupacion[clave].Count;
                            nodos[nuevoNodoRel].Value[5] = nodoPadreID;//padre de la agrupación
                            numNodosIncluAgrup++;

                            foreach (string nodoID in sujNodosAgrupacion[clave])
                            {
                                nodos[nodoPadreID].Key.Remove(nodoID);
                                nodosYSusPadres[nodoID].Remove(nodoPadreID);

                                if (nodosRelAhora.Contains(nodoID) && nodosYSusPadres[nodoID].Count == 0)
                                {
                                    if (nodos.ContainsKey(nodoID))
                                    {
                                        numNodosIncluAgrup--;
                                        nodos.Remove(nodoID);
                                    }

                                    nodosRelAhora.Remove(nodoID);
                                }
                            }
                        }
                    }

                    foreach (string clave in sujNodosSubTiposAgrupacion.Keys)
                    {
                        if (sujNodosSubTiposAgrupacion[clave].Count >= numSubAgrupar)
                        {
                            string nodoPadreID = clave.Substring(0, clave.IndexOf("_clav_"));
                            string nombreSubTipo = clave.Substring(clave.IndexOf("_clav_") + 6);
                            string nuevoNodoRel = sujNodosSubTiposAgrupacion[clave][0] + "_bis_" + nodoPadreID;
                            nodos.Add(nuevoNodoRel, new KeyValuePair<Dictionary<string, string>, object[]>(new Dictionary<string, string>(), new object[] { subTiposAgrupar[0], nivel, new List<string>(), "", null, null, null, "" }));
                            nodos[nodoPadreID].Key.Add(nuevoNodoRel, pPropEnlace);
                            nodos[nuevoNodoRel].Value[4] = sujNodosSubTiposAgrupacion[clave].Count;
                            nodos[nuevoNodoRel].Value[5] = nodoPadreID;//padre de la agrupación
                            nodos[nuevoNodoRel].Value[6] = nombreSubTipo;

                            StringBuilder listaGuidsRecursos = new StringBuilder();
                            foreach (string subNodo in sujNodosSubTiposAgrupacion[clave])
                            {
                                listaGuidsRecursos.Append($"{subNodo},");
                            }
                            UtilCadenas.EliminarUltimosCaracteresStringBuilder(listaGuidsRecursos, ',');
                            nodos[nuevoNodoRel].Value[7] = listaGuidsRecursos;
                            numNodosIncluAgrup++;

                            foreach (string nodoID in sujNodosSubTiposAgrupacion[clave])
                            {
                                if (nodosYSusPadres[nodoID].Count == 1) // (int)nodos[nodoID].Value.GetValue(1) != 1)
                                {
                                    nodos[nodoPadreID].Key.Remove(nodoID);
                                    nodosYSusPadres[nodoID].Remove(nodoPadreID);

                                    if (nodosRelAhora.Contains(nodoID) && nodosYSusPadres[nodoID].Count == 0)
                                    {
                                        if (nodos.ContainsKey(nodoID))
                                        {
                                            numNodosIncluAgrup--;
                                            nodos.Remove(nodoID);
                                        }

                                        nodosRelAhora.Remove(nodoID);
                                    }
                                }
                            }
                        }
                    }

                    if (numNodosMaxSuj.HasValue)
                    {
                        foreach (string nodoID in nodosSujetosAhora)
                        {

                            if (nodos[nodoID].Key.Count > numNodosMaxSuj.Value)
                            {
                                List<string> nodosRel = new List<string>(nodos[nodoID].Key.Keys);
                                int numNodoQuitar = (nodos[nodoID].Key.Count - numNodosMaxSuj.Value);

                                for (int i = 0; i < numNodoQuitar; i++)
                                {

                                    if (nodosYSusPadres.ContainsKey(nodosRel[i]) && nodosRel[i] != nodoDocActualID)
                                    {
                                        nodos[nodoID].Key.Remove(nodosRel[i]);

                                        nodosYSusPadres[nodosRel[i]].Remove(nodoID);

                                        if (nodosYSusPadres[nodosRel[i]].Count == 0)//Se queda solo
                                        {
                                            if (nodos.ContainsKey(nodosRel[i]))
                                            {
                                                numNodosIncluAgrup--;
                                                nodos.Remove(nodosRel[i]);
                                            }

                                            nodosRelAhora.Remove(nodosRel[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Quito los nodos descartados enlazados a través de props no enlazables:
                    foreach (string nodoID in nodosHijosConPropDescartada)
                    {
                        List<string> padres = new List<string>(nodosYSusPadres[nodoID]);

                        foreach (string padre in padres)
                        {
                            if (propsDescartarPorTipoSubTipoPrinc.Contains(nodos[padre].Key[nodoID]) && (padre != nodoDocActualID || nodosRelAhora.Count > 1))//Solo hay que elimina la relación si se hace por una prop descartada y el padre no es el sujeto principal o si lo es no se queda sin hijos:
                            {
                                nodos[padre].Key.Remove(nodoID);
                                nodosYSusPadres[nodoID].Remove(padre);

                                if (nodosYSusPadres[nodoID].Count == 0)//Se queda solo
                                {
                                    if (nodos.ContainsKey(nodoID))
                                    {
                                        numNodosIncluAgrup--;
                                        nodos.Remove(nodoID);
                                    }
                                    nodosRelAhora.Remove(nodoID);
                                }
                            }
                        }
                    }

                    if (nodosRelAhora.Count > 0)
                    {
                        bool hayMas = false;
                        bool hayMasRecip = false;
                        foreach (string nodoID in nodosRelAhora)
                        {
                            if (!nodosUsados.Contains(nodoID))
                            {
                                nodosUsados.Add(nodoID);

                                if (tiposReciprocos.Contains((string)nodos[nodoID].Value[0]))
                                {
                                    hayMasRecip = true;
                                    whereSiguienteConsultaRecip.Append($"<{nodoID}>,");
                                }
                                else
                                {
                                    hayMas = true;
                                    whereSiguienteConsulta.Append($"<{nodoID}>,");
                                }
                            }
                        }

                        if (!hayMas && !hayMasRecip)
                        {
                            break;
                        }

                        if (hayMas)
                        {
                            whereSiguienteConsulta.Remove(whereSiguienteConsulta.Length - 1, 1);
                            whereSiguienteConsulta.Append("))}}");
                        }
                        else
                        {
                            whereSiguienteConsulta.Clear();
                        }

                        if (hayMasRecip)
                        {
                            whereSiguienteConsultaRecip.Remove(whereSiguienteConsultaRecip.Length - 1, 1);
                            whereSiguienteConsultaRecip.Append("))}}");
                        }
                        else
                        {
                            whereSiguienteConsultaRecip.Clear();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }

                facetadoDS_SPO.Tables[0].Clear();
                facetadoDS_TYPE.Tables[0].Clear();
                facetadoDS_TYPE_NAME.Tables[0].Clear();
                facetadoDS_NAME.Tables[0].Clear();
                nivel++;
            }

            string datos = "";

            if (nodos.Count > 0)
            {
                bool hayRelaciones = false;

                StringBuilder datosNodos = new StringBuilder("{\"nodes\": {");
                StringBuilder datosConex = new StringBuilder("},\"edges\": {");

                foreach (string nodoID in nodos.Keys)
                {
                    StringBuilder subTipos = new StringBuilder(",subTipos:\"");

                    foreach (string subTipo in (List<string>)nodos[nodoID].Value[2])
                    {
                        subTipos.Append($"{subTipo},");
                    }

                    subTipos.Append("\"");

                    if (nodos[nodoID].Value[4] != null && (int)nodos[nodoID].Value[4] > 1)
                    {
                        subTipos.Append($",numElemGrupo:{(int)nodos[nodoID].Value[4]},sujRelGrupo:\"{nodos[nodoID].Value[5]}\",propRelGrupo:\"");

                        if (nodos[nodoID].Value[6] == null)
                        {
                            subTipos.Append($"{propAgrupacion}\"");
                        }
                        else
                        {
                            subTipos.Append($"{propAgrupacionSubTipo}\",nombreSubTipo:\"{nodos[nodoID].Value[6]}\",proNombreSubTipo:\"{propSubAgrupacionBusqueda}\"");
                        }

                        subTipos.Append($",GuidsMostrarPop:\"{nodos[nodoID].Value[7]}\"");
                    }

                    string valorComillas;

                    valorComillas = ((string)nodos[nodoID].Value[3]).Replace("\"", "\\\"");


                    datosNodos.Append($"\"{nodoID}\":{{tipo:\"{(string)nodos[nodoID].Value[0]}\",nivel:\"{(int)nodos[nodoID].Value[1]}\",titulo:\"{valorComillas}\"{subTipos}}},");

                    if (nodos[nodoID].Key.Count > 0)
                    {
                        string trozoDatos = $"\"{nodoID}\":{{";
                        StringBuilder trozoRelDatos = new StringBuilder();


                        foreach (string nodoConexID in nodos[nodoID].Key.Keys)
                        {
                            trozoRelDatos.Append($"\"{nodoConexID}\":{{propConexion:\"{nodos[nodoID].Key[nodoConexID]}\"}},");
                        }

                        if (trozoRelDatos.Length > 0)
                        {
                            hayRelaciones = true;
                            trozoRelDatos.Remove(trozoRelDatos.Length - 1, 1);
                            trozoRelDatos.Append("},");
                            datosConex.Append(trozoDatos + trozoRelDatos);
                        }
                    }
                }

                if (!hayRelaciones)
                {
                    return "";
                }

                datosNodos.Remove(datosNodos.Length - 1, 1);
                datosConex.Remove(datosConex.Length - 1, 1);
                datosConex.Append("}}");
                datos += datosNodos.ToString() + datosConex.ToString();
            }
            if (facetadoDS != null)
            {
                facetadoDS.Dispose();
            }
            return datos;
        }

        /// <summary>
        /// Busca los recursos a mostrar en el idioma del navegación del usuario
        /// </summary>
        /// <param name="pFacetado"> Tabla con los recursos del grafo</param>
        /// <param name="pFila">Fila a analizar</param>
        /// <param name="pIdioma">Idioma de navegación</param>
        /// <returns>La fila con la información del recurso</returns>
        private DataRow BuscarIdiomaNavegacion(DataTable pFacetado, DataRow pFila, string pIdioma)
        {
            DataRow fila = null;
            // Si el idioma de la definicion y el del titulo no son iguales al del usuario
            if (!(pIdioma.Equals(pFila[7].ToString()) && pIdioma.Equals(pFila[8].ToString())))
            {
                DataRow[] filasNombreTipeIdiomaUsuario = pFacetado.Select("p = '" + pFila[1] + "' AND o = '" + pFila[2] + "' AND idiSubTipe = '" + pIdioma.ToLower() + "' AND idiNombre = '" + pIdioma.ToLower() + "'");

                DataRow[] filasTituloTipeIdiomaUsuario = pFacetado.Select("p = '" + pFila[1] + "' AND o = '" + pFila[2] + "' AND idiSubTipe = '" + pIdioma.ToLower() + "' AND idiNombre = '" + pIdioma.ToLower() + "'");

                if (filasNombreTipeIdiomaUsuario.Length > 0)
                {
                    // Encontrada fila con el idioma del subtipo y el nombre igual que el idioma del usuario
                    fila = filasNombreTipeIdiomaUsuario[0];
                }
                else if (filasTituloTipeIdiomaUsuario.Length > 0)
                {
                    // Encontrada fila con el idioma del subtipo y el titulo igual que el idioma del usuario
                    fila = filasTituloTipeIdiomaUsuario[0];
                }
                else
                {
                    DataRow[] filasNombreIdiomaUsuario = pFacetado.Select("p = '" + pFila[1] + "' AND o = '" + pFila[2] + "' AND idiNombre = '" + pIdioma.ToLower() + "'");
                    DataRow[] filasTituloIdiomaUsuario = pFacetado.Select("p = '" + pFila[1] + "' AND o = '" + pFila[2] + "' AND idiTitulo = '" + pIdioma.ToLower() + "'");
                    if (filasNombreIdiomaUsuario.Length > 0)
                    {
                        fila = ComprobarSubType(filasNombreIdiomaUsuario, pIdioma);

                    }
                    else if (filasTituloIdiomaUsuario.Length > 0)
                    {
                        fila = ComprobarSubType(filasTituloIdiomaUsuario, pIdioma);
                    }
                    else
                    {
                        DataRow[] filasSubTipeIdiomaUsuario = pFacetado.Select("p = '" + pFila[1] + "' AND o = '" + pFila[2] + "' AND idiSubTipe = '" + pIdioma.ToLower() + "'");
                        // Encontrada fila con el idioma del subtipo igual que el idioma del usuario
                        if (filasSubTipeIdiomaUsuario.Length > 0)
                        {
                            fila = filasSubTipeIdiomaUsuario[0];
                        }
                        else
                        {
                            // Buscamos en otros idiomas
                            if (pIdioma.ToLower().Equals("en"))
                            {
                                fila = pFila;
                            }
                            else if (SECUNDARIO_ESPANNOL.Contains(pIdioma.ToLower()))
                            {
                                fila = BuscarIdiomaNavegacion(pFacetado, pFila, "es");
                            }
                            else
                            {
                                fila = BuscarIdiomaNavegacion(pFacetado, pFila, "en");
                            }
                        }
                    }
                }
            }
            else
            {
                fila = pFila;
            }
            return fila;
        }

        private static DataRow ComprobarSubType(DataRow[] filas, string pIdioma)
        {
            DataRow fila = null;
            DataRow[] filasSubTipe = filas.Where(x => x[7].ToString().Equals("en")).ToArray();
            if (filasSubTipe.Length > 0)
            {
                fila = filasSubTipe[0];
            }

            if (SECUNDARIO_ESPANNOL.Contains(pIdioma.ToLower()))
            {
                filasSubTipe = filas.Where(x => x[7].ToString().Equals("es")).ToArray();
                if (filasSubTipe.Length > 0)
                {
                    fila = filasSubTipe[0];
                }
            }

            if (fila == null)
            {
                fila = filas[0];
            }
            return fila;
        }

        private bool EsEntidadPrincipal(string pGrafo)
        {
            if (pGrafo.IndexOf(".Owl") == -1)
            {
                pGrafo += ".owl";
            }

            return mEntityContext.Documento.Any(item => item.Enlace.Equals(pGrafo) && item.Tipo == 7 && !item.Eliminado);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="pUsuarioID">Grafo de la consulta</param>
        /// <param name="pListaComentariosID">Listado de comentarios</param>
        /// <returns></returns>
        public Dictionary<Guid, bool> ObtenerLeidoListaComentarios(Guid pUsuarioID, List<Guid> pListaComentariosID)
        {
            Dictionary<Guid, bool> listaComentarios = new Dictionary<Guid, bool>();

            FacetadoDS facetadoDS = new FacetadoDS();

            string select = " SELECT DISTINCT ?s  ?leido ";
            string from = " FROM " + ObtenerUrlGrafo(pUsuarioID.ToString()).ToLower() + " ";
            StringBuilder where = new StringBuilder(" WHERE { ?s ?p ?o. ?s gnoss:Leido ?leido. FILTER(?s in (");

            foreach (Guid comentarioID in pListaComentariosID)
            {
                where.Append($"gnoss:{comentarioID.ToString().ToUpper()},");
            }

            where.Remove(where.Length - 1, 1);
            where.Append(" )) }");

            string query = NamespacesVirtuosoLectura + " " + select + from + where;
            LeerDeVirtuoso(query, "Comentario", facetadoDS, pUsuarioID.ToString());

            foreach (DataRow fila in facetadoDS.Tables["Comentario"].Rows)
            {
                Guid comentarioid = new Guid(fila["s"].ToString().Replace("http://gnoss/", ""));
                if (!listaComentarios.ContainsKey(comentarioid))
                {
                    listaComentarios.Add(comentarioid, fila["leido"].ToString() == "Leidos");
                }
            }

            return listaComentarios;
        }

        #endregion

        #region Métodos de consultas generales
        /// <summary>
        /// Devuelve si un texto tiene o no acentos
        /// </summary>
        public static bool TieneAcentos(String texto)
        {
            bool b = false;

            for (int i = 0; i < texto.Length; i++)
            {
                if (consignos.Contains(texto.Substring(i, 1)))
                { b = true; }
            }


            return b;
        }

        /// <summary>
        /// Devuelve el texto sin acentos
        /// </summary>
        public static string removerSignosAcentos(string texto)
        {
            StringBuilder textoSinAcentos = new StringBuilder(texto.Length);
            int indexConAcento;
            foreach (char caracter in texto)
            {
                indexConAcento = consignos.IndexOf(caracter);
                if (indexConAcento > -1)
                    textoSinAcentos.Append(sinsignos.Substring(indexConAcento, 1));
                else
                    textoSinAcentos.Append(caracter);
            }
            return textoSinAcentos.ToString();
        }

        /// <summary>
        /// Quita el prefijo a la faceta
        /// </summary>
        /// <param name="pNombreFaceta">Nombre completo de la faceta</param>
        /// <returns></returns>
        public static string QuitaPrefijo(string pNombreFaceta)
        {
            if (!string.IsNullOrEmpty(pNombreFaceta))
            {
                pNombreFaceta = pNombreFaceta.Replace(":", "");
                pNombreFaceta = pNombreFaceta.Replace("-", "");
            }
            return pNombreFaceta;
        }

        private static string ObtenerFiltroRecursosExcluidos(List<Guid> pListaExcluidos)
        {
            string filtroExcluidos = $"FILTER(?s NOT IN (";
            string listaExcluidos = string.Empty;

            foreach (Guid id in pListaExcluidos)
            {
                listaExcluidos = $"{listaExcluidos}gnoss:{id.ToString().ToUpper()},";
            }

            filtroExcluidos = $"{filtroExcluidos}{listaExcluidos.Substring(0, listaExcluidos.Length - 1)}))";

            return filtroExcluidos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pListaTipos"></param>
        /// <returns></returns>
        public Dictionary<string, int> ObtenerNumeroRecursosPorListaTipos(Guid pProyectoID, List<string> pListaTipos)
        {
            StringBuilder filtrosRDF = new StringBuilder();
            StringBuilder filtrosHASTIPODOC = new StringBuilder();

            foreach (string tipo in pListaTipos)
            {
                if (tipo.Contains("rdf:type="))
                {
                    filtrosRDF.Append($"'{tipo.Replace("rdf:type=", "").Replace("|", "','")}',");
                }
                else if (tipo.Contains("gnoss:hastipodoc="))
                {
                    filtrosHASTIPODOC.Append($"'{tipo.Replace("gnoss:hastipodoc=", "").Replace("|", "','")}',");
                }
            }

            string query = NamespacesVirtuosoLectura;
            query += " select ?o count(?s) ";
            query += " from " + ObtenerUrlGrafo(pProyectoID.ToString()).ToLower() + " ";

            FacetadoDS facetadoDSRDF = new FacetadoDS();
            FacetadoDS facetadoDSHASTIPODOC = new FacetadoDS();
            if (filtrosRDF.Length > 0)
            {
                filtrosRDF.Remove(filtrosRDF.Length - 1, 1);
                string RDF = " where {?s rdf:type ?o. FILTER(?o in(" + filtrosRDF + "))} ";
                LeerDeVirtuoso(query + RDF, "NumeroRecursos", facetadoDSRDF, pProyectoID.ToString());
            }
            if (filtrosHASTIPODOC.Length > 0)
            {
                filtrosHASTIPODOC.Remove(filtrosHASTIPODOC.Length - 1, 1);
                string TIPODOC = " where {?s gnoss:hastipodoc ?o. FILTER(?o in(" + filtrosHASTIPODOC + "))} ";
                LeerDeVirtuoso(query + TIPODOC, "NumeroRecursos", facetadoDSHASTIPODOC, pProyectoID.ToString());
            }

            Dictionary<string, int> resultado = new Dictionary<string, int>();
            foreach (string tipo in pListaTipos)
            {
                resultado.Add(tipo, 0);
            }

            if (facetadoDSRDF.Tables.Contains("NumeroRecursos"))
            {
                foreach (DataRow fila in facetadoDSRDF.Tables["NumeroRecursos"].Rows)
                {
                    string dato = fila[0].ToString();
                    int numero = int.Parse(fila[1].ToString());

                    foreach (string tipo in pListaTipos.Where(item => item.Contains($"rdf:type={dato}")))
                    {
                        resultado[tipo] += numero;
                    }
                }
            }

            if (facetadoDSHASTIPODOC.Tables.Contains("NumeroRecursos"))
            {
                foreach (DataRow fila in facetadoDSHASTIPODOC.Tables["NumeroRecursos"].Rows)
                {
                    string dato = fila[0].ToString();
                    int numero = int.Parse(fila[1].ToString());

                    foreach (string tipo in pListaTipos.Where(item => item.Contains($"gnoss:hastipodoc={dato}")))
                    {
                        resultado[tipo] += numero;
                    }
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene el bloque de privacidad para una búsqueda
        /// </summary>
        /// <param name="pEstaEnMyGnoss">Verdad si el usuario la búsqueda se está realizando en myGnoss</param>
        /// <param name="pEsInvitado">Verdad si el usuario no está registrado</param>
        /// <param name="pEsMiembroComunidad">Verdad si el usuario no es miembro de la comunidad</param>
        /// <param name="pIdentidadID">Identificador de la identidad de la persona</param>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pProyectoID">ID del proyecto de la consulta</param>
        /// <param name="pObtenerRecursosPrivados">TRUE: si debe de obtener tambien los recursos privados en funcion de la identidad. FALSE: si solo debe obtener los públicos</param>
        /// <returns></returns>
        private string ObtenerBloqueHasPrivacidad(bool pEstaEnMyGnoss, bool pEsMiembroComunidad, string pIdentidadID, string pProyectoID, bool pObtenerRecursosPrivados)
        {
            StringBuilder query = new StringBuilder();
            if (UsarPrivacidadEnFacetasYResultados)
            {
                Guid identidadID = new Guid(pIdentidadID);

                if (pEstaEnMyGnoss)
                {
                    query.Append("{?s gnoss:hasprivacidadMyGnoss 'publico'. } ");

                    if (pObtenerRecursosPrivados)
                    {
                        if (ObtenerPrivados)
                        {
                            query.Append("UNION {?s gnoss:hasprivacidadMyGnoss 'privado'.} ");
                            query.Append("UNION {?s gnoss:hasprivacidadMyGnoss 'publicoreg'.} ");
                        }
                        else if (pEsMiembroComunidad)
                        {
                            query.Append(" UNION { ?s gnoss:hasprivacidadMyGnoss 'publicoreg'. } ");

                            if (UsuarioTieneRecursosPrivados)
                            {
                                IdentidadAD identidadAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadAD>(), mloggerFactory);
                                Guid perfilID = identidadAD.ObtenerIdentidadPorID(new Guid(pIdentidadID), true).ListaIdentidad.Find(x => x.IdentidadID.Equals(new Guid(pIdentidadID))).PerfilID;
                                DataWrapperIdentidad dwIdentidad = identidadAD.ObtenerIdentidadesDePerfil(perfilID);
                                string identidades = string.Empty;
                                foreach (EntityModel.Models.IdentidadDS.Identidad identidad in dwIdentidad.ListaIdentidad)
                                {
                                    identidades = $"{identidades}gnoss:{identidad.IdentidadID.ToString().ToUpper()},";
                                }
                                if (identidades.EndsWith(','))
                                {
                                    identidades = identidades.Substring(0, identidades.Length - 1);
                                }

                                query.Append(" UNION {?s gnoss:hasparticipanteIdentidadID ?identidad . FILTER (?identidad  in (" + identidades + ")) . }");
                                query.Append(" UNION {?s  gnoss:hasparticipanteIdentidadID ?grupo. ?grupo gnoss:hasparticipanteID ?identidadParticipaGrupo . FILTER (?identidadParticipaGrupo in (" + identidades + ")) . }");
                            }
                        }
                    }
                }
                else
                {
                    //esta en la comunidad
                    string privacidad = "{ ?s gnoss:hasprivacidadCom 'publico'. } ";

                    if (pObtenerRecursosPrivados)
                    {
                        if (ObtenerPrivados)
                        {
                            privacidad += " UNION { ?s gnoss:hasprivacidadCom 'privado'.} ";
                            privacidad += " UNION { ?s gnoss:hasprivacidadCom 'publicoreg'. } ";
                        }
                        else if (pEsMiembroComunidad)
                        {
                            privacidad += " UNION { ?s gnoss:hasprivacidadCom 'publicoreg'. } ";

                            if (UsuarioTieneRecursosPrivados)
                            {
                                privacidad += " UNION {?s gnoss:hasparticipanteIdentidadID gnoss:" + pIdentidadID.ToUpper() + ". }";

                                if (mIdentidadTieneGruposComunidad == null)
                                {
                                    mIdentidadTieneGruposComunidad = new ConcurrentDictionary<Guid, bool>();
                                }

                                if (!mIdentidadTieneGruposComunidad.ContainsKey(identidadID))
                                {
                                    Guid proyID;
                                    Guid.TryParse(pProyectoID, out proyID);
                                    bool tieneGruposProy = true;
                                    if (!proyID.Equals(Guid.Empty))
                                    {
                                        IdentidadAD idenAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadAD>(), mloggerFactory);
                                        tieneGruposProy = idenAD.TieneIdentidadGruposDeProyeto(proyID, identidadID);
                                        idenAD.Dispose();
                                    }

                                    mIdentidadTieneGruposComunidad.TryAdd(identidadID, tieneGruposProy);
                                }

                                if (mIdentidadTieneGruposComunidad[identidadID])
                                {
                                    privacidad += " UNION {?s  gnoss:hasparticipanteIdentidadID ?grupo.?grupo gnoss:hasparticipanteID gnoss:" + pIdentidadID.ToUpper() + "}";
                                }
                            }
                        }
                    }
                    query.Append(privacidad);
                }

                if (pObtenerRecursosPrivados)
                {
                    if (!mListaGruposPorIdentidad.ContainsKey(identidadID))
                    {
                        if (identidadID.Equals(UsuarioAD.Invitado))
                        {
                            mListaGruposPorIdentidad.TryAdd(identidadID, new List<Guid>());
                        }
                        else
                        {
                            IdentidadAD idenAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadAD>(), mloggerFactory);
                            mListaGruposPorIdentidad.TryAdd(identidadID, idenAD.ObtenerGruposDeOrganizacionDeIdentidad(identidadID));
                            idenAD.Dispose();
                        }
                    }

                    List<Guid> listaGrupos = mListaGruposPorIdentidad[identidadID];
                    if (listaGrupos.Count > 0)
                    {
                        query.Append(" UNION { ?s  gnoss:hasparticipanteIdentidadID ?grupo. FILTER(?grupo in (");

                        foreach (Guid grupo in listaGrupos)
                        {
                            query.Append($"gnoss:{grupo.ToString().ToUpper()},");
                        }
                        query = query.Remove(query.Length - 1, 1);

                        query.Append(" ))} ");
                    }
                }
            }
            return query.ToString();
        }

        /// <summary>
        /// Elimina los filtros que no se deben tener en cuenta en la query
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pClave">Clave del filtro que se busca</param>
        private static void EliminarFiltrosInnecesarios(Dictionary<string, List<string>> pListaFiltros, string pClave)
        {
            while (pListaFiltros[pClave].Contains("Meta"))
            {
                pListaFiltros[pClave].Remove("Meta");
            }
            while (pListaFiltros[pClave].Contains("MyGnoss"))
            {
                pListaFiltros[pClave].Remove("MyGnoss");
            }
            while (pListaFiltros[pClave].Contains("Particular"))
            {
                pListaFiltros[pClave].Remove("Particular");
            }
            while (pListaFiltros[pClave].Contains("PersonaInicial"))
            {
                pListaFiltros[pClave].Remove("PersonaInicial");
            }
        }

        /// <summary>
        /// Obtiene el from para un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerUrlGrafo(string pProyectoID)
        {
            string proyectoID = pProyectoID;
            //Si se trata de perfil personal
            if (proyectoID.Contains("perfil/"))
            {
                proyectoID = proyectoID.Replace("perfil/", "");
            }

            //Si se trata de contribuciones
            if (proyectoID.Contains("contribuciones/"))
            {
                proyectoID = proyectoID.Replace("contribuciones/", "");

            }

            //Contactos
            if (proyectoID.Contains("contacto"))
            {
                proyectoID = "contactos";
            }
            else if (proyectoID.Equals("geonames"))
            {
                proyectoID = "http://gnoss.com/geonames";
            }

            if (!proyectoID.StartsWith("http://") && !pProyectoID.StartsWith("https://"))
            {
                return $" <{mUrlIntranet}{proyectoID}> ";
            }
            else
            {
                return $" <{proyectoID}> ";
            }
        }

        /// <summary>
        /// Obtiene el from para un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public string ObtenerFrom(string pProyectoID)
        {
            return $" from{ObtenerUrlGrafo(pProyectoID)}";
        }

        /// <summary>
        /// Obtiene el from graph para un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        private string ObtenerFromGraph(string pProyectoID)
        {
            return $" FROM{ObtenerUrlGrafo(pProyectoID)}";
        }

        /// <summary>
        /// Devuelve una lista con los identificadores de los recursos del proyecto que coincidan con los filtros introducidos
        /// </summary>
        /// <param name="pListaFiltros">filtros que deben cumplir los recursos. Es un diccionario donde la clave es el tipo de filtro (por ejemplo: rdf:type, schema:name, etc.) y el valor es una lista con los valores a verificar</param>
        /// <param name="pProyectoId">identificador del proyecto seleccionado</param>
        /// <param name="pTipoProyecto">tipo del proyecto seleccionado</param>
        /// <returns>Lista de los identificadores de los recursos del proyecto que cumplen el filtro</returns>
        public List<string> ObtenerIdsRecursos(Dictionary<string, List<string>> pListaFiltros, string pProyectoId, TipoProyecto pTipoProyecto, bool pConsultaFiltrosBusqueda = false)
        {
            int limiteRecursos = 10000;
            int iteraciones = 0;
            FacetadoDS facetadoDS = new FacetadoDS();

            do
            {
                FacetadoDS facetadoDSAux = new FacetadoDS();

                string query = $"select distinct ?s {ObtenerFrom(pProyectoId)} WHERE";
                query += $"{{{ObtenerParteFiltros("", pListaFiltros, new List<string>(), true, pProyectoId, null, pTipoProyecto, false, false, TiposAlgoritmoTransformacion.Ninguno, new Dictionary<string, Tuple<string, string, string, bool>>(), false, pConsultaFiltrosBusqueda: pConsultaFiltrosBusqueda)}}} order by ?s ";
                query = $"{NamespacesVirtuosoLectura} select distinct ?s {ObtenerFrom(pProyectoId)} WHERE {{ {query} }} LIMIT {limiteRecursos} OFFSET {limiteRecursos * iteraciones}";

                LeerDeVirtuoso(query, "Recursos", facetadoDSAux, pProyectoId);
                facetadoDS.Merge(facetadoDSAux);
                iteraciones++;
            }
            while (facetadoDS.Tables["Recursos"].Rows.Count % limiteRecursos == 0);
            List<string> listaIDsRecursos = new List<string>();
            foreach(string fila in facetadoDS.Tables["Recursos"].Rows.OfType<DataRow>().Select((row) => row[0].ToString()).ToList())
            {
                string id = fila;
                if (id.Contains("/"))
                {
                    id = id.Substring(id.LastIndexOf('/') + 1);
                }
                listaIDsRecursos.Add(id);
            }
            return listaIDsRecursos;
        }

        /// <summary>
        /// Devuelve el número de recursos del proyecto que coinciden con los filtros introducidos como parámetro. 
        /// </summary>
        /// <param name="pListaFiltros">filtros que deben cumplir los recursos. Es un diccionario donde la clave es el tipo de filtro (por ejemplo: rdf:type, schema:name, etc.) y el valor es una lista con los valores a verificar</param>
        /// <param name="pProyectoId">identificador del proyecto seleccionado</param>
        /// <param name="pTipoProyecto">tipo del proyecto seleccionado</param>
        /// <returns></returns>
        public int ObtenerNumRecursos(Dictionary<string, List<string>> pListaFiltros, string pProyectoId, TipoProyecto pTipoProyecto, bool pConsultaFiltrosBusqueda = false)
        {
            string query = $"{NamespacesVirtuosoLectura} select count(distinct ?s) {ObtenerFrom(pProyectoId)} WHERE";
            query += $"{{{ObtenerParteFiltros("", pListaFiltros, new List<string>(), true, pProyectoId, null, pTipoProyecto, false, false, TiposAlgoritmoTransformacion.Ninguno, new Dictionary<string, Tuple<string, string, string, bool>>(), false, pConsultaFiltrosBusqueda: pConsultaFiltrosBusqueda)}}}";

            FacetadoDS facetadoDS = new FacetadoDS();

            LeerDeVirtuoso(query, "Recursos", facetadoDS, pProyectoId);

            return Convert.ToInt32(facetadoDS.Tables["Recursos"].Rows[0][0]);
        }


        /// <summary>
        /// Dado un recurso en un proyecto devuelve el contenido de su triple search
        /// </summary>
        /// <param name="pProyectoID">identificador del proyecto seleccionado.</param>
        /// <param name="pGuidRecurso">identificador del recurso a buscar.</param>
        /// <returns>Contenido del triple search del recurso.</returns>
        public string ObtenerSearchDeRecurso(string pProyectoID, string pGuidRecurso)
        {
            string query = $"select * {ObtenerFrom(pProyectoID)} WHERE{{ <http://gnoss/{pGuidRecurso}> <http://gnoss/search> ?o.}}";
            FacetadoDS facetadoDS = new FacetadoDS();

            LeerDeVirtuoso(query, "search", facetadoDS, pProyectoID);

            if (facetadoDS.Tables["search"].Rows.Count == 0)
            {
                return "";
            }
            return facetadoDS.Tables["search"].Rows[0][0].ToString();
        }

        /// <summary>
        /// Devuelve los IDs de los recursos del proyecto dado que contienen pTermino en su triple search 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pTermino"></param>
        /// <returns></returns>
        public List<string> ObtenerIdRecursosConBusquedaPorTextoLibre(string pProyectoID, string pTermino)
        {
            List<string> idRecursos = new List<string>();
            string parametroBifContains = ConstruirParametroBifContains(pTermino);

            string query = $"select ?s {ObtenerFrom(pProyectoID)} WHERE {{ ?s <http://gnoss/search> ?o. ?o bif:contains\"{parametroBifContains}\"}}";

            FacetadoDS facetadoDS = new FacetadoDS();
            LeerDeVirtuoso(query, "RecursosBusqueda", facetadoDS, pProyectoID);

            for (int i = 0; i < facetadoDS.Tables["RecursosBusqueda"].Rows.Count; i++)
            {
                idRecursos.Add(facetadoDS.Tables["RecursosBusqueda"].Rows[i][0].ToString());
            }
            return idRecursos;
        }

        /// <summary>
        /// Comprueba si la búsqueda mediante la instrucción bif:contains por un término encuentra un recurso concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto seleccionado</param>
        /// <param name="pGuidRecurso">Identificador del recurso a buscar</param>
        /// <param name="pTermino">término de búsqueda</param>
        /// <returns>Cierto si el recurso es indexable por el término y falso en caso contrario.</returns>
        public bool RecursoBuscablePorTermino(string pProyectoID, string pGuidRecurso, string pTermino)
        {
            string parametroBifContains = ConstruirParametroBifContains(pTermino);

            string query = $"ASK {ObtenerFrom(pProyectoID)} WHERE {{ <http://gnoss/{pGuidRecurso}> <http://gnoss/search> ?o. ?o bif:contains\"{parametroBifContains}\"}}";

            FacetadoDS facetadoDS = new FacetadoDS();
            LeerDeVirtuoso(query, "Resultado", facetadoDS, pProyectoID);

            return facetadoDS.Tables["Resultado"].Rows[0][0].ToString() == "1";
        }

        /// <summary>
        /// Comprueba si un proyecto contiene un recurso concreto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto seleccionado</param>
        /// <param name="pRecurso">Identificador del recurso</param>
        /// <returns>Cierto si el recurso pertenece al proyecto y falso en caso contrario</returns>
        public bool RecursoEstaEnProyecto(string pProyectoID, string pRecurso)
        {
            string query = $"ASK {ObtenerFrom(pProyectoID)} WHERE{{ <http://gnoss/{pRecurso}> ?p ?o. }}";

            FacetadoDS facetadoDS = new FacetadoDS();
            LeerDeVirtuoso(query, "Resultado", facetadoDS, pProyectoID);

            return (facetadoDS.Tables["Resultado"].Rows[0][0].ToString() == "1");
        }

        /// <summary>
        /// Procesa el término de búsqueda para que sea empleado por una instrucción bif:contains
        /// </summary>
        /// <param name="pTermino"></param>
        /// <returns></returns>
        private static string ConstruirParametroBifContains(string pTermino)
        {
            string[] terminosBifContains = pTermino.Split(" ");
            StringBuilder parametroBifContains = new StringBuilder();
            parametroBifContains.Append($"'{terminosBifContains[0]}'");
            for (int i = 1; i < terminosBifContains.Count(); i++)
            {
                parametroBifContains.Append($"AND'{terminosBifContains[i]}'");
            }
            return parametroBifContains.ToString();
        }

        /// <summary>
        /// Verdad si la búsqueda puede contener personas y organizaciones
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <returns></returns>
        private static bool EstaBuscandoPersonaOOrganizacion(Dictionary<string, List<string>> pListaFiltros)
        {
            if ((pListaFiltros.ContainsKey("rdf:type"))) //&& pListaFiltros["rdf:type"].Count > 0
            {
                List<string> listaTipo = pListaFiltros["rdf:type"];
                if ((listaTipo.Contains("ContactoPer")) || (listaTipo.Contains("ContactoOrg")) || (listaTipo.Contains("Persona")) || (listaTipo.Contains("Organizacion")) || (listaTipo.Contains("Meta")) || pListaFiltros["rdf:type"].Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obtiene el valor concreto para un filtro
        /// </summary>
        /// <param name="pClave">Clave del filtro</param>
        /// <param name="pValor">Valor del filtro</param>
        /// <returns></returns>
        public static string ObtenerValorParaFiltro(string pClave, string pValor, short pTipoPropiedadFaceta)
        {
            if (pClave.Contains(";"))
            {
                pClave = pClave.Substring(pClave.IndexOf(";") + 1);
            }

            List<string> facetasNoConvertir = new List<string>();
            facetasNoConvertir.Add("rdf:type");
            facetasNoConvertir.Add("gnoss:hasEstado");
            facetasNoConvertir.Add("sioc:has_space");
            facetasNoConvertir.Add("skos:ConceptID");
            facetasNoConvertir.Add("gnoss:PerfilID");
            facetasNoConvertir.Add("gnoss:IdentidadID");
            facetasNoConvertir.Add("gnoss:hasComunidadOrigen");

            //Mensajes, comentarios e invitaciones
            facetasNoConvertir.Add("gnoss:Leido");
            facetasNoConvertir.Add("nmo:isRead");
            facetasNoConvertir.Add("dce:type");

            //Inevery, personas y organizaciones
            facetasNoConvertir.Add("cv:OrganizationNameCentroEstudios");
            facetasNoConvertir.Add("cv:OrganizationUnitNameTitulaciones");
            facetasNoConvertir.Add("cv:OrganizationNameDisciplina");

            //Mi espacio personal
            facetasNoConvertir.Add("gnoss:hasOrigen");
            facetasNoConvertir.Add("gnoss:hasEstadoPP");

            //Contribuciones organización
            facetasNoConvertir.Add("gnoss:haspublicadorIdentidadID");

            //Contribuciones organización
            facetasNoConvertir.Add("gnoss:haspublicadorIdentidadID");

            //Sub tipo
            facetasNoConvertir.Add("gnoss:type");

            // Pagina CMS Padre
            facetasNoConvertir.Add("gnoss:hasNombreCortoJerarquia");

            //prod7711
            if (!facetasNoConvertir.Contains(pClave) && !EsIDGnoss(pValor) && !UtilCadenas.EsMultiIdioma(pValor) && !pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.TextoInvariable) && !pClave.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
            {
                //bug7596
                pValor = pValor.ToLowerSearchGraph();
            }

            bool esValorNumerico = pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.Numero) || pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.Calendario) || pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.CalendarioConRangos) || pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.Fecha) || pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.Siglo);
            long n;
            if (!pValor.Contains("gnoss:") && (pClave.Equals("sioc_t:Tag") || pClave.Equals("inv:Delivery_number") || pClave.Equals("inv:Order_number") || !long.TryParse(pValor, out n) || !esValorNumerico))
            {
                string idioma = null;

                if (UtilCadenas.EsMultiIdioma(pValor))
                {
                    idioma = pValor.Substring(pValor.LastIndexOf("@"));
                    pValor = pValor.Substring(0, pValor.LastIndexOf("@"));
                }

                if ((pValor.StartsWith("http://") || pValor.StartsWith("https://")) && !pClave.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
                {
                    return $"<{pValor}>";
                }
                string valor = UtilCadenas.ToSparql(pValor);
                long vaorEnteroLargo = 0;

                //No hay que transformar los enteros a números cuando el filtro es un tag.
                if (esValorNumerico && !pClave.Equals("sioc_t:Tag"))
                {
                    if (long.TryParse(valor, out vaorEnteroLargo))
                    {
                        valor = $"'{valor}'^^xsd:integer";
                    }
                    else
                    {
                        valor = $"'{valor}'{idioma}";
                    }
                }
                else
                {
                    valor = $"'{valor}'{idioma}";
                }

                return valor;
            }
            else if (pValor.Contains("gnoss:"))
            {
                string[] partes = pValor.Split(':');
                string identificador = partes[1];
                Guid id;
                if (Guid.TryParse(identificador, out id))
                {
                    pValor = $"{partes[0]}:{identificador}";
                }
                else
                {
                    throw new ExcepcionWeb($"parametro no valido -- clave del parametro:{pClave} || valor del parametro: {pValor}");
                }
            }
            return pValor;
        }

        /// <summary>
        /// Obtiene la parte de los filtros para una query
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <returns></returns>
        private string ObtenerParteFiltros(Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, bool pInmutable = false, bool pEsMovil = false)
        {
            return ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, "", TipoProyecto.Catalogo, pInmutable, pEsMovil);
        }

        /// <summary>
        /// Obtiene la parte de los filtros para una query
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <returns></returns>
        private string ObtenerParteFiltros(Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, string pFiltrosContextoWhere, bool pEsMovil = false)
        {
            return ObtenerParteFiltros("", pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltrosContextoWhere, TipoProyecto.Catalogo, pEsMovil);
        }

        /// <summary>
        /// Obtiene la parte de los filtros para una query
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pNombreFaceta">Nombre de la faceta a cargar</param>
        /// <returns></returns>
        private string ObtenerParteFiltros(string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsMovil = false)
        {
            return ObtenerParteFiltros(pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, false, pEsMovil);
        }

        /// <summary>
        /// Obtiene la parte de los filtros para una query
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pNombreFaceta">Nombre de la faceta a cargar</param>
        /// <returns></returns>
        private string ObtenerParteFiltros(string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluirPersonas, bool pInmutable, bool pEsMovil = false)
        {
            return ObtenerParteFiltros(pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, pExcluirPersonas, true, TiposAlgoritmoTransformacion.Ninguno, null, pInmutable, pEsMovil);
        }

        private string ObtenerParteFiltros(string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluirPersonas, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil = false, bool pEsPeticionNumResultados = false, bool pConsultaFiltrosBusqueda = false)
        {
            string ultimaFacetaAux = "";
            return ObtenerParteFiltros(pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEsMiembroComunidad, pProyectoID, pFiltroContextoWhere, pTipoProyecto, pExcluirPersonas, pOmitirPalabrasNoRelevantesSearch, pTipoAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pInmutable, out ultimaFacetaAux, pEsMovil, pEsPeticionNumResultados: pEsPeticionNumResultados, pConsultaFiltrosBusqueda: pConsultaFiltrosBusqueda);
        }

        private static string ProcesarParametroSeparadorUltimoDiferente(string pTexto, string pSeparadorInterno, string pValor)
        {
            string separadorValores = pTexto.Split(new string[] { pSeparadorInterno }, StringSplitOptions.RemoveEmptyEntries)[1];
            string[] filtroSeparador = pValor.Split(new string[] { separadorValores }, StringSplitOptions.RemoveEmptyEntries);
            string[] querysAuxArray = pTexto.Split(new string[] { pSeparadorInterno }, StringSplitOptions.RemoveEmptyEntries);
            string queryAuxModificada = querysAuxArray[2];
            string queryAuxModificadaFin = querysAuxArray[3];
            StringBuilder queryAuxFin = new StringBuilder();
            if (pSeparadorInterno == "||")
            {
                int i = 0;
                foreach (string trozo in filtroSeparador)
                {
                    i++;
                    if (i == filtroSeparador.Length)
                    {
                        queryAuxFin.Append($" {queryAuxModificadaFin.Replace("[PARAMETROSEPARADORIN]", trozo)} ");
                    }
                    else
                    {
                        queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROSEPARADORIN]", trozo)} ");
                    }
                }
            }
            else
            {
                int i = 0;
                foreach (string trozo in filtroSeparador)
                {
                    i++;
                    string queryAux = "";
                    if (i == filtroSeparador.Length)
                    {
                        queryAux = queryAuxModificadaFin;
                    }
                    else
                    {
                        queryAux = queryAuxModificada;
                    }
                    while (queryAux.Contains("|||[PARAMETROSEPARADORULTIMODIFERENTE]||"))
                    {
                        int inicio = queryAux.IndexOf("|||[PARAMETROSEPARADORULTIMODIFERENTE]||");
                        string inv = new string(queryAux.Substring(0, inicio).Reverse().ToArray());
                        int numTuberia = 3;
                        while (inv[0] == '|')
                        {
                            inv = inv.Substring(1);
                            numTuberia++;
                        }
                        StringBuilder stringSeparadorExterno = new StringBuilder();
                        StringBuilder stringSeparadorInterno = new StringBuilder();
                        for (int j = 1; j <= numTuberia; j++)
                        {
                            stringSeparadorExterno.Append("|");
                        }
                        for (int j = 1; j <= numTuberia - 1; j++)
                        {
                            stringSeparadorInterno.Append("|");
                        }
                        inicio = queryAux.IndexOf(stringSeparadorExterno + "[PARAMETROSEPARADORULTIMODIFERENTE]||");
                        int fin = queryAux.IndexOf(stringSeparadorExterno.ToString(), inicio + 1) + 3;
                        string queryTransformada = ProcesarParametroSeparadorUltimoDiferente(queryAux.Substring(inicio, fin - inicio), stringSeparadorInterno.ToString(), trozo);
                        queryAux = queryAux.Substring(0, inicio) + queryTransformada + queryAux.Substring(fin);
                        queryAuxFin.Append(queryAux);
                    }
                }
            }
            return queryAuxFin.ToString();
        }
        /// <summary>
        /// Obtiene la parte de los filtros para una query
        /// </summary>
        /// <param name="pListaFiltros">Lista de filtros</param>
        /// <param name="pNombreFaceta">Nombre de la faceta a cargar</param>
        /// <param name="pFiltrosSearchPersonalizados">Diccionario con los filtros tipo 'search' personalizados la clave es el nombre del filtro y el valor es 'WhereSPARQL','OrderBySPARQL','WhereFacetasSPARQL','OmitirRdfType'</param>
        /// <returns></returns>
        private string ObtenerParteFiltros(string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMiembroComunidad, string pProyectoID, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluirPersonas, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, out string pUltimaFacetaAux, bool pEsMovil = false, Guid pPestanyaID = new Guid(), bool pEsPeticionNumResultados = false, bool pConsultaFiltrosBusqueda = false)
        {
            Dictionary<string, List<string>> listaFiltrosAuxOptional = new Dictionary<string, List<string>>(pListaFiltros);
            pUltimaFacetaAux = "";
            List<string> filtrosOpcionales = new List<string>();

            if (mListaItemsBusquedaExtra == null || mListaItemsBusquedaExtra.Count == 0)
            {
                mListaItemsBusquedaExtra = pListaFiltrosExtra;
            }

            foreach (string faceta in listaFiltrosAuxOptional.Keys.Where(item => item.ToLower().StartsWith("optional;")))
            {
                pListaFiltros.Remove(faceta);
                pListaFiltros.Add(faceta.Replace("optional;", ""), listaFiltrosAuxOptional[faceta]);
                filtrosOpcionales.Add(faceta.Replace("optional;", ""));
            }

            #region Filtros facetas por RDF:TYPE
            //Si hay facetas que contienen ; solo afectan al rdf:type que indiquen
            //En ese caso solo hay que mostrar los resultados que se correspondan con los rdf:types que vengan en el filtro y a cada uno le afectan sus facetas
            bool facetasPorTipo = false;
            Dictionary<string, List<string>> listaFiltrosAux = new Dictionary<string, List<string>>(pListaFiltros);
            Dictionary<string, Dictionary<string, List<string>>> listaFiltrosPorTipo = new Dictionary<string, Dictionary<string, List<string>>>();
            string separadorFacetaPorTipo = ";";
            foreach (string faceta in listaFiltrosAux.Keys.Where(item => item.Contains(separadorFacetaPorTipo)))
            {
                facetasPorTipo = true;

                string tipo = faceta.Substring(0, faceta.IndexOf(separadorFacetaPorTipo));
                string tipoFaceta = faceta.Substring(faceta.IndexOf(separadorFacetaPorTipo) + 1);

                if (tipo.StartsWith('(') && tipo.EndsWith(')'))
                {
                    tipo = tipo.Substring(1);
                    tipo = tipo.Substring(0, tipo.Length - 1);
                }

                if (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Contains(tipo))
                {
                    if (!listaFiltrosPorTipo.ContainsKey(tipo))
                    {
                        Dictionary<string, List<string>> filtros = new Dictionary<string, List<string>>();
                        listaFiltrosPorTipo.Add(tipo, filtros);
                    }

                    if (!listaFiltrosPorTipo[tipo].ContainsKey(tipoFaceta))
                    {
                        List<string> filtrosFaceta = new List<string>();
                        listaFiltrosPorTipo[tipo].Add(tipoFaceta, filtrosFaceta);
                    }

                    foreach (string filtroFaceta in listaFiltrosAux[faceta])
                    {
                        if (!listaFiltrosPorTipo[tipo][tipoFaceta].Contains(filtroFaceta))
                        {
                            listaFiltrosPorTipo[tipo][tipoFaceta].Add(filtroFaceta);
                        }
                    }
                }
                else
                {
                    pListaFiltros.Remove(faceta);
                }
            }
            #endregion

            StringBuilder query = new StringBuilder(pFiltroContextoWhere);

            if (pEsMovil)
            {
                query.Append(ObtenerExcepcionesMovil(pProyectoID));
            }

            bool omitirRdfType = false;
            Dictionary<string, string> listaFacetasPorTipo = new Dictionary<string, string>();
            if (pFiltrosSearchPersonalizados != null)
            {
                foreach (string keySearch in pFiltrosSearchPersonalizados.Keys.ToList())
                {
                    foreach (string keyFiltros in pListaFiltros.Keys.ToList())
                    {
                        if (keySearch == keyFiltros || keyFiltros.EndsWith(";" + keySearch))
                        {
                            string valorFiltro = pListaFiltros[keyFiltros][0];
                            string filtroWhere = pFiltrosSearchPersonalizados[keySearch].Item1;
                            if (((!string.IsNullOrEmpty(pNombreFaceta) || mEsPeticionFacetas) || pEsPeticionNumResultados) && !string.IsNullOrEmpty(pFiltrosSearchPersonalizados[keySearch].Item3))
                            {
                                //Si se trata de una faceta y tiene filtro para facetas cogemos el filtro de facetas
                                filtroWhere = pFiltrosSearchPersonalizados[keySearch].Item3;
                            }
                            omitirRdfType = pFiltrosSearchPersonalizados[keySearch].Item4;
                            if (!string.IsNullOrEmpty(filtroWhere))
                            {
                                valorFiltro = valorFiltro.Trim('"');
                                string[] valorFiltroSplit = valorFiltro.Split(new string[] { "$$$" }, StringSplitOptions.RemoveEmptyEntries);

                                string queryPersonalizada = filtroWhere;
                                for (int numFiltro = 0; numFiltro < valorFiltroSplit.Length; numFiltro++)
                                {
                                    string nombreParametro = "PARAMETRO";
                                    valorFiltro = valorFiltroSplit[numFiltro];
                                    if (numFiltro > 0)
                                    {
                                        nombreParametro = ("PARAMETRO" + (numFiltro + 1));
                                    }
                                    //Ejemplos:
                                    //1.-
                                    //|||[PARAMETROESPACIO]||FILTER (?searchTitle LIKE "*[PARAMETROESPACIOIN]*").||| 
                                    //Hace un split de valorFiltro y pone una aparicion de 'FILTER (?searchTitle LIKE "*[PARAMETROESPACIOIN]*").' por cada palabra sustituyendo [PARAMETROESPACIOIN] por la palabra
                                    //2.-
                                    //[PARAMETRO]
                                    //Sustituye [PARAMETRO] por valorFiltro
                                    //3.-
                                    //|||[PARAMETROESPACIOULTIMODIFERENTE]||FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN] *").||FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN]*").||| 
                                    //Hace un split de valorFiltro y pone una aparicion de 'FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN] *").' por cada palabra sustituyendo [PARAMETROESPACIOIN] por la palabra, excepto en la última palabra que pone 'FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN]*").' sustituyendo [PARAMETROESPACIOIN] por la palabra

                                    string[] filtroEspacios = valorFiltro.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                    while (queryPersonalizada.Contains($"|||[{nombreParametro}ESPACIOULTIMODIFERENTE]||"))
                                    {
                                        int inicio = queryPersonalizada.IndexOf($"|||[{nombreParametro}ESPACIOULTIMODIFERENTE]||");
                                        int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                        string[] querysAuxArray = queryPersonalizada.Substring(inicio + 3, fin - inicio - 6).Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                                        string queryAuxModificada = querysAuxArray[1];
                                        string queryAuxModificadaFin = querysAuxArray[2];

                                        StringBuilder queryAuxFin = new StringBuilder();
                                        int i = 0;
                                        foreach (string palabra in filtroEspacios)
                                        {
                                            i++;
                                            if (i == filtroEspacios.Length)
                                            {
                                                queryAuxFin.Append($" {queryAuxModificadaFin.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                            }
                                            else
                                            {
                                                queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                            }

                                        }
                                        queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                    }

                                    while (queryPersonalizada.Contains($"|||[{nombreParametro}ESPACIOULTIMODIFERENTELIMPIO]||"))
                                    {
                                        int numeroTagsDespreciadosTitulo = 0;
                                        //Recorro los tags individuales
                                        List<string> palabrasFiltro = AnalizadorSintactico.ObtenerTagsFrase(valorFiltro, out numeroTagsDespreciadosTitulo, " ", pOmitirPalabrasNoRelevantesSearch);

                                        if (palabrasFiltro.Count == 0)
                                        {
                                            //Si limpia todas, mejor que no limpie ninguna
                                            palabrasFiltro = AnalizadorSintactico.ObtenerTagsFrase(valorFiltro, out numeroTagsDespreciadosTitulo, " ", false);
                                        }

                                        int inicio = queryPersonalizada.IndexOf($"|||[{nombreParametro}ESPACIOULTIMODIFERENTELIMPIO]||");
                                        int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                        string[] querysAuxArray = queryPersonalizada.Substring(inicio + 3, fin - inicio - 6).Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                                        string queryAuxModificada = querysAuxArray[1];
                                        string queryAuxModificadaFin = querysAuxArray[2];

                                        StringBuilder queryAuxFin = new StringBuilder();
                                        int i = 0;
                                        foreach (string palabra in palabrasFiltro)
                                        {
                                            string palabraLimpia = RemoverSignosSearch(palabra).Replace("'", "\\'");

                                            i++;
                                            if (i == palabrasFiltro.Count)
                                            {
                                                queryAuxFin.Append($" {queryAuxModificadaFin.Replace("[PARAMETROESPACIOIN]", palabraLimpia)} ");
                                            }
                                            else
                                            {
                                                queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabraLimpia)} ");
                                            }

                                        }
                                        queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                    }

                                    while (queryPersonalizada.Contains($"|||[{nombreParametro}ESPACIO]||"))
                                    {
                                        int inicio = queryPersonalizada.IndexOf($"|||[{nombreParametro}ESPACIO]||");
                                        int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                        string queryAuxModificada = queryPersonalizada.Substring(inicio, fin - inicio).Replace($"|||[{nombreParametro}ESPACIO]||", "").Replace("|||", "");
                                        StringBuilder queryAuxFin = new StringBuilder();
                                        foreach (string palabra in filtroEspacios)
                                        {
                                            queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                        }
                                        queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                    }

                                    while (queryPersonalizada.Contains($"|||[{nombreParametro}ESPACIOLIMPIO]||"))
                                    {
                                        int numeroTagsDespreciadosTitulo = 0;
                                        //Recorro los tags individuales
                                        List<string> palabrasFiltro = AnalizadorSintactico.ObtenerTagsFrase(valorFiltro, out numeroTagsDespreciadosTitulo, " ", pOmitirPalabrasNoRelevantesSearch);

                                        if (palabrasFiltro.Count == 0)
                                        {
                                            //Si limpia todas, mejor que no limpie ninguna
                                            palabrasFiltro = AnalizadorSintactico.ObtenerTagsFrase(valorFiltro, out numeroTagsDespreciadosTitulo, " ", false);
                                        }

                                        int inicio = queryPersonalizada.IndexOf($"|||[{nombreParametro}ESPACIOLIMPIO]||");
                                        int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                        string queryAuxModificada = queryPersonalizada.Substring(inicio, fin - inicio).Replace($"|||[{nombreParametro}ESPACIOLIMPIO]||", "").Replace("|||", "");
                                        StringBuilder queryAuxFin = new StringBuilder();
                                        foreach (string palabra in palabrasFiltro)
                                        {
                                            queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                        }
                                        queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                    }

                                    while (queryPersonalizada.Contains("|||[PARAMETROSEPARADORULTIMODIFERENTE]||"))
                                    {
                                        int inicio = queryPersonalizada.IndexOf("|||[PARAMETROSEPARADORULTIMODIFERENTE]||");
                                        string inv = new string(queryPersonalizada.Substring(0, inicio).Reverse().ToArray());
                                        int numTuberia = 3;
                                        while (inv[0] == '|')
                                        {
                                            inv = inv.Substring(1);
                                            numTuberia++;
                                        }
                                        StringBuilder stringSeparadorExterno = new StringBuilder();
                                        StringBuilder stringSeparadorInterno = new StringBuilder();
                                        for (int j = 1; j <= numTuberia; j++)
                                        {
                                            stringSeparadorExterno.Append("|");
                                        }
                                        for (int j = 1; j <= numTuberia - 1; j++)
                                        {
                                            stringSeparadorInterno.Append("|");
                                        }
                                        inicio = queryPersonalizada.IndexOf(stringSeparadorExterno + "[PARAMETROSEPARADORULTIMODIFERENTE]||");
                                        int fin = queryPersonalizada.IndexOf(stringSeparadorExterno.ToString(), inicio + 1) + stringSeparadorExterno.Length;
                                        string queryTransformada = ProcesarParametroSeparadorUltimoDiferente(queryPersonalizada.Substring(inicio, fin - inicio), stringSeparadorInterno.ToString(), valorFiltro);
                                        queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryTransformada + queryPersonalizada.Substring(fin);
                                    }

                                    while (queryPersonalizada.Contains($"|||[{nombreParametro}ESPACIOULTIMODIFERENTELIKE]||"))
                                    {
                                        int inicio = queryPersonalizada.IndexOf($"|||[{nombreParametro}ESPACIOULTIMODIFERENTELIKE]||");
                                        int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                        string[] querysAuxArray = queryPersonalizada.Substring(inicio + 3, fin - inicio - 6).Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                                        string queryAuxModificada = querysAuxArray[1];
                                        string queryAuxModificadaFin = querysAuxArray[2];
                                        string valorFiltroAux = valorFiltro.Trim().Normalize(NormalizationForm.FormD);
                                        StringBuilder sb = new StringBuilder();
                                        foreach (char charin in valorFiltroAux)
                                        {
                                            if (char.IsLetterOrDigit(charin) || charin == ' ')
                                            {
                                                sb.Append(charin);
                                            }
                                        }
                                        valorFiltroAux = sb.ToString().ToLower();
                                        string[] filtroEspaciosAux = valorFiltroAux.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                        Dictionary<string, string> listaReemplazos = new Dictionary<string, string>();
                                        listaReemplazos["a"] = "aáàä";
                                        listaReemplazos["e"] = "eéèë";
                                        listaReemplazos["i"] = "iíìï";
                                        listaReemplazos["o"] = "oóòö";
                                        listaReemplazos["u"] = "uúùü";
                                        listaReemplazos["n"] = "nñ";
                                        listaReemplazos["c"] = "cç";
                                        StringBuilder queryAuxFin = new StringBuilder();
                                        int i = 0;
                                        foreach (string palabra in filtroEspaciosAux)
                                        {
                                            string palabraAux = palabra;
                                            foreach (string caracter in listaReemplazos.Keys)
                                            {
                                                palabraAux = palabraAux.Replace(caracter, $"[{listaReemplazos[caracter]}]");
                                            }
                                            i++;
                                            if (i == filtroEspaciosAux.Length)
                                            {
                                                queryAuxFin.Append($" {queryAuxModificadaFin.Replace("[PARAMETROESPACIOIN]", palabraAux)} ");
                                            }
                                            else
                                            {
                                                queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabraAux)} ");
                                            }
                                        }
                                        queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                    }

                                    queryPersonalizada = queryPersonalizada.Replace($"[{nombreParametro}]", valorFiltro);
                                }

                                if (!facetasPorTipo)
                                {
                                    query.Append($" {queryPersonalizada} ");
                                }
                                else
                                {
                                    listaFacetasPorTipo.Add(keyFiltros, queryPersonalizada);
                                }
                            }
                        }
                    }
                }

            }

            //Si es una faceta y hay que omitir el rdftype, lo omitimos
            if ((string.IsNullOrEmpty(pNombreFaceta) && !mEsPeticionFacetas) && omitirRdfType)
            {
                pListaFiltros.Remove("rdf:type");
            }

            string[] LNiveles = null;
            string[] stringSeparators = new string[] { "@@@" };

            //El filtro documentoid se ignora
            pListaFiltros.Remove("documentoid");

            List<string> listaFiltrosUsados = new List<string>();

            List<string> listaItemsAEliminarQuery = new List<string>();
            foreach (string newKey in pListaFiltros.Keys)
            {
                //Comprobar si la faceta es inmutable y el filtro con la clave de la faceta es la misma no hacer nada, no poner sentencia en el where
                //Comprobar si el filtro de la faceta coincide con la clave de la faceta para no hacer nada, si son distintas hacer.
                if (!newKey.Equals(pNombreFaceta) || !pInmutable)
                {
                    //Hacer lo que se viene haciendo.

                    if (string.Compare(newKey, "SPARQL", true) == 0)
                    {
                        query.Append(pListaFiltros[newKey].FirstOrDefault());
                        continue;
                    }
                    //Si el filtro no contiene 2 puntos ni empieza por http, ni es search ni search personalizado no se tiene en cuenta
                    if (!newKey.Contains(":") && !newKey.Contains(";") && !newKey.StartsWith("http") && newKey.ToLower() != "search" && pFiltrosSearchPersonalizados != null && !pFiltrosSearchPersonalizados.ContainsKey(newKey))
                    {
                        continue;
                    }

                    if (filtrosOpcionales.Contains(newKey))
                    {
                        query.Append(" OPTIONAL { ");
                    }

                    string consultaReciproca = string.Empty;
                    string valorFiltro = string.Empty;
                    ObtenerDatosFiltroReciproco(out consultaReciproca, newKey, out valorFiltro);
                    if (pFiltrosSearchPersonalizados == null || !pFiltrosSearchPersonalizados.ContainsKey(newKey.Substring(newKey.IndexOf(";") + 1)))
                    {

                        string Key = newKey;
                        string KeySinTipo = Key;
                        if (KeySinTipo.Contains(";"))
                        {
                            KeySinTipo = KeySinTipo.Substring(KeySinTipo.IndexOf(";") + 1);
                        }
                        EliminarFiltrosInnecesarios(pListaFiltros, Key);
                        Dictionary<string, List<string>> filtrosUsadosFacetaActual = new Dictionary<string, List<string>>();
                        if (pListaFiltros[Key] != null && pListaFiltros[Key].Count > 0)
                        {
                            int nivelReciproca = 0;
                            short tipoPropiedadFaceta = (short)TipoPropiedadFaceta.NULL;
                            if (mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo)))
                            {
                                FacetaObjetoConocimientoProyecto faceta = ObtenerFacetaClaveTipo(KeySinTipo, pListaFiltros);
                                nivelReciproca = int.Parse(faceta.Reciproca.ToString());
                                tipoPropiedadFaceta = faceta.TipoPropiedad.Value;
                            }

                            int numeroFiltros = 10;
                            string numeroFiltro = "";
                            string keySinPrefijo = QuitaPrefijo(Key);
                            if (!string.IsNullOrEmpty(valorFiltro))
                            {
                                keySinPrefijo = QuitaPrefijo(valorFiltro);
                                Key = valorFiltro;
                            }

                            if (Key.Contains("@@@"))
                            {
                                LNiveles = Key.Split(stringSeparators, StringSplitOptions.None);
                                Key = LNiveles[LNiveles.Length - 1];
                                keySinPrefijo = QuitaPrefijo(LNiveles[0]) + QuitaPrefijo(LNiveles[LNiveles.Length - 1]);
                            }

                            if (Key.Substring(0, 2).Equals("cv"))
                            {
                                query.Append("?s gnoss:hasCv ?CV.");
                            }
                            else if (!Key.Equals("sioc_t:Tag") && !Key.Equals("search") && !Key.Equals("autocompletar") && !Key.Equals("listAdded"))
                            {
                                if (valorFiltro.Contains("@@@"))
                                {
                                    bool facetaOR = false;
                                    if (mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo)))
                                    {
                                        facetaOR = mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo) && item.ComportamientoOr);
                                    }
                                    else if (mFacetaDW != null)
                                    {
                                        facetaOR = mFacetaDW.ListaFacetaObjetoConocimiento.Any(item => item.Faceta.Equals(KeySinTipo) && item.ComportamientoOr);
                                    }

                                    if (facetaOR && pListaFiltros[newKey].Count > 0)
                                    {
                                        //Cambio
                                        query.Append(ObtenerParteReciprocaQuery(LNiveles, nivelReciproca, query.ToString(), ref pUltimaFacetaAux, filtrosUsadosFacetaActual, listaFiltrosUsados, pListaFiltros[newKey][0]));

                                        if (pListaFiltros[newKey][0].Equals(NOTHING))
                                        {
                                            query.Append(" }");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        foreach (string valor in pListaFiltros[newKey])
                                        {
                                            query.Append(ObtenerParteReciprocaQuery(LNiveles, nivelReciproca, query.ToString(), ref pUltimaFacetaAux, filtrosUsadosFacetaActual, listaFiltrosUsados, valor));
                                        }
                                    }
                                }
                                else
                                {
                                    if (!pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso) || !Key.Equals("rdf:type"))
                                    {
                                        if (Key.Equals("rdf:type"))
                                        {
                                            query.Append("{");
                                        }

                                        if (string.IsNullOrEmpty(consultaReciproca))
                                        {
                                            if (nivelReciproca == 0)
                                            {
                                                query.Append($"?s {Key} ?{keySinPrefijo}. ");
                                            }
                                            else
                                            {
                                                keySinPrefijo = $"reciproca{NumeroReciproca}";
                                                query.Append($"?{keySinPrefijo} {Key} ?s. ");
                                            }
                                        }
                                        else
                                        {
                                            query.Append($"?s {Key} ?{QuitaPrefijo(valorFiltro)}. ");
                                        }
                                    }
                                }

                                query.Append(consultaReciproca);
                            }

                            if (Key.Equals("rdf:type") && pListaFiltros[Key].Count > 1)
                            {
                                // Recorrer los filtros del rdf:type, si alguno tiene un @ y un igual, hay que sacar un optional
                                foreach (string filtroOpcional in pListaFiltros[Key])
                                {
                                    if (filtroOpcional.Contains("@") && filtroOpcional.Contains("="))
                                    {
                                        string predicadoFiltroOpcional = filtroOpcional.Substring(filtroOpcional.IndexOf("@") + 1);

                                        string[] filtros = predicadoFiltroOpcional.Split('@');
                                        foreach (string filtro in filtros)
                                        {
                                            string predicadoFiltroOpcionalSinIgual = filtro.Substring(0, filtro.IndexOf("="));
                                            string keyOpcionalSinPrefijo = QuitaPrefijo(predicadoFiltroOpcionalSinIgual);

                                            int numericoIncremental = 0;
                                            while (query.ToString().Contains(keyOpcionalSinPrefijo + numericoIncremental))
                                            {
                                                numericoIncremental++;
                                            }
                                            keyOpcionalSinPrefijo += numericoIncremental;

                                            query.Append($" OPTIONAL {{?s {predicadoFiltroOpcionalSinIgual} ?{keyOpcionalSinPrefijo}. }} ");
                                        }
                                    }
                                }
                            }

                            if (Key.Equals("rdf:type") && !pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso))
                            {
                                query.Append(ObtenerParteFiltrosRdfType(Key, keySinPrefijo, tipoPropiedadFaceta, pListaFiltros, pListaFiltrosExtra));
                            }
                            else if ((Key.Equals("autocompletar")) && pListaFiltros[Key].Count > 0)
                            {
                                string facetaSinPrefijo = $"?{QuitaPrefijo(pNombreFaceta)}2000";
                                if (pNombreFaceta.Contains("@@@"))
                                {
                                    LNiveles = pNombreFaceta.Split(stringSeparators, StringSplitOptions.None);
                                    keySinPrefijo = QuitaPrefijo(LNiveles[LNiveles.Length - 1]);
                                    facetaSinPrefijo = $"?{keySinPrefijo}2000";
                                }

                                //Para el autocompletado de tags no hacer nada de las facetas.
                                if (pNombreFaceta != "sioc_t:Tag" && mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(pNombreFaceta)))
                                {
                                    Dictionary<string, List<string>> listaFiltrosCopia = new Dictionary<string, List<string>>(pListaFiltros);
                                    if (!pPestanyaID.Equals(Guid.Empty))
                                    {
                                        string campoFiltro = mEntityContext.ProyectoPestanyaBusqueda.Where(item => item.PestanyaID.Equals(pPestanyaID)).Select(item => item.CampoFiltro).FirstOrDefault();
                                        if (!string.IsNullOrEmpty(campoFiltro))
                                        {
                                            List<string> listaFiltros = campoFiltro.Replace("rdf:type=", "").Split('|').ToList();
                                            listaFiltrosCopia.TryAdd("rdf:type", listaFiltros);
                                        }
                                    }
                                    FacetaObjetoConocimientoProyecto faceta = ObtenerFacetaClaveTipo(pNombreFaceta, listaFiltrosCopia);
                                    nivelReciproca = int.Parse(faceta.Reciproca.ToString());
                                }

                                string tagsindescomponer = pListaFiltros["autocompletar"][0].ToString();
                                string filtro = UtilCadenas.ToSparql(tagsindescomponer);
                                if (pNombreFaceta.Substring(0, 2).Equals("cv"))
                                {
                                    query.Append($"{{?s gnoss:hasCv ?CV. ?CV {pNombreFaceta} {facetaSinPrefijo} FILTER (({facetaSinPrefijo} LIKE '");
                                    query.Append(filtro.Replace("\\'", ""));

                                    query.Append($"%') OR ({facetaSinPrefijo} LIKE '% {filtro}%')) }} ");
                                    if (listaItemsAEliminarQuery.Count > 0)
                                    {
                                        query.Append($" AND ({facetaSinPrefijo} NOT IN(");
                                        foreach (string valor in listaItemsAEliminarQuery)
                                        {
                                            query.Append($"'{valor}', ");
                                        }
                                        query.Remove(query.Length - 2, 2);
                                        query.Append("))");
                                    }
                                    query.Append(")} ");
                                }
                                else
                                {
                                    pUltimaFacetaAux = "autocompletar";

                                    if (pNombreFaceta.Contains("@@@"))
                                    {
                                        query.Append($"{{{ObtenerParteReciprocaQuery(LNiveles, nivelReciproca, query.ToString(), ref pUltimaFacetaAux, filtrosUsadosFacetaActual, listaFiltrosUsados, pListaFiltros[newKey][0])}");
                                        query.Append($" FILTER bif:contains(?{pUltimaFacetaAux}, '");
                                        query.Append(ObtenerFiltroBifContains(filtro, true));
                                        query.Append("') } ");
                                    }
                                    else
                                    {
                                        query.Append($"{{?s  {pNombreFaceta} {facetaSinPrefijo}.");
                                        query.Append($" FILTER bif:contains({facetaSinPrefijo}, '");
                                        query.Append(ObtenerFiltroBifContains(filtro, true));
                                        query.Append("') } ");
                                        pUltimaFacetaAux = facetaSinPrefijo.Replace("?", "");
                                    }
                                }
                            }
                            else if ((Key.Equals("listAdded")) && pListaFiltros[Key].Count > 0)
                            {
                                listaItemsAEliminarQuery.Clear();
                                foreach (string valor in pListaFiltros[newKey])
                                {
                                    listaItemsAEliminarQuery.Add(valor);
                                }

                                string facetaSinPrefijo = $"?{QuitaPrefijo(pNombreFaceta)}2000";
                                query.Append($" MINUS {{?s {pNombreFaceta} {facetaSinPrefijo} FILTER ({facetaSinPrefijo} in (");
                                foreach (string valor in pListaFiltros[newKey])
                                {
                                    query.Append($"'{valor}', ");
                                }
                                query.Remove(query.Length - 2, 2);
                                query.Append("))}");
                            }
                            else if (Key.Equals("search") && pListaFiltros[Key].Count > 0)
                            {
                                query.Append($"{{{ObtenerFiltroSearch(pListaFiltros, Key, pNombreFaceta, pOmitirPalabrasNoRelevantesSearch, true)}");

                                if (mConsultaResultados || EsPeticionPerfilPersonal(pListaFiltros))
                                {
                                    string serchTitleQuery = ObtenerFiltroSearch(pListaFiltros, Key, pNombreFaceta, pOmitirPalabrasNoRelevantesSearch, false);
                                    serchTitleQuery = serchTitleQuery.Replace("gnoss:search", "foaf:firstName").Replace("?search", "?searchTitle").Replace("?scoreSearch", "?scoreTitle");
                                    query.Append($" UNION {serchTitleQuery}");
                                }
                                query.Append("}");
                            }
                            else if ((Key.Equals("sioc_t:Tag")) && pListaFiltros[Key].Count > 0)
                            {
                                bool facetaOR = false;
                                if (mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo)))
                                {
                                    facetaOR = mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo) && item.ComportamientoOr);
                                }
                                else if (mFacetaDW != null)
                                {
                                    facetaOR = mFacetaDW.ListaFacetaObjetoConocimiento.Any(item => item.Faceta.Equals(KeySinTipo) && item.ComportamientoOr);
                                }

                                int auxN = 100;
                                if (facetaOR)
                                {
                                    query.Append($"?s sioc_t:Tag ?Etiqueta{auxN}. ");
                                }
                                else
                                {
                                    foreach (string valor in pListaFiltros[Key])
                                    {
                                        query.Append($"?s sioc_t:Tag ?Etiqueta{auxN}. ");
                                        auxN++;
                                    }
                                }

                                List<string> filtrosBusquedaTextBoxIntro = new List<string>();
                                List<string> filtrosBusquedaNormal = new List<string>();
                                foreach (string valor in pListaFiltros[Key])
                                {
                                    if (valor.StartsWith(">>"))
                                    {
                                        string valorLimpio = valor.Substring(2);
                                        if (!filtrosBusquedaTextBoxIntro.Contains(valorLimpio))
                                        {
                                            filtrosBusquedaTextBoxIntro.Add(valorLimpio);
                                        }
                                    }
                                    else
                                    {
                                        string valorConsulta = ObtenerValorParaFiltro(Key, valor, tipoPropiedadFaceta);

                                        if (!filtrosBusquedaNormal.Contains(valorConsulta))
                                        {
                                            filtrosBusquedaNormal.Add(valorConsulta);
                                        }
                                    }
                                }

                                auxN = 100;
                                if (filtrosBusquedaNormal.Count > 0)
                                {
                                    if (filtrosBusquedaNormal.Count == 1 || !facetaOR)
                                    {
                                        query.Append(" FILTER (");
                                        foreach (string filtro in filtrosBusquedaNormal)
                                        {
                                            query.Append($" ?Etiqueta{auxN}=");
                                            query.Append($"{filtro} and ");
                                            auxN++;
                                        }
                                        query.Remove(query.Length - 4, 4);
                                        query.Append(" )  ");
                                    }
                                    else
                                    {
                                        query.Append($" FILTER ( ?Etiqueta{auxN} in (");
                                        string coma = "";
                                        foreach (string filtro in filtrosBusquedaNormal)
                                        {
                                            query.Append(coma + filtro);
                                            coma = ", ";
                                        }
                                        query.Append(" ))  ");
                                    }
                                }

                                if (filtrosBusquedaTextBoxIntro.Any())
                                {
                                    foreach (string filtroBusquedaTextBoxIntro in filtrosBusquedaTextBoxIntro)
                                    {
                                        query.Append($" FILTER bif:contains (?Etiqueta{auxN}, '");
                                        query.Append(ObtenerFiltroBifContains(filtroBusquedaTextBoxIntro, false));
                                        query.Append("')");

                                        auxN++;
                                    }
                                }
                            }
                            else
                            {
                                if (!pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso) || !Key.Equals("rdf:type"))
                                {
                                    bool facetaOR = false;
                                    if (mFacetaDW != null && mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo)))
                                    {
                                        facetaOR = mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Any(item => item.Faceta.Equals(KeySinTipo) && item.ComportamientoOr.Equals(true));
                                    }

                                    query.Append(ObtenerParteFiltros_PorClave(pListaFiltros, newKey, ref keySinPrefijo, ref numeroFiltro, filtrosUsadosFacetaActual, ref KeySinTipo, tipoPropiedadFaceta, pTipoAlgoritmoTransformacion, pProyectoID, ref pNombreFaceta, ref numeroFiltros, ref Key, query.ToString(), facetaOR));
                                }
                                if (query.Length > 2 && query.ToString().EndsWith(". "))
                                {
                                    query.Remove(query.Length - 2, 2);
                                }
                            }
                        }


                        if (facetasPorTipo)
                        {
                            listaFacetasPorTipo.Add(newKey, query.ToString());
                            query.Clear();
                        }
                    }

                    if (filtrosOpcionales.Contains(newKey))
                    {
                        query.Append(" } ");
                    }
                }
            }

            if (facetasPorTipo)
            {
                foreach (string fac in listaFacetasPorTipo.Keys)
                {
                    if (!fac.StartsWith("rdf:type") && !fac.Contains(";"))
                    {
                        //Si la faceta no es de ningun tipo ni es rdf:type se pone FUERA
                        query.Append(listaFacetasPorTipo[fac]);
                    }
                }

                Dictionary<string, List<string>> diccionarioFiltrosTipo = new Dictionary<string, List<string>>();
                //Agrupamos los filtros por tipo
                foreach (string tipo in pListaFiltros["rdf:type"])
                {
                    StringBuilder filtro = new StringBuilder();
                    foreach (string fac in listaFacetasPorTipo.Keys.Where(item => item.StartsWith($"{tipo};")))
                    {
                        bool facetaSearchPersonalizada = false;
                        string facAtual = fac.Replace(tipo + ";", "");
                        if (pFiltrosSearchPersonalizados != null)
                        {
                            foreach (string keySearch in pFiltrosSearchPersonalizados.Keys)
                            {
                                if (keySearch == facAtual)
                                {
                                    string valorFiltro = pListaFiltros[fac][0];
                                    string filtroWhere = pFiltrosSearchPersonalizados[keySearch].Item1;
                                    if (!string.IsNullOrEmpty(pNombreFaceta) && !string.IsNullOrEmpty(pFiltrosSearchPersonalizados[keySearch].Item3))
                                    {
                                        //Si se trata de una faceta y tiene filtro para facetas cogemos el filtro de facetas
                                        filtroWhere = pFiltrosSearchPersonalizados[keySearch].Item3;
                                    }
                                    if (!string.IsNullOrEmpty(filtroWhere))
                                    {
                                        //Ejemplos:
                                        //1.-
                                        //|||[PARAMETROESPACIO]||FILTER (?searchTitle LIKE "*[PARAMETROESPACIOIN]*").||| 
                                        //Hace un split de valorFiltro y pone una aparicion de 'FILTER (?searchTitle LIKE "*[PARAMETROESPACIOIN]*").' por cada palabra sustituyendo [PARAMETROESPACIOIN] por la palabra
                                        //2.-
                                        //[PARAMETRO]
                                        //Sustituye [PARAMETRO] por valorFiltro
                                        //3.-
                                        //|||[PARAMETROESPACIOULTIMODIFERENTE]||FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN] *").||FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN]*").||| 
                                        //Hace un split de valorFiltro y pone una aparicion de 'FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN] *").' por cada palabra sustituyendo [PARAMETROESPACIOIN] por la palabra, excepto en la última palabra que pone 'FILTER (?searchTitle LIKE "* [PARAMETROESPACIOIN]*").' sustituyendo [PARAMETROESPACIOIN] por la palabra
                                        string queryPersonalizada = filtroWhere;
                                        string[] filtroEspacios = valorFiltro.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                        while (queryPersonalizada.Contains("|||[PARAMETROESPACIOULTIMODIFERENTE]||"))
                                        {
                                            int inicio = queryPersonalizada.IndexOf("|||[PARAMETROESPACIOULTIMODIFERENTE]||");
                                            int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                            string[] querysAuxArray = queryPersonalizada.Substring(inicio + 3, fin - inicio - 6).Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

                                            string queryAuxModificada = querysAuxArray[1];
                                            string queryAuxModificadaFin = querysAuxArray[2];

                                            StringBuilder queryAuxFin = new StringBuilder();
                                            int i = 0;
                                            foreach (string palabra in filtroEspacios)
                                            {
                                                i++;
                                                if (i == filtroEspacios.Length)
                                                {
                                                    queryAuxFin.Append($" {queryAuxModificadaFin.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                                }
                                                else
                                                {
                                                    queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                                }

                                            }
                                            queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                        }

                                        while (queryPersonalizada.Contains("|||[PARAMETROESPACIO]||"))
                                        {
                                            int inicio = queryPersonalizada.IndexOf("|||[PARAMETROESPACIO]||");
                                            int fin = queryPersonalizada.IndexOf("|||", inicio + 1) + 3;
                                            string queryAuxModificada = queryPersonalizada.Substring(inicio, fin - inicio).Replace("|||[PARAMETROESPACIO]||", "").Replace("|||", "");
                                            StringBuilder queryAuxFin = new StringBuilder();
                                            foreach (string palabra in filtroEspacios)
                                            {
                                                queryAuxFin.Append($" {queryAuxModificada.Replace("[PARAMETROESPACIOIN]", palabra)} ");
                                            }
                                            queryPersonalizada = queryPersonalizada.Substring(0, inicio) + queryAuxFin + queryPersonalizada.Substring(fin);
                                        }
                                        queryPersonalizada = queryPersonalizada.Replace("[PARAMETRO]", valorFiltro);

                                        filtro.Append($" {queryPersonalizada} ");
                                        facetaSearchPersonalizada = true;
                                    }
                                }
                            }

                        }
                        if (!facetaSearchPersonalizada)
                        {
                            //Si la faceta es del tipo se pone
                            filtro.Append(listaFacetasPorTipo[fac].Replace($"{tipo};", ""));
                        }
                    }

                    string cadenaFiltro = filtro.ToString();

                    if (diccionarioFiltrosTipo.ContainsKey(cadenaFiltro))
                    {
                        diccionarioFiltrosTipo[cadenaFiltro].Add(tipo);
                    }
                    else
                    {
                        List<string> tipos = new List<string>();
                        tipos.Add(tipo);
                        diccionarioFiltrosTipo.Add(cadenaFiltro, tipos);
                    }
                }

                //Aplicamos los filtros a los tipos en comun
                string union = "";
                foreach (string filtro in diccionarioFiltrosTipo.Keys)
                {
                    query.Append(union);
                    query.Append(" { ");
                    query.Append(filtro);
                    if (diccionarioFiltrosTipo[filtro].Count == 1)
                    {
                        string objetoFiltro = diccionarioFiltrosTipo[filtro][0];
                        if (objetoFiltro.Contains("@") && objetoFiltro.Contains(":"))
                        {
                            query.Append(ObtenerQueryFiltrosCondicionados(objetoFiltro));
                        }
                        else
                        {
                            query.Append($" ?s rdf:type ?rdftype.  FILTER ( ?rdftype ='{objetoFiltro}') ");
                        }
                    }
                    else
                    {
                        query.Append(ObtenerQueryFiltrosCondicionados_IN(diccionarioFiltrosTipo, filtro));
                    }

                    query.Append(" } ");
                    union = " UNION ";
                }
            }

            if (!query.ToString().Contains("?s rdf:type ?rdftype") && !pTipoProyecto.Equals(TipoProyecto.CatalogoNoSocialConUnTipoDeRecurso))
            {

                if (omitirRdfType || pConsultaFiltrosBusqueda)
                {
                    query.Append(SOLO_TRAER_RDFTYPE);
                }
                else
                {
                    if (pProyectoID.Equals(ProyectoAD.MetaProyecto.ToString()))
                    {
                        query.Append(TIPOS_METABUSCADOR_MYGNOSS);
                    }
                    else if (pExcluirPersonas || (!pEsMiembroComunidad && !pProyectoID.Equals(ProyectoAD.MetaProyecto.ToString())))
                    {
                        query.Append(TIPOS_METABUSCADOR_USUARIO_INVITADO);
                    }
                    else
                    {
                        query.Append(TIPOS_METABUSCADOR);
                    }

                    query.Remove(query.Length - 4, 4);

                    foreach (string ontologia in mListaItemsBusquedaExtra)
                    {
                        if (!ontologia.Contains("@"))
                        {
                            query.Append($", '{ontologia}' ");
                        }
                    }
                    query.Append("))");
                }
            }

            return query.ToString();
        }

        private static string ObtenerQueryFiltrosCondicionados_IN(Dictionary<string, List<string>> pDiccionarioFiltrosTipo, string pFiltro)
        {
            StringBuilder declaracionVariables = new StringBuilder(" ?s rdf:type ?rdftype. ");
            StringBuilder contenidoFilter = new StringBuilder(" FILTER ( (");

            foreach (string tipo in pDiccionarioFiltrosTipo[pFiltro])
            {
                if (tipo.Contains("@"))
                {
                    // Separar el "@", coger el segundo trozo y agregar el filtro a la declaración de variables
                    string[] filtroYCondicion = tipo.Split('@');
                    contenidoFilter.Append($" ?rdftype = '{filtroYCondicion[0]}' AND ");

                    if (filtroYCondicion[1].Contains("="))
                    {
                        string[] datosCondicion = filtroYCondicion[1].Split('=');
                        string predicadoCondicion = datosCondicion[0];
                        string valorCondicion = datosCondicion[1];

                        string tempDeclaracion = $" ?s {datosCondicion[0]} ?{QuitaPrefijo(predicadoCondicion)}.";
                        if (!declaracionVariables.ToString().Contains(tempDeclaracion))
                        {
                            declaracionVariables.Append(tempDeclaracion);
                        }

                        contenidoFilter.Append($" ?{QuitaPrefijo(predicadoCondicion)} = ");
                        if (valorCondicion.StartsWith("http://"))
                        {
                            contenidoFilter.Append($"<{valorCondicion}> ");
                        }
                        else if (valorCondicion.Contains(":"))
                        {
                            contenidoFilter.Append(valorCondicion);
                        }
                        else
                        {
                            contenidoFilter.Append($"'{valorCondicion}' ");
                        }
                    }
                }
                else
                {
                    contenidoFilter.Append($" ?rdftype = '{tipo}'");
                }

                contenidoFilter.Append(") OR (");
            }

            if (contenidoFilter.ToString().EndsWith(") OR ("))
            {
                contenidoFilter.Remove(contenidoFilter.Length - 4, 4);
            }

            contenidoFilter.Append(" )");

            return declaracionVariables.ToString() + contenidoFilter.ToString();
        }

        private FacetaObjetoConocimientoProyecto ObtenerFacetaClaveTipo(string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros)
        {
            string claveFiltroTipo = "rdf:type";
            List<string> listaRdfType = new List<string>();
            if (pListaFiltros.ContainsKey(claveFiltroTipo))
            {
                listaRdfType = pListaFiltros[claveFiltroTipo];
            }
            FacetaObjetoConocimientoProyecto facetaObjetoConocimientoProyecto = mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Find(item => item.Faceta.Equals(pClaveFaceta) && listaRdfType.Contains(item.ObjetoConocimiento.ToLower()));

            if (facetaObjetoConocimientoProyecto == null)
            {
                facetaObjetoConocimientoProyecto = mFacetaDW.ListaFacetaObjetoConocimientoProyecto.Find(item => item.Faceta.Equals(pClaveFaceta));
            }

            return facetaObjetoConocimientoProyecto;
        }

        private static string ObtenerQueryFiltrosCondicionados(string pObjetoFiltro)
        {
            StringBuilder query = new StringBuilder();

            // Es una faceta condicionada por un filtro
            string[] filtroCondicionado = pObjetoFiltro.Split('@');
            string filtroRdfType = filtroCondicionado[0];

            Dictionary<string, List<string>> dicFiltrosCondicionados = new Dictionary<string, List<string>>();
            for (int i = 1; i < filtroCondicionado.Length; i++)
            {
                if (filtroCondicionado[i].Contains("="))
                {
                    string[] desgloseDeCondicion = filtroCondicionado[i].Split('=');
                    if (!dicFiltrosCondicionados.ContainsKey(desgloseDeCondicion[0]))
                    {
                        dicFiltrosCondicionados.Add(desgloseDeCondicion[0], new List<string>());
                    }

                    if (!dicFiltrosCondicionados[desgloseDeCondicion[0]].Contains(desgloseDeCondicion[1]))
                    {
                        dicFiltrosCondicionados[desgloseDeCondicion[0]].Add(desgloseDeCondicion[1]);
                    }
                }
            }

            query.Append(" ?s rdf:type ?rdftype. ");

            foreach (string predicado in dicFiltrosCondicionados.Keys)
            {
                query.Append($" ?s {predicado} ?{QuitaPrefijo(predicado)}. ");
            }

            query.Append($" FILTER ( ?rdftype ='{filtroRdfType}'");
            foreach (string predicado in dicFiltrosCondicionados.Keys)
            {
                List<string> valoresFiltros = dicFiltrosCondicionados[predicado];
                query.Append($" AND ?{QuitaPrefijo(predicado)}");

                if (valoresFiltros.Count > 0)
                {
                    query.Append(" IN (");
                }
                else
                {
                    query.Append(" = ");
                }

                foreach (string valorFiltro in valoresFiltros)
                {

                    if (valorFiltro.StartsWith("http://"))
                    {
                        query.Append($"<{valorFiltro}> ");
                    }
                    else if (valorFiltro.Contains(":"))
                    {
                        query.Append(valorFiltro);
                    }
                    else
                    {
                        query.Append($"'{valorFiltro}' ");
                    }
                    query.Append(", ");
                }

                if (query.ToString().EndsWith(", "))
                {
                    query.Remove(query.Length - 2, 2);
                }

                if (valoresFiltros.Count > 0)
                {
                    query.Append(" ) ");
                }
            }

            query.Append(") ");

            return query.ToString();
        }

        private string ObtenerParteFiltros_PorClave(Dictionary<string, List<string>> pListaFiltros, string pNewKey, ref string pKeySinPrefijo, ref string pNumeroFiltro, Dictionary<string, List<string>> pFiltrosUsadosFacetaActual, ref string pKeySinTipo, short pTipoPropiedadFaceta, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, string pProyectoID, ref string pNombreFaceta, ref int pNumeroFiltros, ref string pKey, string pQuery, bool pComportamientoOr)
        {
            StringBuilder sbQuery = new StringBuilder();
            List<string> valoresIn = new List<string>();

            foreach (string filtro in pListaFiltros[pNewKey])
            {
                pKeySinPrefijo += pNumeroFiltro;

                if (pFiltrosUsadosFacetaActual.ContainsKey(filtro) && pFiltrosUsadosFacetaActual[filtro].Count > 0)
                {
                    pKeySinPrefijo = pFiltrosUsadosFacetaActual[filtro][pFiltrosUsadosFacetaActual[filtro].Count - 1];
                }

                string valor = filtro;
                if (valor.Contains(FACETA_CONDICIONADA))
                {
                    string valorAux = valor.Substring(0, valor.IndexOf(FACETA_CONDICIONADA));
                    // Es un filtro de una faceta múltiple, compruebo si el filtro viene de una sub-faceta de tipo rango
                    bool esRango = false;

                    if (valorAux.Contains('-'))
                    {
                        string[] valoresRango = valorAux.Split('-');
                        if (valoresRango.Length > 0 && valoresRango.Length <= 2)
                        {
                            int aux;
                            if (Int32.TryParse(valoresRango[0], out aux) && (valoresRango.Length == 1 || Int32.TryParse(valoresRango[1], out aux)))
                            {
                                if (!PropiedadesRango.Contains(pKey.Substring(pKey.IndexOf(":") + 1)))
                                {
                                    PropiedadesRango.Add(pKey.Substring(pKey.IndexOf(":") + 1));
                                }

                                esRango = true;
                            }
                        }
                    }

                    if (!esRango)
                    {
                        // Si es un rango, se le quita el trozo de faceta condicionada en el método ObtenerParteFiltros_PorClave_NOIN
                        valor = valor.Substring(0, valor.IndexOf(FACETA_CONDICIONADA));
                    }
                }

                valor = ObtenerParteFiltros_PorClave_ValorRealFiltro(valor, pKeySinTipo, pTipoPropiedadFaceta);


                if (pComportamientoOr && pListaFiltros[pNewKey].Count > 1)
                {
                    valoresIn.Add(valor);
                }
                else
                {
                    sbQuery.Append(ObtenerParteFiltros_PorClave_NOIN(ref valor, ref pQuery, ref pKey, ref pKeySinPrefijo, ref pKeySinTipo, pTipoPropiedadFaceta, pTipoAlgoritmoTransformacion, ref pKeySinTipo, ref pProyectoID, ref pNombreFaceta, ref pListaFiltros));

                    mLoggingService.AgregarEntrada($"Así queda el tema NOIN: {sbQuery.ToString()}");
                    pNumeroFiltro = pNumeroFiltros.ToString();
                    pNumeroFiltros++;
                }
            }

            if (valoresIn.Count > 0)
            {
                sbQuery.Append(ObtenerParteFiltros_PorClave_IN(pKeySinPrefijo, valoresIn, pKeySinTipo, pTipoPropiedadFaceta));
            }

            return sbQuery.ToString();
        }

        private string ObtenerParteFiltros_PorClave_NOIN(ref string pValor, ref string pQuery, ref string pKey, ref string pKeySinPrefijo, ref string pNewKey, short pTipoPropiedadFaceta, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, ref string pKeySinTipo, ref string pProyectoID, ref string pNombreFaceta, ref Dictionary<string, List<string>> pListaFiltros)
        {
            StringBuilder sbQuery = new StringBuilder();

            bool esRangoFacetaMultiple = false;
            if (pValor.Contains(FACETA_CONDICIONADA.ToLower()))
            {
                esRangoFacetaMultiple = true;
            }

            if (!pQuery.Contains($"?s {pKey} ?{pKeySinPrefijo}. ") && !pKey.Substring(0, 2).Equals("cv") && !pKey.Equals("search") && !pNewKey.Contains("@@@") && !pQuery.Contains($"{pKey} ?s. "))
            {
                sbQuery.Append($" ?s {pKey} ?{pKeySinPrefijo}. ");
            }
            else
            {
                if (pKey.Substring(0, 2).Equals("cv"))
                {
                    sbQuery.Append($"?CV {pKey} ?{pKeySinPrefijo}. ");
                }
            }
            //************************************************************************************************************************************************** NO QUITAR *******************************************************************************************************************************************************************************************/

            /*Metodo para acceder las coordenadas del mapa.

            wgs84_pos:geometry=intersects(-3.6868, 40.4113)
            bif:st_intersects

            Para ello vamos a comprobar cuantos valores vienen dentro de intersects.
            Si vienen mas de 5 puntos y el ultimo es igual al primero es un poligono.
            Si viene solo uno es un punto.
            */

            if (pKeySinTipo.Equals("wgs84_pos:geometry"))
            {

                sbQuery = ObtenerParteFiltros_PorClave_NOIN_Coordenadas(ref pValor, sbQuery);

            }
            else
            {
                sbQuery = ObtenerParteFiltros_PorClave_NOIN_NoCoordenadas(ref pValor, ref pKey, ref pKeySinPrefijo, pTipoPropiedadFaceta, pTipoAlgoritmoTransformacion, ref pKeySinTipo, ref pProyectoID, ref pNombreFaceta, ref pListaFiltros, esRangoFacetaMultiple, sbQuery);
            }


            return sbQuery.ToString();
        }

        private static StringBuilder ObtenerParteFiltros_PorClave_NOIN_Coordenadas(ref string pValor, StringBuilder sbQuery)
        {
            sbQuery.Append(" FILTER ( bif:st_intersects ( bif: st_geomfromtext ( ?p ),");
            double[,] puntos = JsonConvert.DeserializeObject<double[,]>(pValor);
            //Si es un punto ponemos el filtro del punto
            if (puntos.Length == 1)
            {
                sbQuery.Append($"bif:st_point ({puntos[0, 0]},{puntos[0, 1]})))");
            }
            else
            {
                //establecemos el primer punto y el ultimo
                int numPuntos = puntos.Length;
                double[] primerPunto = { puntos[0, 0], puntos[0, 1] };
                double[] ultimoPunto = { puntos[numPuntos - 1, 0], puntos[numPuntos - 1, 1] };
                bool esPoligono = false;
                int puntosRestantes = 0;

                //comprobamos si son guales el ultimo punto y el primero para saber si es un poligono
                if (primerPunto[0] == ultimoPunto[0] && primerPunto[1] == ultimoPunto[1])
                {
                    esPoligono = true;
                }
                //comprobamos cuantos puntos quedan para hacer un poligono
                if (numPuntos < 6)
                {
                    puntosRestantes = 6 - numPuntos;
                }

                if (esPoligono)
                {
                    sbQuery.Append("bif:st_geomfromtext(\"POLYGON(");
                }
                else
                {
                    sbQuery.Append("bif:st_geomfromtext(\"MULTIPOINT(");
                }

                //recorremos los puntos para ponerlos en la lista
                for (int i = 0; i < puntos.Length; i++)
                {
                    //Si es la primera que no separa por una com
                    if (i != 0)
                    {
                        sbQuery.Append(", ");
                    }
                    sbQuery.Append(puntos[i, 0] + " " + puntos[i, 1]);

                    //Comprobamos si los puntos restantes son mayores que 0 y es poligono que cree los puntos necesarios para poder hacer un poligono.
                    if (puntosRestantes > 0 && esPoligono)
                    {
                        //Vamos a crear todos los puntos necesarios para hacer el poligono.
                        for (int x = 0; x < puntosRestantes; x++)
                        {
                            double decimales = 0.0001;
                            sbQuery.Append($", {(puntos[i, 0] + decimales * (x + 1)).ToString()} {(puntos[i, 0] + decimales * (x + 1)).ToString()}");
                        }
                    }
                }

                sbQuery.Append(")\")");
            }

            return sbQuery;
        }

        private StringBuilder ObtenerParteFiltros_PorClave_NOIN_NoCoordenadas(ref string pValor, ref string pKey, ref string pKeySinPrefijo, short pTipoPropiedadFaceta, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, ref string pKeySinTipo, ref string pProyectoID, ref string pNombreFaceta, ref Dictionary<string, List<string>> pListaFiltros, bool pEsRangoFacetaMultiple, StringBuilder sbQuery)
        {
            if (pEsRangoFacetaMultiple || pTipoPropiedadFaceta == (short)TipoPropiedadFaceta.Siglo)
            {
                sbQuery.Append($" FILTER (xsd:integer(?{pKeySinPrefijo})");
            }
            else
            {
                sbQuery.Append($" FILTER (?{pKeySinPrefijo} ");
            }

            if ((pKey.Equals("gnoss:hasnumeroVotos")) || (pKey.Equals("gnoss:hasnumeroComentarios")) || (pKey.Equals("gnoss:hasnumeroVisitas")) || (pKey.Equals("gnoss:hasfechapublicacion")) || (pKey.Equals("gnoss:hasfechaAlta")) || (pKey.Equals("arecipe:nutrition")) || (pKey.Equals("gnoss:hasfechaAlta")) || (pKey.Equals("nmo:sentDate")) || (PropiedadesRango.Contains(pKey.Substring(pKey.LastIndexOf(":") + 1))) || (PropiedadesFecha.Contains(pKey.Substring(pKey.LastIndexOf(":") + 1))) || pTipoPropiedadFaceta == (short)TipoPropiedadFaceta.Fecha || pTipoPropiedadFaceta == (short)TipoPropiedadFaceta.Calendario || pTipoPropiedadFaceta == (short)TipoPropiedadFaceta.CalendarioConRangos)
            {
                sbQuery.Append(ObtenerParteFiltros_PorClave_FiltroFechaORango(pKey, pTipoPropiedadFaceta, pValor, pKeySinPrefijo, pKeySinTipo));
            }
            else if (pTipoPropiedadFaceta == (short)TipoPropiedadFaceta.Siglo)
            {
                sbQuery.Append(ObtenerParteFiltros_PorClave_FiltroSiglo(pValor, pTipoAlgoritmoTransformacion, pKeySinPrefijo));
            }
            else if (pKey.Equals("gnoss:hastipodoc") || pKey.Equals("gnoss:hastipodocExt"))
            {
                sbQuery.Append($" = '{pValor}'). ");
            }
            //Si es el caso de una persona de una organizacion (no administrador)
            else if (pKey.Equals("gnoss:hasSpaceIDPublicador"))
            {
                sbQuery = new StringBuilder(ObtenerParteFiltros_PorClave_FiltroPersonaOrganizacion(pListaFiltros, pValor, pTipoPropiedadFaceta, pProyectoID, pNombreFaceta, pKey));
            }
            else
            {
                mLoggingService.AgregarEntrada($"Entra para ponerle comillas NOIN: {pValor}");

                if (pValor.StartsWith(">>"))
                {
                    // Quitamos el parámetro del filtro
                    sbQuery = sbQuery.Replace($"(?{pKeySinPrefijo} ", $" (bif:contains (?{pKeySinPrefijo}, '");

                    pValor = pValor.Substring(2);
                    sbQuery.Append(ObtenerFiltroBifContains(pValor, false));

                    sbQuery.Append("')) ");
                }
                else
                {
                    if (pValor.StartsWith("<>"))
                    {
                        pValor = pValor.Substring(2);
                        sbQuery.Append(" LIKE ");
                    }
                    // Se agregan estos filtros a un IN
                    else
                    {
                        sbQuery.Append(" = ");
                    }
                    string valorConsulta = ObtenerValorParaFiltro(pKeySinTipo, pValor, pTipoPropiedadFaceta);
                    sbQuery.Append($"{valorConsulta}). ");

                    mLoggingService.AgregarEntrada($"¿Le habrá puesto comillas NOIN?: {valorConsulta}");
                }
            }

            return sbQuery;
        }

        private string ObtenerParteFiltros_PorClave_IN(string pKeySinPrefijo, List<string> pValoresIn, string pKeySinTipo, short pTipoPropiedadFaceta)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append($" FILTER(?{pKeySinPrefijo}");

            if (pValoresIn.Count > 1)
            {
                sbQuery.Append(" IN (");
                foreach (string valor in pValoresIn)
                {
                    string valorConsulta = ObtenerValorParaFiltro(pKeySinTipo, valor, pTipoPropiedadFaceta);
                    sbQuery.Append($"{valorConsulta}, ");
                    mLoggingService.AgregarEntrada($"¿Le habrá puesto comillas IN?: {valorConsulta}");
                }

                if (sbQuery.ToString().EndsWith(", "))
                {
                    string tempQuery = sbQuery.ToString();
                    tempQuery = tempQuery.Substring(0, tempQuery.Length - 2);
                    sbQuery = new StringBuilder(tempQuery);
                }

                sbQuery.Append(")");
            }
            else
            {
                sbQuery.Append(" = ");
                string valorConsulta = ObtenerValorParaFiltro(pKeySinTipo, pValoresIn[0], pTipoPropiedadFaceta);
                sbQuery.Append(valorConsulta);
            }

            sbQuery.Append(")");

            mLoggingService.AgregarEntrada($"Así queda el tema IN: {sbQuery.ToString()}");

            return sbQuery.ToString();
        }

        private string ObtenerParteFiltros_PorClave_FiltroFechaORango(string pKey, short pTipoPropiedadFaceta, string pValor, string pKeySinPrefijo, string pKeySinTipo)
        {
            StringBuilder sbQuery = new StringBuilder();

            string suplementoInicio = "";
            string suplementoFin = "";
            if (pKey.Equals("gnoss:hasfechapublicacion") || pKey.Equals("nmo:sentDate") || pKey.Equals("gnoss:hasfechaAlta") || (PropiedadesFecha.Contains(pKey.Substring(pKey.LastIndexOf(":") + 1))) || pTipoPropiedadFaceta == (short)TipoPropiedadFaceta.Siglo)
            {
                suplementoInicio = "000000";
                suplementoFin = "235959";
            }

            bool esRangoFacetaMultiple = false;
            if (pValor.Contains(FACETA_CONDICIONADA.ToLower()))
            {
                pValor = pValor.Substring(0, pValor.IndexOf(FACETA_CONDICIONADA.ToLower()));
                esRangoFacetaMultiple = true;
            }

            if (pValor.Contains("-"))
            {
                if (pValor.EndsWith('-'))
                {
                    //Se buscan los valores mayores que un valor
                    sbQuery.Append($" >= {pValor.Remove(pValor.Length - 1)}{suplementoInicio}). ");
                }
                else if (pValor.StartsWith('-') && pValor.LastIndexOf("-") == 0)
                {
                    //Se buscan los valores menores que un valor
                    sbQuery.Append($" <= {pValor.Substring(1)}{suplementoFin}). ");
                }
                else
                {
                    //Se buscan los valores entre un rango
                    int posSeparador = pValor.IndexOf('-', 1);
                    string valor1 = pValor.Substring(0, posSeparador);

                    if (valor1.StartsWith('-'))
                    {
                        valor1 = $"-1*{valor1.Substring(1)}";
                    }

                    string valor2 = pValor.Substring(posSeparador + 1);

                    if (valor2.StartsWith('-'))
                    {
                        valor2 = $"-1*{valor2.Substring(1)}";
                    }

                    string variableFiltro = $"?{pKeySinPrefijo}";
                    if (esRangoFacetaMultiple)
                    {
                        variableFiltro = $"xsd:int(?{pKeySinPrefijo})";
                    }

                    sbQuery.Append($" >= {valor1}{suplementoInicio} AND {variableFiltro} < {valor2}{suplementoFin}). ");
                }
            }
            else
            {
                //Se busca el valor exacto de un filtro con rango
                sbQuery.Append($" = {ObtenerValorParaFiltro(pKeySinTipo, pValor, pTipoPropiedadFaceta)}). ");
            }

            return sbQuery.ToString();
        }

        private static string ObtenerParteFiltros_PorClave_FiltroSiglo(string pValor, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, string pKeySinPrefijo)
        {
            StringBuilder sbQuery = new StringBuilder();

            string suplemento = "000000";

            if (pValor.LastIndexOf("-") > 1)
            {
                int posSeparador = pValor.IndexOf('-', 1);
                string valor1 = pValor.Substring(0, posSeparador);

                if (valor1.StartsWith('-'))
                {
                    valor1 = $"-1*{valor1.Substring(1)}";
                }

                string valor2 = pValor.Substring(posSeparador + 1);

                if (valor2.StartsWith('-'))
                {
                    valor2 = $"-1*{valor2.Substring(1)}";
                }

                if (pTipoAlgoritmoTransformacion == TiposAlgoritmoTransformacion.Rangos)
                {
                    sbQuery.Append($" > {valor1}{suplemento} AND xsd:integer(?{pKeySinPrefijo}) <= {valor2}{suplemento}). ");
                }
                else
                {
                    sbQuery.Append($" >= {valor1}{suplemento} AND xsd:integer(?{pKeySinPrefijo}) < {valor2}{suplemento}). ");
                }
            }
            else
            {
                sbQuery.Append($" = {pValor}{suplemento}) . ");
            }

            return sbQuery.ToString();
        }

        private static string ObtenerParteFiltros_PorClave_FiltroPersonaOrganizacion(Dictionary<string, List<string>> pListaFiltros, string pValor, short pTipoPropiedadFaceta, string pProyectoID, string pNombreFaceta, string pKey)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery = sbQuery.Replace("?s gnoss:hasSpaceIDPublicador ?gnosshasSpaceIDPublicador.  FILTER (?gnosshasSpaceIDPublicador", "    ");

            if (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Contains("Recurso Compartido") && !pListaFiltros["rdf:type"].Contains("Recurso Publicado"))
            {

                sbQuery.Append($"?s gnoss:hasSpaceIDCompartidor ?gnosshasSpaceIDCompartidor.  FILTER (?gnosshasSpaceIDCompartidor LIKE '%{QuitaPrefijo(pValor)}'). FILTER (bif:substring(STR(?siochasspace2),1, 49) = bif:substring(STR(?gnosshasSpaceIDCompartidor),1, 49))  ?s sioc:has_space ?siochasspace2 .     ");


            }
            else if (pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Contains("Recurso Publicado") && !pListaFiltros["rdf:type"].Contains("Recurso Compartido"))
            {
                sbQuery.Append($"?s gnoss:hasSpaceIDPublicador ?gnosshasSpaceIDPublicador.  FILTER (?gnosshasSpaceIDPublicador LIKE '%{QuitaPrefijo(pValor)}'). FILTER (bif:substring(STR(?siochasspace2),1, 49) = bif:substring(STR(?gnosshasSpaceIDPublicador),1, 49))  ?s sioc:has_space ?siochasspace2 .    ");
            }
            else
            {


                sbQuery.Append($"{{?s gnoss:hasSpaceIDPublicador ?gnosshasSpaceIDPublicador.  FILTER (?gnosshasSpaceIDPublicador LIKE '%{pValor}'). FILTER (bif:substring(STR(?siochasspace2),1, 49) = bif:substring(STR(?gnosshasSpaceIDPublicador),1, 49))  ?s sioc:has_space ?siochasspace2 . }} UNION {{?s gnoss:hasSpaceIDCompartidor ?gnosshasSpaceIDCompartidor.  FILTER (?gnosshasSpaceIDCompartidor LIKE '%{pValor}'). FILTER (bif:substring(STR(?siochasspace2),1, 49) = bif:substring(STR(?gnosshasSpaceIDCompartidor),1, 49))  ?s sioc:has_space ?siochasspace2 . }}  ");
            }

            if (pKey.Equals("sioc:has_space") && pNombreFaceta.Equals("gnoss:haspublicador") && pProyectoID.Contains("contribuciones/"))
            {

                sbQuery.Append(" ?s gnoss:haspublicadorSpace ?gnosshaspublicadorSpace. ");
                string valorConsulta = ObtenerValorParaFiltro(pKey, pValor, pTipoPropiedadFaceta);
                string valorconsultasingnoss = valorConsulta.Replace("gnoss:", "");
                sbQuery.Append($"   FILTER (?gnosshaspublicadorSpace LIKE '%{valorconsultasingnoss}'). ");
                sbQuery.Append("   FILTER (bif:substring(STR(?gnosshaspublicador2),1, 10) = bif:substring(STR(?gnosshaspublicadorSpace),1, 10))  ");
            }

            return sbQuery.ToString();
        }

        private string ObtenerParteFiltros_PorClave_ValorRealFiltro(string pFiltro, string pKeySinTipo, short pTipoPropiedadFaceta)
        {
            //Lista de facetas que no deben convertirse a mayusculas
            List<string> facetasNoConvertir = new List<string>();
            #region RellenoListaFacetasNoConvertir

            facetasNoConvertir.Add("skos:ConceptID");
            facetasNoConvertir.Add("gnoss:Estado");
            facetasNoConvertir.Add("gnoss:hasEstado");
            facetasNoConvertir.Add("gnoss:hasEstadoCorreccion");
            facetasNoConvertir.Add("gnoss:hasEstadoPP");
            facetasNoConvertir.Add("gnoss:hasOrigen");
            facetasNoConvertir.Add("gnoss:Leido");
            facetasNoConvertir.Add("nmo:from");
            facetasNoConvertir.Add("nmo:isRead");
            facetasNoConvertir.Add("sioc:has_space");
            facetasNoConvertir.Add("dce:type");
            facetasNoConvertir.Add("gnoss:type");
            facetasNoConvertir.Add("gnoss:hasComunidadOrigen");
            facetasNoConvertir.Add("gnoss:hasNombreCortoJerarquia");
            facetasNoConvertir.Add("cv:OrganizationUnitNameTitulaciones");
            facetasNoConvertir.Add("cv:OrganizationNameCentroEstudios");
            facetasNoConvertir.Add("cv:OrganizationNameDisciplina");
            facetasNoConvertir.Add("gnoss:hasSpaceIDPublicador");

            #endregion

            //bug7596
            string valor = pFiltro;
            if (!facetasNoConvertir.Contains(pKeySinTipo) && !EsIDGnoss(valor) && !UtilCadenas.EsMultiIdioma(valor) && !pTipoPropiedadFaceta.Equals((short)TipoPropiedadFaceta.TextoInvariable) && !pKeySinTipo.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
            {
                //Cualquier otro elemento que contenga un GUID no debe convertirse a minusculas o sino no devolverá resultados.
                if (valor.Contains(":"))
                {
                    string tempGuid = valor.Substring(valor.IndexOf(":") + 1);
                    Guid nuevoID;
                    if (!Guid.TryParse(tempGuid, out nuevoID))
                    {
                        valor = valor.ToLowerSearchGraph();
                    }
                }
                else
                {
                    valor = valor.ToLowerSearchGraph();
                }
            }

            if (valor.Equals(FILTRO_SIN_ESPECIFICAR))
            {
                valor = "";
            }

            #region SubType

            if (pKeySinTipo.EndsWith(FacetaAD.Faceta_Gnoss_SubType))
            {
                valor = ObtenerRealSubType(valor);
            }

            #endregion

            return valor;
        }

        private string ObtenerParteReciprocaQuery(string[] pListaNiveles, int pNivelReciproca, string pQuery, ref string pUltimaFacetaAux, Dictionary<string, List<string>> pFiltrosUsadosFacetaActual = null, List<string> pListaFiltrosUsados = null, string pValor = null)
        {
            StringBuilder reciproca = new StringBuilder();
            bool usarVariableUltimaFaceta = false;
            if (pUltimaFacetaAux.Equals("autocompletar"))
            {
                usarVariableUltimaFaceta = true;
            }
            pUltimaFacetaAux = "";

            // TODO: cambiar como se hace esto...
            // Si nivel reciproca == 1 ==> ?reciproca ?p1 ?s.
            // Si nivel reciproca == 2 ==> ?reciproca ?p1 ?o1. ?o1 ?p2 ?s.
            // Si nivel reciproca == 3 ==> ?reciproca ?p1 ?o1. ?o1 ?p2 ?o2. ?o2 ?p3 ?s.
            // ..... 

            // cambioReciprocoOk = true ==> cuando se enlaza la variale ?reciproca con el ?s.
            bool cambioReciprocoOk = false;

            // todosCambiosReciprocos = true ==> Tras enlazar, cuando el siguiente enlace ya se hace con ?reciproca y no con ?s
            bool todosCambiosReciprocos = false;

            if (!string.IsNullOrEmpty(pValor) && pValor.Equals(NOTHING))
            {
                reciproca.Append(" MINUS {");
            }

            string sujetoReciproca = $"?reciproca{NumeroReciproca}";
            if (pNivelReciproca != 0)
            {
                if (pNivelReciproca == 1)
                {
                    reciproca.Append($" {sujetoReciproca} {pListaNiveles[0]} ?s. ");
                    reciproca.Append($" {sujetoReciproca} {pListaNiveles[1]} ?");

                    pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[0]) + QuitaPrefijo(pListaNiveles[1]), pValor, pListaNiveles[1], true);

                    reciproca.Append($"{pUltimaFacetaAux}. ");
                    cambioReciprocoOk = true;
                    todosCambiosReciprocos = true;
                }
                else
                {
                    reciproca.Append($" {sujetoReciproca} {pListaNiveles[0]} ?");
                    pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[0]), pValor, pListaNiveles[0], pListaNiveles.Length.Equals(1));
                    reciproca.Append($"{pUltimaFacetaAux}. ");

                    if (pNivelReciproca == 2)
                    {
                        reciproca.Append($" ?{pUltimaFacetaAux} {pListaNiveles[1]} ?s. ");
                        cambioReciprocoOk = true;
                    }
                    else
                    {
                        reciproca.Append($" ?{pUltimaFacetaAux} {pListaNiveles[1]} ?");
                        pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[0]) + QuitaPrefijo(pListaNiveles[1]), pValor, pListaNiveles[1], pListaNiveles.Length.Equals(2));
                        reciproca.Append($"{pUltimaFacetaAux}. ");
                    }
                }
            }
            else
            {
                reciproca.Append($" ?s {pListaNiveles[0]} ?");
                pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[0]), pValor, pListaNiveles[0], false);
                reciproca.Append($"{pUltimaFacetaAux}. ");
                reciproca.Append($" ?{pUltimaFacetaAux} {pListaNiveles[1]} ?");
                pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[1]), pValor, pListaNiveles[1], pListaNiveles.Length.Equals(1));
                reciproca.Append($"{pUltimaFacetaAux}. ");
            }

            for (int i = 1; i < pListaNiveles.Length - 1; i++)
            {
                if ((i + 1) == pNivelReciproca && !cambioReciprocoOk && !todosCambiosReciprocos)
                {
                    reciproca.Append($" ?{pUltimaFacetaAux} {pListaNiveles[i + 1]} ?s. ");
                    cambioReciprocoOk = true;
                }
                else if (cambioReciprocoOk && !todosCambiosReciprocos)
                {
                    string condicion = $"{sujetoReciproca} {pListaNiveles[i + 1]}";
                    condicion += " ?";
                    pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[0]) + QuitaPrefijo(pListaNiveles[i + 1]), pValor, pListaNiveles[i + 1], i.Equals(pListaNiveles.Length - 2));
                    condicion += $"{pUltimaFacetaAux}. ";

                    if (!pQuery.Contains(condicion))
                    {
                        reciproca.Append(condicion);
                    }

                    todosCambiosReciprocos = true;
                }
                else
                {
                    string condicion = $" ?{pUltimaFacetaAux} {pListaNiveles[i + 1]}";
                    condicion += " ?";
                    pUltimaFacetaAux = ObtenerNombreFacetaAux(pListaFiltrosUsados, pFiltrosUsadosFacetaActual, QuitaPrefijo(pListaNiveles[i + 1]), pValor, pListaNiveles[i + 1], i.Equals(pListaNiveles.Length - 2));
                    condicion += $"{pUltimaFacetaAux}. ";

                    if (!pQuery.Contains(condicion))
                    {
                        reciproca.Append(condicion);
                    }
                }
            }

            if (!usarVariableUltimaFaceta)
            {
                pUltimaFacetaAux = "";
            }

            reciproca.Append(ObtenerFacetaCondicionada(pListaNiveles, pFiltrosUsadosFacetaActual, pListaFiltrosUsados, pValor));

            return reciproca.ToString();
        }

        private string ObtenerFacetaCondicionada(string[] pListaNiveles, Dictionary<string, List<string>> pFiltrosUsadosFacetaActual, List<string> pListaFiltrosUsados, string pValor)
        {
            StringBuilder condicionada = new StringBuilder();

            if (pFiltrosUsadosFacetaActual != null && pFiltrosUsadosFacetaActual.Count > 0 && !string.IsNullOrEmpty(pValor))
            {
                //Creo una nueva lista para no modificar la original: 
                Dictionary<string, List<string>> filtrosUsadosFacetaActual = new Dictionary<string, List<string>>(pFiltrosUsadosFacetaActual);
                filtrosUsadosFacetaActual[pValor] = new List<string>(pFiltrosUsadosFacetaActual[pValor]);
                List<string> listaFiltrosUsados = new List<string>(pListaFiltrosUsados);

                if (pValor.Contains(FACETA_CONDICIONADA))
                {
                    string facetaCondicionada = pValor.Substring(pValor.IndexOf(FACETA_CONDICIONADA) + FACETA_CONDICIONADA.Length);

                    string faceta = facetaCondicionada.Substring(0, facetaCondicionada.IndexOf('='));
                    string valor = facetaCondicionada.Substring(facetaCondicionada.IndexOf('=') + 1);

                    string[] niveles = faceta.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);

                    int contador = 0;
                    bool hayNivelesNuevos = false;
                    string ultimaFaceta = "";
                    string nivelAnterior = "s";
                    foreach (string nivel in niveles)
                    {
                        if (hayNivelesNuevos || !pListaNiveles.Contains(nivel))
                        {
                            hayNivelesNuevos = true;
                            if (contador > 0 && nivelAnterior.Equals("s"))
                            {
                                nivelAnterior = filtrosUsadosFacetaActual[pValor][contador - 1];
                            }

                            condicionada.Append($" ?{nivelAnterior} {nivel}");
                            ultimaFaceta = ObtenerNombreFacetaAux(listaFiltrosUsados, filtrosUsadosFacetaActual, QuitaPrefijo(nivel), pValor, nivel, contador.Equals(niveles.Length - 1));
                            condicionada.Append($" ?{ultimaFaceta}. ");

                            nivelAnterior = ultimaFaceta;
                        }
                        contador++;
                    }

                    // Añado a los filtros originales los filtros que he usado. No los añado al final porque la última variable añadida se usa para filtrar
                    pListaFiltrosUsados.InsertRange(0, listaFiltrosUsados.Except(pListaFiltrosUsados));

                    foreach (string claveFaceta in filtrosUsadosFacetaActual.Keys)
                    {
                        if (!pFiltrosUsadosFacetaActual.ContainsKey(claveFaceta))
                        {
                            pFiltrosUsadosFacetaActual.Add(claveFaceta, new List<string>());
                        }
                        pFiltrosUsadosFacetaActual[claveFaceta].InsertRange(0, filtrosUsadosFacetaActual[claveFaceta].Except(pFiltrosUsadosFacetaActual[claveFaceta]));
                    }

                    condicionada.Append($" FILTER(?{ultimaFaceta} = {ObtenerValorParaFiltro($"?{ultimaFaceta}", valor, (short)TipoPropiedadFaceta.Texto)}) ");
                }
            }

            return condicionada.ToString();
        }

        //Obtiene el nombre de la faceta auxiliar a utilizar (devolviendo la siguiente que no esté utilizada y almacenando la nueva)
        private string ObtenerNombreFacetaAux(List<string> pListaFiltrosUsados, Dictionary<string, List<string>> pFiltrosUsadosFacetaActual, string pNombreFaceta, string pValorFaceta, string pNivel, bool pEsUltimoNivel)
        {
            string facetaConAuxUltimoFiltro = string.Empty;
            string auxCoord = string.Empty;
            if (FacetaAuxEsCoordenada(pNivel))
            {
                // Si son coordenadas y comparten el mismo inicio, añadir el texto '_Coord' al final del filtro
                auxCoord = "_Coord";
            }

            int auxUltimoFiltro = 0;
            if (pListaFiltrosUsados != null)
            {
                if (!QuitaPrefijo(MandatoryRelacion).Contains(pNombreFaceta))
                {
                    facetaConAuxUltimoFiltro = pNombreFaceta + auxCoord + auxUltimoFiltro;
                    if (!UsarMismsaVariablesParaEntidadesEnFacetas || pEsUltimoNivel)
                    {
                        while (pListaFiltrosUsados.Contains(facetaConAuxUltimoFiltro))
                        {
                            auxUltimoFiltro++;
                            facetaConAuxUltimoFiltro = pNombreFaceta + auxCoord + auxUltimoFiltro;
                        }
                    }
                }
                else
                {
                    facetaConAuxUltimoFiltro = pNombreFaceta + MANDATORY;
                }

                pListaFiltrosUsados.Add(facetaConAuxUltimoFiltro);
                if (!string.IsNullOrEmpty(pValorFaceta))
                {
                    if (!pFiltrosUsadosFacetaActual.ContainsKey(pValorFaceta))
                    {
                        pFiltrosUsadosFacetaActual.Add(pValorFaceta, new List<string>());
                    }
                    pFiltrosUsadosFacetaActual[pValorFaceta].Add(facetaConAuxUltimoFiltro);
                }
            }

            return facetaConAuxUltimoFiltro;
        }

        private bool FacetaAuxEsCoordenada(string pNivel)
        {
            bool facetaEsCoord = false;
            if (mFacetaDW != null && mFacetaDW.ListaFacetaConfigProyMapa.Count > 0)
            {
                FacetaConfigProyMapa facetaMapa = mFacetaDW.ListaFacetaConfigProyMapa.FirstOrDefault();

                string separador = "@@@";
                if (facetaMapa.PropLongitud != null && facetaMapa.PropLongitud.Contains(separador) && facetaMapa.PropLatitud != null && facetaMapa.PropLatitud.Contains(separador))
                {
                    string tempPropLong = facetaMapa.PropLongitud;
                    tempPropLong = tempPropLong.Substring(0, tempPropLong.LastIndexOf(separador));

                    string tempPropLat = facetaMapa.PropLatitud;
                    tempPropLat = tempPropLat.Substring(0, tempPropLat.LastIndexOf(separador));

                    if (tempPropLong == tempPropLat)
                    {
                        if (pNivel.Contains(separador))
                        {
                            pNivel = pNivel.Substring(0, pNivel.LastIndexOf(separador));
                        }

                        string[] separadorArray = { separador };
                        foreach (string subentidad in pNivel.Split(separadorArray, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string prefijo = subentidad.Substring(0, subentidad.IndexOf(":"));
                            if (InformacionOntologias.Values.Any(ns => ns.Contains(prefijo)))//.ContainsValue(prefijo))
                            {
                                prefijo = InformacionOntologias.First(onto => onto.Value.Contains(prefijo)).Key;

                                string propiedadSubEntidad = subentidad.Substring(subentidad.IndexOf(":") + 1);

                                string tempPropSubentidad = prefijo + propiedadSubEntidad;

                                if (tempPropSubentidad.StartsWith('@'))
                                {
                                    tempPropSubentidad = tempPropSubentidad.Substring(1);
                                }

                                if (tempPropLat.Contains(tempPropSubentidad))
                                {
                                    facetaEsCoord = true;
                                }
                                else
                                {
                                    facetaEsCoord = false;
                                }
                            }
                            else
                            {
                                facetaEsCoord = false;
                            }
                        }
                    }
                }
            }

            return facetaEsCoord;
        }

        private static string ObtenerFiltroBifContains(string pFiltroCompleto, bool pAutocompletar)
        {
            StringBuilder filtroTemporal = new StringBuilder();

            string[] delimiter = { " ", "-" };
            string[] filtroTroceado = pFiltroCompleto.Split(delimiter, StringSplitOptions.None);
            for (int i = 0; i < filtroTroceado.Length; i++)
            {
                if (i + 1 == filtroTroceado.Length && pAutocompletar)
                {
                    // El último fragmento del filtro para el servicio autocompletar debe llevar '*'
                    if (filtroTroceado[i].Length >= 4)
                    {
                        filtroTemporal.Append($" NEAR \"{filtroTroceado[i]}*\"");
                    }
                }
                else
                {
                    filtroTemporal.Append($" NEAR \"{filtroTroceado[i]}\"");
                }
            }

            if (filtroTemporal.ToString().StartsWith(" NEAR "))
            {
                filtroTemporal = filtroTemporal.Remove(0, 6);
            }

            return filtroTemporal.ToString();
        }

        private string ObtenerParteFiltrosRdfType(string pKey, string pKeySinPrefijo, short pTipoPropiedadFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra)
        {
            string filtro = " FILTER (";

            filtro += $" ?{pKeySinPrefijo}";

            StringBuilder listaValores = new StringBuilder();
            StringBuilder listaValoresOpcionales = new StringBuilder();
            int numeroFiltros = 0;

            foreach (string valor in pListaFiltros[pKey])
            {
                if (!valor.Contains("@"))
                {
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, valor, pTipoPropiedadFaceta)}, ");
                    numeroFiltros++;
                }
                else
                {
                    // Se trata de un filtro rdftype condicionado?
                    if (valor.Contains("@") && valor.Contains("="))
                    {
                        string rdftypeCondicionado = valor.Substring(0, valor.IndexOf("@"));
                        string filtrosOpcionales = valor.Substring(valor.IndexOf("@") + 1);

                        string[] filtrosOpcionalesTroceados = filtrosOpcionales.Split('@');

                        if (filtrosOpcionalesTroceados.Length > 0)
                        {
                            listaValoresOpcionales.Append($" ?{pKeySinPrefijo} = '{rdftypeCondicionado}' AND (");
                            foreach (string filtroOpcional in filtrosOpcionalesTroceados)
                            {
                                string predicadoFiltroOpcional = filtroOpcional.Substring(0, filtroOpcional.IndexOf("="));
                                string keyOpcionalSinPrefijo = QuitaPrefijo(predicadoFiltroOpcional);

                                int numericoIncremental = 0;
                                while (listaValoresOpcionales.ToString().Contains(keyOpcionalSinPrefijo + numericoIncremental))
                                {
                                    numericoIncremental++;
                                }
                                keyOpcionalSinPrefijo += numericoIncremental;

                                string objetoPredicadoOpcional = filtroOpcional.Substring(filtroOpcional.IndexOf("=") + 1);

                                if (!objetoPredicadoOpcional.StartsWith('\''))
                                {
                                    objetoPredicadoOpcional = $"'{objetoPredicadoOpcional}'";
                                }


                                listaValoresOpcionales.Append($" ?{keyOpcionalSinPrefijo} = {objetoPredicadoOpcional} OR ");
                            }

                            if (listaValoresOpcionales.ToString().EndsWith(" OR "))
                            {
                                listaValoresOpcionales = listaValoresOpcionales.Remove(listaValoresOpcionales.Length - 4, 4);
                            }

                            listaValoresOpcionales.Append(")");
                        }
                    }
                }

                if (valor.Equals(BUSQUEDA_CLASE))
                {
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CLASE_SECUNDARIA, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CLASE_UNIVERSIDAD, pTipoPropiedadFaceta)}, ");
                    numeroFiltros += 2;
                }
                else if (valor.Equals(BUSQUEDA_COMUNIDADES))
                {
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_COMUNIDAD_NO_EDUCATIVA, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_COMUNIDAD_EDUCATIVA, pTipoPropiedadFaceta)}, ");
                    numeroFiltros += 2;
                }

                else if (valor.Equals(BUSQUEDA_CONTRIBUCIONES_RECURSOS))
                {
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMPARTIDO, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_PUBLICADO, pTipoPropiedadFaceta)}, ");
                    numeroFiltros += 2;
                }
                else if (valor.Equals(BUSQUEDA_CONTRIBUCIONES_COMENTARIOS))
                {
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMRECURSOS, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMDEBATES, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMDEBATES, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMENCUESTAS, pTipoPropiedadFaceta)}, ");
                    listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTRIBUCIONES_COMARTICULOBLOG, pTipoPropiedadFaceta)}, ");
                    numeroFiltros += 7;
                }
                else if (valor.Equals(BUSQUEDA_CONTACTOS))
                {
                    if (!pListaFiltros[pKey].Contains(BUSQUEDA_CONTACTOS_PERSONAL) && !pListaFiltros[pKey].Contains(BUSQUEDA_CONTACTOS_ORGANIZACION))
                    {
                        listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTACTOS_ORGANIZACION, pTipoPropiedadFaceta)}, ");
                        listaValores.Append($"{ObtenerValorParaFiltro(pKey, BUSQUEDA_CONTACTOS_PERSONAL, pTipoPropiedadFaceta)}, ");
                        numeroFiltros += 2;
                    }
                }
                else if ((valor.Equals(BUSQUEDA_RECURSOS) || valor.Equals(BUSQUEDA_AVANZADA)) && mListaItemsBusquedaExtra != null)
                {
                    foreach (string ontologia in mListaItemsBusquedaExtra)
                    {
                        if (!ontologia.Contains("@"))
                        {
                            listaValores.Append($"{ObtenerValorParaFiltro(pKey, ontologia, pTipoPropiedadFaceta)}, ");
                            numeroFiltros++;
                        }
                    }
                }
            }
            foreach (string valor in pListaFiltrosExtra)
            {
                if (!valor.Contains("@"))
                {
                    listaValores.Append($"'{valor}', ");
                    numeroFiltros++;
                }
            }

            listaValores = listaValores.Remove(listaValores.Length - 2, 2);

            if (numeroFiltros == 1)
            {
                filtro += " = ";
            }
            else if (numeroFiltros > 1)
            {
                filtro += " in (";
            }

            filtro += listaValores;

            if (numeroFiltros > 1)
            {
                //cierro el IN
                filtro += " )";
            }

            if (listaValoresOpcionales.Length > 0)
            {
                filtro += $" OR ({listaValoresOpcionales})";
            }

            filtro += " ) }";

            return filtro;
        }

        private string ObtenerFiltroSearch(Dictionary<string, List<string>> pListaFiltros, string pFiltro, string pNombreFaceta, bool pOmitirPalabrasNoRelevantesSearch, bool pQuitarAcentos)
        {
            StringBuilder searchQuery = new StringBuilder(" {?s gnoss:search ?search.");

            string campoBusqueda = pListaFiltros[pFiltro][0].ToLowerSearchGraph();

            if (pQuitarAcentos)
            {
                campoBusqueda = removerSignosAcentos(campoBusqueda);
            }

            if (campoBusqueda.StartsWith("<>"))
            {
                return $" {{?s gnoss:search ?search. FILTER(?search LIKE '{campoBusqueda.Replace("<>", "")}')}}";
            }

            campoBusqueda = campoBusqueda.Replace(((char)13).ToString() + ((char)10).ToString() + ((char)10).ToString() + ((char)13).ToString(), " ").Replace((char)10, ' ').Replace("\\", " ").Replace((char)13, ' ').Replace(".", " ");

            int numeroTagsDespreciadosTitulo = 0;

            //Recorro los tags individuales
            List<string> palabrasFiltro = AnalizadorSintactico.ObtenerTagsFrase(campoBusqueda, out numeroTagsDespreciadosTitulo, " ", pOmitirPalabrasNoRelevantesSearch);

            if (palabrasFiltro.Count == 0)
            {
                //Si limpia todas, mejor que no limpie ninguna
                palabrasFiltro = AnalizadorSintactico.ObtenerTagsFrase(campoBusqueda, out numeroTagsDespreciadosTitulo, " ", false);
            }

            if (UsarRegexParaBusquedaPorTextoLibre)
            {
                string and = "";
                searchQuery.Append(" FILTER (");
                foreach (string palabra in palabrasFiltro)
                {
                    searchQuery.Append(and);
                    searchQuery.Append(ObtenerExpresionRegularParaAutocompletar(palabra, "?search"));
                    and = "AND";
                }
                searchQuery.Append(")");
            }
            else
            {
                searchQuery.Append("?search bif:contains \"");
                string and = "";
                foreach (string palabra in palabrasFiltro)
                {
                    searchQuery.Append($"{and}'{palabra.Replace("'", "''")}'");
                    and = " AND ";
                }
                searchQuery.Append("\" ");

                if (!mConsultaNumeroResultados && string.IsNullOrEmpty(pNombreFaceta))
                {
                    searchQuery.Append("OPTION ( SCORE ?scoreSearch )");
                }
            }
            searchQuery.Append("} ");

            return searchQuery.ToString();
        }

        public string ObtenerFiltroConsultaMapaProyectoDesdeDataSetParaFacetas(DataWrapperFacetas pDataWrapperFacetas, TipoBusqueda pTipoBusqueda, Dictionary<string, List<string>> pListaFiltros)
        {
            return ObtenerFiltroConsultaMapaProyectoDesdeDataSet(pDataWrapperFacetas, pTipoBusqueda, true, true, pListaFiltros, "");
        }

        public string ObtenerFiltroConsultaMapaProyectoDesdeDataSet(DataWrapperFacetas pDataWrapperFacetas, TipoBusqueda pTipoBusqueda, bool pPuntos, bool pRutas)
        {
            return ObtenerFiltroConsultaMapaProyectoDesdeDataSet(pDataWrapperFacetas, pTipoBusqueda, pPuntos, pRutas, null, "");
        }

        private string ObtenerFiltroConsultaMapaProyectoDesdeDataSet(DataWrapperFacetas pDataWrapperFacetas, TipoBusqueda pTipoBusqueda, bool pPuntos, bool pRutas, Dictionary<string, List<string>> pListaFiltros, string pQuery)
        {
            StringBuilder filtro = new StringBuilder();
            if (pDataWrapperFacetas.ListaFacetaConfigProyMapa.Count > 0)
            {
                string[] propsLat = null;
                string[] propsLong = null;
                string[] propsRuta = null;
                string[] colorsRuta = null;

                if (pTipoBusqueda != TipoBusqueda.PersonasYOrganizaciones)
                {
                    string propLat = pDataWrapperFacetas.ListaFacetaConfigProyMapa.FirstOrDefault().PropLatitud;
                    if (!string.IsNullOrEmpty(propLat))
                    {
                        propsLat = propLat.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    string propLong = pDataWrapperFacetas.ListaFacetaConfigProyMapa.FirstOrDefault().PropLongitud;
                    if (!string.IsNullOrEmpty(propLong))
                    {
                        propsLong = propLong.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    string propRuta = pDataWrapperFacetas.ListaFacetaConfigProyMapa.FirstOrDefault().PropRuta;
                    if (!string.IsNullOrEmpty(propRuta))
                    {
                        propsRuta = propRuta.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    string colorRuta = pDataWrapperFacetas.ListaFacetaConfigProyMapa.FirstOrDefault().ColorRuta;
                    if (!string.IsNullOrEmpty(colorRuta))
                    {
                        colorsRuta = colorRuta.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }
                else
                {
                    string propiedades = pDataWrapperFacetas.ListaPropsMapaPerYOrg.FirstOrDefault();
                    if (!string.IsNullOrEmpty(propiedades))
                    {
                        propsLat = pDataWrapperFacetas.ListaPropsMapaPerYOrg.FirstOrDefault().Split(',')[0].Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                        propsLong = pDataWrapperFacetas.ListaPropsMapaPerYOrg.FirstOrDefault().Split(',')[1].Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    //Comprobar si hay datos de rutas.
                    if (propiedades != null && propiedades.Split(',').Length >= 2)
                    {
                        propsRuta = (pDataWrapperFacetas.ListaPropsMapaPerYOrg.FirstOrDefault()).Split(',')[2].Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    //Comprobar si hay datos de rutas.
                    if (propiedades != null && propiedades.Split(',').Length >= 3)
                    {
                        colorsRuta = (pDataWrapperFacetas.ListaPropsMapaPerYOrg.FirstOrDefault()).Split(',')[3].Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                    }
                }

                if (pListaFiltros != null)
                {
                    //filtro para facetas
                    if (pPuntos && propsLat != null && propsLong != null)
                    {
                        filtro = new StringBuilder();
                        StringBuilder filtroRaizLat = new StringBuilder(" { ?s ");
                        string locLat = "";
                        for (int i = 0; i < propsLat.Length - 1; i++)
                        {
                            filtroRaizLat.Append($"{locLat} <{propsLat[i]}> ");
                            locLat = $"?loc{i}";
                            filtroRaizLat.Append($"{locLat}. ");
                        }
                        StringBuilder filtroRaizLong = new StringBuilder("{ ?s ");
                        string locLong = "";
                        for (int i = 0; i < propsLong.Length - 1; i++)
                        {
                            filtroRaizLong.Append($"{locLong} <{propsLong[i]}> ");
                            locLong = $"?loc{i}";
                            filtroRaizLong.Append($"{locLong}. ");
                        }

                        StringBuilder filtroLatConPrefijos = new StringBuilder();
                        for (int i = 0; i < propsLat.Length; i++)
                        {
                            if (i > 0)
                            {
                                filtroLatConPrefijos.Append("@@@");
                            }
                            if (propsLat[i].Contains("#"))
                            {
                                string pref = propsLat[i].Substring(0, propsLat[i].IndexOf("#") + 1);
                                string valor = propsLat[i].Substring(propsLat[i].IndexOf("#") + 1);
                                if (InformacionOntologias.ContainsKey("@" + pref))
                                {
                                    filtroLatConPrefijos.Append($"{InformacionOntologias[$"@{pref}"][0]}:{valor}");
                                }
                            }
                            else
                            {
                                filtroLatConPrefijos.Append(propsLat[i]);
                            }
                        }

                        if (!pListaFiltros.ContainsKey(filtroLatConPrefijos.ToString()))
                        {
                            //Si en los filtros no se filtra por latitud hay que meter el filtro para que solo aparezca lo que tenga esa configuracion
                            filtro.Append($"{filtroRaizLat}{locLat} <{propsLat[propsLat.Length - 1]}> ?lat. }} ");
                            filtro.Append($"{filtroRaizLong}{locLong} <{propsLong[propsLong.Length - 1]}> ?long. }} ");
                        }
                    }
                }
                else
                {

                    if (pPuntos && propsLat != null && propsLong != null)
                    {
                        filtro = new StringBuilder();
                        bool esOptional = false;

                        StringBuilder filtroRaizLat = new StringBuilder();
                        if (pPuntos && !pRutas)
                        {
                            filtroRaizLat.Append(" { ");
                        }
                        else
                        {
                            filtroRaizLat.Append(" OPTIONAL { ");
                            esOptional = true;
                        }

                        string loc = "?s";
                        int numVariablesIguales = 0;
                        int numLats = 0;
                        for (int i = 0; i < propsLat.Length - 1; i++)
                        {
                            string prop = $"<{propsLat[i]}>";
                            string nombreObjeto = $"?loc{numLats++}";
                            if (propsLat[i].Contains("#"))
                            {
                                string pref = propsLat[i].Substring(0, propsLat[i].IndexOf("#") + 1);
                                string valor = propsLat[i].Substring(propsLat[i].IndexOf("#") + 1);
                                if (InformacionOntologias.ContainsKey($"@{pref}"))
                                {
                                    prop = $"{InformacionOntologias[$"@{pref}"][0]}:{valor}";
                                    nombreObjeto = $"?{QuitaPrefijo(prop)}";

                                    if (i == 0 && FacetaAuxEsCoordenada(prop))
                                    {
                                        // Si son coordenadas y comparten el mismo inicio, añadir el texto '_Coord' al final del filtro
                                        nombreObjeto = $"{nombreObjeto}_Coord";
                                    }
                                }
                            }

                            if (!UsarMismsaVariablesParaEntidadesEnFacetas)
                            {
                                while (pQuery.Contains($" ?{QuitaPrefijo(prop)}{numVariablesIguales}. "))
                                {
                                    numVariablesIguales++;
                                }
                            }

                            nombreObjeto += numVariablesIguales;

                            string condicion = $"{loc} {prop} {nombreObjeto}. ";
                            if (esOptional || !pQuery.Contains(condicion))
                            {
                                filtroRaizLat.Append(condicion);
                            }
                            loc = nombreObjeto;
                        }

                        filtro.Append($"{filtroRaizLat}{loc} <{propsLat[propsLat.Length - 1]}> ?lat. }} ");

                        StringBuilder filtroRaizLong = new StringBuilder();
                        if (pPuntos && !pRutas)
                        {
                            filtroRaizLong.Append("{ ");
                        }
                        else
                        {
                            filtroRaizLong.Append(" OPTIONAL { ");
                        }
                        loc = "?s";
                        int numLocs = 0;
                        for (int i = 0; i < propsLong.Length - 1; i++)
                        {
                            string prop = $"<{propsLong[i]}>";
                            string nombreObjeto = $"?loc{numLocs++}";
                            if (propsLong[i].Contains("#"))
                            {
                                string pref = propsLong[i].Substring(0, propsLong[i].IndexOf("#") + 1);
                                string valor = propsLong[i].Substring(propsLong[i].IndexOf("#") + 1);
                                if (InformacionOntologias.ContainsKey($"@{pref}"))
                                {
                                    prop = $"{InformacionOntologias["@" + pref][0]}:{valor}";
                                    nombreObjeto = $"?{QuitaPrefijo(prop)}";

                                    if (i == 0 && FacetaAuxEsCoordenada(prop))
                                    {
                                        // Si son coordenadas y comparten el mismo inicio, añadir el texto '_Coord' al final del filtro
                                        nombreObjeto = $"{nombreObjeto}_Coord";
                                    }
                                }
                            }

                            if (!UsarMismsaVariablesParaEntidadesEnFacetas)
                            {
                                while (pQuery.Contains($" ?{QuitaPrefijo(prop)}{numVariablesIguales}. "))
                                {
                                    numVariablesIguales++;
                                }
                            }

                            nombreObjeto += numVariablesIguales;

                            string condicion = $"{loc} {prop} {nombreObjeto}. ";
                            if (esOptional || (!pQuery.Contains(condicion) && !filtro.ToString().Contains(condicion)))
                            {
                                filtroRaizLong.Append(condicion);
                            }
                            loc = nombreObjeto;
                        }

                        filtro.Append($"{filtroRaizLong}{loc} <{propsLong[propsLong.Length - 1]}> ?long. }} ");

                        if (pPuntos && !pRutas && propsRuta != null)
                        {
                            for (int i = 0; i < propsRuta.Length; i++)
                            {
                                string propiedadRuta = propsRuta[i];
                                filtro.Append($" MINUS {{?s <{propiedadRuta}> ");
                                loc = $"?ruta{i}";
                                filtro.Append($"{loc}. }} ");
                            }
                        }
                    }

                    if (pRutas && propsRuta != null)
                    {
                        if (pRutas && !pPuntos)
                        {
                            filtro.Append(" { ");
                        }
                        else
                        {
                            filtro.Append(" OPTIONAL { ");
                        }

                        string loc = "?s";
                        string propiedadRuta = propsRuta[propsRuta.Length - 1];
                        for (int i = 0; i < propsRuta.Length - 1; i++)
                        {
                            propiedadRuta = propsRuta[i];

                            filtro.Append($"{loc} <{propiedadRuta}> ");
                            loc = $"?loc{i}";
                            filtro.Append($"{loc}. ");
                        }

                        filtro.Append($"{loc} <{propiedadRuta}> ?ruta. ");

                        filtro.Append(" } ");

                        if (colorsRuta != null)
                        {
                            filtro.Append(" OPTIONAL { ");

                            loc = "?s";
                            string propiedadColor = colorsRuta[colorsRuta.Length - 1];
                            for (int i = 0; i < colorsRuta.Length - 1; i++)
                            {
                                propiedadColor = colorsRuta[i];

                                filtro.Append($"{loc} <{propiedadColor}> ");
                                loc = $"?loc{i}";
                                filtro.Append($"{loc}. ");
                            }

                            filtro.Append($"{loc} <{propiedadColor}> ?color. ");

                            filtro.Append(" } ");
                        }
                    }
                }
            }

            return filtro.ToString();
        }

        /// <summary>
        /// Obtiene el verdadero valor de un elemento subType almacenado en virtuoso.
        /// </summary>
        /// <param name="pValor">Valor del subType</param>
        /// <returns>Verdadero valor de un elemento subType almacenado en virtuoso</returns>
        public string ObtenerRealSubType(string pValor)
        {
            if (pValor.Contains(":"))
            {
                string[] namesUrl = pValor.Split(':');

                if (InformacionOntologias.Any(onto => onto.Value.Contains(namesUrl[0])))
                {
                    var ontologia = InformacionOntologias.First(onto => onto.Value.Contains(namesUrl[0]));

                    pValor = ontologia.Key + namesUrl[1];

                    if (pValor.StartsWith('@'))
                    {
                        pValor = pValor.Substring(1);
                    }
                }
            }

            return pValor;
        }

        /// <summary>
        /// Devuelve el peso para el proyectoID seleccionado
        /// </summary>
        /// <param name="pProyectoID">ProyectoID del que quiero obtener el peso</param>
        /// <returns>Peso del proyectoID pasado.</returns>
        public int ObtenerPopularidadProyecto(Guid pProyectoID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $" SPARQL SELECT ?peso FROM <{mUrlIntranet}{ProyectoAD.MetaProyecto}> where {{<http://gnoss/{pProyectoID.ToString().ToUpper()}> <http://gnoss/hasPopularidad> ?peso.}} ";

            LeerDeVirtuoso(query, "aaa", facetadoDS, pProyectoID.ToString());

            int pesoVirtuoso = 0;

            if (facetadoDS.Tables["aaa"].Rows.Count > 0)
            {
                int.TryParse(facetadoDS.Tables["aaa"].Rows[0][0].ToString(), out pesoVirtuoso);
            }

            return pesoVirtuoso;
        }

        public int ObtenerRankingDocumento(Guid pDocumentoID, Guid pProyID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = " SPARQL SELECT ?rank FROM <" + mUrlIntranet + pProyID + "> WHERE{<http://gnoss/" + pDocumentoID + "> <http://gnoss/hasPopularidad> ?rank. } ";

            LeerDeVirtuoso(query, "aaa", facetadoDS, pProyID.ToString());

            int rankVirtuoso = -1;

            if (facetadoDS.Tables["aaa"].Rows.Count > 0)
            {
                int.TryParse(facetadoDS.Tables["aaa"].Rows[0][0].ToString(), out rankVirtuoso);
            }

            return rankVirtuoso;
        }

        public FacetadoDS ObtenerIdentidadesEnProyectoID(Guid pProyectoID, List<string> pRdfTypeList)
        {
            FacetadoDS ds = new FacetadoDS();
            StringBuilder query = new StringBuilder($" SPARQL SELECT distinct ?s FROM <{mUrlIntranet}{pProyectoID}> where {{?s rdf:type ?rdftype. FILTER(?rdftype in (");

            foreach (string rdfTpe in pRdfTypeList)
            {
                query.Append($"'{rdfTpe}', ");
            }

            if (query.ToString().EndsWith(", "))
            {
                query = query.Remove(query.Length - 2, 2);
            }

            query.Append(")) ?s ?p ?o. FILTER(?p LIKE '%privacidad%')} ");

            LeerDeVirtuoso(query.ToString(), "aaa", ds, pProyectoID.ToString());

            return ds;
        }

        public FacetadoDS ObtenerEntidadesEnProyectoIDDocumentoID(Guid pProyectoID, Guid pDocumentoID)
        {
            List<string> listaConvertidaIntranet = new List<string>();
            StringBuilder listaStringConvertidaIntranet = new StringBuilder();
            string result = "";
            FacetadoDS ds = new FacetadoDS();
            StringBuilder query = new StringBuilder();
            query.Append("select ?s ?o FROM <");
            query.Append(mUrlIntranet);
            query.Append(pProyectoID.ToString().ToLower());
            query.Append("> WHERE");
            query.Append("{");
            query.Append(" ?s <http://gnossEdu/resource> ?resource");
            query.Append($" FILTER(?resource = <http://gnoss/{pDocumentoID.ToString().ToUpper()}>)");
            query.Append("}");
            LeerDeVirtuoso(query.ToString(), "entidades", ds, pProyectoID.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow entidad in ds.Tables[0].Rows)
                {
                    result = entidad[0].ToString().Replace("http://gnoss/", mUrlIntranet);
                    listaConvertidaIntranet.Add(result);
                }

            }
            foreach (string items in listaConvertidaIntranet)
            {
                listaStringConvertidaIntranet.Append($"<{items}>,");
            }
            if (listaStringConvertidaIntranet.Length > 0)
            {
                listaStringConvertidaIntranet = listaStringConvertidaIntranet.Remove(listaStringConvertidaIntranet.Length - 1, 1);
            }
            query = new StringBuilder();
            query.Append("select ?s ?o FROM <");
            query.Append(mUrlIntranet);
            query.Append("dbpedia.owl");
            query.Append(">");
            query.Append("{");
            query.Append(" ?doc <http://gnoss/hasEntidad> ?s.");
            query.Append(" ?s <http://www.w3.org/2002/07/owl#sameAs> ?o.");
            query.Append($" FILTER(?doc in ({listaStringConvertidaIntranet.ToString().ToLower()}))");
            query.Append("}");
            ds = new FacetadoDS();
            LeerDeVirtuoso(query.ToString(), "entidades", ds, pProyectoID.ToString());
            return ds;
        }

        public FacetadoDS ObtenerTriplesOntologiaACompartir(Guid pProyectoID, string pEnlace)
        {
            FacetadoDS ds = new FacetadoDS();
            bool continuar = true;
            int i = 0;
            while (continuar)
            {
                FacetadoDS dsAux = new FacetadoDS();
                string consulta = "SPARQL select * where { ";
                string consultaOffset = $" select ?s ?p ?o lang(?o) as ?idioma from <{mUrlIntranet}{pProyectoID.ToString().ToLower()}> where {{?s2 ?p2 ?s. ?s ?p ?o. FILTER (?s2=<{mUrlIntranet}{pEnlace.ToLower()}>)}} UNION {{?s ?p ?o. FILTER (?s=<{mUrlIntranet}{pEnlace.ToLower()}>)}} UNION {{?s3 ?p3 ?s2. ?s2 ?p2 ?s. ?s ?p ?o. FILTER (?s3=<{mUrlIntranet}{pEnlace.ToLower()}>)}}";
                consulta += $"{consultaOffset} order by ?s ?p ?o ?idioma }} offset {i * 10000} + limit 10000";

                LeerDeVirtuoso(consulta, "CompartirOntologia", dsAux, pProyectoID.ToString());

                if (dsAux != null && dsAux.Tables != null && dsAux.Tables.Contains("CompartirOntologia") && dsAux.Tables["CompartirOntologia"].Rows.Count > 0)
                {
                    i++;
                    ds.Merge(dsAux);
                    if (dsAux.Tables["CompartirOntologia"].Rows.Count < 10000)
                    {
                        continuar = false;
                    }
                    dsAux.Dispose();
                }
                else
                {
                    continuar = false;
                }
            }

            return ds;
        }

        public bool ComprobarSiComentarioNoExisteOHaSidoLeidoPerfil(string pComentarioId, string pUsuarioId)
        {
            FacetadoDS ds = new FacetadoDS();
            string sql = $"SPARQL SELECT ?s ?p ?o FROM <http://gnoss.com/{pUsuarioId.ToLower()}> WHERE {{?s ?p ?o. FILTER(?s = <http://gnoss/{pComentarioId.ToUpper()}> AND ?p = <http://gnoss/Leido>)}}";
            LeerDeVirtuoso(sql, "TriplesComentarioPerfil", ds, pUsuarioId);

            //No existe el comentario
            bool noExisteOHaSidoLeidoPerfil = ds.Tables["TriplesComentarioPerfil"].Rows.Count == 0;

            //Existe y está leído
            noExisteOHaSidoLeidoPerfil = noExisteOHaSidoLeidoPerfil || (ds.Tables["TriplesComentarioPerfil"].Rows.Count > 0 && ds.Tables["TriplesComentarioPerfil"].Rows[0][2].Equals("Leidos"));

            return noExisteOHaSidoLeidoPerfil;
        }

        /// <summary>
        /// Obtiene todas las triples de un tesauro semántico.
        /// </summary>
        /// <param name="pGrafo">Grafo del tesaro</param>
        /// <param name="pSource">Source del tesauro</param>
        /// <returns>Triples de un tesauro semántico</returns>
        public FacetadoDS ObtenerTesauroSemantico(string pGrafo, string pSource)
        {
            FacetadoDS facDS = new FacetadoDS();

            int limit = 10000;
            string sql = $"SELECT ?s ?p ?o ?idioma FROM {ObtenerUrlGrafo(pGrafo).ToLower()} {{ SELECT ?s ?p ?o lang(?o) as ?idioma FROM {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{?s ?p ?o. ?s <http://purl.org/dc/elements/1.1/source> ?o2 FILTER(?o2 = '{pSource}')}} order by ?s ?o ?p }}";

            int repeticion = 0;
            int numFilas = 0;

            while (numFilas >= limit * repeticion)
            {
                string offsetSQL = "";
                if (repeticion > 0)
                {
                    offsetSQL = $" offset {limit * repeticion}";
                }
                string limitSQL = $" limit {limit}";

                FacetadoDS facDSaux = new FacetadoDS();
                LeerDeVirtuoso(sql + offsetSQL + limitSQL, "TriplesTesSem", facDSaux, pGrafo);
                facDS.Merge(facDSaux);
                numFilas = facDS.Tables[0].Rows.Count;
                repeticion++;
            }

            return facDS;
        }



        #endregion

        #region Actualizaciones


        /// <summary>
        /// Modifica un mensaje, comentario de Pendiente de leer a leido
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadID">Identificadro de la identidad</param>
        public void ModificarPendienteLeerALeido(string pGrafoID, string pElementoID)
        {
            string query = NamespacesVrituosoEscritura;

            query += $"MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{?s ?p ?o. }} INSERT {{ ?s ?p \"Leidos\".}}  where {{?s ?p ?o. FILTER (?s=<http://gnoss/{pElementoID.ToUpper()}> and ?o=\"Pendientes de leer\")}}";

            ActualizarVirtuoso(query, pGrafoID);
        }

        /// <summary>
        /// Modifica un mensaje como eliminado
        /// </summary>
        /// <param name="pGrafoID"></param>
        /// <param name="pElementoID"></param>
        public void ModificarMensajeAEliminado(string pGrafoID, string pElementoID)
        {
            string query = NamespacesVrituosoEscritura;

            query += $"MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{?s ?p ?o. }} INSERT {{ ?s ?p \"Eliminados\".}}  where {{?s ?p ?o. FILTER (?s=<http://gnoss/{pElementoID.ToUpper()}> AND ?p = <http://purl.org/dc/elements/1.1/type>)}}";

            ActualizarVirtuoso(query, pGrafoID);
        }

        /// <summary>
        /// Modifica un mensaje como eliminado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pIdentidadID">Identificadro de la identidad</param>
        public void ModificarSuscripcionCaducidad(string pGrafoID, string pElementoID)
        {
            string query = NamespacesVrituosoEscritura;

            query += $"MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{?s ?p ?o. }} INSERT {{ ?s ?p \"Favoritos\". }} where {{?s ?p ?o. FILTER (?s=<http://gnoss/{pElementoID.ToUpper()}> AND ?p = <http://gnoss/Estado>)}}";

            ActualizarVirtuoso(query, pGrafoID);
        }

        /// <summary>
        /// Inserta una tripleta que dice que un usuario ignora a otro.
        /// </summary>
        /// <param name="pIdentidadIgnorante">Identidad que ignora</param>
        /// <param name="pIdentidadIgnorado">Identidad que es ignorada</param>
        public void InsertUsuarioIgnoraContacto(Guid pIdentidadIgnorante, Guid pIdentidadIgnorado)
        {
            InsertaTripleta("contactos", $"<http://gnoss/{pIdentidadIgnorante.ToString().ToUpper()}>", "<http://gnoss/Ignora>", $"<http://gnoss/{pIdentidadIgnorado.ToString().ToUpper()}>", 1, true);
        }



        /// <summary>
        /// Modifica el estado de una pregunta o debate
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pElementoaModificarID">Identificador del elemento a modificar</param>
        /// <param name="pNuevoEstado">Nuevo estado del debate</param>
        public void ModificarEstadoPreguntaDebate(string pProyectoID, string pElementoaModificarID, string pNuevoEstado)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;
            query += $" select ?o {ObtenerFrom(pProyectoID)} where {{?s gnoss:hasestado ?o. FILTER(?s=gnoss:{pElementoaModificarID.ToUpper()})}} ";

            LeerDeVirtuoso(query, "NumeroActual", facetadoDS, pProyectoID);

            if (facetadoDS.Tables["NumeroActual"].Rows.Count > 0)
            {
                string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s gnoss:hasestado ?o }} INSERT {{ ?s gnoss:hasestado '{pNuevoEstado}'}} FROM {ObtenerUrlGrafo(pProyectoID)} WHERE {{?s gnoss:hasestado ?o. FILTER(?s=gnoss:{pElementoaModificarID.ToUpper()})}}";

                ActualizarVirtuoso(modify, pProyectoID);
            }
            else
            {
                InsertaTripleta(pProyectoID, $"<http://gnoss/{pElementoaModificarID.ToUpper()}>", "<http://gnoss/hasestado>", $"'{pNuevoEstado}'", 1, true);
            }
        }

        /// <summary>
        /// Modifica la fila de virtuoso de un recurso.
        /// </summary>
        /// <param name="pProyectoID">Id del grafo</param>
        /// <param name="pElementoAModificarID">Elemento a modificar</param>
        /// <param name="pTipoElementoAModificar">Tipo de elemento</param>
        public void ModificarVotosVisitasComentarios(string pProyectoID, string pElementoAModificarID, string pTipoElementoAModificar, long pNumVisitas)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} select ?o {ObtenerFrom(pProyectoID)} where {{?s ?p ?o. FILTER (?s=gnoss:{pElementoAModificarID.ToUpper()} and ?p=gnoss:hasnumero{pTipoElementoAModificar})}}";

            LeerDeVirtuoso(query, "NumeroActual", facetadoDS, pProyectoID);

            if (facetadoDS.Tables["NumeroActual"].Rows.Count > 0)
            {
                string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{ ?s ?p ?o_new }} FROM {ObtenerUrlGrafo(pProyectoID)} WHERE {{ SELECT (?o + {pNumVisitas}) AS ?o_new  ?s ?p ?o from {ObtenerUrlGrafo(pProyectoID)} WHERE {{ ?s ?p ?o FILTER (?p=<http://gnoss/hasnumero{pTipoElementoAModificar}> and ?s=gnoss:{pElementoAModificarID.ToUpper()})}}}}";

                ActualizarVirtuoso(modify, pProyectoID);
            }
            else
            {
                long numeronuevo = pNumVisitas;

                InsertaTripleta(pProyectoID, $"<http://gnoss/{pElementoAModificarID.ToUpper()}>", $"<http://gnoss/hasnumero{pTipoElementoAModificar}>", numeronuevo.ToString(), 100, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID">Id del grafo</param>
        /// <param name="pElementosAModificarID">Elementos a modificar</param>
        public void ModificarCertificacionesRecursos(Guid pProyectoID, Dictionary<int, List<Guid>> pElementosAModificar)
        {
            foreach (int nivelCert in pElementosAModificar.Keys)
            {
                if (pElementosAModificar[nivelCert].Count > 0)
                {
                    List<string> DocumentosIn = new List<string>();

                    int i = 0;
                    string coma = "";
                    StringBuilder listaActual = new StringBuilder("(");
                    foreach (Guid idRecurso in pElementosAModificar[nivelCert])
                    {
                        listaActual.Append($"{coma}gnoss:{idRecurso.ToString().ToUpper()}");
                        if (coma == "")
                        {
                            coma = ",";
                        }
                        i++;

                        if (i == 500 || i == pElementosAModificar[nivelCert].Count % 500)
                        {
                            listaActual.Append(")");
                            DocumentosIn.Add(listaActual.ToString());

                            i = 0;
                            coma = "";
                            listaActual = new StringBuilder("(");
                        }
                    }

                    foreach (string listaDocs in DocumentosIn)
                    {
                        string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID.ToString())} DELETE {{ ?s ?p ?o }} INSERT {{ ?s ?p ?o_new }} FROM {ObtenerUrlGrafo(pProyectoID.ToString())} WHERE {{{{ SELECT ({nivelCert}) AS ?o_new ?s ?p ?o from {ObtenerUrlGrafo(pProyectoID.ToString())} WHERE {{ ?s ?p ?o FILTER (?p=<http://gnoss/hasnivelcertification> and ?s IN {listaDocs}) }}}}}}";

                        ActualizarVirtuoso(modify, pProyectoID.ToString(), 1);
                    }
                }
            }
        }

        /// <summary>
        /// Modifica la popularidad 
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pNombreNuevo">Nombre nuevo de la categoria</param>
        /// <param name="pCategoriaID">Identificador de la categoria</param>
        /// <param name="UrlIntragnoss"></param>
        public void ModificarNombreCategoria(string pProyectoID, string pNombreNuevo, string pCategoriaID, bool pCategoriaDeProyecto)
        {
            string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{ <http://gnoss/{pCategoriaID.ToUpper()}> <http://gnoss/CategoryName>\"{pNombreNuevo.ToLower()}\" }} FROM {ObtenerUrlGrafo(pProyectoID)} WHERE {{?s ?p ?o. FILTER(?s=gnoss:{pCategoriaID.ToUpper()} and ?p=<http://gnoss/CategoryName>) }}";

            ActualizarVirtuoso(modify, pProyectoID);
            IdentidadAD idenAD = new IdentidadAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication, mloggerFactory.CreateLogger<IdentidadAD>(), mloggerFactory);
            if (pCategoriaDeProyecto)
            {
                List<Guid> listaOrganizaciones = idenAD.ObtenerOrganizacionesDeContribuidoresEnProyecto(new Guid(pProyectoID));
                foreach (Guid organizacionID in listaOrganizaciones)
                {
                    modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(organizacionID.ToString())} DELETE {{ ?s ?p ?o }} INSERT {{ <http://gnoss/{pCategoriaID.ToUpper()}> <http://gnoss/CategoryName>\"{pNombreNuevo.ToLower()}\" }} FROM {ObtenerUrlGrafo(organizacionID.ToString())} WHERE {{ ?s ?p ?o. FILTER(?s=gnoss:{pCategoriaID.ToUpper()} and ?p = <http://gnoss/CategoryName> ) }}";

                    ActualizarVirtuoso(modify, pProyectoID);
                }

                List<Guid> listaPerfiles = idenAD.ObtenerPerfilesDeContribuidoresEnProyecto(new Guid(pProyectoID));
                foreach (Guid perfilID in listaPerfiles)
                {
                    modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(perfilID.ToString())} DELETE {{ ?s ?p ?o }} INSERT {{ <http://gnoss/{pCategoriaID.ToUpper()}> <http://gnoss/CategoryName>\"{pNombreNuevo.ToLower()}\" }} FROM {ObtenerUrlGrafo(perfilID.ToString())} WHERE {{?s ?p ?o. FILTER(?s=gnoss:{pCategoriaID.ToUpper()} and ?p = <http://gnoss/CategoryName>)}}";

                    ActualizarVirtuoso(modify, pProyectoID);
                }
            }
        }

        /// <summary>
        /// Modifica la popularidad de la identidad indicada
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad a modificar</param>
        public void ModificarPopularidadIdentidad(string pIdentidadID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            //Actualizo popularidad en virtuoso
            string sqlObtenerProyIDRank = $"select Identidad.ProyectoID, Identidad.Rank from Identidad where Identidad.IdentidadID='{pIdentidadID}'";
            DbCommand commandSqlObtenerProyIDRank = ObtenerComando(sqlObtenerProyIDRank);
            CargarDataSet(commandSqlObtenerProyIDRank, facetadoDS, "Identidad");

            if (facetadoDS.Tables["Identidad"].Rows.Count > 0)
            {
                string proyectoID = facetadoDS.Tables["Identidad"].Rows[0][0].ToString();
                string popularidadNueva = facetadoDS.Tables["Identidad"].Rows[0][1].ToString();

                string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(proyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{ <http://gnoss/{pIdentidadID.ToUpper()}> <http://gnoss/hasPopularidad> {popularidadNueva.Replace(",", ".")} }} FROM {ObtenerUrlGrafo(proyectoID)} WHERE {{?s ?p ?o. FILTER(?s=gnoss:{pIdentidadID.ToUpper()} and ?p=gnoss:hasPopularidad)}}";

                ActualizarVirtuoso(modify, proyectoID.ToString(), 1);
            }
        }

        /// <param name="pProyectoID">Comunidad de la cual vamos a modificar el estado</param>
        /// <param name="pNuevoEstado">Posibles valores: Cerrado, CerradoTemporalmente, Definicion, Abierto, Cerrandose</param>
        public void ModificarEstadoComunidadGrafoContactos(string pProyectoID, string pNuevoEstado)
        {
            string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo("contactos")} DELETE {{ ?s ?p ?o }} INSERT {{ <http://gnoss/{pProyectoID.ToUpper()}> <http://gnoss/estadoProyecto> \"{pNuevoEstado}\" }} FROM {ObtenerUrlGrafo("contactos")} WHERE {{?s ?p ?o. FILTER(?s=gnoss:{pProyectoID.ToUpper()} and ?p=gnoss:estadoProyecto) }}";

            ActualizarVirtuoso(modify, "contactos", 1);
        }

        /// <summary>
        /// Modifica los votos de un recurso
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pElementoAModificarID">Identificador del elemento a modificar</param>
        /// <param name="pTipoElementoAModificar">Tipo del elemento amodificar (votos, comentarios o visitas)</param>
        public void ModificarVotosNegativo(string pProyectoID, string pElementoAModificarID, string pTipoElementoAModificar)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} select ?o {ObtenerFrom(pProyectoID)} where {{?s ?p ?o. FILTER (?s=gnoss:{pElementoAModificarID.ToUpper()} and ?p=gnoss:hasnumero{pTipoElementoAModificar}) }}";
            LeerDeVirtuoso(query, "NumeroActual", facetadoDS, pProyectoID);

            if (facetadoDS.Tables["NumeroActual"].Rows.Count > 0)
            {
                string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{ ?s ?p ?o_new }} FROM {ObtenerUrlGrafo(pProyectoID)} WHERE {{{{ SELECT (?o - 1) AS ?o_new ?s ?p ?o from {ObtenerUrlGrafo(pProyectoID)} WHERE {{ ?s ?p ?o FILTER (?p=<http://gnoss/hasnumero{pTipoElementoAModificar}> and ?s=gnoss:{pElementoAModificarID.ToUpper()}) }}}}}}";

                ActualizarVirtuoso(modify, pProyectoID);
            }
            else
            {
                InsertaTripleta(pProyectoID, $"<http://gnoss/{pElementoAModificarID.ToUpper()}>", $"<http://gnoss/hasnumero{pTipoElementoAModificar}>", "-1", 1, true);
            }
        }

        /// <summary>
        /// Modifica la comunidad de en Definición a Abierta
        /// </summary>
        /// <param name="ProyectoID">Identificador de la comunidad</param>
        public void ModificarEstadoComunidadCerrar(string pProyectoID)
        {
            string from = ObtenerUrlGrafo(pProyectoID);
            string query = $"sparql CLEAR GRAPH {from}";

            ActualizarVirtuoso(query, pProyectoID);

            query = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph("11111111111111111111111111111111").ToLower()} {{ ?s ?p ?o }} {ObtenerFrom("11111111111111111111111111111111").ToLower()} WHERE {{?s ?p ?o. ?s <http://rdfs.org/sioc/ns#has_space> ?space.  FILTER(?space=gnoss:{pProyectoID.ToUpper()}) ?s rdf:type ?rdftype. FILTER (?rdftype in ('Recurso', 'Recurso Personal',  'Dafo', 'Pregunta', 'Debate', 'Encuesta')). }}";

            ActualizarVirtuoso(query, pProyectoID);
        }

        /// <summary>
        /// Borra recomendaciones de personas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoItem">Tipo de item del cual se desea borrar la popularidad: recurso, identidad, blog, etc.</param>
        public void BorrarRecomendacionesDePersonas(string pIdentidad)
        {
            string query = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph("contactos").ToLower()} {{ ?s ?p ?o }} {ObtenerFrom("contactos").ToLower()} WHERE {{ ?s ?p ?o. FILTER(?p=<http://gnoss/RecPer> and ?s=gnoss:{pIdentidad.ToUpper()})}}";
            ActualizarVirtuoso(query, "contactos", 1);

            query += $"DELETE {ObtenerFromGraph("contactos").ToLower()} {{ ?o ?p2 ?o2 }} {ObtenerFrom("contactos").ToLower()} WHERE {{ ?s ?p ?o. ?o ?p2 ?o2 FILTER(?p2 in(<http://gnoss/RecID>, <http://gnoss/RecType>, <http://gnoss/RecNum>) and ?s=gnoss:{pIdentidad.ToUpper()}) }}";
            ActualizarVirtuoso(query, "contactos", 1);
        }

        /// <summary>
        /// Borra la popularidad de un recurso, persona o comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTipoItem">Tipo de item del cual se desea borrar la popularidad: recurso, identidad, blog, etc.</param>
        public void BorrarPopularidad(string pProyectoID, List<string> pTipoItem)
        {
            StringBuilder query = new StringBuilder($"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pProyectoID).ToLower()} {{ ?s ?p ?o }} {ObtenerFrom(pProyectoID).ToLower()} WHERE {{ ?s ?p ?o. ?s rdf:type ?rdftype FILTER(?rdftype in (");

            foreach (string valor in pTipoItem)
            {
                query.Append($"'{valor}', ");
            }

            query.Append($"{query.Remove(query.Length - 2, 2)})). FILTER(?p=gnoss:hasPopularidad) }}");

            ActualizarVirtuoso(query.ToString(), pProyectoID, 1);
        }

        /// <summary>
        /// Inserta la popularidad de un recurso, persona o comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="ptriplesInsertar">Tripletas Nuevas</param>
        public void InsertarPopularidad(Guid pProyectoID, string ptriplesInsertar)
        {
            InsertarPopularidad(pProyectoID, ptriplesInsertar, null);
        }

        /// <summary>
        /// Inserta la popularidad de un recurso, persona o comunidad
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="ptriplesInsertar">Tripletas Nuevas</param>
        /// <param name="pProyectoID_2">Proyecto ID donde se va a insertar la condición</param>
        public void InsertarPopularidad(Guid pProyectoID, string ptriplesInsertar, Guid? pProyectoID_2)
        {
            StringBuilder condicion = new StringBuilder($"?s=<http://gnoss/{pProyectoID_2.ToString().ToUpper()}> and ?p=<http://gnoss/hasPopularidad>");
            if (pProyectoID_2.HasValue)
            {
                InsertaTripletasConModify(pProyectoID.ToString(), ptriplesInsertar, new Dictionary<string, string>(), "", condicion.ToString());
            }
            else
            {
                string[] triples = ptriplesInsertar.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                condicion.Append(" ?p=<http://gnoss/hasPopularidad> and ?s in( ");

                string coma = "";
                foreach (string triple in triples)
                {
                    string sujeto = triple.Trim();
                    if (!string.IsNullOrEmpty(sujeto))
                    {
                        sujeto = sujeto.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                        condicion.Append(coma + sujeto);
                    }
                    coma = ",";
                }

                condicion.Append(" )");

                InsertaTripletasConModify(pProyectoID.ToString(), ptriplesInsertar, new Dictionary<string, string>(), "", condicion.ToString());
            }
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo contacto
        /// </summary>
        /// <param name="pIdentidad1">pIdentidad1</param>
        /// <param name="pIdentidad2">pIdentidad2</param>
        public void InsertarNuevoContacto(string pIdentidad1, string pIdentidad2)
        {
            string tripletas = GenerarTripleta($"<http://gnoss/{pIdentidad1.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{pIdentidad2.ToUpper()}>");
            tripletas += GenerarTripleta($"<http://gnoss/{pIdentidad2.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{pIdentidad1.ToUpper()}>");

            InsertaTripletas("contactos", tripletas, 0, true);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina contacto
        /// </summary>
        /// <param name="pIdentidad1">Identidad1</param>
        /// <param name="pIdentidad2">Identidad2</param>
        public void BorrarContacto(string pIdentidad1, string pIdentidad2)
        {
            BorrarTripleta("contactos", $"<http://gnoss/{pIdentidad1.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{pIdentidad2.ToUpper()}>", true);

            BorrarTripleta("contactos", $"<http://gnoss/{pIdentidad2.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{pIdentidad1.ToUpper()}>", true);
        }

        public void BorrarGrupoContactos(string identidad, string grupoID, string nombreGrupo)
        {
            BorrarTripleta("contactos", $"<http://gnoss/{grupoID.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{identidad.ToUpper()}>", true);

            BorrarTripleta("contactos", $"<http://gnoss/{grupoID.ToUpper()}>", "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>", "\"ContactoGrupo\"", true);

            BorrarTripleta("contactos", $"<http://gnoss/{grupoID.ToUpper()}>", "<http://xmlns.com/foaf/0.1/firstName>", $"\"{nombreGrupo}\"", true);
        }


        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo seguidor
        /// </summary>
        /// <param name="perfil1">Perfil1 Perfil que sigue</param>
        /// <param name="perfil1">Perfil2 Perfil Seguido</param>
        public void InsertarNuevoSeguidor(string perfil1, string perfil2, string proyecto)
        {
            string tripletas = GenerarTripleta($"<http://gnoss/{perfil1.ToUpper()}{proyecto.ToUpper()}>", "<http://gnoss/sigue>", $"<http://gnoss/{perfil2.ToUpper()}{proyecto.ToUpper()}>");

            InsertaTripletas("seguidores", tripletas, 0, true);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina contacto
        /// </summary>
        /// <param name="perfil1">Perfil1 Perfil que sigue</param>
        /// <param name="perfil1">Perfil2 Perfil Seguido</param>
        public void BorrarSeguidor(string perfil1, string perfil2, string proyecto)
        {
            BorrarTripleta("seguidores", $"<http://gnoss/{perfil1.ToUpper()}{proyecto.ToUpper()}>", "<http://gnoss/sigue>", $"<http://gnoss/{perfil2.ToUpper()}{proyecto.ToUpper()}>", true);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo seguidor
        /// </summary>
        /// <param name="pIdentidad1">Identidad1</param>
        /// <param name="pIdentidad2">Identidad2</param>
        public void InsertarNuevoSeguidorProyecto(string pIdentidad1, string pIdentidad2, string proyecto)
        {
            string tripletas = GenerarTripleta($"<http://gnoss/{pIdentidad1.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{pIdentidad2.ToUpper()}>");

            InsertaTripletas(proyecto.ToLower(), tripletas, 0, true);
        }

        /// <summary>
        /// Inserta  en virtuoso las tripletas necesarias para un nuevo seguidor
        /// </summary>
        /// <param name="pIdentidad1">Identidad1</param>
        /// <param name="pIdentidad2">Identidad2</param>
        public void BorrarSeguidorProyecto(string pIdentidad1, string pIdentidad2, string proyecto)
        {
            BorrarTripleta(proyecto.ToLower(), $"<http://gnoss/{pIdentidad1.ToUpper()}>", "<http://xmlns.com/foaf/0.1/knows>", $"<http://gnoss/{pIdentidad2.ToUpper()}>", true);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina un grupo
        /// </summary>
        /// <param name="grupoID">ID del grupo</param>
        /// <param name="proyectoID">ID del proyecto</param>
        public void BorrarGrupo(Guid grupoID, string nombreGrupo, Guid proyectoID)
        {
            //Borramos las filas de que el grupo es participante del recurso de virtuoso
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:hasparticipanteIdentidadID", $"gnoss:{grupoID.ToString().ToUpper()}", true);
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:hasparticipante", $"\"{nombreGrupo.ToLower()}\"", true);
            //Borramos las filas de que el grupo es editor del recurso de virtuoso
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:haseditorIdentidadID", $"gnoss:{grupoID.ToString().ToUpper()}", true);
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:haseditor", $"\"{nombreGrupo.ToLower()}\"", true);
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:hasgrupoEditor", $"\"{nombreGrupo.ToLower()}\"", true);
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:hasgrupoLector", $"\"{nombreGrupo.ToLower()}\"", true);
            BorrarTripleta(proyectoID.ToString().ToLower(), null, "gnoss:haseditor", $"\"{nombreGrupo.ToLower()}\"", true);
            //Borramos el grupo de virtuoso
            BorrarTripleta(proyectoID.ToString().ToLower(), $"gnoss:{grupoID.ToString().ToUpper()}", null, null, true);
        }

        /// <summary>
        ///Borra en virtuoso las tripletas necesarias cuando se elimina un grupo
        /// </summary>
        /// <param name="grupoID">ID del grupo</param>
        /// <param name="identidadID">Identiddad del participante</param>
        /// <param name="nombre">Nombre</param>
        /// <param name="proyectoID">ID del proyecto</param>
        public void BorrarParticipanteDeGrupo(Guid grupoID, Guid identidadID, string nombreIdentidad, Guid proyectoID)
        {
            BorrarTripleta(proyectoID.ToString().ToLower(), $"gnoss:{grupoID.ToString().ToUpper()}", "gnoss:hasparticipanteID", $"gnoss:{identidadID.ToString().ToUpper()}", true);
            BorrarTripleta(proyectoID.ToString().ToLower(), $"gnoss:{grupoID.ToString().ToUpper()}", "gnoss:hasparticipante", $"\"{nombreIdentidad.ToLower()}\"", true);
        }

        /// <summary>
        /// Borra un recurso en virtuoso
        /// </summary>
        /// <param name="pGrafoID">Identificador del proyecto o del perfil</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarRecurso(string pGrafoID, Guid pElementoaEliminarID)
        {
            BorrarRecurso(pGrafoID, pElementoaEliminarID, 0);
        }

        /// <summary>
        /// Borra un recurso en virtuoso
        /// </summary>
        /// <param name="pGrafoID">Identificador del proyecto o del perfil</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        /// <param name="pPrioridad">Prioridad para borrar el recurso.</param>
        public void BorrarRecurso(string pGrafoID, Guid pElementoaEliminarID, int pPrioridad)
        {
            BorrarRecurso(pGrafoID, pElementoaEliminarID, pPrioridad, "", true);
        }

        /// <summary>
        /// Borra un recurso en virtuoso
        /// </summary>
        /// <param name="pGrafoID">Identificador del proyecto o del perfil</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarRecurso(string pGrafoID, Guid pElementoaEliminarID, int pPrioridad, string pInfoExtra, bool pUsarColaActualizacion)
        {
            BorrarRecurso(pGrafoID, pElementoaEliminarID, pPrioridad, pInfoExtra, pUsarColaActualizacion, false);
        }

        /// <summary>
        /// Borra un recurso en virtuoso
        /// </summary>
        /// <param name="pGrafoID">Identificador del proyecto o del perfil</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarRecurso(string pGrafoID, Guid pElementoaEliminarID, int pPrioridad, string pInfoExtra, bool pUsarColaActualizacion, bool pBorrarAuxiliar, bool pBorrarMyGnoss = false)
        {
            string query = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pGrafoID)} {{ ?s ?p ?o }} {ObtenerFrom(pGrafoID)} WHERE {{?s ?p ?o. FILTER(?s=gnoss:{pElementoaEliminarID.ToString().ToUpper()} ) }}";

            ActualizarVirtuoso(query, pGrafoID);

            if (pBorrarMyGnoss)
            {
                //borrar de mygnoss
                query = $"{NamespacesVrituosoEscritura} DELETE from  {ObtenerUrlGrafo("11111111-1111-1111-1111-111111111111")} {{ ?s ?p ?o }} from {ObtenerUrlGrafo("11111111-1111-1111-1111-111111111111")} WHERE {{?s ?p ?o. FILTER(?s=gnoss:{pElementoaEliminarID.ToString().ToUpper()})}} ";

                ActualizarVirtuoso(query, pGrafoID);
            }

            if (pBorrarAuxiliar)
            {
                string sujetoExtraBorrar = $"<http://gnossAuxiliar/{pElementoaEliminarID.ToString().ToUpper()}>";

                query = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pGrafoID)} {{ ?s ?p ?o. ?sujAux ?predAux ?s. }} {ObtenerFrom(pGrafoID)} WHERE {{?sujAux ?predAux ?s. FILTER(?sujAux = {sujetoExtraBorrar}) ?s ?p ?o. }}";

                ActualizarVirtuoso(query, pGrafoID);

                if (pBorrarMyGnoss)
                {
                    //borrar de mygnoss
                    query = $"{NamespacesVrituosoEscritura} DELETE from {ObtenerUrlGrafo("11111111-1111-1111-1111-111111111111")} {{ ?s ?p ?o. ?sujAux ?predAux ?s. }} from {ObtenerUrlGrafo("11111111-1111-1111-1111-111111111111")} WHERE {{?sujAux ?predAux ?s. FILTER( ?sujAux = {sujetoExtraBorrar} ) ?s ?p ?o.}} ";

                    ActualizarVirtuoso(query, pGrafoID);
                }
            }
        }

        /// <summary>
        /// Borra lsa tripletas de un formulario semántico
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarTripletasFormularioSemantico(string pNombreGrafo, string pElementoaEliminarID, bool pActualizarEnVirtuoso)
        {
            BorrarTripletasFormularioSemantico(pNombreGrafo, pElementoaEliminarID, pActualizarEnVirtuoso, "");
        }

        /// <summary>
        /// Borra lsa tripletas de un formulario semántico
        /// </summary>
        /// <param name="pNombreGrafo">Nombre del grafo en virtuoso</param>
        /// <param name="pElementoaEliminarID">Identificador del elemento a eliminar</param>
        public void BorrarTripletasFormularioSemantico(string pNombreGrafo, string pElementoaEliminarID, bool pActualizarEnVirtuoso, string pInfoExtra)
        {
            string query = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pNombreGrafo.ToLower())} {{ ?s ?p ?o. ?documento ?hasEntidad ?s.}} {ObtenerFrom(pNombreGrafo)} WHERE {{?documento ?hasEntidad ?s.  FILTER (?documento = <{mUrlIntranet}{pElementoaEliminarID.ToLower()}>) ?s ?p ?o }}";
            VirtuosoConnection conexion = ObtenerConexionParaGrafo(pNombreGrafo, true);
            mServicesUtilVirtuosoAndReplication.ActualizarVirtuoso(query, pNombreGrafo, true, 0, conexion, false);
        }

        public void BorrarTripleta(string pProyectoID, string pSujeto, string pPredicado, string pObjeto)
        {
            BorrarTripleta(pProyectoID, pSujeto, pPredicado, pObjeto, false);
        }

        /// <summary>
        /// Borra un una lista de triples de un sujeto que contengan unos predicados concretos
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto del que se van a eliminar los triples</param>
        /// <param name="pListaPredicados">Predicados que deben contener los triples a borrar del sujeto</param>
        public void BorrarListaPredicadosDeSujeto(string pProyectoID, string pSujeto, List<string> pListaPredicados)
        {
            if (string.IsNullOrEmpty(pSujeto) && pListaPredicados.Count > 0)
            {
                throw new ExcepcionGeneral("No se puede llamar a BorrarTripleta Sujeto, predicado ni objeto");
            }

            StringBuilder query = new StringBuilder(NamespacesVrituosoEscritura);
            query.AppendLine($"DELETE {ObtenerFromGraph(pProyectoID)} {{ ?s ?p ?o }} ");
            query.AppendLine($"{ObtenerFrom(pProyectoID)} WHERE {{?s ?p ?o. ");
            query.Append($"FILTER(?s={AniadirMayorQueYMenorQueAIdentificador(pSujeto)} AND ");

            if (pListaPredicados.Count == 1)
            {
                query.Append($" ?p = {AniadirMayorQueYMenorQueAIdentificador(pListaPredicados[0])}");
            }
            else
            {
                query.Append(" ?p IN (");
                string coma = "";
                foreach (string predicado in pListaPredicados)
                {
                    query.Append($"{coma}{AniadirMayorQueYMenorQueAIdentificador(predicado)}");
                    coma = ", ";
                }
                // Cierro IN
                query.Append(") ");
            }
            // Cierro filter
            query.AppendLine(")");
            // Cierro Where
            query.AppendLine("}");

            ActualizarVirtuoso(query.ToString(), pProyectoID);
        }

        /// <summary>
        /// Borra todos los triples de la lista de sujetos indicada
        /// </summary>
        /// <param name="pGrafo">Identificador del proyecto</param>
        /// <param name="pSujeto">Lista de sujetos a borrar</param>
        public void BorrarListaTriplesDeSujeto(string pGrafo, List<string> pListaSujetos)
        {
            StringBuilder query = new StringBuilder(NamespacesVrituosoEscritura);
            query.AppendLine($"DELETE {ObtenerFromGraph(pGrafo)} {{ ?s ?p ?o }} ");
            query.AppendLine($"{ObtenerFrom(pGrafo)} WHERE {{?s ?p ?o. ");
            query.Append($"FILTER (?s IN ( ");

            foreach (string sujeto in pListaSujetos)
            {
                query.Append($"{AniadirMayorQueYMenorQueAIdentificador(sujeto)}, ");
            }

            string consulta = query.ToString();
            consulta = $"{consulta.Substring(0, consulta.LastIndexOf(','))}) ). }}";

            ActualizarVirtuoso(consulta, pGrafo);
        }

        /// <summary>
        /// Borra una tripleta concreta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public void BorrarTripleta(string pProyectoID, string pSujeto, string pPredicado, string pObjeto, bool pUsarColaActualizacion)
        {
            if (string.IsNullOrEmpty(pSujeto) && string.IsNullOrEmpty(pPredicado) && string.IsNullOrEmpty(pObjeto))
            {
                throw new ExcepcionGeneral("No se puede llamar a BorrarTripleta Sujeto, predicado ni objeto");
            }

            string query = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pProyectoID)} {{ ?s ?p ?o }} {ObtenerFrom(pProyectoID)} WHERE {{?s ?p ?o. ";
            if (!string.IsNullOrEmpty(pSujeto))
            {
                query += $"FILTER(?s={pSujeto})";
            }
            if (!string.IsNullOrEmpty(pPredicado))
            {
                query += $"FILTER(?p={pPredicado})";
            }
            if (!string.IsNullOrEmpty(pObjeto))
            {
                query += $"FILTER(?o={pObjeto})";
            }
            query += " } ";

            ActualizarVirtuoso(query, pProyectoID);
        }


        /// <summary>
        /// Elimina el concept pasado por parámetro del tesauro y los hijos si se le indica
        /// </summary>
        /// <param name="pGrafo">Grafo del que borrar el concept</param>
        /// <param name="pSujetoConcept">Sujeto del concept a eliminar</param>
        /// <param name="pEliminarHijos">Si quieres eliminar o no los hijos</param>
        public void EliminarConceptEHijos(string pGrafo, string pSujetoConcept, bool pEliminarHijos)
        {
            string subconsultaHijos = string.Empty;
            string grafo = ObtenerFromGraph(pGrafo);

            string consulta = $"prefix skos:<http://www.w3.org/2008/05/skos#> delete from graph {grafo.ToLower().Replace("from", string.Empty)} {{ ?s ?p ?o }} {grafo} where {{ {{ ?s ?p ?o . filter((?s = <{pSujetoConcept}> and ?p != skos:narrower) or (?o = <{pSujetoConcept}> and ?p != skos:broader)) }} }}";

            if (pEliminarHijos)
            {
                subconsultaHijos = $"UNION {{ ?s ?p ?o. <{pSujetoConcept}> skos:narrower+ ?s .}}";
                consulta = $"prefix skos:<http://www.w3.org/2008/05/skos#> delete from graph {grafo.ToLower().Replace("from", string.Empty)} {{ ?s ?p ?o }} {grafo} where {{ {{ ?s ?p ?o . filter(?s = <{pSujetoConcept}> or ?o = <{pSujetoConcept}>) }} {subconsultaHijos} }}";
            }

            ActualizarVirtuoso(consulta, pGrafo);
        }

        /// <summary>
        /// Borra un grupo de tripletas de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public void BorrarGrupoTripletas(string pGrafo, string pTripletas, bool pUsarColaActualizacion)
        {
            if (string.IsNullOrEmpty(pTripletas))
            {
                throw new ExcepcionGeneral("No se puede llamar a BorrarGrupoTripletas sin tripletas");
            }

            string query = NamespacesVrituosoEscritura;
            query += $"DELETE DATA {ObtenerFromGraph(pGrafo.ToLower())} {{ {pTripletas} }} ";

            ActualizarVirtuoso(query, pGrafo);
        }

        /// <summary>
        /// Modifica una lista de triples en un grafo, sustituyendo los triples que tengan algún predicado en pListaPredicadosEliminar por los nuevos triples
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pTriplesInsertar">Tripletas a borrar</param>
        /// <param name="pListaPredicadosEliminar">Lista de predicados que van a actualizar los triples.</param>
        /// <param name="pPrioridad">Prioridad de la replicación</param>
        /// <param name="pSujeto">Sujeto al que se le van a modificar los triples. (Opcional, si no se le da valor, se eliminaran todos los triples que contenan los predicados en pListaPredicadosEliminar)</param>
        /// <param name="pEliminarSiempre">Verdad si se quiere eliminar todos los triples del sujeto, sin filtrar por predicado</param>
        public void ModificarListaTripletas(string pProyectoID, string pTriplesInsertar, List<string> pListaPredicadosEliminar, short pPrioridad, string pSujeto, bool pEliminarSiempre = false)
        {
            if (pTriplesInsertar == null || pTriplesInsertar.Length == 0)
            {
                return;
            }

            if (pListaPredicadosEliminar == null || pListaPredicadosEliminar.Count == 0)
            {
                InsertaTripletas(pProyectoID, pTriplesInsertar, pPrioridad);
            }
            else
            {
                StringBuilder query = new StringBuilder(NamespacesVrituosoEscritura);
                query.AppendLine($" MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{{pTriplesInsertar}}} ");
                query.Append(" WHERE {?s ?p ?o. FILTER (");
                query.Append($"?s = {AniadirMayorQueYMenorQueAIdentificador(pSujeto)} ");
                query.Append(" AND ");

                if (pListaPredicadosEliminar.Count == 1)
                {
                    query.Append($" ?p = {AniadirMayorQueYMenorQueAIdentificador(pListaPredicadosEliminar[0])}");
                }
                else
                {
                    query.Append(" ?p IN (");
                    string coma = "";
                    foreach (string predicado in pListaPredicadosEliminar)
                    {
                        query.Append($"{coma}{AniadirMayorQueYMenorQueAIdentificador(predicado)}");
                        coma = ", ";
                    }
                    // Cierro IN
                    query.Append(") ");
                }

                // Cierro filter
                query.AppendLine(")");

                // Cierro WHERE
                query.Append("} ");

                ActualizarVirtuoso(query.ToString(), pProyectoID);
            }
        }

        /// <summary>
        /// Añade (si no los tiene) los símbolos mayor que y menor que a un identificador, para usarlo en una query
        /// </summary>
        /// <param name="pIdentificador">Ej: http://gnoss/C5CAADB1-1B85-45C9-BFAE-C02F1B337B19</param>
        /// <returns>El identificador envuelto en los símbolos mayor que y menor que</returns>
        private static string AniadirMayorQueYMenorQueAIdentificador(string pIdentificador)
        {
            string resultado = pIdentificador;

            if (!resultado.StartsWith('<'))
            {
                resultado = $"<{resultado}";
            }
            if (!resultado.EndsWith('>'))
            {
                resultado = $"{resultado}>";
            }

            return resultado;
        }

        /// <summary>
        /// Nos indica a partir del filtro rdfType si la petición que se está haciendo es a algúna página del espacio personal.
        /// </summary>
        /// <param name="pListaFiltros">Lita de filtros a comprobar</param>
        /// <returns></returns>
        private static bool EsPeticionPerfilPersonal(Dictionary<string, List<string>> pListaFiltros)
        {
            return pListaFiltros.ContainsKey("rdf:type") && pListaFiltros["rdf:type"].Count > 0 && pListaFiltros["rdf:type"].Any(item => mTiposBusquedasEspacioPersonal.Contains(item));
        }

        /// <summary>
        /// Devuelve el texto sin caracteres como . , ; ? ¿ !¡ ...
        /// </summary>
        public static string RemoverSignosSearch(string texto)
        {
            StringBuilder textoSinSignos = new StringBuilder(texto.Length);
            int indexConSigno;
            foreach (char caracter in texto)
            {
                indexConSigno = SIGNOS_ELIMINAR_SEARCH.IndexOf(caracter);
                if (indexConSigno > -1)
                    textoSinSignos.Append(" ");
                else
                    textoSinSignos.Append(caracter);
            }
            return textoSinSignos.ToString();
        }

        /// <summary>
        /// Borra un grupo de tripletas en lista de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public int ModificarGrupoTripletasEnLista(string pGrafo, string[] pTripletaBorrar, string pTripletaInsertar, bool pUsarColaActualizacion, string pInfoExtra)
        {
            int numResultados = -1;
            if (pTripletaBorrar == null || pTripletaBorrar.Length == 0 || pTripletaInsertar == null || pTripletaInsertar.Length == 0)
            {
                return numResultados;
            }

            string query = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pGrafo).ToLower()} DELETE {{ ?s ?p ?o }} INSERT {{{pTripletaInsertar}}} WHERE {{?s ?p ?o. FILTER (";

            string objeto = pTripletaBorrar[2].Replace("\\", "\\\\");

            bool objetoEsString = objeto.StartsWith('\"') && objeto.EndsWith('\"');

            //Si contiene comillas entre medio, se las remplazo por \"
            if ((objeto.Length > 2) && (objeto.StartsWith('\"')) && (objeto.EndsWith('\"')) && (objeto.Substring(1, objeto.Length - 2).Contains("\"")))
            {
                objeto = $"\"{objeto.Substring(1, objeto.Length - 2).Replace("\"", "\\\"")}\"";
            }


            if (pTripletaBorrar.Length > 3 && !string.IsNullOrEmpty(pTripletaBorrar[3]))
            {
                if (!objetoEsString)
                {
                    objeto = $"\"{objeto.Substring(1, objeto.Length - 2)}\"";
                }

                objeto += $"@{pTripletaBorrar[3]}";
            }

            query += $"(?s={pTripletaBorrar[0]} AND ?p={pTripletaBorrar[1]} AND ?o={objeto}) OR {query.Substring(0, query.Length - 3)})}} ";

            numResultados = ActualizarVirtuoso(query, pGrafo);

            return numResultados;
        }

        /// <summary>
        /// Borra un grupo de tripletas en lista de un grafo.
        /// </summary>
        /// <param name="pGrafo">Identificador del grafo</param>
        /// <param name="pTripletas">Tripletas a borrar</param>
        /// <param name="pUsarColaActualizacion">TRUE si se va a mandar </param>
        public int BorrarGrupoTripletasEnLista(string pGrafo, List<TripleWrapper> pTripletas, bool pUsarColaActualizacion, string pInfoExtra)
        {
            int resultado = -1;
            if (pTripletas.Count == 0)
            {
                return resultado;
            }

            StringBuilder query = new StringBuilder($"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pGrafo).ToLower()} {{ ?s ?p ?o }} {ObtenerFrom(pGrafo).ToLower()} WHERE {{?s ?p ?o. FILTER (");

            foreach (TripleWrapper triple in pTripletas)
            {
                string objeto = triple.Object.Replace("\\", "\\\\");

                bool objetoEsString = objeto.StartsWith('\"') && objeto.EndsWith('\"');

                //Si contiene comillas entre medio, se las remplazo por \"
                if (objeto.Length > 2 && objeto.StartsWith('\"') && objeto.EndsWith('\"') && objeto.Substring(1, objeto.Length - 2).Contains("\""))
                {
                    objeto = $"\"{objeto.Substring(1, objeto.Length - 2).Replace("\"", "\\\"")}\"";
                }

                if (!string.IsNullOrEmpty(triple.ObjectLanguage))
                {
                    if (!objetoEsString)
                    {
                        objeto = $"\"{objeto.Substring(1, objeto.Length - 2)}\"";
                    }

                    objeto += "@" + triple.ObjectLanguage;
                }

                if (string.IsNullOrEmpty(triple.ObjectType) || string.IsNullOrWhiteSpace(triple.ObjectType))
                {
                    query.Append($"(?s= {triple.Subject} AND ?p={triple.Predicate} AND ?o={objeto}) OR");
                }
                else
                {
                    query.Append($"(?s= {triple.Subject} AND ?p={triple.Predicate} AND (?o={objeto} or ?o = {objeto}^^<{triple.ObjectType}>)) OR");
                }
            }

            query.Remove(query.Length - 3, 3);
            query.Append(")}");

            resultado = ActualizarVirtuoso(query.ToString(), pGrafo);

            return resultado;
        }

        /// <summary>
        /// Genera una tripleta concreta
        /// </summary>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pIdioma">Idioma</param>
        public static string GenerarTripleta(string pSujeto, string pPredicado, string pObjeto)
        {
            return GenerarTripleta(pSujeto, pPredicado, pObjeto, null);
        }

        /// <summary>
        /// Genera una tripleta concreta
        /// </summary>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pIdioma">Idioma</param>
        public static string GenerarTripleta(string pSujeto, string pPredicado, string pObjeto, string pIdioma)
        {
            string tripleta = $"{pSujeto} {pPredicado} ";

            string puntoFinalTriple = "";
            if (pObjeto.TrimEnd().EndsWith('.'))
            {
                puntoFinalTriple = " .";
                pObjeto = pObjeto.Substring(0, pObjeto.LastIndexOf('.')).Trim();
            }

            bool objetoEsString = pObjeto.StartsWith('\"') && pObjeto.EndsWith('\"');
            bool objetoConTipo = ObjetoTieneTipo(pObjeto);

            if (pObjeto.Contains("\\"))
            {
                pObjeto = pObjeto.Replace("\\n", " ").Replace("\\r", "").Replace("\\t", " ");
            }

            pObjeto = pObjeto.Replace("\\", "\\\\");

            //Si contiene comillas entre medio, se las remplazo por \"
            if (pObjeto.Length > 2 && objetoEsString && pObjeto.Substring(1, pObjeto.Length - 2).Contains("\""))
            {
                pObjeto = $"\"\"\"{pObjeto.Substring(1, pObjeto.Length - 2).Replace("\"", "\\\"")}\"\"\"{puntoFinalTriple}";
            }
            else if (objetoEsString)
            {
                //Se añaden dos comillas más para tener 3. De esta forma el texto se puede insertar con saltos de linea y caracteres de control.
                pObjeto = $"\"\"{pObjeto}\"\"";
            }

            if (!string.IsNullOrEmpty(pIdioma))
            {
                if (!objetoEsString && !objetoConTipo)
                {
                    pObjeto = $"\"\"\"{pObjeto.Substring(1, pObjeto.Length - 2)}\"\"\"";
                }
                if (!objetoConTipo)
                {
                    pObjeto += $"@{pIdioma}";
                }
            }

            tripleta += $"{pObjeto} ";

            if (!tripleta.Contains(". ") || tripleta.Trim().LastIndexOf(".") != tripleta.Trim().Length - 1)
            {
                tripleta += " . ";
            }

            return $"{tripleta} \n ";
        }

        /// <summary>
        /// Nos indica si el objeto del triple pasado por parámetro tiene definido o no el tipo.
        /// Se comprueba con una expresión Regex que valida que el objeto comience por " (siempre serán string) y termine con el tipo escrito con url(<http://www.w3.org/2001/XMLSchema#string>) o con prefijo (xsd:string)
        /// </summary>
        /// <param name="pObjeto">Objeto del triple a comprobar</param>
        /// <returns></returns>
        public static bool ObjetoTieneTipo(string pObjeto)
        {
            return Regex.IsMatch(pObjeto, "^\"(?s).+\"\\^\\^(?:(?:<https?:\\/\\/.+>)|(?:[A-Za-z0-9\\-_]+:[^\\s\"]*))$", RegexOptions.None, new TimeSpan(TimeSpan.TicksPerSecond));
        }

        /// <summary>
        /// Nos indica si el objeto del triple pasado por parámettro sigue una estructura valida para entidades principales
        /// </summary>
        /// <param name="pSujeto">Sujeto a comprobar</param>
        /// <returns></returns>
        public static bool SujetoValido(string pSujeto)
        {
            return Regex.IsMatch(pSujeto, "(^<https?:\\/\\/.*)([0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}_[0-9A-Fa-f]{8}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{4}-[0-9A-Fa-f]{12}>)", RegexOptions.None, new TimeSpan(TimeSpan.TicksPerSecond));
        }

        /// <summary>
        /// Genera tripletas para virtuoso sin hacer conversiones absurdas inecesarias.
        /// </summary>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Tripleta</returns>
        public string GenerarTripletaSinConversionesAbsurdas(string pSujeto, string pPredicado, string pObjeto)
        {
            return GenerarTripletaSinConversionesAbsurdas(pSujeto, pPredicado, pObjeto, null);
        }

        /// <summary>
        /// Genera tripletas para virtuoso sin hacer conversiones absurdas inecesarias.
        /// </summary>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pIdioma">Idioma de la tripleta</param>
        /// <returns>Tripleta</returns>
        public string GenerarTripletaSinConversionesAbsurdas(string pSujeto, string pPredicado, string pObjeto, string pIdioma, string pTipo = null)
        {
            bool objetoEsString = pObjeto.StartsWith('\"') && pObjeto.EndsWith('\"');

            pObjeto = pObjeto.Replace("\\", "\\\\");

            //Si contiene comillas entre medio, se las remplazo por \"
            if (pObjeto.Length > 2 && pObjeto.StartsWith('\"') && pObjeto.EndsWith('\"') && pObjeto.Substring(1, pObjeto.Length - 2).Contains("\""))
            {
                pObjeto = $"\"\"\"{pObjeto.Substring(1, pObjeto.Length - 2).Replace("\"", "\\\"")}\"\"\"";
            }
            else if (objetoEsString)
            {
                //Se añaden dos comillas más para tener 3. De esta forma el texto se puede insertar con saltos de linea y caracteres de control.
                pObjeto = $"\"\"{pObjeto}\"\"";
            }

            if (!string.IsNullOrEmpty(pIdioma))
            {
                if (!objetoEsString)
                {
                    pObjeto = $"\"\"\"{pObjeto.Substring(1, pObjeto.Length - 2)}\"\"\"";
                }

                pObjeto += $"@{pIdioma}";
            }

            string tripleta = "";

            if (string.IsNullOrEmpty(pTipo) || string.IsNullOrWhiteSpace(pTipo))
            {
                tripleta = $"{pSujeto} {pPredicado} {pObjeto} ";
            }
            else
            {
                tripleta = $"{pSujeto} {pPredicado} {pObjeto}^^<{pTipo}> ";
            }

            if (!tripleta.Contains(". ") || tripleta.Trim().LastIndexOf(".") != tripleta.Trim().Length - 1)
            {
                tripleta += " . ";
            }

            return $"{tripleta} \n ";
        }

        public string GenerarTripletaRecogidadeVirtuosoSinConversionesAbsurdas(string pSujeto, string pPredicado, string pObjeto, string pObjetoSinMinuscula, List<string> pFecha, List<string> pNumero, List<FacetaEntidadesExternas> pEntExt, ref string pTripletasSemanticasAdiccionales, bool pTextoTesauroInvariable = false)
        {
            return GenerarTripletaRecogidadeVirtuosoSinConversionesAbsurdas(pSujeto, pPredicado, pObjeto, pObjetoSinMinuscula, pFecha, pNumero, pEntExt, ref pTripletasSemanticasAdiccionales, null, null, pTextoTesauroInvariable);
        }

        public string GenerarTripletaRecogidadeVirtuosoSinConversionesAbsurdas(string pSujeto, string pPredicado, string pObjeto, string pObjetoSinMinuscula, List<string> pFecha, List<string> pNumero, List<FacetaEntidadesExternas> pEntExt, ref string pTripletasSemanticasAdiccionales, string pIdioma, string pTipo = null, bool pTextoTesauroInvariable = false)
        {
            string tripleta = "";
            string tripletaGeonames = "";

            if (pSujeto.StartsWith("http"))
            {
                tripleta += $"<{pSujeto}> ";
            }
            else
            {
                tripleta += pSujeto;
            }

            tripleta += $"<{pPredicado}> ";

            if (!pObjeto.StartsWith("http") || pObjeto.Contains("|") || pObjeto.LastIndexOf("http") != 0 || pObjeto.Contains(" "))
            {
                int n;
                float n2;

                string predicadoSinNamespace = "";
                if (pPredicado.Contains("#"))
                {
                    string[] seccionPredicado = pPredicado.Split('#');
                    predicadoSinNamespace = seccionPredicado[seccionPredicado.Length - 1];
                }
                if (float.TryParse(pObjeto, out n2) && (pNumero.Contains(pPredicado) || (!string.IsNullOrEmpty(predicadoSinNamespace) && pNumero.Contains(predicadoSinNamespace))) && (pObjeto.Contains(".") || pObjeto.Contains(",")))
                {
                    pObjeto = $"\"{pObjeto.Replace(",", ".")}\"^^xsd:decimal";
                }
                else if (int.TryParse(pObjeto, out n) && (pNumero.Contains(pPredicado) || (!string.IsNullOrEmpty(predicadoSinNamespace) && pNumero.Contains(predicadoSinNamespace))))
                {
                    pObjeto = $"\"{pObjeto}\"^^xsd:int";
                }
                else if (TIPOS_GEOMETRIA.Any(item => pObjeto.StartsWith($"{item}(")))
                {
                    pObjeto = $"\"{pObjetoSinMinuscula}\"^^<http://www.openlinksw.com/schemas/virtrdf#Geometry>";
                }
                else if (!pFecha.Contains(pPredicado) && (string.IsNullOrEmpty(predicadoSinNamespace) || !pFecha.Contains(predicadoSinNamespace)) && !pTextoTesauroInvariable)
                {
                    pObjeto = $"\"{UtilCadenas.EliminarHtmlDeTexto(pObjeto)}\"";
                }
                else if (pTextoTesauroInvariable)
                {
                    pObjeto = $"\"{pObjetoSinMinuscula}\"";
                }
            }
            else
            {
                Uri urlValida = null;
                if (string.IsNullOrEmpty(pIdioma) && Uri.TryCreate(UtilCadenas.PasarAUtf8(pObjeto), UriKind.Absolute, out urlValida) && !UtilCadenas.PasarAUtf8(pObjeto).Contains("Ã") && (pTipo == null || !pTipo.Equals(XSD_STRING)))
                {
                    pObjeto = urlValida.AbsoluteUri;
                    pObjeto = $"<{pObjeto}> ";
                }
                else
                {
                    pObjeto = $"\"{pObjeto}\"";
                }
            }

            pObjeto = pObjeto.Replace("\\", "\\\\");

            bool objetoEsString = pObjeto.StartsWith('\"') && pObjeto.EndsWith('\"');

            //Si contiene comillas entre medio, se las remplazo por \"
            if ((pObjeto.Length > 2) && objetoEsString && (pObjeto.Substring(1, pObjeto.Length - 2).Contains("\"")))
            {
                pObjeto = $"\"\"\"{pObjeto.Substring(1, pObjeto.Length - 2).Replace("\"", "\\\"")}\"\"\"";
            }
            else if (objetoEsString)
            {
                pObjeto = $"\"\"{pObjeto}\"\"";
            }

            if (objetoEsString && !string.IsNullOrEmpty(pIdioma))
            {
                pObjeto += $"@{pIdioma}";
            }

            bool esObjetoExterno = false;

            if (pEntExt != null)
            {
                // Traer las entidades externas para objetos que sean URL y puedan tener otras entidades en otros grafos.
                for (int i = 0; i < pEntExt.Count; i++)
                {
                    string entidadID = pEntExt[i].EntidadID;

                    if (!entidadID.EndsWith('_'))
                    {
                        entidadID += "_";
                    }

                    if (pObjeto.ToLower().Contains(pEntExt[i].EntidadID.ToLower()))
                    {
                        Regex regex = new Regex(@"(?im)" + entidadID + @"[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}_[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}");

                        if (pEntExt[i].EsEntidadSecundaria)
                        {
                            pTripletasSemanticasAdiccionales += ObtieneTripletasOtrasEntidades(pObjetoSinMinuscula.ToLower(), pEntExt[i].ProyectoID.ToString(), pEntExt);
                        }
                        else if (pObjeto.ToLower().Contains(pEntExt[i].EntidadID.ToLower()) && !pEntExt[i].EsEntidadSecundaria && regex.IsMatch(pObjeto))
                        {
                            esObjetoExterno = true;
                        }
                    }
                }
            }

            if (esObjetoExterno)
            {
                string objetoExterno = pObjeto.Substring(0, pObjeto.LastIndexOf("_"));
                objetoExterno = "<http://gnoss/" + objetoExterno.Substring(objetoExterno.LastIndexOf("_") + 1).ToUpper() + ">";
                tripleta += objetoExterno;
            }
            else
            {
                tripleta += pObjeto;
            }

            if (!tripleta.Contains(". ") || tripleta.Trim().LastIndexOf(".") != tripleta.Trim().Length - 1)
            {
                tripleta += " . ";
            }

            tripleta += " \n ";

            if (pObjeto.Contains("geonames"))
            {
                tripletaGeonames = ObtieneTripletasGeonames(pObjeto);
                return $"{tripleta}{tripletaGeonames}";
            }
            else
            {
                return tripleta;
            }
        }

        public string GenerarTripletaRecogidadeVirtuoso(string pSujeto, string pPredicado, string pObjeto, string pObjetoSinMinuscula, List<string> pFecha, List<string> pNumero, List<string> pTextoInvariable, List<FacetaEntidadesExternas> pEntExt, string pIdioma)
        {
            string tripleta = "";
            string tripletaGeonames = "";

            if (pPredicado.StartsWith('<') && pPredicado.EndsWith('>'))
            {
                pPredicado = pPredicado.Replace("<", "").Replace(">", "");
            }

            if (pObjeto.StartsWith("<http") && pObjeto.EndsWith('>'))
            {
                pObjeto = pObjeto.Replace("<", "").Replace(">", "");
            }

            if (pSujeto.StartsWith("http"))
            {
                pSujeto = $"<{pSujeto}> ";
            }

            if (pPredicado == "http://gnoss/type" || pTextoInvariable.Contains(pPredicado))//No hay que hay conversiones, solo guardar como string
            {
                pObjeto = pObjetoSinMinuscula;
                if (!pTextoInvariable.Contains(pPredicado))
                {
                    pObjeto = UtilCadenas.EliminarHtmlDeTexto(pObjetoSinMinuscula);
                }
                pObjeto = $"\"{pObjeto}\"";
            }
            else if (!pObjeto.StartsWith("http") || pObjeto.Contains("|") || pObjeto.LastIndexOf("http") != 0 || pObjeto.Contains(" "))
            {
                int n;
                float n2;
                string predicadoSinNamespace = "";
                if (pPredicado.Contains("#"))
                {
                    string[] seccionPredicado = pPredicado.Split('#');
                    predicadoSinNamespace = seccionPredicado[seccionPredicado.Length - 1];
                }
                if (float.TryParse(pObjeto, out n2) && (pNumero.Contains(pPredicado) || (!string.IsNullOrEmpty(predicadoSinNamespace) && pNumero.Contains(predicadoSinNamespace))) && (pObjeto.Contains(".") || pObjeto.Contains(",")))
                {
                    pObjeto = $"\"{pObjeto.Replace(",", ".")}\"^^xsd:decimal";
                }
                else if (int.TryParse(pObjeto, out n) && (pNumero.Contains(pPredicado) || (!string.IsNullOrEmpty(predicadoSinNamespace) && pNumero.Contains(predicadoSinNamespace))))
                {
                    pObjeto = $"\"{pObjeto}\"^^xsd:int";
                }
                else if (TIPOS_GEOMETRIA.Any(item => pObjeto.ToLower().StartsWith($"{item}(")))
                {
                    pObjeto = $"\"{pObjetoSinMinuscula}\"^^<http://www.openlinksw.com/schemas/virtrdf#Geometry>";
                }
                else if (!pFecha.Contains(pPredicado) && (string.IsNullOrEmpty(predicadoSinNamespace) || !pFecha.Contains(predicadoSinNamespace)))
                {
                    pObjeto = UtilCadenas.EliminarHtmlDeTexto(pObjeto);
                    pObjeto = pObjeto.Replace("\\", "\\\\");
                    pObjeto = $"\"{pObjeto}\"";
                }
            }
            else
            {
                Uri urlValida = null;
                if (string.IsNullOrEmpty(pIdioma) && Uri.TryCreate(UtilCadenas.PasarAUtf8(pObjeto), UriKind.Absolute, out urlValida) && !UtilCadenas.PasarAUtf8(pObjeto).Contains("Ã"))
                {
                    pObjeto = urlValida.AbsoluteUri;
                    if (EsIDGnoss(pObjeto))
                    {
                        pObjeto = pObjetoSinMinuscula;
                    }
                    pObjeto = pObjeto.Replace("\\", "\\\\");
                    pObjeto = $"<{pObjeto}> ";
                }
                else
                {
                    pObjeto = UtilCadenas.EliminarHtmlDeTexto(pObjeto);
                    pObjeto = pObjeto.Replace("\\", "\\\\");
                    pObjeto = $"\"{pObjeto}\"";
                }
            }

            bool esObjetoExterno = false;
            string objetoExterno = string.Empty;

            if (pEntExt != null)
            {
                // Traer las entidades externas para objetos que sean URL y puedan tener otras entidades en otros grafos.
                for (int i = 0; i < pEntExt.Count; i++)
                {
                    if (pObjeto.ToLower().Contains(pEntExt[i].EntidadID.ToLower()))
                    {
                        string entidadID = pEntExt[i].EntidadID;

                        if (!entidadID.EndsWith('_'))
                        {
                            entidadID += "_";
                        }
                        Regex regex = new Regex(@"(?im)" + entidadID + @"[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}_[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}");

                        if (pObjeto.ToLower().Contains(pEntExt[i].EntidadID.ToLower()) && !pEntExt[i].EsEntidadSecundaria && regex.IsMatch(pObjeto))
                        {
                            esObjetoExterno = true;
                        }
                    }
                }
            }

            if (esObjetoExterno)
            {
                if (pObjeto.Contains("_"))
                {
                    objetoExterno = pObjeto.Substring(0, pObjeto.LastIndexOf("_"));
                    objetoExterno = "<http://gnoss/" + objetoExterno.Substring(objetoExterno.LastIndexOf("_") + 1).ToUpper() + ">";
                    tripleta = GenerarTripleta(pSujeto, $"<{pPredicado}> ", objetoExterno);
                }
                else if (pObjeto.Contains("<") && pObjeto.Contains(">"))
                {

                    tripleta = GenerarTripleta(pSujeto, $"<{pPredicado}> ", pObjeto);
                }
                else
                {
                    tripleta = GenerarTripleta(pSujeto, $"<{pPredicado}> ", $"<{pObjeto}> ");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(pIdioma))
                {
                    tripleta = GenerarTripleta(pSujeto, $"<{pPredicado}> ", pObjeto);
                }
                else
                {
                    tripleta = GenerarTripleta(pSujeto, $"<{pPredicado}> ", pObjeto, pIdioma);
                }
            }

            //Quitamos los posibles saltos de linea
            tripleta = $"{tripleta.Replace("\n", " ").Replace("\r", string.Empty)} \n ";

            if (pObjeto.Contains("geonames"))
            {
                tripletaGeonames = ObtieneTripletasGeonames(pObjeto);
                return tripleta + tripletaGeonames;
            }
            else
            {
                return tripleta;
            }
        }

        /// <summary>
        /// Indica si un objeto es un ID De GNOSS.
        /// </summary>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>TRUE si un objeto es un ID De GNOSS, FALSE en caso contrario</returns>
        public static bool EsIDGnoss(string pObjeto)
        {
            Guid aux;
            return pObjeto.StartsWith("http://gnoss/") && Guid.TryParse(pObjeto.Substring("http://gnoss/".Length), out aux);
        }

        /// <summary>
        /// Obtiene las tripletas de otras entidades para insertarlas en el grafo de búsqueda
        /// </summary>
        /// <param name="geonamesID">ID de geonames</param>
        public string ObtieneTripletasOtrasEntidades(string pRecursoID, string pGrafoID, List<FacetaEntidadesExternas> pEntExt)
        {
            List<string> recursosCargadosID = new List<string>();
            return ObtieneTripletasOtrasEntidades(pRecursoID, pGrafoID, pEntExt, recursosCargadosID);
        }

        /// <summary>
        /// Obtiene las tripletas de otras entidades para insertarlas en el grafo de búsqueda
        /// </summary>
        /// <param name="geonamesID">ID de geonames</param>
        public string ObtieneTripletasOtrasEntidades(string pRecursoID, string pGrafoID, List<FacetaEntidadesExternas> pEntExt, List<string> pRecursosCargadosID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} select * from <{mUrlIntranet}{pGrafoID}> {{?s ?p ?o. FILTER (?s=<{pRecursoID}> )}} ";

            if (pRecursosCargadosID == null || !pRecursosCargadosID.Contains(pRecursoID))
            {
                if (pRecursosCargadosID != null)
                {
                    pRecursosCargadosID.Add(pRecursoID);
                }

                LeerDeVirtuoso(query, "OtrasEntidades", facetadoDS, "OtrasEntidades");

                if (facetadoDS.Tables["OtrasEntidades"].Rows.Count > 0)
                {
                    StringBuilder tripletasGeonames = new StringBuilder();
                    Dictionary<string, string> entidadesHijas = new Dictionary<string, string>();

                    foreach (DataRow myrow in facetadoDS.Tables["OtrasEntidades"].Rows)
                    {
                        string sujeto = myrow[0].ToString().ToLower();
                        string predicado = myrow[1].ToString();
                        string objeto = myrow[2].ToString().ToLower();
                        if (!predicado.Equals("http://www.w3.org/1999/02/22-rdf-syntax-ns#type"))
                        {
                            //Si es una URL, comprobamos si contiene entidades externas.
                            if (objeto.StartsWith("http"))
                            {
                                Uri urlValida = null;
                                if (Uri.TryCreate(objeto, UriKind.Absolute, out urlValida) && !objeto.Contains("Ã"))
                                {
                                    objeto = urlValida.AbsoluteUri;
                                    objeto = objeto.Replace("\\", "\\\\");
                                    objeto = objeto.Replace("\"", "\\\"");

                                    if (!entidadesHijas.ContainsKey(myrow[2].ToString()) && !predicado.EndsWith("label"))
                                    {
                                        entidadesHijas.Add(myrow[2].ToString(), predicado);
                                    }
                                    objeto = $"<{objeto}> ";
                                }
                                else
                                {
                                    objeto = objeto.Replace("\\", "\\\\");
                                    objeto = objeto.Replace("\"", "\\\"");
                                    objeto = "\"" + objeto + "\"";
                                }

                                tripletasGeonames.Append(GenerarTripleta($"<{sujeto}>", $"<{predicado}>", objeto));
                            }
                            else
                            {
                                tripletasGeonames.Append(GenerarTripleta($"<{sujeto}>", $"<{predicado}>", $"\"{objeto}\""));
                            }
                        }
                    }

                    foreach (string entidadHija in entidadesHijas.Keys)
                    {
                        //Debemos obtener de las entidades externas el grafo
                        foreach (FacetaEntidadesExternas entidadExt in pEntExt)
                        {
                            if (entidadExt.BuscarConRecursividad && entidadHija.Contains(entidadExt.EntidadID))
                            {
                                tripletasGeonames.Append(ObtieneTripletasOtrasEntidades(entidadHija, entidadExt.ProyectoID.ToString(), pEntExt, pRecursosCargadosID));
                            }
                        }
                    }

                    return tripletasGeonames.ToString();
                }
                else { return ""; }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Obtiene las tripletas de otras entidades para insertarlas en el grafo de búsqueda
        /// </summary>
        /// <param name="geonamesID">ID de geonames</param>
        public FacetadoDS ObtieneTripletasOtrasEntidadesDS(string pRecursoID, string pGrafoID, List<FacetaEntidadesExternas> pEntExt)
        {
            List<string> recursosCargadosID = new List<string>();
            return ObtieneTripletasOtrasEntidadesDS(pRecursoID, pGrafoID, pEntExt, recursosCargadosID);
        }

        /// <summary>
        /// Obtiene las tripletas de otras entidades para insertarlas en el grafo de búsqueda
        /// </summary>
        /// <param name="geonamesID">ID de geonames</param>
        public FacetadoDS ObtieneTripletasOtrasEntidadesDS(string pRecursoID, string pGrafoID, List<FacetaEntidadesExternas> pEntExt, List<string> pRecursosCargadosID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} select ?s ?p ?o lang(?o) from <{mUrlIntranet}{pGrafoID}> {{?s ?p ?o. FILTER (?s=<{pRecursoID}> )}} ";

            if (pRecursosCargadosID == null || !pRecursosCargadosID.Contains(pRecursoID))
            {
                if (pRecursosCargadosID != null)
                {
                    pRecursosCargadosID.Add(pRecursoID);
                }

                LeerDeVirtuoso(query, "OtrasEntidades", facetadoDS, "OtrasEntidades");

                if (facetadoDS.Tables["OtrasEntidades"].Rows.Count > 0)
                {
                    Dictionary<string, string> entidadesHijas = new Dictionary<string, string>();

                    foreach (DataRow myrow in facetadoDS.Tables["OtrasEntidades"].Rows)
                    {
                        string predicado = myrow[1].ToString();
                        string objeto = myrow[2].ToString().ToLower();
                        if (!predicado.Equals("http://www.w3.org/1999/02/22-rdf-syntax-ns#type") && objeto.StartsWith("http"))
                        {
                            //Si es una URL, comprobamos si contiene entidades externas.
                            Uri urlValida = null;
                            if (Uri.TryCreate(objeto, UriKind.Absolute, out urlValida) && !objeto.Contains("Ã") && !entidadesHijas.ContainsKey(myrow[2].ToString()) && !predicado.EndsWith("label"))
                            {
                                entidadesHijas.Add(myrow[2].ToString(), predicado);
                            }
                        }
                    }

                    foreach (string entidadHija in entidadesHijas.Keys)
                    {
                        //Debemos obtener de las entidades externas el grafo
                        foreach (FacetaEntidadesExternas entidadExt in pEntExt)
                        {
                            if (entidadExt.BuscarConRecursividad && entidadHija.Contains(entidadExt.EntidadID))
                            {
                                facetadoDS.Merge(ObtieneTripletasOtrasEntidadesDS(entidadHija, entidadExt.ProyectoID.ToString(), pEntExt, pRecursosCargadosID));
                            }
                        }
                    }
                }
            }

            return facetadoDS;
        }

        /// <summary>
        /// Obtiene las tripletas relativas a un recurso en geonames.
        /// </summary>
        /// <param name="geonamesID">ID de geonames</param>
        public string ObtieneTripletasGeonames(string geonamesID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;

            geonamesID = geonamesID.Trim();
            if (geonamesID.EndsWith("/>"))
            {
                geonamesID = geonamesID.Trim('>').Trim('/');
            }

            string geonamesIDsimple = geonamesID.Substring(geonamesID.LastIndexOf("/") + 1).Trim('>');
            query += $" select * from <http://gnoss.com/geonames> {{?s ?p ?o. FILTER (?s=<http://geonames.org/{geonamesIDsimple}> and ?p in (<http://www.geonames.org/ontology#name>, <http://www.geonames.org/ontology#latitude>, <http://www.geonames.org/ontology#longitude>))}} ";

            LeerDeVirtuoso(query, "ids", facetadoDS, "geonames");

            if (facetadoDS.Tables["ids"].Rows.Count > 0)
            {
                StringBuilder tripletasGeonames = new StringBuilder();
                foreach (DataRow myrow in facetadoDS.Tables["ids"].Rows)
                {
                    string propiedad = myrow[1].ToString();
                    string objeto = myrow[2].ToString();
                    float objetoFloat;
                    if ((propiedad.Equals("http://www.geonames.org/ontology#latitude") || propiedad.Equals("http://www.geonames.org/ontology#longitude")) && float.TryParse(objeto, out objetoFloat))
                    {
                        objeto = $"\"{objeto.Replace(',', '.')}\"^^xsd:decimal";
                    }
                    else
                    {
                        objeto = $"\"{objeto.ToLower().Replace(",", ".")}\"";
                    }
                    tripletasGeonames.Append(GenerarTripleta($"<{myrow[0].ToString()}>", $"<{propiedad}>", objeto));
                }
                return tripletasGeonames.ToString();
            }
            else { return ""; }
        }

        /// <summary>
        /// Inserta una tripleta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public void InsertaTripleta(string pProyectoID, string pSujeto, string pPredicado, string pObjeto, short pPrioridad)
        {
            InsertaTripleta(pProyectoID, pSujeto, pPredicado, pObjeto, pPrioridad, false);
        }

        /// <summary>
        /// Inserta una tripleta
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pSujeto">Sujeto</param>
        /// <param name="pPredicado">Predicado</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pPrioridad">Prioridad que se le va a dar a la replicación de esta transacción</param>
        public void InsertaTripleta(string pProyectoID, string pSujeto, string pPredicado, string pObjeto, short pPrioridad, bool pUsarColaActualizacion)
        {
            pObjeto = pObjeto.Replace("\\", "\\\\");

            //Si contiene comillas entre medio, se las remplazo por \"
            if ((pObjeto.Length > 2) && (pObjeto.StartsWith('\"')) && (pObjeto.EndsWith('\"')) && (pObjeto.Substring(1, pObjeto.Length - 2).Contains("\"")))
            {
                pObjeto = "\"" + pObjeto.Substring(1, pObjeto.Length - 2).Replace("\"", "\\\"") + "\"";
            }
            string query = $"SPARQL INSERT INTO {ObtenerUrlGrafo(pProyectoID)} {{ {pSujeto} {pPredicado} {pObjeto} }} ";

            ActualizarVirtuoso(query, pProyectoID, pPrioridad);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="ptripletas"></param>
        /// <param name="pListaElementosaModificarID">IMPORTANTE: Aunque se pasa un diccionario con sujeto/predicado, a la hora de borrar borra lo que tenga el sujeto</param>
        /// <param name="pEliminar"></param>
        /// <param name="pPrioridad"></param>
        public void InsertaTripletasConModify(string pProyectoID, string ptripletas, Dictionary<string, string> pListaElementosaModificarID, bool pEliminar, PrioridadBase pPrioridad)
        {
            InsertaTripletasConModify(pProyectoID, ptripletas, pListaElementosaModificarID, "", "", pEliminar, pPrioridad);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="ptripletas"></param>
        /// <param name="pListaElementosaModificarID">IMPORTANTE: Aunque se pasa un diccionario con sujeto/predicado, a la hora de borrar borra lo que tenga el sujeto</param>
        public void InsertaTripletasConModify(string pProyectoID, string ptripletas, Dictionary<string, string> pListaElementosaModificarID)
        {
            InsertaTripletasConModify(pProyectoID, ptripletas, pListaElementosaModificarID, "", "", true, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="ptripletas"></param>
        /// <param name="pListaElementosaModificarID">IMPORTANTE: Aunque se pasa un diccionario con sujeto/predicado, a la hora de borrar borra lo que tenga el sujeto</param>
        /// <param name="pCondicionesWhere"></param>
        /// <param name="pCondicionesFilter"></param>
        public void InsertaTripletasConModify(string pProyectoID, string ptripletas, Dictionary<string, string> pListaElementosaModificarID, string pCondicionesWhere, string pCondicionesFilter)
        {
            InsertaTripletasConModify(pProyectoID, ptripletas, pListaElementosaModificarID, pCondicionesWhere, pCondicionesFilter, true, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="ptripletas"></param>
        /// <param name="pListaElementosaModificarID">IMPORTANTE: Aunque se pasa un diccionario con sujeto/predicado, a la hora de borrar borra lo que tenga el sujeto</param>
        /// <param name="pCondicionesWhere"></param>
        /// <param name="pCondicionesFilter"></param>
        /// <param name="pEliminar"></param>
        /// <param name="pPrioridad"></param>
        public void InsertaTripletasConModify(string pProyectoID, string ptripletas, Dictionary<string, string> pListaElementosaModificarID, string pCondicionesWhere, string pCondicionesFilter, bool pEliminar, PrioridadBase pPrioridad)
        {
            InsertaTripletasConModify(pProyectoID, ptripletas, pListaElementosaModificarID, pCondicionesWhere, pCondicionesFilter, pEliminar, pPrioridad, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="ptripletas"></param>
        /// <param name="pListaElementosaModificarID">IMPORTANTE: Aunque se pasa un diccionario con sujeto/predicado, a la hora de borrar borra lo que tenga el sujeto</param>
        /// <param name="pCondicionesWhere"></param>
        /// <param name="pCondicionesFilter"></param>
        /// <param name="pEliminarRecursoSiNecesario"></param>
        /// <param name="pPrioridad"></param>
        /// <param name="pBorrarAuxiliar"></param>
        public void InsertaTripletasConModify(string pProyectoID, string ptripletas, Dictionary<string, string> pListaElementosaModificarID, string pCondicionesWhere, string pCondicionesFilter, bool pEliminarRecursoSiNecesario, PrioridadBase pPrioridad, bool pBorrarAuxiliar)
        {
            if ((pListaElementosaModificarID == null || pListaElementosaModificarID.Count == 0) && string.IsNullOrEmpty(pCondicionesWhere) && string.IsNullOrEmpty(pCondicionesFilter))
            {
                throw new ExcepcionGeneral("No se puede llamar a InsertaTripletasConModify sin filtros porque borraría todo el Grafo");
            }

            string sujetoExtraBorrar = null;

            string consulta = $"{NamespacesVirtuosoLectura} ASK FROM {ObtenerUrlGrafo(pProyectoID)}";
            string inicioWhere = $" WHERE {{?s ?p ?o. {pCondicionesWhere}";
            StringBuilder whereSinPredicado = new StringBuilder(inicioWhere);
            StringBuilder whereConPredicado = new StringBuilder(inicioWhere);

            if (pListaElementosaModificarID != null && (pListaElementosaModificarID.Count > 0 || !string.IsNullOrEmpty(pCondicionesFilter)))
            {
                StringBuilder filtro = new StringBuilder(" FILTER( ");
                if (!string.IsNullOrEmpty(pCondicionesFilter))
                {
                    filtro.Append(pCondicionesFilter);
                }

                if (!string.IsNullOrEmpty(pCondicionesFilter) && pListaElementosaModificarID.Count > 0)
                {
                    filtro.Append(" AND ");
                }

                if (pListaElementosaModificarID.Count > 0)
                {
                    if (pListaElementosaModificarID.Count == 1)
                    {
                        filtro.Append(" ?s = ");
                    }
                    else
                    {
                        filtro.Append(" ?s in (");
                    }

                    StringBuilder predicado = new StringBuilder();

                    string coma = "";
                    foreach (string key in pListaElementosaModificarID.Keys)
                    {
                        filtro.Append($"{coma}gnoss:{key.ToUpper()}");

                        if (!string.IsNullOrEmpty(pListaElementosaModificarID[key]))
                        {
                            Guid idKey;
                            if (pBorrarAuxiliar && Guid.TryParse(key, out idKey))
                            {
                                sujetoExtraBorrar = $"<http://gnossAuxiliar/{idKey.ToString().ToUpper()}>";
                            }

                            //Tengo un where con los predicados y otro sin ellos, añado lo que hay ahora mismo en la variable filtro y limpio esa variable
                            whereSinPredicado.Append(filtro);
                            whereConPredicado.Append(filtro);
                            predicado.Append($" AND ?p = {pListaElementosaModificarID[key]}");
                            filtro = new StringBuilder();
                        }
                        coma = ",";
                    }

                    if (pListaElementosaModificarID.Count > 1)
                    {
                        filtro.Append(" ) ");
                    }

                    if (predicado.Length > 0)
                    {
                        whereConPredicado.Append(predicado);
                    }
                }
                filtro.Append(" ) ");

                // Añado lo que queda en la variable filtro a los dos where
                whereSinPredicado.Append(filtro);
                whereConPredicado.Append(filtro);
            }
            whereSinPredicado.Append(" } ");
            whereConPredicado.Append(" } ");

            FacetadoDS facetadoDS = new FacetadoDS();
            // Aquí consulto con el predicado, si existe, hay que hacer un modify

            //El update fallaría si el número de triples es superior a 1400 o supera 10Mb de datos, se lanza un delete y un insert
            bool necesarioEliminar = (ptripletas.Count(caracter => caracter.Equals('\n')) > 1400 || (ptripletas.Length / (1024 * 1024) > 10));

            LeerDeVirtuoso(consulta + whereConPredicado, "prueba", facetadoDS, pProyectoID);
            if (!necesarioEliminar && facetadoDS.Tables["prueba"].Rows.Count > 0 && facetadoDS.Tables["prueba"].Rows[0][0].Equals("1"))
            {
                bool transaccionIniciada = false;
                try
                {
                    if (sujetoExtraBorrar != null)
                    {
                        UsarClienteTradicional = true;
                        transaccionIniciada = IniciarTransaccion();
                    }

                    bool usarInsert = false;
                    if (sujetoExtraBorrar != null)
                    {
                        string delete = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pProyectoID)} {{ ?s ?p ?o. ?sujAux ?predAux ?s. }} {ObtenerFrom(pProyectoID)} WHERE {{?sujAux ?predAux ?s. FILTER(?sujAux = {sujetoExtraBorrar}) ?s ?p ?o. }} ";

                        ActualizarVirtuoso(delete, pProyectoID, (short)pPrioridad);

                        // Es probable que hayamos borrado todos los triples del recurso. Compruebo si es así para hacer un Insert o un Modify
                        facetadoDS.Clear();
                        if (UsarClienteTradicional)
                        {
                            LeerDeVirtuosoClienteTradicional(consulta + whereConPredicado, "prueba", facetadoDS, pProyectoID);
                        }
                        else
                        {
                            LeerDeVirtuoso(consulta + whereConPredicado, "prueba", facetadoDS, pProyectoID);
                        }

                        if (facetadoDS.Tables["prueba"].Rows.Count == 0)
                        {
                            usarInsert = true;
                        }
                    }

                    int resultado = 0;
                    if (usarInsert)
                    {
                        resultado = InsertaTripletas(pProyectoID, ptripletas, (short)pPrioridad);
                    }
                    else
                    {
                        string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{ {ptripletas} }} FROM {ObtenerUrlGrafo(pProyectoID)} {whereSinPredicado}";
                        resultado = ActualizarVirtuoso(modify, pProyectoID, (short)pPrioridad);
                    }

                    if (resultado == 0)
                    {
                        // El modify no ha insertado ningún triple, probablemente porque al usar el cliente tradicional hay algún carácter que al pasarlo a UTF-8 no se ha quedado bien (ej: “”)
                        // Primero hacemos rollback de las instrucciones
                        TerminarTransaccion(false);
                        UsarClienteTradicional = false;
                        transaccionIniciada = false;

                        if (sujetoExtraBorrar != null)
                        {
                            string deleteAux = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pProyectoID)} {{ ?s ?p ?o. ?sujAux ?predAux ?s. }} {ObtenerFrom(pProyectoID)} WHERE {{?sujAux ?predAux ?s. FILTER(?sujAux = {sujetoExtraBorrar}) ?s ?p ?o. }} ";
                            ActualizarVirtuoso(deleteAux, pProyectoID, (short)pPrioridad);
                        }

                        // Hacemos el DELETE - INSERT por el interfaz HTTP POST
                        string delete = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pProyectoID)} {{ ?s ?p ?o. }} {ObtenerFrom(pProyectoID)} {whereSinPredicado}";
                        ActualizarVirtuoso(delete, pProyectoID, (short)pPrioridad);
                        InsertaTripletas(pProyectoID, ptripletas, (short)pPrioridad);
                    }

                    if (transaccionIniciada)
                    {
                        TerminarTransaccion(true);
                    }
                }
                catch
                {
                    if (transaccionIniciada)
                    {
                        TerminarTransaccion(false);
                    }
                    throw;
                }
                finally
                {
                    UsarClienteTradicional = false;
                }
            }
            else
            {
                bool insertado = false;

                if (necesarioEliminar)
                {
                    //No existe ningún sujeto con ese predicado, compruebo si existe el sujeto
                    LeerDeVirtuoso(consulta + whereSinPredicado, "prueba2", facetadoDS, pProyectoID);
                    if (facetadoDS.Tables["prueba2"].Rows.Count > 0 && facetadoDS.Tables["prueba2"].Rows[0][0].Equals("1"))
                    {
                        bool transaccionIniciada = false;
                        UsarClienteTradicional = true;
                        try
                        {
                            transaccionIniciada = IniciarTransaccion();

                            if (sujetoExtraBorrar != null)
                            {
                                string deleteAux = $"{NamespacesVrituosoEscritura} DELETE {ObtenerFromGraph(pProyectoID)} {{ ?s ?p ?o. ?sujAux ?predAux ?s. }} {ObtenerFrom(pProyectoID)} WHERE {{?sujAux ?predAux ?s. FILTER(?sujAux = {sujetoExtraBorrar}) ?s ?p ?o. }} ";
                                ActualizarVirtuoso(deleteAux, pProyectoID, (short)pPrioridad);
                            }

                            string delete = $"{NamespacesVrituosoEscritura} DELETE from {ObtenerUrlGrafo(pProyectoID)} {{ ?s ?p ?o }} from {ObtenerUrlGrafo(pProyectoID)} {whereSinPredicado}";
                            ActualizarVirtuoso(delete, pProyectoID, (short)pPrioridad);
                            InsertaTripletas(pProyectoID, ptripletas, (short)pPrioridad);

                            insertado = true;
                            if (transaccionIniciada)
                            {
                                TerminarTransaccion(true);
                            }
                        }
                        catch
                        {
                            if (transaccionIniciada)
                            {
                                TerminarTransaccion(false);
                            }
                            throw;
                        }
                        finally
                        {
                            UsarClienteTradicional = false;
                        }
                    }
                }

                if (!insertado)
                {
                    //Inserto las tripletas
                    InsertaTripletas(pProyectoID, ptripletas, (short)pPrioridad);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <param name="pTripletas"></param>
        /// <param name="pElementoaEliminarID"></param>
        /// <param name="pNombreGrafo"></param>
        /// <param name="pPrioridad"></param>
        /// <param name="pUrlIntragnoss"></param>
        /// <param name="pEscribirNT"></param>
        /// <param name="pFicheroConfiguracion"></param>
        /// <param name="pInfoExtraReplicacion"></param>
        /// <param name="pUsarColareplicacion"></param>
        public void InsertaTripletasRecursoSemanticoConModify(string pNombreGrafo, string pUrlIntragnoss, string pFicheroConfiguracion, Guid pProyectoID, string pInfoExtraReplicacion, bool pUsarColareplicacion, string pElementoaEliminarID, string pTripletas, short pPrioridad, bool pEscribirNT)
        {
            if (string.IsNullOrEmpty(pElementoaEliminarID))
            {
                throw new ExcepcionGeneral("No se puede llamar a InsertaTripletasRecursoSemanticoConModify sin filtros porque borraría todo el Grafo");
            }

            if (pNombreGrafo != null)
            {
                pNombreGrafo = pNombreGrafo.ToLower();
            }

            string consulta = $"{NamespacesVirtuosoLectura} ASK {ObtenerFromGraph(pNombreGrafo)}";

            string inicioWhere = $" WHERE {{?documento ?hasEntidad ?s. FILTER (?documento = <{mUrlIntranet}{pElementoaEliminarID.ToLower()}>) ?s ?p ?o }} ";
            string whereSinPredicado = inicioWhere;
            string whereConPredicado = inicioWhere;

            FacetadoDS facetadoDS = new FacetadoDS();
            // Aquí consulto con el predicado, si existe, hay que hacer un modify
            LeerDeVirtuoso(consulta + whereConPredicado, "prueba", facetadoDS, pNombreGrafo, true);

            if (facetadoDS.Tables["prueba"].Rows.Count > 0 && facetadoDS.Tables["prueba"].Rows[0][0].Equals("1"))
            {
                IniciarTransaccion(); //esto solo funciona cuando la propiedad UsarClienteTradicional == true
                try
                {
                    string modify = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {ObtenerUrlGrafo(pNombreGrafo)} DELETE {{ ?s ?p ?o }} INSERT {{ {pTripletas} }} FROM {ObtenerUrlGrafo(pNombreGrafo)} {whereSinPredicado}";
                    ActualizarVirtuoso(modify, pNombreGrafo, pPrioridad);
                    TerminarTransaccion(true); //esto solo funciona cuando la propiedad UsarClienteTradicional == true
                }
                catch
                {
                    TerminarTransaccion(false);
                    throw;
                }
            }
            else
            {
                //Inserto las tripletas
                InsertaTripletas(pNombreGrafo, pTripletas, 0, pUsarColareplicacion, pEscribirNT, pInfoExtraReplicacion);
            }
        }

        public void InsertarTriplesEdicionTagsCategoriasSearchRecurso(string pDocumentoID, string pProyectoID, string pTripletas, PrioridadBase pPrioridad, bool pEliminadoTags, bool pEliminandoCategorias)
        {
            string consulta = $"{NamespacesVirtuosoLectura} ASK FROM {ObtenerUrlGrafo(pProyectoID)}";
            string sujeto = $"<http://gnoss/{pDocumentoID.ToUpper()}>";
            string predicadoSearch = "<http://gnoss/search>";
            string predicadoTags = "<http://rdfs.org/sioc/types#Tag>";
            string predicadoCategorias = "<http://www.w3.org/2004/02/skos/core#ConceptID>";
            string inicioWhere = " WHERE {?s ?p ?o. ";
            string whereConSearch = $"{inicioWhere} filter(?s = {sujeto} and ?p in({predicadoSearch}";

            if (pEliminadoTags)
            {
                whereConSearch += $", {predicadoTags}";
            }

            if (pEliminandoCategorias)
            {
                whereConSearch += $", {predicadoCategorias}";
            }

            whereConSearch += "))}";
            string whereSinSearch = $"{inicioWhere} filter(?s = {sujeto}) }}";

            FacetadoDS facetadoDS = new FacetadoDS();
            LeerDeVirtuoso(consulta + whereConSearch, "prueba", facetadoDS, pProyectoID);
            if (facetadoDS.Tables["prueba"].Rows.Count > 0 && facetadoDS.Tables["prueba"].Rows[0][0].Equals("1"))
            {
                IniciarTransaccion();
                try
                {
                    string consultaBorradoEInsercion = $"sparql MODIFY GRAPH {ObtenerUrlGrafo(pProyectoID)} DELETE {{ ?s ?p ?o }} INSERT {{{pTripletas}}} {whereConSearch}";
                    ActualizarVirtuoso(consultaBorradoEInsercion, pProyectoID, (short)pPrioridad);
                    TerminarTransaccion(true);
                }
                catch
                {
                    TerminarTransaccion(false);
                    throw;
                }
            }
            else
            {
                //No existe ningún sujeto con ese predicado, compruebo si existe el sujeto
                LeerDeVirtuoso(consulta + whereSinSearch, "prueba2", facetadoDS, pProyectoID);
                if (facetadoDS.Tables["prueba2"].Rows.Count > 0 && facetadoDS.Tables["prueba2"].Rows[0][0].Equals("1"))
                {
                    IniciarTransaccion();
                    try
                    {
                        string consultaSoloInsercion = $"sparql INSERT INTO {ObtenerUrlGrafo(pProyectoID).ToLower()} {{{pTripletas}}}";
                        ActualizarVirtuoso(consultaSoloInsercion, pProyectoID, (short)pPrioridad);
                        TerminarTransaccion(true);
                    }
                    catch
                    {
                        TerminarTransaccion(false);
                        throw;
                    }
                }
            }
        }

        public string ObtenerGrafoEliminarSujeto(string pRecursoID)
        {
            DataSet facDS = ObtenerGrafo(pRecursoID);
            if (facDS != null && facDS.Tables["Grafo"].Rows.Count > 0)
            {
                return facDS.Tables["Grafo"].Rows[0].ItemArray[0].ToString();
            }

            return "";
        }

        private DataSet ObtenerGrafo(string pRecursoID)
        {
            string query = $"{NamespacesVirtuosoLectura} select distinct ?g WHERE {{graph ?g {{?documento ?hasEntidad ?s.  FILTER (?documento = <{mUrlIntranet}{pRecursoID.ToLower()}>) ?s ?p ?o }}}}";

            return LeerDeVirtuoso(query, "Grafo", null);
        }

        /// <summary>
        /// Escribe fisicamente las entradas en el log
        /// </summary>
        /// <param name="pInfoEntry"></param>
        public void GuardarTriples(string pTriples, string pNombreLog)
        {
            if (pTriples != string.Empty)
            {
                string rutaFichero = $"{DirectorioEscrituraNT}{Path.DirectorySeparatorChar}{pNombreLog}.nt";

                if (!Directory.Exists(DirectorioEscrituraNT))
                {
                    Directory.CreateDirectory(DirectorioEscrituraNT);
                }

                int count = 1;
                string rutaSiguiente = rutaFichero.Replace(".nt", $"_{count}.nt");

                while (File.Exists(rutaSiguiente))
                {
                    count++;
                    rutaFichero = rutaSiguiente;
                    rutaSiguiente = rutaSiguiente.Replace($"_{(count - 1)}.nt", $"_{count}.nt");
                }

                //Si el fichero supera el tamaño máximo lo elimino
                if (File.Exists(rutaFichero))
                {
                    FileInfo fichero = new FileInfo(rutaFichero);
                    if (fichero.Length > 100000000)
                    {
                        rutaFichero = rutaSiguiente;
                    }
                }

                //Añado el error al fichero
                using (StreamWriter sw = new StreamWriter(rutaFichero, true, UtilCadenas.EncodingANSI))
                {
                    sw.Write(pTriples);
                }
            }
        }

        public int InsertaTripletas(string pProyectoID, string ptripletas, short pPrioridad)
        {
            return InsertaTripletas(pProyectoID, ptripletas, pPrioridad, false);
        }

        public int InsertaTripletas(string pProyectoID, string ptripletas, short pPrioridad, bool pUsarColaActualizacion)
        {
            return InsertaTripletas(pProyectoID, ptripletas, pPrioridad, pUsarColaActualizacion, false, "");
        }

        public int InsertaTripletas(string pProyectoID, string ptripletas, short pPrioridad, bool pUsarColaActualizacion, bool pEscribirNT, string pInfoExtra)
        {
            if (string.IsNullOrEmpty(ptripletas))
            {
                return 0;
            }

            string query = $"SPARQL INSERT INTO {ObtenerUrlGrafo(pProyectoID).ToLower()} {{ {ptripletas} }} ";
            int resultado = 0;

            if (pEscribirNT)
            {
                GuardarTriples(ptripletas, pProyectoID);
            }
            else
            {
                resultado = ActualizarVirtuoso(query, pProyectoID, pPrioridad);
            }
            return resultado;
        }

        public string ObtieneTripletasFormularios(FacetadoDS facetadoDS, string idproyecto, string iddoc, List<string> pListRdfTypePadre)
        {
            string nobreontologia = mEntityContext.Documento.JoinDocumentoVinc().Where(x => x.DocumentoVinc.DocumentoID.Equals(Guid.Parse(iddoc))).Select(x => x.Documento.Enlace).FirstOrDefault();

            string query = $"{NamespacesVirtuosoLectura} select ?s ?p ?o lang(?o) as ?idioma {ObtenerFrom(nobreontologia.ToLower())} WHERE {{ ?documento ?hasEntidad ?s. FILTER (?documento = <{mUrlIntranet}{iddoc.ToLower()}>) ?s ?p ?o }} ";

            LeerDeVirtuoso(query, "FormulariosSemanticosPadre", facetadoDS, nobreontologia);

            DataTable tabla = new DataTable("FormulariosSemanticosHijo");
            tabla.Columns.Add("s");
            tabla.Columns.Add("p");
            tabla.Columns.Add("o");
            tabla.Columns.Add("idioma");
            facetadoDS.Tables.Add(tabla);

            string sujetoPadre = string.Empty;

            string rdfTypePadre = pListRdfTypePadre.Find(item => facetadoDS.Tables["FormulariosSemanticosPadre"].Select($"p = 'http://www.w3.org/1999/02/22-rdf-syntax-ns#type' AND o = '{item}'").Any());

            if (!string.IsNullOrEmpty(rdfTypePadre))
            {
                sujetoPadre = facetadoDS.Tables["FormulariosSemanticosPadre"].Select($"p = 'http://www.w3.org/1999/02/22-rdf-syntax-ns#type' AND o = '{rdfTypePadre}'")[0][0].ToString();
            }

            List<DataRow> hijos = new List<DataRow>();
            foreach (DataRow fila in facetadoDS.Tables["FormulariosSemanticosPadre"].Select($"s <> '{sujetoPadre}'"))
            {
                hijos.Add(fila);
                facetadoDS.Tables["FormulariosSemanticosHijo"].ImportRow(fila);
            }

            foreach (DataRow fila in hijos)
            {
                fila.Delete();
            }
            facetadoDS.AcceptChanges();

            return nobreontologia;
        }

        public void ObtieneTripletasFormulariosviejo(ref string mTripletas, ref string mTripletasGnoss, DataWrapperFacetas pFacetaDW, FacetadoDS facetadoDS, string pOrganizacionID, string idproyecto, string iddoc)
        {
            pFacetaDW.ListaOntologiaProyecto = mEntityContext.OntologiaProyecto.Where(item => item.OrganizacionID.Equals(pOrganizacionID) && item.ProyectoID.Equals(idproyecto)).ToList();

            foreach (OntologiaProyecto myrow in pFacetaDW.ListaOntologiaProyecto)
            {
                string query = $"{NamespacesVirtuosoLectura} select * {ObtenerFrom($"{myrow.OntologiaProyecto1}.owl")} WHERE {{?s ?p ?o. FILTER (?s LIKE '%{iddoc.ToLower()}_%')}}";
                LeerDeVirtuoso(query, "FormulariosSemanticos", facetadoDS, idproyecto);
                if (facetadoDS.Tables["FormulariosSemanticos"].Rows.Count > 0)
                {
                    break;
                }
            }
        }

        public void ObtieneTripletasFormulariosCV(FacetadoDS facetadoDS, string idproyecto, string iddoc)
        {
            string query = $"{NamespacesVirtuosoLectura} select * {ObtenerFrom("curriculum.owl")} WHERE {{?s ?p ?o. FILTER (?s LIKE '%{iddoc.ToLower()}_%')}}";

            LeerDeVirtuoso(query, "FormulariosSemanticos", facetadoDS, idproyecto);
        }

        /// <summary>
        /// Obtiene las triplas de un ELEMENTO cualquiera
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pDocumentoID">ID del elemento</param>
        /// <returns>DataSet con una tabla con las triplas</returns>
        public DataSet ObtieneTripletasRecursoEnGrafo(string pGrafo, Guid pElementoID)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} select ?s ?p ?o lang(?o) as ?idioma {ObtenerFrom(pGrafo.ToLower())} WHERE  {{ ?s ?p ?o.  FILTER (?s = <http://gnoss/{pElementoID.ToString().ToUpper()}>)}} ";

            LeerDeVirtuoso(query, "TripleDoc", facetadoDS, pGrafo);
            return facetadoDS;
        }

        /// <summary>
        /// Obtiene las triplas de un ELEMENTO
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pSujeto">Sujeto para hacer la petición</param>
        /// <returns>DataSet con una tabla con las triplas</returns>
        public DataSet ObtieneTripletasRecursoEspecificoEnGrafo(string pGrafo, string pSujeto)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"{NamespacesVirtuosoLectura} select ?s ?p ?o lang(?o) as ?idioma {ObtenerFrom(pGrafo.ToLower())} WHERE {{ ?s ?p ?o.  FILTER (?s = <{pSujeto}>) }} ";

            LeerDeVirtuoso(query, "TripleEsp", facetadoDS, pGrafo);
            return facetadoDS;
        }

        /// <summary>
        /// Obtiene el objeto de las triplas de un documento en un grafo filtrado por una faceta (puede ser jerárquica).
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pDocumentoID">ID de documento</param>
        /// <param name="pFaceta">Faceta</param>
        /// <returns>DataSet con una tabla con las triplas</returns>
        public DataSet ObtenerValorFacetaDocumentoEnGrafo(string pGrafo, Guid pDocumentoID, string pFaceta)
        {
            FacetadoDS facetadoDS = new FacetadoDS();

            StringBuilder query = new StringBuilder($"{ObtenerFrom(pGrafo.ToLower())} WHERE {{ ?s");

            int count = 0;
            foreach (string faceta in pFaceta.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries))
            {
                query.Append($" {faceta} ?o{count}. ?o{count}");
                count++;
            }

            int caracteresEliminar = query.Length - 2 - count.ToString().Length;
            query.Remove(caracteresEliminar, query.Length - caracteresEliminar);
            query.Append($" FILTER (?s = <http://gnoss/{pDocumentoID.ToString().ToUpper()}>) }}");
            query.Insert(0, $"{NamespacesVirtuosoLectura} select ?o{count - 1} lang(?o{count - 1}) as ?idioma ");
                      
            LeerDeVirtuoso(query.ToString(), "TripleDoc", facetadoDS, pGrafo);
            return facetadoDS;
        }

        public void ActualizarPublicadoresRecursos(string pUrlIntragnoss, Guid pGrafoID, Guid pIdentidadID, string pNombrePublicador)
        {
            ActualizarPublicadoresRecursos(pUrlIntragnoss, true, pGrafoID, pIdentidadID, pNombrePublicador);
        }

        public void ActualizarPublicadoresRecursos(string pUrlIntragnoss, bool pUsarColaActualizacion, Guid pGrafoID, Guid pIdentidadID, string pNombrePublicador)
        {
            string nombrePublicadorComillas = pNombrePublicador.Replace("\\", "\\\\");

            nombrePublicadorComillas = nombrePublicadorComillas.ToLower().Replace("\"", "\\\"");

            string query = $"{NamespacesVrituosoEscritura} MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{?s ?p ?o. }} INSERT {{ ?s ?p \"{nombrePublicadorComillas}\". }}  WHERE {{?s ?p ?o. ?s ?p2 ?o2. FILTER(?p2 = <http://gnoss/haspublicadorIdentidadID> AND ?o2 = <http://gnoss/{pIdentidadID.ToString().ToUpper()}> AND ?p = <http://gnoss/haspublicador>) }}";

            ActualizarVirtuoso(query, pGrafoID.ToString());
        }

        public void ActualizarEditoresRecursos(string pUrlIntragnoss, Guid pGrafoID, Guid pIdentidadID, string pNombreEditor, List<Guid> pListaRecursos)
        {
            ActualizarEditoresRecursos(pUrlIntragnoss, true, pGrafoID, pIdentidadID, pNombreEditor, pListaRecursos);
        }

        public void ActualizarEditoresRecursos(string pUrlIntragnoss, bool pUsarColaActualizacion, Guid pGrafoID, Guid pIdentidadID, string pNombreEditor, List<Guid> pListaRecursos)
        {
            string nombreEditorComillas = pNombreEditor.Replace("\\", "\\\\");
            nombreEditorComillas = nombreEditorComillas.ToLower().Replace("\"", "\\\"");

            StringBuilder query = new StringBuilder($"{NamespacesVrituosoEscritura} MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{ ?s ?p ?o. }} INSERT {{ ?s ?p \"{nombreEditorComillas}\". }} WHERE {{ ?s ?p ?o. ?s ?p2 ?o2. FILTER(?p2 = <http://gnoss/haseditorIdentidadID> AND ?o2 = <http://gnoss/{pIdentidadID.ToString().ToUpper()}> AND ?p = <http://gnoss/haseditor> ");

            if (pListaRecursos != null && pListaRecursos.Count > 0)
            {
                query.Append(" AND ?s IN(");
                foreach (Guid sujeto in pListaRecursos)
                {
                    query.Append($" <http://gnoss/{sujeto.ToString().ToUpper()}> ,");
                }

                if (query[query.Length - 1] == ',')
                {
                    query = query.Remove(query.Length - 1, 1);
                }
                query.Append(")");
            }
            query.Append(") }");

            ActualizarVirtuoso(query.ToString(), pGrafoID.ToString());
        }

        public void ActualizarGrupoEditorRecursos(bool pUsarColaActualizacion, Guid pGrafoID, string pNombreGrupoViejo, string pNombreGrupoNuevo)
        {
            string nombreGrupoNuevoComillas = pNombreGrupoNuevo.Replace("\\", "\\\\");
            string nombreGrupoViejoComillas = pNombreGrupoViejo.Replace("\\", "\\\\");

            nombreGrupoNuevoComillas = nombreGrupoNuevoComillas.ToLower().Replace("\"", "\\\"");
            nombreGrupoViejoComillas = nombreGrupoViejoComillas.ToLower().Replace("\"", "\\\"");

            string query = $"{NamespacesVrituosoEscritura} MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{ ?s <http://gnoss/hasgrupoEditor> ?o. }} INSERT {{ ?s <http://gnoss/hasgrupoEditor> \"{nombreGrupoNuevoComillas}\". }} WHERE {{ ?s <http://gnoss/hasgrupoEditor> ?o. FILTER(?o = \"{nombreGrupoViejoComillas}\") }}";

            ActualizarVirtuoso(query, pGrafoID.ToString());
        }

        public void ActualizarGrupoLectorRecursos(bool pUsarColaActualizacion, Guid pGrafoID, string pNombreGrupoViejo, string pNombreGrupoNuevo)
        {
            string nombreGrupoNuevoComillas = pNombreGrupoNuevo.Replace("\\", "\\\\");
            string nombreGrupoViejoComillas = pNombreGrupoViejo.Replace("\\", "\\\\");

            nombreGrupoNuevoComillas = nombreGrupoNuevoComillas.ToLower().Replace("\"", "\\\"");
            nombreGrupoViejoComillas = nombreGrupoViejoComillas.ToLower().Replace("\"", "\\\"");

            string query = $"{NamespacesVrituosoEscritura} MODIFY GRAPH <{mUrlIntranet}{pGrafoID}> DELETE {{ ?s <http://gnoss/hasgrupoLector> ?o. }} INSERT {{ ?s <http://gnoss/hasgrupoLector> \"{nombreGrupoNuevoComillas}\". }} WHERE {{ ?s <http://gnoss/hasgrupoLector> ?o. FILTER(?o = \"{nombreGrupoViejoComillas}\") }}";

            ActualizarVirtuoso(query, pGrafoID.ToString());
        }

        /// <summary>
        /// Obtenemos una consulta paginada a partir de una consulta inicial. La paginación se hará
        /// de 10000 en 10000 entradas. La paginación dependerá del número de iteraciones que se pase por parámetro
        /// </summary>
        /// <param name="pInitialQuery">Consulta principal que queremos paginar</param>
        /// <param name="pIterator">Numero de iteraciones que llevamos pasadas</param>
        /// <param name="pProyectId">Identificador del proyecto</param>
        /// <returns></returns>
        private string GetPaginatedQueryFromQuery(string pInitialQuery, int pIterator, string pProyectId)
        {
            string subQuery = $"select * {ObtenerFrom(pProyectId)} {{{{ ";
            string queryLimitOffset = $"}}}} offset {pIterator * 10000} limit 10000";
            string prefix = pInitialQuery.Substring(0, pInitialQuery.IndexOf("select"));
            string principalQuery = pInitialQuery.Substring(pInitialQuery.IndexOf("select"));

            return prefix + subQuery + principalQuery + queryLimitOffset;
        }

        public void ModificarEstadoDeContenido(Guid pProyectoID, Guid pEstadoOrigen, Guid pEstadoDestino, Guid pContenidoID)
        {
			string grafo = ObtenerUrlGrafo(pProyectoID.ToString());
			string tripleEliminar = $"<http://gnoss/{pContenidoID.ToString().ToUpper()}> <http://gnoss/hasWorkflowState> <http://gnoss/{pEstadoOrigen.ToString().ToUpper()}>";
            string tripleInsertar = $"<http://gnoss/{pContenidoID.ToString().ToUpper()}> <http://gnoss/hasWorkflowState> <http://gnoss/{pEstadoDestino.ToString().ToUpper()}>";            
			string query = $"{NamespacesVrituosoEscritura} MODIFY GRAPH {grafo} DELETE {{ {tripleEliminar} . }} INSERT {{ {tripleInsertar} . }}";

            ActualizarVirtuoso(query, pProyectoID.ToString());
		}

		public void EliminarEstadoDeContenido(Guid pProyectoID, Guid pEstadoID, Guid pContenidoID)
		{
			string grafo = ObtenerUrlGrafo(pProyectoID.ToString());
			string tripleEliminar = $"<http://gnoss/{pContenidoID.ToString().ToUpper()}> <http://gnoss/hasWorkflowState> <http://gnoss/{pEstadoID.ToString().ToUpper()}>";

			string query = $"{NamespacesVrituosoEscritura} DELETE FROM GRAPH {grafo} {{ {tripleEliminar} . }}";

			ActualizarVirtuoso(query, pProyectoID.ToString());
		}

        public void AnyadirEstadoDeContenido(Guid pProyectoID, Guid pEstadoID, Guid pContenidoID)
        {
			string grafo = ObtenerUrlGrafo(pProyectoID.ToString());
			string tripleInsertar = $"<http://gnoss/{pContenidoID.ToString().ToUpper()}> <http://gnoss/hasWorkflowState> <http://gnoss/{pEstadoID.ToString().ToUpper()}> .";
			string query = $"SPARQL INSERT INTO {grafo} {{ {tripleInsertar} }}";

			ActualizarVirtuoso(query, pProyectoID.ToString());
		}

		public void InsertarTripleRdfTypeDeContenido(Guid pProyectoID, Guid pContenidoID, string pRdfType)
        {
			string grafo = ObtenerUrlGrafo(pProyectoID.ToString());
            string tripleInsertar = $"<http://gnoss/{pContenidoID.ToString().ToUpper()}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \"{pRdfType}\" .";
            string query = $"SPARQL INSERT INTO {grafo} {{ {tripleInsertar} }}";

            ActualizarVirtuoso(query, pProyectoID.ToString());
		}

        public void EliminarTripleRdfTypeDeContenido(Guid pProyectoID, Guid pContenidoID, string pRdfType)
        {
			string grafo = ObtenerUrlGrafo(pProyectoID.ToString());
			string tripleEliminar = $"<http://gnoss/{pContenidoID.ToString().ToUpper()}> <http://www.w3.org/1999/02/22-rdf-syntax-ns#type> \"{pRdfType}\" .";
			string query = $"{NamespacesVrituosoEscritura} DELETE FROM GRAPH {grafo} {{ {tripleEliminar} }}";

			ActualizarVirtuoso(query, pProyectoID.ToString());
		}

		#endregion

		#region Tesauro Semántico

		/// <summary>
		/// Obtiene las tripletas de un sujeto que tiene como objeto uno específico.
		/// </summary>
		/// <param name="pGrafo">Grafo del Tesauro Semántico</param>
		/// <param name="pObjeto">Objeto</param>
		/// <returns>Tripletas de un sujeto que tiene como objeto uno específico</returns>
		public FacetadoDS ObtenerTripletasDeSujetoConObjeto(string pGrafo, string pObjeto)
        {
            FacetadoDS dataSet = new FacetadoDS();

            string query = $"{NamespacesVirtuosoLectura} select ?s ?p ?o ";

            string filtro = pObjeto;

            if (filtro.StartsWith("http"))
            {
                filtro = $"<{filtro}>";
            }
            else
            {
                filtro = $"'{filtro}'";
            }

            query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{ ?s ?p ?o. ?s ?p2 ?o2. FILTER (?o2={filtro}) }}";

            LeerDeVirtuoso(query, "SelectPropEnt", dataSet, pGrafo);

            return dataSet;
        }

        /// <summary>
        /// Obtiene una lista de sujetos del del tesauro según el grafo seleccionado y source indicado por parámetro.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pSource">Source</param>
        /// <returns>Lista de sujetos del tesauro segun el grafo y el source indicado</returns>
        public List<string> ObtenerListaSujetosTesauroDeGrafoPorSource(string pGrafo, string pSource)
        {
            FacetadoDS dataSet = new FacetadoDS();

            string query = $"{NamespacesVirtuosoLectura} select distinct(?s) from  {ObtenerUrlGrafo(pGrafo).ToLower()} where {{ ?s <http://purl.org/dc/elements/1.1/source> \"{pSource}\" }}";

            LeerDeVirtuoso(query, "SujetosDeGrafo", dataSet, pGrafo);

            List<string> listaSujetos = new List<string>();

            foreach (DataRow fila in dataSet.Tables[0].Rows)
            {
                if (!listaSujetos.Contains((string)fila[0]))
                {
                    listaSujetos.Add((string)fila[0]);
                }
            }

            return listaSujetos;
        }

        /// <summary>
        /// Obtiene una lista de de triples con sujeto predicado objeto de la lista de sujetos dadas
        /// </summary>
        /// <param name="pGrafo">Grafo del que obtener los triples</param>
        /// <param name="pSujeto">Sujeto para obtener triples</param>
        /// <returns></returns>
        public List<string> ObtenerListaTriplesDeSujeto(string pGrafo, string pSujeto)
        {
            FacetadoDS dataSet = new FacetadoDS();

            string query = $"{NamespacesVirtuosoLectura} select ?p ?o from {ObtenerUrlGrafo(pGrafo).ToLower()} where {{ <{pSujeto}> ?p ?o }}";

            LeerDeVirtuoso(query, "TriplesDeSujeto", dataSet, pGrafo);

            List<string> listaTriples = new List<string>();

            foreach (DataRow fila in dataSet.Tables[0].Rows)
            {
                listaTriples.Add($"{pSujeto} {(string)fila[0]} {(string)fila[1]} . ");
            }

            return listaTriples;
        }

        /// <summary>
        /// Obtiene las tripletas con el mismo predicado de un sujeto que tiene como objeto uno, el cúal tiene como objeto uno específico.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Tripletas con el mismo predicado de un sujeto que tiene como objeto uno, el cúal tiene como objeto uno específico</returns>
        public FacetadoDS ObtenerTripletasMismoPredicadoDeSujetoConObjetoQueTieneUnObjeto(string pGrafo, string pObjeto)
        {
            FacetadoDS dataSet = new FacetadoDS();

            string query = $"{NamespacesVirtuosoLectura} select ?s ?p ?o ";

            string filtro = pObjeto;

            if (filtro.StartsWith("http"))
            {
                filtro = $"<{filtro}>";
            }
            else
            {
                filtro = $"'{filtro}'";
            }

            query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{ ?s ?p ?o. ?s ?p ?o2. ?o2 ?p3 ?o3. FILTER (?o3={filtro}) }}";

            LeerDeVirtuoso(query, "SelectPropEnt", dataSet, pGrafo);

            return dataSet;
        }

        /// <summary>
        /// Obtiene las tripletas de un sujeto que tiene como objeto uno específico.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <param name="pPropiedad">Propiedad vinculante</param>
        /// <returns>Tripletas de un sujeto que tiene como objeto uno específico</returns>
        public FacetadoDS ObtenerTripletasConObjeto(string pGrafo, string pObjeto, string pPropiedad)
        {
            FacetadoDS dataSet = new FacetadoDS();

            string query = NamespacesVirtuosoLectura;

            if (string.IsNullOrEmpty(pPropiedad))
            {
                query += " select ?s ?p ?o ";
            }
            else
            {
                query += $" select ?s <{pPropiedad}> ?o ";
            }

            string filtro = pObjeto;

            if (filtro.StartsWith("http"))
            {
                filtro = $"<{filtro}>";
            }
            else
            {
                if (filtro.StartsWith("literal@"))
                {
                    filtro = filtro.Substring(filtro.IndexOf("@") + 1);
                }

                filtro = $"'{filtro}'";
            }

            string prop = "?p";

            if (!string.IsNullOrEmpty(pPropiedad))
            {
                prop = $"<{pPropiedad}>";
            }

            query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{ ?s {prop} ?o. FILTER (?o={filtro}) }}";

            LeerDeVirtuoso(query, "SelectPropEnt", dataSet, pGrafo);

            return dataSet;
        }

        /// <summary>
        /// Obtiene los sujetos que tienen como objetos unos específicos.
        /// </summary>
        /// <param name="pGrafo">Grafo del Tesauro Semántico</param>
        /// <param name="pObjeto">Objeto</param>
        /// <returns>Lista de sujetos que tienen como objetos unos específicos</returns>
        public List<string> ObtenerSujetosConObjetos(string pGrafo, List<string> pObjetos)
        {
            FacetadoDS dataSet = new FacetadoDS();

            string query = $"{NamespacesVirtuosoLectura} select distinct ?s ";

            StringBuilder filtro = new StringBuilder();
            StringBuilder where = new StringBuilder();
            int count = 0;

            foreach (string objeto in pObjetos)
            {
                where.Append($"?s ?p{count} ?o{count}. ");

                if (filtro.ToString().StartsWith("http"))
                {
                    filtro.Append($"?o{count} = <{objeto}> AND ");
                }
                else
                {
                    if (!objeto.StartsWith("literal@"))
                    {
                        filtro.Append($"?o{count} = '{objeto}' AND ");
                    }
                    else
                    {
                        filtro.Append($"?o{count} = '{objeto.Substring(objeto.IndexOf("@") + 1)}' AND ");
                    }
                }

                count++;
            }

            filtro.Remove(filtro.Length - 4, 4);

            query += $"from {ObtenerUrlGrafo(pGrafo).ToLower()} WHERE {{ ?s ?p ?o. {where} FILTER ({filtro}) }}";

            LeerDeVirtuoso(query, "SelectPropEnt", dataSet, pGrafo);
            List<string> sujetos = new List<string>();

            foreach (DataRow fila in dataSet.Tables[0].Rows)
            {
                sujetos.Add((string)fila[0]);
            }

            dataSet.Dispose();

            return sujetos;
        }

        #endregion

        #region Métodos de enumeraciones

        /// <summary>
        /// Obtiene el string de un tipo de busqueda
        /// </summary>
        /// <param name="pTipoBusqueda">Tipo de búsqueda</param>
        /// <returns></returns>
        public static string TipoBusquedaToString(TipoBusqueda pTipoBusqueda)
        {
            switch (pTipoBusqueda)
            {
                case TipoBusqueda.BusquedaAvanzada:
                    return BUSQUEDA_AVANZADA;
                case TipoBusqueda.Dafos:
                    return BUSQUEDA_DAFOS;
                case TipoBusqueda.Debates:
                    return BUSQUEDA_DEBATES;
                case TipoBusqueda.PersonasYOrganizaciones:
                    return BUSQUEDA_PERSONASYORG;
                case TipoBusqueda.Preguntas:
                    return BUSQUEDA_PREGUNTAS;
                case TipoBusqueda.Recursos:
                    return BUSQUEDA_RECURSOS;
                case TipoBusqueda.Comunidades:
                    return BUSQUEDA_COMUNIDADES;
                case TipoBusqueda.EditarRecursosPerfil:
                    return BUSQUEDA_RECURSOS_PERFIL;
                case TipoBusqueda.Blogs:
                    return BUSQUEDA_BLOGS;
                case TipoBusqueda.Mensajes:
                    return BUSQUEDA_MENSAJES;
            }

            return "";
        }

        #endregion

        #region NewYork Times
        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtienePersonasNewYorkTIme(List<string> Tag, FacetadoDS pFacetadoDS, string baseDatos)
        {
            for (int i = 0; i < Tag.Count; i++)
            {
                if (string.IsNullOrEmpty(Tag[i]))
                    Tag.RemoveAt(i);
            }

            for (int i = 0; i < Tag.Count; i++)
            {
                string untag = Tag[i].ToLower();
                int numpalabras = 0;
                List<string> tagbueno = AnalizadorSintactico.ObtenerTagsFrase(untag, out numpalabras);
                Tag[i] = "";
                for (int z = 0; z < tagbueno.Count; z++)
                {
                    Tag[i] += $"{tagbueno[z]} ";
                }
                if (!string.IsNullOrEmpty(Tag[i]))
                {
                    Tag[i] = Tag[i].Substring(0, Tag[i].Length - 1);
                }
            }

            string query = $"{NamespacesVirtuosoLectura} select ?GUIDFreebase ?Tag ?Coincidencia ?RutaNYT ?RutaDbpedia ?RutaFreebase ?RutaGeonames ?Descripcion from <http://{baseDatos}.com> where {{ ?GUIDFreebase <http://www.w3.org/2004/02/skos/core#prefLabel> ?Tag. FILTER";

            StringBuilder filtro = new StringBuilder("(");
            string or = "";
            bool hayTags = false;
            int contador = 0;
            foreach (string id in Tag)
            {
                filtro.Append(or);
                char[] delimit = new char[] { ' ' };
                string s10 = Tag[contador];
                string[] palabrasTag = s10.Split(delimit);
                if (palabrasTag.Length > 0)
                {
                    string separador = "(";
                    foreach (string substr in palabrasTag)
                    {
                        if (!string.IsNullOrEmpty(substr))
                        {
                            string idmayuscula = substr.Substring(0, 1).ToUpper() + substr.Substring(1);
                            filtro.Append($"{separador}((bif:lower(?Tag) LIKE '{idmayuscula.ToLower().Replace("'", "''")} %') or (bif:lower(?Tag) LIKE ' %{idmayuscula.ToLower()}') or (bif:lower(?Tag) LIKE ' %{idmayuscula.ToLower()}% '))");
                            separador = " and ";
                            hayTags = true;
                        }
                    }
                    filtro.Append(")");
                    or = " or ";

                }
                contador++;
            }

            filtro.Append(")");
            query += $"{filtro} OPTIONAL {{?GUIDFreebase <http://data.nytimes.com/elements/topicPage> ?RutaNYT. }} OPTIONAL {{?GUIDFreebase owl:sameAs ?RutaDbpedia.  FILTER (?RutaDbpedia LIKE '%dbpedia%')}} OPTIONAL {{?GUIDFreebase owl:sameAs ?RutaFreebase.  FILTER (?RutaFreebase LIKE '%freebase%')}} OPTIONAL{{?GUIDFreebase owl:sameAs ?RutaGeonames.  FILTER (?RutaGeonames LIKE '%geonames%')}} OPTIONAL {{?GUIDFreebase skos:definition ?Descripcion. }}}}";

            if (hayTags)
            {
                LeerDeVirtuoso(query, "PersonasNewYorkTimes", pFacetadoDS, "");
            }
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de las personas
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneOrganizacionNewYorkTime(List<string> Tag, FacetadoDS pFacetadoDS, string baseDatos)
        {
            for (int i = 0; i < Tag.Count; i++)
            {
                if (string.IsNullOrEmpty(Tag[i]))
                    Tag.RemoveAt(i);
            }

            for (int i = 0; i < Tag.Count; i++)
            {
                string untag = Tag[i].ToLower();
                int numpalabras = 0;
                List<string> tagbueno = AnalizadorSintactico.ObtenerTagsFrase(untag, out numpalabras);
                Tag[i] = "";
                for (int z = 0; z < tagbueno.Count; z++)
                {
                    Tag[i] += tagbueno[z] + " ";
                }
                if (!string.IsNullOrEmpty(Tag[i]))
                    Tag[i] = Tag[i].Substring(0, Tag[i].Length - 1);
            }

            string query = $"{NamespacesVirtuosoLectura} select ?GUIDFreebase ?Tag2 ?Coincidencia ?RutaNYT ?RutaDbpedia ?RutaFreebase ?RutaGeonames ?Descripcion from <http://{baseDatos}.com> where {{ ?GUIDFreebase ?p ?Tag2. ?GUIDFreebase ?p2 ?Tag. FILTER(?p=<http://www.w3.org/2004/02/skos/core#prefLabel> and ?p2=<http://www.w3.org/2004/02/skos/core#prefLabelBUSCAR>) FILTER";

            int contador = 0;
            StringBuilder filtro = new StringBuilder("(");
            string or = "";

            foreach (string id in Tag)
            {
                filtro.Append(or);
                char[] delimit = new char[] { ' ' };
                string s10 = Tag[contador];
                string[] palabrasTag = s10.Split(delimit);

                string separador = "(";
                foreach (string substr in palabrasTag)
                {
                    if (!string.IsNullOrEmpty(substr))
                    {
                        string idmayuscula = substr.Substring(0, 1).ToUpper() + substr.Substring(1);
                        filtro.Append($"{separador}((bif:lower(?Tag) LIKE '{idmayuscula.ToLower().Replace("'", "''")} %')or(bif:lower(?Tag) LIKE ' %{idmayuscula.ToLower()}')or(bif:lower(?Tag) LIKE ' %{idmayuscula.ToLower()}% '))");
                        separador = " and ";
                    }
                }
                filtro.Append(")");
                or = " or ";
                contador++;
            }
            filtro.Append(")");
            query += $"{filtro} OPTIONAL {{?GUIDFreebase <http://data.nytimes.com/elements/topicPage> ?RutaNYT. }} OPTIONAL {{?GUIDFreebase owl:sameAs ?RutaDbpedia.  FILTER (?RutaDbpedia LIKE '%dbpedia%')}} OPTIONAL {{?GUIDFreebase owl:sameAs ?RutaFreebase.  FILTER (?RutaFreebase LIKE '%freebase%')}} OPTIONAL{{?GUIDFreebase owl:sameAs ?RutaGeonames.  FILTER (?RutaGeonames LIKE '%geonames%')}} OPTIONAL {{?GUIDFreebase skos:definition ?Descripcion. }}}}";

            if (baseDatos.Equals("locations"))
            {
                LeerDeVirtuoso(query, "LugaresNewYorkTimes", pFacetadoDS, "");
            }
            else if (baseDatos.Equals("organizations"))
            {
                LeerDeVirtuoso(query, "OrganizacionesNewYorkTimes", pFacetadoDS, "");
            }
            else if (baseDatos.Equals("descriptors"))
            {
                LeerDeVirtuoso(query, "DescripcionesNewYorkTimes", pFacetadoDS, "");
            }
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de los lugares
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneLugaresNewYorkTime(List<string> Tag, List<string> Tags, FacetadoDS pFacetadoDS)
        {
            for (int i = 0; i < Tag.Count; i++)
            {
                string untag = Tag[i].ToLower();
                int numpalabras = 0;
                List<string> tagbueno = AnalizadorSintactico.ObtenerTagsFrase(untag, out numpalabras);
                StringBuilder bld = new StringBuilder();
                for (int z = 0; z < tagbueno.Count; z++)
                {
                    bld.Append(tagbueno[z] + " ");
                }
                Tag[i] = bld.ToString().Substring(0, bld.Length - 1);
            }

            string query = $"{NamespacesVirtuosoLectura} select distinct  ?GUIDFreebase ?Tag2 ?RutaGeonames ?RutaFreebase ?RutaDbpedia  ?RutaNYT  from <http://locations.com> where {{?GUIDFreebase ?p ?Tag2. ?GUIDFreebase ?p2 ?Tag. FILTER (?GUIDFreebase LIKE '%data.nytimes.com%')FILTER(((?p=<http://www.w3.org/2004/02/skos/core#prefLabel> and ?p2=<http://www.w3.org/2004/02/skos/core#prefLabelBUSCAR>) or ?p=<http://www.geonames.org/ontology#alternateName> ) and ";

            StringBuilder filtro = new StringBuilder("(");
            string or = "";
            int contador = 0;
            foreach (string id in Tag)
            {
                filtro.Append(or);
                char[] delimit = new char[] { ' ' };
                string s10 = Tag[contador];
                string separador = "(";
                foreach (string substr in s10.Split(delimit))
                {
                    string idmayuscula = substr.Substring(0, 1).ToUpper() + substr.Substring(1);
                    filtro.Append($"{separador} (bif:lower((?Tag) = '{idmayuscula.ToLower()}')");
                    separador = " and ";
                }

                filtro.Append(")");
                or = " or ";

                contador++;
            }

            filtro.Append(")");

            query += filtro;
            query += "and (lang(?Tag) = \"es\" or lang(?Tag) = \"eu\"  or lang(?Tag) = \"en\")) OPTIONAL {?GUIDFreebase <http://data.nytimes.com/elements/topicPage> ?RutaNYT. } OPTIONAL {?GUIDFreebase owl:sameAs ?RutaDbpedia.  FILTER (?RutaDbpedia LIKE '%dbpedia%')} OPTIONAL {?GUIDFreebase owl:sameAs ?RutaFreebase.  FILTER (?RutaFreebase LIKE '%freebase%')}OPTIONAL{?GUIDFreebase owl:sameAs ?RutaGeonames.  FILTER (?RutaGeonames LIKE '%geonames%')} }";

            LeerDeVirtuoso(query, "LugaresNewYorkTimes", pFacetadoDS, "");
        }

        /// <summary>
        /// Obtiene la información de organizaciones de dbpedia
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneOrganizacionesDBPedia(FacetadoDS pFacetadoDS)
        {
            string query = $"{NamespacesVirtuosoLectura} select ?o2 from <dbpedia> where {{ ?s rdf:type ?rdftype. FILTER( ?rdftype in (<http://schema.org/Organization>))  ?s rdfs:label ?o2 }} LIMIT 5000 ";

            LeerDeVirtuoso(query, "org", pFacetadoDS, "");
        }

        /// <summary>
        /// Obtiene la información de personas  de dbpedia
        /// </summary>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtienePersonasDBPedia(FacetadoDS pFacetadoDS)
        {
            string query = $"{NamespacesVirtuosoLectura} select ?o2 from <dbpedia> where {{ ?s rdf:type ?rdftype. FILTER( ?rdftype in (<http://schema.org/Person>))  ?s rdfs:label ?o2 }}  LIMIT 5000";

            LeerDeVirtuoso(query, "person", pFacetadoDS, "");
        }

        /// <summary>
        /// Obtiene la información de lugares de dbpedia
        /// </summary>        
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneLugaresDBPedia(FacetadoDS pFacetadoDS)
        {
            string query = $"{NamespacesVirtuosoLectura} select ?o2 from <dbpedia> where {{ ?s rdf:type ?rdftype. FILTER( ?rdftype in (<http://schema.org/Place>)) ?s rdfs:label ?o2 }} LIMIT 5000 ";

            LeerDeVirtuoso(query, "place", pFacetadoDS, "");
        }

        /// <summary>
        /// Obtiene la información de lugares, personas y organizaciones de dbpedia
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void ObtieneLugaresPersonasOrganizacionesDBPedia(FacetadoDS pFacetadoDS)
        {
            string query = $"{NamespacesVirtuosoLectura} select ?o2 from <dbpedia> where {{ ?s rdf:type ?rdftype. FILTER(?rdftype in (<http://schema.org/Person>, <http://schema.org/Organization>,  <http://schema.org/Place>))  ?s rdfs:label ?o2 }} ";

            LeerDeVirtuoso(query, "dbpedia", pFacetadoDS, "");
        }

        /// <summary>
        /// Obtiene la información que se va a pintar en la ficha de los lugares
        /// </summary>
        /// <param name="Tag">Lista con los Tags</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public FacetadoDS ObtieneDescripcionesNewYorkTime(List<string> Tag, FacetadoDS pFacetadoDS)
        {
            FacetadoDS facetadoDsdevolver = new FacetadoDS();

            StringBuilder query = new StringBuilder(NamespacesVirtuosoLectura);
            query.Append("select ?Tag ?GUIDFreebase '0' as ?Coincidencia '' as ?Entidad ?Ruta '' as ?WikipediaID 'descripcion' as ?Tipos ?Descripcion from <http://descriptors.com> where {?GUIDFreebase <http://www.w3.org/2004/02/skos/core#prefLabel> ?Tag. ?GUIDFreebase skos:definition ?Descripcion. ?GUIDFreebase <http://data.nytimes.com/elements/topicPage> ?Ruta. FILTER");
            string anadir = "(?Tag LIKE ";

            foreach (string id in Tag)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string idmayuscula = id.Substring(0, 1).ToUpper() + id.Substring(1);
                    query.Append($"{anadir}'%{id}%' or ?Tag LIKE '%{idmayuscula}%'");
                    anadir = " or ?Tag LIKE ";
                }
            }
            query.Append(" )} ");

            LeerDeVirtuoso(query.ToString(), "PersonasNewYorkTimes", facetadoDsdevolver, "");

            foreach (DataRow fila in facetadoDsdevolver.Tables["PersonasNewYorkTimes"].Rows)
            {
                DataRow newCustomersRow = pFacetadoDS.Tables["PersonasNewYorkTimes"].NewRow();
                newCustomersRow["Tag"] = (string)fila["Tag"];
                newCustomersRow["GUIDFreebase"] = (string)fila["GUIDFreebase"];
                newCustomersRow["Coincidencia"] = 0;
                newCustomersRow["Entidad"] = (string)fila["Tag"];
                newCustomersRow["Ruta"] = (string)fila["Ruta"];
                newCustomersRow["WikipediaID"] = (string)fila["WikipediaID"];
                newCustomersRow["Tipos"] = (string)fila["Tipos"];
                newCustomersRow["Descripcion"] = (string)fila["Descripcion"];
                pFacetadoDS.Tables["DescripcionesNewYorkTimes"].Rows.Add(newCustomersRow);
            }
            return pFacetadoDS;
        }
        #endregion

        #region migradores
        /// <summary>
        /// Inserta una nueva tripleta para cada tripleta ?s obraArte:autor Picasso, Pablo Ruiz que invierte el orden del nombre y apellido y quita la coma(Pablo Ruiz Picaso)
        /// </summary>
        /// <param name="pProyectoID">Proyecto donde insertar las nuevas tripletas</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void AcentosEnFormulariosSemanticos(Guid pOrganizacionID, string pProyectoID, List<string> listaPropiedades, FacetadoDS facetadoDS)
        {
            string lista = " ";
            foreach (string l in listaPropiedades)
            {
                lista = $",{lista}{l}";
            }

            try
            {
                if (InformacionOntologias.Count == 0)
                {
                    InformacionOntologias = ObtenerInformacionOntologias(pOrganizacionID, new Guid(pProyectoID));
                }
                string query = NamespacesVirtuosoLectura;


                query += "  select ?s ?o " + ObtenerFromGraph(pProyectoID) + " where {?s ?p ?o. FILTER (?p in (<http://rdfs.org/sioc/types#Tag>, <http://gnoss/hasTagTituloDesc>, foaf:firstName, foaf:familyName, gnoss:hasnombrecompleto, <http://gnoss/hasTagDesc>" + lista + "))}";

                LeerDeVirtuoso(query, "aaa", facetadoDS, pProyectoID);
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc)
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que está buscando el usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion del proyecto</param>
        public Dictionary<string, List<string>> ObtenerInformacionOntologias(Guid pOrganizacionID, Guid pProyectoID)
        {
            FacetaAD facetaAD = new FacetaAD(mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication,mloggerFactory.CreateLogger<FacetaAD>(),mloggerFactory);

            List<OntologiaProyecto> listaOntologias = facetaAD.ObtenerOntologiasProyecto(pOrganizacionID, pProyectoID);

            return ObtenerInformacionOntologias(listaOntologias);
        }

        /// <summary>
        /// Obtiene la lista de items extra que se obtendrá de la búsqueda y su prefijo (recetas, peliculas, etc) A PARTIR DE UN DATASET DE FACETAS YA CARGADO 
        /// </summary>
        /// <param name="pFacetaDS">DataSet de facetas cargado con los datos de las ontologías</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ObtenerInformacionOntologias(List<OntologiaProyecto> pListaOntologias)
        {
            Dictionary<string, List<string>> informacionOntologias = new Dictionary<string, List<string>>();

            foreach (OntologiaProyecto myrow in pListaOntologias)
            {
                if (!string.IsNullOrEmpty(myrow.OntologiaProyecto1) && !string.IsNullOrEmpty(myrow.Namespace))
                {
                    string urlOntologia = myrow.OntologiaProyecto1;
                    if (urlOntologia.Contains("##REP"))
                    {
                        urlOntologia = urlOntologia.Substring(0, urlOntologia.IndexOf("##REP"));
                    }
                    if (!informacionOntologias.ContainsKey(urlOntologia))
                    {
                        informacionOntologias.Add(urlOntologia, new List<string>());
                    }
                    if (!informacionOntologias[urlOntologia].Contains(myrow.Namespace))
                    {
                        informacionOntologias[urlOntologia].Add(myrow.Namespace);
                    }
                }
            }
            return informacionOntologias;
        }

        /// <summary>
        /// Inserta una nueva tripleta para cada tripleta ?s obraArte:autor Picasso, Pablo Ruiz que invierte el orden del nombre y apellido y quita la coma(Pablo Ruiz Picaso)
        /// </summary>
        /// <param name="pProyectoID">Proyecto donde insertar las nuevas tripletas</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void MigradorAutoresMuseos(string pProyectoID)
        {
            try
            {
                FacetadoDS facetadoDS = new FacetadoDS();

                string query = $" sparql prefix obrasArte:<http://pruebas.gnoss.net/Ontologia/ontologiaobradearte.owl#> SELECT * from <http://pruebas.gnoss.net/{pProyectoID.ToLower()}> where {{ ?s ?p ?o. FILTER (?p=obrasArte:Autor) }} ";

                LeerDeVirtuoso(query, "aaa", facetadoDS, pProyectoID);

                if (facetadoDS.Tables["aaa"].Rows.Count > 0)
                {
                    foreach (DataRow myrow2 in facetadoDS.Tables["aaa"].Rows)
                    {
                        string sujeto = $"<{((string)myrow2[0])}>";
                        string predicado = $"<{((string)myrow2[1])}BUSCAR>";
                        string objeto = (string)myrow2[2];
                        string apellido = "";
                        if (objeto.Contains(","))
                        {
                            string nombre = objeto.Substring(objeto.IndexOf(",") + 2);
                            apellido = objeto.Substring(0, objeto.IndexOf(","));
                            objeto = $"{nombre} {apellido}";
                        }

                        objeto = "\"" + objeto + "\"";

                        InsertaTripleta(pProyectoID, sujeto, predicado, objeto, 1);
                        InsertaTripleta(pProyectoID, sujeto, predicado, "\"" + apellido + "\"", 1);
                    }
                    facetadoDS.Dispose();
                }
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }
        }

        /// <summary>
        /// Inserta una nueva tripleta para cada tripleta ?s obraArte:autor Picasso, Pablo Ruiz que invierte el orden del nombre y apellido y quita la coma(Pablo Ruiz Picaso)
        /// </summary>
        /// <param name="pProyectoID">Proyecto donde insertar las nuevas tripletas</param>
        /// <param name="pFacetadoDS">DataSet de facetado</param>
        public void MigradorRangosMuseos(string pProyectoID)
        {
            try
            {
                FacetadoDS facetadoDS = new FacetadoDS();

                string query = $" sparql prefix obrasArte:<http://pruebas.gnoss.net/Ontologia/ontologiaobradearte.owl#> SELECT distinct * from <http://pruebas.gnoss.net/{pProyectoID.ToLower()}> where {{ ?s ?p  ?o. FILTER (?p=obrasArte:Peso and ?o != '') }} ";

                LeerDeVirtuoso(query, "aaa", facetadoDS, pProyectoID);

                if (facetadoDS.Tables["aaa"].Rows.Count > 0)
                {
                    foreach (DataRow myrow2 in facetadoDS.Tables["aaa"].Rows)
                    {
                        string sujeto = $"<{((string)myrow2[0])}>";
                        string predicado = $"<{((string)myrow2[1])}>";
                        string objeto = (string)myrow2[2];
                        if (!string.IsNullOrEmpty(objeto))
                        {
                            BorrarTripleta(pProyectoID, sujeto, predicado, objeto);
                            InsertaTripleta(pProyectoID, sujeto, predicado, objeto, 1);
                        }
                    }
                    facetadoDS.Dispose();
                }
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }
        }

        public int ObtenerNumMensajesPerfilID_Bandeja(Guid pUsuarioID, Guid pIdentidadID, string pBandeja)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = $"SPARQL {NamespacesBasicos} SELECT count(*) FROM <{mUrlIntranet}{pUsuarioID.ToString().ToLower()}> WHERE {{ {{?s rdf:type ?rdftype. FILTER (?rdftype in ('Mensaje')) }} ?s gnoss:IdentidadID ?gnossIdentidadID. FILTER (?gnossIdentidadID = gnoss:{pIdentidadID.ToString().ToUpper()}) ?s dce:type ?bandeja. FILTER(?bandeja = '{pBandeja}') ?s nmo:isRead ?pendienteLeer. FILTER(?pendienteLeer = 'Pendientes de leer') }}";

            LeerDeVirtuoso(query, "aaa", facetadoDS, pUsuarioID.ToString());

            int mensajesBandeja = 0;

            if (facetadoDS.Tables["aaa"].Rows.Count > 0)
            {
                int.TryParse(facetadoDS.Tables["aaa"].Rows[0][0].ToString(), out mensajesBandeja);
            }

            return mensajesBandeja;
        }

        /// <summary>
        /// Obtiene los sujetos de una propriedad por su valor.
        /// </summary>
        /// <param name="pGrafo">Grafo</param>
        /// <param name="pPropiedad">Propiedad</param>
        /// <param name="pListaValores">Valores</param>
        /// <returns>Lista con los sujetos de una propriedad por su valor</returns>
        public List<string> ObjeterSujetosDePropiedadPorValor(string pGrafo, string pPropiedad, List<string> pListaValores)
        {
            FacetadoDS facetadoDS = new FacetadoDS();
            string query = NamespacesVirtuosoLectura;

            StringBuilder filtro = new StringBuilder("?o");

            if (pListaValores.Count == 1)
            {
                if (pListaValores[0].StartsWith("http://") || pListaValores[0].StartsWith("https://"))
                {
                    filtro.Append($"=<{pListaValores[0]}>");
                }
                else
                {
                    filtro.Append($"='{pListaValores[0].Replace("'", "\\'")}'");
                }
            }
            else
            {
                filtro.Append(" in (");

                foreach (string valor in pListaValores)
                {
                    if (pListaValores[0].StartsWith("http://") || pListaValores[0].StartsWith("https://"))
                    {
                        filtro.Append($"<{pListaValores[0]}>,");
                    }
                    else
                    {
                        filtro.Append($"'{pListaValores[0].Replace("'", "\\'")}',");
                    }
                }

                filtro.Remove(filtro.Length - 1, 1);
                filtro.Append(")");
            }

            query += $" select distinct(?s) from {ObtenerUrlGrafo(pGrafo).ToLower()}  WHERE {{ ?s ?p ?o. FILTER (?p=<{pPropiedad}> AND {filtro}) }} ";

            LeerDeVirtuoso(query, pGrafo, facetadoDS, pGrafo);

            List<string> sujetos = new List<string>();

            foreach (DataRow fila in facetadoDS.Tables[pGrafo].Rows)
            {
                sujetos.Add((string)fila["s"]);
            }

            return sujetos;
        }

        #endregion

        #endregion

        #region FileService

        protected class CallFileService
        {
            private readonly ConfigService mConfigService;
            private readonly string mServicioArchivosUrl;
            private readonly CallTokenService mCallTokenService;
            private readonly LoggingService mLoggingService;
            private readonly TokenBearer mToken;

            public CallFileService(ConfigService configService, LoggingService loggingService)
            {
                mConfigService = configService;
                mServicioArchivosUrl = mConfigService.ObtenerUrlServicio("urlArchivos");
#if !DEBUG
            if (!mConfigService.PeticionHttps())
            {
                if (mServicioArchivosUrl.StartsWith("https://"))
                {
                    mServicioArchivosUrl = mServicioArchivosUrl.Replace("https://", "http://");
                }
            }
#endif
                mLoggingService = loggingService;
                mLoggingService.AgregarEntrada("INICIO Peticion para obtener el token del identity");
                mCallTokenService = new CallTokenService(configService);
                mToken = mCallTokenService.CallTokenApi();
                mLoggingService.AgregarEntrada("FIN Peticion para obtener el token del identity");
            }

            public byte[] ObtenerOntologiaBytes(Guid pOntologiaID)
            {
                mLoggingService.AgregarEntrada("INICIO Peticion ObtenerOntologia");
                string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerOntologia?pOntologiaID={pOntologiaID}", mToken);
                mLoggingService.AgregarEntrada("FIN Peticion ObtenerOntologia");
                byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
                return buffer;
            }

            public byte[] ObtenerXmlOntologiaBytes(Guid pOntologiaID)
            {
                mLoggingService.AgregarEntrada("INICIO Peticion ObtenerXmlOntologia");
                string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerXmlOntologia?pOntologiaID={pOntologiaID}", mToken);
                mLoggingService.AgregarEntrada("FIN Peticion ObtenerXmlOntologia");
                byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
                return buffer;
            }
        }

        /// <summary>
        /// Clase que contiene información util de una propiedad dada
        /// </summary>
        protected class DatosPropiedadOntologia
        {
            /// <summary>
            /// Nombre de la propiedad
            /// </summary>
            public string NombrePropiedad { get; set; }

            /// <summary>
            /// RdfType de la ontología a la que pertenece la propiedad
            /// </summary>
            public string RdfType { get; set; }

            /// <summary>
            /// Objeto con información sobre la configuración de la propiedad
            /// </summary>
            public Propiedad Propiedad { get; set; }

            /// <summary>
            /// Xml de la ontología a la que pertenece la propiedad
            /// </summary>
            public XmlDocument XmlOntologia { get; set; }
        }


        protected class CallTokenService
        {
            private readonly ConfigService mConfigService;

            public CallTokenService(ConfigService configService)
            {
                mConfigService = configService;
            }

            public TokenBearer CallTokenApi()
            {
                if (string.IsNullOrEmpty(mConfigService.ObtenerClientIDIdentity()) || string.IsNullOrEmpty(mConfigService.ObtenerClientSecretIDIdentity()) || string.IsNullOrEmpty(mConfigService.ObtenerScopeIdentity()))
                {
                    throw new ExcepcionGeneral("Es necesario configurar los parametro para el Identity Provider");
                }
                string stringData = $"grant_type=client_credentials&scope={mConfigService.ObtenerScopeIdentity()}&client_id={mConfigService.ObtenerClientIDIdentity()}&client_secret={mConfigService.ObtenerClientSecretIDIdentity()}";
                return CallTokenIdentity(stringData);
            }

            protected TokenBearer CallTokenIdentity(string stringData)
            {
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                contentData.Headers.Add("UserAgent", UtilWeb.GenerarUserAgent());
                HttpResponseMessage response = null;
                try
                {
                    HttpClient client = new HttpClient();
                    client.Timeout = TimeSpan.FromDays(1);
                    string authority = mConfigService.GetAuthority() + "/connect/token";
                    response = client.PostAsync($"{authority}", contentData).Result;
                    response.EnsureSuccessStatusCode();
                    string result = response.Content.ReadAsStringAsync().Result;
                    TokenBearer token = JsonConvert.DeserializeObject<TokenBearer>(result);
                    return token;
                }
                catch (HttpRequestException)
                {
                    if (response != null && !string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                    {
                        throw new HttpRequestException(response.Content.ReadAsStringAsync().Result);
                    }
                    else if (response != null)
                    {
                        throw new HttpRequestException(response.ReasonPhrase);
                    }
                    else
                    {
                        throw new HttpRequestException();
                    }
                }
            }
        }

        protected class CallWebMethods
        {
            /// <summary>
            /// Hace una petición get
            /// </summary>
            /// <param name="urlBase">Url donde se encuentra el api</param>
            /// <param name="urlMethod">Url del método</param>
            /// <param name="pToken">Token para validar la peticion</param>
            public static string CallGetApiToken(string urlBase, string urlMethod, TokenBearer pToken = null)
            {
                string result = "";
                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(urlBase) && !urlBase.EndsWith('/') && !string.IsNullOrEmpty(urlMethod) && !urlMethod.StartsWith('/'))
                {
                    urlBase = $"{urlBase}/";
                }
                try
                {
                    HttpClient client = new HttpClient();
                    client.DefaultRequestHeaders.Add("UserAgent", UtilWeb.GenerarUserAgent());
                    if (pToken != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"{pToken.token_type} {pToken.access_token}");
                    }
                    response = client.GetAsync($"{urlBase}{urlMethod}").Result;
                    response.EnsureSuccessStatusCode();
                    result = response.Content.ReadAsStringAsync().Result;
                }
                catch (HttpRequestException)
                {
                    StringBuilder except = new StringBuilder();
                    except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                    if (!string.IsNullOrEmpty(response.Content.ReadAsStringAsync().Result))
                    {
                        except.AppendLine(response.Content.ReadAsStringAsync().Result);
                        throw new HttpRequestException(except.ToString());
                    }
                    else
                    {
                        except.AppendLine(response.ReasonPhrase);
                        throw new HttpRequestException(except.ToString());
                    }
                }
                catch (Exception ex)
                {
                    StringBuilder except = new StringBuilder();
                    except.AppendLine($"Url del intento de llamada: {urlBase}{urlMethod} --------- error: ");
                    except.AppendLine(ex.Message);
                    throw new ExcepcionGeneral(except.ToString());
                }
                return result;
            }
        }

        #endregion
    }
}
