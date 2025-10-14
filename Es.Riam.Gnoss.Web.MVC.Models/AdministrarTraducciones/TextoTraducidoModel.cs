using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Web.MVC.Models.AdministrarTraducciones
{
    /// <summary>
    /// Model de la pagina "Crear"
    /// </summary>
    [Serializable]
    public class TextoTraducidoModel
    {
        public TextoTraducidoModel()
        {
        }

        public TextoTraducidoModel(Dictionary<string, string> pIdiomasDisponibles)
        {
            Idiomas = pIdiomasDisponibles;
            List<TraduccionModel> idiomasTraducir = new List<TraduccionModel>();
            foreach (string idioma in Idiomas.Keys)
            {
                idiomasTraducir.Add(new TraduccionModel(idioma, string.Empty));
            }
            Traducciones = idiomasTraducir;
        }

        public string TextoID { get; set; }
        public Dictionary<string, string> Idiomas { get; set; }
        public List<TraduccionModel> Traducciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public void AgregarTraduccion(string idioma, string texto)
        {
            foreach (TraduccionModel traduccion in Traducciones)
            {
                if (traduccion.Idioma.Equals(idioma))
                {
                    traduccion.Texto = texto;
                }
            }
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }

    /// <summary>
    /// Modelo usado para integracion continua
    /// </summary>
    [Serializable]
    public class TranslatorModel
    {
        public string TextID { get; set; }
        public Dictionary<string, string> LanguagesText { get; set; }
        public bool Deleted { get; set; }
    }
}
