using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL.Documentacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Microsoft.AspNetCore.Mvc;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Web.MVC.Models.Tesauro;
using Newtonsoft.Json;
using Es.Riam.Gnoss.AD.EntityModel.Models.Cache;
using StackExchange.Redis;
using Es.Riam.Gnoss.Elementos.Notificacion;
using Es.Riam.Interfaces.InterfacesOpen;
using Microsoft.Extensions.Logging;
using Es.Riam.Gnoss.AD.ParametroAplicacion;

namespace Es.Riam.Gnoss.CL.Facetado
{
	[Serializable]
	public class CacheConsultasCostosas
	{
		public Guid ProyectoID { get; set; }
		public Guid OrganizacionID { get; set; }
		public Dictionary<string, List<string>> ListaFiltros { get; set; }
		public List<string> ListaFiltrosExtra { get; set; }
		public bool Descendente { get; set; }
		public string TipoFiltro { get; set; }
		public bool EstaEnMyGnoss { get; set; }
		public bool EsMiembroComunidad { get; set; }
		public bool EsInvitado { get; set; }
		public string IdentidadID { get; set; }
		public int Inicio { get; set; }
		public int Limite { get; set; }
		public List<string> Semanticos { get; set; }
		public string FiltroContextoSelect { get; set; }
		public string FiltroContextoWhere { get; set; }
		public string FiltroContextoOrderBy { get; set; }
		public int FiltroContextoPesoMinimo { get; set; }
		public TipoProyecto TipoProyecto { get; set; }
		public string NamespacesExtra { get; set; }
		public string ResultadosEliminar { get; set; }
		public bool PermitirRecursosPrivados { get; set; }
		public bool OmitirPalabrasNoRelevantesSearch { get; set; }
		public TiposAlgoritmoTransformacion TipoAlgoritmoTransformacion { get; set; }
		public Dictionary<string, Tuple<string, string, string, bool>> FiltrosSearchPersonalizados { get; set; }
		public bool EsMovil { get; set; }
		public string ClaveFaceta { get; set; }
		public TipoDisenio TipoDisenio { get; set; }
		public bool EsRango { get; set; }
		public List<int> ListaRangos { get; set; }
		public bool Excluida { get; set; }
		public bool UsarHilos { get; set; }
		public bool ExcluirPersonas { get; set; }
		public int Reciproca { get; set; }
		public TipoPropiedadFaceta TipoPropiedadesFaceta { get; set; }
		public bool Inmutable { get; set; }
		public string NombreFaceta { get; set; }
		public TiposAlgoritmoTransformacion TiposAlgoritmoTransformacion { get; set; }
		public string LanguageCode { get; set; }
		public string FiltroChart { get; set; }
		public string SelectChart { get; set; }
		public List<Guid> ListaExcluidos { get; set; }
		public TipoBusqueda TipoBusqueda { get; set; }
		public bool EstaEnProyecto { get; set; }
		public string NombreGrafo { get; set; }
		public string FiltrosContexto { get; set; }
		public string FiltroAutocompletar { get; set; }
		public TipoCacheCostosa TipoDeCache { get; set; }
	}

	public enum TipoCacheCostosa
	{
		Resultados = 1,
		NumeroResultados = 2,
		ResultadosFormatoChart = 3,
		ResultadosFormatoMapa = 4,
		Facetas = 5,
		FacetaEspecialDBLP = 6,
		FacetaEspecialDBLPJournalPartOF = 7,
		FacetaSinOrdenDBLP = 8,
		PersonasExacto = 9,
		Autocompletar = 10,
		DatosAutocompletar = 11
	}

	/// <summary>
	/// Cache layer para el Facetado
	/// </summary>
	public class FacetadoCL : BaseCL
	{
		#region Miembros

		/// <summary>
		/// Clave MAESTRA de la caché
		/// </summary>
		private readonly string[] mMasterCacheKeyArray = { NombresCL.FACETADO };

		/// <summary>
		/// Clase de negocio
		/// </summary>
		private DocumentacionCN mDocumentacionCN = null;

		private FacetadoCN mFacetadoCN = null;

		private bool mObtenerDeCache = true;
		private bool mVerificarFechaDeCache = false;

		/// <summary>
		/// Url de la Intranet
		/// </summary>
		private string mUrlIntranet;

		/// <summary>
		/// Diccionario con los nombres de ontologias del proyecto donde se esta buscando y su prefijo
		/// </summary>
		private Dictionary<string, List<string>> mInformacionOntologias;


		/// <summary>
		/// Objetos por los cuales se va a filtrar y no se va a cambiar su parametro de busqueda. EJ: cotecmembership0, cotecmembership1, cotecmembership3 en la busqueda
		/// </summary>InformacionOntologias
		private string mMandatoryRelacion;

		public static ConcurrentDictionary<string, List<PoolRedis>> mListaPoolsRedis = new ConcurrentDictionary<string, List<PoolRedis>>();

		private ConfigService mConfigService;
		private EntityContext mEntityContext;
		private LoggingService mLoggingService;
		private VirtuosoAD mVirtuosoAD;
		private RedisCacheWrapper mRedisCacheWrapper;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private static string COLA_REFRESCO_CACHES_BUSQUEDAS = "ColaRefrescoCachesBusquedas";
		public static long TTL_CACHE_DEFECTO = 12;
		public static long TTL_CACHE_USUARIOS_DEFECTO = 3;
		public static long DURACION_CONSULTA_DEFECTO = 2;
		public static long TIEMPO_RECALCULAR_CACHE_DEFECTO = 30;
		#endregion

		#region constructores

		/// <summary>
		/// Constructor para FacetadoCL
		/// </summary>
		public FacetadoCL(string pUrlIntragnoss, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCL> logger, ILoggerFactory loggerFactory)
			: base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
		{
			mRedisCacheWrapper = redisCacheWrapper;
			mConfigService = configService;
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mVirtuosoAD = virtuosoAD;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mUrlIntranet = pUrlIntragnoss;
			this.mFacetadoCN = new FacetadoCN(pUrlIntragnoss, entityContext, loggingService, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
		}

		/// <summary>
		/// Constructor para FacetadoCL
		/// </summary>
		public FacetadoCL(string pUrlIntragnoss, string pGrafoID, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCL> logger, ILoggerFactory loggerFactory)
			: base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
		{
			mRedisCacheWrapper = redisCacheWrapper;
			mConfigService = configService;
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mVirtuosoAD = virtuosoAD;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mUrlIntranet = pUrlIntragnoss;
			this.mFacetadoCN = new FacetadoCN(pUrlIntragnoss, pGrafoID, entityContext, loggingService, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
		}

		/// <summary>
		/// Constructor para FacetadoCL
		/// </summary>
		/// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
		/// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
		/// <param name="pPoolName">Nombre del pool de conexión</param>
		public FacetadoCL(string pFicheroConfiguracionBD, string pPoolName, string pUrlIntragnoss, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCL> logger, ILoggerFactory loggerFactory)
			: this(pFicheroConfiguracionBD, pPoolName, pUrlIntragnoss, "", entityContext, loggingService, redisCacheWrapper, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, logger,loggerFactory)
		{
		}

		/// <summary>
		/// Constructor para FacetadoCL
		/// </summary>
		/// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
		/// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
		/// <param name="pPoolName">Nombre del pool de conexión</param>
		public FacetadoCL(string pFicheroConfiguracionBD, string pPoolName, string pUrlIntragnoss, string pGrafoID, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCL> logger, ILoggerFactory loggerFactory)
			: base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
		{
			mRedisCacheWrapper = redisCacheWrapper;
			mConfigService = configService;
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mVirtuosoAD = virtuosoAD;
            mlogger = logger;
			mLoggerFactory = loggerFactory;
            mUrlIntranet = pUrlIntragnoss;
			this.mFacetadoCN = new FacetadoCN(mFicheroConfiguracionBD, pUrlIntragnoss, pGrafoID, entityContext, loggingService, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
		}

		/// <summary>
		/// Constructor para el usuario que tiene permiso de ver todas las personas
		/// </summary>
		/// <param name="pUrlIntragnoss">URL de intragnoss</param>
		/// <param name="pObtenerPrivados">Verdad si el usuario actual puede obtener los privados</param>
		public FacetadoCL(string pUrlIntragnoss, bool pObtenerPrivados, string pIdGrafo, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCL> logger,ILoggerFactory loggerFactory)
			: this(pUrlIntragnoss, pObtenerPrivados, pIdGrafo, false, entityContext, loggingService, redisCacheWrapper, configService, virtuosoAD, servicesUtilVirtuosoAndReplication,	logger,loggerFactory)
		{
		}

		/// <summary>
		/// Constructor para el usuario que tiene permiso de ver todas las personas
		/// </summary>
		/// <param name="pUrlIntragnoss">URL de intragnoss</param>
		/// <param name="pObtenerPrivados">Verdad si el usuario actual puede obtener los privados</param>
		public FacetadoCL(string pUrlIntragnoss, bool pObtenerPrivados, string pIdGrafo, bool pCacheSPARQL, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FacetadoCL> logger,ILoggerFactory loggerFactory)
			: base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger, loggerFactory)
		{
			mRedisCacheWrapper = redisCacheWrapper;
			mConfigService = configService;
			mEntityContext = entityContext;
			mLoggingService = loggingService;
			mVirtuosoAD = virtuosoAD;
            mlogger = logger;
            mLoggerFactory = loggerFactory;

            if (pCacheSPARQL && HayCacheSparql)
			{
				mPoolName = "SPARQL";
			}
			mUrlIntranet = pUrlIntragnoss;
			this.mFacetadoCN = new FacetadoCN(pUrlIntragnoss, pObtenerPrivados, pIdGrafo, entityContext, loggingService, configService, virtuosoAD, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
		}

		#endregion

		static List<string> PARAMETROS_EXTRA_CONSULTAS_VIRTUOSO = new List<string>() { "[PARAMETROESPACIOULTIMODIFERENTE]", "[ESPACIOULTIMODIFERENTE]", "[PARAMETROESPACIOIN]", "[ESPACIOULTIMODIFERENTELIMPIO]", "[PARAMETROESPACIOULTIMODIFERENTELIMPIO]" };

		#region Métodos

		#region RSS
		/// <summary>
		/// Obtiene el RSS de la comunidad
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pPagina">Puede ser: recursos,encuestas,debates,preguntas o pestanyasem</param>
		/// <param name="pParam_adicional">Parametro adicional para pestanyasem</param>
		public string ObtenerRSSDeComunidad(Guid pProyectoID, string pPagina, string pParam_adicional)
		{
			string rawKey = string.Concat(NombresCL.RSSSCOMUNIDAD, "_", pProyectoID, "_", pPagina);
			if (pParam_adicional != null)
			{
				rawKey = string.Concat(NombresCL.RSSSCOMUNIDAD, "_", pProyectoID, "_", pPagina, "_", pParam_adicional);
			}


			// Compruebo si está en la caché
			string RSS = ObtenerObjetoDeCache(rawKey, typeof(string)) as string;

			if (string.IsNullOrEmpty(RSS))
			{
				return "";
			}
			else
			{
				return RSS;
			}
		}

		/// <summary>
		/// Agrega el RSS de la comunidad
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pPagina">Puede ser: recursos,encuestas,debates,preguntas o pestanyasem</param>
		/// <param name="pParam_adicional">Parametro adicional para pestanyasem</param>
		public void AgregarRSSDeComunidad(Guid pProyectoID, string pRSS, string pPagina, string pParam_adicional)
		{
			string rawKey = string.Concat(NombresCL.RSSSCOMUNIDAD, "_", pProyectoID, "_", pPagina);
			if (pParam_adicional != null)
			{
				rawKey = string.Concat(NombresCL.RSSSCOMUNIDAD, "_", pProyectoID, "_", pPagina, "_", pParam_adicional);
			}

			AgregarObjetoCache(rawKey, pRSS);
		}

		/// <summary>
		/// Borra el RSS de la comunidad
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pPagina">Puede ser: recursos,encuestas,debates o preguntas</param>
		public void BorrarRSSDeComunidad(Guid pProyectoID)
		{
			string rawKey = string.Concat(NombresCL.RSSSCOMUNIDAD, "_", pProyectoID.ToString());
			List<string> claves = new List<string>();
			if (ClienteRedisLectura != null)
			{
				claves = ClienteRedisLectura.Keys(ObtenerClaveCache(rawKey + "*").ToLower()).Result.ToList();
			}

			InvalidarCachesMultiples(claves);
		}
		#endregion


		/// <summary>
		/// Obtiene las facetas de una determinada búsqueda
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil del usuario que está bu
		/// scando</param>
		/// <param name="pHomeProyecto">Verdad si se están cargando las facetas para la home de la comunidad</param>
		/// <param name="mIdioma">Idioma para el que se cargan las facetas</param>
		/// <param name="pEsUsuarioInvitado">Verdad si es el usuario invitado</param>
		/// <param name="pNumeroFacetas">Número de facetas a cargar</param>
		/// <param name="pParametrosClaveExtra">Parametros extra para la clave de caché</param>
		/// <param name="pBusquedaTipoMapa">Indica si la búsqueda es de tipo mapa</param>
		/// <returns></returns>
		public string ObtenerFacetasDeBusquedaEnProyecto(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, int pNumeroFacetas, bool pEsUsuarioInvitado, string mIdioma, bool pHomeProyecto, string pParametrosClaveExtra, Guid? pOrganizacionID, bool pBusquedaTipoMapa, string pParametros, bool pFacetaPrivadaGrupo)
		{
			string home = "";
			if (pHomeProyecto)
			{
				home = "_home";
			}
			string rawKey = ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, pPerfilID, true, pNumeroFacetas, pEsUsuarioInvitado, pParametrosClaveExtra, pFacetaPrivadaGrupo);
			if (pOrganizacionID.HasValue && !rawKey.Contains(NombresCL.PRIMEROSRECURSOS))
			{
				rawKey += $"_{pOrganizacionID.Value}";
			}
			rawKey += $"_{mIdioma}{home}";

			if (pBusquedaTipoMapa)
			{
				rawKey += "_mapa";
			}

			if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !string.IsNullOrEmpty(pParametros))
			{
				rawKey += $"_{pParametros}";
			}

			string html = ObtenerObjetoDeCache(rawKey, typeof(string)) as string;

			return html;
		}

		/// <summary>
		/// Obtiene los resultados y las facetas de una determinada búsqueda
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil del usuario que está buscando</param>
		/// <returns></returns>
		public string ObtenerResultadosYFacetasDeBusquedaEnProyecto(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, string pNumeroPagina, string pIdioma, Guid? pOrganizacionID, bool pEsIdentidadInvitado, string pParametros, bool pEsUsuarioInvitado)
		{
			string rawKey = ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, pPerfilID, false, pEsUsuarioInvitado);
			if (pOrganizacionID.HasValue && !rawKey.Contains(NombresCL.PRIMEROSRECURSOS))
			{
				rawKey += $"_{pOrganizacionID.Value}";
			}
			if (pEsIdentidadInvitado)
			{
				rawKey += "_invitado";
			}
			rawKey += $"_{pIdioma}_{pNumeroPagina}";

			if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !string.IsNullOrEmpty(pParametros))
			{
				rawKey += $"_{pParametros}";
			}

