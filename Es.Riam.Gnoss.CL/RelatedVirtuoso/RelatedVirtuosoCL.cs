using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
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
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        public RelatedVirtuosoCL(LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, ILogger<RelatedVirtuosoCL> logger, ILoggerFactory loggerFactory) : base(null, loggingService, redisCacheWrapper, configService, null,logger,loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public KeyValuePair<string, VirtuosoConnectionData> ObtenerVirtuosoAfin()
        {
            var objeto = ObtenerObjetoDeCache(KEY, false, typeof(string));
            string virtuosoName = objeto as string;
            if (!_configService.CheckBidirectionalReplicationIsActive())
            {
                return new KeyValuePair<string, VirtuosoConnectionData>(VIRTUOSO_GENERAL_NAME, _configService.ObtenerVirtuosoEscritura().Value);
            }
            else if (!string.IsNullOrEmpty(virtuosoName))
            {
                VirtuosoConnectionData virtuosoConnection = null;
                if (virtuosoName.Equals(VIRTUOSO_GENERAL_NAME))
                {
                    virtuosoConnection = _configService.ObtenerVirtuosoEscritura().Value;
                }
                else
                {
                    virtuosoConnection = _configService.ObtenerVirutosoEscritura(virtuosoName);
                    if (virtuosoConnection == null)
                    {
                        _loggingService.GuardarLogError($"No se ha encontrado la cadena Virtuoso__Escritura__ con nombre {virtuosoName}", mlogger);
                        //InvalidarVirtuosoAfin();
                        _loggingService.GuardarLog($"No se ha encontrado la cadena Virtuoso__Escritura__ con nombre {virtuosoName}, se ha borrado la cache de afinidad", mlogger);
                        return new KeyValuePair<string, VirtuosoConnectionData>();
                    }
                }
                return new KeyValuePair<string, VirtuosoConnectionData>(virtuosoName, virtuosoConnection);
            }
            else if (!_configService.CheckBidirectionalReplicationIsActive())
            {
                return new KeyValuePair<string, VirtuosoConnectionData>(VIRTUOSO_GENERAL_NAME, _configService.ObtenerVirtuosoEscritura().Value);
            }
            else
            {
                return new KeyValuePair<string, VirtuosoConnectionData>();
            }   
        }
        public bool ExisteAfinidadEnCache()
        {
            var objeto = ObtenerObjetoDeCache(KEY, false, typeof(string));
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
