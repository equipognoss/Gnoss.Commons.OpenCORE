using Es.Riam.Gnoss.RabbitMQ.Models;
using System;

namespace Es.Riam.InterfacesOpen.Model
{
    public class TranslationSuccess : EventSwitchingBase
    {
        /// <summary>
        /// Codigo de respuesta. 
        /// Posibles valores:
        /// Ok -> 200
        /// Parametros invalidos -> 400
        /// Usuario no tiene permisos sobre el recurso -> 403
        /// Recurso no existe -> 404
        /// Demasiadas peticiones al servicio -> 429
        /// Error no controlado -> 500
        /// </summary>
        public int Status { get; set; }

        // TODO: Especificar el Objeto de peticion cuando el servicio de traducciones este listo
        /// <summary>
        /// Objeto recibido en la peticion de la traduccion
        /// </summary>
        public TranslationRabbitModel Request { get; set; }

        /// <summary>
        /// Indica la fecha de procesamiento de la traduccion
        /// </summary>
        public DateTime ProcessedDate { get; set; }

        /// <summary>
        /// Indica los tokens consumidos de la traduccion
        /// </summary>
        public int Usage { get; set; }

        public TranslationSuccess() : base("traduccion") { }

        public TranslationSuccess(int status, TranslationRabbitModel request, DateTime processedDate, int usage) : base("traduccion")
        {
            Status = status;
            Request = request;
            ProcessedDate = processedDate;
            Usage = usage;
        }

    }
}
