using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.Flujos;
using Es.Riam.Gnoss.AD.Flujos;
using Es.Riam.Gnoss.Logica.Flujos;
using Es.Riam.Gnoss.Logica.Identidad;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Web.MVC.Models.Flujos;
using Es.Riam.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Es.Riam.Gnoss.UtilServiciosWeb
{	
	public class UtilFlujos
	{
		private EntityContext _entityContext;
		private LoggingService _loggingService;
		private ConfigService _configService;
		private ILogger _logger;
		private ILoggerFactory _loggerFactory;

		public UtilFlujos(EntityContext pEntityContext, LoggingService pLoggingService, ConfigService pConfigService, ILogger<UtilFlujos> logger, ILoggerFactory loggerFactory)
		{
			_entityContext = pEntityContext;
			_loggingService = pLoggingService;
			_configService = pConfigService;
			_logger = logger;
			_loggerFactory = loggerFactory;
		}

		public EstadoModel ObtenerEstadoDeContenido(Guid pEstadoID, Guid pIdentidadID, string pIdioma, string pIdiomaDefecto)
		{
			FlujosCN flujosCN = new FlujosCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<FlujosCN>(), _loggerFactory);
			Estado filaEstado = flujosCN.ObtenerEstadoPorEstadoID(pEstadoID);
			if (filaEstado != null)
			{
				EstadoModel estado = new EstadoModel();
				estado.EstadoID = pEstadoID;
				estado.FlujoID = filaEstado.FlujoID;
				estado.Nombre = UtilCadenas.ObtenerTextoDeIdioma(filaEstado.Nombre, pIdioma, pIdiomaDefecto);
				estado.Publico = filaEstado.Publico;
				estado.GruposLectores = flujosCN.ObtenerGruposLectoresEstado(pEstadoID);
				estado.GruposEditores = flujosCN.ObtenerGruposEditoresEstado(pEstadoID);
				estado.IdentidadesLectoras = flujosCN.ObtenerIdentidadesLectorasEstado(pEstadoID);
				estado.IdentidadesEditoras = flujosCN.ObtenerIdentidadesEditorasEstado(pEstadoID);
				estado.Transiciones = CargarTransicionesDeEstado(pEstadoID, pIdentidadID, pIdioma, pIdiomaDefecto);

				return estado;
			}

			return null;
		}		

		public List<HistorialTransicionModel> ObtenerHistorialDeTransiciones(Guid pContenidoID, TipoContenidoFlujo pTipo, string pIdioma, string pIdiomaDefecto)
		{
			FlujosCN flujosCN = new FlujosCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<FlujosCN>(), _loggerFactory);
			IdentidadCN identidadCN = new IdentidadCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<IdentidadCN>(), _loggerFactory);
			List<HistorialTransicionModel> historial = new List<HistorialTransicionModel>();

			switch (pTipo)
			{
				case TipoContenidoFlujo.Recurso:
					List<HistorialTransicionDocumento> filasHistorialDoc = flujosCN.ObtenerHistorialTransicionesDocumento(pContenidoID);
					foreach (HistorialTransicionDocumento fila in filasHistorialDoc)
					{
						HistorialTransicionModel modeloHistorial = new HistorialTransicionModel();
						modeloHistorial.HistorialID = fila.HistorialTransicionID;
						modeloHistorial.NombreTransicion = UtilCadenas.ObtenerTextoDeIdioma(flujosCN.ObtenerNombreTransicion(fila.TransicionID), pIdioma, pIdiomaDefecto);
						modeloHistorial.Comentario = HttpUtility.UrlDecode(fila.Comentario);
						modeloHistorial.Revisor = identidadCN.ObtenerNombreDeIdentidad(fila.IdentidadID);
						modeloHistorial.Fecha = fila.Fecha;
						modeloHistorial.DocumentoID = pContenidoID;

						historial.Add(modeloHistorial);
					}
					break;
				case TipoContenidoFlujo.ComponenteCMS:
					List<HistorialTransicionCMSComponente> filasHistorialCMS = flujosCN.ObtenerHistorialTransicionesComponenteCMS(pContenidoID);
					foreach (HistorialTransicionCMSComponente fila in filasHistorialCMS)
					{
						HistorialTransicionModel modeloHistorial = new HistorialTransicionModel();
						modeloHistorial.HistorialID = fila.HistorialTransicionID;
						modeloHistorial.NombreTransicion = UtilCadenas.ObtenerTextoDeIdioma(flujosCN.ObtenerNombreTransicion(fila.TransicionID), pIdioma, pIdiomaDefecto);
						modeloHistorial.Comentario = HttpUtility.UrlDecode(fila.Comentario);
						modeloHistorial.Revisor = identidadCN.ObtenerNombreDeIdentidad(fila.IdentidadID);
						modeloHistorial.Fecha = fila.Fecha;
						modeloHistorial.DocumentoID = pContenidoID;

						historial.Add(modeloHistorial);
					}
					break;
				case TipoContenidoFlujo.PaginaCMS:
					List<HistorialTransicionPestanyaCMS> filasHistorialPagina = flujosCN.ObtenerHistorialTransicionesPestanya(pContenidoID);
					foreach (HistorialTransicionPestanyaCMS fila in filasHistorialPagina)
					{
						HistorialTransicionModel modeloHistorial = new HistorialTransicionModel();
						modeloHistorial.HistorialID = fila.HistorialTransicionID;
						modeloHistorial.NombreTransicion = UtilCadenas.ObtenerTextoDeIdioma(flujosCN.ObtenerNombreTransicion(fila.TransicionID), pIdioma, pIdiomaDefecto);
						modeloHistorial.Comentario = HttpUtility.UrlDecode(fila.Comentario);
						modeloHistorial.Revisor = identidadCN.ObtenerNombreDeIdentidad(fila.IdentidadID);
						modeloHistorial.Fecha = fila.Fecha;
						modeloHistorial.DocumentoID = pContenidoID;

						historial.Add(modeloHistorial);
					}
					break;
			}

			flujosCN.Dispose();
			identidadCN.Dispose();

			return historial;
		}

		public bool ComprobarPermisoEdicionEstado(EstadoModel pEstado, Guid pIdentidadID)
		{
			bool tienePermiso = pEstado.IdentidadesEditoras.Contains(pIdentidadID);
			if (!tienePermiso)
			{
				IdentidadCN identidadCN = new IdentidadCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<IdentidadCN>(), _loggerFactory);
				List<Guid> gruposIdentidad = identidadCN.ObtenerIDGruposDeIdentidad(pIdentidadID);

				tienePermiso = gruposIdentidad.Any(g => pEstado.GruposEditores.Contains(g));
			}

			return tienePermiso;
		}

		public List<TransicionModel> CargarTransicionesDeEstado(Guid pEstadoID, Guid pIdentidadID, string pIdioma, string pIdiomaDefecto)
		{
			FlujosCN flujosCN = new FlujosCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<FlujosCN>(), _loggerFactory);
			List<TransicionModel> listaTransiciones = new List<TransicionModel>();
			List<Transicion> filasTransiciones = flujosCN.ObtenerTransicionesDeEstadoDeIdentidad(pEstadoID, pIdentidadID);

			IdentidadCN identidadCN = new IdentidadCN(_entityContext, _loggingService, _configService, null, _loggerFactory.CreateLogger<IdentidadCN>(), _loggerFactory);
			List<Guid> listaGrupos = identidadCN.ObtenerIDGruposDeIdentidad(pIdentidadID);
			foreach (Guid grupoID in listaGrupos)
			{
				List<Transicion> transicionesGrupo = flujosCN.ObtenerTransicionesDeEstadoDeGrupo(pEstadoID, grupoID);
				filasTransiciones = filasTransiciones.Union(transicionesGrupo).ToList();
			}

			foreach (Transicion transicion in filasTransiciones)
			{
				if (!listaTransiciones.Any(t => t.TransicionID.Equals(transicion.TransicionID)))
				{
					TransicionModel transicionModel = new TransicionModel();
					transicionModel.Nombre = UtilCadenas.ObtenerTextoDeIdioma(transicion.Nombre, pIdioma, pIdiomaDefecto);
					transicionModel.TransicionID = transicion.TransicionID;

					listaTransiciones.Add(transicionModel);
				}
			}

			return listaTransiciones;
		}
	}
}
