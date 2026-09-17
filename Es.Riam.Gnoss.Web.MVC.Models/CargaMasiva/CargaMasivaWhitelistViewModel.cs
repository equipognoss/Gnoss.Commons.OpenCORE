using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.CargaMasiva
{
    public enum AmbitoUrl
    {
        Comunidad,
        Ecosistema
    }

    public class CargaMasivaWhitelistViewModel
    {
        public List<UrlItem> UrlsComunidad { get; set; } = new List<UrlItem>();

        public List<UrlItem> UrlsHeredadoEcosistema { get; set; } = new List<UrlItem>();

        public bool EsAdministracionEcosistema { get; set; }
    }

    public class UrlItem
    {
        public string Url {  get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreCreador { get; set; }
        public AmbitoUrl Ambito { get; set; }
    }
}
