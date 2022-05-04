using DotNetOpenAuth.OAuth.ChannelElements;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.LogicaOAuth.OAuth;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Es.Riam.Gnoss.OAuthAD.OAuth.EncapsuladoDatos;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Gestor de tokens de Gnoss
    /// </summary>
    public class ControladorTokens : IServiceProviderTokenManager
    {

        #region Constantes

        /// <summary>
        /// Obtiene el nombre del parámetro token pendiente (usado en la autenticación bidireccional)
        /// </summary>
        public const string PARAMETRO_TOKEN_PENDIENTE = "pending_request_token";

        #endregion

        #region Miembros

        private DataWrapperOAuth mOAuthDW = null;
        private OAuthCN mOauthCN = null;
        private static ConcurrentDictionary<string, TokenGnoss> mListaTokens = null;
        //private static string mRutaFicheroConfiguracion = "";
        public static string RutaTrazas = null;

        private EntityContextOauth mEntityContextOauth;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private EntityContext mEntityContext;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        public ControladorTokens(EntityContextOauth entityContextOauth, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mOAuthDW = new DataWrapperOAuth();
            //mOAuthDW = new OAuthDS();

            mLoggingService = loggingService;
            mEntityContextOauth = entityContextOauth;
            mEntityContext = entityContext;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
        }

        #endregion

        #region Miembros de IServiceProviderTokenManager

        /// <summary>
        /// Obtiene un access token
        /// </summary>
        /// <param name="pToken">access token</param>
        /// <returns></returns>
        public IServiceProviderAccessToken GetAccessToken(string pToken)
        {
            try
            {
                TokenGnoss token = ObtenerToken(pToken);

                if (token.Estado.Equals(EstadosToken.ConAcceso))
                {
                    return token;
                }

                throw new KeyNotFoundException("Unrecognized token");
            }
            catch (InvalidOperationException ex)
            {
                throw new KeyNotFoundException("Unrecognized token", ex);
            }
        }

        /// <summary>
        /// Obtiene a un consumidor concreto a partir de su clave de consumidor
        /// </summary>
        /// <param name="pConsumerKey">clave del consumidor</param>
        /// <returns></returns>
        public IConsumerDescription GetConsumer(string pConsumerKey)
        {
            OAuthConsumer filaConsumidor = ObtenerFilaConsumer(pConsumerKey);

            if (filaConsumidor != null)
            {
                return new Consumidor(filaConsumidor);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Obtiene un request token
        /// </summary>
        /// <param name="pToken">token del request token que se quiere obtener</param>
        /// <returns></returns>
        public IServiceProviderRequestToken GetRequestToken(string pToken)
        {
            try
            {
                TokenGnoss token = ObtenerToken(pToken);
                return token;
            }
            catch (InvalidOperationException ex)
            {
                throw new KeyNotFoundException("Unrecognized token", ex);
            }
        }

        /// <summary>
        /// Comprueba si un request token concreto está autorizado
        /// </summary>
        /// <param name="pRequestToken">Request token a comprobar</param>
        /// <returns></returns>
        public bool IsRequestTokenAuthorized(string pRequestToken)
        {
            TokenGnoss token = ObtenerToken(pRequestToken);
            return !token.Estado.Equals(EstadosToken.NoAutorizado);
        }

        /// <summary>
        /// Actualiza en la base de datos un token concreto
        /// </summary>
        /// <param name="pToken">token actualizado</param>
        public void UpdateToken(IServiceProviderRequestToken pToken)
        {
            ActualizarBaseDeDatos();
        }

        #endregion

        #region Miembros de ITokenManager

        /// <summary>
        /// Elimina de la base de datos un request token y almacena un access token
        /// </summary>
        /// <param name="pConsumerKey">consumer key</param>
        /// <param name="pRequestToken">request token</param>
        /// <param name="pAccessToken">access token</param>
        /// <param name="pAccessTokenSecret">parte secreta del access token</param>
        public virtual void ExpireRequestTokenAndStoreNewAccessToken(string pConsumerKey, string pRequestToken, string pAccessToken, string pAccessTokenSecret)
        {
            TokenGnoss tokenViejo = ObtenerToken(pRequestToken);
            mListaTokens.TryRemove(pRequestToken, out tokenViejo);

            TokenGnoss tokenNuevo = CrearToken(pAccessToken, pAccessTokenSecret, pConsumerKey);

            tokenNuevo.Estado = EstadosToken.ConAcceso;
            tokenNuevo.FilaToken.ConsumerVersion = tokenViejo.FilaToken.ConsumerVersion;
            tokenNuevo.FilaToken.RequestTokenCallback = tokenViejo.FilaToken.RequestTokenCallback;
            tokenNuevo.FilaToken.RequestTokenVerifier = tokenViejo.FilaToken.RequestTokenVerifier;
            tokenNuevo.FilaToken.Scope = tokenViejo.FilaToken.Scope;
            tokenNuevo.FilaToken.UsuarioID = tokenViejo.FilaToken.UsuarioID;

            OauthDW.Merge(OauthCN.ObtenerTokensPorUSuarioIDYConsumerID(tokenViejo.FilaToken.UsuarioID.Value, tokenViejo.FilaToken.ConsumerId));
            //DataRow[] filasTok = new DataRow[OauthDW.OAuthToken.Count];

            //OauthDW.OAuthToken.CopyTo(filasTok, 0);
            //DataRow[] filasPinsTok = new DataRow[OauthDW.PinToken.Count];
            //OauthDW.PinToken.CopyTo(filasPinsTok, 0);

            //Borro las filas del PinToken asociadas a los Tokens:
            foreach (PinToken filaPinToken in OauthDW.PinToken)
            {
                mEntityContextOauth.EliminarItem(filaPinToken);
            }

            //Borro los Tokens anteriores:
            foreach (OAuthToken filaToken in OauthDW.OAuthToken)
            {
                if (filaToken.TokenId != tokenNuevo.FilaToken.TokenId)
                {
                    mEntityContextOauth.EliminarItem(filaToken);
                }
            }

            OauthCN.ActualizarBD();
        }

        /// <summary>
        /// Obtiene la parte secreta de un token
        /// </summary>
        /// <param name="pToken">parte pública del token</param>
        /// <returns></returns>
        public virtual string GetTokenSecret(string pToken)
        {
            return ObtenerToken(pToken).FilaToken.TokenSecret;
        }

        /// <summary>
        /// Obtiene el tipo de un token
        /// </summary>
        /// <param name="pToken">Token a comprobar</param>
        /// <returns></returns>
        public virtual TokenType GetTokenType(string pToken)
        {
            TokenGnoss token = ObtenerToken(pToken);
            return ObtenerTipoToken(token.Estado);
        }

        /// <summary>
        /// Almacena en la base de datos un request token nuevo
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <param name="pResponse">Response</param>
        public virtual void StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest pRequest, DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage pResponse)
        {
            TokenGnoss token = CrearToken(pResponse.Token, pResponse.TokenSecret, pRequest.ConsumerKey);

            if ((pRequest.ExtraData != null) && (pRequest.ExtraData.Count > 0) && (pRequest.ExtraData.ContainsKey(PARAMETRO_TOKEN_PENDIENTE)))
            {
                //Tiene un token vinculado (autorización bidireccional)
                string tokenPendiente = pRequest.ExtraData[PARAMETRO_TOKEN_PENDIENTE];

                OauthDW.OAuthTokenExterno.AddRange(OauthCN.ObtenerTokenExternoPorTokenKey(tokenPendiente));
                //((OAuthTokenExterno)OauthDW.OAuthTokenExterno.Select("Token = '" + tokenPendiente + "'")[0]).TokenVinculadoId = token.FilaToken.TokenId;
                OauthDW.OAuthTokenExterno.Where(oauthTokenExterno => oauthTokenExterno.Token.Equals(tokenPendiente)).FirstOrDefault().TokenVinculadoId = token.FilaToken.TokenId;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene el CN de OAuth
        /// </summary>
        public OAuthCN OauthCN
        {
            get
            {
                if (mOauthCN == null)
                {
                    mOauthCN = new OAuthCN("oauth", mEntityContext, mLoggingService, mConfigService, mEntityContextOauth, mServicesUtilVirtuosoAndReplication);
                }

                return mOauthCN;
            }
        }

        /// <summary>
        /// Obtiene el DS de OAuth
        /// </summary>
        public DataWrapperOAuth OauthDW
        {
            get
            {
                return mOAuthDW;
            }
        }

        /// <summary>
        /// Obtiene la lista de tokens cargados
        /// </summary>
        public ConcurrentDictionary<string, TokenGnoss> ListaTokens
        {
            get
            {
                if (mListaTokens == null)
                {
                    mListaTokens = new ConcurrentDictionary<string, TokenGnoss>();
                }
                return mListaTokens;
            }
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Actualiza la Base de datos
        /// </summary>
        public void ActualizarBaseDeDatos()
        {
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();
            try
            {
                OauthCN.ActualizarBD();
                mLoggingService.AgregarEntradaDependencia($"Actualizar base de datos en BD {OauthCN.InfoConexionBD}", false, "ControladorTokens.ActualizarBaseDeDatos", sw, true);
            }
            catch (Exception ex)
            {
                mLoggingService.AgregarEntradaDependencia($"Error al actualizar base de datos en BD {OauthCN.InfoConexionBD}. Error: {ex.Message}", false, "ControladorTokens.ActualizarBaseDeDatos", sw, true);
                throw;
            }

            List<OAuthTokenExterno> filaTokenExterno = mOAuthDW.OAuthTokenExterno.Where(oauthTokenExterno => oauthTokenExterno.TokenId < 0).ToList();
            //DataRow[] filas = mOAuthDW.OAuthTokenExterno.Select("TokenId < 0");
            foreach (OAuthTokenExterno fila in filaTokenExterno)
            {
                EliminarTokenDeLista(fila.Token);
                mEntityContextOauth.EliminarItem(fila);
            }


            List<OAuthToken> filaToken = mOAuthDW.OAuthToken.Where(oauthToken => oauthToken.TokenId < 0).ToList();
            //filas = mOAuthDW.OAuthToken.Select("TokenId < 0");
            foreach (OAuthToken fila in filaToken)
            {
                mEntityContextOauth.EliminarItem(filaToken);
                EliminarTokenDeLista(fila.Token);
            }

            //mOAuthDW.AcceptChanges();

            mLoggingService.AgregarEntrada("Fin ControladorTokens.cs.ActualizarBaseDeDatos()");
        }

        /// <summary>
        /// Elimina un token de la lista de tokens
        /// </summary>
        /// <param name="pToken"></param>
        protected virtual void EliminarTokenDeLista(string pToken)
        {
            if (ListaTokens.ContainsKey(pToken))
            {
                TokenGnoss tokenGnoss;
                ListaTokens.TryRemove(pToken, out tokenGnoss);
            }
        }

        /// <summary>
        /// Obtiene el tipo de un token
        /// </summary>
        /// <param name="pEstado">Estado del token</param>
        /// <returns></returns>
        protected TokenType ObtenerTipoToken(EstadosToken pEstado)
        {
            switch (pEstado)
            {
                case EstadosToken.NoAutorizado:
                    return TokenType.RequestToken;
                case EstadosToken.Autorizado:
                    return TokenType.RequestToken;
                case EstadosToken.ConAcceso:
                    return TokenType.AccessToken;
                default:
                    return TokenType.InvalidToken;
            }
        }

        /// <summary>
        /// Obtiene el ID de un usuario
        /// </summary>
        /// <param name="pLoginUsuario"></param>
        /// <returns></returns>
        private Guid ObtenerUsuarioID(string pLoginUsuario)
        {
            Stopwatch sw = LoggingService.IniciarRelojTelemetria();
            Guid? idUsuario = OauthCN.ObtenerUsuarioIDPorLogin(pLoginUsuario);
            mLoggingService.AgregarEntradaDependencia($"ObtenerUsuarioID en BD {OauthCN.InfoConexionBD}", false, "ControladorTokens.ObtenerUsuarioID", sw, true);

            if (idUsuario.HasValue)
            {
                mLoggingService.AgregarEntrada("Fin ControladorTokens.cs.ObtenerUsuarioID() con idUsuario=" + idUsuario.Value);
            }

            return idUsuario.Value;

            throw new Exception("User error");
        }

        /// <summary>
        /// Obtine la fila de un token
        /// </summary>
        /// <param name="pToken">token a obtener</param>
        /// <returns></returns>
        public TokenGnoss ObtenerToken(string pToken)
        {
            return ObtenerToken(pToken, true);
        }

        /// <summary>
        /// Obtine la fila de un token
        /// </summary>
        /// <param name="pToken">token a obtener</param>
        /// <param name="pComprobarEnBD">Verdad si hay que comprobar que exista en la BD</param>
        /// <returns></returns>
        private TokenGnoss ObtenerToken(string pToken, bool pComprobarEnBD)
        {
            mLoggingService.AgregarEntrada("Inicio ControladorTokens.cs.ObtenerToken()");

            if (ListaTokens.ContainsKey(pToken))
            {
                mLoggingService.AgregarEntrada("ControladorTokens.cs.ObtenerToken() Token está en lista");
                return ListaTokens[pToken];
            }
            else
            {
                mLoggingService.AgregarEntrada("ControladorTokens.cs.ObtenerToken() Miramos si Token está en DataSet");
                
                OAuthToken filaToken = OauthDW.OAuthToken.FirstOrDefault(oauthToken => oauthToken.Token.Equals(pToken));
                

                //OAuthToken[] filas = (OAuthToken[])mOAuthDW.OAuthToken.Select("Token = '" + pToken + "'");

                if (filaToken != null) 
                {
                    TokenGnoss token = new TokenGnoss(filaToken);
                    ListaTokens.TryAdd(pToken, token);
                    mLoggingService.AgregarEntrada("ControladorTokens.cs.ObtenerToken() Token está en DataSet");
                    return token;
                }
                else
                {
                    if (pComprobarEnBD)
                    {
                        Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                        mOAuthDW.Merge(OauthCN.ObtenerTokenPorTokenKey(pToken));
                        mLoggingService.AgregarEntradaDependencia($"ObtenerToken en BD {OauthCN.InfoConexionBD}", false, "ControladorTokens.ObtenerToken", sw, true);
                        return ObtenerToken(pToken, false);
                    }

                    mLoggingService.AgregarEntrada("ControladorTokens.cs.ObtenerToken() Token NO está en NINGUN LADO");
                    return null;
                }
            }
        }

        /// <summary>
        /// Obtine la fila de un consumer
        /// </summary>
        /// <param name="pConsumerKey">Consumer key</param>
        /// <returns></returns>
        private OAuthConsumer ObtenerFilaConsumer(string pConsumerKey)
        {
            return ObtenerFilaConsumer(pConsumerKey, true);
        }

        /// <summary>
        /// Obtine la fila de un token
        /// </summary>
        /// <param name="pConsumerKey">Consumer key</param>
        /// <param name="pComprobarEnBD">Verdad si hay que comprobar que exista en la BD</param>
        /// <returns></returns>
        private OAuthConsumer ObtenerFilaConsumer(string pConsumerKey, bool pComprobarEnBD)
        {
            mLoggingService.AgregarEntrada("Inicio ControladorTokens.cs.ObtenerFilaConsumer()");
            OAuthConsumer filaConsumer = mOAuthDW.OAuthConsumer.FirstOrDefault(oauthConsumer => oauthConsumer.ConsumerKey.Equals(pConsumerKey));
            //OAuthConsumer[] filas = (OAuthConsumer[])mOAuthDW.OAuthConsumer.Select("ConsumerKey = '" + pConsumerKey + "'");

            if (filaConsumer != null) 
            {
                mLoggingService.AgregarEntrada("ControladorTokens.cs.ObtenerFilaConsumer() ConsumerKey está en DataSet");
                return filaConsumer;
            }
            else
            {
                if (pComprobarEnBD)
                {
                    Stopwatch sw = LoggingService.IniciarRelojTelemetria();

                    OAuthConsumer oauthConsumer = OauthCN.ObtenerConsumerPorConsumerKey(pConsumerKey);
                    if (oauthConsumer != null)
                    {
                        mOAuthDW.OAuthConsumer.Add(oauthConsumer);
                        mLoggingService.AgregarEntradaDependencia($"ObtenerFilaConsumer en BD {OauthCN.InfoConexionBD}", false, "ControladorTokens.ObtenerToken", sw, true);
                        return ObtenerFilaConsumer(pConsumerKey, false);
                    }
                }

                mLoggingService.AgregarEntrada("ControladorTokens.cs.ObtenerFilaConsumer() CosumerKey NO está en NINGUN LADO");
                return null;
            }
        }

        /// <summary>
        /// Crea un token nuevo
        /// </summary>
        /// <param name="pToken">Token</param>
        /// <param name="pTokenSecret">Parte secreta del token</param>
        /// <param name="pConsumerKey">Clave del consumidor</param>
        /// <returns></returns>
        private TokenGnoss CrearToken(string pToken, string pTokenSecret, string pConsumerKey)
        {
            mLoggingService.AgregarEntrada("Inicio ControladorTokens.cs.CrearToken()");

            OAuthConsumer filaConsumer = ObtenerFilaConsumer(pConsumerKey);

            OAuthToken filaToken = new OAuthToken();
            //mOAuthDW.OAuthToken.NewOAuthTokenRow();

            filaToken.Token = pToken;
            filaToken.TokenSecret = pTokenSecret;
            filaToken.ConsumerId = filaConsumer.ConsumerId;
            filaToken.State = (int)EstadosToken.NoAutorizado; 
            filaToken.IssueDate = DateTime.Now;
            filaToken.ConsumerVersion = "1.0.1";

            mOAuthDW.OAuthToken.Add(filaToken);

            TokenGnoss tokenGnoss = new TokenGnoss(filaToken);
            ListaTokens.TryAdd(pToken, tokenGnoss);

            mLoggingService.AgregarEntrada("Fin ControladorTokens.cs.CrearToken() con token=" + tokenGnoss);

            return tokenGnoss;
        }

        /// <summary>
        /// Autoriza un request token sin autorización
        /// </summary>
        /// <param name="pRequestToken">request token sin autorización</param>
        /// <param name="pLoginUsuario">Login del usuario que hace la petición</param>
        public void AutorizarRequestToken(string pRequestToken, string pLoginUsuario)
        {
            if (pLoginUsuario == null)
            {
                mLoggingService.AgregarEntrada("ControladorTokens.cs.AutorizarRequestToken() Error no hay user");
                throw new ArgumentNullException("user");
            }

            AutorizarRequestToken(pRequestToken, ObtenerUsuarioID(pLoginUsuario));
        }

        /// <summary>
        /// Autoriza un request token sin autorización
        /// </summary>
        /// <param name="pRequestToken">request token sin autorización</param>
        /// <param name="pUsuarioID">Usuario que hace la petición</param>
        public void AutorizarRequestToken(string pRequestToken, Guid pUsuarioID)
        {
            mLoggingService.AgregarEntrada("Inicio ControladorTokens.cs.AutorizarRequestToken()");

            if (pRequestToken == null)
            {
                mLoggingService.AgregarEntrada("ControladorTokens.cs.AutorizarRequestToken() Error no hay requestToken");
                throw new ArgumentNullException("requestToken");
            }
            if (pUsuarioID == null)
            {
                mLoggingService.AgregarEntrada("ControladorTokens.cs.AutorizarRequestToken() Error no hay user");
                throw new ArgumentNullException("user");
            }

            TokenGnoss token = ObtenerToken(pRequestToken);
            //token.FilaToken.UsuarioID = ObtenerUsuarioID(pUser);
            token.FilaToken.UsuarioID = pUsuarioID;
            token.Estado = EstadosToken.Autorizado;

            mLoggingService.AgregarEntrada("Fin ControladorTokens.cs.AutorizarRequestToken() con token autorizado '" + token.Token + "' para usuario=" + token.FilaToken.UsuarioID);
        }

        /// <summary>
        /// Comprueba que el usuario que se ha logueado existe en la base de datos OAUTH
        /// </summary>
        public void ComprobarUsuarioEnOAuthBD(string pUsuario, string pUsuarioID)
        {
            mLoggingService.AgregarEntrada("Inicio ControladorTokens.cs.ComprobarUsuarioEnOAuthBD()");

            string usuario = pUsuario;
            if (OauthCN.ObtenerUsuarioIDPorLogin(usuario) == null)
            {
                Stopwatch sw = LoggingService.IniciarRelojTelemetria();
                mEntityContextOauth.Usuario.Add(new Usuario() { UsuarioID = new Guid(pUsuario), Login = pUsuario });

                OauthCN.ActualizarBD();
                mLoggingService.AgregarEntradaDependencia($"ComprobarUsuarioEnOAuthBD en BD {OauthCN.InfoConexionBD}", false, "ControladorTokens.ObtenerToken", sw, true);
            }

            mLoggingService.AgregarEntrada("Fin ControladorTokens.cs.ComprobarUsuarioEnOAuthBD()");
        }

        #endregion


    }
}

