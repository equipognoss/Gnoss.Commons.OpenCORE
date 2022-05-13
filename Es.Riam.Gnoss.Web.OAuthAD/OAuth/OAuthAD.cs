using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.OAuthAD.OAuth.EncapsuladoDatos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.OAuth;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Es.Riam.Gnoss.OAuthAD.OAuth
{

    /// <summary>
    /// Describe los diferentes estados de un token
    /// </summary>
    public enum EstadosToken
    {
        /// <summary>
        /// Token no autorizado
        /// </summary>
        NoAutorizado = 0,

        /// <summary>
        /// Token autorizado
        /// </summary>
        Autorizado = 1,

        /// <summary>
        /// Token con acceso a la información privada del usuario
        /// </summary>
        ConAcceso = 2
    }

    /// <summary>
    /// Acceso a datos para OAuth
    /// </summary>
    public class OAuthAD : BaseAD
    {

        private EntityContextOauth mEntityContextOauth;

        /// <summary>
        /// Información acerca de la conexión a BD.
        /// </summary>
        public string InfoConexionBD
        {
            get
            {
                return mEntityContextOauth.Database.GetConnectionString();
            }
        }


        #region Constructores
        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public OAuthAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextOauth entityContextOauth, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextOauth = entityContextOauth;
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD"></param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public OAuthAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextOauth entityContextOauth, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mEntityContextOauth = entityContextOauth;
        }

        public void ActualizarBD()
        {
            mEntityContextOauth.SaveChanges();
        }

        #endregion

        #region Consultas
        private string sqlSelectUsuario;
        private string sqlSelectOAuthConsumer;
        private string sqlSelectOAuthToken;
        private string sqlSelectNonce;
        private string sqlSelectOAuthTokenExterno;
        private string sqlSelectPinToken;
        private string sqlSelectConsumerData;
        private string sqlSelectUsuarioConsumer;
        #endregion

        #region Metodos generales

        #region Metodos AD
        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {

            #region Consultas
            this.sqlSelectUsuario = "SELECT " + IBD.CargarGuid("Usuario.UsuarioID") + ", Login FROM Usuario";
            this.sqlSelectOAuthConsumer = "SELECT OAuthConsumer.ConsumerId, OAuthConsumer.ConsumerKey, OAuthConsumer.ConsumerSecret, OAuthConsumer.Callback, OAuthConsumer.VerificationCodeFormat, OAuthConsumer.VerificationCodeLength FROM OAuthConsumer";
            this.sqlSelectOAuthToken = "SELECT OAuthToken.TokenId, OAuthToken.Token, OAuthToken.TokenSecret, OAuthToken.State, OAuthToken.IssueDate, OAuthToken.ConsumerId, " + IBD.CargarGuid("OAuthToken.UsuarioID") + ", OAuthToken.Scope, OAuthToken.RequestTokenVerifier, OAuthToken.RequestTokenCallback, OAuthToken.ConsumerVersion FROM OAuthToken";
            this.sqlSelectNonce = "SELECT Context, Code, Timestamp FROM Nonce";
            this.sqlSelectOAuthTokenExterno = "SELECT OAuthTokenExterno.TokenId, OAuthTokenExterno.Token, OAuthTokenExterno.TokenSecret, OAuthTokenExterno.State, OAuthTokenExterno.IssueDate, OAuthTokenExterno.TokenVinculadoId, " + IBD.CargarGuid("OAuthTokenExterno.UsuarioID") + " FROM OAuthTokenExterno";
            this.sqlSelectPinToken = "SELECT PinToken.TokenId, " + IBD.CargarGuid("PinToken.UsuarioID") + ", PinToken.Pin FROM PinToken";
            this.sqlSelectConsumerData = "SELECT ConsumerData.ConsumerId, ConsumerData.Nombre, ConsumerData.Descripcion, ConsumerData.UrlOrigen, ConsumerData.FechaAlta FROM ConsumerData";
            this.sqlSelectUsuarioConsumer = "SELECT" + IBD.CargarGuid("UsuarioConsumer.UsuarioID") + ", UsuarioConsumer.ConsumerId, " + IBD.CargarGuid("UsuarioConsumer.ProyectoID") + "from UsuarioConsumer";

            #endregion

        }

        #endregion

        #region Metodos de consultas

        /// <summary>
        /// Obtiene un token externo por su token key
        /// </summary>
        /// <param name="pTokenKey">Token Key</param>
        /// <returns></returns>
        public List<OAuthTokenExterno> ObtenerTokenExternoPorTokenKey(string pTokenKey)
        {
            return mEntityContextOauth.OAuthTokenExterno.Where(oauthExterno => oauthExterno.Token.Equals(pTokenKey)).ToList();
        }

        /// <summary>
        /// Obtiene un token por ID
        /// </summary>
        /// <param name="pTokenID">ID del token</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerTokenPorID(int pTokenID)
        {
            DataWrapperOAuth resultado = new DataWrapperOAuth();

            resultado.OAuthToken = mEntityContextOauth.OAuthToken.Where(oAuthToken => oAuthToken.TokenId.Equals(pTokenID)).ToList();

            mEntityContextOauth.OAuthConsumer.Join(mEntityContextOauth.OAuthToken, oauthConsumer => oauthConsumer.ConsumerId, oauthToken => oauthToken.ConsumerId, (oauthConsumer, oauthToken) => new { OAuthConsumer = oauthConsumer, OAuthToken = oauthToken }).Where(item => item.OAuthToken.TokenId.Equals(pTokenID));

            return resultado;
        }

        /// <summary>
        /// Obtiene un token por su token key
        /// </summary>
        /// <param name="pTokenKey">Token Key</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerTokenPorTokenKey(string pTokenKey)
        {
            DataWrapperOAuth resultado = new DataWrapperOAuth();

            resultado.OAuthToken = mEntityContextOauth.OAuthToken.Where(oAuthToken => oAuthToken.Token.Equals(pTokenKey)).ToList();

            resultado.Usuario = mEntityContextOauth.OAuthToken.Join(mEntityContextOauth.Usuario, oAuthToken => oAuthToken.UsuarioID, usuario => usuario.UsuarioID, (oAuthToken, usuario) => new
            {
                OAuthToken = oAuthToken,
                Usuario = usuario
            }).Where(item => item.OAuthToken.Token.Equals(pTokenKey)).Select(item => item.Usuario).ToList();


            resultado.OAuthConsumer = mEntityContextOauth.OAuthToken.Join(mEntityContextOauth.OAuthConsumer, oAuthToken => oAuthToken.ConsumerId, oAuthConsumer => oAuthConsumer.ConsumerId, (oAuthToken, oAuthConsumer) => new { OAuthToken = oAuthToken, OAuthConsumer = oAuthConsumer }).Where(item => item.OAuthToken.Token.Equals(pTokenKey)).Select(item => item.OAuthConsumer).ToList();

            resultado.PinToken = mEntityContextOauth.OAuthToken.Join(mEntityContextOauth.PinToken, oauthToken => oauthToken.TokenId, pinToken => pinToken.TokenId, (oauthToken, pinToken) => new { OAuthToken = oauthToken, PinToken = pinToken }).Where(item => item.OAuthToken.Token.Equals(pTokenKey)).Select(item => item.PinToken).ToList();


            resultado.ConsumerData = mEntityContextOauth.OAuthToken.Join(mEntityContextOauth.ConsumerData, oauthToken => oauthToken.ConsumerId, consumerData => consumerData.ConsumerId, (oauthToken, consumerData) => new { OAuthToken = oauthToken, ConsumerData = consumerData }).Where(item => item.OAuthToken.Token.Equals(pTokenKey)).Select(item => item.ConsumerData).ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene un token por su token key
        /// </summary>
        /// <param name="pConsumerKey">Consumer Key</param>
        /// <returns></returns>
        public OAuthConsumer ObtenerConsumerPorConsumerKey(string pConsumerKey)
        {
            return mEntityContextOauth.OAuthConsumer.FirstOrDefault(oAuthConsumer => oAuthConsumer.ConsumerKey.Equals(pConsumerKey));
        }

        /// <summary>
        /// Obtiene un token por su ConsumeId
        /// </summary>
        /// <param name="pConsumerKey">Consumer Id</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerConsumerPorConsumerId(int pConsumerId)
        {
            DataWrapperOAuth resultado = new DataWrapperOAuth();

            resultado.OAuthConsumer = mEntityContextOauth.OAuthConsumer.Where(oAuthConsumer => oAuthConsumer.ConsumerId.Equals(pConsumerId)).ToList();

            resultado.ConsumerData = mEntityContextOauth.ConsumerData.Where(consumerData => consumerData.ConsumerId.Equals(pConsumerId)).ToList();

            return resultado;
        }

        /// <summary>
        /// Obtiene los tokens de un usuario y un consumer.
        /// </summary>
        /// <param name="pUsuarioID">ID del usaurio</param>
        /// <param name="pConsumerID">ID del consumer</param>
        /// <returns></returns>
        public DataWrapperOAuth ObtenerTokensPorUSuarioIDYConsumerID(Guid pUsuarioID, int pConsumerID)
        {
            DataWrapperOAuth resultado = new DataWrapperOAuth();

            resultado.OAuthToken = mEntityContextOauth.OAuthToken.Where(oAuthToken => oAuthToken.UsuarioID.Equals(pUsuarioID) && oAuthToken.ConsumerId.Equals(pConsumerID)).ToList();

            resultado.PinToken = mEntityContextOauth.OAuthToken.Join(mEntityContextOauth.PinToken, oauthToken => oauthToken.TokenId, pintoken => pintoken.TokenId, (oauthToken, pintoken) => new { OAuthToken = oauthToken, PinToken = pintoken }).Where(item => item.OAuthToken.ConsumerId.Equals(pConsumerID) && item.OAuthToken.UsuarioID.Equals(pUsuarioID)).Select(item => item.PinToken).ToList();

            return resultado;
        }


        /// <summary>
        /// Obtiene el ID de un usuario a partir de su login
        /// </summary>
        /// <param name="pLogin">Login del usuario</param>
        /// <returns>ID del usuario si existe, falso en caso contrario</returns>
        public Guid? ObtenerUsuarioIDPorLogin(string pLogin)
        {
            Guid resultado = mEntityContextOauth.Usuario.Where(usuario => usuario.Login.Equals(pLogin)).Select(item => item.UsuarioID).FirstOrDefault();

            if (resultado.Equals(Guid.Empty))
            {
                return null;
            }
            else
            {
                return resultado;
            }
        }

        /// <summary>
        /// Obtiene un usuario a partir de su ID de usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns>DataSet con el usuario a partir de su ID</returns>
        public Usuario ObtenerUsuarioPorUsuarioID(Guid pUsuarioID)
        {
            return mEntityContextOauth.Usuario.FirstOrDefault(usuario => usuario.UsuarioID.Equals(pUsuarioID));
        }

        /// <summary>
        /// Devuelve el access token de un usuario (NULL si no tiene)
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns></returns>
        public void EliminarAccessTokenUsuario(Guid pUsuarioID)
        {
            OAuthTokenExterno item = mEntityContextOauth.OAuthTokenExterno.FirstOrDefault(oAuthTokenExterno => oAuthTokenExterno.UsuarioID.Equals(pUsuarioID) && oAuthTokenExterno.State.Equals((int)EstadosToken.ConAcceso));

            if (item != null)
            {
                mEntityContextOauth.OAuthTokenExterno.Remove(item);
                //EntityContextOauth.Instance.Entry(item).State = System.Data.Entity.EntityState.Deleted;

                mEntityContextOauth.SaveChanges();
            }

        }

        /// <summary>
        /// Devuelve el access token de un usuario (NULL si no tiene)
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns></returns>
        public string ObtenerAccessTokenUsuario(Guid pUsuarioID)
        {
            return mEntityContextOauth.OAuthTokenExterno.Where(oAuthTokenExterno => oAuthTokenExterno.UsuarioID.Equals(pUsuarioID) && oAuthTokenExterno.State.Equals((int)EstadosToken.ConAcceso)).Select(item => item.Token).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene las aplicaciones de un usuario.
        /// </summary>
        /// <param name="pUsuarioID">ID del usaurio</param>
        /// <returns>IDataReader con las aplicaciones</returns>
        public List<ConsumerModel> ObtenerAplicacionesPorUsuarioIDyProyectoID(Guid pUsuarioID, Guid pProyectoID)
        {
            var consulta = mEntityContextOauth.ConsumerData.Join(mEntityContextOauth.OAuthConsumer, consumerdata => consumerdata.ConsumerId, oauthConsumer => oauthConsumer.ConsumerId, (consumerData, oauthConsumer) => new { ConsumerData = consumerData, OAuthConsumer = oauthConsumer }).Join(mEntityContextOauth.UsuarioConsumer, consumer => consumer.OAuthConsumer.ConsumerId, usuario => usuario.ConsumerId, (consumer, usuario) => new { OAuthConsumer = consumer.OAuthConsumer, ConsumerData = consumer.ConsumerData, Usuario = usuario }).Where(item => item.Usuario.UsuarioID.Equals(pUsuarioID) && item.Usuario.ProyectoID.Equals(pProyectoID)).Select(item => new ConsumerModel { AplicationCallback = item.OAuthConsumer.Callback, ConsumerId = item.OAuthConsumer.ConsumerId, Description = item.ConsumerData.Descripcion, Key = item.OAuthConsumer.ConsumerKey, Name = item.ConsumerData.Nombre, Secret = item.OAuthConsumer.ConsumerSecret });
            return consulta.ToList();
        }

        /// <summary>
        /// Obtiene la aplicación de un ConsumerId.
        /// </summary>
        /// <param name="pConsumerId">ConsumerId de la aplicación</param>
        /// <returns></returns>
        public ConsumerModel ObtenerAplicacionPorConsumerId(int pConsumerId)
        {
            return mEntityContextOauth.ConsumerData.Join(mEntityContextOauth.OAuthConsumer, consumerdata => consumerdata.ConsumerId, oauthConsumer => oauthConsumer.ConsumerId, (consumerdata, oauthConsumer) => new { ConsumerData = consumerdata, OAuthConsumer = oauthConsumer }).Where(item => item.ConsumerData.ConsumerId.Equals(pConsumerId)).Select(item => new ConsumerModel { AplicationCallback = item.OAuthConsumer.Callback, ConsumerId = item.OAuthConsumer.ConsumerId, Key = item.OAuthConsumer.ConsumerKey, Secret = item.OAuthConsumer.ConsumerSecret, Name = item.ConsumerData.Nombre, Description = item.ConsumerData.Descripcion }).FirstOrDefault();
        }

        /// <summary>
        /// Obtiene los tokens para una aplicación.
        /// </summary>
        /// <param name="pUsuarioId">UsuarioId del usuario actual</param>
        /// /// <param name="pConsumerId">ConsumerId de la aplicación</param>
        /// <returns>Un array de string con el token y el TokenSecret</returns>
        public OAuthToken ConsultarTokensPorUsuarioIDyConsumerId(Guid pUsuarioID, int pConsumerId)
        {
            return mEntityContextOauth.OAuthToken.Where(oAuthToken => oAuthToken.ConsumerId.Equals(pConsumerId) && oAuthToken.UsuarioID.HasValue && oAuthToken.UsuarioID.Value.Equals(pUsuarioID) && oAuthToken.State.Equals(2)).OrderByDescending(oauthToken => oauthToken.IssueDate).FirstOrDefault();

            #endregion
        }
        #endregion
    }
}