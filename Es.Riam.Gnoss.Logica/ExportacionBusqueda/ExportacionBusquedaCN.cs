using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ParametrosProyecto;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Logica.ExportacionBusqueda
{
    public class ExportacionBusquedaCN : BaseCN, IDisposable
    {
        #region Miembros

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool mDisposed = false;

        #endregion

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ExportacionBusquedaCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.ExportacionBusquedaAD = new ExportacionBusquedaAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor para ExportacionBusquedaCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ExportacionBusquedaCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.ExportacionBusquedaAD = new ExportacionBusquedaAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Metodos Generales

        /// <summary>
        /// Actualiza exportaciones de búsqueda de proyecto
        /// </summary>
        /// <param name="ExportacionBusquedaDS">Dataset de exportaciones de búsqueda</param>
        public void ActualizarExportacionBusqueda()
        {
            ExportacionBusquedaAD.ActualizarExportacionBusqueda();
        }
       
        /// <summary>
        /// Obtiene las exportaciones de búsqueda de un proyecto 
        /// </summary>
        /// <param name="pProyectoID">Identificador del proyecto</param>
        /// <returns>Dataset con las exportaciones de búsqueda del proyecto</returns>
        public DataWrapperExportacionBusqueda ObtenerExportacionesProyecto(Guid pProyectoID)
        {
            return ExportacionBusquedaAD.ObtenerExportacionesProyecto(pProyectoID);
        }

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
        protected virtual void Dispose(bool disposing)
        {
            if (!mDisposed)
            {
                mDisposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (ExportacionBusquedaAD != null)
                        ExportacionBusquedaAD.Dispose();
                }
                ExportacionBusquedaAD = null;
            }
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// DataAdapter de ExportacionBusqueda
        /// </summary>
        private ExportacionBusquedaAD ExportacionBusquedaAD
        {
            get
            {
                return (ExportacionBusquedaAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }


        #endregion
    }
}
