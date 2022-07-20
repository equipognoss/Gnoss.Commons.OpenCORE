using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.Web.MVC.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;

namespace Es.Riam.Gnoss.Servicios.ControladoresServiciosWeb
{
    public class CargadorFacetas : ControladorBase
    {
        /// <summary>
        /// Método para cargar las facetas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pEstaEnProyecto">Booleano que indica si el usuario está en el proyecto</param>
        /// <param name="pEsUsuarioInvitado">Booleano que indica si el usuario es el invitado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pParametros">Parametros</param>
        /// <param name="pUbicacionBusqueda">Ubicacion de la busqueda</param>
        /// <param name="pPrimeraCarga">Booleano que indica si es la primera carga</param>
        /// <param name="pLanguageCode">Codigo del idioma</param>
        /// <param name="pAdministradorVeTodasPersonas">Booleano que indica si el administrador quiere ver todas las personas</param>
        /// <param name="pTipoBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroFacetas">Número de facetas</param>
        /// <param name="pFaceta">Faceta</param>
        /// <param name="pGrafo">Grafo de búsqueda</param>
        /// <param name="pParametros_adiccionales">Parametros adicionales</param>
        /// <param name="pFiltroContexto">Filtro contexto</param>
        /// <param name="pUrlPaginaActual">URL de la página actual</param>
        /// <param name="pUsarMasterParaLectura">Usar master para lectura</param>
        /// <returns></returns>
        public string CargarFacetas(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, string pParametros, string pUbicacionBusqueda, bool pPrimeraCarga, string pLanguageCode, bool pAdministradorVeTodasPersonas, TipoBusqueda pTipoBusqueda, int pNumeroFacetas, string pFaceta, string pGrafo, string pParametros_adiccionales, string pFiltroContexto, string pUrlPaginaActual, bool pUsarMasterParaLectura, Guid? pTokenAfinidad = null, HttpRequest pRequest = null)            
        {
            return CargarFacetas(pProyectoID, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID, pParametros, pUbicacionBusqueda, pPrimeraCarga, pLanguageCode, pAdministradorVeTodasPersonas, pTipoBusqueda, pNumeroFacetas, pFaceta, pGrafo, pParametros_adiccionales, pFiltroContexto, pUrlPaginaActual, pUsarMasterParaLectura, false, pTokenAfinidad, pRequest);
        }

        /// <summary>
        /// Método para cargar las facetas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pEstaEnProyecto">Booleano que indica si el usuario está en el proyecto</param>
        /// <param name="pEsUsuarioInvitado">Booleano que indica si el usuario es el invitado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pParametros">Parametros</param>
        /// <param name="pUbicacionBusqueda">Ubicacion de la busqueda</param>
        /// <param name="pPrimeraCarga">Booleano que indica si es la primera carga</param>
        /// <param name="pLanguageCode">Codigo del idioma</param>
        /// <param name="pAdministradorVeTodasPersonas">Booleano que indica si el administrador quiere ver todas las personas</param>
        /// <param name="pTipoBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroFacetas">Número de facetas</param>
        /// <param name="pFaceta">Faceta</param>
        /// <param name="pGrafo">Grafo de búsqueda</param>
        /// <param name="pParametros_adiccionales">Parametros adicionales</param>
        /// <param name="pFiltroContexto">Filtro contexto</param>
        /// <param name="pUrlPaginaActual">URL de la página actual</param>
        /// <param name="pUsarMasterParaLectura">Usar master para lectura</param>
        /// <returns></returns>
        public FacetedModel CargarFacetasJson(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, string pParametros, string pUbicacionBusqueda, bool pPrimeraCarga, string pLanguageCode, bool pAdministradorVeTodasPersonas, TipoBusqueda pTipoBusqueda, int pNumeroFacetas, string pFaceta, string pGrafo, string pParametros_adiccionales, string pFiltroContexto, string pUrlPaginaActual, bool pUsarMasterParaLectura, Guid? pTokenAfinidad = null)            
        {
            string respuesta = CargarFacetas(pProyectoID, pEstaEnProyecto, pEsUsuarioInvitado, pIdentidadID, pParametros, pUbicacionBusqueda, pPrimeraCarga, pLanguageCode, pAdministradorVeTodasPersonas, pTipoBusqueda, pNumeroFacetas, pFaceta, pGrafo, pParametros_adiccionales, pFiltroContexto, pUrlPaginaActual, pUsarMasterParaLectura, true, pTokenAfinidad);
            FacetedModel facetaModel = JsonConvert.DeserializeObject<FacetedModel>(respuesta);
            return facetaModel;
        }

