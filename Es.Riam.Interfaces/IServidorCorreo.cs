namespace Es.Riam.Interfaces
{
    public interface IServidorCorreo
    {
        string SMTP { get; set; }
        string Usuario { get; set; }
        string Password { get; set; }
        int Puerto { get; set; }
        bool EsSeguro { get; set; }
    }
}
