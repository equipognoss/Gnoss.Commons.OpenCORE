using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.ComparticionAutomatica;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Logica.ComparticionAutomatica
{
    public class ComparticionAutomaticaCN : BaseCN, IDisposable
    {

        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public ComparticionAutomaticaCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.ComparticionAutomaticaAD = new ComparticionAutomaticaAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor para TablasDeConfiguracionCN
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ComparticionAutomaticaCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.ComparticionAutomaticaAD = new ComparticionAutomaticaAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region MetodosGenerales

        public void ActualizarBD()
        {
            ComparticionAutomaticaAD.ActualizarBD();
        }

        /// <summary>
        /// Obtiene el identificador de la compartición automática a partir del nombre de la compartición en proyecto
        /// </summary>
        /// <param name="pNombre">Nombre de la compartición en proyecto</param>
        /// <param name="pProyectoOrigenID">Identificador del proyecto de origen de la compartición</param>
        /// <returns>Guid con el identificador de la compartición. Guid empty si no lo encuentra</returns>
        public Guid? ObtenerComparticionIDPorNombre(Guid pProyectoOrigenID, string pNombre)
        {
            return ComparticionAutomaticaAD.ObtenerComparticionIDPorNombre(pProyectoOrigenID, pNombre);
        }

        /// <summary>
        /// Obtiene la configuración almacenada en ComparticionAutomatica, ComparticionAutomaticaMapping, ComparticionAutomaticaReglas
        /// a partir de un ComparticionID
        /// </summary>
        /// <param name="pComparticionID">Identificador de la compartición</param>
        /// <returns>DataSet con las tablas cargadas</returns>
        public DataWrapperComparticionAutomatica ObtenerComparticionAutomaticaPorComparticionID(Guid pComparticionID)
        {
            return ComparticionAutomaticaAD.ObtenerComparticionAutomaticaPorComparticionID(pComparticionID);
        }

        /// <summary>
        /// Obtiene la configuración almacenada en ComparticionAutomatica, ComparticionAutomaticaMapping, ComparticionAutomaticaReglas
        /// para la compartición automática de un proyecto determinado
        /// </summary>
        /// <param name="pOrganizacionID">OrganizacionID</param>
        /// <param name="pProyectoID">ProyectoID</param>
        /// <returns>DataSet con las tablas cargadas</returns>
        public DataWrapperComparticionAutomatica ObtenerComparticionProyectoPorProyectoID(Guid pOrganizacionID, Guid pProyectoID, bool pTraerEliminadas)
        {
            return ComparticionAutomaticaAD.ObtenerComparticionProyectoPorProyectoID(pOrganizacionID, pProyectoID, pTraerEliminadas);
        }

        #endregion MetodosGenerales

        #region Propiedades

        /// <summary>
        /// DataAdapter de TablasDeConfiguracion
        /// </summary>
        private ComparticionAutomaticaAD ComparticionAutomaticaAD
        {
            get
            {
                return (ComparticionAutomaticaAD)AD;
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
        ~ComparticionAutomaticaCN()
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
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he añadido a esta clase
                    if (ComparticionAutomaticaAD != null)
                    {
                        ComparticionAutomaticaAD.Dispose();
                    }
                }

                ComparticionAutomaticaAD = null;
            }
        }

        #endregion dispose

    }
}