        /// <summary>
        /// Método para cargar las facetas
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <param name="pEstaEnProyecto">Booleano que indica si el usuario está en el proyecto</param>
        /// <param name="pEsUsuarioInvitado">Booleano que indica si el usuario es el invitado</param>
        /// <param name="pIdentidadID">Identificador de la identidad</param>
        /// <param name="pParametros">Parametros</param>
        /// <param name="pUbicacionBusqueda">Ubicacion de la busqueda</param>
        /// <param name="pPrimeraCarga">Booleano que indica si es la primera carga</param>
        /// <param name="pLanguageCode">Codigo del idioma</param>
        /// <param name="pAdministradorVeTodasPersonas">Booleano que indica si el administrador quiere ver todas las personas</param>
        /// <param name="pTipoBusqueda">Tipo de busqueda</param>
        /// <param name="pNumeroFacetas">Número de facetas</param>
        /// <param name="pFaceta">Faceta</param>
        /// <param name="pGrafo">Grafo de búsqueda</param>
        /// <param name="pParametros_adiccionales">Parametros adicionales</param>
        /// <param name="pFiltroContexto">Filtro contexto</param>
        /// <param name="pUrlPaginaActual">URL de la página actual</param>
        /// <param name="pUsarMasterParaLectura">Usar master para lectura</param>
        /// <returns></returns>
        private string CargarFacetas(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, Guid pIdentidadID, string pParametros, string pUbicacionBusqueda, bool pPrimeraCarga, string pLanguageCode, bool pAdministradorVeTodasPersonas, TipoBusqueda pTipoBusqueda, int pNumeroFacetas, string pFaceta, string pGrafo, string pParametros_adiccionales, string pFiltroContexto, string pUrlPaginaActual, bool pUsarMasterParaLectura, bool pJson, Guid? pTokenAfinidad = null, HttpRequest pRequest = null)
        {
            string metodo = "CargarFacetas";

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pEstaEnProyecto", pEstaEnProyecto.ToString());
            parametros.Add("pEsUsuarioInvitado", pEsUsuarioInvitado.ToString());
            parametros.Add("pIdentidadID", pIdentidadID.ToString());
            parametros.Add("pParametros", pParametros);
            parametros.Add("pUbicacionBusqueda", pUbicacionBusqueda);
            parametros.Add("pPrimeraCarga", pPrimeraCarga.ToString());
            parametros.Add("pLanguageCode", pLanguageCode);
            parametros.Add("pAdministradorVeTodasPersonas", pAdministradorVeTodasPersonas.ToString());
            parametros.Add("pTipoBusqueda", ((short)pTipoBusqueda).ToString());
            parametros.Add("pNumeroFacetas", pNumeroFacetas.ToString());
            parametros.Add("pFaceta", pFaceta);
            parametros.Add("pGrafo", pGrafo);
            parametros.Add("pParametros_adiccionales", pParametros_adiccionales);
            parametros.Add("pFiltroContexto", pFiltroContexto);
            parametros.Add("pUrlPaginaActual", HttpUtility.UrlEncode(pUrlPaginaActual));
            parametros.Add("pUsarMasterParaLectura", pUsarMasterParaLectura.ToString());
            parametros.Add("pJson", pJson.ToString());
            string tokenAfinidad = string.Empty;
            if (pTokenAfinidad.HasValue)
            {
                tokenAfinidad = pTokenAfinidad.Value.ToString();
            }
            parametros.Add("tokenAfinidad", tokenAfinidad);

            return PeticionServicio(metodo, parametros, pRequest);
        }


        /// <summary>
        /// Método para refrescar las facetas
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
        public string RefrescarFacetas(Guid pProyectoID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, string pUbicacionBusqueda, bool pPrimeraCarga, string pLanguageCode, TipoBusqueda pTipoBusqueda, int pNumeroFacetas, string pParametros_adiccionales, bool pEsBot, string pFaceta, bool pEsMovil = false)
        {
            string metodo = "RefrescarFacetas";

            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pProyectoID", pProyectoID.ToString());
            parametros.Add("pEstaEnProyecto", pEstaEnProyecto.ToString());
            parametros.Add("pEsUsuarioInvitado", pEsUsuarioInvitado.ToString());
            parametros.Add("pUbicacionBusqueda", pUbicacionBusqueda);
            parametros.Add("pPrimeraCarga", pPrimeraCarga.ToString());
            parametros.Add("pLanguageCode", pLanguageCode);
            parametros.Add("pTipoBusqueda", ((short)pTipoBusqueda).ToString());
            parametros.Add("pNumeroFacetas", pNumeroFacetas.ToString());
            parametros.Add("pFaceta", pFaceta);
            parametros.Add("pEsBot", pEsBot.ToString());
            parametros.Add("pParametros_adiccionales", pParametros_adiccionales);
            if (pEsMovil)
            {
                parametros.Add("esMovil", pEsMovil.ToString());
            }

            return PeticionServicio(metodo, parametros);
        }

        public void InvalidarVistas(Guid pIdentidadID)
        {
            Dictionary<string, string> parametros = new Dictionary<string, string>();
            parametros.Add("pIdentidadID", pIdentidadID.ToString());
            PeticionServicio("LimpiarCache", parametros);
        }      
    }
}

