using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class TranslationService
    {
        private ITranslationStrategy mStrategyTranslation;
        public TranslationService(ITranslationStrategy strategy)
        {
            mStrategyTranslation = strategy;
        }

        public void SetStrategy(ITranslationStrategy strategy)
        {
            mStrategyTranslation = strategy;
        }

        public TranslationResponse ExecuteTranslation(TranslationRequest pTranslationRequest)
        {
            return mStrategyTranslation.Translate(pTranslationRequest);
        }

        public LanguagesResponse GetAvailableLanguages()
        {
            return mStrategyTranslation.GetAvailableLanguages();
        }
    }
}
