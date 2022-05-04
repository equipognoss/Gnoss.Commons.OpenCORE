using Es.Riam.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace Es.Riam.Util.Correo
{
    public class Email : IEmail
    {
        public Email() { }

        public int CorreoID { get; set; }
        public string Remitente { get; set; }
        public string MascaraRemitente { get; set; }
        public string Asunto { get; set; }
        public string HtmlTexto { get; set; }
        public bool EsHtml { get; set; }
        public DateTime FechaPuestaEnCola { get; set; }
        public short Prioridad { get; set; }
        public string DireccionRespuesta { get; set; }
        public string MascaraDireccionRespuesta { get; set; }
        public IServidorCorreo ServidorCorreo { get; set; }
        [JsonIgnore]
        public List<IDestinatarioEmail> Destinatarios { get; set; }
        public string Tipo { get; set; }
    }

    public class DestinatarioEmail : IDestinatarioEmail
    {
        public DestinatarioEmail() { }

        public int CorreoID { get; set; }
        public string Email { get; set; }
        public string MascaraDestinatario { get; set; }
        public short Estado { get; set; }
        public DateTime FechaProcesado { get; set; }
    }
}
