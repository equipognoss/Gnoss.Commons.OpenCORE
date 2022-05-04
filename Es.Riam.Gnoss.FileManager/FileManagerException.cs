using System;

namespace Es.Riam.Gnoss.FileManager
{
    public class FileManagerException : Exception
    {
        public FileManagerException(string pMessage, Exception pOriginalException) : base(pMessage, pOriginalException)
        {

        }
    }
}
