using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;


namespace Es.Riam.Gnoss.AD.EntityModelBASE
{
    public class EntityContextBASEOracle : EntityContextBASE
    {
        public EntityContextBASEOracle(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, bool pCache = false)
            : base(utilPeticion, loggingService, configService, dbContextOptions, pCache)
        {

        }

        internal EntityContextBASEOracle(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, string pDefaultSchema = null, bool pCache = false)
           : base(utilPeticion, loggingService, configService, dbContextOptions, pDefaultSchema, pCache)
        {

        }
    }
}
