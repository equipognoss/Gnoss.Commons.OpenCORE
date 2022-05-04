using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetOpenAuth.OAuth.ChannelElements;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.AbstractsOpen;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Controlador de tokens para el consumidor
    /// </summary>
    public class ControladorTokensConsumidor : ControladorTokens
    {

        #region Miembros

        private Dictionary<string, OAuthTokenExterno> mListaTokensConsumidor = null;
        private Guid mUsuarioID;
        private string mConsumerKey;
        private string mConsumerSecret;
        private EntityContextOauth mEntityContextOauth;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        /// <param name="pConsumerKey">Consumer key</param>
        /// <param name="pConsumerSecret">Consumer secret</param>
        /// <param name="pUsuarioID">Identificador del usuario para el que se solicita acceso</param>
        public ControladorTokensConsumidor(string pConsumerKey, string pConsumerSecret, Guid pUsuarioID, EntityContextOauth entityContextOauth, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContextOauth, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            mConsumerKey = pConsumerKey;
            mConsumerSecret = pConsumerSecret;
            mUsuarioID = pUsuarioID;
            mEntityContextOauth = entityContextOauth;
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
        public override void ExpireRequestTokenAndStoreNewAccessToken(string pConsumerKey, string pRequestToken, string pAccessToken, string pAccessTokenSecret)
        {
            OAuthTokenExterno tokenViejo = ObtenerToken(pRequestToken);
            ListaTokensConsumidor.Remove(pRequestToken);

            OAuthTokenExterno tokenNuevo = CrearFilaToken(pAccessToken, pAccessTokenSecret);
            tokenNuevo.State = (int)EstadosToken.ConAcceso;

            //if (!tokenViejo.IsTokenVinculadoIdNull())
            if (tokenViejo.TokenVinculadoId.HasValue && tokenViejo.TokenVinculadoId.Value > 0)
            {
                //Tiene un token vinculado (autorización bidireccional)
                tokenNuevo.TokenVinculadoId = tokenViejo.TokenVinculadoId;

                OauthDW.Merge(OauthCN.ObtenerTokenPorID(tokenViejo.TokenVinculadoId.Value));
            }
            mEntityContextOauth.EliminarItem(tokenViejo);

            ActualizarBaseDeDatos();
        }

        /// <summary>
        /// Obtiene la parte secreta de un token
        /// </summary>
        /// <param name="pToken">parte pública del token</param>
        /// <returns></returns>
        public override string GetTokenSecret(string pToken)
        {
            return ObtenerToken(pToken).TokenSecret;
        }

        /// <summary>
        /// Obtiene el tipo de un token
        /// </summary>
        /// <param name="pToken">Token a comprobar</param>
        /// <returns></returns>
        public override TokenType GetTokenType(string pToken)
        {
            return ObtenerTipoToken((EstadosToken)ObtenerToken(pToken).State);
        }

        /// <summary>
        /// Almacena en la base de datos un request token nuevo
        /// </summary>
        /// <param name="pRequest">Request</param>
        /// <param name="pResponse">Response</param>
        public override void StoreNewRequestToken(DotNetOpenAuth.OAuth.Messages.UnauthorizedTokenRequest pRequest, DotNetOpenAuth.OAuth.Messages.ITokenSecretContainingMessage pResponse)
        {
            CrearFilaToken(pResponse.Token, pResponse.TokenSecret);
            ActualizarBaseDeDatos();
        }

        #endregion

        #region Miembros de IConsumerTokenManager

        /// <summary>
        /// Obtiene la clave pública del consumer
        /// </summary>
        public string ConsumerKey
        {
            get
            {
                return mConsumerKey;
                //return "sYFT1TPaSjmwSmuBwdmg";
            }
        }

        /// <summary>
        /// Obtiene la clave secreta del cosumer
        /// </summary>
        public string ConsumerSecret
        {
            get
            {
                return mConsumerSecret;
                //return "PDQFNo5ECmsYWNFTcfdHuYDyahIV7SVPgDlriMQb";
            }
        }

        #endregion

        #region Metodos generales

        /// <summary>
        /// Devuelve el access token de un usuario (NULL si no tiene)
        /// </summary>
        /// <param name="pUsuarioID">ID del usuario</param>
        /// <returns></returns>
        public string ObtenerAccessTokenUsuario(Guid pUsuarioID)
        {
            return OauthCN.ObtenerAccessTokenUsuario(pUsuarioID);
        }

        /// <summary>
        /// Crea una fila de token consumidor
        /// </summary>
        /// <param name="pToken"></param>
        /// <param name="pTokenSecret"></param>
        /// <returns></returns>
        private OAuthTokenExterno CrearFilaToken(string pToken, string pTokenSecret)
        {
            //OAuthDS.OAuthTokenExternoRow filaToken = OauthDW.OAuthTokenExterno.NewOAuthTokenExternoRow();
            OAuthTokenExterno tokenExterno = new OAuthTokenExterno();

            tokenExterno.Token = pToken;
            tokenExterno.TokenSecret = pTokenSecret;
            tokenExterno.State = (int)EstadosToken.NoAutorizado;
            tokenExterno.IssueDate = DateTime.Now;
            tokenExterno.UsuarioID = mUsuarioID;

            //OauthDW.OAuthTokenExterno.AddOAuthTokenExternoRow(filaToken);
            OauthDW.OAuthTokenExterno.Add(tokenExterno);

            if (!ListaTokensConsumidor.ContainsKey(pToken))
            {
                ListaTokensConsumidor.Add(pToken, tokenExterno);
            }

            return tokenExterno;
        }

        /// <summary>
        /// Obtine la fila de un token
        /// </summary>
        /// <param name="pToken">token a obtener</param>
        /// <returns></returns>
        public new OAuthTokenExterno ObtenerToken(string pToken)
        {
            return ObtenerToken(pToken, true);
        }

        /// <summary>
        /// Obtine la fila de un token
        /// </summary>
        /// <param name="pToken">token a obtener</param>
        /// <param name="pComprobarEnBD">Verdad si hay que comprobar que exista en la BD</param>
        /// <returns></returns>
        private OAuthTokenExterno ObtenerToken(string pToken, bool pComprobarEnBD)
        {
            OAuthTokenExterno token = null;
            if (ListaTokensConsumidor.ContainsKey(pToken))
            {
                token = ListaTokensConsumidor[pToken];
            }
            else
            {

                token = OauthDW.OAuthTokenExterno.FirstOrDefault(oauthTokenExterno => oauthTokenExterno.Token.Equals(pToken));

                if (token != null)
                {
                    ListaTokensConsumidor.Add(pToken, token);
                }
                else
                {
                    if (pComprobarEnBD)
                    {
                        OauthDW.OAuthTokenExterno.AddRange(OauthCN.ObtenerTokenExternoPorTokenKey(pToken));
                        return ObtenerToken(pToken, false);
                    }
                }
            }

            return token;
        }

        /// <summary>
        /// Elimina un token de la lista de tokens
        /// </summary>
        /// <param name="pToken"></param>
        protected override void EliminarTokenDeLista(string pToken)
        {
            if (ListaTokensConsumidor.ContainsKey(pToken))
            {
                ListaTokensConsumidor.Remove(pToken);
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene la lista de tokens de consumidor
        /// </summary>
        public Dictionary<string, OAuthTokenExterno> ListaTokensConsumidor
        {
            get
            {
                if (mListaTokensConsumidor == null)
                {
                    mListaTokensConsumidor = new Dictionary<string, OAuthTokenExterno>();

                    foreach (OAuthTokenExterno filaToken in OauthDW.OAuthTokenExterno)
                    {
                        mListaTokensConsumidor.Add(filaToken.Token, filaToken);
                    }
                }

                return mListaTokensConsumidor;
            }
        }

        #endregion
    }
}
