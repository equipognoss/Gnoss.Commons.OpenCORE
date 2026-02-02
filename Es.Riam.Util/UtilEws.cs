using Es.Riam.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Es.Riam.Util
{
    public class UtilEws : ICorreo
    {
        #region Miembros

        /// <summary>
        /// Servidor EWS
        /// </summary>
        private ExchangeService mService;
        #endregion

        #region Constructores

        public UtilEws(string pUsuario, string pPasword, string pUrl)
        {
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
            TimeZoneInfo centralTZ = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
            mService = new ExchangeService(ExchangeVersion.Exchange2013_SP1, centralTZ);
            mService.Credentials = new WebCredentials(pUsuario, pPasword);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                mService.HttpHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{pUsuario}:{pPasword}")));
            }
            
            mService.Url = new Uri(pUrl);
        }

        #endregion

        #region Estaticos
        private static bool CertificateValidationCallBack(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Envía un correo electrónico mediante Microsoft Exchange WebServices 
        /// <param name="pCorreo">El correo a enviar</param>
        /// <param name="pDestinatario">El destinatario del correo</param>
        /// </summary>

        public void EnviarCorreo(IEmail pCorreo, IDestinatarioEmail pDestinatario)
        {
            try
            {
                EmailMessage email = new EmailMessage(mService);

                //Añadimos el mensaje
                if (pCorreo.EsHtml)
                {
                    pCorreo.HtmlTexto = "<html><body>" + pCorreo.HtmlTexto + "</body></html>";
                    email.Body = new MessageBody(BodyType.HTML, pCorreo.HtmlTexto);
                }
                else
                {
                    email.Body = new MessageBody(BodyType.Text, pCorreo.HtmlTexto);
                }

                if (!string.IsNullOrEmpty(pCorreo.DireccionRespuesta))
                {
                    email.ReplyTo.Add(pCorreo.DireccionRespuesta);
                }

                //Añadimos los destinatarios 

                email.ToRecipients.Add(new EmailAddress(pDestinatario.MascaraDestinatario, pDestinatario.Email));


                ExtendedPropertyDefinition messageIdHeader = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders, "Message-ID", MapiPropertyType.String);
                // Añadimos el ID personalizado al header
                email.SetExtendedProperty(messageIdHeader, "<" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + pCorreo.CorreoID + "." + pCorreo.Remitente + ">");

                email.Subject = pCorreo.Asunto;
                email.Sender = new EmailAddress(pCorreo.MascaraRemitente, pCorreo.Remitente);
                email.Send();

                pDestinatario.Estado = 1;
                pDestinatario.FechaProcesado = DateTime.Now;
            }
            catch
            {
                pDestinatario.Estado = 2;
                pDestinatario.FechaProcesado = DateTime.Now;
            }
        }

        public void EnviarCorreo(string pDestinatario, string pRemitente, string pMascaraRemitente, string pAsunto, string pMensaje, bool pFormatoHTML, Guid pNotificacionID)
        {
            try
            {
                Console.WriteLine($"Entra en EnviarCorreo");
                EmailMessage email = new EmailMessage(mService);
                Console.WriteLine($"mService: {mService.Url.AbsoluteUri}");
                //Añadimos el mensaje
                if (pFormatoHTML)
                {
                    pMensaje = "<html><body>" + pMensaje + "</body></html>";
                    email.Body = new MessageBody(BodyType.HTML, pMensaje);
                }
                else
                {
                    email.Body = new MessageBody(BodyType.Text, pMensaje);
                }
                Console.WriteLine($"Despues de generar el body del mensaje: {pMensaje}");

                //Añadimos los destinatarios 

                email.ToRecipients.Add(new EmailAddress(pDestinatario));
                Console.WriteLine($"Despues de anadir los destinatarios: {pDestinatario}");

                ExtendedPropertyDefinition messageIdHeader = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.InternetHeaders, "Message-ID", MapiPropertyType.String);
                // Añadimos el ID personalizado al header
                email.SetExtendedProperty(messageIdHeader, "<" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + pNotificacionID + "." + pRemitente + ">");
                Console.WriteLine($"Despues de anadir ID personalizado: {messageIdHeader}");
                email.Subject = pAsunto;
                Console.WriteLine($"Asunto: {pAsunto}");
                email.Sender = new EmailAddress(pMascaraRemitente, pRemitente);
                Console.WriteLine($"Mascara remitente: {pMascaraRemitente} ||| Remitente: {pRemitente}");
                Console.WriteLine($"Antes de enviar");
                email.Send();
                Console.WriteLine($"Despues de enviar");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EnvioCorreo: {ex.Message}");
                throw ex;
            }
        }

        public void Dispose()
        {
        }
    }
}