			string html = ObtenerObjetoDeCache(rawKey, typeof(string)) as string;

			return html;
		}

		/// <summary>
		/// Obtiene una faceta de la cache
		/// </summary>
		/// <param name="pNombreFaceta">Nombre de la faceta</param>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <returns></returns>
		public FacetadoDS ObtenerRecursosDeFiltroPorFaceta(Guid pProyectoID, string pNombreFaceta, string pValor, int pNumResultados, List<string> pListaSemanticos, Guid pIdentidadID, bool pEsIdentidadInvitada, bool pEsUsuarioInvitado, bool pEsMovil = false)
		{
			string rawKey = string.Concat("RecursosDeFiltroPorFaceta_", $"{pNombreFaceta}_{pValor}_", pProyectoID);

			// Compruebo si está en la caché
			FacetadoDS facetadoDS = ObtenerObjetoDeCache(rawKey, typeof(FacetadoDS)) as FacetadoDS;
			if (facetadoDS == null)
			{
				// Si no está, lo cargo y lo almaceno en la caché
				FacetadoCN facetadoCN = new FacetadoCN(mUrlIntranet, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
				facetadoCN.InformacionOntologias = InformacionOntologias;
				facetadoDS = new FacetadoDS();
				facetadoCN.ListaItemsBusquedaExtra = pListaSemanticos;

				Dictionary<string, List<string>> listaFiltros = new Dictionary<string, List<string>>();
				List<string> listaAux = new List<string>();

				if (pValor.Contains("="))
				{
					if (pValor.Contains("|"))
					{
						foreach (string filtro in pValor.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
						{
							listaAux.Add(filtro.Substring(filtro.IndexOf("=") + 1));
							listaFiltros.Add(filtro.Substring(0, filtro.IndexOf("=")), listaAux);
							listaAux = new List<string>();
						}
					}
					else
					{
						listaAux.Add(pValor.Substring(pValor.IndexOf("=") + 1));
						listaFiltros.Add(pValor.Substring(0, pValor.IndexOf("=")), listaAux);
					}
				}
				else
				{
					listaAux.Add(pValor);
					listaFiltros.Add(pNombreFaceta, listaAux);
				}

				listaAux = new List<string>();
				listaAux.Add(FacetadoAD.BUSQUEDA_RECURSOS);
				if (!listaFiltros.ContainsKey("rdf:type"))
				{
					listaFiltros.Add("rdf:type", listaAux);
				}

				facetadoCN.ObtenerResultadosBusqueda(true, facetadoDS, "", listaFiltros, new List<string>(), pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID.ToString().ToUpper(), 0, pNumResultados, pListaSemanticos, "", "", "", 0, pEsMovil);

				AgregarObjetoCache(rawKey, facetadoDS);
				facetadoCN.Dispose();
			}

			return facetadoDS;
		}

		/// <summary>
		/// Obtiene una faceta de la cache
		/// </summary>
		/// <param name="pNombreFaceta">Nombre de la faceta</param>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <returns></returns>
		public FacetadoDS ObtenerFaceta(string pNombreFaceta, Guid pProyectoID, List<string> pListaSemanticos, bool pExcluida, bool pEsMovil, bool pEsIdentidadInvitada, Guid pIdentidadID, bool pEsUsuarioInvitado, List<Guid> pListaExcluidos = null)
		{
			string rawKey = string.Concat("Faceta_", pNombreFaceta + "_", pProyectoID);

			// Compruebo si está en la caché
			FacetadoDS facetadoDS = ObtenerObjetoDeCache(rawKey, typeof(FacetadoDS)) as FacetadoDS;
			if (facetadoDS == null)
			{
				// Si no está, lo cargo y lo almaceno en la caché
				FacetadoCN.InformacionOntologias = InformacionOntologias;
				facetadoDS = new FacetadoDS();
				FacetadoCN.ListaItemsBusquedaExtra = pListaSemanticos;

				FacetadoCN.ObtenerFaceta(pProyectoID.ToString(), facetadoDS, pNombreFaceta, new Dictionary<string, List<string>>(), new List<string>(), pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID.ToString().ToUpper(), TipoDisenio.ListaMenorAMayor, 0, 1000, pListaSemanticos, pExcluida, pEsMovil, false, new Guid(), pListaExcluidos);

				AgregarObjetoCache(rawKey, facetadoDS);
			}

			return facetadoDS;
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
		public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pExcluida, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos, IAvailableServices pAvailableServices)
		{
			ObtenerFaceta(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, false, null, pExcluida, false, false, true, null, pInmutable, pEsMovil, pListaExcluidos, pAvailableServices);
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
			FacetadoCN.ObtenerTituloFacetas(pProyectoID, pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pListaRangos, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pFiltrosSearchPersonalizados, pListaFacetas, pListaFacetasExtraContexto);
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
		public void ObtenerFacetaEspecialDBLPJournalPartOF(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, string pOrden, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, IAvailableServices pAvailableServices)
		{

			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(pProyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.FacetaEspecialDBLPJournalPartOF;
				cacheModel.ProyectoID = new Guid(pProyectoID);
				cacheModel.OrganizacionID = ProyectoAD.MetaOrganizacion;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.NombreFaceta = pNombreFaceta;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerFacetaEspecialDBLPJournalPartOF(pProyectoID, facetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, new Guid(pProyectoID)))
					{
						AgregarBusquedaACache(clave, facetadoDS, new Guid(pProyectoID));
					}					
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerFacetaEspecialDBLPJournalPartOF(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere);
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
		/// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
		/// <param name="pFiltroContextoWhere">Filtros de contexto</param>
		/// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
		/// <param name="pSemanticos">Lista de formularios semánticos</param>
		public void ObtenerFacetaEspecialDBLP(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(pProyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.FacetaEspecialDBLP;
				cacheModel.ProyectoID = new Guid(pProyectoID);
				cacheModel.OrganizacionID = ProyectoAD.MetaOrganizacion;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.NombreFaceta = pNombreFaceta;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicioConsulta = DateTime.Now;
					FacetadoCN.ObtenerFacetaEspecialDBLP(pProyectoID, facetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere);
					DateTime horaFinConsulta = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicioConsulta, horaFinConsulta, new Guid(pProyectoID)))
					{
						AgregarBusquedaACache(clave, facetadoDS, new Guid(pProyectoID));
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerFacetaEspecialDBLP(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoWhere);
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
		/// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
		/// <param name="pFiltroContextoWhere">Filtros de contexto</param>
		/// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
		/// <param name="pSemanticos">Lista de formularios semánticos</param>
		public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos, IAvailableServices pAvailableServices)
		{
			ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, false, pInmutable, pEsMovil, pListaExcluidos, pAvailableServices);
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
		public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos, IAvailableServices pAvailableServices)
		{
			ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos, false, true, null, pInmutable, pEsMovil, pListaExcluidos, pAvailableServices);
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
		public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos, IAvailableServices pAvailableServices)
		{
			ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, true, 0, TipoPropiedadFaceta.Numero, pFiltrosSearchPersonalizados, pInmutable, pEsMovil, pListaExcluidos, pAvailableServices);
		}

		private Dictionary<string, string> ObtenerClavesFacetas(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			// TODO: Crear una configuración
			pEstaEnMyGnoss = false;
			pEsMiembroComunidad = false;
			pEsInvitado = true;
			pIdentidadID = UsuarioAD.Invitado.ToString();

			string clave = $"_obtenerFaceta_{pProyectoID}_{pProyectoID}{pClaveFaceta}{pEstaEnMyGnoss}{pEsMiembroComunidad}{pEsInvitado}{pIdentidadID}{pTipoDisenio}{pInicio}{pLimite}{pFiltroContextoWhere}false"; // Le pongo false al final para no perder las cachés de DBLP por el parámetro pEsCatalogoNoSocialConUnTipo

			string claveParametros = $"pmobtenerFaceta_{pProyectoID}__{pProyectoID}{pClaveFaceta}{pEstaEnMyGnoss}{pEsMiembroComunidad}{pEsInvitado}{pIdentidadID}{pTipoDisenio}{pInicio}{pLimite}{pFiltroContextoWhere}false"; // Le pongo false al final para no perder las cachés de DBLP por el parámetro pEsCatalogoNoSocialConUnTipo

			foreach (string filtro in pListaFiltros.Keys.OrderBy(item => item))
			{
				bool aniadir = true;
				if (pFiltrosSearchPersonalizados.ContainsKey(filtro))
				{
					string valores = $"{pFiltrosSearchPersonalizados[filtro].Item1}{pFiltrosSearchPersonalizados[filtro].Item2}{pFiltrosSearchPersonalizados[filtro].Item3}{pFiltrosSearchPersonalizados[filtro].Item4}";

					foreach (string parametro in PARAMETROS_EXTRA_CONSULTAS_VIRTUOSO)
					{
						if (valores.Contains(parametro))
						{
							aniadir = false;
						}
					}
					if (aniadir)
					{
						string hash = StringToHash(valores);
						clave += $"_{filtro}{hash}";
						claveParametros += $"_{filtro}{hash}";
					}
				}
				else
				{
					foreach (string filtroInt in pListaFiltros[filtro])
					{
						clave += $"_{filtro}_{filtroInt}";
						claveParametros += $"_{filtro}_{filtroInt}";
					}
				}
			}

			foreach (string filtroExtra in pListaFiltrosExtra)
			{
				clave += $"_{filtroExtra}";
			}

			if (pEsRango && pListaRangos != null && pListaRangos.Count > 0)
			{
				clave += $"_{pListaRangos[0]}";
			}
			dictionary.Add("clave", clave);
			dictionary.Add("claveParametros", claveParametros);
			return dictionary;
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
		public bool ExisteFacetaEnCache(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil)
		{
			bool exist = false;
			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(pProyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Facetas;
				cacheModel.ProyectoID = new Guid(pProyectoID);
				cacheModel.ClaveFaceta = pClaveFaceta;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.TipoDisenio = pTipoDisenio;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.OmitirPalabrasNoRelevantesSearch = pOmitirPalabrasNoRelevantesSearch;
				cacheModel.Reciproca = pReciproca;
				cacheModel.TipoPropiedadesFaceta = pTipoPropiedadesFaceta;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.Inmutable = pInmutable;
				cacheModel.EsMovil = pEsMovil;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				exist = ExisteClaveEnCache(clave);
			}

			return exist;
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
		public void ObtenerFaceta(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, bool pExcluirPersonas, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, int pReciproca, TipoPropiedadFaceta pTipoPropiedadesFaceta, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pInmutable, bool pEsMovil, List<Guid> pListaExcluidos, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(pProyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Facetas;
				cacheModel.ClaveFaceta = pClaveFaceta;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.EsMovil = pEsMovil;
				cacheModel.Excluida = pExcluida;
				cacheModel.UsarHilos = pUsarHilos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EsRango = pEsRango;
				cacheModel.OmitirPalabrasNoRelevantesSearch = pOmitirPalabrasNoRelevantesSearch;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.ProyectoID = new Guid(pProyectoID);
				cacheModel.Semanticos = pSemanticos;
				cacheModel.ListaRangos = pListaRangos;
				cacheModel.ExcluirPersonas = pExcluirPersonas;
				cacheModel.TipoDisenio = pTipoDisenio;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.Reciproca = pReciproca;
				cacheModel.TipoPropiedadesFaceta = pTipoPropiedadesFaceta;
				cacheModel.Inmutable = pInmutable;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerFaceta(pProyectoID, facetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados, pInmutable, pEsMovil, pListaExcluidos);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, new Guid(pProyectoID)))
					{
						AgregarBusquedaACache(clave, facetadoDS, new Guid(pProyectoID));
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerFaceta(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos, pExcluirPersonas, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pReciproca, pTipoPropiedadesFaceta, pFiltrosSearchPersonalizados, pInmutable, pEsMovil, pListaExcluidos);
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
		/// <param name="pEsCatalogoNosocial">Verdad si es un catálogo no social</param>
		/// <param name="pFiltroContextoWhere">Filtros de contexto</param>
		/// <param name="pListaFiltrosExtra">Lista de filtros extra</param>
		/// <param name="pSemanticos">Lista de formularios semánticos</param>
		public void ObtenerFacetaSinOrdenDBLP(string pProyectoID, FacetadoDS pFacetadoDS, string pClaveFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, bool pExcluida, bool pUsarHilos, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(pProyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.FacetaSinOrdenDBLP;
				cacheModel.ProyectoID = new Guid(pProyectoID);
				cacheModel.ClaveFaceta = pClaveFaceta;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.TipoDisenio = pTipoDisenio;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.EsRango = pEsRango;
				cacheModel.ListaRangos = pListaRangos;
				cacheModel.Excluida = pExcluida;
				cacheModel.UsarHilos = pUsarHilos;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerFacetaSinOrdenDBLP(pProyectoID, facetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, new Guid(pProyectoID)))
					{
						AgregarBusquedaACache(clave, facetadoDS, new Guid(pProyectoID));
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerFacetaSinOrdenDBLP(pProyectoID, pFacetadoDS, pClaveFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pUsarHilos);
			}
		}

		public void ObtenerFacetaRenombrarSiContieneSufijo(string pProyectoID, FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, TipoDisenio pTipoDisenio, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pEsRango, List<int> pListaRangos, string pSufijo, bool pExcluida, bool pInmutable, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(pProyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Facetas;
				cacheModel.ProyectoID = new Guid(pProyectoID);
				cacheModel.NombreFaceta = pNombreFaceta;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.TipoDisenio = pTipoDisenio;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.EsRango = pEsRango;
				cacheModel.ListaRangos = pListaRangos;
				cacheModel.Excluida = pExcluida;
				cacheModel.Inmutable = pInmutable;

				string clave = GenerarClaveCacheBusqueda(cacheModel);

				FacetadoDS facetadoDS = null;
				if (ComprobarSiClaveExiste($"{clave}{pSufijo}"))
				{
					RenombrarClave($"{clave}{pSufijo}", clave);
					facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;
				}
				else
				{
					facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

					if (facetadoDS == null)
					{
						facetadoDS = new FacetadoDS();
						DateTime horaInicio = DateTime.Now;
						FacetadoCN.ObtenerFaceta(pProyectoID, facetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pInmutable);
						DateTime horaFin = DateTime.Now;

						if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, new Guid(pProyectoID)))
						{
							AgregarBusquedaACache(clave, facetadoDS, new Guid(pProyectoID));
						}
					}
					else
					{
						RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
					}
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerFaceta(pProyectoID, pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pTipoDisenio, pInicio, pLimite, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pEsRango, pListaRangos, pExcluida, pInmutable);
			}
		}

		public void ObtienePersonasExacto(FacetadoDS pFacetadoDS, bool ascOdes, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, int pInicio, int pLimite, Guid pProyectoID, bool pEsIdentidadInvitada, bool pEsUsuarioInvitado, Guid pIdentidadID, IAvailableServices pAvailableServices)
		{
			ObtienePersonasExacto(pFacetadoDS, ascOdes, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pProyectoID.Equals(ProyectoAD.MyGnoss), !pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID, pInicio, pLimite, pProyectoID, pAvailableServices);
		}

		public void ObtienePersonasExacto(FacetadoDS pFacetadoDS, bool ascOdes, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEsMyGnoss, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, int pInicio, int pLimite, Guid pProyectoID, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.PersonasExacto;
				cacheModel.Descendente = ascOdes;
				cacheModel.TipoFiltro = pTipoFiltro;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEsMyGnoss;
				cacheModel.EstaEnProyecto = pEstaEnProyecto;
				cacheModel.EsInvitado = pEsUsuarioInvitado;
				cacheModel.IdentidadID = pIdentidadID.ToString();
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.ProyectoID = pProyectoID;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;
				
				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtienePersonasExacto(facetadoDS, ascOdes, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEsMyGnoss, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID, pInicio, pLimite);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
					{
						AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtienePersonasExacto(pFacetadoDS, ascOdes, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEsMyGnoss, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID, pInicio, pLimite);
			}
		}

		public string ObtenerResultadosBusqueda(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, List<Guid> pListaExcluidos, string pLanguageCode, Guid pProyectoID, IAvailableServices pAvailableServices, bool pUsarAfinidad = false)
		{
			return ObtenerResultadosBusqueda(pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar, pPermitirRecursosPrivados, true, TiposAlgoritmoTransformacion.Ninguno, pFiltrosSearchPersonalizados, pEsMovil, pListaExcluidos, pLanguageCode, pProyectoID, pAvailableServices, pUsarAfinidad);
		}

		public string ObtenerResultadosBusqueda(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, List<Guid> pListaExcluidos, string pLanguageCode, Guid pProyectoID, IAvailableServices pAvailableServices, bool pUsarAfinidad = false)
		{
			string clave = "";

			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Resultados;
				cacheModel.Descendente = pDescendente;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.EsMovil = pEsMovil;
				cacheModel.FiltroContextoOrderBy = pFiltroContextoOrderBy;
				cacheModel.FiltroContextoPesoMinimo = pFiltroContextoPesoMinimo;
				cacheModel.FiltroContextoSelect = pFiltroContextoSelect;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.NamespacesExtra = pNamespacesExtra;
				cacheModel.OmitirPalabrasNoRelevantesSearch = pOmitirPalabrasNoRelevantesSearch;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.ResultadosEliminar = pResultadosEliminar;
				cacheModel.TipoAlgoritmoTransformacion = pTipoAlgoritmoTransformacion;
				cacheModel.TipoFiltro = pTipoFiltro;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.LanguageCode = pLanguageCode;

				clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerResultadosBusqueda(pDescendente, facetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pTipoAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pLanguageCode, pEsMovil, pListaExcluidos, pUsarAfinidad);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
					{
						AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerResultadosBusqueda(pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pTipoAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pLanguageCode, pEsMovil, pListaExcluidos, pUsarAfinidad);
			}

			return clave;
		}

		private Dictionary<string, string> ObtenerClavesResultados(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();

			// TODO: Crear una configuración
			pEstaEnMyGnoss = false;
			pEsMiembroComunidad = false;
			pEsInvitado = true;
			pIdentidadID = UsuarioAD.Invitado.ToString();

			string clave = $"{FacetadoCN.GrafoID}//{pDescendente}{pTipoFiltro}{pEstaEnMyGnoss}{pEsMiembroComunidad}{pEsInvitado}{pIdentidadID}{pInicio}{pLimite}{pFiltroContextoSelect}{pFiltroContextoWhere}{pFiltroContextoOrderBy}";

			string claveParametros = $"{FacetadoCN.GrafoID}//{pDescendente}{pTipoFiltro}{pEstaEnMyGnoss}{pEsMiembroComunidad}{pEsInvitado}{pIdentidadID}{pInicio}{pLimite}{pFiltroContextoSelect}{pFiltroContextoWhere}{pFiltroContextoOrderBy}";

			foreach (string filtro in pListaFiltros.Keys.OrderBy(item => item))
			{
				bool aniadir = true;
				if (pFiltrosSearchPersonalizados.ContainsKey(filtro))
				{
					string valores = $"{pFiltrosSearchPersonalizados[filtro].Item1}{pFiltrosSearchPersonalizados[filtro].Item2}{pFiltrosSearchPersonalizados[filtro].Item3}{pFiltrosSearchPersonalizados[filtro].Item4}";

					foreach (string parametro in PARAMETROS_EXTRA_CONSULTAS_VIRTUOSO)
					{
						if (valores.Contains(parametro))
						{
							aniadir = false;
						}
					}
					if (aniadir)
					{
						string hash = StringToHash(valores);
						clave += $"_{filtro}{hash}";
						claveParametros += $"_{filtro}{hash}";
					}
				}
				else
				{
					foreach (string filtroInt in pListaFiltros[filtro].OrderBy(item => item))
					{
						clave += $"_{filtro}_{filtroInt}";
						claveParametros += $"_{filtro}_{filtroInt}";
					}
				}
			}

			foreach (string filtroExtra in pListaFiltrosExtra.OrderBy(item => item))
			{
				clave += "_" + filtroExtra;
				claveParametros += "_" + filtroExtra;
			}
			clave = $"_resultsearch_{FacetadoCN.GrafoID}_{clave}";
			claveParametros = $"pmresultsearch_{FacetadoCN.GrafoID}__{claveParametros}";
			dictionary.Add("clave", clave);
			dictionary.Add("claveParametros", claveParametros);
			return dictionary;
		}

		public bool ExisteResultadosBusquedaCache(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTipoAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, Guid pProyectoID)
		{
			bool exist = false;
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Resultados;
				cacheModel.Descendente = pDescendente;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.TipoFiltro = pTipoFiltro;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoSelect = pFiltroContextoSelect;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.FiltroContextoOrderBy = pFiltroContextoOrderBy;
				cacheModel.FiltroContextoPesoMinimo = pFiltroContextoPesoMinimo;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.NamespacesExtra = pNamespacesExtra;
				cacheModel.ResultadosEliminar = pResultadosEliminar;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.OmitirPalabrasNoRelevantesSearch = pOmitirPalabrasNoRelevantesSearch;
				cacheModel.TipoAlgoritmoTransformacion = pTipoAlgoritmoTransformacion;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.EsMovil = pEsMovil;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				exist = ExisteClaveEnCache(clave);
			}
			return exist;
		}

		static string ByteArrayToString(byte[] arrInput)
		{
			int i;
			StringBuilder sOutput = new StringBuilder(arrInput.Length);
			for (i = 0; i < arrInput.Length; i++)
			{
				sOutput.Append(arrInput[i].ToString("X2"));
			}
			return sOutput.ToString();
		}

		public static string StringToHash(string data)
		{
			byte[] tmpSource = ASCIIEncoding.ASCII.GetBytes(data);
			byte[] tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
			string hash = ByteArrayToString(tmpHash);
			return hash;
		}

		public string ObtenerResultadosBusquedaRenombrarSiContieneSufijo(bool pDescendente, FacetadoDS pFacetadoDS, string pTipoFiltro, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, int pFiltroContextoPesoMinimo, TipoProyecto pTipoProyecto, string pNamespacesExtra, string pResultadosEliminar, string pSufijo, Guid pProyectoID, IAvailableServices pAvailableServices)
		{
			string clave = "";
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Resultados;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.Descendente = pDescendente;
				cacheModel.TipoFiltro = pTipoFiltro;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoSelect = pFiltroContextoSelect;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.FiltroContextoOrderBy = pFiltroContextoOrderBy;
				cacheModel.FiltroContextoPesoMinimo = pFiltroContextoPesoMinimo;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.NamespacesExtra = pNamespacesExtra;
				cacheModel.ResultadosEliminar = pResultadosEliminar;

				clave = GenerarClaveCacheBusqueda(cacheModel);

				FacetadoDS facetadoDS = null;
				if (ComprobarSiClaveExiste($"{clave}{pSufijo}"))
				{
					RenombrarClave($"{clave}{pSufijo}", clave);
					facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;
				}
				else
				{
					facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

					if (facetadoDS == null)
					{
						facetadoDS = new FacetadoDS();
						DateTime horaInicio = DateTime.Now;
						FacetadoCN.ObtenerResultadosBusqueda(pDescendente, facetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar);
						DateTime horaFin = DateTime.Now;

						if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
						{
							AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
						}
					}
					else
					{
						RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
					}
				}
				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerResultadosBusqueda(pDescendente, pFacetadoDS, pTipoFiltro, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pFiltroContextoPesoMinimo, pTipoProyecto, pNamespacesExtra, pResultadosEliminar);
			}

			return clave;
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
		public void ObtenerResultadosBusquedaFormatoMapa(FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, DataWrapperFacetas pFiltroMapaDataWrapper, bool pPermitirRecursosPrivados, TipoBusqueda pTipoBusqueda, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, Guid pProyectoID, string pLanguageCode, IAvailableServices pAvailableServices)
		{
			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
			List<PresentacionMapaSemantico> listaPresentacionMapaSemantico = null;
			if (pListaFiltros.ContainsKey("rdf:type"))
			{
				listaPresentacionMapaSemantico = proyectoCN.ObtenerListaPresentacionMapaSemantico(pProyectoID, pListaFiltros["rdf:type"].FirstOrDefault());
			}

			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.ResultadosFormatoMapa;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoSelect = pFiltroContextoSelect;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.FiltroContextoOrderBy = pFiltroContextoOrderBy;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.NamespacesExtra = pNamespaceExtra;
				cacheModel.ResultadosEliminar = pResultadosEliminar;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.TipoBusqueda = pTipoBusqueda;
				cacheModel.EsMovil = pEsMovil;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.LanguageCode = pLanguageCode;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerResultadosBusquedaFormatoMapa(facetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pFiltroMapaDataWrapper, pPermitirRecursosPrivados, pTipoBusqueda, pEsMovil, pFiltrosSearchPersonalizados, listaPresentacionMapaSemantico, pLanguageCode);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
					{
						AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerResultadosBusquedaFormatoMapa(pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pFiltroMapaDataWrapper, pPermitirRecursosPrivados, pTipoBusqueda, pEsMovil, pFiltrosSearchPersonalizados, listaPresentacionMapaSemantico, pLanguageCode);
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
		/// <param name="pResultadosEliminar"></param>
		/// <param name="pSelectChart">Select chart</param>
		/// <param name="pFiltroChart">Filtros chart</param>
		public void ObtenerResultadosBusquedaFormatoChart(FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoSelect, string pFiltroContextoWhere, string pFiltroContextoOrderBy, TipoProyecto pTipoProyecto, string pNamespaceExtra, string pResultadosEliminar, string pSelectChart, string pFiltroChart, bool pPermitirRecursosPrivados, bool pEsMovil, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, Guid pProyectoID)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.ResultadosFormatoChart;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.OrganizacionID = ProyectoAD.MetaOrganizacion;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.EsMovil = pEsMovil;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.FiltroContextoSelect = pFiltroContextoSelect;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.FiltroContextoOrderBy = pFiltroContextoOrderBy;
				cacheModel.NamespacesExtra = pNamespaceExtra;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.ResultadosEliminar = pResultadosEliminar;
				cacheModel.SelectChart = pSelectChart;
				cacheModel.FiltroChart = pFiltroChart;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerResultadosBusquedaFormatoChart(facetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pSelectChart, pFiltroChart, pPermitirRecursosPrivados, pEsMovil, pFiltrosSearchPersonalizados);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
					{
						AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
					}
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerResultadosBusquedaFormatoChart(pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoSelect, pFiltroContextoWhere, pFiltroContextoOrderBy, pTipoProyecto, pNamespaceExtra, pResultadosEliminar, pSelectChart, pFiltroChart, pPermitirRecursosPrivados, pEsMovil, pFiltrosSearchPersonalizados);
			}
		}

		public Dictionary<string, string> ObtieneClavesNumeroResultadosCache(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			// TODO: Crear una configuración
			pEstaEnMyGnoss = false;
			pEsMiembroComunidad = false;
			pEsInvitado = true;
			pIdentidadID = UsuarioAD.Invitado.ToString();

			string clave = $"_numeroresultados_{FacetadoCN.GrafoID}_{FacetadoCN.GrafoID}{pNombreFaceta}{pEstaEnMyGnoss}{pEsMiembroComunidad}{pEsInvitado}{pIdentidadID}{pFiltroContextoWhere}false";
			// Le pongo false al final para no perder las cachés de DBLP por el parámetro pEsCatalogoNoSocialConUnTipo
			string claveParametros = $"pmnumeroresultados_{FacetadoCN.GrafoID}_{FacetadoCN.GrafoID}{pNombreFaceta}{pEstaEnMyGnoss}{pEsMiembroComunidad}{pEsInvitado}{pIdentidadID}{pFiltroContextoWhere}false"; // Le pongo false al final para no perder las cachés de DBLP por el parámetro pEsCatalogoNoSocialConUnTipo

			foreach (string filtro in pListaFiltros.Keys.OrderBy(item => item))
			{
				bool aniadir = true;
				if (pFiltrosSearchPersonalizados.ContainsKey(filtro))
				{
					string valores = $"{pFiltrosSearchPersonalizados[filtro].Item1}{pFiltrosSearchPersonalizados[filtro].Item2}{pFiltrosSearchPersonalizados[filtro].Item3}{pFiltrosSearchPersonalizados[filtro].Item4}";

					foreach (string parametro in PARAMETROS_EXTRA_CONSULTAS_VIRTUOSO)
					{
						if (valores.Contains(parametro))
						{
							aniadir = false;
						}
					}
					if (aniadir)
					{
						string hash = StringToHash(valores);
						clave += $"_{filtro}{hash}";
						claveParametros += $"_{filtro}{hash}";
					}
				}
				else
				{
					foreach (string filtroInt in pListaFiltros[filtro].OrderBy(item => item))
					{
						clave += $"_{filtro}_{filtroInt}";
						claveParametros += $"_{filtro}_{filtroInt}";
					}
				}
			}

			foreach (string filtroExtra in pListaFiltrosExtra)
			{
				clave += $"_{filtroExtra}";
			}
			dictionary.Add("clave", clave);
			dictionary.Add("claveParametros", claveParametros);
			return dictionary;
		}

		public bool ExisteNumeroResultadosEnCache(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, Guid pProyectoID)
		{
			bool exist = false;
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.NumeroResultados;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.NombreFaceta = pNombreFaceta;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.OmitirPalabrasNoRelevantesSearch = pOmitirPalabrasNoRelevantesSearch;
				cacheModel.TiposAlgoritmoTransformacion = pTiposAlgoritmoTransformacion;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.EsMovil = pEsMovil;
				
				string clave = GenerarClaveCacheBusqueda(cacheModel);
				exist = ExisteClaveEnCache(clave);
			}

			return exist;
		}

		public void ObtieneNumeroResultados(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaEnMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, List<string> pSemanticos, string pFiltroContextoWhere, TipoProyecto pTipoProyecto, bool pPermitirRecursosPrivados, bool pOmitirPalabrasNoRelevantesSearch, TiposAlgoritmoTransformacion pTiposAlgoritmoTransformacion, Dictionary<string, Tuple<string, string, string, bool>> pFiltrosSearchPersonalizados, bool pEsMovil, List<Guid> pListaExcluidos, Guid pProyectoID, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.NumeroResultados;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.NombreFaceta = pNombreFaceta;
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaEnMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltroContextoWhere = pFiltroContextoWhere;
				cacheModel.TipoProyecto = pTipoProyecto;
				cacheModel.PermitirRecursosPrivados = pPermitirRecursosPrivados;
				cacheModel.OmitirPalabrasNoRelevantesSearch = pOmitirPalabrasNoRelevantesSearch;
				cacheModel.TiposAlgoritmoTransformacion = pTiposAlgoritmoTransformacion;
				cacheModel.FiltrosSearchPersonalizados = pFiltrosSearchPersonalizados;
				cacheModel.EsMovil = pEsMovil;
				cacheModel.ListaExcluidos = pListaExcluidos;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtieneNumeroResultados(facetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pTiposAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pEsMovil, pListaExcluidos);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
					{
						AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtieneNumeroResultados(pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pEstaEnMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pSemanticos, pFiltroContextoWhere, pTipoProyecto, pPermitirRecursosPrivados, pOmitirPalabrasNoRelevantesSearch, pTiposAlgoritmoTransformacion, pFiltrosSearchPersonalizados, pEsMovil);
			}
		}

		/// <summary>
		/// Obtiene las comunidades que le pueden interesar a un perfil
		/// </summary>     
		/// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
		public DataSet ComunidadesQueTePuedanInteresar(Guid pIdentidadMyGnoss, int pInicio, int pLimite, bool pObtenerComunidades, Dictionary<string, List<string>> pListaFiltros)
		{
			return FacetadoCN.ComunidadesQueTePuedanInteresar(pIdentidadMyGnoss, pInicio, pLimite, pObtenerComunidades, pListaFiltros);
		}

		/// <summary>
		/// Obtiene el número de comunidades que le pueden interesar a un perfil
		/// </summary>     
		/// <param name="pIdentidadMyGnoss">Identidad en MyGnoss del perfil</param>     
		public DataSet NumeroComunidadesQueTePuedanInteresar(Guid pIdentidadMyGnoss, Dictionary<string, List<string>> pListaFiltros)
		{
			return FacetadoCN.NumeroComunidadesQueTePuedanInteresar(pIdentidadMyGnoss, pListaFiltros);
		}

		public void ObtenerAutocompletar(string proyectoID, FacetadoDS pFacetadoDS, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, bool pEstaMyGnoss, bool pEsMiembroComunidad, bool pEsInvitado, string pIdentidadID, int pInicio, int pLimite, List<string> pSemanticos, string pFiltrosContexto, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(new Guid(proyectoID)))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.Autocompletar;
				cacheModel.ProyectoID = new Guid(proyectoID);
				cacheModel.ListaFiltros = pListaFiltros;
				cacheModel.ListaFiltrosExtra = pListaFiltrosExtra;
				cacheModel.EstaEnMyGnoss = pEstaMyGnoss;
				cacheModel.EsMiembroComunidad = pEsMiembroComunidad;
				cacheModel.EsInvitado = pEsInvitado;
				cacheModel.IdentidadID = pIdentidadID;
				cacheModel.Inicio = pInicio;
				cacheModel.Limite = pLimite;
				cacheModel.Semanticos = pSemanticos;
				cacheModel.FiltrosContexto = pFiltrosContexto;
				cacheModel.FiltroAutocompletar = pFiltrosContexto;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtenerAutocompletar(proyectoID, facetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltrosContexto);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, new Guid(proyectoID)))
					{
						AgregarBusquedaACache(clave, facetadoDS, new Guid(proyectoID));
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtenerAutocompletar(proyectoID, pFacetadoDS, pListaFiltros, pListaFiltrosExtra, pEstaMyGnoss, pEsMiembroComunidad, pEsInvitado, pIdentidadID, pInicio, pLimite, pSemanticos, pFiltrosContexto);
			}
		}

		public void ObtieneDatosAutocompletar(string nombregrafo, string filtro, FacetadoDS pFacetadoDS, Guid pProyectoID, IAvailableServices pAvailableServices)
		{
			if (HayCacheSparql && EstanCachesBusquedaActivas(pProyectoID))
			{
				CacheConsultasCostosas cacheModel = new CacheConsultasCostosas();
				cacheModel.TipoDeCache = TipoCacheCostosa.DatosAutocompletar;
				cacheModel.ProyectoID = pProyectoID;
				cacheModel.NombreGrafo = nombregrafo;
				cacheModel.FiltroAutocompletar = filtro;

				string clave = GenerarClaveCacheBusqueda(cacheModel);
				FacetadoDS facetadoDS = ObtenerObjetoDeCache(clave, typeof(FacetadoDS)) as FacetadoDS;

				if (facetadoDS == null)
				{
					facetadoDS = new FacetadoDS();
					DateTime horaInicio = DateTime.Now;
					FacetadoCN.ObtieneDatosAutocompletar(nombregrafo, filtro, facetadoDS);
					DateTime horaFin = DateTime.Now;

					if (ComprobarConsultaSuperaTiempoConfigurado(horaInicio, horaFin, pProyectoID))
					{
						AgregarBusquedaACache(clave, facetadoDS, pProyectoID);
					}
				}
				else
				{
					RecalcularCachesBusqueda(clave, cacheModel, pAvailableServices);
				}

				pFacetadoDS.Merge(facetadoDS);
			}
			else
			{
				FacetadoCN.ObtieneDatosAutocompletar(nombregrafo, filtro, pFacetadoDS);
			}
		}

		/// <summary>
		/// Invalida los resultados y las facetas de una determinada búsqueda
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		public void InvalidarResultadosYFacetasDeBusquedaEnProyecto(Guid pProyectoID, string pTipoBusqueda)
		{
			InvalidarResultadosYFacetasDeBusquedaEnProyecto(pProyectoID, pTipoBusqueda, false);
		}

		public bool PerteneceClaveAUsuarioConPrivados(string pClave, Guid pProyectoID)
		{
			string stringProyectoID = pProyectoID.ToString();
			string subClave = pClave.Substring(pClave.IndexOf(stringProyectoID) + stringProyectoID.Length);

			bool perteneceClaveAUsuarioConPrivados = false;
			int i = 0;
			while (subClave.Contains("-"))
			{
				i++;
				subClave = subClave.Substring(subClave.IndexOf("-") + 1);
			}
			if (i >= 4)
			{
				perteneceClaveAUsuarioConPrivados = true;
			}
			return perteneceClaveAUsuarioConPrivados;
		}

		/// <summary>
		/// Invalida los resultados y las facetas de una determinada búsqueda
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pSoloUsuariosConPrivados">Indica si borramos solo la de los usuarios que tengan recursos privados</param>
		public void InvalidarResultadosYFacetasDeBusquedaEnProyecto(Guid pProyectoID, string pTipoBusqueda, bool pSoloUsuariosConPrivados)
		{
			try
			{
				string rawKey = $"{NombresCL.FACETADO}_{pProyectoID}";
				List<string> claves = new List<string>();
				if (ClienteRedisLectura != null)
				{
					claves = ClienteRedisLectura.Keys(ObtenerClaveCache($"{rawKey}*").ToLower()).Result.ToList();
				}

				List<string> clavesElminar = new List<string>();
				foreach (string claveCache in claves)
				{
					if (!pSoloUsuariosConPrivados || PerteneceClaveAUsuarioConPrivados(claveCache, pProyectoID))
					{
						clavesElminar.Add(claveCache);
					}
				}
				InvalidarCachesMultiples(clavesElminar);
			}
			catch (Exception e)
			{
				mLoggingService.GuardarLogError(e, mlogger);
			}
		}

		#region Cache Facetas y resultados

		/// <summary>
		/// Agrega los identificadores de resultados de una determinada búsqueda a la cache (sin caducidad)
		/// </summary>
		/// <param name="pListaResultados">Listado de resultados</param>
		/// <param name="pTipoBusqueda">Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil del usuario que está buscando</param>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pNumeroPagina">Parte de la página que se carga (primera o segunda)</param>
		/// <param name="pOrganizacionID">Identificador de la organización (en caso de que se un perfil de organizacion)</param>
		/// <param name="pEsIdentidadInvitado">Indica si se trata de una identidad invitada</param>
		/// <param name="pParametros">pParametros</param>
		/// <returns></returns>
		public string AgregarListaResultadosDeBusquedaEnProyecto(Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> pListaResultados, string pTipoBusqueda, Guid pPerfilID, Guid pProyectoID, string pNumeroPagina, Guid? pOrganizacionID, bool pEsIdentidadInvitado, string pParametros, bool pEsMovil, bool pEsUsuarioInvitado)
		{
			return AgregarListaResultadosDeBusquedaEnProyecto(pListaResultados, pTipoBusqueda, pPerfilID, pProyectoID, pNumeroPagina, pOrganizacionID, pEsIdentidadInvitado, pParametros, 0.0, false, pEsMovil, pEsUsuarioInvitado);
		}

		/// <summary>
		/// Agrega los identificadores de resultados de una determinada búsqueda a la cache (con caducidad)
		/// </summary>
		/// <param name="pListaResultados">Listado de resultados</param>
		/// <param name="pTipoBusqueda">Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil del usuario que está buscando</param>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pNumeroPagina">Parte de la página que se carga (primera o segunda)</param>
		/// <param name="pOrganizacionID">Identificador de la organización (en caso de que se un perfil de organizacion)</param>
		/// <param name="pEsIdentidadInvitado">Indica si se trata de una identidad invitada</param>
		/// <param name="pParametros">pParametros</param>
		/// <param name="pDuracion">Duración de la caché</param>
		/// <returns></returns>
		public string AgregarListaResultadosDeBusquedaEnProyecto(Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> pListaResultados, string pTipoBusqueda, Guid pPerfilID, Guid pProyectoID, string pNumeroPagina, Guid? pOrganizacionID, bool pEsIdentidadInvitado, string pParametros, double pDuracion, bool pFacetaPrivadaGrupo, bool pEsMovil, bool pEsUsuarioInvitado)
		{
			string rawKey = ObtenerKeyBusquedaFacetadoMVC(pProyectoID, pTipoBusqueda, pPerfilID, false, pEsUsuarioInvitado);
			if (pOrganizacionID.HasValue && !rawKey.Contains(NombresCL.PRIMEROSRECURSOS))
			{
				rawKey += $"_{pOrganizacionID.Value}";
			}
			if (pEsMovil)
			{
				rawKey += "_mobile";
			}
			if (pEsIdentidadInvitado)
			{
				rawKey += "_invitado";
			}
			rawKey += $"_{pNumeroPagina}";

			if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !string.IsNullOrEmpty(pParametros))
			{
				rawKey += $"_{pParametros}";
			}

			string tiempos = "";

			if (pListaResultados != null && pListaResultados.Item1 > 0)
			{
				if (pDuracion != 0.0)
				{
					AgregarObjetoCache(rawKey, pListaResultados, pDuracion);
				}
				else
				{
					AgregarObjetoCache(rawKey, pListaResultados);
				}
			}

			return tiempos;
		}

		/// <summary>
		/// Agrega el Data set de los tesauros del proyecto.
		/// </summary>
		/// <param name="pDStesauro">DataSet de los tesauros</param>
		/// <param name="pProyectoID">ProyectoID</param>
		/// <param name="pClaveFaceta">Clave de la faceta</param>
		/// <returns></returns>
		public string AgregarTesauroSemanticoDeBusquedaEnProyecto(FacetadoDS pDStesauro, string pProyectoID, string pClaveFaceta, string pIdioma = "")
		{
			string rawKey = $"{ObtenerKeyTesauroSemanticoDeBusqueda(pProyectoID, pClaveFaceta)}_{pIdioma}";

			string tiempos = "";

			if (pDStesauro != null)
			{
				AgregarObjetoCache(rawKey, pDStesauro);
			}

			return tiempos;
		}

		/// <summary>
		/// Obtiene el numero total de resultados y los identificadores de resultados de una determinada busqueda
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil del usuario que está buscando</param>
		/// <param name="pNumeroPagina">Parte de la página que se carga (primera o segunda)</param>
		/// <param name="pOrganizacionID">Identificador de la organización (en caso de que se un perfil de organizacion)</param>
		/// <param name="pEsIdentidadInvitado">Indica si se trata de una identidad invitada</param>
		/// <param name="pParametros">pParametros</param>
		/// <returns>Identificadores del resultado de la busqueda con su tipo</returns>
		public Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> ObtenerListaResultadosDeBusquedaEnProyecto(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, string pNumeroPagina, Guid? pOrganizacionID, bool pEsIdentidadInvitado, string pParametros, bool esMovil, bool pEsUsuarioInvitado)
		{
			string rawKey = ObtenerKeyBusquedaFacetadoMVC(pProyectoID, pTipoBusqueda, pPerfilID, false, pEsUsuarioInvitado);
			if (pOrganizacionID.HasValue && !rawKey.Contains(NombresCL.PRIMEROSRECURSOS))
			{
				rawKey += $"_{pOrganizacionID.Value}";
			}
			if (esMovil && !pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.PersonasYOrganizaciones)))
			{
				rawKey += "_mobile";
			}
			if (pEsIdentidadInvitado)
			{
				rawKey += "_invitado";
			}
			rawKey += $"_{pNumeroPagina}";

			if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !string.IsNullOrEmpty(pParametros))
			{
				rawKey += $"_{pParametros}";
			}

			Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> listaRecursos = ObtenerObjetoDeCache(rawKey, typeof(Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>>)) as Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>>;
			return listaRecursos;
		}

		/// <summary>
		/// Agrega el modelo de facetas de una determinada busqueda (sin caducidad)
		/// </summary>
		/// <param name="pListaFacetas">Modelo de facetas</param>
		/// <param name="pTipoBusqueda">>Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Perfil del usuario que está buscando</param>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pNumeroFacetas">Número de facetas</param>
		/// <param name="pEsUsuarioInvitado">Indica si se trata de un usuario invitado</param>
		/// <param name="pIdioma">Idioma de las facetas</param>
		/// <param name="pHomeProyecto">Indica si se trata de la home de un proyecto</param>
		/// <param name="pParametrosClaveExtra">ParametrosClaveExtra</param>
		/// <param name="pOrganizacionID">Identificador de la organización (en caso de que se un perfil de organizacion)</param>
		/// <param name="pBusquedaTipoMapa">Indica si se trata de una busqueda de tipo mapa</param>
		/// <param name="pParametros">Parametros</param>
		public void AgregarModeloFacetasDeBusquedaEnProyectoACache(List<FacetModel> pListaFacetas, string pTipoBusqueda, Guid pPerfilID, Guid pProyectoID, int pNumeroFacetas, bool pEsUsuarioInvitado, string pIdioma, bool pHomeProyecto, string pParametrosClaveExtra, Guid? pOrganizacionID, bool pBusquedaTipoMapa, string pParametros, bool pEsMovil)
		{
			AgregarModeloFacetasDeBusquedaEnProyectoACache(pListaFacetas, pTipoBusqueda, pPerfilID, pProyectoID, pNumeroFacetas, pEsUsuarioInvitado, pIdioma, pHomeProyecto, pParametrosClaveExtra, pOrganizacionID, pBusquedaTipoMapa, pParametros, 0.0, false, pEsMovil);
		}

		/// <summary>
		/// Agrega el modelo de facetas de una determinada busqueda (con caducidad)
		/// </summary>
		/// <param name="pListaFacetas">Modelo de facetas</param>
		/// <param name="pTipoBusqueda">>Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Perfil del usuario que está buscando</param>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pNumeroFacetas">Número de facetas</param>
		/// <param name="pEsUsuarioInvitado">Indica si se trata de un usuario invitado</param>
		/// <param name="pIdioma">Idioma de las facetas</param>
		/// <param name="pHomeProyecto">Indica si se trata de la home de un proyecto</param>
		/// <param name="pParametrosClaveExtra">ParametrosClaveExtra</param>
		/// <param name="pOrganizacionID">Identificador de la organización (en caso de que se un perfil de organizacion)</param>
		/// <param name="pBusquedaTipoMapa">Indica si se trata de una busqueda de tipo mapa</param>
		/// <param name="pParametros">Parametros</param>
		/// <param name="pDuracion">Duración de la caché</param>
		public void AgregarModeloFacetasDeBusquedaEnProyectoACache(List<FacetModel> pListaFacetas, string pTipoBusqueda, Guid pPerfilID, Guid pProyectoID, int pNumeroFacetas, bool pEsUsuarioInvitado, string pIdioma, bool pHomeProyecto, string pParametrosClaveExtra, Guid? pOrganizacionID, bool pBusquedaTipoMapa, string pParametros, double pDuracion, bool pFacetaPrivadaGrupo, bool pEsMovil)
		{
			if (pListaFacetas.Count > 0)
			{
				string rawKey = ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, pPerfilID, true, pNumeroFacetas, pEsUsuarioInvitado, pParametrosClaveExtra, pFacetaPrivadaGrupo);
				string home = "";
				if (pHomeProyecto)
				{
					home = "_home";
				}

				if (pOrganizacionID.HasValue && !rawKey.Contains(NombresCL.PRIMEROSRECURSOS))
				{
					rawKey += $"_{pOrganizacionID.Value}";
				}

				rawKey += $"_{pIdioma}{home}";

				if (pBusquedaTipoMapa)
				{
					rawKey += "_mapa";
				}
				if (pEsMovil)
				{
					rawKey += "_mobile";
				}

				if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !string.IsNullOrEmpty(pParametros))
				{
					rawKey += $"_{pParametros}";
				}

				if (pDuracion != 0.0)
				{
					AgregarObjetoCache(rawKey, pListaFacetas, pDuracion);
				}
				else
				{
					AgregarObjetoCache(rawKey, pListaFacetas);
				}
			}
		}

		/// <summary>
		/// Obtiene el modelo de facetas de una determinada busqueda
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de la búsqueda</param>
		/// <param name="pPerfilID">Perfil del usuario que está buscando</param>
		/// <param name="pNumeroFacetas">Número de facetas</param>
		/// <param name="pEsUsuarioInvitado">Indica si se trata de un usuario invitado</param>
		/// <param name="pIdioma">Idioma de las facetas</param>
		/// <param name="pHomeProyecto">Indica si se trata de la home de un proyecto</param>
		/// <param name="pParametrosClaveExtra">ParametrosClaveExtra</param>
		/// <param name="pOrganizacionID">Identificador de la organización (en caso de que se un perfil de organizacion)</param>
		/// <param name="pBusquedaTipoMapa">Indica si se trata de una busqueda de tipo mapa</param>
		/// <param name="pParametros">Parametros</param>
		/// <returns></returns>
		public List<FacetModel> ObtenerModeloFacetasDeBusquedaEnProyecto(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, int pNumeroFacetas, bool pEsUsuarioInvitado, string pIdioma, bool pHomeProyecto, string pParametrosClaveExtra, Guid? pOrganizacionID, bool pBusquedaTipoMapa, string pParametros, bool pFacetaPrivadaGrupo, bool pEsMovil)
		{
			string home = "";
			if (pHomeProyecto)
			{
				home = "_home";
			}

			string rawKey = ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, pPerfilID, true, pNumeroFacetas, pEsUsuarioInvitado, pParametrosClaveExtra, pFacetaPrivadaGrupo);
			if (pOrganizacionID.HasValue && !rawKey.Contains(NombresCL.PRIMEROSRECURSOS))
			{
				rawKey += $"_{pOrganizacionID.Value}";
			}
			rawKey += $"_{pIdioma}{home}";

			if (pBusquedaTipoMapa)
			{
				rawKey += "_mapa";
			}
			if (pEsMovil)
			{
				rawKey += "_mobile";
			}

			if (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) && !string.IsNullOrEmpty(pParametros))
			{
				rawKey += $"_{pParametros}";
			}

			List<FacetModel> facetas = ObtenerObjetoDeCache(rawKey, typeof(List<FacetModel>)) as List<FacetModel>;

			return facetas;
		}

		/// <summary>
		/// Obtiene el modelo del tesauro semantico del proyecto por clave. 
		/// Se guarda en cache local, si no existe lo busca en redis y lo agrega a cache local de obtenerlo.
		/// Los tesauros son muy pesados y tardan en descargarse de redis.
		/// <paramref name="pProyectoID">Identificador del proyecto</paramref>
		/// <paramref name="claveFaceta">Clave de la faceta del tesauro que se va a almacenar en cache</paramref>>
		/// <paramref name="pIdioma">Idioma en el que se solicitan</paramref>
		/// <returns></returns>
		public FacetadoDS ObtenerModeloTesauroSemanticoDeBusquedaEnProyecto(string pProyectoID, string claveFaceta, string pIdioma)
		{
			string rawKey = $"{ObtenerKeyTesauroSemanticoDeBusqueda(pProyectoID, claveFaceta)}_{pIdioma}";

			FacetadoDS tesauroSemanticoProyecto = ObtenerObjetoDeCacheLocal(rawKey) as FacetadoDS;

			if (tesauroSemanticoProyecto == null)
			{
				tesauroSemanticoProyecto = ObtenerObjetoDeCache(rawKey, typeof(FacetadoDS)) as FacetadoDS;

				if (tesauroSemanticoProyecto != null)
				{
					AgregarObjetoCacheLocal(new Guid(pProyectoID), rawKey, tesauroSemanticoProyecto, true);
				}
			}

			return tesauroSemanticoProyecto;
		}

		/// <summary>
		/// Obtiene la Key de la cache para el tesauro semantico.
		/// </summary>
		/// <param name="pProyectoID">Id del proyecto</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyTesauroSemanticoDeBusqueda(string pProyectoID, string claveFaceta)
		{
			string rawKey = $"{NombresCL.TESAURO}_{pProyectoID}_{claveFaceta}";

			return rawKey;
		}

		#endregion

		/// <summary>
		/// Obtiene la Key de la cache para la búsqueda del facetado.
		/// </summary>
		/// <param name="pProyectoID">c de proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil actual</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyBusquedaFacetado(Guid pProyectoID, string pTipoBusqueda, bool pEsUsuarioInvitado)
		{
			return ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, Guid.Empty, false, pEsUsuarioInvitado);
		}

		/// <summary>
		/// Obtiene la Key de la cache para la búsqueda del facetado.
		/// </summary>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pPerfilID">ID del perfil actual</param>
		/// <param name="pProyectoID">Identificador del proyecto en el que se busca</param>
		/// <param name="pSoloFacetas">Verdad si solo se buscan facetas</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyBusquedaFacetado(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, bool pSoloFacetas, bool pEsUsuarioInvitado)
		{
			return ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, pPerfilID, pSoloFacetas, 0, pEsUsuarioInvitado, "", false);
		}

		/// <summary>
		/// Obtiene la Key de la cache para la búsqueda del facetado.
		/// </summary>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pPerfilID">ID del perfil actual</param>
		/// <param name="pProyectoID">Identificador del proyecto en el que se busca</param>
		/// <param name="pSoloFacetas">Verdad si solo se buscan facetas</param>
		/// <param name="pParametrosClaveExtra">Parametros extra para la clave de caché</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyBusquedaFacetado(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, bool pSoloFacetas, int pNumeroFacetas, bool pEsUsuarioInvitado, string pParametrosClaveExtra, bool pFacetaPrivadaGrupo, bool pEsMovil = false)
		{
			string rawKey = $"{NombresCL.FACETADO}_{pProyectoID}_{pTipoBusqueda}";

			if (pSoloFacetas)
			{
				rawKey += $"_facetas_{pNumeroFacetas}";
			}

			if (pEsMovil)
			{
				rawKey += "_mobile";
			}
			if (pEsUsuarioInvitado)
			{
				rawKey += "_Invitado";
			}

			if (!pPerfilID.Equals(Guid.Empty) && (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) || TienePrivados(pProyectoID, pPerfilID) || pFacetaPrivadaGrupo))
			{
				rawKey = $"{rawKey}_{pPerfilID}";
			}

			if (!string.IsNullOrEmpty(pParametrosClaveExtra))
			{
				rawKey += $"_{pParametrosClaveExtra}";
			}

			return rawKey;
		}

		/// <summary>
		/// Obtiene la Key de la cache para la búsqueda del facetado.
		/// </summary>
		/// <param name="pProyectoID">c de proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pPerfilID">Identificador del perfil actual</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyBusquedaFacetadoMVC(Guid pProyectoID, string pTipoBusqueda, bool pEsUsuarioInvitado)
		{
			return ObtenerKeyBusquedaFacetadoMVC(pProyectoID, pTipoBusqueda, Guid.Empty, false, pEsUsuarioInvitado);
		}

		/// <summary>
		/// Obtiene la Key de la cache para la búsqueda del facetado.
		/// </summary>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pPerfilID">ID del perfil actual</param>
		/// <param name="pProyectoID">Identificador del proyecto en el que se busca</param>
		/// <param name="pSoloFacetas">Verdad si solo se buscan facetas</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyBusquedaFacetadoMVC(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, bool pSoloFacetas, bool pEsUsuarioInvitado)
		{
			return ObtenerKeyBusquedaFacetadoMVC(pProyectoID, pTipoBusqueda, pPerfilID, pSoloFacetas, 0, pEsUsuarioInvitado, "", false);
		}

		/// <summary>
		/// Obtiene la Key de la cache para la búsqueda del facetado.
		/// </summary>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		/// <param name="pPerfilID">ID del perfil actual</param>
		/// <param name="pProyectoID">Identificador del proyecto en el que se busca</param>
		/// <param name="pSoloFacetas">Verdad si solo se buscan facetas</param>
		/// <param name="pParametrosClaveExtra">Parametros extra para la clave de caché</param>
		/// <returns>Key de la cache para la búsqueda del facetado</returns>
		public string ObtenerKeyBusquedaFacetadoMVC(Guid pProyectoID, string pTipoBusqueda, Guid pPerfilID, bool pSoloFacetas, int pNumeroFacetas, bool pEsUsuarioInvitado, string pParametrosClaveExtra, bool pFacetaPrivadaGrupo, bool pEsMovil = false)
		{
			string rawKey = $"{NombresCL.FACETADO}_{pProyectoID}_{pTipoBusqueda}_MVC";

			if (pSoloFacetas)
			{
				rawKey += $"_facetas_{pNumeroFacetas}";
			}

			if (pEsMovil)
			{
				rawKey += "_mobile";
			}
			if (pEsUsuarioInvitado)
			{
				rawKey += "_Invitado";
			}

			if (!pPerfilID.Equals(Guid.Empty) && (pTipoBusqueda.Equals(FacetadoAD.TipoBusquedaToString(TipoBusqueda.Mensajes)) || TienePrivados(pProyectoID, pPerfilID) || pFacetaPrivadaGrupo))
			{
				rawKey = $"{rawKey}_{pPerfilID}";
			}

			if (!string.IsNullOrEmpty(pParametrosClaveExtra))
			{
				rawKey += $"_{pParametrosClaveExtra}";
			}

			return rawKey;
		}

		/// <summary>
		/// Obtiene el número de resultados a partir de la faceta y los filtros pasados como parámetros
		/// </summary>
		/// <param name="pFacetadoDS">Dataset Facetado</param>
		/// <param name="pNombreFaceta">Nombre de la faceta</param>
		/// <param name="pListaFiltros">Lista de filtros</param>
		public void ObtieneNumeroResultados(FacetadoDS pFacetadoDS, string pNombreFaceta, Dictionary<string, List<string>> pListaFiltros, List<string> pListaFiltrosExtra, List<string> pSemanticos, bool pEsMovil, Guid pProyectoID, bool pEsUsuarioInvitado, bool pEsIdentidadInvitada, Guid pIdentidadID)
		{
			string rawKey = $"{NombresCL.FACETADO}_NUMRESULTADOS_{pProyectoID}_{pNombreFaceta}";

			if (pListaFiltros.ContainsKey("rdf:type"))
			{
				foreach (string filtro in pListaFiltros["rdf:type"])
				{
					rawKey += $"_{filtro}";
				}
			}

			// Compruebo si está en la caché
			FacetadoDS facetadoDS = ObtenerObjetoDeCache(rawKey, typeof(FacetadoDS)) as FacetadoDS;
			if (facetadoDS == null)
			{
				// Si no está, lo cargo y lo almaceno en la caché
				FacetadoCN facetadoCN = new FacetadoCN(mUrlIntranet, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FacetadoCN>(), mLoggerFactory);
				facetadoCN.InformacionOntologias = InformacionOntologias;
				facetadoCN.ObtieneNumeroResultados(pFacetadoDS, pNombreFaceta, pListaFiltros, pListaFiltrosExtra, pSemanticos, TiposAlgoritmoTransformacion.Ninguno, pProyectoID, pEsIdentidadInvitada, pEsUsuarioInvitado, pIdentidadID, pEsMovil);
				facetadoCN.Dispose();

				AgregarObjetoCache(rawKey, pFacetadoDS);
			}
			else
			{
				pFacetadoDS.Merge(facetadoDS);
			}
		}

		/// <summary>
		/// Invalida la cache del proyecto pasado como parámetro
		/// </summary>
		/// <param name="pProyectoID">Identificador de proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		public void InvalidarCache(Guid pProyectoID, string pTipoBusqueda, bool pEsUsuarioInvitado)
		{
			InvalidarCache(ObtenerKeyBusquedaFacetado(pProyectoID, pTipoBusqueda, pEsUsuarioInvitado));
		}

		/// <summary>
		/// Invalida la cache del proyecto pasado como parámetro
		/// </summary>
		/// <param name="pProyectoID">Identificador de proyecto</param>
		/// <param name="pTipoBusqueda">Tipo de búsqueda</param>
		public void InvalidarCacheTesauroFaceta(Guid pProyectoID)
		{
			string rawKey = $"{NombresCL.TESAURO}_{pProyectoID}_";
			InvalidarCacheQueContengaCadena(rawKey, false);
		}

		public void InvalidarCachePerfilTieneGrupos(Guid pProyectoID, Guid pPerfilID)
		{
			string rawKey = $"{NombresCL.FACETADO}_tienegrupos{pProyectoID}_{pPerfilID}";

			InvalidarCache(rawKey);
		}

		public void InsertarColaRefrescoCacheConsultasCostosas(CacheConsultasCostosas pCacheConsultasCostosas, IAvailableServices pAvailableServices)
		{
			if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN) && pAvailableServices.CheckIfServiceIsAvailable(pAvailableServices.GetBackServiceCode(BackgroundService.RefreshCacheHeavyQuery), ServiceType.Background))
			{
				using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_REFRESCO_CACHES_BUSQUEDAS, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory))
				{
					rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(pCacheConsultasCostosas));
				}
			}
		}

		private bool EstanCachesBusquedaActivas(Guid pProyectoID)
		{
			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
			bool cachesBusquedaActivas = proyectoCN.EstanCachesDeBusquedasActivas(pProyectoID);
			proyectoCN.Dispose();

			return cachesBusquedaActivas;
		}

		private ConfiguracionCachesCostosas ObtenerConfiguracionCachesBusquedas(Guid pProyectoID)
		{
			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
			ConfiguracionCachesCostosas configuracionCaches = proyectoCN.ObtenerConfiguracionCachesCostosasDeProyecto(pProyectoID);
			proyectoCN.Dispose();

			return configuracionCaches;
		}

		public void AgregarBusquedaACache(string pClave, FacetadoDS pFacetadoDS, Guid pProyectoID)
		{
			ConfiguracionCachesCostosas configuracionCaches = ObtenerConfiguracionCachesBusquedas(pProyectoID);
			if (configuracionCaches != null)
			{
				if (configuracionCaches.CachesAnonimas)
				{
					AgregarObjetoCache(pClave, pFacetadoDS, configuracionCaches.TiempoExpiracion);
				}
				else
				{
					AgregarObjetoCache(pClave, pFacetadoDS, configuracionCaches.TiempoExpiracionCachesDeUsuario);
				}
			}
		}

		public void RecalcularCachesBusqueda(string pClave, CacheConsultasCostosas pCacheModel, IAvailableServices pAvailableServices)
		{
			try
			{
				string clave = ObtenerClaveCache(pClave).ToLower();
				ConfiguracionCachesCostosas configCaches = ObtenerConfiguracionCachesBusquedas(pCacheModel.ProyectoID);
				ConnectionMultiplexer conexion = ConnectionMultiplexer.Connect($"{mConfigService.ObtenerConexionRedisIPMaster("SPARQL")},defaultDatabase={mConfigService.ObtenerConexionRedisBD("SPARQL")}");
				IDatabase db = conexion.GetDatabase();
				
				TimeSpan? ttl = db.KeyTimeToLive(clave);
				long tiempoExpiracion = configCaches.TiempoExpiracion;
				if (!configCaches.CachesAnonimas)
				{
					tiempoExpiracion = configCaches.TiempoExpiracionCachesDeUsuario;
				}

				if (ttl.HasValue && ((tiempoExpiracion - (long)ttl.Value.TotalSeconds) > configCaches.TiempoRecalcularCaches))
				{
					InsertarColaRefrescoCacheConsultasCostosas(pCacheModel, pAvailableServices);
				}

				conexion.Dispose();
			}
			catch (Exception ex)
			{
				mLoggingService.GuardarLogError(ex,$"Error al insertar recalcular las caches de búsqueda",mlogger);
			}
		}

		public bool ComprobarConsultaSuperaTiempoConfigurado(DateTime pHoraInicio, DateTime pHoraFin, Guid pProyectoID)
		{
			ConfiguracionCachesCostosas configCaches = ObtenerConfiguracionCachesBusquedas(pProyectoID);
			long valorSegundos = configCaches.DuracionConsulta;
			if (pHoraFin.Subtract(pHoraInicio).TotalSeconds > valorSegundos)
			{
				return true;
			}

			return false;
		}

		public string GenerarClaveCacheBusqueda(CacheConsultasCostosas pCacheConsultasCostosas)
		{
			string claveCache = string.Empty;
			string listaFiltros = string.Empty;
			string listaFiltrosExtra = string.Empty;
			string listaSemanticos = string.Empty;
			string listaFiltrosSearch = string.Empty;
			string listaExcluidos = string.Empty;
			string listaRangos = string.Empty;

			if (pCacheConsultasCostosas.ListaFiltros != null)
			{
				Dictionary<string, List<string>> diccionarioOrdenado = pCacheConsultasCostosas.ListaFiltros.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
				foreach (string key in diccionarioOrdenado.Keys)
				{
					listaFiltros = $"{listaFiltros}{key}_";
					diccionarioOrdenado[key].Sort();
					foreach (string filtro in diccionarioOrdenado[key])
					{
						listaFiltros = $"{listaFiltros}-{filtro}";
					}
				}
			}
			if (pCacheConsultasCostosas.ListaFiltrosExtra != null)
			{
				pCacheConsultasCostosas.ListaFiltrosExtra.Sort();
				foreach (string filtroExtra in pCacheConsultasCostosas.ListaFiltrosExtra)
				{
					listaFiltrosExtra = $"{listaFiltrosExtra}-{filtroExtra}";
				}
			}
			if (pCacheConsultasCostosas.Semanticos != null)
			{
				pCacheConsultasCostosas.Semanticos.Sort();
				foreach (string semantico in pCacheConsultasCostosas.Semanticos)
				{
					listaSemanticos = $"{listaSemanticos}-{semantico}";
				}
			}
			if (pCacheConsultasCostosas.ListaRangos != null)
			{
				pCacheConsultasCostosas.ListaRangos.Sort();
				foreach (int rango in pCacheConsultasCostosas.ListaRangos)
				{
					listaRangos = $"{listaRangos}-{rango}";
				}
			}
			if (pCacheConsultasCostosas.FiltrosSearchPersonalizados != null)
			{
				var diccionarioOrdenado = pCacheConsultasCostosas.FiltrosSearchPersonalizados.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
				foreach (string key in diccionarioOrdenado.Keys)
				{
					listaFiltrosSearch = $"{listaFiltrosSearch}{key}_{diccionarioOrdenado[key].Item1}-{diccionarioOrdenado[key].Item2}-{diccionarioOrdenado[key].Item3}-{diccionarioOrdenado[key].Item4}";
				}
			}
			if (pCacheConsultasCostosas.ListaExcluidos != null)
			{
				pCacheConsultasCostosas.ListaExcluidos.Sort();
				foreach (Guid id in pCacheConsultasCostosas.ListaExcluidos)
				{
					listaExcluidos = $"{listaExcluidos}-{id}";
				}
			}

			ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ProyectoCN>(), mLoggerFactory);
			ConfiguracionCachesCostosas configCaches = proyectoCN.ObtenerConfiguracionCachesCostosasDeProyecto(pCacheConsultasCostosas.ProyectoID);
			if (configCaches != null && configCaches.CachesAnonimas)
			{
				pCacheConsultasCostosas.IdentidadID = UsuarioAD.Invitado.ToString();
            }

			switch (pCacheConsultasCostosas.TipoDeCache)
			{
				case TipoCacheCostosa.Resultados:
					claveCache = $"BusquedaResultados_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.Descendente}_{pCacheConsultasCostosas.TipoFiltro}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoSelect}_{pCacheConsultasCostosas.FiltroContextoWhere}_{pCacheConsultasCostosas.FiltroContextoOrderBy}_{pCacheConsultasCostosas.FiltroContextoPesoMinimo}_{pCacheConsultasCostosas.TipoProyecto}_{pCacheConsultasCostosas.NamespacesExtra}_{pCacheConsultasCostosas.ResultadosEliminar}_{pCacheConsultasCostosas.PermitirRecursosPrivados}_{pCacheConsultasCostosas.OmitirPalabrasNoRelevantesSearch}_{pCacheConsultasCostosas.TipoAlgoritmoTransformacion}_FiltrosSearch_{listaFiltrosSearch}_FinFiltrosSearch_{pCacheConsultasCostosas.LanguageCode}_{pCacheConsultasCostosas.EsMovil}_Excluidos_{listaExcluidos}_FinExcluidos";
					break;
				case TipoCacheCostosa.NumeroResultados:
					claveCache = $"NumeroResultados_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.NombreFaceta}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoWhere}_{pCacheConsultasCostosas.TipoProyecto}_{pCacheConsultasCostosas.PermitirRecursosPrivados}_{pCacheConsultasCostosas.OmitirPalabrasNoRelevantesSearch}_{pCacheConsultasCostosas.TiposAlgoritmoTransformacion}_{pCacheConsultasCostosas.FiltrosSearchPersonalizados}_{pCacheConsultasCostosas.EsMovil}_Excluidos_{listaExcluidos}_FinExcluidos";
					break;
				case TipoCacheCostosa.ResultadosFormatoChart:
					claveCache = $"BusquedaResultadosFormatoChart_{pCacheConsultasCostosas.ProyectoID}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoSelect}_{pCacheConsultasCostosas.FiltroContextoWhere}_{pCacheConsultasCostosas.FiltroContextoOrderBy}_{pCacheConsultasCostosas.TipoProyecto}_{pCacheConsultasCostosas.NamespacesExtra}_{pCacheConsultasCostosas.ResultadosEliminar}_{pCacheConsultasCostosas.SelectChart}_{pCacheConsultasCostosas.FiltroChart}_{pCacheConsultasCostosas.PermitirRecursosPrivados}_{pCacheConsultasCostosas.EsMovil}_FiltrosSearch_{listaFiltrosSearch}_FinFiltrosSearch";
					break;
				case TipoCacheCostosa.ResultadosFormatoMapa:
					claveCache = $"BusquedaResultadosFormatoMapa_{pCacheConsultasCostosas.ProyectoID}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoSelect}_{pCacheConsultasCostosas.FiltroContextoWhere}_{pCacheConsultasCostosas.FiltroContextoOrderBy}_{pCacheConsultasCostosas.TipoProyecto}_{pCacheConsultasCostosas.NamespacesExtra}_{pCacheConsultasCostosas.ResultadosEliminar}_{pCacheConsultasCostosas.PermitirRecursosPrivados}_{pCacheConsultasCostosas.TipoBusqueda}_{pCacheConsultasCostosas.EsMovil}_FiltrosSearch_{listaFiltrosSearch}_FinFiltrosSearch_{pCacheConsultasCostosas.LanguageCode}";
					break;
				case TipoCacheCostosa.Facetas:
					claveCache = $"BusquedaFacetas_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.NombreFaceta}_{pCacheConsultasCostosas.ClaveFaceta}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.TipoDisenio}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoWhere}_{pCacheConsultasCostosas.TipoProyecto}_{pCacheConsultasCostosas.EsRango}_Rangos_{listaRangos}_FinRangos_{pCacheConsultasCostosas.Excluida}_{pCacheConsultasCostosas.UsarHilos}_{pCacheConsultasCostosas.ExcluirPersonas}_{pCacheConsultasCostosas.PermitirRecursosPrivados}_{pCacheConsultasCostosas.OmitirPalabrasNoRelevantesSearch}_{pCacheConsultasCostosas.Reciproca}_{pCacheConsultasCostosas.TipoPropiedadesFaceta}_FiltrosSearch_{listaFiltrosSearch}_FinFiltrosSearch_{pCacheConsultasCostosas.Inmutable}_{pCacheConsultasCostosas.EsMovil}";
					break;
				case TipoCacheCostosa.FacetaEspecialDBLP:
					claveCache = $"FacetaEspecialDBLP_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.NombreFaceta}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoWhere}";
					break;
				case TipoCacheCostosa.FacetaEspecialDBLPJournalPartOF:
					claveCache = $"FacetaEspecialDBLPJournalPartOF_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.NombreFaceta}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoWhere}";
					break;
				case TipoCacheCostosa.PersonasExacto:
					claveCache = $"PersonasExacto_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.Descendente}_{pCacheConsultasCostosas.TipoFiltro}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EstaEnProyecto}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}";
					break;
				case TipoCacheCostosa.FacetaSinOrdenDBLP:
					claveCache = $"FacetaSinOrdenDBLP_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.ClaveFaceta}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_FinFiltrosExtra_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.TipoDisenio}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltroContextoWhere}_{pCacheConsultasCostosas.TipoProyecto}_{pCacheConsultasCostosas.EsRango}_{listaRangos}_{pCacheConsultasCostosas.Excluida}_{pCacheConsultasCostosas.UsarHilos}";
					break;
				case TipoCacheCostosa.Autocompletar:
					claveCache = $"Autocompletar_{pCacheConsultasCostosas.ProyectoID}_Filtros_{listaFiltros}_FinFiltros_FiltrosExtra_{listaFiltrosExtra}_{pCacheConsultasCostosas.EstaEnMyGnoss}_{pCacheConsultasCostosas.EsMiembroComunidad}_{pCacheConsultasCostosas.EsInvitado}_{pCacheConsultasCostosas.IdentidadID}_{pCacheConsultasCostosas.Inicio}_{pCacheConsultasCostosas.Limite}_Semanticos_{listaSemanticos}_FinSemanticos_{pCacheConsultasCostosas.FiltrosContexto}";
					break;
				case TipoCacheCostosa.DatosAutocompletar:
					claveCache = $"DatosAutocompletar_{pCacheConsultasCostosas.ProyectoID}_{pCacheConsultasCostosas.NombreGrafo}_{pCacheConsultasCostosas.FiltroAutocompletar}";
					break;
			}

			return claveCache;
		}


		#region Privacidad Recursos

		/// <summary>
		/// Obtiene si un perfil tiene acceso a recursos privados en un proyecto
		/// </summary>
		/// <param name="pProyectoID">Identificador del proyecto</param>
		/// <param name="pPerfilID">Identificador del perfil</param>
		/// <returns>TRUE si tiene acceso a recursos privados, FALSE en caso contrario</returns>
		public bool TienePrivados(Guid pProyectoID, Guid pPerfilID)
		{
			bool tienePrivados = false;
			bool tieneGrupos = false;

			DocumentacionCL documentacionCL = new DocumentacionCL(mFicheroConfiguracionBD, mPoolName, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCL>(), mLoggerFactory);

			if (!pPerfilID.Equals(UsuarioAD.Invitado))
			{
				string rawKey = $"{NombresCL.FACETADO}_tienegrupos{pProyectoID}_{pPerfilID}";

				bool? tieneGruposCache = ObtenerObjetoDeCache(rawKey, typeof(bool)) as bool?;

				if (!tieneGruposCache.HasValue)
				{
					IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
					tieneGrupos = identidadCN.TieneIdentidadGrupos(pProyectoID, pPerfilID);
					identidadCN.Dispose();

					AgregarObjetoCache(rawKey, tieneGrupos, 86400); // 1 día
				}
				else
				{
					tieneGrupos = tieneGruposCache.Value;
				}

				if (!tieneGrupos)
				{
					//if(DocumentacionCN.TienePerfilConRecursosPrivados(pProyectoID, pPerfilID))
					//if (documentacionCL.PerfilesConRecursosPrivados(pProyectoID).Contains(pPerfilID))
					{
						tienePrivados = true;
					}
				}
			}
			return tienePrivados || tieneGrupos;
		}

		#endregion

		#endregion

		#region Propiedades

		/// <summary>
		/// Devuelve la clave para la caché
		/// </summary>
		public override string[] ClaveCache
		{
			get
			{
				return mMasterCacheKeyArray;
			}
		}

		/// <summary>
		/// Obtiene o establece la capa de negocio de Documentación
		/// </summary>
		protected DocumentacionCN DocumentacionCN
		{
			get
			{
				if (mDocumentacionCN == null)
				{
					if (mFicheroConfiguracionBD != null && mFicheroConfiguracionBD != "")
					{
						mDocumentacionCN = new DocumentacionCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
					}
					else
					{
						mDocumentacionCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
					}
				}
				return mDocumentacionCN;
			}
		}

		public FacetadoCN FacetadoCN
		{
			get
			{
				return mFacetadoCN;
			}
		}

		/// <summary>
		/// Dominio sobre el que se genera la cache
		/// </summary>
		public bool ObtenerDeCache
		{
			get
			{
				return mObtenerDeCache;
			}
			set
			{
				mObtenerDeCache = value;
			}
		}

		/// <summary>
		/// Dominio sobre el que se genera la cache
		/// </summary>
		public bool VerificarFechaDeCache
		{
			get
			{
				return mVerificarFechaDeCache;
			}
			set
			{
				mVerificarFechaDeCache = value;
			}
		}

		/// <summary>
		/// Dominio sobre el que se genera la cache
		/// </summary>
		public override string Dominio
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

		public Dictionary<string, List<string>> InformacionOntologias
		{
			get
			{
				if (mInformacionOntologias == null)
				{
					mInformacionOntologias = new Dictionary<string, List<string>>();
					mFacetadoCN.InformacionOntologias = mInformacionOntologias;
				}
				return mInformacionOntologias;
			}
			set
			{
				mInformacionOntologias = value;
				mFacetadoCN.InformacionOntologias = value;
			}
		}

		public string MandatoryRelacion
		{
			get
			{
				if (mMandatoryRelacion == null)
				{
					mFacetadoCN.MandatoryRelacion = mMandatoryRelacion;
				}
				return mMandatoryRelacion;
			}
			set
			{
				mMandatoryRelacion = value;
				mFacetadoCN.MandatoryRelacion = value;
			}
		}

		public List<Guid> ListaComunidadesPrivadasUsuario
		{
			get
			{
				return FacetadoCN.ListaComunidadesPrivadasUsuario;
			}
			set
			{
				FacetadoCN.ListaComunidadesPrivadasUsuario = value;
			}
		}


		public List<string> ListaItemsBusquedaExtra
		{
			get
			{
				return FacetadoCN.ListaItemsBusquedaExtra;
			}
			set
			{
				FacetadoCN.ListaItemsBusquedaExtra = value;
			}
		}

		public List<string> PropiedadesRango
		{
			get
			{
				return FacetadoCN.PropiedadesRango;
			}
			set
			{
				FacetadoCN.PropiedadesRango = value;
			}
		}

		public List<string> PropiedadesFecha
		{
			get
			{
				return FacetadoCN.PropiedadesFecha;
			}
			set
			{
				FacetadoCN.PropiedadesFecha = value;
			}
		}

		public DataWrapperFacetas FacetaDW
		{
			get
			{
				return FacetadoCN.FacetaDW;
			}
			set
			{
				FacetadoCN.FacetaDW = value;
			}
		}

		/// <summary>
		/// Verdad si está configurada la caché para consultas SPARQL de larga duración
		/// </summary>
		private bool HayCacheSparql
		{
			get
			{
				bool hayCache = false;
				if ((mListaPoolsRedis.Count > 0 && (mListaPoolsRedis.ContainsKey("SPARQL") || mListaPoolsRedis.First().Value.Exists(item => item.NombreConexion.Equals("SPARQL")))) || !string.IsNullOrEmpty(mConfigService.ObtenerConexionRedisIPMaster("SPARQL")))
				{
					hayCache = true;
					mPoolName = "SPARQL";
				}

				return hayCache;
			}
		}

		/// <summary>
		/// Condición extra para la consulta de facetas.
		/// </summary>
		public string CondicionExtraFacetas
		{
			get
			{
				return FacetadoCN.CondicionExtraFacetas;
			}
			set
			{
				FacetadoCN.CondicionExtraFacetas = value;
			}
		}

		public Dictionary<string, bool> DiccionarioFacetasExcluyentes
		{
			get
			{
				return FacetadoCN.DiccionarioFacetasExcluyentes;
			}
			set
			{
				FacetadoCN.DiccionarioFacetasExcluyentes = value;
			}
		}

		#endregion
	}
}
