using Es.Riam.Gnoss.AD.EntityModel.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Open
{
    public class UtilServicioIntegracionContinuaOpen : IUtilServicioIntegracionContinua
    {
        public void ActualizeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string urlApiDesplieguesEntornoSiguiente, string pUrlApiDespliegue)
        {
        }

        public bool CambiosSinProcesar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return true;
        }

        public void ChangeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
        }

        public string CheckearRepositorio(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return string.Empty;
        }

        public string ComprobarAutorizacionGitBit(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlReturn, string pUrlApiIntegracionContinua)
        {
            return string.Empty;
        }

        public bool ComprobarVersionRepositorioWebCoinciden(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return false;
        }

        public void CrearVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
        }

        public void CompilarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string pVersion, string pNombreRepositorio, string pTokenRepositorio, string pNombreCortoComunidad)
        {

        }

        public void DesplegarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string pVersion, string pNombreCortoComunidad)
        {

        }

        public string ObtenerVersionWeb(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return "";
        }
        public int ObtenerEstadoWeb(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return 0;
        }

        public List<string> ObtenerListaVersionesDesplegadas(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return null;
        }

        public string ObtenerIdFrontAnsible(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return "";
        }
        public string ObtenerIdBackAnsible(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return "";
        }


        public HttpStatusCode DeployVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            return HttpStatusCode.BadRequest;
        }

        public void DescartarCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha, string pTextArea)
        {
        }

        public void DescartarConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
        }

        public void EnviarResultadoConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha)
        {
        }

        public void EnviarResultadoFusion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha)
        {
        }

        public bool EsHotfix(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return false;
        }

        public bool EsPreproduccion(string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return true;
        }

        public bool EsPruebas(string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return true;
        }

        public HttpStatusCode EstabilizarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            return HttpStatusCode.BadRequest;
        }

        public bool EstaEnBD(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return false;
        }

        public bool EstaEntornoBloqueado(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return false;
        }

        public short EstaIniciadoRepositorio(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return 0;
        }

        public bool EstaLogeado(string pNombreCorto, Guid pUsuarioID, string pUrlApiIntegracionContinua)
        {
            return false;
        }

        public bool EstaProcesando(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return false;
        }

        public bool ExistCurrentVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            return false;
        }

        public bool HayCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return false;
        }

        public bool HayConlictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return false;
        }

        public HttpStatusCode MergeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return HttpStatusCode.BadRequest;
        }

        public List<IntegracionContinuaPropiedad> ObtenerConfiguracionParametrosImportacion(string pUrlApiDespliegues, string pNombreCorto)
        {
            return null;
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosImportacion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaViewModel pModel)
        {
            return null;
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pNombreCorto)
        {
            return null;
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public AdministrarIntegracionContinuaRepositorioViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaRepositorioViewModel pModel)
        {
            return null;
        }

        public AdministrarFusionCambiosViewModel ObtenerContenidoCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            return null;
        }

        public AdministrarFusionCambiosViewModel ObtenerContenidoCambioConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            return null;
        }

        public AdministrarRamasViewModel ObtenerEstadoIntegracion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public AdministrarListaCambiosViewModel ObtenerListaCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public AdministracionConflictosCambiosViewModel ObtenerListaConflictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public string ObtenerMascaraPropiedad(IntegracionContinuaPropiedad pPropiedad)
        {
            return string.Empty;
        }

        public List<IntegracionContinuaPropiedad> ObtenerModeloConfiguracionParametros(Guid pProyectoID, string pUrlApiDespliegues, string pNombreCorto)
        {
            return null;
        }

        public string ObtenerUrlApiDesplieguesEntorno(string pNombreEntorno, string pUrlApiIntegracionContinua)
        {
            return string.Empty;
        }

        public AdministrarDesplieguesViewModel ObtenerVersionesProyecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public HttpStatusCode CompilarDll(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            return HttpStatusCode.BadRequest;
        }

        public AdministrarCredencialesViewModel PeticionApiUsuarioExiste(AdministrarCredencialesViewModel pModel, Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return null;
        }

        public string RamaEnUso(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            return string.Empty;
        }

        public string ReemplazarMascaraPropiedad(string pTexto, List<IntegracionContinuaPropiedad> pPropiedades)
        {
            return string.Empty;
        }

        public bool VerificarFechaCambioCorrecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            return false;
        }

        public bool VerificarFechaCambioCorrectoConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            return false;
        }

        public bool VersionActiva(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pVersion)
        {
            return true;
        }

        public bool VersionHotfixSinDesplegar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return false;
        }

        public string ObtenerUrlApiDesplieguesEntornoSiguiente(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID)
        {
            return string.Empty;
        }

        public string ObtenerUrlApiDesplieguesEntornoAnterior(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID)
        {
            return string.Empty;
        }

        public string ObtenerUrlApiDesplieguesEntornoParametro(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID, string pEntorno)
        {
            return string.Empty;
        }

        public string ObtenerUrlApiDesplieguesEntornoActual(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, Guid pUsuarioID)
        {
            return string.Empty;
        }

        public bool UsuarioExiste(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            return false;
        }
    }
}
