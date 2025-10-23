﻿using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.Virtuoso;
using Es.Riam.Gnoss.CL;
using Es.Riam.Gnoss.CL.ParametrosAplicacion;
using Es.Riam.Gnoss.Logica.Documentacion;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Interfaces.InterfacesOpen;
using Es.Riam.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Es.Riam.Gnoss.Web.Controles.Administracion
{
    public class ControladorFlujos : ControladorBase
    {
        #region Miembros

        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;
        private IAvailableServices mAvailableServices;

        #endregion

        #region Constructores

        public ControladorFlujos(LoggingService loggingService, ConfigService configService, EntityContext entityContext, RedisCacheWrapper redisCacheWrapper, GnossCache gnossCache, VirtuosoAD virtuosoAD, IHttpContextAccessor httpContextAccessor, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, IAvailableServices availableServices, ILogger<ControladorFlujos> logger, ILoggerFactory loggerFactory) : base(loggingService, configService, entityContext, redisCacheWrapper, gnossCache, virtuosoAD, httpContextAccessor, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            mAvailableServices = availableServices;
        }

        #endregion

        #region Métodos Publicos
        public List<FlujoViewModel> CargarFlujosProyecto()
        {
            List<FlujoViewModel> listaFlujosViewModel = new List<FlujoViewModel>();

            using (FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory))
            {
                List<Flujo> listaFlujos = flujosCN.ObtenerFlujosPorProyecto(ProyectoSeleccionado.Clave);
                foreach (Flujo filaFlujo in listaFlujos)
                {
                    FlujoViewModel flujoViewModel = new FlujoViewModel();
                    flujoViewModel.FlujoID = filaFlujo.FlujoID;
                    flujoViewModel.Nombre = filaFlujo.Nombre;
                    flujoViewModel.Descripcion = filaFlujo.Descripcion;
                    flujoViewModel.ProyectoID = filaFlujo.ProyectoID;
                    flujoViewModel.Fecha = filaFlujo.Fecha;
                    flujoViewModel.TiposRecursos = flujosCN.ObtenerTiposContenidosPorFlujo(filaFlujo);
                    flujoViewModel.OntologiasProyectoNombre = flujosCN.ObtenerOntologiasNombreFlujo(filaFlujo.FlujoID);
                    listaFlujosViewModel.Add(flujoViewModel);
                }
            }

            return listaFlujosViewModel;
        }

        public FlujoViewModel CargarFlujo(Guid pFlujoID)
        {
            FlujoViewModel flujoViewModel = new FlujoViewModel();
            using (FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory))
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                Flujo filaFlujo = flujosCN.ObtenerFlujoPorFlujoID(pFlujoID);

                flujoViewModel.FlujoID = filaFlujo.FlujoID;
                flujoViewModel.Nombre = filaFlujo.Nombre;
                flujoViewModel.Descripcion = filaFlujo.Descripcion;
                flujoViewModel.ProyectoID = filaFlujo.ProyectoID;
                flujoViewModel.Fecha = filaFlujo.Fecha;
                flujoViewModel.TiposRecursos = flujosCN.ObtenerTiposContenidosPorFlujo(filaFlujo);
                flujoViewModel.OntologiasProyectoNombre = filaFlujo.FlujoObjetoConocimientoProyecto.Select(x => x.Ontologia).ToList();

                List<Estado> listaEstados = flujosCN.ObtenerEstadosPorFlujoID(filaFlujo.FlujoID);
                flujoViewModel.Estados = listaEstados.Select(e => new EstadoViewModel
                {
                    EstadoID = e.EstadoID,
                    FlujoID = e.FlujoID,
                    Nombre = e.Nombre,
                    Publico = e.Publico,
                    TipoEstado = (TipoEstado)e.Tipo,
                    Color = e.Color,
                    ListaEstadoIdentidad = e.EstadoIdentidad.Select(ei => new EstadoIdentidadViewModel
                    {
                        EstadoID = ei.EstadoID,
                        IdentidadID = ei.IdentidadID,
                        Editor = ei.Editor,
                        Nombre = identidadCN.ObtenerNombreDeIdentidad(ei.IdentidadID),
                        PerfilID = identidadCN.ObtenerPerfilIDDeIdentidadID(ei.IdentidadID)
                    }).ToList(),
                    ListaEstadoGrupo = e.EstadoGrupo.Select(eg => new EstadoGrupoViewModel
                    {
                        EstadoID = eg.EstadoID,
                        GrupoID = eg.GrupoID,
                        Editor = eg.Editor,
                        Nombre = identidadCN.ObtenerNombreDeGrupo(eg.GrupoID)
                    }).ToList()
                }).ToList();

                List<Transicion> listaTransiciones = flujosCN.ObtenerTransicionesPorEstadosID(listaEstados.Select(e => e.EstadoID).ToList());
                flujoViewModel.Transiciones = listaTransiciones.Select(t => new TransicionViewModel
                {
                    TransicionID = t.TransicionID,
                    Nombre = t.Nombre,
                    EstadoOrigen = new EstadoViewModel
                    {
                        EstadoID = t.EstadoOrigen.EstadoID,
                        Nombre = t.EstadoOrigen.Nombre,
                        TipoEstado = (TipoEstado)t.EstadoOrigen.Tipo,
                        Color = t.EstadoOrigen.Color
                    },
                    EstadoDestino = new EstadoViewModel
                    {
                        EstadoID = t.EstadoDestino.EstadoID,
                        Nombre = t.EstadoDestino.Nombre,
                        TipoEstado = (TipoEstado)t.EstadoDestino.Tipo,
                        Color = t.EstadoDestino.Color
                    },
                    ListaTransicionIdentidadPerfiles = identidadCN.ObtenerPerfilesIDDeIdentidadesID(t.TransicionIdentidad.Select(ti => ti.IdentidadID).ToList()).Values.ToList(),
                    ListaNombreTransicionIdentidad = t.TransicionIdentidad.Select(ti => ti.IdentidadID).ToDictionary(i => identidadCN.ObtenerPerfilIDDeIdentidadID(i), i => identidadCN.ObtenerNombreDeIdentidad(i)),
                    ListaTransicionGrupo = t.TransicionGrupo.Select(tg => tg.GrupoID).ToList(),
                    ListaNombreTransicionGrupo = identidadCN.ObtenerNombresDeGrupos(t.TransicionGrupo.Select(tg => tg.GrupoID).ToList())
                }).ToList();

                foreach (string nombreOntologia in flujoViewModel.OntologiasProyectoNombre)
                {
                    flujoViewModel.OntologiasProyecto.Add(docCN.ObtenerOntologiaAPartirNombre(ProyectoSeleccionado.Clave, nombreOntologia + ".owl"), nombreOntologia);
                }

                DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
                docCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dwDocumentacion, true, false, false);
                Dictionary<Guid, string> diccionarioOntologias = dwDocumentacion.ListaDocumento.ToDictionary(k => k.DocumentoID, k => k.Enlace.Replace(".owl", ""));
                flujoViewModel.OntologiasProyecto = diccionarioOntologias;

                docCN.Dispose();
                identidadCN.Dispose();
            }
            return flujoViewModel;
        }

        public string ComprobarErrores(FlujoViewModel pModelo)
        {
            string error = "";

            error = ComprobarErrorNombresRepetidos(pModelo);

            if (string.IsNullOrEmpty(error))
            {
                error = ComprobarTransiciones(pModelo.Transiciones);
            }

            if (string.IsNullOrEmpty(error))
            {
                error = ComprobarTiposContenidos(pModelo);
            }

            return error;
        }

        public void GuardarFlujo(FlujoViewModel pModelo)
        {
            using (FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory))
            {
                IdentidadCN identidadCN = new IdentidadCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<IdentidadCN>(), mLoggerFactory);
                DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);
                docCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dwDocumentacion, true, false, false);
                Dictionary<Guid, string> ontologiasProyecto = dwDocumentacion.ListaDocumento.ToDictionary(k => k.DocumentoID, k => k.Enlace.Replace(".owl", ""));

                Flujo filaFlujo = new Flujo
                {
                    FlujoID = pModelo.FlujoID,
                    Nombre = pModelo.Nombre,
                    Descripcion = pModelo.Descripcion,
                    OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID,
                    ProyectoID = ProyectoSeleccionado.Clave,
                    Fecha = DateTime.Now,
                    Nota = pModelo.TiposRecursos[TiposContenidos.Nota],
                    Adjunto = pModelo.TiposRecursos[TiposContenidos.Adjunto],
                    Video = pModelo.TiposRecursos[TiposContenidos.Video],
                    Link = pModelo.TiposRecursos[TiposContenidos.Link],
                    Encuesta = pModelo.TiposRecursos[TiposContenidos.Encuesta],
                    Debate = pModelo.TiposRecursos[TiposContenidos.Debate],
                    PaginaCMS = pModelo.TiposRecursos[TiposContenidos.PaginaCMS],
                    ComponenteCMS = pModelo.TiposRecursos[TiposContenidos.ComponenteCMS],
                    RecursoSemantico = pModelo.TiposRecursos[TiposContenidos.RecursoSemantico]
                };

                // Comprobamos si ha actualizado los recursos afectados del flujo
                Dictionary<TiposContenidos, bool> contenidosAfectadosAnteriores = flujosCN.ObtenerTiposContenidosPorFlujoID(pModelo.FlujoID);
                Dictionary<Guid, string> ontologiasAfectadasAnteriormente = flujosCN.ObtenerOntologiasFlujo(pModelo.FlujoID, ontologiasProyecto);

                foreach (var tipo in contenidosAfectadosAnteriores.Keys)
                {
                    if (contenidosAfectadosAnteriores[tipo] != pModelo.TiposRecursos[tipo] && contenidosAfectadosAnteriores[tipo])
                    {
                        List<Guid> ontologiasAfectadas = new List<Guid>();
                        // Ya no afecta a recursos semanticos
                        if (tipo.Equals(TiposContenidos.RecursoSemantico))
                        {
                            ontologiasAfectadas = ontologiasAfectadasAnteriormente.Keys.ToList();
                        }
                        AplicarEstadoAContenido(tipo, pModelo.FlujoID, null, ProyectoSeleccionado.Clave, ontologiasAfectadas, false, true, flujosCN);
                    }
                }

                // Mirar si hay un numero distinto de ontologias afectadas
                // Si es asi habrá que quitar el estado a los recursos que ya no son afectados por el flujo
                Dictionary<Guid, string> ontologiasABorrar = ontologiasAfectadasAnteriormente.Where(kvp => !pModelo.OntologiasProyecto.ContainsKey(kvp.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                if (ontologiasABorrar.Count > 0)
                {
                    AplicarEstadoAContenido(TiposContenidos.RecursoSemantico, pModelo.FlujoID, null, ProyectoSeleccionado.Clave, ontologiasABorrar.Keys.ToList(), false, true, flujosCN);
                    foreach (string ontologiaABorrar in ontologiasABorrar.Values)
                    {
                        flujosCN.EliminarFlujoObjetoConocimientoProyectoPorNombreOntologia(pModelo.FlujoID, ontologiaABorrar);
                    }
                }

                // Guardamos los datos del flujo en BD
                flujosCN.GuardarFlujo(filaFlujo);

                if (filaFlujo.RecursoSemantico)
                {
                    GuardarFlujoObjetoConocimientoProyecto(filaFlujo.FlujoID, pModelo.OntologiasProyecto.Values.ToList(), flujosCN);
                }

                // Borrar las transiciones y estados que han sido marcados como eliminados
                flujosCN.EliminarTransiciones(pModelo.Transiciones.Where(i => i.Eliminado).Select(i => i.TransicionID).ToList());
                flujosCN.EliminarEstados(pModelo.Estados.Where(i => i.Eliminado).Select(i => i.EstadoID).ToList(), filaFlujo.FlujoID);

                // Seccion de estados
                GuardarEstados(pModelo.Estados, filaFlujo.FlujoID, flujosCN, identidadCN);
                // Seccion de transiciones
                GuardarTransiciones(pModelo.Transiciones, flujosCN, identidadCN);
                // Seccion de aplicar el estadoID
                AplicarEstadosAContenidos(pModelo.TiposRecursos, pModelo.FlujoID, EstadoFinalDeFlujo(pModelo.Transiciones), ProyectoSeleccionado.Clave, pModelo.OntologiasProyecto.Keys.ToList(), false, false, flujosCN);

                identidadCN.Dispose();
            }
        }

        public void EliminarFlujo(Guid pFlujoID)
        {
            using (FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory))
            {
                DataWrapperDocumentacion dwDocumentacion = new DataWrapperDocumentacion();
                DocumentacionCN docCN = new DocumentacionCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<DocumentacionCN>(), mLoggerFactory);

                docCN.ObtenerOntologiasProyecto(ProyectoSeleccionado.Clave, dwDocumentacion, true, false, false);
                docCN.Dispose();

                List<Guid> ontologiasFlujo = flujosCN.ObtenerOntologiasFlujo(pFlujoID, dwDocumentacion.ListaDocumento.ToDictionary(k => k.DocumentoID, k => k.Enlace.Replace(".owl", ""))).Keys.ToList();
                AplicarEstadosAContenidos(flujosCN.ObtenerTiposContenidosPorFlujoID(pFlujoID), pFlujoID, null, ProyectoSeleccionado.Clave, ontologiasFlujo, true, true,
                    flujosCN);
            }
        }

        #endregion

        #region Metodos privados

        private void GuardarEstados(List<EstadoViewModel> pListaEstados, Guid pFlujoID, FlujosCN pFlujosCN, IdentidadCN pIdentidadCN)
        {
            foreach (EstadoViewModel estado in pListaEstados.Where(i => !i.Eliminado).ToList())
            {
                Estado estadoFila = new Estado();

                estadoFila.EstadoID = estado.EstadoID;
                estadoFila.FlujoID = pFlujoID;
                estadoFila.Nombre = estado.Nombre;
                estadoFila.Publico = estado.Publico;
                estadoFila.Tipo = (short)estado.TipoEstado;
                estadoFila.Color = estado.Color;
                pFlujosCN.GuardarEstado(estadoFila);

                EliminarEstadoIdentidadAntiguo(estado.EstadoID, estado.ListaEstadoIdentidad, pFlujosCN, pIdentidadCN);
                EliminarEstadoGrupoAntiguo(estado.EstadoID, estado.ListaEstadoGrupo, pFlujosCN);
                GuardarEstadoIdentidad(estado.ListaEstadoIdentidad, pFlujosCN, pIdentidadCN);
                GuardarEstadoGrupo(estado.ListaEstadoGrupo, pFlujosCN);
            }
        }

        private void GuardarEstadoIdentidad(List<EstadoIdentidadViewModel> pListaEstadoIdentidadViewModel, FlujosCN pFlujosCN, IdentidadCN pIdentidadCN)
        {
            foreach (EstadoIdentidadViewModel perfil in pListaEstadoIdentidadViewModel)
            {
                
                EstadoIdentidad estadoIdentidadFila = new EstadoIdentidad();
                Guid identidadID = (Guid)pIdentidadCN.ObtenerIdentidadIDDePerfilEnProyecto(ProyectoSeleccionado.Clave, perfil.PerfilID);
                estadoIdentidadFila.IdentidadID = identidadID;
                estadoIdentidadFila.EstadoID = perfil.EstadoID;
                estadoIdentidadFila.Editor = perfil.Editor;

                pFlujosCN.GuardarEstadoIdentidad(estadoIdentidadFila);
            }
        }

        private void EliminarEstadoIdentidadAntiguo(Guid pEstadoID, List<EstadoIdentidadViewModel> pListaEstadoIdentidad, FlujosCN pFlujosCN, IdentidadCN pIdentidadCN)
        {
            List<Guid> listaEstadoIdentidad = pFlujosCN.ObtenerIdentidadesDeEstadoPorID(pEstadoID);
            List<Guid> listaIdentidadesActuales = pIdentidadCN.ObtenerIdentidadesIDDePerfilesEnProyecto(pListaEstadoIdentidad.Select(e => e.PerfilID).ToList(), ProyectoSeleccionado.Clave);
            List<Guid> listaEstadoIdentidadEliminados = listaEstadoIdentidad.Where(item => !listaIdentidadesActuales.Contains(item)).ToList();
            if (listaEstadoIdentidadEliminados.Count > 0)
            {
                pFlujosCN.EliminarEstadoIdentidad(listaEstadoIdentidadEliminados, pEstadoID);
            }
        }

        private void GuardarEstadoGrupo(List<EstadoGrupoViewModel> pListaEstadoGrupoViewModel, FlujosCN pFlujosCN)
        {
            foreach (EstadoGrupoViewModel grupo in pListaEstadoGrupoViewModel)
            {
                EstadoGrupo estadoGrupoFila = new EstadoGrupo();
                estadoGrupoFila.EstadoID = grupo.EstadoID;
                estadoGrupoFila.GrupoID = grupo.GrupoID;
                estadoGrupoFila.Editor = grupo.Editor;
                pFlujosCN.GuardarEstadoGrupo(estadoGrupoFila);
            }
        }

        private void EliminarEstadoGrupoAntiguo(Guid pEstadoID, List<EstadoGrupoViewModel> pListaEstadoIdentidad, FlujosCN pFlujosCN)
        {
            List<Guid> listaTransicionGrupo = pFlujosCN.ObtenerGruposIDPorEstadoID(pEstadoID);

            List<Guid> listaEstadoGrupoEliminados = listaTransicionGrupo.Where(item => !pListaEstadoIdentidad.Select(item => item.GrupoID).Contains(item)).ToList();
            if (listaEstadoGrupoEliminados.Count > 0)
            {
                pFlujosCN.EliminarEstadosGrupo(listaEstadoGrupoEliminados, pEstadoID);
            }
        }

        private void GuardarTransiciones(List<TransicionViewModel> pListaTransiciones, FlujosCN pFlujosCN, IdentidadCN pIdentidadCN)
        {
            Guid identidadID = Guid.Empty;

            foreach (TransicionViewModel transicion in pListaTransiciones.Where(i => !i.Eliminado).ToList())
            {
                Transicion transicionFila = new Transicion();

                transicionFila.TransicionID = transicion.TransicionID;
                transicionFila.Nombre = transicion.Nombre;
                transicionFila.EstadoOrigenID = transicion.EstadoOrigen.EstadoID;
                transicionFila.EstadoDestinoID = transicion.EstadoDestino.EstadoID;

                pFlujosCN.GuardarTransicion(transicionFila);

                EliminarTransicionIdentidadAntiguo(transicion.TransicionID, transicion.ListaTransicionIdentidadPerfiles, pFlujosCN, pIdentidadCN);
                EliminarTransicionGrupoAntiguo(transicion.TransicionID, transicion.ListaTransicionGrupo, pFlujosCN);
                GuardarTransicionIdentidades(transicion.TransicionID, transicion.EstadoOrigen, transicion.EstadoDestino, transicion.ListaTransicionIdentidadPerfiles, pFlujosCN, pIdentidadCN);
                GuardarTransicionGrupos(transicion.TransicionID, transicion.EstadoOrigen, transicion.EstadoDestino, transicion.ListaTransicionGrupo, pFlujosCN);
            }
        }

        public void GuardarTransicionIdentidades(Guid pTransicionID, EstadoViewModel pEstadoOrigen, EstadoViewModel pEstadoDestino, List<Guid> pListaPerfiles, FlujosCN pFlujosCN, IdentidadCN pIdentidadCN)
        {
            foreach (Guid perfilID in pListaPerfiles)
            {
                TransicionIdentidad transicionIdentidad = new TransicionIdentidad();
                Guid identidadID = (Guid)pIdentidadCN.ObtenerIdentidadIDDePerfilEnProyecto(ProyectoSeleccionado.Clave, perfilID);

                transicionIdentidad.IdentidadID = identidadID;
                transicionIdentidad.TransicionID = pTransicionID;
                pFlujosCN.GuardarTransicionIdentidad(transicionIdentidad);

                // Comprobamos si el responsable está presente en los estados definidos. Si no lo está será asignado como lector del estado
                EstadoIdentidad filaEstadoIdentidadOrigen = pFlujosCN.ObtenerEstadoIdentidad(pEstadoOrigen.EstadoID, identidadID);
                if (filaEstadoIdentidadOrigen == null && !pEstadoOrigen.Publico)
                {
                    filaEstadoIdentidadOrigen = new EstadoIdentidad();
                    filaEstadoIdentidadOrigen.IdentidadID = identidadID;
                    filaEstadoIdentidadOrigen.EstadoID = pEstadoOrigen.EstadoID;
                    filaEstadoIdentidadOrigen.Editor = false;
                    pFlujosCN.GuardarEstadoIdentidad(filaEstadoIdentidadOrigen);
                }

                EstadoIdentidad filaEstadoIdentidadDestino = pFlujosCN.ObtenerEstadoIdentidad(pEstadoDestino.EstadoID, identidadID);
                if (filaEstadoIdentidadDestino == null && !pEstadoDestino.Publico)
                {
                    filaEstadoIdentidadDestino = new EstadoIdentidad();
                    filaEstadoIdentidadDestino.IdentidadID = identidadID;
                    filaEstadoIdentidadDestino.EstadoID = pEstadoDestino.EstadoID;
                    filaEstadoIdentidadDestino.Editor = false;
                    pFlujosCN.GuardarEstadoIdentidad(filaEstadoIdentidadDestino);
                }

            }
        }

        public void GuardarTransicionGrupos(Guid pTransicionID, EstadoViewModel pEstadoOrigen, EstadoViewModel pEstadoDestino, List<Guid> pListaGrupos, FlujosCN pFlujosCN)
        {
            foreach (Guid grupoID in pListaGrupos)
            {
                TransicionGrupo estadoGrupoFila = new TransicionGrupo();
                estadoGrupoFila.TransicionID = pTransicionID;
                estadoGrupoFila.GrupoID = grupoID;
                pFlujosCN.GuardarTransicionGrupo(estadoGrupoFila);

                // Comprobamos si el responsable está presente en los estados definidos. Si no lo está  y el estado es privado
                // entonces será asignado como lector del estado
                EstadoGrupo filaEstadoGrupoOrigen = pFlujosCN.ObtenerEstadoGrupo(pEstadoOrigen.EstadoID, grupoID);
                if (filaEstadoGrupoOrigen == null && !pEstadoOrigen.Publico)
                {
                    filaEstadoGrupoOrigen = new EstadoGrupo();
                    filaEstadoGrupoOrigen.GrupoID = grupoID;
                    filaEstadoGrupoOrigen.EstadoID = pEstadoOrigen.EstadoID;
                    filaEstadoGrupoOrigen.Editor = false;
                    pFlujosCN.GuardarEstadoGrupo(filaEstadoGrupoOrigen);
                }

                EstadoGrupo filaEstadoGrupoDestino = pFlujosCN.ObtenerEstadoGrupo(pEstadoDestino.EstadoID, grupoID);
                if (filaEstadoGrupoDestino == null && !pEstadoDestino.Publico)
                {
                    filaEstadoGrupoDestino = new EstadoGrupo();
                    filaEstadoGrupoDestino.GrupoID = grupoID;
                    filaEstadoGrupoDestino.EstadoID = pEstadoDestino.EstadoID;
                    filaEstadoGrupoDestino.Editor = false;
                    pFlujosCN.GuardarEstadoGrupo(filaEstadoGrupoDestino);
                }
            }
        }

        public void EliminarTransicionIdentidadAntiguo(Guid pTransicionID, List<Guid> pListaTransicionIdentidadPerfiles, FlujosCN pFlujosCN, IdentidadCN pIdentidadCN)
        {
            // Eliminamos los elementos TransicionIdentidad que no estén en el nuevo modelo
            List<Guid> listaTransicionIdentidad = pFlujosCN.ObtenerIdentidadesIDPorTransicionID(pTransicionID);

            // Obtenemos las identidades del modelo
            List<Guid> listaIdentidadesActuales = pIdentidadCN.ObtenerIdentidadesIDDePerfilesEnProyecto(pListaTransicionIdentidadPerfiles, ProyectoSeleccionado.Clave);

            List<Guid> listaTransicionIdentidadEliminados = listaTransicionIdentidad.Where(item => !listaIdentidadesActuales.Contains(item)).ToList();
            if (listaTransicionIdentidadEliminados.Count > 0)
            {
                pFlujosCN.EliminarTranscionesIdentidad(listaTransicionIdentidadEliminados, pTransicionID);
            }
        }
        public void EliminarTransicionGrupoAntiguo(Guid pTransicionID, List<Guid> pListaTransicionGrupo, FlujosCN pFlujosCN)
        {
            List<Guid> listaTransicionGrupo = pFlujosCN.ObtenerGruposIDPorTransicionID(pTransicionID);

            List<Guid> listaTransicionGrupoEliminados = listaTransicionGrupo.Where(item => !pListaTransicionGrupo.Contains(item)).ToList();
            if (listaTransicionGrupoEliminados.Count > 0)
            {
                pFlujosCN.EliminarTranscionesGrupo(listaTransicionGrupoEliminados, pTransicionID);
            }
        }

        private void GuardarFlujoObjetoConocimientoProyecto(Guid pFlujoID, List<string> nombresOntologias, FlujosCN pFlujosCN)
        {
            foreach (string nombreOntologia in nombresOntologias)
            {
                FlujoObjetoConocimientoProyecto filaFlujoOCProyecto = new FlujoObjetoConocimientoProyecto();
                filaFlujoOCProyecto.FlujoID = pFlujoID;
                filaFlujoOCProyecto.Ontologia = nombreOntologia;
                filaFlujoOCProyecto.ProyectoID = ProyectoSeleccionado.Clave;
                filaFlujoOCProyecto.OrganizacionID = ProyectoSeleccionado.FilaProyecto.OrganizacionID;
                pFlujosCN.GuardarFlujoObjetoConocimientoProyecto(filaFlujoOCProyecto);
            }
        }
        private void AplicarEstadosAContenidos(Dictionary<TiposContenidos, bool> pTiposContenidos, Guid pFlujoID, Guid? pEstadoInicialID, Guid pProyectoID, List<Guid> pListaOntologias, bool pEliminarFlujo, bool pEliminarEstado, FlujosCN pFlujosCN)
        {
            foreach (var tipoRecurso in pTiposContenidos.Where(item => item.Value).Select(item => item.Key).ToList())
            {
                pFlujosCN.InsertarEnColaFlujosCreadosOEliminados(pFlujoID, pEstadoInicialID, pProyectoID, pListaOntologias, tipoRecurso, pEliminarFlujo, pEliminarEstado, mAvailableServices);
            }
        }
        private void AplicarEstadoAContenido(TiposContenidos pTipoContenido, Guid pFlujoID, Guid? pEstadoInicialID, Guid pProyectoID, List<Guid> pListaOntologias, bool pEliminarFlujo, bool pEliminarEstado, FlujosCN pFlujosCN)
        {
            pFlujosCN.InsertarEnColaFlujosCreadosOEliminados(pFlujoID, pEstadoInicialID, pProyectoID, pListaOntologias, pTipoContenido, pEliminarFlujo, pEliminarEstado, mAvailableServices);
        }
        private Guid EstadoFinalDeFlujo(List<TransicionViewModel> pTransiciones)
        {
            Guid estadoID = pTransiciones.FirstOrDefault(t => t.EstadoDestino.TipoEstado.Equals(TipoEstado.Final) && t.EstadoDestino.Publico).EstadoDestino.EstadoID;

            if (estadoID == Guid.Empty)
            {
                estadoID = pTransiciones.FirstOrDefault(t => t.EstadoDestino.TipoEstado.Equals(TipoEstado.Final)).EstadoDestino.EstadoID;
            }
            return estadoID;
        }
        private string ComprobarTransiciones(List<TransicionViewModel> pListaTransiciones)
        {
            string error = "";
            // Comprobar que cada transicion tiene 2 estados asignados
            error = pListaTransiciones.Where(item => (item.EstadoDestino.EstadoID == Guid.Empty || item.EstadoOrigen.EstadoID == Guid.Empty) && !item.Eliminado).Any() ? "ERRORTRANSICIONESINCOMPLETAS" : "";

            if (string.IsNullOrEmpty(error))
            {
                var result = pListaTransiciones.Where(t => !t.Eliminado).FirstOrDefault(t => t.EstadoOrigen.EstadoID.Equals(t.EstadoDestino.EstadoID));
                if (result != null)
                {
                    error = "ERRORTRANSICIONESESTADOSIGUALES";

                }
            }
            if (string.IsNullOrEmpty(error))
            {
                var result = pListaTransiciones.Where(t => !t.Eliminado).GroupBy(t => new { EstadoOrigenID = t.EstadoOrigen.EstadoID, EstadoDestinoID = t.EstadoDestino.EstadoID }).Any(g => g.Count() > 1);

                if (result)
                {
                    error = "ERRORTRANSICIONESIGUALES";
                }
            }

            if (string.IsNullOrEmpty(error))
            {
                error = !pListaTransiciones.Any(i => i.EstadoOrigen.TipoEstado.Equals(TipoEstado.Inicial) && !i.EstadoOrigen.Eliminado) ? "ERRORTRANSICIONESSINESTADOINICIAL" : "";
            }

            if (string.IsNullOrEmpty(error))
            {
                error = !pListaTransiciones.Any(i => i.EstadoDestino.TipoEstado.Equals(TipoEstado.Final) && !i.EstadoOrigen.Eliminado) ? "ERRORTRANSICIONESSINESTADOFINAL" : "";
            }

            return error;
        }
        private string ComprobarErrorNombresRepetidos(FlujoViewModel pModelo)
        {
            string error = "";
            ParametroAplicacionCL paramCL = new ParametroAplicacionCL(mEntityContext, mLoggingService, mRedisCacheWrapper, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<ParametroAplicacionCL>(), mLoggerFactory);
            string idiomaPorDefecto = ParametrosGeneralesRow.IdiomaDefecto != null ? ParametrosGeneralesRow.IdiomaDefecto : paramCL.ObtenerListaIdiomas()[0];

            error = pModelo.Estados.Where(e => !e.Eliminado).GroupBy(item => UtilCadenas.ObtenerTextoDeIdioma(item.Nombre, idiomaPorDefecto, null, true)).Any(item => item.Count() > 1) ? "NOMBREESTADOREPETIDO" : "";

            return error;
        }
        private string ComprobarTiposContenidos(FlujoViewModel pModelo)
        {
            string error = "";

            using (FlujosCN flujosCN = new FlujosCN(mEntityContext, mLoggingService, mConfigService, mServicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosCN>(), mLoggerFactory))
            {
                error = flujosCN.ExisteFlujoAplicadoEnTiposContenidoPorProyecto(pModelo.FlujoID, ProyectoSeleccionado.Clave, pModelo.TiposRecursos, pModelo.OntologiasProyecto);
            }

            return error;
        }
        #endregion
    }
}
