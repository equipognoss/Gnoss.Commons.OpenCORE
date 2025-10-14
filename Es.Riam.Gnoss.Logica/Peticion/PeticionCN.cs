using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.AD.Peticion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;

namespace Es.Riam.Gnoss.Logica.Peticion
{
    /// <summary>
    /// L�gica de peticiones
    /// </summary>
    public class PeticionCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor para PeticionCN
        /// </summary>
        public PeticionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PeticionCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.PeticionAD = new PeticionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuraci�n
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public PeticionCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<PeticionCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            PeticionAD = new PeticionAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<PeticionAD>(), mLoggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region M�todos

        /// <summary>
        /// Comprueba si la petici�n pasada por par�metro est� en espera
        /// </summary>
        /// <param name="pPeticionID">Identificador de la petici�n</param>
        /// <returns>TRUE si la petici�n existe y est� en estado de espera, FALSE en caso contrario</returns>
        public bool EstaPeticionEnEspera(Guid pPeticionID)
        {
            return PeticionAD.EstaPeticionEnEspera(pPeticionID);
        }

        /// <summary>
        /// Elimina todas las peticiones de un usuario pasado por par�metro
        /// </summary>
        /// <param name="pUsuarioID">Identificador de usuario</param>
        public void EliminarPeticionesDeUsuario(Guid pUsuarioID)
        {
            PeticionAD.EliminarPeticionesDeUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene los datos relativos a una petici�n de invitaci�n a participar en una comunidad
        /// </summary>
        /// <param name="pPeticionID">Identificador de la petici�n</param>
        /// <param name="pOrganizacionID">Identificador de la organizaci�n del proyecto</param>
        /// <param name="pProyectoID">Identificador del proyecto al que pertenece la invitaci�n</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionComunidadPorID(Guid pPeticionID, Guid pOrganizacionID, Guid pProyectoID)
        {
            return PeticionAD.ObtenerPeticionInvitacionComunidadPorID(pPeticionID, pOrganizacionID, pProyectoID);
        }

        /// <summary>
        /// Obtiene los datos relativos a una petici�n de invitaci�n a participar en una organizaci�n
        /// </summary>
        /// <param name="pPeticionID">Identificador de la petici�n</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionOrganizacionPorID(Guid pPeticionID)
        {
            return PeticionAD.ObtenerPeticionInvitacionOrganizacionPorID(pPeticionID);
        }

        /// <summary>
        /// Obtiene el proyecto de una invitaci�n a formar parte del mismo (S�LO si la petici�n es de invitacion a comunidad, si no NULL)
        /// </summary>
        /// <param name="pPeticionID">ID de la peticion</param>
        /// <returns>Identificador del proyecto de la petici�n (NULL si la petici�n )</returns>
        public Guid? ObtenerProyectoIDDePeticionPorID(Guid pPeticionID)
        {
            return PeticionAD.ObtenerProyectoIDDePeticionPorID(pPeticionID);
        }

        /// <summary>
        /// Obtiene todos los datos de una peticion por su ID
        /// </summary>
        /// <param name="pPeticionID">ID de la peticion</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionPorID(Guid pPeticionID)
        {
            return PeticionAD.ObtenerPeticionPorID(pPeticionID);
        }

        /// <summary>
        /// Obtiene si un proyecto tiene alguna invitaci�n realizada de Ning
        /// </summary>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <returns>true si tiene peticiones</returns>
        public bool ObtenerSiProyectoTieneInvitacionesDeNing(Guid pProyectoID)
        {
            return PeticionAD.ObtenerSiProyectoTieneInvitacionesDeNing(pProyectoID);
        }

        /// <summary>
        /// Obtiene las invitaciones a organizaci�n aceptadas por un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionesOrganizacionesAceptadasUsuario(Guid pUsuarioID)
        {
            return PeticionAD.ObtenerPeticionInvitacionesOrganizacionesAceptadasUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las invitaciones a comunidades aceptadas por un usuario
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionInvitacionesComunidadesAceptadasUsuario(Guid pUsuarioID)
        {
            return PeticionAD.ObtenerPeticionInvitacionesComunidadesAceptadasUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las peticiones de un usuario
        /// </summary>
        /// <param name="pUsuario">Identificador del usuario</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionPorUsuarioID(Guid pUsuario)
        {
            return PeticionAD.ObtenerPeticionPorUsuarioID(pUsuario);
        }

        /// <summary>
        /// Obtiene las peticiones de un determinado tipo de un usuario pasado por par�metro
        /// </summary>
        /// <param name="pUsuario">Identificador del usuario</param>
        /// <param name="pTipo">Tipo de petici�n que queremos obtener</param>
        /// <param name="pObtenerSoloSinProcesar">TRUE si s�lo se quieren obtener las peticiones que est�n sin procesar, FALSE en caso contrario</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionPorUsuarioIDyTipo(Guid pUsuario, TipoPeticion pTipo, bool pObtenerSoloSinProcesar)
        {
            return PeticionAD.ObtenerPeticionPorUsuarioIDyTipo(pUsuario, pTipo, pObtenerSoloSinProcesar);
        }

        /// <summary>
        /// Obtiene las peticiones de un determinado tipo de un usuario pasado por par�metro
        /// </summary>
        /// <param name="pUsuario">Identificador del usuario</param>
        /// <param name="pTipo">Tipo de petici�n que queremos obtener</param>
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionPorUsuarioIDyTipo(Guid pUsuario, TipoPeticion pTipo)
        {
            return PeticionAD.ObtenerPeticionPorUsuarioIDyTipo(pUsuario, pTipo, false);
        }

        /// <summary>
        /// Obtiene las peticiones de las nuevas comunidades sin aceptar
        /// </summary>        
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionComunidadesPendientesDeAceptar()
        {
            return PeticionAD.ObtenerPeticionComunidadesPendientesDeAceptar();
        }

        /// <summary>
        /// Obtiene el n�mero de las peticiones totales de las nuevas comunidades sin aceptar
        /// </summary>        
        /// <returns>Dataset de peticiones</returns>
        public int ObtenerPeticionComunidadesPendientesDeAceptarCount()
        {
            return PeticionAD.ObtenerPeticionComunidadesPendientesDeAceptarCount();
        }

        /// <summary>
        /// Obtiene las peticiones de las nuevas comunidades sin aceptar
        /// </summary>        
        /// <returns>Dataset de peticiones</returns>
        public DataWrapperPeticion ObtenerPeticionComunidadesPendientesDeAceptarPaginacion(int paginaActual, int numResultados)
        {
            return PeticionAD.ObtenerPeticionComunidadesPendientesDeAceptarPaginacion(paginaActual, numResultados);
        }

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pPeticionDS">Dataset de peticiones</param>
        public void ActualizarBD()
        {
            PeticionAD.ActualizarBD();
        }

        /// <summary>
        /// Comprueba si existe alguna petici�n de nueva comunidad con el mismo nombre que se pasa como par�metro
        /// </summary>
        /// <param name="pNombre">Nombre de comunidad</param>
        /// <returns>TRUE si hay alguna petici�n de comunidad con ese mismo nombre, FALSE en caso contrario</returns>
        public bool ExistePeticionProyectoMismoNombre(string pNombre)
        {
            return PeticionAD.ExistePeticionProyectoMismoNombre(pNombre);
        }

        /// <summary>
        /// Comprueba si existe alguna petici�n de nueva comunidad con el mismo nombre corto que se pasa como par�metro
        /// </summary>
        /// <param name="pNombre">Nombre corto de comunidad</param>
        /// <returns>TRUE si hay alguna petici�n de comunidad con ese mismo nombre corto, FALSE en caso contrario</returns>
        public bool ExistePeticionProyectoMismoNombreCorto(string pNombreCorto)
        {
            return PeticionAD.ExistePeticionProyectoMismoNombreCorto(pNombreCorto);
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~PeticionCN()
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
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he a�adido a esta clase
                    if (PeticionAD != null)
                    {
                        PeticionAD.Dispose();
                    }
                }
                PeticionAD = null;
            }
        }


        #endregion

        #region Propiedades

        /// <summary>
        /// DataAdapter para Peticiones
        /// </summary>
        private PeticionAD PeticionAD
        {
            get
            {
                return (PeticionAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion
    }
}