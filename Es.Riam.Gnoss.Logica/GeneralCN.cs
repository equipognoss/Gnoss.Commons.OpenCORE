using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;

namespace Es.Riam.Gnoss.Logica
{
    /// <summary>
    /// L�gica general
    /// </summary>
    public class GeneralCN : BaseCN, IDisposable
    {

        #region Constructores

        /// <summary>
        /// Constructor sin par�metros
        /// </summary>
        public GeneralCN(EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.GeneralAD = new GeneralAD(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        /// <summary>
        /// Constructor a partir del fichero de configuraci�n de base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuraci�n</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public GeneralCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            this.GeneralAD = new GeneralAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Devuelve la hora del servidor de base de datos
        /// </summary>
        public DateTime HoraServidor
        {
            get
            {
                return GeneralAD.HoraServidor;
            }
        }

        /// <summary>
        /// General AD
        /// </summary>
        private GeneralAD GeneralAD
        {
            get
            {
                return (GeneralAD)AD;
            }
            set
            {
                AD = value;
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Determina si est� disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~GeneralCN()
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
        /// <param name="disposing">Determina si se est� llamando desde el Dispose()</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    //Libero todos los recursos administrados que he a�adido a esta clase
                    if (GeneralAD != null)
                        GeneralAD.Dispose();
                }
                GeneralAD = null;
            }
        }

        #endregion

    }
}
