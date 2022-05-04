using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    /// <summary>
    /// Lógica para modelo BASE de comunidad
    /// </summary>
    public class ReplicacionCN : BaseCN, IDisposable
    {
        public ReplicacionCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            ReplicacionAD = new ReplicacionAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #region Métodos generales

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de replicacion MASTER
        /// </summary>
        /// <param name="pPrioridadBase">Prioridad de los elementos que se quieren obtener</param>
        /// <param name="pNumMaxItems">Número máximo de Items a obtener</param>
        /// <returns></returns>
        public BaseComunidadDS ObtenerElementosPendientesColaReplicacion(int pNumMaxItems, short pEstadoMaximo)
        {
            return ObtenerElementosPendientesColaReplicacion(pNumMaxItems, "ColaReplicacionMaster", pEstadoMaximo);
        }

        /// <summary>
        /// Obtiene los elementos pendientes de una cola de replicacion
        /// </summary>
        /// <param name="pPrioridadBase">Prioridad de los elementos que se quieren obtener</param>
        /// <param name="pNumMaxItems">Número máximo de Items a obtener</param>
        /// <param name="pTablaColaReplica">Tabla de la cola que se quiere cargar</param>
        /// <returns></returns>
        public BaseComunidadDS ObtenerElementosPendientesColaReplicacion(int pNumMaxItems, string pTablaColaReplica, short pEstadoMaximo)
        {
            return ReplicacionAD.ObtenerElementosPendientesColaReplicacion(pNumMaxItems, pTablaColaReplica, pEstadoMaximo);
        }

        public BaseComunidadDS ObtenerElementosColaReplicacionMismaTransaccion(string pNombreTablaReplica, short pEstadoMaximo, string pInfoExtra)
        {
            return ReplicacionAD.ObtenerElementosColaReplicacionMismaTransaccion(pNombreTablaReplica, pEstadoMaximo, pInfoExtra);
        }

        /// <summary>
        /// Inserta en una cola de una réplica particular una consulta
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador de la consulta a replicar</param>
        /// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a replicar la consulta</param>
        public void InsertarConsultaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica)
        {
            InsertarConsultaEnReplica(pOrdenEjecucion, pNombreTablaReplica, "ColaReplicacionMaster");
        }

        /// <summary>
        /// Inserta en una cola de una réplica particular una consulta
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador de la consulta a replicar</param>
        /// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a replicar la consulta</param>
        /// <param name="pNombreTablaOrigen">Nombre de la tabla de origen desde la que se va a copiar la fila</param>
        public void InsertarConsultaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica, string pNombreTablaOrigen)
        {
            ReplicacionAD.InsertarConsultaEnReplica(pOrdenEjecucion, pNombreTablaReplica, pNombreTablaOrigen);
        }


        /// <summary>
        /// Comprueba si una consulta ya existe en una réplica particular
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador de la consulta a comprobar</param>
        /// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a comprobar la consulta</param>
        public bool ComprobarConsultaYaInsertadaEnReplica(int pOrdenEjecucion, string pNombreTablaReplica)
        {
            return ReplicacionAD.ComprobarConsultaYaInsertadaEnReplica(pOrdenEjecucion, pNombreTablaReplica);
        }

        /// <summary>
        /// Actualiza el estado de la cola de replicación MASTER
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador del elemento que se va a actualizar</param>
        /// <param name="pEstado">Nuevo estado del elemento de la cola</param>
        public void ActualizarEstadoCola(int pOrdenEjecucion, short pEstado)
        {
            ActualizarEstadoCola(pOrdenEjecucion, pEstado, "ColaReplicacionMaster");
        }

        /// <summary>
        /// Actualiza el estado de una cola de replicación
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador del elemento que se va a actualizar</param>
        /// <param name="pEstado">Nuevo estado del elemento de la cola</param>
        /// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a actualizar el estado</param>
        public void ActualizarEstadoCola(int pOrdenEjecucion, short pEstado, string pNombreTablaReplica)
        {
            ReplicacionAD.ActualizarEstadoCola(pOrdenEjecucion, pEstado, pNombreTablaReplica);
        }

        /// <summary>
        /// Actualiza el estado de una cola de replicación
        /// </summary>
        /// <param name="pOrdenEjecucion">Identificador del elemento que se va a actualizar</param>
        /// <param name="pEstado">Nuevo estado del elemento de la cola</param>
        /// <param name="pNombreTablaReplica">Nombre de la tabla en la que se va a actualizar el estado</param>
        public void ActualizarEstadoCola(List<int> pOrdenesEjecucion, short pEstado, string pNombreTablaReplica)
        {
            ReplicacionAD.ActualizarEstadoCola(pOrdenesEjecucion, pEstado, pNombreTablaReplica);
        }

        public int? ObtenerUltimaOrdenEjecucionDeCola(string pNombreTablaReplica)
        {
            return ReplicacionAD.ObtenerUltimaOrdenEjecucionDeCola(pNombreTablaReplica);
        }

        public void TransferirFilasACola(string pNombreTablaMaster, string pNombreTablaReplica, int pMaxOrdenEjecucion)
        {
            ReplicacionAD.TransferirFilasACola(pNombreTablaMaster, pNombreTablaReplica, pMaxOrdenEjecucion);
        }

        public void ActualizarEstadoCola(string pNombreTablaMaster, int pMaxOrdenEjecucion)
        {
            ReplicacionAD.ActualizarEstadoCola(pNombreTablaMaster, pMaxOrdenEjecucion);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// AD de baseComunidad.
        /// </summary>
        protected ReplicacionAD ReplicacionAD
        {
            get
            {
                return (ReplicacionAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~ReplicacionCN()
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
        /// <param name="disposing">Determina si se está llamando desde el Dispose()</param>
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;

                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (ReplicacionAD != null)
                    {
                        ReplicacionAD.Dispose();
                    }
                }
                ReplicacionAD = null;
            }
        }

        #endregion
    }
}
