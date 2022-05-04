using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Data;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    /// <summary>
    /// Lógica para el modelo BASE de personas y organizaciones de comunidad
    /// </summary>
    public class BasePerOrgComunidadCN : BaseComunidadCN, IDisposable
    {
        private LoggingService mLoggingService;

        #region Constructor

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        /// <param name="pTablaBaseOrganizacionID">Identificador numerico de la organizacion (-1 si no se va a actualizar tablas de organizacion)</param>
        public BasePerOrgComunidadCN(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, int pTablaBaseOrganizacionID, EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pFicheroConfiguracionBD, pTablaBaseProyectoID, entityContext, loggingService, entityContextBASE, configService)
        {
            mLoggingService = loggingService;
            this.BaseComunidadAD = new BasePerOrgComunidadAD(pFicheroConfiguracionBD, pTablaBaseProyectoID, pTablaBaseOrganizacionID, loggingService, entityContext, entityContextBASE, configService);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Casting mediante una propiedad para poder acceder a los metodos específicos de la clase
        /// </summary>
        public BasePerOrgComunidadAD BasePerOrgComunidadAD
        {
            get
            {
                return (BasePerOrgComunidadAD)BaseComunidadAD;
            }
        }

        #endregion

        #region Métodos generales

        #region Actualizar BD

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pBasePerOrgComunidadDS">Dataset de modelo BASE para personas y organizaciones de comunidad</param>
        public void ActualizarBD(BasePerOrgComunidadDS pBasePerOrgComunidadDS, bool pUsarRabbitSiEstaConfigurado = true)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.BasePerOrgComunidadAD.ActualizarBD(pBasePerOrgComunidadDS, pUsarRabbitSiEstaConfigurado);
                }
                else
                {
                    IniciarTransaccion(false);
                    {
                        this.BasePerOrgComunidadAD.ActualizarBD(pBasePerOrgComunidadDS, pUsarRabbitSiEstaConfigurado);

                        if (pBasePerOrgComunidadDS != null)
                        {
                            pBasePerOrgComunidadDS.AcceptChanges();
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

        #region Override

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public override void EliminarElementosColaProcesadosViejos()
        {
            BasePerOrgComunidadAD.EliminarElementosColaProcesadosViejos("ColaTagsCom_Per_Org");
            //BasePerOrgComunidadAD.EliminarElementosColaProcesadosViejos("ColaTagsCom_Per_Org_Vi");
            //BasePerOrgComunidadAD.EliminarElementosColaProcesadosViejos("ColaTagsCOM_X_ORG_X");
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags para procesar tags.
        /// </summary>
        /// <param name="pPrioridadBase">Prioridad base</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        /// /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        public BasePerOrgComunidadDS ObtenerElementosColaPendientesProcesarTags(int pNumMaxItems)
        {
            return BasePerOrgComunidadAD.ObtenerElementosColaPendientesProcesarTags(pNumMaxItems);
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~BasePerOrgComunidadCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion
    }
}
