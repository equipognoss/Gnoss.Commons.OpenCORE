using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.IntegracionContinua;
using Es.Riam.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    public class UtilIntegracionContinua
    {
        //private LoggingService mLoggingService;
        //private EntityContext mEntityContext;
        //private ConfigService mConfigService;

        //public UtilIntegracionContinua(LoggingService loggingService, EntityContext entityContext, ConfigService configService)
        //{
        //    mLoggingService = loggingService;
        //    mEntityContext = entityContext;
        //    mConfigService = configService;
        //}

        //private static readonly HttpClient mHttpClient = new HttpClient();

        //public AdministrarCredencialesViewModel PeticionApiUsuarioExiste(AdministrarCredencialesViewModel pModel, Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //        try
        //        {
        //            string peticion = UrlApiIntegracionContinua + $"/integracion/user-registered";
        //            string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //            pModel.EstaRegistrado = bool.Parse(UtilGeneral.WebRequest("POST", peticion, byteData));
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            pModel.Error = "Error al ejecutar la petición, contácte con un administrador";
        //        }
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public bool VersionHotfixSinDesplegar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    bool resultado = true;
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/can-deploy";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    resultado = JsonConvert.DeserializeObject<bool>(UtilGeneral.WebRequest("POST", peticion, byteData));
        //    return resultado;
        //}

        //public bool VersionActiva(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pVersion)
        //{
        //    AdministrarDesplieguesViewModel adminDespliegue = new AdministrarDesplieguesViewModel();
        //    bool resultado = false;
        //    try
        //    {
        //        string peticion = UrlApiIntegracionContinua + $"/integracion/get-versions";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //        byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //        adminDespliegue = JsonConvert.DeserializeObject<AdministrarDesplieguesViewModel>(UtilGeneral.WebRequest("POST", peticion, byteData));

        //        if (!adminDespliegue.VersionActualEntornoActual.Equals(pVersion))
        //        {
        //            resultado = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        mLoggingService.GuardarLogError(ex);
        //        adminDespliegue.Error = "Error al ejecutar la petición, contácte con un administrador";
        //    }
        //    return resultado;
        //}

        //public static HttpStatusCode DeployVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    HttpStatusCode resultado = HttpStatusCode.BadRequest;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar) && !string.IsNullOrEmpty(pUrlApiDespliegue))
        //    {
        //        string peticion = UrlApiIntegracionContinua + "/integracion/deploy-version";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&TagName={pTagName}&UrlApiDespliegue={pUrlApiDespliegue}";
        //        var message = WebPost(peticion, requestParameters);
        //        resultado = message.StatusCode;
        //    }
        //    return resultado;
        //}

        ////Hacer peticion para fusionar Master en Develop
        ///// <summary>
        ///// Petición para hacer el merge de master con la rama activa
        ///// </summary>
        ///// <param name="pClaveProyectoSeleccionado"></param>
        ///// <param name="pUsuarioID"></param>
        ///// <param name="pEntornoIntegracionContinua"></param>
        ///// <param name="UrlApiIntegracionContinua"></param>
        ///// <returns></returns>
        //public static HttpStatusCode MergeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    HttpStatusCode resultado = HttpStatusCode.BadRequest;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        string peticion = UrlApiIntegracionContinua + "/integracion/merge-version";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //        var message = WebPost(peticion, requestParameters);
        //        resultado = message.StatusCode;
        //    }
        //    return resultado;
        //}

        ////MODIFICACIONIC:Permitir validar versión
        //public static HttpStatusCode EstabilizarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    HttpStatusCode resultado = HttpStatusCode.BadRequest;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar) && !string.IsNullOrEmpty(pUrlApiDespliegue))
        //    {
        //        string peticion = UrlApiIntegracionContinua + "/integracion/stabilize-version";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&TagName={pTagName}&UrlApiDespliegue={pUrlApiDespliegue}";
        //        var message = WebPost(peticion, requestParameters);
        //        resultado = message.StatusCode;
        //    }
        //    return resultado;
        //}

        //public void ChangeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        //{
        //    PeticionVersion(pClaveProyectoSeleccionado, pUsuarioID, pEntornoIntegracionContinua, UrlApiIntegracionContinua, pTagName, pUrlApiDespliegue, "change-version");
        //}

        //public void CrearVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        //{
        //    PeticionVersion(pClaveProyectoSeleccionado, pUsuarioID, pEntornoIntegracionContinua, UrlApiIntegracionContinua, pTagName, pUrlApiDespliegue, "create-version");
        //}

        //private void PeticionVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue, string pAccion)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/{pAccion}";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&TagName={pTagName}&UrlApiDespliegue={pUrlApiDespliegue}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    UtilGeneral.WebRequest("POST", peticion, byteData);
        //}

        //public void EnviarResultadoFusion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha)
        //{

        //    string peticion = UrlApiIntegracionContinua + $"/integracion/upload-fusion";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Content={HttpUtility.UrlEncode(pContenido.Trim())}&Type={pTipo}&Fecha={pFecha}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    UtilGeneral.WebRequest("POST", peticion, byteData);

        //}

        //public void EnviarResultadoConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/upload-fusion-conflict";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Content={HttpUtility.UrlEncode(pContenido.Trim())}&Type={pTipo}&Fecha={pFecha}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    UtilGeneral.WebRequest("POST", peticion, byteData);

        //}

        //public void DescartarCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha, string pTextArea)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/discard-change";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Fecha={pFecha}&Contenido={HttpUtility.UrlEncode(pTextArea.Trim())}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    UtilGeneral.WebRequest("POST", peticion, byteData);
        //}

        //public void DescartarConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/discard-change-conflict";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Fecha={pFecha}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    UtilGeneral.WebRequest("POST", peticion, byteData);
        //}

        //public AdministrarRamasViewModel ObtenerEstadoIntegracion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    AdministrarRamasViewModel pModel = new AdministrarRamasViewModel();
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //        try
        //        {
        //            string peticion = UrlApiIntegracionContinua + $"/integracion/state-integration";
        //            string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //            IntegracionModel respuesta = JsonConvert.DeserializeObject<IntegracionModel>(UtilGeneral.WebRequest("POST", peticion, byteData));
        //            if (respuesta != null)
        //            {
        //                if (!string.IsNullOrEmpty(respuesta.Nombre))
        //                {
        //                    pModel.Estado = respuesta.Estado;
        //                    pModel.Nombre = respuesta.Nombre;
        //                }
        //                pModel.EsFusionable = respuesta.EsFusionable;
        //                pModel.TodasLasRamasRepositorio = respuesta.TodasLasRamasRepositorio;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            pModel.Error = "Error al ejecutar la petición, contácte con un administrador";
        //        }
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public static AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosImportacion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    AdministrarIntegracionContinuaViewModel pModel = new AdministrarIntegracionContinuaViewModel();
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pNombreCorto)
        //{
        //    AdministrarIntegracionContinuaViewModel pModel = new AdministrarIntegracionContinuaViewModel();
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua))
        //    {
        //        //string peticion = UrlApiIntegracionContinua + $"/config/repositorios";
        //        string peticion = UrlApiIntegracionContinua + $"/config/integracion?pNombreCorto={pNombreCorto}";
        //        pModel = JsonConvert.DeserializeObject<AdministrarIntegracionContinuaViewModel>(UtilGeneral.WebRequest("POST", peticion, null /*byteData*/));
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //        pModel.URLAPIIntegracionContinua = UrlApiIntegracionContinua;
        //        bool iniciado = false;
        //        string peticionRepositorioIniciado = UrlApiIntegracionContinua + "/config/check-repository-status";
        //        AdministrarIntegracionContinuaViewModel model = new AdministrarIntegracionContinuaViewModel();
        //        if (!string.IsNullOrEmpty(pEntornoIntegracionContinua))
        //        {
        //            model.Proyeto = pClaveProyectoSeleccionado;
        //            model.Entorno = Guid.Parse(pEntornoIntegracionContinua);
        //            string response = UtilWeb.WebRequestPostWithJsonObject(peticionRepositorioIniciado, model, "");
        //            short estainiciado = JsonConvert.DeserializeObject<short>(response);
        //            if (estainiciado == 1)
        //            {
        //                iniciado = true;
        //            }
        //        }
        //        pModel.Iniciado = iniciado;
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaViewModel pModel)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua))
        //    {
        //        string peticion = UrlApiIntegracionContinua + $"/config/repositorios";
        //        /*string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //        byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);*/
        //        AdministrarIntegracionContinuaViewModel Model = JsonConvert.DeserializeObject<AdministrarIntegracionContinuaViewModel>(UtilGeneral.WebRequest("POST", peticion, null/* byteData*/));
        //        pModel.EntornoIDDevelop = Model.EntornoIDDevelop;
        //        pModel.EntornoIDRelease = Model.EntornoIDRelease;
        //        pModel.EntornoIDMaster = Model.EntornoIDMaster;
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public static AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosImportacion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaViewModel pModel)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    AdministrarIntegracionContinuaViewModel model = new AdministrarIntegracionContinuaViewModel();
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        string peticion = UrlApiIntegracionContinua + $"/config/repositorios";
        //        /*string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //        byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);*/
        //        AdministrarIntegracionContinuaViewModel Model = JsonConvert.DeserializeObject<AdministrarIntegracionContinuaViewModel>(UtilGeneral.WebRequest("POST", peticion, null/* byteData*/));
        //        model.EntornoIDDevelop = Model.EntornoIDDevelop;
        //        model.EntornoIDRelease = Model.EntornoIDRelease;
        //        model.EntornoIDMaster = Model.EntornoIDMaster;
        //        if (Model.EntornoIDRelease != null)
        //        {
        //            model.Release = "Release";
        //        }
        //        if (Model.EntornoIDMaster != null)
        //        {
        //            model.Master = "Master";
        //        }

        //        model.Entorno = auxiliar;
        //        model.Proyeto = pClaveProyectoSeleccionado;
        //        model.Usuario = pUsuarioID;
        //    }
        //    else
        //    {
        //        model.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return model;
        //}

        //public static AdministrarIntegracionContinuaRepositorioViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaRepositorioViewModel pModel)
        //{
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        pModel.Entorno = auxiliar;
        //        pModel.Proyeto = pClaveProyectoSeleccionado;
        //        pModel.Usuario = pUsuarioID;
        //    }
        //    else
        //    {
        //        pModel.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return pModel;
        //}

        //public void ActualizeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string urlApiDesplieguesEntornoSiguiente, string pUrlApiDespliegue)
        //{
        //    string peticion = pUrlApiIntegracionContinua + $"/integracion/change-to-current-version";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&TagName={string.Empty}&UrlApiDespliegue={pUrlApiDespliegue}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    UtilGeneral.WebRequest("POST", peticion, byteData);
        //}

        //public bool ExistCurrentVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        //{
        //    string peticion = pUrlApiIntegracionContinua + $"/integracion/exist-version";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    return bool.Parse(UtilGeneral.WebRequest("POST", peticion, byteData));
        //}

        //public AdministrarListaCambiosViewModel ObtenerListaCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    AdministrarListaCambiosViewModel model = new AdministrarListaCambiosViewModel();
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        try
        //        {
        //            string peticion = UrlApiIntegracionContinua + $"/integracion/metadata-changes-repository";
        //            string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //            model = JsonConvert.DeserializeObject<AdministrarListaCambiosViewModel>(UtilGeneral.WebRequest("POST", peticion, byteData));
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            model.Error = "Error al ejecutar la petición, contácte con un administrador";
        //        }
        //    }
        //    else
        //    {
        //        model.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return model;
        //}

        //public AdministracionConflictosCambiosViewModel ObtenerListaConflictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    AdministracionConflictosCambiosViewModel model = new AdministracionConflictosCambiosViewModel();
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        try
        //        {
        //            string peticion = UrlApiIntegracionContinua + $"/integracion/metadata-conflicts-repository";
        //            string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //            model = JsonConvert.DeserializeObject<AdministracionConflictosCambiosViewModel>(UtilGeneral.WebRequest("POST", peticion, byteData));

        //            if (model.DiccionarioConflictos.Count == 0)
        //            {
        //                if (HayConlictos(pClaveProyectoSeleccionado, pUsuarioID, pEntornoIntegracionContinua, UrlApiIntegracionContinua))
        //                {
        //                    model.Error = "Hay cambios que requieren fusion en otro entorno, por favor compruebe que estan todos validados para continuar";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            model.Error = "Error al ejecutar la petición, contácte con un administrador";
        //        }
        //    }
        //    else
        //    {
        //        model.Error = "Esta comunidad no no puede usar la integración continua";
        //    }
        //    return model;
        //}

        //public AdministrarFusionCambiosViewModel ObtenerContenidoCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        //{
        //    AdministrarFusionCambiosViewModel resultado = new AdministrarFusionCambiosViewModel();
        //    try
        //    {
        //        string peticion = pUrlApiIntegracionContinua + $"/integracion/change-file";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Fecha={pFecha}";
        //        byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //        resultado.ContenidoCambio = UtilGeneral.WebRequest("POST", peticion, byteData);
        //        resultado.EntornoCambio = pNombreEntorno;
        //        resultado.ContenidoBD = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        mLoggingService.GuardarLogError(ex);
        //        resultado.Error = "Error al ejecutar la petición, contácte con un administrador";
        //    }
        //    return resultado;
        //}

        //public AdministrarFusionCambiosViewModel ObtenerContenidoCambioConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        //{
        //    AdministrarFusionCambiosViewModel resultado = new AdministrarFusionCambiosViewModel();
        //    try
        //    {
        //        string peticion = pUrlApiIntegracionContinua + $"/integracion/change-file-conflict";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Fecha={pFecha}";
        //        byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //        resultado.ContenidoCambio = UtilGeneral.WebRequest("POST", peticion, byteData);
        //        resultado.EntornoCambio = pNombreEntorno;
        //        resultado.ContenidoBD = string.Empty;
        //    }
        //    catch (Exception ex)
        //    {
        //        mLoggingService.GuardarLogError(ex);
        //        resultado.Error = "Error al ejecutar la petición, contácte con un administrador";
        //    }
        //    return resultado;
        //}

        //public AdministrarDesplieguesViewModel ObtenerVersionesProyecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    AdministrarDesplieguesViewModel resultado = new AdministrarDesplieguesViewModel();
        //    try
        //    {
        //        string peticion = UrlApiIntegracionContinua + $"/integracion/get-versions";
        //        string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //        byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //        resultado = JsonConvert.DeserializeObject<AdministrarDesplieguesViewModel>(UtilGeneral.WebRequest("POST", peticion, byteData));
        //    }
        //    catch (Exception ex)
        //    {
        //        mLoggingService.GuardarLogError(ex);
        //        resultado.Error = "Error al ejecutar la petición, contácte con un administrador";
        //    }
        //    return resultado;
        //}

        //public bool VerificarFechaCambioCorrecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/verify-change-is-correct";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Fecha={pFecha}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    return bool.Parse(UtilGeneral.WebRequest("POST", peticion, byteData));
        //}

        //public bool VerificarFechaCambioCorrectoConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/verify-change-is-correct-conflict";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}&FileName={FileName}&Entorno={pNombreEntorno}&Fecha={pFecha}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    return bool.Parse(UtilGeneral.WebRequest("POST", peticion, byteData));
        //}

        //public bool HayConlictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    bool conflictos = false;
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        try
        //        {
        //            string peticionCambios = UrlApiIntegracionContinua + $"/integracion/conflicts";
        //            string requestParametersCambios = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteDataCambios = Encoding.UTF8.GetBytes(requestParametersCambios);
        //            conflictos = JsonConvert.DeserializeObject<bool>(UtilGeneral.WebRequest("POST", peticionCambios, byteDataCambios));
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            return false;
        //        }
        //    }
        //    return conflictos;

        //}

        ///// <summary>
        ///// Metodo para detectar si hay cambios pendientes para hacer la fusión.
        ///// </summary>
        ///// <param name="pClaveProyectoSeleccionado"></param>
        ///// <param name="pUsuarioID"></param>
        ///// <param name="pEntornoIntegracionContinua"></param>
        ///// <param name="UrlApiIntegracionContinua"></param>
        ///// <returns></returns>
        //public bool HayCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    bool conflictos = false;
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        try
        //        {
        //            string peticionCambios = UrlApiIntegracionContinua + $"/integracion/changes";
        //            string requestParametersCambios = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteDataCambios = Encoding.UTF8.GetBytes(requestParametersCambios);
        //            conflictos = JsonConvert.DeserializeObject<bool>(UtilGeneral.WebRequest("POST", peticionCambios, byteDataCambios));
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            return false;
        //        }
        //    }
        //    return conflictos;

        //}

        ///// <summary>
        ///// Metodo para saber si quedan cambios (que ya se han fusionado) sin procesar.
        ///// </summary>
        ///// <param name="pClaveProyectoSeleccionado"></param>
        ///// <param name="pUsuarioID"></param>
        ///// <param name="pEntornoIntegracionContinua"></param>
        ///// <param name="UrlApiIntegracionContinua"></param>
        ///// <returns></returns>
        //public bool CambiosSinProcesar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    bool conflictos = false;
        //    Guid auxiliar = Guid.Empty;
        //    if (!string.IsNullOrEmpty(UrlApiIntegracionContinua) && Guid.TryParse(pEntornoIntegracionContinua, out auxiliar))
        //    {
        //        try
        //        {
        //            string peticionCambios = UrlApiIntegracionContinua + $"/integracion/pending-changes";
        //            string requestParametersCambios = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //            byte[] byteDataCambios = Encoding.UTF8.GetBytes(requestParametersCambios);
        //            conflictos = JsonConvert.DeserializeObject<bool>(UtilGeneral.WebRequest("POST", peticionCambios, byteDataCambios));
        //        }
        //        catch (Exception ex)
        //        {
        //            mLoggingService.GuardarLogError(ex);
        //            return false;
        //        }
        //    }
        //    return conflictos;

        //}

        //public bool ComprobarVersionRepositorioWebCoinciden(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        //{
        //    string peticion = UrlApiIntegracionContinua + $"/integracion/correct-version-state";
        //    string requestParameters = $"User={pUsuarioID}&Project={pClaveProyectoSeleccionado}&Environment={pEntornoIntegracionContinua}";
        //    byte[] byteData = Encoding.UTF8.GetBytes(requestParameters);
        //    return bool.Parse(UtilGeneral.WebRequest("POST", peticion, byteData));
        //}

        //public static string ObtenerMascaraPropiedad(IntegracionContinuaPropiedad pPropiedad)
        //{
        //    return $"[%%%_PROPIEDADINTEGRACION_{pPropiedad.ObjetoPropiedad}|||{pPropiedad.TipoObjeto}|||{pPropiedad.TipoPropiedad}_%%%]";
        //}

        //public static string ReemplazarMascaraPropiedad(string pTexto, List<IntegracionContinuaPropiedad> pPropiedades)
        //{
        //    if (pPropiedades != null)
        //    {
        //        foreach (var propiedad in pPropiedades)
        //        {
        //            string mascara = ObtenerMascaraPropiedad(propiedad);
        //            string valorPropiedad = propiedad.MismoValor ? propiedad.ValorPropiedad : propiedad.ValorPropiedadDestino;
        //            pTexto = pTexto.Replace(mascara, valorPropiedad);
        //        }
        //    }
        //    return pTexto;
        //}

        //public List<IntegracionContinuaPropiedad> ObtenerConfiguracionParametrosImportacion(string pUrlApiDespliegues, string pNombreCorto)
        //{
        //    List<IntegracionContinuaPropiedad> propiedades = null;

        //    try
        //    {
        //        string PROPIEDADES_INTEGRACION = "PropiedadesIntegracion";

        //        string urlPeticionPropiedades = $"{ pUrlApiDespliegues }/{ PROPIEDADES_INTEGRACION }?nombreProy={pNombreCorto}";

        //        string ficheroConfiguracion = UtilGeneral.WebRequest("GET", urlPeticionPropiedades, null);

        //        propiedades = JsonConvert.DeserializeObject<List<IntegracionContinuaPropiedad>>(ficheroConfiguracion);

        //    }
        //    catch (Exception ex)
        //    {
        //        mLoggingService.GuardarLogError(ex);
        //    }

        //    return propiedades;
        //}

        //public List<IntegracionContinuaPropiedad> ObtenerModeloConfiguracionParametros(Guid pProyectoID, string pUrlApiDespliegues, string pNombreCorto)
        //{
        //    List<IntegracionContinuaPropiedad> propiedadesImportadas = ObtenerConfiguracionParametrosImportacion(pUrlApiDespliegues, pNombreCorto);

        //    ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService);
        //    List<IntegracionContinuaPropiedad> propiedadesEntornoActual = proyCN.ObtenerFilasIntegracionContinuaParametro(pProyectoID);
        //    proyCN.Dispose();

        //    if (propiedadesEntornoActual != null && propiedadesImportadas != null)
        //    {
        //        foreach (IntegracionContinuaPropiedad propiedadActual in propiedadesEntornoActual)
        //        {
        //            IntegracionContinuaPropiedad propiedadEntornoSiguiente = propiedadesImportadas.FirstOrDefault(p => p.ObjetoPropiedad.Equals(propiedadActual.ObjetoPropiedad) && p.TipoObjeto.Equals(propiedadActual.TipoObjeto) && p.TipoPropiedad.Equals(propiedadActual.TipoPropiedad));

        //            if (propiedadEntornoSiguiente == null)
        //            {
        //                propiedadActual.Revisada = false;
        //                propiedadActual.MismoValor = false;
        //                propiedadesImportadas.Add(propiedadActual);
        //            }
        //        }
        //    }
        //    else if (propiedadesEntornoActual != null && propiedadesImportadas == null)
        //    {
        //        foreach (IntegracionContinuaPropiedad propiedad in propiedadesEntornoActual)
        //        {
        //            propiedad.Revisada = false;
        //            propiedad.MismoValor = false;
        //        }
        //        return propiedadesEntornoActual;

        //    }
        //    else if (propiedadesImportadas != null && propiedadesEntornoActual == null)
        //    {
        //        return propiedadesImportadas;
        //    }
        //    else if (propiedadesEntornoActual != null && propiedadesImportadas == null)
        //    {
        //        foreach (IntegracionContinuaPropiedad propiedad in propiedadesEntornoActual)
        //        {
        //            propiedad.Revisada = false;
        //            propiedad.MismoValor = false;
        //        }
        //        return propiedadesEntornoActual;

        //    }
        //    else if (propiedadesImportadas != null && propiedadesEntornoActual == null)
        //    {
        //        return propiedadesImportadas;
        //    }

        //    return propiedadesImportadas;
        //}

        //private static HttpResponseMessage WebPost(string url, string pContent)
        //{
        //    StringContent content = new StringContent(pContent, Encoding.UTF8, "application/x-www-form-urlencoded");
        //    var response = mHttpClient.PostAsync(url, content);
        //    return response.Result;
        //}
    }
}
