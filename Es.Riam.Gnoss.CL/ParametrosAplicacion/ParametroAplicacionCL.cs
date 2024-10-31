using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Elementos.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.CL.ParametrosAplicacion
{
    /// <summary>
    /// Cache layer de par�metros de aplicaci�n
    /// </summary>
    public class ParametroAplicacionCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la cach�
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.PARAMETOAPLICACION };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        public ParametroAplicacionCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuraci�n</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroAplicacionCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para FacetadoCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuraci�n</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pPoolName">Nombre del pool</param>
        public ParametroAplicacionCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #endregion

        #region M�todos

        public List<ParametroAplicacion> ObtenerParametrosAplicacionPorContext()
        {
            mEntityContext.UsarEntityCache = true;
            ParametroAplicacionCN parametroAplicacionCN = null;
            // Si no est�, lo cargo y lo almaceno en la cach�
            if (string.IsNullOrEmpty(mFicheroConfiguracionBD))
            {
                parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                parametroAplicacionCN = new ParametroAplicacionCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            // Compruebo si est� en la cach�
            List<ParametroAplicacion> listaParametros = ObtenerObjetoDeCacheLocal(null) as List<ParametroAplicacion>;

            bool enCacheLocal = (listaParametros != null);

            if (!enCacheLocal)
            {
                listaParametros = ObtenerObjetoDeCache(null) as List<ParametroAplicacion>;
            }

            if (listaParametros != null)
            {
                if (listaParametros != null && !enCacheLocal)
                {
                    // Compruebo que la URL intragnoss es la misma con la que voy a buscar la clave
                    string urlIntragnoss = parametroAplicacionCN.ObtenerUrl();

                    ParametroAplicacion filaParametroAplicacion = listaParametros.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.UrlIntragnoss));

                    if (filaParametroAplicacion == null || urlIntragnoss != filaParametroAplicacion.Valor)
                    {
                        listaParametros = null;
                    }
                    else
                    {

                        AgregarObjetoCacheLocal(Guid.Empty, null, listaParametros);
                    }
                }
            }

            if (listaParametros == null)
            {

                listaParametros = mEntityContext.ParametroAplicacion.ToList();
                AgregarObjetoCache(null, listaParametros);
                AgregarObjetoCacheLocal(Guid.Empty, null, listaParametros);
            }

            parametroAplicacionCN.Dispose();
            mEntityContext.UsarEntityCache = false;
            return listaParametros;
        }

        public GestorParametroAplicacion ObtenerGestorParametros()
        {
            mEntityContext.UsarEntityCache = true;
            ParametroAplicacionCN parametroAplicacionCN = null;
            // Si no est�, lo cargo y lo almaceno en la cach�
            if (string.IsNullOrEmpty(mFicheroConfiguracionBD))
            {
                parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                parametroAplicacionCN = new ParametroAplicacionCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            // Compruebo si est� en la cach�
            GestorParametroAplicacion gestorListaParametros = ObtenerObjetoDeCacheLocal("GestorParametroAplicacion") as GestorParametroAplicacion;

            bool enCacheLocal = (gestorListaParametros != null);

            if (!enCacheLocal)
            {
                gestorListaParametros = ObtenerObjetoDeCache("GestorParametroAplicacion") as GestorParametroAplicacion;
            }

            if (gestorListaParametros != null)
            {
                if (gestorListaParametros != null && !enCacheLocal)
                {
                    // Compruebo que la URL intragnoss es la misma con la que voy a buscar la clave
                    string urlIntragnoss = parametroAplicacionCN.ObtenerUrl();

                    ParametroAplicacion filaParametroAplicacion = gestorListaParametros.ParametroAplicacion.FirstOrDefault(parametro => parametro.Parametro.Equals(TiposParametrosAplicacion.UrlIntragnoss));

                    if (filaParametroAplicacion == null || urlIntragnoss != filaParametroAplicacion.Valor)
                    {
                        gestorListaParametros = null;
                    }
                    else
                    {
                        AgregarObjetoCacheLocal(Guid.Empty, "GestorParametroAplicacion", gestorListaParametros);
                    }
                }
            }

            if (gestorListaParametros == null)
            {
                gestorListaParametros = new GestorParametroAplicacion();
                gestorListaParametros.ListaConfiguracionBBDD = mEntityContext.ConfiguracionBBDD.ToList();
                gestorListaParametros.ListaConfiguracionServicios = mEntityContext.ConfiguracionServicios.ToList();
                gestorListaParametros.ListaConfiguracionServiciosDominio = mEntityContext.ConfiguracionServiciosDominio.ToList();
                gestorListaParametros.ListaConfiguracionServiciosProyecto = mEntityContext.ConfiguracionServiciosProyecto.ToList();
                gestorListaParametros.ParametroAplicacion = mEntityContext.ParametroAplicacion.ToList();
                gestorListaParametros.ListaProyectoRegistroObligatorio = mEntityContext.ProyectoRegistroObligatorio.ToList();
                gestorListaParametros.ListaProyectoSinRegistroObligatorio = mEntityContext.ProyectoSinRegistroObligatorio.ToList();
                gestorListaParametros.ListaAccionesExternas = mEntityContext.AccionesExternas.ToList();
                gestorListaParametros.ListaConfigApplicationInsightsDominio = mEntityContext.ConfigApplicationInsightsDominio.ToList();
                gestorListaParametros.ListaTextosPersonalizadosPlataforma = mEntityContext.TextosPersonalizadosPlataforma.ToList();
                AgregarObjetoCache("GestorParametroAplicacion", gestorListaParametros);
                AgregarObjetoCacheLocal(Guid.Empty, "GestorParametroAplicacion", gestorListaParametros);
            }

            parametroAplicacionCN.Dispose();
            mEntityContext.UsarEntityCache = false;
            return gestorListaParametros;
        }

		/// <summary>
		/// Obtiene los c�digos de idioma y los nombres de la tabla "ParametrosAplicacion"
		/// </summary>
		/// <returns>Diccionario donde las claves son los c�digos de idioma y los valores son los nombres de los idiomas</returns>
		public Dictionary<string, string> ObtenerListaIdiomasDictionary()
		{
            Dictionary<string, string> listaIdiomasDictionary = (Dictionary<string, string>)ObtenerObjetoDeCacheLocal("listaIdiomasDictionary");
			if (listaIdiomasDictionary == null || (listaIdiomasDictionary != null && !(listaIdiomasDictionary.Count > 0)))
			{
                listaIdiomasDictionary = new Dictionary<string, string>();
				string idiomas = ObtenerGestorParametros().ParametroAplicacion.Where(item => item.Parametro.Equals("Idiomas")).Select(item => item.Valor).FirstOrDefault();
				if (string.IsNullOrEmpty(idiomas))
				{
                    ParametroAplicacionCN parametroAplicacionCN = new ParametroAplicacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
					listaIdiomasDictionary = parametroAplicacionCN.ObtenerListaIdiomasDictionary();
                    parametroAplicacionCN.Dispose();
				}
				else
				{
					// Dividir la cadena en pares de c�digo de idioma y nombre
					// Ej: es|Espa�ol,en|English,pt|Portuguese,ca|Catal�,eu|Euskera,gl|Galego,fr|Fran�ais,de|Deutsch,it|Italiano
					string[] idiomasArray = idiomas.Split("&&&");
					foreach (string idioma in idiomasArray)
					{
						string[] partes = idioma.Split('|');
						listaIdiomasDictionary.Add(partes[0], partes[1]);
					}
				}
				AgregarObjetoCacheLocal(Guid.Empty, "listaIdiomasDictionary", listaIdiomasDictionary);
			}
			return listaIdiomasDictionary;
		}

		/// <summary>
		/// Obtiene los c�digos de idioma de la tabla "ParametrosAplicacion"
		/// </summary>
		/// <returns>Lista de los c�digos de idioma</returns>
		public List<string> ObtenerListaIdiomas()
		{
			List<string> listacodigos = new List<string>();
			listacodigos = ObtenerListaIdiomasDictionary().Keys.ToList();
			return listacodigos;
		}

		public void InvalidarCacheIdiomas()
		{
			InvalidarCache("listaIdiomasDictionary");
		}

		public void InvalidarCacheParametrosAplicacion()
        {
            InvalidarCache("GestorParametroAplicacion");
            InvalidarCache(null, true);
            InvalidarCacheLocal("GestorParametroAplicacion");
			InvalidarCacheLocal(null);
			VersionarCacheLocal(Guid.Empty);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la clave para la cach�
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        #endregion
    }
}
