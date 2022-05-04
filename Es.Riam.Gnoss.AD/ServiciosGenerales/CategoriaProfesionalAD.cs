using Es.Riam.Gnoss.AD.EntityModel;
using Es.Riam.Gnoss.AD.Facetado;
using Es.Riam.Gnoss.Util.General;
using Es.Riam.Util;
using System;
using System.Data;
using System.Data.Common;

namespace Es.Riam.Gnoss.AD.ServiciosGenerales
{
    /// <summary>
    /// Data adapter para categorías profesionales
    /// </summary>
    public class CategoriaProfesionalAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// Constructor por defecto, sin parámetros, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public CategoriaProfesionalAD(UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext, Configuracion configuracion, FacetadoAD facetadoAD)
            : base(utilPeticion, conexion, error, utilTrazas, entityContext, configuracion, facetadoAD)
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">ruta del fichero de configuración</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public CategoriaProfesionalAD(string pFicheroConfiguracionBD, UtilPeticion utilPeticion, Conexion conexion, Error error, UtilTrazas utilTrazas, EntityContext entityContext, Configuracion configuracion, FacetadoAD facetadoAD)
            : base(pFicheroConfiguracionBD, utilPeticion, conexion, error, utilTrazas, entityContext, configuracion, facetadoAD)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string SelectCategoriaProfesional;
        private string sqlSelectCategoriasProfesionalesDeOrganizacion;
        private string sqlSelectPersonasConCategoria;

        #endregion

        #region DataAdapter

        #region CategoriaProfesional

        private string sqlCategoriaProfesionalInsert;

        private string sqlCategoriaProfesionalDelete;

        private string sqlCategoriaProfesionalModify;

        #endregion

        #endregion

        #region Métodos generales

        #region Métodos del data adapter

        /// <summary>
        /// Actualiza categorías professionales
        /// </summary>
        /// <param name="pCategoriaProfesionalDS">Dataset de categorías profesionales</param>
        public void ActualizarCategoriaProfesional(CategoriaProfesionalDS pCategoriaProfesionalDS)
        {
            EliminarBorrados(pCategoriaProfesionalDS);
            GuardarActualizaciones(pCategoriaProfesionalDS);
        }

        /// <summary>
        /// Eliminar categorías profesionales
        /// </summary>
        /// <param name="pCategoriaProfesionalDS">Dataset de categorías profesionales</param>
        public void EliminarBorrados(CategoriaProfesionalDS pCategoriaProfesionalDS)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pCategoriaProfesionalDS.GetChanges(DataRowState.Deleted);

