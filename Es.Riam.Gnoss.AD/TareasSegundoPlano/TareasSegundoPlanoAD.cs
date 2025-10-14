using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.ParametroAplicacion;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Riam.Gnoss.AD.TareasSegundoPlano
{
	public class TareasSegundoPlanoAD : BaseAD
	{
		private EntityContext mEntityContext;
        private ILogger mlogger;
        private ILoggerFactory mLoggerFactory;

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public TareasSegundoPlanoAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication, ILogger<TareasSegundoPlanoAD> logger, ILoggerFactory loggerFactory)
			: base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication,logger,loggerFactory)
		{
			mEntityContext = entityContext;
            mlogger = logger;
            mLoggerFactory = loggerFactory;
        }



		/// <summary>
		/// Inserta una nueva tarea en la tabla TareasSegundoPlano de la base de datos con los datos introducidos, estado “pendiente” y un identificador generado automáticamente.
		/// </summary>
		/// <param name="pProyectoId">identificador del proyecto al que pertenece la tarea.</param>
		/// <param name="pOrganizacionId">identificador de la organización a la que pertenece el proyecto</param>
		/// <param name="pTipo">tipo de tarea</param>
		/// <param name="pNombre">nombre de la tarea</param>
		/// <param name="pFechaInicio">fecha de inicio de la tarea</param>
		/// <param name="pEventosTotales">número de eventos que constituyen la tarea, sin contar los mensajes de inicio y fin.</param>
		/// <returns>Identificador global único de la nueva tarea</returns>
		public Guid NuevaTarea(Guid pProyectoId, Guid pOrganizacionId, string pTipo, string pNombre, DateTime pFechaInicio, int pEventosTotales){
			Guid tareaId = Guid.NewGuid();
			mEntityContext.TareasSegundoPlano.Add(
				new TareasSegundoPlano() {
					Id = tareaId,
					ProyectoID = pProyectoId,
					OrganizacionID = pOrganizacionId,
					Tipo = pTipo,
					Nombre = pNombre,
					Estado = EstadoTarea.Pendiente,
					FechaInicio = pFechaInicio,
					EventosTotales = pEventosTotales,
				});
			mEntityContext.SaveChanges();
			return tareaId;
		}

		/// <summary>
		/// Obtiene todas las tareas en segundo plano asociadas a un proyecto
		/// </summary>
		/// <param name="pProyectoId">identificador del proyecto</param>
		/// <returns>Lista de las tareas en segundo plano del proyecto</returns>
		public List<TareasSegundoPlano> TareasDeProyecto(Guid pProyectoId)
		{
			return mEntityContext.TareasSegundoPlano.Where((tarea)=>(tarea).ProyectoID.Equals(pProyectoId)).ToList();
		}


		/// <summary>
		/// Modifica el estado de una tarea en segundo plano
		/// </summary>
		/// <param name="pTareaId">identificador de la tarea en segundo plano</param>
		/// <param name="pEstado">nuevo estado</param>
		public void ActualizarEstado(Guid pTareaId, EstadoTarea pEstado)
		{
			TareasSegundoPlano tarea = mEntityContext.TareasSegundoPlano.Where((tarea) => (tarea).Id.Equals(pTareaId)).FirstOrDefault();
			tarea.Estado = pEstado;
			mEntityContext.TareasSegundoPlano.Update(tarea);
			mEntityContext.SaveChanges();
		}

		/// <summary>
		/// Elimina todas las entradas de un proyecto de la tabla TareasSegundoPlano 
		/// </summary>
		/// <param name="pProyectoId">identificador del proyecto</param>
		public void VaciarTablaDeProyecto(Guid pProyectoId)
		{
			mEntityContext.RemoveRange(mEntityContext.TareasSegundoPlano.Where((tarea) => (tarea).ProyectoID.Equals(pProyectoId)).ToArray());
			mEntityContext.SaveChanges();
		}
	}
}
