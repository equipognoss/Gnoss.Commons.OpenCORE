using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Logica.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.CL.ParametrosProyecto
{
    /// <summary>
    /// Cache layer de parámetros de proyecto
    /// </summary>
    public class ParametroGeneralCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.PARAMETROGENERAL };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para ParametroGeneralCL
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ParametroGeneralCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        /// <summary>
        /// Constructor para ParametroGeneralCL
        /// </summary>
        public ParametroGeneralCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Recupera todos los parametrosGenerales del proyecto que se pasa
        /// </summary>
        /// <returns>Países y provincias</returns>
        public GestorParametroGeneral ObtenerParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = pProyectoID.ToString();

            // Compruebo si está en la caché
            GestorParametroGeneral paramDS = ObtenerObjetoDeCacheLocal(ObtenerClaveCache(rawKey)) as GestorParametroGeneral;
            if (paramDS == null)
            {
                paramDS = ObtenerObjetoDeCache(rawKey) as GestorParametroGeneral;
                AgregarObjetoCacheLocal(pProyectoID, ObtenerClaveCache(rawKey), paramDS);
            }

            //if (!ComprobarEstructuraDataSet(paramDS, new GestorParametroGeneral()))
            //{
            //    paramDS = null;
            //}

            if (paramDS == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    paramDS = obtenerFilaParametrosProyecto(ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHija(paramDS, pProyectoID);
                }
                else
                {
                    paramDS = obtenerFilaParametrosProyecto(pProyectoID);
                }

                AgregarObjetoCache(rawKey, paramDS);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, paramDS);
            }
            mEntityContext.UsarEntityCache = false;
            return paramDS;
        }

        public GestorParametroGeneral obtenerFilaParametrosProyecto(Guid pProyectoID)
        {
            GestorParametroGeneral gestor = new GestorParametroGeneral();
            if (!pProyectoID.Equals(Guid.Empty))
            {
                mEntityContext.UsarEntityCache = true;
                ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                var param = parametroGeneralCN.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID);
                if (param != null)
                {
                    gestor.ListaParametroGeneral.Add(param);
                }
                gestor.ListaProyectoElementoHtml.AddRange(parametroGeneralCN.ObtenerFilaProyectoElementoHtml(pProyectoID));
                gestor.ListaProyectoMetaRobots.AddRange(parametroGeneralCN.ObtenerFilaProyectoMetaRobots(pProyectoID));
                mEntityContext.UsarEntityCache = false;
            }
            return gestor;
        }

        /// <summary>
        /// Recupera todos los parametrosGenerales del proyecto que se pasa
        /// </summary>
        /// <returns>Países y provincias</returns>
        //public ParametroGeneralDS ObtenerParametrosGeneralesDeProyectoViejo(Guid pProyectoID)
        //{
        //    string rawKey = pProyectoID.ToString();

        //    // Compruebo si está en la caché
        //    ParametroGeneralDS paramDS = ObtenerObjetoDeCacheLocal(ObtenerClaveCache(rawKey)) as ParametroGeneralDS;
        //    if (paramDS == null)
        //    {
        //        paramDS = ObtenerObjetoDeCache(rawKey) as ParametroGeneralDS;
        //        AgregarObjetoCacheLocal(pProyectoID, ObtenerClaveCache(rawKey), paramDS);
        //    }

        //    if (!ComprobarEstructuraDataSet(paramDS, new ParametroGeneralDS()))
        //    {
        //        paramDS = null;
        //    }

        //    if (paramDS == null)
        //    {
        //        if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
        //        {
        //            // Si no está, lo cargo y lo almaceno en la caché
        //            ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mFicheroConfiguracionBD);
        //            paramDS = (ParametroGeneralDS)parametroGeneralCN.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID).Table.DataSet;
        //            parametroGeneralCN.Dispose();
        //        }
        //        else
        //        {
        //            // Si no está, lo cargo y lo almaceno en la caché
        //            ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN();
        //            paramDS = (ParametroGeneralDS)parametroGeneralCN.ObtenerFilaParametrosGeneralesDeProyecto(pProyectoID).Table.DataSet;
        //            parametroGeneralCN.Dispose();
        //        }

        //        AgregarObjetoCache(rawKey, paramDS);
        //        AgregarObjetoCacheLocal(pProyectoID, rawKey, paramDS);
        //    }

        //    return paramDS;
        //}

        public GestorParametroGeneral ObtenerTextosPersonalizadosPersonalizacionEcosistema(Guid pPersonalizacionEcosistemaID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = "TextosEcosistema_" + pPersonalizacionEcosistemaID.ToString();
            // Compruebo si está en la caché
            GestorParametroGeneral paramDS = ObtenerObjetoDeCacheLocal(rawKey) as GestorParametroGeneral;
            if (paramDS == null)
            {
                paramDS = ObtenerObjetoDeCache(rawKey) as GestorParametroGeneral;
                AgregarObjetoCacheLocal(pPersonalizacionEcosistemaID, rawKey, paramDS);
            }

            if (paramDS == null)
            {
                paramDS = new GestorParametroGeneral();
                if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
                {
                    // Si no está, lo cargo y lo almaceno en la caché
                    ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paramDS.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(pPersonalizacionEcosistemaID);
                    parametroGeneralCN.Dispose();
                }
                else
                {
                    // Si no está, lo cargo y lo almaceno en la caché
                    ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paramDS.ListaTextosPersonalizadosPersonalizacion = parametroGeneralCN.ObtenerTextosPersonalizadosPersonalizacionEcosistema(pPersonalizacionEcosistemaID);
                    parametroGeneralCN.Dispose();
                }

                AgregarObjetoCache(rawKey, paramDS);
                AgregarObjetoCacheLocal(pPersonalizacionEcosistemaID, rawKey, paramDS);
            }
            mEntityContext.UsarEntityCache = false;
            return paramDS;
        }

        /// <summary>
        /// Comprueba si un proyecto tiene establecida la imagen de la home
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public bool TieneProyectoImagenHome(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.TIENEPROYECTOIMAGENHOME, "_", pProyectoID);

            // Compruebo si está en la caché
            bool? tieneImagen = (bool?)ObtenerObjetoDeCacheLocal(rawKey);

            if (!tieneImagen.HasValue)
            {
                tieneImagen = (bool?)ObtenerObjetoDeCache(rawKey);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, tieneImagen);
            }

            if (!tieneImagen.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                ParametroGeneralCN parametroGeneralCN = new ParametroGeneralCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                tieneImagen = parametroGeneralCN.TieneProyectoImagenHome(pProyectoID);
                parametroGeneralCN.Dispose();
                AgregarObjetoCache(rawKey, tieneImagen);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, tieneImagen);
            }
            return tieneImagen.Value;
        }

        /// <summary>
        /// Invalida si un proyecto tiene establecida la imagen de la home
        /// </summary>
        public void InvalidarTieneProyectoImagenHome(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.TIENEPROYECTOIMAGENHOME, "_", pProyectoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Invalida la cache de los textos personalizados personalización del ecosistema
        /// </summary>
        /// <param name="pPersonalizacionEcosistemaID">Identificador de la personalización del ecosistema</param>
        public void InvalidarCacheTextosPersonalizadosPersonalizacionEcosistema(Guid pPersonalizacionEcosistemaID)
        {
            string rawKey = "TextosEcosistema_" + pPersonalizacionEcosistemaID.ToString();
            InvalidarCache(rawKey);

            VersionarCacheLocal(pPersonalizacionEcosistemaID);
        }

        /// <summary>
        /// Invalida todos los parametrosGenerales del proyecto que se pasa
        /// </summary>
        public void InvalidarCacheParametrosGeneralesDeProyecto(Guid pProyectoID)
        {
            InvalidarCache(ObtenerClaveCache(pProyectoID.ToString()));

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Invalida la cache de si un proyecto tiene establecida la imagen de la home
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        public void InvalidarCacheDeTieneProyectoImagenHome(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.TIENEPROYECTOIMAGENHOME, "_", pProyectoID);
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        private void ModificarDataWrapperComunidadHija(GestorParametroGeneral gestorParametroGeneral, Guid pProyectoID)
        {
            foreach (AD.EntityModel.Models.ParametroGeneralDS.ParametroGeneral parametroGeneral in gestorParametroGeneral.ListaParametroGeneral)
            {
                parametroGeneral.ProyectoID = pProyectoID;
            }

            foreach (AD.EntityModel.Models.ParametroGeneralDS.ProyectoElementoHtml proyectoElementoHtml in gestorParametroGeneral.ListaProyectoElementoHtml)
            {
                proyectoElementoHtml.ProyectoID = pProyectoID;
            }

            foreach (AD.EntityModel.Models.ParametroGeneralDS.ProyectoMetaRobots proyectoMetaRobots in gestorParametroGeneral.ListaProyectoMetaRobots)
            {
                proyectoMetaRobots.ProyectoID = pProyectoID;
            }
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

        #endregion
    }
}
