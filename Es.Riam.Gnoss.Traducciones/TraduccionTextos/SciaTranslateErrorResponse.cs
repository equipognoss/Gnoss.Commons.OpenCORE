using System.Text.Json.Serialization;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class SciaTranslateErrorResponse
    {
        public string Error {  get; set; }
        public string Message {  get; set; }
        [JsonPropertyName("status")]
        public int Status {  get; set; }    
    }
}
