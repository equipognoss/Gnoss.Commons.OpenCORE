namespace Es.Riam.Util
{
    /// <summary>
    /// Utilidades de mail
    /// </summary>
    public class UtilMail
    {
        #region Mailto

        /// <summary>
        /// Manda un email atraves del shell de windows
        /// </summary>
        /// <param name="pDireccion">Direccion del destinatario del correo</param>
        public static void EnviarMailTo(string pDireccion)
        {
            EnviarMailTo(pDireccion, "", "", "", "");
        }

        /// <summary>
        /// Manda un email atraves del shell de windows
        /// </summary>
        /// <param name="pDireccion">Direccion del destinatario del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        public static void EnviarMailTo(string pDireccion, string pAsunto)
        {
            EnviarMailTo(pDireccion, pAsunto, "", "", "");
        }

        /// <summary>
        /// Manda un email atraves del shell de windows
        /// </summary>
        /// <param name="pDireccion">Direccion del destinatario del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Mensaje del correo.Para escribir en una otra linea utilizar--> %0A</param>
        public static void EnviarMailTo(string pDireccion, string pAsunto, string pCuerpo)
        {
            EnviarMailTo(pDireccion, pAsunto, pCuerpo, "", "");
        }

        /// <summary>
        /// Manda un email atraves del shell de windows
        /// </summary>
        /// <param name="pDireccion">Direccion del destinatario del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Mensaje del correo.Para escribir en una otra linea utilizar--> %0A</param>
        /// <param name="pCC">Copia</param>        
        public static void EnviarMailTo(string pDireccion, string pAsunto, string pCuerpo, string pCC)
        {
            EnviarMailTo(pDireccion, pAsunto, pCuerpo, pCC, "");
        }

        /// <summary>
        /// Manda un email atraves del shell de windows
        /// </summary>
        /// <param name="pDireccion">Direccion del destinatario del correo</param>
        /// <param name="pAsunto">Asunto del correo</param>
        /// <param name="pCuerpo">Mensaje del correo.Para escribir en una otra linea utilizar--> %0A</param>
        /// <param name="pCC">Copia</param>
        /// <param name="pBCC">Copia Oculta</param>
        public static void EnviarMailTo(string pDireccion, string pAsunto, string pCuerpo, string pCC, string pBCC)
        {
            if (pDireccion.Trim().Length > 0)
            {
                string email = "mailto:" + UtilCadenas.ConvertirTextoAUri(pDireccion);

                if (pAsunto.Trim().Length > 0)
                {
                    email += "?subject=" + UtilCadenas.ConvertirTextoAUri(pAsunto);
                }
                if (pCuerpo.Trim().Length > 0)
                {
                    email += "&body=" + UtilCadenas.ConvertirTextoAUri(pCuerpo);
                }
                if (pCC.Trim().Length > 0)
                {
                    email += "&cc=" + UtilCadenas.ConvertirTextoAUri(pCC);
                }
                if (pBCC.Trim().Length > 0)
                {
                    email += "&bcc=" + UtilCadenas.ConvertirTextoAUri(pBCC);
                }
                System.Diagnostics.Process.Start(email);
            }
        }
        #endregion
    }
}
