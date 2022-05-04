using System;
namespace Es.Riam.Interfaces
{
    public interface IDestinatarioEmail
    {
        int CorreoID { get; set; }
        string Email { get; set; }
        string MascaraDestinatario { get; set; }
        short Estado { get; set; }
        DateTime FechaProcesado { get; set; }
    }
}
