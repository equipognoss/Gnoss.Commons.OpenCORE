using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Tesauro;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.CL.Tesauro
{
    /// <summary>
    /// Cache layer para tesauro
    /// </summary>
    public class TesauroCL : BaseCL, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private TesauroCN mTesauroCN = null;

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.TESAURO };

        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Contructor con fichero de configuración.
        /// </summary>
        public TesauroCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<TesauroCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Contructor con fichero de configuración.
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración</param>
        public TesauroCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<TesauroCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }


        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene el tesauro del metaproyecto
        /// </summary>
        /// <param name="pWikiGnossID">Identificador de wiki</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroDeProyectoMyGnoss()
        {
            return ObtenerTesauroDeProyecto(ProyectoAD.MetaProyecto);
        }

        /// <summary>
        /// Obtiene el tesauro del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de tesauro</returns>
        public DataWrapperTesauro ObtenerTesauroDeProyecto(Guid pProyectoID)
        {
            string rawKey = pProyectoID.ToString();
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperTesauro tesauroDW = ObtenerObjetoDeCacheLocal(ObtenerClaveCache(rawKey)) as DataWrapperTesauro;

            if (tesauroDW == null)
            {
                tesauroDW = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperTesauro)) as DataWrapperTesauro;
                AgregarObjetoCacheLocal(pProyectoID, ObtenerClaveCache(rawKey), tesauroDW);
            }

            if (tesauroDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                tesauroDW = TesauroCN.ObtenerTesauroDeProyecto(pProyectoID);
                if (tesauroDW != null)
                {
                    tesauroDW.CargaRelacionesPerezosasCache();
                }
                if (tesauroDW.ListaCatTesauroCompartida.Count > 0)
                {
                    List<Guid> listaTesauros = new List<Guid>();
                    foreach (AD.EntityModel.Models.Tesauro.CatTesauroCompartida filaCatTesComp in tesauroDW.ListaCatTesauroCompartida)
                    {
                        Guid tesauroID = filaCatTesComp.TesauroOrigenID;
                        if (!listaTesauros.Contains(tesauroID))
                        {
                            listaTesauros.Add(tesauroID);
                        }
                    }
                    DataWrapperTesauro tesauroTempDW = TesauroCN.ObtenerTesauroPorListaIDs(listaTesauros);

                    foreach (CatTesauroCompartida filaCatTesComp in tesauroDW.ListaCatTesauroCompartida.OrderByDescending(item => item.Orden))
                    {
                        //   TesauroDS.CategoriaTesauroRow filaCatTesauro = tesauroTempDW.ListaCategoriaTesauro.FindByTesauroIDCategoriaTesauroID(filaCatTesComp.TesauroOrigenID, filaCatTesComp.CategoriaOrigenID);
                        CategoriaTesauro filaCatTesauro = tesauroTempDW.ListaCategoriaTesauro.FirstOrDefault(item => item.TesauroID.Equals(filaCatTesComp.TesauroOrigenID) && item.CategoriaTesauroID.Equals(filaCatTesComp.CategoriaOrigenID));
                        filaCatTesauro.Orden = filaCatTesComp.Orden;
                        //filaCatTesauro.AcceptChanges();
                        //tesauroDW.CategoriaTesauro.ImportRow(filaCatTesauro);
                        tesauroDW.ListaCategoriaTesauro.Add(filaCatTesauro);

                        if (filaCatTesComp.CategoriaSupDestinoID != null)
                        {
                            AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro filaCatTesCompAgCatTes = new AD.EntityModel.Models.Tesauro.CatTesauroAgCatTesauro();
                            filaCatTesCompAgCatTes.TesauroID = filaCatTesComp.TesauroOrigenID;
                            filaCatTesCompAgCatTes.CategoriaSuperiorID = filaCatTesComp.CategoriaSupDestinoID.Value;
                            filaCatTesCompAgCatTes.CategoriaInferiorID = filaCatTesComp.CategoriaOrigenID;
                            filaCatTesCompAgCatTes.Orden = filaCatTesComp.Orden;

                            tesauroDW.ListaCatTesauroAgCatTesauro.Add(filaCatTesCompAgCatTes);
                        }

                        List<CatTesauroAgCatTesauro> filasCatTesauro = tesauroTempDW.ListaCatTesauroAgCatTesauro.Where(item => item.TesauroID.Equals(filaCatTesComp.TesauroOrigenID) && item.CategoriaSuperiorID.Equals(filaCatTesComp.CategoriaOrigenID)).ToList();
                        //Select("TesauroID = '" + filaCatTesComp.TesauroOrigenID + "' AND CategoriaSuperiorID = '" + filaCatTesComp.CategoriaOrigenID + "'");

                        foreach (CatTesauroAgCatTesauro filaCatTesAgCatTes in filasCatTesauro)
                        {
                            tesauroDW.ListaCatTesauroAgCatTesauro.Add(filaCatTesAgCatTes);
                            ImportarCategoriaCompartidaTesauro(filaCatTesComp.TesauroOrigenID, filaCatTesAgCatTes.CategoriaInferiorID, tesauroTempDW, tesauroDW);
                        }
                    }
                }

                AgregarObjetoCache(rawKey, tesauroDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, tesauroDW);
            }
            mEntityContext.UsarEntityCache = false;
            return tesauroDW;
        }


        private void ImportarCategoriaCompartidaTesauro(Guid pTesauroID, Guid pCategoriaID, DataWrapperTesauro pTesauroOrigen, DataWrapperTesauro pTesauroDestino)
        {
            //FindByTesauroIDCategoriaTesauroID(pTesauroID, pCategoriaID)
            CategoriaTesauro filaCatTesauro = pTesauroOrigen.ListaCategoriaTesauro.FirstOrDefault(item => item.TesauroID.Equals(pTesauroID) && item.CategoriaTesauroID.Equals(pCategoriaID));
            pTesauroDestino.ListaCategoriaTesauro.Add(filaCatTesauro);

            List<CatTesauroAgCatTesauro> filasCatTesauro = pTesauroOrigen.ListaCatTesauroAgCatTesauro.Where(item => item.TesauroID.Equals(pTesauroID) && item.CategoriaSuperiorID.Equals(pCategoriaID)).ToList();

            foreach (CatTesauroAgCatTesauro filaCatTesAgCatTes in filasCatTesauro)
            {
                pTesauroDestino.ListaCatTesauroAgCatTesauro.Add(filaCatTesAgCatTes);
                ImportarCategoriaCompartidaTesauro(pTesauroID, filaCatTesAgCatTes.CategoriaInferiorID, pTesauroOrigen, pTesauroDestino);
            }

        }

        /// <summary>
        /// Invalida la caché del tesauro de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void InvalidarCacheDeTesauroDeProyecto(Guid pProyectoID)
        {
            string rawKey = pProyectoID.ToString();
            InvalidarCache(ObtenerClaveCache(rawKey));

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Actualiza la base de datos con los datos de tesauro pasados por parámetro
        /// </summary>
        /// <param name="pTesauroDW">Dataset de tesauro para actualizar</param>
        public void ActualizarTesauro(DataWrapperTesauro pTesauroDW)
        {
            //Si hay cambios los guardo e invalido la caché
            if (mEntityContext.ChangeTracker.HasChanges())
            {
                //Actualizar los cambios
                TesauroCN.ActualizarTesauro();

                // Invalidar la caché
                InvalidarCache();
            }
        }

        /// <summary>
        /// Obtiene las categorías que permiten solo ciertos tipos de recurso.
        /// </summary>
        /// <param name="pTesauroID">ID de tesauro</param>
        /// <returns>DataSet con la tabla CatTesauroPermiteTipoRec</returns>
        public DataWrapperTesauro ObtenerCategoriasPermitidasPorTipoRecurso(Guid pTesauroID, Guid pProyectoID)
        {
            string rawKey = string.Concat("CategoriasPermitidasPorTipoRecurso", "_", pTesauroID);

            // Compruebo si está en la caché
            DataWrapperTesauro tesauroDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperTesauro;

            if (tesauroDW == null)
            {
                tesauroDW = ObtenerObjetoDeCache(rawKey, typeof(DataWrapperTesauro)) as DataWrapperTesauro;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, tesauroDW);
            }

            if (tesauroDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                tesauroDW = TesauroCN.ObtenerCategoriasPermitidasPorTipoRecurso(pTesauroID);
                AgregarObjetoCache(rawKey, tesauroDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, tesauroDW);
            }

            return tesauroDW;
        }

        /// <summary>
        /// Borra la caché de las categorías que permiten solo ciertos tipos de recurso.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto que tiene el tesauro</param>
        public void BorrarCategoriasPermitidasPorTipoRecurso(Guid pProyectoID)
        {
            DataWrapperTesauro tesDW = ObtenerTesauroDeProyecto(pProyectoID);
            Guid tesauroID = tesDW.ListaTesauro.FirstOrDefault().TesauroID;

            string rawKey = string.Concat("CategoriasPermitidasPorTipoRecurso", "_", tesauroID);
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        #endregion        

        #region Propiedades

        /// <summary>
        /// Obtiene la clase de negocio de tesauro
        /// </summary>
        protected TesauroCN TesauroCN
        {
            get
            {
                if (mTesauroCN == null)
                {
                    if (string.IsNullOrEmpty(mFicheroConfiguracionBD))
                    {
                        mTesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
                    }
                    else
                    {
                        mTesauroCN = new TesauroCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<TesauroCN>(), mLoggerFactory);
                    }
                }
                return mTesauroCN;
            }
        }

        /// <summary>
        /// Obtiene la clave para la caché
        /// </summary>
        public override string[] ClaveCache
        {
            get
            {
                return mMasterCacheKeyArray;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~TesauroCL()
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
        protected override void Dispose(bool pDisposing)
        {
            if (!this.mDisposed)
            {
                this.mDisposed = true;

                try
                {
                    if (pDisposing)
                    {
                    }
                }
                catch (Exception e)
                {
                    mLoggingService.GuardarLogError(e, mlogger);
                }
            }
        }

        #endregion
    }
}
