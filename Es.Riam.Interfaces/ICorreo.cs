using System;

namespace Es.Riam.Interfaces
{
    public interface ICorreo
    {
        void EnviarCorreo(IEmail pCorreo, IDestinatarioEmail pDestinatario);
        void EnviarCorreo(string pDestinatario, string pRemitente, string pMascaraRemitente, string pAsunto, string pMensaje, bool pFormatoHTML, Guid pNotificacionID);
        void Dispose();
    }
}
