using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using Es.Riam.Gnoss.OAuthAD.OAuth;
using Microsoft.AspNetCore.Http;
using System;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{

    /// <summary>
    /// Clase para dar la descripción de un consummer
    /// </summary>
    public class Consumidor : IConsumerDescription
    {

        #region Miembros
        private static string mBaseURL = "http://oauth.gnoss.com";

        private OAuthConsumer mFilaConsumidor;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor de un consumidor
        /// </summary>
        /// <param name="pFilaConsumidor">Fila del consumidor</param>
        public Consumidor(OAuthConsumer pFilaConsumidor)
        {
            mFilaConsumidor = pFilaConsumidor;
        }

        #endregion

        #region Miembros de IConsumerDescription

        /// <summary>
        /// Obtiene la Url de vuelta del consumidor
        /// </summary>
        public Uri Callback
        {
            get 
            {
                return new Uri(mFilaConsumidor.Callback); 
            }
        }

        /// <summary>
        /// Obtiene el certificado de un consumidor
        /// </summary>
        public System.Security.Cryptography.X509Certificates.X509Certificate2 Certificate
        {
            get 
            { 
                return null; 
            }
        }

        /// <summary>
        /// Obtiene la clave de un consumidor
        /// </summary>
        public string Key
        {
            get 
            {
                return mFilaConsumidor.ConsumerKey; 
            }
        }

        /// <summary>
        /// Obtiene la clave secreta de un consumidor
        /// </summary>
        public string Secret
        {
            get 
            { 
                return mFilaConsumidor.ConsumerSecret; 
            }
        }

        /// <summary>
        /// Obtiene el tipo de formato para verificar las llamadas de un consumidor
        /// </summary>
        public DotNetOpenAuth.OAuth.VerificationCodeFormat VerificationCodeFormat
        {
            get
            {
                return (DotNetOpenAuth.OAuth.VerificationCodeFormat)mFilaConsumidor.VerificationCodeFormat;
            }
        }

        /// <summary>
        /// Longitud del código de verificación
        /// </summary>
        public int VerificationCodeLength
        {
            get { return mFilaConsumidor.VerificationCodeLength; }
        }

        #endregion

        #region Propiedades estáticas

        /// <summary>
        /// Url base de la aplicación
        /// </summary>
        public static string BaseURL
        {
            get
            {
                return mBaseURL;
            }
        }

        /// <summary>
        /// Auto descripción del proveedor (gnoss)
        /// </summary>
        public static ServiceProviderDescription AutoDescripcion(string pUrlRequest, string pUrlAccess, string pUrlAuthorize)
        {
            string urlBase = "http://oauth.gnoss.com";
            ServiceProviderDescription description = new ServiceProviderDescription
            {
                AccessTokenEndpoint = new MessageReceivingEndpoint(new Uri(string.Concat(urlBase, "/", pUrlAccess)), HttpDeliveryMethods.PostRequest),
                RequestTokenEndpoint = new MessageReceivingEndpoint(new Uri(string.Concat(urlBase, "/", pUrlRequest)), HttpDeliveryMethods.PostRequest),
                UserAuthorizationEndpoint = new MessageReceivingEndpoint(new Uri(string.Concat(urlBase, "/", pUrlAuthorize)), HttpDeliveryMethods.PostRequest),
                TamperProtectionElements = new ITamperProtectionChannelBindingElement[] {
					new HmacSha1SigningBindingElement(),
				},
            };

            return description;
        }

        #endregion

    }
}
