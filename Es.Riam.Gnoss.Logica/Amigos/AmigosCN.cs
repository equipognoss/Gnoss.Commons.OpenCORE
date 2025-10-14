using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.Amigos;
using Es.Riam.Gnoss.AD.Amigos.Model;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;

namespace Es.Riam.Gnoss.Logica.Amigos
{
    /// <summary>
    /// Lógica de AmigosCN
    /// </summary>
    public class AmigosCN : BaseCN, IDisposable
    {
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #region Constructores

        /// <summary>
        /// Constructor para AmigosCN
        /// </summary>
        public AmigosCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AmigosCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if (loggerFactory == null)
            {
                this.AmigosAD = new AmigosAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, null, null);
            }
            else
            {
                this.AmigosAD = new AmigosAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosAD>(), mLoggerFactory);
            }
        }

        /// <summary>
        /// Constructor a partir del fichero de configuración
        /// </summary>
        public AmigosCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AmigosCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.AmigosAD = new AmigosAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<AmigosAD>(), mLoggerFactory);
            mFicheroConfiguracionBD = pFicheroConfiguracionBD;
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Comprueba si pIdentidadAmigo es amigo de pIdentidad o de alguno de sus amigos
        /// </summary>
        /// <param name="pIdentidad">ID de la identidad</param>
        /// <param name="pIdentidadAmigo">ID del amigo a comprobar</param>
        /// <returns>True si es amigo, false en caso contrario</returns>
        public bool ComprobarIdentidadAmigosDeAmigos(Guid pIdentidad, Guid pIdentidadAmigo)
        {
            return this.AmigosAD.ComprobarIdentidadAmigosDeAmigos(pIdentidad, pIdentidadAmigo);
        }

        /// <summary>
        /// Comprueba dado la clave de un perfil, si la identidad en MYGNOSS asociada a dicho perfil tiene contactos 
        /// </summary>
        /// <param name="pPerfilID">Clave de un perfil</param>
        /// <returns>True si tiene contactos</returns>
        public bool TieneContactosIdentidadDeMyGnoss(Guid pPerfilID)
        {
            return AmigosAD.TieneContactosIdentidadDeMyGnoss(pPerfilID);
        }

        /// <summary>
        /// Devuelve si las dos identidades son amigos
        /// </summary>
        /// <param name="pIdentidad">Identidad del usuario</param>
        /// <param name="pAmigo">Identidad del amigo</param>
        /// <returns></returns>
        public bool EsAmigoDeIdentidad(Guid pIdentidad, Guid pAmigo)
        {
            return AmigosAD.EsAmigoDeIdentidad(pIdentidad, pAmigo);
        }
        public DataWrapperAmigos ObtenerAmigosDeIdentidad(Guid pIdentidadID)
        {
            return AmigosAD.ObtenerAmigosDeIdentidad(pIdentidadID);
        }
        public DataWrapperAmigos ObtenerAmigosOrganizacionConAccesoParaUsuario(Guid pIdentidadOrganizacion, Guid pIdentidadUsuario)
        {
            return AmigosAD.ObtenerAmigosOrganizacionConAccesoParaUsuario(pIdentidadOrganizacion, pIdentidadUsuario);
        }

        /// <summary>
        /// Obtiene los amigos de las organizaciones que el usuario administra pero no participa en ellas
        /// </summary>
        /// <param name="pUsuarioID">Identificador del usuario</param>
        /// <param name="pProyectoID">Proyecto al que deben pertenecer los amigos (null si se quieren obtener todos)</param>
        /// <returns></returns>
        public DataWrapperAmigos ObtenerAmigosDeOrganizacionesAdministradas(Guid pUsuarioID, Guid? pProyectoID)
        {
            return AmigosAD.ObtenerAmigosDeOrganizacionesAdministradas(pUsuarioID, pProyectoID);
        }

        /// <summary>
        /// Carga todas las listas del DataWrapper con los datos de tus amigos y de los que tu eres amigo
        /// </summary>
        /// <param name="pIdentidadID"></param>
        /// <returns></returns>
        public DataWrapperAmigos CargarAmigosCompleto(Guid pIdentidadID)
        {
            return AmigosAD.CargarAmigosCompleto(pIdentidadID);
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
        ~AmigosCN()
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
                    if (AmigosAD != null)
                        AmigosAD.Dispose();
                }
                AmigosAD = null;
            }
        }

        #endregion

        #region Propiedades

        private AmigosAD AmigosAD
        {
            get
            {
                return (AmigosAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        



        #endregion
    }
}
