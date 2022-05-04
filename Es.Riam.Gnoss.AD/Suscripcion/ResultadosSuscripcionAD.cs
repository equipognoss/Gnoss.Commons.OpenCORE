using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Es.Riam.Gnoss.AD.Suscripcion.Model;
using Es.Riam.Gnoss.AD.ServiciosGenerales;
using Es.Riam.Gnoss.AD.Documentacion;
using Es.Riam.Gnoss.Web.MVC.Models;

namespace Es.Riam.Gnoss.AD.Suscripcion
{
    #region Enumeraciones

    /// <summary>
    /// Enumeración para los tipos de resultado de suscripción
    /// </summary>
    public enum TipoResultadoSuscripcion
    {
        /// <summary>
        /// Recurso de una comunidad
        /// </summary>
        RecursoComunidad = 0,
        /// <summary>
        /// Artículo de una comunidad
        /// </summary>
        ArticuloComunidad = 1,
        /// <summary>
        /// Recurso de una persona
        /// </summary>
        RecursoPersona = 2,
        /// <summary>
        /// Entrada de un blog
        /// </summary>
        EntradaBlogPersona = 3,
        /// <summary>
        /// Recurso de una persona en una comunidad.
        /// </summary>
        RecursoPersonaEnComunidad = 4
    }

    #endregion

    /// <summary>
    /// DataAdapter de resultados de suscripción
    /// </summary>
    public class ResultadosSuscripcionAD : BaseAD
    {
        #region Constructores

        /// <summary>
        /// El por defecto, utilizado cuando se requiere el GnossConfig.xml por defecto
        /// </summary>
        public ResultadosSuscripcionAD() : base()
        {
            this.CargarConsultasYDataAdapters();
        }

        /// <summary>
        /// Cuando se desea pasar directamente la ruta del fichero de configuración de conexión a la base de datos
        /// </summary>
        /// <param name="pFicheroConfiguracionBD">Ruta del fichero de configuración de conexión a la base de datos</param>
        /// <param name="pUsarVariableEstatica">Si se están usando hilos con diferentes conexiones: FALSE. En caso contrario TRUE</param>
        public ResultadosSuscripcionAD(string pFicheroConfiguracionBD) : base(pFicheroConfiguracionBD)
        {
            this.CargarConsultasYDataAdapters(IBD);
        }

        #endregion

        #region Consultas

        private string sqlSelectResultadoSuscripcion;
        private string sqlSelectResulSusAgCatTes;
        private string sqlSelectColaSuscripciones;

        #endregion

        #region DataAdapter

        #region ResultadoSuscripcion

        private string sqlResultadoSuscripcionInsert;
        private string sqlResultadoSuscripcionDelete;
        private string sqlResultadoSuscripcionModify;

        #endregion

        #region ResulSusAgCatTes

        private string sqlResulSusAgCatTesInsert;
        private string sqlResulSusAgCatTesDelete;
        private string sqlResulSusAgCatTesModify;

        #endregion

        #region ColaSuscripciones
        private string sqlColaSuscripcionesInsert;
        private string sqlColaSuscripcionesDelete;
        private string sqlColaSuscripcionesModify;
        #endregion

        #endregion

        #region Métodos

        #region Métodos privados

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
            this.sqlSelectResultadoSuscripcion = "SELECT " + IBD.CargarGuid("ResultadoSuscripcion.SuscripcionID") + ", " + IBD.CargarGuid("ResultadoSuscripcion.RecursoID") + ", ResultadoSuscripcion.Titulo, ResultadoSuscripcion.Descripcion, ResultadoSuscripcion.TipoResultado, ResultadoSuscripcion.FechaModificacion, " + IBD.CargarGuid("ResultadoSuscripcion.AutorID") + ", ResultadoSuscripcion.OrigenNombre, ResultadoSuscripcion.OrigenNombreCorto, " + IBD.CargarGuid("ResultadoSuscripcion.OrigenID") + ", ResultadoSuscripcion.TipoDocumento, ResultadoSuscripcion.Enlace, ResultadoSuscripcion.Leido, ResultadoSuscripcion.Sincaducidad, ResultadoSuscripcion.FechaProcesado FROM ResultadoSuscripcion";
            this.sqlSelectResulSusAgCatTes = "SELECT " + IBD.CargarGuid("ResulSusAgCatTes.SuscripcionID") + ", " + IBD.CargarGuid("ResulSusAgCatTes.RecursoID") + ", " + IBD.CargarGuid("ResulSusAgCatTes.TesauroID") + ", " + IBD.CargarGuid("ResulSusAgCatTes.CategoriaTesauroID") + ", Nombre FROM ResulSusAgCatTes";
            this.sqlSelectColaSuscripciones = "SELECT ColaSuscripciones.ColaID, " + IBD.CargarGuid("ColaSuscripciones.ElementoID") + ", " + IBD.CargarGuid("ColaSuscripciones.ProyectoID") + ", ColaSuscripciones.Accion, ColaSuscripciones.Tipo, ColaSuscripciones.NumIntentos, ColaSuscripciones.Fecha FROM ColaSuscripciones";
            #endregion

            #region DataAdapter

            #region ResultadoSuscripcion
            this.sqlResultadoSuscripcionInsert = IBD.ReplaceParam("INSERT INTO ResultadoSuscripcion (SuscripcionID, RecursoID, Titulo, Descripcion, TipoResultado, FechaModificacion, AutorID, OrigenNombre, OrigenNombreCorto, OrigenID, TipoDocumento, Enlace, Leido, Sincaducidad, FechaProcesado) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("RecursoID") + ", @Titulo, @Descripcion, @TipoResultado, @FechaModificacion, " + IBD.GuidParamColumnaTabla("AutorID") + ", @OrigenNombre, @OrigenNombreCorto, " + IBD.GuidParamColumnaTabla("OrigenID") + ", @TipoDocumento, @Enlace, @Leido, @Sincaducidad, @FechaProcesado)");
            this.sqlResultadoSuscripcionDelete = IBD.ReplaceParam("DELETE FROM ResultadoSuscripcion WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("Original_SuscripcionID") + ") AND (RecursoID = " + IBD.GuidParamColumnaTabla("Original_RecursoID") + ") AND (Titulo = @Original_Titulo) AND (Descripcion = @Original_Descripcion OR @Original_Descripcion IS NULL AND Descripcion IS NULL) AND (TipoResultado = @Original_TipoResultado) AND (FechaModificacion = @Original_FechaModificacion) AND (AutorID = " + IBD.GuidParamColumnaTabla("Original_AutorID") + " OR " + IBD.GuidParamColumnaTabla("Original_AutorID") + " IS NULL AND AutorID IS NULL) AND (OrigenNombre = @Original_OrigenNombre) AND (OrigenNombreCorto = @Original_OrigenNombreCorto) AND (OrigenID = " + IBD.GuidParamColumnaTabla("Original_OrigenID") + ") AND (TipoDocumento = @Original_TipoDocumento OR @Original_TipoDocumento IS NULL AND TipoDocumento IS NULL) AND (Enlace = @Original_Enlace OR @Original_Enlace IS NULL AND Enlace IS NULL) AND (Leido = @Original_Leido) AND (Sincaducidad = @Original_Sincaducidad) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            this.sqlResultadoSuscripcionModify = IBD.ReplaceParam("UPDATE ResultadoSuscripcion SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", RecursoID = " + IBD.GuidParamColumnaTabla("RecursoID") + ", Titulo = @Titulo, Descripcion = @Descripcion, TipoResultado = @TipoResultado, FechaModificacion = @FechaModificacion, AutorID = " + IBD.GuidParamColumnaTabla("AutorID") + ", OrigenNombre = @OrigenNombre, OrigenNombreCorto = @OrigenNombreCorto, OrigenID = " + IBD.GuidParamColumnaTabla("OrigenID") + ", TipoDocumento = @TipoDocumento, Enlace = @Enlace, Leido = @Leido, Sincaducidad = @Sincaducidad, FechaProcesado = @FechaProcesado WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("Original_SuscripcionID") + ") AND (RecursoID = " + IBD.GuidParamColumnaTabla("Original_RecursoID") + ") AND (Titulo = @Original_Titulo) AND (Descripcion = @Original_Descripcion OR @Original_Descripcion IS NULL AND Descripcion IS NULL) AND (TipoResultado = @Original_TipoResultado) AND (FechaModificacion = @Original_FechaModificacion) AND (AutorID = " + IBD.GuidParamColumnaTabla("Original_AutorID") + " OR " + IBD.GuidParamColumnaTabla("Original_AutorID") + " IS NULL AND AutorID IS NULL) AND (OrigenNombre = @Original_OrigenNombre) AND (OrigenNombreCorto = @Original_OrigenNombreCorto) AND (OrigenID = " + IBD.GuidParamColumnaTabla("Original_OrigenID") + ") AND (TipoDocumento = @Original_TipoDocumento OR @Original_TipoDocumento IS NULL AND TipoDocumento IS NULL) AND (Enlace = @Original_Enlace OR @Original_Enlace IS NULL AND Enlace IS NULL) AND (Leido = @Original_Leido) AND (Sincaducidad = @Original_Sincaducidad) AND (FechaProcesado = @Original_FechaProcesado OR @Original_FechaProcesado IS NULL AND FechaProcesado IS NULL)");
            #endregion

