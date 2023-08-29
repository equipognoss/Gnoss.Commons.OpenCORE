using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.CL.RelatedVirtuoso
{
    public class RelatedVirtuosoCL : BaseCL
    {
        public static string KEY = "relatedvirtuoso";
        public static string VIRTUOSO_GENERAL_NAME = "virtuoso_proxy";
        public RelatedVirtuosoCL(LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService) : base(null, loggingService, redisCacheWrapper, configService, null)
        {
        }

        public KeyValuePair<string, string> ObtenerVirtuosoAfin()
        {
            var objeto = ObtenerObjetoDeCache(KEY, false);
            string virtuosoName = objeto as string;
            if (!_configService.CheckBidirectionalReplicationIsActive())
            {
                return new KeyValuePair<string, string>(VIRTUOSO_GENERAL_NAME, _configService.ObtenerVirtuosoEscritura().Value);
            }
            else if (!string.IsNullOrEmpty(virtuosoName))
            {
                string virtuosoConnection = "";
                if (virtuosoName.Equals(VIRTUOSO_GENERAL_NAME))
                {
                    virtuosoConnection = _configService.ObtenerVirtuosoEscritura().Value;
                }
                else
                {
                    virtuosoConnection = _configService.ObtenerVirutosoEscritura(virtuosoName);
                    if (string.IsNullOrEmpty(virtuosoConnection))
                    {
                        _loggingService.GuardarLogError($"No se ha encontrado la cadena Virtuoso__Escritura__ con nombre {virtuosoName}");
                        //InvalidarVirtuosoAfin();
                        _loggingService.GuardarLog($"No se ha encontrado la cadena Virtuoso__Escritura__ con nombre {virtuosoName}, se ha borrado la cache de afinidad");
                        return new KeyValuePair<string, string>();
                    }
                }
                return new KeyValuePair<string, string>(virtuosoName, virtuosoConnection);
            }
            else if (!_configService.CheckBidirectionalReplicationIsActive())
            {
                return new KeyValuePair<string, string>(VIRTUOSO_GENERAL_NAME, _configService.ObtenerVirtuosoEscritura().Value);
            }
            else
            {
                return new KeyValuePair<string, string>();
            }   
        }
        public bool ExisteAfinidadEnCache()
        {
            var objeto = ObtenerObjetoDeCache(KEY, false);
            string virtuosoName = objeto as string;
            return !string.IsNullOrEmpty(virtuosoName);
        }
        public void InvalidarVirtuosoAfin()
        {
            InvalidarCache(KEY, false); 
        }

        public void AgregarVirtuosoAfin(string pRelatedVirtuoso)
        {
            AgregarObjetoCache(KEY, pRelatedVirtuoso, 0, false);
        }
    }
}
