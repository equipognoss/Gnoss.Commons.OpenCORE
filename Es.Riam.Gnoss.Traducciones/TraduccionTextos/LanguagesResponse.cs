using BeetleX.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class LanguagesResponse
    {
        public List<string> AvailableLanguajes {  get; set; }
        public string Provider {  get; set; }
        public bool Success => Status == 200;
        public string ErrorMessage {  get; set; }
        public int Status {  get; set; }
    }
}
