using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class SciaTranslateResponse
    {
		[JsonProperty("textTranslate")]
		public string TextTranslate {  get; set; }

		[JsonProperty("usage")]
		public Usage Usage {  get; set; }
    }

    public class Usage
    {
		[JsonProperty("tokens")]
		public int Tokens { get; set; }
    }
}
