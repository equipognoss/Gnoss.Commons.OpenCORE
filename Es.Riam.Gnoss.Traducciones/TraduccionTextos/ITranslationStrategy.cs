using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public interface ITranslationStrategy
    {
        public TranslationResponse Translate(TranslationRequest pTranslationRequest);
        public Task<TranslationResponse> TranslateAsync(TranslationRequest pTranslationRequest, CancellationToken pCancellationToken = default);
        public LanguagesResponse GetAvailableLanguages();
    }
}
