using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.Web.OAuthAD
{
    public class EntityContextOauthOracle : EntityContextOauth
    {
        public EntityContextOauthOracle(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService, ILogger<EntityContextOauthOracle> logger, ILoggerFactory loggerFactory)
            : base(loggingService, dbContextOptions, configService,logger,loggerFactory)
        {

        }

        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al método ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContextOauthOracle(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService, ILogger<EntityContextOauthOracle> logger, ILoggerFactory loggerFactory, string pDefaultSchema = null)
           : base(loggingService, dbContextOptions, configService,logger,loggerFactory ,pDefaultSchema)
        {

        }
    }
}
