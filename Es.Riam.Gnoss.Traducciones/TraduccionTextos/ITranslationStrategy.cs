using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public interface ITranslationStrategy
    {
        public TranslationResponse Translate(TranslationRequest pTranslationRequest);
        public LanguagesResponse GetAvailableLanguages();
    }
}
