using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Es.Riam.Interfaces.InterfacesOpen
{
    public interface IUtilServicioIntegracionContinua
    {
        public void ActualizeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string urlApiDesplieguesEntornoSiguiente, string pUrlApiDespliegue);
        public bool CambiosSinProcesar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public void ChangeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue);
        public bool ComprobarVersionRepositorioWebCoinciden(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public void CrearVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue);
        public void DescartarCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha, string pTextArea);
        public void DescartarConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha);
        public void EnviarResultadoConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha);
        public void EnviarResultadoFusion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha);
        public bool ExistCurrentVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public bool HayCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public bool HayConlictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public List<IntegracionContinuaPropiedad> ObtenerConfiguracionParametrosImportacion(string pUrlApiDespliegues, string pNombreCorto);
        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaViewModel pModel);
        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pNombreCorto);
        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public AdministrarFusionCambiosViewModel ObtenerContenidoCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha);
        public AdministrarFusionCambiosViewModel ObtenerContenidoCambioConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha);
        public AdministrarRamasViewModel ObtenerEstadoIntegracion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public AdministrarListaCambiosViewModel ObtenerListaCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public AdministracionConflictosCambiosViewModel ObtenerListaConflictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public List<IntegracionContinuaPropiedad> ObtenerModeloConfiguracionParametros(Guid pProyectoID, string pUrlApiDespliegues, string pNombreCorto);
        public AdministrarDesplieguesViewModel ObtenerVersionesProyecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public AdministrarCredencialesViewModel PeticionApiUsuarioExiste(AdministrarCredencialesViewModel pModel, Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public bool UsuarioExiste(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public bool VerificarFechaCambioCorrecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha);
        public bool VerificarFechaCambioCorrectoConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha);
        public bool VersionActiva(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pVersion);
        public bool VersionHotfixSinDesplegar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public string ReemplazarMascaraPropiedad(string pTexto, List<IntegracionContinuaPropiedad> pPropiedades);
        public string ObtenerMascaraPropiedad(IntegracionContinuaPropiedad pPropiedad);
        public void CompilarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string pVersion, string pNombreRepositorio, string pTokenRepositorio, string pNombreCortoComunidad);
        public void DesplegarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua,string versionWebActualizada, string pNombreCortoComunidad);
        public HttpStatusCode MergeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public HttpStatusCode DeployVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue);
        public HttpStatusCode EstabilizarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue);
        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosImportacion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua);
        public AdministrarIntegracionContinuaRepositorioViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaRepositorioViewModel pModel);
        public bool EsPreproduccion(string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public bool EsPruebas(string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public bool EstaEntornoBloqueado(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public bool EsHotfix(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public bool EstaProcesando(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public bool EstaEnBD(string pNombreCorto, string pUrlApiIntegracionContinua);
        public bool EstaLogeado(string pNombreCorto, Guid pUsuarioID, string pUrlApiIntegracionContinua);
        public short EstaIniciadoRepositorio(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public string CheckearRepositorio(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua);
        public string ComprobarAutorizacionGitBit(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlReturn, string pUrlApiIntegracionContinua);
        public string RamaEnUso(string pNombreCorto, string pUrlApiIntegracionContinua);
        public string ObtenerUrlApiDesplieguesEntorno(string pNombreEntorno, string pUrlApiIntegracionContinua);
        public string ObtenerUrlApiDesplieguesEntornoSiguiente(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID);
        public string ObtenerUrlApiDesplieguesEntornoAnterior(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID);
        public string ObtenerUrlApiDesplieguesEntornoParametro(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID, string pEntorno);
        public string ObtenerUrlApiDesplieguesEntornoActual(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID);
        public string ObtenerVersionWeb(string pNombreCorto, string pUrlApiIntegracionContinua);
        public int ObtenerEstadoWeb(string pNombreCorto, string pUrlApiIntegracionContinua);
        public List<string> ObtenerListaVersionesDesplegadas(string pNombreCorto, string pUrlApiIntegracionContinua);
        public string ObtenerIdFrontAnsible(string pNombreCorto, string pUrlApiIntegracionContinua);
        public string ObtenerIdBackAnsible(string pNombreCorto, string pUrlApiIntegracionContinua);
        public HttpStatusCode CompilarDll(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue);
    
    }
}
