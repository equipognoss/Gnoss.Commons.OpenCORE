using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.CL.Live
{
    public class LiveCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.LIVE };

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        public LiveCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        /// <param name="pPoolName">Nombre del pool de conexión</param>
        public LiveCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
        }

        #endregion

        #region Metodos

        #region Destacados Home

        /// <summary>
        /// Agrega el HTML de las comunidades de la home de GNOSS
        /// </summary>       
        /// <param name="pHtml">HTML de las comunidades</param>
        /// <param name="pIdioma">Idioma</param>
        public void AgregarComunidadesHomeGNOSSHtml(string pHtml, string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.HOMECOMUNIDADESDESTACADASHTML, "_", pIdioma);
            AgregarObjetoCache(rawKey, pHtml);
        }

        /// <summary>
        /// Obtiene el HTML de las comunidades de la home de GNOSS en Español
        /// </summary>
        /// <returns>String con el HTML</returns>
        public string ObtenerComunidadesHomeGNOSSHtml(string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.HOMECOMUNIDADESDESTACADASHTML, "_", pIdioma);
            return (string)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Invalida la cache del HTML de las comunidades de la home de GNOSS
        /// </summary>
        public void InvalidarCacheComunidadesHomeHtml(List<string> pListaIdiomas)
        {
            foreach (string idioma in pListaIdiomas)
            {
                string rawKey = string.Concat(NombresCL.HOMECOMUNIDADESDESTACADASHTML, "_", idioma);
                InvalidarCache(rawKey);
            }
        }

        /// <summary>
        /// Agrega el DS de de las comunidades más activas para la home de GNOSS
        /// </summary>       
        /// <param name="pProyectoDW">DataSet de proyectos más activos</param>
        public void AgregarComunidadesHomeGNOSSDS(DataWrapperProyecto pProyectoDW)
        {
            string rawKey = NombresCL.HOMECOMUNIDADESDESTACADASDS;
            AgregarObjetoCache(rawKey, pProyectoDW);
        }

        /// <summary>
        /// Obtiene el DS de las comunidades más activas para la home de GNOSS
        /// </summary>
        /// <returns>Lista de proyectos</returns>
        public DataWrapperProyecto ObtenerComunidadesHomeGNOSSDS()
        {
            string rawKey = NombresCL.HOMECOMUNIDADESDESTACADASDS;
            return (DataWrapperProyecto)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Invlaida el DS de las comunidades más activas para la home de GNOSS
        /// </summary>
        public void InvalidarCacheComunidadesHomeDS()
        {
            string rawKey = NombresCL.HOMECOMUNIDADESDESTACADASDS;
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Agrega el HTML de los recursos de la home de GNOSS
        /// </summary>       
        /// <param name="pHtml">HTML de los recursos de la home de GNOSS en Español</param>
        /// <param name="pIdioma">Idioma</param>
        public void AgregarRecursosHomeGNOSSHtml(string pHtml, string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.HOMERECURSOSDESTACADOSHTML, "_", pIdioma);
            AgregarObjetoCache(rawKey, pHtml); //24 horas   
        }

        /// <summary>
        /// Obtiene el HTML de los recursos de la home de GNOSS en español
        /// </summary>
        /// <returns>String con el HTML</returns>
        public string ObtenerRecursosHomeGNOSSHtml(string pIdioma)
        {
            string rawKey = string.Concat(NombresCL.HOMERECURSOSDESTACADOSHTML, "_", pIdioma);
            return (string)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Invalida la cache del HTML de los recursos de la Home de GNOSS
        /// </summary>
        public void InvalidarCacheRecursosHomeHtml(List<string> pListaIdiomas)
        {
            foreach (string idioma in pListaIdiomas)
            {
                string rawKey = string.Concat(NombresCL.HOMERECURSOSDESTACADOSHTML, "_", idioma);
                InvalidarCache(rawKey);
            }
        }

        /// <summary>
        /// Agrega el DS de los recursos de la home de GNOSS
        /// </summary>       
        /// <param name="pDataWrapperDocumentacion">DS de recursos</param>
        public void AgregarRecursosHomeGNOSSDS(DataWrapperDocumentacion pDataWrapperDocumentacion)
        {
            string rawKey = NombresCL.HOMERECURSOSDESTACADOSDS;
            AgregarObjetoCache(rawKey, pDataWrapperDocumentacion);
        }

        /// <summary>
        /// Obtiene el DS de los recursos de la home de GNOSS
        /// </summary>
        /// <returns>DS de recursos</returns>
        public DataWrapperDocumentacion ObtenerRecursosHomeGNOSSDS()
        {
            string rawKey = NombresCL.HOMERECURSOSDESTACADOSDS;
            return (DataWrapperDocumentacion)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Invalida la cache del DS de los recursos de la Home de GNOSS
        /// </summary>
        public void InvalidarCacheRecursosHomeDS()
        {
            string rawKey = NombresCL.HOMERECURSOSDESTACADOSDS;
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Agrega el DS de los usuarios de la home de GNOSS
        /// </summary>       
        ///<param name="pIdentidadDW">DataSet de identidades</param>
        public void AgregarUsuariosHomeGNOSSDS(DataWrapperIdentidad pIdentidadDW)
        {
            if (pIdentidadDW != null)
            {
                string rawKey = NombresCL.HOMEUSUARIOSDESTACADOSDS;
                AgregarObjetoCache(rawKey, pIdentidadDW);
            }
        }

        /// <summary>
        /// Obtiene el DS de los usuarios de la home de GNOSS
        /// </summary>
        /// <returns>Dataset de Identidades</returns>
        public DataWrapperIdentidad ObtenerUsuariosHomeGNOSSDS()
        {
            string rawKey = NombresCL.HOMEUSUARIOSDESTACADOSDS;
            return (DataWrapperIdentidad)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Invalida la cache de usuarios de la home de GNOSS
        /// </summary>
        public void InvalidarCacheUsuariosHomeDS()
        {
            string rawKey = NombresCL.HOMEUSUARIOSDESTACADOSDS;
            InvalidarCache(rawKey);
        }
        #endregion

        /// <summary>
        /// Invalida la cache de eventos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void InvalidarCacheProyecto(Guid pProyectoID)
        {
            string rawKey = ObtenerClaveCache(pProyectoID.ToString());
            InvalidarCache(rawKey, false);

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKey.ToLower() + "*").Result.ToList();
            }

            InvalidarCachesMultiples(claves);
        }

        #endregion

        #region Propiedades

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

        /// <summary>
        /// No queremos que se pieran las cachés LIVE, porque no se regeneran de manera automática
        /// </summary>
        protected override string VersionCache
        {
            get
            {
                return string.Empty;
            }
        }

        #endregion
    }
}
