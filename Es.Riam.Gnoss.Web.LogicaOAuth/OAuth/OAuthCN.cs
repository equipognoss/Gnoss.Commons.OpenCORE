using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Logica;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Es.Riam.Gnoss.OAuthAD.OAuth.EncapsuladoDatos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.OAuth;
using Es.Riam.Gnoss.Web.MVC.Models.ViewModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;

namespace Es.Riam.Gnoss.LogicaOAuth.OAuth
{
    /// <summary>
    /// Lógica de OAuthCN
    /// </summary>
    public class OAuthCN : BaseCN, IDisposable
    {
        #region Miembros

        //private OAuthAD.OAuth.OAuthAD mOAuthAD;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        #endregion

        #region Constructores

        /// <summary>
        /// Constructor para OAuthCN
        /// </summary>
        /// <param name="pFicheroConfiguracion">Fichero de configuración</param>
        public OAuthCN(string pFicheroConfiguracion, EntityContext entityContext, LoggingService loggingService, ConfigService configService, EntityContextOauth entityContextOauth, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<OAuthCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.OAuthAD = new OAuthAD.OAuth.OAuthAD(pFicheroConfiguracion, loggingService, entityContext, configService, entityContextOauth, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OAuthAD.OAuth.OAuthAD>(), mLoggerFactory);
        }

