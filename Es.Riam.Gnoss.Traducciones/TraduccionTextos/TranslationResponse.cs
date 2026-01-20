using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class TranslationResponse
    {
        public string TranslatedText { get; set; }
        public string Provider { get; set; }
        public int Status { get; set; }
        public bool Success =>  Status == 200;
        public string ErrorMessage { get; set; }
    }
}
