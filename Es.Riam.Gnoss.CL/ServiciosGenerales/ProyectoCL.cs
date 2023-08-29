using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Faceta;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.VistaVirtualDS;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Facetado;
using Es.Riam.Gnoss.Logica.Parametro;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Usuarios;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.CL.ServiciosGenerales
{
    public class ProyectoCL : BaseCL, IDisposable
    {
        #region Constantes

        public static string CLAVE_CACHE_LISTA_RUTAPESTANYAS_INVALIDAR = "ListaRutaPestanyasInvalidar_";
        public static string CLAVE_CACHE_LISTA_RUTAPESTANYAS_REGISTRAR = "ListaRutaPestanyasRegistrar_";
        public static string CLAVE_CACHE_CONTADOR_PERSONAS_ORGANIZACIONES_COMUNIDAD = "ContadorPersonasOrganizaciones_";
        public static string CLAVE_CACHE_CONTADOR_RECURSOS_COMUNIDAD = "ContadorRecursos_";

        #endregion

        #region Miembros

        /// <summary>
        /// Clase de negocio
        /// </summary>
        private ProyectoCN mProyectoCN = null;

        /// <summary>
        /// Clave MAESTRA de la cache
        /// </summary>
        private readonly string[] mMasterCacheKeyArray = { NombresCL.PROYECTO };

        /// <summary>
        /// Lógica de parámetro
        /// </summary>
        private ParametroCN mParametroCN;

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ConfigService mConfigService;


        #endregion

        #region Constructores

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        public ProyectoCL(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mVirtuosoAD = virtuosoAD;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        public ProyectoCL(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos LIVE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones en el LIVE: FALSE. En caso contrario TRUE</param>
        public ProyectoCL(string pFicheroConfiguracionBD, string pPoolName, EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, pPoolName, entityContext, loggingService, redisCacheWrapper, configService, servicesUtilVirtuosoAndReplication)
        {

            mVirtuosoAD = virtuosoAD;
            mConfigService = configService;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
        }

        #endregion

        #region Metodos

        public CommunityModel ObtenerComunidadMVC(Guid pProyectoID)
        {
            string rawKey = "ComunidadMVC3_" + pProyectoID;

            CommunityModel ficha = (CommunityModel)ObtenerObjetoDeCache(rawKey);

            if (ficha == null || CommunityModel.LastCacheVersion > ficha.CacheVersion)
            {
                return null;
            }

            return ficha;
        }

        public void AgregarComunidadMVC(Guid pProyectoID, CommunityModel pComunidadMVC)
        {
            string rawKey = "ComunidadMVC3_" + pProyectoID;

            pComunidadMVC.CacheVersion = CommunityModel.LastCacheVersion;

            AgregarObjetoCache(rawKey, pComunidadMVC);
        }

        public void InvalidarComunidadMVC(Guid pProyectoID)
        {
            string rawKey = "ComunidadMVC3_" + pProyectoID;

            InvalidarCache(rawKey);
        }
        public void InvalidarTodasComunidadesMVC()
        {
            string rawKey = "ComunidadMVC3_";

            InvalidarCacheQueContengaCadena(rawKey);
        }

        public object ObtenerCabeceraMVC(Guid pProyectoID)
        {
            string rawKey = "CabeceraMVC_" + pProyectoID;

            object ficha = ObtenerObjetoDeCache(rawKey);

            return ficha;
        }

        public void AgregarCabeceraMVC(Guid pProyectoID, object pCabeceraMVC)
        {
            string rawKey = "CabeceraMVC_" + pProyectoID;

            AgregarObjetoCache(rawKey, pCabeceraMVC, BaseCL.DURACION_CACHE_UN_DIA);
        }

        public void InvalidarCabeceraMVC(Guid pProyectoID)
        {
            string rawKey = "CabeceraMVC_" + pProyectoID;

            InvalidarCache(rawKey);
        }

        public AutenticationModel ObtenerFormularioRegistroMVC(Guid pProyectoID)
        {
            string rawKey = "FormularioRegistroMVC_" + pProyectoID;
            AutenticationModel ficha = (AutenticationModel)ObtenerObjetoDeCache(rawKey);
            return ficha;
        }

        public void AgregarFormularioRegistroMVC(Guid pProyectoID, AutenticationModel pFormularioRegistroMVC)
        {
            string rawKey = "FormularioRegistroMVC_" + pProyectoID;
            AgregarObjetoCache(rawKey, pFormularioRegistroMVC);
        }

        public void InvalidarFormularioRegistroMVC(Guid pProyectoID)
        {
            string rawKey = "FormularioRegistroMVC_" + pProyectoID;
            InvalidarCache(rawKey);
        }



        public Dictionary<Guid, CommunityModel> ObtenerFichasComunidadMVC(List<Guid> pListaComunidadesID)
        {
            Dictionary<Guid, CommunityModel> listaFichas = new Dictionary<Guid, CommunityModel>();
            if (pListaComunidadesID.Count > 0)
            {
                Dictionary<Guid, string> keysComunidades = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaComunidadesID.Count];
                int i = 0;
                foreach (Guid idComunidad in pListaComunidadesID)
                {
                    string clave = ObtenerClaveCache("ComunidadMVC3_" + idComunidad);
                    keysComunidades.Add(idComunidad, clave.ToLower());
                    listaClaves[i] = clave.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(object));
                foreach (Guid idRecurso in keysComunidades.Keys)
                {
                    string clave = keysComunidades[idRecurso];
                    listaFichas.Add(idRecurso, (CommunityModel)(objetosCache[clave]));
                }
            }
            return listaFichas;
        }

        /// <summary>
        /// Agrega un evento a los eventos de acciones pendientes de un usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <param name="pTipoProyectoEventoAccion">Evento</param>
        /// <returns>Lista de acciones</returns>
        public void AgregarEventosAccionProyectoPorProyectoYUsuarioID(Guid pProyectoID, Guid pUsuarioID, TipoProyectoEventoAccion pTipoProyectoEventoAccion)
        {
            string rawKey = string.Concat(NombresCL.PROYECTOEVENTOACCION, "_", pProyectoID, pUsuarioID);
            List<TipoProyectoEventoAccion> listaEventos = ObtenerObjetoDeCache(rawKey) as List<TipoProyectoEventoAccion>;

            if (listaEventos == null)
            {
                listaEventos = new List<TipoProyectoEventoAccion>();
            }
            listaEventos.Add(pTipoProyectoEventoAccion);

            AgregarObjetoCache(rawKey, listaEventos, 120);
        }

        /// <summary>
        /// Obtiene de cache la lista de proyectos con acciones externas
        /// </summary>
        /// <returns>Lista de Identificadores de proyecto con acciones externas configuradas</returns>
        public List<Guid> ObtenerListaIDsProyectosConAccionesExternas()
        {
            string rawKey = string.Concat(NombresCL.PROYECTOSACCIONESEXTERNAS);
            List<Guid> listaProyectosAccionesExternas = ObtenerObjetoDeCache(rawKey) as List<Guid>;

            if (listaProyectosAccionesExternas == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                listaProyectosAccionesExternas = ProyectoCN.ObtenerListaIDsProyectosConAccionesExternas();
                AgregarObjetoCache(rawKey, listaProyectosAccionesExternas);
            }

            return listaProyectosAccionesExternas;
        }

        /// <summary>
        /// Invalida la cache de la lista de proyectos con acciones externas
        /// </summary>
        public void InvalidarCacheListaIDsProyectosConAccionesExternas()
        {
            string rawKey = string.Concat(NombresCL.PROYECTOSACCIONESEXTERNAS);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene los eventos de acciones pendientes de un usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <param name="pUsuarioID">Clave del usuario</param>
        /// <returns>Lista de acciones</returns>
        public List<TipoProyectoEventoAccion> ObtenerEventosAccionProyectoPorProyectoYUsuarioID(Guid pProyectoID, Guid pUsuarioID)
        {
            string rawKey = string.Concat(NombresCL.PROYECTOEVENTOACCION, "_", pProyectoID, pUsuarioID);
            List<TipoProyectoEventoAccion> listaEventos = ObtenerObjetoDeCache(rawKey) as List<TipoProyectoEventoAccion>;

            return listaEventos;
        }

        /// Obtiene los eventos de acciones pendientes de un usuario en un proyecto
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto</param>
        /// <returns>DataSet de Proyecto con la tabla EventoProyecto</returns>
        public void InvalidarCacheEventosAccionProyectoPorProyectoYUsuarioID(Guid pProyectoID, Guid pUsuarioID)
        {
            string rawKey = string.Concat(NombresCL.PROYECTOEVENTOACCION, "_", pProyectoID, pUsuarioID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns></returns>
        public int ObtenerTablaBaseProyectoIDProyectoPorID(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.TABLABASEPROYECTOID, "_", pProyectoID);

            // Compruebo si está en la caché
            object id = ObtenerObjetoDeCacheLocal(rawKey);

            if (id == null)
            {
                id = ObtenerObjetoDeCache(rawKey);
                if (id != null && (int)id != -1)
                {
                    AgregarObjetoCacheLocal(pProyectoID, rawKey, id);
                }
            }

            if (id == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                id = ProyectoCN.ObtenerTablaBaseProyectoIDProyectoPorID(pProyectoID);

                if (id != null && (int)id != -1)
                {
                    AgregarObjetoCache(rawKey, id);
                    AgregarObjetoCacheLocal(pProyectoID, rawKey, id);
                }
            }

            return (int)id;
        }

        /// <summary>
        /// Obtiene los tipo de documentos permitidos para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public List<TiposDocumentacion> ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(Guid pProyectoID, TipoRolUsuario pTipoRol)
        {
            string rawKey = string.Concat("TiposDocumentosPermitidos_", pProyectoID, "_" + (short)pTipoRol);

            // Compruebo si está en la caché
            List<TiposDocumentacion> listaTipos = ObtenerObjetoDeCacheLocal(rawKey) as List<TiposDocumentacion>;

            if (listaTipos == null)
            {
                listaTipos = ObtenerObjetoDeCache(rawKey) as List<TiposDocumentacion>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaTipos);
            }

            if (listaTipos == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    listaTipos = ProyectoCN.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(ProyectoIDPadreEcosistema.Value, pTipoRol);
                }
                else
                {
                    listaTipos = ProyectoCN.ObtenerTiposDocumentosPermitidosUsuarioEnProyecto(pProyectoID, pTipoRol);
                }

                AgregarObjetoCache(rawKey, listaTipos);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaTipos);
            }

            return listaTipos;
        }

        /// <summary>
        /// Invalida los tipo de documentos permitidos para un rol de usuario en un determinado proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto en el que se hace la comprobación</param>
        /// <param name="pTipoRol">Tipo de rol del usuario actual</param>
        /// <returns>Lista con los tipos de documentos permitidos para el rol</returns>
        public void InvalidarTiposDocumentosPermitidosUsuarioEnProyecto(Guid pProyectoID, TipoRolUsuario pTipoRol)
        {
            string rawKey = string.Concat("TiposDocumentosPermitidos_", pProyectoID, "_" + (short)pTipoRol);
            InvalidarCache(rawKey);

            rawKey = string.Concat("TiposOntologiasPermitidas_", pProyectoID, "_" + (short)pTipoRol);
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Invalidar las secciones de la home de un proyecto tipo catálogo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public void InvalidarSeccionesHomeCatalogoDeProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("SeccionesHomeCatalogo_", pProyectoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene el nombre de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Nombre de proyecto</returns>
        public string ObtenerNombreDeProyectoID(Guid pProyectoID)
        {
            string rawKey = string.Concat("NombreProyecto_", pProyectoID);

            // Compruebo si está en la caché
            string nombre = ObtenerObjetoDeCacheLocal(rawKey) as string;

            if (string.IsNullOrEmpty(nombre))
            {
                nombre = ObtenerObjetoDeCache(rawKey) as string;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, nombre);
            }

            if (string.IsNullOrEmpty(nombre))
            {
                // Si no está, lo cargo y lo almaceno en la cache
                nombre = ProyectoCN.ObtenerNombreDeProyectoID(pProyectoID);
                AgregarObjetoCache(rawKey, nombre);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, nombre);
            }

            return nombre;
        }

        /// <summary>
        /// Obtiene el nombre corto de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Nombre corto de proyecto</returns>
        public string ObtenerURLPropiaProyecto(Guid pProyectoID, string pIdiomaActual)
        {
            string rawKey = string.Concat("UrlProyecto_", pProyectoID);

            // Compruebo si está en la caché
            string urlPropia = ObtenerObjetoDeCacheLocal(rawKey) as string;

            if (urlPropia == null)
            {
                urlPropia = ObtenerObjetoDeCache(rawKey) as string;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, urlPropia, true);
            }

            if (urlPropia == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                urlPropia = ProyectoCN.ObtenerURLPropiaProyecto(pProyectoID);

                if (urlPropia == null)
                {
                    urlPropia = "";
                }

                AgregarObjetoCache(rawKey, urlPropia);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, urlPropia, true);
            }

            if (string.IsNullOrEmpty(pIdiomaActual))
            {


                pIdiomaActual = "es";

            }
            

            return UtilCadenas.ObtenerUrlPropiaDeIdioma(urlPropia, pIdiomaActual);
        }

        /// <summary>
        /// Obtiene el nombre corto de un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Nombre corto de proyecto</returns>
        public string ObtenerNombreCortoProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.NOMBRECORTOPROYECTO, "_", pProyectoID);

            // Compruebo si está en la caché
            string nombreCortoProy = ObtenerObjetoDeCache(rawKey) as string;
            if (string.IsNullOrEmpty(nombreCortoProy))
            {
                // Si no está, lo cargo y lo almaceno en la cache
                nombreCortoProy = ProyectoCN.ObtenerNombreCortoProyecto(pProyectoID);
                AgregarObjetoCache(rawKey, nombreCortoProy);
            }

            return nombreCortoProy;
        }

        /// <summary>
        /// Obtiene el DataSet con de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerProyectoPorID(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("ProyectoCompletoPorID3_", pProyectoID);

            if (pProyectoID.Equals(Guid.Empty))
            {
                //En el servicio de facetas llegaban peticiones con guid.empty. 
                return new DataWrapperProyecto();
            }

            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;

            if (dataWrapperProyecto == null || (dataWrapperProyecto.ListaProyecto != null && dataWrapperProyecto.ListaProyecto.Count == 0) || dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistente == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = ProyectoCN.ObtenerProyectoPorID(pProyectoID);
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    DataWrapperProyecto dataWrapperProyectoPadre = new DataWrapperProyecto();
                    dataWrapperProyectoPadre = ProyectoCN.ObtenerProyectoPorID(ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperProyectoHijaDW(dataWrapperProyecto, dataWrapperProyectoPadre, pProyectoID);
                }

                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerProyectoIDPorNombreCorto(string pNombreCorto)
        {
            string rawKey = string.Concat("ProyectoID_", pNombreCorto);

            Guid? proyectoID = ObtenerObjetoDeCacheLocal(rawKey) as Guid?;

            if (!proyectoID.HasValue || proyectoID.Value.Equals(Guid.Empty))
            {
                proyectoID = ObtenerObjetoDeCache(rawKey) as Guid?;
                if (proyectoID.HasValue && !proyectoID.Value.Equals(Guid.Empty))
                {
                    AgregarObjetoCacheLocal(proyectoID.Value, rawKey, proyectoID);
                }
            }

            if (!proyectoID.HasValue || proyectoID.Value.Equals(Guid.Empty))
            {
                // Si no está, lo cargo y lo almaceno en la cache
                proyectoID = ProyectoCN.ObtenerProyectoIDPorNombre(pNombreCorto);
                if (proyectoID.HasValue && !proyectoID.Value.Equals(Guid.Empty))
                {
                    AgregarObjetoCache(rawKey, proyectoID);
                    AgregarObjetoCacheLocal(proyectoID.Value, rawKey, proyectoID);
                }
            }

            return proyectoID.Value;
        }


        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoYProyectoSuperiorIDs(string pNombreCorto)
        {
            string rawKey = string.Concat("ProyectoYProyectoSuperiorID_", pNombreCorto);

            List<Guid> pListaIds = ObtenerObjetoDeCacheLocal(rawKey) as List<Guid>;

            if (pListaIds != null && pListaIds.Count > 0)
            {
                pListaIds = ObtenerObjetoDeCache(rawKey) as List<Guid>;
                if (pListaIds != null && pListaIds.Count > 0)
                {
                    AgregarObjetoCacheLocal(pListaIds.FirstOrDefault(), rawKey, pListaIds);
                }
            }

            if (pListaIds == null || pListaIds.Count == 0)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                pListaIds = ProyectoCN.ObtenerProyectoYProyectoSuperiorIDs(pNombreCorto);
                if (pListaIds != null && pListaIds.Count > 0)
                {
                    AgregarObjetoCache(rawKey, pListaIds);
                    AgregarObjetoCacheLocal(pListaIds.FirstOrDefault(), rawKey, pListaIds);
                }
            }

            return pListaIds;
        }

        /// <summary>
        /// Obtiene el identificador de un proyecto a partir de su nombre CORTO pasado por parámetro
        /// </summary>
        /// <param name="pNombreCorto">Nombre corto del proyecto</param>
        /// <returns>Identificador del proyecto</returns>
        public List<Guid> ObtenerProyectoIDOrganizacionIDPorNombreCorto(string pNombreCorto)
        {
            string rawKey = string.Concat("ProyectoID_OrgID_", pNombreCorto);

            // Compruebo si está en la caché
            List<Guid> listaProyectoIDOrgID = ObtenerObjetoDeCache(rawKey) as List<Guid>;
            if (listaProyectoIDOrgID == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                listaProyectoIDOrgID = ProyectoCN.ObtenerProyectoIDOrganizacionIDPorNombreCorto(pNombreCorto);
                AgregarObjetoCache(rawKey, listaProyectoIDOrgID);
            }

            return listaProyectoIDOrgID;
        }

        /// <summary>
        /// Obtiene el id autonumérico que se le asigna a cada proyecto para crear la tabla BASE
        /// </summary>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerNivelesCertificacionRecursosProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.NIVELESCERTIFICACIONRECURSO, "_", pProyectoID);
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = new DataWrapperProyecto();
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    dataWrapperProyecto = ProyectoCN.ObtenerNivelesCertificacionRecursosProyecto(ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHijaNivelesCertificacion(dataWrapperProyecto, pProyectoID);
                }
                else
                {
                    dataWrapperProyecto = ProyectoCN.ObtenerNivelesCertificacionRecursosProyecto(pProyectoID);
                    if (dataWrapperProyecto != null)
                    {
                        dataWrapperProyecto.CargaRelacionesPerezosasCache();
                    }
                }

                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Invalida la cache de los niveles de certificación de recursos del proyecto indicado
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        public void InvalidarNivelesCertificacionRecursosProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.NIVELESCERTIFICACIONRECURSO, "_", pProyectoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene el identificador de la base de recursos de la comunidad.
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organizacion</param>
        /// <param name="pProyectoID">Identificador de proyecto</param>
        /// <returns>Identificador de la base de recursos de la comunidad</returns>
        public Guid ObtenerBaseRecursosProyecto(Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            string baseRecursosID = null;
            if (pProyectoID != ProyectoAD.MetaProyecto)
            {
                string rawKey = string.Concat(NombresCL.BASERECURSOSID, "_", pProyectoID);

                // Compruebo si está en la caché
                baseRecursosID = ObtenerObjetoDeCache(rawKey) as string;
                if (baseRecursosID == null)
                {
                    // Si no está, lo cargo y lo almaceno en la cache
                    DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    DataWrapperDocumentacion dataWrapperDocumentacion = new DataWrapperDocumentacion();
                    docCN.ObtenerBaseRecursosProyecto(dataWrapperDocumentacion, pProyectoID, pOrganizacionID, pUsuarioID);
                    docCN.Dispose();
                    if (dataWrapperDocumentacion.ListaBaseRecursosProyecto.Count > 0)
                    {
                        baseRecursosID = dataWrapperDocumentacion.ListaBaseRecursosProyecto.First().BaseRecursosID.ToString();
                    }
                    AgregarObjetoCache(rawKey, baseRecursosID);
                }
            }
            if (baseRecursosID != null)
            {
                return new Guid(baseRecursosID);
            }
            else return Guid.Empty;
        }

        /// <summary>
        /// Obtiene de caché las ontologías de los proyectos a los que pertenece un determinado perfil de usuario o las añade si no están.
        /// </summary>
        /// <param name="pPerfil">Identificador del perfil del usuario</param>
        /// <returns>Lista con las ontologías de los proyectos</returns>
        public List<OntologiaProyecto> ObtenerOntologiasProyectosUsuario(Guid pPerfil)
        {
            string rawKey = string.Concat("OntologiasProyectoUsuario", "_", pPerfil);
            List<OntologiaProyecto> listaOntologias = ObtenerObjetoDeCache(rawKey) as List<OntologiaProyecto>;

            // Compruebo si está en la caché
            if (listaOntologias == null)
            {
                // Si no está, lo cargo y lo almaceno en la caché
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                listaOntologias = proyectoCN.ObtenerOntologiasPorPerfilID(pPerfil);
                proyectoCN.Dispose();
                AgregarObjetoCache(rawKey, listaOntologias);
            }

            return listaOntologias;
        }

        /// <summary>
        /// Obtiene de cache el html de los paneles de autopromocion
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pHtmlPanel">Html de los paneles de autopromocion</param>
        /// <returns>Devuelve True si estaba guardado en cache</returns>
        public bool ObtenerProyectoAutoPromocion(Guid pProyectoID, out string pHtmlPanel)
        {
            pHtmlPanel = "";
            // Compruebo si está en la caché
            string rawKey = string.Concat(NombresCL.PROYECTOAUTOPROMOCION, "_", pProyectoID);
            object objetoCache = ObtenerObjetoDeCache(rawKey);
            if (objetoCache != null)
            {
                pHtmlPanel = objetoCache as string;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Agrega a la cache el html de los paneles de autopromocion
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pHtmlPanel">Html de los paneles de autopromocion</param>
        public void AgregarProyectoAutoPromocion(Guid pProyectoID, string pHtmlPanel)
        {
            string rawKey = string.Concat(NombresCL.PROYECTOAUTOPROMOCION, "_", pProyectoID);
            AgregarObjetoCache(rawKey, pHtmlPanel);
        }

        /// <summary>
        /// Obtiene de cache un gestor de identidades con los administradores del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pGestorIdentidades">Gestor de identidades con los administradores del proyecto</param>
        /// <returns>Devuelve True si estaba guardado en cache</returns>
        public bool ObtenerHTMLAdministradoresProyecto(Guid pProyectoID, out string pHtmlPanel)
        {
            pHtmlPanel = "";
            // Compruebo si está en la caché
            string rawKey = string.Concat(NombresCL.HTMLADMINISTRADORESPROYECTO, "_", pProyectoID);
            object objetoCache = ObtenerObjetoDeCache(rawKey);
            if (objetoCache != null)
            {
                pHtmlPanel = objetoCache as string;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Agrega a la cache un gestor de identidades con los administradores del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pGestorIdentidades">Gestor de identidades con los administradores del proyecto</param>
        public void AgregarHTMLAdministradoresProyecto(Guid pProyectoID, string pHtmlPanel)
        {
            string rawKey = string.Concat(NombresCL.HTMLADMINISTRADORESPROYECTO, "_", pProyectoID);
            AgregarObjetoCache(rawKey, pHtmlPanel);
        }

        /// <summary>
        /// Agrega a la cache un gestor de identidades con los administradores del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pGestorIdentidades">Gestor de identidades con los administradores del proyecto</param>
        public void InvalidarHTMLAdministradoresProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.HTMLADMINISTRADORESPROYECTO, "_", pProyectoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Invalida los proyectos de la cache
        /// </summary>
        public void InvalidarFilaProyecto(Guid pProyectoID)
        {
            string rawKey = NombresCL.PROYECTOSPORID + "_" + pProyectoID;
            InvalidarCache(rawKey);

            rawKey = string.Concat("ProyectoCompletoPorID3_", pProyectoID);
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Invalida las pestañas de un proyecto pasado como parametro
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public void InvalidarPestanyasProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("PestanyasProyectoMVC_", pProyectoID);
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        public List<Guid> ObtenerOntologiasPermitidasIdentidadEnProyecto(Guid pIdentidadEnProyID, Guid pIdentidadEnMyGnossID, Guid pProyectoID, TipoRolUsuario pTipoRol, bool pIdentidadDeOtroProyecto)
        {
            string rawKey = "ProyectoOntologiasEcosistema";

            // Compruebo si está en la caché
            Dictionary<Guid, Guid> listaOntologiasEcosistema = ObtenerObjetoDeCacheLocal(rawKey) as Dictionary<Guid, Guid>;
            if (listaOntologiasEcosistema == null)
            {
                listaOntologiasEcosistema = ProyectoCN.ObtenerOntologiasEcosistema();
                AgregarObjetoCache(rawKey, listaOntologiasEcosistema);
            }

            return ProyectoCN.ObtenerOntologiasPermitidasIdentidadEnProyecto(pIdentidadEnProyID, pIdentidadEnMyGnossID, pProyectoID, pTipoRol, pIdentidadDeOtroProyecto, listaOntologiasEcosistema);
        }

        public void InvalidarOntologiasEcosistema()
        {
            InvalidarCache("ProyectoOntologiasEcosistema");
        }

        /// <summary>
        /// Obtiene la fila de un proyecto desde cache
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public AD.EntityModel.Models.ProyectoDS.Proyecto ObtenerFilaProyecto(Guid pProyectoID)
        {
            string rawKey = NombresCL.PROYECTOSPORID + "_" + pProyectoID;
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperProyecto;

            if (dataWrapperProyecto == null)
            {
                dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }
            //Si no hay nada en cache guardamos un dataWrapper con el proyecto que nos viene por parametros
            if (dataWrapperProyecto == null)
            {
                Proyecto filaProy = ProyectoCN.ObtenerProyectoPorIDCargaLigera(pProyectoID);

                if (filaProy != null)
                {
                    // Si no está, lo cargo y lo almaceno en la cache
                    dataWrapperProyecto = new DataWrapperProyecto();
                    dataWrapperProyecto.ListaProyecto.Add(filaProy);
                    if (dataWrapperProyecto != null)
                    {
                        dataWrapperProyecto.CargaRelacionesPerezosasCache();
                    }
                    AgregarObjetoCache(rawKey, dataWrapperProyecto);
                    AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
                }
                else
                {
                    dataWrapperProyecto = new DataWrapperProyecto();
                }
            }

            List<Proyecto> filasProyecto = dataWrapperProyecto.ListaProyecto.Where(proyecto => proyecto.ProyectoID.Equals(pProyectoID)).ToList();
            mEntityContext.UsarEntityCache = false;
            if (filasProyecto.Count > 0)
            {
                return filasProyecto.First();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa un perfil(NO incluye myGnoss)
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>Lista de los proyectos en los que participa un perfil</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerCacheListaProyectosPerfil(Guid pPerfilID)
        {
            string rawKey = NombresCL.LISTAPROYECTOSPERFIL + "_" + pPerfilID;

            // Compruebo si está en la caché
            Dictionary<string, KeyValuePair<string, short>> listaProyectosPerfil = ObtenerObjetoDeCache(rawKey) as Dictionary<string, KeyValuePair<string, short>>;
            //Si no hay nada en cache guardamos un dataset con el proyecto que nos viene por parametros
            if (listaProyectosPerfil == null)
            {
                listaProyectosPerfil = ProyectoCN.ObtenerListaProyectosParticipaPerfilUsuario(pPerfilID);
                AgregarObjetoCache(rawKey, listaProyectosPerfil);
            }

            return listaProyectosPerfil;
        }

        /// <summary>
        /// Obtiene una lista con los proyectos en los que participa el usuarioID
        /// </summary>
        /// <param name="pUsuarioID">Identificado del Usuario</param>
        /// <returns>Lista de los proyectos en los que participa el Usuario</returns>
        public Dictionary<string, KeyValuePair<string, short>> ObtenerCacheListaProyectosUsuario(Guid pUsuarioID)
        {
            string rawKey = NombresCL.LISTAPROYECTOSUSUARIO + "_" + pUsuarioID;

            // Compruebo si está en la caché
            Dictionary<string, KeyValuePair<string, short>> listaProyectosUsuario = ObtenerObjetoDeCache(rawKey) as Dictionary<string, KeyValuePair<string, short>>;
            //Si no hay nada en cache guardamos un dataset con el proyecto que nos viene por parametros
            if (listaProyectosUsuario == null)
            {
                listaProyectosUsuario = ProyectoCN.ObtenerListaProyectosParticipaUsuario(pUsuarioID);
                AgregarObjetoCache(rawKey, listaProyectosUsuario);
            }

            return listaProyectosUsuario;
        }

        /// <summary>
        /// Calcula el rol permitido para un usuario en un proyecto, teniendo en cuenta los roles de los grupos a los que pertenece
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pLogin">Login del usuario (NULL si no se quiere comprobar que el usuario está autenticado)</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pComprobarAutenticacionUsuario">TRUE si debe comprobar que el usuario está autenticado, FALSE en caso contrario</param>
        /// <returns>Rol permitido final del usuario en el proyecto</returns>
        public ulong CalcularRolFinalUsuarioEnProyecto(Guid pUsuarioID, string pLogin, Guid pOrganizacionID, Guid pProyectoID, DataWrapperUsuario pDataWrapperUsuario)
        {
            //Declaramos roles por defecto 
            ulong rolPermitidoUsuario = 0;
            ulong rolDenegadoUsuario = 0;

            if ((pDataWrapperUsuario == null) || (pDataWrapperUsuario.ListaProyectoRolUsuario.Count == 0))
            {
                return ulong.Parse("0000000000000000");
            }
            AD.EntityModel.Models.UsuarioDS.ProyectoRolUsuario filaProyectoRolUsuario = pDataWrapperUsuario.ListaProyectoRolUsuario.FirstOrDefault(proyRolUs => proyRolUs.OrganizacionGnossID.Equals(pOrganizacionID) && proyRolUs.ProyectoID.Equals(pProyectoID) && proyRolUs.UsuarioID.Equals(pUsuarioID));

            if (filaProyectoRolUsuario.RolPermitido != null)
            {
                rolPermitidoUsuario = Convert.ToUInt64(filaProyectoRolUsuario.RolPermitido, 16);
            }

            if (filaProyectoRolUsuario.RolDenegado != null)
            {
                rolDenegadoUsuario = Convert.ToUInt64(filaProyectoRolUsuario.RolDenegado, 16);
            }

            //3 Calculamos el rol final
            ulong rolPermitidoFinal = rolPermitidoUsuario;
            ulong rolDenegadoFinal = rolDenegadoUsuario;

            return (rolPermitidoFinal & ~(rolDenegadoFinal));
        }

        /// <summary>
        /// 
        /// </summary>
        public void InvalidarCacheListaProyectosPerfil(Guid pPerfilID)
        {
            string rawKey = NombresCL.LISTAPROYECTOSPERFIL + "_" + pPerfilID;
            InvalidarCache(rawKey);
        }

        public void InvalidarCacheListaProyectosListaPerfiles(List<Guid> pListaPerfilIDs)
        {
            if (pListaPerfilIDs != null && pListaPerfilIDs.Count > 0)
            {
                List<string> listaClaves = new List<string>();

                foreach (Guid perfilID in pListaPerfilIDs)
                {
                    string rawKey = NombresCL.LISTAPROYECTOSPERFIL + "_" + perfilID;
                    listaClaves.Add(rawKey);
                }

                InvalidarCache(listaClaves);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InvalidarCacheListaProyectosUsuario(Guid pUsuarioID)
        {
            string rawKey = NombresCL.LISTAPROYECTOSUSUARIO + "_" + pUsuarioID;
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// 
        /// </summary>
        public void InvalidarCacheListaProyectosListaUsuarios(List<Guid> pListaUsuarioIDs)
        {
            if (pListaUsuarioIDs != null && pListaUsuarioIDs.Count > 0)
            {
                List<string> listaClaves = new List<string>();

                foreach (Guid usuarioID in pListaUsuarioIDs)
                {
                    string rawKey = NombresCL.LISTAPROYECTOSUSUARIO + "_" + usuarioID;
                    listaClaves.Add(rawKey);
                }

                InvalidarCache(listaClaves);
            }
        }

        /// <summary>
        /// Obtiene la fila de un proyecto desde cache
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public AD.EntityModel.Models.ProyectoDS.Proyecto ObtenerTipoAccesoPorNombreCorto(string pNombreCorto)
        {
            string rawKey = NombresCL.PROYECTOSPORID;
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = new DataWrapperProyecto();
                dataWrapperProyecto.ListaProyecto.Add(ProyectoCN.ObtenerProyectoPorIDCargaLigera(ObtenerProyectoIDPorNombreCorto(pNombreCorto)));
                dataWrapperProyecto.CargaRelacionesPerezosasCache();
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }
            List<AD.EntityModel.Models.ProyectoDS.Proyecto> filasProyecto = dataWrapperProyecto.ListaProyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombreCorto)).ToList();
            if (filasProyecto.Count == 0)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                DataWrapperProyecto dataWrapperProyectoNuevo = new DataWrapperProyecto();
                dataWrapperProyectoNuevo.ListaProyecto.Add(ProyectoCN.ObtenerProyectoPorIDCargaLigera(ObtenerProyectoIDPorNombreCorto(pNombreCorto)));
                dataWrapperProyecto.Merge(dataWrapperProyectoNuevo);
                dataWrapperProyecto.CargaRelacionesPerezosasCache();
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
                proyCN.Dispose();
                filasProyecto = dataWrapperProyecto.ListaProyecto.Where(proyecto => proyecto.NombreCorto.Equals(pNombreCorto)).ToList();
            }

            mEntityContext.UsarEntityCache = false;
            return filasProyecto[0];
        }

        public bool ObtenerUsuarioBloqueadoProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            string rawKey = string.Concat(NombresCL.USUARIOBLOQUEADOPROYECTO, "_", pProyectoID, "_", pUsuarioID);

            // Compruebo si está en la caché
            bool? usuBloqueado = ObtenerObjetoDeCache(rawKey) as bool?;
            if (!usuBloqueado.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                usuBloqueado = proyCN.EstaUsuarioBloqueadoEnProyecto(pUsuarioID, pProyectoID);
                proyCN.Dispose();

                AgregarObjetoCache(rawKey, usuBloqueado);
            }

            return usuBloqueado.Value;
        }

        public void InvalidarUsuarioBloqueadoProyecto(Guid pProyectoID, Guid pUsuarioID)
        {
            string rawKey = string.Concat(NombresCL.USUARIOBLOQUEADOPROYECTO, "_", pProyectoID, "_", pUsuarioID);

            InvalidarCache(rawKey);
        }

        public TipoAcceso ObtenerTipoAccesoProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.TIPOACCESO, "_", pProyectoID);

            // Compruebo si está en la caché
            short? tipoAcceso = ObtenerObjetoDeCache(rawKey) as short?;
            if (!tipoAcceso.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                tipoAcceso = (short)proyCN.ObtenerTipoAccesoProyecto(pProyectoID);
                proyCN.Dispose();
                AgregarObjetoCache(rawKey, tipoAcceso.Value);
            }

            return (TipoAcceso)tipoAcceso.Value;
        }

        /// <summary>
        /// Obtiene las secciones de la home de un proyecto tipo catálogo
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperProyecto ObtenerSeccionesHomeCatalogoDeProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("SeccionesHomeCatalogo_", pProyectoID);

            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = ProyectoCN.ObtenerSeccionesHomeCatalogoDeProyecto(pProyectoID);
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene el tipo de proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public TipoProyecto ObtenerTipoProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("TipoProy_", pProyectoID);

            // Compruebo si está en la caché
            TipoProyecto? tipoProy = ObtenerObjetoDeCache(rawKey) as TipoProyecto?;
            if (!tipoProy.HasValue)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                tipoProy = ProyectoCN.ObtenerTipoProyecto(pProyectoID);
                AgregarObjetoCache(rawKey, tipoProy);
            }

            return tipoProy.Value;
        }

        /// <summary>
        /// Obtiene de la cache el HTML con el listado de la página mis comunidades para el perfil dado
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns>HTML con mis proyectos</returns>
        public string ObtenerMisProyectos(Guid pPerfilID)
        {
            string rawKey = string.Concat(NombresCL.MISCOMUNIDADES, "_", pPerfilID.ToString());
            return (string)ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Agrega a la cache el HTML con el listado de la página mis comunidades para el perfil dado
        /// </summary>
        /// <param name="pHTML">HTML con el listado</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        public void AgregarMisProyectos(string pHTML, Guid pPerfilID)
        {
            string rawKey = string.Concat(NombresCL.MISCOMUNIDADES, "_", pPerfilID.ToString());
            AgregarObjetoCache(rawKey, pHTML);
        }

        /// <summary>
        /// Invalida la cache con el HTML con el listado de la página mis comunidades para el perfil dado
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        public void InvalidarMisProyectos(Guid pPerfilID)
        {
            string rawKey = string.Concat(NombresCL.MISCOMUNIDADES, "_", pPerfilID.ToString());
            InvalidarCache(rawKey);

            InvalidarProyectosRecomendados(pPerfilID);
        }
        /// <summary>
        /// Invalida la cache con el HTML con el listado de la página mis comunidades para el perfil dado
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        public void InvalidarMisProyectosListaPerfiles(List<Guid> pListaPerfilIDs)
        {
            if (pListaPerfilIDs != null && pListaPerfilIDs.Count > 0)
            {
                List<string> listaClaves = new List<string>();

                foreach (Guid perfilID in pListaPerfilIDs)
                {
                    string rawKey = string.Concat(NombresCL.MISCOMUNIDADES, "_", perfilID.ToString());
                    listaClaves.Add(rawKey);
                }

                InvalidarCache(listaClaves);
                InvalidarProyectosRecomendadosListaPerfiles(pListaPerfilIDs);
            }
        }

        /// <summary>
        /// Invalida la cache de las clausulas del registro de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public void InvalidarCacheClausulasRegitroProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.CLAUSULASREGITROPROYECTO, pProyectoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Invalida la cache de la politica de cookies del proyecto
        /// </summary>
        /// <param name="pProyectoID"></param>
        public void InvalidarCachePoliticaCookiesProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat(NombresCL.POLITICACOOKIESPROYECTO, pProyectoID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene las clausulas adicionales de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerClausulasRegitroProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat(NombresCL.CLAUSULASREGITROPROYECTO, pProyectoID);

            // Compruebo si está en la caché
            DataWrapperUsuario usuarioDW = ObtenerObjetoDeCache(rawKey) as DataWrapperUsuario;

            if (usuarioDW != null)
            {
                DataWrapperUsuario usuarioAuxDS = new DataWrapperUsuario();

                try
                {
                    usuarioAuxDS.Merge(usuarioDW);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de cache, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    usuarioDW = usuarioAuxDS;
                }
                catch { usuarioDW = null; }
            }

            if (usuarioDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                usuarioDW = usuarioCN.ObtenerClausulasRegitroProyecto(pProyectoID);
                usuarioCN.Dispose();
                AgregarObjetoCache(rawKey, usuarioDW);
            }
            mEntityContext.UsarEntityCache = false;
            return usuarioDW;
        }

        /// <summary>
        /// Obtiene la política de cookies de un proyecto (del metaproyecto si no tiene el proyecto)
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de usuario con las clausulas</returns>
        public DataWrapperUsuario ObtenerPoliticaCookiesProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat(NombresCL.POLITICACOOKIESPROYECTO, pProyectoID);

            // Compruebo si está en la caché
            DataWrapperUsuario usuarioDW = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperUsuario;
            if (usuarioDW == null)
            {
                usuarioDW = ObtenerObjetoDeCache(rawKey) as DataWrapperUsuario;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, usuarioDW);
            }

            if (usuarioDW != null)
            {
                DataWrapperUsuario usuarioAuxDW = new DataWrapperUsuario();

                try
                {
                    usuarioAuxDW.Merge(usuarioDW);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de cache, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    usuarioDW = usuarioAuxDW;
                }
                catch { usuarioDW = null; }
            }

            if (usuarioDW == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                UsuarioCN usuarioCN = new UsuarioCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                usuarioDW = usuarioCN.ObtenerPoliticaCookiesProyecto(pProyectoID);
                usuarioCN.Dispose();
                AgregarObjetoCache(rawKey, usuarioDW);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, usuarioDW);
            }
            mEntityContext.UsarEntityCache = false;
            return usuarioDW;
        }

        /// <summary>
        /// Obtiene los gadgets de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerGadgetsProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("GadgetsProyecto_", pProyectoID);

            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperProyecto;

            if (dataWrapperProyecto == null)
            {
                dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }


            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = new DataWrapperProyecto();
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    ProyectoCN.ObtenerGadgetsProyecto(ProyectoIDPadreEcosistema.Value, dataWrapperProyecto);
                    ModificarDataWrapperComunidadHija(dataWrapperProyecto, pProyectoID);
                }
                else
                {
                    ProyectoCN.ObtenerGadgetsProyecto(pProyectoID, dataWrapperProyecto);
                }

                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los gadgets de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerPestanyasProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("PestanyasProyectoMVC_", pProyectoID);

            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperProyecto;

            bool tieneComunidadPadreConfigurada = TieneComunidadPadreConfigurada(pProyectoID);

            if (dataWrapperProyecto == null)
            {
                dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }


            if (dataWrapperProyecto != null)
            {
                DataWrapperProyecto dataWrapperProyectoAux = new DataWrapperProyecto();

                try
                {
                    dataWrapperProyectoAux.Merge(dataWrapperProyecto);
                    // Le asigno el creado en esta plataforma, porque si hay campos que no tenía el dataset de cache, luego da problemas cuando intentas acceder a ellos. 
                    // La comprobación de la estructura no sirve, porque la tabla sí contiene la columna, pero la fila no (mu raro). 
                    dataWrapperProyecto = dataWrapperProyectoAux;
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                catch { dataWrapperProyecto = null; }
            }


            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = new DataWrapperProyecto();
                if (tieneComunidadPadreConfigurada)
                {
                    ProyectoCN.ObtenerPestanyasProyecto(ProyectoIDPadreEcosistema.Value, dataWrapperProyecto);
                    ModificarDataWrapperComunidadHija(dataWrapperProyecto, pProyectoID);
                }
                else
                {
                    ProyectoCN.ObtenerPestanyasProyecto(pProyectoID, dataWrapperProyecto);
                }

                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }

                AgregarObjetoCache(rawKey, dataWrapperProyecto);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        private void ModificarDataWrapperComunidadHijaNivelesCertificacion(DataWrapperProyecto dataWrapperProyecto, Guid pProyectoID)
        {
            foreach (NivelCertificacion nivelCertificacion in dataWrapperProyecto.ListaNivelCertificacion)
            {
                nivelCertificacion.ProyectoID = pProyectoID;
            }
        }

        private void ModificarDataWrapperComunidadHija(DataWrapperProyecto dataWrapperProyecto, Guid pProyectoID)
        {
            //Obtengo la de los del proyecto
            DataWrapperProyecto dataWrapperProyectoHija = new DataWrapperProyecto();
            ProyectoCN.ObtenerPestanyasProyecto(pProyectoID, dataWrapperProyectoHija);
            if (dataWrapperProyectoHija.ListaProyecto.Count > 0)
            {
                dataWrapperProyectoHija.ListaProyecto.First().Estado = (short)EstadoProyecto.Abierto;
            }

            foreach (ProyectoPestanyaMenu proyectoPestanyaMenu in dataWrapperProyecto.ListaProyectoPestanyaMenu)
            {
                ProyectoPestanyaMenu proyectoPestanyaMenuHija = dataWrapperProyectoHija.ListaProyectoPestanyaMenu.Where(x => x.NombreCortoPestanya == proyectoPestanyaMenu.NombreCortoPestanya).FirstOrDefault();
                if (proyectoPestanyaMenuHija != null)
                {
                    proyectoPestanyaMenu.Ruta = proyectoPestanyaMenuHija.Ruta;
                }
                proyectoPestanyaMenu.ProyectoID = pProyectoID;
            }

            foreach (ProyectoPestanyaMenuRolGrupoIdentidades proyectoPestanyaMenuRolGrupoIdentidades in dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades)
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidades grupoPadrePestanya = mEntityContext.GrupoIdentidades.Where(x => x.GrupoID.Equals(proyectoPestanyaMenuRolGrupoIdentidades.GrupoID)).FirstOrDefault();
                if (grupoPadrePestanya != null)
                {
                    List<Guid> grupoIdHijas = mEntityContext.GrupoIdentidadesProyecto.Where(x => x.ProyectoID.Equals(pProyectoID)).Select(x => x.GrupoID).ToList();
                    foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades grupoHija in mEntityContext.GrupoIdentidades.Where(x => grupoIdHijas.Contains(x.GrupoID)).ToList())
                    {
                        if (grupoPadrePestanya.NombreCorto.Equals(grupoHija.NombreCorto))
                        {
                            mEntityContext.DetachObjectFromEntityContext(proyectoPestanyaMenuRolGrupoIdentidades);
                            proyectoPestanyaMenuRolGrupoIdentidades.GrupoID = grupoHija.GrupoID;
                        }
                    }
                }
            }

            foreach (ProyectoGadgetIdioma proyectoGadget in dataWrapperProyecto.ListaProyectoGadgetIdioma)
            {
                mEntityContext.DetachObjectFromEntityContext(proyectoGadget);
                proyectoGadget.ProyectoID = pProyectoID;
            }
            foreach (RecursosRelacionadosPresentacion recursosRelacionadosPresentacion in dataWrapperProyecto.ListaRecursosRelacionadosPresentacion)
            {
                mEntityContext.DetachObjectFromEntityContext(recursosRelacionadosPresentacion);
                recursosRelacionadosPresentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionPersonalizadoSemantico presentacion in dataWrapperProyecto.ListaPresentacionPersonalizadoSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionPestanyaMapaSemantico presentacion in dataWrapperProyecto.ListaPresentacionPestanyaMapaSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionMapaSemantico presentacion in dataWrapperProyecto.ListaPresentacionMapaSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionMosaicoSemantico presentacion in dataWrapperProyecto.ListaPresentacionMosaicoSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionPestanyaMosaicoSemantico presentacion in dataWrapperProyecto.ListaPresentacionPestanyaMosaicoSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionListadoSemantico presentacion in dataWrapperProyecto.ListaPresentacionListadoSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }
            foreach (PresentacionPestanyaListadoSemantico presentacion in dataWrapperProyecto.ListaPresentacionPestanyaListadoSemantico)
            {
                mEntityContext.DetachObjectFromEntityContext(presentacion);
                presentacion.ProyectoID = pProyectoID;
            }

            //Modificar los Gadgets
            foreach (ProyectoGadget proyectoGadget in dataWrapperProyecto.ListaProyectoGadget)
            {
                mEntityContext.DetachObjectFromEntityContext(proyectoGadget);
                proyectoGadget.ProyectoID = pProyectoID;
            }
            foreach (ProyectoGadgetContexto proyectoGadgetContexto in dataWrapperProyecto.ListaProyectoGadgetContexto)
            {
                mEntityContext.DetachObjectFromEntityContext(proyectoGadgetContexto);
                string ComunidadOrigen = proyectoGadgetContexto.ComunidadOrigen;
                Proyecto proyectoHijo = mEntityContext.Proyecto.Where(x => x.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
                ComunidadOrigen = ComunidadOrigen.ToLower().Replace(NombreProyectoPadreEcositema, proyectoHijo.NombreCorto);
                proyectoGadgetContexto.ComunidadOrigen = ComunidadOrigen;
                if (proyectoGadgetContexto.ProyectoOrigenID.Equals(ProyectoIDPadreEcosistema))
                {
                    proyectoGadgetContexto.ProyectoOrigenID = pProyectoID;
                }
                proyectoGadgetContexto.ProyectoID = pProyectoID;
            }

        }

        private void ModificarDataWrapperProyectoHijaDW(DataWrapperProyecto dataWrapperProyectoHija, DataWrapperProyecto dataWrapperProyectoPadre, Guid pProyectoID)
        {

            foreach (ProyectoPestanyaMenu proyectoPestanyaMenu in dataWrapperProyectoPadre.ListaProyectoPestanyaMenu)
            {
                ProyectoPestanyaMenu proyectoPestanyaMenuHija = dataWrapperProyectoHija.ListaProyectoPestanyaMenu.Where(x => x.NombreCortoPestanya == proyectoPestanyaMenu.NombreCortoPestanya).FirstOrDefault();
                if (proyectoPestanyaMenuHija != null)
                {
                    proyectoPestanyaMenu.Ruta = proyectoPestanyaMenuHija.Ruta;
                    proyectoPestanyaMenu.Activa = proyectoPestanyaMenuHija.Activa;
                    proyectoPestanyaMenu.Visible = proyectoPestanyaMenuHija.Visible;
                }
                proyectoPestanyaMenu.ProyectoID = pProyectoID;
            }

            if (dataWrapperProyectoHija.ListaProyecto.Count > 0)
            {
                dataWrapperProyectoHija.ListaProyecto.First().Estado = (short)EstadoProyecto.Abierto;
            }

            foreach (ProyectoPestanyaMenuRolGrupoIdentidades proyectoPestanyaMenuRolGrupoIdentidades in dataWrapperProyectoPadre.ListaProyectoPestanyaMenuRolGrupoIdentidades)
            {
                AD.EntityModel.Models.IdentidadDS.GrupoIdentidades grupoPadrePestanya = mEntityContext.GrupoIdentidades.Where(x => x.GrupoID.Equals(proyectoPestanyaMenuRolGrupoIdentidades.GrupoID)).FirstOrDefault();
                if (grupoPadrePestanya != null)
                {
                    List<Guid> grupoIdHijas = mEntityContext.GrupoIdentidadesProyecto.Where(x => x.ProyectoID.Equals(pProyectoID)).Select(x => x.GrupoID).ToList();
                    foreach (AD.EntityModel.Models.IdentidadDS.GrupoIdentidades grupoHija in mEntityContext.GrupoIdentidades.Where(x => grupoIdHijas.Contains(x.GrupoID)).ToList())
                    {
                        if (grupoPadrePestanya.NombreCorto.Equals(grupoHija.NombreCorto))
                        {
                            proyectoPestanyaMenuRolGrupoIdentidades.GrupoID = grupoHija.GrupoID;
                        }
                    }
                }
            }

            dataWrapperProyectoHija.ListaProyectoPestanyaCMS = dataWrapperProyectoPadre.ListaProyectoPestanyaCMS;

            dataWrapperProyectoHija.ListaProyectoPestanyaMenu = dataWrapperProyectoPadre.ListaProyectoPestanyaMenu;

            dataWrapperProyectoHija.ListaProyectoPestanyaBusqueda = dataWrapperProyectoPadre.ListaProyectoPestanyaBusqueda;

            dataWrapperProyectoHija.ListaProyectoPestanyaDashboardAsistente = dataWrapperProyectoPadre.ListaProyectoPestanyaDashboardAsistente;

            dataWrapperProyectoHija.ListaProyectoPestanyaMenuRolGrupoIdentidades = dataWrapperProyectoPadre.ListaProyectoPestanyaMenuRolGrupoIdentidades;

            dataWrapperProyectoHija.ListaProyectoPestanyaMenuRolIdentidad = dataWrapperProyectoPadre.ListaProyectoPestanyaMenuRolIdentidad;
        }

        private void ModificarDataWrapperProyectoHija(DataWrapperProyecto dataWrapperProyecto, Guid pProyectoID)
        {
            /*foreach (Proyecto proyecto in dataWrapperProyecto.ListaProyecto)
            {
                proyecto.ProyectoID = pProyectoID;
            }*/
            foreach (AdministradorProyecto administradorProyecto in dataWrapperProyecto.ListaAdministradorProyecto)
            {
                administradorProyecto.ProyectoID = pProyectoID;
            }
            foreach (AdministradorGrupoProyecto administradorGrupoProyecto in dataWrapperProyecto.ListaAdministradorGrupoProyecto)
            {
                administradorGrupoProyecto.ProyectoID = pProyectoID;
            }
            foreach (ProyectoAgCatTesauro proyectoAgCatTesauro in dataWrapperProyecto.ListaProyectoAgCatTesauro)
            {
                proyectoAgCatTesauro.ProyectoID = pProyectoID;
            }
            foreach (ProyectoCerradoTmp proyectoCerradoTmp in dataWrapperProyecto.ListaProyectoCerradoTmp)
            {
                proyectoCerradoTmp.ProyectoID = pProyectoID;
            }
            foreach (ProyectoCerrandose proyectoCerrandose in dataWrapperProyecto.ListaProyectoCerrandose)
            {
                proyectoCerrandose.ProyectoID = pProyectoID;
            }
            foreach (DatoExtraProyecto datoExtraProyecto in dataWrapperProyecto.ListaDatoExtraProyecto)
            {
                datoExtraProyecto.ProyectoID = pProyectoID;
            }
            foreach (DatoExtraProyectoOpcion datoExtraProyectoOpcion in dataWrapperProyecto.ListaDatoExtraProyectoOpcion)
            {
                datoExtraProyectoOpcion.ProyectoID = pProyectoID;
            }
            foreach (DatoExtraProyectoVirtuoso datoExtraProyectoVirtuoso in dataWrapperProyecto.ListaDatoExtraProyectoVirtuoso)
            {
                datoExtraProyectoVirtuoso.ProyectoID = pProyectoID;
            }
            foreach (ProyectoPasoRegistro proyectoPasoRegistro in dataWrapperProyecto.ListaProyectoPasoRegistro)
            {
                proyectoPasoRegistro.ProyectoID = pProyectoID;
            }
            foreach (CamposRegistroProyectoGenericos camposRegistroProyectoGenericos in dataWrapperProyecto.ListaCamposRegistroProyectoGenericos)
            {
                camposRegistroProyectoGenericos.ProyectoID = pProyectoID;
            }
            foreach (ProyectoPestanyaCMS proyectoPestanyaCMS in dataWrapperProyecto.ListaProyectoPestanyaCMS)
            {
                proyectoPestanyaCMS.ProyectoPestanyaMenu.ProyectoID = pProyectoID;
            }
            foreach (ProyectoPestanyaBusqueda proyectoPestanyaBusqueda in dataWrapperProyecto.ListaProyectoPestanyaBusqueda)
            {
                proyectoPestanyaBusqueda.ProyectoPestanyaMenu.ProyectoID = pProyectoID;
            }
            foreach (ProyectoPestanyaMenuRolGrupoIdentidades proyectoPestanyaMenuRolGrupoIdentidades in dataWrapperProyecto.ListaProyectoPestanyaMenuRolGrupoIdentidades)
            {
                proyectoPestanyaMenuRolGrupoIdentidades.ProyectoPestanyaMenu.ProyectoID = pProyectoID;
            }
            foreach (ProyectoPestanyaMenuRolIdentidad proyectoPestanyaMenuRolIdentidad in dataWrapperProyecto.ListaProyectoPestanyaMenuRolIdentidad)
            {
                proyectoPestanyaMenuRolIdentidad.ProyectoPestanyaMenu.ProyectoID = pProyectoID;
            }
            foreach (ProyectoEventoAccion proyectoEventoAccion in dataWrapperProyecto.ListaProyectoEventoAccion)
            {
                proyectoEventoAccion.ProyectoID = pProyectoID;
            }
            foreach (ProyectoSearchPersonalizado proyectoSearchPersonalizado in dataWrapperProyecto.ListaProyectoSearchPersonalizado)
            {
                proyectoSearchPersonalizado.ProyectoID = pProyectoID;
            }
            foreach (VistaVirtualProyecto vistaVirtualProyecto in dataWrapperProyecto.ListaVistaVirtualProyecto)
            {
                vistaVirtualProyecto.ProyectoID = pProyectoID;
            }
            foreach (ProyectoPestanyaMenu proyectoPestanyaMenu in dataWrapperProyecto.ListaProyectoPestanyaMenu)
            {
                proyectoPestanyaMenu.ProyectoID = pProyectoID;
            }
            foreach (AccionesExternasProyecto accionesExternasProyecto in dataWrapperProyecto.ListaAccionesExternasProyecto)
            {
                accionesExternasProyecto.ProyectoID = pProyectoID;
            }
            foreach (OntologiaProyecto ontologiaProyecto in dataWrapperProyecto.ListaOntologiaProyecto)
            {
                ontologiaProyecto.ProyectoID = pProyectoID;
            }
            foreach (ConfigAutocompletarProy configAutocompletarProy in dataWrapperProyecto.ListaConfigAutocompletarProy)
            {
                configAutocompletarProy.ProyectoID = pProyectoID;
            }
        }

        /// <summary>
        /// Obtiene las páginas html de un proyecto que se le pasa por parametros.
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con las páginas html del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerPaginasHtmlProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("PaginasHtml_", pProyectoID);

            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;

            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = ProyectoCN.ObtenerPaginasHtmlProyecto(pProyectoID);
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }
            //Arreglo TFG FRAN
            if (dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistente == null)
            {
                dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistente = new List<ProyectoPestanyaDashboardAsistente>();
            }
            if (dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistenteDataset == null)
            {
                dataWrapperProyecto.ListaProyectoPestanyaDashboardAsistenteDataset = new List<ProyectoPestanyaDashboardAsistenteDataset>();
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Invalida los gadget de un determinado proyecto.
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto que se va a invalidar</param>
        public void InvalidarGadgetsProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("GadgetsProyecto_", pProyectoID);
            InvalidarCache(rawKey);
            VersionarCacheLocal(pProyectoID);
        }

        public void InvalidarFiltrosOrdenesDeProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("FiltrosOrdenesDeProyectoMVC_", pProyectoID);
            InvalidarCache(rawKey);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Invalida la cache de usuarios de una organizacion con sus identidades
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        public void InvalidarCacheProyectosOrgCargaLigeraParaFiltros(Guid pOrganizacionID)
        {
            string rawKey = string.Concat("ProyectosDeOrganizacionCargaLigeraParaFiltros_", pOrganizacionID);
            InvalidarCache(rawKey);
        }

        /// <summary>
        /// Obtiene el GeneradorMenuComunidad de una comunidad
        /// </summary>
        /// <param name="pProyectoID"></param>
        /// <returns></returns>
        public Object ObtenerGeneradorMenuComunidad(Guid pProyectoID)
        {
            string rawKey = string.Concat("GeneradorMenuComunidad_", pProyectoID);

            // Compruebo si está en la caché
            return ObtenerObjetoDeCache(rawKey);
        }

        /// <summary>
        /// Agrega el GeneradorMenuComunidad de una comunidad
        /// </summary>
        /// <param name="GeneradorMenuComunidad"></param>
        /// <param name="pProyectoID"></param>
        public void AgregarGeneradorMenuComunidad(Object GeneradorMenuComunidad, Guid pProyectoID)
        {
            string rawKey = string.Concat("GeneradorMenuComunidad_", pProyectoID);
            AgregarObjetoCache(rawKey, GeneradorMenuComunidad);
        }

        /// <summary>
        /// Obtiene los filtros de ordenes disponibles para un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet de ProyectoDS con la tabla 'ProyectoPestanyaFiltroOrdenRecursos' cargada para un proyecto</returns>
        public DataWrapperProyecto ObtenerFiltrosOrdenesDeProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("FiltrosOrdenesDeProyectoMVC_", pProyectoID);
            mEntityContext.UsarEntityCache = true;
            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperProyecto;
            
            bool tieneComunidadPadreConfigurada = TieneComunidadPadreConfigurada(pProyectoID);

            if (dataWrapperProyecto == null)
            {
                dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }

            // Si no está, lo cargo y lo almaceno en la caché
            if (dataWrapperProyecto == null)
            {
                ProyectoCN proyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                
                if (tieneComunidadPadreConfigurada)
                {
                    dataWrapperProyecto = ProyectoCN.ObtenerFiltrosOrdenesDeProyecto(ProyectoIDPadreEcosistema.Value);
                }
                else
                {
                    dataWrapperProyecto = ProyectoCN.ObtenerFiltrosOrdenesDeProyecto(pProyectoID);
                }

                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Obtiene los tipos de recursos que no deben ir a la actividad reciente de la comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>DataSet con la tabla ProyTipoRecNoActivReciente cargada para el proyecto</returns>
        public DataWrapperProyecto ObtenerTiposRecursosNoActividadReciente(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("TiposRecursosNoActividadReciente", pProyectoID);

            //ProyectoDS proyectoDS = ObtenerObjetoDeCache(rawKey) as ProyectoDS;
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
            if (dataWrapperProyecto == null)
            {
                ProyectoCN proyectoCN = new ProyectoCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = proyectoCN.ObtenerTiposRecursosNoActividadReciente(pProyectoID);
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        /// <summary>
        /// Agrega a la cache el numero de resultados de un tipo de busqueda
        /// </summary>
        /// <param name="pRefrescoTipoBusqueda">Tipo de busqueda</param>
        /// <param name="pNumResultados">Numero de resultados</param>
        public void AgregarContadorComunidad(Guid pProyectoID, TipoBusqueda pRefrescoTipoBusqueda, int pNumResultados)
        {
            string rawKey = string.Concat("ContadorComunidad", "_", pProyectoID, "_", pRefrescoTipoBusqueda.ToString());
            AgregarObjetoCache(rawKey, pNumResultados);
        }


        /// <summary>
        /// Obtiene de cache el numero de resultados de un tipo de busqueda
        /// </summary>
        /// <param name="pRefrescoTipoBusqueda">Tipo de busqueda</param>
        /// <returns>Numero de resultados</returns>
        public int? ObtenerContadorComunidad(Guid pProyectoID, TipoBusqueda pRefrescoTipoBusqueda)
        {
            string rawKey = string.Concat("ContadorComunidad", "_", pProyectoID, "_", pRefrescoTipoBusqueda.ToString());

            int? numResultados = (int?)ObtenerObjetoDeCache(rawKey);

            if (numResultados == null && pRefrescoTipoBusqueda == TipoBusqueda.Recursos)
            {
                numResultados = ProyectoCN.ObtenerNumRecursosProyecto(pProyectoID);
                AgregarObjetoCache(rawKey, numResultados);
            }

            return numResultados;
        }

        /// <summary>
        /// Agrega a la cache el ultimo proyecto en el que ha estado un usuario
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        public void AgregarUltimoProyectoUsuario(Guid pProyectoID, Guid pUsuarioID, string pDominio)
        {
            string rawKey = string.Concat("UltimoProyecto", "_", pUsuarioID, "_", pDominio);
            AgregarObjetoCache(rawKey, pProyectoID);
        }

        /// <summary>
        /// Obtiene el ultimo proyecto en el que ha estado un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Identificador del proyecto</returns>
        public Guid ObtenerUltimoProyectoUsuario(Guid pUsuarioID, string pDominio)
        {
            string rawKey = string.Concat("UltimoProyecto", "_", pUsuarioID, "_", pDominio);

            Guid? proyectoID = ObtenerObjetoDeCache(rawKey) as Guid?;

            if (proyectoID.HasValue)
            {
                return proyectoID.Value;
            }
            else
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Devuelve las imágenes por defecto según el tipo de imagen por defecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Imagen por defecto según el tipo de imagen por defecto</returns>
        public Dictionary<short, Dictionary<Guid, string>> ObtenerTipoDocImagenPorDefecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("TipoDocImagenPorDefecto", "_", pProyectoID);

            Dictionary<short, Dictionary<Guid, string>> listaTipo = ObtenerObjetoDeCacheLocal(rawKey) as Dictionary<short, Dictionary<Guid, string>>;

            if (listaTipo == null)
            {
                listaTipo = ObtenerObjetoDeCache(rawKey) as Dictionary<short, Dictionary<Guid, string>>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaTipo);
            }

            if (listaTipo == null)
            {
                listaTipo = ProyectoCN.ObtenerTipoDocImagenPorDefecto(pProyectoID);
                AgregarObjetoCache(rawKey, listaTipo);
                AgregarObjetoCacheLocal(pProyectoID, rawKey, listaTipo);
            }

            return listaTipo;
        }

        /// <summary>
        /// Obtiene los parámetros de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <returns>Diccionario con los parámetros del proyecto</returns>
        public Dictionary<string, string> ObtenerParametrosProyecto(Guid pProyectoID)
        {
            string rawKey = string.Concat("ParametroProyecto", "_", pProyectoID);
            Dictionary<string, string> parametros = ObtenerObjetoDeCacheLocal(rawKey) as Dictionary<string, string>;

            if (parametros == null)
            {
                parametros = ObtenerObjetoDeCache(rawKey) as Dictionary<string, string>;
                AgregarObjetoCacheLocal(pProyectoID, rawKey, parametros);
            }

            if (parametros == null)
            {
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    parametros = ParametroCN.ObtenerParametrosProyecto(ProyectoIDPadreEcosistema.Value);
                }
                else
                {
                    parametros = ParametroCN.ObtenerParametrosProyecto(pProyectoID);
                }

                AgregarObjetoCache(rawKey, parametros);
                ParametroCN.Dispose();

                AgregarObjetoCacheLocal(pProyectoID, rawKey, parametros);
            }

            return parametros;
        }

        /// <summary>
        /// Obtiene los parámetros de un proyecto.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <param name="pOrganizacionID">ID de la organización del proyecto</param>
        /// <returns>Diccionario con los parámetros del proyecto</returns>
        public void InvalidarParametrosProyecto(Guid pProyectoID, Guid pOrganizacionID)
        {
            string rawKey = string.Concat("ParametroProyecto", "_", pProyectoID);
            InvalidarCache(rawKey, true);
            InvalidarCacheLocal(rawKey);
            VersionarCacheLocal(pProyectoID);
        }

        #region Recomendaciones

        /// <summary>
        /// Obtiene el html de los proyectos recomendados del usuario.
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil actual en MyGnoss</param>
        /// <returns>Html de proyectos recomendados</returns>
        public string ObtenerProyectosRecomendados(Guid pPerfilID)
        {
            string rawKey = string.Concat(NombresCL.COMUNIDADESRECOMEN, "_", pPerfilID);
            byte[] htmlCompri = (byte[])ObtenerObjetoDeCache(rawKey);
            string html = null;

            if (htmlCompri != null)
            {
                html = (string)UtilZip.UnZip(htmlCompri);
            }

            return html;
        }

        /// <summary>
        /// Guarda el html de los proyectos recomendados del usuario.
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil actual en MyGnoss</param>
        public void GuardarProyectosRecomendados(Guid pPerfilID, string pHtml)
        {
            string rawKey = string.Concat(NombresCL.COMUNIDADESRECOMEN, "_", pPerfilID);
            byte[] htmlCompri = UtilZip.Zip(pHtml);

            AgregarObjetoCache(rawKey, htmlCompri);
        }

        /// <summary>
        /// Invalida el html de los proyectos recomendados del usuario.
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil actual en MyGnoss</param>
        public void InvalidarProyectosRecomendados(Guid pPerfilID)
        {
            string rawKey = string.Concat(NombresCL.COMUNIDADESRECOMEN, "_", pPerfilID);
            InvalidarCache(rawKey, true);
        }

        public void InvalidarProyectosRecomendadosListaPerfiles(List<Guid> pListaPerfilIDs)
        {
            if (pListaPerfilIDs != null && pListaPerfilIDs.Count > 0)
            {
                List<string> listaClaves = new List<string>();

                foreach (Guid perfilID in pListaPerfilIDs)
                {
                    string rawKey = string.Concat(NombresCL.COMUNIDADESRECOMEN, "_", perfilID);
                    listaClaves.Add(rawKey);
                }

                InvalidarCache(listaClaves);
            }
        }

        #endregion

        #region Grafos

        /// <summary>
        /// Obtiene los gadgets de un proyecto que se le pasa por parametros
        /// </summary>
        /// <param name="pProyectoID">Clave del proyecto del que queremos obtener los gadgets</param>
        /// <returns>DataSet con los gadgets del proyecto que se pasa por parametros</returns>
        public DataWrapperProyecto ObtenerGrafosProyecto(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("ProyectoGrafoFichaRec_", pProyectoID);

            // Compruebo si está en la caché
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;

            if (dataWrapperProyecto == null)
            {
                // Si no está, lo cargo y lo almaceno en la cache
                dataWrapperProyecto = ProyectoCN.ObtenerGrafosProyecto(pProyectoID);
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
            }

            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        #endregion

        /// <summary>
        /// Carga la presentación de todos los documentos semántico en una comunidad
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>    
        public DataWrapperProyecto ObtenerPresentacionSemantico(Guid pProyectoID)
        {
            mEntityContext.UsarEntityCache = true;
            string rawKey = string.Concat("PresentacionSemantico", "_", pProyectoID);
            DataWrapperProyecto dataWrapperProyecto = ObtenerObjetoDeCacheLocal(rawKey) as DataWrapperProyecto;

            if (dataWrapperProyecto == null)
            {
                dataWrapperProyecto = ObtenerObjetoDeCache(rawKey) as DataWrapperProyecto;
                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }

            if (dataWrapperProyecto == null)
            {
                if (TieneComunidadPadreConfigurada(pProyectoID))
                {
                    dataWrapperProyecto = ProyectoCN.ObtenerPresentacionSemantico(ProyectoIDPadreEcosistema.Value);
                    ModificarDataWrapperComunidadHija(dataWrapperProyecto, pProyectoID);
                }
                else
                {
                    dataWrapperProyecto = ProyectoCN.ObtenerPresentacionSemantico(pProyectoID);
                }


                if (dataWrapperProyecto != null)
                {
                    dataWrapperProyecto.CargaRelacionesPerezosasCache();
                }
                AgregarObjetoCache(rawKey, dataWrapperProyecto);
                ParametroCN.Dispose();

                AgregarObjetoCacheLocal(pProyectoID, rawKey, dataWrapperProyecto);
            }
            mEntityContext.UsarEntityCache = false;
            return dataWrapperProyecto;
        }

        public void InvalidarPresentacionSemantico(Guid pProyectoID)
        {
            string rawKey = string.Concat("PresentacionSemantico", "_", pProyectoID);
            InvalidarCache(rawKey, true);

            VersionarCacheLocal(pProyectoID);
        }

        /// <summary>
        /// Agrega el contador de personas y organizaciones del pProyectoID
        /// </summary>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <param name="pNumPersonasYOrganizaciones">Número de Personas y Organizaciones del pProyectoID</param>
        /// <param name="pDuracion">Duración de la cache, si la recarga el componente, sin límite de tiempo, si la recarga el método obtener, solo 1 día.</param>
        public void AgregarContadorPersonasYOrganizacionesComunidad(Guid pProyectoID, int pNumPersonasYOrganizaciones, double pDuracion)
        {
            string rawKey = string.Concat(CLAVE_CACHE_CONTADOR_PERSONAS_ORGANIZACIONES_COMUNIDAD, pProyectoID, "_false");
            AgregarObjetoCache(rawKey, pNumPersonasYOrganizaciones, pDuracion);
        }

        public int? ObtenerContadorPersonasYOrganizacionesComunidad(string pUrlIntragnoss, Guid pProyectoID, TipoProyecto pTipoProyecto, bool pContarPersonasNoVisibles = false)
        {
            string rawKey = string.Concat(CLAVE_CACHE_CONTADOR_PERSONAS_ORGANIZACIONES_COMUNIDAD, pProyectoID, "_", pContarPersonasNoVisibles);

            int? numRecursosPersonasYOrganizaciones = ObtenerObjetoDeCache(rawKey) as int?;

            if (!numRecursosPersonasYOrganizaciones.HasValue)
            {
                ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                Dictionary<string, List<string>> listaFiltrosPers = new Dictionary<string, List<string>>();
                List<string> recursosTipo = new List<string>();
                recursosTipo.Add("Organizacion");
                recursosTipo.Add("Persona");
                listaFiltrosPers.Add("rdf:type", recursosTipo);

                // Agrega los filtros en dos FILTER Diferentes y si hay control de privacidad en la query se duplica este filtro.
                // Agregar filtros publicos o privados?
                try
                {
                    FacetadoDS facDS = new FacetadoDS();
                    FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    facCN.ObtieneNumeroResultados(facDS, "RecursosBusqueda", listaFiltrosPers, new List<string>(), false, true, false, UsuarioAD.Invitado.ToString(), new List<string>(), "", pTipoProyecto, true, true, TiposAlgoritmoTransformacion.Ninguno, null);

                    if ((facDS.Tables.Contains("NResultadosBusqueda")) && (facDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
                    {
                        Object numeroPersonas = facDS.Tables["NResultadosBusqueda"].Rows[0][0];
                        var f = numeroPersonas as string;
                        if (numeroPersonas is int)
                        {
                            numRecursosPersonasYOrganizaciones = (int)numeroPersonas;
                        }
                        else if (f != null && numeroPersonas is string)
                        {
                            numRecursosPersonasYOrganizaciones = int.Parse((string)numeroPersonas);
                        }
                    }

                    proyCL.Dispose();
                    facCN.Dispose();

                    AgregarObjetoCache(rawKey, numRecursosPersonasYOrganizaciones.Value, BaseCL.DURACION_CACHE_CUATRO_HORAS);
                }
                catch
                {
                    mLoggingService.GuardarLog("No se ha podido obtenero los numeros de recursos, personas y organizaciones");
                }
            }

            return numRecursosPersonasYOrganizaciones;
        }

        public void InvalidarCacheContadorPersonasYOrganizacionesComunidad(Guid pProyectoID)
        {
            string rawKey = string.Concat(CLAVE_CACHE_CONTADOR_PERSONAS_ORGANIZACIONES_COMUNIDAD, pProyectoID, "_false");
            InvalidarCache(rawKey, true);
        }

        public Dictionary<Guid, int?> ObtenerContadorPersonasYOrganizacionesComunidades(List<Guid> pListaComunidadesID, bool pContarPersonasNoVisibles = false)
        {
            Dictionary<Guid, int?> listaContadores = new Dictionary<Guid, int?>();
            if (pListaComunidadesID.Count > 0)
            {
                Dictionary<Guid, string> keysComunidades = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaComunidadesID.Count];
                int i = 0;
                foreach (Guid idComunidad in pListaComunidadesID)
                {
                    string rawKey = ObtenerClaveCache(string.Concat(CLAVE_CACHE_CONTADOR_PERSONAS_ORGANIZACIONES_COMUNIDAD, idComunidad, "_", pContarPersonasNoVisibles));
                    keysComunidades.Add(idComunidad, rawKey.ToLower());
                    listaClaves[i] = rawKey.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(object));
                foreach (Guid idRecurso in keysComunidades.Keys)
                {
                    string clave = keysComunidades[idRecurso];
                    listaContadores.Add(idRecurso, (int?)(objetosCache[clave]));
                }
            }
            return listaContadores;
        }

        public Dictionary<Guid, int?> ObtenerContadorRecursosComunidades(List<Guid> pListaComunidadesID)
        {
            Dictionary<Guid, int?> listaContadores = new Dictionary<Guid, int?>();
            if (pListaComunidadesID.Count > 0)
            {
                Dictionary<Guid, string> keysComunidades = new Dictionary<Guid, string>();
                string[] listaClaves = new string[pListaComunidadesID.Count];
                int i = 0;
                foreach (Guid idComunidad in pListaComunidadesID)
                {
                    string rawKey = ObtenerClaveCache(string.Concat(CLAVE_CACHE_CONTADOR_RECURSOS_COMUNIDAD, idComunidad));
                    keysComunidades.Add(idComunidad, rawKey.ToLower());
                    listaClaves[i] = rawKey.ToLower();
                    i++;
                }
                Dictionary<string, object> objetosCache = ObtenerListaObjetosCache(listaClaves, typeof(object));
                foreach (Guid idRecurso in keysComunidades.Keys)
                {
                    string clave = keysComunidades[idRecurso];
                    listaContadores.Add(idRecurso, (int?)(objetosCache[clave]));
                }
            }
            return listaContadores;
        }

        public void AgregarContadorRecursosComunidad(Guid pProyectoID, int pNumPersonas)
        {
            string rawKey = string.Concat(CLAVE_CACHE_CONTADOR_RECURSOS_COMUNIDAD, pProyectoID);
            AgregarObjetoCache(rawKey, pNumPersonas);
        }

        public int? ObtenerContadorRecursosComunidad(string pUrlIntragnoss, Guid pProyectoID, Guid pOrganizacionID, TipoProyecto pTipoProyecto, bool pEsMovil)
        {
            string rawKey = string.Concat(CLAVE_CACHE_CONTADOR_RECURSOS_COMUNIDAD, pProyectoID);
            int? numRecursosComunidad = ObtenerObjetoDeCache(rawKey) as int?;

            if (!numRecursosComunidad.HasValue)
            {
                Dictionary<string, List<string>> listaFiltros = new Dictionary<string, List<string>>();
                List<string> recursos = new List<string>();
                recursos.Add("Recurso");
                listaFiltros.Add("rdf:type", recursos);

                FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                //facetaCL.ObtenerOntologiasProyecto(facetaDS, pOrganizacionID, pProyectoID);
                List<OntologiaProyecto> listaOntologias = facetaCL.ObtenerOntologiasBuscablesProyecto(pOrganizacionID, pProyectoID);

                List<string> listaFiltrosExtra = new List<string>();
                foreach (OntologiaProyecto myrow in listaOntologias)
                //foreach (FacetaDS.OntologiaProyectoRow myrow in facetaDS.OntologiaProyecto.Rows) // TODO Daniel usar  listaOntologias y LINQ
                {
                    listaFiltrosExtra.Add(myrow.OntologiaProyecto1);
                }

                try
                {
                    //Si falla al obtener de virtuoso el numero de recursos de una comunidad, controlamos el error y no lo guardamos en cache
                    FacetadoCN facCN = new FacetadoCN(pUrlIntragnoss, pProyectoID.ToString(), mEntityContext, mLoggingService, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    FacetadoDS facDS = new FacetadoDS();
                    facCN.ObtieneNumeroResultados(facDS, "RecursosBusqueda", listaFiltros, listaFiltrosExtra, false, true, false, UsuarioAD.Invitado.ToString(), new List<string>(), null, pTipoProyecto, true, true, TiposAlgoritmoTransformacion.Ninguno, null, pEsMovil);

                    if ((facDS.Tables.Contains("NResultadosBusqueda")) && (facDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
                    {
                        Object numeroRecursos = facDS.Tables["NResultadosBusqueda"].Rows[0][0];
                        if (numeroRecursos is int)
                        {
                            numRecursosComunidad = (int)numeroRecursos;
                        }
                        else if (numeroRecursos is string)
                        {
                            numRecursosComunidad = int.Parse((string)numeroRecursos);
                        }
                    }

                    AgregarObjetoCache(rawKey, numRecursosComunidad, BaseCL.DURACION_CACHE_CUATRO_HORAS);
                }
                catch
                { }
            }

            return numRecursosComunidad;
        }

        public bool? TieneICactivada(Guid pProyectoID)
        {
            string rawKey = "IntegracionContinua_IntegracionContinuaActivada_" + pProyectoID;

            bool? tieneICActivada = (bool?)ObtenerObjetoDeCache(rawKey);

            return tieneICActivada;
        }

        public void AgregarIC(Guid pProyectoID, bool pActivada, bool pIncluirCaducidad = true)
        {
            string rawKey = "IntegracionContinua_IntegracionContinuaActivada_" + pProyectoID;

            if (pIncluirCaducidad)
            {
                AgregarObjetoCache(rawKey, pActivada, 86400); //Cache de un dia
            }
            else
            {
                AgregarObjetoCache(rawKey, pActivada);
            }
        }

        public bool? EsEntornoPruebas(Guid pProyectoID, string pEntorno)
        {
            string rawKey = "IntegracionContinua_EntornoPruebas_" + pProyectoID + "_" + pEntorno;

            bool? esEntornoPruebas = (bool?)ObtenerObjetoDeCache(rawKey);

            if (esEntornoPruebas == null)
            {
                return esEntornoPruebas;
            }

            return esEntornoPruebas;
        }

        public void AgregarEsEntornoPruebas(Guid pProyectoID, string pEntorno, bool pEsPruebas = true)
        {
            string rawKey = "IntegracionContinua_EntornoPruebas_" + pProyectoID + "_" + pEntorno;

            AgregarObjetoCache(rawKey, pEsPruebas);
        }

        public bool? EsEntornoPreproduccion(Guid pProyectoID, string pEntorno)
        {
            string rawKey = "IntegracionContinua_EntornoPre_" + pProyectoID + "_" + pEntorno;

            bool? esEntornoPreproduccion = (bool?)ObtenerObjetoDeCache(rawKey);

            if (esEntornoPreproduccion == null)
            {
                return esEntornoPreproduccion;
            }

            return esEntornoPreproduccion;
        }

        public void AgregarEsEntornoPreproduccion(Guid pProyectoID, string pEntorno, bool pEsPre = true)
        {
            string rawKey = "IntegracionContinua_EntornoPre_" + pProyectoID + "_" + pEntorno;

            AgregarObjetoCache(rawKey, pEsPre);
        }

        public void InvalidarICActivada(Guid pProyectoID)
        {
            string rawKey = "IntegracionContinua_";

            InvalidarCacheQueContengaCadena(rawKey);
        }

        /// <summary>
        /// Obtiene el nº de proyectos con el mismo dominio que el pasado por parámetro
        /// </summary>
        /// <param name="pDominio">Dominio a revisar</param>
        /// <returns></returns>
        public int NumeroProyectosConMismosDominio(string pDominio)
        {
            string rawKey = string.Concat("NumeroProyectosConMismosDominio", pDominio);

            int? numeroProyectosConMismosDominio = ObtenerObjetoDeCache(rawKey) as int?;

            if (!numeroProyectosConMismosDominio.HasValue)
            {
                // Se traen de BBDD y se almacenan en cache por un día.
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                numeroProyectosConMismosDominio = proyCN.NumeroProyectosConMismosDominio(pDominio);
                proyCN.Dispose();

                AgregarObjetoCache(rawKey, numeroProyectosConMismosDominio.Value, BaseCL.DURACION_CACHE_UN_DIA);
            }

            return numeroProyectosConMismosDominio.Value;
        }

        public void InvalidarCacheContadorRecursosComunidad(Guid pProyectoID)
        {
            string rawKey = string.Concat(CLAVE_CACHE_CONTADOR_RECURSOS_COMUNIDAD, pProyectoID);
            InvalidarCache(rawKey, true);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Clase de negocio
        /// </summary>
        protected ProyectoCN ProyectoCN
        {
            get
            {
                if (mProyectoCN == null)
                {
                    if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
                    {
                        mProyectoCN = new ProyectoCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                    else
                    {
                        mProyectoCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                }

                return mProyectoCN;
            }
        }

        /// <summary>
        /// Lógica de parámetro
        /// </summary>
        protected ParametroCN ParametroCN
        {
            get
            {
                if (mParametroCN == null)
                {
                    if (!string.IsNullOrEmpty(mFicheroConfiguracionBD))
                    {
                        mParametroCN = new ParametroCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                    else
                    {
                        mParametroCN = new ParametroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    }
                }

                return mParametroCN;
            }
        }

        /// <summary>
        /// Clave para la cache
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
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ProyectoCL()
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