        /// <summary>
        /// Constructor para OAuthCN
        /// </summary>
        public OAuthCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, EntityContextOauth entityContextOauth, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<OAuthCN> logger, ILoggerFactory loggerFactory)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            this.OAuthAD = new OAuthAD.OAuth.OAuthAD("oauth", loggingService, entityContext, configService, entityContextOauth, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<OAuthAD.OAuth.OAuthAD>(), mLoggerFactory);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pOAuthDS">DataSet</param>
        public void ActualizarBD()
        {
            try
            {
                if (Transaction.Current != null)
                {
                    this.OAuthAD.ActualizarBD();
                }
                else
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        this.OAuthAD.ActualizarBD();
                        ts.Complete();
                    }
                }
            }
            catch (DBConcurrencyException)
            {
                // Error de concurrencia
                throw new Exception("Error de concurrencia");
            }
            catch (DataException)
            {
                //Error interno de la aplicación
                throw new Exception("Error interno de la aplicación");
            }
        }

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pOAuthDS">DataSet</param>
        /// <param name="pConsumerKeyCreado">ConsumerKey del consumidor recién creado</param>
        public void ActualizarBD(string pConsumerKeyCreado)
        {
            try
            {
                //TODO: Comprobar que el ID del ConsumerID se propaga bien
                if (Transaction.Current != null)
                {
                    this.OAuthAD.ActualizarBD();
                }
                else
                {
                    using (TransactionScope ts = new TransactionScope())
                    {
                        this.OAuthAD.ActualizarBD();
                        ts.Complete();
                    }
                }
            }
            catch (DBConcurrencyException)
            {
                // Error de concurrencia
                throw new Exception("Error de concurrencia");
            }
            catch (DataException)
            {
                //Error interno de la aplicación
                throw new Exception("Error interno de la aplicación");
            }
        }

        /// <summary>
        /// Obtiene un token externo por su token key
        /// </summary>
        /// <param name="pTokenKey">Token Key</param>
        /// <returns></returns>
        public List<OAuthTokenExterno> ObtenerTokenExternoPorTokenKey(string pTokenKey)
        {
            return OAuthAD.ObtenerTokenExternoPorTokenKey(pTokenKey);
        }

        /// <summary>
        /// Obtiene un token por ID
        /// </summary>
        /// <param name="pTokenID">ID del token</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerTokenPorID(int pTokenID)
        {
            return OAuthAD.ObtenerTokenPorID(pTokenID);
        }

        /// <summary>
        /// Obtiene los tokens de un usuario y un consumer.
        /// </summary>
        /// <param name="pUsuarioID">ID del usaurio</param>
        /// <param name="pConsumerID">ID del consumer</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerTokensPorUSuarioIDYConsumerID(Guid pUsuarioID, int pConsumerID)
        {
            return OAuthAD.ObtenerTokensPorUSuarioIDYConsumerID(pUsuarioID, pConsumerID);
        }

        /// <summary>
        /// Obtiene un token por su token key
        /// </summary>
        /// <param name="pTokenKey">Token Key</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerTokenPorTokenKey(string pTokenKey)
        {
            return OAuthAD.ObtenerTokenPorTokenKey(pTokenKey);
        }

        /// <summary>
        /// Obtiene un token por su token key
        /// </summary>
        /// <param name="pConsumerKey">Token Key</param>
        /// <returns></returns>
        public OAuthConsumer ObtenerConsumerPorConsumerKey(string pConsumerKey)
        {
            return OAuthAD.ObtenerConsumerPorConsumerKey(pConsumerKey);
        }

        /// <summary>
        /// Obtiene un token por su ConsumerId
        /// </summary>
        /// <param name="pConsumerKey">Consumer Id</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerConsumerPorConsumerId(int pConsumerId)
        {
            return OAuthAD.ObtenerConsumerPorConsumerId(pConsumerId);
        }

        /// <summary>
        /// Obtiene el ID de un usuario a partir de su login
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <returns>ID del usuario si existe, falso en caso contrario</returns>
        public Guid? ObtenerUsuarioIDPorLogin(string pLogin)
        {
            return OAuthAD.ObtenerUsuarioIDPorLogin(pLogin);
        }

        /// <summary>
        /// Obtiene un usuario a partir de su ID de usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns>DataSet con el usuario a partir de su ID</returns>
        public Usuario ObtenerUsuarioPorUsuarioID(Guid pUsuarioID)
        {
            return OAuthAD.ObtenerUsuarioPorUsuarioID(pUsuarioID);
        }

        /// <summary>
        /// Devuelve el access token de un usuario (NULL si no tiene)
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns></returns>
        public string ObtenerAccessTokenUsuario(Guid pUsuarioID)
        {
            return OAuthAD.ObtenerAccessTokenUsuario(pUsuarioID);
        }

        /// <summary>
        /// Devuelve el access token de un usuario (NULL si no tiene)
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns></returns>
        public void EliminarAccessTokenUsuario(Guid pUsuarioID)
        {
            OAuthAD.EliminarAccessTokenUsuario(pUsuarioID);
        }

        /// <summary>
        /// Obtiene las aplicaciones de un usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usaurio</param>
        /// <returns>AdministrarAplicacionesViewModel con las aplicaciones del usuario</returns>
        public AdministrarAplicacionesViewModel ObtenerAplicacionesPorUsuarioIDyProyectoID(Guid pUsuarioID, Guid pProyectoID)
        {
            List<ConsumerModel> listaConsumers = OAuthAD.ObtenerAplicacionesPorUsuarioIDyProyectoID(pUsuarioID, pProyectoID);

            AdministrarAplicacionesViewModel modeloAplicaciones = new AdministrarAplicacionesViewModel();
            modeloAplicaciones.Aplicaciones = listaConsumers;

            return modeloAplicaciones;
        }

        /// <summary>
        /// Obtiene la aplicación de un ConsumerId.
        /// </summary>
        /// <param name="pConsumerId">ConsumerId de la aplicación</param>
        /// <returns>ConsumerModel con los datos de la aplicación</returns>
        public ConsumerModel ObtenerAplicacionPorConsumerId(int pConsumerId)
        {
            return OAuthAD.ObtenerAplicacionPorConsumerId(pConsumerId);
        }

        /// <summary>
        /// Obtiene los tokens para una aplicación.
        /// </summary>
        /// <param name="pUsuarioId">UsuarioId del usuario actual</param>
        /// /// <param name="pConsumerId">ConsumerId de la aplicación</param>
        /// <returns>Un array de string con el token y el TokenSecret</returns>
        public OAuthToken ConsultarTokensPorUsuarioIDyConsumerId(Guid pUsuarioID, int pId)
        {
            return OAuthAD.ConsultarTokensPorUsuarioIDyConsumerId(pUsuarioID, pId);
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
        ~OAuthCN()
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
                //Libero todos los recursos administrados que he añadido a esta clase
                if (OAuthAD != null)
                {
                    OAuthAD.Dispose();
                }
                OAuthAD = null;
            }
        }


        #endregion

        #region Propiedades

        /// <summary>
        /// Información acerca de la conexión a BD.
        /// </summary>
        public string InfoConexionBD
        {
            get
            {
                return OAuthAD.InfoConexionBD;
            }
        }

        private OAuthAD.OAuth.OAuthAD OAuthAD
        {
            get
            {
                return (OAuthAD.OAuth.OAuthAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}
