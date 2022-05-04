using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.EntityModel
{
    public class EntityContextPostgres : EntityContext
    {
        /// <summary>
        /// Constructor internal, para obtener un objeto EntityContext, llamar al m�todo ObtenerEntityContext del BaseAD
        /// </summary>
        public EntityContextPostgres(UtilPeticion utilPeticion, LoggingService loggingService, ILoggerFactory loggerFactory, DbContextOptions<EntityContext> dbContextOptions, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, bool pCache = false)
            : base(utilPeticion, loggingService, loggerFactory, dbContextOptions, configService, servicesUtilVirtuosoAndReplication, pCache)
        {
            
        }

        public EntityContextPostgres(UtilPeticion utilPeticion, LoggingService loggingService, ILoggerFactory loggerFactory, DbContextOptions<EntityContext> dbContextOptions, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, string pDefaultSchema = null, bool pCache = false)
            : base(utilPeticion, loggingService, loggerFactory, dbContextOptions, configService, servicesUtilVirtuosoAndReplication, pDefaultSchema, pCache)
        {
           
        }
    }
}
