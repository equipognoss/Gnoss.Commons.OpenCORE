using Es.Riam.Interfaces;

namespace Es.Riam.Util.Correo
{
    public class ServidorCorreo : IServidorCorreo
    {
        public ServidorCorreo() { }

        public ServidorCorreo(string pSMTP, string pUsuario, string pPassword, int pPuerto, bool pEsSeguro, bool? pSSL)
        {
            SMTP = pSMTP;
            Usuario = pUsuario;
            Password = pPassword;
            Puerto = pPuerto;
            EsSeguro = pEsSeguro;
        }

        public string SMTP { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public int Puerto { get; set; }
        public bool EsSeguro { get; set; }
    }
}
