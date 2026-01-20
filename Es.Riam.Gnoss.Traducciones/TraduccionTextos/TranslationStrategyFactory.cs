using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.Traducciones.TraduccionTextos
{
    public class TranslationStrategyFactory
    {
        public ITranslationStrategy CreateTranslationStrategy(TranslationConfig pTranslationConfig, TranslationProvider pTranslationProvider)
        {
            switch (pTranslationProvider)
            {
                case TranslationProvider.Scia:
                    return new SciaTranslationStrategy(pTranslationConfig);
                    break;
                default:
                    return new SciaTranslationStrategy(pTranslationConfig);
            }
        }
    }
}
