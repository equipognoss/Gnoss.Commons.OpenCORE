namespace Es.Riam.Gnoss.Web.MVC.Models.Administracion
{
    public class AdministrarFusionCambiosViewModel
    {
        public string ContenidoCambio { get; set; }
        public string ContenidoBD { get; set; }
        public string Error { get; set; }
        public string EntornoCambio { get; set; }
        public bool EsImagen { get; set; }
        public AdministrarFusionCambiosViewModel()
        {
            EsImagen = false;
        }
        public TabModel TabModelPruebas { get; set; }
        public TabModel TabModelPro { get; set; }
        public FacetaModel FacetaModelPruebas { get; set; }
        public FacetaModel FacetaModelPro { get; set; }
        public ContextoModel ContextoModelPruebas { get; set; }
        public ContextoModel ContextoModelPro { get; set; }
        public CMSAdminComponenteEditarViewModel ComponenteCMSModelPruebas { get; set; }
        public CMSAdminComponenteEditarViewModel ComponenteCMSModelPro { get; set; }
        public CookiesModel CookieModelPruebas { get; set; }
        public CookiesModel CookieModelPro { get; set; }
        public bool EsFaceta { get; set; }
        public bool EsPagina { get; set; }
        public bool EsGadget { get; set; }
        public bool EsComponente { get; set; }
        public bool EsCookie { get; set; }
    }
}
