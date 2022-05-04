namespace Es.Riam.Gnoss.AD.EntityModel.Models.Carga
{
    public class EstadoCargaModel
    {
        public bool Cerrado { get; set; }
        public EstadoCarga EstadoCarga { get; set; }
        public int NumPaquetesPendientes { get; set; }
        public int NumPaquetesCorrectos { get; set; }
        public int NumPaquetesErroneos { get; set; }
    }
}
