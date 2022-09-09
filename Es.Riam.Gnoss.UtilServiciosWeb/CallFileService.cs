using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.Seguridad;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class CallFileService
    {
        private ConfigService mConfigService;
        private string mServicioArchivosUrl;
        private CallTokenService mCallTokenService;
        private TokenBearer mToken;

        public CallFileService(ConfigService configService)
        {
            mConfigService = configService;
            mServicioArchivosUrl = mConfigService.ObtenerUrlServicio("urlArchivos");
            if (!mConfigService.PeticionHttps())
            {
                if (mServicioArchivosUrl.StartsWith("https://"))
                {
                    mServicioArchivosUrl = mServicioArchivosUrl.Replace("https://", "http://");
                }
            }
            mCallTokenService = new CallTokenService(configService);
            mToken = mCallTokenService.CallTokenApi();
        }

        public string DescargarCSSOntologia(Guid pDocumentoID, string pExtension, bool convertBase64 = false)
        {
            string ontology = "";
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"DescargarCSSOntologia?pDocumentoID={pDocumentoID}&pExtensionArchivo={pExtension}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            if (buffer != null && buffer.Any())
            {
                //Hay que tener cuidado con los Encoding
                if (convertBase64)
                {
                    ontology = Convert.ToBase64String(buffer);
                }
                else
                {
                    ontology = System.Text.Encoding.Default.GetString(buffer);
                }
            }
            return ontology;
        }

        public string ObtenerOntologia(Guid pOntologiaID, bool convertBase64 = false)
        {
            string ontology = "";
            byte[] buffer = ObtenerOntologiaBytes(pOntologiaID);
            if (buffer.Any())
            {
                if (convertBase64)
                {
                    ontology = Convert.ToBase64String(buffer);
                }
                else
                {
                    ontology = System.Text.Encoding.Default.GetString(buffer);
                }
            }
            return ontology;
        }

        public string ObtenerXmlOntologia(Guid pOntologiaID, bool convertBase64 = false)
        {
            string ontology = "";
            byte[] buffer = ObtenerXmlOntologiaBytes(pOntologiaID);
            if (buffer.Any())
            {
                if (convertBase64)
                {
                    ontology = Convert.ToBase64String(buffer);
                }
                else
                {
                    ontology = System.Text.Encoding.Default.GetString(buffer);
                }
            }
            return ontology;
        }

        public void GuardarCSSOntologia(byte[] pFichero, Guid pDocumentoID, string pDirectorio, string pExtensionArchivo)
        {
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarCSSOntologia?pDocumentoID={pDocumentoID}&pDirectorio={pDirectorio}&pExtensionArchivo={pExtensionArchivo}", pFichero, true, "pFichero", mToken);
        }

        public void GuardarOntologia(byte[] pFichero, Guid pOntologiaID)
        {
            //object parametrosPost = new
            //{
            //    pFichero = pFichero,
            //    pOntologiaID = pOntologiaID
            //};
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarOntologia?pOntologiaID={pOntologiaID}", pFichero, true, "pFichero", mToken);

        }

        public void GuardarXmlOntologia(byte[] pFichero, Guid pOntologiaID)
        {
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarXmlOntologia?pOntologiaID={pOntologiaID}", pFichero, true, "pFichero", mToken);
        }

        public byte[] ObtenerXmlOntologiaBytes(Guid pOntologiaID)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerXmlOntologia?pOntologiaID={pOntologiaID}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] ObtenerOntologiaBytes(Guid pOntologiaID)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerOntologia?pOntologiaID={pOntologiaID}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] ObtenerCSSOntologiaBytes(Guid pDocumentoID, string pExtension)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"DescargarCSSOntologia?pDocumentoID={pDocumentoID}&pExtensionArchivo={pExtension}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] DescargarVersionBytes(Guid pOntologiaID, string pVersion)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"DescargarVersion?pOntologiaID={pOntologiaID}&pVersion={pVersion}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }
         public byte[] ObtenerOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerOntologiaFraccionada?pOntologiaID={pOntologiaID}&pNombreFraccion={pNombreFraccion}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }
        public byte[] ObtenerXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerXmlOntologiaFraccionado?pOntologiaID={pOntologiaID}&pNombreFraccion={pNombreFraccion}", mToken);
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }
        public string[] ObtenerHistorialOntologia(Guid pOntologiaID)
        {
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerHistorialOntologia?pOntologiaID={pOntologiaID}", mToken);
            string[] list = JsonConvert.DeserializeObject<string[]>(result);
            return list;
        }
    }
}
