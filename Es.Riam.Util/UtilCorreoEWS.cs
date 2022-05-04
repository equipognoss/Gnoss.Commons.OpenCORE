using Es.Riam.Interfaces;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Es.Riam.Util
{
    [Serializable]
    public class UtilCorreoEWS : ISerializable, ICorreo
    {

        #region Miembros

        /// <summary>
        /// Servidor SMTP
        /// </summary>
        private ExchangeService mServidor;

        /// <summary>
        /// Nombre del Servidor SMTP
        /// </summary>
        private string mServidorSmtp;

        /// <summary>
        /// Usuario del servidor SMTP
        /// </summary>
        private string mUsuario;

        /// <summary>
        /// Password del servidor SMTP
        /// </summary>
        private string mPassword;

        #endregion

        public UtilCorreoEWS(string pServidorEWS, string pEmail, string pPassword)
        {
            mServidor = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            mServidor.Credentials = new WebCredentials(pEmail, pPassword, pServidorEWS);
            mServidor.UseDefaultCredentials = true;
            mServidor.Url = new Uri(pServidorEWS);

            mServidorSmtp = pServidorEWS;
            mUsuario = pEmail;
            mPassword = pPassword;            
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected UtilCorreoEWS(SerializationInfo info, StreamingContext context)
        {
            mServidorSmtp = info.GetString("Smtp");
            mUsuario = info.GetString("Usuario");
            mPassword = info.GetString("Password");

            //Servidor EWS
            mServidor = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            mServidor.Credentials = new WebCredentials(mUsuario, mPassword);
            mServidor.UseDefaultCredentials = true;
            mServidor.AutodiscoverUrl(mUsuario);
        }


        public void EnviarCorreo(IEmail pCorreo, IDestinatarioEmail pDestinatario)
        {
            throw new NotImplementedException();
        }

        public void EnviarCorreo(string pDestinatario, string pRemitente, string pMascaraRemitente, string pAsunto, string pMensaje, bool pFormatoHTML, Guid pNotificacionID)
        {
            EmailMessage emailMessage = new EmailMessage(mServidor);
            if (!pDestinatario.Contains(",") && !pDestinatario.Contains(";"))
            {
                emailMessage.ToRecipients.Add(pDestinatario);
            }
            else
            {
                string[] destinatariosComa = pDestinatario.Split(",");
                if(destinatariosComa.Length > 0)
                {
                    foreach(string destinatario in destinatariosComa)
                    {
                        emailMessage.ToRecipients.Add(destinatario);
                    }
                }
                else
                {
                    string[] destinatariosPuntoComa = pDestinatario.Split(";");
                    foreach(string destinatario in destinatariosPuntoComa)
                    {
                        emailMessage.ToRecipients.Add(destinatario);
                    }
                }
            }

            
            emailMessage.Subject = pAsunto;
            if (pFormatoHTML)
            {
                emailMessage.Body = new MessageBody(BodyType.HTML ,pMensaje);
            }
            else
            {
                emailMessage.Body = new MessageBody(BodyType.Text, pMensaje);
            }

            emailMessage.Sender = pRemitente;
            emailMessage.Subject = pAsunto;

            try
            {
                emailMessage.Send();
            }
            catch (Exception ex)
            {
                throw new GnossSmtpException($"Error al enviar al email {pDestinatario} el mensaje {pNotificacionID}: {pAsunto} con cuerpo {pMensaje}", ex);
            }
        }



        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            mServidor = null;
        }

        /// <summary>
        /// Este método se utiliza para poder serializar el objeto
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Smtp", mServidorSmtp);
            info.AddValue("Usuario", mUsuario);
            info.AddValue("Password", mPassword);
        }
    }
}
