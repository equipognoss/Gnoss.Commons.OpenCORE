using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModelBASE
{
    public class EntityContextBASEPostgres: EntityContextBASE
    {
        public EntityContextBASEPostgres(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, bool pCache = false)
            : base(utilPeticion, loggingService, configService, dbContextOptions, pCache)
        {

        }

        internal EntityContextBASEPostgres(UtilPeticion utilPeticion, LoggingService loggingService, ConfigService configService, DbContextOptions<EntityContextBASE> dbContextOptions, string pDefaultSchema = null, bool pCache = false)
           : base(utilPeticion, loggingService, configService, dbContextOptions, pDefaultSchema, pCache)
        {

        }
    }
}
