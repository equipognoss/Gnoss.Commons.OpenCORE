using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Notificacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.EntityModel.Models.UsuarioDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.Notificacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Usuarios
{
    #region Enumeraciones

    /// <summary>
    /// Contiene los posibles estados de una solicitud
    /// </summary>
    public enum TipoRegistro
    {
        /// <summary>
        /// Registro Normal
        /// </summary>
        Normal = 0,
        /// <summary>
        /// Registro en pagina de promo
        /// </summary>
        Promo = 1,
        /// <summary>
        /// Registro con invitacion
        /// </summary>
        InvitacionCom = 2,
        /// <summary>
        /// Registro con invitacion
        /// </summary>
        InvitacionOrg = 3,
        /// <summary>
        /// Registro con invitacion
        /// </summary>
        Invitacion = 4,
        /// <summary>
        /// Registro con invitacion
        /// </summary>
        CAS_CRFP = 5
    }

    /// <summary>
    /// Contiene los posibles estados de una solicitud
    /// </summary>
    public enum EstadoSolicitud
    {
        /// <summary>
        /// Solicitud en espera
        /// </summary>
        Espera = 0,
        /// <summary>
        /// Solicitud rechazada
        /// </summary>
        Rechazada = 1,
        /// <summary>
        /// Solicitud aceptada
        /// </summary>
        Aceptada = 2
    }

    /// <summary>
    /// Tipos de contenedores de solicitudes
    /// </summary>
    public enum TiposContenedorSolicitudes
    {
        /// <summary>
        /// Solicitudes de nuevos usuarios a GNOSS
        /// </summary>
        NuevosUsuarios = 0,
        /// <summary>
        /// Solicitudes de nuevas organizaciones a GNOSS
        /// </summary>
        NuevasOrganizaciones,
        /// <summary>
        /// Solicitudes de nuevas clases a GNOSS
        /// </summary>
        NuevasClases,
        /// <summary>
        /// Solicitudes de nuevos profesores a GNOSS
        /// </summary>
        NuevosProfesores,
        /// <summary>
        /// Solicitudes de usuarios ya registrados a un proyecto en concreto
        /// </summary>
        AccesoAProyectos
    }

    #endregion

    public class JoinSolicitudSolicitudUsuario
    {
        public Solicitud Solicitud { get; set; }
        public SolicitudUsuario SolicitudUsuario { get; set; }
    }

    public class JoinSolicitudSolicitudGrupo
    {
        public Solicitud Solicitud { get; set; }
        public SolicitudGrupo SolicitudGrupo { get; set; }
    }

    public class JoinSolicitudSolicitudOrganizacion
    {
        public Solicitud Solicitud { get; set; }
        public SolicitudOrganizacion SolicitudOrganizacion { get; set; }
    }

    public class JoinSolicitudSolicitudNuevoUsuario
    {
        public Solicitud Solicitud { get; set; }
        public SolicitudNuevoUsuario SolicitudNuevoUsuario { get; set; }
    }

    public class JoinSolicitudSolicitudNuevaOrganizacion
    {
        public Solicitud Solicitud { get; set; }
        public SolicitudNuevaOrganizacion SolicitudNuevaOrganizacion { get; set; }
    }

    public class JoinSolicitudSolicitudNuevaOrgEmp
    {
        public Solicitud Solicitud { get; set; }
        public SolicitudNuevaOrgEmp SolicitudNuevaOrgEmp { get; set; }
    }

    //INNER JOIN Solicitud ON SolicitudNuevaOrgEmp.SolicitudID = Solicitud.SolicitudID 

    public static class Joins
    {
        public static IQueryable<JoinSolicitudSolicitudNuevaOrgEmp> JoinSolicitudNuevaOrgEmp(this IQueryable<Solicitud> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SolicitudNuevaOrgEmp, solicitud => solicitud.SolicitudID, solicitudNuevaOrgEmp => solicitudNuevaOrgEmp.SolicitudID, (solicitud, solicitudNuevaOrgEmp) => new JoinSolicitudSolicitudNuevaOrgEmp
            {
                SolicitudNuevaOrgEmp = solicitudNuevaOrgEmp,
                Solicitud = solicitud
            });
        }

        public static IQueryable<JoinSolicitudSolicitudNuevaOrganizacion> JoinSolicitudNuevaOrganizacion(this IQueryable<Solicitud> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SolicitudNuevaOrganizacion, solicitud => solicitud.SolicitudID, solicitudNuevaOrganizacion => solicitudNuevaOrganizacion.SolicitudID, (solicitud, solicitudNuevaOrganizacion) => new JoinSolicitudSolicitudNuevaOrganizacion
            {
                SolicitudNuevaOrganizacion = solicitudNuevaOrganizacion,
                Solicitud = solicitud
            });
        }

        public static IQueryable<JoinSolicitudSolicitudNuevoUsuario> JoinSolicitudNuevoUsuario(this IQueryable<Solicitud> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SolicitudNuevoUsuario, solicitud => solicitud.SolicitudID, solicitudNuevoUsuario => solicitudNuevoUsuario.SolicitudID, (solicitud, solicitudNuevoUsuario) => new JoinSolicitudSolicitudNuevoUsuario
            {
                SolicitudNuevoUsuario = solicitudNuevoUsuario,
                Solicitud = solicitud
            });
        }

        public static IQueryable<JoinSolicitudSolicitudOrganizacion> JoinSolicitudOrganizacion(this IQueryable<Solicitud> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SolicitudOrganizacion, solicitud => solicitud.SolicitudID, solicitudOrganizacion => solicitudOrganizacion.SolicitudID, (solicitud, solicitudOrganizacion) => new JoinSolicitudSolicitudOrganizacion
            {
                Solicitud = solicitud,
                SolicitudOrganizacion = solicitudOrganizacion
            });
        }

        public static IQueryable<JoinSolicitudSolicitudGrupo> JoinSolicitudGrupo(this IQueryable<Solicitud> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SolicitudGrupo, solicitud => solicitud.SolicitudID, solicitudGrupo => solicitudGrupo.SolicitudID, (solicitud, solicitudGrupo) => new JoinSolicitudSolicitudGrupo
            {
                Solicitud = solicitud,
                SolicitudGrupo = solicitudGrupo
            });
        }

        public static IQueryable<JoinSolicitudSolicitudUsuario> JoinSolicitudUsuario(this IQueryable<Solicitud> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.SolicitudUsuario, solicitud => solicitud.SolicitudID, solicitudUsuario => solicitudUsuario.SolicitudID, (solicitud, solicitudUsuario) => new JoinSolicitudSolicitudUsuario
            {
                SolicitudUsuario = solicitudUsuario,
                Solicitud = solicitud
            });
        }
    }

    /// <summary>
    /// DataAdapter de solicitudes
    /// </summary>
    public class SolicitudAD : BaseAD
    {
        private EntityContext mEntityContext;

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public SolicitudAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters();
        }


        /// <summary>
        /// Constructor a partir de la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public SolicitudAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContext = entityContext;
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        #region Sólo parte Select

        private string SelectSolicitud;
        private string SelectSolicitudNuevoUsuario;
        private string SelectSolicitudNuevaOrganizacion;
        private string SelectSolicitudNuevaOrgEmp;
        private string SelectSolicitudNuevaClase;
        private string SelectSolicitudUsuario;
        private string SelectSolicitudGrupo;
        private string SelectSolicitudNuevoProfesor;

        private string SelectDatoExtraEcosistemaOpcionSolicitud;
        private string SelectDatoExtraEcosistemaVirtuosoSolicitud;
        private string SelectDatoExtraProyectoOpcionSolicitud;
        private string SelectDatoExtraProyectoVirtuosoSolicitud;

        private string SelectSolicitudOrganizacion;
        private string SelectSolicitudOrgPersona;

        #endregion

        private string sqlSelectSolicitudAccesoProyecto;
        private string sqlSelectSolicitudesAProyectoDeUsuario;
        private string sqlSelectSolicitudUsuarioAccesoProyecto;
        private string sqlSelectSolicitudNuevoUsuarioAccesoProyecto;
        private string sqlSelectSolicitudNuevosUsuarios;
        private string sqlSelectSolicitudNuevoUsuarioNuevosUsuarios;
        private string sqlSelectSolicitudNuevoUsuarioNuevosProfesores;

        private string sqlSelectDatoExtraEcosistemaOpcionSolicitud;
        private string sqlSelectDatoExtraEcosistemaVirtuosoSolicitud;
        private string sqlSelectDatoExtraProyectoOpcionSolicitud;
        private string sqlSelectDatoExtraProyectoVirtuosoSolicitud;

        private string sqlSelectSolicitudNuevaOrganizacionNuevosUsuarios;
        private string sqlSelectSolicitudNuevaOrgEmpNuevosUsuarios;
        private string sqlSelectSolicitudNuevaClaseNuevosUsuarios;
        private string sqlSelectSolicitudUsuariosNuevos;
        private string sqlSelectTodasSolicitudesUsuario;
        private string sqlSelectSolicitudAccesoProyectoPorProyecto;
        private string sqlSelectNumeroSolicitudAccesoProyectoPorProyecto;
        private string sqlSelectSolicitudUsuarioAccesoProyectoPorProyecto;
        private string sqlSelectSolicitudOrganizacionAccesoProyectoPorProyecto;
        private string sqlSelectSolicitudPorID;
        private string sqlSelectSolicitudNuevoUsuarioPorID;
        private string sqlSelectSolicitudNuevaOrganizacionPorID;
        private string sqlSelectSolicitudNuevaOrgEmpPorID;
        private string sqlSelectSolicitudNuevoProfesorPorID;

        private string sqlSelectDatoExtraEcosistemaOpcionSolicitudPorID;
        private string sqlSelectDatoExtraEcosistemaVirtuosoSolicitudPorID;
        private string sqlSelectDatoExtraProyectoOpcionSolicitudPorID;
        private string sqlSelectDatoExtraProyectoVirtuosoSolicitudPorID;

        private string sqlSelectUsuarioYPerfilTienePendienteSolicitudEnProyecto;
        private string sqlSelectUsuarioTienePendienteSolicitudEnProyecto;
        private string sqlSelectOrganizacionSolicitudPendienteEnProyecto;
        private string sqlTieneUsuarioSolicitudOrganizacionNuevaEnlaQueEsAdmininstrador;

        #endregion

        #region DataAdapter

        #region SolicitudNuevoUsuario

        private string sqlSolicitudNuevoUsuarioInsert;
        private string sqlSolicitudNuevoUsuarioDelete;
        private string sqlSolicitudNuevoUsuarioModify;

        #endregion

        #region SolicitudNuevaOrganizacion

        private string sqlSolicitudNuevaOrganizacionInsert;
        private string sqlSolicitudNuevaOrganizacionDelete;
        private string sqlSolicitudNuevaOrganizacionModify;

        #endregion

        #region SolicitudNuevaOrgEmpresa

        private string sqlSolicitudNuevaOrgEmpInsert;
        private string sqlSolicitudNuevaOrgEmpDelete;
        private string sqlSolicitudNuevaOrgEmpModify;

        #endregion

        #region SolicitudNuevaClase

        private string sqlSolicitudNuevaClaseInsert;
        private string sqlSolicitudNuevaClaseDelete;
        private string sqlSolicitudNuevaClaseModify;

        #endregion

        #region Solicitud

        private string sqlSolicitudInsert;
        private string sqlSolicitudDelete;
        private string sqlSolicitudModify;

        #endregion

        #region SolicitudOrgPersona

        private string sqlSolicitudOrgPersonaInsert;
        private string sqlSolicitudOrgPersonaDelete;
        private string sqlSolicitudOrgPersonaModify;

        #endregion

        #region SolicitudUsuario

        private string sqlSolicitudUsuarioInsert;
        private string sqlSolicitudUsuarioDelete;
        private string sqlSolicitudUsuarioModify;

        #endregion

        #region SolicitudGrupo

        private string sqlSolicitudGrupoInsert;
        private string sqlSolicitudGrupoDelete;
        private string sqlSolicitudGrupoModify;

        #endregion

        #region SolicitudOrganizacion

        private string sqlSolicitudOrganizacionInsert;

        

        private string sqlSolicitudOrganizacionDelete;
        private string sqlSolicitudOrganizacionModify;

        #endregion

        #region SolicitudNuevoProfesor
        private string sqlSolicitudNuevoProfesorInsert;
        private string sqlSolicitudNuevoProfesorDelete;
        private string sqlSolicitudNuevoProfesorModify;
        #endregion

        #region DatoExtraEcosistemaOpcionSolicitud
        private string sqlDatoExtraEcosistemaOpcionSolicitudInsert;
        private string sqlDatoExtraEcosistemaOpcionSolicitudDelete;
        private string sqlDatoExtraEcosistemaOpcionSolicitudModify;
        #endregion


        #region DatoExtraEcosistemaVirtuosoSolicitud
        private string sqlDatoExtraEcosistemaVirtuosoSolicitudInsert;
        private string sqlDatoExtraEcosistemaVirtuosoSolicitudDelete;
        private string sqlDatoExtraEcosistemaVirtuosoSolicitudModify;
        #endregion


        #region DatoExtraProyectoOpcionSolicitud
        private string sqlDatoExtraProyectoOpcionSolicitudInsert;
        private string sqlDatoExtraProyectoOpcionSolicitudDelete;
        private string sqlDatoExtraProyectoOpcionSolicitudModify;
        #endregion


        #region DatoExtraProyectoVirtuosoSolicitud
        private string sqlDatoExtraProyectoVirtuosoSolicitudInsert;
        private string sqlDatoExtraProyectoVirtuosoSolicitudDelete;
        private string sqlDatoExtraProyectoVirtuosoSolicitudModify;
        #endregion

        #endregion

        #region Métodos generales

        #region Públicos

        /// <summary>
        /// Modifica la base de datos con los datos del dataset pasado por parámetro
        /// </summary>
        /// <param name="pSolicitudDS">Dataset de solicitudes</param>
        public void ActualizarBD()
        {
            ActualizarBaseDeDatosEntityContext();
        }

        /// <summary>
        /// Obtiene la solicitud de acceso de un usuario a un proyecto (si existe)
        /// </summary>
        /// <param name="pSolicitudDW">DataWrapper de solicitudes</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        public void ObtenerSolicitudAProyectoDeUsuario(DataWrapperSolicitud pSolicitudDW, Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            //Solicitud
            pSolicitudDW.ListaSolicitud = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.SolicitudUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Solicitud).ToList();
        }

        /// <summary>
        /// Devuelve el número de solicitudes a pertenecer en grupos del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de solicitudes con las solicitudes a pertenecer en grupos del proyecto</returns>
        public int ObtenerNumeroSolicitudesPertenecerGruposDeProyecto(Guid pProyectoID)
        {
            return mEntityContext.Solicitud.JoinSolicitudGrupo().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.ProyectoID.Equals(pProyectoID)).Count();
        }

        /// <summary>
        /// Obtiene el número de solicitudes de acceso pendientes a un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public int ObtenerNumeroSolicitudesAccesoProyectoPorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            int countSolicitudSolicitudUsuario = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID)).Count();

            int countSolicitudSolicitudOrganizacion = mEntityContext.Solicitud.JoinSolicitudOrganizacion().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID)).Count();

            return countSolicitudSolicitudUsuario + countSolicitudSolicitudOrganizacion;
        }

        /// <summary>
        /// Obtiene las solicitudes de acceso a un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset con los datos de las solicitudes de acceso a un proyecto</returns>
        public DataWrapperSolicitud ObtenerSolicitudesAccesoProyectoPorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            var primeraParteSubconsultaSolicitud = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Solicitud);

            var segundaParteSubconsultaSolicitud = mEntityContext.Solicitud.JoinSolicitudOrganizacion().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.Solicitud);

            solicitudDW.ListaSolicitud = primeraParteSubconsultaSolicitud.Union(segundaParteSubconsultaSolicitud).OrderBy(item => item.FechaSolicitud).ToList();

            //SolicitudUsuario
            solicitudDW.ListaSolicitudUsuario = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID)).Select(item => item.SolicitudUsuario).ToList();

            //SolicitudOrganizacion
            solicitudDW.ListaSolicitudOrganizacion = mEntityContext.Solicitud.JoinSolicitudOrganizacion().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.SolicitudOrganizacion).ToList();

            //SolicitudNuevoUsuario
            solicitudDW.ListaSolicitudNuevoUsuario = mEntityContext.Solicitud.JoinSolicitudNuevoUsuario().Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID)).Select(item => item.SolicitudNuevoUsuario).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Devuelve la solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la Solicitud</param>       
        /// <returns>Dataset de solicitudes con la solicitud</returns>
        public DataWrapperSolicitud ObtenerSolicitudPorID(Guid pSolicitudID)
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //SolicitudNuevousuario
            solicitudDW.ListaSolicitudNuevoUsuario = mEntityContext.SolicitudNuevoUsuario.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //SolicitudNuevaOrganizacion
            solicitudDW.ListaSolicitudNuevaOrganizacion = mEntityContext.SolicitudNuevaOrganizacion.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //SolicitudNuevaOrgEmp
            solicitudDW.ListaSolicitudNuevaOrgEmp = mEntityContext.SolicitudNuevaOrgEmp.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraEcosistemaOpcionSolicitud
            solicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud = mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraEcosistemaVirtuosoSolicitud
            solicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud = mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraProyectoOpcionSolicitud
            solicitudDW.ListaDatoExtraProyectoOpcionSolicitud = mEntityContext.DatoExtraProyectoOpcionSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraProyectoVirtuosoSolicitud
            solicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud = mEntityContext.DatoExtraProyectoVirtuosoSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Obtiene todas las solicitudes de acceso a un proyecto
        /// </summary>
        /// <returns>Dataset con los datos de las solicitudes de acceso a un proyecto.</returns>
        public DataWrapperSolicitud ObtenerSolicitudesAccesoProyecto()
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.Solicitud.Estado.Equals(0)).Select(item => item.Solicitud).Union(mEntityContext.Solicitud.JoinSolicitudNuevoUsuario().Where(item => item.Solicitud.Estado.Equals(0)).Select(item => item.Solicitud)).Union(mEntityContext.Solicitud.JoinSolicitudNuevaOrganizacion().Where(item => item.Solicitud.Estado.Equals(0)).Select(item => item.Solicitud)).OrderBy(item => item.FechaSolicitud).ToList();

            //SolicitudUsuario
            solicitudDW.ListaSolicitudUsuario = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.Solicitud.Estado.Equals(0)).Select(item => item.SolicitudUsuario).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Devuelve la SolicitudNuevoUsuario del Id pedido
        /// </summary>
        /// <param name="pSolicitudID"></param> Id de la solicitud a obtener
        /// <returns></returns>
        public SolicitudNuevoUsuario ObtenerSolicitudNuevoUsuarioPorSolicitudID(Guid pSolicitudID)
        {
            return mEntityContext.SolicitudNuevoUsuario.Where(item => item.SolicitudID.Equals(pSolicitudID)).FirstOrDefault();
        }

        /// <summary>
        /// Devuelve la Solicitud del Id pedido
        /// </summary>
        /// <param name="pSolicitudID"></param> Id de la solicitud a obtener
        /// <returns></returns>
        public Solicitud ObtenerSolicitudPorSolicitudID(Guid pSolicitudID)
        {
            return mEntityContext.Solicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los datos extras de la solicitud
        /// </summary>
        /// <returns></returns>
        public DataWrapperSolicitud ObtenerSolicitudesDatoExtra()
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //DatoExtraEcosistemaOpcionSolicitud
            solicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud = mEntityContext.DatoExtraEcosistemaOpcionSolicitud.ToList();

            //DatoExtraEcosistemaVirtuosoSolicitud
            solicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud = mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.ToList();

            //DatoExtraProyectoOpcionSolicitud
            solicitudDW.ListaDatoExtraProyectoOpcionSolicitud = mEntityContext.DatoExtraProyectoOpcionSolicitud.ToList();

            //DatoExtraProyectoVirtuosoSolicitud
            solicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud = mEntityContext.DatoExtraProyectoVirtuosoSolicitud.ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Obtiene todas las solicitudes de nuevos usuarios y nuevas organizaciones
        /// </summary>
        /// <returns>Dataset con los datos de las solicitudes de nuevos usuarios</returns>
        public DataWrapperSolicitud ObtenerSolicitudesNuevosUsuariosPorSolicitudID(Guid pSolicitudID)
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.Where(item => item.SolicitudID.Equals(pSolicitudID) && (item.Estado.Equals(0) || item.Estado.Equals(1))).ToList();

            //SolicitudNuevoUsuario
            solicitudDW.ListaSolicitudNuevoUsuario = mEntityContext.Solicitud.JoinSolicitudNuevoUsuario().Where(item => item.SolicitudNuevoUsuario.SolicitudID.Equals(pSolicitudID) && (item.Solicitud.Estado.Equals(0) || item.Solicitud.Estado.Equals(1))).Select(item => item.SolicitudNuevoUsuario).ToList();

            //SolicitudNuevaOrganizacion
            solicitudDW.ListaSolicitudNuevaOrganizacion = mEntityContext.Solicitud.JoinSolicitudNuevaOrganizacion().Where(item => item.Solicitud.Estado.Equals(0) || item.Solicitud.Estado.Equals(1)).OrderBy(item => item.Solicitud.FechaSolicitud).Select(item => item.SolicitudNuevaOrganizacion).ToList();

            //SolicitudNuevaOrgEmp
            solicitudDW.ListaSolicitudNuevaOrgEmp = mEntityContext.Solicitud.JoinSolicitudNuevaOrgEmp().Where(item => item.Solicitud.Estado.Equals(0) || item.Solicitud.Estado.Equals(1)).OrderBy(item => item.Solicitud.FechaSolicitud).Select(item => item.SolicitudNuevaOrgEmp).ToList();

            //DatoExtraEcosistemaOpcionSolicitud
            solicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud = mEntityContext.DatoExtraEcosistemaOpcionSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraEcosistemaVirtuosoSolicitud
            solicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud = mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraProyectoOpcionSolicitud
            solicitudDW.ListaDatoExtraProyectoOpcionSolicitud = mEntityContext.DatoExtraProyectoOpcionSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            //DatoExtraProyectoVirtuosoSolicitud
            solicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud = mEntityContext.DatoExtraProyectoVirtuosoSolicitud.Where(item => item.SolicitudID.Equals(pSolicitudID)).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Obtiene todas las solicitudes de nuevos usuarios y nuevas organizaciones
        /// </summary>
        /// <returns>Dataset con los datos de las solicitudes de nuevos usuarios</returns>
        public DataWrapperSolicitud ObtenerSolicitudesNuevosUsuarios()
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.Where(item => item.Estado.Equals(0) || item.Estado.Equals(1)).ToList();

            //SolicitudNuevoUsuario
            solicitudDW.ListaSolicitudNuevoUsuario = mEntityContext.Solicitud.JoinSolicitudNuevoUsuario().Where(item => item.Solicitud.Estado.Equals(0) || item.Solicitud.Estado.Equals(1)).Select(item => item.SolicitudNuevoUsuario).ToList();

            //SolicitudNuevaOrganizacion
            solicitudDW.ListaSolicitudNuevaOrganizacion = mEntityContext.Solicitud.JoinSolicitudNuevaOrganizacion().Where(item => item.Solicitud.Estado.Equals(0) || item.Solicitud.Estado.Equals(1)).OrderBy(item => item.Solicitud.FechaSolicitud).Select(item => item.SolicitudNuevaOrganizacion).ToList();

            //SolicitudNuevaOrgEmp
            solicitudDW.ListaSolicitudNuevaOrgEmp = mEntityContext.Solicitud.JoinSolicitudNuevaOrgEmp().Where(item => item.Solicitud.Estado.Equals(0) || item.Solicitud.Estado.Equals(1)).OrderBy(item => item.Solicitud.FechaSolicitud).Select(item => item.SolicitudNuevaOrgEmp).ToList();

            //DatoExtraEcosistemaOpcionSolicitud
            solicitudDW.ListaDatoExtraEcosistemaOpcionSolicitud = mEntityContext.DatoExtraEcosistemaOpcionSolicitud.ToList();

            //DatoExtraEcosistemaVirtuosoSolicitud
            solicitudDW.ListaDatoExtraEcosistemaVirtuosoSolicitud = mEntityContext.DatoExtraEcosistemaVirtuosoSolicitud.ToList();

            //DatoExtraProyectoOpcionSolicitud
            solicitudDW.ListaDatoExtraProyectoOpcionSolicitud = mEntityContext.DatoExtraProyectoOpcionSolicitud.ToList();

            //DatoExtraProyectoVirtuosoSolicitud
            solicitudDW.ListaDatoExtraProyectoVirtuosoSolicitud = mEntityContext.DatoExtraProyectoVirtuosoSolicitud.ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Devuelve el tipo de registro del usuario usuario
        /// </summary>
        /// <returns>Tipo de registro del usuario</returns>
        public TipoRegistro ObtenerTipoRegistroUsuario(Guid pUsuarioID)
        {
            SolicitudNuevoUsuario solicitudNuevoUsuario = mEntityContext.SolicitudNuevoUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).FirstOrDefault();

            if (solicitudNuevoUsuario != null)
            {
                return (TipoRegistro)solicitudNuevoUsuario.TipoRegistro;
            }
            else
            {
                return TipoRegistro.Normal;
            }
        }

        /// <summary>
        /// Obtiene TODAS las solicitudes que ha hecho un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de solicitudes</returns>
        public DataWrapperSolicitud ObtenerSolicitudesDeUsuario(Guid pUsuarioID)
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.SolicitudUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Solicitud).Distinct().Union(mEntityContext.Solicitud.JoinSolicitudNuevoUsuario().Where(item => item.SolicitudNuevoUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Solicitud).Distinct()).Union(mEntityContext.Solicitud.JoinSolicitudNuevaOrganizacion().Where(item => item.SolicitudNuevaOrganizacion.UsuarioAdminID.Equals(pUsuarioID)).Select(item => item.Solicitud).Distinct()).ToList();

            //SolicitudUsuario
            solicitudDW.ListaSolicitudUsuario = mEntityContext.SolicitudUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            //SolicitudNuevoUsuario
            solicitudDW.ListaSolicitudNuevoUsuario = mEntityContext.SolicitudNuevoUsuario.Where(item => item.UsuarioID.Equals(pUsuarioID)).ToList();

            //SolicitudNuevaOrganizacion
            solicitudDW.ListaSolicitudNuevaOrganizacion = mEntityContext.SolicitudNuevaOrganizacion.Where(item => item.UsuarioAdminID.Equals(pUsuarioID)).ToList();

            //SolicitudNuevaOrgEmp
            solicitudDW.ListaSolicitudNuevaOrgEmp = mEntityContext.SolicitudNuevaOrgEmp.Where(item => item.UsuarioAdminID.Equals(pUsuarioID)).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Obtiene el proyecto en el que se registro originalmente el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Identificador del proyecto origen del usuario</returns>
        public Guid ObtenerProyectoOrigenUsuario(Guid pUsuarioID)
        {
            return mEntityContext.Solicitud.JoinSolicitudNuevoUsuario().Where(item => item.SolicitudNuevoUsuario.UsuarioID.Equals(pUsuarioID)).Select(item => item.Solicitud.ProyectoID).FirstOrDefault();
        }

        /// <summary>
        /// Comprueba si un usuario tiene una solicitud de acceso pendiente a un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pOrganizacionProyectoID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si la organización tiene solicitudes pendientes en el proyecto, FALSE en caso contrario</returns>
        public bool TieneOrganizacionSolicitudPendienteEnProyecto(Guid pOrganizacionID, Guid pOrganizacionProyectoID, Guid pProyectoID)
        {
            return mEntityContext.Solicitud.JoinSolicitudOrganizacion().Where(item => item.SolicitudOrganizacion.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionProyectoID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Any();
        }

        /// <summary>
        /// Acepta la solicitud de una identidad a participar en un grupo de proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public bool AceptarSolicitudDeIdentidadEnGrupoProyecto(Guid pIdentidadID, Guid pGrupoID)
        {
            List<Guid> listaID = mEntityContext.SolicitudGrupo.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.GrupoID.Equals(pGrupoID)).Select(item => item.SolicitudID).ToList();

            List<Solicitud> listaSolicitudes = mEntityContext.Solicitud.Where(item => listaID.Contains(item.SolicitudID)).ToList();

            foreach (Solicitud solicitud in listaSolicitudes)
            {
                solicitud.Estado = (short)EstadoSolicitud.Aceptada;
                solicitud.FechaProcesado = DateTime.Now;
            }

            ActualizarBaseDeDatosEntityContext();

            return listaSolicitudes.Count > 0;
        }

        /// <summary>
        /// Rechaza la solicitud de una identidad a participar en un grupo de proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public bool RechazarSolicitudDeIdentidadEnGrupoProyecto(Guid pIdentidadID, Guid pGrupoID)
        {
            List<Guid> listaID = mEntityContext.SolicitudGrupo.Where(item => item.IdentidadID.Equals(pIdentidadID) && item.GrupoID.Equals(pGrupoID)).Select(item => item.SolicitudID).ToList();

            List<Solicitud> listaSolicitudes = mEntityContext.Solicitud.Where(item => listaID.Contains(item.SolicitudID)).ToList();

            foreach (Solicitud solicitud in listaSolicitudes)
            {
                solicitud.Estado = (short)EstadoSolicitud.Rechazada;
                solicitud.FechaProcesado = DateTime.Now;
            }

            ActualizarBaseDeDatosEntityContext();

            return listaSolicitudes.Count > 0;
        }

        /// <summary>
        /// Obtiene las solicitudes a participar en gurpos de un proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public DataWrapperSolicitud ObtenerSolicitudesGrupoPorProyecto(Guid pProyectoID)
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.JoinSolicitudGrupo().Where(item => item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Select(item => item.Solicitud).ToList();

            //SolicitudGrupo
            solicitudDW.ListaSolicitudGrupo = mEntityContext.Solicitud.JoinSolicitudGrupo().Where(item => item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Select(item => item.SolicitudGrupo).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Obtiene una solicitud de una identidad a un grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public DataWrapperSolicitud ObtenerSolicitudDeIdentidadEnGrupo(Guid pIdentidadID, Guid pGrupoID)
        {
            DataWrapperSolicitud solicitudDW = new DataWrapperSolicitud();

            //Solicitud
            solicitudDW.ListaSolicitud = mEntityContext.Solicitud.JoinSolicitudGrupo().Where(item => item.SolicitudGrupo.IdentidadID.Equals(pIdentidadID) && item.SolicitudGrupo.GrupoID.Equals(pGrupoID)).Select(item => item.Solicitud).ToList();

            //SolicitudGrupo
            solicitudDW.ListaSolicitudGrupo = mEntityContext.Solicitud.JoinSolicitudGrupo().Where(item => item.SolicitudGrupo.IdentidadID.Equals(pIdentidadID) && item.SolicitudGrupo.GrupoID.Equals(pGrupoID)).Select(item => item.SolicitudGrupo).ToList();

            return solicitudDW;
        }

        /// <summary>
        /// Comprueba si una identidad tiene una solicitud de acceso pendiente a un grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public bool TieneSolicitudPendienteDeIdentidadEnGrupo(Guid pIdentidadID, Guid pGrupoID)
        {
            return mEntityContext.Solicitud.JoinSolicitudGrupo().Where(item => item.SolicitudGrupo.IdentidadID.Equals(pIdentidadID) && item.SolicitudGrupo.GrupoID.Equals(pGrupoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Select(item => item.SolicitudGrupo).Any();
        }

        /// <summary>
        /// Comprueba si un usuario tiene una solicitud de acceso pendiente a un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPerfilID">Identificador del Perfil</param>
        /// <returns>TRUE si el usuario tiene solicitudes pendientes en el proyecto, FALSE en caso contrario</returns>
        public bool TieneUsuarioSolicitudPendienteEnProyecto(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID, Guid pPerfilID)
        {
            return mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.SolicitudUsuario.UsuarioID.Equals(pUsuarioID) && item.SolicitudUsuario.PerfilID.Equals(pPerfilID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Any();
        }

        /// <summary>
        /// Comprueba si un usuario tiene una solicitud de acceso pendiente a un proyecto
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>TRUE si el usuario tiene solicitudes pendientes en el proyecto, FALSE en caso contrario</returns>
        public bool TieneUsuarioSolicitudPendienteEnProyecto(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID)
        {
            return mEntityContext.Solicitud.JoinSolicitudUsuario().Where(item => item.SolicitudUsuario.UsuarioID.Equals(pUsuarioID) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID) && item.Solicitud.Estado.Equals((short)EstadoSolicitud.Espera)).Any();
        }

        /// <summary>
        /// Comprueba si un usuario está asignado como administrador de una nueva organización que se desea registrar.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario que puede ser administrador de una organización pendiente</param>
        /// <returns>TRUE si el usuario es admininstrador de una organización solicitada, FALSE en caso contrario</returns>
        public bool TieneUsuarioSolicitudOrganizacionNuevaEnlaQueEsAdmininstrador(Guid pUsuarioID)
        {
            return mEntityContext.SolicitudNuevaOrganizacion.Where(item => item.UsuarioAdminID.Equals(pUsuarioID)).Any();
        }

        /// <summary>
        /// Guarda el token para un login del API del servicio de login.
        /// </summary>
        /// <param name="pToken">Token</param>
        /// <param name="pLogin">Login de usuario</param>
        public void GuardarTokenAccesoAPILogin(Guid pToken, string pLogin, DateTime pDate)
        {
            //DbCommand comandoInsertTokenApi = ObtenerComando("INSERT INTO TokenApiLogin(Token, Login, Fecha) VALUES (" + IBD.GuidValor(pToken) + "," + IBD.ToParam("Login") + "," + IBD.ToParam("Fecha") + ")");
            //AgregarParametro(comandoInsertTokenApi, IBD.ToParam("Login"), DbType.String, pLogin);
            //AgregarParametro(comandoInsertTokenApi, IBD.ToParam("Fecha"), DbType.DateTime, pDate);
            //EjecutarEscalar(comandoInsertTokenApi);
            TokenApiLogin tokenApiLogin = new TokenApiLogin()
            {
                Fecha = pDate,
                Login = pLogin,
                Token = pToken
            };
            mEntityContext.TokenApiLogin.Add(tokenApiLogin);
            mEntityContext.SaveChanges();
        }

        /// <summary>
        /// Guarda el Token de Alta para un usuario del API del servicio de login.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <param name="pToken">Token de alta</param>
        /// <param name="pNombreCortoProyInicio">Nombre corto del proyecto de inicio</param>
        public void GuardarTokenAltaUsuarioAPILogin(Guid pUsuarioID, Guid pToken, string pNombreCortoProyInicio)
        {
            DbCommand comandoInsertTokenApi = ObtenerComando("INSERT INTO TokenAltaUsuarioApiLogin(UsuarioID, TokenAlta, Fecha, ProyectoInicio) VALUES (" + IBD.GuidValor(pUsuarioID) + "," + IBD.GuidValor(pToken) + "," + IBD.ToParam("Fecha") + "," + IBD.ToParam("ProyInicio") + ")");
            AgregarParametro(comandoInsertTokenApi, IBD.ToParam("Fecha"), DbType.DateTime, DateTime.Now);
            AgregarParametro(comandoInsertTokenApi, IBD.ToParam("ProyInicio"), DbType.String, pNombreCortoProyInicio);
            EjecutarEscalar(comandoInsertTokenApi);
        }

        /// <summary>
        /// Obtiene el Token de Alta de un usuario del API del servicio de login.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <returns>Token de alta del usuario</returns>
        public Guid ObtenerTokenAltaUsuarioAPILogin(Guid pUsuarioID)
        {
            DataSet dataSet = new DataSet();
            DbCommand selectTokenApi = ObtenerComando("SELECT TokenAlta FROM TokenAltaUsuarioApiLogin WHERE UsuarioID=" + IBD.GuidValor(pUsuarioID));
            CargarDataSet(selectTokenApi, dataSet, "TokenAltaUsuarioApiLogin");

            Guid tokenAlta = Guid.Empty;

            if (dataSet.Tables[0].Rows.Count > 0)
            {
                tokenAlta = (Guid)dataSet.Tables[0].Rows[0]["TokenAlta"];
            }

            dataSet.Dispose();
            return tokenAlta;
        }

        /// <summary>
        /// Obtiene el proyecto de inicio de un usuario del API del servicio de login.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <returns>Proyecto de inicio si lo tiene, NULL si no</returns>
        public string ObtenerProyectoInicioUsuarioAPILogin(Guid pUsuarioID)
        {
            DataSet dataSet = new DataSet();
            DbCommand selectTokenApi = ObtenerComando("SELECT ProyectoInicio FROM TokenAltaUsuarioApiLogin WHERE UsuarioID=" + IBD.GuidValor(pUsuarioID));
            CargarDataSet(selectTokenApi, dataSet, "TokenAltaUsuarioApiLogin");

            string proyInicio = null;

            if (dataSet.Tables[0].Rows.Count > 0 && !dataSet.Tables[0].Rows[0].IsNull("ProyectoInicio"))
            {
                proyInicio = (string)dataSet.Tables[0].Rows[0]["ProyectoInicio"];
            }

            dataSet.Dispose();
            return proyInicio;
        }

        /// <summary>
        /// Obtiene el login apartir de un token del API de Login.
        /// </summary>
        /// <param name="pToken">Token</param>
        /// <returns>Login apartir de un token del API de Login</returns>
        public string ObtenerLoginAPILoginDeToken(Guid pToken, bool pBorrarToken)
        {
            string login = mEntityContext.TokenApiLogin.Where(item => item.Token.Equals(pToken) && (item.Fecha > DateTime.UtcNow.AddDays(-1) || item.Login.Equals("Permanente"))).Select(item => item.Login).FirstOrDefault();

            if (pBorrarToken && login != null && !login.Equals("Permanente"))
            {
                BorrarTokenAPILogin(pToken);
            }

            return login;
        }

        /// <summary>
        /// Borra un token del Api de Login.
        /// </summary>
        /// <param name="pToken">Token</param>
        public void BorrarTokenAPILogin(Guid pToken)
        {
            TokenApiLogin token = mEntityContext.TokenApiLogin.Where(item => item.Token.Equals(pToken)).FirstOrDefault();
            if(token != null)
            {
                mEntityContext.Entry(token).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                mEntityContext.SaveChanges();
            }
        }

        /// <summary>
        /// Borra los tokens de Documentos de hace un dia
        /// </summary>
        /// <param name="pToken">Token</param>
        public void BorrarTokenAntiguos()
        {
            DbCommand comandoDeleteTokenApi = ObtenerComando("DELETE FROM TokenApiLogin WHERE Login= " + IBD.ToParam("Login") + " AND Fecha > " + IBD.ToParam("Fecha"));
            AgregarParametro(comandoDeleteTokenApi, IBD.ToParam("Fecha"), DbType.DateTime, DateTime.Today);
            AgregarParametro(comandoDeleteTokenApi, IBD.ToParam("Login"), DbType.String, "Documento");
            EjecutarEscalar(comandoDeleteTokenApi);
        }

        /// <summary>
        /// Guarda promoción y usuario de un nuevo registro.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <param name="pPromocionID">ID de la promoción</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        public void GuardarAltaPromocionUsuario(Guid pUsuarioID, Guid pPromocionID, Guid? pProyectoID)
        {
            string proyectoID = "null";
            if (pProyectoID.HasValue)
            {
                proyectoID = IBD.GuidValor(pProyectoID.Value);
            }
            DbCommand comandoInsertTokenApi = ObtenerComando("INSERT INTO UsuarioProyectoPromocion(UsuarioID, PromocionID, ProyectoID) VALUES (" + IBD.GuidValor(pUsuarioID) + "," + IBD.GuidValor(pPromocionID) + "," + proyectoID + ")");

            EjecutarEscalar(comandoInsertTokenApi);
        }
        public bool RechazarSolicitud(Guid solicitudID)
        {
            bool rechazado = false;
            SolicitudUsuario solicitudUsuario = mEntityContext.SolicitudUsuario.FirstOrDefault(item => item.SolicitudID.Equals(solicitudID));
            if(solicitudUsuario != null)
            {
                mEntityContext.EliminarElemento(solicitudUsuario);
            }
            Solicitud solicitud = mEntityContext.Solicitud.FirstOrDefault(item => item.SolicitudID.Equals(solicitudID));
            if (solicitud != null)
            {
                mEntityContext.EliminarElemento(solicitud);
            }
            if(solicitudUsuario != null && solicitud != null)
            {
                rechazado = true;
            }
            mEntityContext.SaveChanges();
            return rechazado;
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
            #region Consultas

            #region Sólo parte Select

            this.SelectSolicitudNuevaOrganizacion = "SELECT " + IBD.CargarGuid("SolicitudNuevaOrganizacion.SolicitudID") + ", " + IBD.CargarGuid("SolicitudNuevaOrganizacion.UsuarioAdminID") + ", SolicitudNuevaOrganizacion.Nombre, SolicitudNuevaOrganizacion.URLFoto, " + IBD.CargarGuid("SolicitudNuevaOrganizacion.PaisID") + ", " + IBD.CargarGuid("SolicitudNuevaOrganizacion.ProvinciaID") + ", SolicitudNuevaOrganizacion.Provincia, SolicitudNuevaOrganizacion.CP, SolicitudNuevaOrganizacion.Poblacion, SolicitudNuevaOrganizacion.Direccion, SolicitudNuevaOrganizacion.PaginaWeb, SolicitudNuevaOrganizacion.EsBuscable, SolicitudNuevaOrganizacion.EsBuscableExternos, SolicitudNuevaOrganizacion.CargoContactoPrincipal, SolicitudNuevaOrganizacion.EmailContactoPrincipal, SolicitudNuevaOrganizacion.ModoPersonal, SolicitudNuevaOrganizacion.NombreCorto, SolicitudNuevaOrganizacion.Alias FROM SolicitudNuevaOrganizacion";

            this.SelectSolicitudNuevaOrgEmp = "SELECT " + IBD.CargarGuid("SolicitudNuevaOrgEmp.SolicitudID") + ", " + IBD.CargarGuid("SolicitudNuevaOrgEmp.UsuarioAdminID") + ", SolicitudNuevaOrgEmp.CIF, SolicitudNuevaOrgEmp.Tipo, SolicitudNuevaOrgEmp.FechaFundacion, SolicitudNuevaOrgEmp.Empleados, SolicitudNuevaOrgEmp.Sector FROM SolicitudNuevaOrgEmp";

            this.SelectSolicitudNuevaClase = "SELECT " + IBD.CargarGuid("SolicitudNuevaClase.SolicitudID") + ", " + IBD.CargarGuid("SolicitudNuevaClase.UsuarioAdminID") + ", Centro, Asignatura, Curso, Grupo, CursoAcademico, NombreCortoCentro, NombreCortoAsig, TipoClase FROM SolicitudNuevaClase ";

            this.SelectSolicitudNuevoUsuario = "SELECT " + IBD.CargarGuid("SolicitudNuevoUsuario.SolicitudID") + ", " + IBD.CargarGuid("SolicitudNuevoUsuario.UsuarioID") + ", SolicitudNuevoUsuario.Nombre, SolicitudNuevoUsuario.Apellidos, SolicitudNuevoUsuario.URLFoto, " + IBD.CargarGuid("SolicitudNuevoUsuario.PaisID") + ", " + IBD.CargarGuid("SolicitudNuevoUsuario.ProvinciaID") + ", SolicitudNuevoUsuario.Provincia, SolicitudNuevoUsuario.CP, SolicitudNuevoUsuario.Direccion, SolicitudNuevoUsuario.Poblacion, SolicitudNuevoUsuario.Sexo, SolicitudNuevoUsuario.FechaNacimiento, SolicitudNuevoUsuario.EsBuscable, SolicitudNuevoUsuario.EsBuscableExterno, SolicitudNuevoUsuario.Email, SolicitudNuevoUsuario.Idioma, SolicitudNuevoUsuario.NombreCorto, SolicitudNuevoUsuario.EmailTutor, SolicitudNuevoUsuario.CrearClase, SolicitudNuevoUsuario.ClausulasAdicionales, SolicitudNuevoUsuario.CambioPassword, SolicitudNuevoUsuario.ProyectosAutoAcceso, SolicitudNuevoUsuario.FaltanDatos, SolicitudNuevoUsuario.TipoRegistro FROM SolicitudNuevoUsuario";

            this.SelectSolicitud = "SELECT " + IBD.CargarGuid("Solicitud.SolicitudID") + ", Solicitud.FechaSolicitud, Solicitud.FechaProcesado, Solicitud.Estado, " + IBD.CargarGuid("Solicitud.OrganizacionID") + ", " + IBD.CargarGuid("ProyectoID") + " FROM Solicitud";

            this.SelectSolicitudOrgPersona = "SELECT " + IBD.CargarGuid("SolicitudOrgPersona.SolicitudID") + ", " + IBD.CargarGuid("SolicitudOrgPersona.OrganizacionID") + ", " + IBD.CargarGuid("SolicitudOrgPersona.PersonaID") + " FROM SolicitudOrgPersona";

            this.SelectSolicitudUsuario = "SELECT " + IBD.CargarGuid("SolicitudUsuario.SolicitudID") + ", " + IBD.CargarGuid("SolicitudUsuario.UsuarioID") + ", " + IBD.CargarGuid("SolicitudUsuario.PersonaID") + ", " + IBD.CargarGuid("SolicitudUsuario.PerfilID") + ", SolicitudUsuario.ClausulasAdicionales FROM SolicitudUsuario";

            this.SelectSolicitudGrupo = "SELECT " + IBD.CargarGuid("SolicitudGrupo.SolicitudID") + ", " + IBD.CargarGuid("SolicitudGrupo.GrupoID") + ", " + IBD.CargarGuid("SolicitudGrupo.IdentidadID") + " FROM SolicitudGrupo";

            this.SelectSolicitudOrganizacion = "SELECT " + IBD.CargarGuid("SolicitudOrganizacion.SolicitudID") + ", " + IBD.CargarGuid("SolicitudOrganizacion.OrganizacionID") + " FROM SolicitudOrganizacion";

            this.SelectSolicitudNuevoProfesor = "SELECT " + IBD.CargarGuid("SolicitudNuevoProfesor.SolicitudID") + ", " + IBD.CargarGuid("SolicitudNuevoProfesor.UsuarioID") + ", SolicitudNuevoProfesor.Email, SolicitudNuevoProfesor.CentroEstudios, SolicitudNuevoProfesor.AreaEstudios FROM SolicitudNuevoProfesor";

            this.SelectDatoExtraEcosistemaOpcionSolicitud = "SELECT " + IBD.CargarGuid("DatoExtraEcosistemaOpcionSolicitud.DatoExtraID") + ", " + IBD.CargarGuid("DatoExtraEcosistemaOpcionSolicitud.OpcionID") + ", " + IBD.CargarGuid("DatoExtraEcosistemaOpcionSolicitud.SolicitudID") + " FROM DatoExtraEcosistemaOpcionSolicitud";

            this.SelectDatoExtraEcosistemaVirtuosoSolicitud = "SELECT " + IBD.CargarGuid("DatoExtraEcosistemaVirtuosoSolicitud.DatoExtraID") + ", " + IBD.CargarGuid("DatoExtraEcosistemaVirtuosoSolicitud.SolicitudID") + ", DatoExtraEcosistemaVirtuosoSolicitud.Opcion FROM DatoExtraEcosistemaVirtuosoSolicitud";

            this.SelectDatoExtraProyectoOpcionSolicitud = "SELECT " + IBD.CargarGuid("DatoExtraProyectoOpcionSolicitud.OrganizacionID") + ", " + IBD.CargarGuid("DatoExtraProyectoOpcionSolicitud.ProyectoID") + ", " + IBD.CargarGuid("DatoExtraProyectoOpcionSolicitud.DatoExtraID") + ", " + IBD.CargarGuid("DatoExtraProyectoOpcionSolicitud.OpcionID") + ", " + IBD.CargarGuid("DatoExtraProyectoOpcionSolicitud.SolicitudID") + " FROM DatoExtraProyectoOpcionSolicitud";

            this.SelectDatoExtraProyectoVirtuosoSolicitud = "SELECT " + IBD.CargarGuid("DatoExtraProyectoVirtuosoSolicitud.OrganizacionID") + ", " + IBD.CargarGuid("DatoExtraProyectoVirtuosoSolicitud.ProyectoID") + ", " + IBD.CargarGuid("DatoExtraProyectoVirtuosoSolicitud.DatoExtraID") + ", " + IBD.CargarGuid("DatoExtraProyectoVirtuosoSolicitud.SolicitudID") + ", DatoExtraProyectoVirtuosoSolicitud.Opcion FROM DatoExtraProyectoVirtuosoSolicitud";

            #endregion

            this.sqlSelectUsuarioTienePendienteSolicitudEnProyecto = "SELECT 1 FROM SolicitudUsuario INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudUsuario.SolicitudID WHERE SolicitudUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString();

            this.sqlSelectUsuarioYPerfilTienePendienteSolicitudEnProyecto = "SELECT 1 FROM SolicitudUsuario INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudUsuario.SolicitudID WHERE SolicitudUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID") + " AND SolicitudUsuario.PerfilID = " + IBD.GuidParamValor("perfilID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString();

            this.sqlSelectOrganizacionSolicitudPendienteEnProyecto = "SELECT 1 FROM SolicitudOrganizacion INNER JOIN Solicitud ON Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID WHERE SolicitudOrganizacion.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionProyectoID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.Estado = " + ((short)EstadoSolicitud.Espera).ToString();

            this.sqlTieneUsuarioSolicitudOrganizacionNuevaEnlaQueEsAdmininstrador = "SELECT 1 FROM SolicitudNuevaOrganizacion WHERE UsuarioAdminID = " + IBD.GuidParamValor("usuarioID");

            this.sqlSelectSolicitudesAProyectoDeUsuario = SelectSolicitud + " INNER JOIN SolicitudUsuario ON (Solicitud.SolicitudID = SolicitudUsuario.SolicitudID) WHERE Estado = 0 AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND SolicitudUsuario.UsuarioID = " + IBD.GuidParamValor("usuarioID");

            this.sqlSelectSolicitudAccesoProyecto = SelectSolicitud + " INNER JOIN SolicitudUsuario ON (Solicitud.SolicitudID = SolicitudUsuario.SolicitudID) WHERE Estado = 0 UNION ALL " + SelectSolicitud + " INNER JOIN SolicitudNuevoUsuario ON (Solicitud.SolicitudID = SolicitudNuevoUsuario.SolicitudID) WHERE Estado = 0 UNION ALL " + SelectSolicitud + " INNER JOIN SolicitudNuevaOrganizacion ON (Solicitud.SolicitudID = SolicitudNuevaOrganizacion.SolicitudID) WHERE Estado = 0 ORDER FechaSolicitud";

            this.sqlSelectSolicitudUsuarioAccesoProyecto = SelectSolicitudUsuario + " INNER JOIN Solicitud ON SolicitudUsuario.SolicitudID = Solicitud.SolicitudID WHERE Solicitud.Estado = 0";

            this.sqlSelectSolicitudAccesoProyectoPorProyecto = SelectSolicitud + " INNER JOIN SolicitudUsuario ON (Solicitud.SolicitudID = SolicitudUsuario.SolicitudID) WHERE Estado = 0 AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " UNION ALL " + SelectSolicitud + " INNER JOIN SolicitudOrganizacion ON (Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID) WHERE Estado = 0 AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ORDER BY FechaSolicitud";

            this.sqlSelectNumeroSolicitudAccesoProyectoPorProyecto = "SELECT SUM(NUMERO) FROM (SELECT COUNT(*) NUMERO FROM SOLICITUD INNER JOIN SolicitudUsuario ON (Solicitud.SolicitudID = SolicitudUsuario.SolicitudID) WHERE Estado = 0 AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " UNION ALL SELECT COUNT(*) NUMERO FROM SOLICITUD  INNER JOIN SolicitudOrganizacion ON (Solicitud.SolicitudID = SolicitudOrganizacion.SolicitudID) WHERE Estado = 0 AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ") subconsulta";

            this.sqlSelectSolicitudUsuarioAccesoProyectoPorProyecto = SelectSolicitudUsuario + " INNER JOIN Solicitud ON SolicitudUsuario.SolicitudID = Solicitud.SolicitudID WHERE Solicitud.Estado = 0 AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectSolicitudOrganizacionAccesoProyectoPorProyecto = SelectSolicitudOrganizacion + " INNER JOIN Solicitud ON SolicitudOrganizacion.SolicitudID = Solicitud.SolicitudID WHERE Solicitud.Estado = 0 AND solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectSolicitudNuevoUsuarioAccesoProyecto = SelectSolicitudNuevoUsuario + " INNER JOIN Solicitud ON SolicitudNuevoUsuario.SolicitudID = Solicitud.SolicitudID WHERE Solicitud.Estado = 0 AND solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID");

            this.sqlSelectSolicitudPorID = SelectSolicitud + " where solicitud.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectSolicitudNuevoUsuarioPorID = SelectSolicitudNuevoUsuario + " where solicitudnuevousuario.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectSolicitudNuevaOrganizacionPorID = SelectSolicitudNuevaOrganizacion + " where solicitudnuevaorganizacion.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectSolicitudNuevaOrgEmpPorID = SelectSolicitudNuevaOrgEmp + " where solicitudnuevaorgemp.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectSolicitudNuevoProfesorPorID = SelectSolicitudNuevoProfesor + " where solicitudnuevoprofesor.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectDatoExtraEcosistemaOpcionSolicitudPorID = SelectDatoExtraEcosistemaOpcionSolicitud + " where DatoExtraEcosistemaOpcionSolicitud.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectDatoExtraEcosistemaVirtuosoSolicitudPorID = SelectDatoExtraEcosistemaVirtuosoSolicitud + " where DatoExtraEcosistemaVirtuosoSolicitud.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectDatoExtraProyectoOpcionSolicitudPorID = SelectDatoExtraProyectoOpcionSolicitud + " where DatoExtraProyectoOpcionSolicitud.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectDatoExtraProyectoVirtuosoSolicitudPorID = SelectDatoExtraProyectoVirtuosoSolicitud + " where DatoExtraProyectoVirtuosoSolicitud.solicitudid=" + IBD.GuidParamValor("solicitudID");

            this.sqlSelectSolicitudNuevosUsuarios = SelectSolicitud + " INNER JOIN SolicitudNuevoUsuario ON (Solicitud.SolicitudID = SolicitudNuevoUsuario.SolicitudID) WHERE (Estado = 0 OR Estado= 1) UNION " + SelectSolicitud + " INNER JOIN SolicitudNuevaOrganizacion ON (Solicitud.SolicitudID = SolicitudNuevaOrganizacion.SolicitudID) WHERE (Estado = 0 OR Estado= 1) UNION " + SelectSolicitud + " INNER JOIN SolicitudNuevoProfesor ON (Solicitud.SolicitudID = SolicitudNuevoProfesor.SolicitudID) WHERE (Estado = 0 OR Estado= 1) ORDER BY FechaSolicitud";

            this.sqlSelectSolicitudNuevoUsuarioNuevosUsuarios = SelectSolicitudNuevoUsuario + " INNER JOIN Solicitud ON SolicitudNuevoUsuario.SolicitudID = Solicitud.SolicitudID WHERE (Solicitud.Estado = 0 OR Solicitud.Estado= 1) ORDER BY Solicitud.FechaSolicitud";

            this.sqlSelectSolicitudNuevoUsuarioNuevosProfesores = SelectSolicitudNuevoProfesor + " INNER JOIN Solicitud ON SolicitudNuevoProfesor.SolicitudID = Solicitud.SolicitudID WHERE (Solicitud.Estado = 0 OR Solicitud.Estado= 1) ORDER BY Solicitud.FechaSolicitud";

            this.sqlSelectDatoExtraEcosistemaOpcionSolicitud = SelectDatoExtraEcosistemaOpcionSolicitud;

            this.sqlSelectDatoExtraEcosistemaVirtuosoSolicitud = SelectDatoExtraEcosistemaVirtuosoSolicitud;

            this.sqlSelectDatoExtraProyectoOpcionSolicitud = SelectDatoExtraProyectoOpcionSolicitud;

            this.sqlSelectDatoExtraProyectoVirtuosoSolicitud = SelectDatoExtraProyectoVirtuosoSolicitud;

            this.sqlSelectSolicitudNuevaOrganizacionNuevosUsuarios = SelectSolicitudNuevaOrganizacion + " INNER JOIN Solicitud ON SolicitudNuevaOrganizacion.SolicitudID = Solicitud.SolicitudID WHERE (Solicitud.Estado = 0 OR Solicitud.Estado= 1) ORDER BY FechaSolicitud";

            this.sqlSelectSolicitudNuevaOrgEmpNuevosUsuarios = SelectSolicitudNuevaOrgEmp + " INNER JOIN Solicitud ON SolicitudNuevaOrgEmp.SolicitudID = Solicitud.SolicitudID WHERE (Solicitud.Estado = 0 OR Solicitud.Estado= 1) ORDER BY FechaSolicitud";

            this.sqlSelectSolicitudNuevaClaseNuevosUsuarios = SelectSolicitudNuevaClase + " INNER JOIN Solicitud ON SolicitudNuevaClase.SolicitudID = Solicitud.SolicitudID WHERE (Solicitud.Estado = 0 OR Solicitud.Estado= 1) ORDER BY FechaSolicitud";

            this.sqlSelectSolicitudUsuariosNuevos = "SELECT " + IBD.CargarGuid("Solicitud.SolicitudID") + ", FechaSolicitud, FechaProcesado, Estado, Nombre, Apellidos, NIF, FechaNacimiento, Cargo, TelefonoContacto, TelefonoMovil, Email, " + IBD.CargarGuid("PaisID") + ", Provincia, Localidad, CP, Direccion, Login, " + IBD.CargarGuid("ProvinciaID") + ", " + IBD.CargarGuid("ProyectoID") + ", " + IBD.CargarGuid("OrganizacionID") + " FROM Solicitud WHERE Estado = 0 AND Solicitud.ProyectoID = " + IBD.GuidParamValor("proyectoID") + " AND Solicitud.OrganizacionID = " + IBD.GuidParamValor("organizacionID") + " ORDER BY FechaSolicitud";

            this.sqlSelectTodasSolicitudesUsuario = SelectSolicitud.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN SolicitudUsuario ON SolicitudUsuario.SolicitudID = Solicitud.SolicitudID WHERE SolicitudUsuario.UsuarioID = " + IBD.GuidParamValor("UsuarioID") + " UNION " + SelectSolicitud.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN SolicitudNuevoUsuario ON SolicitudNuevoUsuario.SolicitudID = Solicitud.SolicitudID WHERE SolicitudNuevoUsuario.UsuarioID = " + IBD.GuidParamValor("UsuarioID") + " UNION " + SelectSolicitud.Replace("SELECT", "SELECT DISTINCT") + " INNER JOIN SolicitudNuevaOrganizacion ON SolicitudNuevaOrganizacion.SolicitudID = Solicitud.SolicitudID WHERE SolicitudNuevaOrganizacion.UsuarioAdminID = " + IBD.GuidParamValor("UsuarioID");

            #endregion

            #region DataAdapter

            #region SolicitudNuevaOrganizacion
            this.sqlSolicitudNuevaOrganizacionInsert = IBD.ReplaceParam("INSERT INTO SolicitudNuevaOrganizacion (SolicitudID, UsuarioAdminID, Nombre, URLFoto, PaisID, ProvinciaID, Provincia, CP, Poblacion, Direccion, PaginaWeb, EsBuscable, EsBuscableExternos, CargoContactoPrincipal, EmailContactoPrincipal, ModoPersonal, NombreCorto, Alias) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("UsuarioAdminID") + ", @Nombre, @URLFoto, " + IBD.GuidParamColumnaTabla("PaisID") + ", " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", @Provincia, @CP, @Poblacion, @Direccion, @PaginaWeb, @EsBuscable, @EsBuscableExternos, @CargoContactoPrincipal, @EmailContactoPrincipal, @ModoPersonal, @NombreCorto, @Alias)");
            this.sqlSolicitudNuevaOrganizacionDelete = IBD.ReplaceParam("DELETE FROM SolicitudNuevaOrganizacion WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (UsuarioAdminID = " + IBD.GuidParamColumnaTabla("Original_UsuarioAdminID") + ") AND (Nombre = @Original_Nombre OR @Original_Nombre IS NULL AND Nombre IS NULL) AND (URLFoto = @Original_URLFoto OR @Original_URLFoto IS NULL AND URLFoto IS NULL) AND (PaisID = " + IBD.GuidParamColumnaTabla("Original_PaisID") + ") AND (ProvinciaID = " + IBD.GuidParamColumnaTabla("Original_ProvinciaID") + " OR " + IBD.GuidParamColumnaTabla("Original_ProvinciaID") + " IS NULL AND ProvinciaID IS NULL) AND (Provincia = @Original_Provincia) AND (CP = @Original_CP OR @Original_CP IS NULL AND CP IS NULL) AND (Poblacion = @Original_Poblacion) AND (Direccion = @Original_Direccion) AND (PaginaWeb = @Original_PaginaWeb OR @Original_PaginaWeb IS NULL AND PaginaWeb IS NULL) AND (EsBuscable = @Original_EsBuscable OR @Original_EsBuscable IS NULL AND EsBuscable IS NULL) AND (EsBuscableExternos = @Original_EsBuscableExternos OR @Original_EsBuscableExternos IS NULL AND EsBuscableExternos IS NULL) AND (CargoContactoPrincipal = @Original_CargoContactoPrincipal) AND (EmailContactoPrincipal = @Original_EmailContactoPrincipal) AND (ModoPersonal = @Original_ModoPersonal) AND (NombreCorto = @Original_NombreCorto) AND (Alias = @Original_Alias OR @Original_Alias IS NULL AND Alias IS NULL)");
            this.sqlSolicitudNuevaOrganizacionModify = IBD.ReplaceParam("UPDATE SolicitudNuevaOrganizacion SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", UsuarioAdminID = " + IBD.GuidParamColumnaTabla("UsuarioAdminID") + ", Nombre = @Nombre, URLFoto = @URLFoto, PaisID = " + IBD.GuidParamColumnaTabla("PaisID") + ", ProvinciaID = " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", Provincia = @Provincia, CP = @CP, Poblacion = @Poblacion, Direccion = @Direccion, PaginaWeb = @PaginaWeb, EsBuscable = @EsBuscable, EsBuscableExternos = @EsBuscableExternos, CargoContactoPrincipal = @CargoContactoPrincipal, EmailContactoPrincipal = @EmailContactoPrincipal, ModoPersonal = @ModoPersonal, NombreCorto = @NombreCorto, Alias = @Alias WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (UsuarioAdminID = " + IBD.GuidParamColumnaTabla("Original_UsuarioAdminID") + ") AND (Nombre = @Original_Nombre OR @Original_Nombre IS NULL AND Nombre IS NULL) AND (URLFoto = @Original_URLFoto OR @Original_URLFoto IS NULL AND URLFoto IS NULL) AND (PaisID = " + IBD.GuidParamColumnaTabla("Original_PaisID") + ") AND (ProvinciaID = " + IBD.GuidParamColumnaTabla("Original_ProvinciaID") + " OR " + IBD.GuidParamColumnaTabla("Original_ProvinciaID") + " IS NULL AND ProvinciaID IS NULL) AND (Provincia = @Original_Provincia) AND (CP = @Original_CP OR @Original_CP IS NULL AND CP IS NULL) AND (Poblacion = @Original_Poblacion) AND (Direccion = @Original_Direccion) AND (PaginaWeb = @Original_PaginaWeb OR @Original_PaginaWeb IS NULL AND PaginaWeb IS NULL) AND (EsBuscable = @Original_EsBuscable OR @Original_EsBuscable IS NULL AND EsBuscable IS NULL) AND (EsBuscableExternos = @Original_EsBuscableExternos OR @Original_EsBuscableExternos IS NULL AND EsBuscableExternos IS NULL) AND (CargoContactoPrincipal = @Original_CargoContactoPrincipal) AND (EmailContactoPrincipal = @Original_EmailContactoPrincipal) AND (ModoPersonal = @Original_ModoPersonal) AND (NombreCorto = @Original_NombreCorto) AND (Alias = @Original_Alias OR @Original_Alias IS NULL AND Alias IS NULL)");
            #endregion

            #region SolicitudNuevaOrgEmp

            this.sqlSolicitudNuevaOrgEmpInsert = IBD.ReplaceParam("INSERT INTO SolicitudNuevaOrgEmp (SolicitudID, UsuarioAdminID, CIF, Tipo, FechaFundacion, Empleados, Sector) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("UsuarioAdminID") + ", @CIF, @Tipo, @FechaFundacion, @Empleados, @Sector)");

            this.sqlSolicitudNuevaOrgEmpDelete = IBD.ReplaceParam("DELETE FROM SolicitudNuevaOrgEmp WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (UsuarioAdminID = " + IBD.GuidParamColumnaTabla("O_UsuarioAdminID") + ") AND (CIF = @O_CIF OR @O_CIF IS NULL AND CIF IS NULL) AND (Tipo = @O_Tipo) AND (FechaFundacion = @O_FechaFundacion OR @O_FechaFundacion IS NULL AND FechaFundacion IS NULL) AND (Empleados = @O_Empleados) AND (Sector = @O_Sector)");

            this.sqlSolicitudNuevaOrgEmpModify = IBD.ReplaceParam("UPDATE SolicitudNuevaOrgEmp SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", UsuarioAdminID = " + IBD.GuidParamColumnaTabla("UsuarioAdminID") + ", CIF = @CIF, Tipo = @Tipo, FechaFundacion = @FechaFundacion, Empleados = @Empleados, Sector = @Sector WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (UsuarioAdminID = " + IBD.GuidParamColumnaTabla("O_UsuarioAdminID") + ") AND (CIF = @O_CIF OR @O_CIF IS NULL AND CIF IS NULL) AND (Tipo = @O_Tipo) AND (FechaFundacion = @O_FechaFundacion OR @O_FechaFundacion IS NULL AND FechaFundacion IS NULL) AND (Empleados = @O_Empleados) AND (Sector = @O_Sector)");

            #endregion

            #region SolicitudNuevaClase

            this.sqlSolicitudNuevaClaseInsert = IBD.ReplaceParam("INSERT INTO SolicitudNuevaClase (SolicitudID, UsuarioAdminID, Centro, Asignatura, Curso, Grupo, CursoAcademico, NombreCortoCentro, NombreCortoAsig, TipoClase) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("UsuarioAdminID") + ", @Centro, @Asignatura, @Curso, @Grupo, @CursoAcademico, @NombreCortoCentro, @NombreCortoAsig, @TipoClase)");

            this.sqlSolicitudNuevaClaseDelete = IBD.ReplaceParam("DELETE FROM SolicitudNuevaClase WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ")");

            this.sqlSolicitudNuevaClaseModify = IBD.ReplaceParam("UPDATE SolicitudNuevaClase SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", UsuarioAdminID = " + IBD.GuidParamColumnaTabla("UsuarioAdminID") + ", Centro = @Centro, Asignatura = @Asignatura, Curso = @Curso, Grupo = @Grupo, CursoAcademico = @CursoAcademico, NombreCortoCentro = @NombreCortoCentro, NombreCortoAsig = @NombreCortoAsig, TipoClase = @TipoClase WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (UsuarioAdminID = " + IBD.GuidParamColumnaTabla("O_UsuarioAdminID") + ") AND (Centro = @O_Centro) AND (Asignatura = @O_Asignatura) AND (Curso = @O_Curso) AND (Grupo = @O_Grupo OR Grupo IS NULL AND @O_Grupo IS NULL) AND (CursoAcademico = @O_CursoAcademico) AND (NombreCortoCentro = @O_NombreCortoCentro) AND (NombreCortoAsig = @O_NombreCortoAsignatura) AND (TipoClase = @O_TipoClase)");

            #endregion

            #region SolicitudNuevoUsuario
            this.sqlSolicitudNuevoUsuarioInsert = IBD.ReplaceParam("INSERT INTO SolicitudNuevoUsuario (SolicitudID, UsuarioID, Nombre, Apellidos, URLFoto, PaisID, ProvinciaID, Provincia, CP, Direccion, Poblacion, Sexo, FechaNacimiento, EsBuscable, EsBuscableExterno, Email, Idioma, NombreCorto, EmailTutor, CrearClase, ClausulasAdicionales, CambioPassword, ProyectosAutoAcceso, FaltanDatos, TipoRegistro) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", @Nombre, @Apellidos, @URLFoto, " + IBD.GuidParamColumnaTabla("PaisID") + ", " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", @Provincia, @CP, @Direccion, @Poblacion, @Sexo, @FechaNacimiento, @EsBuscable, @EsBuscableExterno, @Email, @Idioma, @NombreCorto, @EmailTutor, @CrearClase, @ClausulasAdicionales, @CambioPassword, @ProyectosAutoAcceso, @FaltanDatos, @TipoRegistro)");
            this.sqlSolicitudNuevoUsuarioDelete = IBD.ReplaceParam("DELETE FROM SolicitudNuevoUsuario WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + ")");
            this.sqlSolicitudNuevoUsuarioModify = IBD.ReplaceParam("UPDATE SolicitudNuevoUsuario SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", Nombre = @Nombre, Apellidos = @Apellidos, URLFoto = @URLFoto, PaisID = " + IBD.GuidParamColumnaTabla("PaisID") + ", ProvinciaID = " + IBD.GuidParamColumnaTabla("ProvinciaID") + ", Provincia = @Provincia, CP = @CP, Direccion = @Direccion, Poblacion = @Poblacion, Sexo = @Sexo, FechaNacimiento = @FechaNacimiento, EsBuscable = @EsBuscable, EsBuscableExterno = @EsBuscableExterno, Email = @Email, Idioma = @Idioma, NombreCorto = @NombreCorto, EmailTutor = @EmailTutor, CrearClase = @CrearClase, ClausulasAdicionales = @ClausulasAdicionales, CambioPassword = @CambioPassword, ProyectosAutoAcceso = @ProyectosAutoAcceso, FaltanDatos = @FaltanDatos, TipoRegistro = @TipoRegistro WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + ") ");
            #endregion

            #region Solicitud

            this.sqlSolicitudInsert = IBD.ReplaceParam("INSERT INTO Solicitud (SolicitudID, FechaSolicitud, FechaProcesado, Estado, OrganizacionID, ProyectoID) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", @FechaSolicitud, @FechaProcesado, @Estado, " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ")");

            this.sqlSolicitudDelete = IBD.ReplaceParam("DELETE FROM Solicitud WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (FechaSolicitud = @O_FechaSolicitud) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL) AND (Estado = @O_Estado) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            this.sqlSolicitudModify = IBD.ReplaceParam("UPDATE Solicitud SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", FechaSolicitud = @FechaSolicitud, FechaProcesado = @FechaProcesado, Estado = @Estado, OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (FechaSolicitud = @O_FechaSolicitud) AND (FechaProcesado = @O_FechaProcesado OR @O_FechaProcesado IS NULL AND FechaProcesado IS NULL) AND (Estado = @O_Estado) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("O_ProyectoID") + ")");

            #endregion

            #region SolicitudOrgPersona

            this.sqlSolicitudOrgPersonaInsert = IBD.ReplaceParam("INSERT INTO SolicitudOrgPersona (SolicitudID, OrganizacionID, PersonaID) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("PersonaID") + ")");

            this.sqlSolicitudOrgPersonaDelete = IBD.ReplaceParam("DELETE FROM SolicitudOrgPersona WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ")");

            this.sqlSolicitudOrgPersonaModify = IBD.ReplaceParam("UPDATE SolicitudOrgPersona SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("O_PersonaID") + ")");

            #endregion

            #region SolicitudUsuario
            this.sqlSolicitudUsuarioInsert = IBD.ReplaceParam("INSERT INTO SolicitudUsuario (SolicitudID, UsuarioID, PersonaID, PerfilID, ClausulasAdicionales) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", " + IBD.GuidParamColumnaTabla("PersonaID") + ", " + IBD.GuidParamColumnaTabla("PerfilID") + ", @ClausulasAdicionales)");
            this.sqlSolicitudUsuarioDelete = IBD.ReplaceParam("DELETE FROM SolicitudUsuario WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("Original_PersonaID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("Original_PerfilID") + ") AND (ClausulasAdicionales = @Original_ClausulasAdicionales OR @Original_ClausulasAdicionales IS NULL AND ClausulasAdicionales IS NULL)");
            this.sqlSolicitudUsuarioModify = IBD.ReplaceParam("UPDATE SolicitudUsuario SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", PersonaID = " + IBD.GuidParamColumnaTabla("PersonaID") + ", PerfilID = " + IBD.GuidParamColumnaTabla("PerfilID") + ", ClausulasAdicionales = @ClausulasAdicionales WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (UsuarioID = " + IBD.GuidParamColumnaTabla("Original_UsuarioID") + ") AND (PersonaID = " + IBD.GuidParamColumnaTabla("Original_PersonaID") + ") AND (PerfilID = " + IBD.GuidParamColumnaTabla("Original_PerfilID") + ") AND (ClausulasAdicionales = @Original_ClausulasAdicionales OR @Original_ClausulasAdicionales IS NULL AND ClausulasAdicionales IS NULL)");
            #endregion

            #region SolicitudGrupo
            this.sqlSolicitudGrupoInsert = IBD.ReplaceParam("INSERT INTO SolicitudGrupo (SolicitudID, GrupoID, IdentidadID) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("GrupoID") + ", " + IBD.GuidParamColumnaTabla("IdentidadID") + ")");
            this.sqlSolicitudGrupoDelete = IBD.ReplaceParam("DELETE FROM SolicitudGrupo WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (GrupoID = " + IBD.GuidParamColumnaTabla("Original_GrupoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("Original_IdentidadID") + ")");
            this.sqlSolicitudGrupoModify = IBD.ReplaceParam("UPDATE SolicitudGrupo SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", GrupoID = " + IBD.GuidParamColumnaTabla("GrupoID") + ", IdentidadID = " + IBD.GuidParamColumnaTabla("IdentidadID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (GrupoID = " + IBD.GuidParamColumnaTabla("Original_GrupoID") + ") AND (IdentidadID = " + IBD.GuidParamColumnaTabla("Original_IdentidadID") + ") ");
            #endregion

            #region SolicitudOrganizacion

            this.sqlSolicitudOrganizacionInsert = IBD.ReplaceParam("INSERT INTO SolicitudOrganizacion (SolicitudID, OrganizacionID) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("OrganizacionID") + ")");

            this.sqlSolicitudOrganizacionDelete = IBD.ReplaceParam("DELETE FROM SolicitudOrganizacion WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            this.sqlSolicitudOrganizacionModify = IBD.ReplaceParam("UPDATE SolicitudOrganizacion SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("O_SolicitudID") + ") AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            #endregion

            #region SolicitudNuevoProfesor
            this.sqlSolicitudNuevoProfesorInsert = IBD.ReplaceParam("INSERT INTO SolicitudNuevoProfesor (SolicitudID, UsuarioID, Email, CentroEstudios, AreaEstudios) VALUES (" + IBD.GuidParamColumnaTabla("SolicitudID") + ", " + IBD.GuidParamColumnaTabla("UsuarioID") + ", @Email, @CentroEstudios, @AreaEstudios)");
            this.sqlSolicitudNuevoProfesorDelete = IBD.ReplaceParam("DELETE FROM SolicitudNuevoProfesor WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ")");
            this.sqlSolicitudNuevoProfesorModify = IBD.ReplaceParam("UPDATE SolicitudNuevoProfesor SET SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", UsuarioID = " + IBD.GuidParamColumnaTabla("UsuarioID") + ", Email = @Email, CentroEstudios = @CentroEstudios, AreaEstudios = @AreaEstudios WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") ");
            #endregion

            #region DatoExtraEcosistemaOpcionSolicitud
            this.sqlDatoExtraEcosistemaOpcionSolicitudInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistemaOpcionSolicitud (DatoExtraID, OpcionID, SolicitudID) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + ", " + IBD.GuidParamColumnaTabla("OpcionID") + ", " + IBD.GuidParamColumnaTabla("SolicitudID") + ")");
            this.sqlDatoExtraEcosistemaOpcionSolicitudDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistemaOpcionSolicitud WHERE (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + " AND OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + "  AND SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + " )");
            this.sqlDatoExtraEcosistemaOpcionSolicitudModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistemaOpcionSolicitud SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", OpcionID = " + IBD.GuidParamColumnaTabla("OpcionID") + ", SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ")");
            #endregion

            #region DatoExtraEcosistemaVirtuosoSolicitud
            this.sqlDatoExtraEcosistemaVirtuosoSolicitudInsert = IBD.ReplaceParam("INSERT INTO DatoExtraEcosistemaVirtuosoSolicitud (DatoExtraID, SolicitudID, Opcion) VALUES (" + IBD.GuidParamColumnaTabla("DatoExtraID") + ", " + IBD.GuidParamColumnaTabla("SolicitudID") + ", @Opcion)");
            this.sqlDatoExtraEcosistemaVirtuosoSolicitudDelete = IBD.ReplaceParam("DELETE FROM DatoExtraEcosistemaVirtuosoSolicitud WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + " AND DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            this.sqlDatoExtraEcosistemaVirtuosoSolicitudModify = IBD.ReplaceParam("UPDATE DatoExtraEcosistemaVirtuosoSolicitud SET DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", Opcion = @Opcion WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ")  AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            #endregion

            #region DatoExtraProyectoOpcionSolicitud
            this.sqlDatoExtraProyectoOpcionSolicitudInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyectoOpcionSolicitud (OrganizacionID, ProyectoID, DatoExtraID, OpcionID, SolicitudID) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", " + IBD.GuidParamColumnaTabla("OpcionID") + ", " + IBD.GuidParamColumnaTabla("SolicitudID") + ")");
            this.sqlDatoExtraProyectoOpcionSolicitudDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyectoOpcionSolicitud WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + " AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ")");
            this.sqlDatoExtraProyectoOpcionSolicitudModify = IBD.ReplaceParam("UPDATE DatoExtraProyectoOpcionSolicitud SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", OpcionID = " + IBD.GuidParamColumnaTabla("OpcionID") + ", SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + " WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ")  AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ") AND (OpcionID = " + IBD.GuidParamColumnaTabla("Original_OpcionID") + ")");
            #endregion

            #region DatoExtraProyectoVirtuosoSolicitud
            this.sqlDatoExtraProyectoVirtuosoSolicitudInsert = IBD.ReplaceParam("INSERT INTO DatoExtraProyectoVirtuosoSolicitud (OrganizacionID, ProyectoID, DatoExtraID, SolicitudID, Opcion) VALUES (" + IBD.GuidParamColumnaTabla("OrganizacionID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", " + IBD.GuidParamColumnaTabla("SolicitudID") + ", @Opcion)");
            this.sqlDatoExtraProyectoVirtuosoSolicitudDelete = IBD.ReplaceParam("DELETE FROM DatoExtraProyectoVirtuosoSolicitud WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ")  AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            this.sqlDatoExtraProyectoVirtuosoSolicitudModify = IBD.ReplaceParam("UPDATE DatoExtraProyectoVirtuosoSolicitud SET OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", DatoExtraID = " + IBD.GuidParamColumnaTabla("DatoExtraID") + ", SolicitudID = " + IBD.GuidParamColumnaTabla("SolicitudID") + ", Opcion = @Opcion WHERE (SolicitudID = " + IBD.GuidParamColumnaTabla("Original_SolicitudID") + ")  AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("Original_OrganizacionID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (DatoExtraID = " + IBD.GuidParamColumnaTabla("Original_DatoExtraID") + ")");
            #endregion

            #endregion
        }

        public List<SolicitudUsuarioComunidadModel> ObtenerPerfilesSolicitudesAccesoProyectoPorProyecto(Guid pOrganizacionID, Guid pProyectoID, int paginaActual, int numResultados)
        {
            var lista = mEntityContext.Solicitud.JoinSolicitudUsuario().Join(mEntityContext.Perfil, solicitud=> solicitud.SolicitudUsuario.PerfilID, perfil => perfil.PerfilID , (solicitud, perfil) => new 
            {
                Solicitud = solicitud.Solicitud,
                SolicitudUsuario = solicitud.SolicitudUsuario,
                Perfil = perfil
            }
            ).Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID)).OrderBy(item => item.Solicitud.FechaSolicitud).Skip(paginaActual*numResultados).Take(numResultados).ToList().Select(item => new SolicitudUsuarioComunidadModel
            { 
                Nombre = item.Perfil.NombrePerfil,
                NombreCorteUsu = item.Perfil.NombreCortoUsu,
                PerfilID = item.SolicitudUsuario.PerfilID,
                PersonaID = item.SolicitudUsuario.PersonaID,
                SolicitudID = item.SolicitudUsuario.SolicitudID,
                UsuarioID = item.SolicitudUsuario.UsuarioID
            }).ToList();
            return lista;
        }

        public int ObtenerPerfilesSolicitudesAccesoProyectoPorProyectoCount(Guid pOrganizacionID, Guid pProyectoID)
        {
            var lista = mEntityContext.Solicitud.JoinSolicitudUsuario().Join(mEntityContext.Perfil, solicitud => solicitud.SolicitudUsuario.PerfilID, perfil => perfil.PerfilID, (solicitud, perfil) => new
            {
                Solicitud = solicitud.Solicitud,
                SolicitudUsuario = solicitud.SolicitudUsuario,
                Perfil = perfil
            }
            ).Where(item => item.Solicitud.Estado.Equals(0) && item.Solicitud.OrganizacionID.Equals(pOrganizacionID) && item.Solicitud.ProyectoID.Equals(pProyectoID)).Count();
            return lista;
        }

        #endregion

        #endregion
    }
}
