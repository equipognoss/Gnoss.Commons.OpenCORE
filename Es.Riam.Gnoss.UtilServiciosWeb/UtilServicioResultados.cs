using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ParametroGeneralDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Facetado.Model;
using Es.Riam.Gnoss.AD.MetaBuscadorAD;
using Es.Riam.Gnoss.AD.Parametro;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Usuarios;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.Facetado;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.CL.ParametrosProyecto;
using Es.Riam.Gnoss.CL.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Facetado;
using Es.Riam.Gnoss.Elementos.Identidad;
using Es.Riam.Gnoss.Elementos.ParametroGeneralDSEspacio;
using Es.Riam.Gnoss.Elementos.ServiciosGenerales;
using Es.Riam.Gnoss.Elementos.Tesauro;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Logica.ServiciosGenerales;
using Es.Riam.Gnoss.Logica.Tesauro;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using System.Threading.Tasks;
using static Es.Riam.Gnoss.UtilServiciosWeb.CargadorResultadosModel;
using Es.Riam.AbstractsOpen;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{
    /// <summary>
    /// Enumeración para distinguir tipos de resultados de la busqueda
    /// </summary>
    public enum TipoResultadoBusqueda
    {
        Correcto = 0,
        PaginaMayorMil = 1,
        FiltrosNoPermitidosAusuario = 2
    }

    public class UtilServicioResultados
    {
        /// <summary>
        /// Ruta del fichero de configuracion si y solo si su fichero config es otro q el de por defecto (se utiliza para servicios con hilos)
        /// </summary>
        private string mFicheroConfiguracionBD = string.Empty;

        private VirtuosoAD mVirtuosoAD;
        private LoggingService mLoggingService;
        private EntityContext mEntityContext;
        private ConfigService mConfigService;
        private RedisCacheWrapper mRedisCacheWrapper;
        private UtilServicios mUtilServicios;
        private UtilServiciosFacetas mUtilServiciosFacetas;
        private GnossCache mGnossCache;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        public UtilServicioResultados(LoggingService loggingService, EntityContext entityContext, ConfigService configService, RedisCacheWrapper redisCacheWrapper, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mLoggingService = loggingService;
            mEntityContext = entityContext;
            mConfigService = configService;
            mRedisCacheWrapper = redisCacheWrapper;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mUtilServicios = new UtilServicios(loggingService, entityContext, configService, redisCacheWrapper, new GnossCache(entityContext, loggingService, redisCacheWrapper, configService, mServicesUtilVirtuosoAndReplication), mServicesUtilVirtuosoAndReplication);
            mUtilServiciosFacetas = new UtilServiciosFacetas(loggingService, entityContext, configService, redisCacheWrapper, virtuosoAD, mServicesUtilVirtuosoAndReplication);
            mGnossCache = new GnossCache(entityContext, loggingService, redisCacheWrapper, configService, mServicesUtilVirtuosoAndReplication);
        }

        public TipoResultadoBusqueda CargarResultadosInt(Guid pProyectoID, Guid pIdentidadID, bool pEstaEnProyecto, bool pEsUsuarioInvitado, TipoBusqueda pTipoBusqueda, string pGrafo, string pParametros, string pParametros_adiccionales, bool pPrimeraCarga, string pLanguageCode, int pNumeroParteResultados, int pNumResultados, TipoFichaResultados pTipoFichaResultados, string pFiltroContexto, bool pAdministradorVeTodasPersonas, CargadorResultadosModel pCargadorResultadosModel, bool pEsMovil, bool pBusquedaSoloIDs = false, bool pUsarAfinidad = false)
        {
            mLoggingService.AgregarEntrada("Empiezo a cargar resultados");

            if (!pProyectoID.Equals(Guid.Empty))
            {
                mUtilServicios.ComprobacionCambiosCachesLocales(ProyectoAD.MetaProyecto);
                mUtilServicios.ComprobacionCambiosCachesLocales(pProyectoID);
            }

            pCargadorResultadosModel.LanguageCode = pLanguageCode;

            //parametros
            pCargadorResultadosModel.ProyectoSeleccionado = pProyectoID;

            #region Recursos por ID

            string[] parametroslista = pParametros.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string parametro in parametroslista)
            {
                if (parametro.StartsWith("listadoRecursosEstatico:"))
                {
                    string[] idRecursos = parametro.Substring(pParametros.IndexOf(":") + 1).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    pCargadorResultadosModel.ListaIdsResultado = new Dictionary<string, TiposResultadosMetaBuscador>();
                    foreach (string idRecurso in idRecursos)
                    {
                        pCargadorResultadosModel.ListaIdsResultado.Add(idRecurso, TiposResultadosMetaBuscador.Documento);
                    }
                    pParametros = "";
                }
            }

            #endregion

            #region Parámetros adiccionales

            if (pTipoBusqueda == TipoBusqueda.VerRecursosPerfil)
            {
                string identidadid = pCargadorResultadosModel.IdentidadID.ToString();
                if (!string.IsNullOrEmpty(pGrafo))
                {
                    identidadid = pGrafo;
                }
                pCargadorResultadosModel.GrafoID = "perfil/" + identidadid;

                Guid PerfilID = new Guid(identidadid);
                TesauroCN tesauroCN = new TesauroCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                try
                {
                    AD.EntityModel.Models.IdentidadDS.Perfil perfilIdentidad = idenCN.ObtenerFilaPerfilPorID(PerfilID);
                    GestionIdentidades gestorIden = new GestionIdentidades(idenCN.ObtenerIdentidadDePerfilEnProyecto(PerfilID, ProyectoAD.MyGnoss), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

                    Guid personaID = perfilIdentidad.PersonaID.Value;
                    GestionTesauro gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroUsuarioPorPersonaID(personaID), mLoggingService, mEntityContext);
                    pParametros_adiccionales += "|skos:ConceptID=gnoss:" + gestorTesauro.TesauroDW.ListaTesauroUsuario.FirstOrDefault().CategoriaTesauroPublicoID.ToString().ToUpper() + "|gnoss:hasEstadoPP=Publicado";
                }
                catch (Exception)
                {
                    GestionTesauro gestorTesauro = new GestionTesauro(tesauroCN.ObtenerTesauroOrganizacion(PerfilID), mLoggingService, mEntityContext);
                    pParametros_adiccionales += "|skos:ConceptID=gnoss:" + gestorTesauro.TesauroDW.ListaTesauroOrganizacion.FirstOrDefault().CategoriaTesauroPublicoID.ToString().ToUpper() + "|gnoss:hasEstadoPP=Publicado";
                    GestionIdentidades gestorIden = new GestionIdentidades(idenCN.ObtenerIdentidadDeOrganizacion(PerfilID, ProyectoAD.MyGnoss, true), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);
                }
                tesauroCN.Dispose();
                idenCN.Dispose();
            }

            #region Sin caché

            if (pParametros_adiccionales.Contains("sinCache=true"))
            {
                pCargadorResultadosModel.SinCache = true;
                pParametros_adiccionales = pParametros_adiccionales.Replace("sinCache=true", "");
            }

            #endregion

            #region Sin privacidad

            if (pParametros_adiccionales.Contains("sinPrivacidad=true"))
            {
                pCargadorResultadosModel.SinPrivacidad = true;
                pParametros_adiccionales = pParametros_adiccionales.Replace("sinPrivacidad=true", "");
            }

            #endregion

            #region Sin Datos extra

            if (pParametros_adiccionales.Contains("sinDatosExtra=true"))
            {
                pCargadorResultadosModel.SinDatosExtra = true;
                pParametros_adiccionales = pParametros_adiccionales.Replace("sinDatosExtra=true", "");
            }

            #endregion

            #region IgnorarNumResultados

            if (pParametros_adiccionales.Contains("ignorarNumResultados=true"))
            {
                pCargadorResultadosModel.IgnorarNumResultados = true;
                pParametros_adiccionales = pParametros_adiccionales.Replace("ignorarNumResultados=true", "");
            }

            #endregion

            #region Es bot

            if (pParametros_adiccionales.Contains("esBot=true"))
            {
                pCargadorResultadosModel.EsBot = true;
                pParametros_adiccionales = pParametros_adiccionales.Replace("esBot=true", "");
            }

            #endregion

            #region PestanyaActual

            if (pParametros_adiccionales.Contains("PestanyaActualID="))
            {
                string trozo1 = pParametros_adiccionales.Substring(0, pParametros_adiccionales.IndexOf("PestanyaActualID="));
                string trozoRutaPestanya = pParametros_adiccionales.Substring(pParametros_adiccionales.IndexOf("PestanyaActualID="));
                string trozo2 = trozoRutaPestanya.Substring(trozoRutaPestanya.IndexOf("|") + 1);
                trozoRutaPestanya = trozoRutaPestanya.Substring(0, trozoRutaPestanya.IndexOf("|"));

                pCargadorResultadosModel.PestanyaActualID = new Guid(trozoRutaPestanya.Substring(trozoRutaPestanya.IndexOf("=") + 1));
                pParametros_adiccionales = trozo1 + trozo2;

                /*if (pCargadorResultadosModel.FilaPestanyaActual != null)
                {
                    if (pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda!=null && pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.CampoFiltro.Contains("rdf:type") && pParametros.Contains("rdf:type"))
                    {
                        string campoFiltroPestanya = pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.CampoFiltro;

                        //Eliminamos los rdf:type de parametros que no estén configurados en la pestaña (si hay rdf:type configurado en la pestanya)
                        List<string> listaTiposDePestanya = new List<string>();
                        string[] camposFiltrosPestanya = campoFiltroPestanya.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tipoPestanya in camposFiltrosPestanya)
                        {
                            string tempTipoPestanya = tipoPestanya;
                            if (tempTipoPestanya.StartsWith("("))
                            {
                                tempTipoPestanya = tempTipoPestanya.Substring(1);
                                tempTipoPestanya = tempTipoPestanya.Substring(0, tempTipoPestanya.Length - 1);
                                string[] camposSubFiltroPestanya = tempTipoPestanya.Split('@');

                                string rdfTypeCondicionado = string.Empty;
                                foreach (string campoSubFiltro in camposSubFiltroPestanya)
                                {
                                    if (campoSubFiltro.StartsWith("rdf:type") && !listaTiposDePestanya.Contains(campoSubFiltro))
                                    {
                                        listaTiposDePestanya.Add(campoSubFiltro);
                                        rdfTypeCondicionado = campoSubFiltro;
                                    }
                                    else if (pParametros.Contains(rdfTypeCondicionado))
                                    {
                                        pParametros += "|" + campoSubFiltro;
                                    }
                                }
                            }
                            else if (tempTipoPestanya.StartsWith("rdf:type") && !listaTiposDePestanya.Contains(tempTipoPestanya))
                            {
                                listaTiposDePestanya.Add(tempTipoPestanya);
                            }
                        }

                        List<string> listaTiposDeParametros = new List<string>();
                        string[] camposFiltrosParametros = pParametros.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string tipoParametro in camposFiltrosParametros)
                        {
                            if (tipoParametro.StartsWith("rdf:type") && !listaTiposDeParametros.Contains(tipoParametro))
                            {
                                listaTiposDeParametros.Add(tipoParametro);
                            }
                        }

                        foreach (string tipoParametro in listaTiposDeParametros)
                        {
                            if (!listaTiposDePestanya.Contains(tipoParametro))
                            {
                                pParametros = pParametros.Replace(tipoParametro, "");
                            }
                        }

                        // Comprobar si en Parametros Adiccionales hay 
                        // string[] camposFiltrosParametros_Adicionales = pParametros_adiccionales.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    }
                }*/
            }

            #endregion

            #region ProyectoOrigenID

            if (pParametros_adiccionales.Contains("proyectoOrigenID="))
            {
                string trozo1 = pParametros_adiccionales.Substring(0, pParametros_adiccionales.IndexOf("proyectoOrigenID="));
                string trozoProyOrgien = pParametros_adiccionales.Substring(pParametros_adiccionales.IndexOf("proyectoOrigenID="));
                string trozo2 = trozoProyOrgien.Substring(trozoProyOrgien.IndexOf("|") + 1);
                trozoProyOrgien = trozoProyOrgien.Substring(0, trozoProyOrgien.IndexOf("|"));

                pCargadorResultadosModel.ProyectoOrigenID = new Guid(trozoProyOrgien.Substring(trozoProyOrgien.IndexOf("=") + 1));
                pParametros_adiccionales = trozo1 + trozo2;
            }

            #endregion

            #region ProyectoVirtualID

            if (pParametros_adiccionales.Contains("proyectoVirtualID="))
            {
                string trozo1 = pParametros_adiccionales.Substring(0, pParametros_adiccionales.IndexOf("proyectoVirtualID="));
                string trozoProyOrgien = pParametros_adiccionales.Substring(pParametros_adiccionales.IndexOf("proyectoVirtualID="));
                string trozo2 = trozoProyOrgien.Substring(trozoProyOrgien.IndexOf("|") + 1);
                trozoProyOrgien = trozoProyOrgien.Substring(0, trozoProyOrgien.IndexOf("|"));

                pParametros_adiccionales = trozo1 + trozo2;
            }

            #endregion

            #region Busqueda tipo mapa y chart

            if (!string.IsNullOrEmpty(pParametros_adiccionales))
            {
                if (pParametros_adiccionales.Contains("busquedaTipoMapa=true"))
                {
                    pCargadorResultadosModel.BusquedaTipoMapa = true;
                    pParametros_adiccionales = pParametros_adiccionales.Replace("busquedaTipoMapa=true", "");
                }
                else if (pParametros_adiccionales.Contains("busquedaTipoChart="))
                {
                    pCargadorResultadosModel.BusquedaTipoChart = true;

                    //TFG FRAN
                    if (pParametros_adiccionales.Contains("busquedaTipoDashboardSelect"))
                    {
                        pCargadorResultadosModel.BusquedaTipoDashboard = true;

                        string trozoCon = pParametros_adiccionales.Substring(0, pParametros_adiccionales.IndexOf("busquedaTipoDashboardSelect="));
                        string trozoSelect = pParametros_adiccionales.Substring(pParametros_adiccionales.IndexOf("busquedaTipoDashboardSelect="));
                        string trozoCon2 = trozoSelect.Substring(trozoSelect.IndexOf("|") + 1);
                        trozoSelect = trozoSelect.Substring(0, trozoSelect.IndexOf("|"));

                        pCargadorResultadosModel.BusquedaTipoDashboardSelect = trozoSelect.Substring(trozoSelect.IndexOf("=") + 1);
                        pParametros_adiccionales = trozoCon + trozoCon2;

                        trozoCon = pParametros_adiccionales.Substring(0, pParametros_adiccionales.IndexOf("busquedaTipoDashboardWhere="));
                        string trozoWhere = pParametros_adiccionales.Substring(pParametros_adiccionales.IndexOf("busquedaTipoDashboardWhere="));
                        trozoCon2 = trozoWhere.Substring(trozoWhere.IndexOf("|") + 1);
                        trozoWhere = trozoWhere.Substring(0, trozoWhere.IndexOf("|"));

                        pCargadorResultadosModel.BusquedaTipoDashboardWhere = trozoWhere.Substring(trozoWhere.IndexOf("=") + 1);
                        pParametros_adiccionales = trozoCon + trozoCon2;
                    }

                    string trozo1 = pParametros_adiccionales.Substring(0, pParametros_adiccionales.IndexOf("busquedaTipoChart="));
                    string trozoProyOrgien = pParametros_adiccionales.Substring(pParametros_adiccionales.IndexOf("busquedaTipoChart="));
                    string trozo2 = trozoProyOrgien.Substring(trozoProyOrgien.IndexOf("|") + 1);
                    trozoProyOrgien = trozoProyOrgien.Substring(0, trozoProyOrgien.IndexOf("|"));

                    pCargadorResultadosModel.ChartID = new Guid(trozoProyOrgien.Substring(trozoProyOrgien.IndexOf("=") + 1));
                    pParametros_adiccionales = trozo1 + trozo2;
                }
            }

            #endregion


            #endregion

            #region Parámetros

            Guid documentoIDMapa = Guid.Empty;
            if (pParametros.Contains("default;rdf:type=") || pParametros.Contains("documentoid="))
            {
                string[] filtrosParametros = pParametros.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                pParametros = "";
                foreach (string filtro in filtrosParametros)
                {
                    if (!filtro.StartsWith("documentoid="))
                    {
                        pParametros += filtro + "|";
                    }
                    else
                    {
                        documentoIDMapa = new Guid(filtro.Substring(filtro.IndexOf("=") + 1));
                    }
                }

                filtrosParametros = pParametros.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                if (!pCargadorResultadosModel.BusquedaTipoMapa)
                {
                    //Si no es la vista mapa y se especifica un tipo por defecto solo traemos los resultados de ese tipo
                    pParametros = "";
                    foreach (string filtro in filtrosParametros)
                    {
                        if (!filtro.StartsWith("rdf:type"))
                        {
                            pParametros += filtro + "|";
                        }
                    }
                }

                pParametros = pParametros.Replace("default;rdf:type", "rdf:type");
            }


            pCargadorResultadosModel.EsMyGnoss = pCargadorResultadosModel.ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto);
            pCargadorResultadosModel.PrimeraCarga = pPrimeraCarga;
            pCargadorResultadosModel.LanguageCode = pLanguageCode;
            pCargadorResultadosModel.IdentidadID = pIdentidadID;
            pCargadorResultadosModel.EsUsuarioInvitado = pEsUsuarioInvitado;
            pCargadorResultadosModel.EstaEnProyecto = pEstaEnProyecto;
            pCargadorResultadosModel.AdministradorQuiereVerTodasLasPersonas = pAdministradorVeTodasPersonas;
            pCargadorResultadosModel.TipoBusqueda = pTipoBusqueda;
            pCargadorResultadosModel.ListaFiltrosFacetasUsuario = new Dictionary<string, List<string>>();
            pCargadorResultadosModel.NumeroParteResultados = pNumeroParteResultados;
            pCargadorResultadosModel.FilasPorPagina = pNumResultados;

            Guid? organizacionPerfilID = null;

            if (!pCargadorResultadosModel.IdentidadID.Equals(UsuarioAD.Invitado))
            {
                pCargadorResultadosModel.IdentidadActual = CargarIdentidad(pCargadorResultadosModel.IdentidadID);

                if (!pCargadorResultadosModel.IdentidadActual.PerfilID.Equals(Guid.Empty))
                {
                    pCargadorResultadosModel.PerfilIdentidadID = pCargadorResultadosModel.IdentidadActual.PerfilID;
                }
                if (pCargadorResultadosModel.IdentidadActual.OrganizacionID.HasValue)
                {
                    organizacionPerfilID = pCargadorResultadosModel.IdentidadActual.OrganizacionID.Value;
                }
            }
            else
            {
                pCargadorResultadosModel.PerfilIdentidadID = UsuarioAD.Invitado;
                pCargadorResultadosModel.UrlPerfil = "/";
            }

            if (pCargadorResultadosModel.ProyectoSeleccionado != ProyectoAD.MetaProyecto || pCargadorResultadosModel.ProyectoMyGnoss == null)
            {
                pCargadorResultadosModel.TipoProyecto = pCargadorResultadosModel.Proyecto.TipoProyecto;

                if (pCargadorResultadosModel.ProyectoSeleccionado == ProyectoAD.MetaProyecto)
                {
                    pCargadorResultadosModel.ProyectoMyGnoss = pCargadorResultadosModel.Proyecto;
                }
            }

            if (pCargadorResultadosModel.ProyectoSeleccionado == ProyectoAD.MetaProyecto)
            {
                ProyectoCN proyCN = new ProyectoCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                pCargadorResultadosModel.Proyecto.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto = pCargadorResultadosModel.Proyecto.GestorProyectos.DataWrapperProyectos.ListaOntologiaProyecto.Union(proyCN.ObtenerOntologiasPorPerfilID(pCargadorResultadosModel.PerfilIdentidadID)).ToList();
            }

            //Comprobaciones de seguridad (por si el usuario altera los parámetro de la petición)
            if (!string.IsNullOrEmpty(pFiltroContexto) && pFiltroContexto.StartsWith("\"") && pFiltroContexto.EndsWith("\""))
            {
                pFiltroContexto = pFiltroContexto.Substring(1, pFiltroContexto.Length - 2);
            }

            if (!string.IsNullOrEmpty(pFiltroContexto))
            {
                #region Configuracion contexto
                //0.- nombreFiltro
                //1.- filtroContextoSelect
                //2.- filtroContextoWhere
                //3.- filtrosOrdenes
                //4.- pesoMinimo
                string[] configFiltrosContexto = pFiltroContexto.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                pCargadorResultadosModel.FiltroContextoNombre = configFiltrosContexto[0];
                pCargadorResultadosModel.FiltroContextoSelect = configFiltrosContexto[1];
                pCargadorResultadosModel.FiltroContextoWhere = configFiltrosContexto[2];
                pCargadorResultadosModel.FiltroContextoOrderBy = configFiltrosContexto[3];
                if (configFiltrosContexto.Length > 4)
                {
                    pCargadorResultadosModel.FiltroContextoPesoMinimo = int.Parse(configFiltrosContexto[4]);
                }
                if (pCargadorResultadosModel.FiltroContextoOrderBy == "null")
                {
                    pCargadorResultadosModel.FiltroContextoOrderBy = "";
                }

                #endregion
            }

            bool ocultarResultadosSinFiltros = false;

            #region No es contexto
            if (!pCargadorResultadosModel.EsContexto)
            {
                if (pCargadorResultadosModel.ProyectoSeleccionado != ProyectoAD.MetaProyecto && pCargadorResultadosModel.Proyecto.EsCatalogo && !pCargadorResultadosModel.EsPeticionGadget && pTipoBusqueda != TipoBusqueda.PersonasYOrganizaciones)
                {
                    pCargadorResultadosModel.FilasPorPagina = 12;
                }

                if (pCargadorResultadosModel.ProyectoSeleccionado != ProyectoAD.MetaProyecto && !pCargadorResultadosModel.EsPeticionGadget)
                {
                    if (pCargadorResultadosModel.FilaPestanyaActual != null && pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda != null)
                    {
                        if (pCargadorResultadosModel.FilaPestanyaActual.PestanyaID == pCargadorResultadosModel.PestanyaActualID)
                        {
                            pCargadorResultadosModel.FilasPorPagina = pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.NumeroRecursos;
                            ocultarResultadosSinFiltros = pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.OcultarResultadosSinFiltros;
                        }
                    }
                }
            }
            if (pCargadorResultadosModel.NumeroResultadosMostrar.HasValue)
            {
                pCargadorResultadosModel.FilasPorPagina = pCargadorResultadosModel.NumeroResultadosMostrar.Value;
            }

            #endregion

            #region Extraigo los filtros
            pCargadorResultadosModel.ListaFiltros = new Dictionary<string, List<string>>();

            if ((pParametros.Contains("ordenarPor=foaf:familyName") || pParametros.Contains("ordenarPor=foaf:firstName")) && !pParametros.Contains("orden="))
            {
                pCargadorResultadosModel.FiltroOrdenDescendente = !pCargadorResultadosModel.FiltroOrdenDescendente;
            }

            Dictionary<string, List<string>> parametrosNegados = null;

            if (!string.IsNullOrEmpty(pParametros))
            {
                bool filtroOrdenDescendente = pCargadorResultadosModel.FiltroOrdenDescendente;
                int paginaActual = pCargadorResultadosModel.PaginaActual;
                string filtroOrdenadoPor = pCargadorResultadosModel.FiltroOrdenadoPor;

                string parametrosSinNegados;
                parametrosNegados = UtilServiciosFacetas.ExtraerParametrosNegados(pParametros, out parametrosSinNegados);
                pParametros = parametrosSinNegados;

                mUtilServiciosFacetas.ExtraerParametros(pCargadorResultadosModel.GestorFacetas.FacetasDW, pProyectoID, pParametros, pCargadorResultadosModel.ListaItemsBusqueda, ref filtroOrdenDescendente, ref paginaActual, ref filtroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaFiltrosFacetasUsuario, pIdentidadID, pCargadorResultadosModel.FiltrosSearchPersonalizados);

                pCargadorResultadosModel.FiltroOrdenDescendente = filtroOrdenDescendente;
                pCargadorResultadosModel.PaginaActual = paginaActual;
                pCargadorResultadosModel.FiltroOrdenadoPor = filtroOrdenadoPor;

                //nuevo Miguel
                if (pParametros.Contains("ordenarPor=") && !pParametros.Contains("orden="))
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    DataWrapperProyecto dataWrapperProyecto = proyCL.ObtenerFiltrosOrdenesDeProyecto(pCargadorResultadosModel.Proyecto.FilaProyecto.ProyectoID);
                    proyCL.Dispose();

                    if (dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Count > 0)
                    {
                        if (pParametros_adiccionales != "")
                        {
                            string pestaña = pParametros_adiccionales;
                            if (pestaña.Contains("|"))
                            {
                                char[] separadoresOrden = { '|' };
                                string[] filtrosOrden = pestaña.Split(separadoresOrden, StringSplitOptions.RemoveEmptyEntries);
                                pestaña = "";
                                string separadorOrden = "";

                                foreach (string filtro in filtrosOrden)
                                {
                                    //Si hay algun orden, lo quito de la pestaña
                                    if (!filtro.StartsWith("orden=") && !filtro.StartsWith("ordenarPor="))
                                    {
                                        pestaña += separadorOrden + filtro;
                                        separadorOrden = "|";
                                    }
                                }
                            }
                            Guid pestanyaID = pCargadorResultadosModel.PestanyaActualID;
                            if (pestanyaID == Guid.Empty)
                            {
                                foreach (ProyectoPestanyaBusqueda filaPestanya in pCargadorResultadosModel.PestanyasProyecto.ListaProyectoPestanyaBusqueda)
                                {
                                    if (filaPestanya.CampoFiltro == pestaña)
                                    {
                                        pestanyaID = filaPestanya.PestanyaID;
                                        break;
                                    }
                                }
                            }

                            ProyectoPestanyaFiltroOrdenRecursos filaFiltroOrden = null;

                            if (!string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor))
                            {
                                filaFiltroOrden = dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Where(fila => fila.PestanyaID.Equals(pestanyaID) && fila.FiltroOrden.StartsWith(pCargadorResultadosModel.FiltroOrdenadoPor)).FirstOrDefault();
                            }
                            else
                            {
                                filaFiltroOrden = dataWrapperProyecto.ListaProyectoPestanyaFiltroOrdenRecursos.Where(fila => fila.PestanyaID.Equals(pestanyaID)).FirstOrDefault();
                            }


                            if (filaFiltroOrden != null)
                            {
                                if (filaFiltroOrden.FiltroOrden.Equals(pCargadorResultadosModel.FiltroOrdenadoPor + "|asc"))
                                {
                                    pParametros += "|orden=asc";
                                    pCargadorResultadosModel.FiltroOrdenDescendente = false;
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Eliminamos de parametros adicionales los parametros q vengan en la búsqueda
            char[] separador = { '|' };
            string[] args = pParametros.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            char[] separadores = { '=' };

            List<string> filtros = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (!string.IsNullOrEmpty(args[i]))
                {
                    string[] filtro = args[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                    string key = filtro[0];
                    if (!filtros.Contains(key))
                    {
                        filtros.Add(key);
                    }
                }
            }

            //TODO JAVIER: Comprobar que estas lineas no afectan a la cache (Antiguo comentario de javi: Creo que esto no tiene sentido, lo comento por si acaso, pero algún día lo borraré.)
            string[] argsAdicionales = pParametros_adiccionales.Split(separador, StringSplitOptions.RemoveEmptyEntries);
            pParametros_adiccionales = "";
            for (int i = 0; i < argsAdicionales.Length; i++)
            {
                if (!string.IsNullOrEmpty(argsAdicionales[i]))
                {
                    string[] filtro = argsAdicionales[i].Split(separadores, StringSplitOptions.RemoveEmptyEntries);
                    string key = filtro[0];
                    string valor = filtro[1];

                    if (key.StartsWith("("))
                    {
                        key = key.Substring(1);
                        if (!filtros.Contains(key))
                        {
                            if (pParametros_adiccionales != "")
                            {
                                pParametros_adiccionales += "|";
                            }
                            pParametros_adiccionales += argsAdicionales[i];
                        }
                    }
                    else
                    {
                        if (!filtros.Contains(key))
                        {
                            if (pParametros_adiccionales != "")
                            {
                                pParametros_adiccionales += "|";
                            }
                            pParametros_adiccionales += key + "=" + valor;

                            if (filtro.Length > 2)
                            {
                                // Había algún signo = más en el parámetro adicional, lo recupero
                                for (int j = 2; j < filtro.Length; j++)
                                {
                                    pParametros_adiccionales += "=" + filtro[j];
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(pParametros_adiccionales))
            {
                bool filtroOrdenDescendente = pCargadorResultadosModel.FiltroOrdenDescendente;
                int paginaActual = pCargadorResultadosModel.PaginaActual;
                string filtroOrdenadoPor = pCargadorResultadosModel.FiltroOrdenadoPor;

                mUtilServiciosFacetas.ExtraerParametros(pCargadorResultadosModel.GestorFacetas.FacetasDW, pProyectoID, pParametros_adiccionales, pCargadorResultadosModel.ListaItemsBusqueda, ref filtroOrdenDescendente, ref paginaActual, ref filtroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaFiltrosFacetasUsuario, pIdentidadID, pCargadorResultadosModel.FiltrosSearchPersonalizados);

                pCargadorResultadosModel.FiltroOrdenDescendente = filtroOrdenDescendente;
                pCargadorResultadosModel.PaginaActual = paginaActual;
                pCargadorResultadosModel.FiltroOrdenadoPor = filtroOrdenadoPor;
            }

            if (!string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor) && pCargadorResultadosModel.FiltroOrdenadoPor.Contains("[lang]"))
            {
                string filtro = pCargadorResultadosModel.FiltroOrdenadoPor.Replace("[lang]", "");
                string condicion = $"[lang(?{FacetadoAD.QuitaPrefijo(filtro)}) = '{pCargadorResultadosModel.LanguageCode}']";
                pCargadorResultadosModel.FiltroOrdenadoPor = filtro + condicion;

            }

            #endregion

            #region Filtros
            bool esBusquedaGrafoHome = EsBusquedaMyGnossGrafoHome(pCargadorResultadosModel.TipoBusqueda);

            if (!pCargadorResultadosModel.ListaFiltros.ContainsKey("rdf:type") && (string.IsNullOrEmpty(pFiltroContexto) || (!pFiltroContexto.Contains("rdf:type ?rdftype ") && (!pFiltroContexto.Contains("rdf:type ?rdftype.")))))
            {
                switch (pCargadorResultadosModel.TipoBusqueda)
                {
                    case TipoBusqueda.Recursos:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_RECURSOS);
                        break;
                    case TipoBusqueda.Debates:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_DEBATES);
                        break;
                    case TipoBusqueda.Preguntas:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_PREGUNTAS);
                        break;
                    case TipoBusqueda.Encuestas:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_ENCUESTAS);
                        break;
                    case TipoBusqueda.Dafos:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_DAFOS);
                        break;
                    case TipoBusqueda.PersonasYOrganizaciones:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_ORGANIZACION);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CLASE);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_PERSONA);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_GRUPO);
                        break;
                    case TipoBusqueda.Comunidades:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_COMUNIDADES);
                        break;
                    case TipoBusqueda.RecomendacionesProys:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_COMUNIDADES_RECOMENDADAS);
                        break;
                    case TipoBusqueda.Blogs:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_BLOGS);
                        break;
                    case TipoBusqueda.Contribuciones:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_PUBLICADO);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPARTIDO);
                        if (pCargadorResultadosModel.EsMyGnoss)
                        {
                            pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENTARIOS);
                        }
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_RECURSOS);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_PREGUNTA);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_DEBATE);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_FACTORDAFO);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTRIBUCIONES_ENCUESTA);
                        break;
                    case TipoBusqueda.EditarRecursosPerfil:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES);
                        break;
                    case TipoBusqueda.Mensajes:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_MENSAJES);
                        break;
                    case TipoBusqueda.Comentarios:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_COMENTARIOS);
                        break;
                    case TipoBusqueda.Invitaciones:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_INVITACIONES);
                        break;
                    case TipoBusqueda.Notificaciones:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_INVITACIONES);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_COMENTARIOS);
                        break;
                    case TipoBusqueda.Suscripciones:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_SUSCRIPCIONES);
                        break;
                    case TipoBusqueda.Contactos:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTACTOS_PERSONAL);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTACTOS_ORGANIZACION);
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTACTOS_GRUPO);
                        break;
                    case TipoBusqueda.Recomendaciones:
                        pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_CONTACTOS_PERSONAL);
                        break;
                }
            }

            if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.BusquedaAvanzada && !string.IsNullOrEmpty(pFiltroContexto))
            {
                pCargadorResultadosModel.ListaItemsBusqueda.Add(FacetadoAD.BUSQUEDA_RECURSOS);
            }

            //Añado a los filtros el tipo de item por el que estamos buscando
            if (pCargadorResultadosModel.ListaItemsBusqueda.Count > 0)
            {
                pCargadorResultadosModel.ListaFiltros.Add("rdf:type", pCargadorResultadosModel.ListaItemsBusqueda);
            }
            else if (pCargadorResultadosModel.ListaFiltros.ContainsKey("rdf:type"))
            {
                foreach (string filtro in pCargadorResultadosModel.ListaFiltros["rdf:type"])
                {
                    pCargadorResultadosModel.ListaItemsBusqueda.Add(filtro);
                }
            }
            else
            {
                //Está buscando en el metabuscador, le indico al buscador facetado si se está buscando dentro de una comunidad o en MyGnoss
                if (pCargadorResultadosModel.ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto))
                {
                    pCargadorResultadosModel.ListaItemsBusqueda.Add("Mygnoss");
                }
                else
                {
                    pCargadorResultadosModel.ListaItemsBusqueda.Add("Meta");
                }
            }
            #endregion

            #region FiltroIdiomaActual

            bool noComprobarMultiidioma = pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.PersonasYOrganizaciones || (pCargadorResultadosModel.ListaFiltros.ContainsKey("rdf:type") && pCargadorResultadosModel.ListaFiltros["rdf:type"].Count == 1 && pCargadorResultadosModel.ListaFiltros["rdf:type"].Contains("Grupo"));

            if (!noComprobarMultiidioma && pCargadorResultadosModel.ParametroProyecto.ContainsKey(ParametroAD.PropiedadContenidoMultiIdioma))
            {
                string valorPropiedadContenidoMultiidioma = pCargadorResultadosModel.ParametroProyecto[ParametroAD.PropiedadContenidoMultiIdioma];

                if (valorPropiedadContenidoMultiidioma.Length > 2 && !string.Equals(valorPropiedadContenidoMultiidioma, "true", StringComparison.OrdinalIgnoreCase) && !string.Equals(valorPropiedadContenidoMultiidioma, "false", StringComparison.OrdinalIgnoreCase))
                {
                    List<string> listaIdiomas = new List<string>();
                    listaIdiomas.Add(pLanguageCode);
                    pCargadorResultadosModel.ListaFiltros.Add(pCargadorResultadosModel.ParametroProyecto[ParametroAD.PropiedadContenidoMultiIdioma], listaIdiomas);
                    pParametros_adiccionales += "|" + pCargadorResultadosModel.ParametroProyecto[ParametroAD.PropiedadContenidoMultiIdioma] + "=" + pLanguageCode;
                }
            }
            #endregion

            if (pCargadorResultadosModel.PaginaActual > 1000)
            {
                if (pCargadorResultadosModel.NumeroParteResultados == 1)
                {
                    return TipoResultadoBusqueda.PaginaMayorMil;
                }
            }


            #region Grafo y filtros
            pCargadorResultadosModel.ListaItemsBusquedaExtra = mUtilServiciosFacetas.ObtenerListaItemsBusquedaExtra(pCargadorResultadosModel.ListaFiltrosFacetasUsuario, pCargadorResultadosModel.TipoBusqueda, pCargadorResultadosModel.Proyecto.FilaProyecto.OrganizacionID, pCargadorResultadosModel.ProyectoSeleccionado);
            pCargadorResultadosModel.FormulariosSemanticos = mUtilServiciosFacetas.ObtenerFormulariosSemanticos(pCargadorResultadosModel.TipoBusqueda, pCargadorResultadosModel.Proyecto.FilaProyecto.OrganizacionID, pCargadorResultadosModel.ProyectoSeleccionado);

            //Preparo el buscador facetado
            Configuracion.ObtenerDesdeFicheroConexion = true;
            pCargadorResultadosModel.FacetadoDS = new FacetadoDS();

            string urlGrafo = mUtilServicios.UrlIntragnoss;

            if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.ArticuloBlogs)
            {
                pCargadorResultadosModel.GrafoID = "blog/" + pGrafo;
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.EditarRecursosPerfil)
            {
                string identidadid = pCargadorResultadosModel.IdentidadID.ToString();
                if (!string.IsNullOrEmpty(pGrafo))
                {
                    identidadid = pGrafo;
                }

                pCargadorResultadosModel.GrafoID = "perfil/" + identidadid;
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.VerRecursosPerfil)
            {
                string identidadid = pCargadorResultadosModel.IdentidadID.ToString();
                if (!string.IsNullOrEmpty(pGrafo))
                {
                    identidadid = pGrafo;
                }

                pCargadorResultadosModel.GrafoID = "perfil/" + identidadid;
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Contribuciones)
            {
                IdentidadCN idenCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);

                if (!string.IsNullOrEmpty(pGrafo))
                {
                    pCargadorResultadosModel.GrafoID = "contribuciones/" + pGrafo;

                    OrganizacionCN orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                    bool existeOrg = orgCN.ExisteOrganizacionPorOrganizacionID(pGrafo);

                    List<Guid> perfilIDOrganizacionID = idenCN.ObtenerPerfilyOrganizacionID(pCargadorResultadosModel.IdentidadID);

                    //Si el grafo es de una organizacion y el usuario actual está conectado con esa organización, metemos los filtros para que vea solo sus contribuciones.
                    if ((!pCargadorResultadosModel.AdministradorQuiereVerTodasLasPersonas) && existeOrg && perfilIDOrganizacionID.Count == 2 && perfilIDOrganizacionID[1].Equals(new Guid(pGrafo)))
                    {
                        List<string> listaAux = new List<string>();

                        listaAux.Add("gnoss:" + pIdentidadID.ToString().ToUpper());
                        pCargadorResultadosModel.ListaFiltros.Add("gnoss:haspublicadorIdentidadID", listaAux);
                    }
                }
                else
                {
                    pCargadorResultadosModel.GrafoID = pGrafo;
                    urlGrafo = mUtilServicios.UrlIntragnoss + "contribuciones/";
                }

                //https://riamgnoss.atlassian.net/browse/CORE-3398
                //No trae resultados de contirbuciones cuando la página de metaproyecto no corresponde con el meta proyecto mygnoss (por ejemplo comunidad unikemia)
                //if (!pCargadorResultadosModel.ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto) && !pCargadorResultadosModel.ListaFiltros.ContainsKey("sioc:has_space"))
                //{
                //    List<string> listaAux = new List<string>();

                //    listaAux.Add("gnoss:" + pProyectoID.ToString().ToUpper());
                //    pCargadorResultadosModel.ListaFiltros.Add("sioc:has_space", listaAux);
                //}
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Mensajes)
            {
                pCargadorResultadosModel.GrafoID = pGrafo;

                //Meto la identidad actual de búsqueda como filtro para que solo aparezcan mensajes de la misma:
                pCargadorResultadosModel.ListaFiltros.Add("gnoss:IdentidadID", new List<string>());
                pCargadorResultadosModel.ListaFiltros["gnoss:IdentidadID"].Add("gnoss:" + pCargadorResultadosModel.IdentidadID.ToString().ToUpper());

                if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor) || pCargadorResultadosModel.FiltroOrdenadoPor == "gnoss:relevancia")
                {
                    pCargadorResultadosModel.FiltroOrdenadoPor = "nmo:sentDate";
                }
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Comentarios)
            {
                pCargadorResultadosModel.GrafoID = pGrafo;

                if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor))
                {
                    pCargadorResultadosModel.FiltroOrdenadoPor = "dce:date";
                }
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Invitaciones || pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Suscripciones)
            {
                pCargadorResultadosModel.GrafoID = pGrafo;

                //Meto la identidad actual de búsqueda como filtro para que solo aparezcan mensajes de la misma:

                if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Invitaciones)
                {
                    pCargadorResultadosModel.ListaFiltros.Add("gnoss:IdentidadID", new List<string>());
                    pCargadorResultadosModel.ListaFiltros["gnoss:IdentidadID"].Add("gnoss:" + pCargadorResultadosModel.IdentidadID.ToString().ToUpper());
                }

                if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor))
                {
                    pCargadorResultadosModel.FiltroOrdenadoPor = "dce:date";
                }
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Notificaciones)
            {
                pCargadorResultadosModel.GrafoID = pGrafo;

                pCargadorResultadosModel.ListaFiltros.Add("Invitacion;gnoss:IdentidadID", new List<string>());
                pCargadorResultadosModel.ListaFiltros["Invitacion;gnoss:IdentidadID"].Add("gnoss:" + pCargadorResultadosModel.IdentidadID.ToString().ToUpper());


                if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor))
                {
                    pCargadorResultadosModel.FiltroOrdenadoPor = "dce:date";
                }
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Contactos)
            {
                pCargadorResultadosModel.GrafoID = "contactos/" + pGrafo;
                if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor))
                {
                    pCargadorResultadosModel.FiltroOrdenadoPor = "foaf:firstName";
                }
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Recomendaciones)
            {
                pCargadorResultadosModel.GrafoID = "contactos/" + pGrafo;

                pCargadorResultadosModel.ListaFiltros.Add("gnoss:RecPer", new List<string>());
                pCargadorResultadosModel.ListaFiltros["gnoss:RecPer"].Add("gnoss:" + pCargadorResultadosModel.IdentidadID.ToString().ToUpper());
            }
            else if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.RecomendacionesProys)
            {
                pCargadorResultadosModel.GrafoID = "contactos/" + pGrafo;
            }
            else if (!string.IsNullOrEmpty(pGrafo))
            {
                pCargadorResultadosModel.GrafoID = pGrafo.ToLower();
            }
            else
            {
                pCargadorResultadosModel.GrafoID = pProyectoID.ToString().ToLower();
            }

            if (pCargadorResultadosModel.ProyectoOrigenID != Guid.Empty)
            {
                pCargadorResultadosModel.GrafoID = pCargadorResultadosModel.ProyectoOrigenID.ToString().ToLower();
            }
            Guid grafoIDAux = Guid.Empty;
			if (pCargadorResultadosModel.GrafoID.Contains("/"))
            {
				grafoIDAux = new Guid(pCargadorResultadosModel.GrafoID.Split("/")[1]);
			}
            else
            {
				grafoIDAux = new Guid(pCargadorResultadosModel.GrafoID);
			}
            
            #endregion
            if (grafoIDAux.Equals(Guid.Empty))
            {
                pCargadorResultadosModel.GrafoID = pCargadorResultadosModel.Proyecto.Clave.ToString().ToLower();
            }

            if (esBusquedaGrafoHome)
            {
                pCargadorResultadosModel.FacetadoCL = new FacetadoCL("acidHome", "", urlGrafo, pCargadorResultadosModel.GrafoID, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                pCargadorResultadosModel.FacetadoCL = new FacetadoCL(urlGrafo, pCargadorResultadosModel.AdministradorQuiereVerTodasLasPersonas, pCargadorResultadosModel.GrafoID, true, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
            }

            if (pCargadorResultadosModel.ProyectoSeleccionado != ProyectoAD.MetaProyecto)
            {
                //PRIVACIDAD: Descomentar este código cuando se quieran volver a mostrar recursos privados y públicos de comunidades privadas
                //pCargadorResultadosModel.FacetadoCL.ListaComunidadesPrivadasUsuario = UtilServiciosFacetas.ObtenerListaComunidadesPrivadasUsuario(pCargadorResultadosModel.IdentidadID, pCargadorResultadosModel.EsUsuarioInvitado);
                pCargadorResultadosModel.FacetadoCL.ListaItemsBusquedaExtra = pCargadorResultadosModel.ListaItemsBusquedaExtra;

                DataWrapperFacetas tConfiguracionOntologia = pCargadorResultadosModel.TConfiguracionOntologia;

                tConfiguracionOntologia = new DataWrapperFacetas();

                pCargadorResultadosModel.InformacionOntologias = mUtilServiciosFacetas.ObtenerInformacionOntologias(pCargadorResultadosModel.Proyecto.FilaProyecto.OrganizacionID, pCargadorResultadosModel.ProyectoSeleccionado, tConfiguracionOntologia);

                pCargadorResultadosModel.TConfiguracionOntologia = tConfiguracionOntologia;

                pCargadorResultadosModel.FacetadoCL.InformacionOntologias = pCargadorResultadosModel.InformacionOntologias;
                pCargadorResultadosModel.FacetadoCL.PropiedadesRango = mUtilServiciosFacetas.ObtenerPropiedadesRango(pCargadorResultadosModel.GestorFacetas);
                pCargadorResultadosModel.FacetadoCL.PropiedadesFecha = mUtilServiciosFacetas.ObtenerPropiedadesFecha(pCargadorResultadosModel.GestorFacetas);
                pCargadorResultadosModel.FacetadoCL.FacetadoCN.FacetadoAD.UsarMismsaVariablesParaEntidadesEnFacetas = pCargadorResultadosModel.ParametroProyecto.ContainsKey(ParametroAD.UsarMismsaVariablesParaEntidadesEnFacetas) && pCargadorResultadosModel.ParametroProyecto[ParametroAD.UsarMismsaVariablesParaEntidadesEnFacetas].Equals("1");
            }
            pCargadorResultadosModel.FacetadoCL.FacetaDW = pCargadorResultadosModel.GestorFacetas.FacetasDW;

            if (parametrosNegados != null && parametrosNegados.Count > 0)
            {
                List<string> listaItemsBusquedaExtra = mUtilServiciosFacetas.ObtenerListaItemsBusquedaExtra(new Dictionary<string, List<string>>(), pTipoBusqueda, pCargadorResultadosModel.Proyecto.FilaProyecto.OrganizacionID, pProyectoID);

                foreach (string facetaNegada in parametrosNegados.Keys)
                {
                    string faceta = facetaNegada.Remove(0, 1);
                    List<string> parametrosAgregarFiltros = new List<string>();

                    //Obtener si es una faceta negada y se quieren obtener los recursos sin ningún valor para esta faceta
                    bool esNothing = parametrosNegados[facetaNegada].Any(item => item.Equals(FacetadoAD.NOTHING));
                    if (esNothing)
                    {
                        parametrosAgregarFiltros.Add(FacetadoAD.NOTHING);
                    }
                    else
                    {
                        FacetadoDS facetadoDS = pCargadorResultadosModel.FacetadoCL.ObtenerFaceta(faceta, pProyectoID, listaItemsBusquedaExtra, false, false, pIdentidadID.Equals(UsuarioAD.Invitado), pIdentidadID, pEsUsuarioInvitado, pCargadorResultadosModel.ListaRecursosExcluidos);

                        //Recorro sus valores y quito los que están negados
                        foreach (DataRow fila in facetadoDS.Tables[0].Rows)
                        {
                            string valor = (string)fila[0];
                            if (!parametrosNegados[facetaNegada].Contains(valor))
                            {
                                parametrosAgregarFiltros.Add(valor);
                            }
                        }
                    }

                    //añado un filtro para los valores que quedan (los que no estaban negados)
                    if (parametrosAgregarFiltros.Count > 0)
                    {
                        pCargadorResultadosModel.ListaFiltros.Add(faceta, parametrosAgregarFiltros);
                    }

                }
            }

            #region Paginación de resultados
            //En virtuoso, el límite representa el número de elementos que se traen desde el inicio (es decir, si inicio = 5; fin = 5;, se treae los elementos del 5 al 10)
            //La página carga los resultados en 2 peticiones, de 0 a 5 y de 5 a 10. 
            if (pCargadorResultadosModel.TipoBusqueda == TipoBusqueda.Mensajes)
            {
                pCargadorResultadosModel.FilasPorPagina = 20;
            }
            int inicio = (pCargadorResultadosModel.PaginaActual - 1) * pCargadorResultadosModel.FilasPorPagina;

            int limite = pCargadorResultadosModel.FilasPorPagina;

            if (pCargadorResultadosModel.NumeroParteResultados == 2)
            {
                inicio = inicio + (pCargadorResultadosModel.FilasPorPagina / 2);
            }
            else if (pCargadorResultadosModel.EsContexto)
            {
                //Es una carga de contextos                
                inicio = (pCargadorResultadosModel.PaginaActual - 1) * pCargadorResultadosModel.FilasPorPagina;
                limite = pCargadorResultadosModel.FilasPorPagina;
            }
            else if (pCargadorResultadosModel.NumeroParteResultados == -1)
            {
                //Es un bot, cargo todos los resultados de golpe
                limite = pCargadorResultadosModel.FilasPorPagina;
            }

            if (inicio == 1)
            {
                inicio = 0;
            }

            bool tieneRecursosPrivados = false;
            if (pCargadorResultadosModel.EstaEnProyecto && pCargadorResultadosModel.FilaParametroGeneral.PermitirRecursosPrivados)
            {
                //Si el usuario no tiene 
                tieneRecursosPrivados = UtilServiciosFacetas.ChequearUsuarioTieneRecursosPrivados(false, pCargadorResultadosModel.PerfilIdentidadID, pCargadorResultadosModel.TipoBusqueda, pCargadorResultadosModel.ProyectoSeleccionado, pCargadorResultadosModel.FacetadoCL);
            }

            pCargadorResultadosModel.FacetadoCL.FacetadoCN.FacetadoAD.UsuarioTieneRecursosPrivados = tieneRecursosPrivados;

            #endregion

            ObtenerFiltrosBusqueda(pCargadorResultadosModel);

            bool buscarResultados = true;
            bool soloCoordenadas = true;
            foreach (string filtro in filtros)
            {
                if (!filtro.StartsWith("eharmonise:location@@@wgs84_pos"))
                {
                    soloCoordenadas = false;
                    break;
                }
            }

            if ((pParametros == "" || soloCoordenadas) && ocultarResultadosSinFiltros)
            {
                //Si no hay filtros o son solo coordenadas y la pestaña esta configurada para que no muestre resultados 
                //cuando no hay filtros, no buscamos.
                buscarResultados = false;
            }
            else if ((!pCargadorResultadosModel.EstaEnProyecto) && (!pCargadorResultadosModel.ProyectoSeleccionado.Equals(ProyectoAD.MetaProyecto)) && (!pCargadorResultadosModel.ProyectoPrincipalUnico.Equals(pCargadorResultadosModel.ProyectoSeleccionado)))
            {
                //Una persona no puede buscar personas dentro de un proyecto del que no es miembro
                buscarResultados = (!pCargadorResultadosModel.ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ORGANIZACION) && !pCargadorResultadosModel.ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PERSONA) && !pCargadorResultadosModel.ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_ALUMNO) && !pCargadorResultadosModel.ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_CLASE) && !pCargadorResultadosModel.ListaItemsBusqueda.Contains(FacetadoAD.BUSQUEDA_PROFESOR));
            }

            bool esPrimeraCarga = (string.IsNullOrEmpty(pParametros) || pParametros.Equals("recibidos") || pParametros.Equals("enviados") || pParametros.Equals("eliminados")) && !pCargadorResultadosModel.SinCache && (parametrosNegados == null || parametrosNegados.Count == 0);
            FacetadoCL facetadoCL = new FacetadoCL(mUtilServicios.UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);

            if (esBusquedaGrafoHome)
            {
                if (pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.Mensajes) && !string.IsNullOrEmpty(pParametros))
                {
                    //facetadoCL = new FacetadoCL("acid", "bandeja", UtilServicios.UrlIntragnoss);
                    facetadoCL = new FacetadoCL("acidHome", "bandeja", mUtilServicios.UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
                else
                {
                    facetadoCL = new FacetadoCL("acidHome", "", mUtilServicios.UrlIntragnoss, mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                }
            }

            //Modifico para que cualquier cosa de mygnoss no se caché.
            bool traerDeCache = (esPrimeraCarga && !pBusquedaSoloIDs && (pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.PersonasYOrganizaciones || !pCargadorResultadosModel.AdministradorQuiereVerTodasLasPersonas)) && string.IsNullOrEmpty(pCargadorResultadosModel.FiltroContextoWhere) && pCargadorResultadosModel.ProyectoOrigenID == Guid.Empty;
            traerDeCache = traerDeCache && (pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.Contribuciones && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.Comentarios && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.Invitaciones && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.Notificaciones && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.Contactos && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.Suscripciones && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.VerRecursosPerfil && pCargadorResultadosModel.TipoBusqueda != TipoBusqueda.EditarRecursosPerfil);

            if (pCargadorResultadosModel.BusquedaTipoMapa)
            {
                // Si estamos en un mapa, siempre hay que ir a virtuoso para trernos las coordenadas
                buscarResultados = true;
                traerDeCache = false;
            }

            if (buscarResultados)
            {
                if (pCargadorResultadosModel.ListaIdsResultado == null)
                {
                    if (!pCargadorResultadosModel.EsRefrescoCache && traerDeCache && (pCargadorResultadosModel.ProyectoSeleccionado != ProyectoAD.MyGnoss || pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.Mensajes)))
                    {
                        string numPag = pCargadorResultadosModel.NumeroParteResultados.ToString();

                        if (pCargadorResultadosModel.BusquedaTipoMapa)
                        {
                            numPag = "mapaview";
                        }
                        else if (pCargadorResultadosModel.BusquedaTipoChart)
                        {
                            numPag = pCargadorResultadosModel.ChartID.ToString();
                        }

                        //Obtengo los resultados de la caché
                        if (pParametros_adiccionales != "")
                        {
                            Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> resultado = facetadoCL.ObtenerListaResultadosDeBusquedaEnProyecto(pCargadorResultadosModel.ProyectoSeleccionado, pParametros_adiccionales, pCargadorResultadosModel.PerfilIdentidadID, numPag, organizacionPerfilID, !pCargadorResultadosModel.EstaEnProyecto, pParametros, pEsMovil, pEsUsuarioInvitado);
                            if (resultado != null)
                            {
                                pCargadorResultadosModel.NumeroResultados = resultado.Item1;
                                pCargadorResultadosModel.ListaIdsResultado = resultado.Item2;
                            }
                        }
                        else
                        {
                            Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> resultado = facetadoCL.ObtenerListaResultadosDeBusquedaEnProyecto(pCargadorResultadosModel.ProyectoSeleccionado, FacetadoAD.TipoBusquedaToString(pCargadorResultadosModel.TipoBusqueda), pCargadorResultadosModel.PerfilIdentidadID, numPag, organizacionPerfilID, !pCargadorResultadosModel.EstaEnProyecto, pParametros, pEsMovil, pEsUsuarioInvitado);
                            if (resultado != null)
                            {
                                pCargadorResultadosModel.NumeroResultados = resultado.Item1;
                                pCargadorResultadosModel.ListaIdsResultado = resultado.Item2;
                            }
                        }
                    }

                    bool permitirBusqueda = true;
                    if (!pCargadorResultadosModel.ProyectoOrigenID.Equals(Guid.Empty))
                    {
                        //No permitimos la busqueda si el proyectoorigenid es privado o reservado
                        ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                        TipoAcceso tipoAcceso = proyCL.ObtenerTipoAccesoProyecto(pCargadorResultadosModel.ProyectoOrigenID);
                        if (tipoAcceso.Equals(TipoAcceso.Privado) || tipoAcceso.Equals(TipoAcceso.Reservado))
                        {
                            permitirBusqueda = false;
                        }
                    }

                    if (permitirBusqueda)
                    {
                        if (pCargadorResultadosModel.ListaIdsResultado == null || pCargadorResultadosModel.ListaIdsResultado.Count == 0 || !esPrimeraCarga || pCargadorResultadosModel.AdministradorQuiereVerTodasLasPersonas)
                        {
                            //Si la caché no estaba cargada, voy a virtuoso para obtener los id's de los resultados a montar
                            pCargadorResultadosModel.FacetadoDS = new FacetadoDS();
                            BuscarResultadosEnVirtuoso(inicio, limite, pCargadorResultadosModel, pEsMovil, pUsarAfinidad);
                        }
                    }
                }
            }

            if (pCargadorResultadosModel.ListaIdsResultado == null || pCargadorResultadosModel.ListaIdsResultado.Count == 0)
            {
                if (!pCargadorResultadosModel.BusquedaTipoChart && pCargadorResultadosModel.FacetadoDS != null)
                {
                    if (pCargadorResultadosModel.FacetadoDS.Tables.Contains("NResultadosBusqueda"))
                    {
                        //Número total de resultados de la búsqueda, sólo se obtienen en la primera petición
                        object numeroResultados = pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows[0][0];
                        if (numeroResultados is long)
                        {
                            pCargadorResultadosModel.NumeroResultados = (int)(long)numeroResultados;
                        }
                        else if (numeroResultados is int)
                        {
                            pCargadorResultadosModel.NumeroResultados = (int)numeroResultados;
                        }
                        else if (numeroResultados is string)
                        {
                            int numeroResultadosInt;
                            int.TryParse((string)numeroResultados, out numeroResultadosInt);
                            pCargadorResultadosModel.NumeroResultados = numeroResultadosInt;
                        }
                    }

                    //carga una lista con cada ID y el tipo del elemento
                    if (pCargadorResultadosModel.ListaIdsResultado == null)
                    {
                        pCargadorResultadosModel.ObtenerListaID();
                    }
                }

                pCargadorResultadosModel.FacetadoCL.Dispose();

                if (traerDeCache)
                {
                    string numPag = pCargadorResultadosModel.NumeroParteResultados.ToString();

                    if (pCargadorResultadosModel.BusquedaTipoMapa)
                    {
                        numPag = "mapaview";
                    }
                    else if (pCargadorResultadosModel.BusquedaTipoChart)
                    {
                        numPag = pCargadorResultadosModel.ChartID.ToString();
                    }

                    //Agrego los resultados a la caché 
                    if (pParametros_adiccionales != "")
                    {
                        Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> resultado = new Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>>(pCargadorResultadosModel.NumeroResultados, pCargadorResultadosModel.ListaIdsResultado);
                        facetadoCL.AgregarListaResultadosDeBusquedaEnProyecto(resultado, pParametros_adiccionales, pCargadorResultadosModel.PerfilIdentidadID, pCargadorResultadosModel.ProyectoSeleccionado, numPag, organizacionPerfilID, !pCargadorResultadosModel.EstaEnProyecto, pParametros, pEsMovil, pEsUsuarioInvitado);
                    }
                    else
                    {
                        if (pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.Mensajes))
                        {
                            Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> resultado = new Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>>(pCargadorResultadosModel.NumeroResultados, pCargadorResultadosModel.ListaIdsResultado);
                            facetadoCL.AgregarListaResultadosDeBusquedaEnProyecto(resultado, FacetadoAD.TipoBusquedaToString(pCargadorResultadosModel.TipoBusqueda), pCargadorResultadosModel.PerfilIdentidadID, pCargadorResultadosModel.ProyectoSeleccionado, numPag, organizacionPerfilID, !pCargadorResultadosModel.EstaEnProyecto, pParametros, 86400, false, pEsMovil, pEsUsuarioInvitado);
                        }
                        else
                        {
                            Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>> resultado = new Tuple<int, Dictionary<string, TiposResultadosMetaBuscador>>(pCargadorResultadosModel.NumeroResultados, pCargadorResultadosModel.ListaIdsResultado);
                            facetadoCL.AgregarListaResultadosDeBusquedaEnProyecto(resultado, FacetadoAD.TipoBusquedaToString(pCargadorResultadosModel.TipoBusqueda), pCargadorResultadosModel.PerfilIdentidadID, pCargadorResultadosModel.ProyectoSeleccionado, numPag, organizacionPerfilID, !pCargadorResultadosModel.EstaEnProyecto, pParametros, pEsMovil, pEsUsuarioInvitado);
                        }
                    }
                }
            }

            mLoggingService.AgregarEntrada("Resultados cargados");
            return TipoResultadoBusqueda.Correcto;
        }

        public static void ProcesarFichasRecursoParaPresentacion(Dictionary<Guid, ResourceModel> pFichasRecurso)
        {
            foreach (ResourceModel ficha in pFichasRecurso.Values)
            {
                ProcesarFichaRecursoParaPresentacion(ficha);
            }
        }

        public static void ProcesarFichaRecursoParaPresentacion(ResourceModel pFichaRecurso)
        {
            pFichaRecurso.Description = UtilCadenas.EliminarVideosEImagenesDeHtml(pFichaRecurso.Description);
        }

        /// <summary>
        /// Carga una identidad cuya clave se pasa por parametro
        /// </summary>
        /// <param name="pIdentidad">Identificador de la identidad que se desea cargar</param>
        /// <returns>Devuelve la identidad cuya clave se pasa por parametro</returns>
        public Identidad CargarIdentidad(Guid pIdentidad)
        {
            IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            DataWrapperIdentidad dataWrapperIdentidad = identidadCN.ObtenerIdentidadPorIDCargaLigeraTablas(pIdentidad);
            identidadCN.Dispose();

            DataWrapperPersona dataWrapperPersona = new DataWrapperPersona();

            DataWrapperOrganizacion organizacionDW = new DataWrapperOrganizacion();

            ObtenerPersonasYOrgDeIdentidades(dataWrapperIdentidad, dataWrapperPersona, organizacionDW, true);

            GestionIdentidades gestorIdentidades = new GestionIdentidades(dataWrapperIdentidad, new GestionPersonas(dataWrapperPersona, mLoggingService, mEntityContext), new GestionOrganizaciones(organizacionDW, mLoggingService, mEntityContext), mLoggingService, mEntityContext, mConfigService, mServicesUtilVirtuosoAndReplication);

            return gestorIdentidades.ListaIdentidades[pIdentidad];
        }

        /// <summary>
        /// Carga los dataset de persona y organizaciones a partir de un dataset de identidades
        /// </summary>
        /// <param name="pDataWrapperIdentidad">Dataset de identidades</param>
        /// <param name="pDataWarpperPersona">Dataset de personas que queremos cargar</param>
        /// <param name="pOrganizacionDW">Dataset de organizaciones que queremos cargar</param>
        /// <param name="pCargaLigera">TRUE para usar cargas ligeras, FALSE en caso contrario</param>
        public void ObtenerPersonasYOrgDeIdentidades(DataWrapperIdentidad pDataWrapperIdentidad, DataWrapperPersona pDataWarpperPersona, DataWrapperOrganizacion pOrganizacionDW, bool pCargaLigera)
        {
            List<Guid> listaOrganizaciones = new List<Guid>();
            List<Guid> listaPersonas = new List<Guid>();
            OrganizacionCN orgCN;
            PersonaCN persCN;

            foreach (AD.EntityModel.Models.IdentidadDS.Perfil fila in pDataWrapperIdentidad.ListaPerfil)
            {
                if (fila.OrganizacionID.HasValue && !listaOrganizaciones.Contains(fila.OrganizacionID.Value))
                {
                    listaOrganizaciones.Add(fila.OrganizacionID.Value);
                }
                if (fila.PersonaID.HasValue && !listaPersonas.Contains(fila.PersonaID.Value))
                {
                    listaPersonas.Add(fila.PersonaID.Value);
                }
            }

            if (mFicheroConfiguracionBD == string.Empty)
            {
                orgCN = new OrganizacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                persCN = new PersonaCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }
            else
            {
                orgCN = new OrganizacionCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
                persCN = new PersonaCN(mFicheroConfiguracionBD, mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication);
            }

            if (!pCargaLigera)
            {
                pOrganizacionDW.Merge(orgCN.ObtenerOrganizacionesPorID(listaOrganizaciones));
                pDataWarpperPersona.Merge(persCN.ObtenerPersonasPorID(listaPersonas));
            }
            else
            {
                pOrganizacionDW.Merge(orgCN.ObtenerOrganizacionesPorIDCargaLigera(listaOrganizaciones));
                pDataWarpperPersona.ListaPersona = pDataWarpperPersona.ListaPersona.Union(persCN.ObtenerPersonasPorIDCargaLigera(listaPersonas)).ToList();
            }

            persCN.Dispose();
            orgCN.Dispose();
        }

        /// <summary>
        /// Obtiene los filtros de la búsqueda que no vienen como parametros (ProyectoID, tipos de resultados...)
        /// </summary>
        public static void ObtenerFiltrosBusqueda(CargadorResultadosModel pCargadorResultadosModel)
        {
            List<string> comunidades = new List<string>();
            if (!pCargadorResultadosModel.ProyectoSeleccionado.Equals(ProyectoAD.MyGnoss))
            {
                comunidades.Add("gnoss:" + pCargadorResultadosModel.ProyectoSeleccionado.ToString().ToUpper());
            }

            if (pCargadorResultadosModel.ListaFiltros.ContainsKey("sioc:has_space"))
            {
                pCargadorResultadosModel.ListaFiltros["sioc:has_space"].AddRange(comunidades);
            }

            List<string> tipoitem = new List<string>();
            foreach (string item in pCargadorResultadosModel.ListaItemsBusqueda)
            {
                if (!tipoitem.Contains(item))
                {
                    tipoitem.Add(item);
                }
            }

            if (pCargadorResultadosModel.ListaFiltros.ContainsKey("rdf:type"))
            {
                foreach (string tipo in tipoitem)
                {
                    if (!pCargadorResultadosModel.ListaFiltros["rdf:type"].Contains(tipo))
                    {
                        pCargadorResultadosModel.ListaFiltros["rdf:type"].Add(tipo);
                    }
                }
            }
            else if (!tipoitem.Contains("Mygnoss"))
            {
                pCargadorResultadosModel.ListaFiltros.Add("rdf:type", tipoitem);
            }
        }

        private static bool EsBusquedaMyGnossGrafoHome(TipoBusqueda pTipoBusqueda)
        {
            return pTipoBusqueda.Equals(TipoBusqueda.Mensajes) || pTipoBusqueda.Equals(TipoBusqueda.Comentarios) || pTipoBusqueda.Equals(TipoBusqueda.Invitaciones) || pTipoBusqueda.Equals(TipoBusqueda.Notificaciones) || pTipoBusqueda.Equals(TipoBusqueda.Suscripciones);
        }

        #region Métodos de búsqueda

        /// <summary>
        /// Obtiene de virtuoso los ID's de los elementos que corresponen a esta búsqueda
        /// </summary>
        /// <param name="pInicio">Inicio de la paginación</param>
        /// <param name="pLimite">Máximo de elementos a traer</param>
        public void BuscarResultadosEnVirtuoso(int pInicio, int pLimite, CargadorResultadosModel pCargadorResultadosModel, bool pEsMovil, bool pUsarAfinidad = false)
        {
            bool ignorarPrivacidadPorPestanya = false;
            //pCargadorResultadosModel.FilaPestanyaActual != null && pCargadorResultadosModel.FilaPestanyaActual.GetProyectoPestanyaBusquedaRows().Length == 1
            if (pCargadorResultadosModel.FilaPestanyaActual != null && pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda != null)
            {
                ignorarPrivacidadPorPestanya = pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.IgnorarPrivacidadEnBusqueda;
            }

            bool permitirRecursosPrivados = pCargadorResultadosModel.FilaParametroGeneral.PermitirRecursosPrivados && !pCargadorResultadosModel.SinPrivacidad && !ignorarPrivacidadPorPestanya;
            bool usarHilos = pCargadorResultadosModel.ParametrosAplicacionDS.Any(item => item.Parametro.Equals(TiposParametrosAplicacion.UsarHilosEnFacetas) && item.Valor.Equals("1"));
            List<Task> listaTareasResultados = new List<Task>();

            pCargadorResultadosModel.FacetadoCL.DiccionarioFacetasExcluyentes = ObtenerDiccionarioFacetasExcluyentes(pCargadorResultadosModel);

            foreach (string clave in pCargadorResultadosModel.FiltrosSearchPersonalizados.Keys.ToList())
            {
                if (pCargadorResultadosModel.FiltrosSearchPersonalizados[clave].Item1.Equals("redis"))
                {
                    if (pCargadorResultadosModel.ListaFiltros.ContainsKey(clave))
                    {
                        string claveCache = pCargadorResultadosModel.ListaFiltros[clave].FirstOrDefault();
                        if (!string.IsNullOrEmpty(claveCache))
                        {
                            ConsultaCacheModelSerializable consultaCache = mGnossCache.ObtenerDeCache($"{pCargadorResultadosModel.Proyecto.Clave}_{claveCache}", true) as ConsultaCacheModelSerializable;
                            pCargadorResultadosModel.FiltrosSearchPersonalizados.Remove(clave);
                            if (consultaCache != null)
                            {
                                Dictionary<string, Tuple<string, string, string, bool>> diccionarioAuxiliar = new Dictionary<string, Tuple<string, string, string, bool>>();
                                Tuple<string, string, string, bool> tuplaAuxiliar = new Tuple<string, string, string, bool>(consultaCache.WhereSPARQL, consultaCache.OrderBy, consultaCache.WhereFacetasSPARQL, consultaCache.OmitirRdfType);

                                pCargadorResultadosModel.FiltrosSearchPersonalizados.Add(clave, tuplaAuxiliar);
                            }
                        }
                    }
                }
            }

            TiposAlgoritmoTransformacion tiposAlgoritmoTransformacion = ObtenerAlogritmosTransofmracionRangosEnFiltros(pCargadorResultadosModel);
            pCargadorResultadosModel.FacetadoCL.MandatoryRelacion = CalcularMandatoryRelacion(pCargadorResultadosModel);
            bool omitirPalabrasNoRelevantesSearch = true;

            if ((string.IsNullOrEmpty(pCargadorResultadosModel.FiltroOrdenadoPor)) && (pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.PersonasYOrganizaciones) || pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.Blogs) || pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.ArticuloBlogs) || pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.Comunidades)))
            {
                pCargadorResultadosModel.FiltroOrdenadoPor = "gnoss:hasPopularidad";
            }

            if (pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.RecomendacionesProys))
            {
                #region Recomendaciones de proyectos

                pCargadorResultadosModel.FacetadoDS.Merge(pCargadorResultadosModel.FacetadoCL.ComunidadesQueTePuedanInteresar(pCargadorResultadosModel.IdentidadID, pInicio, pLimite, true, pCargadorResultadosModel.ListaFiltros));

                if (pCargadorResultadosModel.NumeroParteResultados == 1 || pCargadorResultadosModel.NumeroParteResultados == -1)
                {
                    pCargadorResultadosModel.FacetadoDS.Merge(pCargadorResultadosModel.FacetadoCL.NumeroComunidadesQueTePuedanInteresar(pCargadorResultadosModel.IdentidadID, pCargadorResultadosModel.ListaFiltros));

                    pCargadorResultadosModel.NumeroResultados = 0;
                    if ((pCargadorResultadosModel.FacetadoDS.Tables.Contains("NResultadosBusqueda")) && (pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
                    {
                        pCargadorResultadosModel.NumeroResultados = int.Parse(pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows[0][0].ToString());
                    }
                }

                return;

                #endregion
            }
            else if (pCargadorResultadosModel.BusquedaTipoMapa)
            {
                pCargadorResultadosModel.FacetadoCL.ObtenerResultadosBusquedaFormatoMapa(pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, mUtilServiciosFacetas.ObtenerDataSetConsultaMapaProyecto(pCargadorResultadosModel.Proyecto.FilaProyecto.OrganizacionID, pCargadorResultadosModel.ProyectoSeleccionado, pCargadorResultadosModel.TipoBusqueda), permitirRecursosPrivados, pCargadorResultadosModel.TipoBusqueda, pEsMovil, pCargadorResultadosModel.FiltrosSearchPersonalizados, pCargadorResultadosModel.Proyecto.Clave);
                pCargadorResultadosModel.NumeroResultados = pCargadorResultadosModel.FacetadoDS.Tables["RecursosBusqueda"].Rows.Count;
            }
            else if (pCargadorResultadosModel.TipoBusqueda.Equals(TipoBusqueda.PersonasYOrganizaciones))
            {
                //Buscar personas
                pCargadorResultadosModel.FacetadoCL.ObtienePersonasExacto(pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.FiltroOrdenDescendente, pCargadorResultadosModel.FiltroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID, pInicio, pLimite);
            }
            else if (pCargadorResultadosModel.BusquedaTipoChart)
            {
                //TFG FRAN
                if (pCargadorResultadosModel.BusquedaTipoDashboard)
                {
                    pCargadorResultadosModel.SelectFiltroChart = new KeyValuePair<string, string>(pCargadorResultadosModel.BusquedaTipoDashboardSelect, pCargadorResultadosModel.BusquedaTipoDashboardWhere);
                }
                else
                {
                    pCargadorResultadosModel.SelectFiltroChart = mUtilServiciosFacetas.ObtenerSelectYFiltroConsultaChartProyecto(pCargadorResultadosModel.Proyecto.FilaProyecto.OrganizacionID, pCargadorResultadosModel.ProyectoSeleccionado, pCargadorResultadosModel.ChartID, pCargadorResultadosModel.LanguageCode, mVirtuosoAD);
                }
                pCargadorResultadosModel.FacetadoCL.ObtenerResultadosBusquedaFormatoChart(pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, pCargadorResultadosModel.SelectFiltroChart.Key, pCargadorResultadosModel.SelectFiltroChart.Value, permitirRecursosPrivados, pEsMovil, pCargadorResultadosModel.FiltrosSearchPersonalizados);
                pCargadorResultadosModel.NumeroResultados = pCargadorResultadosModel.FacetadoDS.Tables["RecursosBusqueda"].Rows.Count;
            }
            else
            {
                bool recursosCargados = false;
                bool searchPaginadoSinResultados = false;
                bool existe = pCargadorResultadosModel.FacetadoCL.ExisteResultadosBusquedaCache(pCargadorResultadosModel.FiltroOrdenDescendente, pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.FiltroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pInicio, pLimite, pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.FiltroContextoPesoMinimo, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, permitirRecursosPrivados, false, tiposAlgoritmoTransformacion, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil);

                if (usarHilos && !existe)
                {
                    listaTareasResultados.Add(Task.Factory.StartNew(() =>
                    {
                        if (pCargadorResultadosModel.ListaFiltros.ContainsKey("search"))
                        {
                            pCargadorResultadosModel.FacetadoCL.ObtenerResultadosBusqueda(pCargadorResultadosModel.FiltroOrdenDescendente, pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.FiltroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pInicio, pLimite, pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.FiltroContextoPesoMinimo, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, permitirRecursosPrivados, false, tiposAlgoritmoTransformacion, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil, pCargadorResultadosModel.ListaRecursosExcluidos, pUsarAfinidad);

                            //Si la búsqueda obtiene resutlados salimos del bucle
                            if (pCargadorResultadosModel.FacetadoDS.Tables["RecursosBusqueda"].Rows.Count > 0)
                            {
                                recursosCargados = true;
                                omitirPalabrasNoRelevantesSearch = false;
                            }

                            if (!recursosCargados && pInicio != 0)
                            {
                                searchPaginadoSinResultados = true;
                            }
                        }

                        if (!recursosCargados && !searchPaginadoSinResultados)
                        {
                            //Buscar cualquier otro tipo de resultado
                            pCargadorResultadosModel.FacetadoCL.ObtenerResultadosBusqueda(pCargadorResultadosModel.FiltroOrdenDescendente, pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.FiltroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pInicio, pLimite, pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.FiltroContextoPesoMinimo, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, permitirRecursosPrivados, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil, pCargadorResultadosModel.ListaRecursosExcluidos, pUsarAfinidad);
                        }
                    }));
                }
                else
                {
                    if (pCargadorResultadosModel.ListaFiltros.ContainsKey("search"))
                    {
                        //Intenta hacer la consulta sin omitir palabras irrelevantes
                        pCargadorResultadosModel.FacetadoCL.ObtenerResultadosBusqueda(pCargadorResultadosModel.FiltroOrdenDescendente, pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.FiltroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pInicio, pLimite, pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.FiltroContextoPesoMinimo, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, permitirRecursosPrivados, false, tiposAlgoritmoTransformacion, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil, pCargadorResultadosModel.ListaRecursosExcluidos, pUsarAfinidad);

                        //Si la búsqueda obtiene resutlados salimos del bucle
                        if (pCargadorResultadosModel.FacetadoDS.Tables["RecursosBusqueda"].Rows.Count > 0)
                        {
                            recursosCargados = true;
                            omitirPalabrasNoRelevantesSearch = false;
                        }

                        if (!recursosCargados && pInicio != 0)
                        {
                            searchPaginadoSinResultados = true;
                        }
                    }

                    if (!recursosCargados && !searchPaginadoSinResultados)
                    {
                        //Buscar cualquier otro tipo de resultado
                        pCargadorResultadosModel.FacetadoCL.ObtenerResultadosBusqueda(pCargadorResultadosModel.FiltroOrdenDescendente, pCargadorResultadosModel.FacetadoDS, pCargadorResultadosModel.FiltroOrdenadoPor, pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pInicio, pLimite, pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoSelect, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.FiltroContextoOrderBy, pCargadorResultadosModel.FiltroContextoPesoMinimo, pCargadorResultadosModel.TipoProyecto, pCargadorResultadosModel.NamespacesExtra, pCargadorResultadosModel.ResultadosEliminar, permitirRecursosPrivados, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil, pCargadorResultadosModel.ListaRecursosExcluidos, pUsarAfinidad);
                    }
                }
            }

            if (!pCargadorResultadosModel.IgnorarNumResultados && (pCargadorResultadosModel.NumeroParteResultados == 1 || pCargadorResultadosModel.NumeroParteResultados == -1) && !pCargadorResultadosModel.BusquedaTipoMapa && !pCargadorResultadosModel.BusquedaTipoChart)
            {
                bool existe = pCargadorResultadosModel.FacetadoCL.ExisteNumeroResultadosEnCache(pCargadorResultadosModel.FacetadoDS, "RecursosBusqueda", pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.TipoProyecto, permitirRecursosPrivados, omitirPalabrasNoRelevantesSearch, tiposAlgoritmoTransformacion, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil);

                if (usarHilos && !existe)
                {
                    listaTareasResultados.Add(Task.Factory.StartNew(() =>
                    {
                        if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroContextoNombre) || pCargadorResultadosModel.NumeroParteResultados != -1)
                        {
                            //Obtengo el número total de resultados de la búsqueda
                            pCargadorResultadosModel.FacetadoCL.ObtieneNumeroResultados(pCargadorResultadosModel.FacetadoDS, "RecursosBusqueda", pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.TipoProyecto, permitirRecursosPrivados, omitirPalabrasNoRelevantesSearch, tiposAlgoritmoTransformacion, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil, pCargadorResultadosModel.ListaRecursosExcluidos);

                            pCargadorResultadosModel.NumeroResultados = 0;
                            if ((pCargadorResultadosModel.FacetadoDS.Tables.Contains("NResultadosBusqueda")) && (pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
                            {
                                object numeroResultados = pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows[0][0];
                                if (numeroResultados is long)
                                {
                                    pCargadorResultadosModel.NumeroResultados = (int)(long)numeroResultados;
                                }
                                else if (numeroResultados is int)
                                {
                                    pCargadorResultadosModel.NumeroResultados = (int)numeroResultados;
                                }
                                else if (numeroResultados is string)
                                {
                                    int numeroResultadosInt;
                                    int.TryParse((string)numeroResultados, out numeroResultadosInt);
                                    pCargadorResultadosModel.NumeroResultados = numeroResultadosInt;
                                }
                            }
                        }
                    }));
                }
                else
                {
                    if (string.IsNullOrEmpty(pCargadorResultadosModel.FiltroContextoNombre) || pCargadorResultadosModel.NumeroParteResultados != -1)
                    {
                        //Obtengo el número total de resultados de la búsqueda
                        pCargadorResultadosModel.FacetadoCL.ObtieneNumeroResultados(pCargadorResultadosModel.FacetadoDS, "RecursosBusqueda", pCargadorResultadosModel.ListaFiltros, pCargadorResultadosModel.ListaItemsBusquedaExtra, pCargadorResultadosModel.EsMyGnoss, pCargadorResultadosModel.EstaEnProyecto, pCargadorResultadosModel.EsUsuarioInvitado, pCargadorResultadosModel.IdentidadID.ToString(), pCargadorResultadosModel.FormulariosSemanticos, pCargadorResultadosModel.FiltroContextoWhere, pCargadorResultadosModel.TipoProyecto, permitirRecursosPrivados, omitirPalabrasNoRelevantesSearch, tiposAlgoritmoTransformacion, pCargadorResultadosModel.FiltrosSearchPersonalizados, pEsMovil, pCargadorResultadosModel.ListaRecursosExcluidos);

                        pCargadorResultadosModel.NumeroResultados = 0;
                        if ((pCargadorResultadosModel.FacetadoDS.Tables.Contains("NResultadosBusqueda")) && (pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows.Count > 0))
                        {
                            object numeroResultados = pCargadorResultadosModel.FacetadoDS.Tables["NResultadosBusqueda"].Rows[0][0];
                            if (numeroResultados is long)
                            {
                                pCargadorResultadosModel.NumeroResultados = (int)(long)numeroResultados;
                            }
                            else if (numeroResultados is int)
                            {
                                pCargadorResultadosModel.NumeroResultados = (int)numeroResultados;
                            }
                            else if (numeroResultados is string)
                            {
                                int numeroResultadosInt;
                                int.TryParse((string)numeroResultados, out numeroResultadosInt);
                                pCargadorResultadosModel.NumeroResultados = numeroResultadosInt;
                            }
                        }
                    }
                }
            }

            if (usarHilos && listaTareasResultados.Count > 0)
            {
                try
                {
                    Task.WaitAll(listaTareasResultados.ToArray());
                }
                catch (AggregateException e)
                {
                    mLoggingService.GuardarLogError("\nThe following exceptions have been thrown by WaitAll()");
                    for (int j = 0; j < e.InnerExceptions.Count; j++)
                    {
                        mLoggingService.GuardarLogError(e.InnerExceptions[j].ToString());
                    }
                    throw new Exception(e.Message, e);
                }
                catch (Exception ex)
                {
                    mLoggingService.GuardarLogError(ex);
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        private static string CalcularMandatoryRelacion(CargadorResultadosModel pCargadorResultadosModel)
        {
            if (pCargadorResultadosModel.FilaPestanyaActual != null && pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda != null && pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.RelacionMandatory != null)
            {
                return pCargadorResultadosModel.FilaPestanyaActual.ProyectoPestanyaBusqueda.RelacionMandatory;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Devuelve un diccionario con las facetas que son excluyentes. Servicio Resultados.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, bool> ObtenerDiccionarioFacetasExcluyentes(CargadorResultadosModel pCargadorResultadosModel)
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            foreach (Faceta fac in pCargadorResultadosModel.GestorFacetas.ListaFacetas)
            {
                if (!dic.ContainsKey(fac.ClaveFaceta))
                {
                    dic.Add(fac.ClaveFaceta, fac.Excluyente);
                }
            }

            return dic;
        }

        private static TiposAlgoritmoTransformacion ObtenerAlogritmosTransofmracionRangosEnFiltros(CargadorResultadosModel mCargadorResultadosModel)
        {
            TiposAlgoritmoTransformacion tipo = TiposAlgoritmoTransformacion.Ninguno;

            if (mCargadorResultadosModel.ListaFiltros.Count > 1)
            {
                foreach (string filtro in mCargadorResultadosModel.ListaFiltros.Keys)
                {
                    string tempFiltro = filtro;
                    if (filtro.Contains(";"))
                    {
                        tempFiltro = filtro.Substring(filtro.IndexOf(";") + 1);
                    }
                    if (tempFiltro != "rdf:type")
                    {
                        foreach (Faceta fac in mCargadorResultadosModel.GestorFacetas.ListaFacetas)
                        {
                            if (fac.ClaveFaceta == tempFiltro)
                            {
                                if (fac.AlgoritmoTransformacion == TiposAlgoritmoTransformacion.Rangos)
                                {
                                    tipo = fac.AlgoritmoTransformacion;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return tipo;
        }

        #endregion
    }

    public class CargadorResultadosModel
    {
        #region Miembros

        /// <summary>
        /// DataSet con las pestañas del proyecto
        /// </summary>
        private DataWrapperProyecto mPestanyasProyecto = null;

        private bool mEsBot = false;

        private string mLanguageCode = "es";

        private bool mAdministradorQuiereVerTodasLasPersonas = false;

        private bool mEsMyGnoss = false;

        private bool mEstaEnProyecto = false;

        private bool mEsUsuarioInvitado = false;

        private bool mPrimeraCarga = false;

        private int mNumeroParteResultados = 1;

        private int mPaginaActual = 1;

        private bool mFiltroOrdenDescendente = true;

        private List<string> mListaItemsBusqueda = new List<string>();

        private List<Guid> mListaRecursosExcluidos = new List<Guid>();

        /// <summary>
        /// Parte peso minimo contexto
        /// </summary>
        private int mFiltroContextoPesoMinimo = 0;

        /// <summary>
        /// Número de resultados totales de la búsqueda
        /// </summary>
        private int mNumeroResultados = 0;

        /// <summary>
        /// Verdad si se trata de pintar un contexto en una ficha
        /// </summary>
        private bool mEsContexto = false;

        /// <summary>
        /// URL a partir de la cual se le añade la paginación a los bots
        /// </summary>
        private string mUrlNavegador = "";

        /// <summary>
        /// Máximo de filas por página que se muestran
        /// </summary>
        private int mFilasPorPagina = 10;

        /// <summary>
        /// Gestor de facetas
        /// </summary>
        private GestionFacetas mGestorFacetas;

        /// <summary>
        /// Diccionario con los filtros tipo 'search' personalizados
        /// la clave es el nombre del filtro y el valor es 'WhereSPARQL','OrderBySPARQL','WhereFacetasSPARQL','OmitirRdfType'
        /// </summary>
        Dictionary<string, Tuple<string, string, string, bool>> mFiltrosSearchPersonalizados;

        /// <summary>
        /// Proyecto principal unico
        /// </summary>
        private Guid? mProyectoPrincipalUnico;

        /// <summary>
        /// Fila de parametros generales
        /// </summary>
        private ParametroGeneral mFilaParametroGeneral = null;

        /// <summary>
        /// Parametros del proyecto
        /// </summary>
        private Dictionary<string, string> mParametroProyecto = null;

        /// <summary>
        /// Fila de parámetros de aplicación
        /// </summary>
        //private ParametroAplicacionDS mParametrosAplicacionDS;
        private List<AD.EntityModel.ParametroAplicacion> mParametrosAplicacionDS;

        private bool mEsRefrescoCache = false;


        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private VirtuosoAD mVirtuosoAD;
        private RedisCacheWrapper mRedisCacheWrapper;
        private ConfigService mConfigService;
        private UtilServiciosFacetas mUtilServiciosFacetas;
        private IServicesUtilVirtuosoAndReplication mServicesUtilVirtuosoAndReplication;

        #endregion
        
        public CargadorResultadosModel(EntityContext entityContext, LoggingService loggingService, RedisCacheWrapper redisCacheWrapper, ConfigService configService, VirtuosoAD virtuosoAD, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
        {
            mVirtuosoAD = virtuosoAD;
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mRedisCacheWrapper = redisCacheWrapper;
            mConfigService = configService;
            mServicesUtilVirtuosoAndReplication = servicesUtilVirtuosoAndReplication;
            mUtilServiciosFacetas = new UtilServiciosFacetas(loggingService, entityContext, configService, redisCacheWrapper, virtuosoAD, mServicesUtilVirtuosoAndReplication);
        }

        #region Métodos generales

        /// <summary>
        /// Obtiene la lista de resultados en un diccionario de la manera ID -> tipo resultado
        /// </summary>
        public void ObtenerListaID()
        {
            ListaIdsResultado = ObtenerListaID(FacetadoDS, "RecursosBusqueda", "");
        }

        /// <summary>
        /// Obtiene la lista de resultados en un diccionario de la manera ID -> tipo resultado
        /// </summary>
        public Dictionary<string, TiposResultadosMetaBuscador> ObtenerListaID(FacetadoDS pFacetadoDS, string pTabla, string pTipoResultados)
        {
            Dictionary<string, TiposResultadosMetaBuscador> listaIdsResultado = new Dictionary<string, TiposResultadosMetaBuscador>();

            if (pFacetadoDS.Tables.Contains(pTabla))
            {
                foreach (DataRow myrow in pFacetadoDS.Tables[pTabla].Rows)
                {
                    try
                    {
                        string id = mUtilServiciosFacetas.ObtenerIDTextoDesdeURI((string)myrow[0]);
                        if (!listaIdsResultado.ContainsKey(id))
                        {
                            if (string.IsNullOrEmpty(pTipoResultados))
                            {
                                listaIdsResultado.Add(id, TipoResultadoToTiposResultadoMetaBuscador((string)myrow["rdftype"]));
                            }
                            else
                            {
                                listaIdsResultado.Add(id, TipoResultadoToTiposResultadoMetaBuscador(pTipoResultados));
                            }

                        }
                    }
                    catch (Exception) { }
                }
            }
            return listaIdsResultado;
        }

        /// <summary>
        /// Devuelve el tipo de resultado de metabuscador de un elemento de virtuoso
        /// </summary>
        /// <param name="pTipo">Tipo de resultado de virtuoso</param>
        /// <returns></returns>
        private static TiposResultadosMetaBuscador TipoResultadoToTiposResultadoMetaBuscador(string pTipo)
        {
            if (pTipo.Equals("Comenta")) { pTipo = FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENTARIOS; }
            TiposResultadosMetaBuscador tipo = TiposResultadosMetaBuscador.Documento;
            switch (pTipo)
            {
                case FacetadoAD.BUSQUEDA_RECURSOS:
                case FacetadoAD.BUSQUEDA_DEBATES:
                case FacetadoAD.BUSQUEDA_PREGUNTAS:
                case FacetadoAD.BUSQUEDA_ENCUESTAS:
                case FacetadoAD.BUSQUEDA_RECURSOS_PERSONALES:
                    tipo = TiposResultadosMetaBuscador.Documento;
                    break;
                case FacetadoAD.BUSQUEDA_DAFOS:
                    tipo = TiposResultadosMetaBuscador.Dafo;
                    break;
                case FacetadoAD.BUSQUEDA_PERSONA:
                case FacetadoAD.BUSQUEDA_PROFESOR:
                case FacetadoAD.BUSQUEDA_ALUMNO:
                    tipo = TiposResultadosMetaBuscador.IdentidadPersona;
                    break;
                case FacetadoAD.BUSQUEDA_ORGANIZACION:
                case FacetadoAD.BUSQUEDA_CLASE:
                case FacetadoAD.BUSQUEDA_CLASE_UNIVERSIDAD:
                case FacetadoAD.BUSQUEDA_CLASE_SECUNDARIA:
                    tipo = TiposResultadosMetaBuscador.IdentidadOrganizacion;
                    break;
                case FacetadoAD.BUSQUEDA_COMUNIDADES:
                case FacetadoAD.BUSQUEDA_COMUNIDAD_EDUCATIVA:
                case FacetadoAD.BUSQUEDA_COMUNIDAD_NO_EDUCATIVA:
                    tipo = TiposResultadosMetaBuscador.Comunidad;
                    break;
                case FacetadoAD.BUSQUEDA_BLOGS:
                    tipo = TiposResultadosMetaBuscador.Blog;
                    break;
                case FacetadoAD.BUSQUEDA_ARTICULOSBLOG:
                    tipo = TiposResultadosMetaBuscador.EntradaBlog;
                    break;
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENTARIOS:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMARTICULOBLOG:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMDEBATES:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMENCUESTAS:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMFACTORDAFO:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMPREGUNTAS:
                case FacetadoAD.BUSQUEDA_CONTRIBUCIONES_COMRECURSOS:
                case FacetadoAD.BUSQUEDA_COMENTARIOS:
                    tipo = TiposResultadosMetaBuscador.Comentario;
                    break;
                case FacetadoAD.BUSQUEDA_MENSAJES:
                    tipo = TiposResultadosMetaBuscador.Mensaje;
                    break;
                case FacetadoAD.BUSQUEDA_INVITACIONES:
                    tipo = TiposResultadosMetaBuscador.Invitacion;
                    break;
                case FacetadoAD.BUSQUEDA_SUSCRIPCIONES:
                    tipo = TiposResultadosMetaBuscador.Suscripcion;
                    break;
                case FacetadoAD.BUSQUEDA_CONTACTOS_GRUPO:
                    tipo = TiposResultadosMetaBuscador.GrupoContacto;
                    break;
                case FacetadoAD.BUSQUEDA_CONTACTOS_ORGANIZACION:
                    tipo = TiposResultadosMetaBuscador.OrgContacto;
                    break;
                case FacetadoAD.BUSQUEDA_CONTACTOS_PERSONAL:
                    tipo = TiposResultadosMetaBuscador.PerContacto;
                    break;
                case FacetadoAD.BUSQUEDA_GRUPO:
                    tipo = TiposResultadosMetaBuscador.Grupo;
                    break;
                case FacetadoAD.PAGINA_CMS:
                    tipo = TiposResultadosMetaBuscador.PaginaCMS;
                    break;
            }

            return tipo;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Obtiene las pestañas del proyecto y su configuracion
        /// </summary>
        public DataWrapperProyecto PestanyasProyecto
        {
            get
            {
                if (mPestanyasProyecto == null)
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mPestanyasProyecto = proyCL.ObtenerPestanyasProyecto(ProyectoSeleccionado);
                }
                return mPestanyasProyecto;
            }
        }

        /// <summary>
        /// Verdad si es un bot el que está realizando la búsqueda
        /// </summary>
        public bool EsBot
        {
            get
            {
                return mEsBot;
            }
            set
            {
                mEsBot = value;
            }
        }

        public string LanguageCode
        {
            get
            {
                return mLanguageCode;
            }
            set
            {
                mLanguageCode = value;
            }
        }

        public bool AdministradorQuiereVerTodasLasPersonas
        {
            get
            {
                return mAdministradorQuiereVerTodasLasPersonas;
            }
            set
            {
                mAdministradorQuiereVerTodasLasPersonas = value;
            }
        }

        public bool EsMyGnoss
        {
            get
            {
                return mEsMyGnoss;
            }
            set
            {
                mEsMyGnoss = value;
            }
        }

        public bool EstaEnProyecto
        {
            get
            {
                return mEstaEnProyecto;
            }
            set
            {
                mEstaEnProyecto = value;
            }
        }

        public bool EsUsuarioInvitado
        {
            get
            {
                return mEsUsuarioInvitado;
            }
            set
            {
                mEsUsuarioInvitado = value;
            }
        }

        public bool PrimeraCarga
        {
            get
            {
                return mPrimeraCarga;
            }
            set
            {
                mPrimeraCarga = value;
            }
        }

        public int NumeroParteResultados
        {
            get
            {
                return mNumeroParteResultados;
            }
            set
            {
                mNumeroParteResultados = value;
            }
        }

        public int PaginaActual
        {
            get
            {
                return mPaginaActual;
            }
            set
            {
                mPaginaActual = value;
            }
        }

        public bool FiltroOrdenDescendente
        {
            get
            {
                return mFiltroOrdenDescendente;
            }
            set
            {
                mFiltroOrdenDescendente = value;
            }
        }

        public List<string> ListaItemsBusqueda
        {
            get
            {
                return mListaItemsBusqueda;
            }
            set
            {
                mListaItemsBusqueda = value;
            }
        }

        public List<Guid> ListaRecursosExcluidos
        {
            get
            {
                return mListaRecursosExcluidos;
            }
            set
            {
                mListaRecursosExcluidos = value;
            }
        }

        /// <summary>
        /// Número de resultados totales de la búsqueda
        /// </summary>
        public int NumeroResultados
        {
            get
            {
                return mNumeroResultados;
            }
            set
            {
                mNumeroResultados = value;
            }
        }

        /// <summary>
        /// Verdad si se trata de pintar un contexto en una ficha
        /// </summary>
        public bool EsContexto
        {
            get
            {
                return mEsContexto;
            }
            set
            {
                mEsContexto = value;
            }
        }

        /// <summary>
        /// URL a partir de la cual se le añade la paginación a los bots
        /// </summary>
        public string UrlNavegador
        {
            get
            {
                return mUrlNavegador;
            }
            set
            {
                mUrlNavegador = value;
            }
        }

        public bool EsRefrescoCache
        {
            get
            {
                return mEsRefrescoCache;
            }
            set
            {
                mEsRefrescoCache = value;
            }
        }

        public GestionFacetas GestorFacetas
        {
            get
            {
                if (mGestorFacetas == null)
                {
                    FacetaCL facetaCL = new FacetaCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mGestorFacetas = new GestionFacetas(facetaCL.ObtenerTodasFacetasDeProyecto(null, Proyecto.FilaProyecto.OrganizacionID, ProyectoSeleccionado, false), mLoggingService);
                }
                return mGestorFacetas;
            }
        }

        /// <summary>
        /// Obtiene el proyecto principal de un ecosistema sin metaproyecto
        /// </summary>
        public Guid ProyectoPrincipalUnico
        {
            get
            {
                if (mProyectoPrincipalUnico == null)
                {
                    mProyectoPrincipalUnico = ProyectoAD.MetaProyecto;
                    //if (ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'ComunidadPrincipalID'").Length > 0)
                    if (ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("ComunidadPrincipalID")).ToList().Count > 0)
                    {
                        //mProyectoPrincipalUnico = new Guid(ParametrosAplicacionDS.ParametroAplicacion.Select("Parametro = 'ComunidadPrincipalID'")[0]["Valor"].ToString());
                        mProyectoPrincipalUnico = new Guid(ParametrosAplicacionDS.Where(parametro => parametro.Parametro.Equals("ComunidadPrincipalID")).First().Valor);
                    }
                }
                return mProyectoPrincipalUnico.Value;
            }
        }

        /// <summary>
        /// Obtiene la fila de parametros generales
        /// </summary>
        public ParametroGeneral FilaParametroGeneral
        {
            get
            {
                if (mFilaParametroGeneral == null)
                {
                    GestorParametroGeneral paramDS;
                    ParametroGeneralCL paramCL = new ParametroGeneralCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    paramDS = paramCL.ObtenerParametrosGeneralesDeProyecto(ProyectoSeleccionado);
                    paramCL.Dispose();

                    mFilaParametroGeneral = paramDS.ListaParametroGeneral.FirstOrDefault(parametroGeneral => parametroGeneral.ProyectoID.Equals(ProyectoSeleccionado));
                }
                return mFilaParametroGeneral;
            }
        }

        /// <summary>
        /// Parámetros de un proyecto.
        /// </summary>
        public Dictionary<string, string> ParametroProyecto
        {
            get
            {
                if (mParametroProyecto == null)
                {
                    ProyectoCL proyectoCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    mParametroProyecto = proyectoCL.ObtenerParametrosProyecto(ProyectoSeleccionado);
                    proyectoCL.Dispose();
                }

                return mParametroProyecto;
            }
        }



        /// <summary>
        /// Obtiene el dataset de parámetros de aplicación
        /// </summary>
        //public ParametroAplicacionDS ParametrosAplicacionDS
        public List<AD.EntityModel.ParametroAplicacion> ParametrosAplicacionDS
        {
            get
            {
                if (mParametrosAplicacionDS == null)
                {
                    ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication);
                    mParametrosAplicacionDS = paramCL.ObtenerParametrosAplicacionPorContext();
                    //mParametrosAplicacionDS = ((ParametroAplicacionDS)paramCL.ObtenerParametrosAplicacion());
                }
                return mParametrosAplicacionDS;
            }
        }



        public Dictionary<string, TiposResultadosMetaBuscador> ListaIdsResultado { get; set; }

        /// <summary>
        /// Indica si no debe haber caché.
        /// </summary>
        public bool SinCache { get; set; }

        /// <summary>
        /// Indica si no debe haber privacidad.
        /// </summary>
        public bool SinPrivacidad { get; set; }

        /// <summary>
        /// Indica si no tiene que calcular el número de resultados
        /// </summary>
        public bool IgnorarNumResultados { get; set; }

        /// <summary>
        /// Indica si no deben cargarse las propiedades semanticas.
        /// </summary>
        public bool SinDatosExtra { get; set; }

        /// <summary>
        /// Identificador de la pestanya actual
        /// </summary>
        public Guid PestanyaActualID { get; set; }

        /// <summary>
        /// ID del proyecto de origen para la búsqueda.
        /// </summary>
        public Guid ProyectoOrigenID { get; set; }

        /// <summary>
        /// Indica si la búsqueda es de tipo mapa.
        /// </summary>
        public bool BusquedaTipoMapa { get; set; }

        /// <summary>
        /// Indica si la búsqueda es de tipo chart.
        /// </summary>
        public bool BusquedaTipoChart { get; set; }

        /// <summary>
        /// Indica si la búsqueda es de tipo dashboard
        /// TFGH FRAN
        /// </summary>
        public bool BusquedaTipoDashboard { get; set; }
        /// <summary>
        /// Indica si la búsqueda es de tipo dashboard
        /// </summary>
        public string BusquedaTipoDashboardSelect { get; set; }
        /// <summary>
        /// Indica si la búsqueda es de tipo dashboard
        /// TFGH FRAN
        /// </summary>
        public string BusquedaTipoDashboardWhere { get; set; }

        /// <summary>
        /// ID del chart de la búsqueda.
        /// </summary>
        public Guid ChartID { get; set; }

        public Guid IdentidadID { get; set; }

        /// <summary>
        /// Este ID será proyecto en el caso de la búsqueda en comunidades o mygnoss, el id del perfil en el caso de contribuciones, etc.
        /// </summary>
        public string GrafoID { get; set; }

        /// <summary>
        /// Identidad del usuario actual
        /// </summary>
        public Identidad IdentidadActual { get; set; }

        /// <summary>
        /// Proyecto seleccionado
        /// </summary>
        public Guid ProyectoSeleccionado { get; set; }

        public Guid PerfilIdentidadID { get; set; }

        public string FiltroOrdenadoPor { get; set; }

        public List<string> FormulariosSemanticos { get; set; }

        public Dictionary<string, List<string>> ListaFiltros { get; set; }

        public List<string> ListaItemsBusquedaExtra { get; set; }

        public Dictionary<string, List<string>> InformacionOntologias { get; set; }

        public TipoBusqueda TipoBusqueda { get; set; }

        /// <summary>
        /// Lista de filtros establecidos desde las facetas por el usuario
        /// </summary>
        public Dictionary<string, List<string>> ListaFiltrosFacetasUsuario { get; set; }

        public FacetadoDS FacetadoDS { get; set; }

        public FacetadoCL FacetadoCL { get; set; }

        public DataWrapperFacetas TConfiguracionOntologia { get; set; }

        public TipoProyecto TipoProyecto { get; set; }

        private Elementos.ServiciosGenerales.Proyecto mProyecto = null;

        /// <summary>
        /// Proyecto actual
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto Proyecto
        {
            get
            {
                if (mProyecto == null)
                {
                    ProyectoCL proyCL = new ProyectoCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mVirtuosoAD, mServicesUtilVirtuosoAndReplication);
                    GestionProyecto gestorProy = new GestionProyecto(proyCL.ObtenerProyectoPorID(ProyectoSeleccionado), mLoggingService, mEntityContext);
                    mProyecto = gestorProy.ListaProyectos[ProyectoSeleccionado];
                }
                return mProyecto;
            }
        }

        /// <summary>
        /// Proyecto actual
        /// </summary>
        public Elementos.ServiciosGenerales.Proyecto ProyectoMyGnoss { get; set; }

        /// <summary>
        /// Indica si la petición es de un gadget.
        /// </summary>
        public bool EsPeticionGadget { get; set; }

        /// <summary>
        /// Url del perfil de la identidad actual
        /// </summary>
        public string UrlPerfil { get; set; }

        /// <summary>
        /// Select y Filtro para la consulta de Charts.
        /// </summary>
        public KeyValuePair<string, string> SelectFiltroChart { get; set; }
        public string NamespacesExtra { get; set; }
        public string ResultadosEliminar { get; set; }

        #region Miembros contextos
        /// <summary>
        /// Nombre del filtro de contexto
        /// </summary>
        public string FiltroContextoNombre { get; set; }

        /// <summary>
        /// Parte select filtro contexto
        /// </summary>
        public string FiltroContextoSelect { get; set; }

        /// <summary>
        /// Parte where filtro contexto
        /// </summary>
        public string FiltroContextoWhere { get; set; }

        /// <summary>
        /// Parte ordenes filtro contexto
        /// </summary>
        public string FiltroContextoOrderBy { get; set; }

        /// <summary>
        /// Parte peso minimo contexto
        /// </summary>
        public int FiltroContextoPesoMinimo
        {
            get
            {
                return mFiltroContextoPesoMinimo;
            }
            set
            {
                mFiltroContextoPesoMinimo = value;
            }
        }

        /// <summary>
        /// Diccionario con los filtros tipo 'search' personalizados
        /// la clave es el nombre del filtro y el valor es 'WhereSPARQL','OrderBySPARQL','WhereFacetasSPARQL'
        /// </summary>
        public Dictionary<string, Tuple<string, string, string, bool>> FiltrosSearchPersonalizados
        {
            get
            {
                if (mFiltrosSearchPersonalizados == null)
                {
                    mFiltrosSearchPersonalizados = new Dictionary<string, Tuple<string, string, string, bool>>();

                    if (Proyecto != null && Proyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado != null)
                    {
                        foreach (ProyectoSearchPersonalizado fila in Proyecto.GestorProyectos.DataWrapperProyectos.ListaProyectoSearchPersonalizado)
                        {
                            string whereFacetasSparql = fila.WhereFacetasSPARQL;
                            if (!string.IsNullOrEmpty(whereFacetasSparql))
                            {
                                whereFacetasSparql = whereFacetasSparql.Replace("[PARAMETROIDIOMAUSUARIO]", LanguageCode);
                            }
                            string orderBySPARQL = fila.OrderBySPARQL;
                            if (!string.IsNullOrEmpty(orderBySPARQL))
                            {
                                orderBySPARQL = orderBySPARQL.Replace("[PARAMETROIDIOMAUSUARIO]", LanguageCode);
                            }

                            mFiltrosSearchPersonalizados.Add(fila.NombreFiltro, new Tuple<string, string, string, bool>(fila.WhereSPARQL.Replace("[PARAMETROIDIOMAUSUARIO]", LanguageCode), orderBySPARQL, whereFacetasSparql, fila.OmitirRdfType));
                        }
                    }
                }
                return mFiltrosSearchPersonalizados;
            }
        }

        #endregion

        #region Miembros estáticos

        /// <summary>
        /// Máximo de filas por página que se muestran
        /// </summary>
        public int FilasPorPagina
        {
            get
            {
                return mFilasPorPagina;
            }
            set
            {
                mFilasPorPagina = value;
            }
        }

        /// <summary>
        /// Número de resultados configurado
        /// </summary>
        public int? NumeroResultadosMostrar { get; set; }

        #endregion

        /// <summary>
        /// Fila de la pestanya actual
        /// </summary>
        private AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu mFilaPestanyaActual = null;

        /// <summary>
        /// 
        /// </summary>
        public AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu FilaPestanyaActual
        {
            get
            {
                if (mFilaPestanyaActual == null && PestanyaActualID != null && PestanyaActualID != Guid.Empty)
                {
                    List<AD.EntityModel.Models.ProyectoDS.ProyectoPestanyaMenu> pestanyas = PestanyasProyecto.ListaProyectoPestanyaMenu.Where(pestanya => pestanya.PestanyaID.Equals(PestanyaActualID)).ToList();
                    if (pestanyas.Count == 1)
                    {
                        mFilaPestanyaActual = pestanyas.FirstOrDefault();
                    }
                }
                return mFilaPestanyaActual;
            }
        }

        [Serializable]
        public class ConsultaCacheModelSerializable
        {
            public string WhereSPARQL { get; set; }
            public string WhereFacetasSPARQL { get; set; }
            public string OrderBy { get; set; }
            public bool OmitirRdfType { get; set; }
        }

        #endregion
    }
}
