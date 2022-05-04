using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.InterfacesOpen;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
    public class OAuthOpen : IOAuth
    {
        public OAuthOpen(EntityContext entityContext, LoggingService loggingService, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IHttpContextAccessor httpContextAccessor) : base(entityContext, loggingService, configService, redisCacheWrapper, virtuosoAD, servicesUtilVirtuosoAndReplication, httpContextAccessor)
        {
        }

        public override void ObtenerOAuth(Guid OrganizacionID, Guid ProyectoID)
        {
            throw new NotImplementedException();
        }
    }
}
