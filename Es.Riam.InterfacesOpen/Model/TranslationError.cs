using Es.Riam.Gnoss.RabbitMQ.Models;
using System;
using System.Collections.Generic;

namespace Es.Riam.InterfacesOpen.Model
{
    public class TranslationError : TranslationSuccess
    {
        /// <summary>
        /// Descripción del error producido durante la traduccion:
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Diccionario cuya clave es el codigo de idioma y su valor
        /// un numero que representa el tipo de error:
        /// 0 -> Idioma no soportado
        /// 1 -> Idioma soportado
        /// 3 -> traduccion correcta
        /// 4 -> traduccion fallida
        /// </summary>
        public Dictionary<string, int> LanguagesStatus { get; set; }

        public TranslationError() : base() { }

        public TranslationError(int status, TranslationRabbitModel request, DateTime processedDate, int usage, string pError, Dictionary<string, int> pLanguagesStatus) : base(status, request, processedDate, usage)
        {
            Error = pError;
            LanguagesStatus = pLanguagesStatus;
        }

        public void ParserTranslationSuccess(TranslationSuccess pTranslationSuccess)
        {
            Status = pTranslationSuccess.Status;
            ProcessedDate = pTranslationSuccess.ProcessedDate;
            Status = pTranslationSuccess.Status;
            Request = pTranslationSuccess.Request;
            Usage = pTranslationSuccess.Usage;
        }
    }
}
