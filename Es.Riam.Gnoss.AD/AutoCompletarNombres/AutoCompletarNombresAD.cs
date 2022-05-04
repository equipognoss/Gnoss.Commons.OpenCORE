using Es.Riam.AbstractsOpen;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.Util.Configuracion;
using Es.Riam.Gnoss.Util.General;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.AutocompletarNombres
{
    public class AutoCompletarNombresAD : BaseAD
    {

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public AutoCompletarNombresAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public AutoCompletarNombresAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, servicesUtilVirtuosoAndReplication)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas
        private string sqlSelectEtiquetasElemento;
        private string sqlSelectConfigAutocompletarProy;
        #endregion

        #region Métodos generales

        /// <summary>
        /// En caso de que se utilice el GnossConfig.xml por defecto se sigue utilizando el IBD estático
        /// </summary>
        private void CargarConsultasYDataAdapters()
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// En caso de que se utilice un GnossConfig.xml que no es el de por defecto se pasa un objeto IBaseDatos creado con respecto
        /// al fichero de configuracion que se ha apsado como parámetro
        /// </summary>
        /// <param name="IBD">Objecto IBaseDatos para el archivo pasado al constructor del AD</param>
        private void CargarConsultasYDataAdapters(IBaseDatos IBD)
        {
            #region Consultas
            this.sqlSelectEtiquetasElemento = "SELECT " + IBD.CargarGuid("EtiquetasElemento.ElementoID") + ", EtiquetasElemento.Tipo, " + IBD.CargarGuid("EtiquetasElemento.ProyectoID") + ", EtiquetasElemento.Etiquetas FROM EtiquetasElemento";
            this.sqlSelectConfigAutocompletarProy = "SELECT " + IBD.CargarGuid("ConfigAutocompletarProy.ProyectoID") + ", ConfigAutocompletarProy.Clave, ConfigAutocompletarProy.Valor FROM ConfigAutocompletarProy";
            #endregion
        }

        #endregion

        #region Métodos de consulta

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
            //Devolvemos la IdentidadID del perfil personal, no el de la comunidad...

            bool esOracle = (ConexionMaster is OracleConnection);

            string select = "";

            if (esOracle)
            {
                select = "SELECT \"NombrePerfil\", \"Identidad2\".\"IdentidadID\"";
            }
            else
            {
                select = $"SELECT TOP {pNumElementos} \"NombrePerfil\", \"Identidad2\".\"IdentidadID\"";
            }


            string sql = select + " FROM \"Perfil\" INNER JOIN \"Identidad\" ON \"Identidad\".\"PerfilID\" = \"Perfil\".\"PerfilID\" INNER JOIN \"Identidad\" \"Identidad2\" ON \"Identidad\".\"PerfilID\" = \"Identidad2\".\"PerfilID\" WHERE ";
            sql += " CONTAINS(\"NombrePerfil\", '\"" + pFiltro.Replace("'", "''") + "*\"')";
            sql += " AND \"Identidad\".\"ProyectoID\" = " + IBD.FormatearGuid(pProyectoID);
            sql += " AND \"Identidad2\".\"ProyectoID\" = '" + ProyectoAD.MetaProyecto + "'";

            if (esOracle)
            {
                select = $"AND ROWNUM <= {pNumElementos}";
            }

            DataSet dataSet = new DataSet();

            DbCommand dbCommand = ObtenerComando(sql);
            CargarDataSet(dbCommand, dataSet, "Identidades");

            Dictionary<Guid, string> identidades = new Dictionary<Guid, string>();

            foreach (DataRow fila in dataSet.Tables["Identidades"].Rows)
            {
                Guid identidadID = (Guid)fila["IdentidadID"];
                string nombre = fila["NombrePerfil"].ToString();

                if (!identidades.ContainsKey(identidadID))
                {
                    identidades.Add(identidadID, nombre);
                }
            }


            return identidades;
        }

        /// <summary>
        /// Obtien el nombre de la tabla de tags para la faceta indicada.
        /// </summary>
        /// <param name="pFaceta">Faceta</param>
        /// <returns>Nombre de la tabla de tags para la faceta indicada</returns>
        public string ObtenerTablaNombres(Guid pProyectoID)
        {
            string nombreTabla = "Nombres_" + pProyectoID;
            return "[" + nombreTabla + "]";
        }

        #endregion
    }
}