            #region ResulSusAgCatTes
            this.sqlResulSusAgCatTesInsert = IBD.ReplaceParam("INSERT INTO ResulSusAgCatTes (SuscripcionID, RecursoID, TesauroID, CategoriaTesauroID,Nombre) VALUES (" + IBD.GuidParamColumnaTabla("SuscripcionID") + ", " + IBD.GuidParamColumnaTabla("RecursoID") + ", " + IBD.GuidParamColumnaTabla("TesauroID") + ", " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", @Nombre)");
            this.sqlResulSusAgCatTesDelete = IBD.ReplaceParam("DELETE FROM ResulSusAgCatTes WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (RecursoID = " + IBD.GuidParamColumnaTabla("O_RecursoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ") AND (Nombre = @O_Nombre)");
            this.sqlResulSusAgCatTesModify = IBD.ReplaceParam("UPDATE ResulSusAgCatTes SET SuscripcionID = " + IBD.GuidParamColumnaTabla("SuscripcionID") + ", RecursoID = " + IBD.GuidParamColumnaTabla("RecursoID") + ", TesauroID = " + IBD.GuidParamColumnaTabla("TesauroID") + ", CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("CategoriaTesauroID") + ", Nombre = @Nombre WHERE (SuscripcionID = " + IBD.GuidParamColumnaTabla("O_SuscripcionID") + ") AND (RecursoID = " + IBD.GuidParamColumnaTabla("O_RecursoID") + ") AND (TesauroID = " + IBD.GuidParamColumnaTabla("O_TesauroID") + ") AND (CategoriaTesauroID = " + IBD.GuidParamColumnaTabla("O_CategoriaTesauroID") + ") AND (Nombre = @O_Nombre)");
            #endregion

            #region ColaSuscripciones
            this.sqlColaSuscripcionesInsert = IBD.ReplaceParam("INSERT INTO ColaSuscripciones (ElementoID, ProyectoID, Accion, Tipo, NumIntentos, Fecha) VALUES (" + IBD.GuidParamColumnaTabla("ElementoID") + ", " + IBD.GuidParamColumnaTabla("ProyectoID") + ", @Accion, @Tipo, @NumIntentos, @Fecha)");
            this.sqlColaSuscripcionesDelete = IBD.ReplaceParam("DELETE FROM ColaSuscripciones WHERE (ColaID = @Original_ColaID)");
            this.sqlColaSuscripcionesModify = IBD.ReplaceParam("UPDATE ColaSuscripciones SET ElementoID = " + IBD.GuidParamColumnaTabla("ElementoID") + ", ProyectoID = " + IBD.GuidParamColumnaTabla("ProyectoID") + ", Accion = @Accion, Tipo = @Tipo, NumIntentos = @NumIntentos, Fecha = @Fecha WHERE (ColaID = @Original_ColaID) AND (ElementoID = " + IBD.GuidParamColumnaTabla("Original_ElementoID") + ") AND (ProyectoID = " + IBD.GuidParamColumnaTabla("Original_ProyectoID") + ") AND (Accion = @Original_Accion) AND (Tipo = @Original_Tipo) AND (NumIntentos = @Original_NumIntentos) AND (Fecha = @Original_Fecha)");
            #endregion

            #endregion
        }

        #endregion
        
        #region Métodos públicos

        /// <summary>
        /// Actualiza la base de datos a partir de los datos pasados por parámetro
        /// </summary>
        /// <param name="pDataSet">Datset para actualizar</param>
        public void ActualizarBD(DataSet pDataSet)
        {
            EliminarBorrados(pDataSet);
            GuardarActualizaciones(pDataSet);
        }

        /// <summary>
        /// Elimina de la base de datos los elementos marcados para borrar en el dataset pasado por parámetro
        /// </summary>
        /// <param name="pDataSet">Dataset para eliminar datos</param>
        public void EliminarBorrados(DataSet pDataSet)
        {
            try
            {
                DataSet deletedDataSet;
                deletedDataSet = pDataSet.GetChanges(DataRowState.Deleted);
               
                if (deletedDataSet != null)
                {
                    #region Deleted

                    #region Eliminar tabla ResulSusAgCatTes

                    DbCommand DeleteResulSusAgCatTesCommand = ObtenerComando(sqlResulSusAgCatTesDelete);
                    AgregarParametro(DeleteResulSusAgCatTesCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    AgregarParametro(DeleteResulSusAgCatTesCommand, IBD.ToParam("O_RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Original);
                    AgregarParametro(DeleteResulSusAgCatTesCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    AgregarParametro(DeleteResulSusAgCatTesCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
                    AgregarParametro(DeleteResulSusAgCatTesCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ResulSusAgCatTes", null, null, DeleteResulSusAgCatTesCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ResultadoSuscripcion
                    DbCommand DeleteResultadoSuscripcionCommand = ObtenerComando(sqlResultadoSuscripcionDelete);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_Titulo"), DbType.String, "Titulo", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_TipoResultado"), DbType.Int16, "TipoResultado", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_AutorID"), IBD.TipoGuidToObject(DbType.Guid), "AutorID", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_OrigenNombre"), DbType.String, "OrigenNombre", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_OrigenNombreCorto"), DbType.String, "OrigenNombreCorto", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_OrigenID"), IBD.TipoGuidToObject(DbType.Guid), "OrigenID", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_TipoDocumento"), DbType.Int16, "TipoDocumento", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_Enlace"), DbType.String, "Enlace", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_Leido"), DbType.Boolean, "Leido", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_Sincaducidad"), DbType.Boolean, "Sincaducidad", DataRowVersion.Original);
                    AgregarParametro(DeleteResultadoSuscripcionCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ResultadoSuscripcion", null, null, DeleteResultadoSuscripcionCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Eliminar tabla ColaSuscripciones
                    DbCommand DeleteColaSuscripcionesCommand = ObtenerComando(sqlColaSuscripcionesDelete);
                    AgregarParametro(DeleteColaSuscripcionesCommand, IBD.ToParam("Original_ColaID"), DbType.Int32, "ColaID", DataRowVersion.Original);
                    ActualizarBaseDeDatos(deletedDataSet, "ColaSuscripciones", null, null, DeleteColaSuscripcionesCommand, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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

                    #region Actualizar tabla ColaSuscripciones
                    DbCommand InsertColaSuscripcionesCommand = ObtenerComando(sqlColaSuscripcionesInsert);
                    //AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("ColaID"), DbType.Int32, "ColaID", DataRowVersion.Current);
                    AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("Accion"), DbType.Int16, "Accion", DataRowVersion.Current);
                    AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("NumIntentos"), DbType.Int32, "NumIntentos", DataRowVersion.Current);
                    AgregarParametro(InsertColaSuscripcionesCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);

                    DbCommand ModifyColaSuscripcionesCommand = ObtenerComando(sqlColaSuscripcionesModify);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_ColaID"), DbType.Int32, "ColaID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Original);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_Accion"), DbType.Int16, "Accion", DataRowVersion.Original);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_Tipo"), DbType.Int16, "Tipo", DataRowVersion.Original);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_NumIntentos"), DbType.Int32, "NumIntentos", DataRowVersion.Original);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Original_Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Original);

                    //AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("ColaID"), DbType.Int32, "ColaID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("ElementoID"), IBD.TipoGuidToObject(DbType.Guid), "ElementoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("ProyectoID"), IBD.TipoGuidToObject(DbType.Guid), "ProyectoID", DataRowVersion.Current);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Accion"), DbType.Int16, "Accion", DataRowVersion.Current);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Tipo"), DbType.Int16, "Tipo", DataRowVersion.Current);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("NumIntentos"), DbType.Int32, "NumIntentos", DataRowVersion.Current);
                    AgregarParametro(ModifyColaSuscripcionesCommand, IBD.ToParam("Fecha"), DbType.DateTime, "Fecha", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ColaSuscripciones", InsertColaSuscripcionesCommand, ModifyColaSuscripcionesCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ResultadoSuscripcion
                    DbCommand InsertResultadoSuscripcionCommand = ObtenerComando(sqlResultadoSuscripcionInsert);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("Titulo"), DbType.String, "Titulo", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("TipoResultado"), DbType.Int16, "TipoResultado", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("AutorID"), IBD.TipoGuidToObject(DbType.Guid), "AutorID", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("OrigenNombre"), DbType.String, "OrigenNombre", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("OrigenNombreCorto"), DbType.String, "OrigenNombreCorto", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("OrigenID"), IBD.TipoGuidToObject(DbType.Guid), "OrigenID", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("TipoDocumento"), DbType.Int16, "TipoDocumento", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("Enlace"), DbType.String, "Enlace", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("Leido"), DbType.Boolean, "Leido", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("Sincaducidad"), DbType.Boolean, "Sincaducidad", DataRowVersion.Current);
                    AgregarParametro(InsertResultadoSuscripcionCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);

                    DbCommand ModifyResultadoSuscripcionCommand = ObtenerComando(sqlResultadoSuscripcionModify);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_Titulo"), DbType.String, "Titulo", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_Descripcion"), DbType.String, "Descripcion", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_TipoResultado"), DbType.Int16, "TipoResultado", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_AutorID"), IBD.TipoGuidToObject(DbType.Guid), "AutorID", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_OrigenNombre"), DbType.String, "OrigenNombre", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_OrigenNombreCorto"), DbType.String, "OrigenNombreCorto", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_OrigenID"), IBD.TipoGuidToObject(DbType.Guid), "OrigenID", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_TipoDocumento"), DbType.Int16, "TipoDocumento", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_Enlace"), DbType.String, "Enlace", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_Leido"), DbType.Boolean, "Leido", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_Sincaducidad"), DbType.Boolean, "Sincaducidad", DataRowVersion.Original);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Original_FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Original);

                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Titulo"), DbType.String, "Titulo", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Descripcion"), DbType.String, "Descripcion", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("TipoResultado"), DbType.Int16, "TipoResultado", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("FechaModificacion"), DbType.DateTime, "FechaModificacion", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("AutorID"), IBD.TipoGuidToObject(DbType.Guid), "AutorID", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("OrigenNombre"), DbType.String, "OrigenNombre", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("OrigenNombreCorto"), DbType.String, "OrigenNombreCorto", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("OrigenID"), IBD.TipoGuidToObject(DbType.Guid), "OrigenID", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("TipoDocumento"), DbType.Int16, "TipoDocumento", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Enlace"), DbType.String, "Enlace", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Leido"), DbType.Boolean, "Leido", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("Sincaducidad"), DbType.Boolean, "Sincaducidad", DataRowVersion.Current);
                    AgregarParametro(ModifyResultadoSuscripcionCommand, IBD.ToParam("FechaProcesado"), DbType.DateTime, "FechaProcesado", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ResultadoSuscripcion", InsertResultadoSuscripcionCommand, ModifyResultadoSuscripcionCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

                    #endregion

                    #region Actualizar tabla ResulSusAgCatTes

                    DbCommand InsertResulSusAgCatTesCommand = ObtenerComando(sqlResulSusAgCatTesInsert);
                    AgregarParametro(InsertResulSusAgCatTesCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    AgregarParametro(InsertResulSusAgCatTesCommand, IBD.ToParam("RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Current);
                    AgregarParametro(InsertResulSusAgCatTesCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    AgregarParametro(InsertResulSusAgCatTesCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    AgregarParametro(InsertResulSusAgCatTesCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);

                    DbCommand ModifyResulSusAgCatTesCommand = ObtenerComando(sqlResulSusAgCatTesModify);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("O_SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Original);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("O_RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Original);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("O_TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Original);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("O_CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Original);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("O_Nombre"), DbType.String, "Nombre", DataRowVersion.Original);

                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("SuscripcionID"), IBD.TipoGuidToObject(DbType.Guid), "SuscripcionID", DataRowVersion.Current);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("RecursoID"), IBD.TipoGuidToObject(DbType.Guid), "RecursoID", DataRowVersion.Current);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("TesauroID"), IBD.TipoGuidToObject(DbType.Guid), "TesauroID", DataRowVersion.Current);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("CategoriaTesauroID"), IBD.TipoGuidToObject(DbType.Guid), "CategoriaTesauroID", DataRowVersion.Current);
                    AgregarParametro(ModifyResulSusAgCatTesCommand, IBD.ToParam("Nombre"), DbType.String, "Nombre", DataRowVersion.Current);
                    ActualizarBaseDeDatos(addedAndModifiedDataSet, "ResulSusAgCatTes", InsertResulSusAgCatTesCommand, ModifyResulSusAgCatTesCommand, null, Microsoft.Practices.EnterpriseLibrary.Data.UpdateBehavior.Transactional);

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

        /// <summary>
        /// Obtiene el número de resultados de las suscripciones de todos los perfiles
        /// </summary>
        /// <returns></returns>
        public ResultadosSuscripcionDS ObtenerTodosResultadosSuscripcion()
        {
            ResultadosSuscripcionDS resultadosSuscripcionDS = new ResultadosSuscripcionDS();

            DbCommand comandoResultados = ObtenerComando(sqlSelectResultadoSuscripcion);
            CargarDataSet(comandoResultados, resultadosSuscripcionDS, "ResultadoSuscripcion");

            return resultadosSuscripcionDS;
        }

        /// <summary>
        /// Obtiene los resultados de las suscripciones (ResultadoSuscripcion) de un perfil
        /// </summary>
        /// <param name="pResultadosDS">Dataset de resultados de suscripciones</param>
        /// <param name="pPerfilID">Identificador del perfil para el que se obtienen los resultados</param>
        /// <param name="pInicio">Elemento inicial para paginar</param>
        /// <param name="pLimite">Elemento final para paginar</param>
        /// <param name="pFechaInicical">Fecha inicial para los resultados a partir de la que se obtienen</param>
        /// <param name="pOrderBy">Criterio para ordenar</param>
        /// <param name="pObtenerBloqueadas">TRUE si se desea traer los resultados de suscripciones bloqueadas, 
        /// FALSE en caso contrario</param>
        /// <param name="pTraerRepetidos">Indica si hay que traer repetidos</param>
        /// <returns></returns>
        public int ObtenerResultadosSuscripcionesDePerfil(ResultadosSuscripcionDS pResultadosDS, Guid pPerfilID, int pInicio, int pLimite, DateTime pFechaInicial, string pOrderBy, bool pObtenerBloqueadas, bool pTraerRepetidos)
        {
            ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();

            //El motivo de poner estas lineas es q se paginan los resultados trayendonos entre inicio/limite para la tabla "ResultadoSuscripcion" pero NO se pagina para las relaciones de "ResulSusAgCatTes" se trae todas las filas posibles
            pResultadosDS.EnforceConstraints = false;
            resDS.EnforceConstraints = false;

            int cuantos = 0;

            //1ºObtengo las suscripciones sin repetir para que la paginación funcione:
            DbCommand comandoResultadosNoRepe = ObtenerComando("SELECT DISTINCT RecursoID, MAX(ResultadoSuscripcion.FechaModificacion) FechaModificacion FROM ResultadoSuscripcion INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE ((Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND  ResultadoSuscripcion.Sincaducidad = 0) OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.Sincaducidad = 1))");

            if (!pObtenerBloqueadas)
            {
                comandoResultadosNoRepe.CommandText = comandoResultadosNoRepe.CommandText + " AND Suscripcion.Bloqueada = 0";
            }
            comandoResultadosNoRepe.CommandText += " GROUP BY RecursoID";
            AgregarParametro(comandoResultadosNoRepe, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            AgregarParametro(comandoResultadosNoRepe, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            cuantos = PaginarDataSet(comandoResultadosNoRepe, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");

            List<Guid> listaRecursosID = new List<Guid>();
            foreach (ResultadosSuscripcionDS.ResultadoSuscripcionRow filaResultado in resDS.ResultadoSuscripcion)
            {
                listaRecursosID.Add(filaResultado.RecursoID);
            }

            resDS.Clear();

            if (listaRecursosID.Count > 0)
            {
                //ResultadoSuscripcion
                DbCommand comandoResultados = ObtenerComando(sqlSelectResultadoSuscripcion + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE ((Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND  ResultadoSuscripcion.Sincaducidad = 0) OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.Sincaducidad = 1))");

                comandoResultados.CommandText += " AND ResultadoSuscripcion.RecursoID IN (";

                foreach (Guid recursoID in listaRecursosID)
                {
                    comandoResultados.CommandText += IBD.GuidValor(recursoID) + ",";
                }

                comandoResultados.CommandText = comandoResultados.CommandText.Substring(0, comandoResultados.CommandText.Length - 1) + ")";

                if (!pObtenerBloqueadas)
                {
                    comandoResultados.CommandText = comandoResultados.CommandText + " AND Suscripcion.Bloqueada = 0";
                }

                comandoResultados.CommandText += " " + pOrderBy;

                AgregarParametro(comandoResultados, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
                AgregarParametro(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
                CargarDataSet(comandoResultados, resDS, "ResultadoSuscripcion");

                if (!pTraerRepetidos)
                {
                    List<Guid> listaClavesRecursos = new List<Guid>();
                    List<Guid> listaClavesRecursosDuplicados = new List<Guid>();

                    foreach (ResultadosSuscripcionDS.ResultadoSuscripcionRow fila in resDS.ResultadoSuscripcion.Rows)
                    {
                        if (!listaClavesRecursos.Contains(fila.RecursoID))
                        {
                            listaClavesRecursos.Add(fila.RecursoID);
                        }
                        else
                        {
                            if (!listaClavesRecursosDuplicados.Contains(fila.RecursoID))
                            {
                                listaClavesRecursosDuplicados.Add(fila.RecursoID);
                            }
                        }
                    }
                    int resultadosEliminados = 0;
                    foreach (Guid recursoID in listaClavesRecursosDuplicados)
                    {
                        bool soloUNO = false;
                        while (!soloUNO)
                        {
                            resDS.ResultadoSuscripcion.Select("RecursoID = '" + recursoID + "'")[0].Delete();
                            resDS.ResultadoSuscripcion.AcceptChanges();
                            resultadosEliminados = resultadosEliminados + 1;
                            if (resDS.ResultadoSuscripcion.Select("RecursoID = '" + recursoID + "'").Length == 1)
                            {
                                soloUNO = true;
                            }
                        }
                    }
                }

                //ResulSusAgCatTes
                DbCommand comandoResultadosAgCat = ObtenerComando(sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON ResultadoSuscripcion.SuscripcionID = ResulSusAgCatTes.SuscripcionID AND ResultadoSuscripcion.RecursoID = ResulSusAgCatTes.RecursoID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND  ResultadoSuscripcion.Sincaducidad = 0) OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.Sincaducidad = 1)");

                comandoResultadosAgCat.CommandText += " AND ResultadoSuscripcion.RecursoID IN (";

                foreach (Guid recursoID in listaRecursosID)
                {
                    comandoResultadosAgCat.CommandText += IBD.GuidValor(recursoID) + ",";
                }

                comandoResultadosAgCat.CommandText = comandoResultadosAgCat.CommandText.Substring(0, comandoResultadosAgCat.CommandText.Length - 1) + ")";

                AgregarParametro(comandoResultadosAgCat, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
                AgregarParametro(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);

                if (!pObtenerBloqueadas)
                {
                    comandoResultadosAgCat.CommandText = comandoResultadosAgCat.CommandText + " AND Suscripcion.Bloqueada = 0";
                }
                CargarDataSet(comandoResultadosAgCat, resDS, "ResulSusAgCatTes");

                pResultadosDS.Merge(resDS, true);
            }

            return cuantos;

            #region Antiguo
            //ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();

            ////El motivo de poner estas lineas es q se paginan los resultados trayendonos entre inicio/limite para la tabla "ResultadoSuscripcion" pero NO se pagina para las relaciones de "ResulSusAgCatTes" se trae todas las filas posibles
            //pResultadosDS.EnforceConstraints = false;
            //resDS.EnforceConstraints = false;
            
            //int cuantos = 0;

            ////ResultadoSuscripcion
            //DbCommand comandoResultados = ObtenerComando(sqlSelectResultadoSuscripcion + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND  ResultadoSuscripcion.Sincaducidad = 0) OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.Sincaducidad = 1)");
            
            //if (!pObtenerBloqueadas)
            //{
            //    comandoResultados.CommandText = comandoResultados.CommandText + " AND Suscripcion.Bloqueada = 0";
            //}
            //AgregarParametro(comandoResultados, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            //AgregarParametro(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            //cuantos = PaginarDataSet(comandoResultados, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");

            //List<Guid> listaClavesRecursos = new List<Guid>();
            //List<Guid> listaClavesRecursosDuplicados = new List<Guid>();

            ////DESCOMENTAR SI SE DESEA QUE NO SE REPITAN RECURSOS AUNQUE SEAN DE SUSCRIPCIONES DIFERENTES:

            ////foreach (ResultadosSuscripcionDS.ResultadoSuscripcionRow fila in resDS.ResultadoSuscripcion.Rows)
            ////{
            ////    if (!listaClavesRecursos.Contains(fila.RecursoID))
            ////    {
            ////        listaClavesRecursos.Add(fila.RecursoID);
            ////    }
            ////    else
            ////    {
            ////        if (!listaClavesRecursosDuplicados.Contains(fila.RecursoID))
            ////        {
            ////            listaClavesRecursosDuplicados.Add(fila.RecursoID);
            ////        }
            ////    }
            ////}
            //int resultadosEliminados = 0;
            //foreach (Guid recursoID in listaClavesRecursosDuplicados)
            //{   bool soloUNO = false;
            //    while (!soloUNO)
            //    {
            //        resDS.ResultadoSuscripcion.Select("RecursoID = '" + recursoID + "'")[0].Delete();
            //        resDS.ResultadoSuscripcion.AcceptChanges();
            //        resultadosEliminados = resultadosEliminados + 1;
            //        if (resDS.ResultadoSuscripcion.Select("RecursoID = '" + recursoID + "'").Length == 1)
            //        {
            //            soloUNO = true;
            //        }
            //    }
            //}
            
            //int numeroLimite = pLimite - pInicio;
            //int numeroDiferencia = pLimite - pInicio;
            //if (pInicio != 0)
            //{
            //    numeroLimite = numeroLimite + 1;
            //    numeroDiferencia = numeroDiferencia + 1;
            //}
            //if (cuantos < numeroLimite)
            //{
            //    numeroLimite = cuantos;
            //}
            ////Caso que tendamos toda la pagina llena pero con alguno repetido
            //if (resultadosEliminados > 0 && (numeroLimite == (numeroDiferencia)))
            //{
            //    int resultadosQueFaltan = numeroLimite - resDS.ResultadoSuscripcion.Rows.Count;

            //    DbCommand comandoResultadosAux = ObtenerComando(sqlSelectResultadoSuscripcion + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND  ResultadoSuscripcion.Sincaducidad = 0) OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.Sincaducidad = 1)");

            //    if (!pObtenerBloqueadas)
            //    {
            //        comandoResultadosAux.CommandText = comandoResultadosAux.CommandText + " AND Suscripcion.Bloqueada = 0";
            //    }
            //    AgregarParametro(comandoResultadosAux, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            //    AgregarParametro(comandoResultadosAux, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            //    ResultadosSuscripcionDS resAUXDS = new ResultadosSuscripcionDS();
            //    PaginarDataSet(comandoResultadosAux, pOrderBy, resAUXDS, pInicio + (pLimite - pInicio), pLimite + (pLimite - pInicio), "ResultadoSuscripcion");
            //    int i = 1;
            //    foreach (ResultadosSuscripcionDS.ResultadoSuscripcionRow filaAux in resAUXDS.ResultadoSuscripcion.Rows)
            //    {
            //        if (i <= resultadosQueFaltan)
            //        {
            //            if (!listaClavesRecursos.Contains(filaAux.RecursoID))
            //            {
            //                listaClavesRecursos.Add(filaAux.RecursoID);
            //                resDS.ResultadoSuscripcion.ImportRow(filaAux);
            //                i = i + 1;
            //            }
            //        }
            //    }
            //    cuantos = cuantos - (resultadosEliminados);
            //    resAUXDS.Dispose();
            //    resAUXDS = null;
            //}
            ////Caso que tendamos toda la pagina llena pero sin ninguno repetido
            //else if (resultadosEliminados == 0 && (numeroLimite == (numeroDiferencia)))
            //{

            //}
            ////Caso que tendamos la pagina no llena por pocos resultados llena pero sin ninguno repetido
            //else if (resultadosEliminados == 0 && (numeroLimite < (numeroDiferencia)))
            //{
            //    cuantos = numeroLimite;
            //}
            ////Caso que tendamos la pagina no llena por pocos resultados pero con algun resultado repetido
            //else
            //{
            //    cuantos = cuantos - (resultadosEliminados);
            //}
            //listaClavesRecursosDuplicados.Clear();
            //listaClavesRecursosDuplicados = null;
            //listaClavesRecursos.Clear();
            //listaClavesRecursos = null;

            ////ResulSusAgCatTes
            //DbCommand comandoResultadosAgCat = ObtenerComando(sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON ResultadoSuscripcion.SuscripcionID = ResulSusAgCatTes.SuscripcionID AND ResultadoSuscripcion.RecursoID = ResulSusAgCatTes.RecursoID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND  ResultadoSuscripcion.Sincaducidad = 0) OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.Sincaducidad = 1)");
            //AgregarParametro(comandoResultadosAgCat, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            //AgregarParametro(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            
            //if (!pObtenerBloqueadas)
            //{
            //    comandoResultadosAgCat.CommandText = comandoResultadosAgCat.CommandText + " AND Suscripcion.Bloqueada = 0";
            //}
            //CargarDataSet(comandoResultadosAgCat, resDS, "ResulSusAgCatTes");

            //pResultadosDS.Merge(resDS, true);

            //return cuantos;

            #endregion
        }

        /// <summary>
        /// Obtiene el número de resultados de las suscripciones de un perfil
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil para el que se obtiene el número de resultados</param>
        /// <param name="pFechaInicial">Fecha a partir de la cual un resultado entra en el cómputo</param>
        /// <param name="pObtenerBloqueadas">TRUE si se desean contar los resultados de suscripciones bloqueadas, FALSE en caso contrario</param>
        /// <returns></returns>
        public int ObtenerNumeroResultadosSuscripcionesDePerfil(Guid pPerfilID, DateTime pFechaInicial, bool pObtenerBloqueadas)
        {
            int cuantos = 0;

            DbCommand comandoResultados = ObtenerComando("Select count(*) from ( SELECT DISTINCT " + IBD.CargarGuid("ResultadoSuscripcion.RecursoID") + " FROM ResultadoSuscripcion INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND  ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + " AND (ResultadoSuscripcion.Sincaducidad = 0 AND ResultadoSuscripcion.Leido = 0)");
            
            if (!pObtenerBloqueadas)
            {
                comandoResultados.CommandText = comandoResultados.CommandText + " AND Suscripcion.Bloqueada = 0";
            }
            comandoResultados.CommandText = comandoResultados.CommandText + " ) Contar";

            AgregarParametro(comandoResultados, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            AgregarParametro(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            cuantos = System.Convert.ToInt32(EjecutarEscalar(comandoResultados));

            return cuantos;
        }

        /// <summary>
        /// Obtiene los resultados de una suscripcion concreta
        /// </summary>
        /// <param name="pResultadosDS">Dataset de resultados</param>
        /// <param name="pSuscripcionID">Suscripción para la que se obtienen los resultados</param>
        /// <param name="pTipoSuscripcion">Tipo de la suscripción</param>
        /// <param name="pObtenerBloqueadas">TRUE si se desea traer los resultados de suscripciones bloqueadas, FALSE en caso contrario</param>
        /// <param name="pFechaInicial">Fecha inicial para los resultados a partir de la que se obtienen</param>
        /// <param name="pInicio">Elemento inicial para paginar</param>
        /// <param name="pLimite">Elemento final para paginar</param>
        /// <param name="pOrderBy">Criterio para ordenar</param>
        /// <returns></returns>
        public int ObtenerResultadosSuscripcionPorSuscripcion(ResultadosSuscripcionDS pResultadosDS, Guid pSuscripcionID, TipoSuscripciones pTipoSuscripcion, bool pObtenerBloqueadas, DateTime pFechaInicial, int pInicio, int pLimite, string pOrderBy)
        {
            ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();

            //El motivo de poner estas lineas es q se paginan los resultados trayendonos entre inicio/limite para la tabla "ResultadoSuscripcion" pero NO se pagina para las relaciones de "ResulSusAgCatTes" se trae todas las filas posibles
            pResultadosDS.EnforceConstraints = false;
            resDS.EnforceConstraints = false;

            int cuantos = 0;
            string tipoResultado = " AND (ResultadoSuscripcion.TipoResultado=";

            if (pTipoSuscripcion == TipoSuscripciones.Comunidades)
            {
                tipoResultado += (short)TipoResultadoSuscripcion.RecursoComunidad + ")";
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Personas)
            {
                tipoResultado += (short)TipoResultadoSuscripcion.RecursoPersona + " OR ResultadoSuscripcion.TipoResultado=" + (short)TipoResultadoSuscripcion.RecursoPersonaEnComunidad + ")";
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Blogs)
            {
                tipoResultado += (short)TipoResultadoSuscripcion.EntradaBlogPersona + ")";
            }
            //ResultadoSuscripcion
            DbCommand comandoResultados = ObtenerComando(sqlSelectResultadoSuscripcion + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID WHERE ((ResultadoSuscripcion.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND  ResultadoSuscripcion.Sincaducidad = 0 AND ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + tipoResultado + ") OR (ResultadoSuscripcion.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND  ResultadoSuscripcion.Sincaducidad = 1 " + tipoResultado + "))");
            
            if (!pObtenerBloqueadas)
            {
                comandoResultados.CommandText = comandoResultados.CommandText + " AND Suscripcion.Bloqueada = 0";
            }

            if (!pOrderBy.Equals(string.Empty))
            {
                comandoResultados.CommandText = comandoResultados.CommandText + " " + pOrderBy;
            }
            AgregarParametro(comandoResultados, IBD.GuidParamValor("SuscripcionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pSuscripcionID));
            AgregarParametro(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            cuantos = PaginarDataSet(comandoResultados, pOrderBy, resDS, pInicio, pLimite, "ResultadoSuscripcion");

            //ResulSusAgCatTes
            DbCommand comandoResultadosAgCat = ObtenerComando(sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON ResultadoSuscripcion.SuscripcionID = ResulSusAgCatTes.SuscripcionID AND ResultadoSuscripcion.RecursoID = ResulSusAgCatTes.RecursoID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID WHERE (ResultadoSuscripcion.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND  ResultadoSuscripcion.Sincaducidad = 0 AND ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + tipoResultado + ") OR (ResultadoSuscripcion.SuscripcionID = " + IBD.GuidParamValor("SuscripcionID") + " AND  ResultadoSuscripcion.Sincaducidad = 1 " + tipoResultado + ")");
            AgregarParametro(comandoResultadosAgCat, IBD.GuidParamValor("SuscripcionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pSuscripcionID));
            AgregarParametro(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            
            if (!pObtenerBloqueadas)
            {
                comandoResultadosAgCat.CommandText = comandoResultadosAgCat.CommandText + " AND Suscripcion.Bloqueada = 0";
            }
            CargarDataSet(comandoResultadosAgCat, resDS, "ResulSusAgCatTes");

            pResultadosDS.Merge(resDS, true);

            return cuantos;
        }

        /// <summary>
        /// Obtien los resultados de suscripciones de un determinado elemento.
        /// </summary>
        /// <param name="pElementoID">Identificador del elemento</param>
        /// <returns>DataSet con los resultados de suscripciones de un determinado elemento</returns>
        public ResultadosSuscripcionDS ObtenerResultadosSuscripcionPorElementoID(Guid pElementoID)
        {
            ResultadosSuscripcionDS resulSuscpDS = new ResultadosSuscripcionDS();
            DbCommand comandoSeleSus = ObtenerComando(sqlSelectResultadoSuscripcion + " WHERE RecursoID=" + IBD.GuidValor(pElementoID));
            CargarDataSet(comandoSeleSus, resulSuscpDS, "ResultadoSuscripcion");

            DbCommand comandoSeleSusAgTes = ObtenerComando(sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON (ResulSusAgCatTes.SuscripcionID=ResultadoSuscripcion.SuscripcionID AND ResulSusAgCatTes.RecursoID=ResultadoSuscripcion.RecursoID) WHERE ResultadoSuscripcion.RecursoID=" + IBD.GuidValor(pElementoID));
            CargarDataSet(comandoSeleSusAgTes, resulSuscpDS, "ResulSusAgCatTes");

            return resulSuscpDS;
        }

        /// <summary>
        /// Obtien los resultados de suscripciones de un determinado elemento.
        /// </summary>
        /// <param name="pElementoID">Identificador del elemento</param>
        /// <returns>DataSet con los resultados de suscripciones de un determinado elemento</returns>
        public ResultadosSuscripcionDS ObtenerResultadosSuscripcionPorSuscripcionesIDYElementoesID(Dictionary<Guid, List<Guid>> pElementosID)
        {
            ResultadosSuscripcionDS resulSuscpDS = new ResultadosSuscripcionDS();

            if (pElementosID.Count > 0)
            {
                string where = " WHERE ";

                foreach (Guid suscripcionID in pElementosID.Keys)
                {
                    foreach (Guid recID in pElementosID[suscripcionID])
                    {
                        where += "(ResultadoSuscripcion.RecursoID=" + IBD.GuidValor(suscripcionID) + " AND ResultadoSuscripcion.SuscripcionID=" + IBD.GuidValor(recID) + ") OR";
                    }
                }

                where = where.Substring(0, where.Length - 3);

                DbCommand comandoSeleSus = ObtenerComando(sqlSelectResultadoSuscripcion + where);
                CargarDataSet(comandoSeleSus, resulSuscpDS, "ResultadoSuscripcion");

                DbCommand comandoSeleSusAgTes = ObtenerComando(sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON (ResulSusAgCatTes.SuscripcionID=ResultadoSuscripcion.SuscripcionID AND ResulSusAgCatTes.RecursoID=ResultadoSuscripcion.RecursoID)" + where);
                CargarDataSet(comandoSeleSusAgTes, resulSuscpDS, "ResulSusAgCatTes");
            }

            return resulSuscpDS;
        }

        /// <summary>
        /// Obtiene los resultados de las suscripciones de un perfil, de un tipo concreto
        /// </summary>
        /// <param name="pResultadosDS">Dataset de resultados</param>
        /// <param name="pPerfilID">Identificador del perfil para el que se obtienen los resultados</param>
        /// <param name="pTipoSuscripcion">Tipo de la suscripcion</param>
        /// <param name="pObtenerBloqueadas">TRUE si se desea traer los resultados de suscripciones bloqueadas, FALSE en caso contrario</param>
        /// <param name="pFechaInicial">Fecha inicial para los resultados a partir de la que se obtienen</param>
        /// <param name="pOrderBy">Criterio para ordenar</param>
        /// <returns></returns>
        public void ObtenerResultadosSuscripcionDePerfilPorTipoSuscripcion(ResultadosSuscripcionDS pResultadosDS, Guid pPerfilID, TipoSuscripciones pTipoSuscripcion, bool pObtenerBloqueadas, DateTime pFechaInicial, string pOrderBy)
        {
            ResultadosSuscripcionDS resDS = new ResultadosSuscripcionDS();
            string tipoResultado = " AND (ResultadoSuscripcion.TipoResultado=";
            
            if (pTipoSuscripcion == TipoSuscripciones.Comunidades)
            {
                tipoResultado += (short)TipoResultadoSuscripcion.RecursoComunidad + ")";
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Personas)
            {
                tipoResultado += (short)TipoResultadoSuscripcion.RecursoPersona + " OR ResultadoSuscripcion.TipoResultado=" + (short)TipoResultadoSuscripcion.RecursoPersonaEnComunidad + ")";
            }
            else if (pTipoSuscripcion == TipoSuscripciones.Blogs)
            {
                tipoResultado += (short)TipoResultadoSuscripcion.EntradaBlogPersona + ")";
            }
            //ResultadoSuscripcion
            DbCommand comandoResultados = ObtenerComando(sqlSelectResultadoSuscripcion + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE ((Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND ResultadoSuscripcion.Sincaducidad = 0 AND ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + tipoResultado + ") OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND ResultadoSuscripcion.Sincaducidad = 1" + tipoResultado + "))");

            if (!pObtenerBloqueadas)
            {
                comandoResultados.CommandText = comandoResultados.CommandText + " AND Suscripcion.Bloqueada = 0";
            }

            if (!pOrderBy.Equals(string.Empty))
            {
                comandoResultados.CommandText = comandoResultados.CommandText + " " + pOrderBy;
            }
            AgregarParametro(comandoResultados, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            AgregarParametro(comandoResultados, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            CargarDataSet(comandoResultados, resDS, "ResultadoSuscripcion");

            //ResulSusAgCatTes
            DbCommand comandoResultadosAgCat = ObtenerComando(sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON ResultadoSuscripcion.SuscripcionID = ResulSusAgCatTes.SuscripcionID AND ResultadoSuscripcion.RecursoID = ResulSusAgCatTes.RecursoID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND ResultadoSuscripcion.Sincaducidad = 0 AND ResultadoSuscripcion.FechaModificacion > " + IBD.ToParam("FechaInicial") + tipoResultado + ") OR (Identidad.PerfilID = " + IBD.GuidParamValor("PerfilID") + " AND ResultadoSuscripcion.Sincaducidad = 1" + tipoResultado + ")");
            AgregarParametro(comandoResultadosAgCat, IBD.GuidParamValor("PerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            AgregarParametro(comandoResultadosAgCat, IBD.ToParam("FechaInicial"), DbType.DateTime, pFechaInicial);
            
            if (!pObtenerBloqueadas)
            {
                comandoResultadosAgCat.CommandText = comandoResultadosAgCat.CommandText + " AND Suscripcion.Bloqueada = 0";
            }
            CargarDataSet(comandoResultadosAgCat, resDS, "ResulSusAgCatTes");
          
            pResultadosDS.Merge(resDS, true);
        }

        /// <summary>
        /// Actualiza el valor del campo "Leído" de un resultado de suscripción
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de la suscripción</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pValor">Booleano para introducir en el campo</param>
        public bool ActualizarLeidoDeResulSusc(Guid pSuscripcionID, Guid pRecursoID, bool pValor)
        {
            string comandoSql = string.Empty;

            if (pValor)
            {
                comandoSql = "UPDATE ResultadoSuscripcion SET Leido = 1  WHERE Leido=0 AND RecursoID = " + IBD.GuidValor(pRecursoID);
            }
            else
            {
                comandoSql = "UPDATE ResultadoSuscripcion SET Leido = 0  WHERE Leido=1 AND RecursoID = " + IBD.GuidValor(pRecursoID);
            }

            comandoSql += " AND SuscripcionID IN (SELECT SuscripcionID FROM Suscripcion WHERE IdentidadID IN (SELECT IdentidadID FROM Suscripcion WHERE SuscripcionID = " + IBD.GuidValor(pSuscripcionID) + "))";

            DbCommand commandcomandoSql = ObtenerComando(comandoSql);
            int filas = ActualizarBaseDeDatos(commandcomandoSql);

            return (filas > 0);
        }

        /// <summary>
        /// Actualiza el valor del campo "SinCaducidad" de un resultado de suscripción
        /// </summary>
        /// <param name="pSuscripcionID">Identificador de la suscripción</param>
        /// <param name="pRecursoID">Identificador del recurso</param>
        /// <param name="pValor">Booleano para introducir en el campo</param>
        /// <returns>TRUE si el resultado se pone con caducidad y el recurso no está leido</returns>
        public bool ActualizarSincaducidadDeResulSusc(Guid pSuscripcionID, Guid pRecursoID, bool pValor)
        {
            string comandoSql = string.Empty;

            if (pValor)
            {
                comandoSql = "UPDATE ResultadoSuscripcion SET Sincaducidad = 1  WHERE SuscripcionID = " + IBD.GuidValor(pSuscripcionID) + " AND RecursoID = " + IBD.GuidValor(pRecursoID);
            }
            else
            {
                comandoSql = "UPDATE ResultadoSuscripcion SET Sincaducidad = 0  WHERE SuscripcionID = " + IBD.GuidValor(pSuscripcionID) + " AND RecursoID = " + IBD.GuidValor(pRecursoID);
            }
            DbCommand commandcomandoSql = ObtenerComando(comandoSql);
            ActualizarBaseDeDatos(commandcomandoSql);

            commandcomandoSql = ObtenerComando("SELECT Leido FROM ResultadoSuscripcion WHERE SuscripcionID = " + IBD.GuidValor(pSuscripcionID) + " AND RecursoID = " + IBD.GuidValor(pRecursoID));
            bool leido = (bool)EjecutarEscalar(commandcomandoSql);

            return leido;
        }

        /// <summary>
        /// Elimina las filas de "ResultadosSuscripcion" y "ResSuscAgCatTes" de aquellos resultados de un determinado perfil que no deban estar en su bandeja de suscripciones debidoa su antiguedad y tiempo de permanencia (pCaducidadResSusc), nunca eliminará los resultados  marcados como no caducos
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pCaducidadResSusc">Periodo (7 o 30 días) de permanencia del los resultados de suscripciones en la bandeja</param>
        public void BorrarResultSuscViejasDePerfil(Guid pPerfilID, int pCaducidadResSusc)
        {
            //ResulSusAgCatTes
            string sqlBorrarResulSusAgCatTesViejas = "DELETE FROM ResulSusAgCatTes FROM ResulSusAgCatTes INNER JOIN ResultadoSuscripcion ON ResultadoSuscripcion.SuscripcionID = ResulSusAgCatTes.SuscripcionID AND ResultadoSuscripcion.RecursoID = ResulSusAgCatTes.RecursoID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE Identidad.PerfilID = " + IBD.GuidValor(pPerfilID) + " AND ResultadoSuscripcion.FechaModificacion < " + IBD.ToParam("fechaModificacion") + " AND ResultadoSuscripcion.Sincaducidad = 0 ";
            DbCommand commandBorrarResulSusAgCatTesViejas = ObtenerComando(sqlBorrarResulSusAgCatTesViejas);
            AgregarParametro(commandBorrarResulSusAgCatTesViejas, IBD.ToParam("fechaModificacion"), DbType.DateTime, DateTime.Today.AddDays(pCaducidadResSusc * -1));
            ActualizarBaseDeDatos(commandBorrarResulSusAgCatTesViejas);

            //ResultadoSuscripcion
            string sqlBorrarResultadoSuscripcionViejas = "DELETE FROM ResultadoSuscripcion FROM ResultadoSuscripcion INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE Identidad.PerfilID = " + IBD.GuidValor(pPerfilID) + " AND ResultadoSuscripcion.FechaModificacion < " + IBD.ToParam("fechaModificacion") + " AND ResultadoSuscripcion.Sincaducidad = 0 ";
            DbCommand commandBorarResultadoSuscripcionViejas = ObtenerComando(sqlBorrarResultadoSuscripcionViejas);
            AgregarParametro(commandBorarResultadoSuscripcionViejas, IBD.ToParam("fechaModificacion"), DbType.DateTime, DateTime.Today.AddDays(pCaducidadResSusc * -1));
            ActualizarBaseDeDatos(commandBorarResultadoSuscripcionViejas);
        }

        /// <summary>
        /// Obtiene los elementos de la cola que hay que procesar.
        /// </summary>
        /// <returns>DataSet con la tabla cola cargada</returns>
        public ResultadosSuscripcionDS ObtenerElementosColaProcesar()
        {
            ResultadosSuscripcionDS resulSuscpDS = new ResultadosSuscripcionDS();

            DbCommand comandoSelCola = ObtenerComando(sqlSelectColaSuscripciones + " WHERE NumIntentos < 6");
            CargarDataSet(comandoSelCola, resulSuscpDS, "ColaSuscripciones");

            return resulSuscpDS;
        }

        /// <summary>
        /// Genera las filas de "ResultadosSuscripcion" y "ResSuscAgCatTes" de aquellos resultados nuevos que se han producido desde la última vez que el servicio los generó(pFechaUltimaEjecucion) hasta el momento actual.
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pFechaUltimaEjecucion">Fecha de la última ejecución del servicio para generar los resultados de suscripción nuevos</param>
        public void CrearResultSuscNuevasDePerfil(Guid pPerfilID, DateTime pFechaUltimaEjecucion)
        {
            ResultadosSuscripcionDS resSuscDS = new ResultadosSuscripcionDS();  //Para los que han sido editados
            ResultadosSuscripcionDS resSusc2DS = new ResultadosSuscripcionDS(); // Para los que han sido compartidos
            ResultadosSuscripcionDS resSuscAUXDS = new ResultadosSuscripcionDS(); //Para comprobar contra la BD antes de insertar

            #region Tabla ResultadoSuscripcion

            #region Si se ha modificado el recurso

            #region Parte de suscripciones a personas

            //Personas
            string selectConsultaGlobal = "SELECT DISTINCT SuscripcionTesauroUsuario.SuscripcionID,  Documento.DocumentoID RecursoID, Documento.Titulo, Documento.Descripcion, 2 TipoResultado, Documento.FechaModificacion ,  IdentCreador.IdentidadID AutorID, Perfil.NombrePerfil OrigenNombre, Perfil.NombreCortoUsu OrigenNombreCorto, IdentCreador.IdentidadID OrigenID, Documento.Tipo TipoDocumento, Documento.Enlace, 0 Leido, 0 SinCaducidad ";

            string fromConsultaGlobal = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN ( select Distinct tmp_CatTesSuscrip.CategoriaInferiorID as CategoriaTesauroID from tmp_CatTesSuscrip ) tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN Perfil ON Perfil.PersonaID = Persona.PersonaID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN DocumentoWebVinBaseRecursos ON DocumentoWebAgCatTesauro.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID AND DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID ";

            string restoConsultaGlobal = " WHERE IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion  = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND Documento.FechaModificacion > " + IBD.ToParam("pFechaUltimaEjecucion") + " AND IdentCreador.IdentidadID = documentowebvinbaserecursos.identidadpublicacionID ";

            string condicionesDeJerarquia = " CategoriaTesauroID in (select  TesauroUsuario.CategoriaTesauroPublicoID from SuscripcionTesauroUsuario INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID =  Suscripcion.IdentidadID inner join TesauroUsuario on TesauroUsuario.TesauroID = SuscripcionTesauroUsuario.TesauroID where Identidad.PerfilID =  " + IBD.GuidParamValor("pPerfilID") + ")";

            IConsultaJerarquica iConsultaJerarquica = IBD.CrearConsultaJerarquicaDeGrafo(selectConsultaGlobal, fromConsultaGlobal, restoConsultaGlobal, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscrip", condicionesDeJerarquia, false, "CategoriaTesauro", "CategoriaTesauroID");


            //Personas --> Ejemplo de como quedaria la consulta montada (no borrar para poder entender su analisis)
            //string sqlResultSuscPersonas = "

            //    WITH tmp_CatTesSuscrip (CategoriaSuperiorID, CategoriaInferiorID) as
            //    (
            //      Select CategoriaSuperiorID, CategoriaInferiorID from CatTesauroAgCatTesauro
            //      where CategoriaSuperiorID in (select  TesauroUsuario.CategoriaTesauroPublicoID from SuscripcionTesauroUsuario
            //                INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID
            //                INNER JOIN Identidad ON Identidad.IdentidadID =  Suscripcion.IdentidadID
            //               inner join TesauroUsuario on TesauroUsuario.TesauroID = SuscripcionTesauroUsuario.TesauroID
            //               where Identidad.PerfilID =    '{3110fc3c-25ea-49e4-9190-41d1ddbdbab3}')

            //      union all 

            //       Select CatTesauroAgCatTesauro.CategoriaSuperiorID, CatTesauroAgCatTesauro.CategoriaInferiorID from CatTesauroAgCatTesauro
            //       inner join tmp_CatTesSuscrip
            //       on tmp_CatTesSuscrip.CategoriaInferiorID = CatTesauroAgCatTesauro.CategoriaSuperiorID
            //    )
            //SELECT DISTINCT SuscripcionTesauroUsuario.SuscripcionID,
            //  Documento.DocumentoID RecursoID,
            //  Documento.Titulo,
            //  Documento.Descripcion,
            //  2 TipoResultado,
            //  Documento.FechaModificacion ,
            //  IdentCreador.IdentidadID AutorID,
            //  Perfil.NombrePerfil OrigenNombre,
            //  Perfil.NombreCortoUsu OrigenNombreCorto,
            //  IdentCreador.IdentidadID OrigenID,
            //  Documento.Tipo TipoDocumento,
            //  Documento.Enlace,
            //  0 Leido,
            //  0 SinCaducidad
            //FROM Documento
            //INNER JOIN DocumentoWebAgCatTesauro
            //ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID
            //INNER JOIN
            //  (
            //    select Distinct tmp_CatTesSuscrip.CategoriaInferiorID as CategoriaTesauroID from tmp_CatTesSuscrip
            //    union 
            //    select Distinct tmp_CatTesSuscrip.CategoriaSuperiorID as CategoriaTesauroID from tmp_CatTesSuscrip
            //  ) tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID
            //INNER JOIN SuscripcionTesauroUsuario
            //ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID
            //INNER JOIN Persona
            //ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID
            //INNER JOIN Perfil
            //ON Perfil.PersonaID = Persona.PersonaID
            //INNER JOIN Identidad IdentCreador
            //ON IdentCreador.PerfilID = Perfil.PerfilID
            //INNER JOIN DocumentoWebVinBaseRecursos
            //ON DocumentoWebAgCatTesauro.DocumentoID     = DocumentoWebVinBaseRecursos.DocumentoID
            //AND DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID
            //WHERE IdentCreador.ProyectoID                 = '11111111-1111-1111-1111-111111111111'
            //AND Documento.Borrador                        = 0
            //AND Documento.UltimaVersion                   = 1
            //AND Documento.Eliminado                       = 0
            //AND DocumentoWebVinBaseRecursos.Eliminado     = 0
            //AND Documento.FechaModificacion               > '2010-19-02'
            //AND IdentCreador.IdentidadID = documentowebvinbaserecursos.identidadpublicacionID


                    #endregion

            #region Parte de suscripciones a blogs

            //Blog
            string sqlResultSuscBlog = "SELECT DISTINCT " +
                                                "Suscripcion.SuscripcionID, " +
                                                "EntradaBlog.EntradaBlogID RecursoID, " +
                                                "EntradaBlog.Titulo, " +
                                                "EntradaBlog.Texto Descripcion, " +
                                                (short)TipoResultadoSuscripcion.EntradaBlogPersona + " TipoResultado, " +
                                                "EntradaBlog.FechaModificacion , " +
                                                "EntradaBlog.AutorID , " +
                                                "Blog.Titulo OrigenNombre, " +
                                                "Blog.NombreCorto OrigenNombreCorto, " +
                                                "Blog.BlogID OrigenID, " +
                                                (short)TiposDocumentacion.EntradaBlog + " TipoDocumento, " +
                                                "NULL Enlace, " +
                                                "0 Leido ," +
                                                "0 SinCaducidad " +
                                            "FROM EntradaBlog " +
                                                "INNER JOIN Blog " +
                                                "ON EntradaBlog.BlogID = Blog.BlogID " +
                                                "INNER JOIN SuscripcionBlog " +
                                                "ON SuscripcionBlog.BlogID = Blog.BlogID " +
                                                "INNER JOIN Suscripcion " +
                                                "ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID " +
                                                "INNER JOIN Identidad " +
                                                "ON Identidad.IdentidadID  = Suscripcion.IdentidadID " +
                                            "WHERE " +
                                                "Suscripcion.Bloqueada = 0 " +
                                                "AND EntradaBlog.FechaModificacion > " + IBD.ToParam("pFechaUltimaEjecucion") + " " +
                                                "AND EntradaBlog.Borrador = 0 " +
                                                "AND EntradaBlog.Eliminado = 0 " +
                                                "AND Identidad.PerfilID = " + IBD.GuidParamValor("pPerfilID");
            #endregion

            #region Parte de suscripciones a proyectos

            //Proyectos
            string sqlResultSuscProyectos = "SELECT DISTINCT " +
                                                "tmp_CatTesSuscrip.SuscripcionID, " +
                                                "Documento.DocumentoID RecursoID, " +
                                                "Documento.Titulo, " +
                                                "Documento.Descripcion, " +
                                                (short)TipoResultadoSuscripcion.RecursoComunidad + " TipoResultado, " +
                                                "Documento.FechaModificacion , " +
                                                "NULL AutorID, " +
                                                "Proyecto.Nombre OrigenNombre, " +
                                                "Proyecto.NombreCorto OrigenNombreCorto, " +
                                                "Proyecto.ProyectoID OrigenID, " +
                                                "Documento.Tipo TipoDocumento, " +
                                                "Documento.Enlace, " +
                                                "0 Leido, " +
                                                "0 SinCaducidad " +
                                            "FROM Documento " +
                                               "INNER JOIN DocumentoWebAgCatTesauro " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID " +
                                               "INNER JOIN " +
                                                    "(SELECT CategoriaTesauroID, Suscripcion.SuscripcionID " +
                                                       "FROM categoriaTesVinSuscrip " +
                                                         "INNER JOIN Suscripcion " +
                                                         "ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID " +
                                                         "INNER JOIN Identidad " +
                                                         "ON Identidad.IdentidadID = Suscripcion.IdentidadID " +
                                                      "WHERE Suscripcion.Bloqueada  = 0 " +
                                                      " AND Identidad.PerfilID = " + IBD.GuidParamValor("pPerfilID") +
                                                    " )tmp_CatTesSuscrip " +
                                               "ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID " +
                                               "INNER JOIN TesauroProyecto " +
                                               "ON DocumentoWebAgCatTesauro.TesauroID = TesauroProyecto.TesauroID " +
                                               "INNER JOIN Proyecto " +
                                               "ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID " +
                                               "INNER JOIN DocumentoWebVinBaseRecursos " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID " +
                                               "AND  DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID " +
                                               "INNER JOIN BaseRecursosProyecto " +
                                               "ON DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID " +
                                            "WHERE " +
                                                "Documento.Borrador = 0 " +
                                                "AND Documento.UltimaVersion = 1 " +
                                                "AND Documento.Eliminado = 0 " +
                                                "AND DocumentoWebVinBaseRecursos.Eliminado = 0 " +
                                                "AND Documento.FechaModificacion > " + IBD.ToParam("pFechaUltimaEjecucion");
            #endregion

            DbCommand commandObtenerResultadosSuscNuevos = ObtenerComando(iConsultaJerarquica.ConsultaJerarquica + " UNION ALL " + sqlResultSuscBlog + " UNION ALL " + sqlResultSuscProyectos);

            #endregion

            AgregarParametro(commandObtenerResultadosSuscNuevos, IBD.ToParam("pFechaUltimaEjecucion"), DbType.DateTime, pFechaUltimaEjecucion.AddMinutes(-2));
            AgregarParametro(commandObtenerResultadosSuscNuevos, IBD.GuidParamValor("pPerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            CargarDataSet(commandObtenerResultadosSuscNuevos, resSuscDS, "ResultadoSuscripcion");

            #region Si se ha compartido el recurso

            #region Parte de suscripciones a personas

            //Personas
            string selectConsultaGlobal2 = "SELECT DISTINCT SuscripcionTesauroUsuario.SuscripcionID,  Documento.DocumentoID RecursoID, Documento.Titulo, Documento.Descripcion, 2 TipoResultado, DocumentoWebVinBaseRecursos.FechaPublicacion as FechaModificacion,  IdentCreador.IdentidadID AutorID, Perfil.NombrePerfil OrigenNombre, Perfil.NombreCortoUsu OrigenNombreCorto, IdentCreador.IdentidadID OrigenID, Documento.Tipo TipoDocumento, Documento.Enlace, 0 Leido, 0 SinCaducidad ";

            string fromConsultaGlobal2 = " FROM Documento INNER JOIN DocumentoWebAgCatTesauro ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID INNER JOIN ( select Distinct tmp_CatTesSuscrip.CategoriaInferiorID as CategoriaTesauroID from tmp_CatTesSuscrip ) tmp_CatTesSuscrip ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID INNER JOIN SuscripcionTesauroUsuario ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID INNER JOIN Persona ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID INNER JOIN Perfil ON Perfil.PersonaID = Persona.PersonaID INNER JOIN Identidad IdentCreador ON IdentCreador.PerfilID = Perfil.PerfilID INNER JOIN DocumentoWebVinBaseRecursos ON DocumentoWebAgCatTesauro.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID AND DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID ";

            string restoConsultaGlobal2 = " WHERE IdentCreador.ProyectoID = '" + ProyectoAD.MetaProyecto + "' AND Documento.Borrador = 0 AND Documento.UltimaVersion  = 1 AND Documento.Eliminado = 0 AND DocumentoWebVinBaseRecursos.Eliminado = 0 AND DocumentoWebVinBaseRecursos.FechaPublicacion > " + IBD.ToParam("pFechaUltimaEjecucion") + " AND IdentCreador.IdentidadID = documentowebvinbaserecursos.identidadpublicacionID ";

            string condicionesDeJerarquia2 = " CategoriaTesauroID in (select  TesauroUsuario.CategoriaTesauroPublicoID from SuscripcionTesauroUsuario INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = SuscripcionTesauroUsuario.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID =  Suscripcion.IdentidadID inner join TesauroUsuario on TesauroUsuario.TesauroID = SuscripcionTesauroUsuario.TesauroID where Identidad.PerfilID =  " + IBD.GuidParamValor("pPerfilID") + ")";

            IConsultaJerarquica iConsultaJerarquica2 = IBD.CrearConsultaJerarquicaDeGrafo(selectConsultaGlobal2, fromConsultaGlobal2, restoConsultaGlobal2, "CategoriaSuperiorID", "CategoriaInferiorID", "CatTesauroAgCatTesauro", null, null, "tmp_CatTesSuscrip", condicionesDeJerarquia2, false, "CategoriaTesauro", "CategoriaTesauroID");

            #endregion

            #region Parte de suscripciones a proyectos

            //Proyectos
            string sqlResultSuscProyectos2 = "SELECT DISTINCT " +
                                                "tmp_CatTesSuscrip.SuscripcionID, " +
                                                "Documento.DocumentoID RecursoID, " +
                                                "Documento.Titulo, " +
                                                "Documento.Descripcion, " +
                                                (short)TipoResultadoSuscripcion.RecursoComunidad + " TipoResultado, " +
                                                "DocumentoWebVinBaseRecursos.FechaPublicacion as FechaModificacion , " +
                                                "NULL AutorID, " +
                                                "Proyecto.Nombre OrigenNombre, " +
                                                "Proyecto.NombreCorto OrigenNombreCorto, " +
                                                "Proyecto.ProyectoID OrigenID, " +
                                                "Documento.Tipo TipoDocumento, " +
                                                "Documento.Enlace, " +
                                                "0 Leido, " +
                                                "0 SinCaducidad " +
                                            "FROM Documento " +
                                               "INNER JOIN DocumentoWebAgCatTesauro " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID " +
                                               "INNER JOIN " +
                                                    "(SELECT CategoriaTesauroID, Suscripcion.SuscripcionID " +
                                                       "FROM categoriaTesVinSuscrip " +
                                                         "INNER JOIN Suscripcion " +
                                                         "ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID " +
                                                         "INNER JOIN Identidad " +
                                                         "ON Identidad.IdentidadID = Suscripcion.IdentidadID " +
                                                      "WHERE Suscripcion.Bloqueada  = 0 " +
                                                      " AND Identidad.PerfilID = " + IBD.GuidParamValor("pPerfilID") +
                                                    " )tmp_CatTesSuscrip " +
                                               "ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID " +
                                               "INNER JOIN TesauroProyecto " +
                                               "ON DocumentoWebAgCatTesauro.TesauroID = TesauroProyecto.TesauroID " +
                                               "INNER JOIN Proyecto " +
                                               "ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID " +
                                               "INNER JOIN DocumentoWebVinBaseRecursos " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID " +
                                               "AND  DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID " +
                                               "INNER JOIN BaseRecursosProyecto " +
                                               "ON DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID " +
                                            "WHERE " +
                                                "Documento.Borrador = 0 " +
                                                "AND Documento.UltimaVersion = 1 " +
                                                "AND Documento.Eliminado = 0 " +
                                                "AND DocumentoWebVinBaseRecursos.Eliminado = 0 " +
                                                "AND DocumentoWebVinBaseRecursos.FechaPublicacion > " + IBD.ToParam("pFechaUltimaEjecucion");
            #endregion

            DbCommand commandObtenerResultadosSuscNuevos2 = ObtenerComando(iConsultaJerarquica2.ConsultaJerarquica + " UNION ALL " + sqlResultSuscProyectos2);

            #endregion

            //El pFechaUltimaEjecucion.AddMinutes(-2) es para que optenga los resultados de las suscripciones desde la ultima vez que 
            //se ejecuto el servicio menos dos minutos, para que calcule las que puedan haberse generado en el intervalo de tiempo muerto 
            //que el servicio esta funcionando. Posteriormente al insertarla los resultados se comprobara para que no esten duplicados
            AgregarParametro(commandObtenerResultadosSuscNuevos2, IBD.ToParam("pFechaUltimaEjecucion"), DbType.DateTime, pFechaUltimaEjecucion.AddMinutes(-2));
            AgregarParametro(commandObtenerResultadosSuscNuevos2, IBD.GuidParamValor("pPerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            CargarDataSet(commandObtenerResultadosSuscNuevos2, resSusc2DS, "ResultadoSuscripcion");

            //Si el recurso ha sido editado y compartido, para que solo genere un resultado de suscripción se ignora la de compartido, priorizando la edición
            foreach (ResultadosSuscripcionDS.ResultadoSuscripcionRow filaResultadoSuscripcionEditada in resSuscDS.ResultadoSuscripcion.Rows)
            {
                if (resSusc2DS.ResultadoSuscripcion.FindBySuscripcionIDRecursoID(filaResultadoSuscripcionEditada.SuscripcionID, filaResultadoSuscripcionEditada.RecursoID) != null)
                {
                    resSusc2DS.ResultadoSuscripcion.FindBySuscripcionIDRecursoID(filaResultadoSuscripcionEditada.SuscripcionID, filaResultadoSuscripcionEditada.RecursoID).Delete();

                }
                resSusc2DS.AcceptChanges();
            }

            //UNO LOS RESULTADOS DE EDITADOS Y COMPARTIDOS
            resSuscDS.Merge(resSusc2DS);

            #endregion

            #region Tabla ResulSusAgCatTes

            #region Parte de categorias de tesauro vinculadas a suscripciones a personas

            //Personas
            string sqlResulSusAgCatTesPersonas = "SELECT DISTINCT " +
                                                "tmp_CatTesSuscrip.SuscripcionID, " +
                                                "Documento.DocumentoID RecursoID, " +
                                                "DocumentoWebAgCatTesauro.TesauroID, " +
                                                "DocumentoWebAgCatTesauro.CategoriaTesauroID, " +
                                                "CategoriaTesauro.Nombre " +
                                            "FROM Documento " +
                                            "INNER JOIN DocumentoWebAgCatTesauro " +
                                            "ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID " +
                                            "INNER JOIN (SELECT CategoriaTesauroID, Suscripcion.SuscripcionID from categoriaTesVinSuscrip " +
                                                         "INNER JOIN Suscripcion " +
                                                         "ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID " +
                                                         "INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID " +
                                                         "WHERE Identidad.PerfilID = " + IBD.GuidParamValor("pPerfilID") +
                                                         " AND Suscripcion.Bloqueada = 0) " +
                                            "tmp_CatTesSuscrip " +
                                            "ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID " +
                                            "INNER JOIN CategoriaTesauro " +
                                            "ON CategoriaTesauro.CategoriaTesauroID = DocumentoWebAgCatTesauro.CategoriaTesauroID " +
                                            "INNER JOIN SuscripcionTesauroUsuario " +
                                            "ON DocumentoWebAgCatTesauro.TesauroID = SuscripcionTesauroUsuario.TesauroID " +
                                            "INNER JOIN Persona " +
                                            "ON SuscripcionTesauroUsuario.UsuarioID = Persona.UsuarioID " +
                                            "INNER JOIN Perfil " +
                                            "ON Perfil.PersonaID = Persona.PersonaID " +
                                            "INNER JOIN Identidad IdentCreador " +
                                            "ON IdentCreador.PerfilID = Perfil.PerfilID " +
                                            "INNER JOIN DocumentoWebVinBaseRecursos " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID " +
                                               "AND  DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID " +
                                            "INNER JOIN BaseRecursosUsuario " +
                                            "ON   DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosUsuario.BaseRecursosID " +
                                            "WHERE " +
                                                "IdentCreador.ProyectoID = '" + IBD.ValorDeGuid(ProyectoAD.MetaProyecto) + "' " +
                                                "AND Documento.Borrador = 0 " +
                                                "AND Documento.UltimaVersion = 1 " +
                                                "AND Documento.Eliminado = 0 " +
                                                "AND DocumentoWebVinBaseRecursos.Eliminado = 0 " +
                                                "AND ( Documento.FechaModificacion > " + IBD.ToParam("pFechaUltimaEjecucion")+
                                                " OR DocumentoWebVinBaseRecursos.FechaPublicacion  > " + IBD.ToParam("pFechaUltimaEjecucion")                                               + " )";
            #endregion

            #region Parte de categorias de tesauro vinculadas a suscripciones a blogs

            //Blog
            string sqlResulSusAgCatTesBlog = "SELECT DISTINCT " +
                                                "Suscripcion.SuscripcionID, " +
                                                "EntradaBlogAgCatTesauro.EntradaBlogID RecursoID, " +
                                                "EntradaBlogAgCatTesauro.CategoriaTesauroID, " +
                                                "EntradaBlogAgCatTesauro.TesauroID, " +
                                                "CategoriaTesauro.Nombre " +
                                            "FROM EntradaBlog " +
                                                "INNER JOIN SuscripcionBlog " +
                                                "ON SuscripcionBlog.BlogID = EntradaBlog.BlogID " +
                                                "INNER JOIN EntradaBlogAgCatTesauro " +
                                                "ON EntradaBlogAgCatTesauro.EntradaBlogID = EntradaBlog.EntradaBlogID " +
                                                "INNER JOIN CategoriaTesauro " +
                                                "ON CategoriaTesauro.CategoriaTesauroID = EntradaBlogAgCatTesauro.CategoriaTesauroID " +
                                                "INNER JOIN Suscripcion " +
                                                "ON Suscripcion.SuscripcionID = SuscripcionBlog.SuscripcionID " +
                                                "INNER JOIN Identidad " +
                                                "ON Identidad.IdentidadID  = Suscripcion.IdentidadID " +
                                            "WHERE " +
                                                "Suscripcion.Bloqueada = 0 " +
                                                "AND EntradaBlog.FechaModificacion > " + IBD.ToParam("pFechaUltimaEjecucion") + " " +
                                                "AND EntradaBlog.Borrador = 0 " +
                                                "AND EntradaBlog.Eliminado = 0 " +
                                                "AND Identidad.PerfilID = " + IBD.GuidParamValor("pPerfilID");
            #endregion

            #region Parte de categorias de tesauro vinculadas a suscripciones a proyectos

            //Proyectos
            string sqlResulSusAgCatTesProyectos = "SELECT DISTINCT " +
                                                "tmp_CatTesSuscrip.SuscripcionID, " +
                                                "DocumentoWebAgCatTesauro.DocumentoID RecursoID, " +
                                                "DocumentoWebAgCatTesauro.TesauroID, " +
                                                "DocumentoWebAgCatTesauro.CategoriaTesauroID, " +
                                                "CategoriaTesauro.Nombre " +
                                            "FROM Documento " +
                                               "INNER JOIN DocumentoWebAgCatTesauro " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = Documento.DocumentoID " +
                                               "INNER JOIN " +
                                                    "(SELECT CategoriaTesauroID, Suscripcion.SuscripcionID " +
                                                       "FROM categoriaTesVinSuscrip " +
                                                         "INNER JOIN Suscripcion " +
                                                         "ON Suscripcion.SuscripcionID = categoriaTesVinSuscrip.SuscripcionID " +
                                                         "INNER JOIN Identidad " +
                                                         "ON Identidad.IdentidadID = Suscripcion.IdentidadID " +
                                                      "WHERE Suscripcion.Bloqueada  = 0 " +
                                                      " AND Identidad.PerfilID = " + IBD.GuidParamValor("pPerfilID") +
                                                    " )tmp_CatTesSuscrip " +
                                               "ON DocumentoWebAgCatTesauro.CategoriaTesauroID = tmp_CatTesSuscrip.CategoriaTesauroID " +
                                               "INNER JOIN CategoriaTesauro " +
                                               "ON DocumentoWebAgCatTesauro.CategoriaTesauroID = CategoriaTesauro.CategoriaTesauroID " +
                                               "INNER JOIN TesauroProyecto " +
                                               "ON DocumentoWebAgCatTesauro.TesauroID = TesauroProyecto.TesauroID " +
                                               "INNER JOIN Proyecto " +
                                               "ON TesauroProyecto.ProyectoID = Proyecto.ProyectoID " +
                                               "INNER JOIN DocumentoWebVinBaseRecursos " +
                                               "ON DocumentoWebAgCatTesauro.DocumentoID = DocumentoWebVinBaseRecursos.DocumentoID " +
                                               "AND  DocumentoWebAgCatTesauro.BaseRecursosID = DocumentoWebVinBaseRecursos.BaseRecursosID " +
                                               "INNER JOIN BaseRecursosProyecto " +
                                               "ON DocumentoWebVinBaseRecursos.BaseRecursosID = BaseRecursosProyecto.BaseRecursosID " +
                                            "WHERE " +
                                                "Documento.Borrador = 0 " +
                                                "AND Documento.UltimaVersion = 1 " +
                                                "AND Documento.Eliminado = 0 " +
                                                "AND DocumentoWebVinBaseRecursos.Eliminado = 0 " +
                                                "AND ( Documento.FechaModificacion > " + IBD.ToParam("pFechaUltimaEjecucion") +
                                                " OR DocumentoWebVinBaseRecursos.FechaPublicacion  > " + IBD.ToParam("pFechaUltimaEjecucion")                                                + " )";
            #endregion

            DbCommand commandObtenerResulSusAgCatTesNuevos = ObtenerComando(sqlResulSusAgCatTesPersonas + " UNION ALL " + sqlResulSusAgCatTesBlog + " UNION ALL " + sqlResulSusAgCatTesProyectos);

            //El pFechaUltimaEjecucion.AddMinutes(-2) es para que optenga los resultados de las suscripciones desde la ultima vez que 
            //se ejecuto el servicio menos dos minutos, para que calcule las que puedan haberse generado en el intervalo de tiempo muerto 
            //que el servicio esta funcionando. Posteriormente al insertarla los resultados se comprobara para que no esten duplicados
            AgregarParametro(commandObtenerResulSusAgCatTesNuevos, IBD.ToParam("pFechaUltimaEjecucion"), DbType.DateTime, pFechaUltimaEjecucion.AddMinutes(-2));
            AgregarParametro(commandObtenerResulSusAgCatTesNuevos, IBD.GuidParamValor("pPerfilID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(pPerfilID));
            CargarDataSet(commandObtenerResulSusAgCatTesNuevos, resSuscDS, "ResulSusAgCatTes");

            #endregion

            #region Comprobacion contra la BD

            //Compruebo cada fila de resultados nuevos contra la base de datos para antes de insertar ver que no exista ya
            foreach (ResultadosSuscripcionDS.ResultadoSuscripcionRow filaResultadoSuscripcion in resSuscDS.ResultadoSuscripcion.Rows)
            {
                object existe = null;
                DbCommand commandsqlSelectExisteResultadoSusc = ObtenerComando("SELECT 1 FROM ResultadoSuscripcion WHERE ResultadoSuscripcion.SuscripcionID = " + IBD.GuidParamValor("pSuscripcionID") + " AND ResultadoSuscripcion.RecursoID = " + IBD.GuidParamValor("pRecursoID"));
                AgregarParametro(commandsqlSelectExisteResultadoSusc, IBD.ToParam("pSuscripcionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResultadoSuscripcion.SuscripcionID));
                AgregarParametro(commandsqlSelectExisteResultadoSusc, IBD.ToParam("pRecursoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResultadoSuscripcion.RecursoID));
                existe = EjecutarEscalar(commandsqlSelectExisteResultadoSusc);

                if (existe == null)
                {
                    filaResultadoSuscripcion.SetAdded();
                    resSuscAUXDS.ResultadoSuscripcion.ImportRow(filaResultadoSuscripcion);
                }
                else
                {
                    DbCommand commandsqlUpdateResultadoSusc = ObtenerComando("UPDATE ResultadoSuscripcion SET Enlace = " + IBD.GuidParamValor("Enlace") + ", Descripcion = " + IBD.GuidParamValor("Descripcion") + ", Titulo = " + IBD.GuidParamValor("Titulo") + " WHERE ResultadoSuscripcion.SuscripcionID = " + IBD.GuidParamValor("pSuscripcionID") + " AND ResultadoSuscripcion.RecursoID = " + IBD.GuidParamValor("pRecursoID"));
                    AgregarParametro(commandsqlUpdateResultadoSusc, IBD.ToParam("pSuscripcionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResultadoSuscripcion.SuscripcionID));
                    AgregarParametro(commandsqlUpdateResultadoSusc, IBD.ToParam("pRecursoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResultadoSuscripcion.RecursoID));
                    AgregarParametro(commandsqlUpdateResultadoSusc, IBD.ToParam("Descripcion"), DbType.String, filaResultadoSuscripcion.Descripcion);
                    AgregarParametro(commandsqlUpdateResultadoSusc, IBD.ToParam("Enlace"), DbType.String, filaResultadoSuscripcion.Enlace);
                    AgregarParametro(commandsqlUpdateResultadoSusc, IBD.ToParam("Titulo"), DbType.String, filaResultadoSuscripcion.Titulo);
                    ActualizarBaseDeDatos(commandsqlUpdateResultadoSusc);
                }
                commandsqlSelectExisteResultadoSusc.Dispose();
                commandsqlSelectExisteResultadoSusc = null;
            }

            //Compruebo cada fila de vinculaciones deresultados nuevos con categorias de tesauro contra la base de datos para antes de insertar ver q no exista ya
            foreach (ResultadosSuscripcionDS.ResulSusAgCatTesRow filaResulSusAgCatTes in resSuscDS.ResulSusAgCatTes.Rows)
            {
                object existe = null;
                DbCommand commandsqlSelectExisteResulSusAgCatTes = ObtenerComando("SELECT 1 FROM ResulSusAgCatTes WHERE ResulSusAgCatTes.SuscripcionID = " + IBD.GuidParamValor("pSuscripcionID") + " AND ResulSusAgCatTes.CategoriaTesauroID = " + IBD.GuidParamValor("pCategoriaTesauroID") + " AND ResulSusAgCatTes.TesauroID = " + IBD.GuidParamValor("pTesauroID") + " AND ResulSusAgCatTes.RecursoID = " + IBD.GuidParamValor("pRecursoID"));
                AgregarParametro(commandsqlSelectExisteResulSusAgCatTes, IBD.ToParam("pSuscripcionID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResulSusAgCatTes.SuscripcionID));
                AgregarParametro(commandsqlSelectExisteResulSusAgCatTes, IBD.ToParam("pRecursoID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResulSusAgCatTes.RecursoID));
                AgregarParametro(commandsqlSelectExisteResulSusAgCatTes, IBD.ToParam("pTesauroID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResulSusAgCatTes.TesauroID));
                AgregarParametro(commandsqlSelectExisteResulSusAgCatTes, IBD.ToParam("pCategoriaTesauroID"), IBD.TipoGuidToString(DbType.Guid), IBD.ValorDeGuid(filaResulSusAgCatTes.CategoriaTesauroID));
                existe = EjecutarEscalar(commandsqlSelectExisteResulSusAgCatTes);

                if (existe == null)
                {
                    filaResulSusAgCatTes.SetAdded();
                    resSuscAUXDS.ResulSusAgCatTes.ImportRow(filaResulSusAgCatTes);
                }
                commandsqlSelectExisteResulSusAgCatTes.Dispose();
                commandsqlSelectExisteResulSusAgCatTes = null;
            }
            #endregion

            #region Guardo contra la BD

            if (resSuscAUXDS.ResultadoSuscripcion.Rows.Count > 0)
            {
                this.GuardarActualizaciones(resSuscAUXDS);
            }
            #endregion

            resSuscAUXDS.Dispose();
            resSuscDS.Dispose();
            resSuscAUXDS = null;
            resSuscDS = null;
        }

        /// <summary>
        /// Obtiene las filas de "ResultadosSuscripcion" y "ResSuscAgCatTes" de aquellos resultados de un determinado perfil que no deban estar en su bandeja de suscripciones debidoa su antiguedad y tiempo de permanencia (pCaducidadResSusc), nunca eliminará los resultados  marcados como no caducos
        /// </summary>
        /// <param name="pPerfilID">Identificador del perfil</param>
        /// <param name="pCaducidadResSusc">Periodo (7 o 30 días) de permanencia del los resultados de suscripciones en la bandeja</param>
        /// <returns>DataSet con las filas caducadas</returns>
        public ResultadosSuscripcionDS ObtenerResultSuscCaducadasDePerfil(Guid pPerfilID, int pCaducidadResSusc)
        {
            ResultadosSuscripcionDS resulSuscrpDS = new ResultadosSuscripcionDS();

            //ResulSusAgCatTes
            string sqlBorrarResulSusAgCatTesViejas = sqlSelectResulSusAgCatTes + " INNER JOIN ResultadoSuscripcion ON ResultadoSuscripcion.SuscripcionID = ResulSusAgCatTes.SuscripcionID AND ResultadoSuscripcion.RecursoID = ResulSusAgCatTes.RecursoID INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE Identidad.PerfilID = " + IBD.GuidValor(pPerfilID) + " AND ResultadoSuscripcion.FechaModificacion < " + IBD.ToParam("fechaModificacion") + " AND ResultadoSuscripcion.Sincaducidad = 0 ";
            DbCommand commandBorrarResulSusAgCatTesViejas = ObtenerComando(sqlBorrarResulSusAgCatTesViejas);
            AgregarParametro(commandBorrarResulSusAgCatTesViejas, IBD.ToParam("fechaModificacion"), DbType.DateTime, DateTime.Today.AddDays(pCaducidadResSusc * -1));
            CargarDataSet(commandBorrarResulSusAgCatTesViejas, resulSuscrpDS, "ResulSusAgCatTes");

            //ResultadoSuscripcion
            string sqlBorrarResultadoSuscripcionViejas = sqlSelectResultadoSuscripcion + " INNER JOIN Suscripcion ON Suscripcion.SuscripcionID = ResultadoSuscripcion.SuscripcionID INNER JOIN Identidad ON Identidad.IdentidadID = Suscripcion.IdentidadID WHERE Identidad.PerfilID = " + IBD.GuidValor(pPerfilID) + " AND ResultadoSuscripcion.FechaModificacion < " + IBD.ToParam("fechaModificacion") + " AND ResultadoSuscripcion.Sincaducidad = 0 ";
            DbCommand commandBorarResultadoSuscripcionViejas = ObtenerComando(sqlBorrarResultadoSuscripcionViejas);
            AgregarParametro(commandBorarResultadoSuscripcionViejas, IBD.ToParam("fechaModificacion"), DbType.DateTime, DateTime.Today.AddDays(pCaducidadResSusc * -1));
            CargarDataSet(commandBorarResultadoSuscripcionViejas, resulSuscrpDS, "ResultadoSuscripcion");

            return resulSuscrpDS;
        }

        #endregion

        #endregion
    }
}
