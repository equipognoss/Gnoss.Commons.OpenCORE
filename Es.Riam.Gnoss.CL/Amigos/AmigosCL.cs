using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Logica.BASE_BD;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.CL.Amigos
{
    public class AmigosCL : BaseCL
    {
        #region Miembros

        /// <summary>
        /// Clave MAESTRA de la caché
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.AMIGOS };

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        public AmigosCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AmigosCL> logger,ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        /// <param name="pPoolName">Nombre del pool a conectarse</param>
        public AmigosCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AmigosCL> logger, ILoggerFactory loggerFactory)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public AmigosCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AmigosCL> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication, logger,loggerFactory)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos

        /// <summary>
        /// Obtiene los GUIDs de los amigos que pertenecen a un proyecto
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        /// <param name="pProyecto">Identificador de proyecto</param>
        /// <returns>Dataset de identidades</returns>
        public List<Guid> ObtenerListaIdentidadesAmigosPertenecenProyecto(Guid pIdentidadID, Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.AMIGOSENPROYECTO, "_", pIdentidadID, "_", pProyectoID);
            // TODO: revisar
            List<Guid> amigos = (List<Guid>)ObtenerObjetoDeCache(rawKey, typeof(List<Guid>));
            if (amigos == null)
            {
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                amigos = idenCN.ObtenerListaIdentidadesAmigosPertenecenProyecto(pIdentidadID, pProyectoID);
                idenCN.Dispose();

                AgregarObjetoCache(rawKey, amigos);
            }
            return amigos;
        }

        /// <summary>
        /// Obtiene de la cache los amigos de un perfil
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pDataWrapperIdentidad">DataWrapperIdentidad de identidades </param>
        /// <param name="pDataWrapperPersona">DataSet de personas (Null para no cargarlo)</param>
        /// <param name="pTagDS">DataSet de tags (Null para no cargarlo)</param>
        /// <param name="pUsuarioDS">DataSet de usuarios (Null para no cargarlo)</param>
        /// <param name="pOrganizacionDW">DataSet de organizaciones (Null para no cargarlo)</param>
        /// <param name="pAmigosDS">DataSet de amigos (Null para no cargarlo)</param>
        /// <param name="pAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        /// <returns>TRUE si se ha cargado todo de la cache, FALSE en caso contrario</returns>
        public bool ObtenerAmigos(Guid pIdentidadID, DataWrapperIdentidad pDataWrapperIdentidad, DataWrapperPersona pDataWrapperPersona, DataWrapperOrganizacion pOrganizacionDW, DataWrapperAmigos pDataWrapperAmigos, bool pAmigosIdentidadOrganizacion, bool pSoloComprobar)
        {
            string rawKey = "";
            if (pAmigosIdentidadOrganizacion)
            {
                rawKey = string.Concat(NombresCL.AMIGOSORG, "_", pIdentidadID.ToString());
            }
            else
            {
                rawKey = string.Concat(NombresCL.AMIGOSPER, "_", pIdentidadID.ToString());
            }

            DataWrapperIdentidad dataWrapperIdentidad = null;
            DataWrapperPersona dataWrapperPersona = null;
            DataWrapperAmigos dataWrapperAmigos = null;

            DataWrapperOrganizacion organizacionDW = null;

            bool cargado = true;

            if (pDataWrapperIdentidad != null)
            {
                string rawKeyIdentidadDS = string.Concat(rawKey, "_", NombresCL.IDENTIDADDS);

                if (pSoloComprobar)
                {
                    cargado = ComprobarSiClaveExiste(rawKeyIdentidadDS);
                }
                else
                {
                    dataWrapperIdentidad = ObtenerObjetoDeCacheLocal(rawKeyIdentidadDS) as DataWrapperIdentidad;
                    if (dataWrapperIdentidad == null)
                    {
                        dataWrapperIdentidad = ObtenerObjetoDeCache(rawKeyIdentidadDS, typeof(DataWrapperIdentidad)) as DataWrapperIdentidad;
                        if (dataWrapperIdentidad != null)
                        {
                            AgregarObjetoCacheLocal(Guid.Empty, rawKeyIdentidadDS, dataWrapperIdentidad, pExpirationDate: DateTime.Now.AddHours(1));
                        }
                    }

                    if (dataWrapperIdentidad != null)
                    {
                        pDataWrapperIdentidad.Merge(dataWrapperIdentidad);
                    }
                    else
                    {
                        cargado = false;
                    }
                }
            }
            if (cargado && pDataWrapperAmigos != null)
            {
                string rawKeyAmigosDS = string.Concat(rawKey, "_", NombresCL.AMIGOSDS);

                if (pSoloComprobar)
                {
                    cargado = ComprobarSiClaveExiste(rawKeyAmigosDS);
                }
                else
                {
                    dataWrapperAmigos = ObtenerObjetoDeCacheLocal(rawKeyAmigosDS) as DataWrapperAmigos;
                    if (dataWrapperAmigos == null)
                    {
                        dataWrapperAmigos = ObtenerObjetoDeCache(rawKeyAmigosDS, typeof(DataWrapperAmigos)) as DataWrapperAmigos;
                        if (dataWrapperAmigos != null)
                        {
                            AgregarObjetoCacheLocal(Guid.Empty, rawKeyAmigosDS, dataWrapperIdentidad, pExpirationDate: DateTime.Now.AddHours(1));
                        }
                    }

                    if (dataWrapperAmigos != null)
                    {
                        pDataWrapperAmigos.Merge(dataWrapperAmigos);
                    }
                    else
                    {
                        cargado = false;
                    }
                }
            }
            if (cargado && pDataWrapperPersona != null)
            {
                string rawKeyPersonaDS = string.Concat(rawKey, "_", NombresCL.PERSONADS);

                if (pSoloComprobar)
                {
                    cargado = ComprobarSiClaveExiste(rawKeyPersonaDS);
                }
                else
                {
                    dataWrapperPersona = ObtenerObjetoDeCacheLocal(rawKeyPersonaDS) as DataWrapperPersona;
                    if (dataWrapperPersona == null)
                    {
                        dataWrapperPersona = ObtenerObjetoDeCache(rawKeyPersonaDS, typeof(DataWrapperPersona)) as DataWrapperPersona;
                        if (dataWrapperPersona != null)
                        {
                            AgregarObjetoCacheLocal(Guid.Empty, rawKeyPersonaDS, dataWrapperPersona, pExpirationDate: DateTime.Now.AddHours(1));
                        }
                    }

                    if (dataWrapperPersona != null)
                    {
                        pDataWrapperPersona.Merge(dataWrapperPersona);
                    }
                    else
                    {
                        cargado = false;
                    }
                }

            }

            if (cargado && pOrganizacionDW != null)
            {
                string rawKeyOrganizacionDS = string.Concat(rawKey, "_", NombresCL.ORGANIZACIONDS);
                if (pSoloComprobar)
                {
                    cargado = ComprobarSiClaveExiste(rawKeyOrganizacionDS);
                }
                else
                {
                    organizacionDW = (DataWrapperOrganizacion)ObtenerObjetoDeCacheLocal(rawKeyOrganizacionDS);
                    if (organizacionDW == null)
                    {
                        organizacionDW = (DataWrapperOrganizacion)ObtenerObjetoDeCache(rawKeyOrganizacionDS, typeof(DataWrapperOrganizacion));
                        if (organizacionDW != null)
                        {
                            AgregarObjetoCacheLocal(Guid.Empty, rawKeyOrganizacionDS, organizacionDW, pExpirationDate: DateTime.Now.AddHours(1));
                        }
                    }

                    if (organizacionDW != null)
                    {
                        pOrganizacionDW.Merge(organizacionDW);
                    }
                    else
                    {
                        cargado = false;
                    }
                }
            }

            return cargado;
        }

        /// <summary>
        /// Obtiene de la cache los amigos de un perfil
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pIdentidadDS">Comprobar IdentidadDS</param>
        /// <param name="pPersonaDS">Comprobar PersonaDS</param>
        /// <param name="pOrganizacionDS">Comprobar OrganizacionDS</param>
        /// <param name="pAmigosDS">Comprobar AmigosDS</param>
        /// <param name="pAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        /// <returns>TRUE si se ha cargado todo de la cache, FALSE en caso contrario</returns>
        public bool EstanCargadosAmigos(Guid pIdentidadID, bool pIdentidadDS, bool pPersonaDS, bool pOrganizacionDS, bool pAmigosDS, bool pAmigosIdentidadOrganizacion)
        {
            string rawKey = "";
            if (pAmigosIdentidadOrganizacion)
            {
                rawKey = string.Concat(NombresCL.AMIGOSORG, "_", pIdentidadID.ToString());
            }
            else
            {
                rawKey = string.Concat(NombresCL.AMIGOSPER, "_", pIdentidadID.ToString());
            }

            if (pIdentidadDS)
            {
                string rawKeyIdentidadDS = string.Concat(rawKey, "_", NombresCL.IDENTIDADDS);
                if (!ExisteClaveEnCache(rawKeyIdentidadDS))
                {
                    return false;
                }
            }

            if (pPersonaDS)
            {
                string rawKeyPersonaDS = string.Concat(rawKey, "_", NombresCL.PERSONADS);
                if (!ExisteClaveEnCache(rawKeyPersonaDS))
                {
                    return false;
                }
            }

            if (pOrganizacionDS)
            {
                string rawKeyOrganizacionDS = string.Concat(rawKey, "_", NombresCL.ORGANIZACIONDS);
                if (!ExisteClaveEnCache(rawKeyOrganizacionDS))
                {
                    return false;
                }
            }

            if (pAmigosDS)
            {
                string rawKeyAmigosDS = string.Concat(rawKey, "_", NombresCL.AMIGOSDS);

                if (!ExisteClaveEnCache(rawKeyAmigosDS))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Obtiene de la cache los amigos de un perfil
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pDataWrapperIdentidad">DataSet de identidades (Null para no cargarlo)</param>
        /// <param name="pPersonaDS">DataSet de personas (Null para no cargarlo)</param>
        /// <param name="pTagDS">DataSet de tags (Null para no cargarlo)</param>
        /// <param name="pUsuarioDS">DataSet de usuarios (Null para no cargarlo)</param>
        /// <param name="pOrganizacionDS">DataSet de organizaciones (Null para no cargarlo)</param>
        /// <param name="pAmigosDS">DataSet de amigos (Null para no cargarlo)</param>
        /// <param name="pAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        /// <returns>TRUE si se ha cargado todo de la cache, FALSE en caso contrario</returns>
        public bool ObtenerAmigosEIdentidadesEnMisProyectosPrivados(Guid pIdentidadID, DataWrapperIdentidad pDataWrapperIdentidad, bool pAmigosIdentidadOrganizacion, bool pSoloComprobar)
        {
            string rawKey = "";
            if (pAmigosIdentidadOrganizacion)
            {
                rawKey = string.Concat(NombresCL.AMIGOSORGEIDENTPROYPRIV, "_", pIdentidadID.ToString());
            }
            else
            {
                rawKey = string.Concat(NombresCL.AMIGOSPEREIDENTPROYPRIV, "_", pIdentidadID.ToString());
            }

            DataWrapperIdentidad dataWrapperIdentidad;

            bool cargado = true;

            if (pDataWrapperIdentidad != null)
            {
                string rawKeyIdentidadDS = string.Concat(rawKey, "_", NombresCL.IDENTIDADDS);
                if (pSoloComprobar)
                {
                    cargado = ComprobarSiClaveExiste(rawKeyIdentidadDS);
                }
                else
                {
                    dataWrapperIdentidad = (DataWrapperIdentidad)ObtenerObjetoDeCacheLocal(rawKeyIdentidadDS);
                    if (dataWrapperIdentidad == null)
                    {
                        dataWrapperIdentidad = (DataWrapperIdentidad)ObtenerObjetoDeCache(rawKeyIdentidadDS, typeof(DataWrapperIdentidad));
                        if (dataWrapperIdentidad != null)
                        {
                            AgregarObjetoCacheLocal(Guid.Empty, rawKeyIdentidadDS, dataWrapperIdentidad, pExpirationDate: DateTime.Now.AddHours(1));
                        }
                    }
                    if (dataWrapperIdentidad != null)
                    {
                        pDataWrapperIdentidad.Merge(dataWrapperIdentidad);
                    }
                    else
                    {
                        cargado = false;
                    }
                }
            }

            return cargado;
        }

        public void AgregarCacheAutocompletarInvalidar(Guid pClave)
        {
            string rawkey = NombresCL.AUTOCOMPLETARINVALIDARCACHE;
            AgregarObjetoCache(rawkey, pClave.ToString());
        }

        public Guid ObtenerCacheAutocompletarInvalidar()
        {
            string rawkey = NombresCL.AUTOCOMPLETARINVALIDARCACHE;
            string valor = ObtenerObjetoDeCache(rawkey, typeof(string)) as string;
            Guid respuesta = Guid.Empty;
            Guid.TryParse(valor, out respuesta);
            return respuesta;
        }


        /// <summary>
        /// Agrega a la cache los amigos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pIdentidadDW">DataSet de identidades (Null para no guardarlo)</param>
        /// <param name="pDataWrapperPersona">DataSet de personas (Null para no guardarlo)</param>
        /// <param name="pTagDS">DataSet de tags (Null para no guardarlo)</param>
        /// <param name="pUsuarioDS">DataSet de usuarios (Null para no guardarlo)</param>
        /// <param name="pOrganizacionDW">DataSet de organizaciones (Null para no guardarlo)</param>
        /// <param name="pAmigosDS">DataSet de amigos (Null para no guardarlo)</param>
        /// <param name="pAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        public void AgregarAmigos(Guid pIdentidadID, DataWrapperIdentidad pIdentidadDW, DataWrapperPersona pDataWrapperPersona, DataWrapperOrganizacion pOrganizacionDW, DataWrapperAmigos pDataWrapperAmigos, bool pAmigosIdentidadOrganizacion)
        {
            string rawKey = "";
            if (pAmigosIdentidadOrganizacion)
            {
                rawKey = string.Concat(NombresCL.AMIGOSORG, "_", pIdentidadID.ToString());
            }
            else
            {
                rawKey = string.Concat(NombresCL.AMIGOSPER, "_", pIdentidadID.ToString());
            }

            if (pIdentidadDW != null)
            {
                string rawKeyIdentidadDS = string.Concat(rawKey, "_", NombresCL.IDENTIDADDS);
                AgregarObjetoCache(rawKeyIdentidadDS, pIdentidadDW);
                AgregarObjetoCacheLocal(Guid.Empty, rawKeyIdentidadDS, pIdentidadDW, pExpirationDate: DateTime.Now.AddHours(1));
            }

            if (pDataWrapperPersona != null)
            {
                string rawKeyPersonaDS = string.Concat(rawKey, "_", NombresCL.PERSONADS);
                pDataWrapperPersona.CargaRelacionesPerezosasCache();
                AgregarObjetoCache(rawKeyPersonaDS, pDataWrapperPersona);
                AgregarObjetoCacheLocal(Guid.Empty, rawKeyPersonaDS, pDataWrapperPersona, pExpirationDate: DateTime.Now.AddHours(1));
            }

            if (pOrganizacionDW != null)
            {
                string rawKeyOrganizacionDS = string.Concat(rawKey, "_", NombresCL.ORGANIZACIONDS);
                AgregarObjetoCache(rawKeyOrganizacionDS, pOrganizacionDW);
                AgregarObjetoCacheLocal(Guid.Empty, rawKeyOrganizacionDS, pOrganizacionDW, pExpirationDate: DateTime.Now.AddHours(1));
            }
            if (pDataWrapperAmigos != null)
            {
                string rawKeyAmigosDS = string.Concat(rawKey, "_", NombresCL.AMIGOSDS);
                AgregarObjetoCache(rawKeyAmigosDS, pDataWrapperAmigos);
                AgregarObjetoCacheLocal(Guid.Empty, rawKeyAmigosDS, pDataWrapperAmigos, pExpirationDate: DateTime.Now.AddHours(1));
            }


        }

        /// <summary>
        /// Agrega a la cache los amigos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pDataWrapperIdentidad">DataSet de identidades (Null para no guardarlo)</param>
        /// <param name="pPersonaDS">DataSet de personas (Null para no guardarlo)</param>
        /// <param name="pTagDS">DataSet de tags (Null para no guardarlo)</param>
        /// <param name="pUsuarioDS">DataSet de usuarios (Null para no guardarlo)</param>
        /// <param name="pOrganizacionDS">DataSet de organizaciones (Null para no guardarlo)</param>
        /// <param name="pAmigosDS">DataSet de amigos (Null para no guardarlo)</param>
        /// <param name="pAmigosIdentidadOrganizacion">TRUE si se deben cargar los amigos de la organización o FALSE si se deben cargar los de la persona</param>
        public void AgregarAmigosEIdentidadesEnMisProyectosPrivados(Guid pIdentidadID, DataWrapperIdentidad pDataWrapperIdentidad, bool pAmigosIdentidadOrganizacion)
        {
            string rawKey = "";
            if (pAmigosIdentidadOrganizacion)
            {
                rawKey = string.Concat(NombresCL.AMIGOSORGEIDENTPROYPRIV, "_", pIdentidadID.ToString());
            }
            else
            {
                rawKey = string.Concat(NombresCL.AMIGOSPEREIDENTPROYPRIV, "_", pIdentidadID.ToString());
            }

            if (pDataWrapperIdentidad != null)
            {
                string rawKeyIdentidadDS = string.Concat(rawKey, "_", NombresCL.IDENTIDADDS);
                AgregarObjetoCache(rawKeyIdentidadDS, pDataWrapperIdentidad);
                AgregarObjetoCacheLocal(Guid.Empty, rawKeyIdentidadDS, pDataWrapperIdentidad, pExpirationDate: DateTime.Now.AddHours(1));
            }
        }

        /// <summary>
        /// Agrega a la cache los amigos de una identidad
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        public void InvalidarAmigos(Guid pIdentidadID)
        {
            string rawKeyPer = ObtenerClaveCache(string.Concat(NombresCL.AMIGOSPER, "_", pIdentidadID.ToString(), "_*"));
            string rawKeyOrg = ObtenerClaveCache(string.Concat(NombresCL.AMIGOSORG, "_", pIdentidadID.ToString(), "_*"));
            string rawKeyAmigosEnProyecto = ObtenerClaveCache(string.Concat(NombresCL.AMIGOSENPROYECTO, "_", pIdentidadID.ToString(), "_*"));

            List<string> claves = new List<string>();
            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(rawKeyPer.ToLower()).Result.ToList();
            }
            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Keys(rawKeyOrg.ToLower()).Result.ToList());
            }
            if (ClienteRedisLectura != null)
            {
                claves.AddRange(ClienteRedisLectura.Keys(rawKeyAmigosEnProyecto.ToLower()).Result.ToList());
            }

            InvalidarCachesMultiples(claves);

            try
            {
                InvalidarAmigosEIdentidadesEnMisProyectosPrivados(pIdentidadID);
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e, mlogger);
            }
        }
        
        /// <summary>
        /// Elimina de la cache los amigos e identidades de sus proys privados
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        public void InvalidarAmigosEIdentidadesEnMisProyectosPrivados(Guid pIdentidadID)
        {
            string rawKey = string.Concat(NombresCL.AMIGOSORGEIDENTPROYPRIV, "_", pIdentidadID.ToString());
            string rawKey2 = string.Concat(NombresCL.AMIGOSPEREIDENTPROYPRIV, "_", pIdentidadID.ToString());

            InvalidarCache(string.Concat(rawKey, "_", NombresCL.IDENTIDADDS), true);
            InvalidarCache(string.Concat(rawKey2, "_", NombresCL.IDENTIDADDS), true);
            InvalidarCache(string.Concat(rawKey, "_", NombresCL.AMIGOSDS), true);
            InvalidarCache(string.Concat(rawKey2, "_", NombresCL.AMIGOSDS), true);
        }


        /// <summary>
        /// Invalida la cache de los amigos que pertenecen a un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void InvalidarAmigosPertenecenProyecto(Guid pProyectoID)
        {
            string patronClave = ObtenerClaveCache(string.Concat(NombresCL.AMIGOSENPROYECTO, "_*_", pProyectoID.ToString()));

            List<string> claves = new List<string>();

            if (ClienteRedisLectura != null)
            {
                claves = ClienteRedisLectura.Keys(patronClave.ToLower()).Result.ToList();
            }

            InvalidarCachesMultiples(claves);
        }
        public void RefrescarCacheAmigos(Guid pIdentidadID, EntityContextBASE pEntityContextBASE, IAvailableServices pAvailableServices, bool pEsGnossOrganizador = false)
        {
            BaseComunidadCN baseComunidadCN = new BaseComunidadCN("base", mEntityContext, mLoggingService, pEntityContextBASE, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<BaseComunidadCN>(), mLoggerFactory);
            try
            {
                baseComunidadCN.InsertarFilaColaRefrescoCacheEnRabbitMQ(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosAmigosPrivados, TipoBusqueda.Contactos, $"{pIdentidadID.ToString()}#{pEsGnossOrganizador}", pAvailableServices);
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, "Fallo al insertar en Rabbit, insertamos en la base de datos BASE, tabla colaRefrescoCache",mlogger);
                baseComunidadCN.InsertarFilaEnColaRefrescoCache(ProyectoAD.MetaProyecto, TiposEventosRefrescoCache.CambiosAmigosPrivados, TipoBusqueda.Contactos, $"{pIdentidadID.ToString()}#{pEsGnossOrganizador}");
            }
            baseComunidadCN.Dispose();
        }

        #region Recomendaciones

        /// <summary>
        /// Obtiene el html de los contactos recomendados del usuario.
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        /// <returns>Html de contactos recomendados</returns>
        public string ObtenerContactosRecomendados(Guid pIdentidadID)
        {
            string rawKey = string.Concat(NombresCL.CONTACTOSRECOMEN, "_", pIdentidadID);
            byte[] htmlCompri = (byte[])ObtenerObjetoDeCache(rawKey, typeof(byte[]));
            string html = null;

            if (htmlCompri != null)
            {
                html = (string)UtilZip.UnZip(htmlCompri);
            }

            return html;
        }

        /// <summary>
        /// Guarda el html de los contactos recomendados del usuario.
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        public void GuardarContactosRecomendados(Guid pIdentidadID, string pHtml)
        {
            string rawKey = string.Concat(NombresCL.CONTACTOSRECOMEN, "_", pIdentidadID);
            byte[] htmlCompri = UtilZip.Zip(pHtml);

            AgregarObjetoCache(rawKey, htmlCompri);
        }

        /// <summary>
        /// Invalida el html de los contactos recomendados del usuario.
        /// </summary>
        /// <param name="pIdentidadMyGnossActual">Identificador de la identidad actual en MyGnoss</param>
        public void InvalidarContactosRecomendados(Guid pIdentidadID)
        {
            string rawKey = string.Concat(NombresCL.CONTACTOSRECOMEN, "_", pIdentidadID);
            InvalidarCache(rawKey, true);
        }

        #endregion

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
