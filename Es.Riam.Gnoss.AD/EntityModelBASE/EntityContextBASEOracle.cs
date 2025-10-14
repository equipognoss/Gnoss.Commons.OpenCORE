using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace Es.Riam.Gnoss.AD.EntityModelBASE
{
    public class EntityContextBASEOracle : EntityContextBASE
    {
        public EntityContextBASEOracle(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, ILogger<EntityContextBASEOracle> logger, ILoggerFactory loggerFactory, bool pCache = false)
            : base(utilPeticion, loggingService, configService, dbContextOptions,logger,loggerFactory ,pCache)
        {

        }

        internal EntityContextBASEOracle(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, ILogger<EntityContextBASEOracle> logger, ILoggerFactory loggerFactory, string pDefaultSchema = null, bool pCache = false)
           : base(utilPeticion, loggingService, configService, dbContextOptions,logger,loggerFactory ,pDefaultSchema, pCache)
        {

        }
    }
}
