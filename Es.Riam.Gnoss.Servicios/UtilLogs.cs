using Es.Riam.Util.Correo;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Es.Riam.Gnoss.Servicios
{
    public class UtilLogs
    {
        /// <summary>
        /// Guarda un mensaje en el fichero de log
        /// </summary>
        /// <param name="pMensaje">Mensaje a guardar</param>
        /// <param name="pParametroExtra">Parametro opcional para personalizar el nombre</param>
        /// <param name="pRuta">Ruta del fichero</param>
        public static void GuardarLog(string pMensaje, string pParametroExtra, string pRuta)
        {
            if (pMensaje != string.Empty)
            {
                try
                {
                    string nombreFichero = pRuta + "_";
                    if (pParametroExtra.Length > 0)
                    {
                        nombreFichero += pParametroExtra + "_";
                    }
                    nombreFichero += DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    // File access and writing

                    FileStream logFile = null;

                    if (File.Exists(nombreFichero))
                    {
                        logFile = new FileStream(nombreFichero, FileMode.Append, FileAccess.Write);
                    }
                    else
                    {
                        logFile = new FileStream(nombreFichero, FileMode.Create, FileAccess.Write);
                    }
                    TextWriter logWriter = new StreamWriter(logFile, Encoding.UTF8);

                    // Log entry
                    CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.ToString());
                    string logEntry = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss", culture) + " " + pMensaje;

                    logWriter.WriteLine(logEntry);
                    logWriter.Close();
                    logFile.Close();
                }
                catch { }
            }
        }

        /// <summary>
        /// Guarda una excepcion y el innerexception si tiene
        /// </summary>
        /// <param name="pEx">Excepcion a guardar</param>
        public static void GuardarExcepcion(Exception pEx, string pFicheroConfiguracionBDBase, string pRuta)
        {
            GuardarLog(LogStatus.Error.ToString().ToUpper() + " (" + pFicheroConfiguracionBDBase + ") " + CrearEntradaRegistro(LogStatus.Error, pEx.Message), "", pRuta);

            //Recogemos el innerexception
            if (pEx.InnerException != null)
            {
                GuardarLog(LogStatus.Error.ToString().ToUpper() + " (" + pFicheroConfiguracionBDBase + ") InnerExceptionMessage: " + CrearEntradaRegistro(LogStatus.Error, pEx.InnerException.Message) + " InnerException StackTrace: " + pEx.InnerException.StackTrace, "", pRuta);
            }
        }

        /// <summary>
        /// Determina el texto de la entrada que tendrá una operación de envío de correo
        /// </summary>
        /// <param name="pStatus">Estado de la operación de envio</param>
        /// <param name="pDetalles">Detalles de la operación de envio</param>
        /// <returns>Texto incluyendo estado y detalles del envío</returns>
        public static string CrearEntradaRegistro(LogStatus pEstado, string pDetalles)
        {
            string entradaLog = string.Empty;

            switch (pEstado)
            {
                case LogStatus.Correcto:
                    entradaLog = "\r\n\t >> OK: ";
                    break;
                case LogStatus.Error:
                    entradaLog = "\r\n\t >> ALERT: ";
                    break;
                case LogStatus.NoEnviado:
                    entradaLog = "\r\n\t >> OK: ";
                    break;
            }
            return entradaLog + pDetalles;
        }

        /// <summary>
        /// Guarda el correo enviado en un fichero de texto
        /// </summary>
        /// <param name="correo">Correo enviado</param>
        public static void GuardarCorreo(Email correo, string pRuta)
        {
            try
            {
                string nombreFichero = pRuta + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                // File access and writing

                FileStream logFile = null;
                logFile = new FileStream(nombreFichero, FileMode.Append, FileAccess.Write);

                TextWriter logWriter = new StreamWriter(logFile, Encoding.UTF8);

                // Log entry
                CultureInfo culture = new CultureInfo(CultureInfo.CurrentCulture.ToString());
                JsonSerializer json = new JsonSerializer();
                string logEntry = DateTime.Now.ToString(@"yyyy-MM-dd HH:mm:ss", culture) + " ";
                logWriter.Write(logEntry);
                json.Serialize(logWriter, correo);
                logWriter.WriteLine();
                logWriter.Close();
                logFile.Close();

            }
            catch { }
        }
    }
}