                if (deletedDataSet != null)
                {
                    #region Deleted

                    #region Eliminar tabla CategoriaProfesional

                    DbCommand DeleteCategoriaProfesionalCommand = ObtenerComando(sqlCategoriaProfesionalDelete);
                    AgregarParametro(DeleteCategoriaProfesionalCommand, IBD.ToParam("O_CategoriaProfesionalID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaProfesionalID", DataRowVersion.Original);
                    AgregarParametro(DeleteCategoriaProfesionalCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    AgregarParametro(DeleteCategoriaProfesionalCommand, IBD.ToParam("O_Salario"), DbType.Single, "Salario", DataRowVersion.Original);
                    AgregarParametro(DeleteCategoriaProfesionalCommand, IBD.ToParam("O_OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "CategoriaProfesional", null, null, DeleteCategoriaProfesionalCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

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
        /// Guarda actualizaciones de categorías profesionales
        /// </summary>
        /// <param name="pCategoriaProfesionalDS">Dataset de categorías profesionales</param>
        public void GuardarActualizaciones(CategoriaProfesionalDS pCategoriaProfesionalDS)
        {
            try
            {
                DataSet addedAndModifiedDataSet;
                addedAndModifiedDataSet = pCategoriaProfesionalDS.GetChanges(DataRowState.Added | DataRowState.Modified);
                if (addedAndModifiedDataSet != null)
                {
                    #region AddedAndModified

                    #region Actualizar tabla CategoriaProfesional

                    DbCommand InsertCategoriaProfesionalCommand = ObtenerComando(sqlCategoriaProfesionalInsert);
                    AgregarParametro(InsertCategoriaProfesionalCommand, IBD.ToParam("CategoriaProfesionalID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaProfesionalID", DataRowVersion.Current);
                    AgregarParametro(InsertCategoriaProfesionalCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(InsertCategoriaProfesionalCommand, IBD.ToParam("Salario"), DbType.Single, "Salario", DataRowVersion.Current);
                    AgregarParametro(InsertCategoriaProfesionalCommand, IBD.ToParam("OrganizacionID"), IBD.TipoGuidToObject(DbType.Guid), "OrganizacionID", DataRowVersion.Current);

                    DbCommand ModifyCategoriaProfesionalCommand = ObtenerComando(sqlCategoriaProfesionalModify);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("O_CategoriaProfesionalID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaProfesionalID", DataRowVersion.Original);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("O_Salario"), DbType.Single, "Salario", DataRowVersion.Original);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("O_OrganizacionID"), DbType.Single, "OrganizacionID", DataRowVersion.Original);

                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("CategoriaProfesionalID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaProfesionalID", DataRowVersion.Current);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("Salario"), DbType.Single, "Salario", DataRowVersion.Current);
                    AgregarParametro(ModifyCategoriaProfesionalCommand, IBD.ToParam("OrganizacionID"), DbType.Single, "OrganizacionID", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "CategoriaProfesional", InsertCategoriaProfesionalCommand, ModifyCategoriaProfesionalCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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

        #endregion

        #region Métodos de consultas

        /// <summary>
        /// Obtiene todas las categorías profesionales de una organización pasada como parámetro
        /// </summary>
        /// <param name="pOrganizacionID">Identificador de organización</param>
        /// <returns>Dataset de categorías profesionales</returns>
        public CategoriaProfesionalDS ObtenerCategoriasProfesionalesDeOrganizacion(Guid pOrganizacionID)
        {
            CategoriaProfesionalDS categoriaProfesionalDS = new CategoriaProfesionalDS();

            DbCommand commandsqlSelectCategoriasProfesionalesDeOrganizacion = ObtenerComando(sqlSelectCategoriasProfesionalesDeOrganizacion);
            AgregarParametro(commandsqlSelectCategoriasProfesionalesDeOrganizacion, IBD.ToParam("organizacionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pOrganizacionID));
            CargarDataSet(commandsqlSelectCategoriasProfesionalesDeOrganizacion, categoriaProfesionalDS, "CategoriaProfesional");

            return (categoriaProfesionalDS);
        }

        /// <summary>
        /// Obtiene todas las categorías profesionales del sistema
        /// </summary>
        /// <returns>Dataset de categorías profesionales</returns>
        public CategoriaProfesionalDS ObtenerTODASCategoriasProfesionales()
        {
            CategoriaProfesionalDS categoriaProfesionalDS = new CategoriaProfesionalDS();

            DbCommand commandsqlSelectCategoriasProfesionalesDeOrganizacion = ObtenerComando(SelectCategoriaProfesional + " FROM CategoriaProfesional");
            CargarDataSet(commandsqlSelectCategoriasProfesionalesDeOrganizacion, categoriaProfesionalDS, "CategoriaProfesional");

            return (categoriaProfesionalDS);
        }

        /// <summary>
        /// Comprueba si una categoría profesional pasada por parámetro es usada por alguna persona
        /// </summary>
        /// <param name="categoriaProfesionalID">Identificador de categoría profesional</param>
        /// <returns>TRUE si está siendo usada, FALSE en caso contrario</returns>
        public bool EstaCategoriaProfesionalUsadaPorPersona(Guid categoriaProfesionalID)
        {
            Object categoriaProfesional;
            DbCommand CommandsqlSelectPersonasConCategoria = ObtenerComando(sqlSelectPersonasConCategoria);

            AgregarParametro(CommandsqlSelectPersonasConCategoria, IBD.ToParam("categoriaProfesionalID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(categoriaProfesionalID));

            try
            {
                categoriaProfesional = EjecutarEscalar(CommandsqlSelectPersonasConCategoria);
            }
            catch (Exception x)
            {
                throw x;
            }
            return (categoriaProfesional != null);
        }

        #endregion

        #endregion

        #region Privados

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

            SelectCategoriaProfesional = "SELECT " + IBD.CargarGuid("CategoriaProfesionalID") + ", Nombre, Salario, " + IBD.CargarGuid("OrganizacionID") + " ";

            sqlSelectCategoriasProfesionalesDeOrganizacion = SelectCategoriaProfesional + " FROM CategoriaProfesional WHERE (OrganizacionID = " + IBD.GuidParamValor("organizacionID") + ")";

            sqlSelectPersonasConCategoria = "SELECT " + IBD.CargarGuid("OrganizacionID") + ", " + IBD.CargarGuid("PersonaID") + " FROM PersonaVinculoOrganizacion WHERE (CategoriaProfesionalID = " + IBD.GuidParamValor("categoriaProfesionalID") + ")";

            #endregion

            #region DataAdapter

            #region CategoriaProfesional

            sqlCategoriaProfesionalInsert = IBD.ReplaceParam("INSERT INTO CategoriaProfesional (CategoriaProfesionalID, Nombre, Salario, OrganizacionID) VALUES (" + IBD.GuidParamColumnaTabla("CategoriaProfesionalID") + ", @Nombre, @Salario, " + IBD.GuidParamColumnaTabla("OrganizacionID") + ")");

            sqlCategoriaProfesionalDelete = IBD.ReplaceParam("DELETE FROM CategoriaProfesional WHERE (CategoriaProfesionalID = " + IBD.GuidParamColumnaTabla("O_CategoriaProfesionalID") + ") AND (Nombre = @O_Nombre) AND (Salario = @O_Salario OR @O_Salario IS NULL AND Salario IS NULL) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            sqlCategoriaProfesionalModify = IBD.ReplaceParam("UPDATE CategoriaProfesional SET CategoriaProfesionalID = " + IBD.GuidParamColumnaTabla("CategoriaProfesionalID") + ", Nombre = @Nombre, Salario = @Salario, OrganizacionID = " + IBD.GuidParamColumnaTabla("OrganizacionID") + "  WHERE (CategoriaProfesionalID = " + IBD.GuidParamColumnaTabla("O_CategoriaProfesionalID") + ") AND (Nombre = @O_Nombre) AND (Salario = @O_Salario OR @O_Salario IS NULL AND Salario IS NULL) AND (OrganizacionID = " + IBD.GuidParamColumnaTabla("O_OrganizacionID") + ")");

            #endregion

            #endregion
        }

        #endregion
    }
}