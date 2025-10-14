using Es.Riam.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace Es.Riam.Util
{
    /// <summary>
    /// Utiles de correo
    /// </summary>
    [Serializable]
    public class UtilCorreo : ISerializable, ICorreo
    {

        #region Constantes

        /// <summary>
        /// Máscara genérica para los correos de GNOSS
        /// </summary>
        public static string MASCARA_GENERICA = "GNOSS";

        public static string DOMINIO_SERVIDOR_CLIENTE = "gnoss";

        #region Solicitudes

        /// <summary>
        /// Máscara para solicitudes
        /// </summary>
        public static string MASCARA_SOLICITUDES = "GNOSS";

        /// <summary>
        /// Máscara para las bienvenida de solicitudes
        /// </summary>
        public static string MASCARA_SOLICITUDES_BIENVENIDA = "GNOSS";

        #endregion

        #region Dafos

        /// <summary>
        /// Máscara para dafos
        /// </summary>
        public static string MASCARA_DAFOS = "Dafo GNOSS 2.0";

        #endregion

        #region Invitaciones

        /// <summary>
        /// Máscara para invitaciones
        /// </summary>
        public static string MASCARA_INVITACIONES = "Invitación GNOSS 2.0";

        #endregion

        #region Cambio contraseña

        /// <summary>
        /// Máscara para el cambio de contraseña
        /// </summary>
        public static string MASCARA_CAMBIOCONTRASEÑA = "Reestablecer contraseña GNOSS 2.0";

        #endregion

        #region Sugerencias

        /// <summary>
        /// Máscara para las sugerencias
        /// </summary>
        public static string MASCARA_SUGERENCIAS = "Sugerencias GNOSS 2.0";

        #endregion

        #region Boletín

        /// <summary>
        /// Máscara para los boletines
        /// </summary>
        public static string MASCARA_BOLETIN = "Boletín GNOSS 2.0";

        #endregion

        #region Personalizados

        /// <summary>
        /// Máscara para solicitudes de Corporate Excellence.
        /// </summary>
        public static string MASCARA_SOLICITUD_CORPORATE_EXCELLENCE = "Corporate Excellence";

        /// <summary>
        /// Máscara para IneveryCREA.
        /// </summary>
        public static string MASCARA_IneveryCREA = "Inevery CREA";

        #endregion

        #endregion

        #region Miembros

        /// <summary>
        /// Servidor SMTP
        /// </summary>
        private SmtpClientExtendido mServidor;

        /// <summary>
        /// Nombre del Servidor SMTP
        /// </summary>
        private string mServidorSmtp;

        /// <summary>
        /// Puerto del servidor SMTP
        /// </summary>
        private int mPuerto;

        /// <summary>
        /// Usuario del servidor SMTP
        /// </summary>
        private string mUsuario;

        /// <summary>
        /// Password del servidor SMTP
        /// </summary>
        private string mPassword;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor del gestor de correo
        /// </summary>
        /// <param name="pSmtp">Dirección del servidor</param>
        /// <param name="pPuerto">Puerto del servidor</param>
        /// <param name="pUsuario">Usuario del servidor</param>
        /// <param name="pPassword">Password del usuario</param>
        public UtilCorreo(string pSmtp, int pPuerto, string pUsuario, string pPassword, bool? pSSL)
        {
            mServidorSmtp = pSmtp;
            mPuerto = pPuerto;
            mUsuario = pUsuario;
            mPassword = pPassword;

            //Servidor SMTP
            mServidor = new SmtpClientExtendido(pSmtp, pPuerto);
            mServidor.UseDefaultCredentials = false;
            mServidor.Credentials = new NetworkCredential(pUsuario, pPassword);

            if (pSSL.HasValue && pSSL.Value)
            {
                mServidor.EnableSsl = true;
            }
            else
            {
                //Habilitamos el envio de mensajes seguros.
                if (pPuerto.Equals(465) || pPuerto.Equals(587))
                {
                    mServidor.EnableSsl = true;
                }
                else
                {
                    mServidor.EnableSsl = false;
                }
            }
        }

        /// <summary>
        /// Constructor para la deseralización
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        protected UtilCorreo(SerializationInfo info, StreamingContext context)
        {
            mServidorSmtp = info.GetString("Smtp");
            mPuerto = info.GetInt32("Puerto");
            mUsuario = info.GetString("Usuario");
            mPassword = info.GetString("Password");

            //Servidor SMTP
            mServidor = new SmtpClientExtendido(mServidorSmtp, mPuerto);
            mServidor.UseDefaultCredentials = false;
            mServidor.Credentials = new NetworkCredential(mUsuario, mPassword);
        }

        #endregion

        #region Métodos generales

        #region Públicos


        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        /// <param name="pDestinatario">Email del destinatario</param>
        /// <param name="pRemitente">Email del remitente</param>
        /// <param name="pMascaraRemitente">Máscara del remitente</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        /// <param name="pMensaje">Cuerpo del mensaje</param>
        /// <param name="pFormatoHTML">TRUE si el formato del mensaje es HTML</param>
        /// <param name="pNotificacionID">Identificador de la notificación que se está enviando</param>
        public void EnviarCorreo(string pDestinatario, string pRemitente, string pMascaraRemitente, string pAsunto, string pMensaje, bool pFormatoHTML, Guid pNotificacionID)
        {
            EnviarCorreo(pDestinatario, pRemitente, pMascaraRemitente, null, null, pAsunto, pMensaje, pFormatoHTML, pNotificacionID);
        }

        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        /// <param name="pDestinatario">Email del destinatario</param>
        /// <param name="pRemitente">Email del remitente</param>
        /// <param name="pMascaraRemitente">Máscara del remitente</param>
        /// <param name="pDireccionRespuesta">Dirección a la que se debe responder este mensaje</param>
        /// <param name="pMascaraDireccionRespuesta">Mascara de la dirección de respuesta</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        /// <param name="pMensaje">Cuerpo del mensaje</param>
        /// <param name="pFormatoHTML">TRUE si el formato del mensaje es HTML</param>
        /// <param name="pImagenEmbebida">Imagen embebida para la cabecera</param>
        /// <param name="pNotificacionID">Identificador de la notificación que se está enviando</param>
        public void EnviarCorreo(string pDestinatario, string pRemitente, string pMascaraRemitente, string pDireccionRespuesta, string pMascaraDireccionRespuesta, string pAsunto, string pMensaje, bool pFormatoHTML, Guid pNotificacionID)
        {
            if (!pDestinatario.Contains(',') && !pDestinatario.Contains(';'))
            {
                //El envío se realiza a una sola persona, lo envío desde el otro método para que se añada el campo To
                EnviarCorreoSoloUnaPersona(this.mServidorSmtp, this.mPuerto, this.mUsuario, this.mPassword, pDestinatario, pRemitente, pMascaraRemitente, "", "", pAsunto, pMensaje, pFormatoHTML, pNotificacionID, this.mServidor.EnableSsl);
            }
            else
            {
                // Creamos el mensaje
                MailMessage mensaje = new MailMessage();

                // Remitente
                mensaje.From = new MailAddress(pRemitente, pMascaraRemitente);

                // Destinatarios en copia oculta
                mensaje.Bcc.Add(pDestinatario);

                if (pFormatoHTML)
                {
                    pMensaje = "<html><body>" + pMensaje + "</body></html>";
                   CrearMensajeHTML(mensaje, pMensaje);
                }
                else
                {
                    mensaje.IsBodyHtml = false;
                    mensaje.Body = pMensaje;
                    //mensaje.BodyEncoding = System.Text.Encoding.UTF8;
                }

                if (!pDireccionRespuesta.Trim().Equals(string.Empty))
                {
                    mensaje.ReplyToList.Add(new MailAddress(pDireccionRespuesta, pMascaraDireccionRespuesta));
                }
                //Asunto
                mensaje.Subject = pAsunto;
                mensaje.SubjectEncoding = Encoding.UTF8;
                //mensaje.SubjectEncoding = CodificacionANSI;
                //mensaje.SubjectEncoding = null;
                mensaje.BodyEncoding = null;

                // No enviar un correo al remitente (generalmente ****@no-reply.com)
                //mensaje.To.Add(pRemitente);
                mensaje.Headers.Add("Message-ID", "<" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + pNotificacionID + "." + pRemitente + ">");

                if (!DOMINIO_SERVIDOR_CLIENTE.Equals("gnoss"))
                {
                    mServidor.LocalHostName = DOMINIO_SERVIDOR_CLIENTE;
                }

                try
                {
                    //Enviar el mensaje
                    mServidor.Send(mensaje);
                }
                catch (Exception ex)
                {
                    throw new GnossSmtpException($"Error al enviar al email {pDestinatario} el mensaje {pNotificacionID}: {pAsunto} con cuerpo {pMensaje}", ex);
                }

                // Limpieza
                mensaje.Dispose();
            }
        }

        /// <summary>
        /// Envía un correo electrónico
        /// <param name="pCorreo">El correo a enviar</param>
        /// <param name="pDestinatario">Destinatario del correo</param>
        /// </summary>
        public void EnviarCorreo(IEmail pCorreo, IDestinatarioEmail pDestinatario)
        {
            try
            {
                // Creamos el mensaje
                MailMessage mensaje = new MailMessage();

                // Remitente
                if (!string.IsNullOrEmpty(pCorreo.MascaraRemitente))
                {
                    mensaje.From = new MailAddress(pCorreo.Remitente, pCorreo.MascaraRemitente);
                }
                else
                {
                    mensaje.From = new MailAddress(pCorreo.Remitente);
                }

                if (pCorreo.EsHtml)
                {
                    pCorreo.HtmlTexto = "<html><body>" + pCorreo.HtmlTexto + "</body></html>";
                    CrearMensajeHTML(mensaje, pCorreo.HtmlTexto);
                }
                else
                {
                    mensaje.IsBodyHtml = false;
                    mensaje.Body = pCorreo.HtmlTexto;
                }

                if (!string.IsNullOrEmpty(pCorreo.DireccionRespuesta) && !string.IsNullOrEmpty(pCorreo.MascaraDireccionRespuesta))
                {
                    mensaje.ReplyToList.Add(new MailAddress(pCorreo.DireccionRespuesta, pCorreo.MascaraDireccionRespuesta));
                }
                else if (!string.IsNullOrEmpty(pCorreo.DireccionRespuesta))
                {
                    mensaje.ReplyToList.Add(new MailAddress(pCorreo.DireccionRespuesta));
                }
                //Asunto
                mensaje.Subject = pCorreo.Asunto;
                mensaje.SubjectEncoding = Encoding.UTF8;
                //mensaje.SubjectEncoding = CodificacionANSI;
                //mensaje.SubjectEncoding = null;
                mensaje.BodyEncoding = null;

                // Destinatario
                if (!string.IsNullOrEmpty(pCorreo.MascaraRemitente))
                {
                    mensaje.To.Add(new MailAddress(pDestinatario.Email, pDestinatario.MascaraDestinatario));
                }
                else
                {
                    mensaje.To.Add(new MailAddress(pDestinatario.Email));
                }

                mensaje.Headers.Add("Message-ID", "<" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + pCorreo.CorreoID + "." + pCorreo.Remitente + ">");

                //Habilitamos el envio de mensajes seguros.
                if (pCorreo.ServidorCorreo.EsSeguro)
                {
                    mServidor.EnableSsl = true;
                }
                else
                {
                    mServidor.EnableSsl = false;
                }

                if (!DOMINIO_SERVIDOR_CLIENTE.Equals("gnoss"))
                {
                    mServidor.LocalHostName = DOMINIO_SERVIDOR_CLIENTE;
                }

                try
                {
                    //Enviar el mensaje
                    mServidor.Send(mensaje);
                }
                catch (Exception ex)
                {
                    throw new GnossSmtpException($"Error al enviar al email {pDestinatario.Email} el mensaje {pCorreo.CorreoID}: {pCorreo.Asunto} con cuerpo {pCorreo.HtmlTexto}", ex);
                }

                pDestinatario.Estado = 1; //(short)EstadoEnvio.Enviado;
                pDestinatario.FechaProcesado = DateTime.Now;

                mensaje.Dispose();
            }
            catch (Exception ex)
            {
                pDestinatario.Estado = 2; //(short)EstadoEnvio.Error;
                pDestinatario.FechaProcesado = DateTime.Now;
                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// Obtiene la codificación ANSI
        /// </summary>
        public static Encoding CodificacionANSI
        {
            get
            {
                return Encoding.GetEncoding("iso8859-1");
            }
        }

        /// <summary>
        /// Envía un correo electrónico
        /// </summary>
        /// <param name="pSmtp">Dirección del servidor</param>
        /// <param name="pPuerto">Puerto del servidor</param>
        /// <param name="pUsuario">Usuario del servidor</param>
        /// <param name="pPassword">Password del usuario</param>
        /// <param name="pDestinatario">Email del destinatario</param>
        /// <param name="pRemitente">Email del remitente</param>
        /// <param name="pMascaraRemitente">Máscara del remitente</param>
        /// <param name="pDireccionRespuesta">Dirección a la que se debe responder este mensaje</param>
        /// <param name="pMascaraDireccionRespuesta">Máscara de la dirección de respuesta</param>
        /// <param name="pAsunto">Asunto del mensaje</param>
        /// <param name="pMensaje">Cuerpo del mensaje</param>
        /// <param name="pFormatoHTML">TRUE si el formato del mensaje es HTML</param>
        /// <param name="pNotificacionID">Identificador de la notificación que se está enviando</param>
        public static void EnviarCorreoSoloUnaPersona(string pSmtp, int pPuerto, string pUsuario, string pPassword, string pDestinatario, string pRemitente, string pMascaraRemitente, string pDireccionRespuesta, string pMascaraDireccionRespuesta, string pAsunto, string pMensaje, bool pFormatoHTML, Guid pNotificacionID, bool pSSL)
        {
            SmtpClientExtendido servidor = new SmtpClientExtendido(pSmtp, pPuerto);
            servidor.UseDefaultCredentials = false;
            servidor.Credentials = new NetworkCredential(pUsuario, pPassword);
            servidor.EnableSsl = pSSL;

			// Remitente
			MailAddress remitente = new MailAddress(pRemitente, pMascaraRemitente);

            // Destinatarios
            MailAddress destino = new MailAddress(pDestinatario);

            // Contenido del mensaje
            MailMessage mensaje = new MailMessage(remitente, destino);

            if (pFormatoHTML)
            {
                CrearMensajeHTML(mensaje, pMensaje);
            }
            else
            {
                mensaje.IsBodyHtml = false;
                mensaje.Body = pMensaje;
            }

            if (!pDireccionRespuesta.Trim().Equals(string.Empty))
            {
                mensaje.ReplyToList.Add(new MailAddress(pDireccionRespuesta, pMascaraDireccionRespuesta));
            }

            //Asunto
            mensaje.Subject = pAsunto;
            //mensaje.SubjectEncoding = CodificacionANSI;
            mensaje.SubjectEncoding = Encoding.UTF8;
            mensaje.Headers.Add("Message-ID", "<" + DateTime.Now.ToString("yyyyMMddHHmmss") + "." + pNotificacionID + "." + pRemitente + ">");

            if (!DOMINIO_SERVIDOR_CLIENTE.Equals("gnoss"))
            {
                servidor.LocalHostName = DOMINIO_SERVIDOR_CLIENTE;
            }

            try
            {
                servidor.Send(mensaje);
            }
            catch (Exception ex)
            {
                throw new GnossSmtpException($"Error al enviar al email {pDestinatario} el mensaje {pNotificacionID}: {pAsunto} con cuerpo {pMensaje}", ex);
            }

            mensaje.Dispose();
        }

        /// <summary>
        /// Envia un correo de invitación
        /// </summary>        
        /// <param name="pMensajeError">Mensaje de error</param>     
        /// <param name="pVersion">Versión de GNOSS</param>
        /// <param name="pRemitente">Email del remitente del mensaje</param>
        /// <param name="pDestinatario">Email del destinatario del mensaje</param>
        /// <param name="pSmtp">Dirección del servidor</param>
        /// <param name="pPuerto">Puerto del servidor</param>
        /// <param name="pUsuario">Usuario del servidor</param>
        /// <param name="pContraseña">Password del usuario</param>
        /// <returns>TRUE si se envía el correo correctamente</returns>
        public static bool EnviarCorreoError(string pMensajeError, string pVersion, string pRemitente, string pDestinatario, string pSmtp, int pPuerto, string pUsuario, string pContraseña)
        {
            bool envioCorrecto = true;

            MailMessage mensaje = null;

            try
            {
                if (pSmtp != null && pPuerto != -1 && pUsuario != null && pContraseña != null)
                {
                    if (!ComprobarServidorSmtp(pSmtp, pPuerto))
                    {
                        return false;
                    }
                    string mascaraRemitente = "Error GNOSS";
                    string asuntoMensaje = "Error GNOSS " + pVersion;
                    string cuerpoMensaje = pMensajeError;

                    MailAddress remitente = null;
                    MailAddress destino = null;

                    List<string> destinatarios = pDestinatario.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                    if (pRemitente != null && destinatarios != null && destinatarios.Count > 0)
                    {
                        // Remitente
                        remitente = new MailAddress(pRemitente, mascaraRemitente);

                        // Destinatarios
                        destino = new MailAddress(destinatarios[0]);
                    }
                    // Contenido del mensaje
                    mensaje = new MailMessage(remitente, destino);

                    if (destinatarios.Count > 1)
                    {
                        for (int i = 1; i < destinatarios.Count; i++)
                        {
                            mensaje.To.Add(destinatarios[i]);
                        }
                    }

                    mensaje.Body = pMensajeError;
                    mensaje.BodyEncoding = Encoding.UTF8;

                    //Asunto
                    mensaje.Subject = asuntoMensaje;
                    // Se quita para que el asunto no vaya codificado y se puedan filtrar los correos que llegan a la cuenta
                    // de errores. 
                    //mensaje.SubjectEncoding = System.Text.Encoding.UTF8;

                    //Enviar el mensaje
                    SmtpClientExtendido servidor = new SmtpClientExtendido(pSmtp, pPuerto);
                    servidor.UseDefaultCredentials = false;
                    servidor.Credentials = new NetworkCredential(pUsuario, pContraseña);

                    //Habilitamos el envio de mensajes seguros.
                    if (pPuerto.Equals(465) || pPuerto.Equals(587))
                    {
                        servidor.EnableSsl = true;
                    }
                    else
                    {
                        servidor.EnableSsl = false;
                    }

                    if (!DOMINIO_SERVIDOR_CLIENTE.Equals("gnoss"))
                    {
                        servidor.LocalHostName = DOMINIO_SERVIDOR_CLIENTE;
                    }

                    servidor.Send(mensaje);

                    envioCorrecto = true;
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                // Limpieza
                mensaje.Dispose();
            }
            return envioCorrecto;
        }

        /// <summary>
        /// Comprueba el servidor smtp
        /// </summary>
        /// <param name="pServidor">Servidor smtp</param>
        /// <param name="pPuerto">Puerto del servidor smtp</param>
        /// <returns>TRUE si se establece conexión con el servidor</returns>
        public static bool ComprobarServidorSmtp(string pServidor, int pPuerto)
        {
            string Data;
            byte[] szData;
            string CRLF = "\r\n";
            NetworkStream NetStrm;
            StreamReader RdStrm;

            //Comentamos el try, catch porque queremos recoger el error que se produce...
            //try
            //{
            //Inicializar el servidor
            TcpClient servidorSmtp = new TcpClient(pServidor, pPuerto);
            NetStrm = servidorSmtp.GetStream();
            RdStrm = new StreamReader(servidorSmtp.GetStream());

            // Decir hola al servidor
            Data = "HELLO " + DOMINIO_SERVIDOR_CLIENTE + CRLF;
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            NetStrm.Write(szData, 0, szData.Length);
            RdStrm.ReadLine();

            // quitar del servidor SMTP
            Data = "QUIT " + CRLF;
            szData = System.Text.Encoding.ASCII.GetBytes(Data.ToCharArray());
            NetStrm.Write(szData, 0, szData.Length);
            RdStrm.ReadLine();

            // cerrar conexión
            NetStrm.Close();
            RdStrm.Close();
            servidorSmtp.Dispose();

            return true;
            //}
            //catch (Exception)
            //{
            //    //return false;
            //    throw;
            //}
        }

        #endregion

        #region Privados

        /// <summary>
        /// Crea un mensaje en formato HTML
        /// </summary>
        /// <param name="pMail">Mail</param>
        /// <param name="pMensaje">Mensaje</param>
        private static void CrearMensajeHTML(MailMessage pMail, string pMensaje)
        {
            //Creamos la vista en texto plano del mensaje
            AlternateView plainView = AlternateView.CreateAlternateViewFromString(UtilCadenas.EliminarHtmlParaMensajes(pMensaje), Encoding.UTF8, "text/plain");
            pMail.AlternateViews.Add(plainView);

            //Creamos la vista HTML del mensaje
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(pMensaje, Encoding.UTF8, "text/html");

            pMail.AlternateViews.Add(htmlView);
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Liberamos recursos
        /// </summary>
        public void Dispose()
        {
            mServidor = null;
        }

        #endregion

        #region Miembros de ISerializable

        /// <summary>
        /// Metodo para serializar el objeto
        /// </summary>
        /// <param name="info">Datos serializados</param>
        /// <param name="context">Contexto de serialización</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Smtp", mServidorSmtp);
            info.AddValue("Puerto", mPuerto);
            info.AddValue("Usuario", mUsuario);
            info.AddValue("Password", mPassword);
        }

        #endregion
    }

    public class GnossSmtpException : Exception
    {
        public GnossSmtpException(string pMessage, Exception pInnerException) : base(pMessage, pInnerException)
        {

        }
    }
}
