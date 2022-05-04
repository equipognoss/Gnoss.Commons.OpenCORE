using System;
using System.IO;

namespace Es.Riam.Gnoss.Traducciones
{
    public static class Error
    {
        public static void GuardarLogError(Exception ex, string pError)
        {
            DirectoryInfo logDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log"));
            if (!logDir.Exists)
            {
                logDir.Create();
            }

            string logFile = Path.Combine(logDir.FullName, "error_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log");

            using (StreamWriter sw = new StreamWriter(logFile, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(Environment.NewLine + "Fecha: " + DateTime.Now + Environment.NewLine + Environment.NewLine);
                // Escribo el error
                sw.WriteLine(pError);

                if(ex != null)
                {
                    sw.WriteLine(ex.Message);
                    sw.WriteLine(ex.StackTrace);
                }

                sw.WriteLine(Environment.NewLine + Environment.NewLine + "___________________________________________________________________________________________" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            }
        }
    }
}
