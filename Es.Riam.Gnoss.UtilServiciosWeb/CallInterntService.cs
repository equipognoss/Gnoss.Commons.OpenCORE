using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallInterntService
    {
        private ConfigService mConfigService;
        private string mUrlInternService;
        private CallTokenService mCallTokenService;
        private TokenBearer mToken;
        private LoggingService mLoggingService;

        public CallInterntService(ConfigService configService, LoggingService loggingService)
        {
            mConfigService = configService;
            mUrlInternService = mConfigService.ObtenerUrlServicioInterno().Replace("https://", "http://");
            mCallTokenService = new CallTokenService(mConfigService);
            mLoggingService = loggingService;
            mLoggingService.AgregarEntrada("INICIO Peticion para obtener el token del identity");
            mToken = mCallTokenService.CallTokenApi();
            mLoggingService.AgregarEntrada("FIN Peticion para obtener el token del identity");
        }

        public bool AgregarDocumento(byte[] pBytes, Guid pDocumentoID,string pNombre, string pExtension)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion AgregarDocumento");
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"DocumentosLink/add-document?pDocumentoID={pDocumentoID}&pNombre={pNombre}&pExtension={pExtension}", pBytes, true, "pBytes", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion AgregarDocumento");
            bool exito = JsonConvert.DeserializeObject<bool>(result);
            return exito;
        }

        public int AgregarVideo(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion AgregarVideo");
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/AgregarVideo?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}", bufferRecurso, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion AgregarVideo");
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }

        public int AgregarVideoOrganizacion(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID, Guid pOrganizacionID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion AgregarVideoOrganizacion");
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/AgregarVideoOrganizacion?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}&pOrganizacionID={pOrganizacionID}", bufferRecurso, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion AgregarVideoOrganizacion");
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }

        public int AgregarVideoPersonal(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID, Guid pPersonaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion AgregarVideoPersonal");
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/AgregarVideoPersonal?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}&pPersonaID={pPersonaID}", bufferRecurso, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion AgregarVideoPersonal");
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }
        
        public double ObtenerEspacioVideoPersonal(Guid pDocumentoID, Guid pPersonaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerEspacioVideoPersonal");
            string result = CallWebMethods.CallGetApiToken(mUrlInternService, $"Videos/ObtenerEspacioVideoPersonal?pDocumentoID={pDocumentoID}&pPersonaID={pPersonaID}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerEspacioVideoPersonal");
            double num = JsonConvert.DeserializeObject<double>(result);
            return num;
        }

        public double ObtenerEspacioVideoOrganizacion(Guid pDocumentoID, Guid pPersonaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerEspacioVideoOrganizacion");
            string result = CallWebMethods.CallGetApi(mUrlInternService, $"Videos/ObtenerEspacioVideoOrganizacion?pDocumentoID={pDocumentoID}&pPersonaID={pPersonaID}");
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerEspacioVideoOrganizacion");
            double num = JsonConvert.DeserializeObject<double>(result);
            return num;
        }

        public bool CopiarVideo(Guid pDocumentoID, Guid pDocumentoIDCopia, Guid pPersonaID, Guid pOrganizacionID, Guid pPersonaIDDestino, Guid pOrganizacionIDDestino)
        {
            //(Guid pDocumentoID, Guid pDocumentoIDCopia, Guid pPersonaID, Guid pOrganizacionID, Guid pPersonaIDDestino, Guid pOrganizacionIDDestino)
            object item = new
            {
                pDocumentoID = pDocumentoID,
                pDocumentoIDCopia = pDocumentoIDCopia,
                pOrganizacionID = pOrganizacionID,
                pPersonaID = pPersonaID,
                pOrganizacionIDDestino = pOrganizacionIDDestino
            };
            mLoggingService.AgregarEntrada("INICIO Peticion CopiarVideo");
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/CopiarVideo", item, false, "file", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion CopiarVideo");
            bool exito = JsonConvert.DeserializeObject<bool>(result);
            return exito;
        }


    }
}
