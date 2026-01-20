using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class TranslationRequest
    {
        public string Text {  get; set; }
        public string SourceLanguage {  get; set; }
        public string TargetLanguage {  get; set; }
        public string Model { get; set; }
        public string AdditionalInstructions {  get; set; }
    }
}
