using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Peticion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Peticion
{
    #region Enumeraciones

    /// <summary>
    /// Tipos posibles de peticiones
    /// </summary>
    public enum TipoPeticion
    {
        /// <summary>
        /// Petición para acceder a una organización
        /// </summary>
        AccesoAOrganizacion = 0,
        /// <summary>
        /// Petición para acceder a un proyecto
        /// </summary>
        AccesoAProyecto = 1,
        /// <summary>
        /// Petición para cambio de nombre corto de usuario
        /// </summary>
        CambioNombreCortoUsu = 2,
        /// <summary>
        /// Petición para cambio de contraseña
        /// </summary>
        CambioPassword = 3,
        /// <summary>
        /// Petición para crear un nuevo proyecto
        /// </summary>
        NuevoProyecto = 4,
        /// <summary>
        /// Peticion para acceder a un dafo y a la comunidad del dafo (con esta peticion se generan filas en la tabla peticion, PeticionInvitacionComunidad y PeticionInvitacionDafo
        /// </summary>
        AccesoADafoYComunidad = 5,
        /// <summary>
        /// Peticion para nuevo usuario de gnoss y hacerse contacto
        /// </summary>
        AccesoGnossYContacto = 6,
        /// <summary>
        /// Peticion para acceder a grupos y a la comunidad de los grupos (con esta peticion se generan filas en la tabla peticion, PeticionInvitacionComunidad y PeticionInvitacionGrupo
        /// </summary>
        AccesoAGruposYComunidad = 7,
        /// <summary>
        /// Peticion para autenticarse con doble factor
        /// </summary>
        AutenticacionDobleFactor = 8
    }

    /// <summary>
    /// Estados para las peticiones
    /// </summary>
    public enum EstadoPeticion
    {
        /// <summary>
        /// La petición aún no ha sido procesada
        /// </summary>
        Pendiente,

        /// <summary>
        /// La petición se ha aceptado
        /// </summary>
        Aceptada,

        /// <summary>
        /// La petición se ha rechazado
        /// </summary>
        Rechazada
    }

    #endregion

    public class JoinPeticionPeticionInvitacionComunidad
    {
        public EntityModel.Models.Peticion.Peticion Peticion { get; set; }
        public PeticionInvitacionComunidad PeticionInvitacionComunidad { get; set; }
    }

    public class JoinPeticionPeticionOrgInvitaPers
    {
        public EntityModel.Models.Peticion.Peticion Peticion { get; set; }
        public PeticionOrgInvitaPers PeticionOrgInvitaPers { get; set; }
    }

    public class JoinPeticionPeticionInvitacionGrupo
    {
        public EntityModel.Models.Peticion.Peticion Peticion { get; set; }
        public PeticionInvitacionGrupo PeticionInvitacionGrupo { get; set; }
    }

    public class JoinPeticionPeticionNuevoProyecto
    {
        public EntityModel.Models.Peticion.Peticion Peticion { get; set; }
        public PeticionNuevoProyecto PeticionNuevoProyecto { get; set; }
    }

    public class JoinPeticionPeticionInvitaContacto
    {
        public EntityModel.Models.Peticion.Peticion Peticion { get; set; }
        public PeticionInvitaContacto PeticionInvitaContacto { get; set; }
    }

    //INNER JOIN Peticion ON PeticionInvitacionGrupo.PeticionID = Peticion.PeticionID 

    public static class Joins
    {
        public static IQueryable<JoinPeticionPeticionInvitaContacto> JoinPeticionInvitaContacto(this IQueryable<EntityModel.Models.Peticion.Peticion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PeticionInvitaContacto, peticion => peticion.PeticionID, peticionInvitaContacto => peticionInvitaContacto.PeticionID, (peticion, peticionInvitaContacto) => new JoinPeticionPeticionInvitaContacto
            {
                PeticionInvitaContacto = peticionInvitaContacto,
                Peticion = peticion
            });
        }

        public static IQueryable<JoinPeticionPeticionNuevoProyecto> JoinPeticionNuevoProyecto(this IQueryable<EntityModel.Models.Peticion.Peticion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PeticionNuevoProyecto, peticion => peticion.PeticionID, peticionNuevoProyecto => peticionNuevoProyecto.PeticionID, (peticion, peticionNuevoProyecto) => new JoinPeticionPeticionNuevoProyecto
            {
                PeticionNuevoProyecto = peticionNuevoProyecto,
                Peticion = peticion
            });
        }

        public static IQueryable<JoinPeticionPeticionInvitacionGrupo> JoinPeticionInvitacionGrupo(this IQueryable<EntityModel.Models.Peticion.Peticion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PeticionInvitacionGrupo, peticion => peticion.PeticionID, peticionInvitacionGrupo => peticionInvitacionGrupo.PeticionID, (peticion, peticionInvitacionGrupo) => new JoinPeticionPeticionInvitacionGrupo
            {
                PeticionInvitacionGrupo = peticionInvitacionGrupo,
                Peticion = peticion
            });
        }

        public static IQueryable<JoinPeticionPeticionOrgInvitaPers> JoinPeticionOrgInvitaPers(this IQueryable<EntityModel.Models.Peticion.Peticion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PeticionOrgInvitaPers, peticion => peticion.PeticionID, peticionOrgInvitaPers => peticionOrgInvitaPers.PeticionID, (peticion, peticionOrgInvitaPers) => new JoinPeticionPeticionOrgInvitaPers
            {
                PeticionOrgInvitaPers = peticionOrgInvitaPers,
                Peticion = peticion
            });
        }

        public static IQueryable<JoinPeticionPeticionInvitacionComunidad> JoinPeticionInvitacionComunidad(this IQueryable<EntityModel.Models.Peticion.Peticion> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.PeticionInvitacionComunidad, peticion => peticion.PeticionID, peticionInvitacionComunidad => peticionInvitacionComunidad.PeticionID, (peticion, peticionInvitacionComunidad) => new JoinPeticionPeticionInvitacionComunidad
            {
                PeticionInvitacionComunidad = peticionInvitacionComunidad,
                Peticion = peticion
            });
        }
    }

    /// <summary>
    /// DataAdapter de Peticiones
    /// </summary>
    public class PeticionAD : BaseAD
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public PeticionAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public PeticionAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;

            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        //Parte Select
        private string SelectPeticion;
        private string SelectPeticionInvitacionComunidad;
        private string SelectPeticionInvitacionGrupo;
        private string SelectPeticionOrgInvitaPers;
        private string SelectPeticionNuevoProyecto;
        private string SelectPeticionInvitaContacto;

        //Parte Delete peticiones de usuario
        private string DeletePeticionDeUsuario;
        private string DeletePeticionInvitacionComunidadDeUsuario;
        private string DeletePeticionOrgInvitaPersDeUsuario;
        private string DeletePeticionInvitaContactoDeUsuario;

        //Consultas
        private string sqlSelectPeticion;
        private string sqlSelectPeticionInvitacionComunidad;
        private string sqlSelectPeticionOrgInvitaPers;
        private string sqlSelectPeticionNuevoProyecto;
        private string sqlSelectPeticionInvitacionDafo;
        private string sqlSelectPeticionInvitacionGrupo;
        private string sqlSelectPeticionInvitaContacto;
        private string sqlSelectPeticionInvitacionOrganizacionPorID;
        private string sqlSelectPeticionInvitacionComunidadPorID;
        private string sqlSelectPeticionPorID;
        private string sqlSelectPeticionComunidadPorID;
        private string sqlSelectEstaPeticionEnEspera;
        private string sqlSelectPeticionDeOrganizacionAceptadasUsuario;
        private string sqlSelectPeticionOrgInvitaPersDeOrganizacionAceptadasUsuario;
        private string sqlSelectPeticionDeComunidadAceptadasUsuario;
        private string sqlSelectPeticionInvitacionComunidadDeComunidadAceptadasUsuario;
        private string sqlSelectPeticionPorUsuarioID;
        private string sqlSelectPeticionInvitacionComunidadPorUsuarioID;
        private string sqlSelectPeticionInvitacionGrupoPorUsuarioID;
        private string sqlSelectPeticionOrgInvitaPersPorUsuarioID;
        private string sqlSelectPeticionNuevoProyectoPorUsuarioID;
        private string sqlSelectPeticionInvitaContactoPorUsuarioID;
        private string sqlSelectExistePeticionProyectoMismoNombre;
        private string sqlSelectExistePeticionProyectoMismoNombreCorto;

        #endregion

        #region DataAdapter

        #region Peticion

        private string sqlPeticionInsert;
        private string sqlPeticionDelete;
        private string sqlPeticionModify;

        #endregion

        #region PeticionInvitacionComunidad

        private string sqlPeticionInvitacionComunidadInsert;
        private string sqlPeticionInvitacionComunidadDelete;
        private string sqlPeticionInvitacionComunidadModify;

        #endregion

        #region PeticionInvitacionGrupo

        private string sqlPeticionInvitacionGrupoInsert;
        private string sqlPeticionInvitacionGrupoDelete;
        private string sqlPeticionInvitacionGrupoModify;

        #endregion

        #region PeticionOrgInvitaPers

        private string sqlPeticionOrgInvitaPersInsert;
        private string sqlPeticionOrgInvitaPersDelete;
        private string sqlPeticionOrgInvitaPersModify;

        #endregion

        #region PeticionNuevoProyecto

        private string sqlPeticionNuevoProyectoInsert;
        private string sqlPeticionNuevoProyectoDelete;
        private string sqlPeticionNuevoProyectoModify;

        #endregion

        #region PeticionInvitacionDafo

        private string sqlPeticionInvitacionDafoInsert;
        private string sqlPeticionInvitacionDafoDelete;
        private string sqlPeticionInvitacionDafoModify;

        #endregion

        #region PeticionInvitaContacto

        private string sqlPeticionInvitaContactoInsert;
        private string sqlPeticionInvitaContactoDelete;
        private string sqlPeticionInvitaContactoModify;

        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Modifica la base de datos con los datos del dataset pasado por parámetro
        /// </summary>
        /// <param name="pDataSet">Dataset de peticiones</param>
        public void ActualizarBD()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Comprueba si la petición pasada por parámetro existe y está en estado de espera
        /// </summary>
        /// <param name="pPeticionID">Identificador de la petición</param>
        /// <returns>TRUE si existe y está en estado de espera, FALSE en caso contrario</returns>
        public bool EstaPeticionEnEspera(Guid pPeticionID)
        {
            short estado = mEntityContext.Peticion.Where(item => item.PeticionID.Equals(pPeticionID)).Select(item => item.Estado).FirstOrDefault();

            return estado.Equals((short)EstadoPeticion.Pendiente);
        }

        /// <summary>
        /// Elimina todas las peticiones de un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador de la petición</param>
        public void EliminarPeticionesDeUsuario(Guid pUsuarioID)
        {
            try
            {
                Guid peticionID = mEntityContext.Peticion.Where(item => item.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PeticionID).FirstOrDefault();

                //PeticionInvitacionComunidad
                PeticionInvitacionComunidad peticionInvitacionComunidad = mEntityContext.PeticionInvitacionComunidad.Where(item => item.PeticionID.Equals(peticionID)).FirstOrDefault();
                mEntityContext.EliminarElemento(peticionInvitacionComunidad);

                //PeticionOrgInvitaPers
                PeticionOrgInvitaPers peticionOrgInvitaPers = mEntityContext.PeticionOrgInvitaPers.Where(item => item.PeticionID.Equals(peticionID)).FirstOrDefault();
                mEntityContext.EliminarElemento(peticionOrgInvitaPers);

                //PeticionInvitaContacto
                PeticionInvitaContacto peticionInvitaContacto = mEntityContext.PeticionInvitaContacto.Where(item => item.PeticionID.Equals(peticionID)).FirstOrDefault();
                mEntityContext.EliminarElemento(peticionInvitaContacto);

                //Peticion
                EntityModel.Models.Peticion.Peticion peticion = mEntityContext.Peticion.Where(item => item.PeticionID.Equals(peticionID)).FirstOrDefault();
                mEntityContext.EliminarElemento(peticion);

                ActualizarBaseDeDatosEntityContext();
            }
            catch (Exception e)
            {
                mLoggingService.GuardarLogError(e);
            }
        }

        /// <summary>
        /// Obtiene las peticiones de invitaciónes a comunidad aceptadas por un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionesComunidadesAceptadasUsuario(Guid pUsuarioID)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.JoinPeticionInvitacionComunidad().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID) && item.Peticion.Estado.Equals((short)EstadoPeticion.Aceptada)).Select(item => item.Peticion).ToList();

            //PeticionInvitacionComunidad
            peticionDW.ListaPeticionInvitacionComunidad = mEntityContext.Peticion.JoinPeticionInvitacionComunidad().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID) && item.Peticion.Estado.Equals((short)EstadoPeticion.Aceptada)).Select(item => item.PeticionInvitacionComunidad).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Obtiene las peticiones de invitaciónes a organización aceptadas por un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionesOrganizacionesAceptadasUsuario(Guid pUsuarioID)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.JoinPeticionOrgInvitaPers().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID) && item.Peticion.Estado.Equals((short)EstadoPeticion.Aceptada)).Select(item => item.Peticion).ToList();

            //PeticionOrgInvitaPers
            peticionDW.ListaPeticionOrgInvitaPers = mEntityContext.Peticion.JoinPeticionOrgInvitaPers().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID) && item.Peticion.Estado.Equals((short)EstadoPeticion.Aceptada)).Select(item => item.PeticionOrgInvitaPers).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Obtiene los datos relativos a una petición de invitación a participar en una comunidad
        /// </summary>
        /// <param name="pPeticionID">Identificador de la petición</param>
        /// <param name="pOrganizacionID">Identificador de la organizacion del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto al que pertenece la invitación</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionComunidadPorID(Guid pPeticionID, Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.JoinPeticionInvitacionComunidad().Where(item => item.Peticion.PeticionID.Equals(pPeticionID) && item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente) && item.PeticionInvitacionComunidad.OrganizacionID.Equals(pOrganizacionID) && item.PeticionInvitacionComunidad.ProyectoID.Equals(pProyectoID)).Select(item => item.Peticion).ToList();

            //PeticionInvitacionComunidad
            peticionDW.ListaPeticionInvitacionComunidad = mEntityContext.Peticion.JoinPeticionInvitacionComunidad().Where(item => item.PeticionInvitacionComunidad.PeticionID.Equals(pPeticionID) && item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente) && item.PeticionInvitacionComunidad.OrganizacionID.Equals(pOrganizacionID) && item.PeticionInvitacionComunidad.ProyectoID.Equals(pProyectoID)).Select(item => item.PeticionInvitacionComunidad).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Obtiene los datos relativos a una petición de invitación a participar en una organización
        /// </summary>
        /// <param name="pPeticionID">Identificador de la petición</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionOrganizacionPorID(Guid pPeticionID)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            //PeticionOrgInvitaPers
            peticionDW.ListaPeticionOrgInvitaPers = mEntityContext.Peticion.JoinPeticionOrgInvitaPers().Where(item => item.Peticion.PeticionID.Equals(pPeticionID)).Select(item => item.PeticionOrgInvitaPers).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Obtiene el proyecto de una invitación a formar parte del mismo (SÓLO si la petición es de invitacion a comunidad, si no NULL)
        /// </summary>
        /// <param name="pPeticionID">ID de la peticion</param>
        /// <returns>Identificador del proyecto de la petición (NULL si la petición )</returns>
        public Guid? ObtenerProyectoIDDePeticionPorID(Guid pPeticionID)
        {
            return mEntityContext.PeticionInvitacionComunidad.Where(item => item.PeticionID.Equals(pPeticionID)).Select(item => item.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene todos los datos de una peticion por su ID
        /// </summary>
        /// <param name="pPeticionID">ID de la peticion</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionPorID(Guid pPeticionID)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            //PeticionInvitacionComunidad
            peticionDW.ListaPeticionInvitacionComunidad = mEntityContext.PeticionInvitacionComunidad.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            //PeticionInvitacionGrupo
            peticionDW.ListaPeticionInvitacionGrupo = mEntityContext.PeticionInvitacionGrupo.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            //PeticionNuevoProyecto
            peticionDW.ListaPeticionNuevoProyecto = mEntityContext.PeticionNuevoProyecto.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            //PeticionOrgInvitaPers
            peticionDW.ListaPeticionOrgInvitaPers = mEntityContext.PeticionOrgInvitaPers.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            //PeticionInvitaContacto
            peticionDW.ListaPeticionInvitacionContacto = mEntityContext.PeticionInvitaContacto.Where(item => item.PeticionID.Equals(pPeticionID)).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Obtiene si un proyecto tiene alguna invitación realizada de Ning
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>true si tiene peticiones</returns>
        public bool ObtenerSiProyectoTieneInvitacionesDeNing(Guid pProyectoID)
        {
            return mEntityContext.PeticionInvitacionComunidad.Where(item => item.ProyectoID.Equals(pProyectoID) && item.NingID != null).Any();
        }

        /// <summary>
        /// Obtiene las peticiones del usuario cuyo identificador se pasa por parámetro
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset con las peticiones del usuario</returns>
        public DataWrapperPeticion ObtenerPeticionPorUsuarioID(Guid pUsuarioID)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.Where(item => item.UsuarioID.Value.Equals(pUsuarioID)).ToList();

            //PeticionInvitacionComunidad
            peticionDW.ListaPeticionInvitacionComunidad = mEntityContext.Peticion.JoinPeticionInvitacionComunidad().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PeticionInvitacionComunidad).ToList();

            //PeticionInvitacionGrupo
            peticionDW.ListaPeticionInvitacionGrupo = mEntityContext.Peticion.JoinPeticionInvitacionGrupo().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PeticionInvitacionGrupo).ToList();

            //PeticionOrgInvitaPers
            peticionDW.ListaPeticionOrgInvitaPers = mEntityContext.Peticion.JoinPeticionOrgInvitaPers().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PeticionOrgInvitaPers).ToList();

            //PeticionNuevoProyecto
            peticionDW.ListaPeticionNuevoProyecto = mEntityContext.Peticion.JoinPeticionNuevoProyecto().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PeticionNuevoProyecto).ToList();

            //PeticionInvitaContacto
            peticionDW.ListaPeticionInvitacionContacto = mEntityContext.Peticion.JoinPeticionInvitaContacto().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuarioID)).Select(item => item.PeticionInvitaContacto).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Obtiene los datos relativos de la tabla petición de un usuario y un tipo de peticion.
        /// </summary>
        /// <param name="pUsuario">Identificador del usuario</param>
        /// <param name="pTipo">Tipo de la peticion que queremos obtener</param>
        /// <param name="pObtenerSoloSinProcesar">TRUE si solo se quieren obtener las peticiones que están sin procesar, FALSE en caso contrario</param>
        /// <returns>DataSet con los datos de la petición del usuario</returns>
        public DataWrapperPeticion ObtenerPeticionPorUsuarioIDyTipo(Guid pUsuario, TipoPeticion pTipo, bool pObtenerSoloSinProcesar)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            var queryPeticion = mEntityContext.Peticion.Where(item => item.UsuarioID.Value.Equals(pUsuario) && item.Tipo.Equals((short)pTipo));
            var queryPeticionInvitacionComunidad = mEntityContext.Peticion.JoinPeticionInvitacionComunidad().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuario) && item.Peticion.Tipo.Equals((short)pTipo));
            var queryPeticionOrgInvitaPers = mEntityContext.Peticion.JoinPeticionOrgInvitaPers().Where(item => item.Peticion.UsuarioID.Value.Equals(pUsuario) && item.Peticion.Tipo.Equals((short)pTipo));

            if (pObtenerSoloSinProcesar)
            {
                queryPeticion = queryPeticion.Where(item => item.Estado.Equals((short)EstadoPeticion.Pendiente));
                queryPeticionInvitacionComunidad = queryPeticionInvitacionComunidad.Where(item => item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente));
                queryPeticionOrgInvitaPers = queryPeticionOrgInvitaPers.Where(item => item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente));
            }

            //Peticion
            peticionDW.ListaPeticion = queryPeticion.ToList();

            //PeticionInvitacionComunidad
            peticionDW.ListaPeticionInvitacionComunidad = queryPeticionInvitacionComunidad.Select(item => item.PeticionInvitacionComunidad).ToList();

            //PeticionOrgInvitaPers
            peticionDW.ListaPeticionOrgInvitaPers = queryPeticionOrgInvitaPers.Select(item => item.PeticionOrgInvitaPers).ToList();

            return peticionDW;
        }

        /// <summary>
        ///  Obtiene las peticiones de las nuevas comunidades sin aceptar
        /// </summary>
        /// <returns>DataSet con las peticiones de las comunidades</returns>
        public DataWrapperPeticion ObtenerPeticionComunidadesPendientesDeAceptar()
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.Where(item => item.Tipo.Equals((short)TipoPeticion.NuevoProyecto) && item.Estado.Equals((short)EstadoPeticion.Pendiente)).OrderBy(item => item.FechaPeticion).ToList();

            //PeticionNuevoProyecto
            peticionDW.ListaPeticionNuevoProyecto = mEntityContext.Peticion.JoinPeticionNuevoProyecto().Where(item => item.Peticion.Tipo.Equals((short)TipoPeticion.NuevoProyecto) && item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente)).OrderBy(item => item.Peticion.FechaPeticion).Select(item => item.PeticionNuevoProyecto).ToList();

            return peticionDW;
        }

        /// <summary>
        ///  Obtiene el número de las peticiones de las nuevas comunidades sin aceptar
        /// </summary>
        /// <returns>DataSet con las peticiones de las comunidades</returns>
        public int ObtenerPeticionComunidadesPendientesDeAceptarCount()
        {

            //PeticionNuevoProyecto
            int count = mEntityContext.Peticion.JoinPeticionNuevoProyecto().Where(item => item.Peticion.Tipo.Equals((short)TipoPeticion.NuevoProyecto) && item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente)).OrderBy(item => item.Peticion.FechaPeticion).Select(item => item.PeticionNuevoProyecto).Count();

            return count;
        }

        /// <summary>
        ///  Obtiene las peticiones de las nuevas comunidades sin aceptar
        /// </summary>
        /// <returns>DataSet con las peticiones de las comunidades</returns>
        public DataWrapperPeticion ObtenerPeticionComunidadesPendientesDeAceptarPaginacion(int paginaActual, int numResultados)
        {
            DataWrapperPeticion peticionDW = new DataWrapperPeticion();

            //Peticion
            peticionDW.ListaPeticion = mEntityContext.Peticion.Where(item => item.Tipo.Equals((short)TipoPeticion.NuevoProyecto) && item.Estado.Equals((short)EstadoPeticion.Pendiente)).OrderBy(item => item.FechaPeticion).Skip(paginaActual * numResultados).Take(numResultados).ToList();

            //PeticionNuevoProyecto
            peticionDW.ListaPeticionNuevoProyecto = mEntityContext.Peticion.JoinPeticionNuevoProyecto().Where(item => item.Peticion.Tipo.Equals((short)TipoPeticion.NuevoProyecto) && item.Peticion.Estado.Equals((short)EstadoPeticion.Pendiente)).OrderBy(item => item.Peticion.FechaPeticion).Skip(paginaActual * numResultados).Take(numResultados).Select(item => item.PeticionNuevoProyecto).ToList();

            return peticionDW;
        }

        /// <summary>
        /// Comprueba si existe alguna petición de nueva comunidad con el mismo nombre que se pasa como parámetro
        /// </summary>
        /// <param name="pNombreProyecto">Nombre de comunidad</param>
        /// <returns>TRUE si hay alguna petición de comunidad con ese mismo nombre, FALSE en caso contrario</returns>
        public bool ExistePeticionProyectoMismoNombre(string pNombreProyecto)
        {
            return mEntityContext.PeticionNuevoProyecto.Where(item => item.Nombre.ToUpper().Equals(pNombreProyecto.ToUpper())).Select(item => item.PeticionID).Any();
        }

        /// <summary>
        /// Comprueba si existe alguna petición de nueva comunidad con el mismo nombre corto que se pasa como parámetro
        /// </summary>
        /// <param name="pNombreCortoProyecto">Nombre corto de comunidad</param>
        /// <returns>TRUE si hay alguna petición de comunidad con ese mismo nombre corto, FALSE en caso contrario</returns>
        public bool ExistePeticionProyectoMismoNombreCorto(string pNombreCortoProyecto)
        {
            return mEntityContext.PeticionNuevoProyecto.Where(item => item.NombreCorto.Equals(pNombreCortoProyecto)).Select(item => item.PeticionID).Any();
        }

        #endregion

        #region Privados

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Parte Select

            this.SelectPeticion = "SELECT " + IBD.CargarGuid("Peticion.PeticionID") + ", Peticion.FechaPeticion, Peticion.FechaProcesado, Peticion.Tipo, Peticion.Estado, " + IBD.CargarGuid("Peticion.UsuarioID");

            this.SelectPeticionInvitacionComunidad = "SELECT " + IBD.CargarGuid("PeticionInvitacionComunidad.PeticionID") + ", " + IBD.CargarGuid("PeticionInvitacionComunidad.OrganizacionID") + ", " + IBD.CargarGuid("PeticionInvitacionComunidad.ProyectoID , PeticionInvitacionComunidad.NingID");

            this.SelectPeticionInvitacionGrupo = "SELECT " + IBD.CargarGuid("PeticionInvitacionGrupo.PeticionID") + ", " + IBD.CargarGuid("PeticionInvitacionGrupo.OrganizacionID") + ", " + IBD.CargarGuid("PeticionInvitacionGrupo.ProyectoID , PeticionInvitacionGrupo.GruposID");

            this.SelectPeticionOrgInvitaPers = "SELECT " + IBD.CargarGuid("PeticionOrgInvitaPers.PeticionID") + ", " + IBD.CargarGuid("PeticionOrgInvitaPers.OrganizacionID") + ", PeticionOrgInvitaPers.Cargo, PeticionOrgInvitaPers.Email";

            this.SelectPeticionNuevoProyecto = "SELECT " + IBD.CargarGuid("PeticionNuevoProyecto.PeticionID") + ", PeticionNuevoProyecto.Nombre, PeticionNuevoProyecto.NombreCorto,PeticionNuevoProyecto.Descripcion,PeticionNuevoProyecto.Tipo," + IBD.CargarGuid("PeticionNuevoProyecto.ComunidadPrivadaPadreID") + ", " + IBD.CargarGuid("PeticionNuevoProyecto.PerfilCreadorID");

            this.SelectPeticionInvitaContacto = "SELECT " + IBD.CargarGuid("PeticionInvitaContacto.PeticionID") + ", " + IBD.CargarGuid("PeticionInvitaContacto.IdentidadID");

            this.sqlSelectExistePeticionProyectoMismoNombre = "SELECT " + IBD.CargarGuid("PeticionNuevoProyecto.PeticionID") + " FROM PeticionNuevoProyecto WHERE (UPPER(PeticionNuevoProyecto.Nombre) = UPPER(" + IBD.ToParam("nombre") + "))";

            this.sqlSelectExistePeticionProyectoMismoNombreCorto = "SELECT " + IBD.CargarGuid("PeticionNuevoProyecto.PeticionID") + " FROM PeticionNuevoProyecto WHERE (UPPER(PeticionNuevoProyecto.NombreCorto) = UPPER(" + IBD.ToParam("nombreCorto") + "))";

            #endregion

            #region Consultas

            //Parte Delete peticiones de usuario
            DeletePeticionDeUsuario = "DELETE FROM Peticion WHERE UsuarioID = " + IBD.GuidParamValor("usuarioID");

            DeletePeticionInvitacionComunidadDeUsuario = "DELETE FROM PeticionInvitacionComunidad WHERE PeticionID IN (SELECT " + IBD.CargarGuid("Peticion.PeticionID") + " FROM Peticion WHERE UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            DeletePeticionOrgInvitaPersDeUsuario = "DELETE FROM PeticionOrgInvitaPers WHERE PeticionID IN (SELECT " + IBD.CargarGuid("Peticion.PeticionID") + " FROM Peticion WHERE UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            DeletePeticionInvitaContactoDeUsuario = "DELETE FROM PeticionInvitaContacto WHERE PeticionID IN (SELECT " + IBD.CargarGuid("Peticion.PeticionID") + " FROM Peticion WHERE UsuarioID = " + IBD.GuidParamValor("usuarioID") + ")";

            this.sqlSelectPeticion = SelectPeticion + " FROM Peticion";

            this.sqlSelectPeticionInvitacionComunidad = SelectPeticionInvitacionComunidad + " FROM PeticionInvitacionComunidad";

            this.sqlSelectPeticionOrgInvitaPers = SelectPeticionOrgInvitaPers + " FROM PeticionOrgInvitaPers";

            this.sqlSelectPeticionNuevoProyecto = SelectPeticionNuevoProyecto + " FROM PeticionNuevoProyecto";

            this.sqlSelectPeticionInvitaContacto = SelectPeticionInvitaContacto + " FROM PeticionInvitaContacto";

            this.sqlSelectPeticionPorID = sqlSelectPeticion + " WHERE Peticion.PeticionID = " + IBD.GuidParamValor("peticionID");

            this.sqlSelectPeticionComunidadPorID = sqlSelectPeticion + " INNER JOIN PeticionInvitacionComunidad ON PeticionInvitacionComunidad.PeticionID = Peticion.PeticionID WHERE Peticion.PeticionID = " + IBD.GuidParamValor("peticionID") + " AND Peticion.Estado = " + ((short)EstadoPeticion.Pendiente).ToString() + " AND PeticionInvitacionComunidad.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND PeticionInvitacionComunidad.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectPeticionInvitacionOrganizacionPorID = sqlSelectPeticionOrgInvitaPers + " INNER JOIN Peticion ON PeticionOrgInvitaPers.PeticionID = Peticion.PeticionID WHERE PeticionOrgInvitaPers.PeticionID = " + IBD.GuidParamValor("peticionID");

            this.sqlSelectPeticionInvitacionComunidadPorID = sqlSelectPeticionInvitacionComunidad + " INNER JOIN Peticion ON PeticionInvitacionComunidad.PeticionID = Peticion.PeticionID WHERE PeticionInvitacionComunidad.PeticionID = " + IBD.GuidParamValor("peticionID") + " AND Peticion.Estado = " + ((short)EstadoPeticion.Pendiente).ToString() + " AND PeticionInvitacionComunidad.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND PeticionInvitacionComunidad.ProyectoID = " + IBD.GuidParamValor("proyectoID");

            this.sqlSelectEstaPeticionEnEspera = "SELECT Peticion.Estado FROM Peticion WHERE Peticion.PeticionID = " + IBD.GuidParamValor("peticionID");

            this.sqlSelectPeticionDeOrganizacionAceptadasUsuario = sqlSelectPeticion + " INNER JOIN PeticionOrgInvitaPers ON Peticion.PeticionID = PeticionOrgInvitaPers.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Peticion.Estado = " + ((short)EstadoPeticion.Aceptada).ToString();

            this.sqlSelectPeticionOrgInvitaPersDeOrganizacionAceptadasUsuario = sqlSelectPeticionOrgInvitaPers + " INNER JOIN Peticion ON PeticionOrgInvitaPers.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Peticion.Estado = " + ((short)EstadoPeticion.Aceptada).ToString();

            this.sqlSelectPeticionDeComunidadAceptadasUsuario = sqlSelectPeticion + " INNER JOIN PeticionInvitacionComunidad ON Peticion.PeticionID = PeticionInvitacionComunidad.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Peticion.Estado = " + ((short)EstadoPeticion.Aceptada).ToString();

            this.sqlSelectPeticionInvitacionComunidadDeComunidadAceptadasUsuario = sqlSelectPeticionInvitacionComunidad + " INNER JOIN Peticion ON PeticionInvitacionComunidad.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Peticion.Estado = " + ((short)EstadoPeticion.Aceptada).ToString();

            this.sqlSelectPeticionPorUsuarioID = sqlSelectPeticion + " WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            this.sqlSelectPeticionInvitacionComunidadPorUsuarioID = sqlSelectPeticionInvitacionComunidad + " INNER JOIN Peticion ON PeticionInvitacionComunidad.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            this.sqlSelectPeticionOrgInvitaPersPorUsuarioID = sqlSelectPeticionOrgInvitaPers + " INNER JOIN Peticion ON PeticionOrgInvitaPers.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            this.sqlSelectPeticionNuevoProyectoPorUsuarioID = sqlSelectPeticionNuevoProyecto + " INNER JOIN Peticion ON PeticionNuevoProyecto.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            this.sqlSelectPeticionInvitaContactoPorUsuarioID = sqlSelectPeticionInvitaContacto + " INNER JOIN Peticion ON PeticionInvitaContacto.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            this.sqlSelectPeticionInvitacionDafo = "SELECT " + IBD.CargarGuid("PeticionInvitacionDafo.PeticionID") + ", " + IBD.CargarGuid("PeticionInvitacionDafo.DafoID") + ", PeticionInvitacionDafo.Email, " + IBD.CargarGuid("PeticionInvitacionDafo.IdentidadRemitenteID") + ", " + IBD.CargarGuid("PeticionInvitacionDafo.IdentidadInvitado") + " FROM PeticionInvitacionDafo";

            this.sqlSelectPeticionInvitacionGrupo = SelectPeticionInvitacionGrupo + " FROM PeticionInvitacionGrupo";

            this.sqlSelectPeticionInvitacionGrupoPorUsuarioID = sqlSelectPeticionInvitacionGrupo + " INNER JOIN Peticion ON PeticionInvitacionGrupo.PeticionID = Peticion.PeticionID WHERE Peticion.UsuarioID = " + IBD.GuidParamValor("UsuarioID");

            #endregion

            #region DataAdapter

            #region Peticion

            this.sqlPeticionInsert = IBD.ReplaceParam("INSERT INTO Peticion (PeticionID, FechaPeticion, FechaProcesado, Tipo, Estado, UsuarioID) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", @FechaPeticion, @FechaProcesado, @Tipo, @Estado, " + IBD.GuidParamColumnaTabla("UsuarioID") + ")");

            this.sqlPeticionDelete = IBD.ReplaceParam("DELETE FROM Peticion WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (FechaPeticion = @O_FechaPeticion) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + " OR " + IBD.GuidParamColumnaTabla("O_UsuarioID") + " IS NULL AND UsuarioID IS NULL)");

            this.sqlPeticionModify = IBD.ReplaceParam("UPDATE Peticion SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", FechaPeticion = @FechaPeticion, FechaProcesado = @FechaProcesado, Tipo = @Tipo, Estado = @Estado, UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + " WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (FechaPeticion = @O_FechaPeticion) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL) AND (Tipo = @O_Tipo) AND (Estado = @O_Estado) AND (UsuarioID = " + IBD.GuidParamColumnaTabla("O_UsuarioID") + " OR " + IBD.GuidParamColumnaTabla("O_UsuarioID") + " IS NULL AND UsuarioID IS NULL)");

            #endregion

            #region PeticionInvitacionComunidad

            this.sqlPeticionInvitacionComunidadInsert = IBD.ReplaceParam("INSERT INTO PeticionInvitacionComunidad (PeticionID, OrganizacionID, ProyectoID,NingID) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @NingID)");

            this.sqlPeticionInvitacionComunidadDelete = IBD.ReplaceParam("DELETE FROM PeticionInvitacionComunidad WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            this.sqlPeticionInvitacionComunidadModify = IBD.ReplaceParam("UPDATE PeticionInvitacionComunidad SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " , NingID = @NingID WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region PeticionInvitacionGrupo

            this.sqlPeticionInvitacionGrupoInsert = IBD.ReplaceParam("INSERT INTO PeticionInvitacionGrupo (PeticionID, OrganizacionID, ProyectoID,GruposID) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @GruposID)");

            this.sqlPeticionInvitacionGrupoDelete = IBD.ReplaceParam("DELETE FROM PeticionInvitacionGrupo WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ")");

            this.sqlPeticionInvitacionGrupoModify = IBD.ReplaceParam("UPDATE PeticionInvitacionGrupo SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " , GruposID = @GruposID WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ")");

            #endregion

            #region PeticionOrgInvitaPers

            this.sqlPeticionOrgInvitaPersInsert = IBD.ReplaceParam("INSERT INTO PeticionOrgInvitaPers (PeticionID, OrganizacionID, Cargo, Email) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", @Cargo, @Email)");

            this.sqlPeticionOrgInvitaPersDelete = IBD.ReplaceParam("DELETE FROM PeticionOrgInvitaPers WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Cargo = @O_Cargo OR @O_Cargo IS NULL AND Cargo IS NULL) AND (Email = @O_Email OR @O_Email IS NULL AND Email IS NULL)");

            this.sqlPeticionOrgInvitaPersModify = IBD.ReplaceParam("UPDATE PeticionOrgInvitaPers SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", Cargo = @Cargo, Email = @Email WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (Cargo = @O_Cargo OR @O_Cargo IS NULL AND Cargo IS NULL) AND (Email = @O_Email OR @O_Email IS NULL AND Email IS NULL)");

            #endregion

            #region PeticionNuevoProyecto

            this.sqlPeticionNuevoProyectoInsert = IBD.ReplaceParam("INSERT INTO PeticionNuevoProyecto (PeticionID, Nombre, NombreCorto, Descripcion, Tipo,ComunidadPrivadaPadreID,PerfilCreadorID) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", @Nombre, @NombreCorto, @Descripcion, @Tipo," + IBD.GuidParamColumnaTabla("ComunidadPrivadaPadreID") + ", " + IBD.GuidParamColumnaTabla("PerfilCreadorID") + ")");

            this.sqlPeticionNuevoProyectoDelete = IBD.ReplaceParam("DELETE FROM PeticionNuevoProyecto WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (Nombre=@O_Nombre) AND (NombreCorto = @O_NombreCorto) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Descripcion", true) + " AND (Tipo=@O_Tipo) AND (ComunidadPrivadaPadreID= " + IBD.GuidParamColumnaTabla("O_ComunidadPrivadaPadreID") + " OR @O_ComunidadPrivadaPadreID IS NULL AND ComunidadPrivadaPadreID IS NULL) AND (PerfilCreadorID=" + IBD.GuidParamColumnaTabla("O_PerfilCreadorID") + ")");

            this.sqlPeticionNuevoProyectoModify = IBD.ReplaceParam("UPDATE PeticionNuevoProyecto SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", Nombre=@Nombre, NomberCorto=@NombreCorto, Descripciono = @Descripcion, Tipo = @Tipo,ComunidadPrivadaPadreID=" + IBD.GuidParamColumnaTabla("ComunidadPrivadaPadreID") + ",PerfilCreadorID=" + IBD.GuidParamColumnaTabla("PerfilCreadorID") + " WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("O_PeticionID") + ") AND (Nombre=@O_Nombre) AND (NombreCorto = @O_NombreCorto) AND " + IBD.ComparacionCamposTextoPesadoConOriginal("Descripcion", true) + " AND (Tipo=@O_Tipo) AND (ComunidadPrivadaPadreID= " + IBD.GuidParamColumnaTabla("O_ComunidadPrivadaPadreID") + " OR @O_ComunidadPrivadaPadreID IS NULL AND ComunidadPrivadaPadreID IS NULL) AND (PerfilCreadorID=" + IBD.GuidParamColumnaTabla("O_PerfilCreadorID") + ")");

            #endregion

            #region PeticionInvitacionDafo

            this.sqlPeticionInvitacionDafoInsert = IBD.ReplaceParam("INSERT INTO PeticionInvitacionDafo (PeticionID, DafoID, Email, IdentidadRemitenteID, IdentidadInvitado) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", " + IBD.GuidParamColumnaTabla("DafoID") + ", @Email, " + IBD.GuidParamColumnaTabla("IdentidadRemitenteID") + ", " + IBD.GuidParamColumnaTabla("IdentidadInvitado") + ")");

            this.sqlPeticionInvitacionDafoDelete = IBD.ReplaceParam("DELETE FROM PeticionInvitacionDafo WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("Original_PeticionID") + ") AND (DafoID = " + IBD.GuidParamColumnaTabla("Original_DafoID") + ") AND (Email = @Original_Email OR @Original_Email IS NULL AND Email IS NULL) AND (IdentidadRemitenteID = " + IBD.GuidParamColumnaTabla("Original_IdentidadRemitenteID") + " AND (IdentidadInvitado = " + IBD.GuidParamColumnaTabla("Original_IdentidadInvitado") + ")");

            this.sqlPeticionInvitacionDafoModify = IBD.ReplaceParam("UPDATE PeticionInvitacionDafo SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", DafoID = " + IBD.GuidParamColumnaTabla("DafoID") + ", Email = @Email, IdentidadRemitenteID = " + IBD.GuidParamColumnaTabla("IdentidadRemitenteID") + ", IdentidadInvitado = " + IBD.GuidParamColumnaTabla("IdentidadInvitado") + " WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("Original_PeticionID") + ") AND (DafoID = " + IBD.GuidParamColumnaTabla("Original_DafoID") + ") AND (Email = @Original_Email OR @Original_Email IS NULL AND Email IS NULL) AND (IdentidadRemitenteID = " + IBD.GuidParamColumnaTabla("Original_IdentidadRemitenteID") + "AND (IdentidadInvitado = " + IBD.GuidParamColumnaTabla("Original_IdentidadInvitado") + ")");

            #endregion

            #region PeticionInvitaContacto

            this.sqlPeticionInvitaContactoInsert = IBD.ReplaceParam("INSERT INTO PeticionInvitaContacto (PeticionID, IdentidadID) VALUES (" + IBD.GuidParamColumnaTabla("PeticionID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ")");

            this.sqlPeticionInvitaContactoDelete = IBD.ReplaceParam("DELETE FROM PeticionInvitaContacto WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("Original_PeticionID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("Original_IdentidadID") + ")");

            this.sqlPeticionInvitaContactoModify = IBD.ReplaceParam("UPDATE PeticionInvitaContacto SET PeticionID = " + IBD.GuidParamColumnaTabla("PeticionID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (PeticionID = " + IBD.GuidParamColumnaTabla("Original_PeticionID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("Original_IdentidadID") + ")");

            #endregion

            #endregion
        }

        #endregion

        #endregion
    }
}
