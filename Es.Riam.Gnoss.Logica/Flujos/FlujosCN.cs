using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.AD.Flujos;
using Es.Riam.Gnoss.AD.Identidad;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.RabbitMQ;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Administracion;
using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using Es.Riam.Interfaces.InterfacesOpen;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;

namespace Es.Riam.Gnoss.Logica.Flujos
{
    public class FlujosCN : BaseCN, IDisposable
    {
        private const string COLA_FLUJOS = "ColaFlujos";
        #region Miembros

        private EntityContext mEntityContext;
        private LoggingService mLoggingService;
        private ConfigService mConfigService;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        #endregion

        #region Constructores

        public FlujosCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<FlujosCN> logger, ILoggerFactory loggerFactory) : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication, logger, loggerFactory)
        {
            mEntityContext = entityContext;
            mLoggingService = loggingService;
            mConfigService = configService;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
            if (loggerFactory == null)
            {
                FlujosAD = new FlujosAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, null, null);
            }
            else
            {
                FlujosAD = new FlujosAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication, mLoggerFactory.CreateLogger<FlujosAD>(), mLoggerFactory);
            }
        }

        #endregion

        #region Metodos Publicos

        #region Flujo
        public List<string> ObtenerOntologiaNombrePorPoryectoID(Guid pProyectoID)
        {
            return FlujosAD.ObtenerOntologiaNombrePorPoryectoID(pProyectoID);
        }
        public List<string> ObtenerOntologiasNombreFlujo(Guid pFlujoID)
        {
            return FlujosAD.ObtenerOntologiasNombreFlujo(pFlujoID);
        }
        public Dictionary<Guid, string> ObtenerOntologiasFlujo(Guid pFlujoID, Dictionary<Guid, string> pOntologiasProyecto)
        {
            return FlujosAD.ObtenerOntologiasFlujo(pFlujoID, pOntologiasProyecto);
        }
        public bool ExisteFlujoProyecto(Guid pFlujoID, Guid pProyectoID)
        {
            return FlujosAD.ExisteFlujoProyecto(pFlujoID, pProyectoID);
        }
        public List<Flujo> ObtenerFlujosPorProyecto(Guid pProyectoID)
        {
            return FlujosAD.ObtenerFlujosPorProyecto(pProyectoID);
        }
        public Flujo ObtenerFlujoPorFlujoID(Guid pFlujoID)
        {
            return FlujosAD.ObtenerFlujoPorFlujoID(pFlujoID);
        }

		public Guid ObtenerFlujoIDDeOntologia(Guid pProyectoID, string pNombre)
        {
            return FlujosAD.ObtenerFlujoIDDeOntologia(pProyectoID, pNombre);
        }

		public Guid ObtenerFlujoIDPorProyectoYTipoContenido(Guid pProyectoID, TiposContenidos pTipo)
        {
            return FlujosAD.ObtenerFlujoIDPorProyectoYTipoContenido(pProyectoID, pTipo);
        }

		public string ExisteFlujoAplicadoEnTiposContenidoPorProyecto(Guid pFlujoID, Guid pProyectoID, Dictionary<TiposContenidos, bool> pTipoContenidos, Dictionary<Guid, string> pOntologiasID)
        {
            return FlujosAD.ExisteFlujoAplicadoEnTiposContenidoPorProyecto(pFlujoID, pProyectoID, pTipoContenidos, pOntologiasID);
        }
        public Dictionary<TiposContenidos, bool> ObtenerTiposContenidosPorFlujoID(Guid pFlujoID)
        {
            return FlujosAD.ObtenerTiposContenidosPorFlujoID(pFlujoID);
        }
        public Dictionary<TiposContenidos, bool> ObtenerTiposContenidosPorFlujo(Flujo pFlujo)
        {
            return FlujosAD.ObtenerTiposContenidosPorFlujo(pFlujo);
        }
        public Dictionary<TiposContenidos, bool> ObtenerTiposContenidosEnProyecto(Guid pProyectoID)
        {
            return FlujosAD.ObtenerTiposContenidosEnProyecto(pProyectoID);
        }

        public void GuardarFlujo(Flujo pFlujo)
        {
            FlujosAD.GuardarFlujo(pFlujo);
        }


        public void EliminarFlujo(Guid pFlujoID, Guid pProyectoID)
        {
            FlujosAD.EliminarFlujo(pFlujoID, pProyectoID);
        }
        public void GuardarFlujoObjetoConocimientoProyecto(FlujoObjetoConocimientoProyecto pFlujoOCProyecto)
        {
            FlujosAD.GuardarFlujoObjetoConocimientoProyecto(pFlujoOCProyecto);
        }
        public void EliminarFlujoObjetoConocimientoProyectoPorNombreOntologia(Guid pFlujoID, string pNombreOntologia)
        {
            FlujosAD.EliminarFlujoObjetoConocimientoProyectoPorNombreOntologia(pFlujoID, pNombreOntologia);
        }
        public void EliminarFlujoObjetoConocimientoProyecto(Guid pFlujoID)
        {
            FlujosAD.EliminarFlujoObjetoConocimientoProyecto(pFlujoID);
        }

        #endregion

        #region Estado

        public void GuardarEstado(Estado pEstado)
        {
            FlujosAD.GuardarEstado(pEstado);
        }
        public void EliminarEstados(List<Guid> pListaEstadoID, Guid pFlujoID)
        {
            FlujosAD.EliminarEstados(pListaEstadoID, pFlujoID);
        }
        public string PuedoEliminarEstado(Guid pEstadoID, Guid pFlujoID)
        {
            return FlujosAD.PuedoEliminarEstado(pEstadoID, pFlujoID);
        }
        public List<Guid> ObtenerEstadosIDPorFlujoID(Guid pFlujoID)
        {
            return FlujosAD.ObtenerEstadosIDPorFlujoID(pFlujoID);
        }
        public List<Estado> ObtenerEstadosPorFlujoID(Guid pFlujoID)
        {
            return FlujosAD.ObtenerEstadosPorFlujoID(pFlujoID);
        }
        public Dictionary<Guid, Guid> ActualizarEstadosRecursos(Guid? pEstadoID, Guid pProyectoID, List<Guid> pOntologiaID, short pTipoContenido, bool pEliminado = false)
        {
            return FlujosAD.ActualizarEstadosRecursos(pEstadoID, pProyectoID, pOntologiaID, pTipoContenido, pEliminado);
        }
        public void ActualizarEstadosVersionRecursos(List<Guid> pListaDocumentosID, Guid? pEstadoID, bool pEliminado = false)
        {
            FlujosAD.ActualizarEstadosVersionRecursos(pListaDocumentosID, pEstadoID, pEliminado);
        }
        public Dictionary<Guid, Guid> ActualizarEstadoPaginasCMS(Guid? pEstadoID, Guid pProyectoID, bool pEliminado = false)
        {
            return FlujosAD.ActualizarEstadoPaginasCMS(pEstadoID, pProyectoID, pEliminado);
        }
        public Dictionary<Guid, Guid> ActualizarEstadoComponentesCMS(Guid? pEstadoID, Guid pProyectoID, bool pEliminado = false)
        {
            return FlujosAD.ActualizarEstadoComponentesCMS(pEstadoID, pProyectoID, pEliminado);
        }

        public string ComprobarSiPaginasCMSNoEstanEnEstadoFinal(List<Guid> pListaEstadosFinales, Guid pProyectoID)
        {
            return FlujosAD.ComprobarSiPaginasCMSNoEstanEnEstadoFinal(pListaEstadosFinales, pProyectoID);
        }

        public string ComprobarSiComponenteCMSNoEstanEnEstadoFinal(List<Guid> pListaEstadosFinales, Guid pProyectoID)
        {
            return FlujosAD.ComprobarSiComponenteCMSNoEstanEnEstadoFinal(pListaEstadosFinales, pProyectoID);
        }

        public string ComprobarSiRecursosNoEstanEstadoFinal(List<Guid> pListaEstadosFinales, Guid pProyectoID, List<short> pTipoRecursos)
        {
            return FlujosAD.ComprobarSiRecursosNoEstanEstadoFinal(pListaEstadosFinales, pProyectoID, pTipoRecursos);
        }

        public string ComprobarSiRecursosSemNoEstanEstadoFinal(List<Guid> pListaEstadosFinales, Guid pProyectoID, Dictionary<Guid, string> pListaOntologias)
        {
            return FlujosAD.ComprobarSiRecursosSemNoEstanEstadoFinal(pListaEstadosFinales, pProyectoID, pListaOntologias);
        }

        public List<Guid> ObtenerEstadosFinalesPorFlujoID(Guid pFlujoID)
        {
            return FlujosAD.ObtenerEstadosFinalesPorFlujoID(pFlujoID);
        }

        public bool ComprobarEstadoEsPublico(Guid pEstadoID)
		{
            return FlujosAD.ComprobarEstadoEsPublico(pEstadoID);
		}

		public Estado ObtenerEstadoPorEstadoID(Guid pEstadoID)
        {
            return FlujosAD.ObtenerEstadoPorEstadoID(pEstadoID);
        }

        public List<Guid> ObtenerIdentidadesLectorasEstado(Guid pEstadoID)
        {
            return FlujosAD.ObtenerIdentidadesLectorasEstado(pEstadoID);
        }

		public List<Guid> ObtenerIdentidadesEditorasEstado(Guid pEstadoID)
        {
            return FlujosAD.ObtenerIdentidadesEditorasEstado(pEstadoID);
        }

		public List<Guid> ObtenerGruposLectoresEstado(Guid pEstadoID)
        {
            return FlujosAD.ObtenerGruposLectoresEstado(pEstadoID);
        }

		public List<Guid> ObtenerGruposEditoresEstado(Guid pEstadoID)
        {
            return FlujosAD.ObtenerGruposEditoresEstado(pEstadoID);
        }

		public List<Guid> ObtenerLectoresYEditoresDeEstado(Guid pEstadoID)
        {
            return FlujosAD.ObtenerLectoresYEditoresDeEstado(pEstadoID);
        }
        public List<Guid> ObtenerEditoresDeUnaMejora(Guid pMejoraID)
        {
            return FlujosAD.ObtenerEditoresDeUnaMejora(pMejoraID);
        }

        public string ObtenerNombreDeEstado(Guid pEstadoID)
        {
            return FlujosAD.ObtenerNombreDeEstado(pEstadoID);
        }

		public List<Transicion> ObtenerTransicionesDeEstadoDeIdentidad(Guid pEstadoID, Guid pIdentidadID)
        {
            return FlujosAD.ObtenerTransicionesDeEstadoDeIdentidad(pEstadoID, pIdentidadID);
        }

		public List<Transicion> ObtenerTransicionesDeEstadoDeGrupo(Guid pEstadoID, Guid pGrupoID)
        {
			return FlujosAD.ObtenerTransicionesDeEstadoDeGrupo(pEstadoID, pGrupoID);
		}

        public void ActualizarEditoresRecursos(Dictionary<Guid, Guid> pDiccionarioRecursoIDEstadoID)
        {
            FlujosAD.ActualizarEditoresRecursos(pDiccionarioRecursoIDEstadoID);
        }

		public Guid? ObtenerEstadoInicialDeTipoContenido(Guid pProyectoID, TiposContenidos pTipo, Guid? pFlujoID = null)
        {
            return FlujosAD.ObtenerEstadoInicialDeTipoContenido(pProyectoID, pTipo, pFlujoID);
        }

		public Guid ObtenerEstadoInicialDeFlujo(Guid pFlujoID)
		{
			return FlujosAD.ObtenerEstadoInicialDeFlujo(pFlujoID);
		}

		public Guid ObtenerFlujoIDDeEstadoID(Guid pEstadoID)
        {
            return FlujosAD.ObtenerFlujoIDDeEstadoID(pEstadoID);
        }

        public bool ObtenerPermiteMejoraPorEstadoID(Guid pEstadoID)
        {
            return FlujosAD.ObtenerPermiteMejoraPorEstadoID(pEstadoID);
        }

        #endregion

        #region EstadoIdentidad
        public List<Guid> ObtenerIdentidadesDeEstadoPorID(Guid pEstadoID)
        {
            return FlujosAD.ObtenerIdentidadesDeEstadoPorID(pEstadoID);
        }
        public EstadoIdentidad ObtenerEstadoIdentidad(Guid pEstadoID, Guid pIdentidadID)
        {
            return FlujosAD.ObtenerEstadoIdentidad(pEstadoID, pIdentidadID);
        }
        public void GuardarEstadoIdentidad(EstadoIdentidad pEstadoIdentidad)
        {
            FlujosAD.GuardarEstadoIdentidad(pEstadoIdentidad);
        }
        public void EliminarEstadoIdentidad(List<Guid> pListaIdentidadID, Guid pEstadoID)
        {
            FlujosAD.EliminarEstadoIdentidad(pListaIdentidadID, pEstadoID);
        }

		public bool ComprobarIdentidadTienePermisoLecturaEnEstado(Guid pEstadoID, Guid pIdentidadID, Guid pDocumentoID)
		{
			return FlujosAD.ComprobarIdentidadTienePermisoLecturaEnEstado(pEstadoID, pIdentidadID, pDocumentoID);
		}

		public bool ComprobarIdentidadTienePermisoEdicionEnEstado(Guid pEstadoID, Guid pIdentidadID, Guid pDocumentoID)
        {
            return FlujosAD.ComprobarIdentidadTienePermisoEdicionEnEstado(pEstadoID, pIdentidadID, pDocumentoID);
		}

        public bool EsEstadoInicial(Guid pEstadoID)
        {
            return FlujosAD.EsEstadoInicial(pEstadoID);
        }

        public bool EsEstadoFinal(Guid pEstadoID)
        {
            return FlujosAD.EsEstadoFinal(pEstadoID);
        }

		public bool ComprobarSiEsCreadorYEstadoInicial(Guid pIdentidadID, Guid pDocumentoID, Guid pEstadoID)
		{
            return FlujosAD.ComprobarSiEsCreadorYEstadoInicial(pIdentidadID, pDocumentoID, pEstadoID);
        }

		#endregion

		#region EstadoGrupo
		public List<Guid> ObtenerGruposIDPorEstadoID(Guid pEstadoID)
        {
            return FlujosAD.ObtenerGruposIDPorEstadoID(pEstadoID);
        }
        public EstadoGrupo ObtenerEstadoGrupo(Guid pEstadoID, Guid pGrupoID)
        {
            return FlujosAD.ObtenerEstadoGrupo(pEstadoID, pGrupoID);
        }
        public void GuardarEstadoGrupo(EstadoGrupo pEstadoGrupo)
        {
            FlujosAD.GuardarEstadoGrupo(pEstadoGrupo);
        }
        public void EliminarEstadosGrupo(List<Guid> pListaGrupoID, Guid pEstadoID)
        {
            FlujosAD.EliminarEstadoGrupo(pListaGrupoID, pEstadoID);
        }

        #endregion

        #region Transicion

        public void GuardarTransicion(Transicion pTransicion)
        {
            FlujosAD.GuardarTransicion(pTransicion);
        }
        public void EliminarTransiciones(List<Guid> pListaTransicionID)
        {
            FlujosAD.EliminarTransiciones(pListaTransicionID);
        }
        public string PuedoEliminarTransicion(Guid pTransicionID, Guid pFlujoID)
        {
            return FlujosAD.PuedoEliminarTransicion(pTransicionID, pFlujoID);
        }
        public List<Guid> ObtenerTransicionesIDPorEstadosID(List<Guid> pListaEstadosID)
        {
            return FlujosAD.ObtenerTransicionesIDPorEstadosID(pListaEstadosID);
        }
        public List<Transicion> ObtenerTransicionesPorEstadosID(List<Guid> pListaEstadosID)
        {
            return FlujosAD.ObtenerTransicionesPorEstadosID(pListaEstadosID);
        }

		public void GuardarHistorialTransicionDocumento(Guid pDocumentoID, Guid pTransicionID, string pComentario, Guid pIdentidadID)
        {
            FlujosAD.GuardarHistorialTransicionDocumento(pDocumentoID, pTransicionID, pComentario, pIdentidadID);
        }

		public void GuardarHistorialTransicionComponenteCMS(Guid pComponenteID, Guid pTransicionID, string pComentario, Guid pIdentidadID)
        {
            FlujosAD.GuardarHistorialTransicionComponenteCMS(pComponenteID, pTransicionID, pComentario, pIdentidadID);
        }

		public void GuardarHistorialTransicionPestanyaCMS(Guid pPestanyaID, short pUbicacion, Guid pTransicionID, string pComentario, Guid pIdentidadID)
        {
            FlujosAD.GuardarHistorialTransicionPestanyaCMS(pPestanyaID, pUbicacion, pTransicionID, pComentario, pIdentidadID);
        }

		public List<HistorialTransicionDocumento> ObtenerHistorialTransicionesDocumento(Guid pDocumentoID)
        {
            return FlujosAD.ObtenerHistorialTransicionesDocumento(pDocumentoID);
        }

        public List<HistorialTransicionDocumento> ObtenerHistorialTransicionesDocumento(List<Guid> pDocumentosID)
        {
            return FlujosAD.ObtenerHistorialTransicionesDocumento(pDocumentosID);
        }

        public List<HistorialTransicionCMSComponente> ObtenerHistorialTransicionesComponenteCMS(Guid pComponenteID)
        {
            return FlujosAD.ObtenerHistorialTransicionesComponenteCMS(pComponenteID);
        }

		public List<HistorialTransicionPestanyaCMS> ObtenerHistorialTransicionesPestanya(Guid pPestanyaID)
        {
            return FlujosAD.ObtenerHistorialTransicionesPestanya(pPestanyaID);
        }

		public string ObtenerNombreTransicion(Guid pTransicionID)
        {
            return FlujosAD.ObtenerNombreTransicion(pTransicionID);
        }

		public bool ComprobarIdentidadTienePermisoRealizarTransicion(Guid pTransicionID, Guid pIdentidadID)
        {
            return FlujosAD.ComprobarIdentidadTienePermisoRealizarTransicion(pTransicionID, pIdentidadID);
        }

		public bool ComprobarGrupoTienePermisoRealizarTransicion(Guid pTransicionID, Guid pGrupoID)
        {
            return FlujosAD.ComprobarGrupoTienePermisoRealizarTransicion(pTransicionID, pGrupoID);
        }

		public Guid ObtenerEstadoOrigenTransicion(Guid pTransicionID)
		{
			return FlujosAD.ObtenerEstadoOrigenTransicion(pTransicionID);
		}

		public Guid ObtenerEstadoDestinoTransicion(Guid pTransicionID)
        {
            return FlujosAD.ObtenerEstadoDestinoTransicion(pTransicionID);
        }

        public void CambiarEstadoContenido(Guid pContenidoID, Guid pEstadoDestinoID, TipoContenidoFlujo pTipo)
        {
            FlujosAD.CambiarEstadoContenido(pContenidoID, pEstadoDestinoID, pTipo);
        }

		public Guid ObtenerEstadoIDDeContenido(Guid pContenidoID, TipoContenidoFlujo pTipo)
		{
            return FlujosAD.ObtenerEstadoIDDeContenido(pContenidoID, pTipo);
		}

		#endregion

		#region TransicionIdentidad
		public List<Guid> ObtenerIdentidadesIDPorTransicionID(Guid pTransicionID)
        {
            return FlujosAD.ObtenerIdentidadesIDPorTransicionID(pTransicionID);
        }
        public void GuardarTransicionIdentidad(TransicionIdentidad pEstadoIdentidad)
        {
            FlujosAD.GuardarTransicionIdentidad(pEstadoIdentidad);
        }
        public void EliminarTranscionesIdentidad(List<Guid> pListaIdentidadID, Guid pTransicionID)
        {
            FlujosAD.EliminarTranscionIdentidad(pListaIdentidadID, pTransicionID);
        }

        #endregion

        #region TransicionGrupo
        public List<Guid> ObtenerGruposIDPorTransicionID(Guid pTransicionID)
        {
            return FlujosAD.ObtenerGruposIDPorTransicionID(pTransicionID);
        }
        public void GuardarTransicionGrupo(TransicionGrupo pEstadoGrupo)
        {
            FlujosAD.GuardarTransicionGrupo(pEstadoGrupo);
        }
        public void EliminarTranscionesGrupo(List<Guid> pListaGrupoID, Guid pTransicionID)
        {
            FlujosAD.EliminarTranscionesGrupo(pListaGrupoID, pTransicionID);
        }

        #endregion

        #region Cola Rabbit

        public void InsertarEnColaFlujosCreadosOEliminados(Guid pFlujoID, Guid? pEstadoID, Guid pProyectoID, List<Guid> pOntologiasAfectadas, List<TiposContenidos> pTiposContenidos, bool pEliminarFlujo, bool pEliminarEstado, Guid pUsuarioID, IAvailableServices pAvailableServices)
        {
            if (mConfigService.ExistRabbitConnection(RabbitMQClient.BD_SERVICIOS_WIN) && pAvailableServices.CheckIfServiceIsAvailable(pAvailableServices.GetBackServiceCode(BackgroundService.Workflows), ServiceType.Background))
            {
                using (RabbitMQClient rabbitMQ = new RabbitMQClient(RabbitMQClient.BD_SERVICIOS_WIN, COLA_FLUJOS, mLoggingService, mConfigService, mLoggerFactory.CreateLogger<RabbitMQClient>(), mLoggerFactory))
                {
                    ColaProcesarFlujo filaCola = new ColaProcesarFlujo();
                    filaCola.FlujoID = pFlujoID;
                    filaCola.EstadoID = pEstadoID;
                    filaCola.ProyectoID = pProyectoID;
                    filaCola.OntologiasAfectadas = pOntologiasAfectadas;
                    filaCola.TiposAfectados = pTiposContenidos;
                    filaCola.EliminarFlujo = pEliminarFlujo;
                    filaCola.EliminarEstado = pEliminarEstado;
                    filaCola.UsuarioID = pUsuarioID;

                    rabbitMQ.AgregarElementoACola(JsonConvert.SerializeObject(filaCola));
                }
            }
        }

        #endregion

        #endregion

        #region Propiedades 

        public FlujosAD FlujosAD
        {
            get
            {
                return (FlujosAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~FlujosCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //impido que se finalice dos veces este objeto
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="pDisposing">Determina si se está llamando desde el Dispose()</param>
        protected virtual void Dispose(bool pDisposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;

                if (pDisposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (this.FlujosAD != null)
                    {
                        FlujosAD.Dispose();
                    }
                }
                FlujosAD = null;
            }
        }

        #endregion
    }
}
