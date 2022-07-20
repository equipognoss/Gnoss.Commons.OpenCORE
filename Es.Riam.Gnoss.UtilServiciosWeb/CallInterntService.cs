using Es.Riam.Gnoss.Util.Configuracion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallInterntService
    {
        private ConfigService mConfigService;
        private string mUrlInternService;

        public CallInterntService(ConfigService configService)
        {
            mConfigService = configService;
            mUrlInternService = mConfigService.ObtenerUrlServicioInterno().Replace("https://", "http://");
        }

        public bool AgregarDocumento(byte[] pBytes, Guid pDocumentoID,string pNombre, string pExtension)
        {
            string result = CallWebMethods.CallPostApi(mUrlInternService, $"DocumentosLink/add-document?pDocumentoID={pDocumentoID}&pNombre={pNombre}&pExtension={pExtension}", pBytes, true, "pBytes");
            bool exito = JsonConvert.DeserializeObject<bool>(result);
            return exito;
        }

        public int AgregarVideo(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID)
        {
            string result = CallWebMethods.CallPostApi(mUrlInternService, $"Videos/AgregarVideo?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}", bufferRecurso, true, "pFichero");
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }

        public int AgregarVideoOrganizacion(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID, Guid pOrganizacionID)
        {
            string result = CallWebMethods.CallPostApi(mUrlInternService, $"Videos/AgregarVideoOrganizacion?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}&pOrganizacionID={pOrganizacionID}", bufferRecurso, true, "pFichero");
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }

        public int AgregarVideoPersonal(byte[] bufferRecurso, string pExtensionArchivo, Guid pDocumentoID, Guid pPersonaID)
        {
            string result = CallWebMethods.CallPostApi(mUrlInternService, $"Videos/AgregarVideoPersonal?pDocumentoID={pDocumentoID}&pExtension={pExtensionArchivo}&pPersonaID={pPersonaID}", bufferRecurso, true, "pFichero");
            int exito = JsonConvert.DeserializeObject<int>(result);
            return exito;
        }
        
        public double ObtenerEspacioVideoPersonal(Guid pDocumentoID, Guid pPersonaID)
        {
            string result = CallWebMethods.CallGetApi(mUrlInternService, $"Videos/ObtenerEspacioVideoPersonal?pDocumentoID={pDocumentoID}&pPersonaID={pPersonaID}");
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
            string result = CallWebMethods.CallPostApi(mUrlInternService, $"Videos/CopiarVideo", item);
            bool exito = JsonConvert.DeserializeObject<bool>(result);
            return exito;
        }


    }
}
