using System;
using System.Net;

namespace Es.Riam.Util
{
    public class RiamWebClient : WebClient
    {
        /// <summary>
        /// Timeout en segundos
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Constructor para añadir Timeout a un WebClient
        /// </summary>
        /// <param name="pTimeout">Timeout en segundos</param>
        public RiamWebClient(int pTimeout)
        {
            Timeout = pTimeout;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest w = base.GetWebRequest(uri);
            if (Timeout > 0)
            {
                w.Timeout = Timeout * 1000;
            }
            return w;
        }
    }
}
