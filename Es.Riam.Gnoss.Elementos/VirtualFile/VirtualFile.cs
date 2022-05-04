namespace Es.Riam.Gnoss.Elementos.VirtualFile
{
    public class VirtualFile
    {
        private string nombreFichero;
        private int tamanyoFichero;
        private bool isDirectory;

        public VirtualFile()
        {
            this.nombreFichero = "";
            this.tamanyoFichero = 0;
            this.isDirectory = false;
        }

        public VirtualFile(string nombreFichero, int tamanyoFichero, bool isDirectory)
        {
            this.nombreFichero = nombreFichero;
            this.tamanyoFichero = tamanyoFichero;
            this.isDirectory = isDirectory;
        }

        public string NombreFichero
        {
            get
            {
                return nombreFichero;
            }
            set
            {
                nombreFichero = value;
            }
        }

        public int TamanyoFichero
        {
            get
            {
                return tamanyoFichero;
            }
            set
            {
                tamanyoFichero = value;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return isDirectory;
            }
            set
            {
                isDirectory = value;
            }
        }
    }
}
