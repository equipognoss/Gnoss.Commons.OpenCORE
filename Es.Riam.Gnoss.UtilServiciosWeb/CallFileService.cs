using BeetleX.Buffers;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Seguridad;
using Es.Riam.Util;
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
        private LoggingService mLoggingService;
        private TokenBearer mToken;

        public CallFileService(ConfigService configService, LoggingService loggingService)
        {
            mConfigService = configService;
            mServicioArchivosUrl = mConfigService.ObtenerUrlServicio("urlArchivos");
#if !DEBUG
            if (!mConfigService.PeticionHttps())
            {
                if (mServicioArchivosUrl.StartsWith("https://"))
                {
                    mServicioArchivosUrl = mServicioArchivosUrl.Replace("https://", "http://");
                }
            }
#endif
            mLoggingService = loggingService;
            mLoggingService.AgregarEntrada("INICIO Peticion para obtener el token del identity");
            mCallTokenService = new CallTokenService(configService);
            mToken = mCallTokenService.CallTokenApi();
            mLoggingService.AgregarEntrada("FIN Peticion para obtener el token del identity");
        }

        public string DescargarCSSOntologia(Guid pDocumentoID, string pExtension, bool convertBase64 = false)
        {
            string ontology = "";
            mLoggingService.AgregarEntrada("INICIO Peticion DescargarCSSOntologia");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"DescargarCSSOntologia?pDocumentoID={pDocumentoID}&pExtensionArchivo={pExtension}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion DescargarCSSOntologia");
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
            mLoggingService.AgregarEntrada("INICIO Peticion GuardarCSSOntologia");
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarCSSOntologia?pDocumentoID={pDocumentoID}&pDirectorio={pDirectorio}&pExtensionArchivo={pExtensionArchivo}", pFichero, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion GuardarCSSOntologia");
        }

        public void GuardarOntologia(byte[] pFichero, Guid pOntologiaID)
        {
            //object parametrosPost = new
            //{
            //    pFichero = pFichero,
            //    pOntologiaID = pOntologiaID
            //};
            mLoggingService.AgregarEntrada("INICIO Peticion GuardarOntologia");
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarOntologia?pOntologiaID={pOntologiaID}", pFichero, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion GuardarOntologia");
        }

        public void GuardarXmlOntologia(byte[] pFichero, Guid pOntologiaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion GuardarXmlOntologia");
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarXmlOntologia?pOntologiaID={pOntologiaID}", pFichero, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion GuardarXmlOntologia");
        }

        public byte[] ObtenerXmlOntologiaBytes(Guid pOntologiaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerXmlOntologia");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerXmlOntologia?pOntologiaID={pOntologiaID}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerXmlOntologia");
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] ObtenerOntologiaBytes(Guid pOntologiaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerOntologia");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerOntologia?pOntologiaID={pOntologiaID}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerOntologia");
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] ObtenerCSSOntologiaBytes(Guid pDocumentoID, string pExtension)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion DescargarCSSOntologia");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"DescargarCSSOntologia?pDocumentoID={pDocumentoID}&pExtensionArchivo={pExtension}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion DescargarCSSOntologia");
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] DescargarVersionBytes(Guid pOntologiaID, string pVersion)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion DescargarVersion");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"DescargarVersion?pOntologiaID={pOntologiaID}&pVersion={pVersion}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion DescargarVersion");
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }
         public byte[] ObtenerOntologiaFraccionada(Guid pOntologiaID, string pNombreFraccion)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerOntologiaFraccionada");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerOntologiaFraccionada?pOntologiaID={pOntologiaID}&pNombreFraccion={pNombreFraccion}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerOntologiaFraccionada");
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public byte[] ObtenerXmlOntologiaFraccionado(Guid pOntologiaID, string pNombreFraccion)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerXmlOntologiaFraccionado");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerXmlOntologiaFraccionado?pOntologiaID={pOntologiaID}&pNombreFraccion={pNombreFraccion}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerXmlOntologiaFraccionado");
            byte[] buffer = JsonConvert.DeserializeObject<byte[]>(result);
            return buffer;
        }

        public string[] ObtenerHistorialOntologia(Guid pOntologiaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerHistorialOntologia");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerHistorialOntologia?pOntologiaID={pOntologiaID}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerHistorialOntologia");
            string[] list = JsonConvert.DeserializeObject<string[]>(result);
            return list;
        }

        public void GuardarDocumentacion(string documentacion,Guid communityID, string idioma = "en" ,bool convertBase64 = false)
        {
            string ontology = "";
            byte[] buffer = Encoding.UTF8.GetBytes(documentacion);
            if (buffer.Any())
            {
                if (convertBase64)
                {ontology = Convert.ToBase64String(buffer);}
                else
                {ontology = System.Text.Encoding.Default.GetString(buffer);}
            }
            mLoggingService.AgregarEntrada("INICIO Peticion GuardarDocumentacion");
            string url = $"GuardarDocumentacionOntologia?communityID={communityID}&lang={idioma}";
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, url, buffer, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion GuardarDocumentacion");
        }

        public void GuardarOntologiaUnificada(string ontologia, Guid communityID, bool convertBase64 = false)
        {
            string ontology = "";
            byte[] buffer = Encoding.UTF8.GetBytes(ontologia);
            if (buffer.Any())
            {
                if (convertBase64)
                { ontology = Convert.ToBase64String(buffer); }
                else
                { ontology = System.Text.Encoding.Default.GetString(buffer); }
            }
            mLoggingService.AgregarEntrada("INICIO Peticion GuardarOntologiaUnificada");
            string result = CallWebMethods.CallPostApiToken(mServicioArchivosUrl, $"GuardarOntologiaUnificada?communityID={communityID}", buffer, true, "pFichero", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion GuardarOntologiaUnificada");
        }


        public byte[] ObtenerDocumentacionOntologiaBytes(Guid communityID, string idioma)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerDocumentacionOntologia");
            string url = $"ObtenerDocumentacionOntologia?communityID={communityID}&lang={idioma}";
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl,url, mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerDocumentacionOntologia");
            byte[] list = JsonConvert.DeserializeObject<byte[]>(result);
            return list;
        }

        public string ObtenerDocumentacionOntologia(Guid communityID, string idioma, bool convertBase64 = false)
        {
            //TODO: Por el momento solo se ha implementado ingles y español por el momento por lo que cualquier otro idioma devolvera la documentacion en ingles por defecto
            if (string.IsNullOrEmpty(idioma) || idioma.ToLower() != "en") {
                idioma = "es";
            }

            string documentation = "";
            byte[] buffer = ObtenerDocumentacionOntologiaBytes(communityID, idioma);
            if (buffer.Any())
            {
                if (convertBase64)
                {
                    documentation = Convert.ToBase64String(buffer);
                }
                else
                {
                    documentation = System.Text.Encoding.Default.GetString(buffer);
                }
            }
            return documentation;
        }



        public byte[] ObtenerOntologiaUnificada(Guid pOntologiaID)
        {
            mLoggingService.AgregarEntrada("INICIO Peticion ObtenerOntologiaUnificada");
            string result = CallWebMethods.CallGetApiToken(mServicioArchivosUrl, $"ObtenerOntologiaUnificada?pOntologiaID={pOntologiaID}", mToken);
            mLoggingService.AgregarEntrada("FIN Peticion ObtenerOntologiaUnificada");
            byte[] list = JsonConvert.DeserializeObject<byte[]>(result);
            return list;
        }


    }
}
