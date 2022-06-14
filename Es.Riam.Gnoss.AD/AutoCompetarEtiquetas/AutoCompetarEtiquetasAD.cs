using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Es.Riam.Gnoss.AD.AutoCompetarEtiquetas.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Es.Riam.Gnoss.AD.Usuarios;
using System.Text.RegularExpressions;
using Es.Riam.Util;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.EntityModel.Models.ProyectoDS;
using Es.Riam.Gnoss.AD.EncapsuladoDatos;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Gnoss.Util.Configuracion;
using Npgsql;
using Es.Riam.Gnoss.AD.EntityModelBASE;
using Es.Riam.AbstractsOpen;

namespace Es.Riam.Gnoss.AD.AutoCompetarEtiquetas
{
    /// <summary>
    /// AD para autocompletar etiquetas.
    /// </summary>
    public class AutoCompetarEtiquetasAD : BaseAD
    {
        #region Miembros

        private string mCaracteresExtra = "";

        private static Regex REG_EXP_PRIMER_CARACTER_LETRA = new Regex("[a-zA-Z]{1}", RegexOptions.Compiled);

        #endregion

        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public AutoCompetarEtiquetasAD(LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication)
        {
            CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public AutoCompetarEtiquetasAD(string pFicheroConfiguracionBD, LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication)
        {
            CargarConsultasYDataAdapters(IBD);
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuracion de conexion a la Base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        /// <param name="pCaracteresExtra">Ultimos caracteres que se añaden a la tabla de Etiqueta</param>
        public AutoCompetarEtiquetasAD(string pFicheroConfiguracionBD, string pCaracteresExtra, LoggingService loggingService, EntityContext entityContext, ConfigService configService, EntityContextBASE entityContextBASE, IServicesUtilVirtuosoAndReplication servicesUtilVirtuosoAndReplication)
            : base(pFicheroConfiguracionBD, loggingService, entityContext, configService, entityContextBASE, servicesUtilVirtuosoAndReplication)
        {
            mCaracteresExtra = pCaracteresExtra;
            CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas
        private string sqlSelectEtiquetasElemento;
        private string sqlSelectConfigAutocompletarProy;
        private string sqlSelectConfigSearchProy;
        private string sqlSelectGrupoIdentidades;
        #endregion

        #region DataAdapter

        #region ConfigAutocompletarProy

        private string sqlConfigAutocompletarProyInsert;
        private string sqlConfigAutocompletarProyDelete;
        private string sqlConfigAutocompletarProyModify;

        #endregion ConfigAutocompletarProy

        #region ConfigSearchProy

        private string sqlConfigSearchProyInsert;
        private string sqlConfigSearchProyDelete;
        private string sqlConfigSearchProyModify;

        #endregion

        #region GrupoIdentidades

        private string sqlGrupoIdentidadesInsert;
        private string sqlGrupoIdentidadesDelete;
        private string sqlGrupoIdentidadesModify;

        #endregion

        #endregion DataAdapter

        #region Métodos AD

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataSet"></param>
        public void ActualizarBD(DataSet pDataSet)
        {
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataSet"></param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);
                if (deletedDataSet != null)
                {

                    #region Eliminar tabla ConfigAutocompletarProy
                    DbCommand DeleteConfigAutocompletarProyCommand = ObtenerComando(sqlConfigAutocompletarProyDelete);
                    AgregarParametro(DeleteConfigAutocompletarProyCommand, IBD.ToParam("Original_OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteConfigAutocompletarProyCommand, IBD.ToParam("Original_ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteConfigAutocompletarProyCommand, IBD.ToParam("Original_Clave"), DbType.String, "Clave", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ConfigAutocompletarProy", null, null, DeleteConfigAutocompletarProyCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion Eliminar tabla ConfigAutocompletarProy

                    #region Eliminar tabla ConfigSearchProy
                    DbCommand DeleteConfigSearchProyCommand = ObtenerComando(sqlConfigSearchProyDelete);
                    AgregarParametro(DeleteConfigSearchProyCommand, IBD.ToParam("Original_OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(DeleteConfigSearchProyCommand, IBD.ToParam("Original_ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(DeleteConfigSearchProyCommand, IBD.ToParam("Original_Clave"), DbType.String, "Clave", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "ConfigSearchProy", null, null, DeleteConfigSearchProyCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla GrupoIdentidades
                    DbCommand DeleteGrupoIdentidadesCommand = ObtenerComando(sqlGrupoIdentidadesDelete);
                    AgregarParametro(DeleteGrupoIdentidadesCommand, IBD.ToParam("Original_GrupoID"), DbType.Guid, "GrupoID", DataRowVersion.Original);
                    AgregarParametro(DeleteGrupoIdentidadesCommand, IBD.ToParam("Original_IdentidadID"), DbType.Guid, "IdentidadID", DataRowVersion.Original);

                    ActualizarBaseDeDatos(deletedDataSet, "GrupoIdentidades", null, null, DeleteGrupoIdentidadesCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    deletedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pDataSet"></param>
        public void GuardarActualizaciones(DataSet pDataSet)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pDataSet.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla ConfigAutocompletarProy
                    DbCommand InsertConfigAutocompletarProyCommand = ObtenerComando(sqlConfigAutocompletarProyInsert);
                    AgregarParametro(InsertConfigAutocompletarProyCommand, IBD.ToParam("OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertConfigAutocompletarProyCommand, IBD.ToParam("ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertConfigAutocompletarProyCommand, IBD.ToParam("Clave"), DbType.String, "Clave", DataRowVersion.Current);
                    AgregarParametro(InsertConfigAutocompletarProyCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);
                    AgregarParametro(InsertConfigAutocompletarProyCommand, IBD.ToParam("PestanyaID"), DbType.Guid, "PestanyaID", DataRowVersion.Current);

                    DbCommand ModifyConfigAutocompletarProyCommand = ObtenerComando(sqlConfigAutocompletarProyModify);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("Clave"), DbType.String, "Clave", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("PestanyaID"), DbType.Guid, "PestanyaID", DataRowVersion.Current);

                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("Original_OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("Original_ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyConfigAutocompletarProyCommand, IBD.ToParam("Original_Clave"), DbType.String, "Clave", DataRowVersion.Original);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ConfigAutocompletarProy", InsertConfigAutocompletarProyCommand, ModifyConfigAutocompletarProyCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ConfigSearchProy
                    DbCommand InsertConfigSearchProyCommand = ObtenerComando(sqlConfigSearchProyInsert);
                    AgregarParametro(InsertConfigSearchProyCommand, IBD.ToParam("OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(InsertConfigSearchProyCommand, IBD.ToParam("ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertConfigSearchProyCommand, IBD.ToParam("Clave"), DbType.String, "Clave", DataRowVersion.Current);
                    AgregarParametro(InsertConfigSearchProyCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);

                    DbCommand ModifyConfigSearchProyCommand = ObtenerComando(sqlConfigSearchProyModify);
                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("Clave"), DbType.String, "Clave", DataRowVersion.Current);
                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("Valor"), DbType.String, "Valor", DataRowVersion.Current);

                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("Original_OrganizacionID"), DbType.Guid, "OrganizacionID", DataRowVersion.Original);
                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("Original_ProyectoID"), DbType.Guid, "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyConfigSearchProyCommand, IBD.ToParam("Original_Clave"), DbType.String, "Clave", DataRowVersion.Original);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ConfigSearchProy", InsertConfigSearchProyCommand, ModifyConfigSearchProyCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla GrupoIdentidades
                    DbCommand InsertGrupoIdentidadesCommand = ObtenerComando(sqlGrupoIdentidadesInsert);
                    AgregarParametro(InsertGrupoIdentidadesCommand, IBD.ToParam("GrupoID"), DbType.Guid, "GrupoID", DataRowVersion.Current);
                    AgregarParametro(InsertGrupoIdentidadesCommand, IBD.ToParam("IdentidadID"), DbType.Guid, "IdentidadID", DataRowVersion.Current);

                    DbCommand ModifyGrupoIdentidadesCommand = ObtenerComando(sqlGrupoIdentidadesModify);
                    AgregarParametro(ModifyGrupoIdentidadesCommand, IBD.ToParam("GrupoID"), DbType.Guid, "GrupoID", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoIdentidadesCommand, IBD.ToParam("IdentidadID"), DbType.Guid, "IdentidadID", DataRowVersion.Current);
                    AgregarParametro(ModifyGrupoIdentidadesCommand, IBD.ToParam("Original_GrupoID"), DbType.Guid, "GrupoID", DataRowVersion.Original);
                    AgregarParametro(ModifyGrupoIdentidadesCommand, IBD.ToParam("Original_IdentidadID"), DbType.Guid, "IdentidadID", DataRowVersion.Original);

                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "GrupoIdentidades", InsertGrupoIdentidadesCommand, ModifyGrupoIdentidadesCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #endregion

                    addedAndModifiedDataSet.Dispose();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion Métodos AD

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
            this.sqlSelectConfigAutocompletarProy = "SELECT " + IBD.CargarGuid("ConfigAutocompletarProy.OrganizacionID") + ", " + IBD.CargarGuid("ConfigAutocompletarProy.ProyectoID") + ", \"ConfigAutocompletarProy\".\"Clave\", \"ConfigAutocompletarProy\".\"Valor\", " + IBD.CargarGuid("ConfigAutocompletarProy.PestanyaID") + " FROM \"ConfigAutocompletarProy\"";
            this.sqlSelectConfigSearchProy = "SELECT " + IBD.CargarGuid("ConfigSearchProy.OrganizacionID") + ", " + IBD.CargarGuid("ConfigSearchProy.ProyectoID") + ", \"ConfigSearchProy\".\"Clave\", \"ConfigSearchProy\".\"Valor\" FROM \"ConfigSearchProy\"";
            sqlSelectGrupoIdentidades = $"SELECT {IBD.CargarGuid("GrupoIdentidades.GrupoID")}, {IBD.CargarGuid("GrupoIdentidades.IdentidadID")} FROM \"GrupoIdentidades\"";
            #endregion

            #region DataAdapter

            #region ConfigAutocompletarProy

            this.sqlConfigAutocompletarProyInsert = IBD.ReplaceParam("INSERT INTO ConfigAutocompletarProy (OrganizacionID, ProyectoID, Clave, Valor,PestanyaID) VALUES (@OrganizacionID, @ProyectoID, @Clave, @Valor, @PestanyaID)");
            this.sqlConfigAutocompletarProyDelete = IBD.ReplaceParam("DELETE FROM ConfigAutocompletarProy WHERE (OrganizacionID = @Original_OrganizacionID) AND (ProyectoID = @Original_ProyectoID) AND (Clave = @Original_Clave)");
            this.sqlConfigAutocompletarProyModify = IBD.ReplaceParam("UPDATE ConfigAutocompletarProy SET OrganizacionID = @OrganizacionID, ProyectoID = @ProyectoID, Clave = @Clave, Valor = @Valor, PestanyaID = @PestanyaID WHERE (OrganizacionID = @Original_OrganizacionID) AND (ProyectoID = @Original_ProyectoID) AND (Clave = @Original_Clave)");

            #endregion ConfigAutocompletarProy

            #region ConfigSearchProy

            this.sqlConfigSearchProyInsert = IBD.ReplaceParam("INSERT INTO ConfigSearchProy (OrganizacionID, ProyectoID, Clave, Valor) VALUES (@OrganizacionID, @ProyectoID, @Clave, @Valor)");
            this.sqlConfigSearchProyDelete = IBD.ReplaceParam("DELETE FROM ConfigSearchProy WHERE (OrganizacionID = @Original_OrganizacionID) AND (ProyectoID = @Original_ProyectoID) AND (Clave = @Original_Clave)");
            this.sqlConfigSearchProyModify = IBD.ReplaceParam("UPDATE ConfigSearchProy SET OrganizacionID = @OrganizacionID, ProyectoID = @ProyectoID, Clave = @Clave, Valor = @Valor WHERE (OrganizacionID = @Original_OrganizacionID) AND (ProyectoID = @Original_ProyectoID) AND (Clave = @Original_Clave)");

            #endregion

            #region GrupoIdentidades

            this.sqlGrupoIdentidadesInsert = IBD.ReplaceParam("INSERT INTO GrupoIdentidades (GrupoID, IdentidadID) VALUES (@GrupoID, @IdentidadID)");
            this.sqlGrupoIdentidadesDelete = IBD.ReplaceParam("DELETE FROM GrupoIdentidades WHERE (GrupoID = @Original_GrupoID) AND (IdentidadID = @Original_IdentidadID)");
            this.sqlGrupoIdentidadesModify = IBD.ReplaceParam("UPDATE GrupoIdentidades SET GrupoID = @GrupoID, IdentidadID = @IdentidadID WHERE (GrupoID = @Original_GrupoID) AND (IdentidadID = @Original_IdentidadID)");

            #endregion

            #endregion DataAdapter
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
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pNumElementos">Elementos a traer</param>
        /// <returns>Lista con los elementos encontrados más relevantes</returns>
        public Dictionary<string, int> ObtenerTagsAutocompletar(string pFiltro, Guid pProyectoID, string pFaceta, string pTipo, Guid pIdentidadID, string pIdioma, int pNumElementos, Guid? pGrupoID, List<string> pFacetasFiltro = null)
        {
            return ObtenerTagsAutocompletar(string.Empty, pFiltro, pProyectoID, pFaceta, pTipo, pIdentidadID, pIdioma, pNumElementos, true, pGrupoID, pFacetasFiltro);
        }

        /// <summary>
        /// Obtiene n elementos autocompletables según un filtro.
        /// </summary>
        /// <param name="pFiltro">Texto por el que se filtra</param>
        /// <param name="pProyectoID">ID del proyecto de búsqueda</param>
        /// <param name="pFaceta">Faceta filtro</param>
        /// <param name="pTipo">Origen del filtro</param>
        /// <param name="pIdentidadID">Identidad que realiza la búsqueda, Guid.Empty si en la comunidad es todo público</param>
        /// <param name="pIdioma">Idioma del usuario</param>
        /// <param name="pNumElementos">Elementos a traer</param>
        /// <param name="pNombreTabla">Nombre la tabla a la que se consulta</param>
        /// <returns>Lista con los elementos encontrados más relevantes</returns>
        public Dictionary<string, int> ObtenerTagsAutocompletar(string pNombreTabla, string pFiltro, Guid pProyectoID, string pFaceta, string pTipo, Guid pIdentidadID, string pIdioma, int pNumElementos, bool pUsarContainsObtenerEtiquetas, Guid? pGrupoID, List<string> pFacetasFiltro = null)
        {
            // string update = $"UPDATE \"{nombreBaseDatos}\".\"RdfDocumento_{mCaracteresDoc}\" SET \"RdfSem\" = '{filaDoc.RdfSem}', \"RdfDoc\" = '{filaDoc.RdfDoc}' WHERE (\"DocumentoID\" = {IBD.FormatearGuid(filaDoc.DocumentoID)} AND \"ProyectoID\" = {IBD.FormatearGuid(filaDoc.ProyectoID)})";

            string numFilasConsultar = "";
            bool esOracle = ConexionMaster is OracleConnection;
            if (!esOracle)
            {
                numFilasConsultar = $"TOP {pNumElementos}";
            }

            string select = $"SELECT DISTINCT {numFilasConsultar} \"Etiqueta\", \"Tipo\", \"Extra\", \"Cantidad\"";

            if (string.IsNullOrEmpty(pNombreTabla))
            {
                if (pFaceta.Equals("search"))
                {
                    pNombreTabla = ObtenerNombreTablaLetra(pFiltro);
                }
                else
                {
                    pNombreTabla = ObtenerTablaTag(pFaceta);
                }
            }

            if (esOracle)
            {
                numFilasConsultar = $"ROWNUM <= {pNumElementos} AND";
            }
            else
            {
                numFilasConsultar = "";
            }

            string sql = $"{select} FROM {pNombreTabla} @InnerJoin@ WHERE {numFilasConsultar}";

            DbCommand dbCommand = ObtenerComando(sql);

            if (!string.IsNullOrEmpty(pTipo))
            {
                sql += $" \"Tipo\" = {IBD.ToParam("tipo")} AND";
                AgregarParametro(dbCommand, IBD.ToParam("tipo"), DbType.String, pTipo);
            }

            if (!string.IsNullOrEmpty(pIdioma))
            {
                sql += $" \"Idioma\" = {IBD.ToParam("Idioma")} AND";
                AgregarParametro(dbCommand, IBD.ToParam("Idioma"), DbType.String, pIdioma);
            }

            sql += "@Identidad@";
            sql += " @Grupo@";

            if (pProyectoID != Guid.Empty)
            {
                sql += $" \"ProyectoID\" ={IBD.FormatearGuid(pProyectoID)} AND";
            }

            if (pUsarContainsObtenerEtiquetas)
            {
                AgregarParametro(dbCommand, IBD.ToParam("filtro"), DbType.String, $"\"{pFiltro}*\"");

                if (esOracle)
                {
                    sql += $" CONTAINS(\"Etiqueta\",{IBD.ToParam("filtro")}) > 0";
                }
                else
                {
                    sql += $" CONTAINS(\"Etiqueta\",{IBD.ToParam("filtro")}) ";
                }
            }
            else
            {
                AgregarParametro(dbCommand, IBD.ToParam("filtro"), DbType.String, pFiltro);
                sql += $" \"Etiqueta\" = {IBD.ToParam("filtro")}";
            }

            if (pFacetasFiltro != null && pFacetasFiltro.Count > 0)
            {
                sql += " AND \"Faceta\" ";
                if (pFacetasFiltro.Count == 1)
                {
                    sql += $" = {IBD.ToParam("faceta")}";
                    AgregarParametro(dbCommand, IBD.ToParam("faceta"), DbType.String, pFacetasFiltro[0]);
                }
                else
                {
                    sql += " IN (";
                    int contador = 1;
                    string coma = "";
                    foreach (string faceta in pFacetasFiltro)
                    {
                        string parametro = IBD.ToParam("faceta" + contador++);
                        sql += coma + parametro;
                        AgregarParametro(dbCommand, parametro, DbType.String, faceta);
                        coma = ", ";
                    }
                    sql += ")";
                }
            }

            sql += " ORDER BY \"Cantidad\" DESC";

            string sqlTagsPublicos = sql;
            bool sumarizar = false;
            if (pIdentidadID != Guid.Empty)
            {
                if (pIdentidadID != UsuarioAD.Invitado)
                {
                    sql = sql.Replace(", \"Cantidad\"", ", \"Cantidad\", \"IdentidadID\"");
                    sql = $"{sql} UNION {sql.Replace("@Identidad@", "@Identidad2@")}";
                }

                sql = sql.Replace("@Identidad@", " \"IdentidadID\" IS NULL AND");

                if (pIdentidadID != UsuarioAD.Invitado)
                {
                    if (pTipo.Equals("PerYOrg") || string.IsNullOrEmpty(pTipo))
                    {
                        sql = sql.Replace("@Identidad2@", $" \"IdentidadID\" IN ({IBD.FormatearGuid(pIdentidadID)}, {IBD.FormatearGuid(UsuarioAD.Invitado)}) AND");
                    }
                    else
                    {
                        sql = sql.Replace("@Identidad2@", $" \"IdentidadID\" = {IBD.FormatearGuid(pIdentidadID)} AND");
                    }

                    sumarizar = true;
                }
            }
            else
            {
                sql = sql.Replace("@Identidad@", "");
            }

            if (pGrupoID.HasValue)
            {
                if (pGrupoID.Value != Guid.Empty)
                {
                    sql = sql.Replace(", \"Cantidad\"", ", \"Cantidad\", \"GrupoID\"");
                    sql = $"{sql} UNION {sql.Replace("@Grupo@", "@Grupo2@")}";
                    sql = sql.Replace("@Grupo@", " \"GrupoID\" IS NULL AND");
                    sql = sql.Replace("@Grupo2@", $" \"GrupoID\" = {IBD.FormatearGuid(pGrupoID.Value)} AND");

                    sumarizar = true;
                }
                else
                {
                    sql = sql.Replace("@Grupo@", "");
                }
            }
            else
            {
                if (pIdentidadID != Guid.Empty && pIdentidadID != UsuarioAD.Invitado)
                {
                    //obtener todos los tags de los grupos de la identidad
                    sql = sql.Replace(", \"Cantidad\"", ", \"Cantidad\", \"GrupoID\"");
                    sql = $"{sql} UNION {sqlTagsPublicos.Replace(", \"Cantidad\"", ", \"Cantidad\", \"GrupoIdentidades\".\"GrupoID\", \"GrupoIdentidades\".\"IdentidadID\"").Replace("@Grupo@", "@Grupo2@").Replace("@InnerJoin@", "@InnerJoin2@")}";
                    sql = sql.Replace("@Identidad@", $" \"GrupoIdentidades\".\"IdentidadID\" = {IBD.FormatearGuid(pIdentidadID)} AND");
                    sql = sql.Replace("@Grupo@", " \"GrupoID\" IS NULL AND").Replace("@InnerJoin@", "");
                    sql = sql.Replace("@InnerJoin2@", $" INNER JOIN \"GrupoIdentidades\" ON {pNombreTabla}.\"GrupoID\" = \"GrupoIdentidades\".\"GrupoID\" ");
                    sql = sql.Replace("@Grupo2@", $" \"GrupoIdentidades\".\"IdentidadID\" = {IBD.FormatearGuid(pIdentidadID)} AND");
                }
                else
                {
                    sql = sql.Replace("@Grupo@", "");
                }
            }

            //si no se obtienen todos los grupos limpiar el join
            sql = sql.Replace("@InnerJoin@", "");

            if (sumarizar)
            {
                sql = $"{select.Replace("\"Cantidad\"", "sum(\"Cantidad\") Cantidad")} FROM ({sql}) temp GROUP BY \"Etiqueta\", \"Tipo\", \"Extra\"";
                sql += " ORDER BY \"Cantidad\" DESC";
            }

            DataSet dataSet = new DataSet();

            dbCommand.CommandText = sql;
            CargarDataSet(dbCommand, dataSet, "Etiqueta",null, false,false, mEntityContextBASE);

            Dictionary<string, int> etiquetas = new Dictionary<string, int>();

            foreach (DataRow fila in dataSet.Tables[0].Rows)
            {
                string etiqueta = (string)fila["Etiqueta"];

                if (!fila.IsNull("Extra"))
                {
                    etiqueta += "|||" + (string)fila["Extra"];
                }

                if (!etiquetas.ContainsKey(etiqueta))
                {
                    etiquetas.Add(etiqueta, (int)fila["Cantidad"]);
                }
            }

            return etiquetas;
        }

        /// <summary>
        /// Obtiene la configuración para el autocompletar etiquetas de una comunidad.
        /// </summary>
        /// <param name="pProyectoID">ID de proyecto</param>
        /// <returns>DataSet con la configuración para el autocompletar etiquetas de una comunidad</returns>
        public DataWrapperAutoCompletarEtiquetas ObtenerConfiguracionComunidad(Guid? pOrganizacionID, Guid pProyectoID)
        {
            DataWrapperAutoCompletarEtiquetas dataWrapperAutoCompletarEtiquetas = new DataWrapperAutoCompletarEtiquetas();

            //Carga ConfigAutocompletarProy
            var comandSel = mEntityContext.ConfigAutocompletarProy.Where(item => item.ProyectoID.Equals(pProyectoID));
            if (pOrganizacionID.HasValue)
            {
                comandSel = comandSel.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }
            dataWrapperAutoCompletarEtiquetas.ListaConfigAutocompletarProys = comandSel.ToList();

            //Carga ConfigSearchProy
            var comandSelSearch = mEntityContext.ConfigSearchProy.Where(item => item.ProyectoID.Equals(pProyectoID));
            if (pOrganizacionID.HasValue)
            {
                comandSelSearch = comandSelSearch.Where(item => item.OrganizacionID.Equals(pOrganizacionID.Value));
            }
            dataWrapperAutoCompletarEtiquetas.ListaConfigSearchProy = comandSelSearch.ToList();
            //Carga GrupoIdentidades
            //comandoSel = ObtenerComando(sqlSelectGrupoIdentidades);

            return dataWrapperAutoCompletarEtiquetas;
        }

        /// <summary>
        /// Obtiene la tabla GrupoIdentidades.
        /// </summary>
        /// <returns>DataSet con los identificadores de grupos y sus identidades</returns>
        public TagsAutoDS ObtenerGruposIdentidades()
        {
            TagsAutoDS tagAutoDS = new TagsAutoDS();
            DbCommand comandoSel = ObtenerComando(sqlSelectGrupoIdentidades);
            CargarDataSet(comandoSel, tagAutoDS, "GrupoIdentidades");
            return tagAutoDS;
        }

        /// <summary>
        /// Obtiene la tabla GrupoIdentidades de un grupo.
        /// </summary>
        /// <returns>DataSet con los identificadores de grupos y sus identidades</returns>
        public TagsAutoDS ObtenerGrupoIdentidadesGrupoID(Guid pGrupoID, List<Guid> pListaUsuariosComprobar = null)
        {
            TagsAutoDS tagAutoDS = new TagsAutoDS();
            DbCommand comandoSel = ObtenerComando($"{sqlSelectGrupoIdentidades} WHERE \"GrupoID\" = {IBD.FormatearGuid(pGrupoID)}");

            if (pListaUsuariosComprobar != null && pListaUsuariosComprobar.Count > 0)
            {
                comandoSel.CommandText += $" AND \"GrupoIdentidades\".\"IdentidadID\" IN (";
                string coma = "";
                foreach (Guid identidadID in pListaUsuariosComprobar)
                {
                    comandoSel.CommandText += coma + IBD.FormatearGuid(identidadID);
                    coma = ", ";
                }
                comandoSel.CommandText += ")";
            }

            CargarDataSet(comandoSel, tagAutoDS, "GrupoIdentidades");
            return tagAutoDS;
        }

        /// <summary>
        /// Obtiene las identidades de un grupo de la tabla GrupoIdentidades.
        /// </summary>
        /// <returns>DataSet con los identificadores de identidades de un grupo</returns>
        /// <param name="pGrupoID">IDentificador del grupo</param>
        public List<Guid> ObtenerIdentidadesGrupo(Guid pGrupoID)
        {
            List<Guid> listaIdentidades = new List<Guid>();
            TagsAutoDS tagAutoDS = new TagsAutoDS();
            DbCommand comandoSel = ObtenerComando($"{sqlSelectGrupoIdentidades} WHERE GrupoID = {IBD.GuidValor(pGrupoID)}");
            IDataReader reader = EjecutarReader(comandoSel);
            while (reader.Read())
            {
                if (!listaIdentidades.Contains(reader.GetGuid(0)))
                {
                    listaIdentidades.Add(reader.GetGuid(0));
                }
            }
            reader.Close();
            reader.Dispose();

            return listaIdentidades;
        }

        /// <summary>
        /// Obtiene las identidades de un grupo de la tabla GrupoIdentidades.
        /// </summary>
        /// <returns>DataSet con los identificadores de grupos y sus identidades</returns>
        public List<Guid> ObtenerGruposIdentidad(Guid pIdentidadID)
        {
            List<Guid> listaGrupos = new List<Guid>();
            TagsAutoDS tagAutoDS = new TagsAutoDS();
            DbCommand comandoSel = ObtenerComando($"{sqlSelectGrupoIdentidades} WHERE IdentidadID = {IBD.GuidValor(pIdentidadID)}");
            IDataReader reader = EjecutarReader(comandoSel);
            while (reader.Read())
            {
                if (!listaGrupos.Contains(reader.GetGuid(0)))
                {
                    listaGrupos.Add(reader.GetGuid(0));
                }
            }
            return listaGrupos;
        }

        #region Actualización

        /// <summary>
        /// Actualiza los tags de un elemento.
        /// </summary>
        /// <param name="pElementoID">ID del elemento</param>
        /// <param name="pTipo">Tipo de elemento</param>
        /// <param name="pProyectoID">ID del proyecto</param>
        /// <param name="pTagsAutoDS">DataSet con las variaciones de los tags</param>
        /// <param name="pTagsGuardarElementoDS">Tags para guardar los nuevos tag del elemento</param>
        public void ActualizarElemento(Guid pElementoID, string pTipo, Guid pProyectoID, TagsAutoDS pTagsAutoDS, TagsAutoDS pTagsGuardarElementoDS)
        {
            #region Actualizo los tags de la comunidad

            int filasAfectadas = 0;
            string nombreTabla = "";
            List<string> listaTablasActualizadas = new List<string>();

            foreach (TagsAutoDS.TagsVariarRow filaVariar in pTagsAutoDS.TagsVariar)
            {
                if (filaVariar.Cantidad != 0)
                {
                    nombreTabla = ObtenerTablaTag(filaVariar.Faceta);
                    filasAfectadas = 0;

                    //1º Intento actualizar por si ya existe el tag (seguro que si):
                    DbCommand comandoUpdateTag = ObtenerComando($"UPDATE {nombreTabla} SET Cantidad = Cantidad + {filaVariar.Cantidad}, Extra = {IBD.ToParam("Extra")} WHERE ProyectoID={IBD.GuidValor(filaVariar.ProyectoID)} AND Etiqueta={IBD.ToParam("Tag")} AND Tipo={IBD.ToParam("Tipo")} AND Faceta={IBD.ToParam("Faceta")} AND MetaBusqueda={IBD.ToParam("MetaBusqueda")}");

                    if (!filaVariar.IsIdentidadIDNull())
                    {
                        comandoUpdateTag.CommandText += $" AND IdentidadID={IBD.GuidValor(filaVariar.IdentidadID)}";
                    }
                    else
                    {
                        comandoUpdateTag.CommandText += " AND IdentidadID IS NULL";
                    }

                    if (!filaVariar.IsIdiomaNull())
                    {
                        comandoUpdateTag.CommandText += $" AND Idioma={IBD.ToParam("Idioma")}";
                        AgregarParametro(comandoUpdateTag, IBD.ToParam("Idioma"), DbType.String, filaVariar.Idioma);
                    }
                    else
                    {
                        comandoUpdateTag.CommandText += " AND Idioma IS NULL";
                    }

                    if (!string.IsNullOrEmpty(filaVariar.Extra))
                    {
                        AgregarParametro(comandoUpdateTag, IBD.ToParam("Extra"), DbType.String, filaVariar.Extra);
                    }
                    else
                    {
                        AgregarParametro(comandoUpdateTag, IBD.ToParam("Extra"), DbType.String, null);
                    }

                    AgregarParametro(comandoUpdateTag, IBD.ToParam("Tag"), DbType.String, filaVariar.Etiqueta);
                    AgregarParametro(comandoUpdateTag, IBD.ToParam("Tipo"), DbType.String, filaVariar.Tipo);
                    AgregarParametro(comandoUpdateTag, IBD.ToParam("Faceta"), DbType.String, filaVariar.Faceta);
                    AgregarParametro(comandoUpdateTag, IBD.ToParam("MetaBusqueda"), DbType.Boolean, filaVariar.MetaBusqueda);
                    filasAfectadas = ActualizarBaseDeDatos(comandoUpdateTag);

                    if (filasAfectadas == 0) //Hay que hacer un INSERT
                    {
                        string identidadID = "NULL";
                        string idioma = "NULL";

                        if (!filaVariar.IsIdentidadIDNull())
                        {
                            identidadID = IBD.GuidValor(filaVariar.IdentidadID);
                        }

                        if (!filaVariar.IsIdiomaNull())
                        {
                            idioma = IBD.ToParam("Idioma");
                        }

                        comandoUpdateTag = ObtenerComando($"INSERT INTO {nombreTabla} (ProyectoID, Etiqueta, Tipo, Faceta, Cantidad, IdentidadID, Extra, MetaBusqueda, Idioma) VALUES ({IBD.GuidValor(filaVariar.ProyectoID)}, {IBD.ToParam("Tag")}, {IBD.ToParam("Tipo")}, {IBD.ToParam("Faceta")}, {filaVariar.Cantidad}, {identidadID}, {IBD.ToParam("Extra")}, {IBD.ToParam("MetaBusqueda")}, {idioma})");

                        if (!string.IsNullOrEmpty(filaVariar.Extra))
                        {
                            AgregarParametro(comandoUpdateTag, IBD.ToParam("Extra"), DbType.String, filaVariar.Extra);
                        }
                        else
                        {
                            AgregarParametro(comandoUpdateTag, IBD.ToParam("Extra"), DbType.String, null);
                        }

                        if (!filaVariar.IsIdiomaNull())
                        {
                            AgregarParametro(comandoUpdateTag, IBD.ToParam("Idioma"), DbType.String, filaVariar.Idioma);
                        }

                        AgregarParametro(comandoUpdateTag, IBD.ToParam("Tag"), DbType.String, filaVariar.Etiqueta);
                        AgregarParametro(comandoUpdateTag, IBD.ToParam("Tipo"), DbType.String, filaVariar.Tipo);
                        AgregarParametro(comandoUpdateTag, IBD.ToParam("Faceta"), DbType.String, filaVariar.Faceta);
                        AgregarParametro(comandoUpdateTag, IBD.ToParam("MetaBusqueda"), DbType.Boolean, filaVariar.MetaBusqueda);

                        filasAfectadas = ActualizarBaseDeDatos(comandoUpdateTag);
                    }
                    else if (!listaTablasActualizadas.Contains(nombreTabla))
                    {
                        listaTablasActualizadas.Add(nombreTabla);
                    }
                }
            }

            #endregion

            #region Borro Tags con cantidad 0

            foreach (string nombreTablaBorrar in listaTablasActualizadas)
            {
                DbCommand comandoDeleteTag = ObtenerComando($"DELETE FROM {nombreTablaBorrar} WHERE Cantidad<=0");
                filasAfectadas = ActualizarBaseDeDatos(comandoDeleteTag);
            }

            #endregion

            #region Guardo los tags actuales del elemento

            if (pTagsGuardarElementoDS.TagsVariar.Count == 0) //Hay que borrar el elemento:
            {
                DbCommand comandoDeleteXML = ObtenerComando($"DELETE FROM EtiquetasElemento_{pElementoID.ToString().Substring(0, 3)} WHERE ElementoID={IBD.GuidValor(pElementoID)} AND Tipo={IBD.ToParam("Tipo")} AND ProyectoID={IBD.GuidValor(pProyectoID)}");
                AgregarParametro(comandoDeleteXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                ActualizarBaseDeDatos(comandoDeleteXML);
            }
            else
            {
                string xml = pTagsGuardarElementoDS.GetXml();
                DbCommand comandoUpdateXML = ObtenerComando($"UPDATE EtiquetasElemento_{pElementoID.ToString().Substring(0, 3)} SET Etiquetas={IBD.ToParam("Tags")} WHERE ElementoID={IBD.GuidValor(pElementoID)} AND Tipo={IBD.ToParam("Tipo")} AND ProyectoID={IBD.GuidValor(pProyectoID)}");
                AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, xml);
                AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                filasAfectadas = ActualizarBaseDeDatos(comandoUpdateXML);

                if (filasAfectadas == 0) //Hay que hacer un INSERT
                {
                    comandoUpdateXML = ObtenerComando($"INSERT INTO EtiquetasElemento_{pElementoID.ToString().Substring(0, 3)} (ElementoID, Tipo, ProyectoID, Etiquetas) VALUES ({IBD.GuidValor(pElementoID)}, {IBD.ToParam("Tipo")}, {IBD.GuidValor(pProyectoID)}, {IBD.ToParam("Tags")})");
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, xml);
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                    ActualizarBaseDeDatos(comandoUpdateXML);
                }
            }

            #endregion
        }

        public DataSet ObtenerIdentidadesFaceta(string pNombreTabla, Guid pProyectoID, string pEtiqueta, string pTipo, string pFaceta, string pExtra, bool pMetaBusqueda, string pIdioma)
        {
            DataSet ds = new DataSet();
            string query = $"SELECT IdentidadID FROM {pNombreTabla} WHERE ProyectoID = {IBD.GuidValor(pProyectoID)} AND Etiqueta = {IBD.ToParam("Etiqueta")} AND Tipo = {IBD.ToParam("Tipo")} AND Faceta = {IBD.ToParam("Faceta")} AND MetaBusqueda = {IBD.ToParam("MetaBusqueda")} AND Idioma = {IBD.ToParam("Idioma")}";

            DbCommand dbCommand = ObtenerComando(query);
            AgregarParametro(dbCommand, IBD.ToParam("Etiqueta"), DbType.String, pEtiqueta);
            AgregarParametro(dbCommand, IBD.ToParam("Tipo"), DbType.String, pTipo);
            AgregarParametro(dbCommand, IBD.ToParam("Faceta"), DbType.String, pFaceta);

            if (!string.IsNullOrEmpty(pExtra))
            {
                dbCommand.CommandText += $" AND Extra = {IBD.ToParam("Extra")}";
                AgregarParametro(dbCommand, IBD.ToParam("Extra"), DbType.String, pExtra);
            }
            else
            {
                dbCommand.CommandText += " AND Extra IS NULL ";
            }

            AgregarParametro(dbCommand, IBD.ToParam("MetaBusqueda"), DbType.Boolean, pMetaBusqueda);
            AgregarParametro(dbCommand, IBD.ToParam("Idioma"), DbType.String, pIdioma);

            CargarDataSet(dbCommand, ds, pNombreTabla);

            return ds;
        }

        /// <summary>
        /// Actualiza la cantidad de los tags para la tabla pasada como parámetro
        /// </summary>
        /// <param name="pDicTags">Diccionario con las etiquetas a actualizar y las identidaesId o gruposId de esa etiquera</param>
        /// <param name="pTagsDeGrupos">Indica si el diccionario contiene los tags de los grupos. False si el diccionario contiene identidades</param>
        /// <returns>Diccionario con los tags que no se han podido actualizar y es necesario insertar</returns>
        public List<EtiquetaInsercion> ActualizarElementosDiccionario(Dictionary<string, string> pDicTags, bool pTagsDeGrupos)
        {
            List<EtiquetaInsercion> tagsNoActualizados = new List<EtiquetaInsercion>();

            foreach (string tag in pDicTags.Keys)
            {
                string[] delimiter = { "[|]" };
                string[] filaVariar = tag.Split(delimiter, StringSplitOptions.None);
                string nombreTabla = filaVariar[0];
                string proyID = filaVariar[1];
                string etiqueta = filaVariar[2];
                string tipo = filaVariar[3];
                string faceta = filaVariar[4];
                string cantidad = filaVariar[5];
                string extra = filaVariar[6];
                string metaBusqueda = filaVariar[7];
                string idioma = filaVariar[8];

                string nombreTablaLetra = ObtenerNombreTablaLetra(etiqueta);

                //1º Intento actualizar por si ya existe el tag (seguro que si):
                StringBuilder sbUpdateTag = new StringBuilder();
                if (!string.IsNullOrEmpty(extra))
                {
                    sbUpdateTag.Append($"UPDATE ##NOMBRETABLA## SET Cantidad = Cantidad + {cantidad}, Extra = {IBD.ToParam("Extra")} WHERE ProyectoID = {IBD.GuidValor(new Guid(proyID))} AND Etiqueta = {IBD.ToParam("Tag")} AND Tipo = {IBD.ToParam("Tipo")} AND Faceta = {IBD.ToParam("Faceta")} AND MetaBusqueda = {IBD.ToParam("MetaBusqueda")}");
                }
                else
                {
                    sbUpdateTag.Append($"UPDATE ##NOMBRETABLA## SET Cantidad = Cantidad + {cantidad} WHERE ProyectoID = {IBD.GuidValor(new Guid(proyID))} AND Etiqueta = {IBD.ToParam("Tag")} AND Tipo = {IBD.ToParam("Tipo")} AND Faceta = {IBD.ToParam("Faceta")} AND MetaBusqueda = {IBD.ToParam("MetaBusqueda")}");
                }
                
                //DbCommand comandoUpdateTag = ObtenerComando("UPDATE " + nombreTabla + " SET Cantidad = Cantidad + " + cantidad + ", Extra = " + IBD.ToParam("Extra") + " WHERE ProyectoID=" + IBD.GuidValor(new Guid(proyID)) + " AND Etiqueta=" + IBD.ToParam("Tag") + " AND Tipo=" + IBD.ToParam("Tipo") + " AND Faceta=" + IBD.ToParam("Faceta") + " AND MetaBusqueda=" + IBD.ToParam("MetaBusqueda"));


                if (!string.IsNullOrEmpty(idioma))
                {
                    sbUpdateTag.Append($" AND Idioma = {IBD.ToParam("Idioma")}");
                    //comandoUpdateTag.CommandText += " AND Idioma=" + IBD.ToParam("Idioma");
                }
                else
                {
                    sbUpdateTag.Append($" AND Idioma IS NULL");
                    //comandoUpdateTag.CommandText += " AND Idioma IS NULL";
                }

                string strIdentidadID = "IdentidadID";
                if (pTagsDeGrupos)
                {
                    strIdentidadID = "GrupoID";
                }

                bool hayIdentificadores = false;
                string[] identificadores = null;
                string separador = "";
                if (!string.IsNullOrEmpty(pDicTags[tag]))
                {
                    identificadores = pDicTags[tag].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    if (identificadores.Length > 0)
                    {
                        hayIdentificadores = true;
                        //Recorrer las identidades/grupos e insertarlos en el UPDATE
                        sbUpdateTag.Append($" AND {strIdentidadID} IN (");
                        //comandoUpdateTag.CommandText += " AND " + strIdentidadID + " IN (";

                        foreach (string identificador in identificadores)
                        {
                            Guid identificadorID = Guid.Empty;
                            if (Guid.TryParse(identificador, out identificadorID))
                            {
                                sbUpdateTag.Append($"{separador}{IBD.GuidValor(identificadorID)}");
                                //comandoUpdateTag.CommandText += IBD.GuidValor(identificadorID) + ", ";
                            }
                            separador = ", ";
                        }

                        sbUpdateTag.Append($" )");
                        //comandoUpdateTag.CommandText += " )";

                    }
                }

                if (!hayIdentificadores)
                {
                    //Si no hay identidades/grupos, es el recurso público
                    sbUpdateTag.Append($" AND IdentidadID IS NULL AND GrupoID IS NULL");
                }

                DbCommand comandoUpdateTag = ObtenerComando(sbUpdateTag.ToString().Replace("##NOMBRETABLA##", nombreTabla));
                DbCommand comandoUpdateTagLetra = ObtenerComando(sbUpdateTag.ToString().Replace("##NOMBRETABLA##", nombreTablaLetra));

                if (!string.IsNullOrEmpty(idioma))
                {
                    AgregarParametro(comandoUpdateTag, IBD.ToParam("Idioma"), DbType.String, idioma);
                    AgregarParametro(comandoUpdateTagLetra, IBD.ToParam("Idioma"), DbType.String, idioma);
                }

                string valorExtra = null;
                if (!string.IsNullOrEmpty(extra))
                {
                    valorExtra = extra;
                }

                if (!string.IsNullOrEmpty(extra))
                {
                    AgregarParametro(comandoUpdateTag, IBD.ToParam("Extra"), DbType.String, valorExtra);
                    AgregarParametro(comandoUpdateTagLetra, IBD.ToParam("Extra"), DbType.String, valorExtra);
                }
                
                
                AgregarParametro(comandoUpdateTag, IBD.ToParam("Tag"), DbType.String, etiqueta);
                AgregarParametro(comandoUpdateTag, IBD.ToParam("Tipo"), DbType.String, tipo);
                AgregarParametro(comandoUpdateTag, IBD.ToParam("Faceta"), DbType.String, faceta);
                AgregarParametro(comandoUpdateTag, IBD.ToParam("MetaBusqueda"), DbType.Boolean, metaBusqueda);

                
                AgregarParametro(comandoUpdateTagLetra, IBD.ToParam("Tag"), DbType.String, etiqueta);
                AgregarParametro(comandoUpdateTagLetra, IBD.ToParam("Tipo"), DbType.String, tipo);
                AgregarParametro(comandoUpdateTagLetra, IBD.ToParam("Faceta"), DbType.String, faceta);
                AgregarParametro(comandoUpdateTagLetra, IBD.ToParam("MetaBusqueda"), DbType.Boolean, metaBusqueda);

                int tagsActualizadas = ActualizarBaseDeDatos(comandoUpdateTag, true, true, true, mEntityContextBASE);

                int tagsActualizadasLetra = ActualizarBaseDeDatos(comandoUpdateTagLetra, true, true, true, mEntityContextBASE);

                if (tagsActualizadas == 0 || (hayIdentificadores && tagsActualizadas < identificadores.Length))
                {
                    // No se han insertado todas las filas que deberían
                    tagsNoActualizados.Add(CalcularTagsNoActualizados(tag, identificadores, pTagsDeGrupos, tagsActualizadas, nombreTabla, sbUpdateTag.ToString()));
                }
                if (tagsActualizadasLetra == 0 || (hayIdentificadores && tagsActualizadasLetra < identificadores.Length))
                {
                    // No se han insertado todas las filas que deberían
                    tagsNoActualizados.Add(CalcularTagsNoActualizados(tag, identificadores, pTagsDeGrupos, tagsActualizadasLetra, nombreTablaLetra, sbUpdateTag.ToString()));
                }
            }

            return tagsNoActualizados;
        }

        private EtiquetaInsercion CalcularTagsNoActualizados(string pTag, string[] pIdentificadores, bool pTagsDeGrupos, int pTagsActualizados, string pNombreTabla, string consultaUpdate)
        {
            string[] delimiter = { "[|]" };
            string[] filaVariar = pTag.Split(delimiter, StringSplitOptions.None);
            string nombreTabla = filaVariar[0];
            string proyID = filaVariar[1];
            string etiqueta = filaVariar[2];
            string tipo = filaVariar[3];
            string faceta = filaVariar[4];
            string metaBusqueda = filaVariar[7];
            string idioma = filaVariar[8];

            EtiquetaInsercion etiquetaNoActualizada = new EtiquetaInsercion() { EtiquetaInfo = pTag, TablaInsertar = pNombreTabla };
            List<Guid> identificadoresSinActualizar = new List<Guid>();

            bool hayIdentificadores = pIdentificadores != null && pIdentificadores.Length > 0;

            string consulta = null;
            if (hayIdentificadores)
            {
                List<Guid> identificadores = new List<string>(pIdentificadores).ConvertAll(Guid.Parse);

                if (pTagsActualizados == 0)
                {
                    // No se ha actualizado ningun grupo, hay que insertar todos
                    identificadoresSinActualizar.AddRange(identificadores);
                }
                else
                {
                    // Compruebo cuáles se han actualizado y cuáles hay que insertar
                    int indiceWhere = consultaUpdate.IndexOf("WHERE");
                    consulta = $"SELECT ##NOMBRECOLUMNA## FROM {pNombreTabla} {consultaUpdate.Substring(indiceWhere)}";

                    if (pTagsDeGrupos)
                    {
                        consulta = consulta.Replace("##NOMBRECOLUMNA##", "GrupoID");
                    }
                    else
                    {
                        consulta = consulta.Replace("##NOMBRECOLUMNA##", "IdentidadID");
                    }

                    List<Guid> listaIdsActualizados = new List<Guid>();
                    DbCommand comando = ObtenerComando(consulta);

                    AgregarParametro(comando, IBD.ToParam("Tag"), DbType.String, etiqueta);
                    AgregarParametro(comando, IBD.ToParam("Tipo"), DbType.String, tipo);
                    AgregarParametro(comando, IBD.ToParam("Faceta"), DbType.String, faceta);
                    AgregarParametro(comando, IBD.ToParam("MetaBusqueda"), DbType.Boolean, metaBusqueda);

                    if (!string.IsNullOrEmpty(idioma))
                    {
                        AgregarParametro(comando, IBD.ToParam("Idioma"), DbType.String, idioma);
                    }

                    IDataReader reader = EjecutarReader(comando);
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            listaIdsActualizados.Add(reader.GetGuid(0));
                        }
                    }

                    identificadoresSinActualizar.AddRange(identificadores.Except(listaIdsActualizados));
                }
            }

            if (pTagsDeGrupos)
            {
                etiquetaNoActualizada.Grupos = identificadoresSinActualizar;
            }
            else
            {
                etiquetaNoActualizada.Identidades = identificadoresSinActualizar;
            }

            return etiquetaNoActualizada;
        }

        public void InsertarElementosDiccionario(List<EtiquetaInsercion> pListaEtiquetasInsertar)
        {
            foreach (EtiquetaInsercion tag in pListaEtiquetasInsertar)
            {
                string[] delimiter = { "[|]" };
                string[] filaVariar = tag.EtiquetaInfo.Split(delimiter, StringSplitOptions.None);
                string nombreTabla = filaVariar[0];
                string proyID = filaVariar[1];
                string etiqueta = filaVariar[2];
                string tipo = filaVariar[3];
                string faceta = filaVariar[4];
                int cantidad = int.Parse(filaVariar[5]);
                string extra = filaVariar[6];
                string metaBusqueda = filaVariar[7];
                string idioma = filaVariar[8];
                string strIdentidadID = "NULL";
                string strGrupoID = "NULL";

                StringBuilder sbInsertCommand = new StringBuilder();
                sbInsertCommand.Append($"INSERT INTO {tag.TablaInsertar} (ProyectoID, Etiqueta, Tipo, Faceta, Cantidad, IdentidadID, Extra, MetaBusqueda, Idioma, GrupoID) VALUES");

                bool tagsDeGrupos = tag.Grupos.Count > 0;

                List<Guid> identificadores = tag.Grupos;
                if (tagsDeGrupos)
                {
                    identificadores = tag.Identidades;
                }

                if (identificadores.Count > 0)
                {
                    string separador = "";
                    foreach (Guid identificador in identificadores)
                    {
                        strIdentidadID = IBD.GuidValor(identificador);

                        if (tagsDeGrupos)
                        {
                            strGrupoID = strIdentidadID;
                            strIdentidadID = "NULL";
                        }

                        sbInsertCommand.Append($"{separador} ({IBD.GuidValor(new Guid(proyID))}, {IBD.ToParam("Tag")}, {IBD.ToParam("Tipo")}, {IBD.ToParam("Faceta")}, {cantidad}, {strIdentidadID}, {IBD.ToParam("Extra")}, {IBD.ToParam("MetaBusqueda")}, {IBD.ToParam("Idioma")}, {strGrupoID}) ");
                        separador = ", ";
                    }
                }
                else
                {
                    sbInsertCommand.Append($" ({IBD.GuidValor(new Guid(proyID))}, {IBD.ToParam("Tag")}, {IBD.ToParam("Tipo")}, {IBD.ToParam("Faceta")}, {cantidad}, {strIdentidadID}, {IBD.ToParam("Extra")}, {IBD.ToParam("MetaBusqueda")}, {IBD.ToParam("Idioma")}, {strGrupoID}) ");
                }

                DbCommand comandoInsertTag = ObtenerComando(sbInsertCommand.ToString());

                string valorExtra = null;
                if (!string.IsNullOrEmpty(extra))
                {
                    valorExtra = extra;
                }

                string valorIdioma = null;
                if (!string.IsNullOrEmpty(idioma))
                {
                    valorIdioma = idioma;
                }

                AgregarParametro(comandoInsertTag, IBD.ToParam("Extra"), DbType.String, valorExtra);
                AgregarParametro(comandoInsertTag, IBD.ToParam("Idioma"), DbType.String, valorIdioma);
                AgregarParametro(comandoInsertTag, IBD.ToParam("Tag"), DbType.String, etiqueta);
                AgregarParametro(comandoInsertTag, IBD.ToParam("Tipo"), DbType.String, tipo);
                AgregarParametro(comandoInsertTag, IBD.ToParam("Faceta"), DbType.String, faceta);
                AgregarParametro(comandoInsertTag, IBD.ToParam("MetaBusqueda"), DbType.Boolean, metaBusqueda);

                ActualizarBaseDeDatos(comandoInsertTag, true, false, false, mEntityContextBASE);
            }
        }

        public void ModificarElementos(Guid pElementoID, string pTipo, Guid pProyectoID, string pJson, List<string> pListaTablasActualizadas)
        {
            #region Guardo los tags actuales del elemento
            
            string nombreTabla = $"EtiquetasElemento_{ pElementoID.ToString().Substring(0, 3)}";

            if (mEntityContextBASE.Database.GetDbConnection() is OracleConnection)
            {
                if (string.IsNullOrEmpty(pJson)) //Hay que borrar el elemento:
                {
                    DbCommand comandoDeleteXML = ObtenerComando($"DELETE FROM \"{nombreTabla}\" WHERE \"ElementoID\"={IBD.GuidValor(pElementoID)} AND \"Tipo\"={IBD.ToParam("Tipo")} AND \"ProyectoID\"={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                    AgregarParametro(comandoDeleteXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                    ActualizarBaseDeDatos(comandoDeleteXML, false, true, true, mEntityContextBASE);
                }
                else
                {
                    DbCommand comandoUpdateXML = ObtenerComando($"UPDATE \"{nombreTabla}\" SET Etiquetas={IBD.ToParam("Tags")} WHERE \"ElementoID\"={IBD.GuidValor(pElementoID)} AND \"Tipo\"={IBD.ToParam("Tipo")} AND \"ProyectoID\"={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, pJson);
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);

                    if (ActualizarBaseDeDatos(comandoUpdateXML, false, true, true, mEntityContextBASE) == 0) //Hay que hacer un INSERT
                    {
                        comandoUpdateXML = ObtenerComando($"INSERT INTO \"{nombreTabla}\" (\"ElementoID\", \"Tipo\", \"ProyectoID\", Etiquetas) VALUES ({IBD.GuidValor(pElementoID)}, {IBD.ToParam("Tipo")}, {IBD.GuidValor(pProyectoID)}, {IBD.ToParam("Tags")})");
                        AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, pJson);
                        AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                        ActualizarBaseDeDatos(comandoUpdateXML, false, true, true, mEntityContextBASE);
                    }
                }
            }
            else if (mEntityContextBASE.Database.GetDbConnection() is NpgsqlConnection)
            {
                if (string.IsNullOrEmpty(pJson)) //Hay que borrar el elemento:
                {
                    DbCommand comandoDeleteXML = ObtenerComando($"DELETE FROM \"{nombreTabla}\" WHERE \"ElementoID\"={IBD.GuidValor(pElementoID)} AND \"Tipo\"={IBD.ToParam("Tipo")} AND \"ProyectoID\"={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                    AgregarParametro(comandoDeleteXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                    ActualizarBaseDeDatos(comandoDeleteXML, false, true, true, mEntityContextBASE);
                }
                else
                {
                    DbCommand comandoUpdateXML = ObtenerComando($"UPDATE \"{nombreTabla}\" SET Etiquetas={IBD.ToParam("Tags")} WHERE \"ElementoID\"={IBD.GuidValor(pElementoID)} AND \"Tipo\"={IBD.ToParam("Tipo")} AND \"ProyectoID\"={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, pJson);
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);

                    if (ActualizarBaseDeDatos(comandoUpdateXML, false, true, true, mEntityContextBASE) == 0) //Hay que hacer un INSERT
                    {
                        comandoUpdateXML = ObtenerComando($"INSERT INTO \"{nombreTabla}\" (\"ElementoID\", \"Tipo\", \"ProyectoID\", Etiquetas) VALUES ({IBD.GuidValor(pElementoID)}, {IBD.ToParam("Tipo")}, {IBD.GuidValor(pProyectoID)}, {IBD.ToParam("Tags")})", mEntityContextBASE);
                        AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, pJson);
                        AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                        ActualizarBaseDeDatos(comandoUpdateXML, false, true, true, mEntityContextBASE);
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(pJson)) //Hay que borrar el elemento:
                {
                    DbCommand comandoDeleteXML = ObtenerComando($"DELETE FROM {nombreTabla} WHERE ElementoID={IBD.GuidValor(pElementoID)} AND Tipo={IBD.ToParam("Tipo")} AND ProyectoID={IBD.GuidValor(pProyectoID)}");
                    AgregarParametro(comandoDeleteXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                    ActualizarBaseDeDatos(comandoDeleteXML, false, true, true, mEntityContextBASE);
                }
                else
                {
                    DbCommand comandoUpdateXML = ObtenerComando($"UPDATE {nombreTabla} SET Etiquetas={IBD.ToParam("Tags")} WHERE ElementoID={IBD.GuidValor(pElementoID)} AND Tipo={IBD.ToParam("Tipo")} AND ProyectoID={IBD.GuidValor(pProyectoID)}");
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, pJson);
                    AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);

                    if (ActualizarBaseDeDatos(comandoUpdateXML, false, true, true, mEntityContextBASE) == 0) //Hay que hacer un INSERT
                    {
                        comandoUpdateXML = ObtenerComando($"INSERT INTO {nombreTabla} (ElementoID, Tipo, ProyectoID, Etiquetas) VALUES ({IBD.GuidValor(pElementoID)}, {IBD.ToParam("Tipo")}, {IBD.GuidValor(pProyectoID)}, {IBD.ToParam("Tags")})");
                        AgregarParametro(comandoUpdateXML, IBD.ToParam("Tags"), DbType.String, pJson);
                        AgregarParametro(comandoUpdateXML, IBD.ToParam("Tipo"), DbType.String, pTipo);
                        ActualizarBaseDeDatos(comandoUpdateXML, false, true, true, mEntityContextBASE);
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Obtiene las etiquetas de un elemento en un proyecto.
        /// </summary>
        /// <param name="pElementoID">ID del elemento</param>
        /// <param name="pTipo">Tipo de elemento</param>
        /// <param name="pProyectoID">ID del proyecto del elemento</param>
        public string ObtenerEtiquetasElemento(Guid pElementoID, string pTipo, Guid pProyectoID)
        {            
            string tablaElemento = $"EtiquetasElemento_{pElementoID.ToString().Substring(0, 3)}";
            //TODO Javi Migrar tabla
            VerificarExisteTabla(tablaElemento, true);

            TagsAutoDS tagsDS = new TagsAutoDS();

            if (ConexionMaster is OracleConnection)
            {
                string consulta = "SELECT * FROM 'EtiquetasElemento'";
                DbCommand dbCommand = ObtenerComando($"{consulta.Replace("EtiquetasElemento", tablaElemento)} WHERE 'ElementoID'={IBD.GuidValor(pElementoID)} AND 'Tipo'={IBD.ToParam("Tipo")} AND 'ProyectoID'={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                AgregarParametro(dbCommand, IBD.ToParam("Tipo"), DbType.String, pTipo);
                CargarDataSet(dbCommand, tagsDS, "EtiquetasElemento", null, true, true, mEntityContextBASE);
            }
            else if (ConexionMaster is NpgsqlConnection)
            {
                string consulta = "SELECT * FROM \"EtiquetasElemento\"";
                DbCommand dbCommand = ObtenerComando($"{consulta.Replace("EtiquetasElemento", tablaElemento)} WHERE \"ElementoID\"={IBD.GuidValor(pElementoID)} AND \"Tipo\"={IBD.ToParam("Tipo")} AND \"ProyectoID\"={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                AgregarParametro(dbCommand, IBD.ToParam("Tipo"), DbType.String, pTipo);
                CargarDataSet(dbCommand, tagsDS, "EtiquetasElemento", null, true, true, mEntityContextBASE);
            }
            else 
            {
                DbCommand dbCommand = ObtenerComando($"{sqlSelectEtiquetasElemento.Replace("EtiquetasElemento", tablaElemento)} WHERE ElementoID={IBD.GuidValor(pElementoID)} AND Tipo={IBD.ToParam("Tipo")} AND ProyectoID={IBD.GuidValor(pProyectoID)}", mEntityContextBASE);
                AgregarParametro(dbCommand, IBD.ToParam("Tipo"), DbType.String, pTipo);
                CargarDataSet(dbCommand, tagsDS, "EtiquetasElemento", null, false, false, mEntityContextBASE);
            }

            if (tagsDS.EtiquetasElemento.Count > 0)
            {
                return tagsDS.EtiquetasElemento[0].Etiquetas;
            }

            return null;
        }

        /// <summary>
        /// Obtien el nombre de la tabla de tags para la faceta indicada.
        /// </summary>
        /// <param name="pFaceta">Faceta</param>
        /// <returns>Nombre de la tabla de tags para la faceta indicada</returns>
        public string ObtenerTablaTag(string pFaceta)
        {
            string nombreTabla = $"Tag_{mCaracteresExtra}_{pFaceta.Replace(":", "_").Replace("@@@", "_")}";

            if (nombreTabla.Length > 120)
            {
                nombreTabla = nombreTabla.Substring(0, 70) + nombreTabla.Substring(nombreTabla.Length - 50);
            }

            return $"[{nombreTabla}]";
        }

        public string ObtenerNombreTablaLetra(string pEtiqueta)
        {
            string nombreTabla = $"Tag_{mCaracteresExtra}_search_";

            string letra = UtilCadenas.RemoveAccentsWithRegEx(pEtiqueta.Substring(0, 1));

            if (REG_EXP_PRIMER_CARACTER_LETRA.IsMatch(letra))
            {
                nombreTabla += letra.ToUpper();
            }
            else
            {
                // Las etiquetas que no comiencen por letra, estarán en el grupo de la Z
                nombreTabla += "Z";
            }

            return $"[{nombreTabla}]";
        }

        #endregion

        #region Verificación de existencia y creación de tablas

        /// <summary>
        /// Comprueba si existe la tabla de RdfDocumento. Si no existe y se indica, la crea.
        /// </summary>
        /// <param name="pNombreTabla">Nombre de la tabla</param>
        /// <param name="pCrearTablaSiNoExiste">TRUE si se debe crear la tabla en caso de que no exista</param>
        /// <returns>TRUE si la tabla existe.</returns>
        public bool VerificarExisteTabla(string pNombreTabla, bool pCrearTablaSiNoExiste)
        {
            if (pNombreTabla.IndexOf("[") == 0)
            {
                pNombreTabla = pNombreTabla.Substring(1, pNombreTabla.Length - 2);
            }

            bool existeTabla = VerificarExisteTabla(pNombreTabla);

            if (!existeTabla && pCrearTablaSiNoExiste)
            {
                if (ConexionMaster is OracleConnection)
                {
                    CrearTablaOracle(pNombreTabla);
                }
                else if (ConexionMaster is NpgsqlConnection)
                {
                    CrearTablaPostgre(pNombreTabla);
                }
                else
                {
                    CrearTabla(pNombreTabla);
                }
            }
            return existeTabla;
        }

        /// <summary>
        /// Comprueba si existen las tablas sobre las que está configurado este AD. Si no existen las crea. 
        /// </summary>
        /// <returns>Verdad si la tabla existe (o ha sido recién creada).</returns>
        public bool VerificarExisteTabla(string pNombreTabla)
        {
            string existeTabla = "";

            if (ConexionMaster is OracleConnection)
            {
                existeTabla = $"SELECT 1 FROM ALL_TABLES WHERE TABLE_NAME = {IBD.ToParam("nombreTabla")}";
            }
            else if(ConexionMaster is NpgsqlConnection)
            {
                existeTabla = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = 'public' AND TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = {IBD.ToParam("nombreTabla")}";
            }
            else
            {
                existeTabla = $"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME = {IBD.ToParam("nombreTabla")}";
            }


            DbCommand cmdExisteTabla = ObtenerComando(existeTabla, mEntityContextBASE);
            AgregarParametro(cmdExisteTabla, IBD.ToParam("nombreTabla"), DbType.String, pNombreTabla);

            object resultado = EjecutarEscalar(cmdExisteTabla, false, true, mEntityContextBASE);

            return (resultado != null) && (resultado is int) && (resultado.Equals(1));
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de consulta
        /// </summary>
        private void CrearTabla(string pNombreTabla)
        {
            if (pNombreTabla.StartsWith("EtiquetasElemento_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE [{pNombreTabla}] ([ElementoID] [uniqueidentifier] NOT NULL,[Tipo] [nvarchar](100) NOT NULL,[ProyectoID] [uniqueidentifier] NOT NULL,[Etiquetas] [nvarchar](max) NULL, CONSTRAINT [PK_{pNombreTabla}] PRIMARY KEY CLUSTERED ([ElementoID] ASC,[Tipo] ASC,[ProyectoID] ASC)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY]", mEntityContextBASE);

                ActualizarBaseDeDatos(cmdCrearTabla, true, false, false, mEntityContextBASE);
            }
            else if (pNombreTabla.StartsWith("Tag_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE [{pNombreTabla}]([ID] [int] IDENTITY(1,1) NOT NULL,[ProyectoID] [uniqueidentifier] NOT NULL,[Etiqueta] [nvarchar](1000) NOT NULL,[Tipo] [nvarchar](1000) NOT NULL,[Faceta] [nvarchar](1000) NOT NULL,[Cantidad] [int] NOT NULL,[IdentidadID] [uniqueidentifier] NULL,[Extra] [nvarchar](max) NULL,[MetaBusqueda] [bit] NOT NULL,[Idioma] [nvarchar](4) NULL,[GrupoID] [uniqueidentifier] NULL,CONSTRAINT [PK_{pNombreTabla}] PRIMARY KEY CLUSTERED ([ID] ASC)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]) ON [PRIMARY]", mEntityContextBASE);

                ActualizarBaseDeDatos(cmdCrearTabla, true, false, false, mEntityContextBASE);

                cmdCrearTabla = ObtenerComando($"CREATE FULLTEXT INDEX ON [{pNombreTabla}](Etiqueta LANGUAGE 0) KEY INDEX [PK_{pNombreTabla}] ON Etiqueta_catalog", mEntityContextBASE);
                ActualizarBaseDeDatos(cmdCrearTabla, false, false, false, mEntityContextBASE);
            }
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de consulta
        /// </summary>
        private void CrearTablaOracle(string pNombreTabla)
        {
            if (pNombreTabla.StartsWith("EtiquetasElemento_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\" (\"ElementoID\" RAW(16) NOT NULL, \"Tipo\" NVARCHAR2(100) NOT NULL, \"ProyectoID\" RAW(16) NOT NULL, \"Etiquetas\" NCLOB NULL, PRIMARY KEY(\"ElementoID\", \"Tipo\", \"ProyectoID\"))", mEntityContextBASE);

                ActualizarBaseDeDatos(cmdCrearTabla,true, true, true, mEntityContextBASE);
            }
            else if (pNombreTabla.StartsWith("Tag_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\"(\"ID\" NUMBER(10) GENERATED ALWAYS AS IDENTITY PRIMARY KEY, \"ProyectoID\" RAW(16) NOT NULL, \"Etiqueta\" VARCHAR2(1000) NOT NULL, \"Tipo\" NVARCHAR2(1000) NOT NULL, \"Faceta\" NVARCHAR2(1000) NOT NULL, \"Cantidad\" NUMBER(10) NOT NULL, \"IdentidadID\" RAW(16) NULL, \"Extra\" NCLOB NULL, \"MetaBusqueda\" NUMBER(3) NOT NULL, \"Idioma\" NVARCHAR2(4) NULL, \"GrupoID\" RAW(16) NULL)", mEntityContextBASE);

                ActualizarBaseDeDatos(cmdCrearTabla,true, true, true, mEntityContextBASE);

                cmdCrearTabla = ObtenerComando($"CREATE INDEX \"PK_{pNombreTabla}\" ON \"{pNombreTabla}\" (\"Etiqueta\") INDEXTYPE IS CTXSYS.CONTEXT", mEntityContextBASE);
                ActualizarBaseDeDatos(cmdCrearTabla,true, true, true, mEntityContextBASE);
            }
        }

        /// <summary>
        /// Crea una tabla en función de un tipo de consulta
        /// </summary>
        private void CrearTablaPostgre(string pNombreTabla)
        {
            if (pNombreTabla.StartsWith("EtiquetasElemento_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\" (\"ElementoID\" UUID NOT NULL, \"Tipo\" VARCHAR(100) NOT NULL, \"ProyectoID\" UUID NOT NULL, \"Etiquetas\" text NULL, PRIMARY KEY(\"ElementoID\", \"Tipo\", \"ProyectoID\"))", mEntityContextBASE);

                ActualizarBaseDeDatos(cmdCrearTabla,true , true, true, mEntityContextBASE);
            }
            else if (pNombreTabla.StartsWith("Tag_"))
            {
                DbCommand cmdCrearTabla = ObtenerComando($"CREATE TABLE \"{pNombreTabla}\"(\"ID\" integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY, \"ProyectoID\" UUID NOT NULL, \"Etiqueta\" VARCHAR(1000) NOT NULL, \"Tipo\" VARCHAR(1000) NOT NULL, \"Faceta\" VARCHAR(1000) NOT NULL, \"Cantidad\" integer NOT NULL, \"IdentidadID\" UUID NULL, \"Extra\" text NULL, \"MetaBusqueda\" smallint NOT NULL, \"Idioma\" VARCHAR(4) NULL, \"GrupoID\" UUID NULL)", mEntityContextBASE);

                ActualizarBaseDeDatos(cmdCrearTabla,true, true, true, mEntityContextBASE);

                cmdCrearTabla = ObtenerComando($"CREATE FULLTEXT INDEX ON [\"{pNombreTabla}\"](Etiqueta LANGUAGE 0) KEY INDEX [PK_{pNombreTabla}] ON \"Etiqueta_catalog\"", mEntityContextBASE);
                ActualizarBaseDeDatos(cmdCrearTabla, false, true, true, mEntityContextBASE);
            }
        }

        public List<string> ObtenerNombresTablasTagsDeFacetas()
        {
            List<string> listaTablas = new List<string>();

            string consulta = "";

            if (ConexionMaster is OracleConnection)
            {
                consulta = "SELECT TABLE_NAME FROM ALL_TABLES WHERE TABLE_NAME LIKE 'Tag_%' ORDER BY TABLE_NAME";
            }
            else
            {
                consulta = "SELECT name FROM sys.tables WHERE name LIKE 'Tag_%' ORDER BY name";
            }

            DbCommand commandObtenerTablas = ObtenerComando(consulta, mEntityContextBASE);

            IDataReader reader = EjecutarReader(commandObtenerTablas);
            try
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        listaTablas.Add(reader.GetString(0));
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }

            return listaTablas;
        }

        public TagsAutoDS ObtenerTablaAutocompletarCompleta(string pNombreTabla)
        {
            TagsAutoDS tagsAutoDS = new TagsAutoDS();
            tagsAutoDS.EnforceConstraints = false;

            DbCommand comando = ObtenerComando($"SELECT * FROM \"{pNombreTabla}\"");
            CargarDataSet(comando, tagsAutoDS, "TagsVariar");

            return tagsAutoDS;
        }

        public void InsertarFilasEnTablaAutocompletar(List<TagsAutoDS.TagsVariarRow> pFilas, string pTablaInsercion)
        {
            string query = $"INSERT INTO \"{pTablaInsercion}\" (\"ProyectoID\", \"Etiqueta\", \"Tipo\", \"Faceta\", \"Cantidad\", \"IdentidadID\", \"Extra\", \"MetaBusqueda\", \"Idioma\", \"GrupoID\") VALUES ";

            int contador = 0;
            int numFila = 0;
            int maxFilasInsercion = 250;

            while (contador < pFilas.Count)
            {
                var filas = pFilas.Skip(contador).Take(maxFilasInsercion);
                contador += maxFilasInsercion;

                DbCommand comando = ObtenerComando(query);
                StringBuilder sb = new StringBuilder(query);
                string coma = "";

                foreach (TagsAutoDS.TagsVariarRow fila in filas)
                {
                    numFila++;
                    sb.AppendLine($"{coma}('{fila.ProyectoID}', {IBD.ToParam($"Etiqueta{numFila}")}, {IBD.ToParam($"Tipo{numFila}")}, {IBD.ToParam($"Faceta{numFila}")}, {fila.Cantidad}, {IBD.ToParam($"IdentidadID{numFila}")}, {IBD.ToParam($"Extra{numFila}")}, {Convert.ToInt16(fila.MetaBusqueda)}, {IBD.ToParam($"Idioma{numFila}")}, {IBD.ToParam($"GrupoID{numFila}")})");

                    AgregarParametro(comando, IBD.ToParam($"Etiqueta{numFila}"), DbType.String, fila.Etiqueta);
                    AgregarParametro(comando, IBD.ToParam($"Tipo{numFila}"), DbType.String, fila.Tipo);
                    AgregarParametro(comando, IBD.ToParam($"Faceta{numFila}"), DbType.String, fila.Faceta);

                    if (!fila.IsIdentidadIDNull())
                    {
                        AgregarParametro(comando, IBD.ToParam($"IdentidadID{numFila}"), DbType.Guid, fila.IdentidadID);
                    }
                    else
                    {
                        AgregarParametro(comando, IBD.ToParam($"IdentidadID{numFila}"), DbType.Guid, DBNull.Value);
                    }

                    if (!fila.IsExtraNull())
                    {
                        AgregarParametro(comando, IBD.ToParam($"Extra{numFila}"), DbType.String, fila.Extra);
                    }
                    else
                    {
                        AgregarParametro(comando, IBD.ToParam($"Extra{numFila}"), DbType.String, DBNull.Value);
                    }

                    if (!fila.IsIdiomaNull())
                    {
                        AgregarParametro(comando, IBD.ToParam($"Idioma{numFila}"), DbType.String, fila.Idioma);
                    }
                    else
                    {
                        AgregarParametro(comando, IBD.ToParam($"Idioma{numFila}"), DbType.String, DBNull.Value);
                    }

                    if (!fila.IsGrupoIDNull())
                    {
                        AgregarParametro(comando, IBD.ToParam($"GrupoID{numFila}"), DbType.Guid, fila.GrupoID);
                    }
                    else
                    {
                        AgregarParametro(comando, IBD.ToParam($"GrupoID{numFila}"), DbType.Guid, DBNull.Value);
                    }
                    coma = ", ";
                }

                comando.CommandText = sb.ToString();

                ActualizarBaseDeDatos(comando);
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Parámetros configuración para el autocompletar de etiquetas en comunidades.
    /// </summary>
    public class ParametroCongifAutoEtiProy
    {
        /// <summary>
        /// Para especificar si la tabla de etiquetas es única o con os 3 primeros caractares del guid del proyecto.
        /// </summary>
        public const string TablaPropia = "TablaPropia";

        /// <summary>
        /// Para especificar las facetas de la busqueda de la comunidad.
        /// </summary>
        public const string FacetasBuscadorCom = "FacetasCom";

        /// <summary>
        /// Para especificar las facetas de una determinada página.
        /// </summary>
        public const string FacetasPagina = "FacetasPag_";
    }

    public class EtiquetaInsercion
    {
        public string EtiquetaInfo { get; set; }

        public List<Guid> Identidades { get; set; } = new List<Guid>();

        public List<Guid> Grupos { get; set; } = new List<Guid>();

        public string TablaInsertar { get; set; }
    }
}
