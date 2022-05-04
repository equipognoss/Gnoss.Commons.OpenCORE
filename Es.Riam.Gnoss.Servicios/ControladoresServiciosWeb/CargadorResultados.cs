using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb
{
    public class CargadorResultados : ControladorBase
    {
        public ResultadoModel CargarResultadosJSON(Guid pProyectoID, Guid pIdentidadID, bool pEsUsuarioInvitado, string pUrlPaginaBusqueda, bool pUsarMasterParaLectura, bool pAdministradorVeTodasPersonas, TipoBusqueda pTipoBusqueda, string pGrafo, string pParametros_adiccionales, string pParametros, bool pPrimeraCarga, string pLanguageCode, int pNumeroParteResultados, string pFiltroContexto)
        {
            string respuesta = CargarResultadosInt(pProyectoID, pIdentidadID, pEsUsuarioInvitado, pUrlPaginaBusqueda, pUsarMasterParaLectura, pAdministradorVeTodasPersonas, pTipoBusqueda, pGrafo, pParametros_adiccionales, pParametros, pPrimeraCarga, pLanguageCode, pNumeroParteResultados, pFiltroContexto, true);
            /*JavaScriptSerializer serializador = new JavaScriptSerializer(new SimpleTypeResolver());
            serializador.MaxJsonLength = 50000000;
            ResultadoModel resultadoModel = serializador.Deserialize<ResultadoModel>(respuesta);*/
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All
            };
            ResultadoModel resultadoModel = JsonConvert.DeserializeObject<ResultadoModel>(respuesta,jsonSerializerSettings);
            return resultadoModel;
        }

        public string CargarResultados(Guid pProyectoID, Guid pIdentidadID, bool pEsUsuarioInvitado, string pUrlPaginaBusqueda, bool pUsarMasterParaLectura, bool pAdministradorVeTodasPersonas, TipoBusqueda pTipoBusqueda, string pGrafo, string pParametros_adiccionales, string pParametros, bool pPrimeraCarga, string pLanguageCode, int pNumeroParteResultados, string pFiltroContexto, Guid? pTokenAfinidad = null, HttpRequest pRequest = null)
        {
            return CargarResultadosInt(pProyectoID, pIdentidadID, pEsUsuarioInvitado, pUrlPaginaBusqueda, pUsarMasterParaLectura, pAdministradorVeTodasPersonas, pTipoBusqueda, pGrafo, pParametros_adiccionales, pParametros, pPrimeraCarga, pLanguageCode, pNumeroParteResultados, pFiltroContexto, false, pTokenAfinidad, pRequest);
        }

        private string CargarResultadosInt(Guid pProyectoID, Guid pIdentidadID, bool pEsUsuarioInvitado, string pUrlPaginaBusqueda, bool pUsarMasterParaLectura, bool pAdministradorVeTodasPersonas, TipoBusqueda pTipoBusqueda, string pGrafo, string pParametros_adiccionales, string pParametros, bool pPrimeraCarga, string pLanguageCode, int pNumeroParteResultados, string pFiltroContexto, bool pJson, Guid? pTokenAfinidad = null, HttpRequest pRequest = null)
        {
            string metodo = "CargarResultados";

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pIdentidadID", pIdentidadID.ToString());
            parametros.Add("pEsUsuarioInvitado", pEsUsuarioInvitado.ToString());
            parametros.Add("pUrlPaginaBusqueda", HttpUtility.UrlEncode(pUrlPaginaBusqueda));
            parametros.Add("pUsarMasterParaLectura", pUsarMasterParaLectura.ToString());
            parametros.Add("pAdministradorVeTodasPersonas", pAdministradorVeTodasPersonas.ToString());
            parametros.Add("pTipoBusqueda", ((short)pTipoBusqueda).ToString());
            parametros.Add("pGrafo", pGrafo);
            parametros.Add("pParametros_adiccionales", pParametros_adiccionales);
            parametros.Add("pParametros", pParametros);
            parametros.Add("pPrimeraCarga", pPrimeraCarga.ToString());
            parametros.Add("pLanguageCode", pLanguageCode);
            parametros.Add("pNumeroParteResultados",pNumeroParteResultados.ToString());
            parametros.Add("pFiltroContexto", pFiltroContexto);
            parametros.Add("pJson", pJson.ToString());
            string tokenAfinidad = string.Empty;
            if (pTokenAfinidad.HasValue)
            {
                tokenAfinidad = pTokenAfinidad.Value.ToString();
            }
            parametros.Add("tokenAfinidad", tokenAfinidad);

            return PeticionServicio(metodo, parametros, pRequest);
        }

        public List<ResourceModel> CargarResultadosContexto(Guid pProyectoID, string pParametros, bool pPrimeraCarga, string pLanguageCode, TipoBusqueda pTipoBusqueda, int pNumRecursosPagina, string pGrafo, string pUrlPaginaBusqueda, string pFiltroContexto, bool pEsBot, bool pMostrarEnlaceOriginal, string pNamespacesExtra, string pListaItemsBusqueda, string pResultadosEliminar, bool pNuevaPestanya, string pParametrosAdicionales, Guid pIdentidadID, bool pEsUsuarioInvitado)
        {            
            string metodo = "CargarResultadosContexto";

            Dictionary<string,string> parametros=new Dictionary<string,string>();
            parametros.Add("pProyectoID",pProyectoID.ToString());  
            parametros.Add("pParametros", pParametros);
            parametros.Add("pPrimeraCarga",pPrimeraCarga.ToString());
            parametros.Add("pLanguageCode", pLanguageCode);
            parametros.Add("pTipoBusqueda", ((short)pTipoBusqueda).ToString());
            parametros.Add("pNumRecursosPagina",pNumRecursosPagina.ToString());
            parametros.Add("pGrafo", pGrafo);
            //parametros.Add("pUrlPaginaBusqueda", pUrlPaginaBusqueda);
            parametros.Add("pFiltroContexto", pFiltroContexto);
            parametros.Add("pEsBot",pEsBot.ToString());
            parametros.Add("pMostrarEnlaceOriginal",pMostrarEnlaceOriginal.ToString());
            parametros.Add("pNamespacesExtra", pNamespacesExtra);           
            parametros.Add("pListaItemsBusqueda", pListaItemsBusqueda);
            parametros.Add("pResultadosEliminar", pResultadosEliminar);
            parametros.Add("pNuevaPestanya",pNuevaPestanya.ToString());
            parametros.Add("pParametrosAdicionales", pParametrosAdicionales);
            parametros.Add("pIdentidadID", pIdentidadID.ToString());
            parametros.Add("pEsUsuarioInvitado", pEsUsuarioInvitado.ToString());

            
            string respuesta = PeticionServicio(metodo, parametros);
            List<ResourceModel> listadoElementos = JsonConvert.DeserializeObject<List<ResourceModel>>(respuesta);
            return listadoElementos;
         }

        public ResultadoModel CargarResultadosGadgetJSON(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, string pParametros, bool pPrimeraCarga, string pLanguageCode, TipoBusqueda pTipoBusqueda, int pNumResultados, string pUrlPaginaBusqueda, bool pUsarMasterParaLectura, bool pObtenerDatosExtraRecursos = true, bool pObtenerIdentidades = true, bool pObtenerDatosExtraIdentidades = false)
        {
            string respuesta = CargarResultadosGadget(pProyectoID, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID, pParametros, pPrimeraCarga, pLanguageCode, pTipoBusqueda, pNumResultados, pUrlPaginaBusqueda, TipoFichaResultados.Completa, true, pUsarMasterParaLectura, pObtenerDatosExtraRecursos, pObtenerIdentidades, pObtenerDatosExtraIdentidades);
            var settings = new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects,
                Formatting = Newtonsoft.Json.Formatting.Indented
            };
            ResultadoModel resultadoModel = JsonConvert.DeserializeObject<ResultadoModel>(respuesta, settings);
            //ResultadoModel resultadoModel = System.Text.Json.JsonSerializer.Deserialize<ResultadoModel>(respuesta);
            return resultadoModel;
        }        

        public string CargarResultadosGadgetHTML(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, string pParametros, bool pPrimeraCarga, string pLanguageCode, TipoBusqueda pTipoBusqueda, int pNumResultados, string pUrlPaginaBusqueda,TipoFichaResultados pTipoFichaResultados)
        {
            return CargarResultadosGadget(pProyectoID, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID, pParametros, pPrimeraCarga, pLanguageCode, pTipoBusqueda,  pNumResultados, pUrlPaginaBusqueda, pTipoFichaResultados, false, false);
        }

        private string CargarResultadosGadget(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, string pParametros, bool pPrimeraCarga, string pLanguageCode, TipoBusqueda pTipoBusqueda, int pNumResultados, string pUrlPaginaBusqueda, TipoFichaResultados pTipoFichaResultados, bool pJson, bool pUsarMasterParaLectura, bool pObtenerDatosExtraRecursos = true, bool pObtenerIdentidades = true, bool pObtenerDatosExtraIdentidades = false)
        {
            string metodo = "CargarResultadosGadget";

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pEstaEnProyecto", pEstaEnProyecto.ToString());
            parametros.Add("pEsUsuarioInvitado", pEsUsuarioInvitado.ToString());
            parametros.Add("pIdentidadID", pIdentidadID.ToString());
            parametros.Add("pParametros", pParametros);
            parametros.Add("pPrimeraCarga", pPrimeraCarga.ToString());
            parametros.Add("pLanguageCode", pLanguageCode);
            parametros.Add("pTipoBusqueda", ((short)pTipoBusqueda).ToString());
            parametros.Add("pNumResultados", pNumResultados.ToString());
            parametros.Add("pUrlPaginaBusqueda", pUrlPaginaBusqueda);
            parametros.Add("pTipoFichaResultados", ((short)pTipoFichaResultados).ToString());
            parametros.Add("pJson", pJson.ToString());
            parametros.Add("pObtenerDatosExtraRecursos", pObtenerDatosExtraRecursos.ToString());
            parametros.Add("pObtenerIdentidades", pObtenerIdentidades.ToString());
            parametros.Add("pObtenerDatosExtraIdentidades", pObtenerDatosExtraIdentidades.ToString());
            parametros.Add("pUsarMasterParaLectura", pUsarMasterParaLectura.ToString());

            return PeticionServicio(metodo, parametros);
        }

        public ResultadoModel CargarResultadosGadgetSPARQL(string pSparql, Guid pProyectoID, int pNumItemsPag, int pNumPag,string pIdioma, bool pObtenerDatosExtraRecursos = true, bool pObtenerIdentidades = true, bool pObtenerDatosExtraIdentidades = false)
        {
            string metodo = "CargarResultadosGadgetSPARQL";

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pSPARQL", pSparql);
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pNumItemsPag", pNumItemsPag.ToString());
            parametros.Add("pNumPag", pNumPag.ToString());
            parametros.Add("pLanguageCode", pIdioma);
            parametros.Add("pObtenerDatosExtraRecursos", pObtenerDatosExtraRecursos.ToString());
            parametros.Add("pObtenerIdentidades", pObtenerIdentidades.ToString());
            parametros.Add("pObtenerDatosExtraIdentidades", pObtenerDatosExtraIdentidades.ToString());
            string respuesta = PeticionServicio(metodo, parametros);

            ResultadoModel resultadoModel = JsonConvert.DeserializeObject<ResultadoModel>(respuesta);
            return resultadoModel;
        }

        public void InvalidarVistas()
        {            
            PeticionServicio("LimpiarCache", null);
        }

        /// <summary>
        /// Método para refrescar los resultados
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pEstaEnProyecto">Booleano que indica si el usuario está en el proyecto</param>
        /// <param name="pEsUsuarioInvitado">Booleano que indica si el usuario es el invitado</param>
        /// <param name="pUbicacionBusqueda">Ubicacion de la busqueda</param>
        /// <param name="pPrimeraCarga">Booleano que indica si es la primera carga</param>
        /// <param name="pLanguageCode">Codigo del idioma</param>
        /// <param name="pTipoBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroFacetas">Número de facetas</param>
        /// <param name="pParametros_adiccionales">Parametros adicionales</param>
        /// <param name="pEsBot">Booleano que indica si es bot</param>
        /// <param name="pFaceta">Faceta</param>
        /// <returns></returns>
        public string RefrescarResultados(Guid pProyectoID, bool pEsMyGnoss, bool pEstaEnProyecto, bool pEsUsuarioInvitado, bool pPrimeraCarga, string pLanguageCode, TipoBusqueda pTipoBusqueda, int pNumeroResultados, bool pEsBot, string pParametros_adiccionales, bool pEsMovil = false)
        {
            string metodo = "RefrescarResultados";

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pEsMyGnoss", pEsMyGnoss.ToString());
            parametros.Add("pEstaEnProyecto", pEstaEnProyecto.ToString());
            parametros.Add("pEsUsuarioInvitado", pEsUsuarioInvitado.ToString());
            parametros.Add("pPrimeraCarga", pPrimeraCarga.ToString());
            parametros.Add("pLanguageCode", pLanguageCode);
            parametros.Add("pTipoBusqueda", ((short)pTipoBusqueda).ToString());
            parametros.Add("pNumeroResultados", pNumeroResultados.ToString());
            parametros.Add("pEsBot", pEsBot.ToString());
            parametros.Add("pParametros_adiccionales", pParametros_adiccionales);
            if (pEsMovil)
            {
                parametros.Add("esMovil", pEsMovil.ToString());
            }

            return PeticionServicio(metodo, parametros);
        }
    }
}

