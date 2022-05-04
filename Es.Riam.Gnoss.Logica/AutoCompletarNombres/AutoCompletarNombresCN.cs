using System;
using System.Collections.Generic;
using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.AutocompletarNombres;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
//using Es.Riam.Gnoss.AD.AutocompletarNombres.Model;

namespace Es.Riam.Gnoss.Logica.AutoCompletarNombres
{
    public class AutoCompletarNombresCN : BaseCN, IDisposable
    {
        #region Constructores

        /// <summary>
        /// Constructor sin parámetros
        /// </summary>
        public AutoCompletarNombresCN(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.AutoCompletarNombresAD = new AutoCompletarNombresAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }


        /// <summary>
        /// Constructor a partir del fichero de configuración de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCaracteresExtra">Ultimos caracteres que se añaden a la tabla de Etiqueta</param>
        public AutoCompletarNombresCN(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.AutoCompletarNombresAD = new AutoCompletarNombresAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos

        /// <summary>
        /// Obtiene n elementos autocompletables según un filtro.
        /// </summary>
        /// <param name="pFiltro">Texto por el que se filtra</param>
        /// <param name="pProyectoID">ID del proyecto de búsqueda</param>
        /// <param name="pFaceta">Faceta filtro</param>
        /// <param name="pTipo">Origen del filtro</param>
        /// <param name="pIdentidadID">Identidad que realiza la búsqueda, Guid.Empty si en la comunidad es todo público</param>
        /// <param name="pNumElementos">Elementos a traer</param>
        /// <returns>Lista con los elementos encontrados más relevantes</returns>
        public Dictionary<Guid, string> ObtenerNombresAutocompletar(string pFiltro, Guid pIdentidadID, Guid pProyectoID, int pNumElementos)
        {
            return AutoCompletarNombresAD.ObtenerNombresAutocompletar(pFiltro, pIdentidadID, pProyectoID, pNumElementos);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// AD de autocompletar etiquetas.
        /// </summary>
        private AutoCompletarNombresAD AutoCompletarNombresAD
        {
            get
            {
                return (AutoCompletarNombresAD)AD;
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
        private bool mDisposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~AutoCompletarNombresCN()
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
                    if (AutoCompletarNombresAD != null)
                    {
                        AutoCompletarNombresAD.Dispose();
                    }
                }
                AutoCompletarNombresAD = null;
            }
        }

        #endregion
    }
}
