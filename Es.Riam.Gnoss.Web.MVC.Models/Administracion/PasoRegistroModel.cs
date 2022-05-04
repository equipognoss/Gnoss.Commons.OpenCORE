using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    [Serializable]
    public class PasoRegistroModel
    {
        public PasoRegistroModel()
        {
            NombrePasoRegistro = "";
            Obligatorio = false;
        }
        public PasoRegistroModel(string nombre, bool obligatorio)
        {
            NombrePasoRegistro = nombre;
            Obligatorio = obligatorio;
        }
        public string NombrePasoRegistro { get; set; }
        public bool Obligatorio { get; set; }
        public bool Nueva { get; set; }
        public bool Deleted { get; set; }
    }
}
