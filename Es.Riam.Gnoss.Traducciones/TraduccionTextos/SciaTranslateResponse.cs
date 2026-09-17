using System.Text.Json.Serialization;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class SciaTranslateResponse
    {
		[JsonPropertyName("textTranslate")]
		public string TextTranslate {  get; set; }

		[JsonPropertyName("usage")]
		public Usage Usage {  get; set; }
    }

    public class Usage
    {
		[JsonPropertyName("tokens")]
		public int Tokens { get; set; }
    }
}
