using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;

namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    /// <summary>
    /// Lógica para modelo BASE de recursos de comunidad
    /// </summary>
    public class BaseRecursosComunidadCN : BaseComunidadCN, IDisposable
    {
        private LoggingService mLoggingService;

        #region Constructor

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pTablaBaseProyectoID">Identificador numerico del proyecto (-1 si no se va a actualizar tablas de proyecto)</param>
        public BaseRecursosComunidadCN(string pFicheroConfiguracionBD, int pTablaBaseProyectoID, EntityContext entityContext, LoggingService loggingService, EntityContextBASE entityContextBASE, ConfigService configService)
            : base(pFicheroConfiguracionBD, pTablaBaseProyectoID, entityContext, loggingService, entityContextBASE, configService)
        {
            mLoggingService = loggingService;
            this.BaseComunidadAD = new BaseRecursosComunidadAD(pFicheroConfiguracionBD, pTablaBaseProyectoID, loggingService, entityContext, entityContextBASE, configService);
        }

        #endregion

        #region Métodos

        #region Actualizar BD

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pBaseRecursosComunidadDS">Dataset de modelo BASE de recursos de comunidad</param>
        public void ActualizarBD(BaseRecursosComunidadDS pBaseRecursosComunidadDS, bool pUsarRabbitSiEstaConfigurado = true)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.BaseRecursosComunidadAD.ActualizarBD(pBaseRecursosComunidadDS, pUsarRabbitSiEstaConfigurado);
                }
                else
                {
                    IniciarTransaccion(false);
                    {
                        this.BaseRecursosComunidadAD.ActualizarBD(pBaseRecursosComunidadDS, pUsarRabbitSiEstaConfigurado);

                        if (pBaseRecursosComunidadDS != null)
                        {
                            pBaseRecursosComunidadDS.AcceptChanges();
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

        /// <summary>
        /// Comprueba si existe ya una entrada en la tabla ColaGrafoDbpedia una lista de uris dbpedia para un recurso
        /// </summary>
        /// <param name="pDocumentoID">Identificador del recurso</param>
        /// <param name="pUrisDbpedia">Lista de urls de entidades de dbpedia a comprobar</param>
        /// <param name="pAccion">Accion de la cola que se quiere comprobar</param>
        /// <returns></returns>
        public List<string> ExistenEntidadesParaRecursoEnColaGrafoDbpedia(Guid pDocumentoID, List<string> pUrisDbpedia, int pAccion)
        {
            return BaseRecursosComunidadAD.ExistenEntidadesParaRecursoEnColaGrafoDbpedia(pDocumentoID, pUrisDbpedia, pAccion);
        }

        public void InsertarColaTagsComunidadesSearch(BaseRecursosComunidadDS.ColaTagsComunidadesRow pFilaCola)
        {
            BaseRecursosComunidadAD.InsertarColaTagsComunidadesSearch(pFilaCola);
        }

        public void InsertarColaTagsComunidadesLinkedData(BaseRecursosComunidadDS pDataSet)
        {
            BaseRecursosComunidadAD.InsertarColaTagsComunidadesLinkedData(pDataSet);
        }


        /// <summary>
        /// Obtiene los elementos de la cola search
        /// </summary>
        /// <returns></returns>
        public BaseRecursosComunidadDS ObtenerElementosColaSearch()
        {
            return BaseRecursosComunidadAD.ObtenerElementosColaSearch(EstadosColaTags.EnEspera, EstadosColaTags.PrimerError);
        }

        #region Override

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public override void EliminarElementosColaProcesadosViejos()
        {
            BaseRecursosComunidadAD.EliminarElementosColaProcesadosViejos("ColaTagsComunidades");
            //BaseRecursosComunidadAD.EliminarElementosColaProcesadosViejos("ColaTagsComPriv");
        }

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public void EliminarElementosColaReplicaMasterProcesados()
        {
            BaseRecursosComunidadAD.EliminarElementosColaReplicaProcesados("ColaReplicacionMaster", true, DateTime.Now.AddHours(-1));
        }

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public void EliminarElementosColaReplicaHistoricoProcesados()
        {
            BaseRecursosComunidadAD.EliminarElementosColaReplicaProcesados("ColaReplicacionHistorico", false, DateTime.Now.AddHours(-4));
        }

        /// <summary>
        /// Elimina de la cola los elementos que han sido procesado exitósamente hace una semana
        /// </summary>
        public void EliminarElementosColaReplicaProcesados(string pTablaCola, int horasBorrado = 1)
        {
            BaseRecursosComunidadAD.EliminarElementosColaReplicaProcesados(pTablaCola, false, DateTime.Now.AddHours(-horasBorrado));
        }

        /// <summary>
        /// Obtiene los elementos pendientes de la cola de tags para procesar tags.
        /// </summary>
        /// <param name="pPrioridadBase">Prioridad base</param>
        /// <param name="pNumMaxItems">Numero máximo de items a traer</param>
        /// /// <returns>Devuelve un dataset con los elementos de la cola cargados</returns>
        public BaseRecursosComunidadDS ObtenerElementosColaPendientesProcesarTags(int pNumMaxItems)
        {
            return BaseRecursosComunidadAD.ObtenerElementosColaPendientesProcesarTags(pNumMaxItems);
        }

        #endregion

        #endregion

        #region Dispose

        /// <summary>
        /// Destructor
        /// </summary>
        ~BaseRecursosComunidadCN()
        {
            //Libero los recursos
            Dispose(false);
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Casting mediante una propiedad para poder acceder a los metodos específicos de la clase
        /// </summary>
        public BaseRecursosComunidadAD BaseRecursosComunidadAD
        {
            get
            {
                return (BaseRecursosComunidadAD)BaseComunidadAD;
            }
        }

        #endregion
    }
}
