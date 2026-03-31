using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Asistente;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.AD.Asistentes
{
    public class AsistenteAD : BaseAD
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;

        #region Constructores

        public AsistenteAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<AsistenteAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }

        #endregion

        #region Métodos públicos

        public List<Asistente> ObtenerAsistentesPorProyecto(Guid pProyectoId)
        {
            return mEntityContext.Asistente.Include(x => x.RolAsistentes).Where(x => x.ProyectoID == pProyectoId).ToList();
        }

        public Asistente ObtenerAsistenePorProyecto(Guid pAsistenteId, Guid pProyectoId)
        {
            return mEntityContext.Asistente.Include(x => x.RolAsistentes).FirstOrDefault(x => x.AsistenteID.Equals(pAsistenteId) && x.ProyectoID.Equals(pProyectoId));
        }

        public bool ExisteAsistenteEnProyecto(Guid pAsistenteId, Guid pProyectoId)
        {
            return mEntityContext.Asistente.FirstOrDefault(x => x.AsistenteID.Equals(pAsistenteId) && x.ProyectoID.Equals(pProyectoId)) != null;
        }

        public void GuardarAsistente(Asistente pAsistente)
        {
            Asistente asistenteActual = mEntityContext.Asistente.FirstOrDefault(x => x.AsistenteID.Equals(pAsistente.AsistenteID) && x.ProyectoID.Equals(pAsistente.ProyectoID));
            if (asistenteActual != null)
            {
                asistenteActual.Nombre = pAsistente.Nombre;
                asistenteActual.Descripcion = pAsistente.Descripcion;
                asistenteActual.Token = pAsistente.Token;
                asistenteActual.HostAsistente = pAsistente.HostAsistente;
                asistenteActual.Icono = pAsistente.Icono;
                asistenteActual.Activo = pAsistente.Activo;
            }
            else
            {
                mEntityContext.Asistente.Add(pAsistente);
            }

            mEntityContext.SaveChanges();
        }

        public List<Guid> ObtenerRolesAsistente(Guid pAsistenteId)
        {
            return [.. mEntityContext.RolAsistente.Where(x => x.AsistenteID.Equals(pAsistenteId)).Select(x => x.RolID)];
        }

        public void GuardarRolAsistente(RolAsistente pRolAsistente)
        {
            RolAsistente rolAsistenteDB = mEntityContext.RolAsistente.FirstOrDefault(x => x.RolID.Equals(pRolAsistente.RolID) && x.AsistenteID.Equals(pRolAsistente.AsistenteID));

            if(rolAsistenteDB == null)
            {
                mEntityContext.RolAsistente.Add(pRolAsistente);
                mEntityContext.SaveChanges();
            }
        }

        public void EliminarRolesAsistente(List<Guid> pRolesId, Guid pAsistenteId)
        {
            if(pRolesId.Count > 0)
            {
                List<RolAsistente> rolesAsistente = mEntityContext.RolAsistente.Where(x => x.AsistenteID.Equals(pAsistenteId) && pRolesId.Contains(x.RolID)).ToList();
                mEntityContext.RemoveRange(rolesAsistente);
                mEntityContext.SaveChanges();
            }
        }

        public List<Guid> EliminarRolAsistente(Guid pRolId)
        {
            List<RolAsistente> rolesAsistente = mEntityContext.RolAsistente.Where(x => x.RolID.Equals(pRolId)).ToList();
            List<Guid> asistentesId = new List<Guid>();
            if (rolesAsistente.Count > 0)
            {
                asistentesId = rolesAsistente.Select(x => x.AsistenteID).ToList();
                mEntityContext.RemoveRange(rolesAsistente);
                mEntityContext.SaveChanges();
                
            }
            return asistentesId;
        }

        public bool ComprobarAsistenteSinRoles(Guid pAsistenteId)
        {
            return !mEntityContext.RolAsistente.Any(x => x.AsistenteID.Equals(pAsistenteId));
        }

        public void EliminarAsistente(Guid pAsistenteId, Guid pProyectoId)
        {
            Asistente asistenteFila = mEntityContext.Asistente.Where(item => item.AsistenteID.Equals(pAsistenteId) && item.ProyectoID.Equals(pProyectoId)).FirstOrDefault();
            if (asistenteFila != null)
            {
                EliminarRolesAsisente(pAsistenteId);
                EliminarConfiguracionesAsistente(pAsistenteId);
                mEntityContext.EliminarElemento(asistenteFila);
                mEntityContext.SaveChanges();
            }
        }

        public Dictionary<Guid, string> ObtenerAsistentesAfectadosPorRol(Guid pRolId)
        {
            return mEntityContext.RolAsistente.Include(x => x.Asistente).Where(x=> x.RolID.Equals(pRolId)).ToDictionary(x => x.AsistenteID, x => x.Asistente.Nombre);
        }

        private void EliminarRolesAsisente(Guid pAsistenteId)
        {
            List<RolAsistente> rolesAsistente = mEntityContext.RolAsistente.Where(x => x.AsistenteID.Equals(pAsistenteId)).ToList();
            mEntityContext.RemoveRange(rolesAsistente);
        }

        private void EliminarConfiguracionesAsistente(Guid pAsistenteId)
        {
            List<AsistenteConfigIdentidad> configuraciones = mEntityContext.AsistenteConfigIdentidad.Where(x => x.AsistenteID.Equals(pAsistenteId)).ToList();
            mEntityContext.RemoveRange(configuraciones);
        }

        public void GuardarAsistenteConfigIdentidad(AsistenteConfigIdentidad pAsistenteConfigIdentidad)
        {
            AsistenteConfigIdentidad asistenteConfigActual = mEntityContext.AsistenteConfigIdentidad.FirstOrDefault(x => x.AsistenteID.Equals(pAsistenteConfigIdentidad.AsistenteID) && x.IdentidadID.Equals(pAsistenteConfigIdentidad.IdentidadID));
            if (asistenteConfigActual != null)
            {
                asistenteConfigActual.AsistentePorDefecto = pAsistenteConfigIdentidad.AsistentePorDefecto;
            }
            else
            {
                mEntityContext.AsistenteConfigIdentidad.Add(pAsistenteConfigIdentidad);
            }

            mEntityContext.SaveChanges();
        }
        #endregion
    }
}
