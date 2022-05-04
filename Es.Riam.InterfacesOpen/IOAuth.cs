using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.InterfacesOpen
{
    public abstract class IOAuth
    {
        protected LoggingService mLoggingService;
        protected EntityContext mEntityContext;
        protected ConfigService mConfigService;
        protected RedisCacheWrapper mRedisCacheWrapper;
        protected IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;
        protected IHttpContextAccessor mHttpContextAccessor;
        protected VirtuosoAD mVirtuosoAD;

        public IOAuth(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IHttpContextAccessor httpContextAccessor)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mHttpContextAccessor = httpContextAccessor;
            mVirtuosoAD = virtuosoAD;
        }

        public abstract void ObtenerOAuth(Guid OrganizacionID, Guid ProyectoID);

    }
}
