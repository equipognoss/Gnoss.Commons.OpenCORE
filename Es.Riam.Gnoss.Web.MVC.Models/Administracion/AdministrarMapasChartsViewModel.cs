using System.Collections.Generic;


namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarMapaViewModel
    {
        public string Latidud { get; set; }
        public string Longitud { get; set; }
        public string Ruta { get; set; }
        public string ColorRuta { get; set; }
    }

    public class AdministrarChartsViewModel
    {
        public List<ChartViewModel> ListaCharts { get; set; }
        public List<string> Idiomas { get; set; }
        public string IdiomaUsuario { get; set; }
        public AdministrarChartsViewModel()
        {
            Idiomas = new List<string>();
            ListaCharts = new List<ChartViewModel>();
        }
    }

    public class ChartViewModel
    {
        public string Nombre { get; set; }
        public string Select { get; set; }
        public string Where { get; set; }
        public string Javascript { get; set; }
        public string FuncionJS { get; set; }
        public string ChartID { get; set; }
        public short Orden { get; set; }
        public bool Eliminada { get; set; }
    }

}
