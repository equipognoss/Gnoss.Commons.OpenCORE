using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Asistente;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Asistentes;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class UtilAsistentes
    {
        private readonly EntityContext _entityContext;
        private readonly LoggingService _loggingService;
        private readonly ConfigService _configService;
        private readonly ILogger _logger;
        private ILoggerFactory _loggerFactory;

        public UtilAsistentes(EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, ILogger<UtilAsistentes> logger, ILoggerFactory loggerFactory)
        {
            _entityContext = pEntityContext;
            _loggingService = pLoggingService;
            _configService = pConfigService;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        #region Métodos públicos

        public void EliminarRolAsistente(Guid pRolId)
        {
            AsistenteCN asistenteCN = new AsistenteCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<AsistenteCN>(), _loggerFactory);
            // Borramos las filas de RolAsistentes y nos quedamos con los asistentes afectados y comprobamos si no tienen roles para asignarles el rol de Administrador.
            List<Guid> asistentesAfectados = asistenteCN.EliminarRolAsistente(pRolId);

            foreach (Guid asistenteId in asistentesAfectados.Where(x => asistenteCN.ComprobarAsistenteSinRoles(x)))
            {
                RolAsistente rolAsistente = new RolAsistente();
                rolAsistente.AsistenteID = asistenteId;
                rolAsistente.RolID = ProyectoAD.RolAdministrador;
                asistenteCN.GuardarRolAsistente(rolAsistente);
            }

            asistenteCN.Dispose();
        }

        #endregion
    }
}
