using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System.Data;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    /// <summary>
    /// L�gica de BaseSuscripcionesCN
    /// </summary>
    public class BaseSuscripcionesCN : BaseComunidadCN
    {
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuraci�n de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se est�n usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BaseSuscripcionesCN(EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(entityContext, loggingService, entityContextBASE, configService)
        {
            mLoggingService = loggingService;
            this.BaseComunidadAD = new BaseSuscripcionesAD(loggingService, entityContext, entityContextBASE, configService);
        }

        #endregion

        #region M�todos

        #region Actualizar BD

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pBaseSuscripcionesDS">Dataset de modelo BASE de recursos de comunidad</param>
        public void ActualizarBD(BaseSuscripcionesDS pBaseSuscripcionesDS)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.BaseSuscripcionesAD.ActualizarBD(pBaseSuscripcionesDS);
                }
                else
                {
                    IniciarTransaccion(false);
                    {
                        this.BaseSuscripcionesAD.ActualizarBD(pBaseSuscripcionesDS);

                        if (pBaseSuscripcionesDS != null)
                        {
                            pBaseSuscripcionesDS.AcceptChanges();
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
                //Error interno de la aplicaci�n
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


        #region Override

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exit�samente hace una semana
        /// </summary>
        public override void EliminarElementosColaProcesadosViejos()
        {
            BaseSuscripcionesAD.EliminarElementosColaProcesadosViejos("ColaTagsSuscripciones");
        }

        #endregion


        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~BaseSuscripcionesCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Casting mediante una propiedad para poder acceder a los metodos espec�ficos de la clase
        /// </summary>
        public BaseSuscripcionesAD BaseSuscripcionesAD
        {
            get
            {
                return (BaseSuscripcionesAD)BaseComunidadAD;
            }
        }

        #endregion

    }
}