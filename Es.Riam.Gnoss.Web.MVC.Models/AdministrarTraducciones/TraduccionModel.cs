using System;

namespace Es.Riam.Gnoss.Web.MVC.Models.AdministrarTraducciones
{
    /// <summary>
    /// Model de una traducción de un texto
    /// </summary>
    [Serializable]
    public class TraduccionModel
    {
        public TraduccionModel()
        {
        }

        public TraduccionModel(string pIdioma, string pTexto)
        {
            Idioma = pIdioma;
            Texto = pTexto;
        }

        public string Idioma { get; set; }
        public string Texto { get; set; }
    }
}
