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
            throw new NotImplementedException();
        }

        public bool CambiosSinProcesar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public void ChangeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            throw new NotImplementedException();
        }

        public string CheckearRepositorio(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public string ComprobarAutorizacionGitBit(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlReturn, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool ComprobarVersionRepositorioWebCoinciden(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public void CrearVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode DeployVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            throw new NotImplementedException();
        }

        public void DescartarCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha, string pTextArea)
        {
            throw new NotImplementedException();
        }

        public void DescartarConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            throw new NotImplementedException();
        }

        public void EnviarResultadoConflicto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha)
        {
            throw new NotImplementedException();
        }

        public void EnviarResultadoFusion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, string pContenido, string pTipo, long pFecha)
        {
            throw new NotImplementedException();
        }

        public bool EsHotfix(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool EsPreproduccion(string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool EsPruebas(string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode EstabilizarVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            throw new NotImplementedException();
        }

        public bool EstaEnBD(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool EstaEntornoBloqueado(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public short EstaIniciadoRepositorio(Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool EstaLogeado(string pNombreCorto, Guid pUsuarioID, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool EstaProcesando(Guid pUsuario, Guid pProyectoSeleccionado, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool ExistCurrentVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool HayCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public bool HayConlictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode MergeVersion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public List<IntegracionContinuaPropiedad> ObtenerConfiguracionParametrosImportacion(string pUrlApiDespliegues, string pNombreCorto)
        {
            throw new NotImplementedException();
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosImportacion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaViewModel pModel)
        {
            throw new NotImplementedException();
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosInicial(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pNombreCorto)
        {
            throw new NotImplementedException();
        }

        public AdministrarIntegracionContinuaViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public AdministrarIntegracionContinuaRepositorioViewModel ObtenerConfiguracionParametrosRepositorio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, AdministrarIntegracionContinuaRepositorioViewModel pModel)
        {
            throw new NotImplementedException();
        }

        public AdministrarFusionCambiosViewModel ObtenerContenidoCambio(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            throw new NotImplementedException();
        }

        public AdministrarFusionCambiosViewModel ObtenerContenidoCambioConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            throw new NotImplementedException();
        }

        public AdministrarRamasViewModel ObtenerEstadoIntegracion(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public AdministrarListaCambiosViewModel ObtenerListaCambios(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public AdministracionConflictosCambiosViewModel ObtenerListaConflictos(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public string ObtenerMascaraPropiedad(IntegracionContinuaPropiedad pPropiedad)
        {
            throw new NotImplementedException();
        }

        public List<IntegracionContinuaPropiedad> ObtenerModeloConfiguracionParametros(Guid pProyectoID, string pUrlApiDespliegues, string pNombreCorto)
        {
            throw new NotImplementedException();
        }

        public string ObtenerUrlApiDesplieguesEntorno(string pNombreEntorno, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public AdministrarDesplieguesViewModel ObtenerVersionesProyecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public HttpStatusCode CompilarDll(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string pUrlApiIntegracionContinua, string pTagName, string pUrlApiDespliegue)
        {
            throw new NotImplementedException();
        }

        public AdministrarCredencialesViewModel PeticionApiUsuarioExiste(AdministrarCredencialesViewModel pModel, Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public string RamaEnUso(string pNombreCorto, string pUrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }

        public string ReemplazarMascaraPropiedad(string pTexto, List<IntegracionContinuaPropiedad> pPropiedades)
        {
            throw new NotImplementedException();
        }

        public bool VerificarFechaCambioCorrecto(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            throw new NotImplementedException();
        }

        public bool VerificarFechaCambioCorrectoConflict(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string FileName, string pNombreEntorno, long pFecha)
        {
            throw new NotImplementedException();
        }

        public bool VersionActiva(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua, string pVersion)
        {
            throw new NotImplementedException();
        }

        public bool VersionHotfixSinDesplegar(Guid pClaveProyectoSeleccionado, Guid pUsuarioID, string pEntornoIntegracionContinua, string UrlApiIntegracionContinua)
        {
            throw new NotImplementedException();
        }
    }
}
