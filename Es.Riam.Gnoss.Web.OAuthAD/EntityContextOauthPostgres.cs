using Es.Riam.Gnoss.OAuthAD;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;

namespace Es.Riam.Gnoss.Web.OAuthAD
{
    public class EntityContextOauthPostgres : EntityContextOauth
    {
        public EntityContextOauthPostgres(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService)
            : base(loggingService, dbContextOptions, configService)
        {

        }

        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al método ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContextOauthPostgres(LoggingService loggingService, DbContextOptions<EntityContextOauth> dbContextOptions, ConfigService configService, string pDefaultSchema = null)
           : base(loggingService, dbContextOptions, configService, pDefaultSchema)
        {

        }
    }
}
