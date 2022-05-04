using System;
using System.Collections.Generic;

namespace Es.Riam.Interfaces
{
    public interface IEmail
    {
        int CorreoID { get; set; }
        string Remitente { get; set; }
        string MascaraRemitente { get; set; }
        string Asunto { get; set; }
        string HtmlTexto { get; set; }
        bool EsHtml { get; set; }
        DateTime FechaPuestaEnCola { get; set; }
        short Prioridad { get; set; }
        string DireccionRespuesta { get; set; }
        string MascaraDireccionRespuesta { get; set; }
        IServidorCorreo ServidorCorreo { get; set; }
        string Tipo { get; set; }

        List<IDestinatarioEmail> Destinatarios { get; set; }
    }
}
