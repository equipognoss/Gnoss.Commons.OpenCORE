using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.CMS;
using Es.Riam.Gnoss.AD.EntityModel.Models.Comentario;
using Es.Riam.Gnoss.AD.EntityModel.Models.Documentacion;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.EntityModel.Models.IdentidadDS;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using static Es.Riam.Gnoss.Util.Seguridad.Capacidad;

namespace Es.Riam.Gnoss.AD.Flujos
{
	public enum TipoContenidoFlujo
	{
		Recurso,
		ComponenteCMS,
		PaginaCMS
	}
	public class JoinFlujoObjetoConocimientoProyectoDocumento
    {
        public FlujoObjetoConocimientoProyecto FlujoObjetoConocimientoProyecto { get; set; }
        public Documento Documento { get; set; }
    }
    public static class Joins
    {
        public static IQueryable<JoinFlujoObjetoConocimientoProyectoDocumento> JoinFlujoObjetoConocimientoProyectoDocumento(this IQueryable<FlujoObjetoConocimientoProyecto> pQuery)
        {
            EntityContext entityContext = (EntityContext)QueryContextAccess.GetDbContext(pQuery);
            return pQuery.Join(entityContext.Documento, flujoObjetoConocimientoProyecto => flujoObjetoConocimientoProyecto.Ontologia, documento => documento.Titulo, (flujoObjetoConocimientoProyecto, documento) => new JoinFlujoObjetoConocimientoProyectoDocumento
            {
                FlujoObjetoConocimientoProyecto = flujoObjetoConocimientoProyecto,
                Documento = documento
            });
        }
    }
    public class FlujosAD : BaseAD
    {
        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ILogger mlogger;
        private ILoggerFactory mloggerFactory;

        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin parámetros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public FlujosAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FlujosAD> logger, ILoggerFactory loggerFactory)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mlogger = logger;
            mloggerFactory = loggerFactory;
        }

        #endregion

        #region Metodos públicos

        #region Flujo

        public void GuardarFlujo(Flujo pFlujo)
        {
            Flujo flujo = mEntityContext.Flujo.Where(x => x.FlujoID.Equals(pFlujo.FlujoID)).FirstOrDefault();
            if (flujo != null)
            {
                flujo.Nombre = pFlujo.Nombre;
                flujo.Descripcion = pFlujo.Descripcion;
                flujo.Fecha = pFlujo.Fecha;
                flujo.Nota = pFlujo.Nota;
                flujo.Adjunto = pFlujo.Adjunto;
                flujo.Video = pFlujo.Video;
                flujo.Link = pFlujo.Link;
                flujo.Encuesta = pFlujo.Encuesta;
                flujo.Debate = pFlujo.Debate;
                flujo.PaginaCMS = pFlujo.PaginaCMS;
                flujo.ComponenteCMS = pFlujo.ComponenteCMS;
                flujo.RecursoSemantico = pFlujo.RecursoSemantico;
            }
            else
            {
                mEntityContext.Flujo.Add(pFlujo);
            }
            mEntityContext.SaveChanges();
        }
        public void GuardarFlujoObjetoConocimientoProyecto(FlujoObjetoConocimientoProyecto pFlujoOCProyecto)
        {
            var fila = mEntityContext.FlujoObjetoConocimientoProyecto.Where(i => i.FlujoID.Equals(pFlujoOCProyecto.FlujoID) && i.Ontologia.Equals(pFlujoOCProyecto.Ontologia) && i.ProyectoID.Equals(pFlujoOCProyecto.ProyectoID) && i.OrganizacionID.Equals(pFlujoOCProyecto.OrganizacionID)).FirstOrDefault();
            if (fila == null)
            {
                mEntityContext.FlujoObjetoConocimientoProyecto.Add(pFlujoOCProyecto);
                mEntityContext.SaveChanges();
            }
        }
        public void EliminarFlujoObjetoConocimientoProyectoPorNombreOntologia(Guid pFlujoID, string pNombreOntologia)
        {
            FlujoObjetoConocimientoProyecto flujoOCProyFila = mEntityContext.FlujoObjetoConocimientoProyecto.FirstOrDefault(f => f.FlujoID.Equals(pFlujoID) && f.Ontologia.Equals(pNombreOntologia));
            if (flujoOCProyFila != null)
            {
                mEntityContext.EliminarElemento(flujoOCProyFila);
                mEntityContext.SaveChanges();
            }
        }
        public void EliminarFlujoObjetoConocimientoProyecto(Guid pFlujoID)
        {

            FlujoObjetoConocimientoProyecto flujoOCProyFila = mEntityContext.FlujoObjetoConocimientoProyecto.FirstOrDefault(f => f.FlujoID.Equals(pFlujoID));
            if (flujoOCProyFila != null)
            {
                mEntityContext.EliminarElemento(flujoOCProyFila);
                mEntityContext.SaveChanges();
            }
        }
        public void EliminarFlujo(Guid pFlujoID, Guid pProyectoID)
        {
            Flujo flujoFila = mEntityContext.Flujo.Where(item => item.FlujoID.Equals(pFlujoID) && item.ProyectoID.Equals(pProyectoID)).FirstOrDefault();
            if (flujoFila != null)
            {
                mEntityContext.EliminarElemento(flujoFila);
                mEntityContext.SaveChanges();
            }
        }
        public string ExisteFlujoAplicadoEnTiposContenidoPorProyecto(Guid pFlujoID, Guid pProyectoID, Dictionary<TiposContenidos, bool> pTipoContenidos, Dictionary<Guid, string> pOntologiasID)
        {
            string resultado = "";
            var flujos = mEntityContext.Flujo.Where(f => f.ProyectoID.Equals(pProyectoID) && !f.FlujoID.Equals(pFlujoID)).ToList();
            foreach (var flujo in flujos)
            {
                var diccionarioTipoContenidos = ObtenerTiposContenidosPorFlujo(flujo);
                foreach (var key in diccionarioTipoContenidos.Where(i => i.Value).ToDictionary().Keys)
                {
                    if (diccionarioTipoContenidos[key] == pTipoContenidos[key])
                    {
                        if (key == TiposContenidos.RecursoSemantico)
                        {
                            string error = AfectaMismasOntologias(flujo.FlujoID, flujo.ProyectoID, pOntologiasID);
                            if (!string.IsNullOrEmpty(error))
                            {
                                return $"ERRORFLUJOEXISTENTESOBRETIPO:{error}";
                            }
                        }
                        else
                        {
                            return $"ERRORFLUJOEXISTENTESOBRETIPO:{Enum.GetName(typeof(TiposContenidos), key)}";
                        }
                    }
                }
            }
            return resultado;
        }
        public List<string> ObtenerOntologiasNombreFlujo(Guid pFlujoID)
        {
            return mEntityContext.FlujoObjetoConocimientoProyecto.Where(f => f.FlujoID.Equals(pFlujoID)).Select(f => f.Ontologia).ToList();
        }
        public Dictionary<Guid, string> ObtenerOntologiasFlujo(Guid pFlujoID, Dictionary<Guid, string> pOntologiasProyecto)
        {
            var listaNombreOntologiaProyecto = mEntityContext.FlujoObjetoConocimientoProyecto.Where(i => i.FlujoID.Equals(pFlujoID)).Select(i => i.Ontologia).ToList();

            return pOntologiasProyecto.Where(i => listaNombreOntologiaProyecto.Contains(i.Value)).ToDictionary();
        }
        public List<Flujo> ObtenerFlujosPorProyecto(Guid pProyectoID)
        {
            return mEntityContext.Flujo.Where(f => f.ProyectoID.Equals(pProyectoID)).Include(f => f.FlujoObjetoConocimientoProyecto).ToList();
        }
        public Flujo ObtenerFlujoPorFlujoID(Guid pFlujoID)
        {
            return mEntityContext.Flujo.Where(f => f.FlujoID.Equals(pFlujoID)).Include(f => f.FlujoObjetoConocimientoProyecto).FirstOrDefault();
        }
        public bool ExisteFlujoProyecto(Guid pFlujoID, Guid pProyectoID)
        {
            return mEntityContext.Flujo.Any(f => f.ProyectoID.Equals(pProyectoID) && pFlujoID.Equals(pFlujoID));
        }
        public Dictionary<TiposContenidos, bool> ObtenerTiposContenidosPorFlujoID(Guid pFlujoID)
        {
            var flujo = mEntityContext.Flujo.Where(i => i.FlujoID.Equals(pFlujoID)).FirstOrDefault();
            return ObtenerTiposContenidosPorFlujo(flujo);
        }
        public Dictionary<TiposContenidos, bool> ObtenerTiposContenidosPorFlujo(Flujo pFlujo)
        {
            Dictionary<TiposContenidos, bool> resultado = new Dictionary<TiposContenidos, bool>();
            if (pFlujo == null)
            {
                return resultado;
            }
            else
            {
                resultado = new Dictionary<TiposContenidos, bool>
                {
                    { TiposContenidos.Nota, pFlujo.Nota },
                    { TiposContenidos.Adjunto, pFlujo.Adjunto },
                    { TiposContenidos.Link, pFlujo.Link },
                    { TiposContenidos.Encuesta, pFlujo.Encuesta },
                    { TiposContenidos.Debate, pFlujo.Debate },
                    { TiposContenidos.PaginaCMS, pFlujo.PaginaCMS },
                    { TiposContenidos.ComponenteCMS, pFlujo.ComponenteCMS },
                    { TiposContenidos.Video, pFlujo.Video },
                    { TiposContenidos.RecursoSemantico, pFlujo.RecursoSemantico }
                };
            }
            return resultado;
        }

        public Guid ObtenerFlujoIDDeOntologia(Guid pProyectoID, string pNombre)
        {
            return mEntityContext.FlujoObjetoConocimientoProyecto.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Ontologia.Equals(pNombre)).Select(x => x.FlujoID).FirstOrDefault();
        }

        public Guid ObtenerFlujoIDPorProyectoYTipoContenido(Guid pProyectoID, TiposContenidos pTipo)
        {
            Guid flujo = Guid.Empty;
			switch (pTipo)
			{
				case TiposContenidos.Nota:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Nota).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.Adjunto:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Adjunto).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.Link:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Link).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.Video:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Video).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.Debate:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Debate).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.Encuesta:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Encuesta).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.RecursoSemantico:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.RecursoSemantico).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.PaginaCMS:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.PaginaCMS).Select(x => x.FlujoID).FirstOrDefault();
					break;
				case TiposContenidos.ComponenteCMS:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.ComponenteCMS).Select(x => x.FlujoID).FirstOrDefault();
					break;
				default:
					break;
			}

            return flujo;
		}

		private string AfectaMismasOntologias(Guid pFlujoID, Guid pProyectoID, Dictionary<Guid, string> pOntologiasID)
        {
            string resultado = "";

            var listaOntologiasID = mEntityContext.FlujoObjetoConocimientoProyecto.JoinFlujoObjetoConocimientoProyectoDocumento().Where(i => i.FlujoObjetoConocimientoProyecto.FlujoID.Equals(pFlujoID) && i.Documento.Tipo.Equals((short)TiposDocumentacion.Ontologia) && !i.Documento.Eliminado && i.Documento.ProyectoID.Equals(pProyectoID)).Select(i => i.Documento.DocumentoID).ToList();

            foreach (Guid ontologiaID in listaOntologiasID)
            {
                if (pOntologiasID.ContainsKey(ontologiaID))
                {
                    resultado = pOntologiasID[ontologiaID];
                    break;
                }
            }

            return resultado;
        }

        #endregion

        #region Estado

        public void GuardarEstado(Estado pEstado)
        {
            Estado estado = mEntityContext.Estado.Where(x => x.EstadoID.Equals(pEstado.EstadoID)).FirstOrDefault();
            if (estado != null)
            {
                estado.Nombre = pEstado.Nombre;
                estado.Tipo = pEstado.Tipo;
                estado.Color = pEstado.Color;
                estado.Publico = pEstado.Publico;
            }
            else
            {
                mEntityContext.Estado.Add(pEstado);
            }
            mEntityContext.SaveChanges();
        }
        public void EliminarEstados(List<Guid> pListaEstadoID, Guid pFlujoID)
        {
            List<Estado> listaEstado = mEntityContext.Estado.Where(e => pListaEstadoID.Contains(e.EstadoID) && e.FlujoID.Equals(pFlujoID)).ToList();
            foreach (Estado estado in listaEstado)
            {
                mEntityContext.EliminarElemento(estado);
            }
            mEntityContext.SaveChanges();
        }
        public string PuedoEliminarEstado(Guid pEstadoID, Guid pFlujoID)
        {
            string error = "";
            if (string.IsNullOrEmpty(error))
            {
                var flujo = mEntityContext.Flujo.FirstOrDefault(i => i.FlujoID.Equals(pFlujoID));
                var contenidosAfectados = ObtenerTiposContenidosPorFlujo(flujo);

                foreach (var key in contenidosAfectados.Where(i => i.Value).ToDictionary().Keys)
                {
                    if (!string.IsNullOrEmpty(error)) break;
                    switch (key)
                    {
                        case TiposContenidos.Nota:
                        case TiposContenidos.Adjunto:
                        case TiposContenidos.Link:
                        case TiposContenidos.Video:
                        case TiposContenidos.RecursoSemantico:
                        case TiposContenidos.Debate:
                        case TiposContenidos.Encuesta:
                            List<Documento> documentos = mEntityContext.Documento.Where(d => d.Tipo.Equals((short)key) && !d.Eliminado && d.EstadoID.Equals(pEstadoID)).ToList();
                            if (documentos.Count > 0) error = $"ERRORESTADOAFECTACONTENIDO:{Enum.GetName(typeof(TiposContenidos), key)}";
                            break;
                        case TiposContenidos.PaginaCMS:
                            List<ProyectoPestanyaCMS> paginasCMS = mEntityContext.ProyectoPestanyaCMS.Where(i => i.EstadoID.Equals(pEstadoID)).ToList();
                            if (paginasCMS.Count > 0) error = $"ERRORESTADOAFECTACONTENIDO:{Enum.GetName(typeof(TiposContenidos), key)}";
                            break;
                        case TiposContenidos.ComponenteCMS:
                            List<CMSComponente> componentesCMS = mEntityContext.CMSComponente.Where(i => i.EstadoID.Equals(pEstadoID)).ToList();
                            if (componentesCMS.Count > 0) error = $"ERRORESTADOAFECTACONTENIDO:{Enum.GetName(typeof(TiposContenidos), key)}";
                            break;
                        default:
                            break;
                    }
                }
            }

            return error;
        }
        public List<Guid> ObtenerEstadosIDPorFlujoID(Guid pFlujoID)
        {
            return mEntityContext.Estado.Where(item => item.FlujoID.Equals(pFlujoID)).Select(item => item.EstadoID).ToList();
        }
        public List<Estado> ObtenerEstadosPorFlujoID(Guid pFlujoID)
        {
            return mEntityContext.Estado.Where(e => e.FlujoID.Equals(pFlujoID)).Include(e => e.EstadoIdentidad).Include(e => e.EstadoGrupo).ToList();
        }
        /// <summary>
        /// Aplica el estado a los recursos seleccionados, en caso de estar eliminando el estado se devuelve un diccionario
        /// con los documentos y sus estadoid para asignarle sus editores correspondientes
        /// </summary>
        /// <param name="pEstadoID"></param>
        /// <param name="pProyectoID"></param>
        /// <param name="pOntologiasID"></param>
        /// <param name="pTipoContenido"></param>
        /// <param name="pEliminado"></param>
        /// <returns></returns>
        public Dictionary<Guid, Guid> ActualizarEstadosRecursos(Guid? pEstadoID, Guid pProyectoID, List<Guid> pOntologiasID, short pTipoContenido, bool pEliminado = false)
        {
            Dictionary<Guid, Guid> resultado = new Dictionary<Guid, Guid>();
            var query = mEntityContext.Documento.Where(doc => doc.ProyectoID.Equals(pProyectoID) && doc.Tipo == pTipoContenido && !doc.Eliminado);
            if (pOntologiasID.Count > 0)
            {
                query = query.Where(doc => pOntologiasID.Contains((Guid)doc.ElementoVinculadoID));
            }

            if (!pEliminado)
            {
                query = query.Where(doc => !doc.EstadoID.HasValue || doc.EstadoID.Equals(Guid.Empty));
            }

            var documentosAfectados = query.ToList();

            foreach (Documento doc in documentosAfectados)
            {
                if (pEliminado)
                {
                    resultado.Add(doc.DocumentoID, (Guid)doc.EstadoID);
                }
                else
                {
                    resultado.Add(doc.DocumentoID, (Guid)pEstadoID);
                }
                doc.EstadoID = pEstadoID;
            }
            mEntityContext.SaveChanges();
            return resultado;
        }
        public Dictionary<Guid, Guid> ActualizarEstadoPaginasCMS(Guid? pEstadoID, Guid pProyectoID, bool pEliminado = false)
        {
            Dictionary<Guid, Guid> resultado = new Dictionary<Guid, Guid>();
            var query = mEntityContext.ProyectoPestanyaCMS.Join(mEntityContext.ProyectoPestanyaMenu, proyPestanyaCMS => proyPestanyaCMS.PestanyaID, proyPestanyaMenu => proyPestanyaMenu.PestanyaID, (proyPestanyaCMS, proyPestanyaMenu) => new
            {
                ProyectoPestanyaCMS = proyPestanyaCMS,
                ProyectPestanyaMenuID = proyPestanyaMenu.ProyectoID
            }).Where(proyPestanyaCMS => proyPestanyaCMS.ProyectPestanyaMenuID.Equals(pProyectoID));

            if (!pEliminado)
            {
                query = query.Where(item => !item.ProyectoPestanyaCMS.EstadoID.HasValue || item.ProyectoPestanyaCMS.EstadoID == Guid.Empty);
            }
            var pestanyasCMSAfectadas = query.Select(x => x.ProyectoPestanyaCMS).ToList();
            foreach (ProyectoPestanyaCMS pestanyaCMS in pestanyasCMSAfectadas)
            {
                if (pEliminado)
                {
                    resultado.Add(pestanyaCMS.PestanyaID, (Guid)pestanyaCMS.EstadoID);
                }
                else
                {
                    resultado.Add(pestanyaCMS.PestanyaID, (Guid)pEstadoID);
                }
                pestanyaCMS.EstadoID = pEstadoID;
            }
            mEntityContext.SaveChanges();
            return resultado;
        }
        public Dictionary<Guid, Guid> ActualizarEstadoComponentesCMS(Guid? pEstadoID, Guid pProyectoID, bool pEliminado = false)
        {
            Dictionary<Guid, Guid> resultado = new Dictionary<Guid, Guid>();
            var query = mEntityContext.CMSComponente.Where(c => c.ProyectoID.Equals(pProyectoID));
            if (!pEliminado)
            {
                query = query.Where(c => !c.EstadoID.HasValue || c.EstadoID == Guid.Empty);
            }

            var componentesCMSAfectados = query.ToList();

            foreach (CMSComponente componenteCMS in componentesCMSAfectados)
            {
                if (pEliminado)
                {
                    resultado.Add(componenteCMS.ComponenteID, (Guid)componenteCMS.EstadoID);
                }
                else
                {
                    resultado.Add(componenteCMS.ComponenteID, (Guid)pEstadoID);
                }
                componenteCMS.EstadoID = pEstadoID;
            }
            mEntityContext.SaveChanges();
            return resultado;
        }
        public void ActualizarEditoresRecursos(Dictionary<Guid, Guid> pDiccionarioRecursoIDEstadoID)
        {
            foreach(Guid documentoID in pDiccionarioRecursoIDEstadoID.Keys)
            {
                var listaEstadoIdentidad = mEntityContext.EstadoIdentidad.Where(i => i.EstadoID.Equals(pDiccionarioRecursoIDEstadoID[documentoID])).Select(i => new { i.IdentidadID, i.Editor }).ToList();
                // Actualizar la tabla DocumentoRolIdentidad
                foreach(var estadoIdentidad in listaEstadoIdentidad)
                {
                    Guid perfil = mEntityContext.Identidad.Where(i => i.IdentidadID.Equals(estadoIdentidad.IdentidadID)).Select(i => i.PerfilID).FirstOrDefault();
                    var documentoRolIdentidad = mEntityContext.DocumentoRolIdentidad.FirstOrDefault(i => i.PerfilID.Equals(perfil) && i.DocumentoID.Equals(documentoID));
                    if(documentoRolIdentidad == null)
                    {
                        DocumentoRolIdentidad filaDocumentoRolIdentidad = new DocumentoRolIdentidad();
                        filaDocumentoRolIdentidad.PerfilID = perfil;
                        filaDocumentoRolIdentidad.DocumentoID = documentoID;
                        filaDocumentoRolIdentidad.Editor = estadoIdentidad.Editor;
                        mEntityContext.DocumentoRolIdentidad.Add(filaDocumentoRolIdentidad);
                    }
                    else
                    {
                        documentoRolIdentidad.Editor = estadoIdentidad.Editor;
                    }
                }

                var listaEstadoGrupo = mEntityContext.EstadoGrupo.Where(i => i.EstadoID.Equals(pDiccionarioRecursoIDEstadoID[documentoID])).Select(i => new { i.GrupoID, i.Editor }).ToList();
                // Actualizar la tabla DocumentoRolGrupoIdentidades
                foreach (var estadoGrupo in listaEstadoGrupo)
                {
                    var documentoRolGrupoIdentidad = mEntityContext.DocumentoRolGrupoIdentidades.FirstOrDefault(i => i.GrupoID.Equals(estadoGrupo.GrupoID) && i.DocumentoID.Equals(documentoID));
                    if (documentoRolGrupoIdentidad == null)
                    {
                        DocumentoRolGrupoIdentidades filaDocumentoRolGrupoIdentidad = new DocumentoRolGrupoIdentidades();
                        filaDocumentoRolGrupoIdentidad.GrupoID = estadoGrupo.GrupoID;
                        filaDocumentoRolGrupoIdentidad.DocumentoID = documentoID;
                        filaDocumentoRolGrupoIdentidad.Editor = estadoGrupo.Editor;
                        mEntityContext.DocumentoRolGrupoIdentidades.Add(filaDocumentoRolGrupoIdentidad);
                    }
                    else
                    {
                        documentoRolGrupoIdentidad.Editor = estadoGrupo.Editor;
                    }
                }
            }
        }
        public bool ComprobarEstadoEsPublico(Guid pEstadoID)
        {
            return mEntityContext.Estado.Where(x => x.EstadoID.Equals(pEstadoID)).Select(x => x.Publico).FirstOrDefault();
        }

        public Estado ObtenerEstadoPorEstadoID(Guid pEstadoID)
        {
			return mEntityContext.Estado.FirstOrDefault(x => x.EstadoID.Equals(pEstadoID));
		}

        public List<Guid> ObtenerIdentidadesLectorasEstado(Guid pEstadoID)
        {
            return mEntityContext.EstadoIdentidad.Where(x => x.EstadoID.Equals(pEstadoID) && !x.Editor).Select(x => x.IdentidadID).ToList();
		}

        public List<Guid> ObtenerIdentidadesEditorasEstado(Guid pEstadoID)
        {
            return mEntityContext.EstadoIdentidad.Where(x => x.EstadoID.Equals(pEstadoID) && x.Editor).Select(x => x.IdentidadID).ToList();
		}

        public List<Guid> ObtenerGruposLectoresEstado(Guid pEstadoID)
        {
            return mEntityContext.EstadoGrupo.Where(x => x.EstadoID.Equals(pEstadoID) && !x.Editor).Select(x => x.GrupoID).ToList();
		}

        public List<Guid> ObtenerGruposEditoresEstado(Guid pEstadoID)
        {
            return mEntityContext.EstadoGrupo.Where(x => x.EstadoID.Equals(pEstadoID) && x.Editor).Select(x => x.GrupoID).ToList();
		}

        public List<Guid> ObtenerLectoresYEditoresDeEstado(Guid pEstadoID)
        {
            List<Guid> identidades = mEntityContext.EstadoIdentidad.Where(x => x.EstadoID.Equals(pEstadoID)).Select(x => x.IdentidadID).ToList();
			List<Guid> identidadesGrupo = mEntityContext.EstadoGrupo.Join(mEntityContext.GrupoIdentidadesParticipacion, estado => estado.GrupoID, grupo => grupo.GrupoID, (estado, grupo) => new
			{
				EstadoGrupo = estado,
				GrupoIdentidadesParticipacion = grupo
			}).Where(objeto => objeto.EstadoGrupo.EstadoID.Equals(pEstadoID)).Select(x => x.GrupoIdentidadesParticipacion.IdentidadID).ToList();

            List<Guid> identidadesTotales = identidades.Union(identidadesGrupo).ToList();

			return identidadesTotales;
		}

        public string ObtenerNombreDeEstado(Guid pEstadoID)
        {
            return mEntityContext.Estado.Where(x => x.EstadoID.Equals(pEstadoID)).Select(x => x.Nombre).FirstOrDefault();
        }

        public List<Transicion> ObtenerTransicionesDeEstadoDeIdentidad(Guid pEstadoID, Guid pIdentidadID)
        {
            return mEntityContext.Transicion.Join(mEntityContext.TransicionIdentidad, transicion => transicion.TransicionID, transicionIdentidad => transicionIdentidad.TransicionID, (transicion, transicionIdentidad) => new
			{
				Transicion = transicion,
				TransicionIdentidad = transicionIdentidad
			}).Where(objeto => objeto.TransicionIdentidad.IdentidadID.Equals(pIdentidadID) && objeto.Transicion.EstadoOrigenID.Equals(pEstadoID)).Select(objeto => objeto.Transicion).ToList();
		}

        public List<Transicion> ObtenerTransicionesDeEstadoDeGrupo(Guid pEstadoID, Guid pGrupoID)
        {
            return mEntityContext.Transicion.Join(mEntityContext.TransicionGrupo, transicion => transicion.TransicionID, transicionGrupo => transicionGrupo.TransicionID, (transicion, transicionGrupo) => new
			{
				Transicion = transicion,
				TransicionGrupo = transicionGrupo
			}).Where(objeto => objeto.TransicionGrupo.GrupoID.Equals(pGrupoID) && objeto.Transicion.EstadoOrigenID.Equals(pEstadoID)).Select(x => x.Transicion).ToList();
		}

        public Guid? ObtenerEstadoInicialDeTipoContenido(Guid pProyectoID, TiposContenidos pTipo, Guid? pFlujoID = null)
        {
            Flujo flujo = null;

			switch (pTipo)
			{
				case TiposContenidos.Nota:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Nota).FirstOrDefault();
                    break;
				case TiposContenidos.Adjunto:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Adjunto).FirstOrDefault();
					break;
				case TiposContenidos.Link:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Link).FirstOrDefault();
					break;
				case TiposContenidos.Video:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Video).FirstOrDefault();
                    break;
				case TiposContenidos.Debate:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Debate).FirstOrDefault();
                    break;
				case TiposContenidos.Encuesta:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.Encuesta).FirstOrDefault();
					break;
				case TiposContenidos.RecursoSemantico:
                    if (pFlujoID.HasValue)
                    {
						flujo = mEntityContext.Flujo.Where(x => x.FlujoID.Equals(pFlujoID.Value)).FirstOrDefault();
					}					
					break;
				case TiposContenidos.PaginaCMS:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.PaginaCMS).FirstOrDefault();
					break;
				case TiposContenidos.ComponenteCMS:
					flujo = mEntityContext.Flujo.Where(x => x.ProyectoID.Equals(pProyectoID) && x.ComponenteCMS).FirstOrDefault();
					break;
				default:
					break;
			}

			if (flujo != null)
			{
                return mEntityContext.Estado.Where(x => x.FlujoID.Equals(flujo.FlujoID) && x.Tipo.Equals((short)TipoEstado.Inicial)).Select(x => x.EstadoID).FirstOrDefault();
			}

            return null;
		}

        #endregion

        #region EstadoIdentidad
        public List<Guid> ObtenerIdentidadesDeEstadoPorID(Guid pEstadoID)
        {
            return mEntityContext.EstadoIdentidad.Where(item => item.EstadoID.Equals(pEstadoID)).Select(item => item.IdentidadID).ToList();
        }
        public EstadoIdentidad ObtenerEstadoIdentidad(Guid pEstadoID, Guid pIdentidadID)
        {
            return mEntityContext.EstadoIdentidad.FirstOrDefault(ei => ei.EstadoID.Equals(pEstadoID) && ei.IdentidadID.Equals(pIdentidadID));
        }
        public void GuardarEstadoIdentidad(EstadoIdentidad pEstadoIdentidad)
        {
            EstadoIdentidad estado = mEntityContext.EstadoIdentidad.Where(x => x.EstadoID.Equals(pEstadoIdentidad.EstadoID) && x.IdentidadID.Equals(pEstadoIdentidad.IdentidadID)).FirstOrDefault();
            if (estado != null)
            {
                estado.Editor = pEstadoIdentidad.Editor;
            }
            else
            {
                mEntityContext.EstadoIdentidad.Add(pEstadoIdentidad);
            }
            mEntityContext.SaveChanges();
        }
        /// <summary>
        /// Eliminamos los EstadoIdentiad de una Estado que ha sido modificado
        /// </summary>
        /// <param name="pListaIdentidadID"></param>
        /// <param name="pEstadoID"></param>
        public void EliminarEstadoIdentidad(List<Guid> pListaIdentidadID, Guid pEstadoID)
        {
            List<EstadoIdentidad> listaEstadoIdentidad = mEntityContext.EstadoIdentidad.Where(item => pListaIdentidadID.Contains(item.IdentidadID) && item.EstadoID.Equals(pEstadoID)).ToList();
            foreach (EstadoIdentidad estadoIdentidad in listaEstadoIdentidad)
            {
                mEntityContext.EliminarElemento(estadoIdentidad);
            }
            mEntityContext.SaveChanges();
        }

        public bool ComprobarIdentidadTienePermisoLecturaEnEstado(Guid pEstadoID, Guid pIdentidadID)
        {            
            // TODO FRAN: Comprobar si es el creador
			EstadoIdentidad estadoIdentidad = mEntityContext.EstadoIdentidad.Where(x => x.EstadoID.Equals(pEstadoID) && x.IdentidadID.Equals(pIdentidadID)).FirstOrDefault();

            if (estadoIdentidad == null)
            {
				List<Guid> listaGruposIdentidad = mEntityContext.GrupoIdentidadesParticipacion.Where(x => x.IdentidadID.Equals(pIdentidadID)).Distinct().Select(x => x.GrupoID).ToList();
				List<Guid> gruposEstado = mEntityContext.EstadoGrupo.Where(x => x.EstadoID.Equals(pEstadoID)).Select(x => x.GrupoID).ToList();
				bool tienePermiso = listaGruposIdentidad.Any(g => gruposEstado.Contains(g));

				return tienePermiso;
            }
            else
            {
                return true;
            }
        }

		public bool ComprobarIdentidadTienePermisoEdicionEnEstado(Guid pEstadoID, Guid pIdentidadID)
		{
			EstadoIdentidad estadoIdentidad = mEntityContext.EstadoIdentidad.Where(x => x.EstadoID.Equals(pEstadoID) && x.IdentidadID.Equals(pIdentidadID) && x.Editor).FirstOrDefault();

            if (estadoIdentidad == null)
            {
				List<Guid> listaGruposIdentidad = mEntityContext.GrupoIdentidadesParticipacion.Where(x => x.IdentidadID.Equals(pIdentidadID)).Distinct().Select(x => x.GrupoID).ToList();
				List<Guid> gruposEstado = mEntityContext.EstadoGrupo.Where(x => x.EstadoID.Equals(pEstadoID) && x.Editor).Select(x => x.GrupoID).ToList();

				bool tienePermiso = listaGruposIdentidad.Any(g => gruposEstado.Contains(g));

				return tienePermiso;
            }
            else
            {
                return true;
            }
		}
        #endregion

        #region EstadoGrupo
        public List<Guid> ObtenerGruposIDPorEstadoID(Guid pEstadoID)
        {
            return mEntityContext.EstadoGrupo.Where(item => item.EstadoID.Equals(pEstadoID)).Select(item => item.GrupoID).ToList();
        }
        public EstadoGrupo ObtenerEstadoGrupo(Guid pEstadoID, Guid pGrupoID)
        {
            return mEntityContext.EstadoGrupo.FirstOrDefault(ei => ei.EstadoID.Equals(pEstadoID) && ei.GrupoID.Equals(pGrupoID));
        }
        public void GuardarEstadoGrupo(EstadoGrupo pEstadoGrupo)
        {
            EstadoGrupo estado = mEntityContext.EstadoGrupo.Where(x => x.EstadoID.Equals(pEstadoGrupo.EstadoID) && x.GrupoID.Equals(pEstadoGrupo.GrupoID)).FirstOrDefault();
            if (estado != null)
            {
                estado.Editor = pEstadoGrupo.Editor;
            }
            else
            {
                mEntityContext.EstadoGrupo.Add(pEstadoGrupo);
            }
            mEntityContext.SaveChanges();
        }
        /// <summary>
        /// Eliminamos los EstadoGrupo de un estado modificado
        /// </summary>
        /// <param name="pListaGrupoID"></param>
        /// <param name="pEstadoID"></param>
        public void EliminarEstadoGrupo(List<Guid> pListaGrupoID, Guid pEstadoID)
        {
            List<EstadoGrupo> listaEstadoGrupo = mEntityContext.EstadoGrupo.Where(item => pListaGrupoID.Contains(item.GrupoID) && item.EstadoID.Equals(pEstadoID)).ToList();
            foreach (EstadoGrupo estadoGrupo in listaEstadoGrupo)
            {
                mEntityContext.EliminarElemento(estadoGrupo);
            }
            mEntityContext.SaveChanges();
        }

        public void CambiarEstadoContenido(Guid pContenidoID, Guid pEstadoDestinoID, TipoContenidoFlujo pTipo)
        {
            switch (pTipo)
            {
                case TipoContenidoFlujo.Recurso:
					Documento doc = mEntityContext.Documento.Where(x => x.DocumentoID.Equals(pContenidoID)).FirstOrDefault();
                    doc.EstadoID = pEstadoDestinoID;
					break;
                case TipoContenidoFlujo.ComponenteCMS:
                    CMSComponente cms = mEntityContext.CMSComponente.Where(x => x.ComponenteID.Equals(pContenidoID)).FirstOrDefault();
                    cms.EstadoID = pEstadoDestinoID;
					break;
                case TipoContenidoFlujo.PaginaCMS:
                    ProyectoPestanyaCMS pagina = mEntityContext.ProyectoPestanyaCMS.Where(x => x.PestanyaID.Equals(pContenidoID)).FirstOrDefault();
                    pagina.EstadoID = pEstadoDestinoID;
					break;
            }

            mEntityContext.SaveChanges();
        }

		public Guid ObtenerEstadoIDDeContenido(Guid pContenidoID, TipoContenidoFlujo pTipo)
        {
			Guid? estado = null;
			switch (pTipo)
			{
				case TipoContenidoFlujo.PaginaCMS:
					estado = mEntityContext.ProyectoPestanyaCMS.Where(x => x.PestanyaID.Equals(pContenidoID)).Select(x => x.EstadoID).FirstOrDefault();
					break;
				case TipoContenidoFlujo.ComponenteCMS:
					estado = mEntityContext.CMSComponente.Where(x => x.ComponenteID.Equals(pContenidoID)).Select(x => x.EstadoID).FirstOrDefault();
					break;
				case TipoContenidoFlujo.Recurso:
					estado = mEntityContext.Documento.Where(x => x.DocumentoID.Equals(pContenidoID)).Select(x => x.EstadoID).FirstOrDefault();
					break;
			}

			return estado.HasValue ? estado.Value : Guid.Empty;
		}

		#endregion

		#region Transicion

		public List<Guid> ObtenerTransicionesIDPorEstadosID(List<Guid> pListaEstadosID)
        {
            return mEntityContext.Transicion.Where(t => pListaEstadosID.Contains(t.EstadoOrigenID) || pListaEstadosID.Contains(t.EstadoDestinoID)).Select(t => t.TransicionID).ToList();
        }
        public List<Transicion> ObtenerTransicionesPorEstadosID(List<Guid> pListaEstadosID)
        {
            return mEntityContext.Transicion.Where(t => pListaEstadosID.Contains(t.EstadoOrigenID) || pListaEstadosID.Contains(t.EstadoDestinoID)).Include(t => t.TransicionIdentidad).Include(t => t.TransicionGrupo).ToList();
            //return mEntityContext.Transicion.Where(t => pListaEstadosID.Contains(t.EstadoOrigenID) || pListaEstadosID.Contains(t.EstadoDestinoID)).ToList();
        }
        public void GuardarTransicion(Transicion pTransicion)
        {
            Transicion transicion = mEntityContext.Transicion.Where(x => x.TransicionID.Equals(pTransicion.TransicionID)).FirstOrDefault();
            if (transicion != null)
            {
                transicion.Nombre = pTransicion.Nombre;
                transicion.EstadoDestinoID = pTransicion.EstadoDestinoID;
                transicion.EstadoOrigenID = pTransicion.EstadoOrigenID;
            }
            else
            {
                mEntityContext.Transicion.Add(pTransicion);
            }
            mEntityContext.SaveChanges();
        }
        public void EliminarTransiciones(List<Guid> pListaTransicionID)
        {
            List<Transicion> listaTransicion = mEntityContext.Transicion.Where(item => pListaTransicionID.Contains(item.TransicionID)).ToList();
            foreach (Transicion transicion in listaTransicion)
            {
                mEntityContext.EliminarElemento(transicion);
            }
            mEntityContext.SaveChanges();
        }
        public string PuedoEliminarTransicion(Guid pTransicionID, Guid pFlujoID)
        {
            string error = "";
            var transicion = mEntityContext.Transicion.Where(t => t.TransicionID.Equals(pTransicionID)).FirstOrDefault();
            var flujo = mEntityContext.Flujo.FirstOrDefault(t => t.FlujoID.Equals(pFlujoID));
            var contenidosAfectados = ObtenerTiposContenidosPorFlujo(flujo);
            foreach (var key in contenidosAfectados.Where(i => i.Value).ToDictionary().Keys)
            {
                switch (key)
                {
                    case TiposContenidos.Nota:
                    case TiposContenidos.Adjunto:
                    case TiposContenidos.Link:
                    case TiposContenidos.Video:
                    case TiposContenidos.RecursoSemantico:
                    case TiposContenidos.Debate:
                    case TiposContenidos.Encuesta:
                        List<Documento> documentos = mEntityContext.Documento.Where(d => d.Tipo.Equals((short)key) && !d.Eliminado && (d.EstadoID.Equals(transicion.EstadoOrigenID) || d.EstadoID.Equals(transicion.EstadoDestinoID))).ToList();
                        if (documentos.Count > 0) error = $"ERRORTRANSICIONAFECTACONTENIDO:{Enum.GetName(typeof(TiposContenidos), key)}";
                        break;
                    case TiposContenidos.PaginaCMS:
                        List<ProyectoPestanyaCMS> paginasCMS = mEntityContext.ProyectoPestanyaCMS.Where(i => i.EstadoID.Equals(transicion.EstadoOrigenID) || i.EstadoID.Equals(transicion.EstadoDestinoID)).ToList();
                        if (paginasCMS.Count > 0) error = $"ERRORTRANSICIONAFECTACONTENIDO:{Enum.GetName(typeof(TiposContenidos), key)}";
                        break;
                    case TiposContenidos.ComponenteCMS:
                        List<CMSComponente> componentesCMS = mEntityContext.CMSComponente.Where(i => i.EstadoID.Equals(transicion.EstadoOrigenID) || i.EstadoID.Equals(transicion.EstadoDestinoID)).ToList();
                        if (componentesCMS.Count > 0) error = $"ERRORTRANSICIONAFECTACONTENIDO:{Enum.GetName(typeof(TiposContenidos), key)}";
                        break;
                    default:
                        break;
                }
            }
            return error;
        }

        public void GuardarHistorialTransicionDocumento(Guid pDocumentoID, Guid pTransicionID, string pComentario, Guid pIdentidadID)
        {
			HistorialTransicionDocumento historial = new HistorialTransicionDocumento();
			historial.HistorialTransicionID = Guid.NewGuid();
			historial.TransicionID = pTransicionID;
            historial.IdentidadID = pIdentidadID;
			historial.Comentario = pComentario;
			historial.DocumentoID = pDocumentoID;
			historial.Fecha = DateTime.Now;
			mEntityContext.HistorialTransicionDocumento.Add(historial);

			mEntityContext.SaveChanges();
		}

		public void GuardarHistorialTransicionComponenteCMS(Guid pComponenteID, Guid pTransicionID, string pComentario, Guid pIdentidadID)
		{
			HistorialTransicionCMSComponente historial = new HistorialTransicionCMSComponente();
			historial.HistorialTransicionID = Guid.NewGuid();
			historial.TransicionID = pTransicionID;
			historial.IdentidadID = pIdentidadID;
			historial.Comentario = pComentario;
			historial.ComponenteID = pComponenteID;
			historial.Fecha = DateTime.Now;
			mEntityContext.HistorialTransicionCMSComponente.Add(historial);

			mEntityContext.SaveChanges();
		}

		public void GuardarHistorialTransicionPestanyaCMS(Guid pPestanyaID, short pUbicacion, Guid pTransicionID, string pComentario, Guid pIdentidadID)
		{
			HistorialTransicionPestanyaCMS historial = new HistorialTransicionPestanyaCMS();
			historial.HistorialTransicionID = Guid.NewGuid();
			historial.TransicionID = pTransicionID;
			historial.IdentidadID = pIdentidadID;
			historial.Comentario = pComentario;
			historial.PestanyaID = pPestanyaID;
            historial.Ubicacion = pUbicacion;
			historial.Fecha = DateTime.Now;
			mEntityContext.HistorialTransicionPestanyaCMS.Add(historial);

			mEntityContext.SaveChanges();
		}

		public List<HistorialTransicionDocumento> ObtenerHistorialTransicionesDocumento(Guid pDocumentoID)
        {
			return mEntityContext.HistorialTransicionDocumento.Where(x => x.DocumentoID.Equals(pDocumentoID)).ToList();
		}

		public List<HistorialTransicionCMSComponente> ObtenerHistorialTransicionesComponenteCMS(Guid pComponenteID)
		{
			return mEntityContext.HistorialTransicionCMSComponente.Where(x => x.ComponenteID.Equals(pComponenteID)).ToList();
		}

		public List<HistorialTransicionPestanyaCMS> ObtenerHistorialTransicionesPestanya(Guid pPestanyaID)
		{
			return mEntityContext.HistorialTransicionPestanyaCMS.Where(x => x.PestanyaID.Equals(pPestanyaID)).ToList();
		}

        public string ObtenerNombreTransicion(Guid pTransicionID)
        {
            return mEntityContext.Transicion.Where(x => x.TransicionID.Equals(pTransicionID)).Select(x => x.Nombre).FirstOrDefault();
        }

        public bool ComprobarIdentidadTienePermisoRealizarTransicion(Guid pTransicionID, Guid pIdentidadID)
        {
            return mEntityContext.TransicionIdentidad.FirstOrDefault(x => x.IdentidadID.Equals(pIdentidadID) && x.TransicionID.Equals(pTransicionID)) != null;
        }

		public bool ComprobarGrupoTienePermisoRealizarTransicion(Guid pTransicionID, Guid pGrupoID)
		{
			return mEntityContext.TransicionGrupo.FirstOrDefault(x => x.GrupoID.Equals(pGrupoID) && x.TransicionID.Equals(pTransicionID)) != null;
		}

		public Guid ObtenerEstadoOrigenTransicion(Guid pTransicionID)
		{
			return mEntityContext.Transicion.Where(x => x.TransicionID.Equals(pTransicionID)).Select(x => x.EstadoOrigenID).FirstOrDefault();
		}

		public Guid ObtenerEstadoDestinoTransicion(Guid pTransicionID)
        {
            return mEntityContext.Transicion.Where(x => x.TransicionID.Equals(pTransicionID)).Select(x => x.EstadoDestinoID).FirstOrDefault();
        }

        #endregion

        #region TransicionIdentidad
        public List<Guid> ObtenerIdentidadesIDPorTransicionID(Guid pTransicionID)
        {
            return mEntityContext.TransicionIdentidad.Where(item => item.TransicionID.Equals(pTransicionID)).Select(item => item.IdentidadID).ToList();
        }
        public void GuardarTransicionIdentidad(TransicionIdentidad pTransicionIdentidad)
        {
            TransicionIdentidad transicionIdentidad = mEntityContext.TransicionIdentidad.Where(x => x.TransicionID.Equals(pTransicionIdentidad.TransicionID) && x.IdentidadID.Equals(pTransicionIdentidad.IdentidadID)).FirstOrDefault();
            if (transicionIdentidad == null)
            {
                mEntityContext.TransicionIdentidad.Add(pTransicionIdentidad);
                mEntityContext.SaveChanges();
            }
        }
        /// <summary>
        /// Eliminamos las TransicionIdentidad de una transicion modificada
        /// </summary>
        /// <param name="pListaIdentidadID"></param>
        /// <param name="pTransicionID"></param>
        public void EliminarTranscionIdentidad(List<Guid> pListaIdentidadID, Guid pTransicionID)
        {
            List<TransicionIdentidad> listaTranscionIdentidad = mEntityContext.TransicionIdentidad.Where(item => pListaIdentidadID.Contains(item.IdentidadID) && item.TransicionID.Equals(pTransicionID)).ToList();
            foreach (TransicionIdentidad transicionIdentidad in listaTranscionIdentidad)
            {
                mEntityContext.EliminarElemento(transicionIdentidad);
            }
            mEntityContext.SaveChanges();
        }

        #endregion

        #region TransicionGrupo
        public List<Guid> ObtenerGruposIDPorTransicionID(Guid pTransicionID)
        {
            return mEntityContext.TransicionGrupo.Where(item => item.TransicionID.Equals(pTransicionID)).Select(item => item.GrupoID).ToList();
        }
        public void GuardarTransicionGrupo(TransicionGrupo pTransicionGrupo)
        {
            TransicionGrupo transicionGrupo = mEntityContext.TransicionGrupo.Where(x => x.TransicionID.Equals(pTransicionGrupo.TransicionID) && x.GrupoID.Equals(pTransicionGrupo.GrupoID)).FirstOrDefault();
            if (transicionGrupo == null)
            {
                mEntityContext.TransicionGrupo.Add(pTransicionGrupo);
                mEntityContext.SaveChanges();
            }
        }
        /// <summary>
        /// Eliminamos las TranscionesGrupo de una transicion modificada
        /// </summary>
        /// <param name="pListaGrupoID"></param>
        /// <param name="pTransicionID"></param>
        public void EliminarTranscionesGrupo(List<Guid> pListaGrupoID, Guid pTransicionID)
        {
            List<TransicionGrupo> listaTranscionGrupo = mEntityContext.TransicionGrupo.Where(item => pListaGrupoID.Contains(item.GrupoID) && item.TransicionID.Equals(pTransicionID)).ToList();
            foreach (TransicionGrupo transicionGrupo in listaTranscionGrupo)
            {
                mEntityContext.EliminarElemento(transicionGrupo);
            }
            mEntityContext.SaveChanges();
        }

        #endregion

        #endregion
    }
}
