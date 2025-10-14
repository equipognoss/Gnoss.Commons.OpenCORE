using Es.Riam.Gnoss.Elementos.Suscripcion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class ServicioDocLinks
    {
        private ConfigService mConfigService;
        private string mUrlInternService;
        private CallTokenService mCallTokenService;
        private TokenBearer mToken;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        public ServicioDocLinks(ConfigService configService, LoggingService loggingService, ILogger<ServicioDocLinks> logger, ILoggerFactory loggerFactory)
        {
            mConfigService = configService;
            mUrlInternService = mConfigService.ObtenerUrlServicioInterno();
            mCallTokenService = new CallTokenService(mConfigService);
            mLoggingService = loggingService;
            mLoggingService.AgregarEntrada("INICIO Peticion para obtener el token del identity");
            mToken = mCallTokenService.CallTokenApi();
            mLoggingService.AgregarEntrada("FIN Peticion para obtener el token del identity");
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }

        public bool EliminarDocLinksDeRecurso(Guid pDocumentoID)
        {
            try
            {
                mLoggingService.AgregarEntrada("INICIO Peticion EliminarArchivosDeRecurso");
                string result = CallWebMethods.CallGetApiToken(mUrlInternService, $"DocumentosLink/BorrarDirectorioDeDocumento?pDocumentoID={pDocumentoID}", mToken);
                mLoggingService.AgregarEntrada("FIN Peticion EliminarArchivosDeRecurso");
                bool exito = JsonConvert.DeserializeObject<bool>(result);
                return exito;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, $"Error al eliminar los documentos link del recurso {pDocumentoID}",mlogger);
                return false;
            }
        }

        public bool MoverDocLinkRecursoModificado(string pDocLink, Guid pDocumentoID)
        {
            try
            {
                CallWebMethods.CallGetApiToken(mUrlInternService, $"DocumentosLink/move-doclink-modified-resource?pImagen={HttpUtility.UrlEncode(pDocLink)}&pDocumentoID={pDocumentoID}", mToken);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool MoverDocLinksRecursoEliminado(string pRuta, Guid pDocumentoID)
        {
            try
            {
                CallWebMethods.CallGetApiToken(mUrlInternService, $"DocumentosLink/move-doclinks-deleted-resource?relative_path={HttpUtility.UrlEncode(pRuta)}&pDocumentoID={pDocumentoID}", mToken);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex, mlogger);
                return false;
            }
        }

        public bool CopiarDocLinks(Guid pDocumentoIDOrigen, Guid pDocumentoIDDestino)
        {
            try
            {
                CallWebMethods.CallGetApiToken(mUrlInternService, $"DocumentosLink/CopiarDocumentoDeDirectorio?pDocumentoIDOrigen={pDocumentoIDOrigen}&pDocumentoIDDestino={pDocumentoIDDestino}", mToken);
                return true;
            }
            catch (Exception ex)
            {
                mLoggingService.GuardarLogError(ex);
                return false;
            }
        }
    }
}
