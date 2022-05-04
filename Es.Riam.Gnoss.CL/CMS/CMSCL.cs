using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Logica.CMS;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.CL.CMS
{
    public class CMSCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private CMSCN mCMSCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.CMS };

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para CMSCL
        /// </summary>
        public CMSCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para DocumentacionCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CMSCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para CMSCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pPoolName">Nombre del pool de conexión</param>
        public CMSCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #endregion

        #region Metodos

        #region Componentes

        /// <summary>
        /// Obtiene un componente de un proyecto en un idioma
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pComponenteID">ID del componente</param>
        /// <param name="pLanguageCode">Idioma</param>
        /// <returns>FichaComponente</returns>
        public CMSComponent ObtenerComponentePorIDEnProyecto(Guid pProyectoID, Guid pComponenteID, string pLanguageCode)
        {
            string rawKey = string.Concat("ComponenteCMSDeproyectoMVC_", pProyectoID.ToString(), "_", pComponenteID.ToString(), "_", pLanguageCode);
            CMSComponent componente = ObtenerObjetoDeCache(rawKey) as CMSComponent;
            return componente;
        }

        /// <summary>
        /// Actualiza en la cache el HTML de un componente
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pComponenteID">ID del componente</param>
        /// <param name="pLanguageCode">Idioma</param>
        /// <param name="pFichaComponente">FichaComponente</param>
        public void RefrescarComponentePorIDEnProyecto(Guid pProyectoID, Guid pComponenteID, string pLanguageCode, CMSComponent pFichaComponente)
        {
            string rawKey = string.Concat("ComponenteCMSDeproyectoMVC_", pProyectoID.ToString(), "_", pComponenteID.ToString(), "_", pLanguageCode);
            AgregarObjetoCache(rawKey, pFichaComponente);
        }

        /// <summary>
        /// Obtiene una lista de componentes de un proyecto en un idioma
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pListaComponentesID"></param>
        /// <param name="pLanguageCode">Idioma</param>
        /// <returns>Diccionario con la lista de componentes</returns>
        public Dictionary<Guid, CMSComponent> ObtenerListaComponentesPorIDEnProyecto(Guid pProyectoID, List<Guid> pListaComponentesID, string pLanguageCode)
        {
            Dictionary<Guid, CMSComponent> listaComponentes = new Dictionary<Guid, CMSComponent>();

            string[] componentes = new string[pListaComponentesID.Count];

            for (int i = 0; i < pListaComponentesID.Count; i++)
            {
                componentes[i] = ObtenerClaveCache(string.Concat("ComponenteCMSDeproyectoMVC_", pProyectoID.ToString(), "_", pListaComponentesID[i].ToString(), "_", pLanguageCode));
            }

            if (componentes.Length > 0)
            {

                Dictionary<string, object> lista = ObtenerListaObjetosCache(componentes, typeof(CMSComponent));

                foreach (string clave in lista.Keys)
                {
                    CMSComponent componente = (CMSComponent)(lista[clave]);

                    if (componente != null)
                    {
                        listaComponentes.Add(componente.Key, componente);
                    }
                }
            }
            return listaComponentes;
        }


        #endregion

        #region Configuracion

        /// <summary>
        /// Obtiene las tablasCMSPagina, CMSBLoque,CMSBLoqueComponente, CMSComponente y CMSComponentePropiedad de una ubicacion en un proyecto
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerCMSDeUbicacionDeProyecto(short pUbicacion, Guid pProyectoID, bool pTraerSoloActivos)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("ConfiguracionCMSDeUbicacionEnComunidad_", pUbicacion, "_", pProyectoID.ToString(), "_" + pTraerSoloActivos.ToString());

            DataWrapperCMS CMSDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperCMS;
            if (CMSDW == null)
            {
                CMSDW = ObtenerObjetoDeCache(rawKey) as DataWrapperCMS;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, CMSDW);
            }

            if (CMSDW != null)
            {
                DataWrapperCMS CMSDSAuxDW = new DataWrapperCMS();

                try
                {
                    CMSDSAuxDW.Merge(CMSDW);

                    if (CMSDW != null)
                    {
                        // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de caché, luego da problemas cuando intentas acceder a ellos. 
                        // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                        CMSDW = CMSDSAuxDW;
                    }
                }
                catch { CMSDW = null; }
            }

            if (CMSDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    CMSDW = CMSCN.ObtenerCMSDeUbicacionDeProyecto(pUbicacion, ProyectoIDPadreEcosistema.Value, 2, pTraerSoloActivos);
                    ModificarDataWrapperComunidadHija(CMSDW, pProyectoID);
                }

                if (CMSDW == null || CMSDW.ListaCMSBloque.Count == 0) // Es null o el padre no ha definido componentes para esta página
                {
                    CMSDW = CMSCN.ObtenerCMSDeUbicacionDeProyecto(pUbicacion, pProyectoID, 2, pTraerSoloActivos);
                }

                AgregarObjetoCache(rawKey, CMSDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, CMSDW);
            }
            mEntityContext.UsarEntityCache = false;
            return CMSDW;
        }

        /// <summary>
        /// Invalida la cache de la configuracion de la ubicacion de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Guid del proyecto</param>
        public void InvalidarCacheCMSDeUbicacionDeProyecto(short pUbicacion, Guid pProyectoID)
        {
            string rawKey1 = string.Concat("ConfiguracionCMSDeUbicacionEnComunidad_", pUbicacion, "_", pProyectoID.ToString(), "_", true.ToString());
            InvalidarCache(rawKey1);

            string rawKey2 = string.Concat("ConfiguracionCMSDeUbicacionEnComunidad_", pUbicacion, "_", pProyectoID.ToString(), "_", false.ToString());
            InvalidarCache(rawKey2);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Invalida las caches de las configuraciones de las ubicaciones de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Guid del proyecto</param>
        public void InvalidarCachesCMSDeUbicacionesDeProyecto(Guid pProyectoID)
        {
            string rawKey1 = ObtenerClaveCache(string.Concat("ConfiguracionCMSDeUbicacionEnComunidad_*_", pProyectoID.ToString(), "_", true.ToString()));
            string rawKey2 = ObtenerClaveCache(string.Concat("ConfiguracionCMSDeUbicacionEnComunidad_*_", pProyectoID.ToString(), "_", false.ToString()));

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey1.ToLower()).Result.ToList();
                claves.AddRange(ClienteRedisLectura.Keys(rawKey2.ToLower()).Result.ToList());
            }
            InvalidarCachesMultiples(claves);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Invalida un componente de un proyecto en un idioma
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pComponenteID">ID del componente</param>
        /// <param name="pLanguageCode">Idioma</param>
        public void InvalidarCacheDeComponentePorIDEnProyecto(Guid pProyectoID, Guid pComponenteID, string pLanguageCode)
        {
            string rawKey = string.Concat("ComponenteCMSDeproyectoMVC_", pProyectoID.ToString(), "_", pComponenteID.ToString(), "_", pLanguageCode);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Invalida un componente de un proyecto en todos los idiomas
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pComponenteID">ID del componente</param>
        public void InvalidarCacheDeComponentePorIDEnProyectoTodosIdiomas(Guid pProyectoID, Guid pComponenteID)
        {
            string rawKey = ObtenerClaveCache(string.Concat("ComponenteCMSDeproyectoMVC_", pProyectoID.ToString(), "_", pComponenteID.ToString(), "_*"));

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower()).Result.ToList();
            }
            InvalidarCachesMultiples(claves);
        }

        /// <summary>
        /// Invalida todos los componentes de un proyecto en todos los idiomas
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        public void InvalidarCachesDeComponentesEnProyecto(Guid pProyectoID)
        {
            string rawKey = ObtenerClaveCache(string.Concat("ComponenteCMSDeproyectoMVC_", pProyectoID.ToString(), "_*"));
            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower()).Result.ToList();
            }
            InvalidarCachesMultiples(claves);
        }


        /// <summary>
        /// Obtiene el Dataset CMSDS de la comunidad, las tablas CMSPagina, CMSBloque y CMSBloqueComponente
        /// </summary>
        /// <param name="pProyecto">Guid del proyecto</param>
        /// <returns>DataSet de CMS</returns>
        public DataWrapperCMS ObtenerConfiguracionCMSPorProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("ConfiguracionCMSComunidad_", pProyectoID.ToString());

            bool tieneComunidadPadreConfigurada = TieneComunidadPadreConfigurada(pProyectoID);

            DataWrapperCMS CMSDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperCMS;
            if (CMSDW == null)
            {
                CMSDW = ObtenerObjetoDeCache(rawKey) as DataWrapperCMS;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, CMSDW);
            }

            if (CMSDW != null)
            {
                DataWrapperCMS CMSDSAuxDW = new DataWrapperCMS();

                try
                {
                    CMSDSAuxDW.Merge(CMSDW);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de caché, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    CMSDW = CMSDSAuxDW;
                }
                catch { CMSDW = null; }
            }

            if (CMSDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                CMSDW = CMSCN.ObtenerConfiguracionCMSPorProyecto(pProyectoID);
                if (tieneComunidadPadreConfigurada)
                {
                    CMSDW = CMSCN.ObtenerConfiguracionCMSPorProyecto(ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHija(CMSDW, pProyectoID);
                }
                else
                {
                    CMSDW = CMSCN.ObtenerConfiguracionCMSPorProyecto(pProyectoID);
                }
                AgregarObjetoCache(rawKey, CMSDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, CMSDW);
            }
            mEntityContext.UsarEntityCache = false;
            return CMSDW;
        }

        private void ModificarDataWrapperComunidadHija(DataWrapperCMS cMSDW, Guid pProyectoID)
        {
            foreach (AD.EntityModel.Models.CMS.CMSPagina cmsPagina in cMSDW.ListaCMSPagina)
            {
                cmsPagina.ProyectoID = pProyectoID;
            }
            foreach (AD.EntityModel.Models.CMS.CMSBloque cmsBloque in cMSDW.ListaCMSBloque)
            {
                cmsBloque.ProyectoID = pProyectoID;
            }
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponente cmsBloqueComponente in cMSDW.ListaCMSBloqueComponente)
            {
                cmsBloqueComponente.ProyectoID = pProyectoID;
            }
            foreach (AD.EntityModel.Models.CMS.CMSBloqueComponentePropiedadComponente cmsBloqueComponentePropiedadComponente in cMSDW.ListaCMSBloqueComponentePropiedadComponente)
            {
                cmsBloqueComponentePropiedadComponente.ProyectoID = pProyectoID;
            }
            foreach (AD.EntityModel.Models.CMS.CMSComponentePrivadoProyecto cmsComponentePrivadoProyecto in cMSDW.ListaCMSComponentePrivadoProyecto)
            {
                cmsComponentePrivadoProyecto.ProyectoID = pProyectoID;
            }
        }

        /// <summary>
        /// Invalida la cache de la configuracion del CMS de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Guid del proyecto</param>
        public void InvalidarCacheConfiguracionCMSPorProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("ConfiguracionCMSComunidad_", pProyectoID.ToString());
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }
        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected CMSCN CMSCN
        {
            get
            {
                if (mCMSCN == null)
                {
                    if (mFicheroConfiguracionBD != null && mFicheroConfiguracionBD != "")
                    {
                        mCMSCN = new CMSCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                    else
                    {
                        mCMSCN = new CMSCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                }

                return mCMSCN;
            }
        }

        /// <summary>
        /// Clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
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

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~CMSCL()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.disposed = true;

                try
                {
                    if (disposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e);
                }
            }
        }

        #endregion
    }
}
