using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.BASE_BD;
using Es.Riam.Gnoss.AD.BASE_BD.Model;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using System;
using System.Collections.Generic;
using System.Data;


namespace Es.Riam.Gnoss.Logica.BASE_BD
{
    public class BaseVisitasCN : BaseCN
    {
        private LoggingService mLoggingService;

        #region Constructores

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Fichero de configuración de la base de datos BASE</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public BaseVisitasCN(string pFicheroConfiguracionBD, EntityContext entityContext, LoggingService loggingService, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(entityContext, loggingService, configService, servicesUtilVirtuosoAndReplication)
        {
            mLoggingService = loggingService;
            this.BaseVisitasAD = new BaseVisitasAD(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication);
        }

        #endregion

        #region Métodos

        #region Actualizar BD

        /// <summary>
        /// Actualiza BD
        /// </summary>
        /// <param name="pBaseVisitasDS">Dataset de modelo BASE de las visitas</param>
        public void ActualizarBD(BaseVisitasDS pBaseVisitasDS)
        {
            try
            {
                if (Transaccion != null)
                {
                    this.BaseVisitasAD.ActualizarBD(pBaseVisitasDS);
                }
                else
                {
                    IniciarTransaccion(false);
                    {
                        this.BaseVisitasAD.ActualizarBD(pBaseVisitasDS);

                        if (pBaseVisitasDS != null)
                        {
                            pBaseVisitasDS.AcceptChanges();
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

        #region Dispose

        /// <summary>
        /// Determina si está disposed
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Destructor
        /// </summary>
        ~BaseVisitasCN()
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
                //Libero todos los recursos administrados que he añadido a esta clase
                if (BaseVisitasAD != null)
                {
                    BaseVisitasAD.Dispose();
                }
                BaseVisitasAD = null;
            }
        }

        #endregion

        #region Propiedades

        private BaseVisitasAD BaseVisitasAD
        {
            get
            {
                return (BaseVisitasAD)AD;
            }
            set
            {
                this.AD = value;
            }
        }

        #endregion

        public List<Guid> ObtenerRecursosMasVisistadosProyecto(Guid pProyectoID, int dias, int pNumDocumentos)
        {
            DataSet dataSet = BaseVisitasAD.ObtenerRecursosMasVisistadosProyecto(pProyectoID, dias, pNumDocumentos);

            List<Guid> listaDocumentos = new List<Guid>(); 

            foreach (DataRow fila in dataSet.Tables["MasVisitados"].Select("", "Visitas desc"))
            {
                listaDocumentos.Add((Guid)fila["DocumentoID"]);
            }
            return listaDocumentos;
        }

        public void ActualizarNumeroVisitasDocumentoProyecto(Guid pDocumentoID, Guid pProyectoID, DateTime pFecha, int pNumVisitas)
        {
            BaseVisitasAD.ActualizarNumeroVisitasDiariasDocumentoProyecto(pDocumentoID, pProyectoID, pFecha, pNumVisitas);
            BaseVisitasAD.ActualizarNumeroVisitasMensualesDocumentoProyecto(pDocumentoID, pProyectoID, pFecha, pNumVisitas);
        }


        public void EliminarVisitasDiariasAntiguas(int pNumDiasTranscurridos)
        {
            BaseVisitasAD.EliminarVisitasDiariasAntiguas(pNumDiasTranscurridos);
        }

    }
}