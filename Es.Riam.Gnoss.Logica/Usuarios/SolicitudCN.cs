using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Solicitud;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.Usuarios
{
    /// <summary>
    /// Lógica para solicitudes
    /// </summary>
    public class SolicitudCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public SolicitudCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<SolicitudCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            SolicitudAD = new SolicitudAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public SolicitudCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<SolicitudCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            SolicitudAD = new SolicitudAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<SolicitudAD>(), mLoggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Actualiza la base de datos
        /// </summary>
        /// <param name="pSolicitudDS">Dataset de solicitudes para actualizar</param>
        public void ActualizarBD()
        {
            SolicitudAD.ActualizarBD();
        }

        /// <summary>
        /// Devuelve el número de solicitudes a pertenecer en grupos del proyecto
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de solicitudes con las solicitudes a pertenecer en grupos del proyecto</returns>
        public int ObtenerNumeroSolicitudesPertenecerGruposDeProyecto(Guid pProyectoID)
        {
            return SolicitudAD.ObtenerNumeroSolicitudesPertenecerGruposDeProyecto(pProyectoID);
        }

        /// <summary>
        /// Devuelve el número de solicitudes de acceso pendientes a un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de solicitudes con las solicitudes de acceso a un proyecto</returns>
        public int ObtenerNumeroSolicitudesAccesoProyectoPorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return SolicitudAD.ObtenerNumeroSolicitudesAccesoProyectoPorProyecto(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Devuelve las solicitudes de acceso a un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de solicitudes con las solicitudes de acceso a un proyecto</returns>
        public DataWrapperSolicitud ObtenerSolicitudesAccesoProyectoPorProyecto(Guid pOrganizacionID, Guid pProyectoID)
        {
            return SolicitudAD.ObtenerSolicitudesAccesoProyectoPorProyecto(pOrganizacionID, pProyectoID);
        }
        /// <summary>
        /// Devuelve el modelo para la vista para administrar las solicitudes de una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de solicitudes con las solicitudes de acceso a un proyecto</returns>
        public List<SolicitudUsuarioComunidadModel> ObtenerModelUsuarioSolicitudesAccesoProyectoPorProyecto(Guid pOrganizacionID, Guid pProyectoID, int paginaActual, int numResultados)
        {
            return SolicitudAD.ObtenerPerfilesSolicitudesAccesoProyectoPorProyecto(pOrganizacionID, pProyectoID, paginaActual, numResultados);
        }

        /// <summary>
        /// Devuelve el modelo para la vista para administrar las solicitudes de una comunidad
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset de solicitudes con las solicitudes de acceso a un proyecto</returns>
        public int ObtenerModelUsuarioSolicitudesAccesoProyectoPorProyectoCount(Guid pOrganizacionID, Guid pProyectoID)
        {
            return SolicitudAD.ObtenerPerfilesSolicitudesAccesoProyectoPorProyectoCount(pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Devuelve la solicitud
        /// </summary>
        /// <param name="pSolicitudID">Identificador de la Solicitud</param>       
        /// <returns>Dataset de solicitudes con la solicitud</returns>
        public DataWrapperSolicitud ObtenerSolicitudPorID(Guid pSolicitudID)
        {
            return SolicitudAD.ObtenerSolicitudPorID(pSolicitudID);
        }

        /// <summary>
        /// Devuelve las solicitudes de acceso a un proyecto
        /// </summary>
        /// <returns>Dataset de solicitudes con las solicitudes de acceso a un proyecto</returns>
        public DataWrapperSolicitud ObtenerSolicitudesAccesoProyecto()
        {
            return SolicitudAD.ObtenerSolicitudesAccesoProyecto();
        }

        /// <summary>
        /// Devuelve las solicitudes de nuevos usuarios
        /// </summary>
        /// <returns>Dataset de solicitudes con las solicitudes de nuevos usuarios</returns>
        public DataWrapperSolicitud ObtenerSolicitudesNuevosUsuariosPorSolicitudID(Guid pSolicitudID)
        {
            return SolicitudAD.ObtenerSolicitudesNuevosUsuariosPorSolicitudID(pSolicitudID);
        }

        /// <summary>
        /// Devuelve la SolicitudNuevoUsuario del Id pedido
        /// </summary>
        /// <param name="pSolicitudID"></param> Id de la solicitud a obtener
        /// <returns></returns>
        public SolicitudNuevoUsuario ObtenerSolicitudNuevoUsuarioPorSolicitudID(Guid pSolicitudID)
        {
            return SolicitudAD.ObtenerSolicitudNuevoUsuarioPorSolicitudID(pSolicitudID);
        }

        /// <summary>
        /// Obtiene los datos extras de la solicitud
        /// </summary>
        /// <returns></returns>
        public DataWrapperSolicitud ObtenerSolicitudesDatoExtra()
        {
            return SolicitudAD.ObtenerSolicitudesDatoExtra();
        }

        /// <summary>
        /// Devuelve la Solicitud del Id pedido
        /// </summary>
        /// <param name="pSolicitudID"></param> Id de la solicitud a obtener
        /// <returns></returns>
        public Solicitud ObtenerSolicitudPorSolicitudID(Guid pSolicitudID)
        {
            return SolicitudAD.ObtenerSolicitudPorSolicitudID(pSolicitudID);
        }

        /// <summary>
        /// Devuelve el tipo de registro del usuario usuario
        /// </summary>
        /// <returns>Tipo de registro del usuario</returns>
        public TipoRegistro ObtenerTipoRegistroUsuario(Guid pUsuarioID)
        {
            return SolicitudAD.ObtenerTipoRegistroUsuario(pUsuarioID);
        }


        /// <summary>
        /// Obtiene la solicitud de acceso de un usuario a un proyecto (si existe)
        /// </summary>
        /// <param name="pSolicitudDW">Dataset de solicitudes</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        public void ObtenerSolicitudAProyectoDeUsuario(DataWrapperSolicitud pSolicitudDW, Guid pOrganizacionID, Guid pProyectoID, Guid pUsuarioID)
        {
            SolicitudAD.ObtenerSolicitudAProyectoDeUsuario(pSolicitudDW, pOrganizacionID, pProyectoID, pUsuarioID);
        }

        /// <summary>
        /// Obtiene TODAS las solicitudes que ha hecho un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>DataWrapper de solicitudes con las solicitudes de un usuario</returns>
        public DataWrapperSolicitud ObtenerSolicitudesDeUsuario(Guid pUsuarioID)
        {
            return SolicitudAD.ObtenerSolicitudesDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las solicitudes de todos los usuarios pendientes de aceptar o rechazados
        /// </summary>
        /// <returns>DataWrapper de solicitudes de usuarios</returns>
        public DataWrapperSolicitud ObtenerSolicitudesNuevosUsuarios()
        {
            return SolicitudAD.ObtenerSolicitudesNuevosUsuarios();
        }

        /// <summary>
        /// Obtiene el proyecto en el que se registro originalmente el usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Identificador del proyecto origen del usuario</returns>
        public Guid ObtenerProyectoOrigenUsuario(Guid pUsuarioID)
        {
            return SolicitudAD.ObtenerProyectoOrigenUsuario(pUsuarioID);
        }

        /// <summary>
        /// Acepta la solicitud de una identidad a participar en un grupo de proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public bool AceptarSolicitudDeIdentidadEnGrupoProyecto(Guid pIdentidadID, Guid pGrupoID)
        {
            return SolicitudAD.AceptarSolicitudDeIdentidadEnGrupoProyecto(pIdentidadID, pGrupoID);
        }

        /// <summary>
        /// Rechaza la solicitud de una identidad a participar en un grupo de proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public bool RechazarSolicitudDeIdentidadEnGrupoProyecto(Guid pIdentidadID, Guid pGrupoID)
        {
            return SolicitudAD.RechazarSolicitudDeIdentidadEnGrupoProyecto(pIdentidadID, pGrupoID);
        }

        /// <summary>
        /// Obtiene las solicitudes a participar en gurpos de un proyecto
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public DataWrapperSolicitud ObtenerSolicitudesGrupoPorProyecto(Guid pProyectoID)
        {
            return SolicitudAD.ObtenerSolicitudesGrupoPorProyecto(pProyectoID);
        }

        /// <summary>
        /// Obtiene una solicitud de una identidad a un grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public DataWrapperSolicitud ObtenerSolicitudDeIdentidadEnGrupo(Guid pIdentidadID, Guid pGrupoID)
        {
            return SolicitudAD.ObtenerSolicitudDeIdentidadEnGrupo(pIdentidadID, pGrupoID);
        }

        public void BorrarTokenAntiguos()
        {
            SolicitudAD.BorrarTokenAntiguos();
        }

        /// <summary>
        /// Comprueba si una identidad tiene una solicitud de acceso pendiente a un grupo
        /// </summary>
        /// <param name="pIdentidadID">Identificador de la Identidad</param>
        /// <param name="pGrupoID">Identificador del grupo</param>
        /// <returns></returns>
        public bool TieneSolicitudPendienteDeIdentidadEnGrupo(Guid pIdentidadID, Guid pGrupoID)
        {
            return SolicitudAD.TieneSolicitudPendienteDeIdentidadEnGrupo(pIdentidadID, pGrupoID);
        }

        /// <summary>
        /// Comprueba si un usuario tiene una solicitud de acceso pendiente a un proyecto con el perfil indicado
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <returns></returns>
        public bool TieneUsuarioSolicitudPendienteEnProyecto(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID, Guid pPerfilID)
        {
            return SolicitudAD.TieneUsuarioSolicitudPendienteEnProyecto(pUsuarioID, pOrganizacionID, pProyectoID, pPerfilID);
        }

        /// <summary>
        /// Comprueba si un usuario tiene una solicitud de acceso pendiente
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pOrganizacionID">Identificador de la organización</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public bool TieneUsuarioSolicitudPendienteEnProyecto(Guid pUsuarioID, Guid pOrganizacionID, Guid pProyectoID)
        {
            return SolicitudAD.TieneUsuarioSolicitudPendienteEnProyecto(pUsuarioID, pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Comprueba si un usuario tiene una solicitud de acceso pendiente a un proyecto
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de la organizacion</param>
        /// <param name="pOrganizacionProyectoID">Identificador de la organización del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns></returns>
        public bool TieneOrganizacionSolicitudPendienteEnProyecto(Guid pOrganizacionID, Guid pOrganizacionProyectoID, Guid pProyectoID)
        {
            return SolicitudAD.TieneOrganizacionSolicitudPendienteEnProyecto(pOrganizacionID, pOrganizacionProyectoID, pProyectoID);
        }

        public bool RechazarSolicitud(Guid solicitudID)
        {
            return SolicitudAD.RechazarSolicitud(solicitudID);
        }

        /// <summary>
        /// Comprueba si un usuario está asignado como administrador de una nueva organización que se desea registrar.
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario que puede ser administrador de una organización pendiente</param>
        /// <returns>TRUE si el usuario es admininstrador de una organización solicitada, FALSE en caso contrario</returns>
        public bool TieneUsuarioSolicitudOrganizacionNuevaEnlaQueEsAdmininstrador(Guid pUsuarioID)
        {
            return SolicitudAD.TieneUsuarioSolicitudOrganizacionNuevaEnlaQueEsAdmininstrador(pUsuarioID);
        }

        /// <summary>
        /// Guarda el token para un login del API del servicio de login.
        /// </summary>
        /// <param name="pToken">Token</param>
        /// <param name="pLogin">Login de usuario</param>
        public void GuardarTokenAccesoAPILogin(Guid pToken, string pLogin)
        {
            GuardarTokenAccesoAPILogin(pToken, pLogin, DateTime.UtcNow);
        }

        /// <summary>
        /// Guarda el token para un login del API del servicio de login.
        /// </summary>
        /// <param name="pToken">Token</param>
        /// <param name="pLogin">Login de usuario</param>
        public void GuardarTokenAccesoAPILogin(Guid pToken, string pLogin, DateTime pDateTime)
        {
            SolicitudAD.GuardarTokenAccesoAPILogin(pToken, pLogin, pDateTime);
        }

        /// <summary>
        /// Guarda el Token de Alta para un usuario del API del servicio de login.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <param name="pToken">Token de alta</param>
        /// <param name="pNombreCortoProyInicio">Nombre corto del proyecto de inicio</param>
        public void GuardarTokenAltaUsuarioAPILogin(Guid pUsuarioID, Guid pToken, string pNombreCortoProyInicio)
        {
            SolicitudAD.GuardarTokenAltaUsuarioAPILogin(pUsuarioID, pToken, pNombreCortoProyInicio);
        }

        /// <summary>
        /// Obtiene el Token de Alta de un usuario del API del servicio de login.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <returns>Token de alta del usuario</returns>
        public Guid ObtenerTokenAltaUsuarioAPILogin(Guid pUsuarioID)
        {
            return SolicitudAD.ObtenerTokenAltaUsuarioAPILogin(pUsuarioID);
        }

        /// <summary>
        /// Obtiene el proyecto de inicio de un usuario del API del servicio de login.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <returns>Proyecto de inicio si lo tiene, NULL si no</returns>
        public string ObtenerProyectoInicioUsuarioAPILogin(Guid pUsuarioID)
        {
            return SolicitudAD.ObtenerProyectoInicioUsuarioAPILogin(pUsuarioID);
        }

        /// <summary>
        /// Obtiene el login apartir de un token del API de Login.
        /// </summary>
        /// <param name="pToken">Token</param>
        /// <returns>Login apartir de un token del API de Login</returns>
        public string ObtenerLoginAPILoginDeToken(Guid pToken, bool pBorrarToken = false)
        {
            return SolicitudAD.ObtenerLoginAPILoginDeToken(pToken, pBorrarToken);
        }

        /// <summary>
        /// Guarda promoción y usuario de un nuevo registro.
        /// </summary>
        /// <param name="pUsuarioID">ID de usuario</param>
        /// <param name="pPromocionID">ID de la promoción</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        public void GuardarAltaPromocionUsuario(Guid pUsuarioID, Guid pPromocionID, Guid? pProyectoID)
        {
            SolicitudAD.GuardarAltaPromocionUsuario(pUsuarioID, pPromocionID, pProyectoID);
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
        ~SolicitudCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (SolicitudAD != null)
                        SolicitudAD.Dispose();
                }
                SolicitudAD = null;
            }
        }

        #endregion

        #region Propiedades

        private SolicitudAD SolicitudAD
        {
            get
            {
                return (SolicitudAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

    }
}
