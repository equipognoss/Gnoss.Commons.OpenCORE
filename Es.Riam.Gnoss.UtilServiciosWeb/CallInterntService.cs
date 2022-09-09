using Es.Riam.Gnoss.Util.Configuracion;
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

        public CallInterntService(ConfigService configService)
        {
            mConfigService = configService;
            mUrlInternService = mConfigService.ObtenerUrlServicioInterno().Replace("https://", "http://");
            mCallTokenService = new CallTokenService(mConfigService);
            mToken = mCallTokenService.CallTokenApi();
        }

        public bool AgregarDocumento(byte[] pBytes, Guid pDocumentoID,string pNombre, string pExtension)
        {
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"DocumentosLink/add-document?pDocumentoID={pDocumentoID}&pNombre={pNombre}&pExtension={pExtension}", pBytes, true, "pBytes", mToken);
            bool exito = JsonConvert.DeserializeObject<bool>(result);
            return exito;
        }

        public int AgregarVideo(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID)
        {
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/AgregarVideo?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}", bufferRecurso, true, "pFichero", mToken);
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }

        public int AgregarVideoOrganizacion(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID, Guid pOrganizacionID)
        {
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/AgregarVideoOrganizacion?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}&pOrganizacionID={pOrganizacionID}", bufferRecurso, true, "pFichero", mToken);
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }

        public int AgregarVideoPersonal(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID, Guid pPersonaID)
        {
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/AgregarVideoPersonal?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}&pPersonaID={pPersonaID}", bufferRecurso, true, "pFichero", mToken);
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }
        
        public double ObtenerEspacioVideoPersonal(Guid pDocumentoID, Guid pPersonaID)
        {
            string result = CallWebMethods.CallGetApiToken(mUrlInternService, $"Videos/ObtenerEspacioVideoPersonal?pDocumentoID={pDocumentoID}&pPersonaID={pPersonaID}", mToken);
            double num = JsonConvert.DeserializeObject<double>(result);
            return num;
        }

        public double ObtenerEspacioVideoOrganizacion(Guid pDocumentoID, Guid pPersonaID)
        {
            string result = CallWebMethods.CallGetApi(mUrlInternService, $"Videos/ObtenerEspacioVideoOrganizacion?pDocumentoID={pDocumentoID}&pPersonaID={pPersonaID}");
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
            string result = CallWebMethods.CallPostApiToken(mUrlInternService, $"Videos/CopiarVideo", item, false, "file", mToken);
            bool exito = JsonConvert.DeserializeObject<bool>(result);
            return exito;
        }


    }
}
