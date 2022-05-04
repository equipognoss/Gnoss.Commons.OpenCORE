using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;

namespace Es.Riam.Gnoss.Web.UtilOAuth
{
    /// <summary>
    /// A custom web app version of the message sent to request an unauthorized token.
    /// </summary>
    public class RequestScopedTokenMessage : UnauthorizedTokenRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestScopedTokenMessage"/> class.
        /// </summary>
        /// <param name="pEndpoint">The endpoint that will receive the message.</param>
        /// <param name="pVersion">The OAuth version.</param>
        public RequestScopedTokenMessage(MessageReceivingEndpoint pEndpoint, Version pVersion)
            : base(pEndpoint, pVersion)
        {
        }

        /// <summary>
        /// Gets or sets the scope of the access being requested.
        /// </summary>
        [MessagePart("scope", IsRequired = false)]
        public string Scope
        {
            get;
            set;
        }
    }

    /// <summary>
    /// A custom class that will cause the OAuth library to use our custom message types
    /// where we have them.
    /// </summary>
    public class CustomOAuthMessageFactory : OAuthServiceProviderMessageFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomOAuthMessageFactory"/> class.
        /// </summary>
        /// <param name="pTokenManager">The token manager instance to use.</param>
        public CustomOAuthMessageFactory(IServiceProviderTokenManager pTokenManager)
            : base(pTokenManager)
        {
        }

        /// <summary>
        /// Obtiene un nuevo request message
        /// </summary>
        /// <param name="pRecipient">Sujeto</param>
        /// <param name="pFields">Campos</param>
        /// <returns></returns>
        public override IDirectedProtocolMessage GetNewRequestMessage(MessageReceivingEndpoint pRecipient, IDictionary<string, string> pFields)
        {
            var message = base.GetNewRequestMessage(pRecipient, pFields);

            // inject our own type here to replace the standard one
            if (message is UnauthorizedTokenRequest)
            {
                message = new RequestScopedTokenMessage(pRecipient, message.Version);
            }

            return message;
        }
    }
}
