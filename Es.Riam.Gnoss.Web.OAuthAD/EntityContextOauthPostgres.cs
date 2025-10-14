using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Es.Riam.Gnoss.Web.OAuthAD
{
    public class EntityContextOauthPostgres : EntityContextOauth
    {
        public EntityContextOauthPostgres(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService, ILogger<EntityContextOauthPostgres> logger, ILoggerFactory loggerFactory)
            : base(loggingService, dbContextOptions, configService, logger, loggerFactory)
        {

        }

        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al método ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContextOauthPostgres(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService, ILogger<EntityContextOauthPostgres> logger, ILoggerFactory loggerFactory, string pDefaultSchema = null)
           : base(loggingService, dbContextOptions, configService, logger, loggerFactory, pDefaultSchema)
        {

        }
    }
}
