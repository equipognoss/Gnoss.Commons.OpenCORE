using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetOpenAuth.OAuth.ChannelElements;
using Es.Riam.Gnoss.OAuthAD.OAuth;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Clase para representar un token en Gnoss
    /// </summary>
    public class TokenGnoss : IServiceProviderRequestToken, IServiceProviderAccessToken
    {

        #region Miembros

        private OAuthToken mFilaToken;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="pFilaToken">Fila del token</param>
        public TokenGnoss(OAuthToken pFilaToken)
        {
            mFilaToken = pFilaToken;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene los permisos del consumidor con este token
        /// </summary>
        public string Permisos
        {
            get
            {
                return mFilaToken.Scope;
            }
        }

        /// <summary>
        /// Obtiene el estado del token
        /// </summary>
        public EstadosToken Estado
        {
            get
            {
                return (EstadosToken)mFilaToken.State;
            }
            set
            {
                mFilaToken.State = (int)value;
            }
        }

        /// <summary>
        /// Obtiene la fila del token
        /// </summary>
        public OAuthToken FilaToken
        {
            get
            {
                return mFilaToken;
            }
        }

        #endregion

        #region Miembros de IServiceProviderRequestToken

        /// <summary>
        /// Obitne o establece la Url de vuelta del consumidor
        /// </summary>
        public Uri Callback
        {
            get
            {
                if (!string.IsNullOrEmpty(mFilaToken.RequestTokenCallback))
                {
                    return new Uri(mFilaToken.RequestTokenCallback);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                mFilaToken.RequestTokenCallback = value.ToString();
            }
        }

        /// <summary>
        /// Obtiene la clave del consumidor del token
        /// </summary>
        public string ConsumerKey
        {
            get { return mFilaToken.OAuthConsumer.ConsumerKey; }
        }

        /// <summary>
        /// Obtiene la versión del consumidor
        /// </summary>
        public Version ConsumerVersion
        {
            get
            {
                return new Version(mFilaToken.ConsumerVersion);
            }
            set
            {

            }
        }

        /// <summary>
        /// Obtiene la fecha de creación del token
        /// </summary>
        public DateTime CreatedOn
        {
            get 
            { 
                return mFilaToken.IssueDate; 
            }
        }

        /// <summary>
        /// Obtiene el token
        /// </summary>
        public string Token
        {
            get 
            { 
                return mFilaToken.Token; 
            }
        }

        /// <summary>
        /// Obtiene o establece el código de verificación
        /// </summary>
        public string VerificationCode
        {
            get
            {
                return mFilaToken.RequestTokenVerifier;
            }
            set
            {
                mFilaToken.RequestTokenVerifier = value;
            }
        }

        #endregion

        #region Miembros de IServiceProviderAccessToken

        /// <summary>
        /// Obtiene la fecha de expiración del token
        /// </summary>
        public DateTime? ExpirationDate
        {
            get 
            { 
                return null; 
            }
        }

        /// <summary>
        /// Obtiene los roles permitidos para este token
        /// </summary>
        public string[] Roles
        {
            get
            {
                string[] roles = { "todoPermitido" };
                return roles;
            }
        }

        /// <summary>
        /// Obtiene el nombre de usuario del token
        /// </summary>
        public string Username
        {
            get 
            {
                return mFilaToken.Usuario.Login;
            }
        }

        #endregion
    }
}
