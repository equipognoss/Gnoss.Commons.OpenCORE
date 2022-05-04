namespace Es.Riam.Gnoss.Web.RSS.RSS20
{
    /// <summary>
    /// Protocolos utilizados por RSSCanalCloud
    /// </summary>
    public enum RSSCanalCloudProtocol
    {
        Empty,
        XmlRpc,
        Soap,
        HttpPost
    }

    /// <summary>
    /// Is used to register processes with a cloud to be notified immediately of updates of the channel.
    /// </summary>
    public class RSSCanalCloud
    {
        private string mDomain;
        private int mPort;
        private string mPath;
        private string mRegisterProcedure;
        private RSSCanalCloudProtocol mProtocol;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pDomain">Nombre de dominio o IP del servicio</param>
        /// <param name="pPort">Puerto del servicio</param>
        /// <param name="pPath">Path del servicio</param>
        /// <param name="pRegisterProcedure">Procedimiento a llamar</param>
        /// <param name="pProtocol">Protocolo utilizado</param>
        public RSSCanalCloud(string pDomain,int pPort,string pPath,string pRegisterProcedure, RSSCanalCloudProtocol pProtocol)
        {
            mDomain = pDomain;
            mPort = pPort;
            mPath = pPath;
            mRegisterProcedure = pRegisterProcedure;
            mProtocol = pProtocol;
        }

        /// <summary>
        /// Obtiene el nombre de dominio o IP del servicio
        /// </summary>
        public string Domain
        {
            get { return mDomain; }
        }

        /// <summary>
        /// Obtiene el puerto del servicio
        /// </summary>
        public int Port
        {
            get { return mPort; }
        }

        /// <summary>
        /// Obtiene el Path del servicio
        /// </summary>
        public string Path
        {
            get { return mPath; }
        }

        /// <summary>
        /// Obtiene el nombre del procedimiento
        /// </summary>
        public string RegisterProcedure
        {
            get { return mRegisterProcedure; }
        }

        /// <summary>
        /// Obtiene el protocolo utilizado
        /// </summary>
        public RSSCanalCloudProtocol Protocol
        {
            get { return mProtocol; }
        }


    }
}
