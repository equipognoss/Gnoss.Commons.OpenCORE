using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System.Data;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    public class BasePaginaCMSCN : BaseComunidadCN
    {
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BasePaginaCMSCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pFicheroConfiguracionBD, entityContext, loggingService, entityContextBASE, configService)
        {

            this.BasePaginaCMSAD = new BasePaginaCMSAD(pFicheroConfiguracionBD, loggingService, entityContext, entityContextBASE, configService);
        }

        #endregion

        #region Métodos

        #region Actualizar BD

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pBaseBlogComunidadDS">Dataset de modelo BASE de recursos de comunidad</param>
        public void ActualizarBD(BasePaginaCMSDS pBasePaginaCMS, bool pUsarRabbitSiEstaConfigurado = true)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.BasePaginaCMSAD.ActualizarBD(pBasePaginaCMS, pUsarRabbitSiEstaConfigurado);
                }
                else
                {
                    IniciarTransaccion(false);
                    {
                        this.BasePaginaCMSAD.ActualizarBD(pBasePaginaCMS, pUsarRabbitSiEstaConfigurado);

                        if (pBasePaginaCMS != null)
                        {
                            pBasePaginaCMS.AcceptChanges();
                        }
                        TerminarTransaccion(true);
                    }
                }
            }
            catch (DBConcurrencyException ex)
            {
                TerminarTransaccion(false);
                // Error de concurrencia
                mLoggingService.GuardarLogError(ex);
                throw new ErrorConcurrencia(ex.Row);
            }
            catch (DataException ex)
            {
                TerminarTransaccion(false);
                //Error interno de la aplicación
                mLoggingService.GuardarLogError(ex);
                throw new ErrorInterno();
            }
            catch
            {
                TerminarTransaccion(false);
                throw;
            }
        }

        #endregion

        #endregion

        #region Propiedades

        /// <summary>
        /// AD de baseComunidad.
        /// </summary>
        protected BasePaginaCMSAD BasePaginaCMSAD
        {
            get
            {
                return (BasePaginaCMSAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion
    }
}
